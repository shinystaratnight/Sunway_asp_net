Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Xml
Imports Intuitive.DateFunctions
Imports Intuitive.Functions
Imports System.Text
Imports Microsoft.SqlServer.Server

''' <summary>
''' Class containing functions execute sql statements
''' </summary>
Public Class SQL

	''' <summary>
	''' Object used for synclocking the cache building process
	''' </summary>
	Private Shared oLoadBalancedConnectStringLock As New Object

#Region "connect string"

	''' <summary>
	''' Gets connect string
	''' </summary>
	Public Shared Function ConnectString() As String
		Return SQL.ConnectString("")
	End Function

	''' <summary>
	''' Gets the connect string, first get's override connect string, then gets the config connect string, if the connect string begins with LoadBalanced, 
	''' gets a loadbalanced connect string using the specified sessionkey
	''' </summary>
	''' <param name="SessionKey">The session key, used for load balanced connect string.</param>
	Public Shared Function ConnectString(SessionKey As String) As String

		'check for override connect string
		Dim sConnectString As String = ""
		If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("overrideconnectstring") IsNot Nothing Then
			sConnectString = HttpContext.Current.Session("overrideconnectstring").ToString
		End If

		'retrieve config connect string
		If sConnectString = "" Then
			sConnectString = AppSettings.Get("ConnectString").ToSafeString()
		End If

		Return sConnectString
	End Function

	''' <summary>
	''' Sets the override connect string on the session.
	''' </summary>
	''' <param name="ConnectString">The overriding connect string.</param>
	''' <exception cref="System.Exception">Session Cannot be found</exception>
	Public Shared Sub SetOverrideConnectString(ConnectString As String)

		If Not System.Web.HttpContext.Current Is Nothing Then
			System.Web.HttpContext.Current.Session("overrideconnectstring") = ConnectString
		Else
			Throw New Exception("Session Cannot be found")
		End If
	End Sub

	''' <summary>
	''' Removes the override connect string from the session.
	''' </summary>
	Public Shared Sub ClearOverrideConnectString()
		If Not System.Web.HttpContext.Current Is Nothing AndAlso Not System.Web.HttpContext.Current.Session("overrideconnectstring") Is Nothing Then
			System.Web.HttpContext.Current.Session.Remove("overrideconnectstring")
		End If
	End Sub

#End Region

#Region "Software load-balancing"

	''' <summary>
	''' Sends an email about a failure to retrieve a connect string from a pool
	''' </summary>
	''' <param name="PoolName">Name of the pool.</param>
	Public Shared Sub SendPoolConnectStringEmail(PoolName As String)

		Dim sMachineName As String = Environment.MachineName
		Dim sEmailTo As String = SafeString(AppSettings("ConnectStringErrorEmailAddress"))

		Dim oEmail As New Email
		With oEmail
			.SMTPHost = SafeString(AppSettings("SMTPHost"))
			.Subject = "Connect string failure"
			.From = "Intuitive Admin"
			.FromEmail = "admin@intuitivesystems.com"
			.EmailTo = If(sEmailTo <> "", sEmailTo, "Infrastructure@intuitivesystems.co.uk")
			.Body = String.Format("A connect string for pool id {0} could not be found on {1}", PoolName, sMachineName)
		End With

		oEmail.SendEmail()
	End Sub

	''' <summary>
	''' Class representing a list of connect string options
	''' </summary>
	''' <seealso cref="System.Collections.Generic.List(Of T)" />
	Public Class ConnectStringOptions
		Inherits Generic.List(Of ConnectStringOption)

		''' <summary>
		''' Gets or sets the time an instance of this connect string options was created.
		''' </summary>
		Public Property StartTime As Date

		''' <summary>
		''' Gets how long the connect string options has been in the cache in minutes.
		''' </summary>
		Public Property MinutesInCache As Integer
			Get
				Dim oTimespan As TimeSpan = Date.Now.Subtract(Me.StartTime)
				Return oTimespan.Minutes
			End Get
			Set(value As Integer)
			End Set
		End Property

		''' <summary>
		''' Initializes a new instance of the <see cref="ConnectStringOptions"/> class, sets the StartTime to the current time.
		''' </summary>
		Public Sub New()
			Me.StartTime = Date.Now
		End Sub

		''' <summary>
		''' Adds a new <see cref="ConnectString"/> to the list.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="ConnectProbability">The probability for this connect string to be used.</param>
		''' <param name="MinPercentage">The minimum percentage required for the string to be used, higher reduces the chance it will be used.</param>
		''' <param name="MaxPercentage">The maximum percentage required for the string to be used, lower reduces the chance it will be used.</param>
		Public Sub AddNew(ConnectString As String, ConnectProbability As Decimal, MinPercentage As Decimal, MaxPercentage As Decimal)
			Me.Add(New ConnectStringOption(ConnectString, ConnectProbability, MinPercentage, MaxPercentage))
		End Sub

		''' <summary>
		''' Gets the connect string.
		''' </summary>
		''' <param name="SessionKey">The session key.</param>
		Public Function GetConnectString(Optional ByVal SessionKey As String = "") As String

			Dim nPercentage As Decimal
			If SessionKey = "" Then
				nPercentage = SafeDecimal(Date.Now.Millisecond / 10)
			Else
				Dim sHashCode As String = SessionKey.GetHashCode.ToString
				nPercentage = SafeDecimal(sHashCode.Substring(sHashCode.Length - 2, 2).ToSafeInt)
			End If

			For Each oConnectStringOption As ConnectStringOption In Me

				If nPercentage >= oConnectStringOption.MinPercentage AndAlso (nPercentage < oConnectStringOption.MaxPercentage OrElse oConnectStringOption.MaxPercentage = 100) Then

					oConnectStringOption.Count += 1
					If SessionKey <> "" Then oConnectStringOption.HashCount += 1
					Return oConnectStringOption.ConnectString
				End If

			Next

			'nothing returned? return the first item
			Me(0).Count += 1
			If SessionKey <> "" Then Me(0).HashCount += 1
			Return Me(0).ConnectString
		End Function
	End Class

	''' <summary>
	''' Class representing a connect string.
	''' </summary>
	Public Class ConnectStringOption
		''' <summary>
		''' The connect string
		''' </summary>
		Public ConnectString As String

		''' <summary>
		''' The connect probability
		''' </summary>
		Public ConnectProbability As Decimal

		''' <summary>
		''' The minimum percentage
		''' </summary>
		Public MinPercentage As Decimal

		''' <summary>
		''' The maximum percentage
		''' </summary>
		Public MaxPercentage As Decimal

		''' <summary>
		''' The count
		''' </summary>
		Public Count As Integer = 0

		''' <summary>
		''' The hash count
		''' </summary>
		Public HashCount As Integer = 0

		'just returns the server
		''' <summary>
		''' Gets the connect string server from the <see cref="connectstring"/>.
		''' </summary>
		Public Property ConnectStringServer As String
			Get
				Return Me.ConnectString.Split(";"c)(0).Split("="c)(1)
			End Get
			Set(value As String)
			End Set
		End Property

		''' <summary>
		''' Initializes a new instance of the <see cref="ConnectStringOption"/> class.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="ConnectStringOption"/> class.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="ConnectProbability">The probability the connect string will be used.</param>
		''' <param name="MinPercentage">The minimum percentage required for the string to be used, higher reduces the chance it will be used.</param>
		''' <param name="MaxPercentage">The maximum percentage required for the string to be used, lower reduces the chance it will be used.</param>
		Public Sub New(ConnectString As String, ConnectProbability As Decimal, MinPercentage As Decimal, MaxPercentage As Decimal)

			Me.ConnectString = ConnectString
			Me.ConnectProbability = ConnectProbability
			Me.MinPercentage = MinPercentage
			Me.MaxPercentage = MaxPercentage
		End Sub
	End Class

	''' <summary>
	''' Gets loadbalanced connect strings as a <see cref="ConnectStringOptions"/> object.
	''' </summary>
	Public Shared Function GetConnectStringData() As ConnectStringOptions
		Return GetCache(Of ConnectStringOptions)("SQL_LoadbalancedConnectString")
	End Function

#End Region

#Region "Support: FKCheck, DupeCheck, CheckSPExists, Convertdatatabletoarray, BuildConnectString, Datatabletocsv, FormatStatement, FlushDatarowtoobject"

	''' <summary>
	''' Converts the data table to array.
	''' </summary>
	''' <param name="oDataTable">The data table to convert.</param>
	Public Shared Function ConvertDataTableToArray(oDataTable As DataTable) As Object(,)
		Dim iRows As Integer
		Dim iColumns As Integer
		Dim i As Integer
		Dim j As Integer

		'work out number of rows and columns and create the array
		iRows = oDataTable.Rows.Count
		iColumns = oDataTable.Columns.Count
		Dim aReturn(iRows - 1, iColumns - 1) As Object
		For i = 0 To iRows - 1
			For j = 0 To iColumns - 1
				aReturn(i, j) = oDataTable.Rows(i).Item(j)
			Next
		Next

		Return aReturn
	End Function

	''' <summary>
	''' Checks tables for references to another table, if none exist, returns true
	''' </summary>
	''' <param name="sTable">The table to check for references.</param>
	''' <param name="sField">The field to check against.</param>
	''' <param name="iID">The identifier of the field.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">
	''' Table must be specified
	''' or
	''' Field must be specified
	''' or
	''' ID must be specified
	''' </exception>
	Public Overloads Shared Function FKCheck(sTable As String, sField As String, iID As Integer, Optional ConnectString As String = "") As Boolean

		Dim sSql As String
		Dim iCount As Integer

		If sTable = "" Then
			Throw New Exception("Table must be specified")
		ElseIf sField = "" Then
			Throw New Exception("Field must be specified")
		ElseIf iID = 0 Then
			Throw New Exception("ID must be specified")
		End If

		sSql = String.Format("Select count(*) " & "from {0} where {1}={2} ", sTable, sField, iID)

		If ConnectString = "" Then
			iCount = SQL.ExecuteSingleValue(sSql)
		Else
			iCount = SQL.ExecuteSingleValueWithConnectString(ConnectString, sSql)
		End If

		Return iCount = 0
	End Function

	''' <summary>
	''' Checks table for reference to both specified fields with the specified values, if none are found, returns true.
	''' </summary>
	''' <param name="sTable">The table to check.</param>
	''' <param name="sField1">The first field to check.</param>
	''' <param name="iID1">The first id to check.</param>
	''' <param name="sField2">The second field to check.</param>
	''' <param name="iID2">The second id to check.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">
	''' Table must be specified
	''' or
	''' Field 1 must be specified
	''' or
	''' ID 1 must be specified
	''' or
	''' Field 2 must be specified
	''' or
	''' ID 2 must be specified
	''' </exception>
	Public Overloads Shared Function FKCheck(sTable As String, sField1 As String, iID1 As Integer, sField2 As String, iID2 As Integer, Optional ConnectString As String = "") As Boolean

		Dim sSql As String
		Dim iCount As Integer

		If sTable = "" Then
			Throw New Exception("Table must be specified")
		ElseIf sField1 = "" Then
			Throw New Exception("Field 1 must be specified")
		ElseIf iID1 = 0 Then
			Throw New Exception("ID 1 must be specified")
		ElseIf sField2 = "" Then
			Throw New Exception("Field 2 must be specified")
		ElseIf iID2 = 0 Then
			Throw New Exception("ID 2 must be specified")
		End If

		sSql = String.Format("Select count(*) " & "from {0} where {1}={2} " & "and {3}={4}", sTable, sField1, iID1, sField2, iID2)

		If ConnectString = "" Then
			iCount = SQL.ExecuteSingleValue(sSql)
		Else
			iCount = SQL.ExecuteSingleValueWithConnectString(ConnectString, sSql)
		End If

		Return iCount = 0
	End Function

	''' <summary>
	''' Dupes the check.
	''' </summary>
	''' <param name="sTable">The s table.</param>
	''' <param name="sField">The s field.</param>
	''' <param name="sValue">The s value.</param>
	''' <param name="iTableID">The i table identifier.</param>
	''' <param name="sParentField">The s parent field.</param>
	''' <param name="sParentValue">The s parent value.</param>
	Public Shared Function DupeCheck(sTable As String, sField As String, sValue As String, iTableID As Integer, Optional ByVal sParentField As String = "", Optional ByVal sParentValue As String = "") As Boolean

		Return SQL.DupeCheckWithConnectString(SQL.ConnectString, sTable, sField, sValue, iTableID, sParentField, sParentValue)
	End Function

	''' <summary>
	''' Checks table for entries whose field has the same value as the value passed in.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sTable">The table to check for duplicates.</param>
	''' <param name="sField">The field to check the value of.</param>
	''' <param name="sValue">The value to check.</param>
	''' <param name="iTableID">The id of the entry in the table to check.</param>
	''' <param name="sParentField">Optional parent field to check.</param>
	''' <param name="sParentValue">Optional parent value to check.</param>
	''' <exception cref="System.Exception">
	''' Table must be specified
	''' or
	''' Field must be specified
	''' or
	''' Value must be specified
	''' </exception>
	Public Shared Function DupeCheckWithConnectString(ConnectString As String, sTable As String, sField As String, sValue As String, iTableID As Integer, Optional ByVal sParentField As String = "", Optional ByVal sParentValue As String = "") As Boolean

		Dim sSql As String
		Dim iCount As Integer

		If sTable = "" Then
			Throw New Exception("Table must be specified")
		ElseIf sField = "" Then
			Throw New Exception("Field must be specified")
		ElseIf sValue = "" Then
			Throw New Exception("Value must be specified")
		End If

		sSql = "Select count(*) " & "from {0} where {1}='{2}' " & "and {0}ID<>{3}"

		'add parentfield filter?
		If sParentField <> "" Then
			sSql += " And {4}={5}"
		End If

		Dim iParentValue As Integer = SafeInt(sParentValue)

		sSql = String.Format(sSql, sTable, sField.ToString, sValue.Replace("'", "''"), iTableID, sParentField, If(iParentValue <> 0, iParentValue.ToString, SQL.GetSqlValue(sParentValue, SqlValueType.String)))

		iCount = SQL.ExecuteSingleValueWithConnectString(ConnectString, sSql)
		Return iCount = 0
	End Function

	''' <summary>
	''' Checks sys.procedures for a stored procedure with the specified name.
	''' </summary>
	''' <param name="sStoredProcedureName">Name of the stored procedure.</param>
	Public Shared Function CheckStoredProcedureExists(sStoredProcedureName As String) As Boolean

		Dim sSQL As String = "select count(*) " & "from sys.procedures " & "where name = {0}"
		Dim iCount As Integer = SQL.ExecuteSingleValue(sSQL, SQL.GetSqlValue(sStoredProcedureName))

		Return iCount <> 0
	End Function

	''' <summary>
	''' Checks sys.tables for a table with the specified name.
	''' </summary>
	''' <param name="sTableName">Name of the table.</param>
	Public Shared Function CheckTableExists(sTableName As String) As Boolean
		Dim sSQL As String = "select count(*) from sys.tables where name = {0}"
		Dim iCount As Integer = SQL.ExecuteSingleValue(sSQL, SQL.GetSqlValue(sTableName))

		Return iCount <> 0
	End Function

	''' <summary>
	''' Builds and returns a connect string using the specified parameters.
	''' </summary>
	''' <param name="sServer">The server.</param>
	''' <param name="sDatabase">The database.</param>
	''' <param name="sUser">The user uid.</param>
	''' <param name="sPassword">The password.</param>
	Public Shared Function BuildConnectString(sServer As String, sDatabase As String, sUser As String, sPassword As String) As String

		Return String.Format("server={0};database={1};uid={2};pwd={3}", sServer, sDatabase, sUser, sPassword)
	End Function

	''' <summary>
	''' Gets the last error.
	''' </summary>
	Public ReadOnly Property LastError As String

	''' <summary>
	''' Converts data table to csv
	''' </summary>
	''' <param name="DataTable">The data table.</param>
	Public Shared Function DataTableToCSV(DataTable As DataTable) As String

		Dim oCSV As New StringBuilder

		Dim bPrecedingComma As Boolean = False

		' Header
		For Each oColumn As DataColumn In DataTable.Columns
			If bPrecedingComma Then oCSV.Append(",")

			oCSV.Append(oColumn.ColumnName)

			bPrecedingComma = True
		Next

		oCSV.Append(Environment.NewLine)

		' Values
		For Each oRow As DataRow In DataTable.Rows
			bPrecedingComma = False

			For i As Integer = 0 To DataTable.Columns.Count - 1
				If bPrecedingComma Then oCSV.Append(",")

				oCSV.Append(oRow(i).ToString.Replace(Char.ConvertFromUtf32(10), "").Replace(Char.ConvertFromUtf32(13), "").Replace(",", String.Empty))

				bPrecedingComma = True
			Next

			oCSV.Append(Environment.NewLine)
		Next

		Return oCSV.ToString
	End Function

	''' <summary>
	''' Converts datatable to csv and saves to specified file
	''' </summary>
	''' <param name="dt">The dt.</param>
	''' <param name="sFilename">The s filename.</param>
	Public Shared Sub DatatableToCSV(dt As DataTable, sFilename As String)

		Dim sCSV As String = DataTableToCSV(dt)

		File.WriteAllText(sFilename, sCSV)
	End Sub

	''' <summary>
	''' Adds number of {i} parameters equal to the length of the param array to the end of the sql statement then returns the statement with the parameters inserted.
	''' </summary>
	''' <param name="sSql">The SQL.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function FormatStatement(sSql As String, ByVal ParamArray aParams() As Object) As String

		If sSql.IndexOf("{"c) < 1 And aParams.Length > 0 Then
			For i As Integer = 0 To aParams.Length - 1
				sSql = sSql & " {" & i.ToString & "},"
			Next
			sSql = sSql.Chop()
		End If

		sSql = String.Format(sSql, aParams)

		Return sSql
	End Function

	''' <summary>
	''' Sets values of specified object fields to the value of their matching fields in the datarow
	''' </summary>
	''' <param name="o">The object to set the values of.</param>
	''' <param name="dr">The datarow to get the values from.</param>
	''' <exception cref="System.Exception">no translation for field type</exception>
	Public Shared Sub FlushDataRowToObject(o As Object, dr As DataRow)

		Dim sValue As String

		For Each oColumn As DataColumn In dr.Table.Columns

			For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

				If oColumn.ColumnName.ToLower = oField.Name.ToLower Then

					sValue = dr(oColumn.ColumnName).ToString

					Select Case oField.FieldType.Name
						Case "String"
							oField.SetValue(o, sValue)
						Case "Int32"
							oField.SetValue(o, SafeInt(sValue))
						Case "DateTime"
							oField.SetValue(o, DateFunctions.SafeDate(sValue))
						Case "Double"
							oField.SetValue(o, SafeNumeric(sValue))
						Case "Decimal"
							oField.SetValue(o, CType(SafeNumeric(sValue), Decimal))
						Case "Boolean"
							oField.SetValue(o, SafeBoolean(sValue))
						Case Else
							Throw New Exception("no translation for field type " & oField.FieldType.Name)
					End Select
				End If

			Next

		Next
	End Sub

#End Region

#Region "ExecuteAsType"
	Public Shared Function ExecuteAsType(sSql As String, eType As SqlReturnType, ByVal ParamArray aParams() As Object) As Object
		sSql = SQL.FormatStatement(sSql, aParams)
		Return ExecuteAsType(sSql, eType)
	End Function

	Public Shared Function ExecuteAsType(sSql As String, eType As SqlReturnType) As Object
		Select Case eType
			Case SqlReturnType.None
				Return Execute(sSql)
			Case SqlReturnType.Scalar
				Return GetValue(sSql)
			Case SqlReturnType.Xml
				Return GetXMLDoc(sSql)
			Case SqlReturnType.Table
				Return GetDataTable(sSql)
			Case Else
				Return Nothing
		End Select
	End Function
#End Region

#Region "Get Datatable"

#Region "Async Cache"

	''' <summary>
	''' Gets datatable using specified sql, if it's already been cached, returns the cached datatable, otherwise executes sql and caches datatable
	''' </summary>
	''' <param name="sSql">The SQL for getting the datatable.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataTableAsyncCache(sSql As String, ByVal ParamArray aParams() As Object) As DataTable
		sSql = SQL.FormatStatement(sSql, aParams)
		Return SQL.GetDataTableAsyncCache(sSql)
	End Function

	''' <summary>
	''' Gets datatable using specified sql, if it's already been cached, returns the cached datatable, otherwise executes sql and caches datatable
	''' </summary>
	''' <param name="sSql">The SQL for getting the datatable.</param>
	Public Shared Function GetDataTableAsyncCache(sSql As String) As DataTable
		Return SQL.GetDataTableAsyncCache(sSql, SQL.ConnectString)
	End Function

	''' <summary>
	''' Gets datatable using specified sql against specified connect string, if it's already been cached, returns the cached datatable, otherwise executes sql and caches datatable
	''' </summary>
	''' <param name="sSql">The SQL for getting the datatable.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataTableAsyncCache(sSql As String, ConnectString As String, ByVal ParamArray aParams() As Object) As DataTable

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTableAsyncCache(sSql, ConnectString)
	End Function

	''' <summary>
	''' Gets datatable using specified sql against specified connect string, 
	''' if it's already been cached, returns the cached datatable, otherwise executes sql and caches datatable. 
	''' Doesn't get datatable from the cache when debugging.
	''' </summary>
	''' <param name="sSQL">The SQLfor getting the datatable.</param>
	''' <param name="ConnectString">The connect string.</param>
	Public Shared Function GetDataTableAsyncCache(sSQL As String, ConnectString As String) As DataTable

		Dim sCacheKey As String = AsyncCache.Controller(Of DataTable).GenerateKey(sSQL.GetHashCode.ToString)

		'return non cached in debug mode
		If IsDebugging Then Return SQL.GetDataTable(sSQL)

		'in release mode then return the cached item
		Return AsyncCache.Controller(Of DataTable).GetCache(sCacheKey, 5, Function()
																			  Return SQL.GetDataTable(sSQL)
																		  End Function)
	End Function

#End Region

	''' <summary>
	''' Gets the datatable from the cache, if it's not in the cache or we're debugging, executes the sql then adds the datatable to the cache.
	''' </summary>
	''' <param name="sSQL">The SQL for getting the datatable.</param>
	Public Shared Function GetDataTableCache(sSQL As String) As DataTable

		'Dim oCache As Caching.Cache = HttpContext.Current.Cache
		Dim sKey As String = sSQL.GetHashCode.ToString
		Dim dt As DataTable = GetCache(Of DataTable)(sKey)

		If dt Is Nothing OrElse IsDebugging Then
			dt = SQL.GetDataTable(sSQL)
			Intuitive.Functions.AddToCache(sKey, dt, 5)
		End If

		Return dt
	End Function

	''' <summary>
	''' Gets datatable from the cache using the specified connect string, if it's not in the cache or we're debugging, executes the sql then adds the datatable to the cache.
	''' </summary>
	''' <param name="sSql">The SQL for getting the datatable.</param>
	''' <param name="ConnectString">The connect string where we want to get the datatable from.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataTableWithConnectStringCache(sSql As String, ConnectString As String, ByVal ParamArray aParams() As Object) As DataTable
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTableWithConnectStringCache(sSql, ConnectString)
	End Function

	''' <summary>
	''' Gets datatable from the cache using the specified connect string, if it's not in the cache or we're debugging, executes the sql then adds the datatable to the cache.
	''' </summary>
	''' <param name="sSql">The SQL for getting the datatable.</param>
	''' <param name="ConnectString">The connect string where we want to get the datatable from.</param>
	Public Shared Function GetDataTableWithConnectStringCache(sSql As String, ConnectString As String) As DataTable

		'Dim oCache As Caching.Cache = HttpContext.Current.Cache
		Dim sKey As String = sSql.GetHashCode.ToString
		Dim dt As DataTable = GetCache(Of DataTable)(sKey)

		If dt Is Nothing OrElse IsDebugging Then
			dt = SQL.GetDataTableWithConnectString(sSql, ConnectString)
			Intuitive.Functions.AddToCache(sKey, dt, 5)
		End If

		Return dt
	End Function

	''' <summary>
	''' Gets the datatable from the cache, if it's not in the cache or we're debugging, executes the sql then adds the datatable to the cache.
	''' </summary>
	''' <param name="sSQL">The SQL for getting the datatable.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataTableCache(sSQL As String, ByVal ParamArray aParams() As Object) As DataTable
		sSQL = SQL.FormatStatement(sSQL, aParams)
		Return SQL.GetDataTableCache(sSQL)
	End Function

	''' <summary>
	''' Gets datatable from specified sql using specified parameters.
	''' </summary>
	''' <param name="sSql">The SQL to create the datatable.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataTable(sSql As String, ByVal ParamArray aParams() As Object) As DataTable

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTable(sSql)
	End Function

	''' <summary>
	''' Gets datatable from sql query
	''' </summary>
	''' <param name="sSql">The SQL to create the datatable.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataTable(sSql As String) As DataTable
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = oConnection.ConnectionTimeout
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataTable", oConnection.ConnectionString, sSql, oLogHandler)

			Return oDataSet.Tables(0)

		Catch ex As Exception
			Throw New Exception("GetDatatable Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Gets datatable using specified sql and parameters with specified timeout in seconds
	''' </summary>
	''' <param name="sSql">The SQL to create the datatable with.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataTableWithTimeout(sSql As String, iTimeoutSeconds As Integer, ByVal ParamArray aParams() As Object) As DataTable
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTableWithTimeout(sSql, iTimeoutSeconds)
	End Function

	''' <summary>
	''' Gets datatable using specified sql with specified timeout in seconds.
	''' </summary>
	''' <param name="sSql">The SQL to create the datatable with.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataTableWithTimeout(sSql As String, iTimeoutSeconds As Integer) As DataTable
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = iTimeoutSeconds
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataTable", oConnection.ConnectionString, sSql, oLogHandler)

			Return oDataSet.Tables(0)

		Catch ex As Exception
			Throw New Exception("GetDatatable Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Gets datatable using specified sql with specified parameters against specified connect string.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataTableWithConnectString(sSql As String, ConnectString As String, ByVal ParamArray aParams() As Object) As DataTable

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTableWithConnectString(sSql, ConnectString)
	End Function

	''' <summary>
	''' Gets datatable using specified sql against specified connect string.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataTableWithConnectString(sSql As String, ConnectString As String) As DataTable
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = oConnection.ConnectionTimeout
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataTableWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			Return oDataSet.Tables(0)

		Catch ex As Exception
			Throw New Exception("GetDatatable Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Gets datatable using sql with parameters against the connect string with a specified timeout.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataTableWithConnectStringTimeout(sSql As String, ConnectString As String, iTimeoutSeconds As Integer, ByVal ParamArray aParams() As Object) As DataTable

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataTableWithConnectStringTimeout(sSql, ConnectString, iTimeoutSeconds)
	End Function

	''' <summary>
	''' Gets datatable using sql against connect string with a specified timeout
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataTableWithConnectStringTimeout(sSql As String, ConnectString As String, iTimeoutSeconds As Integer) As DataTable
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = iTimeoutSeconds
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataTableWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			Return oDataSet.Tables(0)

		Catch ex As Exception
			Throw New Exception("GetDatatable Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Gets dataset using sql with parameters
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataSet(sSql As String, ByVal ParamArray aParams() As Object) As DataSet
		Return GetDataSet(SQL.FormatStatement(sSql, aParams))
	End Function

	''' <summary>
	''' Gets dataset using sql
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDatatable Failed
	''' </exception>
	Public Overloads Shared Function GetDataSet(sSql As String) As DataSet

		Dim oDataSet As New DataSet

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			Using oConnection As New SqlConnection(ConnectString)
				Using oDataAdapter As New SqlDataAdapter(sSql, oConnection)

					oDataAdapter.Fill(oDataSet)
					oConnection.Close()

					If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataSet", oConnection.ConnectionString, sSql, oLogHandler)

				End Using
			End Using

		Catch ex As Exception
			Throw New Exception("GetDataset Failed " & sSql, ex)
		End Try

		Return oDataSet

	End Function
#End Region

#Region "Get Dataset"
	''' <summary>
	''' Gets dataset using sql, optional params 
	''' </summary>
	''' <param name="aParams">The optional parameters.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDataSet Failed
	''' </exception>
	Public Overloads Shared Function GetDataSetWithConnectString(ConnectString As String, sSql As String, ParamArray aParams() As Object) As DataSet
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet

		If (aParams IsNot Nothing And aParams.Length > 0) Then
			sSql = SQL.FormatStatement(sSql, aParams)
		End If

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = oConnection.ConnectionTimeout
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataSetWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			Return oDataSet

		Catch ex As Exception
			Throw New Exception("GetDataSet Failed " & sSql, ex)
		End Try

		Return Nothing
	End Function

#End Region

#Region "GetXMLDoc"

	''' <summary>
	''' Gets XML document from cache.
	''' </summary>
	''' <param name="iMinutes">How long to cache result for in minutes.</param>
	''' <param name="sSQL">The SQL that generates the xml document.</param>
	Public Shared Function GetXMLDocCache(iMinutes As Integer, sSQL As String) As XmlDocument

		'Dim oCache As Caching.Cache = HttpContext.Current.Cache
		Dim sKey As String = "xml" & sSQL.GetHashCode.ToString
		Dim oXML As XmlDocument = GetCache(Of XmlDocument)(sKey)

		If oXML Is Nothing OrElse Functions.IsDebugging Then
			oXML = SQL.GetXMLDoc(sSQL)
			Intuitive.Functions.AddToCache(sKey, oXML, iMinutes)
		End If

		Return oXML
	End Function

	''' <summary>
	''' Gets XML document from the cache.
	''' </summary>
	''' <param name="sSQL">The SQL that generates the xml document.</param>
	Public Shared Function GetXMLDocCache(sSQL As String) As XmlDocument
		Return SQL.GetXMLDocCache(5, sSQL)
	End Function

	''' <summary>
	''' Gets XML document from the cache.
	''' </summary>
	''' <param name="iMinutes">How long to cache the result for in minutes.</param>
	''' <param name="sSQL">The SQL that generates the xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDocCache(iMinutes As Integer, sSQL As String, ByVal ParamArray aParams() As Object) As XmlDocument
		sSQL = SQL.FormatStatement(sSQL, aParams)
		Return SQL.GetXMLDocCache(iMinutes, sSQL)
	End Function

	''' <summary>
	''' Gets XML document from the cache.
	''' </summary>
	''' <param name="sSQL">The SQL to generate the xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDocCache(sSQL As String, ByVal ParamArray aParams() As Object) As XmlDocument
		sSQL = SQL.FormatStatement(sSQL, aParams)
		Return SQL.GetXMLDocCache(sSQL)
	End Function

	''' <summary>
	''' Gets XML document from sql with parameters.
	''' </summary>
	''' <param name="sSql">The SQL to generate xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDoc(sSql As String, ByVal ParamArray aParams() As Object) As XmlDocument

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetXMLDoc(sSql)
	End Function

	''' <summary>
	''' Gets the XML document from sql using connect string and parameters.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to generate the xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDocWithConnectString(ConnectString As String, sSql As String, ByVal ParamArray aParams() As Object) As XmlDocument

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetXMLDocWithConnectString(ConnectString, sSql)
	End Function

	''' <summary>
	''' Generates xml document using sql
	''' </summary>
	''' <param name="sSql">The SQL to generate xml document.</param>
	Public Shared Function GetXMLDoc(sSql As String) As XmlDocument

		Dim oXMLDocument As New XmlDocument

		Using oConnection As New SqlConnection(SQL.ConnectString)
			Dim oCommand As New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			'open up the connection and read the results into the text reader
			oConnection.Open()
			Dim oXMLReader As XmlReader = oCommand.ExecuteXmlReader

			oXMLDocument.Load(oXMLReader)

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetXMLDoc", oConnection.ConnectionString, sSql, oLogHandler)
		End Using

		Return oXMLDocument

	End Function

	''' <summary>
	''' Generates xml document using sql against specified connect string
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to generate the xml document.</param>
	Public Shared Function GetXMLDocWithConnectString(ConnectString As String, sSql As String) As XmlDocument

		Dim oXMLDocument As New XmlDocument

		'create the connection and command object
		Using oConnection As New SqlConnection(ConnectString)
			Dim oCommand As New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			'open up the connection and read the results into the text reader
			oConnection.Open()
			Dim oXMLReader As XmlReader = oCommand.ExecuteXmlReader

			oXMLDocument.Load(oXMLReader)

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetXMLDocWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)
		End Using

		Return oXMLDocument
	End Function

	''' <summary>
	''' Gets xml document from cache against specified connect string
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to generate/retrieve the xml document with.</param>
	''' <param name="Minutes">How long to cache for in minutes.</param>
	Public Shared Function GetXMLDocWithConnectStringCache(ConnectString As String, sSql As String, Optional Minutes As Integer = 5) As XmlDocument

		Dim sKey As String = sSql.GetHashCode.ToString
		Dim oXMLDocument As XmlDocument = GetCache(Of XmlDocument)(sKey)

		If oXMLDocument Is Nothing OrElse IsDebugging Then
			'create the connection and command object
			Using oConnection As New SqlConnection(ConnectString)
				Dim oCommand As New SqlCommand(sSql, oConnection)
				oCommand.CommandTimeout = oConnection.ConnectionTimeout

				Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

				'open up the connection and read the results into the text reader
				oConnection.Open()
				Dim oXMLReader As XmlReader = oCommand.ExecuteXmlReader
				oXMLDocument = New XmlDocument
				oXMLDocument.Load(oXMLReader)

				If oLogHandler.ProcessEnd Then SQLLogging.End("GetXMLDocWithConnectStringCache", oConnection.ConnectionString, sSql, oLogHandler)

			End Using

			' add to cache
			Intuitive.Functions.AddToCache(sKey, oXMLDocument, Minutes)
		End If

		Return oXMLDocument
	End Function

	''' <summary>
	''' Gets XML document using sql and params with timeout.
	''' </summary>
	''' <param name="iTimeoutSeconds">Not used</param>
	''' <param name="sSql">The SQL to get the xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDocWithTimeout(iTimeoutSeconds As Integer, sSql As String, ByVal ParamArray aParams() As Object) As XmlDocument
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetXMLDocWithTimeout(iTimeoutSeconds, sSql)
	End Function

	''' <summary>
	''' Gets XML document using sql and params with connect string and specified timeout.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to get the xml document.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetXMLDocWithConnectStringWithTimeout(ConnectString As String, iTimeoutSeconds As Integer, sSql As String, ByVal ParamArray aParams() As Object) As XmlDocument
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetXMLDocWithConnectStringWithTimeout(ConnectString, iTimeoutSeconds, sSql)
	End Function

	''' <summary>
	''' Gets XML document using sql with timeout.
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to get the xml document.</param>
	Public Shared Function GetXMLDocWithTimeout(iTimeoutSeconds As Integer, sSql As String) As XmlDocument

		Dim oXMLDocument As New XmlDocument

		'create the connection and command object
		Using oConnection As New SqlConnection(SQL.ConnectString)
			Dim oCommand As New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = iTimeoutSeconds

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			'open up the connection and read the results into the text reader
			oConnection.Open()
			Dim oXMLReader As XmlReader = oCommand.ExecuteXmlReader

			oXMLDocument.Load(oXMLReader)

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetXMLDoc", oConnection.ConnectionString, sSql, oLogHandler)
		End Using

		Return oXMLDocument
	End Function

	''' <summary>
	''' Gets XML document using sql with connect string and specified timeout.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to get the xml document.</param>
	Public Shared Function GetXMLDocWithConnectStringWithTimeout(ConnectString As String, iTimeoutSeconds As Integer, sSql As String) As XmlDocument

		Dim oXMLDocument As New XmlDocument

		'create the connection and command object
		Using oConnection As New SqlConnection(ConnectString)
			Dim oCommand As New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = iTimeoutSeconds

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			'open up the connection and read the results into the text reader
			oConnection.Open()
			Dim oXMLReader As XmlReader = oCommand.ExecuteXmlReader
			oXMLDocument.Load(oXMLReader)

			'Close the connection
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetXMLDocWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

		End Using

		Return oXMLDocument

	End Function

#End Region

#Region "GetDataRow"

	''' <summary>
	''' Gets datarow using sql and parameters.
	''' </summary>
	''' <param name="sSql">The SQL to get the datarow.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataRow(sSql As String, ByVal ParamArray aParams() As Object) As DataRow

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetDataRow(sSql)
	End Function

	''' <summary>
	''' Gets datarow using sql.
	''' </summary>
	''' <param name="sSql">The SQL to get the datarow.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDataRow Failed
	''' </exception>
	Public Overloads Shared Function GetDataRow(sSql As String) As DataRow
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet
		Dim oDataRow As DataRow

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = oConnection.ConnectionTimeout
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataRow", oConnection.ConnectionString, sSql, oLogHandler)

			If oDataSet.Tables.Count > 0 AndAlso oDataSet.Tables(0).Rows.Count > 0 Then
				oDataRow = oDataSet.Tables(0).Rows(0)
				Return oDataRow
			End If

		Catch ex As Exception
			Throw New Exception("GetDataRow Failed " & sSql, ex)
		End Try

		Return Nothing
	End Function

	''' <summary>
	''' Gets datarow using sql and paramters with specified connect string.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to get the datarow.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetDataRowWithConnectString(ConnectString As String, sSql As String, ByVal ParamArray aParams() As Object) As DataRow

		sSql = SQL.FormatStatement(sSql, aParams)
		Return SQL.GetDataRowWithConnectString(ConnectString, sSql)
	End Function

	''' <summary>
	''' Gets datarow using sql with specified connect string.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to get the datarow.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetDataRow Failed
	''' </exception>
	Public Overloads Shared Function GetDataRowWithConnectString(ConnectString As String, sSql As String) As DataRow
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet
		Dim oDataRow As DataRow

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataAdapter.SelectCommand.CommandTimeout = oConnection.ConnectionTimeout
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetDataRowWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			If oDataSet.Tables.Count > 0 AndAlso oDataSet.Tables(0).Rows.Count > 0 Then
				oDataRow = oDataSet.Tables(0).Rows(0)
				Return oDataRow
			End If

		Catch ex As Exception
			Throw New Exception("GetDataRow Failed " & sSql, ex)
		End Try

		Return Nothing
	End Function

	''' <summary>
	''' Gets datarow from cache, if it's not in the cache or we're debugging, executes sql and adds result to cache
	''' </summary>
	''' <param name="sSQL">The SQL to get the datarow.</param>
	Public Shared Function GetDataRowCache(sSQL As String) As DataRow

		Dim sKey As String = ("GetDataRowCache" & sSQL).GetHashCode.ToString
		Dim dr As DataRow = GetCache(Of DataRow)(sKey)

		If dr Is Nothing OrElse IsDebugging Then
			dr = SQL.GetDataRow(sSQL)
			If dr IsNot Nothing Then Intuitive.Functions.AddToCache(sKey, dr, 5)
		End If

		Return dr
	End Function

	''' <summary>
	''' Gets datarow from cache, if it's not in the cache or we're debugging, executes sql with paramters and adds result to cache
	''' </summary>
	''' <param name="sSQL">The SQL to get the datarow.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataRowCache(sSQL As String, ByVal ParamArray aParams() As Object) As DataRow
		sSQL = SQL.FormatStatement(sSQL, aParams)
		Return SQL.GetDataRowCache(sSQL)
	End Function

	''' <summary>
	''' Gets datarow from cache, if it's not in the cache or we're debugging, executes sql against connect string and adds result to cache.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to get the datarow.</param>
	Public Shared Function GetDataRowCacheWithConnectString(ConnectString As String, sSQL As String) As DataRow

		Dim sKey As String = ("GetDataRowCache" & sSQL).GetHashCode.ToString
		Dim dr As DataRow = GetCache(Of DataRow)(sKey)

		If dr Is Nothing OrElse IsDebugging Then
			dr = SQL.GetDataRowWithConnectString(ConnectString, sSQL)
			If dr IsNot Nothing Then Intuitive.Functions.AddToCache(sKey, dr, 5)
		End If

		Return dr
	End Function

	''' <summary>
	''' Gets datarow from cache, if it's not in the cache or we're debugging, executes sql with params against connect string and adds result to cache.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to get the datarow.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDataRowCacheWithConnectString(ConnectString As String, sSQL As String, ByVal ParamArray aParams() As Object) As DataRow
		sSQL = SQL.FormatStatement(sSQL, aParams)
		Return SQL.GetDataRowCacheWithConnectString(ConnectString, sSQL)
	End Function

#End Region

#Region "ExecuteSingleValue"

	''' <summary>
	''' Executes sql with parameters and returns an integer value
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function ExecuteSingleValue(sSql As String, ByVal ParamArray aParams() As Object) As Integer

		sSql = SQL.FormatStatement(sSql, aParams)
		Return ExecuteSingleValue(sSql)
	End Function

	''' <summary>
	''' Executes sql and returns an integer value
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteSingleValue Failed
	''' </exception>
	Public Overloads Shared Function ExecuteSingleValue(sSql As String) As Integer
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New System.Data.SqlClient.SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)

			oCommand.CommandTimeout = oConnection.ConnectionTimeout

			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteSingleValue", oConnection.ConnectionString, sSql, oLogHandler)

			If oObject IsNot Nothing AndAlso InList(oObject.GetType.Name, "Int32", "Decimal", "Boolean") Then
				Return Convert.ToInt32(oObject)
			Else
				Return 0
			End If

		Catch ex As Exception
			Throw New Exception("ExecuteSingleValue Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with params against specified connect string and returns a single value
	''' </summary>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteSingleValue Failed
	''' </exception>
	Public Overloads Shared Function ExecuteSingleValueWithConnectString(sConnectString As String, sSql As String, ByVal ParamArray aParams() As Object) As Integer
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		sSql = SQL.FormatStatement(sSql, aParams)

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New System.Data.SqlClient.SqlConnection(sConnectString)
			oCommand = New SqlCommand(sSql, oConnection)

			oCommand.CommandTimeout = oConnection.ConnectionTimeout

			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteSingleValueWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			If Not (oObject Is Nothing) AndAlso (oObject.GetType.Name = "Int32" OrElse oObject.GetType.Name = "Decimal") Then
				Return CType(oObject, Integer)
			Else
				Return 0
			End If

		Catch ex As Exception
			Throw New Exception("ExecuteSingleValue Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql and returns an object
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="bReturnObject">Not used</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteSingleValue Failed
	''' </exception>
	Public Overloads Shared Function ExecuteSingleValue(sSql As String, bReturnObject As Boolean) As Object
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)
			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteSingleValue", oConnection.ConnectionString, sSql, oLogHandler)

			Return If(oObject, "")

		Catch ex As Exception
			Throw New Exception("ExecuteSingleValue Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with params and specified timeout and returns an integer value
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function ExecuteSingleValueWithTimeout(iTimeoutSeconds As Integer, sSql As String, ByVal ParamArray aParams() As Object) As Integer
		sSql = SQL.FormatStatement(sSql, aParams)
		Return ExecuteSingleValueWithTimeout(iTimeoutSeconds, sSql)
	End Function

	''' <summary>
	''' Executes sql and specified timeout and returns an integer value
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteSingleValue Failed
	''' </exception>
	Public Overloads Shared Function ExecuteSingleValueWithTimeout(iTimeoutSeconds As Integer, sSql As String) As Integer
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New System.Data.SqlClient.SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)

			oCommand.CommandTimeout = iTimeoutSeconds

			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteSingleValue", oConnection.ConnectionString, sSql, oLogHandler)

			If Not (oObject Is Nothing) AndAlso (oObject.GetType.Name = "Int32" OrElse oObject.GetType.Name = "Decimal") Then
				Return CType(oObject, Integer)
			Else
				Return 0
			End If

		Catch ex As Exception
			Throw New Exception("ExecuteSingleValue Failed " & sSql, ex)
		End Try
	End Function

#End Region

#Region "GetValue"

	''' <summary>
	''' Executes sql and returns a string value of the result.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetValue Failed
	''' </exception>
	Public Overloads Shared Function GetValue(sSql As String) As String
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		'get a single string value from the query
		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetValue", oConnection.ConnectionString, sSql, oLogHandler)

			If Not oObject Is Nothing AndAlso oObject.GetType.ToString <> "System.DBNull" Then
				Return CType(oObject, String)
			Else
				Return ""
			End If

		Catch ex As Exception
			Throw New Exception("GetValue Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with parameters and returns a string value of the result
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function GetValue(sSql As String, ByVal ParamArray aParams() As Object) As String

		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetValue(sSql)
	End Function

	''' <summary>
	''' Executes sql against specified connect string and returns a string value of the result
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' GetValue Failed
	''' </exception>
	Public Overloads Shared Function GetValueWithConnectString(ConnectString As String, sSql As String) As String
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		'get a single string value from the query
		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetValueWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			If Not oObject Is Nothing AndAlso oObject.GetType.ToString <> "System.DBNull" Then
				Return CType(oObject, String)
			Else
				Return ""
			End If

		Catch ex As Exception
			Throw New Exception("GetValue Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with parameters against specified connect string and returns a string value of the result
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with sql.</param>
	Public Overloads Shared Function GetValueWithConnectString(ConnectString As String, sSql As String, ByVal ParamArray aParams() As Object) As String
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetValueWithConnectString(ConnectString, sSql)
	End Function

	''' <summary>
	''' Executes sql with a given timeout and returns a string value of the result
	''' </summary>
	''' <param name="iTimeout">The amount of time to wait for a connection before terminating</param>
	''' <param name="sSql">The sql to execute</param>
	''' <returns></returns>
	Public Shared Function GetValueWithTimeout(iTimeout As Integer, sSql As String) As String
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		'get a single string value from the query
		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = iTimeout
			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetValue", oConnection.ConnectionString, sSql, oLogHandler)

			If Not oObject Is Nothing AndAlso oObject.GetType.ToString <> "System.DBNull" Then
				Return CType(oObject, String)
			Else
				Return ""
			End If

		Catch ex As Exception
			Throw New Exception("GetValue Failed " & sSql, ex)
		End Try
	End Function

	Public Shared Function GetValueWithTimeout(iTimeout As Integer, sSql As String, ByVal ParamArray aParams() As Object) As String
		sSql = SQL.FormatStatement(sSql, aParams)
		Return GetValueWithTimeout(iTimeout, sSql)
	End Function

#End Region

#Region "Execute"

	''' <summary>
	''' Executes the specified SQL with parameters, returns Identity of affected table.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function Execute(sSql As String, ByVal ParamArray aParams() As Object) As Integer

		sSql = SQL.FormatStatement(sSql, aParams)
		Return Execute(sSql)
	End Function

	''' <summary>
	''' Executes the specified SQL, returns Identity of affected table.
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' Execute Failed
	''' </exception>
	Public Overloads Shared Function Execute(sSql As String) As Integer
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim iIdentity As Integer
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oConnection.Open()

			sSql += Environment.NewLine & "select @@identity"
			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("Execute", oConnection.ConnectionString, sSql, oLogHandler)

			If Not oObject Is Nothing AndAlso oObject.GetType.Name = "Decimal" Then
				iIdentity = CType(oObject, Integer)
			Else
				iIdentity = 0
			End If

			Return iIdentity

		Catch ex As Exception
			Throw New Exception("Execute Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with parameters against the specified connect string and returns an Identity of the affected table.
	''' </summary>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Function ExecuteWithConnectString(sConnectString As String, sSql As String, ByVal ParamArray aParams() As Object) As Integer

		sSql = SQL.FormatStatement(sSql, aParams)
		Return SQL.ExecuteWithConnectString(sConnectString, sSql)
	End Function

	''' <summary>
	''' Executes sql against the specified connect string and returns an Identity of the affected table.
	''' </summary>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' Execute Failed
	''' </exception>
	Public Overloads Shared Function ExecuteWithConnectString(sConnectString As String, sSql As String) As Integer
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim iIdentity As Integer
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(sConnectString)
			oConnection.Open()

			sSql += Environment.NewLine & "select @@identity"
			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteWithConnectString", oConnection.ConnectionString, sSql, oLogHandler)

			If Not oObject Is Nothing AndAlso oObject.GetType.Name = "Decimal" Then
				iIdentity = CType(oObject, Integer)
			Else
				iIdentity = 0
			End If

			Return iIdentity

		Catch ex As Exception
			Throw New Exception("Execute Failed " & sSql, ex)
		End Try
	End Function

	''' <summary>
	''' Executes sql with parameters
	''' </summary>
	''' <param name="sSql">The s SQL.</param>
	''' <param name="aParams">a parameters.</param>
	Public Overloads Shared Sub ExecuteWithoutReturn(sSql As String, ByVal ParamArray aParams() As Object)

		sSql = SQL.FormatStatement(sSql, aParams)
		ExecuteWithoutReturn(sSql)
	End Sub

	''' <summary>
	''' Executes sql
	''' </summary>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteWithoutReturn Failed
	''' </exception>
	Public Overloads Shared Sub ExecuteWithoutReturn(sSql As String)
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oConnection.Open()

			oCommand = New SqlCommand(sSql, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteWithoutReturn", oConnection.ConnectionString, sSql, oLogHandler)

		Catch ex As Exception
			Throw New Exception("ExecuteWithoutReturn Failed " & sSql, ex)
		End Try
	End Sub

	''' <summary>
	''' Executes sql with parameters against specified connect string
	''' </summary>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Sub ExecuteWithoutReturnWithConnectString(sConnectString As String, sSQL As String, aParams() As Object)
		sSQL = SQL.FormatStatement(sSQL, aParams)
		ExecuteWithoutReturnWithConnectString(sConnectString, sSQL)
	End Sub

	''' <summary>
	''' Executes sql against specified connect string.
	''' </summary>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteWithoutReturn Failed
	''' </exception>
	Public Overloads Shared Sub ExecuteWithoutReturnWithConnectString(sConnectString As String, sSQL As String)
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSQL = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(sConnectString)
			oConnection.Open()

			oCommand = New SqlCommand(sSQL, oConnection)
			oCommand.CommandTimeout = oConnection.ConnectionTimeout
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteWithoutReturn", oConnection.ConnectionString, sSQL, oLogHandler)

		Catch ex As Exception
			Throw New Exception("ExecuteWithoutReturn Failed " & sSQL, ex)
		End Try
	End Sub

	''' <summary>
	''' Executes sql with parameters against connect string with a set timeout
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Sub ExecuteWithTimeoutWithConnectstring(iTimeoutSeconds As Integer, sConnectString As String, sSQL As String, aParams() As Object)
		sSQL = SQL.FormatStatement(sSQL, aParams)
		ExecuteWithTimeoutWithConnectstring(iTimeoutSeconds, sConnectString, sSQL)
	End Sub

	''' <summary>
	''' Executes sql against connect string with a set timeout
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sConnectString">The connect string.</param>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteWithoutReturn Failed
	''' </exception>
	Public Overloads Shared Sub ExecuteWithTimeoutWithConnectstring(iTimeoutSeconds As Integer, sConnectString As String, sSQL As String)

		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSQL = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(sConnectString)
			oConnection.Open()

			oCommand = New SqlCommand(sSQL, oConnection)
			oCommand.CommandTimeout = iTimeoutSeconds
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteWithoutReturn", oConnection.ConnectionString, sSQL, oLogHandler)

		Catch ex As Exception
			Throw New Exception("ExecuteWithoutReturn Failed " & sSQL, ex)
		End Try
	End Sub

	''' <summary>
	''' Executes sql with parameters and a set timeout
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Overloads Shared Sub ExecuteWithoutReturnWithTimeout(iTimeoutSeconds As Integer, sSql As String, ByVal ParamArray aParams() As Object)
		sSql = SQL.FormatStatement(sSql, aParams)
		ExecuteWithoutReturnWithTimeout(iTimeoutSeconds, sSql)
	End Sub

	''' <summary>
	''' Executes sql with a set timeout
	''' </summary>
	''' <param name="iTimeoutSeconds">The timeout in seconds.</param>
	''' <param name="sSql">The SQL to execute.</param>
	''' <exception cref="System.Exception">
	''' Sql Statement has not been specified
	''' or
	''' ExecuteWithoutReturn Failed
	''' </exception>
	Public Overloads Shared Sub ExecuteWithoutReturnWithTimeout(iTimeoutSeconds As Integer, sSql As String)
		Dim oConnection As SqlConnection
		Dim oCommand As SqlCommand
		Dim oObject As Object

		If sSql = "" Then
			Throw New Exception("Sql Statement has not been specified")
		End If

		Try

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New System.Data.SqlClient.SqlConnection(SQL.ConnectString)
			oCommand = New SqlCommand(sSql, oConnection)

			oCommand.CommandTimeout = iTimeoutSeconds

			oConnection.Open()
			oObject = oCommand.ExecuteScalar()
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("ExecuteWithoutReturn", oConnection.ConnectionString, sSql, oLogHandler)

		Catch ex As Exception
			Throw New Exception("ExecuteWithoutReturn Failed " & sSql, ex)
		End Try
	End Sub

	''' <summary>
	''' Executes the named stored procedure passing in the specified parameters.
	''' </summary>
	''' <param name="Name">The name of the stored procedure to execute.</param>
	''' <param name="oParams">The parameters to pass into the stored procedure.</param>
	Public Shared Function ExecuteStoredProcedureWithParameters(Name As String, oParams As Generic.List(Of SqlParameter)) As DataTable

		Dim ds As New DataSet

		'Note: the "Using" means don't have to close connection
		Using oConnection As New SqlConnection(ConnectString)

			'Likewise, don't have to dispose of command now
			Using oCommand As New SqlCommand

				oConnection.Open()

				With oCommand
					.CommandType = CommandType.StoredProcedure
					.CommandText = Name
					.Connection = oConnection

					For Each oParameter As SqlParameter In oParams
						.Parameters.Add(oParameter)
					Next

					Dim oAdaptor As New SqlDataAdapter(oCommand)
					oAdaptor.Fill(ds)

				End With
			End Using
		End Using

		Return ds.Tables(0)
	End Function

#End Region

#Region "GetList"

	''' <summary>
	''' Returns datatable from cache, if not in cache or we're debugging, executes <see cref="SQL.GetList"/>
	''' </summary>
	''' <param name="sTable">Table to get list from.</param>
	''' <param name="sField">Field to retrieve values from.</param>
	''' <param name="sFilter">Filter statement, does not require 'where'.</param>
	''' <param name="sFieldOverride">Alias for field.</param>
	''' <param name="sOrder">Order statement, does not require 'order by'.</param>
	''' <param name="IDField">ID field.</param>
	Public Shared Function GetListCache(sTable As String, sField As String, Optional ByVal sFilter As String = "", Optional ByVal sFieldOverride As String = "", Optional ByVal sOrder As String = "", Optional ByVal IDField As String = "") As DataTable

		Dim sKey As String = ("GetListCache" & "#" & sTable & "#" & "#" & sField & "#" & "#" & sFilter & "#" & "#" & sFieldOverride & "#" & "#" & sOrder & "#").GetHashCode.ToString
		Dim dt As DataTable = GetCache(Of DataTable)(sKey)

		If dt Is Nothing OrElse IsDebugging Then
			dt = SQL.GetList(sTable, sField, sFilter, sFieldOverride, sOrder)
			If dt IsNot Nothing Then Intuitive.Functions.AddToCache(sKey, dt, 5)
		End If

		Return dt
	End Function

	''' <summary>
	''' Returns datatable of values for specified field from specified table, with optional filter and ordering
	''' </summary>
	''' <param name="sTable">Table to get list from.</param>
	''' <param name="sField">Field to retrieve values from.</param>
	''' <param name="sFilter">Filter statement, does not require 'where'.</param>
	''' <param name="sFieldOverride">Alias for field.</param>
	''' <param name="sOrder">Order statement, does not require 'order by'.</param>
	''' <param name="IDField">ID field.</param>
	Public Shared Function GetList(sTable As String, sField As String, Optional ByVal sFilter As String = "", Optional ByVal sFieldOverride As String = "", Optional ByVal sOrder As String = "", Optional ByVal IDField As String = "") As DataTable

		Dim sSql As String
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet
		Dim oDataTable As DataTable = Nothing
		Dim sCacheName As String = ""
		'Dim sCacheDependencyFile As String = ConfigurationManager.AppSettings.Get("CacheDependencyFolder")

		If sTable = "" Then
			Throw New Exception("sTable has not been specified")
		ElseIf sField = "" Then
			Throw New Exception("Parameter sField has not been specified")
		End If

		'set up sql statement
		sTable = sTable.Trim
		sField = sField.Trim

		'add the filter if required
		If sFilter <> "" Then
			sFilter = " Where " & sFilter & " "
		End If

		'build up sql statement
		If sOrder = "" Then
			sOrder = sField
		End If

		sSql = "Select " & sField & If(sFieldOverride <> "", " as " & sFieldOverride, "").ToString & ", " & If(IDField <> "", IDField, sTable & "ID ").ToString & " from " & sTable & sFilter & " Order by " & sOrder

		'do we have the results already cached? (only if no filter)
		If sFilter = "" Then
			sCacheName = "List" & sTable & sField
			oDataTable = GetCache(Of DataTable)(sCacheName)
		End If

		If sFilter <> "" OrElse oDataTable Is Nothing Then

			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			'connect to database
			oConnection = New SqlConnection(SQL.ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetList", oConnection.ConnectionString, sSql, oLogHandler)

			'get the table from the dataset
			oDataTable = oDataSet.Tables(0)

			'do we have a no cache entry in the config file
			Dim bNoCache As Boolean = SafeBoolean(AppSettings("NoCache"))

			Dim sDateBase As String = oConnection.Database

		End If

		Return oDataTable
	End Function

#End Region

#Region "GetRow"

	''' <summary>
	''' Returns 1 row from specified table based on specified ID
	''' </summary>
	''' <param name="sTable">The table name to retrieve the row from.</param>
	''' <param name="iID">The id of the row to return, matches to the TableNameID column e.g. the BookingID column of the Booking table</param>
	''' <exception cref="System.Exception">
	''' Parameter sTable has not been specified
	''' or
	''' Parameter iID has not been specified
	''' or
	''' GetRow returned more than one row
	''' or
	''' Execute Failed
	''' </exception>
	Public Shared Function GetRow(sTable As String, iID As Integer) As DataTable
		Dim sSql As String
		Dim oConnection As SqlConnection
		Dim oDataAdapter As SqlDataAdapter
		Dim oDataSet As DataSet
		Dim oDataTable As DataTable

		If sTable = "" Then
			Throw New Exception("Parameter sTable has not been specified")
		ElseIf iID < 1 Then
			Throw New Exception("Parameter iID has not been specified")
		End If

		'set up sql statement
		sTable = sTable.Trim
		sSql = "Select * from " & sTable
		If iID > 0 Then
			sSql += " where " & sTable & "ID=" & iID.ToString
		End If

		'connect to database
		Try
			Dim oLogHandler As SQLLogging.SQLLogHandler = SQLLogging.Start()

			oConnection = New SqlConnection(SQL.ConnectString)
			oDataAdapter = New SqlDataAdapter(sSql, oConnection)
			oDataSet = New DataSet
			oDataAdapter.Fill(oDataSet)
			oConnection.Close()

			If oLogHandler.ProcessEnd Then SQLLogging.End("GetRow", oConnection.ConnectionString, sSql, oLogHandler)

			'get the table from the dataset
			oDataTable = oDataSet.Tables(0)

			'make sure we have 1 and only 1 row
			If oDataTable.Rows.Count <> 1 Then
				Throw New Exception("GetRow returned more than one row")
			End If

			'grab the row and return it
			Return oDataTable

		Catch ex As Exception
			Throw New Exception("Execute Failed " & sSql, ex)
		End Try
	End Function

#End Region

#Region "GetSQLValue"

	''' <summary>
	''' Returns the value in a format that's valid in sql.
	''' </summary>
	''' <param name="oValue">The value.</param>
	''' <param name="SqlValueType">The desired type of the value.</param>
	''' <param name="sXMLEncoding">The XML encoding to use when the SQLValue type is xml.</param>
	Public Shared Function GetSqlValue(oValue As Object, Optional ByVal SqlValueType As SqlValueType = SqlValueType.String, Optional sXMLEncoding As String = "") As String
		Dim sSqlValue As String = ""

		Select Case SqlValueType
			Case SqlValueType.Boolean
				sSqlValue = "Boolean"
			Case SqlValueType.Date
				sSqlValue = "Date"
			Case SqlValueType.DateTime
				sSqlValue = "Datetime"
			Case SqlValueType.Double
				sSqlValue = "Double"
			Case SqlValueType.Integer
				sSqlValue = "Integer"
			Case SqlValueType.Numberic
				sSqlValue = "Numeric"
			Case SqlValueType.String
				sSqlValue = "String"
			Case SQL.SqlValueType.Multilingual
				sSqlValue = "MultiLingual"
			Case SQL.SqlValueType.XML
				sSqlValue = "XML"
		End Select
		Return GetSqlValue(oValue, sSqlValue, sXMLEncoding)
	End Function

	''' <summary>
	''' Gets the SQL value.Returns the value in a format that's valid in sql.
	''' </summary>
	''' <param name="oValue">The value.</param>
	''' <param name="sType">The desired type of the value.</param>
	''' <param name="sXMLEncoding">The XML encoding to use when the SQLValue type is xml.</param>
	''' <exception cref="System.Exception">No handler for type</exception>
	Public Shared Function GetSqlValue(oValue As Object, sType As String, Optional ByVal sXMLEncoding As String = "") As String

		If oValue Is Nothing OrElse oValue.ToString = "" Then
			Select Case sType.ToLower
				Case "string", "text", "multilingual", "xml"
					Return "''"
				Case "integer", "numeric", "double"
					Return "0"
				Case "date", "datetime"
					Return "null"
				Case "boolean"
					Return "0"
			End Select
		End If

		Dim sValue As String = oValue.ToString
		Dim iValue As Integer
		Dim bValue As Boolean
		Dim nValue As Decimal

		Dim bInt As Boolean = Integer.TryParse(sValue, iValue)
		Dim bNumeric As Boolean = Decimal.TryParse(sValue, nValue)

		Select Case sType.ToLower
			Case "string", "text"

				'replace single quotes with doubles
				sValue = sValue.Replace("'", "''").Trim
				sValue = "'" & sValue & "'"

			Case "multilingual"

				'replace single quotes with doubles & stick an N on the front
				sValue = sValue.Replace("'", "''").Trim
				sValue = "N'" & sValue & "'"

			Case "integer"
				If bInt Then
					sValue = iValue.ToString
				Else
					Return "0"
				End If
			Case "numeric", "double"
				If bNumeric Then
					sValue = nValue.ToString
				Else
					Return "0"
				End If
			Case "boolean"
				bValue = CType(oValue, Boolean)
				sValue = If(bValue, "1", "0").ToString
			Case "date"

				sValue = sValue.Replace("'", "")
				If Not sValue = "" Then
					Dim dDate As Date = DateFunctions.SafeDate(oValue)
					If Not DateFunctions.IsEmptyDate(CType(sValue, Date)) AndAlso dDate >= DateTime.Parse("01/01/1753") Then 'N.B. SQL does not allow dates before 1753!
						sValue = "'" & CType(sValue, Date).ToString("MM/dd/yyyy") & "'"
					Else
						sValue = "null"
					End If
				Else
					sValue = "null"
				End If

			Case "datetime"

				Dim dDate As Date = DateFunctions.SafeDate(sValue)
				If Not DateFunctions.IsEmptyDate(dDate) AndAlso dDate >= DateTime.Parse("01/01/1753") Then
					sValue = "'" & dDate.ToString("MM/dd/yyyy HH:mm:ss.fff") & "'"
				Else
					sValue = "null"
				End If

			Case "time"

				Dim dTime As Date = SafeDate(oValue)

				If IsEmptyDate(dTime) Then
					sValue = "null"
				Else
					sValue = "'" & dTime.ToString("HH:mm:ss.fff") & "'"
				End If

			Case "xml"

				Dim sb As New StringBuilder(oValue.ToString)

				'deal with the prologue
				Dim sRegEx As String = "(\<\?xml.*\?\>)"
				Dim sXMLDeclaration As String = String.Empty
				If Not String.IsNullOrEmpty(sXMLEncoding) Then
					sXMLDeclaration = "<?xml version=""1.0"" encoding=""" + sXMLEncoding + """?>"
				End If

				Dim aCapture As ArrayList = Nothing

				Intuitive.Functions.RegExp.Capture(sb.ToString, sRegEx, aCapture)

				If Not aCapture Is Nothing AndAlso aCapture.Count > 1 Then
					sb.Replace(aCapture(0).ToString, sXMLDeclaration)
				ElseIf sb.Length > 0 Then
					sb.Insert(0, sXMLDeclaration)
				End If

				'do standard string stuff too
				'replace single quotes with doubles
				sb.Replace("'", "''")
				sb.Insert(0, "'").Append("'")

				' If there isn't a specified declaration then make the string unicode to allow it to accept more character codes
				If String.IsNullOrEmpty(sXMLDeclaration) Then
					sb.Insert(0, "N")
				End If

				sValue = sb.ToString

			Case Else
				Throw New Exception("No handler for type " & sType)
		End Select

		Return sValue

	End Function

	'This is a handy helper function to save me keystrokes ;), but it can't know the impossible. If it doesn't work for you, don't use it!
	''' <summary>
	''' Gets SQL valid values for all values in an array of values.
	''' </summary>
	''' <param name="aParams">List of values to get sql valid values for.</param>
	Public Shared Function GetSqlValues(ByVal ParamArray aParams As Object()) As Object()
		For i As Integer = 0 To aParams.Length - 1
			If TypeOf aParams(i) Is Boolean Then
				aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.Boolean)
			ElseIf TypeOf aParams(i) Is Date Then
				If CType(aParams(i), Date).Date = CType(aParams(i), Date) Then
					aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.Date)
				Else
					aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.DateTime)
				End If
			ElseIf TypeOf aParams(i) Is Double Then
				aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.Double)
			ElseIf TypeOf aParams(i) Is Integer Then
				aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.Integer)
			ElseIf TypeOf aParams(i) Is Decimal Then
				aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.Numberic)
			ElseIf TypeOf aParams(i) Is String Then
				aParams(i) = SQL.GetSqlValue(aParams(i), SqlValueType.String)
			ElseIf TypeOf aParams(i) Is XmlDocument Then
				aParams(i) = SQL.GetSqlValue(CType(aParams(i), XmlDocument).InnerXml, SqlValueType.XML)
			ElseIf TypeOf aParams(i) Is [Enum] Then
				aParams(i) = SQL.GetSqlValue(aParams(i).ToString, SqlValueType.String)
			ElseIf TypeOf aParams(i) Is Generic.List(Of Integer) Then
				aParams(i) = SQL.GetSqlValue(String.Join(",", CType(aParams(i), Generic.List(Of Integer)).ToArray), SqlValueType.String)
			ElseIf aParams(i) Is Nothing Then
				aParams(i) = "null"
			Else
				aParams(i) = SQL.GetSqlValue(aParams(i))
			End If
		Next
		Return aParams
	End Function

#End Region

#Region "List To Table Valued Parameter"

	'Only added 1 dimensional string type, if want to add different structures need to create the type in sql databases you intend to access
	'Format of type creation is "CREATE TYPE [Type Name] AS TABLE", and then define the type structure as you would a table, including any primary keys

	''' <summary>
	''' Converts list of strings to a list of table valued parameters
	''' </summary>
	''' <param name="List">The list.</param>
	Public Shared Function ListOfStringToTVP(List As Generic.List(Of String)) As Generic.List(Of SqlDataRecord)

		Dim oRecords As New Generic.List(Of SqlDataRecord)
		Dim tvpDef As SqlMetaData() = {New SqlMetaData("Value", SqlDbType.VarChar, 50)}

		For Each item As String In List
			Dim oRecord As New SqlDataRecord(tvpDef)
			oRecord.SetValue(0, item)
			oRecords.Add(oRecord)
		Next

		Return oRecords
	End Function

#End Region

#Region "objectrow"

	''' <summary>
	''' Class used to map datarow to a specific object
	''' </summary>
	<Serializable()>
	Public Class ObjectRow
		''' <summary>
		''' Uses reflection to map columns from datarow to fields on this object
		''' </summary>
		''' <param name="dr">The datarow.</param>
		''' <exception cref="System.Exception">no translation for field type  &amp; oField.FieldType.Name</exception>
		Public Sub Setup(dr As DataRow)

			Dim sValue As String

			For Each oColumn As DataColumn In dr.Table.Columns

				For Each oField As System.Reflection.FieldInfo In Me.GetType.GetFields

					If oColumn.ColumnName.ToLower = oField.Name.ToLower Then

						sValue = dr(oColumn.ColumnName).ToString

						Select Case oField.FieldType.Name
							Case "String"
								oField.SetValue(Me, sValue)
							Case "Int32"
								oField.SetValue(Me, SafeInt(sValue))
							Case "DateTime"
								oField.SetValue(Me, DateFunctions.SafeDate(sValue))
							Case "Double"
								oField.SetValue(Me, SafeNumeric(sValue))
							Case "Decimal"
								oField.SetValue(Me, CType(SafeNumeric(sValue), Decimal))
							Case "Boolean"
								oField.SetValue(Me, SafeBoolean(sValue))
							Case Else
								Throw New Exception("no translation for field type " & oField.FieldType.Name)
						End Select
					End If

				Next

			Next
		End Sub
	End Class

#End Region

#Region "Enums"

	''' <summary>
	''' Enum of possible valid sql value types
	''' </summary>
	Public Enum SqlValueType
		[String]
		[Integer]
		[Numberic]
		[Double]
		[Boolean]
		[Date]
		[DateTime]
		[XML]
		[Multilingual]
	End Enum

	Public Enum SqlReturnType
		[None]
		[Scalar]
		[Xml]
		[Json]
		[Table]
	End Enum

#End Region

#Region "table builder"

	''' <summary>
	''' Class used 
	''' </summary>
	Public Class TableBuilder

#Region "create table"

		''' <summary>
		''' Creates table against connect string with specified table name and specified fields.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="sTableName">Name of the table.</param>
		''' <param name="oFields">List of <see cref="Field"/>.</param>
		''' <exception cref="System.Exception">
		''' the table name specified is not valid
		''' or
		''' the table already exists
		''' or
		''' at least one field must be specified
		''' </exception>
		Public Shared Sub CreateTableWithConnectString(ConnectString As String, sTableName As String, oFields As Fields)

			'make sure
			'   the table name is valid
			'   the table doesn't exist
			'   at least one field has been specified
			If Not SQL.Meta.ValidateTableAndFieldName(sTableName) Then Throw New Exception("the table name specified is not valid - " & sTableName)
			If SQL.Meta.TableExists(sTableName) Then Throw New Exception("the table already exists - " & sTableName)
			If oFields.Count = 0 Then Throw New Exception("at least one field must be specified")

			'build table def
			Dim sb As New StringBuilder
			sb.AppendFormat("create table [{0}] (", sTableName)

			'for each field
			For Each oField As Field In oFields
				sb.Append(oField.ToString).Append(",")
			Next

			Dim sCommand As String = sb.ToString.Chop()

			'primary key builder
			Dim oPrimaryKeyFields As Generic.List(Of Field) = oFields.PrimaryKeyFields

			If oPrimaryKeyFields.Count > 0 Then

				Dim oSBPrimary As New StringBuilder
				For Each oField As Field In oPrimaryKeyFields
					oSBPrimary.Append(oField.FieldName).Append(" asc,")
				Next
				Dim sPrimary As String = oSBPrimary.ToString.Chop()
				sCommand &= String.Format(", constraint [PK_{0}] primary key clustered ({1})", sTableName, sPrimary)

			End If

			sCommand &= ")"

			'execute (actually happy to throw the sql exception here)
			SQL.ExecuteWithConnectString(ConnectString, sCommand)
		End Sub

		''' <summary>
		''' Creates table with specified table name and specified fields.
		''' </summary>
		''' <param name="sTableName">Name of the table.</param>
		''' <param name="oFields">List of <see cref="Field"/>.</param>
		''' <exception cref="System.Exception">
		''' the table name specified is not valid
		''' or
		''' the table already exists
		''' or
		''' at least one field must be specified
		''' </exception>
		Public Shared Sub CreateTable(sTableName As String, oFields As Fields)

			'make sure
			'   the table name is valid
			'   the table doesn't exist
			'   at least one field has been specified
			If Not SQL.Meta.ValidateTableAndFieldName(sTableName) Then Throw New Exception("the table name specified is not valid - " & sTableName)
			If SQL.Meta.TableExists(sTableName) Then Throw New Exception("the table already exists - " & sTableName)
			If oFields.Count = 0 Then Throw New Exception("at least one field must be specified")

			'build table def
			Dim sb As New StringBuilder
			sb.AppendFormat("create table [{0}] (", sTableName)

			'for each field
			For Each oField As Field In oFields
				sb.Append(oField.ToString).Append(",")
			Next

			Dim sCommand As String = sb.ToString.Chop()

			'primary key builder
			Dim oPrimaryKeyFields As Generic.List(Of Field) = oFields.PrimaryKeyFields

			If oPrimaryKeyFields.Count > 0 Then

				Dim oSBPrimary As New StringBuilder
				For Each oField As Field In oPrimaryKeyFields
					oSBPrimary.Append(oField.FieldName).Append(" asc,")
				Next
				Dim sPrimary As String = oSBPrimary.ToString.Chop()
				sCommand &= String.Format(", constraint [PK_{0}] primary key clustered ({1})", sTableName, sPrimary)

			End If

			sCommand &= ")"

			'execute (actually happy to throw the sql exception here)
			SQL.Execute(sCommand)
		End Sub

#End Region

#Region "drop table"

		''' <summary>
		''' Drops specified table, will remove it from replication if it's replicated.
		''' </summary>
		''' <param name="TableName">Name of the table to drop.</param>
		Public Shared Sub DropTable(TableName As String)

			'if replicated remove it from replication
			If SQL.Meta.IsReplicated(TableName) Then
				SQL.Meta.RemoveTableFromReplication(TableName, SQL.Meta.GetTableReplicationPublications(TableName))
			End If

			If SQL.Meta.TableExists(TableName) Then
				SQL.Execute("drop table [{0}]", TableName)
			End If
		End Sub

#End Region

#Region "add field(s)"

		'add field
		''' <summary>
		''' Adds field to the table, if it's replicated, removes table from replication before adding the field then adds it back into replication.
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="Field">The field.</param>
		''' <exception cref="System.Exception">
		''' table does not exist
		''' or
		''' field already exists
		''' </exception>
		Public Shared Sub AddField(TableName As String, Field As Field)

			If Not Meta.TableExists(TableName) Then Throw New Exception("table does not exist - " & TableName)
			If Meta.FieldExists(TableName, Field.FieldName) Then Throw New Exception("field already exists - " & TableName & "/" & Field.FieldName)

			'if it's in replication then take it out
			Dim oPublications As New Generic.List(Of String)
			Dim bReplicated As Boolean = SQL.Meta.IsReplicated(TableName)
			If bReplicated Then
				oPublications = SQL.Meta.GetTableReplicationPublications(TableName)
				SQL.Meta.RemoveTableFromReplication(TableName, oPublications)
			End If

			Try
				Dim sb As New System.Text.StringBuilder
				sb.AppendFormat("alter table [{0}] add {1}", TableName, Field.ToString)

				SQL.Execute(sb.ToString)
			Catch ex As Exception

			Finally
				'restore the replication
				If bReplicated Then
					SQL.Meta.AddTableToReplication(TableName, oPublications)
				End If
			End Try
		End Sub

		'add fields
		''' <summary>
		''' Adds fields to table, will remove tables from replication prior to adding fields and re-add them afterwards.
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="Fields">The fields.</param>
		Public Shared Sub AddFields(TableName As String, Fields As Fields)
			For Each oField As Field In Fields
				TableBuilder.AddField(TableName, oField)
			Next
		End Sub

#End Region

#Region "drop field"

		''' <summary>
		''' Removes field from table, removes from replication first then re-adds to replication after field removed.
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="FieldName">Name of the field.</param>
		''' <exception cref="System.Exception">
		''' table does not exist
		''' or
		''' field already exists
		''' </exception>
		Public Shared Sub DropField(TableName As String, FieldName As String)

			If Not Meta.TableExists(TableName) Then Throw New Exception("table does not exist - " & TableName)
			If Not Meta.FieldExists(TableName, FieldName) Then Throw New Exception("field doesn't exist - " & TableName & "/" & FieldName)

			'if it's in replication then take it out
			Dim oPublications As New Generic.List(Of String)
			Dim bReplicated As Boolean = SQL.Meta.IsReplicated(TableName)
			If bReplicated Then
				oPublications = SQL.Meta.GetTableReplicationPublications(TableName)
				SQL.Meta.RemoveTableFromReplication(TableName, oPublications)
			End If

			Try
				Dim sb As New System.Text.StringBuilder
				sb.AppendFormat("alter table [{0}] drop column [{1}]", TableName, FieldName)

				SQL.Execute(sb.ToString)

			Catch ex As Exception

			Finally
				'restore the replication
				If bReplicated Then
					SQL.Meta.AddTableToReplication(TableName, oPublications)
				End If
			End Try
		End Sub

#End Region

#Region "Rename Field"

		''' <summary>
		''' Renames table field, removes table from replication before renaming and re-adds it afterwards.
		''' </summary>
		''' <param name="sTable">The table.</param>
		''' <param name="sOriginalFieldName">Original name of the field.</param>
		''' <param name="sNewFieldName">New name for the field.</param>
		Public Shared Sub RenameField(sTable As String, sOriginalFieldName As String, sNewFieldName As String)

			'if it's in replication then take it out
			Dim oPublications As New Generic.List(Of String)
			Dim bReplicated As Boolean = SQL.Meta.IsReplicated(sTable)
			If bReplicated Then
				oPublications = SQL.Meta.GetTableReplicationPublications(sTable)
				SQL.Meta.RemoveTableFromReplication(sTable, oPublications)
			End If

			Try
				SQL.Execute("exec sp_rename '{0}.{1}', '{2}', 'column'", sTable, sOriginalFieldName, sNewFieldName)
			Catch ex As Exception

			Finally
				'restore the replication
				If bReplicated Then
					SQL.Meta.AddTableToReplication(sTable, oPublications)
				End If
			End Try
		End Sub

#End Region

#Region "Rename Table"

		''' <summary>
		''' Renames the table, removes from replication before renaming and re-adds it afterwards.
		''' </summary>
		''' <param name="sOldTable">The old table name.</param>
		''' <param name="sNewTableName">New table name.</param>
		Public Shared Sub RenameTable(sOldTable As String, sNewTableName As String)

			'if it's in replication then take it out
			Dim oPublications As New Generic.List(Of String)
			Dim bReplicated As Boolean = SQL.Meta.IsReplicated(sOldTable)
			If bReplicated Then
				oPublications = SQL.Meta.GetTableReplicationPublications(sOldTable)
				SQL.Meta.RemoveTableFromReplication(sOldTable, oPublications)
			End If

			Try
				SQL.Execute("exec sp_rename [{0}] , [{1}]", sOldTable, sNewTableName)
				'restore the replication
				If bReplicated Then
					SQL.Meta.AddTableToReplication(sNewTableName, oPublications)
				End If
			Catch ex As Exception
				'restore the replication
				If bReplicated Then
					SQL.Meta.AddTableToReplication(sOldTable, oPublications)
				End If
			End Try
		End Sub

#End Region

#Region "sql field collection"

		''' <summary>
		''' Class representing a list of <see cref="Field"/>
		''' </summary>
		''' <seealso cref="T:System.Collections.Generic.List{T}" />
		Public Class Fields
			Inherits Generic.List(Of Field)

			''' <summary>
			''' Adds a new <see cref="Field"/> to the list.
			''' </summary>
			''' <param name="sFieldName">Name of the field.</param>
			''' <param name="oDataType">Type of the field.</param>
			''' <param name="iWidth">Used for strings and decimals,
			''' if it's a string - sets the max varchar length of the field, 
			''' if it's a decimal - sets the precision and scale of the decimal, sets precision to be iWidth+10 and scale to be iWidth.
			''' </param>
			''' <param name="bIsIdentity">If set to <c>true</c>, sets field to be an identity.</param>
			''' <param name="bAllowNulls">If set to <c>true</c>, field allows nulls.</param>
			''' <param name="bIsPrimaryKey">If set to <c>true</c>, sets field to be a primary key.</param>
			''' <param name="bIsUnicode">Only used when datatype is string, if set to <c>true</c>, specified whether the text of the field is unicode.</param>
			Public Overloads Sub Add(sFieldName As String, oDataType As SqlValueType, Optional ByVal iWidth As Integer = 0, Optional ByVal bIsIdentity As Boolean = False, Optional ByVal bAllowNulls As Boolean = True, Optional ByVal bIsPrimaryKey As Boolean = False, Optional ByVal bIsUnicode As Boolean = False)
				Me.Add(New Field(sFieldName, oDataType, iWidth, bIsIdentity, bAllowNulls, bIsPrimaryKey, bIsUnicode))
			End Sub

			''' <summary>
			''' Returns a list of fields where IsPrimaryKey = True.
			''' </summary>
			Public ReadOnly Property PrimaryKeyFields() As Generic.List(Of Field)
				Get
					Dim oFields As New Generic.List(Of Field)
					For Each oField As Field In Me
						If oField.IsPrimaryKey Then
							oFields.Add(oField)
						End If
					Next
					Return oFields
				End Get
			End Property

			''' <summary>
			''' Returns a list of fields with the specified <see cref="SqlValueType"/>.
			''' </summary>
			Public ReadOnly Property ByFieldType(oType As SqlValueType) As Generic.List(Of Field)
				Get
					Dim oReturn As New Generic.List(Of Field)
					For Each oField As Field In Me
						If oField.DataType = oType Then
							oReturn.Add(oField)
						End If
					Next
					Return oReturn
				End Get
			End Property
		End Class

#End Region

#Region "sql field definition"

		''' <summary>
		''' Class representing a field in a table
		''' </summary>
		Public Class Field
			''' <summary>
			''' The field name
			''' </summary>
			Public FieldName As String

			''' <summary>
			''' The type of the field.
			''' </summary>
			Public DataType As SqlValueType

			''' <summary>
			''' Used for strings and decimals,
			''' if it's a string - sets the max varchar length of the field, 
			''' if it's a decimal - sets the precision and scale of the decimal, sets precision to be iWidth+10 and scale to be iWidth.
			''' </summary>
			Public Width As Integer

			''' <summary>
			''' Specifies whether field is an identity
			''' </summary>
			Public IsIdentity As Boolean = False

			''' <summary>
			''' Specifies whether field allows nulls
			''' </summary>
			Public AllowNulls As Boolean = True

			''' <summary>
			''' Specifies whether field is a primary key
			''' </summary>
			Public IsPrimaryKey As Boolean = False

			''' <summary>
			''' Only used for strings, specifies whether field data is unicode
			''' </summary>
			Public IsUnicode As Boolean = False

			''' <summary>
			''' Initializes a new instance of the <see cref="Field"/> class.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="Field"/> class.
			''' </summary>
			''' <param name="sFieldName">Name of the s field.</param>
			''' <param name="oDataType">Type of the field.</param>
			''' <param name="iWidth">Used for strings and decimals,
			''' if it's a string - sets the max varchar length of the field, 
			''' if it's a decimal - sets the precision and scale of the decimal, sets precision to be iWidth+10 and scale to be iWidth.</param>
			''' <param name="bIsIdentity">If set to <c>true</c>, sets field to be an identity.</param>
			''' <param name="bAllowNulls">If set to <c>true</c>, field allows nulls.</param>
			''' <param name="bIsPrimaryKey">If set to <c>true</c>, sets field to be a primary key.</param>
			''' <param name="bIsUnicode">Only used when datatype is string, if set to <c>true</c>, specified whether the text of the field is unicode.</param>
			''' <exception cref="System.Exception">
			''' field is not valid
			''' or
			''' a width must be specified for a field that it is a string
			''' or
			''' only integers can be identity fields
			''' </exception>
			Public Sub New(sFieldName As String, oDataType As SqlValueType, Optional ByVal iWidth As Integer = 0, Optional ByVal bIsIdentity As Boolean = False, Optional ByVal bAllowNulls As Boolean = True, Optional ByVal bIsPrimaryKey As Boolean = False, Optional ByVal bIsUnicode As Boolean = False)

				Me.FieldName = sFieldName
				Me.DataType = oDataType
				Me.Width = iWidth
				Me.IsIdentity = bIsIdentity
				Me.AllowNulls = bAllowNulls
				Me.IsPrimaryKey = bIsPrimaryKey
				Me.IsUnicode = bIsUnicode
				'only allow nulls for non primary and non identity fields
				If Me.IsPrimaryKey OrElse Me.IsIdentity Then Me.AllowNulls = False

				'make sure
				'   the field name is valid
				'   strings have a width
				'   only integers can be an identity field
				If Not SQL.Meta.ValidateTableAndFieldName(sFieldName) Then
					Throw New Exception("field is not valid " & sFieldName)
				End If

				If Me.DataType = SqlValueType.String AndAlso Me.Width = 0 Then
					Throw New Exception("a width must be specified for a field that it is a string")
				End If

				If Me.IsIdentity AndAlso Me.DataType <> SqlValueType.Integer Then
					Throw New Exception("only integers can be identity fields " & sFieldName)
				End If
			End Sub

			''' <summary>
			''' Returns a <see cref="System.String" /> that represents this instance.
			''' </summary>
			Public Overrides Function ToString() As String

				Dim sb As New System.Text.StringBuilder
				sb.AppendFormat("[{0}] ", Me.FieldName)

				Select Case Me.DataType

					Case SqlValueType.String

						'a fraction shonky but set the width to max if massive or -1
						Dim sWidth As String = Me.Width.ToString
						If Me.Width > 250 OrElse Me.Width < 0 Then
							sWidth = "max"
						End If
						If (Me.IsUnicode) Then sb.Append("n")
						sb.AppendFormat("varchar({0})", sWidth)

					Case SqlValueType.Integer
						sb.Append("int")
					Case SqlValueType.Numberic
						If Me.Width = 0 Then
							sb.Append("decimal(14,2)")
						Else
							sb.AppendFormat("decimal({0},{1})", (Me.Width + 10).ToString, Me.Width.ToString)
						End If
					Case SqlValueType.Double
						sb.Append("real")
					Case SqlValueType.Boolean
						sb.Append("bit")
					Case SqlValueType.Date
						sb.Append("datetime")
					Case SqlValueType.DateTime
						sb.Append("datetime")
					Case SqlValueType.XML
						sb.Append("xml")
				End Select

				If Me.IsIdentity Then
					sb.Append(" not null identity(1,1)")
				Else

					If Not Me.AllowNulls Then
						sb.Append(" not")
					End If

					sb.Append(" null")
				End If

				Return sb.ToString
			End Function
		End Class

#End Region
	End Class

#End Region

#Region "meta"

	''' <summary>
	''' Class with functions to access sql database meta information
	''' </summary>
	Public Class Meta
		''' <summary>
		''' Checks information_schema.tables for the existance of a table matching the TableName
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		Public Shared Function TableExists(TableName As String) As Boolean
			Return SQL.ExecuteSingleValue("select count(*) from information_schema.tables where Table_Name={0}", SQL.GetSqlValue(TableName)) > 0
		End Function

		''' <summary>
		''' Checks information_schema.columns for existance of a table with specified field name.
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="FieldName">Name of the field.</param>
		Public Shared Function FieldExists(TableName As String, FieldName As String) As Boolean
			Return SQL.ExecuteSingleValue("select count(*) from information_schema.columns where Table_Name={0} and column_name={1}", SQL.GetSqlValue(TableName), SQL.GetSqlValue(FieldName)) > 0
		End Function

		''' <summary>
		''' Checks information_schema.columns for a table with the specified fieldnames.
		''' </summary>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="FieldNames">The field names.</param>
		Public Shared Function TableAndFieldsExist(TableName As String, ByVal ParamArray FieldNames As String()) As Boolean
			Dim oFieldNames As New Generic.List(Of String)(FieldNames)

			Return SQL.GetDataRowCache("select count(*) [Count] from information_schema.columns where Table_Name={0} and column_name in ({1})", SQL.GetSqlValue(TableName), String.Join(",", oFieldNames.ConvertAll(Function(oFieldName) SQL.GetSqlValue(oFieldName)).ToArray))("Count").ToString.ToSafeInt = oFieldNames.Count
		End Function

		''' <summary>
		''' Checks sys.procedures for the existance of the specified stored procedure
		''' </summary>
		''' <param name="sStoredProcedureName">Name of the stored procedure.</param>
		''' <param name="sConnectionString">Optional, the connection string to check against.</param>
		Public Shared Function StoredProcedureExists(sStoredProcedureName As String, Optional ByVal sConnectionString As String = Nothing) As Boolean
			If sConnectionString Is Nothing Then sConnectionString = SQL.ConnectString
			Return SQL.ExecuteSingleValueWithConnectString(sConnectionString, "select count(*) from sys.procedures where name = {0}", SQL.GetSqlValue(sStoredProcedureName)) <> 0
		End Function

		''' <summary>
		''' Checks that the specified function exists.
		''' </summary>
		''' <param name="FunctionName">Name of the function.</param>
		Public Shared Function FunctionExists(FunctionName As String) As Boolean
			Return Meta.FunctionExists(FunctionName, SQL.ConnectString)
		End Function

		''' <summary>
		''' Checks that the specified function exists against the specified connect string.
		''' </summary>
		''' <param name="FunctionName">Name of the function.</param>
		''' <param name="ConnectString">The connect string to check against.</param>
		Public Shared Function FunctionExists(FunctionName As String, ConnectString As String) As Boolean
			Dim sSQL As String = SQL.FormatStatement("select isnull(OBJECT_ID({0},'FN'),0)", SQL.GetSqlValue(FunctionName))
			Return SQL.ExecuteSingleValueWithConnectString(ConnectString, sSQL) <> 0
		End Function

		''' <summary>
		''' Checks string is valid for a table name or field name
		''' </summary>
		''' <param name="Name">The name.</param>
		Public Shared Function ValidateTableAndFieldName(Name As String) As Boolean
			Return Regex.IsMatch(Name, "^[A-Za-z][A-Za-z_0-9]*$")
		End Function

		''' <summary>
		''' Gets a list of <see cref="Field"/> objects for the specified table name from information_schema.columns.
		''' </summary>
		''' <param name="sTableName">Name of the table.</param>
		Public Shared Function FieldList(sTableName As String) As TableBuilder.Fields

			Dim oReturn As New TableBuilder.Fields
			Dim dtFields As DataTable = SQL.GetDataTable("select column_name, data_type, is_nullable, character_maximum_length, numeric_scale from " & " information_schema.columns where table_name = {0} order by column_name", SQL.GetSqlValue(sTableName, SqlValueType.String))
			For Each drField As DataRow In dtFields.Rows
				Dim oField As New TableBuilder.Field
				With oField
					.FieldName = drField("column_name").ToString
					.AllowNulls = (drField("is_nullable").ToString.ToLower = "yes")
					Select Case drField("data_type").ToString.ToLower
						Case "varchar"
							.DataType = SqlValueType.String
						Case "decimal"
							.DataType = SqlValueType.Numberic
						Case "int"
							.DataType = SqlValueType.Integer
						Case "datetime"
							.DataType = SqlValueType.DateTime
						Case "bit"
							.DataType = SqlValueType.Boolean
					End Select
				End With
				oReturn.Add(oField)
			Next
			Return oReturn
		End Function

		''' <summary>
		''' Checks sysarticles and syspublications to determine whether the specified table is replicated.
		''' </summary>
		''' <param name="sTableName">Name of the table.</param>
		Public Shared Function IsReplicated(sTableName As String) As Boolean
			Try
				Dim dt As DataTable = SQL.GetDataTable("select syspublications.name Publication from sysarticles " & "inner join syspublications on syspublications.pubid = sysarticles.pubid where sysarticles.dest_table = {0}", SQL.GetSqlValue(sTableName))

				Return dt.Rows.Count > 0
			Catch ex As Exception
				Return False
			End Try
		End Function

		''' <summary>
		''' Gets list of publications that contain the specified table
		''' </summary>
		''' <param name="sTableName">Name of the table.</param>
		Public Shared Function GetTableReplicationPublications(sTableName As String) As Generic.List(Of String)

			Dim dt As DataTable = SQL.GetDataTable("select syspublications.name Publication from sysarticles " & "inner join syspublications on syspublications.pubid = sysarticles.pubid where sysarticles.dest_table = {0}", SQL.GetSqlValue(sTableName))

			Dim oPublications As New Generic.List(Of String)

			For Each dr As DataRow In dt.Rows
				oPublications.Add(SafeString(dr("Publication")))
			Next

			Return oPublications
		End Function

		''' <summary>
		''' Removes specified table from the publications in the list
		''' </summary>
		''' <param name="sTableName">Name of the table to remove from replication.</param>
		''' <param name="oPublications">List of publications to remove the table from.</param>
		Public Shared Sub RemoveTableFromReplication(sTableName As String, oPublications As Generic.List(Of String))

			Dim oSQL As New SQLTransaction

			oSQL.Add("SET TRANSACTION ISOLATION LEVEL READ COMMITTED")
			For Each sPublication As String In oPublications
				oSQL.Add("exec Replication_AlterPublication {0}, {1}, ''", SQL.GetSqlValue(sTableName), SQL.GetSqlValue(sPublication))
			Next

			If oSQL.HasTransactions Then oSQL.Execute()
		End Sub

		''' <summary>
		''' Adds the table to publications in the list.
		''' </summary>
		''' <param name="sTableName">Name of the table to add to replication.</param>
		''' <param name="oPublications">List of publications to add the table to.</param>
		Public Shared Sub AddTableToReplication(sTableName As String, oPublications As Generic.List(Of String))

			Dim oSQL As New SQLTransaction

			oSQL.Add("SET TRANSACTION ISOLATION LEVEL READ COMMITTED")
			For Each sPublication As String In oPublications
				oSQL.Add("exec Replication_AlterPublication {0}, '', {1}", SQL.GetSqlValue(sTableName), SQL.GetSqlValue(sPublication))
				oSQL.Add("exec sys.sp_refreshsubscriptions {0}", SQL.GetSqlValue(sPublication))
				oSQL.Add("exec sys.sp_startpublication_snapshot @publication = {0}", SQL.GetSqlValue(sPublication))
			Next

			If oSQL.HasTransactions Then oSQL.Execute()
		End Sub
	End Class

#End Region

#Region "Field Def"

#End Region

#Region "Dictionary Cache"

	'integer, string
	''' <summary>
	''' Gets cache of integer-string dictionary, if the cache doesn't exist or we're debugging, it runs the sql and stores the result in the cache.
	''' </summary>
	''' <param name="sSQL">The SQL used to build the cache.</param>
	''' <param name="aParams">The parameters to be used with sql.</param>
	Public Shared Function GetDictionaryCache_IntegerString(sSQL As String, ParamArray aParams() As Object) As Generic.Dictionary(Of Integer, String)

		Dim sKey As String = sSQL.GetHashCode.ToString

		Dim oDictionary As Generic.Dictionary(Of Integer, String) = GetCache(Of Generic.Dictionary(Of Integer, String))(sKey)

		If oDictionary Is Nothing OrElse IsDebugging Then
			Dim dt As DataTable = SQL.GetDataTable(sSQL, aParams)

			oDictionary = New Generic.Dictionary(Of Integer, String)
			For Each dr As DataRow In dt.Rows
				If Not oDictionary.ContainsKey(SafeInt(dr(0))) Then oDictionary.Add(SafeInt(dr(0)), dr(1).ToString)
			Next

			Intuitive.Functions.AddToCache(sKey, oDictionary, 5)
		End If

		Return oDictionary
	End Function

	'string, integer
	''' <summary>
	''' Gets cache of string-integer dictionary, if the cache doesn't exist or we're debugging, it runs the sql and stores the result in the cache.
	''' </summary>
	''' <param name="sSQL">The SQL used to build the cache.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDictionaryCache_StringInteger(sSQL As String, ParamArray aParams() As Object) As Generic.Dictionary(Of String, Integer)

		Dim sKey As String = sSQL.GetHashCode.ToString

		Dim oDictionary As Generic.Dictionary(Of String, Integer) = GetCache(Of Generic.Dictionary(Of String, Integer))(sKey)

		If oDictionary Is Nothing OrElse IsDebugging Then
			Dim dt As DataTable = SQL.GetDataTable(sSQL, aParams)

			oDictionary = New Generic.Dictionary(Of String, Integer)
			For Each dr As DataRow In dt.Rows
				If Not oDictionary.ContainsKey(SafeString(dr(0))) Then oDictionary.Add(SafeString(dr(0)), SafeInt(dr(1)))
			Next

			Intuitive.Functions.AddToCache(sKey, oDictionary, 5)
		End If

		Return oDictionary
	End Function

	'string, string
	''' <summary>
	''' Gets cache of string-string dictionary, if the cache doesn't exist or we're debugging, it runs the sql and stores the result in the cache.
	''' </summary>
	''' <param name="sSQL">The SQL used to build the cache.</param>
	''' <param name="aParams">The parameters to use with the sql.</param>
	Public Shared Function GetDictionaryCache_StringString(sSQL As String, ParamArray aParams() As Object) As Generic.Dictionary(Of String, String)

		Dim sKey As String = sSQL.GetHashCode.ToString

		Dim oDictionary As Generic.Dictionary(Of String, String) = GetCache(Of Generic.Dictionary(Of String, String))(sKey)
		If oDictionary Is Nothing OrElse IsDebugging Then
			Dim dt As DataTable = SQL.GetDataTable(sSQL, aParams)

			oDictionary = New Generic.Dictionary(Of String, String)
			For Each dr As DataRow In dt.Rows
				If Not oDictionary.ContainsKey(SafeString(dr(0))) Then oDictionary.Add(SafeString(dr(0)), SafeString(dr(1)))
			Next

			Intuitive.Functions.AddToCache(sKey, oDictionary, 5)
		End If

		Return oDictionary
	End Function

#End Region

#Region "Write CSV To TextWriter"

	''' <summary>
	''' Executes sql, creates CSV of results and writes CSV to text writer.
	''' </summary>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="oTextWriter">The text writer to write the csv to.</param>
	''' <param name="aParams">Parameters used with the sql.</param>
	Public Shared Function WriteCSVToTextWriter(sSQL As String, oTextWriter As TextWriter, ByVal ParamArray aParams() As Object) As Integer
		Return WriteCSVToTextWriter(sSQL, ","c, """"c, """"c, True, False, True, oTextWriter, aParams)
	End Function

	''' <summary>
	''' Executes sql, creates CSV of results and writes CSV to text writer.
	''' </summary>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="sDelimiter">The csv delimiter.</param>
	''' <param name="oTextWriter">The text writer to write the csv to.</param>
	''' <param name="aParams">Parameters used with the sql.</param>
	Public Shared Function WriteCSVToTextWriter(sSQL As String, sDelimiter As Char, oTextWriter As TextWriter, ByVal ParamArray aParams() As Object) As Integer
		Return WriteCSVToTextWriter(sSQL, sDelimiter, """"c, """"c, True, False, True, oTextWriter, aParams)
	End Function

	''' <summary>
	''' Executes sql, creates CSV of results and writes CSV to text writer.
	''' </summary>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="sDelimiter">The csv delimiter.</param>
	''' <param name="bAddColumnsNamesIfNoData">if set to <c>true</c>, will add column name even if there's no data returned.</param>
	''' <param name="oTextWriter">The text writer to write the csv to.</param>
	''' <param name="aParams">Parameters used with the sql.</param>
	Public Shared Function WriteCSVToTextWriter(sSQL As String, sDelimiter As Char, bAddColumnsNamesIfNoData As Boolean, oTextWriter As TextWriter, ByVal ParamArray aParams() As Object) As Integer
		Return WriteCSVToTextWriter(sSQL, sDelimiter, """"c, """"c, True, False, bAddColumnsNamesIfNoData, oTextWriter, aParams)
	End Function

	''' <summary>
	''' Executes sql, creates CSV of results and writes CSV to text writer.
	''' </summary>
	''' <param name="sSQL">The SQL to execute.</param>
	''' <param name="sDelimiter">The csv delimiter.</param>
	''' <param name="sQuoteCharacter">Quote character.</param>
	''' <param name="sEscapeCharacter">Used to escape characters in the csv.</param>
	''' <param name="bAddColumnNames">if set to <c>true</c>, adds column names.</param>
	''' <param name="bAddIndexColumn">if set to <c>true</c>, adds index column.</param>
	''' <param name="bAddColumnsNamesIfNoData">if set to <c>true</c>, will add column name even if there's no data returned.</param>
	''' <param name="oTextWriter">The text writer to write the csv to.</param>
	''' <param name="aParams">Parameters used with the sql.</param>
	Public Shared Function WriteCSVToTextWriter(sSQL As String, sDelimiter As Char, sQuoteCharacter As Char, sEscapeCharacter As Char, bAddColumnNames As Boolean, bAddIndexColumn As Boolean, bAddColumnsNamesIfNoData As Boolean, oTextWriter As TextWriter, ByVal ParamArray aParams() As Object) As Integer

		Dim oSQLConnection As New SqlConnection(SQL.ConnectString)
		Dim oSQLCommand As New SqlCommand(SQL.FormatStatement(sSQL, aParams), oSQLConnection)

		Dim iLinesWritten As Integer
		Dim oException As Exception = Nothing

		Try
			oSQLConnection.Open()

			Dim oSQLDataReader As SqlDataReader = oSQLCommand.ExecuteReader()
			Dim oCSVWriter As New LargeData.CSVWriter(oTextWriter, sDelimiter, sQuoteCharacter, sEscapeCharacter)
			Dim iColumnsOffset As Integer = If(bAddIndexColumn, 1, 0)

			If bAddColumnNames AndAlso (bAddColumnsNamesIfNoData OrElse oSQLDataReader.HasRows) Then
				Dim oColumnNames(iColumnsOffset + oSQLDataReader.FieldCount - 1) As String

				If bAddIndexColumn Then oColumnNames(0) = "Index"

				For i As Integer = iColumnsOffset To oSQLDataReader.FieldCount - 1
					oColumnNames(i) = oSQLDataReader.GetName(i)
				Next

				oCSVWriter.WriteHeaderRow(oColumnNames)
			End If

			If oSQLDataReader.HasRows Then

				Dim oValues(oSQLDataReader.FieldCount - 1) As Object
				Dim oFinalValues(iColumnsOffset + oSQLDataReader.FieldCount - 1) As Object

				While oSQLDataReader.Read()
					oSQLDataReader.GetSqlValues(oValues)

					If bAddIndexColumn Then
						oFinalValues(0) = oCSVWriter.CurrentLine + 1
						Array.Copy(oValues, 0, oFinalValues, 1, oValues.Length)
						oCSVWriter.WriteRow(oFinalValues)
					Else
						oCSVWriter.WriteRow(oValues)
					End If
				End While
			End If

			oCSVWriter.Flush()
			iLinesWritten = oCSVWriter.CurrentLine

		Catch ex As Exception
			oException = ex
		Finally
			If oSQLConnection IsNot Nothing Then oSQLConnection.Close()
		End Try

		If oException IsNot Nothing Then Throw oException
		Return iLinesWritten
	End Function

#End Region

#Region "sqllogging"

	''' <summary>
	''' Class used to for logging 
	''' </summary>
	Public Class SQLLogging

		''' <summary>
		''' Begins logging
		''' </summary>
		Public Shared Function Start() As SQLLogHandler

			'hack - trying to access the request from application_start was causing a problem on azure
			Try

				'query string
				If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing AndAlso HttpContext.Current.Request.QueryString.ToString.Contains("emailsqllog=") Then
					Return New SQLLogHandler With {.ProcessEnd = True, .Ticks = Date.Now.Ticks,
					 .EmailTo = HttpContext.Current.Request.QueryString("emailsqllog")}
				End If

				'support toolbar
				If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Session Is Nothing AndAlso Not HttpContext.Current.Session("__websupporttoolbar_sql") Is Nothing Then
					Return New SQLLogHandler With {.ProcessEnd = True, .Ticks = Date.Now.Ticks, .WebSupportToolbar = True}
				End If
			Catch ex As Exception

			End Try

			Return New SQLLogHandler
		End Function

		''' <summary>
		''' Finish logging, creates log and send email if EmailTo on the LogHandler is set.
		''' </summary>
		''' <param name="CallingProcedure">The calling procedure.</param>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="SQL">The SQL.</param>
		''' <param name="LogHandler">The log handler.</param>
		Public Shared Sub [End](CallingProcedure As String, ConnectString As String, SQL As String, LogHandler As SQLLogHandler)

			'build up email body
			Dim sb As New System.Text.StringBuilder
			sb.Append("Call Proc        ").Append(CallingProcedure).Append(Environment.NewLine)
			sb.Append("Connect String   ").Append(ConnectString).Append(Environment.NewLine)
			sb.Append("Total Time (ms)  ").Append(((Date.Now.Ticks - LogHandler.Ticks) / 10000).ToString("N")).Append(Environment.NewLine)

			'stack trace
			Dim oStackTrace As Generic.List(Of String) = Intuitive.Diagnostics.StackTrace
			Dim sLabel As String = "Stack Trace      "
			For Each sMethod As String In oStackTrace
				sb.Append(sLabel).Append(sMethod).Append(Environment.NewLine)
				sLabel = "                 "
			Next

			'sql
			sb.Append(Environment.NewLine)
			sb.Append(SQL)

			'websupport toolbar?
			If LogHandler.WebSupportToolbar Then
				Dim sbLog As System.Text.StringBuilder = CType(HttpContext.Current.Session("__websupporttoolbar_sql"), System.Text.StringBuilder)
				sbLog.Append(sb.ToString)
				sbLog.AppendLines(3)
			End If

			'send email?
			If LogHandler.EmailTo <> "" Then

				Dim oEmail As New Intuitive.Email(True)
				With oEmail
					.FromEmail = "sqltimes@ivector.co.uk"
					.From = "SQL Logging"
					.Subject = "SQL Logging - " & CallingProcedure
					.EmailTo = LogHandler.EmailTo
					.Body = sb.ToString
					.SendEmail()
				End With

			End If
		End Sub

		''' <summary>
		''' Support class to assist with logging sql functions
		''' </summary>
		Public Class SQLLogHandler
			Public Property ProcessEnd As Boolean = False
			Public Property Ticks As Long
			Public Property EmailTo As String = ""
			Public Property WebSupportToolbar As Boolean = False
		End Class
	End Class

#End Region

End Class
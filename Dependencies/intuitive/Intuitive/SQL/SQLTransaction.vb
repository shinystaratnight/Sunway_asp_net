Imports System.Text

''' <summary>
''' Class used to execute a list of sql statements.
''' </summary>
Public Class SQLTransaction

	''' <summary>
	''' List of transactions
	''' </summary>
	Private oTransactions As New Generic.List(Of String)

	''' <summary>
	''' Clears transactions
	''' </summary>
	Public Sub Clear()
		oTransactions = New Generic.List(Of String)
	End Sub

	''' <summary>
	''' Adds the specified SQL formatted with parameters to the list of transactions.
	''' </summary>
	''' <param name="sSql">The s SQL.</param>
	''' <param name="aParams">a parameters.</param>
	Public Sub Add(ByVal sSql As String, ByVal ParamArray aParams() As Object)
		oTransactions.Add(SQL.FormatStatement(sSql, aParams))
	End Sub

	''' <summary>
	''' Gets a value indicating whether this instance has transactions.
	''' </summary>
	Public ReadOnly Property HasTransactions() As Boolean
		Get
			Return oTransactions.Count > 0
		End Get
	End Property

	''' <summary>
	''' Builds query from the list of transactions.
	''' </summary>
	Public Function BuildQuery() As String

		Dim sb As New StringBuilder

		For Each sTransactions As String In oTransactions
			sb.AppendLine(sTransactions)
		Next

		Return sb.ToString

	End Function

	''' <summary>
	''' Executes the sql built from the list of transactions
	''' </summary>
	Public Function Execute() As Integer
		Return Me.Execute(SQL.ConnectString)
	End Function

	''' <summary>
	''' Executes the sql built from the list of transactions against the specified connect string
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">No Sql Statements have been added to be executed</exception>
	Public Function Execute(ByVal ConnectString As String) As Integer

		If Not Me.HasTransactions() Then
			Throw New Exception("No Sql Statements have been added to be executed")
		End If

		Return SQL.ExecuteWithConnectString(ConnectString, Me.BuildQuery)

	End Function

	''' <summary>
	''' Executes sql built from the list of transactions
	''' </summary>
	Public Function ExecuteSingleValue() As Integer
		Return Me.ExecuteSingleValue(SQL.ConnectString)
	End Function

	''' <summary>
	''' Executes sql built from the list of transactions against specified connect string and returns a single value.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">No Sql Statements have been added to be executed</exception>
	Public Function ExecuteSingleValue(ByVal ConnectString As String) As Integer

		If Not Me.HasTransactions() Then
			Throw New Exception("No Sql Statements have been added to be executed")
		End If

		Return SQL.ExecuteSingleValueWithConnectString(ConnectString, Me.BuildQuery)

	End Function

	''' <summary>
	''' Asynchronously executes the sql built from the list of transactions.
	''' </summary>
	''' <exception cref="System.Exception">No Sql Statements have been added to be executed</exception>
	Public Sub AsyncExecute()

		If Not Me.HasTransactions() Then
			Throw New Exception("No Sql Statements have been added to be executed")
		End If

		AsyncSQL.Execute(SQL.ConnectString, Me.BuildQuery)
	End Sub

	''' <summary>
	''' Asynchronously executes sql built from the list of transactions against the specified connect string.
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <exception cref="System.Exception">No Sql Statements have been added to be executed</exception>
	Public Sub AsyncExecute(ByVal ConnectString As String)

		If Not Me.HasTransactions() Then
			Throw New Exception("No Sql Statements have been added to be executed")
		End If

		AsyncSQL.Execute(ConnectString, Me.BuildQuery)
	End Sub

End Class
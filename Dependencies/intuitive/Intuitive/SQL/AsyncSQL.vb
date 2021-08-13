Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' 
''' </summary>
Public Class AsyncSQL

	''' <summary>
	''' Asynchronously executes the specified SQL with specified params against the specified connect string
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="SQL">The SQL to execute.</param>
	''' <param name="aParams">Array of parameters for the SQL.</param>
	Public Overloads Shared Sub Execute(ByVal ConnectString As String, SQL As String, ByVal ParamArray aParams() As Object)
		SQL = Intuitive.SQL.FormatStatement(SQL, aParams)
		AsyncSQL.Execute(ConnectString, SQL)
	End Sub

	''' <summary>
	''' Asynchronously executes the specified SQL against the specified connect string
	''' </summary>
	''' <param name="ConnectString">The connect string.</param>
	''' <param name="SQL">The SQL to execute.</param>
	Public Overloads Shared Sub Execute(ByVal ConnectString As String, SQL As String)
		'if we have an httpcontext then load this as a little man job
		'otherwise just execute synchronously as this is all the async sql job did before
		If System.Web.HttpContext.Current IsNot Nothing Then
			Dim oAsyncSQL As New LittleManOnTheServer.SQLJobNoTimeOut(ConnectString, SQL)
			oAsyncSQL.CustomLogFile = "Async SQL Error"
			LittleManOnTheServer.CurrentInstance.AddJob(oAsyncSQL)
		Else
			Intuitive.SQL.ExecuteWithConnectString(ConnectString, SQL)
		End If
	End Sub

End Class
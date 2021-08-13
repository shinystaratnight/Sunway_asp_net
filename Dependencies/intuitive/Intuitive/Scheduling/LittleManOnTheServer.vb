Imports System
Imports System.Web
Imports System.Text.RegularExpressions
Imports System.Collections.Generic

''' <summary>
''' Class that provides functionality for scheduled tasks
''' </summary>
Public Class LittleManOnTheServer

#Region "Properties"

	''' <summary>
	''' The SMTP host
	''' </summary>
	Public SMTPHost As String
	''' <summary>
	''' The address to email errors to
	''' </summary>
	Public EmailErrorsTo As String
	''' <summary>
	''' The list of timer jobs
	''' </summary>
	Private Shared oTimerJobs As New Generic.List(Of TimerJob)

	''' <summary>
	''' Gets the current instance.
	''' </summary>
	Public Shared ReadOnly Property CurrentInstance() As LittleManOnTheServer
		Get
			If HttpContext.Current IsNot Nothing Then
				Threading.Interlocked.CompareExchange(Of Object)(HttpContext.Current.Application("LittleManOnTheServer"), New LittleManOnTheServer, Nothing)
				Return CType(HttpContext.Current.Application("LittleManOnTheServer"), LittleManOnTheServer)
			Else
				Return Nothing
			End If
		End Get
	End Property

	''' <summary>
	''' Gets the <see cref="LittleManOnTheServerConfig"/>.
	''' </summary>
	Private ReadOnly Property Config() As LittleManOnTheServer.LittleManOnTheServerConfig
		Get
			Return TryCast(System.Configuration.ConfigurationManager.GetSection("IntuitiveConfig/LittleManOnTheServer"), LittleManOnTheServer.LittleManOnTheServerConfig)
		End Get
	End Property

	''' <summary>
	''' Gets all Timer jobs.
	''' </summary>
	Public Shared ReadOnly Property AllTimerJobs As Generic.List(Of TimerJob)
		Get
			Return oTimerJobs
		End Get
	End Property

#End Region

#Region "Constructor(s)"

	''' <summary>
	''' Initializes a new instance of the <see cref="LittleManOnTheServer"/> class.
	''' </summary>
	''' <param name="bCheckDatabaseForEmailSettings">The ivectorerrors email address no longer exists so this method is no longer supported</param>
	Public Sub New(Optional ByVal bCheckDatabaseForEmailSettings As Boolean = True)
				Me.SMTPHost = ""
				Me.EmailErrorsTo = ""
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="LittleManOnTheServer"/> class, setting the SMTPHost and Error email address.
	''' </summary>
	''' <param name="sSMTPHost">The s SMTP host.</param>
	''' <param name="sEmailErrorsTo">The s email errors to.</param>
	Public Sub New(ByVal sSMTPHost As String, ByVal sEmailErrorsTo As String)
		Me.SMTPHost = sSMTPHost
		Me.EmailErrorsTo = sEmailErrorsTo
	End Sub

#End Region

#Region "Add Job"

	'Public Delegate Sub ExecuteDelegate()

	''' <summary>
	''' Adds a job to the timer jobs list.
	''' </summary>
	''' <param name="oJob">The job to add.</param>
	Public Sub AddJob(ByVal oJob As LittleManOnTheServer.IJob)

		Dim oExecuteHelper As New LittleManOnTheServer.ExecuteHelper
		With oExecuteHelper
			.Job = oJob
			.LittleManOnTheServerConfig = Me.Config
			.SMTPHost = Me.SMTPHost
			.EmailErrorsTo = Me.EmailErrorsTo
		End With

		'long running tasks, or heavy load should not use the thread pool - left in just in case we want to use it later - TK
		'Dim oDelegate As New LittleManOnTheServer.ExecuteDelegate(AddressOf oExecuteHelper.ExecuteJob)
		'oDelegate.BeginInvoke(Nothing, Nothing)

		Dim oThread As New Threading.Thread(AddressOf oExecuteHelper.ExecuteJob)
		oThread.IsBackground = True
		oThread.Start()

		If oJob IsNot Nothing AndAlso GetType(TimerJob).IsAssignableFrom(oJob.GetType) Then
			Dim oTimerJob As TimerJob = DirectCast(oJob, TimerJob)
			oTimerJob.Thread = oThread
			LittleManOnTheServer.AllTimerJobs.Add(oTimerJob)
		End If

	End Sub

#End Region

#Region "Execute Helper"

	''' <summary>
	''' Class to prevent the application pool from crashing because of unhandled thread exceptions
	''' </summary>
	Private Class ExecuteHelper

		''' <summary>
		''' The job to execute
		''' </summary>
		Public Job As LittleManOnTheServer.IJob

		''' <summary>
		''' The little man on the server configuration
		''' </summary>
		Public LittleManOnTheServerConfig As LittleManOnTheServer.LittleManOnTheServerConfig
		''' <summary>
		''' The SMTP host
		''' </summary>
		Public SMTPHost As String
		''' <summary>
		''' The address to email errors to
		''' </summary>
		Public EmailErrorsTo As String

		''' <summary>
		''' Executes the job.
		''' </summary>
		Public Sub ExecuteJob()

			Try

				Me.Job.Execute()

			Catch ex As Exception

				'for timer jobs add some extra info
				If Me.Job IsNot Nothing AndAlso GetType(TimerJob).IsAssignableFrom(Me.Job.GetType) Then

					Dim oTimerjob As TimerJob = CType(Me.Job, TimerJob)

					oTimerjob.LastException = ex
					oTimerjob.LastExceptionDate = Date.Now

				End If

				'bomb out if the thread is being aborted
				If ex.GetType() = GetType(Threading.ThreadAbortException) Then
					Intuitive.FileFunctions.AddLogEntry("LittleManOnTheServer", "Unhandled Exception Caught", Functions.SafeString(ex))
					Return
				End If

				'otherwise, deal with the exception
				If Functions.IsDebugging Then

					'Throw ex

				ElseIf Me.LittleManOnTheServerConfig Is Nothing OrElse LittleManOnTheServerConfig.PreventErrorLogging Then

					Dim sbMessage As New System.Text.StringBuilder
					sbMessage.AppendFormat("Time: {0}", Date.Now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine()
					sbMessage.AppendFormat("Server Name: {0}", System.Environment.MachineName.ToUpper).AppendLine()
                    sbMessage.AppendLine()
					sbMessage.Append(ex.ToString).AppendLine()
					sbMessage.AppendLine()
					sbMessage.AppendLine("Job XML:")

					'if this fails to serialize, then we don't want to crash
					Try
						sbMessage.Append(XMLFunctions.FormatXML(Serializer.Serialize(Me.Job, True).InnerXml))
					Catch oSerializationException As Exception
						sbMessage.Append(oSerializationException.Message)
					End Try

					'try to send an email
					If Me.SMTPHost <> "" AndAlso Me.EmailErrorsTo <> "" Then

						Dim oEmail As New Email

						With oEmail
							.SMTPHost = Me.SMTPHost
							.EmailTo = Me.EmailErrorsTo
							.From = "Little Man On The Server"
							.FromEmail = "littlemanontheserver@ivector.co.uk"
							.Subject = "Unhandled Exception Caught"
							.Body = sbMessage.ToString
							.SendEmail()
						End With

					End If

					'log to a file as well
					Intuitive.FileFunctions.AddLogEntry("LittleManOnTheServer", "Unhandled Exception Caught", sbMessage.ToString)

					sbMessage = Nothing

				End If

			End Try

		End Sub

	End Class

#End Region

#Region "SQL Jobs"

	''' <summary>
	''' Class representing a SQL little man job
	''' </summary>
	''' <seealso cref="Intuitive.LittleManOnTheServer.IJob" />
	Public Class SQLJob
		Implements LittleManOnTheServer.IJob

		''' <summary>
		''' The connect string
		''' </summary>
		Public ConnectString As String

		''' <summary>
		''' The SQL string
		''' </summary>
		Public SQLString As String

		''' <summary>
		''' The desired name for the log file to use for logging
		''' </summary>
		Public CustomLogFile As String = "SQL Error"

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJob"/> class.
		''' </summary>
		Public Sub New()
			'this is needed for inheritance and for error logging to succeed
			'IMPORTANT: if you derive from this class then make sure you add this constructor too
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJob"/> class.
		''' </summary>
		''' <param name="SQLString">The SQL string.</param>
		''' <param name="Params">The parameters for sql query.</param>
		Public Sub New(ByVal SQLString As String, ByVal ParamArray Params() As Object)
			Me.New(SQL.ConnectString, SQLString, Params)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJob"/> class.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="SQLString">The SQL string.</param>
		''' <param name="Params">The parameters for the sql query.</param>
		Public Sub New(ByVal ConnectString As String, ByVal SQLString As String, ByVal ParamArray Params() As Object)
			Me.New(ConnectString, SQL.FormatStatement(SQLString, Params))
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJob"/> class.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="SQLString">The SQL string.</param>
		Public Sub New(ByVal ConnectString As String, ByVal SQLString As String)
			Me.ConnectString = ConnectString
			Me.SQLString = SQLString
		End Sub

		''' <summary>
		''' Executes this instance.
		''' </summary>
		''' <exception cref="System.Exception">Connect string has not been specified, SQL Statement has not been specified, SQL Task Failed</exception>
		Public Overridable Sub Execute() Implements IJob.Execute

			If Me.ConnectString = "" Then Throw New Exception("Connect string has not been specified")
			If Me.SQLString = "" Then Throw New Exception("SQL Statement has not been specified")

			Try

				Dim oConnection As New SqlClient.SqlConnection(Me.ConnectString)
				oConnection.Open()

				Dim oCommand As New SqlClient.SqlCommand(Me.SQLString, oConnection)
				oCommand.CommandTimeout = oConnection.ConnectionTimeout
				oCommand.ExecuteScalar()
				oConnection.Close()

			Catch ex As Exception
				Intuitive.FileFunctions.AddLogEntry("LittleManOnTheServer", Me.CustomLogFile, Me.SQLString)
				Throw New Exception("SQL Task Failed: " & Me.SQLString, ex)
			End Try

		End Sub

	End Class

	''' <summary>
	''' Class representing a sql little man job with no timeout
	''' </summary>
	''' <seealso cref="Intuitive.LittleManOnTheServer.SQLJob" />
	Public Class SQLJobNoTimeOut
		Inherits SQLJob

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJobNoTimeOut"/> class.
		''' </summary>
		Public Sub New()
			'this is needed for error logging to succeed
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJobNoTimeOut"/> class.
		''' </summary>
		''' <param name="SQLString">The SQL string.</param>
		''' <param name="Params">The parameters for the sql query.</param>
		Public Sub New(ByVal SQLString As String, ByVal ParamArray Params() As Object)
			MyBase.New(SQL.ConnectString, SQLString, Params)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJobNoTimeOut"/> class.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="SQLString">The SQL string.</param>
		''' <param name="Params">The parameters for the sql query.</param>
		Public Sub New(ByVal ConnectString As String, ByVal SQLString As String, ByVal ParamArray Params() As Object)
			MyBase.New(ConnectString, SQLString, Params)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SQLJobNoTimeOut"/> class.
		''' </summary>
		''' <param name="ConnectString">The connect string.</param>
		''' <param name="SQLString">The SQL string.</param>
		Public Sub New(ByVal ConnectString As String, ByVal SQLString As String)
			MyBase.New(ConnectString, SQLString)
		End Sub

		''' <summary>
		''' Executes this instance.
		''' </summary>
		Public Overrides Sub Execute()
			Me.ConnectString = Regex.Replace(Me.ConnectString, "connect\stimeout=[0-9]+", "connect timeout=0")
			MyBase.Execute()
		End Sub

	End Class

#End Region

#Region "VB Jobs"

	''' <summary>
	''' Abstract class for VB little man jobs
	''' </summary>
	''' <seealso cref="Intuitive.LittleManOnTheServer.IJob" />
	Public MustInherit Class VBJob
		Implements IJob

		''' <summary>
		''' Executes this instance.
		''' </summary>
		Public MustOverride Sub Execute() Implements IJob.Execute

	End Class

#End Region

#Region "Job Interface and TimerJob class"

	''' <summary>
	''' Interface for little man jobs
	''' </summary>
	Public Interface IJob
		''' <summary>
		''' Executes this instance.
		''' </summary>
		Sub Execute()
	End Interface

	''' <summary>
	''' Abstract class for Timed jobs
	''' </summary>
	''' <seealso cref="Intuitive.LittleManOnTheServer.IJob" />
	Public MustInherit Class TimerJob
		Implements IJob

		''' <summary>
		''' The interval in seconds between executions
		''' </summary>
		Public IntervalSeconds As Integer = 30

		''' <summary>
		''' Indicated whether to keep running after executing
		''' </summary>
		Public KeepRunning As Boolean = True

		''' <summary>
		''' Indicated whether the job is enabled
		''' </summary>
		Public Enabled As Boolean = True

		''' <summary>
		''' The last time the job was run
		''' </summary>
		Public LastRan As DateTime = Date.Now

		''' <summary>
		''' The list of logs
		''' </summary>
		Public Log As New List(Of String)

		''' <summary>
		''' The time the job was started
		''' </summary>
		Public Started As DateTime = Date.Now

		''' <summary>
		''' Function to run when the timer fires
		''' </summary>
		Public MustOverride Sub TimerFire()

		''' <summary>
		''' Gets or sets the thread.
		''' </summary>
		Friend Property Thread As Threading.Thread
		''' <summary>
		''' The last exception to be raised
		''' </summary>
		Public LastException As Exception
		''' <summary>
		''' The date the last exeption was raised
		''' </summary>
		Public LastExceptionDate As DateTime = DateFunctions.EmptyDate

		''' <summary>
		''' Executes this instance.
		''' </summary>
		Public Sub Execute() Implements IJob.Execute

			Me.LogEntry("Task Started")

			While Me.KeepRunning

				If Me.Enabled Then
					Me.LastRan = Date.Now
					Me.TimerFire()
				End If

				System.Threading.Thread.Sleep(Me.IntervalSeconds * 1000)

			End While

			Me.LogEntry("Task Ended")

		End Sub

		'start
		''' <summary>
		''' Starts the job.
		''' </summary>
		Public Function StartJob() As String

			Dim sReturn As String = ""

			If Me.KeepRunning AndAlso Me.Thread.IsAlive Then
				sReturn = "Task already started."
			ElseIf Not Me.Thread.IsAlive Then
				Me.Thread = New Threading.Thread(AddressOf Me.Execute)
				Me.Thread.IsBackground = True
				Me.Thread.Start()
				Me.LastException = Nothing
				Me.LastExceptionDate = DateFunctions.EmptyDate
			End If

			If sReturn = "" Then
				Me.Enabled = True
			End If

			Return sReturn

		End Function

		'stop
		''' <summary>
		''' Stops the job.
		''' </summary>
		Public Function StopJob() As String

			Dim sReturn As String = ""

			If Not Me.KeepRunning Then
				sReturn = "Task not started."
			End If

			If sReturn = "" Then
				Me.Enabled = False
			End If

			Return sReturn

		End Function

		'log
		''' <summary>
		''' Adds new log entry
		''' </summary>
		''' <param name="LogEntry">The log entry.</param>
		Public Sub LogEntry(ByVal LogEntry As String)

			If Me.Log.Count = 200 Then
				Me.Log.RemoveAt(0)
			End If

			Log.Add(Date.Now.ToString("dd/MM/yyyy HH:mm:ss") & " - " & LogEntry)

		End Sub

	End Class

#End Region

#Region "Support Classes And Enums"

	''' <summary>
	''' Class containing configuration information for the <see cref="LittleManOnTheServer"/>
	''' </summary>
	Public Class LittleManOnTheServerConfig
		''' <summary>
		''' Indicates whether to prevent error logging
		''' </summary>
		Public PreventErrorLogging As Boolean = False
	End Class

#End Region

End Class
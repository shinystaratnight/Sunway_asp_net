Imports System.Drawing
Imports System.Web

''' <summary>
''' Class used for logging processes and times
''' </summary>
Public Class ProcessTimer

	''' <summary>
	''' Gets or sets the timer unique identifier.
	''' </summary>
	Public Property TimerGUID As Guid

	''' <summary>
	''' Gets or sets the name of the process.
	''' </summary>
	Public Property ProcessName As String

	''' <summary>
	''' Gets or sets the times.
	''' </summary>
	Public Property Times As New TimerItems

	''' <summary>
	''' Gets or sets the step number.
	''' </summary>
	Private Property iStepNumber As Integer = 0

	''' <summary>
	''' The lock object used for synclocking
	''' </summary>
	Private Lock As New Object

#Region "Record Start"

	''' <summary>
	''' Adds start step
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	Public Sub RecordStart(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.Start, Process, ProcessTimerItemType.General, 0)
	End Sub

	''' <summary>
	''' Adds start step
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	Public Sub RecordStart(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.Start, Process, ItemType, Level)
	End Sub

	''' <summary>
	''' Adds start step
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	''' <param name="Ticks">The ticks.</param>
	Public Sub RecordStart(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer, ByVal Ticks As Long)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.Start, Process, ItemType, Level, Ticks)
	End Sub

#End Region

#Region "Record End"

	''' <summary>
	''' Adds end step
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	Public Sub RecordEnd(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.End, Process, ProcessTimerItemType.General, 0)
	End Sub

	''' <summary>
	''' Adds end step
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	Public Sub RecordEnd(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.End, Process, ItemType, Level)
	End Sub

#End Region

#Region "Record Single Event"

	''' <summary>
	''' Records single event.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	Public Sub RecordSingleEvent(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String)
		Me.RecordSingleEvent(ApplicationName, StepName, Process, ProcessTimerItemType.General)
	End Sub

	''' <summary>
	''' Records single event.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	Public Sub RecordSingleEvent(ByVal ApplicationName As String, ByVal StepName As String, ByVal Process As String, ByVal ItemType As ProcessTimerItemType)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.Start, Process, ItemType, 0)
		Me.RecordStep(ApplicationName, StepName, ProcessTimerStartEnd.End, Process, ItemType, 0)
	End Sub

#End Region

#Region "Record Step"

	''' <summary>
	''' Records new step.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="StepName">Name of the step.</param>
	''' <param name="StopStart"><see cref="ProcessTimerStartEnd"/></param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	Private Sub RecordStep(ByVal ApplicationName As String, ByVal StepName As ProcessTimerStep, ByVal StopStart As ProcessTimerStartEnd, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer)
		Me.RecordStep(ApplicationName, StepName, StopStart, String.Empty, ItemType, Level)
	End Sub

	''' <summary>
	''' Records new step.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="Step">The step.</param>
	''' <param name="StopStart"><see cref="ProcessTimerStartEnd"/></param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	Private Sub RecordStep(ByVal ApplicationName As String, ByVal [Step] As ProcessTimerStep, ByVal StopStart As ProcessTimerStartEnd,
	  ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer)
		Me.RecordStep(ApplicationName, [Step].ToString, StopStart, Process, ItemType, 0)
	End Sub
	''' <summary>
	''' Records new step.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="Step">The step.</param>
	''' <param name="StopStart"><see cref="ProcessTimerStartEnd"/></param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	Private Sub RecordStep(ByVal ApplicationName As String, ByVal [Step] As String, ByVal StopStart As ProcessTimerStartEnd,
	  ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer)
		Me.RecordStep(ApplicationName, [Step].ToString, StopStart, Process, ItemType, Level, Date.Now.Ticks)
	End Sub

	''' <summary>
	''' Records a new step.
	''' </summary>
	''' <param name="ApplicationName">Name of the application.</param>
	''' <param name="Step">The step.</param>
	''' <param name="StopStart"><see cref="ProcessTimerStartEnd"/></param>
	''' <param name="Process">The process.</param>
	''' <param name="ItemType">Type of the item.</param>
	''' <param name="Level">The level.</param>
	''' <param name="Ticks">The ticks.</param>
	Private Sub RecordStep(ByVal ApplicationName As String, ByVal [Step] As String, ByVal StopStart As ProcessTimerStartEnd,
	  ByVal Process As String, ByVal ItemType As ProcessTimerItemType, ByVal Level As Integer, ByVal Ticks As Long)

		Try

			If StopStart = ProcessTimerStartEnd.Start Then
				System.Threading.Interlocked.Increment(iStepNumber)
			End If

			Dim oItem As New TimerItem
			oItem.GUID = Me.TimerGUID
			oItem.Process = Process
			oItem.Step = [Step].ToString
			oItem.hlpApplicationName = ApplicationName
			oItem.StepNumber = Me.iStepNumber
			oItem.hlpApplicationName = ApplicationName
			oItem.ItemType = ItemType
			oItem.Level = Level

			If StopStart = ProcessTimerStartEnd.Start Then
				oItem.MaxThreadCount = GetMaxThreads
				oItem.MinThreadCount = GetMinThreads
				oItem.AvailableThreadCount = GetAvailableThreads
			End If

			SyncLock (Lock)

				If StopStart = ProcessTimerStartEnd.Start Then
					oItem.StartTicks = Ticks
					Me.Times.Add(oItem)
				End If
				If StopStart = ProcessTimerStartEnd.End Then
					Dim sKey As String = TimerItem.CompositeKey(oItem)
					If Me.Times.ContainsKey(sKey) Then
						Me.Times(sKey).EndTicks = Ticks
					End If
				End If
			End SyncLock

		Catch
		End Try

	End Sub

#End Region

#Region "Save"

	''' <summary>
	''' Saves timer logs to server
	''' </summary>
	''' <param name="OverrideConnectString">The override connect string.</param>
	Public Sub Save(Optional ByVal OverrideConnectString As String = Nothing)

		Try

			'Save it to the database
			Dim oTask As New System.Threading.Tasks.Task(Sub() Save(Me.Times, OverrideConnectString))
			oTask.Start()

		Catch
		End Try

	End Sub

	''' <summary>
	''' Saves timer logs to server
	''' </summary>
	''' <param name="TimerItems">The timer items.</param>
	''' <param name="OverrideConnectString">The override connect string.</param>
	Private Sub Save(ByVal TimerItems As TimerItems, Optional ByVal OverrideConnectString As String = Nothing)

		Try

			If OverrideConnectString = Nothing Then OverrideConnectString = SQL.ConnectString

			Dim TimeTable As DataTable = TimerItems.Unbind()

			Using oBulkCopy As New System.Data.SqlClient.SqlBulkCopy(OverrideConnectString)

				'Map the columns 1 to 1
				For iColumn As Integer = 0 To TimeTable.Columns.Count - 1
					oBulkCopy.ColumnMappings.Add(iColumn, iColumn + 1) 'Add one onto the source to take into account the ID column
				Next

				'Copy the data table to the database using bulk insert
				oBulkCopy.DestinationTableName = "ProcessStepTime"
				oBulkCopy.WriteToServer(TimeTable)

			End Using

		Catch ex As Exception
			FileFunctions.AddLogEntry("ProcessStepTime", "Exception", ex.ToString)

		End Try

	End Sub

#End Region

#Region "Constuctor"

	''' <summary>
	''' Initializes a new instance of the <see cref="ProcessTimer"/> class.
	''' </summary>
	Public Sub New()
		MyBase.New()
		Me.TimerGUID = Guid.NewGuid
		Me.ProcessName = "Main"
	End Sub

#End Region

#Region "Time table class"

	''' <summary>
	''' Class representing a table containing information about process recorder times
	''' </summary>
	''' <seealso cref="System.Data.DataTable" />
	Public Class TimeTable
		Inherits DataTable

		''' <summary>
		''' Initializes a new instance of the <see cref="TimeTable"/> class.
		''' </summary>
		Public Sub New()
			MyBase.New()
			Me.Columns.Add("GUID", GetType(Guid))
			Me.Columns.Add("ApplicationName", GetType(String))
			Me.Columns.Add("Process", GetType(String))
			Me.Columns.Add("Step", GetType(String))
			Me.Columns.Add("StepNumber", GetType(Integer))
			Me.Columns.Add("StartTicks", GetType(Long))
			Me.Columns.Add("EndTicks", GetType(Long))
		End Sub

	End Class

#End Region

#Region "Timer Item"

	''' <summary>
	''' Class representing a collection of <see cref="TimerItem"/>
	''' </summary>
	''' <seealso cref="T:System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}" />
	Public Class TimerItems
		Inherits System.Collections.Concurrent.ConcurrentDictionary(Of String, TimerItem) 'Thread safe dictionary
		'Inherits Generic.Dictionary(Of String, TimerItem)

		'Public Sub New()
		'	MyBase.New(System.Environment.ProcessorCount, 101) 'prime numbers are best for capacity apparently
		'End Sub

		''' <summary>
		''' Adds the specified timer item.
		''' </summary>
		''' <param name="TimerItem">The timer item.</param>
		Public Overloads Sub Add(ByVal TimerItem As TimerItem)
			Dim sKey As String = TimerItem.CompositeKey(TimerItem)
			If Me.ContainsKey(sKey) Then
				Me(sKey) = TimerItem
			Else
				Me.TryAdd(sKey, TimerItem)
			End If
		End Sub

		''' <summary>
		''' Returns <see cref="TimeTable"/> with values taken from <see cref="Keys"/>
		''' </summary>
		Public Function Unbind() As DataTable

			Dim oTable As New TimeTable

			For Each sKey As String In Me.Keys

				Dim oItem As TimerItem = Me(sKey)

				Dim oRow As DataRow = oTable.NewRow
				oRow("GUID") = oItem.GUID
				oRow("ApplicationName") = oItem.hlpApplicationName
				oRow("Process") = oItem.Process.ToString
				oRow("Step") = oItem.Step.ToString
				oRow("StepNumber") = oItem.StepNumber
				oRow("StartTicks") = oItem.StartTicks
				oRow("EndTicks") = oItem.EndTicks

				oTable.Rows.Add(oRow)

			Next

			Return oTable

		End Function

	End Class

	''' <summary>
	''' Class containing information about a specific timer entry
	''' </summary>
	Public Class TimerItem

		''' <summary>
		''' The unique identifier
		''' </summary>
		Public GUID As Guid
		''' <summary>
		''' The process
		''' </summary>
		Public Process As String = String.Empty
		''' <summary>
		''' The application name
		''' </summary>
		Public hlpApplicationName As String = String.Empty
		''' <summary>
		''' The appliction color
		''' </summary>
		Public hlpApplictionColor As System.Drawing.Color = Color.FromArgb(128, 128, 128)
		''' <summary>
		''' The step
		''' </summary>
		Public [Step] As String = String.Empty
		''' <summary>
		''' The step number
		''' </summary>
		Public StepNumber As Integer = 0
		''' <summary>
		''' The start ticks
		''' </summary>
		Public StartTicks As Long = 0
		''' <summary>
		''' The end ticks
		''' </summary>
		Public EndTicks As Long = 0
		''' <summary>
		''' The level
		''' </summary>
		Public Level As Integer = 0
		''' <summary>
		''' The item type
		''' </summary>
		Public ItemType As ProcessTimerItemType

		''' <summary>
		''' The maximum thread count
		''' </summary>
		Public MaxThreadCount As Integer
		''' <summary>
		''' The minimum thread count
		''' </summary>
		Public MinThreadCount As Integer
		''' <summary>
		''' The available thread count
		''' </summary>
		Public AvailableThreadCount As Integer

		''' <summary>
		''' Gets the composite key.
		''' </summary>
		Public Shared ReadOnly Property CompositeKey(ByVal Item As TimerItem) As String
			Get
				Return TimerItem.CompositeKey(Item.hlpApplicationName, Item.Step, Item.Process)
			End Get
		End Property

		''' <summary>
		''' Gets the composite key consiting of ApplicationName|Step|Process
		''' </summary>
		Public Shared ReadOnly Property CompositeKey(ByVal ApplicationName As String, ByVal [Step] As String, ByVal Process As String) As String
			Get
				Return ApplicationName & "|" & [Step].ToString & "|" & Process.ToString
			End Get
		End Property

	End Class

#End Region

#Region "Session"
	Private Const ProcessTimerSession As String = "__processtimer"
	''' <summary>
	''' Gets or sets the current process timer.
	''' </summary>
	Public Shared Property CurrentProcessTimer(Optional ByVal RequiresReturn As Boolean = False) As ProcessTimer
		Get
			Dim oProcessTimer As ProcessTimer = Nothing
			If RequiresReturn Then oProcessTimer = New ProcessTimer

			If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then

				If HttpContext.Current.Session(ProcessTimerSession) Is Nothing Then
					HttpContext.Current.Session(ProcessTimerSession) = oProcessTimer
				Else
					oProcessTimer = CType(HttpContext.Current.Session(ProcessTimerSession), ProcessTimer)
				End If

			End If

			Return oProcessTimer
		End Get
		Set(ByVal value As ProcessTimer)
			If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
				HttpContext.Current.Session(ProcessTimerSession) = value
			End If
		End Set
	End Property

	'Public Shared Property ProcessTimers As Generic.Dictionary(Of ProcessTimeType, ProcessTimer)
	'	Get
	'		If HttpContext.Current.Session("search_processtimers") Is Nothing Then
	'			HttpContext.Current.Session("search_processtimers") = New Generic.Dictionary(Of ProcessTimeType, ProcessTimer)
	'		End If

	'		Return CType(HttpContext.Current.Session("search_processtimers"), Generic.Dictionary(Of ProcessTimeType, ProcessTimer))

	'	End Get
	'	Set(ByVal value As Generic.Dictionary(Of ProcessTimeType, ProcessTimer))
	'		HttpContext.Current.Session("search_processtimers") = value
	'	End Set
	'End Property

	''' <summary>
	''' Enum for possible process timer types
	''' </summary>
	Public Enum ProcessTimeType
		CentralisedSearch
		HotelSplitSearch
		FlightSplitSearch
	End Enum

#End Region

#Region "Thread Info"

	''' <summary>
	''' Gets the number of active threads.
	''' </summary>
	Private Shared ReadOnly Property GetActiveThreads() As Integer
		Get
			Dim iActiveThreads As Integer
			iActiveThreads = GetMaxThreads - GetAvailableThreads
			Return iActiveThreads
		End Get
	End Property

	''' <summary>
	''' Gets the number of available threads
	''' </summary>
	Private Shared ReadOnly Property GetAvailableThreads() As Integer
		Get
			Dim iAvailableWorkerThreads As Integer
			Dim iAvailableIOThreads As Integer
			Threading.ThreadPool.GetAvailableThreads(iAvailableWorkerThreads, iAvailableIOThreads)
			Return iAvailableWorkerThreads ' Only really interested in the worker threads not the IO threads
		End Get
	End Property

	''' <summary>
	''' Gets the max number of threads.
	''' </summary>
	Private Shared ReadOnly Property GetMaxThreads As Integer
		Get
			Dim iMaxWorkerThreads As Integer
			Dim iMaxIOThreads As Integer
			Threading.ThreadPool.GetMaxThreads(iMaxWorkerThreads, iMaxIOThreads)
			Return iMaxWorkerThreads ' Only really interested in the worker threads not the IO threads
		End Get
	End Property

	''' <summary>
	''' Gets the get minimum threads.
	''' </summary>
	Private Shared ReadOnly Property GetMinThreads As Integer
		Get
			Dim iMinWorkerThreads As Integer
			Dim iMinIOThreads As Integer
			Threading.ThreadPool.GetMinThreads(iMinWorkerThreads, iMinIOThreads)
			Return iMinWorkerThreads ' Only really interested in the worker threads not the IO threads
		End Get
	End Property

#End Region

	''' <summary>
	''' Gets the main process.
	''' </summary>
	Public Shared ReadOnly Property MainProcess As String
		Get
			Dim sProcess As String = "Main"
			If System.Configuration.ConfigurationManager.AppSettings("ApplicationName") IsNot Nothing Then
				sProcess += " " & System.Configuration.ConfigurationManager.AppSettings("ApplicationName")
			End If
			Return sProcess
		End Get
	End Property

End Class

Public Class ProcessTimerStep
	Public Const Unknown As String = "Unknown"
	'Public Const ThreadStarted As String = "ThreadStarted"
	Public Const ThreadedSearch As String = "ThreadedSearch"
	Public Const BuildingRequests As String = "BuildingRequests"
	Public Const PerformingSearch As String = "PerformingSearch"
	Public Const SendSearchRequestThread As String = "SendSearchRequestThread"
	Public Const InsertIntoSQL As String = "InsertIntoSQL"
	Public Const CostExhaust As String = "CostExhaust"
	Public Const EmailLogsTo As String = "EmailLogsTo"
	Public Const TransformSearchResponse As String = "TransformSearchResponse"
	Public Const SyncLockRequest As String = "SyncLockRequest"
	Public Const ProcessingResponseException As String = "ProcessingResponseException"
	Public Const SQLSearch As String = "SQLSearch"
	Public Const SearchException As String = "SearchException"
	Public Const SearchThreadException As String = "SearchThreadException"
	Public Const WaitForTasks As String = "WaitForTasks"
	Public Const ThreadCancelled As String = "ThreadCancelled"
	'Public Const SendingRequests As String = "SendingRequests"
	Public Const AddTask As String = "AddTask"
	Public Const ProcessingResponses As String = "ProcessingResponses"
	Public Const AllRequests As String = "AllRequests"
	Public Const VBDedupe As String = "VBDedupe"
	Public Const VBDedupeResults As String = "VBDedupeResults"
	Public Const VBDedupeGetSQL As String = "VBDedupeGetSQL"

	Public Const ThreadMakeSearchDecision As String = "ThreadMakeSearchDecision"
	Public Const ThreadGettingResortSplits As String = "ThreadGettingResortSplits"
	Public Const ThreadProcessingPropertySupplier As String = "ThreadProcessingPropertySupplier"
	Public Const MakeWebRequest As String = "MakeWebRequest"
End Class

Public Enum ProcessTimerStartEnd
	[Start]
	[End]
End Enum

''' <summary>
''' Enum for possible timer types
''' </summary>
Public Enum ProcessTimerItemType
	General
	Flight
	[Property]
End Enum
Imports System.Threading.Tasks
Imports System.Threading
Imports System.Xml


Namespace DataStore

    ''' <summary>
    ''' This class is responsible for taking sets of specific data and logging to a database.
    ''' It should not be a store of any data.
    ''' </summary>
    ''' <remarks>
    ''' Any global values, eg connect strings or setting values should be passed into the constructor.
    ''' </remarks>
    Public NotInheritable Class Logger

        Private _LogData As Boolean
        Private _LogConnectString As String
        ' Public Const SessionCustomerIdKey As String = "__datalogger_customerid"
        ' Public Const SessionGuidKey As String = "__datalogger_guid"
        'Public Const SessionVisitIdKey As String = "__datalogger_visitid"
        ' Public Const CookieCustomerIdKey As String = "__dl_cid"
        Public Const CookieCustomerGuidKey As String = "__dl_guid"

        Public Sub New()
        End Sub
        Public Sub New(LogData As Boolean, LogConnectString As String)
            Me._LogData = LogData
            Me._LogConnectString = LogConnectString
        End Sub

#Region "Public logging methods"

        Public Function LogSessionStarted(CustomerGuid As String, StartDateTime As DateTime, UserAgent As String, Browser As String, BrowserVersion As String,
                                     OperatingSystem As String, IPAddress As String, Server As String, SessionID As String) As String

            If Not Me._LogData Then Return ""

            '2a if unavailable, generate GUID and set on session (use GUID in case SQL has not returned Customer Id by the time we log something else)
            If CustomerGuid = "" Then
                CustomerGuid = System.Guid.NewGuid.ToString
            End If

            Try

                Task.Factory.StartNew(Sub() Me.MakeNewVisitTransaction(StartDateTime, UserAgent, Browser, BrowserVersion, OperatingSystem,
                                                                                 IPAddress, Server, SessionID, CustomerGuid))

            Catch ex As Exception
                Me.HandleError(ex)
            End Try

            Return CustomerGuid

        End Function

        Public Sub LogSearch(SearchDetails As BookingSearch)

            If Not Me._LogData Then Exit Sub

            Try

                Dim oSearchStore As New Search.SearchStore(SearchDetails)
                Dim oSearchStoreXml As XmlDocument = Intuitive.Serializer.Serialize(oSearchStore, True)
                Task.Factory.StartNew(Sub() Me.MakeSearchStoreTransaction(oSearchStoreXml))

            Catch ex As Exception
                Me.HandleError(ex)
            End Try


            'ThreadPool.QueueUserWorkItem(Function() Me.MakeSearchStoreTransaction(SearchDetails))

        End Sub

        Public Sub LogPageView(RequestTimes As List(Of Logging.RequestTime),
                              WidgetSpeeds As List(Of Logging.WidgetSpeed),
                              VisitID As Integer,
                              SessionID As String,
                              SearchGuid As String,
                              Url As String,
                              TotalRequestTime As Double)

            If Not Me._LogData Then Exit Sub

            Try

                Dim oRequestLog As New DataStore.PageRequest.PageRequestLog(RequestTimes, WidgetSpeeds)
                Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oRequestLog)
                Task.Factory.StartNew(Sub() Me.MakePageViewTransaction(VisitID, SessionID, SearchGuid, Url, TotalRequestTime, oXML))

            Catch ex As Exception
                Me.HandleError(ex)
            End Try

        End Sub

        Public Sub LogBrowserPerformance()

            If Not Me._LogData Then Exit Sub

        End Sub


#End Region

#Region "Error handling"


        Private Sub HandleError(Exception As Exception)

            Logging.LogError("DataStore", "Logger", Exception)

        End Sub


#End Region

#Region "Data transactions"


        Private Sub MakeSearchStoreTransaction(SearchStoreXml As XmlDocument)

            Dim sSQL As String = String.Format("exec LogSearchStore {0}",
                                             SQL.GetSqlValue(SearchStoreXml.InnerXml, SQL.SqlValueType.XML))
        
            Intuitive.SQL.ExecuteWithoutReturnWithConnectString(Me._LogConnectString, sSQL)

       
        End Sub


        Private Sub MakePageViewTransaction(VisitID As Integer,
                              SessionID As String,
                              SearchGuid As String,
                              Url As String,
                              TotalRequestTime As Double,
                              PageViewTimesXML As XmlDocument)

            'Dim sSQL As String = String.Format("exec LogPageView {0}, {1}, {2}, {3}, {4}, {5}, {6}",
            '                                 SQL.GetSqlValue(Url, SQL.SqlValueType.String),
            '                                 VisitID,
            '                                 SQL.GetSqlValue(SessionID, SQL.SqlValueType.String),
            '                                 SQL.GetSqlValue(SearchGuid, SQL.SqlValueType.String),
            '                                 SQL.GetSqlValue(DateTime.Now, SQL.SqlValueType.DateTime),
            '                                 SQL.GetSqlValue(TotalRequestTime, SQL.SqlValueType.Double),
            '                                 SQL.GetSqlValue(PageViewTimesXML.InnerXml, SQL.SqlValueType.XML))

            'Intuitive.SQL.ExecuteWithoutReturnWithConnectString(Me._LogConnectString, sSQL)

       
        End Sub

        Private Sub MakeNewVisitTransaction(StartDateTime As DateTime, UserAgent As String, Browser As String, BrowserVersion As String,
                                     OperatingSystem As String, IPAddress As String, Server As String, SessionID As String, Guid As String)

                'Dim sSQL As String = String.Format("exec LogNewVisit {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                '                            SQL.GetSqlValue(Guid, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(SessionID, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(StartDateTime, SQL.SqlValueType.DateTime),
                '                            SQL.GetSqlValue(UserAgent, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(Browser, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(BrowserVersion, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(OperatingSystem, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(IPAddress, SQL.SqlValueType.String),
                '                            SQL.GetSqlValue(Server, SQL.SqlValueType.String))

                'Intuitive.SQL.ExecuteWithoutReturnWithConnectString(Me._LogConnectString, sSQL)

        End Sub

#End Region




    End Class


End Namespace
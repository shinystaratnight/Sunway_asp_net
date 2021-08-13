Imports System.Xml
Imports System.Text
Imports System.Configuration.ConfigurationManager
Imports Intuitive
Imports Intuitive.Functions
Imports System.Net

Public Class Logging

#Region "Fields and properties"

    Public Shared SessionKey As String = "usersession_ivclogging"

    Public Shared Property Current As Logging
        Get
            If Not BookingBase.Session.Get(SessionKey) Is Nothing Then
                Return CType(BookingBase.Session.Get(SessionKey), Logging)
            Else
                Dim oIVCLogging As New Logging
                BookingBase.Session.Set(SessionKey, oIVCLogging)
                Return oIVCLogging
            End If
        End Get
        Set(ByVal value As Logging)
            BookingBase.Session.Set(SessionKey, value)
        End Set
    End Property

    Public IVCLogs As New Generic.List(Of IVCLog)
    Public PageEvents As New PageRequestEvents(HttpContext.Current.Request.Url.ToString)
    Public WidgetSpeeds As New List(Of WidgetSpeed)

#End Region

#Region "IVC Logs"


    Public Sub AddIVCLog(ByVal Name As String, ByVal ResponseTime As Long,
                         Optional ByVal Response As XmlDocument = Nothing,
                         Optional ByVal Request As XmlDocument = Nothing)

        Dim oIVCLog As New IVCLog
        With oIVCLog
            .Name = Name
            .ResponseTime = ResponseTime
            .Request = Request
            .Response = Response
        End With

        Me.IVCLogs.Add(oIVCLog)

    End Sub

    Public Sub ClearIVCLogs()
        Me.IVCLogs.Clear()
    End Sub


    Public Class IVCLog
        Public Name As String
        Public ResponseTime As Long
        Public Request As XmlDocument
        Public Response As XmlDocument
    End Class


#End Region

#Region "request times"

    Public Class PageRequestEvents

        Public Url As String
        Public CurrentEvents As New List(Of RequestTime)
        Public PreviousEvents As New List(Of RequestTime)

        Private EventDictionary As New Dictionary(Of RequestTime.eEvent, Double)

        Public Sub New(Url As String)
            Me.Url = Url
        End Sub

        Public Sub Log([event] As RequestTime.eEvent, stage As RequestTime.eStage)

            'ajax requests make this confusing at the moment as we call Response.End and this skips Application_PostRequestHandlerExecute in global asax
            'so if it contains executeformfunction just ignore it
            If Me.Url.Contains("executeformfunction") Then Exit Sub


            Dim path As String = HttpContext.Current.Request.Path
            Dim watch As System.Diagnostics.Stopwatch = CType(HttpContext.Current.Items("__global_stopwatch"), System.Diagnostics.Stopwatch)
            Dim timeInSeconds As Double = watch.ElapsedMilliseconds / 1000

            If Not watch Is Nothing Then

                'if key not present, AND it is the start - then add it to event dictionary
                If Not Me.EventDictionary.ContainsKey([event]) AndAlso stage = RequestTime.eStage.Start Then
                    Me.EventDictionary.Add([event], timeInSeconds)
                End If

                'else - if the key is present AND it is the end, add to CurrentEvents
                If Me.EventDictionary.ContainsKey([event]) AndAlso stage = RequestTime.eStage.End Then
                    Dim startTime As Double = Me.EventDictionary([event])
                    Dim timeDiff As Double = Math.Truncate((timeInSeconds - startTime) * 1000) / 1000
                    Me.CurrentEvents.Add(New RequestTime([event], timeDiff))
                    Me.EventDictionary.Remove([event])
                End If
                
            End If
        End Sub

    End Class


    Public Class RequestTime
        Public Sub New()
        End Sub
        Public Sub New([event] As eEvent, time As Double)
            Me.[Event] = [event]
            Me.TimeInSeconds = time
        End Sub
        Public [Event] As eEvent
        Public TimeInSeconds As Double

        Public Enum eEvent
            Setup
            Translation
            MainRender
            VersionFiles
            LinkChecker
        End Enum

        Public Enum eStage
            Start
            [End]
        End Enum

    End Class


#End Region


#Region "Widgets"

    Public Class WidgetSpeed
        Public Sub New()
        End Sub
        Public Sub New(name As String, time As Double)
            Me.Name = name
            Me.TimeInSeconds = time
        End Sub

        Public Name As String
        Public TimeInSeconds As Double
    End Class

   


#End Region

#Region "WebErrorHandling"

    Public Shared Sub LogError(ByVal sModule As String, ByVal sTitle As String, ByVal PageException As Exception, Optional ByVal sDetails As String = "")

        Try

            Dim sError As String = CreateWebPageErrorEmailBody(sModule, sTitle, PageException, sDetails)

            If Not sError = "" Then
                FileFunctions.AddLogEntry(sModule, sTitle, sError)

                Dim oEmail As New Email
                With oEmail
                    .EmailTo = BookingBase.Params.ErrorEmail
                    .SMTPHost = BookingBase.Params.SMTPHost
                    .FromEmail = "admin@intuitivesystems.co.uk"
                    .From = "WebTemplateLogging"
                    .Subject = "WebTemplateError"
                    .Body = sError
                    .SendEmail()
                End With
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Shared Function CreateWebPageErrorEmailBody(ByVal sModule As String,
                                                        ByVal sTitle As String,
                                                        ByVal PageException As Exception,
                                                        Optional ByVal sDetails As String = "") As String

        If PageException.Message.ToLower = "file does not exist." OrElse PageException.Message.ToLower = "failed to execute url." Then
            'We want to bomb out and not send email if this is a file not found error.
            Return ""
        End If

        Dim iPad As Integer = 30
        'user and date info
        Dim sb As New StringBuilder

        sb.AppendLine("***Error***")
        sb.AppendPad("Date and Time", 30).Append(Now.ToString("dd MMM yy HH:mm")).Append("\n")

        ' We need to build up a base url using the errorurl we retrieved
        Dim sBaseURL As String = String.Empty


        'gather as much error info as possible about the server, pass that back along with the whole exception
        sb.AppendPad("Server Name", 30).Append(Dns.GetHostName).Append("\n")
        If Not HttpContext.Current.Server Is Nothing Then sb.AppendPad("Physical Path", 30).Append(HttpContext.Current.Server.MapPath("/")).Append("\n")

        Dim sVersion As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString
        sb.AppendPad("Software version", 30).Append(sVersion).AppendLine()

        Try

            Dim sSQLConnection() As String = SQL.ConnectString.Split(";"c)
            For Each sAttribute As String In sSQLConnection
                If sAttribute.ToLower.StartsWith("server=") Then
                    sb.AppendPad("SQL Server", 30).Append(sAttribute.ToLower.Replace("server=", "")).Append("\n")
                End If
                If sAttribute.ToLower.StartsWith("database=") Then
                    sb.AppendPad("Database", 30).Append(sAttribute.ToLower.Replace("database=", "")).Append("\n")
                End If
            Next
        Catch ex As Exception

        End Try


        Dim oHost As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName)
        For Each oIPAddress As IPAddress In oHost.AddressList
            sb.AppendPad("IP Address", iPad).Append(oIPAddress.ToString).Append("\n")
        Next

        'client info
        sb.AppendPad("Client IP", iPad).Append(HttpContext.Current.Request.UserHostAddress).Append("\n")
        sb.AppendPad("Client Host Name", iPad).Append(HttpContext.Current.Request.UserHostName).Append("\n")

        'Exception
        sb.AppendLine(" ")
        sb.AppendLine("***Module***")
        sb.AppendPad("Module", iPad).Append(sModule).Append("\n")
        sb.AppendPad("Title", iPad).Append(sTitle).Append("\n")

        If sDetails <> "" Then
            sb.AppendLine(" ")
            sb.AppendLine("***Details***")
            sb.AppendLine(sDetails)
        End If

        sb.AppendLine(" ")
        sb.AppendLine("***Exception***")
        sb.AppendLine(PageException.ToString)

        Return sb.ToString.Replace("\n", Environment.NewLine)


    End Function


#End Region

End Class

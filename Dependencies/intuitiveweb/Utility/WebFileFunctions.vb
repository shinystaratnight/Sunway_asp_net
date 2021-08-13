Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.IO
Imports Intuitive.Functions
Imports System.Web

Public Class FileFunctions

#Region "log path and url proprties"

    Public Shared ReadOnly Property LogPath As String
        Get
            'try the normal way that happens in intuitive
            Dim sLogPath As String = SafeString(AppSettings("LogPath"))

            'if empty, check the booking base params
            If sLogPath = "" Then
                sLogPath = SafeString(BookingBase.Params.LogPath)
            End If

            Return sLogPath
        End Get
    End Property


    Public Shared ReadOnly Property LogURL As String
        Get
            Dim sURL As String = SafeString(AppSettings("LogURL"))
            If Not sURL = "" AndAlso Not sURL.EndsWith("/") Then sURL &= "/"
            Return sURL
        End Get
    End Property

#End Region

    Public Shared Function AddLogEntry(ByVal [Module] As String, ByVal Title As String, ByVal Text As String, _
   Optional ByVal Email As Boolean = False, Optional ByVal EmailTo As String = "", _
   Optional ByVal SMTPHost As String = "", Optional ByVal XMLDoc As XmlDocument = Nothing) As String

        Try

            '1 changed to have 1 filename and then add .txt or .xml later on
            Dim sTextFile As String = ""
            Dim sXMLFile As String = ""

            '2a standard file logging
            If FileFunctions.LogPath <> "" Then

                'make sure folder exists
                Dim sFolderBase As String = FileFunctions.LogPath & Now.ToString("yyMM MMM yy") & "\" & [Module] & "\" & Now.ToString("yyMMdd dd MMM yy")
                Intuitive.FileFunctions.CreateFolder(sFolderBase)

                sTextFile = GetLogFileName(sFolderBase, Title, False)
                sXMLFile = GetLogFileName(sFolderBase, Title, True)


                'if normal text then save .txt file
                If Text <> "" Then
                    'create log file
                    Dim oFile As New FileStream(sTextFile, FileMode.Create, FileAccess.Write)

                    'write to file
                    Dim oStreamWriter As New StreamWriter(oFile)
                    oStreamWriter.Write(Text)
                    oStreamWriter.Flush()
                    oFile.Close()
                End If

                'if we've got an xml doc then just save it
                If Not XMLDoc Is Nothing Then

                    Try
                        XMLDoc.Save(sXMLFile)
                    Catch ex As Exception

                    End Try

                End If

            End If


            '2b proper logging
            If FileFunctions.LogURL <> "" Then

                '2bi build url
                Dim sb As New System.Text.StringBuilder(FileFunctions.LogURL)
                sb.Append("log.ashx?")

                sb.Append("server=").Append(System.Environment.MachineName)
                Try
                    If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing Then
                        Dim sURL As String = HttpContext.Current.Request.Url.AbsoluteUri
                        If Not HttpContext.Current.Request.Url.Query = "" Then
                            sURL = sURL.Replace(HttpContext.Current.Request.Url.Query, "")
                        End If
                        sb.Append("&url=").Append(sURL)
                    End If
                Catch ex As Exception

                End Try
                sb.Append("&assembly=").Append(System.Reflection.Assembly.GetExecutingAssembly.FullName.Split(","c)(0))
                sb.Append("&module=").Append([Module])
                sb.Append("&category=").Append(Title)


                '2bii build payload
                Dim sbLogDetail As New System.Text.StringBuilder
                If Not Text = "" Then
                    sbLogDetail.Append(Text)
                End If

                If Not XMLDoc Is Nothing Then
                    If Text <> "" Then
                        sbLogDetail.Append(ControlChars.NewLine).Append(ControlChars.NewLine)
                        sbLogDetail.AppendLine("XML")
                        sbLogDetail.AppendLine("===")
                    End If
                    sbLogDetail.Append(Intuitive.XMLFunctions.FormatXML(XMLDoc.InnerXml))
                End If


                '2biii send
                Intuitive.Net.WebRequests.GetResponse(sb.ToString, sbLogDetail.ToString, False, "")


            End If


            'send email if needed
            If Email Then

                Dim oEmail As New Email
                With oEmail
                    .EmailTo = EmailTo
                    .From = "Logging"
                    .FromEmail = "logging@intuitivesystems.co.uk"
                    .SMTPHost = SMTPHost
                    .Subject = [Module] & " " & Title
                    .Body = Text

                    If Not XMLDoc Is Nothing Then
                        .Body = .Body & ControlChars.NewLine & "XML" _
                         & ControlChars.NewLine & "===" & _
                         ControlChars.NewLine & Intuitive.XMLFunctions.FormatXML(XMLDoc.InnerXml)
                    End If

                    .SendEmail()
                End With
            End If


            'return filename 15/01/2013 DH
            Return sTextFile.Replace(".txt", "")

        Catch ex As Exception
            Return ""
        End Try


    End Function

    Private Shared Function GetLogFileName(ByVal FolderBase As String, ByVal Title As String, bXML As Boolean) As String

        Dim sLogFileName As String = String.Format("{0}\{1}_{2}", FolderBase, Intuitive.FileFunctions.SafeFileName(Title, False),
         Now.ToString("ddMMyyyyHHmmss") & Now.Millisecond.ToString) & IIf(bXML, ".xml", ".txt")

        'if file exists hold on for a few millisecond to ensure unique file log names
        While File.Exists(sLogFileName)
            System.Threading.Thread.Sleep(2)
            sLogFileName = String.Format("{0}\{1}_{2}", FolderBase, Intuitive.FileFunctions.SafeFileName(Title, False),
              Now.ToString("ddMMyyyyHHmmss") & Now.Millisecond.ToString) & IIf(bXML, ".xml", ".txt")
        End While

        Return sLogFileName

    End Function

End Class

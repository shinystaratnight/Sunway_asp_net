Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.IO
Imports Intuitive.Functions
Imports Intuitive.Net.WebRequests

''' <summary>
''' Utility class that contains logging methods
''' </summary>
Partial Public Class FileFunctions

#Region "log path and url proprties"

	''' <summary>
	''' Gets the log path from the configuration file.
	''' </summary>
	Public Shared ReadOnly Property LogPath As String
		Get
			Return SafeString(AppSettings("LogPath"))
		End Get
	End Property

	''' <summary>
	''' Gets a value indicating whether external logging is allowed.
	''' </summary>
	Public Shared ReadOnly Property AllowExternalLogging As Boolean
		Get
			Return SafeBoolean(AppSettings("AllowExternalLogging"))
		End Get
	End Property

	''' <summary>
	''' Gets the log URL from the configuration file.
	''' </summary>
	Public Shared ReadOnly Property LogURL As String
		Get
			Dim sURL As String = SafeString(AppSettings("LogURL"))
			If Not sURL = "" AndAlso Not sURL.EndsWith("/") Then sURL &= "/"
			Return sURL
		End Get
	End Property

#End Region

	''' <summary>
	''' Adds a new log entry
	''' Saves to logpath specified in the configuration file.
	''' Saves through log handler at specified url in configuration file.
	''' Emails log to speficied email.
	''' </summary>
	''' <param name="Module">The module.</param>
	''' <param name="Title">The title.</param>
	''' <param name="Text">The text.</param>
	''' <param name="Email">if set to <c>true</c> [email].</param>
	''' <param name="EmailTo">The email to.</param>
	''' <param name="SMTPHost">The SMTP host.</param>
	''' <param name="XMLDoc">The XML document.</param>
	''' <returns></returns>
	Public Shared Function AddLogEntry(
		ByVal [Module] As String,
		ByVal Title As String,
		ByVal Text As String,
		Optional ByVal Email As Boolean = False,
		Optional ByVal EmailTo As String = "",
		Optional ByVal SMTPHost As String = "",
		Optional ByVal XMLDoc As XmlDocument = Nothing,
		Optional sURL As String = "") As String

		Try

			'1 changed to have 1 filename and then add .txt or .xml later on
			Dim sTextFile As String = ""
			Dim sXMLFile As String = ""
			Dim sLogPath As String = ""

			If AllowExternalLogging AndAlso Not String.IsNullOrEmpty(SQL.ConnectString) Then
				sLogPath = SQL.GetValue("select dbo.GetSetting('Config', 'CloudLogsPath')")
			End If

			Dim bLocalLog As Boolean = True

			'2a amazon file logging
			If sLogPath <> "" Then
				Try
					Dim oStorage As New AmazonStorage(sLogPath)
					If Text <> "" Then
						Dim fileInfo As New AmazonStorage.TextFileInfo(String.Empty, Title)
						Dim oStoreReturn As AmazonStorage.StoreReturn = oStorage.StoreFile([Module], Text, fileInfo)
						sTextFile = oStoreReturn.Url
						bLocalLog = False
					End If
					If Not XMLDoc Is Nothing Then
						Dim fileInfo As New AmazonStorage.TextFileInfo(String.Empty, Title & "_XML")
						Dim oStoreReturn As AmazonStorage.StoreReturn = oStorage.StoreFile([Module], XMLDoc.InnerXml, fileInfo)
						sXMLFile = oStoreReturn.Url
						bLocalLog = False
					End If
				Catch ex As Exception
					sLogPath = LogPath
					bLocalLog = True
				End Try
			Else
				sLogPath = LogPath
			End If

			'2b standard file logging
			If sLogPath <> "" AndAlso bLocalLog Then

				'make sure folder exists
				Dim sFolderBase As String = sLogPath & Date.Now.ToString("yyMM MMM yy") & "\" & [Module] & "\" & Date.Now.ToString("yyMMdd dd MMM yy")

				FileFunctions.CreateFolder(sFolderBase)

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

			'2c proper logging
			If FileFunctions.LogURL <> "" Then

				'2bi build url
				Dim sb As New System.Text.StringBuilder(FileFunctions.LogURL)
				sb.Append("log.ashx?")

				sb.Append("server=").Append(System.Environment.MachineName)
				sb.Append("&url=").Append(sURL)
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
						sbLogDetail.Append(Environment.NewLine).Append(Environment.NewLine)
						sbLogDetail.AppendLine("XML")
						sbLogDetail.AppendLine("===")
					End If
					sbLogDetail.Append(Intuitive.XMLFunctions.FormatXML(XMLDoc.InnerXml))
				End If

				'2biii send
				Dim oRequest As New Request
				With oRequest
					.EndPoint = sb.ToString
					.Method = eRequestMethod.POST
					.ContentType = ContentType.Application_x_www_form_urlencoded
					.SetRequest(sbLogDetail.ToString)
					.Send()
				End With

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
						.Body = .Body & Environment.NewLine & "XML" _
						 & Environment.NewLine & "===" &
						 Environment.NewLine & Intuitive.XMLFunctions.FormatXML(XMLDoc.InnerXml)
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

	''' <summary>
	''' Creates a log file name based on the folderbase, the title and the current time.
	''' If file name already exists, waits a few milliseconds before trying to create a unique filename again.
	''' </summary>
	''' <param name="FolderBase">The folder base.</param>
	''' <param name="Title">The title.</param>
	''' <param name="bXML">if set to <c>true</c> will give the file .xml extension.</param>
	''' <returns></returns>
	Private Shared Function GetLogFileName(ByVal FolderBase As String, ByVal Title As String, bXML As Boolean) As String

		Dim sLogFileName As String = String.Format("{0}\{1}_{2}", FolderBase, FileFunctions.SafeFileName(Title, False),
			Date.Now.ToString("ddMMyyyyHHmmss") & Date.Now.Millisecond.ToString) & IIf(bXML, ".xml", ".txt")

		'if file exists hold on for a few millisecond to ensure unique file log names
		While File.Exists(sLogFileName)
			Threading.Thread.Sleep(2)
			sLogFileName = String.Format("{0}\{1}_{2}", FolderBase, FileFunctions.SafeFileName(Title, False),
			  Date.Now.ToString("ddMMyyyyHHmmss") & Date.Now.Millisecond.ToString) & IIf(bXML, ".xml", ".txt")
		End While

		Return sLogFileName

	End Function

End Class
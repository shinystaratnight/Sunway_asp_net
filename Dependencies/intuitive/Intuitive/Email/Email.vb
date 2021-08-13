Imports System.Net.Mail
Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports System.Linq
Imports Intuitive.Functions
Imports System.Configuration.ConfigurationManager
Imports Amazon.S3

''' <summary>
''' Class that represents an email
''' </summary>
Public Class Email
	Implements IEmail

#Region "Properties"

	''' <summary>
	''' The SMTP host
	''' </summary>
	Private sSMTPHost As String

	''' <summary>
	''' The SMTP port.
	''' </summary>
	Public Property SMTPPort As Integer = 0 Implements IEmail.SMTPPort

	''' <summary>
	''' Who the email is from
	''' </summary>
	Private sFrom As String

	''' <summary>
	''' The from email address
	''' </summary>
	Private sFromEmail As String

	''' <summary>
	''' The email to
	''' </summary>
	Private sEmailTo As String

	''' <summary>
	''' The CC email
	''' </summary>
	Private sCC As String

	''' <summary>
	''' The BCC Email
	''' </summary>
	Private sBCC As String

	''' <summary>
	''' The email subject
	''' </summary>
	Private sSubject As String

	''' <summary>
	''' The email body
	''' </summary>
	Private sBody As String

	''' <summary>
	''' List of attachments
	''' </summary>
	Private aAttachments As New ArrayList

	''' <summary>
	''' The last error
	''' </summary>
	Private sLastError As String

	''' <summary>
	''' The priority of the email
	''' </summary>
	Private sPriority As System.Net.Mail.MailPriority = System.Net.Mail.MailPriority.Normal

	''' <summary>
	''' The unique message identifier
	''' </summary>
	Private sMessageID As String

	''' <summary>
	''' Whether the email will be an html email or not
	''' </summary>
	Public HTMLFormat As Boolean = False

	''' <summary>
	''' The SMTP Username
	''' </summary>
	Public Property Username As String = "" Implements IEmail.Username

	''' <summary>
	''' The SMTP Password
	''' </summary>
	Public Property Password As String = "" Implements IEmail.Password

	''' <summary>
	''' Gets or sets the SMTP host.
	''' </summary>
	Public Property SMTPHost() As String Implements IEmail.SMTPHost
		Get
			Return sSMTPHost
		End Get
		Set(ByVal Value As String)
			sSMTPHost = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the from field.
	''' </summary>
	Public Property From() As String Implements IEmail.From
		Get
			Return sFrom
		End Get
		Set(ByVal Value As String)
			sFrom = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the from email.
	''' </summary>
	Public Property FromEmail() As String Implements IEmail.FromEmail
		Get
			Return sFromEmail
		End Get
		Set(ByVal Value As String)
			sFromEmail = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the email to field.
	''' </summary>
	Public Property EmailTo() As String Implements IEmail.EmailTo
		Get
			Return sEmailTo
		End Get
		Set(ByVal Value As String)
			sEmailTo = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the CC field
	''' </summary>
	Public Property CC() As String Implements IEmail.CC
		Get
			Return sCC
		End Get
		Set(ByVal Value As String)
			sCC = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the BCC field
	''' </summary>
	Public Property BCC() As String Implements IEmail.BCC
		Get
			Return sBCC
		End Get
		Set(ByVal Value As String)
			sBCC = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the subject field.
	''' </summary>
	Public Property Subject() As String Implements IEmail.Subject
		Get
			Return sSubject
		End Get
		Set(ByVal Value As String)
			sSubject = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the email body.
	''' </summary>
	Public Property Body() As String Implements IEmail.Body
		Get
			Return sBody
		End Get
		Set(ByVal Value As String)
			sBody = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the attachments.
	''' </summary>
	Public Property Attachments() As ArrayList Implements IEmail.Attachments
		Get
			Return aAttachments
		End Get
		Set(ByVal Value As ArrayList)
			aAttachments = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the last error.
	''' </summary>
	Public Property LastError() As String Implements IEmail.LastError
		Get
			Return sLastError
		End Get
		Set(ByVal Value As String)
			sLastError = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the priority.
	''' </summary>
	Public Property Priority() As System.Net.Mail.MailPriority Implements IEmail.Priority
		Get
			Return sPriority
		End Get
		Set(ByVal Value As System.Net.Mail.MailPriority)
			sPriority = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the message identifier.
	''' </summary>
	Public Property MessageID() As String
		Get
			If sMessageID <> String.Empty Then
				Return sMessageID
			ElseIf AppSettings.Get("EmailDomain") <> String.Empty Then
				Return String.Format("<{0}@{1}>", Guid.NewGuid.ToString, AppSettings.Get("EmailDomain"))
			End If

			Return String.Empty
		End Get
		Set(ByVal Value As String)
			sMessageID = Value
		End Set
	End Property

	Public Property S3Bucket() As String = String.Empty

	Private Const S3RegionPart As String = ".s3-eu-west-1"


	''' <summary>
	''' Gets or sets the stream of email attachments.
	''' </summary>
	<XmlIgnore()>
	Public Property StreamAttachments As New Generic.Dictionary(Of String, MemoryStream) Implements IEmail.StreamAttachments

#End Region

#Region "Constructor"

	''' <summary>
	''' Initializes a new instance of the <see cref="Email"/> class.
	''' </summary>
	Public Sub New()

	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="Email"/> class.
	''' </summary>
	''' <param name="LoadServerSettingsFromConfig">if set to <c>true</c> loads the SMTPHost, SMTPPort, SMTPUsername and SMTPPassword from the config file.</param>
	Public Sub New(ByVal LoadServerSettingsFromConfig As Boolean)

		If LoadServerSettingsFromConfig Then
			Me.SMTPHost = SafeString(AppSettings("SMTPHost"))
			Me.SMTPPort = SafeInt(AppSettings("SMTPPort"))
			Me.Username = SafeString(AppSettings("SMTPUsername"))
			Me.Password = SafeString(AppSettings("SMTPPassword"))
		End If

	End Sub

#End Region

#Region "Send"

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <returns>A flag indicating whether the email was sent successfully or not</returns>
	Public Function SendEmail(Optional ByVal bHTMLFormat As Boolean = False) As Boolean Implements IEmail.SendEmail

		Dim sError As String = ""
		Dim sException As String = ""
		Try

			'validate properties
			If sSMTPHost = "" Then
				sError = "SMTP Host not specified"
			ElseIf sFrom = "" Then
				sError = "From Email not specified"
			ElseIf sEmailTo = "" Then
				sError = "To Email not specified"
			ElseIf sSubject = "" Then
				sError = "Subject not specified"
			ElseIf sFromEmail = "" Then
				sError = "From Email Address not specified"
			End If

			'create the email
			If sError = "" Then

				Using oMail As New MailMessage

					Dim sAttachment As String

					With oMail

						Me.HTMLFormat = bHTMLFormat
						.IsBodyHtml = bHTMLFormat

						If sFromEmail <> "" Then
							.From = New MailAddress(sFromEmail, sFrom)
						Else
							.From = New MailAddress(sFromEmail)
						End If

						If sCC <> "" Then
							For Each sEmailAddress As String In sCC.Split(";"c)
								.CC.Add(New MailAddress(sEmailAddress))
							Next
						End If

						If sBCC <> "" Then
							For Each sEmailAddress As String In sBCC.Split(";"c)
								.Bcc.Add(New MailAddress(sEmailAddress))
							Next
						End If

						For Each sEmailAddress As String In sEmailTo.Split(";"c)
							.To.Add(New MailAddress(sEmailAddress))
						Next

						.Subject = sSubject
						.Body = sBody
						.Priority = sPriority

						If MessageID <> String.Empty Then
							.Headers.Remove("Message-ID")
							.Headers.Add("Message-ID", MessageID)
						End If

						'check each attachment exists, if ok add it
						For Each sAttachment In aAttachments
							If File.Exists(sAttachment) Then
								.Attachments.Add(New Attachment(sAttachment))
							ElseIf sAttachment.Contains(S3RegionPart) Then
								S3Bucket = sAttachment.Substring(7, sAttachment.IndexOf(S3RegionPart) - 7) 'sAttachment should begin "http://[bucketname].s3-eu-west-1", hence the 7s
								Dim oRegion As Amazon.RegionEndpoint = Amazon.RegionEndpoint.EUWest1
								Dim oStorage As New AmazonStorage(New AmazonS3Client(oRegion), S3Bucket)
								Dim sFileName As String = sAttachment.Substring(sAttachment.IndexOf(".com/") + 5)
								Dim oResponse As AmazonStorage.SaveReturn = oStorage.SaveFile(sFileName)
								If oResponse.Success Then
									.Attachments.Add(New Attachment(oResponse.BaseStream, sFileName))
									.Attachments.Last.ContentDisposition.FileName = sFileName
								End If
							Else
								sError = "Could not find attachment " & sAttachment
								Exit For
							End If
						Next

						'Add in stream attachments
						For Each oAttachment As Generic.KeyValuePair(Of String, MemoryStream) In StreamAttachments
							If oAttachment.Value IsNot Nothing AndAlso System.IO.Path.GetExtension(oAttachment.Key) <> "" Then
								.Attachments.Add(New Attachment(oAttachment.Value, oAttachment.Key))
							Else
								sError = "Could not find attachment " & oAttachment.Key
							End If
						Next

					End With

					If sError = "" Then

						Dim oClient As SmtpClient = Nothing

						Using oClient

							'if we've specified a port, use it here
							If Me.SMTPPort > 0 Then
								oClient = New SmtpClient(sSMTPHost, Me.SMTPPort)
							Else
								oClient = New SmtpClient(sSMTPHost)
							End If

							'credentials if required
							If Me.Username <> "" AndAlso Me.Password <> "" Then
								oClient.Credentials = New System.Net.NetworkCredential(Me.Username, Me.Password)
							End If

							'send the email
							oClient.Send(oMail)

							'call the dispose directly as this was causing emails to intermittently fail for some clients
							oClient.Dispose()

						End Using

					End If

				End Using

			End If

		Catch ex As Exception
			sError = "Email Send Failed"
			sException = ex.ToString
		End Try

		If sError <> "" Then
			sLastError = sError
			Intuitive.FileFunctions.AddLogEntry("EmailSend", sError, BuildLog(sException))
		End If

		Return sError = ""

	End Function

#End Region

#Region "Add email address helpers"

	''' <summary>
	''' Adds specified address to the email, if an email address is already set, will append new email to the end using ;.
	''' </summary>
	''' <param name="EmailType">Type of address to add, EmailTo, CC or BCC.</param>
	''' <param name="EmailAddress">The email address to add.</param>
	Public Sub AddAddress(ByVal EmailType As eEmailType, ByVal EmailAddress As String) Implements IEmail.AddAddress

		If Validators.IsEmail(EmailAddress) Then

			Select Case EmailType
				Case eEmailType.EmailTo
					Me.EmailTo = IIf(Me.EmailTo = "", EmailAddress, Me.EmailTo & ";" & EmailAddress)
				Case eEmailType.CC
					Me.CC = IIf(Me.CC = "", EmailAddress, Me.CC & ";" & EmailAddress)
				Case eEmailType.BCC
					Me.BCC = IIf(Me.BCC = "", EmailAddress, Me.BCC & ";" & EmailAddress)
			End Select

		End If

	End Sub

	''' <summary>
	''' Enum for the type of email address, EmailTo, CC and BCC
	''' </summary>
	Public Enum eEmailType
		''' <summary>
		''' The email to
		''' </summary>
		EmailTo
		''' <summary>
		''' </summary>
		CC
		''' <summary>
		''' </summary>
		BCC
	End Enum

#End Region

#Region "Helpers"

	Private Function BuildLog(ByVal sException As String) As String
		Dim sbLog As New StringBuilder

		With sbLog
			.AppendFormatLine("SMTPHost: {0}", SMTPHost)
			.AppendFormatLine("SMTPPort: {0}", SMTPPort)
			.AppendFormatLine("Username: {0}", Username)
			.AppendFormatLine("Password: {0}", Password)
			.AppendFormatLine("From: {0}", Me.From)
			.AppendFormatLine("FromEmail: {0}", FromEmail)
			.AppendFormatLine("To: {0}", EmailTo)
			.AppendFormatLine("CC: {0}", CC)
			.AppendFormatLine("BCC: {0}", BCC)
			.AppendFormatLine("Subject: {0}", Subject)
			.AppendFormatLine("Body: {0}", Body)
			.AppendFormatLine("Attachments: {0}", String.Join(",", Attachments.ToArray))
			.AppendFormatLine("Priority: {0}", Priority)
			.AppendFormat("Exception: {0}", sException)
		End With

		Return sbLog.ToString
	End Function

#End Region

End Class
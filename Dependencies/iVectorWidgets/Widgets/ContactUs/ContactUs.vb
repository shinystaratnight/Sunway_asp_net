Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports System.Xml
Imports Intuitive.Web
Imports System.ComponentModel

Public Class ContactUs
	Inherits WidgetBase

	Public Shared Property ContactUsAddress As String
		Get
			If Not HttpContext.Current.Session("usersession_contactusaddress") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_contactusaddress"), String)
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("usersession_contactusaddress") = value
		End Set
	End Property

	Public Shared Property EmailSubject As String
		Get
			If Not HttpContext.Current.Session("usersession_emailsubject") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_emailsubject"), String)
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("usersession_emailsubject") = value
		End Set
	End Property

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oXML As New XmlDocument
		oXML = Utility.BigCXML(Me.ObjectType, 1, 60)

		'get label & placeholder overrides xml
		Dim oLabelOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("LabelOverrides"), "LabelOverrides")
		Dim oPlaceholderOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("PlaceholderOverrides"), "PlaceholderOverrides")

		'merge xml
		oXML = XMLFunctions.MergeXMLDocuments(oXML, oLabelOverridesXML, oPlaceholderOverridesXML)

		'get email address
		ContactUsAddress = SafeString(GetSetting(eSetting.EmailTo))

		'email subject
		If SafeString(GetSetting(eSetting.EmailSubject)) <> "" Then
			EmailSubject = GetSetting(eSetting.EmailSubject)
		Else
			EmailSubject = "Contact Us"
		End If

		'Get our various infomessages
		Dim oMessages As New InfoMessages
		oMessages = Me.Messages()

		'params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("IncludeContactInformation", GetSetting(eSetting.IncludeContactInformation).ToSafeBoolean.ToString.ToLower)
			.AddParam("IncludeMainImage", GetSetting(eSetting.IncludeMainImage).ToSafeBoolean.ToString.ToLower)
			.AddParam("IncludeBreadcrumbs", GetSetting(eSetting.IncludeBreadcrumbs).ToSafeBoolean.ToString.ToLower)
			.AddParam("ShowHeading", GetSetting(eSetting.ShowHeading).ToSafeBoolean.ToString.ToLower)
			.AddParam("WarningText", oMessages.WarningMessage)
			.AddParam("CaptchaWarningText", oMessages.CaptchaWarning)
			.AddParam("SuccessText", oMessages.SuccessMessage)
		End With

		'3. Transform
		If Intuitive.ToSafeString(Settings.GetValue("TemplateOverride")) <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & Intuitive.ToSafeString(Settings.GetValue("TemplateOverride"))), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.ContactUs, True, True), writer, oXSLParams)
		End If


	End Sub


	Public Overridable Sub SendEmail(ByVal QueryString As String)

		Dim oValues As Generic.Dictionary(Of String, String) = Functions.Web.ConvertQueryStringToDictionary(QueryString)

		Dim sFrom As String = oValues("txtName")
		Dim sFromEmail As String = oValues("txtEmail")

		Dim sb As New StringBuilder
		With sb

			.AppendLine("This is an automated email from the contact us page.")
			.AppendLines(2)

			.AppendFormatLine("Name: {0}", sFrom)
			.AppendFormatLine("Phone Number: {0}", oValues("txtPhoneNumber"))
			.AppendFormatLine("Email: {0}", sFromEmail)
			.AppendLines(1)

			.AppendLine("Message:")
			.AppendLine(oValues("txtMessage"))

		End With

		Dim oEmail As New Intuitive.Email
		With oEmail
			.SMTPHost = BookingBase.Params.SMTPHost
			.Subject = EmailSubject
			.From = sFrom
			.FromEmail = sFromEmail
			.EmailTo = ContactUsAddress
			.Body = sb.ToString
		End With

		oEmail.SendEmail()

	End Sub

#Region "helper functions"

	Public Function Messages() As InfoMessages

		Dim oMessages As New InfoMessages

		'success message
		If SafeString(GetSetting(eSetting.SuccessText)) <> "" Then
			oMessages.SuccessMessage = GetSetting(eSetting.SuccessText)
		Else
			oMessages.SuccessMessage = "Thank you for your message, we will be in touch."
		End If

		'warning message
		If SafeString(GetSetting(eSetting.WarningText)) <> "" Then
			oMessages.WarningMessage = GetSetting(eSetting.WarningText)
		Else
			oMessages.WarningMessage = "Please ensure that all required fields have been entered"
		End If

		'captcha message
		If SafeString(GetSetting(eSetting.CaptchaWarningText)) <> "" Then
			oMessages.CaptchaWarning = GetSetting(eSetting.CaptchaWarningText)
		Else
			oMessages.CaptchaWarning = "Captcha validation failed. Please Try again"
		End If

		Return oMessages

	End Function

	Public Class InfoMessages
		Public SuccessMessage As String
		Public WarningMessage As String
		Public CaptchaWarning As String
	End Class

#End Region

#Region "Settings"

	Public Enum eSetting
		<Title("Contact us XSL Template Override")>
		<Description("This setting specifies a template to override the iVectorWidgets ContactUs.XSL (if needed)")> _
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Include contact information")>
		<Description("This setting specifies where we show company contact information")> _
		IncludeContactInformation

		<Title("Include Breadcrumbs")>
		<Description("Decides whether we want breadcrumbs on the page or not")>
		IncludeBreadcrumbs

		<Title("Main Image")>
		<Description("Decides whether we want to show a main image")>
		IncludeMainImage

		<Title("Show Heading")>
		<Description("Decides whether we want to show an H1 on the page")>
		ShowHeading

		<Title("Warning message text")>
		<Description("The message that is displayed when something goes wrong")>
		WarningText

		<Title("Captcha Warning message text")>
		<Description("The message that is displayed when captcha validation fails")>
		CaptchaWarningText

		<Title("success message text")>
		<Description("The message that is displayed when the message is sent")>
		SuccessText

		<Title("Email subject")>
		<Description("The subject of the email")>
		EmailSubject

		<Title("Recipient address")>
		<Description("Who gets the email")>
		EmailTo
	End Enum

#End Region

End Class

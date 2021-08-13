Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions

Public Class MyBookingsLogin
	Inherits WidgetBase

	Public Shared Property CustomerID As Integer
		Get
			If Not HttpContext.Current.Session("usersession_customerid") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_customerid"), Integer)
			Else
				Return 0
			End If
		End Get
		Set(ByVal value As Integer)
			HttpContext.Current.Session("usersession_customerid") = value
		End Set
	End Property

	Public Shared Property BrandID As Integer
		Get
			If Not HttpContext.Current.Session("usersession_brandid") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_brandid"), Integer)
			Else
				Return 0
			End If
		End Get
		Set(ByVal value As Integer)
			HttpContext.Current.Session("usersession_brandid") = value
		End Set
	End Property

	Public Shared Property MyBookingsReference As String
		Get
			If Not HttpContext.Current.Session("usersession_mybookingsreference") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_mybookingsreference"), String)
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("usersession_mybookingsreference") = value
		End Set
	End Property

	Public Shared Property ManageMyBookingDetails As BookingManagement.BookingDetailsReturn
		Get
			If HttpContext.Current.Session("usersession_managemybookingdetails") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("usersession_managemybookingdetails"), BookingManagement.BookingDetailsReturn)
			End If
			Return New BookingManagement.BookingDetailsReturn
		End Get
		Set(value As BookingManagement.BookingDetailsReturn)
			HttpContext.Current.Session("usersession_managemybookingdetails") = value
		End Set
	End Property

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'Lets set up some custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.MMBURL = Intuitive.Functions.SafeString(Settings.GetValue("MMBURL"))
			.LoginType = Intuitive.Functions.SafeString(Settings.GetValue("LoginType"))
			.CSSClassOverride = Intuitive.Functions.SafeString(Settings.GetValue("CSSClassOverride"))
			.EmailFromAddress = Intuitive.Functions.SafeString(Settings.GetValue("EmailFromAddress"))
			.ShowReminderLink = Intuitive.Functions.SafeBoolean(Settings.GetValue("ShowReminderLink"))
			.InsertWarningAfter = Intuitive.Functions.SafeString(Settings.GetValue("InsertWarningAfter"))
			.TemplateOverride = Settings.GetValue("TemplateOverride")
			.ClearBasket = Intuitive.Functions.SafeBoolean(Settings.GetValue("ClearBasket"))
			.RestrictBookingAccess = BookingBase.Params.RestrictBookingAccess
		End With

		MyBookingsLogin.CustomSettings = oCustomSettings

		Dim oXML As XmlDocument = Utility.BigCXML("MyBookingsLogin", 1, 60)

		MyBookingsLogin.CustomerID = 0
		MyBookingsLogin.MyBookingsReference = ""

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("LoginType", MyBookingsLogin.CustomSettings.LoginType)
		oXSLParams.AddParam("MMBURL", MyBookingsLogin.CustomSettings.MMBURL)
		oXSLParams.AddParam("CSSClassOverride", MyBookingsLogin.CustomSettings.CSSClassOverride)
		oXSLParams.AddParam("ShowReminderLink", MyBookingsLogin.CustomSettings.ShowReminderLink)
		oXSLParams.AddParam("InsertWarningAfter", MyBookingsLogin.CustomSettings.InsertWarningAfter)

		If MyBookingsLogin.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & MyBookingsLogin.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, res.MyBookingsLogin, writer, oXSLParams)
		End If

	End Sub

	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("mybookingslogin_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("mybookingslogin_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("mybookingslogin_customsettings") = value
		End Set
	End Property

	Public Shared Function DocumentationSuppressed(tagText As String) As Boolean

		Dim bSuppressed As Boolean = True

		If Not ManageMyBookingDetails.BookingDetails.BookingTags Is Nothing Then
			For Each bookingTag As iVectorConnectInterface.Support.BookingTag In ManageMyBookingDetails.BookingDetails.BookingTags
				If String.Equals(bookingTag.TagType, tagText, StringComparison.CurrentCultureIgnoreCase) Then
					Boolean.TryParse(bookingTag.TagText, bSuppressed)
				End If
			Next
		End If

		Return bSuppressed
	End Function


#Region "Login"

	Public Shared Function Login(ByVal sJSON As String) As String

		Dim oLoginParams As BookingManagement.LoginRequestDetails = Newtonsoft.Json.JsonConvert.DeserializeObject(Of BookingManagement.LoginRequestDetails)(sJSON)

		Dim oCustomerLoginReturn As BookingManagement.CustomerLoginReturn = BookingManagement.CustomerLogin(oLoginParams)

		Dim iCustomerID As Integer = oCustomerLoginReturn.CustomerID
		Dim iBookingID As Integer = oCustomerLoginReturn.BookingID

		If oCustomerLoginReturn.BrandID <> BookingBase.Params.BrandID AndAlso MyBookingsLogin.CustomSettings.RestrictBookingAccess Then
			oCustomerLoginReturn.OK = False
		End If

		If oCustomerLoginReturn.OK Then
			If iCustomerID <> 0 AndAlso oLoginParams.BookingReference <> "" Then
				MyBookingsLogin.CustomerID = oCustomerLoginReturn.CustomerID
				MyBookingsLogin.MyBookingsReference = oLoginParams.BookingReference
			ElseIf iCustomerID <> 0 Then
				MyBookingsLogin.CustomerID = oCustomerLoginReturn.CustomerID
			ElseIf iBookingID <> 0 Then
				MyBookingsLogin.MyBookingsReference = oLoginParams.BookingReference
			End If

			BookingBase.LoggedIn = True
		End If

		'some clients may have upsell on MMB in which case we'll want to clear down the basket
		If MyBookingsLogin.CustomSettings.ClearBasket Then
			BookingBase.ClearBookingBasket()
		End If

		Dim sJSONReturn As String = Newtonsoft.Json.JsonConvert.SerializeObject(oCustomerLoginReturn)
		Return sJSONReturn

	End Function

	Public Shared Function UpdateManageMyBookingDetails(ByVal BookingReference As String) As Boolean

		MyBookingsLogin.ManageMyBookingDetails = BookingManagement.GetBookingDetails(BookingReference)

		Return True

	End Function

#End Region

#Region "email"

	'getbookingsbyemail
	Public Shared Function GetBookingsByEmailAddress(ByVal EmailAddress As String) As Generic.List(Of String)

		'1. set up return 
		Dim aReturn As New Generic.List(Of String)

		'2. make call
		Dim oXML As XmlDocument = Utility.CustomQueryXML("GetBookingsByEmail", EmailAddress)

		'3. sort results if there are any 
		If Not oXML Is Nothing _
		  AndAlso SafeNodeValue(oXML, "CustomQueryResponse/ReturnStatus/Success") = "true" _
		  AndAlso oXML.SelectNodes("//BookingReference").Count > 0 Then

			For Each oRef As XmlNode In oXML.SelectNodes("//BookingReference")
				aReturn.Add(oRef.InnerText)
			Next
		End If

		'4. return 
		Return aReturn

	End Function


	'SendBookingReferencesEmail
	Public Shared Function SendBookingReferencesEmail(ByVal EmailAddress As String) As String

		'1 Get email addresses
		Dim aReturn As Generic.List(Of String) =
		 MyBookingsLogin.GetBookingsByEmailAddress(EmailAddress)

		'2 Send Emails and return bSuccess
		If aReturn.Count > 0 Then
			MyBookingsLogin.SendEmail(EmailAddress, aReturn)
			Return "success"
		Else
			Return "failed"
		End If

	End Function



	'send email
	Public Shared Sub SendEmail(ByVal EmailAddress As String, ByVal aBookingReferences As Generic.List(Of String))

		'1. build email body
		Dim sb As New StringBuilder

		'1ai html head
		sb.AppendLine("<html><head><style type=""text/css"">body {font-family:calibri,helvetica;}</style></head><body>")

		'1a leadin sentence
		sb.AppendFormat("<p>Many thanks for booking to stay at {0}. Your booking reference", BookingBase.Params.Website)

		'1b first paragraph with # of bookings
		If aBookingReferences.Count = 1 Then
			sb.AppendFormatLine(" is {0}.", aBookingReferences(0))
		Else
			sb.Append("s are ")

			For Each sReference As String In aBookingReferences
				If sReference = aBookingReferences.Last Then
					sb.Append(" and " & sReference)
				ElseIf sReference = aBookingReferences(0) Then
					sb.Append(sReference)
				Else
					sb.Append(", " & sReference)
				End If
			Next
		End If

		sb.AppendLine(".</p>")
		sb.AppendLines(1)

		'1c 
		Dim sURL As String = ""
		sURL = BookingBase.Params.Domain + MyBookingsLogin.CustomSettings.MMBURL


		sb.AppendLine("<p>To view a summary of your bookings, login" &
		" via the <a href=""" & sURL & """>MANAGE MY BOOKING</a> section of our " &
		"website using your booking" &
		" reference (shown above) and your email address.</p>")
		sb.AppendLines(1)


		'1d footer
		sb.AppendLine("<p>If you have any questions please contact a member of our sales team on +44 (0) 207 952 2919.</p>")
		sb.AppendLine("<p>" & BookingBase.Params.Website & "</p>")
		sb.AppendLines(1)


		'1e end html
		sb.AppendLine("</body></html>")


		'2. send email
		Dim oEmail As New Email(True)
		With oEmail
			.EmailTo = EmailAddress
			.FromEmail = MyBookingsLogin.CustomSettings.EmailFromAddress
			.From = BookingBase.Params.Website
			.Subject = BookingBase.Params.Website & " – Booking Reference Confirmation"
			.Body = sb.ToString
		End With

		oEmail.SendEmail(True)

	End Sub

#End Region

#Region "Classes"

	Public Class BookingLogin
		Public Property Login As String
		Public Property Password As String
		Public Property BrandID As Integer
	End Class

	Public Class CustomSetting
		Public BrandOverride As Boolean
		Public GetPoints As Boolean
		Public LoginType As String
		Public MMBURL As String
		Public CSSClassOverride As String
		Public EmailFromAddress As String
		Public ShowReminderLink As Boolean
		Public InsertWarningAfter As String
		Public TemplateOverride As String
		Public ClearBasket As Boolean
		Public RestrictBookingAccess As Boolean = True
	End Class

#End Region

End Class
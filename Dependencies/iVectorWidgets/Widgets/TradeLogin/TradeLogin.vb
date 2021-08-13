Imports System.Xml
Imports Newtonsoft.Json
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets


Public Class TradeLogin
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("tradelogin_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("tradelogin_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("tradelogin_customsettings") = value
		End Set
	End Property

#End Region

#Region "Draw"

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		If HttpContext.Current.Request.QueryString.ToString.Contains("logout=true") Then
			BookingTrade.Logout()
		End If

		'OK Lets start by trying to auto-login 
		'no point in trying to render anything/ do anything else if we're just going to login and redirect
		If Not Me.Setting("SupressAutoLogin").ToSafeBoolean AndAlso Me.AutoLogin() Then
			Response.Redirect(Functions.SafeString(Settings.GetValue("RedirectURL")))
		End If

		'save custom settings for use later
		Dim oCustomSetting As New CustomSetting
		With oCustomSetting
			.LoginType = SafeEnum(Of BookingTrade.TradeLoginDetails.eLoginType)(Settings.GetValue("LoginType"))
			.LoginMethod = SafeEnum(Of BookingTrade.eLoginMethod)(Settings.GetValue("LoginMethod"))
			.RedirectURL = Functions.SafeString(Settings.GetValue("RedirectURL"))
			.SupressRememberMe = Functions.SafeBoolean(Settings.GetValue("SupressRememberMe"))
			.SupressAutoLogin = Functions.SafeBoolean(Settings.GetValue("SuppressAutoLogin"))
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
			.UseLoginType = Functions.SafeBoolean(Settings.GetValue("UseLoginType"))
		End With
		TradeLogin.CustomSettings = oCustomSetting

		'get our xsl paramteres
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("LoginType", TradeLogin.CustomSettings.LoginType.ToString)
			.AddParam("LoginMethod", TradeLogin.CustomSettings.LoginMethod.ToString)
			.AddParam("RedirectURL", TradeLogin.CustomSettings.RedirectURL)
			.AddParam("HideHeader", Functions.SafeBoolean(Settings.GetValue("HideHeader")))
			.AddParam("Placeholders", Functions.SafeString(Settings.GetValue("PlaceholderOverrides")))
			.AddParam("DrawRememberMe", Functions.SafeBoolean(Settings.GetValue("DrawRememberMe")).ToString)
			.AddParam("TemplateOverride", TradeLogin.CustomSettings.TemplateOverride)
			.AddParam("UseLoginType", TradeLogin.CustomSettings.UseLoginType.ToString.ToLower)
		End With

		'we need to get our valid login modes
		Dim aLoginTypes As Generic.List(Of String) = Me.Setting("LoginModes").Split("|"c).ToList

		'GEt our tradelogin details from cookie
		Dim oTradeLoginDetails As BookingTrade.TradeLoginDetails = BookingTrade.GetLoginDetailsFromCookie()

		'what happens if our tradelogindetails are for a login mode no longer supported?
		If Not aLoginTypes.Contains(oTradeLoginDetails.LoginType.ToString) Then
			'clear our logindetails cookie to be safe
			BookingTrade.DeleteLoginDetailsCookie()
			'set up a new tradelogindetails
			oTradeLoginDetails = New BookingTrade.TradeLoginDetails
			oTradeLoginDetails.LoginType = SafeEnum(Of BookingTrade.TradeLoginDetails.eLoginType)(aLoginTypes.FirstOrDefault)
		End If


		'serialise our trade details to use in our xsl
		Dim oXML As System.Xml.XmlDocument = Serializer.Serialize(BookingTrade.GetLoginDetailsFromCookie())

		'add in our loginmodes to xml
		Dim oLoginTypes As System.Xml.XmlNode = oXML.CreateElement("LoginTypes")
		For Each sLoginType As String In aLoginTypes
			Dim oLoginType As System.Xml.XmlNode = oXML.CreateElement("LoginType")
			oLoginType.AppendChild(oXML.CreateTextNode(sLoginType))
			oLoginTypes.AppendChild(oLoginType)
		Next
		oXML.DocumentElement.AppendChild(oLoginTypes)

		Dim cmsObject As String = Functions.SafeString(Settings.GetValue("CMSContentObject"))

		If cmsObject <> "" Then
			Dim oBigCXML As XmlDocument = Utility.BigCXML(cmsObject, 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oBigCXML)
		End If

		'transform
		If TradeLogin.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & TradeLogin.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, res.TradeLogin, writer, oXSLParams)
		End If

	End Sub

#End Region

#Region "Login"

	Public Overridable Function Login(ByVal sJSON As String) As String

		'set login params
		Dim oLoginParams As BookingTrade.TradeLoginDetails = JsonConvert.DeserializeObject(Of BookingTrade.TradeLoginDetails)(sJSON)

		'overrides from custom settings
		If TradeLogin.CustomSettings.SupressRememberMe Then oLoginParams.RememberMe = False
		oLoginParams.AutoLogin = oLoginParams.RememberMe AndAlso Not TradeLogin.CustomSettings.SupressAutoLogin

		'attempt trade login
		Dim oTradeLoginReturn As BookingTrade.TradeLoginReturn = BookingBase.Trade.Login(oLoginParams)

		'serialize to json and return
		Dim sJSONReturn As String = JsonConvert.SerializeObject(oTradeLoginReturn)
		Return sJSONReturn

	End Function


	Protected Function AutoLogin() As Boolean

		'1. Get login details from cookie
		Dim oTradeLoginDetails As BookingTrade.TradeLoginDetails = BookingTrade.GetLoginDetailsFromCookie()

		'2. If autologin enabled try loggin in
		If oTradeLoginDetails.AutoLogin Then
			Return BookingBase.Trade.Login(oTradeLoginDetails).OK
		End If

		Return False
	End Function

#End Region

#Region "Support Classes"

	Public Class CustomSetting
		Public LoginType As BookingTrade.TradeLoginDetails.eLoginType
		Public LoginMethod As BookingTrade.eLoginMethod
		Public RedirectURL As String
		Public SupressRememberMe As Boolean
		Public SupressAutoLogin As Boolean
		Public TemplateOverride As String
		Public UseLoginType As Boolean = False
	End Class

#End Region

End Class

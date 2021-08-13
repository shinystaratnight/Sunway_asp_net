Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel


Public Class CookieCompliance
	Inherits WidgetBase

	Public Shared Property CMSObject As String
		Get
			Return CType(HttpRuntime.Cache("__CookieCompliance_CMSObject"), String)
		End Get
		Set(value As String)
			HttpRuntime.Cache("__CookieCompliance_CMSObject") = value
		End Set
	End Property


	Public Shared Property BigCContent As XmlDocument
		Get

			If HttpRuntime.Cache("__CookieCompliance_BigC") IsNot Nothing Then
				Return CType(HttpRuntime.Cache("__CookieCompliance_BigC"), XmlDocument)
			Else
				Dim oXML As XmlDocument = Utility.BigCXML(CookieCompliance.CMSObject, 1, 600)
				HttpRuntime.Cache("__CookieCompliance_BigC") = oXML
				Return oXML
			End If

		End Get
		Set(value As XmlDocument)
			HttpRuntime.Cache("__CookieCompliance_BigC") = value
		End Set
	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Try

			'Check if we need to do anything, if we don't exit
			If Not Me.CheckIfRequired() Then Exit Sub

			'2. Create params
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams

			With oXSLParams
                .AddParam("RequiresAccepting", GetSetting(eSetting.RequiresAccepting))
                .AddParam("HideCookieMessage", GetSetting(eSetting.HideCookieMessage))
			End With

			'3. Transform
			If Intuitive.ToSafeString(GetSetting(eSetting.TemplateOverride)) <> "" Then
				Me.XSLPathTransform(CookieCompliance.BigCContent, HttpContext.Current.Server.MapPath("~" & Intuitive.ToSafeString(GetSetting(eSetting.TemplateOverride))), writer, oXSLParams)
			Else
				Me.XSLTransform(CookieCompliance.BigCContent, res.CookieCompliance, writer, oXSLParams)
			End If

		Catch ex As Exception
			FileFunctions.AddLogEntry("CookieCompliance", "DrawException", ex.ToString)
		End Try

	End Sub

	Public Function CheckIfRequired() As Boolean

		'If Cookie is set bomb out
		If GetCookieLawCookie() Then Return False

		'If cookie not set, need to set up widget
		Me.SetProperties()

		'If CMS enable is required, chceck BigC Content if not return true
		Return CheckIfCMSEnabled()

	End Function

	Public Sub SetProperties()

		CookieCompliance.CMSObject = GetSetting(eSetting.CMSObject)

	End Sub

	Public Function GetCookieLawCookie() As Boolean

		Dim bAccepted As Boolean = Intuitive.ToSafeBoolean(Intuitive.CookieFunctions.Cookies.GetValue("__Cookie_Law_Acceptance"))

		Return bAccepted

	End Function

	Public Function CheckIfCMSEnabled() As Boolean

		'If Functionality is controlled by CMS switch, check bigC content for switch, If not assume always on
		If Intuitive.ToSafeBoolean(GetSetting(eSetting.CMSEnabled)) Then

			Dim sCMSEnabled As String = Intuitive.XMLFunctions.SafeNodeValue(CookieCompliance.BigCContent, "//Enabled")

			Return Intuitive.ToSafeBoolean(sCMSEnabled)

		End If

		Return True

	End Function

	Public Enum eSetting

		<Title("CMSObject Name")>
		 <DeveloperOnly(True)>
		 <Description("The name of the CMS object in iVector that contains the cookie message and more info url")>
		 CMSObject

		<Title("Template Override Path")>
		  <DeveloperOnly(True)>
		  <Description("The path in the project to the override template file")>
		  TemplateOverride

		<Title("Requires Accepting Boolean")>
		<Description("If this is true, we will not attempt to save the cookie by default until the user clicks accept")>
		RequiresAccepting


        <Title("CMS Enabled Boolean")>
         <DeveloperOnly(True)>
        <Description("If this is true, will check BigC Content for an enabled flag that turns on/off widget")>
        CMSEnabled

        <Title("Hide the Cookie Acceptance Message")>
        <Description("If this is true, will check the '_Cookie_Law_Acceptance' cookie, if exists then Hide the Cookie Acceptance Message")>
        HideCookieMessage

	End Enum



End Class

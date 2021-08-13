Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel

Public Class TrackingAffiliate
	Inherits WidgetBase

	Public Property AffiliateXML As XmlDocument
		Get
			If Not HttpContext.Current.Cache("trackingaffiliate_affiliatexml") Is Nothing Then
				Return CType(HttpContext.Current.Cache("trackingaffiliate_affiliatexml"), XmlDocument)
			Else
				Return Nothing
			End If
		End Get
		Set(value As XmlDocument)
			HttpContext.Current.Cache("trackingaffiliate_affiliatexml") = value
		End Set
	End Property


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'set affiliate xml if not set
		If Me.AffiliateXML Is Nothing Then
			Me.AffiliateXML = Utility.BigCXML(Me.Setting("ObjectType"), 1, 60)
		End If


		'deserialize to generic list
		Dim oAffiliates As New Generic.List(Of TrackingAffiliate)
		Try
			oAffiliates = Utility.XMLToGenericList(Of TrackingAffiliate)(Me.AffiliateXML)
		Catch ex As Exception
			'sometimes no data in big c for website - we dont want to break site
		End Try


		'check if URL contains a referrer and set affiliate id on basket and cookie
		Dim sURL As String = Me.Page.Request.RawUrl.ToLower

		For Each oAffiliate As TrackingAffiliate In oAffiliates
			If oAffiliate.Type = "Affiliate" AndAlso sURL.Contains(oAffiliate.QueryStringIdentifier.ToLower) Then
				BookingBase.Basket.TrackingAffiliateID = oAffiliate.TrackingAffiliateID
				BookingBase.Basket.TrackingAffiliate = oAffiliate.TrackingAffiliateName
				Me.SetAffiliateCookie(oAffiliate.TrackingAffiliateID, oAffiliate.ValidForDays)
			End If
		Next


		'if we have affiliates and no tracking affiliate set, try set from cookie
		If oAffiliates.Where(Function(o) o.Type = "Affiliate").Count > 0 AndAlso BookingBase.Basket.TrackingAffiliateID = 0 Then
			BookingBase.Basket.TrackingAffiliateID = Me.GetAffiliateCookie
		End If

		'legacy code dictates you need to pass in a setting, confirmation=true
		'this is not much use if we want the tracking widget as a common widget
		'instead check this and also check if a confirmation page url is set, if it is then match against the request url
		Dim confirmationSetting As String = Me.GetSetting(eSetting.Confirmation).ToString.ToLower
		Dim confirmationURLSetting As String = Me.GetSetting(eSetting.ConfirmationUrl).ToString.ToLower

		Dim onConfirmationPage As Boolean = confirmationSetting = "true" _
		  OrElse confirmationURLSetting = HttpContext.Current.Request.Url.AbsolutePath.ToLower

		'set params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Confirmation", onConfirmationPage.ToString.ToLower)
		End With


		'transform
		Me.XSLTransform(Me.AffiliateXML, res.TrackingAffiliate, writer, oXSLParams)

	End Sub


#Region "Set and Get Affiliate Cookie"

	'set
	Public Sub SetAffiliateCookie(ByVal TrackingAffiliateID As Integer, ByVal ValidForDays As Integer)
		Dim oCookie As HttpCookie = HttpContext.Current.Request.Cookies("TrackingAffiliateID")
		If oCookie Is Nothing Then
			oCookie = New HttpCookie("TrackingAffiliateID")
		End If

		oCookie.Value = TrackingAffiliateID.ToString
		oCookie.Expires = Now.AddDays(ValidForDays)
		HttpContext.Current.Response.Cookies.Add(oCookie)
	End Sub

	'get
	Public Function GetAffiliateCookie() As Integer
		Try
			Return Functions.SafeInt(CookieFunctions.Cookies.GetValue("TrackingAffiliateID"))
		Catch ex As Exception
			Return 0
		End Try
	End Function

#End Region


	Public Class TrackingAffiliate
		Public TrackingAffiliateID As Integer
		Public TrackingAffiliateName As String
		Public Type As String
		Public QueryStringIdentifier As String
		Public ValidForDays As Integer
		Public Script As String
		Public ConfirmationScript As String
	End Class

	Public Enum eSetting
		<DeveloperOnly(True)>
		<Title("Confirmation flag")>
		<Description("Set to true when widget on confirmation page, if it is a common element widget then set the ConfirmationUrl setting instead")>
		Confirmation

		<DeveloperOnly(True)>
		<Title("Confirmation Url")>
		<Description("For common element tracking, if the setting matches the Url of the current request then it will output confirmation script from the XML")>
		ConfirmationUrl
	End Enum

End Class

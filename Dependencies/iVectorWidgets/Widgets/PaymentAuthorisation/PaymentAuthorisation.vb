Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports Intuitive.Functions

Public Class PaymentAuthorisation
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("paymentauthorisation_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("paymentauthorisation_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("paymentauthorisation_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.AuthFailURL = SafeString(Settings.GetValue("AuthFailURL"))
		End With

		PaymentAuthorisation.CustomSettings = oCustomSettings

		'get our various redirect urls
		Dim sAuthFailURL As String = IIf(PaymentAuthHandler.CustomSettings.AuthFailURL <> "", PaymentAuthHandler.CustomSettings.AuthFailURL, "/payment?warn=authfailed")

		If CompleteBooking.Get3DSecureRedirectDetails.HTMLData Is Nothing Then
			Response.Redirect(sAuthFailURL)
		End If

		'set up parameters
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("HTML", CompleteBooking.Get3DSecureRedirectDetails.HTMLData)

		'xml
		Dim oXML As XmlDocument = New XmlDocument

		'render the html
		Me.XSLTransform(oXML, XSL.SetupTemplate(res.PaymentAuthorisation, True, True), writer, oXSLParams)

	End Sub

	Public Class CustomSetting
		Public AuthFailURL As String
	End Class

End Class

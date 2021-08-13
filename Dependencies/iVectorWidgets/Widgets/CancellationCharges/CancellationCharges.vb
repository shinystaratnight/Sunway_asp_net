Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class CancellationCharges
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Get xml from content path
		Dim oXML As New XmlDocument
		oXML = BookingBase.Basket.XML

		'2. Create params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("DateToday", Intuitive.DateFunctions.SQLDateFormat(Today.Date))
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("IntroductionOverride", Intuitive.Functions.SafeString(Settings.GetValue("IntroductionOverride")))
		End With

		'3. Transform
		If Intuitive.ToSafeString(Settings.GetValue("TemplateOverride")) <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & Intuitive.ToSafeString(Settings.GetValue("TemplateOverride"))), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, res.CancellationCharges, writer, oXSLParams)
		End If
	End Sub
End Class

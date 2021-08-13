Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.ComponentModel

Public Class FlightExtras
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'no flights return
		If BookingBase.SearchBasket.BasketFlights.Count = 0 Then Return

		'no extras
		If (BookingBase.SearchBasket.BasketFlights(0).BasketFlightExtras.Count + BookingBase.SearchBasket.BasketFlights(0).ReturnMultiCarrierFlightExtras.Count) = 0 Then Return

		'set up basket xml 
		Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML

		'set up text overrides xml
		Dim oTextOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("TextOverrides"))

		'merge xml
		Dim oXML As XmlDocument = MergeXMLDocuments(oBasketXML, oTextOverridesXML)

		'get warning messages xml
		Dim oWarningMessagesXML As XmlDocument = Utility.BigCXML("WarningMessage", 1, 60)
		Dim sBaggageQuantity As String = SafeString(XMLFunctions.SafeNodeValue(oWarningMessagesXML, "WarningMessages/WarningMessage[Type='flightbaggagequantity']/Message"))
		Dim sExtraQuantity As String = SafeString(XMLFunctions.SafeNodeValue(oWarningMessagesXML, "WarningMessages/WarningMessage[Type='flightextraquantity']/Message"))

		'set our messages
		Dim sBaggageQuantityError As String = IIf(sBaggageQuantity <> "", sBaggageQuantity, "Unable to book more bags than there are passengers")
		Dim sExtraQuantityError As String = IIf(sExtraQuantity <> "", sExtraQuantity, "Unable to book more extras than there are passengers")

		'set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Title", Functions.SafeString(GetSetting(eSetting.Title)))
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("PriceFormat", Functions.SafeString(GetSetting(eSetting.PriceFormat)))
			.AddParam("BaggageQuantityError", Intuitive.Web.Translation.GetCustomTranslation("Flight Extras", sBaggageQuantityError))
			.AddParam("ExtraQuantityError", Intuitive.Web.Translation.GetCustomTranslation("Flight Extras", sExtraQuantityError))
		End With


		'transform
		Dim sTemplateOverride As String = Functions.SafeString(GetSetting(eSetting.TemplateOverride))
		If sTemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & sTemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.FlightExtras, True, False), writer, oXSLParams)
		End If

	End Sub

	Public Function UpdateExtras(ByVal sJSON As String, ByVal bReturn As Boolean) As String

		'deserialize
		Dim oExtraTokens As New FlightResultHandler.ExtraOptions
		oExtraTokens = Newtonsoft.Json.JsonConvert.DeserializeObject(Of FlightResultHandler.ExtraOptions)(sJSON)

		'update baggage on basket
		If BookingBase.SearchBasket.BasketFlights.Count > 0 Then
			If bReturn Then
				BookingBase.SearchBasket.BasketFlights(0).UpdateReturnFlightExtrasQuantity(oExtraTokens)
			Else
				BookingBase.SearchBasket.BasketFlights(0).UpdateFlightExtrasQuantity(oExtraTokens)
			End If
		End If

		'reset prebook
        BookingBase.SearchBasket.PreBooked = False
        BookingBase.Basket.PreBooked = False

		Return "Success"

	End Function

	Public Enum eSetting

		<Title("Template Override")>
		<Description("XSL Template Override")>
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Title")>
		<Description("Title to display in the header box")>
		Title

		<Title("Price Format")>
		<Description("Format the prices show in the widget")>
		<DeveloperOnly(True)>
		PriceFormat

	End Enum

End Class

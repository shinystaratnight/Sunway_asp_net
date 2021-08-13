Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.ComponentModel

Public Class Baggage
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'no flights return
		If BookingBase.SearchBasket.BasketFlights.Count = 0 Then Return

        'has included baggage
        Dim bHasIncludedBaggage As Boolean = BookingBase.SearchBasket.BasketFlights(0).Flight.IncludedBaggageAllowance > 0

		'no extras
		If (BookingBase.SearchBasket.BasketFlights(0).BasketFlightExtras.Count +
			BookingBase.SearchBasket.BasketFlights(0).ReturnMultiCarrierFlightExtras.Count) = 0 AndAlso
			Not bHasIncludedBaggage Then Return

		'set up basket xml 
		Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML

		'no default baggage extra
		If BookingBase.SearchBasket.BasketFlights(0).BasketFlightExtras.FirstOrDefault(Function(oExtra) oExtra.DefaultBaggage = "true") Is Nothing AndAlso
			(BookingBase.SearchBasket.BasketFlights(0).ReturnMultiCarrierFlightExtras Is Nothing OrElse
			BookingBase.SearchBasket.BasketFlights(0).ReturnMultiCarrierFlightExtras.FirstOrDefault(Function(oExtra) oExtra.DefaultBaggage = "true") Is Nothing) AndAlso
			Not bHasIncludedBaggage Then Return


		'set up text overrides xml
		Dim oTextOverridesXML As XmlDocument = Utility.GetTextOverridesXML(GetSetting(eSetting.TextOverrides))

		'merge xml
		Dim oXML As XmlDocument = MergeXMLDocuments(oBasketXML, oTextOverridesXML)

		'set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Title", GetSetting(eSetting.Title))
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("PriceFormat", GetSetting(eSetting.PriceFormat))
		End With


		'transform
		Dim overrideTemplate As String = GetSetting(eSetting.TemplateOverride)

		If overrideTemplate <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & overrideTemplate), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.Baggage, True, False), writer, oXSLParams)
		End If

	End Sub



	Public Function UpdateBaggage(ByVal sJSON As String) As String

		If Not String.IsNullOrEmpty(BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken) Then
			If BookingBase.SearchBasket.BasketFlights(0).Flight.BookingToken <> BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken Then
				BookingBase.SearchBasket.BasketFlights(0).Flight.BookingToken = BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken
			End If
		End If

		'deserialize
		Dim oBaggageTokens As New FlightResultHandler.BaggageOptions
		oBaggageTokens = Newtonsoft.Json.JsonConvert.DeserializeObject(Of FlightResultHandler.BaggageOptions)(sJSON)

		'update baggage on basket
		If BookingBase.SearchBasket.BasketFlights.Count > 0 Then
			BookingBase.SearchBasket.BasketFlights(0).UpdateBaggageQuantity(oBaggageTokens)
		End If

		'reset prebook
		BookingBase.SearchBasket.PreBooked = False

		Return "Success"


	End Function

	Public Function UpdateFlightExtras(ByVal sJSON As String) As String

	    If Not String.IsNullOrEmpty(BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken) Then
	        If BookingBase.SearchBasket.BasketFlights(0).Flight.BookingToken <> BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken Then
	            BookingBase.SearchBasket.BasketFlights(0).Flight.BookingToken = BookingBase.SearchBasket.BasketFlights(0).Flight.hlpSearchBookingToken
	        End If
	    End If

	    'deserialize
	    Dim oBaggageTokens As New FlightResultHandler.ExtraOptions
	    oBaggageTokens = Newtonsoft.Json.JsonConvert.DeserializeObject(Of FlightResultHandler.ExtraOptions)(sJSON)

	    'update baggage on basket
	    If BookingBase.SearchBasket.BasketFlights.Count > 0 Then
	        BookingBase.SearchBasket.BasketFlights(0).UpdateFlightExtrasQuantity(oBaggageTokens)
	    End If

	    'reset prebook
	    BookingBase.SearchBasket.PreBooked = False

	    Return "Success"
	End Function


#Region "Settings"

	Public Enum eSetting
		<Title("Baggage XSL Template Override")>
		<Description("This setting specifies a template to override the iVectorWidgets Baggage.XSL (if needed)")> _
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Title")>
		<Description("Title for the baggage section")>
		Title

		<Title("Price format")>
		<Description("XSL number format eg ###,##0")>
		<DeveloperOnly(True)>
		PriceFormat

		<Title("Text overrides for labels - these should be split out into individual labels, this is confusing")>
		<Description("Configurable labels eg Description|Baggage Description#Quantity|Number Of Items#Price|Price Per Item")>
		TextOverrides
	End Enum

#End Region


End Class

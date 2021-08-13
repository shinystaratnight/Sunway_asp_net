Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel

Public Class HotelUpsell
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("hotelUpsell_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelUpsell_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("hotelUpsell_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'sort out the Custom settings.
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.Format = Functions.SafeEnum(Of Format)(GetSetting(eSetting.Format))
			.Title = Functions.SafeString(GetSetting(eSetting.Title))
			.SubTitle = Functions.SafeString(GetSetting(eSetting.SubTitle))
			.TemplateOverride = Functions.SafeString(GetSetting(eSetting.TemplateOverride))
			.PerPersonPrice = Functions.SafeBoolean(GetSetting(eSetting.PerPersonPrice))
		End With
		HotelUpsell.CustomSettings = oCustomSettings

		'get page xml
		Dim oXML As XmlDocument = New XmlDocument

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly And Me.PageDefinition.PageName = "BookingSummary" Then

			oXML = SearchForHotelToUpsell()

		End If

		'transform xsl
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("Format", HotelUpsell.CustomSettings.Format)
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("PriceFormat", "###,##0.00")
			.AddParam("HotelsToDisplay", "3")
			.AddParam("JustResults", False)
			.AddParam("FlightPrice", BookingBase.SearchBasket.BasketFlights.Sum(Function(oFlight) oFlight.Flight.Price))
			.AddParam("Title", oCustomSettings.Title)
			.AddParam("SubTitle", oCustomSettings.SubTitle)
			.AddParam("PerPersonPrice", HotelUpsell.CustomSettings.PerPersonPrice)
		End With

		'transform
        If HotelUpsell.CustomSettings.TemplateOverride <> "" Then
            Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & HotelUpsell.CustomSettings.TemplateOverride), writer, oXSLParams)
        Else
            Me.XSLTransform(oXML, XSL.SetupTemplate(res.HotelUpsell, True, False), writer, oXSLParams)
        End If

	End Sub

	Public Function UpdateResults(ByVal iHotelsToDisplay As Integer) As String

		'get page xml
		Dim iCurrentPage As Integer = BookingBase.SearchDetails.PropertyResults.CurrentPage
		Dim oXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(iCurrentPage)

		'transform xsl
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("Format", HotelUpsell.CustomSettings.Format)
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("PriceFormat", "###,##0.00")
			.AddParam("HotelsToDisplay", iHotelsToDisplay)
			.AddParam("JustResults", True)
			.AddParam("FlightPrice", BookingBase.SearchBasket.BasketFlights.Sum(Function(oFlight) oFlight.Flight.Price))
			.AddParam("PerPersonPrice", HotelUpsell.CustomSettings.PerPersonPrice)
		End With

		Dim sHTML As String = ""

        If HotelUpsell.CustomSettings.TemplateOverride <> "" Then
            sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelUpsell.CustomSettings.TemplateOverride), oXSLParams)
        Else
            sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelUpsell, True, False), oXSLParams)
        End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

	Public Function ChangeToFlightAndHotel(ByVal iDays As Integer) As String

		'clear hotel from basket
		BookingBase.SearchBasket.BasketProperties.Clear()

		'get current search details and set to hotel only and new departure date
		Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails

		Dim iRegionID As Integer = CheckForAirportForRegion()

		'The above function will return a -1 if no region is found (for example a non-brand location)
		If iRegionID = -1 Then
			Return "Error|Sorry there are no hotels available for your selected flight date. Please try an alternative date option."
		Else
			oSearchDetails.ArrivingAtID = iRegionID
		End If

		oSearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly

		'search
		Dim oSearchReturn As New BookingSearch.SearchReturn
		oSearchReturn = oSearchDetails.Search()
		oSearchReturn.PropertyCount = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count

		'if no hotel results return
		If oSearchReturn.PropertyCount = 0 Then
			Return "Error|Sorry there are no hotels available for your selected flight date. Please try an alternative date option."
		End If

		'set search back to flight and hotel
		oSearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel

		'serialize to json and return
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSearchReturn)


	End Function

	Public Function SearchForHotelToUpsell() As XmlDocument

		'get current search details and set to hotel only
		Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails
		oSearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly

		'Do the searching
		Dim oSearchReturn As New BookingSearch.SearchReturn
		oSearchReturn = oSearchDetails.Search()
		oSearchReturn.PropertyCount = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count

		'set search back to flight only
		oSearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly

		If oSearchReturn.PropertyCount = 0 Then
			Return New XmlDocument
		Else
			Dim iCurrentPage As Integer = BookingBase.SearchDetails.PropertyResults.CurrentPage
			Return BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(iCurrentPage)
		End If
	End Function

	Public Function CheckForAirportForRegion(Optional ByVal iAirportID As Integer = -1) As Integer

		'If we have not passed in an airportID get it off the search
		If iAirportID = -1 Then
			Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails
			iAirportID = oSearchDetails.ArrivingAtID - 1000000
		End If

		'Get some resorts from the airport
		Dim iResorts As Generic.List(Of Integer) = Lookups.GetAirportGeographyLevel3IDs(iAirportID)

		'use a resort to find the region
		Dim oLocation As Lookups.Location = Lookups.GetLocationFromResort(iResorts.FirstOrDefault)
		If oLocation IsNot Nothing Then
			Return oLocation.GeographyLevel2ID
		Else
			Return -1
		End If

	End Function

	Public Function AddRoomOptionToBasket(ByVal sJSON As String) As String

		Try

			'deserialize
			Dim oAddToBasket As New AddToBasket
			oAddToBasket = Newtonsoft.Json.JsonConvert.DeserializeObject(Of AddToBasket)(sJSON)

			'get hash tokens
			Dim HashTokens As New Generic.List(Of String)
			For Each sIndex As String In oAddToBasket.PropertyRoomIndexes

				Dim sToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(sIndex)
				HashTokens.Add(sToken)

			Next

			'add room(s) to basket
			BookingProperty.AddRoomsToBasket(HashTokens)

		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

		Return "true"

	End Function


	Public Class AddToBasket
		Public FlightOptionHashToken As String
		Public PropertyRoomIndexes As New Generic.List(Of String)
	End Class


#Region "Settings"

	Public Class CustomSetting
		Public Format As Format
		Public Title As String
		Public SubTitle As String
		Public TemplateOverride As String
		Public PerPersonPrice As Boolean
	End Class


	Public Enum Format
		ResultsPage
		ExtrasPage
	End Enum


	Public Enum eSetting
		<Title("Title")>
		<Description("Main title text for hotel upsell section")>
		Title

		<Title("Sub Title")>
		<Description("Sub title text for hotel upsell section")>
		SubTitle

		<Title("Per Person Price Boolean")>
		<Description("Boolean to specify whether or not to show prices per person")>
		PerPersonPrice

		<Title("XSL Template Override")>
		<Description("File path to an overide XSL (name it breadcrumbs.xsl)")>
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Page Format")>
		<Description("ResultsPage or ExtrasPage")>
		<DeveloperOnly(True)>
		Format
	End Enum

#End Region

End Class
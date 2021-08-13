Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class FlightResults
	Inherits WidgetBase



#Region "Draw"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("flightresults_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("flightresults_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("flightresults_customsettings") = value
		End Set

	End Property

	Public Shared Property oPagingScript As PagingScriptJSON

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oCustomSettings As CustomSetting = Me.GetCustomSetting()

		'set display mode
		oCustomSettings.DisplayMode = Functions.IIf(oCustomSettings.DisplayMode <> "", oCustomSettings.DisplayMode, "ListView")

		'save settings
		FlightResults.CustomSettings = oCustomSettings


		'0. reset current page if set
		If oCustomSettings.ResetCurrentPage Then
			BookingBase.SearchDetails.FlightResults.CurrentPage = 1
		End If

		'1. get xml
		Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetPageXML(BookingBase.SearchDetails.FlightResults.CurrentPage)

		'How we get our points values (currently only used for monster)
		Dim oPointsXML As New XmlDocument
		If oCustomSettings.UsePoints = True Then
			'We probably want to inlcude these in our Global on session start to get them in the cache early.
			oPointsXML = Utility.BigCXML("BrandTiers", 1, 600)
			oPointsXML = XMLFunctions.MergeXMLDocuments("Points", oPointsXML, Utility.BigCXML("Settings", 1, 600))
		End If

		'1.1 get extra xml and merge
		Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
		If FlightResults.CustomSettings.AddBasketXML Then
			Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML, oPointsXML, oBasketXML)
		Else
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML, oPointsXML)
		End If

		'1.2 Carrierlogo overbranding
		If oCustomSettings.CarrierLogoOverbranding Then
			Dim oCarrierLogosXML As XmlDocument = Utility.BigCXML("CarrierLogos", 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oCarrierLogosXML)
		End If

		'2. set up params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = FlightResults.GetFlightResultsXSLParams(True)

		'Paging Script Override
		Dim oPagingScript As PagingScriptJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(Of PagingScriptJSON)(CustomSettings.PagingScript)
		FlightResults.oPagingScript = oPagingScript


		If FlightResults.CustomSettings.AddHeaderPaging Then
			GeneratePaging(writer, "divFlightResultsHeader")
		End If

		'3. transform
		If FlightResults.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & FlightResults.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.FlightResults, True, False), writer, oXSLParams)
		End If

		'4. Add paging
		If FlightResults.CustomSettings.AddPaging Then
			GeneratePaging(writer, "divFlightResultsFooter")
		End If

	End Sub

	Private Sub GeneratePaging(writer As HtmlTextWriter, id As String)
		writer.AddAttribute(UI.HtmlTextWriterAttribute.Id, id) 'possibly could add a content box class to this
		writer.RenderBeginTag(UI.HtmlTextWriterTag.Div)

		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = Math.Ceiling(BookingBase.SearchDetails.FlightResults.TotalFlights / BookingBase.Params.FlightResultsPerPage).ToSafeInt
			If .TotalPages = 0 Then .TotalPages = 1
			.TotalLinksToDisplay = CustomSettings.PagingTotalLinks
			.CurrentPage = BookingBase.SearchDetails.FlightResults.CurrentPage
			.ScriptPrevious = "FlightResults.PreviousPage();"
			If Not FlightResults.oPagingScript Is Nothing AndAlso Not FlightResults.oPagingScript.ScriptPrevious = "" Then .ScriptPrevious = FlightResults.oPagingScript.ScriptPrevious
			.ScriptNext = "FlightResults.NextPage();"
			If Not FlightResults.oPagingScript Is Nothing AndAlso Not FlightResults.oPagingScript.ScriptNext = "" Then .ScriptNext = FlightResults.oPagingScript.ScriptNext
			.ScriptPage = "FlightResults.SelectPage({0})"
			If Not FlightResults.oPagingScript Is Nothing AndAlso Not FlightResults.oPagingScript.ScriptPage = "" Then .ScriptPage = FlightResults.oPagingScript.ScriptPage
			.ShowTotalPages = True
		End With
		oPaging.RenderControl(writer)

		writer.AddAttribute(UI.HtmlTextWriterAttribute.Class, "clear")
		writer.RenderBeginTag(UI.HtmlTextWriterTag.Div)
		writer.RenderEndTag()

		writer.RenderEndTag()

	End Sub


#End Region

#Region "Custom Settings"

	''' <summary>
	''' Gets all the custom settings from the client
	''' </summary>
	''' <returns>Custom Setting with client's settings</returns>
	''' <remarks></remarks>
	Public Function GetCustomSetting() As CustomSetting

		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.DisplayMode = Intuitive.Functions.SafeString(Settings.GetValue("DisplayMode"))
			.SelectButtonValue = Settings.GetValue("SelectButtonValue")
			.PerPersonPrices = Functions.SafeBoolean(Settings.GetValue("PerPersonPrices"))
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
			.PriceFormat = Intuitive.Functions.IIf(Settings.GetValue("PriceFormat") = "", "###,##0", Settings.GetValue("PriceFormat"))
			.UsePoints = Functions.SafeBoolean(Settings.GetValue("UsePoints"))
			.AddPaging = Functions.SafeBoolean(Settings.GetValue("AddPaging"))
			.AddHeaderPaging = Functions.SafeBoolean(Settings.GetValue("AddHeaderPaging"))
			.AddBasketXML = Functions.SafeBoolean(Settings.GetValue("AddBasketXML"))
			.AddCheapestFlightToBasket = Functions.SafeBoolean(Settings.GetValue("AddCheapestFlightToBasket"))
			.RedirectURL = Functions.SafeString(Settings.GetValue("RedirectURL"))
			.FlightOnlyRedirectURL = Functions.SafeString(Settings.GetValue("FlightOnlyRedirectURL"))
			.IgnorePriceChange = Functions.SafeBoolean(Settings.GetValue("IgnorePriceChange"))
			.CarrierLogoOverbranding = Functions.SafeBoolean(Settings.GetValue("CarrierLogoOverbranding"))
			.PagingScript = Functions.SafeString(Settings.GetValue("PagingScript"))
			.ResetCurrentPage = Functions.SafeBoolean(Settings.GetValue("ResetCurrentPage"))
			.BookingAdjustmentTypeCSV = Functions.SafeString(Settings.GetValue("BookingAdjustmentTypeCSV"))

			If Settings.GetValue("PagingTotalLinks") <> "" Then
				.PagingTotalLinks = Functions.SafeInt(Settings.GetValue("PagingTotalLinks"))
			End If

		End With

		Return oCustomSettings

	End Function

#End Region

#Region "FlightResults XSL Params"

	Public Shared Function GetFlightResultsXSLParams(ByVal bIncludeContainer As Boolean) As Intuitive.WebControls.XSL.XSLParams

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("TotalPassengers", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
			.AddParam("SelectButtonValue", FlightResults.CustomSettings.SelectButtonValue)
			.AddParam("DisplayMode", FlightResults.CustomSettings.DisplayMode)
			.AddParam("PerPersonPrices", FlightResults.CustomSettings.PerPersonPrices)
			.AddParam("PriceFormat", FlightResults.CustomSettings.PriceFormat)
			.AddParam("BrandID", BookingBase.Params.BrandID)
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("HasOverBrandedLogos", FlightResults.CustomSettings.CarrierLogoOverbranding)
			.AddParam("OneWay", Intuitive.Functions.SafeString(BookingBase.SearchDetails.OneWay).ToLower)
			.AddParam("MarkupAmount", BookingBase.SearchDetails.FlightResults.MarkupAmount)
			.AddParam("MarkupPercentage", BookingBase.SearchDetails.FlightResults.MarkupPercentage)
			.AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(FlightResults.CustomSettings.BookingAdjustmentTypeCSV))
			.AddParam("ShowFlightClass", BookingBase.Params.ShowFlightClassInResults.ToString)
			.AddParam("IncludeContainer", bIncludeContainer)
			.AddParam("IsCommissionable", BookingBase.Trade.Commissionable)
		    .AddParam("IsFlightItinerarySearch", BookingBase.SearchDetails.FlightSearchMode = BookingSearch.FlightSearchModes.FlightItinerary)
		End With

		Return oXSLParams

	End Function

#End Region

#Region "Update Results"

	Public Shared Function UpdateResults(ByVal Page As Integer, Optional ByVal IncludeBaggagePrice As Boolean = False) As String

		Dim iCurrentPage As Integer = Functions.IIf(Page <> 0, Page, BookingBase.SearchDetails.FlightResults.CurrentPage)

		Dim oUpdateResultsResponse As New UpdateResultsResponse

		Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetPageXML(iCurrentPage)

		'Set new page
		BookingBase.SearchDetails.FlightResults.CurrentPage = iCurrentPage

		'How we get our points values (currently only used for monster)
		Dim oPointsXML As New XmlDocument
		If FlightResults.CustomSettings.UsePoints = True Then
			oPointsXML = Utility.BigCXML("BrandTiers", 1, 600)
			oPointsXML = XMLFunctions.MergeXMLDocuments("Points", oPointsXML, Utility.BigCXML("Settings", 1, 600))
		End If

		Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
		If FlightResults.CustomSettings.AddBasketXML Then
			Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML, oPointsXML, oBasketXML)
		Else
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML, oPointsXML)
		End If

		If FlightResults.CustomSettings.CarrierLogoOverbranding Then
			Dim oCarrierLogosXML As XmlDocument = Utility.BigCXML("CarrierLogos", 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oCarrierLogosXML)
		End If

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = FlightResults.GetFlightResultsXSLParams(False)
		oXSLParams.AddParam("IncludeBaggagePrice", IncludeBaggagePrice)

		Dim sHTML As String
		If FlightResults.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & FlightResults.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.FlightResults, True, True), oXSLParams)
		End If
		oUpdateResultsResponse.FlightResultsHTML = Intuitive.Web.Translation.TranslateHTML(sHTML)

		'add paging
		'paging
		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = Math.Ceiling(BookingBase.SearchDetails.FlightResults.TotalFlights / BookingBase.Params.FlightResultsPerPage).ToSafeInt
			If .TotalPages = 0 Then .TotalPages = 1
			.TotalLinksToDisplay = CustomSettings.PagingTotalLinks
			.CurrentPage = BookingBase.SearchDetails.FlightResults.CurrentPage
			.ScriptPrevious = "FlightResults.PreviousPage();"
			If FlightResults.oPagingScript IsNot Nothing AndAlso Not FlightResults.oPagingScript.ScriptPrevious = "" Then .ScriptPrevious = FlightResults.oPagingScript.ScriptPrevious
			.ScriptNext = "FlightResults.NextPage();"
			If FlightResults.oPagingScript IsNot Nothing AndAlso Not FlightResults.oPagingScript.ScriptNext = "" Then .ScriptNext = FlightResults.oPagingScript.ScriptNext
			.ScriptPage = "FlightResults.SelectPage({0})"
			If FlightResults.oPagingScript IsNot Nothing AndAlso Not FlightResults.oPagingScript.ScriptPage = "" Then .ScriptPage = FlightResults.oPagingScript.ScriptPage
			.ShowTotalPages = True
		End With


		If FlightResults.CustomSettings.AddPaging Then
			oUpdateResultsResponse.PagingHTML = Intuitive.Web.Translation.TranslateHTML(Functions.RenderControlToString(oPaging))
		End If

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oUpdateResultsResponse)

	End Function

	Public Shared Sub SortResults(ByVal SortBy As String, ByVal SortOrder As String)

		'sort results
		If SortBy = "Price" Then BookingBase.SearchDetails.FlightResults.ResultsSort.SortBy = FlightResultHandler.eSortBy.Price

		If SortOrder = "Ascending" Then
			BookingBase.SearchDetails.FlightResults.ResultsSort.SortOrder = FlightResultHandler.eSortOrder.Ascending
		ElseIf SortOrder = "Descending" Then
			BookingBase.SearchDetails.FlightResults.ResultsSort.SortOrder = FlightResultHandler.eSortOrder.Descending
		End If

		BookingBase.SearchDetails.FlightResults.SortResults()

	End Sub

#End Region


#Region "Add to basket"

	Public Function AddFlightOptionToBasket(ByVal FlightOptionHashToken As String) As String

		Dim sRedirectQuery As String = ""

		Try

			'if flight option token is set add to basket
			If FlightOptionHashToken <> "" Then

				'create flight option from hash token to get the search price for the flight
				Dim oPreprebookFlightOption As BookingFlight.BasketFlight.FlightOption = BookingFlight.BasketFlight.FlightOption.DeHashToken(Of BookingFlight.BasketFlight.FlightOption)(FlightOptionHashToken)

				'Get Flight Markups
				Dim aFlightMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Flight AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop
				Dim iSearchPrice As Decimal = oPreprebookFlightOption.Price

				'add markup (after creating flight option)
				For Each oMarkup As BookingBase.Markup In aFlightMarkups
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							iSearchPrice -= oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							iSearchPrice -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
						Case BookingBase.Markup.eType.Percentage
							iSearchPrice -= oMarkup.Value
					End Select
				Next


				'add to basket
				BookingFlight.AddFlightToBasket(FlightOptionHashToken)


				'create flight option from hash token
				Dim oFlightOption As BookingFlight.BasketFlight.FlightOption = BookingFlight.BasketFlight.FlightOption.DeHashToken(Of BookingFlight.BasketFlight.FlightOption)(FlightOptionHashToken)
				Dim iBasketPrice As Double = BookingBase.SearchBasket.BasketFlights(0).Flight.Price

				'Compare the price of the flight before and after it prebooked to see if we have a price increase (such as from infants)
				If Not FlightResults.CustomSettings.IgnorePriceChange Then
					If iBasketPrice <> iSearchPrice Then
						sRedirectQuery = Intuitive.Functions.SafeString("?warn=pricechange&amount=" & (iBasketPrice - iSearchPrice))
					End If
				End If

				'if flight and hotel and flight option date is different from search date perform new hotel search
				If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso
				 oFlightOption.OutboundDepartureDate <> BookingBase.SearchDetails.DepartureDate Then

					'clear hotel from basket
					BookingBase.SearchBasket.BasketProperties.Clear()

					'get current search details and set to hotel only and new departure date
					Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails
					oSearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly
					oSearchDetails.DepartureDate = oFlightOption.OutboundDepartureDate

					'search
					Dim oSearchReturn As New BookingSearch.SearchReturn
					oSearchReturn = oSearchDetails.Search()
					oSearchReturn.PropertyCount = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count

					'set search back to flight and hotel
					oSearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel

					'if we have hotels setup the selected flight
					If oSearchReturn.PropertyCount > 0 Then
						BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight()
						BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
						BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder)
					End If

					'if no hotel results return
					If oSearchReturn.PropertyCount = 0 Then
						Return "Error|Sorry there are no hotels available for your selected flight date. Please try an alternative date option."
					End If

				End If

			End If

		Catch ex As Exception
			Return "Error|Sorry we have been unable to add your selected flight. Please choose an alternative flight option and try again."
		End Try

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
			If FlightResults.CustomSettings.RedirectURL <> "" Then
				Return FlightResults.CustomSettings.RedirectURL & sRedirectQuery
			Else
				Return "/search-results" & sRedirectQuery
			End If
		Else
			If FlightResults.CustomSettings.FlightOnlyRedirectURL <> "" Then
				Return FlightResults.CustomSettings.FlightOnlyRedirectURL & sRedirectQuery
			Else
				Return "/booking-summary"
			End If
		End If

	End Function

#End Region

	Public Class CustomSetting
		Public DisplayMode As String
		Public SelectButtonValue As String
		Public PerPersonPrices As Boolean
		Public TemplateOverride As String
		Public PriceFormat As String
		Public UsePoints As Boolean
		Public AddPaging As Boolean
		Public AddHeaderPaging As Boolean
		Public PagingTotalLinks As Integer = 5
		Public AddBasketXML As Boolean
		Public AddCheapestFlightToBasket As Boolean
		Public RedirectURL As String
		Public FlightOnlyRedirectURL As String
		Public IgnorePriceChange As Boolean
		Public CarrierLogoOverbranding As Boolean
		Public PagingScript As String
		Public ResetCurrentPage As Boolean
		Public BookingAdjustmentTypeCSV As String = ""
	End Class

	Public Class PagingScriptJSON
		Public ScriptPrevious As String
		Public ScriptNext As String
		Public ScriptPage As String
	End Class

	Public Class UpdateResultsResponse
		Public FlightResultsHTML As String = ""
		Public PagingHTML As String = ""
	End Class

End Class

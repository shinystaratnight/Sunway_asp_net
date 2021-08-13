Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class ResultsCarousel
	Inherits WidgetBase

	Public Shared Property CarouselSettings As CarouselSetting
		Get
			If Not HttpContext.Current.Session("resultscarousel_carouselsettings") Is Nothing Then
				Return CType(HttpContext.Current.Session("resultscarousel_carouselsettings"), CarouselSetting)
			Else
				Return New CarouselSetting
			End If
		End Get
		Set(ByVal value As CarouselSetting)
			HttpContext.Current.Session("resultscarousel_carouselsettings") = value
		End Set
	End Property


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'setup error message
		Dim sWarningMessage As String
		If Intuitive.Functions.SafeString(Me.Setting("NoResultsWarningMessage")) <> "" Then
			sWarningMessage = Intuitive.Web.Translation.GetCustomTranslation("Results Carousel", Me.Setting("NoResultsWarningMessage"))
		Else
			sWarningMessage = Intuitive.Web.Translation.GetCustomTranslation("Results Carousel", "Sorry but there is no availability for your search criteria. Please try an alternative search.")
		End If

		'set settings
		Dim oCarouselSettings As New CarouselSetting
		With oCarouselSettings
			.Title = Me.Setting("Title")
			.Mode = Intuitive.Functions.SafeEnum(Of CarouselMode)(Me.Setting("Mode"))
		
			If Intuitive.Functions.SafeInt(Me.Setting("DaysEitherSide")) > 0 Then
				.DaysEitherSide = Intuitive.Functions.SafeInt(Me.Setting("DaysEitherSide"))
			ElseIf BookingBase.Params.FlightCarouselDaysEitherSide > 0 Then
				.DaysEitherSide = BookingBase.Params.FlightCarouselDaysEitherSide
			End If

			If Intuitive.Functions.SafeInt(Me.Setting("DaysPerPage")) > 0 Then
				.DaysPerPage = Intuitive.Functions.SafeInt(Me.Setting("DaysPerPage"))
			ElseIf BookingBase.Params.FlightCarouselDaysEitherSide > 0 Then
				.DaysPerPage = 2 * BookingBase.Params.FlightCarouselDaysEitherSide + 1
			End If

			.DayWidth = Intuitive.Functions.SafeInt(Me.Setting("DayWidth"))
			.PerPersonPrices = Intuitive.Functions.SafeBoolean(Me.Setting("PerPersonPrices"))
			.PriceFormat = Intuitive.Functions.IIf(Settings.GetValue("PriceFormat") = "", "###,##0", Settings.GetValue("PriceFormat"))
			.FullDate = Intuitive.Functions.SafeBoolean(Me.Setting("FullDate"))
			.UpdateFunction = Intuitive.Functions.SafeString(Me.Setting("UpdateFunction"))
			.CSSClassOverride = Intuitive.Functions.SafeString(Me.Setting("CSSClassOverride"))
			.TemplateOverride = Intuitive.Functions.SafeString(Me.Setting("TemplateOverride"))
			.FlightPlusHotelURL = Intuitive.Functions.SafeString(Me.Setting("FlightPlusHotelURL"))
			.FlightOnlyURL = Intuitive.Functions.SafeString(Me.Setting("FlightOnlyURL"))
			.HideIfNoAlternatives = Intuitive.Functions.SafeBoolean(Me.Setting("HideIfNoAlternatives"))
			.NoResultsWarningMessage = sWarningMessage
			.BookingAdjustmentTypeCSV = Intuitive.Functions.SafeString(Settings.GetValue("BookingAdjustmentTypeCSV"))
		End With
		ResultsCarousel.CarouselSettings = oCarouselSettings


		'setup xsl
		Dim oFlightCarouselXML As XmlDocument = BookingBase.SearchDetails.FlightCarouselResults.GetResultXML(oCarouselSettings.DaysEitherSide)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
        
		'Work out if we should hide the left arrow
		Dim iDays As Double = Intuitive.Functions.IIf(BookingBase.Params.FlightCarouselSearchAgain, ResultsCarousel.CarouselSettings.DaysPerPage, _
											Math.Ceiling(ResultsCarousel.CarouselSettings.DaysPerPage / 2))

		Dim dLastDate As Date = BookingBase.SearchDetails.DepartureDate.AddDays(iDays * -1)
		Dim dMinDate As Date = Today.AddDays(BookingBase.Params.Search_BookAheadDays)

		Dim bHideLeftArrow As Boolean = dLastDate < dMinDate


		With oXSLParams

			'currency
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))

			'carousel widget settings
			.AddParam("Title", ResultsCarousel.CarouselSettings.Title)
			.AddParam("Mode", ResultsCarousel.CarouselSettings.Mode.ToString)
			.AddParam("DaysEitherSide", ResultsCarousel.CarouselSettings.DaysEitherSide)
			.AddParam("DaysPerPage", ResultsCarousel.CarouselSettings.DaysPerPage)
			.AddParam("DayWidth", ResultsCarousel.CarouselSettings.DayWidth)
			.AddParam("PerPersonPrices", ResultsCarousel.CarouselSettings.PerPersonPrices)
			.AddParam("PriceFormat", ResultsCarousel.CarouselSettings.PriceFormat)
			.AddParam("FullDate", ResultsCarousel.CarouselSettings.FullDate)
			.AddParam("HideLeftArrow", bHideLeftArrow)
			.AddParam("UpdateFunction", ResultsCarousel.CarouselSettings.UpdateFunction)
			.AddParam("CSSClassOverride", ResultsCarousel.CarouselSettings.CSSClassOverride)
			.AddParam("FlightPlusHotelURL", ResultsCarousel.CarouselSettings.FlightPlusHotelURL)
			.AddParam("FlightOnlyURL", ResultsCarousel.CarouselSettings.FlightOnlyURL)
			.AddParam("HideIfNoAlternatives", ResultsCarousel.CarouselSettings.HideIfNoAlternatives)
			.AddParam("NoResultsWarningMessage", ResultsCarousel.CarouselSettings.NoResultsWarningMessage)

			'search details
			.AddParam("SelectedDate", BookingBase.SearchDetails.FlightResults.ResultsFilter.OutboundDepartureDate.ToString("s"))
			.AddParam("TotalPax", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)

			'get results min price - if flight plus hotel we need both property and flight min price
			Dim nMinPrice As Decimal = 0
			If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso BookingBase.SearchDetails.PropertyResults.MinPrice > 0 _
			 AndAlso BookingBase.SearchDetails.FlightResults.MinPrice > 0 Then
				nMinPrice = BookingBase.SearchDetails.PropertyResults.MinPrice + BookingBase.SearchDetails.FlightResults.MinPrice
			ElseIf BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then
				nMinPrice = BookingBase.SearchDetails.PropertyResults.MinPrice
			End If

			.AddParam("PropertyResultsMinPrice", nMinPrice)
			.AddParam("FlightCarouselMode", BookingBase.Params.FlightCarouselMode)
			.AddParam("FlightCarouselSearchAgain", BookingBase.Params.FlightCarouselSearchAgain)
			.AddParam("FromFlightPrice", BookingBase.SearchDetails.FlightResults.MinPrice)
			.AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(ResultsCarousel.CarouselSettings.BookingAdjustmentTypeCSV))

		End With

		'transform
		If ResultsCarousel.CarouselSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oFlightCarouselXML, HttpContext.Current.Server.MapPath("~" & ResultsCarousel.CarouselSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oFlightCarouselXML, XSL.SetupTemplate(res.ResultsCarousel, True, True), writer, oXSLParams)
		End If


	End Sub

	Public Function SearchAgain(ByVal iDays As Integer) As String
        
		'clear down our results from basket
		BookingBase.SearchBasket.BasketFlights.Clear()
		BookingBase.SearchBasket.BasketProperties.Clear()

		'get current search details and set to hotel only and new departure date
		Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails

		'amend the search date
		oSearchDetails.DepartureDate = oSearchDetails.DepartureDate.AddDays(iDays)

		'Adjust the search date if we're searching in the past
		oSearchDetails.DepartureDate = CDate(IIf(oSearchDetails.DepartureDate < Today, Today, oSearchDetails.DepartureDate))

		'search
		Dim oSearchReturn As New BookingSearch.SearchReturn
		oSearchReturn = oSearchDetails.Search()
		oSearchReturn.PropertyCount = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count

		'perform initial filter and sort
		BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
		BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder, _
		  BookingBase.SearchDetails.PriorityPropertyID)
		BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter)

		'if flight plus hotel set selected flight
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then

			BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight()

		End If

		'serialize to json and return
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSearchReturn)


	End Function
    

#Region "Filter"

	Public Function Filter(ByVal DepartureDate As Date) As String

		BookingBase.SearchDetails.FlightResults.ResultsFilter = New FlightResultHandler.Filters

		'set outbound departure date
		BookingBase.SearchDetails.FlightResults.ResultsFilter.OutboundDepartureDate = DepartureDate

		'filter results
		BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter, True)


		'update content
		Dim sHTML As String = ResultsCarousel.UpdateContent()


		'return
		Return sHTML


	End Function


#End Region


	Public Shared Function UpdateContent() As String

		'setup xsl
		Dim oFlightCarouselXML As XmlDocument = BookingBase.SearchDetails.FlightCarouselResults.GetResultXML(ResultsCarousel.CarouselSettings.DaysEitherSide)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams

		With oXSLParams

			'currency
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))

			'carousel widget settings
			.AddParam("Title", ResultsCarousel.CarouselSettings.Title)
			.AddParam("Mode", ResultsCarousel.CarouselSettings.Mode.ToString)
			.AddParam("DaysEitherSide", ResultsCarousel.CarouselSettings.DaysEitherSide)
			.AddParam("DaysPerPage", ResultsCarousel.CarouselSettings.DaysPerPage)
			.AddParam("DayWidth", ResultsCarousel.CarouselSettings.DayWidth)
			.AddParam("PerPersonPrices", ResultsCarousel.CarouselSettings.PerPersonPrices)
			.AddParam("PriceFormat", ResultsCarousel.CarouselSettings.PriceFormat)
			.AddParam("FullDate", ResultsCarousel.CarouselSettings.FullDate)
			.AddParam("CSSClassOverride", ResultsCarousel.CarouselSettings.CSSClassOverride)
			.AddParam("FlightPlusHotelURL", ResultsCarousel.CarouselSettings.FlightPlusHotelURL)
			.AddParam("NoResultsWarningMessage", ResultsCarousel.CarouselSettings.NoResultsWarningMessage)
			.AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(ResultsCarousel.CarouselSettings.BookingAdjustmentTypeCSV))

			'search details
			.AddParam("SelectedDate", BookingBase.SearchDetails.FlightResults.ResultsFilter.OutboundDepartureDate.ToString("s"))
			.AddParam("TotalPax", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
			.AddParam("PropertyResultsMinPrice", BookingBase.SearchDetails.PropertyResults.MinPrice)
			.AddParam("FlightCarouselMode", BookingBase.Params.FlightCarouselMode)
			.AddParam("DaysOnly", True)
            
		End With

		Dim sHTML As String
		If ResultsCarousel.CarouselSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oFlightCarouselXML, HttpContext.Current.Server.MapPath("~" & ResultsCarousel.CarouselSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oFlightCarouselXML, XSL.SetupTemplate(res.ResultsCarousel, True, True), oXSLParams)
		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)


	End Function


#Region "ReSearch"

	Public Overridable Function ReSearch(ByVal DepartureDate As Date) As String

		'clear session
		BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
        BookingBase.SearchBasket = New BookingBasket(True)


		'get value pairs and decode to object
		Dim oBookingSearch As New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
		Dim sKeyValuePairs As String = oBookingSearch.GetSearchCookie
		oBookingSearch.Decode(sKeyValuePairs)

		oBookingSearch.DepartureDate = DepartureDate
		BookingBase.SearchDetails.DepartureDate = DepartureDate


		'search
		Dim oSearchReturn As New BookingSearch.SearchReturn
		oSearchReturn = oBookingSearch.Search()


		'perform initial filter and sort
		BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
		BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder)

		BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter)
		BookingBase.SearchDetails.FlightResults.SortResults()


		'if flight plus hotel set selected flight
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
			BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight()
		End If


		'setup basket guests from search
		BookingBase.SearchBasket.SetupBasketGuestsFromSearch()


		'set return object
		Dim oReturn As New CarouselSearchReturn
		oReturn.SearchMode = BookingBase.SearchDetails.SearchMode.ToString
		oReturn.FlightCount = oSearchReturn.FlightCount
		oReturn.PropertyCount = oSearchReturn.PropertyCount


		Return Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)

	End Function

#End Region

	Public Class CarouselSearchReturn
		Public SearchMode As String
		Public FlightCount As Integer
		Public PropertyCount As Integer
	End Class



	Public Class CarouselSetting
		Public Title As String
		Public Mode As CarouselMode = CarouselMode.Flight
		Public DaysEitherSide As Integer = 10
		Public DaysPerPage As Integer = 7
		Public DayWidth As Integer = 90
		Public PerPersonPrices As Boolean
		Public PriceFormat As String
		Public FullDate As Boolean
		Public UpdateFunction As String
		Public CSSClassOverride As String
		Public TemplateOverride As String
		Public FlightPlusHotelURL As String
		Public FlightOnlyURL As String
		Public HideIfNoAlternatives As Boolean
		Public NoResultsWarningMessage As String
		Public BookingAdjustmentTypeCSV As String = ""
	End Class


	Public Enum CarouselMode
		Flight
		FlightAndHotel
	End Enum

End Class

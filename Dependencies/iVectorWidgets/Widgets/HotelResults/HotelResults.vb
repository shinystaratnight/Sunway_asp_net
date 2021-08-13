Imports System.Configuration.ConfigurationManager
Imports System.Threading.Tasks
Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.BookingProperty.BasketProperty
Imports Intuitive.Web.Widgets
Imports iVectorConnectInterface.Property
Imports Newtonsoft.Json
Imports ivci = iVectorConnectInterface
Imports System.Linq

Public Class HotelResults
	Inherits WidgetBase

#Region "Constructors"
	Public Sub New()

	End Sub
#End Region

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("hotelResults_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelResults_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("hotelResults_customsettings") = value
		End Set

	End Property

	Public Shared Shadows Property PrebookReturns As Dictionary(Of string, PreBookResponse)

		Get
			If HttpContext.Current.Session("hotelResults_cancellationcharges") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelResults_cancellationcharges"), Dictionary(Of String, PreBookResponse))
			End If
			Return New Dictionary(Of String, PreBookResponse)
		End Get
		Set(value As Dictionary(Of String, PreBookResponse))
			HttpContext.Current.Session("hotelResults_cancellationcharges") = value
		End Set

	End Property

	Public Shared Property CurrentView As View

		Get
			If HttpContext.Current.Session("hotelResults_currentview") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelResults_currentview"), View)
			End If
			Return View.Results
		End Get
		Set(value As View)
			HttpContext.Current.Session("hotelResults_currentview") = SafeEnum(Of View)(value)
		End Set

	End Property

	Public Shared Property PageDef As PageDefinition

#End Region

#Region "Draw"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Set up session variables
		HotelResults.CustomSettings = GetCustomSetting()

		HotelResults.PrebookReturns = New Dictionary(Of String, PreBookResponse)()

		HotelResults.PageDef = Me.PageDefinition

		'2. do we want to add the cheapest flight to the basket? (in cases where the flight results come after the hotel results)
		If HotelResults.CustomSettings.AddCheapestFlightToBasket Then BookingBase.SearchDetails.FlightResults.AddCheapestFlightToBasket()

		'3. Setup Results XML
		Dim oXML As XmlDocument = GetHotelResultsXML()

		If HotelResults.CustomSettings.UseCustomXML Then
			Dim oCustomXML As XmlDocument = Utility.BigCXML(HotelResults.CustomSettings.CustomXMLObject, 1, 60, False)
			oXML = Intuitive.XMLFunctions.MergeXMLDocuments(oXML, oCustomXML)
		End If

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML)
		End If


		'4. Setup xsl params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = HotelResults.GetHotelResultsXSLParams("")

		'5. build xsl
		If HotelResults.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & HotelResults.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.HotelResults, True, True), writer, oXSLParams)
		End If

	End Sub

	''' <summary>
	''' Gets all the custom settings from the client
	''' </summary>
	''' <returns>Custom Setting with client's settings</returns>
	''' <remarks></remarks>
	Public Function GetCustomSetting() As CustomSetting

		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.TabbedContent = IIf(Settings.GetValue("TabbedContent") = "", True, Functions.SafeBoolean(Settings.GetValue("TabbedContent")))
			.SummaryLength = Functions.SafeInt(Microsoft.VisualBasic.IIf(Settings.GetValue("SummaryLength") = "", True, Settings.GetValue("SummaryLength")))
			.RedirectURL = Settings.GetValue("RedirectURL")
			.HotelOnlyRedirectURL = Settings.GetValue("HotelOnlyRedirectURL")
			.PackagePrice = IIf(Settings.GetValue("PackagePrice") = "", False, Functions.SafeBoolean(Settings.GetValue("PackagePrice")))
			.PerPersonPrice = IIf(Settings.GetValue("PerPersonPrice") = "", False, Functions.SafeBoolean(Settings.GetValue("PerPersonPrice")))
			.PriceFormat = IIf(Settings.GetValue("PriceFormat") = "", "###,##0.00", Settings.GetValue("PriceFormat"))
			.Regionresort = IIf(Settings.GetValue("RegionResort") = "", False, Functions.SafeBoolean(Settings.GetValue("RegionResort")))
			.DisplayBestSeller = IIf(Settings.GetValue("DisplayBestSeller") = "", False, Functions.SafeBoolean(Settings.GetValue("DisplayBestSeller")))
			.BestSellerImagePath = IIf(Settings.GetValue("BestSellerImagePath") = "", "", Functions.SafeString(Settings.GetValue("BestSellerImagePath")))
			.BestSellerPosition = IIf(Settings.GetValue("BestSellerPosition") = "", BestSellerPosition.None,
			  Functions.SafeEnum(Of BestSellerPosition)(Functions.SafeString(Settings.GetValue("BestSellerPosition"))))
			.DisplayEmailDescription = IIf(Settings.GetValue("DisplayEmailDescription") = "", False, Functions.SafeBoolean(Settings.GetValue("DisplayEmailDescription")))
			.MapTab = IIf(Settings.GetValue("MapTab") = "", False, Functions.SafeBoolean(Settings.GetValue("MapTab")))
			.TAReviewsTab = IIf(Settings.GetValue("TAReviewsTab") = "", False, Functions.SafeBoolean(Settings.GetValue("TAReviewsTab")))
			.TemplateOverride = Settings.GetValue("TemplateOverride")
			.RequestRoomPopupOverride = Settings.GetValue("RequestRoomPopupOverride")
			.HotelPriceOnly = IIf(Settings.GetValue("HotelPriceOnly") = "", False, Functions.SafeBoolean(Settings.GetValue("HotelPriceOnly")))
			'TODO - add the canx charges link to the main templates (only in bookabed custom at the moment)
			.CancellationChargesPosition = SafeEnum(Of CancellationChargesPosition)(Settings.GetValue("CancellationChargesPosition"), True)
			.UsePoints = IIf(Settings.GetValue("UsePoints") = "", False, Functions.SafeBoolean(Settings.GetValue("UsePoints")))
			.MapIconsJSON = HotelResults.GetMapIcons()
			.AddBasketXML = Functions.SafeBoolean(Settings.GetValue("AddBasketXML"))
			.AddCheapestFlightToBasket = Functions.SafeBoolean(Settings.GetValue("AddCheapestFlightToBasket"))
			.ShowTotalHolidayPriceLabel = IIf(Settings.GetValue("ShowTotalHolidayPriceLabel") = "", False, Functions.SafeBoolean(Settings.GetValue("ShowTotalHolidayPriceLabel")))
			.AddSelectedFlightToBasket = Functions.SafeBoolean(Settings.GetValue("AddSelectedFlightToBasket"))
			.BookingAdjustmentTypeCSV = Functions.SafeString(Settings.GetValue("BookingAdjustmentTypeCSV"))
			.UseCustomXML = Functions.SafeBoolean(Settings.GetValue("UseCustomXML"))
			.CustomXMLObject = Functions.SafeString(Settings.GetValue("CustomXMLObject"))
			.Upsell = Functions.SafeBoolean(Settings.GetValue("Upsell"))
			.ShowErrataOnCancellationPopup = Functions.SafeBoolean(Settings.GetValue("ShowErrataOnCancellationPopup"))
			.PrebookOnAddToBasket = Functions.SafeBoolean(Settings.GetValue("PrebookOnAddToBasket"))
		End With

		Return oCustomSettings

	End Function

	''' <summary>
	''' get our points values (currently only used for monster)
	''' </summary>
	''' <returns>Points values BigC XML</returns>
	''' <remarks></remarks>
	Private Function GetPointsXML() As XmlDocument

		Dim oPointsXML As New XmlDocument
		'We probably want to inlcude these in our Global on session start to get them in the cache early.
		oPointsXML = Utility.BigCXML("BrandTiers", 1, 600)
		oPointsXML = XMLFunctions.MergeXMLDocuments("Points", oPointsXML, Utility.BigCXML("Settings", 1, 600))

		Return oPointsXML
	End Function

	''' <summary>
	''' Get the XML for the results widget
	''' </summary>
	''' <returns>XML doc for results widget</returns>
	''' <remarks></remarks>
	Public Function GetHotelResultsXML() As XmlDocument

		'if we use points then get the points xml
		Dim PointsXML As New XmlDocument
		If HotelResults.CustomSettings.UsePoints = True Then PointsXML = GetPointsXML()

		Dim XML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(1)

		If HotelResults.CustomSettings.AddBasketXML Then
			Return XMLFunctions.MergeXMLDocuments(XML, PointsXML, BookingBase.SearchBasket.XML)
		Else
			Return XMLFunctions.MergeXMLDocuments(XML, PointsXML)
		End If
	End Function

#End Region

#Region "Form Functions"

	'gets the url from bigc - side effect of this is we already have the big c xml for that property so we should cache
	' and use this rather than get again on the next page load
	Public Function GetPropertyURL(PropertyReferenceID As Integer) As String

		Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", PropertyReferenceID, 60)
		Dim sURLNodeName As String = "URL"
		If Not BookingBase.Params.BigCPropertyURLNodeName = "" Then
			sURLNodeName = BookingBase.Params.BigCPropertyURLNodeName
		End If

		Return XMLFunctions.SafeNodeValue(oHotelXML, "Property/" & sURLNodeName)

	End Function

	''' <summary>
	''' Gets hotel info from BigC when we click on the more info tab
	''' </summary>
	''' <param name="PropertyReferenceID">PropertyReferenceID of proeprty we want info for</param>
	''' <returns>More Info HTML as string</returns>
	''' <remarks>created by Matt S 09/10/14</remarks>
	Public Function GetHotelMoreInfo(ByVal PropertyReferenceID As Integer) As String

		Dim sHTML As String = ""

		If PropertyReferenceID > 0 Then
			'get hotel xml
			Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", PropertyReferenceID, 60)

			'set up params
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("PropertyReferenceID", PropertyReferenceID)
				.AddParam("MapMarkerPath", HotelPopup.CustomSettings.MapMarkerPath)
			End With

			'transform
			If HotelResults.CustomSettings.TemplateOverride <> "" Then
				sHTML = Intuitive.XMLFunctions.XMLTransformToString(oHotelXML, HttpContext.Current.Server.MapPath("~/widgets/hotelresults/hotelcontent.xsl"), oXSLParams)
			Else
				sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oHotelXML, XSL.SetupTemplate(res.HotelContent, True, True), oXSLParams)
			End If

		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)
	End Function

#End Region

#Region "Update Results"

	Public Shared Function UpdateResults() As String

		Dim iCurrentPage As Integer = BookingBase.SearchDetails.PropertyResults.CurrentPage

		'How we get our points values (currently only used for monster)
		Dim oPointsXML As New XmlDocument
		If HotelResults.CustomSettings.UsePoints = True Then
			'We probably want to inlcude these in our Global on session start to get them in the cache early.
			oPointsXML = Utility.BigCXML("BrandTiers", 1, 600)
			oPointsXML = XMLFunctions.MergeXMLDocuments("Points", oPointsXML, Utility.BigCXML("Settings", 1, 600))
		End If

		Dim oXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(iCurrentPage)

		If HotelResults.CustomSettings.AddBasketXML Then
			Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oPointsXML, oBasketXML)
		Else
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oPointsXML)
		End If

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML)
		End If

		If HotelResults.CustomSettings.UseCustomXML Then
			Dim oCustomXML As XmlDocument = Utility.BigCXML(HotelResults.CustomSettings.CustomXMLObject, 1, 60, False)
			oXML = Intuitive.XMLFunctions.MergeXMLDocuments(oXML, oCustomXML)
		End If

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = HotelResults.GetHotelResultsXSLParams(HotelResults.CurrentView.ToString)

		Dim sHTML As String = ""
		If HotelResults.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelResults.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelResults, True, True), oXSLParams)
		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

#End Region

#Region "Tabbed Content"

	Public Shared Function TabbedContent(ByVal PropertyReferenceID As Integer, ByVal Tab As ContentTab) As String

		Dim iIndex As Integer = BookingBase.SearchDetails.PropertyResults.GetHotelResultIndex(PropertyReferenceID)

		'get result xml
		Dim oXML As New XmlDocument

		'if showing rates or flights get result xml
		If Tab = ContentTab.Rates OrElse Tab = ContentTab.Flights Then
			oXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)
		End If

		'if showing hotel details or hotel images get property content from big c
		If Tab = ContentTab.HotelDetails OrElse Tab = ContentTab.HotelImages OrElse Tab = ContentTab.HotelMap OrElse Tab = ContentTab.HotelFacilities Then
			oXML = Utility.BigCXML("Property", PropertyReferenceID, 0)
		End If

		'if showing reviews get review xml - TODO should this be moved out to a seperate function?
		If Tab = ContentTab.TAReviews Then
			Dim oiVCGetReviewsRequest As New ivci.GetReviewsRequest

			With oiVCGetReviewsRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.PropertyReferenceID = PropertyReferenceID
				.BrandID = BookingBase.Params.BrandID
				.SalesChannelID = 1
				.LanguageID = BookingBase.Params.LanguageID

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count = 0 Then
					Dim oGetReviewsRequestXML As System.Xml.XmlDocument = Serializer.Serialize(oiVCGetReviewsRequest)

					'Log the request
#If DEBUG Then
					Intuitive.FileFunctions.AddLogEntry("iVectorConnect/GetReviews", "GetReviewsRequest", "", , , , oGetReviewsRequestXML)
#End If

					'Send the request
					Dim sGetReviewsResponse As String = Intuitive.Net.WebRequests.GetResponse(BookingBase.Params.ServiceURL & "ivectorconnect.ashx",
					  oGetReviewsRequestXML.InnerXml, , "")
					Dim oGetReviewsResponseXML As New System.Xml.XmlDocument
					oGetReviewsResponseXML.LoadXml(sGetReviewsResponse)

					'Log the response
#If DEBUG Then
					Intuitive.FileFunctions.AddLogEntry("iVectorConnect/GetReviews", "GetReviewsResponse", "", , , , oGetReviewsResponseXML)
#End If

					'Check for exceptions
					If XMLFunctions.SafeNodeValue(oGetReviewsResponseXML, "//ReturnStatus/Success").ToSafeBoolean = True Then
						oXML = oGetReviewsResponseXML
					Else
						Intuitive.FileFunctions.AddLogEntry("iVectorConnect/GetReviews", "GetReviewsExeption",
						   oGetReviewsResponseXML.SelectSingleNode("//ReturnStatus/Exceptions/Exception").InnerText)
					End If

				Else
					Intuitive.FileFunctions.AddLogEntry("iVectorConnect/GetReviews", "GetReviewsExeption", Serializer.Serialize(oWarnings, True).OuterXml)
				End If

			End With

		End If

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = HotelResults.GetHotelResultsXSLParams(Tab.ToString, PropertyReferenceID)
		Dim sHTML As String = ""
		If HotelResults.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelResults.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelResults, True, True), oXSLParams)
		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

#End Region

#Region "Change Flight"

	Public Shared Function ChangeFlight(ByVal PropertyReferenceID As Integer) As String
		BookingBase.SearchDetails.PropertyResults.ChangeFlightProperties.Add(PropertyReferenceID)
		Return SelectedFlightHTML(PropertyReferenceID)
	End Function

	Public Shared Function KeepFlight(ByVal PropertyReferenceID As Integer) As String
		BookingBase.SearchDetails.PropertyResults.ChangeFlightProperties.Remove(PropertyReferenceID)
		Return SelectedFlightHTML(PropertyReferenceID)
	End Function

	Public Shared Function UpdateSelectedFlight(ByVal propertyReferenceID As Integer, ByVal flightToken As String) As String
		BookingBase.SearchDetails.PropertyResults.UpdateHotelSelectedFlight(propertyReferenceID, flightToken)
		Return HotelResults.UpdateResults()
	End Function

	Public Shared Function SelectedFlightHTML(ByVal PropertyReferenceID As Integer) As String

		Dim iIndex As Integer = BookingBase.SearchDetails.PropertyResults.GetHotelResultIndex(PropertyReferenceID)
		Dim oXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)

		Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
		oXML = XMLFunctions.MergeXMLDocuments(oXML, oFlightResultsExtraXML)

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = HotelResults.GetHotelResultsXSLParams("SelectedFlight", PropertyReferenceID)
		Dim sHTML As String = ""
		If HotelResults.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelResults.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelResults, True, True), oXSLParams)
		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)
	End Function

#End Region

#Region "HotelResults XSL Params"
	Public Shared Function GetHotelResultsXSLParams(ByVal Template As String, Optional ByVal TAPropertyReferenceID As Integer = 0) As Intuitive.WebControls.XSL.XSLParams
		Dim sIsOverbranded As String = "false"
		If HotelResults.PageDef IsNot Nothing Then
			sIsOverbranded = HotelResults.PageDef.Overbranding.isOverbranded.ToString.ToLower
		End If

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("TabbedContent", HotelResults.CustomSettings.TabbedContent)
			.AddParam("SummaryLength", HotelResults.CustomSettings.SummaryLength)
			.AddParam("RedirectURL", HotelResults.CustomSettings.RedirectURL)
			.AddParam("HotelOnlyRedirectURL", HotelResults.CustomSettings.HotelOnlyRedirectURL)
			.AddParam("PackagePrice", HotelResults.CustomSettings.PackagePrice)
			.AddParam("PerPersonPrice", HotelResults.CustomSettings.PerPersonPrice)
			.AddParam("PriceFormat", HotelResults.CustomSettings.PriceFormat)
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("DisplayBestSeller", HotelResults.CustomSettings.DisplayBestSeller)
			.AddParam("BestSellerImagePath", HotelResults.CustomSettings.BestSellerImagePath)
			.AddParam("BestSellerPosition", HotelResults.CustomSettings.BestSellerPosition)
			.AddParam("Template", Template)
			.AddParam("DisplayEmailDescription", HotelResults.CustomSettings.DisplayEmailDescription)
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("HotelArrivalDate", BookingBase.SearchDetails.HotelArrivalDate.ToString("dd MMMM yyy"))
			.AddParam("HotelDuration", BookingBase.SearchDetails.HotelDuration)
			.AddParam("MapTab", HotelResults.CustomSettings.MapTab)
			.AddParam("HotelPriceOnly", HotelResults.CustomSettings.HotelPriceOnly)
			.AddParam("TAReviewsTab", HotelResults.CustomSettings.TAReviewsTab)
			If TAPropertyReferenceID > 0 Then .AddParam("TAPropertyReferenceID", TAPropertyReferenceID)
			.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
			.AddParam("CancellationChargesPosition", HotelResults.CustomSettings.CancellationChargesPosition.ToString)
			.AddParam("BrandID", BookingBase.Params.BrandID)
			.AddParam("PriorityPropertyID", BookingBase.SearchDetails.PriorityPropertyID)
			.AddParam("MapIconsJSON", HotelResults.CustomSettings.MapIconsJSON)
			.AddParam("IsLoggedIn", BookingBase.LoggedIn)
			.AddParam("ShowTotalHolidayPriceLabel", HotelResults.CustomSettings.ShowTotalHolidayPriceLabel)
			.AddParam("TADisabled", SafeString(BookingBase.Params.TripAdvisorDisabled).ToLower)
			.AddParam("AddSelectedFlightToBasket", Functions.SafeBoolean(HotelResults.CustomSettings.AddSelectedFlightToBasket))
			.AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(HotelResults.CustomSettings.BookingAdjustmentTypeCSV))
			.AddParam("IsCommissionable", BookingBase.Trade.Commissionable)
			.AddParam("Upsell", HotelResults.CustomSettings.Upsell)
			.AddParam("TotalCommissionablePax", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
			.AddParam("FlightMarkupAmount", BookingBase.SearchDetails.FlightResults.MarkupAmount)
			.AddParam("IsOverbranded", sIsOverbranded)
			.AddParam("RoomTypeMapping", (BookingBase.UseRoomMapping _
						AndAlso BookingBase.SearchDetails.SearchMode <> BookingSearch.SearchModes.FlightPlusHotel _
						AndAlso BookingBase.SearchDetails.SearchMode <> BookingSearch.SearchModes.Anywhere).ToString())
			.AddParam("FlightSearchMode", BookingBase.SearchDetails.FlightSearchMode)
			.AddParam("CurrentCenter", BookingBase.SearchDetails.ItineraryDetails.CurrentCenter)
			.AddParam("CurrentCenterTotalPrice",
					  BookingBase.SearchBasket.TotalPrice -
					  BookingBase.SearchBasket.BasketProperties.Where(Function(o) o.MultiCenterId >= BookingBase.SearchDetails.ItineraryDetails.CurrentCenter).Sum(Function(o) o.RoomOptions.Sum(Function(p) p.TotalPrice)))
		End With
		Return oXSLParams
	End Function
#End Region

#Region "Add to basket"

	Public Function AddRoomOptionToBasket(ByVal sJSON As String) As String

		Try

			'deserialize
			Dim oAddToBasket As New AddToBasket
			oAddToBasket = Newtonsoft.Json.JsonConvert.DeserializeObject(Of AddToBasket)(sJSON)

			'get hash tokens
			Dim HashTokens As New Generic.List(Of String)
			Dim iIndex As Integer = -1
			For Each sIndex As String In oAddToBasket.PropertyRoomIndexes

				If iIndex = -1 Then iIndex = SafeInt(sIndex.Split("#"c)(0))

				Dim sToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(sIndex)
				HashTokens.Add(sToken)

			Next

			'add room(s) to basket
			BookingProperty.AddRoomsToBasket(HashTokens, MultiCenterID:=BookingBase.SearchDetails.ItineraryDetails.CurrentCenter)

			Dim oWorkTableItem As PropertyResultHandler.WorkTableItem = BookingBase.SearchDetails.PropertyResults.WorkTable.FirstOrDefault(Function(oItem) oItem.Index = iIndex)
			Dim oPropertyResult As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(oWorkTableItem.PropertyReferenceID)
			Dim oFlight As FlightResultHandler.Flight = oPropertyResult.SelectedFlight

			'if flight option has token is set then also add the flight
			If Not oFlight Is Nothing And oAddToBasket.AddSelectedFlightToBasket Then
				BookingFlight.AddFlightToBasket(oFlight.FlightOptionHashToken)
			End If

			If HotelResults.CustomSettings.PrebookOnAddToBasket AndAlso BookingBase.SearchBasket.BasketProperties.Last() IsNot Nothing Then

				If HotelResults.PrebookReturns.ContainsKey($"{oWorkTableItem.PropertyReferenceID}_{oAddToBasket.PropertyRoomIndexes(0)}") Then
					Dim oPrebookResponse As PreBookResponse = HotelResults.PrebookReturns($"{oWorkTableItem.PropertyReferenceID}_{oAddToBasket.PropertyRoomIndexes(0)}")

					BookingBase.SearchBasket.BasketErrata.AddRange(oPrebookResponse.Errata)
					BookingBase.SearchBasket.PreBookResponse = New ivci.Basket.PreBookResponse
					BookingBase.SearchBasket.PreBookResponse.PropertyBookings.Add(oPrebookResponse)
				Else

					'create a nwe basket to prebook (dont want to prebook any other crap that may be in the session one)
					Dim oBasket As New BookingBasket(True)

					'add to our basket
					oBasket.BasketProperties.Add(BookingBase.SearchBasket.BasketProperties.Last())

					'prebook the basket
					Dim oPreBookReturn As BookingBasket.PreBookReturn = oBasket.PreBook()

					BookingBase.SearchBasket.BasketErrata = oBasket.BasketErrata
					BookingBase.SearchBasket.PreBookResponse = New ivci.Basket.PreBookResponse
					If oBasket.PreBookResponse IsNot Nothing Then
						BookingBase.SearchBasket.PreBookResponse.PropertyBookings.Add(oBasket.PreBookResponse.PropertyBookings(0))
					End If

					'if not succeeded - bomb out
					If Not oPreBookReturn.OK Then Throw New Exception("Prebook failed")
				End If

			End If

		Catch ex As Exception
			'we were returning .net error to the customer - not cool
			'if this has failed then session results have probably died, in which case redirect to homepage with a nice error
			Return "/?warn=bookfailed"
		End Try

		Dim sRedirectURL As String = ""
		If HotelResults.CustomSettings.HotelOnlyRedirectURL <> "" AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then
			sRedirectURL = HotelResults.CustomSettings.HotelOnlyRedirectURL
		Else
			sRedirectURL = HotelResults.CustomSettings.RedirectURL
		End If

		Return sRedirectURL

	End Function

	Public Function AddFlightToBasketOnMobile(ByVal sJSON As String) As String

		'deserialize
		Dim oAddToBasket As New AddToBasket
		oAddToBasket = Newtonsoft.Json.JsonConvert.DeserializeObject(Of AddToBasket)(sJSON)

		Dim oPropertyResult As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(oAddToBasket.PropertyReferenceID)
		Dim oFlight As FlightResultHandler.Flight = oPropertyResult.SelectedFlight

		'if flight option has token is set then also add the flight
		If Not oFlight Is Nothing And oAddToBasket.AddSelectedFlightToBasket Then
			BookingFlight.AddFlightToBasket(oFlight.FlightOptionHashToken)
		End If

		Return Me.GetPropertyURL(oAddToBasket.PropertyReferenceID)

	End Function

#End Region

#Region "Request Room"

	Public Function GetRequestRoomHTML() As String

		Try
			Dim sHTML As String = ""
			If HotelResults.CustomSettings.TemplateOverride <> "" Then
				sHTML = Intuitive.XMLFunctions.XMLTransformToString(New XmlDocument, HttpContext.Current.Server.MapPath("~" & HotelResults.CustomSettings.RequestRoomPopupOverride), New Intuitive.WebControls.XSL.XSLParams)
			Else
				sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(New XmlDocument, XSL.SetupTemplate(res.HotelResults_RequestRoomPopup, True, True), New Intuitive.WebControls.XSL.XSLParams)
			End If
			Return Intuitive.Web.Translation.TranslateHTML(sHTML)
		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

	End Function

	Public Function RequestRoom(ByVal sJSON As String) As String

		Try
			Dim oRequestRoomData As New RequestRoomData
			oRequestRoomData = Newtonsoft.Json.JsonConvert.DeserializeObject(Of RequestRoomData)(sJSON)

			SendRequestRoomEmail(oRequestRoomData)

			Return "success"
		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

	End Function

	Public Sub SendRequestRoomEmail(ByVal oRequestRoomData As RequestRoomData)

		Dim oEmail As New Email
		Dim sb As New StringBuilder

		sb.AppendLine("Passenger Name: " & oRequestRoomData.PassengerName)
		sb.AppendLine("Passenger Email Address: " & oRequestRoomData.PassengerEmailAddress)
		sb.AppendLine("Passenger Telephone Number: " & oRequestRoomData.PassengerTelephoneNumber)
		For Each sRoomIndex As String In oRequestRoomData.RoomIndex.Split("|"c)
			If Not String.IsNullOrEmpty(sRoomIndex) Then
				Dim oProperyRoomOption As BasketPropertyRoomOption = BasketPropertyRoomOption.DeHashToken(Of BasketPropertyRoomOption)(BookingBase.SearchDetails.PropertyResults.RoomHashToken(sRoomIndex))
				sb.AppendLine("")
				sb.AppendLine("Property Name: " & oProperyRoomOption.PropertyName)
				sb.AppendLine("Property Reference ID: " & oProperyRoomOption.PropertyReferenceID)
				sb.AppendLine("Room Type: " & oProperyRoomOption.RoomType)
				sb.AppendLine("Room Type ID: " & oProperyRoomOption.PropertyRoomTypeID)
				sb.AppendLine("Meal Basis: " & Lookups.GetKeyPairValue(Lookups.LookupTypes.MealBasis, oProperyRoomOption.MealBasisID))
				sb.AppendLine("Meal Basis ID: " & oProperyRoomOption.MealBasisID)
				sb.AppendLine("Guests: " & oProperyRoomOption.GuestConfiguration.Adults & " Adult(s), " & oProperyRoomOption.GuestConfiguration.Children & " Child(ren), " & oProperyRoomOption.GuestConfiguration.Infants & " Infant(s)")
				sb.AppendLine("Start Date: " & oProperyRoomOption.ArrivalDate.ToString("D"))
				sb.AppendLine("Duration: " & oProperyRoomOption.Duration & " night(s)")
			End If
		Next

		Dim oHotelResultsXML As XmlDocument = Utility.BigCXML("HotelResults", 1, 60)
		Dim sRequestRoomEmail As String = XMLFunctions.SafeNodeValue(oHotelResultsXML, "//HotelResults/RequestRoomEmailAddress")

		With oEmail
			.SMTPHost = AppSettings("SMTPHost")
			.EmailTo = sRequestRoomEmail
			.From = "Website - " & BookingBase.Params.Domain
			.FromEmail = sRequestRoomEmail
			.Subject = "Room Request"
			.Body = sb.ToString
			.SendEmail()
		End With

	End Sub

#End Region

#Region "Map Popup"

	Public Overridable Function MapPopup(ByVal PropertyReferenceID As Integer) As String

		Dim iIndex As Integer = 0
		For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
			If oWorkTableItem.PropertyReferenceID = PropertyReferenceID Then
				iIndex = oWorkTableItem.Index
				Exit For
			End If
		Next

		Dim oHotelXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("PackagePrice", HotelResults.CustomSettings.PackagePrice)
			.AddParam("PerPersonPrice", HotelResults.CustomSettings.PerPersonPrice)
			.AddParam("PriceFormat", HotelResults.CustomSettings.PriceFormat)
			.AddParam("HotelPriceOnly", HotelResults.CustomSettings.HotelPriceOnly)
		End With

		Dim sHTML As String = Intuitive.XMLFunctions.XMLStringTransformToString(oHotelXML, XSL.SetupTemplate(res.HotelResultsMapPopup, True, True), oXSLParams)
		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

#End Region

#Region "Cancellation Charges"

	Public Shared Function GetCancellationCharges(ByVal sCancellationChargesPosition As String, ByVal iPropertyReferenceID As Integer, ByVal sIndex As String) As String

		Dim sReturn As String = ""

		Try

			'create a nwe basket to prebook (dont want to prebook any other crap that may be in the session one)
			Dim oBasket As New BookingBasket(True)

			'create basket property
			Dim oBasketProperty As New BookingProperty.BasketProperty
			oBasketProperty.ComponentID = 1

			'create room option from hash token
			Dim sRoomToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(sIndex)
			Dim oRoomOption As BookingProperty.BasketProperty.BasketPropertyRoomOption =
			 BookingProperty.BasketProperty.BasketPropertyRoomOption.DeHashToken(Of BookingProperty.BasketProperty.BasketPropertyRoomOption)(sRoomToken)
			oBasketProperty.RoomOptions.Add(oRoomOption)

			'add to our basket
			oBasket.BasketProperties.Add(oBasketProperty)

			'prebook the basket
			Dim oPreBookReturn As BookingBasket.PreBookReturn = oBasket.PreBook()

			'if not succeeded - bomb out
			If Not oPreBookReturn.OK Then Throw New Exception("Prebook failed")

			If Not HotelResults.PrebookReturns.ContainsKey($"{iPropertyReferenceID}_{sIndex}") Then
				HotelResults.PrebookReturns.Add($"{iPropertyReferenceID}_{sIndex}", oBasket.PreBookResponse.PropertyBookings.Last)
			Else
				HotelResults.PrebookReturns($"{iPropertyReferenceID}_{sIndex}") = oBasket.PreBookResponse.PropertyBookings.Last
			End If


			'get cancellation charges xml
			Dim oCancellationChargesXML As XmlDocument = Serializer.Serialize(oBasket.PreBookResponse.PropertyBookings.Last.Cancellations)
			Dim oErrataXML As XmlDocument = Serializer.Serialize(oBasket.PreBookResponse.PropertyBookings.Last.Errata)

			oCancellationChargesXML = XMLFunctions.MergeXMLDocuments(oCancellationChargesXML, oErrataXML)

			'set up xsl params
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("CancellationChargesPosition", sCancellationChargesPosition)
				.AddParam("PropertyReferenceID", iPropertyReferenceID)
				.AddParam("Index", sIndex)
				.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
				.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
				.AddParam("DateToday", Intuitive.DateFunctions.SQLDateFormat(Today.Date))
				.AddParam("DepartureDate", Intuitive.DateFunctions.SQLDateFormat(oBasketProperty.RoomOptions.Last.ArrivalDate))
				.AddParam("ShowErrataOnCancellationPopup", HotelResults.CustomSettings.ShowErrataOnCancellationPopup)
				.AddParam("UseRoomMapping", True)
			End With

			'transform xml to get canx charges html markup
			sReturn = Intuitive.XMLFunctions.XMLStringTransformToString(oCancellationChargesXML, XSL.SetupTemplate(res.HotelResults_CancellationCharges, True), oXSLParams)

		Catch ex As Exception
			sReturn = "<p ml=""CancellationCharges"">Sorry there was a problem getting the cancellation charges for this room</p>"
			sReturn += "<a class=""close"" href=""javascript:HotelResults.HideCancellationCharges('" & sCancellationChargesPosition & "', " & iPropertyReferenceID & ",'" & sIndex & "')"" ml=""Hotel Results"">hide</a>"
		End Try

		Return Intuitive.Web.Translation.TranslateHTML(sReturn)

	End Function
    Public Shared Function GetCancellationErrataForBatch(ByVal sBatch As String, ByVal sTail As String) As String
        Dim oReturns As New PrebookBatchReturn With {
                .sTail = sTail,
                .sBatch = sBatch
                }
        Dim oTasks As New Generic.List(Of System.Threading.Tasks.Task)
        For Each sProperty As String In sBatch.Split(","c).ToList()
            Dim oPropertyParts As List(Of String) = sProperty.Split("|"c).ToList()
            Dim oPreBookBasket as BookingBasket = BuildBasketToPrebook(oPropertyParts(3))
            Dim oLoginDetails as iVectorConnectInterface.LoginDetails = BookingBase.IVCLoginDetails
            Dim bLog as Boolean = BookingBase.LogAllXML
            Dim oCanxChargeTask As Task(Of PrebookTaskReturn) = Task(Of PrebookTaskReturn).Factory.StartNew(Function() PrebookPropertyBasket(oPreBookBasket,sProperty, oPropertyParts, oLoginDetails,bLog))
            oTasks.Add(oCanxChargeTask)
        Next
        Task.WaitAll(oTasks.ToArray, 10000)
        For Each oTask As Task(Of PrebookTaskReturn) In oTasks.Where(Function(o) o.Status = TaskStatus.RanToCompletion)
            oReturns.oPropertyPrebooks.Add(New PrebookBatchReturnElement(oTask.Result.sProperty, BuildPrebookHTMLString(oTask.Result.oBasket, sBatch.IndexOf(oTask.Result.sProperty).ToString(), oTask.Result.oPropertyParts(1).ToSafeInt(), oTask.Result.oPropertyParts(3))))
        Next
        Return JsonConvert.SerializeObject(oReturns)
    End Function

    Private Shared Function PrebookPropertyBasket(ByVal oBasket As BookingBasket, ByVal sProperty As String, ByVal oPropertyParts As List(Of String), ByVal oLoginDetails As iVectorConnectInterface.LoginDetails, ByVal bLog As Boolean) As PrebookTaskReturn

        Dim oPreBookReturn as BookingBasket.PreBookReturn =  oBasket.PreBook(oLoginDetails, bLog)
        Return New PrebookTaskReturn() With {
                                    .oBasket = oBasket,
                                    .oPropertyParts = oPropertyParts,
                                    .sProperty = sProperty,
                                    .oPreBookReturn = oPreBookReturn}
    End Function

    Public Shared Function GetCancellationErrata(ByVal sCancellationChargesPosition As String, ByVal iPropertyReferenceID As Integer, ByVal sIndex As String) As String
			'create a nwe basket to prebook (dont want to prebook any other crap that may be in the session one)
        Dim oBasket As BookingBasket = BuildBasketToPrebook(sIndex)
			Dim oPreBookReturn As BookingBasket.PreBookReturn = oBasket.PreBook()
			'if not succeeded - bomb out
			If Not oPreBookReturn.OK Then Throw New Exception("Prebook failed")
        Return BuildPrebookHTMLString(oBasket, sCancellationChargesPosition, iPropertyReferenceID, sIndex)
    End Function

    Private Shared Function BuildPrebookHTMLString(ByRef oBasket As BookingBasket, ByVal sCancellationChargesPosition As String, ByVal iPropertyReferenceID As Integer, ByVal sIndex As String) As String
        Dim sReturn As String = ""
        Dim sErrata As String = ""
        Dim bUseRoomMapping As Boolean = True
        Try
			If Not HotelResults.PrebookReturns.ContainsKey($"{iPropertyReferenceID}_{sIndex}") Then
				HotelResults.PrebookReturns.Add($"{iPropertyReferenceID}_{sIndex}", oBasket.PreBookResponse.PropertyBookings.Last)
			Else
				HotelResults.PrebookReturns($"{iPropertyReferenceID}_{sIndex}") = oBasket.PreBookResponse.PropertyBookings.Last
			End If


			'get cancellation charges xml
			Dim oCancellationChargesXML As XmlDocument = Serializer.Serialize(oBasket.PreBookResponse.PropertyBookings.Last.Cancellations)
			Dim oErrataXML As XmlDocument = Serializer.Serialize(oBasket.PreBookResponse.PropertyBookings.Last.Errata)

			oCancellationChargesXML = XMLFunctions.MergeXMLDocuments(oCancellationChargesXML, oErrataXML)

			'set up xsl params
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("CancellationChargesPosition", sCancellationChargesPosition)
				.AddParam("PropertyReferenceID", iPropertyReferenceID)
				.AddParam("Index", sIndex)
				.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
				.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
				.AddParam("DateToday", Intuitive.DateFunctions.SQLDateFormat(Today.Date))
                .AddParam("DepartureDate", Intuitive.DateFunctions.SQLDateFormat(oBasket.BasketProperties(0).RoomOptions.Last.ArrivalDate))
				.AddParam("ShowErrataOnCancellationPopup", HotelResults.CustomSettings.ShowErrataOnCancellationPopup)
				.AddParam("UseRoomMapping", bUseRoomMapping)
			End With

			'transform xml to get canx charges html markup
			sReturn = Intuitive.XMLFunctions.XMLStringTransformToString(oCancellationChargesXML, XSL.SetupTemplate(res.HotelResults_CancellationCharges, True), oXSLParams)
			sErrata = Intuitive.XMLFunctions.XMLStringTransformToString(oCancellationChargesXML, XSL.SetupTemplate(res.HotelResults_Errata, True), oXSLParams)

			If sErrata.EndsWith("?>") Then
				sErrata = "<p></p>"
			End If

		Catch ex As Exception
			sReturn = "<p ml=""CancellationCharges"">Sorry there was a problem getting the cancellation charges for this room</p>"
			If Not bUseRoomMapping Then
				sReturn += "<a class=""close"" href=""javascript:HotelResults.HideCancellationCharges('" & sCancellationChargesPosition & "', " & iPropertyReferenceID & ",'" & sIndex & "')"" ml=""Hotel Results"">hide</a>"
			End If
			sErrata = "<p></p>"
		End Try

		Return String.Concat(Intuitive.Web.Translation.TranslateHTML(sReturn).Replace("|", "&#124;"), "|", Intuitive.Web.Translation.TranslateHTML(sErrata).Replace("|", "&#124;"))
    End Function

    Private Shared Function BuildBasketToPrebook(sIndex As String) As BookingBasket

        Dim oBasket as New BookingBasket(True)

        'create basket property
        Dim oBasketProperty As New BookingProperty.BasketProperty
        oBasketProperty.ComponentID = 1

        'create room option from hash token
        Dim sRoomToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(sIndex)
        Dim oRoomOption As BookingProperty.BasketProperty.BasketPropertyRoomOption =
                BookingProperty.BasketProperty.BasketPropertyRoomOption.DeHashToken(Of BookingProperty.BasketProperty.BasketPropertyRoomOption)(sRoomToken)
        oBasketProperty.RoomOptions.Add(oRoomOption)

        'add to our basket
        oBasket.BasketProperties.Add(oBasketProperty)
        Return oBasket

        'prebook the basket
	End Function

#End Region

#Region "SetCurrentView"

	Public Shared Function SetCurrentView(ByVal sView As String) As String

		HotelResults.CurrentView = SafeEnum(Of View)(sView)
		Return "True"

	End Function

#End Region

#Region "Get Map Icons JSON"

	Public Shared Function GetMapIcons() As String

		Dim oMapIcons As Generic.List(Of MapIcon) = Utility.XMLToGenericList(Of MapIcon)(Utility.BigCXML("MapIcons", 1, 60))
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapIcons)

	End Function

#End Region

#Region "Support Classes"

	Public Class AddToBasket
		Public AddSelectedFlightToBasket As Boolean
		Public PropertyRoomIndexes As New Generic.List(Of String)
		Public PropertyReferenceID As Integer
		Public MultiCenterID As Integer
		Public FlightOptionHashToken As String
	End Class

	Public Class RequestRoomData
		Public RoomIndex As String
		Public PassengerName As String
		Public PassengerEmailAddress As String
		Public PassengerTelephoneNumber As String
	End Class

    Public Class PrebookBatchReturn
        Public oPropertyPrebooks As New List(Of PrebookBatchReturnElement)
        Public sBatch As String
        Public sTail As String
    End Class

    Public Class PrebookBatchReturnElement
        Public Sub New(sPrebookData As String, sHTML As String)
            Me.sPrebookData = sPrebookData
            Me.sHTML = sHTML
        End Sub
        Public sPrebookData As String
        Public sHTML As String
    End Class

    Public Class PrebookTaskReturn
        Public oBasket As BookingBasket
        Public sProperty As String
        Public oPropertyParts As List(Of String)
        Public oPreBookReturn As BookingBasket.PreBookReturn
    End Class

	Public Enum ContentTab
		Rates
		HotelDetails
		HotelImages
		HotelMap
		HotelFacilities
		Flights
		TAReviews
	End Enum

	Public Enum BestSellerPosition
		None
		Header
		Summary
	End Enum

	Public Enum CancellationChargesPosition
		None
		Popup
		NewRow 'TODO
	End Enum

	Public Enum View
		Results
		QuickView
	End Enum

	Public Class MapIcon
		Public Property Name As String
		Public Property URL As String
		Public Property Width As Integer
		Public Property Height As Integer
		Public Property AnchorX As Integer
		Public Property AnchorY As Integer
		Public Property OriginX As Integer
		Public Property OriginY As Integer
	End Class

	Public Class CustomSetting
		Public TabbedContent As Boolean
		Public SummaryLength As Integer
		Public RedirectURL As String
		Public HotelOnlyRedirectURL As String
		Public PackagePrice As Boolean
		Public PerPersonPrice As Boolean
		Public PriceFormat As String
		Public Regionresort As Boolean
		Public DisplayBestSeller As Boolean
		Public BestSellerImagePath As String
		Public BestSellerPosition As BestSellerPosition
		Public DisplayEmailDescription As Boolean
		Public MapTab As Boolean
		Public TAReviewsTab As Boolean
		Public TemplateOverride As String
		Public RequestRoomPopupOverride As String
		Public HotelPriceOnly As Boolean
		Public CancellationChargesPosition As CancellationChargesPosition = HotelResults.CancellationChargesPosition.None
		Public UsePoints As Boolean
		Public MapIconsJSON As String
		Public AddCheapestFlightToBasket As Boolean
		Public AddBasketXML As Boolean
		Public ShowTotalHolidayPriceLabel As Boolean
		Public AddSelectedFlightToBasket As Boolean
		Public BookingAdjustmentTypeCSV As String = ""
		Public UseCustomXML As Boolean
		Public CustomXMLObject As String
		Public Upsell As Boolean = False
		Public ShowErrataOnCancellationPopup As Boolean = False
		Public PrebookOnAddToBasket As Boolean = False
	End Class

#End Region

End Class
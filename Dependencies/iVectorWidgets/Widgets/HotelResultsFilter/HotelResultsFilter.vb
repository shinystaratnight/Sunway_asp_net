Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.WebControls
Imports System.Xml
Imports Intuitive.Web.PropertyResultHandler


Public Class HotelResultsFilter
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("hotelResultsfilter_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelResultsfilter_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("hotelResultsfilter_customsettings") = value
		End Set

	End Property

#End Region

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

        'build XML
		Dim oXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(1)
        Dim oPlaceholderOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("PlaceholderOverrides"), "PlaceholderOverrides")
        oXML = XMLFunctions.MergeXMLDocuments(oXML, oPlaceholderOverridesXML, Serializer.Serialize(HotelResultsFilter.CustomSettings, True))

        'get settings
		HotelResultsFilter.CustomSettings = Me.GetCustomSetting()

        'params
        Dim oXSLParams As WebControls.XSL.XSLParams = Me.GetParams()

        'transform
        If Functions.SafeString(Me.Settings.GetValue("TemplateOverride")) <> "" Then
            Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & Me.Settings.GetValue("TemplateOverride")), writer, oXSLParams)
        Else
            Me.XSLTransform(oXML, XSL.SetupTemplate(res.HotelResultsFilter, True, False), writer, oXSLParams)
        End If

	End Sub

	Public function Redraw() As string

	    'build XML
	    Dim oXML As XmlDocument = BookingBase.SearchDetails.PropertyResults.GetResultsAsHotelXML(1)
	    Dim oPlaceholderOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("PlaceholderOverrides"), "PlaceholderOverrides")
	    oXML = XMLFunctions.MergeXMLDocuments(oXML, oPlaceholderOverridesXML, Serializer.Serialize(HotelResultsFilter.CustomSettings, True))

	    Dim sHTML As String = ""
	    'params
	    Dim oXSLParams As WebControls.XSL.XSLParams = Me.GetParams()

        oXSLParams.AddParam("Template", "Filters")

	    'transform
	    If HotelResultsFilter.CustomSettings.TemplateOverride <> "" Then
	        sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelResultsFilter.CustomSettings.TemplateOverride), oXSLParams)
	    Else
	        sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelResults, True, True), oXSLParams)
	    End If

	    Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End function


    Protected Function GetParams() As WebControls.XSL.XSLParams

        'get longitude & latitude
        Dim nLongitude As Decimal = 0
        Dim nLatitude As Decimal = 0
        If BookingBase.SearchDetails.PropertyResults.WorkTable.Count > 0 Then
            nLongitude = BookingBase.SearchDetails.PropertyResults.WorkTable(0).Longitude
            nLatitude = BookingBase.SearchDetails.PropertyResults.WorkTable(0).Latitude
        End If

        'arriving destination
        Dim sDestination As String = ""
		Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails

        'Altered so Destination can take airport as parameter as well
        If oSearchDetails.ArrivingAtID > 1000000 Then
            sDestination = Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.AirportGroupAndAirport, oSearchDetails.ArrivingAtID - 1000000)
        ElseIf oSearchDetails.GeographyGroupingID > 0 Then
            sDestination = Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.GeographyGrouping, oSearchDetails.GeographyGroupingID)
        ElseIf oSearchDetails.ArrivingAtID > 0 Then
            sDestination = Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.Region, oSearchDetails.ArrivingAtID)
        Else
            sDestination = Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.Resort, oSearchDetails.ArrivingAtID * -1)
        End If

        Dim oParams As New WebControls.XSL.XSLParams
        With oParams
            .AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("PerPersonPrices", HotelResultsFilter.CustomSettings.PerPersonPrices)
            .AddParam("ShowMapLink", HotelResultsFilter.CustomSettings.ShowMapLink)
            .AddParam("ShowAverageScoreFilter", HotelResultsFilter.CustomSettings.ShowAverageScoreFilter)
            .AddParam("MaxAverageScore", IIf(HotelResultsFilter.CustomSettings.MaxAverageScore = 0, 10, HotelResultsFilter.CustomSettings.MaxAverageScore))
            .AddParam("AlwaysShow", HotelResultsFilter.CustomSettings.AlwaysShow)
            .AddParam("UseDynamicMap", HotelResultsFilter.CustomSettings.UseDynamicMap)
            .AddParam("Longitude", nLongitude)
            .AddParam("Latitude", nLatitude)
            .AddParam("SearchDestination", sDestination)
            .AddParam("TotalHotels", BookingBase.SearchDetails.PropertyResults.TotalHotels)
            .AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
            .AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(HotelResultsFilter.CustomSettings.BookingAdjustmentTypeCSV))
            .AddParam("ScrollToTopAfterFilter", HotelResultsFilter.CustomSettings.ScrollToTopAfterFilter)
            .AddParam("CustomTitle", HotelResultsFilter.CustomSettings.CustomTitle)
            .AddParam("FlightSearchMode", BookingBase.SearchDetails.FlightSearchMode)
        End With

        Return oParams

    End Function

    ''' <summary>
    ''' Gets all the custom settings from the client
    ''' </summary>
    ''' <returns>Custom Setting with client's settings</returns>
    ''' <remarks></remarks>
    Public Function GetCustomSetting() As CustomSetting

        Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .ShowMapLink = Settings.GetValue("ShowMapLink").ToSafeBoolean
            .FiltersCSV = Settings.GetValue("FiltersCSV").ToSafeString
            .PerPersonPrices = Settings.GetValue("PerPersonPrices").ToSafeBoolean
            .BookingAdjustmentTypeCSV = Settings.GetValue("BookingAdjustmentTypeCSV").ToSafeString
            .ShowAverageScoreFilter = Settings.GetValue("ShowAverageScoreFilter").ToSafeBoolean
            .MaxAverageScore = Settings.GetValue("MaxAverageScore").ToSafeInt
            .AlwaysShow = Settings.GetValue("AlwaysShow").ToSafeBoolean
            .UseDynamicMap = Settings.GetValue("UseDynamicMap").ToSafeBoolean
            .ScrollToTopAfterFilter = Functions.SafeBoolean(IIf(Settings.GetValue("ScrollToTopAfterFilter") = "", True, Settings.GetValue("ScrollToTopAfterFilter")))
            .CustomTitle = Settings.GetValue("CustomTitle").ToSafeString
            .TemplateOverride = Me.Settings.GetValue("TemplateOverride")
        End With

        Return oCustomSettings

    End Function


    Public Shared Function FilterResults(ByVal StarRatingCSV As String, ByVal TripAdvisorRatingCSV As String, ByVal MealBasisIDCSV As String,
        ByVal FacilityFilterIDCSV As String, ByVal GeographyLevel3IDCSV As String, ByVal ProductAttributeIDCSV As String,
        ByVal MinPrice As Integer, ByVal MaxPrice As Integer, ByVal HotelName As String, ByVal BestSeller As Boolean,
        ByVal MinAverageScore As Decimal, ByVal MaxAverageScore As Decimal, ByVal LandmarkID As Integer, ByVal DistanceFromLandmark As Integer,
        ByVal PropertyTypeIDCSV As String, ByVal GeographyLevel2IDCSV As String) As String

        'setup filter object
        Dim oFilter As PropertyResultHandler.Filters = BookingBase.SearchDetails.PropertyResults.ResultsFilter
        With oFilter
            .RatingCSV = StarRatingCSV
            .TripAdvisorRatingCSV = TripAdvisorRatingCSV
            .MealBasisIDCSV = MealBasisIDCSV
            .FilterFacilityIDCSV = FacilityFilterIDCSV
            .GeographyLevel3IDCSV = GeographyLevel3IDCSV
            .ProductAttributeIDCSV = ProductAttributeIDCSV
            .MinPrice = MinPrice
            .MaxPrice = MaxPrice
            .Name = HotelName
            .BestSeller = BestSeller
            .MinAverageScore = MinAverageScore
            .MaxAverageScore = MaxAverageScore
            .LandmarkID = LandmarkID
            .DistanceFromLandmark = DistanceFromLandmark
            .PropertyTypeIDCSV = PropertyTypeIDCSV
            .GeographyLevel2IDCSV = GeographyLevel2IDCSV
        End With


        'filter results
        BookingBase.SearchDetails.PropertyResults.ApplyDefaultFilters(BookingBase.SearchDetails, BookingBase.SearchDetails.PropertyResults.ResultsFilter)
        BookingBase.SearchDetails.PropertyResults.FilterResults(oFilter)


        'serialize to json and return
        Return Newtonsoft.Json.JsonConvert.SerializeObject(oFilter)


    End Function


    Public Shared Function UpdateFilter() As String

        'get current filters
        Dim oFilter As PropertyResultHandler.Filters = BookingBase.SearchDetails.PropertyResults.ResultsFilter

        'serialize to json and return
        Return Newtonsoft.Json.JsonConvert.SerializeObject(oFilter)

    End Function


    <Serializable()>
    Public Class CustomSetting

#Region "Properties"
        Private _ShowMapLink As Boolean
        Private _FiltersCSV As String
        Private _PerPersonPrices As Boolean
        Private _BookingAdjustmentTypeCSV As String
        Private _ShowAverageScoreFilter As Boolean
        Private _MaxAverageScore As Integer
        Private _AlwaysShow As Boolean
        Private _UseDynamicMap As Boolean
        Private _ScrollToTopAfterFilter As Boolean
        Private _CustomTitle As String
        Private _TempateOverride As String
#End Region

#Region "accessors and mutators"

        Public Property ShowMapLink As Boolean
            Get
                Return Me._ShowMapLink
            End Get
            Set(value As Boolean)
                Me._ShowMapLink = value
            End Set
        End Property

        Public Property FiltersCSV As String
            Get
                Return Me._FiltersCSV
            End Get
            Set(value As String)
                Me._FiltersCSV = value
            End Set
        End Property

        Public Property PerPersonPrices As Boolean
            Get
                Return Me._PerPersonPrices
            End Get
            Set(value As Boolean)
                Me._PerPersonPrices = value
            End Set
        End Property

        Public Property BookingAdjustmentTypeCSV As String
            Get
                If Not Me._BookingAdjustmentTypeCSV Is Nothing Then
                    Return Me._BookingAdjustmentTypeCSV
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                Me._BookingAdjustmentTypeCSV = value
            End Set
        End Property

        Public Property ShowAverageScoreFilter As Boolean
            Get
                Return Me._ShowAverageScoreFilter
            End Get
            Set(value As Boolean)
                Me._ShowAverageScoreFilter = value
            End Set
        End Property

        Public Property MaxAverageScore As Integer
            Get
                Return Me._MaxAverageScore
            End Get
            Set(value As Integer)
                Me._MaxAverageScore = value
            End Set
        End Property

        Public Property AlwaysShow As Boolean
            Get
                Return Me._AlwaysShow
            End Get
            Set(value As Boolean)
                Me._AlwaysShow = value
            End Set
        End Property

        Public Property UseDynamicMap As Boolean
            Get
                Return Me._UseDynamicMap
            End Get
            Set(value As Boolean)
                Me._UseDynamicMap = value
            End Set
        End Property

        Public Property ScrollToTopAfterFilter As Boolean
            Get
                Return Me._ScrollToTopAfterFilter
            End Get
            Set(value As Boolean)
                Me._ScrollToTopAfterFilter = value
            End Set
        End Property

        Public Property CustomTitle As String
            Get
                If Not Me._CustomTitle Is Nothing Then
                    Return Me._CustomTitle
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                Me._CustomTitle = value
            End Set
        End Property
        
        Public Property TemplateOverride As String
            Get
                If Not Me._TempateOverride Is Nothing Then
                    Return Me._TempateOverride
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                Me._TempateOverride = value
            End Set
        End Property
        


        'do we show the filter? (note these are effectively ReadOnly but VB doesn't serialize ReadOnly Properties so we have to have an
        'empty set).
        Public Property ShowPriceSlider As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.PriceSlider.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowHotelName As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.HotelName.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowLandmark As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.Landmark.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowTripAdvisorSlider As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.TripAdvisorSlider.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowStarRating As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.StarRating.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowTripAdvisorRating As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.TripAdvisorRating.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowMealBasis As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.MealBasis.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowResort As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.Resort.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowFacilities As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.Facilities.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

        Public Property ShowAttributes As Boolean
            Get
                If Me._FiltersCSV = "" Then Return True
                Dim Filters As String() = Me._FiltersCSV.Split(","c)
                Return Filters.Contains(FilterTypes.Attributes.ToString)
            End Get
            Set(value As Boolean)

            End Set
        End Property

#End Region

        Public Enum FilterTypes
            PriceSlider
            HotelName
            Landmark
            TripAdvisorSlider
            StarRating
            TripAdvisorRating
            MealBasis
            Resort
            Facilities
            Attributes
        End Enum


    End Class


End Class

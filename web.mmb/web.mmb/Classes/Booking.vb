Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions

Public Class Booking
	Inherits Intuitive.Web.BookingBase

	Public Shared Property SiteCaptchaKey As String
		Get
			If HttpContext.Current.Session("sitecaptcha") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("sitecaptcha"), String)
			End If
			Return ""
		End Get
		Set(value As String)
			HttpContext.Current.Session("sitecaptcha") = value
		End Set
	End Property

	Public Shared Property SecretCaptchaKey As String
		Get
			If HttpContext.Current.Session("secretcaptcha") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("secretcaptcha"), String)
			End If
			Return ""
		End Get
		Set(value As String)
			HttpContext.Current.Session("secretcaptcha") = value
		End Set
	End Property
    Public Shared Property NoResultsMessagesEnabled As Boolean
		Get
			If HttpContext.Current.Session("noresultsmessagesenabled") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("noresultsmessagesenabled"), Boolean)
			End If
			Return False
		End Get
		Set(value As Boolean)
			HttpContext.Current.Session("noresultsmessagesenabled") = value
		End Set
	End Property

	Public Overrides Sub Setup()

		'get service URL so we can get settings from BigC
		Booking.Params.ServiceURL = Config.ServiceURL

		Dim oBookingSettingsXML As XmlDocument = Utility.BookingSettingsXML(600)
		Dim oBookingSettings As Generic.List(Of BookingSetting) = Utility.XMLToGenericList(Of BookingSetting)(oBookingSettingsXML, "BookingSettings/BookingSetting")

		Dim sCurrentDomain As String = Booking.URL

		'current settings
		Dim oBookingSetting As BookingSetting = 
            oBookingSettings.Where(Function(o) o.Domain.ToLower = sCurrentDomain.ToLower _
              OrElse o.Domain.ToLower = sCurrentDomain.ToLower.Replace("https", "http")).FirstOrDefault

		If Not oBookingSetting Is Nothing Then
			Booking.Params.BrandID = oBookingSetting.BrandID
			Booking.Params.LanguageID = oBookingSetting.LanguageID
			Booking.Params.BaseLanguageID = Functions.SafeInt(IIf(oBookingSetting.BaseLanguageID > 0, oBookingSetting.BaseLanguageID, 1))
			Booking.Params.CurrencyID = oBookingSetting.CurrencyID
			Booking.Params.SellingCurrencyID = oBookingSetting.SellingCurrencyID
			Booking.Params.CMSWebsiteID = oBookingSetting.CMSWebsiteID
			Booking.Params.Website = oBookingSetting.Website
			Booking.DisplayLanguageID = oBookingSetting.LanguageID
			Booking.Params.SMTPHost = Intuitive.Functions.SafeString(Config.GetSetting("SMTPHost"))
			Booking.Params.Theme = oBookingSetting.Theme
			Booking.Params.SellingGeographyLevel1ID = oBookingSetting.SellingGeographyLevel1ID
			Booking.Params.DefaultAirportID = oBookingSetting.DefaultAirportID
			Booking.Params.Domain = oBookingSetting.Domain
			Booking.Params.RootPath = Intuitive.Functions.SafeString(Config.GetSetting("RootPath", False))
			Booking.Params.HotelResults_DefaultSortBy = SafeEnum(Of PropertyResultHandler.eSortBy)(oBookingSetting.HotelResults_DefaultSortBy)
			Booking.Params.HotelResults_DefaultSortOrder = SafeEnum(Of PropertyResultHandler.eSortOrder)(oBookingSetting.HotelResults_DefaultSortOrder)
			Booking.Params.BigCPropertyURLNodeName = Config.GetSetting("BigCPropertyURLNodeName", False)
			Booking.Params.SuppressHotelsWithoutMainImage = Intuitive.Functions.SafeBoolean(Config.GetSetting("SuppressHotelsWithoutMainImage", False))
			Booking.Params.UseVersionedAssets = Intuitive.Functions.SafeBoolean(Config.GetSetting("UseVersionedAssets", False))
			Booking.Params.PreTranslateTemplates = Intuitive.Functions.SafeBoolean(Config.GetSetting("PreTranslateTemplates", False))
			Booking.Params.UseAdvancedTracking = Intuitive.Functions.SafeBoolean(Config.GetSetting("UseAdvancedTracking", False))
			Booking.Params.ClearExtrasOnFlightOrHotelChange = Intuitive.Functions.SafeBoolean(Config.GetSetting("ClearExtrasOnFlightOrHotelChange", False))
			Booking.Params.SearchBookingAdjustments = oBookingSetting.SearchBookingAdjustments
			Booking.Params.BookingAdjustmentsCSV = oBookingSetting.BookingAdjustmentsCSV
			Booking.Params.ExcludeWidgetCSS = True
            Booking.Params.RestrictBookingAccess = oBookingSetting.RestrictBookingAccess

			Booking.Params.CMSBaseURL = oBookingSetting.CMSBaseURL
			Booking.Params.AllowMultipleComponents = False

			Booking.Params.ErrorEmail = IIf(oBookingSetting.ErrorEmail <> "", oBookingSetting.ErrorEmail, Intuitive.Functions.SafeString(Config.GetSetting("ErrorEmail", False)))

			'captcha keys
			Booking.SiteCaptchaKey = oBookingSetting.SiteCaptchaKey
			Booking.SecretCaptchaKey = oBookingSetting.SecretCaptchaKey

			'ivectorconnect
			Booking.Params.iVectorConnectLogin = oBookingSetting.Login
			Booking.Params.iVectorConnectPassword = oBookingSetting.Password
			Booking.Params.iVectorConnectTradeLogin = oBookingSetting.TradeLogin
			Booking.Params.iVectorConnectTradePassword = oBookingSetting.TradePassword
			Booking.Params.ServiceTimeout = 60000
			Booking.Params.Log = Intuitive.Functions.IsDebugging

			Booking.Params.Search_DefaultSearchMode = oBookingSetting.DefaultSearchMode
			Booking.Params.Search_DefaultDate = Now.AddDays(14)
			Booking.Params.Search_DefaultDuration = 7
			Booking.Params.Search_BookAheadDays = oBookingSetting.SearchBookAheadDays

			'deeplink search
			Booking.Params.Deeplink_DefaultMinDuration = oBookingSetting.DeeplinkDefaultMinDuration
			Booking.Params.Deeplink_DefaultMaxDuration = oBookingSetting.DeeplinkDefaultMaxDuration

			'hotel results
			Booking.Params.HotelResultsPerPage = 20
			Booking.Params.HotelResultsPackagePrices = True
            
            'flights
			Booking.Params.FlightCarouselNeoCache = oBookingSetting.NeoCache
            Booking.Params.CarouselMode = oBookingSetting.CarouselMode
            Booking.NoResultsMessagesEnabled = oBookingSetting.NoResultsMessagesEnabled

			If oBookingSetting.FlightCarouselDaysEitherSide = 0 Then
				Booking.Params.FlightCarouselDaysEitherSide = 2
			Else
				Booking.Params.FlightCarouselDaysEitherSide = oBookingSetting.FlightCarouselDaysEitherSide
			End If
			If oBookingSetting.FlightResultsPerPage = 0 Then
				Booking.Params.FlightResultsPerPage = 20
			Else
				Booking.Params.FlightResultsPerPage = oBookingSetting.FlightResultsPerPage
			End If
			If oBookingSetting.FlightCarouselMode = "" Then
				Booking.Params.FlightCarouselMode = BookingSearch.FlightCarouselModes.Results
			Else
				Booking.Params.FlightCarouselMode = Intuitive.Functions.SafeEnum(Of BookingSearch.FlightCarouselModes)(oBookingSetting.FlightCarouselMode)
			End If

			Booking.Params.SpecificAirportsEnabled = oBookingSetting.SpecificAirportsEnabled
			Booking.Params.ShowFlightClassInResults = oBookingSetting.ShowFlightClassInResults

			'price change warning supressor
			Booking.Params.SuppressPriceChangeWarning = oBookingSetting.SuppressPriceChangeWarning

			'logging
			Booking.Params.LogConnectString = Intuitive.Functions.SafeString(Config.GetSetting("LogConnectString", False))
            Booking.Params.LogPerformanceData = Intuitive.Functions.SafeBoolean(Config.GetSetting("LogPerformanceData", False))
            Booking.Params.UseTranspiledResources = True
        Else

			'add log entry
			Dim sb As New StringBuilder
			sb.Append(Functions.CreateFixedLengthString("URL", 24)).Append(sCurrentDomain).AppendLine()
			Intuitive.FileFunctions.AddLogEntry("Booking Setup", "URL Not Found", sb.ToString)

		End If

	End Sub

	Public Shared ReadOnly Property URL As String
		Get
			Dim sURL As String = ""
#If Not Debug Then
			sURL= Intuitive.Functions.GetBaseURL.Replace(":81", "").Replace(":80", "")
#Else
			sURL = Config.WebsiteURL
#End If

			'get URL and remove port in case this was included

			Dim oWebsiteSettingsXML As New System.Xml.XmlDocument
			oWebsiteSettingsXML.Load(HttpContext.Current.Server.MapPath("~/WebsiteURLSettings.xml"))

			'the site may be visited by ip address or monitor so need to return the correct website URL for these cases
			For Each oNode As System.Xml.XmlNode In oWebsiteSettingsXML.SelectNodes("/WebsiteURLSettings/URL")
				If sURL = XMLFunctions.SafeNodeValue(oNode, "BaseURL") Then
					sURL = XMLFunctions.SafeNodeValue(oNode, "WebsiteURL")
					Exit For
				End If
			Next

			Return sURL

		End Get
	End Property

	Public Class BookingSetting
		Public SearchBookAheadDays As Integer
		Public Domain As String
		Public BrandID As Integer
		Public LanguageID As Integer
		Public BaseLanguageID As Integer
		Public CurrencyID As Integer
		Public SellingCurrencyID As Integer
		Public CMSWebsiteID As Integer
		Public Website As String
		Public Login As String
		Public Password As String
		Public TradeLogin As String
		Public TradePassword As String
		Public CMSBaseURL As String
		Public Theme As String
		Public FlightResultsPerPage As Integer
		Public FlightCarouselDaysEitherSide As Integer
		Public FlightCarouselMode As String
		Public CarouselMode As Boolean
		Public DefaultAirportID As Integer
		Public SellingGeographyLevel1ID As Integer
		Public DeeplinkDefaultMinDuration As Integer
		Public DeeplinkDefaultMaxDuration As Integer
		Public HotelResults_DefaultSortBy As PropertyResultHandler.eSortBy
		Public HotelResults_DefaultSortOrder As PropertyResultHandler.eSortOrder
		Public NeoCache As Boolean
		Public SiteCaptchaKey As String
		Public SecretCaptchaKey As String
		Public ErrorEmail As String = ""
		Public DefaultSearchMode As BookingSearch.SearchModes
		Public SuppressHotelsWithoutMainImage As Boolean
		Public SpecificAirportsEnabled As Boolean
		Public SearchBookingAdjustments As Boolean = False
		Public BookingAdjustmentsCSV As String = ""
		Public ShowFlightClassInResults As Boolean
		Public ExcludeWidgetCSS As Boolean = False
		Public SuppressPriceChangeWarning As Boolean = False
        Public NoResultsMessagesEnabled As Boolean = False
        Public ResortSpecificLandmarks As Boolean = False
        Public RestrictBookingAccess As Boolean
	End Class

End Class
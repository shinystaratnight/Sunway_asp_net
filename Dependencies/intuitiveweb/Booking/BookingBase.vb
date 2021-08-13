Public MustInherit Class BookingBase

	Public Sub New()

		'setup params
		BookingBase.Params = New ParamDef
		Me.Setup()

		'setup lookups
		Dim oLookups As New Lookups(BookingBase.Params)
		BookingBase.Lookups = oLookups

		'setup search
		Dim oSearch As New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
		oSearch.Setup()
		BookingBase.SearchDetails = oSearch

		'setup basket
		Dim oBasket As New BookingBasket(True)
		BookingBase.Basket = oBasket

		'setup search basket
		Dim oSearchBasket As New BookingBasket(True)
		BookingBase.SearchBasket = oSearchBasket

		'data logger
		Dim oDataLogger As New DataStore.Logger(BookingBase.Params.LogPerformanceData, BookingBase.Params.LogConnectString)
		BookingBase.DataLogger = oDataLogger

	End Sub

	Public Shared DataLogger As New DataStore.Logger

	'force override of setup on inheriting classes
	Public MustOverride Sub Setup()

	Public Shared Function CloneLoginDetails() As LoginDetails
		Return Serializer.DeSerialize(Of LoginDetails)(Serializer.Serialize(BookingBase.IVCLoginDetails, True).InnerXml)
	End Function

#Region "holders for params, basket, searchdetails"

	Public Shared Property Lookups As Lookups
		Get
			If Not BookingBase.Session.Get("__booking_lookups") Is Nothing Then
				Return CType(BookingBase.Session.Get("__booking_lookups"), Lookups)
			Else
				Dim oLookups As New Lookups(BookingBase.Params)
				BookingBase.Lookups = oLookups
				Return oLookups
			End If
		End Get
		Set(value As Lookups)
			BookingBase.Session.Set("__booking_lookups", value)
		End Set
	End Property

	Public Shared Property Params As ParamDef
		Get
			Return CType(HttpRuntime.Cache("__booking_params"), ParamDef)
		End Get
		Set(value As ParamDef)
			HttpRuntime.Cache("__booking_params") = value
		End Set
	End Property

	Public Shared Property AgentSessions As Dictionary(Of Integer, String)
		Get

			If Not HttpRuntime.Cache("__booking_agent_sessions") Is Nothing Then
				Return CType(HttpRuntime.Cache("__booking_agent_sessions"), Dictionary(Of Integer, String))
			Else
				Dim oDictionary As New Dictionary(Of Integer, String)
				BookingBase.AgentSessions = oDictionary
				Return oDictionary
			End If

		End Get
		Set(value As Dictionary(Of Integer, String))
			HttpRuntime.Cache("__booking_agent_sessions") = value
		End Set
	End Property

	Public Shared Property LogAllXML As Boolean
		Get
			If Not BookingBase.Session.Get("__booking_logallxml") Is Nothing Then
				Return CType(BookingBase.Session.Get("__booking_logallxml"), Boolean)
			Else
				Return False
			End If
		End Get
		Set(value As Boolean)
			BookingBase.Session.Set("__booking_logallxml", value)
		End Set
	End Property

	Public Shared Property Basket As BookingBasket
		Get

			If Not BookingBase.Session.Get("__booking_basket") Is Nothing Then
				Return CType(BookingBase.Session.Get("__booking_basket"), BookingBasket)
			Else
				Dim oBasket As New BookingBasket(True)
				BookingBase.Basket = oBasket
				Return oBasket
			End If

		End Get
		Set(value As BookingBasket)
			BookingBase.Session.Set("__booking_basket", value)
		End Set
	End Property

	Public Shared Property SearchBasket As BookingBasket
		Get

			If Not BookingBase.Session.Get("__booking_searchbasket") Is Nothing Then
				Return CType(BookingBase.Session.Get("__booking_searchbasket"), BookingBasket)
			Else
				Dim oBasket As New BookingBasket(True)
				BookingBase.SearchBasket = oBasket
				Return oBasket
			End If

		End Get
		Set(value As BookingBasket)
			BookingBase.Session.Set("__booking_searchbasket", value)
		End Set
	End Property

	Public Shared Property SearchDetails As BookingSearch
		Get
			Dim oSearch As BookingSearch = CType(BookingBase.Session.Get("__booking_searchdetails"), BookingSearch)
			If oSearch Is Nothing Then
				oSearch = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
				oSearch.Setup()
				BookingBase.SearchDetails = oSearch
			End If
			Return oSearch
		End Get
		Set(value As BookingSearch)
			BookingBase.Session.Set("__booking_searchdetails", value)
		End Set
	End Property

	Public Shared Property UseRoomMapping As Boolean
		Get
			Dim bUseRoomMapping As Boolean = CType(BookingBase.Session.Get("__booking_useroommapping"), Boolean)
			Return bUseRoomMapping
		End Get
		Set(value As Boolean)
			BookingBase.Session.Set("__booking_useroommapping", value)
		End Set
	End Property

	Public Shared Property SearchAvailabilities As BookingAvailability.Availability
		Get
			Dim oAvailabilities As BookingAvailability.Availability = CType(BookingBase.Session.Get("__booking_searchavailabilities"), BookingAvailability.Availability)
			If oAvailabilities Is Nothing Then
				oAvailabilities = New BookingAvailability.Availability
				BookingBase.SearchAvailabilities = oAvailabilities
			End If
			Return oAvailabilities
		End Get
		Set(value As BookingAvailability.Availability)
			BookingBase.Session.Set("__booking_searchavailabilities", value)
		End Set
	End Property

	Public Shared Property SearchBookings As BookingManagement
		Get
			Dim oBookings As BookingManagement = CType(BookingBase.Session.Get("__booking_savedbookings"), BookingManagement)
			If oBookings Is Nothing Then
				oBookings = New BookingManagement
				BookingBase.SearchBookings = oBookings
			End If
			Return oBookings
		End Get
		Set(value As BookingManagement)
			BookingBase.Session.Set("__booking_savedbookings", value)
		End Set
	End Property

	Public Shared Property Trade As BookingTrade
		Get
			Dim oTrade As BookingTrade = CType(BookingBase.Session.Get("__booking_trade"), BookingTrade)
			If oTrade Is Nothing Then
				oTrade = New BookingTrade
				BookingBase.Trade = oTrade
			End If
			Return oTrade
		End Get
		Set(value As BookingTrade)
			BookingBase.Session.Set("__booking_trade", value)
		End Set
	End Property

	Public Shared Property DisplayLanguageID As Integer
		Get
			Dim iDisplayLanguageID As Integer = 0
			If HttpContext.Current IsNot Nothing AndAlso Not BookingBase.Session.Get("__booking_displaylanguageid") Is Nothing Then
				iDisplayLanguageID = Functions.SafeInt(BookingBase.Session.Get("__booking_displaylanguageid"))
			End If
			If iDisplayLanguageID <= 0 Then iDisplayLanguageID = BookingBase.Params.LanguageID
			Return iDisplayLanguageID
		End Get
		Set(value As Integer)
			BookingBase.Session.Set("__booking_displaylanguageid", value)
		End Set
	End Property

	Public Shared Property CurrencyID As Integer
		Get
			Dim iCurrencyID As Integer = 0
			If Not BookingBase.Session.Get("__booking_currencyid") Is Nothing Then
				iCurrencyID = Functions.SafeInt(BookingBase.Session.Get("__booking_currencyid"))
			End If
			If iCurrencyID <= 0 Then iCurrencyID = BookingBase.Params.CurrencyID
			Return iCurrencyID
		End Get
		Set(value As Integer)
			BookingBase.Session.Set("__booking_currencyid", value)
		End Set
	End Property

	Public Shared Property VisitID As Integer
		Get
			Dim iVisitID As Integer = 0
			If Not BookingBase.Session.Get("__booking_visitid") Is Nothing Then
				iVisitID = Functions.SafeInt(BookingBase.Session.Get("__booking_visitid"))
			End If
			Return iVisitID
		End Get
		Set(value As Integer)
			BookingBase.Session.Set("__booking_visitid", value)
		End Set
	End Property

	Public Shared Property LoggedIn As Boolean
		Get
			Dim bLoggedIn As Boolean = False
			If Not BookingBase.Session.Get("__booking_loggedin") Is Nothing Then
				bLoggedIn = Functions.SafeBoolean(BookingBase.Session.Get("__booking_loggedin"))
			End If
			Return bLoggedIn
		End Get
		Set(value As Boolean)
			BookingBase.Session.Set("__booking_loggedin", value)
		End Set
	End Property

	Public Shared Property SellingCurrencyID As Integer
		Get
			Dim iSellingCurrencyID As Integer = 0
			If Not BookingBase.Session.Get("__booking_sellingcurrencyid") Is Nothing Then
				iSellingCurrencyID = Functions.SafeInt(BookingBase.Session.Get("__booking_sellingcurrencyid"))
			End If
			If iSellingCurrencyID <= 0 Then iSellingCurrencyID = BookingBase.Params.SellingCurrencyID
			Return iSellingCurrencyID
		End Get
		Set(value As Integer)
			BookingBase.Session.Set("__booking_sellingcurrencyid", value)
		End Set
	End Property

	Public Shared Property Markups As Generic.List(Of Markup)
		Get
			If BookingBase.Params.TestProject Then
				' added to stop test projects erroring, if markup is needed set a markup object on the test project and get this to return that- JS 13/5/14
				Return New Generic.List(Of Markup)
			ElseIf Not BookingBase.Session.Get("__booking_markups") Is Nothing Then
				Return CType(BookingBase.Session.Get("__booking_markups"), Generic.List(Of Markup))
			ElseIf CookieFunctions.Cookies.Exists("__booking_markups") Then
				Try
					Dim oMarkupCookie As MarkupCookie = Serializer.DeSerialize(Of MarkupCookie)(Intuitive.Functions.Decrypt(CookieFunctions.Cookies.GetValue("__booking_markups")), False)
					BookingBase.Session.Set("__booking_markups", oMarkupCookie.Markups)
					Return oMarkupCookie.Markups
				Catch ex As Exception
				End Try
			End If

			Dim oMarkups As New Generic.List(Of Markup)
			BookingBase.Session.Set("__booking_markups", oMarkups)
			Return oMarkups
		End Get
		Set(value As Generic.List(Of Markup))
			BookingBase.Session.Set("__booking_markups", value)
			Dim oMarkupCookie As New MarkupCookie With {.Markups = value}
			Intuitive.CookieFunctions.Cookies.SetValue("__booking_markups", Intuitive.Functions.Encrypt(Serializer.Serialize(oMarkupCookie).OuterXml), CookieFunctions.CookieExpiry.OneMonth)
		End Set
	End Property

	Public Shared Property iVectorConnectLoginOverride As String
		Get
			If Not BookingBase.Session.Get("__booking_ivcloginoverride") Is Nothing Then
				Return Functions.SafeString(BookingBase.Session.Get("__booking_ivcloginoverride"))
			Else
				Return ""
			End If
		End Get
		Set(value As String)
			BookingBase.Session.Set("__booking_ivcloginoverride", value)
		End Set
	End Property

	Public Shared Property iVectorConnectPasswordOverride As String
		Get
			If Not BookingBase.Session.Get("__booking_ivcpasswordoverride") Is Nothing Then
				Return Functions.SafeString(BookingBase.Session.Get("__booking_ivcpasswordoverride"))
			Else
				Return ""
			End If
		End Get
		Set(value As String)
			BookingBase.Session.Set("__booking_ivcpasswordoverride", value)
		End Set
	End Property

	Public Shared ReadOnly Property IVCLoginDetails As iVectorConnectInterface.LoginDetails
		Get

			Dim sLogin As String = BookingBase.Params.iVectorConnectLogin
			If BookingBase.iVectorConnectLoginOverride <> "" Then
				sLogin = BookingBase.iVectorConnectLoginOverride
			ElseIf BookingBase.LoggedIn AndAlso BookingBase.Params.iVectorConnectTradeLogin <> "" Then
				sLogin = BookingBase.Params.iVectorConnectTradeLogin
			End If

			Dim sPassword As String = BookingBase.Params.iVectorConnectPassword
			If BookingBase.iVectorConnectPasswordOverride <> "" Then
				sPassword = BookingBase.iVectorConnectPasswordOverride
			ElseIf BookingBase.LoggedIn AndAlso BookingBase.Params.iVectorConnectTradePassword <> "" Then
				sPassword = BookingBase.Params.iVectorConnectTradePassword
			End If

			Dim oLoginDetails As New iVectorConnectInterface.LoginDetails
			With oLoginDetails
				.Login = sLogin
				.Password = sPassword
				.AgentReference = BookingBase.Trade.AgentReference
				.SellingCurrencyID = BookingBase.SellingCurrencyID
				.LanguageID = BookingBase.DisplayLanguageID
				.TrackingAffiliateID = BookingBase.Basket.TrackingAffiliateID
			End With

			Return oLoginDetails
		End Get
	End Property

	Public Shared Property EmailSearchImage As String
		Get
			If Not BookingBase.Session.Get("__booking_emailsearchimage") Is Nothing Then
				Return Functions.SafeString(BookingBase.Session.Get("__booking_emailsearchimage"))
			Else
				Return ""
			End If
		End Get
		Set(value As String)
			BookingBase.Session.Set("__booking_emailsearchimage", value)
		End Set
	End Property

#End Region

#Region "param definition class"

	Public Class ParamDef

		'website
		Public Property BrandID As Integer
		Public Property LanguageID As Integer
		Public Property BaseLanguageID As Integer
		Public Property CurrencyID As Integer
		Public Property SellingCurrencyID As Integer
		Public Property CMSWebsiteID As Integer
		Public Property Website As String
		Public Property Theme As String
		Public Property SMTPHost As String = ""
		Public Property Domain As String
		Public Property CMSBaseURL As String
		Public Property SellingGeographyLevel1ID As Integer
		Public Property SellingCountryISO2Code As String 'ISO 3166-1 Alpha-2 code
		Public Property LogPath As String
		Public Property AllowMultipleComponents As Boolean = False
		Public Property LoggedInCustomersOnly As Boolean = False
		Public Property TaxExclusiveRates As Boolean = False
		Public Property StandardAirportArrivalMinutes As Integer = 0
		Public Property UseAdvancedTracking As Boolean 'uses booking tracking affiliates + tokens instead of old widget
		Public Property SearchBookingAdjustments As Boolean = False
		Public Property BookingAdjustmentsCSV As String = "" 'list of BookingAdjustmentType.AdjustmentType in csv format
		Public Property ExcludeWidgetCSS As Boolean = False
		Public Property RestrictBookingAccess As Boolean = False
        Public Property GoogleMapsApiKey As String = ""
        Public Property TicketingOffset As Integer = 0
        Public Property AutomaticPrebook As Boolean = False

        'User
        Public Property LoggedIn As Boolean = False
		Public Property IsTrade As Boolean = False
		Public Property UseCustomerCurrency As Boolean = False
		Public Property UniqueTradeSessions As Boolean = False

		'ivector connect
		Public Property ServiceURL As String
		Public Property BookingServiceURL As String = ""
		Public Property iVectorConnectLogin As String
		Public Property iVectorConnectPassword As String
		Public Property iVectorConnectTradeLogin As String
		Public Property iVectorConnectTradePassword As String
		Public Property iVectorConnectAgentReference As String
		Public Property ServiceTimeout As Integer
		Public Property Log As Boolean = Intuitive.Functions.IsDebugging
		Public Property ErrorEmail As String = ""
		Public Property OverrideTranslationAgentReference As String = ""
		Public Property ShowAllLiveCustomerPayments As Boolean = False

		Public ReadOnly Property LogConnectStringProperty As String
			Get
				Return LogConnectString
			End Get
		End Property

		Public Property OverrideAgentReference As String = ""

		'Override
		Public Property CustomQueryOverrideiVCLogin As String = ""
		Public Property CustomQueryOverrideiVCPassword As String = ""

		'search
		Public Property Search_DefaultSearchMode As BookingSearch.SearchModes
		Public Property Search_DefaultDate As Date
		Public Property Search_DefaultDuration As Integer
		Public Property Search_DefaulRoomAdults As Integer = 2
		Public Property Search_BookAheadDays As Integer = 0
		Public Property Search_CookieExpiry As CookieFunctions.CookieExpiry = CookieFunctions.CookieExpiry.OneWeek

		'deeplink search
		Public Property Deeplink_DefaultMinDuration As Integer = 1
		Public Property Deeplink_DefaultMaxDuration As Integer = 29

		'hotel results
		Public Property HotelResultsPerPage As Integer = 10
		Public Property HotelResultsPackagePrices As Boolean = False 'Used for display on hotel results to decide if we want to display prices together
		Public Property HotelFilterPackagePrices As Boolean = False 'Used for Filter/Sort to decide if we want to filter/sort based on combined price.
		Public Property HotelResults_DefaultSortBy As PropertyResultHandler.eSortBy = PropertyResultHandler.eSortBy.Price
		Public Property HotelResults_DefaultSortOrder As PropertyResultHandler.eSortOrder = PropertyResultHandler.eSortOrder.Ascending
		Public Property SuppressHotelsWithoutMainImage As Boolean
		Public Property HotelResults_PropertyMinimumScore As Decimal = -1
		Public Property HotelResults_PropertyMinimumScoreLabel As String = ""
		Public Property SuppressOnRequestRoomTypes As Boolean = False

		'flights
		Public Property DirectFlightsOnly As Boolean = False
		Public Property DefaultAirportID As Integer
		Public Property FlightCarouselMode As BookingSearch.FlightCarouselModes = BookingSearch.FlightCarouselModes.None
		Public Property FlightCarouselDaysEitherSide As Integer
		Public Property FlightCarouselSearchAgain As Boolean = False
		Public Property FlightCalendarCarouselSearch As Boolean = False
		Public Property FlightResultsPerPage As Integer = 10
		Public Property FlightCarouselNeoCache As Boolean = False
		Public Property SpecificAirportsEnabled As Boolean = False
		Public Property ShowFlightClassInResults As Boolean = False
		Public Property ExactMatchesOnly As Boolean = False
		Public Property CarouselMode As Boolean = False
        public Property MultiCenterAddReturnToDepartureAirport As Boolean = false

		'Transfers
		Public Property DedupeTransfers As Boolean = True
		Public Property UseReturnTransferJourneyTime As Boolean = True

		'extras
		Public Property ClearExtrasOnFlightOrHotelChange As Boolean = False

		'anywhere search
		Public Property AnywhereSearchMaxFlightsPerRequest As Integer = 10

		'bookings
		Public Property BookingsPerPage As Integer = 20
		Public Property EarliestDepartureDateAddDays As Integer = 1

		'payment
		Public Property PaymentURL As String = "/payment"
		Public Property SecurePaymentPage As Boolean = False

		'price change warning message suppressor
		Public Property SuppressPriceChangeWarning As Boolean = False

		'reviews
		Public Property TripAdvisorDisabled As Boolean

		'test prject
		Public Property TestProject As Boolean = False

		'used if we do not want the app to run under a base directory instead of the root eg /holidays/default.aspx
		'default should be empty
		'when set up should start with a slash, and have no trailing slash eg "/holidays"
		Public Property RootPath As String = ""

		'LCH BigC XML as a different URL Node name to every other client
		'in order to make the code generic we need this setting to override for LCH based sites
		Public Property BigCPropertyURLNodeName As String = ""

		'warning emails for booking failure
		Private _SendFailedBookingEmail As Boolean = False
		Private _WarningEmail As String = ""

		'logging
		Public LogConnectString As String
		Public LogPerformanceData As Boolean

		'misc
		Public UseVersionedAssets As Boolean
		Public PreTranslateTemplates As Boolean
		Public UseTranspiledResources As Boolean = False
		Public UseRoomMapping As Boolean = True

		Public Property SendFailedBookingEmail As Boolean
			Get
				Return Me._SendFailedBookingEmail
			End Get
			Set(value As Boolean)
				Me._SendFailedBookingEmail = value
			End Set
		End Property

		Public Property WarningEmail As String
			Get
				Return Me._WarningEmail
			End Get
			Set(value As String)
				Me._WarningEmail = value
			End Set
		End Property

	End Class

#End Region

#Region "session handler"

	'wrapper so if debugging use the cache rather than the session
	Public Class Session

		Public Shared Function [Get](ByVal Key As String) As Object

			'if running test project use the cache as session not available
			If BookingBase.Params.TestProject Then
				Return HttpRuntime.Cache(Key)
			End If

			Return HttpContext.Current.Session(Key)
		End Function

		Public Shared Sub [Set](ByVal Key As String, ByVal [Object] As Object)

			'if running test project use the cache as session not available
			If BookingBase.Params.TestProject Then
				HttpRuntime.Cache(Key) = [Object]
				Return
			End If

			HttpContext.Current.Session(Key) = [Object]
		End Sub

	End Class

#End Region

#Region "Search To Main Basket"

	Public Shared Sub SearchToMainBasket()

		'0. sanity checks
		If BookingBase.SearchBasket.TotalComponents = 0 Then Throw New Exception("There must be at least one component on the search basket")

		'1. if not multiple components clear down main basket first
		If Not BookingBase.Params.AllowMultipleComponents Then
			BookingBase.Basket = New BookingBasket(True)
		End If

		'2. add search guests to basket guests
		BookingBase.Basket.GuestDetails.AddRange(BookingBase.SearchBasket.GuestDetails)

		'lead guest details
		BookingBase.Basket.LeadCustomer = BookingBase.SearchBasket.LeadCustomer
		BookingBase.Basket.CustomerPassword = BookingBase.SearchBasket.CustomerPassword

		'3. add search basket components to basket

		'properties
		BookingBase.Basket.BasketProperties.AddRange(BookingBase.SearchBasket.BasketProperties)

		'flights
		BookingBase.Basket.BasketFlights.AddRange(BookingBase.SearchBasket.BasketFlights)

		'transfers
		BookingBase.Basket.BasketTransfers.AddRange(BookingBase.SearchBasket.BasketTransfers)

		'extras
		BookingBase.Basket.BasketExtras.AddRange(BookingBase.SearchBasket.BasketExtras)

		'car hires
		BookingBase.Basket.BasketCarHires.AddRange(BookingBase.SearchBasket.BasketCarHires)

		'clear search basket if in multi-component mode
		If BookingBase.Params.AllowMultipleComponents Then
			BookingBase.SearchBasket = New BookingBasket(True)
		End If

		'reset pre booked
		BookingBase.Basket.PreBooked = False

	End Sub

#End Region

#Region "Markup"
	Public Class Markup
		Public Component As eComponentType
        public SubComponent As eSubComponentType
		Public Type As eType
		Public Value As Decimal

		Public Enum eComponentType
			Basket
			[Property]
			Flight
			Transfer
			Extra
            CarHire
        End Enum

        public Enum eSubComponentType
            None
            Excursion
            Tour
		End Enum

		Public Enum eType
			Amount 'total
			AmountPP 'per person
			Percentage
		End Enum
	End Class
	Public Class MarkupCookie
		Public Markups As New Generic.List(Of Markup)
	End Class
#End Region

#Region "clear basket"

	Public Shared Sub ClearSearchBasket()

		'1.If we are on a site that cares about keeping the customer Logged in we want to save the email before clearing down.
		Dim LeadGuestEmail As String = ""
		If BookingBase.Params.LoggedInCustomersOnly AndAlso BookingBase.SearchBasket.LeadCustomer.CustomerEmail <> "" Then
			LeadGuestEmail = BookingBase.SearchBasket.LeadCustomer.CustomerEmail
		End If

		'2.Clear the basket down
		BookingBase.SearchBasket = New BookingBasket(True)

		'3.Update/preserve lead guest
		BookingBase.SearchBasket.LeadCustomer.CustomerEmail = LeadGuestEmail

	End Sub

	Public Shared Sub ClearBookingBasket()
		BookingBase.Basket = New BookingBasket(True)
	End Sub

#End Region

End Class

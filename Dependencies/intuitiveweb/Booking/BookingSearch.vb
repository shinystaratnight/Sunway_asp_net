Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports System.Collections.Specialized
Imports Intuitive
Imports Intuitive.Functions
Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Configuration.ConfigurationManager

Public Class BookingSearch

	<XmlIgnore()>
	Public Params As BookingBase.ParamDef
	<XmlIgnore()>
	Public Markups As New List(Of BookingBase.Markup)
	<XmlIgnore()>
	Public Lookups As Lookups

	Public Sub New(Params As BookingBase.ParamDef, Markups As List(Of BookingBase.Markup), Lookups As Lookups)
		Me.Params = Params
		Me.Markups = Markups
		Me.Lookups = Lookups
		Me.FlightResults = New FlightResultHandler(Me.Params, Me, Me.Markups, Me.Lookups)
	End Sub
	Public Sub New()
	End Sub

#Region "Properties"

	'login details
	Public Property LoginDetails As iVectorConnectInterface.LoginDetails

	'search parameters
	Public Property SearchMode As SearchModes
	Public Property DepartingFromID As Integer 'airportid or airportgroupid+1e6
	Public Property ArrivingAtID As Integer '-geog3id or geog2id or arrivalairportid+1e6
	Public Property GeographyGroupingID As Integer
	Public Property PropertyReferenceID As Integer
	Public Property PropertyReferenceIDs As List(Of Integer)
	Public Property ProductAttributeID As Integer

	Public Property DepartureDate As Date
	Public Property Duration As Integer
	Public Property Rating As Integer
	Public Property MealBasisID As Integer
	Public Property Rooms As Integer
	Public Property RoomGuests As New RoomGuestDef

	Public Property Longitude As Decimal
	Public Property Latitude As Decimal
	Public Property Radius As Integer

	Public Property OneWay As Boolean = False
	Public Property MultiCarrier As Boolean = True
	Public Property FlightSearchMode As FlightSearchModes = FlightSearchModes.FlightSearch

    Public Property ItineraryDetails As FlightItineraryDef = New FlightItineraryDef

	Public Property Direct As Boolean = False

	Public Property MaxResults As Integer = 0

	Public Property WidenSearch As Boolean = True
	Public Property FlightClassID As Integer

	'FlightItinerary
	Public Property FlightSectors As New Generic.List(Of FlightItinerarySearchRequest.FlightSector)

	'transfers
	Public Property TransferSearch As New BookingTransfer.TransferSearch

	'extras
	Public Property ExtraSearch As New BookingExtra.ExtraSearch

	'car hire
	Public Property CarHireSearch As New BookingCarHire.CarHireSearch

	'results
	<XmlIgnore()>
	Public Property PropertyResults As New PropertyResultHandler
	<XmlIgnore()>
	Public Property FlightResults As FlightResultHandler
	'Public Property FlightResults As New FlightResultHandler

	<XmlIgnore()>
	Public Property TransferResults As New BookingTransfer.Results
	<XmlIgnore()>
	Public Property CarHireResults As New BookingCarHire.CarHireResults
	<XmlIgnore()>
	Public Property ExtraResults As New ExtraResultHandler

	<XmlIgnore()>
	Public Property TwoStepExtraResults As New TwoStepExtraResultsHandler

	<XmlIgnore()>
	Public Property FlightCarouselResults As New BookingFlight.FlightCarousel.Results
	<XmlIgnore()>
	Public Property BookingAdjustments As New BookingAdjustment.Results

	'misc
	Public Property PriorityPropertyID As Integer 'not used for searching, used for sorting results
	Public Property HotelArrivalDate As Date 'not used for searching
	Public Property HotelDuration As Integer 'not used for searching
	Public Property SearchGuid As String 'used for tracking

	'email search times
	Public EmailSearchTimes As Boolean
	Public EmailTo As String = ""

	'reporting
	<XmlIgnore()>
	Public Property ProcessTimer As New ProcessTimer

	'helper properties
	Public ReadOnly Property TotalAdults As Integer
		Get
			Dim iAdults As Integer = 0
			For Each RoomGuests As Guest In Me.RoomGuests
				iAdults += RoomGuests.Adults
			Next
			Return iAdults
		End Get
	End Property

	Public ReadOnly Property TotalChildren As Integer
		Get
			Dim iChildren As Integer = 0
			For Each RoomGuests As Guest In Me.RoomGuests
				iChildren += RoomGuests.Children
			Next
			Return iChildren
		End Get
	End Property

	Public ReadOnly Property TotalInfants As Integer
		Get
			Dim iInfants As Integer = 0
			For Each RoomGuests As Guest In Me.RoomGuests
				iInfants += RoomGuests.Infants
			Next
			Return iInfants
		End Get
	End Property

	Public ReadOnly Property AllChildAges As Generic.List(Of Integer)
		Get
			Dim oChildAges As New Generic.List(Of Integer)
			For Each RoomGuests As Guest In Me.RoomGuests
				oChildAges.AddRange(RoomGuests.ChildAges)
			Next
			Return oChildAges
		End Get
	End Property

	Public Property RemoveLowRatedHotels As Boolean = False

#End Region

#Region "class and enum definitions"

	'room guests
	Public Class RoomGuestDef
		Inherits Generic.List(Of Guest)
		Public Sub AddRoom(ByVal Adults As Integer, ByVal Children As Integer, ByVal Infants As Integer,
		   ByVal ParamArray ChildAges() As Integer)
			Me.Add(New Guest(Adults, Children, Infants, ChildAges))
		End Sub
	End Class

    Public Class  FlightItineraryDef

        ''' <summary>
        ''' 'When doing a flight itinerary search denotes the number of legs you search for.
        ''' </summary>
        ''' <value>
        ''' The flight centers you search for as an integer, defaults to 0.
        ''' </value>
        public Readonly Property FlightCenters As Integer
            Get
                Dim count As integer = 0

                If Me.Centers IsNot nothing
                    count = Me.Centers.Count
                End If

                return count
            End Get
        End Property
    
        ''' <summary>
        ''' When doing a flight itinerary search with hotels, used to track which of the flight centers we are currently searching hotels for
        ''' </summary>
        ''' <value>
        ''' The current center you are searching for, as an integer, defaults to 0.
        ''' </value>
        public property CurrentCenter As Integer = 0

        Public property Centers As List(Of Center) = New List(Of Center)

        Public Class Center
            Public Property ArrivalDate As datetime
            Public Property Duration As integer
            public Property Seq As integer

            Public Property ArrivalId As Integer

            Public Property ArrivalType As Geographylevel

            Public Property ArrivalRegion As string

            public enum Geographylevel
                Region
                Resort
            End enum
        End Class

    End Class

	'guest
	Public Class Guest
		Public Property Adults As Integer
		Public Property Children As Integer
		Public Property Infants As Integer
		Public Property ChildAges As New Generic.List(Of Integer)

		Public Sub New(ByVal Adults As Integer, ByVal Children As Integer, ByVal Infants As Integer,
		 ByVal ParamArray ChildAges() As Integer)

			If Children <> ChildAges.Length Then Throw New Exception("Not all children have child ages")
			For Each iChildAge As Integer In ChildAges
				If iChildAge < 2 OrElse iChildAge > 17 Then Throw New Exception("Children must be between 2 and 17")
			Next
			Me.Adults = Adults
			Me.Children = Children
			Me.Infants = Infants
			If ChildAges.Length > 0 Then Me.ChildAges.AddRange(ChildAges)
		End Sub

		Public Sub New()
		End Sub
	End Class

	Public Enum SearchModes
		FlightPlusHotel
		FlightOnly
		HotelOnly
		TransferOnly
		AirportHotel
		CarHireOnly
		ExtraOnly
		Anywhere
	End Enum

	Public Enum FlightSearchModes
		FlightSearch
		FlightItinerary
	End Enum

	Public Enum FlightCarouselModes
		None
		Cache
		Results
		CalendarSearch
	End Enum

	Public Class SearchReturn
		Public OK As Boolean = True
		Public Warning As New Generic.List(Of String)

		Public PropertyResults As New Generic.List(Of ivci.Property.SearchResponse.PropertyResult)
		Public FlightResults As New Generic.List(Of ivci.Flight.SearchResponse.Flight)
		Public FlightCarouselDates As New Generic.List(Of ivci.FlightCarouselResponse.Date)
		Public TransferResults As New Generic.List(Of ivci.Transfer.SearchResponse.Transfer)
		Public ExtraResults As New Generic.List(Of ivci.Extra.SearchResponse.ExtraType)
		Public CarHireResults As New Generic.List(Of ivci.CarHire.SearchResponse.CarHireResult)
		Public ExtraAvailabilityResults As New List(Of ivci.Extra.AvailabilityResponse.ExtraType)
		Public ExtraOptionResult As New List(Of Extra.OptionsResponse.Option)
		Public CarHireDepots As New Generic.List(Of BookingCarHire.CarHireDepot)
		Public BookingAdjustments As New Generic.List(Of ivci.BookingAdjustmentSearchResponse.BookingAdjustmentType)

		Public PropertyCount As Integer
		Public FlightCount As Integer
		Public ExactMatchFlightCount As Integer
		Public FlightCarouselCount As Integer
		Public TransferCount As Integer
		Public ExtraCount As Integer
		Public CarHireCount As Integer
		Public CarHireDepotCount As Integer

		Public HotelArrivalDate As Date
		Public HotelDuration As Integer

		Public RequestInfo As New RequestInfo

		Public DataCollectionInfo As New PropertyResultHandler.LogValues

	End Class

	Public Class RequestInfo
		Public RequestXML As XmlDocument
		Public ResponseXML As XmlDocument
		Public Type As RequestInfoType
		Public RequestTime As Double
		Public NetworkLatency As Double

		Public Function ConvertToDiagnostic() As RequestDiagnostic
			Dim oRequestDiagnostic As New RequestDiagnostic
			oRequestDiagnostic.ApiTime = Me.RequestTime - Me.NetworkLatency
			oRequestDiagnostic.NetworkTime = NetworkLatency
			oRequestDiagnostic.RequestTime = RequestTime
			Return oRequestDiagnostic
		End Function

	End Class

	Public Enum RequestInfoType
		[Default]
		FlightSearch
		PropertySearch
		TransferSearch
		ExtraSearch
		FlightExtraSearch
		BasketPreBook
		BasketBook
		BookingAdjustment
	End Enum

#End Region

#Region "Clear Results"

	Public Sub ClearExtraResults()
		BookingBase.SearchDetails.ExtraResults = New ExtraResultHandler
	End Sub

	Public Sub ClearTransferResults()
		BookingBase.SearchDetails.TransferResults = New BookingTransfer.Results
	End Sub

#End Region

#Region "encode and decode"

	Public Function Encode() As String

		Dim oFields As New Dictionary(Of String, Object)
		oFields.Add("SearchMode", Me.SearchMode.ToString)
		oFields.Add("DepartingFromID", Me.DepartingFromID)
		If Me.GeographyGroupingID > 0 Then
			oFields.Add("ArrivingAtID", (Me.GeographyGroupingID * -1) - 1000000)
		Else
			oFields.Add("ArrivingAtID", Me.ArrivingAtID)
		End If
		oFields.Add("PropertyReferenceID", Me.PropertyReferenceID)
		oFields.Add("DepartureDate", Me.DepartureDate.ToString("dd/MM/yyyy"))
		oFields.Add("Duration", Me.Duration)
		oFields.Add("Rating", Me.Rating)
		oFields.Add("MealBasisID", Me.MealBasisID)
		oFields.Add("Rooms", Me.Rooms)

		oFields.Add("Longitude", Me.Longitude)
		oFields.Add("Latitude", Me.Latitude)
		oFields.Add("Radius", Me.Radius)

		For i As Integer = 1 To Me.RoomGuests.Count

			Dim oGuests As Guest = Me.RoomGuests(i - 1)
			oFields.Add("Adults_" & i.ToString, oGuests.Adults)
			oFields.Add("Children_" & i.ToString, oGuests.Children)
			oFields.Add("Infants_" & i.ToString, oGuests.Infants)

			For j As Integer = 1 To oGuests.ChildAges.Count
				oFields.Add("ChildAge_" & i.ToString & "_" & j.ToString, oGuests.ChildAges(j - 1))
			Next

		Next

		For Each oSector As ivci.FlightItinerarySearchRequest.FlightSector In Me.FlightSectors

			Dim iPos As Integer = oSector.Position
			oFields.Add("FIArrID_" & iPos, oSector.ArrivalAirportID)
			oFields.Add("FIDepFromID_" & iPos, oSector.DepartureAirportID)
			oFields.Add("FlightItineraryDepartureDate_" & iPos, oSector.DepartureDate)

		Next

		'transfers
		If Me.SearchMode = SearchModes.TransferOnly Then
			oFields.Add("PickupType", Me.TransferSearch.DepartureParentType)
			oFields.Add("TransferPickupLocationID", Me.TransferSearch.DepartureParentID)
			oFields.Add("DropOffType", Me.TransferSearch.ArrivalParentType)
			oFields.Add("TransferDropOffLocationID", Me.TransferSearch.ArrivalParentID)

			oFields.Add("PickupDate", Me.TransferSearch.DepartureDate.ToString("dd/MM/yyyy"))
			oFields.Add("ArrivalTime", Me.TransferSearch.DepartureTime)
			oFields.Add("DropOffDate", Me.TransferSearch.ReturnDate.ToString("dd/MM/yyyy"))
			oFields.Add("DropOffTime", Me.TransferSearch.ReturnTime)

			oFields.Add("ReturnTransfer", Not Me.TransferSearch.Oneway)
		End If

		'car hire
		If Me.SearchMode = SearchModes.CarHireOnly Then
			oFields.Add("CarHirePickUpDepotID", Me.CarHireSearch.PickUpDepotID)
			oFields.Add("CarHireDropOffDepotID", Me.CarHireSearch.DropOffDepotID)
			oFields.Add("CarHirePickUpDate", Me.CarHireSearch.PickUpDate.ToString("dd/MM/yyyy"))
			oFields.Add("CarHirePickUpTime", Me.CarHireSearch.PickUpTime)
			oFields.Add("CarHireDropOffDate", Me.CarHireSearch.DropOffDate.ToString("dd/MM/yyyy"))
			oFields.Add("CarHireDropOffTime", Me.CarHireSearch.DropOffTime)
			oFields.Add("CarHireDriverBookingCountryID", Me.CarHireSearch.LeadDriverBookingCountryID)
			oFields.Add("CarHireDriverAge", String.Join("|", Me.CarHireSearch.DriverAges))
			oFields.Add("CarHirePassengers", Me.CarHireSearch.TotalPassengers)
			oFields.Add("CarHireCountryID", BookingBase.Lookups.GetDepotByDepotID(Me.CarHireSearch.PickUpDepotID).GeographyLevel1ID)
		End If

		Return Utility.DictionaryKeyValuePair.Encode(oFields)

	End Function

	Public Sub Decode(ByVal KeyValuePair As String)

		Dim oFields As Dictionary(Of String, Object)
		oFields = Utility.DictionaryKeyValuePair.Decode(KeyValuePair, True)

		Me.SearchMode = SafeEnum(Of SearchModes)(oFields("SearchMode"))
		Me.DepartingFromID = SafeInt(oFields("DepartingFromID"))
		If SafeInt(oFields("ArrivingAtID")) < -1000000 Then
			Me.GeographyGroupingID = (SafeInt(oFields("ArrivingAtID")) * -1) - 1000000
			Me.ArrivingAtID = 0
		Else
			Me.ArrivingAtID = SafeInt(oFields("ArrivingAtID"))
		End If
		If oFields.ContainsKey("PropertyReferenceID") Then Me.PropertyReferenceID = SafeInt(oFields("PropertyReferenceID"))
		If oFields.ContainsKey("acpPriorityPropertyIDHidden") Then Me.PriorityPropertyID = SafeInt(oFields("acpPriorityPropertyIDHidden"))
		Me.DepartureDate = DateFunctions.SafeDate(oFields("DepartureDate"))
		Me.Duration = SafeInt(oFields("Duration"))
		If oFields.ContainsKey("Rating") Then Me.Rating = SafeInt(oFields("Rating"))
		If oFields.ContainsKey("MealBasisID") Then Me.MealBasisID = SafeInt(oFields("MealBasisID"))
		Me.Rooms = SafeInt(oFields("Rooms"))

		If oFields.ContainsKey("Longitude") Then Me.Longitude = SafeDecimal(oFields("Longitude")) Else Me.Longitude = 0D
		If oFields.ContainsKey("Latitude") Then Me.Latitude = SafeDecimal(oFields("Latitude")) Else Me.Longitude = 0D
		If oFields.ContainsKey("Radius") Then Me.Radius = SafeInt(oFields("Radius")) Else Me.Radius = 0

		If oFields.ContainsKey("TripType") Then
			If SafeString(oFields("TripType")).ToLower = "oneway" Then Me.OneWay = True Else Me.OneWay = False
			If SafeString(oFields("TripType")).ToLower = "flightitinerary" Then Me.FlightSearchMode = FlightSearchModes.FlightItinerary
		End If

		If oFields.ContainsKey("HighRatedHotelFilter") Then
			Me.RemoveLowRatedHotels = SafeBoolean(oFields("HighRatedHotelFilter"))
		Else
			Me.RemoveLowRatedHotels = False
		End If

		If oFields.ContainsKey("chkDirect") Then Me.Direct = SafeBoolean(oFields("chkDirect"))

		If oFields.ContainsKey("ProductAttributeID") Then
			Me.ProductAttributeID = Functions.SafeInt(oFields("ProductAttributeID"))
		End If

		If oFields.ContainsKey("FlightClassID") Then Me.FlightClassID = SafeInt(oFields("FlightClassID"))

		For i As Integer = 1 To Me.Rooms
			Dim oGuest As New Guest
			Me.RoomGuests.Add(oGuest)

			oGuest.Adults = SafeInt(oFields("Adults_" & i))
			oGuest.Children = SafeInt(oFields("Children_" & i))
			oGuest.Infants = SafeInt(oFields("Infants_" & i))
			For j As Integer = 1 To oGuest.Children
				oGuest.ChildAges.Add(SafeInt(oFields("ChildAge_" & i & "_" & j)))
			Next

		Next

		If Me.FlightSearchMode = FlightSearchModes.FlightItinerary Then
            If oFields("IsMulti") IsNot "false" Then
                Dim iNumberOfSectors As Integer = SafeInt(oFields("FlightSectors"))
                Dim iDayCount As Integer = 0
                For iCurrentSector As Integer = 1 To iNumberOfSectors

                    
                    Dim oCenter As New FlightItineraryDef.Center

                    Dim departureID As Integer
                    Dim DepartureDate As DateTime
                    Dim ArrivalDate As DateTime
                    If oFields.ContainsValue("FIDepFromID_" & iCurrentSector) Then
                        departureID = SafeInt(oFields("FIDepFromID_" & iCurrentSector))
                        DepartureDate = DateFunctions.SafeDate(oFields("FlightItineraryDepartureDate_" & iCurrentSector))
                        ArrivalDate = DateFunctions.SafeDate(oFields("FlightItineraryDepartureDate_" & iCurrentSector))
                    Else
                        If iCurrentSector = 1 Then
                            departureID = SafeInt(oFields("AirportDepartingFromID"))
                        Else
                            departureID = SafeInt(oFields("FIArrID_" & iCurrentSector - 1))
                            iDayCount += SafeInt(oFields("divFlightItineraryRow_" & iCurrentSector - 1 & "_Nights"))
                        End If
                        ArrivingAtID = SafeInt(oFields("FIArrID_" & iCurrentSector))
                        DepartureDate = DateFunctions.SafeDate(oFields("DepartureDate")).AddDays(iDayCount)
                        ArrivalDate = DateFunctions.SafeDate(oFields("DepartureDate")).AddDays(iDayCount)
                    End If

                    PopulateSector(DepartureDate, ArrivalDate, iCurrentSector, departureID, ArrivingAtID)

                    PopulateCenter(oCenter, SafeInt(oFields("FIArrID_" & iCurrentSector)),
                                             iCurrentSector, DepartureDate, SafeInt(oFields("divFlightItineraryRow_" & iCurrentSector & "_Nights")))
                    Me.ItineraryDetails.Centers.Add(oCenter)
                Next

                If BookingBase.Params.MultiCenterAddReturnToDepartureAirport then
                    iDayCount += SafeInt(oFields($"divFlightItineraryRow_{iNumberOfSectors}_Nights"))
                    AddReturnFlightToDepartureAirport(oFields, iNumberOfSectors, iDayCount)
                End If

                Me.DepartureDate = DepartureDate
            Else
			Dim NumberOfSectors As Integer = SafeInt(oFields("FlightSectors"))

			For i As Integer = 1 To NumberOfSectors

				Dim oSector As New ivci.FlightItinerarySearchRequest.FlightSector

				With oSector
					.ArrivalAirportID = SafeInt(oFields("FIArrID_" & i))
					.DepartureAirportID = SafeInt(oFields("FIDepFromID_" & i))
					.DepartureDate = DateFunctions.SafeDate(oFields("FlightItineraryDepartureDate_" & i))
					.ArrivalDate = DateFunctions.SafeDate(oFields("FlightItineraryDepartureDate_" & i))
					.Position = i
				End With

				Me.FlightSectors.Add(oSector)

			Next

			Me.DepartureDate = DateFunctions.SafeDate(oFields("FlightItineraryDepartureDate_" & 1))

		End If
		End If

		If Me.SearchMode = SearchModes.TransferOnly Then
			Me.TransferSearch.LoginDetails = BookingBase.IVCLoginDetails

			Me.TransferSearch.DepartureParentType = SafeEnum(Of BookingTransfer.TransferSearch.ParentType)(oFields("PickupType"))
			Me.TransferSearch.DepartureParentID = SafeInt(oFields("TransferPickupLocationID"))
			Me.TransferSearch.ArrivalParentType = SafeEnum(Of BookingTransfer.TransferSearch.ParentType)(oFields("DropOffType"))
			Me.TransferSearch.ArrivalParentID = SafeInt(oFields("TransferDropOffLocationID"))

			Me.TransferSearch.DepartureDate = DateFunctions.SafeDate(oFields("PickupDate"))
			Me.TransferSearch.DepartureTime = SafeString(oFields("ArrivalTime"))
			Me.TransferSearch.ReturnDate = DateFunctions.SafeDate(oFields("DropOffDate"))
			Me.TransferSearch.ReturnTime = SafeString(oFields("DropOffTime"))

			Me.TransferSearch.Oneway = Not SafeBoolean(oFields("ReturnTransfer"))

			Me.TransferSearch.Adults = SafeInt(oFields("Adults_1"))
			Me.TransferSearch.Children = SafeInt(oFields("Children_1"))
			Me.TransferSearch.Infants = SafeInt(oFields("Infants_1"))

			For i As Integer = 1 To Me.TransferSearch.Children
				Me.TransferSearch.ChildAges.Add(SafeInt(oFields("ChildAge_1_" & i)))
			Next

		End If

		If Me.SearchMode = SearchModes.CarHireOnly Then

			Me.CarHireSearch.LoginDetails = BookingBase.IVCLoginDetails
			Me.CarHireSearch.CustomerIP = HttpContext.Current.Request.UserHostAddress
			Me.CarHireSearch.PickUpDepotID = SafeInt(oFields("CarHirePickUpDepotID"))
			Me.CarHireSearch.DropOffDepotID = SafeInt(oFields("CarHireDropOffDepotID"))
			Me.CarHireSearch.PickUpDate = DateFunctions.SafeDate(oFields("CarHirePickUpDate"))
			Me.CarHireSearch.PickUpTime = SafeString(oFields("CarHirePickUpTime"))
			Me.CarHireSearch.DropOffDate = DateFunctions.SafeDate(oFields("CarHireDropOffDate"))
			Me.CarHireSearch.DropOffTime = SafeString(oFields("CarHireDropOffTime"))
			Me.CarHireSearch.LeadDriverBookingCountryID = SafeInt(oFields("CarHireDriverBookingCountryID"))
			Me.CarHireSearch.DriverAges = New Generic.List(Of String)(SafeString(oFields("CarHireDriverAge")).Split("|"c)).ConvertAll(Function(str) SafeInt(str))
			Me.CarHireSearch.TotalPassengers = SafeInt(oFields("CarHirePassengers"))

			Me.RoomGuests.Clear()
			Dim oGuest As New Guest
			Me.RoomGuests.Add(oGuest)

			oGuest.Adults = SafeInt(oFields("CarHirePassengers"))
			oGuest.Children = 0
			oGuest.Infants = 0

		End If

	End Sub

	Private Sub AddReturnFlightToDepartureAirport(oFields As Dictionary(Of String,Object), iNumberOfSectors As Integer, iDuration As Integer)
	    Dim dReturnDate as datetime = DateFunctions.SafeDate(oFields("DepartureDate")).AddDays(iDuration)
	    PopulateSector(dReturnDate, dReturnDate, iNumberOfSectors + 1,  SafeInt(oFields("FIArrID_" & iNumberOfSectors)), SafeInt(oFields("AirportDepartingFromID")) + 1000000)
	End Sub

	Private Sub PopulateSector(ByVal dDepartureDate As Date, ByVal dArrivalDate As Date,
                               ByVal iCurrentSector As Integer, ByVal iDepartureID As Integer, ByVal iArrivingAtID As Integer)

	    Dim oSector As New FlightItinerarySearchRequest.FlightSector

        With oSector
            .DepartureDate = dDepartureDate
            .ArrivalDate = dArrivalDate
            .Position = iCurrentSector
        End With

        If iCurrentSector = 1 Then
            If iDepartureID > 1000000 Then
                oSector.DepartureAirportGroupID = iDepartureID
            Else
                oSector.DepartureAirportID = iDepartureID
            End If
        ElseIf iDepartureID > 0 Then
            oSector.DepartureGeoLevel2ID = iDepartureID
        Else
            oSector.DepartureGeoLevel3ID = iDepartureID * -1
        End If

        If iArrivingAtID > 1000000 Then
            oSector.ArrivalAirportID = iArrivingAtID - 1000000
        Else if iArrivingAtID > 0 
            oSector.ArrivalGeoLevel2ID = iArrivingAtID
        Else
            oSector.ArrivalGeoLevel3ID = iArrivingAtID * -1
        End If

	    Me.FlightSectors.Add(oSector)

    End Sub


    Private Sub PopulateCenter(ByRef oCenter As FlightItineraryDef.Center, ByVal iGeographyCode as Integer, ByVal  iCurrentSector As Integer,
                               ByVal dArrivalDate As Date, ByVal iDuration As Integer)
        Dim ArrivalRegion As String
        Dim CurrentSeq As Integer = iCurrentSector

        If iGeographyCode > 0 Then
            ArrivalRegion = Lookups.Locations.Where(
                Function(loc) loc.GeographyLevel2ID = iGeographyCode).FirstOrDefault().GeographyLevel2Name
        Else
            ArrivalRegion = Lookups.Locations.Where(
                Function(loc) loc.GeographyLevel3ID = iGeographyCode * -1).FirstOrDefault().GeographyLevel2Name
        End If

        With oCenter
            .ArrivalRegion = ArrivalRegion
            .ArrivalDate = dArrivalDate
            .ArrivalId = iGeographyCode
            .ArrivalType = IIf(iGeographyCode > 0,
                               FlightItineraryDef.Center.Geographylevel.Region, FlightItineraryDef.Center.Geographylevel.Resort)
            .Duration = iDuration
            .Seq = CurrentSeq
        End With
    End Sub
#End Region

#Region "setup"

	'setup - called on first run
	'either detect a search from the cookie or set defaults
	'or reset if required
	Public Sub Setup(Optional ByVal ResetCookie As Boolean = False, Optional ByVal QueryString As NameValueCollection = Nothing)

		Dim sSearchCookie As String = Me.GetSearchCookie
		If sSearchCookie = "" OrElse ResetCookie Then
			Me.SearchMode = BookingBase.Params.Search_DefaultSearchMode
			Me.DepartureDate = BookingBase.Params.Search_DefaultDate
			Me.Duration = BookingBase.Params.Search_DefaultDuration

			Me.TransferSearch.DepartureTime = "12:00"
			Me.TransferSearch.DepartureDate = BookingBase.Params.Search_DefaultDate
			Me.TransferSearch.ReturnTime = "12:00"
			Me.TransferSearch.ReturnDate = BookingBase.Params.Search_DefaultDate.AddDays(BookingBase.Params.Search_DefaultDuration)

			Me.Rooms = 1
			Me.RoomGuests.Clear()
			Me.RoomGuests.AddRoom(BookingBase.Params.Search_DefaulRoomAdults, 0, 0)
			Me.SetSearchCookie()
		End If

		If QueryString IsNot Nothing AndAlso QueryString.HasKeys Then
			Me.UnloadQueryString(QueryString)
			Me.SetSearchCookie()
		End If

	End Sub

#End Region

#Region "Unload Query String"

	Public Sub UnloadQueryString(ByVal QueryString As NameValueCollection)

		Dim iAdults As Integer = 0
		Dim iChildren As Integer = 0
		Dim sChildAges As String = ""
		Dim oChildAges As New Generic.List(Of Integer)
		Dim iInfants As Integer = 0
		Dim sDeparting As String = ""
		Dim sArriving As String = ""

		For Each sKey As String In QueryString
			Select Case sKey.ToLower
				Case "mode"
					Try
						Me.SearchMode = DirectCast([Enum].Parse(GetType(BookingSearch.SearchModes), QueryString.Get(sKey)), BookingSearch.SearchModes)
					Catch ex As Exception
						Select Case QueryString.Get(sKey).ToLower
							Case "flight"
								Me.SearchMode = SearchModes.FlightOnly
							Case "flightplushotel"
								Me.SearchMode = SearchModes.FlightPlusHotel
							Case "hotel"
								Me.SearchMode = SearchModes.HotelOnly
							Case "transfer"
								Me.SearchMode = SearchModes.TransferOnly
							Case "airporthotel"
								Me.SearchMode = SearchModes.AirportHotel
							Case Else
								Me.SearchMode = BookingBase.Params.Search_DefaultSearchMode
						End Select
					End Try

				Case "dep"
					sDeparting = QueryString.Get(sKey)

				Case "arr"
					sArriving = QueryString.Get(sKey).Replace("/", ",")

				Case "date"
					Try
						Dim sDate As String = QueryString.Get(sKey)
						Me.DepartureDate = Date.ParseExact(sDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat)
					Catch ex As Exception
						Me.DepartureDate = BookingBase.Params.Search_DefaultDate
					End Try

				Case "duration"
					Dim iDuration As Integer = SafeInt(QueryString.Get(sKey))
					If iDuration > 0 Then Me.Duration = iDuration

				Case "adults"
					iAdults = SafeInt(QueryString.Get(sKey))

				Case "children"
					iChildren = SafeInt(QueryString.Get(sKey))

				Case "childages"
					sChildAges = QueryString.Get(sKey)

				Case "infants"
					iInfants = SafeInt(QueryString.Get(sKey))
			End Select

		Next

		If Not String.IsNullOrEmpty(sDeparting) Then
			Me.DepartingFromID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.AirportIATACode, sDeparting)
		End If

		If Not String.IsNullOrEmpty(sArriving) Then
			Me.ArrivingAtID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.AirportIATACode, sArriving)
			If Me.ArrivingAtID > 0 Then
				Me.ArrivingAtID += 1000000
			Else
				Dim aArrivingGeographies As String() = sArriving.Split(","c)

				If aArrivingGeographies.Length > 1 Then
					Dim iResortID As Integer = 0
					iResortID = SafeInt(BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.Resort, SafeString(aArrivingGeographies(1))))

					If iResortID > 0 Then
						Me.ArrivingAtID = iResortID * -1
					Else
						Me.ArrivingAtID = BookingBase.Lookups.GetKeyPairID(Web.Lookups.LookupTypes.Region, SafeString(aArrivingGeographies(0)))
					End If
				Else
					Me.ArrivingAtID = BookingBase.Lookups.GetKeyPairID(Web.Lookups.LookupTypes.Region, SafeString(aArrivingGeographies(0)))
				End If

			End If
		End If

		If iAdults > 0 Then
			iChildren = IIf(iChildren > 0, iChildren, 0)
			iInfants = IIf(iInfants > 0, iInfants, 0)
			Me.Rooms = 1
			Me.RoomGuests.Clear()

			If iChildren > 0 Then
				Dim aChildAges As String() = sChildAges.Split(","c)
				Me.AllChildAges.Clear()
				For x As Integer = 1 To iChildren
					If aChildAges.Length >= x Then
						Dim iAge As Integer = SafeInt(aChildAges(x - 1))
						If iAge > 0 Then
							oChildAges.Add(iAge)
						End If
					End If
				Next
			End If
			Try
				Me.RoomGuests.AddRoom(iAdults, iChildren, iInfants, oChildAges.ToArray)
			Catch ex As Exception

			End Try
		End If

	End Sub

#End Region

#Region "Search"

	Public Function Search() As SearchReturn

		Me.LoginDetails = BookingBase.IVCLoginDetails

		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.BookingSearch, ProcessTimer.MainProcess)

		'save on session
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveSearchDetailsToSession, ProcessTimer.MainProcess)
		BookingBase.SearchDetails = Me
		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveSearchDetailsToSession, ProcessTimer.MainProcess)

		If Not HttpContext.Current Is Nothing AndAlso HttpContext.Current.Request.QueryString.ToString.ToLower.Contains("emailsearchtimes") Then
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetEmailLogsTo, ProcessTimer.MainProcess)
			Try

				Me.EmailSearchTimes = True
				BookingBase.LogAllXML = True
				'can have form function part of query string in format ?emailsearchtimes=email@email.com?formfunction=etc
				'so strip this out if this has happened
				Dim sEmailTo As String = HttpContext.Current.Request.QueryString("emailsearchtimes")
				If sEmailTo.Contains("?") Then sEmailTo = sEmailTo.Split("?"c)(0)
				Me.EmailTo = sEmailTo

			Catch ex As Exception
				'dont fall over just because of logging
			End Try
			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetEmailLogsTo, ProcessTimer.MainProcess)
		End If


		Dim oSearchReturn As New SearchReturn
		Select Case Me.SearchMode
			Case (SearchModes.FlightPlusHotel)
                if(Me.FlightSearchMode = FlightSearchModes.FlightItinerary) Then
                    oSearchReturn = Me.FlightOnlySearch()
                Else
				oSearchReturn = Me.FlightPlusHotelSearch()
                End If
			Case (SearchModes.HotelOnly)
				oSearchReturn = Me.HotelOnlySearch()
			Case (SearchModes.FlightOnly)
				oSearchReturn = Me.FlightOnlySearch()
			Case (SearchModes.TransferOnly)
				oSearchReturn = Me.TransferOnlySearch()
			Case (SearchModes.CarHireOnly)
				oSearchReturn = Me.CareHireOnlySearch()
			Case (SearchModes.Anywhere)
				oSearchReturn = Me.AnywhereSearch()
		End Select

		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.BookingSearch, ProcessTimer.MainProcess)

		'return
		Return oSearchReturn

	End Function

	Private Function FlightPlusHotelSearch() As SearchReturn
		Dim oSearchReturn As New SearchReturn
		Dim bCompleteInTime As Boolean
		Dim oLookups As Lookups = BookingBase.Lookups
		Dim bUseRoomMapping As Boolean = BookingBase.UseRoomMapping

		'List of the search threads, new threads should be added to this list, passed into Task.WaitAll using .ToArray
		Dim oTasks As New Generic.List(Of System.Threading.Tasks.Task)

		'Run the searches in parallel
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightAndHotelSearch, ProcessTimer.MainProcess)

		Dim oPropertySearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingProperty.Search(Me, oLookups, bUseRoomMapping))
		oTasks.Add(oPropertySearch)

		Dim oFlightSearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingFlight.Search(Me, oLookups))
		oTasks.Add(oFlightSearch)

		Dim oBookingAdjustmentSearch As Task(Of SearchReturn) = Nothing
		If Me.Params.SearchBookingAdjustments Then
			oBookingAdjustmentSearch = Task(Of SearchReturn).Factory.StartNew(Function() BookingAdjustment.Search(Me))
			oTasks.Add(oBookingAdjustmentSearch)
		End If

		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightAndHotelSearch, ProcessTimer.MainProcess)

		'Wait for them to complete
		If BookingBase.Params.FlightCarouselMode = FlightCarouselModes.Cache Then
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)

			Dim oFlightCarouselSearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(
					 Function() BookingFlight.FlightCarousel.Search(Me, BookingBase.Params.FlightCarouselDaysEitherSide, oLookups))

			bCompleteInTime = Task.WaitAll(oTasks.ToArray, BookingBase.Params.ServiceTimeout)

			oSearchReturn.FlightCarouselCount = oFlightCarouselSearch.Result.FlightCarouselCount

			If oFlightCarouselSearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oFlightCarouselSearch.Result.Warning)

			If oFlightCarouselSearch.Result.FlightCarouselCount > 0 Then
				BookingFlight.FlightCarousel.Results.Save(oFlightCarouselSearch.Result.FlightCarouselDates)
			End If

			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
		Else
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)
			bCompleteInTime = Task.WaitAll(oTasks.ToArray, BookingBase.Params.ServiceTimeout)
			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)
		End If

		'Set up the return class
		If bCompleteInTime Then
			oSearchReturn.OK = oPropertySearch.Result.OK AndAlso oFlightSearch.Result.OK
			oSearchReturn.PropertyCount = oPropertySearch.Result.PropertyCount
			oSearchReturn.FlightCount = oFlightSearch.Result.FlightCount
			oSearchReturn.ExactMatchFlightCount = oFlightSearch.Result.ExactMatchFlightCount

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oPropertySearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oPropertySearch.Result.Warning)
				If oFlightSearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oFlightSearch.Result.Warning)
				If Not oBookingAdjustmentSearch Is Nothing Then
					If oBookingAdjustmentSearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oBookingAdjustmentSearch.Result.Warning)
				End If
			End If
		Else
			oSearchReturn.OK = False
			oSearchReturn.PropertyCount = 0
			oSearchReturn.FlightCount = 0
			oSearchReturn.ExactMatchFlightCount = 0
			oSearchReturn.Warning.Add("Timeout exceeded")
		End If

		If BookingBase.LogAllXML Then
			WebSupportToolbar.SearchLogger = Nothing
			Dim oList As New List(Of BookingSearch.RequestInfo)
			oList.Add(oPropertySearch.Result.RequestInfo)
			oList.Add(oFlightSearch.Result.RequestInfo)
			WebSupportToolbar.SearchLogger = oList

			If Me.EmailSearchTimes Then
				WebSupportToolbar.EmailSearchLogs(Me.EmailTo)
			End If
		End If

		'Save the results
		If oSearchReturn.OK Then

			'these MUST be set before we call BookingProperty.Results.Save
			'save to search so we know correct values (because of offset days)
			BookingBase.SearchDetails.HotelArrivalDate = oPropertySearch.Result.HotelArrivalDate
			BookingBase.SearchDetails.HotelDuration = oPropertySearch.Result.HotelDuration

			'save flights if we have them
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)
			If oFlightSearch.Result.FlightCount > 0 Then
				BookingBase.SearchDetails.FlightResults.Save(oFlightSearch.Result.FlightResults, True, oFlightSearch.Result.RequestInfo)
			End If
			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)

			'save hotels if we have them
			If oPropertySearch.Result.PropertyCount > 0 Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
				BookingBase.SearchDetails.PropertyResults.Save(oPropertySearch.Result.PropertyResults, True, oPropertySearch.Result.RequestInfo)
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
			End If

			'generate flight carousel if using prices from results
			If BookingBase.Params.FlightCarouselMode = FlightCarouselModes.Results AndAlso oFlightSearch.Result.FlightCount > 0 Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
				BookingFlight.FlightCarousel.GenerateFromFlightResults()
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
			End If

			If BookingBase.Params.FlightCarouselMode = FlightCarouselModes.CalendarSearch Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
				BookingFlight.FlightCarousel.GenerateFromCalendarSearch(Me, BookingBase.Params.FlightCarouselDaysEitherSide)
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
			End If

			If Not oBookingAdjustmentSearch Is Nothing Then
				If oBookingAdjustmentSearch.Result.BookingAdjustments.Count > 0 Then
					BookingAdjustment.Save(oBookingAdjustmentSearch.Result.BookingAdjustments)
				End If
			End If

		End If

		Return oSearchReturn
	End Function

	Private Function HotelOnlySearch() As SearchReturn
		Dim oSearchReturn As New SearchReturn
		Dim bCompleteInTime As Boolean
		Dim oLookups As Lookups = BookingBase.Lookups
		Dim bUseRoomMapping As Boolean = BookingBase.UseRoomMapping

		'perform property search
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartHotelSearch, ProcessTimer.MainProcess)
		Dim oPropertySearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingProperty.Search(Me, oLookups, bUseRoomMapping))
		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartHotelSearch, ProcessTimer.MainProcess)
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)
		bCompleteInTime = oPropertySearch.Wait(BookingBase.Params.ServiceTimeout)
		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)

		'set search return
		If bCompleteInTime Then
			oSearchReturn = oPropertySearch.Result

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oPropertySearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oPropertySearch.Result.Warning)
			End If
		Else
			oSearchReturn.OK = False
			oSearchReturn.PropertyCount = 0
			oSearchReturn.Warning.Add("Timeout exceeded")
		End If

		'Save the results
		If oSearchReturn.OK Then

			'these MUST be set before we call BookingProperty.Results.Save
			'save to search so we know correct values (because of offset days)
			BookingBase.SearchDetails.HotelArrivalDate = oPropertySearch.Result.HotelArrivalDate
			BookingBase.SearchDetails.HotelDuration = oPropertySearch.Result.HotelDuration

			'save hotels if we have them
			If oPropertySearch.Result.PropertyCount > 0 Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
				BookingBase.SearchDetails.PropertyResults.Save(oPropertySearch.Result.PropertyResults, True, oPropertySearch.Result.RequestInfo, oPropertySearch.Result.DataCollectionInfo)
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
			End If

		End If

		If BookingBase.LogAllXML Then
			WebSupportToolbar.SearchLogger = Nothing
			Dim oList As New List(Of BookingSearch.RequestInfo)
			oList.Add(oPropertySearch.Result.RequestInfo)
			WebSupportToolbar.SearchLogger = oList

			If Me.EmailSearchTimes Then
				WebSupportToolbar.EmailSearchLogs(Me.EmailTo)
			End If
		End If

		Return oSearchReturn
	End Function

	Private Function FlightOnlySearch() As SearchReturn
		Dim oSearchReturn As New SearchReturn
		Dim bCompleteInTime As Boolean
		Dim oLookups As Lookups = BookingBase.Lookups

		'List of the search threads, new threads should be added to this list, passed into Task.WaitAll using .ToArray
		Dim oTasks As New Generic.List(Of System.Threading.Tasks.Task)

		'perform flight search
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightSearch, ProcessTimer.MainProcess)

		'Decide if we want to do a FlightItinerarySearch or FlightSearch
		Dim oFlightSearch As Task(Of SearchReturn)
		If Me.FlightSearchMode = FlightSearchModes.FlightItinerary Then
			oFlightSearch = Task(Of SearchReturn).Factory.StartNew(Function() BookingFlight.ItinerarySearch(Me))
		Else
			oFlightSearch = Task(Of SearchReturn).Factory.StartNew(Function() BookingFlight.Search(Me, oLookups))
		End If

		oTasks.Add(oFlightSearch)

		Dim oBookingAdjustmentSearch As Task(Of SearchReturn) = Nothing
		If Me.Params.SearchBookingAdjustments Then
			oBookingAdjustmentSearch = Task(Of SearchReturn).Factory.StartNew(Function() BookingAdjustment.Search(Me))
			oTasks.Add(oBookingAdjustmentSearch)
		End If

		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightSearch, ProcessTimer.MainProcess)
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)
		bCompleteInTime = Task.WaitAll(oTasks.ToArray, BookingBase.Params.ServiceTimeout)
		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)

		'set search return
		If bCompleteInTime Then
			oSearchReturn = oFlightSearch.Result

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oFlightSearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oFlightSearch.Result.Warning)
				If Not oBookingAdjustmentSearch Is Nothing Then
					If oBookingAdjustmentSearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oBookingAdjustmentSearch.Result.Warning)
				End If
			End If
		Else
			oSearchReturn.OK = False
			oSearchReturn.FlightCount = 0
			oSearchReturn.Warning.Add("Timeout exceeded")
		End If

		'Save the results
		If oSearchReturn.OK Then

			'flight results
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)
			If oFlightSearch.Result.FlightCount > 0 Then BookingBase.SearchDetails.FlightResults.Save(oFlightSearch.Result.FlightResults, True, oFlightSearch.Result.RequestInfo)
			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)

			'generate flight carousel if using prices from results
			If BookingBase.Params.FlightCarouselMode = FlightCarouselModes.Results AndAlso oFlightSearch.Result.FlightCount > 0 Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
				BookingFlight.FlightCarousel.GenerateFromFlightResults()
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
			End If

			If BookingBase.Params.FlightCarouselMode = FlightCarouselModes.CalendarSearch Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
				BookingFlight.FlightCarousel.GenerateFromCalendarSearch(Me, BookingBase.Params.FlightCarouselDaysEitherSide)
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightCarousel, ProcessTimer.MainProcess)
			End If

			If Not oBookingAdjustmentSearch Is Nothing Then
				If oBookingAdjustmentSearch.Result.BookingAdjustments.Count > 0 Then
					BookingAdjustment.Save(oBookingAdjustmentSearch.Result.BookingAdjustments)
				End If
			End If

		End If

		If BookingBase.LogAllXML Then
			WebSupportToolbar.SearchLogger = Nothing
			Dim oList As New List(Of BookingSearch.RequestInfo)
			oList.Add(oFlightSearch.Result.RequestInfo)
			WebSupportToolbar.SearchLogger = oList

			If Me.EmailSearchTimes Then
				WebSupportToolbar.EmailSearchLogs(Me.EmailTo)
			End If
		End If

		Return oSearchReturn
	End Function

	Private Function TransferOnlySearch() As SearchReturn

		'perform transfer search
		Dim oSearchReturn As SearchReturn = BookingTransfer.Search(Me.TransferSearch)

		'save the results
		If oSearchReturn.OK Then
			If oSearchReturn.TransferCount > 0 Then
				BookingTransfer.Results.Save(oSearchReturn.TransferResults, Me.TransferSearch, "", "")
			End If
		End If

		'if not ok add warnings
		If Not oSearchReturn.OK Then
			If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
		End If

		'Set Search cookie
		BookingBase.SearchDetails.SetSearchCookie()

		Return oSearchReturn
	End Function

	Private Function CareHireOnlySearch() As SearchReturn
		Dim oSearchReturn As SearchReturn = BookingCarHire.Search(Me.CarHireSearch)

		If oSearchReturn.OK Then
			If oSearchReturn.CarHireCount > 0 Then
				BookingCarHire.CarHireResults.Save(oSearchReturn.CarHireResults)
			End If
		End If

		If Not oSearchReturn.OK Then
			If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
		End If

		'Set Search cookie
		BookingBase.SearchDetails.SetSearchCookie()

		Return oSearchReturn
	End Function

	Private Function AnywhereSearch() As SearchReturn
		Dim oSearchReturn As New SearchReturn
		Dim bCompleteInTime As Boolean
		Dim oLookups As Lookups = BookingBase.Lookups
		Dim bUseRoomMapping As Boolean = BookingBase.UseRoomMapping

		'List of the search threads, new threads should be added to this list, passed into Task.WaitAll using .ToArray
		Dim oTasks As New List(Of Task)
		Dim oFlightTasks As New List(Of Task(Of SearchReturn))

		'Run the searches in parallel
		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightAndHotelSearch, ProcessTimer.MainProcess)

		'Get Property Reference IDs linked to product attribute
		Me.PropertyReferenceIDs = New List(Of Integer)
		Dim oProperties As List(Of Lookups.PropertyReference) = Me.GetPropertiesFromProductAttribute()
		For Each oProperty As Lookups.PropertyReference In oProperties
			Me.PropertyReferenceIDs.Add(oProperty.PropertyReferenceID)
		Next

		Dim oPropertySearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingProperty.Search(Me, oLookups, bUseRoomMapping))
		oTasks.Add(oPropertySearch)


		'Get unique airports linked to resorts of the properties and search for flights to each of those airports
		Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsFromPropertyList(oProperties)

		For Each iAirportID As Integer In oArrivalAirportIDs

			Dim oFlightSearchDetails As New BookingSearch
			With oFlightSearchDetails
				.LoginDetails = Me.LoginDetails
				.DepartingFromID = Me.DepartingFromID
				.ArrivingAtID = iAirportID + 1000000
				.DepartureDate = Me.DepartureDate
				.Duration = Me.Duration
				.RoomGuests = Me.RoomGuests
				.Params = Me.Params
				.Lookups = Me.Lookups
				.MaxResults = BookingBase.Params.AnywhereSearchMaxFlightsPerRequest
				.WidenSearch = False
			End With

			Dim oFlightSearch As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingFlight.Search(oFlightSearchDetails, oLookups))
			oTasks.Add(oFlightSearch)
			oFlightTasks.Add(oFlightSearch)
		Next

		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.StartFlightAndHotelSearch, ProcessTimer.MainProcess)

		Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)
		bCompleteInTime = Task.WaitAll(oTasks.ToArray, BookingBase.Params.ServiceTimeout)
		Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.WaitForThreads, ProcessTimer.MainProcess)


		'Set up the return class
		Dim iFlightCount As Integer = 0
		Dim oFlightResults As New List(Of ivci.Flight.SearchResponse.Flight)

		If bCompleteInTime Then
			Dim bFlightReturnOk As Boolean = False
			Dim iExactMatchFlightCount As Integer = 0
			Dim oFlightWarnings As New List(Of String)

			For Each oTask As Task(Of SearchReturn) In oFlightTasks
				iFlightCount += oTask.Result.FlightCount
				iExactMatchFlightCount += oTask.Result.ExactMatchFlightCount

				oFlightResults.AddRange(oTask.Result.FlightResults)

				If Not bFlightReturnOk AndAlso oTask.Result.OK Then
					bFlightReturnOk = True
				End If

				If oTask.Result.Warning.Count > 0 Then
					oFlightWarnings.AddRange(oTask.Result.Warning)
				End If
			Next

			oSearchReturn.OK = oPropertySearch.Result.OK AndAlso bFlightReturnOk
			oSearchReturn.PropertyCount = oPropertySearch.Result.PropertyCount
			oSearchReturn.FlightCount = iFlightCount
			oSearchReturn.ExactMatchFlightCount = iExactMatchFlightCount

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oPropertySearch.Result.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oPropertySearch.Result.Warning)
				If oFlightWarnings.Count > 0 Then oSearchReturn.Warning.AddRange(oFlightWarnings)
			End If
		Else
			oSearchReturn.OK = False
			oSearchReturn.PropertyCount = 0
			oSearchReturn.FlightCount = 0
			oSearchReturn.ExactMatchFlightCount = 0
			oSearchReturn.Warning.Add("Timeout exceeded")
		End If

		'Save the results
		If oSearchReturn.OK Then

			'these MUST be set before we call BookingProperty.Results.Save
			'save to search so we know correct values (because of offset days)
			BookingBase.SearchDetails.HotelArrivalDate = oPropertySearch.Result.HotelArrivalDate
			BookingBase.SearchDetails.HotelDuration = oPropertySearch.Result.HotelDuration

			'save flights if we have them
			Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)
			If iFlightCount > 0 Then
				BookingBase.SearchDetails.FlightResults.Save(oFlightResults, True)
			End If
			Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveFlights, ProcessTimer.MainProcess)

			'save hotels if we have them
			If oPropertySearch.Result.PropertyCount > 0 Then
				Me.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
				BookingBase.SearchDetails.PropertyResults.Save(oPropertySearch.Result.PropertyResults, True, oPropertySearch.Result.RequestInfo)
				Me.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SaveHotels, ProcessTimer.MainProcess)
			End If

		End If

		Return oSearchReturn
	End Function

	Private Function GetPropertiesFromProductAttribute() As List(Of Lookups.PropertyReference)
		Dim oPropertiesXML As XmlDocument = Utility.BigCXML("AnywhereSearchProperties", Me.ProductAttributeID, 600)
		Dim oPropertyReferences As List(Of Lookups.PropertyReference) = Utility.XMLToGenericList(Of Lookups.PropertyReference)(oPropertiesXML)
		Return oPropertyReferences
	End Function

	Private Function GetArrivalAirportIDsFromPropertyList(ByVal properties As List(Of Lookups.PropertyReference)) As List(Of Integer)
		Dim oAirportIDs As New List(Of Integer)

		Dim oValidAirportsXML As XmlDocument = Utility.BigCXML("AnywhereSearchAirports", Me.ProductAttributeID, 600)
		Dim oValidAirports As List(Of Lookups.Airport) = Utility.XMLToGenericList(Of Lookups.Airport)(oValidAirportsXML)
		Dim oValidAirportIDs As List(Of Integer) = oValidAirports.Select(Function(airport) airport.AirportID).ToList()

		For Each oProperty As Lookups.PropertyReference In properties
			Dim oPropertyAirportIDs As List(Of Integer) = Lookups.GetGeographyLevel3AirportIDs(oProperty.GeographyLevel3ID)
			For Each iAirportID As Integer In oPropertyAirportIDs
				If Not oAirportIDs.Contains(iAirportID) AndAlso oValidAirportIDs.Contains(iAirportID) Then
					oAirportIDs.Add(iAirportID)
				End If
			Next
		Next
		Return oAirportIDs
	End Function

#End Region

#Region "Transfer Search From Basket (Flight & Hotel)"

	Public Function GetTransferSearchFromBasket() As BookingTransfer.TransferSearch

		Dim oTransferSearch As New BookingTransfer.TransferSearch

		'check if we have basket hotel and flight
		If BookingBase.Basket.BasketProperties.Count = 0 OrElse BookingBase.Basket.BasketFlights.Count = 0 Then
			oTransferSearch.Warning.Add("No hotel or flight in basket")
			Return oTransferSearch
		End If

		Try

			Dim oBasketFlightOption As BookingFlight.BasketFlight.FlightOption = BookingBase.Basket.BasketFlights(0).Flight

			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
				.DepartureParentID = oBasketFlightOption.ArrivalAirportID

				.ArrivalParentType = BookingTransfer.TransferSearch.ParentType.Property
				.ArrivalParentID = BookingBase.Basket.BasketProperties(0).RoomOptions(0).PropertyReferenceID

				.DepartureDate = oBasketFlightOption.OutboundDepartureDate
				.DepartureTime = oBasketFlightOption.OutboundDepartureTime

				.ReturnDate = oBasketFlightOption.ReturnArrivalDate
				.ReturnTime = oBasketFlightOption.ReturnArrivalTime

				'pax
				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

			'return Transfer Search
			Return oTransferSearch

		Catch ex As Exception
			oTransferSearch.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "GetTransferSearchException", ex.ToString)
		End Try

		Return oTransferSearch

	End Function

    public Sub PopulateTransferSearchFlightDetailsFromBasket(ByVal oTransferSearch As BookingTransfer.TransferSearch)
        
        Dim iArrivalAirportId As Integer
        Dim dDepartureDate As DateTime
        Dim sDepartureTime As string
        Dim dReturnDate As DateTime
        Dim sReturnTime As string

        Dim basketFlight as BookingFlight.BasketFlight = BookingBase.SearchBasket.BasketFlights.Last()

        If Bookingbase.SearchDetails.FlightSearchMode = FlightSearchModes.FlightSearch Then

            iArrivalAirportId = basketFlight.Flight.ArrivalAirportID
            dDepartureDate = basketFlight.Flight.OutboundArrivalDate
            sDepartureTime = basketFlight.Flight.OutboundArrivalTime
            dReturnDate = basketFlight.Flight.ReturnDepartureDate
            sReturnTime = basketFlight.Flight.ReturnDepartureTime

        Else If Bookingbase.SearchDetails.FlightSearchMode = FlightSearchModes.FlightItinerary
     
            If(basketFlight.CurrentCentersArrivalAndDepartureMatch()) Then
                iArrivalAirportId = basketFlight.CurrentCenterLastArrivalSector.ArrivalAirportID
                dDepartureDate = basketFlight.CurrentCenterLastArrivalSector.ArrivalDate
                sDepartureTime = basketFlight.CurrentCenterLastArrivalSector.ArrivalTime

                dReturnDate = basketFlight.NextCenterFirstDepartureSector.DepartureDate
                sReturnTime =basketFlight.NextCenterFirstDepartureSector.DepartureTime
            else
                oTransferSearch.Warning.Add("Transfers are only available when the departure an arrival airports are the same")
            End If

        End If
        
        with oTransferSearch
            .DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
            .DepartureParentID = iArrivalAirportId
            .DepartureDate = dDepartureDate
            .DepartureTime = sDepartureTime
            .ReturnDate = dReturnDate
            .ReturnTime = sReturnTime
        End With

    End Sub

	Public Function TransferSearchFromBasket() As SearchReturn

		Dim oSearchReturn As New SearchReturn

		'check if we have basket hotel and flight
		If BookingBase.SearchBasket.BasketProperties.Count = 0 OrElse BookingBase.SearchBasket.BasketFlights.Count = 0 Then
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add("No hotel or flight in basket")
			Return oSearchReturn
		End If

		Try

			Dim oTransferSearch As New BookingTransfer.TransferSearch

			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.ArrivalParentType = BookingTransfer.TransferSearch.ParentType.Property
				.ArrivalParentID = BookingBase.SearchBasket.BasketProperties.Last().RoomOptions(0).PropertyReferenceID

				'pax
				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

            PopulateTransferSearchFlightDetailsFromBasket(oTransferSearch)
			
            If(oTransferSearch.Warning.Any()) then
                oSearchReturn.OK = False
                oSearchReturn.Warning.AddRange(oTransferSearch.Warning)
            Else 
			'perform search
			oSearchReturn = BookingTransfer.Search(oTransferSearch)
            End If
			

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.TransferCount > 0 Then
					Dim sOutboundFlightCode As String = BookingBase.SearchBasket.BasketFlights.Last().Flight.OutboundFlightCode
					Dim sReturnFlightCode As String = BookingBase.SearchBasket.BasketFlights.Last().Flight.ReturnFlightCode
					BookingTransfer.Results.Save(oSearchReturn.TransferResults, oTransferSearch, sOutboundFlightCode, sReturnFlightCode)
				End If

			End If

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

#End Region

#Region "Transfer Search From Hotel"

	Public Function TransferSearchFromHotel(ByVal AirportID As Integer, ByVal DepartureTime As String, ByVal ReturnTime As String,
	 ByVal OutboundFlightCode As String, ByVal ReturnFlightCode As String) As SearchReturn

		Dim oSearchReturn As New SearchReturn

		'check if we have basket hotel
		If BookingBase.SearchBasket.BasketProperties.Count = 0 Then
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add("No hotel in basket")
			Return oSearchReturn
		End If

		Try

			Dim oTransferSearch As New BookingTransfer.TransferSearch
			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
				.DepartureParentID = AirportID

				.ArrivalParentType = BookingTransfer.TransferSearch.ParentType.Property
				.ArrivalParentID = BookingBase.SearchBasket.BasketProperties(0).RoomOptions(0).PropertyReferenceID

				.DepartureDate = BookingBase.SearchDetails.DepartureDate
				.DepartureTime = DepartureTime

				.ReturnDate = BookingBase.SearchDetails.DepartureDate.AddDays(BookingBase.SearchDetails.Duration)
				.ReturnTime = ReturnTime

				'pax
				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

			'perform search
			oSearchReturn = BookingTransfer.Search(oTransferSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.TransferCount > 0 Then
					BookingTransfer.Results.Save(oSearchReturn.TransferResults, oTransferSearch, OutboundFlightCode, ReturnFlightCode)
				End If

			End If

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

#End Region

#Region "Transfer Search from Extra Location"

	Public Function TransferSearchFromExtraLocation(ByVal AirportID As Integer, ByVal DepartureTime As String, ByVal ReturnTime As String,
	 ByVal OutboundFlightCode As String, ByVal ReturnFlightCode As String, ByVal LocationType As BookingTransfer.TransferSearch.ParentType _
	 , ByVal ParentID As Integer) As SearchReturn

		Dim oSearchReturn As New SearchReturn

		'check if we have basket extra
		If BookingBase.SearchBasket.BasketExtras.Count = 0 Then
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add("No extra in basket")
			Return oSearchReturn
		End If

		Try

			Dim oTransferSearch As New BookingTransfer.TransferSearch
			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
				.DepartureParentID = AirportID

				.ArrivalParentType = LocationType
				.ArrivalParentID = ParentID

				.DepartureDate = BookingBase.SearchDetails.DepartureDate
				.DepartureTime = DepartureTime

				.ReturnDate = BookingBase.SearchDetails.DepartureDate.AddDays(BookingBase.SearchDetails.Duration)
				.ReturnTime = ReturnTime

				'pax
				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

			'perform search
			oSearchReturn = BookingTransfer.Search(oTransferSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.TransferCount > 0 Then
					BookingTransfer.Results.Save(oSearchReturn.TransferResults, oTransferSearch, OutboundFlightCode, ReturnFlightCode)
				End If

			End If

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

#End Region

#Region "Transfer Search From Flight"

	Public Function TransferSearchFromFlight(ByVal PropertyReferenceID As Integer, ByVal ReturnDepartureTime As String) As SearchReturn

		Dim oSearchReturn As New SearchReturn

		'check if we have basket hotel
		'If BookingBase.SearchBasket.BasketProperties.Count = 0 Then
		'	oSearchReturn.OK = False
		'	oSearchReturn.Warning.Add("No flight in basket")
		'	Return oSearchReturn
		'End If

		Try

			Dim oTransferSearch As New BookingTransfer.TransferSearch
			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
				.DepartureParentID = BookingBase.SearchBasket.BasketFlights.Last.Flight.ArrivalAirportID

				.ArrivalParentType = BookingTransfer.TransferSearch.ParentType.Property
				.ArrivalParentID = PropertyReferenceID

				.DepartureDate = BookingBase.SearchBasket.BasketFlights.Last.Flight.OutboundArrivalDate
				.DepartureTime = BookingBase.SearchBasket.BasketFlights.Last.Flight.OutboundArrivalTime

				.ReturnDate = BookingBase.SearchBasket.BasketFlights.Last.Flight.ReturnDepartureDate
				.ReturnTime = ReturnDepartureTime

				'pax
				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

			'perform search
			oSearchReturn = BookingTransfer.Search(oTransferSearch)

			'get fliht codes
			Dim sOutboundFlightCode As String = BookingBase.SearchBasket.BasketFlights.Last.Flight.OutboundFlightCode

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.TransferCount > 0 Then
					BookingTransfer.Results.Save(oSearchReturn.TransferResults, oTransferSearch, sOutboundFlightCode, "")
				End If
			End If

			'if not ok add warnings
			If Not oSearchReturn.OK Then
				If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

#End Region

#Region "Transfer Search From Hotel Results"

	Public Function TransferSearchFromHotelResults() As SearchReturn

		'get unique resorts from current results
		Dim aResorts As IEnumerable(Of Integer) = From oIVCPropertyResult As ivci.Property.SearchResponse.PropertyResult In BookingBase.SearchDetails.PropertyResults.iVectorConnectResults Select oIVCPropertyResult.GeographyLevel3ID Distinct

		'loop through each resort and create a transfer search request
		Dim aTasks As New Generic.List(Of Task)
		For Each iResort As Integer In aResorts

			'if resort does not have a selected flight, skip to next resort
			If Not Me.PropertyResults.FlightDictionary.ContainsKey(iResort) Then
				Continue For
			End If

			'get selected flight for the resort
			Dim oFlight As New FlightResultHandler.Flight
			oFlight = Me.PropertyResults.FlightDictionary(iResort)

			'build ivc request
			Dim oiVectorConnectSearchRequest As New ivci.Transfer.SearchRequest
			Dim oTransferSearch As New BookingTransfer.TransferSearch
			With oTransferSearch
				.LoginDetails = BookingBase.IVCLoginDetails

				.DepartureParentType = BookingTransfer.TransferSearch.ParentType.Airport
				.DepartureParentID = oFlight.ArrivalAirportID

				.ArrivalParentType = BookingTransfer.TransferSearch.ParentType.Resort
				.ArrivalParentID = iResort

				.DepartureDate = oFlight.OutboundArrivalDate
				.DepartureTime = oFlight.OutboundArrivalTime

				.ReturnDate = oFlight.ReturnDepartureDate
				.ReturnTime = oFlight.ReturnDepartureTime

				'pax
				.Adults = Me.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = Me.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = Me.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In Me.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

			End With

			'add to tasks
			Dim oTransferSearchTask As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingTransfer.Search(oTransferSearch))
			aTasks.Add(oTransferSearchTask)

			'save on session so we have dates when saving results
			Me.TransferSearch = oTransferSearch

		Next

		'perform search
		Task.WaitAll(aTasks.ToArray)

		'loop through each result and add to search return
		Dim oSearchReturn As New SearchReturn
		For Each oTask As Task(Of SearchReturn) In aTasks
			oSearchReturn.TransferResults.AddRange(oTask.Result.TransferResults)
			oSearchReturn.TransferCount += oTask.Result.TransferCount
		Next

		'save results
		If oSearchReturn.TransferResults.Count > 0 Then
			BookingTransfer.Results.Save(oSearchReturn.TransferResults, Me.TransferSearch, "", "")
		End If

		'return
		Return oSearchReturn

	End Function

#End Region

#Region "Car Hire Search From Results"

	Public Function CarHireSearchFromResults() As SearchReturn

		''get distinct selected flights
		Dim oFlights As New List(Of FlightResultHandler.Flight)

		'Don't add the same flight twice
		For Each oFlight As FlightResultHandler.Flight In Me.PropertyResults.FlightDictionary.Values.ToList()
			If Not oFlights.Contains(oFlight) Then
				oFlights.Add(oFlight)
			End If
		Next

		'loop through each airport and create a car hire search request
		Dim aTasks As New Generic.List(Of Task)
		For Each oFlight As FlightResultHandler.Flight In oFlights

			Dim oCarHireSearch As New BookingCarHire.CarHireSearch

			With oCarHireSearch

				'login details
				.LoginDetails = BookingBase.IVCLoginDetails

				'get our depots
				Dim oDepots As Generic.List(Of Lookups.CarHireDepot) = BookingBase.Lookups.GetDepotsByAirportID(oFlight.ArrivalAirportID)

				'if no depots skip to next flight
				If oDepots.Count = 0 Then Continue For

				'Pick up and Drop off data
				.PickUpDepotID = oDepots.First.CarHireDepotID
				.PickUpDate = oFlight.OutboundArrivalDate
				.PickUpTime = oFlight.OutboundArrivalTime

				.DropOffDepotID = oDepots.First.CarHireDepotID
				.DropOffDate = oFlight.ReturnDepartureDate
				.DropOffTime = oFlight.ReturnDepartureTime

				.TotalPassengers = Me.TotalAdults + Me.TotalChildren + Me.TotalInfants

				'required for connect validation
				.CustomerIP = HttpContext.Current.Request.UserHostAddress
				.DriverAges.Add(30)
				.LeadDriverBookingCountryID = BookingBase.Params.SellingGeographyLevel1ID

			End With

			'add to tasks
			Dim oCarHireSearchTask As Task(Of SearchReturn) = Task(Of SearchReturn).Factory.StartNew(Function() BookingCarHire.Search(oCarHireSearch))
			aTasks.Add(oCarHireSearchTask)

		Next

		'perform search
		Task.WaitAll(aTasks.ToArray)

		'loop through each result and add to search return
		Dim oSearchReturn As New SearchReturn
		For Each oTask As Task(Of SearchReturn) In aTasks
			oSearchReturn.CarHireResults.AddRange(oTask.Result.CarHireResults)
			oSearchReturn.CarHireCount += oTask.Result.CarHireCount
		Next

		'save results
		If oSearchReturn.CarHireResults.Count > 0 Then
			BookingCarHire.CarHireResults.Save(oSearchReturn.CarHireResults)
		End If

		'return
		Return oSearchReturn

	End Function

#End Region

#Region "Extra Search From Basket (Flight & Hotel)"

	Public Function GetExtraSearchFromBasket(Optional ByVal sSearchDate As Date = Nothing) As BookingExtra.ExtraSearch

		If BookingBase.Basket.ResetBasketOnSearch Then
			BookingBase.Basket = New BookingBasket
		End If

		Dim oExtraSearch As New BookingExtra.ExtraSearch

		'check if we have basket hotel
		If (BookingBase.SearchBasket.BasketProperties.Count = 0 AndAlso
			BookingBase.SearchBasket.BasketFlights.Count = 0 AndAlso
			BookingBase.SearchBasket.BasketExtras.Count = 0) Then

			oExtraSearch.Warning.Add("No hotel, flight or extras in basket")
			Return oExtraSearch

		End If

		Try

			Dim oBasketFlightOption As BookingFlight.BasketFlight.FlightOption = Nothing
			Dim oBasketProperty As BookingProperty.BasketProperty = Nothing
			Dim oBasketExtra As BookingExtra.BasketExtra = Nothing

			If BookingBase.SearchBasket.BasketProperties.Count <> 0 Then
				oBasketProperty = BookingBase.SearchBasket.BasketProperties.Last()
			End If

			If BookingBase.SearchBasket.BasketFlights.Count <> 0 Then
				oBasketFlightOption = BookingBase.SearchBasket.BasketFlights(0).Flight
			End If

			If BookingBase.SearchBasket.BasketExtras.Count <> 0 Then
				oBasketExtra = BookingBase.SearchBasket.BasketExtras(0)
			End If

			'create search
			With oExtraSearch

				.LoginDetails = BookingBase.IVCLoginDetails

				If Not oBasketFlightOption Is Nothing AndAlso BookingBase.SearchDetails.FlightSearchMode = FlightSearchModes.FlightSearch Then
					.DepartureAirportID = oBasketFlightOption.DepartureAirportID
					.ArrivalAirportID = oBasketFlightOption.ArrivalAirportID
					.DepartureTime = oBasketFlightOption.OutboundDepartureTime
					.ReturnTime = oBasketFlightOption.ReturnArrivalTime
				End If

				If Not oBasketProperty Is Nothing Then

					'Get the location using the basket property
					Dim oLocation As Lookups.Location = BookingBase.Lookups.GetLocationFromResort(oBasketProperty.RoomOptions(0).GeographyLevel3ID)

					.GeographyLevel3ID = oLocation.GeographyLevel3ID
					.GeographyLevel2ID = oLocation.GeographyLevel2ID
					.GeographyLevel1ID = oLocation.GeographyLevel1ID
				End If

				If Not oBasketExtra Is Nothing Then
					Dim oLocation As ivci.Extra.ExtraLocation = oBasketExtra.ExtraLocations.Where(Function(o) o.ExtraLocationType = "Resort").FirstOrDefault
					If Not oLocation Is Nothing Then
						'Get the location using the basket property
						Dim oLocations As Lookups.Location = BookingBase.Lookups.GetLocationFromResort(oLocation.ExtraLocationID)
						.GeographyLevel3ID = oLocations.GeographyLevel3ID
						.GeographyLevel2ID = oLocations.GeographyLevel2ID
						.GeographyLevel1ID = oLocations.GeographyLevel1ID
					Else
						oExtraSearch.Warning.Add("No Resort Location Type for that Extra")
					End If
				End If

				'If we want to search for one date at a time override
				If Not sSearchDate = Nothing Then
					If Not oBasketFlightOption Is Nothing Then
						.DepartureDate = sSearchDate
						.DepartureTime = oBasketFlightOption.OutboundDepartureTime
						.ReturnDate = sSearchDate
						.ReturnTime = oBasketFlightOption.ReturnArrivalTime
					ElseIf Not oBasketProperty Is Nothing Then
						.DepartureDate = sSearchDate
						.DepartureTime = "10:00"
						.ReturnDate = sSearchDate
						.ReturnTime = "10:00"
					ElseIf Not oBasketExtra Is Nothing Then
						.DepartureDate = sSearchDate
						.DepartureTime = "10:00"
						.ReturnDate = sSearchDate
						.ReturnTime = "10:00"
					End If
				Else
					If Not oBasketFlightOption Is Nothing Then
						.DepartureDate = oBasketFlightOption.OutboundDepartureDate
						.DepartureTime = oBasketFlightOption.OutboundDepartureTime
						.ReturnDate = oBasketFlightOption.ReturnArrivalDate
						.ReturnTime = oBasketFlightOption.ReturnArrivalTime
					ElseIf Not oBasketProperty Is Nothing Then
						.DepartureDate = BookingBase.SearchDetails.DepartureDate
						.DepartureTime = "10:00"
						.ReturnDate = BookingBase.SearchDetails.DepartureDate.AddDays(BookingBase.SearchDetails.Duration)
						.ReturnTime = "10:00"
					ElseIf Not oBasketExtra Is Nothing Then
						.DepartureDate = BookingBase.SearchDetails.DepartureDate
						.DepartureTime = "10:00"
						.ReturnDate = BookingBase.SearchDetails.DepartureDate.AddDays(BookingBase.SearchDetails.Duration)
						.ReturnTime = "10:00"
					End If
				End If

				.Adults = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.Children = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.Infants = BookingBase.SearchDetails.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests
					.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

				'extras based off a percentage need the price
				.BookingPrice = BookingBase.SearchBasket.TotalPrice + BookingBase.SearchBasket.TotalMarkup

			End With

			'return extra search
			Return oExtraSearch

		Catch ex As Exception
			oExtraSearch.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/ExtraSearch", "GetExtraSearchException", ex.ToString)
		End Try

		Return oExtraSearch

	End Function

	Public Function ExtraSearchFromBasket(ByVal Identifier As String, Optional ByVal ExtraID As Integer = 0,
										  Optional ByVal ExtraGroupID As Integer = 0,
										  Optional ByVal ExtraTypeIDs As Generic.List(Of Integer) = Nothing,
										  Optional ByVal sSearchDate As Date = Nothing,
										  Optional ByVal bAppend As Boolean = False) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn

		Dim oExtraSearch As BookingExtra.ExtraSearch = BookingBase.SearchDetails.GetExtraSearchFromBasket(sSearchDate)

		If ExtraID > 0 Then oExtraSearch.ExtraID = ExtraID
		If ExtraGroupID > 0 Then oExtraSearch.ExtraGroupID = ExtraGroupID
		If ExtraTypeIDs IsNot Nothing Then oExtraSearch.ExtraTypeIDs.AddRange(ExtraTypeIDs)

		If oExtraSearch.Warning.Count = 0 Then

			Dim oLoginDetails As iVectorConnectInterface.LoginDetails = BookingBase.IVCLoginDetails
			oSearchReturn = BookingExtra.Search(oExtraSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.ExtraCount > 0 Then BookingBase.SearchDetails.ExtraResults.Save(Identifier, oSearchReturn.ExtraResults, True, oExtraSearch)
			End If
		Else
			oSearchReturn.Warning.AddRange(oExtraSearch.Warning)
		End If

		Return oSearchReturn

	End Function

	Public Function ExtraSearchFromObject(ByVal Identifier As String, oExtraSearch As BookingExtra.ExtraSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn

		If oExtraSearch.Warning.Count = 0 Then

			Dim oLoginDetails As iVectorConnectInterface.LoginDetails = BookingBase.IVCLoginDetails
			oSearchReturn = BookingExtra.Search(oExtraSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.ExtraCount > 0 Then BookingBase.SearchDetails.ExtraResults.Save(Identifier, oSearchReturn.ExtraResults, False, oExtraSearch)
			End If
		Else
			oSearchReturn.Warning.AddRange(oExtraSearch.Warning)
		End If

		Return oSearchReturn

	End Function

	Public Function ExtraAvailabilitySearchFromObject(ByVal Identifier As String, oExtraSearch As BookingExtra.ExtraSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn

		If oExtraSearch.Warning.Count = 0 Then

			Dim oLoginDetails As iVectorConnectInterface.LoginDetails = BookingBase.IVCLoginDetails
			oSearchReturn = BookingExtra.AvailabilitySearch(oExtraSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.ExtraCount > 0 Then BookingBase.SearchDetails.TwoStepExtraResults.SaveAvailability(Identifier, oSearchReturn.ExtraAvailabilityResults, False, oExtraSearch)
			End If
		Else
			oSearchReturn.Warning.AddRange(oExtraSearch.Warning)
		End If

		Return oSearchReturn

	End Function

	Public Function ExtraOptionSearchFromObject(ByVal Identifier As String, oExtraSearch As BookingExtra.ExtraSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn

		If oExtraSearch.Warning.Count = 0 Then

			Dim oLoginDetails As iVectorConnectInterface.LoginDetails = BookingBase.IVCLoginDetails
			oSearchReturn = BookingExtra.OptionSearch(oExtraSearch)

			'Save the results
			If oSearchReturn.OK Then
				If oSearchReturn.ExtraCount > 0 Then BookingBase.SearchDetails.TwoStepExtraResults.SaveOption(Identifier, oSearchReturn.ExtraOptionResult)
			End If
		Else
			oSearchReturn.Warning.AddRange(oExtraSearch.Warning)
		End If

		Return oSearchReturn

	End Function


	Public Function GetExtraSearchFromParam(ByVal ResortID As Integer, ByVal DepartureDate As Date, ReturnDate As Date, ByVal Adults As Integer, ByVal Children As Integer, ByVal Infants As Integer, Optional ByVal ExtraID As Integer = 0, Optional ByVal ExtraGroupID As Integer = 0, Optional ByVal ExtraTypeIDs As Generic.List(Of Integer) = Nothing, Optional ByVal ChildAges As Generic.List(Of Integer) = Nothing, Optional ByVal AdultAges As Generic.List(Of Integer) = Nothing) As BookingExtra.ExtraSearch

		Dim oExtraSearch As New BookingExtra.ExtraSearch

		Try

			'create search
			With oExtraSearch

				.LoginDetails = BookingBase.IVCLoginDetails

				'Get the location using the basket property
				Dim oLocation As Lookups.Location = BookingBase.Lookups.GetLocationFromResort(ResortID)

				'Build up object based on whats passed in
				.GeographyLevel3ID = oLocation.GeographyLevel3ID
				.GeographyLevel2ID = oLocation.GeographyLevel2ID
				.GeographyLevel1ID = oLocation.GeographyLevel1ID
				.DepartureDate = DepartureDate
				.DepartureTime = "10:00"
				.ReturnDate = ReturnDate
				.ReturnTime = "10:00"
				.Adults = Adults
				.Children = Children
				.Infants = Infants

				.ChildAges = ChildAges
				.AdultAges = AdultAges

				If ExtraID > 0 Then oExtraSearch.ExtraID = ExtraID
				If ExtraGroupID > 0 Then oExtraSearch.ExtraGroupID = ExtraGroupID
				If ExtraTypeIDs IsNot Nothing Then oExtraSearch.ExtraTypeIDs.AddRange(ExtraTypeIDs)

			End With

			'return extra search
			Return oExtraSearch

		Catch ex As Exception
			oExtraSearch.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/ExtraSearch", "GetExtraSearchException", ex.ToString)
		End Try

		Return oExtraSearch

	End Function

#End Region

#Region "Package Search"

	Public Function PackageSearch(ByVal PackageReference As String) As SearchReturn

		'clear session
		BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)

		'setup package details request
		Dim oPackageDetailsRequest As New ivci.GetPackageDetailsRequest
		With oPackageDetailsRequest
			.LoginDetails = BookingBase.IVCLoginDetails
			.PackageReference = PackageReference
		End With

		'send request
		Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
		oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.GetPackageDetailsResponse)(oPackageDetailsRequest)

		Dim oPackageDetailsResponse As New ivci.GetPackageDetailsResponse

		'if response is ok setup search from package details
		If oIVCReturn.Success Then

			oPackageDetailsResponse = CType(oIVCReturn.ReturnObject, ivci.GetPackageDetailsResponse)

			Me.DepartingFromID = oPackageDetailsResponse.DepartureAirportID
			Me.ArrivingAtID = -oPackageDetailsResponse.GeographyLevel3ID
			Me.PropertyReferenceID = oPackageDetailsResponse.PropertyReferenceID
			Me.DepartureDate = oPackageDetailsResponse.DepartureDate
			Me.Duration = oPackageDetailsResponse.Duration
			Me.Rooms = 1

			'setup child ages
			Dim aChildAges As New Generic.List(Of Integer)
			Dim iChild As Integer = 1
			While iChild <= oPackageDetailsResponse.Children
				aChildAges.Add(8)
				iChild += 1
			End While

			'add room guests
			Me.RoomGuests.AddRoom(oPackageDetailsResponse.Adults, oPackageDetailsResponse.Children, oPackageDetailsResponse.Infants, aChildAges.ToArray)

		End If

		'update cookie
		Me.SetSearchCookie()

		'perform search and return
		Dim oReturn As New SearchReturn
		oReturn = Me.Search()
		Return oReturn

	End Function

#End Region

#Region "support - get cookie"

	Public Function GetSearchCookie() As String
		Dim sKeyValuePairs As String = Intuitive.CookieFunctions.Cookies.GetValue("__search_1")
		Return sKeyValuePairs
	End Function

	Public Sub SetSearchCookie()
		Try
			Intuitive.CookieFunctions.Cookies.SetValue("__search_1", Me.Encode(), BookingBase.Params.Search_CookieExpiry)
		Catch ex As Exception

		End Try
	End Sub

#End Region

#Region "Guest To GuestConfiguration"

	Public Shared Function GuestToGuestConfiguration(ByVal Guest As Guest) As ivci.Support.GuestConfiguration

		Dim oGuestConfiguration As New ivci.Support.GuestConfiguration
		With oGuestConfiguration
			.Adults = Guest.Adults
			.Children = Guest.Children
			.Infants = Guest.Infants
			.ChildAges = Guest.ChildAges
		End With
		Return oGuestConfiguration

	End Function

#End Region

End Class
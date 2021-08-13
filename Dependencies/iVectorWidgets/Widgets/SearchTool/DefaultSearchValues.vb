Imports Intuitive.Web
Imports Intuitive

Public Class DefaultSearchValues

	Private _SearchMode As String
	Private _DepartingFromID As Integer
	Private _ArrivingAtID As Integer
	Private _DepartureDate As Date
	Private _Duration As Integer
	Private _ArrivalTime As String
	Private _PickupDate As Date
	Private _DropOffTime As String
	Private _DropOffDate As Date
	Private _MealBasisID As Integer
	Private _Rating As Integer

	'Transfer defaults
	Private _TransferPickupLocationID As Integer
	Private _TransferDropOffLocationID As Integer
	Private _PickupType As String
	Private _DropOffType As String
	Private _ReturnTransfer As Boolean

	'Car Hire defaults
	Private _CarHireCountryID As Integer
	Private _CarHirePickUpDepotID As Integer
	Private _CarHireDropOffDepotID As Integer
	Private _CarHirePickUpDate As Date
	Private _CarHirePickUpTime As String
	Private _CarHireDropOffDate As Date
	Private _CarHireDropOffTime As String
	Private _CarHireDriverSellingCountryID As Integer
	Private _CarHireDriverAge As Integer
	Private _CarHirePassengers As Integer

	'Room and passenger Details
	Private _Rooms As Integer
	Private _RoomPassengers As List(Of RoomPassengersDef)


	Public Property SearchMode As String
		Get
			Return Me._SearchMode
		End Get
		Set(value As String)
			Me._SearchMode = value
		End Set
	End Property

	Public Property DepartingFromID As Integer
		Get
			Return Me._DepartingFromID
		End Get
		Set(value As Integer)
			Me._DepartingFromID = value
		End Set
	End Property

	Public Property ArrivingAtID As Integer
		Get
			Return Me._ArrivingAtID
		End Get
		Set(value As Integer)
			Me._ArrivingAtID = value
		End Set
	End Property

	Public Property DepartureDate As String
		Get
			Return Me._DepartureDate.ToShortDateString
		End Get
		Set(value As String)
			Me._DepartureDate = value.ToSafeDate
		End Set
	End Property

	Public Property Duration As Integer
		Get
			Return Me._Duration
		End Get
		Set(value As Integer)
			Me._Duration = value
		End Set
	End Property

	Public Property ArrivalTime As String
		Get
			Return Me._ArrivalTime
		End Get
		Set(value As String)
			Me._ArrivalTime = value
		End Set
	End Property

	Public Property PickupDate As String
		Get
			Return Me._PickupDate.ToShortDateString
		End Get
		Set(value As String)
			Me._PickupDate = value.ToSafeDate
		End Set
	End Property

	Public Property DropOffTime As String
		Get
			Return Me._DropOffTime
		End Get
		Set(value As String)
			Me._DropOffTime = value
		End Set
	End Property

	Public Property DropOffDate As String
		Get
			Return Me._DropOffDate.ToShortDateString
		End Get
		Set(value As String)
			Me._DropOffDate = value.ToSafeDate
		End Set
	End Property

	Public Property MealBasisID As Integer
		Get
			Return Me._MealBasisID
		End Get
		Set(value As Integer)
			Me._MealBasisID = value
		End Set
	End Property

	Public Property Rating As Integer
		Get
			Return Me._Rating
		End Get
		Set(value As Integer)
			Me._Rating = value
		End Set
	End Property

	Public Property TransferPickupLocationID As Integer
		Get
			Return Me._TransferPickupLocationID
		End Get
		Set(value As Integer)
			Me._TransferPickupLocationID = value
		End Set
	End Property

	Public Property TransferDropOffLocationID As Integer
		Get
			Return Me._TransferDropOffLocationID
		End Get
		Set(value As Integer)
			Me._TransferDropOffLocationID = value
		End Set
	End Property

	Public Property PickupType As String
		Get
			Return Me._PickupType
		End Get
		Set(value As String)
			Me._PickupType = value
		End Set
	End Property

	Public Property DropOffType As String
		Get
			Return Me._DropOffType
		End Get
		Set(value As String)
			Me._DropOffType = value
		End Set
	End Property

	Public Property ReturnTransfer As Boolean
		Get
			Return Me._ReturnTransfer
		End Get
		Set(value As Boolean)
			Me._ReturnTransfer = value
		End Set
	End Property

	Public Property Rooms As Integer
		Get
			Return Me._Rooms
		End Get
		Set(value As Integer)
			Me._Rooms = value
		End Set
	End Property

	Public Property RoomPassengers As List(Of RoomPassengersDef)
		Get
			Return Me._RoomPassengers
		End Get
		Set(value As List(Of RoomPassengersDef))
			Me._RoomPassengers = value
		End Set
	End Property

	Public Property CarHireCountryID As Integer
		Get
			Return Me._CarHireCountryID
		End Get
		Set(value As Integer)
			Me._CarHireCountryID = value
		End Set
	End Property

	Public Property CarHirePickUpDepotID As Integer
		Get
			Return Me._CarHirePickUpDepotID
		End Get
		Set(value As Integer)
			Me._CarHirePickUpDepotID = value
		End Set
	End Property

	Public Property CarHireDropOffDepotID As Integer
		Get
			Return Me._CarHireDropOffDepotID
		End Get
		Set(value As Integer)
			Me._CarHireDropOffDepotID = value
		End Set
	End Property

	Public Property CarHirePickUpDate As String
		Get
			Return Me._CarHirePickUpDate.ToShortDateString
		End Get
		Set(value As String)
			Me._CarHirePickUpDate = value.ToSafeDate
		End Set
	End Property

	Public Property CarHirePickUpTime As String
		Get
			Return Me._CarHirePickUpTime
		End Get
		Set(value As String)
			Me._CarHirePickUpTime = value
		End Set
	End Property

	Public Property CarHireDropOffDate As String
		Get
			Return Me._CarHireDropOffDate.ToShortDateString
		End Get
		Set(value As String)
			Me._CarHireDropOffDate = value.ToSafeDate
		End Set
	End Property

	Public Property CarHireDropOffTime As String
		Get
			Return Me._CarHireDropoffTime
		End Get
		Set(value As String)
			Me._CarHireDropoffTime = value
		End Set
	End Property

	Public Property CarHireDriverSellingCountryID As Integer
		Get
			Return Me._CarHireDriverSellingCountryID
		End Get
		Set(value As Integer)
			Me._CarHireDriverSellingCountryID = value
		End Set
	End Property

	Public Property CarHireDriverAge As Integer
		Get
			Return Me._CarHireDriverAge
		End Get
		Set(value As Integer)
			Me._CarHireDriverAge = value
		End Set
	End Property

	Public Property CarHirePassengers As Integer
		Get
			Return Me._CarHirePassengers
		End Get
		Set(value As Integer)
			Me._CarHirePassengers = value
		End Set
	End Property

    Public Property HighRatedHotelFilter As Boolean


	Public Sub New()

		Me._SearchMode = BookingBase.Params.Search_DefaultSearchMode.ToString
		Me._DepartingFromID = 0
		Me._ArrivingAtID = 0
		Me._DepartureDate = BookingBase.Params.Search_DefaultDate
		Me._Duration = BookingBase.Params.Search_DefaultDuration
		Me._ArrivalTime = "12:00"
		Me._PickupDate = DepartureDate.ToSafeDate
		Me._DropOffTime = "12:00"
		Me._DropOffDate = BookingBase.Params.Search_DefaultDate.AddDays(Me.Duration)
		Me._MealBasisID = 0
		Me._Rating = 0


		Me.TransferPickupLocationID = 0
		Me.TransferDropOffLocationID = 0
		Me.PickupType = ""
		Me.DropOffType = ""
		Me.ReturnTransfer = True

		Me._CarHireCountryID = 0
		Me._CarHirePickUpDepotID = 0
		Me._CarHireDropOffDepotID = 0
		Me._CarHirePickUpDate = BookingBase.Params.Search_DefaultDate
		Me._CarHirePickUpTime = ""
		Me._CarHireDropOffDate = BookingBase.Params.Search_DefaultDate.AddDays(Me.Duration)
		Me._CarHireDropOffTime = ""
		Me._CarHireDriverSellingCountryID = BookingBase.Params.SellingGeographyLevel1ID
		Me._CarHireDriverAge = 25
		Me._CarHirePassengers = 1

		Me.Rooms = 1
		Me.RoomPassengers = New List(Of RoomPassengersDef)
		Me.RoomPassengers.Add(New RoomPassengersDef(BookingBase.Params.Search_DefaulRoomAdults, 0, 0))
		Me.RoomPassengers.Add(New RoomPassengersDef())
		Me.RoomPassengers.Add(New RoomPassengersDef())

        Me.HighRatedHotelFilter = True

	End Sub

	Class RoomPassengersDef

		Private _Adults As Integer
		Private _Children As Integer
		Private _ChildAges As String
		Private _Infants As Integer

		Public Property Adults As Integer
			Get
				Return Me._Adults
			End Get
			Set(value As Integer)
				Me._Adults = value
			End Set
		End Property

		Public Property Children As Integer
			Get
				Return Me._Children
			End Get
			Set(value As Integer)
				Me._Children = value
			End Set
		End Property

		Public Property ChildAges As String
			Get
				Return Me._ChildAges
			End Get
			Set(value As String)
				Me._ChildAges = value
			End Set
		End Property

		Public Property Infants As Integer
			Get
				Return Me._Infants
			End Get
			Set(value As Integer)
				Me._Infants = value
			End Set
		End Property

		Public Sub New()
			Me._Adults = 0
			Me._Children = 0
			Me._ChildAges = ""
			Me._Infants = 0
		End Sub

		Public Sub New(ByVal Adults As Integer, ByVal Children As Integer, ByVal Infants As Integer)
			Me._Adults = Adults
			Me._Children = Children
			Me._ChildAges = ""
			Me._Infants = Infants

			If Children > 0 Then
				Me._ChildAges = "5"
				For ChildNumber As Integer = 2 To Children
					Me._ChildAges += "#5"
				Next
			End If
		End Sub

	End Class


End Class

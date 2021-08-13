Imports Intuitive
Imports Intuitive.Functions
Imports System.Text

Public Class BookingDeeplink

#Region "Properties"

	'deeplink params
	Public Mode As String
	Public DepDate As DateTime
	Public Nights As Integer

	Public Airport As String
	Public Region As String
    Public Resort As String
    Public GeographyGroup As String

	Public Longitude As Decimal
	Public Latitude As Decimal
	Public Radius As Integer

	Public Adults As Integer
	Public Children As Integer
	Public Infants As Integer
	Public ChildAges As String

	Public [Property] As Integer
	Public BoardBasis As Integer
	Public Rating As Integer
	Public ArrivalAirport As String
	Public Direct As Boolean

	Public Rooms As New Generic.List(Of MultiRoom)

	Public PropertyReferenceID As Integer = 0
	Public ArrivalAirportID As Integer

	Public PriorityPropertyID As Integer = 0


	'Transfer search params
	Public ArrivalParentType As String
	Public ArrivalParentID As String
	Public DepartureParentType As String
	Public DepartureParentID As String
	Public ReturnTime As String
	Public DepartureTime As String
	Public TransferSearch As New BookingTransfer.TransferSearch


	'one way - used for transfers and flight only
	Public OneWay As Boolean


	'booking search params
	Public SearchMode As BookingSearch.SearchModes
	Public Property DepartingFromID As Integer 'airportid or airportgroupid+1e6
    Public Property ArrivingAtID As Integer '-geog3id or geog2id
    Public Property GeographyGroupingID As Integer


	'valid
	Public Valid As Boolean
	Public ErrorMessage As String


#End Region


#Region "Constructor - Build from query string"

	Public Sub New()
	End Sub

	Public Sub New(ByVal sQueryString As String)

		'Unind query string to class
		Functions.Web.UnbindQuerystringToClass(Me, sQueryString, False)

		'check for multiroom
		Me.AddMultiRoom(sQueryString)

		'validate fields
		Me.Valid = Me.Validate()

	End Sub

#End Region


#Region "MultiRoom"

	Public Sub AddMultiRoom(ByVal sQueryString As String)

		Dim aValues As Hashtable = Functions.Web.ConvertQueryStringToHashTable(sQueryString)

		For Each sKey As String In aValues.Keys

			If sKey.StartsWith("room") Then

				Dim sPax As String = aValues(sKey).ToString.Split("|"c)(0)
				Dim sChildAges As String = ""

				If aValues(sKey).ToString.Contains("|"c) Then
					sChildAges = aValues(sKey).ToString.Split("|"c)(1)
				End If

				Dim oRoom As New MultiRoom

				With oRoom
					.Adults = SafeInt(sPax.Split(","c)(0))
					.Children = SafeInt(sPax.Split(","c)(1))
					.Infants = SafeInt(sPax.Split(","c)(2))
					.Childages = sChildAges
				End With

				Me.Rooms.Add(oRoom)

			End If

		Next

	End Sub


	Public Class MultiRoom

		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public Childages As String

	End Class

#End Region


#Region "Validation"

	Public Function Validate() As Boolean

		'validate fields
		Try

			'search mode
			If Not Me.Mode.ToLower = "hotel" AndAlso Not Me.Mode.ToLower = "flightplushotel" AndAlso Not Me.Mode.ToLower = "flight" AndAlso Not Me.Mode.ToLower = "transfer" Then
				Throw New Exception("could not establish search mode")
			End If


			'set booking search mode
			Select Case Me.Mode.ToLower
				Case "hotel"
					Me.SearchMode = BookingSearch.SearchModes.HotelOnly
				Case "flightplushotel"
					Me.SearchMode = BookingSearch.SearchModes.FlightPlusHotel
				Case "flight"
					Me.SearchMode = BookingSearch.SearchModes.FlightOnly
				Case "transfer"
					Me.SearchMode = BookingSearch.SearchModes.TransferOnly
			End Select


			'departure date
			Dim dDepartureDate As Date = DateFunctions.SafeDate(Me.DepDate)
			If dDepartureDate < Date.Now.Date OrElse dDepartureDate < DateTime.Now.Date.AddDays(BookingBase.Params.Search_BookAheadDays) Then
				Throw New Exception(String.Format("Date must be at least {0} days from today", BookingBase.Params.Search_BookAheadDays))
			End If


			'nights
            If Not Me.OneWay AndAlso Me.Nights < BookingBase.Params.Deeplink_DefaultMinDuration OrElse Me.Nights > BookingBase.Params.Deeplink_DefaultMaxDuration Then
                Throw New Exception(String.Format("Number of nights must be between {0} and {1}", BookingBase.Params.Deeplink_DefaultMinDuration, BookingBase.Params.Deeplink_DefaultMaxDuration))
            End If


			'airport
			If Me.Mode.ToLower = "flightplushotel" OrElse Me.Mode.ToLower = "flight" Then

				'set departure airport id
				If Not IsNumeric(Me.Airport) Then
					Me.DepartingFromID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.AirportIATACode, Me.Airport)
				Else
					Me.DepartingFromID = SafeInt(Me.Airport)
				End If


				'validate
				If Me.DepartingFromID < 1 Then
					Throw New Exception("could not establish departure airport")
				End If

			End If


			'Set up the transfer search object based on params
			If Me.Mode.ToLower = "transfer" Then
				With Me.TransferSearch

					.ArrivalParentType = SafeEnum(Of BookingTransfer.TransferSearch.ParentType)(Me.ArrivalParentType)
					.ArrivalParentID = SafeInt(Me.ArrivalParentID)
					.DepartureParentType = SafeEnum(Of BookingTransfer.TransferSearch.ParentType)(Me.DepartureParentType)
					.DepartureParentID = SafeInt(Me.DepartureParentID)

					.Adults = SafeInt(Me.Adults)
					.Children = SafeInt(Me.Children)
					.Infants = SafeInt(Me.Infants)

					.DepartureTime = Me.DepartureTime
					.DepartureDate = DateFunctions.SafeDate(Me.DepDate)

					.Oneway = SafeBoolean(Me.OneWay)
					If Not .Oneway Then
						.ReturnDate = DateFunctions.SafeDate(Me.DepDate).AddDays(SafeInt(Me.Nights))
						.ReturnTime = Me.ReturnTime
					End If

					.LoginDetails = BookingBase.IVCLoginDetails

				End With
			End If


			'arrival airport
			If Me.ArrivalAirport <> "" Then

				'set arrival airport id
				If Not IsNumeric(Me.ArrivalAirport) Then
					Me.ArrivalAirportID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.AirportIATACode, Me.ArrivalAirport)
				Else
					Me.ArrivalAirportID = SafeInt(Me.ArrivalAirport)
				End If

				Me.ArrivingAtID = Me.ArrivalAirportID + 1000000

			Else

				'region
				If Not IsNumeric(Me.Region) Then
					Me.ArrivingAtID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.Region, Me.Region)
				Else
					Me.ArrivingAtID = SafeInt(Me.Region)
				End If

				'resort
				If Not Me.Resort Is Nothing AndAlso Not IsNumeric(Me.Resort) Then
					Me.ArrivingAtID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.Resort, Me.Resort) * -1
				ElseIf SafeInt(Me.Resort) > 0 Then
					Me.ArrivingAtID = SafeInt(Me.Resort) * -1
                End If

                'geography group
                If Me.GeographyGroup <> "" Then
                    If Not IsNumeric(Me.GeographyGroup) Then
                        Me.GeographyGroupingID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.GeographyGrouping, Me.GeographyGroup)
                    Else
                        Me.GeographyGroupingID = SafeInt(Me.GeographyGroup)
                    End If
                End If

			End If


			'adults
			If Me.Adults < 1 Then
				Throw New Exception("At least 1 adult required")
			End If


			'child ages
			If Me.Children > 0 AndAlso Me.ChildAges = "" Then
				Throw New Exception("Child ages not set")
			End If


			'all ok
			Return True

		Catch ex As Exception
			Me.ErrorMessage = ex.Message
			Return False
		End Try

	End Function


#End Region


#Region "Search"

	Public Function Search() As BookingSearch.SearchReturn

		'clear session
		BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)


		'get value pairs and decode to object
		Dim oBookingSearch As New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
		With oBookingSearch
			.SearchMode = Me.SearchMode
			.DepartingFromID = Me.DepartingFromID
            .ArrivingAtID = Me.ArrivingAtID
            .GeographyGroupingID = Me.GeographyGroupingID
			.Direct = Me.Direct

			'set oneway if flight only
			If Me.SearchMode = BookingSearch.SearchModes.FlightOnly Then
				.OneWay = Me.OneWay
			End If

			.Longitude = Me.Longitude
			.Latitude = Me.Latitude
			.Radius = Me.Radius
            .MealBasisID = Me.BoardBasis
            .Rating = Me.Rating

			.PropertyReferenceID = Functions.IIf(Me.Property > 0, Me.Property, Me.PropertyReferenceID)
			.PriorityPropertyID = Me.PriorityPropertyID
			.DepartureDate = Me.DepDate
			.Duration = Me.Nights

			'Me.rooms is where the multi rooms live, first room is seperate for some dumb reason
			.Rooms = Functions.IIf(Me.Rooms.Count = 0, 1, Me.Rooms.Count + 1)

			'add first room
			Dim ChildAges As New Generic.List(Of Integer)
			If Me.Children > 0 Then
				For Each sChildAge As String In Me.ChildAges.Split(","c)
					ChildAges.Add(Functions.SafeInt(sChildAge))
				Next
			End If
			.RoomGuests.AddRoom(Me.Adults, Me.Children, Me.Infants, ChildAges.ToArray)


			'multi rooms
			For Each oRoom As MultiRoom In Me.Rooms
				ChildAges = New Generic.List(Of Integer)
				If oRoom.Children > 0 Then
					For Each sChildAge As String In oRoom.Childages.Split(","c)
						ChildAges.Add(Functions.SafeInt(sChildAge))
					Next
				End If
				.RoomGuests.AddRoom(oRoom.Adults, oRoom.Children, oRoom.Infants, ChildAges.ToArray)
			Next

			.TransferSearch = Me.TransferSearch
			.TransferSearch.ChildAges = ChildAges

		End With


		'save on session and save cookie
		BookingBase.SearchDetails = oBookingSearch
		BookingBase.SearchDetails.SetSearchCookie()


		'search and return
		Dim oReturn As BookingSearch.SearchReturn
		oReturn = BookingBase.SearchDetails.Search()
		Return oReturn


	End Function

#End Region


#Region "Current Search to Deeplink"

	Public Shared Function CurrentSearchToDeepLink(Optional ByVal oSearch As BookingSearch = Nothing) As String

		If oSearch Is Nothing Then oSearch = BookingBase.SearchDetails

		Dim sbDeeplinkURL As New StringBuilder
		With sbDeeplinkURL

			'Build up string builder from current search

			'mode
			Dim sSearchMode As String = ""
			Select Case oSearch.SearchMode.ToString
				Case "HotelOnly"
					sSearchMode = "hotel"
				Case "FlightPlusHotel"
					sSearchMode = "flightplushotel"
				Case "FlightOnly"
					sSearchMode = "flight"
				Case "TransferOnly"
					sSearchMode = "transfer"
			End Select
			.Append("?mode=").Append(sSearchMode)


			'date and duration
			.Append("&depdate=").Append(oSearch.DepartureDate.ToString.Substring(0, 10))
			.Append("&nights=").Append(oSearch.Duration.ToString)


			'geography
			If oSearch.ArrivingAtID > 0 Then
				.Append("&region=").Append(oSearch.ArrivingAtID.ToString)
			Else
				.Append("&resort=").Append((oSearch.ArrivingAtID * -1).ToString)
            End If
            If oSearch.GeographyGroupingID > 0 Then
                .Append("&geographygroup=").Append(oSearch.GeographyGroupingID.ToString)
            End If
			If oSearch.SearchMode = BookingSearch.SearchModes.FlightPlusHotel OrElse oSearch.SearchMode = BookingSearch.SearchModes.FlightOnly Then
				If oSearch.DepartingFromID > 0 Then
					.Append("&airport=").Append(oSearch.DepartingFromID.ToString)
				End If
				If oSearch.ArrivingAtID > 1000000 Then
					.Append("&arrivalairport=").Append((oSearch.ArrivingAtID - 1000000).ToString)
				End If
			End If


			'If a transfer only Search need some different values on the deep link
			If oSearch.SearchMode = BookingSearch.SearchModes.TransferOnly Then

				.Append("&ArrivalParentType=").Append(oSearch.TransferSearch.ArrivalParentType.ToString)
				.Append("&ArrivalParentID=").Append(oSearch.TransferSearch.ArrivalParentID.ToString)

				.Append("&DepartureParentType=").Append(oSearch.TransferSearch.DepartureParentType.ToString)
				.Append("&DepartureParentID=").Append(oSearch.TransferSearch.DepartureParentID.ToString)

				.Append("&OneWay=").Append(oSearch.TransferSearch.Oneway.ToString)

				.Append("&Returntime=").Append(oSearch.TransferSearch.ReturnTime.ToString)
				.Append("&DepartureTime=").Append(oSearch.TransferSearch.DepartureTime.ToString)
            Else
                .Append("&OneWay=").Append(oSearch.OneWay.ToString)
            End If


			'geocode
			If oSearch.Longitude <> 0 Then
				.AppendFormat("&longitude={0}", oSearch.Longitude.ToString)
				.AppendFormat("&latitude={0}", oSearch.Latitude.ToString)
				.AppendFormat("&radius={0}", oSearch.Radius.ToString)
			End If


			'property
			If oSearch.PropertyReferenceID > 0 Then
				.Append("&property=").Append(oSearch.PropertyReferenceID.ToString)
			End If

			If oSearch.PriorityPropertyID > 0 Then
				.Append("&prioritypropertyid=").Append(oSearch.PriorityPropertyID.ToString)
			End If


			'pax
			If oSearch.Rooms > 0 Then
				.Append("&adults=").Append(oSearch.RoomGuests(0).Adults.ToString)
				.Append("&children=").Append(oSearch.RoomGuests(0).Children.ToString)
				.Append("&infants=").Append(oSearch.RoomGuests(0).Infants.ToString)
				If oSearch.RoomGuests(0).Children > 0 Then
					.Append("&childages=")
					Dim sChildAges As String = ""
					For Each iChild As Integer In oSearch.RoomGuests(0).ChildAges
						sChildAges = sChildAges & iChild.ToString & ","
					Next
					.Append(sChildAges.Chop())
				End If
			End If


			'others
			If oSearch.MealBasisID > 0 Then
				.Append("&boardbasis=").Append(oSearch.MealBasisID.ToString)
			End If
			If oSearch.Rating > 0 Then
				.Append("&rating=").Append(oSearch.Rating.ToString)
			End If


			'MultiRoom
			Dim i As Integer = 1
			While i < oSearch.Rooms
				.Append("&room").Append((i + 1).ToString).Append("=")
				'Adults
				.Append(oSearch.RoomGuests(i).Adults)
				.Append(",")
				'Children
				.Append(oSearch.RoomGuests(i).Children)
				.Append(",")
				'Infants
				.Append(oSearch.RoomGuests(i).Infants)
				'ChildAges
				If oSearch.RoomGuests(i).ChildAges.Count > 0 Then
					.Append("|")
					Dim first As Boolean = True
					For Each iChildAge As Integer In oSearch.RoomGuests(i).ChildAges
						If Not first Then .Append(",")
						.Append(iChildAge.ToString)
						first = False
					Next
				End If
				i += 1
			End While

		End With

		Return sbDeeplinkURL.ToString

	End Function

#End Region

End Class

Imports ivci = iVectorConnectInterface

Public Class BookingAvailability

	Public Shared Function Search(ByVal ArrivingAtID As Integer, ByVal ArrivalDate As Date,
 ByVal Duration As Integer, ByVal Adults As Integer) As ivci.Property.RoomAvailabilityResponse


		'1. build request
		Dim oSearchRequest As New ivci.Property.RoomAvailabilityRequest
		oSearchRequest.LoginDetails = BookingBase.IVCLoginDetails

		Dim iGeographyLevel2ID As Integer

		If ArrivingAtID > 0 Then
			oSearchRequest.GeographyLevel2ID = ArrivingAtID
			iGeographyLevel2ID = ArrivingAtID
		Else
			oSearchRequest.GeographyLevel3ID = -ArrivingAtID
			iGeographyLevel2ID = BookingBase.Lookups.GetLocationFromResort(-ArrivingAtID).GeographyLevel2ID
		End If

		oSearchRequest.ArrivalDate = ArrivalDate
		oSearchRequest.Duration = Duration


		'2 validate
		Dim aWarnings As Generic.List(Of String) = oSearchRequest.Validate()
		If aWarnings.Count > 0 Then
			Throw New Exception("search did not validate " & aWarnings(0).ToString)
		End If


		'3. send request
		Dim oiVCResponse As Utility.iVectorConnect.iVectorConnectReturn
		oiVCResponse = Utility.iVectorConnect.SendRequest(Of ivci.Property.RoomAvailabilityResponse)(oSearchRequest)


		'4. return if ok, else nothing
		If oiVCResponse.Success Then

			'5. Get the response into a format that we can use
			Dim oRoomAvailabilityResponse As ivci.Property.RoomAvailabilityResponse = CType(oiVCResponse.ReturnObject, ivci.Property.RoomAvailabilityResponse)

			'6. Save Results
			BookingAvailability.Availability.Save(oRoomAvailabilityResponse.PropertyResults, Duration, ArrivalDate, iGeographyLevel2ID, Adults)

			'7. Set false if we dont have enough availability

			Return oRoomAvailabilityResponse
		End If

		Return Nothing

	End Function


#Region "Results"

	Public Class Availability

#Region "Properties"

		Public Availabilities As New Generic.List(Of ivci.Property.RoomAvailabilityResponse.PropertyResult)
		Public ArrivalDate As New Date
		Public Duration As New Integer
		Public ArrivingAtID As New Integer
		Public Availability As Boolean = False
		Public Adults As New Integer

#End Region

		Public Shared Sub Save(ByVal RoomAvailabilities As Generic.List(Of ivci.Property.RoomAvailabilityResponse.PropertyResult), ByVal Duration As Integer, _
		  ByVal ArrivalDate As Date, ByVal GeographyLevel2ID As Integer, ByVal Adults As Integer)

			Dim oAvailability As New Availability
			'Use a counter to ensure between our availabilities we have enough for the pax we've sear
			Dim PaxAvailability As New Integer

			'loop through flight results and add to collection
			For Each oRoomAvailability As ivci.Property.RoomAvailabilityResponse.PropertyResult In RoomAvailabilities

				'We only want to loop through until we have confirmed we have enough availability, no point wasting time after that.
				If PaxAvailability < Adults Then
					For Each oRoom As ivci.Property.RoomAvailabilityResponse.RoomType In oRoomAvailability.RoomTypes

						If oRoom.AvailableRooms > 0 And oRoom.Unavailable = False Then
							PaxAvailability += (oRoom.AvailableRooms * oRoom.MaxOccupancy)
						End If

						If PaxAvailability >= Adults Then Exit For

					Next
				End If


				'add Each Property Availability
				oAvailability.Availabilities.Add(oRoomAvailability)

			Next

			'Set some values that we need to access elsewhere from the search
			oAvailability.Duration = Duration
			oAvailability.ArrivalDate = ArrivalDate
			oAvailability.ArrivingAtID = GeographyLevel2ID

			oAvailability.Availability = PaxAvailability >= Adults
			oAvailability.Adults = Adults

			'save on session
			BookingBase.SearchAvailabilities = oAvailability

		End Sub
	End Class
#End Region



End Class
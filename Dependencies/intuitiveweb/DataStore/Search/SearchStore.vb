Namespace DataStore.Search

    Public NotInheritable Class SearchStore

        Public SearchGuid As String
        Public SearchType As String
        Public Destination As Destination.DestinationDetails
        Public Departure As Departure.DepartureDetails
        Public Passengers As New SearchPassengers
        Public SearchRequests As New List(Of Request.SearchRequest)

        Public Sub New()
        End Sub

        Public Sub New(SearchDetails As BookingSearch)

            'common fields
            Me.SearchGuid = SearchDetails.SearchGuid
            Me.SearchType = SearchDetails.SearchMode.ToString

			Dim oPassengers As New SearchPassengers
			oPassengers.Adults = SearchDetails.TotalAdults
            oPassengers.Children = SearchDetails.TotalChildren
            oPassengers.Infants = SearchDetails.TotalInfants
            Me.Passengers = oPassengers

            'departure strategy
            Dim oDeparture As New Departure.DepartureDetails
            Dim oDepartureStrategy As Departure.IDepartureStrategy
            If SearchDetails.DepartingFromID > 1000000 Then
                oDepartureStrategy = New Departure.AirportGroupDeparture
            Else
                oDepartureStrategy = New Departure.AirportDeparture
            End If
            oDeparture.Setup(oDepartureStrategy, SearchDetails.DepartingFromID)
            Me.Departure = oDeparture


            'destination strategy
            Dim oDestination As New Destination.DestinationDetails
            Dim oLocationStrategy As Destination.ILocationStrategy

            If SearchDetails.ArrivingAtID < 0 Then
                oLocationStrategy = New Destination.ResortLocation
            Else
                oLocationStrategy = New Destination.RegionLocation
            End If

            oDestination.SetGeographyIDs(oLocationStrategy, SearchDetails.ArrivingAtID)
            oDestination.Duration = SearchDetails.Duration
            oDestination.MealBasisID = SearchDetails.MealBasisID
            oDestination.MinStarRating = SearchDetails.Rating
            Me.Destination = oDestination

            If SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
                OrElse SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then

                Dim oPropertyStrategy As New Request.PropertyRequest
                Dim oPropertyRequest As New Request.SearchRequest
                oPropertyRequest.Setup(oPropertyStrategy, SearchDetails)
                Me.SearchRequests.Add(oPropertyRequest)

            End If

            If SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
                OrElse SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly Then

                Dim oFlightStrategy As New Request.FlightRequest
                Dim oFlightRequest As New Request.SearchRequest
                oFlightRequest.Setup(oFlightStrategy, SearchDetails)
                Me.SearchRequests.Add(oFlightRequest)

            End If

        End Sub


    End Class


End Namespace


Namespace DataStore.Search.Departure

    Public Class AirportDeparture
        Implements IDepartureStrategy

        Public Sub Implement(ParentID As Integer, ByRef Departure As DepartureDetails) Implements IDepartureStrategy.Implement

            Departure.ParentType = "Airport"
            Departure.ParentId = ParentID

        End Sub
    End Class


End Namespace

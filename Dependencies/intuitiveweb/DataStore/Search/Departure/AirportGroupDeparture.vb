
Namespace DataStore.Search.Departure

    Public Class AirportGroupDeparture
        Implements IDepartureStrategy

        Public Sub Implement(ParentID As Integer, ByRef Departure As DepartureDetails) Implements IDepartureStrategy.Implement

            Departure.ParentType = "AirportGroup"
            Departure.ParentId = ParentID - 1000000

        End Sub

    End Class


End Namespace

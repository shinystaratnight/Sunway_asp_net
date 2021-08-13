
Namespace DataStore.Search.Destination

    Public Class ResortLocation
        Implements ILocationStrategy

        Public Function Implement(DestinationId As Integer) As Lookups.Location Implements ILocationStrategy.Implement

            Return BookingBase.Lookups.GetLocationFromResort(DestinationId * -1)

        End Function

    End Class


End Namespace

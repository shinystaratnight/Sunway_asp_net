
Namespace DataStore.Search.Destination

    Public Class RegionLocation
        Implements ILocationStrategy

        Public Function Implement(DestinationId As Integer) As Lookups.Location Implements ILocationStrategy.Implement

            Return BookingBase.Lookups.GetLocationFromRegion(DestinationId)

        End Function

    End Class



End Namespace
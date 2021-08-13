
Namespace DataStore.Search.Destination

    Public Class DestinationDetails

        Property Duration As Integer
        Property MealBasisID As Integer
        Property MinStarRating As Integer
        Property GeographyLevel1ID As Integer
        Property GeographyLevel2ID As Integer
        Property GeographyLevel3ID As Integer

        Public Sub SetGeographyIDs(LocationStrategy As ILocationStrategy, DestinationId As Integer)

            Dim oLocation As Lookups.Location = LocationStrategy.Implement(DestinationId)
            Me.GeographyLevel1ID = oLocation.GeographyLevel1ID
            Me.GeographyLevel2ID = oLocation.GeographyLevel2ID
            Me.GeographyLevel3ID = oLocation.GeographyLevel3ID

        End Sub

    End Class


End Namespace

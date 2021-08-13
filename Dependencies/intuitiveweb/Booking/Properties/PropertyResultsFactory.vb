Public Class PropertyResultsFactory

    Public Function Create(PropertyResult As iVectorConnectInterface.Property.SearchResponse.PropertyResult) As Booking.Property.PropertyResult

        Dim oPropertyResult As New Booking.Property.PropertyResult
        With oPropertyResult
            .BookingToken = PropertyResult.BookingToken
            .ContractID = PropertyResult.ContractID
            .GeographyLevel1ID = PropertyResult.GeographyLevel1ID
            .GeographyLevel2ID = PropertyResult.GeographyLevel2ID
            .GeographyLevel3ID = PropertyResult.GeographyLevel3ID
            .PropertyReferenceID = PropertyResult.PropertyReferenceID
            .Rental = PropertyResult.Rental
            .RoomTypes = PropertyResult.RoomTypes
            .SearchResponseXML = PropertyResult.SearchResponseXML
            .Source = PropertyResult.Source
            .TokenHelpers = PropertyResult.TokenHelpers
            .ArrivalDate = BookingBase.SearchDetails.HotelArrivalDate
			.Duration = BookingBase.SearchDetails.HotelDuration
			.RoomGroups = PropertyResult.RoomGroups
		End With

        Return oPropertyResult

    End Function

    Public Function CreateList(PropertyResults As List(Of iVectorConnectInterface.Property.SearchResponse.PropertyResult)) As List(Of Booking.Property.PropertyResult)
        Return PropertyResults.Select(Function(o) Create(o)).ToList()
    End Function

    Public Function CreateIVC(PropertyResult As Booking.Property.PropertyResult) As iVectorConnectInterface.Property.SearchResponse.PropertyResult

        Dim oPropertyResult As New iVectorConnectInterface.Property.SearchResponse.PropertyResult
        With oPropertyResult
            .BookingToken = PropertyResult.BookingToken
            .ContractID = PropertyResult.ContractID
            .GeographyLevel1ID = PropertyResult.GeographyLevel1ID
            .GeographyLevel2ID = PropertyResult.GeographyLevel2ID
            .GeographyLevel3ID = PropertyResult.GeographyLevel3ID
            .PropertyReferenceID = PropertyResult.PropertyReferenceID
            .Rental = PropertyResult.Rental
			.RoomTypes = PropertyResult.RoomTypes
			.RoomGroups = PropertyResult.RoomGroups
			.SearchResponseXML = PropertyResult.SearchResponseXML
            .Source = PropertyResult.Source
            .TokenHelpers = PropertyResult.TokenHelpers
        End With

        Return oPropertyResult

    End Function

    Public Function CreateIVCList(PropertyResults As List(Of Booking.Property.PropertyResult)) As List(Of iVectorConnectInterface.Property.SearchResponse.PropertyResult)
        Return PropertyResults.Select(Function(o) CreateIVC(o)).ToList()
    End Function

End Class

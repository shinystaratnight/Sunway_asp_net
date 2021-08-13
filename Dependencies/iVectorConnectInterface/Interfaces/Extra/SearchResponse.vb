Imports System.Xml.Serialization

Namespace Extra

    <XmlRoot("ExtraSearchResponse")>
    Public Class SearchResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

        Public Property ExtraTypes As Generic.List(Of ExtraType)

#End Region

#Region "Helper Classes"

        Public Class ExtraType
            Public Property ExtraTypeID As Integer
            Public Property ExtraSubTypes As Generic.List(Of ExtraSubType)
            Public Property Notes As String
        End Class

        Public Class ExtraSubType
            Public Property ExtraSubTypeID As Integer
            Public Property Extras As Generic.List(Of Extra)

        End Class

        Public Class Extra
            Public Property ExtraID As Integer
            Public Property Source As String
            Public Property ExtraName As String
            Public Property ExtraSubName As String
            Public Property ExtraType As String
            Public Property SupplierID As Integer
            Public Property Notes As String
            Public Property RequiresFullPaxInfo As Boolean
            Public Property RecordWeight As Boolean
            Public Property Options As Generic.List(Of [Option])
            Public Property Languages As Generic.List(Of Language)
            Public Property ProductAttributes As Generic.List(Of ProductAttribute)
            Public Property PickupPoints As Generic.List(Of PickupPoint)
            Public Property ExtraSchedules As Generic.List(Of ExtraSchedule)
            Public Property ExtraLocations As ExtraLocations
            Public Property ItineraryInformation As New ItineraryInformation
            Public Property BookableByRoom As Boolean
        End Class

        Public Class [Option]
            Public Property BookingToken As String
            Public Property ExtraCategoryID As Integer
            Public Property ExtraCategory As String
            Public Property TPExtraSubName As String
            Public Property Notes As String
            Public Property ExtraDurationID As Integer
            Public Property Duration As Integer
            Public Property DateRequired As Boolean
            Public Property TimeRequired As Boolean
            Public Property AgeRequired As Boolean
            Public Property StartDate As Date
            Public Property StartTime As String
            Public Property EndDate As Date
            Public Property EndTime As String

            Public Property OccupancyRules As Boolean
            Public Property MinPassengers As Integer
            Public Property MaxPassengers As Integer
            Public Property MinAdults As Integer
            Public Property MaxAdults As Integer
            Public Property MinChildren As Integer
            Public Property MaxChildren As Integer
            Public Property SupplierID As Integer

            Public Property MinimumAge As Integer
            Public Property MaximumAge As Integer
            Public Property MinChildAge As Integer
            Public Property MaxChildAge As Integer
            Public Property SeniorAge As Integer

            Public Property PricingType As String
            Public Property ExtraPrice As Decimal
            Public Property AdultPrice As Decimal
            Public Property ChildPrice As Decimal
            Public Property InfantPrice As Decimal
            Public Property SeniorPrice As Decimal
            Public Property TotalPrice As Decimal
            Public Property TotalCommission As Decimal
            Public Property PriceChangePercentage As Decimal
            Public Property MoreInfoKey As String

            Public Property Description As String
            <XmlArrayItem("GenericDetail")>
            Public Property GenericDetails As Generic.List(Of String)
            Public Property MultiBook As Boolean
            Public Property MaximumQuantity As Integer

            Public Property AttractionDetails As AttractionDetail
            Public Property OptionalSupplements As Generic.List(Of OptionalSupplement)
            Public Property CarParkInformation As CarParkDetails
            Public Property TrainDetails As TrainDetails
            Public Property SupplierDetails As Support.SupplierDetails
            Public Property Guaranteed As Boolean
        End Class

        Public Class OptionalSupplement
            Public Property SupplementBookingToken As String
            Public Property ExtraSupplementID As Integer
            Public Property Supplement As String
            Public Property PricingType As String
            Public Property ExtraPrice As Decimal
            Public Property AdultPrice As Decimal
            Public Property ChildPrice As Decimal
            Public Property InfantPrice As Decimal
            Public Property TourItineraryDay As Integer
        End Class

        Public Class CarParkDetails
            Public Property AirportID As Integer
            Public Property AirportTerminalID As Integer
            Public Property CarParkName As String
            Public Property CarParkCode As String
            Public Property TransferFrequency As String
            Public Property TransferTime As String
            Public Property CarParkInformation As String
            Public Property MapURL As String
        End Class

		Public Class TrainDetails
			<XmlElement("Class")>
			Public Property TrainClass As String
			Public Property TPRef As String
			Public Property ClassCode As String
			Public Property FareName As String
			Public Property FareCode As String

			<XmlArrayItem("Sector")>
			Public Property Sectors As List(Of TrainSector)
		End Class

		Public Class TrainSector
			Public Property TrainRef As String
			Public Property Direction As Domain.Enums.Train.Direction
			Public Property Sequence As Integer
			Public Property Category As String
			Public Property Number As Integer
			Public Property DepartureStation As String
			Public Property DepartureStationID As Integer
			Public Property DepartureDate As Date
			Public Property DepartureTime As String
			Public Property ArrivalStation As String
			Public Property ArrivalStationID As Integer
			Public Property ArrivalDate As Date
			Public Property ArrivalTime As String
		End Class

        Public Class AttractionDetail
            Public Property Image As String
            Public Property ImageThumbnail As String
            <XmlArrayItem("OtherImage")>
            Public Property OtherImages As Generic.List(Of String)
            Public Property URL As String
            Public Property Description As String
            Public Property TicketType As String
            Public Property IndividualInfoItems As Generic.List(Of IndividualInfoItem)
            <XmlArrayItem("GroupInfoItem")>
            Public Property GroupInfoItems As Generic.List(Of IndividualInfoItem)
            <XmlArrayItem("Time")>
            Public Property Times As Generic.List(Of String)
            Public Property CancellationFees As Generic.List(Of CancellationFee)
        End Class

        Public Class IndividualInfoItem
            Public Property Name As String
            Public Property ID As Integer
            Public Property Type As String
            Public Property Values As Generic.List(Of String)
        End Class

        Public Class CancellationFee
            Public Property StartDate As Date
            Public Property EndDate As Date
            Public Property Percentage As String
            Public Property Amount As Decimal
        End Class

        Public Class Language
            Public Property LanguageID As Integer
            Public Property LanguageName As String
        End Class

        Public Class ProductAttribute
            Public Property ProductAttributeID As Integer
            Public Property ProductAttributeName As String
        End Class

        Public Class PickupPoint
            Public Property PickupPointID As Integer
            Public Property PickupTime As String
            Public Property PickupLocation As String
        End Class

        Public Class ExtraSchedule
            Public Property Schedule As String
            Public Property StartDate As String
            Public Property EndDate As String
            Public Property ExtraTime As String
            Public Property DayOfWeek As Integer
        End Class

        Public Class ItineraryInformation
            Public Property ExtraDurations As New List(Of ExtraDuration)
            Public Property PassengerTypes As New PassengerTypes
            Public Property RateHeader As New RateHeader
            Public Property Schedule As String
            Public Property PricingType As String
            Public Property HlpRequiresPickupPoint As Boolean = False
            Public Property HlpRequiresDropoffPoint As Boolean = False
            Public Property GeographyLevel1ID As Integer
            Public Property Basis As String
            Public Property Summary As String
            Public Property Supplier As String
            Public Property JoiningInstructions As String
        End Class

        Public Class ExtraDuration
            Public Property ExtraDurationID As Integer
            Public Property ExtraDuration As Decimal
            Public Property ExtraEndDuration As Decimal
        End Class


        Public Class PassengerTypes
            Public Property Adult As Boolean
            Public Property Child As Boolean
            Public Property Infant As Boolean
            Public Property Youth As Boolean
            Public Property Senior As Boolean
        End Class

        Public Class RateHeader
            Public Property [Single] As Boolean
            Public Property [Double] As Boolean
            Public Property Triple As Boolean
            Public Property Quad As Boolean
            Public Property ExtraAdult As Boolean
        End Class
#End Region

    End Class

End Namespace
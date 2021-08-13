Imports System.Xml.Serialization

Namespace CarHire

	<XmlRoot("CarHireSearchResponse")>
	Public Class SearchResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

		Public Property CarHireResults As New Generic.List(Of CarHireResult)

		Public Class CarHireResult
			Public Property BookingToken As String
			Public Property OnRequest As Integer
			Public Property VehicleDescription As String
			Public Property PaxCapacity As Integer
			Public Property AdultCapacity As Integer
			Public Property ChildCapacity As Integer
			Public Property BaggageCapacity As Integer
			Public Property SmallBaggageCapacity As Integer
			Public Property LargeBaggageCapacity As Integer
			Public Property Price As Decimal
			Public Property ImageURL As String
			Public Property CarInformation As String
			Public Property SIPPCode As String
			Public Property AdditionalInformation As String
			Public Property IncludedInRate As String

			Public Property CarHireExtras As Generic.List(Of iVectorConnectInterface.CarHire.SearchResponse.CarHireExtra)
			Public Property SupplierDetails As New Support.SupplierDetails
			Public Property CarHireContractID As Integer
			Public Property CarHireContractCarTypeID As Integer

			Public Property PickUpInformation As DepotInformation
			Public Property PickUpDepotID As Integer
			Public Property PickUpDepotName As String
			Public Property PickUpDate As Date
			Public Property PickUpTime As String

			Public Property DropOffInformation As DepotInformation
			Public Property DropOffDepotID As Integer
			Public Property DropOffDate As Date
            Public Property DropOffDepotName As String
			Public Property DropOffTime As String
            Public Property SupplierName As String
            Public Property ItineraryInformation As New ItineraryInformation

		End Class

#Region "helper classes"

		Public Class CarHireExtra
			Public Property ExtraBookingToken As String
			Public Property Description As String
			Public Property Price As Decimal
			Public Property Mandatory As Boolean
			Public Property IncludedInPrice As Boolean
			Public Property PrePaid As Boolean
            Public Property MaxAmount As Integer
			Public Property LocalCost As CarHireExtraLocalCost

			Public Class CarHireExtraLocalCost
				Public Property Cost As Decimal
				Public Property CurrencySymbol As String
				Public Property CurrencyCode As String
				Public Property SymbolPosition As String
			End Class
		End Class

		Public Class DepotInformation
			Public Property DepotName As String
			Public Property Address1 As String
			Public Property City As String
			Public Property Phone As String
			Public Property Postcode As String
			Public Property OpeningTimes As String
            Public Property TPDepotID As Integer
		End Class

        Public Class ItineraryInformation

            Public Property Basis As String
            Public Property [Class] As String
            Public Property Make As String
            Public Property Model As String
            Public Property ExampleCar As String
            Public Property UnlimitedMileage As Boolean
            Public Property MaxPassengers As Integer
            Public Property EngineCapacity As Decimal
            Public Property Features As List(Of Feature)

            Public Property IncludedOneWayCharge As Boolean
            Public Property DropOffDepot As String
            Public Property DropOffInformation As String

            Public Property PickUpDepot As String
            Public Property PickUpInformation As String



        End Class

        Public Class Feature
            Public Property VehicleFeature As String
        End Class

#End Region

	End Class

End Namespace
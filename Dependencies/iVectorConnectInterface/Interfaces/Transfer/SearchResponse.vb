Imports System.Xml.Serialization

Namespace Transfer

	<XmlRoot("TransferSearchResponse")>
	Public Class SearchResponse
		Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property Transfers As New Generic.List(Of Transfer)

		Public Class Transfer
			Public Property BookingToken As String
			Public Property Vehicle As String
			Public Property VehicleTypeID As Integer
			Public Property MinimumCapacity As Integer
			Public Property MaximumCapacity As Integer
			Public Property MaximumBaggage As Integer
			Public Property BaggageDescription As String
			Public Property VehicleQuantity As Integer
			Public Property DepartureParentType As String
			Public Property DepartureParentID As Integer
			Public Property ArrivalParentType As String
			Public Property ArrivalParentID As Integer
			Public Property OutboundJourneyTime As String
			Public Property ReturnJourneyTime As String
			Public Property Price As Decimal
			Public Property TotalCommission As Decimal
			Public Property SupplierDetails As New Support.SupplierDetails

			Public Property TransferContractID As Integer
			Public Property VehicleClass As String

            Public Property ItineraryInformation As New ItineraryInformation

			Public Function ShouldSerializeTransferContractID() As Boolean
				Return False
			End Function

		End Class

        Public Class ItineraryInformation
            Public Property Source As String
            Public Property SupplierName As String
        End Class

	End Class

End Namespace
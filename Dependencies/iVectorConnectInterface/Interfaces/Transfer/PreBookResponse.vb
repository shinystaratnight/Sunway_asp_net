Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace Transfer
	<XmlType("TransferPreBookResponse")>
	<XmlRoot("TransferPreBookResponse")>
	Public Class PreBookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingToken As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
        Public Property SupplierDetails As Support.SupplierDetails
		Public Property Cancellations As New CancellationsList(Of Cancellation)
		Public Property PaymentsDue As New List(Of Support.PaymentDue)
		Public Property CommissionType As String
        Public Property CommissionValue As String
        Public Property DepartureNotes As String
        Public Property ReturnNotes As String
        Public Property OutboundJourneyTime As string
        Public Property ReturnJourneyTime As String
        Public Property BondingID As Integer

        <XmlIgnore()>
		Public Property hlpTransferBookingID As Integer
	End Class
End Namespace

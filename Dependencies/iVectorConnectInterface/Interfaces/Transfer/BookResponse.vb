Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace Transfer
	<XmlType("TransferBookResponse")>
	<XmlRoot("TransferBookResponse")>
	Public Class BookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingReference As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property Cancellations As New CancellationsList(Of Cancellation)
		Public Property SupplierDetails as New Support.SupplierDetails 
        <XmlIgnore()> Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)
        <XmlIgnore()> Public Property hlpCurrencyID As Integer

	End Class
End Namespace

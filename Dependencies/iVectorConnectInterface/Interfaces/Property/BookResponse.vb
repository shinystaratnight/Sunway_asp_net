Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace [Property]
	<XmlType("PropertyBookResponse")>
	<XmlRoot("PropertyBookResponse")>
	Public Class BookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingReference As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property PayLocalTotal As Decimal
		Public Property SupplierDetails As Support.SupplierDetails
		Public Property PaymentsDue As New List(Of Support.PaymentDue)
        Public Property Cancellations As New CancellationsList(Of Cancellation)

        <XmlIgnore()>
		Public Property hlpCurrencyID As Integer
	End Class
End Namespace

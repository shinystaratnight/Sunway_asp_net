Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace Flight
	<XmlType("FlightBookResponse")>
	<XmlRoot("FlightBookResponse")>
	Public Class BookResponse
		Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingReference As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property PaymentsDue As List(Of Support.PaymentDue)
		Public Property ProductAttributes As New List(Of Support.ProductAttribute)
		Public Property Cancellations As CancellationsList(Of Cancellation)

		''' <summary>
		'''     The multi carrier cancellations.
		''' </summary>
		Public MultiCarrierCancellations As CancellationsList(Of Cancellation)
	End Class
End Namespace
Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace AdHoc
	<XmlType("AdHocPropertyBookResponse")>
	<XmlRoot("AdHocPropertyBookResponse")>
	Public Class PropertyBookResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements iVectorConnectResponse.ReturnStatus
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property PaymentsDue As List(Of Support.PaymentDue)
		Public Property Cancellations As CancellationsList(Of Cancellation)

	End Class
End Namespace


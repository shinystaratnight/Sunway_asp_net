Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace [Property]
	<XmlType("PropertyPreBookLiteResponse")>
	<XmlRoot("PropertyPreBookLiteResponse")>
	Public Class PreBookLiteResponse

        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse
        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
		Public Property BookingToken As String
		Public Property CurrencyCode As String
		Public Property TotalPrice As Decimal
		Public Property Cancellations As New CancellationsList(Of Cancellation)


		<XmlIgnore()>
		Public Property hlpCurrencyID As Integer
	End Class
End Namespace

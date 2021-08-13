Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial

Namespace Extra
	<XmlType("ExtraBookResponse")>
	<XmlRoot("ExtraBookResponse")>
	Public Class BookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property Cancellations As New CancellationsList(Of Cancellation)

        <XmlIgnore()>
        Public Property PaymentsDue As New List(Of Support.PaymentDue)

        Public Property ExtraLocations As ExtraLocations

		Public property JourneyDetails As JourneyDetails

		<XmlIgnore()>
		Public Property hlpCurrencyID As Integer
	End Class
End Namespace
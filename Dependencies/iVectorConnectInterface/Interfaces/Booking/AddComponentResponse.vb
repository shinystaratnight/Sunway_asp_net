Imports System.Xml.Serialization


Public Class AddComponentResponse

	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property BookingReference As String
	Public Property TotalPrice As Decimal
	Public Property TotalCommission As Decimal
	Public Property VATOnCommission As Decimal

	<XmlArrayItem("PropertyBookResponse")>
	Public Property PropertyBookings As New Generic.List(Of [Property].BookResponse)
	<XmlArrayItem("FlightBookResponse")>
	Public Property FlightBookings As New Generic.List(Of Flight.BookResponse)
	<XmlArrayItem("TransferBookResponse")>
	Public Property TransferBookings As New Generic.List(Of Transfer.BookResponse)
    <XmlArrayItem("ExtraBookResponse")>
    Public Property ExtraBookings As New Generic.List(Of Extra.BookResponse)
    <XmlArrayItem("CarHireBookResponse")>
    Public Property CarHireBookings As New Generic.List(Of CarHire.BookResponse)

	Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)

End Class

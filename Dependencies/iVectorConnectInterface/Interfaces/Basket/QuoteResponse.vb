Imports System.Xml.Serialization

Namespace Basket
	<XmlType("BasketQuoteResponse")>
	<XmlRoot("BasketQuoteResponse")>
	Public Class QuoteResponse
		Inherits BasketResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse
		Public Property QuoteReference As String
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
	End Class

End Namespace
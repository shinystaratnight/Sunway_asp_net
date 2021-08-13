Imports System.Xml.Serialization

Namespace Basket

	<XmlType("BasketBookResponse")>
	<XmlRoot("BasketBookResponse")>
	Public Class BookResponse
		Inherits BookableBasketResponse

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
		<XmlArrayItem("AdHocPropertyBookResponse")>
		Public Property AdHocPropertyBookings As New Generic.List(Of AdHoc.PropertyBookResponse)
		<XmlArrayItem("OwnArrangement")>
		Public Property OwnArrangements As New Generic.List(Of Itinerary.OwnArrangementsResponse)

		Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)

		Public Property QuoteID As Integer

	End Class

End Namespace

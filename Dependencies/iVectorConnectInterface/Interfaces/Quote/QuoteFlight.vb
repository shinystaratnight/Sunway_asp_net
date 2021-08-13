Imports System.Xml.Serialization

Public Class QuoteFlight

	Public FlightQuoteID As Integer
	Public Status As String
	Public TotalPrice As Decimal
	Public TotalCommission As Decimal
	Public VATOnCommission As Decimal

	Public BaggagePrice As Decimal
	Public BaggageQuantity As Integer

	Public DepartureAirportID As Integer
	Public DepartureAirport As String

	Public ArrivalAirportID As Integer
	Public ArrivalAirport As String

	Public OutboundDepartureDate As Date
	Public OutboundDepartureTime As String
	Public OutboundArrivalDate As Date
	Public OutboundArrivalTime As String
	Public OutboundFlightCode As String

	Public ReturnArrivalDate As Date
	Public ReturnArrivalTime As String
	Public ReturnDepartureDate As Date
	Public ReturnDepartureTime As String
	Public ReturnFlightCode As String

	Public Source As String
	Public FlightCarrierID As Integer
	Public FlightCarrier As String
	Public OneWay As Boolean

    <XmlArray("FlightSectors")>
    <XmlArrayItem("FlightSector")>
    Public FlightSectors As List(Of QuoteFlightSector)

	<XmlArrayItem("GuestID")>
	Public GuestIDs As New Generic.List(Of Integer)

	Public ComponentRepriced As Boolean
    Public BookingToken As String
    Public QuotedTotalPrice As Decimal
	Public Property OutboundFlightClassID As Integer
	Public Property ReturnFlightClassID As Integer
	Public Property ShowDateOfBirth As Boolean
	Public Property PaymentsDue As New List(Of Support.PaymentDue)
	Public Property Cancellations As New List(Of Support.Cancellation)
	Public Property TermsAndConditions As String
	Public Property TermsAndConditionsURL As String
	Public Property Extras As New List(Of Flight.PreBookResponse.Extra)


End Class
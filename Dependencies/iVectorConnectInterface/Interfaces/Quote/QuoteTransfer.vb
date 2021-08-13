Imports System.Xml.Serialization

Public Class QuoteTransfer

    Public TransferQuoteID As Integer
    Public Source As String
    Public Status As String
    Public DepartureParentType As String
    Public DepartureParentID As Integer
    Public ArrivalParentType As String
    Public ArrivalParentID As Integer
    Public OneWay As Boolean

    Public DepartureDate As Date
    Public DepartureTime As String
    Public DepartureFlightCode As String
    Public DepartureNotes As String
    Public ReturnDate As Date
    Public ReturnTime As String
    Public ReturnFlightCode As String
    Public ReturnNotes As String
    Public Adults As Integer
    Public Children As Integer
    Public Infants As Integer
    Public VehicleClass As String
    Public Vehicle As String
    Public VehicleQuantity As Integer
    Public TotalPrice As Decimal
    Public TotalCommission As Decimal
    Public VATOnCommission As Decimal
    Public DepartureParentName As String
    Public ArrivalParentName As String

    Public ComponentRepriced As Boolean
    Public BookingToken As String

    Public QuotedTotalPrice As Decimal

    <XmlArrayItem("GuestID")>
    Public GuestIDs As New Generic.List(Of Integer)


    Public Property PaymentsDue As New List(Of Support.PaymentDue)
    Public Property Cancellations As New List(Of Support.Cancellation)
    Public Property TermsAndConditions As String
    Public Property TermsAndConditionsURL As String
End Class
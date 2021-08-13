Imports System.Xml.Serialization

Public Class QuoteCarHire

    Public CarHireQuoteID As Integer
    Public Source As String
    Public Status As String
    Public PickUpDepotID As Integer
    Public DropOffDepotID As Integer
    Public PickUpDate As Date
    Public PickUpTime As String
    Public DropOffDate As Date
    Public DropOffTime As String

    Public Adults As Integer
    Public Children As Integer
    Public Infants As Integer

    Public VehicleDescription As String
    Public VehicleImage As String
    Public CarInformation As String
    Public PickUpInformation As String
    Public DropOffInformation As String
    Public AdditionalInformation As String

    Public TotalPrice As Decimal
    Public TotalCommission As Decimal
    Public VATOnCommission As Decimal

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
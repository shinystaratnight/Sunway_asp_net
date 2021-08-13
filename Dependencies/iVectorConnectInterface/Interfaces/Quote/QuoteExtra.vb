Imports System.Xml.Serialization

Public Class QuoteExtra

    Public ExtraQuoteID As Integer
    Public ExtraName As String
    Public ExtraCategory As String
    Public ExtraID As Integer
    Public ExtraTypeID As Integer
    Public ExtraCategoryID As Integer
    Public ExtraDurationID As Integer
    Public Status As String
    Public StartDate As Date
    Public StartTime As String
    Public EndDate As Date
    Public EndTime As String
    Public Adults As Integer
    Public Children As Integer
    Public Infants As Integer
    Public Seniors As Integer
    Public TotalPrice As Decimal
    Public TotalCommission As Decimal
    Public VATOnCommission As Decimal
    Public Source As String

    Public ComponentRepriced As Boolean
    Public BookingToken As String
    Public QuotedTotalPrice As Decimal

    Public ExtraOptions As New Generic.List(Of iVectorConnectInterface.GetBookingDetailsResponse.Extra.ExtraOption)

    <XmlArrayItem("GuestID")>
    Public GuestIDs As New Generic.List(Of Integer)

    Public Property PaymentsDue As New List(Of Support.PaymentDue)
    Public Property Cancellations As New List(Of Support.Cancellation)
    Public Property TermsAndConditions As String
    Public Property TermsAndConditionsURL As String

End Class
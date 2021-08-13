Imports System.Xml.Serialization
Imports iVectorConnectInterface.GetBookingDetailsResponse

Public Class QuoteProperty

    Public PropertyQuoteID As Integer
    Public PropertyID As Integer
    Public Source As String
    Public PropertyReferenceID As Integer
    Public GeographyLevel1ID As Integer
    Public GeographyLevel2ID As Integer
    Public GeographyLevel3ID As Integer
    Public Status As String
    Public ArrivalDate As Date
    Public ReturnDate As Date
    Public Duration As Integer
    Public TotalPrice As Decimal
    Public TotalCommission As Decimal
    Public VATOnCommission As Decimal
    Public Rooms As New Generic.List(Of Room)
    Public Errata As New Generic.List(Of Erratum)
    <XmlArrayItem("Comment")>
    Public Comments As New Generic.List(Of String)

    Public ComponentRepriced As Boolean
    Public BookingToken As String
    Public QuotedTotalPrice As Decimal

    Public Property PaymentsDue As New List(Of Support.PaymentDue)
    Public Property Cancellations As New List(Of Support.Cancellation)
    Public Property TermsAndConditions As String
    Public Property TermsAndConditionsURL As String

End Class
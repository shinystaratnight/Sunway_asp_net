Imports System.Xml.Serialization
Imports iVectorConnectInterface.Basket.PreBookResponse
Imports iVectorConnectInterface.Interfaces

Public Class QuoteRetrieveResponse
    Implements iVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

    Public Property QuoteID As Integer
    Public Property QuoteReference As String
    Public Property QuotedDate As Date
    Public Property SystemUserID As Integer

    Public Property TradeContactID As Integer?
    Public Property TradeID As Integer?

    Public Property LeadCustomer As Support.LeadCustomerDetails
    Public Property Adults As Integer
    Public Property Children As Integer
    Public Property Infants As Integer
    Public Property FirstDepartureDate As Date
    Public Property LastReturnDate As Date
    Public Property TotalPrice As Decimal
    Public Property TotalCommission As Decimal
    Public Property VATOnCommission As Decimal

    Public Property GuestDetails As New List(Of Support.GuestDetail)

    <XmlArrayItem("Property")>
    Public Property Properties As List(Of QuoteProperty)
    <XmlArrayItem("Flight")>
    Public Property Flights As List(Of QuoteFlight)
    <XmlArrayItem("Transfer")>
    Public Property Transfers As List(Of QuoteTransfer)
    <XmlArrayItem("Extra")>
    Public Property Extras As List(Of QuoteExtra)
    <XmlArrayItem("CarHire")>
    Public Property CarHires As List(Of QuoteCarHire)
    <XmlArrayItem("AdHoc")>
    Public Property AdHocComponents As List(Of QuoteAdhocComponent)
    <XmlArrayItem("Cruise")>
    Public Property Cruises As Generic.List(Of QuoteCruise)

    Public Property BookingAdjustments As New List(Of BookingAdjustment)

End Class
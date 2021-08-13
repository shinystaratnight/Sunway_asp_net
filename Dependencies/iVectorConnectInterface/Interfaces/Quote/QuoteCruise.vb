Imports System.Xml.Serialization
Imports iVectorConnectInterface.Cruise

Public Class QuoteCruise

    Public Property ComponentRepriced As Boolean
    Public Property BookingToken As String
    Public Property TotalPrice As Decimal
    Public Property TotalCommission As Decimal
    Public Property VATOnCommission As Decimal
    Public Property SupplierReference As String
    Public Property Source As String
    Public Property ConfirmationStatus As String
    Public Property CruiseShipID As Integer
    Public Property ShipName As String
    Public Property DeparturePort As String
    Public Property DeparturePortID As Integer
    Public Property ReturnPort As String
    Public Property ReturnPortID As Integer
    Public Property Status As String
    Public Property ItineraryName As String
    Public Property ItineraryDetails As String
    Public Property DepartureDate As Date
    Public Property ReturnDate As Date
    Public Property Duration As Integer
    Public Property PortTaxes As Boolean
    Public Property GratuitiesIncluded As Boolean
    Public Property PayLocal As Boolean
    Public Property PayLocalTotal As Decimal
    Public Property QuotedTotalPrice As Decimal
    Public Property Cancellations As New List(Of Support.Cancellation)
    Public Property PaymentsDue As New List(Of Support.PaymentDue)
    Public Property Cabins As New List(Of Cabin)


End Class


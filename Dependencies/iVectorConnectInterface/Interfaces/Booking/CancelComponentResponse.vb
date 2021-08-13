Imports System.Xml.Serialization


Public Class CancelComponentResponse

    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

    Public Property BookingReference As String
    Public Property TotalPrice As Decimal
    Public Property TotalCommission As Decimal
    Public Property VATOnCommission As Decimal

    Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)

End Class

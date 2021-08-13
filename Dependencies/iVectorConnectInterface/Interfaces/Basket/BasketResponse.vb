Namespace Basket

    Public Class BasketResponse
        Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse
        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus
        Public Property TotalPrice As Decimal
        Public Property TotalCommission As Decimal
        Public Property VATOnCommission As Decimal
    End Class

End Namespace
Public Class ApplyPromoCodeResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

    Public Property Discount As Decimal
    Public Property Warning As String

End Class

Public Class CustomerLoginResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property CustomerID As Integer
    Public Property BookingID As Integer

    Public Property BrandID As Integer

End Class

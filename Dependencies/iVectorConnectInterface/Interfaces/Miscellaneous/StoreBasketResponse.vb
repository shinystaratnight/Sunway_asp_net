Public Class StoreBasketResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
	Public BasketStoreID As Integer
	Public BookingReference As String

End Class

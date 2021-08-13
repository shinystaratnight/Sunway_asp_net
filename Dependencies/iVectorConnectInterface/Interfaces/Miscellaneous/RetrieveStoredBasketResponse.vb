Public Class RetrieveStoredBasketResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
	Public BasketXML As String

End Class

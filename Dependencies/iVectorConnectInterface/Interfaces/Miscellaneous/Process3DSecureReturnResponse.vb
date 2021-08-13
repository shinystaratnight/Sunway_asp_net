Public Class Process3DSecureReturnResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
	Public Property ThreeDSecureCode As String
	Public Property PaymentToken As String

End Class

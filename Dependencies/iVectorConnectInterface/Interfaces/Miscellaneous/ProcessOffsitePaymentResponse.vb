Public Class ProcessOffsitePaymentRedirectResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public AuthorisationCode As String = ""

End Class

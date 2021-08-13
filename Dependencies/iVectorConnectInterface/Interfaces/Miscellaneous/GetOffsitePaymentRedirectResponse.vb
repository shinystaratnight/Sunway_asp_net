Public Class GetOffsitePaymentRedirectResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public RedirectURL As String = ""
	Public HTML As String = ""
	Public Reference1 As String = ""
	Public Reference2 As String = ""

End Class

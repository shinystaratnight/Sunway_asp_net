Public Class Get3DSecureRedirectResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property RequiresSecureAuthenticationSuccess As Boolean
    Public Property Enrollment As Boolean
    Public Property HTMLData As String
    Public Property PaymentToken As String

End Class

Public Class ResponseStatus
	Implements Interfaces.iVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

    Public Sub New()

    End Sub

    Public Sub New(ByVal warnings As List(Of String))
        ReturnStatus.Success = False
        ReturnStatus.Exceptions = warnings
    End Sub

End Class
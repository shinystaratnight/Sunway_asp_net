
Public Class TranslationResponse
    Implements Interfaces.IVectorConnectResponse


    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
	Public Property TranslationAvailable As Boolean = False
    Public Property TranslationText As String

End Class

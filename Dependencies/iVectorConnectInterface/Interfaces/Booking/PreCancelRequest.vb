Public Class PreCancelRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

   Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property BookingReference As String

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'booking reference
        If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be specified.")

        Return aWarnings

    End Function

#End Region

End Class

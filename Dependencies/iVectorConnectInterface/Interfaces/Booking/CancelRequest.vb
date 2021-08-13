Public Class CancelRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

  
#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property BookingReference As String
	Public Property CancellationCost As Decimal = -1
	Public Property CancellationToken As String
    Public Property CancellationReason As String

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'booking reference
        If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be specified.")

        'cancellation cost and token
        If Me.CancellationCost < 0 Then aWarnings.Add("A cancellation cost must be specified.")
        If Me.CancellationToken = "" Then aWarnings.Add("A cancellation token must be specified.")

        Return aWarnings

    End Function

#End Region

End Class

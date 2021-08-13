Public Class PreCancelResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property BookingReference As String
	Public Property CancellationCost As Decimal
	Public Property CancellationToken As String
	Public Property SupplierDetails As New SupplierDetail

#Region "Helper Classes"

	Public Class SupplierDetail
		Public Property SupplierID As Integer
		Public Property CurrencyID As Integer
		Public Property Cost As Decimal
	End Class

#End Region

End Class


Public Class ReserveSeatsResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property BookingReference As String
	Public Property TotalPrice As Decimal
	Public Property TotalCommission As Decimal
	Public Property VATOnCommission As Decimal

	Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)

#End Region

End Class

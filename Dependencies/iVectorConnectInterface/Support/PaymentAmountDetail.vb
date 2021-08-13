Namespace Support

	Public Class PaymentAmountDetail
		Public DueNowPaymentAmount As Decimal = 0
		Public TotalPaymentAmount As Decimal = 0
		Public FlightSupplierPaymentAmount As Decimal = 0
		Public FlightSupplierFee As Decimal = 0
		Public CreditCardFeeDetails As New List(Of CreditCardFeeDetail)
	End Class

End Namespace
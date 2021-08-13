Namespace Support

	Public Class PaymentDue
		Public Property Amount As Decimal
		Public Property DateDue As Date

		Public Sub New()
		End Sub

		Public Sub New(ByVal Amount As Decimal, ByVal DateDue As Date)
			Me.Amount = Amount
			Me.DateDue = DateDue
		End Sub

	End Class

End Namespace
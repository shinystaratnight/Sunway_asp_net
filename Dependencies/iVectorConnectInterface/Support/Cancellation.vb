Namespace Support

	Public Class Cancellation
		Public Property StartDate As Date
		Public Property EndDate As Date
		Public Property Amount As Decimal

		Public Sub New()
		End Sub

		Public Sub New(ByVal StartDate As Date, ByVal EndDate As Date, ByVal Amount As Decimal)
			Me.StartDate = StartDate
			Me.EndDate = EndDate
			Me.Amount = Decimal.Round(Amount, 2)
		End Sub

	End Class

End Namespace
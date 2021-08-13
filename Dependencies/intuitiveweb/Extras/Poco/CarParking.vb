
Namespace Extras.Poco

	Public Class CarParking
		Public Title As String
		Public SubTitle As String
		Public Price As Decimal
		Public Property WasPrice As Decimal
			Get
				If DiscountPercentage <> 0 Then
					Return Math.Ceiling(Price * (1 + (DiscountPercentage / 100)))
				Else
					Return Price
				End If
			End Get
			Set(value As Decimal)
			End Set
		End Property
		Public Key As String
		Public BookingToken As String
		Private DiscountPercentage As Decimal
		
		Public Information As New CarParkingInformation


		Public Sub SetDiscountPercentage(Percentage As Decimal)
			Me.DiscountPercentage = Percentage
		End Sub

		Public Class CarParkingInformation
			Public Description As String
			Public TransferTime As String
			Public MapUrl As String
			Public Details As New List(Of DetailInformation)

			Public Class DetailInformation
				Public Detail As String

				Public Sub New()
				End Sub
				Public Sub New(Detail As String)
					Me.Detail = Detail
				End Sub
			End Class

		End Class
	End Class



End Namespace

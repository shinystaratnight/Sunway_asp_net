<Serializable()>
Public Class Quote

	Public Property QuoteReference As String
	Public Property TradeReference As String
	Public Property Status As String
	Public Property AccountStatus As String
	Public Property LeadCustomerFirstName As String
	Public Property LeadCustomerLastName As String
	Public Property TotalPax As Integer
	Public Property BookingDate As Date
	Public Property ArrivalDate As Date
	Public Property Duration As Integer
	Public Property ArrivalAirportID As Integer
	Public Property DepartureAirportID As Integer
	Public Property GeographyLevel1ID As Integer
	Public Property GeographyLevel2ID As Integer
	Public Property GeographyLevel3ID As Integer
	Public Property LastReturnDate As Date
	Public Property TotalPrice As Decimal
	Public Property TotalCommission As Decimal
	Public Property ComponentSummary As Generic.List(Of Component)

End Class
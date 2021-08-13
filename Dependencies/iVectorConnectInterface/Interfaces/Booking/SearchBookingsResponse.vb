Public Class SearchBookingsResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
	Public Property Bookings As New Generic.List(Of Booking)

#End Region

#Region "Helper Classes"

	<Serializable()>
 Public Class Booking

		Public Property BookingReference As String
		Public Property TradeReference As String
		Public Property Status As String
		Public Property AccountStatus As String
		Public Property LeadCustomerFirstName As String
		Public Property LeadCustomerLastName As String
		Public Property TotalPax As Integer
		Public Property BookingDate As Date
		Public Property ArrivalDate As Date
		Public Property Duration As Integer
		Public Property GeographyLevel1ID As Integer
		Public Property GeographyLevel2ID As Integer
		Public Property GeographyLevel3ID As Integer
		Public Property hlpGeographyLevel3Name As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property TotalPaid As Decimal
		Public Property TotalOutstanding As Decimal
		Public Property ArrivalDateReached As Boolean = False
		Public Property TotalVATOnCommission As Decimal
		Public Property CustomerCurrencyID As Integer
		Public Property CurrencySymbolPosition As String
		Public Property CurrencySymbol As String


		Public Function ShouldSerializeArrivalDateReached() As Boolean
			Return Me.ArrivalDateReached
		End Function

		Public Property ComponentSummary As Generic.List(Of Component)


		Public Class Component
			Public Property ComponentType As String
			Public Property Status As String
			Public Property Reference As String
			Public Property hlpComponentName As String
            Public Property ComponentBookingID As Integer
			Public Property FailedComponent As Boolean
		End Class
	End Class

#End Region

End Class

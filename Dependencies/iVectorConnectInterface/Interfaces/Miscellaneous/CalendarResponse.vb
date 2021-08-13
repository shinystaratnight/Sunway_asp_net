Public Class CalendarSearchResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property Days As New Generic.List(Of Day)

#End Region

#Region "Helper Classes"

	Public Class Day

		Public Property [Date] As Date
		Public TotalPrice As Decimal

		'for property and package searches
		Public PropertyReferenceID As Integer
		Public MealBasisID As Integer

		'for package searches
		Public PackageReference As String

		'for flights and packages
		Public DepartureAirportID As Integer
		Public ArrivalAirportID As Integer
		Public Duration As Integer

	End Class

#End Region

End Class

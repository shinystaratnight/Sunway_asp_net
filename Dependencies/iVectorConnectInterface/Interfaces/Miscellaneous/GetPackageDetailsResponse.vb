Public Class GetPackageDetailsResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus


	Public Property DepartureAirportID As Integer
	Public Property ArrivalAirportID As Integer
	Public Property DepartureDate As Date
	Public Property Duration As Integer
	Public Property GeographyLevel3ID As Integer
	Public Property PropertyReferenceID As Integer
	Public Property PropertyRoomTypeID As Integer
	Public Property MealBasisID As Integer
	Public Property Adults As Integer
	Public Property Children As Integer
	Public Property Infants As Integer
	Public Property BaggageCostDeduction As Decimal
	Public Property TotalPrice As Decimal


End Class

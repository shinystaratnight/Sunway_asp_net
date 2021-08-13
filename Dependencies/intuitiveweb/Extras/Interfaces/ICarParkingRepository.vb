Imports Intuitive.Web.Extras.Poco

Namespace Extras.Interfaces

	Public Interface ICarParkingRepository

		Function List(ExtraTypeID As Integer) As List(Of CarParking)

	End Interface

End Namespace
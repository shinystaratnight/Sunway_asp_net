Imports Intuitive.Web.Extras.Interfaces
Imports Intuitive.Web.Extras.Poco
Imports ivci = iVectorConnectInterface

Namespace Extras.Repositories

	Public Class CarParkingRepository
		Implements ICarParkingRepository

		Public Function List(ExtraTypeID As Integer) As List(Of CarParking) Implements ICarParkingRepository.List
			Dim oResults As Generic.List(Of ivci.Extra.SearchResponse.ExtraType) = _
			BookingBase.SearchDetails.ExtraResults.FulliVectorConnectResults

			Dim carParkings As New List(Of CarParking)

			For Each oResult As Extra.SearchResponse.ExtraType In oResults.Where(Function(o) o.ExtraTypeID = ExtraTypeID)

				For Each oExtraResult As Extra.SearchResponse.ExtraSubType In oResult.ExtraSubTypes
					For Each oExtra As Extra.SearchResponse.Extra In oExtraResult.Extras
						For Each oOption As Extra.SearchResponse.Option In oExtra.Options

							Dim sExtraKey As String = Intuitive.Functions.SafeString(oResult.ExtraTypeID &
																				 "-" &
																				 oResult.ExtraSubTypes.IndexOf(oExtraResult) &
																				  "-" &
																				   oExtraResult.Extras.IndexOf(oExtra) &
																				   "-" &
																				   oExtra.Options.IndexOf(oOption))

							Dim carParking As New CarParking

							'build up object from result
							carParking.Title = oExtra.ExtraName
							carParking.SubTitle = oExtra.ExtraSubName
							carParking.Price = oOption.TotalPrice
							carParking.Key = sExtraKey
							carParking.BookingToken = oOption.BookingToken

							'TODO Replace all hardcoded information when connect results finished
							carParking.Information.Description = oOption.Description
							carParking.Information.TransferTime = oOption.CarParkInformation.TransferTime
							carParking.Information.MapUrl = oOption.CarParkInformation.MapURL

							For Each oGenericDetail As String In oOption.GenericDetails
								carParking.Information.Details.Add(New CarParking.CarParkingInformation.DetailInformation(oGenericDetail))
							Next
							carParking.SetDiscountPercentage(oOption.PriceChangePercentage)

							'carParking.Title = oExtra.ExtraName
							'carParking.SubTitle = "Test Subtitle"
							'carParking.Price = oOption.TotalPrice
							'carParking.Key = sExtraKey
							''carParking.BookingToken = oOption.BookingToken

							''TODO Replace all hardcoded information when connect results finished
							'carParking.Information.Description = "The Best Value Parking at the Airport"
							'carParking.Information.TransferTime = "2 minute walk"
							'carParking.Information.MapUrl = "http://adt.adtrez.com//rz_uploads/P2%20Map.jpg"
							'carParking.Information.Details.Add(New CarParking.CarParkingInformation.DetailInformation("The lowest price but no cancellation or amends"))
							'carParking.Information.Details.Add(New CarParking.CarParkingInformation.DetailInformation("The nearest car park at the airport"))
							'carParking.Information.Details.Add(New CarParking.CarParkingInformation.DetailInformation("You can check in within minutes"))
							'carParking.SetDiscountPercentage(20)

							carParkings.Add(carParking)
						Next
					Next
				Next

			Next

			Return carParkings

		End Function

		'Public Sub Remove(Identifier As String, Id As Integer) Implements ICarParkingRepository.Remove

		'End Sub

		'Public Function Find(Identifier As String, Id As Integer) As CarParking Implements ICarParkingRepository.Find

		'End Function



	End Class

End Namespace
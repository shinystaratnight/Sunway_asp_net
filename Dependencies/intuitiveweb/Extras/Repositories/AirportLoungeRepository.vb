Imports ivci = iVectorConnectInterface
Imports Intuitive.Web.Extras.Interfaces
Imports Intuitive.Web.Extras.Poco
Imports Intuitive.Functions

Namespace Extras.Repositories

    Public Class AirportLoungeRepository
        Implements Extras.Interfaces.IAirportLoungeRepository

        Public Function List(ExtraTypeID As Integer) As List(Of AirportLounge) Implements IAirportLoungeRepository.List

            Dim oResults As Generic.List(Of ivci.Extra.SearchResponse.ExtraType) = _
            BookingBase.SearchDetails.ExtraResults.FulliVectorConnectResults
            Dim lounges As New List(Of AirportLounge)

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

							Dim lounge As New AirportLounge

							lounge.Title = oExtra.ExtraName
							lounge.UnitPrice = SafeDecimal(oOption.TotalPrice)
							lounge.Description = oExtra.ExtraSubName
							lounge.MinPassengers = 0
							lounge.MaxPassengers = 20
							lounge.Key = sExtraKey
							lounge.BookingToken = oOption.BookingToken
                            'lounge.Details.Add(New AirportLounge.DetailInformation("Enjoy comfortable seating and free WiFi"))
                            'lounge.Details.Add(New AirportLounge.DetailInformation("Help yourself to drinks and a snack"))
                            'lounge.Details.Add(New AirportLounge.DetailInformation("Children under 12 not permitted"))


                            For Each oGenericDetail As String In oOption.GenericDetails
								lounge.Details.Add(New AirportLounge.DetailInformation(oGenericDetail))
							Next

							lounges.Add(lounge)
						Next
					Next
				Next

			Next

			Return lounges

        End Function

    End Class


End Namespace

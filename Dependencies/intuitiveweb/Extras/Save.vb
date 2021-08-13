Imports ivci = iVectorConnectInterface

Namespace Extras

	Public Class Save


		Public Sub Go(Identifier As String,
					  Results As Generic.List(Of ivci.Extra.SearchResponse.ExtraType),
					  ExtraSearch As BookingExtra.ExtraSearch)

			BookingBase.SearchDetails.ExtraResults.Save(Identifier, Results)

		End Sub


	End Class


End Namespace

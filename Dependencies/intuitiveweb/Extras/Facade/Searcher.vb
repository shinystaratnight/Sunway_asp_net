
Namespace Extras.Facade

	Public Class Searcher

		Private Context As HttpContext

		Public Sub New(Context As HttpContext)
			Me.Context = Context
		End Sub

		Public Function Go() As String

			Dim oExtraSearch As New Extras.Search(Me.Context)
			Dim oExtraReturn As Extras.Search.SearchReturn = oExtraSearch.Go()


			'it transpires the Save routine runs as part of the ExtraSearchFromBasket method
			'we may split this out but currently it is working like this

			'Dim oExtraSave As New Extras.Save()
			'oExtraSave.Go(oExtraReturn.Identifier, oExtraReturn.Results)

			Return oExtraReturn.Identifier

		End Function

	End Class


End Namespace

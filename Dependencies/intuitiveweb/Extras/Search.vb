
Imports Intuitive.Functions
Imports ivci = iVectorConnectInterface

Namespace Extras


	Public Class Search

		Private Const SESSION_BASKET_CHECK_KEY As String = "__session_basket_check"
		Private Const DEFAULT_SEARCH_IDENTIFIER As String = "__default_extra_search"
		'Private SearchStatus As Status = Status.Unsearched
		Private Const SEARCH_STATUS_KEY As String = "__extra_search_status_key"
		Private Context As HttpContext

		Public Sub New(Context As HttpContext)
			Me.Context = Context
		End Sub

		'this should accept a strategy and execute it - may need to refactor in the future to do this
		Public Function Go() As SearchReturn
			Return Me.SendSearch(DEFAULT_SEARCH_IDENTIFIER)
		End Function


		Public Function Go(Identifier As String) As SearchReturn
			Return Me.SendSearch(Identifier)
		End Function


		Private Function SendSearch(Identifier As String) As SearchReturn

			Dim oBasket As BookingBasket = BookingBase.SearchBasket


			If SafeEnum(Of Status)(Me.Context.Items(SEARCH_STATUS_KEY)) = Status.Unsearched AndAlso oBasket.SearchExtras Then

                BookingBase.SearchDetails.ExtraResults.ClearWorkTable(Identifier)

                'this shouldnt happen here - should be using the extras visitor
                'quick hack to get ADT-567 out of the door 
                BookingBase.SearchBasket.BasketExtras.Clear()

                Dim oExtraSearchReturn As BookingSearch.SearchReturn =
                    BookingBase.SearchDetails.ExtraSearchFromBasket(Identifier)
                Me.Context.Items(SEARCH_STATUS_KEY) = Status.Completed

                oBasket.SearchExtras = False

                Return New SearchReturn(Identifier, oExtraSearchReturn.ExtraResults)

            End If

            'TODO - don't return nothing!! should return a null object 
            Return New SearchReturn(Identifier, Nothing)

        End Function


		Private Enum Status
			Unsearched
			Completed
		End Enum

		Public Class SearchReturn
			Public Sub New(Identifier As String, Results As Generic.List(Of ivci.Extra.SearchResponse.ExtraType))
				Me.Identifier = Identifier
				Me.Results = Results
			End Sub
			Public Identifier As String
			Public Results As Generic.List(Of ivci.Extra.SearchResponse.ExtraType)
		End Class



	End Class


End Namespace

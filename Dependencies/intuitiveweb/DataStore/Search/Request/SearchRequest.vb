
Namespace DataStore.Search.Request

    Public Class SearchRequest

        Public [Type] As String
        Public ResultCount As Integer
        Public RequestTime As Double
        Public ApiTime As Double
        Public SearchTime As Double
        Public NetworkTime As Double
        Public SaveTime As Double
        Public FilterTime As Double
        Public SortTime As Double
        Public OtherSetupTime As Double

        Public Sub Setup(RequestStrategy As IRequestStrategy, SearchDetails As BookingSearch)

            RequestStrategy.Setup(SearchDetails, Me)

        End Sub

    End Class


End Namespace

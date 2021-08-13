
Namespace DataStore.Search.Request

    Public Class FlightRequest
        Implements IRequestStrategy

        Public Sub Setup(SearchDetails As BookingSearch, ByRef SearchRequest As SearchRequest) Implements IRequestStrategy.Setup

            SearchRequest.Type = "Flight"
            SearchRequest.ApiTime = SearchDetails.FlightResults.RequestDiagnostic.ApiTime / 1000
            SearchRequest.NetworkTime = SearchDetails.FlightResults.RequestDiagnostic.NetworkTime / 1000
            SearchRequest.SearchTime = SearchDetails.FlightResults.RequestDiagnostic.SearchTime / 1000
            SearchRequest.RequestTime = SearchDetails.FlightResults.RequestDiagnostic.RequestTime / 1000
            SearchRequest.ResultCount = SearchDetails.FlightResults.TotalFlights
            SearchRequest.SaveTime = GetTime(SearchDetails, "IntuitiveWeb|Save flight results|Main")
            SearchRequest.FilterTime = GetTime(SearchDetails, "IntuitiveWeb|Filter flight results|Main")
            SearchRequest.SortTime = GetTime(SearchDetails, "IntuitiveWeb|Sort flight results|Main")

        End Sub

        Private Function GetTime(SearchDetails As BookingSearch, Identifier As String) As Double

            Try
                Dim oTimerItem As Intuitive.ProcessTimer.TimerItem = SearchDetails.ProcessTimer.Times(Identifier)
                Dim oTime As Double = (oTimerItem.EndTicks - oTimerItem.StartTicks) / System.TimeSpan.TicksPerSecond
                Return oTime
            Catch ex As Exception
                'in case the identifiers have been changed - we don't want the app to fall over
                Return 0
            End Try

        End Function

    End Class

End Namespace

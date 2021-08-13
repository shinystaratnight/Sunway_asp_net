
Namespace DataStore.Search.Request

    Public Class PropertyRequest
        Implements IRequestStrategy

        Public Sub Setup(SearchDetails As BookingSearch, ByRef SearchRequest As SearchRequest) Implements IRequestStrategy.Setup

                SearchRequest.Type = "Hotel"
                SearchRequest.ApiTime = SearchDetails.PropertyResults.RequestDiagnostic.ApiTime / 1000
                SearchRequest.NetworkTime = SearchDetails.PropertyResults.RequestDiagnostic.NetworkTime / 1000
                SearchRequest.SearchTime = SearchDetails.PropertyResults.RequestDiagnostic.SearchTime / 1000
                SearchRequest.RequestTime = SearchDetails.PropertyResults.RequestDiagnostic.RequestTime / 1000
                SearchRequest.ResultCount = SearchDetails.PropertyResults.TotalHotels
                SearchRequest.SaveTime = GetTime(SearchDetails, "IntuitiveWeb|Save property results|Main")
                SearchRequest.FilterTime = GetTime(SearchDetails, "IntuitiveWeb|Filter property results|Main")
                SearchRequest.SortTime = GetTime(SearchDetails, "IntuitiveWeb|Sort property results|Main")
            SearchRequest.OtherSetupTime = GetTime(SearchDetails, "IntuitiveWeb|Setup selected flight|Main")

        End Sub

        Private Function GetTime(SearchDetails As BookingSearch, Identifier As String) As Double

            Try
                Dim oTimerItem As Intuitive.ProcessTimer.TimerItem = SearchDetails.ProcessTimer.Times(Identifier)
                Dim oTime As Double = (oTimerItem.EndTicks - oTimerItem.StartTicks) / System.TimeSpan.TicksPerSecond
                Return oTime
            Catch ex As Exception
                ' in case identifiers have been changed
                'or hotel only - won't set up the selected flight
                Return 0
            End Try

        End Function

    End Class

End Namespace
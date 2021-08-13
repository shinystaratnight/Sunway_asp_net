
Namespace DataStore.Search.Departure

    Public Class DepartureDetails
        Public ParentType As String
        Public ParentId As Integer

        Public Sub Setup(DepartureStrategy As IDepartureStrategy, Id As Integer)

            DepartureStrategy.Implement(Id, Me)

        End Sub

    End Class

End Namespace

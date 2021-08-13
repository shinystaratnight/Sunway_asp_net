
Namespace DataStore.PageRequest

    Public NotInheritable Class PageRequestLog
        Public Sub New()
        End Sub
        Public Sub New(RequestTimes As List(Of Logging.RequestTime),
                       WidgetSpeeds As List(Of Logging.WidgetSpeed))
            Me.WidgetSpeeds = WidgetSpeeds
            Me.RequestTimes = RequestTimes
        End Sub

        Public RequestTimes As New List(Of Logging.RequestTime)
        Public WidgetSpeeds As New List(Of Logging.WidgetSpeed)
    End Class


End Namespace

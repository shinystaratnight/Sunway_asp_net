Namespace Flight.FareFamily

    ''' <summary>
    ''' Airline Fare Family Service Group
    ''' </summary>
    Public Class ServiceGroup

        ''' <summary>
        ''' Gets or sets the name of the service group.
        ''' </summary>
        ''' <value>
        ''' The name of the service group.
        ''' </value>
        Public Property ServiceGroupName As String

        ''' <summary>
        ''' Gets or sets the service group inclusion.
        ''' </summary>
        ''' <value>
        ''' The service group inclusion.
        ''' </value>
        Public Property ServiceGroupInclusion As String

        ''' <summary>
        ''' Gets or sets the services.
        ''' </summary>
        ''' <value>
        ''' The services.
        ''' </value>
        Public Property Services As List(Of Service)

    End Class

End Namespace
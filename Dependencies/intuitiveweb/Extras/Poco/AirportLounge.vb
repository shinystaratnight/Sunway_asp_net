
Namespace Extras.Poco

    Public Class AirportLounge

        Public Title As String
        Public Description As String
        Public MaxPassengers As Integer
        Public MinPassengers As Integer
        Public UnitPrice As Decimal
        Public Details As New List(Of DetailInformation)
        Public Key As String
        Public BookingToken As String

        Public Class DetailInformation
            Public Detail As String

            Public Sub New()
            End Sub
            Public Sub New(Detail As String)
                Me.Detail = Detail
            End Sub
        End Class


    End Class


End Namespace

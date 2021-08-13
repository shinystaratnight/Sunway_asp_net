Namespace Flight.FareFamily

    ''' <summary>
    ''' Airline Fare Family 
    ''' </summary>
    Public Class FareFamily

        ''' <summary>
        ''' The fare family name'
        ''' </summary>
        Public FareFamilyName As String

        ''' <summary>
        ''' The fare family services'
        ''' </summary>
        Public FareFamilyServices As List(Of ServiceGroup)

        ''' <summary>
        ''' The additional cost.
        ''' Additional cost of fare relative to the Cost of the Fare returned at search/ pre-book.
        ''' </summary>
        Public AdditionalCost As Decimal

    End Class

End Namespace

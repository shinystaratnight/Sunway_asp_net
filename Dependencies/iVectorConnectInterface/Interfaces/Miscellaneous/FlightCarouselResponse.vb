Public Class FlightCarouselResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

    Public Property Dates As New Generic.List(Of [Date])

#End Region

#Region "Helper Classes"

	Public Class [Date]

        Public Property [Date] As Date
        Public Property AvailableFlights As Integer
        Public Property FlightFromPrice As Decimal
        Public Property PropertyFromPrice As Decimal

	End Class

#End Region

End Class

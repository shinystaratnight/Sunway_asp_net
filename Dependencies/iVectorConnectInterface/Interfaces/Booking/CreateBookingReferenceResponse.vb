
Public Class CreateBookingReferenceResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
    Public Property BookingReference As String

End Class

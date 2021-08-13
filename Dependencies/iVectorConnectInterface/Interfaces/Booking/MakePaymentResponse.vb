Public Class MakePaymentResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse


    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
    Public BookingBalance As Decimal
   
End Class

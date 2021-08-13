Public Class MakePaymentRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest


    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
    Public Property BookingReference As String
    Public Property Payment As Support.PaymentDetails


    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)


        If Me.BookingReference = "" Then aWarnings.Add("A Booking Reference must be specified.")
        If Me.Payment Is Nothing Then aWarnings.Add("Payment details must be specified.")

        'payment 
        If Not Me.Payment Is Nothing Then

            aWarnings.AddRange(Me.Payment.Validate)

        End If


        Return aWarnings

    End Function
End Class

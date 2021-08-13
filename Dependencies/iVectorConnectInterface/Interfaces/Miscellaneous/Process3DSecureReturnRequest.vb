Public Class Process3DSecureReturnRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property URL As String
	Public Property QueryString As String
	Public Property FormValues As String
	Public Property Headers As New List(Of Header)
	Public Property Cookies As New List(Of Cookie)
	Public Property Body As String
	Public Property Payment As Support.PaymentDetails


	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		If Me.Payment Is Nothing Then aWarnings.Add("Payment details must be specified.")

		'payment 
		If Not Me.Payment Is Nothing Then

			aWarnings.AddRange(Me.Payment.Validate)

		End If

		Return aWarnings

	End Function

#Region "Helper Class"

	Public Class Header
		Public Property Name As String
		Public Property Value As String
	End Class

	Public Class Cookie
		Public Property Name As String
		Public Property Value As String
	End Class

#End Region

End Class

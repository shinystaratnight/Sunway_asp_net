Public Class StoreBasketRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public BookingReference As String
	Public BasketXML As String
	Public BasketStoreID As Integer = 0

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'If Me.BookingReference = "" AndAlso Me.BasketStoreID <= 0 Then aWarnings.Add("A booking reference or basket store id must be specified.")
		If Me.BasketXML = "" Then aWarnings.Add("Some basket XML must be specified.")

		Return aWarnings

	End Function

#End Region

End Class

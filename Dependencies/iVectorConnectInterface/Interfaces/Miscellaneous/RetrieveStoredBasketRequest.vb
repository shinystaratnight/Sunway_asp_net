Public Class RetrieveStoredBasketRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public BookingReference As String
    Public BasketStoreID As Integer = 0
    Public ClearBasket As Boolean = False

#Region "Validate"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		If Me.BookingReference = "" AndAlso Me.BasketStoreID = 0 Then
			aWarnings.Add("Either a booking reference or a basket store ID needs to be specified.")
		End If
		
		Return aWarnings

	End Function

#End Region

End Class

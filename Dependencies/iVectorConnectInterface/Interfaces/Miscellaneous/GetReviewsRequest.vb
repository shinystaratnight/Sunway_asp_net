
Public Class GetReviewsRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property PropertyReferenceID As Integer
	Public Property SalesChannelID As Integer
	Public Property BrandID As Integer
	Public Property LanguageID As Integer

#End Region

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		If Me.LoginDetails Is Nothing Then aWarnings.Add("Login details must be provided.")

		If Me.PropertyReferenceID = 0 Then aWarnings.Add("A Property Reference ID must be provided.")
		If Me.SalesChannelID = 0 Then aWarnings.Add("A Sales Channel ID must be provided.")
		If Me.BrandID = 0 Then aWarnings.Add("A Brand ID must be provided.")
		If Me.LanguageID = 0 Then aWarnings.Add("A Language ID must be provided.")

		Return aWarnings

	End Function

#End Region

End Class
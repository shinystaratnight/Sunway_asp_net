Imports Intuitive.DateFunctions

Public Class PostcodeLookupRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property Postcode As String
	Public Property Customer As String
	Public Property GeographyLevel1ID As Integer


#End Region

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'login details
		If Me.LoginDetails Is Nothing Then aWarnings.Add("Login details must be provided.")

		'postcode
		If Me.Postcode = "" Then aWarnings.Add("A Postcode must be provided.")

		Return aWarnings

	End Function

#End Region


End Class

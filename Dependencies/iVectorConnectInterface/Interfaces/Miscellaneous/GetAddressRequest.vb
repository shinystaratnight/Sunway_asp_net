Imports Intuitive.DateFunctions

Public Class GetAddressRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property BuildingID As String
	Public Property Description As String



#End Region

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'login details
		If Me.LoginDetails Is Nothing Then aWarnings.Add("Login details must be provided.")

		'building id
		If Me.BuildingID = "" Then aWarnings.Add("A Building ID must be provided.")

		Return aWarnings

	End Function

#End Region


End Class

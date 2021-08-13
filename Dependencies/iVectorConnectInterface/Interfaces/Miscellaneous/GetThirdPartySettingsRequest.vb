Public Class GetThirdPartySettingsRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property ThirdParty As String


#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		If Me.ThirdParty = "" Then aWarnings.Add("A third party must be specified.")

		Return aWarnings

	End Function

#End Region

End Class

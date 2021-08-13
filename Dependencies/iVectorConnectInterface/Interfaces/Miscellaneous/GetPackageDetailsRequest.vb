Public Class GetPackageDetailsRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property PackageReference As String

	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
		Dim aWarnings As New Generic.List(Of String)
		If Me.PackageReference Is Nothing OrElse Me.PackageReference = "" Then aWarnings.Add("A package reference must be supplied")
		Return aWarnings
	End Function
End Class

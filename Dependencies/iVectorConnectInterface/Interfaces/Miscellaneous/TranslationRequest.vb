
Public Class TranslationRequest
    Implements Interfaces.IVectorConnectRequest


	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property LanguageID As Integer = 0
	Public Property Group As String
    Public Property Key As String
	Public Property SubKey As String = ""



    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
		Dim aWarnings As New Generic.List(Of String)

		If Me.Group = "" Then aWarnings.Add("A Group must be specified.")
		If Me.Key = "" Then aWarnings.Add("A Key must be specified.")

		Return aWarnings

	End Function
End Class

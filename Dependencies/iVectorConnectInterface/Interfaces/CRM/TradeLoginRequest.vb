Imports Intuitive.Validators

Public Class TradeLoginRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
 
    Public Property Email As String
    Public Property UserName As String
    Public Property Password As String
    Public Property WebsitePassword As String


#Region "Validation"
    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

		'email/username
		If Me.UserName = "" AndAlso Me.Email = "" AndAlso Me.LoginDetails.AgentReference = "" Then
			aWarnings.Add("A username or valid email address must be specified.")
		End If


		'passwords
		If Me.WebsitePassword = "" AndAlso Me.Password = "" Then
			aWarnings.Add("A password must be specified")
		End If

        Return aWarnings

    End Function
#End Region


End Class

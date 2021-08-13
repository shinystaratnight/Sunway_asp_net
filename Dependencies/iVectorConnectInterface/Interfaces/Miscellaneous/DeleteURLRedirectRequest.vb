Imports iVectorConnectInterface.Interfaces

Public Class DeleteURLRedirectRequest
    Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

    Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
    Public Property RedirectID As Integer

    Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

        Dim oWarnings As New List(Of String)

        If Me.RedirectID = 0 Then
            oWarnings.Add("A valid RedirectID must be specified")
        End If

        Return oWarnings

    End Function
End Class

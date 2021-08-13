Imports iVectorConnectInterface.Interfaces

Public Class ModifyURLRedirectRequest
    Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

    Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
    Public Property RedirectID As Integer
    Public Property OldURL As String
    Public Property CurrentURL As String

    Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

        Dim oWarnings As New List(Of String)

        If Me.RedirectID = 0 Then
            oWarnings.Add("A valid RedirectID must be specified")
        End If

        If String.IsNullOrEmpty(Me.OldURL) AndAlso String.IsNullOrEmpty(Me.CurrentURL) Then
            oWarnings.Add("At least one of the Old URL and the Current URL must be specified")
        End If

        Return oWarnings

    End Function
End Class

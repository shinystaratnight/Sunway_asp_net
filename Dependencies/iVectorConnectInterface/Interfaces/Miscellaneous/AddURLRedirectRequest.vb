Imports iVectorConnectInterface.Interfaces

Public Class AddURLRedirectRequest
    Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

    Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
    Public Property OldURL As String
    Public Property CurrentURL As String

    Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

        Dim oWarnings As New Generic.List(Of String)

        If String.IsNullOrEmpty(Me.OldURL) OrElse String.IsNullOrEmpty(Me.CurrentURL) Then
            oWarnings.Add("Both the Old URL and the Current URL must be specified")
        End If

        Return oWarnings

    End Function
End Class

Imports iVectorConnectInterface.Interfaces

Namespace Modules

    Public Class ModuleRequest
        Implements iVectorConnectRequest

        Public Property ModuleName As String
        Public Property Route As String
        Public Property Method As String
        Public Property Body As String
        Public Property Headers As String

        Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)
        
            If String.IsNullOrEmpty(Me.ModuleName) Then aWarnings.Add("A Module Name must be specified.") 
            If String.IsNullOrEmpty(Me.Route) Then aWarnings.Add("A Route must be specified.") 
            If String.IsNullOrEmpty(Me.Method) Then aWarnings.Add("A Method must be specified.") 

            Return aWarnings

        End Function

    End Class

End Namespace
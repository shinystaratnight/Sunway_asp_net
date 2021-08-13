Namespace Modules

    Public Class ModuleResponse
        Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus
        Public Property Response As Object

        Public Sub New()

        End Sub

        Public Sub New(ByVal oResponse As Object)
            Me.Response = oResponse
            Me.ReturnStatus.Success = True
        End Sub

        Public Sub New(ByVal oResponse As Object, ByVal oExceptions As List(Of String))
            Me.Response = oResponse
            Me.ReturnStatus.Success = False
            Me.ReturnStatus.Exceptions.AddRange(oExceptions)
        End Sub

    End Class

End Namespace
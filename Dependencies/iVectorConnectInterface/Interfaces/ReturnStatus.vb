Imports System.Xml.Serialization

Public Class ReturnStatus

    Public Property Success As Boolean = True

    <XmlArrayItem("Exception")>
    Public Property Exceptions As New Generic.List(Of String)

    <XmlArrayItem("ThirdPartyError")>
    Public Property ThirdPartyErrors As New Generic.List(Of String)

    'bChangeStatus = false : don't fail the booking because of the warning
    Public Sub AddWarning(Warning As String, Optional bChangeStatus As Boolean = True)
        If Warning <> "" Then
            Me.Exceptions.Add(Warning)
            If bChangeStatus Then Me.Success = False
        End If
    End Sub

    Public Sub AddWarning(Warning As String, ByVal ParamArray aParam() As String)
        If Warning <> "" Then
            Me.Exceptions.Add(String.Format(Warning, aParam))
            Me.Success = False
        End If
    End Sub

    Public Sub AddWarnings(Warnings As Generic.List(Of String), Optional bChangeStatus As Boolean = True)
        Me.Exceptions.AddRange(Warnings)
        If bChangeStatus Then Me.Success = Me.Exceptions.Count = 0
    End Sub

End Class
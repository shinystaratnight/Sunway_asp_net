Imports System.Text

Public Class TrackingControl
    Inherits System.Web.UI.Control

    Private Property CustomNode As New Generic.List(Of String)
    Private Property BottomCustomNodes As New Generic.List(Of String)


    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

        Dim sb As New System.Text.StringBuilder

        For Each sCustomNode As String In Me.CustomNode
            sb.AppendFormat("\t{0}\n", sCustomNode)
        Next

        For Each sBottomCustomNode As String In Me.BottomCustomNodes
            sb.AppendFormat("\t{0}\n", sBottomCustomNode)
        Next

        writer.Write(sb.ToString.Replace("\n", ControlChars.NewLine).Replace("\t", ControlChars.Tab))

    End Sub

    Public Sub AddCustomNode(ByVal Node As String)
        If Node <> "" Then
            Me.CustomNode.Add(Node)
        End If
    End Sub

    Public Sub AddCustomNodeAtBottom(ByVal Node As String)
        If Node <> "" Then
            Me.BottomCustomNodes.Add(Node)
        End If
    End Sub

End Class

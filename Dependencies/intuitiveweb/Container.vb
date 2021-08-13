Imports Intuitive


Public Class Container
    Inherits System.Web.UI.Control

	Public CSSClass As String = ""
	Public OverrideOuterHTML As Boolean = False

	Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

		If Me.OverrideOuterHTML Then
			MyBase.Render(writer)
		ElseIf Me.Controls.Count > 0 Then
			writer.WriteLine(String.Format("<div id=""{0}"" class=""{1}"">", Me.ID, Me.CSSClass))
			MyBase.Render(writer)
			writer.WriteLine("</div>")
		End If

	End Sub
End Class

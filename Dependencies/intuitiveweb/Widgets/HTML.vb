Imports System.IO

Namespace Widgets

	Public Class HTML
		Inherits Widgets.WidgetBase

		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

			Dim sHTMLPath As String = Me.GetWidgetFilePathFromExtension(".htm")
			Dim sHTML As String = ""
			If System.IO.File.Exists(sHTMLPath) Then
				sHTML = File.ReadAllText(sHTMLPath)
			End If
			writer.Write(sHTML)

		End Sub

	End Class


End Namespace

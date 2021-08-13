Namespace Widgets


	Public Class RawHTML
		Inherits WidgetBase


		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)
			writer.Write(Me.RawHTML)
		End Sub

	End Class


End Namespace

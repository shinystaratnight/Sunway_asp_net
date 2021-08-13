Namespace WebControls
	Public Class PlaceHolder
		Inherits System.Web.UI.Control

		Private sText As String = ""

		Public Property Text() As String
			Get
				Return sText
			End Get
			Set(ByVal Value As String)
				sText = Value
			End Set
		End Property

		Public Sub AddText(ByVal sString As String, ByVal ParamArray aParams() As Object)

			Me.Text = String.Format(sString, aParams)
		End Sub

		Public Sub AppendText(ByVal sString As String, ByVal ParamArray aParams() As Object)

			Me.Text += String.Format(sString, aParams)
		End Sub

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			writer.Write(sText)
		End Sub
	End Class
End Namespace
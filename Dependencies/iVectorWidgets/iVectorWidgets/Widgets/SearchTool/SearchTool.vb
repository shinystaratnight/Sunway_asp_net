Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports System.Threading.Tasks


Public Class SearchTool
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)


		'1.build control
		Dim sControlPath As String = Me.URLPath & "/searchtool.ascx"
		Dim oControl As Control = Me.Page.LoadControl(sControlPath)
		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
		Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oControl)

		writer.Write(sControlHTML)


	End Sub





End Class




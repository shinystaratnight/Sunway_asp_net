Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.ComponentModel

Public Class PageNotFound
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oXML As New XmlDocument
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams

		Me.XSLTransform(oXML, XSL.SetupTemplate(res.PageNotFound, True, False), writer, oXSLParams)


	End Sub


End Class

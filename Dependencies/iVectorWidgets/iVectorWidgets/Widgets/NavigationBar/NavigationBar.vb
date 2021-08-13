Imports System.Xml
Imports Intuitive.Web.Widgets
Imports Intuitive.Web

Public Class NavigationBar
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oXML As XmlDocument = Utility.URLToXML(Me.CMSURL)
		Me.XSLTransform(oXML, res.NavigationBar, writer)

	End Sub

End Class

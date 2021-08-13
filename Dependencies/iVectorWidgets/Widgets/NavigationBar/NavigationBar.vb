Imports System.Xml
Imports Intuitive.Web.Widgets
Imports Intuitive.Web

Public Class NavigationBar
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oXML As XmlDocument = Utility.BigCXML(Me.ObjectType, Me.ObjectID, 60)
		Me.XSLTransform(oXML, res.NavigationBar, writer)

	End Sub

End Class

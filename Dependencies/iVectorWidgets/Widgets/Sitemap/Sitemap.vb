Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class Sitemap
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML

		'similar hotels xml
		Dim oSitemapXML As New XmlDocument
		oSitemapXML.LoadXml(oPageXML.SelectSingleNode("//Sitemap").OuterXml)

		'transform xsl
		Me.XSLTransform(oSitemapXML, res.Sitemap, writer)

	End Sub

End Class

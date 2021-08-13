Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class SimilarHotels
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML


		'similar hotels xml
		Dim oSimilarHotelsXML As New XmlDocument
		oSimilarHotelsXML.LoadXml(oPageXML.SelectSingleNode("//SimilarHotels").OuterXml)


		'transform xsl
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		Me.XSLTransform(oSimilarHotelsXML, res.SimilarHotels, writer)

	End Sub

End Class


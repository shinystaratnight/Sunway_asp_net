Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class RelatedPages
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML


		'related pages xml
		Dim oRelatedPagesXML As New XmlDocument
		Dim sOuterXML As String = XMLFunctions.SafeOuterXML(oPageXML, "//RelatedPages")

		If sOuterXML <> "" Then

			oRelatedPagesXML.LoadXml(sOuterXML)

			'transform xsl
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			Me.XSLTransform(oRelatedPagesXML, res.RelatedPages, writer)

		End If

	End Sub

End Class
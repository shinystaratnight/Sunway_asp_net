Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class UsefulLinks
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML

		'useful links xml
		Dim oUsefulLinksXML As New XmlDocument
		Dim sOuterXML As String = XMLFunctions.SafeOuterXML(oPageXML, "//UsefulLinks")

        If sOuterXML <> "" AndAlso XMLFunctions.SafeNodeValue(oPageXML, "//UsefulLinks") <> "" Then

            oUsefulLinksXML.LoadXml(sOuterXML)

            'transform xsl
            Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
            Me.XSLTransform(oUsefulLinksXML, XSL.SetupTemplate(res.UsefulLinks, False, True), writer)

        End If

	End Sub

End Class

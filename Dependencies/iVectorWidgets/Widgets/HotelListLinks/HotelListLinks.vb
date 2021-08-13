Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class HotelListLinks
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML

		'hotel list link xml
		Dim oHotelListLinksXML As New XmlDocument
		Dim sOuterXML As String = XMLFunctions.SafeOuterXML(oPageXML, "//HotelListLinks")

		If sOuterXML <> "" Then

			oHotelListLinksXML.LoadXml(sOuterXML)

			'transform xsl
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			Me.XSLTransform(oHotelListLinksXML, res.HotelListLinks, writer)

		End If

	End Sub

End Class
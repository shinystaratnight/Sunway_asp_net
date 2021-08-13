Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class FavouriteHotels
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get page xml
		Dim oPageXML As XmlDocument = Me.PageDefinition.Content.XML


		'favourite hotels xml
		Dim oFavouriteHotelsXML As New XmlDocument
		Dim sOuterXML As String = XMLFunctions.SafeOuterXML(oPageXML, "//FavouriteHotels")

		If sOuterXML <> "" Then

			oFavouriteHotelsXML.LoadXml(sOuterXML)

			'transform xsl
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			Me.XSLTransform(oFavouriteHotelsXML, res.FavouriteHotels, writer)

		End If

	End Sub

End Class


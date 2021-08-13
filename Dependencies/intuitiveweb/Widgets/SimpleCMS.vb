Imports System.Xml
Namespace Widgets


	Public Class SimpleCMS
		Inherits WidgetBase


		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

			Dim oXML As New XmlDocument

			'get xml from specified URL or use the page content xml
			If Me.ObjectType <> "" Then
				oXML = Utility.BigCXML(Me.ObjectType, Functions.IIf(Me.ObjectID = 0, 1, Me.ObjectID), 60)
			ElseIf Not Me.PageDefinition.Content Is Nothing Then
				oXML = Me.PageDefinition.Content.XML
			End If

			Me.XSLTransform(oXML, writer)

		End Sub

	End Class



End Namespace


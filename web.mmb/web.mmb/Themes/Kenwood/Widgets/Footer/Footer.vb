Imports System.Collections.Generic
Imports System.Linq
Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions

Namespace Widgets
	Public Class Footer
		Inherits WidgetBase

#Region "Page Lifecycle"

		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

			'XML generated and transformed
			Dim oXML As New XmlDocument
			XSLTransform(oXML, writer)

		End Sub

#End Region

	End Class
End Namespace
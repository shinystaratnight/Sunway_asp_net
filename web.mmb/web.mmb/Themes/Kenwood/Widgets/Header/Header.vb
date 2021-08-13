Imports System.Xml
Imports Intuitive
Imports Intuitive.Web.Widgets

Namespace Widgets
	Public Class Header
		Inherits WidgetBase

#Region "Page Lifecycle"

		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)
            'Create params
            Dim oXSLParams As New WebControls.XSL.XSLParams
            With oXSLParams
                .AddParam("LoggedIn", BookingBase.LoggedIn)
            End With

			'generates XML and transforms it
			Dim oXML As New XmlDocument
			XSLTransform(oXML, writer, oXSLParams)
		End Sub

#End Region

	End Class
End Namespace
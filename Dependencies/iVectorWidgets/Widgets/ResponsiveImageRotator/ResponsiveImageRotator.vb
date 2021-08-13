Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.Drawing
Imports System.Net
Imports System.IO

Public Class ResponsiveImageRotator
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Get content path from widget settigns
		Dim sObjectType As String = Settings.GetValue("ObjectType")


		'2. Get xml from content path
		Dim oXML As XmlDocument = Intuitive.Web.Utility.BigCXML(sObjectType, 1, 60)


		'3. Create params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Identifier", Settings.GetValue("Identifier"))
			.AddParam("NavigationType", Settings.GetValue("NavigationType").ToLower)
			.AddParam("TransitionType", Settings.GetValue("TransitionType").ToLower)
			.AddParam("HoldTime", Settings.GetValue("HoldTime"))
			.AddParam("ImageWidth", Settings.GetValue("ImageWidth"))
		End With



		'4. Transform
		Me.XSLTransform(oXML, res.ResponsiveImageRotator, writer, oXSLParams)


	End Sub

End Class


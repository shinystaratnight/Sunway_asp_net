Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.Drawing
Imports System.Net
Imports System.IO

Public Class ImageRotator
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Get content path from widget settigns
		Dim sContentPath As String = Settings.GetValue("ContentPath")

		'2. Get xml from content path
		Dim oXML As XmlDocument = Intuitive.Web.Utility.URLToXML(sContentPath)

		'3. Create params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("Identifier", Settings.GetValue("Identifier"))
		oXSLParams.AddParam("NavigationType", Settings.GetValue("NavigationType").ToLower)
		oXSLParams.AddParam("TransitionType", Settings.GetValue("TransitionType").ToLower)
		oXSLParams.AddParam("TransitionTime", Settings.GetValue("TransitionTime"))
		oXSLParams.AddParam("ImageWidth", Settings.GetValue("ImageWidth"))

		'4. Transform
		Me.XSLTransform(oXML, res.ImageRotator, writer, oXSLParams)



	End Sub

End Class


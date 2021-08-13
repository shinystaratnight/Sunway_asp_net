Imports iw = Intuitive.Web
Imports System.IO

Public Class MapControl
	Inherits UserControlBase

	Public MapPointsJSON As String
	Public IconsJSON As String

	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

		Me.hidPopupX.Value = Settings.GetValue("PopupX")
		Me.hidPopupY.Value = Settings.GetValue("Popupy")
		Me.hidClusterPopupX.Value = Settings.GetValue("ClusterPopupX")
		Me.hidClusterPopupY.Value = Settings.GetValue("ClusterPopupY")
		Me.h2Title.InnerText = Settings.GetValue("TitleOverride")

	End Sub

	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

		'set theme
		Me.hidMapTheme.Value = iw.BookingBase.Params.Theme

		'set map points
		Me.hidMapPoints.Value = Me.MapPointsJSON

		'Get icons
		Me.hidMapIcons.Value = Me.IconsJSON

		'strip out id/name buggeration

		'get the render string
		Dim sb As New StringBuilder
		Dim oWriter As HtmlTextWriter = New HtmlTextWriter(New StringWriter(sb))
		MyBase.Render(oWriter)

		'cut the crap IDs out
		Dim sRender As String = sb.ToString.Replace("id=""src_", "id=""")
		sRender = sRender.Replace("name=""src:", "name=""")
		sRender = sRender.Replace(Me.UniqueID & "$", "")
		sRender = sRender.Replace("href=""widgets/propertyresults/#""", "href=""#""")

		writer.Write(sRender)

	End Sub

End Class
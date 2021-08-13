Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.XMLFunctions
Imports Intuitive.Web.Widgets
Imports Intuitive.Web
Imports System.Xml
Imports System.ComponentModel

Public Class FindBooking
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. determine the xml
		Dim oXML As New XmlDocument

		'1.1 get overrides and convert to xml
		Dim oLabelOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Functions.SafeString(GetSetting(eSetting.LabelOverrides)), "LabelOverrides")
		Dim oPlaceholderOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Functions.SafeString(GetSetting(eSetting.PlaceholderOverrides)), "PlaceholderOverrides")

		'1.2 merge xml
		oXML = MergeXMLDocuments("Overrides", oXML, oLabelOverridesXML, oPlaceholderOverridesXML)

		'2. set up parameters
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("ImageFolder", Functions.SafeString(GetSetting(eSetting.ImageFolder)))
			.AddParam("DatepickerMonths", Functions.SafeInt(GetSetting(eSetting.DatepickerMonths)))
			.AddParam("DatepickerFirstDay", Functions.SafeInt(GetSetting(eSetting.DatepickerFirstDay)))
		End With

		'4. transform
		Dim sTemplateOverride As String = Functions.SafeString(GetSetting(eSetting.TemplateOverride))
		If sTemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & sTemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.FindBooking, True, True), writer, oXSLParams)
		End If

	End Sub

	Public Enum eSetting

		<Title("Template Override")>
		<Description("XSL Template Override")>
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Image Folder")>
		<Description("Unused")>
		<DeveloperOnly(True)>
		ImageFolder

		<Title("Datepicker Months")>
		<Description("Number of months to display on the datepicker")>
		DatepickerMonths

		<Title("Datepicker First Day")>
		<Description("Day to use as first day of the week on the datepicker, 0-6 (0 = Sunday)")>
		DatepickerFirstDay

		<Title("Label Overrides")>
		<Description("Overrides label text. Format: [ID]|[Value]#[ID]|[Value]. Valid IDs: lblReference, lblGuestName, lblBooked, lblBookedStart, lblBookedEnd, lblTravelling, lblArrivalStart, lblArrivalEnd.")>
		LabelOverrides

		<Title("Placeholder Overrides")>
		<Description("Overrides textbox placeholder text. Format: [ID]|[Value]#[ID]|[Value]. Valid IDs: txtReference, txtGuestName.")>
		PlaceholderOverrides

	End Enum

End Class

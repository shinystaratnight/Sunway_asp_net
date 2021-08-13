Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions

Public Class HotelResultsFooter
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1.build control
		'Dim sControlPath As String = Me.URLPath & "/HotelResultsFooter.ascx"
		'Dim oControl As Control = Me.Page.LoadControl(sControlPath)
		'CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)




		'Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oControl)

		'writer.Write(sControlHTML)

		If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")
		Dim sControlPath As String

		If Me.URLPath.EndsWith(".ascx") Then
			sControlPath = Me.URLPath
		Else
			sControlPath = Me.URLPath & "/" & "HotelResultsFooter.ascx"
		End If


		Dim oControl As New Control
		Try
			oControl = Me.Page.LoadControl(sControlPath)
		Catch ex As Exception
			oControl = Me.Page.LoadControl("/widgetslibrary/HotelResultsFooter/HotelResultsFooter.ascx")
		End Try

		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
		Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oControl)

		writer.Write(sControlHTML)





	End Sub


	Public Overridable Function SelectPage(ByVal PageNumber As Integer) As String

		BookingBase.SearchDetails.PropertyResults.CurrentPage = PageNumber

		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = Intuitive.Functions.IIf(BookingBase.SearchDetails.PropertyResults.TotalPages = 0, 1, _
	 BookingBase.SearchDetails.PropertyResults.TotalPages)
			.TotalLinksToDisplay = SearchSummary.CustomSettings.PagingTotalLinks
			.CurrentPage = PageNumber
			.ScriptPrevious = "HotelResultsFooter.PreviousPage();"
			.ScriptNext = "HotelResultsFooter.NextPage();"
			.ScriptPage = "HotelResultsFooter.SelectPage({0})"
			.NextIcon = SearchSummary.CustomSettings.NextIcon
			.PreviousIcon = SearchSummary.CustomSettings.PreviousIcon
            .ShowTotalPages = SearchSummary.CustomSettings.ShowTotalPages
		End With

		Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oPaging)
        
		Return Intuitive.Web.Translation.TranslateHTML(sControlHTML)

	End Function



End Class




Imports iw = Intuitive.Web
Imports System.IO
Imports Intuitive.Functions

Public Class HotelResultsFooterControl
	Inherits UserControlBase

	Public Property PagingShowTotalPages As Boolean = False
	Public NextIcon As String
	Public PreviousIcon As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

	End Sub

	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

		'disclaimer text
		If Settings.GetValue("DisclaimrerText") <> "" Then
			Me.pDisclaimerText.InnerHtml = Settings.GetValue("DisclaimrerText")
		Else
			Me.pDisclaimerText.Visible = False
		End If

		'check for paging total pages setting
		Dim bShowTotalPages As Boolean = SafeBoolean(Settings.GetValue("ShowTotalPages"))

		Me.btnBackToTop.Visible = SafeBoolean(Settings.GetValue("BackToTop"))

		Me.PagingShowTotalPages = bShowTotalPages
		Me.NextIcon = SafeString(Settings.GetValue("NextIcon"))
		Me.PreviousIcon = SafeString(Settings.GetValue("PreviousIcon"))

	End Sub


	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

		'get search details
		Dim oSearchDetails As iw.BookingSearch = iw.BookingBase.SearchDetails

		'setup paging
		Me.divHotelResultsFooterControls.Visible = oSearchDetails.PropertyResults.TotalPages > 1
		Me.divPagingBottom.Visible = oSearchDetails.PropertyResults.TotalPages > 1
		Me.btnBackToTop.Visible = oSearchDetails.PropertyResults.TotalPages > 1

		If Me.divPagingBottom.Visible Then
			With Me.pagingBottom
				.TotalPages = oSearchDetails.PropertyResults.TotalPages
				.TotalPages = Intuitive.Functions.IIf(Me.pagingBottom.TotalPages = 0, 1, Me.pagingBottom.TotalPages)
				.TotalLinksToDisplay = 5
				.CurrentPage = oSearchDetails.PropertyResults.CurrentPage
				.ScriptPrevious = "SearchSummary.PreviousPage();"
				.ScriptNext = "SearchSummary.NextPage();"
				.ScriptPage = "SearchSummary.SelectPage({0})"
				.ShowTotalPages = Me.PagingShowTotalPages
				.NextIcon = Me.NextIcon
				.PreviousIcon = Me.PreviousIcon
			End With
		End If


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
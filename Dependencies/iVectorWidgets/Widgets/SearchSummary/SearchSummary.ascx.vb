Imports iw = Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.IO
Imports System.Xml
Imports System.Net
Imports Intuitive.Functions
Imports Intuitive.Web.Translation

Public Class SearchSummaryControl
	Inherits UserControlBase

	Public Property PagingShowTotalPages As Boolean = False

	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

		'set best seller sort
		If SafeBoolean(Settings.GetValue("BestSellerSort")) Then
			Dim sBestSellerText As String = Settings.GetValue("BestSellerText")
			Me.ddlSortOrder.Options = IIf(sBestSellerText <> "", sBestSellerText, "Best Seller") & "|BestSeller_Ascending#" & Me.ddlSortOrder.Options
		End If

		'popular helper
		Dim bShowPopularHelper As Boolean = SafeBoolean(Settings.GetValue("ShowPopularHelper"))
		Dim sPopularHelperText As String = SafeString(Settings.GetValue("PopularHelperText"))
		If bShowPopularHelper Then
			Me.divPopularHelper.InnerText = IIf(sPopularHelperText <> "", sPopularHelperText, "Click to view most popular hotels")
		Else
			Me.divPopularHelper.Visible = False
		End If

		'tab text
		Dim sPopularTabText As String = SafeString(Settings.GetValue("PopularTabText"))
		If sPopularTabText <> "" Then
			Me.aPopular.InnerText = sPopularTabText
		End If

		'show/hide quick view tab
		Me.aQuickView.Visible = SafeBoolean(Settings.GetValue("ShowQuickViewTab"))

		'override labels
		Dim sLabelOverrides As String = Settings.GetValue("LabelOverrides")
		If sLabelOverrides <> "" Then
			Me.UpdateOverrides("Labels", sLabelOverrides)
		End If

		'check for paging total pages setting
		Dim bShowTotalPages As Boolean = SafeBoolean(Settings.GetValue("ShowTotalPages"))
		Me.PagingShowTotalPages = bShowTotalPages

	End Sub


	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

		Dim oSearchDetails As iw.BookingSearch = iw.BookingBase.SearchDetails

		'set search 
		Me.hSummary.InnerHtml = SearchSummary.GenerateSearchSummary()


		'setup paging
		With Me.pagingTop
			.TotalPages = oSearchDetails.PropertyResults.TotalPages
			.TotalPages = Intuitive.Functions.IIf(Me.pagingTop.TotalPages = 0, 1, Me.pagingTop.TotalPages)

			.TotalLinksToDisplay = 5
			.CurrentPage = oSearchDetails.PropertyResults.CurrentPage
			.ScriptPrevious = "SearchSummary.PreviousPage();"
			.ScriptNext = "SearchSummary.NextPage();"
			.ScriptPage = "SearchSummary.SelectPage({0})"
			.ShowTotalPages = Me.PagingShowTotalPages
		End With


		'only show controls if not on flight results page unless we want to always show it.
		Me.divSearchSummaryControls.Visible = (Not SearchSummary.CustomSettings.FlightPage AndAlso Not SearchSummary.CustomSettings.TransferOnlyJourney) _
						 OrElse SearchSummary.CustomSettings.AlwaysShow
		If oSearchDetails.PropertyResults.TotalHotels < 2 OrElse SearchSummary.CustomSettings.AlwaysShow Then Me.divSearchSummary.Attributes("class") = "nocontrols"


		'hide sort dropdown if only 1 result
		Me.divSortOrder.Visible = oSearchDetails.PropertyResults.TotalHotels > 1



		'add pages class in case we need some styling differences
		If oSearchDetails.PropertyResults.TotalPages > 1 Then
			Dim sSearchSummaryCurrentClass As String = Me.divSearchSummary.Attributes("class")
			Me.divSearchSummary.Attributes("class") = IIf(sSearchSummaryCurrentClass = "", "pages", sSearchSummaryCurrentClass & " pages")
		End If


		'hide map tab if no geocoded hotels
		If Not oSearchDetails.PropertyResults.GeocodedHotels Then
			Me.aMapView.Attributes("class") = "display:none;"
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

	Public Sub UpdateOverrides(ByVal sType As String, ByVal sOverrides As String)

		'split overrides into array
		Dim aOverrides As String() = sOverrides.Split("#"c)

		'loop through each override and set value if control exists
		For Each sOverride As String In aOverrides

			Dim sID As String = sOverride.Split("|"c)(0)
			Dim sValue As String = sOverride.Split("|"c)(1)

			Dim oElement As Control = Me.FindControl(sID)

			Try

				If Not oElement Is Nothing Then

					If sType = "Placeholders" Then
						If TypeOf oElement Is HtmlInputText Then
							CType(oElement, HtmlInputText).Attributes("placeholder") = GetCustomTranslation("Search Tool", sValue)
						ElseIf TypeOf oElement Is iw.Controls.Dropdown Then
							CType(oElement, iw.Controls.Dropdown).OverrideBlankText = GetCustomTranslation("Search Tool", sValue)
						End If
					Else
						If TypeOf oElement Is HtmlAnchor Then
							CType(oElement, HtmlAnchor).InnerText = GetCustomTranslation("Search Tool", sValue)
						Else
							CType(oElement, HtmlGenericControl).InnerHtml = GetCustomTranslation("Search Tool", sValue)
						End If
					End If

				End If

			Catch ex As Exception

			End Try

		Next

	End Sub


End Class
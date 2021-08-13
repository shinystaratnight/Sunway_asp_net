Imports System.Web.UI
Imports Intuitive.Functions

Namespace WebControls
	Public Class Paging
		Inherits System.Web.UI.Control

		Public TotalPages As Integer
		Public CurrentPage As Integer
		Public TotalLinksToDisplay As Integer = 10

		Public OfText As String = "of"
		Public PreviousPageText As String = "Prev"
		Public NextPageText As String = "Next"

		Public ScriptPrevious As String = ""
		Public ScriptNext As String = ""
		Public ScriptPage As String = ""

		Public SuppressIfOnlyOnePage As Boolean = True

		Public IsMultiLingual As Boolean = False

		Public AllLink As Boolean = False
		Public AllText As String = ""
		Public AllScript As String = ""

		Public FirstAndLastLinks As Boolean = False
		Public FirstText As String = "First"
		Public LastText As String = "Last"

#Region "Show"

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

			'Suppress if only one page
			If Me.SuppressIfOnlyOnePage AndAlso Me.TotalPages = 1 Then
				Return
			End If

			'set up ml page if multiLingual
			Dim sMlString As String = IIf(Me.IsMultiLingual, "ml=""Paging Control""", "")

			'draw paging
			Dim sb As New System.Text.StringBuilder

			'check assumptions
			If Me.TotalPages = 0 Then Throw New Exception("expected total pages to be set")

			'previous link
			Dim sLimitClass As String = IIf(Me.CurrentPage = 1, " limit", "")

			'remove script if first page otherwise keep script set
			If (Me.CurrentPage = 1) Then
				Me.ScriptPrevious = "return false;"
			Else
				Me.ScriptPrevious += ";return false;"
			End If

			'show a first and last button?
			If Me.FirstAndLastLinks Then
				If Not 1 = CurrentPage AndAlso Me.TotalPages > Me.TotalLinksToDisplay Then
					Dim sPageScript As String = String.Format(Me.ScriptPage, 1)
					sb.AppendFormat("<a class=""paging"" id=""aPaging_{0}"" href=""#"" onclick=""{1};return false;"">", 1, sPageScript).Append(Me.FirstText).Append("</a>")
				End If
			End If

			sb.AppendFormat("<a class=""pagingPrev{1}"" href=""#"" onclick=""{0}"" {2}>", Me.ScriptPrevious, sLimitClass, sMlString)
			sb.Append(Me.PreviousPageText)
			sb.Append("</a>")

			'draw the pages
			Dim iFirstLink As Integer = 1
			Dim iLastLink As Integer = IIf(Me.TotalPages < Me.TotalLinksToDisplay, Me.TotalPages, Me.TotalLinksToDisplay)
			Dim iLinksBefore As Integer
			Dim iLinksAfter As Integer

			If Me.TotalPages > Me.TotalLinksToDisplay Then

				'work out how many links each side of current page
				iLinksBefore = SafeInt((Me.TotalLinksToDisplay - Me.TotalLinksToDisplay Mod 2) / 2)
				iLinksAfter = iLinksBefore - (Me.TotalLinksToDisplay + 1) Mod 2

				'set first and last link
				If Me.CurrentPage - iLinksBefore > 1 Then
					iFirstLink = CurrentPage - iLinksBefore
					iLastLink = CurrentPage + iLinksAfter
				End If

				If iLastLink > Me.TotalPages Then
					iLastLink = Me.TotalPages
					iFirstLink = iLastLink - (Me.TotalLinksToDisplay - 1)
				End If

			End If

			'loop out links
			For i As Integer = iFirstLink To iLastLink
				If i = CurrentPage Then
					sb.AppendFormat("<a class=""paging current"" id=""aPaging_{0}"" href=""#"">", i).Append(i).Append("</a>")
				Else
					Dim sPageScript As String = String.Format(Me.ScriptPage, i)
					sb.AppendFormat("<a class=""paging"" id=""aPaging_{0}"" href=""#"" onclick=""{1};return false;"">", i, sPageScript).Append(i).Append("</a>")
				End If

			Next

			'of
			If IsMultiLingual Then
				sb.AppendFormat("<span {0} mlparams=""{1}"">", sMlString, Me.TotalPages)
				sb.Append("of {0}</span>")
			Else
				sb.AppendFormat("<span>{1} {0}</span>", Me.TotalPages, Me.OfText)
			End If

			'next link
			sLimitClass = IIf(Me.CurrentPage = Me.TotalPages, " limit", "")

			'remove script if last page otherwise keep script set
			If (Me.CurrentPage = Me.TotalPages) Then
				Me.ScriptNext = ""
			Else
				Me.ScriptNext += ";return false;"
			End If

			sb.AppendFormat("<a class=""pagingNext{1}"" href=""#"" onclick=""{0}"" {2}>", Me.ScriptNext, sLimitClass, sMlString)
			sb.Append(Me.NextPageText)
			sb.Append("</a>")

			'show a first and last button?
			If Me.FirstAndLastLinks Then
				If Not Me.TotalPages = CurrentPage AndAlso Me.TotalPages > Me.TotalLinksToDisplay Then
					Dim sPageScript As String = String.Format(Me.ScriptPage, Me.TotalPages)
					sb.AppendFormat("<a class=""paging"" id=""aPaging_{0}"" href=""#"" onclick=""{1};return false;"">", Me.TotalPages, sPageScript).Append(Me.LastText).Append("</a>")
				End If
			End If

			'show an all button at the end?
			If Me.AllLink Then
				sb.AppendFormat("<a class=""paging"" id=""aPaging_All"" href=""#"" onclick=""{0};return false;"">", Me.AllScript).Append(Me.AllText).Append("</a>")
			End If

			writer.Write(sb.ToString)

		End Sub

#End Region

	End Class

End Namespace
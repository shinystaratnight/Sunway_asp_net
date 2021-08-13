Imports System.Web.UI
Imports Intuitive.Functions

Namespace Controls

	Public Class Paging
		Inherits System.Web.UI.Control


		Public TotalPages As Integer
		Public CurrentPage As Integer = 1
		Public TotalLinksToDisplay As Integer = 3

		Public ScriptPrevious As String = ""
		Public ScriptNext As String = ""
		Public ScriptPage As String = ""

		Public NextIcon As String
		Public PreviousIcon As String

		Public PageURL As String = ""

		Public ShowTotalPages As Boolean = False


#Region "Show"

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)


			'Suppress if only one page
			If Me.TotalPages = 1 Then
				Return
			End If

			'set up ml page if multiLingual
			Dim sMlString As String = "ml=""Paging Control"""


			'draw paging
			Dim sb As New System.Text.StringBuilder

			'check assumptions
			If Me.TotalPages = 0 Then Throw New Exception("expected total pages to be set")


			sb.Append("<div class=""paging"">")
			sb.Append("<ul>")


			'class
			Dim sClass As String = IIf(Me.CurrentPage = 1, " disabled", "")

			'remove script if first page otherwise keep script set
			If (Me.CurrentPage = 1) Then
				Me.ScriptPrevious = "return false;"
			Else
				Me.ScriptPrevious += ";return false;"
			End If

			sb.AppendFormat("<li class=""prev{1}""><a href=""#"" onclick=""{0}"" {2}>{3}</a></li>", Me.ScriptPrevious, sClass, sMlString,
				Me.PreviousIcon)



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

				If i = Me.CurrentPage Then
					sb.AppendFormat("<li class=""current""><a id=""aPaging_{0}"" href=""#"">", i).Append(i).Append("</a></li>")
				Else
					Dim sPageScript As String = String.Format(Me.ScriptPage, i)
					sb.AppendFormat("<li><a id=""aPaging_{0}"" href=""#"" onclick=""{1};return false;"">", i, sPageScript).Append(i).Append("</a></li>")
				End If

			Next


			'class
			sClass = IIf(Me.CurrentPage = Me.TotalPages, " disabled", "")


			'remove script if last page otherwise keep script set
			If (Me.CurrentPage = Me.TotalPages) Then
				Me.ScriptNext = ""
			Else
				Me.ScriptNext += ";return false;"
			End If

			If Me.ShowTotalPages Then
				sb.AppendFormat("<li class=""totalpages"" {0} mlparams=""{1}"">", sMlString, Me.TotalPages)
				sb.Append("of <span>{0}</span></li>")
			End If

			sb.AppendFormat("<li class=""next{1}""><a href=""#"" onclick=""{0}"" {2}>{3}</a></li>", Me.ScriptNext, sClass, sMlString,
				Me.NextIcon)

			sb.Append("</ul></div>")

            Dim finalHtml As String = sb.ToString
            If BookingBase.Params.PreTranslateTemplates Then
                finalHtml = Intuitive.Web.Translation.TranslateHTML(finalHtml)
            End If
            writer.Write(finalHtml)

		End Sub

#End Region

	End Class

End Namespace

Imports System.Text

Public Class OpenGraph
	Public Property Type As ContentType
	Public Property Title As String
	Public Property Description As String
	Public Property URL As String
	Public Property SiteName As String
	Public Property PublishedDate As DateTime
	Public Property Image As String

	Public Function ToHTML() As String

		Dim sb As New StringBuilder()

		With sb

			If Me.Title <> "" Then
				.AppendFormatLine("<meta property=""og:title"" content=""{0}"" />", Me.Title)
			End If

			If Me.Type <> ContentType.None Then
				.AppendFormatLine("<meta property=""og:type"" content=""{0}"" />", Me.Type.ToString.ToLower)
			End If

			If Me.Description <> "" Then
				.AppendFormatLine("<meta property=""og:description"" content=""{0}"" />", Me.Description)
			End If

			If Me.URL <> "" Then
				.AppendFormatLine("<meta property=""og:url"" content=""{0}"" />", Me.URL)
			End If

			If Me.SiteName <> "" Then
				.AppendFormatLine("<meta property=""og:site_name "" content=""{0}"" />", Me.SiteName)
			End If

			If Me.PublishedDate <> Nothing Then
				.AppendFormatLine("<meta property=""og:updated_time"" content=""{0}"" />", _
								  Me.PublishedDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffzzz"))
			End If

			If Me.Image <> "" Then
				.AppendFormatLine("<meta property=""og:image"" content=""{0}"" />", Me.Image)
			End If

		End With


		Return sb.ToString

	End Function

	Public Enum ContentType
		None = 0
		Article
	End Enum

End Class

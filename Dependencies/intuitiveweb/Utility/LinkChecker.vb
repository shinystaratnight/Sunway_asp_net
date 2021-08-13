Imports Intuitive.Functions
Imports System.Configuration.ConfigurationManager
Imports System.Text.RegularExpressions

Public Class LinkChecker

#Region "Check hrefs and img src"

	Public Shared Function Go(ByVal HTML As String) As String

		Dim sRegex As String = "<a\s[^>]*>.*<\/a>|<img[^>]*/>"

		'declare a new regex object which has a delegate as it third? parameter. See.... translation class in intuitive project
		Dim oRegexLink As New Regex(sRegex, RegexOptions.Compiled Or RegexOptions.Singleline)
		HTML = oRegexLink.Replace(HTML, AddressOf LinkDelegate)

		Return HTML

	End Function

	Public Shared Function LinkDelegate(ByVal oMatch As System.Text.RegularExpressions.Match) As String
		Dim oRegex As New Regex("(?<=(?:src=""|href="")).*(?="")", RegexOptions.Compiled)
		Dim sReturn As String = oRegex.Replace(oMatch.Value, AddressOf LinkAttributeDelegate)
		Return sReturn
	End Function

	Public Shared Function LinkAttributeDelegate(ByVal oMatch As System.Text.RegularExpressions.Match) As String

		Try
			Dim sMatchedValue As String = oMatch.Value

			'this would naturally fail somewhere below but decided to explicitly trap these ones
			If sMatchedValue.StartsWith("http") Then Throw New Exception("cannot version non relative files")
			If sMatchedValue.StartsWith("//ajax.googleapis") Then Throw New Exception("cannot version google hosted libraries")
			If sMatchedValue.StartsWith("javascript") Then Throw New Exception("leave these alone")
			If sMatchedValue.StartsWith("#") Then Throw New Exception("leave these alone")
			If sMatchedValue.StartsWith("tel") Then Throw New Exception("leave these alone")


			'for some websites we need to serve the whole website under a root path eg holidays.domain.com/{holidays}/search-results
			'this is achieved by some code that runs from the config setting
			'and also by a regex entry in the IIS Url rewrite module
			'if this has been set up then edit the file path accordingly
			'the root path should be in the format: "/holidays" with no trailing slash
			Dim sRootPath As String = SafeString(AppSettings("RootPath"))


			Dim sReturnValue As String = ""
			'return the file with a version number attached
			If sMatchedValue.StartsWith(sRootPath) Then
				sReturnValue = sMatchedValue
			Else
				sReturnValue = sRootPath & sMatchedValue
			End If

			'chop off trailing slash
			If Not sReturnValue = "/" AndAlso sReturnValue.EndsWith("/") Then
				sReturnValue = sReturnValue.Chop(1)
			End If

			Return sReturnValue

		Catch ex As Exception

			'do NOT convert this one to lower
			Return oMatch.Value

		End Try

	End Function

#End Region




End Class

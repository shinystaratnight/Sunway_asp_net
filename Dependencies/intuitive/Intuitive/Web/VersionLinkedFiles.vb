Imports System.Text.RegularExpressions
Imports System.Web
Imports Intuitive.Functions
Imports System.Configuration.ConfigurationManager
Imports System.IO

''' <summary>
''' Class used to update version numbers on javascript and css files
''' </summary>
Public Class VersionLinkedFiles

	'the purpose of this is to have a central function that will update version numbers on javascript/css files

#Region "Cache Properties"

	''' <summary>
	''' The linked file cache name
	''' </summary>
	Public Shared LinkedFileCacheName As String = "LinkedFileContentCache"

	''' <summary>
	''' Gets or sets the linked file content cache.
	''' </summary>
	Public Shared Property LinkedFileContentCache() As Hashtable
		Get
			Dim aFileContent As Hashtable = Functions.GetCache(Of Hashtable)(LinkedFileCacheName)

			If aFileContent Is Nothing Then
				aFileContent = New Hashtable
				VersionLinkedFiles.LinkedFileContentCache = aFileContent
			End If

			Return aFileContent
		End Get
		Set(ByVal value As Hashtable)
			Functions.AddToCache(LinkedFileCacheName, value, 60)
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether to fix relative paths.
	''' </summary>
	Public Shared Property FixRelativePaths As Boolean
		Get
			Return SafeBoolean(HttpContext.Current.Session("FixRelativePaths"))
		End Get
		Set(ByVal value As Boolean)
			HttpContext.Current.Session.Add("FixRelativePaths", value)
		End Set
	End Property

#End Region

#Region "Version Up External Files"

	''' <summary>
	''' Sets the version of all javascript and css files on a page.
	''' </summary>
	''' <param name="HTML">The HTML to replace the javascript and css files version numbers.</param>
	''' <param name="bFixRelativePaths">Sets value of FixRelativePaths.</param>
	''' <param name="bIgnoreJavascript">if set to <c>true</c>, javascript files won't be updated.</param>
	Public Shared Function SetVersionLinkedFiles(ByVal HTML As String, Optional ByVal bFixRelativePaths As Boolean = False, Optional ByVal bIgnoreJavascript As Boolean = False) As String

		'set boolean on session
		VersionLinkedFiles.FixRelativePaths = bFixRelativePaths

		Dim sRegex As String = ""
		If bIgnoreJavascript Then
			sRegex = "<link[^>]*/>"
		Else
			sRegex = "<script\s[^>]*>\s?<\/script>|<link[^>]*/>"
		End If

		'declare a new regex object which has a delegate as it third? parameter. See.... translation class in intuitive project
		Dim oRegexLink As New Regex(sRegex, RegexOptions.Compiled Or RegexOptions.Singleline)
		HTML = oRegexLink.Replace(HTML, AddressOf VersionLinkedFileDelegate)

		Return HTML

	End Function

	''' <summary>
	''' Replaces the matched tag with a versioned file number.
	''' </summary>
	''' <param name="oMatch">The item to replace values of.</param>
	Public Shared Function VersionLinkedFileDelegate(ByVal oMatch As System.Text.RegularExpressions.Match) As String
		If oMatch.Value.ToLower.Contains("canonical") Then Return oMatch.Value
		Dim oRegex As New Regex("(?<=(?:src=""|href="")).*(?="")", RegexOptions.Compiled)
		Dim sReturn As String = oRegex.Replace(oMatch.Value, AddressOf VersionLinkedFileLocationDelegate)
		Return sReturn
	End Function

	''' <summary>
	''' Takes a matched file and adds a version number to the end if possible
	''' </summary>
	''' <param name="oMatch">The item to replace values of.</param>
	''' <exception cref="System.Exception">
	''' cannot version non relative files
	''' or
	''' file cannot be versioned
	''' </exception>
	Public Shared Function VersionLinkedFileLocationDelegate(ByVal oMatch As System.Text.RegularExpressions.Match) As String

		Try
			Dim sMatchedValue As String = oMatch.Value

			'this would naturally fail somewhere below but decided to explicitly trap these ones
			If sMatchedValue.StartsWith("http") Then Throw New Exception("cannot version non relative files")

			'split the relative path so that we can find the physical file
			Dim iQueryStringIndex As Integer = sMatchedValue.IndexOf("?")
			If iQueryStringIndex > -1 Then sMatchedValue = sMatchedValue.Remove(iQueryStringIndex)

			'sort out improper paths - if we want to fix relative paths then find our current location otherwise set to the root
			'./ should always find current file location but in order to not break things this is optional
			Dim sMatchPrefix As String = "~"
			If VersionLinkedFiles.FixRelativePaths AndAlso Not sMatchedValue.StartsWith("/") Then
				sMatchPrefix = "./"
			ElseIf Not sMatchedValue.StartsWith("/") Then
				sMatchedValue = "/" & sMatchedValue
			End If

			'for some websites we need to serve the whole website under a root path eg holidays.domain.com/{holidays}/search-results
			'this is achieved by some code that runs from the config setting
			'and also by a regex entry in the IIS Url rewrite module
			'if this has been set up then edit the file path accordingly
			'the root path should be in the format: "/holidays" with no trailing slash
			Dim sRootPath As String = SafeString(AppSettings("RootPath"))

			'get the filepath to the relative file as it is one of ours
			Dim sFilePath As String = HttpContext.Current.Server.MapPath(sMatchPrefix & sMatchedValue.Substring(sRootPath.Length))

			'try to find this in an existing cache
			If Not LinkedFileContentCache.ContainsKey(sFilePath) Then

				'if we cannot open this one then do not version it up
				If Not File.Exists(sFilePath) Then Throw New Exception("file cannot be versioned")

				'add the hash version of the contents to a cache
				LinkedFileContentCache.Add(sFilePath, HashCode.GetHash(System.IO.File.ReadAllText(sFilePath)))

			End If

			'return the file with a version number attached
			Return sMatchedValue & "?ivn=" & SafeInt(LinkedFileContentCache(sFilePath))

		Catch ex As Exception

			'do NOT convert this one to lower
			Return oMatch.Value

		End Try

	End Function

#End Region

End Class
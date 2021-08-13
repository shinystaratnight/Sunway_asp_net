Imports System.Text.RegularExpressions
Imports System.Xml

''' <summary>
''' Provides an interface to translate Xsl templates, rather than translating HTML at runtime
''' It should handle nested Xsl includes
''' </summary>
''' <remarks></remarks>
Public Class XslTranslation

#Region "Constants"

    Private Const XSL_INCLUDE_REGEX As String = "<xsl:include.*\/>"
	Private Const BASE_TRANSLATED_TEMPLATES_DIRECTORY As String = "\TranslatedTemplates\"
	Private Const BASE_TRANSLATED_LIBRARY_TEMPLATES_DIRECTORY As String = "\TranslatedTemplates\WidgetsLibrary\"
	Private Const CACHE_TEMPLATE_TIME As Integer = 720
	Private Const CACHE_PREFIX As String = "__pretranslated_templates_"

#End Region

#Region "Cache"

	Private Function GetCache(Key As String) As TranslatedTemplate
        Key = CACHE_PREFIX & Key
		Dim cachedTemplate As TranslatedTemplate = Functions.GetCache(Of TranslatedTemplate)(Key)
		If Not cachedTemplate Is Nothing Then
			Return cachedTemplate
		Else
			Return Nothing
        End If
    End Function

    Private Sub SetCache(Key As String, Template As TranslatedTemplate)
        Key = CACHE_PREFIX & Key
        Intuitive.Functions.AddToCache(Key, Template, CACHE_TEMPLATE_TIME)
    End Sub


#End Region

#Region "public api"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Xsl">string of xsl to translate</param>
    ''' <param name="WidgetName">widget name, must be set</param>
    ''' <returns>Translated template, only uses root attribute but do this so caching is consistent</returns>
    ''' <remarks>Assumes this is a library widget with xsl from resource file being passed in so we ignore any includes. Includes property should be empty</remarks>
    Public Function TranslateTemplateFromString(Xsl As String, WidgetName As String) As TranslatedTemplate

        Dim originalXsl As String = Xsl
        Dim newPath As String = HttpContext.Current.Server.MapPath(BASE_TRANSLATED_LIBRARY_TEMPLATES_DIRECTORY & WidgetName & "\" & WidgetName & ".xsl")
        Dim translatedTemplate As New TranslatedTemplate
        translatedTemplate.TranslatedPath = newPath

        If GetCache(newPath) Is Nothing Then
            Dim translatedXsl As String = Intuitive.Web.Translation.TranslateHTML(originalXsl, True)
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newPath))
            Intuitive.FileFunctions.TextToFile(newPath, translatedXsl)
            SetCache(newPath, translatedTemplate)
        Else
            translatedTemplate = GetCache(newPath)
        End If

        Return translatedTemplate

    End Function

    ''' <summary>
    ''' Pre translates Xsl templates so we do not translate whole HTML on every page load
    ''' </summary>
    ''' <param name="XslFilePath">Absolute file path to top level file</param>
    ''' <returns>Translated template root file location and also file locations of included xsl templates</returns>
    ''' <remarks></remarks>
    Public Function TranslateTemplateFromPath(XslFilePath As String) As TranslatedTemplate


        'add base template to a cache, with a timestamp expiry + includes dependencies so we don't have to run regex each time
        'instead we just get the file path, see it is cached, get the other files it depends on if cached, re-translate if not
        'and return straight off


        'take base template file path
        'get text from file
        Dim relativeFilePath As String = GetPathRelativeToAppRoot(XslFilePath)
        Dim languageSpecificPath As String = GetLanguageSpecificFilePath(relativeFilePath)
        Dim newPath As String = HttpContext.Current.Server.MapPath(BASE_TRANSLATED_TEMPLATES_DIRECTORY &
                                                                   languageSpecificPath)
        Dim translatedTemplate As New TranslatedTemplate
        translatedTemplate.TranslatedPath = newPath
        translatedTemplate.OriginalPath = XslFilePath

        CheckCacheAndTranslateFile(XslFilePath, newPath, translatedTemplate)

        Return translatedTemplate

    End Function


#End Region

#Region "internal workings"

    Private Shared LockDictionary As New Dictionary(Of String, Object)

    Private Sub CheckCacheAndTranslateFile(originalPath As String, newPath As String, ByRef TranslatedTemplate As TranslatedTemplate)

        If Not LockDictionary.ContainsKey(newPath) Then
            LockDictionary.Add(newPath, New Object)
        End If

        'if file is cached, dont regex and translate as we already have the info
        'double checking here to be thread safe
        If GetCache(newPath) Is Nothing Then

            SyncLock LockDictionary(newPath)

                If GetCache(newPath) Is Nothing Then
                    'regex out the includes, add to collection and remove from base template file text
                    Dim xslText As String = Intuitive.FileFunctions.FileToText(originalPath)
                    Dim xslIncludeRegex As New Regex(XSL_INCLUDE_REGEX, RegexOptions.Compiled)
                    Dim includes As MatchCollection = xslIncludeRegex.Matches(xslText)
                    xslText = xslIncludeRegex.Replace(xslText, "")


                    'translate base template and save in translated folder with same directory structure and _languageId appended
                    Dim translatedXsl As String = Intuitive.Web.Translation.TranslateHTML(xslText, True)
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newPath))
                    Intuitive.FileFunctions.TextToFile(newPath, translatedXsl)


                    'recursively call this function with each include so they are all also translated and saved
                    'DISCLAIMER
                    'recursive calling with locks is dangerous, the top level lock will not be released until the whole function tree
                    ' returns. This has been written with this in mind. The trees are very short and will only translate each file
                    ' every 12 hours. The number of situations where we may get a lock wait should be very small. It is a trade off
                    ' we have made.
                    For Each include As Match In includes
                        TranslateInclude(include.ToString, originalPath, TranslatedTemplate)
                    Next

                    SetCache(newPath, TranslatedTemplate)
                End If

            End SyncLock

        Else

            TranslatedTemplate = GetCache(newPath)
            'recursively call this function with each include so they are all also translated and saved
            For Each include As String In TranslatedTemplate.OriginalIncludePaths
                TranslateTemplateFromPath(include)
            Next

        End If


    End Sub


    Private Sub TranslateInclude(IncludePath As String, XslFilePath As String, ByRef TranslatedTemplate As TranslatedTemplate)
        'get relative file path 
        Dim xslInclude As String = IncludePath
        Dim filePathRegex As New Regex(""".*""")
        Dim filePathMatch As String = filePathRegex.Match(xslInclude).ToString
        'remove the surrounding quote marks
        Dim filepath As String = filePathMatch.Substring(1, filePathMatch.Length - 2)
        'the file path is relative to the current file directory so work out the actual file path relative to the root 
        Dim rootRelativeFilePath As String = GetFullPath(XslFilePath, filepath)
        Dim includeTranslatedTemplate As TranslatedTemplate = TranslateTemplateFromPath(rootRelativeFilePath)
        TranslatedTemplate.TranslatedIncludePaths.Add(includeTranslatedTemplate.TranslatedPath)
        TranslatedTemplate.TranslatedIncludePaths.AddRange(includeTranslatedTemplate.TranslatedIncludePaths)
    End Sub

    Private Function GetPathRelativeToAppRoot(AbsoluteFilePath As String) As String
        Dim aFileParts() As String = AbsoluteFilePath.Split(New String() {HttpContext.Current.Server.MapPath("/")}, StringSplitOptions.None)
        If aFileParts.Length > 1 Then
            Return aFileParts(1)
        End If
        Return ""
    End Function

    Private Function GetLanguageSpecificFilePath(RelativeFilePath As String) As String

        Dim directory As String = System.IO.Path.GetDirectoryName(RelativeFilePath)
        Dim newFileName As String = System.IO.Path.GetFileNameWithoutExtension(RelativeFilePath) & "_" & BookingBase.Params.LanguageID & ".xsl"
        Return directory & "/" & newFileName

    End Function

    Private Function GetFullPath(CurrentPath As String, PathToTraverse As String) As String

        '  get current directory
        Dim currentDirectory As String = System.IO.Path.GetDirectoryName(CurrentPath)


        ' get the combined path 
        Dim combinedPath As String = System.IO.Path.Combine(currentDirectory, PathToTraverse)


        'return the full path 
        Return System.IO.Path.GetFullPath(combinedPath)


    End Function



#End Region

#Region "Related classes"

    Public Class TranslatedTemplate
        Public TranslatedPath As String
        Public OriginalPath As String
        Public TranslatedIncludePaths As New List(Of String)
        Public OriginalIncludePaths As New List(Of String)
    End Class


#End Region


End Class



Imports System.Text.RegularExpressions

''' <summary>
''' Class containing functions to perform for translations
''' </summary>
Public Class TranslationDelegate

	''' <summary>
	''' Specifies that the language is the system language
	''' </summary>
	Private IsDefaultLanguage As Boolean

	''' <summary>
	''' Specifies to translate even when the language is the default language
	''' </summary>
	Private TranslateDefaultLanguage As Boolean

	''' <summary>
	''' The language id of the language
	''' </summary>
	Private LanguageID As Integer

	''' <summary>
	''' The culture code for the language
	''' </summary>
	Private CultureCode As String

	''' <summary>
	''' a prefix which is prepended to the group to define an alternative translation set
	''' </summary>
	Private GroupPrefix As String

	''' <summary>
	''' Specifies whether to suppress translations
	''' </summary>
	Private SuppressTranslation As Boolean

	''' <summary>
	''' Specifies whether text should be right to left 
	''' </summary>
	Private RightToLeftText As Boolean

	''' <summary>
	''' The count of self closing tags
	''' </summary>
	Public SelfClosingTagCount As Integer = 0

	''' <summary>
	''' The count of normal closing tags
	''' </summary>
	Public NormalClosingTagCount As Integer = 0

	''' <summary>
	''' The select tag count
	''' </summary>
	Public SelectTagCount As Integer = 0

	''' <summary>
	''' The count of empty trans tags
	''' </summary>
	Public EmptyTransTagCount As Integer = 0

	''' <summary>
	''' The collection of translation items
	''' </summary>
	Public TranslationItems As TranslationItems = New TranslationItems

	''' <summary>
	''' Gets the diagnostic mode.
	''' </summary>
	Public ReadOnly Property DiagnosticMode As Multilingual.DiagnosticMode

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean,
	   ByVal bTranslateDefaultLanguage As Boolean)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, String.Empty, Multilingual.DiagnosticMode.None, bTranslateDefaultLanguage, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="bSuppressTranslation">if set to <c>true</c>, translation will be suppressed.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean,
	   ByVal bTranslateDefaultLanguage As Boolean, bSuppressTranslation As Boolean)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, String.Empty, Multilingual.DiagnosticMode.None, bTranslateDefaultLanguage, bSuppressTranslation, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, String.Empty, Multilingual.DiagnosticMode.None, False, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean, ByVal DiagnosticMode As Multilingual.DiagnosticMode,
				ByVal bTranslateDefaultLanguage As Boolean)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, String.Empty, DiagnosticMode, bTranslateDefaultLanguage, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean, ByVal DiagnosticMode As Multilingual.DiagnosticMode)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, String.Empty, DiagnosticMode, False, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean, ByVal GroupPrefix As String,
	 ByVal bTranslateDefaultLanguage As Boolean)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, GroupPrefix, Multilingual.DiagnosticMode.None, bTranslateDefaultLanguage, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	Public Sub New(ByVal iLanguageID As Integer, ByVal sCultureCode As String, ByVal bIsDefaultLanguage As Boolean, ByVal GroupPrefix As String)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, GroupPrefix, Multilingual.DiagnosticMode.None, False, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="sGroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	Public Sub New(
		ByVal iLanguageID As Integer,
		ByVal sCultureCode As String,
		ByVal bIsDefaultLanguage As Boolean,
		ByVal sGroupPrefix As String,
		ByVal DiagnosticMode As Multilingual.DiagnosticMode)
		Me.New(iLanguageID, sCultureCode, bIsDefaultLanguage, sGroupPrefix, DiagnosticMode, False, False, False)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="TranslationDelegate"/> class.
	''' </summary>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="sGroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="bSuppressTranslation">if set to <c>true</c>, translation will be suppressed.</param>
	Public Sub New(
		ByVal iLanguageID As Integer,
		ByVal sCultureCode As String,
		ByVal bIsDefaultLanguage As Boolean,
		ByVal sGroupPrefix As String,
		ByVal DiagnosticMode As Multilingual.DiagnosticMode,
		ByVal bTranslateDefaultLanguage As Boolean,
		ByVal bSuppressTranslation As Boolean,
		ByVal RightToLeftText As Boolean)

		Me.LanguageID = iLanguageID
		Me.CultureCode = sCultureCode
		Me.IsDefaultLanguage = bIsDefaultLanguage
		Me.TranslateDefaultLanguage = bTranslateDefaultLanguage
		Me.GroupPrefix = sGroupPrefix
		Me.DiagnosticMode = DiagnosticMode
		Me.SuppressTranslation = bSuppressTranslation
		Me.RightToLeftText = RightToLeftText

		'if suppress translations is set. override to default language
		If bSuppressTranslation Then Me.IsDefaultLanguage = True
	End Sub

	''' <summary>
	''' Gets translation attributes from specified HTML.
	''' </summary>
	''' <param name="TagName">Unused</param>
	''' <param name="TagHTML">The HTML to get the attributes from.</param>
	Public Shared Function GetAttributes(ByVal TagName As String, ByVal TagHTML As String) As MultilingualAttributes

		Dim oReturn As New MultilingualAttributes
		Dim bSpecificAttributes As Boolean = False

		Dim oAttributeRegex As MatchCollection = Regex.Matches(TagHTML, "mlattributes=""(?<attribute>[^""]*?)""", RegexOptions.Compiled)

		If oAttributeRegex.Count > 0 Then
			oReturn.TranslateContent = False

			For Each oMatch As Match In oAttributeRegex
				Dim sAttributes As String() = oMatch.Groups("attribute").Value.ToString.Trim.ToLower.Split("|".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
				For Each sAttribute As String In sAttributes
					Select Case sAttribute
						Case "tagcontents"
							oReturn.TranslateContent = True
							bSpecificAttributes = True
						Case "alt", "caption", "title", "value", "content"
							oReturn.Add(sAttribute)
							bSpecificAttributes = True
						Case Else
							'nothing as we cannot process anything else - or can we actually process anything?
					End Select
				Next
			Next
		End If

		If Not bSpecificAttributes Then
			oReturn.TranslateContent = True
			oReturn.Add("alt")
			oReturn.Add("title")
			oReturn.Add("caption")
			oReturn.Add("value")
			oReturn.Add("content")
		End If

		Return oReturn

	End Function

#Region "Replace Delegate Methods"
	'tag process - <tag ml="group/key">lookup</tag> returns <tag>translation</tag>
	''' <summary>
	''' Function that gets performed on all matching tags, translates the content of the tag
	''' </summary>
	''' <param name="m">The match.</param>
	Public Function TagProcess(ByVal m As Match) As String

		'some diagnostics
		If Me.DiagnosticMode <> Multilingual.DiagnosticMode.None Then Me.NormalClosingTagCount += 1
		Dim dStart As DateTime = Date.Now

		Dim sTagHTML As String = m.Value
		Dim sOriginal As String = sTagHTML
		Dim bError As Boolean = False

		Try

			Dim sTagContents As String = m.Groups("text").Value
			Dim sTagName As String = m.Groups("tag").Value

			If sTagName.ToLower <> "select" Then

				Dim sTranslation As String = sTagContents

				Dim oMultilingualAttributes As MultilingualAttributes = TranslationDelegate.GetAttributes(sTagName, sTagHTML)

				Dim oGroupCode As New GroupCodeSplit(m.Groups("mltag").Value)

				If oMultilingualAttributes.TranslateContent Then

					If oGroupCode.Key = "" Then
						oGroupCode.Key = sTagContents.Trim.Replace(Environment.NewLine, "")
						oGroupCode.Key = Regex.Replace(oGroupCode.Key, "\s{2,}", " ", RegexOptions.Compiled)
					End If

					'set the translation as the tagcontents by default
					If Not Me.SuppressTranslation Then
						If Not Me.IsDefaultLanguage AndAlso oGroupCode.LanguageID = 0 Then
							sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix,
							 oGroupCode.Key, oGroupCode.SubKey, Me.LanguageID, Me.IsDefaultLanguage, Me.TranslateDefaultLanguage)
						ElseIf oGroupCode.LanguageID > 0 Then
							sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix,
							oGroupCode.Key, oGroupCode.SubKey, oGroupCode.LanguageID, False, Me.TranslateDefaultLanguage)
						End If
					End If

					'process the parameters
					sTranslation = Me.ProcessParameters(sTagHTML, sTranslation)
				End If

				'process any attributes (titles on anchors)
				sTagHTML = Me.ProcessAttributes(m, oMultilingualAttributes)

				'replace the text in the middle of the tags and take out the ml
				sTagHTML = Regex.Replace(sTagHTML, "(?<=>).+(?=<)", sTranslation, RegexOptions.Singleline Or RegexOptions.Compiled)
				sTagHTML = TranslationDelegate.RemoveMultilingualAttributes(sTagHTML)
				'Replace any removed characters
				sTagHTML = Me.UnprocessParameters(sTagHTML)
				If oGroupCode.Key <> sTranslation Then
					sTagHTML = TranslationDelegate.AllignByLanguageDirection(sTagHTML, Me.RightToLeftText)
				End If


			End If

		Catch ex As Exception

			'if we error then return the original
			sTagHTML = sOriginal
			bError = True

		Finally

			'stick in the diagnostics
			If Me.DiagnosticMode = Multilingual.DiagnosticMode.Full Then
				Dim dEnd As DateTime = Date.Now
				Dim nDuration As Double = dEnd.Subtract(dStart).TotalMilliseconds
				Me.TranslationItems.Add(TranslationItemType.Normal, nDuration, sOriginal, sTagHTML, False, bError)
			End If

		End Try

		'return translated stuff
		Return sTagHTML

	End Function

	'self closing tag process <tag ml="group/key" />, translates alt, value and title tags
	''' <summary>
	''' Process that gets performed on all matching self closing tags.
	''' </summary>
	''' <param name="m">The match.</param>
	Public Function SelfClosingTagProcess(ByVal m As Match) As String

		'some diagnostics
		Dim dStart As DateTime = Date.Now
		If Me.DiagnosticMode <> Multilingual.DiagnosticMode.None Then Me.SelfClosingTagCount += 1

		Dim sTagHTML As String = m.Value
		Dim sTagName As String = m.Groups("tag").Value
		Dim oMultilingualAttributes As MultilingualAttributes = TranslationDelegate.GetAttributes(sTagName, sTagHTML)

		If Me.DiagnosticMode = Multilingual.DiagnosticMode.Full Then
			Dim dEnd As DateTime = Date.Now
			Dim nDuration As Double = dEnd.Subtract(dStart).TotalMilliseconds
			Me.TranslationItems.Add(TranslationItemType.Normal, nDuration)
		End If

		Return TranslationDelegate.RemoveMultilingualAttributes(Me.ProcessAttributes(m, oMultilingualAttributes))

	End Function

	'select process - sorts out the options
	''' <summary>
	''' Process that gets performed on all matching select tags, goes through each option in the select and translates them.
	''' </summary>
	''' <param name="m">The match.</param>
	Public Function SelectProcess(ByVal m As Match) As String

		Dim dStart As DateTime = Date.Now
		If Me.DiagnosticMode <> Multilingual.DiagnosticMode.None Then Me.SelectTagCount += 1

		Dim sTagHTML As String = m.Value

		Dim oGroupCode As New GroupCodeSplit(m.Groups("mltag").Value)

		'find all the option matches in the string
		Dim oMatches As MatchCollection = Regex.Matches(sTagHTML, "<option[^>]*?>(?<text>.*?)</option>", RegexOptions.Singleline)

		'loop through them all
		For Each oMatch As Match In oMatches

			Dim sTagText As String = oMatch.Groups("text").Value

			'set the translation as the tagcontents by default
			Dim sTranslation As String = sTagText
			If Not Me.SuppressTranslation Then
				If Not Me.IsDefaultLanguage AndAlso oGroupCode.LanguageID = 0 Then
					sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix, oGroupCode.Key, sTagText, Me.LanguageID, Me.IsDefaultLanguage, Me.TranslateDefaultLanguage)
				ElseIf oGroupCode.LanguageID > 0 Then
					sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix, oGroupCode.Key, sTagText, oGroupCode.LanguageID, False, Me.TranslateDefaultLanguage)
				End If
			End If

			'regex replace the old option HTML to new HTML, then string replace back into the select HTML
			Dim sOptionTag As String = oMatch.Value
			Dim sNewOptionTag As String = Regex.Replace(sOptionTag, "(?<=>)[^<]+(?=<)", sTranslation, RegexOptions.Compiled)

			sTagHTML = sTagHTML.Replace(sOptionTag, sNewOptionTag)

		Next

		Dim sReturnHTML As String = TranslationDelegate.RemoveMultilingualAttributes(sTagHTML)

		If Me.DiagnosticMode = Multilingual.DiagnosticMode.Full Then
			Dim dEnd As DateTime = Date.Now
			Dim nDuration As Double = dEnd.Subtract(dStart).TotalMilliseconds
			Me.TranslationItems.Add(TranslationItemType.Normal, nDuration)
		End If

		Return sReturnHTML

	End Function

	''' <summary>
	''' Function to be performed on empty trans tags, increments the <see cref="EmptyTransTagCount"/> when diagnostic mode isn't none.
	''' </summary>
	''' <param name="m">The match.</param>
	Public Function EmptyTransDelegate(ByVal m As Match) As String
		'some diagnostics
		Dim dStart As DateTime = Date.Now
		If Me.DiagnosticMode <> Multilingual.DiagnosticMode.None Then Me.EmptyTransTagCount += 1

		If Me.DiagnosticMode = Multilingual.DiagnosticMode.Full Then
			Dim dEnd As DateTime = Date.Now
			Dim nDuration As Double = dEnd.Subtract(dStart).TotalMilliseconds
			Me.TranslationItems.Add(TranslationItemType.EmptyTrans, nDuration)
		End If

		Return String.Empty

	End Function

#End Region

#Region "Processing Attributes"

	''' <summary>
	''' Processes the attributes, performs translation, inserts parameters into attributes.
	''' </summary>
	''' <param name="m">The match.</param>
	''' <param name="MultilingualAttributes">The multilingual attributes.</param>
	Private Function ProcessAttributes(ByVal m As Match, ByVal MultilingualAttributes As MultilingualAttributes) As String

		Dim sTagHTML As String = m.Value

		'get the tag name
		Dim sTagName As String = Regex.Match(sTagHTML, "^\<(?<tagname>[a-zA-Z]+)\s", RegexOptions.Compiled).Groups("tagname").Value.ToString.ToLower

		Dim bIncludeValueAttribute As Boolean = True
		If sTagName = "input" Then
			'only do the value attribute on buttons or submit inputs
			Dim sType As String = Regex.Match(sTagHTML, "type=\""(?<type>[a-zA-Z]+)\""", RegexOptions.Compiled).Groups("type").ToString.ToLower
			bIncludeValueAttribute = (sType = "button" OrElse sType = "submit" OrElse sType = "hidden")
		End If

		'work out which attributes we are going to replace
		Dim sAttributes As String = ""
		For Each sAttribute As String In MultilingualAttributes
			If sAttribute <> "value" OrElse (sAttribute = "value" AndAlso bIncludeValueAttribute) Then
				sAttributes &= sAttribute & "|"
			End If
		Next
		sAttributes = Chop(sAttributes)

		If sAttributes <> String.Empty Then

			Dim sAttributeRegexPattern As String = String.Format("\s(?<tag>{0})=\""(?<att>[^""]+)\""", sAttributes)

			For Each oMatch As Match In Regex.Matches(sTagHTML, sAttributeRegexPattern, RegexOptions.IgnoreCase And RegexOptions.Compiled)

				Dim sAttributeName As String = oMatch.Groups("tag").Value
				Dim sAttributeValue As String = oMatch.Groups("att").Value

				Dim oGroupCode As New GroupCodeSplit(m.Groups("mltag").Value)
				If oGroupCode.Key = "" Then oGroupCode.Key = sAttributeValue

				'set the translation as the attribute value by default
				Dim sTranslation As String = sAttributeValue
				If Not Me.SuppressTranslation Then
					If Not Me.IsDefaultLanguage AndAlso Me.LanguageID > 0 Then
						sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix, oGroupCode.Key, oGroupCode.SubKey, Me.LanguageID, Me.IsDefaultLanguage, Me.TranslateDefaultLanguage)
					ElseIf oGroupCode.LanguageID > 0 Then
						sTranslation = Multilingual.GetCustomTranslation(oGroupCode.Group, Me.GroupPrefix, oGroupCode.Key, oGroupCode.SubKey, oGroupCode.LanguageID, Me.IsDefaultLanguage, Me.TranslateDefaultLanguage)
					End If
				End If

				'if there is an mlparams tag then split the params out and string format
				Dim oParamRegex As MatchCollection = Regex.Matches(sTagHTML, "mlparams=""(?<params>[^""]+?)""", RegexOptions.Compiled)
				If oParamRegex.Count > 0 Then
					'string format those badboys
					Dim aParams As String() = oParamRegex(0).Groups("params").Value.Split("|"c)
					sTranslation = String.Format(sTranslation, aParams)
				End If

				Dim sValueMatchString As String = String.Format("\s{0}=\""[^""]+\""", sAttributeName)
				sTagHTML = Regex.Replace(sTagHTML, sValueMatchString, String.Format(" {0}=""{1}""", sAttributeName, sTranslation), RegexOptions.Compiled)

			Next

		End If

		Return sTagHTML

	End Function

#End Region

#Region "Processing Parameters"

	''' <summary>
	''' Processes the parameters to be inserted into translated text.
	''' </summary>
	''' <param name="sTagHTML">The HTML to get the parameters from.</param>
	''' <param name="sTranslation">The translation.</param>
	Private Function ProcessParameters(ByVal sTagHTML As String, ByVal sTranslation As String) As String

		Try

			'if there is an mlparams tag then split the params out and string format
			Dim oParamRegex As MatchCollection = Regex.Matches(sTagHTML, "mlparams=""(?<params>[^""]*?)""")
			If oParamRegex.Count > 0 Then

				'string format those badboys
				Dim aParams As String() = oParamRegex(0).Groups("params").Value.Split("|"c)
				For iParams As Integer = 0 To aParams.Length - 1
					Dim sParam As String = aParams(iParams)
					Dim aFormatParam As String() = sParam.Split("~"c)

					'Make safe the regex reserved character $0
					aParams(iParams) = aParams(iParams).Replace("$0", "¬¬")

					'format the parameters according to the formats provided
					If aFormatParam.Length = 2 Then
						If DateFunctions.IsDate(aFormatParam(0)) Then
							aParams(iParams) = Multilingual.DisplayDate(DateFunctions.SafeDate(aFormatParam(0)), aFormatParam(1), Me.LanguageID, Me.CultureCode, Me.IsDefaultLanguage, , Me.TranslateDefaultLanguage)
						Else
							aParams(iParams) = String.Format(aFormatParam(0), aFormatParam(1))
						End If
					ElseIf aFormatParam.Length = 3 AndAlso DateFunctions.IsDate(aFormatParam(0)) Then
						aParams(iParams) = Multilingual.DisplayDate(DateFunctions.SafeDate(aFormatParam(0)), aFormatParam(1), Me.LanguageID, Me.CultureCode, Me.IsDefaultLanguage, aFormatParam(2))
					End If

				Next

				sTranslation = String.Format(sTranslation, aParams)

			End If

		Catch ex As Exception
			'bubble the exception back up to the parent
			Throw ex

		End Try

		Return sTranslation

	End Function

	''' <summary>
	''' Replaces '¬¬' WITH '$0' in specified html
	''' </summary>
	''' <param name="sTagHTML">The s tag HTML.</param>
	Private Function UnprocessParameters(ByVal sTagHTML As String) As String

		Try

			sTagHTML = sTagHTML.Replace("¬¬", "$0")

		Catch ex As Exception
			'bubble the exception back up to the parent
			Throw ex

		End Try

		Return sTagHTML

	End Function

#End Region

#Region "Remove ml attributes"

	''' <summary>
	''' Removes the multilingual attributes from specified HTML.
	''' </summary>
	''' <param name="sTagHTML">The HTML to remove the attributes from.</param>
	Private Shared Function RemoveMultilingualAttributes(ByVal sTagHTML As String) As String
		Return Regex.Replace(sTagHTML, "\s(ml|mlparams|mlattributes)=\""[^""]+\""", "", RegexOptions.Singleline And RegexOptions.Compiled)
	End Function

#End Region

#Region "Add directional attribute"

	Private Shared Function AllignByLanguageDirection(ByVal sTagHTML As String, ByVal RightToLeftText As Boolean) As String

		'add the allignment style to the HTML tag

		If RightToLeftText Then
			Dim r As New Regex(">")
			sTagHTML = r.Replace(sTagHTML, " style=""text-align:right;direction:rtl;"">", 1)
		End If

		Return sTagHTML

	End Function

#End Region

#Region "Group Code Split"

	'group code split
	''' <summary>
	''' Class that takes a GroupCode and splits it into it's components
	''' </summary>
	Public Class GroupCodeSplit
		''' <summary>
		''' The group
		''' </summary>
		Public Group As String = ""
		''' <summary>
		''' The group's key
		''' </summary>
		Public Key As String = ""
		''' <summary>
		''' The group's sub key
		''' </summary>
		Public SubKey As String = ""
		''' <summary>
		''' The group's language id
		''' </summary>
		Public LanguageID As Integer = 0

		''' <summary>
		''' Initializes a new instance of the <see cref="GroupCodeSplit"/> class.
		''' </summary>
		''' <param name="sGroupCode">The group code.</param>
		Public Sub New(ByVal sGroupCode As String)
			Dim aGroupMatch() As String = sGroupCode.Split(";"c)
			Me.Group = aGroupMatch(0)
			If aGroupMatch.Length > 1 Then Me.Key = aGroupMatch(1)
			If aGroupMatch.Length > 2 Then Me.SubKey = aGroupMatch(2)
			If aGroupMatch.Length > 3 Then Me.LanguageID = aGroupMatch(3).ToSafeInt
		End Sub
	End Class

#End Region

#Region "Translation Attributes"

	''' <summary>
	''' Class containing a list of Multilingual attributes
	''' </summary>
	''' <seealso cref="System.Collections.Generic.List" />
	Public Class MultilingualAttributes
		Inherits Generic.List(Of String)
		''' <summary>
		''' Specifies whether to translate the content of this list
		''' </summary>
		Public TranslateContent As Boolean = True
	End Class

#End Region

End Class
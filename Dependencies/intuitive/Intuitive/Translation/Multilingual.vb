Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Configuration.ConfigurationManager

Public Class Multilingual

#Region "Translated Page"

	''' <summary>
	''' Enum for possible diagnostic modes
	''' </summary>
	Public Enum DiagnosticMode
		None = 0
		Normal = 1
		Full = 2
	End Enum

	''' <summary>
	''' Class containing information about a translated page
	''' </summary>
	Public Class TranslatedPage

		''' <summary>
		''' The HTML of the translated page
		''' </summary>
		Public HTML As String = String.Empty
		''' <summary>
		''' How long a page translation took
		''' </summary>
		Public Duration As Double = 0
		''' <summary>
		''' The Translation Delegate
		''' </summary>
		Public TranslationDelegate As TranslationDelegate = Nothing

		''' <summary>
		''' Gets the count of translation items on the translation delegate.
		''' </summary>
		Public ReadOnly Property Count As Integer
			Get
				Try
					Return Me.TranslationDelegate.TranslationItems.Count
				Catch ex As Exception
					Return 0
				End Try
			End Get
		End Property

		''' <summary>
		''' Gets the maximum duration of the translation items on the translation delegate.
		''' </summary>
		Public ReadOnly Property MaxDuration As Double
			Get
				Try
					Return Me.TranslationDelegate.TranslationItems.Max(Function(o) o.Duration)
				Catch ex As Exception
					Return 0
				End Try
			End Get
		End Property

		''' <summary>
		''' Gets the average duration of the translation items on the translation delegate.
		''' </summary>
		Public ReadOnly Property AverageDuration As Double
			Get
				Try
					Return Me.TranslationDelegate.TranslationItems.Average(Function(o) o.Duration)
				Catch ex As Exception
					Return 0
				End Try
			End Get
		End Property

		''' <summary>
		''' Gets the longest length of the original html from the translation items on the translation delegate
		''' </summary>
		Public ReadOnly Property MaxHTMLLength As Integer
			Get
				Try
					Return Me.TranslationDelegate.TranslationItems.Max(Function(o) o.Original.Length)
				Catch ex As Exception
					Return 0
				End Try
			End Get
		End Property

		''' <summary>
		''' Gets the count of the translation items that have errored.
		''' </summary>
		Public ReadOnly Property ErrorCount As Integer
			Get
				Try
					Return Me.TranslationDelegate.TranslationItems.Where(Function(o) o.Errored).Count
				Catch ex As Exception
					Return 0
				End Try
			End Get
		End Property

	End Class

#End Region

#Region "Get Translation"

	''' <summary>
	''' Gets the cache key for the specified language id.
	''' </summary>
	Private Shared ReadOnly Property CacheKey(ByVal LanguageID As Integer) As String
		Get
			Return "MultilingualTranslations_" & LanguageID.ToString
		End Get
	End Property

	'main get multilingual translation routine
	''' <summary>
	''' Gets the custom translation for the specified LookupKey against the specified language.
	''' </summary>
	''' <param name="LookupKey">The lookup key to get the translation for.</param>
	''' <param name="iLanguageID">The language id of the translations to retrieve.</param>
	''' <param name="IsDefaultLanguage">If set to <c>true</c>, will not translate.</param>
	''' <param name="bTranslateDefaultLanguage">If set to <c>true</c>, will translate even if IsDefaultLanguage is true.</param>
	Private Shared Function GetCustomTranslation(ByVal LookupKey As MultiLingualLookupKey,
	   ByVal iLanguageID As Integer,
	   ByVal IsDefaultLanguage As Boolean, ByVal bTranslateDefaultLanguage As Boolean) As String

		If LookupKey.Key.Trim = String.Empty Then Return LookupKey.DefaultLanguageValue

		If Not IsDefaultLanguage OrElse bTranslateDefaultLanguage Then
			Dim oLookups As MultilingualLookup = Multilingual.GetMultilingualLookups(iLanguageID)

			' if multilingual exists
			If oLookups.ContainsKey(LookupKey.CompoundKey) Then

				'get the translated version.
				Dim sTranslation As String = oLookups(LookupKey.CompoundKey)
				If sTranslation = String.Empty Then
					Return LookupKey.DefaultLanguageValue
				Else
					Return sTranslation
				End If

			Else

				Dim sConnectString As String = AppSettings.Get("BookingConnectString")

				'changed by DM to go through normal connect string
				If sConnectString = "" Then sConnectString = SQL.ConnectString 'AppSettings.Get("ConnectString")

				' insert multilingual
				SQL.ExecuteWithConnectString(sConnectString, "exec Translation_Add",
				  SQL.GetSqlValue(LookupKey.Group),
				  SQL.GetSqlValue(LookupKey.Key),
				  SQL.GetSqlValue(LookupKey.SubKey))

				' update cache with default language translation
				oLookups.AddKey(LookupKey.Group, LookupKey.Key, LookupKey.SubKey, LookupKey.DefaultLanguageValue)
				Functions.AddToCache(Multilingual.CacheKey(iLanguageID), oLookups, 50)
			End If
		End If

		Return LookupKey.DefaultLanguageValue

	End Function

	'main get multilingual translation routine
	''' <summary>
	''' Gets the custom translation of the specified lookup key against the specified language id.
	''' </summary>
	''' <param name="LookupKey">The lookup key to get the translation for.</param>
	''' <param name="iLanguageID">The language id of the translations to retrieve.</param>
	''' <param name="IsDefaultLanguage">If set to <c>true</c>, will not translate.</param>
	Private Shared Function GetCustomTranslation(ByVal LookupKey As MultiLingualLookupKey,
	   ByVal iLanguageID As Integer,
	   ByVal IsDefaultLanguage As Boolean) As String

		If LookupKey.Key.Trim = String.Empty Then Return LookupKey.DefaultLanguageValue

		If Not IsDefaultLanguage Then
			Dim oLookups As MultilingualLookup = Multilingual.GetMultilingualLookups(iLanguageID)

			' if multilingual exists
			If oLookups.ContainsKey(LookupKey.CompoundKey) Then

				'get the translated version.
				Dim sTranslation As String = oLookups(LookupKey.CompoundKey)
				If sTranslation = String.Empty Then
					Return LookupKey.DefaultLanguageValue
				Else
					Return sTranslation
				End If

			Else

				Dim sConnectString As String = AppSettings.Get("BookingConnectString")

				'changed by DM to go through normal connect string
				If sConnectString = "" Then sConnectString = SQL.ConnectString 'AppSettings.Get("ConnectString")

				' insert multilingual
				SQL.ExecuteWithConnectString(sConnectString, "exec Translation_Add",
				  SQL.GetSqlValue(LookupKey.Group),
				  SQL.GetSqlValue(LookupKey.Key),
				  SQL.GetSqlValue(LookupKey.SubKey))

				' update cache with default language translation
				oLookups.AddKey(LookupKey.Group, LookupKey.Key, LookupKey.SubKey, LookupKey.DefaultLanguageValue)
				Functions.AddToCache(Multilingual.CacheKey(iLanguageID), oLookups, 50)
			End If
		End If

		Return LookupKey.DefaultLanguageValue

	End Function

	'group and key
	''' <summary>
	''' Gets the custom translation.
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML.</param>
	''' <param name="Key">the unique individual key of the item being translated.</param>
	''' <param name="iLanguageID">the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal Key As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean,
	  ByVal bTranslateDefaultLanguage As Boolean) As String

		Return Multilingual.GetCustomTranslation(Group, String.Empty, Key, String.Empty, iLanguageID, bDefaultLanguage, bTranslateDefaultLanguage)
	End Function

	'group and key
	''' <summary>
	''' Gets the custom translation.
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML.</param>
	''' <param name="Key">the unique individual key of the item being translated.</param>
	''' <param name="iLanguageID">the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal Key As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean) As String

		Return Multilingual.GetCustomTranslation(Group, String.Empty, Key, String.Empty, iLanguageID, bDefaultLanguage, False)
	End Function

	'group, key and subkey
	''' <summary>
	''' Gets the custom translation.
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML.</param>
	''' <param name="Key">the unique individual key of the item being translated.</param>
	''' <param name="SubKey">the unique individual sub key of the item being translated.</param>
	''' <param name="iLanguageID">the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal Key As String,
	  ByVal SubKey As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean,
	  ByVal bTranslateDefaultLanguage As Boolean) As String
		Return Multilingual.GetCustomTranslation(New MultiLingualLookupKey(Group, Key, SubKey), iLanguageID, bDefaultLanguage, bTranslateDefaultLanguage)
	End Function

	'group, key and subkey
	''' <summary>
	''' Gets the custom translation.
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML.</param>
	''' <param name="Key">the unique individual key of the item being translated.</param>
	''' <param name="SubKey">the unique individual sub key of the item being translated.</param>
	''' <param name="iLanguageID">the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal Key As String,
	  ByVal SubKey As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean) As String
		Return Multilingual.GetCustomTranslation(New MultiLingualLookupKey(Group, Key, SubKey), iLanguageID, bDefaultLanguage, False)
	End Function

	''' <summary>
	''' This version allows you to specify a GroupPrefix. This allows the same HTML source code to be translated hence not
	''' requiring specific new HTML source, but it does allow for alternative translations to be returned in a special Group
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set</param>
	''' <param name="Key">the unique individual key of the item being translated</param>
	''' <param name="SubKey">the unique individual sub key of the item being translated</param>
	''' <param name="iLanguageID">the langauge to be translated into</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal GroupPrefix As String,
	  ByVal Key As String,
	  ByVal SubKey As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean,
	  ByVal bTranslateDefaultLanguage As Boolean) As String

		Dim sGroupKey As String = Group
		If GroupPrefix <> String.Empty Then sGroupKey = GroupPrefix & " " & Group

		Return Multilingual.GetCustomTranslation(New MultiLingualLookupKey(sGroupKey, Key, SubKey), iLanguageID, bDefaultLanguage, bTranslateDefaultLanguage)
	End Function

	''' <summary>
	''' This version allows you to specify a GroupPrefix. This allows the same HTML source code to be translated hence not
	''' requiring specific new HTML source, but it does allow for alternative translations to be returned in a special Group
	''' </summary>
	''' <param name="Group">The natural group name picked up from the source HTML.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="Key">the unique individual key of the item being translated.</param>
	''' <param name="SubKey">the unique individual sub key of the item being translated.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Shared Function GetCustomTranslation(ByVal Group As String,
	  ByVal GroupPrefix As String,
	  ByVal Key As String,
	  ByVal SubKey As String,
	  ByVal iLanguageID As Integer,
	  ByVal bDefaultLanguage As Boolean) As String

		Dim sGroupKey As String = Group
		If GroupPrefix <> String.Empty Then sGroupKey = GroupPrefix & " " & Group

		Return Multilingual.GetCustomTranslation(New MultiLingualLookupKey(sGroupKey, Key, SubKey), iLanguageID, bDefaultLanguage, False)
	End Function

#End Region

#Region "Date Translation"

	''' <summary>
	''' Converts a date to the desired translated format
	''' </summary>
	''' <param name="dDateTime">The date to transform.</param>
	''' <param name="sFormat">The desired format for the date.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="sCultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="CustomFormat">Custom date format, used when sFormat is set to 'custom'.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function DisplayDate(ByVal dDateTime As DateTime, ByVal sFormat As String,
	  ByVal iLanguageID As Integer, ByVal sCultureCode As String,
	  ByVal bDefaultLanguage As Boolean,
	  Optional ByVal CustomFormat As String = "", Optional ByVal bTranslateDefaultLanguage As Boolean = False) As String

		'need to use some odd characters which are not going to be in the translated date as placeholders for "st/th/nd" and '

		'some intuitive standards
		If sFormat.ToLower = "fulldate" Then
			sFormat = "d@ MMMM yyyy"
		ElseIf sFormat.ToLower = "longdate" Then 'EU friendly non ordinal
			sFormat = "d MMMM #yy"
		ElseIf sFormat.ToLower = "mediumdate" Then
			sFormat = "d MMM yyyy"
		ElseIf sFormat.ToLower = "shortdate" Then
			sFormat = "d MMM #yy"
		ElseIf sFormat.ToLower = "custom" Then
			sFormat = CustomFormat
		End If

		Dim oDateTimeCulture As New CultureInfo(sCultureCode)
		Dim oDateTimeFormatInfo As DateTimeFormatInfo = oDateTimeCulture.DateTimeFormat
		Dim sFormattedDate As String = dDateTime.ToString(sFormat, oDateTimeFormatInfo)

		'@ is a placeholder for st, nd, rd, th and so on based on last digit of days
		If sFormat.Contains("@") Then
			sFormattedDate = sFormattedDate.Replace("@", Multilingual.DayOrdinal(dDateTime.Day, iLanguageID, bDefaultLanguage, bTranslateDefaultLanguage))
		End If

		'put the ' back in, ' isn't liked in the formatting string
		sFormattedDate = sFormattedDate.Replace("#", "'")
		Return sFormattedDate

	End Function

	''' <summary>
	''' Gets the ordinal for a specified day integer and translates it.
	''' </summary>
	''' <param name="iDay">The i day.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function DayOrdinal(ByVal iDay As Integer, ByVal iLanguageID As Integer, ByVal bDefaultLanguage As Boolean,
	  ByVal bTranslateDefaultLanguage As Boolean) As String
		Return Multilingual.DayOrdinal(iDay, iLanguageID, bDefaultLanguage, String.Empty, bTranslateDefaultLanguage)
	End Function

	''' <summary>
	''' Gets the ordinal for a specified day integer and translates it.
	''' </summary>
	''' <param name="iDay">The i day.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Shared Function DayOrdinal(ByVal iDay As Integer, ByVal iLanguageID As Integer, ByVal bDefaultLanguage As Boolean) As String
		Return Multilingual.DayOrdinal(iDay, iLanguageID, bDefaultLanguage, String.Empty, False)
	End Function

	''' <summary>
	''' Gets the ordinal for a specified day integer and translates it.
	''' </summary>
	''' <param name="iDay">The day integer.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	Public Shared Function DayOrdinal(ByVal iDay As Integer, ByVal iLanguageID As Integer, ByVal bDefaultLanguage As Boolean, ByVal GroupPrefix As String,
	  ByVal bTranslateDefaultLanguage As Boolean) As String

		'not sure that hard coding english is right, but it comes ultimately from xsl full date function, so this is simple existing functionality moved
		Dim sEnglish As String = ""
		Select Case iDay
			Case 1, 21, 31
				sEnglish = "st"
			Case 2, 22
				sEnglish = "nd"
			Case 3, 23
				sEnglish = "rd"
			Case Else
				sEnglish = "th"
		End Select

		Return Multilingual.GetCustomTranslation("Day Ordinal", GroupPrefix, "Day " & iDay.ToString,
		   sEnglish, iLanguageID, bDefaultLanguage, bTranslateDefaultLanguage)

	End Function

	''' <summary>
	''' Gets the ordinal for a specified day integer and translates it.
	''' </summary>
	''' <param name="iDay">The day integer.</param>
	''' <param name="iLanguageID">the id of the langauge to be translated into.</param>
	''' <param name="bDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	Public Shared Function DayOrdinal(ByVal iDay As Integer, ByVal iLanguageID As Integer, ByVal bDefaultLanguage As Boolean, ByVal GroupPrefix As String) As String

		'not sure that hard coding english is right, but it comes ultimately from xsl full date function, so this is simple existing functionality moved
		Dim sEnglish As String = ""
		Select Case iDay
			Case 1, 21, 31
				sEnglish = "st"
			Case 2, 22
				sEnglish = "nd"
			Case 3, 23
				sEnglish = "rd"
			Case Else
				sEnglish = "th"
		End Select

		Return Multilingual.GetCustomTranslation("Day Ordinal", GroupPrefix, "Day " & iDay.ToString,
		   sEnglish, iLanguageID, bDefaultLanguage, False)

	End Function

#End Region

#Region "Translate XSLFO"

	'copied largely with no comprehension of how it works from translate page
	''' <summary>
	''' Translates the specified xml into the desired language using regex.
	''' </summary>
	''' <param name="XML">The XML to translate.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="bSuppressTranslations">if set to <c>true</c>, translation will be suppressed.</param>
	Public Shared Function TranslateXSLFOPart(ByVal XML As String, ByVal LanguageID As Integer,
	  ByVal CultureCode As String, ByVal IsDefaultLanguage As Boolean, Optional ByVal bTranslateDefaultLanguage As Boolean = False, Optional bSuppressTranslations As Boolean = False) As String

		'1 build delegate collections
		Dim oDelegate As TranslationDelegate = New TranslationDelegate(LanguageID, CultureCode, IsDefaultLanguage, bTranslateDefaultLanguage, bSuppressTranslations)
		Dim oTranslationRegexCollection As New TranslationRegexCollection()
		oTranslationRegexCollection.Add("<(?<tag>fo:\w+|trans)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>(?<text>.*?)</\k<tag>>",
		 New MatchEvaluator(AddressOf oDelegate.TagProcess))

		'4 translate
		Return oTranslationRegexCollection.TranslatePage(XML)

	End Function

#End Region

#Region "Translate Page"

	'overloaded test method
	''' <summary>
	''' Translates the page into the desired language.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translate.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="RightToLeftText">if set to <c>true</c>, text goes from right to left.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean,
		ByVal bTranslateDefaultLanguage As Boolean,
		ByVal RightToLeftText As Boolean) As Multilingual.TranslatedPage
		Return Multilingual.TranslatePage(sPageHTML, LanguageID, CultureCode, IsDefaultLanguage, "", bTranslateDefaultLanguage, RightToLeftText)
	End Function

	''' <summary>
	''' Translates the page into desired language.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translate.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean) As Multilingual.TranslatedPage
		Return Multilingual.TranslatePage(sPageHTML, LanguageID, CultureCode, IsDefaultLanguage, "", False, False)
	End Function

	''' <summary>
	''' Translates the page into desired language.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translate.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupCode">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="RightToLeftText">if set to <c>true</c>, text goes from right to left.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean,
		ByVal GroupCode As String,
		ByVal bTranslateDefaultLanguage As Boolean,
		ByVal RightToLeftText As Boolean) As Multilingual.TranslatedPage
		Return Multilingual.TranslatePage(sPageHTML, LanguageID, CultureCode, IsDefaultLanguage, GroupCode, DiagnosticMode.None, bTranslateDefaultLanguage, RightToLeftText)
	End Function

	''' <summary>
	''' Translates the page into desired language.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translate.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupCode">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean,
		ByVal GroupCode As String) As Multilingual.TranslatedPage
		Return Multilingual.TranslatePage(sPageHTML, LanguageID, CultureCode, IsDefaultLanguage, GroupCode, DiagnosticMode.None, False, False)
	End Function

	''' <summary>
	''' Translates the page into desired language, it won't translate if IsDefaultLanguage is true.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translateL.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean,
		ByVal GroupPrefix As String,
		ByVal DiagnosticMode As DiagnosticMode) As Multilingual.TranslatedPage

		Dim oTranslatedPage As New TranslatedPage

		'create an instance of the delegate class with the languageid and code passed in. The delegate functions can then access their instanciated properties when called
		oTranslatedPage.TranslationDelegate = New TranslationDelegate(LanguageID, CultureCode, IsDefaultLanguage, GroupPrefix, DiagnosticMode, False, False, False)

		Dim oTranslationRegexCollection As New TranslationRegexCollection()

		oTranslationRegexCollection.Add("<trans\s[^>]*?ml=""[^""]+?""[^>]*?\s*/>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.EmptyTransDelegate))
		oTranslationRegexCollection.Add("<select\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>.*?</select>",
			New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.SelectProcess))
		oTranslationRegexCollection.Add("<(?<tag>\w+)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>(?<text>(.(?!<\k<tag>))*?)</\k<tag>>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.TagProcess))
		oTranslationRegexCollection.Add("<(?<tag>\w+?)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?\s*/>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.SelfClosingTagProcess))

		With oTranslatedPage
			.HTML = oTranslationRegexCollection.TranslatePage(sPageHTML)
			.Duration = oTranslationRegexCollection.EndTime.Subtract(oTranslationRegexCollection.StartTime).TotalMilliseconds
		End With

		Return oTranslatedPage

	End Function

	''' <summary>
	''' Translates the page into desired language.
	''' </summary>
	''' <param name="sPageHTML">The s page HTML.</param>
	''' <param name="LanguageID">the id of the langauge to be translated into.</param>
	''' <param name="CultureCode">The culture code used to retrieve specific culture formatting information.</param>
	''' <param name="IsDefaultLanguage">is the language in iLanguageID the system language.</param>
	''' <param name="GroupPrefix">a prefix which is passed in and prepended to the group to define an alternative translation set.</param>
	''' <param name="DiagnosticMode">The diagnostic mode.</param>
	''' <param name="bTranslateDefaultLanguage">if set to <c>true</c>, still translate the default language.</param>
	''' <param name="RightToLeftText">if set to <c>true</c>, text goes from right to left.</param>
	Public Shared Function TranslatePage(
		ByVal sPageHTML As String,
		ByVal LanguageID As Integer,
		ByVal CultureCode As String,
		ByVal IsDefaultLanguage As Boolean,
		ByVal GroupPrefix As String,
		ByVal DiagnosticMode As DiagnosticMode,
		ByVal bTranslateDefaultLanguage As Boolean,
		ByVal RightToLeftText As Boolean) As Multilingual.TranslatedPage

		Dim oTranslatedPage As New TranslatedPage

		'create an instance of the delegate class with the languageid and code passed in. The delegate functions can then access their instanciated properties when called
		oTranslatedPage.TranslationDelegate = New TranslationDelegate(LanguageID, CultureCode, IsDefaultLanguage, GroupPrefix, DiagnosticMode, bTranslateDefaultLanguage, False, RightToLeftText)

		Dim oTranslationRegexCollection As New TranslationRegexCollection()

		oTranslationRegexCollection.Add("<trans\s[^>]*?ml=""[^""]+?""[^>]*?\s*/>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.EmptyTransDelegate))
		oTranslationRegexCollection.Add("<select\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>.*?</select>",
		 New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.SelectProcess))
		oTranslationRegexCollection.Add("<(?<tag>\w+)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>(?<text>(.(?!<\k<tag>))*?)</\k<tag>>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.TagProcess))
		oTranslationRegexCollection.Add("<(?<tag>\w+?)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?\s*/>",
		  New MatchEvaluator(AddressOf oTranslatedPage.TranslationDelegate.SelfClosingTagProcess))

		With oTranslatedPage
			.HTML = oTranslationRegexCollection.TranslatePage(sPageHTML)
			.Duration = oTranslationRegexCollection.EndTime.Subtract(oTranslationRegexCollection.StartTime).TotalMilliseconds
		End With

		Return oTranslatedPage

	End Function

#End Region

#Region "get multilingual lookups and support classes"

	''' <summary>
	''' Gets the multilingual lookups from the cache if possible, if not gets them from the database.
	''' </summary>
	''' <param name="iLanguageID">The id of the language to retrieve the translation lookups.</param>
	Private Shared Function GetMultilingualLookups(ByVal iLanguageID As Integer) As MultilingualLookup

		'get it from the cache if possible
		Dim sCacheKey As String = Multilingual.CacheKey(iLanguageID)

		Dim oLookups As MultilingualLookup = Functions.GetCache(Of MultilingualLookup)(sCacheKey)
		If oLookups Is Nothing Then

			oLookups = New MultilingualLookup

			Dim dt As DataTable = SQL.GetDataTable("exec MultilingualTranslations", iLanguageID)
			For Each dr As DataRow In dt.Rows
				oLookups.AddKey(dr("Group").ToString, dr("Key").ToString, dr("Subkey").ToString, dr("Translation").ToString)
			Next

			'pop it back in the cache
			Intuitive.Functions.AddToCache(sCacheKey, oLookups, 5)

		End If

		Return oLookups

	End Function

	'lookup collection
	''' <summary>
	''' Class containing a list of lookup keys and values
	''' </summary>
	Public Class MultilingualLookup
		Inherits Generic.Dictionary(Of Integer, String)

		''' <summary>
		''' Adds a new multilingual lookup key.
		''' </summary>
		''' <param name="Group">The group.</param>
		''' <param name="Key">The key.</param>
		''' <param name="SubKey">The sub key.</param>
		''' <param name="LookupValue">The lookup value.</param>
		Public Sub AddKey(ByVal Group As String, ByVal Key As String, ByVal SubKey As String, ByVal LookupValue As String)
			Dim iKey As Integer = New MultiLingualLookupKey(Group, Key, SubKey).CompoundKey
			If Not Me.ContainsKey(iKey) Then Me.Add(iKey, LookupValue)
		End Sub

	End Class

	'lookup key
	''' <summary>
	''' Class representing a lookup key
	''' </summary>
	Public Class MultiLingualLookupKey
		''' <summary>
		''' The group
		''' </summary>
		Public Group As String
		''' <summary>
		''' The key
		''' </summary>
		Public Key As String
		''' <summary>
		''' The sub key
		''' </summary>
		Public SubKey As String

		''' <summary>
		''' Initializes a new instance of the <see cref="MultiLingualLookupKey"/> class.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="MultiLingualLookupKey"/> class.
		''' </summary>
		''' <param name="Group">The group.</param>
		''' <param name="Key">The key.</param>
		''' <param name="Subkey">The subkey.</param>
		Public Sub New(ByVal Group As String, ByVal Key As String, ByVal Subkey As String)
			Me.Group = Group
			Me.Key = Key
			Me.SubKey = Subkey
		End Sub

		''' <summary>
		''' Generates a compound key from the group, key and subkey and returns the hash code of the result
		''' </summary>
		Public ReadOnly Property CompoundKey() As Integer
			Get
				Return (Me.Group.ToLower & "|" & Me.Key.ToLower & "|" & Me.SubKey.ToLower).GetHashCode
			End Get
		End Property

		''' <summary>
		''' Gets the default language value.
		''' </summary>
		Public ReadOnly Property DefaultLanguageValue() As String
			Get
				'just return the english
				If Me.SubKey <> "" Then
					Return Me.SubKey
				Else
					Return Me.Key
				End If
			End Get
		End Property

	End Class

#End Region

End Class
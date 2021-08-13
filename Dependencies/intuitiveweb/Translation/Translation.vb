Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports Intuitive
Imports Intuitive.Functions
Imports System.Xml


Public Class Translation

#Region "Properties"

	'the base language will always be English, this is referring to the language the website is written in
	Private Shared ReadOnly Property BaseLanguage As Boolean
		Get
			Return BookingBase.DisplayLanguageID = BookingBase.Params.BaseLanguageID
		End Get
	End Property

	'the default language is the language used in iVector and the one shown by default on the website
	Private Shared ReadOnly Property DefaultLanguage As Boolean
		Get
			Return BookingBase.DisplayLanguageID = BookingBase.Params.LanguageID
		End Get
	End Property

	Private Shared ReadOnly Property CacheKey(ByVal LanguageID As Integer, Optional ByVal UnavailableLookups As Boolean = False) As String
		Get
			If UnavailableLookups Then
				Return "MultilingualTranslations_" & LanguageID.ToString & "Unavailable"
			Else
				Return "MultilingualTranslations_" & LanguageID.ToString
			End If
		End Get
	End Property

#End Region


#Region "Public Functions"

	'translate HTML
	Public Shared Function TranslateHTML(ByVal HTML As String) As String

        Return TranslateHTML(HTML, BookingBase.DisplayLanguageID,
                             BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CultureCode, BookingBase.DisplayLanguageID),
                             BaseLanguage)

	End Function

    Public Shared Function TranslateHTML(ByVal HTML As String, PreTranslatingXslTemplate As Boolean) As String

        Return TranslateHTML(HTML, BookingBase.DisplayLanguageID,
                             BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CultureCode, BookingBase.DisplayLanguageID),
                             BaseLanguage,
                             PreTranslatingXslTemplate)

    End Function

	'get single translation - subkey is used for translating dropdowns
	'this is main bizzo
	Public Shared Function GetCustomTranslation(ByVal Group As String, ByVal Key As String, Optional ByVal SubKey As String = "", Optional ByVal JavascriptSafe As Boolean = False) As String

		If JavascriptSafe Then
			Return GetCustomTranslation(New MultiLingualLookupKey(Group, Key, SubKey), BookingBase.DisplayLanguageID, BaseLanguage).Replace("'", "\'")
		Else
			Return GetCustomTranslation(New MultiLingualLookupKey(Group, Key, SubKey), BookingBase.DisplayLanguageID, BaseLanguage)
		End If

	End Function


	'get single table translation
	Public Shared Function GetTableTranslation(ByVal Table As String, ByVal Field As String, ByVal DefaultLanguageText As String) As String

		Return Translate(Table & "." & Field, DefaultLanguageText, BookingBase.DisplayLanguageID, DefaultLanguage)

	End Function


	'get single table translation by ID
	Public Shared Function GetTableTranslationByID(ByVal Table As String, ByVal Field As String, ByVal SourceID As Integer, Optional ByVal NullIfValue As String = "") As String

		Return TranslateByID(Table & "." & Field, SourceID, NullIfValue, BookingBase.DisplayLanguageID)

	End Function


	'display date - accepted formats are fulldate, longdate, mediumdate, shortdate and custom
	Public Shared Function TranslateDate(ByVal DateTime As DateTime, ByVal Format As String, Optional ByVal CustomFormat As String = "") As String

		Return DisplayDate(DateTime, Format, BookingBase.DisplayLanguageID, BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencyCode, BookingBase.DisplayLanguageID), BaseLanguage, CustomFormat)

	End Function


	'get translated ordinal
	Public Shared Function GetOrdinal(ByVal i As Integer) As String

		Dim sEnglishSuffix As String = ""
		Select Case i Mod 10
			Case 1
				sEnglishSuffix = "st"
			Case 2
				sEnglishSuffix = "nd"
			Case 3
				sEnglishSuffix = "rd"
			Case Else
				sEnglishSuffix = "th"
		End Select
		Select Case i Mod 100
			Case 11, 12, 13
				sEnglishSuffix = "th"
		End Select

		Return GetCustomTranslation("Ordinal", i.ToString & sEnglishSuffix)

	End Function


#End Region


#Region "Support Functions and Multilingual Lookups"


	'main get multilingual translation routine 
	Private Shared Function GetCustomTranslation(ByVal LookupKey As MultiLingualLookupKey, _
												 ByVal iLanguageID As Integer, _
												 ByVal IsDefaultLanguage As Boolean) As String

		If LookupKey.Key.Trim = String.Empty Then Return LookupKey.DefaultLanguageValue

		If Not IsDefaultLanguage Then

			Dim oLookups As MultilingualLookup = GetMultilingualLookups(iLanguageID)
			Dim oUnavailableLookups As MultilingualLookup = GetMultilingualLookups(iLanguageID, True)

			' if multilingual exists
			If oLookups.ContainsKey(LookupKey.CompoundKey) Then

				'get the translated version.
				Dim sTranslation As String = oLookups(LookupKey.CompoundKey)
				If sTranslation = String.Empty Then
					Return LookupKey.DefaultLanguageValue
				Else
					Return sTranslation
				End If

				'check we have not requested the same thing previously and it is unavailable
			ElseIf Not oUnavailableLookups.ContainsKey(LookupKey.CompoundKey) Then

				'request it from iVector connect if it doesn't exist on the webcontent server
				Dim oTranslationRequest As New iVectorConnectInterface.TranslationRequest
				oTranslationRequest.LoginDetails = BookingBase.IVCLoginDetails
				oTranslationRequest.Group = LookupKey.Group
				oTranslationRequest.Key = LookupKey.Key
				oTranslationRequest.SubKey = LookupKey.SubKey
				oTranslationRequest.LanguageID = iLanguageID

                If BookingBase.Params.OverrideTranslationAgentReference <> "" Then
                    oTranslationRequest.LoginDetails.AgentReference = BookingBase.Params.OverrideTranslationAgentReference
                End If

				'build request
				Dim oSearchRequestXML As XmlDocument = Serializer.Serialize(oTranslationRequest)

				'Send the request
				Dim sSearchResponse As String = Intuitive.Net.WebRequests.GetResponse(BookingBase.Params.ServiceURL & "ivectorconnect.ashx", oSearchRequestXML.InnerXml, False, "")
				Dim oResponseXML As New XmlDocument
				oResponseXML.LoadXml(sSearchResponse)

				Dim oTranslationResponse As New iVectorConnectInterface.TranslationResponse

				If SafeBoolean(XMLFunctions.SafeNodeValue(oResponseXML, "//ReturnStatus/Success")) Then
					oTranslationResponse = Serializer.DeSerialize(Of iVectorConnectInterface.TranslationResponse)(sSearchResponse)
				End If

				If oTranslationResponse.TranslationAvailable Then

	
					'add it to lookups
					If Not oLookups.ContainsKey(LookupKey.CompoundKey) Then
						oLookups.Add(LookupKey.CompoundKey, oTranslationResponse.TranslationText)

						'recache
						Dim sCacheKey As String = CacheKey(iLanguageID)
						Intuitive.Functions.AddToCache(sCacheKey, oLookups, 5)

					End If


					'return translated text
					Return oTranslationResponse.TranslationText

				Else

					'find some way of not requesting the same thing a million times
					If Not oUnavailableLookups.ContainsKey(LookupKey.CompoundKey) Then
						oUnavailableLookups.Add(LookupKey.CompoundKey, "")
						Dim sCacheKey As String = CacheKey(iLanguageID, True)
						Intuitive.Functions.AddToCache(sCacheKey, oUnavailableLookups, 60)
					End If

					Return oTranslationResponse.TranslationText

					End If

			End If

		End If

		Return LookupKey.DefaultLanguageValue

	End Function


	'get multilingual lookups
	Private Shared Function GetMultilingualLookups(ByVal iLanguageID As Integer, Optional ByVal bUnavailableLookups As Boolean = False) As MultilingualLookup

		'get it from the cache if possible
		Dim sCacheKey As String = CacheKey(iLanguageID, bUnavailableLookups)

		Dim oLookups As MultilingualLookup = Functions.GetCache(Of MultilingualLookup)(sCacheKey)
		If oLookups Is Nothing Then

			oLookups = New MultilingualLookup

			If Not bUnavailableLookups Then


				Dim dt As DataTable = SQL.GetDataTable("exec IVC_TranslationText_CreateLookup", iLanguageID)
				For Each dr As DataRow In dt.Rows
					oLookups.AddKey(dr("Group").ToString, dr("Key").ToString, dr("Subkey").ToString, dr("Translation").ToString)
				Next


			End If

			'pop it back in the cache
			Intuitive.Functions.AddToCache(sCacheKey, oLookups, 5)

		End If

		Return oLookups

	End Function

    Public Shared Function TranslateAndFormatDate(ByVal DateTime As DateTime, Format As String) As String
        Return DisplayDate(DateTime, Format,
                           BookingBase.Params.LanguageID,
                           BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CultureCode, BookingBase.DisplayLanguageID),
                           Translation.BaseLanguage)
    End Function

    'display date
    Private Shared Function DisplayDate(ByVal DateTime As DateTime, ByVal Format As String,
      ByVal LanguageID As Integer, ByVal CultureCode As String, _
      ByVal DefaultLanguage As Boolean, Optional ByVal CustomFormat As String = "") As String

        'need to use some odd characters which are not going to be in the translated date as placeholders for "st/th/nd" and '

        'some intuitive standards
        If Format.ToLower = "fulldate" Then
            Format = "d@ MMMM yyyy"
        ElseIf Format.ToLower = "longdate" Then 'EU friendly non ordinal 
            Format = "d MMMM yyyy"
        ElseIf Format.ToLower = "mediumdate" Then
            Format = "d MMM yyyy"
        ElseIf Format.ToLower = "shortdate" Then
            Format = "d MMM #yy"
        ElseIf Format.ToLower = "custom" Then
            Format = CustomFormat
        End If

        Dim oDateTimeCulture As New CultureInfo(CultureCode)
        Dim oDateTimeFormatInfo As DateTimeFormatInfo = oDateTimeCulture.DateTimeFormat
        Dim sFormattedDate As String = DateTime.ToString(Format, oDateTimeFormatInfo)

        '@ is a placeholder for st, nd, rd, th and so on based on last digit of days
        If Format.Contains("@") Then
            sFormattedDate = sFormattedDate.Replace("@", GetOrdinal(DateTime.Day))
        End If

        'put the ' back in, ' isn't liked in the formatting string
        Return sFormattedDate.Replace("#", "'")

    End Function


    'table translation
    Private Shared Function Translate(ByVal SourceFieldName As String, ByVal DefaultLanguageText As String, ByVal LanguageID As Integer, ByVal DefaultLanguage As Boolean) As String

        If DefaultLanguage Then Return DefaultLanguageText

        Dim oTranslations As Hashtable = Functions.GetCache(Of Hashtable)("translations")
        Dim iKey As Integer

        If oTranslations Is Nothing Then

            oTranslations = New Hashtable
            Dim dtTranslations As DataTable = SQL.GetDataTable("exec IVC_TranslationTable_CreateLookup")
            For Each dr As DataRow In dtTranslations.Rows

                'key based on table, field, text and languageid
                iKey = (dr("Item").ToString & "##" & dr("Translation").ToString & "##" & dr("LanguageID").ToString).Replace(Chr(10), "").Replace(Chr(13), "").GetHashCode
                If Not oTranslations.Contains(iKey) Then
                    oTranslations.Add(iKey, dr("Translation").ToString)
                End If

            Next

            Intuitive.Functions.AddToCache("translations", oTranslations, 60 * 12)
        End If


        iKey = (SourceFieldName & "##" & DefaultLanguageText & "##" & LanguageID.ToString).Replace(Chr(10), "").Replace(Chr(13), "").GetHashCode
        If oTranslations.Contains(iKey) Then
            Return oTranslations(iKey).ToString
        Else
            Return DefaultLanguageText
        End If

    End Function


    'table translation by ID
    Public Shared Function TranslateByID(ByVal SourceFieldName As String, ByVal SourceID As Integer, _
        ByVal NullIfValue As String, ByVal LanguageID As Integer) As String

        Dim oTranslations As Hashtable = Functions.GetCache(Of Hashtable)("translationsbyid")
        Dim iKey As Integer

        If oTranslations Is Nothing Then

            oTranslations = New Hashtable
            Dim dtTranslations As DataTable = SQL.GetDataTable("exec IVC_TranslationTable_CreateLookup")
            For Each dr As DataRow In dtTranslations.Rows

                'key based on table, field, sourceid and languageid
                iKey = (dr("Item").ToString & "##" & dr("SourceID").ToString & "##" & dr("LanguageID").ToString).Replace(Chr(10), "").Replace(Chr(13), "").GetHashCode
                If Not oTranslations.Contains(iKey) Then
                    oTranslations.Add(iKey, dr("Translation").ToString)
                End If

            Next

            Intuitive.Functions.AddToCache("translationsbyid", oTranslations, 60 * 12)
        End If


        iKey = (SourceFieldName & "##" & SourceID & "##" & LanguageID.ToString).Replace(Chr(10), "").Replace(Chr(13), "").GetHashCode
        If oTranslations.Contains(iKey) Then
            Return oTranslations(iKey).ToString
        Else
            Return NullIfValue
        End If


    End Function


    'translate HTML
    'overloaded test method
    Private Shared Function TranslateHTML(ByVal sPageHTML As String,
                                          ByVal LanguageID As Integer,
                                          ByVal CultureCode As String,
                                          ByVal IsDefaultLanguage As Boolean) As String

        Return TranslateHTML(sPageHTML, LanguageID, CultureCode, IsDefaultLanguage, False)

    End Function

    Private Shared Function TranslateHTML(ByVal sPageHTML As String,
                                         ByVal LanguageID As Integer,
                                         ByVal CultureCode As String,
                                         ByVal IsDefaultLanguage As Boolean,
                                         PreTranslatingXslTemplate As Boolean) As String

        'create an instance of the delegate class with the languageid and code passed in. The delegate functions can then access their instanciated properties when called
        Dim oDelegate As TranslationDelegate = New TranslationDelegate(LanguageID, CultureCode, IsDefaultLanguage, PreTranslatingXslTemplate)

        Dim oTranslationRegexCollection As New TranslationRegexCollection()

        oTranslationRegexCollection.Add("<select\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>.*?</select>", _
          New MatchEvaluator(AddressOf oDelegate.SelectProcess))
        oTranslationRegexCollection.Add("<(?<tag>\w+)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?>(?<text>.*?)</\k<tag>>", _
          New MatchEvaluator(AddressOf oDelegate.TagProcess))
        oTranslationRegexCollection.Add("<(?<tag>\w+?)\s[^>]*?ml=""(?<mltag>[^""]+?)""[^>]*?\s*/>", _
          New MatchEvaluator(AddressOf oDelegate.SelfClosingTagProcess))

        Dim oTranslatedPage As New TranslatedPage
        With oTranslatedPage
            .HTML = oTranslationRegexCollection.TranslatePage(sPageHTML)
            .Duration = oTranslationRegexCollection.EndTime.Subtract(oTranslationRegexCollection.StartTime).TotalMilliseconds
        End With

        Return oTranslatedPage.HTML

    End Function

#End Region

#Region "Private Classes"

    'lookup collection
    Public Class MultilingualLookup
        Inherits Generic.Dictionary(Of Integer, String)

        Public Sub AddKey(ByVal Group As String, ByVal Key As String, ByVal SubKey As String, ByVal LookupValue As String)
            Dim iKey As Integer = New MultiLingualLookupKey(Group, Key, SubKey).CompoundKey
            If Not Me.ContainsKey(iKey) Then Me.Add(iKey, LookupValue)
        End Sub

    End Class

    'lookup key
    Public Class MultiLingualLookupKey
        Public Group As String
        Public Key As String
        Public SubKey As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal Group As String, ByVal Key As String, ByVal Subkey As String)
            Me.Group = Group
            Me.Key = Key
            Me.SubKey = Subkey
        End Sub

        Public ReadOnly Property CompoundKey() As Integer
            Get
                Return (Me.Group.ToLower & "|" & Me.Key.ToLower & "|" & Me.SubKey.ToLower).GetHashCode
            End Get
        End Property

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

    Private Class TranslatedPage
        Public HTML As String
        Public Duration As Double
    End Class

    Private Class TranslationDelegate
        Private IsDefaultLanguage As Boolean
        Private LanguageID As Integer
        Private CultureCode As String
        Private PreTranslatingXslTemplate As Boolean

        Public Sub New(ByVal iLanguageID As Integer,
                       ByVal sCultureCode As String,
                       ByVal bIsDefaultLanguage As Boolean)
            Me.LanguageID = iLanguageID
            Me.CultureCode = sCultureCode
            Me.IsDefaultLanguage = bIsDefaultLanguage
        End Sub

        Public Sub New(ByVal iLanguageID As Integer,
                       ByVal sCultureCode As String,
                       ByVal bIsDefaultLanguage As Boolean,
                       PreTranslatingXslTemplate As Boolean)
            Me.LanguageID = iLanguageID
            Me.CultureCode = sCultureCode
            Me.IsDefaultLanguage = bIsDefaultLanguage
            Me.PreTranslatingXslTemplate = PreTranslatingXslTemplate
        End Sub

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
        Public Function TagProcess(ByVal m As Match) As String

            Dim sTagHTML As String = m.Value
            Dim sTagContents As String = m.Groups("text").Value
            Dim sTagName As String = m.Groups("tag").Value

            Dim dStart As DateTime = DateTime.Now

            If sTagName.ToLower <> "select" Then

                Dim sTranslation As String = sTagContents

                Dim oMultilingualAttributes As MultilingualAttributes = TranslationDelegate.GetAttributes(sTagName, sTagHTML)

                If oMultilingualAttributes.TranslateContent Then

                    Dim oGroupCode As New GroupCodeSplit(m.Groups("mltag").Value)
                    If oGroupCode.Key = "" Then
                        oGroupCode.Key = sTagContents.Trim.Replace(Environment.NewLine, "")
                        oGroupCode.Key = Regex.Replace(oGroupCode.Key, "\s{2,}", " ", RegexOptions.Compiled)
                    End If


                    'set the translation as the tagcontents by default
                    If Not Me.IsDefaultLanguage Then
                        sTranslation = GetCustomTranslation(oGroupCode.Group, oGroupCode.Key, oGroupCode.SubKey)
                        'sTranslation = oGroupCode.Group
                    End If

                    'process the parameters
                    Try
                        sTranslation = Me.ProcessParameters(sTagHTML, sTranslation, Me.PreTranslatingXslTemplate)
                    Catch ex As Exception

                    End Try


                End If

                'process any attributes (titles on anchors)
                sTagHTML = Me.ProcessAttributes(m, oMultilingualAttributes)

                'replace the text in the middle of the tags and take out the ml
                sTagHTML = Regex.Replace(sTagHTML, "(?<=>).+(?=<)", IIf(sTranslation Is Nothing, sTagContents, sTranslation), RegexOptions.Singleline Or RegexOptions.Compiled)
                sTagHTML = TranslationDelegate.RemoveMultilingualAttributes(sTagHTML)

            End If

            Dim dEnd As DateTime = DateTime.Now

            Dim nDuration As Double = dEnd.Subtract(dStart).TotalMilliseconds

            Return sTagHTML

        End Function


        'self closing tag process <tag ml="group/key" />, translates alt, value and title tags
        Public Function SelfClosingTagProcess(ByVal m As Match) As String

            Dim sTagHTML As String = m.Value
            Dim sTagName As String = m.Groups("tag").Value
            Dim oMultilingualAttributes As MultilingualAttributes = TranslationDelegate.GetAttributes(sTagName, sTagHTML)
            Return TranslationDelegate.RemoveMultilingualAttributes(Me.ProcessAttributes(m, oMultilingualAttributes))

        End Function


        'select process - sorts out the options
        Public Function SelectProcess(ByVal m As Match) As String

            Dim sTagHTML As String = m.Value

            Dim oGroupCode As New GroupCodeSplit(m.Groups("mltag").Value)

            'find all the option matches in the string
            Dim oMatches As MatchCollection = Regex.Matches(sTagHTML, "<option[^>]*?>(?<text>.*?)</option>", RegexOptions.Singleline)

            'loop through them all
            For Each oMatch As Match In oMatches

                Dim sTagText As String = oMatch.Groups("text").Value

                'set the translation as the tagcontents by default
                Dim sTranslation As String = sTagText
                If Not Me.IsDefaultLanguage Then sTranslation = GetCustomTranslation(oGroupCode.Group, oGroupCode.Key, sTagText)

                'translate the option text - note the option value is not being dicked with
                'Dim sTranslation As String = Multilingual.GetTranslation(oGroupCode.Group, oGroupCode.Key, sTagText)

                'regex replace the old option HTML to new HTML, then string replace back into the select HTML
                Dim sOptionTag As String = oMatch.Value
                Dim sNewOptionTag As String = Regex.Replace(sOptionTag, "(?<=>)[^<]+(?=<)", sTranslation, RegexOptions.Compiled)

                sTagHTML = sTagHTML.Replace(sOptionTag, sNewOptionTag)

            Next

            Return TranslationDelegate.RemoveMultilingualAttributes(sTagHTML)

        End Function


#End Region


#Region "Processing Attributes"

        Private Function ProcessAttributes(ByVal m As Match, ByVal MultilingualAttributes As MultilingualAttributes) As String

            Dim sTagHTML As String = m.Value

            'get the tag name
            Dim sTagName As String = Regex.Match(sTagHTML, "^\<(?<tagname>[a-zA-Z]+)\s", RegexOptions.Compiled).Groups("tagname").Value.ToString.ToLower

            Dim bIncludeValueAttribute As Boolean = True
            If sTagName = "input" Then
                'only do the value attribute on buttons or submit inputs
                Dim sType As String = Regex.Match(sTagHTML, "type=\""(?<type>[a-zA-Z]+)\""", RegexOptions.Compiled).Groups("type").ToString.ToLower
                bIncludeValueAttribute = (sType = "button" OrElse sType = "submit" OrElse sType = "hidden" OrElse sType = "text")
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
                    If Not Me.IsDefaultLanguage Then sTranslation = GetCustomTranslation(oGroupCode.Group, oGroupCode.Key, oGroupCode.SubKey)

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

        Private Function ProcessParameters(ByVal sTagHTML As String, ByVal sTranslation As String) As String

            Return ProcessParameters(sTagHTML, sTranslation, False)

        End Function

        Private Function ProcessParameters(ByVal sTagHTML As String, ByVal sTranslation As String, PreTranslatingXslTemplate As Boolean) As String

            'if there is an mlparams tag then split the params out and string format
            Dim oParamRegex As MatchCollection = Regex.Matches(sTagHTML, "mlparams=""(?<params>[^""]*?)""")
            If oParamRegex.Count > 0 Then

                'string format those badboys
                Dim aParams As String() = oParamRegex(0).Groups("params").Value.Split("|"c)
                For iParams As Integer = 0 To aParams.Length - 1
                    Dim sParam As String = aParams(iParams)

                    'don't directly swap the ml param - sub in the xsl value of statement
                    'this will not work with dates, they will need to be pre-formatted before the Xml is passed to the Xsl template now
                    If PreTranslatingXslTemplate Then
                        sParam = String.Format("<xsl:value-of select=""{0}""/>", sParam.Substring(1, sParam.Length - 2))
                        aParams(iParams) = sParam
                    End If

                    Dim aFormatParam As String() = sParam.Split("~"c)

                    'format the parameters according to the formats provided
                    If aFormatParam.Length = 2 Then
                        If DateFunctions.IsDate(aFormatParam(0)) Then
                            aParams(iParams) = DisplayDate(DateFunctions.SafeDate(aFormatParam(0)), aFormatParam(1), Me.LanguageID, Me.CultureCode, Me.IsDefaultLanguage)
                        Else
                            aParams(iParams) = String.Format(aFormatParam(0), aFormatParam(1))
                        End If
                    ElseIf aFormatParam.Length = 3 AndAlso DateFunctions.IsDate(aFormatParam(0)) Then
                        aParams(iParams) = DisplayDate(DateFunctions.SafeDate(aFormatParam(0)), aFormatParam(1), Me.LanguageID, Me.CultureCode, Me.IsDefaultLanguage, aFormatParam(2))
                    End If

                Next

                sTranslation = String.Format(sTranslation, aParams)

            End If

            Return sTranslation

        End Function

#End Region

#Region "Remove ml attributes"

        Private Shared Function RemoveMultilingualAttributes(ByVal sTagHTML As String) As String
            Return Regex.Replace(sTagHTML, "\s(ml|mlparams|mlattributes)=\""[^""]+\""", "", RegexOptions.Singleline And RegexOptions.Compiled)
        End Function

#End Region

#Region "Group Code Split"

        'group code split
        Public Class GroupCodeSplit
            Public Group As String = ""
            Public Key As String = ""
            Public SubKey As String = ""

            Public Sub New(ByVal sGroupCode As String)
                Dim aGroupMatch() As String = sGroupCode.Split(";"c)
                Me.Group = aGroupMatch(0)
                If aGroupMatch.Length > 1 Then Me.Key = aGroupMatch(1)
                If aGroupMatch.Length > 2 Then Me.SubKey = aGroupMatch(2)
            End Sub
        End Class

#End Region

#Region "Translation Attributes"

        Public Class MultilingualAttributes
            Inherits Generic.List(Of String)
            Public TranslateContent As Boolean = True
        End Class

#End Region



    End Class

    Private Class TranslationRegexCollection
        Inherits Generic.List(Of TranslationRegex)

        Public StartTime As DateTime
        Public EndTime As DateTime

        Public ReadOnly Property Milliseconds() As Decimal
            Get
                Return Intuitive.Functions.SafeDecimal(EndTime.Subtract(StartTime).TotalMilliseconds)
            End Get
        End Property

        Public Overloads Sub Add(ByVal sPattern As String, ByVal oEvaluator As MatchEvaluator)

            Dim oTranslationRegex As New TranslationRegex
            With oTranslationRegex
                .Pattern = sPattern
                .Evaluator = oEvaluator
            End With

            Me.Add(oTranslationRegex)

        End Sub

        Public Function TranslatePage(ByVal sPageHTML As String) As String

            Me.StartTime = DateTime.Now

            For Each oTranslationRegex As TranslationRegex In Me
                Dim dStart As DateTime = DateTime.Now
                sPageHTML = Regex.Replace(sPageHTML, oTranslationRegex.Pattern, oTranslationRegex.Evaluator, RegexOptions.Compiled Or RegexOptions.Singleline)
                oTranslationRegex.TimeTaken = Intuitive.Functions.SafeDecimal(Now.Subtract(dStart).TotalMilliseconds)
            Next

            sPageHTML = Regex.Replace(sPageHTML, "<(/?)trans[^>]*>", "", RegexOptions.Compiled)
            sPageHTML = Regex.Replace(sPageHTML, "<(/?)fo:trans[^>]*>", "", RegexOptions.Compiled)
            sPageHTML = Regex.Replace(sPageHTML, "\sml=\""[^""]+\""", "", RegexOptions.Compiled)

            Me.EndTime = DateTime.Now



            Return sPageHTML

        End Function



    End Class

    Private Class TranslationRegex

        Public Pattern As String
        Public TimeTaken As Decimal
        Public Evaluator As MatchEvaluator

    End Class


#End Region


End Class



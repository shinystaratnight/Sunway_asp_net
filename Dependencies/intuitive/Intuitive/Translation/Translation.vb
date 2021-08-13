Imports System.Xml
Imports System.IO
Imports System.Web
Imports System.Text.RegularExpressions
Imports System.Configuration.ConfigurationManager
Imports Intuitive.Functions

Public Class Translation

	'we seem to be repeating stuff here
	'we have a new translate by call which seems to be replacing using an XML file which does pretty much the same thing with different but similar classes and methods
	'I will shortly come in and see if we can rationalise these methods into more succinct ones. Not my A1 priority right now.

#Region "Core Translation  Stuff"

	'translate
	''' <summary>
	''' Translates the specified text into the desired language.
	''' </summary>
	''' <param name="SourceFieldName">Name of the source field to get the translation from.</param>
	''' <param name="DefaultLanguageText">The default language text.</param>
	''' <param name="LanguageID">The id of the language to translate to.</param>
	''' <param name="IsDefaultLanguage">if set to <c>true</c>, doesn't translate.</param>
	Public Shared Function Translate(ByVal SourceFieldName As String, ByVal DefaultLanguageText As String,
	 ByVal LanguageID As Integer, ByVal IsDefaultLanguage As Boolean) As String

		If IsDefaultLanguage Then Return DefaultLanguageText

		Dim oTranslations As Hashtable = Functions.GetCache(Of Hashtable)("translations")
		Dim iKey As Integer

		If oTranslations Is Nothing Then

			'put this in here so you can skip over once and then merrily translate everything without having to step over all the time
			If (Functions.IsDebugging AndAlso Functions.SafeBoolean(AppSettings("SkipTranslations"))) Then Return "{" & DefaultLanguageText & "}"

			oTranslations = New Hashtable
			Dim dtTranslations As DataTable = SQL.GetDataTable("exec Translation_CreateLookup")
			For Each dr As DataRow In dtTranslations.Rows

				iKey = (dr("Item").ToString & "##" & dr("DefaultLanguage").ToString & "##" &
				  dr("LanguageID").ToString).Replace(Char.ConvertFromUtf32(10), "").Replace(Char.ConvertFromUtf32(13), "").GetHashCode
				If Not oTranslations.Contains(iKey) Then
					oTranslations.Add(iKey, dr("Translation").ToString)
				End If

			Next

			Intuitive.Functions.AddToCache("translations", oTranslations, 60 * 12)
		End If

		iKey = (SourceFieldName & "##" & DefaultLanguageText & "##" & LanguageID.ToString).Replace(Char.ConvertFromUtf32(10), "").Replace(Char.ConvertFromUtf32(13), "").GetHashCode
		If oTranslations.Contains(iKey) Then
			Return oTranslations(iKey).ToString
		Else
			Return DefaultLanguageText
		End If

	End Function

	'translate by id
	''' <summary>
	''' Gets a translation based on the specified ID
	''' </summary>
	''' <param name="SourceFieldName">Name of the source field to get the translation from.</param>
	''' <param name="SourceID">The ID of the translation to retrieve.</param>
	''' <param name="NullIfValue">The value to return if there are no translations.</param>
	''' <param name="LanguageID">The id of the language to translate to.</param>
	Public Shared Function TranslateByID(ByVal SourceFieldName As String, ByVal SourceID As Integer,
										 ByVal NullIfValue As String, ByVal LanguageID As Integer) As String

		Dim oTranslations As Hashtable = Functions.GetCache(Of Hashtable)("translationsbyid")
		Dim iKey As Integer

		If oTranslations Is Nothing Then

			If (Functions.IsDebugging AndAlso Functions.SafeBoolean(AppSettings("SkipTranslations"))) Then Return "{" & NullIfValue & "}"

			oTranslations = New Hashtable
			Dim dtTranslations As DataTable = SQL.GetDataTable("exec Translation_CreateLookup")
			For Each dr As DataRow In dtTranslations.Rows

				iKey = (dr("Item").ToString & "##" & dr("SourceID").ToString & "##" &
				  dr("LanguageID").ToString).Replace(Char.ConvertFromUtf32(10), "").Replace(Char.ConvertFromUtf32(13), "").GetHashCode
				If Not oTranslations.Contains(iKey) Then
					oTranslations.Add(iKey, dr("Translation").ToString)
				End If

			Next

			Intuitive.Functions.AddToCache("translationsbyid", oTranslations, 60 * 12)
		End If

		iKey = (SourceFieldName & "##" & SourceID & "##" & LanguageID.ToString).Replace(Char.ConvertFromUtf32(10), "").Replace(Char.ConvertFromUtf32(13), "").GetHashCode
		If oTranslations.Contains(iKey) Then
			Return oTranslations(iKey).ToString
		Else
			Return NullIfValue
		End If

	End Function

#End Region

#Region "XML Document Translation"

	'xml translate
	''' <summary>
	''' Translates the XML document using translation file.
	''' </summary>
	''' <param name="oXML">The XML to translate.</param>
	''' <param name="sTranslationLookupFile">The translation lookup file.</param>
	''' <param name="sCall">The group of translations to retrieve from the translation lookup file.</param>
	''' <param name="iLanguageID">The id of the language to translate to.</param>
	''' <param name="IsDefaultLanguage">if set to <c>true</c>, will not translate.</param>
	Public Shared Function TranslateXMLDocument(ByVal oXML As XmlDocument, ByVal sTranslationLookupFile As String,
	  ByVal sCall As String, ByVal iLanguageID As Integer, ByVal IsDefaultLanguage As Boolean) As XmlDocument

		If File.Exists(sTranslationLookupFile) AndAlso Not IsDefaultLanguage Then
			Dim oTranslationLookups As Generic.List(Of Translation.TranslationLookup) =
			 Translation.GetTranslationLookupsByCall(sTranslationLookupFile, sCall)
			For Each oTranslationLookup As Translation.TranslationLookup In oTranslationLookups
				Translation.DocumentReplace(oXML, oTranslationLookup.XPath, oTranslationLookup.SourceFieldName,
				   oTranslationLookup.Regex, iLanguageID, IsDefaultLanguage)
			Next
		End If

		Return oXML

	End Function

	''' <summary>
	''' Translates the XML document using specified translation xml.
	''' </summary>
	''' <param name="oXML">The o XML.</param>
	''' <param name="oTranslationXML">The translation XML to get the translations from.</param>
	''' <param name="sCall">The group of translations to retrieve from the translation XML.</param>
	''' <param name="iLanguageID">The id of the language to translate to.</param>
	''' <param name="bIsDefaultLanguage">is the language in iLanguageID the system language, will not translate if true.</param>
	Public Shared Function TranslateXMLDocument(ByVal oXML As XmlDocument, ByVal oTranslationXML As XmlDocument,
	  ByVal sCall As String, ByVal iLanguageID As Integer, ByVal bIsDefaultLanguage As Boolean) As XmlDocument

		If Not bIsDefaultLanguage Then
			Dim oTranslationLookups As Generic.List(Of Translation.TranslationLookup) =
			 Translation.GetTranslationLookupsByCall(oTranslationXML, sCall)
			For Each oTranslationLookup As Translation.TranslationLookup In oTranslationLookups
				Translation.DocumentReplace(oXML, oTranslationLookup.XPath, oTranslationLookup.SourceFieldName,
				   oTranslationLookup.Regex, iLanguageID, bIsDefaultLanguage)
			Next
		End If

		Return oXML

	End Function

	'document replace
	''' <summary>
	''' Documents the replace.
	''' </summary>
	''' <param name="XML">The XML to translate.</param>
	''' <param name="XPath">The xpath of the nodes to replace.</param>
	''' <param name="SourceFieldName">Name of the field to retrieve the translation from.</param>
	''' <param name="RegexPattern">The regex pattern used to find matches to translate.</param>
	''' <param name="LanguageID">The id of the language to translate to.</param>
	''' <param name="IsDefaultLanguage">if set to <c>true</c>, will not translate.</param>
	Public Shared Function DocumentReplace(ByVal XML As XmlDocument, ByVal XPath As String, ByVal SourceFieldName As String,
	 ByVal RegexPattern As String, ByVal LanguageID As Integer, ByVal IsDefaultLanguage As Boolean) As XmlDocument

		If Not XML Is Nothing Then
			Dim oNodes As XmlNodeList = XML.SelectNodes(XPath)

			For Each oNode As XmlNode In oNodes
				Dim sNodeValue As String = oNode.InnerText
				If RegexPattern <> "" Then
					Dim oMatches As Match = Regex.Match(sNodeValue, RegexPattern)
					If oMatches.Success Then
						Dim sMatch As String = oMatches.Groups("text").Value.ToString.Trim()
						Dim sReplace As String = Translation.Translate(SourceFieldName, sMatch, LanguageID, IsDefaultLanguage)
						If sMatch <> "" AndAlso sReplace <> "" Then oNode.InnerText = sNodeValue.Replace(sMatch, sReplace)
					Else
						'leave as is.
					End If
				Else
					oNode.InnerText = Translation.Translate(SourceFieldName, sNodeValue, LanguageID, IsDefaultLanguage)
				End If
			Next
		End If

		Return XML
	End Function

#End Region

#Region "Translation By Call"

	''' <summary>
	''' Translates a single item
	''' </summary>
	''' <param name="sTranslationLookupFile">The translation lookup file.</param>
	''' <param name="Call">The group of translations to retrieve from the lookup file.</param>
	''' <param name="Type">The type of the translation to retrieve from the lookup file.</param>
	''' <param name="ID">The id of the translation to use.</param>
	''' <param name="LanguageID">The id of the language to translate to.</param>
	''' <param name="DefaultText">The default text to use if nothing's translated.</param>
	Public Shared Function TranslateSingleItem(ByVal sTranslationLookupFile As String, ByVal [Call] As String, ByVal Type As String,
	  ByVal ID As Integer, ByVal LanguageID As Integer, ByVal DefaultText As String) As String

		'I do not know the implications of changing this but it should almost certainly be changed to isdefaultlanguage as a parameter rather than languageID > 0
		If File.Exists(sTranslationLookupFile) AndAlso LanguageID > 0 Then
			Return TranslateByID(Translation.GetTranslationItemLookup(sTranslationLookupFile, [Call], Type), ID, DefaultText, LanguageID)
		End If

		Return DefaultText

	End Function

	''' <summary>
	''' Gets the translation item lookup from a translation lookup file.
	''' </summary>
	''' <param name="TranslationLookupFile">The translation lookup file.</param>
	''' <param name="Call">The group of translations to retrieve from the lookup file.</param>
	''' <param name="Type">The type of the translation to retrieve from the lookup file.</param>
	Public Shared Function GetTranslationItemLookup(ByVal TranslationLookupFile As String, ByVal [Call] As String, ByVal Type As String) As String

		Dim sSourceField As String = ""

		Dim oXMLTranslationLookups As New XmlDocument
		oXMLTranslationLookups.Load(TranslationLookupFile)
		Dim oXMLNode As XmlNode = oXMLTranslationLookups.SelectSingleNode(String.Format("//Lookup/TranslationLookups/TranslationLookupByID" &
   "[Call='{0}' and Type='{1}']", [Call], Type))
		If Not oXMLNode Is Nothing Then
			Return oXMLNode.SelectSingleNode("SourceFieldName").InnerText
		End If

		Return sSourceField
	End Function

	''' <summary>
	''' Gets a list of translation lookups with the specified Call from the specified file.
	''' </summary>
	''' <param name="TranslationLookupFile">The translation lookup file.</param>
	''' <param name="Call">The group of translations to retrieve from the lookup file.</param>
	Public Shared Function GetTranslationLookupsByCall(ByVal TranslationLookupFile As String, ByVal [Call] As String) As Generic.List(Of TranslationLookup)

		Dim sCacheKey As String = "Translation_GetTranslationLookupsByCall"
		Dim oReturnTransationLookups As New Generic.List(Of TranslationLookup)

		Dim oLookup As Lookup = Functions.GetCache(Of Lookup)(sCacheKey)

		If oLookup Is Nothing Then

			Dim oXMLTranslationLookups As New XmlDocument
			oXMLTranslationLookups.Load(TranslationLookupFile)

			oLookup = New Lookup
			oLookup = CType(Intuitive.Serializer.DeSerialize(GetType(Lookup), oXMLTranslationLookups.InnerXml), Lookup)

			Functions.AddToCache(sCacheKey, oLookup, New Caching.CacheDependency(TranslationLookupFile))
		End If

		For Each oTranslationLookup As TranslationLookup In oLookup.TranslationLookups
			If oTranslationLookup.Call = [Call] Then
				oReturnTransationLookups.Add(oTranslationLookup)
			End If
		Next

		Return oReturnTransationLookups

	End Function

	''' <summary>
	''' Gets a list of translation lookups with the specified call from the specified xml document.
	''' </summary>
	''' <param name="oXMLTranslationLookups">The translation lookups XML.</param>
	''' <param name="Call">The group of translations to retrieve from the lookup XML.</param>
	Public Shared Function GetTranslationLookupsByCall(ByVal oXMLTranslationLookups As XmlDocument, ByVal [Call] As String) As Generic.List(Of TranslationLookup)

		Dim oReturnTransationLookups As New Generic.List(Of TranslationLookup)

		Dim oLookup As Lookup = CType(Intuitive.Serializer.DeSerialize(GetType(Lookup), oXMLTranslationLookups.InnerXml), Lookup)

		For Each oTranslationLookup As TranslationLookup In oLookup.TranslationLookups
			If oTranslationLookup.Call = [Call] Then
				oReturnTransationLookups.Add(oTranslationLookup)
			End If
		Next

		Return oReturnTransationLookups

	End Function

#Region "clearcache"

	''' <summary>
	''' Clears translations and translationsbyid from cache
	''' </summary>
	Public Shared Sub ClearCache()
		HttpContext.Current.Cache.Remove("translations")
		HttpContext.Current.Cache.Remove("translationsbyid")
	End Sub

#End Region

#Region "Auxilliary Classes"

	''' <summary>
	''' Class containing a list of translation lookups
	''' </summary>
	Public Class Lookup
		Public TranslationLookups As New Generic.List(Of TranslationLookup)
	End Class

	''' <summary>
	''' Class representing a translation lookup
	''' </summary>
	Public Class TranslationLookup
		''' <summary>
		''' The group of translations to get from translation files
		''' </summary>
		Public [Call] As String
		''' <summary>
		''' The xpath of the item to translate
		''' </summary>
		Public XPath As String
		''' <summary>
		''' The name of the field to retrieve the translation from
		''' </summary>
		Public SourceFieldName As String
		''' <summary>
		''' The regex to match translation items
		''' </summary>
		Public Regex As String = ""
		''' <summary>
		''' Specifies whether this translation is for character data
		''' </summary>
		Public IsCData As Boolean = False
	End Class

#End Region

#End Region

#Region "XML Translator"

	''' <summary>
	''' 
	''' </summary>
	Public Class XMLTranslator

		''' <summary>
		''' The XML to translate
		''' </summary>
		Public XML As XmlDocument
		''' <summary>
		''' The id of the language to translate to
		''' </summary>
		Public LanguageID As Integer
		''' <summary>
		''' The list of XML translation definitions used for translation
		''' </summary>
		Private XMLTranslations As New Generic.List(Of XMLTranslationDefinition)

		'contructor
		''' <summary>
		''' Initializes a new instance of the <see cref="XMLTranslator"/> class.
		''' </summary>
		''' <param name="XML">The XML to translate.</param>
		''' <param name="LanguageID">The id of the language to translate to.</param>
		Public Sub New(ByVal XML As XmlDocument, ByVal LanguageID As Integer)
			Me.XML = XML
			Me.LanguageID = LanguageID
		End Sub

		'process
		''' <summary>
		''' Translates the nodes in the XML document based on the translation definitions in this instances XMLTranslations list
		''' </summary>
		Public Function Process() As XmlDocument

			If Not Me.XML Is Nothing And Me.XMLTranslations.Count > 0 Then

				For Each oTranslation As XMLTranslationDefinition In Me.XMLTranslations

					'find all nodes
					Dim aNodes As XmlNodeList = Me.XML.SelectNodes(oTranslation.NodeXPath)

					'for each node
					'	get the id
					'	get the element value (to be used for the nullif)
					'	update the element value with the translation
					For Each oNode As XmlNode In aNodes

						'ensure that both the IDElement and the ValueElement tags exist in xml document before attempting to transalte
						If Not (oNode.SelectSingleNode(oTranslation.IDElement) Is Nothing OrElse oNode.SelectSingleNode(oTranslation.ValueElement) Is Nothing) Then

							Dim iID As Integer = SafeInt(oNode.SelectSingleNode(oTranslation.IDElement).InnerText)
							Dim sElementValue As String = SafeString(oNode.SelectSingleNode(oTranslation.ValueElement).InnerText)

							If iID > 0 AndAlso sElementValue <> "" Then

								'get the value from the DB
								Dim sTranslatedValue As String = Translation.TranslateByID(oTranslation.TableName & "." & oTranslation.FieldName, iID, sElementValue, Me.LanguageID)

								'if there is a regex involved then use it
								If oTranslation.RegexPattern <> "" Then
									Dim oMatches As Match = Regex.Match(sElementValue, oTranslation.RegexPattern)
									If oMatches.Success Then
										Dim sMatch As String = oMatches.Groups("text").Value.ToString.Trim
										If sMatch <> "" AndAlso sTranslatedValue <> "" Then
											oNode.SelectSingleNode(oTranslation.ValueElement).InnerText = sElementValue.Replace(sMatch, sTranslatedValue)
										End If
									End If
								Else
									oNode.SelectSingleNode(oTranslation.ValueElement).InnerText = sTranslatedValue
								End If

							End If

						End If

					Next

				Next

			End If

			Return Me.XML

		End Function

#Region "translation def and add"

		''' <summary>
		''' Adds a new translation definition to the list of translations on this instance.
		''' </summary>
		''' <param name="NodeXPath">The node x path.</param>
		''' <param name="IDElement">The identifier element.</param>
		''' <param name="ValueElement">The value element.</param>
		''' <param name="TableName">Name of the table.</param>
		''' <param name="FieldName">Name of the field.</param>
		''' <param name="RegexPattern">The regex pattern.</param>
		Public Sub AddTranslation(ByVal NodeXPath As String, ByVal IDElement As String, ByVal ValueElement As String,
								  ByVal TableName As String, ByVal FieldName As String, Optional ByVal RegexPattern As String = "")
			Dim oTranslation As New XMLTranslationDefinition With {.NodeXPath = NodeXPath, .IDElement = IDElement,
			 .ValueElement = ValueElement, .TableName = TableName, .FieldName = FieldName, .RegexPattern = RegexPattern}
			Me.XMLTranslations.Add(oTranslation)
		End Sub

		''' <summary>
		''' Class containing information used for an xml translation
		''' </summary>
		Public Class XMLTranslationDefinition
			''' <summary>
			''' The xpath for the node to be translated
			''' </summary>
			Public NodeXPath As String
			''' <summary>
			''' The identifier element
			''' </summary>
			Public IDElement As String
			''' <summary>
			''' The element of the node whose value will be translated
			''' </summary>
			Public ValueElement As String
			''' <summary>
			''' The table to take the translation from
			''' </summary>
			Public TableName As String
			''' <summary>
			''' The name of the field to take the translation from
			''' </summary>
			Public FieldName As String
			''' <summary>
			''' The regex pattern used to match items to translate
			''' </summary>
			Public RegexPattern As String
		End Class

#End Region

	End Class

#End Region

End Class
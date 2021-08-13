Imports System.Web
Imports System.Web.UI
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Configuration.ConfigurationManager

''' <summary>
''' Class containing many useful functions
''' </summary>
Public Class Functions

	''' <summary>
	''' Gets a value indicating whether we are currently is debugging.
	''' </summary>
	Public Shared ReadOnly Property IsDebugging() As Boolean
		Get

			'hacked by pauln to prevent problem on azure / global asax
			Try
				'if we have a context check the URL
				If Not HttpContext.Current Is Nothing Then

					Return HttpContext.Current.Request.Url.ToString.StartsWith("http://localhost") AndAlso HttpContext.Current.Request.QueryString("ReleaseMode").ToSafeBoolean = False
				Else
#If DEBUG Then
					Return True
#Else
                    Return False
#End If

				End If

			Catch ex As Exception
				Return False
			End Try

		End Get
	End Property

	<Obsolete("MaxNumber is obsolete, use Math.Max instead")>
	Public Shared Function MaxNumber(ByVal n1 As Double, ByVal n2 As Double) As Double
		Return Math.Max(n1, n2)
	End Function

	<Obsolete("MaxInteger is obsolete, use Math.Max instead")>
	Public Shared Function MaxInteger(ByVal i1 As Integer, ByVal i2 As Integer) As Integer
		Return Math.Max(i1, i2)
	End Function

	''' <summary>
	''' Returns the larger of 2 objects of the specified type
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <param name="x">The first object.</param>
	''' <param name="y">The second object.</param>
	Public Shared Function Max(Of tType)(ByVal x As tType, ByVal y As tType) As tType
		If Generic.Comparer(Of tType).Default.Compare(x, y) > 0 Then
			Return x
		Else
			Return y
		End If
	End Function

	''' <summary>
	''' Returns the largest object from an array of objects of the specified type
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <param name="oValues">The array of objects to compare.</param>
	Public Shared Function Max(Of tType)(ByVal ParamArray oValues() As tType) As tType
		Array.Sort(oValues, Generic.Comparer(Of tType).Default)
		Return oValues(oValues.Length - 1)
	End Function

	''' <summary>
	''' Returns the smaller of 2 objects of the specified type
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <param name="x">The first object.</param>
	''' <param name="y">The second object.</param>
	Public Shared Function Min(Of tType)(ByVal x As tType, ByVal y As tType) As tType
		If Generic.Comparer(Of tType).Default.Compare(x, y) > 0 Then
			Return y
		Else
			Return x
		End If
	End Function

	''' <summary>
	''' Returns the smallest object from an array of objects of the specified type
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <param name="oValues">The array of objects to compare.</param>
	Public Shared Function Min(Of tType)(ByVal ParamArray oValues() As tType) As tType
		Array.Sort(oValues, Generic.Comparer(Of tType).Default)
		Return oValues(0)
	End Function

	''' <summary>
	''' Returns string builder with the specified number of characters removed from the end
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="CharactersToChop">The characters to chop.</param>
	Public Shared Sub SBChop(ByRef sb As StringBuilder, Optional ByVal CharactersToChop As Integer = 1)
		sb = New StringBuilder(Chop(sb.ToString, CharactersToChop))
	End Sub

	''' <summary>
	''' Checks whether an integer is between 2 other integers, inclusive
	''' </summary>
	''' <param name="TestValue">The test value.</param>
	''' <param name="LowerBound">The lower bound.</param>
	''' <param name="UpperBound">The upper bound.</param>
	Public Shared Function Between(ByVal TestValue As Integer, ByVal LowerBound As Integer, ByVal UpperBound As Integer) As Boolean
		Return TestValue >= LowerBound AndAlso TestValue <= UpperBound
	End Function

	''' <summary>
	''' Checks that a number range overlaps another number range
	''' </summary>
	''' <param name="iCheckStart">The starting number of the first range.</param>
	''' <param name="iCheckEnd">The ending number of the first range.</param>
	''' <param name="iStart">The starting number of the second range.</param>
	''' <param name="iEnd">The ending number of the second range.</param>
	Public Shared Function NumberRangeOverlaps(ByVal iCheckStart As Integer, ByVal iCheckEnd As Integer,
				ByVal iStart As Integer, ByVal iEnd As Integer) As Boolean

		Return (iStart >= iCheckStart AndAlso iStart <= iCheckEnd) _
			   OrElse (iEnd >= iCheckStart AndAlso iEnd <= iCheckEnd) _
			   OrElse (iStart <= iCheckStart AndAlso iEnd >= iCheckEnd)
	End Function

	''' <summary>
	''' Gets the <see cref="HttpRequest"/>s AbsoluteUri with the ApplicationPath appended if applicable
	''' </summary>
	''' <param name="Secure">if set to <c>true</c>, return secure url.</param>
	''' <param name="OverrideContext">The overriding HttpContext.</param>
	Public Shared Function GetBaseURL(Optional ByVal Secure As Boolean = False, Optional ByVal OverrideContext As HttpContext = Nothing) As String

		Dim oRequest As HttpRequest = Nothing

		'allow context to be overwritten so this can be called from a background thread
		If OverrideContext Is Nothing Then
			oRequest = HttpContext.Current.Request
		Else
			oRequest = OverrideContext.Request
		End If

		Dim sFullURL As String = oRequest.Url.AbsoluteUri.ToLower
		Dim sApplicationName As String = oRequest.ApplicationPath.ToLower
		Dim sBase As String

		If sApplicationName <> "/" Then
			sBase = sFullURL.Substring(0, sFullURL.IndexOf(sApplicationName) + sApplicationName.Length + 1)
		Else
			sBase = sFullURL.Substring(0, sFullURL.IndexOf("/", 9)) & "/"
		End If

		If Not Secure Then
			Return sBase.Replace("https:", "http:")
		Else
			Return sBase.Replace("http:", "https:")
		End If

	End Function

	''' <summary>
	''' Determines whether the current connection is secure
	''' </summary>
	Public Shared Function IsSecurePage() As Boolean

        If System.Configuration.ConfigurationManager.AppSettings("ForceSecurePages").ToSafeBoolean Then
            Return True
        ElseIf HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing Then
			Return HttpContext.Current.Request.IsSecureConnection OrElse HttpContext.Current.Request.Url.Port = 81
		Else
			Return False
		End If

	End Function

	<Obsolete("MapPath is obsolete, use HttpContext.Current.Server.MapPath(sPath) instead")>
	Public Shared Function MapPath(ByVal sWebPath As String) As String
		Return HttpContext.Current.Server.MapPath(sWebPath)
	End Function

	'Map To Virtual Path
	'I have done a side by side comparison of 1 million replaces using CaseInsensitiveReplace, and the same combining ToLower with String.Replace
	'The speed of each are almost identical
	''' <summary>
	''' Maps physical path to a virtual path.
	''' </summary>
	''' <param name="sPhysicalPath">The physical path.</param>
	''' <param name="bSecure">If true, get a secure base url</param>
	Public Shared Function MapToVirtualPath(ByVal sPhysicalPath As String, Optional ByVal bSecure As Boolean = False) As String

		'get the parent folder of the application with the base URL as a fall-back
		Dim sPhysicalApplicationPath As String = HttpContext.Current.Request.PhysicalApplicationPath
		Dim sApplicationName As String = Functions.GetBaseURL(bSecure)
		If sPhysicalApplicationPath.Split("\"c).Length > 1 Then
			sApplicationName = sPhysicalApplicationPath.Split("\"c)(sPhysicalApplicationPath.Split("\"c).Length - 2) & " "
		End If

		Return Functions.CaseInsensitiveReplace(sPhysicalPath, sPhysicalApplicationPath.Replace("\"c, "/"c), sApplicationName)
	End Function

	''' <summary>
	''' Maps physical path to specified virtual path.
	''' </summary>
	''' <param name="sPhysicalPath">The physical path to map.</param>
	''' <param name="sLocalPath">The local path to replace.</param>
	''' <param name="sRemapToURL">The url to replace the local path with.</param>
	Public Shared Function MapToVirtualPath(ByVal sPhysicalPath As String, ByVal sLocalPath As String, ByVal sRemapToURL As String) As String
		Return Functions.CaseInsensitiveReplace(sPhysicalPath, sLocalPath, sRemapToURL).Replace("\"c, "/"c)
	End Function

	''' <summary>
	''' Registers script on specified page to open the specified pdf
	''' </summary>
	''' <param name="oPage">The page.</param>
	''' <param name="sDocumentURL">The url for the PDF to display.</param>
	Public Shared Sub DisplayPDF(ByVal oPage As Page, ByVal sDocumentURL As String)

		oPage.ClientScript.RegisterStartupScript(GetType(Page), "popup",
			String.Format("<script language=""javascript"">window.open('{0}')</script>", sDocumentURL))

	End Sub

	''' <summary>
	''' Checks that the specified string is in a parameter list of strings
	''' </summary>
	''' <param name="sString">The string to check for.</param>
	''' <param name="aTest">The list to check against.</param>
	Public Shared Function InList(ByVal sString As String,
			ByVal ParamArray aTest() As String) As Boolean
		If sString Is Nothing Then Throw New ArgumentNullException("sString")

		Dim sTest As String
		For Each sTest In aTest
			If sTest Is Nothing Then Throw New ArgumentNullException("aTest", "A string in the test array was null")
			If sString.ToLower.Trim = sTest.ToLower.Trim Then
				Return True
			End If
		Next
		Return False

	End Function

	''' <summary>
	''' Checks that the specified integer is in a parameter list of integers
	''' </summary>
	''' <param name="iInteger">The integer to check.</param>
	''' <param name="aTest">Param list of integers to check against.</param>
	Public Shared Function InList(ByVal iInteger As Integer,
			ByVal ParamArray aTest() As Integer) As Boolean
		Dim iTest As Integer

		For Each iTest In aTest
			If iInteger = iTest Then
				Return True
			End If
		Next
		Return False

	End Function

	''' <summary>
	''' Checks that an object of the specified type is in a parameter list of objects of the same specified type
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <param name="oObject">The object to check.</param>
	''' <param name="aTest">Param list of objects to check against.</param>
	Public Shared Function InList(Of tType)(ByVal oObject As tType,
			ByVal ParamArray aTest() As tType) As Boolean
		Dim oTest As tType
		Dim oComparer As Generic.Comparer(Of tType) = Generic.Comparer(Of tType).Default

		For Each oTest In aTest
			If oComparer.Compare(oObject, oTest) = 0 Then
				Return True
			End If
		Next
		Return False

	End Function

	<Obsolete("Function is obsolete, use System.IO.File.ReadAllText(sFilePath) instead")>
	Public Shared Function ReadFileToString(ByVal sFilename As String) As String
		Dim sWarn As String = ""
		Dim oStreamReader As StreamReader
		Dim sReturn As String = ""

		'validate parameter and make sure file exists
		If sFilename = "" Then
			sWarn = "Filename parameter not specified"
		ElseIf Not File.Exists(sFilename) Then
			sWarn = "File " & sFilename & " could not be found"
		End If

		'read the file in
		If sWarn = "" Then
			Try
				oStreamReader = New StreamReader(sFilename)
				sReturn = oStreamReader.ReadToEnd
				oStreamReader.Close()
			Catch ex As Exception
				sReturn = ""
			Finally

			End Try
		End If

		Return sReturn
	End Function

	<Obsolete("Function is obsolete, use System.IO.File.WriteAllText(sFilePath, sString) instead")>
	Public Shared Sub WriteStringToFile(ByVal sFilename As String, ByVal sString As String)
		Functions.WriteStringToFile(sFilename, sString, System.Text.Encoding.UTF8)
	End Sub

	<Obsolete("Function is obsolete, use System.IO.File.WriteAllText(sFilePath, sString, Encoding) instead")>
	Public Shared Sub WriteStringToFile(ByVal sFilename As String, ByVal sString As String, ByVal Encoding As System.Text.Encoding)

		Try
			'so that dispose is called properly
			Using oStreamWriter As StreamWriter = New StreamWriter(sFilename, False, Encoding)
				oStreamWriter.Write(sString)
				oStreamWriter.Close()
			End Using
		Catch ex As Exception
			Dim s As String = ex.Message

		End Try
	End Sub

	''' <summary>
	''' Removes the specified number of characters from the end of the specified string, if no number is specified, removes the last character.
	''' </summary>
	''' <param name="sString">The string to chop from.</param>
	''' <param name="iCharactersToChop">The number of characters to chop from the end.</param>
	Public Shared Function Chop(ByVal sString As String,
		Optional ByVal iCharactersToChop As Integer = 1) As String

		If iCharactersToChop < sString.Length Then
			Return sString.Substring(0, sString.Length - iCharactersToChop)
		Else
			Return sString
		End If
	End Function

	''' <summary>
	''' Removes the specified number of characters from the beginning of the specified string, if no number is specified, removes the first character.
	''' </summary>
	''' <param name="sString">The string to remove the characters from.</param>
	''' <param name="iCharactersToChop">The number of characters to chop.</param>
	Public Shared Function ChopLeft(ByVal sString As String,
	   Optional ByVal iCharactersToChop As Integer = 1) As String

		If iCharactersToChop >= 1 AndAlso iCharactersToChop < sString.Length Then
			Return sString.Substring(iCharactersToChop, sString.Length - iCharactersToChop)
		Else
			Return sString
		End If
	End Function

	''' <summary>
	''' Gets the page name from the URL.
	''' </summary>
	''' <param name="oURL">The URL.</param>
	Public Shared Function GetPageFromURL(ByVal oURL As System.Uri) As String
		If oURL Is Nothing Then Return ""

		Dim sURL() As String = oURL.Segments

		If sURL.Length > 0 Then
			Return sURL(sURL.Length - 1)
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Trims specified string to max length - 2, adding '..' to the end to indicate it's been trimmed.
	''' </summary>
	''' <param name="sString">The string.</param>
	''' <param name="iMaxLength">Maximum length of the string.</param>
	Public Shared Function TrimString(ByVal sString As String,
		ByVal iMaxLength As Integer) As String

		sString = sString.Trim

		If iMaxLength > 0 And sString.Length >= iMaxLength Then
			Return sString.Substring(0, iMaxLength - 2) & ".."
		Else
			Return sString
		End If

	End Function

	''' <summary>
	''' Removes leading and trailing whitespace, returns substring with the specified max length
	''' </summary>
	''' <param name="sString">The string.</param>
	''' <param name="iMaxLength">Maximum length of the substring.</param>
	Public Shared Function SafeSubString(ByVal sString As String,
	   ByVal iMaxLength As Integer) As String

		sString = sString.Trim

		If iMaxLength > 0 And sString.Length >= iMaxLength Then
			Return sString.Substring(0, iMaxLength)
		Else
			Return sString
		End If

	End Function

	''' <summary>
	''' Retrieves the text from the specified string between the start character and the end character.
	''' </summary>
	''' <param name="sString">The string to retrieve the text from.</param>
	''' <param name="sStartChar">The start character.</param>
	''' <param name="sEndChar">The end character.</param>
	Public Shared Function RetrieveText(ByVal sString As String, ByVal sStartChar As String, ByVal sEndChar As String) As String

		Return sString.Substring(sString.IndexOf(sStartChar), sString.IndexOf(sEndChar) + sEndChar.Length - sString.IndexOf(sStartChar))

	End Function

	''' <summary>
	''' Hashes password using SHA1 cryptography
	''' </summary>
	''' <param name="sPassword">The password to hash.</param>
	Public Shared Function PasswordHash(ByVal sPassword As String) As String

		Dim Algorithm As System.Security.Cryptography.SHA1 = System.Security.Cryptography.SHA1.Create()
		Dim Data As Byte() = Algorithm.ComputeHash(Encoding.UTF8.GetBytes(sPassword))
		Dim Hashed As String = ""

		For i As Integer = 0 To Data.Length - 1
			Hashed &= Data(i).ToString("x2").ToUpperInvariant()
		Next

		Return Hashed

	End Function

	''' <summary>
	''' Converts a UTF8 string to a hex value
	''' </summary>
	''' <param name="StringToConvert">String to convert</param>
	''' <returns>Hex value</returns>
	Public Shared Function StringToHex(StringToConvert As String) As String
		Return BytesToHex(Encoding.UTF8.GetBytes(StringToConvert))
	End Function

	''' <summary>
	''' Converts a hex value to a UTF8 string
	''' </summary>
	''' <param name="HexToConvert">Hex input</param>
	''' <returns>String result</returns>
	Public Shared Function HexToString(HexToConvert As String) As String
		Return Encoding.UTF8.GetString(HexToBytes(HexToConvert))
	End Function

	''' <summary>
	''' Converts a byte array to a hex value
	''' </summary>
	''' <param name="BytesToConvert">Byte array to convert</param>
	''' <returns>Hex value</returns>
	Public Shared Function BytesToHex(BytesToConvert As Byte()) As String

		Dim Data As Byte() = BytesToConvert
		Dim Hashed As String = ""

		For i As Integer = 0 To Data.Length - 1
			Hashed &= Data(i).ToString("x2")
		Next

		Return Hashed

	End Function

	''' <summary>
	''' Converts a hex value to a byte array
	''' </summary>
	''' <param name="HexToConvert">Hex input</param>
	''' <returns>Byte array result</returns>
	Public Shared Function HexToBytes(HexToConvert As String) As Byte()

		Dim Bytes As Byte()
		Bytes = New Byte(SafeInt(HexToConvert.Length / 2) - 1) {}
		For i As Integer = 0 To Bytes.Length - 1
			Bytes(i) = Convert.ToByte(HexToConvert.Substring(i * 2, 2), 16)
		Next

		Return Bytes

	End Function

	''' <summary>
	''' Gets the root URL from the current http context request.
	''' </summary>
	Public Shared Function GetRootURL() As String

		Dim oRequest As HttpRequest = HttpContext.Current.Request
		Return "http://" & oRequest.Url.Host &
			IIf(oRequest.ApplicationPath = "/", "", oRequest.ApplicationPath).ToString & "/"

	End Function

	''' <summary>
	''' Replaces section of specified base string that matches the specified pattern with the specified replacement string. 
	''' This ignores case.
	''' </summary>
	''' <param name="sBaseString">The base string.</param>
	''' <param name="sPattern">The pattern.</param>
	''' <param name="sReplacement">The replacement.</param>
	Public Shared Function CaseInsensitiveReplace(ByVal sBaseString As String, ByVal sPattern As String, ByVal sReplacement As String) As String

		'Using a regex to do a case insensitive replace is terribly inefficient.
		'This way is about 20 times faster.

		'Get both strings in upper case to compare with
		Dim sUpperBaseString As String = sBaseString.ToUpper()
		Dim sUpperPattern As String = sPattern.ToUpper()

		'Create an array of chars to put the result string into (we must make sure it's long enough)
		Dim iNumberOfPossibleAdditionalChars As Integer = CType(Math.Ceiling((sBaseString.Length / sPattern.Length) * (sReplacement.Length - sPattern.Length)), Integer)
		Dim oChars(sBaseString.Length + Math.Max(0, iNumberOfPossibleAdditionalChars)) As Char

		'Initialize the counters (and find the first match)
		Dim iCurrentReplaceBasePosition As Integer = 0
		Dim iMatchStartPosition As Integer = sUpperBaseString.IndexOf(sUpperPattern, iCurrentReplaceBasePosition)
		Dim iCurrentCharIndex As Integer = 0

		'While we have a match
		While iMatchStartPosition <> -1

			'Add the part before the match into the char array using the original string
			For iLoop As Integer = iCurrentReplaceBasePosition To iMatchStartPosition - 1
				oChars(iCurrentCharIndex) = sBaseString(iLoop)
				iCurrentCharIndex += 1
			Next

			'Add the replacement onto the end of the char array
			For iLoop As Integer = 0 To sReplacement.Length - 1
				oChars(iCurrentCharIndex) = sReplacement(iLoop)
				iCurrentCharIndex += 1
			Next

			'Increment the base position to the end of the match
			iCurrentReplaceBasePosition = iMatchStartPosition + sPattern.Length

			'Find the next match
			iMatchStartPosition = sUpperBaseString.IndexOf(sUpperPattern, iCurrentReplaceBasePosition)

		End While

		'Return result (the if block is a speedup)
		If iCurrentReplaceBasePosition = 0 Then
			Return sBaseString
		Else

			'Finish off the remainder of the string using the characters from the original
			For iLoop As Integer = iCurrentReplaceBasePosition To sBaseString.Length - 1
				oChars(iCurrentCharIndex) = sBaseString(iLoop)
				iCurrentCharIndex += 1
			Next

			Return New String(oChars, 0, iCurrentCharIndex)
		End If

	End Function

	''' <summary>
	''' Capitalises the first letter of each word, the other letters are unchanged.
	''' </summary>
	''' <param name="sString">The string.</param>
	Public Shared Function Capitalise(ByVal sString As String) As String

		Dim sReturn As String = ""

		For Each sWord As String In sString.Split(" "c)
			sReturn += sWord.Substring(0, 1).ToUpper & sWord.Substring(1, sWord.Length - 1) & " "
		Next

		Return sReturn.Chop()

	End Function

	''' <summary>
	''' Capitalises the first letters of each word in the string, ensures the rest of the characters are lowercase.
	''' </summary>
	''' <param name="sString">The string to capitalise.</param>
	Public Shared Function CapitaliseFirstLettersOnly(ByVal sString As String) As String

		Try

			Dim sReturn As String = ""

			For Each sWord As String In sString.Split(" "c)
				sReturn += sWord.Substring(0, 1).ToUpper & sWord.Substring(1, sWord.Length - 1).ToLower & " "
			Next

			Return Intuitive.Functions.Chop(sReturn)

		Catch ex As Exception
			Return ""
		End Try

	End Function

	''' <summary>
	''' Removes foreign characters from specified string.
	''' </summary>
	''' <param name="s">The string to remove foreign characters from.</param>
	Public Shared Function RemoveForeignCharacters(ByVal s As String) As String
		Dim stFormD As String = s.Normalize(NormalizationForm.FormD)
		Dim sb As New StringBuilder

		Dim iCH As Integer = 0

		While iCH < stFormD.Length
			Dim uc As UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(stFormD(iCH))
			If uc <> UnicodeCategory.NonSpacingMark Then
				sb.Append(stFormD(iCH))
			End If
			iCH += 1
		End While

		Return sb.ToString().Normalize(NormalizationForm.FormC)
	End Function

	''' <summary>
	''' Removes ID from string, splits string based on capitalisation so "GetNiceNameID" returns "Get Nice Name"
	''' </summary>
	''' <param name="sString">The string.</param>
	''' <remarks>
	''' This will add a space if a character in the string is uppercase and either the previous character is lower case or
	''' the next character is lower case and the previous character is alphabetical
	''' so AbcDe becomes Abc De, aBCDe becomes a BC De, Abc De is unchanged, Abd (De) is unchanged
	''' </remarks>
	Public Shared Function GetNiceName(ByVal sString As String) As String

		Dim sReturn As String = ""

		sString = sString.Replace("ID", "")
		For i As Integer = 0 To sString.Length - 1

			Dim sCharacter As String = sString.Substring(i, 1)
			Dim sPrevChar As String = ""
			Dim sNextChar As String = ""

			If i > 0 Then
				sPrevChar = sString.Substring(i - 1, 1)
			End If

			If i < sString.Length - 1 Then
				sNextChar = sString.Substring(i + 1, 1)
			End If

			'See remarks above
			If i > 0 AndAlso Char.IsLetter(sCharacter, 0) AndAlso sCharacter.ToUpper = sCharacter AndAlso
			 ((Char.IsLetter(sPrevChar, 0) AndAlso sPrevChar.ToLower = sPrevChar) OrElse
			 (sNextChar <> "" AndAlso Char.IsLetter(sNextChar, 0) AndAlso sNextChar.ToLower = sNextChar AndAlso Char.IsLetter(sPrevChar, 0))) Then
				sReturn += " "
			End If

			sReturn += sCharacter

		Next

		Return sReturn

	End Function

	''' <summary>
	''' Removes keys from the session that match the specified key. If a blank key is specified, removes all keys from the session
	''' </summary>
	''' <param name="sKey">The key.</param>
	Public Shared Sub ClearSessionItems(ByVal sKey As String)
		Dim iKeys As Integer = HttpContext.Current.Session.Keys.Count
		Dim i As Integer
		Dim sSessionKey As String

		sKey = sKey.Trim.ToLower

		For i = HttpContext.Current.Session.Keys.Count - 1 To 0 Step -1
			sSessionKey = HttpContext.Current.Session.Keys(i).Trim.ToLower

			If sKey.Length = 0 OrElse
					sSessionKey.IndexOf(sKey) = 0 Then
				HttpContext.Current.Session.Remove(sSessionKey)
			End If
		Next
	End Sub

	''' <summary>
	''' Clears DLOutput, DataList and DrillList items from the session
	''' </summary>
	Public Shared Sub ClearAllSessionItems()
		ClearSessionItems("DLOutput")
		ClearSessionItems("DataList")
		ClearSessionItems("DrillList")

	End Sub

	''' <summary>
	''' Clears all keys from the HttpContext.Current cache and HttpRuntime cache, 
	''' Doesn't remove DedupePropertiesAfterDedupe_, DedupeLeadInPrices_ or DedupeProperties_ keys.
	''' </summary>
	Public Shared Sub ClearCache()

		'list of items we do not want to clear!
		Dim aItemsToKeep As New Generic.List(Of String)
		aItemsToKeep.Add("DedupePropertiesAfterDedupe_")
		aItemsToKeep.Add("DedupeLeadInPrices_")
		aItemsToKeep.Add("DedupeProperties_")
        aItemsToKeep.Add("dnr_")

		'current context cache which should be deprecated
		For Each oCacheItem As DictionaryEntry In HttpContext.Current.Cache
            Dim sKey As String = oCacheItem.Key.ToString()
            If Not Functions.CheckList(sKey, aItemsToKeep) Then
                HttpContext.Current.Cache.Remove(oCacheItem.Key.ToString)
            End If
		Next

		'runtime cache which is replacing it as it is available across multi threading
		For Each oCacheItem As DictionaryEntry In HttpRuntime.Cache
			If Not Functions.CheckList(oCacheItem.Key.ToString, aItemsToKeep) Then HttpRuntime.Cache.Remove(oCacheItem.Key.ToString)
		Next

	End Sub

	''' <summary>
	''' Removes specified key from the HttpContext.Current cache and HttpRuntime cache
	''' </summary>
	''' <param name="sKey">The key to remove.</param>
	Public Shared Sub ClearCacheKey(ByVal sKey As String)
		HttpContext.Current.Cache.Remove(sKey)
		HttpRuntime.Cache.Remove(sKey)
	End Sub

	''' <summary>
	''' Checks whether a string starts with any items in a given list
	''' </summary>
	''' <param name="sString">The string to check.</param>
	''' <param name="aList">The list to check against.</param>
	Private Shared Function CheckList(sString As String, aList As Generic.List(Of String)) As Boolean

		For Each s As String In aList
			If sString.StartsWith(s) Then
				Return True
			End If
		Next

		Return False

	End Function

	<Obsolete("This function is obsolete, use System.IO.File.Exists(sPath) instead")>
	Public Shared Function FileExists(ByVal sPath As String) As Boolean
		Return System.IO.File.Exists(sPath)
	End Function

	''' <summary>
	''' Returns a list of all children controls of the specified control that have an id. 
	''' Goes through each control and adds it's children to the list, if the child control has children they also get added to the list.
	''' </summary>
	''' <param name="oControl">The o control.</param>
	Public Shared Function GetChildControls(ByVal oControl As Control) As ArrayList
		Dim aControls As New ArrayList
		Dim aIDControls As New ArrayList
		Dim oCurrentControl As Control
		Dim oChildControl As Control
		Dim iPoint As Integer = 0

		'add form as first control
		'"recursively" go through each control
		'in the arraylist adding any child controls
		'until the last control has been 'done'
		aControls.Add(oControl)
		While (iPoint < aControls.Count)
			oCurrentControl = CType(aControls(iPoint), Control)
			If oCurrentControl.HasControls Then
				For Each oChildControl In oCurrentControl.Controls
					If oChildControl.HasControls = True Then
						aControls.Add(oChildControl)
					End If
					If Not oChildControl.ID Is Nothing Then
						aIDControls.Add(oChildControl)
					End If
				Next
			End If
			iPoint += 1
		End While

		Return aIDControls
	End Function

	''' <summary>
	''' Takes a '#' separated list and returns a list separated by ',' with an 'and' before the last item.
	''' </summary>
	''' <param name="sList">The list, items must be separated by '#', e.g. item1#item2#item3.</param>
	Public Shared Function List(ByVal sList As String) As String

		'This function takes a # separated string and returns a proper list
		'i.e. Clothes#Boots#Motorcyle becomes
		'Clothes, Boots and Motorcycle

		'If we have any hashes then proceed
		If sList.Contains("#") Then

			'Replace every # with a comma and space
			sList = sList.Replace("#", ", ")

			'Insert "and" in front of final comma
			sList = sList.Insert(sList.LastIndexOf(","), " and")

			'Remove final comma
			sList = sList.Replace("and,", "and")
		End If

		Return sList
	End Function

	''' <summary>
	''' Removes trailing zeros from a number up to the specified decimal places, if no decimal places are specified, defaults to 2
	''' </summary>
	''' <param name="sNumber">The number.</param>
	''' <param name="iDecimalPlaces">The decimal places.</param>
	Public Shared Function RemoveZeros(ByVal sNumber As String, Optional iDecimalPlaces As Integer = 2) As String

		Dim iDecimalPointIndex As Integer = sNumber.IndexOf(".")

		'If there is no decimal point return straight away like
		If iDecimalPointIndex > -1 Then

			'Just want to remove the trailing zeros like
			Do While sNumber.EndsWith("0") AndAlso (sNumber.Length - iDecimalPointIndex) > 2

				'If there are zeros to the right of a decimal point and
				'there is a non zero number between the decimal point and the number
				'then remove the zero
				If sNumber.Substring(sNumber.Length - 1, 1) = "0" Then
					sNumber = Chop(sNumber)
				End If
			Loop
		End If

		'If therre is only one number to the right of the decimal place then add a zero
		If iDecimalPlaces > 1 AndAlso sNumber.Substring(sNumber.Length - 2, 1) = "." Then sNumber += "0"

		Return sNumber
	End Function

	<Obsolete("This function is obsolete, use Math.Round instead")>
	Public Shared Function Round(ByVal nNumber As Double) As Double

		Dim nFinal As Double = (nNumber * 1000)
		Dim nRem As Double = (CDbl(nFinal) Mod 10)
		Dim nInt As Double = SafeInt(nFinal) - nRem
		If nRem >= 5 Then
			nFinal = (nInt + 10) / 1000
		Else
			nFinal = nInt / 1000
		End If

		Return nFinal

	End Function

	<Obsolete("This function is obsolete, use x Mod y instead")>
	Public Shared Function Modulus(ByVal iBigNumber As Integer, ByVal iSmallNumber As Integer) As Integer
		Return iBigNumber - (CType(Math.Floor(iBigNumber / iSmallNumber), Integer) * iSmallNumber)
	End Function

	'I dunno what the above two functions are for, but these two are genuinely useful
	''' <summary>
	''' Rounds a number up to the specified number of decimal places.
	''' </summary>
	''' <param name="nNumber">The number to round.</param>
	''' <param name="iDecimalPlaces">The decimal places to round up to.</param>
	Public Shared Function RoundUp(ByVal nNumber As Decimal, Optional ByVal iDecimalPlaces As Integer = 2) As Decimal
		Dim nRounded As Decimal = Decimal.Round(nNumber, iDecimalPlaces)
		Return nRounded + IIf(nRounded < nNumber, CType(10L ^ (-iDecimalPlaces), Decimal), 0)
	End Function

	''' <summary>
	''' Rounds a number down to the specified number of decimal places.
	''' </summary>
	''' <param name="nNumber">The number to round.</param>
	''' <param name="iDecimalPlaces">The decimal places to round down to.</param>
	Public Shared Function RoundDown(ByVal nNumber As Decimal, Optional ByVal iDecimalPlaces As Integer = 2) As Decimal
		Dim nRounded As Decimal = Decimal.Round(nNumber, iDecimalPlaces)
		Return nRounded - IIf(nRounded > nNumber, CType(10L ^ (-iDecimalPlaces), Decimal), 0)
	End Function

	''' <summary>
	''' Executes print command using adobe reader.
	''' </summary>
	''' <param name="PDFFilePath">The file path for the PDF to print.</param>
	Public Sub PrintPDF(ByVal PDFFilePath As String)
		'open
		Dim sCmd As String
		'path to the acrobat reader
		Dim sReaderPath As String = "C:\Program Files\Adobe\Acrobat 6.0\Reader\AcroRd32.exe"
		'create command line with print parameters and hide parameters
		sCmd = "" & sReaderPath & """ /p/h """ & PDFFilePath & ""
		'start process with our command line
		System.Diagnostics.Process.Start(sCmd)
	End Sub

	''' <summary>
	''' Deletes all files in specified file path that match specified file extension that are older than the specified OlderThanDate
	''' </summary>
	''' <param name="Path">The path.</param>
	''' <param name="FileExtension">The file extension for files to delete.</param>
	''' <param name="OlderThanDate">Files with a creation date older than this will be deleted.</param>
	Public Shared Sub TidyFolder(ByVal Path As String,
						ByVal FileExtension As String,
						Optional ByVal OlderThanDate As Date = #1/1/1901#)
		Dim oFile As System.IO.FileInfo
		Dim oFileInfo() As System.IO.FileInfo
		Dim oDirInfo As System.IO.DirectoryInfo

		Try
			If Path <> "" AndAlso System.IO.Directory.Exists(Path) = True Then
				'get the directory object
				oDirInfo = New System.IO.DirectoryInfo(Path)
				'get files in directory object into fileinfo array (filtered by extension)
				oFileInfo = oDirInfo.GetFiles("*." & FileExtension)

				For Each oFile In oFileInfo
					'delete file (if file is older than our OlderThanDate param)
					If oFile.CreationTime.Date <= OlderThanDate.Date Then
						oFile.Delete()
					End If
				Next
			End If
		Catch ex As Exception
			ex = Nothing
		End Try
	End Sub

	''' <summary>
	''' Uses regex to remove non digit characters from a string
	''' </summary>
	''' <param name="sString">The string.</param>
	Public Shared Function DigitsOnly(ByVal sString As String) As String
		'remove non didgit chars
		Return System.Text.RegularExpressions.Regex.Replace(sString, "\D", String.Empty)
	End Function

	''' <summary>
	''' Uses regex to remove any decimal digit characters from a string
	''' </summary>
	''' <param name="sString">The string.</param>
	Public Shared Function AlphaOnly(ByVal sString As String) As String
		'remove non alpha chars
		Return System.Text.RegularExpressions.Regex.Replace(sString, "\d", String.Empty)
	End Function

	''' <summary>
	''' Creates post request with specified against specified url.
	''' </summary>
	''' <param name="sURL">The URL to post data to.</param>
	''' <param name="sPostData">The data to post.</param>
	Public Shared Function PostData(ByVal sURL As String, ByVal sPostData As String) As String
		'request the securehosting processing page
		Dim oRequest As System.Net.HttpWebRequest =
			CType(System.Net.WebRequest.Create(sURL), System.Net.HttpWebRequest)
		Dim oWriter As StreamWriter

		'set the request object to post
		oRequest.Method = "POST"
		oRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)"
		oRequest.ContentLength = sPostData.Length
		oRequest.ContentType = "application/x-www-form-urlencoded"

		'add the post data
		'(try n catch cos GetRequestStream can bollox us
		'up and we need to close the writer no matter what)
		Try
			oWriter = New StreamWriter(oRequest.GetRequestStream())
			oWriter.Write(sPostData)
			oWriter.Close()
		Catch ex As Exception
		End Try

		'get the response
		Dim oResponse As System.Net.WebResponse = oRequest.GetResponse
		Dim srResult As StreamReader

		'get the result of our Post to the URL
		srResult = New StreamReader(oResponse.GetResponseStream())
		'return what we got from the URL post
		Return srResult.ReadToEnd
	End Function

	''' <summary>
	''' Writes all cache and session content to the trace listeners in the <see cref="system.Diagnostics.Debug.Listeners"/> collection.
	''' </summary>
	Public Shared Sub DumpCacheAndSession()

		Debug.WriteLine("Cache Items")
		Debug.WriteLine("===========")
		Dim oCacheItem As DictionaryEntry
		For Each oCacheItem In HttpContext.Current.Cache
			Debug.WriteLine(oCacheItem.Key.ToString & " - " & oCacheItem.Value.ToString)
		Next

		Debug.WriteLine("")
		Debug.WriteLine("Session Items")
		Debug.WriteLine("=============")
		Dim oSessionItem As Object
		For Each oSessionItem In HttpContext.Current.Session
			Debug.WriteLine(oSessionItem.ToString)
		Next

	End Sub

	''' <summary>
	''' Checks that the specified object is an integer.
	''' </summary>
	''' <param name="oTest">The object to check.</param>
	Public Shared Function CheckInt(ByVal oTest As Object) As Boolean

		Dim sTest As String = SafeString(oTest)

		'first check to see if it's numeric
		Return SafeInt(sTest) <> 0 OrElse sTest.Trim <> "0"

	End Function

	''' <summary>
	''' Returns converted to a string, if object is nothing, returns an empty string.
	''' </summary>
	''' <param name="oString">The object to convert to a string.</param>
	Public Shared Function SafeString(ByVal oString As Object) As String
		Dim sReturnValue As String = ""
		If Not oString Is Nothing Then
			sReturnValue = oString.ToString
		End If

		Return sReturnValue
	End Function

	''' <summary>
	''' Converts object to an integer, if unable to convert, returns 0
	''' </summary>
	''' <param name="oInteger">The object to convert to an integer.</param>
	Public Shared Function SafeInt(ByVal oInteger As Object) As Integer

		Dim iReturnValue As Integer

		Try
			iReturnValue = CInt(Math.Round(SafeNumeric(oInteger)))
		Catch
			iReturnValue = 0
		End Try

		Return iReturnValue

	End Function

	''' <summary>
	''' Converts an object to a double, if unable to convert, returns 0
	''' </summary>
	''' <param name="oDouble">The object to convert to a double.</param>
	Public Shared Function SafeNumeric(ByVal oDouble As Object) As Double

		Dim nReturnValue As Double = 0

		If Not oDouble Is Nothing Then
			Dim sDouble As String = oDouble.ToString
			If sDouble <> "" Then
				Double.TryParse(sDouble, nReturnValue)
			End If
		End If

		Return nReturnValue
	End Function

	''' <summary>
	''' Converts object to a decimal, if unable to convert, returns 0
	''' </summary>
	''' <param name="oDecimal">The object to convert to a decimal.</param>
	Public Shared Function SafeDecimal(ByVal oDecimal As Object) As Decimal

		Dim nReturnValue As Decimal = 0

		If Not oDecimal Is Nothing Then
			Dim sDecimal As String = oDecimal.ToString
			If Not sDecimal = "" AndAlso Not sDecimal.ToLower() = "nan" Then

				'convert to double, round and return
				Dim nDouble As Double = SafeNumeric(oDecimal)
				nDouble = Math.Round(nDouble, 8)
				nReturnValue = Decimal.Parse(nDouble.ToString("#########0.0##########"))
			End If
		End If

		Return nReturnValue
	End Function

	''' <summary>
	''' Converts object to decimal value with 2 decimal places
	''' </summary>
	''' <param name="oDecimal">The object to convert to currency format.</param>
	Public Shared Function SafeMoney(ByVal oDecimal As Object) As Decimal

		Return Decimal.Parse(SafeNumeric(oDecimal).ToString("##################.00"))
	End Function

	''' <summary>
	''' Converts object to a boolean
	''' </summary>
	''' <param name="oBoolean">The object to convert to a boolean.</param>
	Public Shared Function SafeBoolean(ByVal oBoolean As Object) As Boolean

		If Not oBoolean Is Nothing Then
			Dim sBoolean As String = oBoolean.ToString.ToUpper
			If Not sBoolean = "" AndAlso InList(sBoolean, "T", "TRUE", "1", "ON") Then
				Return True
			End If
		End If

		Return False

	End Function

	''' <summary>
	''' Converts object to an Enum of the specified Type
	''' </summary>
	''' <param name="oType">Type of Enum to convert the object to.</param>
	''' <param name="oValue">The object to convert to an enum.</param>
	''' <param name="bIgnoreSpaces">If set to <c>true</c>, removes spaces from value before converting.</param>
	Public Shared Function SafeEnum(ByVal oType As System.Type, ByVal oValue As Object, Optional ByVal bIgnoreSpaces As Boolean = False) As Object

		If Not oType.IsEnum Then Return Nothing

		Dim oReturnObject As Object

		If oValue Is Nothing Then
			oReturnObject = [Enum].ToObject(oType, 0)

		Else
			Dim sValue As String = oValue.ToString
			If bIgnoreSpaces Then sValue = sValue.Replace(" "c, "")

			Dim iValue As Integer

			If Integer.TryParse(sValue, iValue) Then
				oReturnObject = [Enum].ToObject(oType, iValue)

				If Array.IndexOf([Enum].GetValues(oType), oReturnObject) = -1 Then
					oReturnObject = [Enum].ToObject(oType, 0)
				End If
			Else
				Try
					oReturnObject = [Enum].Parse(oType, sValue, True)
				Catch ex As Exception
					oReturnObject = [Enum].ToObject(oType, 0)
				End Try
			End If
		End If

		Return oReturnObject

	End Function

	''' <summary>
	''' Converts object to an Enum of the specified Type
	''' </summary>
	''' <typeparam name="tEnumType">Type of Enum to convert the object to.</typeparam>
	''' <param name="oValue">The object to convert to an enum.</param>
	''' <param name="bIgnoreSpaces">If set to <c>true</c>, removes spaces from value before converting.</param>
	Public Shared Function SafeEnum(Of tEnumType As New)(ByVal oValue As Object, Optional ByVal bIgnoreSpaces As Boolean = False) As tEnumType
		Return CType(Functions.SafeEnum(GetType(tEnumType), oValue, bIgnoreSpaces), tEnumType)
	End Function

	Public Shared Function SafeXmlAttributeEnum(Of T)(ByVal value As String) As T

		For Each o As Object In [Enum].GetValues(GetType(T))
			Dim enumValue As T = CType(o, T)
			If GetEnumXmlAttributeName(enumValue) = value Then
				Return CType(o, T)
			End If
		Next

		Throw New ArgumentException("No code exists for type " + GetType(T).ToString() + " corresponding to value of " + value)
	End Function

	Public Shared Function GetEnumXmlAttributeName(Of T)(ByVal pEnumVal As T) As String

		Dim type As Type = pEnumVal?.GetType()
		Dim info As FieldInfo = type.GetField([Enum].GetName(GetType(T), pEnumVal))
		Dim attributes As Object() = info.GetCustomAttributes(GetType(XmlEnumAttribute), False)

		If attributes.Length = 0 Then
			Return info.Name
		Else
			Return CType(attributes(0), XmlEnumAttribute).Name
		End If

	End Function

	''' <summary>
	''' Evaluated expression and returns an object depending on whether the expression evaluated to true or false
	''' </summary>
	''' <typeparam name="tType">The type of object to return.</typeparam>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="oTrueObject">The object to return if expression is true.</param>
	''' <param name="oFalseObject">The object to return if expression is false.</param>
	Public Shared Function IIf(Of tType)(ByVal bTest As Boolean, ByVal oTrueObject As tType, ByVal oFalseObject As tType) As tType
		If bTest Then
			Return oTrueObject
		Else
			Return oFalseObject
		End If
	End Function

	''' <summary>
	''' Evaluated expression and returns a string depending on whether the expression evaluated to true or false
	''' </summary>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="sTrueString">The string to return if expression is true.</param>
	''' <param name="sFalseString">The string to return if expression is false.</param>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal sTrueString As String,
		ByVal sFalseString As String) As String
		If bTest Then
			Return sTrueString
		Else
			Return sFalseString
		End If
	End Function

	<Obsolete("This function if obsolete, use Microsoft.VisualBasic.IIF(bTest, oTrueObject, oFalseObject) instead")>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal sTrueObject As Object,
		ByVal sFalseObject As Object) As Object

		If bTest Then
			Return sTrueObject
		Else
			Return sFalseObject
		End If

	End Function

	''' <summary>
	''' Evaluated expression and returns an integer depending on whether the expression evaluated to true or false
	''' </summary>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="iTrueInteger">The integer to return if expression is true.</param>
	''' <param name="iFalseInteger">The integer to return if expression is false.</param>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal iTrueInteger As Integer,
		ByVal iFalseInteger As Integer) As Integer
		If bTest Then
			Return iTrueInteger
		Else
			Return iFalseInteger
		End If
	End Function

	''' <summary>
	''' Evaluated expression and returns a double depending on whether the expression evaluated to true or false
	''' </summary>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="TrueDouble">The double to return if expression is true.</param>
	''' <param name="FalseDouble">The double to return if expression is false.</param>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal TrueDouble As Double,
		ByVal FalseDouble As Double) As Double
		If bTest Then
			Return TrueDouble
		Else
			Return FalseDouble
		End If
	End Function

	''' <summary>
	''' Evaluated expression and returns a decimal depending on whether the expression evaluated to true or false
	''' </summary>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="TrueDecimal">The decimal to return if expression is true.</param>
	''' <param name="FalseDecimal">The decimal to return if expression is false.</param>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal TrueDecimal As Decimal,
		ByVal FalseDecimal As Decimal) As Decimal
		If bTest Then
			Return TrueDecimal
		Else
			Return FalseDecimal
		End If
	End Function

	''' <summary>
	''' Evaluated expression and returns a date depending on whether the expression evaluated to true or false
	''' </summary>
	''' <param name="bTest">The expression to evaluate.</param>
	''' <param name="TrueDate">The date to return if expression is true.</param>
	''' <param name="FalseDate">The date to return if expression is false.</param>
	Public Shared Function IIf(ByVal bTest As Boolean, ByVal TrueDate As Date,
	   ByVal FalseDate As Date) As Date
		If bTest Then
			Return TrueDate
		Else
			Return FalseDate
		End If
	End Function

#Region "Caching"

	'AddToCache
	<Obsolete("This function is obsolete, Caching by dependency file has been deprecated")>
	Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object,
			ByVal sDependencyFile As String)

		'pn 10/04/2012
		Throw New Exception("Caching by dependency file has been deprecated")

	End Sub

	''' <summary>
	''' Adds object to cache with specified dependency and no expiration date.
	''' </summary>
	''' <param name="sCacheName">Name of the cache key.</param>
	''' <param name="oObject">The object to add.</param>
	''' <param name="oDependency">The <see cref="Caching.CacheDependency"/>.</param>
	Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object,
	ByVal oDependency As Caching.CacheDependency)

		Dim oCache As Caching.Cache = HttpRuntime.Cache

		oCache.Insert(sCacheName, oObject, oDependency, Caching.Cache.NoAbsoluteExpiration, Caching.Cache.NoSlidingExpiration)

	End Sub

	''' <summary>
	''' Adds object to either the specified cache or the current application cache if none is specified. 
	''' Caches the object for the specified number of minutes, if minutes is set to 0, cache will not expire.
	''' </summary>
	''' <param name="sCacheName">The cache key name.</param>
	''' <param name="oObject">The object to store.</param>
	''' <param name="iMinutes">How long to cache the object for in minutes.</param>
	''' <param name="oCache">The cache to add the object to.</param>
	Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object,
			ByVal iMinutes As Integer, Optional ByVal oCache As Caching.Cache = Nothing)

		If oCache Is Nothing Then oCache = HttpRuntime.Cache

		Dim dExpiration As Date = Date.Now.AddMinutes(iMinutes)
		If iMinutes = 0 Then dExpiration = Caching.Cache.NoAbsoluteExpiration

		oCache.Insert(sCacheName, oObject, Nothing, dExpiration, Nothing)

	End Sub

	''' <summary>
	''' Adds object to specified cache with no expiry date and no dependency
	''' </summary>
	''' <param name="sCacheName">The cache key name.</param>
	''' <param name="oObject">The object to add to the cache.</param>
	''' <param name="oCache">The cache to add the object to.</param>
	Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object, ByVal oCache As Caching.Cache)
		AddToCache(sCacheName, oObject, 0, oCache)
	End Sub

	''' <summary>
	''' Adds object to current application cache with no dependencies and no expiration
	''' </summary>
	''' <param name="sCacheName">The cache key name.</param>
	''' <param name="oObject">The object to add to the cache.</param>
	Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object)
		AddToCache(sCacheName, oObject, 0)
	End Sub

	<Obsolete("This function is obsolete, use HttpRuntime.Cache(sCacheName) instead")>
	Public Shared Function GetCache(ByVal sCacheName As String) As Object
		Return HttpRuntime.Cache(sCacheName)
	End Function

	''' <summary>
	''' Gets an object from the cache based on the cache key, converts it to the specified type.
	''' </summary>
	''' <typeparam name="tObjectType">The type of the object to return.</typeparam>
	''' <param name="sCacheName">Cache key to get the object from.</param>
	Public Shared Function GetCache(Of tObjectType As Class)(ByVal sCacheName As String) As tObjectType
		Return CType(HttpRuntime.Cache(sCacheName), tObjectType)
	End Function

	<Obsolete("This function is obsolete, use HttpRuntime.Cache.Remove(sCacheName) instead")>
	Public Shared Sub RemoveCache(ByVal sCacheName As String)
		HttpRuntime.Cache.Remove(sCacheName)
	End Sub

#End Region

	'Credit Card Mask
	''' <summary>
	''' Masks the middle 8 numbers of provided credit card number, replaces the numbers with xxxx-xxxx. 
	''' e.g. 4111111111111111 becomes 4111-xxxx-xxxx-1111
	''' </summary>
	''' <param name="sCreditCardNumber">The credit card number.</param>
	Public Shared Function MaskCreditCard(ByVal sCreditCardNumber As String) As String

		Dim sMaskedCreditCardNumber As String

		If sCreditCardNumber.Length > 4 Then

			sMaskedCreditCardNumber = sCreditCardNumber.Substring(0, 4) 'first four chars
			sMaskedCreditCardNumber += "-xxxx-xxxx-" ' blank out middle
			sMaskedCreditCardNumber += sCreditCardNumber.Substring(sCreditCardNumber.Length - 4, 4) 'last four chars

		Else
			sMaskedCreditCardNumber = sCreditCardNumber
		End If

		Return sMaskedCreditCardNumber

	End Function

	''' <summary>
	''' Converts control object to a string.
	''' </summary>
	''' <param name="oControl">The control.</param>
	Public Shared Function RenderControlToString(ByVal oControl As Control) As String

		Dim oWriter As New StringWriter
		Dim oHTMLWriter As New Html32TextWriter(oWriter)
		oControl.RenderControl(oHTMLWriter)
		Return oWriter.ToString

	End Function

	''' <summary>
	''' Converts control object to a string while running through asp page lifecycle
	''' </summary>
	''' <param name="oControl">The control.</param>
	''' <param name="oControlPage">Page container for control</param>
	Public Shared Function RenderUserControlToString(ByVal oControl As Control, oControlPage As Page) As String

		Dim oWriter As New StringWriter
		oControlPage.Controls.Add(oControl)
		HttpContext.Current.Server.Execute(oControlPage, oWriter, False)
		Return oWriter.ToString()

	End Function

	''' <summary>
	''' Converts boolean to 'Yes' or 'No'
	''' </summary>
	''' <param name="oBoolean">The boolean.</param>
	Public Shared Function DisplayBoolean(ByVal oBoolean As Object) As String

		Dim bBoolean As Boolean = SafeBoolean(oBoolean)
		If bBoolean Then
			Return "Yes"
		ElseIf oBoolean.ToString = "" Then
			Return ""
		Else
			Return "No"
		End If

	End Function

	''' <summary>
	''' Removes non-permitted tags from given XHTML.
	''' </summary>
	''' <param name="sXHTML">The XHTML.</param>
	Public Shared Function CleanXHTML(ByVal sXHTML As String) As String

		Dim sTag As String = "</?.+?>"
		Dim sPermittedTag As String = "</?(p|/p|div|/div|strong|/strong|em|/em|span|/span|h1|/h1|h2|/h2|h3|/h3|" &
			"ul|/ul|ol|/ol|li|/li|br|br/)(.+?)?/?>"

		'match every html tag
		Dim aMatches As MatchCollection = Regex.Matches(sXHTML, sTag)
		Dim sMatch As String
		Dim sContent As String
		For Each oMatch As Match In aMatches

			sMatch = oMatch.ToString

			'first change to lowercase
			sXHTML = sXHTML.Replace(sMatch, sMatch.ToLower)

			'if it's not a permitted tag then remove it completely
			If Not Regex.IsMatch(sMatch.ToLower, sPermittedTag) Then
				sXHTML = sXHTML.Replace(sMatch.ToLower, "")
			End If

			'if it is then check the content of the tag to make sure it's ok
			If Regex.IsMatch(sMatch.ToLower, sPermittedTag) AndAlso sMatch.IndexOf(" ") > -1 Then

				'get the cotent out of the tag
				sContent = sMatch.ToLower.Substring(sMatch.IndexOf(" "), sMatch.IndexOf(">") - sMatch.IndexOf(" ")).Trim

				'check it is permitted
				If Not InList(sContent, "class=""rightalign""", "class=""leftalign""", "class=""centeralign""",
						"class=""underline""") Then
					sXHTML = sXHTML.Replace(sMatch.ToLower, "")
				End If
			End If
		Next

		'clean xml
		sXHTML = XMLFunctions.CleanXMLNamespaces(sXHTML).OuterXml

		Return sXHTML
	End Function

	''' <summary>
	''' Cleans given XML.
	''' </summary>
	''' <param name="sString">The string.</param>
	<Obsolete("Use XMLFunctions.CleanXMLNamespaces instead")>
	Public Shared Function CleanXML(ByVal sString As String) As String

		Dim sb As New StringBuilder
		Dim iOpenTag As Integer
		Dim iCloseTag As Integer
		Dim oStack As New Stack
		Dim oIndexStack As New Stack
		Dim iPoint As Integer
		Dim oTag As XMLTag
		Dim bDiscard As Boolean
		Dim iIndex As Integer = 0

		While iIndex < sString.Length

			'get next bit of content, tag - 0 get tag, > 0 add content, -1 end
			iOpenTag = sString.IndexOf("<"c, iIndex)
			iCloseTag = sString.IndexOf(">"c, iIndex)
			bDiscard = False

			If iOpenTag = iIndex And iCloseTag > iIndex Then
				oTag = New XMLTag(sString.Substring(iOpenTag, iCloseTag - iOpenTag + 1))

				If Not oTag.Closing Then
					oStack.Push(oTag)
					oIndexStack.Push(iIndex)
				Else

					'check to see if matches, if not discard
					If oStack.Count > 0 AndAlso CType(oStack.Peek, XMLTag).TagType = oTag.TagType Then
						oStack.Pop()
						oIndexStack.Pop()
					Else
						bDiscard = True
					End If

				End If

				iPoint = iCloseTag + 1
			ElseIf iOpenTag > iIndex Then
				iPoint = iOpenTag
			Else
				iPoint = sString.Length
			End If

			If Not bDiscard Then
				sb.Append(sString.Substring(iIndex, iPoint - iIndex))
			End If

			iIndex = iPoint
		End While

		'get rid of any remaining unclosed tags
		Dim sXHTML As String = sb.ToString
		While oStack.Count > 0

			oTag = CType(oStack.Pop, XMLTag)
			iIndex = SafeInt(oIndexStack.Pop)

			'remove the tag unless it's a br
			If oTag.TagType <> "br" Then sXHTML = sXHTML.Remove(iIndex, oTag.TagText.Length)
		End While

		Return sXHTML
	End Function

	<Obsolete("Use sBase.PadLeft(iZeroes, ""0""c) instead")>
	Public Shared Function PadWithZeros(ByVal sBase As String, ByVal iZeroes As Integer) As String
		Return sBase.PadLeft(iZeroes, "0"c)
	End Function

	<Obsolete("Use sBase.PadRight(iSpaces) instead")>
	Public Shared Function PadLeft(ByVal sBase As String, ByVal iSpaces As Integer) As String
		Return sBase.PadRight(iSpaces)
	End Function

	''' <summary>
	''' Removes leading and trailing whitespace from string then pads it with spaces until it reaches the desired length. 
	''' If the string is longer than the desired length, it trims the whitespace and returns the string.
	''' </summary>
	''' <param name="Base">The base string.</param>
	''' <param name="Length">The desired string length.</param>
	''' <param name="PadLeft">If set to <c>true</c>, will add the spaces to the beginning of the string rather than the end.</param>
	Public Shared Function CreateFixedLengthString(ByVal Base As String, ByVal Length As Integer, Optional ByVal PadLeft As Boolean = False) As String

		Base = Base.Trim

		Dim sSpaces As String = ""

		Dim iSpaces As Integer = Length - Base.Length
		If iSpaces > 0 Then
			sSpaces = New String(" "c, iSpaces)
		End If

		Return IIf(PadLeft, sSpaces & Base, Base & sSpaces)

	End Function

	''' <summary>
	''' Converts datatable to an arraylist of strings with 1 entry per row
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToArrayList(ByVal dt As DataTable) As ArrayList

		Dim aList As New ArrayList
		For Each dr As DataRow In dt.Rows

			aList.Add(dr(0).ToString)
		Next
		Return aList
	End Function

	''' <summary>
	''' Converts datatable to an arraylist of integers with 1 entry per row
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToIntegerArrayList(ByVal dt As DataTable) As ArrayList

		Dim aList As New ArrayList
		For Each dr As DataRow In dt.Rows

			aList.Add(SafeInt(dr(0)))
		Next
		Return aList
	End Function

	''' <summary>
	''' Converts datatable to a generic list of the specified type.
	''' </summary>
	''' <typeparam name="tType">The type of the list to generate.</typeparam>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToGenericList(Of tType)(ByVal dt As DataTable) As Generic.List(Of tType)

		Dim oList As New Generic.List(Of tType)
		For Each dr As DataRow In dt.Rows
			oList.Add(CType(dr(0), tType))
		Next

		Return oList
	End Function

	''' <summary>
	''' Converts datatable to a string of options separated by '#', for use with dropdown options.
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToOptionList(ByVal dt As DataTable) As String

		If dt.Rows.Count > 0 Then

			Dim sb As New StringBuilder
			For Each dr As DataRow In dt.Rows

				sb.Append(dr(0)).Append("#")
			Next

			Return Chop(sb.ToString)
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Converts datatable to a string of options and their values.
	''' Each entry is separated by a '#', options and their values are separated by '|'.
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToOptionValueList(ByVal dt As DataTable) As String

		If dt.Columns.Count = 1 Then

			Dim sb As New StringBuilder
			For Each dr As DataRow In dt.Rows
				sb.AppendFormat("{0}|{1}#", dr(0), dr(0))
			Next

			Return Functions.Chop(sb.ToString)

		ElseIf dt.Rows.Count > 0 Then

			Dim sb As New StringBuilder
			For Each dr As DataRow In dt.Rows
				sb.AppendFormat("{0}|{1}#", dr(0), dr(1))
			Next

			Return Functions.Chop(sb.ToString)

		Else
			Return ""
		End If

	End Function

	''' <summary>
	''' Removes invalid characters from specified file name
	''' </summary>
	''' <param name="sFileName">File to remove unsafe characters from.</param>
	Public Shared Function SafeFileName(ByVal sFileName As String) As String

		sFileName = sFileName.Replace("*", " ").Replace(".", " ").Replace("""", " ").Replace("/", " ")
		sFileName = sFileName.Replace("\", " ").Replace("[", " ").Replace("]", " ").Replace(":", " ")
		sFileName = sFileName.Replace(";", " ").Replace("|", " ").Replace("=", " ").Replace(",", " ")

		Return sFileName
	End Function

	''' <summary>
	''' Formats a double amount as a monetary value
	''' </summary>
	''' <param name="nAmount">The amount to format.</param>
	''' <param name="sCurrency">The currency symbol.</param>
	''' <param name="SymbolPosition">The symbol position.</param>
	Public Shared Function FormatMoney(ByVal nAmount As Double, Optional ByVal sCurrency As String = "", Optional ByVal SymbolPosition As String = "Preprend") As String

		Dim bNeg As Boolean = nAmount < 0
		If bNeg Then nAmount = nAmount * -1

		Dim sFormatString As String = "{0}{1}{2}"
		If SymbolPosition = "Append" Then sFormatString = "{0}{2}{1}" 'switch it all around
		Return String.Format(sFormatString, IIf(bNeg, "-", ""), sCurrency, nAmount.ToString("###,###,###,##0.00"))

	End Function

	''' <summary>
	''' Formats a decimal amount as a monetary value
	''' </summary>
	''' <param name="nAmount">The decimal amount to format</param>
	''' <param name="sCurrency">The currency symbol eg "£"</param>
	''' <param name="SymbolPosition">Prepend or Append - the position currency symbol</param>
	Public Shared Function FormatMoney(ByVal nAmount As Decimal, Optional ByVal sCurrency As String = "", Optional ByVal SymbolPosition As String = "Preprend") As String

		Dim bNeg As Boolean = nAmount < 0
		If bNeg Then nAmount = nAmount * -1

		Dim sFormatString As String = "{0}{1}{2}"
		If SymbolPosition = "Append" Then sFormatString = "{0}{2}{1}" 'switch it all around
		Return String.Format(sFormatString, IIf(bNeg, "-", ""), sCurrency, nAmount.ToString("###,###,###,##0.00"))

	End Function

	''' <summary>
	''' Generates a random integer between the minimum and maximum numbers specified.
	''' </summary>
	''' <param name="iMin">The minimum.</param>
	''' <param name="iMax">The maximum.</param>
	Public Shared Function RandomInteger(ByVal iMin As Integer, ByVal iMax As Integer) As Integer
		Dim oRandom As New Random()
		Return oRandom.Next(iMin, iMax)
	End Function

	<Obsolete("Please don't use this")>
	Public Shared Function GetPlural(ByVal iThings As Integer) As String
		If iThings <> 1 Then
			Return "s"
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Gets the last word from a string.
	''' </summary>
	''' <param name="sBase">The string.</param>
	''' <param name="sDelimiter">The delimiter used to split the string.</param>
	Public Shared Function GetLastWord(ByVal sBase As String, Optional ByVal sDelimiter As Char = " "c) As String

		Dim aWords As String() = sBase.Split(sDelimiter)
		Return aWords(aWords.Length - 1)

	End Function

	'Copies a stream into memory so that we don't need the original anymore
	''' <summary>
	''' Converts stream to a memory stream
	''' </summary>
	''' <param name="oStreamToRelease">The stream to release.</param>
	''' <param name="bDisposeOfOriginal">If set to <c>true</c>, disposes of stream after storing it in buffer array.</param>
	Public Shared Function ReleaseStream(ByVal oStreamToRelease As System.IO.Stream, Optional ByVal bDisposeOfOriginal As Boolean = True) As System.IO.Stream

		Dim iStreamLength As Integer = CType(oStreamToRelease.Length, Integer)

		Dim oBuffer(iStreamLength) As Byte
		oStreamToRelease.Read(oBuffer, 0, iStreamLength)

		If bDisposeOfOriginal Then oStreamToRelease.Dispose()

		Dim oMemoryStream As New System.IO.MemoryStream(oBuffer)
		Return oMemoryStream

	End Function

	'Removes empty columns from a datatable
	''' <summary>
	''' Removes empty columns from datatable.
	''' </summary>
	''' <param name="dt">The datatable.</param>
	Public Shared Sub RemoveEmptyDatatableColumns(ByVal dt As DataTable)
		Dim aEmptyColumns(dt.Columns.Count - 1) As Boolean

		For i As Integer = 0 To dt.Columns.Count - 1
			aEmptyColumns(i) = True
		Next

		Dim bNoEmptyColumns As Boolean

		For Each dr As DataRow In dt.Rows
			bNoEmptyColumns = True

			For i As Integer = 0 To dt.Columns.Count - 1
				If aEmptyColumns(i) = False Then
					Continue For
				Else
					aEmptyColumns(i) = aEmptyColumns(i) AndAlso (dr.IsNull(i) OrElse dr.Item(i).ToString = "")
					bNoEmptyColumns = bNoEmptyColumns AndAlso Not aEmptyColumns(i)
				End If

			Next

			If bNoEmptyColumns Then Exit For
		Next

		If Not bNoEmptyColumns Then
			For i As Integer = 0 To dt.Columns.Count - 1
				If aEmptyColumns(i) AndAlso dt.Columns.CanRemove(dt.Columns(i)) Then dt.Columns.RemoveAt(i)
			Next
		End If
	End Sub

#Region "Array List"

	''' <summary>
	''' Checks the arraylist contains the string, case insensitive
	''' </summary>
	''' <param name="aArrayList">The array list to check.</param>
	''' <param name="sString">The string to check for.</param>
	Public Shared Function ArrayListContains(ByVal aArrayList As ArrayList,
		ByVal sString As String) As Boolean


		For Each sTest As String In aArrayList
			If sTest.ToLower.IndexOf(sString.ToLower.Trim) > -1 Then
				Return True
			End If
		Next

		Return False
	End Function

	<Obsolete("Use oArrayList.Cast(Of Type)().ToList()")>
	Public Shared Function ArrayListToGenericList(Of T)(ByVal oArrayList As ArrayList) As Generic.List(Of T)

		Dim oGenericList As New Generic.List(Of T)

		For Each oObject As Object In oArrayList
			oGenericList.Add(CType(oObject, T))
		Next

		Return oGenericList
	End Function

#End Region

#Region "CSV/Delimiter functions"
	''' <summary>
	''' Splits delimited string on separator, adds each subsequent item to an arraylist.
	''' </summary>
	''' <param name="sDelimited">The delimited string.</param>
	''' <param name="sSeperator">The seperator.</param>
	Public Shared Function DelimitedStringToArrayList(ByVal sDelimited As String, Optional ByVal sSeperator As Char = ","c) As ArrayList
		Dim aReturn As New ArrayList

		For Each sElement As String In sDelimited.Split(sSeperator)
			aReturn.Add(sElement)
		Next

		Return aReturn
	End Function

	''' <summary>
	''' Splits delimited string on separator, adds each subsequent item to an arraylist.
	''' </summary>
	''' <param name="Source">The delimited string.</param>
	''' <param name="Separator">The separator.</param>
	Public Shared Function DelimitedStringToGenericList(ByVal Source As String, Optional ByVal Separator As Char = ","c) As Generic.List(Of Integer)

		Dim aReturn As New Generic.List(Of Integer)

		If Source <> "" Then
			For Each sItem As String In Source.Split(Separator)
				aReturn.Add(SafeInt(sItem))
			Next
		End If

		Return aReturn
	End Function

	''' <summary>
	''' Arrays the list to delimited string.
	''' </summary>
	''' <param name="aArrayList">Array list to convert.</param>
	''' <param name="sSeperator">The seperator to use in the delimited string.</param>
	<Obsolete("Use String.Join(sSeperator, aArrayList.ToArray) instead")>
	Public Shared Function ArrayListToDelimitedString(ByVal aArrayList As ArrayList, Optional ByVal sSeperator As String = ",") As String

		Dim sb As New StringBuilder()
		For Each oItem As Object In aArrayList
			sb.Append(oItem.ToString).Append(sSeperator)
		Next
		Return Chop(sb.ToString, sSeperator.Length)

	End Function

	'Don't you think this way is much more novel? - Tom ;)
	''' <summary>
	''' Creates delimited string from a generic list of the specified type
	''' </summary>
	''' <typeparam name="tType">The type of objects in the generic list.</typeparam>
	''' <param name="aList">a list.</param>
	''' <param name="sDelimiter">The s delimiter.</param>
	Public Shared Function GenericListToDelimitedString(Of tType)(ByVal aList As Generic.List(Of tType), Optional ByVal sDelimiter As String = ",") As String
		If aList Is Nothing Then Return ""
		Return String.Join(sDelimiter, aList.ToArray)
	End Function

	''' <summary>
	''' Converts values from datatable to a CSV, taking the first value in each datarow.
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	Public Shared Function DatatableToCSVString(ByVal dt As DataTable) As String

		Dim sb As New StringBuilder

		For Each dr As DataRow In dt.Rows
			sb.Append(dr(0)).Append(",")
		Next

		Return Chop(sb.ToString)

	End Function

	''' <summary>
	''' Converts datatable to CSV using specified number of columns and including column headers if desired.
	''' </summary>
	''' <param name="dt">The datatable to convert.</param>
	''' <param name="iColumnsToUse">The number of columns to use.</param>
	''' <param name="IncludeColumnHeaders">If set to <c>true</c>, include column headers in csv.</param>
	Public Shared Function DatatableToCSV(ByVal dt As DataTable,
			Optional ByVal iColumnsToUse As Integer = 0, Optional IncludeColumnHeaders As Boolean = True) As String

		Dim sb As New StringBuilder
		If iColumnsToUse = 0 Then iColumnsToUse = dt.Columns.Count

		'start with the column names
		If IncludeColumnHeaders Then
			Dim dc As DataColumn
			For iLoop As Integer = 0 To iColumnsToUse - 1
				dc = dt.Columns(iLoop)
				sb.Append(dc.ColumnName).Append(",")
			Next
		End If

		'do a new line
		sb.Append(Environment.NewLine)

		For Each dr As DataRow In dt.Rows

			For iLoop As Integer = 0 To iColumnsToUse - 1
				sb.Append(dr(iLoop)).Append(",")
			Next

			sb.Append(Environment.NewLine)
		Next

		Return sb.ToString

	End Function

	''' <summary>
	''' Converts datatable to a csv, escapes quotation marks around strings.
	''' </summary>
	''' <param name="dt">The datatable.</param>
	Public Shared Function DatatableToEscapedCSV(ByVal dt As DataTable) As String

		Dim sb As New StringBuilder

		'1. work out columns that are strings
		Dim aStringColumns As New Generic.List(Of Integer)
		For i As Integer = 0 To dt.Columns.Count - 1
			If dt.Columns(i).DataType.FullName = "System.String" Then aStringColumns.Add(i)
		Next

		'2. build up column names
		For Each oColumn As DataColumn In dt.Columns
			sb.Append(oColumn.ColumnName).Append(",")
		Next
		sb.Append(Environment.NewLine)

		'3. scan through rows
		For Each dr As DataRow In dt.Rows

			For iColumn As Integer = 0 To dt.Columns.Count - 1
				If Not aStringColumns.Contains(iColumn) Then
					sb.Append(dr(iColumn)).Append(",")
				Else
					sb.Append("""").Append(dr(iColumn).ToString.Replace("""", """""")).Append("""").Append(",")
				End If

			Next

			sb.Append(Environment.NewLine)
		Next

		Return sb.ToString

	End Function

	''' <summary>
	''' Converts datatable to an XML Document, creates a new node for each datarow
	''' </summary>
	''' <param name="dt">The datatable.</param>
	''' <param name="NodeName">Name to use for root and each node.</param>
	Public Shared Function DatatableToXMLDocument(ByVal dt As DataTable, NodeName As String) As System.Xml.XmlDocument

		Dim sb As New StringBuilder

		'1. work out columns that are strings
		Dim aStringColumns As New Generic.List(Of Integer)
		For i As Integer = 0 To dt.Columns.Count - 1
			If dt.Columns(i).DataType.FullName = "System.String" Then aStringColumns.Add(i)
		Next

		'2. root node
		sb.Append("<").Append(NodeName).Append("s>")

		'3. scan through rows
		For Each dr As DataRow In dt.Rows

			sb.Append("<").Append(NodeName).Append(">")

			For iColumn As Integer = 0 To dt.Columns.Count - 1

				sb.Append("<").Append(dt.Columns(iColumn).ColumnName).Append(">")

				If Not aStringColumns.Contains(iColumn) Then
					sb.Append(dr(iColumn))
				Else
					sb.Append(System.Web.HttpUtility.HtmlEncode(dr(iColumn).ToString))
				End If

				sb.Append("</").Append(dt.Columns(iColumn).ColumnName).Append(">")

			Next

			sb.Append("</").Append(NodeName).Append(">")
		Next

		'4. close root node
		sb.Append("</").Append(NodeName).Append("s>")

		'5 load into xml document
		Dim oXML As New System.Xml.XmlDocument
		oXML.LoadXml(sb.ToString)

		'6. return
		Return oXML

	End Function

	''' <summary>
	''' Creates csv from a generic list of integers.
	''' </summary>
	''' <param name="o">The list of integers to convert.</param>
	Public Shared Function GenericIntegerListToDelimitedString(ByVal o As Generic.List(Of Integer)) As String

		Return Functions.GenericListToDelimitedString(o)

	End Function

#End Region

#Region "Support Classes"

	'xml tag
	''' <summary>
	''' Class representing an xml tag
	''' </summary>
	Private Class XMLTag

		''' <summary>
		''' The tag text
		''' </summary>
		Public TagText As String
		''' <summary>
		''' The tag type
		''' </summary>
		Public TagType As String
		''' <summary>
		''' Specifies whether this tag is a closing tag
		''' </summary>
		Public Closing As Boolean

		''' <summary>
		''' Initializes a new instance of the <see cref="XMLTag"/> class.
		''' </summary>
		''' <param name="Tag">The tag.</param>
		Public Sub New(ByVal Tag As String)

			Me.TagText = Tag

			Dim iOpenTag As Integer = Tag.IndexOf("<"c)
			Dim iCloseOpenTag As Integer = Tag.IndexOf("</")
			Dim iSpace As Integer = Tag.IndexOf(" "c)
			Dim iCloseTag As Integer = Tag.IndexOf(">"c)

			Dim iTagTypeStart As Integer = IIf(iCloseOpenTag > -1, iCloseOpenTag + 1, iOpenTag + 1)
			Dim iTagTypeEnd As Integer = IIf(iSpace > -1, iSpace - 1, iCloseTag - 1)

			Me.TagType = Tag.Substring(iTagTypeStart, iTagTypeEnd - iTagTypeStart + 1).Replace("/", "")

			Me.Closing = iCloseOpenTag > -1

		End Sub

	End Class

#End Region

#Region "Add Log Entry"
	''' <summary>
	''' Adds new entry to the log table
	''' </summary>
	''' <param name="sMessage">The message.</param>
	''' <param name="SystemUserID">The system user identifier.</param>
	''' <param name="Request">The request.</param>
	Public Shared Sub AddLogEntry(ByVal sMessage As String, Optional ByVal SystemUserID As Integer = 0,
		Optional ByVal Request As String = "")

		Try
			SQL.Execute("insert into log (SystemUserID,Request,Message) values ({0},{1},{2})", SystemUserID,
				SQL.GetSqlValue(Request), SQL.GetSqlValue(sMessage))
		Catch ex As Exception

		End Try

	End Sub
#End Region

#Region "regex"

	''' <summary>
	''' Class containing functions to perform with regular expressions
	''' </summary>
	Public Class RegExp

		''' <summary>
		''' Returns the first match between the source and regex
		''' </summary>
		''' <param name="Source">The source.</param>
		''' <param name="RegEx">The regex.</param>
		Public Shared Function SingleCapture(ByVal Source As String, ByVal RegEx As String) As String

			Dim aCapture As New ArrayList
			If RegExp.Capture(Source, RegEx, aCapture) Then
				Return aCapture(0).ToString
			Else
				Return ""
			End If

		End Function

		''' <summary>
		''' Uses regex to find matches in the source string and replaces them with the replacement text
		''' </summary>
		''' <param name="Source">The source.</param>
		''' <param name="RegEx">The regex.</param>
		''' <param name="ReplaceText">The text to replace with.</param>
		Public Shared Function Replace(ByVal Source As String, ByVal RegEx As String, Optional ByVal ReplaceText As String = "") As String

			Dim aMatches As New ArrayList
			Dim sResult As String = Source
			If RegExp.Match(Source, RegEx, aMatches) Then

				For Each sMatch As String In aMatches
					If sMatch <> "" Then sResult = sResult.Replace(sMatch, ReplaceText)
				Next

			End If

			Return sResult

		End Function

		''' <summary>
		''' Returns true if the source matches the regex
		''' </summary>
		''' <param name="Source">The source.</param>
		''' <param name="RegEx">The regex.</param>
		''' <param name="Matches">ArrayList of matches.</param>
		Public Shared Function Match(ByVal Source As String, ByVal RegEx As String,
			ByRef Matches As ArrayList) As Boolean

			Dim oRegEx As New System.Text.RegularExpressions.Regex(RegEx)
			Dim oRegExMatches As System.Text.RegularExpressions.MatchCollection = oRegEx.Matches(Source)

			Matches = New ArrayList
			For Each oMatch As RegularExpressions.Match In oRegExMatches

				For i As Integer = 0 To oMatch.Captures.Count - 1
					Matches.Add(oMatch.Captures(i).Value.Trim)
				Next

			Next

			Return Matches.Count > 0

		End Function

		''' <summary>
		''' Checks source against regex for the first match, adds matche groups to the captures arraylist, returns true if there are any matches
		''' </summary>
		''' <param name="Source">The source.</param>
		''' <param name="RegEx">The regex.</param>
		''' <param name="Captures">The arraylist of captures.</param>
		Public Shared Function Capture(ByVal Source As String, ByVal RegEx As String,
			ByRef Captures As ArrayList) As Boolean

			Dim oRegEx As New System.Text.RegularExpressions.Regex(RegEx)
			Dim oRegExMatch As System.Text.RegularExpressions.Match = oRegEx.Match(Source)

			If oRegExMatch.Success Then

				Captures = New ArrayList
				For i As Integer = 1 To oRegExMatch.Groups.Count
					Captures.Add(oRegExMatch.Groups(i).Value.Trim)
				Next

				Return True
			Else
				Return False
			End If

		End Function

		''' <summary>
		''' Determines whether the specified source is a match to the regex.
		''' </summary>
		''' <param name="Source">The source.</param>
		''' <param name="RegEx">The regex.</param>
		Public Shared Function IsMatch(ByVal Source As String, ByVal RegEx As String) As Boolean
			Dim oRegEx As New System.Text.RegularExpressions.Regex(RegEx)
			Return oRegEx.IsMatch(Source)
		End Function

	End Class

#End Region

#Region "encrypt/decrypt"

	''' <summary>
	''' Encrypts string
	''' </summary>
	''' <param name="StringToEncrypt">The string to encrypt.</param>
	Public Shared Function Encrypt(ByVal StringToEncrypt As String) As String
		Dim oSecretKeeper As New Intuitive.Security.SecretKeeper(AppSettings.Get("EncryptionKey"))
		Return oSecretKeeper.Encrypt(StringToEncrypt)
	End Function

	''' <summary>
	''' Decrypts string
	''' </summary>
	''' <param name="StringToDecrypt">The string to decrypt.</param>
	Public Shared Function Decrypt(ByVal StringToDecrypt As String) As String
		Dim oSecretKeeper As New Intuitive.Security.SecretKeeper(AppSettings.Get("EncryptionKey"))
		Return oSecretKeeper.Decrypt(StringToDecrypt)
	End Function

	''' <summary>
	''' Checks whether specified string is encrypted.
	''' </summary>
	''' <param name="sString">The string to check.</param>
	Public Shared Function IsEncrypted(ByVal sString As String) As Boolean

		Try
			Decrypt(sString)
		Catch ex As Exception
			Return False
		End Try

		Return True

	End Function

#End Region

#Region "web support"

	Public Class Web

		''' <summary>
		''' Takes keys and their values from the querystring and adds them to a hashtable
		''' </summary>
		''' <param name="QueryString">The query string.</param>
		Public Shared Function ConvertQueryStringToHashTable(ByVal QueryString As String) As Hashtable

			Dim aPairs() As String = QueryString.Split("&"c)
			Dim oHashTable As New Hashtable
			For Each sPair As String In aPairs
				Dim aKeyValue() As String = sPair.Split("="c)
				If aKeyValue.Length = 2 AndAlso Not oHashTable.ContainsKey(aKeyValue(0)) Then
					oHashTable.Add(aKeyValue(0), HttpUtility.UrlDecode(aKeyValue(1)))
				End If
			Next

			Return oHashTable

		End Function

		''' <summary>
		''' Takes keys and their values from the querystring and adds them to a dictionary
		''' </summary>
		''' <param name="QueryString">The query string.</param>
		Public Shared Function ConvertQueryStringToDictionary(ByVal QueryString As String) As Generic.Dictionary(Of String, String)

			Dim aPairs() As String = QueryString.Split("&"c)
			Dim aList As New Generic.Dictionary(Of String, String)

			For Each sPair As String In aPairs
				Dim aKeyValue() As String = sPair.Split("="c)
				If aKeyValue.Length = 2 AndAlso Not aList.ContainsKey(aKeyValue(0)) Then
					aList.Add(aKeyValue(0), HttpUtility.UrlDecode(aKeyValue(1)))
				End If
			Next

			Return aList

		End Function

		''' <summary>
		''' Gets the value of the specified key from the querystring
		''' </summary>
		''' <param name="QueryString">The query string.</param>
		''' <param name="Key">The key to retrieve the value of.</param>
		Public Shared Function QueryStringValue(ByVal QueryString As String, ByVal Key As String) As String
			Dim oPairs As Hashtable = Web.ConvertQueryStringToHashTable(QueryString)
			If oPairs.ContainsKey(Key) Then
				Return oPairs(Key).ToString
			End If

			Return ""
		End Function

		'unbind to class
		''' <summary>
		''' Takes values from the requests form and assigns them to the corresponding fields on the specified object
		''' </summary>
		''' <param name="o">The object to set the values on.</param>
		''' <param name="oRequest">The request to get the form values from.</param>
		''' <exception cref="System.Exception">no translation for field type</exception>
		Public Shared Sub UnbindToClass(ByVal o As Object, ByVal oRequest As HttpRequest)

			Dim sValue As String
			Dim sKeyName As String

			For Each sKey As String In oRequest.Form.AllKeys

				If sKey.Length > 3 Then

					sKeyName = sKey.Substring(3)

					For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

						If sKeyName.ToLower = oField.Name.ToLower Then

							sValue = oRequest.Form(sKey)

							Select Case oField.FieldType.Name
								Case "String"
									oField.SetValue(o, sValue)
								Case "Int32"
									oField.SetValue(o, SafeInt(sValue))
								Case "DateTime"
									oField.SetValue(o, DateFunctions.SafeDate(sValue))
								Case "Double"
									oField.SetValue(o, SafeNumeric(sValue))
								Case "Decimal"
									oField.SetValue(o, CType(SafeNumeric(sValue), Decimal))
								Case "Boolean"
									oField.SetValue(o, SafeBoolean(sValue))
								Case Else
									Throw New Exception("no translation for field type " & oField.FieldType.Name)
							End Select
						End If

					Next

				End If

			Next

		End Sub

		'unbind to collection
		''' <summary>
		''' Takes values from form in given request and sets values on objects in specified collection.
		''' </summary>
		''' <param name="o">The List of objects to set the values on.</param>
		''' <param name="IDProperty">Used to identify which object from the colletion to set the values of.</param>
		''' <param name="oRequest">The request containing values.</param>
		Public Shared Sub UnbindToCollection(ByVal o As Generic.List(Of Object), ByVal IDProperty As String, ByVal oRequest As HttpRequest)
			Web.UnbindToCollection(Of Object)(o, IDProperty, oRequest)
		End Sub

		' I need to motivate core to put this in intuitive
		'unbind to collection
		''' <summary>
		''' Takes values from form in given request and sets values on objects in specified collection.
		''' </summary>
		''' <typeparam name="tObjectType">The type of object in the collection.</typeparam>
		''' <param name="o">The List of objects to set the values on.</param>
		''' <param name="IDProperty">Used to identify which object from the colletion to set the values of.</param>
		''' <param name="oRequest">The request containing values.</param>
		''' <exception cref="System.Exception">no translation for field type</exception>
		Public Shared Sub UnbindToCollection(Of tObjectType As Class)(ByVal o As Generic.List(Of tObjectType), ByVal IDProperty As String, ByVal oRequest As HttpRequest)

			Dim sValue As String
			Dim sKeyName As String
			Dim oBaseObject As Object
			Dim iID As Integer

			For Each sKey As String In oRequest.Form.AllKeys

				sKeyName = sKey.Substring(3)

				If sKeyName.IndexOf("_") > -1 Then

					iID = SafeInt(sKeyName.Split("_"c)(1))

					'see if we can get the object in the collection
					oBaseObject = Nothing
					For Each oObject As tObjectType In o
						For Each oField As System.Reflection.FieldInfo In oObject.GetType.GetFields
							If oField.Name = IDProperty AndAlso SafeInt(oField.GetValue(oObject)) = iID Then
								oBaseObject = oObject
								Exit For
							End If
						Next
						If Not oBaseObject Is Nothing Then Exit For
					Next

					'if we have any object, scan through the fields and set value if appropriate
					If Not oBaseObject Is Nothing Then

						sKeyName = sKeyName.Split("_"c)(0)

						For Each oField As System.Reflection.FieldInfo In oBaseObject.GetType.GetFields

							If sKeyName.ToLower = oField.Name.ToLower Then

								sValue = oRequest.Form(sKey)

								Select Case oField.FieldType.Name
									Case "String"
										oField.SetValue(oBaseObject, sValue)
									Case "Int32"
										oField.SetValue(oBaseObject, SafeInt(sValue))
									Case "DateTime"
										oField.SetValue(oBaseObject, DateFunctions.SafeDate(sValue))
									Case "Double"
										oField.SetValue(oBaseObject, SafeNumeric(sValue))
									Case "Decimal"
										oField.SetValue(oBaseObject, CType(SafeNumeric(sValue), Decimal))
									Case "Boolean"
										oField.SetValue(oBaseObject, SafeBoolean(sValue))
									Case Else
										Throw New Exception("no translation for field type " & oField.FieldType.Name)
								End Select
							End If

						Next

					End If

				End If

			Next

		End Sub

		'unbind to class
		''' <summary>
		''' Takes values from querystring and sets the on the specified object
		''' </summary>
		''' <param name="o">The object to set the values on.</param>
		''' <param name="QueryString">The query string to take values from.</param>
		''' <param name="bControls">Specifies whether we're taking values from controls.</param>
		''' <exception cref="System.Exception">no translation for field type</exception>
		Public Shared Sub UnbindQuerystringToClass(ByVal o As Object, ByVal QueryString As String, Optional ByVal bControls As Boolean = True)

			Dim sValue As String
			Dim sKeyName As String

			Dim oKeys As Hashtable = ConvertQueryStringToHashTable(QueryString)

			For Each sKey As DictionaryEntry In oKeys

				If bControls Then
					sKeyName = sKey.Key.ToString.Substring(3)
				Else
					sKeyName = sKey.Key.ToString
				End If

				For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

					If sKeyName.ToLower = oField.Name.ToLower Then

						sValue = sKey.Value.ToString

						Select Case oField.FieldType.Name
							Case "String"
								oField.SetValue(o, sValue)
							Case "Int32"
								oField.SetValue(o, SafeInt(sValue))
							Case "DateTime"
								oField.SetValue(o, DateFunctions.SafeDate(sValue))
							Case "Double"
								oField.SetValue(o, SafeNumeric(sValue))
							Case "Decimal"
								oField.SetValue(o, CType(SafeNumeric(sValue), Decimal))
							Case "Boolean"
								oField.SetValue(o, SafeBoolean(sValue))
							Case Else
								Throw New Exception("no translation for field type " & oField.FieldType.Name)
						End Select
					End If

				Next

			Next

		End Sub

		'Convert links to hyperlinks
		''' <summary>
		''' Converts any links starting with http:// in the specified HTML to hyperlinks
		''' </summary>
		''' <param name="HTML">The HTML.</param>
		Public Shared Function ConvertLinksToHyperlinks(ByVal HTML As String) As String
			Dim oRegExp As New System.Text.RegularExpressions.Regex("http://\S+")
			Dim oLinkProcessor As New MatchEvaluator(AddressOf ConvertLinksToHyperLinksProcess)
			Return oRegExp.Replace(HTML, oLinkProcessor)
		End Function
		''' <summary>
		''' Surrounds match with an a tag
		''' </summary>
		''' <param name="m">The match.</param>
		Private Shared Function ConvertLinksToHyperLinksProcess(ByVal m As Match) As String
			Return "<a href=""" & m.Value & """>" & m.Value & "</a>"
		End Function

	End Class

#End Region

#Region "gethash"

	''' <summary>
	''' Gets Hash for specified string
	''' </summary>
	''' <param name="s">The s.</param>
	''' <exception cref="System.Exception">max length permitted for gethash operation is 100 characters</exception>
	Public Shared Function GetHashX(ByVal s As String) As Integer

		If s.Length > 99 Then Throw New Exception("max length permitted for gethash operation is 100 characters")

		Dim aPrimes As Integer() = {
			   2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
			  31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
			  73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
			 127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
			 179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
			 233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
			 283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
			 353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
			 419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
			 467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
			 547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
			 607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
			 661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
			 739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
			 811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
			 877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
			 947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013,
			1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
			1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151,
			1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
			1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291,
			1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
			1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451,
			1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
			1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
			1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
			1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733,
			1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
			1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
			1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
			1993, 1997, 1999, 2003, 2011, 2017, 2027, 2029, 2039, 2053,
			2063, 2069, 2081, 2083, 2087, 2089, 2099, 2111, 2113, 2129,
			2131, 2137, 2141, 2143, 2153, 2161, 2179, 2203, 2207, 2213,
			2221, 2237, 2239, 2243, 2251, 2267, 2269, 2273, 2281, 2287,
			2293, 2297, 2309, 2311, 2333, 2339, 2341, 2347, 2351, 2357,
			2371, 2377, 2381, 2383, 2389, 2393, 2399, 2411, 2417, 2423,
			2437, 2441, 2447, 2459, 2467, 2473, 2477, 2503, 2521, 2531,
			2539, 2543, 2549, 2551, 2557, 2579, 2591, 2593, 2609, 2617,
			2621, 2633, 2647, 2657, 2659, 2663, 2671, 2677, 2683, 2687,
			2689, 2693, 2699, 2707, 2711, 2713, 2719, 2729, 2731, 2741
		}

		Dim aCharacters() As Char = s.ToCharArray
		Dim iReturn As Integer = 0

		For i As Integer = 0 To aCharacters.Length - 1
			iReturn += aPrimes(Convert.ToInt32(aCharacters(i))) * aPrimes(i + 255)
			iReturn = iReturn Mod 2000000000
		Next

		Return CType(iReturn, Integer)

	End Function

#End Region

#Region "Proper Case"

	''' <summary>
	''' Propers the case.
	''' </summary>
	''' <param name="sTextToformat">The s text toformat.</param>
	Public Shared Function ProperCase(ByVal sTextToformat As String) As String
		Return ProperCase(sTextToformat, False)
	End Function

	''' <summary>
	''' Converts string to proper case, capitalising the first letter of each word, also capitalising the first letter after a Mc e.g. McDonald
	''' </summary>
	''' <param name="sTextToformat">The text to format.</param>
	''' <param name="bAddFullStopAfterSingleLetter">If set to <c>true</c>, add full stop after single letter.</param>
	Public Shared Function ProperCase(ByVal sTextToformat As String, ByVal bAddFullStopAfterSingleLetter As Boolean) As String

		If String.IsNullOrEmpty(sTextToformat) Then Return sTextToformat

		Dim sPattern As String = "\w+|\W+"
		Dim sResult As String = ""
		Dim bCapitalizeNext As Boolean = True

		For Each m As Match In Regex.Matches(sTextToformat, sPattern)
			' get the matched string
			Dim x As String = m.ToString().ToLower()

			' if the first char is lower case
			If Char.IsLower(x(0)) AndAlso bCapitalizeNext Then
				' capitalize it
				x = Char.ToUpper(x(0)) + x.Substring(1, x.Length - 1)
			End If

			' Check if the word starts with Mc
			'if (x[0] == 'M' && x[1] == 'c' && !String.IsNullOrEmpty(x[2].ToString()))
			If x(0) = "M"c AndAlso x.Length > 1 AndAlso x(1) = "c"c AndAlso x.Length > 2 AndAlso Not [String].IsNullOrEmpty(x(2).ToString()) Then
				' Capitalize the letter after Mc
				x = ("Mc" & Char.ToUpper(x(2))) + x.Substring(3, x.Length - 3)
			End If

			If bCapitalizeNext = False Then
				bCapitalizeNext = True
			End If

			' if the apostrophe is at the end i.e. Andrew's
			' then do not capitalize the next letter
			If x(0).ToString() = "'" AndAlso m.NextMatch().ToString().Length = 1 Then
				bCapitalizeNext = False
			End If

			' collect all text
			sResult += x
		Next

		If (bAddFullStopAfterSingleLetter) AndAlso (sResult.Length = 1) Then
			sResult = sResult & "."
		End If

		Return sResult

	End Function

#End Region

#Region "collection functions"

	'ported from the helper functions in iVectorTest - this is something I've wanted to do several times for real, so here we are
	'tests whether two collections contain exactly the same values (irrespective of the order in each)
	''' <summary>
	''' Tests whether two collections contain exactly the same values (irrespective of the order in each)
	''' </summary>
	''' <param name="oCollection1">The first collection.</param>
	''' <param name="oCollection2">The second collection.</param>
	Public Shared Function CollectionsAreEquivalent(ByVal oCollection1 As ICollection, ByVal oCollection2 As ICollection) As Boolean

		'first, check that each collection has the same number of items
		If oCollection1.Count <> oCollection2.Count Then Return False

		'get the expected objects and their counts from the first collection
		Dim oExpecting As New Generic.Dictionary(Of Object, Integer)
		For Each oExpectedObject As Object In oCollection1

			If Not oExpecting.ContainsKey(oExpectedObject) Then
				oExpecting.Add(oExpectedObject, 1)
			Else
				oExpecting(oExpectedObject) += 1
			End If
		Next

		'check we have the same number of each in the other collection
		For Each oExpectedObject As Object In oExpecting.Keys
			Dim iActualCount As Integer = 0

			For Each oActualObject As Object In oCollection2
				If oExpectedObject.Equals(oActualObject) Then iActualCount += 1
			Next

			If iActualCount <> oExpecting(oExpectedObject) Then Return False
		Next

		Return True

	End Function

	''' <summary>
	''' Creates a generic list of integers containing values between the specified integers.
	''' </summary>
	''' <param name="iFrom">The beginning number of the range.</param>
	''' <param name="iTo">The end number of the range.</param>
	''' <param name="iStep">The amount the counter will be increased by between each number.</param>
	Public Shared Function CreateRange(ByVal iFrom As Integer, ByVal iTo As Integer, Optional ByVal iStep As Integer = 1) As Generic.List(Of Integer)
		Dim oReturn As New Generic.List(Of Integer)

		For i As Integer = iFrom To iTo Step iStep
			oReturn.Add(i)
		Next

		Return oReturn
	End Function

	''' <summary>
	''' Creates a generic list of decimal containing values between the specified decimal.
	''' </summary>
	''' <param name="nFrom">The beginning number of the range.</param>
	''' <param name="nTo">The end number of the range.</param>
	''' <param name="nStep">The amount the counter will be increased by between each number.</param>
	Public Shared Function CreateRange(ByVal nFrom As Decimal, ByVal nTo As Decimal, Optional ByVal nStep As Decimal = 1) As Generic.List(Of Decimal)
		Dim oReturn As New Generic.List(Of Decimal)

		For n As Decimal = nFrom To nTo Step nStep
			oReturn.Add(n)
		Next

		Return oReturn
	End Function

	''' <summary>
	''' Returns a new <see cref="ComparisonFunctionComparer"/> of specified type created with the specified comparison function.
	''' </summary>
	''' <typeparam name="t">The type of comparison function comparer to create.</typeparam>
	''' <param name="oComparison">The comparison function.</param>
	Public Shared Function ComparerFromComparison(Of t)(ByVal oComparison As Comparison(Of t)) As Generic.IComparer(Of t)
		Return New ComparisonFunctionComparer(Of t)(oComparison)
	End Function

	''' <summary>
	''' Creates <see cref="ComparisonFunctionComparer"/> of specified type using comparer function created with specified converter.
	''' </summary>
	''' <typeparam name="t">The type of comparison function comparer to create</typeparam>
	''' <typeparam name="tCompare">The type of the compare.</typeparam>
	''' <param name="oConvertor">The convertor.</param>
	Public Shared Function ComparerFromConvertor(Of t, tCompare)(ByVal oConvertor As Converter(Of t, tCompare)) As Generic.IComparer(Of t)
		Return New ComparisonFunctionComparer(Of t)(Function(x, y) Generic.Comparer(Of tCompare).Default.Compare(oConvertor(x), oConvertor(y)))
	End Function

#Region "reverse comparer class"

	''' <summary>
	''' Class used to compare objects of specified types, it compares the items passed to it in reverse order. 
	''' e.g. calls Compare(y,x) instead of Compare(x,y)
	''' </summary>
	''' <typeparam name="tType">The type of the objects to compare.</typeparam>
	''' <seealso cref="System.Collections.Generic.IComparer" />
	Public Class ReverseComparer(Of tType)
		Implements Generic.IComparer(Of tType)

		''' <summary>
		''' The comparer
		''' </summary>
		Public Comparer As Generic.Comparer(Of tType)

		''' <summary>
		''' Initializes a new instance of the <see cref="ReverseComparer"/> class.
		''' </summary>
		''' <param name="Comparer">The comparer.</param>
		Public Sub New(ByVal Comparer As Generic.Comparer(Of tType))
			Me.Comparer = Comparer
		End Sub

		''' <summary>
		''' Compares two objects of the ReverseComparers type, calls the compare function with the parameters reversed.
		''' </summary>
		''' <param name="x">Object to compare.</param>
		''' <param name="y">Object to compare.</param>
		Public Function Compare(ByVal x As tType, ByVal y As tType) As Integer Implements System.Collections.Generic.IComparer(Of tType).Compare
			Return Me.Comparer.Compare(y, x) 'reverse the order of the parameters
		End Function
	End Class

#End Region

#Region "comparer from comparison function class"

	''' <summary>
	''' Class used for comparing 2 objects of specified type using a comparison function
	''' </summary>
	''' <typeparam name="tType">The type of object to compare.</typeparam>
	''' <seealso cref="System.Collections.Generic.IComparer" />
	Public Class ComparisonFunctionComparer(Of tType)
		Implements Generic.IComparer(Of tType)

		''' <summary>
		''' The comparison function
		''' </summary>
		Public ComparisonFunction As Comparison(Of tType)

		''' <summary>
		''' Initializes a new instance of the <see cref="ComparisonFunctionComparer"/> class.
		''' </summary>
		''' <param name="oComparisonFunction">The comparison function.</param>
		Public Sub New(ByVal oComparisonFunction As Comparison(Of tType))
			Me.ComparisonFunction = oComparisonFunction
		End Sub

		''' <summary>
		''' Compares 2 objects using this instances comparison function.
		''' </summary>
		''' <param name="x">The x.</param>
		''' <param name="y">The y.</param>
		Public Function Compare(x As tType, y As tType) As Integer Implements System.Collections.Generic.IComparer(Of tType).Compare
			Return ComparisonFunction(x, y)
		End Function
	End Class

#End Region

#End Region

#Region "Server and IP information"
	<Obsolete("Use System.Environment.MachineName instead")>
	Public Shared Function GetServerName() As String
		Return System.Environment.MachineName
	End Function

	''' <summary>
	''' Gets the server ip address.
	''' </summary>
	Public Shared Function GetServerIPAddress() As String
		Dim oHost As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName)

		Dim sIPAddress As String = ""

		For Each oIPAddress As System.Net.IPAddress In oHost.AddressList
			'check it's an IP and not a mac address
			If oIPAddress.ToString.IndexOf(":") = -1 AndAlso oIPAddress.ToString.Length <= 15 Then
				sIPAddress = oIPAddress.ToString
				Exit For
			End If
		Next

		Return sIPAddress
	End Function

	''' <summary>
	''' Gets the client ip address.
	''' </summary>
	Public Shared Function GetClientIPAddress() As String
		Dim sClientIPAddress As String = ""

		If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing Then
			sClientIPAddress = HttpContext.Current.Request.UserHostAddress.ToString
		End If

		Return sClientIPAddress
	End Function
#End Region

#Region "Download Stream"

	''' <summary>
	''' 
	''' </summary>
	''' <seealso cref="System.IO.Stream" />
	Public Class DownloadStream
		Inherits Stream

		''' <summary>
		''' The HTTP response
		''' </summary>
		Private oHttpResponse As HttpResponse
		''' <summary>
		''' The buffer size
		''' </summary>
		Private iBufferSize As Integer
		''' <summary>
		''' The buffer array
		''' </summary>
		Private aBuffer() As Byte
		''' <summary>
		''' The used buffer
		''' </summary>
		Private iUsedBuffer As Integer
		''' <summary>
		''' The current position
		''' </summary>
		Private iCurrentPosition As Long

		''' <summary>
		''' The <see cref="DownloadStreamStatus"/>
		''' </summary>
		Private eStatus As DownloadStreamStatus
		''' <summary>
		''' The filename
		''' </summary>
		Private sFilename As String
		''' <summary>
		''' The type of content of the request
		''' </summary>
		Private sContentType As String
		''' <summary>
		''' The content length
		''' </summary>
		Private iContentLength As Long
		''' <summary>
		''' The force download
		''' </summary>
		Private bForceDownload As Boolean

		''' <summary>
		''' The response ended
		''' </summary>
		Private bResponseEnded As Boolean

#Region "Public Properties"

		''' <summary>
		''' Gets the HTTP response.
		''' </summary>
		Public ReadOnly Property HttpResponse As HttpResponse
			Get
				Return Me.oHttpResponse
			End Get
		End Property

		''' <summary>
		''' Gets the <see cref="DownloadStreamStatus"/>.
		''' </summary>
		Public ReadOnly Property Status As DownloadStreamStatus
			Get
				Return Me.eStatus
			End Get
		End Property

		''' <summary>
		''' Gets the filename.
		''' </summary>
		Public ReadOnly Property Filename As String
			Get
				Return Me.sFilename
			End Get
		End Property

		''' <summary>
		''' Gets the content type.
		''' </summary>
		Public ReadOnly Property ContentType As String
			Get
				Return Me.sContentType
			End Get
		End Property

		''' <summary>
		''' Gets the content lenth.
		''' </summary>
		Public ReadOnly Property ContentLenth As Long
			Get
				Return Me.iContentLength
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether [force download].
		''' </summary>
		Public ReadOnly Property ForceDownload As Boolean
			Get
				Return Me.bForceDownload
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether [response ended].
		''' </summary>
		Public ReadOnly Property ResponseEnded As Boolean
			Get
				Return Me.bResponseEnded
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether [data written].
		''' </summary>
		Public ReadOnly Property DataWritten As Boolean
			Get
				Return Me.eStatus = DownloadStreamStatus.HeadersWritten OrElse Me.eStatus = DownloadStreamStatus.DataWritten OrElse Me.bResponseEnded
			End Get
		End Property

#End Region

#Region "Constructor"

		''' <summary>
		''' Initializes a new instance of the <see cref="DownloadStream"/> class.
		''' </summary>
		''' <param name="HttpResponse">The HTTP response.</param>
		''' <param name="Filename">The filename.</param>
		''' <param name="ContentType">Type of the content.</param>
		''' <param name="ContentLength">Length of the content.</param>
		''' <param name="ForceDownload">if set to <c>true</c> [force download].</param>
		''' <param name="WriteResponseIfNoData">if set to <c>true</c> [write response if no data].</param>
		''' <param name="BufferSize">Size of the buffer.</param>
		Public Sub New(ByVal HttpResponse As HttpResponse, ByVal Filename As String,
				Optional ByVal ContentType As String = "application/octet-stream", Optional ByVal ContentLength As Long = 0,
				Optional ByVal ForceDownload As Boolean = False, Optional WriteResponseIfNoData As Boolean = True, Optional ByVal BufferSize As Integer = 1024)

			'set up response and buffer
			Me.oHttpResponse = HttpResponse
			Me.oHttpResponse.BufferOutput = False
			Me.iBufferSize = BufferSize
			If Me.iBufferSize <> 0 Then ReDim Me.aBuffer(Me.iBufferSize - 1)

			'set handed in params
			Me.sFilename = Filename
			Me.sContentType = ContentType
			Me.iContentLength = ContentLength
			Me.bForceDownload = ForceDownload

			'write the headers here if we can
			If WriteResponseIfNoData Then Me.WriteHeaders()

		End Sub

#End Region

#Region "Mandatory Stream Stuff"

#Region "CanRead, CanSeek, CanWrite"

		''' <summary>
		''' When overridden in a derived class, gets a value indicating whether the current stream supports reading.
		''' </summary>
		Public Overrides ReadOnly Property CanRead As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
		''' </summary>
		Public Overrides ReadOnly Property CanSeek As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' When overridden in a derived class, gets a value indicating whether the current stream supports writing.
		''' </summary>
		Public Overrides ReadOnly Property CanWrite As Boolean
			Get
				Return True
			End Get
		End Property

#End Region

#Region "Length, Position"

		''' <summary>
		''' Returns the position within the current stream.
		''' </summary>
		Public Overrides ReadOnly Property Length As Long
			Get
				Return iCurrentPosition
			End Get
		End Property

		''' <summary>
		''' When overridden in a derived class, gets or sets the position within the current stream.
		''' </summary>
		''' <exception cref="System.NotSupportedException"></exception>
		Public Overrides Property Position As Long
			Get
				Return iCurrentPosition
			End Get
			Set(ByVal value As Long)
				Throw New System.NotSupportedException(String.Format("The {0} class doesn't support setting the stream position", Me.GetType.Name))
			End Set
		End Property

#End Region

#Region "Unsupported bits"

		''' <summary>
		''' When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		''' </summary>
		''' <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
		''' <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
		''' <param name="count">The maximum number of bytes to be read from the current stream.</param>
		''' <returns>
		''' The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
		''' </returns>
		''' <exception cref="System.NotSupportedException"></exception>
		Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
			Throw New System.NotSupportedException(String.Format("The {0} class doesn't support reading from the output stream", Me.GetType.Name))
		End Function

		''' <summary>
		''' When overridden in a derived class, sets the position within the current stream.
		''' </summary>
		''' <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
		''' <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
		''' <returns>
		''' The new position within the current stream.
		''' </returns>
		''' <exception cref="System.NotSupportedException"></exception>
		Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
			Throw New System.NotSupportedException(String.Format("The {0} class doesn't support seeking", Me.GetType.Name))
		End Function

		''' <summary>
		''' When overridden in a derived class, sets the length of the current stream.
		''' </summary>
		''' <param name="value">The desired length of the current stream in bytes.</param>
		''' <exception cref="System.NotSupportedException"></exception>
		Public Overrides Sub SetLength(ByVal value As Long)
			Throw New System.NotSupportedException(String.Format("The {0} class doesn't support setting the stream length", Me.GetType.Name))
		End Sub

#End Region

#End Region

#Region "Write Headers"

		''' <summary>
		''' Writes the headers to this instances HttpResponse.
		''' </summary>
		Private Sub WriteHeaders()

			If Me.bForceDownload Then
				Me.oHttpResponse.AddHeader("Content-Type", "application/force-download")
				Me.oHttpResponse.AddHeader("Content-Type", "application/octet-stream")
			Else
				Me.oHttpResponse.AddHeader("Content-Type", Me.sContentType)
			End If
			Me.oHttpResponse.AddHeader("Content-Disposition", String.Format("attachment; filename=""{0}""", Me.sFilename))
			If Me.iContentLength <> 0 Then Me.oHttpResponse.AddHeader("Content-Length", Me.iContentLength.ToString)

			Me.eStatus = DownloadStreamStatus.HeadersWritten

		End Sub

#End Region

#Region "Write, Flush"

		''' <summary>
		''' When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		''' </summary>
		''' <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
		''' <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		''' <param name="count">The number of bytes to be written to the current stream.</param>
		''' <exception cref="System.ObjectDisposedException"></exception>
		Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)

			If Me.eStatus = DownloadStreamStatus.Ready Then
				Me.WriteHeaders()
			ElseIf Me.eStatus = DownloadStreamStatus.Closed Then
				Throw New ObjectDisposedException(Me.GetType.Name, String.Format("This instance of {0} is already closed", Me.GetType.Name))
			End If

			If Me.iBufferSize <> 0 Then
				Dim iAmountToCopy As Integer

				Do While count <> 0 AndAlso Me.oHttpResponse.IsClientConnected

					iAmountToCopy = Min(count, Me.iBufferSize - Me.iUsedBuffer)
					Array.ConstrainedCopy(buffer, offset, Me.aBuffer, iUsedBuffer, iAmountToCopy)

					Me.iUsedBuffer += iAmountToCopy
					Me.iCurrentPosition += iAmountToCopy
					offset += iAmountToCopy
					count -= iAmountToCopy

					If Me.iUsedBuffer = Me.iBufferSize Then
						Me.oHttpResponse.OutputStream.Write(Me.aBuffer, 0, Me.iBufferSize)
						iUsedBuffer = 0
					End If
				Loop
			ElseIf Me.oHttpResponse.IsClientConnected Then
				Me.oHttpResponse.OutputStream.Write(buffer, offset, count)
			End If

			If Me.eStatus = DownloadStreamStatus.HeadersWritten Then Me.eStatus = DownloadStreamStatus.DataWritten

		End Sub

		''' <summary>
		''' When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		''' </summary>
		''' <exception cref="System.ObjectDisposedException"></exception>
		Public Overrides Sub Flush()
			If Me.eStatus = DownloadStreamStatus.Closed Then Throw New ObjectDisposedException(Me.GetType.Name, String.Format("This instance of {0} is already closed", Me.GetType.Name))

			If Me.iUsedBuffer <> 0 Then
				Me.oHttpResponse.OutputStream.Write(Me.aBuffer, 0, Me.iUsedBuffer)
				Me.iUsedBuffer = 0
			End If

			Me.oHttpResponse.OutputStream.Flush()
		End Sub

#End Region

#Region "Close"

		''' <summary>
		''' Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.
		''' </summary>
		Public Overrides Sub Close()
			Me.Flush()

			If Me.eStatus <> DownloadStreamStatus.Ready Then
				Me.oHttpResponse.End()
				Me.bResponseEnded = True
			End If

			Me.eStatus = DownloadStreamStatus.Closed
		End Sub

#End Region

#Region "Send Data"

		''' <summary>
		''' Writes data to current output stream.
		''' </summary>
		''' <param name="oStream">The data stream to send.</param>
		''' <exception cref="System.ObjectDisposedException"></exception>
		''' <exception cref="System.IO.InternalBufferOverflowException"></exception>
		Public Sub SendData(ByVal oStream As Stream)

			If Me.eStatus = DownloadStreamStatus.Ready Then
				Me.WriteHeaders()
			ElseIf Me.eStatus = DownloadStreamStatus.Closed Then
				Throw New ObjectDisposedException(Me.GetType.Name, String.Format("This instance of {0} is already closed", Me.GetType.Name))
			End If

			If Me.iBufferSize <> 0 Then
				Dim iBytesRead As Integer

				Do While Me.oHttpResponse.IsClientConnected
					iBytesRead = oStream.Read(Me.aBuffer, Me.iUsedBuffer, Me.iBufferSize - Me.iUsedBuffer)

					If iBytesRead <> 0 Then
						Me.iUsedBuffer += iBytesRead
						Me.iCurrentPosition += iBytesRead

						If Me.iUsedBuffer = Me.iBufferSize Then
							Me.oHttpResponse.OutputStream.Write(Me.aBuffer, 0, Me.iBufferSize)
							Me.iUsedBuffer = 0
						End If
					Else
						Exit Do
					End If
				Loop
			Else
				Throw New System.IO.InternalBufferOverflowException(String.Format("This instance of {0} was created without a buffer, so is unable to read from the input stream", Me.GetType.Name))
			End If

			If Me.eStatus = DownloadStreamStatus.HeadersWritten Then Me.eStatus = DownloadStreamStatus.DataWritten

		End Sub

#End Region

#Region "Send File Or URL"

		''' <summary>
		''' Sends the file data to specified url
		''' </summary>
		''' <param name="HttpResponse">The HTTP response.</param>
		''' <param name="sFilePathOrURL">The s file path or URL.</param>
		''' <param name="bForceDownload">Set to true to specify a forced download.</param>
		Public Shared Sub SendFileOrURL(ByVal HttpResponse As HttpResponse, ByVal sFilePathOrURL As String, Optional bForceDownload As Boolean = False)

			Dim oWebClient As System.Net.WebClient = Nothing
			Dim oFileStream As Stream
			If sFilePathOrURL.StartsWith("http") Then
				oWebClient = New System.Net.WebClient
				oFileStream = oWebClient.OpenRead(sFilePathOrURL)
			Else
				oFileStream = New FileStream(sFilePathOrURL, FileMode.Open, FileAccess.Read, FileShare.Read)
			End If

			Dim oDownloadStream As New DownloadStream(HttpResponse, Path.GetFileName(sFilePathOrURL),
				FileFunctions.ConvertFileExtensionToMIMEType(Path.GetExtension(sFilePathOrURL)), New FileInfo(sFilePathOrURL).Length, bForceDownload)
			oDownloadStream.SendData(oFileStream)

			oFileStream.Dispose()
			If oWebClient IsNot Nothing Then oWebClient.Dispose()
			oDownloadStream.Close()

		End Sub

#End Region

#Region "Status Enum"

		''' <summary>
		''' Enum for possible Download Stream Statuses
		''' </summary>
		Public Enum DownloadStreamStatus
			Ready
			HeadersWritten
			DataWritten
			Closed
		End Enum

#End Region

	End Class

#End Region

	''' <summary>
	''' Converts an local path to relative URL.
	''' </summary>
	''' <param name="LocalPath">The local path.</param>
	Public Shared Function ConvertAbsoluteLocalPathToRelativeURL(ByVal LocalPath As String) As String
		LocalPath = LocalPath.ToLower.ToLower
		LocalPath = LocalPath.Replace(HttpContext.Current.Server.MapPath("/").ToLower, "")
		LocalPath = LocalPath.Replace("\", "/")
		LocalPath = "/" & LocalPath

		Return LocalPath

	End Function
End Class
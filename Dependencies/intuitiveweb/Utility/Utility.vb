Imports System.Xml
Imports ivci = iVectorConnectInterface
Imports System.Web.HttpUtility
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Configuration.ConfigurationManager
Imports Intuitive.Net

Public Class Utility

#Region "DictionaryKeyValuePair"

	Public Class DictionaryKeyValuePair
		'encode
		Public Shared Function Encode(ByVal Fields As Generic.Dictionary(Of String, Object)) As String
			Dim sb As New System.Text.StringBuilder
			For Each oField As KeyValuePair(Of String, Object) In Fields
				sb.Append(oField.Key).Append("=").Append(oField.Value.ToString).Append("&")
			Next
			Return Intuitive.Functions.Chop(sb.ToString)
		End Function

		'decode
		Public Shared Function Decode(ByVal KeyValuePairs As String, Optional URLDecode As Boolean = False) As Generic.Dictionary(Of String, Object)

			Dim oDictionary As New Generic.Dictionary(Of String, Object)
			Dim aPairs As String() = KeyValuePairs.Split("&"c)
			For Each sPair As String In aPairs
				Dim aBits() As String = sPair.Split("="c)
				If URLDecode Then
					oDictionary.Add(HttpUtility.UrlDecode(aBits(0)), HttpUtility.UrlDecode(aBits(1)))
				Else
					oDictionary.Add(aBits(0), aBits(1))
				End If

			Next

			Return oDictionary
		End Function
	End Class

#End Region

#Region "text overrides xml"

	Public Shared Function GetTextOverridesXML(ByVal sOverrides As String, Optional ByVal sNodeName As String = "") As XmlDocument

		Dim oOverridesXML As New XmlDocument

		If sNodeName = "" Then
			XMLFunctions.AddXMLNode(oOverridesXML, "", "TextOverrides", "")
		Else
			XMLFunctions.AddXMLNode(oOverridesXML, "", sNodeName, "")
		End If

		Try

			'split overrides into array
			Dim aOverrides As String() = sOverrides.Split("#"c)

			'loop through each override and add a new node for each
			For Each sOverride As String In aOverrides

				Dim sID As String = sOverride.Split("|"c)(0)
				Dim sValue As String = sOverride.Split("|"c)(1)

				If sNodeName = "" Then
					XMLFunctions.AddXMLNode(oOverridesXML, "TextOverrides", sID, sValue)
				Else
					XMLFunctions.AddXMLNode(oOverridesXML, sNodeName, sID, sValue)
				End If

			Next

		Catch ex As Exception

		End Try

		Return oOverridesXML
	End Function

#End Region

#Region "XML to generic list"

	Public Shared Function XMLToGenericList(Of T)(ByVal oXML As XmlDocument) As Generic.List(Of T)

		Dim oList As New Generic.List(Of T)

		Try
			If oXML.ChildNodes.Count > 0 Then
				Dim oNodeList As XmlNodeList = oXML.ChildNodes(0).ChildNodes
				For Each oNode As XmlNode In oNodeList
					Dim oT As T = CType(Serializer.DeSerialize(GetType(T), oNode.OuterXml), T)
					oList.Add(oT)
				Next
			End If
		Catch ex As Exception
		End Try

		Return oList
	End Function

    Public Shared Function XMLToGenericList(Of T)(ByVal oXML As XmlDocument, ByVal XPath As String) As Generic.List(Of T)

        Dim oList As New Generic.List(Of T)

		Try
			If oXML.SelectSingleNode(XPath) IsNot Nothing Then
				Dim oNodeList As XmlNodeList = oXML.SelectNodes(XPath)
				For Each oNode As XmlNode In oNodeList
					Dim oT As T = CType(Serializer.DeSerialize(GetType(T), oNode.OuterXml), T)
					oList.Add(oT)
				Next
			End If
		Catch ex As Exception
		End Try

		Return oList
	End Function

    Public Shared Function InnerXMLToGenericList(Of T)(ByVal oXML As XmlDocument, ByVal XPath As String) As Generic.List(Of T)

        Dim oList As New Generic.List(Of T)

		Try
			If oXML.SelectSingleNode(XPath) IsNot Nothing Then
				Dim oNodeList As XmlNodeList = oXML.SelectNodes(XPath)
				For Each oNode As XmlNode In oNodeList
                    Dim oT As T = Microsoft.VisualBasic.CTypeDynamic(Of T)(oNode.InnerXml)
					oList.Add(oT)
				Next
			End If
		Catch ex As Exception
		End Try

		Return oList
	End Function

#End Region

#Region "copy library widgets"

	Public Shared Sub CopyLibraryWidgets(ByVal ApplicationFolder As String, ByVal iVectorWidgetPath As String)

		'Variable to hold the location of where the widgets will eventually go.
		Dim sLocalPath As String = ApplicationFolder & "WidgetsLibrary\"

		'1.Get the directory that the widget library widgets live in, and get all directories within there.
		Dim oDirectory As DirectoryInfo = New DirectoryInfo(iVectorWidgetPath & "Widgets\")
		Dim aDirectories() As DirectoryInfo = oDirectory.GetDirectories("*.*", IO.SearchOption.TopDirectoryOnly)

		'2a. Loop through our array of directories.
		For Each aDirectory As DirectoryInfo In aDirectories

			'2b.Check if the directory exists within the project, if it does not, create it.
			Dim sFolderPath As String = sLocalPath & aDirectory.Name
			If Not System.IO.File.Exists(sFolderPath) Then
				Intuitive.FileFunctions.CreateFolder(sFolderPath)
			End If

			'3. Get a collection of all files within the directory, and then loop through them.
			Dim aFiles() As String = System.IO.Directory.GetFiles(aDirectory.FullName)

			For Each sFile As String In aFiles
				'We only care about CSS,JS,HTML or ASCX files, when we find one, copy it over.
				If sFile.EndsWith(".css") OrElse sFile.EndsWith(".js") OrElse sFile.EndsWith(".htm") OrElse sFile.EndsWith(".ascx") Then
					copyLibraryWidget(sFile, aDirectory, sLocalPath)
				End If
			Next

		Next

		'4. copy scripts
		CopyLibraryScripts(ApplicationFolder, iVectorWidgetPath, "Script")
		CopyLibraryScripts(ApplicationFolder, iVectorWidgetPath, "Script\LanguageScripts")

		'5 copy services
		CopyServices(ApplicationFolder, iVectorWidgetPath)
	End Sub

	Public Shared Sub CopyServices(ByVal ApplicationFolder As String, ByVal iVectorWidgetPath As String)

		Dim sServicesPath As String = iVectorWidgetPath & "Services"
		Dim aServices() As String = System.IO.Directory.GetFiles(sServicesPath)

		Dim sLocalServicesPath As String = ApplicationFolder & "Services"

		For Each sFile As String In aServices.Where(Function(o) o.EndsWith(".ashx"))

			Dim sLocalFilePath As String = sLocalServicesPath & "\" & New System.IO.FileInfo(sFile).Name

			If System.IO.File.Exists(sLocalFilePath) Then
				Dim oReadOnlyFile As IO.FileInfo = New System.IO.FileInfo(sLocalFilePath)
				oReadOnlyFile.IsReadOnly = False
				System.IO.File.Delete(sLocalFilePath)
			End If

			System.IO.File.Copy(sFile, sLocalFilePath, True)

		Next
	End Sub

	Public Shared Sub copyLibraryWidget(File As String, Directory As DirectoryInfo, ByVal LocalPath As String)

		'1.Build up file path of where we will be moving the file to in the project. 
		Dim sLocalFilePath As String = LocalPath & Directory.Name & "\" & New System.IO.FileInfo(File).Name

		'3.If it exists already, we want to delete it ready for hte new file.
		If System.IO.File.Exists(sLocalFilePath) Then
			Dim oReadOnlyFile As IO.FileInfo = New System.IO.FileInfo(sLocalFilePath)
			oReadOnlyFile.IsReadOnly = False
			System.IO.File.Delete(sLocalFilePath)
		End If

		'4.Copy over the file
		System.IO.File.Copy(File, sLocalFilePath, True)

	End Sub

	Public Shared Sub CopyLibraryScripts(ByVal ApplicationFolder As String, ByVal iVectorWidgetPath As String, ByVal iVectorScriptPath As String)

		Dim sScriptPath As String = iVectorWidgetPath & iVectorScriptPath
		Dim aScriptFiles() As String = System.IO.Directory.GetFiles(sScriptPath)

		Dim sLocalScriptsPath As String = ApplicationFolder & iVectorScriptPath

        'If the directory does not exist make one
	    If Not Directory.Exists(sLocalScriptsPath) Then
	        Directory.CreateDirectory(sLocalScriptsPath)
	    End If

		For Each sFile As String In aScriptFiles

			Dim sLocalFilePath As String = sLocalScriptsPath & "\" & New System.IO.FileInfo(sFile).Name

			If System.IO.File.Exists(sLocalFilePath) Then
				Dim oReadOnlyFile As IO.FileInfo = New System.IO.FileInfo(sLocalFilePath)
				oReadOnlyFile.IsReadOnly = False
				System.IO.File.Delete(sLocalFilePath)
			End If

			System.IO.File.Copy(sFile, sLocalFilePath, True)

		Next
	End Sub

#End Region

#Region "URLToXML"

	Public Shared Function BookingSettingsXML(ByVal CacheMinutes As Integer) As XmlDocument
		Dim sURL As String = String.Format(BookingBase.Params.ServiceURL & "cms/{0}/{1}" & "?languageid={2}&cmswebsiteid={3}",
			"BookingSettings",
			1,
			0,
			0)

		Return URLToXML(sURL, CacheMinutes)
	End Function

	Public Shared Function BigCXML(ByVal URL As String, ByVal CacheMinutes As Integer) As XmlDocument

		Dim sURL As String = String.Format(URL & "?languageid={0}&cmswebsiteid={1}",
			BookingBase.DisplayLanguageID,
			BookingBase.Params.CMSWebsiteID)

		'Big C calls should not even take 10 secs but I am being cautious to start
		Return URLToXML(sURL, CacheMinutes, Timeout:=10)
	End Function

	Public Shared Function BigCXML(ByVal ObjectType As String, ByVal ID As Integer, ByVal CacheMinutes As Integer,
			  Optional Languageid As Integer = -1, Optional CMSWebsiteID As Integer = -1) As XmlDocument

		Dim iLanguageID As Integer = Intuitive.Functions.IIf(Languageid > -1, Languageid, BookingBase.DisplayLanguageID)
		Dim iCMSWebsiteID As Integer = Intuitive.Functions.IIf(CMSWebsiteID > -1, CMSWebsiteID, BookingBase.Params.CMSWebsiteID)


		Dim sURL As String = String.Format(BookingBase.Params.ServiceURL & "cms/{0}/{1}" & "?languageid={2}&cmswebsiteid={3}",
			ObjectType,
			ID,
			iLanguageID,
			iCMSWebsiteID)

		'Big C calls should not even take 10 secs but I am being cautious to start
		Return URLToXML(sURL, CacheMinutes, Timeout:=10)
	End Function

	Public Shared Function URLToXML(ByVal URL As String,
									ByVal CacheMinutes As Integer,
									Optional ByVal CreateLog As Boolean = False,
									Optional Timeout As Integer = 100) As XmlDocument

		Dim oXML As New XmlDocument
		oXML = Intuitive.Functions.GetCache(Of XmlDocument)(URL)

		If oXML Is Nothing Then

			Try

				oXML = SendiVCRequest(URL, CacheMinutes, CreateLog, Timeout)

			Catch ex As Exception
				oXML = New XmlDocument
				oXML.LoadXml("<Error>" & ex.Message & "</Error>")
				FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Error", oXML.InnerXml)
			End Try

		End If

		Return oXML
	End Function

	Public Shared Function BigCXML(ByVal ObjectType As String, ByVal ID As Integer, ByVal CacheMinutes As Integer, ByVal IgnoreCachedXML As Boolean,
	 Optional Languageid As Integer = -1, Optional CMSWebsiteID As Integer = -1) As XmlDocument

		Dim iLanguageID As Integer = Intuitive.Functions.IIf(Languageid > -1, Languageid, BookingBase.DisplayLanguageID)
		Dim iCMSWebsiteID As Integer = Intuitive.Functions.IIf(CMSWebsiteID > -1, CMSWebsiteID, BookingBase.Params.CMSWebsiteID)


		Dim sURL As String = String.Format(BookingBase.Params.ServiceURL & "cms/{0}/{1}" & "?languageid={2}&cmswebsiteid={3}",
			ObjectType,
			ID,
			iLanguageID,
			iCMSWebsiteID)

		'Big C calls should not even take 10 secs but I am being cautious to start
		Return URLToXML(sURL, IgnoreCachedXML, CacheMinutes, Timeout:=10)
	End Function

	Public Shared Function URLToXML(ByVal URL As String,
									ByVal IgnoreCachedXML As Boolean,
									ByVal CacheMinutes As Integer,
									Optional ByVal CreateLog As Boolean = False,
									Optional Timeout As Integer = 100) As XmlDocument

		Dim oXML As New XmlDocument
		oXML = Intuitive.Functions.GetCache(Of XmlDocument)(URL)

		If oXML Is Nothing OrElse IgnoreCachedXML Then

			Try

				oXML = SendiVCRequest(URL, CacheMinutes, CreateLog, Timeout)

			Catch ex As Exception
				oXML = New XmlDocument
				oXML.LoadXml("<Error>" & ex.Message & "</Error>")
				FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Error", oXML.InnerXml)
			End Try

		End If

		Return oXML
	End Function

	Private Shared Function SendiVCRequest(ByVal URL As String,
										   ByVal CacheMinutes As Integer,
										   ByVal CreateLog As Boolean,
										   ByVal Timeout As Integer) As XmlDocument

		Dim oXML As New XmlDocument
		Dim oRequest As New Intuitive.Net.WebRequests.Request

		With oRequest
			.EndPoint = URL
			.SOAP = False
			.ContentType = Intuitive.Net.WebRequests.ContentType.Application_x_www_form_urlencoded
			.Method = Net.WebRequests.eRequestMethod.GET
			.UseGZip = True
			.TimeoutInSeconds = Timeout
		End With

		Dim watch As Stopwatch = Stopwatch.StartNew()
		oRequest.Send()
		watch.Stop()
		Dim elapsedMs As Long = watch.ElapsedMilliseconds

		oXML = oRequest.ResponseXML

		'at least let someone know these calls are slow
		If oRequest.TimeOut Then
			Utility.EmailTimeouts(URL, elapsedMs, oRequest.ResponseXML)
		End If

		If CreateLog Then
			If URL.ToLower.Contains("lookups") AndAlso Not oRequest.ResponseXML.InnerXml.Contains("<Lookups>") Then
				FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Request", oRequest.RequestXML.InnerXml)
				FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Response", oRequest.ResponseXML.InnerXml)
			End If
		End If

		If oRequest.ResponseString.Contains("Error") Then
			FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Error", oRequest.ResponseLog)
		End If

		If CacheMinutes > 0 AndAlso Not oXML.InnerXml = "" AndAlso Not oXML.InnerXml = "<Error></Error>" Then
			Intuitive.Functions.AddToCache(URL, oXML, CacheMinutes)
		End If

		'this function gets called from inside of a thread
		'so don't try and log anything to the session
		If Not HttpContext.Current Is Nothing Then
			Logging.Current.AddIVCLog("iVC / " & URL, elapsedMs, oXML)
		End If

		Return oXML
	End Function

	Private Shared Sub EmailTimeouts(type As String, time As Double, response As XmlDocument)

		Dim oEmail As New Email
		Dim sb As New StringBuilder

		sb.AppendFormatLine("{0} timed out after took {1}ms/{2}seconds", type, time.ToString(), (time / 1000).ToString())

		With oEmail
			.SMTPHost = AppSettings("SMTPHost")
			.EmailTo = AppSettings("SystemEmail")
			.From = "iVectorConnect Logging"
			.FromEmail = "connect-logging@intuitivesystems.co.uk"
			.Subject = "iVector Connect Logging for " & type
			.Body = sb.ToString.Replace("\n", ControlChars.NewLine)

			Dim oResponse As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(response.InnerXml))
			.StreamAttachments.Add("Response.xml", oResponse)

			.SendEmail()

		End With
	End Sub

#End Region

#Region "IVC Send Request"

	Public Class iVectorConnect
		Private Shared Function RequestObjectToLogName(ByVal Request As Object) As String
			If Not Request Is Nothing Then
				Return Request.GetType.ToString.Replace(".", "").Replace("Request", "")
			Else
				Return "Unknown"
			End If
		End Function

		Public Shared Function SendRequest(Of ResponseType As Class)(ByVal Request As Object, Optional ByVal CreateLog As Boolean = False,
		   Optional ByVal TimeOutSeconds As Integer = 100, Optional ByVal Headers As Intuitive.Net.WebRequests.RequestHeaders = Nothing, Optional ByVal OverrideServiceURL As String = "") As iVectorConnectReturn
			Return SendRequest(Of ResponseType)(Request, "", CreateLog, TimeOutSeconds, Headers, OverrideServiceURL)
		End Function

		Public Shared Function SendRequest(Of ResponseType As Class)(ByVal Request As Object, ByVal EmailSearchTimesAddress As String,
		   Optional ByVal CreateLog As Boolean = False, Optional ByVal TimeOutSeconds As Integer = 100,
		   Optional ByVal Headers As Intuitive.Net.WebRequests.RequestHeaders = Nothing, Optional ByVal OverrideServiceURL As String = "") As iVectorConnectReturn


			'0. work out logname
			Dim sCallName As String = Utility.iVectorConnect.RequestObjectToLogName(Request)


			'1. Deserialise the class
			Dim oRequestXML As XmlDocument = Serializer.Serialize(Request)

			Dim oNode As XmlNode = oRequestXML.CreateNode(XmlNodeType.Element, "Diagnostics", "")
			oNode.InnerText = "true"
			oRequestXML.DocumentElement.AppendChild(oNode)

			'2. Send the request, if we have an override service url use that if not use the standard one
			Dim sEndPoint As String =
					IIf(OverrideServiceURL <> "",
						OverrideServiceURL & "ivectorconnect.ashx",
						BookingBase.Params.ServiceURL & "ivectorconnect.ashx").ToString

			If Not EmailSearchTimesAddress = "" Then sEndPoint += "?emailsearchtimes=" & EmailSearchTimesAddress

			Dim oWebRequest As New Intuitive.Net.WebRequests.Request
			With oWebRequest
				.EndPoint = sEndPoint
				.SOAP = False
				.ContentType = Intuitive.Net.WebRequests.ContentType.Application_x_www_form_urlencoded
				If Headers IsNot Nothing Then .Headers.AddRange(Headers)
				.Source = sCallName
				.LogFileName = sCallName & " Request"
				.CreateLog = Functions.IIf(CreateLog, CreateLog, BookingBase.Params.Log)
				.UseGZip = True
				.SetRequest(oRequestXML)
				.TimeoutInSeconds = TimeOutSeconds
			End With

			Dim watch As Stopwatch = Stopwatch.StartNew()
			oWebRequest.Send()
			watch.Stop()
			Dim elapsedMs As Long = watch.ElapsedMilliseconds

			Dim oResponseXML As New XmlDocument
			oResponseXML = oWebRequest.ResponseXML

			'3. build response
			'if no errors then deserialize
			Dim iVectorConnectReturn As New iVectorConnectReturn
			If XMLFunctions.SafeNodeValue(oResponseXML, "//ReturnStatus/Success").ToSafeBoolean Then
				iVectorConnectReturn.Success = True
				iVectorConnectReturn.ReturnObject = Serializer.DeSerialize(Of ResponseType)(oWebRequest.ResponseXML.OuterXml)
			Else

				iVectorConnectReturn.Success = False

				If XMLFunctions.SafeNodeValue(oResponseXML, "//ReturnStatus/Exceptions/Exception") <> "" Then
					iVectorConnectReturn.Warning.Add(XMLFunctions.SafeNodeValue(oResponseXML, "//ReturnStatus/Exceptions/Exception"))
                Try
					iVectorConnectReturn.ReturnObject = Serializer.DeSerialize(Of ResponseType)(oWebRequest.ResponseXML.OuterXml)
			        Catch ex As Exception
                        FileFunctions.AddLogEntry("iVectorConnect/SendRequest", "Return DeSerialize Error", ex.Message)
			        End Try
				Else

					'Added this bit also in the event that no data was returned from the remote server.
					Dim sResponseParentNode As String = GetResponseParentNode(oRequestXML)
					If sResponseParentNode <> "" Then
						Dim xmlNoResultStr As String =
								String.Format(
									"<{0}><ReturnStatus><Success>false</Success><Exceptions><Exception>Response Not Returned.</Exception></Exceptions></ReturnStatus></{0}>",
									sResponseParentNode)
						iVectorConnectReturn.ReturnObject = Serializer.DeSerialize(Of ResponseType)(xmlNoResultStr)
					End If

				End If

			End If

			Dim dRequestTicks As Double = 0
			Dim dRequestMs As Double = 0
			Dim dNetworkLatencyMs As Double = 0

			dRequestTicks = Intuitive.ToSafeDecimal(CType(XMLFunctions.SafeNodeValue(oResponseXML,
				"//ReturnStatus/Diagnostics/Difference"),
				Double))

			If dRequestTicks <> 0 Then
				dRequestMs = dRequestTicks / TimeSpan.TicksPerMillisecond
				dNetworkLatencyMs = elapsedMs - dRequestMs
			End If

			iVectorConnectReturn.RequestTime = elapsedMs
			iVectorConnectReturn.RequestXML = oRequestXML
			iVectorConnectReturn.ResponseXML = oResponseXML
			iVectorConnectReturn.NetworkLatency = dNetworkLatencyMs
			iVectorConnectReturn.Timeout = oWebRequest.TimeOut

			Return iVectorConnectReturn
		End Function

		Public Shared Function GetResponseParentNode(ByVal RequestXML As XmlDocument) As String

			If RequestXML.FirstChild IsNot Nothing Then

				If RequestXML.FirstChild.Name.Contains("Request") Then
					Return RequestXML.FirstChild.Name.Replace("Request", "Response")
				ElseIf RequestXML.FirstChild.NextSibling IsNot Nothing AndAlso RequestXML.FirstChild.NextSibling.Name.Contains("Request") Then
					Return RequestXML.FirstChild.NextSibling.Name.Replace("Request", "Response")
				End If

			End If

			Return ""
		End Function

		Public Class iVectorConnectReturn
			Public Success As Boolean = False
			Public Warning As New Generic.List(Of String)
			Public ReturnObject As Object
			Public RequestXML As XmlDocument
			Public ResponseXML As XmlDocument
			Public RequestTime As Double
			Public NetworkLatency As Double
			Public Timeout As Boolean
		End Class
	End Class

#End Region

#Region "Custom queries"

	Public Shared Function CustomQueryXML(ByVal CustomQuery As String, ByVal ParamArray Params() As String) As XmlDocument

		'Check if we have override login details set up for custom queries, regrettably this could be needed
		'because custom queries are broken for aggregate log ins, and rather that fix connect we've been asked to 
		'work around it.
		Dim sLogin As String =
				IIf(BookingBase.Params.CustomQueryOverrideiVCLogin = "",
					BookingBase.IVCLoginDetails.Login,
					BookingBase.Params.CustomQueryOverrideiVCLogin).ToString
		Dim sPassword As String =
				IIf(BookingBase.Params.CustomQueryOverrideiVCPassword = "",
					BookingBase.IVCLoginDetails.Password,
					BookingBase.Params.CustomQueryOverrideiVCPassword).ToString

		Dim oLoginDetails As New LoginDetails

		With oLoginDetails
			.Login = sLogin
			.Password = sPassword
			End With

		Return CustomQueryXML(CustomQuery, BookingBase.Params.ServiceURL, oLoginDetails, Params)

	End Function

	Public Shared Function CustomQueryXML(ByVal CustomQuery As String,
										  ByVal ServiceURL As String,
										  ByVal LoginDetails As LoginDetails,
										  ByVal ParamArray Params() As String) As XmlDocument

		Dim oCustomQueryRequest As New CustomQueryRequest

		With oCustomQueryRequest

			.LoginDetails = LoginDetails
			.QueryName = CustomQuery

			For i As Integer = 1 To Params.Count
				Dim oField As FieldInfo = .Parameters.GetType.GetField("Param" & i)
				If oField IsNot Nothing Then
					oField.SetValue(.Parameters, Params(i - 1))
				End If
			Next

			End With

		Dim oReturn As iVectorConnect.iVectorConnectReturn = iVectorConnect.SendRequest(Of CustomQueryResponse)(
			oCustomQueryRequest, OverrideServiceURL:=ServiceURL)

		Return oReturn.ResponseXML

	End Function

#End Region

#Region "Get third party settings"

	Public Shared Function GetThirdPartySettings(ByVal ThirdParty As String) As Generic.Dictionary(Of String, String)

		Dim oSettings As New Generic.Dictionary(Of String, String)

		'Set up the request
		Dim oGetThirdPartySettingsRequest As New ivci.GetThirdPartySettingsRequest
		oGetThirdPartySettingsRequest.LoginDetails = BookingBase.IVCLoginDetails
		oGetThirdPartySettingsRequest.ThirdParty = ThirdParty

		'Get the response
		Dim oivcReturn As iVectorConnect.iVectorConnectReturn =
		  iVectorConnect.SendRequest(Of ivci.GetThirdPartySettingsResponse)(oGetThirdPartySettingsRequest)


		'If it is successful add all the settings to the returning dictionary
		If oivcReturn.Success Then

			Dim oGetThirdPartySettingsResponse As ivci.GetThirdPartySettingsResponse =
			   CType(oivcReturn.ReturnObject, ivci.GetThirdPartySettingsResponse)

			For Each oThirdPartySetting As ivci.GetThirdPartySettingsResponse.ThirdPartySetting In oGetThirdPartySettingsResponse.ThirdPartySettings
				oSettings.Add(oThirdPartySetting.Name, oThirdPartySetting.Value)
			Next

		End If

		Return oSettings
	End Function

#End Region

#Region "hasher class (for component hashes)"

	Public Class Hasher
		Private sHashToken As String = ""

		Public Overridable Property HashToken As String
			Get
				If sHashToken = "" Then
					sHashToken = "xx"
					sHashToken = Me.GenerateHashToken()
				End If
				Return sHashToken
			End Get
			Set(value As String)
			End Set
		End Property

		Public Overridable Function GenerateHashToken() As String
			Return Functions.Encrypt(Serializer.Serialize(Me, True).InnerXml)
		End Function

		Public Shared Function DeHashToken(Of tObjectType As Class)(ByVal HashToken As String) As tObjectType

			Dim sXML As String = ""
			Try
				sXML = Functions.Decrypt(HashToken)
				Return CType(Serializer.DeSerialize(GetType(tObjectType), sXML), tObjectType)
			Catch ex As Exception
				Dim sDecodedHashToken As String = UrlDecode(HashToken)
				sXML = Functions.Decrypt(sDecodedHashToken)
				Return CType(Serializer.DeSerialize(GetType(tObjectType), sXML), tObjectType)
			End Try
		End Function
	End Class

#End Region

#Region "Anonymous Type to JSON"

	Public Shared Function SerializeAnonymousTypeToJSON(ByVal o As Object) As String

		'build up the name/value pairs
		Dim aPairs As New Generic.Dictionary(Of String, String)
		Dim sValue As String = ""
		Dim oValue As Object

		'for each field
		For Each oField As System.Reflection.PropertyInfo In o.GetType.GetProperties

			oValue = oField.GetValue(o, Nothing)
			sValue = ""
			If Not oValue Is Nothing Then
				Select Case oField.PropertyType.Name
					Case "String"
						sValue = oValue.ToString
					Case "Int32"
						sValue = oValue.ToString
					Case "DateTime"
						sValue = Intuitive.DateFunctions.DisplayDate(oValue.ToString)
					Case "Double", "Decimal"
						sValue = oValue.ToString
					Case "Boolean"
						sValue = oValue.ToString
					Case Else
						sValue = "**Ignore"
				End Select
			End If

			If sValue <> "**Ignore" Then
				sValue = """" & sValue.Replace("\", "\\").Replace("""", "\""").Replace("'", "\'").Replace(ControlChars.NewLine, "") &
						 """"
				aPairs.Add(oField.Name, sValue)
			End If

		Next

		'build up the json string
		Dim sb As New System.Text.StringBuilder
		sb.Append("{")

		For Each oPair As Generic.KeyValuePair(Of String, String) In aPairs
			sb.Append("""").Append(oPair.Key).Append(""" : ").Append(oPair.Value).Append(",")
		Next

		sb.Chop()

		sb.Append("}")

		Return sb.ToString
	End Function

#End Region

#Region "Process timer"

	Friend Class ProcessTimerSupport
		Public Const ApplicationName As String = "IntuitiveWeb"

		Friend Class ProcessStep
			Public Const BookingSearch As String = "Booking Search"
			Public Const PropertySearch As String = "Property Search"
			Public Const FlightSearch As String = "Flight Search"
			Public Const FlightItinerary As String = "Flight Itinerary"
			Public Const SendingRequestToiVectorConnect As String = "Sending Request To iVectorConnect"
			Public Const SaveSearchDetailsToSession As String = "Save search details to session"
			Public Const SetEmailLogsTo As String = "Set email logs to"
			Public Const StartFlightAndHotelSearch As String = "Start flight and hotel search"
			Public Const StartFlightSearch As String = "Start flight search"
			Public Const StartHotelSearch As String = "Start hotel search"
			Public Const FlightCarousel As String = "Flight carousel"
			Public Const WaitForThreads As String = "Wait for threads"
			Public Const SaveFlights As String = "Save flight results"
			Public Const SaveHotels As String = "Save property results"
			Public Const SetIVCRequestDetails As String = "Set iVectorConnect request details"
		End Class
	End Class

#End Region
End Class

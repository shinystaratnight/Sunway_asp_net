Imports System.Collections.Generic
Imports System.Text
Imports System.IO.Compression
Imports System.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Threading
Imports Intuitive.Functions
Imports System.IO
Imports System.Xml
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.HttpUtility

Namespace Net

	Public Class WebRequests

		''' <summary>
		''' Web exchange request or response.
		''' </summary>
		Public Enum WebExchange

			''' <summary>
			''' The request.
			''' </summary>
			Request

			''' <summary>
			''' The response.
			''' </summary>
			Response

		End Enum

#Region "Request functions"

#Region "Make Request"

        ''' <summary>
        ''' Takes and processes a WebRequest
        ''' </summary>
        ''' <param name="WebRequest">The web request.</param>
		Public Shared Sub MakeRequest(ByVal WebRequest As Request)

			Try

				'log: start time
				WebRequest.SetProcessStartTime()

				'0. validate
				If Not WebRequest.Validate Then
					WebRequest.SetProcessFinishedTime()
					Return
				End If

				'1. start the timer and log
				Dim oReader As StreamReader

				'2. set up the stream if we're doing a post or put request
				If InList(WebRequest.Method, eRequestMethod.POST, eRequestMethod.PUT) Then

					'log: get request stream time
					WebRequest.SetGetRequestStreamStartTime()

					'3. write the xml onto the stream
					Dim oRequestStream As Stream = WebRequest.HTTPWebRequest.GetRequestStream
					Dim oBytes() As Byte = WebRequest.ResponseEncoding.GetBytes(WebRequest.RequestString)
					oRequestStream.Write(oBytes, 0, oBytes.Length)
					oRequestStream.Close()

					'log: get request stream time
					WebRequest.SetGetRequestStreamFinishedTime()

				End If

				'log: get response time
				WebRequest.SetRequestStartTime()

				'4. get the response
				Dim oResponseStream As Stream = Nothing
				WebRequest.SaveRequestLog()
				WebRequest.HTTPWebResponse = CType(WebRequest.HTTPWebRequest.GetResponse(), HttpWebResponse)
				oResponseStream = WebRequest.HTTPWebResponse.GetResponseStream

				'log: get response time
				WebRequest.SetRequestReceivedTime()

				'log: read response time
				WebRequest.SetReadResponseStreamStartTime()

				'5. un zip
				If WebRequest.HTTPWebResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
					oResponseStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oResponseStream)
					oReader = New StreamReader(oResponseStream, WebRequest.ResponseEncoding)
				Else
					oReader = New StreamReader(oResponseStream, WebRequest.ResponseEncoding)
				End If

				Dim sResponse As String
				sResponse = oReader.ReadToEnd
				oResponseStream.Close()

				'log: read response time
				WebRequest.SetReadResponseStreamFinishedTime()

				'6. load the response onto our class
				WebRequest.SetProcessFinishedTime()
				WebRequest.SetResponse(sResponse)
				WebRequest.SaveResponseLog()

				'7. close everything
				If WebRequest.HTTPWebResponse IsNot Nothing Then
					WebRequest.HTTPWebResponse.Close()
				End If
				If oReader IsNot Nothing Then
					oReader.Close()
				End If

			Catch ex As WebException

				'8. try to get as many details as we can
				Dim oError As New RequestError
				With oError
					.Text = ex.ToString
					.Status = ex.Status.ToString

					If ex.Response IsNot Nothing Then
						Dim oErrorResponse As WebResponse = ex.Response
						Dim oStream As Stream = oErrorResponse.GetResponseStream

						If oStream IsNot Nothing AndAlso oStream.CanRead Then

							Dim oExceptionReader As StreamReader

							'9.a un zip
							If oErrorResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
								oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oStream)
								oExceptionReader = New StreamReader(oStream, Encoding.Default)
							Else
								oExceptionReader = New StreamReader(oStream, True)
							End If

							.Response = oExceptionReader.ReadToEnd
							oExceptionReader.Close()

						End If

						oErrorResponse.Close()

					End If

				End With

				'9. log the error
				WebRequest.SetRequestReceivedTime()
				WebRequest.SetProcessFinishedTime()
				WebRequest.RequestError = oError
				WebRequest.SaveResponseLog()

			End Try

			'10. Process the response
			If WebRequest.ReturnFunction IsNot Nothing Then
				WebRequest.ReturnFunction(WebRequest)
			End If

		End Sub

#End Region

#Region "AsynchronousRequests"

        ''' <summary>
        ''' Takes a WebRequest and handles it asynchronously
        ''' </summary>
        ''' <param name="WebRequest">The web request.</param>
		Public Shared Sub MakeAsynchronousRequest(ByVal WebRequest As Request)

			Try

				'0. validate
				If Not WebRequest.Validate Then
					Return
				End If

				'1. start the timer
				' WebRequest.SetStartTime()

				'2. set up the stream if we're doing a post request
				If WebRequest.Method = eRequestMethod.POST Then

					'3. get the request stream for posts
					WebRequest.HTTPWebRequest.BeginGetRequestStream(AddressOf RequestStreamCallBack, WebRequest)
				Else

					'3. otherwise just send off the request
					WebRequest.SaveRequestLog()
					WebRequest.HTTPWebRequest.BeginGetResponse(AddressOf ReadResponse, WebRequest)
				End If

			Catch ex As Exception
			End Try

		End Sub

        ''' <summary>
        ''' Callback to handle results from asynch request
        ''' </summary>
        ''' <param name="oResult">The result.</param>
		Private Shared Sub RequestStreamCallBack(ByVal oResult As IAsyncResult)

			Dim oStream As Stream = Nothing
			Dim oHTTPRequest As HttpWebRequest
			Dim oRequest As Request = CType(oResult.AsyncState, Request)

			Try

				'1. get the request stream
				oHTTPRequest = oRequest.HTTPWebRequest
				oStream = oHTTPRequest.EndGetRequestStream(oResult)

				'2. write the request to the stream
				Dim oWriter As New StreamWriter(oStream)
				oWriter.Write(oRequest.RequestString)
				oWriter.Close()

				'3. send off the request
				oRequest.SaveRequestLog()
				oHTTPRequest.BeginGetResponse(AddressOf ReadResponse, oRequest)
				oStream.Close()

			Catch ex As WebException

				'4. try to get as many details as we can
				Dim oError As New RequestError
				With oError
					.Text = ex.ToString
					.Status = ex.Status.ToString

					If ex.Response IsNot Nothing Then
						Dim oErrorResponse As WebResponse = ex.Response
						Dim oErrorStream As Stream = oErrorResponse.GetResponseStream
						Dim oExceptionReader As StreamReader

						'4.a un zip
						If oErrorResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
							oErrorStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oErrorStream)
							oExceptionReader = New StreamReader(oErrorStream, Encoding.Default)
						Else
							oExceptionReader = New StreamReader(oErrorStream, True)
						End If

						.Response = oExceptionReader.ReadToEnd
						oExceptionReader.Close()
						oErrorResponse.Close()
					End If

				End With

				'5. log the error
				'oRequest.SetReceivedTime()
				oRequest.RequestError = oError
				oRequest.SaveResponseLog()

				'6. Process the response
				If oRequest.ReturnFunction IsNot Nothing Then
					oRequest.ReturnFunction(oRequest)
				End If

			End Try

		End Sub

        ''' <summary>
        ''' Reads the response.
        ''' </summary>
        ''' <param name="oResult">The result.</param>
		Private Shared Sub ReadResponse(oResult As IAsyncResult)

			' Dim oHTTPResponse As HttpWebResponse
			Dim oRequest As New Request
			Dim oReader As StreamReader
			Dim oStream As Stream = Nothing

			Try

				'1. get the response
				oRequest = CType(oResult.AsyncState, Request)
				oRequest.HTTPWebResponse = CType(oRequest.HTTPWebRequest.EndGetResponse(oResult), HttpWebResponse)
				oRequest.HTTPWebRequest.Timeout = oRequest.HTTPWebRequest.Timeout
				oStream = oRequest.HTTPWebResponse.GetResponseStream

				'2. un zip
				If oRequest.HTTPWebResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
					oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oStream)
					oReader = New StreamReader(oStream, Encoding.Default)
				Else
					oReader = New StreamReader(oStream, True)
				End If

				'2a. Close the Stream
				oStream.Close()

				'3. load the response onto our class
				Dim sResponse As String = oReader.ReadToEnd
				oRequest.SetResponse(sResponse)
				' oRequest.SetReceivedTime()
				oRequest.SaveResponseLog()

				'4. close everything
				If oRequest.HTTPWebResponse IsNot Nothing Then
					oRequest.HTTPWebResponse.Close()
				End If
				If oReader IsNot Nothing Then
					oReader.Close()
				End If

			Catch ex As WebException

				'5. try to get as many details as we can
				Dim oError As New RequestError
				With oError
					.Text = ex.ToString
					.Status = ex.Status.ToString

					If ex.Response IsNot Nothing Then
						Dim oErrorResponse As WebResponse = ex.Response
						Dim oErrorStream As Stream = oErrorResponse.GetResponseStream
						Dim oExceptionReader As StreamReader

						'5.a un zip
						If oErrorResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
							oErrorStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oErrorStream)
							oExceptionReader = New StreamReader(oErrorStream, Encoding.Default)
						Else
							oExceptionReader = New StreamReader(oErrorStream, True)
						End If

						.Response = oExceptionReader.ReadToEnd
						oExceptionReader.Close()
						oErrorResponse.Close()
					End If

				End With

				'6. log the error
				'oRequest.SetReceivedTime()
				oRequest.RequestError = oError
				oRequest.SaveResponseLog()

			End Try

			'7. Process the response
			If oRequest.ReturnFunction IsNot Nothing Then
				oRequest.ReturnFunction(oRequest)
			End If

		End Sub

#End Region

#Region "Old requests"

        ''' <summary>
        ''' Send an HTTP request and handle the response
        ''' </summary>
        ''' <param name="URL">The URL to send the request to.</param>
        ''' <param name="XML">The XML.</param>
        ''' <param name="bSecure">if set to <c>true</c>, https.</param>
        ''' <param name="Param">The parameter.</param>
        ''' <param name="ContentType">Type of the content.</param>
        ''' <param name="AuthenticationMode">The authentication mode.</param>
        ''' <param name="AuthUsername">The authentication username.</param>
        ''' <param name="AuthPassword">The authentication password.</param>
        ''' <param name="AddDefaultCredentials">If set to <c>true</c> add default credentials to http request.</param>
        ''' <param name="TimeoutInSeconds">The timeout in seconds.</param>
        ''' <param name="oHeaders">The headers to add to the request.</param>
        ''' <exception cref="System.Exception">Empty response returned</exception>
		<Obsolete("This method is obsolete, use the Request class instead")>
		Public Shared Function GetResponse(ByVal URL As String, ByVal XML As String,
		  Optional ByVal bSecure As Boolean = False, Optional ByVal Param As String = "Data",
		  Optional ByVal ContentType As String = "application/x-www-form-urlencoded",
		  Optional ByVal AuthenticationMode As String = "none", Optional ByVal AuthUsername As String = "",
		  Optional ByVal AuthPassword As String = "", Optional ByVal AddDefaultCredentials As Boolean = False,
		  Optional ByVal TimeoutInSeconds As Integer = 100, Optional ByVal oHeaders As RequestHeaders = Nothing) As String

			Try

				Dim oHTTPRequest As HttpWebRequest
				Dim oStream As Stream = Nothing

				'secure up the url
				If bSecure Then
					URL = URL.Replace("http:", "https:")
				End If

				'standard header stuff
				oHTTPRequest = CType(HttpWebRequest.Create(URL), HttpWebRequest)
				oHTTPRequest.ContentType = ContentType
				oHTTPRequest.KeepAlive = False
				oHTTPRequest.Timeout = TimeoutInSeconds * 1000

				'header authorisation
				If AuthenticationMode = "Basic" Then
					oHTTPRequest.Headers.Add(HttpRequestHeader.Authorization,
					 "Basic " + Convert.ToBase64String(New ASCIIEncoding().GetBytes(AuthUsername.ToString & ":" & AuthPassword.ToString)))
				End If

				If AddDefaultCredentials Then oHTTPRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
				oHTTPRequest.Method = "POST"

				'any custom headers?
				If oHeaders IsNot Nothing Then
					For Each oRequestHeader As RequestHeader In oHeaders
						oHTTPRequest.Headers.Add(oRequestHeader.Name, oRequestHeader.Value)
					Next
				End If

				If Param <> "" Then
					XML = Param & "=" & System.Web.HttpUtility.UrlEncode(XML)
				End If

				oStream = oHTTPRequest.GetRequestStream
				Dim oWriter As New StreamWriter(oStream)
				oWriter.Write(XML)
				oWriter.Close()

				'get response
				Dim oResponse As WebResponse = oHTTPRequest.GetResponse()
				oStream = oResponse.GetResponseStream

				Dim oReader As StreamReader
				If oResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then

					'.net comes with some classes for dealing with gzip but these appear to give us trouble when we have a large number of requests
					'I have added SharpZipLib to see if this works any better
					'PG 12/01/2012
					'oStream = New System.IO.Compression.GZipStream(oStream, Compression.CompressionMode.Decompress)
					oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oStream)
					oReader = New StreamReader(oStream, Encoding.Default)
				Else
					oReader = New StreamReader(oStream, True)
				End If

				Dim sResponse As String = oReader.ReadToEnd

				If sResponse = "" Then
					Throw New Exception("Empty response returned")
				Else
					Return sResponse
				End If

			Catch ex As WebException

				Dim sErrorResponse As String = ""
				Dim sReturn As String = "<Error>Unknown error</Error>"

				Try

					'grab the error response if we can
					If ex.Response IsNot Nothing Then
						Dim oErrorResponse As WebResponse = ex.Response
						Dim oStream As Stream = oErrorResponse.GetResponseStream
						Dim oReader As StreamReader = New StreamReader(oStream, True)
						sErrorResponse = oReader.ReadToEnd
						sReturn = sErrorResponse
					ElseIf ex.Status.ToString.ToLower = "timeout" Then
						sReturn = ("<Error>timeout</Error>")

					Else
						sErrorResponse = ex.ToString
						sReturn = String.Format("<Error><![CDATA[{0}]]></Error>", sErrorResponse)
					End If

					Dim oSBError As New StringBuilder

					With oSBError
						.Append("URL: ").AppendLine(URL)
						.Append("Secure: ").AppendLine(bSecure.ToString)
						.Append("Param: ").AppendLine(Param)
						.Append("Content type: ").AppendLine(ContentType)
						.Append("Auth Mode: ").AppendLine(AuthenticationMode)
						.Append("Auth Password: ").AppendLine(AuthPassword)
						.Append("Add Default Credentials: ").AppendLine(AddDefaultCredentials.ToString)
						.Append("Timeout: ").AppendLine(TimeoutInSeconds.ToString)
						.AppendLine.AppendLine.AppendLine(XML.ToString)
						.AppendLine.AppendLine.AppendLine(sErrorResponse)
					End With

					Intuitive.FileFunctions.AddLogEntry("WebRequest", "Request Failed : " & URL, oSBError.ToString)

				Catch InnerEx As Exception
				End Try

				Return sReturn

			End Try

		End Function


        ''' <summary>
        ''' Send a SOAP request and handle the response
        ''' </summary>
        ''' <param name="URL">The URL to send the request to.</param>
        ''' <param name="SOAPAction">The SOAP action.</param>
        ''' <param name="XML">The XML.</param>
        ''' <param name="ContentType">Content type of the request.</param>
        ''' <param name="oHeaders">The headers to add to the request.</param>
        ''' <param name="TimeoutInSeconds">The timeout in seconds.</param>
        ''' <exception cref="System.Exception">Empty response returned</exception>
		<Obsolete("This method is obsolete, use the Request class instead")>
		Public Shared Function GetSOAPResponse(ByVal URL As String, ByVal SOAPAction As String, ByVal XML As String,
		   Optional ByVal ContentType As String = "text/xml; charset=utf-8", Optional ByVal oHeaders As RequestHeaders = Nothing,
		 Optional ByVal TimeoutInSeconds As Integer = 100) As String

			Try

				Dim oHTTPRequest As HttpWebRequest
				Dim oStream As Stream

				'standard header stuff
				oHTTPRequest = CType(HttpWebRequest.Create(URL), HttpWebRequest)

				Dim dBuffer As Byte() = System.Text.Encoding.UTF8.GetBytes(XML)
				oHTTPRequest.Timeout = TimeoutInSeconds * 1000
				oHTTPRequest.KeepAlive = False
				oHTTPRequest.UserAgent = "Mozilla/4.0 (compatible;MSIE 6.0b;Windows NT 5.0)"
				oHTTPRequest.AllowAutoRedirect = True
				oHTTPRequest.AllowWriteStreamBuffering = True

				oHTTPRequest.ContentType = ContentType

				With oHTTPRequest.Headers
					.Add(RequestHeaderName.Action, SOAPAction)
					.Add(RequestHeaderName.SoapAction, SOAPAction)
				End With

				oHTTPRequest.Method = "POST"
				oHTTPRequest.ContentLength = dBuffer.Length
				oHTTPRequest.Accept = "text/xml"

				'any custom headers?
				If oHeaders IsNot Nothing Then
					For Each oRequestHeader As RequestHeader In oHeaders
						oHTTPRequest.Headers.Add(oRequestHeader.Name, oRequestHeader.Value)
					Next
				End If

				oStream = oHTTPRequest.GetRequestStream

				'Dim oWriter As New StreamWriter(oStream)
				oStream.Write(dBuffer, 0, dBuffer.Length)
				oStream.Close()

				'get response
				Dim oResponse As WebResponse

				oResponse = oHTTPRequest.GetResponse()
				oStream = oResponse.GetResponseStream

				Dim oReader As StreamReader
				If oResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
					'.net comes with some classes for dealing with gzip but these appear to give us trouble when we have a large number of requests
					'I have added SharpZipLib to see if this works any better
					'PG 12/01/2012
					'oStream = New System.IO.Compression.GZipStream(oStream, Compression.CompressionMode.Decompress)
					oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oStream)
					oReader = New StreamReader(oStream, Encoding.Default)
				Else
					oReader = New StreamReader(oStream, True)
				End If

				Dim sResponse As String = oReader.ReadToEnd

				If sResponse = "" Then
					Throw New Exception("Empty response returned")
				Else
					Return sResponse
				End If

			Catch ex As WebException

				Dim sErrorResponse As String = ""

				Try
					'grab the error response if we can
					If ex.Response IsNot Nothing Then
						Dim oErrorResponse As WebResponse = ex.Response
						Dim oStream As Stream = oErrorResponse.GetResponseStream
						Dim oReader As StreamReader = New StreamReader(oStream, True)
						sErrorResponse = oReader.ReadToEnd
					ElseIf ex.Status.ToString.ToLower = "timeout" Then
						sErrorResponse = ("<Error>timeout</Error>")
					Else
						sErrorResponse = ex.ToString
					End If
				Catch InnerEx As Exception
				End Try

				Intuitive.FileFunctions.AddLogEntry("WebRequest", "Request Failed : " & URL, sErrorResponse)
				Return sErrorResponse
			End Try

		End Function


        ''' <summary>
        ''' Sends SOAP Request with UTF32 encoding
        ''' </summary>
        ''' <param name="URL">The URL to send the request to.</param>
        ''' <param name="SOAPAction">The SOAP action.</param>
        ''' <param name="XML">The XML.</param>
        ''' <param name="ContentType">Content type of the request.</param>
		<Obsolete("This method is obsolete, use the Request class instead")>
		Public Shared Function GetUTF32SOAPResponse(ByVal URL As String, ByVal SOAPAction As String, ByVal XML As String,
		   Optional ByVal ContentType As String = "text/xml; charset=utf-32") As String

			Dim oHTTPRequest As HttpWebRequest
			Dim oStream As Stream

			'standard header stuff
			oHTTPRequest = CType(HttpWebRequest.Create(URL), HttpWebRequest)

			Dim dBuffer As Byte() = System.Text.Encoding.UTF32.GetBytes(XML)
			oHTTPRequest.Timeout = 60000
			oHTTPRequest.KeepAlive = False
			oHTTPRequest.UserAgent = "Mozilla/4.0 (compatible;MSIE 6.0b;Windows NT 5.0)"
			oHTTPRequest.AllowAutoRedirect = True
			oHTTPRequest.AllowWriteStreamBuffering = True

			oHTTPRequest.ContentType = ContentType

			With oHTTPRequest.Headers
				.Add(RequestHeaderName.Action, SOAPAction)
				.Add(RequestHeaderName.SoapAction, SOAPAction)
			End With

			oHTTPRequest.Method = "POST"
			oHTTPRequest.ContentLength = dBuffer.Length
			oHTTPRequest.Accept = "text/xml"

			oStream = oHTTPRequest.GetRequestStream

			'Dim oWriter As New StreamWriter(oStream)
			oStream.Write(dBuffer, 0, dBuffer.Length)
			oStream.Close()

			'get response
			Try
				Dim oResponse As WebResponse = oHTTPRequest.GetResponse()
				oStream = oResponse.GetResponseStream
			Catch ex As Exception
				Return ""
			End Try

			Dim oReader As StreamReader = New StreamReader(oStream)
			Return oReader.ReadToEnd

		End Function


        ''' <summary>
        ''' Sends specified file to the specified URL
        ''' </summary>
        ''' <param name="URL">The URI path to upload the file to.</param>
        ''' <param name="Filename">The file to send.</param>
		Public Shared Function SendFile(ByVal URL As String, ByVal Filename As String) As String

			Try

				Dim oWebClient As New System.Net.WebClient
				oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials

				Dim oResponse As Byte()
				oResponse = oWebClient.UploadFile(URL, Filename)

				Return Encoding.ASCII.GetString(oResponse)

			Catch ex As Exception
				Debug.Print("error: " & ex.Message)

				Return "-1"
			End Try

		End Function

        ''' <summary>
        ''' Downloads file from the specified URL.
        ''' </summary>
        ''' <param name="URL">The URI to download from.</param>
        ''' <param name="FileLocation">The local file to save the file to.</param>
		Public Shared Function DownloadURLToFile(ByVal URL As String, ByVal FileLocation As String) As Boolean

			Try
				Dim oWebClient As New System.Net.WebClient
				oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials
				oWebClient.DownloadFile(URL, FileLocation)
				Return True
			Catch ex As Exception
				Return False
			End Try

		End Function

        ''' <summary>
        ''' Downloads file from specified url, returns exception if thrown.
        ''' </summary>
        ''' <param name="URL">The URL of the file to download.</param>
        ''' <param name="FileLocation">Where to save the file to.</param>
        ''' <returns></returns>
		Public Shared Function DownloadURLToFileWithException(ByVal URL As String, ByVal FileLocation As String) As Exception

			Try
				Dim oWebClient As New System.Net.WebClient
				oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials
				oWebClient.DownloadFile(URL, FileLocation)
				Return Nothing
			Catch ex As Exception
				Return ex
			End Try

		End Function

        ''' <summary>
        ''' Sends inner Xml of XmlDocument to specified URL.
        ''' </summary>
        ''' <param name="URL">The URL.</param>
        ''' <param name="XML">The XML.</param>
		Public Shared Function SendXML(ByVal URL As String, ByVal XML As XmlDocument) As String

			Dim oWebClient As New System.Net.WebClient
			oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials

			Dim oResponse As Byte()
			oResponse = oWebClient.UploadData(URL, Encoding.ASCII.GetBytes(XML.InnerXml))

			Return Encoding.ASCII.GetString(oResponse)

		End Function

        ''' <summary>
        ''' Sends the specified XML to the specified URL.
        ''' </summary>
        ''' <param name="URL">The URL.</param>
        ''' <param name="XML">The XML.</param>
		Public Shared Function SendXML(ByVal URL As String, ByVal XML As String) As String

			Dim oWebClient As New System.Net.WebClient
			oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials

			Dim oResponse As Byte()
			oResponse = oWebClient.UploadData(URL, Encoding.ASCII.GetBytes(XML))

			Return Encoding.ASCII.GetString(oResponse)

		End Function

        ''' <summary>
        ''' Sends the inner Xml of the XmlDocument to the specified URL with ISO-8859-1 encoding.
        ''' </summary>
        ''' <param name="URL">The URL to send the xml to.</param>
        ''' <param name="XML">The XML to send.</param>
		Public Shared Function SendXMLWithISOEncoding(ByVal URL As String, ByVal XML As XmlDocument) As String

			Dim oWebClient As New System.Net.WebClient
			oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials

			Dim oResponse As Byte()
			oResponse = oWebClient.UploadData(URL, Encoding.GetEncoding("ISO-8859-1").GetBytes(XML.InnerXml))

			Return Encoding.ASCII.GetString(oResponse)

		End Function

        ''' <summary>
        ''' Sends the XML to the specified URL with ISO-8859-1 encoding.
        ''' </summary>
        ''' <param name="URL">The URL to send the xml to.</param>
        ''' <param name="XML">The XML to send.</param>
		Public Shared Function SendXMLWithISOEncoding(ByVal URL As String, ByVal XML As String) As String

			Dim oWebClient As New System.Net.WebClient
			oWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials

			Dim oResponse As Byte()
			oResponse = oWebClient.UploadData(URL, Encoding.GetEncoding("ISO-8859-1").GetBytes(XML))

			Return Encoding.ASCII.GetString(oResponse)

		End Function


        ''' <summary>
        ''' Sends request to specified URL
        ''' </summary>
        ''' <param name="sURL">The URL for the request.</param>
        ''' <param name="oHeaders">The headers to add to the request.</param>
        ''' <param name="TimeoutInSeconds">The timeout in seconds.</param>
		<Obsolete("This method is obsolete")>
		Public Shared Function GetURL(ByVal sURL As String, Optional ByVal oHeaders As RequestHeaders = Nothing,
				 Optional ByVal TimeoutInSeconds As Integer = 100) As String

			Dim oRequest As System.Net.WebRequest
			Dim oResponse As System.Net.WebResponse
			Dim oStream As System.IO.Stream
			Dim oReader As System.IO.StreamReader

			oRequest = System.Net.WebRequest.Create(sURL)
			oRequest.Timeout = TimeoutInSeconds * 1000

			'any custom headers?
			If oHeaders IsNot Nothing Then
				For Each oRequestHeader As RequestHeader In oHeaders
					oRequest.Headers.Add(oRequestHeader.Name, oRequestHeader.Value)
				Next
			End If

			oRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
			oResponse = oRequest.GetResponse()

			If oResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
				oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oResponse.GetResponseStream())
				oReader = New StreamReader(oStream, Encoding.Default)
			Else
				oReader = New StreamReader(oResponse.GetResponseStream(), True)
			End If

			Dim sResult As String = oReader.ReadToEnd

			oReader.Close()
			oResponse.Close()

			Return sResult

		End Function


        ''' <summary>
        ''' Sends request to specified URL using basic HTTP Authentication
        ''' </summary>
        ''' <param name="URL">The URL for the request.</param>
        ''' <param name="bSecure">If set to <c>true</c> use https.</param>
        ''' <param name="ContentType">Content type of the URL.</param>
        ''' <param name="AuthenticationMode">The authentication mode.</param>
        ''' <param name="AuthUsername">The authentication username.</param>
        ''' <param name="AuthPassword">The authentication password.</param>
        ''' <param name="TimeoutInSeconds">The timeout in seconds.</param>
        ''' <exception cref="System.Exception">Empty response returned</exception>
		<Obsolete("This method is obsolete")>
		Public Shared Function GetURLWithCredentials(ByVal URL As String,
		  Optional ByVal bSecure As Boolean = False, Optional ByVal ContentType As String = "application/x-www-form-urlencoded",
		  Optional ByVal AuthenticationMode As String = "none", Optional ByVal AuthUsername As String = "",
		  Optional ByVal AuthPassword As String = "", Optional ByVal TimeoutInSeconds As Integer = 100) As String

			Try

				Dim oHTTPRequest As HttpWebRequest
				Dim oStream As Stream = Nothing

				'secure up the url
				If bSecure Then
					URL = URL.Replace("http:", "https:")
				End If

				'standard header stuff
				oHTTPRequest = CType(HttpWebRequest.Create(URL), HttpWebRequest)
				oHTTPRequest.ContentType = ContentType
				oHTTPRequest.KeepAlive = False
				oHTTPRequest.Timeout = TimeoutInSeconds * 1000

				'header authorisation
				If AuthenticationMode = "Basic" Then
					oHTTPRequest.Headers.Add(HttpRequestHeader.Authorization,
					 "Basic " + Convert.ToBase64String(New ASCIIEncoding().GetBytes(AuthUsername.ToString & ":" & AuthPassword.ToString)))
				End If

				'get response
				Dim oResponse As WebResponse = oHTTPRequest.GetResponse()
				oStream = oResponse.GetResponseStream

				Dim oReader As New StreamReader(oStream, True)
				Dim sResponse As String = oReader.ReadToEnd

				If sResponse = "" Then
					Throw New Exception("Empty response returned")
				Else
					Return sResponse
				End If

			Catch ex As WebException

				Intuitive.FileFunctions.AddLogEntry("WebRequest", "Request Failed : " & URL, ex.ToString)
				Return ex.ToString
			End Try

		End Function

        ''' <summary>
        ''' Sends request and returns response as an XmlDocument using default encoding.
        ''' </summary>
        ''' <param name="sURL">The s URL.</param>
		<Obsolete("This method is obsolete")>
		Public Shared Function GetXML(ByVal sURL As String) As XmlDocument
			Return Net.WebRequests.GetXML(sURL, Encoding.Default)
		End Function

        ''' <summary>
        ''' Sends request and converts response to an XmlDocument using specified encoding.
        ''' </summary>
        ''' <param name="sURL">The URL.</param>
        ''' <param name="oEncoding">The character encoding to use.</param>
        ''' <param name="iTimeoutInMilliSeconds">The timeout in milliseconds.</param>
		<Obsolete("This method is obsolete")>
		Public Shared Function GetXML(ByVal sURL As String, ByVal oEncoding As Encoding, Optional ByVal iTimeoutInMilliSeconds As Integer = 100000) As XmlDocument

			Dim oRequest As System.Net.WebRequest
			Dim oResponse As System.Net.WebResponse

			oRequest = System.Net.WebRequest.Create(sURL)
			oRequest.Timeout = iTimeoutInMilliSeconds
			oRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
			oResponse = oRequest.GetResponse()

			Dim oResponseStream As System.IO.Stream = CType(oResponse, HttpWebResponse).GetResponseStream()
			Dim oXML As New XmlDocument
			Dim oReader As XmlReader = XmlReader.Create(New StreamReader(oResponseStream, oEncoding))
			oXML.Load(oReader)

			oReader.Close()
			oResponse.Close()

			Return oXML

		End Function

#End Region

#End Region

#Region "Support Classes"

        ''' <summary>
        ''' Class representing a request
        ''' </summary>
		Public Class Request
            Implements IRequest 

#Region "Public Properties"

            Public Sub New()
				Me.Method = WebRequests.eRequestMethod.POST
                Me.TimeoutInSeconds = 100
				Me.UseGZip = False
				Me.UseDeflate = False
                Me.UserAgent = ""
                Me.Accept = ""
				Me.ContentType = WebRequests.ContentType.Text_xml
                Me.SoapAction  = ""
				Me.EndPoint = ""
				Me.Headers = New RequestHeaders()
				Me.RequestError = New RequestError()
				Me.Param = ""
				Me.UserName = ""
                Me.AuthenticationMode = eAuthenticationMode.None
				Me.Password = ""
                Me.Source = "Unspecified"
				Me.LogFileName = "Unspecified"
				Me.CreateLog = False
				Me.CreateErrorLog = True
				Me.DataMasking = New Generic.List(Of DataMaskDef)
				Me.ExtraInfo = Nothing
				Me.ResponseHolder = New Generic.List(Of Object)
                Me.SOAP = False
				Me.RequestCookies = New CookieCollection()
				Me.SuppressExpectHeaders = False
				Me.ClientCertificates = New ClientCertificates()
				Me.KeepAlive = False
            End Sub

            ''' <summary>
            ''' Gets or sets the method for the request.
            ''' </summary>
			Public Property Method As eRequestMethod Implements IRequest.Method 
     
            ''' <summary>
            ''' Gets or sets the timeout in seconds for the request.
            ''' </summary>
			Public Property TimeoutInSeconds As Integer Implements IRequest.TimeoutInSeconds
            ''' <summary>
            ''' Gets or sets a value indicating whether to receive the response gzipped
            ''' </summary>
			Public Property UseGZip As Boolean Implements IRequest.UseGZip

			'Public Property Success As Boolean = False

            ''' <summary>
            ''' Gets or sets a value indicating whether to use deflate.
            ''' </summary>
			Public Property UseDeflate As Boolean Implements IRequest.UseDeflate
            ''' <summary>
            ''' Gets or sets the value of the user-agent header.
            ''' </summary>
			Public Property UserAgent As String Implements IRequest.UserAgent 
            ''' <summary>
            ''' Gets or sets the value of the accept http header.
            ''' </summary>
			Public Property Accept As String Implements IRequest.Accept
            ''' <summary>
            ''' Gets or sets the value of the content-type http header.
            ''' </summary>
			Public Property ContentType As String Implements IRequest.ContentType
            ''' <summary>
            ''' Gets or sets the SOAP action.
            ''' </summary>
			Public Property SoapAction As String Implements IRequest.SoapAction
            ''' <summary>
            ''' Gets or sets the end point for the request.
            ''' </summary>
			Public Property EndPoint As String Implements IRequest.EndPoint
            ''' <summary>
            ''' Gets or sets the headers to add to the request.
            ''' </summary>
			Public Property Headers As RequestHeaders Implements IRequest.Headers
            ''' <summary>
            ''' Gets or sets the request error.
            ''' </summary>
			Public Property RequestError As  RequestError Implements IRequest.RequestError
            ''' <summary>
            ''' Gets or sets the HTTP web response.
            ''' </summary>
			Public Property HTTPWebResponse As HttpWebResponse Implements IRequest.HTTPWebResponse 
            ''' <summary>
            ''' Gets or sets the parameters for the request string.
            ''' </summary>
			Public Property Param As String Implements IRequest.Param
            ''' <summary>
            ''' Gets or sets the authentication mode.
            ''' </summary>
			Public Property AuthenticationMode As eAuthenticationMode Implements IRequest.AuthenticationMode
            ''' <summary>
            ''' Gets or sets the name of the user.
            ''' </summary>
			Public Property UserName As String Implements IRequest.UserName 
            ''' <summary>
            ''' Gets or sets the password.
            ''' </summary>
			Public Property Password As String Implements IRequest.Password
            ''' <summary>
            ''' Gets or sets the source of the request.
            ''' </summary>
			Public Property Source As String Implements IRequest.Source
            ''' <summary>
            ''' Gets or sets the name of the log file.
            ''' </summary>
			Public Property LogFileName As String Implements IRequest.LogFileName 
            ''' <summary>
            ''' Gets or sets a value indicating whether to create a log of the request.
            ''' </summary>
			Public Property CreateLog As Boolean Implements IRequest.CreateLog
            ''' <summary>
            ''' Gets or sets a value indicating whether to create a log of any errors.
            ''' </summary>
			Public Property CreateErrorLog As Boolean Implements IRequest.CreateErrorLog
            ''' <summary>
            ''' Gets or sets the DataMasks used to mask data in the request.
            ''' </summary>
			Public Property DataMasking As Generic.List(Of DataMaskDef) Implements IRequest.DataMasking 
            ''' <summary>
            ''' Gets or sets extra information.
            ''' </summary>
			Public Property ExtraInfo As Object Implements IRequest.ExtraInfo
            ''' <summary>
            ''' Gets or sets the response holder.
            ''' </summary>
			Public Property ResponseHolder As Generic.List(Of Object) Implements IRequest.ResponseHolder
            ''' <summary>
            ''' Gets or sets the request tracker.
            ''' </summary>
			Public Property RequestTracker As RequestTracker Implements IRequest.RequestTracker
            ''' <summary>
            ''' Gets or sets a value indicating whether this <see cref="Request"/> is SOAP.
            ''' </summary>
			Public Property SOAP As Boolean Implements IRequest.SOAP
            ''' <summary>
            ''' Gets or sets the cancellation token source.
            ''' </summary>
			Public Property CancellationTokenSource As CancellationTokenSource Implements IRequest.CancellationTokenSource
            ''' <summary>
            ''' Gets or sets the request cookies.
            ''' </summary>
			Public Property RequestCookies As CookieCollection Implements IRequest.RequestCookies
            ''' <summary>
            ''' Gets or sets a value indicating whether to suppress the <see cref="ServicePointManager.Expect100Continue"/>.
            ''' </summary>
			Public Property SuppressExpectHeaders As Boolean Implements IRequest.SuppressExpectHeaders 
            ''' <summary>
            ''' Gets or sets the client certificates for this request.
            ''' </summary>
			Public Property ClientCertificates As ClientCertificates Implements IRequest.ClientCertificates 
            ''' <summary>
            ''' Gets or sets a value indicating whether to make the connection to the resource persistent.
            ''' </summary>
			Public Property KeepAlive As Boolean Implements IRequest.KeepAlive 

            ''' <summary>
            ''' Function to execute when the request is complete
            ''' </summary>
			Public ReturnFunction As CallbackDelegate 'Implements IRequest.ReturnFunction

#End Region

#Region "ReadOnly properties"

            ''' <summary>
            ''' Container for the response Xml.
            ''' </summary>
			Private Property oResponseXML As XmlDocument = Nothing

            ''' <summary>
            ''' Container for the HttpWebRequest.
            ''' </summary>
			Private Property oHTTPWebRequest As HttpWebRequest = Nothing

            ''' <summary>
            ''' Holds the request string as an array of bytes.
            ''' </summary>
			Private Property oBuffer As Byte()

            ''' <summary>
            ''' Gets or sets the request string.
            ''' </summary>
			Private Property sRequestString As String = ""

            ''' <summary>
            ''' Gets or sets the response string.
            ''' </summary>
			Private Property sResponseString As String = ""

            ''' <summary>
            ''' DateTime representing when the request is sent.
            ''' </summary>
			Private Property dRequestTimeSent As DateTime

            ''' <summary>
            ''' DateTime representing when the request is received.
            ''' </summary>
			Private Property dRequestTimeReceived As DateTime

            ''' <summary>
            ''' DateTime representing when the request stream is started.
            ''' </summary>
			Private Property dGetRequestStreamStarted As DateTime

            ''' <summary>
            ''' DateTime representing when the request stream is finished.
            ''' </summary>
			Private Property dGetRequestStreamFinished As DateTime

            ''' <summary>
            ''' DateTime representing when the read of the response stream is started.
            ''' </summary>
			Private Property dReadStreamStarted As DateTime

            ''' <summary>
            ''' DateTime representing when the read of the response stream is finished.
            ''' </summary>
			Private Property dReadStreamFinished As DateTime

            ''' <summary>
            ''' DateTime representing when the request process is started.
            ''' </summary>
			Private Property dProcessStarted As DateTime

            ''' <summary>
            ''' DateTime representing when the request process is finished.
            ''' </summary>
			Private Property dProcessFinished As DateTime

            ''' <summary>
            ''' Encoding to use for the response.
            ''' </summary>
			Private Property oResponseEncoding As System.Text.Encoding = System.Text.Encoding.UTF8

            ''' <summary>
            ''' Gets or sets the replacement character used in data masking.
            ''' </summary>
			Private Shared Property ReplacementCharacter As String = "X"

            ''' <summary>
            ''' Gets the request log.
            ''' </summary>
			Public ReadOnly Property RequestLog As String Implements IRequest.RequestLog
				Get

					Dim sbRequestLog As New StringBuilder
					With sbRequestLog
						.AppendFormatLine("URL: {0}", MaskData(Me.EndPoint, Me.DataMasking, True))
						.AppendLine()
						.AppendLine("**HEADERS**")
						.AppendLine()

						Dim i As Integer
						If oHTTPWebRequest IsNot Nothing Then
							For i = 0 To oHTTPWebRequest.Headers.Count - 1
								.AppendFormatLine("{0}: {1}", oHTTPWebRequest.Headers.Keys(i), oHTTPWebRequest.Headers.Item(i))
							Next
						End If

						.AppendFormatLine("Timeout: {0}", Me.TimeoutInSeconds)

						.AppendLine()
						.AppendLine()
						.AppendLine("**REQUEST**")
						.AppendLine()
						If Me.RequestXML.InnerText <> "" Then
							.AppendLine(XMLFunctions.XMLDocumentToFormattedString(
							 MaskData(Me.RequestXML, Me.DataMasking)))
						Else
							.AppendLine(MaskData(Me.RequestString, Me.DataMasking))
						End If

					End With

					Return sbRequestLog.ToString

				End Get
			End Property

            ''' <summary>
            ''' Gets the response log.
            ''' </summary>
			Public ReadOnly Property ResponseLog As String Implements IRequest.ResponseLog
				Get

					Dim sbResponseLog As New StringBuilder
					With sbResponseLog
						.AppendFormatLine("RequestWriteTimeTaken: {0}", SafeDecimal(dGetRequestStreamFinished.Subtract(dGetRequestStreamStarted).TotalMilliseconds / 1000))
						.AppendFormatLine("RequestTimeTaken: {0}", SafeDecimal(dRequestTimeReceived.Subtract(dRequestTimeSent).TotalMilliseconds / 1000))
						.AppendFormatLine("ResponseReadTimeTaken: {0}", SafeDecimal(dReadStreamFinished.Subtract(dReadStreamStarted).TotalMilliseconds / 1000))
						.AppendFormatLine("TotalTimeTaken: {0}", Me.RequestDuration)
						.AppendFormatLine("Timeout: {0}", Me.TimeoutInSeconds)
						.AppendLine()
						.AppendLine("**HEADERS**")
						.AppendLine()

						If HTTPWebResponse IsNot Nothing Then
							Dim i As Integer
							For i = 0 To HTTPWebResponse.Headers.Count - 1
								.AppendFormatLine("{0}: {1}", HTTPWebResponse.Headers.Keys(i), HTTPWebResponse.Headers.Item(i))
							Next
						End If

						.AppendLine()
						.AppendLine()
						.AppendLine("**RESPONSE**")
						.AppendLine()

						If Me.ResponseXML.InnerXml <> "" AndAlso
							Me.ContentType <> WebRequests.ContentType.Application_json AndAlso
							Me.ResponseXML.FirstChild.Name <> "Error" Then
							.AppendLine(XMLFunctions.XMLDocumentToFormattedString(MaskData(Me.ResponseXML, Me.DataMasking)))
						Else
							.AppendLine(MaskData(Me.ResponseString, Me.DataMasking))
						End If
						If Me.RequestError.Text <> "" Then

							Dim sErrorResponse As String = ""
							If Me.RequestError.Response <> "" Then
								Try
									Dim oErrorXML As New XmlDocument
									oErrorXML.LoadXml(Me.RequestError.Response)
									sErrorResponse = XMLFunctions.XMLDocumentToFormattedString(oErrorXML)
								Catch ex As Exception
									sErrorResponse = Me.RequestError.Response
								End Try
							End If

							.AppendLine()
							.AppendLine()
							.AppendLine("**ERRORS**")
							.AppendLine()
							.AppendLine(Me.RequestError.Text)
							.AppendLine()
							.AppendLine(Me.RequestError.Status)
							.AppendLine()
							.AppendLine(sErrorResponse)
						End If

					End With

					Return sbResponseLog.ToString

				End Get
			End Property

			''' <summary>
			''' Gets the HTML logs.
			''' </summary>
			Public ReadOnly Property HtmlLogs As Dictionary(Of WebExchange, String) Implements IRequest.HtmlLogs
				Get
					Dim aHtmlLogs As New Dictionary(Of WebExchange, String)

					Dim oRequestStringBuilder As New StringBuilder()

					With oRequestStringBuilder
						.AppendLine("<h1>Request Log</h1>")

						.AppendLine("<dl>")

						.AppendLine("<dt>URL</dt>")
						.AppendFormatLine("<dd><a href=""{0}"" target=""_blank"">{0}</a></dd>",
							MaskData(EndPoint, DataMasking, True))

						.AppendLine("</dl>")

						.AppendLine("<h2>Headers</h2>")

						.AppendLine("<dl>")

						If oHTTPWebRequest IsNot Nothing Then

							For i As Integer = 0 To oHTTPWebRequest.Headers.Count - 1
								.AppendFormatLine("<dt>{0}</dt>",
									oHTTPWebRequest.Headers.Keys(i))
								.AppendFormatLine("<dd>{0}</dd>",
									oHTTPWebRequest.Headers.Item(i))
							Next
						End If

						If HttpContext.Current IsNot Nothing Then
							.AppendLine("<dt>HostAddress</dt>")
							.AppendFormatLine("<dd>{0}</dd>",
								HttpContext.Current.Request.UserHostAddress)
						End If

						.AppendLine("<dt>Timeout</dt>")
						.AppendFormatLine("<dd>{0}</dd>",
							TimeoutInSeconds)

						.AppendLine("</dl>")

						.AppendLine("<h2>Body</h2>")

						If RequestXML.InnerText IsNot String.Empty Then
							.AppendFormatLine("<pre>{0}</pre>",
								HtmlEncode(XMLFunctions.XMLDocumentToFormattedString(MaskData(RequestXML, DataMasking))))
						Else
							.AppendLine(HtmlEncode(MaskData(RequestString, DataMasking)))
						End If
					End With

					aHtmlLogs.Add(WebExchange.Request, oRequestStringBuilder.ToString())

					Dim oResponseStringBuilder As New StringBuilder

					With oResponseStringBuilder
						.AppendLine("<h1>Response Log</h1>")

						.AppendLine("<dl>")

						.AppendLine("<dt>RequestWriteTimeTaken</dt>")
						.AppendFormatLine("<dd>{0}</dd>",
							SafeDecimal(dGetRequestStreamFinished.Subtract(dGetRequestStreamStarted).TotalMilliseconds / 1000))

						.AppendLine("<dt>RequestTimeTaken</dt>")
						.AppendFormatLine("<dd>{0}</dd>",
							SafeDecimal(dRequestTimeReceived.Subtract(dRequestTimeSent).TotalMilliseconds / 1000))

						.AppendLine("<dt>ResponseReadTimeTaken</dt>")
						.AppendFormatLine("<dd>{0}</dd>",
							SafeDecimal(dReadStreamFinished.Subtract(dReadStreamStarted).TotalMilliseconds / 1000))

						.AppendLine("<dt>TotalTimeTaken</dt>")
						.AppendFormatLine("<dd>{0}</dd>",
							RequestDuration)

						.AppendLine("<dt>Timeout</dt>")
						.AppendFormatLine("<dd>{0}</dd>", TimeoutInSeconds)

						.AppendLine("</dl>")

						.AppendLine("<h2>Headers</h2>")

						If HTTPWebResponse IsNot Nothing Then

							For i As Integer = 0 To HTTPWebResponse.Headers.Count - 1
								.AppendFormatLine("<dt>{0}</dt>",
									HTTPWebResponse.Headers.Keys(i))
								.AppendFormatLine("<dd>{0}</dd>",
									HTTPWebResponse.Headers.Item(i))
							Next
						End If

						.AppendLine("<h2>Body</h2>")

						If ResponseXML.InnerXml IsNot String.Empty Then
							.AppendFormatLine("<pre>{0}</pre>",
								HtmlEncode(XMLFunctions.XMLDocumentToFormattedString(MaskData(ResponseXML, DataMasking))))
						Else
							.AppendLine(HtmlEncode(MaskData(ResponseString, DataMasking)))
						End If

						If RequestError.Text IsNot String.Empty Then

							.AppendLine("<h2>Errors</h2>")

							.AppendLine(RequestError.Text)
							.AppendLine()
							.AppendLine(RequestError.Status)
							.AppendLine()

							If RequestError.Response IsNot String.Empty Then

								Try
									Dim oErrorXml As New XmlDocument

									oErrorXml.LoadXml(RequestError.Response)

									.AppendFormatLine("<pre>{0}</pre>",
										XMLFunctions.XMLDocumentToFormattedString(oErrorXml))
								Catch ex As Exception
									.AppendLine(RequestError.Response)
								End Try
							End If
						End If
					End With

					aHtmlLogs.Add(WebExchange.Response, oResponseStringBuilder.ToString())

					Return aHtmlLogs
				End Get
			End Property

            ''' <summary>
            ''' Gets the duration of the request.
            ''' </summary>
			Public ReadOnly Property RequestDuration As Decimal Implements IRequest.RequestDuration
				Get
					Return Intuitive.Functions.SafeDecimal(dProcessFinished.Subtract(dProcessStarted).TotalMilliseconds / 1000)
				End Get
			End Property

            ''' <summary>
            ''' Gets the request string.
            ''' </summary>
			Public ReadOnly Property RequestString As String Implements IRequest.RequestString
				Get

					If Me.Param <> "" Then
						Return Me.Param & "=" & System.Web.HttpUtility.UrlEncode(sRequestString)
					Else
						Return sRequestString
					End If

				End Get
			End Property

            ''' <summary>
            ''' Gets the response string.
            ''' </summary>
			Public ReadOnly Property ResponseString As String Implements IRequest.ResponseString
				Get
					Return sResponseString
				End Get
			End Property

            ''' <summary>
            ''' Gets the request XML.
            ''' </summary>
			Public ReadOnly Property RequestXML As XmlDocument Implements IRequest.RequestXML
				Get

					Dim oRequestXML As XmlDocument = New XmlDocument

					'check we have some valid xml just in case someone is relying on it
					If sRequestString <> "" Then
						Try
							oRequestXML.LoadXml(sRequestString)
						Catch ex As Exception

						End Try
					End If

					Return oRequestXML

				End Get
			End Property

            ''' <summary>
            ''' Gets the response XML.
            ''' </summary>
			Public ReadOnly Property ResponseXML As XmlDocument Implements IRequest.ResponseXML
				Get
					'check we have some valid xml just in case someone is relying on it
					If oResponseXML Is Nothing AndAlso sResponseString <> "" Then
						oResponseXML = New XmlDocument
						Try
							oResponseXML.LoadXml(sResponseString)
						Catch ex As Exception
						    oResponseXML.LoadXml("<Error>Failed to .LoadXml Response, Bad XML</Error>")
						End Try
					ElseIf oResponseXML Is Nothing Then
						oResponseXML = New XmlDocument
						oResponseXML.LoadXml("<Error></Error>")
					End If

					Return oResponseXML
				End Get
			End Property

            ''' <summary>
            ''' Gets the buffer of the request string.
            ''' </summary>
			Public ReadOnly Property Buffer As Byte() Implements IRequest.Buffer
				Get
					If oBuffer Is Nothing Then
						Dim sRequestString As String = Me.RequestString
                        oBuffer = Me.ResponseEncoding.GetBytes(sRequestString)
					End If
					Return oBuffer
				End Get
			End Property

            ''' <summary>
            ''' Gets the HTTP web request, if it doesn't exist, the request is created then returned.
            ''' </summary>
			Public ReadOnly Property HTTPWebRequest As HttpWebRequest Implements IRequest.HTTPWebRequest 
				Get

					If Me.oHTTPWebRequest Is Nothing Then

						'dotnet automatically adds on a this header which is usually helpful
						'some server don't accept it so check to see if we want it suppressed
						If SuppressExpectHeaders Then
							ServicePointManager.Expect100Continue = False
						End If

						oHTTPWebRequest = CType(HttpWebRequest.Create(Me.EndPoint), HttpWebRequest)
						With oHTTPWebRequest

							'settings we control
							.Timeout = Me.TimeoutInSeconds * 1000
							.Method = Me.Method.ToString
							.KeepAlive = Me.KeepAlive

							'headers
							For Each oHeader As RequestHeader In Me.Headers
								.Headers.Add(oHeader.Name, oHeader.Value)
							Next

							'user agent
							If Me.UserAgent <> "" Then
								.UserAgent = Me.UserAgent
							End If

							'accept
							If Me.Accept <> "" Then
								.Accept = Me.Accept
							End If

							'soap
							If Me.SOAP OrElse Me.SoapAction <> "" Then
								With .Headers
									.Add(RequestHeaderName.Action, Me.SoapAction)
									.Add(RequestHeaderName.SoapAction, Me.SoapAction)
								End With
							End If

							'authentication
							If Me.AuthenticationMode = eAuthenticationMode.Basic Then

								Dim sValue As String = Me.GetBasicAuthHeader(Me.UserName, Me.Password)

								.Headers.Add(HttpRequestHeader.Authorization, sValue)

							ElseIf Me.AuthenticationMode = eAuthenticationMode.Digest Then

								Dim oCredentials As New CredentialCache
								Dim oURIPrefix As New Uri(oHTTPWebRequest.RequestUri.GetLeftPart(UriPartial.Authority))

								Dim oNetworkCredentials As New NetworkCredential(Me.UserName, Me.Password)

								oCredentials.Add(oURIPrefix, "Digest", oNetworkCredentials)

								oHTTPWebRequest.Credentials = oCredentials

							End If

							'cookies
							Dim oCookieContainer As New CookieContainer
							If Me.RequestCookies.Count > 0 Then

								'set the domain if its not done already
								For Each oCookie As Cookie In Me.RequestCookies
									If oCookie.Domain = "" Then
										oCookie.Domain = oHTTPWebRequest.Host
									End If
								Next

								oCookieContainer.Add(Me.RequestCookies)
							End If
							.CookieContainer = oCookieContainer

							'gzip
							If Me.UseGZip Then
								.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip")
							End If

							'Certificates
							If Me.ClientCertificates.Count > 0 Then

								For Each oCertificate As X509Certificate2 In Me.ClientCertificates

									'Complete the certificate chain
									Dim oX509Chain As New X509Chain

									'Add the certificate to the request
									oHTTPWebRequest.ClientCertificates.Add(oCertificate)

									oX509Chain.Build(oCertificate)

								Next

							End If

							'standard settings
							.Credentials = System.Net.CredentialCache.DefaultCredentials

							If InList(Me.Method, eRequestMethod.POST, eRequestMethod.PUT) Then
								.ContentLength = Me.Buffer.Length
								.AllowAutoRedirect = True
								.AllowWriteStreamBuffering = True
								.ContentType = Me.ContentType
							End If

						End With

					End If

					Return oHTTPWebRequest

				End Get
			End Property

            ''' <summary>
            ''' Gets a value indicating whether there is an exception.
            ''' </summary>
			Public ReadOnly Property Exception As Boolean Implements IRequest.Exception
				Get
					Return Me.RequestError.Text <> "" AndAlso Me.RequestError.Status.ToLower <> "timeout"
				End Get
			End Property

            ''' <summary>
            ''' Gets a value indicating whether the request timed out.
            ''' </summary>
			Public ReadOnly Property TimeOut As Boolean Implements IRequest.TimeOut 
				Get
					Return Me.RequestError.Status.ToLower = "timeout"
				End Get
			End Property

            ''' <summary>
            ''' Returns true when <see cref="RequestError.Status"/> and <see cref="RequestError.Text"/> are blank
            ''' </summary>
			Public ReadOnly Property Success As Boolean Implements IRequest.Success
				Get
					Return Me.RequestError.Status = "" AndAlso Me.RequestError.Text = ""
				End Get
			End Property

            ''' <summary>
            ''' Gets the response cookies.
            ''' </summary>
			Public ReadOnly Property ResponseCookies As CookieCollection Implements IRequest.ResponseCookies
				Get
                    Return Me.HTTPWebResponse?.Cookies
				End Get
			End Property

            ''' <summary>
            ''' Gets the response encoding.
            ''' </summary>
			Public ReadOnly Property ResponseEncoding As System.Text.Encoding Implements IRequest.ResponseEncoding
				Get
					Return oResponseEncoding
				End Get
			End Property

#End Region

#Region "Setters"

			''' <summary>
			''' Sets the process start time.
			''' </summary>
			Public Sub SetProcessStartTime() Implements IRequest.SetProcessStartTime
				Me.dProcessStarted = Date.Now
			End Sub

			''' <summary>
			''' Sets the process finished time.
			''' </summary>
			Public Sub SetProcessFinishedTime() Implements IRequest.SetProcessFinishedTime
				Me.dProcessFinished = Date.Now
			End Sub

			''' <summary>
			''' Sets the request stream start time.
			''' </summary>
			Public Sub SetGetRequestStreamStartTime() Implements IRequest.SetGetRequestStreamStartTime
				Me.dGetRequestStreamStarted = Date.Now
			End Sub

			''' <summary>
			''' Sets the request stream finished time.
			''' </summary>
			Public Sub SetGetRequestStreamFinishedTime() Implements IRequest.SetGetRequestStreamFinishedTime
				Me.dGetRequestStreamFinished = Date.Now
			End Sub

			''' <summary>
			''' Sets the response stream read start time.
			''' </summary>
			Public Sub SetReadResponseStreamStartTime() Implements IRequest.SetReadResponseStreamStartTime
				Me.dReadStreamStarted = Date.Now
			End Sub

			''' <summary>
			''' Sets the response stream read finished time.
			''' </summary>
			Public Sub SetReadResponseStreamFinishedTime() Implements IRequest.SetReadResponseStreamFinishedTime
				Me.dReadStreamFinished = Date.Now
			End Sub

			''' <summary>
			''' Sets the request start time.
			''' </summary>
			Public Sub SetRequestStartTime() Implements IRequest.SetRequestStartTime
				Me.dRequestTimeSent = Date.Now
			End Sub

			''' <summary>
			''' Sets the request received time.
			''' </summary>
			Public Sub SetRequestReceivedTime() Implements IRequest.SetRequestReceivedTime
				Me.dRequestTimeReceived = Date.Now
			End Sub

			''' <summary>
			''' Sets the request.
			''' </summary>
			''' <param name="Request">The request.</param>
			Public Sub SetRequest(ByVal Request As String) Implements IRequest.SetRequest
				Me.sRequestString = Request
			End Sub

            ''' <summary>
            ''' Sets the request to the outer xml of the specified XmlDocument.
            ''' </summary>
            ''' <param name="Request">The request.</param>
			Public Sub SetRequest(ByVal Request As XmlDocument) Implements IRequest.SetRequest
				Me.sRequestString = Request.OuterXml
			End Sub

            ''' <summary>
            ''' Sets the response.
            ''' </summary>
            ''' <param name="Response">The response.</param>
			Public Sub SetResponse(ByVal Response As String) Implements IRequest.SetResponse
				Me.sResponseString = Response
			End Sub

            ''' <summary>
            ''' Sets the response encoding.
            ''' </summary>
            ''' <param name="oEncoding">The encoding.</param>
			Public Sub SetResponseEncoding(ByVal oEncoding As eEncoding) Implements IRequest.SetResponseEncoding

				Select Case oEncoding
					Case eEncoding.ASCII
						Me.oResponseEncoding = System.Text.Encoding.ASCII
					Case eEncoding.UTF8
						Me.oResponseEncoding = System.Text.Encoding.UTF8
					Case eEncoding.UTF32
						Me.oResponseEncoding = System.Text.Encoding.UTF32
					Case eEncoding.ISO_8859_1
						Me.oResponseEncoding = System.Text.Encoding.GetEncoding(28591)
				End Select

			End Sub

#End Region

#Region "Functions"

            ''' <summary>
            ''' Validates this request instance.
            ''' </summary>
			Public Function Validate() As Boolean Implements IRequest.Validate

				'1. check the endpoint
				If Me.EndPoint = "" Then
					Me.RequestError.Text = "An endpoint needs to be set"
					Return False
				End If

				'2. check specific values for post requests
				If Me.Method = eRequestMethod.POST Then

					If (Me.RequestString Is Nothing OrElse Me.RequestString = "") AndAlso Me.HTTPWebRequest.ContentLength <> 0 Then
						Me.RequestError.Text = "The xml request is blank"
						Return False
					End If

					'3. SOAP

				End If

				Return True

			End Function

            ''' <summary>
            ''' Sends this request.
            ''' </summary>
			Public Sub Send() Implements IRequest.Send

				MakeRequest(Me)

			End Sub

            ''' <summary>
            ''' Adds a log entry for this request if <see cref="CreateLog"/> is True
            ''' </summary>
			Public Sub SaveRequestLog() Implements IRequest.SaveRequestLog

				If Me.CreateLog Then

					FileFunctions.AddLogEntry(Me.Source, Me.LogFileName & "Request", Me.RequestLog)

				End If

			End Sub

            ''' <summary>
            ''' Adds a log entry for the response if <see cref="CreateLog"/> is True
            ''' </summary>
			Public Sub SaveResponseLog() Implements IRequest.SaveResponseLog

				If Me.CreateLog Then

					FileFunctions.AddLogEntry(Me.Source, Me.LogFileName & "Response", Me.ResponseLog)

				ElseIf Me.CreateErrorLog AndAlso Me.Success = False Then

					FileFunctions.AddLogEntry(Me.Source, Me.LogFileName & "Error", Me.ResponseLog)

				End If

			End Sub

            ''' <summary>
            ''' Executes this requests <see cref="ReturnFunction"/>
            ''' </summary>
			Public Sub Complete() Implements IRequest.Complete
				ReturnFunction(Me)
			End Sub

            ''' <summary>
            ''' Adds a cookie with the specified Name, Value and Domain to this requests <see cref="RequestCookies"/>
            ''' </summary>
            ''' <param name="CookieName">Name of the cookie.</param>
            ''' <param name="CookieValue">The cookie value.</param>
            ''' <param name="Domain">The domain for the cookie.</param>
			Public Sub AddCookie(ByVal CookieName As String, ByVal CookieValue As String, Optional ByVal Domain As String = "") Implements IRequest.AddCookie

				Dim oCookie As New Cookie(CookieName, CookieValue)

				If Domain <> "" Then
					oCookie.Domain = Domain
				End If

				Me.RequestCookies.Add(oCookie)

			End Sub

			Public Function GetBasicAuthHeader(UserName As String, Password As String) As String
				Return "Basic " + Convert.ToBase64String(New ASCIIEncoding().GetBytes(UserName & ":" & Password))
			End Function

#Region "Secure Logging"

			''' <summary>
			''' Mask data in the specified request using the specified list of datamasks
			''' </summary>
			''' <param name="Request">The request.</param>
			''' <param name="DataMasking">The data masking.</param>
			''' <param name="URL">If set to <c>true</c> [URL].</param>
			Public Function MaskData(ByVal Request As String, ByVal DataMasking As Generic.List(Of DataMaskDef), Optional ByVal URL As Boolean = False) As String Implements IRequest.MaskData

				Try
					'Go Through Each DataMasking and Remove the Stuff
					For Each oDataMaskDef As DataMaskDef In Me.DataMasking

						If Not URL OrElse (URL AndAlso oDataMaskDef.MaskingType = eDataMaskType.URL) Then

							'Set the Replacement character needs to be set on the the class,
							'As the Delegate function wont let me pass it.
							ReplacementCharacter = oDataMaskDef.ReplacementCharacter

							If ReplacementCharacter = "" Then
								ReplacementCharacter = "X"
							End If

							'If we are masking the Request
							If oDataMaskDef.MaskingType = eDataMaskType.RequestData Then

								If oDataMaskDef.RegEx <> "" Then

									Request = RegExReplace(Request, oDataMaskDef.RegEx)

								End If

								'If we are Masking the URL
							Else

								If oDataMaskDef.XPath <> "" Then
									Request = QueryStringReplace(Request, oDataMaskDef)
								Else
									Request = RegExReplace(Request, oDataMaskDef.RegEx)
								End If

							End If

						End If

					Next

				Catch ex As Exception
					'Do Nothing
				End Try

				Return Request

			End Function

            ''' <summary>
            ''' Masks the data in the specified request xml using the data masks in the specified list.
            ''' </summary>
            ''' <param name="RequestXml">The request XML.</param>
            ''' <param name="DataMasking">The data masking.</param>
			Public Function MaskData(ByVal RequestXml As XmlDocument, ByVal DataMasking As Generic.List(Of DataMaskDef)) As XmlDocument Implements IRequest.MaskData

				Dim oXML As New XmlDocument
				oXML.LoadXml(RequestXml.OuterXml)

				'Set up the XML Wrapper, It will build the Namespace manager for you
				Dim oXMLWrapper As New Intuitive.XMLFunctions.XMLDocumentWrapper(oXML)

				Try

					'Go Through each DataMasking and Remove the Stuff
					For Each oDataMaskDef As DataMaskDef In Me.DataMasking

						'Set the Replacement character needs to be set on the the class,
						'As the Delegate function wont let me pass it.
						ReplacementCharacter = oDataMaskDef.ReplacementCharacter

						If ReplacementCharacter = "" Then
							ReplacementCharacter = "X"
						End If

						If oDataMaskDef.XPath <> "" Then

							oXMLWrapper = XPathRegExReplace(oXMLWrapper, oDataMaskDef)

						ElseIf oDataMaskDef.RegEx <> "" Then

							oXMLWrapper = RegExReplace(oXMLWrapper, oDataMaskDef.RegEx)

						End If

					Next

				Catch ex As Exception

				End Try

				'Return the XMl Document
				Return oXMLWrapper.XmlDocument

			End Function

            ''' <summary>
            ''' This Function Will apply a regex to particular XPath in an xml doc, the xmlWrapper Class generates the namespace for you.
            ''' </summary>
            ''' <param name="XMLWrapper">The XML wrapper.</param>
            ''' <param name="DataMaskDef">The data mask definition.</param>
			Public Function XPathRegExReplace(ByVal XMLWrapper As Intuitive.XMLFunctions.XMLDocumentWrapper, ByVal DataMaskDef As DataMaskDef) As Intuitive.XMLFunctions.XMLDocumentWrapper Implements IRequest.XPathRegExReplace

				'This Function Will apply a regex to particular XPath in an xml doc, the xmlWrapper Class generates the namespace for you.
				Try
					'Grab the Nodes that match the xPath
					Dim oNodes As XmlNodeList = XMLWrapper.SelectNodes(DataMaskDef.XPath)

					'for each match Replace the Stuff using the Regex
					For Each oNode As XmlNode In oNodes
						Dim sNodeValue As String = oNode.InnerText

						oNode.InnerText = Regex.Replace(sNodeValue, DataMaskDef.RegEx,
						 New MatchEvaluator(AddressOf DataMaskReplace), RegexOptions.Compiled)

					Next

				Catch ex As Exception
					'Do Nothing
				End Try

				Return XMLWrapper

			End Function

            ''' <summary>
            ''' Replaces query string using specified datamask.
            ''' </summary>
            ''' <param name="Request">The request.</param>
            ''' <param name="DataMaskDef">The data mask definition.</param>
			Public Function QueryStringReplace(ByVal Request As String, ByVal DataMaskDef As DataMaskDef) As String Implements IRequest.QueryStringReplace
				Try
					Dim sBaseURL As String = Request.Split("?"c)(0)
					Dim sFullQueryString As String = Request.Split("?"c)(1)
					Dim sFinalURL As String = sBaseURL & "?"

					Dim sStrings() As String = sFullQueryString.Split("&"c)

					For Each sString As String In sStrings

						Dim sKey As String = sString.Split("="c)(0)
						Dim sValue As String = sString.Split("="c)(1)

						If DataMaskDef.XPath = sKey Then

							sValue = Regex.Replace(sValue, DataMaskDef.RegEx,
							  New MatchEvaluator(AddressOf DataMaskReplace), RegexOptions.Compiled)

						End If

						sFinalURL = sFinalURL & sKey & "=" & sValue & "&"

					Next

					Request = sFinalURL.Substring(0, sFinalURL.Length - 1)

				Catch ex As Exception
					'Do Nothing
				End Try

				Return Request

			End Function

            ''' <summary>
            ''' Replaces contents of xml that matches specified RegEx with X's
            ''' </summary>
            ''' <param name="oXMLWrapper">The XML wrapper.</param>
            ''' <param name="RegEx">The reg ex.</param>
			Public Function RegExReplace(ByVal oXMLWrapper As Intuitive.XMLFunctions.XMLDocumentWrapper, ByVal RegEx As String) As Intuitive.XMLFunctions.XMLDocumentWrapper Implements IRequest.RegExReplace

				'X's out all regex matches
				Try

					'Replace any thing that Matches in the xml with X's
					oXMLWrapper.XmlDocument.InnerXml = RegularExpressions.Regex.Replace(oXMLWrapper.XmlDocument.InnerXml, RegEx,
									 New MatchEvaluator(AddressOf DataMaskReplace),
									  RegexOptions.Compiled)
				Catch ex As Exception
					'Do Nothing
				End Try

				Return oXMLWrapper

			End Function

            ''' <summary>
            ''' Replaces content in the Request that matches the specified RegEx with X's
            ''' </summary>
            ''' <param name="Request">The request.</param>
            ''' <param name="RegEx">The reg ex.</param>
            ''' <returns></returns>
			Public Function RegExReplace(ByVal Request As String, ByVal RegEx As String) As String Implements IRequest.RegExReplace

				'X's out all regex matches
				Try

					'Replace any thing that Matches in the xml with X's
					Request = RegularExpressions.Regex.Replace(Request, RegEx, New MatchEvaluator(AddressOf DataMaskReplace),
									  RegexOptions.Compiled)
				Catch ex As Exception
					'Do Nothing
				End Try

				Return Request

			End Function

            ''' <summary>
            ''' Returns a string with the number of <see cref="ReplacementCharacter"/> matching the Matches length
            ''' </summary>
            ''' <param name="oMatch">The match.</param>
			Public Shared Function DataMaskReplace(ByVal oMatch As Match) As String
				Return "".PadLeft(oMatch.Length, CChar(ReplacementCharacter))
			End Function

#End Region

#End Region

		End Class

        ''' <summary>
        ''' Class representing a list of <see cref="X509Certificate2"/>
        ''' </summary>
        ''' <seealso cref="T:System.Collections.Generic.List{T}" />
		Public Class ClientCertificates
			Inherits Generic.List(Of X509Certificate2)

            ''' <summary>
            ''' Adds a new certificate from a byte array
            ''' </summary>
            ''' <param name="Certificate">The byte array containing data for the certificate.</param>
            ''' <param name="CertificatePassword">The password to access the certificate data.</param>
			Public Sub AddNew(ByVal Certificate() As Byte, ByVal CertificatePassword As String)

				Dim oClientCertificate As New X509Certificate2(Certificate, CertificatePassword)
				Me.Add(oClientCertificate)

			End Sub

            ''' <summary>
            ''' Adds a new certificate
            ''' </summary>
            ''' <param name="Certificate">The <see cref="X509Certificate2"/> certificate.</param>
			Public Sub AddNew(ByVal Certificate As X509Certificate2)
				Me.Add(Certificate)
			End Sub

            ''' <summary>
            ''' Adds a new certificate
            ''' </summary>
            ''' <param name="CertificateFriendlyName">Friendly name of the certificate.</param>
			Public Sub AddNew(ByVal CertificateFriendlyName As String)
				Me.Add(Security.Functions.GetCertificate(CertificateFriendlyName))
			End Sub

		End Class

        ''' <summary>
        ''' Class representing a list of Request Headers
        ''' </summary>
        ''' <seealso cref="T:System.Collections.Generic.List{T}" />
		Public Class RequestHeaders
			Inherits Generic.List(Of RequestHeader)

            ''' <summary>
            ''' Adds a new request header with the specified name and value.
            ''' </summary>
            ''' <param name="Name">The name.</param>
            ''' <param name="Value">The value.</param>
			Public Overloads Sub Add(Name As String, Value As String)

				Me.Add(New RequestHeader(Name, Value))

			End Sub

            ''' <summary>
            ''' Adds a new request header with the specified name and value.
            ''' </summary>
            ''' <param name="Name">The name.</param>
            ''' <param name="Value">The value.</param>
			Public Sub AddNew(ByVal Name As String, ByVal Value As String)

				Me.Add(Name, Value)

			End Sub

		End Class

        ''' <summary>
        ''' Class representing information for a request header
        ''' </summary>
		Public Class RequestHeader
            ''' <summary>
            ''' The request header name
            ''' </summary>
			Public Name As String
            ''' <summary>
            ''' The request header value
            ''' </summary>
			Public Value As String

			''' <summary>
			''' Initializes a new instance of the <see cref="RequestHeader"/> class.
			''' </summary>
            ''' <param name="Name">The name of the header.</param>
            ''' <param name="Value">The value of the header.</param>
			Public Sub New(Optional Name As String = Nothing, Optional Value As String = Nothing)

				Me.Name = Name
				Me.Value = Value

			End Sub

		End Class

        ''' <summary>
        ''' Class containing error information for a request
        ''' </summary>
		Public Class RequestError
            ''' <summary>
            ''' Gets or sets the status.
            ''' </summary>
			Public Property Status As String = ""
            ''' <summary>
            ''' Gets or sets the response.
            ''' </summary>
			Public Property Response As String = ""
            ''' <summary>
            ''' Gets or sets the text.
            ''' </summary>
			Public Property Text As String = ""
		End Class

        ''' <summary>
        ''' Class containing information about requests
        ''' </summary>
		Public Class RequestTracker
            ''' <summary>
            ''' Gets or sets the number of requests sent.
            ''' </summary>
			Public Property RequestsSent As Integer = 0

            ''' <summary>
            ''' Gets or sets the number of requests received.
            ''' </summary>
			Public Property RequestsReceived As Integer = 0

            ''' <summary>
            ''' List of request times
            ''' </summary>
			Public Overridable Property RequestTimes As New Generic.List(Of RequestTime)
		End Class

        ''' <summary>
        ''' Class for storing request times
        ''' </summary>
		Public Class RequestTime

            ''' <summary>
            ''' Whether there was an exception from the request
            ''' </summary>
			Public Exception As Boolean = False

            ''' <summary>
            ''' Whether the request timed out.
            ''' </summary>
			Public Timeout As Boolean = True

            ''' <summary>
            ''' Whether the request was successful.
            ''' </summary>
			Public Success As Boolean = False

            ''' <summary>
            ''' The start time of the request
            ''' </summary>
			Private dRequestStartTime As Date

            ''' <summary>
            ''' The total response time
            ''' </summary>
			Public TotalResponseTime As Decimal

			''' <summary>
			''' Starts the request timer.
			''' </summary>
			Public Sub StartRequestTimer()
				Me.dRequestStartTime = Date.Now
			End Sub

			''' <summary>
			''' Stops the request timer.
			''' </summary>
			Public Sub StopRequestTimer()
				Me.TotalResponseTime = SafeDecimal((Date.Now.Subtract(Me.dRequestStartTime).TotalMilliseconds / 1000))
			End Sub

		End Class

        ''' <summary>
        ''' Class containing information used with data masking
        ''' </summary>
		Public Class DataMaskDef

            ''' <summary>
            ''' The path for the nodes to have their data replaced
            ''' </summary>
			Public Property XPath As String = ""

            ''' <summary>
            ''' RegEx for data to replace
            ''' </summary>
			Public Property RegEx As String = ""

            ''' <summary>
            ''' The character to replace data with, X by default
            ''' </summary>
			Public Property ReplacementCharacter As String = "X"

            ''' <summary>
            ''' Gets or sets the type of the masking, <see cref="eDataMaskType"/>, defaults to <see cref="eDataMaskType.RequestData"/>.
            ''' </summary>
			Public Property MaskingType As eDataMaskType = eDataMaskType.RequestData

            ''' <summary>
            ''' Initializes a new instance of the <see cref="DataMaskDef"/> class.
            ''' </summary>
            ''' <param name="XPath">The XPath of nodes to mask.</param>
            ''' <param name="RegX">The RegEx to match data on.</param>
            ''' <param name="ReplacementCharacter">The character to replace matching data with, X by default.</param>
            ''' <param name="MaskingType">Type of masking required, defaults to <see cref="eDataMaskType.RequestData"/>.</param>
			Public Sub New(ByVal XPath As String, ByVal RegX As String, Optional ByVal ReplacementCharacter As String = "X",
			   Optional ByVal MaskingType As eDataMaskType = eDataMaskType.RequestData)

				Me.XPath = XPath
				Me.RegEx = RegX
				Me.MaskingType = MaskingType

				If ReplacementCharacter = "" Then
					Me.ReplacementCharacter = "X"
				Else
					Me.ReplacementCharacter = ReplacementCharacter
				End If

			End Sub
		End Class

#Region "Enums and constants"

        ''' <summary>
        ''' Enum for possible http request methods
        ''' </summary>
		Public Enum eRequestMethod

			''' <summary>
			''' GET method
			''' </summary>
			[GET]

			''' <summary>
			''' POST method
			''' </summary>
			[POST]

			''' <summary>
			''' PUT method
			''' </summary>
			[PUT]

			''' <summary>
			''' DELETE method
			''' </summary>
			[DELETE]

		End Enum

        ''' <summary>
        ''' Enum for possible authentication modes
        ''' </summary>
		Public Enum eAuthenticationMode

			''' <summary>
			''' Basic authentication
			''' </summary>
			Basic

			''' <summary>
			''' Digest authentication
			''' </summary>
			Digest

			''' <summary>
			''' No authentication
			''' </summary>
			None

		End Enum

        ''' <summary>
        ''' Enum for possible character encoding
        ''' </summary>
		Public Enum eEncoding

			''' <summary>
			''' UTF8 encoding
			''' </summary>
			UTF8

			''' <summary>
			''' UTF16 encoding
			''' </summary>
			UTF32

			''' <summary>
			''' ASCII encoding
			''' </summary>
			ASCII

			''' <summary>
			''' ISO-8859-1 encoding
			''' </summary>
			ISO_8859_1

		End Enum

        ''' <summary>
        ''' Enum for possible Data Mask Types
        ''' </summary>
		Public Enum eDataMaskType

			''' <summary>
			''' Mask request data
			''' </summary>
			RequestData

			''' <summary>
			''' Mask URL
			''' </summary>
			URL

		End Enum

        ''' <summary>
        ''' Class containing constants for possible MIME types for a web request
        ''' </summary>
		Public Class ContentType

			''' <summary>
			''' The application SOAP XML.
			''' </summary>
			Public Const Application_SOAP_XML As String = "application/soap+xml"

            ''' <summary>
            ''' The application_x_www_form_urlencoded
            ''' </summary>
			Public Const Application_x_www_form_urlencoded As String = "application/x-www-form-urlencoded"

            ''' <summary>
            ''' The text_xml
            ''' </summary>
			Public Const Text_xml As String = "text/xml"

            ''' <summary>
            ''' The text_html
            ''' </summary>
			Public Const Text_html As String = "text/html"

			''' <summary>
            ''' The text_html_charset_utf_8
            ''' </summary>
            Public Const Text_html_charset_utf_8 As String = "text/html; charset=utf-8"

            ''' <summary>
            ''' The application_xhtml_xml
            ''' </summary>
			Public Const Application_xhtml_xml As String = "application/xhtml+xml"

            ''' <summary>
            ''' The text_ xml_charset_utf_8
            ''' </summary>
			Public Const Text_Xml_charset_utf_8 As String = "text/xml; charset=utf-8"

            ''' <summary>
            ''' The application_xml
            ''' </summary>
			Public Const Application_xml As String = "application/xml"

            ''' <summary>
            ''' The application_json
            ''' </summary>
			Public Const Application_json As String = "application/json"

		End Class

#End Region

#End Region

#Region "Muliple Request holder"

        ''' <summary>
        ''' Class to handle multiple requests
        ''' </summary>
		Public Class MultipleRequestHandler

            ''' <summary>
            ''' List of <see cref="Request"/>.
            ''' </summary>
			Public Requests As New Generic.List(Of Request)

            ''' <summary>
            ''' Number responses received
            ''' </summary>
			Public ResponsesReceived As Integer

            ''' <summary>
            ''' The response holder
            ''' </summary>
			Public ResponseHolder As Generic.List(Of Object)

            ''' <summary>
            ''' Gets a value indicating whether the number of responses received matches the request count.
            ''' </summary>
			Public ReadOnly Property Complete As Boolean
				Get
					Return Requests.Count = ResponsesReceived
				End Get
			End Property

            ''' <summary>
            ''' Send each request
            ''' </summary>
			Public Sub Send()

				For Each WebRequest As Request In Me.Requests

					Try

						'0. validate
						If Not WebRequest.Validate Then
							Return
						End If

						'1. start the timer
						' WebRequest.SetStartTime()

						'2. set up the stream if we're doing a post request
						If WebRequest.Method = eRequestMethod.POST Then

							'3. get the request stream for posts
							WebRequest.HTTPWebRequest.BeginGetRequestStream(AddressOf RequestStreamCallBack, WebRequest)
						Else

							'3. otherwise just send off the request
							WebRequest.SaveRequestLog()
							WebRequest.HTTPWebRequest.BeginGetResponse(AddressOf ReadResponse, WebRequest)
						End If

					Catch ex As Exception
					End Try

				Next

			End Sub

            ''' <summary>
            ''' Callback to handle asyn results.
            ''' </summary>
            ''' <param name="oResult">The result.</param>
			Private Sub RequestStreamCallBack(ByVal oResult As IAsyncResult)

				Try

					Dim oStream As Stream = Nothing
					Dim oHTTPRequest As HttpWebRequest
					Dim oRequest As Request = CType(oResult.AsyncState, Request)

					'1. get the request stream
					oHTTPRequest = oRequest.HTTPWebRequest
					oStream = oHTTPRequest.EndGetRequestStream(oResult)

					'2. write the request to the stream
					Dim oWriter As New StreamWriter(oStream)
					oWriter.Write(oRequest.RequestString)
					oWriter.Close()

					'3. send off the request
					oRequest.SaveRequestLog()
					oHTTPRequest.BeginGetResponse(AddressOf ReadResponse, oRequest)

				Catch ex As Exception
				End Try

			End Sub

            ''' <summary>
            ''' Reads the response of result.
            ''' </summary>
            ''' <param name="oResult">The result.</param>
			Private Sub ReadResponse(oResult As IAsyncResult)

				Dim oHTTPResponse As HttpWebResponse
				Dim oRequest As New Request
				Dim oReader As StreamReader
				Dim oStream As Stream = Nothing

				Try

					'1. get the response
					oRequest = CType(oResult.AsyncState, Request)
					oHTTPResponse = CType(oRequest.HTTPWebRequest.EndGetResponse(oResult), HttpWebResponse)
					oStream = oHTTPResponse.GetResponseStream

					'2. un zip
					If oHTTPResponse.Headers(HttpResponseHeader.ContentEncoding) = "gzip" Then
						oStream = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(oStream)
						oReader = New StreamReader(oStream, Encoding.Default)
					Else
						oReader = New StreamReader(oStream, True)
					End If

					'3. load the response onto our class
					Dim sResponse As String = oReader.ReadToEnd
					oRequest.SetResponse(sResponse)
					oRequest.SaveResponseLog()

					'4. close everything
					If oHTTPResponse IsNot Nothing Then
						oHTTPResponse.Close()
					End If
					If oReader IsNot Nothing Then
						oReader.Close()
					End If

				Catch ex As WebException

					'5. try to get as many details as we can
					Dim oError As New RequestError
					With oError
						.Text = ex.ToString
						.Status = ex.Status.ToString

						If ex.Response IsNot Nothing Then
							Dim oErrorResponse As WebResponse = ex.Response
							Dim oExStream As Stream = oErrorResponse.GetResponseStream
							Dim oExceptionReader As StreamReader = New StreamReader(oExStream, True)
							.Response = oExceptionReader.ReadToEnd
							oExceptionReader.Close()
							oErrorResponse.Close()
						End If

					End With

					oRequest.RequestError = oError
					oRequest.SaveResponseLog()

				End Try

				'6. set the end time
				' oRequest.SetReceivedTime()

				'7. finished
				oRequest.Complete()

				Interlocked.Increment(Me.ResponsesReceived)

			End Sub

		End Class

#End Region

#Region "Interface"

        ''' <summary>
        ''' Delegate function for requests
        ''' </summary>
        ''' <param name="Request">The request.</param>
		Public Delegate Sub RequestDelegate(ByVal Request As Request)

        ''' <summary>
        ''' Delegate function for callbacks
        ''' </summary>
        ''' <param name="Request">The request.</param>
		Public Delegate Sub CallbackDelegate(ByVal Request As Request)

#End Region

	End Class

End Namespace
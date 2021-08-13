Imports System.Xml

Namespace Net

	Public Interface IRequest

#Region "Public Properties"

		''' <summary>
		''' Gets or sets the method for the request.
		''' </summary>
		Property Method As WebRequests.eRequestMethod

		''' <summary>
		''' Gets or sets the timeout in seconds for the request.
		''' </summary>
		Property TimeoutInSeconds As Integer
		''' <summary>
		''' Gets or sets a value indicating whether to receive the response gzipped
		''' </summary>
		Property UseGZip As Boolean

		'Public Property Success As Boolean = False

		''' <summary>
		''' Gets or sets a value indicating whether to use deflate.
		''' </summary>
		Property UseDeflate As Boolean
		''' <summary>
		''' Gets or sets the value of the user-agent header.
		''' </summary>
		Property UserAgent As String
		''' <summary>
		''' Gets or sets the value of the accept http header.
		''' </summary>
		Property Accept As String
		''' <summary>
		''' Gets or sets the value of the content-type http header.
		''' </summary>
		Property ContentType As String
		''' <summary>
		''' Gets or sets the SOAP action.
		''' </summary>
		Property SoapAction As String
		''' <summary>
		''' Gets or sets the end point for the request.
		''' </summary>
		Property EndPoint As String
		''' <summary>
		''' Gets or sets the headers to add to the request.
		''' </summary>
		Property Headers As WebRequests.RequestHeaders
		''' <summary>
		''' Gets or sets the request error.
		''' </summary>
		Property RequestError As WebRequests.RequestError
		''' <summary>
		''' Gets or sets the HTTP web response.
		''' </summary>
		Property HTTPWebResponse As System.Net.HttpWebResponse
		''' <summary>
		''' Gets or sets the parameters for the request string.
		''' </summary>
		Property Param As String
		''' <summary>
		''' Gets or sets the authentication mode.
		''' </summary>
		Property AuthenticationMode As WebRequests.eAuthenticationMode
		''' <summary>
		''' Gets or sets the name of the user.
		''' </summary>
		Property UserName As String
		''' <summary>
		''' Gets or sets the password.
		''' </summary>
		Property Password As String
		''' <summary>
		''' Gets or sets the source of the request.
		''' </summary>
		Property Source As String
		''' <summary>
		''' Gets or sets the name of the log file.
		''' </summary>
		Property LogFileName As String
		''' <summary>
		''' Gets or sets a value indicating whether to create a log of the request.
		''' </summary>
		Property CreateLog As Boolean
		''' <summary>
		''' Gets or sets a value indicating whether to create a log of any errors.
		''' </summary>
		Property CreateErrorLog As Boolean
		''' <summary>
		''' Gets or sets the DataMasks used to mask data in the request.
		''' </summary>
		Property DataMasking As Generic.List(Of WebRequests.DataMaskDef)
		''' <summary>
		''' Gets or sets extra information.
		''' </summary>
		Property ExtraInfo As Object
		''' <summary>
		''' Gets or sets the response holder.
		''' </summary>
		Property ResponseHolder As Generic.List(Of Object)
		''' <summary>
		''' Gets or sets the request tracker.
		''' </summary>
		Property RequestTracker As WebRequests.RequestTracker
		''' <summary>
		''' Gets or sets a value indicating whether this <see cref="WebRequests.Request"/> is SOAP.
		''' </summary>
		Property SOAP As Boolean
		''' <summary>
		''' Gets or sets the cancellation token source.
		''' </summary>
		Property CancellationTokenSource As Threading.CancellationTokenSource
		''' <summary>
		''' Gets or sets the request cookies.
		''' </summary>
		Property RequestCookies As System.Net.CookieCollection
		''' <summary>
		''' Gets or sets a value indicating whether to suppress the <see cref="System.Net.ServicePointManager.Expect100Continue"/>.
		''' </summary>
		Property SuppressExpectHeaders As Boolean
		''' <summary>
		''' Gets or sets the client certificates for this request.
		''' </summary>
		Property ClientCertificates As Net.WebRequests.ClientCertificates
		''' <summary>
		''' Gets or sets a value indicating whether to make the connection to the resource persistent.
		''' </summary>
		Property KeepAlive As Boolean

#End Region

#Region "ReadOnly properties"

		''' <summary>
		''' Gets the request log.
		''' </summary>
		ReadOnly Property RequestLog As String

		''' <summary>
		''' Gets the response log.
		''' </summary>
		ReadOnly Property ResponseLog As String

		''' <summary>
		''' Gets the HTML logs.
		''' </summary>
		ReadOnly Property HtmlLogs As Collections.Generic.Dictionary(Of WebRequests.WebExchange, String)

		''' <summary>
		''' Gets the duration of the request.
		''' </summary>
		ReadOnly Property RequestDuration As Decimal

		''' <summary>
		''' Gets the request string.
		''' </summary>
		ReadOnly Property RequestString As String

		''' <summary>
		''' Gets the response string.
		''' </summary>
		ReadOnly Property ResponseString As String

		''' <summary>
		''' Gets the request XML.
		''' </summary>
		ReadOnly Property RequestXML As System.Xml.XmlDocument

		''' <summary>
		''' Gets the response XML.
		''' </summary>
		ReadOnly Property ResponseXML As System.Xml.XmlDocument

		''' <summary>
		''' Gets the buffer of the request string.
		''' </summary>
		ReadOnly Property Buffer As Byte()

		''' <summary>
		''' Gets the HTTP web request, if it doesn't exist, the request is created then returned.
		''' </summary>
		ReadOnly Property HTTPWebRequest As System.Net.HttpWebRequest

		''' <summary>
		''' Gets a value indicating whether there is an exception.
		''' </summary>
		ReadOnly Property Exception As Boolean

		''' <summary>
		''' Gets a value indicating whether the request timed out.
		''' </summary>
		ReadOnly Property TimeOut As Boolean

		''' <summary>
		''' Returns true when <see cref="WebRequests.RequestError.Status"/> and <see cref="WebRequests.RequestError.Text"/> are blank
		''' </summary>
		ReadOnly Property Success As Boolean

		''' <summary>
		''' Gets the response cookies.
		''' </summary>
		ReadOnly Property ResponseCookies As System.Net.CookieCollection

		''' <summary>
		''' Gets the response encoding.
		''' </summary>
		ReadOnly Property ResponseEncoding As System.Text.Encoding

#End Region

#Region "Functions"

		''' <summary>
		''' Sets the process start time.
		''' </summary>
		Sub SetProcessStartTime()

		''' <summary>
		''' Sets the process finished time.
		''' </summary>
		Sub SetProcessFinishedTime()

		''' <summary>
		''' Sets the request stream start time.
		''' </summary>
		Sub SetGetRequestStreamStartTime()

		''' <summary>
		''' Sets the request stream finished time.
		''' </summary>
		Sub SetGetRequestStreamFinishedTime()

		''' <summary>
		''' Sets the response stream read start time.
		''' </summary>
		Sub SetReadResponseStreamStartTime()

		''' <summary>
		''' Sets the response stream read finished time.
		''' </summary>
		Sub SetReadResponseStreamFinishedTime()

		''' <summary>
		''' Sets the request start time.
		''' </summary>
		Sub SetRequestStartTime()

		''' <summary>
		''' Sets the request received time.
		''' </summary>
		Sub SetRequestReceivedTime()

		''' <summary>
		''' Sets the request.
		''' </summary>
		''' <param name="Request">The request.</param>
		Sub SetRequest(ByVal Request As String)

		''' <summary>
		''' Sets the request to the outer xml of the specified XmlDocument.
		''' </summary>
		''' <param name="Request">The request.</param>
		Sub SetRequest(ByVal Request As XmlDocument)

		''' <summary>
		''' Sets the response.
		''' </summary>
		''' <param name="Response">The response.</param>
		Sub SetResponse(ByVal Response As String)

		''' <summary>
		''' Sets the response encoding.
		''' </summary>
		''' <param name="oEncoding">The encoding.</param>
		Sub SetResponseEncoding(ByVal oEncoding As WebRequests.eEncoding)

		''' <summary>
		''' Validates this request instance.
		''' </summary>
		Function Validate() As Boolean

		''' <summary>
		''' Sends this request.
		''' </summary>
		Sub Send()

		''' <summary>
		''' Adds a log entry for this request if <see cref="CreateLog"/> is True
		''' </summary>
		Sub SaveRequestLog()

		''' <summary>
		''' Adds a log entry for the response if <see cref="CreateLog"/> is True
		''' </summary>
		Sub SaveResponseLog()

		''' <summary>
		''' Executes this requests <see cref="Webrequests.Request.ReturnFunction"/>
		''' </summary>
		Sub Complete()

		''' <summary>
		''' Adds a cookie with the specified Name, Value and Domain to this requests <see cref="RequestCookies"/>
		''' </summary>
		''' <param name="CookieName">Name of the cookie.</param>
		''' <param name="CookieValue">The cookie value.</param>
		''' <param name="Domain">The domain for the cookie.</param>
		Sub AddCookie(ByVal CookieName As String, ByVal CookieValue As String, Optional ByVal Domain As String = "")

		''' <summary>
		''' Mask data in the specified request using the specified list of datamasks
		''' </summary>
		''' <param name="Request">The request.</param>
		''' <param name="DataMasking">The data masking.</param>
		''' <param name="URL">If set to <c>true</c> [URL].</param>
		Function MaskData(ByVal Request As String, ByVal DataMasking As Generic.List(Of WebRequests.DataMaskDef), Optional ByVal URL As Boolean = False) As String

		''' <summary>
		''' Masks the data in the specified request xml using the data masks in the specified list.
		''' </summary>
		''' <param name="RequestXml">The request XML.</param>
		''' <param name="DataMasking">The data masking.</param>
		Function MaskData(ByVal RequestXml As XmlDocument, ByVal DataMasking As Generic.List(Of WebRequests.DataMaskDef)) As XmlDocument

		''' <summary>
		''' This Function Will apply a regex to particular XPath in an xml doc, the xmlWrapper Class generates the namespace for you.
		''' </summary>
		''' <param name="XMLWrapper">The XML wrapper.</param>
		''' <param name="DataMaskDef">The data mask definition.</param>
		Function XPathRegExReplace(ByVal XMLWrapper As Intuitive.XMLFunctions.XMLDocumentWrapper, ByVal DataMaskDef As WebRequests.DataMaskDef) As Intuitive.XMLFunctions.XMLDocumentWrapper

		''' <summary>
		''' Replaces query string using specified datamask.
		''' </summary>
		''' <param name="Request">The request.</param>
		''' <param name="DataMaskDef">The data mask definition.</param>
		Function QueryStringReplace(ByVal Request As String, ByVal DataMaskDef As WebRequests.DataMaskDef) As String

		''' <summary>
		''' Replaces contents of xml that matches specified RegEx with X's
		''' </summary>
		''' <param name="oXMLWrapper">The XML wrapper.</param>
		''' <param name="RegEx">The reg ex.</param>
		Function RegExReplace(ByVal oXMLWrapper As Intuitive.XMLFunctions.XMLDocumentWrapper, ByVal RegEx As String) As Intuitive.XMLFunctions.XMLDocumentWrapper

		''' <summary>
		''' Replaces content in the Request that matches the specified RegEx with X's
		''' </summary>
		''' <param name="Request">The request.</param>
		''' <param name="RegEx">The reg ex.</param>
		''' <returns></returns>
		Function RegExReplace(ByVal Request As String, ByVal RegEx As String) As String

#End Region

	End Interface

End Namespace
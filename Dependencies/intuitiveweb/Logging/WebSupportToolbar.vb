Imports System.Text
Imports Intuitive
Imports System.Web
Imports System.Text.RegularExpressions
Imports Intuitive.Functions
Imports System.Configuration.ConfigurationManager

Public Class WebSupportToolbar


#Region "properties (connect string, loggedin, sqllogger and searchlogger"

	'logged in
    Public Shared Property LoggedIn() As Boolean
        Get
            Return Intuitive.Functions.SafeBoolean(HttpContext.Current.Session("__websupporttoolbar_loggedin"))
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("__websupporttoolbar_loggedin") = value
        End Set
    End Property


    'searchlogger
	Public Shared Property SearchLogger() As List(Of BookingSearch.RequestInfo)
		Get
			If HttpContext.Current.Session("__websupporttoolbar_search") Is Nothing Then
				WebSupportToolbar.SearchLogger = New List(Of BookingSearch.RequestInfo)
			End If
			Return CType(HttpContext.Current.Session("__websupporttoolbar_search"), List(Of BookingSearch.RequestInfo))
		End Get
		Set(value As List(Of BookingSearch.RequestInfo))
			HttpContext.Current.Session.Add("__websupporttoolbar_search", value)
		End Set
	End Property


#End Region

	Public Shared Sub AddUniqueLog(Log As BookingSearch.RequestInfo)
		Try
			Dim oInfos As IEnumerable(Of BookingSearch.RequestInfo) = WebSupportToolbar.SearchLogger.Where(Function(o) _
			  o.Type = Log.Type)

			'only keep the last search for now, can't just clear them down as we want to keep property/flight also
			'long winded but defensive
			Dim oInfosToRemove As New List(Of BookingSearch.RequestInfo)
			For Each oInfo As BookingSearch.RequestInfo In oInfos
				oInfosToRemove.Add(oInfo)
			Next
			For Each oInfo As BookingSearch.RequestInfo In oInfosToRemove
				WebSupportToolbar.SearchLogger.Remove(oInfo)
			Next

			WebSupportToolbar.SearchLogger.Add(Log)
		Catch ex As Exception
			Dim s As String = ""
		End Try
	End Sub

#Region "Widget + Request Times"

    Public Function Time() As String

        Dim sb As New System.Text.StringBuilder

        sb.AppendLine("Page Event Times")
        sb.AppendLine("======")

        sb.AppendLines(2)

        sb.AppendLine("url: " & Logging.Current.PageEvents.Url)
        sb.AppendLines(2)

		For Each oTime As Logging.RequestTime In Logging.Current.PageEvents.PreviousEvents


			sb.AppendLine("Stage: " & oTime.[Event].ToString)
			sb.AppendLine("Time taken: " & oTime.TimeInSeconds)

			sb.AppendLines(1)


		Next

		Return Me.FormatResponse(sb.ToString)

    End Function



    Public Function Widget() As String

        Dim sb As New System.Text.StringBuilder

        sb.AppendLine("Widget Draw Times")
        sb.AppendLine("======")

        sb.AppendLines(2)
        Dim total As Double

		For Each oWidget As Logging.WidgetSpeed In Logging.Current.WidgetSpeeds

			sb.AppendLine("Name: " & oWidget.Name)
			sb.AppendLine("Time: " & oWidget.TimeInSeconds)
			total += oWidget.TimeInSeconds
			sb.AppendLines(1)

		Next

		sb.AppendLines(2)
        sb.AppendLine("Total: " & total)

        Return Me.FormatResponse(sb.ToString)

    End Function

#End Region

#Region "login"

	Public Function Login(ByVal EmailAddress As String, ByVal Password As String) As String

		Dim iSystemUserID As Integer

		If EmailAddress = "paul" AndAlso Password = "****" Then
			iSystemUserID = 1
		Else

		End If


		If iSystemUserID > 0 Then
			WebSupportToolbar.LoggedIn = True
			Return WebSupportToolbar.GetToolbar()
		Else
			WebSupportToolbar.LoggedIn = False
			Return ""
		End If

	End Function

#End Region


#Region "gettoolbar"

	Public Shared Function GetToolbar() As String

		Dim sb As New System.Text.StringBuilder


		sb.AppendLine("<style type=""text/css"">")
		sb.AppendLine("	#__websupport {position:fixed;top:-1px;left:10px;padding:3px;background:#fff;border:solid 1px #ccc;z-index:1000;}")
		sb.AppendLine("	#__websupport a {background-image: url('style/glyphicons-halflings.png');display:block;float:left;margin-right:3px;height:14px;line-height:14px;vertical-align:text-top;width:14px;text-indent:-1000px;}")
		sb.AppendLine("	#__websupport a.server {background-position: -96px -24px;}")
		sb.AppendLine("	#__websupport a.basket {background-position: -360px -120px;}")
		sb.AppendLine("	#__websupport a.xml {background-position: -264px -48px;}")
		sb.AppendLine("	#__websupport a.flight {background-position: -168px -120px;}")
		sb.AppendLine("	#__websupport a.search {background-position: -48px 0;}")
        sb.AppendLine("	#__websupport a.packagesearch {background-position: -120px 0;}")
        sb.AppendLine("	#__websupport a.request {background-position: -384px -96px;}")
        sb.AppendLine("	#__websupport a.sql {background-position: -264px -24px;}")
        sb.AppendLine("	#__websupport a.widget {background-position: -384px -144px;}")
		sb.AppendLine("	#__websupport a.sqlclear {background-position:  -312px 0;}")
		sb.AppendLine("	#__websupport a.logout {background-position: -384px 0;}")
		sb.AppendLine(" div.__pop {position:fixed !important;top:5% !important;left:5% !important;width:90%;height:90%;background:#fff;border:solid 5px #ddd;z-index:101;padding:10px;overflow:scroll;}")
		sb.AppendLine(" div.__pop pre {font-family:consolas;}")
		sb.AppendLine("</style>")

		sb.AppendLine("<div id=""__websupport"">")
		sb.AppendLine("	<a href=""#"" class=""server"" title=""Server"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Server', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">Server</a>")
		sb.AppendLine("	<a href=""#"" class=""basket"" title=""Basket"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Basket', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">Basket</a>")
		sb.AppendLine("	<a href=""#"" class=""basket"" title=""Search Basket"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.SearchBasket', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">Basket</a>")
		sb.AppendLine("	<a href=""#"" class=""sql"" title=""Page Performance"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.PagePerformance', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})}, JSON.stringify(window.performance.timing));"">Page Performance</a>")
		sb.AppendLine("	<a href=""#"" class=""xml"" title=""iVector Connect calls"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.IVCCalls', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">iVector Connect Calls</a>")

		sb.AppendLine("	<a href=""#"" class=""search"" title=""Search"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Search', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">Search</a>")

        sb.AppendLine("	<a href=""#"" class=""widget"" title=""Widget Render Times"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Widget', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">widget</a>")
        sb.AppendLine("	<a href=""#"" class=""request"" title=""Request Times"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Time', function(s) {int.f.ShowPopup(this, '__pop', s);document.body.onclick=function() {int.f.HidePopup();};int.f.AttachEvent('divPopup', 'click', function(e) { console.log(e);e.stopPropagation()})});"">widget</a>")

		'sb.AppendLine("	<a href=""#"" class=""packagesearch"" title=""Package Search"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.PackageSearch', function(s) {int.f.ShowPopup(this, '__pop', s);});document.body.onclick=function() {int.f.HidePopup();};return false;"">Package Search</a>")
		'sb.AppendLine("	<a href=""#"" class=""sql"" title=""SQL"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.SQLLog', function(s) {int.f.ShowPopup(this, '__pop', s);});document.body.onclick=function() {int.f.HidePopup();};return false;"">SQL</a>")
		'sb.AppendLine("	<a href=""#"" class=""sqlclear"" title=""Clear SQL"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.ClearSQLLog', function(s) {return false;});return false;"">Clear SQL Log</a>")
		sb.AppendLine("	<a href=""#"" class=""logout"" title=""Log out"" onclick=""int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Logout', function(s) {int.f.GetObject('__websupport').removeNode(true);});return false;"">Logout</a>")
		sb.AppendLine("</div>")



		Return sb.ToString

	End Function

#End Region



#Region "result - server"

	Public Function Server() As String

		Dim sb As New System.Text.StringBuilder

		sb.AppendLine("Server")
		sb.AppendLine("======")
		sb.AppendLine(HttpContext.Current.Server.MachineName)
		sb.AppendLines(2)

		sb.AppendLine("Session")
		sb.AppendLine("=======")
		sb.AppendLine(HttpContext.Current.Session.SessionID)
		sb.AppendLines(2)

		'Main Connect String
		Dim sConnectString As String = System.Configuration.ConfigurationManager.AppSettings("ConnectString").ToSafeString()

		If sConnectString <> "" Then
			sb.AppendLine("ConnectString")
			sb.AppendLine("=============")
			sb.AppendLine(WebSupportToolbar.FormatConnectString(sConnectString))
			sb.AppendLines(2)
		End If

		'Booking Connect String
		Dim sBookingConnectString As String = System.Configuration.ConfigurationManager.AppSettings("BookingConnectString").ToSafeString()

		If sBookingConnectString <> "" Then
			sb.AppendLine("Booking ConnectString")
			sb.AppendLine("=====================")
			sb.AppendLine(WebSupportToolbar.FormatConnectString(sBookingConnectString))
			sb.AppendLines(2)
		End If

		'Package Connect String
		Dim sPackageConnectString As String = System.Configuration.ConfigurationManager.AppSettings("PackageConnectString").ToSafeString()

		If sPackageConnectString <> "" Then
			sb.AppendLine("Package ConnectString")
			sb.AppendLine("=====================")
			sb.AppendLine(WebSupportToolbar.FormatConnectString(sPackageConnectString))
			sb.AppendLines(2)
		End If


		'Search Store Connect String
		'Dim sSearchStoreConnectString As String = Settings.Search.SearchStoreConnectString

		If sPackageConnectString <> "" Then
			sb.AppendLine("SearchStore ConnectString")
			sb.AppendLine("=========================")
			'   sb.AppendLine(WebSupportToolbar.FormatConnectString(sSearchStoreConnectString))
			sb.AppendLines(2)
		End If


		Return Me.FormatResponse(sb.ToString)

	End Function


#End Region

#Region "result - basket"

	Public Function Basket() As String

		Dim sb As New System.Text.StringBuilder
		sb.AppendLine("Basket")
		sb.AppendLine("======")
		sb.AppendLines(2)

		Return Me.FormatResponse(sb.ToString() & Intuitive.XMLFunctions.FormatXML(BookingBase.Basket.XML.InnerXml))
	End Function

	Public Function SearchBasket() As String

		Dim sb As New System.Text.StringBuilder
		sb.AppendLine("Search Basket")
		sb.AppendLine("======")
		sb.AppendLines(2)

		Return Me.FormatResponse(sb.ToString() & Intuitive.XMLFunctions.FormatXML(BookingBase.SearchBasket.XML.InnerXml))
	End Function

#End Region


#Region "Performance"

	Public Function PagePerformance(Performance As String) As String

		Dim oPerformance As New Performance
		oPerformance = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Performance)(Performance)

		Dim sb As New System.Text.StringBuilder
		sb.AppendLine("Page Performance")
		sb.AppendLine("======")
		sb.AppendLines(2)

		If Not oPerformance.redirectStart = 0 Then
			sb.AppendLine("Redirect")
			sb.AppendLine("======")
			sb.AppendLine(oPerformance.Redirect)
			sb.AppendLines(2)
		End If

		sb.AppendLine("DNS Lookup")
		sb.AppendLine("======")
		sb.AppendLine(oPerformance.DnsLookup)
		sb.AppendLines(2)


		sb.AppendLine("TCP")
		sb.AppendLine("======")
		sb.AppendLine(oPerformance.TCP)
		sb.AppendLines(2)


        sb.AppendLine("Server response time (including network latency)")
		sb.AppendLine("======")
		sb.AppendLine(oPerformance.NetworkLatency)
		sb.AppendLines(2)


		sb.AppendLine("DOM Ready (Customer can interact with page)")
		sb.AppendLine("======")
		sb.AppendLine(oPerformance.DomReady)
		sb.AppendLines(2)


		sb.AppendLine("Page Loaded (incl. all images and frames)")
		sb.AppendLine("======")
		sb.AppendLine(oPerformance.PageLoad)
		sb.AppendLines(2)

		Return Me.FormatResponse(sb.ToString())

	End Function


	Public Class Performance
		Public connectEnd As Long
		Public connectStart As Long
		Public domComplete As Long
		Public domContentLoadedEventEnd As Long
		Public domContentLoadedEventStart As Long
		Public domInteractive As Long
		Public domLoading As Long
		Public domainLookupEnd As Long
		Public domainLookupStart As Long
		Public fetchStart As Long
		Public loadEventEnd As Long
		Public loadEventStart As Long
		Public navigationStart As Long
		Public redirectEnd As Long
		Public redirectStart As Long
		Public requestStart As Long
		Public responseEnd As Long
		Public responseStart As Long
		Public secureConnectionStart As Long
		Public unloadEventEnd As Long
		Public unloadEventStart As Long

		Public Function PageLoad() As String
			Return SafeString((Me.loadEventEnd - Me.navigationStart) / 1000) & " seconds"
		End Function

		Public Function DomReady() As String
			Return SafeString((Me.domContentLoadedEventEnd - Me.navigationStart) / 1000) & " seconds"
		End Function

		Public Function DnsLookup() As String
			Return SafeString((Me.domainLookupEnd - Me.domainLookupStart) / 1000) & " seconds"
		End Function

		Public Function Redirect() As String
			Return SafeString((Me.redirectEnd - Me.redirectStart) / 1000) & " seconds"
		End Function

		Public Function TCP() As String
			Return SafeString((Me.connectEnd - Me.connectStart) / 1000) & " seconds"
		End Function

		Public Function NetworkLatency() As String
			Return SafeString((Me.responseEnd - Me.requestStart) / 1000) & " seconds"
		End Function


	End Class



#End Region


#Region "ivc calls"


	Public Function IVCCalls() As String

		Dim sb As New System.Text.StringBuilder

		sb.AppendLine("iVC Calls")
		sb.AppendLine("======")
		sb.AppendLines(2)

		For Each oIVCLog As Logging.IVCLog In Logging.Current.IVCLogs
			sb.Append(Functions.CreateFixedLengthString(oIVCLog.Name, 150)).Append(oIVCLog.ResponseTime.ToString & "ms").AppendLine()
			sb.AppendLines(2)
		Next

		Return Me.FormatResponse(sb.ToString())

	End Function


#End Region


#Region "result - xml"


	Public Function XML() As String

		If Not HttpContext.Current.Session("__booking_basket") Is Nothing Then

			'get the basket
			Dim oBasket As BookingBasket =
			 CType(HttpContext.Current.Session("__booking_basket"), BookingBasket)

			'get the BookingXML nodes that we are interested in
			Dim oBookingsXMLNodes As System.Xml.XmlNodeList = oBasket.XML.GetElementsByTagName("BookingXML")

			Dim sResponse As String = ""

			'if there are 0 then send back a message saying so
			If oBookingsXMLNodes.Count = 0 Then
				sResponse = "There are no Booking XML items in the basket"
				Return sResponse
			Else

				Dim oBookingXMLs As New BookingXMLs

				'loop through each of the nodes and add them to our class
				For Each oXMLNode As System.Xml.XmlNode In oBookingsXMLNodes

					Dim oBookingXML As New BookingXML
					oBookingXML.ParentType = (oXMLNode.Item("ParentType").InnerXml.ToString)
					oBookingXML.Type = (oXMLNode.Item("Type").InnerXml.ToString)
					oBookingXML.XML.InnerXml = System.Web.HttpUtility.HtmlDecode(oXMLNode.Item("XML").InnerXml.ToString).ToString

					oBookingXMLs.BookingXMLs.Add(oBookingXML)

				Next

				'then put each of the bookingxmls in our class in to the response and seperate with some lines to make it look ok
				For Each oBookingXML As BookingXML In oBookingXMLs.BookingXMLs
					sResponse = sResponse & "=============" & ControlChars.NewLine & ControlChars.NewLine & oBookingXML.Type & ControlChars.NewLine & ControlChars.NewLine & Intuitive.XMLFunctions.FormatXML(oBookingXML.XML.InnerXml) & ControlChars.NewLine & ControlChars.NewLine
				Next

				Return Me.FormatResponse(sResponse)


			End If

		Else
			Return "No basket found"
		End If

	End Function


#End Region



#Region "result - search"

	Public Function Search() As String

		Dim sb As New System.Text.StringBuilder

		sb.AppendLine("Last search(es)")
		sb.AppendLine("======")
		sb.AppendLines(2)

		sb.AppendLine("For the response XML, please refresh the page with ?sendlogs=your@emailhere.com appended to be emailed the logs")
		sb.AppendLines(2)

		For Each oRequestInfo As BookingSearch.RequestInfo In WebSupportToolbar.SearchLogger
			sb.AppendLine(oRequestInfo.Type.ToString & " took " & oRequestInfo.RequestTime / 1000 & " seconds")
			sb.AppendLine("Of this, network latency took up " & oRequestInfo.NetworkLatency / 1000 & " seconds")
			sb.AppendLines(2)
		Next

		For Each oRequestInfo As BookingSearch.RequestInfo In WebSupportToolbar.SearchLogger

			sb.AppendLines(2)
			sb.AppendLine("XML - " & oRequestInfo.Type.ToString)
			sb.AppendLine("======")
			sb.AppendLines(2)

			sb.AppendLine("Request")
			sb.AppendLine("======")
			sb.AppendLine(PrettifyXML(oRequestInfo.RequestXML.InnerXml))
			sb.AppendLines(2)

		Next

		Return Me.FormatResponse(sb.ToString)
	End Function

	Private Function PrettifyXML(XMLString As String) As String
		Dim sw As New System.IO.StringWriter()
		Dim xw As New System.Xml.XmlTextWriter(sw)
		xw.Formatting = System.Xml.Formatting.Indented
		xw.Indentation = 4
		Dim doc As New System.Xml.XmlDocument
		doc.LoadXml(XMLString)
		doc.Save(xw)
		Return sw.ToString()
	End Function

#End Region



#Region "result - logout"

	Public Function Logout() As String
		WebSupportToolbar.LoggedIn = False
		Return ""
	End Function

#End Region


#Region "support (formatrespose)"

	Private Function FormatResponse(ByVal Response As String) As String

		Dim sReturn As String = Response
		sReturn = Regex.Replace(sReturn, "<Login>.*<\/Login>", "<Login>???</Login>")
		sReturn = Regex.Replace(sReturn, "<Password>.*<\/Password>", "<Password>???</Password>")

		Return "<input onclick=""int.f.SelectText(int.f.GetObject('__pre_websupport_text'))"" style=""float:right;"" type=""button"" value=""Copy"" />" & _
		  "<pre id=""__pre_websupport_text"">" & _
		  System.Web.HttpUtility.HtmlEncode(sReturn) & "</pre>"
	End Function

#End Region

#Region "support (BookingXML class)"

	Public Class BookingXMLs
		Public Property BookingXMLs As New Generic.List(Of BookingXML)
	End Class

	Public Class BookingXML
		Public Property XML As New System.Xml.XmlDocument
		Public Property ParentType As String
		Public Property Type As String
	End Class
#End Region

#Region "format connectstring"

	Private Shared Function FormatConnectString(ByVal ConnectString As String) As String

		'example
		'server=mars;database=lch_dev;uid=sa;pwd=trek1200;connect timeout=5;

		Dim sReturn As String = ConnectString

		'remove the username and password!!
		sReturn = Regex.Replace(sReturn, "uid=[^;]+", "uid=???")
		sReturn = Regex.Replace(sReturn, "pwd=[^;]+", "pwd=???")

		Return sReturn

	End Function


	Public Shared Sub EmailSearchLogs(EmailTo As String)


		Dim oEmail As New Email
		Dim sb As New StringBuilder

		'sb.AppendFormatLine("{0} round trip took {1}ms/{2}seconds", oRequestInfo.Type, oRequestInfo.RequestTime.ToString(), (oRequestInfo.RequestTime / 1000).ToString())

		For Each oInfo As BookingSearch.RequestInfo In WebSupportToolbar.SearchLogger
			sb.AppendLine(oInfo.Type.ToString & " took " & oInfo.RequestTime / 1000 & " seconds")
		Next

		With oEmail
			.SMTPHost = AppSettings("SMTPHost")
			.EmailTo = EmailTo
			.From = "iVectorConnect Logging"
			.FromEmail = "connect-logging@intuitivesystems.co.uk"
			.Subject = "iVector Connect Logging for " & Functions.GetBaseURL()
			.Body = sb.ToString.Replace("\n", ControlChars.NewLine)

			For Each oInfo As BookingSearch.RequestInfo In WebSupportToolbar.SearchLogger
				Dim oRequest As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(oInfo.RequestXML.InnerXml))
				.StreamAttachments.Add(oInfo.Type.ToString & " Request.xml", oRequest)

				Dim oResponse As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(oInfo.ResponseXML.InnerXml))
				.StreamAttachments.Add(oInfo.Type.ToString & " Response.xml", oResponse)
			Next

			.SendEmail()
		End With

	End Sub


#End Region


End Class


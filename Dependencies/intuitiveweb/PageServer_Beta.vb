Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports System.Reflection
Imports System.IO

''' <summary>
''' This beta class is so we have backwards compatible code (we could remove the page server interface otherwise), 
''' although this is an improved architecture for widgets + their settings
''' we should really migrate other customers gradually and generalise code eg zuji do lots differently in their page server class
''' </summary>
''' <remarks></remarks>
Public Class PageServer_Beta
    Implements PageServer

    Private iVectorWidgetPath As String
    Private iVectorWidgetsAssembly As Assembly
	Private BaseAssembly As Assembly
	Private DomainRedirects As New Generic.List(Of DomainRedirect)

    Public Sub New(iVectorWidgetPath As String, iVectorWidgetsAssembly As Assembly, BaseAssembly As Assembly)
        Me.iVectorWidgetPath = iVectorWidgetPath
        Me.iVectorWidgetsAssembly = iVectorWidgetsAssembly
        Me.BaseAssembly = BaseAssembly
    End Sub

    'empty constructor for rebuilding page def to get widget settings on a cache miss (normally through an ajax call) - called from widgetbase as an example
    'widgetbase does not need the getwidgetreference function so we can ignore those constructor parameters above
    Public Sub New()

    End Sub

#Region "Get Page Definition"

    Public Function GetPageDefinition(PageName As String) As Intuitive.Web.PageDefinition Implements PageServer.GetPageDefinition

        Return GetPageDefinitionCommon(PageName, True)

    End Function

    Public Function GetPageDefinition(Request As System.Web.HttpRequest) As Intuitive.Web.PageDefinition Implements PageServer.GetPageDefinition

        '0. if query string contains copylib update the widget library files
        If Request.RawUrl.IndexOf("copylib") > -1 Then
            Intuitive.Web.Utility.CopyLibraryWidgets(HttpContext.Current.Server.MapPath("~/"), Me.iVectorWidgetPath)
        End If

        '2. get url from request
        Dim sPath As String = Request.Url.AbsolutePath.ToLower
        sPath = sPath.Replace("default.aspx", "")

        'this code is only if it is running under a virtual directory - currently wont get used
        Dim sAppPath As String = HttpContext.Current.Request.ApplicationPath
        'conditions
        '1 the app path is not "/" - because we only care if its under a virtual directory
        '2 the path is not the root
        '3 the path starts with the app path
        If Not sAppPath = "/" AndAlso Not sPath = "/" AndAlso sPath.StartsWith(sAppPath) Then
            sPath = sPath.Substring(sAppPath.Length)
        End If


        Dim sRootPath As String = BookingBase.Params.RootPath
        'if the root path is set then remove that from the start of the URL
        If Not sRootPath = "" AndAlso sPath.StartsWith(sRootPath) Then
            'remove the root part
            sPath = sPath.Substring(sRootPath.Length)
            'add a '/' if its empty
            If sPath = "" Then sPath = "/"
		End If


        Return GetPageDefinitionCommon(sPath)

    End Function


    Private Function GetPageDefinitionCommon(identifier As String, Optional usePageName As Boolean = False) As Intuitive.Web.PageDefinition


        '1. load up xml 
        Dim oXML As New XmlDocument
        oXML.Load(HttpContext.Current.Server.MapPath("~/pagedefinition.xml"))

        'check for theme override
        Dim oThemeXML As New XmlDocument
        If File.Exists(HttpContext.Current.Server.MapPath("~/themes/" & BookingBase.Params.Theme & "/pagedefinition.xml")) Then
            oThemeXML.Load(HttpContext.Current.Server.MapPath("~/themes/" & BookingBase.Params.Theme & "/pagedefinition.xml"))
        End If



        '3. get page definition
        Dim oNodeResult As NodeResult = GetNode(identifier, oThemeXML, oXML, usePageName)
		Dim bIsWildcard As Boolean = False



		'4 add 301 redirect?

		'4.1 if current URL does not match domain, check if we have a domain redirect
		Dim sBaseURL As String = Intuitive.Functions.GetBaseURL().Replace(":80", "").Replace(":81", "").ToLower
		Dim bDomainRedirect As Boolean = False

		If sBaseURL <> BookingBase.Params.Domain.ToLower Then

			Dim oDomainRedirectXML As XmlDocument = Utility.BigCXML("DomainRedirects", 1, 600)
			Me.DomainRedirects = Utility.XMLToGenericList(Of DomainRedirect)(oDomainRedirectXML)

			For Each oDomainRedirect As DomainRedirect In Me.DomainRedirects
				If oDomainRedirect.URL.ToLower = sBaseURL Then
					bDomainRedirect = True
					Exit For
				End If
			Next

		End If


		'4.2 check if set by URL content
		Dim sURL301Redirect As String = ""
		If Not oNodeResult.Content Is Nothing Then
			sURL301Redirect = oNodeResult.Content.URL301Redirect
		End If


		'4.3 set 301

		'status
		If bDomainRedirect OrElse sURL301Redirect <> "" Then
			HttpContext.Current.Response.StatusCode = 301
			HttpContext.Current.Response.Status = "301 Moved Permanantly"
		End If

		'location
		If bDomainRedirect AndAlso sURL301Redirect = "" Then
			HttpContext.Current.Response.AddHeader("Location", BookingBase.Params.Domain.TrimEnd("/"c) & identifier)
		ElseIf bDomainRedirect AndAlso sURL301Redirect <> "" Then
			HttpContext.Current.Response.AddHeader("Location", BookingBase.Params.Domain.TrimEnd("/"c) & sURL301Redirect)
		ElseIf sURL301Redirect <> "" Then
			HttpContext.Current.Response.AddHeader("Location", sURL301Redirect)
		End If



		'5. no node, no worky
		If oNodeResult.Node Is Nothing Then
			'return 404 and dump in the needed, error if none set up
			HttpContext.Current.Response.StatusCode = 404
			oNodeResult = GetNode("/404", oThemeXML, oXML)
			If oNodeResult.Node Is Nothing Then
				HttpContext.Current.Response.StatusCode = 500
				Throw New Exception("404 page error - not set up")
			End If
		End If


		'6. build final page definition
		'if we have any url lookup content than add this to the page definition
		Dim oPage As PageDefinition
		oPage = Intuitive.Serializer.DeSerialize(Of PageDefinition)(oNodeResult.Node.OuterXml)
		oPage.Content = oNodeResult.Content

        If oPage.RequiresLogin AndAlso Not BookingBase.LoggedIn Then
			HttpContext.Current.Response.Redirect("/")
		End If

		'7. add any common elements
		'try direct hit first in theme override and default
		Dim oXMLCommonElements As XmlNode = oThemeXML.SelectSingleNode("Pages/CommonElements")

		If oXMLCommonElements Is Nothing Then
			oXMLCommonElements = oXML.SelectSingleNode("Pages/CommonElements")
		End If

		If Not oXMLCommonElements Is Nothing And Not oPage.ExcludeCommonElements Then
			Dim oCommonElements As Intuitive.Web.PageDefinition.CommonElements =
			 Intuitive.Serializer.DeSerialize(Of Intuitive.Web.PageDefinition.CommonElements)(oXMLCommonElements.OuterXml)

			'make sure they are added in the order we put them in the XML - otherwise this is confusing
			oCommonElements.Widgets.Reverse()

			For Each oWidget As PageDefinition.Widget In oCommonElements.Widgets
				'insert at beginning - common elements have higher "rendering priority" per section than other widgets
				oPage.Widgets.Insert(0, oWidget)
			Next
		End If


		Return oPage

	End Function

#End Region


#Region "Get Node"

    Public Function GetNode(sPath As String, oThemeXML As XmlDocument, oXML As XmlDocument, Optional usePageName As Boolean = False) As NodeResult

        Dim oNode As XmlNode
        Dim oContent As CMS.URLContent = Nothing

        Dim sKey As String = "URL"
        If usePageName Then sKey = "PageName"

        'check theme from page def and URL - and only then check base page def
        oNode = Me.GetCorrectNode(oThemeXML, usePageName, sKey, sPath, oContent)

        ' oNode = oThemeXML.SelectSingleNode(String.Format("/Pages/PageDefinition[{0}='{1}']", sKey, sPath))
        If oNode Is Nothing Then
            'oNode = oXML.SelectSingleNode(String.Format("/Pages/PageDefinition[{0}='{1}']", sKey, sPath))
            oNode = Me.GetCorrectNode(oXML, usePageName, sKey, sPath, oContent)
        End If

        Return New NodeResult(oNode, oContent)

    End Function

    'the ByRef is important
    Private Function GetCorrectNode(XML As XmlDocument, usePageName As Boolean, Key As String, Path As String,
                             ByRef Content As CMS.URLContent) As XmlNode

        Dim oNode As XmlNode
        oNode = XML.SelectSingleNode(String.Format("/Pages/PageDefinition[{0}='{1}']", Key, Path))

        '3b. lookup page in the cms
        'dont bother if we are using page name as an identifier, as we have no way to link that to a CMS page
        If Not usePageName Then
            If oNode Is Nothing Then
                Content = CMS.LookupURL(Path)
                If Content.Success Then
                    Dim sXPath As String = String.Format("/Pages/PageDefinition[URL='ObjectType={0}']", Content.ObjectType)
                    oNode = XML.SelectSingleNode(sXPath)
                End If
            End If
        End If

        Return oNode

    End Function

    Public Class NodeResult
        Public Node As XmlNode
        Public Content As CMS.URLContent
        Public Sub New(Node As XmlNode, Content As CMS.URLContent)
            Me.Node = Node
            Me.Content = Content
        End Sub
    End Class

#End Region

#Region "getwidgetreference"

    Public Function GetWidgetReference(WidgetName As String) As Intuitive.Web.Widgets.WidgetBase Implements PageServer.GetWidgetReference


        'if the widget name contains a . assume already namespaced, etc
        'else build up from current project with assemblyname.widgets.classname
        Dim sClassName As String
        Dim oAssembly As Assembly

        If WidgetName.Contains(".") Then
            sClassName = WidgetName
            oAssembly = iVectorWidgetsAssembly
        Else
            sClassName = BaseAssembly.GetName().Name
            sClassName &= ".Widgets"
            sClassName &= "." & WidgetName
            oAssembly = BaseAssembly
        End If

        Try
            Dim oWidget As Intuitive.Web.Widgets.WidgetBase
            oWidget = CType(oAssembly.CreateInstance(sClassName), Intuitive.Web.Widgets.WidgetBase)
            If oWidget Is Nothing Then Throw New Exception("Could not instantiate widget")
            Return oWidget
        Catch ex As Exception
            Throw New Exception("Could not instantiate widget " & sClassName & " (check the namespace!)")
        End Try

    End Function

#End Region

#Region "Support Classes"

	Public Class DomainRedirect
		Public URL As String
	End Class

#End Region


End Class

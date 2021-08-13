'Imports System.ComponentModel
Imports System.Threading
Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports ivc = iVectorConnectInterface

Namespace Widgets

    Public MustInherit Class WidgetBase
        Inherits System.Web.UI.Control

        Public Lookups As Lookups = BookingBase.Lookups

#Region "properties and draw stub"

        Public Property WidgetName As String
        Public Property PageDefinition As PageDefinition
        Public Property FromLibrary As Boolean
        Public Property LocalPath As String
        Public Property URLPath As String
        Public Property RawHTML As String
        Public Property ObjectType As String
        Public Property ObjectID As Integer
        Public Property CacheMinutes As Integer = 0
        Public Property Settings As New Intuitive.Web.PageDefinition.WidgetSettings
        Public Property LoginStatus As LoginState
        Public ExtraParams As New Dictionary(Of String, String)

        Public Function GetSetting(Setting As [Enum]) As String

            'enforces the settings to be set up correctly whilst developing - doesn't need to happen in the wild though
#If DEBUG Then
            If Not Setting.GetType.Name = "eSetting" Then
                'this should be an unhandled exception - we should never pass in an invalid enum and it should complain if we do
                Throw New Exception("invalid setting enum")
            End If
#End If

            Return Me.Settings.GetValue(Setting.ToString)
        End Function

        <Obsolete("Please use GetSetting and pass in an enum instead")>
        Public ReadOnly Property Setting(ByVal Key As String, Optional ByVal MustExist As Boolean = False) As String
            Get
                Return Me.Settings.GetValue(Key, MustExist)
            End Get
        End Property

        Public ReadOnly Property PageURL(Optional ByVal Full As Boolean = False) As String
            Get
                If Not Full Then
                    Return Me.Page.Request.Url.LocalPath
                Else
                    Return Me.Page.Request.Url.AbsoluteUri
                End If
            End Get
        End Property

        Public MustOverride Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)


#End Region


        Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

            writer.WriteLine()
            writer.WriteLine(String.Format("<!-- widget start render {0} -->", Me.GetType.Name))


			Dim watch As Stopwatch = Stopwatch.StartNew()

			Try
                Me.Draw(writer)

			catch ex As ThreadAbortException
				'do nothing being this isn't exception we're just trying to redirect in a widget
            Catch ex As Exception
#If DEBUG Then
                Throw New Exception(ex.ToString)
#Else
				writer.WriteLine(String.Format("<!-- {0} errored during draw method, with this exception: {1} -->", Me.GetType.Name, ex.ToString()))
#End If
            End Try

            watch.Stop()
            Dim widget As New Logging.WidgetSpeed(Me.WidgetName, watch.ElapsedMilliseconds / 1000)
            Logging.Current.WidgetSpeeds.Add(widget)

            writer.WriteLine()
            writer.WriteLine(String.Format("<!-- widget end render {0} -->", Me.GetType.Name))


        End Sub


#Region "support - getwidgetfilepath"

        Public Function GetWidgetFilePathFromExtension(ByVal Extension As String) As String

            If Extension.StartsWith(".") AndAlso Extension.Length > 1 Then
                Extension = Extension.Substring(1)
            End If
            Return Me.LocalPath & Me.WidgetName & "." & Extension
        End Function

#End Region


#Region "XSL Transform"

        Public Sub XSLTransform(oXML As System.Xml.XmlDocument, writer As System.Web.UI.HtmlTextWriter,
         Optional oXSLParams As Intuitive.WebControls.XSL.XSLParams = Nothing)


            '1. Get XSL Path
            Dim sXSLPath As String = Me.GetWidgetFilePathFromExtension("xsl")

            '2. If file exists at XSL Path, then transform, otherwise return empty string
            Dim sHTML As String = ""
            If System.IO.File.Exists(sXSLPath) Then
                If Not oXSLParams Is Nothing Then
                    sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, sXSLPath, oXSLParams)
                Else
                    sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, sXSLPath)
                End If
            End If


            '3 write
            writer.Write(sHTML)

        End Sub

        'BEWARE - if you are calling XSLTransform function here on an ajax call and are using pre translated templates
        ' you must make sure you manually set Me.WidgetName = "xxx" on the concrete widget implementation 
        ' it should have been made MustOverride originally - instead we only rip out of the page def xml

        Public Sub XSLTransform(ByVal oXML As System.Xml.XmlDocument, ByVal sXSL As String,
           ByVal writer As System.Web.UI.HtmlTextWriter, Optional oXSLParams As Intuitive.WebControls.XSL.XSLParams = Nothing)

            Dim sHTML As String = XSLTransform(oXML, sXSL, oXSLParams)
            writer.Write(sHTML)

        End Sub

        Public Function XSLTransform(ByVal oXML As System.Xml.XmlDocument, ByVal sXSL As String,
                                     Optional oXSLParams As Intuitive.WebControls.XSL.XSLParams = Nothing,
                                     Optional Translate As Boolean = False) As String

            Dim html As String = ""

            If BookingBase.Params.PreTranslateTemplates Then
                Dim xslTranslation As New XslTranslation
                Dim newPath As String = xslTranslation.TranslateTemplateFromString(sXSL, Me.WidgetName).TranslatedPath
                html = Intuitive.XMLFunctions.XMLTransformToString(oXML, newPath, oXSLParams)
            Else
                html = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, sXSL, oXSLParams)
                If Translate Then
                    html = Intuitive.Web.Translation.TranslateHTML(html)
                End If
            End If

            Return html

        End Function


        Public Sub XSLPathTransform(ByVal oXML As System.Xml.XmlDocument, ByVal sXSLPath As String,
           ByVal writer As System.Web.UI.HtmlTextWriter, Optional oXSLParams As Intuitive.WebControls.XSL.XSLParams = Nothing)

            Dim sHTML As String = XSLPathTransform(oXML, sXSLPath, oXSLParams)
            writer.Write(sHTML)

        End Sub

        Public Function XSLPathTransform(ByVal oXML As System.Xml.XmlDocument, ByVal sXSLPath As String,
                                         Optional oXSLParams As Intuitive.WebControls.XSL.XSLParams = Nothing,
                                         Optional Translate As Boolean = False) As String

            Dim html As String = ""

            If BookingBase.Params.PreTranslateTemplates Then
                Dim xslTranslation As New XslTranslation
                Dim translatedTemplate As New XslTranslation.TranslatedTemplate
                translatedTemplate = xslTranslation.TranslateTemplateFromPath(sXSLPath)
                Dim xslString As String = Me.SetupTemplate(translatedTemplate.TranslatedPath, translatedTemplate.TranslatedIncludePaths)
                html = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, xslString, oXSLParams)
            Else
                html = Intuitive.XMLFunctions.XMLTransformToString(oXML, sXSLPath, oXSLParams)
                If Translate Then
                    html = Intuitive.Web.Translation.TranslateHTML(html)
                End If
            End If

            Return html

        End Function

#End Region

#Region "Draw control"

        Public Sub DrawControl(ByVal writer As System.Web.UI.HtmlTextWriter, Control As System.Web.UI.Control)

            Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(Control)

            If BookingBase.Params.PreTranslateTemplates Then
                sControlHTML = Intuitive.Web.Translation.TranslateHTML(sControlHTML)
            End If

            writer.Write(sControlHTML)

        End Sub

#End Region

#Region "Combine Xsl Templates"

        Public Function SetupTemplate(TemplatePath As String, TemplateIncludesPaths As List(Of String)) As String

            'get template xml
            Dim oXSLTemplate As New XmlDocument
            oXSLTemplate.PreserveWhitespace = True 'ensures <xsl:text> </xsl:text> is not replaced with a closed node
            oXSLTemplate.Load(TemplatePath)


            'setup xsl namespace
            Dim oNamespaces As New XmlNamespaceManager(oXSLTemplate.NameTable)
            oNamespaces.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform")

            'get root node
            Dim oRootNode As XmlNode = oXSLTemplate.SelectSingleNode("/xsl:stylesheet", oNamespaces)

            'include markdown if set
            For Each XslIncludePath As String In TemplateIncludesPaths
                Dim oIncludeTemplate As New XmlDocument
                oIncludeTemplate.Load(XslIncludePath)
                Dim oNodeList As XmlNodeList = oIncludeTemplate.SelectNodes("//xsl:template", oNamespaces)
                For Each oNode As XmlNode In oNodeList
                    oRootNode.AppendChild(oRootNode.OwnerDocument.ImportNode(oNode, True))
                Next

            Next


            'return
            Return oXSLTemplate.InnerXml

        End Function

#End Region

#Region "Custom Settings"

		'I do not like having to pass in the page server instance from every widget but to be backwards compatible I must do so
		Public Function CustomSettings(Of T)(WidgetSettings As Widgets.iWidgetSettings(Of T),
											 WidgetName As String, PageServer As PageServer) As T

			Dim PageName As String = ""
			Dim ajax As Boolean

			If Not Me.PageDefinition Is Nothing AndAlso Not Me.PageDefinition.PageName Is Nothing Then
				PageName = Me.PageDefinition.PageName
			Else
                'if it is empty we assume this is an ajax request
                PageName = CType(HttpContext.Current.Items("__current_request_pagename"), String)
				ajax = True
			End If

            ' if in cache return 
            Dim sCacheName As String = WidgetName & "_settings_" & PageName

			If Not HttpRuntime.Cache(sCacheName) Is Nothing Then
				Return CType(HttpRuntime.Cache(sCacheName), T)
			End If

            'if ajax and nothing in cache we need to manually get the settings from the page def
            'set up page def + widget
            If ajax Then

				Dim oPageDef As PageDefinition = PageServer.GetPageDefinition(PageName)

				Me.PageDefinition = oPageDef
				Dim oWidget As PageDefinition.Widget = oPageDef.Widgets.Where(Function(o) o.Name = WidgetName).FirstOrDefault
				For Each Setting As PageDefinition.WidgetSetting In oWidget.Settings
					Dim oSetting As New PageDefinition.WidgetSetting
					oSetting.Key = Setting.Key
					oSetting.Value = Setting.Value
					Me.Settings.Add(oSetting)
				Next

			End If

			Dim oSettings As T = WidgetSettings.Setup(Me)

            '3. add to cache and return
            Intuitive.Functions.AddToCache(sCacheName, oSettings, 6000)
            Return oSettings

        End Function

#End Region

#Region "Enums"
        Public Enum LoginState
            None
            LoggedIn
            LoggedOut
        End Enum
#End Region

        Public Class Response

            Public Shared Sub Redirect(URL As String)

                Dim response As HttpResponse = HttpContext.Current.Response
                URL = String.Format("{0}{1}", BookingBase.Params.RootPath, URL)
                response.Redirect(URL)

            End Sub

        End Class


    End Class


    Public Interface iWidgetSettings(Of t)
        Function Setup(Widget As WidgetBase) As t
    End Interface

End Namespace

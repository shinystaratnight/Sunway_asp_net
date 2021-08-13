Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.ComponentModel

Public Class TradeReference
    Inherits WidgetBase

#Region "Properties"

    Private customWidgetSettings As CustomSetting
    Private _xml As XmlDocument

    Public Property XML As XmlDocument
        Get
            If Not Me._xml Is Nothing Then
                Return Me._xml
            Else
                Return New XmlDocument
            End If
        End Get
        Set(value As XmlDocument)
            If Not value Is Nothing Then
                Me._xml = value
            End If
        End Set
    End Property

#End Region

#Region "Render"

    Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)
        Dim sHTML As String = WidgetRender()
        writer.Write(sHTML)
    End Sub


    Public Overridable Sub Setup()

        '1. save xml (empty by default)
        Me.XML = New XmlDocument

        '2. save settings
        Me.customWidgetSettings = New CustomSetting
        Me.customWidgetSettings = Me.CustomSettings(Of CustomSetting)(Me.customWidgetSettings, "TradeReference", New PageServer_Beta())

    End Sub


    Private Function WidgetRender() As String

        '1. run any setup code 
        Me.Setup()

        '2. get params
        Dim oXSLParams As New WebControls.XSL.XSLParams
        '2. transform
        Dim sHTML As String
        If Me.GetSetting(eSetting.TemplateOverride) <> "" Then
            sHTML = Me.XSLPathTransform(Me.XML, HttpContext.Current.Server.MapPath("~" & Me.customWidgetSettings.TemplateOverride), oXSLParams)
        Else
            sHTML = Me.XSLTransform(Me.XML, XSL.SetupTemplate(res.TradeReference, True, False), oXSLParams)
        End If

        '4. return
        Return sHTML

    End Function

#End Region

#Region "Form Functions"

    Public Sub SetBasketTradeReference(ByVal sTradeReference As String)
        BookingBase.Basket.TradeReference = Functions.SafeString(sTradeReference)
    End Sub

#End Region

#Region "Settings"

    Private Class CustomSetting
        Implements iWidgetSettings(Of CustomSetting)

        Public TemplateOverride As String

        Private Function Setup(TradeReference As WidgetBase) As CustomSetting Implements iWidgetSettings(Of CustomSetting).Setup

            Me.TemplateOverride = Functions.SafeString(TradeReference.GetSetting(eSetting.TemplateOverride))

            Return Me

        End Function

    End Class

    Public Enum eSetting

        <Title("Template Override")>
        <Description("Path for XSL override template")>
        TemplateOverride

    End Enum

#End Region

End Class

Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.ComponentModel

Public Class AlternateEmail
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
        Me.customWidgetSettings = Me.CustomSettings(Of CustomSetting)(Me.customWidgetSettings, "AlternateEmail", New PageServer_Beta())

    End Sub


    Private Function WidgetRender() As String

        '1. run any setup code 
        Me.Setup()

        '2. get params
        Dim oXSLParams As WebControls.XSL.XSLParams = Me.XSLParameters()

        '3. transform
        Dim sHTML As String
        If Me.GetSetting(eSetting.TemplateOverride) <> "" Then
            sHTML = Me.XSLPathTransform(Me.XML, HttpContext.Current.Server.MapPath("~" & Me.customWidgetSettings.TemplateOverride), oXSLParams)
        Else
            sHTML = Me.XSLTransform(Me.XML, XSL.SetupTemplate(res.AlternateEmail, True, False), oXSLParams)
        End If

        '4. return
        Return sHTML

    End Function


    Public Function XSLParameters() As WebControls.XSL.XSLParams

        Dim oXSLParams As New WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("TradeContactEmail", Functions.SafeString( _
                      Functions.IIf(BookingBase.Trade.TradeContactID <> 0, BookingTrade.GetTradeContact.Email, BookingBase.Trade.Email)))
        End With

        Return oXSLParams

    End Function

#End Region

#Region "Form Functions"

    Public Sub SetAlternateEmailAddress(ByVal sAlternateEmail As String)
        BookingBase.Basket.LeadCustomer.CustomerEmail = Functions.SafeString(sAlternateEmail)
    End Sub

#End Region

#Region "Settings"

    Private Class CustomSetting
        Implements iWidgetSettings(Of CustomSetting)

        Public TemplateOverride As String

        Private Function Setup(AlternateEmail As WidgetBase) As CustomSetting Implements iWidgetSettings(Of CustomSetting).Setup

            Me.TemplateOverride = Functions.SafeString(AlternateEmail.GetSetting(eSetting.TemplateOverride))

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

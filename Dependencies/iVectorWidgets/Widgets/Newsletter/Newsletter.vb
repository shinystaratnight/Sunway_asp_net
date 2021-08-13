Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel

Public Class Newsletter
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

    Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim sHTML As String = WidgetRender()
        writer.Write(sHTML)
    End Sub

    Public Overridable Sub Setup()

        '1. save settings
        Me.customWidgetSettings = New CustomSetting
        Me.customWidgetSettings = Me.CustomSettings(Of CustomSetting)(Me.customWidgetSettings, "Newsletter", New PageServer_Beta())

        '2. save xml
        Me.XML = Utility.GetTextOverridesXML(Me.customWidgetSettings.TextOverrides)

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
            sHTML = Me.XSLTransform(Me.XML, XSL.SetupTemplate(res.Newsletter, True, False), oXSLParams)
        End If

        '4. return
        Return sHTML

    End Function


    Public Function XSLParameters() As WebControls.XSL.XSLParams

        Dim oXSLParams As New WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("CSSClassOverride", Me.customWidgetSettings.CSSClassOverride)
            .AddParam("InjectContainer", Me.customWidgetSettings.InjectContainer)
            .AddParam("ErrorStyle", Me.customWidgetSettings.ErrorStyle)
        End With

        Return oXSLParams

    End Function

#End Region

#Region "Form Functions"

    Public Shared Function Subscribe(ByVal EmailAddress As String) As Boolean

        Dim oNewsletterSignupReturn As New NewsletterSignupReturn
        oNewsletterSignupReturn.OK = True

        Try
            Dim oNewsletterSignupRequest As New iVectorConnectInterface.NewsletterSignupRequest
            With oNewsletterSignupRequest
                .LoginDetails = BookingBase.IVCLoginDetails
                .LoginDetails.AgentReference = BookingBase.Params.iVectorConnectAgentReference
                .Email = EmailAddress

                'Do the iVectorConnect validation procedure
                Dim oWarnings As Generic.List(Of String) = .Validate()

                If oWarnings.Count > 0 Then
                    oNewsletterSignupReturn.OK = False
                    oNewsletterSignupReturn.Warnings.AddRange(oWarnings)
                End If

            End With

            'If everything is ok then serialise the request to xml
            If oNewsletterSignupReturn.OK Then

                Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
                oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.NewsletterSignupResponse)(oNewsletterSignupRequest)

                Dim oNewsletterSignupResponse As New iVectorConnectInterface.NewsletterSignupResponse

                If oIVCReturn.Success Then

                    oNewsletterSignupResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.NewsletterSignupResponse)

                Else
                    oNewsletterSignupReturn.OK = False
                End If
            End If

        Catch ex As Exception
            oNewsletterSignupReturn.OK = False
            oNewsletterSignupReturn.Warnings.Add(ex.ToString)
        End Try

        Return oNewsletterSignupReturn.OK

    End Function

#End Region

#Region "Support Classes"

    Public Class NewsletterSignupReturn
        Public Property OK As Boolean
        Public Property Warnings As New Generic.List(Of String)
    End Class

#End Region

#Region "Settings"

    Private Class CustomSetting
        Implements iWidgetSettings(Of CustomSetting)

        Public TemplateOverride As String
        Public CSSClassOverride As String
        Public TextOverrides As String
        Public InjectContainer As String
        Public ErrorStyle As String

        Public Function Setup(Newsletter As WidgetBase) As CustomSetting Implements iWidgetSettings(Of CustomSetting).Setup

            Me.TemplateOverride = Functions.SafeString(Newsletter.GetSetting(eSetting.TemplateOverride))
            Me.CSSClassOverride = Functions.SafeString(Newsletter.GetSetting(eSetting.CSSClassOverride))
            Me.TextOverrides = Functions.SafeString(Newsletter.GetSetting(eSetting.TextOverrides))
            Me.InjectContainer = Functions.SafeString(Newsletter.GetSetting(eSetting.InjectContainer))
            Me.ErrorStyle = Functions.SafeString(Newsletter.GetSetting(eSetting.ErrorStyle))

            Return Me

        End Function

    End Class

    Public Enum eSetting

        <Title("XSL Template Override")>
        <DeveloperOnly(True)>
        <Description("Override the XSL template used for this widget")>
        TemplateOverride

        <Title("CSS Class Override")>
        <DeveloperOnly(True)>
        <Description("Overrides Css Class")>
        CSSClassOverride

        <Title("Text Overrides")>
        <Description("Text that controles the header that appears")>
        TextOverrides

        <Title("Inject Container")>
        <DeveloperOnly(True)>
        <Description("Text that decides the div that the widget is injected into")>
        InjectContainer

        <Title("Error Style")>
        <Description("Setting the controls the type of Error message that is displayed")>
        ErrorStyle

    End Enum

#End Region

End Class
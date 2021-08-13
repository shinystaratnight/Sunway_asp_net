Imports Intuitive.Web.Widgets
Imports Intuitive.Web
Imports Intuitive.Functions
Imports Intuitive
Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.ComponentModel

Public Class TermsAndConditions
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

        '1. save settings
        Me.customWidgetSettings = New CustomSetting
        Me.customWidgetSettings = Me.CustomSettings(Of CustomSetting)(Me.customWidgetSettings, "TermsAndConditions", New PageServer_Beta())

        '2. set customer contact flag if opt in by default setting is true
        If Me.customWidgetSettings.OptInByDefault Then Me.SetCustomerContactFlag(True)

        '3a. Get xml from content path
        Me.XML = Utility.BigCXML("TermsAndConditions", 1, 60)

        '3b. Use Booking Terms And Conditions from basket?
        If Me.customWidgetSettings.UseBasketTermsAndConditions Then

            Dim oXMLWrapper As New XmlDocument
            oXMLWrapper.InnerXml = "<TermsAndConditions></TermsAndConditions>"

            'Wrap BigCXML and BasketXML
            Me.XML = Intuitive.XMLFunctions.MergeXMLDocuments(oXMLWrapper, Me.XML, Intuitive.Serializer.Serialize(BookingBase.Basket))

        End If

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
            sHTML = Me.XSLTransform(Me.XML, XSL.SetupTemplate(res.TermsAndConditions, True, False), oXSLParams)
        End If

        '4. return
        Return sHTML

    End Function


    Private Function XSLParameters() As WebControls.XSL.XSLParams

        Dim oXSLParams As New WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("DocumentPath", Me.customWidgetSettings.DocumentPath)
            .AddParam("IAgreeText", Me.customWidgetSettings.IAgreeText)
            .AddParam("CSSClassOverride", Me.customWidgetSettings.CSSClassOverride)
        End With

        Return oXSLParams

    End Function

#End Region

#Region "Form Functions"

    Public Sub SetCustomerContactFlag(OptIn As Boolean)
        BookingBase.Basket.LeadCustomer.ContactCustomer = OptIn
    End Sub

#End Region

#Region "Settings"

    Private Class CustomSetting
        Implements iWidgetSettings(Of CustomSetting)

        Public DocumentPath As String
        Public IAgreeText As String
        Public CSSClassOverride As String
        Public TemplateOverride As String
        Public UseBasketTermsAndConditions As Boolean
        Public OptInByDefault As Boolean

        Public Function Setup(TermsAndConditions As WidgetBase) As CustomSetting Implements iWidgetSettings(Of CustomSetting).Setup

            Me.DocumentPath = Functions.SafeString(TermsAndConditions.GetSetting(eSetting.DocumentPath))
            Me.IAgreeText = Functions.SafeString(TermsAndConditions.GetSetting(eSetting.IAgreeText))
            Me.CSSClassOverride = Functions.SafeString(TermsAndConditions.GetSetting(eSetting.CSSClassOverride))
            Me.TemplateOverride = Functions.SafeString(TermsAndConditions.GetSetting(eSetting.TemplateOverride))
            Me.UseBasketTermsAndConditions = Functions.SafeBoolean(TermsAndConditions.GetSetting(eSetting.UseBasketTermsAndConditions))
            Me.OptInByDefault = Functions.SafeBoolean(TermsAndConditions.GetSetting(eSetting.OptInByDefault))

            Return Me

        End Function

    End Class

    Public Enum eSetting

        <Title("Document Path")>
        <Description("URL to terms and conditions page or document")>
        DocumentPath

        <Title("I Agree Text")>
        <Description("String to override the default I Agree text with")>
        IAgreeText

        <Title("CSS Class Override")>
        <Description("Override the class of the widget's container")>
        CSSClassOverride

        <Title("Template Override")>
        <Description("Path for XSL override template")>
        TemplateOverride

        <Title("Use Basket Terms and Conditions")>
        <Description("Use Terms and Conditions Basket XML")>
        UseBasketTermsAndConditions

        <Title("Opt In By Default")>
        <Description("Automatically confirm that terms and conditions are agreed")>
        OptInByDefault

    End Enum

#End Region

End Class

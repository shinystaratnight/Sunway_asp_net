Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class WaitMessage
	Inherits WidgetBase

    Public Shared Property WaitMessageXML As XmlDocument
        Get
            If Not HttpRuntime.Cache(WaitMessage.XMLCacheKey) Is Nothing Then
                Return CType(HttpRuntime.Cache(WaitMessage.XMLCacheKey), XmlDocument)
            Else
                Return New XmlDocument
            End If
        End Get
        Set(value As XmlDocument)
            HttpRuntime.Cache(WaitMessage.XMLCacheKey) = value
        End Set
    End Property

    Public Shared ReadOnly Property XMLCacheKey As String
        Get
            Return "waitmessage_xml_" & Functions.IIf(Intuitive.Web.BookingBase.LoggedIn, "trade", "public")
        End Get
    End Property

	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("waitmessage_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("waitmessage_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("waitmessage_customsettings") = value
		End Set
	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'Lets set up some custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
            .TemplateOverride = Intuitive.Functions.SafeString(Settings.GetValue("TemplateOverride"))
            .RemoveProtocall = Intuitive.Functions.SafeString(Settings.GetValue("RemoveProtocall"))
            If .RemoveProtocall = "" Then .RemoveProtocall = "True"
        End With

		WaitMessage.CustomSettings = oCustomSettings

		If HttpRuntime.Cache("waitmessage_xml") Is Nothing Then

			'1. Get content path from widget settings
			Dim sObjectType As String = Settings.GetValue("ObjectType")

			'2. Get xml from content path
			Dim oXML As XmlDocument = Intuitive.Web.Utility.BigCXML(sObjectType, 1, 60)

			'3. save to cache
			WaitMessage.WaitMessageXML = oXML

		End If

		Dim bEmbedded As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("Embedded"))
		Dim sType As String = Settings.GetValue("Type")

		If bEmbedded Then

			'Create(params)
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("Type", sType)
				.AddParam("Theme", Intuitive.Web.BookingBase.Params.Theme)
				.AddParam("Website", Intuitive.Web.BookingBase.Params.Website)
				.AddParam("LoggedIn", Intuitive.Web.BookingBase.LoggedIn)
				.AddParam("Embedded", Intuitive.Functions.SafeString(Settings.GetValue("Embedded")))
			End With


			'Transform
			If WaitMessage.CustomSettings.TemplateOverride <> "" Then
				Me.XSLPathTransform(WaitMessage.WaitMessageXML, HttpContext.Current.Server.MapPath("~" & Settings.GetValue("TemplateOverride")), writer, oXSLParams)
			Else
				Me.XSLTransform(WaitMessage.WaitMessageXML, XSL.SetupTemplate(res.WaitMessage, True, True), writer, oXSLParams)
			End If

		Else

			'Create(params)
			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("Type", "Default")
				.AddParam("Theme", Intuitive.Web.BookingBase.Params.Theme)
				.AddParam("Website", Intuitive.Web.BookingBase.Params.Website)
				.AddParam("Hidden", "True")
				.AddParam("LoggedIn", Intuitive.Web.BookingBase.LoggedIn)
				.AddParam("RemoveProtocall", oCustomSettings.RemoveProtocall)
				.AddParam("FirstRender", "True")
			End With

			'Transform
			If WaitMessage.CustomSettings.TemplateOverride <> "" Then
				Me.XSLPathTransform(WaitMessage.WaitMessageXML, HttpContext.Current.Server.MapPath("~" & WaitMessage.CustomSettings.TemplateOverride), writer, oXSLParams)
			Else
				Me.XSLTransform(WaitMessage.WaitMessageXML, XSL.SetupTemplate(res.WaitMessage, True, True), writer, oXSLParams)
			End If

		End If

	End Sub


	Public Overridable Function Show(ByVal Type As String) As String

		Dim eType As WaitMessageType = Intuitive.Functions.SafeEnum(Of WaitMessageType)(Type)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Type", eType.ToString)
			.AddParam("Theme", Intuitive.Web.BookingBase.Params.Theme)
			.AddParam("Website", Intuitive.Web.BookingBase.Params.Website)
			.AddParam("LoggedIn", Intuitive.Web.BookingBase.LoggedIn)
		End With


		Dim sHTML As String

		'Transform
		If WaitMessage.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(WaitMessage.WaitMessageXML, HttpContext.Current.Server.MapPath("~" & WaitMessage.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(WaitMessage.WaitMessageXML, res.WaitMessage, oXSLParams)
		End If

		Return sHTML

	End Function


	'Show Timeout Message, requires type to show the correct message for each WaitMessage type
	Public Overridable Function ShowTimeout(ByVal type As String) As String

		Dim eType As WaitMessageType = Intuitive.Functions.SafeEnum(Of WaitMessageType)(type)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Type", eType.ToString)
			.AddParam("Theme", Intuitive.Web.BookingBase.Params.Theme)
			.AddParam("Website", Intuitive.Web.BookingBase.Params.Website)
		End With

		Dim sHTML As String

		'Set sHTML to use the WaitMessageTimeout.xsl
		sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(WaitMessage.WaitMessageXML, res.WaitMessageTimeout, oXSLParams)

		Return sHTML
	End Function


	Public Class CustomSetting
        Public TemplateOverride As String
        Public RemoveProtocall As String
	End Class

	Public Enum WaitMessageType
		Search
		'choose  is really only for the last page before summary, as we have to wait for extras to search
		Choose
		PreBook
		Book
		Cancellation
        Results
        TransferSearch
    End Enum

End Class

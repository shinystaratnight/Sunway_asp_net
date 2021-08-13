Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml


Public Class HotelPopup
    Inherits WidgetBase

#Region "Properties"

    Public Shared Shadows Property CustomSettings As CustomSetting

        Get
            If HttpContext.Current.Session("hotelPopup_customsettings") IsNot Nothing Then
                Return CType(HttpContext.Current.Session("hotelPopup_customsettings"), CustomSetting)
            End If
            Return New CustomSetting
        End Get
        Set(value As CustomSetting)
            HttpContext.Current.Session("hotelPopup_customsettings") = value
        End Set

    End Property

#End Region

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Set up session variables
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.MapMarkerPath = Functions.SafeString(Settings.GetValue("MapMarkerPath"))
			.EmailLogoName = Functions.SafeString(Settings.GetValue("EmailLogoName"))
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
			.ImageTemplateOverride = Functions.SafeString(Settings.GetValue("ImageTemplateOverride"))
			.EmailTemplateOverride = Functions.SafeString(Settings.GetValue("EmailTemplateOverride"))
			.PerPersonPrice = Functions.SafeBoolean(Settings.GetValue("PerPersonPrice"))
			.PopupClassOverride = Functions.SafeString(Settings.GetValue("PopupClassOverride"))
			.PackagePrice = Functions.SafeBoolean(Settings.GetValue("PackagePrice"))
			.RedirectURL = Functions.SafeString(Settings.GetValue("RedirectURL"))
			.PriceFormat = Functions.SafeString(Settings.GetValue("PriceFormat"))
		End With

		HotelPopup.CustomSettings = oCustomSettings

	End Sub

	Public Shared Function GetGroupsPopupContent(ByVal iPropertyReferenceID As Integer, ByVal iGroupID As Integer, ByVal iMealBasisID As Integer, ByVal sCurrencySymbol As String) As String

		Dim oResultDetailsXML As New XmlDocument

		If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then

			Dim iIndex As Integer = 0
			For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
				If oWorkTableItem.PropertyReferenceID = iPropertyReferenceID Then
					iIndex = oWorkTableItem.Index
					Exit For
				End If
			Next

			oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)
		End If

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("groupid", iGroupID.ToSafeString())
			.AddParam("mealbasisid", iMealBasisID.ToSafeString())
			.AddParam("currencysymbol", sCurrencySymbol)
		    .AddParam("AutoPrebook",BookingBase.Params.AutomaticPrebook)
		End With

		Dim sHTML As String = Intuitive.XMLFunctions.XMLTransformToString(oResultDetailsXML, HttpContext.Current.Server.MapPath("~/Widgets/HotelPopup/HotelGroupPopup.xsl"), oXSLParams)

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)
	End Function

	Public Shared Function GetPopupContent(ByVal iPropertyReferenceID As Integer, Optional ByVal sMode As String = "Content") As String

        Dim oReturn As New HotelPopupReturn

        'get hotel xml
        Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", iPropertyReferenceID, 60)

        'get result details
        Dim oResultDetailsXML As New XmlDocument
        If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then

            Dim iIndex As Integer = 0
            For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
                If oWorkTableItem.PropertyReferenceID = iPropertyReferenceID Then
                    iIndex = oWorkTableItem.Index
                    Exit For
                End If
            Next

            oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)
        End If

        'merge xml
        Dim oXML As XmlDocument = MergeXMLDocuments(oHotelXML, oResultDetailsXML)

        'set up params
        Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("PropertyReferenceID", iPropertyReferenceID)
            .AddParam("MapMarkerPath", HotelPopup.CustomSettings.MapMarkerPath)
            .AddParam("PerPersonPrice", HotelPopup.CustomSettings.PerPersonPrice)
            .AddParam("PopupClassOverride", HotelPopup.CustomSettings.PopupClassOverride)
            .AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
            .AddParam("PackagePrice", HotelPopup.CustomSettings.PackagePrice)
            .AddParam("RedirectURL", HotelPopup.CustomSettings.RedirectURL)
            .AddParam("PriceFormat", HotelPopup.CustomSettings.PriceFormat)
        End With


        'transform
        Dim sHTML As String = ""
        Dim sXSLFile As String = ""

        'check if we're going to render the info popup or the image gallery
        'then check if we have an override template
        If sMode = "Images" Then
            If HotelPopup.CustomSettings.ImageTemplateOverride <> "" Then
                sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelPopup.CustomSettings.ImageTemplateOverride), oXSLParams)
            Else
                sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelPopup_Images, True, True), oXSLParams)
            End If
        Else
            If HotelPopup.CustomSettings.TemplateOverride <> "" Then
                sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelPopup.CustomSettings.TemplateOverride), oXSLParams)
            Else
                sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelPopup, True, True), oXSLParams)
            End If
        End If

        Return Intuitive.Web.Translation.TranslateHTML(sHTML)

    End Function



#Region "Property Email"

    Public Sub EmailProperty(ByVal SenderName As String, ByVal EmailAddress As String, ByVal Message As String,
                             ByVal PropertyReferenceID As Integer)

        Dim xmlSenderEmail As XmlDocument = Utility.BigCXML("WebsiteSettings", 1, 0)
        Dim SenderEmail As String = Intuitive.XMLFunctions.SafeNodeValue(xmlSenderEmail, "B2BEmails/B2BHotelInformationEmail/B2BHotelInformationEmail")
        EmailPropertyWithSender(SenderName, EmailAddress, Message, PropertyReferenceID, SenderEmail)
    End Sub

    Public Sub EmailPropertyWithSender(SenderName As String, EmailAddress As String, Message As String, PropertyReferenceID As Integer, SenderEmail As String)

        'set up xml
        Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", PropertyReferenceID, 0)

        Dim oResultDetailsXML As New XmlDocument
        If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then

            Dim iIndex As Integer = 0
            For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
                If oWorkTableItem.PropertyReferenceID = PropertyReferenceID Then
                    iIndex = oWorkTableItem.Index
                    Exit For
                End If
            Next

            oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)

        End If

        'merge xml
        Dim oXML As XmlDocument = MergeXMLDocuments(oHotelXML, oResultDetailsXML)

        'set up params
        Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("BaseURL", Functions.GetBaseURL)
            .AddParam("Theme", BookingBase.Params.Theme)
            .AddParam("Message", Message)
            .AddParam("SenderName", SenderName)
            .AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("EmailLogoName", HotelPopup.CustomSettings.EmailLogoName)
        End With

        'transform
        Dim sHTML As String = ""
        If HotelPopup.CustomSettings.EmailTemplateOverride <> "" Then
            sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelPopup.CustomSettings.EmailTemplateOverride), oXSLParams)
        Else
            sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.HotelDetailsEmail, True, True), oXSLParams)
        End If

        'set up and send email
        Dim oPropertyEmail As New Intuitive.Email
        With oPropertyEmail
            .SMTPHost = BookingBase.Params.SMTPHost
            .Subject = "Property: " & oXML.SelectSingleNode("/Property/Name").InnerText & " " &
                       oXML.SelectSingleNode("/Property/Resort").InnerText
            .Body = Intuitive.Web.Translation.TranslateHTML(sHTML)
            .From = SenderName
            .FromEmail = SenderEmail
            .EmailTo = EmailAddress

            .SendEmail(True)
        End With
    End Sub

#End Region


    Public Class HotelPopupReturn
        Public PopupHTML As String
    End Class

    Public Class CustomSetting
        Public MapMarkerPath As String
        Public EmailLogoName As String
        Public TemplateOverride As String
        Public ImageTemplateOverride As String
        Public EmailTemplateOverride As String
        Public PerPersonPrice As Boolean
        Public PopupClassOverride As String
        Public PackagePrice As Boolean
        Public RedirectURL As String = ""
        Public PriceFormat As String
    End Class

End Class


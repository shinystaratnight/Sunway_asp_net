
Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions


Public Class HotelEmailDescription
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("hotelEmailDescription_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelEmailDescription_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("hotelEmailDescription_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)


		'1. Set up session variables
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.EmailBodyOverride = Functions.SafeString(Settings.GetValue("EmailBodyOverride"))
		End With

		HotelEmailDescription.CustomSettings = oCustomSettings

	End Sub


	Public Shared Function BuildPopup(ByVal iPropertyReferenceID As Integer) As String

		Dim oReturn As New HotelEmailDescriptionPopupReturn

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

		'set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("PropertyReferenceID", iPropertyReferenceID)
		End With


		'transform
		Dim sHTML As String = Intuitive.XMLFunctions.XMLStringTransformToString(oResultDetailsXML, XSL.SetupTemplate(res.HotelEmailDescription, True, True), oXSLParams)
		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function
	Public Shared Function BuildPrintPopup(ByVal iPropertyReferenceID As Integer) As String

		'set up xml
		Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", iPropertyReferenceID, 0)
		Dim oHeaderXML As XmlDocument = Utility.BigCXML("HeaderLogo", 1, 60)

		Dim iIndex As Integer = 0
		For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
			If oWorkTableItem.PropertyReferenceID = iPropertyReferenceID Then
				iIndex = oWorkTableItem.Index
				Exit For
			End If
		Next


		Dim oTradeXML As New XmlDocument
		'If they are a Trade, get their Trade XML
		If BookingBase.Trade.TradeID > 0 Then
			oTradeXML = Utility.BigCXML("Trade", BookingBase.Trade.TradeID, 30)
		End If


		Dim oResultDetailsXML As New XmlDocument
		If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then
			oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)
		End If

		'merge xml
		Dim oXML As XmlDocument = MergeXMLDocuments(oHotelXML, oResultDetailsXML, oTradeXML, oHeaderXML)

		'set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Duration", BookingBase.SearchDetails.Duration)
			.AddParam("DepartureDate", BookingBase.SearchDetails.DepartureDate)
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("TotalPax", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
			.AddParam("Print", "True")
		End With

		'transform

		If HotelEmailDescription.CustomSettings.EmailBodyOverride <> "" Then
			Dim sHTML As String = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelEmailDescription.CustomSettings.EmailBodyOverride), oXSLParams)
			Return Intuitive.Web.Translation.TranslateHTML(sHTML)
		Else
			Dim sHTML As String = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.EmailBody, True, True), oXSLParams)
			Return Intuitive.Web.Translation.TranslateHTML(sHTML)
		End If

	End Function


#Region "Property Email"

	Public Sub SendEmail(ByVal SenderEmail As String, ByVal EmailAddress As String, ByVal Message As String, _
  ByVal PropertyReferenceID As Integer)

		'set up xml
		Dim oHotelXML As XmlDocument = Utility.BigCXML("Property", PropertyReferenceID, 0)
		Dim oHeaderXML As XmlDocument = Utility.BigCXML("HeaderLogo", 1, 60)


		Dim iIndex As Integer = 0
		For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
			If oWorkTableItem.PropertyReferenceID = PropertyReferenceID Then
				iIndex = oWorkTableItem.Index
				Exit For
			End If
		Next


		Dim oTradeXML As New XmlDocument
		'If they are a Trade, get their Trade XML
		If BookingBase.Trade.TradeID > 0 Then
			oTradeXML = Utility.BigCXML("Trade", BookingBase.Trade.TradeID, 30)
		End If


		Dim oResultDetailsXML As New XmlDocument
		If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then
			oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)
		End If

		'merge xml
		Dim oXML As XmlDocument = MergeXMLDocuments(oHotelXML, oResultDetailsXML, oTradeXML, oHeaderXML)

		'set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Message", Message)
			.AddParam("Duration", BookingBase.SearchDetails.Duration)
			.AddParam("DepartureDate", BookingBase.SearchDetails.DepartureDate)
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("TotalPax", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
			.AddParam("Print", "False")
		End With

		'transform

		Dim sHTML As String = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.EmailBody, True, True), oXSLParams)

		If HotelEmailDescription.CustomSettings.EmailBodyOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & HotelEmailDescription.CustomSettings.EmailBodyOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.EmailBody, True, True), oXSLParams)
		End If

		Dim sSubject As String = "Your Hotel Description"
		If BookingBase.Trade.TradeID > 0 Then
			sSubject = "Your Property Quote"
		End If


		'set up and send email
		Dim oPropertyEmail As New Intuitive.Email
		With oPropertyEmail
			.SMTPHost = BookingBase.Params.SMTPHost
			.Subject = sSubject
			.Body = Intuitive.Web.Translation.TranslateHTML(sHTML)
			.From = BookingBase.Params.Theme
			.FromEmail = SenderEmail
			.EmailTo = EmailAddress

			.SendEmail(True)
		End With


	End Sub

#End Region

	Public Class HotelEmailDescriptionPopupReturn
		Public PopupHTML As String
	End Class

	Public Class CustomSetting
		Public EmailBodyOverride As String
	End Class

End Class
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports Intuitive.XMLFunctions
Imports System.Xml

Public Class PromoCode
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("promocode_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("promocode_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("promocode_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.InfoParagraph = SafeString(Settings.GetValue("InfoParagraph"))
			.Title = SafeString(Settings.GetValue("Title"))
			.InjectTarget = SafeString(Settings.GetValue("InjectTarget"))
			.TemplateOverride = SafeString(Settings.GetValue("TemplateOverride"))
		End With

		PromoCode.CustomSettings = oCustomSettings

		writer.Write(Me.GetHTML())

	End Sub

	Public Function GetHTML() As String


		'template override?
		Dim sTemplateOverride As String = CustomSettings.TemplateOverride

		'1. get basket 
		Dim oBasket As BookingBasket = BookingBase.Basket

		'2. set up xml
		Dim oXML As XmlDocument = oBasket.XML

		'3. set up params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParams()

		'4. transform
		Dim sHTML As String = ""
		If sTemplateOverride <> "" Then
			sHTML = XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & sTemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.PromoCode, True, True), oXSLParams)
		End If

		Return sHTML

	End Function

	Public Function InjectHTML(ByVal sInjectID As String) As String

		Dim oInjectPromoCodeReturn As New InjectPromoCodeReturn
		oInjectPromoCodeReturn.HTML = Me.GetHTML
		oInjectPromoCodeReturn.InjectID = CustomSettings.InjectTarget

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oInjectPromoCodeReturn)

	End Function

	Public Class InjectPromoCodeReturn
		Public HTML As String
		Public InjectID As String
	End Class

	Public Shared Function XSLParams() As Intuitive.WebControls.XSL.XSLParams
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Title", PromoCode.CustomSettings.Title)
			.AddParam("InfoParagraph", PromoCode.CustomSettings.InfoParagraph)
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("InjectTarget", PromoCode.CustomSettings.InjectTarget)
			.AddParam("CurrentDiscount", BookingBase.Basket.PromoCodeDiscount)
		End With

		Return oXSLParams
	End Function

	Public Function ApplyPromoCode(ByVal PromoCode As String) As String

		'create new return object to handle in the javascript
		Dim oApplyPromoCodeReturn As New ApplyPromoCodeReturn

		'apply code
		Dim oPromoCodeReturn As BookingBasket.ApplyPromoCodeReturn = BookingBase.Basket.ApplyPromoCode(PromoCode)

		'save return status
		oApplyPromoCodeReturn.Success = oPromoCodeReturn.OK

		'check if successfull
		If oPromoCodeReturn.OK AndAlso BookingBase.Basket.PromoCodeDiscount > 0 Then
			oApplyPromoCodeReturn.Discount = oPromoCodeReturn.Discount
			oApplyPromoCodeReturn.TotalPriceUnformated = BookingBase.Basket.TotalPrice
		Else
			oApplyPromoCodeReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("PromoCode", "Incorrect code")
		End If

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oApplyPromoCodeReturn)

	End Function

	Public Function PromoCodeWarning(ByVal iVectorWarning As String) As String

		Dim sWarning As String = ""

		If iVectorWarning.StartsWith("The total price of the hotel must not be smaller") _
		 OrElse iVectorWarning.StartsWith("The booking total price must not be smaller than") Then
			sWarning = "Sorry, your booking does not meet the minimum spend requirement of the code."
		ElseIf iVectorWarning = "Invalid Promotional Code setup" OrElse iVectorWarning = "The promotional code is not valid" Then
			sWarning = "Sorry, the code you entered is not valid."
		ElseIf iVectorWarning.Contains("date must be between") OrElse iVectorWarning.Contains("date and time must be between") _
		 OrElse iVectorWarning = "The promotional code has been closed" Then
			sWarning = "Sorry, the code you have entered has expired and is not valid."
		Else
			sWarning = "Sorry, your code is not valid for this property."
		End If

		Return Intuitive.Web.Translation.GetCustomTranslation("PromoCode", sWarning)

	End Function

	Public Shared Function RemovePromoCode() As String

		Dim oRemovePromoCodeReturn As New RemovePromoCodeReturn

		If BookingBase.Basket.PromoCode <> "" Then
			BookingBase.Basket.PromoCode = ""
			BookingBase.Basket.PromoCodeDiscount = 0
		End If

		oRemovePromoCodeReturn.Success = True
		oRemovePromoCodeReturn.TotalPriceUnformated = BookingBase.Basket.TotalPrice

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oRemovePromoCodeReturn)

	End Function

	Public Class CustomSetting
		Public InfoParagraph As String
		Public Title As String
		Public InjectTarget As String
		Public TemplateOverride As String
	End Class

	Public Class ApplyPromoCodeReturn
		Public Success As Boolean
		Public Warning As String = ""
		Public Discount As Decimal = 0
		Public TotalPriceUnformated As Decimal = 0
	End Class

	Public Class RemovePromoCodeReturn
		Public Success As Boolean
		Public TotalPriceUnformated As Decimal = 0
	End Class

End Class

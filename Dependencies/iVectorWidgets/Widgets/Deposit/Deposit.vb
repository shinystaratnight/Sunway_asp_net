Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel

Public Class Deposit
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'0. if amount due today is zero or equal to the basket total we do not want to show deposit option
		'TODO - log if amount due today is larger as something is wrong then
		If BookingBase.Basket.AmountDueToday > BookingBase.Basket.TotalPrice Then
			Dim s As String = ""
		End If

		If BookingBase.Basket.AmountDueToday >= BookingBase.Basket.TotalPrice _
		 OrElse (BookingBase.Basket.AmountDueToday = 0 AndAlso Not SafeBoolean(GetSetting(eSetting.EnableAllowNoPayment))) Then Return

		'1. Get xml from content path
		Dim oXML As XmlDocument = BookingBase.Basket.XML

		'2. Create params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("CSSClassOverride", Intuitive.ToSafeString(GetSetting(eSetting.CSSClassOverride)))
			.AddParam("Text", SafeString(GetSetting(eSetting.Text)))
			.AddParam("InjectDiv", SafeString(GetSetting(eSetting.InjectDiv)))
			.AddParam("UpdateTotalPriceDisplay", SafeBoolean(GetSetting(eSetting.UpdateTotalPriceDisplay)))
			.AddParam("BrandID", BookingBase.Params.BrandID)
			.AddParam("SeparateDecimals", SafeBoolean(GetSetting(eSetting.SeparateDecimals)))
			.AddParam("PriceFormat", IIf(GetSetting(eSetting.PriceFormat) = "", "###,##0.00", GetSetting(eSetting.PriceFormat)))
			.AddParam("AllowNoPayment", Intuitive.Functions.SafeBoolean(BookingBase.Basket.AmountDueToday = 0) AndAlso _
											SafeBoolean(GetSetting(eSetting.EnableAllowNoPayment)))
		End With

		'3. Transform
		'Me.XSLTransform(oXML, XSL.SetupTemplate(res.Deposit, True, False), writer, oXSLParams)
		If Intuitive.ToSafeString(GetSetting(eSetting.TemplateOverride)) <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & Intuitive.ToSafeString(GetSetting(eSetting.TemplateOverride))), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.Deposit, True, False), writer, oXSLParams)
		End If

	End Sub

	Public Shared Sub UpdatePayment(ByVal PayDeposit As Boolean)
		BookingBase.Basket.PayDeposit = PayDeposit
	End Sub

	Public Function AllowNoPayment(ByVal bAllow As Boolean) As Boolean
		BookingBase.Basket.IncludePaymentDetails = Not bAllow
		BookingBase.Basket.AllowNoPayment = bAllow
	End Function

	Public Enum eSetting
		<Title("CSS Class Override")>
		<DeveloperOnly(True)>
		<Description("Overrides Css Class")>
		CSSClassOverride

		<Title("Text")>
		<Description("Text that controles the sub heading paragraph")>
		Text

		<Title("Inject Div")>
		<DeveloperOnly(True)>
		<Description("Text that decides the div that the widget is injected into")>
		InjectDiv

		<Title("Update Total Price Display")>
		<Description("Setting the controls whether the total price gets updated when deposit is selected")>
		UpdateTotalPriceDisplay

		<Title("Template Override")>
		<Description("Template override path")>
		TemplateOverride

		<Title("Price Format")>
		<Description("THe format of the displayed prices e.g. ###,##0.00")>
		PriceFormat

		<Title("Separate Decimals")>
		<Description("Whether or not the decimals in the prices are in a seperate span")>
		SeparateDecimals

		<Title("Enable Allow No Payment")>
		<Description("If true, when the amount due today is 0, render the widget to allow option of continuing booking without paying")>
		EnableAllowNoPayment

	End Enum

End Class

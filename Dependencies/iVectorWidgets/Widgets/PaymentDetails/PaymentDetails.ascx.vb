Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports System.Xml
Imports Intuitive

Public Class PaymentControl
	Inherits UserControlBase

	Public Overrides Sub ApplySettings(ByVal Settings As PageDefinition.WidgetSettings)

		'get settings
		Dim sTitle As String = SafeString(Settings.GetValue("Title"))
		Dim sText As String = SafeString(Settings.GetValue("Text"))
		Dim sCSVTooltipText As String = IIf(Settings.GetValue("CSVTooltipText") = "", "This is the three-digit number found on or above the signature strip on most cards. On American express cards, this is the four-digit number just to the right of your main card number.", Intuitive.Functions.SafeString(Settings.GetValue("CSVTooltipText")))
		Dim sCSVTooltipPosition As String = SafeString(Settings.GetValue("CSVTooltipPosition"))
		Dim bTradeOptionalCard As Boolean = SafeBoolean(Settings.GetValue("TradeOptionalCard"))
        Dim bIsOverbranded As Boolean = SafeBoolean(Settings.GetValue("IsOverbranded"))
		Dim sClassOverride As String = SafeString(Settings.GetValue("CSSClassOverride"))
		Dim bIssueNumber As Boolean = SafeBoolean(Settings.GetValue("IssueNumber"))
		Dim bDisablePriceUpdate As Boolean = SafeBoolean(Settings.GetValue("DisablePriceUpdate"))


		'show payment option box if setting set
		If bTradeOptionalCard AndAlso Not bIsOverbranded Then
			Me.hidPerformValidation.Value = "false"
			Me.divPaymentDetails.Attributes.Add("Style", "display:none;")
		Else
			Me.divPaymentDetails_Options.Visible = False
		End If


		'show/hide issue number field
		Me.dtIssueNumber.Visible = bIssueNumber
		Me.ddIsueNumber.Visible = bIssueNumber


		'disable the card update javscript
		If bDisablePriceUpdate Then
			Me.hidDisablePriceUpdate.Value = "True"
		End If

		'set values of elements if settings exist
		If Not sTitle = "" Then
			Me.hPayment_Title.InnerHtml = sTitle
		End If

		If Not sText = "" Then
			Me.pPayment_Text.InnerHtml = sText
        End If

        If Not sClassOverride = "" Then
            Me.divPaymentDetails.Attributes("class") = sClassOverride
        End If

		Me.aPayment_CSVTooltip.Attributes.Add("onmouseover", "web.Tooltip.Show(this, '" + sCSVTooltipText + "', '" + sCSVTooltipPosition + "');")
		Me.aPayment_CSVTooltip.Attributes.Add("onmouseout", "web.Tooltip.Hide();")

		'Billing Address
		Me.divBillingAddress.Visible = SafeBoolean(Settings.GetValue("UseBillingAddress"))
		Me.hidUseBillingAddress.Value = SafeBoolean(Settings.GetValue("UseBillingAddress")).ToString.ToLower

	End Sub


	Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

		'start date
		Me.ddlCCStartMonth.Options = "01#02#03#04#05#06#07#08#09#10#11#12"
		Dim iFirstStartYear As Integer = Now.Date.Year - 10
		Dim iLastStartYear As Integer = Now.Date.Year

		Dim sbStartYear As New System.Text.StringBuilder

		For i As Integer = iFirstStartYear To iLastStartYear
			sbStartYear.Append(i)
			sbStartYear.Append("#")
		Next

		Me.ddlCCStartYear.Options = Chop(sbStartYear.ToString)



		'expiry date
		Me.ddlCCExpireMonth.Options = "01#02#03#04#05#06#07#08#09#10#11#12"

		Dim iFirstExpiryYear As Integer = Now.Date.Year
		Dim iLastExpiryYear As Integer = Now.Date.Year + 8

		Dim sb As New System.Text.StringBuilder

		For i As Integer = iFirstExpiryYear To iLastExpiryYear
			sb.Append(i)
			sb.Append("#")
		Next

		Me.ddlCCExpireYear.Options = Chop(sb.ToString)


		MyBase.Render(writer)

	End Sub



End Class
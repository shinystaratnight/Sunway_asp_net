Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.IO
Imports System.Configuration.ConfigurationManager


Public Class CompleteBookingControl
	Inherits UserControlBase

	Public Overrides Sub ApplySettings(ByVal Settings As PageDefinition.WidgetSettings)

		'get settings
		Dim sValidationScript As String = Settings.GetValue("ValidationScript")
		Dim sCompleteScript As String = Settings.GetValue("CompleteScript")

		Dim sRedirectURL As String = Settings.GetValue("RedirectURL")
		Dim sBasketSkipURL As String = Settings.GetValue("BasketSkipURL")
		Dim bSuppressBasketSkipURL As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("SuppressBasketSkipURL"))

		If sBasketSkipURL <> "" AndAlso BookingBase.Params.AllowMultipleComponents AndAlso BookingBase.Basket.TotalComponents = 0 AndAlso Not bSuppressBasketSkipURL Then
			sRedirectURL = sBasketSkipURL
		End If

		Dim sFailURL As String = Intuitive.Functions.SafeString(Settings.GetValue("FailURL"))
		Dim bPreBook As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("PreBook"))
		Dim bCompleteBooking As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("CompleteBooking"))
		Dim sTitle As String = Intuitive.Functions.IIf(Settings.GetValue("Title") <> Nothing, Settings.GetValue("Title"), "Complete Booking")
		Dim sMessageText As String = Intuitive.Functions.IIf(Settings.GetValue("MessageText") <> Nothing, Settings.GetValue("MessageText"), "Click below to complete your booking")
		Dim sButtonText As String = Intuitive.Functions.IIf(Settings.GetValue("ButtonText") <> Nothing, Settings.GetValue("ButtonText"), "Continue")
		Dim bButtonOnly As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("ButtonOnly"))
		Dim sInsertWarningAfter As String = Intuitive.Functions.SafeString(Settings.GetValue("InsertWarningAfter"))
		Dim sWarningMessageOverride As String = Intuitive.Functions.SafeString(Settings.GetValue("WarningMessageOverride"))

		'get base url
		Dim sBaseURL As String = Functions.GetBaseURL

		'set base URL to secure if not completing booking, and secure payment page set in config
		If Not bCompleteBooking Then
			sBaseURL = Functions.IIf(CompleteBookingControl.UseSecurePaymentPage, sBaseURL.Replace("http", "https"), sBaseURL)
		End If

		'set redirect URL
		Me.hidRedirectURL.Value = sBaseURL.Chop(1) & sRedirectURL
		Me.hidFailURL.Value = sBaseURL.Chop(1) & sFailURL


		'css class override
		If Settings.GetValue("CSSClassOverride") <> "" Then
			Me.divCompleteBooking.Attributes("class") = Settings.GetValue("CSSClassOverride")
		End If

		'set warning message
		If sWarningMessageOverride <> "" Then
			Me.hidWarningMessage.Value = Intuitive.Web.Translation.GetCustomTranslation("Complete Booking", sWarningMessageOverride)
		Else
			Me.hidWarningMessage.Value = Intuitive.Web.Translation.GetCustomTranslation("Complete Booking", "Please ensure that all required fields are set. Incorrect fields have been highlighted.")
		End If

		'If we want to show the timeout message, set to "true", otherwise set to "false"
		Me.hidShowTimeoutMessage.Value = IIf(Settings.GetValue("ShowTimeoutMessage") <> "", Settings.GetValue("ShowTimeoutMessage").ToLower, "false").ToString

		'add scripts to button
		Me.btnCompleteBooking.Attributes.Add("onclick", "CompleteBooking.ValidateScripts('" + sValidationScript + "', function() { " + sCompleteScript + "});")
		Me.aCompleteBooking_AddComponent.Attributes.Add("onclick", "CompleteBooking.ValidateScripts('" + sValidationScript + "', function() { CompleteBooking.AddComponentSearchShow();int.f.RemoveClass('divSearch', 'hidden'); });")

		'set header, message and button text
		Me.hCompleteHeader.InnerHtml = sTitle
		Me.pText.InnerHtml = sMessageText
		Me.btnCompleteBooking.Value = sButtonText


		'get currency symbol
		Dim sCurrencySymbol As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
		Dim sCurrencyPosition As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)


		'set currency hidden inputs
		Me.hidCompleteBooking_CurrencySymbol.Value = sCurrencySymbol
		Me.hidCompleteBooking_CurrencyPosition.Value = sCurrencyPosition


		'show extra buttons if set
		If Settings.GetValue("ExtraButtons") <> "" Then
			Dim aExtraButtons As String() = Settings.GetValue("ExtraButtons").Split("#"c)

			For Each oExtraButton As String In aExtraButtons
				Dim oElement As Control = Me.FindControl(oExtraButton.Split("|"c)(0))

				'if button exists, sort of the text
				If Not oElement Is Nothing Then

					Dim sText As String = oExtraButton.Split("|"c)(1)

					'if we're dealing with the back button, do some replacing/hiding depending what search mode we're in
					If oElement.ID = "aCompleteBooking_Back" Then
						Dim sHref As String = ""

						Select Case BookingBase.SearchDetails.SearchMode.ToString
							Case "TransferOnly"
								oElement.Visible = False
							Case "FlightOnly"
								sText = sText.Replace("{SearchMode}", "flight")
								sHref = "/flight-results"
							Case Else
								sText = sText.Replace("{SearchMode}", "hotel")
								sHref = "/search-results"
						End Select

						'set href
						CType(oElement, HtmlAnchor).HRef = sHref
					End If

					'set text of element
					CType(oElement, HtmlAnchor).InnerText = sText

				End If
			Next

			'if we've looped through and not found text for a button, hide it
			If Me.aCompleteBooking_AddComponent.InnerText = "" Then Me.aCompleteBooking_AddComponent.Visible = False
			If Me.aCompleteBooking_Back.InnerText = "" Then Me.aCompleteBooking_Back.Visible = False
		Else
			'if no extra buttons set, hide them
			Me.aCompleteBooking_AddComponent.Visible = False
			Me.aCompleteBooking_Back.Visible = False
		End If


		'set total price
		Dim dTotalMarkup As Decimal = Intuitive.Functions.SafeMoney(BookingBase.Basket.TotalMarkup)

		If sCurrencyPosition = "Append" Then
			Me.strTotalPrice.InnerHtml = (BookingBase.Basket.TotalPrice + dTotalMarkup).ToString("#####0.00") + sCurrencySymbol
		Else
			Me.strTotalPrice.InnerHtml = sCurrencySymbol + (BookingBase.Basket.TotalPrice + dTotalMarkup).ToString("#####0.00")
		End If

		'Hide Price if required
		If Intuitive.ToSafeBoolean(Settings.GetValue("HidePrice")) = True Then
			Me.divCompletePrices.Visible = False
		End If



		'set deposit price
		If sCurrencyPosition = "Append" Then
			Me.strDepositPrice.InnerHtml = BookingBase.Basket.AmountDueToday.ToString("#.##") + sCurrencySymbol
		Else
			Me.strDepositPrice.InnerHtml = sCurrencySymbol + BookingBase.Basket.AmountDueToday.ToString("#.##")
		End If

		'set the div the warning message will appear after
		Me.hidInsertWarningAfter.Value = sInsertWarningAfter

	End Sub

	Public Shared Function UseSecurePaymentPage() As Boolean

		Return BookingBase.Params.SecurePaymentPage OrElse Functions.SafeBoolean(AppSettings("SecurePaymentPage"))

	End Function

#Region "render - strip out id/name buggeration"

	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

		'exit sub if nothing in basket and we're on the basket page
		If Me.Request.RawUrl = "/basket" AndAlso BookingBase.Basket.TotalComponents = 0 Then Exit Sub

		'get the render string
		Dim sb As New StringBuilder
		Dim oWriter As HtmlTextWriter = New HtmlTextWriter(New StringWriter(sb))
		MyBase.Render(oWriter)

		'cut the crap IDs out
		Dim sRender As String = sb.ToString.Replace("id=""src_", "id=""")
		sRender = sRender.Replace("name=""src:", "name=""")
		sRender = sRender.Replace(Me.UniqueID & "$", "")
		sRender = sRender.Replace("href=""widgetslibrary/CompleteBooking/#""", "href=""#""")

		writer.Write(sRender)
	End Sub

#End Region

End Class
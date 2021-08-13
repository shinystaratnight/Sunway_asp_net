Imports Intuitive
Imports iw = Intuitive.Web


Public Class LeadGuestDetailsControl
	Inherits UserControlBase


	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

        If Settings.GetValue("CSSClassOverride") <> "" Then
            Me.divLeadGuestDetails.Attributes("class") = Settings.GetValue("CSSClassOverride")
        End If

		If Settings.GetValue("Title") <> "" Then
			Me.h2LeadGuestDetails.InnerHtml = Settings.GetValue("Title")
		End If


		'If the customer has logged, we need to ensure the booking is made against their email.
		If iw.BookingBase.Params.LoggedInCustomersOnly AndAlso iw.BookingBase.SearchBasket.LeadCustomer.CustomerEmail <> "" Then
			Me.txtLeadGuestDetails_Email.Value = iw.BookingBase.SearchBasket.LeadCustomer.CustomerEmail
			Me.txtLeadGuestDetails_Email.Disabled = True
		End If


		'Hide/show post code
		Dim bHidePostcode As Boolean = Functions.SafeBoolean(Settings.GetValue("HidePostcode"))
		Me.trPostCode.Visible = Not bHidePostcode


		'validation exclude
		Dim sValidationExclude As String = Settings.GetValue("ValidationExclude")
		Me.hidLeadGuestDetails_ValidationExclude.Value = sValidationExclude
		Me.UpdateMandatoryLabels(sValidationExclude)


		'get boolean
		Dim bUsePostcodeLookup As Boolean = Functions.SafeBoolean(Settings.GetValue("UsePostcodeLookup"))
		If Not Me.trPostcodeLookup Is Nothing Then
			Me.trPostcodeLookup.Visible = bUsePostcodeLookup
		End If


	End Sub


	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

	End Sub


#Region "Update mandatory labels"

	Public Sub UpdateMandatoryLabels(ByVal ExcludeItems As String)

		'get excluded items
		Dim aExcludeItems As String() = ExcludeItems.Split(","c)


		'loop through each item and remove * if excluded
		For Each sExcludeItem As String In aExcludeItems

			'find control
			Dim sID As String = "lblLeadGuestDetails_" & sExcludeItem
			Dim oElement As Control = Me.FindControl(sID)

			'remove *
			Try

				If Not oElement Is Nothing Then
					If TypeOf oElement Is HtmlTableCell Then
						CType(oElement, HtmlTableCell).InnerText = CType(oElement, HtmlTableCell).InnerText.Replace("*", "")
					End If
				End If

			Catch ex As Exception
			End Try

		Next

	End Sub


#End Region

End Class
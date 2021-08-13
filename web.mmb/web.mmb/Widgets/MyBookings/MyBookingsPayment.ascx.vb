Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions

Public Class MyBookingsPayment
    Inherits iVectorWidgets.UserControlBase


    Public Overrides Sub ApplySettings(ByVal Settings As PageDefinition.WidgetSettings)

        Dim sBookingReference As String = iVectorWidgets.MyBookingsLogin.MyBookingsReference

        Dim oBookingDetails As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(sBookingReference)
        Dim oXML As XmlDocument = Utility.BigCXML("ManageMyBooking", 1, 60)


        'get settings
        Dim sTitle As String = SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/PaymentTitle"))
        Dim sText As String = SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/PaymentText"))
        Dim bIssueNumber As Boolean = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/ShowIssueNumber"))
        Dim sCSVTooltipText As String = IIf(SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/CSVText")) = "", "The 3 digit number on the back of your card.", SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/CSVText")))
        Dim sClassOverride As String = SafeString(Settings.GetValue("CSSClassOverride"))
        Dim bHideStartDate As Boolean = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/HideStartDate"))
        Dim bHideIssueNumber As Boolean = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/HideIssueNumber"))
        Dim bUseOffsitePayment As Boolean = SafeBoolean(Settings.GetValue("UseOffsitePayment"))

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

        'get currency symbol
        Dim sCurrencySymbol As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
        Dim sCurrencyPosition As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)
        Me.hidCurrencySymbol.Value = sCurrencySymbol
        Me.hidCurrencyPosition.Value = sCurrencyPosition

        'set total price
        Me.spnMyBookingsPaymentTotalPrice.InnerHtml = FormatMoney(oBookingDetails.BookingDetails.TotalPrice, sCurrencySymbol, sCurrencyPosition)
        Me.spnMyBookingsPaymentTotalPaid.InnerHtml = FormatMoney(oBookingDetails.BookingDetails.TotalPaid, sCurrencySymbol, sCurrencyPosition)
        Me.spnMyBookingsPaymentTotalOutstanding.InnerHtml = FormatMoney(oBookingDetails.BookingDetails.TotalOutstanding, sCurrencySymbol, sCurrencyPosition)

        Me.spnAddress1.InnerHtml = oBookingDetails.BookingDetails.LeadCustomer.CustomerAddress1
        Me.hidAddress1.Value = oBookingDetails.BookingDetails.LeadCustomer.CustomerAddress1
        If oBookingDetails.BookingDetails.LeadCustomer.CustomerAddress2 <> "" Then
            Me.spnAddress2.InnerHtml = oBookingDetails.BookingDetails.LeadCustomer.CustomerAddress2
            Me.hidAddress2.Value = oBookingDetails.BookingDetails.LeadCustomer.CustomerAddress2
        Else
            Me.spnAddress2.Visible = False
        End If

        Me.spnTownCity.InnerHtml = oBookingDetails.BookingDetails.LeadCustomer.CustomerTownCity
        Me.hidTownCity.Value = oBookingDetails.BookingDetails.LeadCustomer.CustomerTownCity
        Me.spnPostcode.InnerHtml = oBookingDetails.BookingDetails.LeadCustomer.CustomerPostcode
        Me.hidPostcode.Value = oBookingDetails.BookingDetails.LeadCustomer.CustomerPostcode

        Me.hidAmount.Value = SafeString(oBookingDetails.BookingDetails.TotalOutstanding)

        If bUseOffsitePayment Then
            Me.dlCardDetails.Visible = False
            Me.btnPayBalance.InnerText = "Make Payment"
        Else
            Me.hidCSVTooltipText.Value = sCSVTooltipText

            If bHideStartDate Then
                Me.ddStartDate.Style.Add("display", "none")
                Me.dtStartDate.Style.Add("display", "none")
            End If

            If bHideIssueNumber Then
                Me.ddIssueNumber.Style.Add("display", "none")
                Me.dtIssueNumber.Style.Add("display", "none")
            End If
        End If

        Dim sWarningText As String = SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/PaymentWarning"))
        If Not sWarningText = "" Then Me.divPaymentDetailsWarning.InnerHtml = sWarningText

        Dim sSentText As String = SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/PaymentSentText"))
        If Not sSentText = "" Then Me.pPaymentDetailsSent.InnerHtml = sSentText

        Dim sNotSentText As String = SafeString(XMLFunctions.SafeNodeValue(oXML, "ManageMyBooking/PaymentNotSentText"))
        If Not sNotSentText = "" Then Me.pPaymentDetailsNotSent.InnerHtml = sNotSentText

    End Sub

    Public Function FormatMoney(ByVal Price As Decimal, ByVal CurrencySymbol As String, ByVal CurrencySymbolPosition As String) As String
        Dim FormattedPrice As String = ""
        If CurrencySymbolPosition <> "Prepend" Then
            FormattedPrice = SafeString(Price) + " " + CurrencySymbol
        Else
            FormattedPrice = CurrencySymbol + " " + SafeString(Price)
        End If
        Return FormattedPrice
    End Function

    Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

        'start date
        Me._ddlCCStartMonth.Options = "01#02#03#04#05#06#07#08#09#10#11#12"
        Dim iFirstStartYear As Integer = Now.Date.Year - 10
        Dim iLastStartYear As Integer = Now.Date.Year

        Dim sbStartYear As New System.Text.StringBuilder

        For i As Integer = iFirstStartYear To iLastStartYear
            sbStartYear.Append(i)
            sbStartYear.Append("#")
        Next

        Dim sBookingReference As String = iVectorWidgets.MyBookingsLogin.MyBookingsReference
        Dim oBookingDetails As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(sBookingReference)
        Dim dBookingDate As Date = oBookingDetails.BookingDetails.BookingDate

        Dim oCardTypes As XmlDocument = BookingBase.Lookups.LookupXML("CardType", True)
        Dim oLookupPairs As New Lookups.KeyValuePairs

        For Each oCardType As XmlNode In oCardTypes.SelectNodes("/Lookups/CardTypes/CardType")
            Dim dBookingStartDate As Date = Intuitive.DateFunctions.SafeDate(XMLFunctions.SafeNodeValue(oCardType, "BookingDates/BookingDateStart"))
            Dim dBookingEndDate As Date = Intuitive.DateFunctions.SafeDate(XMLFunctions.SafeNodeValue(oCardType, "BookingDates/BookingDateEnd"))

            If DateFunctions.IsEmptyDate(dBookingStartDate) OrElse dBookingDate > dBookingStartDate Then
                If DateFunctions.IsEmptyDate(dBookingEndDate) OrElse dBookingDate < dBookingEndDate Then
                    Dim iCardTypeID As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(oCardType, "CreditCardTypeID"))
                    Dim sCardType As String = Functions.SafeString(XMLFunctions.SafeNodeValue(oCardType, "CreditCardType"))

                    oLookupPairs.Add(iCardTypeID, sCardType)
                End If
            End If
        Next

        Me.ddlCCCardTypeID.KeyValuePairs.Append(oLookupPairs)

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
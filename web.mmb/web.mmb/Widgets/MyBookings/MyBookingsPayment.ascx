<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MyBookingsPayment.ascx.vb" Inherits="Web.MMB.MyBookingsPayment" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive" %>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb" %>

<div id="divPaymentDetails" class="box primary clear" runat="server">

    <div class="boxTitle">
        <h2 id="hPayment_Title" runat="server" ml="Payment">Payment Details</h2>
    </div>

    <div class="form clear" id="divPaymentDetailsForm">
        <p id="pPayment_Text" runat="server" ml="Payment">
            Please enter your payment card details below.
        </p>

        <div id="divPaymentDetailsWarning" class="warning infobox" style="display: none;" runat="server">
            Please ensure that all required fields are set.
        </div>

        <div class="totals">
            <div class="paymentTotal clear">
                <div><span class="payment">Total Holiday Price</span> <span id="spnMyBookingsPaymentTotalPrice" runat="server">0.00</span></div>
                <div><span class="payment">Total Paid</span> <span id="spnMyBookingsPaymentTotalPaid" runat="server">0.00</span></div>
                <div><span class="payment">Total Outstanding</span> <span id="spnMyBookingsPaymentTotalOutstanding" runat="server">0.00</span></div>
            </div>
        </div>

        <dl id="dlCardDetails" runat="server">
            <dt><label>Amount to Pay</label></dt>
            <dd><div class="paymentAmount"><span id="spnAmount"></span><input type="number" id="txtAmount" class="textbox large"/></div></dd>
            <dt><label>Name<span>*</span></label></dt>
            <dd><input type="text" id="txtCCCardHoldersName" name="txtCCCardHoldersName" class="textbox large" /></dd>
            <dt><label>Card Type<span>*</span></label></dt>
            <dd>
                <div class="custom-select large">
                    <web:Dropdown CSSClass="large" ID="ddlCCCardTypeID" Script="MyBookings.CreditCardSelect();" runat="server" />
                </div>
                <span id="spnCreditCardSurcharge" style="display: none;"></span>
            </dd>
            <dt><label>Card Number<span>*</span></label></dt>
            <dd><input type="text" id="txtCCCardNumber" name="txtCCCardNumber" class="textbox large" /></dd>
            <dt><label>Security / CSV<span>*</span></label></dt>
            <dd><input type="text" id="txtCCSecurityCode" name="txtCCSecurityCode" class="textbox tiny" /></dd>
            <dt id="dtStartDate" runat="server"><label>Start Date<span ml="Payment" class="applicable">(if applicable)</span></label></dt>
            <dd id="ddStartDate" runat="server">
                <div class="custom-select small"><int:DropDown ID="ddlCCStartMonth" runat="server" AutoFilter="false" /></div>
                <div class="custom-select small"><int:DropDown ID="ddlCCStartYear" runat="server" AutoFilter="false" /></div>
            </dd>
            <dt><label>Expiry Date<span>*</span></label></dt>
            <dd id="ddExpiryDate"><div class="custom-select small"><int:DropDown ID="ddlCCExpireMonth" runat="server" AutoFilter="false" /></div>
                <div class="custom-select small"><int:DropDown ID="ddlCCExpireYear" runat="server" AutoFilter="false" /></div>
            </dd>
            <dt id="dtIssueNumber" runat="server"><label>Issue Number<span class="applicable" ml="Payment">(if applicable)</span></label></dt>
            <dd id="ddIssueNumber" runat="server"><input type="text" id="txtCCIssueNumber" name="txtCCIssueNumber" class="textbox tiny" /></dd>
        </dl>
        
        <div id="divBillingAddress">
            <label>Billing Address<span>*</span></label>
            <div id="divExistingAddress" class="address-container selected">
                <input type="radio" onclick="MyBookings.ChangeBillingAddress();" name="billingaddress" id="radBillingAddress" checked="checked"/>
                <div class="billing-address">
                    <span id="spnAddress1" runat="server"></span>
                    <span id="spnAddress2" runat="server"></span>
                    <span id="spnTownCity" runat="server"></span>
                    <span id="spnPostcode" runat="server"></span>
                    <int:Hidden ID="hidAddress1" runat="server"/>
                    <int:Hidden ID="hidAddress2" runat="server"/>
                    <int:Hidden ID="hidTownCity" runat="server"/>
                    <int:Hidden ID="hidPostcode" runat="server"/>
                </div>
            </div>

            <div id="divNewAddress" class="address-container">
                <p>Enter Billing Address (if different from above)</p>
                <input type="radio" onclick="MyBookings.ChangeBillingAddress();" name="billingaddress" id="radNewBillingAddress"/>
                <div class="billing-address">
                    <input type="text" id="txtAddress1" class="textbox large" placeholder="Address Line 1"/>
                    <input type="text" id="txtAddress2" class="textbox large" placeholder="Address Line 2"/>
                    <input type="text" id="txtTownCity" class="textbox large" placeholder="Town/City"/>
                    <input type="text" id="txtPostcode" class="textbox small" placeholder="Postcode"/>
                </div>
            </div>
        </div>

        <a class="button primary icon chevron-right balance" id="btnPayBalance" href="javascript:void(0)" runat="server">
            <span>Submit Payment</span>
        </a>

        <int:Hidden ID="hidCSVTooltipText" runat="server" />
        <int:Hidden ID="hidAmount" runat="server" />
        <int:Hidden ID="hidSurcharge" runat="server" Value="0" />
        <int:Hidden ID="hidSurchargePercentage" runat="server" Value="0" />
        <int:Hidden ID="hidCurrencySymbol" runat="server" />
        <int:Hidden ID="hidCurrencyPosition" runat="server" />
    </div>

    <div id="divPaymentDetailsSending" style="display: none;">
        <p id="pPaymentDetailsSending" runat="server" ml="Payment">Please wait as we send your request.</p>
        <img class="spinner" src="/themes/Kenwood/images/loader.gif" alt="loading..." />
    </div>

    <div id="divPaymentDetailsSent" style="display: none;">
        <p id="pPaymentDetailsSent" runat="server" ml="Payment">Payment authorised, thank you.</p>
    </div>

    <div id="divPaymentDetailsNotSent" style="display: none;">
        <p id="pPaymentDetailsNotSent" runat="server" ml="Payment">Sorry, there seems to be a problem with your payment. Try again later.</p>
    </div>

</div>
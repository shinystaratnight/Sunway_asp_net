<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PaymentDetails.ascx.vb" Inherits="iVectorWidgets.PaymentControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>

<div id="divPaymentDetails_Options" class="box primary" runat="server">

	<div class="boxTitle">
		<h2 id="hPaymentOption_Title" runat="server">Payment</h2>
	</div>

	<label class="radioLabel selected" for="radPayByInvoice" onclick="web.CustomInputs.ToggleRadioLabel(this, 'paymentoption');">
		<input type="radio" id="radPayByInvoice" class="radio" name="paymentoption" onclick="int.f.SetValue('hidPerformValidation', 'false');PaymentDetails.SetPaymentType();" checked="checked" />			
		<trans ml="Payment">Pay By Invoice</trans>
	</label>

	<label class="radioLabel" for="radPayByCard" onclick="web.CustomInputs.ToggleRadioLabel(this, 'paymentoption');">
		<input type="radio" id="radPayByCard" class="radio" name="paymentoption" onclick="int.f.SetValue('hidPerformValidation', 'true');PaymentDetails.SetPaymentType();" />			
		<trans ml="Payment">Pay By CreditCard</trans>
	</label>

</div>

<div id="divBillingAddress" class="box primary" runat="server" style="display:none;">

	<div class="boxTitle">
		<h2 id="h1" runat="server">Billing Address</h2>
	</div>

	<input type="hidden" id="hidUseBillingAddress" runat="server" value="False" />

	<div class="form">

		<fieldset>
			<dl>		
				<dt><label><trans ml="Payment">Title</trans> <span>*</span></label></dt>
				<dd><web:dropdown id="ddlBillingAddress_Title" options="Mr|Mr#Mrs|Mrs#Miss|Miss#Ms|Ms" runat="server" addblank="true" overrideblanktext="Title..." multilingualtag="Payment Details;Title" /></dd>
			
				<dt><label><trans ml="Payment">First Name</trans> <span>*</span></label></dt>
				<dd><input type="text" id="txtBillingAddress_FirstName" name="txtBillingAddress_FirstName" class="large textbox" /></dd>
			
				<dt><label><trans ml="Payment">Last Name</trans> <span>*</span></label></dt>
				<dd><input type="text" id="txtBillingAddress_LastName" name="txtBillingAddress_LastName" class="large textbox" /></dd>

				<dt><label><trans ml="Payment">Address</trans> <span>*</span></label></dt>
				<dd><input type="text" id="txtBillingAddress_Address" name="txtBillingAddress_Address" class="large textbox" /></dd>

				<dt><label><trans ml="Payment">City</trans> <span>*</span></label></dt>
				<dd><input type="text" id="txtBillingAddress_City" name="txtBillingAddress_City" class="small textbox" /></dd>

				<dt><label><trans ml="Payment">Postcode</trans> <span>*</span></label></dt>
				<dd><input type="text" id="txtBillingAddress_Postcode" name="txtBillingAddress_Postcode" class="tiny textbox" /></dd>

				<dt><label><trans ml="Payment">Country</trans> <span>*</span></label></dt>
				<dd><web:dropdown id="ddlBillingAddress_BookingCountry" cssclass="large" addblank="true" lookup="BookingCountry" overrideblanktext="Country..." runat="server" multilingualtag="Paymentt Details;Country" /></dd>
	
			</dl>
		</fieldset>

	</div>

</div>

<div id="divPaymentDetails" class="box primary" runat="server">	

	<div class="boxTitle">
		<h2 id="hPayment_Title" runat="server">Payment Details</h2>
	</div>

	<input type="hidden" id="hidDisablePriceUpdate" runat="server" value="False" />

	<p id="pPayment_Text" runat="server" ml="Payment">Please enter your payment card details below.</p>

	<div class="form">
	
		<fieldset>
			<dl>
				<dt><label><trans ml="Payment">Card Type</trans><span>*</span></label></dt>
				<dd>
					<web:dropdown ID="ddlCCCardTypeID" lookup="CardType" script="PaymentDetails.CreditCardSelect();"  runat="server"/>
				</dd>
				<dt><label><trans ml="Payment">Card Number</trans><span>*</span></label></dt>
				<dd><input type="text" id="txtCCCardNumber" name="txtCCCardNumber" class="textbox" /></dd>
				
				<dt><label><trans ml="Payment">Name On Card</trans><span>*</span></label></dt>
				<dd><input type="text" id="txtCCCardHoldersName" name="txtCCCardHoldersName" class="textbox" /></dd>				
				
				<dt><label><trans ml="Payment">Start Date</trans> <span class="applicable">(if applicable)</span></label></dt>
				<dd>
					<int:dropdown ID="ddlCCStartMonth" runat="server" autofilter="false" />
					<int:dropdown ID="ddlCCStartYear" runat="server" autofilter="false" />
				</dd>
				
				<dt><label><trans ml="Payment">Expiry Date</trans><span>*</span></label></dt>
				<dd>
					<int:dropdown ID="ddlCCExpireMonth" runat="server" autofilter="false" />
					<int:dropdown ID="ddlCCExpireYear" runat="server" autofilter="false" />
				</dd>
				
				<dt id="dtSecruityCode"><label><trans ml="Payment">Security / CSV</trans><span>*</span></label></dt>
				<dd>
					<input type="text" id="txtCCSecurityCode" name="txtCCSecurityCode" class="textbox tiny" />
					<a id="aPayment_CSVTooltip" class="tooltip" href="javascript:void()" ml="Payment" runat="server">What is this?</a>
				</dd>
				
				<dt id="dtIssueNumber" runat="server"><label ml="Payment">Issue Number <span class="applicable">(if applicable)</span></label></dt>
				<dd id="ddIsueNumber" runat="server">
					<input type="text" id="txtCCIssueNumber" name="txtCCIssueNumber" class="textbox tiny" />
				</dd>
			</dl>
		</fieldset>
		
		<int:hidden ID="hidSurcharge" runat="server" />
		<int:hidden ID="hidTotalAmount" runat="server" />
		<int:hidden ID="hidCurrencySymbol" runat="server" />

	</div>
</div>

<input type="hidden" id="hidPerformValidation" value="true" runat="server" />
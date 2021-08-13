<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CompleteBooking.ascx.vb" Inherits="iVectorWidgets.CompleteBookingControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>

<div id="divCompleteBooking" class="box primary clear" runat="server">

	<div class="boxTitle">
		<h2 id="hCompleteHeader" runat="server" ml="Complete Booking">Complete Booking</h2>
	</div>

	<p id="pText" runat="server" ml="Complete Booking">Click below to complete your booking</p>

	<div id="divCompletePrices"  runat="server">
		<h3><trans ml="Complete Booking">Total Price</trans> <strong id="strTotalPrice" runat="server"></strong><span ml="Complete Booking"> (including all charges and fees)</span></h3>
		<h4 id="hAmountDueToday" style="display:none"><trans ml="Complete Booking">Amount Due Today</trans><strong id="strDepositPrice" runat="server"></strong></h4>
	</div>

	<a id="aCompleteBooking_Back" class="button primary" runat="server" ml="Complete Booking"></a>
	<a id="aCompleteBooking_AddComponent" class="button primary" runat="server" ml="Complete Booking"></a>

	<input id="btnCompleteBooking" type="button" class="button primary" value="Continue" runat="server" ml="Complete Booking" />

	<input type="hidden" id="hidFailURL" runat="server" />
	<input type="hidden" id="hidRedirectURL" runat="server" />
	<input type="hidden" id="hidCompleteBooking_CurrencySymbol" runat="server" />
	<input type="hidden" id="hidCompleteBooking_CurrencyPosition" runat="server" />
	<input type="hidden" id="hidInsertWarningAfter" runat="server" />
	<input type="hidden" id="hidWarningMessage" runat="server" />
	<input type="hidden" id="hidShowTimeoutMessage" runat="server" />
</div>
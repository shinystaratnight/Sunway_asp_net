<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="BookingReference"/>

	<xsl:template match="/">

		<input type="hidden" id="hidEmailBookingReference"/>

		<div id="divSendDocumentationForm" class="box primary">

			<div class="boxTitle">
				<h2 ml="Trade Bookings">Email documents</h2>
			</div>

			<p ml="Trade Bookings">Please specify an email below if you wish for your documents to be sent to a different address</p>
			<label for="txtOverrideEmail" ml="Trade Bookings">Alternative Email (optional):</label>
			<input type="text" id="txtOverrideEmail" class="textbox"/>

			<div id="divButtons">
				<a class="button primary" id="aEmailVoucher" href="javascript:TradeBookings.SendDocumentation('{$BookingReference}', 'Client Vouchers Only')" ml="Trade Bookings">Email Voucher</a>
				<a class="button primary" id="aEmailInvoice" href="javascript:TradeBookings.SendDocumentation('{$BookingReference}', 'Trade Documentation Only')" ml="Trade Bookings">Email Invoice</a>
			</div>			

			<div class="clearing">
				<xsl:text> </xsl:text>
			</div>

		</div>

		<p id="pEmailDocumentationRequestThankyou" style="display:none;" ml="Trade Bookings">Your documentation has been sent.</p>
		<p id="pEmailDocumentationRequestError" style="display:none;" ml="Trade Bookings">Sorry there was a problem processing your request.</p>

	</xsl:template>

</xsl:stylesheet>




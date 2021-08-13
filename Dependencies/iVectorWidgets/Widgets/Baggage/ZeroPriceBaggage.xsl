<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>

	<xsl:param name="Title" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="PriceFormat" />

	<xsl:template match="/">

		<div id="divBaggage" class="box primary">

			<input type="hidden" id="hidBaggagePax" name="hidBaggagePax" value="{/BookingBasket/TotalAdults + /BookingBasket/TotalChildren}" />
			<input type="hidden" id="hidBaggage_Invalid" value="Unable to book more bags than there are passengers" ml="Transfers" runat="server" />

			<div class="boxTitle">
				<h2 ml="Baggage">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Baggage</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
			</div>


			<p class="disclaimer" ml="Baggage">* Luggage allowance will vary between 15-22kgs per bag. Check your confirmation for your exact allowance.</p>
			<table class="def">

				<tr>
					<th ml="Baggage">
						<xsl:text>Description</xsl:text>
					</th>
					<th ml="Baggage">
						<xsl:text>Quantity</xsl:text>
					</th>
					<th ml="Baggage">
						<xsl:text>Price</xsl:text>
					</th>
					<th ml="Baggage">
						<xsl:text>Total</xsl:text>
					</th>
				</tr>

				
				<!-- Free baggage -->
				<tr>
					<td>
						<xsl:text>Included</xsl:text>
					</td>
					<td>
						<xsl:value-of select="BookingBasket/BasketFlights/BasketFlight/BasketFlightExtras/BasketFlightExtra[ExtraType='Baggage']/QuantitySelected"/>
					</td>
					<td>
						<xsl:text>FREE</xsl:text>
					</td>
					<td>
						<xsl:text>FREE</xsl:text>
					</td>
				</tr>

			</table>

		</div>

	</xsl:template>

</xsl:stylesheet>
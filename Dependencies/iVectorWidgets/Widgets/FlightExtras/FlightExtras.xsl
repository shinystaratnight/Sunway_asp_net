<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>

	<xsl:param name="Title"/>
	<xsl:param name="CurrencySymbol"/>
	<xsl:param name="CurrencySymbolPosition"/>
	<xsl:param name="PriceFormat"/>
	<xsl:param name="BaggageQuantityError"/>
	<xsl:param name="ExtraQuantityError"/>

	<xsl:template match="/">

		<div id="divFlightExtras" class="box primary">

			<input type="hidden" id="hidFlightExtraPax" name="hidFlightExtraPax"
			       value="{/BookingBasket/TotalAdults + /BookingBasket/TotalChildren}"/>
			<input type="hidden" id="hidFlightExtra_Invalid" value="{$ExtraQuantityError}" runat="server"/>
			<input type="hidden" id="hidFlightExtra_InvalidBaggage" value="{$BaggageQuantityError}" runat="server"/>

			<div class="boxTitle">
				<h2 ml="Flight Extras">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Baggage and Flight extras</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
				<xsl:if test="BookingBasket/TextOverrides/SubTitle != ''">
					<p ml="Flight Extras">
						<xsl:value-of select="BookingBasket/TextOverrides/SubTitle"/>
					</p>
				</xsl:if>
			</div>


			<p class="disclaimer" ml="Flight Extras">* Luggage allowance will vary. Check your confirmation for your exact allowance.</p>
			<table class="def">


				<xsl:for-each select="BookingBasket/BasketFlights/BasketFlight">
					<xsl:if
						test="Flight/ReturnMultiCarrierDetails/BookingToken and Flight/ReturnMultiCarrierDetails/BookingToken != ''">
						<tr>
							<td colspan="4" class="flightExtraHeaders">
								<h3>Outbound Flight Baggage and Extras</h3>
							</td>
						</tr>
					</xsl:if>
					<xsl:call-template name="BaggageAndExtraHeaders"/>
					<xsl:for-each select="BasketFlightExtras/BasketFlightExtra">

						<xsl:call-template name="BaggageAndExtrasContent">
							<xsl:with-param name="Return" select="'false'"/>
						</xsl:call-template>

					</xsl:for-each>
					<xsl:if
						test="Flight/ReturnMultiCarrierDetails/BookingToken and Flight/ReturnMultiCarrierDetails/BookingToken != ''">
						<tr>
							<td colspan="4" class="flightExtraHeaders">
								<h3>Return Flight Baggage and Extras</h3>
							</td>
						</tr>

						<xsl:call-template name="BaggageAndExtraHeaders"/>

						<xsl:for-each select="ReturnMultiCarrierFlightExtras/BasketFlightExtra">
							<xsl:call-template name="BaggageAndExtrasContent">
								<xsl:with-param name="Return" select="'true'"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:if>
				</xsl:for-each>
			</table>

		</div>

	</xsl:template>

	<xsl:template name="BaggageAndExtraHeaders">
		<tr>
			<th ml="Flight Extras">
				<xsl:choose>
					<xsl:when test="BookingBasket/TextOverrides/Description != ''">
						<xsl:value-of select="BookingBasket/TextOverrides/Description"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Description</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</th>
			<th ml="Flight Extras">
				<xsl:choose>
					<xsl:when test="BookingBasket/TextOverrides/Quantity != ''">
						<xsl:value-of select="BookingBasket/TextOverrides/Quantity"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Quantity</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</th>
			<th ml="Flight Extras">
				<xsl:choose>
					<xsl:when test="BookingBasket/TextOverrides/Price != ''">
						<xsl:value-of select="BookingBasket/TextOverrides/Price"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Price</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</th>
			<th ml="Flight Extras">
				<xsl:choose>
					<xsl:when test="BookingBasket/TextOverrides/Total != ''">
						<xsl:value-of select="BookingBasket/TextOverrides/Total"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Total</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</th>
		</tr>
	</xsl:template>

	<xsl:template name="BaggageAndExtrasContent">
		<xsl:param name="Return" select="'false'"/>

		<xsl:variable name="ReturnString">
			<xsl:choose>
				<xsl:when test="$Return = 'true'">
					<xsl:text>Return</xsl:text>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>

		<input type="hidden" name="hid{$ReturnString}FlightExtraType" value="{ExtraType}">
			<xsl:attribute name="id">
				<xsl:choose>
					<xsl:when test="$Return = 'true'">
						<xsl:text>hidReturnFlightExtraType_</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>hidFlightExtraType_</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:value-of select="position()"/>
			</xsl:attribute>
		</input>

		<tr>
			<td>
				<xsl:value-of select="Description"/>
				<input type="hidden" name="hid{$ReturnString}FlightExtraCost" value="{ExtraBookingToken}">
					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$Return = 'true'">
								<xsl:text>hidReturnFlightExtraToken_</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>hidFlightExtraToken_</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="position()"/>
					</xsl:attribute>
				</input>
			</td>
			<td class="quantity">
				<xsl:variable name="TotalPax" select="/BookingBasket/TotalAdults + /BookingBasket/TotalChildren"/>
				<xsl:variable name="MaxQuantity">
					<xsl:choose>
						<!-- Per Booking applies to ALL PAX -->
						<xsl:when test="CostingBasis = 'Per Booking'">
							<xsl:value-of select="1"/>
						</xsl:when>
						<!-- Per Passenger applies to Adults and Children up to a maximum of the Quantity Available -->
						<xsl:when test="CostingBasis = 'Per Passenger'">
							<xsl:choose>
								<xsl:when test="$TotalPax &gt; QuantityAvailable and QuantityAvailable &gt; 0">
									<xsl:value-of select="QuantityAvailable"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$TotalPax"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<!-- A costing basis of None is from 0 to the QuantityAvailable -->
						<xsl:when test="CostingBasis = 'None'">
							<xsl:value-of select="QuantityAvailable"/>
						</xsl:when>
					</xsl:choose>
				</xsl:variable>
				<select onchange="FlightExtras.Update{$ReturnString}();return(false);">

					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$Return = 'true'">
								<xsl:text>ddlReturnFlightExtraQuantity_</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>ddlFlightExtraQuantity_</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="position()"/>
					</xsl:attribute>

					<xsl:call-template name="CreateNumericDropDown">
						<xsl:with-param name="MaxValue" select="$MaxQuantity"/>
						<xsl:with-param name="Selected" select="QuantitySelected"/>
					</xsl:call-template>

					<input type="hidden" name="hid{$ReturnString}FlightExtraQuant">
						<xsl:attribute name="id">
							<xsl:choose>
								<xsl:when test="$Return = 'true'">
									<xsl:text>hidReturnFlightExtraQuant_</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>hidFlightExtraQuant_</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
							<xsl:value-of select="position()"/>
						</xsl:attribute>
					</input>

				</select>
			</td>
			<td>
				<xsl:call-template name="GetSellingPrice">
					<xsl:with-param name="Value" select="Price"/>
					<xsl:with-param name="Format" select="$PriceFormat"/>
					<xsl:with-param name="Currency" select="$CurrencySymbol"/>
					<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
				</xsl:call-template>
				<input type="hidden" name="hid{$ReturnString}FlightExtraCost" value="{Price}">
					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$Return = 'true'">
								<xsl:text>hidReturnFlightExtraCost_</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>hidFlightExtraCost_</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="position()"/>
					</xsl:attribute>
				</input>
			</td>
			<td>
				<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
					<xsl:value-of select="$CurrencySymbol"/>
				</xsl:if>
				<span>
					<xsl:attribute name="id">
						<xsl:choose>
							<xsl:when test="$Return = 'true'">
								<xsl:text>spReturnFlightExtraTotal_</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>spFlightExtraTotal_</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:value-of select="position()"/>
					</xsl:attribute>
					<xsl:value-of select="format-number(Price * QuantitySelected, $PriceFormat)"/>
				</span>
				<xsl:if test="$CurrencySymbolPosition = 'Append'">
					<xsl:value-of select="$CurrencySymbol"/>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="CreateNumericDropDown">
		<xsl:param name="Value" select="0"/>
		<xsl:param name="MaxValue"/>
		<xsl:param name="Selected"/>

		<option>
			<xsl:attribute name="value">
				<xsl:value-of select="$Value"/>
			</xsl:attribute>

			<xsl:if test="$Value = $Selected">
				<xsl:attribute name="selected">
					<xsl:text>selected</xsl:text>
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$Value"/>
			<xsl:text> </xsl:text>
		</option>

		<xsl:choose>
			<xsl:when test="$Value &lt; $MaxValue">
				<xsl:call-template name="CreateNumericDropDown">
					<xsl:with-param name="Value" select="$Value + 1"/>
					<xsl:with-param name="MaxValue" select="$MaxValue"/>
					<xsl:with-param name="Selected" select="$Selected"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>

			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

</xsl:stylesheet>
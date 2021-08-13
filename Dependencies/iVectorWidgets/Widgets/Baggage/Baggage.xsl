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
				<xsl:if test="BookingBasket/TextOverrides/SubTitle != ''">
					<p ml="Baggage">
						<xsl:value-of select="BookingBasket/TextOverrides/SubTitle"/>
					</p>
				</xsl:if>
			</div>


			
			<p class="disclaimer" ml="Baggage">* Luggage allowance will vary. Check your confirmation for your exact allowance.</p>
			<table class="def">

				<tr>
					<th ml="Baggage">
						<xsl:choose>
							<xsl:when test="BookingBasket/TextOverrides/Description != ''">
								<xsl:value-of select="BookingBasket/TextOverrides/Description"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Description</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th ml="Baggage">
						<xsl:choose>
							<xsl:when test="BookingBasket/TextOverrides/Quantity != ''">
								<xsl:value-of select="BookingBasket/TextOverrides/Quantity"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Quantity</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th ml="Baggage">
						<xsl:choose>
							<xsl:when test="BookingBasket/TextOverrides/Price != ''">
								<xsl:value-of select="BookingBasket/TextOverrides/Price"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Price</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th ml="Baggage">
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

        <xsl:variable name="TotalPax" select="BookingBasket/TotalAdults + BookingBasket/TotalChildren" />

				<xsl:for-each select="BookingBasket/BasketFlights/BasketFlight">

          <xsl:if test="Flight/IncludedBaggageAllowance &gt; 0">

            <tr>
              <td>
                <xsl:value-of select="Flight/IncludedBaggageText"/>
              </td>
              <td class="quantity">
                <xsl:value-of select="$TotalPax"/>
              </td>
              <td>
                <xsl:text>INCLUDED</xsl:text>
              </td>
              <td>
                <xsl:if test="$CurrencySymbolPosition = 'Prepend'">
                  <xsl:value-of select="$CurrencySymbol"/>
                </xsl:if>
                <span>
                  <xsl:value-of select="format-number(0, $PriceFormat)"/>
                </span>
                <xsl:if test="$CurrencySymbolPosition = 'Append'">
                  <xsl:value-of select="$CurrencySymbol"/>
                </xsl:if>
              </td>
            </tr>

          </xsl:if>
          
					<xsl:for-each select="BasketFlightExtras/BasketFlightExtra[ExtraType='Baggage']">

						<tr>
							<td>
								<xsl:value-of select="Description"/>
								<input type="hidden" name="hidBaggageCost" value="{ExtraBookingToken}">
									<xsl:attribute name ="id">
										<xsl:text>hidBaggageToken_</xsl:text>
										<xsl:value-of select ="position()"/>
									</xsl:attribute>
								</input>
							</td>
							<td class="quantity">
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
								<select onchange="Baggage.Update();return(false);">

									<xsl:attribute name ="id">
										<xsl:text>ddlBaggageQuantity_</xsl:text>
										<xsl:value-of select ="position()"/>
									</xsl:attribute>

									<xsl:call-template name="CreateNumericDropDown">
										<xsl:with-param name="MaxValue" select="$MaxQuantity" />
										<xsl:with-param name="Selected" select="QuantitySelected" />
									</xsl:call-template>

									<input type="hidden" name="hidBaggageQuant">
										<xsl:attribute name ="id">
											<xsl:text>hidBaggageQuant_</xsl:text>
											<xsl:value-of select ="position()"/>
										</xsl:attribute>
									</input>

								</select>
							</td>
							<td>
								<xsl:call-template name="GetSellingPrice">
									<xsl:with-param name="Value" select="Price" />
									<xsl:with-param name="Format" select="$PriceFormat" />
									<xsl:with-param name="Currency" select="$CurrencySymbol" />
									<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
								</xsl:call-template>
								<input type="hidden" name="hidBaggageCost" value="{Price}">
									<xsl:attribute name ="id">
										<xsl:text>hidBaggageCost_</xsl:text>
										<xsl:value-of select ="position()"/>
									</xsl:attribute>
								</input>
							</td>
							<td>
								<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>
								<span>
									<xsl:attribute name ="id">
										<xsl:text>spBaggageTotal_</xsl:text>
										<xsl:value-of select ="position()"/>
									</xsl:attribute>
									<xsl:value-of select="format-number(Price * QuantitySelected, $PriceFormat)"/>
								</span>
								<xsl:if test="$CurrencySymbolPosition = 'Append'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>
							</td>
						</tr>

					</xsl:for-each>
                  <xsl:for-each select="ReturnMultiCarrierFlightExtras/BasketFlightExtra[ExtraType='Baggage']">

                    <tr>
                      <td>
                        <xsl:value-of select="Description"/>
                        <input type="hidden" name="hidBaggageCost" value="{ExtraBookingToken}">
                          <xsl:attribute name ="id">
                            <xsl:text>hidBaggageToken_</xsl:text>
                            <xsl:value-of select ="position()"/>
                          </xsl:attribute>
                        </input>
                      </td>
                      <td class="quantity">
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
                        <select onchange="Baggage.Update();return(false);">

                          <xsl:attribute name ="id">
                            <xsl:text>ddlBaggageQuantity_</xsl:text>
                            <xsl:value-of select ="position()"/>
                          </xsl:attribute>

                          <xsl:call-template name="CreateNumericDropDown">
                            <xsl:with-param name="MaxValue" select="$MaxQuantity" />
                            <xsl:with-param name="Selected" select="QuantitySelected" />
                          </xsl:call-template>

                          <input type="hidden" name="hidBaggageQuant">
                            <xsl:attribute name ="id">
                              <xsl:text>hidBaggageQuant_</xsl:text>
                              <xsl:value-of select ="position()"/>
                            </xsl:attribute>
                          </input>

                        </select>
                      </td>
                      <td>
                        <xsl:call-template name="GetSellingPrice">
                          <xsl:with-param name="Value" select="Price" />
                          <xsl:with-param name="Format" select="$PriceFormat" />
                          <xsl:with-param name="Currency" select="$CurrencySymbol" />
                          <xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
                        </xsl:call-template>
                        <input type="hidden" name="hidBaggageCost" value="{Price}">
                          <xsl:attribute name ="id">
                            <xsl:text>hidBaggageCost_</xsl:text>
                            <xsl:value-of select ="position()"/>
                          </xsl:attribute>
                        </input>
                      </td>
                      <td>
                        <xsl:if test="$CurrencySymbolPosition = 'Prepend'">
                          <xsl:value-of select="$CurrencySymbol"/>
                        </xsl:if>
                        <span>
                          <xsl:attribute name ="id">
                            <xsl:text>spBaggageTotal_</xsl:text>
                            <xsl:value-of select ="position()"/>
                          </xsl:attribute>
                          <xsl:value-of select="format-number(Price * QuantitySelected, $PriceFormat)"/>
                        </span>
                        <xsl:if test="$CurrencySymbolPosition = 'Append'">
                          <xsl:value-of select="$CurrencySymbol"/>
                        </xsl:if>
                      </td>
                    </tr>

                  </xsl:for-each>
				</xsl:for-each>

			</table>

		</div>

	</xsl:template>


	<xsl:template name="CreateNumericDropDown">
		<xsl:param name="Value" select="0" />
		<xsl:param name="MaxValue" />
		<xsl:param name="Selected" />

		<option>
			<xsl:attribute name ="value">
				<xsl:value-of select="$Value"/>
			</xsl:attribute>

			<xsl:if test="$Value = $Selected">
				<xsl:attribute name="selected" >
					<xsl:text>selected</xsl:text>
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$Value"/>
			<xsl:text> </xsl:text>
		</option>

		<xsl:choose>
			<xsl:when test="$Value &lt; $MaxValue">
				<xsl:call-template name="CreateNumericDropDown">
					<xsl:with-param name="Value" select="$Value + 1" />
					<xsl:with-param name="MaxValue" select="$MaxValue" />
					<xsl:with-param name="Selected" select="$Selected" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>

			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

</xsl:stylesheet>
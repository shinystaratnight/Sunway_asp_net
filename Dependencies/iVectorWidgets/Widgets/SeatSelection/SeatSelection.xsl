<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />

	<xsl:template match="/">

		<div id="divSeatSelections">

			<xsl:for-each select="BookingBasket/BasketFlights/BasketFlight">

				<xsl:if test="count(SeatMaps/SeatMap/Seats/Seat) &gt; 0">

					<div id="divSeatSelection" class="box primary clear seatselection hiddencontent">
						<div class="boxTitle">
							<h2>
								<trans ml="SeatSelection">Select Seat</trans>
								<a id="aHideSelection" class="symbol" style="display:none;" href="#" onclick="SeatSelection.ToggleVisibility('Hide');return false;">-</a>
								<a id="aShowSelection" class="symbol" href="#" onclick="SeatSelection.ToggleVisibility('Show');return false;">+</a>
							</h2>
						</div>

						<div class="tabbedBox clear" style="display:none;">
							<div class="tabs">
								<ul>
									<!-- tab for each direction -->
									<xsl:for-each select="SeatMaps/SeatMap">
										<li class="tab">
											<xsl:if test="position() = 1">
												<xsl:attribute name="class">tab selected</xsl:attribute>
											</xsl:if>
											<a href="javascript:SeatSelection.ChangeTab()" ml="SeatSelection">
												<xsl:value-of select="Direction"/>
											</a>
										</li>
									</xsl:for-each>
								</ul>
							</div>
							<xsl:for-each select="SeatMaps/SeatMap">
								<xsl:variable name="direction" select="Direction" />

								<div id="divSeatMap_{$direction}" class="tabContent">
									<xsl:if test="position() != 1">
										<xsl:attribute name="style">display:none;</xsl:attribute>
									</xsl:if>

									<img class="seatMap" src="{Image}" />

									<div class="selectSeats">

										<p ml="SeatSelection">Please select your seat</p>

										<table>
											<thead>
												<tr>
													<th>
														<xsl:text> </xsl:text>
													</th>
													<th>
														<xsl:text> </xsl:text>
													</th>
													<th ml="SeatSelection">Price</th>
												</tr>
											</thead>

											<xsl:variable name="seats" select="Seats" />
											<xsl:for-each select="/BookingBasket/GuestDetails/GuestDetail">
												<tr>
													<td>
														<trans ml="SeatSelection">Passenger</trans>
														<xsl:value-of select="concat(' ', GuestID)"/>
													</td>
													<td>
														<select id="ddlSeats_{GuestID}_{$direction}" name="ddlSeats_{GuestID}_{$direction}" onchange="SeatSelection.SelectSeat({GuestID}, '{$direction}');">
															<option value="0_0" selected="selected"/>
															<xsl:for-each select="$seats/Seat">
																<option value="{SeatToken}_{Price}">
																	<xsl:value-of select="SeatCode"/>
																</option>
															</xsl:for-each>
														</select>
													</td>
													<td>
														<xsl:if test="$CurrencySymbolPosition='Prepend'">
															<xsl:value-of select="$CurrencySymbol"/>
														</xsl:if>
														<span id="spnSeatPrice_{GuestID}_{$direction}">
															<xsl:text>0</xsl:text>
														</span>
														<xsl:if test="$CurrencySymbolPosition='Append'">
															<xsl:value-of select="$CurrencySymbol"/>
														</xsl:if>
													</td>
												</tr>
											</xsl:for-each>
										</table>

									</div>

								</div>
							</xsl:for-each>
						</div>
					</div>
				</xsl:if>

			</xsl:for-each>

			<xsl:if test="count(BookingBasket/BasketFlights/BasketFlight/SeatMaps/SeatMap/Seats/Seat) = 0">
				<xsl:text> </xsl:text>
			</xsl:if>

		</div>

	</xsl:template>


</xsl:stylesheet>

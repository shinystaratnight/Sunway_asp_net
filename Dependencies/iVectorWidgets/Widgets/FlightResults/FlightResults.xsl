<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:param name="CMSBaseURL" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="TotalPassengers" />
	<xsl:param name="DisplayMode" />
	<xsl:param name="SelectButtonValue" />
	<xsl:param name="PerPersonPrices" />
	<xsl:param name="PriceFormat" />
	<xsl:param name="MarkupAmount" />
	<xsl:param name="MarkupPercentage" />
  <xsl:param name="IncludeContainer" />

	<xsl:template match="/">

    <xsl:if test="$IncludeContainer = 'True'">
      <xsl:text disable-output-escaping="yes">&lt;div id="divFlightResults"&gt;</xsl:text>
    </xsl:if>

			<xsl:if test="count(Results/Flights/Flight) = 0">
				<p id="pNoResults" ml="Flight Results">No results found.</p>
			</xsl:if>

			<xsl:choose>
				<xsl:when test ="$DisplayMode = 'ListView'">
					<xsl:call-template name ="FlightResultsListView"/>
				</xsl:when>
				<xsl:when test ="$DisplayMode = 'GridView'">
					<xsl:call-template name ="FlightResultsGridView"/>
				</xsl:when>
			</xsl:choose>

    <xsl:if test="$IncludeContainer = 'True'">
      <xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
    </xsl:if>
		
  </xsl:template>

	<xsl:template name ="FlightResultsGridView">
		<table>
			<tr>
				<th>
					<trans ml="Flight Results">Airline</trans>
				</th>
				<th>
					<trans ml="Flight Results">From/to</trans>
				</th>
				<th>
					<trans ml="Flight Results">Date</trans>
				</th>
				<th>
					<trans ml="Flight Results">Depart</trans>
				</th>
				<th>
					<trans ml="Flight Results">Arrive</trans>
				</th>
				<th class="price">
					<xsl:choose>
						<xsl:when test="$MarkupPercentage > 0 or $MarkupAmount > 0">
							<trans ml="Flight Results">Client Total Price</trans>
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Flight Results">Net Total Price</trans>
						</xsl:otherwise>
					</xsl:choose>
					<p>
						<xsl:text> </xsl:text>
						<trans ml="Flight Results">incl Tax for entire party</trans>
					</p>
				</th>
				<th>
					<xsl:text> </xsl:text>
				</th>
			</tr>
			
			<xsl:for-each select="Results/Flights/Flight">
				
				<!--Outbound-->
				<tr class="upper">
					<td rowspan="2">
						<img src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}"/>						
					</td>
					<td>
						<xsl:value-of select="DepartureAirport"/>
					</td>					
					<td>
						<xsl:call-template name="ShortDate">
							<xsl:with-param name="SQLDate" select="OutboundDepartureDate"/>
						</xsl:call-template>
					</td>
					<td>
						<xsl:value-of select="OutboundDepartureTime"/>
					</td>
					<td>
						<xsl:value-of select="OutboundArrivalTime"/>
					</td>
					<td rowspan="2" class="price">
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="Total"/>
							<xsl:with-param name="Format" select="$PriceFormat"/>
							<xsl:with-param name="Currency" select="$CurrencySymbol"/>
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
						</xsl:call-template>
					</td>
					<td rowspan="2">
						<a id="aSelectFlight_{position()}" class="button primary" href="#" onclick="FlightResults.SelectFlight('{FlightOptionHashToken}');return false;">
							<xsl:choose>
								<xsl:when test ="$SelectButtonValue!=''">
									<xsl:value-of select ="$SelectButtonValue"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Select</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</a>
					</td>
				</tr>
				
				<!--Return-->
				<tr class="lower">
					<td>
						<xsl:value-of select="ArrivalAirport"/>
					</td>
					<td>
						<xsl:call-template name="ShortDate">
							<xsl:with-param name="SQLDate" select="ReturnDepartureDate"/>
						</xsl:call-template>
					</td>
					<td>
						<xsl:value-of select="ReturnDepartureTime"/>
					</td>
					<td>
						<xsl:value-of select="ReturnArrivalTime"/>
					</td>
				</tr>		
				
			</xsl:for-each>
		</table>
	</xsl:template>

	<xsl:template name="FlightResultsListView">
		<xsl:for-each select="Results/Flights/Flight">
			
			<xsl:variable name="resultposition" select="position()" />
			
			<div class="result clear">

			<div class="details">

				<xsl:choose>
					<xsl:when test="count(FlightSectors/FlightSector[Direction = 'Outbound']) &gt; 0">
						<xsl:for-each select="FlightSectors/FlightSector[Direction = 'Outbound']">
							<div class="outbound clear">
								<xsl:if test="count(../../FlightSectors/FlightSector[Direction = 'Outbound']) = 1">
									<xsl:attribute name="class">outbound clear singleLeg</xsl:attribute>
								</xsl:if>
								<div>
									<div class="carrierlogo">
										<img class="carrierlogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}"/>
									</div>
									<div class="outboundIcon">
										<xsl:text> </xsl:text>
									</div>
								</div>

								<xsl:call-template name="MultiSectorFlightLeg"/>

							</div>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<div class="outbound clear">
							<div>
								<div class="carrierlogo">
									<img class="carrierlogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}"/>
								</div>
								<div class="outboundIcon">
									<xsl:text> </xsl:text>
								</div>
							</div>

							<div>
								<p>
									<xsl:value-of select="DepartureAirport"/>
									<xsl:text> (</xsl:text>
									<xsl:value-of select="DepartureAirportCode"/>
									<xsl:text>) </xsl:text>
									<xsl:value-of select="OutboundDepartureTime"/>
									<xsl:text>, </xsl:text>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="OutboundDepartureDate"/>
									</xsl:call-template>
								</p>
								<p>
									<xsl:value-of select="ArrivalAirport"/>
									<xsl:text> (</xsl:text>
									<xsl:value-of select="ArrivalAirportCode"/>
									<xsl:text>) </xsl:text>
									<xsl:value-of select="OutboundArrivalTime"/>
									<xsl:text>, </xsl:text>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="OutboundArrivalDate"/>
									</xsl:call-template>
								</p>
							</div>
						</div>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:choose>
					<xsl:when test="count(FlightSectors/FlightSector[Direction = 'Return']) &gt; 0">
						<xsl:for-each select="FlightSectors/FlightSector[Direction = 'Return']">
							<div>
								<xsl:attribute name="class">
									<xsl:choose>
										<xsl:when test="count(../../FlightSectors/FlightSector[Direction = 'Return']) = 1">return clear singleLeg</xsl:when>
										<xsl:when test="position() &gt; 1">outbound clear</xsl:when>
										<xsl:otherwise>return clear</xsl:otherwise>									
									</xsl:choose>
								</xsl:attribute>
								<div>
									<div class="carrierlogo">
										<img class="carrierlogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}"/>
									</div>
									<div class="inboundIcon">
										<xsl:text> </xsl:text>
									</div>
								</div>
								<xsl:call-template name="MultiSectorFlightLeg"/>
							</div>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<div class="return clear">
							<div>
								<div class="carrierLogo">
									<img class="carrierlogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}"/>
								</div>
								<div class="inboundIcon">
									<xsl:text> </xsl:text>
								</div>
							</div>

							<div>
								<p>
									<span class="airport">
										<xsl:value-of select="ArrivalAirport"/>
									</span>

									<span class="code">
										<xsl:text> (</xsl:text>
										<xsl:value-of select="ArrivalAirportCode"/>
										<xsl:text>) </xsl:text>
									</span>

									<span class="time">
										<xsl:value-of select="ReturnDepartureTime"/>
									</span>

									<span class="dateTime">
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnDepartureDate"/>
										</xsl:call-template>
									</span>
								</p>
								<p>
									<span class="airport">
										<xsl:value-of select="DepartureAirport"/>
									</span>

									<span class="code">
										<xsl:text> (</xsl:text>
										<xsl:value-of select="DepartureAirportCode"/>
										<xsl:text>) </xsl:text>
									</span>

									<span class="time">
										<xsl:value-of select="ReturnArrivalTime"/>
									</span>

									<span class="dateTime">
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnArrivalDate"/>
										</xsl:call-template>
									</span>
								</p>
							</div>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</div>

			<div class="price">
				<p ml="Flight Results">Return Flight</p>
				<xsl:choose>
					<xsl:when test="$PerPersonPrices = 'True'">
						<p class="price">
							<xsl:call-template name="GetSellingPrice">
								<xsl:with-param name="Value" select="Total div $TotalPassengers"/>
								<xsl:with-param name="Format" select="$PriceFormat"/>
								<xsl:with-param name="Currency" select="$CurrencySymbol"/>
								<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
							</xsl:call-template>
						</p>
						<p ml="Flight Results">per person</p>
					</xsl:when>
					<xsl:otherwise>
						<p class="price">
							<xsl:call-template name="GetSellingPrice">
								<xsl:with-param name="Value" select="Total"/>
								<xsl:with-param name="Format" select="$PriceFormat"/>
								<xsl:with-param name="Currency" select="$CurrencySymbol"/>
								<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
							</xsl:call-template>
						</p>
					</xsl:otherwise>
				</xsl:choose>
				
				<xsl:variable name="flightcarrierid" select="FlightCarrierID"/>
				<xsl:choose>
					<xsl:when test="/Results/FlightResultsExtraXML/FlightCarriers">
						<xsl:for-each select="/Results/FlightResultsExtraXML/FlightCarriers/FlightCarrier[SourceID = $flightcarrierid]">
							<xsl:if test="BaggageMessage != ''">
								<p ml="Flight Results">Baggage + Deposit Information</p>
								<a id="aBaggageInfo_{$resultposition}" class="tooltip baggageInfo" onmouseover="web.Tooltip.Show(this, int.f.GetHTML('divBaggageInfoPopup_{$resultposition}'), 'bottom', null, -50, 0, '');" onmouseout="web.Tooltip.Hide();" href="javascript:void;"><xsl:text> </xsl:text></a>
								<div id="divBaggageInfoPopup_{$resultposition}" style="display:none;">
									<xsl:value-of select="BaggageMessage"/>
								</div>
							</xsl:if>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<p>
							<em ml="Flight Results">.excl baggage</em>
						</p>
					</xsl:otherwise>
				</xsl:choose>
				
				<a id="aSelectFlight_{position()}" class="button primary xlarge" href="#" onclick="FlightResults.SelectFlight('{FlightOptionHashToken}');return false;">Select</a>
			</div>

		</div>
		</xsl:for-each>
	</xsl:template>
	

	
	<xsl:template name="MultiSectorFlightLeg">
		<div>
			<p>
				<span class="airport">
					<xsl:value-of select="DepartureAirport"/>
				</span>
				
				<span class="code">
					<xsl:text> (</xsl:text>
					<xsl:value-of select="DepartureAirportCode"/>
					<xsl:text>) </xsl:text>
				</span>
				
				<span class="time">
					<xsl:value-of select="DepartureTime"/>
				</span>
				
				<span class="date">
					<xsl:call-template name="ShortDate">
						<xsl:with-param name="SQLDate" select="DepartureDate"/>
					</xsl:call-template>
				</span>				
			</p>
			<p>
				<span class="airport">
					<xsl:value-of select="ArrivalAirport"/>
				</span>
				
				<span class="code">
					<xsl:text> (</xsl:text>
					<xsl:value-of select="ArrivalAirportCode"/>
					<xsl:text>) </xsl:text>
				</span>
				
				<span class="time">
					<xsl:value-of select="ArrivalTime"/>
				</span>
				
				<span class="date">
					<xsl:call-template name="ShortDate">
						<xsl:with-param name="SQLDate" select="ArrivalDate"/>
					</xsl:call-template>
				</span>				
			</p>
		</div>
	</xsl:template>
	
</xsl:stylesheet>

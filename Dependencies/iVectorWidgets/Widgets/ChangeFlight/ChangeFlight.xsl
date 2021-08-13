<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:param name="Theme" />
	<xsl:param name="SelectedFlightToken" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="Template" />
	<xsl:param name="Passengers" />
    <xsl:param name="CMSBaseURL" />
	<xsl:param name="ChangeFlightText" />
	<xsl:param name="ChangeFlightTextAlt" />

	
	<xsl:template match="/">
		
		<xsl:choose>
			<xsl:when test ="$Template ='ResultsOnly'">
				<xsl:call-template name="ChangeFlight"/>
			</xsl:when>

			<xsl:otherwise>
				<div id="ChangeFlightHolder">
					<xsl:call-template name="ChangeFlight"/>
				</div>
			</xsl:otherwise>
			
		</xsl:choose>


	</xsl:template>

	
	<xsl:template name ="ChangeFlight">

		<!-- variables -->
		<xsl:variable name="PriceFormat" select="'###,##0.00'" />
		<xsl:variable name="SelectedFlightPrice" select="Results/Flights/Flight[BookingToken = $SelectedFlightToken]/Total" />        
		<xsl:variable name="ExactMatchFlightAlternatives" select="count(Results/Flights/Flight[BookingToken != $SelectedFlightToken and ExactMatch = 'true'])" />

		
		<!-- change flight -->
		<div id="divChangeFlight" class="clear">
			<h2>Selected Flights</h2>
			
			
			<!-- selected flight -->
			<div id="divSelectedFlight">
				
				<xsl:for-each select="Results/Flights/Flight[BookingToken = $SelectedFlightToken]">
					<div class="flightResult clear">
                       
						
						<!-- flight details -->
						<div class="flightdetails">
							
							<!-- outbound -->
							<div class="outbound">
								<xsl:for-each select="FlightSectors/FlightSector[Direction='Outbound']">
									<xsl:call-template name="FlightLeg" />
								</xsl:for-each>
							</div>
							
							<!-- return -->
							<div class="return">
								<xsl:for-each select="FlightSectors/FlightSector[Direction='Return']">
									<xsl:call-template name="FlightLeg" />
								</xsl:for-each>
							</div>
							
						</div>

									
						<!-- if we have alternatives show change flight button -->
						<xsl:if test="$ExactMatchFlightAlternatives &gt; 0">
							<div class="price">
								<input type="hidden" id="hidChangeFlightText" value="{$ChangeFlightText}" />
								<input type="hidden" id="hidChangeFlightTextAlt" value="{$ChangeFlightTextAlt}" />
					
								<a id="aChangeFlight" class="button primary" onclick="ChangeFlight.ShowFlights();">
									<xsl:choose>
										<xsl:when test="$ChangeFlightText = ''">
											<xsl:text>Change Flight</xsl:text>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$ChangeFlightText"/>
										</xsl:otherwise>
									</xsl:choose>
								</a>					
							</div>	
						</xsl:if>						
						
					</div>										
					
				</xsl:for-each>
				
			</div>
			
			
			<!-- if we have exact match flights that is not our selected flight, render options -->
			<xsl:if test="$ExactMatchFlightAlternatives &gt; 0">
				
			
				<div id="divFlightOptions" style="display:none">
					
					<h2>Alternative Flights</h2>
					
					<!-- loop through exact match flights that are not our selected flight -->
					<xsl:for-each select="Results/Flights/Flight[BookingToken != $SelectedFlightToken and ExactMatch = 'true']">
						
						<div class="flightResult clear">
							
							
							<!-- flight details -->
							<div class="flightdetails">
							
								<!-- outbound -->
								<div class="outbound">
									<xsl:for-each select="FlightSectors/FlightSector[Direction='Outbound']">
										<xsl:call-template name="FlightLeg" />
									</xsl:for-each>
								</div>
							
								<!-- return -->
								<div class="return">
									<xsl:for-each select="FlightSectors/FlightSector[Direction='Return']">
										<xsl:call-template name="FlightLeg" />
									</xsl:for-each>
								</div>
							
							</div>

							
							
							<!-- price -->
							<div class="price clear">
								<h3>
									<xsl:variable name="SelectedPricePerPerson" select="$SelectedFlightPrice div $Passengers" />
									<xsl:variable name="TotalPerPerson" select="Total div $Passengers" />
									
									<xsl:choose>
										<xsl:when test="$SelectedFlightPrice > Total">
											<span class="minus"><xsl:text>-</xsl:text></span>
											<xsl:variable name="PriceDifference" select="$TotalPerPerson - $SelectedPricePerPerson" />
											<xsl:variable name="Absolute" select="$PriceDifference*($PriceDifference >=0) - $PriceDifference*($PriceDifference &lt; 0)" />
											<xsl:value-of select="concat($CurrencySymbol, format-number($Absolute, $PriceFormat))"/>
										</xsl:when>
										<xsl:otherwise>
											<span class="plus"><xsl:text>+</xsl:text></span>
											<xsl:value-of select="concat($CurrencySymbol, format-number($TotalPerPerson - $SelectedPricePerPerson, $PriceFormat))"/>
										</xsl:otherwise>
									</xsl:choose>									
								</h3>
								<a class="button primary" onclick="ChangeFlight.SelectFlight('{BookingToken}');">Select</a>
							</div>
							
						</div>
						
					</xsl:for-each>
				</div>
			</xsl:if>
			
		</div>
	</xsl:template>
	
	
	
	

	<xsl:template name="FlightLeg">		
		
		<div class="flightLeg clear">			
			
			<!-- carrier logo -->
			<div class="carrierLogo">
				<img class="carrierLogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="Flight Carrier" />
			</div>
			
			
			<!-- airports -->
			<div class="airports">
				
				<xsl:if test="position() = 1">
					<p><strong><xsl:value-of select="Direction"/></strong></p>
				</xsl:if>
				
				<p>
					<xsl:value-of select="DepartureAirport"/>
					<xsl:text> (</xsl:text>
					<xsl:value-of select="DepartureAirportCode"/>
					<xsl:text>)</xsl:text>
				</p>
				<p>
					<xsl:value-of select="ArrivalAirport"/>
					<xsl:text> (</xsl:text>
					<xsl:value-of select="ArrivalAirportCode"/>
					<xsl:text>)</xsl:text>
				</p>
			</div>
			
			<!-- dates -->
			<div class="dates">
				<xsl:if test="position() = 1">
					<p><strong>Date</strong></p>
				</xsl:if>
				<p>
					<xsl:call-template name="ShortDate">
						<xsl:with-param name="SQLDate" select="DepartureDate"/>
					</xsl:call-template>
				</p>
				<p>
					<xsl:call-template name="ShortDate">
						<xsl:with-param name="SQLDate" select="ArrivalDate"/>
					</xsl:call-template>
				</p>
			</div>
			
			<!-- times -->
			<div class="times">
				<xsl:if test="position() = 1">
					<p><strong>Time</strong></p>
				</xsl:if>
				<p><xsl:value-of select="DepartureTime"/></p>
				<p><xsl:value-of select="ArrivalTime"/></p>
			</div>
			
		</div>
	</xsl:template>
	
</xsl:stylesheet>
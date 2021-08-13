<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CMSBaseURL" />
	<xsl:param name="PerPersonPrices" />
	<xsl:param name="PaxCount" />
	<xsl:param name="TimeOfDayControls" />
	<xsl:param name="TimeSliders" />
	<xsl:param name="UpdateResultsFunction" />
	<xsl:param name="IncludeContainer" />
	<xsl:param name="IncludeBaggageFilter" />

    <xsl:template match="/">

			<xsl:if test="count(Results/Flights/Flight) > 0">
				
				<xsl:for-each select="Results/Filters">

					<xsl:if test="$IncludeContainer = 'True'">
						<xsl:text disable-output-escaping="yes">&lt;div id="divFlightFilter" class="sidebarBox primary clear"&gt;</xsl:text>
					</xsl:if>
						
						<div class="boxTitle">
							<h2 ml="Results Filter">Filter Results</h2>
						</div>
						
						
						<!-- Min/Max Price -->
						<div id="divMinMaxPrice" class="filter">
							<h4 ml="Results Filter">Price</h4>
							<p ml="Results Filter" id="pMinMaxFilterMessage">Drag the sliders to choose your min/max flight price</p>

							<!-- Slider Control -->
							<div id="sldFlightPrice" class="slider">
								<label class="range start">

									<xsl:variable name="minprice">
										<xsl:choose>
											<xsl:when test="$PerPersonPrices = 'True'">
												<xsl:value-of select="floor(MinPrice div $PaxCount)"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="floor(MinPrice)"/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:variable>

									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="$minprice" />
										<xsl:with-param name="Format" select="'###,##0'" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>

									<xsl:if test="$PerPersonPrices = 'True'">
										<xsl:text>pp</xsl:text>
									</xsl:if>

								</label>
								<label class="range end">

									<xsl:variable name="maxprice">
										<xsl:choose>
											<xsl:when test="$PerPersonPrices = 'True'">
												<xsl:value-of select="floor(MaxPrice div $PaxCount)"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="floor(MaxPrice)"/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:variable>

									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="$maxprice" />
										<xsl:with-param name="Format" select="'###,##0'" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>

									<xsl:if test="$PerPersonPrices = 'True'">
										<xsl:text>pp</xsl:text>
									</xsl:if>

								</label>
								<div id="sldFlightPrice_Highlight" class="highlight" style="left:0;width:100%;">
									<xsl:text> </xsl:text>
								</div>
								<a id="sldFlightPrice_Start" class="sliderbutton" style="left:0;">
									<xsl:text> </xsl:text>
									<!--<span>
										<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
											<xsl:value-of select="$CurrencySymbol"/>
										</xsl:if>
										<xsl:choose>
											<xsl:when test="$PerPersonPrices = 'True'">
												<xsl:value-of select="floor(MinPrice div $PaxCount)"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="floor(MinPrice)"/>
											</xsl:otherwise>
										</xsl:choose>
										<xsl:if test="$CurrencySymbolPosition = 'Append'">
											<xsl:value-of select="$CurrencySymbol"/>
										</xsl:if>
										<xsl:if test="$PerPersonPrices = 'True'">
											<xsl:text>pp</xsl:text>
										</xsl:if>
									</span>-->
								</a>
								<a id="sldFlightPrice_End" class="sliderbutton" style="right:0;">
									<xsl:text> </xsl:text>
									<!--<span>
										<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
											<xsl:value-of select="$CurrencySymbol"/>
										</xsl:if>
										<xsl:choose>
											<xsl:when test="$PerPersonPrices = 'True'">
												<xsl:value-of select="floor(MaxPrice div $PaxCount)"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="floor(MaxPrice)"/>
											</xsl:otherwise>
										</xsl:choose>
										<xsl:if test="$CurrencySymbolPosition = 'Append'">
											<xsl:value-of select="$CurrencySymbol"/>
										</xsl:if>
										<xsl:if test="$PerPersonPrices = 'True'">
											<xsl:text>pp</xsl:text>
										</xsl:if>
									</span>-->
								</a>
							</div>

							<p class="displayrange">
								<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>
								<span id="sldFlightPrice_DisplayMin">
									<xsl:choose>
										<xsl:when test="$PerPersonPrices = 'True'">
											<xsl:value-of select="floor(MinPrice div $PaxCount)"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="floor(MinPrice)"/>
										</xsl:otherwise>
									</xsl:choose>
								</span>
								<xsl:if test="$CurrencySymbolPosition = 'Append'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>

								<xsl:text> - </xsl:text>

								<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>
								<span id="sldFlightPrice_DisplayMax">
									<xsl:choose>
										<xsl:when test="$PerPersonPrices = 'True'">
											<xsl:value-of select="floor(MaxPrice div $PaxCount)"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="floor(MaxPrice)"/>
										</xsl:otherwise>
									</xsl:choose>
								</span>
								<xsl:if test="$CurrencySymbolPosition = 'Append'">
									<xsl:value-of select="$CurrencySymbol"/>
								</xsl:if>

								<xsl:if test="$PerPersonPrices = 'True'">
									<xsl:text>pp</xsl:text>
								</xsl:if>
							</p>

							<input type="hidden" id="sldFlightPrice_MinValue" value="{floor(MinPrice)}" />
							<input type="hidden" id="sldFlightPrice_MaxValue" value="{ceiling(MaxPrice)}" />
							<input type="hidden" id="sldFlightPrice_AbsMinValue" value="{floor(MinPrice)}" />
							<input type="hidden" id="sldFlightPrice_AbsMaxValue" value="{ceiling(MaxPrice)}" />
							<!--<input type="hidden" id="sldFlightPrice_CurrencySymbol" value="{$CurrencySymbol}" />
							<input type="hidden" id="sldFlightPrice_CurrencySymbolPosition" value="{$CurrencySymbolPosition}" />-->
							<xsl:if test="$PerPersonPrices = 'True'">
								<input type="hidden" id="sldFlightPrice_DisplayValueDivide" value="{$PaxCount}" />
							</xsl:if>
							<input type="hidden" id="sldFlightPrice_OnChange" value="FlightResultsFilter.Filter();"/>

							<script type="text/javascript">
								web.Slider.Setup('sldFlightPrice');
							</script>

						</div>
						
				
						
						<!-- Carriers -->
						<xsl:if test="count(FlightCarriers/FlightCarrier) > 1">
							<div id="divFlightCarrier" class="filter">
								<h4 ml="Results Filter">Carrier</h4>

								<xsl:for-each select="FlightCarriers/FlightCarrier">
									<div class="flightCarrierFilter">
										<label class="checkboxLabel">
                      <input type="checkbox" class="checkbox" value="true" id="chk_fc_{FlightCarrierID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
                        <xsl:if test ="Selected = 'true'">
                          <xsl:attribute name ="checked">                         
                              <xsl:text>checked</xsl:text>                         
                          </xsl:attribute>
                        </xsl:if>
                      </input>                    
											<img class="carrierlogo" src="{$CMSBaseURL}Carriers/{FlightCarrierLogo}" alt="{FlightCarrier}" />
										</label>

										<xsl:text> (</xsl:text>
										<span id="spn_fc_count_{FlightCarrierID}">
											<xsl:value-of select="Count"/>
										</span>
										<xsl:text>)</xsl:text>
									</div>
								</xsl:for-each>
							
							</div>
						</xsl:if>
						
						
						
						<!-- Stops -->
						<xsl:if test="count(Stops/FlightStop) > 1">
							
							<div id="divStops" class="filter">
								<h4 ml="Results Filter">Number of Stops</h4>
								<!--<p ml="Results Filter">Number of stops</p>-->
							
								<!--<select id="ddlFlightStops">
									<xsl:for-each select="Stops/FlightStop">
										<option value="{Stops}">
											<xsl:choose>
												<xsl:when test="Stops=0">												
													<xsl:text>Direct</xsl:text>
												</xsl:when>
												<xsl:when test="Stops=1">
													<xsl:text>1 Stop</xsl:text>
												</xsl:when>
												<xsl:otherwise>
													<xsl:value-of select="concat(Stops, 'Stops')"/>
												</xsl:otherwise>										
											</xsl:choose>					
										</option>
									</xsl:for-each>
								</select>-->

								<div class="flightStopsFilter">									
									<xsl:for-each select="Stops/FlightStop">
										<div>
											<label class="checkboxLabel">
												<input type="checkbox" class="checkbox" value="true" id="chk_stp_{Stops}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
												<xsl:choose>
													<xsl:when test="Stops=0">
														<xsl:text>Direct</xsl:text>
													</xsl:when>
													<xsl:when test="Stops=1">
														<xsl:text>1 Stop</xsl:text>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="concat(Stops, ' Stops')"/>
													</xsl:otherwise>
												</xsl:choose>
											</label>

											<xsl:text> (</xsl:text>
											<span id="spn_stp_count_{Stops}">
												<xsl:value-of select="Count"/>
											</span>
											<xsl:text>)</xsl:text>
										</div>
									</xsl:for-each>					
		
								</div>
								
							</div>
							
						</xsl:if>
						
						
					
						<!-- Airports -->
						<xsl:if test="count(DepartureAirports/Airport) > 1">
							<div id="divDepartureAirports" class="filter">
								<h4 ml="Results Filter">Departure Airport</h4>
																					
								<xsl:for-each select="DepartureAirports/Airport">
									<div>
										<label class="checkboxLabel">
											<input type="checkbox" class="checkbox" value="true" id="chk_dptapt_{AirportID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
											<xsl:value-of select="AirportName"/>
										</label>

										<xsl:text> (</xsl:text>
										<span id="spn_apt_count_dpt_{AirportID}">
											<xsl:value-of select="Count"/>
										</span>
										<xsl:text>)</xsl:text>
									</div>										
								</xsl:for-each>						
							</div>
						</xsl:if>
						
						
						
						<!-- Arrival Airport-->
						<xsl:if test="count(ArrivalAirports/Airport) > 1">
							<div id="divArrivalAirports" class="filter">
								<h4 ml="Results Filter">Arrival Airport</h4>																					
								<xsl:for-each select="ArrivalAirports/Airport">
									<div>
										<label class="checkboxLabel">
											<input type="checkbox" class="checkbox" value="true" id="chk_arrapt_{AirportID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
											<xsl:value-of select="AirportName"/>
										</label>

										<xsl:text> (</xsl:text>
										<span id="spn_apt_count_arr_{AirportID}">
											<xsl:value-of select="Count"/>
										</span>
										<xsl:text>)</xsl:text>
									</div>		
								</xsl:for-each>						
							</div>
						</xsl:if>
						
						

						<!-- Time sliders -->
						<xsl:if test="$TimeSliders = 'True'">
							<div id="divTimeSliders" class="filter">
								<h4>Departure Times</h4>
								<!-- departure time slider -->
								<div id="divDepartureSlider">

									<div id="sldDeparture" class="slider time">
										<label class="range start">

											<xsl:text>00:00</xsl:text>

										</label>
										<label class="range end">

											<xsl:text>23:59</xsl:text>

										</label>
										<div id="sldDeparture_Highlight" class="highlight" style="left:0;width:100%;">
											<xsl:text> </xsl:text>
										</div>
										<a id="sldDeparture_Start" class="sliderbutton" style="left:0;">
											<xsl:text> </xsl:text>
										</a>
										<a id="sldDeparture_End" class="sliderbutton" style="right:0;">
											<xsl:text> </xsl:text>
										</a>
									</div>

									<p class="displayrange">
										<xsl:text>Outbound </xsl:text>
										<span id="sldDeparture_DisplayMin">
											<xsl:text>00:00</xsl:text>
										</span>

										<xsl:text> - </xsl:text>

										<span id="sldDeparture_DisplayMax">
											<xsl:text>23:59</xsl:text>
										</span>
									</p>

									<input type="hidden" id="sldDeparture_MinValue" value="0" />
									<input type="hidden" id="sldDeparture_MaxValue" value="1439" />
									<input type="hidden" id="sldDeparture_AbsMinValue" value="0" />
									<input type="hidden" id="sldDeparture_AbsMaxValue" value="1439" />
									<input type="hidden" id="sldDeparture_OnChange" value="FlightResultsFilter.Filter();"/>
									<!--<span id="sldDeparture_Show" style="display:none;"><xsl:text> </xsl:text></span>-->

									<script type="text/javascript">
										web.Slider.Setup('sldDeparture');
									</script>
								</div>

								<div id="divReturnSlider">

									<div id="sldReturn" class="slider time">
										<label class="range start">

											<xsl:text>00:00</xsl:text>

										</label>
										<label class="range end">

											<xsl:text>23:59</xsl:text>

										</label>
										<div id="sldReturn_Highlight" class="highlight" style="left:0;width:100%;">
											<xsl:text> </xsl:text>
										</div>
										<a id="sldReturn_Start" class="sliderbutton" style="left:0;">
											<xsl:text> </xsl:text>
										</a>
										<a id="sldReturn_End" class="sliderbutton" style="right:0;">
											<xsl:text> </xsl:text>
										</a>
									</div>

									<p class="displayrange">
										<xsl:text>Return </xsl:text>
										<span id="sldReturn_DisplayMin">
											<xsl:text>00:00</xsl:text>
										</span>

										<xsl:text> - </xsl:text>

										<span id="sldReturn_DisplayMax">
											<xsl:text>23:59</xsl:text>
										</span>
									</p>

									<input type="hidden" id="sldReturn_MinValue" value="0" />
									<input type="hidden" id="sldReturn_MaxValue" value="1439" />
									<input type="hidden" id="sldReturn_AbsMinValue" value="0" />
									<input type="hidden" id="sldReturn_AbsMaxValue" value="1439" />
									<input type="hidden" id="sldReturn_OnChange" value="FlightResultsFilter.Filter();"/>
									<!--<span id="sldDeparture_Show" style="display:none;"><xsl:text> </xsl:text></span>-->

									<script type="text/javascript">
										web.Slider.Setup('sldReturn');
									</script>
								</div>

							</div>
						</xsl:if>	
						
						
					
						<!-- Times of Day -->
						<xsl:if test="$TimeOfDayControls = 'True'">
							
							<!-- Outbound Departure Time -->
							<xsl:if test="count(DepartureTimes/FlightTime) > 1">
								<div id="divDepartureTime" class="filter">
									<h4 ml="Results Filter">Outbound Departure Time</h4>
									<xsl:for-each select="DepartureTimes/FlightTime">
										<div>
											<label class="checkboxLabel">
												<input type="checkbox" class="checkbox" value="true" id="chk_dpt_{TimeOfDay}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
												<xsl:value-of select="TimeOfDay"/>
											</label>

											<xsl:text> (</xsl:text>
											<span id="spn_tod_count_dpt_{TimeOfDay}">
												<xsl:value-of select="Count"/>
											</span>
											<xsl:text>)</xsl:text>
										</div>
									</xsl:for-each>
								</div>
							</xsl:if>
							
							
							<!-- Return Departure Time -->
							<xsl:if test="count(ReturnTimes/FlightTime) > 1">
								<div id="divReturnTime" class="filter">
									<h4 ml="Results Filter">Return Departure Time</h4>							
									<xsl:for-each select="ReturnTimes/FlightTime">
										<div>
											<label class="checkboxLabel">
												<input type="checkbox" class="checkbox" value="true" id="chk_rtn_{TimeOfDay}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
												<xsl:value-of select="TimeOfDay"/>
											</label>

											<xsl:text> (</xsl:text>
											<span id="spn_tod_count_rtn_{TimeOfDay}">
												<xsl:value-of select="Count"/>
											</span>
											<xsl:text>)</xsl:text>
										</div>
									</xsl:for-each>
								</div>
							</xsl:if>
		
						</xsl:if>
					
					
					
						<!-- Baggage -->
						<xsl:if test="$IncludeBaggageFilter = 'True'">
							<div id="divBaggageIncluded" class="filter">
								<h4 ml="Results Filter">Baggage</h4>

								<div>
									<label class="checkboxLabel">
										<input type="checkbox" class="checkbox" value="true" id="chk_inclbag" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
										<trans ml="Results Filter">Baggage Included</trans>
									</label>
									
									<xsl:text> (</xsl:text>
									<span id="spn_inclbag_count">
										<xsl:value-of select="count(/Results/Flights/Flight[IncludesSupplierBaggage = 'true'])"/>
									</span>
									<xsl:text>)</xsl:text>
								</div>
							</div>
						</xsl:if>
					
					<xsl:if test="$IncludeContainer = 'True'">
						<xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
					</xsl:if>

				</xsl:for-each>
				
				<xsl:if test="$IncludeContainer = 'True'">
					<script type="text/javascript">
						<xsl:text>int.ll.OnLoad.Run(function () { FlightResultsFilter.Setup( function() {</xsl:text>
						<xsl:value-of select ="$UpdateResultsFunction"/>
						<xsl:text>}); });</xsl:text>
					</script>
				</xsl:if>

			</xsl:if>
		
    </xsl:template>
</xsl:stylesheet> 

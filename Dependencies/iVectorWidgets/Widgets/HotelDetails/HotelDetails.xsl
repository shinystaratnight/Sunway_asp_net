<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:include href="../../xsl/markdown.xsl "/>
	<xsl:include href="../../xsl/functions.xsl "/>

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />


	<xsl:template match="/">

		<div id="divHotelDetails">

			
			<!-- Hotel main details -->
			<div class="maindetails">
				<h1>
					<xsl:value-of select="Property/Name"/>				
				</h1>
				<xsl:call-template name="StarRating">
					<xsl:with-param name="Rating" select="Property/Rating" />									
				</xsl:call-template>	
				<h2 class="geography"><xsl:value-of select="concat(Property/Resort, ', ', Property/Region)"/></h2>
				<p>
					<xsl:if test="Property/Address1 != ''">
						<xsl:value-of select="Property/Address1"/>
					</xsl:if>
					<xsl:if test="Property/Address2 != ''">
						<xsl:text>, </xsl:text>
						<xsl:value-of select="Property/Address2"/>
					</xsl:if>
					<xsl:if test="Property/Resort != ''">
						<xsl:text>, </xsl:text>
						<xsl:value-of select="Property/Resort"/>
					</xsl:if>
					<xsl:if test="Property/PostcodeZip != ''">
						<xsl:text>, </xsl:text>
						<xsl:value-of select="Property/PostcodeZip"/>
					</xsl:if>
					<xsl:text> </xsl:text>
				</p>
			</div>

			
			
			<!-- Images -->
			<div class="hotelimages clear">
				
				<!-- Main Image -->
				<img id="imgMainImage" src="{Property/MainImage}" alt="{Property/MainImage_Title}"/>

				<!-- Other images -->
				<xsl:if test="count(Property/Images/Image) &gt; 0">
					<div id="divOtherImages" class="clear">	
						<img class="selected" src="{Property/MainImage}" alt="{Property/MainImage_Title}" onmouseover="HotelDetails.ImageHover(this);" />						
						<xsl:for-each select="Property/Images/Image">
							<img src="{Image}" alt="{Image_Title}" onmouseover="HotelDetails.ImageHover(this);" />
						</xsl:for-each>
					</div>
				</xsl:if>
				
			</div>

			
			<!-- Description -->
			<div class="description">
				<xsl:call-template name="Markdown">
					<xsl:with-param name="text" select="Property/Description"/>
				</xsl:call-template>
			</div>

			
			
			<!-- search result details -->
			<xsl:if test="count(Property/Hotel/Rooms) > 0">
				<div class="resultdetails">
				
				
					<!-- Flight Details -->
					<xsl:for-each select="Property/Hotel/SelectedFlight">

						<div class="selectedflight clear">
							
							<h2 ml="Hotel Details">Flights</h2>

							<!-- Outbound -->
							<div class="outbound">
								<h4 ml="Hotel Details">Outbound</h4>

								<p>
									<trans ml="Hotel Details">Departs </trans>
									<strong>
										<xsl:value-of select="DepartureAirport"/>
									</strong>
								</p>
								<p><xsl:value-of select="OutboundFlightCode"/></p>
								<p>
									<trans ml="Hotel Details" mlparams="{OutboundDepartureDate}~ShortDate">{0}</trans>
									<xsl:text> </xsl:text>
									<strong>
										<xsl:value-of select="OutboundDepartureTime"/>
									</strong>
								</p>

								<p>
									<trans ml="Hotel Details">Arrives </trans>
									<strong>
										<xsl:value-of select="ArrivalAirport"/>
									</strong>
								</p>
								<p>
									<trans ml="Hotel Details" mlparams="{OutboundArrivalDate}~ShortDate">{0}</trans>
									<xsl:text> </xsl:text>
									<strong>
										<xsl:value-of select="OutboundArrivalTime"/>
									</strong>
								</p>

							</div>

							<!-- Return -->
							<div class="return">
								<h4 ml="Hotel Details">Return</h4>

								<p>
									<trans ml="Hotel Details">Departs </trans>
									<strong>
										<xsl:value-of select="ArrivalAirport"/>
									</strong>
								</p>
								<p><xsl:value-of select="ReturnFlightCode"/></p>
								<p>
									<trans ml="Hotel Details" mlparams="{ReturnDepartureDate}~ShortDate">{0}</trans>
									<xsl:text> </xsl:text>
									<strong>
										<xsl:value-of select="ReturnDepartureTime"/>
									</strong>
								</p>

								<p>
									<trans ml="Hotel Details">Arrives </trans>
									<strong>
										<xsl:value-of select="DepartureAirport"/>
									</strong>
								</p>
								<p>
									<trans ml="Hotel Details" mlparams="{ReturnArrivalDate}~ShortDate">{0}</trans>
									<xsl:text> </xsl:text>
									<strong>
										<xsl:value-of select="ReturnArrivalTime"/>
									</strong>
								</p>

							</div>

						</div>
						
						<input type="hidden" id="hidFlightOptionHashToken" value="{FlightOptionHashToken}" />

					</xsl:for-each>
			
			
			
					<!-- Rates Table -->
					<xsl:variable name="roomcount" select="count(Property/Hotel/Rooms/Room)" />
					<xsl:variable name="paxcount" select="sum(Property/Hotel/Rooms/Room/Adults) + sum(Property/Hotel/Rooms/Room/Children)" />
					
					<input type="hidden" id="hidPropertyResults_RoomCount" value="{$roomcount}" />
					<input type="hidden" id="hidWarning_MultiRoomSelect" value="Please select a room option for each room." ml="Hotel Details" />
					
					<xsl:for-each select="Property/Hotel/Rooms">
				
						<xsl:variable name="flightprice">
							<xsl:choose>
								<xsl:when test="../SelectedFlight/BookingToken != ''">
									<xsl:value-of select="../SelectedFlight/Total"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>0</xsl:text>					
								</xsl:otherwise>
							</xsl:choose>			
						</xsl:variable>
						
						
						

						<div class="rates">

							<h2 ml="Hotel Details">Room Types</h2>
					
							<xsl:for-each select="Room">
								
								<xsl:variable name="roomnumber" select="position()" />
								
								<xsl:if test="$roomcount > 1">
									<h3 ml="Hotel Details" mlparams="{$roomnumber}">Room {0}</h3>
								</xsl:if>
								
								<xsl:variable name="roomflightprice">
									<xsl:value-of select="($flightprice div $paxcount) * (Adults + Children)"/>
								</xsl:variable>
								
								
								<table class="def striped">
									<tr>
										<th ml="Hotel Details">Room Type</th>
										<th ml="Hotel Details">Meal Basis</th>
										<th ml="Hotel Details">Total</th>
										<th ml="Hotel Details">Select Room</th>
									</tr>
							
									<xsl:for-each select="RoomOptions/RoomOption">
										<tr>
											<td>
												<xsl:value-of select="RoomType"/>
												<xsl:if test="RoomView != ''">
													<xsl:value-of select="concat(' - ', RoomView)"/>
												</xsl:if>
											</td>
											<td><xsl:value-of select="MealBasis"/></td>
											<td>
												<strong>
													<xsl:call-template name="GetSellingPrice">
														<xsl:with-param name="Value" select="Price + $roomflightprice" />
														<xsl:with-param name="Format" select="'######'" />
														<xsl:with-param name="Currency" select="$CurrencySymbol" />
														<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />														
													</xsl:call-template>
												</strong>
											</td>
											<td class="right book">
												<xsl:choose>
													<xsl:when test="$roomcount > 1">
														<input type="radio" class="radio" value="{Index}" name="rad_roomoption_{$roomnumber}" />
													</xsl:when>
													<xsl:otherwise>
														<input type="button" class="button primary small" value="Select" onclick="HotelDetails.SelectRoom('{Index}');" ml="Hotel Details" />														
													</xsl:otherwise>													
												</xsl:choose>
											</td>
										</tr>
									</xsl:for-each>
						
								</table>

							</xsl:for-each>
							
							<xsl:if test="$roomcount > 1">
								<input type="button" class="button primary small" value="Select" onclick="HotelDetails.SelectMultiRoom();" ml="Hotel Details" />
							</xsl:if>
			
						</div>
				
				
					</xsl:for-each>
			
				</div>
			</xsl:if>

			
			<!-- Hotel Facilities-->
			<div class="facilities">
				
				<xsl:if test="count(Property/Facilities/Facility[FacilityType = 'Hotel']) &gt; 0">
					<div class="detail clear">
						<h2 ml="Hotel Details">Hotel Facilities</h2>
						<ul id="ulHotelFacilities" class="facilties">
							<xsl:for-each select="Property/Facilities/Facility[FacilityType = 'Hotel'][position() &lt; 17]">
								<li>
									<xsl:value-of select="Facility"/>
									<xsl:text> </xsl:text>
								</li>
							</xsl:for-each>
						</ul>
						<xsl:if test="count(Property/Facilities/Facility[FacilityType = 'Hotel']) > 16">
							<a id="aShowHotelFacilities" onclick="f.Hide('ulHotelFacilities');f.Hide('aShowHotelFacilities');f.Show('ulMoreHotelFacilities');f.Show('aHideHotelFacilities');return false;" ml="Hotel Details">Show more</a>
							<ul id="ulMoreHotelFacilities" style="display:none" class="facilties">
								<xsl:for-each select="Property/Facilities/Facility[FacilityType = 'Hotel'][position() &lt; 17]">
									<li>
										<xsl:value-of select="Facility"/>
										<xsl:text> </xsl:text>
									</li>
								</xsl:for-each>
							</ul>
							<a id="aHideHotelFacilities" onclick="f.Show('ulHotelFacilities');f.Show('aShowHotelFacilities');f.Hide('ulMoreHotelFacilities');f.Hide('aHideHotelFacilities');return false;" ml="Hotel Details">Show more</a>
						</xsl:if>
					</div>
				</xsl:if>

				<xsl:if test="count(Property/Facilities/Facility[FacilityType = 'Room']) &gt; 0">
					<div class="detail clear">
						<h2 ml="Hotel Details">Room Facilities</h2>
						<ul id="ulRoomFacilities" class="facilties">
							<xsl:for-each select="Property/Facilities/Facility[FacilityType = 'Room'][position() &lt; 17]">
								<li>
									<xsl:value-of select="Facility"/>
									<xsl:text> </xsl:text>
								</li>
							</xsl:for-each>
						</ul>
						<xsl:if test="count(Property/Facilities/Facility[FacilityType = 'Room']) > 16">
							<a id="aShowRoomFacilities" onclick="f.Hide('ulRoomFacilities');f.Hide('aShowRoomFacilities');f.Show('ulMoreRoomFacilities');f.Show('aHideRoomFacilities');return false;" ml="Hotel Details">Show more</a>
							<ul id="ulMoreRoomFacilities" style="display:none" class="facilties">
								<xsl:for-each select="Property/Facilities/Facility[FacilityType = 'Room'][position() &lt; 17]">
									<li>
										<xsl:value-of select="Facility"/>
										<xsl:text> </xsl:text>
									</li>
								</xsl:for-each>
							</ul>
							<a id="aHideRoomFacilities" onclick="f.Show('ulRoomFacilities');f.Show('aShowRoomFacilities');f.Hide('ulMoreRoomFacilities');f.Hide('aHideRoomFacilities');return false;" ml="Hotel Details">Show more</a>
						</xsl:if>
					</div>
				</xsl:if>

				<xsl:if test="/SEOPage/PropertyContent/HotelPolicy != ''">
					<div class="detail">
						<h2 ml="Hotel Details">Hotel Policy</h2>
						<xsl:call-template name="Markdown">
							<xsl:with-param name="text" select="/SEOPage/PropertyContent/HotelPolicy"/>
						</xsl:call-template>
					</div>
				</xsl:if>
			</div>

			
			<xsl:if test="count(Property/Hotel/Rooms) > 0">
				<a id="aBackToResults" href="#" onclick="window.location='/search-results'" ml="Hotel Details">&lt; Back to Search results</a>				
			</xsl:if>

		</div>

</xsl:template>
	
</xsl:stylesheet>
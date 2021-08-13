<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="CurrencySymbol"/>
	<xsl:param name="CurrencySymbolPosition"/>
	<xsl:param name="CMSBaseURL" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="SeatReservations"/>
	<xsl:param name="SeatReservationsChanged"/>
	<xsl:param name="LogoutRedirectURL"/>
	<xsl:param name="Theme"/>
	<xsl:param name="UseShortDates" />
	<xsl:param name="SupressPropertyLandingPages" />
	

	<xsl:template match="/">
		
		<div id="divMyBookings">
			<xsl:if test="$CSSClassOverride != ''">
				<xsl:attribute name="class">
					<xsl:value-of select="$CSSClassOverride"/>
				</xsl:attribute>
			</xsl:if>

			<input type="hidden" id="hidLogoutRedirectURL" value="{$LogoutRedirectURL}"/>
			
			<a href="#" id="aLogOut" class="button primary" onclick="MyBookings.LogOut();" ml="My Booking">Leave Manage My Booking</a>

			<h1 ml="My Booking">Manage My Booking</h1>

			<p>
				<trans ml="My Booking">To view or email your hotel booking confirmation voucher select</trans><xsl:text> </xsl:text><strong ml="My Booking">View Documents</strong>
				<xsl:text> </xsl:text><trans ml="My Booking">or</trans><xsl:text> </xsl:text>
				<strong ml="My Booking">Email Documents</strong>
				<xsl:text>. </xsl:text>
				<trans ml="My Booking">You can also choose to Amend or Cancel a booking below.</trans>
			</p>
			<p>
				<strong ml="My Booking">Amend Booking</strong>
				<xsl:text>: </xsl:text>
				<trans ml="My Booking">
					you will be able to request a new booking and if available we will cancel your current booking and secure a new booking
					for you. If your request is not available we will contact you within 48 hours to let you know. It is important to note that when we secure a new
					booking for you, the new booking will need to be paid in full and any refund amount from your previous booking minus applicable cancellation charges
					will be refunded within 15 working days.
				</trans>
			</p>
			<p>
				<strong ml="My Booking">Cancel Booking</strong>
				<xsl:text>: </xsl:text>
				<trans ml="My Booking">a cancellation fee may be charged. This fee will be disclosed during the cancellation process.</trans>
			</p>

			<xsl:for-each select="MyBookings/GetBookingDetailsResponse">

				<div class="booking" id="divBooking_{BookingReference}">
					
					<h2>
						<trans ml="My Booking">Booking Details</trans>
						<xsl:text> (</xsl:text>
						<trans ml="My Booking">Ref</trans>
						<xsl:text> </xsl:text>
						<xsl:value-of select="BookingReference"/>
						<xsl:text>)</xsl:text>
					</h2>

					<xsl:for-each select="Properties/Property">

					<div class="mainImage">
						
						<xsl:choose>
							<xsl:when test ="$SupressPropertyLandingPages = 'True'">
								<img alt="{Name}" src="{MainImage}" class="mainImage"/>								
							</xsl:when>
							<xsl:otherwise>
								<a href="{URL}">
									<img alt="{Name}" src="{MainImage}" class="mainImage"/>
								</a>								
							</xsl:otherwise>
						</xsl:choose>	
						
					</div>
					
					<div class="details">

						<h3>

							<xsl:choose>
								<xsl:when test ="$SupressPropertyLandingPages = 'True'">
									<xsl:value-of select="Name"/>
								</xsl:when>
								<xsl:otherwise>
									<a href="{URL}">
										<xsl:value-of select="Name"/>
									</a>
								</xsl:otherwise>
							</xsl:choose>

						</h3>

						<xsl:call-template name="StarRating">
							<xsl:with-param name="Rating" select="Rating" />
						</xsl:call-template>

						<dl>
							<dt>
								<trans ml="My Booking">Rooms</trans><xsl:text>:</xsl:text>
							</dt>
							<xsl:for-each select="Rooms/Room">
								<dd class="rooms">
									<xsl:attribute name="class">
										<xsl:if test="position() != 1">
											multi
										</xsl:if>
									</xsl:attribute>
									<xsl:value-of select="RoomType"/>
									<xsl:text> - </xsl:text>
									<xsl:value-of select="MealBasis"/>
								</dd>
							</xsl:for-each>
							<dt>
								<trans ml="My Booking">Location</trans><xsl:text>:</xsl:text>
							</dt>
							<dd>
								<xsl:value-of select="Resort"/>
							</dd>
							<dt>
								<trans ml="My Booking">Check In Date</trans>
								<xsl:text>:</xsl:text>
							</dt>
							<dd class="checkIn">
								<xsl:call-template name="ShortDate">
									<xsl:with-param name="SQLDate" select="ArrivalDate"/>
								</xsl:call-template>
							</dd>
							<dt>
								<trans ml="My Booking">Check Out Date</trans>
								<xsl:text>:</xsl:text>
							</dt>
							<dd>
								<xsl:call-template name="ShortDate">
									<xsl:with-param name="SQLDate" select="ReturnDate" />
								</xsl:call-template>
							</dd>
							<dt>
								<trans ml="My Booking">Number Of Nights</trans>
								<xsl:text>:</xsl:text>
							</dt>
							<dd>
								<xsl:value-of select="Duration"/>
							</dd>
						</dl>
					</div>
						
					</xsl:for-each>

					<xsl:if test="Status != 'Cancelled'">
					<div class="sidebarBox primary">
						<div class="boxTitle">
							<h2 ml="My Booking">Options</h2>
						</div>
						<div class="actionLinks" id="divActionLinks_{BookingReference}">
							<a id="aViewDocuments" href="#" onclick="MyBookings.ViewDocumentation({BookingReference});return false;" ml="My Booking">View Documents</a>
							<a id="aEmailDocuments" href="#" onclick="MyBookings.SendDocumentation({BookingReference});return false;" ml="My Booking">Email Documents</a>
							<a id="aAmendBooking" href="#" onclick="MyBookings.ShowAmendmentPopup({BookingReference});return false;" ml="My Booking">Amend Booking</a>
							<a id="aCancelBooking" href="#" onclick="MyBookings.ShowCancellationPopup({BookingReference});return false;" ml="My Booking">Cancel Booking</a>
						</div>
					</div>
					</xsl:if>

					<xsl:if test="count(Properties/Property) &gt; 0">
						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</xsl:if>

					<xsl:for-each select="Flights/Flight">

						<div class="outbound">

							<h3 ml="My Booking">Outbound Flight</h3>

							<h5>
								<xsl:value-of select="DepartureAirport"/>
								<xsl:text> - </xsl:text>
								<xsl:value-of select="ArrivalAirport"/>
							</h5>

							<dl>
								<dt>
									<trans ml="My Booking">Departs</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="OutboundDepartureDate"/>
									</xsl:call-template>
									<xsl:text> </xsl:text>
									<xsl:value-of select="OutboundDepartureTime"/>
								</dd>
								<dt>
									<trans ml="My Booking">Arrives</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="OutboundArrivalDate"/>
									</xsl:call-template>
									<xsl:text> </xsl:text>
									<xsl:value-of select="OutboundArrivalTime"/>
								</dd>
								<xsl:if test="$SeatReservations = 'true'">
									<xsl:for-each select="FlightPassengers/FlightPassenger">
										<xsl:if test="hlpOutboundSeatCode != '' or count(../../SeatMaps/SeatMap[Direction='Outbound']) &gt; 0">
											<dt>
												<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ':')"/>
											</dt>
											<dd>
												<xsl:choose>
													<xsl:when test="hlpOutboundSeatCode != ''">
														<trans ml="My Booking">Seat</trans>
														<xsl:text> </xsl:text>
														<xsl:value-of select="hlpOutboundSeatCode"/>
													</xsl:when>
													<xsl:otherwise>
														<trans ml="My Booking">No seat selected</trans>
													</xsl:otherwise>
												</xsl:choose>
												<a class="textlink" href="#" onclick="MyBookings.EditSeatReservations({../../FlightBookingID}, 0);return false;" ml="My Booking">Edit</a>
											</dd>
										</xsl:if>
									</xsl:for-each>
								</xsl:if>
							</dl>
							
						</div>

						<div class="return">

							<h3 ml="My Booking">Return Flight</h3>

							<h5>
								<xsl:value-of select="ArrivalAirport"/>
								<xsl:text> - </xsl:text>
								<xsl:value-of select="DepartureAirport"/>
							</h5>

							<dl>
								<dt>
									<trans ml="My Booking">Departs</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="ReturnDepartureDate"/>
									</xsl:call-template>
									<xsl:text> </xsl:text>
									<xsl:value-of select="ReturnDepartureTime"/>
								</dd>
								<dt>
									<trans ml="My Booking">Arrives</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="ReturnArrivalDate"/>
									</xsl:call-template>
									<xsl:text> </xsl:text>
									<xsl:value-of select="ReturnArrivalTime"/>
								</dd>
								<xsl:if test="$SeatReservations = 'true'">
									<xsl:for-each select="FlightPassengers/FlightPassenger">
										<xsl:if test="hlpReturnSeatCode != '' or count(../../SeatMaps/SeatMap[Direction='Return']) &gt; 0">
											<dt>
												<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ':')"/>
											</dt>
											<dd>
												<xsl:choose>
													<xsl:when test="hlpReturnSeatCode != ''">
														<trans ml="My Booking">Seat</trans>
														<xsl:text> </xsl:text>
														<xsl:value-of select="hlpReturnSeatCode"/>
													</xsl:when>
													<xsl:otherwise>
														<trans ml="My Booking">No seat selected</trans>
													</xsl:otherwise>
												</xsl:choose>
												<a class="textlink" href="#" onclick="MyBookings.EditSeatReservations({../../FlightBookingID}, 1);return false;" ml="My Booking">Edit</a>
											</dd>
										</xsl:if>
									</xsl:for-each>
								</xsl:if>
							</dl>

						</div>
						
						<div class="clear">
							<xsl:text> </xsl:text>
						</div>

						<xsl:if test="BaggageQuantity > 0">
							<p class="baggage">
								<trans ml="My Booking">Baggage</trans>
								<xsl:text>: </xsl:text>
								<xsl:value-of select="BaggageQuantity"/>
								<xsl:text> </xsl:text>
								<xsl:choose>
									<xsl:when test="BaggageQuantity = 1">
										<trans ml="My Booking">Bag</trans>
									</xsl:when>
									<xsl:otherwise>
										<trans ml="My Booking">Bags</trans>
									</xsl:otherwise>
								</xsl:choose>
							</p>
						</xsl:if>
						
						<xsl:if test="$SeatReservationsChanged = 'true'">
							<div id="divSeatsConfirmed_{FlightBookingID}">
								<h5 ml="SeatSelection">Thank you, your seats were confirmed successfully.</h5>
								<p ml="SeatSelection">Please review your new seats above.</p>
							</div>
						</xsl:if>

						<div id="divSeatSelection_{FlightBookingID}" class="box primary clear seatselection" style="display:none;">
							
							<xsl:variable name="OriginalPrice" select="sum(SeatMaps/SeatMap/Seats/Seat[BookedGuestID!=0]/Price)"/>
							
							<div class="boxTitle">
								<h2>
									<trans ml="SeatSelection">Select Seat</trans>
									<a id="aCloseSelection" class="symbol" href="#" onclick="int.f.Hide('divSeatSelection_' + {FlightBookingID});return false;">x</a>
								</h2>
							</div>

							<div class="tabbedBox clear">
								<div class="tabs">
									<ul>
										<!-- tab for each direction -->
										<xsl:for-each select="SeatMaps/SeatMap">
											<li class="tab">
												<xsl:if test="position() = 1">
													<xsl:attribute name="class">tab selected</xsl:attribute>
												</xsl:if>
												<a href="javascript:MyBookings.ChangeSeatSelectionTab({../../FlightBookingID})" ml="SeatSelection">
													<xsl:value-of select="Direction"/>
												</a>
											</li>
										</xsl:for-each>
									</ul>
								</div>
								<xsl:for-each select="SeatMaps/SeatMap">
									<xsl:variable name="direction" select="Direction" />

									<div id="divSeatMap__{FlightBookingID}_{$direction}" class="tabContent">
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
												<xsl:for-each select="../../FlightPassengers/FlightPassenger">
													<xsl:variable name="PassengerID" select="FlightBookingPassengerID"/>
													<tr>
														<td>
															<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName)"/>
														</td>
														<td>
															<select id="ddlSeats_{../../FlightBookingID}_{FlightBookingPassengerID}_{$direction}" onchange="MyBookings.SelectSeat({../../FlightBookingID}, {FlightBookingPassengerID}, '{$direction}', {$OriginalPrice});">
																<option value="0_0">
																	<xsl:if test="count($seats/Seat[BookedGuestID=$PassengerID]) = 0">
																		<xsl:attribute name="selected">selected</xsl:attribute>
																	</xsl:if>
																</option>
																<xsl:for-each select="$seats/Seat">
																	<option value="{SeatToken}_{Price}">
																		<xsl:if test="BookedGuestID=$PassengerID">
																			<xsl:attribute name="selected">selected</xsl:attribute>
																		</xsl:if>
																		<xsl:value-of select="SeatCode"/>
																	</option>
																</xsl:for-each>
															</select>
														</td>
														<td>
															<xsl:if test="$CurrencySymbolPosition='Prepend'">
																<xsl:value-of select="$CurrencySymbol"/>
															</xsl:if>
															<span id="spnSeatPrice_{../../FlightBookingID}_{FlightBookingPassengerID}_{$direction}">
																<xsl:choose>
																	<xsl:when test="count($seats/Seat[BookedGuestID=$PassengerID]) &gt; 0">
																		<xsl:value-of select="$seats/Seat[BookedGuestID=$PassengerID]/Price"/>
																	</xsl:when>
																	<xsl:otherwise>0</xsl:otherwise>
																</xsl:choose>
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

							<div class="confirm clear">

								<div class="totalprice">
									<trans ml="SeatSelection">Total Price</trans>
									<xsl:text> </xsl:text>
									<xsl:if test="$CurrencySymbolPosition='Prepend'">
										<xsl:value-of select="$CurrencySymbol"/>
									</xsl:if>
									<span id="spnTotalSeatPrice_{FlightBookingID}">
										<xsl:value-of select="$OriginalPrice"/>
									</span>
									<xsl:if test="$CurrencySymbolPosition='Append'">
										<xsl:value-of select="$CurrencySymbol"/>
									</xsl:if>
								</div>
								<div class="totalprice">
									<trans ml="SeatSelection">To Pay</trans>
									<xsl:text> </xsl:text>
									<xsl:if test="$CurrencySymbolPosition='Prepend'">
										<xsl:value-of select="$CurrencySymbol"/>
									</xsl:if>
									<span id="spnAmountDue_{FlightBookingID}">0</span>
									<xsl:if test="$CurrencySymbolPosition='Append'">
										<xsl:value-of select="$CurrencySymbol"/>
									</xsl:if>
								</div>
								
								<a id="btnConfirmSeatSelection" class="button tertiary" href="javascript:MyBookings.ConfirmSeatSelection({../../BookingReference},{FlightBookingID}, {$OriginalPrice});" ml="My Booking">Confirm Selection</a>
							</div>

						</div>
						
					</xsl:for-each>

					<xsl:for-each select="Transfers/Transfer">

						<div class="transfer outbound">

							<h3 ml="My Booking">Transfer</h3>

							<h5>
								<xsl:if test="DepartureParentName != ''">
									<xsl:value-of select="DepartureParentName"/>
								</xsl:if>
								<xsl:if test="DepartureParentName = ''">
									<xsl:value-of select="OutboundDetails/JourneyOrigin"/>
								</xsl:if>
								<xsl:text> - </xsl:text>
								<xsl:value-of select="ArrivalParentName"/>
							</h5>

							<dl>
								<dt>
									<trans ml="My Booking">Passengers</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:value-of select="Adults"/>
									<xsl:text> </xsl:text>
									<trans ml="My Booking">
										<xsl:text> adult</xsl:text>
										<xsl:if test="Adults &gt; 1">
											<xsl:text>s</xsl:text>
										</xsl:if>
									</trans>
									<xsl:if test="Children &gt; 0">
										<xsl:if test="Infants = 0">
											<trans ml="My Booking">and</trans>
										</xsl:if>
										<xsl:if test="Infants &gt; 0">
											<xsl:text>,</xsl:text>
										</xsl:if>
										<xsl:value-of select="concat (' ', Children, ' ')"/>
										<trans ml="My Booking">
											<xsl:text>child</xsl:text>
											<xsl:if test="Children &gt; 1">
												<xsl:text>ren</xsl:text>
											</xsl:if>
										</trans>
									</xsl:if>
									<xsl:if test="Infants &gt; 0">
										<xsl:text> </xsl:text>
										<trans ml="My Booking">and</trans>
										<xsl:value-of select="concat(' ', Infants, ' ')"/>
										<trans ml="My Booking">
											<xsl:text>infant</xsl:text>
											<xsl:if test="Infants &gt; 1">
												<xsl:text>s</xsl:text>
											</xsl:if>
										</trans>
									</xsl:if>
								</dd>
								<dt>
									<trans ml="My Booking">Flight Code</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:value-of select="DepartureFlightCode"/>
								</dd>
								<dt>
									<trans ml="My Booking">Flight Time</trans>
									<xsl:text>:</xsl:text>
								</dt>
								<dd>
									<xsl:value-of select="DepartureTime"/>
									<xsl:text> on </xsl:text>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="DepartureDate"/>
									</xsl:call-template>
								</dd>
							</dl>
							
						</div>

						<xsl:if test="OnewWay = 'false'">

							<div class="transfer return">

								<h3 ml="My Booking">Transfer (Return)</h3>

								<h5>
									<xsl:value-of select="ArrivalParentName"/>
									<xsl:text> - </xsl:text>
									<xsl:if test="DepartureParentName != ''">
										<xsl:value-of select="DepartureParentName"/>
									</xsl:if>
									<xsl:if test="DepartureParentName = ''">
										<xsl:value-of select="OutboundDetails/JourneyOrigin"/>
									</xsl:if>
								</h5>
								
								<dl>
									<dt>
										<trans ml="My Booking">Passengers</trans>
										<xsl:text>:</xsl:text>
									</dt>
									<dd>
										<xsl:value-of select="Adults"/>
										<xsl:text> </xsl:text>
										<trans ml="My Booking">
											<xsl:text> adult</xsl:text>
											<xsl:if test="Adults &gt; 1">
												<xsl:text>s</xsl:text>
											</xsl:if>
										</trans>
										<xsl:if test="Children &gt; 0">
												<xsl:if test="Infants = 0">
													<trans ml="My Booking">and</trans>
												</xsl:if>
												<xsl:if test="Infants &gt; 0">
													<xsl:text>,</xsl:text>
												</xsl:if>
											<xsl:value-of select="concat (' ', Children, ' ')"/>
											<trans ml="My Booking">
												<xsl:text>child</xsl:text>
												<xsl:if test="Children &gt; 1">
													<xsl:text>ren</xsl:text>
												</xsl:if>
											</trans>
										</xsl:if>
										<xsl:if test="Infants &gt; 0">
											<xsl:text> </xsl:text>
											<trans ml="My Booking">and</trans>
											<xsl:value-of select="concat(' ', Infants, ' ')"/>
											<trans ml="My Booking">
												<xsl:text>infant</xsl:text>
												<xsl:if test="Infants &gt; 1">
													<xsl:text>s</xsl:text>
												</xsl:if>
											</trans>
										</xsl:if>								
									</dd>
									<dt>
										<trans ml="My Booking">Flight Code</trans>
										<xsl:text>:</xsl:text>
									</dt>
									<dd>
										<xsl:value-of select="ReturnFlightCode"/>
									</dd>
									<dt>
										<trans ml="My Booking">Flight Time</trans>
										<xsl:text>:</xsl:text>
									</dt>
									<dd>								
										<xsl:value-of select="ReturnTime"/>
										<xsl:text> on </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnDate"/>
										</xsl:call-template>
									</dd>
								</dl>

							</div>

						</xsl:if>

						<div class="clear">
							<xsl:text> </xsl:text>
						</div>

					</xsl:for-each>

					<xsl:if test="Status='Cancelled'">
						<div class="cancelledBooking" ml="My Booking">This booking has been cancelled</div>
					</xsl:if>

					<xsl:if test="Status!='Cancelled'">
						<div class="totalprice" id="divTotalPrice_{BookingReference}">
							<trans ml="My Booking" mlparams="{CurrencySymbol}|{TotalPrice}">
								Total Price in {0} is {1}
							</trans>
							<xsl:if test="number(CustomerPayment/TotalOutstanding) &lt;= 0">
								<xsl:text> </xsl:text>
								<span>
									<xsl:text>(Fully paid)</xsl:text>
								</span>
							</xsl:if>
						</div>
					</xsl:if>

				</div>

			</xsl:for-each>
		</div>

		<div id="divAmendment" class="action" style="display:none;">

			<h1 ml="My Booking">Booking Amendment</h1>
		
			<!-- Request Form -->
			<div id="divAmendmentForm">
				<a href="#" onclick="web.ModalPopup.Hide();return false;" class="close">
					<xsl:text> </xsl:text>
				</a>
				<p ml="My Booking">
					Enter details of your new booking below. It is important to note that you are requesting a new booking and if this is available we will cancel your current booking and secure
					a new booking for you. If your request is not available we will contact you within 48 hours to let you know. When we secure a new booking for you, the new booking will need to
					be paid in full and any refund from your previous booking minus applicable cancellation charges will be refunded within 15 working days
				</p>

				<div id="divAmendmentInputs" class="form">

					<div class="left">
						<div class="textbox icon calendar right embedded">
							<label ml="My Booking">Check In Date</label>
							<i class="checkin">
								<xsl:text> </xsl:text>
							</i>
							<input id="txtAmendmentDepartureDate" name="txtAmendmentDepartureDate" type="text" class="textbox"/>
						</div>
						<div class="textbox icon calendar right embedded">
							<label ml="My Booking">Check Out Date</label>
							<i class="checkout">
								<xsl:text> </xsl:text>
							</i>
							<input id="txtAmendmentReturnDate" name="txtAmendmentReturnDate" type="text" class="textbox"/>
						</div>
						<div>
							<label ml="My Booking">Destination</label>
							<input type="text" id="txtDestination" class="textbox"/>
							<span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your destination.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
							  onmouseout="f.HidePopup();">
								<xsl:text> </xsl:text>
							</span>
						</div>
						<div class="bottom">
							<label ml="My Booking">Hotel Name</label>
							<input type="text" id="txtHotelName" class="textbox"/>
							<span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your hotel.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
							  onmouseout="f.HidePopup();">
								<xsl:text> </xsl:text>
							</span>
						</div>
						<input type="hidden" id="hidUseShortDates" value="{$UseShortDates}" />
					</div>

					<div class="left end">
						<div>
							<label ml="My Booking" mlparams="{$CurrencySymbol}">Max Total Rate {0}</label>
							
							<select id="ddlTotalRate">
								<option>100</option>
								<option>150</option>
								<option>200</option>
								<option>300</option>
								<option>400</option>
								<option>500</option>
								<option>600</option>
								<option>700</option>
								<option>800</option>
								<option>900</option>
								<option>1000</option>
								<option>1500</option>
								<option>2000</option>
								<option>2500</option>
								<option>5000</option>
								<option>10000</option>
								<option>15000</option>
								<option>20000</option>
							</select>
							<span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Please indicate a max total amount that you authorise us to charge you. You will be charged via the payment method that you used to make the original booking.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
							  onmouseout="f.HidePopup();">
								<xsl:text> </xsl:text>
							</span>
						</div>
						<div class="bottom">
							<label class="amendment" ml="My Booking">Additional Information</label>
							<textarea id="txtAdditionalInformation" rows="4">
								<xsl:text> </xsl:text>
							</textarea>
						</div>
					</div>
					<div class="clear">
						<xsl:text> </xsl:text>
					</div>
					<div class="authorise">
						<input type="checkbox" id="chkAuthorise"/>
						<label id="lblAuthorise" for="chkAuthorise" ml="My Booking">I permit you to cancel my existing booking and make a new booking based on the new details and authorised amount provided.</label>
					</div>
				</div>

				<div class="confirm">
					<a class="button tertiary" onclick="MyBookings.RequestAmendmentValidate();" ml="My Booking">
						Confirm
					</a>
				</div>
			</div>

			<!-- Wait Message -->
			<div id="divAmendRequestWaitMessage" style="display:none;">
				<p ml="My Booking">Please wait as we send through your request.</p>
				<img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
			</div>

			<!-- Request Sent -->
			<div id="divRequestSent" style="display:none;">
				<p ml="My Booking">Your booking amendment request has been sent to our reservations team and they will contact you within 48 hours to confirm your booking details.</p>
			</div>

		</div>

		<div id="divCancellation" class="action" style="display:none;">

			<h1 ml="My Booking">Request Cancellation</h1>
			<a href="#" onclick="web.ModalPopup.Hide();return false;" class="close">
				<xsl:text> </xsl:text>
			</a>

			<!-- Cancellation Form -->
			<div id="divCancellationForm">
				<p ml="My Booking">
					To cancel your hotel booking, you will need to accept the cancellation charges and click Confirm. The cancellation charges are displayed below. Any refund due
					after applicable cancellation charges will be credited to the payment method that you used when you completed the booking. Note, it can take up to 15 working days for the
					refunded amount to clear.
				</p>

				<div id="divCancellationInputs" class="form">
					<h5 ml="My Booking">Cancellation Charge</h5>
					<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
						<span>
							<xsl:value-of select="$CurrencySymbol"/>
						</span>
					</xsl:if>
					<span id="spnCancellationCost">
						<xsl:text> </xsl:text>
					</span>
					<xsl:if test="$CurrencySymbolPosition = 'Append'">
						<span>
							<xsl:value-of select="$CurrencySymbol"/>
						</span>
					</xsl:if>
					<input type="hidden" id="hidCancellationToken" value=""/>
					<div class="authorise">
						<input id="chkConfirmCancel" type="checkbox"/>
						<label for="chkConfirmCancel" id="lblConfirmCancel" ml="My Booking">Please tick to confirm you wish to cancel this booking</label>
					</div>

				</div>

				<div class="confirm">
					<a id="btnConfirmCancellation" class="button tertiary" href="javascript:MyBookings.RequestCancellationValidate()" ml="My Booking">
						Confirm
					</a>
				</div>
			</div>


			<!-- Wait Message -->
			<div id="divCancellationWaitMessage" style="display:none">
				<p ml="My Booking">Please wait as we process your request. Please do not leave this page.</p>
				<img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
			</div>

			<!-- Success -->
			<div id="divCancellationConfirmation" style="display:none">
				<p ml="My Booking">You will receive a confirmation email shortly to confirm that your booking has been cancelled.</p>
			</div>

			<!-- Failure -->
			<div id="divCancellationFailed" style="display:none">
				<p ml="My Booking">Cancellation attempt failed.</p>
			</div>

		</div>

		<div id="divEmailDocuments" class="action" style="display:none;">

			<h1 ml="My Booking">Email Documents</h1>

			<!-- Wait Message -->
			<div id="divEmailWaitMessage">
				<p ml="My Booking">Please wait as we process your request.</p>
				<img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
			</div>

			<!-- Request Sent -->
			<div id="divEmailSuccess" style="display:none;">
				<p ml="My Booking">Thank You. Your documents have been sent successfully.</p>
			</div>

		</div>


		<div id="divViewDocuments" class="action" style="display:none;">

			<h1 ml="My Booking">View Documents</h1>

			<!-- Wait Message -->
			<div>
				<p ml="My Booking">Please wait as we process your request.</p>
				<img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
			</div>


		</div>
		
	</xsl:template>

	<!-- Star Rating -->
	<xsl:template name="StarRating">
		<xsl:param name="Rating" />
		<xsl:param name="Small" select="'false'"/>


		<xsl:variable name="class">
			<xsl:text>rating star</xsl:text>
			<xsl:value-of select="substring($Rating,1,1)"/>

			<xsl:if test="substring($Rating,3,1)='5'">
				<xsl:text> half</xsl:text>
			</xsl:if>

			<xsl:if test="$Small = 'true'">
				<xsl:text> small</xsl:text>
			</xsl:if>
		</xsl:variable>


		<span class="{$class}">
			<xsl:text> </xsl:text>
		</span>

	</xsl:template>

	<!-- Date Functions -->
	<xsl:template name="ShortDate">
		<xsl:param name="SQLDate"/>
		<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>

		<xsl:value-of select="substring($SQLDate,9,2)"/>
		<xsl:choose>
			<xsl:when test="$MonthNumber='01'">
				<xsl:text> Jan '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='02'">
				<xsl:text> Feb '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='03'">
				<xsl:text> Mar '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='04'">
				<xsl:text> Apr '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='05'">
				<xsl:text> May '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='06'">
				<xsl:text> Jun '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='07'">
				<xsl:text> Jul '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='08'">
				<xsl:text> Aug '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='09'">
				<xsl:text> Sep '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='10'">
				<xsl:text> Oct '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='11'">
				<xsl:text> Nov '</xsl:text>
			</xsl:when>
			<xsl:when test="$MonthNumber='12'">
				<xsl:text> Dec '</xsl:text>
			</xsl:when>
		</xsl:choose>
		<xsl:value-of select="substring($SQLDate,3,2)"/>

	</xsl:template>

	<!-- get selling price -->
	<xsl:template name="GetSellingPrice">
		<xsl:param name="Value"/>
		<xsl:param name="Exchange" select="'1'"/>
		<xsl:param name="Currency"/>
		<xsl:param name="Format" select="'###,##0.00'"/>
		<xsl:param name="CurrencySymbolPosition" select="'Prepend'"/>
		<xsl:param name="RoundingRule" select="'Unrounded'"/>

		<!-- do the exchange -->
		<xsl:variable name="ConvertedValue" select="$Value * $Exchange"/>

		<xsl:variable name="RoundingInteger" select="floor($ConvertedValue) mod 10"/>

		<!--Round-->
		<xsl:variable name="RoundedValue">
			<xsl:choose>
				<xsl:when test="$RoundingRule = 'Round' or $RoundingRule = 'Round (Total)'">
					<xsl:value-of select="round($ConvertedValue)" />
				</xsl:when>
				<xsl:when test="$RoundingRule = 'Round Up' or $RoundingRule = 'Round Up (Total)'">
					<xsl:value-of select="ceiling($ConvertedValue)" />
				</xsl:when>
				<xsl:when test="$RoundingRule = '5s and 9s'">
					<xsl:choose>
						<xsl:when test="$RoundingInteger = 0 or $RoundingInteger = 6">
							<xsl:value-of select="floor($ConvertedValue) - 1"/>
						</xsl:when>
						<xsl:when test="$RoundingInteger = 1">
							<xsl:value-of select="floor($ConvertedValue) - 2"/>
						</xsl:when>
						<xsl:when test="$RoundingInteger = 2">
							<xsl:value-of select="floor($ConvertedValue) + 3"/>
						</xsl:when>
						<xsl:when test="$RoundingInteger = 3 or $RoundingInteger = 7">
							<xsl:value-of select="floor($ConvertedValue) + 2"/>
						</xsl:when>
						<xsl:when test="$RoundingInteger = 4 or $RoundingInteger = 8">
							<xsl:value-of select="floor($ConvertedValue) + 1"/>
						</xsl:when>
						<xsl:when test="$RoundingInteger = 5 or $RoundingInteger = 9">
							<xsl:value-of select="floor($ConvertedValue)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$ConvertedValue"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$ConvertedValue"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="Sign">
			<xsl:if test="$RoundedValue &lt; 0">-</xsl:if>
		</xsl:variable>

		<xsl:variable name="NewValue">
			<xsl:choose>
				<xsl:when test="$RoundedValue &lt; 0">
					<xsl:value-of select="$RoundedValue * -1"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$RoundedValue"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>


		<xsl:value-of select="$Sign"/>
		<xsl:if test="$CurrencySymbolPosition='Prepend'">
			<xsl:value-of select="$Currency"/>
		</xsl:if>
		<xsl:value-of select="format-number($NewValue, $Format)"/>
		<xsl:if test="$CurrencySymbolPosition='Append'">
			<xsl:value-of select="$Currency"/>
		</xsl:if>

	</xsl:template>

</xsl:stylesheet>

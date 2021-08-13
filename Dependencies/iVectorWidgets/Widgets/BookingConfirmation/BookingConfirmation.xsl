<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="TradeSite" />
	<xsl:param name="SummaryTitle" />
	<xsl:param name="DetailedPriceBreakdown" />
	<xsl:param name="SummaryIncludeDates" />
	<xsl:param name="PriceFormat" />
	<xsl:param name="SubText" />

	<xsl:template match="/">

		<div id="divConfirmation">

			<xsl:for-each select="GetBookingDetailsResponse">

				<xsl:variable name="FlightSupplierPaymentAmount" select="BookingBasket/FlightSupplierPaymentAmount" />

				<div>

					<div class="box primary">

						<div class="boxTitle">
							<h2>
								<xsl:choose>
									<xsl:when test ="$SummaryTitle != ''">
										<trans ml="Booking Confirmation">
											<xsl:value-of select ="$SummaryTitle" />
										</trans>
									</xsl:when>
									<xsl:otherwise>
										<trans ml="Booking Confirmation">Your Reservation</trans>
									</xsl:otherwise>
								</xsl:choose>
							</h2>
						</div>

						<xsl:if test="$SubText != ''">
							<p>
								<xsl:value-of select="$SubText" />
							</p>
						</xsl:if>

						<dl>
							<dt ml="Booking Confirmation">Booking Reference</dt>
							<dd>
								<xsl:value-of select="BookingReference" />
							</dd>

							<xsl:if test="$TradeSite = 'true'">
								<xsl:if test="TradeReference !=''">
									<dt ml="Booking Confirmation">Your Reference</dt>
									<dd>
										<xsl:value-of select="TradeReference" />
									</dd>
								</xsl:if>

								<dt ml="Booking Confirmation">Status</dt>
								<dd>
									<xsl:value-of select="Status" />
								</dd>
							</xsl:if>

							<xsl:if test="$SummaryIncludeDates = 'true'">
								<dt ml="Booking Confirmation">Date of Departure</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="FirstDepartureDate" />
									</xsl:call-template>
								</dd>
							</xsl:if>

							<dt ml="Booking Confirmation">Lead Guest</dt>
							<dd class="multiline">
								<p>
									<xsl:value-of select="concat(LeadCustomer/CustomerTitle, ' ', LeadCustomer/CustomerFirstName, ' ', LeadCustomer/CustomerLastName)" />
									<xsl:text>, </xsl:text>
								</p>
								<xsl:if test="$TradeSite != 'true'">
									<p>
										<xsl:value-of select="LeadCustomer/CustomerAddress1" />
										<xsl:text>, </xsl:text>
									</p>
									<p>
										<xsl:value-of select="LeadCustomer/CustomerTownCity" />
										<xsl:text>, </xsl:text>
									</p>
									<p>
										<xsl:value-of select="LeadCustomer/CustomerPostcode" />
									</p>
									<p>
										<xsl:value-of select="LeadCustomer/CustomerPhone" />
									</p>
								</xsl:if>
							</dd>

							<xsl:choose>
								<xsl:when test="$DetailedPriceBreakdown = 'true'">
									<dt ml="Booking Confirmation">Total Price</dt>
									<dd>
										<xsl:call-template name="GetSellingPrice">
											<xsl:with-param name="Value" select="TotalPrice + $FlightSupplierPaymentAmount" />
											<xsl:with-param name="Format" select="$PriceFormat" />
											<xsl:with-param name="Currency" select="CurrencySymbol" />
											<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
										</xsl:call-template>
									</dd>

									<!--<dt ml="Booking Confirmation">Invoiced</dt>
							<dd>
								<xsl:text> .</xsl:text>
							</dd>-->

									<dt ml="Booking Confirmation">Paid</dt>
									<dd>
										<xsl:call-template name="GetSellingPrice">
											<xsl:with-param name="Value" select="TotalPaid + $FlightSupplierPaymentAmount" />
											<xsl:with-param name="Format" select="$PriceFormat" />
											<xsl:with-param name="Currency" select="CurrencySymbol" />
											<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
										</xsl:call-template>
									</dd>

									<dt ml="Booking Confirmation">Balance Due</dt>
									<dd>
										<xsl:call-template name="GetSellingPrice">
											<xsl:with-param name="Value" select="TotalOutstanding" />
											<xsl:with-param name="Format" select="$PriceFormat" />
											<xsl:with-param name="Currency" select="CurrencySymbol" />
											<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
										</xsl:call-template>
									</dd>

									<xsl:if test="BookingBasket/BookResponse/FlightBookings/FlightBookResponse != ''">
										<dt ml="Booking Confirmation">Flight Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/FlightBookings/FlightBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Flight Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/FlightBookings/FlightBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
									<xsl:if test="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse != ''">
										<dt ml="Booking Confirmation">Property Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Property Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
									<xsl:if test="BookingBasket/BookResponse/TransferBookings/TransferBookResponse != ''">
										<dt ml="Booking Confirmation">Transfer Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/TransferBookings/TransferBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Transfer Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/TransferBookings/TransferBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
								</xsl:when>
								<xsl:otherwise>
									<dt ml="Booking Confirmation">Total Price</dt>
									<dd>
										<xsl:call-template name="GetSellingPrice">
											<xsl:with-param name="Value" select="TotalPrice" />
											<xsl:with-param name="Format" select="$PriceFormat" />
											<xsl:with-param name="Currency" select="CurrencySymbol" />
											<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
										</xsl:call-template>
									</dd>
									<xsl:if test="BookingBasket/BookResponse/FlightBookings/FlightBookResponse != ''">
										<dt ml="Booking Confirmation">Flight Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/FlightBookings/FlightBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Flight Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/FlightBookings/FlightBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
									<xsl:if test="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse != ''">
										<dt ml="Booking Confirmation">Property Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Property Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/PropertyBookings/PropertyBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
									<xsl:if test="BookingBasket/BookResponse/TransferBookings/TransferBookResponse != ''">
										<dt ml="Booking Confirmation">Transfer Commission</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/TransferBookings/TransferBookResponse/TotalCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
										<dt ml="Booking Confirmation">Transfer Commission VAT</dt>
										<dd>
											<xsl:call-template name="GetSellingPrice">
												<xsl:with-param name="Value" select="BookingBasket/BookResponse/TransferBookings/TransferBookResponse/VATOnCommission" />
												<xsl:with-param name="Format" select="$PriceFormat" />
												<xsl:with-param name="Currency" select="CurrencySymbol" />
												<xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
											</xsl:call-template>
										</dd>
									</xsl:if>
								</xsl:otherwise>
							</xsl:choose>
						</dl>

						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</div>

					<xsl:for-each select="Properties/Property">
						<div class="box primary">

							<div class="boxTitle">
								<h2>
									<xsl:value-of select="Name" />
									<xsl:call-template name="StarRating">
										<xsl:with-param name="Rating" select="Rating" />
									</xsl:call-template>
								</h2>
							</div>

							<img src="{MainImage}" alt="{Name}" />

							<dl class="property">
								<dt ml="Booking Confirmation">Stay</dt>
								<dd>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="ArrivalDate" />
									</xsl:call-template>
									<xsl:text> </xsl:text>
									<trans ml="Booking Confirmation">to</trans>
									<xsl:text> </xsl:text>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="ReturnDate" />
									</xsl:call-template>
								</dd>
								<dt ml="Booking Confirmation">Location</dt>
								<dd>
									<xsl:value-of select="Resort" />
								</dd>
								<xsl:variable name="RoomCount" select="count(Rooms/Room)" />
								<xsl:for-each select="Rooms/Room">
									<dt>
										<trans ml="Booking Confirmation">Room</trans>
										<xsl:if test="$RoomCount &gt; 1">
											<xsl:text> </xsl:text>
											<xsl:value-of select="position()" />
										</xsl:if>
									</dt>
									<dd>
										<xsl:value-of select="RoomType" />
										<xsl:if test="/GetBookingDetailsResponse/RoomViewLookupList/RoomViews/RoomViewLookup[RoomViewID = RoomViewID]/RoomView !=''">
											<xsl:text> (</xsl:text>
											<xsl:value-of select="/GetBookingDetailsResponse/RoomViewLookupList/RoomViews/RoomViewLookup[RoomViewID = RoomViewID]/RoomView" />
											<xsl:text>)</xsl:text>
										</xsl:if>
										<xsl:text> - </xsl:text>
										<xsl:value-of select="MealBasis" />
									</dd>
									<dt ml="Booking Confirmation">Guests</dt>
									<dd class="multiline">
										<xsl:for-each select="GuestDetails/GuestDetail">
											<p>
												<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName)" />
											</p>
										</xsl:for-each>
									</dd>
								</xsl:for-each>
							</dl>

							<div class="clear">
								<xsl:text> </xsl:text>
							</div>
						</div>
					</xsl:for-each>

					<xsl:for-each select="Flights/Flight">
						<div class="box primary flights">

							<div class="boxTitle">
								<h2 ml="Booking Confirmation">Flights</h2>
							</div>

							<dl class="clear">
								<dt ml="Booking Confirmation">Outbound</dt>
								<dd class="multiline">
									<p>
										<trans ml="Booking Confirmation">Flight Carrier</trans>
										<xsl:text>: </xsl:text>
										<xsl:value-of select="FlightCarrier" />
									</p>
									<p>
										<trans ml="Booking Confirmation">Flight Number</trans>
										<xsl:text> </xsl:text>
										<xsl:value-of select="OutboundFlightCode" />
									</p>
									<p>
										<xsl:value-of select="DepartureAirport" />
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">to</trans>
										<xsl:text> </xsl:text>
										<xsl:value-of select="ArrivalAirport" />
									</p>
									<p>
										<trans ml="Booking Confirmation">Dep:</trans>
										<xsl:text> </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="OutboundDepartureDate" />
										</xsl:call-template>
										<xsl:text> </xsl:text>
										<xsl:value-of select="OutboundDepartureTime" />
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">Arr:</trans>
										<xsl:text> </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="OutboundArrivalDate" />
										</xsl:call-template>
										<xsl:text> </xsl:text>
										<xsl:value-of select="OutboundArrivalTime" />
									</p>
								</dd>
								<xsl:if test="count(FlightPassengers/FlightPassenger/hlpOutboundSeatCode) &gt; 0">
									<dt ml="Booking Confirmation">Seats</dt>
									<dd>
										<xsl:for-each select="FlightPassengers/FlightPassenger">
											<xsl:if test="hlpOutboundSeatCode and hlpOutboundSeatCode != ''">
												<p>
													<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ': ')" />
													<trans ml="Booking Confirmation">Seat</trans>
													<xsl:text> </xsl:text>
													<xsl:value-of select="hlpOutboundSeatCode" />
												</p>
											</xsl:if>
										</xsl:for-each>
									</dd>
								</xsl:if>
								<dt ml="Booking Confirmation">Inbound</dt>
								<dd class="multiline">
									<p>
										<trans ml="Booking Confirmation">Flight Number</trans>
										<xsl:text> </xsl:text>
										<xsl:value-of select="ReturnFlightCode" />
									</p>
									<p>
										<xsl:value-of select="ArrivalAirport" />
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">to</trans>
										<xsl:text> </xsl:text>
										<xsl:value-of select="DepartureAirport" />
									</p>
									<p>
										<trans ml="Booking Confirmation">Dep:</trans>
										<xsl:text> </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnDepartureDate" />
										</xsl:call-template>
										<xsl:text> </xsl:text>
										<xsl:value-of select="ReturnDepartureTime" />
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">Arr:</trans>
										<xsl:text> </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnArrivalDate" />
										</xsl:call-template>
										<xsl:text> </xsl:text>
										<xsl:value-of select="ReturnArrivalTime" />
									</p>
								</dd>
								<xsl:if test="count(FlightPassengers/FlightPassenger/hlpReturnSeatCode) &gt; 0">
									<dt ml="Booking Confirmation">Seats</dt>
									<dd>
										<xsl:for-each select="FlightPassengers/FlightPassenger">
											<xsl:if test="hlpReturnSeatCode and hlpReturnSeatCode != ''">
												<p>
													<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ': ')" />
													<trans ml="Booking Confirmation">Seat</trans>
													<xsl:text> </xsl:text>
													<xsl:value-of select="hlpReturnSeatCode" />
												</p>
											</xsl:if>
										</xsl:for-each>
									</dd>
								</xsl:if>
								<dt ml="Booking Confirmation">Passengers</dt>
								<dd>
									<xsl:value-of select="/GetBookingDetailsResponse/Adults" />
									<xsl:text> </xsl:text>
									<xsl:choose>
										<xsl:when test="/GetBookingDetailsResponse/Adults &gt; 1">
											<trans ml="Booking Confirmation">adults</trans>
										</xsl:when>
										<xsl:otherwise>
											<trans ml="Booking Confirmation">adult</trans>
										</xsl:otherwise>
									</xsl:choose>
									<xsl:if test="/GetBookingDetailsResponse/Children &gt; 0">
										<xsl:if test="/GetBookingDetailsResponse/Infants = 0">
											<xsl:text> </xsl:text>
											<trans ml="Booking Confirmation">and</trans>
										</xsl:if>
										<xsl:if test="/GetBookingDetailsResponse/Infants &gt; 0">
											<xsl:text>,</xsl:text>
										</xsl:if>
										<xsl:value-of select="concat (' ', /GetBookingDetailsResponse/Children, ' ')" />
										<xsl:choose>
											<xsl:when test="/GetBookingDetailsResponse/Children &gt; 1">
												<trans ml="Booking Confirmation">children</trans>
											</xsl:when>
											<xsl:otherwise>
												<trans ml="Booking Confirmation">child</trans>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:if>
									<xsl:if test="/GetBookingDetailsResponse/Infants &gt; 0">
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">and</trans>
										<xsl:value-of select="concat(' ', /GetBookingDetailsResponse/Infants, ' ')" />
										<trans ml="Booking Confirmation">
											<xsl:text>infant</xsl:text>
											<xsl:if test="/GetBookingDetailsResponse/Infants &gt; 1">
												<xsl:text>s</xsl:text>
											</xsl:if>
										</trans>
									</xsl:if>
								</dd>
							</dl>

							<div class="clear">
								<xsl:text> </xsl:text>
							</div>
						</div>
					</xsl:for-each>

					<xsl:for-each select="Transfers/Transfer">
						<div class="box primary">

							<div class="boxTitle">
								<h2 ml="Booking Confirmation">Transfer</h2>
							</div>

							<dl>
								<dt ml="Booking Confirmation">Outbound Journey</dt>
								<dd>
									<xsl:if test="DepartureParentName != ''">
										<xsl:value-of select="DepartureParentName" />
									</xsl:if>
									<xsl:if test="DepartureParentName = ''">
										<xsl:value-of select="OutboundDetails/JourneyOrigin" />
									</xsl:if>
									<xsl:text> - </xsl:text>
									<xsl:value-of select="ArrivalParentName" />
								</dd>
								<dt ml="Booking Confirmation">Flight Code</dt>
								<dd>
									<xsl:value-of select="DepartureFlightCode" />
								</dd>
								<dt ml="Booking Confirmation">Flight Time</dt>
								<dd>
									<xsl:value-of select="DepartureTime" />
									<xsl:text> </xsl:text>
									<trans ml="Booking Confirmation">on</trans>
									<xsl:text> </xsl:text>
									<xsl:call-template name="ShortDate">
										<xsl:with-param name="SQLDate" select="DepartureDate" />
									</xsl:call-template>
								</dd>
							</dl>

							<xsl:if test="OneWay = 'false'">

								<dl class="return">
									<dt ml="Booking Confirmation">Return Journey</dt>
									<dd>
										<xsl:value-of select="ArrivalParentName" />
										<xsl:text> - </xsl:text>
										<xsl:if test="DepartureParentName != ''">
											<xsl:value-of select="DepartureParentName" />
										</xsl:if>
										<xsl:if test="DepartureParentName = ''">
											<xsl:value-of select="OutboundDetails/JourneyOrigin" />
										</xsl:if>
									</dd>
									<dt ml="Booking Confirmation">Flight Code</dt>
									<dd>
										<xsl:value-of select="ReturnFlightCode" />
									</dd>
									<dt ml="Booking Confirmation">Flight Time</dt>
									<dd>
										<xsl:value-of select="ReturnTime" />
										<xsl:text> </xsl:text>
										<trans ml="Booking Confirmation">on</trans>
										<xsl:text> </xsl:text>
										<xsl:call-template name="ShortDate">
											<xsl:with-param name="SQLDate" select="ReturnDate" />
										</xsl:call-template>
									</dd>
								</dl>
							</xsl:if>

							<div class="clear">
								<xsl:text> </xsl:text>
							</div>
						</div>
					</xsl:for-each>

					<!--<xsl:for-each select="CarHireBookings/CarHireBooking">
				<div class="box primary">

					<div class="boxTitle">
						<h2 ml="Booking Confirmation">Car H</h2>
					</div>

					<dl>
						<dt ml="Booking Confirmation">Outbound Journey</dt>
						<dd>
							<xsl:if test="DepartureParentName != ''">
								<xsl:value-of select="DepartureParentName" />
							</xsl:if>
							<xsl:if test="DepartureParentName = ''">
								<xsl:value-of select="OutboundDetails/JourneyOrigin" />
							</xsl:if>
							<xsl:text> - </xsl:text>
							<xsl:value-of select="ArrivalParentName" />
						</dd>
						<dt ml="Booking Confirmation">Flight Code</dt>
						<dd>
							<xsl:value-of select="DepartureFlightCode" />
						</dd>
						<dt ml="Booking Confirmation">Flight Time</dt>
						<dd>
							<xsl:value-of select="DepartureTime" />
							<xsl:text> </xsl:text>
							<trans ml="Booking Confirmation">on</trans>
							<xsl:text> </xsl:text>
							<xsl:call-template name="ShortDate">
								<xsl:with-param name="SQLDate" select="DepartureDate" />
							</xsl:call-template>
						</dd>
					</dl>

					<xsl:if test="OnewWay = 'false'">

						<dl class="return">
							<dt ml="Booking Confirmation">Return Journey</dt>
							<dd>
								<xsl:value-of select="ArrivalParentName" />
								<xsl:text> - </xsl:text>
								<xsl:if test="DepartureParentName != ''">
									<xsl:value-of select="DepartureParentName" />
								</xsl:if>
								<xsl:if test="DepartureParentName = ''">
									<xsl:value-of select="OutboundDetails/JourneyOrigin" />
								</xsl:if>
							</dd>
							<dt ml="Booking Confirmation">Flight Code</dt>
							<dd>
								<xsl:value-of select="ReturnFlightCode" />
							</dd>
							<dt ml="Booking Confirmation">Flight Time</dt>
							<dd>
								<xsl:value-of select="ReturnTime" />
								<xsl:text> </xsl:text>
								<trans ml="Booking Confirmation">on</trans>
								<xsl:text> </xsl:text>
								<xsl:call-template name="ShortDate">
									<xsl:with-param name="SQLDate" select="ReturnDate" />
								</xsl:call-template>
							</dd>
						</dl>
					</xsl:if>

					<div class="clear">
						<xsl:text> </xsl:text>
					</div>
				</div>
			</xsl:for-each>-->
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>

	<!-- Date Functions -->
	<xsl:template name="ShortDate">
		<xsl:param name="SQLDate" />
		<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)" />

		<xsl:value-of select="substring($SQLDate,9,2)" />
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
		<xsl:value-of select="substring($SQLDate,3,2)" />
	</xsl:template>

	<!-- get selling price -->
	<xsl:template name="GetSellingPrice">
		<xsl:param name="Value" />
		<xsl:param name="Exchange" select="'1'" />
		<xsl:param name="Currency" />
		<xsl:param name="Format" select="'###,##0.00'" />
		<xsl:param name="CurrencySymbolPosition" select="'Prepend'" />
		<xsl:param name="RoundingRule" select="'Unrounded'" />

		<!-- do the exchange -->
		<xsl:variable name="ConvertedValue" select="$Value * $Exchange" />

		<xsl:variable name="RoundingInteger" select="floor($ConvertedValue) mod 10" />

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
							<xsl:value-of select="floor($ConvertedValue) - 1" />
						</xsl:when>
						<xsl:when test="$RoundingInteger = 1">
							<xsl:value-of select="floor($ConvertedValue) - 2" />
						</xsl:when>
						<xsl:when test="$RoundingInteger = 2">
							<xsl:value-of select="floor($ConvertedValue) + 3" />
						</xsl:when>
						<xsl:when test="$RoundingInteger = 3 or $RoundingInteger = 7">
							<xsl:value-of select="floor($ConvertedValue) + 2" />
						</xsl:when>
						<xsl:when test="$RoundingInteger = 4 or $RoundingInteger = 8">
							<xsl:value-of select="floor($ConvertedValue) + 1" />
						</xsl:when>
						<xsl:when test="$RoundingInteger = 5 or $RoundingInteger = 9">
							<xsl:value-of select="floor($ConvertedValue)" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$ConvertedValue" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$ConvertedValue" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="Sign">
			<xsl:if test="$RoundedValue &lt; 0">-</xsl:if>
		</xsl:variable>

		<xsl:variable name="NewValue">
			<xsl:choose>
				<xsl:when test="$RoundedValue &lt; 0">
					<xsl:value-of select="$RoundedValue * -1" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$RoundedValue" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:value-of select="$Sign" />
		<xsl:if test="$CurrencySymbolPosition='Prepend'">
			<xsl:value-of select="$Currency" />
		</xsl:if>
		<xsl:value-of select="format-number($NewValue, $Format)" />
		<xsl:if test="$CurrencySymbolPosition='Append'">
			<xsl:value-of select="$Currency" />
		</xsl:if>
	</xsl:template>

	<!-- Star Rating -->
	<xsl:template name="StarRating">
		<xsl:param name="Rating" />
		<xsl:param name="Small" select="'false'" />

		<xsl:variable name="class">
			<xsl:text>rating star</xsl:text>
			<xsl:value-of select="substring($Rating,1,1)" />

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
</xsl:stylesheet>
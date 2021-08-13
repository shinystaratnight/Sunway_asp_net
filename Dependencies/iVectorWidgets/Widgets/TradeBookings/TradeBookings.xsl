<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:param name="SellingCurrencyExchange" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CreditCardAgent" />
	<xsl:param name="Update" />

	<xsl:template match="/">

		<xsl:choose>
			<xsl:when test="$Update = 'True'">
				<xsl:call-template name="BookingsTable" />
			</xsl:when>
			<xsl:otherwise>

				<div id="divTradeBookings" class="box primary clear">

					<div class="boxTitle">
						<h2>Your Bookings</h2>
					</div>

					<div id="divBookingsTable">
						<xsl:call-template name="BookingsTable" />
					</div>

				</div>
				
			</xsl:otherwise>
		</xsl:choose>
		
		<div id="divDocumentationWaitMessage" style="display:none;">
			<p ml="Trade Bookings">Please wait whilst we generate the documents</p>
		</div>

	</xsl:template>

	<xsl:template name="BookingsTable">

		<!--Trade Bookings-->
		<table id="tblBookingsTable" class="standardTable">
			<thead>
				<tr>
					<th class="sort">
						<!--extra div for relative positioning of sort options in firefox - necessary!-->
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/Reference != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/Reference"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Reference</xsl:text>
									</xsl:otherwise>
								</xsl:choose>							
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('BookingReference', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('BookingReference', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th class="sort">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/LeadGuest != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/LeadGuest"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Lead Guest</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('LeadCustomerName', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('LeadCustomerName', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th class="sort date">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/BookingDate != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/BookingDate"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Booking Date</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('BookingDate', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('BookingDate', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th class="sort date">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/ArrivalDate != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/ArrivalDate"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Arrival Date</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('ArrivalDate', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('ArrivalDate', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th class="sort">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/Resort != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/Resort"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Resort</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('Resort', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('Resort', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th ml="Trade Bookings">
						<xsl:choose>
							<xsl:when test="/Booking/HeaderOverrides/PropVehicleName != ''">
								<xsl:value-of select="/Booking/HeaderOverrides/PropVehicleName"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Property/Vehicle Name</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th ml="Trade Bookings">
						<xsl:choose>
							<xsl:when test="/Booking/HeaderOverrides/Price != ''">
								<xsl:value-of select="/Booking/HeaderOverrides/Price"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Price</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th ml="Trade Bookings">
						<xsl:choose>
							<xsl:when test="/Booking/HeaderOverrides/Commission != ''">
								<xsl:value-of select="/Booking/HeaderOverrides/Commission"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Commission</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</th>
					<th class="sort">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/BookingStatus != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/BookingStatus"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Booking Status</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('Status', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('Status', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th class="sort">
						<div>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/PaymentStatus != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/PaymentStatus"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Payment Status</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
							<div class="sort">
								<a onclick="TradeBookings.Sort('PaymentStatus', 'Ascending', this)" class="sort ascending" >a</a>
								<a onclick="TradeBookings.Sort('PaymentStatus', 'Descending', this)" class="sort descending" >a</a>
							</div>
						</div>
					</th>
					<th>
						<trans ml="Trade Bookings">
							<xsl:choose>
								<xsl:when test="/Booking/HeaderOverrides/Docs != ''">
									<xsl:value-of select="/Booking/HeaderOverrides/Docs"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Docs</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</trans>
					</th>
					<th class="cancelBooking">
						<trans ml="Trade Bookings">
							<xsl:choose>
								<xsl:when test="/Booking/HeaderOverrides/CancelReq != ''">
									<xsl:value-of select="/Booking/HeaderOverrides/CancelReq"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Cancel Req.</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</trans>
					</th>
					<xsl:if test="$CreditCardAgent = 'true'">
						<th>
							<trans ml="Trade Bookings">
								<xsl:choose>
									<xsl:when test="/Booking/HeaderOverrides/Pay != ''">
										<xsl:value-of select="/Booking/HeaderOverrides/Pay"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>Pay</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
							</trans>
						</th>
					</xsl:if>
				</tr>
			</thead>

			<tbody>
				<xsl:for-each select="Booking/Booking">
					<xsl:call-template name="Booking">
						<xsl:with-param name="SellingCurrencyExchange" select="$SellingCurrencyExchange" />
						<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						<xsl:with-param name="CreditCardAgent" select="$CreditCardAgent" />
					</xsl:call-template>
				</xsl:for-each>
			</tbody>
		</table>
		
		<input type="hidden" id="hidTradeBookings_ErrorNoDocuments" ml="Trade Bookings" value="No documentation for this type could be found" />

	</xsl:template>

	<xsl:template name="Booking">

		<tr class="myBooking">
			<xsl:if test="position() mod 2 = 0">
				<xsl:attribute name="class">
					<xsl:text>myBooking even</xsl:text>
				</xsl:attribute>
			</xsl:if>

			<td>
				<xsl:value-of select="concat(BookingReference, ' / ', TradeReference)"/>
			</td>
			<td>
				<xsl:value-of select="concat(LeadCustomerLastName, ', ', LeadCustomerFirstName)"/>
			</td>
			<td ml="Trade Bookings" mlparams="{BookingDate}~ShortDate">{0}</td>
			<td ml="Trade Bookings" mlparams="{ArrivalDate}~ShortDate">
				<xsl:text>{0}</xsl:text>
				<input type="hidden" id="hidArrivalDate_{BookingReference}" value="{ArrivalDate}" />
			</td>
			<td class="resort">
				<xsl:call-template name="string-replace-all">
					<xsl:with-param name="text" select="hlpGeographyLevel3Name"/>
					<xsl:with-param name="replace" select="'/'"/>
					<xsl:with-param name="by" select="'/ '"/>
				</xsl:call-template>
				<xsl:text> </xsl:text>
			</td>

			<xsl:variable name="PropertyName" select="ComponentSummary/Component[ComponentType='PropertyBooking'][1]/hlpComponentName" />
			<xsl:variable name="TransferName" select="ComponentSummary/Component[ComponentType='TransferBooking'][1]/hlpComponentName" />

			<td>
				<xsl:choose>
					<xsl:when test="$PropertyName != ''">
						<xsl:value-of select="$PropertyName"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$TransferName"/>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text> </xsl:text>
			</td>
			<td>
				<xsl:call-template name="GetSellingPrice">
					<xsl:with-param name="Value" select="TotalPrice" />
					<xsl:with-param name="Format" select="'###,##0.00'" />
					<xsl:with-param name="Currency" select="$CurrencySymbol" />
					<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
				</xsl:call-template>
			</td>
			<td>
				<xsl:call-template name="GetSellingPrice">
					<xsl:with-param name="Value" select="TotalCommission" />
					<xsl:with-param name="Format" select="'###,##0.00'" />
					<xsl:with-param name="Currency" select="$CurrencySymbol" />
					<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
				</xsl:call-template>
			</td>
			<td ml="Trade Bookings">
				<xsl:value-of select="Status"/>
			</td>
			<td ml="Trade Bookings">
				<xsl:value-of select="AccountStatus"/>
			</td>
			<td class="documents">						
				<xsl:if test="Status != 'Cancelled' and count(ComponentSummary/Component[Status != 'Cancelled' and FailedComponent = 'true']) = 0">
					<a class="viewBookingDocuments agent" href="javascript:TradeBookings.ViewDocuments('{BookingReference}', 'Trade Documentation Only')">a</a>
					<a class="viewBookingDocuments customer" href="javascript:TradeBookings.ViewDocuments('{BookingReference}', 'Client Vouchers Only')">a</a>
					<a class="viewBookingDocuments sendDocs" href="javascript:TradeBookings.GetSendDocumentationPopup('{BookingReference}')" alt="Send Documentation">a</a>
				</xsl:if>
				<xsl:text> </xsl:text>
			</td>
			<td class="cancel">
				<xsl:if test="Status != 'Cancelled'">
					<a id="aCancelBooking_{BookingReference}" class="cancelBooking" href="javascript:TradeBookings.ComponentCancellation_GetPopup('{BookingReference}')">a</a>
				</xsl:if>
				<xsl:text> </xsl:text>
			</td>

			<xsl:if test="$CreditCardAgent = 'true'">
				<td>
					<xsl:if test="Status != 'Cancelled' and TotalOutstanding &gt; 0">
						<a class="cancelBooking" href="/mmb-payment?BookingReference={BookingReference}">
							<img src="https://placehold.it/10x10" class="actionImage" alt="Pay Now With CreditCard" title="Pay Now With CreditCard"  />
						</a>
					</xsl:if>
					<xsl:text> </xsl:text>
				</td>
			</xsl:if>

		</tr>

	</xsl:template>

	<xsl:template name="string-replace-all">
		<xsl:param name="text" />
		<xsl:param name="replace" />
		<xsl:param name="by" />
		<xsl:choose>
			<xsl:when test="contains($text, $replace)">
				<xsl:value-of select="substring-before($text,$replace)" />
				<xsl:value-of select="$by" />
				<xsl:call-template name="string-replace-all">
					<xsl:with-param name="text"
						select="substring-after($text,$replace)" />
					<xsl:with-param name="replace" select="$replace" />
					<xsl:with-param name="by" select="$by" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>

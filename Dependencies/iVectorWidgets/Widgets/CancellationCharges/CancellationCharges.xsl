<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="CurrencySymbol"  />
	<xsl:param name="CurrencySymbolPosition"  />
	<xsl:param name="DateToday" />
	<xsl:param name="SearchMode" />
	<xsl:param name="IntroductionOverride" />


	<xsl:template match="/">
		<xsl:if test="count(/BookingBasket/PreBookResponse/PropertyBookings/PropertyPreBookResponse/Cancellations/Cancellation) &gt; 0
					or count(/BookingBasket/PreBookResponse/FlightBookings/FlightPreBookResponse/Cancellations/Cancellation) &gt; 0
					or count(/BookingBasket/PreBookResponse/TransferBookings/TransferPreBookResponse/Cancellations/Cancellation) &gt; 0">

			<div id="divCancellationCharges" class="box primary clear">
				<!-- Cancellation Charges -->

				<div class="boxTitle">
					<h2 ml="CancellationCharges">Cancellation Charges</h2>
				</div>

				<xsl:choose>
					<xsl:when test ="$IntroductionOverride != ''">
						<p>
							<xsl:value-of select ="$IntroductionOverride"/>
						</p>
					</xsl:when>
					<xsl:otherwise>
						<p ml="CancellationCharges">The following costs will apply when making a cancellation of this booking.</p>
					</xsl:otherwise>

				</xsl:choose>

				<!-- Property -->
				<xsl:if test="count(/BookingBasket/PreBookResponse/PropertyBookings/PropertyPreBookResponse/Cancellations/Cancellation) &gt; 0">
					<div class="cancellationChargesComponent hotel">
						<h3 ml="CancellationCharges">Hotel</h3>

						<xsl:for-each select="/BookingBasket/PreBookResponse/PropertyBookings/PropertyPreBookResponse">
							<xsl:variable name="bookingToken" select="BookingToken" />
							<h4>
								<xsl:value-of select="/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption[BookingToken = $bookingToken][position() = 1]/PropertyName"/>
							</h4>
							<table id="tblCancellationCosts" cellpadding="0" cellspacing="0">
								<xsl:for-each select="Cancellations/Cancellation">
									<xsl:sort select="StartDate"/>
									<xsl:call-template name="Cancellation">
										<xsl:with-param name="DateToday" select="$DateToday" />
										<xsl:with-param name="DepartureDate" select="/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption[BookingToken = $bookingToken][position() = 1]/DepartureDate" />
										<xsl:with-param name="StartDate" select="StartDate" />
										<xsl:with-param name="EndDate" select="EndDate" />
										<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</xsl:for-each>
							</table>
						</xsl:for-each>
					</div>
				</xsl:if>

				<!-- Flight -->
				<xsl:if test="count(/BookingBasket/PreBookResponse/FlightBookings/FlightPreBookResponse/Cancellations/Cancellation) &gt; 0">
					<div class="cancellationChargesComponent flight">
						<h3 ml="CancellationCharges">Flight</h3>

						<xsl:for-each select="/BookingBasket/PreBookResponse/FlightBookings/FlightPreBookResponse">
							<xsl:variable name="bookingToken" select="BookingToken" />
							<xsl:variable name="flight" select="/BookingBasket/BasketFlights/BasketFlight/Flight[BookingToken = $bookingToken]" />

							<xsl:variable name="MultiCarrierFlight">
								<xsl:choose>
									<xsl:when test="MultiCarrierOutbound = 'true'">
										<xsl:value-of select="'True'"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="'False'"/>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>

							<table id="tblCancellationCosts" cellpadding="0" cellspacing="0">
								<xsl:for-each select="MultiCarrierCancellations/Cancellation[$MultiCarrierFlight = 'True'] | Cancellations/Cancellation[$MultiCarrierFlight = 'False']">
									<xsl:sort select="StartDate"/>
									<xsl:call-template name="Cancellation">
										<xsl:with-param name="DateToday" select="$DateToday" />
										<xsl:with-param name="DepartureDate" select="/BookingBasket/BasketFlights/BasketFlight/Flight/OutboundDepartureDate" />
										<xsl:with-param name="StartDate" select="StartDate" />
										<xsl:with-param name="EndDate" select="EndDate" />
										<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</xsl:for-each>
							</table>

						</xsl:for-each>
					</div>
				</xsl:if>

				<!-- Transfer -->
				<xsl:if test="count(/BookingBasket/PreBookResponse/TransferBookings/TransferPreBookResponse/Cancellations/Cancellation) &gt; 0">
					<div class="cancellationChargesComponent transfer">

						<xsl:if test="count(/BookingBasket/PreBookResponse/PropertyBookings/PropertyPreBookResponse/Cancellations/Cancellation) &gt; 0
								and count(/BookingBasket/PreBookResponse/FlightBookings/FlightPreBookResponse/Cancellations/Cancellation) &gt; 0">
							<xsl:attribute name="style">clear:both;</xsl:attribute>
						</xsl:if>

						<h3 ml="CancellationCharges">Transfer</h3>

						<xsl:for-each select="/BookingBasket/PreBookResponse/TransferBookings/TransferPreBookResponse">
							<xsl:variable name="bookingToken" select="BookingToken" />
							<xsl:variable name="transfer" select="/BookingBasket/BasketTransfers/BasketTransfer/Transfer[BookingToken = $bookingToken]" />

							<table id="tblCancellationCosts" cellpadding="0" cellspacing="0">
								<xsl:for-each select="Cancellations/Cancellation">
									<xsl:sort select="StartDate"/>
									<xsl:call-template name="Cancellation">
										<xsl:with-param name="DateToday" select="$DateToday" />
										<xsl:with-param name="DepartureDate" select="/BookingBasket/BasketTransfers/BasketTransfer/Transfer/DepartureDate" />
										<xsl:with-param name="StartDate" select="StartDate" />
										<xsl:with-param name="EndDate" select="EndDate" />
										<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</xsl:for-each>
							</table>
						</xsl:for-each>
					</div>
				</xsl:if>


				<!-- Extras -->
				<xsl:if test="count(/BookingBasket/PreBookResponse/ExtraBookings/ExtraPreBookResponse/Cancellations/Cancellation) &gt; 0">
					<div class="cancellationChargesComponent extra">

						<h3 ml="CancellationCharges">Extras</h3>

						<xsl:for-each select="/BookingBasket/PreBookResponse/ExtraBookings/ExtraPreBookResponse">

							<xsl:variable name="bookingToken" >
								<xsl:choose>
									<xsl:when test="count(ExtraOptions/ExtraOption) &gt; 0">
										<xsl:value-of select="ExtraOptions/ExtraOption[position() = 1]/BookingToken"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="BookingToken"/>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>
							<xsl:variable name="extra" select="/BookingBasket/BasketExtras/BasketExtra/BasketExtraOptions/BasketExtraOption[BookingToken = $bookingToken][position() = 1]" />

							<h4>
								<xsl:value-of select="$extra/ExtraName"/>
							</h4>

							<table id="tblCancellationCosts" cellpadding="0" cellspacing="0">
								<xsl:for-each select="Cancellations/Cancellation">
									<xsl:sort select="StartDate"/>
									<xsl:call-template name="Cancellation">
										<xsl:with-param name="DateToday" select="$DateToday" />
										<xsl:with-param name="DepartureDate" select="/BookingBasket/BasketTransfers/BasketTransfer/Transfer/DepartureDate" />
										<xsl:with-param name="StartDate" select="StartDate" />
										<xsl:with-param name="EndDate" select="EndDate" />
										<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</xsl:for-each>
							</table>
						</xsl:for-each>
					</div>
				</xsl:if>

				<!-- Car Hire -->
				<xsl:if test="count(/BookingBasket/PreBookResponse/CarHireBookings/CarHirePreBookResponse/Cancellations/Cancellation) &gt; 0">
					<div class="cancellationChargesComponent carHire">

						<h3 ml="CancellationCharges">Car Hire</h3>

						<xsl:for-each select="/BookingBasket/PreBookResponse/CarHireBookings/CarHirePreBookResponse">
							<xsl:variable name="bookingToken" select="BookingToken" />
							<xsl:variable name="carHire" select="/BookingBasket/BasketCarHires/BasketCarHire/CarHire[BookingToken = $bookingToken]" />

							<table id="tblCancellationCosts" cellpadding="0" cellspacing="0">
								<xsl:for-each select="Cancellations/Cancellation">
									<xsl:sort select="StartDate"/>
									<xsl:call-template name="Cancellation">
										<xsl:with-param name="DateToday" select="$DateToday" />
										<xsl:with-param name="DepartureDate" select="/BookingBasket/BasketCarHires/BasketCarHire/CarHire/PickUpDate" />
										<xsl:with-param name="StartDate" select="StartDate" />
										<xsl:with-param name="EndDate" select="EndDate" />
										<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</xsl:for-each>
							</table>
						</xsl:for-each>
					</div>
				</xsl:if>


				<div id="divCancellationAccept">
					<p ml="CancellationCharges">
						<strong ml="CancellationCharges">In the event of a no show or early checkout then a charge of up to 100% of the entire cost of the booking may be applied.</strong>
						<xsl:text> </xsl:text>
					</p>

					<div class="form">
						<label id="lblCancellationCharges" class="checkboxLabel">
							<input class="checkbox" type="checkbox" name="chkAcceptCancellation" id="chkAcceptCancellation" onclick="int.f.ToggleClass(this.parentNode, 'selected');"/>
							<trans ml="CancellationCharges">To continue with your booking you must accept these cancellation charges. I Agree.</trans>
						</label>
					</div>
				</div>

				<div class="clearing">
					<xsl:text> </xsl:text>
				</div>

			</div>

			<div class="contentBoxShadowSlim contentBoxShadowSmallThirdWidthRight">
				<xsl:text> </xsl:text>
			</div>

		</xsl:if>
	</xsl:template>


	<xsl:template name="Cancellation">
		<xsl:param name="DateToday" />
		<xsl:param name="DepartureDate" />
		<xsl:param name="StartDate" />
		<xsl:param name="EndDate" />
		<xsl:param name="CurrencySymbol" />
		<xsl:param name="CurrencySymbolPosition"  />

		<xsl:variable name="EndDateNumber" select="number(concat(substring($EndDate,1,4),substring($EndDate,6,2),substring($EndDate,9,2)))" />
		<xsl:variable name="DepartureDateNumber" select="number(concat(substring($DepartureDate,1,4),substring($DepartureDate,6,2),substring($DepartureDate,9,2)))" />


		<tr>

			<td class="date" ml="CancellationCharges" mlparams="{$EndDate}~ShortDate|{$StartDate}~ShortDate|{$DateToday}~ShortDate">

				<xsl:choose>
					<xsl:when test="(substring($StartDate,1,10) = substring($DateToday,1,10)) and ($EndDateNumber &lt; $DepartureDateNumber)">
						<xsl:text>{2} to {0}</xsl:text>
					</xsl:when>
					<xsl:when test="(substring($StartDate,1,10) = substring($DateToday,1,10)) and ($EndDateNumber &gt; $DepartureDateNumber)">
						<xsl:text>{1}</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>{1}</xsl:text>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:choose>
					<xsl:when test="($EndDateNumber &gt; $DepartureDateNumber)">
						<xsl:text> onwards</xsl:text>
					</xsl:when>
					<xsl:when test="substring($StartDate,1,10) != substring($DateToday,1,10)">
						<xsl:text> to {0}</xsl:text>
					</xsl:when>
				</xsl:choose>

			</td>

			<td>
				<xsl:choose>
					<xsl:when test="Amount = 0">
						<xsl:attribute name ="class">
							<xsl:text>noCharge</xsl:text>
						</xsl:attribute>
						<trans ml="CancellationCharges">No Charge</trans>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name ="class">
							<xsl:text>charge</xsl:text>
						</xsl:attribute>
						<trans ml="CancellationCharges">You will be charged</trans>
						<xsl:text> </xsl:text>
						<strong>
							<xsl:value-of select="concat($CurrencySymbol,format-number(Amount,'###,###0.00'))"/>
						</strong>
					</xsl:otherwise>
				</xsl:choose>
			</td>

		</tr>

	</xsl:template>

</xsl:stylesheet>
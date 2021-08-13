<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>
	<xsl:param name="SellingCurrencySymbol" />

	<xsl:template match="/">

		<input type="hidden" id="hidCancelComponent_SellingCurrencySymbol" value="{$SellingCurrencySymbol}" />
		<input type="hidden" id="hidCancelComponent_BookingReference" value="{CancellationDetails/BookingReference}" />

		<div id="divCancellationForm" class="box primary">

			<div class="boxTitle">
				<h3 ml="Trade Bookings">Request cancellation</h3>
			</div>			

			<p id="pCancellationRequestNoComponents" class="warning" style="display:none;" ml="Trade Bookings">Please select at least one component or all</p>

			<table class="def striped" id="tblCancelComponent">

				<thead>
					<tr>
						<th ml="Trade Bookings">Component</th>
						<th ml="Trade Bookings">Canx fee</th>
						<th>
							<xsl:text> </xsl:text>
						</th>
					</tr>
				</thead>

				<tbody>

					<!-- cancel whole booking -->
					<tr class="even">
						<td ml="Trade Bookings">All</td>
						<td class="numeric">
							<xsl:value-of select="concat($SellingCurrencySymbol, format-number(CancellationDetails/CancellationCost, '###,##0.00'))"/>
						</td>
						<td class="input">
							<input type="hidden" id="hidCancelComponent_All_{CancellationDetails/BookingCancellationToken}_0" value="{CancellationDetails/CancellationCost}" />
							<input type="checkbox" id="chkCancelComponent_All_{CancellationDetails/BookingCancellationToken}_0" onclick="TradeBookings.ComponentCancellation_SelectComponent(this);" />
						</td>
					</tr>

					<!-- Components -->
					<xsl:for-each select="CancellationDetails/CancellationComponentDetails/CancellationComponent">
						<tr>
							<td>
								<trans ml="Trade Bookings">
									<xsl:value-of select="Component"/>
								</trans>
								<xsl:value-of select="concat(' - ', Description)"/>
							</td>
							<td class="numeric">
								<xsl:value-of select="concat($SellingCurrencySymbol, format-number(CancellationCost, '###,##0.00'))"/>
							</td>
							<td class="input">
								<input type="hidden" id="hidCancelComponent_{Component}_{CancellationToken}_{ComponentBookingID}" value="{CancellationCost}" />
								<input type="checkbox" id="chkCancelComponent_{Component}_{CancellationToken}_{ComponentBookingID}" onclick="TradeBookings.ComponentCancellation_SelectComponent(this);" />
							</td>
						</tr>
					</xsl:for-each>

					<!-- Total -->
					<tr class="even">
						<td ml="Trade Bookings">Total Canx Fee</td>
						<td class="numeric">
							<span id="spCancelComponent_Total"><xsl:text> </xsl:text></span>
							<!--<input type="text" id="txtCancelComponent_Total" readonly="readonly" class="textbox" />-->
						</td>
						<td>
							<a id="aCancellationRequestButton" class="button small" href="javascript:TradeBookings.ComponentCancellation_CancelComponent()" ml="Trade Bookings">Cancel</a>
						</td>
					</tr>


				</tbody>

			</table>

			<input type="hidden" id="hidCancelComponent_Total" value="0"/>


			<div class="clearing">
				<xsl:text> </xsl:text>
			</div>

			<p id="pCancellationRequestThankyou" style="display:none;" ml="Trade Bookings">Your cancellation request has been sent.</p>
			<p id="pCancellationRequestError" style="display:none;" ml="Trade Bookings">Sorry there was a problem with your cancellation request.</p>
			
		</div>
		
		
	</xsl:template>


</xsl:stylesheet>

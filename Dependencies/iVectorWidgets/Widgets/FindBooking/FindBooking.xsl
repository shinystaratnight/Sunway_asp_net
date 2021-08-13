<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Reference" />
	<xsl:param name="GuestName" />
	<xsl:param name="Booked" />
	<xsl:param name="Arrival" />
	<xsl:param name="BookedStartDate" />
	<xsl:param name="BookedEndDate" />
	<xsl:param name="ArrivalStartDate" />
	<xsl:param name="ArrivalEndDate" />
	<xsl:param name="CustomDateChange" />
	<xsl:param name="ImageFolder" />
	<xsl:param name="DatepickerMonths" />
	<xsl:param name="DatepickerFirstDay" />

	<xsl:template match="/">

		<!--Find Booking-->
		<div id="divFindBooking" class="box primary clear">

			<input type="hidden" id="hidImageFolder" value="{$ImageFolder}" />

			<input type="hidden" id="hidFindBooking_ValidRef" value="Please provide a valid number in the booking reference field" ml="FindBooking" />

			<input type="hidden" id="hidCustomDateChange" value="{$CustomDateChange}" />

			<div class="boxTitle">
				<h2 ml="FindBooking">Find a Booking</h2>
			</div>

			<div id="divReference" class="formItem">
				<label id="lblReference" ml="FindBooking">
					<xsl:choose>
						<xsl:when test="/Overrides/LabelOverrides/lblReference != ''">
							<xsl:value-of select="/Overrides/LabelOverrides/lblReference"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Reference</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</label>
				<input type="text" class="textbox" id="txtReference" value="{$Reference}"/>
			</div>

			<div id="divGuestName" class="formItem">
				<label id="lblGuestName" ml="FindBooking">
					<xsl:choose>
						<xsl:when test="/Overrides/LabelOverrides/lblGuestName != ''">
							<xsl:value-of select="/Overrides/LabelOverrides/lblGuestName"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Guest name</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</label>
				<input type="text" class="textbox" id="txtGuestName" value="{$GuestName}"/>
			</div>

			<div id="divBooked" class="formItem">
				<label id="lblBooked" ml="FindBooking">
					<xsl:choose>
						<xsl:when test="/Overrides/LabelOverrides/lblBooked != ''">
							<xsl:value-of select="/Overrides/LabelOverrides/lblBooked"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Booked</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</label>
				<div>
					<select id="sddBookedDateRange" onchange="FindBooking.ChangeDates('Booked');" ml="FindBooking;DateRange">
						<option value="Any">
							<xsl:if test="$Booked = 'Any'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Any</xsl:text>
						</option>
						<option value="Today">
							<xsl:if test="$Booked = 'Today'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Today</xsl:text>
						</option>
						<option value="Yesterday">
							<xsl:if test="$Booked = 'Yesterday'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Yesterday</xsl:text>
						</option>
						<option value="Last Week">
							<xsl:if test="$Booked = 'Last Week'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Last Week</xsl:text>
						</option>
						<option value="Last Fortnight">
							<xsl:if test="$Booked = 'Last Fortnight'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Last Fortnight</xsl:text>
						</option>
						<option value="Range">
							<xsl:if test="$Booked = 'Range'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Range</xsl:text>
						</option>
					</select>
				</div>

				<div id="divBookedStartDate" class="date">
					<label id="lblBookedStart" ml="FindBooking">
						<xsl:choose>
							<xsl:when test="/Overrides/LabelOverrides/lblBookedStart != ''">
								<xsl:value-of select="/Overrides/LabelOverrides/lblBookedStart"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Start</xsl:text>
							</xsl:otherwise>
						</xsl:choose>						
					</label>
					<div class="textbox calendar">
						<input class="textbox" type="text" id="txtBookedStartDate"/>
					</div>
				</div>
				<div id="divBookedEndDate" class="date">
					<label id="lblBookedEnd" ml="FindBooking">
						<xsl:choose>
							<xsl:when test="/Overrides/LabelOverrides/lblBookedEnd != ''">
								<xsl:value-of select="/Overrides/LabelOverrides/lblBookedEnd"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>End</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</label>
					<div class="textbox calendar">
						<input class="textbox" type="text" id="txtBookedEndDate"/>
					</div>
				</div>
			</div>


			<div id="divArrival" class="formItem">
				<div class="range">
					<label id="lblTravelling" class="travelling" ml="FindBooking">
						<xsl:choose>
							<xsl:when test="/Overrides/LabelOverrides/lblTravelling != ''">
								<xsl:value-of select="/Overrides/LabelOverrides/lblTravelling"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Travelling</xsl:text>
							</xsl:otherwise>
						</xsl:choose>						
					</label>
					<select id="sddArrivalDateRange" onchange="FindBooking.ChangeDates('Arrival');" ml="FindBooking;DateRange">
						<option value="Any">
							<xsl:if test="$Booked = 'Any'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Any</xsl:text>
						</option>
						<option value="Range">
							<xsl:if test="$Arrival = 'Range'">
								<xsl:attribute name="selected">True</xsl:attribute>
							</xsl:if>
							<xsl:text>Range</xsl:text>
						</option>
					</select>
				</div>
				<div id="divArrivalStartDate" class="date">
					<label id="lblArrivalStart" ml="FindBooking">
						<xsl:choose>
							<xsl:when test="/Overrides/LabelOverrides/lblArrivalStart != ''">
								<xsl:value-of select="/Overrides/LabelOverrides/lblArrivalStart"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Start</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</label>
					<div class="textbox calendar">
						<input class="textbox" type="text" id="txtArrivalStartDate"/>
					</div>
				</div>
				<div id="divArrivalEndDate" class="date">
					<label id="lblArrivalEnd" ml="FindBooking">
						<xsl:choose>
							<xsl:when test="/Overrides/LabelOverrides/lblArrivalEnd != ''">
								<xsl:value-of select="/Overrides/LabelOverrides/lblArrivalEnd"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>End</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</label>
					<div class="textbox calendar">
						<input class="textbox" type="text" id="txtArrivalEndDate"/>
					</div>
				</div>

			</div>

			<div class="clearing">
				<xsl:text> </xsl:text>
			</div>
			<a id="aSearch" class="button icon search" onclick="FindBooking.Validate();">
				<i>
					<xsl:text> </xsl:text>
				</i>
				<xsl:text> Search</xsl:text>
			</a>

		</div>

		<input type="hidden" id="hidFindBookings_DatepickerMonths" runat="server" value="{$DatepickerMonths}" />
		<input type="hidden" id="hidFindBookings_DatepickerFirstDay" runat="server" value="{$DatepickerFirstDay}" />
		
		<xsl:for-each select="/Overrides/PlaceholderOverrides">
			<xsl:if test="txtReference != ''">
				<input type="hidden" id="hidFindBooking_ReferencePlaceholder" runat="server" value="{txtReference}" />
			</xsl:if>
			<xsl:if test="txtGuestName != ''">
				<input type="hidden" id="hidFindBooking_GuestNamePlaceholder" runat="server" value="{txtGuestName}" />
			</xsl:if>
		</xsl:for-each>
		

		<!--Setup javascript-->
		<script type="text/javascript">
			<xsl:text>FindBooking.Setup();</xsl:text>
		</script>

	</xsl:template>

</xsl:stylesheet>

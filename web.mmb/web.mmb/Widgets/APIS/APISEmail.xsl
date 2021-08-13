<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="BookingReference"/>
	<xsl:param name="FlightBookingReference"/>
	<xsl:param name="SupplierReference"/>
	
	<xsl:template match="/">

		<table style="text-align:left;margin-bottom:10px;">

			<tr>
				<th ml="My Booking">Booking Referece</th>
				<th>
					<xsl:value-of select="$BookingReference"/>
				</th>
			</tr>
			<tr>
				<th ml="My Booking">Flight Booking Referece</th>
				<th>
					<xsl:value-of select="$FlightBookingReference"/>
				</th>
			</tr>
			<tr>
				<th ml="My Booking">Carrier Booking Reference</th>
				<th>
					<xsl:value-of select="$SupplierReference"/>
				</th>
			</tr>

		</table>

		<table style="text-align:left;">			

			<xsl:for-each select="ApisPassenger/ApisPassenger">
				<tr>
					<th ml="My Booking">Flight Booking Passenger ID</th>
					<th>
						<xsl:value-of select="FlightBookingPassengerID"/>
					</th>
				</tr>

				<tr>
					<th ml="My Booking">Title</th>
					<td>
						<xsl:value-of select="Title"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">First Name</th>
					<td>
						<xsl:value-of select="FirstName"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Middle Name</th>
					<td>
						<xsl:value-of select="MiddleName"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Last Name</th>
					<td>
						<xsl:value-of select="LastName"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Date Of Birth</th>
					<td>
						<xsl:value-of select="DateOfBirth"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Gender</th>
					<td>
						<xsl:value-of select="Gender"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Nationality</th>
					<td>
						<xsl:value-of select="Nationality"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Nationality Code</th>
					<td>
						<xsl:value-of select="NationalityCode"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Passport Number</th>
					<td>
						<xsl:value-of select="PassportNumber"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Passport Issue Date</th>
					<td>
						<xsl:value-of select="PassportIssueDate"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Passport Expiry Date</th>
					<td>
						<xsl:value-of select="PassportExpiryDate"/>
					</td>
				</tr>
				<tr>
					<th ml="My Booking">Passport Place of Issue</th>
					<td>
						<xsl:value-of select="PassportIssuePlaceName"/>
					</td>
				</tr>
				<tr>
					<th>
						<xsl:text> </xsl:text>
					</th>
					<td>
						<xsl:text> </xsl:text>
					</td>
				</tr>
			</xsl:for-each>
			
		</table>

	</xsl:template>

</xsl:stylesheet>

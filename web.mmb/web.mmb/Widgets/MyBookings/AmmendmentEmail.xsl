<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="PropertyBookingReference" />
	<xsl:param name="CheckInDate" />
	<xsl:param name="CheckOutDate" />
	<xsl:param name="Destination" />
	<xsl:param name="HotelName" />
	<xsl:param name="AdditionalInformation" />

	<xsl:template match="/">

		-------------------------------------------------------------------------
		<h3>Amendment Request</h3>
		-------------------------------------------------------------------------

		<table style="text-align:left;">
			<tr>
				<th>Booking Reference:</th>
				<td>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/BookingReference"/>
				</td>
			</tr>
			<tr>
				<th>Property Booking Reference:</th>
				<td>
					<xsl:value-of select="$PropertyBookingReference"/>
				</td>
			</tr>
			<tr>
				<th>Source Reference:</th>
				<td>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/Properties/Property[PropertyBookingReference = $PropertyBookingReference]/SourceReference"/>
				</td>
			</tr>
		</table>

		-------------------------------------------------------------------------
		<h3>Form Details</h3>
		-------------------------------------------------------------------------
		<table style="text-align:left;">
			<tr>
				<th>Hotel Name:</th>
				<td>
					<xsl:value-of select="$HotelName"/>
				</td>
			</tr>
			<tr>
				<th>Destination:</th>
				<td>
					<xsl:value-of select="$Destination"/>
				</td>
			</tr>
			<tr>
				<th>Check In Date:</th>
				<td>
					<xsl:value-of select="substring($CheckInDate, 1, 10)"/>
				</td>
			</tr>
			<tr>
				<th>Check Out Date:</th>
				<td>
					<xsl:value-of select="substring($CheckOutDate, 1, 10)"/>
				</td>
			</tr>

			<tr>
				<th>Additional Information:</th>
				<td>
					<xsl:value-of select="$AdditionalInformation"/>
				</td>
			</tr>
		</table>


		-------------------------------------------------------------------------
		<h3>Lead Customer Details</h3>
		-------------------------------------------------------------------------
		<table style="text-align:left;">			
			<tr>
				<th>Name:</th>
				<td>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerTitle"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerFirstName"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerLastName"/>
				</td>
			</tr>
			<tr>
				<th>Address:</th>
				<td>					
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerAddress1"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerAddress2"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerTownCity"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerCounty"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerPostcode"/>
				</td>
			</tr>
			<tr>
				<th>Email:</th>
				<td>
					<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerEmail"/>
				</td>
			</tr>
			<xsl:if test="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerPhone != ''">
				<tr>
					<th>Customer Phone:</th>
					<td>
						<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerPhone"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerMobile != ''">
				<tr>
					<th>Customer Mobile:</th>
					<td>
						<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerMobile"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerFax != ''">
				<tr>
					<th>Customer Fax:</th>
					<td>
						<xsl:value-of select="MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerFax"/>
					</td>
				</tr>
			</xsl:if>
		</table>

	</xsl:template>

</xsl:stylesheet>

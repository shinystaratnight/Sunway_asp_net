<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="BookingReference" />
	<xsl:param name="HeaderText" />
	<xsl:param name="BookingDocument" />
  <xsl:param name="HideDocuments"/>

	<xsl:template match="/">

		<div id="divConfirmationPrint" class="sidebarBox primary">

			<div class="boxTitle">
				<h2>
					<xsl:choose>
						<xsl:when test ="$HeaderText != ''">
							<trans ml="Booking Confirmation">
								<xsl:value-of select ="$HeaderText" />
							</trans>
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Booking Confirmation">Thank you for your booking</trans>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
			</div>

			<div ml="Booking Confirmation">
				Please check and confirm all details on screen. All documentation will be sent to you via e-mail.
			</div>

			<div>
				<a href="#" onclick="window.print();return false;" class="button primary icon print">
					<i>
						<xsl:text> </xsl:text>
					</i>
					<trans ml="Booking Confirmation">Print Page</trans>
				</a>
				 <xsl:if test="$HideDocuments != 'True'">  
					  <a href="#" onclick="BookingConfirmationPrint.ViewDocumentation('{$BookingReference}', '{$BookingDocument}');return false;" class="button primary icon file">
						  <i>
							  <xsl:text> </xsl:text>
						  </i>
						  <trans ml="Booking Confirmation">Documents</trans>
					  </a>
				</xsl:if>
			</div>
		</div>
	</xsl:template>
</xsl:stylesheet>
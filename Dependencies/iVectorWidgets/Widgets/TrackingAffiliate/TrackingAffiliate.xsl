<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:param name="Confirmation" select="'false'" />
	
	<xsl:template match="/">		
		
		<xsl:for-each select="TrackingAffiliates/TrackingAffiliate">
			
			<xsl:choose>
				<xsl:when test="$Confirmation = 'true' and ConfirmationScript != ''">
					<xsl:value-of select="ConfirmationScript" disable-output-escaping="yes" />
				</xsl:when>
				<xsl:when test="Script != ''">				
					<xsl:value-of select="Script" disable-output-escaping="yes"/>				
				</xsl:when>				
			</xsl:choose>
	
		</xsl:for-each>
			
	</xsl:template>

</xsl:stylesheet>

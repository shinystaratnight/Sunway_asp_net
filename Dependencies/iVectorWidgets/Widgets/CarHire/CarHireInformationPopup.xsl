<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl" />

	<xsl:template match="/">

		<xsl:variable name="IncludedServices">
			<xsl:choose>
				<xsl:when test="contains(InformationPreBookReturn/Response/RentalConditions, '## Included Services')">
					<xsl:text>## Included Services</xsl:text>
					<xsl:value-of select="substring-after(InformationPreBookReturn/Response/RentalConditions, '## Included Services')"/>
				</xsl:when>
				<xsl:when test="contains(InformationPreBookReturn/Response/RentalConditions, '## Inbegrepen diensten')">
					<xsl:text>## Inbegrepen diensten</xsl:text>
					<xsl:value-of select="substring-after(InformationPreBookReturn/Response/RentalConditions, '## Inbegrepen diensten')"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="InformationPreBookReturn/Response/RentalConditions"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<p>
			<xsl:call-template name="Markdown">
				<xsl:with-param name="text" select="$IncludedServices"/>
			</xsl:call-template>
		</p>

	</xsl:template>
</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:param name="CancellationChargesPosition" />
	<xsl:param name="PropertyReferenceID" />
	<xsl:param name="Index" />

	<xsl:param name="CurrencySymbol"  />
	<xsl:param name="CurrencySymbolPosition"  />
	<xsl:param name="DateToday" />
	<xsl:param name="DepartureDate" />
	<xsl:param name="UseRoomMapping" />

	<xsl:template match="/">
		<xsl:if test ="ArrayOfCancellation/ArrayOfErratum/Erratum and $UseRoomMapping">
			<div class="errata">
				<h5>Information</h5>
				<xsl:for-each select ="ArrayOfCancellation/ArrayOfErratum/Erratum">
					<p>
						<xsl:value-of select ="ErratumDescription"/>
					</p>
				</xsl:for-each>
			</div>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>

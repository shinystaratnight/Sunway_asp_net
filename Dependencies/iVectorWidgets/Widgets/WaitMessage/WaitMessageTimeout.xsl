<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Type" />
	<xsl:param name="Theme" />
	<xsl:param name="Website" />
	<xsl:param name="Hidden" select="'False'" />
    <xsl:param name="RemoveProtocall" select="'True'" />
	
	<xsl:template match="/">
		
		
		<xsl:if test="WaitMessages/WaitMessage[Type=$Type]/TimeoutText and WaitMessages/WaitMessage[Type=$Type]/TimeoutText != ''">
			<div id="divWaitMessageTimeout">
				<xsl:for-each select="WaitMessages/WaitMessage[Type=$Type]">
					<p>
						<xsl:value-of select="TimeoutText"/>
					</p>
					<a href="{TimeoutURL}" class="button primary">Continue</a>
				</xsl:for-each>
			</div>
		</xsl:if>
		
	</xsl:template>

</xsl:stylesheet>

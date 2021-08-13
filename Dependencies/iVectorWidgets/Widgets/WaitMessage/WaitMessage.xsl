<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Type" />
	<xsl:param name="Theme" />
	<xsl:param name="Website" />
	<xsl:param name="Hidden" select="'False'" />
    <xsl:param name="RemoveProtocall" select="'True'" />
	
	<xsl:template match="/">

		<div id="divWaitMessage">
			
			<xsl:if test="$Hidden = 'True'">
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>
			
			<xsl:for-each select="WaitMessages/WaitMessage[Type=$Type]">
				<xsl:if test="Image != ''">
					<img src="{Image}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" /><!-- allow for use on secure pages-->
				</xsl:if>				
				<p><xsl:value-of select="Text"/></p>
				<xsl:if test="SubText != ''">
					<p class="subText">
						<xsl:value-of select="SubText"/>
					</p>
				</xsl:if>
			</xsl:for-each>
			<xsl:text> </xsl:text>
			
		</div>

	</xsl:template>

</xsl:stylesheet>

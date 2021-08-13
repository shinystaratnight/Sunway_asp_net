<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Type" />
	<xsl:param name="Theme" />
	<xsl:param name="Website" />
	<xsl:param name="Hidden" select="'False'" />
	
	<xsl:template match="/">

		<div id="divWaitMessage">
			
			<xsl:if test="$Hidden = 'True'">
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>			
			
			<xsl:for-each select="WaitMessages/WaitMessage[Type=$Type]">
				<xsl:if test="Image != ''">
					<!-- if the image comes back as https then just render it out, if not then remove the protocol and use the relative path -->
					<xsl:choose>
						<xsl:when test="string-length(substring-after(Image, 'https')) &gt; 0">
							<img src="{Image}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" class="waitmessage-image" />
						</xsl:when>
						<xsl:otherwise>
							<!-- allow for use on secure pages-->
							<img src="{substring-after(Image, 'http:')}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" class="waitmessage-image" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<p><xsl:value-of select="Text"/></p>
				<xsl:if test="SubText != ''">
					<p class="subText">
						<xsl:value-of select="SubText"/>
					</p>
				</xsl:if>
				<img class="spinner" src="/themes/{$Theme}/images/loader.gif"></img>
			</xsl:for-each>
			<xsl:text> </xsl:text>
			
		</div>

	</xsl:template>

</xsl:stylesheet>

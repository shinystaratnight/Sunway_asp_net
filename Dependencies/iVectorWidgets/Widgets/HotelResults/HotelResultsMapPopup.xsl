<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
<xsl:include href="../../xsl/functions.xsl"/>
<xsl:include href="../../xsl/markdown.xsl"/>
	
<xsl:param name="CurrencySymbol" />
<xsl:param name="CurrencySymbolPosition" />
<xsl:param name="PackagePrice" />
<xsl:param name="PerPersonPrice" />
<xsl:param name="PriceFormat" />
<xsl:param name="HotelPriceOnly" />


	<xsl:template match="/">
	
	<xsl:for-each select="Hotel">
		
		<xsl:variable name="paxcount" select="sum(Rooms/Room/Adults) + sum(Rooms/Room/Children)" />
		
		<h2><xsl:value-of select="Name"/></h2>
				
		<xsl:call-template name="StarRating">
			<xsl:with-param name="Rating" select="Rating" />
			<xsl:with-param name="Small" select="'true'" />
		</xsl:call-template>
		
		<span class="fromPrice">					
			<xsl:choose>
				<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
					<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice div $paxcount, $PriceFormat), 'pp')"/>
				</xsl:when>
				<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice = 0">
					<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice div $paxcount, $PriceFormat), 'pp')"/>
				</xsl:when>
				<xsl:when test="$PerPersonPrice = 'False' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
					<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice, $PriceFormat))"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice, $PriceFormat))"/>
				</xsl:otherwise>
			</xsl:choose>
		</span>
		
		<div class="summary">
			<img class="mainImage" src="{MainImage/Image}" alt="" />
			
				<xsl:variable name="SummaryLength" select="'100'" />
				<xsl:variable name="summary">
					<xsl:choose>
						<xsl:when test="string-length(Summary) >= $SummaryLength">
							<xsl:value-of select="substring(Summary, 1, $SummaryLength)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="Summary" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
			
				<xsl:variable name="ellipsis">
					<xsl:if test="substring($summary, string-length($summary) - 1, string-length($summary)) != '.'">
						<xsl:text>...</xsl:text>
					</xsl:if>
				</xsl:variable>
			
				<xsl:call-template name="Markdown">
					<xsl:with-param name="text" select="concat($summary, $ellipsis)" />
				</xsl:call-template>
			
				<p id="pMoreInfo" ml="Hotel Results">Click on map pin for more info.</p>
		</div>
	
	</xsl:for-each>

</xsl:template>

</xsl:stylesheet>
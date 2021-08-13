<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">

    <xsl:if test="count(HotelListLinks/HotelListLink) &gt; 0">
    
		  <div class="sidebarBox linklist">

			  <div class="boxTitle">
				  <h2>
					  <trans ml="Hotel List Links">Hotels in</trans>
					  <xsl:text> </xsl:text>
					  <xsl:value-of select="HotelListLinks/Destination"/>
				  </h2>
			  </div>

			  <xsl:for-each select="HotelListLinks/HotelListLink">
				  <a href="{URL}">
					  <xsl:value-of select="Name"/>
				  </a>
			  </xsl:for-each>

		  </div>

    </xsl:if>

	</xsl:template>

</xsl:stylesheet>
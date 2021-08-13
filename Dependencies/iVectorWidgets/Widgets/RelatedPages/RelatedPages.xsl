<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<xsl:template match="/">

  <xsl:if test="count(RelatedPages/RelatedPage) &gt; 0">
    
    <div class="sidebarBox linklist">

      <div class="boxTitle">
        <h2>
          <xsl:choose>
            <xsl:when test="RelatedPages/RelatedPagesHeader != ''">
              <xsl:value-of select="RelatedPages/RelatedPagesHeader"/>
            </xsl:when>
            <xsl:otherwise>
              <trans ml="Related Pages">Related Pages</trans>
            </xsl:otherwise>
          </xsl:choose>
        </h2>
      </div>

      <xsl:for-each select="RelatedPages/RelatedPage">
        <a href="{URL}">
          <xsl:value-of select="Name"/>
        </a>
      </xsl:for-each>

    </div>
    
  </xsl:if>

</xsl:template>

</xsl:stylesheet>
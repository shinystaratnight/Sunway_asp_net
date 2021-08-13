<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <xsl:param name="RedirectURL" />

  <xsl:template match="/">
    <div>
      <input type="hidden" id="hidRedirectUL" value="{$RedirectURL}"/>
      <xsl:text> </xsl:text>
    </div>
  </xsl:template>
</xsl:stylesheet>
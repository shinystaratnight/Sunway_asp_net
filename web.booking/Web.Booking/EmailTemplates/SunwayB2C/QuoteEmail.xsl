<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:include href="../sunway/QuoteEmail.xsl" />
  <xsl:template match="/">
    <xsl:call-template name="QuoteEmail"/>
  </xsl:template>
</xsl:stylesheet>
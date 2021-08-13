<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:include href="../sunway/QuoteEmailPDF.xsl" />
  <xsl:template match="/">
    <xsl:call-template name="QuoteEmailPDF"/>
  </xsl:template>
</xsl:stylesheet>
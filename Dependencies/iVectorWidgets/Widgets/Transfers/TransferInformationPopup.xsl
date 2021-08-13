<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <xsl:include href="../../xsl/functions.xsl"/>
  <xsl:include href="../../xsl/markdown.xsl" />

    <xsl:template match="/">

      <xsl:if test="TransferInformationReturn/Response/VehicleDetails != ''">
        <h5 ml="Transfers">
          <xsl:text>Vehicle Details</xsl:text>
        </h5>
        <xsl:call-template name="Markdown">
          <xsl:with-param name="text" select="TransferInformationReturn/Response/VehicleDetails"/>
        </xsl:call-template>
      </xsl:if>

      <xsl:if test="TransferInformationReturn/Response/BaggageDescription != ''">
        <h5 ml="Transfers">
          <xsl:text>Baggage Description</xsl:text>
        </h5>
        <xsl:call-template name="Markdown">
          <xsl:with-param name="text" select="TransferInformationReturn/Response/BaggageDescription"/>
        </xsl:call-template>
      </xsl:if>

      <xsl:if test="TransferInformationReturn/Response/VehicleDetails = ''
          and TransferInformationReturn/Response/BaggageDescription = ''">
        <p ml="Transfer">Unable to display vehicle information</p>
      </xsl:if>

    </xsl:template>
</xsl:stylesheet>

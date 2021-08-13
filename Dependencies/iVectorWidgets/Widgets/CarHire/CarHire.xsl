<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>

	<xsl:param name="Title" />
	<xsl:param name="SubTitle" />
	<xsl:param name="Theme" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="SearchMode" />
	<xsl:param name="AutomaticSearchType" />
	<xsl:param name="SearchWarning" />
	<xsl:param name="UseBasketCarHire" />
	
    <xsl:template match="/">
		<input type="hidden" id="hidAutomaticSearchType" value="{$AutomaticSearchType}" />
		<input type="hidden" id="hidSearchWarning" value="{$SearchWarning}" />
		<input type="hidden" id="hidUseBasketCarHire" value="{$UseBasketCarHire}"/>
		
		<div id="divCarHire">
			<xsl:attribute name ="class">
				<xsl:choose>
					<xsl:when test="$CSSClassOverride != ''">
						<xsl:value-of select="$CSSClassOverride" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>box primary clear</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<div class="boxTitle">
				<h2 ml="Transfers">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Car Hire</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
				<xsl:if test="$SubTitle != ''">
					<p>
						<xsl:value-of select="$SubTitle"/>
					</p>
				</xsl:if>
			</div>
			
			
			<xsl:if test="$UseBasketCarHire = 'True'">
				<div id="divSelectedCar">
					<xsl:text> </xsl:text>
					<script type="text/javascript">CarHire.GetSelectedCarHireHTML();</script>
				</div>
			</xsl:if>
			
			<div id="divCarHireResults">
				
				<xsl:choose>
					<xsl:when test="$SearchMode = 'Manual'">
						<xsl:attribute name="style">
							<xsl:text>display:none;</xsl:text>
						</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<img id="imgLoading" alt="" src="/themes/{$Theme}/images/loading.gif"/>
						<p id="pSearchWait">Please wait whilst we search for your car hire...</p>

						<script type="text/javascript">CarHire.GetCarHireHTML();</script>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:text> </xsl:text>
			</div>
			
		</div>
		
    </xsl:template>
</xsl:stylesheet>

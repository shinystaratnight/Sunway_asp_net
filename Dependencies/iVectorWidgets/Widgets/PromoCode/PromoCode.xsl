<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href = "../../xsl/Functions.xsl" />
	
	<xsl:param name="InfoParagraph" />
	<xsl:param name="Title" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="InjectTarget" />
	<xsl:param name="CurrentDiscount" />

	<xsl:template match="/">

		<div id="divPromoCode" class="box">
			<xsl:if test="InjectTarget != ''">
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>

			<input type="hidden" id="hidPromoCode_CurrencySymbol" value="{$CurrencySymbol}"/>
			<input type="hidden" id="hidPromoCode_CurrencyPosition" value="{$CurrencySymbolPosition}"/>
			
			<!--<input type="hidden" id="hidAirportID" value="{Results/HotelAirports/Airports/Airport[1]/AirportID}" />-->

			<div class="boxTitle">
				<h2 ml="Promo Code">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Promotional Code</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
			</div>

			<xsl:choose>
				<xsl:when test="$InfoParagraph != ''">
					<p id="pPromoCodeInfo">
						<xsl:if test="$CurrentDiscount != 0">
							<xsl:attribute name="style">display:none;</xsl:attribute>
						</xsl:if>
						<xsl:value-of select="$InfoParagraph"/>
					</p>
				</xsl:when>
				<xsl:otherwise>
					<p id="pPromoCodeInfo">
						<xsl:if test="$CurrentDiscount != 0">
							<xsl:attribute name="style">display:none;</xsl:attribute>
						</xsl:if>
						<trans ml="Promo Code">If you have a promotion code, please enter it in the field below and click update to re-price your booking</trans>
					</p>
				</xsl:otherwise>
			</xsl:choose>

			<p id="pAppliedCode">
				<xsl:if test="$CurrentDiscount = 0">
					<xsl:attribute name="style">display:none;</xsl:attribute>
				</xsl:if>
				
				<span class="promo" ml="Promo Code">The promotional code has been successfully applied to your booking and has discounted </span>
				<span id="spnPromoCodeDiscount">
					<xsl:call-template name="GetSellingPrice">
						<xsl:with-param name="Value" select="$CurrentDiscount" />
						<xsl:with-param name="Format" select="'###,###0.00'" />
						<xsl:with-param name="Currency" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
					</xsl:call-template>
				</span>
			</p>

			<div id="divUpdate">
				<input type="text" id="txtPromoCode" name="txtPromoCode" class="textbox tiny"/>
				
				<a type="button" id="aUpdatePromoCode" href="#" class="button primary" onclick="PromoCode.ApplyPromoCode();return false;">
					<xsl:if test="$CurrentDiscount != 0">
						<xsl:attribute name="style">display:none;</xsl:attribute>
					</xsl:if>
					<trans ml="Promo Code">Update</trans>
				</a>
				
				<a type="button" id="aRemovePromoCode" class="button primary" value="Remove" onclick="PromoCode.RemovePromoCode();">
					<xsl:if test="$CurrentDiscount = 0">
						<xsl:attribute name="style">display:none;</xsl:attribute>
					</xsl:if>
					<trans ml="Promo Code">Remove</trans>
				</a>
			</div>			

			<input type="hidden" id="hidPromoCode_InjectTarget" value="{$InjectTarget}" />
			
			<script type="text/javascript">
				int.ll.OnLoad.Run(function () { PromoCode.Setup(); });
			</script>
			
		</div>
		
    </xsl:template>
</xsl:stylesheet>

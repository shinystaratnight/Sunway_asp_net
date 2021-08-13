<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="DocumentPath" />
<xsl:param name="IAgreeText" />
<xsl:param name="CSSClassOverride" />
	
<xsl:template match="/">

	<div id="divTermsAndConditions">

    <xsl:attribute name ="class">
      <xsl:choose>
        <xsl:when test="$CSSClassOverride != ''">
          <xsl:value-of select="$CSSClassOverride" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>box primary</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
    
		<div class="boxTitle">
			<h2 ml="Terms and Conditions">Terms and Conditions</h2>
		</div>

		
		<xsl:choose>
			<xsl:when test="SellingProfiles/SellingProfile[1]/TermsAndConditions != ''">
				<p ml="Terms and Conditions"><xsl:value-of select="SellingProfiles/SellingProfile[1]/TermsAndConditions"/></p>
				<p>
					<trans ml="Terms and Conditions">Tick this box to confirm all the information entered above is correct and that you have read and understand our booking</trans>
					<xsl:choose>
						<xsl:when test="SellingProfiles/SellingProfile[1]/TermsAndConditionsURL != ''">
							<a href="#" onclick="window.open('{SellingProfiles/SellingProfile[1]/TermsAndConditionsURL}', 'Terms and Conditions', 'scrollbars=yes,resizable=yes,toolbar=yes,location=0,menubar=no,copyhistory=no,status=no,directories=no');return false;" ml="Terms and Conditions">Terms and Conditions</a>
						</xsl:when>
						<xsl:otherwise>
							<a href="#" onclick="window.open('{$DocumentPath}', 'Terms and Conditions', 'scrollbars=yes,resizable=yes,toolbar=yes,location=0,menubar=no,copyhistory=no,status=no,directories=no');return false;" ml="Terms and Conditions">Terms and Conditions</a>
						</xsl:otherwise>
					</xsl:choose>	
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p>
					<trans ml="Terms and Conditions">Please read our</trans>
					<xsl:choose>
						<xsl:when test="SellingProfiles/SellingProfile[1]/TermsAndConditionsURL != ''">
							<a href="#" onclick="window.open('{SellingProfiles/SellingProfile[1]/TermsAndConditionsURL}', 'Terms and Conditions', 'scrollbars=yes,resizable=yes,toolbar=yes,location=0,menubar=no,copyhistory=no,status=no,directories=no');return false;" ml="Terms and Conditions">Terms and Conditions</a>
						</xsl:when>
						<xsl:otherwise>
							<a href="#" onclick="window.open('{$DocumentPath}', 'Terms and Conditions', 'scrollbars=yes,resizable=yes,toolbar=yes,location=0,menubar=no,copyhistory=no,status=no,directories=no');return false;" ml="Terms and Conditions">Terms and Conditions</a>
						</xsl:otherwise>
					</xsl:choose>	
				</p>
			</xsl:otherwise>
		</xsl:choose>		
		
		<div class="form">
			<label id="lblTermsAndConditions" class="checkboxLabel">
				<input type="checkbox" id="cbTermsAndConditions" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');"></input>
				<xsl:choose>
					<xsl:when test="$IAgreeText != ''">
						<trans ml="Terms and Conditions"><xsl:value-of select="$IAgreeText"/></trans>
					</xsl:when>
					<xsl:otherwise>
						<trans ml="Terms and Conditions">
							<xsl:text>I agree to the Terms and Conditions</xsl:text>							
						</trans>
					</xsl:otherwise>
				</xsl:choose>				
			</label>
		</div>
		
	</div>

</xsl:template>

</xsl:stylesheet>
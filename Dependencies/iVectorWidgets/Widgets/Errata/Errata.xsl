<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>
	
	<xsl:param name="Title" />
    <xsl:param name="CustomErrataType" />

	<xsl:template match="/">

        <xsl:variable name="BasketErrataCount" select="count(/BookingBasket/BasketErrata/Erratum)" />
        <xsl:variable name="DisplayCustomErrata" select="/BookingBasket/CustomErrata/Errata != ''
                                                        and (/BookingBasket/BasketProperties/BasketProperty and $CustomErrataType = 'Property')"/>
        

		<div id="divErrata" class="box primary clear">
			<div class="boxTitle">
				<h2 ml="Errata">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>Booking Conditions</xsl:otherwise>
					</xsl:choose>
				</h2>
			</div>
			<xsl:if test="$DisplayCustomErrata = true()">
				<xsl:call-template name="Markdown">
					<xsl:with-param name="text" select="/BookingBasket/CustomErrata/Errata"/>
				</xsl:call-template>
			</xsl:if>
			<xsl:if test="$BasketErrataCount &gt; 0">				
				<p ml="Errata">The following conditions will apply to your booking:</p>

				<xsl:for-each select="/BookingBasket/BasketErrata/Erratum">
					<p class="erratumDescription">
						<!--no markdown available for this data in ivector but some clients do input html--> 
						<xsl:value-of select="ErratumDescription" disable-output-escaping="yes" />
					</p>					
				</xsl:for-each>
			</xsl:if>
								
            <xsl:if test="$DisplayCustomErrata = true() or $BasketErrataCount &gt; 0">
				<div id="divErrataAccept">
					<label id="lblAcceptErrata" class="checkboxLabel">
            <input type="checkbox" name="chkAcceptErrata" id="chkAcceptErrata" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
						<trans ml="CancellationCharges">To continue with your booking you must accept these booking conditions. I Agree.</trans>
					</label>
				</div>
			</xsl:if>
		</div>

	</xsl:template>

</xsl:stylesheet>

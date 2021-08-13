<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Identifier" />
	<xsl:param name="NavigationType" />
	<xsl:param name="TransitionType" select ="'slide'"/>
	<xsl:param name="HoldTime" select="2" />
	<xsl:param name="ImageWidth" />
	<xsl:param name="CMSBaseURL" />
	<xsl:param name="Overbranded" />
	
	<xsl:template match="/">

		<xsl:choose>
			<xsl:when test="$Overbranded = 'false'">
				<xsl:call-template name="ImageRotator" />
			</xsl:when>
			<xsl:otherwise>					
				<xsl:for-each select="ImageRotator/Overbrand/CustomContent/Content">
					<xsl:call-template name="ImageRotator" />
				</xsl:for-each>
			</xsl:otherwise>		
		</xsl:choose>		
	
	</xsl:template>
	
	
	
<xsl:template name="ImageRotator">
	
	<xsl:variable name="ImageCount" select="count(ImageRotator/Images/Image)" />
	<xsl:variable name="TotalWidth" select="$ImageCount * $ImageWidth" />

	<xsl:for-each select="ImageRotator">

		<xsl:if test="$ImageCount > 0">

			<div id="divImageRotator_{$Identifier}" class="imageRotator {$TransitionType} clear">

				<div id="divImageRotatorWindow_{$Identifier}" class="imageRotatorWindow">

					<div id="divImageRotatorImages_{$Identifier}" class="imageRotatorImages">
						
						<xsl:if test="$TransitionType = 'slide'">
							<xsl:attribute name="style"><xsl:value-of select="concat('width:', $TotalWidth, 'px;')" /></xsl:attribute>
						</xsl:if>

						<xsl:for-each select="Images/Image">

							<xsl:variable name="pos" select="position()" />
							
							<xsl:choose>
								<xsl:when test="URL != ''">
									<a id="aImageRotatorImage_{$Identifier}_{$pos}" href="{URL}" class="mainImage">
										
										<xsl:choose>
											<xsl:when test="$TransitionType = 'fade' and $pos = 1">
												<xsl:attribute name="style">opacity:1;z-index:100;</xsl:attribute>
											</xsl:when>
											<xsl:when test="$TransitionType = 'fade' and $pos > 1">
												<xsl:attribute name="style">opacity:0;z-index:0;</xsl:attribute>
											</xsl:when>											
										</xsl:choose>
										
										<img src="{$CMSBaseURL}{Image}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" />
									</a>
								</xsl:when>
								<xsl:when test="Script != ''">
									<a id="aImageRotatorImage_{$Identifier}_{$pos}" href="#" class="mainImage" onclick="{Script}return false;">
										<img src="{$CMSBaseURL}{Image}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" />
									</a>
								</xsl:when>
								<xsl:otherwise>
									<a id="aImageRotatorImage_{$Identifier}_{$pos}" href="#" class="mainImage" onclick="return false;">
										<img src="{$CMSBaseURL}{Image}" alt="{Image_AdditionalInfo}" title="{Image_ImageTitle}" />
									</a>
								</xsl:otherwise>
							</xsl:choose>

						</xsl:for-each>

					</div>
					
				</div>

				<!-- build navigation links -->

				<!-- numbers -->
				<xsl:if test="contains($NavigationType, 'numbers')">
					<div id="divImageRotatorNumbers_{$Identifier}" class="imageRotatorNumbers">

						<xsl:for-each select="Images/Image">

							<xsl:variable name="pos" select="position()" />

							<a id="aImageRotatorNumber_{$pos}" href="#" onclick="oIR_{$Identifier}.SelectImage({$pos});return false;">
								<xsl:if test="$pos = 1">
									<xsl:attribute name="class">selected</xsl:attribute>
								</xsl:if>
								<xsl:value-of select="$pos"/>
							</a>
									
						</xsl:for-each>
								
					</div>
				</xsl:if>
					
						
				<!-- scrollers -->
				<xsl:if test="contains($NavigationType,'scrollers')">
					<a id="aImageRotatorScrollerLeft_{$Identifier}" class="imageRotatorScroller left" onclick="oIR_{$Identifier}.Backward();">
						<xsl:text> </xsl:text>
					</a>
					<a id="aImageRotatorScrollerRight_{$Identifier}" class="imageRotatorScroller right" onclick="oIR_{$Identifier}.Forward();">
						<xsl:text> </xsl:text>
					</a>
				</xsl:if>

			</div>


			<script type="text/javascript">
				<xsl:value-of select="concat('var oIR_', $Identifier, ' = new ImageRotator(&quot;oIR_', $Identifier, '&quot;);')" />
				<xsl:value-of select="concat('oIR_',$Identifier, '.Setup(&quot;', $Identifier, '&quot;, ', $ImageCount, ', ', $ImageWidth, ', 0.5, &quot;', $NavigationType, '&quot;, &quot;', $TransitionType, '&quot;, ', $HoldTime, ', 1);')" />
			</script>

		</xsl:if>

	</xsl:for-each>
	
</xsl:template>

</xsl:stylesheet>
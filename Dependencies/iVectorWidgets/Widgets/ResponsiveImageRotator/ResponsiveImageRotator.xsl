<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="Identifier" />
	<xsl:param name="NavigationType" />
	<xsl:param name="TransitionType" select ="'slide'"/>
	<xsl:param name="HoldTime" select="2" />
	<xsl:param name="CMSBaseURL" />

	<xsl:template match="/">

		<xsl:variable name="ImageCount" select="count(ImageRotator/Images/Image)" />
		<xsl:variable name="TotalWidth" select="100 * $ImageCount" />

		<xsl:for-each select="ImageRotator">

			<xsl:if test="$ImageCount > 0">

				<div id="divImageRotator_{$Identifier}" class="imageRotator {$TransitionType} clear">

					<div id="divImageRotatorWindow_{$Identifier}" class="imageRotatorWindow">

						<div id="divImageRotatorImages_{$Identifier}" class="imageRotatorImages">

							<xsl:if test="$TransitionType = 'slide'">
								<xsl:attribute name="style">
									<xsl:value-of select="concat('width:', $TotalWidth, '%;')" />
								</xsl:attribute>
							</xsl:if>

							<xsl:for-each select="Images/Image">

								<xsl:variable name="pos" select="position()" />

								<xsl:choose>
									<xsl:when test="URL != ''">
										<a id="aImageRotatorImage_{$Identifier}_{$pos}" href="{URL}" class="mainImage" onclick="return false;">
											<img src="{$CMSBaseURL}{Image}" alt="" />
										</a>
									</xsl:when>
									<xsl:otherwise>
										<a id="aImageRotatorImage_{$Identifier}_{$pos}" href="#" class="mainImage" onclick="return false;">
											<img src="{$CMSBaseURL}{Image}" id="imgImageRotatorImage_{$Identifier}_{$pos}" alt="" />
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
					<xsl:value-of select="concat('var oIR_', $Identifier, ' = new ResponsiveImageRotator(&quot;oIR_', $Identifier, '&quot;);')" />
					<xsl:value-of select="concat('oIR_',$Identifier, '.Setup(&quot;', $Identifier, '&quot;, ', $ImageCount, ', 0.5, &quot;', $NavigationType, '&quot;, &quot;', $TransitionType, '&quot;, ', $HoldTime, ', 1);')" />
				</script>

			</xsl:if>

		</xsl:for-each>

	</xsl:template>

</xsl:stylesheet>
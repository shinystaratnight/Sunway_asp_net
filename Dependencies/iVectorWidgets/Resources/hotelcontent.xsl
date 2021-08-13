<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:param name="PropertyReferenceID"/>
	<xsl:param name="MapMarkerPath"/>

	<xsl:template match="/">
		<xsl:for-each select="Property">
			<div class="leftContent">

				<xsl:call-template name="StarRating">
					<xsl:with-param name="Rating" select="Rating" />
				</xsl:call-template>

				<h3 class="whyHotel" ml="Hotel Results" mlparams="{Name}">
					<xsl:text>Why {0}?</xsl:text>
				</h3>

				<div class="hotelSummary">
					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="Summary" />
					</xsl:call-template>
				</div>

				<h3 class="whyHotel" ml="Hotel Results">
					<xsl:text>Local Attractions</xsl:text>
				</h3>

				<div class="hotelSummary">
					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="Description" />
					</xsl:call-template>
				</div>

				<span id="spnHideHotelContent_{$PropertyReferenceID}" class="hideHoteInfo moreInfo" ml="Hotel Results">see less</span>

			</div>

			<div class="rightContent">

				<div class="mainImage">
					<span style="background:url({MainImage}) no-repeat 0 0;background-size:100% 100%;">
						<xsl:text> </xsl:text>
					</span>
				</div>

				<ul data-propertyreferenceid="{$PropertyReferenceID}" class="imageThumbs">
					<xsl:for-each select="Images/Image">
						<li>
							<span style="background:url({Image}) no-repeat 0 0;background-size:100% 100%;">
								<xsl:text> </xsl:text>
							</span>
						</li>
					</xsl:for-each>
				</ul>

				<!-- Map -->
				<div class="staticMap">
					<span style="background:url(http://maps.googleapis.com/maps/api/staticmap?center={Latitude},{Longitude}&amp;zoom=14&amp;size=480x200&amp;maptype=roadmap&amp;markers=icon:{$MapMarkerPath}|{Latitude},{Longitude}&amp;sensor=false) no-repeat 0 0;background-size:100% 100%;">
						<xsl:text> </xsl:text>
					</span>
				</div>
			</div>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>

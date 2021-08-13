<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">

		<div id="divFavouriteHotels">

			<h2 ml="Favourite Hotels">Favourite Hotels</h2>

			<xsl:for-each select="FavouriteHotels/FavouriteHotel">
				<a class="favouriteHotel" href="{URL}">
					<img src="{MainImage}" alt="Hotel"/>

					<h3>
						<xsl:value-of select="Name"/>
					</h3>
					<p class="location">
						<xsl:value-of select="Resort"/>
						<xsl:text>, </xsl:text>
						<xsl:value-of select="Region"/>
					</p>

					<xsl:call-template name="StarRating">
						<xsl:with-param name="Rating" select="Rating" />
					</xsl:call-template>
				</a>
			</xsl:for-each>

		</div>

	</xsl:template>


	<!-- Star Rating -->
	<xsl:template name="StarRating">
		<xsl:param name="Rating" />
		<xsl:param name="Small" select="'false'"/>


		<xsl:variable name="class">
			<xsl:text>rating star</xsl:text>
			<xsl:value-of select="substring($Rating,1,1)"/>

			<xsl:if test="substring($Rating,3,1)='5'">
				<xsl:text> half</xsl:text>
			</xsl:if>

			<xsl:if test="$Small = 'true'">
				<xsl:text> small</xsl:text>
			</xsl:if>
		</xsl:variable>


		<span class="{$class}">
			<xsl:text> </xsl:text>
		</span>

	</xsl:template>

</xsl:stylesheet>

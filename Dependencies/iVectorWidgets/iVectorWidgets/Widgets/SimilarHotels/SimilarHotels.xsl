<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">

		<div id="divSimilarHotels">
			
				<h2>Similar Hotels</h2>

				<xsl:for-each select="SimilarHotelResults/SimilarHotels/SimilarHotel">
					<div class="similarHotel">
						<div class="left">
							<a href="/hotel-details?propertyid={PropertyReferenceID}">
								<p>
									<xsl:value-of select="Name"/>
								</p>
								<p class="location">
									<xsl:value-of select="Resort"/>
									<xsl:text>, </xsl:text>
									<xsl:value-of select="Region"/>
								</p>
							</a>
						</div>
						<div class="left ratingholder">
							<span class="rating star{Rating}">
								<xsl:text> </xsl:text>
							</span>
						</div>
						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</div>
				</xsl:for-each>

		</div>
		
    </xsl:template>
	
</xsl:stylesheet>

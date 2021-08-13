<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:template match="/">

		<xsl:for-each select="Property[1]">

			<div id="divProperty_Images" class="popupTrans">

				<h3>
					<xsl:value-of select="Region" />
					<xsl:text> - </xsl:text>
					<xsl:value-of select="Resort" />
					<xsl:text> - </xsl:text>
					<xsl:value-of select="Name" />
				</h3>

				<xsl:choose>
					<xsl:when test="count(Images/Image) = 0">
						<p ml="Hotel Results">No images available</p>
					</xsl:when>
					<xsl:otherwise>
						<img id="imgImagePopup_CurrentImage" class="current popupTrans" src="{Images/Image[1]/Image}" alt="{Images/Image[1]/ImageTitle}" onerror="this.src = this.src='https://placehold.it/210x140&amp;text=Image+Missing'" />

						<a id="aImageNavLeft" href="javascript:void(0);" class="nav left popupShortTrans" alt="Gallery Left" style="display:none;">
							<xsl:text> </xsl:text>
						</a>
						<a id="aImageNavRight" href="javascript:void(0);" class="nav right popupShortTrans" alt="Gallery Left">
							<xsl:text> </xsl:text>
						</a>

						<xsl:if test="count(Images/Image) > 1">
							<div id="divImagePopup_Thumbnails" class="thumbnails popupTrans">
								<div class="wrapper">
									<div id="divImagePopup_ThumbnailHolder" class="holder popupTrans">
										<xsl:for-each select="Images/Image">
											<div id="divImagePopup_Thumbnail_{position()}" class="image">
												<img src="{Image}" alt="{ImageTitle}" class="thumbnail" onerror="this.src = this.src='https://placehold.it/210x140&amp;text=Image+Missing'">
													<xsl:if test="position() = 1">
														<xsl:attribute name="class">selected</xsl:attribute>
													</xsl:if>
												</img>
											</div>
										</xsl:for-each>
									</div>
								</div>
								<a id="aThumbNavLeft" href="javascript:void(0);" class="thumbNav left popupTrans" alt="Thumbnails Left">
									<xsl:text> </xsl:text>
								</a>
								<a id="aThumbNavRight" href="javascript:void(0);" class="thumbNav right popupTrans" alt="Thumbnails Right">
									<xsl:text> </xsl:text>
								</a>
							</div>
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<xsl:template match="/">

	<div id="divNavigationBar">

		<ul>
			<li class="title">
				<xsl:value-of select="NavigationBar/NavigationBarTitle"/>
			</li>
			<xsl:for-each select="NavigationBar/MainMenu/MainMenuItem">
			<li>
				<a href="{URL}">
					<xsl:value-of select="Name"/>
				</a>
				<ul class="droplist">
					<xsl:for-each select="SubMenu/SubMenuItem">
						<li>
							<a href="{URL}">
								<xsl:value-of select="Name"/>
							</a>
						</li>
					</xsl:for-each>
				</ul>
			</li>
			</xsl:for-each>

		</ul>

		<div class="clear">
			<xsl:text> </xsl:text>
		</div>

	</div>
	
</xsl:template>

</xsl:stylesheet>
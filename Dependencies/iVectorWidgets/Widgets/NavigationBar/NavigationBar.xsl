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
				<!-- menu items -->
				<a href="{URL}">
					<xsl:value-of select="Name"/>
				</a>
				
				<!-- sub menu items -->
				<xsl:if test="count(SubMenu/SubMenuItem) &gt; 0">
					<ul class="droplist sub">
						<xsl:for-each select="SubMenu/SubMenuItem">
							<li>
								<a class="icon chevron-right" href="{URL}">
									<xsl:value-of select="Name"/>
									<i>
										<xsl:text> </xsl:text>
									</i>
								</a>
								
								<!-- sub-sub menu items -->
								<xsl:if test="count(SubSubMenu/SubSubMenuItem) &gt; 0">

									<ul class="droplist subsub">
										<xsl:for-each select="SubSubMenu/SubSubMenuItem">
											<li>
												<a href="{URL}">
													<xsl:value-of select="Name"/>
												</a>
											</li>
										</xsl:for-each>
									</ul>
								</xsl:if>
							</li>
						</xsl:for-each>
					</ul>
				</xsl:if>
			</li>
			</xsl:for-each>

		</ul>

		<div class="clear">
			<xsl:text> </xsl:text>
		</div>

	</div>
	
</xsl:template>

</xsl:stylesheet>
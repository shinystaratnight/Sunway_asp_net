<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">		

			<xsl:for-each select="Sitemap">

				<div id="divSitemap">

					<xsl:if test="Header != ''">
						<h1>
							<xsl:value-of select="Header"/>
						</h1>
					</xsl:if>

					<xsl:for-each select="Groups/Group">
						<div class="group clear">

							<!--Title-->
							<xsl:if test="@Title != ''">
								<h2>
									<xsl:choose>
										<xsl:when test="@URL != ''">
											<a href="{@URL}">
												<xsl:value-of select="@Title"/>
											</a>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="@Title"/>
										</xsl:otherwise>
									</xsl:choose>
								</h2>
							</xsl:if>
							
							<!--Pages-->
							<xsl:variable name="TotalPages" select="count(Pages/Page)"/>
							<xsl:variable name="Rows" select="ceiling(count(Pages/Page) div 4)"/>

							<div class="pages clear">
								<xsl:call-template name="Columns">
									<xsl:with-param name="Rows" select="$Rows"/>
									<xsl:with-param name="Total" select="$TotalPages"/>
									<xsl:with-param name="Count" select="1"/>
									<xsl:with-param name="Start" select="1"/>
									<xsl:with-param name="End" select="$Rows"/>
								</xsl:call-template>	
							</div>
							
						</div>
					</xsl:for-each>
				
	
				</div>

			</xsl:for-each>	
	

	</xsl:template>

	<xsl:template name="Columns">
		<xsl:param name="Rows"/>
		<xsl:param name="Total"/>
		<xsl:param name="Count"/>
		<xsl:param name="Start"/>
		<!--<xsl:param name="End"/>-->


		<xsl:variable name="End" select="$Start + $Rows"/>

		<xsl:if test="$Count &lt;= 4">

			<div class="col col{$Count}">				
					
				<xsl:for-each select="Pages/Page[position() >= $Start and  position() &lt; $End]">

					<xsl:choose>
						<xsl:when test="@Type = 'Heading'">
							<h3>
								<a href="{@URL}">
									<xsl:value-of select="@Name"/>
								</a>
							</h3>
						</xsl:when>
						<xsl:otherwise>
							<a href="{@URL}">
								<xsl:value-of select="@Name"/>
							</a>
						</xsl:otherwise>
					</xsl:choose>

				</xsl:for-each>

				<xsl:text> </xsl:text>

			</div>
			
			<xsl:call-template name="Columns">
				<xsl:with-param name="Total" select="$Total"/>
				<xsl:with-param name="Count" select="$Count + 1"/>
				<xsl:with-param name="Start" select="$End"/>
				<xsl:with-param name="Rows" select="$Rows"/>
			</xsl:call-template>
		</xsl:if>

		

	</xsl:template>

</xsl:stylesheet>

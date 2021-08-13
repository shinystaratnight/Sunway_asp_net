<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:template match="/">

		<div class="sidebarBox linklist">

			<div class="boxTitle">
				<h2 ml="Useful Links">Useful Links</h2>
			</div>

			<xsl:call-template name="Markdown">
				<xsl:with-param name="text" select="UsefulLinks"/>
			</xsl:call-template>

		</div>

	</xsl:template>

</xsl:stylesheet>

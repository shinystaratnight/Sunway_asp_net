<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">

		<div id="divErrorHandler" class="contentBox">
			<div class="logo">
				<xsl:text> </xsl:text>
			</div>
			<div ml="Error">
				An unexpected error has occurred.
			</div>
			<div>
				<a href="/" ml="Error">Return to Homepage</a>
			</div>
		</div>

	</xsl:template>
</xsl:stylesheet>


<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="RequiresAccepting" />
  <xsl:param name="HideCookieMessage"/>
  
	<xsl:template match="/">

		<div class="cookieCompliance" id="divCookieCompliance">

			<div class="container">
				
				<p>
					<xsl:value-of select ="CookieCompliance/Message"/>
					<xsl:text> </xsl:text>
				</p>

				<a href="javascript:void(0);" id="aCookieCompliance_Accept" class="button primary icon"  ml="CookieCompliance" >Accept</a>

				<xsl:if test ="CookieCompliance/CookiePageURL">
					<a href="{CookieCompliance/CookiePageURL}" ml="CookieCompliance" class="button icon">More Information</a>
				</xsl:if>

				<xsl:text> </xsl:text>
			</div>

			<xsl:text> </xsl:text>
		</div>

		<script type="text/javascript">
			<xsl:text>int.ll.OnLoad.Run(function () { CookieCompliance.Setup('</xsl:text>
			<xsl:value-of select ="$RequiresAccepting"/>
      <xsl:text>', '</xsl:text>
      <xsl:value-of select ="$HideCookieMessage"/>
			<xsl:text>'); });</xsl:text>
		</script>		

	</xsl:template>

</xsl:stylesheet>

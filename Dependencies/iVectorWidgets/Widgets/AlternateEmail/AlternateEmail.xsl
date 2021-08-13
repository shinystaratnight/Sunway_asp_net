<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:param name="TradeContactEmail" />

	<xsl:template match="/">

		<div id="divAlternateEmail" class="alternate-email box primary">
			
			<div class="boxTitle">
				<h2 ml="Alternate Email">Confirmation Email</h2>
			</div>
			
			<p>
				<trans ml="Alternate Email">Your documentation will be sent to</trans>
				<xsl:text> </xsl:text>
				<xsl:value-of select="$TradeContactEmail"/>
			</p>
			
			<p ml="Alternate Email">If you would like it to be sent to a different email address, please provide your email address below:</p>
			
			<input type="text" name="txtAlternateEmail_Email" id="txtAlternateEmail_Email" class="textbox" />
			
			<input type="hidden" name="hidAlternateEmail_EmailPlaceholder" id="hidAlternateEmail_EmailPlaceholder" value="Email" />
			<input type="hidden" name="hidAlternaeEmail_TradeContactEmail" id="hidAlternaeEmail_TradeContactEmail" value="{$TradeContactEmail}" />
			
			<script type="text/javascript">	
				int.ll.OnLoad.Run(function () { AlternateEmail.Setup(); });
			</script>
			
		</div>

	</xsl:template>

</xsl:stylesheet>

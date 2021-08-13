<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>


	<xsl:template match="/">

		<div id="divTradeReference" class="trade-reference box primary">

			<div class="boxTitle">
				<h2>
					<trans ml="Trade Reference">Your Reference</trans>
					<span ml="Trade Reference">*</span>
				</h2>
			</div>

			<input type="text" name="txtTradeReference" id="txtTradeReference" class="textbox" maxlength="20" />
			<input type="hidden" name="hidTradeReferencePlaceholder" id="hidTradeReferencePlaceholder" value="Reference" />

			<script type="text/javascript">
				int.ll.OnLoad.Run(function () { TradeReference.Setup(); });
			</script>

		</div>

	</xsl:template>

</xsl:stylesheet>

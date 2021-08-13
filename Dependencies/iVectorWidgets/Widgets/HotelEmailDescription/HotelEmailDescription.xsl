<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	<xsl:param name="PropertyReferenceID" />
	<xsl:include href="../../xsl/markdown.xsl "/>
	<xsl:include href="../../xsl/functions.xsl "/>



	<xsl:template match="/">
		<!-- Email hotel -->
		<div id="divHotelEmailDescription">
			<h4 ml="Hotel Results" >Email description to...</h4>
			<p id="pHotelEmailDescription_EmailDone" style="display:none;" ml="Hotel Results">Thank you, the quote has been e-mailed.</p>
			<p id="pHotelEmailDescription_EmailError" style="display:none;" ml="Hotel Results">Please ensure madatory fields have been entered</p>


			<xsl:variable name="EmailPlaceholder">
				<trans ml="Hotel Results">To: Enter email address here</trans>
				<xsl:text> *</xsl:text>
			</xsl:variable>
			<xsl:variable name="RecipientPlaceholder">
				<trans ml="Hotel Results">From: Enter email address here</trans>
				<xsl:text> *</xsl:text>
			</xsl:variable>
			<xsl:variable name="MessagePlaceholder">
				<trans ml="Hotel Results">Type your message here</trans>
			</xsl:variable>


			<xsl:variable name="EmailPlaceholder_JSSafe">
				<xsl:call-template name="Replace">
					<xsl:with-param name="text" select="$EmailPlaceholder" />
					<xsl:with-param name="find" select='"&apos;"' />
					<xsl:with-param name="replacement" select='"\&apos;"' />
				</xsl:call-template>
			</xsl:variable>

			<xsl:variable name="RecipientPlaceholder_JSSafe">
				<xsl:call-template name="Replace">
					<xsl:with-param name="text" select="$RecipientPlaceholder" />
					<xsl:with-param name="find" select='"&apos;"' />
					<xsl:with-param name="replacement" select='"\&apos;"' />
				</xsl:call-template>
			</xsl:variable>

			<xsl:variable name="MessagePlaceholder_JSSafe">
				<xsl:call-template name="Replace">
					<xsl:with-param name="text" select="$MessagePlaceholder" />
					<xsl:with-param name="find" select='"&apos;"' />
					<xsl:with-param name="replacement" select='"\&apos;"' />
				</xsl:call-template>
			</xsl:variable>

			<input id='hidEmailPlaceholder' type='hidden' value='{$EmailPlaceholder_JSSafe}' />
			<input id='hidRecipientPlaceholder' type='hidden' value='{$RecipientPlaceholder_JSSafe}' />
			<input id='hidMessagePlaceholder' type='hidden' value='{$MessagePlaceholder_JSSafe}' />

			<input id="txtHotelEmailDescription_EmailYourAddress" type="text" class="textbox" ml="Hotel Results" />
			<input id="txtHotelEmailDescription_EmailRecipientAddress" type="text" class="textbox" ml="Hotel Results" />
			<!--<input id="txtHotelEmailDescription_EmailMessage" type="text" class="textbox" ml="Hotel Results" />-->
			<!--<textarea rows="3" id="txtHotelEmailDescription_EmailMessage" class="textbox" ml="Hotel Results"> </textarea>-->

			<textarea  id="txtHotelEmailDescription_EmailMessage" class="textbox" ml="Hotel Results" rows="4" placeholder="Type your message here">
				<xsl:text> </xsl:text>
			</textarea>

			<input type="button" class="button primary" value="Send" ml="Hotel Results" onclick="HotelEmailDescription.EmailDescription({$PropertyReferenceID});" />
		</div>


	</xsl:template>

</xsl:stylesheet>
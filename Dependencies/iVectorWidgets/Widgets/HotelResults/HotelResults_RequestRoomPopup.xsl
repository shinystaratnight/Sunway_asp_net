<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:template match="/">

		<div class="overlay">
			<xsl:text> </xsl:text>
		</div>

		<div class="popup">
			<p id="pRoomRequestSuccess" class="message success" ml="Hotel Results" style="display:none;">Your request has been sent to our Resort Specialists who will contact you to confirm the availability of your chosen accommodation and proceed with your booking.</p>
			<p id="pRoomRequestFailed" class="message failed" ml="Hotel Results" style="display:none;">Sorry, there has been a problem sending your request.</p>
			<p id="pRoomRequestInvalid" class="message failed" ml="Hotel Results" style="display:none;">Please ensure you have filled in each field.</p>
			<div class="form">
				<div>
					<label ml="Hotel Results">Lead Passenger Name:</label>
					<input type="text" id="txtRequestRoom_Name" class="textbox" />
					<xsl:text> </xsl:text>
				</div>
				<div>
					<label ml="Hotel Results">Email Address:</label>
					<input type="text" id="txtRequestRoom_Email" class="textbox" />
					<xsl:text> </xsl:text>
				</div>
				<div>
					<label ml="Hotel Results">Contact Telephone:</label>
					<input type="text" id="txtRequestRoom_Telephone" class="textbox" />
					<xsl:text> </xsl:text>
				</div>
				<xsl:text> </xsl:text>
			</div>
			<div class="buttons">
				<input type="button" class="button primary large" value="CLOSE" ml="Hotel Results" onclick="HotelResults.CloseRequestRoomPopup();" />
				<input id="btnRoomRequest_Submit" type="button" class="button primary large" value="SUBMIT" ml="Hotel Results" onclick="HotelResults.RequestRoom();" />
				<xsl:text> </xsl:text>
			</div>
			<xsl:text> </xsl:text>
		</div>
		
	</xsl:template>

</xsl:stylesheet>
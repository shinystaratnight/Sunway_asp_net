<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/Markdown.xsl"/>

	<xsl:param name="CMSBaseURL" />

	<xsl:template match="/">

		<div id="divHotelDetails">

			<div class="top">
				<h1>
					<xsl:value-of select="Name"/><span class="rating star3"><xsl:text> </xsl:text></span></h1>
				<h4>
					<xsl:if test="Address1 != ''">
						<xsl:value-of select="Address1"/>,
					</xsl:if>
					<xsl:if test="Address2 != ''">
						<xsl:value-of select="Address2"/>,
					</xsl:if>
					<xsl:if test="Resort != ''">
						<xsl:value-of select="Resort"/>,
					</xsl:if>
					<xsl:if test="PostCodeZip != ''">
						<xsl:value-of select="PostCodeZip"/>
					</xsl:if>
				</h4>
			</div>

			<div>
				
				<!-- Main Image -->
				<img id="imgMainImage" src="/Themes/GoSyden/Images/call-center.jpg" alt="Test"/>

				<!-- Other images -->

				<div id="divOtherImages">
					<img src="/Themes/GoSyden/Images/footer-icons.jpg" alt="Test" onmouseover="HotelDetails.ImageHover(this)"/>
					<img src="/Themes/GoSyden/Images/call-center.jpg" alt="Test" onmouseover="HotelDetails.ImageHover(this)"/>
				</div>

				<div class="clear">
					<xsl:text> </xsl:text>
				</div>

			</div>

			<div>
				<xsl:call-template name="Markdown">
					<xsl:with-param name="text" select="Description"/>
				</xsl:call-template>
			</div>

			<div class="roomoptions">
				<table class="roomoptions">
					<thead>
						<tr>
							<th>Room Type</th>
							<th>Board Basis</th>
							<th>Holiday Type</th>
							<th>Total Price</th>
							<th>
								<xsl:text> </xsl:text>
							</th>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>Double Room</td>
							<td>Bed and Breakfast</td>
							<td>Hotel Only</td>
							<td>£500</td>
							<td>
								<a href="#" class="button primary small">Book</a>
							</td>
						</tr>
						<tr class="alt">
							<td>Double Room</td>
							<td>Half Board</td>
							<td>Hotel Only</td>
							<td>£560</td>
							<td>
								<a href="#" class="button primary small">Book</a>
							</td>
						</tr>
					</tbody>
				</table>
			</div>

			<div>
				<h3>Flight Details</h3>

				<h4>Outbound</h4>
				<dl>
					<dd>Departure Airport</dd>
					<dt>Airport Name</dt>
					<dd>Departs</dd>
					<dt>16 June 2013 12:00</dt>
				</dl>
				<dl>
					<dd>Arrival Airport</dd>
					<dt>Airport Name</dt>
					<dd>Arrives</dd>
					<dt>16 June 2013 17:00</dt>
				</dl>
				<div class="clear">
					<xsl:text> </xsl:text>
				</div>

				<h4>Inbound</h4>
				<dl>
					<dd>Departure Airport</dd>
					<dt>Airport Name</dt>
					<dd>Departs</dd>
					<dt>21 June 2013 11:00</dt>
				</dl>
				<dl>
					<dd>Arrival Airport</dd>
					<dt>Airport Name</dt>
					<dd>Arrives</dd>
					<dt>21 June 2013 16:00</dt>
				</dl>
				<div class="clear">
					<xsl:text> </xsl:text>
				</div>

			</div>

			<div>
				<xsl:if test="count(Facilities/Facility[FacilityType = 'Hotel']) &gt; 0">
					<div class="detail">
						<h1>Hotel Facilities</h1>
						<ul id="ulHotelFacilities" class="facilties">
							<xsl:for-each select="Facilities/Facility[FacilityType = 'Hotel'][position() &lt; 17]">
								<li>
									<xsl:value-of select="Facility"/>
									<xsl:text> </xsl:text>
								</li>
							</xsl:for-each>
						</ul>
						<xsl:if test="count(Facilities/Facility[FacilityType = 'Hotel']) > 16">
							<a id="aShowHotelFacilities" onclick="f.Hide('ulHotelFacilities');f.Hide('aShowHotelFacilities');f.Show('ulMoreHotelFacilities');f.Show('aHideHotelFacilities');return false;">Show more</a>
							<ul id="ulMoreHotelFacilities" style="display:none" class="facilties">
								<xsl:for-each select="Facilities/Facility[FacilityType = 'Hotel'][position() &lt; 17]">
									<li>
										<xsl:value-of select="Facility"/>
										<xsl:text> </xsl:text>
									</li>
								</xsl:for-each>
							</ul>
							<a id="aHideHotelFacilities" onclick="f.Show('ulHotelFacilities');f.Show('aShowHotelFacilities');f.Hide('ulMoreHotelFacilities');f.Hide('aHideHotelFacilities');return false;">Show more</a>
						</xsl:if>
						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</div>
				</xsl:if>

				<xsl:if test="count(Facilities/Facility[FacilityType = 'Room']) &gt; 0">
					<div class="detail">
						<h1>Room Facilities</h1>
						<ul id="ulRoomFacilities" class="facilties">
							<xsl:for-each select="Facilities/Facility[FacilityType = 'Room'][position() &lt; 17]">
								<li>
									<xsl:value-of select="Facility"/>
									<xsl:text> </xsl:text>
								</li>
							</xsl:for-each>
						</ul>
						<xsl:if test="count(Facilities/Facility[FacilityType = 'Room']) > 16">
							<a id="aShowRoomFacilities" onclick="f.Hide('ulRoomFacilities');f.Hide('aShowRoomFacilities');f.Show('ulMoreRoomFacilities');f.Show('aHideRoomFacilities');return false;">Show more</a>
							<ul id="ulMoreRoomFacilities" style="display:none" class="facilties">
								<xsl:for-each select="Facilities/Facility[FacilityType = 'Room'][position() &lt; 17]">
									<li>
										<xsl:value-of select="Facility"/>
										<xsl:text> </xsl:text>
									</li>
								</xsl:for-each>
							</ul>
							<a id="aHideRoomFacilities" onclick="f.Show('ulRoomFacilities');f.Show('aShowRoomFacilities');f.Hide('ulMoreRoomFacilities');f.Hide('aHideRoomFacilities');return false;">Show more</a>
						</xsl:if>
						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</div>
				</xsl:if>

				<xsl:if test="/SEOPage/PropertyContent/HotelPolicy != ''">
					<div class="detail">
						<h1>Hotel Policy</h1>
						<xsl:call-template name="Markdown">
							<xsl:with-param name="text" select="/SEOPage/PropertyContent/HotelPolicy"/>
						</xsl:call-template>

						<div class="clear">
							<xsl:text> </xsl:text>
						</div>
					</div>
				</xsl:if>

				<div class="clear">
					<xsl:text> </xsl:text>
				</div>
			</div>
			<div>
				<a href="#">&lt; Back to Search results</a>
			</div>

			<script type="text/javascript">
				<xsl:text disable-output-escaping="yes">&lt;!--
						if (!window.addEventListener) {
							window.attachEvent('onload', function () { HotelDetails.Setup(); });
						} else {
							window.addEventListener('load', function () { HotelDetails.Setup(); }, false);
						};
			
					--&gt;
				</xsl:text>
			</script>

		</div>

</xsl:template>

</xsl:stylesheet>
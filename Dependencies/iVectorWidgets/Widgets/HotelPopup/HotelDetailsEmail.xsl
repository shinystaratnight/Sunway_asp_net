<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="BaseURL" />
	<xsl:param name="Theme" />
	<xsl:param name="Message" />
	<xsl:param name="SenderName" />
	<xsl:param name="SellingCurrencySymbol" />
	<xsl:param name="RoomCount" />
	<xsl:param name="ResultsSummary" />
	<xsl:param name="EmailLogoName" />

	<xsl:template match="/">

		<xsl:variable name="tradeName">
			<xsl:value-of select="'bookabed.ie'" />
		</xsl:variable>

		<html>
			<head>
				<style type="text/css">
					table {width:100%;}
					table tr td {color:#74757c;text-align:left;font-size:11px;font-family:Verdana,sans-serif;}
					table tr th {text-align:left;font-weight:normal;color:#74757c;font-size:13px;font-family:Verdana,sans-serif;}
					table tr td h4 {color:#74757c;margin:5px 0 10px 0;font-size:13px;font-weight:normal;}
					p {padding:0;margin:0 15px 0 0;}
					div p {margin:0;}
					table, tr, td {vertical-align:top;}
					img {border:none;}
					h3 {font-size:12px;margin:5px 0;color:#74757c;font-weight:normal;}
					a {color:#666;}
					ul {list-style-type:none;padding:0 0 0 10px;margin:0;}
				</style>
				<style type="text/css">#outlook a {padding: 0 0 0 0;}</style>
			</head>

			<body style="text-align:center;background:#f6f6ee;">
				<table style="font-family:Verdana,sans-serif;color:#74757c;font-size:12px;line-height:1.4em;border-collapse:collapse;padding:0 10px 0 10px;width:630px;background:#fff;border:solid 1px #cacaca;">

					<tr style="padding:5px 10px 5px 10px;">
						<td colspan="2">

							<div style="display:block;margin:0 0 10px 0;font-size:9px;line-height:1.0em;">
								<a ml="PropertyEmail">
									<xsl:value-of select="concat('You have been sent this email as your email address was entered to receive these property details on ', $tradeName)" />
								</a>
							</div>

							<div style="display:block;margin:0 0 40px;font-size:14px;">
								<xsl:choose>
									<xsl:when test="$Message != ''">
										<h2 style="font-size:18px;margin:0;">
											<trans ml="PropertyEmail" mlparams="{$SenderName}">
												{0} says
											</trans>
										</h2>
										<span style="font-size:20px;">"</span>
										<span style="color:#74757c;">
											<xsl:value-of select="$Message" />
										</span>
										<span style="font-size:20px;">"</span>
									</xsl:when>
									<xsl:otherwise>
										<h2 style="font-size:18px;margin:0;">
											<trans ml="PropertyEmail" mlparams="{$SenderName}">
												{0} says
											</trans>
										</h2>
										<span style="font-size:20px;">"</span>
										<span ml="PropertyEmail" style="color:#74757c;">Check out this hotel</span>
										<span style="font-size:20px;">"</span>
									</xsl:otherwise>
								</xsl:choose>
							</div>

							<img src="{$BaseURL}Themes/{$Theme}/Images/{$EmailLogoName}" alt="{$Theme}" />
						</td>
					</tr>

					<xsl:for-each select="Property">
						<tr style="padding:10px 10px 10px 10px;">

							<td style="vertical-align:top;">

								<!-- Hotel Name -->
								<h2 style="color:#74757c">
									<xsl:value-of select="Name" />
								</h2>
								<!-- Rating-->
								<!--<div class="starRating">
									<span class="Star{round(Rating)}">
										<xsl:text> </xsl:text>
									</span>
								</div>-->
								<!-- Location -->
								<h4 class="location">
									<xsl:value-of select="concat(Region,', ',Resort)" />
								</h4>
								<p class="address">
									<xsl:value-of select="Address1" />
									<xsl:if test="Address2 and Address2 != ''">
										<xsl:value-of select="concat(', ', Address2)" />
									</xsl:if>
									<xsl:if test="TownCity and TownCity != ''">
										<xsl:value-of select="concat(', ', TownCity)" />
									</xsl:if>
									<xsl:if test="County and County != ''">
										<xsl:value-of select="concat(', ', County)" />
									</xsl:if>
									<xsl:if test="PostCodeZip and PostCodeZip != ''">
										<xsl:value-of select="concat(', ', PostCodeZip)" />
									</xsl:if>
								</p>

								<!-- description -->
								<xsl:call-template name="Markdown">
									<xsl:with-param name="text" select="Description" />
								</xsl:call-template>

								<!-- Map -->
								<xsl:if test="../MapDefinition/MapPoints/MapPoint[Type = 'Property']/Latitude != 0">
									<h5 style="color:#5191ce;font-weight:normal;margin:20px 0 10px 0;" ml="PropertyEmail">Map</h5>
									<img src="https://maps.google.com/maps/api/staticmap?center={Latitude},{Longitude}&amp;zoom=14&amp;size=370x200&amp;maptype=roadmap&amp;markers=color:blue|{Latitude},{Longitude}&amp;sensor=false" alt="" />
								</xsl:if>
							</td>

							<!-- Images -->
							<td style="vertical-align:top;width:221px;">
								<xsl:for-each select="Images/Image[Image != '' and position() &lt; 7]">
									<div style="margin-bottom:20px;padding:4px;border:solid 1px #ddd;">
										<img height="140" width="210" src="{Image}" alt="{ImageTitle}" />
									</div>
								</xsl:for-each>
								<xsl:text> </xsl:text>
							</td>
						</tr>

						<tr>
							<td colspan="2" style="color:#666; font-family:arial; font-size:13px; font-weight:bold; margin:10px 0;">
								<xsl:value-of select="$ResultsSummary" />
								<xsl:text> </xsl:text>
							</td>
						</tr>

						<xsl:for-each select="ResultDetails">
							<tr>
								<td colspan="2">
									<xsl:variable name="NumberOfRooms">
										<xsl:value-of select="count(Rooms/Room)" />
									</xsl:variable>

									<xsl:for-each select="Rooms/Room">

										<xsl:variable name="RoomNumber">
											<xsl:value-of select="position()" />
										</xsl:variable>

										<xsl:if test="$NumberOfRooms &gt; 1">
											<p class="room">

												<trans ml="PropertyEmail">Room</trans>
												<xsl:value-of select="position()" />
												<xsl:text>: </xsl:text>

												<xsl:if test="Adults &gt; 0">
													<xsl:value-of select="Adults" />
													<xsl:text> </xsl:text>
													<trans ml="PropertyEmail">Adults</trans>
												</xsl:if>

												<xsl:if test="Children &gt; 0">
													<xsl:text>, </xsl:text>
													<xsl:value-of select="Children" />
													<xsl:text> </xsl:text>
													<trans ml="PropertyEmail">Children</trans>
												</xsl:if>

												<xsl:if test="Infants &gt; 0">
													<xsl:text>, </xsl:text>
													<xsl:value-of select="Infants" />
													<xsl:text> </xsl:text>
													<trans ml="PropertyEmail">Infants</trans>
												</xsl:if>
											</p>
										</xsl:if>
									</xsl:for-each>

									<xsl:if test="$NumberOfRooms &gt; 1">
										<div class="clearing">
											<xsl:text> </xsl:text>
										</div>
									</xsl:if>
								</td>
							</tr>
						</xsl:for-each>
					</xsl:for-each>
					<tr>
						<td colspan="2" style="padding:5px;">
							<xsl:text> </xsl:text>
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
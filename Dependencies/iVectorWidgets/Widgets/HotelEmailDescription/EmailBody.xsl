<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl "/>
	<xsl:include href="../../xsl/markdown.xsl "/>

	<xsl:param name="Message"/>
	<xsl:param name="Duration"/>
	<xsl:param name="DepartureDate"/>
	<xsl:param name="CurrencySymbol"/>
	<xsl:param name="CurrencySymbolPosition"/>
	<xsl:param name="TotalPax"/>
	<xsl:param name="Print"/>
	


	<xsl:param name="SellingCurrencySymbol" />

	<!--<xsl:param name="EmailLogoName"/>-->


	<xsl:template match="/">

		<html>
			<head>
				<style type="text/css">
					body{color:#111;}
					table tr td {;text-align:left;font-size:11px;font-family:Verdana,sans-serif;}
					table tr th {text-align:left;font-weight:normal;font-size:13px;font-family:Verdana,sans-serif;}
					table tr td h4 {margin:5px 0 10px 0;font-size:13px;font-weight:normal;}
					p {padding:0;margin:0 15px 0 0;}
					div p {margin:0;}
					table, tr, td {vertical-align:top;}
					img {border:none;}
					h3 {font-size:12px;margin:5px 0;font-weight:normal;}
					h4 {font-size:14px;font-weight:bold;}
					a {color:#666;}
					ul {list-style-type:none;padding:0 0 0 10px;margin:0;}
				</style>
				<style type="text/css">#outlook a {padding: 0 0 0 0;}</style>
			</head>

			<body style="text-align:center;background:#f6f6ee;">
				<xsl:if test="$Print= 'True'">
					<a class="print" href="javascript:window.print()"><span ml="Hotel Results" >Print</span></a>
				</xsl:if>
				
				<table style="font-family:Verdana,sans-serif;color:#74757c;font-size:12px;line-height:1.4em;border-collapse:collapse;padding:0 10px 0 10px;width:900px;background:#fff;border:solid 1px #cacaca;" width="630">


					<tr style="padding:5px 10px 5px 10px;">
						<td colspan="2">
							<table width="900" style="width:900px;font-family:Verdana,sans-serif;">
								<tr>
									<td rowspan="6">
										<img src="{Property/Trade/Logo}" alt="Travel" style="width:900px;" width="900" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="2" style="background-color:#111; color:#fff;">
							<div style="float:left;">
								<span></span>
								
							</div>
							<div>
								
							</div>
							
							<table style="color:#fff; font-family:Verdana,sans-serif;">
								<tr>
									<td>											
										<xsl:value-of select="concat(substring($DepartureDate,1,10), ' ')"/>
										<trans ml="PropertyEmail" mlparams="{$Duration}">
											<xsl:text>, {0} nights</xsl:text>
										</trans>
									</td>
								</tr>
								<tr>
									<td style="width:900px;">
										<h2 style="color:#fff;">
											<xsl:value-of select ="Property/Name"/>
										</h2>
									</td>
								</tr>
								
							</table>
							
							
						</td>
					</tr>

					<tr style="padding:5px 10px 5px 10px;">
						<td colspan="2">
							<div style="display:block;margin:0 0 15px;font-size:14px;">

								<xsl:if test="$Message != ''">

									<span>
										<xsl:value-of select="$Message"/>
									</span>
								</xsl:if>

							</div>

						</td>
					</tr>

					<xsl:for-each select="Property">
						<tr style="padding:10px 10px 10px 10px;">

							<td style="vertical-align:top;">

								<!-- description -->
								<xsl:call-template name="Markdown">
									<xsl:with-param name="text" select="Description" />
								</xsl:call-template>
								
								<div style="margin-top:20px;">
									<img src="https://maps.google.com/maps/api/staticmap?center={Latitude},{Longitude}&amp;zoom=14&amp;size=650x250&amp;maptype=roadmap&amp;markers=color:blue|{Latitude},{Longitude}&amp;sensor=false" alt="" />
								</div>
								
							</td>


							<!-- Images -->
							<td style="vertical-align:top;width:221px;">
								<xsl:for-each select="Images/Image[Image != '' and position() &lt; 5]">
									<div style="margin-bottom:20px;padding:4px;border:solid 1px #ddd;">
										<img height="140" width="210" src="{Image}" alt="{ImageTitle}"/>
									</div>
								</xsl:for-each>
								<xsl:text> </xsl:text>
							</td>


						</tr>
						
					</xsl:for-each>
				
					<tr>
						<td colspan="2" style="padding:5px;">
							<table style="width:900px;font-family:Verdana,sans-serif;" cellspacing="0">
								<tr style="background-color:#aab824;color:#FFF; padding-top:10px;padding-bottom:10px;margin-top:5px;">
									<td ml="PropertyEmail" style="padding-left:5px;">Room Type</td>
									<td ml="PropertyEmail">Meal Basis</td>
								</tr>
								<xsl:for-each select ="Property/Hotel/Rooms/Room/RoomOptions/RoomOption">
									<tr style="padding-top:10px;">
										<td style="padding-left:5px;">
											<xsl:value-of select ="RoomType"/>
										</td>
										<td>
											<xsl:value-of select ="MealBasis"/>
										</td>
									</tr>
								</xsl:for-each>
							</table>
						</td>
					</tr>

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
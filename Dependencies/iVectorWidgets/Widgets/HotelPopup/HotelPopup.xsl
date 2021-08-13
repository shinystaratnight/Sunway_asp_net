<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:include href="../../xsl/Markdown.xsl" />
	<xsl:include href="../../xsl/Functions.xsl" />

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="PropertyReferenceID" />
	<xsl:param name="MapMarkerPath" />
	<xsl:param name="PerPersonPrice" />

	<xsl:template match="/">

		<div id="divHotelPopup">

			<a class="print" href="javascript:window.print()">
				<span ml="Hotel Results" >Print</span>
			</a>

			<xsl:for-each select="Property">

				<!-- Name -->
				<h2>
					<xsl:value-of select="Name" />
				</h2>

				<!-- Rating -->
				<xsl:call-template name="StarRating">
					<xsl:with-param name="Rating" select="Rating" />
				</xsl:call-template>

				<!-- Geography -->
				<h3 class="geography">
					<xsl:value-of select="concat(Region, ', ', Resort)" />
				</h3>

				<!-- Address -->
				<p>
					<xsl:value-of select="concat(Address1, ' ', Address2, ' ', TownCity, ' ', PostcodeZip)" />
				</p>

				<div id="divHotelPopup_ScrollingContent">

					<div class="left">

						<!-- Description -->
						<xsl:call-template name="Markdown">
							<xsl:with-param name="text" select="Description" />
						</xsl:call-template>

						<!-- Map -->
						<img id="imgStaticMap" title="{Name}" alt="" src="https://maps.googleapis.com/maps/api/staticmap?center={Latitude},{Longitude}&amp;zoom=14&amp;size=480x200&amp;maptype=roadmap&amp;markers=icon:{$MapMarkerPath}|{Latitude},{Longitude}&amp;sensor=false&amp;" />

						<!-- Email hotel -->
						<div id="divHotelPopup_EmailForm">
							<h4 ml="Hotel Results" >Email these details</h4>
							<p id="pHotelPopup_EmailDone" style="display:none;" ml="Hotel Results">Thank you, the details have been e-mailed.</p>
							<p id="pHotelPopup_EmailError" style="display:none;" ml="Hotel Results">Please ensure madatory fields have been entered</p>
							<p ml="Hotel Results" >To email these details, please complete the following;</p>

							<xsl:variable name="NamePlaceholder">
								<trans ml="Hotel Results">Your Name</trans>
								<xsl:text> *</xsl:text>
							</xsl:variable>
							<xsl:variable name="EmailPlaceholder">
								<trans ml="Hotel Results">Your Email</trans>
								<xsl:text> *</xsl:text>
							</xsl:variable>
							<xsl:variable name="RecipientPlaceholder">
								<trans ml="Hotel Results">Recipient's Email</trans>
								<xsl:text> *</xsl:text>
							</xsl:variable>
							<xsl:variable name="MessagePlaceholder">
								<trans ml="Hotel Results">Message</trans>
							</xsl:variable>

							<xsl:variable name="NamePlaceholder_JSSafe">
								<xsl:call-template name="Replace">
									<xsl:with-param name="text" select="$NamePlaceholder" />
									<xsl:with-param name="find" select='"&apos;"' />
									<xsl:with-param name="replacement" select='"\&apos;"' />
								</xsl:call-template>
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

							<input id='hidNamePlaceholder' type='hidden' value='{$NamePlaceholder_JSSafe}' />
							<input id='hidEmailPlaceholder' type='hidden' value='{$EmailPlaceholder_JSSafe}' />
							<input id='hidRecipientPlaceholder' type='hidden' value='{$RecipientPlaceholder_JSSafe}' />
							<input id='hidMessagePlaceholder' type='hidden' value='{$MessagePlaceholder_JSSafe}' />

							<input id="txtHotelPopup_EmailName" type="text" class="textbox" ml="Hotel Results" />
							<input id="txtHotelPopup_EmailYourAddress" type="text" class="textbox" ml="Hotel Results" />
							<input id="txtHotelPopup_EmailRecipientAddress" type="text" class="textbox" ml="Hotel Results" />
							<input id="txtHotelPopup_EmailMessage" type="text" class="textbox" ml="Hotel Results" />

							<!--add onfocus and onblur events for placeholder if in old browser-->

							<input type="button" class="button primary" value="Send Now" ml="Hotel Results" onclick="HotelPopup.EmailDetails({$PropertyReferenceID});" />
						</div>
					</div>

					<div class="right">

						<!-- Images -->
						<img src="{MainImage}" alt="MainImage" />
						<xsl:for-each select="Images/Image">
							<img src="{Image}" alt="Image" />
						</xsl:for-each>
					</div>
				</div>

				<!-- Rates Table -->
				<div id="divHotelPopup_{$PropertyReferenceID}">
					<xsl:variable name="roomcount" select="count(Hotel/Rooms/Room)" />
					<xsl:variable name="paxcount" select="sum(Hotel/Rooms/Room/Adults) + sum(Hotel/Rooms/Room/Children)" />
					<xsl:for-each select="Hotel/Rooms/Room">
						<xsl:call-template name="Rates">
							<xsl:with-param name="roomcount" select="$roomcount" />
							<xsl:with-param name="roomnumber" select="position()" />
							<xsl:with-param name="paxcount" select="$paxcount" />
						</xsl:call-template>
					</xsl:for-each>
					<xsl:if test="$roomcount > 1">
						<input type="button" class="button primary multiroom" value="Continue" onclick="HotelResults.SelectMultiRoom({$PropertyReferenceID});" ml="Hotel Results" />
					</xsl:if>
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>

	<!-- Rates -->
	<xsl:template name="Rates">
		<xsl:param name="roomcount" />
		<xsl:param name="roomnumber" />
		<xsl:param name="paxcount" />

		<xsl:variable name="flightprice">
			<xsl:choose>
				<xsl:when test="../../SelectedFlight/BookingToken != ''">
					<xsl:value-of select="../../SelectedFlight/Total" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>0</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="roomflightprice">
			<xsl:value-of select="($flightprice div $paxcount) * (Adults + Children)" />
		</xsl:variable>

		<table class="def striped hover">
			<tr>
				<th ml="Hotel Results">Room Type</th>
				<th ml="Hotel Results">Meal Basis</th>
				<th ml="Hotel Results">Total Price</th>
				<th class="book">
					<xsl:text> </xsl:text>
				</th>
			</tr>

			<xsl:for-each select="RoomOptions/RoomOption">
				<tr>
					<td>
						<xsl:value-of select="RoomType" />
						<xsl:if test="RoomView != ''">
							<xsl:value-of select="concat(' - ', RoomView)" />
						</xsl:if>
					</td>
					<td>
						<xsl:value-of select="MealBasis" />
					</td>
					<td>
						<strong>
							<xsl:call-template name="GetSellingPrice">
								<xsl:with-param name="Value" select="Price + $roomflightprice" />
								<xsl:with-param name="Format" select="'######'" />
								<xsl:with-param name="Currency" select="$CurrencySymbol" />
								<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
							</xsl:call-template>
						</strong>
					</td>

					<td class="right book">
						<xsl:variable name="propertyid" select="../../../../PropertyReferenceID" />
						<xsl:choose>
							<xsl:when test="$roomcount > 1">
								<input type="radio" class="radio" value="{Index}" name="rad_roomoption_{$propertyid}_{$roomnumber}" />
							</xsl:when>
							<xsl:otherwise>
								<input type="button" class="button primary small" value="Select" ml="Hotel Results" onclick="HotelResults.SelectRoom({$propertyid},'{Index}');" />
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>
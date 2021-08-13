<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:param name="LoginType" />
	<xsl:param name="MMBURL" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="ShowReminderLink" />
	<xsl:param name="InsertWarningAfter" />

	<xsl:template match="/">

		<div id="divBookingLogin" class="sidebarBox primary">
			<xsl:if test="$CSSClassOverride != ''">
				<xsl:attribute name="class">
					<xsl:value-of select="$CSSClassOverride" />
				</xsl:attribute>
			</xsl:if>

			<input type="hidden" id="hidInsertWarningAfter" value="{$InsertWarningAfter}" />

			<div class="boxTitle">
				<h2 ml="Bookings Login">Customer Login</h2>
			</div>

			<!-- login with username and password -->
			<div class="loginDetails">
				<xsl:choose>

					<xsl:when test="$LoginType = 'BookingDetails'">
						<div>
							<label class="textboxlabel" ml="Bookings Login">Booking Reference</label>
							<input type="text" id="txtBookingRef" class="textbox" />
						</div>
						<div>
							<label class="textboxlabel" ml="Bookings Login">Last Name</label>
							<input type="text" id="txtLastName" class="textbox" />
						</div>
						<div>
							<label class="textboxlabel" ml="Bookings Login">Departure Date</label>
							<div class="textbox icon calendar right embedded">
								<i>
									<xsl:text> </xsl:text>
								</i>
								<input id="txtDepartureDate" name="txtDepartureDate" type="text" class="textbox" />
							</div>
						</div>
					</xsl:when>
					<xsl:when test="$LoginType = 'EmailAndReference'">
						<div>
							<label class="textboxlabel" ml="Bookings Login">Email Address</label>
							<input type="text" id="txtEmailAddress" class="textbox" />
						</div>
						<div>
							<label class="textboxlabel" ml="Bookings Login">Booking Reference</label>
							<input type="text" id="txtBookingRef" class="textbox" />
						</div>
					</xsl:when>
					<xsl:otherwise>
						<div>
							<label class="textboxlabel" ml="Bookings Login">Email Address</label>
							<input type="text" id="txtEmailAddress" class="textbox" />
						</div>
						<div>
							<label class="textboxlabel" ml="Bookings Login">Password</label>
							<input type="password" id="txtPassword" class="textbox" />
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</div>

			<div id="divLoginContinue">
				<a href="#" class="button primary" onclick="MyBookingsLogin.Validate('{$LoginType}');return false;" ml="Bookings Login">Continue</a>

				<!--Reminder Link-->
				<xsl:if test="$ShowReminderLink != 'False'">
					<div id="divReminder">
						<a id="aReminder"  ml="Bookings Login" onclick="MyBookingsLogin.SendReminder();return false;">I do not remember or cannot find my Booking reference.</a>
					</div>
				</xsl:if>
			</div>

			<div style="clear:both">
				<xsl:text> </xsl:text>
			</div>
		</div>

		<div style="clear:both;">
			<xsl:text> </xsl:text>
		</div>

		<script type="text/javascript">
			int.ll.OnLoad.Run(function () { MyBookingsLogin.Setup('<xsl:value-of select ="$MMBURL" />'); });
		</script>
	</xsl:template>
</xsl:stylesheet>
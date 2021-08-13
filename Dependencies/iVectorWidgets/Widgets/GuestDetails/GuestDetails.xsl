<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="AdultYears"/>
	<xsl:param name="ChildYears"/>
	<xsl:param name="InfantYears"/>
	<xsl:param name="RequiresDOB" select="'true'"/>
	<xsl:param name="ShowCopyLastName" select="'false'" />
	<xsl:param name="FirstDepartureDate" />
	<xsl:param name="RequiresInfantDOB" select="'false'"/>
	<xsl:param name="Title" />
	<xsl:param name="DOBWarning" />
	<xsl:param name="MissingFieldsWarning" />
	
	<xsl:template match="/">
		
		<input type="hidden" id="hidGuestDetails_MissingFields" value="{$MissingFieldsWarning}" />
		<input type="hidden" id="hidGuestDetails_DOBWarning" value="{$DOBWarning}" />
		<input type="hidden" id="hidGuestDetails_RequiresDOB" value="{$RequiresDOB}" />
		<input type="hidden" id="hidGuestDetails_RequiresInfantDOB" value="{$RequiresInfantDOB}" />
		<input type="hidden" id="hidGuestDetails_FirstDepartureDate" value="{$FirstDepartureDate}" />
		
		<div id="divGuestDetails" class="box primary form">

			<div class="boxTitle">
				<h2>
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
							<xsl:text> </xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Guest Details">Guest Details</trans>
						</xsl:otherwise>
					</xsl:choose>
					<span ml="Guest Details">*Required Fields</span>
				</h2>
			</div>

			<input type="hidden" id="hidGuestDetails_RoomCount" value="{count(/BookingSearch/RoomGuests/Guest)}" />
			<input type="hidden" id="hisGuestDetails_FirstNamePlaceholder" value="First Name" ml="Guest Details" />
			<input type="hidden" id="hisGuestDetails_LastNamePlaceholder" value="Last Name" ml="Guest Details" />

			<xsl:for-each select="/BookingSearch/RoomGuests/Guest">

				<xsl:variable name="RoomNumber" select="position()"/>

				<div id="divGuestDetails_Room{$RoomNumber}">

					<xsl:if test="count(/BookingSearch/RoomGuests/Guest) &gt; 1">
						<h3 ml="Guest Details" mlparams="{$RoomNumber}">
							Room {0}
						</h3>
					</xsl:if>

					<input type="hidden" id="hidGuestDetails_PaxCount_{$RoomNumber}" value="{Adults + Children + Infants}" />

					<table class="def">
						<!--Add classes to table so we can style widths etc depending on how many columns we have-->
						<xsl:choose>
							<xsl:when test="($RequiresDOB = 'true' or $RequiresInfantDOB = 'true') and (Children + Infants &gt; 0)">
								<xsl:attribute name="class">def requiresDOB requiresAge</xsl:attribute>
							</xsl:when>
							<xsl:when test="$RequiresDOB = 'true' or $RequiresInfantDOB = 'true'">
								<xsl:attribute name="class">def requiresDOB</xsl:attribute>
							</xsl:when>
							<xsl:when test="Children + Infants &gt; 0">
								<xsl:attribute name="class">def requiresAge</xsl:attribute>
							</xsl:when>
						</xsl:choose>
						<tr>
							<th ml="Guest Details">Title</th>
							<th ml="Guest Details">First Name</th>
							<th ml="Guest Details">
								<xsl:if test="$RequiresDOB = 'false' and ($RequiresInfantDOB = 'false' and Infants = 0) and Children + Infants = 0">
									<xsl:attribute name="class">end</xsl:attribute>
								</xsl:if>
								<xsl:text>Last Name</xsl:text>
							</th>
							<xsl:if test="$RequiresDOB = 'true' or ($RequiresInfantDOB = 'true' and Infants &gt; 0)">
								<th ml="Guest Details">
									<xsl:if test="Children + Infants = 0">
										<xsl:attribute name="class">end</xsl:attribute>
									</xsl:if>
									<xsl:text>Date of Birth</xsl:text>
								</th>
							</xsl:if>
							<xsl:if test="Children + Infants &gt; 0">
								<th ml="Guest Details" class="end">Age</th>
							</xsl:if>
						</tr>

						<!-- Adults -->
						<xsl:call-template name="Passenger">
							<xsl:with-param name="Type" select="'Adult'" />
							<xsl:with-param name="Count" select="Adults" />
							<xsl:with-param name="RoomNumber" select="$RoomNumber" />
							<xsl:with-param name="IncludeDOB" select="$RequiresDOB" />
							<xsl:with-param name="ChildPlusInfantCount" select="Infants + Children" />
						</xsl:call-template>
						<!-- Children -->
						<xsl:call-template name="Passenger">
							<xsl:with-param name="Type" select="'Child'" />
							<xsl:with-param name="Count" select="Children" />
							<xsl:with-param name="GuestNumber" select="Adults + 1" />
							<xsl:with-param name="RoomNumber" select="$RoomNumber" />
							<xsl:with-param name="IncludeDOB" select="$RequiresDOB" />
							<xsl:with-param name="ChildPlusInfantCount" select="Infants + Children" />
						</xsl:call-template>
						<!-- Infants -->
						<xsl:call-template name="Passenger">
							<xsl:with-param name="Type" select="'Infant'" />
							<xsl:with-param name="Count" select="Infants" />
							<xsl:with-param name="GuestNumber" select="Adults + Children + 1" />
							<xsl:with-param name="RoomNumber" select="$RoomNumber" />
							<xsl:with-param name="IncludeDOB">
								<xsl:choose>
									<xsl:when test="$RequiresDOB = 'true' or $RequiresInfantDOB = 'true'">true</xsl:when>
									<xsl:otherwise>false</xsl:otherwise>
								</xsl:choose>
							</xsl:with-param>
							<xsl:with-param name="ChildPlusInfantCount" select="Infants + Children" />
						</xsl:call-template>
					</table>
				</div>
			</xsl:for-each>

		</div>
		
	</xsl:template>


	<xsl:template name="Passenger">
		<xsl:param name="Type" />
		<xsl:param name="Count" />
		<xsl:param name="RoomNumber" select="1" />
		<xsl:param name="GuestNumber" select="1" />
		<xsl:param name="IncludeDOB" />
		<xsl:param name="ChildPlusInfantCount" select="0" />

		<xsl:if test="$Count &gt; 0">
			<tr>
				<td>
					<input type="hidden" id="hidGuestDetailsType_{$RoomNumber}_{$GuestNumber}" value="{$Type}" />
					<select id="ddlGuestDetailsTitle_{$RoomNumber}_{$GuestNumber}" ml="GuestDetails;Title" class="title">
						<xsl:if test="$Type = 'Adult'">
							<option value="Mr">Mr</option>
							<option value="Mrs">Mrs</option>
						</xsl:if>
						<xsl:if test="$Type = 'Child' or $Type = 'Infant'">
							<option value="Master">Master</option>
						</xsl:if>
						<option value="Miss">Miss</option>
						<xsl:if test="$Type = 'Adult'">
							<option value="Ms">Ms</option>
						</xsl:if>
					</select>
				</td>
				<td>
					<input type="text" id="txtGuestDetailsFirstName_{$RoomNumber}_{$GuestNumber}" class="textbox small firstName" onBlur="GuestDetails.CapitaliseFirstLetter(this)"/>
				</td>
				<td>
					<xsl:if test="$IncludeDOB = 'false' and $ChildPlusInfantCount = 0">
						<xsl:attribute name="class">end</xsl:attribute>
					</xsl:if>
					<!--Uneeded markup, but if we want to absolute position this copy element, it will need to be in a div to work in FF.-->
					<div>
						<input type="text" id="txtGuestDetailsLastName_{$RoomNumber}_{$GuestNumber}" class="textbox small lastName" onBlur="GuestDetails.CapitaliseFirstLetter(this)"/>
						<xsl:if test="$GuestNumber != 1">
							<span class="lastNameCopy" onclick="GuestDetails.CopyLastName({$RoomNumber}, {$GuestNumber});return false;">Copy</span>
						</xsl:if>
					</div>
				</td>
				<xsl:choose>
					<xsl:when test="$IncludeDOB = 'true'">
						<td class="dob">
							<xsl:if test="Children + Infants = 0">
								<xsl:attribute name="class">dob end</xsl:attribute>
							</xsl:if>
							<span class="dob" id="spanDOB_{$RoomNumber}_{$GuestNumber}">
								<xsl:attribute name="class">
									<xsl:choose>
										<xsl:when test="$Type='Child'">childDOB dob</xsl:when>
										<xsl:when test="$Type='Infant'">infantDOB dob</xsl:when>
										<xsl:otherwise>dob</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<select id="ddlDOBDay_{$RoomNumber}_{$GuestNumber}" name="ddlDOBDay_{$RoomNumber}_{$GuestNumber}">
									<xsl:call-template name="DrawOptions">
										<xsl:with-param name="Options" select="'#1#2#3#4#5#6#7#8#9#10#11#12#13#14#15#16#17#18#19#20#21#22#23#24#25#26#27#28#29#30#31#'"/>
										<xsl:with-param name="Selected" select="substring(DateOfBirth,9,2)"/>
									</xsl:call-template>
								</select>
								<select id="ddlDOBMonth_{$RoomNumber}_{$GuestNumber}" name="ddlDOBMonth_{$RoomNumber}_{$GuestNumber}" ml="Guest Details;Month">
									<xsl:call-template name="DrawOptions">
										<xsl:with-param name="Options" select="'#Jan|1#Feb|2#Mar|3#Apr|4#May|5#Jun|6#Jul|7#Aug|8#Sep|9#Oct|10#Nov|11#Dec|12#'"/>
									</xsl:call-template>
								</select>
								<select id="ddlDOBYear_{$RoomNumber}_{$GuestNumber}" name="ddlDOBYear_{$RoomNumber}_{$GuestNumber}">
									<xsl:variable name="Options">
										<xsl:if test="$Type='Adult'">
											<xsl:value-of select="$AdultYears"/>
										</xsl:if>
										<xsl:if test="$Type='Child'">
											<xsl:value-of select="$ChildYears"/>
										</xsl:if>
										<xsl:if test="$Type='Infant'">
											<xsl:value-of select="$InfantYears"/>
										</xsl:if>
									</xsl:variable>
									<xsl:call-template name="DrawOptions">
										<xsl:with-param name="Options" select="$Options"/>
										<xsl:with-param name="Selected" select="substring(DateOfBirth,1,4)"/>
									</xsl:call-template>
								</select>
							</span>
						</td>
					</xsl:when>
					<!--If we're supressing the date of birth, but we have infants we still need a blank cell for layout-->
					<xsl:when test="$ChildPlusInfantCount &gt; 0 and $RequiresDOB = 'true'">
						<td>
							<xsl:text> </xsl:text>
						</td>
					</xsl:when>
				</xsl:choose>
				<xsl:if test="$Type!='Adult' or $ChildPlusInfantCount &gt; 0 ">
					<td class="age end">
						<xsl:if test="$Type='Adult'">
							<p class="guestAge">
								<xsl:text>-</xsl:text>
							</p>
						</xsl:if>
						<xsl:if test="$Type='Child'">
							<p class="guestAge">
								<xsl:value-of select="ChildAges/int[position() = count(../int) - $Count +1]"/>
							</p>
							<input type="hidden" id="hidChildAge_{$RoomNumber}_{$GuestNumber}" name="hidChildAge_{$RoomNumber}_{$GuestNumber}" value="ChildAges/int[position() = count(../int) - $Count +1]" />
						</xsl:if>
						<xsl:if test="$Type='Infant'">
							<p class="guestAge" ml="GuestDetails">
								<xsl:text>infant</xsl:text>
							</p>
						</xsl:if>
					</td>
				</xsl:if>
			</tr>
			<!-- Recall template to draw next passenger -->
			<xsl:variable name="GuestsRemaining" select="$Count - 1"/>
			<xsl:variable name="NextGuest" select="$GuestNumber + 1"/>
			<xsl:call-template name="Passenger">
				<xsl:with-param name="Count" select="$GuestsRemaining" />
				<xsl:with-param name="Type" select="$Type" />
				<xsl:with-param name="RoomNumber" select="$RoomNumber" />
				<xsl:with-param name="GuestNumber" select="$NextGuest" />
				<xsl:with-param name="IncludeDOB" select="$IncludeDOB" />
				<xsl:with-param name="ChildPlusInfantCount" select="$ChildPlusInfantCount" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>


	<!-- Draw Dropdown Options from '#' delimited string-->
	<xsl:template name="DrawOptions">
		<xsl:param name="Options"/>
		<xsl:param name="Selected"/>

		<xsl:variable name="Option" select="substring-before($Options,'#')"/>
		
		<xsl:variable name="Remainder" select="substring-after($Options,'#')"/>

		<xsl:variable name="DisplayValue">
			<xsl:choose>
				<xsl:when test="contains($Option, '|')">
					<xsl:value-of select="substring-before($Option,'|')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$Option"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="Value">
			<xsl:choose>
				<xsl:when test="contains($Option, '|')">
					<xsl:value-of select="substring-after($Option,'|')" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$Option"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<option value="{$Value}">
			<xsl:if test="$Selected=$Value">
				<xsl:attribute name="selected">selected</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$DisplayValue"/>
		</option>

		<xsl:if test="$Remainder!=''">
			<xsl:call-template name="DrawOptions">
				<xsl:with-param name="Options" select="$Remainder"/>
				<xsl:with-param name="Selected" select="$Selected"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	
	<!-- Convert 2 digit numeric month to short month name -->
	<xsl:template name="ConvertMMToMMM">
		<xsl:param name="Month"/>
		<xsl:param name="Year"/>

		<xsl:if test="not(starts-with($Year, '00'))">
			<xsl:choose>
				<xsl:when test="$Month='01'">
					<xsl:text>Jan</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='02'">
					<xsl:text>Feb</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='03'">
					<xsl:text>Mar</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='04'">
					<xsl:text>Apr</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='05'">
					<xsl:text>May</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='06'">
					<xsl:text>Jun</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='07'">
					<xsl:text>Jul</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='08'">
					<xsl:text>Aug</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='09'">
					<xsl:text>Sep</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='10'">
					<xsl:text>Oct</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='11'">
					<xsl:text>Nov</xsl:text>
				</xsl:when>
				<xsl:when test="$Month='12'">
					<xsl:text>Dec</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text> </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>

		<xsl:if test="starts-with($Year, '00')">
			<xsl:text> </xsl:text>
		</xsl:if>

	</xsl:template>

	
</xsl:stylesheet>
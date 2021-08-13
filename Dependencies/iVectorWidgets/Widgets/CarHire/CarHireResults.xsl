<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>

	<xsl:param name="Title" />
	<xsl:param name="Theme" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="SearchMode" />
	<xsl:param name="AutomaticSearchType" />
	<xsl:param name="PriceFormat" />
	

	<xsl:key name="type" match="/CarHireResults/CarHires/CarHire/Type/text()" use="." />

	<xsl:template match="/">

		<table id="tblResults" class="def carHireResults">
			<tr>
				<th class="col1" colspan="2" ml="Car Hire">Car hire options</th>
				<th>Total Price</th>
				<th>Select</th>
			</tr>

			<xsl:for-each select="/CarHireResults/CarHires/CarHire/Type/text()[generate-id() = generate-id(key('type', .)[1])]">
				<xsl:call-template name="CarHireOption" />
			</xsl:for-each>

			<xsl:for-each select="/CarHireResults/CarHires/CarHire/Type/text()[generate-id() != generate-id(key('type', .)[1])]">
				<xsl:call-template name="CarHireOption">
					<xsl:with-param name="option" select="'true'" />
				</xsl:call-template>
			</xsl:for-each>

		</table>

		<xsl:variable name="distinctTypes" select="count(//CarHireResults/CarHires/CarHire[not(Type=preceding-sibling::CarHire/Type)]/Type)"/>

		<input id="btnAddCar" type="button" class="button primary" value="Add car" />
		
		<xsl:if test="count(CarHireResults/CarHires/CarHire) > $distinctTypes">
			<a id="aShowExtraOptions" class="options" href="#" onclick="CarHire.ShowMoreOptions();return false;" ml="Car Hire">More car options</a>
			<a id="aHideExtraOptions" class="options" href="#" onclick="CarHire.HideMoreOptions();return false;" style="display:none;" ml="Car Hire">Hide car options</a>
		</xsl:if>

		<div id="divRentalConditionsWaiting" style="display:none;">
			<img id="imgLoading" alt="" src="/themes/{$Theme}/images/loading.gif"/>
		</div>

	</xsl:template>

	<xsl:template name="CarHireOption">
		<xsl:param name="option" />

		<tr class="mobileShow">
			<xsl:if test="$option = 'true'">
				<xsl:attribute name="class">option mobileShow</xsl:attribute>
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>
			<td colspan="4">
				<h4 class="carName">
					<xsl:if test="../../VehicleCode != ''">
						<xsl:value-of select="../../VehicleCode" />
						<xsl:text> - </xsl:text>
					</xsl:if>
					<xsl:value-of select="../../VehicleDescription"/>
				</h4>
			</td>
		</tr>
		
		<tr class="carResultRow">
			<xsl:if test="$option = 'true'">
				<xsl:attribute name="class">carResultRow option</xsl:attribute>
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>

			<!-- car image -->
			<td class="col1">
				<img src="{../../ImageURL}" />
			</td>

			<!-- car info -->
			<td class="col2">
				<div class="singleCar">
					
					<h4 class="carName mobileHide">
						<xsl:if test="../../VehicleCode != ''">
							<xsl:value-of select="../../VehicleCode" />
							<xsl:text> - </xsl:text>
						</xsl:if>
						<xsl:value-of select="../../VehicleDescription"/>
					</h4>
					<xsl:text> </xsl:text>

					<span class="carType mobileHide ">
						<xsl:value-of select="../../Type"/>
					</span>

					<span class="mobileHide featureIcon doors">
						<xsl:text> </xsl:text>
					</span>
					<label class="mobileHide">
						<xsl:value-of select="../../NumberOfDoors"/>
					</label>

					<xsl:choose>
						<xsl:when test="../../ChildCapacity &gt; 0">
							<span class="mobileHide  featureIcon paxMultiple">
								<xsl:text> </xsl:text>
							</span>
							<label class="mobileHide">
								<xsl:value-of select="concat(../../AdultCapacity, ' + ', ../../ChildCapacity)"/>
							</label>
						</xsl:when>
						<xsl:when test="../../AdultCapacity &gt; 0">
							<span class="mobileHide featureIcon pax">
								<xsl:text> </xsl:text>
							</span>
							<label class="mobileHide">
								<xsl:value-of select="../../AdultCapacity"/>
							</label>
						</xsl:when>
						<xsl:otherwise>
							<span class="mobileHide featureIcon pax">
								<xsl:text> </xsl:text>
							</span>
							<label class="mobileHide">
								<xsl:value-of select="../../PaxCapacity"/>
							</label>
						</xsl:otherwise>
					</xsl:choose>

					<xsl:choose>
						<xsl:when test="../../SmallBaggageCapacity &gt; 0">
							<span class="mobileHide featureIcon baggageMultiple">
								<xsl:text> </xsl:text>
							</span>
							<label class="mobileHide">
								<xsl:value-of select="concat(../../LargeBaggageCapacity, ' + ', ../../SmallBaggageCapacity)"/>
							</label>
						</xsl:when>
						<xsl:otherwise>
							<span class="mobileHide featureIcon baggage">
								<xsl:text> </xsl:text>
							</span>
							<label class="mobileHide">
								<xsl:value-of select="../../LargeBaggageCapacity"/>
							</label>
						</xsl:otherwise>
					</xsl:choose>

					<span class="mobileHide featureIcon transmission">
						<xsl:text> </xsl:text>
					</span>
					<label class="mobileHide">
						<xsl:choose>
							<xsl:when test="../../TransmissionType = 'Manual'">
								<xsl:value-of select="'M'"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="'A'"/>
							</xsl:otherwise>
						</xsl:choose>
					</label>

					<xsl:if test="../../AirConditioning = 'true'">
						<span class="mobileHide featureIcon airCon">
							<xsl:text> </xsl:text>
						</span>
					</xsl:if>


				</div>
			</td>

			<!-- price -->
			<td class="col3">
				<p class="price">
					<xsl:value-of select="$CurrencySymbol" />
					<xsl:value-of select="format-number(../../Price, $PriceFormat)"/>
				</p>
				<!--<p class="rateServiceType">
					<xsl:value-of select="../../RateServiceType" />
					<xsl:text> </xsl:text>
					<a class="info" onclick="CarHire.ShowInfo('{../../BookingToken}');return false;" href="#">
						<xsl:text> info </xsl:text>
					</a>
				</p>-->
			</td>

			<!-- select -->
			<td class="col4">
				<!--<input type="checkbox" class="checkbox" id="chk_ch_{../../CarHireOptionHashToken}" onclick="int.f.ToggleClass(this.parentNode, 'selected');CarHire.CheckboxSelect(this);" />-->
				<input type="radio" class="" name="cahire" id="rad_ch_{../../CarHireOptionHashToken}" />

				<!--<input type="button" class="button primary small" ml="CarHire" value="Select"/>
				<span class="selected" ml="Transfers">Selected</span>-->
			</td>

		</tr>

		
		<!-- feature view mobile size -->	
		<tr class="mobileShow features">
			<xsl:if test="$option = 'true'">
				<xsl:attribute name="class">option mobileShow features</xsl:attribute>
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>
			<td colspan="4">

				<xsl:if test="../../NumberOfDoors &gt; 0">
					<span class="featureIcon doors">
						<xsl:text> </xsl:text>
					</span>
					<label>
						<xsl:value-of select="../../NumberOfDoors"/>
					</label>
				</xsl:if>

				<xsl:choose>
					<xsl:when test="../../ChildCapacity &gt; 0">
						<span class="featureIcon paxMultiple">
							<xsl:text> </xsl:text>
						</span>
						<label>
							<xsl:value-of select="concat(../../AdultCapacity, ' + ', ../../ChildCapacity)"/>
						</label>
					</xsl:when>
					<xsl:when test="../../AdultCapacity &gt; 0">
						<span class="featureIcon pax">
							<xsl:text> </xsl:text>
						</span>
						<label>
							<xsl:value-of select="../../AdultCapacity"/>
						</label>
					</xsl:when>
					<xsl:otherwise>
						<span class="featureIcon pax">
							<xsl:text> </xsl:text>
						</span>
						<label>
							<xsl:value-of select="../../PaxCapacity"/>
						</label>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:choose>
					<xsl:when test="../../SmallBaggageCapacity &gt; 0">
						<span class="featureIcon baggageMultiple">
							<xsl:text> </xsl:text>
						</span>
						<label>
							<xsl:value-of select="concat(../../LargeBaggageCapacity, ' + ', ../../SmallBaggageCapacity)"/>
						</label>
					</xsl:when>
					<xsl:otherwise>
						<span class="featureIcon baggage">
							<xsl:text> </xsl:text>
						</span>
						<label>
							<xsl:value-of select="../../LargeBaggageCapacity"/>
						</label>
					</xsl:otherwise>
				</xsl:choose>

				<span class="featureIcon transmission">
					<xsl:text> </xsl:text>
				</span>
				<label>
					<xsl:choose>
						<xsl:when test="../../TransmissionType = 'Manual'">
							<xsl:value-of select="'M'"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="'A'"/>
						</xsl:otherwise>
					</xsl:choose>
				</label>

				<xsl:if test="../../AirConditioning = 'true'">
					<span class="featureIcon airCon">
						<xsl:text> </xsl:text>
					</span>
				</xsl:if>
				
			</td>
						
		</tr>
		
	</xsl:template>

</xsl:stylesheet>

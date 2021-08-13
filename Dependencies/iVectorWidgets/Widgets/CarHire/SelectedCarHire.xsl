<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	<xsl:include href="../../xsl/functions.xsl"/>


	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="ListClass" />
	<xsl:param name="ListItemClass" />


	<xsl:template match="/BasketCarHire">

		<xsl:for-each select="CarHire">
			<h3>Your selected car hire</h3>

			<table class="def carHireResults">
				<tr>
					<th class="col1" colspan="2" ml="Car Hire">Car hire option</th>
					<th>Total Price</th>
				</tr>

				<tr class="singleCar">

					<td class="col1">
						<img src="{ImageURL}" alt="car hire image" />
					</td>
					<td class="col2">
						<h4 class="carName mobileHide">
							<xsl:value-of select="VehicleCode" />
							<xsl:text> - </xsl:text>
							<xsl:value-of select="VehicleDescription"/>
						</h4>
						<xsl:text> </xsl:text>

						<span class="carType mobileHide ">
							<xsl:value-of select="Type"/>
						</span>

						<span class="mobileHide featureIcon doors">
							<xsl:text> </xsl:text>
						</span>
						<label class="mobileHide">
							<xsl:value-of select="NumberOfDoors"/>
						</label>

						<xsl:choose>
							<xsl:when test="../../ChildCapacity &gt; 0">
								<span class="mobileHide  featureIcon paxMultiple">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="concat(AdultCapacity, ' + ', ChildCapacity)"/>
								</label>
							</xsl:when>
							<xsl:when test="AdultCapacity &gt; 0">
								<span class="mobileHide featureIcon pax">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="AdultCapacity"/>
								</label>
							</xsl:when>
							<xsl:otherwise>
								<span class="mobileHide featureIcon pax">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="PaxCapacity"/>
								</label>
							</xsl:otherwise>
						</xsl:choose>

						<xsl:choose>
							<xsl:when test="SmallBaggageCapacity &gt; 0">
								<span class="mobileHide featureIcon baggageMultiple">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="concat(LargeBaggageCapacity, ' + ', SmallBaggageCapacity)"/>
								</label>
							</xsl:when>
							<xsl:when test="LargeBaggageCapacity &gt; 0">
								<span class="mobileHide featureIcon baggage">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="LargeBaggageCapacity"/>
								</label>
							</xsl:when>
							<xsl:otherwise>
								<span class="mobileHide featureIcon baggage">
									<xsl:text> </xsl:text>
								</span>
								<label class="mobileHide">
									<xsl:value-of select="BaggageCapacity"/>
								</label>
							</xsl:otherwise>
						</xsl:choose>

						<span class="mobileHide featureIcon transmission">
							<xsl:text> </xsl:text>
						</span>
						<label class="mobileHide">
							<xsl:choose>
								<xsl:when test="TransmissionType = 'Manual'">
									<xsl:value-of select="'M'"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="'A'"/>
								</xsl:otherwise>
							</xsl:choose>
						</label>

						<xsl:if test="AirConditioning = 'true'">
							<span class="mobileHide featureIcon airCon">
								<xsl:text> </xsl:text>
							</span>
						</xsl:if>
					</td>
					<td class="col3">
						<p class="price">
							<xsl:value-of select="$CurrencySymbol" />
							<xsl:value-of select="format-number(Price, '###,##0.00')"/>
						</p>
					</td>
				
				</tr>
			
			</table>

			<!--<h4>Select extra car hire options</h4>-->
	
			<div class="includedExtras">
				<h4>Included in price</h4>
				<ul class="{$ListClass}">
					<xsl:for-each select="CarHireExtras/CarHireExtraOption[IncludedInPrice = 'true']">
						<li>
							<i class="{$ListItemClass}">
								<xsl:text> </xsl:text>
							</i>
							<p>
								<xsl:value-of select="Description"/>
							</p>
						</li>
					</xsl:for-each>
				</ul>
			</div>

			<div class="bookableExtras clear">
				<h4>Car hire extras</h4>
				<xsl:for-each select="CarHireExtras/CarHireExtraOption[IncludedInPrice = 'false']">
					<div>
						<span>
							<p>
								<xsl:value-of select="Description"/>
							</p>
							<strong class="price">
								<xsl:value-of select="$CurrencySymbol" />
								<xsl:value-of select="format-number(Price, '###,##0.00')"/>
								<xsl:text> per rental</xsl:text>
							</strong>
						</span>
												
						<select id="ddl_ch_extra_{ExtraBookingToken}">
							<xsl:call-template name="options">
								<xsl:with-param name="number" select="0" />
							</xsl:call-template>
						</select>

					</div>
				</xsl:for-each>
				
			</div>

			<input id="btnRemoveCarHire" type="button" class="button primary" value="Remove car hire" />

			<input id="btnAddExtras" type="button" class="button primary" value="Add extras" />

		</xsl:for-each>
	</xsl:template>

	<xsl:template name="options">
		<xsl:param name="number" />

		<option value="{$number}">
			<xsl:if test="$number = Quantity">
				<xsl:attribute name="selected">
					selected
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$number"/>
		</option>
		<xsl:if test="$number &lt;5">
			<xsl:call-template name="options">
				<xsl:with-param name="number" select="$number + 1"/>
			</xsl:call-template>
		</xsl:if>
		
	</xsl:template>

</xsl:stylesheet>

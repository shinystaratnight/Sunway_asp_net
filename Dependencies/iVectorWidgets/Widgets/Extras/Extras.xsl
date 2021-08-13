<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	<xsl:include href="../../xsl/functions.xsl "/>
	<xsl:param name="Title" />
	<xsl:param name="DisplayType" />
	<xsl:param name="CurrentBasketExtraCode" />
	<xsl:param name="JustResults" />
	<xsl:param name="Identifier" />
	<xsl:param name="ExtraCount" />
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="SubText" />

	<xsl:template match="/">

		<xsl:if test="Extras/Extra/Extra/Options/Option/BookingToken">
			<xsl:choose>
				<xsl:when test="$DisplayType = 'SimpleRadio' and $JustResults= 'True'">
					<xsl:call-template name="SimpleRadio" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="Details" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	
	<xsl:template name="Details">		


		<div class="box primary clear">
			<xsl:attribute name="id">
				<xsl:text>div</xsl:text>
				<xsl:choose>
					<xsl:when test="$Title != ''">
						<xsl:value-of select="$Title"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Extras</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<xsl:attribute name ="class">
				<xsl:choose>
					<xsl:when test="$CSSClassOverride != ''">
						<xsl:value-of select="$CSSClassOverride" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>box primary clear</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<div class="boxTitle">
				<h2 ml="Transfers">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Extras</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
				<xsl:if test="$SubText != ''">
					<p>
						<xsl:value-of select="$SubText"/>
					</p>
				</xsl:if>
			</div>

			<div>
				<xsl:attribute name="id">
					<xsl:text>div</xsl:text>
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Extras</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>Results</xsl:text>
				</xsl:attribute>
				

				<xsl:choose>
					<xsl:when test="$DisplayType = 'SimpleRadio'">
						<xsl:call-template name="SimpleRadio" />
					</xsl:when>
				</xsl:choose>

			</div>

		</div>

	</xsl:template>



	<xsl:template name="SimpleCheckBox" mode="SimpleCheckBox" match="results">

	</xsl:template>

	<xsl:template name="SimpleRadio" mode="SimpleRadio" match="results">

		<table class="def">

			<tr class="head">
				<th ml="Extras">Option</th>
				<th class="price" ml="Extras">Total Price</th>
				<th>
					<xsl:text> </xsl:text>
				</th>
			</tr>

			<tr>
				<xsl:attribute name="id">
					<xsl:text>tr_Extras_</xsl:text>
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Extras</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>_0</xsl:text>
				</xsl:attribute>

				<xsl:attribute name="class">
					<xsl:text>extraOption</xsl:text>
					<xsl:if test="not($ExtraCount &gt;'0')">
						<xsl:text> selected</xsl:text>
					</xsl:if>
				</xsl:attribute>

				
				<xsl:if test="$ExtraCount &gt;'0'">
					<xsl:attribute name="onclick">
						<xsl:text>Extras.RemoveExtraByType('</xsl:text>
						<xsl:value-of select="$Identifier"/>
						<xsl:text>');</xsl:text>
					</xsl:attribute >
				</xsl:if>
					
				
				<td ml="Transfers" mlparams="{$Identifier}">No {0}</td>
				<td class="price">
					<xsl:call-template name="GetSellingPrice">
						<xsl:with-param name="Value" select="'0'" />
						<xsl:with-param name="Format" select="'#####0'" />
						<xsl:with-param name="Currency" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
					</xsl:call-template>
				</td>
				<td class="end">
					<input type="button" class="button primary small" id="btn_Extras_0" name="btn_Extras" value="Select" onclick="Extras.RemoveExtraByType('{$Identifier}');" ml="Transfers">

						<xsl:if test="not($ExtraCount &gt; '0')">
							<xsl:attribute name="class">
								<xsl:text>button primary small selected</xsl:text>
							</xsl:attribute>
							<xsl:attribute name="onclick">
								<xsl:text> </xsl:text>
							</xsl:attribute>
							<xsl:attribute name="value">
								<xsl:text>Selected</xsl:text>
							</xsl:attribute>
						</xsl:if>

					</input>
				</td>
			</tr>

			<xsl:for-each select="Extras/Extra/Extra">
				<tr>
					<xsl:variable name="BookingToken" select="Options/Option/BookingToken"/>

					<xsl:variable name="InBasket">

						<xsl:for-each select="../../BookingBasket/BasketExtras/BasketExtra/BasketExtraOptions/BasketExtraOption">
							<xsl:if test="BookingToken = $BookingToken">
								<xsl:text>True</xsl:text>
							</xsl:if>
						</xsl:for-each>

					</xsl:variable>

					<xsl:attribute name="class">
						<xsl:if test="$InBasket = 'True'">
							<xsl:text>selected</xsl:text>
						</xsl:if>
					</xsl:attribute>

					<xsl:if test="$InBasket != 'True'">
						<xsl:attribute name="onclick">
							<xsl:text>Extras.AddToBasket('</xsl:text>
							<xsl:value-of select="Options/Option/BookingToken"/>
							<xsl:text>', '</xsl:text>
							<xsl:value-of select="$Identifier"/>
							<xsl:text>');return false;</xsl:text>
						</xsl:attribute>
					</xsl:if>

					<td>
						<xsl:value-of select="ExtraName"/>
					</td>

					<td class="price">
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="Options/Option/TotalPrice" />
							<xsl:with-param name="Format" select="'######.00'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						</xsl:call-template>
					</td>

					<td>
						<xsl:choose>
							<xsl:when test="$InBasket = 'True'">
								<input type="button" class="button primary small selected" name="btn_Extras" value="Selected" ml="Transfers" />
							</xsl:when>
							<xsl:otherwise>
								<a class="button" href="#">
									<xsl:text> select</xsl:text>
								</a>
							</xsl:otherwise>
						</xsl:choose>						
					</td>

					<!--</xsl:for-each>-->
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>



</xsl:stylesheet>

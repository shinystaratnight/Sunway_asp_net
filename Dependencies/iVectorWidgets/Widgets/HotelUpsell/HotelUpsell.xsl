<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="SearchMode" />
	<xsl:param name="Format" />

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="PriceFormat" />
	<xsl:param name="FlightPrice" />
	<xsl:param name="HotelsToDisplay" />
	<xsl:param name="JustResults" select="false" />

	<xsl:param name="PerPersonPrice" select="False" />
	<xsl:param name="PackagePrice" select="false" />

	<xsl:param name="BestSellerImagePath" select="false" />
	<xsl:param name="BestSellerPosition" select="false" />
	<xsl:param name="SummaryLength" select="100" />


	<xsl:param name="HotelArrivalDate" />
	<xsl:param name="HotelDuration" />

	<xsl:param name="Title" />
	<xsl:param name="SubTitle" />

	<xsl:include href="../../xsl/markdown.xsl "/>
	<xsl:include href="../../xsl/functions.xsl "/>

	<xsl:template match="/">

		<xsl:if test="$SearchMode ='FlightOnly'">
			<xsl:choose>
				<xsl:when test="$Format = 'ResultsPage'">
					<xsl:call-template name="ResultsPage" />
				</xsl:when>
				<xsl:when test="$Format = 'ExtrasPage' and $JustResults != 'True'">
					<xsl:call-template name="ExtrasPage" />
				</xsl:when>
				<xsl:when test="$Format = 'ExtrasPage' and $JustResults = 'True'">
					<xsl:call-template name="Results" />
				</xsl:when>
			</xsl:choose>
		</xsl:if>

	</xsl:template>

	<xsl:template name="ResultsPage">

		<div id="divHotelUpsell" onclick="HotelUpsell.ChangeToFlightAndHotel();" class="mobileHide">
			<a class="button primary">
				<trans ml="Hotel Upsell">Book your flight &amp; Hotel Together </trans>
				<i>
					<xsl:text> </xsl:text>
				</i>
			</a>
		</div>

	</xsl:template>


	<xsl:template name="ExtrasPage">
		<xsl:if test ="Results/Hotels/Hotel">
			<div id="divHotelUpsellResults" class="mobileHide">

				<div class="boxTitle">
					<h2 ml="Hotel Upsell">
						<xsl:choose>
							<xsl:when test="$Title != ''">
								<xsl:value-of select="$Title"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Why not add a Hotel?</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</h2>
					<xsl:if test="$SubTitle != ''">
						<p ml="Hotel Upsell">
							<xsl:choose>
								<xsl:when test="$SubTitle != ''">
									<xsl:value-of select="$SubTitle"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text> </xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</p>
					</xsl:if>
				</div>

				<div id="divHotelUpsellResultsContent">
					<xsl:call-template name="Results" />
				</div>

			</div>
		</xsl:if>

	</xsl:template>


	<xsl:template name="Results">
		<xsl:variable name="paxcount" select="sum(Results/Hotels/Hotel[1]/Rooms/Room/Adults) + sum(Results/Hotels/Hotel[1]/Rooms/Room/Children)" />

		<xsl:for-each select="Results/Hotels/Hotel[position() &lt;= $HotelsToDisplay]">

			<div class="box primary result clear">
				<xsl:if test="BestSeller = 'true'">
					<xsl:attribute name="class">box primary result clear bestSeller</xsl:attribute>
				</xsl:if>


				<div class="details">

					<h2 class="name">
						<xsl:value-of select="Name"/>
					</h2>
					<h3>
						<xsl:value-of select="Address1"/>
					</h3>

					<xsl:call-template name="StarRating">
						<xsl:with-param name="Rating" select="Rating" />
					</xsl:call-template>


					<img class="mainImage" src="{MainImage/Image}" alt="{MainImage/ImageTitle}" />

					<xsl:variable name="summary">
						<xsl:choose>
							<xsl:when test="string-length(Summary) >= $SummaryLength">
								<xsl:value-of select="substring(Summary, 1, $SummaryLength)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="Summary" />
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<xsl:variable name="ellipsis">
						<xsl:if test="substring($summary, string-length($summary) - 1, string-length($summary)) = '.'">
							<xsl:text>...</xsl:text>
						</xsl:if>
					</xsl:variable>

					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="concat($summary, '...', '&lt;a class=&quot;moreInfo&quot; href=&quot;#&quot; onclick=&quot;HotelResults.ShowDetailsPopup(', PropertyReferenceID, ');return false;&quot;&gt;More Info&lt;/a&gt;')" />
					</xsl:call-template>

				</div>

				<xsl:if test="Bestseller = true">
					<div class="reviews">
						<span id="spnBestSeller_{PropertyReferenceID}" class="bestseller" ml="Hotel Results">Popular Hotel</span>
					</div>
				</xsl:if>


				<div class="price">
					<h2 class="fromPrice">
						<span class="totalPriceText" style="display:none;">
							<xsl:text>Total Price: </xsl:text>
						</span>
						<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice, $PriceFormat))"/>
					</h2>
				</div>

				<div id="divRatesTable_{PropertyReferenceID}" class="ratesTable">
					<xsl:variable name="roomcount" select="count(Rooms/Room)" />

					<xsl:for-each select="Rooms/Room">

						<xsl:call-template name="Rates">
							<xsl:with-param name="roomcount" select="$roomcount" />
							<xsl:with-param name="roomnumber" select="position()" />
							<xsl:with-param name="paxcount" select="$paxcount" />
						</xsl:call-template>

					</xsl:for-each>

					<xsl:if test="count(Rooms/Room/RoomOptions/RoomOption) &gt; 1">
						<a href="#" class="viewToggle" id="aViewMore_{PropertyReferenceID}" onclick="HotelUpsell.ShowRoomOptions('{PropertyReferenceID}');return false;">View more room options</a>
						<a href="#" class="viewToggle" id="aViewLess_{PropertyReferenceID}" onclick="HotelUpsell.HideRoomOptions('{PropertyReferenceID}');return false;" style="display:none;">Hide room options</a>
					</xsl:if>
				</div>

			</div>

		</xsl:for-each>

		<xsl:if test="$HotelsToDisplay &lt; 20">
			<a class="button primary" id="aHotelUpsellMoreHotels" onclick="HotelUpsell.UpdateResults({$HotelsToDisplay} + 3);">View more hotels</a>
		</xsl:if>

	</xsl:template>



	<!-- Rates -->
	<xsl:template name="Rates">
		<xsl:param name="roomcount" />
		<xsl:param name="roomnumber" />
		<xsl:param name="paxcount" />


		<!-- calculate flight price for the room as we may have for example 3 adults with 2 in 1 room and 1 in the other-->
		<xsl:variable name="roomflightprice">
			<xsl:value-of select="($FlightPrice div $paxcount) * (Adults + Children)"/>
		</xsl:variable>

		<!-- per person price -->
		<xsl:variable name="roomflightpricepp">
			<xsl:value-of select="$roomflightprice div (Adults + Children)"/>
		</xsl:variable>

		<table class="def striped hover">
			<tr>
				<th ml="Hotel Results">Room Type</th>
				<th ml="Hotel Results">Meal Basis</th>
				<xsl:choose>
					<xsl:when test="$PackagePrice = 'True'">
						<th ml="Hotel Results">
							<xsl:text>Total Price</xsl:text>
							<xsl:if test="$PerPersonPrice = 'True'">
								<xsl:text> (pp)</xsl:text>
							</xsl:if>
						</th>
					</xsl:when>
					<xsl:otherwise>
						<th ml="Hotel Results">Flight</th>
						<th ml="Hotel Results">Hotel</th>
						<th ml="Hotel Results">
							<xsl:text>Total</xsl:text>
							<xsl:if test="$PerPersonPrice = 'True'">
								<xsl:text> (pp)</xsl:text>
							</xsl:if>
						</th>
					</xsl:otherwise>
				</xsl:choose>

				<th class="book">
					<xsl:text> </xsl:text>
				</th>
			</tr>

			<xsl:for-each select="RoomOptions/RoomOption">
				<tr>
					<xsl:attribute name="class">
						<xsl:text>roomOption</xsl:text>
						<xsl:if test="position() != 1">
							<xsl:text> first</xsl:text>
						</xsl:if>
					</xsl:attribute>
					<xsl:if test="position() != 1">
						<xsl:attribute name="style">
							<xsl:text>display:none;</xsl:text>
						</xsl:attribute>
					</xsl:if>

					<td>
						<xsl:value-of select="RoomType"/>
						<xsl:if test="RoomView != ''">
							<xsl:value-of select="concat(' - ', RoomView)"/>
						</xsl:if>
					</td>
					<td>
						<xsl:value-of select="MealBasis"/>
					</td>
					<xsl:choose>
						<!-- 1. Per Person Prices for package or hotel only -->
						<xsl:when test="($PackagePrice = 'True') and $PerPersonPrice = 'True'">
							<td>
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="Price div (../../Adults + ../../Children) + $roomflightpricepp" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
						</xsl:when>

						<!-- 2. Total price for package or hotel only -->
						<xsl:when test="($PackagePrice = 'True') and $PerPersonPrice = 'False'">
							<td>
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="Price + $roomflightprice" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
						</xsl:when>


						<!-- 3. Per Person Prices for flight, hotel and total -->
						<xsl:when test="$PerPersonPrice = 'True'">
							<td class="priceFlight">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="$roomflightpricepp" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
							<td class="priceHotel">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="Price div (../../Adults + ../../Children)" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
							<td class="priceTotal">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="(Price div (../../Adults + ../../Children)) + $roomflightpricepp" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
						</xsl:when>

						<!-- 4. Total prices for flight, hotel and total -->
						<xsl:otherwise>
							<td class="priceFlight">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="$roomflightprice" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
							<td class="priceHotel">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="Price" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
							<td class="priceTotal">
								<strong>
									<xsl:call-template name="GetSellingPrice">
										<xsl:with-param name="Value" select="Price + $roomflightprice" />
										<xsl:with-param name="Format" select="$PriceFormat" />
										<xsl:with-param name="Currency" select="$CurrencySymbol" />
										<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
									</xsl:call-template>
								</strong>
							</td>
						</xsl:otherwise>
					</xsl:choose>

					<td class="right book">
						<xsl:variable name="propertyid" select="../../../../PropertyReferenceID" />

						<input id="btnBook_{$propertyid}_{$roomnumber}" type="button" class="button primary small" value="Book" ml="Hotel Results" onclick="HotelUpsell.SelectRoom({$propertyid},'{Index}');" />
					</td>
				</tr>
			</xsl:for-each>

		</table>



	</xsl:template>



</xsl:stylesheet>
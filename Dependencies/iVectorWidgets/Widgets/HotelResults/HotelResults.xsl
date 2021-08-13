<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<xsl:include href="../../xsl/functions.xsl"/>
<xsl:include href="../../xsl/markdown.xsl"/>
	
<xsl:param name="CurrencySymbol" />
<xsl:param name="CurrencySymbolPosition" />
<xsl:param name="TabbedContent" />
<xsl:param name="SummaryLength" />
<xsl:param name="RedirectURL" />
<xsl:param name="PackagePrice" />
<xsl:param name="PerPersonPrice" />
<xsl:param name="PriceFormat" />
<xsl:param name="SearchMode" />
<xsl:param name="DisplayBestSeller" />
<xsl:param name="BestSellerImagePath" />
<xsl:param name="BestSellerPosition" />
<xsl:param name="Template" />
<xsl:param name="DisplayEmailDescription" />
<xsl:param name="Theme" />
<xsl:param name="HotelArrivalDate" />
<xsl:param name="HotelDuration" />
<xsl:param name="MapTab" />
<xsl:param name="HotelPriceOnly" />	
<xsl:param name="CancellationChargesPosition" />
<xsl:param name="MapIconsJSON" />
<xsl:param name="AddSelectedFlightToBasket" />

<xsl:template match="/">
	
	<xsl:variable name="paxcount" select="sum(Results/Hotels/Hotel[1]/Rooms/Room/Adults) + sum(Results/Hotels/Hotel[1]/Rooms/Room/Children)" />
	<xsl:variable name="roomcount" select="count(Results/Hotels/Hotel[1]/Rooms/Room)" />
	
	<input type="hidden" id="hidMapIcons" value="{$MapIconsJSON}" />
	<input type="hidden" id="hidAddSelectedFlight" value="{$AddSelectedFlightToBasket}" />
	
	<xsl:choose>
		<xsl:when test="$Template = 'Results'">			
			<xsl:call-template name="Results">
				<xsl:with-param name="paxcount" select="$paxcount" />
			</xsl:call-template>		
		</xsl:when>
		<xsl:when test="$Template = 'QuickView'">
			<xsl:call-template name="QuickView">
				<xsl:with-param name="paxcount" select="$paxcount" />
			</xsl:call-template>
		</xsl:when>
		<xsl:when test="$Template = 'Rates'">			
			<xsl:for-each select="Hotel/Rooms/Room">
				<xsl:call-template name="Rates">
					<xsl:with-param name="roomcount" select="count(/Hotel/Rooms/Room)" />
					<xsl:with-param name="roomnumber" select="position()" />
					<xsl:with-param name="paxcount" select="sum(/Hotel/Rooms/Room/Adults) + sum(/Hotel/Rooms/Room/Children)" />
				</xsl:call-template>
			</xsl:for-each>
		</xsl:when>
		<xsl:when test="$Template = 'HotelDetails'">
			<xsl:call-template name="HotelDetails" />	
		</xsl:when>
		<xsl:when test="$Template = 'HotelImages'">
			<xsl:call-template name="HotelImages" />
		</xsl:when>
		<xsl:when test="$Template = 'Flights'">
			<xsl:call-template name="Flights" />
		</xsl:when>
		<xsl:when test="$Template = 'HotelMap'">
			<xsl:call-template name="HotelMap" />
		</xsl:when>
		<xsl:otherwise>
			<div id="divHotelResultsLoading" style="display:none">
				<p>Please wait as we update your results...</p>
			</div>
			<div id="divHotelResults">
				<xsl:call-template name="Results">
					<xsl:with-param name="paxcount" select="$paxcount" />
				</xsl:call-template>
			</div>
			<input type="hidden" id="hidMapTheme" value="{$Theme}" />
			<div id="divHotelResultsMapHolder" class="box primary" style="display:none;">
				<div id="divHotelResultsMap"><xsl:text> </xsl:text></div>
			</div>

			<div id="divRequestRoomPopup" style="display:none;">
				<xsl:text> </xsl:text>
			</div>
			
			<input type="hidden" id="hidPropertyResults_RoomCount" value="{count(Results/Hotels/Hotel[1]/Rooms/Room)}" />
			<input type="hidden" id="hidWarning_MultiRoomSelect" value="Please select a room option for each room." ml="Hotel Details" />
			<script type="text/javascript">
				int.ll.OnLoad.Run(function () { HotelResults.SetupMapEvents() } );
			</script>
		</xsl:otherwise>
	</xsl:choose>

</xsl:template>
	
	
<!-- Results -->
<xsl:template name="Results">
	<xsl:param name="paxcount" />

	<xsl:if test="count(Results/Hotels/Hotel) = 0 or (count(Results/Hotels/Hotel[SelectedFlight = true()]) = 0 and $PackagePrice = 'True' and $SearchMode = 'FlightPlusHotel')">
		<div class="box primary noresults">
			<h2 ml="Hotel Results">Sorry there are no matching results for your search. Please select an alternative date.</h2>
		</div>
	</xsl:if>
	
	<xsl:for-each select="Results/Hotels/Hotel">

		<div class="box primary result clear">
			<xsl:if test="$DisplayBestSeller = 'True' and BestSeller = 'true'">
				<xsl:attribute name="class">box primary result clear bestSeller</xsl:attribute>
			</xsl:if>
			
			<div class="boxTitle clear">
				<h2 class="name"><xsl:value-of select="Name"/></h2>

				<h2 class="fromPrice">
					<span class="totalPriceText" style="display:none;">
						<xsl:text>Total Price: </xsl:text>
					</span>
					
					<xsl:choose>
						<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice = 0">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:when test="$PerPersonPrice = 'False' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice, $PriceFormat))"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice, $PriceFormat))"/>
						</xsl:otherwise>
					</xsl:choose>

				</h2>

				<span class="dateDurationInfo" style="display:none;">
					<xsl:value-of select="$HotelArrivalDate"/>
					<xsl:text>, </xsl:text>
					<xsl:value-of select="$HotelDuration"/>
					<xsl:text> </xsl:text>
					<xsl:choose>
						<xsl:when test="$HotelDuration = 1">
							<xsl:text>night</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>nights</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> </xsl:text>
				</span>
				
				<span class="perPersonPrice" style="display:none;">
					<xsl:text>(</xsl:text>
					<xsl:choose>
						<xsl:when test="MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice div $paxcount, $PriceFormat))"/>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> pp)</xsl:text>
				</span>

				<h3 class="geography"><xsl:value-of select="concat(Region, ', ', Resort)"/></h3>
				
				<xsl:call-template name="StarRating">
					<xsl:with-param name="Rating" select="Rating" />
				</xsl:call-template>
				
				
				<xsl:if test="$BestSellerPosition = 'Header' and $DisplayBestSeller = 'True' and BestSeller = 'true'">
					<img id="imgBestSeller_{PropertyReferenceID}" class="bestSeller" src="{$BestSellerImagePath}" alt="BestSeller" />
				</xsl:if>
			
			</div>

			<div class="top clear">
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
				
				<xsl:if test="$BestSellerPosition = 'Summary' and $DisplayBestSeller = 'True' and BestSeller = 'true'">
					<img id="imgBestSeller_{PropertyReferenceID}" class="bestSeller" src="{$BestSellerImagePath}" alt="BestSeller" />
				</xsl:if>
				
			</div>


			<xsl:choose>
				<xsl:when test="$TabbedContent = 'True'">
					<div class="tabbedBox">
						<div class="tabs">
							<ul>
								<li id="liRates_{PropertyReferenceID}" class="selected">
									<a href="javascript:HotelResults.ShowTab({PropertyReferenceID}, 'Rates')" ml="Hotel Results">Price</a>
								</li>
								<li id="liHotelDetails_{PropertyReferenceID}">
									<a href="javascript:HotelResults.ShowTab({PropertyReferenceID}, 'HotelDetails')" ml="Hotel Results">Hotel Information</a>
								</li>
								<li id="liHotelImages_{PropertyReferenceID}">
									<a href="javascript:HotelResults.ShowTab({PropertyReferenceID}, 'HotelImages')" ml="Hotel Results">Images</a>
								</li>

								<xsl:if test="$MapTab = 'True'">
									<li id="liHotelMap_{PropertyReferenceID}">
										<a href="javascript:HotelResults.ShowTab({PropertyReferenceID}, 'HotelMap')" ml="Hotel Results">Map</a>
									</li>
								</xsl:if>

								<xsl:if test="SelectedFlight/BookingToken != ''">
									<li id="liFlights_{PropertyReferenceID}">
										<a href="javascript:HotelResults.ShowTab({PropertyReferenceID}, 'Flights')" ml="Hotel Results">Flight Information</a>
									</li>
								</xsl:if>

							</ul>
						</div>


						<!-- Rates -->
						<div id="divTabbedContent_{PropertyReferenceID}">
							<xsl:variable name="roomcount" select="count(Rooms/Room)" />
							<xsl:for-each select="Rooms/Room">
								<xsl:call-template name="Rates">
									<xsl:with-param name="roomcount" select="$roomcount" />
									<xsl:with-param name="roomnumber" select="position()" />
									<xsl:with-param name="paxcount" select="$paxcount" />
								</xsl:call-template>
							</xsl:for-each>
							<xsl:if test="$roomcount > 1 and $RedirectURL != 'Property'">
								<input type="button" class="button primary small" value="Select" onclick="HotelResults.SelectMultiRoom({PropertyReferenceID});" ml="Hotel Details" />
							</xsl:if>
						</div>

					</div>
				</xsl:when>
				<xsl:otherwise>
					<div id="divTabbedContent_{PropertyReferenceID}">
						<xsl:variable name="roomcount" select="count(Rooms/Room)" />					
						<xsl:for-each select="Rooms/Room">
							<xsl:call-template name="Rates">
								<xsl:with-param name="roomcount" select="$roomcount" />
								<xsl:with-param name="roomnumber" select="position()" />
								<xsl:with-param name="paxcount" select="$paxcount" />
							</xsl:call-template>										
						</xsl:for-each>
						<xsl:if test="$roomcount > 1 and $RedirectURL != 'Property'">
							<input type="button" class="button primary multiroom" value="Continue" onclick="HotelResults.SelectMultiRoom({PropertyReferenceID});" ml="Hotel Details" />
						</xsl:if>
					</div>
				</xsl:otherwise>
			</xsl:choose>

			
				<xsl:if test ="$DisplayEmailDescription = 'True'">
					<div id="divExtraOptions">
						<a href="javascript:HotelEmailDescription.ShowPopup('{PropertyReferenceID}')">Email Description</a>
						<!--<a href="#" onclick="HotelEmailDescription.ShowPopup('{PropertyReferenceID}');return false;"  ml="Hotel Results">Email Description</a>-->
					</div>
				</xsl:if> 
			
		</div>		
		
	</xsl:for-each>	
	
</xsl:template>
	

<!-- Quick View -->
<xsl:template name="QuickView">
	<xsl:param name="paxcount" />

	<xsl:if test="count(Results/Hotels/Hotel) = 0 or (count(Results/Hotels/Hotel[SelectedFlight = true()]) = 0 and $PackagePrice = 'True' and $SearchMode = 'FlightPlusHotel')">
		<div class="box primary noresults">
			<h2 ml="Hotel Results">Sorry there are no matching results for your search. Please select an alternative date.</h2>
		</div>
	</xsl:if>
	
	<xsl:for-each select="Results/Hotels/Hotel">

		<div class="box primary result clear">
			<xsl:if test="$DisplayBestSeller = 'True' and BestSeller = 'true'">
				<xsl:attribute name="class">box primary result clear bestSeller</xsl:attribute>
			</xsl:if>
			
			<div class="boxTitle clear">
				<h2 class="name"><xsl:value-of select="Name"/></h2>

				<h2 class="fromPrice">
					<span class="totalPriceText" style="display:none;">
						<xsl:text>Total Price: </xsl:text>
					</span>
					
					<xsl:choose>
						<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:when test="$PerPersonPrice = 'True' and MinPackagePrice = 0">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:when test="$PerPersonPrice = 'False' and MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice, $PriceFormat))"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice, $PriceFormat))"/>
						</xsl:otherwise>
					</xsl:choose>

				</h2>

				<span class="dateDurationInfo" style="display:none;">
					<xsl:value-of select="$HotelArrivalDate"/>
					<xsl:text>, </xsl:text>
					<xsl:value-of select="$HotelDuration"/>
					<xsl:text> </xsl:text>
					<xsl:choose>
						<xsl:when test="$HotelDuration = 1">
							<xsl:text>night</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>nights</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> </xsl:text>
				</span>
				
				<span class="perPersonPrice" style="display:none;">
					<xsl:text>(</xsl:text>
					<xsl:choose>
						<xsl:when test="MinPackagePrice > 0 and $HotelPriceOnly != 'True'">
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinPackagePrice div $paxcount, $PriceFormat))"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="concat($CurrencySymbol, format-number(MinHotelPrice div $paxcount, $PriceFormat))"/>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> pp)</xsl:text>
				</span>

				<h3 class="geography"><xsl:value-of select="concat(Region, ', ', Resort)"/></h3>
				
				<xsl:call-template name="StarRating">
					<xsl:with-param name="Rating" select="Rating" />
				</xsl:call-template>
				
				
				<xsl:if test="$BestSellerPosition = 'Header' and $DisplayBestSeller = 'True' and BestSeller = 'true'">
					<img id="imgBestSeller_{PropertyReferenceID}" class="bestSeller" src="{$BestSellerImagePath}" alt="BestSeller" />
				</xsl:if>
			
			</div>

			<div class="top clear">
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
				
				<xsl:if test="$BestSellerPosition = 'Summary' and $DisplayBestSeller = 'True' and BestSeller = 'true'">
					<img id="imgBestSeller_{PropertyReferenceID}" class="bestSeller" src="{$BestSellerImagePath}" alt="BestSeller" />
				</xsl:if>
				
			</div>

			<div class="box">

				<!-- Rates -->
				<div id="divTabbedContent_{PropertyReferenceID}">
					<xsl:variable name="roomcount" select="count(Rooms/Room)" />
					<xsl:for-each select="Rooms/Room">
						<xsl:call-template name="Rates">
							<xsl:with-param name="roomcount" select="$roomcount" />
							<xsl:with-param name="roomnumber" select="position()" />
							<xsl:with-param name="paxcount" select="$paxcount" />
						</xsl:call-template>
					</xsl:for-each>
					<xsl:if test="$roomcount > 1 and $RedirectURL != 'Property'">
						<input type="button" class="button primary small" value="Select" onclick="HotelResults.SelectMultiRoom({PropertyReferenceID});" ml="Hotel Details" />
					</xsl:if>
				</div>

			</div>
			
		</div>		
		
	</xsl:for-each>
	
</xsl:template>
	
	
<!-- Rates -->
<xsl:template name="Rates">
	<xsl:param name="roomcount" />
	<xsl:param name="roomnumber" />
	<xsl:param name="paxcount" />
		
	<xsl:variable name="flightprice">
		<xsl:choose>
			<xsl:when test="../../SelectedFlight/BookingToken != ''">
				<xsl:value-of select="../../SelectedFlight/Total"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>0</xsl:text>					
			</xsl:otherwise>
		</xsl:choose>			
	</xsl:variable>
	
	
	<!-- calculate flight price for the room as we may have for example 3 adults with 2 in 1 room and 1 in the other-->
	<xsl:variable name="roomflightprice">
		<xsl:value-of select="($flightprice div $paxcount) * (Adults + Children)"/>
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
				<xsl:when test="$PackagePrice = 'True' or $SearchMode = 'HotelOnly'">
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
			
			<th class="book"><xsl:text> </xsl:text></th>
		</tr>
						
		<xsl:for-each select="RoomOptions/RoomOption">			
			<tr>
				<td>
					<xsl:value-of select="RoomType"/>
					<xsl:if test="RoomView != ''">
						<xsl:value-of select="concat(' - ', RoomView)"/>
					</xsl:if>				
				</td>
				<td><xsl:value-of select="MealBasis"/></td>
				<xsl:choose>
					<!-- 1. Per Person Prices for package or hotel only -->
					<xsl:when test="($PackagePrice = 'True' or $SearchMode = 'HotelOnly') and $PerPersonPrice = 'True'">
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
					<xsl:when test="($PackagePrice = 'True' or $SearchMode = 'HotelOnly') and $PerPersonPrice = 'False'">
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
					<xsl:if test="$PackagePrice != 'True'">
						<xsl:attribute name="class">right book packagePrice</xsl:attribute>
					</xsl:if>
					<xsl:variable name="propertyid" select="../../../../PropertyReferenceID" />
					<xsl:choose>
						<xsl:when test="$RedirectURL = 'Property'">
							<input id="btnBook_{$propertyid}_{$roomnumber}" type="button" class="button primary small" value="Select" ml="Hotel Results" onclick="window.location='{../../../../URL}'" />
						</xsl:when>
						<xsl:when test="$roomcount > 1">
							<input type="radio" class="radio" value="{Index}" name="rad_roomoption_{$propertyid}_{$roomnumber}" />
						</xsl:when>								  
						<xsl:otherwise>
							<input id="btnBook_{$propertyid}_{$roomnumber}" type="button" class="button primary small" value="Select" ml="Hotel Results" onclick="HotelResults.SelectRoom({$propertyid},'{Index}');" />
						</xsl:otherwise>
					</xsl:choose>		
				</td>
			</tr>
		</xsl:for-each>
		
	</table>

	
		
</xsl:template>
	
	
<!-- Description -->
<xsl:template name="HotelDetails">
	
	<xsl:call-template name="Markdown">
		<xsl:with-param name="text" select="Property/Description"/>
	</xsl:call-template>	
		
</xsl:template>
	
	
<!-- Images -->
<xsl:template name="HotelImages">
		
	<!-- Main Image -->
	<img id="imgMainImage_{Property/PropertyReferenceID}" class="mainImage" src="{Property/MainImage}" alt="" />	

	<!-- Other images -->
	<xsl:if test="count(Property/Images/Image) &gt; 0">
		<div id="divOtherImages_{Property/PropertyReferenceID}" class="otherimages clear">
			<img src="{Property/MainImage}" alt="" onmouseover="web.ImageHover(this, 'imgMainImage_{/Property/PropertyReferenceID}', 'divOtherImages_{/Property/PropertyReferenceID}');" />	
			<xsl:for-each select="Property/Images/Image">
				<img src="{Image}" alt="" onmouseover="web.ImageHover(this, 'imgMainImage_{/Property/PropertyReferenceID}', 'divOtherImages_{/Property/PropertyReferenceID}');"/>
			</xsl:for-each>
		</div>
	</xsl:if>
	
</xsl:template>


<!-- Map -->
<xsl:template name="HotelMap">

	<!--<xsl:variable name="apos" select="" />-->
	
	<script type="text/javascript">
		MapWidget.Setup('divMap_<xsl:value-of select="Property/PropertyReferenceID"/>', '<xsl:value-of select="$Theme"/>');
	</script>

	<script type="text/javascript">
	    MapWidget.AddSingleMarker(<xsl:value-of select="Property/Latitude"/>, <xsl:value-of select="Property/Longitude"/>, 'markerid_<xsl:value-of select="Property/PropertyReferenceID"/>', 'Hotel');
	</script>
	
	<!-- map container -->
	<div id="divMap_{Property/PropertyReferenceID}" class="mapTab">
		Testing 123
	</div>

</xsl:template>
	
	
<!-- Flight Details -->
<xsl:template name="Flights">
		
	<xsl:for-each select="Hotel/SelectedFlight">
		
		<div class="selectedflight clear">
			
			<!-- Outbound -->
			<div class="outbound">
				<h4 ml="Hotel Results">Outbound</h4>
				
				<p>
					<trans ml="Hotel Results">Departs </trans>
					<strong><xsl:value-of select="DepartureAirport"/></strong>					
				</p>
				<p><xsl:value-of select="OutboundFlightCode"/></p>
				<p>
					<trans ml="Hotel Details" mlparams="{OutboundDepartureDate}~ShortDate">{0}</trans>
					<xsl:text> </xsl:text>
					<strong><xsl:value-of select="OutboundDepartureTime"/></strong>					
				</p>
				
				<p>
					<trans ml="Hotel Results">Arrives </trans>
					<strong><xsl:value-of select="ArrivalAirport"/></strong>					
				</p>				
				<p>
					<trans ml="Hotel Details" mlparams="{OutboundArrivalDate}~ShortDate">{0}</trans>
					<xsl:text> </xsl:text>
					<strong><xsl:value-of select="OutboundArrivalTime"/></strong>					
				</p>
				
			</div>
			
			<!-- Return -->
			<div class="return">
				<h4 ml="Hotel Details">Return</h4>
				
				<p>
					<trans ml="Hotel Details">Departs </trans>
					<strong><xsl:value-of select="ArrivalAirport"/></strong>					
				</p>
				<p><xsl:value-of select="ReturnFlightCode"/></p>
				<p>
					<trans ml="Hotel Details" mlparams="{ReturnDepartureDate}~ShortDate">{0}</trans>
					<xsl:text> </xsl:text>
					<strong><xsl:value-of select="ReturnDepartureTime"/></strong>					
				</p>				
				
				<p>
					<trans ml="Hotel Details">Arrives </trans>
					<strong><xsl:value-of select="DepartureAirport"/></strong>					
				</p>				
				<p>
					<trans ml="Hotel Details" mlparams="{ReturnArrivalDate}~ShortDate">{0}</trans>
					<xsl:text> </xsl:text>
					<strong><xsl:value-of select="ReturnArrivalTime"/></strong>					
				</p>
				
			</div>
						
		</div>		
		
	</xsl:for-each>
		
</xsl:template>

</xsl:stylesheet>
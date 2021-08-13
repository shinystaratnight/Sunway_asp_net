<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
<xsl:include href="../../xsl/functions.xsl"/>
	
<xsl:param name="CurrencySymbol" />
<xsl:param name="CurrencySymbolPosition" />	
<xsl:param name="PerPersonPrices" />
<xsl:param name="ShowMapLink" />
<xsl:param name="ScrollToTopAfterFilter" />
<xsl:param name="ShowAverageScoreFilter" />
<xsl:param name="MaxAverageScore" />
<xsl:param name="AlwaysShow" />
<xsl:param name="UseDynamicMap" />
<xsl:param name="Longitude" />
<xsl:param name="Latitude" />
<xsl:param name="CustomTitle" />
<xsl:param name="SearchMode" />
	

	<xsl:template match="/">

	<xsl:variable name="HotelCount" select="count(Results/Hotels/Hotel)"/>
	<xsl:variable name="HotelWithFlightCount" select="count(Results/Hotels/Hotel[SelectedFlight/BookingToken != ''])"/>

	<xsl:if test="($SearchMode = 'HotelOnly' and $HotelCount > 0) or ($SearchMode = 'FlightPlusHotel' and $HotelWithFlightCount > 0)">
		
		<xsl:variable name="paxcount" select="sum(Results/Hotels/Hotel[1]/Rooms/Room/Adults) + sum(Results/Hotels/Hotel[1]/Rooms/Room/Children)" />

		<xsl:for-each select="Results/Filters">
		
			<div id="divHotelFilter" class="sidebarBox primary">
				<xsl:if test ="count(../../Results/Hotels/Hotel) = 1 and $AlwaysShow != 'true'">
					<xsl:attribute name="style">
						<xsl:text>display:none;</xsl:text>
					</xsl:attribute>
				</xsl:if>
				<div class="boxTitle">
					<xsl:choose>
						<xsl:when test="$CustomTitle != ''">
							<h2 ml="Results Filter">
								<xsl:value-of select="$CustomTitle"/>
								<xsl:text> </xsl:text>
							</h2>
						</xsl:when>
						<xsl:otherwise>
							<h2 ml="Results Filter">Filter Results</h2>
						</xsl:otherwise>
					</xsl:choose>
				</div>
				
				<!-- Map Link -->
				<xsl:if test="$ShowMapLink = 'True'">
					<div id="divMapLink" class="filter">
						<h4 ml="Results Filter">Show on Map</h4>	

                        <xsl:if test="$UseDynamicMap = 'True'">
                            <a class="dynamic" href="#" onclick="MapWidget.Show();HotelResults.Hide();HotelResultsFooter.Hide();HotelResultsFilter.ClearBestSeller(true);SearchSummary.Hide();SearchSummary.SelectTab(int.f.GetObject('aMapView'));return false;">
                                <img id="imgStaticMap" title="{Name}" alt="map" src="https://maps.googleapis.com/maps/api/staticmap?center={$Latitude},{$Longitude}&amp;zoom=10&amp;size=178x92&amp;scale=2&amp;maptype=roadmap&amp;sensor=false&amp;"/>
                                <span>View</span>
                            </a>
                        </xsl:if>							
                        <xsl:if test="$UseDynamicMap != 'True'">
                            <a href="#" onclick="MapWidget.Show();HotelResults.Hide();HotelResultsFooter.Hide();HotelResultsFilter.ClearBestSeller(true);SearchSummary.Hide();SearchSummary.SelectTab(int.f.GetObject('aMapView'));return false;">
                                <span>View</span>
                            </a>
                        </xsl:if>
					</div>
				</xsl:if>
				
				<!-- Price Slider -->
				<xsl:if test="/Results/CustomSetting/ShowPriceSlider = 'true'">
					<div id="divMinMaxPrice" class="filter">
						<h4 ml="Results Filter">Price</h4>
						<p ml="Results Filter" id="pMinMaxFilterMessage">Drag the sliders to choose your min/max hotel price</p>

						<!-- Slider Control -->
						<div id="sldPrice" class="slider">
							<label class="range start">

								<xsl:variable name="minprice">
									<xsl:choose>
										<xsl:when test="$PerPersonPrices = 'True'">
											<xsl:value-of select="floor(/Results/MinPrice div $paxcount)"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="floor(/Results/MinPrice)"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>

								<xsl:call-template name="GetSellingPrice">
									<xsl:with-param name="Value" select="$minprice" />
									<xsl:with-param name="Format" select="'###,##0'" />
									<xsl:with-param name="Currency" select="$CurrencySymbol" />
									<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
								</xsl:call-template>

								<xsl:if test="$PerPersonPrices = 'True'">
									<xsl:text>pp</xsl:text>
								</xsl:if>

							</label>
							<label class="range end">

								<xsl:variable name="maxprice">
									<xsl:choose>
										<xsl:when test="$PerPersonPrices = 'True'">
											<xsl:value-of select="floor(/Results/MaxPrice div $paxcount)"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="floor(/Results/MaxPrice)"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:variable>

								<xsl:call-template name="GetSellingPrice">
									<xsl:with-param name="Value" select="$maxprice" />
									<xsl:with-param name="Format" select="'###,##0'" />
									<xsl:with-param name="Currency" select="$CurrencySymbol" />
									<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
								</xsl:call-template>

								<xsl:if test="$PerPersonPrices = 'True'">
									<xsl:text>pp</xsl:text>
								</xsl:if>

							</label>
							<div id="sldPrice_Highlight" class="highlight" style="left:0;width:100%;">
								<xsl:text> </xsl:text>
							</div>
							<a id="sldPrice_Start" class="sliderbutton needsclick" style="left:0;">
								<xsl:text> </xsl:text>								
							</a>
							<a id="sldPrice_End" class="sliderbutton needsclick" style="right:0;">
								<xsl:text> </xsl:text>							
							</a>
						</div>

						<p class="displayrange">
							<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
								<xsl:value-of select="$CurrencySymbol"/>
							</xsl:if>
							<span id="sldPrice_DisplayMin">
								<xsl:choose>
									<xsl:when test="$PerPersonPrices = 'True'">
										<xsl:value-of select="floor(/Results/MinPrice div $paxcount)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="floor(/Results/MinPrice)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$CurrencySymbolPosition = 'Append'">
								<xsl:value-of select="$CurrencySymbol"/>
							</xsl:if>

							<xsl:text> - </xsl:text>

							<xsl:if test="$CurrencySymbolPosition = 'Prepend'">
								<xsl:value-of select="$CurrencySymbol"/>
							</xsl:if>
							<span id="sldPrice_DisplayMax">
								<xsl:choose>
									<xsl:when test="$PerPersonPrices = 'True'">
										<xsl:value-of select="floor(/Results/MaxPrice div $paxcount)"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="floor(/Results/MaxPrice)"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
							<xsl:if test="$CurrencySymbolPosition = 'Append'">
								<xsl:value-of select="$CurrencySymbol"/>
							</xsl:if>

							<xsl:if test="$PerPersonPrices = 'True'">
								<xsl:text>pp</xsl:text>
							</xsl:if>
						</p>

						<input type="hidden" id="sldPrice_MinValue" value="{floor(/Results/MinPrice)}" />
						<input type="hidden" id="sldPrice_MaxValue" value="{ceiling(/Results/MaxPrice)}" />
						<input type="hidden" id="sldPrice_AbsMinValue" value="{floor(/Results/MinPrice)}" />
						<input type="hidden" id="sldPrice_AbsMaxValue" value="{ceiling(/Results/MaxPrice)}" />

						<xsl:if test="$PerPersonPrices = 'True'">
							<input type="hidden" id="sldPrice_DisplayValueDivide" value="{$paxcount}" />
						</xsl:if>
						<input type="hidden" id="sldPrice_OnChange" value="HotelResultsFilter.Filter();"/>

						<script type="text/javascript">
							web.Slider.Setup('sldPrice');
						</script>

					</div>
				</xsl:if>
				
				<!-- Hotel Name -->
				<xsl:if test="/Results/CustomSetting/ShowHotelName = 'true'">
					<div id="divHotelName" class="filter">
						<h4 ml="Results Filter">Hotel Name</h4>
						<p ml="Results Filter">If you know the name of the hotel you wish to stay in, you can use the search below.</p>
						<div class="textbox icon search right">
							<i>
								<xsl:text> </xsl:text>
							</i>
							<input type="text" id="txtFilterHotelName" onkeyup="HotelResultsFilter.HotelNameKeyUp();" class="textbox" placeholder="Hotel Name..."/>
							<input type="hidden" name="txtFilterHotelName_Placeholder" id="txtFilterHotelName_Placeholder" ml="Results Filter">
								<xsl:attribute name="value">
									<xsl:choose>
										<xsl:when test="/Results/PlaceholderOverrides/txtFilterHotelName != ''">
											<xsl:value-of select="/Results/PlaceholderOverrides/txtFilterHotelName"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:text>Hotel Name...</xsl:text>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
							</input>
						</div>
					</div>
				</xsl:if>

				<!-- Landmarks -->
				<xsl:if test="/Results/CustomSetting/ShowLandmark = 'true'">
					<xsl:if test ="count(Landmarks/Landmark) > 0">
						<div id="divLandmarks" class="filter">
							<h4 ml="Results Filter">Landmark</h4>

							<select id="ddlLandmarkDuration">
								<option value="1">
									<xsl:text>1km</xsl:text>
								</option>
								<option value="2">
									<xsl:text>2km</xsl:text>
								</option>
								<option value="3">
									<xsl:text>3km</xsl:text>
								</option>
								<option value="4">
									<xsl:text>4km</xsl:text>
								</option>
								<option value="5" selected="selected">
									<xsl:text>5km</xsl:text>
								</option>
								<option value="6">
									<xsl:text>6km</xsl:text>
								</option>
								<option value="7">
									<xsl:text>7km</xsl:text>
								</option>
								<option value="8">
									<xsl:text>8km</xsl:text>
								</option>
								<option value="9">
									<xsl:text>9km</xsl:text>
								</option>
								<option value="10">
									<xsl:text>10km</xsl:text>
								</option>
							</select>

							<label class="from">From</label>

							<div class="landmarkFilter">
								<label class="checkboxLabel">
									<input type="checkbox" class="checkbox" checked="checked" value="true" id="chk_l_{0}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
									<xsl:text>Any</xsl:text>
								</label>
							</div>

							<xsl:for-each select="Landmarks/Landmark">
								<div class="landmarkFilter">
									<label class="checkboxLabel">
										<input type="checkbox" class="checkbox" value="true" id="chk_l_{LandmarkID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
										<xsl:value-of select="Name"/>
									</label>
								</div>
							</xsl:for-each>


							<xsl:text> </xsl:text>
						</div>
					</xsl:if>
				</xsl:if>
				
				<!-- Average Rating Slider -->
				<xsl:if test="/Results/CustomSetting/ShowTripAdvisorSlider = 'true'">
					<div id="divAverageRating" class="filter">
						<xsl:if test="$ShowAverageScoreFilter = 'False'">
							<xsl:attribute name="style">
								<xsl:text>display:none;</xsl:text>
							</xsl:attribute>
						</xsl:if>
						<h4 ml="Results Filter">Average Rating:</h4>


						<!-- Slider Control -->
						<div id="sldAverageRating" class="slider">
							<label class="range start">
								<xsl:text>1</xsl:text>
							</label>

							<label class="range end">
								<xsl:value-of select="$MaxAverageScore" />
							</label>
							<div id="sldAverageRating_Highlight" class="highlight" style="left:0;width:100%;">
								<xsl:text> </xsl:text>
							</div>
							<a id="sldAverageRating_Start" class="sliderbutton" style="left:0;">
								<xsl:text> </xsl:text>
							</a>
							<a id="sldAverageRating_End" class="sliderbutton" style="right:-7px;">
								<xsl:text> </xsl:text>
							</a>
						</div>

						<p class="displayrange">
							<span id="sldAverageRating_DisplayMin">
								<xsl:text>1</xsl:text>
							</span>

							<span id="sldAverageRating_DisplayMax">
								<xsl:value-of select="$MaxAverageScore" />
							</span>
						</p>

						<input type="hidden" id="sldAverageRating_MinValue" value="1" />
						<input type="hidden" id="sldAverageRating_MaxValue" value="{$MaxAverageScore}" />
						<input type="hidden" id="sldAverageRating_AbsMinValue" value="1" />
						<input type="hidden" id="sldAverageRating_AbsMaxValue" value="{$MaxAverageScore}" />
						<input type="hidden" id="sldAverageRating_OnChange" value="HotelResultsFilter.Filter();"/>

						<script type="text/javascript">
							web.Slider.Setup('sldAverageRating');
						</script>

					</div>
				</xsl:if>
			
				<!-- Rating -->
				<xsl:if test="/Results/CustomSetting/ShowStarRating = 'true'">
					<div id="divRating" class="filter">
						<h4 ml="Results Filter">Star Rating</h4>

						<xsl:for-each select="Ratings/Rating">
							<xsl:sort data-type="number" select="Rating" order="ascending"/>
							<div class="starRatingFilter">
								<label class="checkboxLabel">
									<xsl:if test="Selected = 'true'">
										<xsl:attribute name="class">selected</xsl:attribute>
									</xsl:if>
									<input type="checkbox" class="checkbox" value="true" id="chk_r_{Rating}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="checked">true</xsl:attribute>
										</xsl:if>
									</input>
									<xsl:choose>
										<xsl:when test="Rating = 0">
											<span ml="Results Filter">
												<xsl:text>Unrated</xsl:text>
											</span>
										</xsl:when>
										<xsl:otherwise>
											<trans ml="Results Filter" mlparams="{Rating}">{0} Star</trans>
										</xsl:otherwise>
									</xsl:choose>
								</label>

								<xsl:text> (</xsl:text>
								<span id="spn_r_count_{floor(Rating)}">
									<xsl:value-of select="Count"/>
								</span>
								<xsl:text>)</xsl:text>

							</div>
						</xsl:for-each>
					</div>
				</xsl:if>

				<!-- TripAdvisor Rating -->
				<xsl:if test="/Results/CustomSetting/ShowTripAdvisorRating = 'true' and count(TripAdvisorRatings/TripAdvisorRatings/TripAdvisorRating[Rating &gt; 0]) > 0">
					<div id="divTripAdvisorRating" class="filter">
						<h4 ml="Results Filter">
							<img src="https://www.tripadvisor.co.uk/img/cdsi/langs/en_UK/tripadvisor_logo_158x29-15293-0.gif"/>
						</h4>

						<xsl:for-each select="TripAdvisorRatings/TripAdvisorRatings/TripAdvisorRating[Rating &gt; 0]">
							<xsl:sort data-type="number" select="Rating" order="ascending"/>
							<div class="taRatingFilter">
								<label class="checkboxLabel taCheckBoxLabel checkboxLabel_{Rating}">
									<xsl:if test="Selected = 'true'">
										<xsl:attribute name="class">selected</xsl:attribute>
									</xsl:if>
									<input type="checkbox" class="checkbox taCheckBox" value="true" id="chk_ta_{Rating}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="checked">true</xsl:attribute>
										</xsl:if>
									</input>
									<img width="91" src="https://www.tripadvisor.com/img/cdsi/img2/ratings/traveler/{format-number(Rating, '#0.0')}-11367-1.gif" alt="" />
								</label>

								<xsl:text> (</xsl:text>
								<span id="spn_ta_count_{floor(Rating)}">
									<xsl:value-of select="Count"/>
								</span>
								<xsl:text>)</xsl:text>

							</div>
						</xsl:for-each>
					</div>
				</xsl:if>
			
			
			
				<!-- Board Basis -->
				<xsl:if test="/Results/CustomSetting/ShowMealBasis = 'true'">
					<div id="divMealBasis" class="filter">
						<h4 ml="Results Filter">Board Basis</h4>

						<xsl:for-each select="MealBases/MealBasis">
							<div class="mealBasisFilter">
								<label class="checkboxLabel">
									<xsl:if test="Selected = 'true'">
										<xsl:attribute name="class">selected</xsl:attribute>
									</xsl:if>
									<input type="checkbox" class="checkbox" value="true" id="chk_mb_{MealBasisID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="checked">true</xsl:attribute>
										</xsl:if>
									</input>
									<xsl:value-of select="MealBasis"/>
								</label>

								<xsl:text> (</xsl:text>
								<span id="spn_mb_count_{MealBasisID}">
									<xsl:value-of select="Count"/>
								</span>
								<xsl:text>)</xsl:text>
							</div>
						</xsl:for-each>

					</div>
				</xsl:if>
			
			
				<!-- Resorts -->
				<xsl:if test="/Results/CustomSetting/ShowResort = 'true'">
					<xsl:if test="count(Resorts/Resort) > 1">
						<div id="divGeographyLevel3" class="filter alt">
							<h4 ml="Results Filter">Resort</h4>

							<xsl:for-each select="Resorts/Resort[Count &gt; 0]">
								<xsl:sort select="Resort" data-type="text"/>
								<div>
									<label class="checkboxLabel">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="class">selected</xsl:attribute>
										</xsl:if>
										<input type="checkbox" class="checkbox" value="true" id="chk_g3_{GeographyLevel3ID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
											<xsl:if test="Selected = 'true'">
												<xsl:attribute name="checked">true</xsl:attribute>
											</xsl:if>
										</input>
										<xsl:value-of select="Resort"/>
									</label>

									<xsl:text> (</xsl:text>
									<span id="spn_g3_count_{GeographyLevel3ID}">
										<xsl:value-of select="Count"/>
									</span>
									<xsl:text>)</xsl:text>
								</div>
							</xsl:for-each>
						</div>
					</xsl:if>
				</xsl:if>
				
				
				
				<!-- Facilities -->
				<xsl:if test="/Results/CustomSetting/ShowFacilities = 'true'">
					<xsl:if test="count(FilterFacilities/FilterFacility[Count > 0]) > 0">
						<div id="divFilterFacility" class="filter">
							<h4 ml="Results Filter">Facilities</h4>

							<xsl:for-each select="FilterFacilities/FilterFacility[Count > 0]">
								<xsl:sort select="FilterFacility" data-type="text"/>
								<div>
									<label class="checkboxLabel">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="class">selected</xsl:attribute>
										</xsl:if>
										<input type="checkbox" class="checkbox" value="true" id="chk_f_{FilterFacilityID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
											<xsl:if test="Selected = 'true'">
												<xsl:attribute name="checked">true</xsl:attribute>
											</xsl:if>
										</input>
										<xsl:value-of select="Facility"/>
									</label>

									<!-- Change if showing totals or from price -->
									<xsl:text> (</xsl:text>
									<span id="spn_f_count_{Priority}">
										<xsl:value-of select="Count"/>
									</span>
									<xsl:text>)</xsl:text>

								</div>

							</xsl:for-each>

						</div>
					</xsl:if>
				</xsl:if>

				<!-- Attributes -->
				<xsl:if test="/Results/CustomSetting/ShowAttributes = 'true'">

                <xsl:if test="count(ProductAttributes/ProductAttribute[Count > 0]) > 0">
					<div id="divFilterAttributes" class="filter">
						<h4 ml="Results Filter">Attributes</h4>

						<xsl:for-each select="ProductAttributes/ProductAttribute[Count > 0]">
							<xsl:sort select="ProductAttribute" data-type="text"/>
							<div>
								<label class="checkboxLabel">
									<xsl:if test="Selected = 'true'">
										<xsl:attribute name="class">selected</xsl:attribute>
									</xsl:if>
									<input type="checkbox" class="checkbox" value="true" id="chk_pa_{ProductAttributeID}" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
										<xsl:if test="Selected = 'true'">
											<xsl:attribute name="checked">true</xsl:attribute>
										</xsl:if>
									</input>
									<xsl:value-of select="ProductAttribute"/>
								</label>

								<!-- Change if showing totals or from price -->
								<xsl:text> (</xsl:text>
								<span id="spn_pa_count_{ProductAttributeID}">
									<xsl:value-of select="Count"/>
								</span>
								<xsl:text>)</xsl:text>

							</div>

						</xsl:for-each>

					</div>
				</xsl:if>
              </xsl:if>
				
				
				<!-- Best Seller -->
				<input type="hidden" id="hidBestSeller" value="false" />
				
				<!-- Scroll After Filter -->
				<input type="hidden" id="hidHotelFilter_ScrollAfterFilter">
					<xsl:attribute name="value">
						<xsl:choose>
							<xsl:when test="$ScrollToTopAfterFilter = 'False'">False</xsl:when>
							<xsl:otherwise>True</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</input>
				
			</div>
		
		</xsl:for-each>
	
		<script type="text/javascript">
			int.ll.OnLoad.Run(function () { HotelResultsFilter.Setup(); });
		</script>

	</xsl:if>

</xsl:template>

</xsl:stylesheet>
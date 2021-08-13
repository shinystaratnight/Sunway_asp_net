<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	
	<xsl:param name="Title" />	
	<xsl:param name="Mode" />	
	<xsl:param name="DaysEitherSide" />
	<xsl:param name="DaysPerPage" />
	<xsl:param name="DayWidth" />
	<xsl:param name="PerPersonPrices" />
	<xsl:param name="PriceFormat" />
	<xsl:param name="FullDate" />
	
	<xsl:param name="DaysOnly" select="'False'" />		
	<xsl:param name="SelectedDate" />	
	<xsl:param name="TotalPax" />	
	<xsl:param name="PropertyResultsMinPrice" />
	<xsl:param name="FlightCarouselMode" />
	<xsl:param name="FlightCarouselSearchAgain" />
	<xsl:param name="HideLeftArrow" />
	<xsl:param name="UpdateFunction" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="FlightPlusHotelURL" />
	<xsl:param name="FlightOnlyURL" />
	<xsl:param name="HideIfNoAlternatives" />
	<xsl:param name="NoResultsWarningMessage" />
	

<xsl:template match="/">

	<input type="hidden" id="hidWarning_NoSearchResults" value="{$NoResultsWarningMessage}" />
	<input type="hidden" id="hidCarouselDaysPerPage" value="{$DaysPerPage}" />
		
		<!--Check whether we want to display the carousel when there are no alternative flights-->
		<xsl:variable name ="alternateFlights">
			<xsl:choose>
				<!--If yes, check how many alternative flights there are-->
				<xsl:when test="$HideIfNoAlternatives = 'True'">
					<xsl:value-of select ="sum(Results/Dates/Date[Date != $SelectedDate]/AvailableFlights)"/>
				</xsl:when>
				<!--If no, set alternateFlights to 1 so it will always return true in the next test-->
				<xsl:otherwise>
					<xsl:text>1</xsl:text>
				</xsl:otherwise>
			</xsl:choose>			
		</xsl:variable>
	
	
		<xsl:variable name="totalpages">
			<xsl:value-of select="ceiling((($DaysEitherSide * 2) + 1) div $DaysPerPage)"/>
		</xsl:variable>
		
		<xsl:variable name="currentpage">
			<xsl:value-of select="ceiling($totalpages div 2)"/>
		</xsl:variable>

	
		<!--Check of there are alternate flights, if yes render the widget-->
		<xsl:if test="count(Results/Dates/Date) > 0 and $alternateFlights &gt; 0">

			<input type="hidden" id="hidFlightPlusHotelRedirectURL" value="{$FlightPlusHotelURL}" />
			<input type="hidden" id="hidFlightOnlyRedirectURL" value="{$FlightOnlyURL}" />

			<xsl:choose>
				<xsl:when test="$DaysOnly = 'False'">
				
					<div id="divResultsCarousel" class="rotatorBox clear">
						
						<xsl:if test="$Title != ''">
							<h3 ml="Results Carousel"><xsl:value-of select="$Title"/></h3>
						</xsl:if>
			
						<div id="divResultsCarouselContent">
							<xsl:attribute name="class">
									<xsl:if test ="$CSSClassOverride != ''">
										<xsl:value-of select="$CSSClassOverride"/>
										<xsl:text> </xsl:text>
									</xsl:if>								
									<xsl:text>rotatorBoxContent</xsl:text>							
							</xsl:attribute>

							<xsl:variable name="totalwidth" select="count(Results/Dates/Date) * $DayWidth" />
			
							<div id="divResultsCarouselItems" class="itemHolder">
								<xsl:if test="$totalwidth > 0">
									<xsl:attribute name="style">width:<xsl:value-of select="$totalwidth"/>px</xsl:attribute>	
								</xsl:if>
								
								<xsl:for-each select="Results/Dates/Date">
									<xsl:call-template name="CarouselItem" />
								</xsl:for-each>

							</div>	
							
						</div>
						
						<input type="hidden" id="hidCarouselSearchAgain" name="hidCarouselSearchAgain" value="{$FlightCarouselSearchAgain}" />
						
						<xsl:if test="$totalpages > 1 or $FlightCarouselSearchAgain = 'True'">
							<xsl:if test ="not($HideLeftArrow = 'True')">
								<a id="aResultsCarouselLeft" class="scroller left {$CSSClassOverride}" onclick="ResultsCarousel.Backward({$DaysEitherSide});"><xsl:text> </xsl:text></a>
							</xsl:if>						
							<a id="aResultsCarouselRight" class="scroller right {$CSSClassOverride}" onclick="ResultsCarousel.Forward({$DaysEitherSide});"><xsl:text> </xsl:text></a>
						</xsl:if>

					</div>

					<script type="text/javascript">
						<xsl:text>int.ll.OnLoad.Run(function () {</xsl:text>
						<xsl:text>ResultsCarousel.Setup('</xsl:text>
						<xsl:value-of select="$Mode"/><xsl:text>',</xsl:text>
						<xsl:value-of select="$currentpage"/><xsl:text>,</xsl:text>
						<xsl:value-of select="$totalpages"/><xsl:text>,</xsl:text>
						<xsl:value-of select="$DayWidth * $DaysPerPage"/>
						<xsl:if test="$UpdateFunction != ''">
							<xsl:text>,</xsl:text>
							<xsl:value-of select="$UpdateFunction"/>
						</xsl:if>
						<xsl:text>); });</xsl:text>				
					</script>
				
				</xsl:when>
				<xsl:otherwise>
					<xsl:for-each select="Results/Dates/Date">
						<xsl:call-template name="CarouselItem" />					
					</xsl:for-each>					
				</xsl:otherwise>
			</xsl:choose>

		</xsl:if>

</xsl:template>
	

<xsl:template name="CarouselItem">
	
	<a href="#" onclick="">
		<xsl:choose>
			<xsl:when test="$FlightCarouselMode = 'Cache' and AvailableFlights > 0">
				<xsl:attribute name="onclick">ResultsCarousel.ReSearch('<xsl:value-of select="Date"/>');return false;</xsl:attribute>
			</xsl:when>
			<xsl:when test="$FlightCarouselMode = 'Results' and AvailableFlights > 0">
				<xsl:attribute name="onclick">ResultsCarousel.Filter('<xsl:value-of select="Date"/>');return false;</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="onclick">return false;</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
							
		<div>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="Date = $SelectedDate">
						<xsl:text>carousel selected</xsl:text>
					</xsl:when>
					<xsl:when test="AvailableFlights = 0">
						<xsl:text>carousel noflights</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>carousel</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<h4>
				<xsl:if test ="$FullDate='True'">
				<xsl:call-template name ="GetDay">
					<xsl:with-param name="SQLDate" select="Date"/>
				</xsl:call-template>
				<xsl:text> </xsl:text>
				</xsl:if>
				<span>
					<xsl:call-template name="ShortDate">
						<xsl:with-param name="SQLDate" select="Date"/>
					</xsl:call-template>
				</span>
			</h4>
						
			<xsl:choose>
				
				<!-- Flight and hotel mode, current date, use property results min price if set (includes its selected flight price -->
				<xsl:when test="$Mode = 'FlightAndHotel' and Date = $SelectedDate and $PropertyResultsMinPrice > 0">
					
					<p class="from" ml="Results Carousel">from</p>
					<p class="price">																				
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="$PropertyResultsMinPrice"/>
							<xsl:with-param name="Format" select="$PriceFormat"/>
							<xsl:with-param name="Currency" select="$CurrencySymbol"/>
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
						</xsl:call-template>					
					</p>					
					<xsl:if test="$PerPersonPrices = 'True'">
						<p ml="Results Carousel" class="perPerson">per person</p>
					</xsl:if>				
					
				</xsl:when>
				
				<xsl:when test="AvailableFlights &gt; 0 and ($Mode = 'Flight' or ($Mode = 'FlightAndHotel' and PropertyFromPrice > 0))">
					
					<xsl:variable name="price">
						<xsl:choose>
							<xsl:when test="$Mode = 'FlightAndHotel'">
								<xsl:value-of select="FlightFromPrice + PropertyFromPrice"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="FlightFromPrice"/>
							</xsl:otherwise>												
						</xsl:choose>
					</xsl:variable>
					
					<xsl:variable name="displayprice">
						<xsl:choose>
							<xsl:when test="$PerPersonPrices = 'False'">
								<xsl:value-of select="$price * $TotalPax"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="$price"/>
							</xsl:otherwise>												
						</xsl:choose>						
					</xsl:variable>					
						
					<p class="from" ml="Results Carousel">from</p>
					<p class="price">		
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="$displayprice"/>
							<xsl:with-param name="Format" select="$PriceFormat"/>
							<xsl:with-param name="Currency" select="$CurrencySymbol"/>
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
						</xsl:call-template>					
					</p>					
					<xsl:if test="$PerPersonPrices = 'True'">
						<p ml="Results Carousel" class="perPerson">per person</p>
					</xsl:if>
					
					<xsl:variable name="selectedprice">
						<xsl:choose>
							<xsl:when test="$PerPersonPrices = 'False'">
								<xsl:value-of select="(/Results/Dates/Date[Date = $SelectedDate]/FlightFromPrice * $TotalPax) + /Results/Dates/Date[Date = $SelectedDate]/PropertyFromPrice"/>		
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="/Results/Dates/Date[Date = $SelectedDate]/FlightFromPrice + /Results/Dates/Date[Date = $SelectedDate]/PropertyFromPrice"/>		
							</xsl:otherwise>
						</xsl:choose>										
					</xsl:variable>
					
					<xsl:if test="$displayprice &lt; $selectedprice">
						<p class="saving">
							<trans ml="Results Carousel">Save</trans>
							<xsl:text> </xsl:text>
							<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="ceiling($selectedprice - $displayprice)"/>
							<xsl:with-param name="Format" select="'###,##0'"/>
							<xsl:with-param name="Currency" select="$CurrencySymbol"/>
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
						</xsl:call-template>						
						</p>
					</xsl:if>
					
				</xsl:when>	
				
				<xsl:when test="($Mode = 'FlightAndHotel' and PropertyFromPrice = 0) and AvailableFlights > 0">
					<p class="unavailable" ml="Results Carousel">No Available</p>
					<p class="unavailable" ml="Results Carousel">Hotels</p>
				</xsl:when>			
				
				<xsl:when test="AvailableFlights = 0">
					<p class="unavailable" ml="Results Carousel">No Available</p>
					<p class="unavailable" ml="Results Carousel">Flights</p>
				</xsl:when>		
				
			</xsl:choose>
	
		</div>
		
	</a>
				
</xsl:template>
				  
</xsl:stylesheet>

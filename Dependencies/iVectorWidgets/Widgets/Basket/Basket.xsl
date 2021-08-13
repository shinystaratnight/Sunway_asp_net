<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:include href="../../xsl/functions.xsl "/>
	
	<xsl:param name="Title" />
	<xsl:param name="ExcludeSubtotals" />	
	<xsl:param name="PassengerBreakdown" />
	<xsl:param name="HideBaggage" />
	<xsl:param name="TotalComponents" />
	
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CMSBaseURL" />

	<xsl:param name="IncludeContainer" select="'False'" />
	<xsl:param name="Printable" />
	<xsl:param name="DescriptiveSubtotals" />
	
	<xsl:param name="CSSClassOverride" />
	
	<xsl:param name="UpdateFunction" />
	<xsl:param name="PriceFormat" />

<xsl:template match="/">
	
	<xsl:if test="$TotalComponents > 0">
	
		<xsl:if test="$IncludeContainer = 'True'">
			<xsl:choose>
				<xsl:when test="$CSSClassOverride != ''">
					<xsl:text disable-output-escaping="yes">&lt;div id="divBasket" class="</xsl:text>
					<xsl:value-of select="$CSSClassOverride"/>
					<xsl:text disable-output-escaping="yes">"&gt;</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text disable-output-escaping="yes">&lt;div id="divBasket" class="sidebarBox primary clear"&gt;</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>

			<!-- Title -->
			<div class="boxTitle">
				<h2 ml="Basket">
					<xsl:choose>
						<xsl:when test="$Title != ''">
							<xsl:value-of select="$Title"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>Booking Summary</xsl:text>
						</xsl:otherwise>
					</xsl:choose>	
				</h2>
			</div>
		
	
			<!-- passengers -->
			<xsl:if test="PassengerBreakdown = 'True'">
				<div id="divBasket_Passengers">
					<h3 id="hPassengers" class="section" ml="Basket">
						<xsl:choose>
							<xsl:when test="/BookingBasket/TextOverrides/PassengersTitle != ''">
								<xsl:value-of select="/BookingBasket/TextOverrides/PassengersTitle" />
							</xsl:when>
							<xsl:otherwise>					
								<xsl:text>Passengers</xsl:text>
							</xsl:otherwise>
						</xsl:choose>			
					</h3>
					<p><xsl:text> </xsl:text></p>
				</div>
			</xsl:if>
	
		
			<!-- hotel -->
			<xsl:for-each select="/BookingBasket/BasketProperties/BasketProperty">
				<xsl:call-template name="Hotel" />				
			</xsl:for-each>	
		
		
			<!-- flight -->
			<xsl:for-each select="/BookingBasket/BasketFlights/BasketFlight">
				<xsl:call-template name="Flight" />			
			</xsl:for-each>
	
	
			<!-- transfer -->
			<xsl:for-each select="/BookingBasket/BasketTransfers/BasketTransfer">
				<xsl:call-template name="Transfer" />
			</xsl:for-each>
		
		
			<!-- extra -->
			<xsl:for-each select="/BookingBasket/BasketExtras/BasketExtra">
				<xsl:call-template name="Extra" />
			</xsl:for-each>
		
		
			<!-- Total -->
			<div class="total">

				<h2>
					<trans ml="Basket">Total Price</trans>
					<span id="spnBasketTotal">
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="/BookingBasket/TotalPrice + /BookingBasket/TotalMarkup" />
							<xsl:with-param name="Format" select="'######'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						</xsl:call-template>
					</span>
				</h2>
			
			</div>
			<xsl:if test ="$Printable = 'True'">
				<a class="print" href="javascript:window.print()"><span ml="Basket">Print</span></a>
			</xsl:if>
	
		<xsl:if test="$IncludeContainer = 'True'">
			<xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
		</xsl:if>
		
	</xsl:if>
	
	<input type="hidden" id="hidBasketUpdateFunction" value="{$UpdateFunction}"/>

</xsl:template>


	<xsl:template name="Hotel">

		<h3 id="hHotel" class="section" ml="Basket">
			<xsl:choose>
				<xsl:when test="/BookingBasket/TextOverrides/HotelTitle != ''">
					<xsl:value-of select="/BookingBasket/TextOverrides/HotelTitle" />
				</xsl:when>
				<xsl:otherwise>					
					<xsl:text>Your Hotel</xsl:text>
				</xsl:otherwise>
			</xsl:choose>			
		</h3>

		<div>
			<xsl:choose>
				<xsl:when test="$ExcludeSubtotals = 'True'">
					<xsl:attribute name="class">property clear package</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">property clear</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			
			
			
			<img src="{ContentXML/Hotel/MainImage/Image}" alt="" />
			<h4>
				<xsl:value-of select="ContentXML/Hotel/Name"/>
			</h4>
			<xsl:call-template name="StarRating">
				<xsl:with-param name="Rating" select="ContentXML/Hotel/Rating" />
				<xsl:with-param name="Small" select="'true'" />
			</xsl:call-template>
			<p>
				<xsl:value-of select="concat(ContentXML/Hotel/Region, ', ', ContentXML/Hotel/Resort)"/>
			</p>


			<!-- display each room -->
			<xsl:for-each select="RoomOptions/BasketPropertyRoomOption">
				
				<xsl:variable name="token" select="RoomBookingToken"/>

				<xsl:for-each select="../../ContentXML/Hotel/Rooms/Room">
					
					<xsl:variable name="roomnumber" select="position()" />					
					
					<xsl:for-each select="RoomOptions/RoomOption[RoomBookingToken=$token]">
					
						<div class="room clear">
							<h5>
								<trans ml="Basket">Room</trans>
								<xsl:text> </xsl:text>
								<xsl:value-of select="$roomnumber"/>
							</h5>
							<p>
								<xsl:value-of select="concat(RoomType, ', ', MealBasis, ', ')"/>
								<xsl:value-of select="concat(../../Adults, ' ')" />
								<trans ml="Basket">Adults</trans>
								<xsl:value-of select="concat(', ', ../../Children, ' ')" />
								<trans ml="Basket">Children</trans>
								<xsl:value-of select="concat(', ', ../../Infants, ' ')" />
								<trans ml="Basket">Infants</trans>
							</p>
						</div>						
						
					</xsl:for-each>
					

				</xsl:for-each>
				
			</xsl:for-each>


			<!-- subtotal -->
			<xsl:if test="$ExcludeSubtotals = 'False'">
				<h3 class="price">
					<xsl:choose>
						<xsl:when test="$DescriptiveSubtotals != ''">
							Hotel:
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Basket">Subtotal</trans>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> </xsl:text>
					<span>
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="sum(/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption/TotalPrice) + sum(/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption/Markup)" />
							<xsl:with-param name="Format" select="'######'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						</xsl:call-template>
					</span>
				</h3>				
			</xsl:if>


		</div>
		
		
	</xsl:template>


	<xsl:template name="Flight">

		<div class="flights clear">

			<h3 id="hFlights" class="section" ml="Basket">
				<xsl:choose>
					<xsl:when test="/BookingBasket/TextOverrides/FlightTitle != ''">
						<xsl:value-of select="/BookingBasket/TextOverrides/FlightTitle" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Your Flights</xsl:text>
					</xsl:otherwise>
				</xsl:choose>	
			</h3>


			<!-- outbound -->
			<div class="flight">
				<h5 ml="Basket">Outbound</h5>
				<img class="carrierLogo" src="{$CMSBaseURL}Carriers/{(ContentXML/Flight/FlightSectors/FlightSector[Direction = 'Outbound'])[1]/FlightCarrierLogo}" alt="" />
				<p>
					<xsl:value-of select="concat(ContentXML/Flight/DepartureAirport, ' (', ContentXML/Flight/DepartureAirportCode, ') ')" />
					<trans ml="Basket">to</trans>
					<xsl:text> </xsl:text>
					<xsl:value-of select="concat(ContentXML/Flight/ArrivalAirport, ' (', ContentXML/Flight/ArrivalAirportCode, ')')" />
				</p>
				<p>
					<xsl:value-of select="ContentXML/Flight/OutboundDepartureTime"/>
					<xsl:text> </xsl:text>
					<trans ml="Basket" mlparams="{ContentXML/Flight/OutboundDepartureDate}~ShortDate">{0}</trans>
				</p>
			</div>

			<xsl:if test="count(SeatMaps/SeatMap[Direction='Outbound']/Seats/Seat[BookedGuestID!=0]) &gt; 0">
				<div class="seats">
					<h5 ml="Basket">Seats</h5>
					<xsl:variable name="seats" select="SeatMaps/SeatMap[Direction='Outbound']/Seats"/>
					<xsl:for-each select="/BookingBasket/GuestDetails/GuestDetail">
						<p>
							<xsl:variable name="CurrentGuestID" select="GuestID"/>
							<xsl:if test="count($seats/Seat[BookedGuestID=$CurrentGuestID]) &gt; 0">
								<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ': ')"/>
								<xsl:for-each select="$seats/Seat[BookedGuestID=$CurrentGuestID]">
									<xsl:value-of select="SeatCode"/>
								</xsl:for-each>
							</xsl:if>
						</p>
					</xsl:for-each>
				</div>
			</xsl:if>


			<!-- return -->
			<div class="flight">
				<h5 ml="Basket">Return</h5>
				<img class="carrierLogo" src="{$CMSBaseURL}Carriers/{(ContentXML/Flight/FlightSectors/FlightSector[Direction = 'Return'])[1]/FlightCarrierLogo}" alt="" />
				<p>
					<xsl:value-of select="concat(ContentXML/Flight/ArrivalAirport, ' (', ContentXML/Flight/ArrivalAirportCode, ') ')" />
					<trans ml="Basket">to</trans>
					<xsl:text> </xsl:text>
					<xsl:value-of select="concat(ContentXML/Flight/DepartureAirport, ' (', ContentXML/Flight/DepartureAirportCode, ')')" />
				</p>
				<p>
					<xsl:value-of select="ContentXML/Flight/ReturnDepartureTime"/>
					<xsl:text> </xsl:text>
					<trans ml="Basket" mlparams="{ContentXML/Flight/ReturnDepartureDate}~ShortDate">{0}</trans>
				</p>
			</div>

			<xsl:if test="count(SeatMaps/SeatMap[Direction='Return']/Seats/Seat[BookedGuestID!=0]) &gt; 0">
				<div class="seats">
					<h5 ml="Basket">Seats</h5>
					<xsl:variable name="seats" select="SeatMaps/SeatMap[Direction='Return']/Seats"/>
					<xsl:for-each select="/BookingBasket/GuestDetails/GuestDetail">
						<p>
							<xsl:variable name="CurrentGuestID" select="GuestID"/>
							<xsl:if test="count($seats/Seat[BookedGuestID=$CurrentGuestID]) &gt; 0">
								<xsl:value-of select="concat(Title, ' ', FirstName, ' ', LastName, ': ')"/>
								<xsl:for-each select="$seats/Seat[BookedGuestID=$CurrentGuestID]">
									<xsl:value-of select="SeatCode"/>
								</xsl:for-each>
							</xsl:if>
						</p>
					</xsl:for-each>
				</div>
			</xsl:if>

			<!-- baggage -->
			<xsl:if test="$HideBaggage = 'False' and BasketFlightExtras/BasketFlightExtra[DefaultBaggage='true']/QuantitySelected &gt; 0">
				<xsl:for-each select ="BasketFlightExtras/BasketFlightExtra[ExtraType='Baggage']">
					<xsl:if test ="QuantitySelected &gt; 0">
						<div class="baggage">
							<h5 ml="Basket">Baggage</h5>
							<xsl:value-of select ="Description"/>
							<xsl:text>: </xsl:text>
							<strong>
								<xsl:value-of select ="QuantitySelected"/>
							</strong>
						</div>
					</xsl:if>
				</xsl:for-each>				
			</xsl:if>

			<!-- extras -->
			<xsl:if test="count(BasketFlightExtras/BasketFlightExtra[ExtraType != 'Baggage' and QuantitySelected &gt; 0]) &gt; 0">
				<p>
					<h5 ml="Basket">Extras:</h5>
					<xsl:for-each select="BasketFlightExtras/BasketFlightExtra[ExtraType != 'Baggage' and QuantitySelected &gt; 0]">
						<p>
							<trans ml="Basket" mlparams="Description|QuantitySelected">{0} x {1}</trans>
						</p>
					</xsl:for-each>
				</p>				
			</xsl:if>

			<!-- subtotal -->
			<xsl:if test="$ExcludeSubtotals = 'False'">
				<h3 class="price">
					<xsl:choose>
						<xsl:when test="$DescriptiveSubtotals != ''">
							Flights:
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Basket">Subtotal</trans>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text> </xsl:text>
					<span>
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="sum(/BookingBasket/BasketFlights/BasketFlight/Flight/Price) + sum(/BookingBasket/BasketFlights/BasketFlight/Markup)" />
							<xsl:with-param name="Format" select="'######'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						</xsl:call-template>
					</span>
				</h3>
			</xsl:if>
			
			
			<!-- Package Price -->
			<xsl:variable name="TransferCount" select="count(/BookingBasket/BasketTransfers/BasketTransfer)" />
	
			<xsl:if test="$ExcludeSubtotals = 'True' and $TransferCount > 0">
			
				<xsl:variable name="PackageTotal" select="sum(/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption/TotalPrice) 
							  + sum(/BookingBasket/BasketFlights/BasketFlight/Flight/Price)
							  + sum(/BookingBasket/BasketProperties/BasketProperty/RoomOptions/BasketPropertyRoomOption/Markup) 
							  + sum(/BookingBasket/BasketFlights/BasketFlight/Markup)" />
			
				<h3 class="price">
					<trans ml="Basket">Subtotal</trans>
					<xsl:text> </xsl:text>
					<span>
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="$PackageTotal" />
							<xsl:with-param name="Format" select="'######'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						</xsl:call-template>
					</span>
				</h3>
			</xsl:if>

		</div>
		
		
		
	</xsl:template>


	<xsl:template name="Transfer">

		<div class="transfer">

			<h3 class="section" ml="Basket">
				<xsl:choose>
					<xsl:when test="/BookingBasket/TextOverrides/TransferTitle != ''">
						<xsl:value-of select="/BookingBasket/TextOverrides/TransferTitle" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>Your Transfers</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</h3>


			<!-- outbound -->
			<xsl:if test="ContentXML/Transfer/OneWay = 'false'">
				<h5 ml="Basket">Outbound</h5>
			</xsl:if>
			<p>
				<xsl:value-of select="concat(ContentXML/Transfer/DepartureParent, ' ')"/>
				<trans ml="Basket">to</trans>
				<xsl:value-of select="concat(' ', ContentXML/Transfer/ArrivalParent)"/>
			</p>
			<p>
				<trans ml="Basket" mlparams="{ContentXML/Transfer/OutboundDate}~ShortDate">{0}</trans>
				<xsl:value-of select="concat(', ', ContentXML/Transfer/OutboundTime)"/>
			</p>
			<xsl:if test="ContentXML/Transfer/OutboundTime != ContentXML/Transfer/OutboundJourneyTime">
				<p ml="Basket" mlparams="{ContentXML/Transfer/OutboundJourneyTime}">Transfer time {0} minutes</p>
			</xsl:if>

			<!-- return -->
			<xsl:if test="ContentXML/Transfer/OneWay = 'false'">
				<h5 ml="Basket">Return</h5>
				<p>
					<xsl:value-of select="concat(ContentXML/Transfer/ArrivalParent, ' ')"/>
					<trans ml="Basket">to</trans>
					<xsl:value-of select="concat(' ', ContentXML/Transfer/DepartureParent)"/>
				</p>
				<p>
					<trans ml="Basket" mlparams="{ContentXML/Transfer/ReturnDate}~ShortDate">{0}</trans>
					<xsl:value-of select="concat(', ', ContentXML/Transfer/ReturnTime)"/>
				</p>
				<xsl:if test="ContentXML/Transfer/ReturnTime != ContentXML/Transfer/ReturnJourneyTime">
					<p ml="Basket" mlparams="{ContentXML/Transfer/ReturnJourneyTime}">Transfer time {0} minutes</p>
				</xsl:if>
			</xsl:if>
			
			

			<!-- subtotal -->
			<h3 class="price">
				<xsl:choose>
					<xsl:when test="$DescriptiveSubtotals != ''">
						Transfers:
					</xsl:when>
					<xsl:otherwise>
						<trans ml="Basket">Subtotal</trans>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text> </xsl:text>
				<span>
					<xsl:call-template name="GetSellingPrice">
						<xsl:with-param name="Value" select="sum(/BookingBasket/BasketTransfers/BasketTransfer/Transfer/Price) + sum(/BookingBasket/BasketTransfers/BasketTransfer/Markup)" />
						<xsl:with-param name="Format" select="'######'" />
						<xsl:with-param name="Currency" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
					</xsl:call-template>
				</span>
			</h3>

		</div>
		
	</xsl:template>
	
	
	<xsl:template name="Extra">
		
		<div class="extra">
			
			<h3 id="hHotel" class="section" ml="Basket">
				<xsl:choose>
					<xsl:when test="/BookingBasket/TextOverrides/ExtraTitle != ''">
						<xsl:value-of select="/BookingBasket/TextOverrides/ExtraTitle" />
					</xsl:when>
					<xsl:otherwise>					
						<xsl:text>Your Extras</xsl:text>
					</xsl:otherwise>
				</xsl:choose>			
			</h3>
			
			<h5>
				<xsl:value-of select="BasketExtraOptions/BasketExtraOption/ExtraName"/>
			</h5>
			
			<p>
				<trans ml="Basket">For</trans>
				<xsl:text> </xsl:text>
				<xsl:call-template name="PassengerSummary">
					<xsl:with-param name="Adults" select="BasketExtraOptions/BasketExtraOption/Adults" />
					<xsl:with-param name="Children" select="BasketExtraOptions/BasketExtraOption/Children" />
					<xsl:with-param name="Infants" select="BasketExtraOptions/BasketExtraOption/Infants" />
				</xsl:call-template>
			</p>
			
			<!-- subtotal -->
			<h3 class="price">
				<xsl:choose>
					<xsl:when test="$DescriptiveSubtotals != ''">
						<trans ml="Basket">Extras:</trans>
					</xsl:when>
					<xsl:otherwise>
						<trans ml="Basket">Subtotal</trans>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text> </xsl:text>
				<span>
					<xsl:call-template name="GetSellingPrice">
						<xsl:with-param name="Value" select="BasketExtraOptions/BasketExtraOption/TotalPrice" />
						<xsl:with-param name="Format" select="'######'" />
						<xsl:with-param name="Currency" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
					</xsl:call-template>
				</span>
			</h3>
			
		</div>
		
	</xsl:template>
			  
</xsl:stylesheet>
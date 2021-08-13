<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />
  <xsl:include href="../../xsl/markdown.xsl" />
  <xsl:include href="../../xsl/functions.xsl" />
  <xsl:template name="QuoteEmailPDF" match="/">
    <html>
      <body>
        <table id="wrapper">
          <tr>
            <td class="MainCell">
              <xsl:call-template name="AgentHeader" />
            </td>
          </tr>
          <tr>
            <td class="MainCell">
              <xsl:call-template name="QuoteHeader" />
            </td>
          </tr>
          <tr>
            <td class="MainCell">
              <xsl:call-template name="PropertyInfoBox" />
            </td>
          </tr>
          <tr>
            <td class="MainCell">
              <xsl:call-template name="components" />
            </td>
          </tr>
          <tr>
            <td class="MainCell">
              <xsl:call-template name="FlightDetails" />
            </td>
          </tr>
          <tr>
            <td class="MainCell">
              <xsl:call-template name="TermsAndConditions" />
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="AgentHeader" >
    <xsl:param name="AgentInfo" />
    <table id="AgentHeader">
      <tr class="AgentPhoneRow" >
        <td></td>
        <td class="NumberCell">
          <xsl:choose >
            <xsl:when test="QuoteDocumentationModel/Trade" >
              <xsl:value-of select="QuoteDocumentationModel/Trade/Telephone" />
            </xsl:when>
            <xsl:otherwise >
              <xsl:value-of select="QuoteDocumentationModel/FooterContent/ContactNumber" />
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </tr>
      <tr>
        <td class="AgentName">
          <xsl:choose >
            <xsl:when test="QuoteDocumentationModel/Trade" >
              <xsl:value-of select="QuoteDocumentationModel/Trade/Name" />
            </xsl:when>
            <xsl:otherwise >
              <img src="https://www.sunway.ie/images/sun-holidays.png" />
            </xsl:otherwise>
          </xsl:choose>
        </td>
        <td class="AdditionalContactInfoContainer" >
          <table class="AgentAdditionalContact">
            <tr class="AgentEmail">
              <td>
                <xsl:choose >
                  <xsl:when test="QuoteDocumentationModel/Trade" >
                    <xsl:value-of select="QuoteDocumentationModel/Trade/Email" />
                  </xsl:when>
                  <xsl:otherwise >
                    <xsl:value-of select="QuoteDocumentationModel/FooterContent/ContactEmail" />
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
            <tr>
              <td class="AgentAddress" style="text-align:right;font-size:16px;padding-right:8px;font-size:15px;">
                <xsl:choose >
                  <xsl:when test="QuoteDocumentationModel/Trade" >
                    <xsl:value-of select="QuoteDocumentationModel/Trade/Address1" />
                  </xsl:when>
                  <xsl:otherwise >
                    <xsl:value-of select="QuoteDocumentationModel/FooterContent/ContactAddress" />
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
						<xsl:if test="QuoteDocumentationModel/Trade" >
							<xsl:if test="QuoteDocumentationModel/Trade/Address2 !=''">
								<tr>
									<td class="AgentAddress2" style="text-align:right;font-size:16px;padding-right:8px;font-size:15px;">
										<xsl:value-of select="QuoteDocumentationModel/Trade/Address2" />
									</td>
								</tr>
							</xsl:if>
							<tr>
								<td class="AgentTownCity" style="text-align:right;font-size:16px;padding-right:8px;font-size:15px;">
									<xsl:value-of select="QuoteDocumentationModel/Trade/TownCity" />
								</td>
							</tr>
							<xsl:if test="QuoteDocumentationModel/Trade/County !=''">
								<tr>
									<td class="AgentCounty" style="text-align:right;font-size:16px;padding-right:8px;font-size:15px;">
										<xsl:value-of select="QuoteDocumentationModel/Trade/County" />
									</td>
								</tr>
							</xsl:if>
							<tr>
								<td class="AgentPostCode" style="text-align:right;font-size:16px;padding-right:8px;font-size:15px;">
									<xsl:value-of select="QuoteDocumentationModel/Trade/PostCode" />
								</td>
							</tr>
						</xsl:if>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="QuoteHeader" >
    <table id="QuoteHeader">
      <tr>
        <td class="QuoteHeaderText">YOUR QUOTE</td>
        <td class="CostBoxContainer">
          <table class="CostBox">
            <tr>
              <td class="QuoteHeaderLabelContainer">
                <table class="QuoteHeaderLabel">
                  <tr>
                    <td>Total Party</td>
                  </tr>
                  <tr class="QuoteHeaderLabelLower" >
                    <td>From</td>
                  </tr>
                </table>
              </td>
              <td class="QuoteHeaderPrice">
                <xsl:variable name="FlightCost" >
                  <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/TotalPrice" />
                </xsl:variable>
                <xsl:variable name="RoomCost" >
                  <xsl:value-of select="sum(QuoteDocumentationModel/QuoteDocumentationRoomOptions/QuoteDocumentationRoomOption/RoomOption/TotalPrice)" />
                </xsl:variable>
                <xsl:value-of select="QuoteDocumentationModel/SellingCurrencySymbol" />
                <xsl:value-of select="format-number($RoomCost+$FlightCost, '#.##')" />
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="PropertyInfoBox" >
    <table id="PropertyInfoBox">
      <tr class="PropertyInfoRow">
        <td class="PropertyInfoImages">
          <table>
            <xsl:for-each select="QuoteDocumentationModel/Property/Images/Image" >
              <xsl:if test="position() &lt;= 5">
                <tr>
                  <td class="PropertyInfoImageContainer">
                    <img src="{Source}" width="300px"/>
                  </td>
                </tr>
              </xsl:if>
            </xsl:for-each>
          </table>
        </td>
        <td class="PropertyInfoTextContainer">
          <table >
            <tr class="PropertyInfoHeaderRow" >
              <td>
                <table class="PropertyInfoHeaderContainer">
                  <tr>
                    <td class="PropertyInfoName">
                      <xsl:value-of select="QuoteDocumentationModel/Property/Name" />
                    </td>
                    <td class="PropertyInfoStarRating">
                      <xsl:call-template name="GetStarRating" >
                        <xsl:with-param name="StarRating" select="QuoteDocumentationModel/Property/Rating" />
                      </xsl:call-template>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr class="PropertyInfoLocationRow">
              <td>
                <xsl:value-of select="QuoteDocumentationModel/Property/Country" />
                <xsl:text >, </xsl:text>
                <xsl:value-of select="QuoteDocumentationModel/Property/Resort" />
              </td>
            </tr>
            <tr class="PropertyInfoDescriptionRow" >
              <td>
                <xsl:call-template name="Markdown" >
                  <xsl:with-param name="text" select="QuoteDocumentationModel/Property/Description" />
                </xsl:call-template>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="components" >
    <table id="components">
      <tr>
        <td class="ComponentsHeader">COMPONENTS</td>
      </tr>
      <tr>
        <td class="ComponentsTableContainer">
          <table class="ComponentsTable">
            <tr>
              <th class="FirstCell">Room Type</th>
              <th>Board Basis</th>
              <th>Check-in</th>
              <th>Duration</th>
              <th>Baggage</th>
              <th>Transfers</th>
            </tr>
            <xsl:for-each select="QuoteDocumentationModel/QuoteDocumentationRoomOptions/QuoteDocumentationRoomOption" >
              <tr>
                <td class="FirstCell">
                  <xsl:value-of select="RoomOption/RoomType" />
                </td>
                <td>
                  <xsl:value-of select="MealBasis/Name" />
                </td>
                <td>
                  <xsl:call-template name="ShortDate" >
                    <xsl:with-param name="SQLDate" select="ancestor::QuoteDocumentationModel/PropertyResult/ArrivalDate" />
                  </xsl:call-template>
                </td>
                <td>
                  <xsl:value-of select="ancestor::QuoteDocumentationModel/PropertyResult/Duration" />
                  <xsl:choose >
                    <xsl:when test="ancestor::QuoteDocumentationModel/PropertyResult/Duration = 1" >
                      <xsl:text > Night</xsl:text>
                    </xsl:when>
                    <xsl:otherwise >
                      <xsl:text > Nights</xsl:text>
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
                <td>
                  <xsl:call-template name="ExcludedIncluded"><xsl:with-param name="source"><xsl:value-of select="ancestor::QuoteDocumentationModel/FlightResult/FlightResult/Source"/></xsl:with-param></xsl:call-template>
                </td>
                <td>
                  <xsl:call-template name="ExcludedIncluded"><xsl:with-param name="source"><xsl:value-of select="ancestor::QuoteDocumentationModel/FlightResult/FlightResult/Source"/></xsl:with-param></xsl:call-template>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="FlightDetails">
    <xsl:variable name="OutboundFlightCode" select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/FlightCode" />
    <table id="FlightDetails">
      <tr>
        <td class="FlightDetailsHeader">
          FLIGHT DETAILS
        </td>
      </tr>
      <tr>
        <td>
          <table class="FlightRows">
            <tr class="OutboundFlightRow">
              <td class="FlightInfo">
                <table>
                  <tr>
                    <td>
                      <img src="{concat('https://sunway.ivector.co.uk/Content/Carriers/',QuoteDocumentationModel/FlightResult/CarrierLogo)}" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/CarrierName" />
                      <xsl:value-of select="$OutboundFlightCode" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ClassName" />
                    </td>
                  </tr>
                </table>
              </td>
              <td class="FlightDetailsDeparture">
                <table>
                  <tr>
                    <td class="b">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/DepartureTime" />
                    </td>
                  </tr>
                  <tr>
                    <td class="b">
                      <xsl:call-template name="ShortDate" >
                        <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/DepartureDate" />
                      </xsl:call-template>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/DepartureAirportCode" />
                    </td>
                  </tr>
                  <tr>
                    <td class="AirportName">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/DepartureAirportName" />
                    </td>
                  </tr>
                </table>
              </td>
              <td class="arrow">
                <table class="TravelArrow">
                  <tr>
                    <td>
                      <img src="../Images/arrow.png" width="150px" height="auto" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:choose >
                        <xsl:when test="count(QuoteDocumentationModel/FlightResult/FlightResult/FlightSectors/FlightSector[Direction='Outbound']) = 1" >
                          <xsl:value-of select="'Direct'" />
                        </xsl:when>
                        <xsl:otherwise >
                          <xsl:value-of select="concat(count(QuoteDocumentationModel/FlightResult/FlightSectors/FlightSector[Direction='Outbound']),' Stops')" />
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                  </tr>
                </table>
              </td>
              <td class="FlightDetailsArrival">
                <table>
                  <tr>
                    <td class="b">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/ArrivalTime" />
                    </td>
                  </tr>
                  <tr>
                    <td class="b">
                      <xsl:call-template name="ShortDate" >
                        <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/ArrivalDate" />
                      </xsl:call-template>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ArrivalAirportCode" />
                    </td>
                  </tr>
                  <tr>
                    <td class="AirportName">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ArrivalAirportName" />
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr class="HorizontalLine">
              <td></td>
              <td></td>
              <td></td>
              <td></td>
            </tr>
            <xsl:variable name="ReturnFlightCode" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/FlightCode" />
            <tr class="ReturnFlightRow">
              <td  class="FlightInfo">
                <table>
                  <tr>
                    <td>
                      <img src="{concat('https://sunway.ivector.co.uk/Content/Carriers/',QuoteDocumentationModel/FlightResult/CarrierLogo)}" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/CarrierName" />
                      <xsl:value-of select="$ReturnFlightCode" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ClassName" />
                    </td>
                  </tr>
                </table>
              </td>
              <td class="FlightDetailsDeparture">
                <table>
                  <tr>
                    <td class="b">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/DepartureTime" />
                    </td>
                  </tr>
                  <tr>
                    <td class="b">
                      <xsl:call-template name="ShortDate" >
                        <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/DepartureDate" />
                      </xsl:call-template>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/DepartureAirportCode" />
                    </td>
                  </tr>
                  <tr>
                    <td class="AirportName">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/DepartureAirportName" />
                    </td>
                  </tr>
                </table>
              </td>
              <td class="arrow">
                <table class="TravelArrow">
                  <tr>
                    <td>
                      <img src="../Images/arrow.png" width="150px" height="auto" />
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:choose >
                        <xsl:when test="count(QuoteDocumentationModel/FlightResult/FlightResult/FlightSectors/FlightSector[Direction='Outbound']) = 1" >
                          <xsl:value-of select="'Direct'" />
                        </xsl:when>
                        <xsl:otherwise >
                          <xsl:value-of select="concat(count(QuoteDocumentationModel/FlightResult/FlightSectors/FlightSector[Direction='Outbound']),' Stops')" />
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                  </tr>
                </table>
              </td>
              <td class="FlightDetailsArrival">
                <table>
                  <tr>
                    <td class="b">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/ArrivalTime" />
                    </td>
                  </tr>
                  <tr>
                    <td class="b">
                      <xsl:call-template name="ShortDate" >
                        <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/ArrivalDate" />
                      </xsl:call-template>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ArrivalAirportCode" />
                    </td>
                  </tr>
                  <tr>
                    <td class="AirportName">
                      <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ArrivalAirportName" />
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="TermsAndConditions" >
    <table id="TermsAndConditions">
      <tr class="TermsAndConditionsHeader">
        <td>Terms and Conditions</td>
      </tr>
      <tr class="TermsAndConditionsText">
        <td>
          <xsl:text >To proceed with your booking please contact us. Please note prices are subject to change depending on availability. All bookings are subject to our standard T&amp;C's which are on our website. E&amp;OE</xsl:text>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="GetStarRating" >
    <xsl:param name="StarRating" />
    <xsl:choose >
      <xsl:when test="$StarRating = '1.0'" >
        <i class="fas fa-star"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '1half'or $StarRating = '1.5'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star-half"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '2.0'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '2half'or $StarRating = '2.5'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star-half"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '3.0'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '3half' or $StarRating = '3.5'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star-half"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '4.0'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '4half'or $StarRating = '4.5'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star-half"></i>
      </xsl:when>
      <xsl:when test="$StarRating = '5.0'" >
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
        <i class="fas fa-star"></i>
      </xsl:when>
      <xsl:otherwise >&#160;</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template name="ExcludedIncluded">
    <xsl:param name="source" />
    <xsl:choose>
      <xsl:when test="$source = 'Own'">
        <xsl:text>Included</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>Excluded</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>


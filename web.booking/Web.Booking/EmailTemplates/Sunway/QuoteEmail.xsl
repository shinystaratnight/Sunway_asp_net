<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />
  <xsl:include href="xsl/markdown.xsl" />
  <xsl:include href="../../xsl/functions.xsl" />

  <xsl:template name="QuoteEmail" match="/" >
    <html >
      <head >
        <style >
          *{border-collapse:collapse;font-family:arial,helvetica,Sans-Serif;padding:0;margin:0;}
          #wrapper td{font-family:arial;}
          body{width:900px;}
          img{max-width:200px;}
          #wrapper{background-color:#eee;}
          #wrapper td.MainCell{padding-right:20px;padding-left:20px;padding-top:10px;}
          #AgentHeader,#FlightDetails,#PropertyInfoBox,#QuoteHeader,#TermsAndConditions,#components{width:900px;border:2px solid #dedede;background-color:#fff;}
          #AgentHeader .AgentAdditionalContact{width:400px;}
          #AgentHeader .AgentAdditionalContact td{text-align:right;font-size:16px;padding-right:8px;}
          #AgentHeader .AgentAdditionalContact .AgentAddress{font-size:18px;}
          #AgentHeader .AgentAdditionalContact{padding-top:10px;font-weight:700;}
          #AgentHeader .AgentName{vertical-align:text-top;border-collapse:collapse;width:500px;padding:0 0 20px 20px; font-size:30px;}
          #AgentHeader .AgentPhoneRow td{height:30px;margin-bottom:0;padding-bottom:0;}
          #AgentHeader .NumberCell{font-size:30px;text-align:right;padding-right:10px;padding-top:20px;vertical-align:bottom;}
          #AgentHeader .AgentEmail{color:#f9b568;}
          #AgentHeader {padding-bottom:20px;}

          #QuoteHeader .CostBoxContainer,#QuoteHeader .QuoteHeaderText{padding:20px;}
          #QuoteHeader td{vertical-align:center;color:#fff;}
          #QuoteHeader .CostBoxContainer{width:300px;}
          #QuoteHeader .CostBox{background-color:#228054;width:270px;}
          #QuoteHeader .QuoteHeaderText,#FlightDetails .FlightDetailsHeader,#components .ComponentsHeader{color:#228054;font-size:30px;}
          #QuoteHeader td.QuoteHeaderPrice{font-size:35px;text-align:right;padding-right:10px;font-weight:700;}
          #QuoteHeader td.QuoteHeaderLabelContainer{padding:10px;}
          #QuoteHeader table.QuoteHeaderLabel{width:100%;font-size:17px;border-collapse:collapse;}
          #QuoteHeader table.QuoteHeaderLabel.td{margin:5px;}
          #QuoteHeader td.QuoteHeaderLabelLower{vertical-align:text-top;padding:0;}

          #PropertyInfoBox .PropertyInfoImages{max-width:200px;}
          #PropertyInfoBox .PropertyInfoImages img {max-width:200px;}
          #PropertyInfoBox .PropertyInfoImages tr{margin-bottom:10px;}
          #PropertyInfoBox .PropertyInfoTextContainer{width:600px;}
          #PropertyInfoBox .PropertyInfoTextContainer table.td{padding:0;margin:0;}
          #PropertyInfoBox .PropertyInfoTextContainer table{width:100%;border-collapse:collapse;}
          #PropertyInfoBox .PropertyInfoTextContainer{vertical-align:text-top; padding:20px 20px 0 20px;}
          #PropertyInfoBox .PropertyInfoHeaderRow td,#TermsAndConditions .TermsAndConditionsHeader{color:#5e5e5e;}
          #PropertyInfoBox .PropertyInfoHeaderRow .PropertyInfoStarRating{width:300px;}
          #PropertyInfoBox h3,#PropertyInfoBox h4{color:#228054;font-size:20px;padding:0;margin:0;}
          #PropertyInfoBox .PropertyInfoTextContainer td{font-size:18px;color:#5e5e5e;}
          #PropertyInfoBox .PropertyInfoHeaderRow .PropertyInfoName{font-size:30px;white-space: nowrap;}
          #PropertyInfoBox .PropertyInfoImages>table,#PropertyInfoBox .PropertyInfoTextContainer{border-collapse:collapse;}
          #PropertyInfoBox .PropertyInfoImages{padding:20px 0 0 20px;}
          #PropertyInfoBox .PropertyInfoLocationRow{margin-bottom:10px;}
          #PropertyInfoBox .PropertyInfoImageContainer{max-width:200px;}
          
          #components{collapse-borders:collapse-borders;}
          #components .ComponentsHeader{padding:20px 20px 0;}
          #components td{padding:0;}
          #components .ComponentsTable{width:100%;}
          #components .ComponentsTable th{background-color:#e7edef;}
          #components .ComponentsTable tr td,#components .ComponentsTable tr th{padding:10px 0 20px;width:90px;text-align:left;}
          #components .ComponentsTable tr td.FirstCell,#components .ComponentsTable tr th.FirstCell{width:200px;padding-left:10px;}
          #components .ComponentsTableContainer{padding:10px 20px 20px;}

          #FlightDetails {padding:0;margin:0;}
          #FlightDetails table {background-color:#fff;width:100%;}
          #FlightDetails .FlightDetailsHeader{padding:20px 0 0 20px;}
          #FlightDetails tr{margin:0;padding:0;}
          #FlightDetails td{color:#5e5e5e;font-face:Arial;}
          #FlightDetails .FlightDetailsDeparture td{text-align:right;}
          #FlightDetails .FlightDetailsDeparture tr{width:100%;}
          #FlightDetails .FlightDetailsArrival td{text-align:left;}
          #FlightDetails .arrow{width:150px;text-align:center;}
          #FlightDetails .FlightDetailsDeparture{width:200px;}
          #FlightDetails .FlightDetailsDeparture table{width:100%;}
          #FlightDetails .FlightInfo{padding-left:20px;padding-top:20px;width:200px;text-align:center;}
          #FlightDetails .FlightInfo td{text-align:center;}
          #FlightDetails .b{font-weight:700;}
          #FlightDetails .AirportName{font-size:12px;padding-top:10px;}
          #FlightDetails .ReturnFlightRow{padding-bottom:15px;}
          #FlightDetails .HorizontalLine td{border-bottom:2px solid #f5f5f5; height:10px;}
          #FlightDetails .arrow .img { height:auto;}
          
          #TermsAndConditions {margin-bottom:20px;}
          #TermsAndConditions td{padding:none;border-collapse:collapse;}
          #TermsAndConditions .TermsAndConditionsHeader td{padding:20px 0 0 20px;}
          #TermsAndConditions .TermsAndConditionsText td{padding: 0 0 10px 20px;}
        </style>
      </head>
      <body style="width:900px;border-collapse:collapse;font-family:Arial,helvetica,Sans-Serif;padding:0;margin:0;color:#000;">
        <table id="wrapper" style="background-color:#eee;">
          <tr>
            <td class="MainCell" style="padding-right:20px;padding-left:20px;padding-top:10px;">
              <xsl:call-template name="AgentHeader" />
            </td>
          </tr>
          <tr>
            <td class="MainCell"  style="padding-right:20px;padding-left:20px;padding-top:10px;">
              <xsl:call-template name="QuoteHeader" />
            </td>
          </tr>
          <tr>
            <td class="MainCell"  style="padding-right:20px;padding-left:20px;padding-top:10px;">
              <xsl:call-template name="PropertyInfoBox" />
            </td>
          </tr>
          <tr>
            <td class="MainCell"  style="padding-right:20px;padding-left:20px;padding-top:10px;">
              <xsl:call-template name="components" />
            </td>
          </tr>
          <xsl:if test="QuoteDocumentationModel/FlightResult">
            <tr>
              <td class="MainCell"  style="padding-right:20px;padding-left:20px;padding-top:10px;">
                <xsl:call-template name="FlightDetails" />
              </td>
            </tr>
          </xsl:if>
          <tr>
            <td class="MainCell"  style="padding-right:20px;padding-left:20px;padding-top:10px;">
              <xsl:call-template name="TermsAndConditions" />
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="AgentHeader" >
    <xsl:param name="AgentInfo" />
    <table id="AgentHeader" style="width:900px;border:2px solid #dedede;background-color:#fff;padding-bottom:20px;">
      <tr class="AgentPhoneRow" >
        <td style="height:30px;margin-bottom:0;padding-bottom:0;"></td>
        <td class="NumberCell" style="height:30px;margin-bottom:0;padding-bottom:0;font-size:30px;text-align:right;padding-right:10px;padding-top:20px;vertical-align:bottom;">
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
        <td class="AgentName" style="vertical-align:text-top;border-collapse:collapse;width:500px;padding:0 0 20px 20px; font-size:30px;">
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
          <table class="AgentAdditionalContact" style="width:400px;padding-top:10px;font-weight:700;">
            <tr class="AgentEmail" style="color:#f9b568;">
              <td style="text-align:right;font-size:16px;padding-right:8px;">
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
    <table id="QuoteHeader" style="width:900px;border:2px solid #dedede;background-color:#fff;padding:0;margin:0;height:100px;">
      <tr>
        <td class="QuoteHeaderText" style="padding:20px;vertical-align:center;color:#fff;color:#228054;font-size:30px;">YOUR QUOTE</td>
        <td class="CostBoxContainer" style="padding:20px 0 20px 0;vertical-align:center;color:#fff;width:300px;">
          <table class="CostBox" style="background-color:#228054;width:270px;padding:0;max-height:60px;">
            <tr>
              <td class="QuoteHeaderLabelContainer" style="vertical-align:center;color:#fff;">
                <table class="QuoteHeaderLabel" style="width:100%;font-size:17px;border-collapse:collapse;">
                  <tr>
                    <td style="vertical-align:center;color:#fff;margin:5px;">Total Party</td>
                  </tr>
                  <tr class="QuoteHeaderLabelLower" style="vertical-align:text-top;padding:0;">
                    <td style="vertical-align:center;color:#fff;margin:5px;">From</td>
                  </tr>
                </table>
              </td>
              <td class="QuoteHeaderPrice" style="vertical-align:center;color:#fff;font-size:35px;text-align:right;padding-right:10px;font-weight:700;">
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
    <table id="PropertyInfoBox" style="width:900px;border:2px solid #dedede;background-color:#fff;">
      <tr>
        <td class="PropertyInfoImages" style="max-width:300px;padding:20px 0 0 20px;vertical-align:top;">
          <table style="border-collapse:collapse;">
            <xsl:for-each select="QuoteDocumentationModel/Property/Images/Image" >
              <xsl:if test="position() &lt;= 5">
                <tr style="margin-bottom:10px;">
                  <td class="PropertyInfoImageContainer" style="max-width:300px;">
                    <img src="{Source}" width="300" style="max-width:300px;"/>
                  </td>
                </tr>
              </xsl:if>
            </xsl:for-each>
          </table>
        </td>
        <td class="PropertyInfoTextContainer" style="width:600px;vertical-align:text-top; padding:20px 20px 0 20px;border-collapse:collapse;color:#5e5e5e;font-size:18px;">
          <table style="width:100%;border-collapse:collapse;">
            <tr class="PropertyInfoHeaderRow" >
              <td style="padding:0;margin:0;color:#5e5e5e;font-size:18px;">
                <table class="PropertyInfoHeaderContainer" style="width:100%;border-collapse:collapse;">
                  <tr>
                    <td class="PropertyInfoName" style="padding:0;margin:0;color:#5e5e5e;font-size:30px;white-space: nowrap;">
                      <xsl:value-of select="QuoteDocumentationModel/Property/Name" />
                    </td>
                    <td class="PropertyInfoStarRating" style="padding:0;margin:0;color:#5e5e5e;width:300px;font-size:18px;">
                      <xsl:call-template name="GetStarRating" >
                        <xsl:with-param name="StarRating" select="QuoteDocumentationModel/Property/Rating" />
                      </xsl:call-template>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr class="PropertyInfoLocationRow" style="margin-bottom:10px">
              <td style="padding:0;margin:0;">
                <xsl:value-of select="QuoteDocumentationModel/Property/Country" />
                <xsl:text >, </xsl:text>
                <xsl:value-of select="QuoteDocumentationModel/Property/Resort" />
              </td>
            </tr>
            <tr class="PropertyInfoDescriptionRow">
              <td style="padding:10px 0 0 0;margin:0;">
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
    <table id="components" style="width:900px;border:2px solid #dedede;background-color:#fff;collapse-borders:collapse-borders;">
      <tr>
        <td class="ComponentsHeader" style="padding:20px 20px 0;color:#228054;font-size:30px;">COMPONENTS</td>
      </tr>
      <tr>
        <td class="ComponentsTableContainer" style="padding:10px 20px 0 20px;width:90px;text-align:left;">
          <table class="ComponentsTable" style="width:100%;border-collapse:collapse;">
            <tr>
              <th class="FirstCell" style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;width:200px;padding-left:10px;">Room Type</th>
              <th style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;">Board Basis</th>
              <th style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;">Check-in</th>
              <th style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;">Duration</th>
              <th style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;">Baggage</th>
              <th style="background-color:#e7edef;padding:10px 0 20px;width:90px;text-align:left;">Transfers</th>
            </tr>
            <xsl:for-each select="QuoteDocumentationModel/QuoteDocumentationRoomOptions/QuoteDocumentationRoomOption" >
              <tr>
                <td class="FirstCell" style="padding:0;padding:10px 0 20px;width:90px;text-align:left;width:200px;padding-left:10px;">
                  <xsl:value-of select="RoomOption/RoomType" />
                </td>
                <td style="padding:0;padding:10px 0 20px;width:90px;text-align:left;">
                  <xsl:value-of select="MealBasis/Name" />
                </td>
                <td style="padding:0;padding:10px 0 20px;width:90px;text-align:left;">
                  <xsl:call-template name="ShortDate" >
                    <xsl:with-param name="SQLDate" select="ancestor::QuoteDocumentationModel/PropertyResult/ArrivalDate" />
                  </xsl:call-template>
                </td>
                <td style="padding:0;padding:10px 0 20px;width:90px;text-align:left;">
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
                <td style="padding:0;padding:10px 0 20px;width:90px;text-align:left;">
                  <xsl:call-template name="ExcludedIncluded">
                    <xsl:with-param name="source">
                      <xsl:value-of select="ancestor::QuoteDocumentationModel/FlightResult/FlightResult/Source"/>
                    </xsl:with-param>
                  </xsl:call-template>
                </td>
                <td style="padding:0;padding:10px 0 20px;width:90px;text-align:left;">
                  <xsl:call-template name="ExcludedIncluded">
                    <xsl:with-param name="source">
                      <xsl:value-of select="ancestor::QuoteDocumentationModel/FlightResult/FlightResult/Source"/>
                    </xsl:with-param>
                  </xsl:call-template>
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
    <table id="FlightDetails" style="width:900px;border:2px solid #dedede;background-color:#fff;padding:0;margin:0;">
      <tr style="margin:0;padding:0;">
        <td class="FlightDetailsHeader" style="padding:20px 0 0 20px;font-face:Arial;color:#228054;font-size:30px;">
          FLIGHT DETAILS
        </td>
      </tr>
      <tr style="margin:0;padding:0;">
        <table class="FlightRows" stlye="background-color:#fff;width:100%;">
          <tr class="OutboundFlightRow" style="margin:0;padding:0color:#5e5e5e;font-face:Arial;;">
            <td class="FlightInfo" style="padding-left:20px;padding-top:20px;width:200px;text-align:center;">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <img src="{concat('https://sunway.ivector.co.uk/Content/Carriers/',QuoteDocumentationModel/FlightResult/CarrierLogo)}" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/CarrierName" />
                    <xsl:value-of select="$OutboundFlightCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ClassName" />
                  </td>
                </tr>
              </table>
            </td>
            <td class="FlightDetailsDeparture" style="color:#5e5e5e;font-face:Arial;">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;width:100%;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:right;font-weight:700;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/DepartureTime" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;width:100%;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:right;font-weight:700;">
                    <xsl:call-template name="ShortDate" >
                      <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/DepartureDate" />
                    </xsl:call-template>
                  </td>
                </tr>
                <tr style="margin:0;padding:0;width:100%;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:right;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/DepartureAirportCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;width:100%;">
                  <td class="AirportName" style="color:#5e5e5e;font-face:Arial;text-align:right;font-size:12px;padding-top:10px;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/DepartureAirportName" />
                  </td>
                </tr>
              </table>
            </td>
            <td class="arrow" style="width:150px;text-align:center;">
              <table class="TravelArrow"  stye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;">
                    <img src="arrow.png" width="150px" height="auto" style="height:auto;" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;">
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
            <td class="FlightDetailsArrival" style="color:#5e5e5e;font-face:Arial;">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:left;font-weight:700;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/ArrivalTime" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:left;font-weight:700;">
                    <xsl:call-template name="ShortDate" >
                      <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/OutboundFlightDetails/ArrivalDate" />
                    </xsl:call-template>
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:left;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ArrivalAirportCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="AirportName" style="color:#5e5e5e;font-face:Arial;text-align:left;font-size:12px;padding-top:10px;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $OutboundFlightCode]/ArrivalAirportName" />
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr class="HorizontalLine" style="margin:0;padding:0;">
            <td style="border-bottom:2px solid #f5f5f5; height:10px;"></td>
            <td style="border-bottom:2px solid #f5f5f5; height:10px;"></td>
            <td style="border-bottom:2px solid #f5f5f5; height:10px;"></td>
            <td style="border-bottom:2px solid #f5f5f5; height:10px;"></td>
          </tr>
          <xsl:variable name="ReturnFlightCode" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/FlightCode" />
          <tr class="ReturnFlightRow" style="padding-bottom:15px;">
            <td  class="FlightInfo" style="color:#5e5e5e;font-face:Arial;padding-left:20px;padding-top:20px;width:200px;text-align:center;">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <img src="{concat('https://sunway.ivector.co.uk/Content/Carriers/',QuoteDocumentationModel/FlightResult/CarrierLogo)}" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/CarrierName" />
                    <xsl:value-of select="$ReturnFlightCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:center;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ClassName" />
                  </td>
                </tr>
              </table>
            </td>
            <td class="FlightDetailsDeparture" style="color:#5e5e5e;font-face:Arial;">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:right;font-weight:700;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/DepartureTime" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:right;font-weight:700;">
                    <xsl:call-template name="ShortDate" >
                      <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/DepartureDate" />
                    </xsl:call-template>
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:right;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/DepartureAirportCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="AirportName" style="color:#5e5e5e;font-face:Arial;text-align:right;font-size:12px;padding-top:10px;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/DepartureAirportName" />
                  </td>
                </tr>
              </table>
            </td>
            <td class="arrow" style="color:#5e5e5e;font-face:Arial;width:150px;text-align:center; ">
              <img src="arrow.png" width="150px" height="auto" style="height:auto;"/>
            </td>
            <td class="FlightDetailsArrival">
              <table  stlye="background-color:#fff;width:100%;">
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:left;font-weight:700;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/ArrivalTime" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="b" style="color:#5e5e5e;font-face:Arial;text-align:left;font-weight:700;">
                    <xsl:call-template name="ShortDate" >
                      <xsl:with-param name="SQLDate" select="QuoteDocumentationModel/FlightResult/FlightResult/ReturnFlightDetails/ArrivalDate" />
                    </xsl:call-template>
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td style="color:#5e5e5e;font-face:Arial;text-align:left;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ArrivalAirportCode" />
                  </td>
                </tr>
                <tr style="margin:0;padding:0;">
                  <td class="AirportName" style="color:#5e5e5e;font-face:Arial;text-align:left;font-size:12px;padding-top:10px;">
                    <xsl:value-of select="QuoteDocumentationModel/FlightResult/QuoteDocumentationSectorAdditionalInformation/QuoteDocumentationSectorAdditionalInformation[FlightCode = $ReturnFlightCode]/ArrivalAirportName" />
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="TermsAndConditions" >
    <table id="TermsAndConditions" style="width:900px;border:2px solid #dedede;background-color:#fff;margin-bottom:20px;padding: 10px 0 10px 20px;font-size:15px;">
      <tr class="TermsAndConditionsHeader" style="color:#5e5e5e;">
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

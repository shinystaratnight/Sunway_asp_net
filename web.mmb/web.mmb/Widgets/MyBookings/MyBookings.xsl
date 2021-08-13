<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <xsl:include href="../../xsl/markdown.xsl" />
  <xsl:include href="../../xsl/functions.xsl" />

  <xsl:param name="CurrencySymbol" />
  <xsl:param name="CurrencySymbolPosition" />
  <xsl:param name="CSSClassOverride" />
  <xsl:param name="SeatReservations" />
  <xsl:param name="SeatReservationsChanged" />
  <xsl:param name="LogoutRedirectURL" />
  <xsl:param name="Theme" />
  <xsl:param name="PaymentSuccessful" />
  <xsl:param name="PriceFormat" />
  <xsl:param name="ConsolidateAmendBookingFunctionality" select="/MyBookings/ManageMyBooking/ConsolidateAmendBookingFunctionality = 1" />
  <xsl:param name="Property" select="/MyBookings/GetBookingDetailsResponse/Properties/Property" />

  <xsl:template match="/">

    <xsl:if test="$PaymentSuccessful = 'True'">
      <input type="hidden" id="hidPaymentSuccessful" value="{$PaymentSuccessful}" />
      <input type="hidden" id="hidPaymentSuccessText" value="Your payment was successful." ml="My Booking" />
    </xsl:if>

    <input type="hidden" id="hidOutstandingAmount" value="{MyBookings/GetBookingDetailsResponse/TotalOutstanding}" />
    <input type="hidden" id="hidTotalPaid" value="{MyBookings/GetBookingDetailsResponse/TotalPaid}" />
    <input type="hidden" id="hidTotalPrice" value="{MyBookings/GetBookingDetailsResponse/TotalPrice}" />
    <input type="hidden" id="hidCurrencySymbol" value="{$CurrencySymbol}" />
    <input type="hidden" id="hidCurrencySymbolPosition" value="{$CurrencySymbolPosition}" />

    <div id="divMyBookings" class="clear">
      <xsl:if test="$CSSClassOverride != ''">
        <xsl:attribute name="class">
          <xsl:value-of select="$CSSClassOverride" />
        </xsl:attribute>
      </xsl:if>

      <input type="hidden" id="hidLogoutRedirectURL" value="{$LogoutRedirectURL}" />
      <input type="hidden" id="hidBookingReference" value="{MyBookings/GetBookingDetailsResponse/BookingReference}" />
      <input type="hidden" id="hidExternalReference" value="{MyBookings/GetBookingDetailsResponse/ExternalReference}" />

      <a href="javascript:void(0)" id="aLogOut" ml="My Booking">
        <xsl:choose>
          <xsl:when test ="/MyBookings/ManageMyBooking/LeaveManageMyBookingButtonText != ''">
            <xsl:value-of select="/MyBookings/ManageMyBooking/LeaveManageMyBookingButtonText" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>Leave Manage My Booking</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </a>

      <h1 ml="My Booking">Manage My Booking</h1>

      <div class="optionalParagraph">
        <xsl:call-template name="Markdown">
          <xsl:with-param name="text" select="MyBookings/ManageMyBooking/MainText" />
        </xsl:call-template>
        <xsl:text> </xsl:text>
      </div>

      <xsl:if test="MyBookings/GetBookingDetailsResponse/TotalOutstanding &gt; 0">
        <div class="paymentDetails" id="divPaymentDetailsHolder" style="display:none;">
          <xsl:text> </xsl:text>
        </div>
      </xsl:if>

      <xsl:for-each select="MyBookings/GetBookingDetailsResponse">

        <div class="booking" id="divBooking_{BookingReference}">

          <h2 class="bookingRef">
            <trans ml="My Booking">Booking Details</trans>
            <xsl:text> (</xsl:text>
            <trans ml="My Booking">Ref</trans>
            <xsl:text> </xsl:text>
            <xsl:value-of select="BookingReference" />
            <xsl:text>)</xsl:text>
          </h2>

          <div class="LeadGuest section">

            <dl>
              <dt>
                <trans ml="My Booking">Name:</trans>
              </dt>
              <dd>
                <xsl:value-of select="LeadCustomer/CustomerTitle"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="LeadCustomer/CustomerFirstName"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="LeadCustomer/CustomerLastName"/>
              </dd>
              <xsl:if test="LeadCustomer/DateOfBirth != '' and LeadCustomer/DateOfBirth != '1900-01-01T00:00:00.001'">
                <dt>
                  <trans ml="My Booking">Date of Birth:</trans>
                </dt>
                <dd>
                  <xsl:call-template name="FullDate">
                    <xsl:with-param name="SQLDate" select="LeadCustomer/DateOfBirth" />
                  </xsl:call-template>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <dt class="address">
                <trans ml="My Booking">Address:</trans>
              </dt>
              <dd class="address">
                <xsl:value-of select="LeadCustomer/CustomerAddress1"/>
                <xsl:text> </xsl:text>
              </dd>
              <xsl:if test="LeadCustomer/CustomerAddress2 != ''">
                <dt class="address blank">
                  <xsl:text> </xsl:text>
                </dt>
                <dd class="address">
                  <xsl:value-of select="LeadCustomer/CustomerAddress2"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <xsl:if test="LeadCustomer/CustomerTownCity != ''">
                <dt class="address blank">
                  <xsl:text> </xsl:text>
                </dt>
                <dd class="address">
                  <xsl:value-of select="LeadCustomer/CustomerTownCity"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <xsl:if test="LeadCustomer/CustomerCounty != ''">
                <dt class="address blank">
                  <xsl:text> </xsl:text>
                </dt>
                <dd class="address">
                  <xsl:value-of select="LeadCustomer/CustomerCounty"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <dt class="blank">
                <xsl:text> </xsl:text>
              </dt>
              <dd>
                <xsl:value-of select="LeadCustomer/CustomerPostcode"/>
                <xsl:text> </xsl:text>
              </dd>
              <xsl:if test="LeadCustomer/CustomerPhone != ''">
                <dt>
                  <trans ml="My Booking">Phone:</trans>
                </dt>
                <dd>
                  <xsl:value-of select="LeadCustomer/CustomerPhone"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <xsl:if test="LeadCustomer/CustomerMobile != ''">
                <dt>
                  <trans ml="My Booking">Mobile:</trans>
                </dt>
                <dd>
                  <xsl:value-of select="LeadCustomer/CustomerMobile"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <xsl:if test="LeadCustomer/CustomerFax != ''">
                <dt>
                  <trans ml="My Booking">Fax:</trans>
                </dt>
                <dd>
                  <xsl:value-of select="LeadCustomer/CustomerFax"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
              <xsl:if test="LeadCustomer/CustomerEmail != ''">
                <dt>
                  <trans ml="My Booking">Email:</trans>
                </dt>
                <dd>
                  <xsl:value-of select="LeadCustomer/CustomerEmail"/>
                  <xsl:text> </xsl:text>
                </dd>
              </xsl:if>
            </dl>

            <a href="#" class="button primary" id="aSendDocumentation" ml="My Booking">
              <xsl:choose>
                <xsl:when test="../ManageMyBooking/ResendDocButtonText != ''">
                  <xsl:value-of select="../ManageMyBooking/ResendDocButtonText"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>Re-send Booking Confirmation</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
            </a>
          </div>

          <xsl:if test="count(Properties/Property[Status != 'Cancelled']) &gt; 0">
            <div class="property">
              <h2 class="leftBorder">
                <trans ml="My Booking">Your Accommodation</trans>
              </h2>

              <div class="componentDetails section">
                <xsl:for-each select="Properties/Property[Status != 'Cancelled']">
                  <div class="clear property">
                    <div class="mainImage">
                      <xsl:if test="MainImage != ''">
                        <img alt="{Name}" src="{MainImage}" class="mainImage"/>
                      </xsl:if>
                      <xsl:text> </xsl:text>
                    </div>

                    <div class="details">
                      <h3>
                        <xsl:value-of select="Name"/>
                      </h3>

                      <xsl:call-template name="StarRating">
                        <xsl:with-param name="Rating" select="Rating" />
                      </xsl:call-template>

                      <xsl:if test="Status != 'Cancelled' and $ConsolidateAmendBookingFunctionality != 'True' and ../../../ManageMyBooking/DisplayAmendBookingButton = 1">
                        <div class="sidebarBox primary">
                          <div class="actionLinks" id="divActionLinks_{PropertyBookingReference}">
                            <xsl:choose>
                              <xsl:when test="../../../ManageMyBooking/AllowCancellations = 1">
                                <a href="javascript:void(0)" onclick="MyBookings.ShowAmendmentPopup('{PropertyBookingReference}');return false;" ml="My Booking">Amend / Cancel Booking Det</a>
                              </xsl:when>
                              <xsl:otherwise>
                                <a href="javascript:void(0)" onclick="MyBookings.ShowAmendmentPopup('{PropertyBookingReference}');return false;" ml="My Booking">Amend Booking Details</a>
                              </xsl:otherwise>
                            </xsl:choose>
                          </div>
                        </div>
                        <input type="hidden" id="hidAmendData_{PropertyBookingReference}" value="{Name}#{ArrivalDate}#{ReturnDate}#{Resort}"/>
                      </xsl:if>

                      <div class="clear">
                        <xsl:text> </xsl:text>
                      </div>

                      <dl class="clear">
                        <dt>
                          <trans ml="My Booking">Rooms</trans>
                          <xsl:text>:</xsl:text>
                        </dt>
                        <xsl:for-each select="Rooms/Room">
                          <dd>
                            <xsl:attribute name="class">
                              <xsl:text> rooms </xsl:text>
                              <xsl:if test="position() != 1">
                                multi
                              </xsl:if>
                            </xsl:attribute>
                            <xsl:value-of select="RoomType" />
                            <xsl:text> - </xsl:text>
                            <xsl:value-of select="MealBasis" />
                          </dd>
                          <xsl:variable name="GuestCount" select="count(GuestDetails/GuestDetail)"/>
                          <xsl:if test="$GuestCount &gt; 0">
                            <dt class="blank">
                              <xsl:text> </xsl:text>
                            </dt>
                            <dd class="roomguests">
                              <xsl:text>(</xsl:text>
                              <xsl:for-each select="GuestDetails/GuestDetail">
                                <xsl:value-of select="Title"/>
                                <xsl:text> </xsl:text>
                                <xsl:value-of select="FirstName"/>
                                <xsl:text> </xsl:text>
                                <xsl:value-of select="LastName"/>
                                <xsl:if test="position() &lt; $GuestCount">
                                  <xsl:text> , </xsl:text>
                                </xsl:if>
                              </xsl:for-each>
                              <xsl:text>)</xsl:text>
                            </dd>
                          </xsl:if>
                        </xsl:for-each>
                        <xsl:if test="Resort != ''">
                          <dt>
                            <trans ml="My Booking">Location</trans>
                            <xsl:text>:</xsl:text>
                          </dt>
                          <dd>
                            <xsl:value-of select="Resort" />
                          </dd>
                        </xsl:if>
                        <dt>
                          <trans ml="My Booking">Check In Date</trans>
                          <xsl:text>:</xsl:text>
                        </dt>
                        <dd class="checkIn">
                          <xsl:call-template name="ShortDate">
                            <xsl:with-param name="SQLDate" select="ArrivalDate" />
                          </xsl:call-template>
                        </dd>
                        <dt>
                          <trans ml="My Booking">Check Out Date</trans>
                          <xsl:text>:</xsl:text>
                        </dt>
                        <dd>
                          <xsl:call-template name="ShortDate">
                            <xsl:with-param name="SQLDate" select="ReturnDate" />
                          </xsl:call-template>
                        </dd>
                        <dt>
                          <trans ml="My Booking">Number Of Nights</trans>
                          <xsl:text>:</xsl:text>
                        </dt>
                        <dd>
                          <xsl:value-of select="Duration" />
                        </dd>
                      </dl>
                    </div>
                  </div>
                </xsl:for-each>


                <div class="clear">
                  <xsl:text> </xsl:text>
                </div>
              </div>
            </div>
          </xsl:if>

          <xsl:if test="count(Flights/Flight[Status != 'Cancelled']) &gt; 0">

            <div class="flight section">
              <h2 class="leftBorder">
                <trans ml="My Booking">Your Flights</trans>
              </h2>

              <div class="componentDetails">
                <xsl:for-each select="Flights/Flight[Status != 'Cancelled']">
                  <xsl:choose>
                    <xsl:when test="count(FlightSectors/FlightSector[Direction='Outbound']) &gt; 0">
                      <xsl:call-template name="FlightSectorFlight" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:call-template name="NonFlightSectorFlight" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:for-each>

                <xsl:if test="/MyBookings/ManageMyBooking/ShowSubmitPassportInformationButton = 1 and count(Flights/Flight[Status != 'Cancelled']) &gt; 0">
                  <div id="divShowApis">
                    <a class="button primary" id="btnShowApis" href="javascript:void(0)" ml="My Booking">
                      Submit Passport Information
                    </a>
                  </div>
                </xsl:if>
              </div>
            </div>
          </xsl:if>

          <xsl:if test="count(Transfers/Transfer[Status != 'Cancelled']) &gt; 0">
            <div class="transfer section">
              <h2 class="leftBorder">
                <trans ml="My Booking">Your Transfers</trans>
              </h2>

              <div class="componentDetails">
                <xsl:for-each select="Transfers/Transfer[Status != 'Cancelled']">

                  <div class="transfer outbound">
                    <h3>
                      <xsl:if test="DepartureParentName != ''">
                        <xsl:value-of select="DepartureParentName" />
                      </xsl:if>
                      <xsl:if test="DepartureParentName = ''">
                        <xsl:value-of select="OutboundDetails/JourneyOrigin" />
                      </xsl:if>
                      <xsl:text> - </xsl:text>
                      <xsl:value-of select="ArrivalParentName" />
                    </h3>
                    <div class="vehicle">
                      <span ml="My Booking" mlparams="{Vehicle}">
                        Vehicle {0}
                      </span>
                    </div>
                    <div class="small">
                      <span ml="My Booking">Flight Code</span>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="DepartureFlightCode" />
                    </div>
                    <div class="small">
                      <span ml="My Booking">Flight Time</span>
                      <xsl:text> </xsl:text>
                      <xsl:call-template name="ShortDate">
                        <xsl:with-param name="SQLDate" select="DepartureDate" />
                      </xsl:call-template>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="DepartureTime" />
                    </div>
                  </div>

                  <xsl:if test="OneWay = 'false'">
                    <div class="transfer return">
                      <h3>
                        <xsl:value-of select="ArrivalParentName" />
                        <xsl:text> - </xsl:text>
                        <xsl:if test="DepartureParentName != ''">
                          <xsl:value-of select="DepartureParentName" />
                        </xsl:if>
                        <xsl:if test="DepartureParentName = ''">
                          <xsl:value-of select="OutboundDetails/JourneyOrigin" />
                        </xsl:if>
                      </h3>
                      <div class="vehicle">
                        <span ml="My Booking" mlparams="{Vehicle}">
                          Vehicle {0}
                        </span>
                      </div>
                      <div class="small">
                        <span ml="My Booking">Flight Code</span>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="ReturnFlightCode" />
                      </div>
                      <div class="small">
                        <span ml="My Booking">Flight Time</span>
                        <xsl:text> </xsl:text>
                        <xsl:call-template name="ShortDate">
                          <xsl:with-param name="SQLDate" select="ReturnDate" />
                        </xsl:call-template>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="ReturnTime"/>
                      </div>
                    </div>
                  </xsl:if>

                  <div class="clear">
                    <xsl:text> </xsl:text>
                  </div>
                </xsl:for-each>
              </div>
            </div>
          </xsl:if>

          <xsl:if test="count(CarHireBookings/CarHireBooking[Status != 'Cancelled']) &gt; 0">
            <div class="carhire section">
              <h2 class="leftBorder">
                <trans ml="My Booking">Your Car Hire</trans>
              </h2>

              <div class="componentDetails">
                <xsl:for-each select="CarHireBookings/CarHireBooking[Status != 'Cancelled']">
                  <div class="carhireDetails">
                    <table>
                      <tr class="top">
                        <th class="left">
                          <xsl:text> </xsl:text>
                        </th>
                        <th>Date</th>
                        <th>Time</th>
                        <th>Location</th>
                      </tr>
                      <tr class="pickup">
                        <th class="left">Pick Up</th>
                        <td>
                          <xsl:call-template name="ShortDate">
                            <xsl:with-param name="SQLDate" select="PickUpDate" />
                          </xsl:call-template>
                        </td>
                        <td>
                          <xsl:value-of select="PickUpTime"/>
                        </td>
                        <td>
                          <xsl:value-of select="PickUpDepotName"/>
                        </td>
                      </tr>
                      <tr class="dropoff">
                        <th class="left">Drop Off</th>
                        <td>
                          <xsl:call-template name="ShortDate">
                            <xsl:with-param name="SQLDate" select="DropOffDate" />
                          </xsl:call-template>
                        </td>
                        <td>
                          <xsl:value-of select="DropOffTime"/>
                        </td>
                        <td>
                          <xsl:value-of select="DropOffDepotName"/>
                        </td>
                      </tr>
                      <tr class="bottom">
                        <th class="left">Car Type</th>
                        <td colspan="3">
                          <xsl:value-of select="VehicleDescription"/>
                        </td>
                      </tr>
                    </table>
                  </div>
                </xsl:for-each>
              </div>
            </div>
          </xsl:if>

          <xsl:if test="count(Extras/Extra[Status != 'Cancelled' and ExtraName != '']) &gt; 0">
            <div class="extra section">
              <h2 class="leftBorder">
                <trans ml="My Booking">Your Extras</trans>
              </h2>

              <xsl:variable name="ExpiryExists">
                <xsl:value-of select="count(Extras/Extra[Status != 'Cancelled' and (Expiry != StartDate and (Expiry != '1900-01-01T00:00:00.001' and Expiry != '0001-01-01T00:00:00')) or
										(Expiry = StartDate and StartTime != '' and EndTime != '' and StartTime != EndTime)]) &gt; 0"/>
              </xsl:variable>

              <div class="componentDetails">
                <table class="extraDetails">
                  <tr class="top">
                    <th class="left">
                      <xsl:text> </xsl:text>
                    </th>
                    <th>
                      <xsl:if test="$ExpiryExists = 'true'">
                        <xsl:attribute name="colspan">
                          <xsl:text>2</xsl:text>
                        </xsl:attribute>
                      </xsl:if>
                      <xsl:text>Date</xsl:text>
                    </th>
                    <th>Details</th>
                  </tr>

                  <xsl:for-each select="Extras/Extra[Status != 'Cancelled' and ExtraName != '']">

                    <xsl:variable name="HasExpiry">
                      <xsl:value-of select="(Expiry != StartDate and (Expiry != '1900-01-01T00:00:00.001' and Expiry != '0001-01-01T00:00:00')) or
																	(Expiry = StartDate and StartTime != '' and EndTime != '' and StartTime != EndTime)"/>
                    </xsl:variable>

                    <tr>
                      <xsl:attribute name="class">
                        <xsl:text>extra </xsl:text>
                        <xsl:if test="position() = last()">
                          <xsl:text> bottom </xsl:text>
                        </xsl:if>
                        <xsl:if test="$HasExpiry = 'true'">
                          <xsl:text> expires </xsl:text>
                        </xsl:if>
                      </xsl:attribute>

                      <th class="left">
                        <xsl:value-of select="ExtraType"/>
                        <xsl:text> </xsl:text>
                      </th>
                      <xsl:if test="$ExpiryExists = 'true'">
                        <td class="small">
                          <xsl:if test="$HasExpiry = 'true'">
                            From
                          </xsl:if>
                          <xsl:text> </xsl:text>
                        </td>
                      </xsl:if>
                      <td class="date">
                        <xsl:call-template name="ShortDate">
                          <xsl:with-param name="SQLDate" select="StartDate"/>
                        </xsl:call-template>
                        <xsl:if test="StartTime != ''">
                          <xsl:value-of select="concat(', ', StartTime)"/>
                        </xsl:if>
                      </td>
                      <td>
                        <xsl:value-of select="ExtraName"/>
                      </td>
                    </tr>

                    <xsl:if test="$HasExpiry = 'true'">
                      <tr class="expires">
                        <td class="blank">
                          <xsl:text> </xsl:text>
                        </td>
                        <td class="small">To</td>
                        <td class="date">
                          <xsl:call-template name="ShortDate">
                            <xsl:with-param name="SQLDate" select="Expiry"/>
                          </xsl:call-template>
                          <xsl:if test="EndTime != ''">
                            <xsl:value-of select="concat(', ', EndTime)"/>
                          </xsl:if>
                        </td>
                        <td>
                          <xsl:if test="ExtraType = 'Airport Parking'">
                            <xsl:value-of select="concat('Car Registration ', CarParkInformation/CarRegistration)"/>
                          </xsl:if>
                        </td>
                      </tr>
                    </xsl:if>
                  </xsl:for-each>
                </table>
              </div>
            </div>
          </xsl:if>

          <xsl:if test="count(AdHocBookings/AdHocBooking[Status != 'Cancelled']) &gt; 0">
            <xsl:for-each select="AdHocBookings/AdHocBooking[Status != 'Cancelled']">
              <div class="adhoc section">
                <h2 class="leftBorder">
                  <xsl:value-of select="Description"/>
                </h2>

                <div class="componentDetails adhoc clear">
                  <xsl:if test="UseStartDate != '1900-01-01T00:00:00.001' and UseStartDate != '0001-01-01T00:00:00'">
                    <h3 class="date">
                      <xsl:call-template name="ShortDate">
                        <xsl:with-param name="SQLDate" select="UseStartDate"/>
                      </xsl:call-template>
                      <xsl:if test="UseEndDate != UseStartDate and (UseEndDate != '1900-01-01T00:00:00.001' and UseEndDate != '0001-01-01T00:00:00')">
                        <trans ml="My Booking"> to </trans>
                        <xsl:call-template name="ShortDate">
                          <xsl:with-param name="SQLDate" select="UseEndDate"/>
                        </xsl:call-template>
                      </xsl:if>
                    </h3>
                  </xsl:if>

                  <div class="left mainDetails">
                    <ul>
                      <xsl:if test="SubDescription != ''">
                        <li class="subdescription">
                          <xsl:value-of select="SubDescription"/>
                        </li>
                      </xsl:if>
                      <xsl:if test="sum(Adults | Children | Infants) &gt; 0">
                        <li class="passenger">
                          <xsl:if test="Adults &gt; 0">
                            <xsl:value-of select="concat(Adults, ' Adult')"/>
                            <xsl:if test="Adults &gt; 1">
                              <xsl:text>s</xsl:text>
                            </xsl:if>
                          </xsl:if>
                          <xsl:if test="Children &gt; 0">
                            <xsl:if test="Adults &gt; 0">
                              <xsl:text>, </xsl:text>
                            </xsl:if>
                            <xsl:value-of select="concat(Children, ' Child')"/>
                            <xsl:if test="Children &gt; 1">
                              <xsl:text>s</xsl:text>
                            </xsl:if>
                          </xsl:if>
                          <xsl:if test="Infants &gt; 0">
                            <xsl:if test="sum(Adults | Children) &gt; 0">
                              <xsl:text>, </xsl:text>
                            </xsl:if>
                            <xsl:value-of select="concat(Infants, ' Infant')"/>
                            <xsl:if test="Infants &gt; 1">
                              <xsl:text>s</xsl:text>
                            </xsl:if>
                          </xsl:if>
                        </li>
                      </xsl:if>
                    </ul>
                  </div>
                  <xsl:if test="count(AdHocCustoms/AdHocCustom) &gt; 0">
                    <div class="right customDetails">
                      <table>
                        <xsl:for-each select="AdHocCustoms/AdHocCustom">
                          <tr>
                            <xsl:attribute name="class">
                              <xsl:text>custom </xsl:text>
                              <xsl:choose>
                                <xsl:when test="position() = 1">
                                  <xsl:text> top</xsl:text>
                                </xsl:when>
                                <xsl:when test="position() = last">
                                  <xsl:text> bottom</xsl:text>
                                </xsl:when>
                              </xsl:choose>
                            </xsl:attribute>
                            <td>
                              <xsl:value-of select="CustomFieldName"/>
                            </td>
                            <td>
                              <xsl:value-of select="CustomValue"/>
                            </td>
                          </tr>
                        </xsl:for-each>
                      </table>
                    </div>
                  </xsl:if>
                </div>
              </div>
            </xsl:for-each>
          </xsl:if>

          <xsl:if test="Status='Cancelled'">
            <div class="cancelledBooking" ml="My Booking">This booking has been cancelled</div>
          </xsl:if>

          <xsl:if test="Status != 'Cancelled' and $ConsolidateAmendBookingFunctionality">
            <div class="amendBooking">
              <div class="actionLinks" id="divActionLinks_{$Property/PropertyBookingReference}">
                <xsl:choose>
                  <xsl:when test="../../../ManageMyBooking/AllowCancellations = 1">
                    <a href="javascript:void(0)" onclick="MyBookings.ShowAmendmentPopup('{$Property/PropertyBookingReference}');return false;" ml="My Booking">Amend / Cancel Booking Det</a>
                  </xsl:when>
                  <xsl:otherwise>
                    <a href="javascript:void(0)" onclick="MyBookings.ShowAmendmentPopup('{$Property/PropertyBookingReference}');return false;" ml="My Booking">Amend Booking Details</a>
                  </xsl:otherwise>
                </xsl:choose>
              </div>
            </div>
            <input type="hidden" id="hidAmendData_{$Property/PropertyBookingReference}" value="{$Property/Name}#{$Property/ArrivalDate}#{$Property/ReturnDate}#{$Property/Resort}"/>
          </xsl:if>

          <xsl:if test="Status!='Cancelled'">
            <div class="paymentInformation">
              <div class="totalprice clear" id="divTotalPrice_{BookingReference}">
                <span ml="My Booking" class="left">Total Price</span>
                <span>
                  <xsl:call-template name="GetSellingPrice">
                    <xsl:with-param name="Value" select="TotalPrice" />
                    <xsl:with-param name="Currency" select="$CurrencySymbol" />
                    <xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
                    <xsl:with-param name="SeparateDecimals" select="'True'" />
                  </xsl:call-template>
                </span>
              </div>

              <div class="totalprice clear" id="divTotalPaid_{BookingReference}">
                <span ml="My Booking" class="left">Total Paid</span>
                <span id="spnTotalPaid_{BookingReference}">
                  <xsl:call-template name="GetSellingPrice">
                    <xsl:with-param name="Value" select="TotalPaid" />
                    <xsl:with-param name="Currency" select="$CurrencySymbol" />
                    <xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
                    <xsl:with-param name="SeparateDecimals" select="'True'" />
                  </xsl:call-template>
                </span>
              </div>

              <div class="totalprice clear" id="divTotalOutstanding_{BookingReference}">
                <span ml="My Booking" class="left">Total Outstanding</span>
                <span id="spnTotalOutstanding_{BookingReference}">
                  <xsl:call-template name="GetSellingPrice">
                    <xsl:with-param name="Value" select="TotalOutstanding" />
                    <xsl:with-param name="Currency" select="$CurrencySymbol" />
                    <xsl:with-param name="CurrencySymbolPosition" select="CurrencySymbolPosition" />
                    <xsl:with-param name="SeparateDecimals" select="'True'" />
                  </xsl:call-template>
                </span>
              </div>

              <xsl:if test="TotalOutstanding != 0">
                <a class="button primary icon chevron-right balance" ml="My Booking" id="btnPayBalanceShow" href="javascript:void(0)">
                  <xsl:choose>
                    <xsl:when test="/MyBookings/ManageMyBooking/PayBalanceButtonText">
                      <xsl:value-of select="/MyBookings/ManageMyBooking/PayBalanceButtonText" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:text>Pay Balance</xsl:text>
                    </xsl:otherwise>
                  </xsl:choose>
                </a>
              </xsl:if>



            </div>
          </xsl:if>
        </div>
      </xsl:for-each>

      <div id="divEmailDocuments" class="action" style="display:none;">

        <h1 ml="My Booking">Email Documents</h1>

        <!-- Wait Message -->
        <div id="divEmailWaitMessage">
          <img class="spinner" src="/themes/{$Theme}/images/loading.gif" alt="loading..." />
          <p ml="My Booking">Please wait as we process your request.</p>
        </div>

        <!-- Request Sent -->
        <div id="divEmailSuccess" ml="My Booking" style="display:none;">
          <xsl:text>Thank You. Your documents have been sent successfully.</xsl:text>
        </div>

      </div>

    </div>

    <div id="divAmendment" class="action clear" style="display:none;">

      <h1 ml="My Booking">Booking Amendment / Cancellation</h1>

      <xsl:choose>
        <xsl:when test="$ConsolidateAmendBookingFunctionality">
          <!-- Request Form -->
          <div id="divAmendmentForm">

            <p ml="My Booking">
              <xsl:call-template name="Markdown">
                <xsl:with-param name="text" select="MyBookings/ManageMyBooking/AmendBookingText" />
              </xsl:call-template>
              <xsl:text> </xsl:text>
            </p>

            <div id="divAmendmentWarning" class="warning infobox" style="display:none;">
              <xsl:value-of select="MyBookings/ManageMyBooking/AmendBookingWarning" />
              <xsl:text> </xsl:text>
            </div>


            <div id="divAmendmentInputs" class="form">

              <div class="left">

                <xsl:choose>
                  <xsl:when test="count(/MyBookings/GetBookingDetailsResponse/Properties/Property) &gt; 1">
                    <div class="divhotelname">
                      <label class="hotelname" ml="My Booking">Hotel Name</label>
                      <div class="custom-select">
                        <select id="ddlHotelName" value="{$Property/PropertyBookingReference}" onchange="MyBookings.ChangePropertyDetails(event);">

                          <xsl:for-each select="$Property">
                            <option id="Property_{PropertyBookingReference}" value="{PropertyBookingReference}" >
                              <xsl:value-of select="Name"/>
                            </option>
                          </xsl:for-each>
                        </select>
                      </div>
                      <xsl:for-each select="$Property">
                        <input type="hidden" id="hidProperty_{PropertyBookingReference}" value="{Name}#{ArrivalDate}#{ReturnDate}#{Resort}"/>
                      </xsl:for-each>

                      <span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your hotel.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
														onmouseout="f.HidePopup();">
                        <xsl:text> </xsl:text>
                      </span>
                    </div>
                  </xsl:when>
                  <xsl:otherwise>
                    <div class="divhotelname">
                      <label class="hotelname" ml="My Booking">Hotel Name</label>
                      <div class="">
                        <input type="text" id="txtHotelName" class="textbox" />
                        <span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your hotel.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
                              onmouseout="f.HidePopup();">
                          <xsl:text> </xsl:text>
                        </span>
                      </div>
                    </div>
                  </xsl:otherwise>
                </xsl:choose>


                <label class="checkin" ml="My Booking">Check In Date</label>
                <div class="textbox icon right embedded">
                  <i class="checkin">
                    <xsl:text> </xsl:text>
                  </i>
                  <input id="txtAmendmentDepartureDate" name="txtAmendmentDepartureDate" type="text" class="textbox" />
                </div>

                <label class="checkout" ml="My Booking">Check Out Date</label>
                <div class="textbox icon right embedded">
                  <i class="checkout">
                    <xsl:text> </xsl:text>
                  </i>
                  <input id="txtAmendmentReturnDate" name="txtAmendmentReturnDate" type="text" class="textbox" />
                </div>

                <label class="destination" ml="My Booking">Destination</label>
                <div class="">
                  <input type="text" id="txtDestination" class="textbox" />
                  <span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your destination.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
												onmouseout="f.HidePopup();">
                    <xsl:text> </xsl:text>
                  </span>
                </div>
              </div>

              <div class="left end">
                <div class="bottom">
                  <label class="amendment" ml="My Booking">Additional Information</label>
                  <textarea id="txtAdditionalInformation" rows="4">
                    <xsl:text> </xsl:text>
                  </textarea>
                </div>
              </div>
              <div class="clear">
                <xsl:text> </xsl:text>
              </div>
            </div>

            <div class="confirm">
              <a class="button tertiary" onclick="MyBookings.RequestAmendmentValidate();" ml="My Booking">
                Confirm
              </a>
            </div>
          </div>
        </xsl:when>
        <xsl:otherwise>
          <!-- Request Form -->
          <div id="divAmendmentForm">

            <p ml="My Booking">
              <xsl:call-template name="Markdown">
                <xsl:with-param name="text" select="MyBookings/ManageMyBooking/AmendBookingText" />
              </xsl:call-template>
              <xsl:text> </xsl:text>
            </p>

            <div id="divAmendmentWarning" class="warning infobox" style="display:none;">
              <xsl:value-of select="MyBookings/ManageMyBooking/AmendBookingWarning" />
              <xsl:text> </xsl:text>
            </div>


            <div id="divAmendmentInputs" class="form">

              <div class="left">

                <label class="checkin" ml="My Booking">Check In Date</label>
                <div class="textbox icon right embedded">
                  <i class="checkin">
                    <xsl:text> </xsl:text>
                  </i>
                  <input id="txtAmendmentDepartureDate" name="txtAmendmentDepartureDate" type="text" class="textbox" />
                </div>

                <label class="checkout" ml="My Booking">Check Out Date</label>
                <div class="textbox icon right embedded">
                  <i class="checkout">
                    <xsl:text> </xsl:text>
                  </i>
                  <input id="txtAmendmentReturnDate" name="txtAmendmentReturnDate" type="text" class="textbox" />
                </div>

                <label class="destination" ml="My Booking">Destination</label>
                <div class="">
                  <input type="text" id="txtDestination" class="textbox" />
                  <span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your destination.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
                    onmouseout="f.HidePopup();">
                    <xsl:text> </xsl:text>
                  </span>
                </div>

                <label class="hotelname" ml="My Booking">Hotel Name</label>
                <div class="">
                  <input type="text" id="txtHotelName" class="textbox" />
                  <span class="icon info" onmouseover="f.ShowPopup(this, 'popup', '&lt;span class=&quot;top&quot;&gt;&lt;/span&gt;&lt;p&gt;Leave blank if you do not wish to change your hotel.&lt;/p&gt;&lt;span class=&quot;bottom&quot;&gt;&lt;/span&gt;','',false,25,-105)"
                    onmouseout="f.HidePopup();">
                    <xsl:text> </xsl:text>
                  </span>
                </div>
              </div>

              <div class="left end">
                <div class="bottom">
                  <label class="amendment" ml="My Booking">Additional Information</label>
                  <textarea id="txtAdditionalInformation" rows="4">
                    <xsl:text> </xsl:text>
                  </textarea>
                </div>
              </div>
              <div class="clear">
                <xsl:text> </xsl:text>
              </div>
            </div>

            <div class="confirm">
              <a class="button tertiary" onclick="MyBookings.RequestAmendmentValidate();" ml="My Booking">
                Confirm
              </a>
            </div>
          </div>
        </xsl:otherwise>
      </xsl:choose>



      <!-- Wait Message -->
      <div id="divAmendRequestWaitMessage" style="display:none;">
        <p ml="My Booking">Please wait as we send through your request.</p>
        <img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
      </div>

      <!-- Request Sent -->
      <div id="divRequestSent" style="display:none;">
        <xsl:call-template name="Markdown">
          <xsl:with-param name="text" select="MyBookings/ManageMyBooking/AmendBookingSentText" />
        </xsl:call-template>
        <xsl:text> </xsl:text>
      </div>

      <!-- Request Not Sent -->
      <div id="divRequestNotSent" style="display:none;">
        <xsl:call-template name="Markdown">
          <xsl:with-param name="text" select="MyBookings/ManageMyBooking/AmendBookingNotSentText" />
        </xsl:call-template>
        <xsl:text> </xsl:text>
      </div>

      <div id="div3DSRedirect">
        <xsl:text> </xsl:text>
      </div>

      <xsl:text> </xsl:text>
    </div>
  </xsl:template>


  <xsl:template name="FlightSectorFlight">
    <div class="flight">
      <div class="outbound">
        <xsl:choose>
          <xsl:when test="count(FlightSectors/FlightSector[Direction='Outbound']) &gt; 1">
            <h3 ml="My Booking">Outbound Flights</h3>
          </xsl:when>
          <xsl:otherwise>
            <h3 ml="My Booking">Outbound Flight</h3>
          </xsl:otherwise>
        </xsl:choose>

        <xsl:for-each select="FlightSectors/FlightSector[Direction='Outbound']">
          <div class="flightroute">
            <p>
              <xsl:value-of select="DepartureAirport" />
              <trans ml="My Booking"> to </trans>
              <xsl:value-of select="ArrivalAirport" />
            </p>
            <p class="carrier">
              <xsl:text> (</xsl:text>
              <xsl:value-of select="FlightCarrier" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="FlightCode" />
              <xsl:text>) </xsl:text>
            </p>
          </div>
          <div class="small">
            <span ml="My Booking">Departs</span>
            <xsl:text> </xsl:text>
            <xsl:call-template name="ShortDate">
              <xsl:with-param name="SQLDate" select="DepartureDate" />
            </xsl:call-template>
            <xsl:text> </xsl:text>
            <xsl:value-of select="DepartureTime" />
          </div>
          <div class="small">
            <span ml="My Booking">Arrives</span>
            <xsl:text> </xsl:text>
            <xsl:call-template name="ShortDate">
              <xsl:with-param name="SQLDate" select="ArrivalDate" />
            </xsl:call-template>
            <xsl:text> </xsl:text>
            <xsl:value-of select="ArrivalTime" />
          </div>
          <div class="clear">
            <xsl:text> </xsl:text>
          </div>
        </xsl:for-each>

      </div>

      <xsl:if test="count(FlightSectors/FlightSector[Direction='Return']) &gt; 0">
        <div class="return">
          <xsl:choose>
            <xsl:when test="count(FlightSectors/FlightSector[Direction='Return']) &gt; 1">
              <h3 ml="My Booking">Return Flights</h3>
            </xsl:when>
            <xsl:otherwise>
              <h3 ml="My Booking">Return Flight</h3>
            </xsl:otherwise>
          </xsl:choose>

          <xsl:for-each select="FlightSectors/FlightSector[Direction='Return']">
            <div class="flightroute">
              <p>
                <xsl:value-of select="DepartureAirport" />
                <trans ml="My Booking"> to </trans>
                <xsl:value-of select="ArrivalAirport" />
              </p>
              <p class="carrier">
                <xsl:text> (</xsl:text>
                <xsl:value-of select="FlightCarrier" />
                <xsl:text> </xsl:text>
                <xsl:value-of select="FlightCode" />
                <xsl:text>) </xsl:text>
              </p>
            </div>
            <div class="small">
              <span ml="My Booking">Departs</span>
              <xsl:text> </xsl:text>
              <xsl:call-template name="ShortDate">
                <xsl:with-param name="SQLDate" select="DepartureDate" />
              </xsl:call-template>
              <xsl:text> </xsl:text>
              <xsl:value-of select="DepartureTime" />
            </div>
            <div class="small">
              <span ml="My Booking">Arrives</span>
              <xsl:text> </xsl:text>
              <xsl:call-template name="ShortDate">
                <xsl:with-param name="SQLDate" select="ArrivalDate" />
              </xsl:call-template>
              <xsl:text> </xsl:text>
              <xsl:value-of select="ArrivalTime" />
            </div>

            <div class="clear">
              <xsl:text> </xsl:text>
            </div>
          </xsl:for-each>
        </div>
      </xsl:if>
      <div class="clear">
        <xsl:text> </xsl:text>
      </div>

      <xsl:variable name="FlightGuestCount" select="count(FlightPassengers/FlightPassenger)"/>
      <xsl:if test="$FlightGuestCount &gt; 0">
        <p class="FlightGuests">
          <xsl:for-each select="FlightPassengers/FlightPassenger">
            <xsl:value-of select="Title"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="FirstName"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="LastName"/>
            <xsl:if test="position() &lt; $FlightGuestCount">
              <xsl:text>, </xsl:text>
            </xsl:if>
          </xsl:for-each>
        </p>
      </xsl:if>

      <xsl:if test="count(FlightExtras/FlightExtra[ExtraType='Baggage']) > 0">
        <xsl:for-each select="FlightExtras/FlightExtra[ExtraType='Baggage']">
          <p class="baggage">
            <trans ml="My Booking">Baggage</trans>
            <xsl:text>: </xsl:text>
            <xsl:value-of select="Quantity"/>
            <xsl:text> x </xsl:text>
            <xsl:value-of select="Description"/>
          </p>
        </xsl:for-each>
      </xsl:if>

    </div>
  </xsl:template>



  <xsl:template name="NonFlightSectorFlight">
    <div class="flight">
      <div class="outbound">
        <h3 ml="My Booking">Outbound Flight</h3>
        <div class="flightroute">
          <p>
            <xsl:value-of select="DepartureAirport" />
            <trans ml="My Booking"> to </trans>
            <xsl:value-of select="ArrivalAirport" />
          </p>
          <p class="carrier">
            <xsl:text> (</xsl:text>
            <xsl:value-of select="FlightCarrier" />
            <xsl:text> </xsl:text>
            <xsl:value-of select="OutboundFlightCode" />
            <xsl:text>) </xsl:text>
          </p>
        </div>
        <div class="small">
          <span ml="My Booking">Departs</span>
          <xsl:text> </xsl:text>
          <xsl:call-template name="ShortDate">
            <xsl:with-param name="SQLDate" select="OutboundDepartureDate" />
          </xsl:call-template>
          <xsl:text> </xsl:text>
          <xsl:value-of select="OutboundDepartureTime" />
        </div>
        <div class="small">
          <span ml="My Booking">Arrives</span>
          <xsl:text> </xsl:text>
          <xsl:call-template name="ShortDate">
            <xsl:with-param name="SQLDate" select="OutboundArrivalDate" />
          </xsl:call-template>
          <xsl:text> </xsl:text>
          <xsl:value-of select="OutboundArrivalTime" />
        </div>
        <div class="clear">
          <xsl:text> </xsl:text>
        </div>
      </div>

      <xsl:if test="ReturnFlightCode != ''">
        <div class="return">
          <h3 ml="My Booking">Return Flight</h3>
          <div class="flightroute">
            <p>
              <xsl:value-of select="ArrivalAirport" />
              <trans ml="My Booking"> to </trans>
              <xsl:value-of select="DepartureAirport" />
            </p>
            <p class="carrier">
              <xsl:text> (</xsl:text>
              <xsl:value-of select="FlightCarrier" />
              <xsl:text> </xsl:text>
              <xsl:value-of select="ReturnFlightCode" />
              <xsl:text>) </xsl:text>
            </p>
          </div>
          <div class="small">
            <span ml="My Booking">Departs</span>
            <xsl:text> </xsl:text>
            <xsl:call-template name="ShortDate">
              <xsl:with-param name="SQLDate" select="ReturnDepartureDate" />
            </xsl:call-template>
            <xsl:text> </xsl:text>
            <xsl:value-of select="ReturnDepartureTime" />
          </div>
          <div class="small">
            <span ml="My Booking">Arrives</span>
            <xsl:text> </xsl:text>
            <xsl:call-template name="ShortDate">
              <xsl:with-param name="SQLDate" select="ReturnArrivalDate" />
            </xsl:call-template>
            <xsl:text> </xsl:text>
            <xsl:value-of select="ReturnArrivalTime" />
          </div>

          <div class="clear">
            <xsl:text> </xsl:text>
          </div>

        </div>
      </xsl:if>
      <div class="clear">
        <xsl:text> </xsl:text>
      </div>

      <xsl:variable name="FlightGuestCount" select="count(FlightPassengers/FlightPassenger)"/>
      <xsl:if test="$FlightGuestCount &gt; 0">
        <p class="FlightGuests">
          <xsl:for-each select="FlightPassengers/FlightPassenger">
            <xsl:value-of select="Title"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="FirstName"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="LastName"/>
            <xsl:if test="position() &lt; $FlightGuestCount">
              <xsl:text>, </xsl:text>
            </xsl:if>
          </xsl:for-each>
        </p>
      </xsl:if>

      <xsl:if test="count(FlightExtras/FlightExtra[ExtraType='Baggage']) > 0">
        <xsl:for-each select="FlightExtras/FlightExtra[ExtraType='Baggage']">
          <p class="baggage">
            <trans ml="My Booking">Baggage</trans>
            <xsl:text>: </xsl:text>
            <xsl:value-of select="Quantity"/>
            <xsl:text> x </xsl:text>
            <xsl:value-of select="Description"/>
          </p>
        </xsl:for-each>
      </xsl:if>

    </div>
  </xsl:template>
</xsl:stylesheet>
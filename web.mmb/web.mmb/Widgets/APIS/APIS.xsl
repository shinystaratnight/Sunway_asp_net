<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <!--<xsl:include href = "../../../../xsl/Markdown.xsl"/>-->

  <xsl:param name="SellingGeographyLevel1ID"/>
  <xsl:param name="Theme"/>
  <xsl:param name="BookingReference"/>
  <xsl:param name="RenderOnPage" select="false"/>

  <xsl:template match="/">

    <div id="divAPISWrapper">
      <xsl:attribute name="style">
        <xsl:choose>
          <xsl:when test="$RenderOnPage = 'True'">
            <xsl:text>display:block;</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>display:none;</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      
      
      <input type="hidden" id="hidAPISBookingReference" value="{$BookingReference}" />
      <input type="hidden" id="hidAPISFlightBookingReference" value="{Flight/Flight/FlightBookingReference}" />
      <input type="hidden" id="hidAPISSupplierReference" value="{Flight/Flight/SourceReference}" />

      <h1 ml="My Booking">Passport Information</h1>

      <!--form-->
      <div id="divAPIS">

        <div id="divAPISWarning" class="warning infobox" style="display:none;">
          <trans ml="My Booking">Please make sure you have filled in all the fields for each passenger.</trans>
        </div>

        <xsl:for-each select="Flight/Flight[Status != 'Cancelled']">

          <xsl:variable name="FlightPosition" select="position()" />

          <div id="APIS_{$FlightPosition}">

            <h2 class="flightBooking">
              <xsl:value-of select="DepartureAirport" /> - <xsl:value-of select="ArrivalAirport" />
            </h2>

            <xsl:for-each select="FlightPassengers/FlightPassenger">

              <xsl:variable name="PassengerPosition" select="position()" />

              <h2 class="leftBorder closed" id="APIS_{$FlightPosition}_{$PassengerPosition}" >
                <trans ml="My Booking">Flight Passenger</trans><xsl:text> </xsl:text><xsl:value-of select="$PassengerPosition" />:
              </h2>

              <div id="APIS_{$FlightPosition}_{$PassengerPosition}_Collapsable" class="closed passenger" >

                <div class="Name">
                  <label ml="My Booking">Name:</label>
                  <input type="text" id="APISTitle" value="{Title}" disabled=""/>
                  <input type="text" id ="APISFirstName" value="{FirstName}" disabled="" />
                  <input type="text" id ="APISMiddleName" value="{MiddleName}" class="validate" placeholder="Middle Name" />
                  <input type="text" id ="APISLastName" value="{LastName}" disabled="" />
                </div>

                <div>
                  <xsl:variable name="FormattedDOB">
                    <xsl:if test="DateOfBirth != '0001-01-01T00:00:00'">
                      <xsl:call-template name="ShortDate">
                        <xsl:with-param name="SQLDate" select="DateOfBirth" />
                      </xsl:call-template>
                    </xsl:if>
                  </xsl:variable>
                  <label id="lblAPISDateOfBirth" ml="My Booking" for="APISDateOfBirth">Date of Birth *</label>
                  <div id="divAPISDateOfBirth_{$FlightPosition}_{$PassengerPosition}" class="divAPISDateOfBirth textbox icon calendar right embedded">
                    <i>
                      <xsl:text> </xsl:text>
                    </i>
                    <input type="text" id ="APISDateOfBirth_{$FlightPosition}_{$PassengerPosition}" class="date-max-yesterday validate calendar APISDateOfBirth" value="{$FormattedDOB}">
                        <xsl:if test="$RenderOnPage ='True'">
                          <xsl:attribute name="disabled"></xsl:attribute>
                        </xsl:if>	
                    </input>
                  </div>

                  <label for="APISGender_{$FlightPosition}_{$PassengerPosition}" id ="lblAPISGender" ml="My Booking" class="gender">Gender *</label>
                  <span class="gender">
                    <input type="radio" id ="APISGenderMale" name="APISGender_{$FlightPosition}_{$PassengerPosition}" value="male" class="validate">
                      <xsl:if test="Gender = 'male'">
                        <xsl:attribute name="checked">
                          <xsl:text>checked</xsl:text>
                        </xsl:attribute>	
                      </xsl:if>
                    </input>
                    <label for="APISGenderMale" ml="My Booking" class="gender">Male</label>
                    <input type="radio" id ="APISGenderFemale" name="APISGender_{$FlightPosition}_{$PassengerPosition}" value="female" class="validate">
                      <xsl:if test="Gender = 'female'">
                        <xsl:attribute name="checked">
                          <xsl:text>checked</xsl:text>
                        </xsl:attribute>
                      </xsl:if>
                    </input>
                    <label for="APISGenderFemale" ml="My Booking" class="gender">Female</label>
                  </span>

                  <div class="nationalitycontainer">
                    <label for="APISNationality" ml="My Booking" id ="lblAPISNationality">Nationality *</label>
                    <div class="custom-select nationality">
                      <select id ="APISNationality" class="validate">
                        <option></option>
                        <xsl:variable name="passengerNationalityID" select="NationalityID"/>
                        <xsl:for-each select="../../../Nationalities/Nationality">
                          <option value="{ISOCode}">
                            <xsl:if test="NationalityID = $passengerNationalityID">
                              <xsl:attribute name="selected">
                                <xsl:text>selected</xsl:text>
                              </xsl:attribute>
                            </xsl:if>
                            <xsl:value-of select="Nationality"/>
                          </option>
                        </xsl:for-each>
                      </select>
                    </div>
                  </div>
                </div>

                <div class="twos">
                  <label for="APISPassportNumber" ml="My Booking">Passport Number *</label>
                  <input type="text" id ="APISPassportNumber" class="validate">
                    <xsl:attribute name="value">
                      <xsl:value-of select="PassportNumber"/>
                    </xsl:attribute>
                  </input>
                  
                  <label for="APISPassportIssuePlace" ml="My Booking">Passport Issue Place *</label>
                  <div class="custom-select passportIssue">
                    <select id ="APISPassportIssuePlace" class="validate">
                      <option></option>
                      <xsl:variable name ="PassportIssuingGeographyLevel1ID" select="PassportIssuingGeographyLevel1ID"/>
                      <xsl:for-each select="../../../IssueLocations/IssueLocation">
                        <option value="{LocationID}">
                            <xsl:if test="LocationID = $PassportIssuingGeographyLevel1ID">
                              <xsl:attribute name="selected">
                                <xsl:text>selected</xsl:text>
                              </xsl:attribute>
                            </xsl:if>
                          <xsl:value-of select="LocationName"/>
                        </option>
                      </xsl:for-each>
                    </select>
                  </div>
                </div>

                <div class="twos">
                  <xsl:variable name="PassportIssueDate">
                    <xsl:if test="PassportIssueDate != '0001-01-01T00:00:00'">
                      <xsl:call-template name="ShortDate">
                        <xsl:with-param name="SQLDate" select="PassportIssueDate" />
                      </xsl:call-template>
                    </xsl:if>
                  </xsl:variable>

                  <label for="APISPassportIssueDate" ml="My Booking">Passport Issue Date *</label>
                  <div class="textbox icon calendar right embedded">
                    <i class ="APISPassportIssueDate_{$FlightPosition}_{$PassengerPosition}">
                      <xsl:text> </xsl:text>
                    </i>
                    <input type="text" id ="APISPassportIssueDate_{$FlightPosition}_{$PassengerPosition}" class="date-max-yesterday calendar validate" value="{$PassportIssueDate}"/>
                  </div>

                  <xsl:variable name="PassportExpiryDate">
                    <xsl:if test="PassportExpiryDate != '0001-01-01T00:00:00'">
                      <xsl:call-template name="ShortDate">
                        <xsl:with-param name="SQLDate" select="PassportExpiryDate" />
                      </xsl:call-template>
                    </xsl:if>
                  </xsl:variable>

                  <label for="APISPassportExpiryDate" ml="My Booking">Passport Expiry Date *</label>
                  <div class="textbox icon calendar right embedded">
                    <i class="APISPassportExpiryDate_{$FlightPosition}_{$PassengerPosition}">
                      <xsl:text> </xsl:text>
                    </i>
                    <input type="text" id ="APISPassportExpiryDate_{$FlightPosition}_{$PassengerPosition}" class="date-min-tomorrow calendar validate" value="{$PassportExpiryDate}"/>
                  </div>
                </div>

                <input id="APISPassengerID" type="hidden" value="{FlightBookingPassengerID}" />

              </div>

            </xsl:for-each>

            <a class="button primary icon submit chevron-right balance" id="btnUpdateApis_{$FlightPosition}" href="javascript:void(0)" >
              <span ml="Manage My Booking">
                Submit Passport Information
              </span>
            </a>

          </div>
        </xsl:for-each>

      </div>

      <!-- Wait Message -->
      <div id="divAPISWaitMessage" style="display:none;">
        <p ml="My Booking">Please wait as we send through your request.</p>
        <img class="spinner" src="/themes/{$Theme}/images/loader.gif" alt="loading..." />
      </div>

      <!-- Request Sent -->
      <div id="divAPISRequestSent" style="display:none;">
        <p ml="My Booking">Thank you. Your passport information has been submitted.</p>
      </div>

      <!-- Request Not Sent -->
      <div id="divAPISRequestNotSent" style="display:none;">
        <p ml="My Booking">Sorry, there seems to be a problem sending your passport information. Please try again later or contact us.</p>
      </div>

    </div>
  </xsl:template>

  <xsl:template name="ShortDate">

    <xsl:param name="SQLDate"/>

    <xsl:value-of select="substring($SQLDate,9,2)"/>/<xsl:value-of select="substring($SQLDate,6,2)"/>/<xsl:value-of select="substring($SQLDate,1,4)"/>

  </xsl:template>

</xsl:stylesheet>

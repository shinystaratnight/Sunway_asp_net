<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:include href="Markdown.xsl"/>
  <xsl:include href="Functions.xsl"/>

  <xsl:param name="Locale"/>
  <xsl:param name="AirportID"/>
  <xsl:param name="CurrencySymbol"/>
  <xsl:param name="CurrencySymbolPosition"/>

  <xsl:template match="/">
    <xsl:apply-templates select="CustomQueryResponse/CustomXML"/>
  </xsl:template>

  <xsl:template match="CustomXML">
      <xsl:variable name="headerLogo">
          <xsl:choose>
              <xsl:when test="SpecialOffer/TradeImage != ''">
                  <xsl:value-of select="SpecialOffer/TradeImage"/>
              </xsl:when>
              <xsl:otherwise>
                  <xsl:value-of select="'../Style/Images/IF_Only_Grey-01.png'"/>
              </xsl:otherwise>    
          </xsl:choose>
      </xsl:variable>
      <xsl:if test ="SpecialOffer/PosterImage != ''">
          <img class="main">
              <xsl:attribute name="src">
                  <xsl:value-of select="SpecialOffer/PosterImage"/>
              </xsl:attribute>
          </img>
      </xsl:if>

    <h2>
      <xsl:value-of select="SpecialOffer/HotelName"/>
    </h2>

    <xsl:if test ="SpecialOffer/AirportPrices[Airport = $AirportID]/PerPersonPrice != ''">
        <p class="offer">From </p>
        <p class="offer">
            <span class="price">
                <xsl:call-template name="GetSellingPrice">
                    <xsl:with-param name="Value" select="SpecialOffer/AirportPrices[Airport = $AirportID]/PerPersonPrice"/>
                    <xsl:with-param name="Exchange" select="1"/>
                    <xsl:with-param name="Currency" select="$CurrencySymbol"  />
                    <xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition"/>
                </xsl:call-template>
            </span>
        </p>
        <p class="offer">per person</p>
    </xsl:if>
 

    <xsl:variable name="priceIncludes">
      <xsl:call-template name="Replace">
        <xsl:with-param name="text" select="SpecialOffer/AdditionalInformation/PosterPriceIncludes" />
        <xsl:with-param name="find" select="'##airport##'" />
        <xsl:with-param name="replacement" select="SpecialOffer/AirportPrices[Airport = $AirportID]/AirportName" />
      </xsl:call-template>
    </xsl:variable>
      
      <xsl:if test ="SpecialOffer/RoundelImage != ''">
          <img class="roundel">
              <xsl:attribute name="src">
                  <xsl:value-of select="SpecialOffer/RoundelImage"/>
              </xsl:attribute>
          </img>
      </xsl:if>

      <xsl:if test ="$priceIncludes != ''">
          <table class="includes">
              <tr>
                  <th>Price includes:</th>
              </tr>
              <tr>
                  <td>
                      <xsl:call-template name="Markdown">
                          <xsl:with-param name="text" select="$priceIncludes"/>
                          <xsl:with-param name ="MaxLoops" select ="'5'"/>
                      </xsl:call-template>
                  </td>
              </tr>
          </table>
      </xsl:if>
      <div class="Logo">
          <img class="Logo">
              <xsl:attribute name="src">
                  <xsl:value-of select="$headerLogo" />
              </xsl:attribute>
          </img>
      </div>

       <xsl:if test ="SpecialOffer/AdditionalInformation/Smallprint != ''">
           <div class="footer">
               <div class="smallPrint">
                   
                   <xsl:variable name="smallPrint">
                       <xsl:choose>
                           <xsl:when test="$Locale = 'Default'">
                               <xsl:value-of select ="concat(SpecialOffer/AdditionalInformation/Smallprint,' ', SpecialOffer/AdditionalInformation/SmallPrintATOL)"/>
                           </xsl:when>
                           <xsl:otherwise>
                               <xsl:value-of select ="SpecialOffer/AdditionalInformation/Smallprint"/>
                           </xsl:otherwise>
                       </xsl:choose>
                   </xsl:variable>

                   <xsl:call-template name="Markdown">
                       <xsl:with-param name="text" select="$smallPrint"/>
                   </xsl:call-template>
               </div>
           </div>
       </xsl:if>
  </xsl:template>
</xsl:stylesheet>

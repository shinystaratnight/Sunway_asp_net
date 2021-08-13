<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template name="FormatMoney">
    <xsl:param name="Currency"/>
    <xsl:param name="Value"/>

    <xsl:variable name="Sign">
      <xsl:if test="$Value &lt; 0">-</xsl:if>
    </xsl:variable>

    <xsl:variable name="NewValue">
      <xsl:choose>
        <xsl:when test="$Value &lt; 0">
          <xsl:value-of select="$Value * -1"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$Value"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:value-of select="concat($Sign,$Currency,format-number($NewValue,'###,###,##0.00'))"/>
  </xsl:template>

  <xsl:template name="FormatRoundedMoney">
    <xsl:param name="Currency"/>
    <xsl:param name="Value"/>

    <xsl:variable name="Sign">
      <xsl:if test="$Value &lt; 0">-</xsl:if>
    </xsl:variable>

    <xsl:variable name="NewValue">
      <xsl:choose>
        <xsl:when test="$Value &lt; 0">
          <xsl:value-of select="$Value * -1"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$Value"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:value-of select="concat($Sign,$Currency,format-number($NewValue,'###,###,##0'))"/>
  </xsl:template>



  <!-- get selling price -->
  <xsl:template name="GetSellingPrice">
    <xsl:param name="Value"/>
    <xsl:param name="Exchange" select="'1'"/>
    <xsl:param name="Currency"/>
    <xsl:param name="Format" select="'###,##0.00'"/>
    <xsl:param name="CurrencySymbolPosition" select="'Prepend'"/>
    <xsl:param name="RoundingRule" select="'Unrounded'"/>

    <!-- do the exchange -->
    <xsl:variable name="ConvertedValue" select="$Value * $Exchange"/>

    <xsl:variable name="RoundingInteger" select="floor($ConvertedValue) mod 10"/>

    <!--Round-->
    <xsl:variable name="RoundedValue">
      <xsl:choose>
        <xsl:when test="$RoundingRule = 'Round' or $RoundingRule = 'Round (Total)'">
          <xsl:value-of select="round($ConvertedValue)" />
        </xsl:when>
        <xsl:when test="$RoundingRule = 'Round Up' or $RoundingRule = 'Round Up (Total)'">
          <xsl:value-of select="ceiling($ConvertedValue)" />
        </xsl:when>
        <xsl:when test="$RoundingRule = '5s and 9s'">
          <xsl:choose>
            <xsl:when test="$RoundingInteger = 0 or $RoundingInteger = 6">
              <xsl:value-of select="floor($ConvertedValue) - 1"/>
            </xsl:when>
            <xsl:when test="$RoundingInteger = 1">
              <xsl:value-of select="floor($ConvertedValue) - 2"/>
            </xsl:when>
            <xsl:when test="$RoundingInteger = 2">
              <xsl:value-of select="floor($ConvertedValue) + 3"/>
            </xsl:when>
            <xsl:when test="$RoundingInteger = 3 or $RoundingInteger = 7">
              <xsl:value-of select="floor($ConvertedValue) + 2"/>
            </xsl:when>
            <xsl:when test="$RoundingInteger = 4 or $RoundingInteger = 8">
              <xsl:value-of select="floor($ConvertedValue) + 1"/>
            </xsl:when>
            <xsl:when test="$RoundingInteger = 5 or $RoundingInteger = 9">
              <xsl:value-of select="floor($ConvertedValue)"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$ConvertedValue"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$ConvertedValue"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="Sign">
      <xsl:if test="$RoundedValue &lt; 0">-</xsl:if>
    </xsl:variable>

    <xsl:variable name="NewValue">
      <xsl:choose>
        <xsl:when test="$RoundedValue &lt; 0">
          <xsl:value-of select="$RoundedValue * -1"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$RoundedValue"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>


    <xsl:value-of select="$Sign"/>
    <xsl:if test="$CurrencySymbolPosition='Prepend'">
      <xsl:value-of select="$Currency"/>
    </xsl:if>
    <xsl:value-of select="format-number($NewValue, $Format)"/>
    <xsl:if test="$CurrencySymbolPosition='Append'">
      <xsl:value-of select="$Currency"/>
    </xsl:if>

  </xsl:template>


  <xsl:template name="FormatTotal">
    <xsl:param name="Total"/>

    <xsl:if test="round($Total)=$Total">
      <xsl:value-of select="format-number($Total,'##,##0')"/>
    </xsl:if>
    <xsl:if test="round($Total)!=$Total">
      <xsl:value-of select="format-number($Total,'##,##0.00')"/>
    </xsl:if>
  </xsl:template>


  <!-- Date Functions -->
  <xsl:template name="ShortDate">
    <xsl:param name="SQLDate"/>
    <xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>

    <xsl:value-of select="substring($SQLDate,9,2)"/>
    <xsl:choose>
      <xsl:when test="$MonthNumber='01'">
        <xsl:text> Jan '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='02'">
        <xsl:text> Feb '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='03'">
        <xsl:text> Mar '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='04'">
        <xsl:text> Apr '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='05'">
        <xsl:text> May '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='06'">
        <xsl:text> Jun '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='07'">
        <xsl:text> Jul '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='08'">
        <xsl:text> Aug '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='09'">
        <xsl:text> Sep '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='10'">
        <xsl:text> Oct '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='11'">
        <xsl:text> Nov '</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='12'">
        <xsl:text> Dec '</xsl:text>
      </xsl:when>
    </xsl:choose>
    <xsl:value-of select="substring($SQLDate,3,2)"/>

  </xsl:template>


  <xsl:template name="MediumDate">
    <xsl:param name="SQLDate"/>
    <xsl:param name="DayOfWeek"/>

    <xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>

    <xsl:value-of select="concat(substring($DayOfWeek,1,3), ' ')"/>

    <xsl:value-of select="substring($SQLDate,9,2)"/>
    <xsl:choose>
      <xsl:when test="$MonthNumber='01'">
        <xsl:text> Jan </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='02'">
        <xsl:text> Feb </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='03'">
        <xsl:text> Mar </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='04'">
        <xsl:text> Apr </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='05'">
        <xsl:text> May </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='06'">
        <xsl:text> Jun </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='07'">
        <xsl:text> Jul </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='08'">
        <xsl:text> Aug </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='09'">
        <xsl:text> Sep </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='10'">
        <xsl:text> Oct </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='11'">
        <xsl:text> Nov </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='12'">
        <xsl:text> Dec </xsl:text>
      </xsl:when>
    </xsl:choose>
    <xsl:value-of select="substring($SQLDate,1,4)"/>

  </xsl:template>


  <xsl:template name="FullDate">
    <xsl:param name="SQLDate"/>

    <xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
    <xsl:variable name="DayNumber" select="substring($SQLDate,9,2)"/>

    <xsl:value-of select="number($DayNumber)"/>
    <xsl:choose>
      <xsl:when test="($DayNumber='11' or $DayNumber='12' or $DayNumber='13') or (substring($DayNumber,2,1)!='1' and substring($DayNumber,2,1)!='2' and substring($DayNumber,2,1)!='3')">
        <xsl:text>th</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="substring($DayNumber,2,1)='1'">
          <xsl:text>st</xsl:text>
        </xsl:if>
        <xsl:if test="substring($DayNumber,2,1)='2'">
          <xsl:text>nd</xsl:text>
        </xsl:if>
        <xsl:if test="substring($DayNumber,2,1)='3'">
          <xsl:text>rd</xsl:text>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>

    <xsl:choose>
      <xsl:when test="$MonthNumber='01'">
        <xsl:text> January </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='02'">
        <xsl:text> February </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='03'">
        <xsl:text> March </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='04'">
        <xsl:text> April </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='05'">
        <xsl:text> May </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='06'">
        <xsl:text> June </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='07'">
        <xsl:text> July </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='08'">
        <xsl:text> August </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='09'">
        <xsl:text> September </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='10'">
        <xsl:text> October </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='11'">
        <xsl:text> November </xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber='12'">
        <xsl:text> December </xsl:text>
      </xsl:when>
    </xsl:choose>

    <xsl:value-of select="substring($SQLDate,1,4)"/>

  </xsl:template>

  <xsl:template name ="GetDay">
    <xsl:param name="SQLDate"/>

    <xsl:variable name="yyyy" select="substring-before($SQLDate, '-')"/>
    <xsl:variable name="mm" select="substring($SQLDate, 6, 2 )"/>
    <xsl:variable name="dd" select="substring($SQLDate, 9, 2)"/>

    <xsl:variable name="a" select="floor((14 - $mm) div 12)"/>
    <xsl:variable name="y" select="$yyyy - $a"/>
    <xsl:variable name="m" select="$mm + 12 * $a - 2"/>

    <xsl:variable name="w" select="($dd + $y + floor($y div 4) - floor($y div 100) + floor($y div 400) + floor((31 * $m) div 12)) mod 7"/>


    <xsl:choose>
      <xsl:when test="$w = 0">Sunday</xsl:when>
      <xsl:when test="$w = 1">Monday</xsl:when>
      <xsl:when test="$w = 2">Tuesday</xsl:when>
      <xsl:when test="$w = 3">Wednesday</xsl:when>
      <xsl:when test="$w = 4">Thursday</xsl:when>
      <xsl:when test="$w = 5">Friday</xsl:when>
      <xsl:when test="$w = 6">Saturday</xsl:when>
    </xsl:choose>


  </xsl:template>


  <xsl:template name="ShortMonth">
    <xsl:param name="MonthNumber"/>

    <xsl:choose>
      <xsl:when test="$MonthNumber=1">
        <xsl:text>Jan</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=2">
        <xsl:text>Feb</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=3">
        <xsl:text>Mar</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=4">
        <xsl:text>Apr</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=5">
        <xsl:text>May</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=6">
        <xsl:text>Jun</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=7">
        <xsl:text>Jul</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=8">
        <xsl:text>Aug</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=9">
        <xsl:text>Sep</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=10">
        <xsl:text>Oct</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=11">
        <xsl:text>Nov</xsl:text>
      </xsl:when>
      <xsl:when test="$MonthNumber=12">
        <xsl:text>Dec</xsl:text>
      </xsl:when>
    </xsl:choose>
  </xsl:template>


  <!-- Abbreviation Functions -->
  <xsl:variable name="upc" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
  <xsl:variable name="lwc" select="'abcdefghijklmnopqrstuvwxyz'"/>

  <xsl:template name="AbbreviateMealBasis">
    <xsl:param name="MealBasisText"/>

    <xsl:choose>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='all inclusive'">AI</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='full board'">FB</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='half board'">HB</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='bed &amp; breakfast'">B&amp;B</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='b &amp; b'">B&amp;B</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='self catering'">SC</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='room only'">RO</xsl:when>
      <xsl:when test="translate($MealBasisText,$upc,$lwc)='family board'">Fam</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$MealBasisText"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="AbbreviateMonth">
    <xsl:param name="MonthText"/>

    <xsl:choose>
      <xsl:when test="translate($MonthText,$upc,$lwc)='january'">Jan</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='feburary'">Feb</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='march'">Mar</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='april'">Apr</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='may'">May</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='june'">Jun</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='july'">Jul</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='august'">Aug</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='september'">Sep</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='october'">Oct</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='november'">Nov</xsl:when>
      <xsl:when test="translate($MonthText,$upc,$lwc)='december'">Dec</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$MonthText"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>



  <xsl:template name="DateRange">
    <xsl:param name="StartDate"/>
    <xsl:param name="EndDate"/>

    <xsl:call-template name="ShortDate">
      <xsl:with-param name="SQLDate" select="$StartDate"/>
    </xsl:call-template>
    <xsl:text> to </xsl:text>
    <xsl:call-template name="ShortDate">
      <xsl:with-param name="SQLDate" select="$EndDate"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="NumberOrDash">
    <xsl:param name="Number"/>

    <xsl:if test="number($Number) &gt; 0">
      <xsl:value-of select="$Number"/>
    </xsl:if>
    <xsl:if test="number($Number) = 0">
      <xsl:text>-</xsl:text>
    </xsl:if>
  </xsl:template>


  <!-- Multiline -->
  <xsl:template name="MultiLine">
    <xsl:param name="text" select="."/>
    <xsl:choose>
      <xsl:when test="contains($text, '&#xA;')">
        <xsl:value-of select="substring-before($text, '&#xA;')"/>
        <br/>
        <xsl:call-template name="MultiLine">
          <xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Multiline paragraph -->
  <xsl:template name="MultiLineParagraph">
    <xsl:param name="text" select="."/>

    <xsl:choose>
      <xsl:when test="contains($text, '&#xA;') and substring-before($text, '&#xA;')=''">
        <xsl:call-template name="MultiLineParagraph">
          <xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($text, '&#xA;')">
        <xsl:value-of disable-output-escaping="yes" select="'&lt;p&gt;'"/>
        <xsl:value-of select="substring-before($text, '&#xA;')"/>
        <xsl:value-of disable-output-escaping="yes" select="'&lt;/p&gt;'"/>
        <xsl:call-template name="MultiLineParagraph">
          <xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of disable-output-escaping="yes" select="'&lt;p&gt;'"/>
        <xsl:value-of select="$text"/>
        <xsl:value-of disable-output-escaping="yes" select="'&lt;/p&gt;'"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Substring after last character in string -->
  <xsl:template name="substring-after-last">
    <xsl:param name="string" />
    <xsl:param name="delimiter" />
    <xsl:choose>
      <xsl:when test="contains($string, $delimiter)">
        <xsl:call-template name="substring-after-last">
          <xsl:with-param name="string" select="substring-after($string, $delimiter)" />
          <xsl:with-param name="delimiter" select="$delimiter" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Count occurences of a substring within a string -->
  <xsl:template name="substring-count">
    <xsl:param name="string"/>
    <xsl:param name="substr"/>

    <xsl:choose>
      <xsl:when test="contains($string, $substr) and $string and $substr">
        <xsl:variable name="rest">
          <xsl:call-template name="substring-count">
            <xsl:with-param name="string" select="substring-after($string, $substr)"/>
            <xsl:with-param name="substr" select="$substr"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="$rest + 1"/>
      </xsl:when>
      <xsl:otherwise>0</xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <!-- PadLeft -->
  <xsl:template name="PadLeft">
    <xsl:param name="Base"/>
    <xsl:param name="Length"/>
    <xsl:choose>
      <xsl:when test="string-length($Base) &lt; number($Length)">
        <xsl:call-template name="PadLeft">
          <xsl:with-param name="Base" select="concat(' ',$Base)"/>
          <xsl:with-param name="Length" select="$Length"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$Base"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- PadRight -->
  <xsl:template name="PadRight">
    <xsl:param name="Base"/>
    <xsl:param name="Length" select="20"/>
    <xsl:choose>
      <xsl:when test="string-length($Base) &lt; number($Length)">
        <xsl:call-template name="PadRight">
          <xsl:with-param name="Base" select="concat($Base,' ')"/>
          <xsl:with-param name="Length" select="$Length"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$Base"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <!-- Underline template -->
  <xsl:template name="underline">
    <xsl:param name="Base"/>
    <xsl:param name="Count" select="1"/>
    <xsl:param name="Char" select="'-'"/>
    <xsl:param name="newline" select="' '"/>

    <xsl:if test="$Count=1">
      <xsl:value-of select="concat($Base,$newline)"/>
    </xsl:if>
    <xsl:value-of select="$Char"/>
    <xsl:if test="$Count=string-length($Base)">
      <xsl:value-of select="$newline"/>
    </xsl:if>

    <xsl:if test="$Count &lt; string-length($Base)">
      <xsl:call-template name="underline">
        <xsl:with-param name="Base" select="$Base"/>
        <xsl:with-param name="Count" select="$Count+1"/>
        <xsl:with-param name="Char" select="$Char"/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>


  <xsl:template name="Replace">
    <xsl:param name="text" />
    <xsl:param name="find" select="'cockpisspatridge'" />
    <xsl:param name="replacement" select="''" />


    <xsl:choose>
      <xsl:when test="$find=''">
        <xsl:value-of select="$text"/>
      </xsl:when>
      <xsl:when test="contains($text,$find)">

        <xsl:value-of select="concat(substring-before($text,$find),$replacement)"/>
        <xsl:call-template name="Replace">
          <xsl:with-param name="text" select="substring-after($text,$find)"/>
          <xsl:with-param name="find" select="$find"/>
          <xsl:with-param name="replacement" select="$replacement"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


  <!-- Draw Link -->
  <xsl:template name="DrawLink">
    <xsl:param name="LinkText"/>
    <xsl:param name="DetectLinkTargetWindow"/>
    <xsl:param name="AddBaseURL"/>
    <xsl:param name="AddClass"/>

    <xsl:if test="contains($LinkText,'[') and contains($LinkText,'](') and contains($LinkText,')')">

      <xsl:variable name="insquarebracket" select="substring-before(substring-after($LinkText,'['),']')"/>
      <xsl:variable name="inroundbracket" select="substring-before(substring-after($LinkText,']('),')')"/>

      <a>
        <xsl:attribute name="href">
          <xsl:choose>
            <xsl:when test="$AddBaseURL!='' and starts-with($inroundbracket, '/')">
              <xsl:value-of select="$AddBaseURL"/>
              <xsl:value-of select="substring($inroundbracket,2)"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$inroundbracket"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>

        <xsl:if test="$DetectLinkTargetWindow='True' and starts-with($inroundbracket, 'http://')">
          <xsl:attribute name="target">_blank</xsl:attribute>
        </xsl:if>

        <xsl:if test="$AddClass!=''">
          <xsl:attribute name="class">
            <xsl:value-of select="$AddClass"/>
          </xsl:attribute>
        </xsl:if>

        <xsl:value-of select="$insquarebracket"/>
      </a>

    </xsl:if>

  </xsl:template>

  <!-- Passenger Summary -->
  <xsl:template name="PassengerSummary">
    <xsl:param name="Adults" select="0"/>
    <xsl:param name="Children" select="0"/>
    <xsl:param name="Infants" select="0"/>


    <xsl:if test="$Adults>0">
      <trans ml="Basket" mlparams="{$Adults}">
        <xsl:value-of select="'{0} Adult'"/>
        <xsl:if test="$Adults>1">
          <xsl:text>s</xsl:text>
        </xsl:if>
      </trans>
    </xsl:if>

    <xsl:if test="$Children>0">
      <xsl:if test="$Adults>0">
        <xsl:text>, </xsl:text>
      </xsl:if>
      <trans ml="Basket" mlparams="{$Children}">
        <xsl:value-of select="'{0} Child'"/>
        <xsl:if test="$Children>1">
          <xsl:text>ren</xsl:text>
        </xsl:if>
      </trans>
    </xsl:if >

    <xsl:if test="$Infants>0">
      <xsl:if test="$Adults>0 or $Children>0">
        <xsl:text>, </xsl:text>
      </xsl:if>
      <trans ml="Basket" mlparams="{$Infants}">
        <xsl:value-of select="'{0} Infant'"/>
        <xsl:if test="$Infants>1">
          <xsl:text>s</xsl:text>
        </xsl:if>
      </trans>
    </xsl:if>

  </xsl:template>


  <!-- Star Rating -->
  <xsl:template name="StarRating">
    <xsl:param name="Rating" />
    <xsl:param name="Small" select="'false'"/>


    <xsl:variable name="class">
      <xsl:text>rating star</xsl:text>
      <xsl:value-of select="substring($Rating,1,1)"/>

      <xsl:if test="substring($Rating,3,1)='5'">
        <xsl:text> half</xsl:text>
      </xsl:if>

      <xsl:if test="$Small = 'true'">
        <xsl:text> small</xsl:text>
      </xsl:if>
    </xsl:variable>

    <!-- using this method as we do not want a space within the span but need to prevent the tag from self closing -->
    <xsl:text disable-output-escaping="yes">&lt;span class="</xsl:text>
    <xsl:value-of select="$class"/>
    <xsl:text disable-output-escaping="yes">"&gt;&lt;/span&gt;</xsl:text>

  </xsl:template>


  <xsl:template name="MinutesToHours">
    <xsl:param name="TimeMinutes"/>

    <xsl:variable name="Hours">
      <xsl:value-of select="floor($TimeMinutes div 60)"/>
    </xsl:variable>

    <xsl:variable name="Minutes">
      <xsl:value-of select="floor($TimeMinutes mod 60)"/>
    </xsl:variable>

    <xsl:if test="$Hours &gt; 0">
      <xsl:value-of select="concat($Hours, 'hrs ')"/>
    </xsl:if>
    <xsl:if test="$Minutes &gt; 0">
      <xsl:value-of select="concat($Minutes, 'min')"/>
    </xsl:if>

  </xsl:template>

</xsl:stylesheet>


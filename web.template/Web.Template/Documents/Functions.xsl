<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format" version="1.0" >

<!-- Passenger Summary -->
<xsl:template name="PassengerSummary">
      <xsl:param name="Adults" select="0"/>
      <xsl:param name="Children" select="0"/>
      <xsl:param name="Infants" select="0"/>
      
      <xsl:if test="$Adults>0">
            <xsl:value-of select="concat($Adults, ' Adult')"/>
      </xsl:if>
      <xsl:if test="$Adults>1">
            <xsl:text>s</xsl:text>
      </xsl:if>
      
      <xsl:if test="$Children>0">
            <xsl:if test="$Adults>0">
                  <xsl:text>, </xsl:text>
            </xsl:if>
            <xsl:value-of select="concat($Children, ' Child')"/>
            <xsl:if test="$Children>1">
                  <xsl:text>ren</xsl:text>
            </xsl:if>
      </xsl:if >
      
      <xsl:if test="$Infants>0">
            <xsl:if test="$Adults>0 or $Children>0">
                  <xsl:text>, </xsl:text>
            </xsl:if>
            <xsl:value-of select="concat($Infants, ' Infant')"/>
            <xsl:if test="$Infants>1">
                  <xsl:text>s</xsl:text>
            </xsl:if>
      </xsl:if>
      
</xsl:template>

	
	
<xsl:template name="FormatRoundedMoney">
	<xsl:param name="Currency"/>
	<xsl:param name="Value"/>
	
	<xsl:variable name="Sign">
		<xsl:if test="$Value &lt; 0">-</xsl:if>
	</xsl:variable>
	
	<xsl:variable name="NewValue">
		<xsl:choose>
			<xsl:when test="$Value &lt; 0"><xsl:value-of select="$Value * -1"/></xsl:when>
			<xsl:otherwise><xsl:value-of select="$Value"/></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:value-of select="concat($Sign,$Currency,format-number($NewValue,'###,###,##0'))"/>
</xsl:template>
  
  <xsl:template name="SumRoundedValues">
    <xsl:param name="Value" />
    <xsl:param name="Sum" select="0" />
    <xsl:param name="DecimalPlaces" select="2" />

    <xsl:variable name="DecimalPlacesFormat">
      <xsl:call-template name="GetNumberFormatForDecimalPlaces">
        <xsl:with-param name="NumberOfPlaces" select="$DecimalPlaces" />
      </xsl:call-template>
    </xsl:variable>
    
    <xsl:choose>
      <xsl:when test="$Value">
        <xsl:call-template name="SumRoundedValues">
          <xsl:with-param name="Value" select="$Value[position() > 1]" />
          <xsl:with-param name="Sum" select="$Sum + format-number($Value[1], $DecimalPlacesFormat)" />
          <xsl:with-param name="DecimalPlaces" select="$DecimalPlaces" />
          
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$Sum" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="GetNumberFormatForDecimalPlaces">
    <xsl:param name="NumberOfPlaces" />
    <xsl:param name="Output" select="'0.'" />

    <xsl:choose>
      <xsl:when test="string-length($Output) &lt; $NumberOfPlaces + 2">
        <xsl:call-template name="GetNumberFormatForDecimalPlaces">
          <xsl:with-param name="NumberOfPlaces" select="$NumberOfPlaces" />
          <xsl:with-param name="Output" select="concat($Output, '#')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$Output" />
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>
  
<!-- get selling price -->
<xsl:template name="GetSellingPrice">
	<xsl:param name="Value"/>
	<xsl:param name="Exchange"/>
	<xsl:param name="Currency"/>
	<xsl:param name="Type" select="'Integer'"/>
  <xsl:param name="CurrencySymbolPosition" select="'Prepend'"/>
  
	<!-- do the exchange -->
	<xsl:variable name="ConvertedValue" select="$Value * $Exchange"/>
	
	<xsl:variable name="Sign">
		<xsl:if test="$ConvertedValue &lt; 0">-</xsl:if>
	</xsl:variable>
	
	<xsl:variable name="NewValue">
		<xsl:choose>
			<xsl:when test="$ConvertedValue &lt; 0"><xsl:value-of select="$ConvertedValue * -1"/></xsl:when>
			<xsl:otherwise><xsl:value-of select="$ConvertedValue"/></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>


  <xsl:choose>
    
    <xsl:when test="$CurrencySymbolPosition='Prepend'"> 
      <xsl:choose>
		    <xsl:when test="$Type='Integer'">
                <xsl:value-of select ="$Sign"/>
                <span class="currencySymbol">
                    <xsl:value-of select ="$Currency"/>
                </span>
			    <xsl:value-of select="format-number($NewValue,'###,###,##0')"/>
		    </xsl:when>
		    <xsl:otherwise>
			    <xsl:value-of select="concat($Sign,$Currency,format-number($NewValue,'###,###,##0.00'))"/>
		    </xsl:otherwise>
	    </xsl:choose>
    </xsl:when>
    
    <xsl:when test="$CurrencySymbolPosition='Append'">
      <xsl:choose>
        <xsl:when test="$Type='Integer'">
          <xsl:value-of select="concat($Sign,format-number($NewValue,'###,###,##0'),$Currency)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat($Sign,format-number($NewValue,'###,###,##0.00'),$Currency)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:when>
    
  </xsl:choose>
	
</xsl:template>	


<!-- date formatter -->
<xsl:template name="FormatDate">
	<xsl:param name="SQLDate"/>
	<xsl:param name="Language"/>
	<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
	
	<xsl:value-of select="substring($SQLDate,9,2)"/>
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English' or $Language='en'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'"><xsl:text> January </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='02'"><xsl:text> February </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='03'"><xsl:text> March </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='04'"><xsl:text> April </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='05'"><xsl:text> May </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='06'"><xsl:text> June </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='07'"><xsl:text> July </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='08'"><xsl:text> August </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='09'"><xsl:text> September </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='10'"><xsl:text> October </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='11'"><xsl:text> November </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='12'"><xsl:text> December </xsl:text></xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='German' or $Language='de'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'"><xsl:text> Januar </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='02'"><xsl:text> Februar </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='03'"><xsl:text> März </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='04'"><xsl:text> April </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='05'"><xsl:text> Mai </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='06'"><xsl:text> Juni </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='07'"><xsl:text> Juli </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='08'"><xsl:text> August </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='09'"><xsl:text> September </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='10'"><xsl:text> Oktober </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='11'"><xsl:text> November </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='12'"><xsl:text> Dezember </xsl:text></xsl:when>
				</xsl:choose>
			</xsl:when>	
			<xsl:when test="$Language='French' or $Language='fr'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'"><xsl:text> Janvier </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='02'"><xsl:text> Février </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='03'"><xsl:text> Mars </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='04'"><xsl:text> Avril </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='05'"><xsl:text> Mai </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='06'"><xsl:text> Juin </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='07'"><xsl:text> Juillet </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='08'"><xsl:text> Août </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='09'"><xsl:text> Septembre  </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='10'"><xsl:text> Octobre </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='11'"><xsl:text> Novembre </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='12'"><xsl:text> Décembre  </xsl:text></xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Dutch' or $Language='nl'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'"><xsl:text> Januari </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='02'"><xsl:text> Februari </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='03'"><xsl:text> Maart </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='04'"><xsl:text> April </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='05'"><xsl:text> Mei </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='06'"><xsl:text> Juni </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='07'"><xsl:text> Juli </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='08'"><xsl:text> Augustus </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='09'"><xsl:text> September  </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='10'"><xsl:text> Oktober </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='11'"><xsl:text> November </xsl:text></xsl:when>
					<xsl:when test="$MonthNumber='12'"><xsl:text> December  </xsl:text></xsl:when>
				</xsl:choose>
			</xsl:when>
		</xsl:choose>	
	<xsl:value-of select="substring($SQLDate,0,5)"/>
</xsl:template>


<!-- short date -->
<xsl:template name="ShortDate">
	<xsl:param name="SQLDate"/>
	<xsl:param name="Language"/>
	<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
	
	<xsl:value-of select="substring($SQLDate,9,2)"/>
	<xsl:choose>
		<!-- language or ISOCode-->
		<xsl:when test="$Language='' or $Language='English' or $Language='gb'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> May '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Oct '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dec '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Dutch' or $Language='nl'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Maa '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mei '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Okt '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dec '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='French' or $Language='fr'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Fév '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Avr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mai '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aoû '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Oct '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Déc '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>	
		<xsl:when test="$Language='German' or $Language='de'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mrz '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mai '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Okt '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dez '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Spanish' or $Language='es'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Ene '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Abr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> May '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Ago '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Oct '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dic '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Swedish' or $Language='se' or $Language='sv'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Maj '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Okt '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dec '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Italian' or $Language='it'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Gen '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mag '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Giu '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Lug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Ago '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Set '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Ott '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dic '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Norwegian' or $Language='no'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mai '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Okt '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Des '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Irish' or $Language='en'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> May '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Oct '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dec '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Portuguese' or $Language='pt'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Fev '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Abr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Mai '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Set '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Out '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dez '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Finnish' or $Language='fi'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Tam '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Hel '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Maa '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Huh '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Tou '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Kes '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Hei '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Elo '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Syy '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Lok '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Jou '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Danish' or $Language='da'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Jan '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Feb '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Mar '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Apr '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Maj '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Jun '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Jul '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Aug '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Sep '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Okt '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Nov '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> Dec '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$Language='Croatian'">
			<xsl:choose>
				<xsl:when test="$MonthNumber='01'"><xsl:text> Sij '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='02'"><xsl:text> Velg '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='03'"><xsl:text> Ozu '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='04'"><xsl:text> Tra '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='05'"><xsl:text> Svi '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='06'"><xsl:text> Lip '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='07'"><xsl:text> Srp '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='08'"><xsl:text> Kol '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='09'"><xsl:text> Ruj '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='10'"><xsl:text> Lis '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='11'"><xsl:text> Stu '</xsl:text></xsl:when>
				<xsl:when test="$MonthNumber='12'"><xsl:text> pro '</xsl:text></xsl:when>
			</xsl:choose>
		</xsl:when>
		
    <xsl:when test="$Language='Traditional Chinese'">
      <xsl:choose>
        <xsl:when test="$MonthNumber='01'"><xsl:text> 一月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='02'"><xsl:text> 二月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='03'"><xsl:text> 三月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='04'"><xsl:text> 四月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='05'"><xsl:text> 五月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='06'"><xsl:text> 六月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='07'"><xsl:text> 七月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='08'"><xsl:text> 八月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='09'"><xsl:text> 九月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='10'"><xsl:text> 十月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='11'"><xsl:text> 十一月 '</xsl:text></xsl:when>
        <xsl:when test="$MonthNumber='12'"><xsl:text> 十二月 '</xsl:text></xsl:when>
      </xsl:choose>
    </xsl:when>    
	</xsl:choose>
	
	
	<xsl:value-of select="substring($SQLDate,3,2)"/>
</xsl:template>


<!-- full date -->
<xsl:template name="FullDate">
    <xsl:param name="SQLDate"/>
	<xsl:param name="DateSuffix" select="'true'"/>
	<xsl:param name="ShortMonth" select="'false'"/>
    
    <xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
    <xsl:variable name="DayNumber" select="substring($SQLDate,9,2)"/>

    <xsl:value-of select="number($DayNumber)"/>

	<xsl:if test="$DateSuffix = 'true'">
		<xsl:choose>
			<xsl:when test="($DayNumber='11' or $DayNumber='12' or $DayNumber='13') or (substring($DayNumber,2,1)!='1' and substring($DayNumber,2,1)!='2' and substring($DayNumber,2,1)!='3')">
				<xsl:text>th</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="substring($DayNumber,2,1)='1'"><xsl:text>st</xsl:text></xsl:if>
				<xsl:if test="substring($DayNumber,2,1)='2'"><xsl:text>nd</xsl:text></xsl:if>
				<xsl:if test="substring($DayNumber,2,1)='3'"><xsl:text>rd</xsl:text></xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>

    <xsl:if test="$ShortMonth='false'">
		<xsl:choose>
			<xsl:when test="$MonthNumber='01'"><xsl:text> January </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='02'"><xsl:text> February </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='03'"><xsl:text> March </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='04'"><xsl:text> April </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='05'"><xsl:text> May </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='06'"><xsl:text> June </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='07'"><xsl:text> July </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='08'"><xsl:text> August </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='09'"><xsl:text> September </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='10'"><xsl:text> October </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='11'"><xsl:text> November </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='12'"><xsl:text> December </xsl:text></xsl:when>
		</xsl:choose>
	</xsl:if>
	<xsl:if test="$ShortMonth!='false'">
		<xsl:choose>
			<xsl:when test="$MonthNumber='01'"><xsl:text> Jan </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='02'"><xsl:text> Feb </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='03'"><xsl:text> Mar </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='04'"><xsl:text> Apr </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='05'"><xsl:text> May </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='06'"><xsl:text> Jun </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='07'"><xsl:text> Jul </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='08'"><xsl:text> Aug </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='09'"><xsl:text> Sep </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='10'"><xsl:text> Oct </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='11'"><xsl:text> Nov </xsl:text></xsl:when>
			<xsl:when test="$MonthNumber='12'"><xsl:text> Dec </xsl:text></xsl:when>
		</xsl:choose>
	</xsl:if>
    
    <xsl:value-of select="substring($SQLDate,1,4)"/>

</xsl:template>

<!-- day of month -->
<xsl:template name="DayOfMonth">
	<xsl:param name="SQLDate"/>

	<xsl:variable name="DayNumber" select="substring($SQLDate,9,2)"/>

	<xsl:value-of select="number($DayNumber)"/>
	<xsl:choose>
		<xsl:when test="($DayNumber='11' or $DayNumber='12' or $DayNumber='13') or (substring($DayNumber,2,1)!='1' and substring($DayNumber,2,1)!='2' and substring($DayNumber,2,1)!='3')">
			<xsl:text>th</xsl:text>
		</xsl:when>
		<xsl:otherwise>
			<xsl:if test="substring($DayNumber,2,1)='1'"><xsl:text>st</xsl:text></xsl:if>
			<xsl:if test="substring($DayNumber,2,1)='2'"><xsl:text>nd</xsl:text></xsl:if>
			<xsl:if test="substring($DayNumber,2,1)='3'"><xsl:text>rd</xsl:text></xsl:if>
		</xsl:otherwise>
	</xsl:choose>

</xsl:template>
	
<!-- day of week -->
<xsl:template name="DayOfWeek">
	<xsl:param name="WeekDay"/>

	<xsl:choose>
		<xsl:when test="$WeekDay='1'"><xsl:text>Sunday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='2'"><xsl:text>Monday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='3'"><xsl:text>Tuesday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='4'"><xsl:text>Wednesday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='5'"><xsl:text>Thursday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='6'"><xsl:text>Friday</xsl:text></xsl:when>
		<xsl:when test="$WeekDay='7'"><xsl:text>Saturday</xsl:text></xsl:when>
	</xsl:choose>
</xsl:template>

	<xsl:template name="DayOfWeekShort">
		<xsl:param name="WeekDay"/>

		<xsl:choose>
			<xsl:when test="$WeekDay='1'">
				<xsl:text>Sun</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='2'">
				<xsl:text>Mon</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='3'">
				<xsl:text>Tue</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='4'">
				<xsl:text>Wed</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='5'">
				<xsl:text>Thu</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='6'">
				<xsl:text>Fri</xsl:text>
			</xsl:when>
			<xsl:when test="$WeekDay='7'">
				<xsl:text>Sat</xsl:text>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

<!-- Month and year -->
<xsl:template name="MonthYear">
	<xsl:param name="SQLDate"/>

	<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>

	<xsl:choose>
		<xsl:when test="$MonthNumber='01'"><xsl:text>January </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='02'"><xsl:text>February </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='03'"><xsl:text>March </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='04'"><xsl:text>April </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='05'"><xsl:text>May </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='06'"><xsl:text>June </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='07'"><xsl:text>July </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='08'"><xsl:text>August </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='09'"><xsl:text>September </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='10'"><xsl:text>October </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='11'"><xsl:text>November </xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='12'"><xsl:text>December </xsl:text></xsl:when>
	</xsl:choose>

	<xsl:value-of select="substring($SQLDate,1,4)"/>

</xsl:template>



	<!-- short dateband -->
<xsl:template name="ShortDateBand">
	<xsl:param name="StartDate"/>
	<xsl:param name="EndDate"/>
	
	<xsl:variable name="ShortStartDate">
		<xsl:call-template name="ShortDate">
			<xsl:with-param name="SQLDate" select="$StartDate"/>
		</xsl:call-template>
	</xsl:variable>
	
	<xsl:variable name="ShortEndDate">
		<xsl:call-template name="ShortDate">
			<xsl:with-param name="SQLDate" select="$EndDate"/>
		</xsl:call-template>
	</xsl:variable>
	
	<xsl:value-of select="$ShortStartDate"/>
	<xsl:choose>
		<xsl:when test="number(substring($ShortEndDate,9,2)) &gt; 9">
			<xsl:text> onwards</xsl:text>
		</xsl:when>
		<xsl:otherwise>
			<xsl:text> to </xsl:text>
			<xsl:value-of select="$ShortEndDate"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- date formatter Short-->
<xsl:template name="FormatDateShort">
	<xsl:param name="SQLDate"/>
	<xsl:param name="IncludeYear" select="'true'"/>
	<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
	
	<xsl:value-of select="substring($SQLDate,9,2)"/>
	<xsl:choose>
		<xsl:when test="$MonthNumber='01'"><xsl:text> Jan</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='02'"><xsl:text> Feb</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='03'"><xsl:text> Mar</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='04'"><xsl:text> Apr</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='05'"><xsl:text> May</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='06'"><xsl:text> Jun</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='07'"><xsl:text> Jul</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='08'"><xsl:text> Aug</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='09'"><xsl:text> Sep</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='10'"><xsl:text> Oct</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='11'"><xsl:text> Nov</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='12'"><xsl:text> Dec</xsl:text></xsl:when>
	</xsl:choose>
	
	<xsl:if test="$IncludeYear='true'">
		<xsl:text> '</xsl:text>
		<xsl:value-of select="substring($SQLDate,3,2)"/>
	</xsl:if>
	
</xsl:template>
	
<!-- display date -->
<xsl:template name="DisplayDate">
	<xsl:param name="SQLDate"/>
	
	<xsl:value-of select="concat(substring($SQLDate,9,2),' ')"/>
	 <xsl:call-template name="MonthName">
		<xsl:with-param name="SQLDate" select="$SQLDate"/>
	</xsl:call-template> 
	<xsl:value-of select="concat(' ',substring($SQLDate,1,4))"/>
	
</xsl:template>

<!-- shorter date -->
<xsl:template name="ShorterDate">
	<xsl:param name="SQLDate"/>
	<xsl:param name="DateSeparator" select="'/'"/>

	<xsl:value-of select="substring($SQLDate,9,2)"/>
	<xsl:value-of select="$DateSeparator"/>
	<xsl:value-of select="substring($SQLDate,6,2)"/>
	<xsl:value-of select="$DateSeparator"/>
	<xsl:value-of select="substring($SQLDate,3,2)"/>
</xsl:template>

<!-- shorter dateband -->
<xsl:template name="ShorterDateband">
	<xsl:param name="StartDate"/>
	<xsl:param name="EndDate"/>
	
	<xsl:call-template name="ShorterDate">
		<xsl:with-param name="SQLDate" select="$StartDate"/>
	</xsl:call-template>
	<xsl:text>-</xsl:text>
	<xsl:call-template name="ShorterDate">
		<xsl:with-param name="SQLDate" select="$EndDate"/>
	</xsl:call-template>
	
</xsl:template>

<xsl:template name="DateRange">
	<xsl:param name="StartDate"/>
	<xsl:param name="EndDate"/>
	<xsl:param name="Language"/>
	
	
	<xsl:call-template name="ShortDate">
		<xsl:with-param name="SQLDate" select="$StartDate"/>
		<xsl:with-param name="Language" select="$Language"/>
	</xsl:call-template>
	
	<xsl:choose>
		<xsl:when test="$Language='German' or $Language='de'"><xsl:text> bis </xsl:text></xsl:when>
		<xsl:when test="$Language='Spanish' or $Language='es'"><xsl:text> a </xsl:text></xsl:when>
		<xsl:when test="$Language='Swedish' or $Language='sv' or $Language='se'"><xsl:text> till </xsl:text></xsl:when>
		<xsl:when test="$Language='Italian' or $Language='it'"><xsl:text> a </xsl:text></xsl:when>
		<xsl:when test="$Language='Norwegian' or $Language='no'"><xsl:text> til </xsl:text></xsl:when>
		<xsl:when test="$Language='Irish' or $Language='en'"><xsl:text> to </xsl:text></xsl:when>
		<xsl:when test="$Language='Portuguese' or $Language='pt'"><xsl:text> para </xsl:text></xsl:when>
		<xsl:when test="$Language='Finnish' or $Language='fi'"><xsl:text> minne </xsl:text></xsl:when>
		<xsl:when test="$Language='Danish' or $Language='da'"><xsl:text> til </xsl:text></xsl:when>
		<xsl:when test="$Language='Dutch' or $Language='nl'"><xsl:text> tot </xsl:text></xsl:when>
		<xsl:when test="$Language='French' or $Language='fr'"><xsl:text> au </xsl:text></xsl:when>
		<xsl:otherwise><xsl:text> to </xsl:text></xsl:otherwise>
	</xsl:choose>
	
	<xsl:call-template name="ShortDate">
		<xsl:with-param name="SQLDate" select="$EndDate"/>
		<xsl:with-param name="Language" select="$Language"/>
	</xsl:call-template>
</xsl:template>


<!-- date compare -->
<xsl:template name="DateCompare">
    <xsl:param name="SQLDate1"/>
    <xsl:param name="SQLDate2"/>
    <xsl:param name="SQLDate1LaterThanSQLDate2Text"/>
    <xsl:param name="SQLDate1EqualToSQLDate2Text"/>
    <xsl:param name="SQLDate1EarlierThanSQLDate2Text"/>

    <xsl:variable name="MonthNumber1" select="number(substring($SQLDate1,6,2))"/>
    <xsl:variable name="MonthNumber2" select="number(substring($SQLDate2,6,2))"/>
    <xsl:variable name="YearNumber1" select="number(substring($SQLDate1,0,5))"/>
    <xsl:variable name="YearNumber2" select="number(substring($SQLDate2,0,5))"/>
    <xsl:variable name="DayNumber1" select="number(substring($SQLDate1,9,2))"/>
    <xsl:variable name="DayNumber2" select="number(substring($SQLDate2,9,2))"/>

    <xsl:choose>
        <xsl:when test="$YearNumber1 > $YearNumber2">
            <xsl:value-of select="$SQLDate1LaterThanSQLDate2Text"/>
        </xsl:when>
        <xsl:when test="$YearNumber1 &lt; $YearNumber2">
            <xsl:value-of select="$SQLDate1EarlierThanSQLDate2Text"/>
        </xsl:when>
        <xsl:when test="$MonthNumber1 > $MonthNumber2">
            <xsl:value-of select="$SQLDate1LaterThanSQLDate2Text"/>
        </xsl:when>
        <xsl:when test="$MonthNumber1 &lt; $MonthNumber2">
            <xsl:value-of select="$SQLDate1EarlierThanSQLDate2Text"/>
        </xsl:when>
        <xsl:when test="$DayNumber1 > $DayNumber2">
            <xsl:value-of select="$SQLDate1LaterThanSQLDate2Text"/>
        </xsl:when>
        <xsl:when test="$DayNumber1 &lt; $DayNumber2">
            <xsl:value-of select="$SQLDate1EarlierThanSQLDate2Text"/>
        </xsl:when>
        <xsl:otherwise>
            <xsl:value-of select="$SQLDate1EqualToSQLDate2Text"/>
        </xsl:otherwise>
    </xsl:choose>
    
</xsl:template>
	
<!--Date before-->
<xsl:template name="DateBefore">
	<xsl:param name="SQLDate1"/>
	<xsl:param name="SQLDate2"/>
	
	<xsl:variable name="MonthNumber1" select="number(substring($SQLDate1,6,2))"/>
    <xsl:variable name="MonthNumber2" select="number(substring($SQLDate2,6,2))"/>
    <xsl:variable name="YearNumber1" select="number(substring($SQLDate1,0,5))"/>
    <xsl:variable name="YearNumber2" select="number(substring($SQLDate2,0,5))"/>
    <xsl:variable name="DayNumber1" select="number(substring($SQLDate1,9,2))"/>
    <xsl:variable name="DayNumber2" select="number(substring($SQLDate2,9,2))"/>

    <xsl:choose>
        <xsl:when test="$YearNumber1 &gt; $YearNumber2">
			<xsl:text>false</xsl:text>
        </xsl:when>
        <xsl:when test="$YearNumber1 = $YearNumber2 and $MonthNumber1 &gt; $MonthNumber2">
			<xsl:text>false</xsl:text>
        </xsl:when>
        <xsl:when test="$YearNumber1 = $YearNumber2 and $MonthNumber1 and $MonthNumber2 and $DayNumber1 &gt; $DayNumber2">
			<xsl:text>false</xsl:text>
        </xsl:when>
        <xsl:otherwise>
			<xsl:text>true</xsl:text>
        </xsl:otherwise>
    </xsl:choose>
	
</xsl:template>


<!-- Date and Time -->
<xsl:template name="DateTime">
	<xsl:param name="SQLDate"/>
	<xsl:call-template name="FormatDate">
		<xsl:with-param name="SQLDate" select="$SQLDate"/>
	</xsl:call-template>
	<xsl:text> </xsl:text>
	<xsl:call-template name="FormatTime">
		<xsl:with-param name="SQLDate" select="$SQLDate"/>
	</xsl:call-template>
</xsl:template>
	
<!-- Date and Time -->
<xsl:template name="ShortDateTime">
	<xsl:param name="SQLDate"/>
	<xsl:call-template name="ShortDate">
		<xsl:with-param name="SQLDate" select="$SQLDate"/>
	</xsl:call-template>
	<xsl:text> </xsl:text>
	<xsl:call-template name="FormatTime">
		<xsl:with-param name="SQLDate" select="$SQLDate"/>
	</xsl:call-template>
</xsl:template>
	
<!-- dd/mm/yyyy -->
<xsl:template name="FormatDateForwardSlash">
	<xsl:param name="SQLDate"/>
	
	<xsl:value-of select="concat(substring($SQLDate,9,2),'/')"/>
	<xsl:value-of select="substring($SQLDate,6,2)"/>
	<xsl:value-of select="concat('/',substring($SQLDate,1,4))"/>
	
</xsl:template>
	
<!-- dd mm yy e.g 12 Sept 15-->
	<xsl:template name="ShortDateYear">
		<xsl:param name="SQLDate"/>
		<xsl:param name="Language"/>
		<xsl:param name="FullYear" select="'False'"/>
		<xsl:param name="RemoveLeadingZeros" select="'False'"/>
		
		<xsl:variable name="MonthNumber" select="substring($SQLDate,6,2)"/>
		<xsl:variable name="DayNumber" select="substring($SQLDate,9,2)"/>
		
		<xsl:choose>
			<xsl:when test="$RemoveLeadingZeros = 'True' and starts-with($DayNumber, '0')">
				<xsl:value-of select="substring($DayNumber, 2)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$DayNumber"/>
			</xsl:otherwise>
		</xsl:choose>

		<xsl:choose>
			<!-- language or ISOCode-->
			<xsl:when test="$Language='' or $Language='English' or $Language='gb'">
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
			</xsl:when>
			<xsl:when test="$Language='Dutch' or $Language='nl'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Jan </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Feb </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Maa </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Apr </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Mei </xsl:text>
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
						<xsl:text> Okt </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dec </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='French' or $Language='fr'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Jan </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Fév </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Mar </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Avr </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Mai </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='06'">
						<xsl:text> Jun </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='07'">
						<xsl:text> Jul </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='08'">
						<xsl:text> Aoû </xsl:text>
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
						<xsl:text> Déc </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='German' or $Language='de'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Jan </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Feb </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Mrz </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Apr </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Mai </xsl:text>
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
						<xsl:text> Okt </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dez </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Spanish' or $Language='es'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Ene </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Feb </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Mar </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Abr </xsl:text>
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
						<xsl:text> Ago </xsl:text>
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
						<xsl:text> Dic </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Swedish' or $Language='se' or $Language='sv'">
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
						<xsl:text> Maj </xsl:text>
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
						<xsl:text> Okt </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dec </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Italian' or $Language='it'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Gen </xsl:text>
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
						<xsl:text> Mag </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='06'">
						<xsl:text> Giu </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='07'">
						<xsl:text> Lug </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='08'">
						<xsl:text> Ago </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='09'">
						<xsl:text> Set </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='10'">
						<xsl:text> Ott </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dic </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Norwegian' or $Language='no'">
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
						<xsl:text> Mai </xsl:text>
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
						<xsl:text> Okt </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Des </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Irish' or $Language='en'">
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
			</xsl:when>
			<xsl:when test="$Language='Portuguese' or $Language='pt'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Jan </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Fev </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Mar </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Abr </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Mai </xsl:text>
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
						<xsl:text> Set </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='10'">
						<xsl:text> Out </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dez </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Finnish' or $Language='fi'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Tam </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Hel </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Maa </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Huh </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Tou </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='06'">
						<xsl:text> Kes </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='07'">
						<xsl:text> Hei </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='08'">
						<xsl:text> Elo </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='09'">
						<xsl:text> Syy </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='10'">
						<xsl:text> Lok </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Mar </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Jou </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Danish' or $Language='da'">
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
						<xsl:text> Maj </xsl:text>
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
						<xsl:text> Okt </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Nov </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> Dec </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Language='Croatian'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> Sij </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> Velg </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> Ozu </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> Tra </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> Svi </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='06'">
						<xsl:text> Lip </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='07'">
						<xsl:text> Srp </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='08'">
						<xsl:text> Kol </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='09'">
						<xsl:text> Ruj </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='10'">
						<xsl:text> Lis </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> Stu </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> pro </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>

			<xsl:when test="$Language='Traditional Chinese'">
				<xsl:choose>
					<xsl:when test="$MonthNumber='01'">
						<xsl:text> 一月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='02'">
						<xsl:text> 二月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='03'">
						<xsl:text> 三月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='04'">
						<xsl:text> 四月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='05'">
						<xsl:text> 五月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='06'">
						<xsl:text> 六月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='07'">
						<xsl:text> 七月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='08'">
						<xsl:text> 八月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='09'">
						<xsl:text> 九月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='10'">
						<xsl:text> 十月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='11'">
						<xsl:text> 十一月 </xsl:text>
					</xsl:when>
					<xsl:when test="$MonthNumber='12'">
						<xsl:text> 十二月 </xsl:text>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
		</xsl:choose>

		<xsl:choose>
			<xsl:when test="$FullYear = 'True'">
				<xsl:value-of select="substring($SQLDate,1,4)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring($SQLDate,3,2)"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
<!-- Time formatter-->
<xsl:template name="FormatTime">
	<xsl:param name="SQLDate"/>
		<xsl:value-of select="substring($SQLDate,12,5)"/>
</xsl:template>

<xsl:template name="FormatTimePeriod">
	<xsl:param name="Time"/>
	<xsl:param name="Format"/>
	<xsl:choose>

		<xsl:when test="$Format = '12h'">

			<xsl:variable name="Hour" select="number(substring($Time, 1,2))" />
			<xsl:variable name="Minutes" select="substring($Time, 4,2)" />

			<!-- HOUR -->
			<xsl:variable name="DisplayHour">
				<xsl:choose>
					<xsl:when test="$Hour = 0">
						<xsl:text>12</xsl:text>
					</xsl:when>
					<xsl:when test="$Hour &gt; 12">
						<xsl:value-of select="$Hour - 12" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$Hour" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<!-- MINUTES - This includes . between hours and minutes-->
			<xsl:variable name="DisplayMinute">
				<xsl:choose>
					<xsl:when test="$Minutes = '00'">
						<xsl:value-of select="''" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat('.', $Minutes)" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<!-- AM/PM -->
			<xsl:variable name="Period">
				<xsl:choose>
					<xsl:when test="$Hour &gt; 11">
						<xsl:value-of select="'pm'" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="'am'" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<!-- TIME -->
			<xsl:value-of select="concat($DisplayHour, $DisplayMinute, ' ', $Period)" />

		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$Time"/>
		</xsl:otherwise>

	</xsl:choose>
</xsl:template>
	

<!-- Format Address -->
<xsl:template name="FormatAddress">
	<xsl:param name="Address1"/>
	<xsl:param name="Address2"/>
	<xsl:param name="Address3"/>
	<xsl:param name="TownCity"/>
	<xsl:param name="County"/>
	<xsl:param name="Postcode"/>
	<xsl:param name="Country"/>
	
	<xsl:value-of select="$Address1"/>
	<xsl:if test="string-length($Address1)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$Address2"/>
	<xsl:if test="string-length($Address2)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$Address3"/>
	<xsl:if test="string-length($Address3)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$TownCity"/>
	<xsl:if test="string-length($TownCity)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$County"/>
	<xsl:if test="string-length($County)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$Postcode"/>
	<xsl:if test="string-length($Postcode)!=0"><xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text></xsl:if>
	<xsl:value-of select="$Country"/>
	
</xsl:template> 
		
	
<!-- Format Address -->
<xsl:template name="FormatAddressPDF">
	<xsl:param name="Address1"/>
	<xsl:param name="Address2"/>
	<xsl:param name="Address3"/>
	<xsl:param name="TownCity"/>
	<xsl:param name="County"/>
	<xsl:param name="Postcode"/>
	<xsl:param name="Country"/>
	
	
	<xsl:if test="string-length($Address1)!=0"><fo:block><xsl:value-of select="$Address1"/></fo:block></xsl:if>
	<xsl:if test="string-length($Address2)!=0"><fo:block><xsl:value-of select="$Address2"/></fo:block></xsl:if>
	<xsl:if test="string-length($Address3)!=0"><fo:block><xsl:value-of select="$Address3"/></fo:block></xsl:if>
	<xsl:if test="string-length($TownCity)!=0"><fo:block><xsl:value-of select="$TownCity"/></fo:block></xsl:if>
	<xsl:if test="string-length($County)!=0"><fo:block><xsl:value-of select="$County"/></fo:block></xsl:if>
	<xsl:if test="string-length($Postcode)!=0"><fo:block><xsl:value-of select="$Postcode"/></fo:block></xsl:if>
	<xsl:if test="string-length($Country)!=0"><fo:block><xsl:value-of select="$Country"/></fo:block></xsl:if>
	
</xsl:template> 	
	
<!-- Format Address -->
<xsl:template name="FormatAddressInline">
	<xsl:param name="Address1"/>
	<xsl:param name="Address2"/>
	<xsl:param name="Address3"/>
	<xsl:param name="TownCity"/>
	<xsl:param name="County"/>
	<xsl:param name="Postcode"/>
	<xsl:param name="Country"/>
	
	<xsl:variable name="Address">
		<xsl:value-of select="$Address1"/>
		<xsl:if test="string-length($Address1)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$Address2"/>
		<xsl:if test="string-length($Address2)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$Address3"/>
		<xsl:if test="string-length($Address3)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$TownCity"/>
		<xsl:if test="string-length($TownCity)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$County"/>
		<xsl:if test="string-length($County)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$Postcode"/>
		<xsl:if test="string-length($Postcode)!=0"><xsl:text>, </xsl:text></xsl:if>
		<xsl:value-of select="$Country"/>
		<xsl:if test="string-length($Country)!=0"><xsl:text>, </xsl:text></xsl:if>
	</xsl:variable>

	<xsl:call-template name="Chop">
		<xsl:with-param name="text" select="$Address"/>
		<xsl:with-param name="length" select="2"/>
	</xsl:call-template>

</xsl:template> 
	
<!--European Address Format-->
<xsl:template name="EURFormatAddress">
	<xsl:param name="Address1"/>
	<xsl:param name="Address2"/>
	<xsl:param name="TownCity"/>
	<xsl:param name="County"/>
	<xsl:param name="Postcode"/>
	<xsl:param name="Country"/>

	<xsl:value-of select="$Address2"/>
	<xsl:if test="string-length($Address2)!=0">
		<xsl:text> </xsl:text>
	</xsl:if>
	<xsl:value-of select="$Address1"/>
	<xsl:if test="string-length($Address1)!=0 or string-length($Address2)!=0">
		<xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text>
	</xsl:if>

	<xsl:value-of select="$Postcode"/>
	<xsl:if test="string-length($Postcode)!=0">
		<xsl:text> </xsl:text>
	</xsl:if>
	<xsl:value-of select="$TownCity"/>
	<xsl:if test="string-length($TownCity)!=0 or string-length($Postcode)!=0">
		<xsl:text disable-output-escaping="yes">&lt;br&#x2f;&gt;</xsl:text>
	</xsl:if>
		
	<xsl:value-of select="$County"/>
	<xsl:if test="string-length($County)!=0">
		<xsl:text> </xsl:text>
	</xsl:if>
	<xsl:value-of select="$Country"/>
		
</xsl:template>

<xsl:template name="EURFormatAddressPDF">
	<xsl:param name="Address1"/>
	<xsl:param name="Address2"/>
	<xsl:param name="TownCity"/>
	<xsl:param name="County"/>
	<xsl:param name="Postcode"/>
	<xsl:param name="Country"/>

	<xsl:if test="string-length($Address1)!=0 or string-length($Address2)!=0">
		<fo:block>
			<xsl:value-of select="$Address2"/>
			<xsl:if test="string-length($Address2)!=0">
				<xsl:text> </xsl:text>
			</xsl:if>
			<xsl:value-of select="$Address1"/>
		</fo:block>
	</xsl:if>

	<xsl:if test="string-length($TownCity)!=0 or string-length($Postcode)!=0">
		<fo:block>
			<xsl:value-of select="$Postcode"/>
			<xsl:if test="string-length($Postcode)!=0">
				<xsl:text> </xsl:text>
			</xsl:if>
			<xsl:value-of select="$TownCity"/>
		</fo:block>
	</xsl:if>

	<xsl:if test="string-length($County)!=0 or string-length($Country)!=0">
		<fo:block>
			<xsl:value-of select="$County"/>
			<xsl:if test="string-length($County)!=0">
				<xsl:text> </xsl:text>
			</xsl:if>
			<xsl:value-of select="$Country"/>
		</fo:block>
	</xsl:if>

</xsl:template>

<!-- Month -->
<xsl:template name="MonthName">
	<xsl:param name="SQLDate"/>
	<xsl:param name="FullMonth" select="'False'"/>

	<xsl:call-template name="MonthNameFromNumber">
		<xsl:with-param name="MonthNumber" select="substring($SQLDate,6,2)"/>
		<xsl:with-param name="FullMonth" select="$FullMonth"/>
	</xsl:call-template>
	
</xsl:template>
	
<xsl:template name="MonthNameFromNumber">
	<xsl:param name="MonthNumber"/>
	<xsl:param name="FullMonth" select="'False'"/>
	
	<xsl:choose>
		<xsl:when test="$MonthNumber='01'"><xsl:text>Jan</xsl:text><xsl:if test="$FullMonth!='False'">uary</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='02'"><xsl:text>Feb</xsl:text><xsl:if test="$FullMonth!='False'">ruary</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='03'"><xsl:text>Mar</xsl:text><xsl:if test="$FullMonth!='False'">ch</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='04'"><xsl:text>Apr</xsl:text><xsl:if test="$FullMonth!='False'">il</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='05'"><xsl:text>May</xsl:text></xsl:when>
		<xsl:when test="$MonthNumber='06'"><xsl:text>Jun</xsl:text><xsl:if test="$FullMonth!='False'">e</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='07'"><xsl:text>Jul</xsl:text><xsl:if test="$FullMonth!='False'">y</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='08'"><xsl:text>Aug</xsl:text><xsl:if test="$FullMonth!='False'">ust</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='09'"><xsl:text>Sep</xsl:text><xsl:if test="$FullMonth!='False'">tember</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='10'"><xsl:text>Oct</xsl:text><xsl:if test="$FullMonth!='False'">tober</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='11'"><xsl:text>Nov</xsl:text><xsl:if test="$FullMonth!='False'">vember</xsl:if></xsl:when>
		<xsl:when test="$MonthNumber='12'"><xsl:text>Dec</xsl:text><xsl:if test="$FullMonth!='False'">cember</xsl:if></xsl:when>
	</xsl:choose>
</xsl:template>

  <!-- toUpper/toLower -->
 <xsl:template name="toLower">
   <xsl:param name="word"/>
   
   <xsl:variable name="lower" select="'abcdefghijklmnopqrstuvwxyz'"/>
   <xsl:variable name="upper" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>
   <xsl:value-of select="translate($word,$upper,$lower)"/>
   
</xsl:template>

  <xsl:template name="toUpper">
    <xsl:param name="word"/>

    <xsl:variable name="lower" select="'abcdefghijklmnopqrstuvwxyz'"/>
    <xsl:variable name="upper" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'"/>

    <xsl:value-of select="translate($word,$lower,$upper)"/>
  </xsl:template>  
  
  
<!-- Multiline -->
<xsl:template name="MultiLine">
	<xsl:param name="text" select="."/>
	<xsl:param name="disableoutput" select="'no'"/>
	<xsl:choose>
		
		<xsl:when test="contains($text, '&#xA;')">
			
			<xsl:choose>
				<xsl:when test="$disableoutput='yes'"><xsl:value-of select="substring-before($text, '&#xA;')" disable-output-escaping="yes"/></xsl:when>
				<xsl:otherwise><xsl:value-of select="substring-before($text, '&#xA;')"/></xsl:otherwise>
			</xsl:choose>

      <br/><!-- Add a line break -->
		
			<!-- Process the remaining text -->
			<xsl:call-template name="MultiLine">
				<xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
				<xsl:with-param name="disableoutput" select="$disableoutput"/>
			</xsl:call-template>
			
		</xsl:when>
		
		<xsl:otherwise>			
			<xsl:choose>
				<xsl:when test="$disableoutput='yes'"><xsl:value-of select="$text" disable-output-escaping="yes"/></xsl:when>
				<xsl:otherwise><xsl:value-of select="$text"/></xsl:otherwise>
			</xsl:choose>
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
	

	<!-- Multiline pdf -->
<xsl:template name="MultiLinePDF">
	<xsl:param name="text" select="."/>
	
	<xsl:choose>
		<xsl:when test="contains($text, '&#xA;') and substring-before($text, '&#xA;')=''">
			<xsl:call-template name="MultiLinePDF">
				<xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:when test="contains($text, '&#xA;')">
			<fo:block>
				<xsl:value-of select="substring-before($text, '&#xA;')" disable-output-escaping="yes"/>			
			</fo:block>
			<xsl:call-template name="MultiLinePDF">
				<xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<fo:block>
				<xsl:value-of select="$text" disable-output-escaping="yes"/>
			</fo:block>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	

	<!--Option Selected-->
	<xsl:template name="SetOptionSelected">
		<xsl:param name="Value"/>
		<xsl:param name="Test"/>
			<xsl:choose>
				<xsl:when test="$Value = $Test">
					<option selected="selected"><xsl:value-of select="$Test"/></option>
				</xsl:when>
				<xsl:otherwise>
						<option><xsl:value-of select="$Test"/></option>
				</xsl:otherwise>
			</xsl:choose>		
	</xsl:template>

	<xsl:template name="SetOptionValueSelected">
		<xsl:param name="Value"/>
		<xsl:param name="Text"/>
		<xsl:param name="Test"/>
			<xsl:choose>
				<xsl:when test="$Value = $Test">
					<option selected="selected" value="{$Value}"><xsl:value-of select="$Text"/></option>
				</xsl:when>
				<xsl:otherwise>
					<option value="{$Value}"><xsl:value-of select="$Text"/></option>
					<!--<option value="{$Value}"><xsl:text>XXX</xsl:text></option>-->
				</xsl:otherwise>
			</xsl:choose>
	</xsl:template>

	<xsl:template name="FormatBoolean">
		<xsl:param name="Value"/>
		<xsl:choose>
			<xsl:when test="$Value=1">
				<xsl:text>Yes</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>No</xsl:text>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>
	
	<!--Star Rating-->
	<xsl:template name="FormatStarRating">
		<xsl:param name="Value"/>
		<xsl:choose>
			<xsl:when test="$Value=1.0">
				<xsl:text>1 Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=1.5">
				<xsl:text>1½ Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=2.0">
				<xsl:text>2 Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=2.5">
				<xsl:text>2½ Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=3.0">
				<xsl:text>3 Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=3.5">
				<xsl:text>3½ Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=4.0">
				<xsl:text>4 Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=4.5">
				<xsl:text>4½ Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=5.0">
				<xsl:text>5 Star</xsl:text>
			</xsl:when>
			<xsl:when test="$Value=5.5">
				<xsl:text>5½ Star</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text></xsl:text>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>
	
	<xsl:template name="FormatMoney">
		<xsl:param name="Currency"/>
		<xsl:param name="Value"/>
		<xsl:param name="CurrencySymbolPosition" select="'Prepend'"/>
		<xsl:param name="SymbolGap" select="'false'"/>
		
		<xsl:variable name="Sign">
			<xsl:if test="$Value &lt; 0">-</xsl:if>
		</xsl:variable>
		<xsl:variable name="NewValue">
			<xsl:choose>
				<xsl:when test="$Value &lt; 0"><xsl:value-of select="$Value * -1"/></xsl:when>
				<xsl:otherwise><xsl:value-of select="$Value"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:choose>
		  <xsl:when test="$SymbolGap = 'true' and $CurrencySymbolPosition='Append'">
			<xsl:value-of select="concat($Sign,format-number($NewValue,'###,###,##0.00'),' ',$Currency)"/>
		  </xsl:when>
		  <xsl:when test="$SymbolGap = 'true' and $CurrencySymbolPosition='Prepend'">
			<xsl:value-of select="concat($Sign,$Currency,' ',format-number($NewValue,'###,###,##0.00'))"/>
		  </xsl:when>
		  <xsl:when test="$CurrencySymbolPosition='Append'">
			<xsl:value-of select="concat($Sign,format-number($NewValue,'###,###,##0.00'),$Currency)"/>
		  </xsl:when>
		  <xsl:when test="$CurrencySymbolPosition='Prepend'">
			<xsl:value-of select="concat($Sign,$Currency,format-number($NewValue,'###,###,##0.00'))"/>    
		  </xsl:when>
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
		<xsl:param name="Length"/>
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
	<xsl:param name="newline" select="' '"/>
	
	<xsl:if test="$Count=1"><xsl:value-of select="$newline"/></xsl:if>
	<xsl:text>-</xsl:text>
	<xsl:if test="$Count=string-length($Base)"><xsl:value-of select="$newline"/></xsl:if>
	
	<xsl:if test="$Count &lt; string-length($Base)">
		<xsl:call-template name="underline">
			<xsl:with-param name="Base" select="$Base"/>
			<xsl:with-param name="Count" select="$Count+1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template name="underlinenobase">
	<xsl:param name="Length"/>
	<xsl:param name="Count" select="1"/>
	
	<xsl:if test="$Count &lt;= $Length">
		<xsl:value-of select="'-'"/>
		<xsl:call-template name="underlinenobase">
			<xsl:with-param name="Length" select="$Length"/>
			<xsl:with-param name="Count" select="$Count+1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<!-- ABS template -->
<xsl:template name="ABS">
	<xsl:param name="Number"/>
	
	<xsl:choose>
		<xsl:when test="$Number &lt; 0"><xsl:value-of select="$Number * -1"/></xsl:when>
		<xsl:otherwise><xsl:value-of select="$Number"/></xsl:otherwise>
	</xsl:choose>
</xsl:template>
	
	
<!-- Guest Name -->
<xsl:template name="GuestName">
	
	<!-- work out if we're at lead guest (booking level) or not -->
	<xsl:if test="name()='Booking' or name()='Property'">
		<xsl:if test="LeadGuestTitle!='TBA'"><xsl:value-of select="concat(LeadGuestTitle,' ')"/></xsl:if>
		<xsl:if test="LeadGuestFirstName!='TBA'"><xsl:value-of select="concat(LeadGuestFirstName,' ')"/></xsl:if>
		<xsl:if test="LeadGuestLastName!='TBA'"><xsl:value-of select="LeadGuestLastName"/></xsl:if>
	</xsl:if>

	<xsl:if test="name()!='Booking' and name()!='Property'">
		<xsl:if test="Title!='TBA'"><xsl:value-of select="concat(Title,' ')"/></xsl:if>
		<xsl:if test="FirstName!='TBA'"><xsl:value-of select="concat(FirstName,' ')"/></xsl:if>
		<xsl:if test="LastName!='TBA'"><xsl:value-of select="LastName"/></xsl:if>
	</xsl:if>
	
	
</xsl:template>
	

	
<!-- Booking Passenger Template-->	
<xsl:template name="BookingPassengerSummary">
	<xsl:param name="Language"/>
	
	<xsl:variable name="Adult">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Adult</xsl:when>
			<xsl:when test="$Language='Dutch'">Volwassene</xsl:when>
			<xsl:when test="$Language='French'">Adulte</xsl:when>
			<xsl:when test="$Language='German'">Erwachsener</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="AdultPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Adults</xsl:when>
			<xsl:when test="$Language='Dutch'">Volwassenen</xsl:when>
			<xsl:when test="$Language='French'">Adultes</xsl:when>
			<xsl:when test="$Language='German'">Erwachsene</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Child">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Child</xsl:when>
			<xsl:when test="$Language='Dutch'">Kind</xsl:when>
			<xsl:when test="$Language='French'">Enfant</xsl:when>
			<xsl:when test="$Language='German'">Kind</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="ChildPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Children</xsl:when>
			<xsl:when test="$Language='Dutch'">Kinderen</xsl:when>
			<xsl:when test="$Language='French'">Enfants</xsl:when>
			<xsl:when test="$Language='German'">Kinder</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Infant">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Infant</xsl:when>
			<xsl:when test="$Language='Dutch'">Baby</xsl:when>
			<xsl:when test="$Language='French'">Bébé</xsl:when>
			<xsl:when test="$Language='German'">Kleinkind</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="InfantPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Infants</xsl:when>
			<xsl:when test="$Language='Dutch'">Baby's</xsl:when>
			<xsl:when test="$Language='French'">Bébés</xsl:when>
			<xsl:when test="$Language='German'">Kleinkinder</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Years">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">yrs</xsl:when>
			<xsl:when test="$Language='Dutch'">jaar</xsl:when>
			<xsl:when test="$Language='French'">ans</xsl:when>
		</xsl:choose>
	</xsl:variable>

	
	<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Adult']) > 0">
		<xsl:value-of select="count(BookingPassengers/BookingPassenger[GuestType='Adult'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Adult']) = 1"><xsl:value-of select="$Adult"/></xsl:if>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Adult']) > 1"><xsl:value-of select="$AdultPlural"/></xsl:if>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType!='Adult']) > 0"><xsl:text>, </xsl:text></xsl:if>
	</xsl:if>
	
	<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Child']) > 0">
		<xsl:value-of select="count(BookingPassengers/BookingPassenger[GuestType='Child'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Child']) = 1"><xsl:value-of select="$Child"/></xsl:if>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Child']) > 1"><xsl:value-of select="$ChildPlural"/></xsl:if>
		
		<!-- child ages -->
		<xsl:text> (</xsl:text>
		<xsl:for-each select="BookingPassengers/BookingPassenger[GuestType='Child']">
			<xsl:value-of select="concat(GuestAge,' ',$Years)"/>
			<xsl:if test="position() != last() and count(../BookingPassenger[GuestType='Child']) > 1">, </xsl:if>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
		
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Infant']) > 0"><xsl:text>, </xsl:text></xsl:if>
	</xsl:if>
	
	<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Infant']) > 0">
		<xsl:value-of select="count(BookingPassengers/BookingPassenger[GuestType='Infant'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Infant']) = 1"><xsl:value-of select="$Infant"/></xsl:if>
		<xsl:if test="count(BookingPassengers/BookingPassenger[GuestType='Infant']) > 1"><xsl:value-of select="$InfantPlural"/></xsl:if>
	</xsl:if>	
</xsl:template>
	
	
<!-- Property Room Guest Template-->	
<xsl:template name="PropertyRoomGuestSummary">
	<xsl:param name="Language"/>
	
	
	<xsl:variable name="Adult">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Adult</xsl:when>
			<xsl:when test="$Language='Dutch'">Volwassene</xsl:when>
			<xsl:when test="$Language='French'">Adulte</xsl:when>
			<xsl:when test="$Language='German'">Erwachsener</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="AdultPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Adults</xsl:when>
			<xsl:when test="$Language='Dutch'">Volwassenen</xsl:when>
			<xsl:when test="$Language='French'">Adultes</xsl:when>
			<xsl:when test="$Language='German'">Erwachsene</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Child">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Child</xsl:when>
			<xsl:when test="$Language='Dutch'">Kind</xsl:when>
			<xsl:when test="$Language='French'">Enfant</xsl:when>
			<xsl:when test="$Language='German'">Kind</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="ChildPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Children</xsl:when>
			<xsl:when test="$Language='Dutch'">Kinderen</xsl:when>
			<xsl:when test="$Language='French'">Enfants</xsl:when>
			<xsl:when test="$Language='German'">Kinder</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Infant">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Infant</xsl:when>
			<xsl:when test="$Language='Dutch'">Baby</xsl:when>
			<xsl:when test="$Language='French'">Bébé</xsl:when>
			<xsl:when test="$Language='German'">Kleinkind</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="InfantPlural">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">Infants</xsl:when>
			<xsl:when test="$Language='Dutch'">Baby's</xsl:when>
			<xsl:when test="$Language='French'">Bébés</xsl:when>
			<xsl:when test="$Language='German'">Kleinkinder</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Years">
		<xsl:choose>
			<xsl:when test="$Language='' or $Language='English'">yrs</xsl:when>
			<xsl:when test="$Language='Dutch'">jaar</xsl:when>
			<xsl:when test="$Language='French'">ans</xsl:when>
		</xsl:choose>
	</xsl:variable>
	
	
	<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Adult']) > 0">
		<xsl:value-of select="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Adult'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Adult']) = 1"><xsl:value-of select="$Adult"/></xsl:if>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Adult']) > 1"><xsl:value-of select="$AdultPlural"/></xsl:if>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType!='Adult']) > 0"><xsl:text>, </xsl:text></xsl:if>
	</xsl:if>
	
	<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Child']) > 0">
		<xsl:value-of select="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Child'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Child']) = 1"><xsl:value-of select="$Child"/></xsl:if>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Child']) > 1"><xsl:value-of select="$ChildPlural"/></xsl:if>
		
		<!-- child ages -->
		<xsl:text> (</xsl:text>
		<xsl:for-each select="PropertyRoomGuests/PropertyRoomGuest[GuestType='Child']">
			<xsl:value-of select="concat(GuestAge,' ',$Years)"/>
			<xsl:if test="position() != last() and count(../PropertyRoomGuest[GuestType='Child']) > 1">, </xsl:if>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
		
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Infant']) > 0"><xsl:text>, </xsl:text></xsl:if>
	</xsl:if>
	
	<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Infant']) > 0">
		<xsl:value-of select="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Infant'])"/>
		<xsl:text> </xsl:text>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Infant']) = 1"><xsl:value-of select="$Infant"/></xsl:if>
		<xsl:if test="count(PropertyRoomGuests/PropertyRoomGuest[GuestType='Infant']) > 1"><xsl:value-of select="$InfantPlural"/></xsl:if>
	</xsl:if>	
</xsl:template>

<!-- Capitalise First Letter -->
<xsl:template name="CaseConvert">
	<xsl:param name="text"/>

	<xsl:variable name="item">
		<xsl:choose>
			<xsl:when test="contains($text,' ')">
				<xsl:value-of select="substring-before($text, ' ')" disable-output-escaping="yes"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text" disable-output-escaping="yes"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="remainder">
		<xsl:if test="contains($text,' ')">
			<xsl:value-of select="substring-after($text,' ')" disable-output-escaping="yes"/>
		</xsl:if>
	</xsl:variable>

	<xsl:value-of select="translate(substring($item,1,1),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/>
	<xsl:value-of select="translate(substring($item,2),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>
	
	<xsl:if test="$remainder!=''">
		<xsl:text> </xsl:text>
		<xsl:call-template name="CaseConvert">
			<xsl:with-param name="text" select="$remainder"/>
		</xsl:call-template>
	</xsl:if>
	
	
</xsl:template>

<xsl:template name="Chop">
	<xsl:param name="text"/>
	<xsl:param name="length" select="1"/>
	<xsl:value-of select="substring($text,1,string-length($text)-$length)"/>
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
	
	
<xsl:template name="MultipleReplace">
    <xsl:param name="text"/>
    <xsl:param name="find"/>
    <xsl:param name="replacements"/>

    <xsl:variable name="CurrentFind">
        <xsl:choose>
            <xsl:when test="contains($find,'|')"><xsl:value-of select="substring-before($find,'|')"/></xsl:when>
            <xsl:otherwise><xsl:value-of select="$find"/></xsl:otherwise>
        </xsl:choose>
    </xsl:variable>
    <xsl:variable name="CurrentReplacement">
        <xsl:choose>
            <xsl:when test="contains($replacements,'|')"><xsl:value-of select="substring-before($replacements,'|')"/></xsl:when>
            <xsl:otherwise><xsl:value-of select="$replacements"/></xsl:otherwise>
        </xsl:choose>
    </xsl:variable>

    <xsl:if test="$CurrentFind != ''">
        <xsl:call-template name="Replace">
            <xsl:with-param name="find" select="$CurrentFind"/>
            <xsl:with-param name="replacement" select="$CurrentReplacement"/>
            <xsl:with-param name="text">
                <xsl:choose>
                    <xsl:when test="contains($find,'|')">
                        <xsl:call-template name="MultipleReplace">
                            <xsl:with-param name="text" select="$text"/>
                            <xsl:with-param name="find" select="substring-after($find,'|')"/>
                            <xsl:with-param name="replacements">
                                <xsl:choose>
                                    <xsl:when test="contains($replacements,'|')">
                                        <xsl:value-of select="substring-after($replacements,'|')"/>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <xsl:value-of select="$replacements"/>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:with-param>
                        </xsl:call-template>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of select="$text"/></xsl:otherwise>
                </xsl:choose>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:if>
</xsl:template>


<xsl:template name="CumulativeNodeSum">
  
  <xsl:param name="NodeSet" />
  <xsl:param name="Param1Name"/>
  <xsl:param name="Param2Name"/>
  <xsl:param name="CurrentSum" select="number(0)"/>
  <xsl:param name="NodeCount" select="count($NodeSet)"/>
  
  <xsl:variable name="Param1" select="number($NodeSet[number($NodeCount)]/*[name()=$Param1Name])"/>
  <xsl:variable name="Param2" select="number($NodeSet[number($NodeCount)]/*[name()=$Param2Name])"/>

  <xsl:variable name="NodeSum" select="number($Param1 * $Param2)"/>
  <xsl:variable name="CycleSum" select="number($CurrentSum + $NodeSum)"/>

  <xsl:choose>
    <xsl:when test="number($NodeCount - 1) > 0 ">
      <xsl:call-template name="CumulativeNodeSum">
        <xsl:with-param name="CurrentSum" select="$CycleSum"/>
        <xsl:with-param name="NodeCount" select="number($NodeCount - 1)"/>
        <xsl:with-param name="NodeSet" select="$NodeSet"/>
        <xsl:with-param name="Param1Name" select="$Param1Name"/>
        <xsl:with-param name="Param2Name" select="$Param2Name"/>
      </xsl:call-template>
    </xsl:when>
    <xsl:otherwise>
      <xsl:value-of select="$CycleSum"/>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>


</xsl:stylesheet>
  

  

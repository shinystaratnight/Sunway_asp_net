<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:include href="../../xsl/functions.xsl "/>
	
	<xsl:param name="CurrencySymbol" />
	<xsl:param name="CurrencySymbolPosition" />
	<xsl:param name="CSSClassOverride" />
	<xsl:param name="Text" />
	<xsl:param name="InjectDiv" />
	<xsl:param name="UpdateTotalPriceDisplay" />
	
<xsl:template match="/">

	<div id="divDeposit" >

    <xsl:attribute name ="class">
      <xsl:choose>
        <xsl:when test="$CSSClassOverride != ''">
          <xsl:value-of select="$CSSClassOverride" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>box primary</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>

	<input type="hidden" id="hidDepositAmount" value="{BookingBasket/AmountDueToday}" />
	<input type="hidden" id="hidTotalAmount" value="{BookingBasket/TotalPrice}" />
	<input type="hidden" id="hidUpdateTotalPriceDisplay" value="{$UpdateTotalPriceDisplay}" />
		
    <div class="boxTitle">
			<h2 ml="Deposit">Deposit</h2>
		</div>

		<xsl:choose>
			<xsl:when test="$Text != ''">
				<p id="pDepositText">
					<xsl:value-of select="$Text" />
				</p>
			</xsl:when>
			<xsl:otherwise>
				<p id="pDepositText" ml="Deposit">Secure your holiday with a low deposit</p>
			</xsl:otherwise>
		</xsl:choose>		

		<label id="lblDeposit" class="radioLabel" for="radUseDeposit" onclick="web.CustomInputs.ToggleRadioLabel(this, 'deposit');">
			<xsl:if test="BookingBasket/PayDeposit = 'true'">
				<xsl:attribute name="class">radioLabel selected</xsl:attribute>
			</xsl:if>
			
			
			<input type="radio" id="radUseDeposit" class="radio" name="deposit" onclick="Deposit.PayDeposit(true);int.f.Show('pDueDate_Deposit');">				
				<xsl:if test="BookingBasket/PayDeposit = 'true'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>			
			
			<trans ml="Deposit">Secure your booking with a deposit of</trans>
			<span class="price updateablePrice" >
				<xsl:call-template name="GetSellingPrice">
					<xsl:with-param name="Value" select="BookingBasket/AmountDueToday" />
					<xsl:with-param name="Format" select="'######'" />
					<xsl:with-param name="Currency" select="$CurrencySymbol" />
					<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
				</xsl:call-template>
			</span>
			
		</label>
		
		<!-- Better solution to get latest due date anyone? - AndyF -->
		<xsl:variable name="DueDate">
				
			<xsl:variable name="LatestPropertyPaymentDue">
				<xsl:for-each select="BookingBasket/PreBookResponse/PropertyBookings/PropertyPreBookResponse/PaymentsDue/PaymentDue">
					<xsl:sort select="DateDue" order="descending"/>
					<xsl:if test="position() = 1">
						<xsl:value-of select="substring(translate(DateDue, '-', ''), 1, 8)"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:variable>
				
			<xsl:variable name="LatestFlightPaymentDue">
				<xsl:for-each select="BookingBasket/PreBookResponse/FlightBookings/FlightPreBookResponse/PaymentsDue/PaymentDue">
					<xsl:sort select="DateDue" order="descending"/>
					<xsl:if test="position() = 1">
						<xsl:value-of select="substring(translate(DateDue, '-', ''), 1, 8)"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:variable>
				
			<xsl:variable name="LatestDate">
				<xsl:choose>
					<xsl:when test="$LatestFlightPaymentDue &gt; $LatestPropertyPaymentDue">
						<xsl:call-template name="ShortCondensedDate">
							<xsl:with-param name="Date" select="$LatestFlightPaymentDue" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="ShortCondensedDate">
							<xsl:with-param name="Date" select="$LatestPropertyPaymentDue" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
				
			<xsl:value-of select="$LatestDate"/>
				
		</xsl:variable>
			
		<p id="pDueDate_Deposit" ml="Deposit">
			<xsl:if test="BookingBasket/PayDeposit = 'false'">
				<xsl:attribute name="style">display:none;</xsl:attribute>
			</xsl:if>					
			<trans ml="Deposit">(Balance due by</trans>
			<xsl:text> </xsl:text>
			<xsl:value-of select="$DueDate"/>
			<xsl:text>)</xsl:text>
		</p>

		<label id="lblPayFull" class="radioLabel" for="radPayFull" onclick="web.CustomInputs.ToggleRadioLabel(this, 'deposit');">
			<xsl:if test="BookingBasket/PayDeposit = 'false'">
				<xsl:attribute name="class">radioLabel selected</xsl:attribute>
			</xsl:if>

			<input type="radio" id="radPayFull" class="radio" name="deposit" onclick="Deposit.PayDeposit(false);int.f.Hide('pDueDate_Deposit');">
				<xsl:if test="BookingBasket/PayDeposit = 'false'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>
			<trans ml="Deposit">Pay the full amount of</trans>
			<span class="price updateablePrice">
				<xsl:call-template name="GetSellingPrice">
					<xsl:with-param name="Value" select="BookingBasket/TotalPrice" />
					<xsl:with-param name="Format" select="'######'" />
					<xsl:with-param name="Currency" select="$CurrencySymbol" />
					<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
				</xsl:call-template>
			</span>
		</label>
		
	</div>

	<script type="text/javascript">
		int.ll.OnLoad.Run(function () { Deposit.Setup('<xsl:value-of select="$InjectDiv"/>'); });
	</script>
	
</xsl:template>
	
<xsl:template name="ShortCondensedDate">
		<xsl:param name="Date"/>
		<xsl:variable name="MonthNumber" select="substring($Date,5,2)"/>

		<xsl:value-of select="substring($Date,7,2)"/>
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
		<xsl:value-of select="substring($Date,3,2)"/>

	</xsl:template>

</xsl:stylesheet>
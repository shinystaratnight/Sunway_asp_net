<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

	<xsl:param name="CancellationChargesPosition" />
	<xsl:param name="PropertyReferenceID" />
	<xsl:param name="Index" />

	<xsl:param name="CurrencySymbol"  />
	<xsl:param name="CurrencySymbolPosition"  />
	<xsl:param name="DateToday" />
	<xsl:param name="DepartureDate" />
	<xsl:param name="UseRoomMapping" />
	
	<xsl:include href="../CancellationCharges/CancellationCharges.xsl"/>	

    <xsl:template match="/">

			<table class="cancellationCosts" cellpadding="0" cellspacing="0">
				<xsl:for-each select="ArrayOfCancellation/Cancellation">
					<xsl:sort select="StartDate"/>
					<xsl:call-template name="Cancellation">
						<xsl:with-param name="DateToday" select="$DateToday" />
						<xsl:with-param name="DepartureDate" select="$DepartureDate" />
						<xsl:with-param name="StartDate" select="StartDate" />
						<xsl:with-param name="EndDate" select="EndDate" />
						<xsl:with-param name="CurrencySymbol" select="$CurrencySymbol" />
						<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />
						<xsl:with-param name="UseRoomMapping" select="$UseRoomMapping" />
					</xsl:call-template>
				</xsl:for-each>
			</table>

            <xsl:if test ="ArrayOfCancellation/ArrayOfErratum/Erratum and not($UseRoomMapping)">
							<div class="errata">
                <h5>Information</h5>
                <xsl:for-each select ="ArrayOfCancellation/ArrayOfErratum/Erratum">
									<p>
										<xsl:value-of select ="ErratumDescription"/>
									</p>
                </xsl:for-each>
							</div>
            </xsl:if>

			<xsl:if test="not($UseRoomMapping)">
				<a class="close" href="javascript:HotelResults.HideCancellationCharges('{$CancellationChargesPosition}', {$PropertyReferenceID},'{$Index}')" ml="Hotel Results">hide</a>
			</xsl:if>

		<div class="clearing">
			<xsl:text> </xsl:text>
		</div>
		
    </xsl:template>



	<xsl:template name="Cancellation">
		<xsl:param name="DateToday" />
		<xsl:param name="DepartureDate" />
		<xsl:param name="StartDate" />
		<xsl:param name="EndDate" />
		<xsl:param name="CurrencySymbol" />
		<xsl:param name="CurrencySymbolPosition"  />
		<xsl:param name="UseRoomMapping" />

		<xsl:variable name="EndDateNumber" select="number(concat(substring($EndDate,1,4),substring($EndDate,6,2),substring($EndDate,9,2)))" />
		<xsl:variable name="DepartureDateNumber" select="number(concat(substring($DepartureDate,1,4),substring($DepartureDate,6,2),substring($DepartureDate,9,2)))" />
    <xsl:variable name="TodaysDateNumber" select="number(concat(substring($DateToday,1,4),substring($DateToday,6,2),substring($DateToday,9,2)))" />
    <xsl:variable name="StartDateNumber" select="number(concat(substring($StartDate,1,4),substring($StartDate,6,2),substring($StartDate,9,2)))" />
    <xsl:if test="$TodaysDateNumber &lt; $EndDateNumber">
      <tr>
			<xsl:variable name="noBorder">
				<xsl:choose>
					<xsl:when test="$UseRoomMapping">noBorder</xsl:when>
					<xsl:otherwise></xsl:otherwise>
				</xsl:choose>
			</xsl:variable>

			<td class="date {$noBorder}" ml="CancellationCharges" mlparams="{$EndDate}~ShortDate|{$StartDate}~ShortDate|{$DateToday}~ShortDate">

				<xsl:choose>
          <xsl:when test="($StartDateNumber &lt;= $TodaysDateNumber)">
            <xsl:text>Today</xsl:text>
          </xsl:when>
          <xsl:when test="(substring($StartDate,1,10) = substring($DateToday,1,10)) and ($EndDateNumber &lt; $DepartureDateNumber)">
						<xsl:text>{2} to {0}</xsl:text>
					</xsl:when>
					<xsl:when test="(substring($StartDate,1,10) = substring($DateToday,1,10)) and ($EndDateNumber &gt; $DepartureDateNumber)">
						<xsl:text>{1}</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>{1}</xsl:text>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:choose>
					<xsl:when test="($EndDateNumber &gt; $DepartureDateNumber)">
						<xsl:text> onwards</xsl:text>
					</xsl:when>
					<xsl:when test="substring($StartDate,1,10) != substring($DateToday,1,10)">
						<xsl:text> to {0}</xsl:text>
					</xsl:when>
				</xsl:choose>

			</td>

			<td>
				<xsl:choose>
					<xsl:when test="Amount = 0">
						<xsl:attribute name ="class">
							<xsl:text>noCharge </xsl:text>
							<xsl:value-of select="$noBorder"/>
						</xsl:attribute>
						<trans ml="CancellationCharges">No Charge</trans>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name ="class">
							<xsl:text>charge</xsl:text>
						</xsl:attribute>
						<trans ml="CancellationCharges">You will be charged</trans>
						<xsl:text> </xsl:text>
						<strong>
							<xsl:value-of select="concat($CurrencySymbol,format-number(Amount,'###,###0.00'))"/>
						</strong>
					</xsl:otherwise>
				</xsl:choose>
			</td>

		</tr>
    </xsl:if>
	</xsl:template>


</xsl:stylesheet>

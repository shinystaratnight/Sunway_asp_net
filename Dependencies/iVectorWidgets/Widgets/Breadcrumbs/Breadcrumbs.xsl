<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
<xsl:param name="BaseURL" />
<xsl:param name="BookingPathway" select="'false'"/>
<xsl:param name="CurrentPage" />
<xsl:param name="SearchMode" />
<xsl:param name="ExtraTypes" />
<xsl:param name="UseAccessibleURLs" />
<xsl:param name="SeparateXML" />
<xsl:param name="UseNumbers" />
<xsl:param name="ObjectType" />
	
	
<xsl:variable name="propertyurl">
	<xsl:choose>
		<xsl:when test="$ObjectType = 'Property'">
			<xsl:value-of select="$CurrentPage"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="/Breadcrumbs/BookingBasket/BasketProperties/BasketProperty/ContentXML/Hotel/URL"/>	
		</xsl:otherwise>
	</xsl:choose>	
</xsl:variable>
		
<xsl:variable name="currentpos">	
	<xsl:choose>
		<xsl:when test="$CurrentPage = $propertyurl">
			<xsl:value-of select="/Breadcrumbs/Breadcrumb[SetURLFromBasketProperty='true']/Position"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="/Breadcrumbs/Breadcrumb[URL = $CurrentPage]/Position"/>
		</xsl:otherwise>		
	</xsl:choose>		
</xsl:variable>
	
<xsl:template match="/">
	
	<div>
		<xsl:attribute name="class">
			<xsl:choose>
				<xsl:when test="$BookingPathway = 'true'">
					<xsl:text>breadcrumbs bookingPathway </xsl:text>
					<xsl:value-of select="$SearchMode"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="substring-after($CurrentPage, '/')"/>
				</xsl:when>
				<xsl:otherwise>breadcrumbs</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
		
		<ul>		
			<xsl:for-each select="Breadcrumbs/Breadcrumb[$BookingPathway = 'false' or SearchModes = 'All' or (contains(SearchModes, $SearchMode) and (count(ExtraTypes/ExtraType) = 0 or count(ExtraTypes/ExtraType[contains($ExtraTypes, .)]) > 0))]">
				
				<xsl:variable name="IsCurrentPage">
					<xsl:choose>
						<xsl:when test="URL = $CurrentPage or ($CurrentPage = $propertyurl and SetURLFromBasketProperty='true') or $CurrentPage = Override">
							<xsl:text>true</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>false</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				
				<xsl:choose>
					<!-- SEO breadcrumbs -->
					
					<xsl:when test="$BookingPathway = 'false'">
						<xsl:call-template name="Crumb">
							<xsl:with-param name="Type" select="Type" />
							<xsl:with-param name="Position" select="position()" />
							<xsl:with-param name="UseNumbers" select="$UseNumbers" />
						</xsl:call-template>
					</xsl:when>
					
					<!-- Booking journey breadcrumbs-->
					<xsl:otherwise>
						
						<xsl:variable name ="URLTEST">
							<xsl:value-of select="URL"/>
						</xsl:variable>
						

						<xsl:if test ="not(../Breadcrumb[Override = $URLTEST])">
							<xsl:variable name="type">
								<xsl:choose>
									<xsl:when test="$IsCurrentPage = 'true'">current</xsl:when>
									<xsl:when test="Position &gt; $currentpos or $CurrentPage='/confirmation'">future</xsl:when>
									<xsl:when test="Position &lt; $currentpos">previous</xsl:when>
								</xsl:choose>
							</xsl:variable>

							<xsl:if test="SearchModes = 'All' or contains(SearchModes, $SearchMode)">
								<xsl:call-template name="Crumb">
									<xsl:with-param name="Type" select="$type" />	
									<xsl:with-param name="Position" select="position()" />
									<xsl:with-param name="UseNumbers" select="$UseNumbers" />
								</xsl:call-template>
							</xsl:if>
							
						</xsl:if>

					</xsl:otherwise>
				</xsl:choose>

			</xsl:for-each>
		</ul>
		
	</div>

</xsl:template>


<xsl:template name="Crumb">
	<xsl:param name="Type" />
	<xsl:param name="Position" />
	<xsl:param name="UseNumbers" />
	

	<xsl:variable name="url">
		<xsl:choose>
			<xsl:when test="SetURLFromBasketProperty = 'true'">
				<xsl:value-of select="$propertyurl"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="URL"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="Class">
		<xsl:choose>
			<xsl:when test="$CurrentPage = $propertyurl and Position = $currentpos - 1">
				<xsl:value-of select="concat($Type, ' last')"/>
			</xsl:when>
			<xsl:when test="Position = $currentpos - 1">
				<xsl:value-of select="concat($Type, ' last')"/>
			</xsl:when>
			<xsl:when test="($Type = 'previous' and $UseAccessibleURLs = 'true' and count(/Breadcrumbs/Breadcrumb[URL = $CurrentPage]/AccessibleURLs/AccessibleURL[URL = $url]) = 0) or URL = ''">
				disabled
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$Type"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<li>
		
		<xsl:attribute name="class">
			<xsl:choose>
				<xsl:when test="position() = last()">
					<xsl:value-of select="concat($Class, ' final')"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$Class"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>		
		
		<a>		
			
			<xsl:choose>
				<xsl:when test="($UseAccessibleURLs = 'true' and count(/Breadcrumbs/Breadcrumb[URL = $CurrentPage]/AccessibleURLs/AccessibleURL[URL = $url]) &gt; 0) or ($UseAccessibleURLs != 'true' and $Type = 'previous')">

					<xsl:attribute name="href">
						<xsl:choose>
							<xsl:when test="URL = ''">
								<xsl:text>javascript:void(0);</xsl:text>
							</xsl:when>
							<xsl:when test="contains(URL, 'http')">
								<xsl:value-of select="URL"/>
							</xsl:when>
							<xsl:when test="SetURLFromBasketProperty = 'true'">
								<xsl:value-of select="concat($BaseURL, $propertyurl)"/>
							</xsl:when>
							<xsl:when test="UseBaseURL = 'false'">
								<xsl:value-of select="URL"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="concat($BaseURL, URL)"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
										
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="href">javascript:void(0)</xsl:attribute>
					<!--<xsl:attribute name="onclick">return false;</xsl:attribute>-->			
				</xsl:otherwise>
			</xsl:choose>

			<xsl:if test="$UseNumbers = 'true'">
				<span class="number">
					<xsl:value-of select="$Position" />
				</span>
			</xsl:if>			
			
			<xsl:value-of select="Name"/>
		</a>

		<xsl:if test="position() != last()">
			<span class="divider">
				<xsl:text> </xsl:text>
			</span>
		</xsl:if>
	</li>

</xsl:template>	

</xsl:stylesheet>
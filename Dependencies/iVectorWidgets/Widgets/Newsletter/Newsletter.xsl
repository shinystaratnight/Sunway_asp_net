<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="CSSClassOverride" />
	<xsl:param name="InjectContainer" />
	<xsl:param name="ErrorStyle" />
	
	
	<xsl:template match="/">	
			
		<div id="divNewsletter">			
			<xsl:if test="$InjectContainer != ''">
				<xsl:attribute name="style"><xsl:text>display:none;</xsl:text></xsl:attribute>
			</xsl:if>

			<xsl:attribute name ="class">
				<xsl:choose>
					<xsl:when test="$CSSClassOverride != ''">
						<xsl:value-of select="$CSSClassOverride" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>box </xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>

			<div class="boxTitle">
				<h2>
					<xsl:choose>
						<xsl:when test="TextOverrides/Title != ''">
							<trans ml="Newsletter">
								<xsl:value-of select="TextOverrides/Title"/>
							</trans>
						</xsl:when>
						<xsl:otherwise>
							<trans ml="Newsletter">Sign up to news</trans>
						</xsl:otherwise>
					</xsl:choose>
				</h2>
			</div>

			<p ml="Newsletter">
				<xsl:choose>
					<xsl:when test="TextOverrides/Text != ''">
						<trans ml="Newsletter">
							<xsl:value-of select="TextOverrides/Text"/>
						</trans>
					</xsl:when>
					<xsl:otherwise>
						<trans ml="Newsletter">Sign up to receive our newsletter emails.</trans>
					</xsl:otherwise>
				</xsl:choose>
			</p>

			<div class="textbox icon">
				<input id="txtSubscribeEmail" type="email" class="textbox" placeholder="{TextOverrides/Placeholder}" />
				<i><xsl:text> </xsl:text></i>
			</div>

			<xsl:if test="TextOverrides/Placeholder != ''">
				<input type="hidden" id="hidSubscribeEmail_Placeholder" value="{TextOverrides/Placeholder}" />
			</xsl:if>

			<a class="button secondary" id="aNewsLetterSubmit" href="#" onclick="Newsletter.Submit();return false;">
				<xsl:choose>
					<xsl:when test="TextOverrides/ButtonText != ''">
						<trans ml="Newsletter">
							<xsl:value-of select="TextOverrides/ButtonText"/>
						</trans>
					</xsl:when>
					<xsl:otherwise>
						<trans ml="Newsletter">Sign Up</trans>
					</xsl:otherwise>
				</xsl:choose>
			</a>

			<p id="pSuccess" style="display:none;" ml="Newsletter">You were signed up successfully.</p>
			<p id="pFailure" style="display:none;" ml="Newsletter">Signup failed - Please check email address is correct.</p>

			<input type="hidden" id="hidNewsletterErrorStyle" value="{$ErrorStyle}" />
			
		</div>
		
		
		<xsl:choose>
			<xsl:when test="$InjectContainer != ''">
				<script type="text/javascript">
					int.ll.OnLoad.Run(function () { Newsletter.Inject('<xsl:value-of select="$InjectContainer"/>'); });
				</script>					
			</xsl:when>
			<xsl:otherwise>
				<script type="text/javascript">
					int.ll.OnLoad.Run(function () { Newsletter.Setup(); });
				</script>
			</xsl:otherwise>				
		</xsl:choose>
		

	</xsl:template>
</xsl:stylesheet>
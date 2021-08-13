<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" omit-xml-declaration="yes" indent="yes"/>


	<!-- Markdown -->
	<xsl:template name="Markdown">
		<xsl:param name="text"/>
		<xsl:param name="mode"/>
		<xsl:param name="SuppressLinks" select="'false'"/>
	    <xsl:param name="MaxLoops" select="'10000'"/>
	    <xsl:param name="CurrentLoop" select="'1'"/>



		<xsl:variable name="line">
			<xsl:choose>
				<xsl:when test="contains($text,'&#xA;')">
					<xsl:value-of select="substring-before($text, '&#xA;')" disable-output-escaping="yes"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$text" disable-output-escaping="yes"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="remainder">
			<xsl:if test="contains($text,'&#xA;')">
				<xsl:value-of select="substring-after($text,'&#xA;')" disable-output-escaping="yes"/>
			</xsl:if>
		</xsl:variable>

		<xsl:if test="$mode!='ul' and starts-with($text,'- ')">
			<xsl:value-of disable-output-escaping="yes" select="'&lt;ul class=&quot;markdown&quot; &gt;'"/>
		</xsl:if>
		<xsl:if test="$mode='ul' and not(starts-with($text,'- '))">
			<xsl:value-of disable-output-escaping="yes" select="'&lt;/ul&gt;'"/>
		</xsl:if>

		<xsl:if test="$mode!='ol' and starts-with($text,'= ')">
			<xsl:value-of disable-output-escaping="yes" select="'&lt;ol&gt;'"/>
		</xsl:if>
		<xsl:if test="$mode='ol' and not(starts-with($text,'= '))">
			<xsl:value-of disable-output-escaping="yes" select="'&lt;/ol&gt;'"/>
		</xsl:if>

		<xsl:variable name="nextmode">
			<xsl:choose>
				<xsl:when test="starts-with($text,'- ')">
					<xsl:text>ul</xsl:text>
				</xsl:when>
				<xsl:when test="starts-with($text,'= ')">
					<xsl:text>ol</xsl:text>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>

		<xsl:choose>


			<xsl:when test="starts-with($text,'##')">
				<h4>
					<xsl:call-template name="MarkdownBlock">
						<xsl:with-param name="block" select="substring($line,3)"/>
						<xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
					</xsl:call-template>
				</h4>
			</xsl:when>

			<xsl:when test="starts-with($text,'#')">
				<h3>
					<xsl:call-template name="MarkdownBlock">
						<xsl:with-param name="block" select="substring($line,2)"/>
						<xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
					</xsl:call-template>
				</h3>
			</xsl:when>

			<xsl:when test="starts-with($text,'- ') and $CurrentLoop &lt; $MaxLoops">
				<li>
					<xsl:call-template name="MarkdownBlock">
						<xsl:with-param name="block" select="substring($line, 3)"/>
						<xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
					</xsl:call-template>
				</li>
			</xsl:when>

			<xsl:when test="starts-with($text,'= ')">
				<li>
					<xsl:call-template name="MarkdownBlock">
						<xsl:with-param name="block" select="substring($line, 3)"/>
						<xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
					</xsl:call-template>
				</li>
			</xsl:when>
			<!-- 		
		<xsl:when test="contains($text, '&#xA;')">
			<xsl:value-of select="substring-before($text, '&#xA;')"/>			
			
			
			<xsl:call-template name="MultiLine">
				<xsl:with-param name="text" select="substring-after($text,'&#xA;')"/>
			</xsl:call-template>
		</xsl:when>
		-->

			<xsl:otherwise>
				<xsl:if test="$line!='' and $CurrentLoop &lt; $MaxLoops">
					<p>
						<xsl:call-template name="MarkdownBlock">
							<xsl:with-param name="block" select="$line"/>
							<xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
						</xsl:call-template>
					</p>
				</xsl:if>
			</xsl:otherwise>

		</xsl:choose>


		<xsl:if test="$remainder!=''">
		    <xsl:call-template name="Markdown">
		        <xsl:with-param name="text" select="$remainder"/>
		        <xsl:with-param name="mode" select="$nextmode"/>
		        <xsl:with-param name="SuppressLinks" select="$SuppressLinks"/>
		        <xsl:with-param name="CurrentLoop" select="$CurrentLoop + 1"/>
		        <xsl:with-param name="MaxLoops" select="$MaxLoops"/>
		    </xsl:call-template>
		</xsl:if>

	</xsl:template>



	<xsl:template name="MarkdownBlock">
		<xsl:param name="block"/>
		<xsl:param name="SuppressLinks"/>

		<xsl:choose>

			<xsl:when test="contains($block,'**') and contains(substring-after($block,'**'),'**')">
				<xsl:variable name="beforefirststar" select="substring-before($block,'**')"/>
				<xsl:variable name="afterfirststar" select="substring-after($block,'**')"/>
				<xsl:variable name="emtext" select="substring-before($afterfirststar,'**')"/>
				<xsl:variable name="aftersecondstar" select="substring-after($afterfirststar,'**')"/>

				<xsl:value-of select="$beforefirststar" disable-output-escaping="yes"/>

				<strong>
					<xsl:value-of select="$emtext"/>
				</strong>
				<xsl:call-template name="MarkdownBlock">
					<xsl:with-param name="block" select="$aftersecondstar"/>
				</xsl:call-template>
			</xsl:when>

			<xsl:when test="contains($block,'*') and contains(substring-after($block,'*'),'*')">
				<xsl:variable name="beforefirststar" select="substring-before($block,'*')"/>
				<xsl:variable name="afterfirststar" select="substring-after($block,'*')"/>
				<xsl:variable name="emtext" select="substring-before($afterfirststar,'*')"/>
				<xsl:variable name="aftersecondstar" select="substring-after($afterfirststar,'*')"/>

				<xsl:value-of select="$beforefirststar" disable-output-escaping="yes"/>

				<em>
					<xsl:value-of select="$emtext"/>
				</em>
				<xsl:call-template name="MarkdownBlock">
					<xsl:with-param name="block" select="$aftersecondstar"/>
				</xsl:call-template>
			</xsl:when>

			<xsl:when test="contains($block,'[') and contains($block,'](') and contains($block,')')">

				<xsl:variable name="beforeopensquarebracket" select="substring-before($block,'[')"/>
				<xsl:variable name="insquarebracket" select="substring-before(substring-after($block,'['),']')"/>
				<xsl:variable name="inroundbracket" select="substring-before(substring-after($block,']('),')')"/>

				<xsl:value-of select="$beforeopensquarebracket" disable-output-escaping="yes"/>

                <xsl:variable name="href">
                    <xsl:choose>
                        <xsl:when test="starts-with($inroundbracket,'^')">
                            <xsl:value-of select="substring-after($inroundbracket,'^')"/>
                        </xsl:when>
                        <xsl:otherwise><xsl:value-of select="$inroundbracket"/></xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>

				<xsl:choose>
					<xsl:when test="$SuppressLinks = 'false'">
						<a href="{$href}">
							<xsl:if test="starts-with($inroundbracket,'^')"><xsl:attribute name="target">_blank</xsl:attribute></xsl:if>
							<xsl:value-of select="$insquarebracket"/>
						</a>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$insquarebracket"/>
					</xsl:otherwise>
				</xsl:choose>
				

				<xsl:call-template name="MarkdownBlock">
					<xsl:with-param name="block" select="substring-after($block,')')"/>
				</xsl:call-template>

			</xsl:when>

			<xsl:otherwise>
				<xsl:value-of select="$block" disable-output-escaping="yes"/>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

</xsl:stylesheet>



<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>


	<xsl:include href="../../xsl/functions.xsl"/>
	<xsl:include href="../../xsl/markdown.xsl"/>

	<xsl:param name="IncludeContactInformation" />
	<xsl:param name="IncludeMainImage" />
	<xsl:param name="IncludeBreadcrumbs" />
	<xsl:param name="ShowHeading"/>
	<xsl:param name="WarningText"/>
	<xsl:param name="CaptchaWarningText"/>
	<xsl:param name="SuccessText"/>
	
	<xsl:template match="/">

		<xsl:if test="$IncludeBreadcrumbs = 'true'">
			<div class="breadcrumbs contactus">
				<ul>
					<li class="previous last">
						<a ml="Breadcrumbs" href="/">Home</a>
						<span class="divider">
							<xsl:text> </xsl:text>
						</span>
					</li>
					<li class="final current">
						<a ml="Breadcrumbs" href="javascript:void(0)">Contact Us</a>
						<span class="divider">
							<xsl:text> </xsl:text>
						</span>
					</li>
				</ul>
			</div>
		</xsl:if>
		
		<xsl:choose>
			<xsl:when test="$ShowHeading = 'true'">
				<div id="divContactUsHeading">
					<h1>
						<xsl:value-of select="/ContactUs/Title" />
					</h1>
					<xsl:if test ="$IncludeMainImage = 'true'">
						<div id="divContactUsImgContainer">
							<img class="mainImage" src="{/ContactUs/MainImage}" alt=""/>
						</div>
					</xsl:if>
					<xsl:value-of select="/ContactUs/Text" disable-output-escaping="yes" />
				</div>				
			</xsl:when>
			<xsl:when test="$IncludeMainImage = 'true'">
				<div id="divContactUsImgContainer">
					<img class="mainImage" src="{/ContactUs/MainImage}" alt=""/>
				</div>
			</xsl:when>		
		</xsl:choose>

		
		<div id="divContactUs" class="clear">
			
			<xsl:if test="/ContactUs/Text != ''">
				<div id="divContactUsContent">
					<h2><xsl:value-of select="/ContactUs/Title"/></h2>			
					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="/ContactUs/Text" />
					</xsl:call-template>	
				</div>			
			</xsl:if>
			
			<xsl:if test="$IncludeContactInformation = 'true'">
				<div id="divContactInformation">

					<xsl:variable name ="EmailTo">
						<xsl:text>EmailTo:</xsl:text>
						<xsl:value-of select ="/ContactUs/EmailAddress"/>
					</xsl:variable>

					<p>
						<xsl:text>Tel:</xsl:text>
						<xsl:value-of select ="/ContactUs/TelephoneNumber"/>
					</p>

					<p class="topMargin">
						<xsl:text>Email:</xsl:text>
						<a href="{$EmailTo}">
							<xsl:value-of select ="/ContactUs/EmailAddress"/>
						</a>
					</p>

					<p  class="topMargin">Address:</p>

					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="/ContactUs/Address"/>
					</xsl:call-template>

					<p  class="topMargin">Opening Times:</p>
					<xsl:call-template name="Markdown">
						<xsl:with-param name="text" select="/ContactUs/OpeningTimes"/>
					</xsl:call-template>

				</div>
			</xsl:if>
				
		

			<div id="divContactUsForm" class="box primary">

				<div class="boxTitle">
					<h2 ml="Static Page">Contact us Form</h2>
				</div>

				<div id="divInputs" class="form">

					<div class="field name">
						<label id="lblName" for="txtName" ml="Static Page" class="textboxlabel">
							<xsl:choose>
								<xsl:when test="//LabelOverrides/lblName != ''">
									<xsl:value-of select="//LabelOverrides/lblName"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Your name</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</label>
						<input id="txtName" name="txtName" type="text" class="textbox"/>
					</div>

					<div class="field email">
						<label id="lblEmail" for="txtEmail" ml="Static Page" class="textboxlabel">
							<xsl:choose>
								<xsl:when test="//LabelOverrides/lblEmail != ''">
									<xsl:value-of select="//LabelOverrides/lblEmail"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Your email address</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</label>
						<input id="txtEmail" name="txtEmail" type="text" class="textbox"/>
					</div>

					<div class="field phone">
						<label id="lblPhoneNumber" for="txtPhoneNumber" ml="Static Page" class="textboxlabel">
							<xsl:choose>
								<xsl:when test="//LabelOverrides/lblPhoneNumber != ''">
									<xsl:value-of select="//LabelOverrides/lblPhoneNumber"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Your phone number</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</label>
						<input id="txtPhoneNumber" name="txtPhoneNumber" type="text" class="textbox"/>
					</div>

					<div class="field message">
						<label id="lblMessage" for="txtMessage" ml="Static Page">
							<xsl:choose>
								<xsl:when test="//LabelOverrides/lblMessage != ''">
									<xsl:value-of select="//LabelOverrides/lblMessage"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>Message</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</label>
						<textarea id="txtMessage" name="txtMessage">
							<xsl:text> </xsl:text>
						</textarea>
						</div>

					<div class="clearing">
						<xsl:text> </xsl:text>
					</div>

					<p id="pThankYou" style="display:none;" ml="Static Page">Thank you for your message, we will be in touch.</p>

					<a href="javascript:ContactUs.Submit();" id="aSubmit" class="button primary" ml="Static Page">
						<xsl:choose>
							<xsl:when test="//LabelOverrides/aSubmit != ''">
								<xsl:value-of select="//LabelOverrides/aSubmit"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Send</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</a>

					<xsl:for-each select="//PlaceholderOverrides">
						<xsl:if test="txtName != ''">
							<input type="hidden" id="hidContact_NamePlaceholder" value="{txtName}" />
						</xsl:if>
						<xsl:if test="txtEmail != ''">
							<input type="hidden" id="hidContact_EmailPlaceholder" value="{txtEmail}" />
						</xsl:if>
						<xsl:if test="txtPhoneNumber != ''">
							<input type="hidden" id="hidContact_PhonePlaceholder" value="{txtPhoneNumber}" />
						</xsl:if>
						<xsl:if test="txtMessage != ''">
							<input type="hidden" id="hidContact_MessagePlaceholder" value="{txtMessage}" />
						</xsl:if>
					</xsl:for-each>
				
				</div>			

			</div>
			
			<!--Warning message text-->
			<input type="hidden" id="hidWarningText" ml="Contact Us Page">
				<xsl:attribute name="value">
					<xsl:choose>
						<xsl:when test="$WarningText != ''">
							<xsl:value-of select="$WarningText"/>
						</xsl:when>
						<xsl:otherwise>
								<xsl:text>Please ensure that all required fields have been entered</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</input>
			
		</div>
		
		
		
		<!--Setup javascript-->
		<script type="text/javascript">
			<xsl:text>ContactUs.Setup();</xsl:text>
		</script>
		
	</xsl:template>

</xsl:stylesheet>

<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

	<xsl:param name="LoginType" />
	<xsl:param name="LoginMethod" />
	<xsl:param name="RedirectURL" />
	<xsl:param name="Email" />
	<xsl:param name="HideHeader" />
	<xsl:param name="Placeholders" />
	<xsl:param name="DrawRememberMe" />
	<xsl:param name="UseLoginType" />

	<xsl:template match="/">

		<div id="divTradeLogin" class="box clear">

			<div class="boxTitle">
				<xsl:if test="$HideHeader = 'True'">
					<xsl:attribute name="style">display:none;</xsl:attribute>
				</xsl:if>
				<h2 ml="Login">Login</h2>
			</div>

			<div class="loginDetails">

				<!-- Display fields based on Login Type-->
				<!-- Trade -->
				<xsl:if test="TradeLoginDetails/LoginType = 'Trade' or $UseLoginType = 'false'">
					<div class="loginType" id="divAgentLogin">

						<xsl:choose>
							<xsl:when test="$LoginMethod = 'TradeDetails'">
								<input type="text" id="txtAgentReference" class="textbox" onkeydown="TradeLogin.OnKeyPress(event);">
									<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
										<xsl:attribute name="value">
											<xsl:value-of select="TradeLoginDetails/AgentReference"/>
										</xsl:attribute>
									</xsl:if>
								</input>
							</xsl:when>

							<xsl:when test="$LoginMethod = 'EmailAndPassword'">
								<input type="text" id="txtEmailAddress" class="textbox" onkeydown="TradeLogin.OnKeyPress(event);">
									<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
										<xsl:attribute name="value">
											<xsl:value-of select="TradeLoginDetails/EmailAddress"/>
										</xsl:attribute>
									</xsl:if>
								</input>
							</xsl:when>
						</xsl:choose>

						<input type="password" id="txtWebsitePassword" class="textbox" onkeydown="TradeLogin.OnKeyPress(event);" >
							<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
								<xsl:attribute name="value">
									<xsl:value-of select="TradeLoginDetails/WebsitePassword"/>
								</xsl:attribute>
							</xsl:if>
						</input>
					</div>
				</xsl:if>

				<!-- Trade Contact -->
				<xsl:if test="TradeLoginDetails/LoginType = 'TradeContact' or $UseLoginType = 'false'">
					<div class="loginType" id="divTradeContactLogin" style="display:none;">

						<input type="text" id="txtAgencyUserName" class="textbox" onfocus="int.f.SetValue(this, '')" >
							<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
								<xsl:attribute name="value">
									<xsl:value-of select="TradeLoginDetails/AgentReference"/>
								</xsl:attribute>
							</xsl:if>
						</input>

						<input type="text" id="txtUsername" class="textbox" onfocus="int.f.SetValue(this, '')" >
							<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
								<xsl:attribute name="value">
									<xsl:value-of select="TradeLoginDetails/Username"/>
								</xsl:attribute>
							</xsl:if>
						</input>

						<input type="password" id="txtPassword" class="textbox" onfocus="int.f.SetValue(this, '');" onkeydown="TradeLogin.OnKeyPress(event);">
							<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
								<xsl:attribute name="value">
									<xsl:value-of select="TradeLoginDetails/Password"/>
								</xsl:attribute>
							</xsl:if>
						</input>
					</div>
				</xsl:if>

				<!-- Remember Me -->
				<xsl:if test ="$DrawRememberMe = 'True'">
					<label id="chkTradeLogin_RememberMe" class="checkboxLabel">
						<xsl:text>Remember me</xsl:text>
						<input type="checkbox" id="chkRememberMe" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');">
							<xsl:if test="TradeLoginDetails/RememberMe = 'true'">
								<xsl:attribute name="checked">checked</xsl:attribute>
							</xsl:if>
						</input>
					</label>
				</xsl:if>

				<!-- Change Login Type -->
				<div id="divLoginOptions">
					<xsl:if test="count(TradeLoginDetails/LoginTypes/LoginType) &lt;= 1">
						<xsl:attribute name="style">display:none;</xsl:attribute>
					</xsl:if>
					<a id="pLoginAsTradeMember" href="javascript:TradeLogin.ToggleLoginMode();int.f.SetValue('hidLoginType','Trade');" ml="Login">
						<!-- really - eugh-->
						<xsl:if test="TradeLoginDetails/LoginType = 'Trade'">
							<xsl:attribute name="style">display:none</xsl:attribute>
						</xsl:if>
						<xsl:text>Login as Agency</xsl:text>
					</a>
					<a id="pLoginAsTradeContact" href="javascript:TradeLogin.ToggleLoginMode();int.f.SetValue('hidLoginType','TradeContact');" ml="Login">
						<xsl:if test="TradeLoginDetails/LoginType = 'TradeContact'">
							<xsl:attribute name="style">display:none</xsl:attribute>
						</xsl:if>
						<xsl:text>Staff Member Login</xsl:text>
					</a>
				</div>

				<a id="aLogin" class="button primary" href="javascript:TradeLogin.Validate()" ml="Login">Login</a>

				<!--<p><xsl:value-of select="$Placeholders"/></p>
				<p ml="Login"><xsl:value-of select="$Placeholders"/></p>-->

			</div>

			<input type="hidden" id="hidLoginType" name="hidLoginType" value="{TradeLoginDetails/LoginType}" />
			<input type="hidden" id="hidLoginMethod" name="hidLoginMethod" value="{$LoginMethod}" />

			<input type="hidden" id="hidLoginRedirectURL" name="hidLoginRedirectURL" value="{$RedirectURL}" />
			<input type="hidden" id="hidIncorrectFields" name="hidIncorrectFields" value="Please make sure all fields are entered correctly. Incorrect fields have been highlighted." ml="Login" />
			<input type="hidden" id="hidLoginFailed" name="hidLoginFailed" value="Sorry, your login details were incorrect. Please check and try again." ml="Login" />
			<input type="hidden" id="hidTradeLogin_Placeholders" name="hidTradeLogin_Placeholders" value="{$Placeholders}" ml="Login" />


			<script type="text/javascript">
				int.ll.OnLoad.Run(function () { TradeLogin.Setup(); });
			</script>

		</div>

	</xsl:template>
</xsl:stylesheet>

<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
	
<xsl:include href="../../xsl/functions.xsl"/>
	
<xsl:param name="Theme"/>
<xsl:param name="Title" />
<xsl:param name="CurrencySymbol" />
<xsl:param name="CurrencySymbolPosition" />
<xsl:param name="SearchMode" />
<xsl:param name="ShowFlightCodeFields" />
<xsl:param name="RequiresPhoneNumber" />
<xsl:param name="HotelOnlyInitialSearch" select="'False'"/>
<xsl:param name="EnablePropertySearch" select="'False'"/>
<xsl:param name="OneWay" />
<xsl:param name="CSSClassOverride" />

	<!-- the below arent being used in this template yet... -->
	<xsl:param name="CurrentOutboundTime"/>
	<xsl:param name="CurrentReturnTime"/>
	<xsl:param name="CurrentOutboundFlightCode"/>
	<xsl:param name="CurrentReturnFlightCode"/>
	<xsl:param name="CurrentDepartureParentID"/>
	<xsl:param name="CurrentDepartureParentType"/>



	<xsl:param name="Template" />

<xsl:template match="/">
	
	
	<xsl:choose>
		<xsl:when test="$Template = 'Rates'">
			<xsl:call-template name="RatesTable" />			
		</xsl:when>
		<xsl:otherwise>
			
			<div id="divTransfers" class="box primary clear">

				<xsl:attribute name ="class">
					<xsl:choose>
						<xsl:when test="$CSSClassOverride != ''">
							<xsl:value-of select="$CSSClassOverride" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>box primary clear</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>


				<div class="boxTitle">
					<h2 ml="Transfers">
						<xsl:choose>
							<xsl:when test="$Title != ''">
								<xsl:value-of select="$Title"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Transfers</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</h2>
					<xsl:if test="Results/TextOverrides/SubTitle != ''">
						<p ml="Transfers">
							<xsl:value-of select="Results/TextOverrides/SubTitle"/>
						</p>
					</xsl:if>
				</div>
				
				
				<div id="divTransferResults">
					<xsl:if test="count(Results/Transfers/Transfer) = 0">
						<xsl:attribute name="style">display:none;</xsl:attribute>
					</xsl:if>
					
					<p id="pTransfers_Text">
						<xsl:value-of select="Results/TextOverrides/Text_TransfersTable"/>
					</p>
					
					<div id="divTransferOptions">
						<xsl:call-template name="RatesTable" />		
						<xsl:text> </xsl:text>
					</div>					
					
				</div>
				

				
				<xsl:if test="$SearchMode = 'FlightPlusHotel' and count(Results/Transfers/Transfer) = 0">
					<p ml="Transfers">Sorry there are no transfers available for your holiday combination.</p>
				</xsl:if>
				
				<xsl:if test="$SearchMode = 'FlightOnly' and count(Results/Transfers/Transfer) = 0 and $EnablePropertySearch = 'True'">
					<xsl:call-template name="PropertyTransferSearch" />
				</xsl:if>
				
				<!-- Show flight details form if search mode is hotel only -->
				<xsl:if test="$SearchMode = 'HotelOnly' and (count(Results/Transfers/Transfer) = 0 or $HotelOnlyInitialSearch = 'True')">
					<xsl:call-template name="FlightDetails" />	
				</xsl:if>
				
				<xsl:call-template name="WaitMessage" />
				
			</div>
			
		</xsl:otherwise>
	</xsl:choose>

</xsl:template>
	
<xsl:template name="RatesTable">
	
	<xsl:if test="$SearchMode = 'HotelOnly' and count(Results/Transfers/Transfer) = 0">
		<p ml="Transfers">Sorry there are no transfers available for your holiday combination.</p>
	</xsl:if>
	
	<xsl:if test="count(Results/Transfers/Transfer) &gt; 0">
		<table class="def">
			<tr>
				<th ml="Transfers">Description</th>
				<th ml="Transfers">Price</th>
				<th ml="Transfers">Select</th>
			</tr>
						
			<tr id="tr_Transfers_0" class="transferOption selected">
				<td ml="Transfers">None</td>
				<td>
					<strong>
						<xsl:call-template name="GetSellingPrice">
							<xsl:with-param name="Value" select="'0'" />
							<xsl:with-param name="Format" select="'#####0'" />
							<xsl:with-param name="Currency" select="$CurrencySymbol" />
							<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />							
						</xsl:call-template>	
					</strong>
				</td>
				<td>
					<label class="radioLabel selected" onclick="web.CustomInputs.ToggleRadioLabel(this, 'rad_Transfers');">
						<input type="radio" class="radio" id="rad_Transfers_0" name="rad_Transfers" checked="checked" onclick="Transfers.RemoveTransfer();" ml="Transfers" />
					</label>
				</td>
					
			</tr>
			<xsl:for-each select="Results/Transfers/Transfer">
					
				<tr id="tr_Transfers_{position()}" class="transferOption">
					<td><xsl:value-of select="Vehicle"/></td>
					<td>
						<strong>
							<xsl:call-template name="GetSellingPrice">
								<xsl:with-param name="Value" select="Price" />
								<xsl:with-param name="Format" select="'######'" />
								<xsl:with-param name="Currency" select="$CurrencySymbol" />
								<xsl:with-param name="CurrencySymbolPosition" select="$CurrencySymbolPosition" />							
							</xsl:call-template>						
						</strong>
					</td>
					<td>
						<label class="radioLabel" onclick="web.CustomInputs.ToggleRadioLabel(this, 'rad_Transfers');">
							<input type="radio" class="radio" id="rad_Transfers_{position()}" name="rad_Transfers" onclick="Transfers.AddTransfer('{TransferOptionHashToken}', {position()});" ml="Transfers" />
						</label>
					</td>
				</tr>
							
			</xsl:for-each>	
		</table>

		<xsl:if test ="$RequiresPhoneNumber = 'True'">
			<div class="PhoneNumber">
				<input type="text" id="txtTransfer_GuestPhoneNumber" class="textbox" placeholder="Enter Mobile Number"/>
				<p>Please enter the mobile number of one of the party traveling - we will pass this number on to the transfer company should the need arise for them to contact you.</p>
			</div>
		</xsl:if>
		
		<xsl:if test ="$ShowFlightCodeFields = 'True'">
			
			<input type="hidden" id="hidTransfer_AddOutboundFlightNumberPlaceholder" value="Outbound Flight Number" ml="Transfers" />
			<input type="hidden" id="hidTransfer_AddReturnFlightNumberPlaceholder" value="Return Flight Number" ml="Transfers" />
			<input type="hidden" id="hidWarning_Invalid" value="Please ensure that all required fields are set. Incorrect fields have been highlighted." ml="Transfers" runat="server" />
			
			<div id="divTransferAddFlightCodes">
				<xsl:choose>
					<xsl:when test="$OneWay = 'true'">
						<div class="flightCode">
							<h4 ml="Transfers">Flight Code</h4>
							<input type="text" id="txtTransfer_AddOneWayFlightNumber" class="textbox" />
						</div>						
					</xsl:when>
					<xsl:otherwise>
						<div class="flightCode">
							<h4 ml="Transfers">Outbound Flight</h4>
							<input type="text" id="txtTransfer_AddOutboundFlightNumber" class="textbox" />
						</div>
						<div class="flightCode">
							<h4 ml="Transfers">Return Flight</h4>
							<input type="text" id="txtTransfer_AddReturnFlightNumber" class="textbox" />
						</div>						
					</xsl:otherwise>
				</xsl:choose>							
			</div>

			<script type="text/javascript">
				int.ll.OnLoad.Run(function () { Transfers.AttachPlaceHolders(); });
			</script>
		</xsl:if>
		
		<input type="hidden" id="hidShowFlightCodeFields" value="{$ShowFlightCodeFields}" />
	</xsl:if>
	
	<input type="hidden" id="hidTransferAddSuccessMessage" value="Your transfer has been added successfully" ml="Transfers" />
	
	<xsl:if test="$SearchMode = 'FlightOnly' and $EnablePropertySearch = 'True'">
		<xsl:if test="count(Results/Transfers/Transfer) = 0">
			<p ml="Transfers">Sorry, we could not find any transfers for your search. Please try again.</p>
		</xsl:if>
		<input id="btnAmendPropertyTransferSearch" type="button" class="button primary" value="Amend Search" onclick="int.f.Hide('divTransferResults');int.f.Show('divPropertyTransferSearch');" ml="Transfers" />
	</xsl:if>
	
	<!--<xsl:if test="$SearchMode = 'HotelOnly'" >
		<input type="button" class="button primary" value="Search Again" onclick="int.f.Hide('divTransferResults');int.f.Show('divTransfers_HotelOnlyFlightDetails');int.f.Hide('this');return false;" />
	</xsl:if>-->
	
</xsl:template>
	
<xsl:template name="FlightDetails">
	
	<div id="divTransfers_HotelOnlyFlightDetails">
		<p id="pTransfers_Text">
			<xsl:value-of select="Results/TextOverrides/Text_FlightDetails"/>
		</p>
		
		<xsl:choose>
			<xsl:when test="count(Results/HotelAirports/Airports/Airport) = 1">
				<p>
					<xsl:text>Your nearest airport is </xsl:text>
					<strong>
						<xsl:value-of select="Results/HotelAirports/Airports/Airport/Airport"/>
					</strong>
					<xsl:text>.</xsl:text>
				</p>
			</xsl:when>
			<xsl:otherwise>
				<fieldset>
					<label>Nearby airports</label>
					<select onchange="int.f.SetValue('hidAirportID', this.value);">
						<xsl:for-each select="Results/HotelAirports/Airports/Airport">
							<option value="{AirportID}">
								<xsl:value-of select="Airport"/>
							</option>
						</xsl:for-each>					
					</select>
				</fieldset>
			</xsl:otherwise>
		</xsl:choose>
	
		<table class="def">
			<tr>
				<th colspan="2" ml="Transfers">Outbound</th>
				<th colspan="2" ml="Transfers">Return</th>
			</tr>
			<tr>
				<td ml="Transfers">Flight Number</td>
				<td><input type="text" id="txtTransfer_OutboundFlightNumber" class="textbox" /></td>
				<td ml="Transfers">Flight Number</td>
				<td><input type="text" id="txtTransfer_ReturnFlightNumber" class="textbox" /></td>
			</tr>
			<tr>
				<td ml="Transfers">Time</td>
				<td><input type="text" id="txtTransfer_OutboundFlightTime" class="textbox" onblur="ParseTime(this);" /></td>
				<td ml="Transfers">Time</td>
				<td><input type="text" id="txtTransfer_ReturnFlightTime" class="textbox" onblur="ParseTime(this);" /></td>
			</tr>
		</table>
	
		<input type="button" class="button primary" value="Search for Transfers" onclick="Transfers.SearchTransfers();" />
		
		<input type="hidden" id="hidAirportID" value="{Results/HotelAirports/Airports/Airport[1]/AirportID}" />
		
		
		<script type="text/javascript">
			int.ll.OnLoad.Run(function () { Transfers.AttachPlaceHolders(); });
		</script>
		
	</div>
	
	<xsl:call-template name="WaitMessage" />
	
</xsl:template>

<xsl:template name="WaitMessage">
	<!-- Wait Message -->
	<div id="divTransferSearchWait" style="display:none;">
		<h3>Searching...</h3>
		<img src="\Themes\{$Theme}\Images\loader.gif" alt="" />
	</div>
</xsl:template>
	
<xsl:template name="PropertyTransferSearch">
	<div id="divPropertyTransferSearch">
		<p ml="Transfers">* Return transfer from your airport to your hotel.</p>
		<table class="def">
			<tr>
				<th ml="Transfers">Outbound</th>
				<th ml="Transfers">Return</th>
			</tr>
			<tr>
				<td ml="Transfers">
					<label ml="Transfers">Drop off Hotel</label>
					<div id="divTransferPropertyIDAuto" class="autocomplete">
						<input id="acpTransferPropertyID" name="acpTransferPropertyID" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'Transfers.GetProperties(&quot;acpTransferPropertyID&quot;)')"
							onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" tabindex="0"/>
						<div id="acpTransferPropertyIDOptions" class="autocompleteoptions" style="display:none;"><xsl:text> </xsl:text></div>
						<input type="hidden" id="acpTransferPropertyIDHidden" value="0" />
						<input type="hidden" id="hidPropertyTransferACPPlaceholder" value="Search for Hotel" ml="Transfers" />
					</div>
				</td>
				<td>
					<label ml="Transfers">Pickup Time</label>
					<input type="text" id="txtTransfer_ReturnPickupTime" class="textbox" onblur="ParseTime(this);" />
				</td>
			</tr>
		</table>
				
		<input id="btnPropertyTransferSearch" type="button" class="button primary" value="Search for Transfers" onclick="Transfers.PropertyTransferSearch();" ml="Transfers" />
		
		<script type="text/javascript">
			int.ll.OnLoad.Run(function () { Transfers.AttachPlaceHolders(); });
		</script>		
	</div>
</xsl:template>
	
</xsl:stylesheet>

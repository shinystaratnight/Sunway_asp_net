<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SearchTool.ascx.vb" Inherits="iVectorWidgets.SearchToolUserControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>


<div id="divSearch" class="sidebarBox primary clear" runat="server" enableviewstate="false" >
	<int:hidden id="hidInsertWarningAfter" runat="server" />
	<int:hidden id="hidDefaultSearchValues" runat="server" />
    <int:Hidden ID="hidDatepickerCultureCode" runat="server" />

	<div id="divSearchTitle" class="boxTitle" runat="server">
		<h2 id="h2_SearchTitle" ml="Search Tool" runat="server">Search</h2>
	</div>


	<!-- search mode -->
	<div id="divSearchMode" class="tabs radio" runat="server">
		<ul id="ulSearchMode" runat="server"></ul>
		<int:hidden id="hidSearchMode" runat="server" />
		<int:hidden id="hidFlightPlusHotelURL" runat="server" />
		<int:hidden id="hidUseFlightCarouselResults" runat="server" />
	</div>

	<!-- where from and to -->
	<div id="divSearch_Where" class="form grouped">
		<h3 ml="Search Tool">Where?</h3>
		<fieldset id="fldDeparting">
			<dl class="clear">
				<dt><label ml="Search Tool" id="lblDepartureLocation" runat="server">Departing from</label></dt>
				<dd>

					<div id="divDepartingFromCountryCountry" runat="server">
						<web:dropdown id="ddlDepartingFromCountryID" script="SearchTool.Support.SelectCountry('Departure');" runat="server" lookup="Country"/>
					</div>	

					<div id="divDepartingFromDropdown" runat="server">
						<i></i>
						<web:dropdown id="ddlDepartingFromID" lookup="AirportGroupAndAirport" script="int.f.SetValue('hidAirportDepartingFromID', int.dd.GetIntValue(this));SearchTool.Support.SetDepartingFromID();" runat="server" />
					</div>

					<div id="divDepartingFromAuto" class="autocomplete" runat="server">
						<input id="acpDepartingFromID" name="acpDepartingFromID" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetDepartureAirports(&quot;acpDepartingFromID&quot;)')"
							onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Search Airport" runat="server" />
						<div id="acpDepartingFromIDOptions" class="autocompleteoptions" style="display:none;"></div>
						<input type="hidden" id="acpDepartingFromIDHidden" value="0" runat="server" />
						<input type="hidden" id="acpDepartingFromIDScript" value="SearchTool.Support.SetDepartingFromID()" runat="server" />
					</div>

					<div id="divShowHideDepartingFromAutoComplete" runat="server">					
						<a id="aShowDepartingFromDropdown" href="#" onclick="SearchTool.Support.ShowHideDepartingFromAutoComplete();return false;" ml="Search Tool" runat="server">or select airport from a list</a>
						<a id="aShowDepartingFromAutoComplete" href="#" onclick="SearchTool.Support.ShowHideDepartingFromAutoComplete();return false;" ml="Search Tool" runat="server" style="display:none">or type in using auto-suggest</a>
					</div>

					<input type="hidden" id="hidAirportDepartingFromID" value="0" runat="server" />
				</dd>
			</dl>
		</fieldset>

		<fieldset id="fldTo">
			<dl class="clear">
				<dt><label ml="Search Tool" id="lblDestination" runat="server">To</label></dt>
				<dd>

					<div id="divArrivingAtDestination" runat="server">
						
						<div id="divArrivingAtCountryCountry" style= "display:none;" runat="server">
							<web:dropdown id="ddlCountryID" script="SearchTool.Support.SelectCountry('Arrival');" value="Country" runat="server" lookup="Country"/>
						</div>	
										
						<div id="divArrivingAtDropdown" runat="server">
							<i></i>		

							<web:dropdown id="ddlRegionID" script="int.f.SetValue('hidArrivingAtID', int.dd.GetIntValue(this));SearchTool.Support.ArrivingAtDropdowns();" runat="server" lookup="CountryRegion" includeparent="true"/>
						</div>

						<div id="divArrivingAtAuto" runat="server">
							<i></i>
							<input id="acpArrivingAtID" name="acpArrivingAtID" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetRegionResorts()')" 
								onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Please type your destination here" runat="server" />
							<div id="acpArrivingAtIDOptions" class="autocompleteoptions" style="display:none;"></div>
							<input type="hidden" id="acpArrivingAtIDHidden" value="0" runat="server" />
							<input type="hidden" id="acpArrivingAtIDScript" value="SearchTool.Support.SetArrivingAtID()" runat="server" />
						</div>

						<div id="divShowHideArrivingAtAutoComplete" runat="server">					
							<a id="aShowArrivingAtDropdown" href="#" onclick="SearchTool.Support.ShowHideArrivingAtAutoComplete();return false;" ml="Search Tool" runat="server">or select destination from a list</a>
							<a id="aShowArrivingAtAutoComplete" href="#" onclick="SearchTool.Support.ShowHideArrivingAtAutoComplete();return false;" ml="Search Tool" runat="server" style="display:none">or type in using auto-suggest</a>
						</div>
					</div>

					<div id="divArrivingAtAirportControls">						
						<div id="divArrivingAtAirport" runat="server" style="display:none">
							<web:dropdown id="ddlArrivalAirportID" script="int.f.SetValue('hidArrivingAtID', int.dd.GetIntValue(this) + 1000000);" runat="server" lookup="ArrivalAirport" />
						</div>

						<div id="divArrivingAtAirportAuto" style="display:none" runat="server">
							<i></i>
							<input id="acpArrivingAtAirportID" name="acpArrivingAtAirportID" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetAirportsByDeparture(&quot;acpArrivingAtAirportID&quot;)')" 
								onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Please type your destination here" runat="server" />
							<div id="acpArrivingAtAirportIDOptions" class="autocompleteoptions" style="display:none;"></div>
							<input type="hidden" id="acpArrivingAtAirportIDHidden" value="0" runat="server" />
							<input type="hidden" id="acpArrivingAtAirportIDScript" value="SearchTool.Support.SetArrivingAtID()" runat="server" />
						</div>
						
						<div id="divShowHideAirportArrivingAtAutoComplete" runat="server">				
							<a href="javascript:void;" id="aShowAutoComplete" onclick="SearchTool.Support.ToggleAirportAutoComplete();return false;" runat="server">or type in using auto-suggest</a>
							<a href="javascript:void;" id="aHideAutoComplete" onclick="SearchTool.Support.ToggleAirportAutoComplete();return false;" style="display:none" runat="server">or select destination from a list</a>
						</div>	
					</div>
					
					<input type="hidden" id="hidArrivingAtID" value="0" runat="server" />
				</dd>
			</dl>
		</fieldset>

		<fieldset id="fldPriorityProperty" runat="server">
			<dl>
				<dt><label ml="Search Tool" id="lblProperty" runat="server">Property</label></dt>
				<dd>
					<div id="divPropertyAuto" class="autocomplete">
						<input id="acpPriorityPropertyID" name="acpPropertyID" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetProperties(&quot;acpPriorityPropertyID&quot;)')"
							onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Property Name" runat="server" />
						<div id="acpPriorityPropertyIDOptions" class="autocompleteoptions" style="display:none;"></div>
						<input type="hidden" id="acpPriorityPropertyIDHidden" value="0" runat="server" />
					</div>
				</dd>			
			</dl>
		</fieldset>

	</div>


	<!-- when and for how long -->
	<div id="divSearch_When" class="form grouped">
		<h3 ml="Search Tool">When?</h3>	
		<fieldset id="fldWhen">
			<dl id="dlDeparting">
				<dt><label ml="Search Tool" id="lblDepartureDate" runat="server">Departing</label></dt>
				<dd>
					<div class="textbox icon calendar right embedded">
						<i class="departureDate"></i>
						<input id="txtDepartureDate" name="txtDepartureDate" type="text" class="textbox"/>
					</div>				
				</dd>
			</dl>

			<dl id="dlReturn" runat="server">
				<dt><label ml="Search Tool" id="lblReturnDate" runat="server">Returning</label></dt>
				<dd>
					<div class="textbox icon calendar right embedded">
						<i class="returnDate"></i>
						<input id="txtReturnDate" name="txtReturnDate" type="text" class="textbox"/>
					</div>				
				</dd>
			</dl>

			<dl id="dlNights">
				<dt><label ml="Search Tool" id="lblDuration" runat="server">Nights</label></dt>
				<dd>
					<web:dropdown id="ddlDuration" runat="server" />
					<input type="hidden" id="hidDurationSingularSuffix" runat="server" ml="Search Tool" />
					<input type="hidden" id="hidDurationPluralSuffix" runat="server" ml="Search Tool" />
				</dd>
			</dl>
		</fieldset>
		<input type="hidden" id="hidDatepickerMonths" runat="server" />
		<input type="hidden" id="hidDatepickerFirstDay" runat="server" />
		<input type="hidden" id="hidShowDatePickerDropDowns" runat="server" />
		<input type="hidden" id="hidReturnDatepicker" runat="server" />
		<input type="hidden" id="hidDaysAhead" runat="server" />
		<input type="hidden" id="hidReturnMinDaysAhead" runat="server" />
	</div>


	<!--rooms-->
	<div id="divSearch_Rooms" class="form grouped">
		<%--<h3 ml="Search Tool">Who?</h3>--%>	
		<fieldset id="fldRooms">
			<dl>
				<dt><label ml="Search Tool" id="lblRooms" runat="server">Rooms</label></dt>
				<dd>
					<web:dropdown id="ddlRooms" cssclass="range_1_3" runat="server" script="SearchTool.Show();" />
					<input type="hidden" id="hidRoomsSingularSuffix" runat="server" ml="Search Tool" />
					<input type="hidden" id="hidRoomsPluralSuffix" runat="server" ml="Search Tool" />
				</dd>
			</dl>
		</fieldset>
	</div>


	<!-- Transfer Inputs-->
	<div id="divSearch_Transfers" class="form grouped clear">

		<fieldset id="fldPickupLocation" class="clear">
			<dl class="clear">
				<dt><label ml="Search Tool" id="lblPickupLocation" runat="server">Pick-up Point</label></dt>
				<dd><web:dropdown id="ddlPickupType" options="Airport|Airport#Property|Property" script="SearchTool.Support.TransferDropDowns(this);" runat="server"  multilingualtag="Search Tool" overrideblanktext="Pickup Point" /></dd>
			</dl>

			<div id="divPropertyAutoTransferPickup" class="autocomplete">
				<input id="acpPropertyIDTransferPickup" name="acpyPropertyIDTransferPickup" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetProperties(&quot;acpPropertyIDTransferPickup&quot;)')"
					onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Pick-up Location" runat="server" />
				<div id="acpPropertyIDTransferPickupOptions" class="autocompleteoptions" style="display:none;"></div>
				<input type="hidden" id="acpPropertyIDTransferPickupHidden" value="0" runat="server" />
			</div>

			<div id="divAirportAutoTransferPickup" class="autocomplete">
				<input id="acpAirportIDTransferPickup" name="acpyAirportIDTransferPickup" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetDepartureAirports(&quot;acpAirportIDTransferPickup&quot;)')" onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off"
					placeholder="Pick-up Location" runat="server" />
				<div id="acpAirportIDTransferPickupOptions" class="autocompleteoptions" style="display:none;"></div>
				<input type="hidden" id="acpAirportIDTransferPickupHidden" value="0" runat="server" />
			</div>

			<div id="divAirportDropdownTransferPickup" runat="server" style="display:none">
				<i></i>
				<web:dropdown id="ddlAirportTransferPickup" lookup="AirportGroupAndAirport" script="int.f.SetValue('hidTransferPickupLocationID', int.dd.GetIntValue(this));SearchTool.Support.SetTransferDepartingFromID();" runat="server" />
			</div>

			<div id="divShowHideTransferDepartingFromAutoComplete" runat="server">					
				<a id="aShowTransferDepartingFromDropDown" href="#" onclick="SearchTool.Support.ShowHideTransferDepartingFromAutoComplete();return false;" ml="Search Tool" runat="server">or select airport from a list</a>
				<a id="aShowTransferDepartingFromAutoComplete" href="#" onclick="SearchTool.Support.ShowHideTransferDepartingFromAutoComplete();return false;" ml="Search Tool" runat="server" style="display:none">or type in using auto-suggest</a>
			</div>

			<input type="hidden" id="hidTransferPickupLocationID" value="0" runat="server" />

		</fieldset>

		<fieldset id="fldDropOffLocation">
			<dl class="clear">
				<dt><label ml="Search Tool" id="lblDropOffLocation" runat="server">Drop-off Point</label></dt>
				<dd><web:dropdown id="ddlDropOffType" options="Airport|Airport#Property|Property" script="SearchTool.Support.TransferDropDowns(this);" runat="server" multilingualtag="Search Tool" overrideblanktext="Drop Off Point" /></dd>
			</dl>
			<div id="divPropertyAutoTransferDropOff" class="autocomplete">
				<input id="acpPropertyIDTransferDropOff" name="acpPropertyIDTransferPickup" type="text" class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetProperties(&quot;acpPropertyIDTransferDropOff&quot;)')"
					onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off" placeholder="Drop-off Location" runat="server" />
				<div id="acpPropertyIDTransferDropOffOptions" class="autocompleteoptions" style="display:none;"></div>
				<input type="hidden" id="acpPropertyIDTransferDropOffHidden" value="0" runat="server" />
			</div>


			<div id="divAirportAutoTransferDropOff" class="autocomplete">
				<input id="acpAirportIDTransferDropOff" name="acpyAirportIDTransferDropOff" type="text"
					class="textbox autocomplete" onkeyup="web.AutoComplete.AutoSuggestKeyUp(event, this, 'SearchTool.Support.GetDepartureAirports(&quot;acpAirportIDTransferDropOff&quot;)')" onkeydown="web.AutoComplete.AutoSuggestKeyDown(event, this)" autocomplete="off"
					placeholder="Drop-off Location" runat="server" />
				<div id="acpAirportIDTransferDropOffOptions" class="autocompleteoptions" style="display:none;"></div>
				<input type="hidden" id="acpAirportIDTransferDropOffHidden" value="0" runat="server" />
			</div>

			<div id="divAirportDropdownTransferDropOff" runat="server" style="display:none">
				<i></i>
				<web:dropdown id="ddlAirportTransferDropoff" lookup="AirportGroupAndAirport" script="int.f.SetValue('hidTransferPickupLocationID', int.dd.GetIntValue(this));SearchTool.Support.SetTransferArrivingAtID();" runat="server" />
			</div>

			<div id="divShowHideTransferArrivingAtAutoComplete" runat="server">					
				<a id="aShowTransferArrivingAtDropDown" href="#" onclick="SearchTool.Support.ShowHideTransferArrivingAtAutoComplete();return false;" ml="Search Tool" runat="server">or select airport from a list</a>
				<a id="aShowTransferArrivingAtAutoComplete" href="#" onclick="SearchTool.Support.ShowHideTransferArrivingAtAutoComplete();return false;" ml="Search Tool" runat="server" style="display:none">or type in using auto-suggest</a>
			</div>

			<input type="hidden" id="hidTransferDropOffLocationID" value="0" runat="server" />
		</fieldset>

		<label id="lblReturnTransfer" class="checkboxLabel">
			<input type="checkbox" id="cbReturnTransfer" class="checkbox" onclick="SearchTool.Support.ReturnTransfer();int.f.ToggleClass(this.parentNode, 'selected');" />
			<trans ml="Search Tool">I would like a return transfer</trans>
		</label>
		<input type="hidden" id="hidReturnTransfer" name="hidReturnTransfer" value="false" runat="server" />

		<fieldset id="fldPickupDate">
			<dl>
				<dt><label ml="Search Tool" id="lblPickupDate" runat="server">Pick-up Date</label></dt>
				<dd>
					<div class="textbox icon calendar">
						<i class="pickupDate"></i>
						<input id="txtPickupDate" name="txtPickupDate" type="text" class="textbox"/>
					</div>
				</dd>
			</dl>
			<dl>
				<dt><label ml="Search Tool" id="lblArrivalTime" runat="server">Flight Arrival Time</label></dt>
				<dd><input id="txtArrivalTime" name="txtArrivalTime" type="text" class="textbox" onblur="ParseTime(this);" /></dd>
			</dl>
		</fieldset>

		<fieldset id="fldDropOffDate">
			<dl>
				<dt><label ml="Search Tool" id="lblDropOffDate" runat="server">Drop-off Date</label></dt>
				<dd>
					<div class="textbox icon calendar">
						<i class="dropOffDate"></i>
						<input id="txtDropOffDate" name="txtDropOffDate" type="text" class="textbox"/>
					</div>
				</dd>
			</dl>
			<dl>
				<dt><label ml="Search Tool" id="lblDropOffTime" runat="server">Drop-off Time</label></dt>
				<dd><input id="txtDropOffTime" name="txtDropOffTime" type="text" class="textbox" onblur="ParseTime(this);" /></dd>
			</dl>
		</fieldset>
	</div>



	<!--guests-->
	<div id="divSearch_Guests" class="form grouped clear">
		<h3 ml="Search Tool">Who?</h3>

		<label id="lblPassengers" style="display:none;" ml="Search Tool" runat="server">Passengers</label>

		<table id="tblOccupancy" cellspacing="0" cellpadding="0">
			<tr id="trOccupancyHeaders">
				<th class="label">&nbsp;</th>
				<th class="occupancyHeader"><label id="lblAdults" runat="server" ml="Search Tool">Adults</label></th>
				<th class="occupancyHeader"><label id="lblChildren" runat="server" ml="Search Tool">Children<em> (2-17)</em></label></th>
				<th class="occupancyHeader"><label id="lblInfants" runat="server" ml="Search Tool">Infants<em> (0-1)</em></label></th>
			</tr>

			<tr id="trGuests_1">
				<th id="tdRoom1" class="label" ml="Search Tool" runat="server">Room 1</th>
				<td><web:dropdown id="ddlAdults_1" runat="server" /></td>
				<td id="tdRoom1_Children"><web:dropdown id="ddlChildren_1" runat="server" script="SearchTool.Show();" /></td>
				<td id="tdRoom1_Infants"><web:dropdown id="ddlInfants_1" runat="server" /></td>
			</tr>
			<tr id="trAges_1" style="display:none;">
				<td id="tdAgesLabel_1" class="childAges" colspan="2" ml="Search Tool">Child ages</td>
				<td colspan="2" class="ageDropdowns">
					<web:dropdown id="ddlChildAge_1_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

			<tr id="trGuests_2" style="display:none;">
				<th class="label" ml="Search Tool">Room 2</th>
				<td><web:dropdown id="ddlAdults_2" runat="server" /></td>
				<td><web:dropdown id="ddlChildren_2" runat="server" script="SearchTool.Show();" /></td>
				<td><web:dropdown id="ddlInfants_2" runat="server" /></td>
			</tr>
			<tr id="trAges_2" style="display:none;">
				<td class="childAges" colspan="2" ml="Search Tool">Child ages</td>
				<td colspan="2" class="ageDropdowns">
					<web:dropdown id="ddlChildAge_2_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

			<tr id="trGuests_3" style="display:none;">
				<th class="label" ml="Search Tool">Room 3</th>
				<td><web:dropdown id="ddlAdults_3" runat="server" /></td>
				<td><web:dropdown id="ddlChildren_3" runat="server" script="SearchTool.Show();" /></td>
				<td><web:dropdown id="ddlInfants_3" runat="server" /></td>
			</tr>
			<tr id="trAges_3" style="display:none;">
				<td class="childAges" colspan="2" ml="Search Tool">Child ages</td>
				<td colspan="2" class="ageDropdowns">
					<web:dropdown id="ddlChildAge_3_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

		</table>

		<input type="hidden" id="hidAdultsSingularSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidAdultsPluralSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidChildrenSingularSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidChildrenPluralSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidInfantsSingularSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidInfantsPluralSuffix" runat="server" ml="Search Tool" />

		<input type="hidden" id="hidAgeSingularSuffix" runat="server" ml="Search Tool" />
		<input type="hidden" id="hidAgePluralSuffix" runat="server" ml="Search Tool" />

	</div>




	<!-- advanced options-->
	<div id="divSearch_Advanced" class="form grouped" style="display:none;" runat="server">
		<h3 ml="Search Tool">Advanced Options</h3>
		<fieldset id="fldAdvanced">		
			<dl>
				<dt><label ml="Search Tool">Meal Basis</label></dt>
				<dd><web:dropdown id="ddlMealBasisID" Lookup="MealBasis" runat="server" /></dd>
				<dt><label ml="Search Tool">Min Star Rating</label></dt>
				<dd>
					<label class="checkboxLabel"><input type="checkbox" id="cb5Star" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');SearchTool.Support.SetRating();" /><span class="rating star5 small"></span></label>
					<label class="checkboxLabel"><input type="checkbox" id="cb4Star" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');SearchTool.Support.SetRating();" /><span class="rating star4 small"></span></label>
					<label class="checkboxLabel"><input type="checkbox" id="cb3Star" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');SearchTool.Support.SetRating();" /><span class="rating star3 small"></span></label>
				</dd>
			</dl>
		</fieldset>
		<input type="hidden" id="hidRating" name="hidRating" value="0" runat="server" />
		<input type="hidden" id="hidPropertyReferenceID" name="hidPropertyReferenceID" value="0" runat="server" />
	</div>
	<a href="javascript:void;" id="aShowAdvancedOptions" onclick="SearchTool.Support.ToggleAdvancedOptions();return false;" runat="server">Show Advanced Options</a>
	<a href="javascript:void;" id="aHideAdvancedOptions" onclick="SearchTool.Support.ToggleAdvancedOptions();return false;" style="display:none" runat="server">Hide Advanced Options</a>

	<a type="button" class="button" id="btnClear" onclick="SearchTool.Clear();" runat="server" ml="Search Tool">Clear</a>
	<a type="button" class="button" id="btnHide" onclick="int.f.Hide('divSearch');int.f.Show('divSearchAgain');" runat="server" ml="Search Tool">Hide</a>
	<a type="button" class="button" id="btnClose" onclick="web.ModalPopup.Hide();" runat="server" ml="Search Tool" style="display:none;">Close</a>

	<a type="button" class="button primary icon" id="btnSearch" onclick="SearchTool.Search();" runat="server" ml="Search Tool">Search Now</a>
	
	
	<input type="hidden" id="hidSingleRoomLabel" value="Guests" ml="Search Tool" runat="server" />
	<input type="hidden" id="hidMultiRoomLabel" value="Room 1" ml="Search Tool"  runat="server" />

	<input type="hidden" id="hidWarning_Invalid" value="Please ensure that all required fields are set. Incorrect fields have been highlighted." ml="Search Tool" runat="server" />
	<input type="hidden" id="hidWarning_NoResults" value="Sorry but there is no availability for your search criteria. Please try an alternative search." ml="Search Tool" runat="server" />

	<input type="hidden" id="hidStartMode" value="SearchTool" runat="server" />
	<input type="hidden" id="hidExpandAction" value="Show" runat="server" />
	<input type="hidden" id="hidUseShortDates" value="false" runat="server" />
	<input type="hidden" id="hidSearchRedirects" value="false" runat="server" />

    <label id="lblHighRatedHotelFilter" class="checkboxLabel" runat="server">
        <input type="checkbox" id="chkHighRatedHotelFilter" name="chkHighRatedHotelFilter" class="checkbox" onclick="int.f.ToggleClass(this.parentNode, 'selected');" />
        <span id="spnHighRatedHotelFilter" runat="server">Show only high rated properties</span>
    </label>

</div>


<!-- Search Again -->
<div id="divSearchAgain" class="sidebarBox primary clear" style="display:none;" runat="server">
	<a type="button" class="button primary icon search" id="btnSearchAgain" onclick="SearchTool.Expand();" runat="server" ml="Search Tool">Search Again<i></i></a>
</div>



<!-- presearchscript -->
<int:placeholder id="plcPreSearchScript" runat="server" />

<script type="text/javascript">
	int.ll.OnLoad.Run(function () { SearchTool.Setup(); });
</script>
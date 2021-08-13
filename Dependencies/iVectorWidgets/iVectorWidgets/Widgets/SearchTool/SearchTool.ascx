<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SearchTool.ascx.vb" Inherits="iVectorWidgets.SearchToolUserControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>


<div id="divSearch" class="sidebarBox primary" runat="server" enableviewstate="false" >
	<div class="boxTitle">
		<h2 id="h2_SearchTitle" runat="server">Search</h2>
	</div>


	<!-- search mode -->
	<div id="divSearchMode" class="tabs radio">
		<ul id="ulSearchMode">
			<li id="li_SearchMode_FlightPlusHotel" runat="server"><a href="#" onclick="SearchTool.Support.SetSearchMode('FlightPlusHotel');return false;"><label>Holiday</label></a></li>
			<li id="li_SearchMode_Hotel" runat="server"><a href="#" onclick="SearchTool.Support.SetSearchMode('Hotel');return false;"><label>Hotel Only</label></a></li>
			<li id="li_SearchMode_Flight" runat="server"><a href="#" onclick="SearchTool.Support.SetSearchMode('Flight');return false;"><label>Flight Only</label></a></li>
		</ul>
		<int:hidden id="hidSearchMode" runat="server" />
	</div>



	<!-- where from and to -->
	<div id="divSearch_Where" class="form grouped">
		<h3>Where?</h3>
		<fieldset id="fldDeparting">
			<dl>
				<dt><label>Departing from</label></dt>
				<dd><web:dropdown id="ddlDepartingFromID" overrideblanktext="Leaving from..." lookup="AirportGroupAndAirport" runat="server" /></dd>
			</dl>
		</fieldset>

		<fieldset id="fldTo">
			<dl>
				<dt><label>To</label></dt>
				<dd><web:dropdown id="ddlArrivingAtID" runat="server" overrideblanktext="Going to..." lookup="CountryRegion" includeparent="true"/></dd>
			</dl>
		</fieldset>
	</div>


	<!-- when and for how long -->
	<div id="divSearch_When" class="form grouped">
		<h3>When?</h3>	
		<fieldset id="fldWhen">
			<dl>
				<dt><label>Departing</label></dt>
				<dd>
					<div class="textbox icon calendar right embedded">
						<i></i>
						<input id="txtDepartureDate" type="textbox" class="textbox"/>
					</div>				
				</dd>
			</dl>
			<dl>
				<dt><label>Nights</label></dt>
				<dd><web:dropdown id="ddlDuration" cssclass="range_1_17" runat="server" /></dd>
			</dl>
		</fieldset>

	</div>


	<!-- advanced options-->
	<div id="divSearch_Advanced" class="form grouped" style="display:none;">
		<h3>Advanced Options</h3>
		<fieldset id="fldAdvanced">
		
			<dl>
				<dt><label>Meal Basis</label></dt>
				<dd><web:dropdown id="ddlMealBasisID" Lookup="MealBasis" runat="server" /></dd>
			</dl>
		</fieldset>
	</div>


	<!-- rooms and guests -->
	<div id="divSearch_RoomsAndGuests" class="form grouped">
		<h3>Who?</h3>
		
		<fieldset>
			<dl>
				<dt><label>Rooms</label></dt>
				<dd><web:dropdown id="ddlRooms" cssclass="range_1_3" runat="server" script="SearchTool.Show();" /></dd>
			</dl>
		</fieldset>

		<table id="tblOccupancy">
			<tr>
				<td class="label">&nbsp;</td>
				<td>Adults</td>
				<td>Children <em>(2-18)</em></td>
				<td>Infants <em>(0-1)</em></td>
			</tr>

			<tr id="trGuests_1">
				<td class="label">Room 1</td>
				<td><web:dropdown id="ddlAdults_1" cssclass="range_1_9"  runat="server" /></td>
				<td><web:dropdown id="ddlChildren_1" cssclass="range_0_4" runat="server" script="SearchTool.Show();" /></td>
				<td><web:dropdown id="ddlInfants_1" cssclass="range_0_9" runat="server" /></td>
			</tr>
			<tr id="trAges_1" style="display:none;">
				<td class="childAges" colspan="2">Child ages</td>
				<td colspan="2">
					<web:dropdown id="ddlChildAge_1_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_1_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

			<tr id="trGuests_2" style="display:none;">
				<td class="label">Room 2</td>
				<td><web:dropdown id="ddlAdults_2" cssclass="range_1_9" runat="server" /></td>
				<td><web:dropdown id="ddlChildren_2" cssclass="range_0_4" runat="server" script="SearchTool.Show();" /></td>
				<td><web:dropdown id="ddlInfants_2" cssclass="range_0_9" runat="server" /></td>
			</tr>
			<tr id="trAges_2" style="display:none;">
				<td class="childAges" colspan="2">Child ages</td>
				<td colspan="2">
					<web:dropdown id="ddlChildAge_2_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_2_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

			<tr id="trGuests_3" style="display:none;">
				<td class="label">Room 3</td>
				<td><web:dropdown id="ddlAdults_3" cssclass="range_1_9" runat="server" /></td>
				<td><web:dropdown id="ddlChildren_3" cssclass="range_0_4" runat="server" script="SearchTool.Show();" /></td>
				<td><web:dropdown id="ddlInfants_3" cssclass="range_0_9" runat="server" /></td>
			</tr>
			<tr id="trAges_3" style="display:none;">
				<td class="childAges" colspan="2">Child ages</td>
				<td colspan="2">
					<web:dropdown id="ddlChildAge_3_1" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_2" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_3" cssclass="range_2_17" runat="server"/>
					<web:dropdown id="ddlChildAge_3_4" cssclass="range_2_17" runat="server"/>
				</td>
			</tr>

		</table>
	</div>


	<input type="button" class="button primary" id="btnSearch" onclick="SearchTool.Search();" value="Search" runat="server" />


	
	<int:hidden id="hidBookAheadDays" runat="server" />
	<int:hidden id="hidSingleRoomLabel" runat="server" value="Guests"/>
	<int:hidden id="hidMultiRoomLabel" runat="server" value="Room 1"/>
	<int:hidden id="hidDepartureDate" runat="server" />
	<int:hidden id="hidReturnDate" runat="server" />
	<int:hidden id="hidCachedSearch" runat="server" />

</div>

<script type="text/javascript">
	int.ll.OnLoad.Run(function () { SearchTool.Setup(); });
</script>

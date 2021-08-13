<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CarHire.ascx.vb" Inherits="iVectorWidgets.CarHire1" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>


<input type="hidden" id="hidCarHireDatePickerShowOn" value="both" />
<input type="hidden" id="hidCarHireDatePickerButtonText" value="<i class='fa fa-calendar'></i>" />
<input type="hidden" id="hidDefaultDriverCountryID" runat="server" />
<input type="hidden" id="hidAutoSearch" runat="server" />
<input type="hidden" id="hidCarHireResultsPage" runat="server" />


<div id="divCarHire" class="box primary clear">

	<div class="boxTitle" id="divCarHireTitle">
		<h2 id="h2CarHire" runat="server">Car Hire</h2>

		<p id="pSubTitle" runat="server" visible="false">Why not try our great value car hire?</p>	
	</div>	

	<div id="divCarHireContainer" runat="server">
		
		<div id="divCarHireSearch" class="clear" runat="server">		

			<fieldset>
				<div class="clear">
					<label>Pick up</label>
					<web:dropdown id="ddlCarHirePickUpDepotID" runat="server" />
				</div>
		
				<div class="clear">
					<label>Date</label>
					
					<div class="textbox icon calendar">
						<i class="carHirePickupDate"></i>
						<input class="textbox" type="text" id="txtCarHirePickUpDate" readonly="true"/>
						<input id="hidCarHirePickUpDate" type="hidden" runat="server" />
					</div>
					
					<label class="time">Time</label>
					<input id="txtCarHirePickUpTime" name="txtCarHirePickUpTime" type="text" class="textbox time" onblur="ParseTime(this);" placeholder="HH:MM"/>
			
				</div>
		
			</fieldset>

	
			<fieldset>
				<div class="clear">
					<label>Drop off</label>
					<web:dropdown id="ddlCarHireDropOffDepotID" runat="server" />
				</div>

				<div class="clear">
					<label>Date</label>
					<div class="textbox icon calendar">
						<i class="carHireDropOffDate"></i>
						<%--<input id="txtCarHireDropOffDate" name="txtDropOffDate" type="text" class="textbox"/>--%>
						<input class="textbox" type="text" id="txtCarHireDropOffDate" readonly="true"/>
						<input id="hidCarHireDropOffDate" type="hidden" runat="server" />
					</div>
					
					<label class="time">Time</label>
					<input id="txtCarHireDropOffTime" name="txtCarHireDropOffTime" type="text" class="textbox time" onblur="ParseTime(this);" placeholder="HH:MM"/>
				
				</div>
			
			</fieldset>

			<dl class="pax clear">
				<dt><label>Main driver age</label></dt>
				<dd>
					<web:dropdown id="ddlCarHireDriverAge" runat="server" />
				</dd>
				<dt><label>Main driver country of residence</label></dt>
				<dd>
					<web:dropdown id="ddlCarHireLeadDriverBookingCountryID" runat="server" lookup="Country"/>
				</dd>
				<dt><label>Total passengers (incl. driver)</label></dt>
				<dd>
					<select id="ddlCarHireTotalPassengers">
						<option value="1">1</option>
						<option value="2">2</option>
						<option value="3">3</option>
						<option value="4">4</option>
						<option value="5">5</option>
						<option value="6">6</option>
						<option value="7">7</option>
						<option value="8">8</option>
					</select>
				</dd>
			</dl>

			<input id="btnSearchCarHire" class="button primary search" type="button" onclick="CarHire.Search();return false;" value="Search" />

			<input id="btnChangeCarHire"  class="button" type="button" onclick="$('#divCarHireResults').show();return false;" value="Change car" style="display:none" />

		</div>

		<input id="btnSearchAgain"  class="button primary" type="button" value="Search again" style="display:none" />

		<div id="divSelectedCar" runat="server">
		
		</div>

		<p id="pSearchWait" style="display:none">Please wait whilst we search for your car hire...</p>

		<div id="divCarHireResults" class="clear" style="display:none">
				
	
		</div>

	</div>	

	<div id="divCustomScript" runat="server">
	
	</div>

</div>


<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="LeadGuestDetails.ascx.vb" Inherits="iVectorWidgets.LeadGuestDetailsControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>

<div id="divLeadGuestDetails" class="box primary form" runat="server">

	<div class="boxTitle">
		<h2 id="h2LeadGuestDetails" ml="Lead Guest Details" runat="server">Lead Guest Details</h2>
	</div>


	<table>
		<tr>
			<td ml="Lead Guest Details">Title *</td>
			<td><web:dropdown id="ddlLeadGuestDetails_Title" options="Mr|Mr#Mrs|Mrs#Miss|Miss#Ms|Ms" runat="server" addblank="true" overrideblanktext="Title..." multilingualtag="Lead Guest Details;Title" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">First Name *</td>
			<td><input id="txtLeadGuestDetails_FirstName" type="text" class="textbox large" maxlength="30" runat="server" /></td>
		</tr    
		<tr>
			<td ml="Lead Guest Details">Last Name *</td>
			<td><input id="txtLeadGuestDetails_LastName" type="text" class="textbox large" maxlength="30" runat="server" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Email *</td>
			<td><input id="txtLeadGuestDetails_Email" type="text" class="textbox large" maxlength="50" runat="server" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Repeat Email *</td>
			<td><input id="txtLeadGuestDetails_RepeatEmail" type="text" class="textbox large" maxlength="50" runat="server" /></td>
		</tr>
		<tr runat="server" id="trPostcodeLookup">
			<td>
				<table>
					<tr id="trPostcodeLookup_Heading">
						<td ml="Lead Guest Details">Look up your address by entering your post code or enter your address manually.</td>
					</tr>
					<tr id="trPostcodeLookup_Postcode">
						<td ml="Lead Guest Details">Postcode</td>
						<td><input id="txtPostcodeLookup_Postcode" type="text" class="textbox tiny" maxlength="15" runat="server" placeholder="Postcode" /></td>
						<td ml="Lead Guest Details" id="tdPostcodeLookup_Find"><a id="aPostcodeLookup_Find" href="javascript:void(0);">Find</a></td>
					</tr>
					<tr id="trPostcodeLookup_Select" style="display: none">
						<td></td>
						<td id="tdPostcodeLookup_Select"><select id="ddlPostcodeLookup_Addresses"></select></td>
						<td></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Address *</td>
			<td><input id="txtLeadGuestDetails_Address" type="text" class="textbox large" maxlength="70" runat="server" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">City *</td>
			<td><input id="txtLeadGuestDetails_City" type="text" class="textbox small" maxlength="30" runat="server" /></td>
		</tr>
		<tr id="trPostCode" runat="server">
			<td ml="Lead Guest Details" id="lblLeadGuestDetails_Postcode" runat="server">Postcode *</td>
			<td><input id="txtLeadGuestDetails_Postcode" type="text" class="textbox tiny" maxlength="15" runat="server" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Country *</td>
			<td><web:dropdown id="ddlLeadGuestDetails_BookingCountry" cssclass="large" addblank="true" lookup="BookingCountry" overrideblanktext="Country..." runat="server" multilingualtag="Lead Guest Details;Country" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Telephone Number *</td>
			<td><input id="txtLeadGuestDetails_PhoneNumber" type="text" class="textbox small" runat="server" /></td>
		</tr>
		<tr>
			<td ml="Lead Guest Details">Mobile Number</td>
			<td><input id="txtLeadGuestDetails_MobileNumber" type="text" class="textbox small" runat="server" /></td>
		</tr>
	</table>


	<input type="hidden" id="hidLeadGuestDetails_ValidationExclude" runat="server" />

</div>
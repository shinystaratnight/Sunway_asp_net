<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="HotelRequests.ascx.vb" Inherits="iVectorWidgets.HotelRequestsControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>

<div id="divHotelRequests" class="box primary" runat="server">

	<div class="boxTitle">
		<h2 id="hHotelRequests_Title" ml="Hotel Requests" runat="server">Hotel Requests</h2>
	</div>

	<p id="pHotelRequests_Text" ml="Hotel Requests" runat="server">Please enter any special requests you have for the hotel below.</p>

	<!-- arrival time dropdown -->
	<div id="divArrivalTime" runat="server" visible="false">
		<label>Arrival Time*</label>
		<int:dropdown ID="ddlArrivalTime" runat="server" autofilter="false"/>
	</div>

	<textarea id="txtHotelRequests_Requests" rows="10" runat="server" placeholder="Please enter your requests here"></textarea>

</div>
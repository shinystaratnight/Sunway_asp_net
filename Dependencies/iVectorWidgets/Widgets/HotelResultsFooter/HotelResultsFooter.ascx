<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="HotelResultsFooter.ascx.vb" Inherits="iVectorWidgets.HotelResultsFooterControl" %>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>

<div id="divHotelResultsFooter">

	<div id="divHotelResultsFooterControls" class="box clear" runat="server">
		<div id="divPagingBottom" runat="server">
			<web:paging id="pagingBottom" runat="server" />
		</div>
	</div>

    <input type="button" class="button large" id="btnBackToTop" value="Back to Top" runat="server" />

	<p id="pDisclaimerText" runat="server"></p>


	<script type="text/javascript">
		int.ll.OnLoad.Run(function () { HotelResultsFooter.Setup(); });
	</script>

</div>
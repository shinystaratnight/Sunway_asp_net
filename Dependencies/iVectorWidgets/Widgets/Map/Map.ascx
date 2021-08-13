<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Map.ascx.vb" Inherits="iVectorWidgets.MapControl" %>


<div id="divMapHolder" class="sidebarBox primary" runat="server">

	<div class="boxTitle">
		<h2 id="h2Title" ml="Map" runat="server">Map</h2>
	</div>

	<div id="divMap"></div>

	<input type="hidden" id="hidMapIcons" value="" runat="server" />

	<input type="hidden" id="hidMapTheme" value="" runat="server" />
	<input type="hidden" id="hidMapPoints" value="" runat="server" />

	<input type="hidden" id="hidPopupX" value="" runat="server" />
	<input type="hidden" id="hidPopupY" value="" runat="server" />
	<input type="hidden" id="hidClusterPopupX" value="" runat="server" />
	<input type="hidden" id="hidClusterPopupY" value="" runat="server" />
</div>


<script type="text/javascript">
	MapWidget.Setup();
</script>
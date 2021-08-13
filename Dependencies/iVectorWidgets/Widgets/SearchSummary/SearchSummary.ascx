<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SearchSummary.ascx.vb" Inherits="iVectorWidgets.SearchSummaryControl" %>
<%@ Register Namespace="Intuitive.WebControls" TagPrefix="int" Assembly="Intuitive"%>
<%@ Register Namespace="Intuitive.Web.Controls" TagPrefix="web" Assembly="IntuitiveWeb"%>


<div id="divSearchSummary" runat="server">

	<h4 id="hSummary" runat="server"></h4>

	<div id="divSearchSummaryControls" class="box clear" runat="server">

		<div id="divPagingTop">
			<web:paging id="pagingTop" runat="server" />
		</div>

		<div id="divSortOrder" runat="server">
			<label id="lblSortBy" runat="server" ml="Search Summary">Sort by</label>
			<int:dropdown id="ddlSortOrder" options="Price Ascending|Price_Ascending#Price Descending|Price_Descending#Rating Ascending|Rating_Ascending#Rating Descending|Rating_Descending#Name A to Z |Name_Ascending#Name Z to A |Name_Descending" addblank="false" AutoFilter="false"  script="SearchSummary.Sort();" multilingualtag="Search Summary;Sort Order" runat="server" />
		</div>
		
		<div id="divSearchSummaryViewTabs" class="clear" runat="server">		
			<a id="aListView" href="javascript:void(0)" onclick="MapWidget.Hide();HotelResults.SetCurrentView('Results');HotelResults.HideMap();HotelResultsFooter.Show();HotelResultsFilter.ClearBestSeller(true);SearchSummary.Show();SearchSummary.SelectTab(this);return false;" class="selected">List View</a>
			<a id="aMapView" href="javascript:void(0)" onclick="MapWidget.Show();HotelResults.Hide();HotelResults.ShowMap();HotelResultsFooter.Hide();HotelResultsFilter.ClearBestSeller(true);SearchSummary.Hide();SearchSummary.SelectTab(this);return false;" runat="server">Map View</a>
			<a id="aQuickView" href="javascript:void(0);" onclick="MapWidget.Hide();HotelResults.SetCurrentView('QuickView');HotelResults.HideMap();HotelResultsFooter.Show();HotelResultsFilter.ClearBestSeller(true);SearchSummary.Show();SearchSummary.SelectTab(this);return false;" runat="server">Quick View</a>
			<a id="aPopular" href="javascript:void(0)" onclick="MapWidget.Hide();HotelResults.Show();HotelResults.HideMap();HotelResultsFooter.Show();HotelResultsFilter.SetBestSeller(true);SearchSummary.Show();SearchSummary.SelectTab(this);return false;" runat="server">Popular</a>		
		</div>

		<div id="divPopularHelper" class="popularHelperPopup" runat="server" ml="SearchSummary"></div>

	</div>

</div>
var SearchSummary = SearchSummary || {};

//#region Paging		
SearchSummary.CurrentPage = 1;

web.PubSub.subscribe('SearchResults_Updated', function () {
    SearchSummary.Update();
});

SearchSummary.PreviousPage = function () {
	SearchSummary.SelectPage(SearchSummary.CurrentPage - 1);
};

SearchSummary.NextPage = function () {
	SearchSummary.SelectPage(SearchSummary.CurrentPage + 1);
};

SearchSummary.SelectPage = function (iPage) {
    SearchSummary.CurrentPage = iPage;
    int.ff.Call(
		'=iVectorWidgets.SearchSummary.SelectPage',
		function (sHTML) {
		    int.f.SetHTML('divPagingTop', sHTML);
		    int.f.SetHTML('divPagingBottom', sHTML);
		    int.e.ScrollIntoView('divPagingTop', 10, 2);
		    HotelResults.UpdateResults();
		},
		iPage
	);
};

SearchSummary.UpdatePaging = function () {
    SearchSummary.CurrentPage = 1;
    int.ff.Call(
		'=iVectorWidgets.SearchSummary.SelectPage',
		function (sHTML) {
		    int.f.SetHTML('divPagingTop', sHTML);
		},
		SearchSummary.CurrentPage
	);
};

//#endregion


//#region Sort

SearchSummary.Sort = function () {

    var sSortBy = int.dd.GetValue('ddlSortOrder').split('_')[0];
    var sSortOrder = int.dd.GetValue('ddlSortOrder').split('_')[1];

    int.ff.Call('=iVectorWidgets.SearchSummary.SortResults', HotelResults.UpdateResults, sSortBy, sSortOrder);

};

SearchSummary.FlightSort = function () {

    var sSortBy = int.dd.GetValue('ddlFlightSortOrder').split('_')[0];
    var sSortOrder = int.dd.GetValue('ddlFlightSortOrder').split('_')[1];

    int.ff.Call('=iVectorWidgets.SearchSummary.SortFlightResults', FlightResults.UpdateResults, sSortBy, sSortOrder);

};

//#endregion


//#region Update

SearchSummary.Update = function () {
	int.ff.Call('=iVectorWidgets.SearchSummary.Update', SearchSummary.UpdateComplete);
}

SearchSummary.UpdateComplete = function (sJSON) {

	var oReturn = JSON.parse(sJSON);

	int.f.SetHTML('hSummary', oReturn.SummaryHeader);
	int.f.SetHTML('divPagingTop', oReturn.PagingControl);
	if (int.f.GetObject('spnHotelCount')) {
		int.f.SetHTML('spnHotelCount', oReturn.HotelCount);
	}

}

//#endregion

SearchSummary.RenderControl = function () {
	int.ff.Call('=iVectorWidgets.SearchSummary.RenderControlHTML', SearchSummary.RenderControlComplete);
}

SearchSummary.RenderControlComplete = function (sHTML) {
	int.f.SetHTML('divSummaryContainer', sHTML);
}

SearchSummary.Hide = function () {
	int.f.Hide('divPagingTop');
	int.f.Hide('divSortOrder');
}


SearchSummary.Show = function () {
	int.f.Show('divPagingTop');
	int.f.Show('divSortOrder');
}



SearchSummary.SelectTab = function (oTab) {
	$('#divSearchSummaryViewTabs .selected').removeClass('selected');
	int.f.AddClass(oTab, 'selected');
	int.f.GetObject('divPopularHelper') && oTab.id == 'aPopular' ? int.f.Hide('divPopularHelper') : int.f.Show('divPopularHelper');

	var aTabs = int.f.GetObjectsByIDPrefix('a', 'a', 'divSearchSummaryViewTabs');

	for (var i = 0; i < aTabs.length; i++) {
		this.DisableButton(aTabs[i].id);
	}
}

SearchSummary.DisableButton = function (sElementId) {
	var oBtn = int.f.GetObject(sElementId);
	oBtn.disabled = true;

	setTimeout(function () { SearchSummary.EnableButton(sElementId) }, 2500);
}

SearchSummary.EnableButton = function (sElementId) {
	var oBtn = int.f.GetObject(sElementId);
	oBtn.disabled = false;
}
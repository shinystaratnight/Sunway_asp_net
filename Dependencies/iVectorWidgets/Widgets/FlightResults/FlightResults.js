var FlightResults = FlightResults || {};

web.PubSub.subscribe('FlightResults.UpdateResults', function () {
    FlightResults.UpdateResults();
});

FlightResults.CurrentPage = 1;

//#region Paging
FlightResults.PreviousPage = function () {
	FlightResults.UpdateResults(FlightResults.CurrentPage - 1);
}
FlightResults.NextPage = function () {
	FlightResults.UpdateResults(FlightResults.CurrentPage + 1);
}
FlightResults.SelectPage = function (iPage) {
	FlightResults.UpdateResults(iPage);
}

//#endregion


//#region UpdateResults
FlightResults.UpdateResults = function (iPage) {
	iPage = iPage || 1;
	FlightResults.CurrentPage = iPage;

	//for now, set the includes baggage price param to false
	var bIncludeBagagePrice = false

	int.ff.Call('=iVectorWidgets.FlightResults.UpdateResults', FlightResults.UpdateResultsComplete, iPage, bIncludeBagagePrice);
}

FlightResults.UpdateResultsComplete = function (sReturn) {
	var oReturn = JSON.parse(sReturn);
	int.f.SetHTML('divFlightResults', oReturn.FlightResultsHTML);
	int.f.Show('divFlightResults');
	if (!!int.f.GetObject('divFlightResultsFooter')) int.f.SetHTML('divFlightResultsFooter', oReturn.PagingHTML + '<div class="clear"></div>');
	if (!!int.f.GetObject('divFlightResultsHeader')) int.f.SetHTML('divFlightResultsHeader', oReturn.PagingHTML + '<div class="clear"></div>');
	if (!!int.f.GetObject('divPagingTop')) int.f.SetHTML('divPagingTop', oReturn.PagingHTML + '<div class="clear"></div>');
	int.e.ScrollIntoView('divMain', int.e.GetPosition('divMain').Height * -1, 2)
	if (FlightResults.AttachEvents != undefined && FlightResults.AttachEvents != null) { FlightResults.AttachEvents(); };
}
//#endregion


//#region Select Flight
FlightResults.SelectFlight = function (sFlightOptionHashToken) {
	WaitMessage.Show('PreBook');
	int.ff.Call('=iVectorWidgets.FlightResults.AddFlightOptionToBasket', FlightResults.SelectFlightComplete, sFlightOptionHashToken);
}

FlightResults.SelectFlightComplete = function (sReturn) {
	if (int.s.StartsWith(sReturn, 'Error')) {
		WaitMessage.Hide();
		var sWaring = sReturn.split('|')[1];
		web.InfoBox.Show(sWaring);
	}
	else {
		web.Window.Redirect(sReturn);
	};
}
//#endregion

FlightResults.MobileShowDetails = function (oContainer) {
	var aDivs = int.f.GetElementsByClassName('div', 'mobileFlightDetails', 'divFlightResults');

	for (var i = 0; i < aDivs.length; i++) {
		int.f.Hide(aDivs[i]);
	}


	var aDivs = int.f.GetElementsByClassName('div', 'mobileFlightDetails', oContainer);

	for (var i = 0; i < aDivs.length; i++) {
		int.f.Show(aDivs[i]);
	}
}
	
//in case we have show a hotel with the flight
FlightResults.ShowDetailsPopup = function (iPropertyReferenceID) {
	HotelPopup.ShowPopup(iPropertyReferenceID, true);
}


FlightResults.CalculateDuration = function (iDepTime, iArrTime) {

	function toText(m) {
		var minutes = m % 60;
		var hours = Math.floor(m / 60);

		minutes = (minutes < 10 ? '0' : '') + minutes;
		hours = (hours < 10 ? '0' : '') + hours;

		return hours + ' hrs ' + minutes + ' mins';
	}



	var aDep = iDepTime.split(':');
	var aArr = iArrTime.split(':');

	var iStart = (int.n.SafeInt(aDep[0]) * 60) + int.n.SafeInt(aDep[1]);
	var iEnd = (int.n.SafeInt(aArr[0]) * 60) + int.n.SafeInt(aArr[1]);

	var iDuration = iEnd - iStart;

	if (iDuration < 0) {
		iDuration = (iDuration + 1440)
		int.f.SetHTML('spanToolTipValue', toText(iDuration));

	}
	else {
		int.f.SetHTML('spanToolTipValue', toText(iDuration));
	}


}

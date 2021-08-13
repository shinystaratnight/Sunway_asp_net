var ChangeFlight = ChangeFlight || {};

//show
ChangeFlight.ShowFlights = function () {

	var oFlights = int.f.GetObject('divFlightOptions');
	int.f.Toggle(oFlights);

	//If the flights are showing then change the Change Flights button to display 'Cancel'
	if (int.f.Visible(oFlights)) {
		if (int.f.GetValue('hidChangeFlightTextAlt') == '') {
			int.f.SetHTML('aChangeFlight', 'Cancel');
		} else {
			int.f.SetHTML('aChangeFlight', int.f.GetValue('hidChangeFlightTextAlt'));
		};
	}
	else {
		if (int.f.GetValue('hidChangeFlightText') == '') {
			int.f.SetHTML('aChangeFlight', 'Change Flight');
		} else {
			int.f.SetHTML('aChangeFlight', int.f.GetValue('hidChangeFlightText'));
		};
	}
}


//update prices
ChangeFlight.LabelPrices = function (iCurrentPrice) {
	//Change the Prices to '-' or '+' relative to original price
	var aPrices = int.f.GetObjectsByIDPrefix('hFlightPrice_', 'h2', 'divFlightOptions');

	for (var i = 0; i < aPrices.length; i++) {

		if (aPrices[i].innerHTML > iCurrentPrice) {
			int.f.SetHTML(aPrices[i].id + '_displayed', '+' + ((aPrices[i].innerHTML - iCurrentPrice).toFixed(2)).toString())
		}
		else if (aPrices[i].innerHTML < iCurrentPrice) {
			int.f.SetHTML(aPrices[i].id + '_displayed', ((aPrices[i].innerHTML - iCurrentPrice).toFixed(2)).toString())
		}
	}
}


//select flight
ChangeFlight.SelectFlight = function (sBookingToken) {
	int.ff.Call('=iVectorWidgets.ChangeFlight.SelectFlight', ChangeFlight.SelectFlightDone, sBookingToken);
}


//select flight done
ChangeFlight.SelectFlightDone = function (sJSON) {

	var oReturn = JSON.parse(sJSON);

	if (oReturn.Success === true) {
		int.f.SetHTML('ChangeFlightHolder', oReturn.HTML);
		HotelResults.UpdateResults();
		int.e.ScrollIntoView('divTopHeader');
	} else {
		web.Window.Redirect(oReturn.RedirectURL);
	}
}

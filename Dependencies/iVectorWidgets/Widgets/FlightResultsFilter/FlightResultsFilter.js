var FlightResultsFilter = FlightResultsFilter || {};

FlightResultsFilter.UpdateFunction;

FlightResultsFilter.Setup = function (UpdateFunction) {
	FlightResultsFilter.UpdateFunction = UpdateFunction;
	//add events to filter checkboxes
	for (var aFilters = FlightResultsFilter.GetFilterCheckboxes(), i = 0; i < aFilters.length; i++) {
		int.f.AttachEvent(aFilters[i], 'click', FlightResultsFilter.Filter);
	}

	var aSections = int.f.GetElementsByClassName('h4', 'sectionHeader', 'divFlightFilter');

	for (var i = 0; i < aSections.length; i++) {

		int.f.AttachEvent(
			aSections[i],
			'click',
			function () {
				FlightResultsFilter.ToggleSection(this);
			}
		);
	}

}

//get filter checkboxes
FlightResultsFilter.GetFilterCheckboxes = function () {
	if (int.f.GetObject('divFlightFilter')) {
		return int.f.GetObjectsByIDPrefix('chk_', 'input', 'divFlightFilter');
	}
	else {
		return new Array();
	}
}

FlightResultsFilter.Filter = function () {

	//price
	var iMinPrice = int.f.GetValue('sldFlightPrice_MinValue');
	var iMaxPrice = int.f.GetValue('sldFlightPrice_MaxValue');

	//departure times in minutes
	var iMinOutboundDepartureMinutes = int.f.GetValue('sldDeparture_MinValue');
	var iMaxOutboundDepartureMinutes = int.f.GetValue('sldDeparture_MaxValue');
	var iMinReturnDepartureMinutes = int.f.GetValue('sldReturn_MinValue');
	var iMaxReturnDepartureMinutes = int.f.GetValue('sldReturn_MaxValue');

	if (iMinOutboundDepartureMinutes == "") { iMinOutboundDepartureMinutes = 0 };
	if (iMaxOutboundDepartureMinutes == "") { iMaxOutboundDepartureMinutes = 1439 };
	if (iMinReturnDepartureMinutes == "") { iMinReturnDepartureMinutes = 0 };
	if (iMaxReturnDepartureMinutes == "") { iMaxReturnDepartureMinutes = 1439 };

	var aStops = new Array();
	var aCarriers = new Array();
	var aDepartureTOD = new Array();
	var aReturnTOD = new Array();
	var aDepartureAirports = new Array();
	var aArrivalAirports = new Array();
	var aClasses = new Array();

	var bIncludesSupplierBaggage = 'False';

	for (var aFilters = FlightResultsFilter.GetFilterCheckboxes(), i = 0; i < aFilters.length; i++) {
		if (aFilters[i].checked) {
			iID = aFilters[i].id.split('_')[2];
			if (int.s.StartsWith(aFilters[i].id, 'chk_stp_')) aStops.push(int.n.SafeInt(iID));
			if (int.s.StartsWith(aFilters[i].id, 'chk_fc_')) aCarriers.push(int.n.SafeInt(iID));
			if (int.s.StartsWith(aFilters[i].id, 'chk_dpt_')) aDepartureTOD.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_rtn_')) aReturnTOD.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_dptapt_')) aDepartureAirports.push(int.n.SafeInt(iID));
			if (int.s.StartsWith(aFilters[i].id, 'chk_arrapt_')) aArrivalAirports.push(int.n.SafeInt(iID));
			if (int.s.StartsWith(aFilters[i].id, 'chk_clss_')) aClasses.push(int.n.SafeInt(iID));
			if (aFilters[i].id == 'chk_inclbag') bIncludesSupplierBaggage = 'True';
		}
	}

	var oFilterRequest = {
		FilterDepartureAirportIDs: aDepartureAirports,
		FilterArrivalAirportIDs: aArrivalAirports,
		FilterFlightCarrierIDs: aCarriers,
		FilterDepartureTimes: aDepartureTOD,
		FilterReturnTimes: aReturnTOD,
		FilterStops: aStops,
		FilterFlightClassIDs: aClasses,
		MinOutboundDepartureFlightTimeMinutes: int.n.SafeInt(iMinOutboundDepartureMinutes),
		MaxOutboundDepartureFlightTimeMinutes: int.n.SafeInt(iMaxOutboundDepartureMinutes),
		MinReturnDepartureFlightTimeMinutes: int.n.SafeInt(iMinReturnDepartureMinutes),
		MaxReturnDepartureFlightTimeMinutes: int.n.SafeInt(iMaxReturnDepartureMinutes),
		MinPrice: int.n.SafeNumeric(iMinPrice),
		MaxPrice: int.n.SafeNumeric(iMaxPrice),
		IncludesSupplierBaggage: bIncludesSupplierBaggage
	};


	int.ff.Call(
		'=iVectorWidgets.FlightResultsFilter.FilterResults',
		FlightResultsFilter.FilterComplete,
		JSON.stringify(oFilterRequest)
	);


}

FlightResultsFilter.FilterComplete = function (sJSON) {

    console.log('Normal', JSON.parse(sJSON), sJSON);
	var oReturn = JSON.parse(sJSON);
	FlightResultsFilter.UpdateFilterOptions(oReturn);
	FlightResultsFilter.UpdateFunction();
}

FlightResultsFilter.UpdateFilterOptions = function (oReturn) {

	var oFilters = oReturn.Filters;

	//price
	var oCurrencySymbol = int.f.SafeObject('sldFlightPrice_CurrencySymbol');
	var oCurrencySymbolPosition = int.f.SafeObject('sldFlightPrice_CurrencySymbolPosition');

	var oMinPrice = int.f.SafeObject('sldFlightPrice_MinValue');
	var oMaxPrice = int.f.SafeObject('sldFlightPrice_MaxValue');

	var oPaxCount = int.f.SafeObject('sldFlightPrice_DisplayValueDivide');
	var iPaxCount = oPaxCount == null ? 1 : oPaxCount.value;

	if (oReturn.BookingAdjustmentAmount == null || oReturn.BookingAdjustmentAmount == undefined) {
		oReturn.BookingAdjustmentAmount = 0;
	}

	if (oMinPrice != null && oMaxPrice != null && oCurrencySymbol != null && oCurrencySymbolPosition != null) {
		var sMinPriceFormatted = int.n.FormatMoney(Math.floor((parseFloat(oMinPrice.value) + parseFloat(oReturn.BookingAdjustmentAmount)) / iPaxCount), oCurrencySymbol.value, oCurrencySymbolPosition.value);
		sMinPriceFormatted = sMinPriceFormatted.substring(0, sMinPriceFormatted.indexOf('.'));

		var sMaxPriceFormatted = int.n.FormatMoney(Math.floor((parseFloat(oMaxPrice.value) + parseFloat(oReturn.BookingAdjustmentAmount)) / iPaxCount), oCurrencySymbol.value, oCurrencySymbolPosition.value);
		sMaxPriceFormatted = sMaxPriceFormatted.substring(0, sMaxPriceFormatted.indexOf('.'));

		var sPerPerson = oPaxCount == null ? '' : 'pp';

		int.f.SetHTML('sldFlightPrice_Start', '<span id="sldFlightPrice_DisplayMinSlider">' + sMinPriceFormatted + sPerPerson + '</span>');
		int.f.SetHTML('sldFlightPrice_End', '<span  id="sldFlightPrice_DisplayMaxSlider">' + sMaxPriceFormatted + sPerPerson + '</span>');
						
	}

	//carriers
	var aCarrierCounts = int.f.GetObjectsByIDPrefix('spn_fc_count_', 'span', 'divFlightFilter');
	for (var i = 0; i < aCarrierCounts.length; i++) {
		for (var j = 0; j < oFilters.FlightCarriers.length; j++) {
			int.f.SetHTML(aCarrierCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.FlightCarriers[j].FlightCarrierID) == int.n.SafeNumeric(aCarrierCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aCarrierCounts[i], int.n.SafeInt(oFilters.FlightCarriers[j].Count));
				break;
			};
		};
	};

	//stops
	var aStopCounts = int.f.GetObjectsByIDPrefix('spn_stp_count_', 'span', 'divFlightFilter');
	for (var i = 0; i < aStopCounts.length; i++) {
		for (var j = 0; j < oFilters.Stops.length; j++) {
			int.f.SetHTML(aStopCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.Stops[j].Stops) == int.n.SafeNumeric(aStopCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aStopCounts[i], int.n.SafeInt(oFilters.Stops[j].Count));
				break;
			};
		};
	};

	//classes
	var aClassCounts = int.f.GetObjectsByIDPrefix('spn_clss_count_', 'span', 'divFlightFilter');
	for (var i = 0; i < aClassCounts.length; i++) {
		for (var j = 0; j < oFilters.FlightClasses.length; j++) {
			int.f.SetHTML(aClassCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.FlightClasses[j].FlightClassID) == int.n.SafeNumeric(aClassCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aClassCounts[i], int.n.SafeInt(oFilters.FlightClasses[j].Count));
				break;
			};
		};
	};

	//departure airports
	var aDepartureAirportCounts = int.f.GetObjectsByIDPrefix('spn_apt_count_dpt_', 'span', 'divFlightFilter');
	for (var i = 0; i < aDepartureAirportCounts.length; i++) {
		for (var j = 0; j < oFilters.DepartureAirports.length; j++) {
			int.f.SetHTML(aDepartureAirportCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.DepartureAirports[j].AirportID) == int.n.SafeNumeric(aDepartureAirportCounts[i].id.split('_')[4])) {
				int.f.SetHTML(aDepartureAirportCounts[i], int.n.SafeInt(oFilters.DepartureAirports[j].Count));
				break;
			};
		};
	};

	//arrival airports
	var aArrivalAirportCounts = int.f.GetObjectsByIDPrefix('spn_apt_count_arr_', 'span', 'divFlightFilter');
	for (var i = 0; i < aArrivalAirportCounts.length; i++) {
		for (var j = 0; j < oFilters.ArrivalAirports.length; j++) {
			int.f.SetHTML(aArrivalAirportCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.ArrivalAirports[j].AirportID) == int.n.SafeNumeric(aArrivalAirportCounts[i].id.split('_')[4])) {
				int.f.SetHTML(aArrivalAirportCounts[i], int.n.SafeInt(oFilters.ArrivalAirports[j].Count));
				break;
			};
		};
	};

	//depart times
	var aDepartTimeCounts = int.f.GetObjectsByIDPrefix('spn_tod_count_dpt_', 'span', 'divFlightFilter');
	for (var i = 0; i < aDepartTimeCounts.length; i++) {
		for (var j = 0; j < oFilters.DepartureTimes.length; j++) {
			int.f.SetHTML(aDepartTimeCounts[i], '0');
			if (oFilters.DepartureTimes[j].TimeOfDay == aDepartTimeCounts[i].id.split('_')[4]) {
				int.f.SetHTML(aDepartTimeCounts[i], int.n.SafeInt(oFilters.DepartureTimes[j].Count));
				break;
			};
		};
	};

	//return times
	var aReturnTimeCounts = int.f.GetObjectsByIDPrefix('spn_tod_count_rtn_', 'span', 'divFlightFilter');
	for (var i = 0; i < aReturnTimeCounts.length; i++) {
		for (var j = 0; j < oFilters.ReturnTimes.length; j++) {
			int.f.SetHTML(aReturnTimeCounts[i], '0');
			if (oFilters.ReturnTimes[j].TimeOfDay == aReturnTimeCounts[i].id.split('_')[4]) {
				int.f.SetHTML(aReturnTimeCounts[i], int.n.SafeInt(oFilters.ReturnTimes[j].Count));
				break;
			};
		};
	};

	//baggage included
	if (int.f.GetObject('chk_inclbag')) {
		int.f.SetHTML('spn_inclbag_count', oReturn.FlightsWithBaggage);
	}

}

FlightResultsFilter.RedrawFilters = function () {
	int.ff.Call('=iVectorWidgets.FlightResultsFilter.RedrawFilters', function (sHTML) { FlightResultsFilter.RedrawFiltersComplete(sHTML) });
}

FlightResultsFilter.RedrawFiltersComplete = function (sHTML) {
	int.f.SetHTML('divFlightFilter', sHTML, true);
	FlightResultsFilter.Setup(FlightResultsFilter.UpdateFunction);
}

FlightResultsFilter.ToggleSection = function (oHeader) {

	var sSectionID = int.s.Substring(oHeader.id, 1);

	var sIDWithPrefix = 'div' + sSectionID + '_Content'

	int.f.Toggle(sIDWithPrefix);

	if (int.f.HasClass(oHeader, 'hidden')) {
		int.f.RemoveClass(oHeader, 'hidden');
	}
	else {
		int.f.AddClass(oHeader, 'hidden');
	}
}


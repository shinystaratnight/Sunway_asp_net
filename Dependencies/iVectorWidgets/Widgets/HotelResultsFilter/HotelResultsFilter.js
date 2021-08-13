var HotelResultsFilter = HotelResultsFilter || {};

HotelResultsFilter.HotelNameTimer;
HotelResultsFilter.MapView = false;

HotelResultsFilter.HotelNameKeyUp = function () {
	var sHotelName = int.f.GetValue('txtFilterHotelName');
	if (HotelResultsFilter.HotelNameTimer) clearTimeout(HotelResultsFilter.HotelNameTimer);
	HotelResultsFilter.HotelNameTimer = setTimeout(HotelResultsFilter.Filter, 300);
}


//get filter checkboxes
HotelResultsFilter.GetFilterCheckboxes = function () {
	if (int.f.GetObject('divHotelFilter')) {
		return int.f.GetObjectsByIDPrefix('chk_', 'input', 'divHotelFilter');
	}
	else {
		return new Array();
	}
}


HotelResultsFilter.Setup = function () {

	var aSections = int.f.GetElementsByClassName('h4', 'sectionHeader', 'divHotelFilter');

	for (var i = 0; i < aSections.length; i++) {

		int.f.AttachEvent(
			aSections[i],
			'click',
			function () {
				HotelResultsFilter.ToggleSection(HotelResultsFilter());
			}
		);
	}

	//placeholder
	web.Placeholder.AttachEvents('txtFilterHotelName', int.f.GetValue('txtFilterHotelName_Placeholder'));


	//add events to filter checkboxes
	var aFilters = HotelResultsFilter.GetFilterCheckboxes();
	for (var i = 0; i < aFilters.length; i++) {

		if (int.s.StartsWith(aFilters[i].id, 'chk_l_')) {
			int.f.AttachEvent(aFilters[i], 'click', function () { HotelResultsFilter.ToggleLandmarks(this) });
		}
		else {
			int.f.AttachEvent(aFilters[i], 'click', HotelResultsFilter.Filter);
		}

	}

	//if we have a landmark filter, attach an event to the checkbox
	if (int.f.GetObject("ddlLandmarkDuration")) {
		int.f.AttachEvent("ddlLandmarkDuration", 'change', HotelResultsFilter.Filter);

	}

	if (int.f.GetObject('ddlLandmark')) {
		int.f.AttachEvent("ddlLandmark", 'change', HotelResultsFilter.Filter);
	}

};



HotelResultsFilter.SetBestSeller = function (bFilter) {
	int.f.SetValue('hidBestSeller', 'true');
	bFilter = (bFilter == undefined) ? true : bFilter;
	if (bFilter) HotelResultsFilter.Filter();
}



HotelResultsFilter.ClearBestSeller = function (bFilter) {
	int.f.SetValue('hidBestSeller', 'false');
	bFilter = (bFilter == undefined) ? true : bFilter;
	if (bFilter) HotelResultsFilter.Filter();
}


HotelResultsFilter.ToggleLandmarks = function (oLandmark) {

	if (!oLandmark.checked) {
		var oLabel = $('label[for=' + oLandmark.id + ']');
		oLabel.removeClass('selected');
	}
	
	var iLandmarkID = oLandmark.id.split('_')[2];
	var aOptions = int.f.GetObjectsByIDPrefix('chk_l', 'input', 'divHotelFilter');

	for (var i = 0; i < aOptions.length; i++) {
		if (!int.s.StartsWith(aOptions[i].id, 'chk_l_' + iLandmarkID)) {
			aOptions[i].checked = false;
			var oLabel = $('label[for=' + aOptions[i].id + ']');
			oLabel.removeClass('selected');
		}

		if (aOptions[i].checked) {
			var oLabel = $('label[for=' + aOptions[i].id + ']');
			oLabel.addClass('selected');
		}
	}

	HotelResultsFilter.Filter();
}


HotelResultsFilter.Filter = function () {

	if (!HotelResultsFilter.MapView) HotelResults.ShowLoadingMessage();

	var aStarRating = new Array();
	var aTARating = new Array();
	var aMealBasisID = new Array();
	var aFacilityFilterID = new Array();
	var aProductAttributeID = new Array();
	var aGeographyLevel3ID = new Array();
	var aPropertyTypeID = new Array();
	var iLandmarkID = 0;
    var aGeographyLevel2ID = new Array();

	var aFilters = HotelResultsFilter.GetFilterCheckboxes();

	for (var i = 0; i < aFilters.length; i++) {
		if (aFilters[i].checked) {
			iID = aFilters[i].id.split('_')[2];
			if (int.s.StartsWith(aFilters[i].id, 'chk_r_')) aStarRating.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_ta_')) aTARating.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_mb_')) aMealBasisID.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_f_')) aFacilityFilterID.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_pa_')) aProductAttributeID.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_g3_')) aGeographyLevel3ID.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_pt_')) aPropertyTypeID.push(iID);
			if (int.s.StartsWith(aFilters[i].id, 'chk_l_') && aFilters[i].id != 'chk_l_0') iLandmarkID = iID;
			if (int.s.StartsWith(aFilters[i].id, 'chk_ta_')) iMinScore = iID;
            if (int.s.StartsWith(aFilters[i].id, 'chk_g2_')) aGeographyLevel2ID.push(iID);
		}
	}

	var iMinPrice = int.f.GetValue('sldPrice_MinValue');
	var iMaxPrice = int.f.GetValue('sldPrice_MaxValue');
	var sHotelName = ((int.f.GetValue('txtFilterHotelName') != int.f.GetValue('txtFilterHotelName_Placeholder')) ? int.f.GetValue('txtFilterHotelName') : '');
	var bBestSeller = int.f.GetValue('hidBestSeller');

	//average review score
	var iMinScore = int.f.GetValue('sldAverageRating_MinValue');
	var iMaxScore = int.f.GetValue('sldAverageRating_MaxValue');

	//if we don't have a max score, set it correctly
	if (iMaxScore == 0) { iMaxScore = 10; };

	//Landmarks
	var iDistanceFromLandmark = 0;
	if (int.f.GetObject('sldLandmark')) {
		iDistanceFromLandmark = int.f.GetValue('sldLandmark_MaxValue');
	} else {
		iDistanceFromLandmark = int.f.GetValue('ddlLandmarkDuration');
	}
	
	if (int.f.GetObject('ddlLandmark')) {
		iLandmarkID = int.dd.GetValue('ddlLandmark');
	}

	if (int.f.GetObject('hidTripAdvisorTabValues')) {
	    var aTripAdvisorValues = int.f.GetValue('hidTripAdvisorTabValues').split(',');
	    if (aTripAdvisorValues.length !== 0 && aTripAdvisorValues[0] !== '') {
	        aTARating.length = 0;
	    }

	    for (var j = 0; j < aTripAdvisorValues.length; j++) {
	        aTARating.push(aTripAdvisorValues[j]);
	    }
	}

	int.ff.Call(
		'=iVectorWidgets.HotelResultsFilter.FilterResults', HotelResultsFilter.FilterComplete,
		int.s.ArrayToCSV(aStarRating), int.s.ArrayToCSV(aTARating), int.s.ArrayToCSV(aMealBasisID), int.s.ArrayToCSV(aFacilityFilterID),
		int.s.ArrayToCSV(aGeographyLevel3ID), int.s.ArrayToCSV(aProductAttributeID),
		iMinPrice, iMaxPrice, sHotelName, bBestSeller, iMinScore, iMaxScore, iLandmarkID, iDistanceFromLandmark,
		int.s.ArrayToCSV(aPropertyTypeID), int.s.ArrayToCSV(aGeographyLevel2ID)
	);
}


HotelResultsFilter.FilterComplete = function (sJSON) {

	var oFilters = eval('(' + sJSON + ')');

	HotelResultsFilter.UpdateFilterOptions(oFilters);

	if (!HotelResultsFilter.MapView) {
        if (typeof(SearchSummary) != 'undefined') SearchSummary.UpdatePaging();
        if (typeof (HotelResults) != 'undefined') HotelResults.UpdateResults();
        if (typeof (HotelResultsFooter) != 'undefined') HotelResultsFooter.Update();
	}
	else {
	    MapWidget.CreateMapFromFilteredHotels();
	}

    //May want to use setting, but fairly certain all our sites use Search summary and Hotel Filter together -JS
	if (typeof (SearchSummary) != 'undefined') SearchSummary.Update();

}

HotelResultsFilter.RedrawFilter = function () {
    int.ff.Call('=iVectorWidgets.HotelResultsFilter.Redraw', HotelResultsFilter.RedrawFilterComplete);
}

HotelResultsFilter.RedrawFilterComplete = function (sHtml) {
    int.f.SetHTML('divHotelFilter', sHtml, true);
    HotelResultsFilter.Setup();
    HotelResultsFilter.AttachEvents();
}

HotelResultsFilter.UpdateFilter = function () {
	int.ff.Call('=iVectorWidgets.HotelResultsFilter.UpdateFilter', HotelResultsFilter.UpdateFilterComplete);
}


HotelResultsFilter.UpdateFilterComplete = function (sJSON) {
	var oFilters = eval('(' + sJSON + ')');
	HotelResultsFilter.UpdateFilterOptions(oFilters);
}

HotelResultsFilter.UpdateFilterOptions = function (oFilters) {

	//price		
	var oCurrencySymbol = int.f.SafeObject('sldPrice_CurrencySymbol');
	var oCurrencySymbolPosition = int.f.SafeObject('sldPrice_CurrencySymbolPosition');

	var oMinPrice = int.f.SafeObject('sldPrice_MinValue');
	var oMaxPrice = int.f.SafeObject('sldPrice_MaxValue');

	var oPaxCount = int.f.SafeObject('sldPrice_DisplayValueDivide');
	var iPaxCount = oPaxCount == null ? 1 : oPaxCount.value;

	if (oMinPrice != null && oMaxPrice != null && oCurrencySymbol != null && oCurrencySymbolPosition != null) {
		var sMinPriceFormatted = int.n.FormatMoney(Math.floor(oMinPrice.value / iPaxCount), oCurrencySymbol.value, oCurrencySymbolPosition.value);
		sMinPriceFormatted = sMinPriceFormatted.substring(0, sMinPriceFormatted.indexOf('.'));

		var sMaxPriceFormatted = int.n.FormatMoney(Math.floor(oMaxPrice.value / iPaxCount), oCurrencySymbol.value, oCurrencySymbolPosition.value);
		sMaxPriceFormatted = sMaxPriceFormatted.substring(0, sMaxPriceFormatted.indexOf('.'));

		var sPerPerson = oPaxCount == null ? '' : 'pp';

		int.f.SetHTML('sldPrice_Start', '<span>' + sMinPriceFormatted + sPerPerson + '</span>');
		int.f.SetHTML('sldPrice_End', '<span>' + sMaxPriceFormatted + sPerPerson + '</span>');
	}


	//rating
	var aRatingCounts = int.f.GetObjectsByIDPrefix('spn_r_count_', 'span', 'divHotelFilter');
	for (var i = 0; i < aRatingCounts.length; i++) {
		for (var j = 0; j < oFilters.Ratings.length; j++) {
			int.f.SetHTML(aRatingCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.Ratings[j].Rating) == int.n.SafeNumeric(aRatingCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aRatingCounts[i], int.n.SafeInt(oFilters.Ratings[j].Count));
				break;
			};
		};
	};

   //regions
	var aRegionPrices = int.f.GetObjectsByIDPrefix('spn_g2_price_', 'span', 'divHotelFilter');
	for (var i = 0; i < aRegionPrices.length; i++) {
	    var nPrice = 0;
	    var iRegionID = int.n.SafeNumeric(aRegionPrices[i].id.split('_')[3]);
		for (var j = 0; j < oFilters.Regions.length; j++) {
			if (int.n.SafeNumeric(oFilters.Regions[j].GeographyLevel2ID) === iRegionID) {
				nPrice = int.n.FormatCommas(oFilters.Regions[j].FromPrice.toFixed(0));
				break;
			};
		};
        int.f.SetHTML(aRegionPrices[i], nPrice);
	};

	//resorts
	var aResortCounts = int.f.GetObjectsByIDPrefix('spn_g3_count_', 'span', 'divHotelFilter');
	for (var i = 0; i < aResortCounts.length; i++) {
		for (var j = 0; j < oFilters.Resorts.length; j++) {
			int.f.SetHTML(aResortCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.Resorts[j].GeographyLevel3ID) == int.n.SafeNumeric(aResortCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aResortCounts[i], int.n.SafeInt(oFilters.Resorts[j].Count));
				break;
			};
		};
	};

	// Meal Basis
	var aMealBasisCounts = int.f.GetObjectsByIDPrefix('spn_mb_count_', 'span', 'divHotelFilter');
	for (var i = 0; i < aMealBasisCounts.length; i++) {
		for (var j = 0; j < oFilters.MealBases.length; j++) {
			int.f.SetHTML(aMealBasisCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.MealBases[j].MealBasisID) == int.n.SafeNumeric(aMealBasisCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aMealBasisCounts[i], int.n.SafeInt(oFilters.MealBases[j].Count));
				break;
			};
		};
	};

	// Facilities
	var aFilterFacilityCounts = int.f.GetObjectsByIDPrefix('spn_f_count_', 'span', 'divHotelFilter');
	for (var i = 0; i < aFilterFacilityCounts.length; i++) {
		for (var j = 0; j < oFilters.FilterFacilities.length; j++) {
			int.f.SetHTML(aFilterFacilityCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.FilterFacilities[j].Priority) == int.n.SafeNumeric(aFilterFacilityCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aFilterFacilityCounts[i], int.n.SafeInt(oFilters.FilterFacilities[j].Count));
				break;
			};
		};
	};

	// Trip Advisor
	var aTripAdvisorCounts = int.f.GetObjectsByIDPrefix('spn_ta_count_', 'span', 'divHotelFilter');
	var bUseTripAdvisorValuesOnly = false;
    var aTripAdvisorValues;
    if (int.f.GetObject('hidTripAdvisorTabValues')) {
        aTripAdvisorValues = int.f.GetValue('hidTripAdvisorTabValues').split(',');
        if (aTripAdvisorValues.length !== 0 && aTripAdvisorValues[0] !== '') {
            bUseTripAdvisorValuesOnly = true;
        }
    }

    for (var i = 0; i < aTripAdvisorCounts.length; i++) {
        var iTripAdvisorCount = int.n.SafeNumeric(aTripAdvisorCounts[i].id.split('_')[3]);
        for (var j = 0; j < oFilters.TripAdvisorRatings.TripAdvisorRatings.length; j++) {
            int.f.SetHTML(aTripAdvisorCounts[i], '0');
            var iTripAdvisorRating = int.n.SafeNumeric(oFilters.TripAdvisorRatings.TripAdvisorRatings[j].Rating);

            if (iTripAdvisorRating === iTripAdvisorCount) {
                var bTAMatch = bUseTripAdvisorValuesOnly && aTripAdvisorValues.indexOf(iTripAdvisorCount.toString()) !== -1;
                int.f.ShowIf(aTripAdvisorCounts[i].parentNode, (bTAMatch || !bUseTripAdvisorValuesOnly));
                if (!bUseTripAdvisorValuesOnly || bTAMatch) {
                    int.f.SetHTML(aTripAdvisorCounts[i], int.n.SafeInt(oFilters.TripAdvisorRatings.TripAdvisorRatings[j].Count));
                    break;
                }
                
            };
        };
    };

    // Attributes
	var aProductAttributesCounts = int.f.GetObjectsByIDPrefix('spn_pa_count_', 'span', 'divHotelFilter');
	for (var i = 0; i < aProductAttributesCounts.length; i++) {
		for (var j = 0; j < oFilters.ProductAttributes.length; j++) {
			int.f.SetHTML(aProductAttributesCounts[i], '0');
			if (int.n.SafeNumeric(oFilters.ProductAttributes[j].ProductAttributeID) == int.n.SafeNumeric(aProductAttributesCounts[i].id.split('_')[3])) {
				int.f.SetHTML(aProductAttributesCounts[i], int.n.SafeInt(oFilters.ProductAttributes[j].Count));
				break;
			};
		};
	};

};



HotelResultsFilter.ToggleSection = function (oHeader) {

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
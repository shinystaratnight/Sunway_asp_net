var SearchTool = SearchTool || {};

var me = SearchTool;

SearchTool.StartMode;
SearchTool.ExpandAction; /* Show, Popup */
SearchTool.InitialRender = true;

SearchTool.Timeout;
SearchTool.SuppressResults = false;
SearchTool.AdvancedOptionsShown = false;

SearchTool.PreSearchFunction = null;
SearchTool.AdditionalValidationFunction = null;

SearchTool.ShowDatePickerDropDowns;
SearchTool.InsertWarningAfter = "";

SearchTool.CustomCallBacks = [];

SearchTool.DefaultValues = {};

/*

1. Setup
2. Bind
3. Show
4. Search
5. Unbind
6. Expand
7. Clear

x. Support
x.1 PopulateDropdownRanges

*/

//1. Setup
SearchTool.Setup = function () {
	me.DefaultValues = JSON.parse(int.f.GetValue('hidDefaultSearchValues'));

	me.StartMode = int.f.GetValue('hidStartMode');
	me.ExpandAction = int.f.GetValue('hidExpandAction');
	me.ShowDatePickerDropDowns = int.f.GetValue('hidShowDatePickerDropDowns');

	me.Support.PopulateDropdownRanges();
	me.Bind();
	me.Show();
	me.InsertWarningAfter = int.f.GetValue('hidInsertWarningAfter');

	web.PubSub.publish('SearchTool.Setup');
}

//2. Bind
SearchTool.Bind = function () {
	var aCookiePairs = me.Support.GetCookie();
	var SearchValues = me.CombineDefaultValuesWithCookie(aCookiePairs, me.DefaultValues);

	//Set things depending on search mode
	int.f.SetValue('hidSearchMode', SearchValues.SearchMode);
	me.Support.SetSearchMode(SearchValues.SearchMode);
	int.f.AddClass('divSearch', SearchValues.SearchMode);

	//sets the default tab as selected
	if (int.f.GetObject('li_SearchMode_' + SearchValues.SearchMode)) {
		int.f.AddClass('li_SearchMode_' + SearchValues.SearchMode, 'selected');
	}

	int.f.ShowIf('divArrivingAtAirport', SearchValues.SearchMode == 'FlightOnly');

	if (SearchValues.SearchMode == 'TransferOnly') {
		//set transfer only as selected
		int.f.AddClass('li_SearchMode_TransferOnly', 'selected');

		//We need to set the values of the corrisponding hidden inputs, as well as get the correct names associated with those id's for the text boxes.
		int.f.SetValue('hidTransferPickupLocationID', SearchValues.TransferPickupLocationID);
		int.f.SetValue('ddlPickupType', SearchValues.PickupType);
		if (SearchValues.PickupType == 'Airport') {
			SearchTool.Support.SetAirportName(SearchValues.TransferPickupLocationID, int.f.GetObject('acpAirportIDTransferPickup'))
			int.f.SetValue('acpAirportIDTransferPickupHidden', SearchValues.TransferPickupLocationID);
		}
		else {
			SearchTool.Support.SetPropertyName(SearchValues.TransferPickupLocationID, int.f.GetObject('acpPropertyIDTransferPickup'))
			int.f.SetValue('acpPropertyIDTransferPickupHidden', SearchValues.TransferPickupLocationID);
		}

		int.f.SetValue('hidTransferDropOffLocationID', SearchValues.TransferDropOffLocationID);
		int.f.SetValue('ddlDropOffType', SearchValues.DropOffType);
		if (SearchValues.DropOffType == 'Airport') {
			SearchTool.Support.SetAirportName(SearchValues.TransferDropOffLocationID, int.f.GetObject('acpAirportIDTransferDropOff'))
			int.f.SetValue('acpAirportIDTransferDropOffHidden', SearchValues.TransferDropOffLocationID);
		}
		else {
			SearchTool.Support.SetPropertyName(SearchValues.TransferDropOffLocationID, int.f.GetObject('acpPropertyIDTransferDropOff'))
			int.f.SetValue('acpPropertyIDTransferDropOffHidden', SearchValues.TransferDropOffLocationID);
		}

		//check return transfer checkbox if we're in transfer mode, hide return fields if we're in transfer mode but not with return journey
		int.cb.SetValue('cbReturnTransfer', SearchValues.ReturnTransfer);
		int.f.SetValue('hidReturnTransfer', SearchValues.ReturnTransfer);
		if (SearchValues.ReturnTransfer == 'True') {
			int.f.AddClass('lblReturnTransfer', 'selected');
		}
		else {
			int.f.Hide('fldDropOffDate');
		}
	}

	if (SearchValues.SearchMode == 'AirportHotel') {
		SearchTool.Override.SetAirportHotelDetails();
	};

	//Sets the departure airport
	if (SearchValues.DepartingFromID != 0) {
		int.f.SetValue('hidAirportDepartingFromID', SearchValues.DepartingFromID);
		int.dd.SetValue('ddlDepartingFromID', SearchValues.DepartingFromID);

		if (int.f.GetObject('acpDepartingFromID')) SearchTool.Support.SetDepartingFromName();
	}

	//Sets the holiday destination
	if (SearchValues.ArrivingAtID != 0) {
		int.f.SetValue('hidArrivingAtID', SearchValues.ArrivingAtID);
		SearchTool.Support.ArrivingAtDropdowns();
		SearchTool.Support.ArrivalAirportDropdown();
		if (int.f.GetObject('acpArrivingAtID')) SearchTool.Support.SetArrivingAtName();
		if (int.f.GetObject('ddlArrivalAirportID') && SearchValues.ArrivingAtID > 1000000) {
			int.dd.SetValue('ddlArrivalAirportID', SearchValues.ArrivingAtID - 1000000);
		} else if (int.f.GetObject('ddlRegionID') && SearchValues.ArrivingAtID > 0) {
			int.dd.SetValue('ddlRegionID', SearchValues.ArrivingAtID);
		} else if (int.f.GetObject('ddlResortID') && SearchValues.ArrivingAtID < 0) {
			int.dd.SetValue('ddlResortID', SearchValues.ArrivingAtID);
		}
	}

    if (document.getElementById('acpHotelOnlyArrivingAtID')){
        //Sets the hotel destination
        if (SearchValues.ArrivingAtID != 0) {
            int.f.SetValue('acpPriorityPropertyIDHidden', SearchValues.PriorityPropertyID);
            SearchTool.Support.ArrivingAtDropdowns();
            if (int.f.GetObject('acpHotelOnlyArrivingAtID')) SearchTool.HotelOnlySupport.HotelArrivingAtName();
        }
    }

	// Sets the departure date
	if (SearchValues.DepartureDate != '01/01/1900') {
		int.f.SetValue('txtDepartureDate', SearchValues.DepartureDate);
	}

	//outbound time
	int.f.SetValue('txtArrivalTime', SearchValues.ArrivalTime);

	//outbound date
	int.f.SetValue('txtPickupDate', SearchValues.PickupDate);

	//return time
	int.f.SetValue('txtDropOffTime', SearchValues.DropOffTime);

	//return date (add duration to departure date if not on cookie)
	var dDropOffDate = SearchValues.DropOffDate;
	if (dDropOffDate == undefined || (int.d.New(SearchValues.DropOffDate) < int.d.New(SearchValues.PickupDate))) {
		var dDepartureDate = int.d.New(SearchValues.DepartureDate.split('/')[0], SearchValues.DepartureDate.split('/')[1], SearchValues.DepartureDate.split('/')[2]);
		dDropOffDate = int.d.AddDays(dDepartureDate, SearchValues.Duration);
	}
	dDropOffDate = ((dDropOffDate instanceof Date) ? int.d.DatepickerDate(dDropOffDate) : dDropOffDate); //convert to string if date
	int.f.SetValue('txtDropOffDate', dDropOffDate);

	//duration
	var oDropdown = int.f.GetObject('ddlDuration');
	var bDropdownContainsDuration = false;
	for (var i = 0; i < oDropdown.options.length; i++) {
	    if (oDropdown.options[i].text === SearchValues.Duration) {
	        bDropdownContainsDuration = true;
	    }
	}
	if (!bDropdownContainsDuration) {
	    int.dd.AddOption(oDropdown, SearchValues.Duration, SearchValues.Duration, 'hide');
	}

	int.dd.SetValue('ddlDuration', SearchValues.Duration);

	//rooms
	int.dd.SetValue('ddlRooms', SearchValues.Rooms);

	//pax
	for (var i = 0; i <= 2; i++) {
		//adults
		int.dd.SetValue('ddlAdults_' + (i + 1), SearchValues.RoomPassengers[i].Adults);

		//children
		int.dd.SetValue('ddlChildren_' + (i + 1), SearchValues.RoomPassengers[i].Children);

		//child ages
		int.f.ShowIf('trAges_' + (i + 1), SearchValues.RoomPassengers[i].Children > 0);
		for (var j = 1; j <= 4; j++) {
			int.dd.SetValue('ddlChildAge_' + (i + 1) + '_' + j, SearchValues.RoomPassengers[i].ChildAges.split('#')[j - 1]);
			int.f.ShowIf('ddlChildAge_' + (i + 1) + '_' + j, j <= SearchValues.RoomPassengers[i].Children);
		}

		//infants
		int.dd.SetValue('ddlInfants_' + (i + 1), SearchValues.RoomPassengers[i].Infants);
	}

	//advanced search
	int.dd.SetValue('ddlMealBasisID', SearchValues.MealBasisID);

	if (SearchValues.Rating == 5) int.cb.SetValue('cb5Star', 'true');
	if (SearchValues.Rating == 4) int.cb.SetValue('cb4Star', 'true');
	if (SearchValues.Rating == 3) int.cb.SetValue('cb3Star', 'true');

	if (SearchValues.MealBasisID > 0 || SearchValues.Rating > 0) SearchTool.Support.ToggleAdvancedOptions();

	//date picker
	var iDatepickerMonths = int.f.GetIntValue('hidDatepickerMonths');
	var iDatepickerFirstDay = int.f.GetIntValue('hidDatepickerFirstDay');

	// only come in here if we are displaying the return datepicker
	//this will be off by default and controlled by a page def setting -CM 24/04/2014
	var bUseReturnDatepicker = int.f.GetValue('hidReturnDatepicker');

	//Boolean set up to decide whether we want to use short or full dates in datepicker months - JS
	var bUseShortDates = int.f.GetValue('hidUseShortDates');

	//If we have a duration on the cookie use that if not add 7 days
	var dReturnDate;
	var dReturnMinDate;
	var dStartDate = new Date;

	var iDaysAhead = int.f.GetObject('hidDaysAhead') ? int.f.GetValue('hidDaysAhead') : 0;
	var iReturnMinDaysAhead = int.f.GetObject('hidReturnMinDaysAhead') ? int.f.GetValue('hidReturnMinDaysAhead') : 1;

	//if we have a defined start date, work out the dates based on this
	if (int.f.GetObject('hidDefaultSearchDate')) {
		var sDefaultSearchDate = int.f.GetValue('hidDefaultSearchDate');

		dReturnDate = int.d.AddDays(int.d.New(sDefaultSearchDate.split('/')[0], sDefaultSearchDate.split('/')[1], sDefaultSearchDate.split('/')[2]), SearchValues.Duration == '' ? 7 : SearchValues.Duration);

		dReturnMinDate = int.d.AddDays(int.d.New(sDefaultSearchDate.split('/')[0], sDefaultSearchDate.split('/')[1], sDefaultSearchDate.split('/')[2]), iReturnMinDaysAhead);
		dStartDate = int.d.New(sDefaultSearchDate.split('/')[0], sDefaultSearchDate.split('/')[1], sDefaultSearchDate.split('/')[2])

		int.f.SetValue('txtDepartureDate', int.d.DatepickerDate(dStartDate));
		int.f.SetValue('txtReturnDate', int.d.DatepickerDate(dReturnDate));
	}
	else {
		dReturnDate = int.d.AddDays(int.d.New(SearchValues.DepartureDate.split('/')[0], SearchValues.DepartureDate.split('/')[1], SearchValues.DepartureDate.split('/')[2]), SearchValues.Duration == '' ? 7 : SearchValues.Duration);

		dReturnMinDate = int.d.AddDays(int.d.New(SearchValues.DepartureDate.split('/')[0], SearchValues.DepartureDate.split('/')[1], SearchValues.DepartureDate.split('/')[2]), iReturnMinDaysAhead);
		int.f.SetValue('txtReturnDate', int.d.DatepickerDate(dReturnDate));

		dStartDate = int.d.AddDays(dStartDate, iDaysAhead);
	}

	var datepickerShowOn = int.f.GetValue('hidSearchDatePickerShowOn');
	var datepickerButtonText = int.f.GetValue('hidSearchDatePickerButtonText');
	var datepickerCultureCode = int.f.GetValue('hidDatepickerCultureCode');

	web.DatePicker.Setup('#txtDepartureDate', dStartDate, SearchValues.DepartureDate, iDatepickerMonths, iDatepickerFirstDay, '', SearchTool.UpdateArrivalDate, 'departureDate',
		me.ShowDatePickerDropDowns, '', '', '', '', '', bUseShortDates, 0, datepickerShowOn, datepickerButtonText, '', datepickerCultureCode);

	web.DatePicker.Setup('#txtReturnDate', dReturnMinDate, int.d.DatepickerDate(dReturnDate), iDatepickerMonths, iDatepickerFirstDay, '', SearchTool.UpdateDuration, 'returnDate',
		me.ShowDatePickerDropDowns, '', '', '', '', '', bUseShortDates, 0, datepickerShowOn, datepickerButtonText, '', datepickerCultureCode);

	web.DatePicker.Setup('#txtPickupDate', new Date, SearchValues.PickupDate, iDatepickerMonths, iDatepickerFirstDay, '', '', 'pickupDate', me.ShowDatePickerDropDowns, '', '', '', '', '', bUseShortDates,
		0, datepickerShowOn, datepickerButtonText, '', datepickerCultureCode);

	web.DatePicker.Setup('#txtDropOffDate', new Date, dDropOffDate, iDatepickerMonths, iDatepickerFirstDay, '', '', 'dropOffDate', me.ShowDatePickerDropDowns, '', '', '', '', '', bUseShortDates,
		0, datepickerShowOn, datepickerButtonText, '', datepickerCultureCode);

	if (int.f.GetObject('lblHighRatedHotelFilter') && int.f.GetObject('chkHighRatedHotelFilter')) {
		int.f.SetClassIf('lblHighRatedHotelFilter', 'selected',
			SearchValues.HighRatedHotelFilter === 'true' || SearchValues.HighRatedHotelFilter === true);
		int.cb.SetValue('chkHighRatedHotelFilter', SearchValues.HighRatedHotelFilter === 'true' || SearchValues.HighRatedHotelFilter === true);
	}
}

SearchTool.UpdateDuration = function () {
	var sStartDate = int.f.GetValue('txtDepartureDate');
	var sEndDate = int.f.GetValue('txtReturnDate');

	var sFormattedStartDate = int.d.New(sStartDate.split('/')[0], sStartDate.split('/')[1], sStartDate.split('/')[2]);
	var sFormattedEndDate = int.d.New(sEndDate.split('/')[0], sEndDate.split('/')[1], sEndDate.split('/')[2]);

	var iDuration = int.d.DateDiff(sFormattedStartDate, sFormattedEndDate);

	var oDropdown = int.f.GetObject('ddlDuration');

	var bDropdownContainsDuration = false;

	for (var i = 0; i < oDropdown.options.length; i++) {
		if (oDropdown.options[i].text == iDuration) {
			bDropdownContainsDuration = true;
		}
	}

	if (!bDropdownContainsDuration) {
		int.dd.AddOption(oDropdown, iDuration, iDuration, 'hide');
	}

	int.dd.SetValue('ddlDuration', iDuration);
}

SearchTool.UpdateArrivalDate = function () {
	var sDepartureDate = int.f.GetValue('txtDepartureDate');
	var dDepartureDate = int.d.New(sDepartureDate.split('/')[0], sDepartureDate.split('/')[1], sDepartureDate.split('/')[2]);

	//Use the Min Days ahead if its set
	var iReturnMinDaysAhead = int.f.GetObject('hidReturnMinDaysAhead') ? int.f.GetValue('hidReturnMinDaysAhead') : 1;
	var dMinReturn = new Date(int.d.AddDays(dDepartureDate, iReturnMinDaysAhead))
	var dReturnDate = new Date(int.d.AddDays(dDepartureDate, 6))

	$("#txtReturnDate").datepicker("option", "minDate", dMinReturn);

	$("#txtReturnDate").datepicker("setDate", dReturnDate);
	SearchTool.UpdateDuration();
}

//3. Show
SearchTool.Show = function () {
	web.PubSub.publish('SearchTool.Show');

	//start mode
	if (me.InitialRender) {
		int.f.ShowIf('divSearch', me.StartMode == 'SearchTool');
		int.f.ShowIf('divSearchAgain', me.StartMode == 'Collapsed');
	}
	me.InitialRender = false;

	//search modes
	var sSearchMode = int.f.GetValue('hidSearchMode');

	int.f.ShowIf('fldDeparting', sSearchMode == 'FlightPlusHotel' || sSearchMode == 'FlightOnly' || sSearchMode == 'Anywhere');

	int.f.ShowIf('divSearch_Transfers', sSearchMode == 'TransferOnly');

	int.f.ShowIf('divSearch_CarHire', sSearchMode == 'CarHireOnly');

	int.f.ShowIf('divSearch_Where', sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly');
	int.f.ShowIf('divSearch_When', sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly');
	int.f.ShowIf('divSearch_Rooms', sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly');

	if (sSearchMode === 'TransferOnly') {
		SearchTool.Support.TransferDropDowns(int.f.GetObject('ddlPickupType'));
		SearchTool.Support.TransferDropDowns(int.f.GetObject('ddlDropOffType'));

		int.f.Hide('aShowAdvancedOptions');
	}

	if (sSearchMode === 'CarHireOnly') {
		int.f.Hide('aShowAdvancedOptions');
	}

	if (int.f.GetObject('fldPriorityProperty')) {
		int.f.ShowIf('fldPriorityProperty', sSearchMode === 'FlightPlusHotel' || sSearchMode === 'HotelOnly');
		int.f.EnableIf('acpPriorityPropertyID', int.f.GetValue('hidArrivingAtID') !== 0);
	}

	//rooms
	var iRooms = int.dd.GetIntValue('ddlRooms');
	var sGuestPrefix = 'tr';

	if (int.f.GetValue('hidGuestPrefix') != "") {
		sGuestPrefix = int.f.GetValue('hidGuestPrefix');
	};

	for (var i = 1; i <= 3; i++) {
		int.f.ShowIf(sGuestPrefix + 'Guests_' + i, i <= iRooms);

		var iChildren = int.dd.GetIntValue('ddlChildren_' + i);

		int.f.ShowIf(sGuestPrefix + 'Ages_' + i, iChildren > 0 && i <= iRooms);

		for (var j = 1; j <= 4; j++) {
			int.f.ShowIf('ddlChildAge_' + i + '_' + j, j <= iChildren)
		}
	}
}

//4. Search
SearchTool.Search = function () {
	//hide any warnings
	web.InfoBox.Close();

	//clear timeout reset suppress
	clearTimeout(SearchTool.Timeout);
	SearchTool.SuppressResults = false;

	if (SearchTool.PreSearchFunction != null) {
		try {
			SearchTool.PreSearchFunction();
		}
		catch (exc) {
			if (exc.exit === true) {
				return;
			}
			//if (console != undefined && log in console) console.log('PreSearchFunctionError:- ' + exc);
		}
	}

	//To avoid messy logic in BookingSearch.VB, only use one hidden input for each Transfer fieldset
	SearchTool.Support.SetTransferParentID();

	//validate
	var sSearchMode = int.f.GetValue('hidSearchMode');

	var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divSearch');

	for (var i = 0; i < aErrorControls.length; i++) {
		int.f.RemoveClass(aErrorControls[i], 'error');
	}

	//duration - don't validate if one-way flight or carhire search
	var bValidateDuration = true;
	if (int.f.GetObject('rad_OneWay')) {
		if (int.cb.Checked('rad_OneWay') && sSearchMode == 'FlightOnly' || sSearchMode == 'CarHireOnly') {
			bValidateDuration = false
		}
	}

	if (bValidateDuration) {
		if (int.f.GetObject('ddlDuration')) int.f.SetClassIf('ddlDuration', 'error', int.dd.GetValue('ddlDuration') == -1);
		if (int.f.GetObject('txtReturnDate')) int.f.SetClassIf('txtReturnDate', 'error', int.dd.GetValue('ddlDuration') == -1);
	}

	//departing from
	if (sSearchMode === 'FlightPlusHotel' || sSearchMode === 'FlightOnly' || sSearchMode === 'Anywhere') {
		if (int.f.GetObject('acpDepartingFromID')) int.f.SetClassIf('acpDepartingFromID', 'error', int.f.GetIntValue('hidAirportDepartingFromID') == 0);
		if (int.f.GetObject('ddlDepartingFromID')) int.f.SetClassIf(SearchTool.CustomSelectContainer('ddlDepartingFromID'), 'error', int.f.GetIntValue('hidAirportDepartingFromID') == 0);
		web.PubSub.publish('SearchTool.FlightOnlyValidate');
	}

	//arriving at
	if (sSearchMode == 'FlightPlusHotel' || sSearchMode == 'HotelOnly') {

        if (int.f.GetValue('hidTripType') === 'flightitinerary') {
            for (var count = 1; count <= int.f.GetIntValue('hidFlightSectors') ; count++) {
                    int.f.SetClassIf(SearchTool.CustomSelectContainer('ddlFlightItineraryArrival_' + count),
				        'error',
				        int.f.GetIntValue('hidFIArrID_' + count) == 0);
                    int.f.SetClassIf(int.f.GetObject('acpDepartingFromID_' + count),
                        'error',
                        int.f.GetIntValue('hidFIArrID_' + count) == 0);
            }
        } else {
		if (int.f.GetObject('acpArrivingAtID')) int.f.SetClassIf('acpArrivingAtID', 'error', int.f.GetIntValue('hidArrivingAtID') == 0);
		if (int.f.GetObject('ddlRegionID')) int.f.SetClassIf(SearchTool.CustomSelectContainer('ddlRegionID'), 'error', int.f.GetIntValue('hidArrivingAtID') == 0);
        }
	} else if (sSearchMode === 'Anywhere') {
		var iArrivatingAtID = int.f.GetIntValue('hidArrivingAtID');

		var iProductAttributeID = 0;
		if (int.f.GetObject('ddlProductAttributeID')) {
			iProductAttributeID = int.dd.GetValue('ddlProductAttributeID');
		} else if (int.f.GetObject('hidProductAttributeID')) {
			iProductAttributeID = int.dd.GetValue('hidProductAttributeID');
		}

		if (int.f.GetObject('ddlProductAttributeID')) {
			int.f.SetClassIf(SearchTool.CustomSelectContainer('ddlProductAttributeID'), 'error', iProductAttributeID === 0);
		} else if (int.f.GetObject('acpArrivingAtID')) {
			int.f.SetClassIf('acpArrivingAtID', 'error', iArrivatingAtID === 0 && iProductAttributeID === 0);
		}
	}

	if (sSearchMode == 'FlightOnly') {
		if (int.f.GetObject('acpArrivingAtAirportID')) int.f.SetClassIf('acpArrivingAtAirportID', 'error', int.f.GetIntValue('hidArrivingAtID') <= 0);
		if (int.f.GetObject('ddlArrivalAirportID')) int.f.SetClassIf('acpArrivingAtAirportID', 'error', int.f.GetIntValue('hidArrivingAtID') <= 0);
	}

	if (sSearchMode != "TransferOnly" && sSearchMode != "CarHireOnly") {
		//date
		int.f.SetClassIf('txtDepartureDate', 'error', int.f.GetValue('txtDepartureDate') == '');
	}

	//child ages
	if (sSearchMode != "CarHireOnly" || sSearchMode !== 'Multi') {
		var iRooms = int.dd.GetValue('ddlRooms');
		for (var i = 1; i <= iRooms; i++) {
			var iChildren = int.dd.GetIntValue('ddlChildren_' + i);
			for (var j = 1; j <= iChildren; j++) {
				int.f.SetClassIf('ddlChildAge_' + i + '_' + j, 'error', int.dd.GetIntValue('ddlChildAge_' + i + '_' + j) == 0);
			}
		}
	}

	if (sSearchMode == "TransferOnly") {
		//Validate Pickup Drop down and auto complete inputs
		if (int.f.GetObject('acpAirportIDTransferPickup')) int.f.SetClassIf('acpAirportIDTransferPickup', 'error', int.f.GetValue('acpAirportIDTransferPickupHidden') == 0 && int.f.GetValue('ddlPickupType') == 'Airport');

		if (int.f.GetObject('acpPropertyIDTransferPickup')) int.f.SetClassIf('acpPropertyIDTransferPickup', 'error', int.f.GetValue('acpPropertyIDTransferPickupHidden') == 0 && int.f.GetValue('ddlPickupType') == 'Property');

		int.f.SetClassIf('ddlPickupType', 'error', int.f.GetValue('ddlPickupType') != 'Airport' && int.f.GetValue('ddlPickupType') != 'Property');

		//Validate DropOff Drop down and auto complete inputs
		if (int.f.GetObject('acpAirportIDTransferDropOff')) int.f.SetClassIf('acpAirportIDTransferDropOff', 'error', int.f.GetValue('acpAirportIDTransferDropOffHidden') == 0 && int.f.GetValue('ddlDropOffType') == 'Airport');

		if (int.f.GetObject('acpPropertyIDTransferDropOff')) int.f.SetClassIf('acpPropertyIDTransferDropOff', 'error', int.f.GetValue('acpPropertyIDTransferDropOffHidden') == 0 && int.f.GetValue('ddlDropOffType') == 'Property');

		int.f.SetClassIf('ddlDropOffType', 'error', int.f.GetValue('ddlDropOffType') != 'Airport' && int.f.GetValue('ddlDropOffType') != 'Property');

		//Transfer dates

		int.f.SetClassIf('txtPickupDate', 'error', int.f.GetValue('txtPickupDate') == '');
		int.f.SetClassIf('txtArrivalTime', 'error', int.f.GetValue('txtArrivalTime') == '');

		int.f.SetClassIf('txtDropOffDate', 'error', int.f.GetValue('txtDropOffDate') == '' && int.cb.Checked('cbReturnTransfer'));
		int.f.SetClassIf('txtDropOffTime', 'error', int.f.GetValue('txtDropOffTime') == '' && int.cb.Checked('cbReturnTransfer'));
	}

	if (sSearchMode == 'CarHireOnly') {
		//Validate country
        if (int.f.GetObject('ddlCarHireCountryID')) {
            if (int.f.GetObject('acpCarHireCountryID')) {
                var bCountryNotSet = int.f.GetValue('acpCarHireCountryIDHidden') == 0 && int.dd.GetValue('ddlCarHireCountryID') == 0;
                int.f.SetClassIf('acpCarHireCountryID', 'error', bCountryNotSet);
                int.f.SetClassIf('ddlCarHireCountryID', 'error', bCountryNotSet);
            } else {
                int.f.SetClassIf('ddlCarHireCountryID', 'error', int.dd.GetValue('ddlCarHireCountryID') == 0);
            }
        } else if (int.f.GetObject('acpCarHireCountryID')) {
            int.f.SetClassIf('acpCarHireCountryID', 'error', int.f.GetValue('acpCarHireCountryIDHidden') == 0);
        }

		//validate pickup location
		if (int.f.GetObject('ddlCarHirePickUpDepotID')) int.f.SetClassIf('ddlCarHirePickUpDepotID', 'error', int.f.GetIntValue('hidCarHirePickUpDepotID') == 0);
		if (int.f.GetObject('acpCarHirePickUpDepotID')) int.f.SetClassIf('acpCarHirePickUpDepotID', 'error', int.f.GetIntValue('hidCarHirePickUpDepotID') == 0);

		//validate pick date
		if (int.f.GetObject('txtCarHirePickUpDate')) int.f.SetClassIf('txtCarHirePickUpDate', 'error',
			int.f.GetValue('txtCarHirePickUpDate') == ''
			|| int.v.IsValidDate(int.f.GetValue('txtCarHirePickUpDate')) == false);

		//validate pick time
		if (int.f.GetObject('txtCarHirePickUpTime')) int.f.SetClassIf('txtCarHirePickUpTime', 'error', int.f.GetValue('txtCarHirePickUpTime') == 0 || int.v.IsTime(int.f.GetValue('txtCarHirePickUpTime')) == false);

		//validate drop off location
		if (int.f.GetObject('ddlCarHireDropOffDepotID')) int.f.SetClassIf('ddlCarHireDropOffDepotID', 'error', int.f.GetIntValue('hidCarHireDropOffDepotID') == 0);
		if (int.f.GetObject('acpCarHireDropOffDepotID')) int.f.SetClassIf('acpCarHireDropOffDepotID', 'error', int.f.GetIntValue('hidCarHireDropOffDepotID') == 0);

		//validate drop off date
		if (int.f.GetObject('txtCarHireDropOffDate')) int.f.SetClassIf('txtCarHireDropOffDate', 'error',
			int.f.GetValue('txtCarHireDropOffDate') == ''
			|| int.v.IsValidDate(int.f.GetValue('txtCarHireDropOffDate')) == false);

		//validate drop off time
		if (int.f.GetObject('txtCarHireDropOffTime')) int.f.SetClassIf('txtCarHireDropOffTime', 'error', int.f.GetValue('txtCarHireDropOffTime') == '' || int.v.IsTime(int.f.GetValue('txtCarHireDropOffTime')) == false);

		//validate the pick up date is before the drop off date
		var aPickUpDate = int.f.GetValue('txtCarHirePickUpDate').split('/');
		var aDropOffDate = int.f.GetValue('txtCarHireDropOffDate').split('/');
		var PickUpDate = new Date(aPickUpDate[2], aPickUpDate[1], aPickUpDate[0]);
		var DropOffDate = new Date(aDropOffDate[2], aDropOffDate[1], aDropOffDate[0]);
		int.f.SetClassIf('txtCarHireDropOffDate', 'error', DropOffDate < PickUpDate);

		//validate driver country of residence
		if (int.f.GetObject('ddlCarHireDriverBookingCountryID')) int.f.SetClassIf('ddlCarHireDriverBookingCountryID', 'error', int.dd.GetValue('ddlCarHireDriverBookingCountryID') == 0);

		//validate driver age
		if (int.f.GetObject('ddlCarHireDriverAge')) int.f.SetClassIf('ddlCarHireDriverAge', 'error', int.dd.GetValue('ddlCarHireDriverAge') == 0);

		//validate passenger number
		if (int.f.GetObject('ddlCarHirePassengers')) int.f.SetClassIf('ddlCarHirePassengers', 'error', int.dd.GetValue('ddlCarHirePassengers') == 0);
	}

	//anywhere search
	if (sSearchMode === 'Anywhere') {
		if (int.f.GetObject('ddlProductAttributeID')) {
			int.f.SetClassIf(SearchTool.CustomSelectContainer('ddlProductAttributeID'),
				'error', int.dd.GetIntValue('ddlProductAttributeID') === 0);
		}
	}

	if (SearchTool.AdditionalValidationFunction != null) {
		try {
			SearchTool.AdditionalValidationFunction();
		}
		catch (exc) {
			//if (console != undefined && log in console) console.log('PreSearchFunctionError:- ' + exc);
		}
	}

	//if we have errors show warning message otherwise perform search
	var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divSearch');

    if (aErrorControls.length == 0) {
        if (SearchTool.HasLogger()) {
            if (int.f.GetObject('hidLogger') && sSearchMode === 'HotelOnly' && int.dd.GetValue('ddlRooms') == 1) {
                Logger.LogEvent('Search', 0);
            } else if (int.f.GetObject('hidLogger')) {
                Logger.LogEvent('OtherSearch', 0);
            }
        }
		SearchTool.SendSearch();
	}
	else {
		if (typeof (SearchTool.OverrideWarningInfoBox) == 'function') {
			SearchTool.OverrideWarningInfoBox('hidWarning_Invalid');
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidWarning_Invalid'), 'warning', null, me.InsertWarningAfter);
		};
	}
}

SearchTool.CustomSelectContainer = function (id) {
	var oSelectInput = int.f.GetObject(id);
	if (int.f.HasClass(oSelectInput.parentNode, 'custom-select')) {
		return oSelectInput.parentNode;
	} else {
		return oSelectInput;
	}
}

SearchTool.SendSearch = function () {
	var bDeepLink = int.f.GetValue('hidDeeplinkSearch');
	SearchTool.Support.SetCookie();

	if (bDeepLink == "true") {
		int.ff.Call('=iVectorWidgets.SearchTool.DeeplinkSearch', SearchTool.DeeplinkSearchComplete)
	} else {
		WaitMessage.Show(
			'Search',
			function () {
				int.ff.Call('=iVectorWidgets.SearchTool.Search', SearchTool.SearchComplete);
			}
		);

		SearchTool.Timeout = setTimeout(
			function () {
				SearchTool.SearchTimeout();
			},
			60000
		);
	};
};

SearchTool.DeeplinkSearchComplete = function (sDeeplink) {
	//Launch search in current window - Parent allows us to launch outside of iFrame
	web.PubSub.publish('SearchTool.DeeplinkSearchComplete');
	window.parent.location.href = sDeeplink;
};

//6. Expand
SearchTool.Expand = function () {
	me.Collapsed = false;
	me.Expanded = true;

	if (me.ExpandAction == 'Show') {
		int.f.Show('divSearch');
		int.f.Hide('btnClose');
		int.f.Hide('divSearchAgain');
	}
	else if (me.ExpandAction == 'Popup') {
		int.f.Show('divSearch');
		int.f.Show('btnClose');
		web.ModalPopup.Show('divSearch', false, 'searchpopup');
	}
}

SearchTool.SearchTimeout = function () {
	WaitMessage.Hide();
	WaitMessage.Suppress = true;

	var sSearchMode = int.f.GetValue('hidSearchMode');
	var bUseNoResultsFunctionality = 'False';
	if (int.f.GetObject('hidNoResultsFunctionality')) {
		bUseNoResultsFunctionality = int.f.GetValue('hidNoResultsFunctionality');
	}

	if (bUseNoResultsFunctionality === 'True') {
		SearchTool.SuppressResults = true;
		clearTimeout(SearchTool.Timeout);

		var oRedirects = SearchTool.RedirectObject();

		if (sSearchMode == 'FlightPlusHotel') {
			web.Window.Redirect(int.f.GetValue('hidFlightPlusHotelURL'));
		}
		else if (sSearchMode === 'HotelOnly') {
			web.Window.Redirect(oRedirects.HotelOnlyURL);
		}
		else if (sSearchMode === 'FlightOnly') {
			web.Window.Redirect(oRedirects.FlightOnlyURL);
		}
		else if (sSearchMode === 'TransferOnly') {
			web.Window.Redirect(oRedirects.TransferOnlyURL);
		}
		else if (sSearchMode === 'CarHireOnly') {
			web.Window.Redirect(oRedirects.CarHireOnlyURL);
		}
	} else {
		if (typeof (SearchTool.OverrideWarningInfoBox) == 'function') {
			SearchTool.OverrideWarningInfoBox('hidWarning_NoResults');
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidWarning_NoResults'), 'warning', null, me.InsertWarningAfter);
		};
	}
	SearchTool.SuppressResults = true;
	clearTimeout(SearchTool.Timeout);
}

SearchTool.SearchComplete = function (sJSON) {
	//clear timeout
	clearTimeout(SearchTool.Timeout);

	//parse the return object
	var oReturn = JSON.parse(sJSON);

	//if we have suppressed the results due to a timeout return
	if (SearchTool.SuppressResults) return;

	//parse results and redirect to appropriate page
	var oSearchReturn = oReturn.SearchReturn;
	var oAvailabilityReturn = oReturn.SearchAvailabilities;

	web.PubSub.publish('SearchTool.SearchComplete');

	//Decide where we are redirecting to
	SearchTool.Redirect(oSearchReturn, oAvailabilityReturn);
}

SearchTool.Redirect = function (oSearchReturn, oAvailabilityReturn) {
	var sSearchMode = int.f.GetValue('hidSearchMode');
	var bUseFlightCarouselResults = int.f.GetValue('hidUseFlightCarouselResults');
	var bUseNoResultsFunctionality = 'False';

	if (int.f.GetObject('hidNoResultsFunctionality')) {
		bUseNoResultsFunctionality = int.f.GetValue('hidNoResultsFunctionality');
	}

	var oRedirects = SearchTool.RedirectObject();

	if (sSearchMode === 'FlightPlusHotel' && oSearchReturn.PropertyCount > 0 && oSearchReturn.ExactMatchFlightCount > 0) {
		web.Window.Redirect(int.f.GetValue('hidFlightPlusHotelURL'));
	}
    else if ((sSearchMode === 'HotelOnly' && (oSearchReturn.PropertyCount > 0 || oAvailabilityReturn.Availability === true)) || (sSearchMode === 'HotelOnly' && bUseNoResultsFunctionality === 'True')) {
        if (SearchTool.HasLogger()) {
            Logger.LogEvent('WebsitePostConnectElapsedTime', oSearchReturn.DataCollectionInfo.WebsitePostConnectElapsedTime);
        }
		web.Window.Redirect(oRedirects.HotelOnlyURL);
	}
	else if ((sSearchMode === 'FlightOnly' && oSearchReturn.FlightCount > 0) || (sSearchMode === 'FlightOnly' && oSearchReturn.FlightCount === 0 && bUseNoResultsFunctionality === 'True')) {
		web.Window.Redirect(oRedirects.FlightOnlyURL);
	}
	else if (sSearchMode === 'TransferOnly' && oSearchReturn.TransferCount > 0) {
		web.Window.Redirect(oRedirects.TransferOnlyURL);
	}
	else if (((sSearchMode === 'FlightPlusHotel' || sSearchMode === 'FlightOnly') && oSearchReturn.FlightCarouselCount > 0 && bUseFlightCarouselResults === 'true') || (sSearchMode === 'FlightPlusHotel' && bUseNoResultsFunctionality === 'True')) {
		web.Window.Redirect(int.f.GetValue('hidFlightPlusHotelURL'));
	}
	else if (sSearchMode === 'CarHireOnly' && oSearchReturn.CarHireCount > 0) {
		web.Window.Redirect(oRedirects.CarHireOnlyURL);
	}
	else if (sSearchMode === 'Anywhere' && oSearchReturn.PropertyCount > 0 && oSearchReturn.ExactMatchFlightCount > 0) {
		web.Window.Redirect(oRedirects.AnywhereURL);
	}
    else if (sSearchMode === 'FlightPlusHotel' && oSearchReturn.FlightCount > 0 && int.f.GetValue('hidTripType') === 'flightitinerary') {
        web.Window.Redirect(int.f.GetValue('hidFlightPlusHotelURL'));
    }
	else {
		SearchTool.NoResultsHandler(sSearchMode);
	}
}

SearchTool.NoResultsHandler = function (sSearchMode) {
	WaitMessage.Hide();
    if (int.f.GetObject('divContent')) int.f.Hide('divContent');
    if (int.f.GetObject('divFlightFilter')) int.f.Hide('divFlightFilter');
	if (sSearchMode != undefined && sSearchMode == 'CarHireOnly' && int.f.GetObject('hidWarning_CarHireNoResults') != undefined
		&& int.f.GetValue('hidWarning_CarHireNoResults') != '' && typeof (SearchTool.OverrideWarningInfoBox) == 'function') {
		SearchTool.OverrideWarningInfoBox('hidWarning_CarHireNoResults');
	}
	else if (sSearchMode != undefined && sSearchMode == 'CarHireOnly' && int.f.GetObject('hidWarning_CarHireNoResults') != undefined
		&& int.f.GetValue('hidWarning_CarHireNoResults') != '') {
		web.InfoBox.Show(int.f.GetValue('hidWarning_CarHireNoResults'), 'warning', null, me.InsertWarningAfter);
	}
	else if (typeof (SearchTool.OverrideWarningInfoBox) == 'function') {
		SearchTool.OverrideWarningInfoBox('hidWarning_NoResults');
	}
	else {
		web.InfoBox.Show(int.f.GetValue('hidWarning_NoResults'), 'warning', null, me.InsertWarningAfter);
	};
}

SearchTool.RedirectObject = function () {
	var sRedirects = int.f.GetValue('hidSearchRedirects');

	var oRedirects;

	//parse array of redirects
	if (sRedirects != "") {
		oRedirects = JSON.parse(int.f.GetValue('hidSearchRedirects'));

		if (!oRedirects.hasOwnProperty('HotelOnlyURL')) oRedirects.HotelOnlyURL = '/search-results';
		if (!oRedirects.hasOwnProperty('FlightOnlyURL')) oRedirects.FlightOnlyURL = '/flight-results';
		if (!oRedirects.hasOwnProperty('TransferOnlyURL')) oRedirects.TransferOnlyURL = '/booking-summary';
		if (!oRedirects.hasOwnProperty('CarHireOnlyURL')) oRedirects.CarHireOnlyURL = '/car-hire-results';
		if (!oRedirects.hasOwnProperty('AnywhereURL')) oRedirects.CarHireOnlyURL = '/search-results';
	}
	else {
		oRedirects = {
			HotelOnlyURL: '/search-results',
			FlightOnlyURL: '/flight-results',
			TransferOnlyURL: '/booking-summary',
			CarHireOnlyURL: '/car-hire-results',
			AnywhereURL: '/search-results'
		};
	}

	return oRedirects
}

//7. Clear
SearchTool.Clear = function () {
	var aPairs = me.Support.GetCookie();

	int.dd.SetIndex('ddlDepartingFromID', 0);
	int.f.SetValue('acpArrivingAtID', int.f.GetObject('acpArrivingAtID').placeholder == undefined ? '' : int.f.GetObject('acpArrivingAtID').placeholder);

    if (int.f.GetObject('acpHotelOnlyArrivingAtID')) {
        int.f.SetValue('acpHotelOnlyArrivingAtID', int.f.GetObject('acpHotelOnlyArrivingAtID').placeholder == undefined
            ? '' : int.f.GetObject('acpHotelOnlyArrivingAtID').placeholder);       
    }

	int.dd.SetIndex('ddlRegionID', 0);
	int.dd.SetIndex('ddlResortID', 0);
	int.f.SetValue('txtDepartureDate', aPairs['DepartureDate'] != '01/01/1900' && aPairs['DepartureDate'] != undefined ? aPairs['DepartureDate'] : '');

	int.f.SetValue('ddlDuration', 7);
	int.f.SetValue('ddlRooms', 1);

	int.f.SetValue('ddlAdults_1', 2);
	int.f.SetValue('ddlAdults_2', 1);
	int.f.SetValue('ddlAdults_3', 1);

	if (int.f.GetObject('li_SearchMode_TransferOnly')) {
		int.f.SetValue('acpAirportIDTransferPickup', int.f.GetObject('acpAirportIDTransferPickup').placeholder);
		int.f.SetValue('acpPropertyIDTransferPickup', int.f.GetObject('acpPropertyIDTransferPickup').placeholder);

		if (aPairs['PickupDate'] != '01/01/1900' && aPairs['PickupDate'] != undefined) {
			int.f.SetValue('txtPickupDate', aPairs['PickupDate']);
		}
		else {
			int.f.SetValue('txtPickupDate', '');
		}

		int.f.SetValue('acpPropertyIDTransferDropOff', int.f.GetObject('acpPropertyIDTransferDropOff').placeholder);
		int.f.SetValue('acpAirportIDTransferDropOff', int.f.GetObject('acpAirportIDTransferDropOff').placeholder);

		if (aPairs['DropOffDate'] != '01/01/1900' && aPairs['DropOffDate'] != undefined) {
			int.f.SetValue('txtDropOffDate', aPairs['DropOffDate']);
		}
		else {
			int.f.SetValue('txtDropOffDate', '');
		}

		int.cb.SetValue('cbReturnTransfer', false);

		int.f.SetValue('txtArrivalTime', '');
		int.f.SetValue('txtDropOffTime', '');
	}

	for (i = 1; i <= 3; i++) int.f.SetValue('ddlChildren_' + i, 0);
	for (i = 1; i <= 3; i++) int.f.SetValue('ddlInfants_' + i, 0);

	var oOccupancyTable = int.f.GetObject('tblOccupancy');
	var aOccupancyDropdowns = oOccupancyTable.getElementsByTagName('select');

	for (i = 0; i < aOccupancyDropdowns.length; i++) {
		if (aOccupancyDropdowns[i].id.indexOf('ddlChildAge_') != -1) int.dd.SetIndex(aOccupancyDropdowns[i], 0)
	}

	int.f.Hide('trAges_1');
	int.f.Hide('trGuests_2');
	int.f.Hide('trAges_2');
	int.f.Hide('trGuests_3');
	int.f.Hide('trAges_3');

	int.f.Hide('divSearch_Advanced');
	int.f.Show('aShowAdvancedOptions');
	int.f.Hide('aHideAdvancedOptions');
}

//x. Support
SearchTool.Support = new function () {
	//x.1 populate dropdown ranges
	//picks up range_[from]_[to] in classname and add options accordingly
	this.PopulateDropdownRanges = function () {
		var aRangeDropdowns = int.f.GetElementsByClassName('select', 'range_', 'divSearch');

		for (var i = 0; i < aRangeDropdowns.length; i++) {
			var oDropdown = aRangeDropdowns[i];

			int.dd.Clear(oDropdown);

			var aToFrom = /range_(\d+)_(\d+)/.exec(oDropdown.className);

			//add options
			for (var j = int.n.SafeInt(aToFrom[1]); j <= int.n.SafeInt(aToFrom[2]); j++) {
				//get suffix for dropdowns
				var sSuffix = SearchTool.Support.GetDropdownSuffix(oDropdown, j);

				int.dd.AddOption(oDropdown, j + sSuffix, j);
			}
		}
	}

	this.GetDropdownSuffix = function (oDropdown, iPosition) {
		var sSuffix = '';

		//get suffix depending on dropdown type and position
		if (oDropdown.id.indexOf('ddlDuration') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidDurationSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidDurationPluralSuffix');
		}
		else if (oDropdown.id.indexOf('ddlRooms') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidRoomsSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidRoomsPluralSuffix');
		}
		else if (oDropdown.id.indexOf('ddlAdults') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidAdultsSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidAdultsPluralSuffix');
		}
		else if (oDropdown.id.indexOf('ddlChildren') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidChildrenSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidChildrenPluralSuffix');
		}
		else if (oDropdown.id.indexOf('ddlInfants') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidInfantsSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidInfantsPluralSuffix');
		}
		else if (oDropdown.id.indexOf('ddlChildAge') > -1) {
			if (iPosition == 1)
				sSuffix = int.f.GetValue('hidAgeSingularSuffix');
			else
				sSuffix = int.f.GetValue('hidAgePluralSuffix');
		}

		if (sSuffix != '') sSuffix = ' ' + sSuffix;

		return sSuffix;
	}

	//Do the switching of pick up/drop off points
	this.TransferDropDowns = function (oDropDown) {
		sDropdownID = oDropDown.id;
		sType = int.dd.GetValue(oDropDown);
		//we want to show the property textbox as a fallback.
		if (sType != 'Property' && sType != "Airport") {
			sType = 'Property'
		}

		if (oDropDown.id == 'ddlPickupType') {
			int.f.Hide('divPropertyAutoTransferPickup');
			int.f.Hide('divAirportAutoTransferPickup');

			int.f.Show('div' + sType + 'AutoTransferPickup');
			SearchTool.Support.SetTransferLabel();

			if (sType == 'Airport') {
				int.f.Show('divShowHideTransferDepartingFromAutoComplete');
			}
			else {
				int.f.Hide('divShowHideTransferDepartingFromAutoComplete');
				int.f.Hide('divAirportDropdownTransferPickup');
				int.f.Show('aShowTransferDepartingFromDropDown');
				int.f.Hide('aShowTransferDepartingFromAutoComplete');
			}
		}
		else {
			int.f.Hide('divPropertyAutoTransferDropOff');
			int.f.Hide('divAirportAutoTransferDropOff');

			int.f.Show('div' + sType + 'AutoTransferDropOff');

			if (sType == 'Airport') {
				int.f.Show('divShowHideTransferArrivingAtAutoComplete');
			}
			else {
				int.f.Hide('divShowHideTransferArrivingAtAutoComplete');
				int.f.Hide('divAirportDropdownTransferDropOff');
				int.f.Show('aShowTransferArrivingAtDropDown');
				int.f.Hide('aShowTransferArrivingAtAutoComplete');
			}
		}
	}

	//getcookie
	//get cookie, split up and build associative array
	this.GetCookie = function () {
		var aList = new Array();
		var sKeyValuePairs = unescape(int.c.Get('__search_1'));
		var aBits = sKeyValuePairs.split('&');
		for (var i = 0; i < aBits.length; i++) {
			var aKeyValue = aBits[i].split('=');
			if (aKeyValue.length == 2) aList[aKeyValue[0]] = aKeyValue[1];
		}
		return aList;
	}

	//setcookie
	this.SetCookie = function () {
		//decide the fields we want to exclude
		var aExclude = new Array();
		aExclude.push('acpPriorityPropertyID');
		aExclude.push('acpAirportIDTransferPickup');
		aExclude.push('acpPropertyIDTransferPickup');
		aExclude.push('acpAirportIDTransferDropOff');
		aExclude.push('acpPropertyIDTransferDropOff');
		aExclude.push('hidWarning_NoResults');
		aExclude.push('hidInsertWarningAfter');
		aExclude.push('hidWarning_Invalid');
		aExclude.push('acpArrivingAtAirportIDScript');
		aExclude.push('acpArrivingAtIDScript');
		aExclude.push('acpDepartingFromIDScript');
		aExclude.push('acpArrivingAtID');
		aExclude.push('ddlCarHirePickUpDepotID');
		aExclude.push('ddlCarHireDropOffDepotID');
        aExclude.push('acpCarHireCountryID');
		aExclude.push('acpCarHirePickUpDepotID');
		aExclude.push('acpCarHireDropOffDepotID');
        aExclude.push('acpCarHireCountryIDHidden');
		aExclude.push('acpCarHirePickUpDepotIDHidden');
		aExclude.push('acpCarHireDropOffDepotIDHidden');
		aExclude.push('acpCarHirePickUpDepotIDScript');
		aExclude.push('aHideDropOffDepotAutoComplete');
		aExclude.push('hidDefaultSearchValues');

		for (var i = 1; i <= 10; i++) {
			aExclude.push('acpFlightItineraryDepartureAutoID_' + i + 'Script');
			aExclude.push('acpFlightItineraryDepartureAutoID_' + i + 'Hidden');
			aExclude.push('acpFIArrAutoID_' + i + 'Script');
			aExclude.push('acpFIArrAutoID_' + i + 'Hidden');
			aExclude.push('ddlFlightItineraryArrival_' + i);
		}

		//get key value pairs
		var sKeyValuePairs = int.f.GetContainerQueryStringByIDs('divSearch', aExclude);

		//remove input prefixes
		sKeyValuePairs = int.s.Replace(sKeyValuePairs, 'txt', '');
		sKeyValuePairs = int.s.Replace(sKeyValuePairs, 'ddl', '');
		sKeyValuePairs = int.s.Replace(sKeyValuePairs, 'hid', '');
		sKeyValuePairs = int.s.Replace(sKeyValuePairs, 'chk', '');
		//save cookie
		int.c.Set('__search_1', sKeyValuePairs, 7);

		return sKeyValuePairs;
	};

	//getvalue
	this.GetValue = function (KeyValuePair, Key) {
		var regex = new RegExp(Key + '=([\\w\\d]+)');
		var aMatches = regex.exec(KeyValuePair);
		if (aMatches != null) return aMatches[1];
	}

	//set search mode
	this.SetSearchMode = function (sSearchMode) {
		//if start mode is undefined then setup has not run yet so bomb out
		if (!me.StartMode) return false;

		int.f.SetClassIf('divSearch', 'FlightPlusHotel', sSearchMode == 'FlightPlusHotel');
		int.f.SetClassIf('divSearch', 'HotelOnly', sSearchMode == 'HotelOnly');
		int.f.SetClassIf('divSearch', 'FlightOnly', sSearchMode == 'FlightOnly');
		int.f.SetClassIf('divSearch', 'TransferOnly', sSearchMode == 'TransferOnly');
		int.f.SetClassIf('divSearch', 'CarHireOnly', sSearchMode == 'CarHireOnly');

		int.f.SetValue('hidSearchMode', sSearchMode);

		int.f.SetClassIf('li_SearchMode_FlightPlusHotel', 'selected', sSearchMode == 'FlightPlusHotel');
		int.f.SetClassIf('li_SearchMode_HotelOnly', 'selected', sSearchMode == 'HotelOnly');

		int.f.RemoveClass('divSearch', 'advanced');

		int.f.ShowIf('divArrivingAtDestination', sSearchMode != 'FlightOnly');
		int.f.ShowIf('divArrivingAtAirport', sSearchMode == 'FlightOnly');
		int.f.ShowIf('divArrivingAtAirportControls', sSearchMode == 'FlightOnly');

		if (sSearchMode == 'HotelOnly') int.f.Show('divArrivingAtDestination');

		var sGuestPrefix = 'tr';
		if (int.f.GetValue('hidGuestPrefix') != "") {
			sGuestPrefix = int.f.GetValue('hidGuestPrefix');
		};

		if (int.f.GetObject('li_SearchMode_FlightOnly')) {
			if (sSearchMode == 'FlightOnly') {
				int.f.AddClass('li_SearchMode_FlightOnly', 'selected');
				int.f.Show('lblPassengers');
				int.f.Hide('divArrivingAtAirportAuto');
				int.f.RemoveClass('divSearch_Guests', 'transferOnly');
				int.f.AddClass('divSearch_Guests', 'flightOnly');

				if (sGuestPrefix == 'tr') {
					int.f.GetObject('tdAgesLabel_1').colSpan = 1;
				};

				int.dd.SetValue('ddlRooms', 1);
				int.f.Hide('fldRooms');
				SearchTool.Support.ArrivalAirportDropdown();
			}
			else {
				int.f.RemoveClass('li_SearchMode_FlightOnly', 'selected');
				int.f.Hide('lblPassengers');
				int.f.RemoveClass('divSearch_Guests', 'flightOnly');
				if (sGuestPrefix == 'tr') {
					int.f.GetObject('tdAgesLabel_1').colSpan = 2;
				}
				int.f.Show('fldRooms');
			}
		};

		if (int.f.GetObject('li_SearchMode_TransferOnly')) {
			int.f.SetClassIf('li_SearchMode_TransferOnly', 'selected', sSearchMode == 'TransferOnly');

			if (sSearchMode == 'TransferOnly') {
				SearchTool.Support.ReturnTransfer();

				int.f.Show('divSearch_Transfers');
				int.f.Hide('divSearch_Where');
				int.f.Hide('divSearch_When');
				int.f.Hide('divSearch_Rooms');

				int.f.Show('lblPassengers');
				int.f.RemoveClass('divSearch_Guests', 'flightOnly');
				int.f.AddClass('divSearch_Guests', 'transferOnly');

				int.f.Show('txtPickUpLocationAirport');
				int.f.Hide('divPropertyAutoTransferPickup');
				int.f.Show('txtDropOffLocationAirport');
				int.f.Hide('divPropertyAutoTransferDropOff');
			}
			else {
				int.f.Hide('divSearch_Transfers');

				int.f.Show('divSearch_Where');
				int.f.Show('divSearch_When');
			}
		};

		if (int.f.GetObject('li_SearchMode_CarHireOnly')) {
			int.f.SetClassIf('li_SearchMode_CarHireOnly', 'selected', sSearchMode == 'CarHireOnly');

			if (sSearchMode == 'CarHireOnly') {
				int.f.Show('divSearch_CarHire');
				int.f.Hide('divSearch_Where');
				int.f.Hide('divSearch_When');
				int.f.Hide('divSearch_Rooms');
				int.f.Hide('fldTo');
				int.f.Hide('divSearch_Guests')
			} else {
				int.f.Hide('divSearch_CarHire');
				int.f.Show('fldTo');
				int.f.Show('divSearch_Guests');
			}
		};

		int.f.ShowIf('aShowAdvancedOptions', (sSearchMode != 'FlightOnly' && sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly') && !me.AdvancedOptionsShown);
		int.f.ShowIf('aHideAdvancedOptions', sSearchMode != 'FlightOnly' && sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly' && me.AdvancedOptionsShown);
		int.f.ShowIf('divSearch_Advanced', (sSearchMode != 'FlightOnly' && sSearchMode != 'TransferOnly' && sSearchMode != 'CarHireOnly') && me.AdvancedOptionsShown);

		// if switching to hotel only reset airport and redraw dropdowns
		if (sSearchMode == 'HotelOnly') {
			if (!isNaN(int.dd.GetValue('ddlDepartingFromID'))) {
				//					int.dd.SetValue('ddlDepartingFromID', 0);
				int.dd.SetIndex('ddlDepartingFromID', 0);
			}
			SearchTool.Support.ArrivingAtDropdowns();
		}

		if (sSearchMode == 'HotelOnly' || sSearchMode == 'FlightPlusHotel') {
			int.f.RemoveClass('divSearch_Guests', 'flightOnly');
			int.f.RemoveClass('divSearch_Guests', 'transferOnly');
		}

		int.f.ShowIf('lblHighRatedHotelFilter', sSearchMode == 'HotelOnly' || sSearchMode == 'FlightPlusHotel')

		SearchTool.Show();
	}

	//set DepartingFromID
	this.SetDepartingFromID = function () {
		if (int.f.GetValue('acpDepartingFromIDHidden') != 0 && int.f.Visible('divDepartingFromAuto')) {
			int.f.SetValue('hidAirportDepartingFromID', int.f.GetValue('acpDepartingFromIDHidden'));
			int.dd.SetValue('ddlDepartingFromID', int.f.GetValue('acpDepartingFromIDHidden'));
		}
		else {
			int.f.SetValue('hidAirportDepartingFromID', int.dd.GetIntValue('ddlDepartingFromID'));
			int.f.SetValue('acpDepartingFromIDHidden', int.dd.GetIntValue('ddlDepartingFromID'));
			SearchTool.Support.SetAirportName(int.dd.GetIntValue('ddlDepartingFromID'), int.f.GetObject('acpDepartingFromID'));
		}

		//update destination dropdown
		SearchTool.Support.ArrivingAtDropdowns();

		//update arrival airport dropdown so the airports are filtered by route availability
		if (int.f.GetObject('ddlArrivalAirportID')) {
			SearchTool.Support.ArrivalAirportDropdown();
		}
	}

	//set DepartingFromID for Transfer only Search
	this.SetTransferDepartingFromID = function () {
		if (int.f.GetValue('acpAirportIDTransferPickupHidden') != 0 && int.f.Visible('divAirportAutoTransferPickup')) {
			int.f.SetValue('hidTransferPickupLocationID', int.f.GetValue('acpAirportIDTransferPickupHidden'));
			int.dd.SetValue('ddlAirportTransferPickup', int.f.GetValue('acpAirportIDTransferPickupHidden'));
		}
		else {
			int.f.SetValue('hidTransferPickupLocationID', int.dd.GetIntValue('ddlAirportTransferPickup'));
			int.f.SetValue('acpAirportIDTransferPickupHidden', int.dd.GetIntValue('ddlAirportTransferPickup'));
			SearchTool.Support.SetAirportName(int.dd.GetIntValue('ddlAirportTransferPickup'), int.f.GetObject('acpAirportIDTransferPickup'));
		}
	}

	//set ArrivingAtId
	this.SetArrivingAtID = function () {
		var sSearchMode = int.f.GetValue('hidSearchMode');

		if (sSearchMode == 'FlightOnly' && int.f.GetValue('acpArrivingAtAirportIDHidden') != 0 && int.f.Visible('divArrivingAtAirportAuto')) {
			int.f.SetValue('hidArrivingAtID', int.f.GetIntValue('acpArrivingAtAirportIDHidden') + 1000000);
		} else if (int.f.GetValue('acpArrivingAtIDHidden') != 0) {
			int.f.SetValue('hidArrivingAtID', int.f.GetValue('acpArrivingAtIDHidden'));
		} else if (int.dd.GetIntValue('ddlResortID') == 0) {
			int.f.SetValue('hidArrivingAtID', int.dd.GetIntValue('ddlRegionID'));
		} else {
			int.f.SetValue('hidArrivingAtID', int.dd.GetIntValue('ddlResortID') * -1);
		};

		if (int.f.GetObject('fldPriorityProperty')) {
			int.f.EnableIf('acpPriorityPropertyID', int.f.GetValue('hidArrivingAtID') != 0);
		}
	}

	//set ArrivingAtID for Transfer only Search
	this.SetTransferArrivingAtID = function () {
		if (int.f.GetValue('acpAirportIDTransferDropOffHidden') != 0 && int.f.Visible('divAirportAutoTransferDropOff')) {
			int.f.SetValue('hidTransferDropOffLocationID', int.f.GetValue('acpAirportIDTransferDropOffHidden'));
			int.dd.SetValue('ddlAirportTransferDropoff', int.f.GetValue('acpAirportIDTransferDropOffHidden'));
		}
		else {
			int.f.SetValue('hidTransferDropOffLocationID', int.dd.GetIntValue('ddlAirportTransferDropoff'));
			int.f.SetValue('acpAirportIDTransferDropOffHidden', int.dd.GetIntValue('ddlAirportTransferDropoff'));
			SearchTool.Support.SetAirportName(int.dd.GetIntValue('ddlAirportTransferDropoff'), int.f.GetObject('acpAirportIDTransferDropOff'));
		}
	}

	//set car hire pickup id
	this.SetCarHirePickUpDepotID = function () {
		if (int.f.GetIntValue('acpCarHirePickUpDepotIDHidden') != 0) {
			int.f.SetValue('hidCarHirePickUpDepotID', int.f.GetIntValue('acpCarHirePickUpDepotIDHidden'));
		}
	}

	//set car hire dropoff id
	this.SetCarHireDropOffDepotID = function () {
		if (int.f.GetIntValue('acpCarHireDropOffDepotIDHidden') != 0) {
			int.f.SetValue('hidCarHireDropOffDepotID', int.f.GetIntValue('acpCarHireDropOffDepotIDHidden'));
		}
	}

	//ArrivingAtDropdowns
	this.ArrivingAtDropdowns = function () {
		var iArrivingAtID = int.f.GetValue('hidArrivingAtID');
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');
		int.ff.Call(
			'=iVectorWidgets.SearchTool.ArrivingAtDropdowns',
			function (sHTML) {
				int.f.SetHTML('divArrivingAtDropdown', sHTML);
			},
			iArrivingAtID, iDepartureAirportID
		);
	}

	//ArrivingAtDropdownsCountry, filters region dropdown by country when the country and region dropdowns are separate
	this.ArrivingAtDropdownsCountry = function () {
		var iArrivingAtID = int.f.GetValue('hidArrivingAtID');
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');
		var iCountryID = int.f.GetValue('ddlCountryID');
		int.ff.Call(
			'=iVectorWidgets.SearchTool.ArrivingAtDropdownsCountry',
			function (sHTML) {
				int.f.SetHTML('divArrivingAtDropdown', sHTML);
			},
			iArrivingAtID, iDepartureAirportID, iCountryID
		);
	}

	//airport dropdown
	this.ArrivalAirportDropdown = function () {
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');
		var iArrivalAirportID = int.f.GetValue('hidArrivingAtID') - 1000000;
		int.ff.Call(
			'=iVectorWidgets.SearchTool.ArrivalAirportDropdown',
			function (sHTML) {
				int.f.SetHTML('divArrivingAtAirport', sHTML);
			},
			iDepartureAirportID, iArrivalAirportID
		);
	}

	//ArrivingAtDropdowns
	this.SelectCountry = function (sMode) {
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');

		if (sMode == 'Arrival') {
			var iCountryID = int.f.GetValue('ddlCountryID');
			int.ff.Call(
				'=iVectorWidgets.SearchTool.ArrivingAtCountryDropdowns',
				function (sHTML) {
					int.f.SetHTML('divArrivingAtDropdown', sHTML);
				},
				iCountryID, iDepartureAirportID
			);
		}
		else {
			var iCountryID = int.f.GetValue('ddlDepartingFromCountryID');
			int.ff.Call(
				'=iVectorWidgets.SearchTool.DepratingFromCountryDropdowns',
				function (sHTML) {
					int.f.SetHTML('divDepartingFromDropdown', sHTML);
				},
				iCountryID
			);
		}
	}

	//autocomplete region resorts
	this.GetRegionResorts = function () {
		var sText = int.f.GetValue('acpArrivingAtID');
		var sSearchMode = int.f.GetValue('hidSearchMode');

		var iDepartureAirportID = 0
		if (sSearchMode == 'FlightOnly' || sSearchMode == 'FlightPlusHotel') {
			iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');
		}

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject('acpArrivingAtIDOptions'));
			return;
		}
		web.AutoComplete.Selected = false;
		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteRegionDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, iDepartureAirportID);
	}

	this.GetDepots = function (oTextBox) {
		var sText = int.f.GetValue(oTextBox);

		var iGeographyLevel1ID = int.f.GetValue('ddlCarHireCountryID');

		web.AutoComplete.Selected = false;
		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteCarHireDepotDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, iGeographyLevel1ID, oTextBox.id);
	}

	//autocomplete
	this.SetDepartingFromName = function () {
		var iDepartingFromID = int.f.GetValue('hidAirportDepartingFromID');
		int.ff.Call(
			'=iVectorWidgets.SearchTool.AirPortName',
			function (sValue) {
				int.f.SetValue('acpDepartingFromID', sValue);
			},
			iDepartingFromID
		);
	}

	//autocomplete
	this.SetArrivingAtName = function () {
		var iArrivingAtID = int.f.GetValue('hidArrivingAtID');
		int.ff.Call(
			'=iVectorWidgets.SearchTool.ArrivingAtName',
			function (sValue) {
				int.f.SetValue('acpArrivingAtID', sValue);
			},
			iArrivingAtID
		);
	}

	//autocomplete Airport
	this.SetAirportName = function (iAirportID, oInput) {
		int.ff.Call(
			'=iVectorWidgets.SearchTool.AirPortName',
			function (sValue) {
				int.f.SetValue(oInput, sValue);
			},
			iAirportID
		);
	}
	//autocomplete Airport
	this.SetPropertyName = function (iPropertyID, oInput) {
		int.ff.Call(
			'=iVectorWidgets.SearchTool.PropertyName',
			function (sValue) {
				int.f.SetValue(oInput, sValue);
			},
			iPropertyID
		);
	}

	//show hide departing from auto complete
	this.ShowHideDepartingFromAutoComplete = function () {
		int.f.Toggle('divDepartingFromDropdown');

		if (int.f.GetObject('divDepartingFromCountryCountry')) int.f.Toggle('divDepartingFromCountryCountry');

		int.f.Toggle('divDepartingFromAuto');
		int.f.Toggle('aShowDepartingFromDropdown');
		int.f.Toggle('aShowDepartingFromAutoComplete');
	}

	//show hide arriving at auto complete
	this.ShowHideArrivingAtAutoComplete = function () {
		int.f.Toggle('divArrivingAtDropdown');
		int.f.Toggle('divArrivingAtAuto');
		int.f.Toggle('aShowArrivingAtDropdown');
		int.f.Toggle('aShowArrivingAtAutoComplete');

		if (int.f.GetObject('divArrivingAtCountryCountry'))
			int.f.Toggle('divArrivingAtCountryCountry');
	}

	//show hide departing from auto complete for Transfer only search
	this.ShowHideTransferDepartingFromAutoComplete = function () {
		int.f.Toggle('divAirportDropdownTransferPickup');
		int.f.Toggle('divAirportAutoTransferPickup');
		int.f.Toggle('aShowTransferDepartingFromDropDown');
		int.f.Toggle('aShowTransferDepartingFromAutoComplete');
	}

	//show hide arriving at auto complete for Transfer only search
	this.ShowHideTransferArrivingAtAutoComplete = function () {
		int.f.Toggle('divAirportDropdownTransferDropOff');
		int.f.Toggle('divAirportAutoTransferDropOff');
		int.f.Toggle('aShowTransferArrivingAtDropDown');
		int.f.Toggle('aShowTransferArrivingAtAutoComplete');
	}

	//autocomplete property
	this.GetProperties = function (sTextBoxID) {
		var sText = int.f.GetValue(sTextBoxID);

		var iArrivingAtID = 0;

		if (sTextBoxID == 'acpPriorityPropertyID') {
			iArrivingAtID = int.f.GetValue('hidArrivingAtID');
		}

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;

		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompletePropertyDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, iArrivingAtID, sTextBoxID);
	}

    this.GetCountries = function(sTextBoxID) {
        var sText = int.f.GetValue(sTextBoxID);

        if (sText.length < 3) {
            web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
            return;
        }

        web.AutoComplete.Selected = false;

        int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteCarHireCountryDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID, int.f.GetValue(sTextBoxID + 'Script'));
    }

	//autocomplete property
	this.GetAirports = function (sTextBoxID) {
		var sText = int.f.GetValue(sTextBoxID);

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;

		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteAirportDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID, int.f.GetValue(sTextBoxID + 'Script'), 0, false);
	}

	this.GetDepartureAirports = function (sTextBoxID) {
		var sText = int.f.GetValue(sTextBoxID);

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;

		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteDepartureAirport', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID, int.f.GetValue(sTextBoxID + 'Script'));
	}

	//autocomplete property
	this.GetAirportsByDeparture = function (sTextBoxID) {
		var sText = int.f.GetValue(sTextBoxID);
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;
		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteAirportDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID, int.f.GetValue(sTextBoxID + 'Script'), iDepartureAirportID, false);
	}

	//autocomplete property
	this.GetAirportsAndAirportGroupsByDeparture = function (sTextBoxID) {
		var sText = int.f.GetValue(sTextBoxID);
		var iDepartureAirportID = int.f.GetValue('ddlDepartingFromID');

		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;
		int.ff.Call('=iVectorWidgets.SearchTool.AutoCompleteAirportDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID, int.f.GetValue(sTextBoxID + 'Script'), iDepartureAirportID, true);
	}

	this.GetPropertyName = function () {
		var iArrivingAtID = int.f.GetValue('hidArrivingAtID');
		int.ff.Call(
			'=iVectorWidgets.SearchTool.PropertyName',
			function (sValue) {
				int.f.SetValue('acpArrivingAtID', sValue);
			},
			iArrivingAtID
		);
	}

	//toggle advanced options
	this.ToggleAdvancedOptions = function () {
		if (!me.AdvancedOptionsShown) {
			int.f.Show('divSearch_Advanced');
			int.f.Show('aHideAdvancedOptions');
			int.f.Hide('aShowAdvancedOptions');
			int.f.AddClass('divSearch', 'advanced');
			me.AdvancedOptionsShown = true;
		}
		else if (me.AdvancedOptionsShown) {
			int.f.Hide('divSearch_Advanced');
			int.f.Show('aShowAdvancedOptions');
			int.f.Hide('aHideAdvancedOptions');
			int.f.RemoveClass('divSearch', 'advanced');
			me.AdvancedOptionsShown = false;
		}
		else {
			return false;
		}
	}

	this.ToggleAirportAutoComplete = function () {
		int.f.Toggle('divArrivingAtAirport');
		int.f.Toggle('divArrivingAtAirportAuto');

		int.f.Toggle('aShowAutoComplete');
		int.f.Toggle('aHideAutoComplete');
	}

	this.TogglePickUpDepotAutoComplete = function () {

		int.f.Toggle('ddlCarHirePickUpDepotID');
		int.f.Toggle('acpCarHirePickUpDepotID');

		int.f.Toggle('aShowPickUpDepotAutoComplete');
		int.f.Toggle('aHidePickUpDepotAutoComplete');
	}

	this.ToggleDropOffDepotAutoComplete = function () {

		int.f.Toggle('ddlCarHireDropOffDepotID');
		int.f.Toggle('acpCarHireDropOffDepotID');

		int.f.Toggle('aShowDropOffDepotAutoComplete');
		int.f.Toggle('aHideDropOffDepotAutoComplete');
	}

	//star rating
	this.SetRating = function () {
		int.f.SetValue('hidRating', 0);
		int.f.SetValueIf('hidRating', 5, int.cb.Checked('cb5Star'));
		int.f.SetValueIf('hidRating', 4, int.cb.Checked('cb4Star'));
		int.f.SetValueIf('hidRating', 3, int.cb.Checked('cb3Star'));
	}

	this.ReturnTransfer = function () {
		int.f.ShowIf(int.f.GetObject('fldDropOffDate'), int.cb.Checked('cbReturnTransfer'));
		int.f.SetValue('hidReturnTransfer', int.cb.Checked('cbReturnTransfer'));
	}

	this.SetTransferParentID = function () {
		sPickupType = int.f.GetValue('ddlPickupType');

		sDropoffType = int.f.GetValue('ddlDropOffType');

		int.f.SetValue('hidTransferPickupLocationID', int.f.GetValue('acp' + sPickupType + 'IDTransferPickupHidden'));

		int.f.SetValue('hidTransferDropOffLocationID', int.f.GetValue('acp' + sDropoffType + 'IDTransferDropOffHidden'));
	}

	this.SetPriorityPropertyID = function () {
	}

	this.SetTransferLabel = function () {
		var sPickupType = int.dd.GetValue('ddlPickupType');
		if (sPickupType == 'Airport') {
			int.f.SetHTML('lblArrivalTime', 'Flight Arrival Time');
		}
		else if (sPickupType == 'Property') {
			int.f.SetHTML('lblArrivalTime', 'Pick Up Time');
		}
	}

    this.RestrictCarHireGuests = function (maxGuests) {
        if (int.f.GetObject('ddlCarHirePassengers')) {
            var sOptions = 'Select the Number of Passengers|0#';
            for (var i = 1; i <= maxGuests; i++) {
                sOptions += i + '|' + i + '#';
            }
            int.dd.SetOptions('ddlCarHirePassengers', sOptions.slice(0, -1));
        }
    }

    this.SetCarHirePickupDate = function (date) {
        int.f.SetValue('txtCarHirePickUpDate', date);
    }
}

//Combines the cookie values with the default search values set on the server - Matt D
SearchTool.CombineDefaultValuesWithCookie = function (oCookie, oDefaultValues) {
	if (oCookie == undefined) oCookie = [];

	var oCombinedSearchValues = {
	};

	//Search values
	oCombinedSearchValues.SearchMode = oCookie['SearchMode'] != undefined && int.f.GetObject('li_SearchMode_' + oCookie['SearchMode']) != undefined ? oCookie['SearchMode'] : oDefaultValues.SearchMode;
	oCombinedSearchValues.DepartingFromID = oCookie['DepartingFromID'] != undefined ? oCookie['DepartingFromID'] : oDefaultValues.DepartingFromID;
	oCombinedSearchValues.ArrivingAtID = oCookie['ArrivingAtID'] != undefined ? oCookie['ArrivingAtID'] : oDefaultValues.ArrivingAtID;
	oCombinedSearchValues.DepartureDate = oCookie['DepartureDate'] != undefined ? oCookie['DepartureDate'] : oDefaultValues.DepartureDate;
	oCombinedSearchValues.Duration = oCookie['Duration'] != undefined ? oCookie['Duration'] : oDefaultValues.Duration;
	oCombinedSearchValues.MealBasisID = oCookie['MealBasisID'] != undefined ? oCookie['MealBasisID'] : oDefaultValues.MealBasisID;
	oCombinedSearchValues.Rating = oCookie['Rating'] != undefined ? oCookie['Rating'] : oDefaultValues.Rating;
    oCombinedSearchValues.PriorityPropertyID = oCookie['acpPriorityPropertyIDHidden'] != undefined ? oCookie['acpPriorityPropertyIDHidden'] : 0;

	//Rooms and passengers
	oCombinedSearchValues.Rooms = oCookie['Rooms'] != undefined ? oCookie['Rooms'] : oDefaultValues.Rooms;
	oCombinedSearchValues.RoomPassengers = oDefaultValues.RoomPassengers;
	//Set each rooms passengers
	for (var i = 1; i <= 3; i++) {
		oCombinedSearchValues.RoomPassengers[i - 1].Adults = oCookie['Adults_' + i] != undefined ? oCookie['Adults_' + i] : oDefaultValues.RoomPassengers[i - 1].Adults;
		oCombinedSearchValues.RoomPassengers[i - 1].Children = oCookie['Children_' + i] != undefined ? oCookie['Children_' + i] : oDefaultValues.RoomPassengers[i - 1].Children;
		oCombinedSearchValues.RoomPassengers[i - 1].Infants = oCookie['Infants_' + i] != undefined ? oCookie['Infants_' + i] : oDefaultValues.RoomPassengers[i - 1].Infants;

		//set child ages
		if (oCombinedSearchValues.RoomPassengers[i - 1].Children > 0) {
			oCombinedSearchValues.RoomPassengers[i - 1].ChildAges = '';
			for (var j = 1; j <= 4; j++) {
				if (j == 1) {
					oCombinedSearchValues.RoomPassengers[i - 1].ChildAges += oCookie['ChildAge_' + i + '_' + j] != undefined ? oCookie['ChildAge_' + i + '_' + j] : 0;
				}
				else {
					var sAge = oCookie['ChildAge_' + i + '_' + j] != undefined ? oCookie['ChildAge_' + i + '_' + j] : 0;
					oCombinedSearchValues.RoomPassengers[i - 1].ChildAges = oCombinedSearchValues.RoomPassengers[i - 1].ChildAges + '#' + sAge;
				};
			}
		}
		else {
			oCombinedSearchValues.RoomPassengers[i - 1].ChildAges = '0#0#0#0';
		}
	}

	//other search fields
	oCombinedSearchValues.PickupDate = oCookie['PickupDate'] != undefined ? oCookie['PickupDate'] : oDefaultValues.PickupDate;
	oCombinedSearchValues.ArrivalTime = oCookie['ArrivalTime'] != undefined ? oCookie['ArrivalTime'] : oDefaultValues.ArrivalTime;
	oCombinedSearchValues.DropOffTime = oCookie['DropOffTime'] != undefined ? oCookie['DropOffTime'] : oDefaultValues.DropOffTime;
	oCombinedSearchValues.DropOffDate = oCookie['DropOffDate'] != undefined ? oCookie['DropOffDate'] : oDefaultValues.DropOffDate;

	//setup transfer default values
	oCombinedSearchValues.TransferPickupLocationID = oCookie['TransferPickupLocationID'] != undefined ? oCookie['TransferPickupLocationID'] : oDefaultValues.TransferPickupLocationID;
	oCombinedSearchValues.PickupType = oCookie['PickupType'] != undefined ? oCookie['PickupType'] : oDefaultValues.PickupType;
	oCombinedSearchValues.TransferDropOffLocationID = oCookie['TransferDropOffLocationID'] != undefined ? oCookie['TransferDropOffLocationID'] : oDefaultValues.TransferDropOffLocationID;
	oCombinedSearchValues.DropOffType = oCookie['DropOffType'] != undefined ? oCookie['DropOffType'] : oDefaultValues.DropOffType;
	oCombinedSearchValues.ReturnTransfer = oCookie['ReturnTransfer'] != undefined ? oCookie['ReturnTransfer'] : oDefaultValues.ReturnTransfer;

	oCombinedSearchValues.CarHireCountryID = oCookie['CarHireCountryID'] != undefined ? oCookie['CarHireCountryID'] : oDefaultValues.CarHireCountryID;
	oCombinedSearchValues.CarHirePickUpDepotID = oCookie['CarHirePickUpDepotID'] != undefined ? oCookie['CarHirePickUpDepotID'] : oDefaultValues.CarHirePickUpDepotID;
	oCombinedSearchValues.CarHireDropOffDepotID = oCookie['CarHireDropOffDepotID'] != undefined ? oCookie['CarHireDropOffDepotID'] : oDefaultValues.CarHireDropOffDepotID;
	oCombinedSearchValues.CarHirePickUpDate = oCookie['CarHirePickUpDate'] != undefined ? oCookie['CarHirePickUpDate'] : oDefaultValues.CarHirePickUpDate;
	oCombinedSearchValues.CarHirePickUpTime = oCookie['CarHirePickUpTime'] != undefined ? oCookie['CarHirePickUpTime'] : oDefaultValues.CarHirePickUpTime;
	oCombinedSearchValues.CarHireDropOffDate = oCookie['CarHireDropOffDate'] != undefined ? oCookie['CarHireDropOffDate'] : oDefaultValues.CarHireDropOffDate;
	oCombinedSearchValues.CarHireDropOffTime = oCookie['CarHireDropOffTime'] != undefined ? oCookie['CarHireDropOffTime'] : oDefaultValues.CarHireDropOffTime;
	oCombinedSearchValues.CarHireDriverSellingCountryID = oCookie['CarHireDriverBookingCountryID'] != undefined ? oCookie['CarHireDriverBookingCountryID'] : oDefaultValues.CarHireDriverSellingCountryID;
	oCombinedSearchValues.CarHireDriverAge = oCookie['CarHireDriverAge'] != undefined ? oCookie['CarHireDriverAge'] : oDefaultValues.CarHireDriverAge;
	oCombinedSearchValues.CarHirePassengers = oCookie['CarHirePassengers'] != undefined ? oCookie['CarHirePassengers'] : oDefaultValues.CarHirePassengers;
	oCombinedSearchValues.HighRatedHotelFilter = oCookie['HighRatedHotelFilter'] != undefined ? oCookie['HighRatedHotelFilter'] : oDefaultValues.HighRatedHotelFilter;

	return oCombinedSearchValues
};

SearchTool.HasLogger = function () {
    return typeof Logger != "undefined";
}
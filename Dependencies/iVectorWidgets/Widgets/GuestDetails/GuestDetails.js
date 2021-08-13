var GuestDetails = GuestDetails || {};


GuestDetails.CompleteFunction = null;


GuestDetails.Validate = function (oCompleteFunction) {

	web.InfoBox.Close();

	GuestDetails.CompleteFunction = oCompleteFunction;
	GuestDetails.WarningMessage = "";
    GuestDetails.ValidateTitles();
	GuestDetails.ValidateNames();
	GuestDetails.ValidateDOBs();

	GuestDetails.CheckForErrors();
};


//#region Validate Support

GuestDetails.CheckForErrors = function () {
	var aErrors = int.f.GetElementsByClassName('', 'error', 'divGuestDetails');
	if (aErrors.length == 0) {
		GuestDetails.AddToBasket();
	}
	else {
		if (GuestDetails.CompleteFunction != null && GuestDetails.CompleteFunction != undefined) GuestDetails.CompleteFunction();
	};
};

GuestDetails.ShowWarning = function (aErrors) {
	//get warning messages
	var MissingFieldsWarning = int.f.GetValue('hidGuestDetails_MissingFields');
	var DOBWarning = int.f.GetValue('hidGuestDetails_DOBWarning');

	//if there is a DOB warning see if DOB error
	var aDOB = new Array();
	if (DOBWarning != '') {		
		for (var i = 0; i < aErrors.length; i++) {
			if (int.s.StartsWith(aErrors[i].id, 'ddlDOB')) aDOB.push(aErrors[i]);
		};
	};

	//Show appropraiate warning
	if (aDOB.length > 0 && aDOB.length == aErrors.length) {
		web.InfoBox.Show(DOBWarning, 'warning');	
	} else if (aErrors.length > 0 && aDOB.length > 0 && aDOB.length != aErrors.length) {
		web.InfoBox.Show(MissingFieldsWarning + '</br>' + DOBWarning, 'warning');
	} else if (aDOB.length == 0 && aErrors.length > 0) {
		web.InfoBox.Show(MissingFieldsWarning, 'warning');
	};
};

GuestDetails.ValidateNames = function () {

	//validate first names
	GuestDetails.ValidateNameType('txtGuestDetailsFirstName_', 'hisGuestDetails_FirstNamePlaceholder');

	//validate last names
	GuestDetails.ValidateNameType('txtGuestDetailsLastName_', 'hisGuestDetails_LastNamePlaceholder');

};


GuestDetails.ValidateNameType = function (InputIDPrefix, HiddenPlaceholderID) {
	//get name inputs with type prefix
	var NameInputs = int.f.GetObjectsByIDPrefix(InputIDPrefix, 'input', 'divGuestDetails');
	//validate each name
	NameInputs.forEach(function (NameInput) {
		var Name = int.f.GetValue(NameInput);
		int.f.SetClassIf(NameInput, 'error', Name == '' || Name == int.f.GetValue(HiddenPlaceholderID) || !int.v.IsAlpha(Name));
	});
};


GuestDetails.ValidateDOBs = function () {
	//if DOB is required
	if (int.f.GetValue('hidGuestDetails_RequiresDOB') == 'true' || int.f.GetValue('hidGuestDetails_RequiresInfantDOB') == 'true') {
		//get DOB ddls
		var Dropdowns = int.f.GetObjectsByIDPrefix('ddlDOB', 'select', 'divGuestDetails');
		//validate that they are all non zero
		Dropdowns.forEach(function (Dropdown) {

		    var oElement = Dropdown;

		    if (int.f.HasClass(Dropdown.parentNode, 'custom-select')) {
		        oElement = Dropdown.parentNode;
		    }

		    int.f.SetClassIf(oElement, 'error', int.dd.GetIntValue(Dropdown) == 0);
		});
	};
};

GuestDetails.ValidateTitles = function() {
    var aTitleDropdowns = int.f.GetObjectsByIDPrefix('ddlGuestDetailsTitle', 'select', 'divGuestDetails');
    aTitleDropdowns.forEach(function (Dropdown) {
        var oElement = Dropdown;

        if (int.f.HasClass(Dropdown.parentNode, 'custom-select')) {
            oElement = Dropdown.parentNode;
        }

        int.f.SetClassIf(oElement, 'error', int.dd.GetValue(Dropdown) === '');
    });
};

GuestDetails.ValidateChildAges = function () {
	var CompareDate = GuestDetails.TryGetFirstDepartureDateOtherwiseTodaysDate();
	//get children to validate
	var oChildDOBSpans = int.f.GetElementsByClassName('span', 'childDOB', 'divGuestDetails');
	//validate each child
	oChildDOBSpans.forEach(function (oChildDOBSpan) {
		var IDSuffix = oChildDOBSpan.id.slice(oChildDOBSpan.id.indexOf('_'));
		GuestDetails.ValidateChildInfantAges(IDSuffix, CompareDate);
	});
};


GuestDetails.ValidateInfantAges = function () {
	var CompareDate = GuestDetails.TryGetFirstDepartureDateOtherwiseTodaysDate();
	//get infants to validate
	var oInfantDOBSpans = int.f.GetElementsByClassName('span', 'infantDOB', 'divGuestDetails');
	//validate each infant
	oInfantDOBSpans.forEach(function (oInfantDOBSpan) {
		var IDSuffix = oInfantDOBSpan.id.slice(oInfantDOBSpan.id.indexOf('_'));
		GuestDetails.ValidateChildInfantAges(IDSuffix, CompareDate);
	});

};


GuestDetails.ValidateChildInfantAges = function (IDSuffix, CompareDate) {

	var sDOBDay = int.dd.GetValue('ddlDOBDay' + IDSuffix);
	var sDOBMonth = int.dd.GetValue('ddlDOBMonth' + IDSuffix);
	var sDOBYear = int.dd.GetValue('ddlDOBYear' + IDSuffix);

	if (sDOBDay != "" && sDOBMonth != "" && sDOBYear != "" && CompareDate != "") {

		var DOBAge = GuestDetails.CalculateAge(new Date(sDOBYear, sDOBMonth - 1, sDOBDay), CompareDate);

		//try get age searched
		var SearchedAge = int.f.SafeObject('pChildAge' + IDSuffix) == undefined ? 0 : parseInt(int.f.SafeObject('pChildAge' + IDSuffix).innerText);

		//set class error if ages dont match
		if ((SearchedAge > 1 && DOBAge != SearchedAge) || (SearchedAge == 0 && DOBAge > 1)) {
			int.f.SetClass('ddlDOBDay' + IDSuffix, 'error');
			int.f.SetClass('ddlDOBMonth' + IDSuffix, 'error');
			int.f.SetClass('ddlDOBYear' + IDSuffix, 'error');			
		};
	};
};

//#endregion


//#region Support

GuestDetails.TryGetFirstDepartureDateOtherwiseTodaysDate = function () {

	var dDate = new Date();

	//get string first departure date
	var sFirstDepartureDate = int.f.GetValue('hidGuestDetails_FirstDepartureDate');
	if (sFirstDepartureDate != '') {
		var d = sFirstDepartureDate.substring(0, 2);
		var m = sFirstDepartureDate.substring(3, 5);
		var y = sFirstDepartureDate.substring(6, 10);
		dDate = new Date(y, m, d);
	};
	dDate.setHours(0, 0, 0, 0);
	return dDate;
};


GuestDetails.CalculateAge = function (DOB, CompareDate) {
	var Age = CompareDate.getFullYear() - DOB.getFullYear();
	var m = CompareDate.getMonth() - DOB.getMonth();
	if (m < 0 || (m === 0 && CompareDate.getDate() < DOB.getDate())) {
		Age--;
	};
	return Age;
};


GuestDetails.CapitaliseFirstLetter = function (oTextBox) {
	var sString = oTextBox.value;
	var sCapital = int.s.Left(sString, 1);
	var sRemainder = int.s.Substring(sString, 1, sString.length);

	int.f.SetValue(oTextBox, sCapital.toUpperCase() + sRemainder);
};


GuestDetails.CopyLastName = function (iRoomNumber, iGuestNumber) {
	var oFirstInput = int.f.SafeObject('txtGuestDetailsLastName_' + iRoomNumber + '_' + '1');
	var oNewInput = int.f.SafeObject('txtGuestDetailsLastName_' + iRoomNumber + '_' + iGuestNumber);
	if (oFirstInput.value != '') int.f.SetValue(oNewInput, oFirstInput.value);
};


GuestDetails.ToggleCollapse = function (otd) {
	atds = int.f.GetElementsByClassName('td', 'collapsible', otd.parentElement);
	int.f.ToggleClass(otd, 'collapsed');
	for (var i = 0; i < atds.length; i++) {
		int.f.Toggle(atds[i]);
		int.f.ToggleClass(atds[i], 'hidden');
	};
};

//#endregion


//#region Setup

GuestDetails.SetupPlaceholders = function () {

	//attach first name placeholders
	GuestDetails.AttachPlaceholders('txtGuestDetailsFirstName_', 'hisGuestDetails_FirstNamePlaceholder');

	//attach middle name placeholders
	GuestDetails.AttachPlaceholders('txtGuestDetailsMiddleName_', 'hisGuestDetails_MiddleNamePlaceholder');

	//attach last name placeholders
	GuestDetails.AttachPlaceholders('txtGuestDetailsLastName_', 'hisGuestDetails_LastNamePlaceholder');

};


GuestDetails.AttachPlaceholders = function (InputIDPrefix, HiddenPlaceholderID) {
	//get inputs with input prefix
	var Inputs = int.f.GetObjectsByIDPrefix(InputIDPrefix, 'input', 'divGuestDetails');
	//attach hidden placeholder values to those inputs
	Inputs.forEach(function (Input) {
		web.Placeholder.AttachEvents(Input, int.f.GetValue(HiddenPlaceholderID));
	});
};

//#endregion


//#region AddToBasket

GuestDetails.AddToBasket = function () {
	var oCurrentSearchGuestDetails = {
		RequiresDOB: int.f.GetValue('hidGuestDetails_RequiresDOB') == 'true',
        RequiresInfantDOB: int.f.GetValue('hidGuestDetails_RequiresInfantDOB') == 'true',
        RequiresLeadDriver: int.f.GetValue('hidGuestDetails_RequireLeadDriver').toLowerCase() == 'true',
		Rooms: []
    };

    var iLeadDriverID;

    if (oCurrentSearchGuestDetails.RequiresLeadDriver) {
        iLeadDriverID = int.f.GetRadioButtonValue('lead-guest');
    }
	for (iRoomCount = int.f.GetIntValue('hidGuestDetails_RoomCount'), a = 1; a <= iRoomCount; a++) {
		var oRoom = {
			RoomNumber: a,
			Guests: []
		};
		for (iRoomPaxCount = int.f.GetIntValue('hidGuestDetails_PaxCount_' + a), b = 1; b <= iRoomPaxCount; b++) {
			var oGuest = {
				GuestType: int.f.GetValue('hidGuestDetailsType_' + a + '_' + b),
				Title: int.dd.GetValue('ddlGuestDetailsTitle_' + a + '_' + b),
				FirstName: int.f.GetValue('txtGuestDetailsFirstName_' + a + '_' + b),
				MiddleName: int.f.GetValue('txtGuestDetailsMiddleName_' + a + '_' + b),
				LastName: int.f.GetValue('txtGuestDetailsLastName_' + a + '_' + b)
			};
			if (oCurrentSearchGuestDetails.RequiresDOB || (oCurrentSearchGuestDetails.RequiresInfantDOB && oGuest.GuestType == 'Infant')) {
				var dDOB = int.d.New(int.dd.GetIntValue('ddlDOBDay_' + a + '_' + b), int.dd.GetIntValue('ddlDOBMonth_' + a + '_' + b), int.dd.GetIntValue('ddlDOBYear_' + a + '_' + b));
				//strip timezone info
				int.d.TimeZoneSafeDate(dDOB);
				oGuest.DateOfBirth = dDOB;
            };
            if (oCurrentSearchGuestDetails.RequiresLeadDriver && iLeadDriverID === (a + '_' + b)) {
                oGuest.LeadDriver = true;
            }
			oRoom.Guests.push(oGuest);
		};
		oCurrentSearchGuestDetails.Rooms.push(oRoom);
	};
	int.ff.Call('=iVectorWidgets.GuestDetails.AddToBasket', GuestDetails.AddToBasketComplete, JSON.stringify(oCurrentSearchGuestDetails));
};


GuestDetails.AddToBasketComplete = function (sReturn) {
	if (sReturn == 'incomplete') {
		web.InfoBox.Show('Please fill in all fields to continue', 'warning');
	}
	else {
		try {
			var oReturn = JSON.parse(sReturn);
			if (oReturn.Success) {
				if (GuestDetails.CompleteFunction != null && GuestDetails.CompleteFunction != undefined) GuestDetails.CompleteFunction();
			}
			else {
				for (var i = 0; i < oReturn.Warnings.length; i++) web.InfoBox.Show(oReturn.Warnings[i], 'warning');
			};
		}
		catch (sException) {
			web.InfoBox.Show('Sorry, an unexpected error occurred.', 'warning');
		};
	};
};

//#endregion


$(document).ready(function () {
	GuestDetails.SetupPlaceholders();
});
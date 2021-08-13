var APIS = APIS || {};


APIS.Setup = function () {

    APIS.SetupDatePickers();

    APIS.AttachValidate();

    APIS.AttachCollapsePassenger();

};

APIS.SetupDatePickers = function () {
    //dates
    var dToday = new Date;
    var dDisplayDate = int.d.DatepickerDate(dToday);
    var dMinDate = new Date(2000, 1, 1);

    //get datepicker fields
    var oDatePickerInputs = int.f.GetElementsByClassName('INPUT', 'calendar', 'divAPIS');
    for (var i = 0; i < oDatePickerInputs.length; i++) {

        var oRestrictions = APIS.GetDatePickerRestrictions(oDatePickerInputs[i]);

        var oParams = {
            ID: '#' + oDatePickerInputs[i].id,
            Class: oDatePickerInputs[i].id,
            UseShortDates: true,
            LanguageCode: '',
            ConstrainInput: false,
            MaxDate: oRestrictions.MaxDate,
            MinDate: oRestrictions.MinDate,
            DefaultDate: oRestrictions.DefaultDate,
            changeYear: true,
            changeMonth: true,
            yearRange: 'c-110:c+11',
            BeforeShowFunction: function (input, inst) {
                setTimeout(function () {
                    inst.dpDiv.css({
                        position: 'absolute',
                        'z-index': 100004
                    });
                }, 0);
            }
        }
        web.DatePicker.SetupFromObject(oParams);

        int.f.AttachEvent(oDatePickerInputs[i],
                          'blur',
                          function () { return APIS.SetFieldDate(this) });
    };
};

APIS.GetDatePickerRestrictions = function(datepicker) {

    var oRestrictions = {};

    if ((` ${datepicker.className} `).indexOf(" date-max-yesterday ") > -1) {
        var oDate = new Date();
        oDate.setDate(oDate.getDate() - 1);
        oRestrictions.MaxDate = oDate;
        oRestrictions.DefaultDate = oDate;
    }

    if ((` ${datepicker.className} `).indexOf(" date-min-tomorrow ") > -1) {
        var oDate = new Date();
        oDate.setDate(oDate.getDate() + 1);
        oRestrictions.MinDate = oDate;
        oRestrictions.DefaultDate = oDate;
    }

    return oRestrictions;
};

APIS.SetFieldDate = function(dateField) {
    var date = int.f.GetValue(dateField);
    var formattedDate = APIS.FormatDate(date);
    int.f.SetValue(dateField, formattedDate);
};

APIS.DateStringContainsSeparators = function(dateString) {
    var containsSeparators = false;
    var validSeparators = ['/', '.', '-'];
    for (var i = 0; i < validSeparators.length; i++) {
        var separator = validSeparators[i];
        if (dateString.indexOf(separator) !== -1) {
            containsSeparators = true;
        }
    }
    return containsSeparators;
}
APIS.FormatDate = function (dateString) {
    var containsSeparators = this.DateStringContainsSeparators(dateString);
    var date = '';
    var dateLength = dateString.length;
    switch (dateLength) {
        case 6:
            date = APIS.FormatDateNoSeparator(dateString);
            break;
        case 8: {
            if (containsSeparators) {
                date = APIS.FormatDateWithSeparator(dateString);
            } else {
                date = APIS.FormatDateNoSeparator(dateString);
            }
        }
        case 10: {
            if (containsSeparators) {
                date = APIS.FormatDateWithSeparator(dateString);
            }
        }
        default:
    }
    return date;
};

APIS.GetFullYear = function(year) {
    var fullyear = new Date(year, 1, 1).getFullYear();
    var returnYear = year;
    if (fullyear <= (new Date().getFullYear() - 90)) {
        returnYear = fullyear + 100;
    } else {
        returnYear = fullyear;
    }
    return returnYear;
}

APIS.FormatDateWithSeparator = function(dateString) {
    var year = dateString.substring(6);
    var month = dateString.substring(3, 5);
    var day = dateString.substring(0, 2);
    if (year.length === 2) {
        year = this.GetFullYear(year);
    }
    return day + '/' + month + '/' + year;
};

APIS.FormatDateNoSeparator = function(dateString) {
    var year = dateString.substring(4);
    var month = dateString.substring(2, 4);
    var day = dateString.substring(0, 2);
    year = this.GetFullYear(year);
    return day + '/' + month + '/' + year;
};

APIS.AttachValidate = function () {
    //get buttons and attach validate function
    var oSubmitButtons = int.f.GetObjectsByIDPrefix('btnUpdateApis_', 'a', 'divAPIS');
    for (var i = 0; i < oSubmitButtons.length; i++) {
        int.f.AttachEvent(oSubmitButtons[i], 'click', APIS.Validate.bind(null, oSubmitButtons[i].id.replace('btnUpdateApis_', '')));
    };
};


APIS.AttachCollapsePassenger = function () {

    //get containing divs of each flight booking
    var oFlightBookings = APIS.GetObjectsByIDPrefixWithoutClass('APIS_', 'div', 'divAPIS', 'closed');

    //for each flight booking
    for (var i = 0; i < oFlightBookings.length; i++) {

        //get all passenger headings
        var oPassengerHeadings = int.f.GetObjectsByIDPrefix(oFlightBookings[i].id, 'h2', oFlightBookings[i].id);

        //for each passenger heading
        for (var j = 0; j < oPassengerHeadings.length; j++) {

            //Attach event to PassengerHeading
            int.f.AttachEvent(oPassengerHeadings[j], 'click',
				APIS.TogglePassengerDivCollapse.bind(null, oPassengerHeadings[j].id)
			);

            //if first passenger details remove closed class
            if (int.s.EndsWith(oPassengerHeadings[j].id, '_1')) APIS.TogglePassengerDivCollapse(oPassengerHeadings[j].id);
        };
    };
}


APIS.TogglePassengerDivCollapse = function (sIDPrefix) {

    //find corresponding div		
    var oCorrespondingDiv = document.getElementById(sIDPrefix + '_Collapsable')

    //add or remove closed class on heading and div
    if (int.f.HasClass(oCorrespondingDiv, 'closed')) {
        int.f.RemoveClass(sIDPrefix, 'closed');
        int.f.RemoveClass(oCorrespondingDiv, 'closed');
    }
    else {
        int.f.AddClass(sIDPrefix, 'closed');
        int.f.AddClass(oCorrespondingDiv, 'closed');
    };
};


APIS.GetObjectsByIDPrefixWithoutClass = function (sPrefix, sTagName, oContainer, sClass) {

    oContainer = oContainer == undefined ? document : int.f.SafeObject(oContainer);
    if (sTagName == undefined) sTagName = 'input';

    var aObjects = new Array();
    var aElements = oContainer.getElementsByTagName(sTagName);

    for (var i = 0; i < aElements.length; i++) {
        if (int.s.StartsWith(aElements[i].id, sPrefix) && !int.f.HasClass(aElements[i].id, sClass)) aObjects.push(aElements[i]);
    }

    return aObjects;
}


APIS.Validate = function (sFlightDivNumber) {

    //get containing flight div 
    var sFlightDivID = 'APIS_' + sFlightDivNumber;
    var oFlightDiv = document.getElementById(sFlightDivID);

    //get all passenger divs
    var oPassengerDivs = int.f.GetObjectsByIDPrefix(sFlightDivID + '_', 'div', sFlightDivID);

    //validate each passenger's details
    for (var i = 0; i < oPassengerDivs.length; i++) {
        APIS.ValidatePassenger(oPassengerDivs[i].id);
    };

    var oErrors = int.f.GetElementsByClassName('*', 'error', 'divAPIS');
    //if no error ff dot call
    if (oErrors.length == 0) {

        //build JSON
        var aJSON = APIS.BuildJSON(oPassengerDivs);

        //Add booking References
        var oBookingReferences = {
            BookingReference: int.f.GetValue('hidAPISBookingReference'),
            FlightBookingReference: int.f.GetValue('hidAPISFlightBookingReference'),
            SupplierReference: int.f.GetValue('hidAPISSupplierReference')
        };

        var sBookingReferencesJSON = JSON.stringify(oBookingReferences);

        //Show and hide	
        int.f.Hide('divAPISWarning');
        int.f.Hide('divAPIS');
        int.f.Show('divAPISWaitMessage');

        int.ff.Call('Widgets.APIS.SubmitApisInformation',
						function (bSuccess) {
						    APIS.SubmitApisInformationComplete(bSuccess);
						},
						aJSON, sBookingReferencesJSON);
    }
    else { //if error 		
        int.f.Show('divAPISWarning');
    };

};


APIS.BuildJSON = function (oPassengerDivs) {

    function ApisPassenger() {

        this.Title;
        this.FirstName;
        this.MiddleName;
        this.LastName;
        this.DateOfBirth;
        this.Nationality;
        this.NationalityCode;
        this.PassportNumber;
        this.PassportIssueDate;
        this.PassportExpiryDate;
        this.PassportIssuePlaceID;
        this.PassportIssuePlaceName;
        this.FlightBookingPassengerID;
        this.Gender;

    };

    var aPax = [];

    for (var i = 0; i < oPassengerDivs.length; i++) {

        var person = new ApisPassenger();
        person.FlightBookingPassengerID = int.n.SafeInt(int.f.GetValue(int.f.GetObjectsByIDPrefix('APISPassengerID', 'INPUT', oPassengerDivs[i].id)[0]));
        person.Title = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISTitle', 'INPUT', oPassengerDivs[i].id)[0]);
        person.FirstName = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISFirstName', 'INPUT', oPassengerDivs[i].id)[0]);
        person.MiddleName = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISMiddleName', 'INPUT', oPassengerDivs[i].id)[0]);
        person.LastName = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISLastName', 'INPUT', oPassengerDivs[i].id)[0]);

        person.DateOfBirth = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISDateOfBirth', 'INPUT', oPassengerDivs[i].id)[0]);
        person.Gender = int.f.GetRadioButtonValue(int.f.GetObjectsByIDPrefix('APISGender', 'INPUT', oPassengerDivs[i].id)[0].name);
        person.PassportNumber = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISPassportNumber', 'INPUT', oPassengerDivs[i].id)[0]);
        person.PassportIssueDate = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISPassportIssueDate', 'INPUT', oPassengerDivs[i].id)[0]);
        person.PassportExpiryDate = int.f.GetValue(int.f.GetObjectsByIDPrefix('APISPassportExpiryDate', 'INPUT', oPassengerDivs[i].id)[0]);

        var oNationality = int.f.GetObjectsByIDPrefix('APISNationality', 'select', oPassengerDivs[i].id)[0];
        person.Nationality = oNationality.options[oNationality.selectedIndex].text;
        person.NationalityCode = oNationality.options[oNationality.selectedIndex].value;

        var oPassportIssuePlace = int.f.GetObjectsByIDPrefix('APISPassportIssuePlace', 'select', oPassengerDivs[i].id)[0];;
        person.PassportIssuePlaceID = int.n.SafeInt(oPassportIssuePlace.options[oPassportIssuePlace.selectedIndex].value);
        person.PassportIssuePlaceName = oPassportIssuePlace.options[oPassportIssuePlace.selectedIndex].text;

        aPax.push(person);
    };

    return JSON.stringify(aPax);
};


APIS.SubmitApisInformationComplete = function (bSuccess) {
    if (bSuccess == 'True') {
        int.f.Hide('divAPISWaitMessage');
        int.f.Show('divAPISRequestSent');
    }
    else {
        int.f.Hide('divAPISWaitMessage');
        int.f.Show('divAPISRequestNotSent');
    }
}


APIS.ValidatePassenger = function (sPassengerDivID) {

    //remove error class
    var oErrors = int.f.GetElementsByClassName('*', 'error', sPassengerDivID);
    oErrors.forEach(function (oElement) {
        int.f.RemoveClass(oElement, 'error');
    });

    //get elements to validate
    var eles = int.f.GetElementsByClassName('', 'validate', sPassengerDivID);

    //validate elements
    eles.forEach(function (oElement) {
        APIS.ValidateInputs(oElement);
    });

};


APIS.ValidateInputs = function (oElement) {
    switch (oElement.type) {
        case 'select-one':
            if (int.f.HasClass(oElement.parentNode, 'custom-select')) {
                int.f.SetClassIf(oElement.parentNode, 'error', oElement.value == '');
            } else {
                int.f.SetClassIf(oElement, 'error', oElement.value == '');
            }
            break;
        case 'radio':
            int.f.SetClassIf(oElement.parentNode, 'error', int.f.GetRadioButtonValue(oElement.name) == '');
            break;
        case 'text':
            if (oElement.id == 'APISMiddleName') {
                int.f.SetClassIf(oElement, 'error', !int.v.IsAlpha(oElement.value));
            }
            else if (oElement.id == 'APISPassportNumber') {
                var oAlphaNumericNotBlankRegex = new RegExp(/^[a-zA-Z0-9]+$/);
                int.f.SetClassIf(oElement, 'error', !oAlphaNumericNotBlankRegex.test(oElement.value));
            }
            else if (int.f.HasClass(oElement, 'calendar')) {
                int.f.SetClassIf(oElement, 'error', !int.v.IsValidDate(oElement.value));
            }
            break;
    };
};


$(document).ready(function () {
    APIS.Setup();
});
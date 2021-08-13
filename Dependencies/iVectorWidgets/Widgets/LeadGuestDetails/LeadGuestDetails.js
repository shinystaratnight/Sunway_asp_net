var LeadGuestDetails = LeadGuestDetails || {};


	LeadGuestDetails.CompleteFunction = null;


	LeadGuestDetails.Validate = function (oCompleteFunction) {

		web.InfoBox.Close();


		//get exclude items
		var aExcludeItems = int.f.GetValue('hidLeadGuestDetails_ValidationExclude').split(',');


		//set completefunction
		LeadGuestDetails.CompleteFunction = oCompleteFunction;

		if (!int.f.GetObject('divLeadGuestDetails')) {
		    LeadGuestDetails.AddDetailsToBasketComplete();
		    return;
		}

		//title
		int.f.SetClassIf('ddlLeadGuestDetails_Title', 'error', int.dd.GetIndex('ddlLeadGuestDetails_Title') == 0);


		//names
		var sFirstName = int.f.GetValue('txtLeadGuestDetails_FirstName');
		int.f.SetClassIf('txtLeadGuestDetails_FirstName', 'error', sFirstName == '' || !int.v.IsAlpha(sFirstName));

		var sLastName = int.f.GetValue('txtLeadGuestDetails_LastName');
		int.f.SetClassIf('txtLeadGuestDetails_LastName', 'error', sLastName == '' || !int.v.IsAlpha(sLastName));


		//email
		int.f.SetClassIf('txtLeadGuestDetails_Email', 'error', int.v.IsEmail(int.f.GetValue('txtLeadGuestDetails_Email')) == false);
		int.f.SetClassIf('txtLeadGuestDetails_RepeatEmail', 'error', int.v.IsEmail(int.f.GetValue('txtLeadGuestDetails_RepeatEmail')) == false
			|| int.f.GetValue('txtLeadGuestDetails_RepeatEmail') != int.f.GetValue('txtLeadGuestDetails_Email'));


		//address
		int.f.SetClassIf('txtLeadGuestDetails_Address', 'error', int.f.GetValue('txtLeadGuestDetails_Address') == '');

		//date of birth details
		if (int.f.GetObject('ddlDOBDay')) {
			int.f.SetClassIf('ddlDOBDay', 'error', int.f.GetValue('ddlDOBDay') == '');
		}
		if (int.f.GetObject('ddlDOBMonth')) {
			int.f.SetClassIf('ddlDOBMonth', 'error', int.f.GetValue('ddlDOBMonth') == '');
		}
		if (int.f.GetObject('ddlDOBYear')) {
			int.f.SetClassIf('ddlDOBYear', 'error', int.f.GetValue('ddlDOBYear') == '');
		}

		//postcode
		if (int.f.GetObject('txtLeadGuestDetails_Postcode') && !int.a.ArrayContains(aExcludeItems, 'Postcode')) {
			int.f.SetClassIf('txtLeadGuestDetails_Postcode', 'error', int.f.GetValue('txtLeadGuestDetails_Postcode') == '');
		}

		//passport number
		if (int.f.GetObject('txtLeadGuestDetails_Passport')) {
			int.f.SetClassIf('txtLeadGuestDetails_Passport', 'error', int.f.GetValue('txtLeadGuestDetails_Passport') == '');
		}

		int.f.SetClassIf('txtLeadGuestDetails_City', 'error', int.f.GetValue('txtLeadGuestDetails_City') == '');
		int.f.SetClassIf('ddlLeadGuestDetails_BookingCountry', 'error', int.dd.GetIntValue('ddlLeadGuestDetails_BookingCountry') == 0);

		//phone number
		var sPhoneNo = int.f.GetValue('txtLeadGuestDetails_PhoneNumber');
		int.f.SetClassIf('txtLeadGuestDetails_PhoneNumber', 'error', sPhoneNo == '' || !int.v.IsNumericPhoneNumber(sPhoneNo));

        //mobile number
        var sMobNo = int.f.GetValue('txtLeadGuestDetails_MobileNumber');
        if (int.f.GetObject('txtLeadGuestDetails_MobileNumber') && !int.a.ArrayContains(aExcludeItems, 'mobile')) {
            int.f.SetClassIf('txtLeadGuestDetails_MobileNumber', 'error', sMobNo == '' || !int.v.IsNumericPhoneNumber(sMobNo));
        }

		//if valid add details to basket
		var iErrors = int.f.GetElementsByClassName('', 'error', 'divLeadGuestDetails').length
		if (iErrors == 0) {
			LeadGuestDetails.AddDetailsToBasket();
		}
		else {
			LeadGuestDetails.AddDetailsToBasketComplete();
		}

	}


	LeadGuestDetails.AddDetailsToBasket = function () {

		var oLeadGuestDetails = {
			CustomerTitle: int.dd.GetValue('ddlLeadGuestDetails_Title'),
			CustomerFirstName: int.f.GetValue('txtLeadGuestDetails_FirstName'),
			CustomerLastName: int.f.GetValue('txtLeadGuestDetails_LastName'),
			CustomerEmail: int.f.GetValue('txtLeadGuestDetails_Email'),
			CustomerAddress1: int.f.GetValue('txtLeadGuestDetails_Address'),
			CustomerAddress2: int.f.SafeObject('txtLeadGuestDetails_Address2') ? int.f.GetValue('txtLeadGuestDetails_Address2') : '',
			CustomerTownCity: int.f.GetValue('txtLeadGuestDetails_City'),
			CustomerCounty: int.f.SafeObject('txtLeadGuestDetails_County') ? int.f.GetValue('txtLeadGuestDetails_County') : '',
			CustomerPostcode: int.f.GetValue('txtLeadGuestDetails_Postcode'),
			CustomerBookingCountryID: int.dd.GetIntValue('ddlLeadGuestDetails_BookingCountry'),
			CustomerPhone: int.f.GetValue('txtLeadGuestDetails_PhoneNumber'),
			CustomerMobile: int.f.SafeObject('txtLeadGuestDetails_MobileNumber') ? int.f.GetValue('txtLeadGuestDetails_MobileNumber') : '',
			CustomerPassportNumber: int.f.SafeObject('txtLeadGuestDetails_Passport') ? int.f.GetValue('txtLeadGuestDetails_Passport') : '',
			DateOfBirth: int.f.SafeObject('ddlDOBDay') ? int.d.New(int.f.GetValue('ddlDOBDay'), int.f.GetValue('ddlDOBMonth'), int.f.GetValue('ddlDOBYear')) : int.d.New('1','1','1900')
		};
		
		//convert to JSON
		var sJSON = JSON.stringify(oLeadGuestDetails)

		//set details
		int.ff.Call('=iVectorWidgets.LeadGuestDetails.AddDetailsToBasket', function () { 
			LeadGuestDetails.AddDetailsToBasketComplete() 
		}, sJSON);

	}

	
	LeadGuestDetails.AddDetailsToBasketComplete = function () {
		if (LeadGuestDetails.CompleteFunction != null && LeadGuestDetails.CompleteFunction != undefined) {
			LeadGuestDetails.CompleteFunction();
		}
	}


	LeadGuestDetails.CapitaliseFirstLetter = function (oTextBox) {
		var sString = oTextBox.value;
		var sCapital = int.s.Left(sString, 1);
		var sRemainder = int.s.Substring(sString, 1, sString.length);

		int.f.SetValue(oTextBox, sCapital.toUpperCase() + sRemainder);

	}


	LeadGuestDetails.FindAddresses = function () {

		//get input	
		var sPostCode = int.f.GetValue('txtPostcodeLookup_Postcode');

		if (sPostCode != '') {
			//ff.call
			int.ff.Call('=iVectorWidgets.LeadGuestDetails.FindAddresses', LeadGuestDetails.FindAddressesReturn, sPostCode);
		}
	};


	LeadGuestDetails.FindAddressesReturn = function (sHTML) {

		//if found
		if (sHTML != '') {

			document.getElementById('ddlPostcodeLookup_Addresses').innerHTML = sHTML;
			document.getElementById('trPostcodeLookup_Select').style.display = 'block';

			int.f.AttachEvent('ddlPostcodeLookup_Addresses', 'change', function () {
				LeadGuestDetails.SelectAddress();
			});

			//else error?
		}
		else {
			web.ModalPopup.Show("<div><h3>No addresses found.</h3><p>Please check your postcode or enter your adddress manually.</p></div>", true, 'modalpopup PostcodeError');
			document.getElementById('trPostcodeLookup_Select').style.display = 'none';
			//attach overlay hide
			int.f.AttachEvent('divOverlay', 'click', function () {
				web.ModalPopup.Hide();
			});
		}
	};


	LeadGuestDetails.SelectAddress = function () {

		//get address
		var oAddresses = document.getElementById('ddlPostcodeLookup_Addresses');
		var sSelectedBuildingID = oAddresses.options[oAddresses.selectedIndex].value;

		if (sSelectedBuildingID != "0" && sSelectedBuildingID != null && sSelectedBuildingID != undefined) {
			int.ff.Call('=iVectorWidgets.LeadGuestDetails.SelectAddress', LeadGuestDetails.SelectAddressReturn, sSelectedBuildingID);
		};
	};


	LeadGuestDetails.SelectAddressReturn = function (sJSON) {

		var oReturn = JSON.parse(sJSON);

		//update address input
		if (oReturn.Address1 != '' && oReturn.Address2 != '') {
			document.getElementById('txtLeadGuestDetails_Address').value = oReturn.Address1 + ', ' + oReturn.Address2;
		}
		else if (oReturn.Address1 != '') {
			document.getElementById('txtLeadGuestDetails_Address').value = oReturn.Address1;
		};
		//update city input
		if (oReturn.TownCity != '') {
			document.getElementById('txtLeadGuestDetails_City').value = oReturn.TownCity;
		};
		//update postcode input
		if (oReturn.Postcode != '') {
			document.getElementById('txtLeadGuestDetails_Postcode').value = oReturn.Postcode;
		};
		//update country input
		if (oReturn.BookingCountryID != '') {
			document.getElementById('ddlLeadGuestDetails_BookingCountry').value = oReturn.BookingCountryID;
		};

	};


	LeadGuestDetails.Setup = function () {
		//If find address button exists attach event
		var aFind = document.getElementById('aPostcodeLookup_Find');
		if (aFind) {
			int.f.AttachEvent(aFind, 'click', function () {
				LeadGuestDetails.FindAddresses();
			});
		};

	};


	$(document).ready(function () {
		LeadGuestDetails.Setup();
	});
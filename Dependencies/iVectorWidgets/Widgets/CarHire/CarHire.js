var CarHire = CarHire || {}

	$('document').ready(function () {
		CarHire.Setup();

		if ($('#hidCarHireResultsPage').val = 'True') {
			CarHire.PreloadResultsSetup(true);
		}

	});

	CarHire.Setup = function () {

		//check if we need to auto search, if we do then hide the search tool and show the wait message
		//and obviously auto search
		var bAutoSearch = int.f.GetValue('hidAutoSearch');
		if (bAutoSearch == 'True') {
		    if (int.f.GetObject('divSelectedCar')) {
			var sSelectedCarHireHTML = int.f.GetObject('divSelectedCar').innerHTML;

			if (sSelectedCarHireHTML == '') {

				int.f.Hide('divCarHireSearch');
				int.f.Show('pSearchWait');

				CarHire.GetCarHireHTML(true);
			}
		}
		}

		$('#btnSearchAgain').on('click', function () {
			$('#divCarHireSearch').show();
			$('#btnSearchAgain').hide();
		});

		//Build the date picker params
		var sStartDate = int.f.GetValue('hidCarHirePickUpDate');
		var sEndDate = int.f.GetValue('hidCarHireDropOffDate');

		var iDatePickerMonths = 1;
        if (int.f.GetObject('hidDatePickerMonths')) {
            iDatePickerMonths = int.f.GetIntValue('hidDatePickerMonths');
        }

		var dStartDate = new Date(sStartDate.split('/')[2], sStartDate.split('/')[1] - 1, sStartDate.split('/')[0]);
		var dEndDate = new Date(sEndDate.split('/')[2], sEndDate.split('/')[1] - 1, sEndDate.split('/')[0]);

		$('#btnAddExtras').on('click', function () {
			CarHire.BookExtras();
		});

		var oParams = {
			ID: '#txtCarHirePickUpDate',
			defaultDate: dStartDate,
			MinDate: dStartDate,
			MaxDate: dEndDate,
			DatepickerMonths: iDatePickerMonths,
			Class: 'carHirePickupDate'
		};

		//Call the datepicker setup
		web.DatePicker.SetupFromObject(oParams);

		oParams = {
			ID: '#txtCarHireDropOffDate',
			defaultDate: dStartDate,
			MinDate: dStartDate,
			MaxDate: dEndDate,
			DatepickerMonths: iDatePickerMonths,
			Class: 'carHireDropOffDate'
		};

		web.DatePicker.SetupFromObject(oParams);

		//set the default driver country based on the site selling geo level 1
		int.dd.SetValue('ddlCarHireLeadDriverBookingCountryID', int.f.GetValue('hidDefaultDriverCountryID'));

	}

	CarHire.Search = function () {

		var search = new this.searchModel();
		var searchObj = null;
		$('select, input').removeClass('error');

		if (search.validate()) {
			searchObj = JSON.stringify(search.getData());
			$('#divCarHireResults').hide();
			$('#pSearchWait').show();
			int.ff.Call('=iVectorWidgets.CarHire.GetCarHireHTML', CarHire.GetCarHireHTMLDone, searchObj);
		} else {
			for (var i = 0; i < search.errors.length; i++) {
				$('#' + search.errors[i].element).addClass('error');
			}
			web.InfoBox.Show('Please check all information has been entered correctly and times are in the HH:MM format. Incorrect fields have been highlighted.', 'warning');
		}

	};

	/* validation not translation safe yet */
	CarHire.searchModel = function () {

		// defaults
		var data = {
			PickUpDepotID: 0,
			PickUpDate: new Date(),
			PickUpTime: '',
			DropOffDepotID: 0,
			DropOffDate: new Date(),
			DropOffTime: '',
			LeadDriverBookingCountryID: 0,
			TotalPassengers: 0,
			DriverAges: []
		}

		var mappings = {
			CountryID: {
				element: 'ddlCountryID',
				validate: function (val){
					var valid = false;
					if ($('#ddlCountryID').length > 0) {
						valid = val > 0;
					} else{
						valid = true;
					}

					return valid
				},
				error: {
					message: 'please select a country'
				}
			},
			PickUpDepotID: {
				element: 'ddlCarHirePickUpDepotID',
				validate: function (val) {
					return val > 0;
				},
				error: {
					message: 'please select a pick up depot'
				}
			},
			PickUpDate: {
				element: 'txtCarHirePickUpDate',
				validate: function (val) {
					var valid = false;
					var dateParts = val.split('/');
					// first check we have 3 parts split by slashes
					if (dateParts.length === 3) {
						// build date from the UK format we pass in 
						var nativeDate = new Date(dateParts[2], dateParts[1], dateParts[0]);
						valid = int.d.IsDate(nativeDate);
					}

					return valid;
				},
				error: {
					message: 'please select a valid pick up date'
				}
			},
			PickUpTime: {
				element: 'txtCarHirePickUpTime',
				validate: function (val) {
					return int.v.IsTime(val)
				},
				error: {
					message: 'please select a valid pick up time'
				}
			},
			DropOffDepotID: {
				element: 'ddlCarHireDropOffDepotID',
				validate: function (val) {
					return val > 0;
				},
				error: {
					message: 'please select a drop off depot'
				}
			},
			DropOffDate: {
				element: 'txtCarHireDropOffDate',
				validate: function (val) {

					var valid = false;
					var dateParts = val.split('/');
					// first check we have 3 parts split by slashes
					if (dateParts.length === 3) {
						// build date from the UK format we pass in 
						var nativeDate = new Date(dateParts[2], dateParts[1], dateParts[0]);
						valid = int.d.IsDate(nativeDate);
					}

					return valid;
				},
				error: {
					message: 'please select a valid drop off date'
				}
			},
			DropOffTime: {
				element: 'txtCarHireDropOffTime',
				validate: function (val) {
					return int.v.IsTime(val);
				},
				error: {
					message: 'please select a valid drop off time'
				}
			},
			LeadDriverBookingCountryID: {
				element: 'ddlCarHireLeadDriverBookingCountryID',
				validate: function (val) {
					return val > 0;
				},
				error: {
					message: 'please select a lead guest booking country'
				}
			},
			TotalPassengers: {
				element: 'ddlCarHireTotalPassengers',
				validate: function (val) {
					return val > 0;
				},
				error: {
					message: 'please select total passengers'
				}
			},
			DriverAges: {
				element: 'ddlCarHireDriverAge',
				validate: function (val) {
					return val > 0;
				},
				error: {
					message: 'please select main drivers age'
				}
			}
		}

		//build data
		for (prop in mappings) {
			var value = $('#' + mappings[prop].element).val();
			if (int.a.IsArray(data[prop])) {
				data[prop].push(value);
			} else {
				data[prop] = value;
			}
		}

		return {
			validate: function () {

				for (prop in data) {
					var currentItem = mappings[prop];
					if (!currentItem.validate(data[prop])) {
						this.errors.push({
							message: currentItem.error.message,
							element: currentItem.element
						}
						);
					}
				}

				return this.errors.length === 0;
			},
			getData: function () {
				return data;
			},
			errors: []
		}

	}


	CarHire.GetCarHireHTML = function (ShowSearchAgain) {

		var bShowSearchAgain = false;
		if (ShowSearchAgain) {
			bShowSearchAgain = true;
		}

		int.ff.Call('=iVectorWidgets.CarHire.GetCarHireHTML', function (sJSON) { CarHire.GetCarHireHTMLDone(sJSON, bShowSearchAgain); });

	};

	CarHire.GetCarHireHTMLDone = function (sJSON, bShowSearchAgain) {

		var oReturn = JSON.parse(sJSON);

		if (oReturn.Success) {
			int.f.SetHTML('divCarHireResults', oReturn.HTML, true);
			$('#pSearchWait').hide();
			$('#divCarHireResults').show();

			$('#btnAddCar').on('click', function () {
				CarHire.Select();
			});

			if (int.f.GetValue('hidUseBasketCarHire') != 'True') {
				$('#divSelectedCar').hide();
			}

			if (bShowSearchAgain) {
				$('#btnSearchAgain').show();
			}
		}
		else {
			if (int.f.GetValue('hidSearchWarning') != '') {
				int.f.Show('divCarHireResults');
				int.f.Show('divCarHireSearch');
				int.f.SetHTML('divCarHireResults', '<p class="noResults">' + int.f.GetValue('hidSearchWarning') + '</p>');
			}
			else {
				int.f.Show('divCarHireResults');
				int.f.Show('divCarHireSearch');
				int.f.SetHTML('divCarHireResults', '<p class="noResults">' + oReturn.Warning + '</p>');
			}
			$('#pSearchWait').hide();
		}

	}

	CarHire.PreloadResultsSetup = function (bShowSearchAgain) {

	    $('#pSearchWait').hide();

		$('#btnAddCar').on('click', function () {
			CarHire.Select();
		});

		if (int.f.GetValue('hidUseBasketCarHire') != 'True') {
			$('#divSelectedCar').hide();
		}

		if (bShowSearchAgain) {
			$('#btnSearchAgain').show();
		}

		CarHire.Bind();

	}

	CarHire.Bind = function () {

		var sJSON = $('#hidCarHireSearchJSON').val();
		if (sJSON != '' && sJSON != undefined) {
			var oCarHireSearch = JSON.parse(sJSON);
		}

	}

	CarHire.Select = function () {

		var aCars = int.f.GetObjectsByIDPrefix('rad_ch_', 'input', 'divCarHireResults');
		var token = '';

		//set the Car Hire Search ID
		for (i = 0; i < aCars.length; i++) {
			if ($(aCars[i]).is(':checked')) {
				token = aCars[i].id.split('_')[2];
			}
		}

		if (token) {
			CarHire.AddCar(token, null, CarHire.AddCarCompleteAlt);
		} else {
			web.InfoBox.Show('Please select a car', 'warning');
		}


	}

	CarHire.CheckboxSelect = function (chkCar) {

		var sID = chkCar.id;
		var sHashToken = chkCar.id.split('_')[2];
		var aCars = int.f.GetObjectsByIDPrefix('chk_ch_', 'input');

		if (int.cb.Checked(chkCar)) {
			//if the car is checked, add it to the basket. 
			//Basket is cleared by default currently - open to change in future if a client allows multiple cars to be selected
			CarHire.AddCar(sHashToken, sID, CarHire.AddCarComplete);
		}
		else {
			// if we're unticking then we are removing all car hires
			int.ff.Call('=iVectorWidgets.CarHire.RemoveCarHire', function (sReturn) { CarHire.RemoveCarComplete(sReturn); });
		}

	}

	CarHire.AddCar = function (HashToken, ID, callback) {

		int.ff.Call('=iVectorWidgets.CarHire.AddCarHireToBasket', function (sReturn) { callback(sReturn, ID); }, HashToken);

	}

	CarHire.AddCarComplete = function (sReturn, ID) {

		var oReturn = JSON.parse(sReturn);
		var aCars = int.f.GetObjectsByIDPrefix('chk_ch_', 'input', 'divCarHireResults');

		if (oReturn.Success) {
			//remove untick any options that arent the selected one		

			for (i = 0; i < aCars.length; i++) {
				if (aCars[i].id != ID) {
					int.f.RemoveClass(aCars[i], 'selected');
					int.cb.SetValue(aCars[i], 'false');
				}
			}
		}
		else {
			//if it fails to add to basket, untick everything
			for (i = 0; i < aCars.length; i++) {
				int.f.RemoveClass(aCars[i], 'selected');
				int.cb.SetValue(aCars[i], 'false');
			}
			web.InfoBox.Show(oReturn.Warnings, 'warning');
		}

		//update to basket
		Basket.UpdateBasketHTML();

	}

	// there should be one function that can deal with checkboxes and buttons - this is an interim measure
	CarHire.AddCarCompleteAlt = function (sReturn, ID) {

		var oReturn = JSON.parse(sReturn);


		if (oReturn.Success) {

		    if (oReturn.Redirect === true) {
		        window.location = oReturn.RedirectUrl;
		        return;
		    }

			$('#divCarHireResults').hide();
			int.ff.Call('=iVectorWidgets.CarHire.GetSelectedCarHireAndExtras', function (sReturn) { CarHire.GetSelectedCarDone(sReturn); });
		}
		else {
			web.InfoBox.Show(oReturn.Warnings, 'warning');
		}

		//update to basket
		Basket.UpdateBasketHTML();

	}

	CarHire.GetSelectedCarHireHTML = function () {
		int.ff.Call('=iVectorWidgets.CarHire.GetSelectedCarHireAndExtras', function (sReturn) { CarHire.GetSelectedCarDone(sReturn); });
	}

	CarHire.GetSelectedCarDone = function (html) {

		$('#divSelectedCar').html(html);
		$('#divSelectedCar').show();
		$('#btnAddExtras').on('click', function () {
			CarHire.BookExtras();
		});
		$('#btnRemoveCarHire').on('click', function () {
			CarHire.RemoveCarHire();
		});

	}

	CarHire.UpdateindividualCarHireExtra = function (ExtraOption, quantity) {
		var extra = [];

		extra.push(ExtraOption.id.split('_')[1] + '###' + quantity);

		int.ff.Call('=iVectorWidgets.CarHire.AddExtrasToBasket', function (sReturn) { CarHire.BookExtrasComplete(sReturn); }, JSON.stringify(extra));
	}

	CarHire.BookExtras = function () {
		var extras = [];
		var extraOptions = $('.bookableExtras select');
		for (i = 0; i < extraOptions.length; i++) {
			extras.push(extraOptions[i].id.split('_')[3] + '###' + extraOptions[i].value)
		}

		int.ff.Call('=iVectorWidgets.CarHire.AddExtrasToBasket', function (sReturn) { CarHire.BookExtrasComplete(sReturn); }, JSON.stringify(extras));
	}

	CarHire.BookExtrasComplete = function (sHTML) {

		CarHire.GetSelectedCarDone(sHTML);

		Basket.UpdateBasketHTML();

	}

	CarHire.UpdateExtras = function () {

		aCarHireExtras = int.f.GetObjectsByIDPrefix('ddl_ch_extra_', 'select', 'divSelectedCar');

		for (i = 0; i < aCarHireExtras.length; i++) {

			//get the extra price and quantity selected
			var iPrice = int.n.SafeInt(int.f.GetHTML('spnExtraPrice_' + i));
			var iQuantity = int.dd.GetValue(aCarHireExtras[i].id);

			var iTotal = iPrice * iQuantity

			//update front end
			int.f.SetHTML('spnCarHireExtraTotal_' + i, iTotal);

		}

		//update the basket
		CarHire.BookExtras();

	}

	CarHire.RemoveCarHire = function () {

		int.ff.Call('=iVectorWidgets.CarHire.RemoveCarHire', function (sReturn) { CarHire.RemoveCarComplete(sReturn); });

	}

	CarHire.RemoveCarComplete = function (sReturn) {

		var oReturn = JSON.parse(sReturn);

		if (oReturn.Success) {

			//check if we're using the extras functionality
			if (int.f.GetObject('divSelectedCar')) {
				int.f.Hide('divSelectedCar');
			}
			else {
				//remove class and unselect all options
				var aCars = int.f.GetObjectsByIDPrefix('chk_ch_', 'input');

				for (i = 0; i < aCars.length; i++) {
					int.f.RemoveClass(aCars[i], 'selected');
					int.cb.SetValue(aCars[i], 'false');
				}
			}

		}

		//update to basket
		Basket.UpdateBasketHTML();

	}

	CarHire.ShowMoreOptions = function () {

		var aCarOptions = int.f.GetElementsByClassName('tr', 'option', 'tblResults');
		for (i = 0; i < aCarOptions.length; i++) {
			int.f.Show(aCarOptions[i]);
		}

		int.f.Show('aHideExtraOptions');
		int.f.Hide('aShowExtraOptions');

	}

	CarHire.HideMoreOptions = function () {

		var aCarOptions = int.f.GetElementsByClassName('tr', 'option', 'tblResults');
		for (i = 0; i < aCarOptions.length; i++) {
			int.f.Hide(aCarOptions[i]);
		}

		int.f.Hide('aHideExtraOptions');
		int.f.Show('aShowExtraOptions');

	}

	CarHire.ShowInfo = function (sBookingToken) {

		//show the rental conditions popup
		web.ModalPopup.Show('divRentalConditionsWaiting');

		int.ff.Call('=iVectorWidgets.CarHire.InformationPrebook', function (sReturn) { CarHire.ShowInfoComplete(sReturn, sBookingToken); }, sBookingToken);

	}

	CarHire.ShowInfoComplete = function (sReturn, sBookingToken) {

		var oReturn = JSON.parse(sReturn);

		web.ModalPopup.Hide('divRentalConditionsWaiting');

		if (oReturn.Success) {
			web.ModalPopup.Show(oReturn.HTML, true, 'modalpopup rentalConditions');
		}
		else {
			web.ModalPopup.Show('<p class="error">' + oReturn.Warning + '</p>', true);
		}

	}


	CarHire.ResetForm = function (oCompleteFunction) {

		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divCarHire');
		for (var i = 0; i < aErrorControls.length; i++) {
			int.f.RemoveClass(aErrorControls[i], 'error');
		}

		if (oCompleteFunction != null && oCompleteFunction != undefined) oCompleteFunction();
	}

	CarHire.SelectCountry = function (CountryID) {
		if (CountryID > 0) {

			CarHire.SetDepotDropdown(CountryID);
		} else {
			int.dd.SetValue('ddlCarHirePickUpDepotID', '');
			int.f.GetObject('ddlCarHirePickUpDepotID').disabled = true;
			int.dd.SetValue('ddlCarHireDropOffDepotID', '');
			int.f.GetObject('ddlCarHireDropOffDepotID').disabled = true;
		}
	}

	CarHire.SetDepotDropdown = function (CountryID) {

		int.ff.Call('=iVectorWidgets.CarHire.GetCarHireDepotHTML',
			function (sHTML) {
				CarHire.SetDepotDropdown_Done(sHTML);
			},
			CountryID);

	}

	CarHire.SetDepotDropdown_Done = function (sHTML) {

		int.f.GetObject('ddlCarHirePickUpDepotID').disabled = false;
		int.f.SetHTML('ddlCarHirePickUpDepotID', sHTML);
		int.f.GetObject('ddlCarHireDropOffDepotID').disabled = false;
		int.f.SetHTML('ddlCarHireDropOffDepotID', sHTML);

	}

	CarHire.ValidateBasket = function (oCompleteFunction) {
		
		int.ff.Call('=iVectorWidgets.CarHire.ValidateBasket',
			function (iCarHireCount) {
				CarHire.ValidateBasket_Done(iCarHireCount, oCompleteFunction)
			});

	}

	CarHire.ValidateBasket_Done = function (iCarHireCount, oCompleteFunction) {

		if (iCarHireCount == 0) {
			web.InfoBox.Show('Please select a car', 'warning');
			return false;
		} else {
			oCompleteFunction();
			return true;
		}

	}

    CarHire.Filter = function() {
        
        var oFilter = {
            Transmission: int.dd.GetValue('ddlTransmission'),
            AirConditioning: int.dd.GetValue('ddlAirConditioning')
        }

        int.ff.Call('=iVectorWidgets.CarHire.Filter',
			function (sJSON) {
			    CarHire.FilterComplete(sJSON);
			},
			JSON.stringify(oFilter));

    }

    CarHire.FilterComplete = function (sJSON) {

        var oReturn = JSON.parse(sJSON);

        if (oReturn.Success) {
            int.f.SetHTML('divCarHireResults', oReturn.HTML, true);
            $('#divCarHireResults').show();
        }
        else {
            if (int.f.GetValue('hidFilterWarning') != '') {
                int.f.Show('divCarHireResults');
                int.f.Show('divCarHireSearch');
                int.f.SetHTML('divCarHireResults', '<p class="noResults">' + int.f.GetValue('hidFilterWarning') + '</p>');
            }
            else {
                int.f.Show('divCarHireResults');
                int.f.Show('divCarHireSearch');
                int.f.SetHTML('divCarHireResults', '<p class="noResults">' + oReturn.Warning + '</p>');
            }
        }
    }


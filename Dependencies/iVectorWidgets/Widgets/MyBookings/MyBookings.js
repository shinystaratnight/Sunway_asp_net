var MyBookings = MyBookings || {};

	var me = MyBookings;
	MyBookings.BookingReference = '';

	MyBookings.ShowCancellationPopup = function (iBookingReference) {

		me.BookingReference = iBookingReference;

		int.ff.Call('=iVectorWidgets.MyBookings.PreCancel', function (sJSON) { MyBookings.RequestPreCancelComplete(sJSON) }, me.BookingReference);
//		int.e.ModalPopup.Show('divCancellation');
		web.ModalPopup.Show('divCancellation', true, 'modalpopup container');
	}

	MyBookings.RequestPreCancelComplete = function (sJSON) {

		var oPreCancelReturn = eval('(' + sJSON + ')');

		if (oPreCancelReturn['OK'] == true) {

			int.f.SetHTML('spnCancellationCost', oPreCancelReturn['CancellationCost']);
			int.f.SetValue('hidCancellationToken', oPreCancelReturn['CancellationToken']);
//			int.e.ModalPopup.Show('divCancellation');
			web.ModalPopup.Show('divCancellation', true, 'modalpopup container');


		} else {

			web.InfoBox.Show('Sorry, there was a problem calculating the cancellation charges.', 'warning');

		};

	}

	MyBookings.RequestCancellationValidate = function () {

		var nCancellationCharge = int.f.GetHTML('spnCancellationCost');
		var sCancellationToken = int.f.GetValue('hidCancellationToken');

		int.f.SetClassIf('lblConfirmCancel', 'error', !int.cb.Checked('chkConfirmCancel'));

		if (int.cb.Checked('chkConfirmCancel')) {
			int.f.Show('divCancellationWaitMessage');
			int.f.Hide('divCancellationForm');
			int.ff.Call('=iVectorWidgets.MyBookings.CancelBooking', function (sJSON) { MyBookings.CancelBookingComplete(sJSON) }, nCancellationCharge, sCancellationToken, me.BookingReference);
		}

	};

	MyBookings.CancelBookingComplete = function (sJSON) {

		//hide action links, total price and wait message
		//		f.Hide('divActionLinks_' + sBookingID);
		int.f.Hide('divCancellationWaitMessage');

		var oCancelReturn = eval('(' + sJSON + ')');

		if (oCancelReturn['OK'] == true) {

			int.f.Show('divCancellationConfirmation');
			int.f.Hide('divTotalPrice_' + me.BookingReference);
			var oNotification = document.createElement('div');
			oNotification.className = 'cancelledBooking';
			oNotification.innerHTML = 'This booking has been cancelled';
			int.f.GetObject('divBooking_' + me.BookingReference).appendChild(oNotification);

		} else {
			int.f.Show('divCancellationFailed');
		};

	};

	MyBookings.ShowAmendmentPopup = function (iBookingReference) {

	    int.f.Show('divAmendmentForm');
	    int.f.Hide('divRequestSent');

		var dMinDate = new Date();

		var dReturnMinDate = new Date();
		int.d.AddDays(dReturnMinDate, 1);

		var bUseShortDates = int.f.GetValue('hidUseShortDates');

		int.f.SetValue('txtAmendmentDepartureDate', MyBookings.FormatDate(dMinDate));
		web.DatePicker.Setup('#txtAmendmentDepartureDate', dMinDate, dMinDate, 1, 1, 0, null, 'checkin', false, '', '', '', '', '', bUseShortDates);

		int.f.SetValue('txtAmendmentReturnDate', MyBookings.FormatDate(dReturnMinDate));
		web.DatePicker.Setup('#txtAmendmentReturnDate', dReturnMinDate, dReturnMinDate, 1, 1, 0, null, 'checkout', false, '', '', '', '', '', bUseShortDates);

		me.BookingReference = iBookingReference;

		//		int.e.ModalPopup.Show('divAmendment');
		web.ModalPopup.Show('divAmendment', true, 'modalpopup container booking-amendment');
	}

	MyBookings.FormatDate = function (dRawDate) {

		var dMonth = dRawDate.getMonth() + 1;

		if (dMonth < 10) {
			dMonth = '0' + dMonth;
		};

		return dRawDate.getDate() + '/' + dMonth + '/' + dRawDate.getFullYear();

	};

	/* Request Amendment */
	MyBookings.RequestAmendmentValidate = function () {

		//check date and duration
		var sDepartureDate = int.f.GetValue('txtAmendmentDepartureDate');
		var sReturnDate = int.f.GetValue('txtAmendmentReturnDate');

		var dDepartureDate = int.d.New(sDepartureDate.split('/')[0], sDepartureDate.split('/')[1], sDepartureDate.split('/')[2]);
		var dReturnDate = int.d.New(sReturnDate.split('/')[0], sReturnDate.split('/')[1], sReturnDate.split('/')[2]);

		var iNights = int.d.DateDiff(dDepartureDate, dReturnDate);

		int.f.SetClassIf('txtAmendmentReturnDate', 'error', iNights < 1);

		int.f.SetClassIf('txtAmendmentDepartureDate', 'error', !int.d.IsDate(dDepartureDate) || int.f.GetValue('txtAmendmentDepartureDate') == '');
		int.f.SetClassIf('txtAmendmentReturnDate', 'error', !int.d.IsDate(dReturnDate) || int.f.GetValue('txtAmendmentReturnDate') == '' || iNights < 1);

		//max total rate
		int.f.SetClassIf('ddlTotalRate', 'error', int.dd.GetText('ddlTotalRate') == '');

		//authorise payment
		int.f.SetClassIf('lblAuthorise', 'error', int.f.GetObject('chkAuthorise').checked == false);

		//if no errors send request otherwise show warning
		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divAmendment');

		if (aErrorControls.length == 0) {
			int.f.Hide('divAmendmentWarning');
			int.f.Hide('divAmendmentForm');
			int.f.Show('divAmendRequestWaitMessage');

			int.ff.Call('=iVectorWidgets.MyBookings.RequestAmendment', function (sResult) { MyBookings.RequestAmendmentComplete(sResult) }, me.BookingReference,
			dDepartureDate, dReturnDate, int.f.GetValue('txtDestination'), int.f.GetValue('txtHotelName'), int.dd.GetText('ddlTotalRate'),
			int.f.GetValue('txtAdditionalInformation'));

		} else {
			int.f.Show('divAmendmentWarning');
		}

	}

	MyBookings.RequestAmendmentComplete = function (sResult) {

		if (sResult == 'Success') {
			int.f.Hide('divAmendRequestWaitMessage');
			int.f.Hide('divAmendmentForm');
			int.f.Show('divRequestSent');
			int.f.SetHTML('h3divAmendment', 'Booking Amendment Request Sent');
		}

	}

	MyBookings.ViewDocumentation = function (sBookingReference) {

		web.ModalPopup.Show('divViewDocuments');

		int.ff.Call('=iVectorWidgets.MyBookings.ShowDocumentation',
            function (sJSON) { MyBookings.ViewDocumentationDone(sJSON) },
            sBookingReference
        );
	};

	MyBookings.ViewDocumentationDone = function (sJSON) {

		web.ModalPopup.Hide();

		// should be an array of keys
		var oReturn = JSON.parse(sJSON);

		if (oReturn['OK'] == true && oReturn.Keys.length > 0) {
			for (var i = 0; i < oReturn.Keys.length; i++) {
				window.open('/services/view-documentation/' + oReturn.Keys[i],
					'documentation' + i, 'toolbar=no,location=no,menubar=no,copyhistory=no,status=no,directories=no');
			};
		}
		else {
			web.InfoBox.Show('Sorry, there was a problem generating your documentation.', 'warning');
		};

	};

	MyBookings.SendDocumentation = function (sBookingReference) {

//		int.e.ModalPopup.Show('divEmailDocuments');
		web.ModalPopup.Show('divEmailDocuments');


		int.ff.Call('=iVectorWidgets.MyBookings.SendDocumentation',
            function (sJSON) { MyBookings.SendDocumentationDone(sJSON) },
            sBookingReference
        );
	};

	MyBookings.SendDocumentationDone = function (sJSON) {

		var oReturn = eval('(' + sJSON + ')');

		if (oReturn['OK'] == true) {

			int.f.Hide('divEmailWaitMessage');
			int.f.Show('divEmailSuccess');
			setTimeout('web.ModalPopup.Hide()', 2000);
		}
		else {
		    web.InfoBox.Show('Sorry, there was a problem sending your documentation.', 'warning');
		    web.ModalPopup.Hide();
		};

	};

	MyBookings.EditSeatReservations = function (iFlightBookingID, iTabIndex) {

		int.f.Show('divSeatSelection_' + iFlightBookingID);

		if (iTabIndex == 1) {
			me.ChangeSeatSelectionTab(iFlightBookingID);
		}

		if (int.f.GetObject('divSeatsConfirmed_' + iFlightBookingID)) {
			int.f.Hide('divSeatsConfirmed_' + iFlightBookingID);
		}

	}

	MyBookings.ChangeSeatSelectionTab = function (iParentID) {
		var aTabs = int.f.GetElementsByClassName('li', 'tab', 'divSeatSelection_' + iParentID);
		for (var i = 0; i < aTabs.length; i++) int.f.ToggleClass(aTabs[i], 'selected');
		var aTabContents = int.f.GetElementsByClassName('div', 'tabContent', 'divSeatSelection_' + iParentID);
		for (var i = 0; i < aTabContents.length; i++) int.f.Toggle(aTabContents[i]);
	}

	MyBookings.SelectSeat = function (iFlightBookingID, iFlightBookingPassengerID, sDirection, nOriginalPrice) {

		web.InfoBox.Close();

		var oDropdown = int.f.GetObject('ddlSeats_' + iFlightBookingID + '_' + iFlightBookingPassengerID + '_' + sDirection)
		var aOption = int.dd.GetValue(oDropdown).split('_');

		var aSeatDropdowns = int.f.GetObjectsByIDPrefix('ddlSeats_', 'select', 'divSeatSelection_' + iFlightBookingID);
		var bAlreadySelected = false;
		for (var i = 0; i < aSeatDropdowns.length; i++) {
			if (aSeatDropdowns[i] != oDropdown && int.dd.GetValue(aSeatDropdowns[i]) == int.dd.GetValue(oDropdown)) {
				bAlreadySelected = true;
			}
		}

		if (!bAlreadySelected || aOption[0] == 0) {

			// set supplement
			int.f.SetHTML('spnSeatPrice_' + iFlightBookingID + '_' + iFlightBookingPassengerID + '_' + sDirection, aOption[1]);

		}
		else {

			// reset dropdown selection and display warning
			web.InfoBox.Show('Seat ' + int.dd.GetText(oDropdown) + ' has already been selected for another passenger, please reselect a different option.', 'warning');
			oDropdown.selectedIndex = 0;
			// reset supplement
			int.f.SetHTML('spnSeatPrice_' + iFlightBookingID + '_' + iFlightBookingPassengerID + '_' + sDirection, 0);

		}

		var nPrice = 0;
		for (var j = 0; j < aSeatDropdowns.length; j++) {
			nPrice = nPrice + int.n.SafeNumeric(int.dd.GetValue(aSeatDropdowns[j]).split('_')[1]);
		}

		var nAmountDue = nPrice - nOriginalPrice;
		if (nAmountDue < 0) {
			nAmountDue = 0;
		}

		int.f.SetHTML('spnTotalSeatPrice_' + iFlightBookingID, nPrice);
		int.f.SetHTML('spnAmountDue_' + iFlightBookingID, nAmountDue);

	}

	MyBookings.ConfirmSeatSelection = function (sBookingReference, iFlightBookingID, nOriginalPrice) {

		var oExtras = {
			BasketFlightExtras: []
		};

		var aSeatDropdowns = int.f.GetObjectsByIDPrefix('ddlSeats_', 'select', 'divSeatSelection_' + iFlightBookingID);

		for (var i = 0; i < aSeatDropdowns.length; i++) {

			var aDropdownID = aSeatDropdowns[i].id.split('_');
			var aOption = int.dd.GetValue(aSeatDropdowns[i]).split('_');

			if (aOption[0] != 0) {

				var oBasketFlightExtra = {
					ExtraBookingToken: aOption[0],
					QuantitySelected: 1,
					ExtraType: 'Seat',
					GuestID: aDropdownID[2],
					Price: aOption[1]
				};

				oExtras.BasketFlightExtras.push(oBasketFlightExtra);
			}

		}

		int.ff.Call("=iVectorWidgets.MyBookings.AddSeatsToBasket", MyBookings.AddSeatsToBasketComplete, sBookingReference, iFlightBookingID, nOriginalPrice, JSON.stringify(oExtras));

	}

	MyBookings.AddSeatsToBasketComplete = function (sCompleteFunction) {

		web.InfoBox.Close();

		try {
			var oCompleteFunction = eval('(' + sCompleteFunction + ')');
			if (oCompleteFunction) oCompleteFunction();
		}
		catch (exception) {
			web.InfoBox.Show('Sorry, there was a problem reserving your seats. Please try again or choose an alternative option.', 'warning');
		}
	}

	MyBookings.ReserveSeatsDone = function (sWarning) {

		if (sWarning == '') {
			int.ff.Call('=iVectorWidgets.MyBookings.UpdateMyBookingsHTML', function (sHTML) { int.f.SetHTML('divMyBookings', sHTML); }, 'true');
		}
		else {
			web.InfoBox.Show(sWarning, 'warning');
		}
	}

	MyBookings.LogOut = function () {

		var sLogoutRedirectURL = '/booking-login';

		if (int.f.GetValue('hidLogoutRedirectURL') != '') {
			sLogoutRedirectURL = int.f.GetValue('hidLogoutRedirectURL');
		}

		int.ff.Call('=iVectorWidgets.MyBookings.LogOut', function () { web.Window.Redirect(sLogoutRedirectURL); });

	}
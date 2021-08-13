var CompleteBooking = CompleteBooking || {};

CompleteBooking.CurrentValidateScript = 0;
CompleteBooking.ValidationScripts = new Array;
CompleteBooking.CompleteFunction = null;
CompleteBooking.InfoBoxOffset = 20;
CompleteBooking.InProgress = false;

//Variables for timeout message
var bookingTimeout;
var bTimeoutRunning = false;
var bTimeoutMessageShown = false;

//#region Validate
CompleteBooking.ValidateScripts = function (sScripts, oCompleteFunction) {
	if (sScripts != '') {
		CompleteBooking.CurrentValidateScript = 0;
		CompleteBooking.ValidationScripts = sScripts.split('#');
		CompleteBooking.CompleteFunction = oCompleteFunction;
		CompleteBooking.ValidateNext();
	}
	else {
		CompleteBooking.CompleteFunction = oCompleteFunction;
		CompleteBooking.Validate();
	}
};

CompleteBooking.ValidateNext = function () {
	CompleteBooking.CurrentValidateScript += 1;
	if (CompleteBooking.CurrentValidateScript <= CompleteBooking.ValidationScripts.length) {
		var sFunction = CompleteBooking.ValidationScripts[CompleteBooking.CurrentValidateScript - 1];
		setTimeout(sFunction + '(CompleteBooking.ValidateNext)', 0);
	}
	else {
		CompleteBooking.Validate();
	}
};

CompleteBooking.Validate = function () {
	var iErrors = int.f.GetElementsByClassName('', 'error', 'divAll').length
	if (iErrors == 0) {
		if (CompleteBooking.CompleteFunction != undefined) CompleteBooking.CompleteFunction();
	}
	else {
		var sInsertWarningAfter = int.f.GetValue('hidInsertWarningAfter');
		if (sInsertWarningAfter !== '') {
			web.InfoBox.Show(int.f.GetValue('hidWarningMessage'), 'warning', null, sInsertWarningAfter, null, CompleteBooking.InfoBoxOffset);
		} else {
			web.InfoBox.Show(int.f.GetValue('hidWarningMessage'), 'warning', null, null, null, CompleteBooking.InfoBoxOffset);
		}
		window.scrollTo(0, 0);
	}
};
//#endregion

//#region Copy Search Basket To Main Basket
CompleteBooking.SearchToMainBasket = function () {
	WaitMessage.Show('PreBook');
	int.ff.Call('=iVectorWidgets.CompleteBooking.SearchToMainBasket', CompleteBooking.SearchToMainBasketComplete);
};

CompleteBooking.SearchToMainBasketComplete = function (sReturn) {
	if (sReturn == 'Fail') {
		WaitMessage.Hide();
		web.Window.Redirect('/booking-summary?warn=prebookfailed');
	}
	else {
		//if redirecting to payment page perform pre book
		if (int.f.GetValue('hidRedirectURL').indexOf('/payment') != -1) {
			CompleteBooking.PreBookNoCopy();
		}
		else {
			web.Window.Redirect(int.f.GetValue('hidRedirectURL'));
		}
	}
};
//#endregion

//#region Add component
CompleteBooking.AddComponentSearchShow = function () {
	//add search basket to main basket so we don't lose what's already been searched
	int.ff.Call('=iVectorWidgets.CompleteBooking.SearchToMainBasket', CompleteBooking.AddComponentSearchShowComplete);
};

CompleteBooking.AddComponentSearchShowComplete = function () {
	//once we've finished, then we can show the search tool popup
	SearchTool.Expand();
};
//#endregion

//#region Prebook
CompleteBooking.PreBook = function () {
	WaitMessage.Show('PreBook');
	int.ff.Call('=iVectorWidgets.CompleteBooking.PreBook', CompleteBooking.PreBookComplete);
};

CompleteBooking.PreBookNoCopy = function () {
	WaitMessage.Show('PreBook');
	int.ff.Call('=iVectorWidgets.CompleteBooking.PreBookNoCopy', CompleteBooking.PreBookComplete);
};

CompleteBooking.PreBookComplete = function (sJSON) {
	var oReturn = JSON.parse(sJSON);
	if (oReturn.OK == false) {
		var sFailURL = int.f.GetValue('hidFailURL');
		WaitMessage.Hide();
		//on failure, replace the original page so customer can't go back to it
		web.Window.Replace(sFailURL !== '' ? sFailURL + oReturn.RedirectString : '/booking-summary?warn=prebookfailed');
	}
	else {
		var redirectURL = int.f.GetValue('hidRedirectURL');
		web.Window.Redirect(oReturn.RedirectString !== null ? redirectURL + oReturn.RedirectString : redirectURL);
	}
};
//#endregion

//#region Book
CompleteBooking.Book = function () {
    if (CompleteBooking.InProgress === false) {
        CompleteBooking.InProgress = true;
	WaitMessage.Show('Book');
	int.ff.Call('=iVectorWidgets.CompleteBooking.Book', CompleteBooking.BookComplete);

	//Show the timeout message if the booking times out
	if (int.f.GetValue('hidShowTimeoutMessage') == 'true') {
		bookingTimeout = setTimeout(function () { CompleteBooking.ShowTimeoutMessage('Book') }, 120000);
		bTimeoutRunning = true;
	}
    };
};

CompleteBooking.BookComplete = function (sJSON) {
    CompleteBooking.InProgress = false;

	//If we're running a booking timeout, clear it.
	if (bTimeoutRunning) {
		clearTimeout(bookingTimeout);
	}

	var oReturn = JSON.parse(sJSON);

	//If we've shown the timeout message and the booking subsequently goes through, don't redirect to the confirmation page
	if (!bTimeoutMessageShown) {
		if (oReturn.SecureEnrollment == true) {
			web.Window.Redirect('/paymentauthorisation');
		}
		else {
			if (oReturn.OK == false) {
				var sFailURL = int.f.GetValue('hidFailURL');
				WaitMessage.Hide();
				//on failure, replace the original page so customer can't go back to it
				web.Window.Replace(sFailURL != '' ? sFailURL + oReturn.RedirectString : '/payment?warn=bookfailed');
			}
			else {
				var redirectURL = int.f.GetValue('hidRedirectURL');
				web.Window.Redirect(oReturn.RedirectString !== null ? redirectURL + oReturn.RedirectString : redirectURL);
			}
		}
	}
};
//#endregion

//#region UpdatePrice
CompleteBooking.UpdatePrice = function (oJSON) {
	var symbol = int.f.GetValue('hidCompleteBooking_CurrencySymbol');
	var position = int.f.GetValue('hidCompleteBooking_CurrencyPosition');

	if (int.f.GetObject('strTotalPrice')) {
		int.f.SetHTML(strTotalPrice, int.n.FormatMoney(oJSON.TotalPriceUnformated, symbol, position));
		int.f.SetHTML(strDepositPrice, int.n.FormatMoney(oJSON.DepositAmountUnformated, symbol, position));

		if (int.f.GetObject('strSurcharge')) {
			if (oJSON.Surcharge && oJSON.Surcharge != 0) {
				var dSurchargeAmount = oJSON.TotalPriceSurchargeAmount;
				var	dDepositSurchargeAmount = oJSON.DepositPriceSurchargeAmount;
				
				int.f.SetHTML('strSurcharge', 'Credit Card Fee Applied (' + int.n.FormatNumber(oJSON.Surcharge, 2) + '%) : ' + int.n.FormatMoney(dSurchargeAmount, symbol, position));
				if (int.f.GetObject('strDepositSurcharge')) {
					int.f.SetHTML('strDepositSurcharge', 'Credit Card Fee Applied To Deposit (' + int.n.FormatNumber(oJSON.Surcharge, 2) + '%) : ' + int.n.FormatMoney(dDepositSurchargeAmount, symbol, position));
				}
			}
			else {
				int.f.SetHTML('strSurcharge', '');
				if (int.f.GetObject('strDepositSurcharge')) {
					int.f.SetHTML('strDepositSurcharge', '');
				}
			}
		}
	/* only update one or the other */
	} else if (int.f.GetObject('spnBasketTotal')) {
		int.f.SetHTML('spnBasketTotal', int.n.FormatMoney(oJSON.TotalPriceUnformated, int.f.GetValue('hidCompleteBooking_CurrencySymbol'), int.f.GetValue('hidCompleteBooking_CurrencyPosition')));
        //			int.f.SetHTML('spnBasketTotal', int.n.FormatMoney(oJSON.DepositAmountUnformated, int.f.GetValue('hidCompleteBooking_CurrencySymbol'), int.f.GetValue('hidCompleteBooking_CurrencyPosition')));
	}
	if (oJSON.FlightSupplierFee > 0) {
		int.f.SetHTML('spnFlightFee', 'Flight Fees Applied : ' + int.n.FormatMoney(oJSON.FlightSupplierFee, symbol, position));
	}
};
//#endregion

//#region ShowTimeoutMessage
CompleteBooking.ShowTimeoutMessage = function (Type) {
	//Show waitmessage timeout message
	WaitMessage.ShowTimeout(Type);
		
	//Timeout message has been shown
	bTimeoutMessageShown = true;
}
//#endregion
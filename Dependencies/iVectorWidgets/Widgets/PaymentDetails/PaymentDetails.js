var PaymentDetails = PaymentDetails || {}

	var me = this;

	//hack - call this to in turn update the price on the complete booking widget
	web.PubSub.subscribe('PromoCode.ChangeSuccess', function () {
		PaymentDetails.CreditCardSelect();
	});

	PaymentDetails.CompleteFunction = null;
	PaymentDetails.PerformValidation = true;

	PaymentDetails.SetPaymentType = function () {

		var bPayByCard = int.cb.Checked('radPayByCard');
		var bUseBillingAddress = int.f.GetValue('hidUseBillingAddress');

		int.f.ShowIf('divPaymentDetails', bPayByCard == false);
		int.f.ShowIf('divPaymentDetails', bPayByCard == true);
		int.f.ShowIf('divBillingAddress', bPayByCard == true && bUseBillingAddress == 'true');

	};

	PaymentDetails.Validate = function (oCompleteFunction) {

		PaymentDetails.PerformValidation = int.f.GetValue('hidPerformValidation');

		//set completefunction
		PaymentDetails.CompleteFunction = oCompleteFunction;

		if (int.f.GetObject('divPaymentDetails') != undefined && PaymentDetails.PerformValidation == 'true') {

			//clear error classes
			for (var aErrors = int.f.GetElementsByClassName('*', 'error', int.f.GetObject('divPaymentDetails')), i = 0; i < aErrors.length; i++) {
				int.f.RemoveClass(aErrors[i], 'error');
			}

			//get form values
			var sCardNumber = int.f.GetValue('txtCCCardNumber');
			var sSecurityCode = int.f.GetIntValue('txtPayment_CSV');
			var sExpiryMonth = int.dd.GetValue('ddlCCExpireMonth');
			var sExpiryYear = int.dd.GetValue('ddlCCExpireYear');

			//set error class if any form elements aren't set/are set incorrectly
			int.f.SetClassIf('ddlCCCardTypeID', 'error', int.dd.GetIntValue('ddlCCCardTypeID') == 0);
			int.f.SetClassIf('txtCCCardNumber', 'error', !int.v.IsCardNumber(sCardNumber) || sCardNumber == '' || int.n.SafeInt(sCardNumber) == 0);
			int.f.SetClassIf('txtCCCardHoldersName', 'error', int.f.GetValue('txtCCCardHoldersName') == '');
			int.f.SetClassIf('ddlCCExpireMonth', 'error', sExpiryMonth == '' || !int.v.IsNumeric(sExpiryMonth));
			int.f.SetClassIf('ddlCCExpireYear', 'error', sExpiryYear == '' || !int.v.IsNumeric(sExpiryYear));

			PaymentDetails.ValidateCSV();

			//billing address
			var bPayByCard = int.cb.Checked('radPayByCard');
			var bUseBillingAddress = int.f.GetValue('hidUseBillingAddress');
			if (bPayByCard && bUseBillingAddress == 'true') {
				int.f.SetClassIf('ddlBillingAddress_Title', 'error', int.dd.GetIndex('ddlBillingAddress_Title') == 0);
				int.f.SetClassIf('txtBillingAddress_FirstName', 'error', int.f.GetValue('txtBillingAddress_FirstName') == '');
				int.f.SetClassIf('txtBillingAddress_LastName', 'error', int.f.GetValue('txtBillingAddress_LastName') == '');
				int.f.SetClassIf('txtBillingAddress_Address', 'error', int.f.GetValue('txtBillingAddress_Address') == '');
				int.f.SetClassIf('txtBillingAddress_City', 'error', int.f.GetValue('txtBillingAddress_City') == '');
				int.f.SetClassIf('txtBillingAddress_Postcode', 'error', int.f.GetValue('txtBillingAddress_Postcode') == '');
				int.f.SetClassIf('ddlBillingAddress_BookingCountry', 'error', int.dd.GetIndex('ddlBillingAddress_BookingCountry') == 0);
			};

			//if valid add details to basket
			var iErrors = int.f.GetElementsByClassName('', 'error', 'divPaymentDetails').length;
			if (bPayByCard && bUseBillingAddress == 'true') {
				iErrors = iErrors + int.f.GetElementsByClassName('', 'error', 'divBillingAddress').length;
			};

			iErrors == 0 ? PaymentDetails.AddDetailsToBasket() : PaymentDetails.AddDetailsToBasketComplete();
		}
		else {
			int.ff.Call('=iVectorWidgets.PaymentDetails.ExcludePaymentDetails', PaymentDetails.AddDetailsToBasketComplete);
		}
	}

	PaymentDetails.ValidateCSV = function () {

		//Get the datattribute to see if we should validate the CSV, lots of customers have override controls and js, so if this is not set(and is null), also consider this true.
		var bCSVRequired = document.getElementById("txtCCSecurityCode").getAttribute("validation-required");
		var bCSVRequired = bCSVRequired == "true" || bCSVRequired == null;
		int.f.SetClassIf('txtCCSecurityCode', 'error', (int.f.GetValue('txtCCSecurityCode') == '' && bCSVRequired));

	}

	// this is currently ibiza rocks specific
	PaymentDetails.MMBValidate = function () {

		if (int.f.GetObject('divPaymentDetails') != undefined) {

			//clear error classes
			for (var aErrors = int.f.GetElementsByClassName('*', 'error', int.f.GetObject('divPaymentDetails')), i = 0; i < aErrors.length; i++) {
				int.f.RemoveClass(aErrors[i], 'error');
			}

			//get form values
			var sCardNumber = int.f.GetValue('txtCCCardNumber');
			var sSecurityCode = int.f.GetIntValue('txtPayment_CSV');
			var sExpiryMonth = int.dd.GetValue('ddlCCExpireMonth');
			var sExpiryYear = int.dd.GetValue('ddlCCExpireYear');
			var sPaymentAmount = int.f.GetValue('txtAmount');
			var sOutstandingAmount = int.f.GetValue('txtAmountDueNow');

			//set error class if any form elements aren't set/are set incorrectly
			int.f.SetClassIf('ddlCCCardTypeID', 'error', int.dd.GetValue('ddlCCCardTypeID') == 0);
			int.f.SetClassIf('txtCCCardNumber', 'error', !int.v.IsCardNumber(sCardNumber) || sCardNumber == '');
			int.f.SetClassIf('txtCCCardHoldersName', 'error', int.f.GetValue('txtCCCardHoldersName') == '');
			int.f.SetClassIf('ddlCCExpireMonth', 'error', sExpiryMonth == '' || !int.v.IsNumeric(sExpiryMonth));
			int.f.SetClassIf('ddlCCExpireYear', 'error', sExpiryYear == '' || !int.v.IsNumeric(sExpiryYear));
			int.f.SetClassIf('txtCCSecurityCode', 'error', (int.f.GetValue('txtCCSecurityCode') == '' && int.f.Visible('txtCCSecurityCode')))
			int.f.SetClassIf('txtAmount', 'error', int.n.SafeNumeric(sPaymentAmount) == 0);

			// payment less than 50 euros? 
			var bInvalidPaymentAmount = false
			if (int.n.SafeNumeric(sPaymentAmount) > 0 && int.n.SafeNumeric(sPaymentAmount) < 50 && int.n.SafeNumeric(sOutstandingAmount) >= 50) bInvalidPaymentAmount = true;

			//outstanding < 50 and payment < outstanding
			var bInvalidSmallPaymentAmount = false
			if (int.n.SafeNumeric(sOutstandingAmount) < 50 && int.n.SafeNumeric(sPaymentAmount) < int.n.SafeNumeric(sOutstandingAmount)) bInvalidSmallPaymentAmount = true;

			// success?
			var iErrors = int.f.GetElementsByClassName('', 'error', 'divPaymentDetails').length;
			if (iErrors > 0) {
				web.InfoBox.Show('Please ensure that all required fields are set. Incorrect fields have been highlighted.', 'warning', null, 'divCountDown');
			}
			else if (bInvalidPaymentAmount == true) {
				web.InfoBox.Show('A minimum payment of 50 euros is required.', 'warning', null, 'divCountDown');
			}
			else if (bInvalidSmallPaymentAmount) {
				web.InfoBox.Show('A minimum payment of 50 euros or your deposit/balance (whichever is lower) is required.', 'warning', null, 'divCountDown');
			}
			else {
				PaymentDetails.MakePayment()
			}

		}
	}

	PaymentDetails.AddDetailsToBasket = function () {
		//get key value pairs
		var sKeyValuePairs = int.f.GetContainerQueryString('divPaymentDetails');

		var bPayByCard = int.cb.Checked('radPayByCard');
		var bUseBillingAddress = int.f.GetValue('hidUseBillingAddress');
		if (bPayByCard && bUseBillingAddress == 'true') {
			sKeyValuePairs = sKeyValuePairs + '&' + int.f.GetContainerQueryString('divBillingAddress');
			sKeyValuePairs = sKeyValuePairs + '&ddlBillingAddress_BookingCountryID=' + int.dd.GetIndex('ddlBillingAddress_BookingCountry');
		};

		//add to basket
		int.ff.Call('=iVectorWidgets.PaymentDetails.AddDetailsToBasket', PaymentDetails.AddDetailsToBasketComplete, sKeyValuePairs);
	}

	PaymentDetails.AddDetailsToBasketComplete = function () {
		if (PaymentDetails.CompleteFunction != null && PaymentDetails.CompleteFunction != undefined) PaymentDetails.CompleteFunction();
	}


	PaymentDetails.MakePayment = function () {
		//get key value pairs
		var sKeyValuePairs = int.f.GetContainerQueryString('divPaymentDetails');

		if (int.f.GetObject('txtAmount')) {
			int.f.AddClass('txtAmount', 'paying');
		}
		//go off and make the payment
		int.ff.Call('=iVectorWidgets.PaymentDetails.MakePayment', PaymentDetails.MakePaymentCompleteComplete, sKeyValuePairs);
	}

	PaymentDetails.MakePaymentCompleteComplete = function (sSuccess) {

		if (int.f.GetObject('txtAmount')) {
			int.f.RemoveClass('txtAmount', 'paying');
		}

		if (sSuccess == 'True') {
			web.Window.Redirect('/view-booking');
		}
		else {
			web.InfoBox.Show('Sorry, there was an error with your payment. Please contact us.', 'warning', null, 'divCountDown');
		}
	}

	//Credit Card Select
	PaymentDetails.CreditCardSelect = function () {
		var iCreditCardTypeID = int.dd.GetValue('ddlCCCardTypeID');
		int.ff.Call('=iVectorWidgets.PaymentDetails.CreditCardSelect', PaymentDetails.CreditCardSelect_Callback, iCreditCardTypeID);
	};

	PaymentDetails.CreditCardSelect_Callback = function (sJSON) {
		//get the values out of the JSON
		var oReturn = JSON.parse(sJSON);

		//set the value of the hidden inputs used in the add to basket.
		int.f.SetValue(hidSurcharge, oReturn['Surcharge']);
		int.f.SetValue(hidTotalAmount, oReturn['TotalPriceUnformated']);

		//Only show the CSV fields if the card type requires a CSV
		int.f.ShowIf('txtCCSecurityCode', oReturn['RequiresCSV']);
		int.f.ShowIf('dtSecruityCode', oReturn['RequiresCSV']);
		int.f.ShowIf('aPayment_CSVTooltip', oReturn['RequiresCSV']);

		if (!oReturn['RequiresCSV']) {
			int.f.SetValue('txtCCSecurityCode', '');
		}

		//pass the values to complete Booking to update the display
		var bDisablePriceUpdate = int.f.GetValue('hidDisablePriceUpdate');
		if (bDisablePriceUpdate !== 'True') {
			CompleteBooking.UpdatePrice(oReturn);
		}
	};




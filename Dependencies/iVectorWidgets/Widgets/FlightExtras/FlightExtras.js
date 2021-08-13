var FlightExtras = FlightExtras || {}

FlightExtras.Update = function (oObject) {
    FlightExtras.CentralUpdate(oObject, false);
}

FlightExtras.UpdateReturn = function (oObject) {
    FlightExtras.CentralUpdate(oObject, true);
}

FlightExtras.CentralUpdate = function (oObject, bReturn) {

	var sReturnString = '';

	if (bReturn) {
	    sReturnString = 'Return';
	}

	var iTotalPax = int.f.GetValue('hidFlightExtraPax');
	var bExtrasValid = true;
	var iBaggageQuantity = 0;

	//Get all the extra tokens
	var aFlightExtraTypes = int.f.GetObjectsByIDPrefix('hid' + sReturnString + 'FlightExtraToken_', 'input', 'divFlightExtras');

	//An object to hold the extra options.
	var oFlightExtras = {
		ExtraOptions: []
	}

	//Loop through each object we just returned and build up the object with the tokens and the quantity.
	for (var i = 0; i < aFlightExtraTypes.length; i++) {
		var iIndex = aFlightExtraTypes[i].id.split('_');
		var iQuantity = parseInt(int.f.GetValue('ddl' + sReturnString + 'FlightExtraQuantity_' + iIndex[1]), 10);

		if (iTotalPax >= iQuantity) {

			//build up the object that will associate tokens with quanity
			var oOption = {
				Token: aFlightExtraTypes[i].value,
				Quantity: iQuantity
			}

			oFlightExtras.ExtraOptions.push(oOption);

			var bDisplayDecimals = false
			if (int.f.GetObject('hidPriceFormat')) {
				if (int.f.GetValue('hidPriceFormat') == '###,##0.00') {
					bDisplayDecimals = true
				}
			};

			//update what we display to the user
			var nFlightExtraCost = int.f.GetValue('hid' + sReturnString + 'FlightExtraCost_' + iIndex[1]);
			if (bDisplayDecimals) {
			    int.f.SetHTML('sp' + sReturnString + 'FlightExtraTotal_' + iIndex[1], int.n.FormatNumber(nFlightExtraCost * iQuantity, 2));
			}
			else {
			    int.f.SetHTML('sp' + sReturnString + 'FlightExtraTotal_' + iIndex[1], Math.ceil(nFlightExtraCost * iQuantity));
			}			
			int.f.SetValue('hid' + sReturnString + 'FlightExtraQuant_' + iIndex[1], iQuantity);

		} else {

			bExtrasValid = false;
			int.f.SetValue('ddl' + sReturnString + 'FlightExtraQuantity_' + iIndex[1], int.f.GetValue('ddl' + sReturnString + 'FlightExtraQuantity_' + iIndex[1]));

		};

		//if the extratype is baggage then check how many we have
		if (int.f.GetValue('hid' + sReturnString + 'FlightExtraType_' + iIndex[1]) == 'Baggage') {
			iBaggageQuantity += iQuantity;
		}

	};

	var aExtraQuantityDropdowns = int.f.GetObjectsByIDPrefix('ddl' + sReturnString + 'FlightExtraQuantity_', 'select', 'divFlightExtras');

	for (var i = 0; i < aExtraQuantityDropdowns.length; i++) {
	    int.f.SetClassIf(aExtraQuantityDropdowns[i], 'error', (iBaggageQuantity > iTotalPax) || !bExtrasValid);
	}

	//we need to check that the total number of bags selected doesn't exceed the total pax
	//this is different to the logic above, which is for each individual bag type
	if (iBaggageQuantity > iTotalPax) {
	    web.InfoBox.Show(int.f.GetValue('hidFlightExtra_InvalidBaggage'), 'warning');
	    if (bReturn) {
	        FlightExtras.ReturnExtrasValid = false;
	    } else {
	        FlightExtras.OutboundExtrasValid = false;
	    }
	} 
	else {
		if (bExtrasValid) {
		    if (bReturn) {
		        FlightExtras.ReturnExtrasValid = true;
		    } else {
		        FlightExtras.OutboundExtrasValid = true;
		    }
		    if (FlightExtras.ReturnExtrasValid && FlightExtras.OutboundExtrasValid) {
		        web.InfoBox.Close();
		    }

			//convert to JSON string
			var sJSON = JSON.stringify(oFlightExtras);
			int.ff.Call('=iVectorWidgets.FlightExtras.UpdateExtras', function () { Basket.UpdateBasketHTML(); }, sJSON, bReturn);

		} else {
		    if (bReturn) {
		        FlightExtras.ReturnExtrasValid = false;
		    } else {
		        FlightExtras.OutboundExtrasValid = false;
		    }
			web.InfoBox.Show(int.f.GetValue('hidFlightExtra_Invalid'), 'warning');

		};
	}

}

FlightExtras.OutboundExtrasValid = true;
FlightExtras.ReturnExtrasValid = true;

FlightExtras.Validate = function (oCompleteFunction) {

	//Only validate if we have a warning to validate
	if (int.f.GetObject('divFlightExtras')) {

		var iTotalPax = int.f.GetValue("hidFlightExtraPax");
		var bExtrasValid = true;
		var iBaggageQuantity = 0;
		var iReturnBaggageQuantity = 0;
		//Get all the extra tokens
		var aFlightExtraTypes = int.f.GetObjectsByIDPrefix('hidFlightExtraToken_', 'input', 'divFlightExtras');

		for (var i = 0; i <= aFlightExtraTypes.length - 1; i++) {
			var iIndex = aFlightExtraTypes[i].id.split("_");
			var iQuantity = parseInt(int.f.GetValue("ddlFlightExtraQuantity_" + iIndex[1]), 10);			

			//if the extratype is baggage then check how many we have
			if (int.f.GetValue('hidFlightExtraType_' + iIndex[1]) == 'Baggage') {
				iBaggageQuantity += iQuantity;
			}
		};
        
		var aReturnFlightExtraTypes = int.f.GetObjectsByIDPrefix('hidReturnFlightExtraToken_', 'input', 'divFlightExtras');

		for (var i = 0; i <= aReturnFlightExtraTypes.length - 1; i++) {
		    var iIndex = aReturnFlightExtraTypes[i].id.split("_");
		    var iQuantity = parseInt(int.f.GetValue("ddlReturnFlightExtraQuantity_" + iIndex[1]), 10);

		    //if the extratype is baggage then check how many we have
		    if (int.f.GetValue('hidReturnFlightExtraType_' + iIndex[1]) == 'Baggage') {
		        iReturnBaggageQuantity += iQuantity;
		    }
		};

		//we need to check that the total number of bags selected doesn't exceed the total pax
		if (iBaggageQuantity > iTotalPax || iReturnBaggageQuantity > iTotalPax) {
			web.InfoBox.Show(int.f.GetValue('hidFlightExtra_InvalidBaggage'), 'warning');
			window.scrollTo(0, 0);
			return false
		}
		else {
			if (oCompleteFunction != null && oCompleteFunction != undefined) {
				oCompleteFunction();
			}
			return true
		}
	} 
	else {
		//run complete function if exists
		if (oCompleteFunction != null && oCompleteFunction != undefined) {
			oCompleteFunction();
		}
		return true
	}
}
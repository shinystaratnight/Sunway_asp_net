var OffsitePaymentRedirect = OffsitePaymentRedirect || {}


OffsitePaymentRedirect.GetOffsitePaymentURL = function () {

	//get payment amount and booking reference if we're on MMB
	var nPaymentAmount;
	if (int.f.GetObject('hidOutstandingAmount')) {
		nPaymentAmount = int.f.GetValue('hidOutstandingAmount');
	}
	else {
		nPaymentAmount = 0;
	}

	//get the booking reference if we're on MMB
	var sBookingReference;
	var sExternalReference;
	if (int.f.GetObject('hidBookingReference')) {
	    sBookingReference = int.f.GetValue('hidBookingReference');
	    sExternalReference = int.f.GetValue('hidExternalReference');
	}
	else {
	    sBookingReference = '';
	    sExternalReference = ''; 
	}

	int.ff.Call('Widgets.OffsitePaymentRedirect.GetOffsitePaymentURL', OffsitePaymentRedirect.GetOffsitePaymentURLComplete, nPaymentAmount, sBookingReference, sExternalReference);

}

OffsitePaymentRedirect.GetOffsitePaymentURLComplete = function (sJSON) {

	//update html if successful
	var oReturn = JSON.parse(sJSON);

	if (oReturn.Success) {
		$('html').html(oReturn.HTML);
	}
	else if (oReturn.Warnings.length > 0) {
	    window.location = window.location + '?warn=' + oReturn.Warnings[0];
	}
	else {        
		window.location = window.location + '?warn=bookfailed'
	}

}
var MyBookingsLogin = MyBookingsLogin || {};

//1. Setup
MyBookingsLogin.ExtraSetup = function () {

    if ((int.f.getParameterByName('paymentfailed') === 'true') || (int.f.getParameterByName('authfailed') === 'true')) {
	    web.InfoBox.Show('There has been an error processing your payment', 'warning');
	}
};

$(document).ready(function () {
    MyBookingsLogin.ExtraSetup();
});
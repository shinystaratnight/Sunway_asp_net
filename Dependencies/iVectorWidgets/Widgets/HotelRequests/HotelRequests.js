var HotelRequests = HotelRequests || {};


HotelRequests.CompleteFunction = null;

HotelRequests.Validate = function (oCompleteFunction) {

    HotelRequests.CompleteFunction = oCompleteFunction;

	if (int.f.GetObject('divHotelRequests')) {

		if (int.f.GetObject('ddlArrivalTime')) {
			int.f.SetClassIf('ddlArrivalTime', 'error', int.f.GetValue('ddlArrivalTime') == '');
		}

		//if valid add details to basket
		var iErrors = int.f.GetElementsByClassName('', 'error', 'divHotelRequests').length
		if (iErrors == 0) {
			HotelRequests.AddRequestToBasket(oCompleteFunction);
		}
		else {
			HotelRequests.AddRequestToBasketComplete();
		}
	}
	else {
	    HotelRequests.CompleteFunction();
	}

}

HotelRequests.AddRequestToBasket = function (oCompleteFunction) {
	//set completefunction
	this.CompleteFunction = oCompleteFunction;

	//if we have a request save to basket
	var sArrivalTime = ''
	if (int.f.GetObject('ddlArrivalTime')) {
		sArrivalTime = int.f.GetValue('ddlArrivalTime');
	}

	var sRequest = int.f.GetValue('txtHotelRequests_Requests');
	if (sRequest != '' || sArrivalTime != '') {
		int.ff.Call('=iVectorWidgets.HotelRequests.AddRequestToBasket', function () { HotelRequests.AddRequestToBasketComplete(); }, sRequest, sArrivalTime);
	}
	else {
		HotelRequests.AddRequestToBasketComplete();
	}

}

HotelRequests.AddRequestToBasketComplete = function () {
    if (HotelRequests.CompleteFunction != null && HotelRequests.CompleteFunction != undefined) {
        HotelRequests.CompleteFunction();
	}
}
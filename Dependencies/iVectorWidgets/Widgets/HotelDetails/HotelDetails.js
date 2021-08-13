var HotelDetails = HotelDetails || {}

HotelDetails.RequestRoomKey = '';
HotelDetails.RequestRoomPopup = null;

HotelDetails.Setup = function () {

}


HotelDetails.SelectRoom = function (RoomOptionIndex) {

	//flight
	var sFlightOptionHashToken = int.f.GetValue('hidFlightOptionHashToken');

	//transfer
	var sTransferOptionHashToken = int.f.GetValue('hidTransferOptionHashToken');

	//car hire
	var sCarHireOptionHashToken = int.f.GetValue('hidCarHireOptionHashToken');

	//build up JSON object
	var oAddToBasket = {
		FlightOptionHashToken: sFlightOptionHashToken,
		TransferOptionHashToken: sTransferOptionHashToken,
		CarHireOptionHashToken: sCarHireOptionHashToken,
		PropertyRoomIndexes: new Array()
	}
	oAddToBasket.PropertyRoomIndexes.push(RoomOptionIndex);

	var sJSON = JSON.stringify(oAddToBasket);

	//add room (and flight if set)
	int.ff.Call('=iVectorWidgets.HotelDetails.AddRoomOptionToBasket', function (sReturn) { HotelDetails.SelectRoomComplete(sReturn); }, sJSON);

}


HotelDetails.SelectMultiRoom = function () {

    //flight
    var sFlightOptionHashToken = int.f.GetValue('hidFlightOptionHashToken');

    //transfer
    var sTransferOptionHashToken = int.f.GetValue('hidTransferOptionHashToken');

    //car hire
    var sCarHireOptionHashToken = int.f.GetValue('hidCarHireOptionHashToken');

    var oAddToBasket = {
    	FlightOptionHashToken: sFlightOptionHashToken,
    	TransferOptionHashToken: sTransferOptionHashToken,
    	CarHireOptionHashToken: sCarHireOptionHashToken,
        PropertyRoomIndexes: new Array()
    }

    var iRoomCount = int.f.GetValue('hidPropertyResults_RoomCount');

    for (var i = 1; i <= iRoomCount; i++) {

    	var sRoomOptionIndex = int.rb.GetValue('rad_roomoption_' + i);
    	sRoomOptionIndex = sRoomOptionIndex.split('_')[0];

        if (sRoomOptionIndex == null) {
            web.InfoBox.Show(int.f.GetValue('hidWarning_MultiRoomSelect'), 'warning');
            return;
        }
        else {
            oAddToBasket.PropertyRoomIndexes.push(sRoomOptionIndex);
        }
    }

    //convert to JSON string
    var sJSON = JSON.stringify(oAddToBasket);


    //add room (and flight if set)
    int.ff.Call('=iVectorWidgets.HotelDetails.AddRoomOptionToBasket', function (sReturn) { HotelDetails.SelectRoomComplete(sReturn); }, sJSON);

}


HotelDetails.SelectRoomComplete = function (sReturn) {
    if (sReturn.indexOf('Error') == 0) {
        web.InfoBox.Show('Sorry, this room is no longer available.');
    } else {
        web.Window.Redirect(sReturn);
    }
}


HotelDetails.ImageHover = function (element) {

    var sNewSRC = element.src;
    int.f.GetObject('imgMainImage').setAttribute('src', sNewSRC);

    var aImages = int.f.GetObject('divOtherImages').getElementsByTagName('img');

    for (var i = 0; i < aImages.length; i++) {
        int.f.SetClassIf(aImages[i], 'selected', aImages[i] == element);
    };

}


HotelDetails.ShowTab = function (Tab) {

	//hide any open tooltips from other tabs
    web.Tooltip.Hide();

	//remove selected class from all tabs
    var aTabs = int.f.GetObjectsByIDPrefix('liPropertyDetail', 'li', 'divHotelDetails');
	for (i = 0; i < aTabs.length; i++) {
		int.f.RemoveClass(aTabs[i], 'selected');
	}

	//add selected class to new tab
	if (int.f.GetObject('liPropertyDetail' + Tab)) {
		int.f.AddClass('liPropertyDetail' + Tab, 'selected');
	}

    //get content and update
    int.ff.Call('=iVectorWidgets.HotelDetails.TabbedContent',
		function (sHTML) {
			int.f.SetHTML('divHotelDetailsTabbedContent', sHTML, true);
		}, Tab
	);
}


HotelDetails.SelectRadio = function (oContainer) {

	var aRadioButtons = int.f.GetElementsByClassName('input', 'roomOption', oContainer);
	int.cb.SetValue(aRadioButtons[0], true)

	var aRoomOptionDivs = int.f.GetElementsByClassName('div', 'roomOption', oContainer.parentElement);
	for (var i = 0; i < aRoomOptionDivs.length; i++) {
		int.f.RemoveClass(aRoomOptionDivs[i], 'select');
	}

	int.f.AddClass(oContainer, 'select');
};

HotelDetails.SelectRoomOption = function () {

	var iRoomCount = int.f.GetValue('hidPropertyResults_RoomCount');
	var bOnRequest = false;

	for (var i = 1; i <= iRoomCount; i++) {
		var sPropertyRoomValue = int.rb.GetValue('rad_roomoption_' + i);
		var sPropertyRoomOnRequest = 'false';
		if (sPropertyRoomValue != null) {
			sPropertyRoomOnRequest = sPropertyRoomValue.split('_')[1];
		};

		if (sPropertyRoomOnRequest == 'true') {
			bOnRequest = true;
		};

	};

	if (bOnRequest) {
		int.f.Hide('aHotelDetailsBtn');
		int.f.Show('aHotelDetailsRequestRoomBtn');
	} else {
		int.f.Hide('aHotelDetailsRequestRoomBtn');
		int.f.Show('aHotelDetailsBtn');
	};

};

HotelDetails.SelectRequestRoomPopup = function () {

	var iRoomCount = int.f.GetValue('hidPropertyResults_RoomCount');
	var sIndex = '';

	for (var i = 1; i <= iRoomCount; i++) {
		var sPropertyRoomValue = int.rb.GetValue('rad_roomoption_' + i);
		var sPropertyRoomIndex = sPropertyRoomValue.split('_')[0];

		if (sPropertyRoomIndex == null) {
			web.InfoBox.Show(int.f.GetValue('hidWarning_MultiRoomSelect'), 'warning');
			return;
		}
		else {
			sIndex = sIndex + sPropertyRoomIndex + '|';
		};

	};

	HotelDetails.RequestRoomKey = sIndex;

	if (HotelDetails.RequestRoomPopup == null) {
		int.ff.Call('=iVectorWidgets.HotelDetails.GetRequestRoomHTML', HotelDetails.GetRequestRoomHTMLComplete);
	} else {
		HotelDetails.ShowRequestRoomPopup();
	};

};

HotelDetails.GetRequestRoomHTMLComplete = function (sReturn) {

	if (int.s.StartsWith(sReturn, 'Error') || sReturn == '') {
		web.InfoBox.Show('Sorry, we are unable to process requests at this time.');
	}
	else {
		HotelDetails.RequestRoomPopup = int.f.GetObject('divRequestRoomPopup');
		int.f.SetHTML(HotelDetails.RequestRoomPopup, sReturn);
		HotelDetails.ShowRequestRoomPopup();
	};

};

HotelDetails.ShowRequestRoomPopup = function () {
	int.f.Show(HotelDetails.RequestRoomPopup);
};

HotelDetails.CloseRequestRoomPopup = function () {
	int.f.Hide('pRoomRequestSuccess');
	int.f.Hide('pRoomRequestFailed');
	int.f.Hide('pRoomRequestInvalid');
	int.f.Show('btnRoomRequest_Submit');
	int.f.Hide(HotelDetails.RequestRoomPopup);
};

HotelDetails.RequestRoom = function () {

	int.f.Hide('pRoomRequestSuccess');
	int.f.Hide('pRoomRequestFailed');
	int.f.Hide('pRoomRequestInvalid');

	var sPassengerName = int.f.GetValue('txtRequestRoom_Name');
	var sPassengerEmailAddress = int.f.GetValue('txtRequestRoom_Email');
	var sPassengerTelephoneNumber = int.f.GetValue('txtRequestRoom_Telephone');

	if (sPassengerName == '' || sPassengerEmailAddress == '' || sPassengerTelephoneNumber == '') {
		int.f.Show('pRoomRequestInvalid');
	} else {

		var oRequestRoom = {
			RoomIndex: HotelDetails.RequestRoomKey,
			PassengerName: sPassengerName,
			PassengerEmailAddress: sPassengerEmailAddress,
			PassengerTelephoneNumber: sPassengerTelephoneNumber
		};

		var sJSON = JSON.stringify(oRequestRoom);

		int.ff.Call('=iVectorWidgets.HotelDetails.RequestRoom', HotelDetails.RequestRoomComplete, sJSON);

	};

};

HotelDetails.RequestRoomComplete = function (sReturn) {

	if (int.s.StartsWith(sReturn, 'Error')) {
		int.f.Show('pRoomRequestFailed');
	}
	else {
		int.f.Show('pRoomRequestSuccess');
		int.f.Hide('btnRoomRequest_Submit');
	};

};


HotelDetails.ChangeFlight = function(iPropertyReferenceID) {
   int.ff.Call('=iVectorWidgets.HotelDetails.ChangeFlight',
    function(sHTML) {
        int.f.SetHTML('divSelectedFlight', sHTML);
        int.f.AddClass('divSelectedFlight', 'change-flight');
    }, iPropertyReferenceID);
}

HotelDetails.KeepFlight = function(iPropertyReferenceID) {
   int.ff.Call('=iVectorWidgets.HotelDetails.KeepFlight',
    function(sHTML) {
        int.f.SetHTML('divSelectedFlight', sHTML);
        int.f.RemoveClass('divSelectedFlight', 'change-flight');
    }, iPropertyReferenceID);
}

HotelDetails.UpdateFlight = function(iPropertyReferenceID, sFlightBookingToken) {
   int.ff.Call('=iVectorWidgets.HotelDetails.UpdateSelectedFlight',
    function(sHTML) {
        int.f.SetHTML('divSelectedFlight', sHTML);
        int.f.RemoveClass('divSelectedFlight', 'change-flight');
    }, iPropertyReferenceID, sFlightBookingToken);
}

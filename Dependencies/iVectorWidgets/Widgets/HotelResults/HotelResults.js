var HotelResults = HotelResults || {};

web.PubSub.subscribe('HotelResults.UpdateResults', function () {
    HotelResults.UpdateResults();
});

HotelResults.TabContent = [];
HotelResults.RequestRoomKey = '';
HotelResults.RequestRoomPopup = null;
HotelResults.MapView = false;

HotelResults.Redirect = function (ID) {

	int.ff.Call('=iVectorWidgets.HotelResults.GetPropertyURL', function (URL) {

		web.Window.Redirect(URL);

	}, ID);

}

HotelResults.Hide = function () {
	int.f.Hide('divHotelResults');
}

HotelResults.Show = function () {
	int.f.Show('divHotelResults');
}

HotelResults.SetCurrentView = function (sView) {
	HotelResults.Hide();
	int.ff.Call('=iVectorWidgets.HotelResults.SetCurrentView', function () { }, sView);
}

HotelResults.HideMap = function () {
	int.f.Hide('divHotelResultsMapHolder');
	web.Tooltip.Hide();
	HotelResults.MapView = false;
}

HotelResults.ShowMap = function () {
	int.f.Show('divHotelResultsMapHolder');
	HotelResults.MapView = true;
}

HotelResults.SetupMapEvents = function () {

	web.PubSub.publish('HotelResults_Setup');

	var oOnClick = function (oMarker) {
        var oMapPoint = MapWidget.GetMapPoint(oMarker.MarkerID.split('_')[1]);
		HotelResults.ShowDetailsPopup(oMapPoint.ID);
	}

	var oOnMouseOver = function (oMarker) {
		web.Tooltip.Hide();

		var iPropertyID = oMarker.MarkerID.split('_')[1];
		var oSetPopupContent = function (sHTML) {
            web.Tooltip.Show('', sHTML, 'top', MapWidget.GoogleMap.GetMarkerScreenPosition(oMarker), MapWidget.iPopupX + 35 , MapWidget.iPopupY - 15, 'mapHover');
		};

		int.ff.Call('=iVectorWidgets.HotelResults.MapPopup', oSetPopupContent, iPropertyID);
	};

	var oOnMouseOut = function () {
		web.Tooltip.Hide();
	};

	//Check if map js and DefaultMarkerEvents has been loaded is set before calling the setup
	if (MapWidget.DefaultMarkerEvents) {
		MapWidget.DefaultMarkerEvents.Setup(oOnClick, oOnMouseOver, oOnMouseOut);
	};		

}
	
HotelResults.ShowTab = function (PropertyReferenceID, Tab) {

	web.Tooltip.Hide(); //hide any open tooltips from other tabs

	//set selected tab
	int.f.SetClassIf('liRates_' + PropertyReferenceID, 'selected', Tab == 'Rates');
	int.f.SetClassIf('liHotelDetails_' + PropertyReferenceID, 'selected', Tab == 'HotelDetails');
	int.f.SetClassIf('liHotelImages_' + PropertyReferenceID, 'selected', Tab == 'HotelImages');

	if (int.f.GetObject('liHotelMap_' + PropertyReferenceID)) {
		int.f.SetClassIf('liHotelMap_' + PropertyReferenceID, 'selected', Tab == 'HotelMap');
	};

	if (int.f.GetObject('liFlights_' + PropertyReferenceID)) {
		int.f.SetClassIf('liFlights_' + PropertyReferenceID, 'selected', Tab == 'Flights');
	};

	if (int.f.GetObject('liTAReviews_' + PropertyReferenceID)) {
		int.f.SetClassIf('liTAReviews_' + PropertyReferenceID, 'selected', Tab == 'TAReviews');
	};

	if (int.f.GetObject('liHotelOverview_' + PropertyReferenceID)) {
		int.f.SetClassIf('liHotelOverview_' + PropertyReferenceID, 'selected', Tab == 'Overview');
	};

	//get content and update
	if (HotelResults.TabContent[PropertyReferenceID + '_' + Tab] !== undefined) {
		int.f.SetHTML('divTabbedContent_' + PropertyReferenceID, HotelResults.TabContent[PropertyReferenceID + '_' + Tab], true);
	}
	else {
		int.ff.Call(
			'=iVectorWidgets.HotelResults.TabbedContent',
			function (sHTML) {
				int.f.SetHTML('divTabbedContent_' + PropertyReferenceID, sHTML, true);
				if (Tab != 'Rates') HotelResults.TabContent[PropertyReferenceID + '_' + Tab] = sHTML;
			},
			PropertyReferenceID, Tab
		);
	}

}


//#region Update Results
HotelResults.UpdateResults = function () {

	//if not map view show loading message
	if (!HotelResults.MapView) {
		HotelResults.ShowLoadingMessage();
	}

	//update results
	int.ff.Call('=iVectorWidgets.HotelResults.UpdateResults', HotelResults.UpdateResultsComplete);

}


HotelResults.UpdateResultsComplete = function (sHTML) {
	int.f.SetHTML('divHotelResults', sHTML);
	int.f.Hide('divHotelResultsLoading');

	//if not map view show hotel results
	if (!HotelResults.MapView) {
		int.f.Show('divHotelResults');
	}
	
	if (int.f.GetValue('hidHotelFilter_ScrollAfterFilter') == 'True') int.e.ScrollIntoView('divTopHeader');

    web.PubSub.publish('SearchResults_Updated');
}

HotelResults.ShowLoadingMessage = function () {
	int.f.Show('divHotelResultsLoading');
	int.f.Hide('divHotelResults');
}
//#endregion


HotelResults.ShowDetailsPopup = function (iPropertyReferenceID) {
	HotelPopup.ShowPopup(iPropertyReferenceID, true);
}


//#region Select Room
HotelResults.SelectRoom = function (PropertyReferenceID, Index, Callback) {

	if (Warning.Validate()) {

		//check if we need to add the flight to the basket
		var bAddSelectedFlight = int.f.GetValue('hidAddSelectedFlight');
		if (!bAddSelectedFlight) {
			bAddSelectedFlight = false;
		}

		//build up JSON object
		var oAddToBasket = {
			AddSelectedFlightToBasket: bAddSelectedFlight,
			PropertyRoomIndexes: new Array()
		};
		oAddToBasket.PropertyRoomIndexes.push(Index);

		var sJSON = JSON.stringify(oAddToBasket);

		if (Callback == undefined || typeof Callback != 'function') {
			Callback = HotelResults.SelectRoomComplete
		}

		web.PubSub.publish('HotelResults_RoomSelected');

		//add room (and flight if set)
		int.ff.Call('=iVectorWidgets.HotelResults.AddRoomOptionToBasket', Callback, sJSON);

	}
}


HotelResults.SelectMultiRoom = function (PropertyReferenceID, Callback) {

	var oAddToBasket = {
		PropertyRoomIndexes: new Array()
	}

    //check if we need to add the flight to the basket
	var bAddSelectedFlight = int.f.GetValue('hidAddSelectedFlight');
	if (!bAddSelectedFlight) {
	    bAddSelectedFlight = false;
	}

	var iRoomCount = int.f.GetValue('hidPropertyResults_RoomCount');

	for (var i = 1; i <= iRoomCount; i++) {

		var sPropertyRoomIndex = int.rb.GetValue('rad_roomoption_' + PropertyReferenceID + '_' + i);

		if (sPropertyRoomIndex == null) {
			web.InfoBox.Show(int.f.GetValue('hidWarning_MultiRoomSelect'), 'warning');
			return;
		}
		else {
			oAddToBasket.PropertyRoomIndexes.push(sPropertyRoomIndex);
		}
	}

	oAddToBasket.AddSelectedFlightToBasket = bAddSelectedFlight;
	var sJSON = JSON.stringify(oAddToBasket);

	if (Callback == undefined || typeof Callback != 'function') {
		Callback = HotelResults.SelectRoomComplete
    }

	//add room (and flight if set)
	int.ff.Call('=iVectorWidgets.HotelResults.AddRoomOptionToBasket', Callback, sJSON);

}


HotelResults.SelectRoomComplete = function (sReturn) {

	if (int.s.StartsWith(sReturn, 'Error')) {
		var sWaring = sReturn.split('|')[1];
        web.InfoBox.Show(sWaring);
	}
	else {
        web.Window.Redirect(sReturn);
    }

}

HotelResults.SelectRequestRoomPopup = function (Index) {
	HotelResults.RequestRoomKey = Index;
	if (HotelResults.RequestRoomPopup == null) {
		int.ff.Call('=iVectorWidgets.HotelResults.GetRequestRoomHTML', HotelResults.GetRequestRoomHTMLComplete);
	} else {
		HotelResults.ShowRequestRoomPopup();
	};
};

HotelResults.GetRequestRoomHTMLComplete = function (sReturn) {

	if (int.s.StartsWith(sReturn, 'Error') || sReturn == '') {
		web.InfoBox.Show('Sorry, we are unable to process requests at this time.');
	}
	else {
		HotelResults.RequestRoomPopup = int.f.GetObject('divRequestRoomPopup');
		int.f.SetHTML(HotelResults.RequestRoomPopup, sReturn);
		HotelResults.ShowRequestRoomPopup();
	};

};

HotelResults.ShowRequestRoomPopup = function () {
	int.f.Show(HotelResults.RequestRoomPopup);
};

HotelResults.CloseRequestRoomPopup = function () {
	int.f.Hide('pRoomRequestSuccess');
	int.f.Hide('pRoomRequestFailed');
	int.f.Hide('pRoomRequestInvalid');
	int.f.Show('btnRoomRequest_Submit');
	int.f.Hide(HotelResults.RequestRoomPopup);
};

HotelResults.RequestRoom = function () {

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
			RoomIndex: HotelResults.RequestRoomKey,
			PassengerName: sPassengerName,
			PassengerEmailAddress: sPassengerEmailAddress,
			PassengerTelephoneNumber: sPassengerTelephoneNumber
		};

		var sJSON = JSON.stringify(oRequestRoom);

		int.ff.Call('=iVectorWidgets.HotelResults.RequestRoom', HotelResults.RequestRoomComplete, sJSON);

	};

};

HotelResults.RequestRoomComplete = function (sReturn) {

	if (int.s.StartsWith(sReturn, 'Error')) {
		int.f.Show('pRoomRequestFailed');
	}
	else {
		int.f.Show('pRoomRequestSuccess');
		int.f.Hide('btnRoomRequest_Submit');
	};

};

HotelResults.ShowExtraRooms = function (iPropertyReferenceID, iRoom, sIdentifier) {

	sIdentifier = ((sIdentifier == undefined || sIdentifier == null) ? '' : sIdentifier + '_');

	var aExtraRooms = int.f.GetElementsByClassName('tr', 'extra', 'tblRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
	for (i = 0; i < aExtraRooms.length; i++) {
		int.f.Show(aExtraRooms[i]);
	}

	int.f.Show('aHideExtraRooms_'  + sIdentifier + iPropertyReferenceID + '_' + iRoom);
	int.f.Hide('aShowExtraRooms_'  + sIdentifier + iPropertyReferenceID + '_' + iRoom);

}

HotelResults.HideExtraRooms = function (iPropertyReferenceID, iRoom, sIdentifier) {

	sIdentifier = ((sIdentifier == undefined || sIdentifier == null) ? '' : sIdentifier + '_');

	var aExtraRooms = int.f.GetElementsByClassName('tr', 'extra', 'tblRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
	for (i = 0; i < aExtraRooms.length; i++) {
		int.f.Hide(aExtraRooms[i]);
	}

	int.f.Hide('aHideExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
	int.f.Show('aShowExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);

}

//#endregion

//#region Groups
HotelResults.Groups = [];
HotelResults.GetGroups = function (aLink, iGroupID, iPropertyReferenceID, iMealBasisID, sCurrencySymbol) {
	web.Tooltip.Hide();
	web.Tooltip.Show(aLink, 'Please wait..', 'right', undefined, 118, 5);
	
	int.ff.Call('=iVectorWidgets.HotelPopup.GetGroupsPopupContent',
		function (sHTML) {
			web.Tooltip.Hide();
			HotelPopup.ShowGroupPopupDone(sHTML);
		},
		iPropertyReferenceID, iGroupID, iMealBasisID, sCurrencySymbol
	);
}
//#endregion


//#region CancellationCharges
HotelResults.CancellationCharges = [];
HotelResults.GetCancellationCharges = function (aLink, sCancellationChargesPosition, iPropertyReferenceID, Index, sDisplayID, sErrataDisplayID) {

	switch (sCancellationChargesPosition) {
		case 'Popup':
			//hide any tooltips already showing
			web.Tooltip.Hide();
			//show wait message
			web.Tooltip.Show(aLink, 'Please wait...', 'right', undefined, 118, 5);
			break;
		case 'NewRow':
			//todo
			break;
	}


	//create call back function 
	var oGetCancellationChargesDone = function (sReturn) {
		var aReturn = sReturn.split('|');
		switch (sCancellationChargesPosition) {
			case 'Popup':
				//hide wait message tooltip
				web.Tooltip.Hide();
				//show cancellation charges in tooltip
				if (!sDisplayID || sDisplayID === '') {
					web.Tooltip.Show(aLink, aReturn[0], 'right', undefined, 118, 5, 'cancellationCharges');
				} else {
					int.f.SetHTML(int.f.GetObject(sDisplayID), aReturn[0]);
					int.f.SetHTML(int.f.GetObject(sErrataDisplayID), aReturn[1]);
				}
				//add result to collection
				HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index] = sReturn;
				break;
			case 'NewRow':
				//todo
				break;
		}
	};

	HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index] !== undefined ?
		oGetCancellationChargesDone(HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index]) :
		int.ff.Call('=iVectorWidgets.HotelResults.GetCancellationErrata', oGetCancellationChargesDone, sCancellationChargesPosition, iPropertyReferenceID, Index);


}

HotelResults.HideCancellationCharges = function (sCancellationChargesPosition, iPropertyReferenceID, Index) {
	switch (sCancellationChargesPosition) {
		case 'Popup':
			web.Tooltip.Hide();
			break;
		case 'NewRow':
			//todo
			break;
	}
}

HotelResults.CancellationErrata = [];
HotelResults.GetCancellationErrata = function (aLink, sCancellationChargesPosition, iPropertyReferenceID, Index, sDisplayID) {
	var oGetCancellationErrataDone = function (sReturn) {
		switch (sCancellationChargesPosition) {
			case 'Popup':
				//show cancellation charges in tooltip
				int.f.SetHTML(int.f.GetObject(sDisplayID), sReturn);
				//add result to collection
				HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index] = sReturn;
				break;
			case 'NewRow':
				//todo
				break;
		}
	};

	HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index] !== undefined ?
		oGetCancellationChargesDone(HotelResults.CancellationCharges[iPropertyReferenceID + '_' + Index]) :
		int.ff.Call('=iVectorWidgets.HotelResults.GetCancellationErrata', oGetCancellationErrataDone, sCancellationChargesPosition, iPropertyReferenceID, Index);
}

HotelResults.ShowRooms = function (iPropertyReferenceID) {
	var aRooms = int.f.GetElementsByClassName('tr', 'hiddenRooms_' + iPropertyReferenceID, 'divHotelResults');
	for (var i = 0; i < aRooms.length; i++) {
	    int.f.Toggle(aRooms[i]);
	}

	if (int.f.Visible(aRooms[0])) {
	    int.f.SetHTML('spShowRooms_' + iPropertyReferenceID, 'Hide more options');
	    int.f.AddClass('spShowRooms_' + iPropertyReferenceID, 'expanded');
	}
	else {
	    int.f.SetHTML('spShowRooms_' + iPropertyReferenceID, 'Show more options');
	    int.f.RemoveClass('spShowRooms_' + iPropertyReferenceID, 'expanded');
	}
}


HotelResults.ShowMoreOptions = function (iPropertyReferenceID, iRoomNumber) {

    //check if we have any translations
    var sShowMoreText = (int.f.GetObject('hidShowMoreText') ? int.f.GetValue('hidShowMoreText') : 'Show more options');
    var sHideMoreText = (int.f.GetObject('hidHideMoreText') ? int.f.GetValue('hidHideMoreText') : 'Hide more options');

	var aRooms = int.f.GetElementsByClassName('tr', 'hiddenRooms_' + iPropertyReferenceID + '_' + iRoomNumber, 'divHotelResults');
	for (var i = 0; i < aRooms.length; i++) {
		int.f.Toggle(aRooms[i]);
	}

	if (int.f.Visible(aRooms[0])) {
	    int.f.SetHTML('spShowRooms_' + iPropertyReferenceID + '_' + iRoomNumber, sHideMoreText);
		int.f.AddClass('spShowRooms_' + iPropertyReferenceID + '_' + iRoomNumber, 'expanded');
	}
	else {
	    int.f.SetHTML('spShowRooms_' + iPropertyReferenceID + '_' + iRoomNumber, sShowMoreText);
		int.f.RemoveClass('spShowRooms_' + iPropertyReferenceID + '_' + iRoomNumber, 'expanded');
	}
}



HotelResults.ToggleParentClass = function (oParent, sParentClass, oParentContainer, sClass) {

	var aElements = int.f.GetElementsByClassName(oParent.tagName, sParentClass, oParentContainer);

	for (var i = 0; i < aElements.length; i++) {
		int.f.ToggleClassIf(aElements[i], sClass, '', aElements[i] == oParent);
	}

}

	
//#endregion

HotelResults.ChangeFlight = function(iPropertyReferenceID) {
   int.ff.Call('=iVectorWidgets.HotelResults.ChangeFlight',
    function(sHTML) {
        int.f.SetHTML('divSelectedFlight_' + iPropertyReferenceID, sHTML);
        int.f.AddClass('divSelectedFlight_' + iPropertyReferenceID, 'change-flight');
    }, iPropertyReferenceID);
}

HotelResults.KeepFlight = function(iPropertyReferenceID) {
   int.ff.Call('=iVectorWidgets.HotelResults.KeepFlight',
    function(sHTML) {
        int.f.SetHTML('divSelectedFlight_' + iPropertyReferenceID, sHTML);
        int.f.RemoveClass('divSelectedFlight_' + iPropertyReferenceID, 'change-flight');
    }, iPropertyReferenceID);
}

HotelResults.UpdateFlight = function(iPropertyReferenceID, sFlightBookingToken) {
   int.ff.Call('=iVectorWidgets.HotelResults.UpdateSelectedFlight',
    function(sHTML) {
        int.f.SetHTML('divHotelResults', sHTML);
        int.f.RemoveClass('divSelectedFlight_' + iPropertyReferenceID, 'change-flight');
    }, iPropertyReferenceID, sFlightBookingToken);
}
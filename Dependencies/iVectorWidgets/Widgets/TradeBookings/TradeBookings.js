var TradeBookings = TradeBookings || {}

TradeBookings.CurrentPage = 1;

//#region Paging
TradeBookings.PreviousPage = function () {
	TradeBookings.UpdateResults(TradeBookings.CurrentPage - 1);
}

TradeBookings.NextPage = function () {
	TradeBookings.UpdateResults(TradeBookings.CurrentPage + 1);
}

TradeBookings.SelectPage = function (iPage) {
	TradeBookings.UpdateResults(iPage);
}


//#region Sort
TradeBookings.Sort = function (sField, sOrder, aLink) {

	int.ff.Call('=iVectorWidgets.TradeBookings.SortBookings', function (sHTML) { TradeBookings.UpdateResults(sHTML); }, sField, sOrder);

	aSortLinks = int.f.GetElementsByClassName('a', 'sort', 'divTradeBookings');
	for (var i = 0; i < aSortLinks.length; i++) {
		int.f.RemoveClass(aSortLinks[i], 'selected');
	};
	int.f.AddClass(aLink, 'selected');

};


TradeBookings.UpdateResults = function (iPage) {
	TradeBookings.CurrentPage = iPage || 1;
	int.ff.Call('=iVectorWidgets.TradeBookings.UpdateResults', TradeBookings.UpdateResultsComplete, TradeBookings.CurrentPage);
}

TradeBookings.UpdateResultsComplete = function (sReturn) {
		
	try {
		var oReturn = JSON.parse(sReturn);
		int.f.SetHTML('divBookingsTable', oReturn.TradeBookingsHTML);
		int.f.SetHTML('divTradeBookingsFooter', oReturn.PagingHTML + '<div class="clear"></div>');
		int.e.ScrollIntoView('divFindBooking', int.e.GetPosition('divFindBooking').Height * -1, 2)
	}
	catch (sExcept) {
		web.InfoBox.Show('Sorry, there was an unexpected error', 'warning');
	}

}

TradeBookings.ViewDocuments = function (BookingReference, DocumentationName) {
			
	web.ModalPopup.Show('divDocumentationWaitMessage', true);

	// Form function
	int.ff.Call('=iVectorWidgets.TradeBookings.GetDocumentation',
		function (sJSON) {
			TradeBookings.ViewDocumentationDone(sJSON)
		},
		BookingReference, DocumentationName
	);

};

TradeBookings.ViewDocumentationDone = function (sJSON) {

	var oReturn = JSON.parse(sJSON);

	// Hide popup
	int.f.Hide('divDocumentationWaitMessage');
	web.ModalPopup.Hide();

	if (oReturn.ReturnStatus.Success && oReturn.DocumentPaths.length > 0) {
		for (var i = 0; i < oReturn.DocumentPaths.length; i++) {
			window.open(oReturn.DocumentPaths[i], 'documentation' + i, 'toolbar=no,location=no,menubar=no,copyhistory=no,status=no,directories=no');
		};
	}
	else {
		web.InfoBox.Show(int.f.GetValue('hidTradeBookings_ErrorNoDocuments'), 'warning');
	};

};

//#region Send Documents

TradeBookings.GetSendDocumentationPopup = function (sBookingReference) {
	int.ff.Call('=iVectorWidgets.TradeBookings.GetSendDocumentationPopupContent',
        function (sHTML) {
            TradeBookings.GetSendDocumentationPopupDone(sHTML);
        },
        sBookingReference
    );
};

TradeBookings.GetSendDocumentationPopupDone = function (sHTML) {

	web.ModalPopup.Show(sHTML, true, 'modalpopup tradeEmailDocs');

};

TradeBookings.SendDocumentation = function (sBookingReference, sDocumentationName) {

	int.ff.Call('=iVectorWidgets.TradeBookings.SendDocumentation',
            function (sJSON) {
                TradeBookings.SendDocumentationDone(sJSON)
            },
            sBookingReference, sDocumentationName, int.f.GetValue('txtOverrideEmail'));
};

TradeBookings.SendDocumentationDone = function (sJSON) {

	var oReturn = eval('(' + sJSON + ')');

	int.f.Hide('divSendDocumentationForm');

	if (oReturn.ReturnStatus.Success) {
		int.f.Show('pEmailDocumentationRequestThankyou');
	}
	else {
		int.f.Show('pEmailDocumentationRequestError');
	};

};

//#endregion

//#region cancellations

TradeBookings.ComponentCancellation_GetPopup = function (sBookingReference) {
	int.ff.Call('=iVectorWidgets.TradeBookings.GetCancellationPopupDetails',
        function (sJSON) {
            TradeBookings.ComponentCancellation_GetPopupDone(sJSON);
        },
        sBookingReference
    );
};

TradeBookings.ComponentCancellation_GetPopupDone = function (sJSON) {
	//		var oContainer = int.f.GetObject('divModalPopupsContainer')

	//		if (!oContainer) {
	//			var oContainer = document.createElement('div');
	//			oContainer.id = 'divModalPopupsContainer';
	//			document.body.appendChild(oContainer);
	//		};

	//		int.f.SetHTML(oContainer, sJSON);

	//		int.e.ModalPopup.Show('divModalPopupsContainer');

	web.ModalPopup.Show(sJSON, true, 'modalpopup tradeCancelComponent');
};

TradeBookings.ComponentCancellation_SelectComponent = function (chkComponent) {

	var i = 0;
	var aComponents = int.f.GetObjectsByIDPrefix('chkCancelComponent_', 'input');

	//if all ticked - untick individual components
	if (int.cb.Checked(chkComponent) && chkComponent.id.split('_')[1] == 'All') {
		for (i = 0; i < aComponents.length; i++) int.cb.SetValue(aComponents[i], false);
		int.cb.SetValue(chkComponent, true);
	}
	else if (int.cb.Checked(chkComponent)) {
		int.cb.SetValue(int.f.GetObjectsByIDPrefix('chkCancelComponent_All', 'input')[0], false);
	};

	//add up total cancellation amount
	var nTotal = 0.0;
	var bComponentSelected = false;
	for (i = 0; i < aComponents.length; i++) {
		var aComponentID_Split = aComponents[i].id.split('_');
		if (int.cb.Checked(aComponents[i])) {
			nTotal += int.n.SafeNumeric(int.f.GetValue('hidCancelComponent_' + aComponentID_Split[1] + '_' + aComponentID_Split[2] + '_' + aComponentID_Split[3]));
			bComponentSelected = true;
		};
	}

	int.f.SetHTML('spCancelComponent_Total', bComponentSelected ? int.f.GetValue('hidCancelComponent_SellingCurrencySymbol') + nTotal : '');
	int.f.SetValue('hidCancelComponent_Total', bComponentSelected ? nTotal : 0);

}

TradeBookings.ComponentCancellation_CancelComponent = function () {

	int.f.Hide('pCancellationRequestNoComponents');

	var sBookingReference = int.f.GetValue('hidCancelComponent_BookingReference');

	//if all checked cancel booking
	if (int.cb.Checked(int.f.GetObjectsByIDPrefix('chkCancelComponent_All', 'input')[0])) {
		TradeBookings.CancelBooking(sBookingReference, int.f.GetValue('hidCancelComponent_Total'), int.f.GetObjectsByIDPrefix('chkCancelComponent_All', 'input')[0].id.split('_')[2]);
	} else {

		var i = 0;
		var aComponents = int.f.GetObjectsByIDPrefix('chkCancelComponent_', 'input');



		var oCancelComponentsRequest = {
			BookingReference: sBookingReference,
			BookingComponents: new Array()
		};

		var bComponentSelected = false;
		for (i = 0; i < aComponents.length; i++) {
			var aComponentID_Split = aComponents[i].id.split('_');
			if (int.cb.Checked(aComponents[i]) && aComponentID_Split[1] != 'All') {
				bComponentSelected = true;
				var oComponent = {
					ComponentBookingID: aComponentID_Split[3],
					ComponentType: aComponentID_Split[1],
					CancellationCost: int.n.SafeNumeric(int.f.GetValue('hidCancelComponent_' + aComponentID_Split[1] + '_' + aComponentID_Split[2] + '_' + aComponentID_Split[3])),
					CancellationToken: aComponentID_Split[2]
				}
				oCancelComponentsRequest.BookingComponents.push(oComponent);
			}
		}

		if (bComponentSelected) {
			//			WaitMessage.ShowWaitMessage();
			int.ff.Call('=iVectorWidgets.TradeBookings.CancelComponents', TradeBookings.CancelBookingDone, JSON.stringify(oCancelComponentsRequest));

		}
		else {
			int.f.Show('pCancellationRequestNoComponents');
		}

	};

};



TradeBookings.CancelBooking = function (sBookingReference, nCancellationCost, sCancellationToken) {

	//		WaitMessage.ShowWaitMessage();

	int.ff.Call('=iVectorWidgets.TradeBookings.RequestCancellation',
        function (sSuccess) {
            TradeBookings.CancelBookingDone(sSuccess);
        },
        sBookingReference, nCancellationCost, sCancellationToken
    );
};

TradeBookings.CancelBookingDone = function (sSuccess) {

	int.f.Hide('pCancellationRequestAsk');
	int.f.Hide('pCancellationRequestButton');
	int.f.Hide('pCancellationRequestNoComponents');
	int.f.Hide('tblCancelComponent');

	int.f.ShowIf('pCancellationRequestThankyou', sSuccess == 'true'); //weakly typed js!
	int.f.ShowIf('pCancellationRequestError', sSuccess != 'true');

	//		WaitMessage.CloseWaitMessage();

	window.location.reload();

}

//#endregion
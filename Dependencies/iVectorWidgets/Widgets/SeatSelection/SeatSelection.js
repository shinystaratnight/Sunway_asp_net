
var SeatSelection = new function () {

	var me = this;
	this.CompleteFunction = null;

	this.ChangeTab = function () {
		var aTabs = int.f.GetElementsByClassName('li', 'tab', 'divSeatSelection');
		for (var i = 0; i < aTabs.length; i++) int.f.ToggleClass(aTabs[i], 'selected');
		var aTabContents = int.f.GetElementsByClassName('div', 'tabContent', 'divSeatSelection');
		for (var i = 0; i < aTabContents.length; i++) int.f.Toggle(aTabContents[i]);
	};

	this.ToggleVisibility = function (sAction) {

		var aBoxes = int.f.GetElementsByClassName('div', 'tabbedBox', 'divSeatSelection');

		for (var i = 0; i < aBoxes.length; i++) {
			int.f.Toggle(aBoxes[i]);
		};

		int.f.ToggleClass('divSeatSelection', 'hiddencontent');

		int.f.Hide('a' + sAction + 'Selection');

		if (sAction == 'Hide') {
			int.f.Show('aShowSelection');
		}
		else {
			int.f.Show('aHideSelection');
		};

	};

	this.SelectSeat = function (iGuestID, sDirection) {

		web.InfoBox.Close();

		var oDropdown = int.f.GetObject('ddlSeats_' + iGuestID + '_' + sDirection)
		var aOption = int.dd.GetValue(oDropdown).split('_');

		var aSeatDropdowns = int.f.GetObjectsByIDPrefix('ddlSeats_', 'select', 'divSeatSelection');
		var bAlreadySelected = false;
		for (var i = 0; i < aSeatDropdowns.length; i++) {
			if (aSeatDropdowns[i] != oDropdown && int.dd.GetValue(aSeatDropdowns[i]) == int.dd.GetValue(oDropdown)) {
				bAlreadySelected = true;
			}
		}

		if (!bAlreadySelected || aOption[0] == 0) {

			// set supplement
			int.f.SetHTML('spnSeatPrice_' + iGuestID + '_' + sDirection, int.n.SafeNumeric(aOption[1]));

		}
		else {

			// reset dropdown selection and display warning
			web.InfoBox.Show('Seat ' + int.dd.GetText(oDropdown) + ' has already been selected for another guest, please reselect a different option.', 'warning');
			oDropdown.selectedIndex = 0;
			// reset supplement
			int.f.SetHTML('spnSeatPrice_' + iGuestID + '_' + sDirection, 0);

		}

	}

	this.Validate = function (oCompleteFunction) {

		//set completefunction
		this.CompleteFunction = oCompleteFunction;

		SeatSelection.AddSeatsToBasket();
	}

	this.AddSeatsToBasket = function () {

		//get key value pairs
		var sKeyValuePairs = int.f.GetContainerQueryString('divSeatSelections');

		if (sKeyValuePairs != '') {
			//add to basket
			int.ff.Call('=iVectorWidgets.SeatSelection.AddSeatsToBasket', function () { SeatSelection.AddSeatsToBasketComplete(); }, sKeyValuePairs);
		}
		else {
			//skip add to basket (nothing to add)
			SeatSelection.AddSeatsToBasketComplete();
		}

	}

	this.AddSeatsToBasketComplete = function () {
		if (me.CompleteFunction != null && me.CompleteFunction != undefined) {
			me.CompleteFunction();
		}
	}

}
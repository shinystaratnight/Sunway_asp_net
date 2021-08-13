
var HotelUpsell = HotelUpsell || {}


	HotelUpsell.ChangeToFlightAndHotel = function () {
		int.ff.Call('=iVectorWidgets.HotelUpsell.ChangeToFlightAndHotel', HotelUpsell.ChangeToFlightAndHotelComplete);
		WaitMessage.Show('Search');
	}

	HotelUpsell.ChangeToFlightAndHotelComplete = function (sReturn) {

		if (int.s.StartsWith(sReturn, 'Error')) {
			var sWarning = sReturn.split('|')[1];
			web.InfoBox.Show(sWarning, 'warning');
		}
		else {
			web.Window.Redirect('/Flight-Results');
		}

		setTimeout(WaitMessage.Hide(), 5000);


	}

	HotelUpsell.ShowRoomOptions = function (iPropertyRefrenceID) {

		aTableRows = int.f.GetElementsByClassName('tr', 'roomOption', 'divRatesTable_' + iPropertyRefrenceID);


		for (var i = 0; i < aTableRows.length; i++) {
			int.f.Show(aTableRows[i]);
		}

		int.f.Hide('aViewMore_' + iPropertyRefrenceID);
		int.f.Show('aViewLess_' + iPropertyRefrenceID);

	}

	HotelUpsell.HideRoomOptions = function (iPropertyRefrenceID) {

		aTableRows = int.f.GetElementsByClassName('tr', 'roomOption', 'divRatesTable_' + iPropertyRefrenceID);


		for (var i = 0; i < aTableRows.length; i++) {
			if (int.f.HasClass(aTableRows[i], 'first')) int.f.Hide(aTableRows[i]);
		}

		int.f.Show('aViewMore_' + iPropertyRefrenceID);
		int.f.Hide('aViewLess_' + iPropertyRefrenceID);



	}

	HotelUpsell.SelectRoom = function (PropertyReferenceID, Index) {

		if (Warning.Validate()) {

			//flight
			var sFlightOptionHashToken = int.f.GetValue('hidFlightOptionHashToken_' + PropertyReferenceID);

			//build up JSON object
			var oAddToBasket = {
				FlightOptionHashToken: sFlightOptionHashToken,
				PropertyRoomIndexes: new Array()
			};
			oAddToBasket.PropertyRoomIndexes.push(Index);

			var sJSON = JSON.stringify(oAddToBasket);

			//add room (and flight if set)
			int.ff.Call('=iVectorWidgets.HotelUpsell.AddRoomOptionToBasket', HotelUpsell.SelectRoomComplete, sJSON);
		}
	}


	HotelUpsell.SelectRoomComplete = function (sReturn) {

		if (typeof HotelUpsell.OverrideSelectRoomComplete === 'function') {
			HotelUpsell.OverrideSelectRoomComplete(sReturn)
			return;
		}

		if (int.s.StartsWith(sReturn, 'Error')) {
			var sWaring = sReturn.split('|')[1];
			web.InfoBox.Show(sWaring);
		}
		else {
			Basket.UpdateBasketHTML();
			web.InfoBox.Show("Hotel added to basket", 'success');
		}

	}

	//#region Update Results
	HotelUpsell.UpdateResults = function (iHotelstoDisplay) {
		int.ff.Call('=iVectorWidgets.HotelUpsell.UpdateResults', HotelUpsell.UpdateResultsComplete, iHotelstoDisplay);
	}

	HotelUpsell.UpdateResultsComplete = function (sHTML) {
		int.f.SetHTML('divHotelUpsellResultsContent', sHTML, true);
	}



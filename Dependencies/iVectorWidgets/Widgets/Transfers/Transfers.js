var Transfers = Transfers || {}

	//#region Validate
	Transfers.Validate = function (oCompleteFunction) {
		
        //check we've rendered the transfers widget to begin with!
        if(!!int.f.GetObject('divTransfers')) {
        
		    var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divTransfers');

		    for (var i = 0; i < aErrorControls.length; i++) {
			    int.f.RemoveClass(aErrorControls[i], 'error');
		    }

		    //Get the value of the mobile input field
		    sMobileNumber = int.f.GetValue('txtTransfer_GuestPhoneNumber');

		    //check mobile number if field exists
		    if (int.f.GetObject('txtTransfer_GuestPhoneNumber') && !int.cb.Checked('rad_Transfers_0'))
			    int.f.SetClassIf('txtTransfer_GuestPhoneNumber', 'error', sMobileNumber == '');

		    //check we have flight codes
		    if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber') && int.f.GetObject('txtTransfer_AddReturnFlightNumber') && !int.cb.Checked('rad_Transfers_0')) {
			    int.f.SetClassIf('txtTransfer_AddOutboundFlightNumber', 'error', int.f.GetValue('txtTransfer_AddOutboundFlightNumber') == '');
			    int.f.SetClassIf('txtTransfer_AddReturnFlightNumber', 'error', int.f.GetValue('txtTransfer_AddReturnFlightNumber') == '');
		    }
		    else if (int.f.GetObject('txtTransfer_AddOneWayFlightNumber') && !int.cb.Checked('rad_Transfers_0')) {
			    int.f.SetClassIf('txtTransfer_AddOneWayFlightNumber', 'error', int.f.GetValue('txtTransfer_AddOneWayFlightNumber') == '');
		    };

		    //if we have errors show warning message otherwise perform add to basket
		    var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divTransfers');

		    //add room (and flight if set)
		    if (aErrorControls.length == 0 && !int.cb.Checked('rad_Transfers_0') && int.f.GetObject('rad_Transfers_0') != undefined) {
			    int.ff.Call(
                    "=iVectorWidgets.Transfers.UpdateLeadGuest", 
                    function () { 
                        Transfers.ValidateComplete(oCompleteFunction);
                    }, 
                    sMobileNumber
                );
            }
		    else if (aErrorControls.length == 0) {
			    Transfers.ValidateComplete(oCompleteFunction);
            }
		    else {
			    web.InfoBox.Show(int.f.GetValue('hidWarning_Invalid'), 'warning');
            };

        }
        else {
            Transfers.ValidateComplete(oCompleteFunction);
        };

	}

	Transfers.ValidateComplete = function (oCompleteFunction) {
		//run complete function if exists
		if (oCompleteFunction != null && oCompleteFunction != undefined) oCompleteFunction();
	}
	//#endregion


	//#region Add via radio button
	Transfers.AddTransfer = function (HashToken, Position) {

		var sReturnFlightCode = "";
		var sOutboundFlightCode = "";
		var sReturnTime = "";
		var sOutboundTime = "";

		if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber')) {
			sOutboundFlightCode = int.f.GetValue('txtTransfer_AddOutboundFlightNumber');
			sReturnFlightCode = int.f.GetValue('txtTransfer_AddReturnFlightNumber');
			//Brief validation, we need flight codes.
			int.f.SetClassIf('txtTransfer_AddOutboundFlightNumber', 'error', sOutboundFlightCode == '');
			int.f.SetClassIf('txtTransfer_AddReturnFlightNumber', 'error', sReturnFlightCode == '');
		}
		else if (int.f.GetObject('txtTransfer_AddOneWayFlightNumber')) {
			sOutboundFlightCode = int.f.GetValue('txtTransfer_AddOneWayFlightNumber');
			int.f.SetClassIf('txtTransfer_AddOneWayFlightNumber', 'error', sOutboundFlightCode == '');
		}
		//This will be set true of we are using doing a Hotel Only Initial Search and thus need to update the times/codes
		else if (int.f.GetValue('hidValidateFlightDetails') == 'true') {

			sOutboundFlightCode = int.f.GetValue('txtTransfer_OutboundFlightNumber');
			sReturnFlightCode = int.f.GetValue('txtTransfer_ReturnFlightNumber');
			sOutboundTime = int.f.GetValue('txtTransfer_OutboundFlightTime');
			sReturnTime = int.f.GetValue('txtTransfer_ReturnFlightTime');

			//Brief validation, we need flight codes.
			int.f.SetClassIf('txtTransfer_OutboundFlightNumber', 'error', sOutboundFlightCode == '');
			int.f.SetClassIf('txtTransfer_ReturnFlightNumber', 'error', sReturnFlightCode == '');
			int.f.SetClassIf('txtTransfer_OutboundFlightTime', 'error', sOutboundTime == '' || sOutboundTime == 'hh:mm');
			int.f.SetClassIf('txtTransfer_ReturnFlightTime', 'error', sReturnTime == '' || sReturnTime == 'hh:mm');
		}

		//if we have errors show warning message otherwise perform add to basket
		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divTransfers');

		if (aErrorControls.length == 0) {
			//add room (and flight if set)
			int.ff.Call(
				'=iVectorWidgets.Transfers.AddTransferToBasket',
				function (sReturn) {
					Transfers.AddTransferComplete(sReturn, Position);
				},
				HashToken, sOutboundFlightCode, sReturnFlightCode, sReturnTime, sOutboundTime
			);
		}
		else {
			int.cb.SetValue('rad_Transfers_0', true);
			web.InfoBox.Show(int.f.GetValue('hidWarning_Invalid'), 'warning');
		}
	}

	Transfers.AddTransferComplete = function (sReturn, iPosition) {

		try {
			var oReturn = JSON.parse(sReturn);
			if (oReturn.Success && oReturn.ResultsHTML != '') {
				int.f.SetHTML('divTransferOptions', oReturn.ResultsHTML);
			}
			else if (oReturn.Success) {
				//remove class from all options
				var aOptions = int.f.GetElementsByClassName('tr', 'transferOption', 'divTransfers');
				for (i = 0; i < aOptions.length; i++) int.f.RemoveClass(aOptions[i], 'selected');
				//add class to selected option
				int.f.AddClass('tr_Transfers_' + iPosition, 'selected');
			}
			else if (!oReturn.Success) {
				//remove class from all options
				var aOptions = int.f.GetElementsByClassName('tr', 'transferOption', 'divTransfers');
				for (i = 0; i < aOptions.length; i++) int.f.RemoveClass(aOptions[i], 'selected');

				//set class on none
				int.cb.SetValue('rad_Transfers_0', true);
				int.f.SetClassIf('tr_Transfers_0', 'selected', !int.f.HasClass('tr_Transfers_0', 'selected'));
//				int.f.AddClass('tr_Transfers_0', 'selected');
			}
			web.InfoBox.Show(oReturn.Warnings, 'warning');
		}
		catch (ex) {

		}

		//update to basket
		Basket.UpdateBasketHTML();
	}
	//#endregion

	//#region Add via standard button - This is currently only for Ibiza Rocks CM 28/04/2014
	Transfers.AddTransferByButton = function (HashToken, Position) {

		var sReturnFlightCode = "";
		var sOutboundFlightCode = "";

		if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber')) {
			sOutboundFlightCode = int.f.GetValue('txtTransfer_AddOutboundFlightNumber');
			sReturnFlightCode = int.f.GetValue('txtTransfer_AddReturnFlightNumber');
			//Brief validation, we need flight codes.
			int.f.SetClassIf('txtTransfer_AddOutboundFlightNumber', 'error', sOutboundFlightCode == '');
			int.f.SetClassIf('txtTransfer_AddReturnFlightNumber', 'error', sReturnFlightCode == '');
		}

		//if we have errors show warning message otherwise perform add to basket
		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divTransfers');

		if (aErrorControls.length == 0) {
			//add room (and flight if set)
			int.ff.Call(
				'=iVectorWidgets.Transfers.AddTransferToBasket',
				function (sReturn) {
					Transfers.AddTransferCompleteByButton(sReturn, Position);
				},
				HashToken, sReturnFlightCode, sOutboundFlightCode
			);
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidWarning_Invalid'), 'warning');
		}
	}

	Transfers.AddTransferCompleteByButton = function (sReturn, iPosition) {
		if (sReturn == 'Success') {

			//update to basket
			//Basket.UpdateBasketHTML();

			//show/hide buttons and text
			var aAddButtons = int.f.GetObjectsByIDPrefix('btn_TransferAdd_', 'input', 'divTransfers');
			for (i = 0; i < aAddButtons.length; i++) {
				if (aAddButtons[i].id == 'btn_TransferAdd_' + iPosition) {
					int.f.Hide('btn_TransferAdd_' + iPosition);
					int.f.Show('btn_TransferRemove_' + iPosition);
					int.f.Show('spnBooked_' + iPosition);
				} else {
					int.f.Show('btn_TransferAdd_' + (i + 1));
					int.f.Hide('btn_TransferRemove_' + (i + 1));
					int.f.Hide('spnBooked_' + (i + 1));
				}
			}

			//hide warnings
			if (int.f.GetValue('hidShowFlightCodeFields') == 'True') {
				web.InfoBox.Show(int.f.GetValue('hidTransferAddSuccessMessage'), 'success');
			}

		}
	}
	//#endregion


	//#region Remove
	Transfers.RemoveTransfer = function () {
		int.ff.Call('=iVectorWidgets.Transfers.RemoveTransfer', Transfers.RemoveTransferComplete);
	}

	Transfers.RemoveTransferComplete = function (sJSON) {

		var oReturn = eval('(' + sJSON + ')');

		if (oReturn.OK == 'True') {

			//update basket
			if (oReturn.UpdateBasket == 'True') Basket.UpdateBasketHTML();

			//remove class from all options
			var aOptions = int.f.GetElementsByClassName('tr', 'transferOption', 'divTransfers');
			for (i = 0; i < aOptions.length; i++) {
				int.f.RemoveClass(aOptions[i], 'selected');
			}

			//add class to selected option
			int.f.AddClass('tr_Transfers_0', 'selected');

			//clear times
			if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber') != undefined)
				int.f.SetValue('txtTransfer_AddOutboundFlightNumber', '');
			if (int.f.GetObject('txtTransfer_AddReturnFlightNumber') != undefined)
				int.f.SetValue('txtTransfer_AddReturnFlightNumber', '');
		}
	}
	//#endregion

	//#region Remove by button
	Transfers.RemoveTransferByButton = function () {
		int.ff.Call('=iVectorWidgets.Transfers.RemoveTransfer', Transfers.RemoveTransferCompleteByButton);
	}

	Transfers.RemoveTransferCompleteByButton = function (sJSON) {

		var oReturn = eval('(' + sJSON + ')');

		if (oReturn.OK == 'True') {

			//update basket
			//if (oReturn.UpdateBasket == 'True') Basket.UpdateBasketHTML();

			//remove class from all options
			var aAddButtons = int.f.GetObjectsByIDPrefix('btn_TransferAdd_', 'input', 'divTransfers');
			for (i = 0; i < aAddButtons.length; i++) {
				int.f.Show('btn_TransferAdd_' + (i + 1));
				int.f.Hide('btn_TransferRemove_' + (i + 1));
				int.f.Hide('spnBooked_' + (i + 1));
			}

			//clear times
			if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber') != undefined)
				int.f.SetValue('txtTransfer_AddOutboundFlightNumber', '');
			if (int.f.GetObject('txtTransfer_AddReturnFlightNumber') != undefined)
				int.f.SetValue('txtTransfer_AddReturnFlightNumber', '');
		}
	}
	//#endregion


	Transfers.ChangeSelectedTransfer = function (sSelectedHashToken, iSelectedPosition) {
		int.f.SetValue('hidSelectedHashToken', sSelectedHashToken);
		int.f.SetValue('hidSelectedPosition', iSelectedPosition);

		//remove class from all options
		var aOptions = int.f.GetElementsByClassName('tr', 'transferOption', 'divTransfers');
		for (i = 0; i < aOptions.length; i++) {
			int.f.RemoveClass(aOptions[i], 'selected');
		}

		//add class to selected option
		int.f.AddClass('tr_Transfers_' + iSelectedPosition, 'selected');
	}


	//#region Search
	Transfers.SearchTransfers = function () {

		int.f.Show('divTransferSearchWait');
		int.f.Hide('divTransfers_HotelOnlyFlightDetails');

		var oRequest = {
			AirportID: int.f.GetValue('hidAirportID'),
			OutboundArrivalTime: int.f.GetValue('txtTransfer_OutboundFlightTime'),
			ReturnDepartureTime: int.f.GetValue('txtTransfer_ReturnFlightTime'),
			OutboundFlightCode: int.f.GetValue('txtTransfer_OutboundFlightNumber'),
			ReturnFlightCode: int.f.GetValue('txtTransfer_ReturnFlightNumber')
		};

		int.ff.Call('=iVectorWidgets.Transfers.SearchTransfers', Transfers.SearchTransfersComplete, JSON.stringify(oRequest));
	}

	Transfers.SearchTransfersComplete = function (sJSON) {

		var oReturn = JSON.parse(sJSON);

		if (oReturn.Success) {
			int.f.SetHTML('divTransferOptions', oReturn.HTML);
			int.f.Hide('divTransferSearchWait');
			int.f.Show('divTransferResults');
		}
		else {
			int.f.Hide('divTransferSearchWait');
			int.f.Show('divTransfers_HotelOnlyFlightDetails');
			web.InfoBox.Show(oReturn.Warning, 'warning');
		}
	}

	Transfers.PropertyTransferSearch = function () {

		int.f.Show('divTransferSearchWait');
		int.f.Hide('divPropertyTransferSearch');

		var oRequest = {
			PropertyReferenceID: int.f.GetValue('acpTransferPropertyIDHidden'),
			ReturnDepartureTime: int.f.GetValue('txtTransfer_ReturnPickupTime')
		};

		int.ff.Call('=iVectorWidgets.Transfers.SearchTransfers', Transfers.PropertyTransferSearchComplete, JSON.stringify(oRequest));
	}

	Transfers.PropertyTransferSearchComplete = function (sJSON) {

		var oReturn = JSON.parse(sJSON);

		if (oReturn.Success) {
			int.f.SetHTML('divTransferOptions', oReturn.HTML);
			int.f.Hide('divTransferSearchWait');
			int.f.Show('divTransferResults');
		}
		else {
			int.f.Hide('divTransferSearchWait');
			int.f.Show('divPropertyTransferSearch');
		}

	}

//#endregion

	//autocomplete property
	Transfers.GetProperties = function (sTextBoxID) {

		var sText = int.f.GetValue(sTextBoxID);
		if (sText.length < 3) {
			web.AutoComplete.AutoSuggestHideContainer(int.f.GetObject(sTextBoxID + 'Options'));
			return;
		}

		web.AutoComplete.Selected = false;

		int.ff.Call('=iVectorWidgets.Transfers.AutoCompletePropertyDropdown', web.AutoComplete.AutoCompleteDisplayResults, sText, sTextBoxID);

	}

	Transfers.AttachPlaceHolders = function () {

		if (int.f.GetObject('acpTransferPropertyID')) {
			web.Placeholder.AttachEvents('acpTransferPropertyID', int.f.GetValue('hidPropertyTransferACPPlaceholder'));
			web.Placeholder.AttachEvents(int.f.GetObject('txtTransfer_ReturnPickupTime'), 'hh:mm');
		}

		if (int.f.GetObject('txtTransfer_AddOutboundFlightNumber')) {
			web.Placeholder.AttachEvents(int.f.GetObject('txtTransfer_AddOutboundFlightNumber'), int.f.GetValue('hidTransfer_AddOutboundFlightNumberPlaceholder'));
			web.Placeholder.AttachEvents(int.f.GetObject('txtTransfer_AddReturnFlightNumber'), int.f.GetValue('hidTransfer_AddReturnFlightNumberPlaceholder'));
		}

		if (int.f.GetObject('txtTransfer_OutboundFlightTime')) {
			web.Placeholder.AttachEvents(int.f.GetObject('txtTransfer_OutboundFlightTime'), 'hh:mm');
			web.Placeholder.AttachEvents(int.f.GetObject('txtTransfer_ReturnFlightTime'), 'hh:mm');
		}

	}

	//#region Change Airport
	Transfers.ChangeAirport = function (oDropdown) {
		int.ff.Call(
			'=iVectorWidgets.Transfers.ChangeAirport',
			function (sReturn) {
				int.f.SetHTML('divTransferOptions', sReturn);
				int.f.Show('divTransferResults');
				Basket.UpdateBasketHTML();
			},
			int.dd.GetIntValue(oDropdown)
		);
	}

//#region transfer information
	Transfers.TransferInformation = new function () {

	    var me = this;
	    this.InfoItems = [];
	    this.CompleteFunction = null;

	    this.GetHTML = function (iPosition, oCompleteFunction) {

	        me.CompleteFunction = oCompleteFunction;

	        var oInfoReturn = me.GetFromCollection(iPosition);
	        if (oInfoReturn != undefined) {
	            oCompleteFunction(iPosition, oInfoReturn);
	        }
	        else {
	            int.ff.Call(
                '=iVectorWidgets.Transfers.TransferInformation',
                function (sJSON, oCompleteFunction) {
                    me.AddToCollection(iPosition, sJSON);
                },
                iPosition - 1);
	        }	        
	    }

	    this.GetFromCollection = function (iPosition) {            
	        return me.InfoItems[iPosition]
	    }

	    this.AddToCollection = function (iPosition, sJSON) {

	        me.InfoItems[iPosition] = sJSON;
	        me.CompleteFunction(iPosition, sJSON);
	       
	    }
	}
	
//#endregion


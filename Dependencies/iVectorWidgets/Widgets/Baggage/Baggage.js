
var Baggage = Baggage || {}

	//This is a mess, rewrote the handle multiple bag types, then found out we had to validate on our end, will rewrite when I have a spare moment
//-Js

Baggage.UpdateCompleteFunction = function () { Basket.UpdateBasketHTML(); };

Baggage.Update = function (oObject) {

	var iTotalPax = int.f.GetValue("hidBaggagePax");
	var bBaggageValid = true;

	//Get all the baggage tokens
	aBaggageTypes = int.f.GetObjectsByIDPrefix('hidBaggageToken_', 'input', 'divBaggage');

	//An object to hold the baggage options.
	var oBaggage = {
		BaggageOptions: []
	}


	//Loop through each object we just returned and build up the object with the tokens and the quantity.
	for (var i = 0; i <= aBaggageTypes.length - 1; i++) {
		var iIndex = aBaggageTypes[i].id.split("_");
		var iQuantity = parseInt(int.f.GetValue("ddlBaggageQuantity_" + iIndex[1]), 10);

		if (iTotalPax >= iQuantity) {

			//build up the object that will associate tokens with quanity
			var oOption = {
				Token: aBaggageTypes[i].value,
				Quantity: iQuantity
			}

			oBaggage.BaggageOptions.push(oOption);

			//update what we display to the user
			var nBaggageCost = int.f.GetValue('hidBaggageCost_' + iIndex[1]);
			var nTotalPrice = 0
			var bRoundTotal = true;

			if (int.f.GetObject('hidBaggageRoundTotal')) {
				bRoundTotal = int.f.GetValue('hidBaggageRoundTotal').toLowerCase() === 'true';
			}

			if (bRoundTotal) {
				nTotalPrice = Math.ceil(nBaggageCost * iQuantity);
			} else {
				nTotalPrice = int.n.Cent(int.n.Round(nBaggageCost * iQuantity));
			}

			int.f.SetHTML('spBaggageTotal_' + iIndex[1], nTotalPrice);
			int.f.SetValue('hidBaggageQuant_' + iIndex[1], iQuantity);

		} else {

			bBaggageValid = false;
			int.f.SetValue('ddlBaggageQuantity_' + iIndex[1], int.f.GetValue('hidBaggageQuant_' + iIndex[1]));

		};

	};

	if (bBaggageValid) {

		//convert to JSON string
		var sJSON = JSON.stringify(oBaggage);
		int.ff.Call('=iVectorWidgets.Baggage.UpdateBaggage', Baggage.UpdateCompleteFunction, sJSON);


	} else {

		web.InfoBox.Show(int.f.GetValue('hidBaggage_Invalid'), 'warning');

	};


}
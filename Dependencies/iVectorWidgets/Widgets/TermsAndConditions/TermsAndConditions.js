var TermsAndConditions = TermsAndConditions || {};


TermsAndConditions.Validate = function (oCompleteFunction) {

	web.InfoBox.Close();

	//remove error class in case it's already there
	int.f.RemoveClass(int.f.GetObject('lblTermsAndConditions'), 'error');

	//add error class if checkbox not checked
	int.f.SetClassIf('lblTermsAndConditions', 'error', int.cb.Checked('cbTermsAndConditions') == false);

	//validate all Basket T&Cs
	var oBasketTandCs = int.f.GetObjectsByIDPrefix('cbTermsAndConditions_', 'input', 'divTermsAndConditions');
	for (var i = 0; i < oBasketTandCs.length; i++) {

		var bChecked = int.cb.Checked(oBasketTandCs[i].id);
		var iID = oBasketTandCs[i].id.split('_')[2];
		var sType = oBasketTandCs[i].id.split('_')[1];

		int.f.SetClassIf('lblTermsAndConditions_' + sType + '_' + iID, 'error', bChecked == false);

	};

	//run complete function if exists
	if (oCompleteFunction != null && oCompleteFunction != undefined) {
		oCompleteFunction();
	};

};
var Errata = Errata || {};

Errata.Validate = function (oCompleteFunction) {
	//check if checkbox exists, if so validate
	var oCheckbox = int.f.GetObject('chkAcceptErrata');
	var oLabel = int.f.GetObject('lblAcceptErrata');
	if (oCheckbox != undefined && oCheckbox != null && oLabel != undefined && oLabel != null) int.f.SetClassIf(oLabel, 'error', !oCheckbox.checked);
	//run complete function if exists
	if (oCompleteFunction != null && oCompleteFunction != undefined) oCompleteFunction();
};



var Warning = new function () {


	this.Validate = function (oCompleteFunction) {

		//Only validate if we have a warning to validate
		if (int.f.GetObject('chkAcceptWarning') != undefined) {

			//remove error class in case it's already there
			int.f.RemoveClass(int.f.GetObject('chkAcceptWarning'), 'error')

			//add error class if checkbox not checked
			int.f.SetClassIf('pWarning', 'error', int.cb.Checked('chkAcceptWarning') == false);

			//We only want to run through the complete function if they have checked, otherwise the new warnings will hide
			//this one.
			if (int.cb.Checked('chkAcceptWarning')) {
				//run complete function if exists
				if (oCompleteFunction != null && oCompleteFunction != undefined) {
					oCompleteFunction();
				}
				return true

			} else {
				window.scrollTo(0, 0);
				return false
			}
		} else {
			//run complete function if exists
			if (oCompleteFunction != null && oCompleteFunction != undefined) {
				oCompleteFunction();
			}
			return true
		}
	}
}
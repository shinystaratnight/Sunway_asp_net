var CancellationCharges = new function () {


//	this.Validate = function () {

//		// if there are cancellation charges, check the customer has checked the box
//		if (f.GetObject('divCancellationCharges') != null) {
//			if (!cb.Checked('chkAcceptCancellation')) {
//				oErrorMessages.push(f.GetValue('hidConfirmBooking_CanxCahrgeError'));
//				f.AddClass('chkAcceptCancellation', 'error');
//			};
//		};

//	}

	this.Validate = function (oCompleteFunction) {

		//remove error class in case it's already there
		int.f.RemoveClass(int.f.GetObject('lblCancellationCharges'), 'error')

		//add error class if checkbox not checked
		int.f.SetClassIf('lblCancellationCharges', 'error', int.cb.Checked('chkAcceptCancellation') == false);

		//run complete function if exists
		if (oCompleteFunction != null && oCompleteFunction != undefined) {
			oCompleteFunction();
		}

	}

}
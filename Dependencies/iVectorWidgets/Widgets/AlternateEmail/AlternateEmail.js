var AlternateEmail = AlternateEmail || {};

AlternateEmail.CompleteFunction = null;


AlternateEmail.Setup = function () {
    web.Placeholder.AttachEvents('txtAlternateEmail_Email', int.f.GetValue('hidAlternateEmail_EmailPlaceholder'));
};


AlternateEmail.Validate = function (oCompleteFunction) {

    //remove error class if it exists
    int.f.RemoveClass('txtAlternateEmail_Email', 'error');

    //get email address
    var sAlternateEmailAddress = int.f.GetValue('txtAlternateEmail_Email');

    //if we have something entered but it's not an email address, error
    if (sAlternateEmailAddress != '' && web.Placeholder.NotPlaceholderText('txtAlternateEmail_Email') && !int.v.IsEmail(sAlternateEmailAddress)) {
        int.f.AddClass('txtAlternateEmail_Email', 'error');
        AlternateEmail.SetAlternateEmailComplete(oCompleteFunction);
    }

        //if we have something and it is an email, set alternate email
    else if (sAlternateEmailAddress != '' && web.Placeholder.NotPlaceholderText('txtAlternateEmail_Email') && int.v.IsEmail(sAlternateEmailAddress)) {
        int.ff.Call(
			'=iVectorWidgets.AlternateEmail.SetAlternateEmailAddress',
			function () {
			    AlternateEmail.SetAlternateEmailComplete(oCompleteFunction);
			},
			sAlternateEmailAddress
		);
    }

        //if there's no email address entered, get the default from hidden input and set
    else {
        int.ff.Call(
			'=iVectorWidgets.AlternateEmail.SetAlternateEmailAddress',
			function () {
			    AlternateEmail.SetAlternateEmailComplete(oCompleteFunction);
			},
			int.f.GetValue('hidAlternaeEmail_TradeContactEmail')
		);
    }
};


AlternateEmail.SetAlternateEmailComplete = function (oCompleteFunction) {
    if (oCompleteFunction != null && oCompleteFunction != undefined) {
        oCompleteFunction();
    }
};
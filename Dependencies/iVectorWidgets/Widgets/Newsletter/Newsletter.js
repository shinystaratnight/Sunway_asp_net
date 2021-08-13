var Newsletter = Newsletter || {};


Newsletter.Setup = function () {

    //setup placeholders
    if (int.f.GetObject('hidSubscribeEmail_Placeholder') != undefined)
        web.Placeholder.AttachEvents('txtSubscribeEmail', int.f.GetValue('hidSubscribeEmail_Placeholder'));

};


Newsletter.Inject = function (sID) {

    var oInjectContainer = int.f.GetObject(sID);
    var oNewsletter = int.f.GetObject('divNewsletter');

    oInjectContainer.appendChild(oNewsletter);
    int.f.Show(oNewsletter);

};


Newsletter.Submit = function () {

	// clear error status
	int.f.RemoveClass('txtSubscribeEmail', 'error');

	var test = int.v.IsEmail(int.f.GetValue('txtSubscribeEmail'));
	if (int.f.GetValue('txtSubscribeEmail') == '' || !int.v.IsEmail(int.f.GetValue('txtSubscribeEmail'))) {
		int.f.AddClass('txtSubscribeEmail', 'error');

		if (int.f.GetValue('hidNewsletterErrorStyle') == 'Inline') {
			int.f.Show('pFailure');
		} else {
			web.InfoBox.Show('Please enter a valid email address', 'warning');
		}		
			
	}
	else {
		int.ff.Call('=iVectorWidgets.Newsletter.Subscribe', function (bSuccess) { Newsletter.SubmitComplete(bSuccess) }, int.f.GetValue('txtSubscribeEmail'));
	}

};


Newsletter.SubmitComplete = function (bSuccess) {

	if (bSuccess) {
		int.f.Hide('pFailure');
		int.f.Show('pSuccess');
		web.InfoBox.Close();
	}
	else {
		int.f.Show('pFailure');
	}

};
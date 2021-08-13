var Deeplink = Deeplink || {}


//search
Deeplink.Search = function () {
	int.ff.Call('=iVectorWidgets.Deeplink.Search', function (sJSON) { Deeplink.SearchComplete(sJSON) });
}

Deeplink.SearchComplete = function (sJSON) {

	var oReturn = JSON.parse(sJSON);

	if (oReturn.RedirectURL.indexOf('Invalid') == -1) {
		web.Window.Redirect(oReturn.RedirectURL);
	} else {
		web.InfoBox.Show(oReturn.InvalidMessage, 'warning');
	}
}


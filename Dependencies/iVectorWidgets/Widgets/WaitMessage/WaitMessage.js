var WaitMessage = WaitMessage || {};

WaitMessage.Suppress = false;
WaitMessage.Timeout;

WaitMessage.Show = function (Type, OnCompleteFunction) {
	clearTimeout(WaitMessage.Timeout);
	WaitMessage.Suppress = false;
	int.ff.Call('=iVectorWidgets.WaitMessage.Show', function (sHTML) { WaitMessage.ShowComplete(sHTML, OnCompleteFunction); }, Type);
	WaitMessage.Timeout = setTimeout(function () { 'WaitMessage.ShowDefault(' + OnCompleteFunction + ')' }, 1000);
}

WaitMessage.ShowComplete = function (sHTML, OnCompleteFunction) {
	clearTimeout(WaitMessage.Timeout);
	if (!WaitMessage.Suppress) {
		web.ModalPopup.Show(sHTML, false, 'modalpopup waitmessage','','','','waitmessage overlay');
		if (OnCompleteFunction) OnCompleteFunction();
	}
}

WaitMessage.Hide = function () {
	web.ModalPopup.Hide();
}


WaitMessage.ShowDefault = function (OnCompleteFunction) {
	WaitMessage.Suppress = true;
	clearTimeout(WaitMessage.Timeout);
	int.f.Show('divWaitMessage');
	web.ModalPopup.Show('divWaitMessage', false, 'modalpopup waitmessage');
	if (OnCompleteFunction) OnCompleteFunction();
}

//Show TimeoutMessage
WaitMessage.ShowTimeout = function (Type) {
	int.ff.Call('=iVectorWidgets.WaitMessage.ShowTimeout', function (sHTML) { WaitMessage.ShowTimeoutComplete(sHTML); }, Type);
}

WaitMessage.ShowTimeoutComplete = function (sHTML) {
	//If there is no timeout message to display, keep showing the waitmessage
	if (sHTML != '') {
		WaitMessage.Hide();
		web.ModalPopup.Show(sHTML, false, 'modalpopup waitmessage', '', '', '', 'waitmessage overlay');
	}
}
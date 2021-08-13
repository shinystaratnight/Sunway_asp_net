var WaitMessage = WaitMessage || {};

	WaitMessage.Suppress = false;
	WaitMessage.Timeout;

	$(document).ready(function () {
		window.WaitMessage.Show = function (Type, OnCompleteFunction) {
			clearTimeout(WaitMessage.Timeout);
			WaitMessage.Suppress = false;
			int.ff.Call('=iVectorWidgets.WaitMessage.Show', function (sHTML) { WaitMessage.ShowComplete(sHTML, OnCompleteFunction); }, Type);
			WaitMessage.Timeout = setTimeout(function () { 'WaitMessage.ShowDefault(' + OnCompleteFunction + ')' }, 1000);
		}

		window.WaitMessage.ShowComplete = function (sHTML, OnCompleteFunction) {
			clearTimeout(WaitMessage.Timeout);
			if (!WaitMessage.Suppress) {
				web.JQueryModalPopup.Show(sHTML, false, 'waitmessage');
				if (OnCompleteFunction) OnCompleteFunction();
			}
		}

		window.WaitMessage.Hide = function () {
			web.JQueryModalPopup.Hide();
		}

		window.WaitMessage.ShowDefault = function (OnCompleteFunction) {
			WaitMessage.Suppress = true;
			clearTimeout(WaitMessage.Timeout);
			int.f.Show('divWaitMessage');
			web.JQueryModalPopup.Show('divWaitMessage', false, 'waitmessage');
			if (OnCompleteFunction) OnCompleteFunction();
		}

		window.WaitMessage.ShowTimeout = function (Type) {
			int.ff.Call('=iVectorWidgets.WaitMessage.ShowTimeout', function (sHTML) { WaitMessage.Override.ShowTimeoutComplete(sHTML); }, Type);
		}

		window.WaitMessage.ShowTimeoutComplete = function (sHTML) {
			//If there is no timeout message to display, keep showing the waitmessage
			if (sHTML != '') {
				WaitMessage.Override.Hide();
				web.JQueryModalPopup.Show(sHTML, false, 'waitmessage');
			}
		}
	})
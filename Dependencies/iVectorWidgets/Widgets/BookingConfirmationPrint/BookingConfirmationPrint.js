
var BookingConfirmationPrint = new function () {

	this.ViewDocumentation = function (sBookingReference, sBookingDocument) {

		int.ff.Call('=iVectorWidgets.BookingConfirmationPrint.ShowDocumentation',
            function (sJSON) { BookingConfirmationPrint.ViewDocumentationDone(sJSON) },
            sBookingReference, sBookingDocument
        );
	};

	this.ViewDocumentationDone = function (sJSON) {

		var oReturn = eval('(' + sJSON + ')');

		if (oReturn['OK'] == true && oReturn.DocumentPaths.length > 0) {
			for (var i = 0; i < oReturn.DocumentPaths.length; i++) {
				window.open(oReturn.DocumentPaths[i], 'documentation' + i, 'toolbar=no,location=no,menubar=no,copyhistory=no,status=no,directories=no');
			};
		}
		else {
			web.InfoBox.Show('Sorry, there was a problem generating your documentation.', 'warning');
		};

	};

}
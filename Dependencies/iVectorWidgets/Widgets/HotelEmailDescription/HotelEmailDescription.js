
var HotelEmailDescription = new function () {

	var me = this;

	this.Setup = function () {

	}


	this.ShowPopup = function (iPropertyReferenceID) {

		int.ff.Call('=iVectorWidgets.HotelEmailDescription.BuildPopup',
            function (sHTML) {
            	HotelEmailDescription.ShowPopupDone(sHTML);
            	web.Placeholder.AttachEvents('txtHotelEmailDescription_EmailYourAddress', int.f.GetValue('hidEmailPlaceholder'));
            	web.Placeholder.AttachEvents('txtHotelEmailDescription_EmailRecipientAddress', int.f.GetValue('hidRecipientPlaceholder'));
            	web.Placeholder.AttachEvents('txtHotelEmailDescription_EmailMessage', int.f.GetValue('hidMessagePlaceholder'));
            	//Silly hack can't render blank text area without it collapsing in IE, need to include space in XSL, and then remove it
				//in JS to show the place holder.
				int.f.SetValue('txtHotelEmailDescription_EmailMessage', '');
            },
            iPropertyReferenceID
        );
	}

	this.ShowPrintPopup = function (iPropertyReferenceID) {

		int.ff.Call('=iVectorWidgets.HotelEmailDescription.BuildPrintPopup',
		function (sHTML) {
            HotelEmailDescription.ShowPopupDone(sHTML);
		},
		iPropertyReferenceID
	);
	}

	this.ShowPopupDone = function (sHTML) {
		//show popup
		web.ModalPopup.Show(sHTML, true);
	}

	this.ClosePopup = function () {
		//hide popup
		web.ModalPopup.Hide();
	};

	//#region Email 

	this.EmailDescription = function (iPropertyReferenceID) {

		int.f.Hide('pHotelPopup_EmailDone');

		int.f.SetClassIf('txtHotelEmailDescription_EmailRecipientAddress', 'error', int.f.GetValue('txtHotelEmailDescription_EmailRecipientAddress') == '' || !int.v.IsEmail(int.f.GetValue('txtHotelEmailDescription_EmailRecipientAddress')));
		int.f.SetClassIf('txtHotelEmailDescription_EmailYourAddress', 'error', int.f.GetValue('txtHotelEmailDescription_EmailYourAddress') == '' || !int.v.IsEmail(int.f.GetValue('txtHotelEmailDescription_EmailYourAddress')));

		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divHotelEmailDescription');

		//show error message if required
		if (aErrorControls.length > 0) {
			int.f.Show('pHotelEmailDescription_EmailError');
		}
		else {
			int.f.Hide('pHotelEmailDescription_EmailError');

			//call ff
			int.ff.Call('=iVectorWidgets.HotelEmailDescription.SendEmail',
                function (sJSON) {
                	HotelEmailDescription.EmailDetailsDone(sJSON);
                },
                int.f.GetValue('txtHotelEmailDescription_EmailRecipientAddress'), int.f.GetValue('txtHotelEmailDescription_EmailYourAddress'), int.f.GetValue('txtHotelEmailDescription_EmailMessage'), iPropertyReferenceID);
		};

	};


	this.EmailDetailsDone = function (sJSON) {
		int.f.Show('pHotelEmailDescription_EmailDone');
	};

}

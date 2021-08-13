var ContactUs = ContactUs || {}

	ContactUs.Setup = function () {

		//setup placeholders
		if (int.f.GetObject('hidContact_NamePlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtName', int.f.GetValue('hidContact_NamePlaceholder'));
		if (int.f.GetObject('hidContact_EmailPlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtEmail', int.f.GetValue('hidContact_EmailPlaceholder'));
		if (int.f.GetObject('hidContact_PhonePlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtPhoneNumber', int.f.GetValue('hidContact_PhonePlaceholder'));
		if (int.f.GetObject('hidContact_MessagePlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtMessage', int.f.GetValue('hidContact_MessagePlaceholder'));

		//Silly hack can't render blank text area without it collapsing in IE, need to include space in XSL, and then remove it
		//in JS to show the place holder.
		int.f.SetValue('txtMessage', '');

	}

	ContactUs.Submit = function () {

		if (!ContactUs.Validate()) { return };

		var sQueryString = int.f.GetContainerQueryString('divContactUsForm');
		int.ff.Call('=iVectorWidgets.ContactUs.SendEmail', function () { int.f.Show('pThankYou'); }, sQueryString);

	};


	ContactUs.Validate = function () {

		var aErrors = int.f.GetElementsByClassName('*', 'error', 'divContactUsForm');
		for (var i = 0; i < aErrors.length; i++) {
			int.f.RemoveClass(aErrors[i], 'error');
		};

		var iErrors = 0;

		if (int.f.GetValue('txtName') == '') { iErrors += 1; int.f.AddClass('txtName', 'error'); };
		if (int.f.GetValue('txtEmail') == '') { iErrors += 1; int.f.AddClass('txtEmail', 'error'); };
		if (int.s.Trim(int.f.GetValue('txtMessage')) == '') { iErrors += 1; int.f.AddClass('txtMessage', 'error'); };

		if (iErrors > 0) {
			web.InfoBox.Show(int.f.GetValue('hidWarningText'), 'warning');
			return false;
		} else {
			return true;
		};

	};
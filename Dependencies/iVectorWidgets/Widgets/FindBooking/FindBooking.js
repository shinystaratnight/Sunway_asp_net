var FindBooking = new function () {

	var me = this;

	//#region setup

	this.Setup = function () {

		var sImageFolder = int.f.GetValue("hidImageFolder");

		//setup placeholders
		if (int.f.GetObject('hidFindBooking_ReferencePlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtReference', int.f.GetValue('hidFindBooking_ReferencePlaceholder'));
		if (int.f.GetObject('hidFindBooking_GuestNamePlaceholder') != undefined)
			web.Placeholder.AttachEvents('txtGuestName', int.f.GetValue('hidFindBooking_GuestNamePlaceholder'));

		//setup datepickers
		var dMinDate = new Date();
		dMinDate.setMonth(dMinDate.getMonth() - 3);
		var sMinDate = int.d.DatepickerDate(dMinDate);
		var iDatepickerMonths = int.f.GetIntValue('hidFindBookings_DatepickerMonths');
		var iDatepickerFirstDay = int.f.GetIntValue('hidFindBookings_DatepickerFirstDay');

		web.DatePicker.Setup('#txtBookedStartDate', sMinDate, dMinDate, iDatepickerMonths, iDatepickerFirstDay, '', '', '', false);
		web.DatePicker.Setup('#txtBookedEndDate', sMinDate, dMinDate, iDatepickerMonths, iDatepickerFirstDay, '', '', '', false);
		web.DatePicker.Setup('#txtArrivalStartDate', sMinDate, dMinDate, iDatepickerMonths, iDatepickerFirstDay, '', '', '', false);
		web.DatePicker.Setup('#txtArrivalEndDate', sMinDate, dMinDate, iDatepickerMonths, iDatepickerFirstDay, '', '', '', false);

		this.ChangeDates('Arrival');
		this.ChangeDates('Booked');

	}

	//#endregion	


	//#region Validate
	this.Validate = function () {

		//		// If there is already a divInfo then clear it down, we want to start again
		//		//		FormHandler.CloseInfo();

		//		// Create an array for us to add messages to
		//		var oErrorMessages = new Array();

		//		// check there is a number in the reference field
		//		var iReference = int.f.GetValue('txtReference');
		//		iReference = int.n.SafeInt(iReference);

		//		if (iReference == 0 && int.f.GetValue('txtReference') != '') oErrorMessages.push(int.f.GetValue('hidFindBooking_ValidRef'));


		//		// If there are any error messages then show them, otherwise do a search
		//		if (oErrorMessages.length > 0) {
		//			//			FormHandler.ShowWarning(oErrorMessages);
		//		}
		//		else {
		//			//			FormHandler.CloseInfo();
		me.FilterBookings();
		//	};

	}
	//#endregion


	//#region Filter

	this.FilterBookings = function () {

		var oParams = {
		};

		//Booking reference
		oParams.Reference = web.Placeholder.NotPlaceholderText('txtReference') ? int.f.GetValue('txtReference') : '';

		// Guest Name
		var sGuestName = web.Placeholder.NotPlaceholderText('txtGuestName') ? int.f.GetValue('txtGuestName') : '';
		oParams.GuestName = sGuestName;

		// Booked start date
		var sBookedStartDate = int.f.GetValue('txtBookedStartDate');
		if (sBookedStartDate != '') {
			oParams.BookedStartDate = int.d.New(sBookedStartDate.split('/')[0], sBookedStartDate.split('/')[1], sBookedStartDate.split('/')[2]);
		};

		// Booked end date
		var sBookedEndDate = int.f.GetValue('txtBookedEndDate');
		if (sBookedEndDate != '') {
			oParams.BookedEndDate = int.d.New(sBookedEndDate.split('/')[0], sBookedEndDate.split('/')[1], sBookedEndDate.split('/')[2]);
		};

		// Arrival start date
		var sArrivalStartDate = int.f.GetValue('txtArrivalStartDate');
		if (sArrivalStartDate != '') {
			oParams.ArrivalStartDate = int.d.New(sArrivalStartDate.split('/')[0], sArrivalStartDate.split('/')[1], sArrivalStartDate.split('/')[2]);
		};

		// Arrival end date
		var sArrivalEndDate = int.f.GetValue('txtArrivalEndDate');
		if (sArrivalEndDate != '') {
			oParams.ArrivalEndDate = int.d.New(sArrivalEndDate.split('/')[0], sArrivalEndDate.split('/')[1], sArrivalEndDate.split('/')[2]);
		};

		// Booking date dropdown
		var sBooked = int.f.GetValue('sddBookedDateRange');
		oParams.Booked = sBooked;

		// Booking date dropdown
		var sArrival = int.f.GetValue('sddArrivalDateRange');
		oParams.Arrival = sArrival;


		// convert the parameters object to a JSON string
		var sJSONParams = JSON.stringify(oParams);

		// if we are already on my bookings page then show the results, else go to the page

		int.ff.Call('=iVectorWidgets.TradeBookings.FilterBookings', FindBooking.FilterComplete, sJSONParams);

	}


	//#region ChangeDates

	this.ChangeDates = function (sTextBox) {

		if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Today') {
			var dDateYesterday = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), -1));
			var dDateTomorrow = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), 1));
			int.f.SetValue('txt' + sTextBox + 'StartDate', dDateYesterday);
			int.f.SetValue('txt' + sTextBox + 'EndDate', dDateTomorrow);
			int.f.Hide('div' + sTextBox + 'StartDate');
			int.f.Hide('div' + sTextBox + 'EndDate');
		}
		else if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Yesterday') {
			var dDate1 = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), -2));
			var dDate2 = int.d.DatepickerDate(int.d.Today());
			int.f.SetValue('txt' + sTextBox + 'StartDate', dDate1);
			int.f.SetValue('txt' + sTextBox + 'EndDate', dDate2);
			int.f.Hide('div' + sTextBox + 'StartDate');
			int.f.Hide('div' + sTextBox + 'EndDate');
		}
		else if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Last Week') {
			var dDate1 = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), -8));
			var dDate2 = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), 1));
			int.f.SetValue('txt' + sTextBox + 'StartDate', dDate1);
			int.f.SetValue('txt' + sTextBox + 'EndDate', dDate2);
			int.f.Hide('div' + sTextBox + 'StartDate');
			int.f.Hide('div' + sTextBox + 'EndDate');
		}
		else if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Last Fortnight') {
			var dDate1 = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), -15));
			var dDate2 = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), 1));
			int.f.SetValue('txt' + sTextBox + 'StartDate', dDate1);
			int.f.SetValue('txt' + sTextBox + 'EndDate', dDate2);
			int.f.Hide('div' + sTextBox + 'StartDate');
			int.f.Hide('div' + sTextBox + 'EndDate');
		}
		else if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Range') {
			var dNow = int.d.Today();
			if (int.f.GetValue('hidCustomDateChange') == '') {
				int.f.SetValue('txt' + sTextBox + 'StartDate', int.d.DatepickerDate(dNow));
				int.f.SetValue('txt' + sTextBox + 'EndDate', int.d.DatepickerDate(dNow));
			};
			int.f.Show('div' + sTextBox + 'StartDate');
			int.f.Show('div' + sTextBox + 'EndDate');
			int.f.RemoveClass('divFindBooking', 'close');
			int.f.AddClass('divFindBooking', 'open');
		}
		else if (int.dd.GetValue('sdd' + sTextBox + 'DateRange') == 'Any') {
			var dDate = int.d.DatepickerDate(int.d.AddDays(int.d.Today(), 1));
			int.f.SetValue('txt' + sTextBox + 'StartDate', '');
			int.f.SetValue('txt' + sTextBox + 'EndDate', '');
			int.f.Hide('div' + sTextBox + 'StartDate');
			int.f.Hide('div' + sTextBox + 'EndDate');
		};

	};

	//#endregion


	//#region Filter Complete
	this.FilterComplete = function (sJSON) {

		//var oFilters = eval('(' + sJSON + ')');

		//		me.UpdateFilterOptions(oFilters);

		TradeBookings.UpdateResults();

	}
	//#endregion

}
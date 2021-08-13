var MyBookingsLogin = MyBookingsLogin || {};

MyBookingsLogin.RedirectURL = '';
MyBookingsLogin.InsertWarningAfter = '';

//1. Setup
MyBookingsLogin.Setup = function (sURL) {

    var dToday = new Date;
    var dDisplayDate = MyBookingsLogin.FormatDate(dToday);
    MyBookingsLogin.RedirectURL = sURL;
    MyBookingsLogin.InsertWarningAfter = int.f.GetValue('hidInsertWarningAfter');
    var sDatepickerCultureCode = int.f.GetValue('hidDatepickerCultureCodeMyBookings');
    int.f.SetValue('txtDepartureDate', dDisplayDate);

    var oDatePickerOptions = {
        ID: '#txtDepartureDate',
        MinDate: dToday,
        DefaultDate: dToday,
        UseShortDates: true,
        LanguageCode: sDatepickerCultureCode
    };

    web.DatePicker.SetupFromObject(oDatePickerOptions);

};

MyBookingsLogin.FormatDate = function (dRawDate) {

    var dMonth = dRawDate.getMonth() + 1;

    if (dMonth < 10) {
        dMonth = '0' + dMonth;
    };

    return dRawDate.getDate() + '/' + dMonth + '/' + dRawDate.getFullYear();

};

MyBookingsLogin.Validate = function (sLoginType) {



    if (sLoginType === "BookingDetails") {
        int.f.SetClassIf('txtBookingRef', 'error', int.f.GetValue('txtBookingRef') === '');
        int.f.SetClassIf('txtLastName', 'error', int.f.GetValue('txtLastName') === '');
        int.f.SetClassIf('txtDepartureDate', 'error', int.f.GetValue('txtDepartureDate') === '');
    } else if (sLoginType === "EmailAndReference") {
        int.f.SetClassIf('txtEmailAddress', 'error', int.f.GetValue('txtEmailAddress') === '');
        int.f.SetClassIf('txtBookingRef', 'error', int.f.GetValue('txtBookingRef') === '');
    } else if (sLoginType === "EmailAndPassword") {
        int.f.SetClassIf('txtEmailAddress', 'error', int.f.GetValue('txtEmailAddress') === '');
        int.f.SetClassIf('txtPassword', 'error', int.f.GetValue('txtPassword') === '');
    }

    var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divBookingLogin');

    if (aErrorControls.length === 0) {

        var oLoginDetails = {};

        oLoginDetails.BookingReference = int.f.GetValue('txtBookingRef');

        if (sLoginType === "BookingDetails") {
            oLoginDetails.LastName = int.f.GetValue('txtLastName');
            oLoginDetails.DepartureDate = int.f.GetValue('txtDepartureDate');
        } else if (sLoginType === "EmailAndReference") {
            oLoginDetails.EmailAddress = int.f.GetValue('txtEmailAddress');
        } else if (sLoginType === "EmailAndPassword") {
            oLoginDetails.EmailAddress = int.f.GetValue('txtEmailAddress');
            oLoginDetails.Password = int.f.GetValue('txtPassword');
        };
        oLoginDetails.LoginType = sLoginType

        var sJSONDetails = JSON.stringify(oLoginDetails);

        int.ff.Call('=iVectorWidgets.MyBookingsLogin.Login', function (sJSON) { MyBookingsLogin.LoginComplete(sJSON); }, sJSONDetails);

    } else {
        web.InfoBox.Show('Please make sure all fields are entered correctly. Incorrect fields have been highlighted.', 'warning', null, MyBookingsLogin.InsertWarningAfter);
    }

}

MyBookingsLogin.LoginComplete = function (sJSON) {

    var oCustomerLoginReturn = eval('(' + sJSON + ')');

    if (oCustomerLoginReturn['OK'] === true) {
        if (MyBookingsLogin.RedirectURL !== '') {
            web.Window.Redirect(MyBookingsLogin.RedirectURL);
        } else {
            web.Window.Redirect('/my-bookings');
        }

    } else {

        web.InfoBox.Show('Sorry, your login details were incorrect. Please check and try again.', 'warning', null, MyBookingsLogin.InsertWarningAfter);

    };
};


MyBookingsLogin.SendReminder = function () {

    MyBookingsLogin.ShowWait('aReminder');

    var sEmail = int.f.GetValue('txtEmailAddress');

    // validate email field
    int.f.SetClassIf('txtEmailAddress', 'error', !MyBookingsLogin.IsEmail(sEmail));
    int.f.RemoveClass('txtBookingRef', 'error');

    var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divBookingLogin');

    if (aErrorControls.length === 0) {
        int.ff.Call('=iVectorWidgets.MyBookingsLogin.SendBookingReferencesEmail', function (sReturn) { MyBookingsLogin.SendReminder_Done(sReturn); }, sEmail);
    }
    else {
        MyBookingsLogin.RemoveWait('aReminder');
        web.InfoBox.Show('Please complete the highlighted fields.', 'warning', null, MyBookingsLogin.InsertWarningAfter);
    }

} // SendReminder


MyBookingsLogin.SendReminder_Done = function (sReturn) {

    if (sReturn === 'failed') {
        web.InfoBox.Show('There are no bookings associated with the specified email.', 'warning', null, MyBookingsLogin.InsertWarningAfter);
        MyBookingsLogin.RemoveWait('aReminder');
    }
    else {
        int.f.SetClass('aReminder', 'done');
        int.f.GetObject('aReminder').setAttribute('onclick', '');
        int.f.SetHTML('aReminder', 'A reminder has been sent to the email address provided');
    }

} // SendReminder_Done


MyBookingsLogin.IsEmail = function (sEmail) {
    var sEmailRegEx = /^[a-zA-Z0-9._-]+@([a-zA-Z0-9.-]+\.)+[a-zA-Z0-9.-]{2,4}$/;
    var o = new RegExp(sEmailRegEx);
    return o.test(sEmail);
} // IsEm


MyBookingsLogin.ShowWait = function (sElement) {
    int.f.AddClass(sElement, 'working');
    MyBookingsLogin.RemoveText(sElement);
} // ShowWait


MyBookingsLogin.RemoveWait = function (sElement) {
    int.f.RemoveClass(sElement, 'working');
    MyBookingsLogin.SetText(sElement);
} // RemoveWait


MyBookingsLogin.SetText = function (sElement) {
    if (sElement === 'aLogin') {
        int.f.SetHTML(sElement, 'Login');
    }
    else if (sElement === 'aReminder') {
        int.f.SetHTML(sElement, '> I do not remember or cannot find my Booking reference.');
    }
} // SetText

MyBookingsLogin.RemoveText = function (sElement) {
    int.f.SetHTML(sElement, ' ');
} // RemoveText
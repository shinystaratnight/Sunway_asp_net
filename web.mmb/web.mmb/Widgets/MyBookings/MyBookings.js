var MyBookings = MyBookings || {};

MyBookings.PropertyBookingReference = "";

MyBookings.ShowAmendmentPopup = function (sPropertyBookingReference) {

    MyBookings.PropertyBookingReference = sPropertyBookingReference;

    //get data
    var sPropertyBookingData = int.f.GetValue('hidAmendData_' + sPropertyBookingReference);

    if (sPropertyBookingData != '') {
        MyBookings.SetValues(sPropertyBookingData);
    }

    //show popup
    //web.ModalPopup.Show('divAmendment', true, 'modalpopup container');
    web.JQueryModalPopup.Show('divAmendment', true, 'modalpopup container jquery');
    //attach hide popup to overlay
    int.f.AttachEvent('divOverlay', 'click',
        function () {
            web.JQueryModalPopup.Hide();
        });

};

MyBookings.ChangePropertyDetails = function (event) {

    var sPropertyBookingData = int.f.GetValue('hidProperty_' + event.target.value);

    if (sPropertyBookingData != '') {
        MyBookings.SetValues(sPropertyBookingData);
    }

};

MyBookings.SetValues = function (sPropertyBookingData) {
    var oData = sPropertyBookingData.split('#');

    //set hotel name
    int.f.SetValue('txtHotelName', oData[0]);

    if (document.getElementById('ddlHotelName') == '') {
        document.getElementById('txtHotelName').disabled = true;
    }
    //set Destination
    int.f.SetValue('txtDestination', oData[3]);
    document.getElementById('txtDestination').disabled = true;

    //set dates
    var dDefaultDepartureDate = new Date(oData[1].substring(0, 4), oData[1].substring(5, 7) - 1, oData[1].substring(8, 10));
    int.f.SetValue('txtAmendmentDepartureDate', MyBookings.FormatDate(dDefaultDepartureDate));
    document.getElementById('txtAmendmentDepartureDate').disabled = true;
    var dDefaultReturnDate = new Date(oData[2].substring(0, 4), oData[2].substring(5, 7) - 1, oData[2].substring(8, 10));
    int.f.SetValue('txtAmendmentReturnDate', MyBookings.FormatDate(dDefaultReturnDate));
    document.getElementById('txtAmendmentReturnDate').disabled = true;
}

MyBookings.FormatDate = function (dRawDate) {
    var dMonth = dRawDate.getMonth() + 1;
    if (dMonth < 10) {
        dMonth = '0' + dMonth;
    };
    return dRawDate.getDate() + '/' + dMonth + '/' + dRawDate.getFullYear();
};

MyBookings.RequestAmendmentValidate = function (event) {

    //check date and duration
    var sDepartureDate = int.f.GetValue('txtAmendmentDepartureDate');
    var sReturnDate = int.f.GetValue('txtAmendmentReturnDate');
    var dDepartureDate = int.d.New(sDepartureDate.split('/')[0], sDepartureDate.split('/')[1], sDepartureDate.split('/')[2]);
    var dReturnDate = int.d.New(sReturnDate.split('/')[0], sReturnDate.split('/')[1], sReturnDate.split('/')[2]);
    var iNights = int.d.DateDiff(dDepartureDate, dReturnDate);
    int.f.SetClassIf('txtAmendmentReturnDate', 'error', iNights < 1);
    int.f.SetClassIf('txtAmendmentDepartureDate', 'error', !int.d.IsDate(dDepartureDate) || int.f.GetValue('txtAmendmentDepartureDate') == '');
    int.f.SetClassIf('txtAmendmentReturnDate', 'error', !int.d.IsDate(dReturnDate) || int.f.GetValue('txtAmendmentReturnDate') == '' || iNights < 1);

    //check no of errors 
    var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divAmendment');

    if (aErrorControls.length == 0) {
        //if no errors show and hide
        int.f.Hide('divAmendmentWarning');
        int.f.Hide('divAmendmentForm');
        int.f.Show('divAmendRequestWaitMessage');

        if (int.f.GetValue('txtHotelName') != '') {
            var sHotelName = int.f.GetValue('txtHotelName');
            var sPropertyBookingReference = MyBookings.PropertyBookingReference;
        } else {
            var sHotelName = int.dd.GetText('ddlHotelName');
            var sPropertyBookingReference = int.dd.GetValue('ddlHotelName');
        }

        //and do ff.call
        int.ff.Call('Widgets.MyBookings.RequestHotelAmendment',
            function (sResult) {
                MyBookings.RequestAmendmentComplete(sResult);
            },
            int.f.GetValue('hidBookingReference'),
            sPropertyBookingReference,
            dDepartureDate,
            dReturnDate,
            int.f.GetValue('txtDestination'),
            sHotelName,
            int.f.GetValue('txtAdditionalInformation'));
    }
    else {
        //if errors show warning
        int.f.Show('divAmendmentWarning');
    }
};

MyBookings.RequestAmendmentComplete = function (sResult) {
    //if success show sent message
    if (sResult == 'Success') {
        int.f.Hide('divAmendRequestWaitMessage');
        int.f.Hide('divAmendmentForm');
        int.f.Show('divRequestSent');
    }
    else {
        int.f.Hide('divAmendRequestWaitMessage');
        int.f.Hide('divAmendmentForm');
        int.f.Show('divRequestNotSent');
    }
};

MyBookings.SendDocumentation = function () {

    web.ModalPopup.Show('divEmailDocuments');

    var sBookingReference = int.f.GetValue('hidBookingReference');

    int.ff.Call('Widgets.MyBookings.SendCustomerDocumentation',
        function (sJSON) { MyBookings.SendDocumentationDone(sJSON) },
        sBookingReference
    );
};

MyBookings.SendDocumentationDone = function (sJSON) {

    var oReturn = eval('(' + sJSON + ')');

    if (oReturn['OK'] == true) {

        int.f.Hide('divEmailWaitMessage');
        int.f.Show('divEmailSuccess');
        setTimeout('web.ModalPopup.Hide()', 2000);
    }
    else {
        web.ModalPopup.Hide();
        web.InfoBox.Show('Sorry, there was a problem sending your documentation.', 'warning');
    };

};


MyBookings.GetPaymentPopup = function () {
    int.ff.Call('Widgets.MyBookings.GetPaymentForm',
        function (sJson) { MyBookings.GetPaymentPopupDone(sJson) }
    );
}

MyBookings.GetPaymentPopupDone = function (sJson) {

    var oResult = JSON.parse(sJson);

    if (oResult.Success === true) {
        var oPaymentHolder = int.f.GetObject('divPaymentDetailsHolder');

        if (oPaymentHolder) {
            web.JQueryModalPopup.Show(oResult.HTML, true, 'modalpopup container jquery payment');

            var btnPayBalanceShow = document.getElementById('btnPayBalance');
            if (btnPayBalanceShow != null) {
                int.f.AttachEvent('btnPayBalance', 'click', function () {
                    MyBookings.MakePayment(oResult.UseOffsitePayment);
                });
            };

            var iTotalPrice = int.f.GetValue('hidTotalPrice');
            var iTotalPaid = int.f.GetValue('hidTotalPaid');
            var iTotalOutStanding = int.f.GetValue('hidOutstandingAmount');

            var hidAmount = int.f.GetObject('hidAmount');
            if (hidAmount) {
                int.f.SetValue(hidAmount, iTotalOutStanding);
            }

            var sCurrencySymbol = oResult.CurrencySymbol;
            var sCurrencySymbolPosition = oResult.CurrencySymbolPosition;

            int.f.SetHTML('spnMyBookingsPaymentTotalPrice', int.n.FormatMoney(iTotalPrice, sCurrencySymbol, sCurrencySymbolPosition));
            int.f.SetHTML('spnMyBookingsPaymentTotalPaid', int.n.FormatMoney(iTotalPaid, sCurrencySymbol, sCurrencySymbolPosition));
            int.f.SetHTML('spnMyBookingsPaymentTotalOutstanding', int.n.FormatMoney(iTotalOutStanding, sCurrencySymbol, sCurrencySymbolPosition));
            int.f.SetHTML('spnAmount', sCurrencySymbol);
            int.f.SetValue('txtAmount', iTotalOutStanding);
        }

        int.f.AttachEvent('txtAmount', 'change', function () {
            MyBookings.SetSurchargeAmount();
        });
    }
}

MyBookings.ShowPaymentPopup = function (iBookingReference) {

    //show popup
    web.JQueryModalPopup.Show('divPaymentDetails', true, 'modalpopup container');
    //attach hide popup to overlay
    int.f.AttachEvent('divOverlay', 'click',
        function () {
            web.JQueryModalPopup.Hide();
        });

    //add csv tooltip
    var oCSVTooltipText = int.f.SafeObject('hidCSVTooltipText');
    if (oCSVTooltipText) {
        var sCSVTooltipText = oCSVTooltipText.value;
        var oCSVLink = document.getElementById('aPayment_CSVTooltip');
        int.f.AttachEvent(oCSVLink,
            'mouseover',
            function () {
                web.Tooltip.Show(oCSVLink, sCSVTooltipText, 'top', null, -50, 0, 'csvTooltip');
            }
        );
        int.f.AttachEvent(oCSVLink,
            'mouseout',
            function () {
                web.Tooltip.Hide();
            }
        );
    }
};

MyBookings.MakePayment = function (bUseOffsitePayment) {

    //if valid
    if (MyBookings.ValidatePayment(bUseOffsitePayment)) {

        //show and hide stuff
        int.f.Hide('divPaymentDetailsWarning');
        int.f.Hide('divPaymentDetailsForm');
        int.f.Show('divPaymentDetailsSending');

        var oPaymentDetails = MyBookings.GetPaymentDetails(bUseOffsitePayment);

        oPaymentDetails.BillingAddress = MyBookings.GetBillingAddress();

        var sPaymentDetails = JSON.stringify(oPaymentDetails);

        //do ff.call 
        int.ff.Call('Widgets.MyBookings.MakePayment',
            function (sJson) {
                MyBookings.MakePaymentComplete(sJson);
            },
            sPaymentDetails
        );
    }
    else {
        //if not valid
        int.f.Show('divPaymentDetailsWarning');
    }
}

MyBookings.GetPaymentDetails = function (bUseOffsitePayment) {
    var paymentDetails = {
        Amount: bUseOffsitePayment ? parseFloat(int.f.GetValue('hidOutstandingAmount')) : parseFloat(int.f.GetValue('txtAmount')),
        CCCardHoldersName: bUseOffsitePayment ? int.f.GetValue('txtCCCardHoldersName') : '',
        CCCardTypeID: bUseOffsitePayment ? 0 : int.dd.GetValue('ddlCCCardTypeID'),
        CCCardNumber: bUseOffsitePayment ? int.f.GetValue('txtCCCardNumber') : '',
        CCSecurityCode: bUseOffsitePayment ? int.f.GetValue('txtCCSecurityCode') : '',
        CCExpireMonth: bUseOffsitePayment ? int.dd.GetValue('ddlCCExpireMonth') : '',
        CCExpireYear: bUseOffsitePayment ? int.dd.GetValue('ddlCCExpireYear') : '',
        Surcharge: parseFloat(int.f.GetValue('hidSurcharge'))
    }

    paymentDetails.TotalAmount = paymentDetails.Amount + paymentDetails.Surcharge;

    if (!bUseOffsitePayment) {

        if (int.f.GetValue('txtCCIssueNumber') !== '') {
            paymentDetails.CCIssueNumber = parseInt(int.f.GetValue('txtCCIssueNumber'), 10);
        }
        if (int.dd.GetValue('ddlCCStartMonth') !== '') {
            paymentDetails.CCStartMonth = parseInt(int.dd.GetValue('ddlCCStartMonth'), 10);
        }
        if (int.dd.GetValue('ddlCCStartYear') !== '') {
            paymentDetails.CCStartYear = parseInt(int.dd.GetValue('ddlCCStartYear'), 10);
        }

    }

    return paymentDetails;
}

MyBookings.GetBillingAddress = function () {

    var billingaddress = new Object();
    var prefix = '';
    if (int.cb.Checked('radBillingAddress')) {
        prefix = 'hid';
    } else if (int.cb.Checked('radNewBillingAddress')) {
        prefix = 'txt';
    }

    billingaddress.Address1 = int.f.GetValue(prefix + 'Address1');
    billingaddress.Address2 = int.f.GetValue(prefix + 'Address2');
    billingaddress.TownCity = int.f.GetValue(prefix + 'TownCity');
    billingaddress.Postcode = int.f.GetValue(prefix + 'Postcode');

    return billingaddress;
};

MyBookings.MakePaymentComplete = function (sJson) {

    var oResult = JSON.parse(MyBookings.RemoveScriptsFromJson(sJson));

    switch (oResult.ReturnType) {
        case "SecureRedirect":
            MyBookings.SecureRedirect(oResult);
            break;
        case "Payment":
            MyBookings.PaymentComplete(oResult);
            break;
        case "Offsite Payment":
            MyBookings.GetOffsitePaymentComplete(oResult);
            break;
    }
};

MyBookings.RemoveScriptsFromJson = function (sJson) {
    // hack to strip out scripts (probably added by new reilc) that are breaking this
    return sJson.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, "");
};

MyBookings.PaymentComplete = function (oResult) {
    if (oResult.Success === true) {

        var sCurrencySymbol = int.f.GetValue('hidCurrencySymbol');
        var sCurrencySymbolPosition = int.f.GetValue('hidCurrencySymbolPosition');

        if (oResult.TotalOutstanding) {
            int.f.SetHTML('spnTotalOutstanding_' + oResult.BookingReference, int.n.FormatMoney(oResult.TotalOutstanding, sCurrencySymbol, sCurrencySymbolPosition));
            int.f.SetValue('hidOutstandingAmount', oResult.TotalOutstanding);
        }

        if (oResult.TotalPaid) {
            int.f.SetHTML('spnTotalPaid_' + oResult.BookingReference, int.n.FormatMoney(oResult.TotalPaid, sCurrencySymbol, sCurrencySymbolPosition));
            int.f.SetValue('hidTotalPaid', oResult.TotalPaid);
        }

        //show and hide stuff
        int.f.Hide('divPaymentDetailsSending');
        int.f.Show('divPaymentDetailsSent');
    }
    else {
        //show and hide stuff
        int.f.Hide('divPaymentDetailsSending');
        int.f.Show('divPaymentDetailsNotSent');
    }
}

MyBookings.SecureRedirect = function () {
    window.location = '/threedsecure.aspx';
}

MyBookings.GetOffsitePaymentComplete = function (oReturn) {

    if (oReturn.Success) {
        document.write(oReturn.HTML);

        //Hide HPP form data before redirect occurs. CSS unloaded at this point
        document.getElementsByName("frm1")[0].style.display = "none";

        onload();
    } else {
        int.f.Hide('divPaymentWaitMessage');
        web.InfoBox.Show(oReturn.Warnings, 'warning');
        web.ModalPopup.Hide();
    }
};

MyBookings.ProcessOffsitePaymentComplete = function (sReturn) {
    var oReturn = JSON.parse(sReturn);
    if (oReturn.Success === true) {

        //do the payment recording
        //CompleteBooking.Book();

    } else {
        web.Window.Replace(int.f.GetValue('hidFailURL') + '?warn=bookfailed');
    }
};


MyBookings.ValidatePayment = function (bUseOffsitePayment) {

    //remove error class
    for (var aErrors = int.f.GetElementsByClassName('*', 'error', int.f.GetObject('divPaymentDetails')), i = 0; i < aErrors.length; i++) {
        int.f.RemoveClass(aErrors[i], 'error');
    }

    if (!bUseOffsitePayment) {

        //get values
        var sCardNumber = int.f.GetValue('txtCCCardNumber');
        var sSecurityCode = int.f.GetIntValue('txtCCSecurityCode');
        var sExpiryMonth = int.dd.GetValue('ddlCCExpireMonth');
        var sExpiryYear = int.dd.GetValue('ddlCCExpireYear');
        var sPaymentAmount = int.f.GetValue('txtAmount');
        var sOutstandingAmount = int.f.GetValue('hidOutstandingAmount');

        var oCardType = int.f.GetObject('ddlCCCardTypeID');
        var oExpireMonth = int.f.GetObject('ddlCCExpireMonth');
        var oExpireYear = int.f.GetObject('ddlCCExpireYear');

        if (int.f.HasClass(oCardType.parentNode, 'custom-select')) {
            oCardType = oCardType.parentNode;
        };
        if (int.f.HasClass(oExpireMonth.parentNode, 'custom-select')) {
            oExpireMonth = oExpireMonth.parentNode;
        };
        if (int.f.HasClass(oExpireYear.parentNode, 'custom-select')) {
            oExpireYear = oExpireYear.parentNode;
        };

        if (int.cb.Checked('radNewBillingAddress')) {
            int.f.SetClassIf('txtAddress1', 'error', int.f.GetValue('txtAddress1') === '');
            int.f.SetClassIf('txtTownCity', 'error', int.f.GetValue('txtTownCity') === '');
            int.f.SetClassIf('txtPostcode', 'error', int.f.GetValue('txtPostcode') === '');
        };

        //set error class if not valid
        int.f.SetClassIf(oCardType, 'error', int.dd.GetValue('ddlCCCardTypeID') == 0);
        int.f.SetClassIf(oExpireMonth, 'error', sExpiryMonth == '' || !int.v.IsNumeric(sExpiryMonth));
        int.f.SetClassIf(oExpireYear, 'error', sExpiryYear == '' || !int.v.IsNumeric(sExpiryYear));

        int.f.SetClassIf('txtCCCardNumber', 'error', !int.v.IsCardNumber(sCardNumber) || sCardNumber == '');
        int.f.SetClassIf('txtCCCardHoldersName', 'error', int.f.GetValue('txtCCCardHoldersName') == '');
        int.f.SetClassIf('txtCCSecurityCode', 'error', int.f.GetValue('txtCCSecurityCode') == '');

        var bAmountInValid = false;

        if ((int.n.SafeNumeric(sPaymentAmount) <= 0) || int.n.SafeNumeric(sPaymentAmount) > int.n.SafeNumeric(sOutstandingAmount)) {
            bAmountInValid = true;
        }

        int.f.SetClassIf('txtAmount', 'error', bAmountInValid);

    }

    // success?
    var iErrors = int.f.GetElementsByClassName('', 'error', 'divPaymentDetails').length;
    if (iErrors > 0) {
        return false;
    }
    else {
        return true;
    }
};

MyBookings.ChangeBillingAddress = function () {
    var aRadios = int.f.GetObjectsByIDPrefix('rad', 'input', 'divBillingAddress');

    for (var i = 0; i < aRadios.length; i++) {
        int.f.SetClassIf(aRadios[i].parentNode, 'selected', int.cb.Checked(aRadios[i]));
    }
}

MyBookings.LogOut = function () {
    var sLogoutRedirectURL = '/booking-login';
    if (int.f.GetValue('hidLogoutRedirectURL') != '') {
        sLogoutRedirectURL = int.f.GetValue('hidLogoutRedirectURL');
    }
    int.ff.Call('=iVectorWidgets.MyBookings.LogOut',
        function () {
            window.location = sLogoutRedirectURL;
        });
};


MyBookings.CreditCardSelect = function () {
    var iCreditCardTypeID = int.dd.GetValue('ddlCCCardTypeID');

    int.ff.Call('Widgets.MyBookings.CreditCardSelect', function (sJSON) { MyBookings.CreditCardSelectDone(sJSON); }, iCreditCardTypeID);
};

MyBookings.CreditCardSelectDone = function (sJSON) {
    var result = JSON.parse(sJSON);

    if (result) {
        int.f.SetValue('hidTotalAmount', result['TotalPriceUnformated']);
        var nSurcharge = result['Surcharge'];
        int.f.SetValue('hidSurchargePercentage', nSurcharge);

        MyBookings.SetSurchargeAmount();
    }
}

MyBookings.ShowApisPopup = function () {
    web.JQueryModalPopup.Show('divAPISWrapper', true, 'container');
    return false;
};


MyBookings.SetSurchargeAmount = function () {
    var nSurcharge = int.f.GetValue('hidSurchargePercentage');
    var nSurchargeAmount = int.f.GetValue('txtAmount') * (nSurcharge / 100);
    int.f.SetValue('hidSurcharge', nSurchargeAmount);

    if (nSurcharge > 0) {
        var sCurrencySymbol = int.f.GetValue('hidCurrencySymbol');
        var sCurrencyPosition = int.f.GetValue('hidCurrencySymbolPosition');
        int.f.SetHTML('spnCreditCardSurcharge', 'A surcharge of ' + int.n.FormatMoney(nSurchargeAmount, sCurrencySymbol, sCurrencyPosition) + ' will apply.');
    }

    int.f.ShowIf('spnCreditCardSurcharge', nSurcharge > 0);
}


MyBookings.Setup = function () {

    //if payment successful, show message
    if (int.f.GetObject('hidPaymentSuccessful')) {
        web.InfoBox.Show(int.f.GetValue('hidPaymentSuccessText'), 'success');
    }

    //if 3ds payment successful, show message
    if (int.f.getParameterByName('paymentsuccessful') === 'true') {
        web.InfoBox.Show('Your payment has been successful', 'success');
    }

    //if 3ds Payment failed
    if ((int.f.getParameterByName('paymentfailed') === 'true') || (int.f.getParameterByName('authfailed') === 'true')) {
        web.InfoBox.Show('Your payment has failed', 'warning');
    }

    //Attach log out
    var aLogOut = document.getElementById('aLogOut');
    if (aLogOut != null) {
        int.f.AttachEvent('aLogOut', 'click', function () {
            MyBookings.LogOut();
        });
    };

    //Attach show apis 
    var btnShowAPIS = document.getElementById('btnShowApis');
    if (btnShowAPIS != null) {
        int.f.AttachEvent('btnShowApis', 'click', function () {
            MyBookings.ShowApisPopup();
        });
    };

    //Attach show pay balance 
    var btnPayBalanceShow = document.getElementById('btnPayBalanceShow');
    if (btnPayBalanceShow != null) {
        int.f.AttachEvent('btnPayBalanceShow', 'click', function () {
            MyBookings.GetPaymentPopup();
        });
    };

    //Attach send documentation event
    int.f.AttachEvent('aSendDocumentation', 'click', function () {
        MyBookings.SendDocumentation();
    });
};


$(document).ready(function () {
    MyBookings.Setup();
});
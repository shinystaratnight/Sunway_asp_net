var PaymentAuthHandler = PaymentAuthHandler || {};

PaymentAuthHandler.Init = function () {
    var redirectURL = int.f.GetValue('hidRedirectUL');
    if (redirectURL == '') {
        redirectURL = '/';
    }
    window.location.href = redirectURL;
};


$(document).ready(function () {
    PaymentAuthHandler.Init();
});
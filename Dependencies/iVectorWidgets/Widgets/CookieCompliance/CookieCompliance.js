var CookieCompliance = CookieCompliance || {}

CookieCompliance.Setup = function (bRequiresAccepting, bHideCookieMessage) {
    if (bHideCookieMessage = 'true') {
        if (int.c.Get('__Cookie_Law_Acceptance')) {
            CookieCompliance.Close();
        }
    }

	//Only set the cookie by default if the widget does not require the user to accept to use cookies.
	if (bRequiresAccepting != 'true') {
		CookieCompliance.SetCookie();
	}

	CookieCompliance.AttachEvents();

}

CookieCompliance.AttachEvents = function () {

	int.f.AttachEvent('aCookieCompliance_Accept','click',CookieCompliance.AcceptCookies);

}

CookieCompliance.AcceptCookies = function () {

	CookieCompliance.Close();

	CookieCompliance.SetCookie()
}

CookieCompliance.Close = function () {

	var oDiv = int.f.GetObject('divCookieCompliance');

	oDiv.style.minHeight = 0;
	oDiv.style.maxHeight = 0;
	oDiv.style.padding = 0;

}

CookieCompliance.SetCookie = function () {

	//save cookie
	int.c.Set('__Cookie_Law_Acceptance', true, 1000);

}

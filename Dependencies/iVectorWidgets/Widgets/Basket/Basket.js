var Basket = Basket || {}

Basket.Setup = function () {
    var aSections = int.f.GetElementsByClassName('h3', 'section', 'divBasket');

    for (var i = 0; i < aSections.length; i++) {

        int.f.AttachEvent(
            aSections[i],
            'click',
            function () {
                Basket.ToggleSection(this);
            }
        );
    }
}

Basket.UpdateBasketHTML = function () {

    //Check whether to use an overrided function
    var sFunction = int.f.GetValue('hidBasketUpdateFunction');

    if (sFunction == undefined || sFunction == '') {
        int.ff.Call('=iVectorWidgets.Basket.UpdateBasketHTML', function (sHTML, iTotalComponents) { Basket.UpdateBasketHTMLDone(sHTML, iTotalComponents); });
    }
    else {
        int.ff.Call(sFunction, function (sHTML, iTotalComponents) { Basket.UpdateBasketHTMLDone(sHTML, iTotalComponents); });
    }
}

Basket.UpdateBasketHTMLDone = function (sHTML) {

    //if basket does not exist, create it and add the new html into it, then add to sidebar
    if (int.f.GetObject('divBasket') == undefined) {

        //create new basket elements
        var oDivBasket = document.createElement('div');
        oDivBasket.id = 'divBasket';
        oDivBasket.className = 'sidebarBox primary clear';
        oDivBasket.innerHTML = sHTML;

        //add to sidebar
        var oSideBar = int.f.GetObject('divSidebar');
        oSideBar.appendChild(oDivBasket);

        }
    else {
        int.f.SetHTML('divBasket', sHTML, true);
    }
}

Basket.RemoveRoom = function (BookingToken) {
    int.ff.Call('=iVectorWidgets.Basket.RemoveRoom', Basket.UpdateBasketHTML, BookingToken)
}

Basket.ToggleSection = function (oHeader) {

    var sSectionID = int.s.Substring(oHeader.id, 1);

    var sIDWithPrefix = 'div' + sSectionID + '_Content'

    int.f.Toggle(sIDWithPrefix);

    if (int.f.HasClass(oHeader, 'hidden')) {
        int.f.RemoveClass(oHeader, 'hidden');
    }
    else {
        int.f.AddClass(oHeader, 'hidden');
    }
}

web.PubSub.subscribe('PromoCode.ChangeSuccess', function () {
    Basket.UpdateBasketHTML();
});

web.PubSub.subscribe('Basket Changed', function () {
    Basket.UpdateBasketHTML();
});
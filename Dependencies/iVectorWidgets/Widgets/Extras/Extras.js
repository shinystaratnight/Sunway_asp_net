var Extras = Extras || {}

Extras.UpdateCompleteFunction = function () { Basket.UpdateBasketHTML(); };


//#region Add To Basket

Extras.AddToBasket = function (sBookingToken, sIdentifier, bClearBasket) {
    var ClearBasket = !!bClearBasket;
    int.ff.SyncCall('=iVectorWidgets.Extras.AddExtraToBasket', Extras.AddToBasketComplete, sBookingToken, sIdentifier, ClearBasket);
}

Extras.AddToBasketComplete = function (sJSON) {
	var oReturn = JSON.parse(sJSON);
	Extras.UpdateCompleteFunction();
    int.f.SetHTML('div' + int.s.Replace(oReturn.Title, ' ', '') + 'Results', oReturn.HTML);
}

//#endregion


//#region Remove From Basket

//By Type
Extras.RemoveExtraByType = function (sIdentifier) {
    int.ff.Call('=iVectorWidgets.Extras.RemoveExtraByType', Extras.RemoveExtraByTypeComplete, sIdentifier);
}

//By Index
Extras.RemoveExtraByIndex = function (iIndex, sIdentifier) {
    int.ff.Call('=iVectorWidgets.Extras.RemoveExtraByIndex', Extras.RemoveExtraByTypeComplete, iIndex, sIdentifier);
}

//on complete
Extras.RemoveExtraByTypeComplete = function (sJSON) {
    var oReturn = JSON.parse(sJSON);
    Extras.UpdateCompleteFunction();
    int.f.SetHTML('div' + int.s.Replace(oReturn.Title, ' ', '') + 'Results', oReturn.HTML);
}

//#endregion

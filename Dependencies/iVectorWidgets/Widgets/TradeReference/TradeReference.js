var TradeReference = TradeReference || {};

TradeReference.CompleteFunction = null;


TradeReference.Setup = function () {
    web.Placeholder.AttachEvents('txtTradeReference', int.f.GetValue('hidTradeReferencePlaceholder'));
};


TradeReference.SetBasketTradeReference = function (oCompleteFunction) {

    me.CompleteFunction = oCompleteFunction;

    var oTradeReference = int.f.GetObject('txtTradeReference');
    if (oTradeReference != undefined){
        int.f.RemoveClass(oTradeReference, 'error');

        if (oTradeReference.value != '' && oTradeReference.value != int.f.GetValue('hidTradeReferencePlaceholder')) {
            int.ff.Call('=iVectorWidgets.TradeReference.SetBasketTradeReference', TradeReference.SetBasketTradeReferenceComplete, oTradeReference.value);
        }
        else {
            int.f.AddClass(oTradeReference, 'error');
            TradeReference.SetBasketTradeReferenceComplete();
        }
    } else {
        TradeReference.SetBasketTradeReferenceComplete();
    }


};


TradeReference.SetBasketTradeReferenceComplete = function () {
    if (me.CompleteFunction != null && me.CompleteFunction != undefined) {
        me.CompleteFunction();
    }
};
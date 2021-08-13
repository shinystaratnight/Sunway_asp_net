var PromoCode = PromoCode || {}

PromoCode.Setup = function () {

	var oInjectTarget = int.f.GetObject(int.f.GetValue('hidPromoCode_InjectTarget'));
	if (oInjectTarget) {
		var oPromoCode = int.f.GetObject('divPromoCode');
		oInjectTarget.parentNode.insertBefore(oPromoCode, oInjectTarget);
		int.f.Show(oPromoCode);
	}

}

PromoCode.Inject = function (sContainer) {

	int.ff.Call('=iVectorWidgets.PromoCode.InjectHTML', PromoCode.InjectHTMLComplete, sContainer);

}

PromoCode.InjectHTMLComplete = function (oJSON) {

	var oReturn = JSON.parse(oJSON);

	var oPromoCode = document.createElement('div');
	oPromoCode.setAttribute('id', 'divPromoCodeContainer');
	oPromoCode.innerHTML = oReturn.HTML;

	document.body.appendChild(oPromoCode);

	var oInjectTarget = int.f.GetObject(oReturn.InjectID);
	if (oInjectTarget) {
		var oPromoCode = int.f.GetObject('divPromoCode');
		oInjectTarget.parentNode.insertBefore(oPromoCode, oInjectTarget);
		int.f.Show(oPromoCode);
	}

};

PromoCode.ApplyPromoCode = function () {

	web.InfoBox.Close();

	var sCode = int.f.GetValue('txtPromoCode');

	int.ff.Call(
		'=iVectorWidgets.PromoCode.ApplyPromoCode',
		function (sReturn) {
			PromoCode.ApplyPromoCodeComplete(sReturn);
		},
		sCode
	);

};

PromoCode.ApplyPromoCodeComplete = function (oJSON) {

	var symbol = int.f.GetValue('hidPromoCode_CurrencySymbol');
	var position = int.f.GetValue('hidPromoCode_CurrencyPosition');

	var oReturn = JSON.parse(oJSON);

	if (oReturn.Warning == 0) {

		int.f.SetHTML('spnPromoCodeDiscount', int.n.FormatMoney(oReturn.Discount, symbol, position));
		int.f.SetValue('txtPromoCode', '');
		int.f.Hide('txtPromoCode');
		int.f.Hide('pPromoCodeInfo');
		int.f.Hide('aUpdatePromoCode');
		int.f.Show('pAppliedCode');
		int.f.Show('aRemovePromoCode');

		// reload basket
		if (typeof PaymentBasket !== 'undefined') {
			PaymentBasket.UpdateBasketHTML();
		}

		web.PubSub.publish('PromoCode.ChangeSuccess');

	}
	else {
		int.f.SetValue('txtPromoCode', oReturn.Warning);
	}
};

PromoCode.RemovePromoCode = function () {
	int.ff.Call(
		'=iVectorWidgets.PromoCode.RemovePromoCode',
		function (sOK) {
			PromoCode.RemovePromoCodeComplete(sOK);
		}
	);
};

PromoCode.RemovePromoCodeComplete = function (sJSON) {

	var oReturn = JSON.parse(sJSON);

	int.f.Show('pPromoCodeInfo');
	int.f.Show('aUpdatePromoCode');
	int.f.Show('txtPromoCode');
	int.f.Hide('pAppliedCode');
	int.f.Hide('aRemovePromoCode');

	if (oReturn.Success != 'False') {

		web.PubSub.publish('PromoCode.ChangeSuccess');

		if (typeof PaymentBasket !== "undefined") {
			// reload basket
			PaymentBasket.UpdateBasketHTML();
		}

	};
};
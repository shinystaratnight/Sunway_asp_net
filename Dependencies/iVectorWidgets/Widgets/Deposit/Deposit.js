var Deposit = Deposit || {}

	Deposit.Setup = function (sContainer) {
		var oInjectDiv = int.f.GetObject(sContainer);
		if (oInjectDiv != null) {
			oInjectDiv.appendChild(int.f.GetObject('divDeposit'));
			//			int.f.Show('divMarkupTool');
		}
	}

	Deposit.PayDeposit = function (bUseDeposit) {
		int.ff.Call('=iVectorWidgets.Deposit.UpdatePayment', function () { Deposit.PayDepositComplete(bUseDeposit); }, bUseDeposit);
	}


	Deposit.PayDepositComplete = function (bUseDeposit) {
		if (int.f.GetObject('hAmountDueToday')) {
			int.f.ShowIf('hAmountDueToday', bUseDeposit);
		}

		if (int.f.GetValue('hidUpdateTotalPriceDisplay') == 'True') {

			if (bUseDeposit) {
				int.f.SetValue('txtTotalPrice', int.f.GetValue('hidDepositAmount'));
			}
			else {
				int.f.SetValue('txtTotalPrice', int.f.GetValue('hidTotalAmount'));
			}

		}
	}

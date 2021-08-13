
var TradeLogin = TradeLogin || {};

var AgentReference = '';
var AutoCompleteLogin = false;

	//#region Setup

	TradeLogin.Setup = function () {
		TradeLogin.SetupPlaceholders();
	};

	TradeLogin.SetupPlaceholders = function () {

		//get placeholders and loop through them
		var aPlaceholders = int.f.GetValue('hidTradeLogin_Placeholders').split('#');
		for (i = 0; i < aPlaceholders.length; i++) {
			var sID = aPlaceholders[i].split('|')[0];
			var sText = aPlaceholders[i].split('|')[1];
			web.Placeholder.AttachEvents(sID, sText);
		}

	}

	//#endregion

	//#region Login mode

	TradeLogin.ToggleLoginMode = function () {
		//toggle the login type - this will need to be changed when there are more than 2 login types
		int.f.Toggle('divTradeContactLogin');
		int.f.Toggle('pLoginAsTradeContact');
		int.f.Toggle('divAgentLogin');
		int.f.Toggle('pLoginAsTradeMember');
	}

	//#endregion

	//#region Login

	TradeLogin.Validate = function () {

		//validate fields 
		if (int.f.Visible('divAgentLogin')) {
			int.f.SetClassIf('txtAgentReference', 'error', int.f.GetObject('txtAgentReference') && (int.f.GetValue('txtAgentReference') == '' || !web.Placeholder.NotPlaceholderText('txtAgentReference')));
			int.f.SetClassIf('txtWebsitePassword', 'error', int.f.GetObject('txtWebsitePassword') && (int.f.GetValue('txtWebsitePassword') == '' || !web.Placeholder.NotPlaceholderText('txtWebsitePassword')));
		};


		if (int.f.Visible('divTradeContactLogin')) {
			int.f.SetClassIf('txtUsername', 'error', int.f.GetObject('txtUsername') && (int.f.GetValue('txtUsername') == '' || !web.Placeholder.NotPlaceholderText('txtUsername')));
			int.f.SetClassIf('txtPassword', 'error', int.f.GetObject('txtPassword') && (int.f.GetValue('txtPassword') == '' || !web.Placeholder.NotPlaceholderText('txtPassword')));
			int.f.SetClassIf('txtAgencyUserName', 'error', int.f.GetObject('txtAgencyUserName') && (int.f.GetValue('txtAgencyUserName') == '' || !web.Placeholder.NotPlaceholderText('txtAgencyUserName')));

		};

		//check for errors, login if none
		var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divTradeLogin');
		if (aErrorControls.length == 0) {
			TradeLogin.Login(int.f.GetValue('hidLoginType'));
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidIncorrectFields'), 'warning');
		}

	}

	TradeLogin.Login = function (sLoginType) {

		//create our request object
		var oLoginDetails = {
			LoginType: sLoginType,
			AgentReference: '',
			EmailAddress: '',
			WebsitePassword: '',
			UserName: '',
			Password: '',
			RememberMe: false
		};

		//get login method
		var sLoginMethod = int.f.GetValue('hidLoginMethod');

		//populate our request object based on the login type and method
		if (sLoginType == 'TradeContact') {
			oLoginDetails.AgentReference = int.f.GetValue('txtAgencyUserName');
			oLoginDetails.UserName = int.f.GetValue('txtUsername');
			oLoginDetails.Password = int.f.GetValue('txtPassword');
		}
		else {
			if (sLoginMethod == 'EmailAndPassword') {
				oLoginDetails.EmailAddress = int.f.GetValue('txtEmailAddress');
				oLoginDetails.WebsitePassword = int.f.GetValue('txtWebsitePassword');
			}
			else {
				oLoginDetails.AgentReference = int.f.GetValue('txtAgentReference');
				oLoginDetails.WebsitePassword = int.f.GetValue('txtWebsitePassword');
			}
		}

		//TODO - write remember me logic better - at the moment we are assuming if not rendered we default to rememberme true
		//it gets set to false if widget setting mandates it
		oLoginDetails.RememberMe = !int.f.GetObject('chkRememberMe') || int.cb.Checked('chkRememberMe');

		//attempt login
		int.ff.Call('=iVectorWidgets.TradeLogin.Login', TradeLogin.LoginComplete, JSON.stringify(oLoginDetails));

	}

	TradeLogin.LoginComplete = function (sJSON) {

		var oTradeLoginReturn = JSON.parse(sJSON);

		if (oTradeLoginReturn['OK'] == true) {
			web.Window.Redirect(int.f.GetValue('hidLoginRedirectURL'));
		}
		else if (oTradeLoginReturn['Warnings'] == 'Duplicate Login') {
			web.InfoBox.Show(int.f.GetValue('hidLoginFailed_Duplicates'), 'warning');
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidLoginFailed'), 'warning');
		};

	};
	//#endregion


	//#region Event helpers

	TradeLogin.OnKeyPress = function (KeyEvent) {
		KeyEvent.which = KeyEvent.which || KeyEvent.keyCode;
		if (KeyEvent.which == 13) {
			TradeLogin.Validate('TradeOnly');
			return false;
		}
	}

	//#endregion

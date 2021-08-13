
var SearchTool = new function () {

	var me = this;

	/*

	1. Setup
	2. Bind
	3. Show
	4. Search
	5. Unbind

	x. Support
	x.1 PopulateDropdownRanges


	*/




	//1. Setup
	this.Setup = function () {

		me.Support.PopulateDropdownRanges();
		me.Bind();
		me.Show();


	}



	//2. Bind
	this.Bind = function () {

		var aPairs = me.Support.GetCookie();
		int.f.SetValue('hidSearchMode', aPairs['SearchMode']);

		int.f.SetClassIf('li_SearchMode_FlightPlusHotel', 'selected', aPairs['SearchMode'] == 'FlightPlusHotel');
		int.f.SetClassIf('li_SearchMode_Hotel', 'selected', aPairs['SearchMode'] == 'Hotel');

		if (int.f.GetObject('li_SearchMode_Flight')) {
			int.f.SetClassIf('li_SearchMode_Flight', 'selected', aPairs['SearchMode'] == 'Flight');
		}

		if (aPairs['DepartingFromID'] > 0) {
			int.dd.SetValue('ddlDepartingFromID', aPairs['DepartingFromID']);
		}

		if (aPairs['ArrivingAtID'] > 0) {
			int.dd.SetValue('ddlArrivingAtID', aPairs['ArrivingAtID']);
		}

		int.f.SetValue('txtDepartureDate', aPairs['DepartureDate']);
		int.dd.SetValue('ddlDuration', aPairs['Duration']);
		int.dd.SetValue('ddlMealBasisID', aPairs['MealBasisID']);
		int.dd.SetValue('ddlRooms', aPairs['Rooms']);


		//rooms
		for (var i = 0; i <= 3; i++) {
			int.dd.SetValue('ddlAdults_' + i, aPairs['Adults_' + i]);

		}

		//date picker
		web.DatePicker.Setup('#txtDepartureDate', new Date, aPairs['DepartureDate']);

	}


	//3. Show
	this.Show = function () {

		//search modes
		var sSearchMode = int.f.GetValue('hidSearchMode');

		int.f.ShowIf('fldDeparting', sSearchMode == 'FlightPlusHotel' || sSearchMode == 'Flight');



		//rooms
		var iRooms = int.dd.GetValue('ddlRooms');

		for (var i = 1; i <= 3; i++) {

			int.f.ShowIf('trGuests_' + i, i <= iRooms);

			var iChildren = int.n.SafeInt(int.dd.GetValue('ddlChildren_' + i));

			int.f.ShowIf('trAges_' + i, iChildren > 0 && i <= iRooms);

			for (var j = 1; j <= 4; j++) {
				int.f.ShowIf('ddlChildAge_' + i + '_' + j, j <= iChildren)
			}
		}

	}




	//4. Search
	this.Search = function () {


	}


	//x. Support
	this.Support = new function () {


		//x.1 populate dropdown ranges
		//picks up range_[from]_[to] in classname and add options accordingly
		this.PopulateDropdownRanges = function () {
			var aRangeDropdowns = int.f.GetElementsByClassName('select', 'range_', 'divSearch');

			for (var i = 0; i < aRangeDropdowns.length; i++) {
				var oDropdown = aRangeDropdowns[i];

				int.dd.Clear(oDropdown);

				var aToFrom = /range_(\d+)_(\d+)/.exec(oDropdown.className);

				for (var j = int.n.SafeInt(aToFrom[1]); j <= int.n.SafeInt(aToFrom[2]); j++) {
					int.dd.AddOption(oDropdown, j, j)
				}
			}

		}


		//getcookie
		//get cookie, split up and build associative array
		this.GetCookie = function () {
			var aList = new Array();
			var aBits = int.c.Get('__search').split('&');
			for (var i = 0; i < aBits.length; i++) {
				var aKeyValue = aBits[i].split('=');
				if (aKeyValue.length == 2) { aList[aKeyValue[0]] = aKeyValue[1] }
			}
			return aList;
		}


		//setcookie



		//getvalue
		this.GetValue = function (KeyValuePair, Key) {
			var regex = new RegExp(Key + '=([\\w\\d]+)');
			var aMatches = regex.exec(KeyValuePair);
			alert(aMatches);
			if (aMatches != null) { return aMatches[1]; }
		}


		//set search mode
		this.SetSearchMode = function (sSearchMode) {

			int.f.SetValue('hidSearchMode', sSearchMode);

			int.f.SetClassIf('li_SearchMode_FlightPlusHotel', 'selected', sSearchMode == 'FlightPlusHotel');
			int.f.SetClassIf('li_SearchMode_Hotel', 'selected', sSearchMode == 'Hotel');

			SearchTool.Show();
		}

	}

}

var HotelDetails = new function () {

	var me = this;

	this.Setup = function () {

//		var oSideBar = int.f.GetObject('divSidebar');
//		var oMapHolder = document.createElement('div')
//		oMapHolder.setAttribute('id', 'divPropertyMap');
//		oSideBar.insertBefore(oMapHolder, int.f.GetObject('divSimilarHotels').nextSibling)

//		Map.Setup('divPropertyMap');
//		int.ff.Call('iVectorWidgets.HotelDetails.SetupMap', function (sJSON) {Map.GetMapComplete();});

	}

	this.SelectRoom = function (iPropertyRoomNumber, sRoomBookingToken) {

		FormHandler.CloseInfo();

		var sRoomBookingTokens = ''
		f.SetValue('hidSelectedRoom_' + iPropertyRoomNumber, sRoomBookingToken);

		/* loop through rooms */
		var bAllRoomsSelected = true;
		var aRoomTokens = f.GetObjectsByIDPrefix('hidSelectedRoom');
		for (var i = 1; i <= aRoomTokens.length; i++) {
			if (f.GetValue('hidSelectedRoom_' + i) == '') {
				bAllRoomsSelected = false;
			}
			else {
				sRoomBookingTokens += f.GetValue('hidSelectedRoom_' + i) + ',';
			}
		}
		sRoomBookingTokens = s.Chop(sRoomBookingTokens);

		if (bAllRoomsSelected) {

			ff.Call('Widgets.PropertyDetails.AddToBasket',
		 function (bOK) { PropertyDetails.SelectRoomComplete(bOK) },
		 f.GetValue('hidPropertyBookingToken'), sRoomBookingTokens);

		}

	}

	this.SelectRoomComplete = function (bOK) {

		if (bOK == false) {

			FormHandler.ShowWarning('Sorry, this room is no longer available.');

		} else {

			PropertyDetails.PrebookRoom();
		}
	}

	this.PrebookRoom = function () {

		ff.Call('Widgets.PropertyDetails.Prebook',
		 function (sJSON) { PropertyDetails.PrebookRoomComplete(sJSON) });
	}

	this.PrebookRoomComplete = function (sJSON) {

		var oPrebookReturn = eval('(' + sJSON + ')');

		if (oPrebookReturn['Success'] == true) {

			if (oPrebookReturn['RedirectURL'] != '') {
				window.location = oPrebookReturn['RedirectURL'];
			};

		} else {

			FormHandler.ShowWarning('We were unable to confirm availability for all components. Please select a different option.')

		};

	}

	this.ImageHover = function (element) {

		var sNewSRC = element.src;
		int.f.GetObject('imgMainImage').setAttribute('src', sNewSRC);

		var aImages = int.f.GetObject('divOtherImages').getElementsByTagName('img');

		for (var i = 0; i < aImages.length; i++) {
			int.f.SetClassIf(aImages[i], 'selected', aImages[i] == element);
		};

	};



}
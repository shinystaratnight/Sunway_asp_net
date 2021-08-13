var HotelPopup = HotelPopup || {};

web.PubSub.subscribe('HotelPopup.ShowPopup', function () {
    var args = [].slice.call(arguments);

    if (typeof args[1] === 'object') {
        var sFunction = args[1].Function ? args[1].Function : 'HotelPopup.ShowPopup';
        var fn = window[sFunction.split('.')[0]][sFunction.split('.')[1]];

        if (typeof fn === 'function') {
            fn(args[1].Id);
        }
    }
    else {
        HotelPopup.ShowPopup(args[1]);
    }
    
});

HotelPopup.ImageGallery;

HotelPopup.ShowPopup = function (iPropertyReferenceID) {

    int.ff.Call('=iVectorWidgets.HotelPopup.GetPopupContent',
        function (sHTML) {
            HotelPopup.ShowPopupDone(sHTML);
        },
        iPropertyReferenceID, 'Content'
    );
};

HotelPopup.ShowPopupDone = function (sHTML) {
    //show popup
    web.ModalPopup.Show(sHTML, true, 'modalpopup hotel-popup');
    web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailName'), int.f.GetValue('hidNamePlaceholder'));
    web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailRecipientAddress'), int.f.GetValue('hidRecipientPlaceholder'));
    web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailMessage'), int.f.GetValue('hidMessagePlaceholder'));
	int.f.SetValue('txtHotelPopup_EmailMessage', '');

    //attach the email event
    if (int.f.GetObject('hidPropertyReferenceID')) {
        var iPropertyReferenceID = int.f.GetValue('hidPropertyReferenceID');
        var oSend = int.f.GetObject('btnSendEmail');

        int.f.AttachEvent(oSend, 'click', function () {
			HotelPopup.EmailDetails(iPropertyReferenceID);
		});
    }

};

HotelPopup.ShowGroupPopupDone = function (sHTML) {
	//show popup
	web.ModalPopup.Show(sHTML, true, 'modalpopup group-popup');
	web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailName'), int.f.GetValue('hidNamePlaceholder'));
	web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailRecipientAddress'), int.f.GetValue('hidRecipientPlaceholder'));
	web.Placeholder.AttachEvents(int.f.GetObject('txtHotelPopup_EmailMessage'), int.f.GetValue('hidMessagePlaceholder'));
	int.f.SetValue('txtHotelPopup_EmailMessage', '');
	HotelPopup.DisableScroll();

	var oClose = int.f.GetElementsByClassName('a', 'close', int.f.GetObject('divModalPopup'))[0];
	int.f.AttachEvent(oClose, 'click', function () {
        HotelPopup.EnableScroll();

    });
    int.f.AttachEvent(document.body, 'keyup', function (evt) {
        evt = evt || window.event;
        if (evt.keyCode == 27) HotelPopup.EnableScroll(); web.ModalPopup.Hide();
    });

	//attach the email event
	if (int.f.GetObject('hidPropertyReferenceID')) {
		var iPropertyReferenceID = int.f.GetValue('hidPropertyReferenceID');
		var oSend = int.f.GetObject('btnSendEmail');

		int.f.AttachEvent(oSend, 'click', function () {
			HotelPopup.EmailDetails(iPropertyReferenceID);
		});
    }

    web.PubSub.publish('HotelPopup_ShowGroupPopupDone');

};

HotelPopup.ClosePopup = function () {
	//hide popup
	web.ModalPopup.Hide();
};

HotelPopup.DisableScroll = function () {
	var oBody = int.f.GetObject('frm');
	int.f.AddClass(oBody, 'scroll-lock');
}

HotelPopup.EnableScroll = function () {
	var oBody = int.f.GetObject('frm');
	int.f.RemoveClass(oBody, 'scroll-lock');
}

//#region Email 

HotelPopup.EmailDetails = function (iPropertyReferenceID) {

	int.f.Hide('pHotelPopup_EmailDone');

	int.f.SetClassIf('txtHotelPopup_EmailName', 'error', int.f.GetValue('txtHotelPopup_EmailName') == '' || int.f.GetValue('txtHotelPopup_EmailName') == int.f.GetValue('hidNamePlaceholder'));
	int.f.SetClassIf('txtHotelPopup_EmailRecipientAddress', 'error', int.f.GetValue('txtHotelPopup_EmailRecipientAddress') == '' || !int.v.IsEmail(int.f.GetValue('txtHotelPopup_EmailRecipientAddress')));
	var aErrorControls = int.f.GetElementsByClassName('*', 'error', 'divHotelPopup_EmailForm');

	//show error message if required
	if (aErrorControls.length > 0) {
		int.f.Show('pHotelPopup_EmailError');
	}
	else {
		int.f.Hide('pHotelPopup_EmailError');

		//call ff
		int.ff.Call('=iVectorWidgets.HotelPopup.EmailProperty',
            function (sJSON) {
                HotelPopup.EmailDetailsDone(sJSON);
            },
            int.f.GetValue('txtHotelPopup_EmailName'), int.f.GetValue('txtHotelPopup_EmailRecipientAddress'), 
			int.f.GetValue('txtHotelPopup_EmailMessage'), iPropertyReferenceID/*, int.f.GetValue('txtHotelPopup_EmailYourAddress')*/);
	};

};


HotelPopup.EmailDetailsDone = function (sJSON) {
	int.f.Show('pHotelPopup_EmailDone');
};

//#endregion

//#region Tabbed popup

HotelPopup.SelectTab = function (sTab) {
    //creates an array and goes through it removing the 'selected' class off all the li elements
    var aPropertyTabs = int.f.GetObjectsByIDPrefix('liProperty_', 'li', 'ulTabsList');

    for (var i = 0; i < aPropertyTabs.length; i++) {
        int.f.RemoveClass(aPropertyTabs[i], 'selected');
    }

    //creates an object and adds the selected class to the new selected tab
    var oNewTab = int.f.GetObject('liProperty_' + sTab);
    int.f.AddClass(oNewTab, 'selected');

    //creates an array and goes through it adding the hidden class to all content in the tabs
    var aPropertyContent = int.f.GetObjectsByIDPrefix('divProperty_', 'div', 'divHotelPopupTabs');
    for (var i = 0; i < aPropertyContent.length; i++) {
        int.f.AddClass(aPropertyContent[i], 'hidden');
    }

    //creates an object and removes the hidden class to the content that needs to be shown
    var oNewContent = int.f.GetObject('divProperty_' + sTab);
    int.f.RemoveClass(oNewContent, 'hidden');
};

//#endregion

HotelPopup.ShowExtraRooms = function (iPropertyReferenceID, iRoom, sIdentifier) {

    sIdentifier = ((sIdentifier == undefined || sIdentifier == null) ? '' : sIdentifier + '_');

    var aExtraRooms = int.f.GetElementsByClassName('tr', 'extra', 'tblPopupRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
    for (i = 0; i < aExtraRooms.length; i++) {
        int.f.Show(aExtraRooms[i]);
    }

    int.f.Show('aPopupHideExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
    int.f.Hide('aPopupShowExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);

};

HotelPopup.HideExtraRooms = function (iPropertyReferenceID, iRoom, sIdentifier) {

    sIdentifier = ((sIdentifier == undefined || sIdentifier == null) ? '' : sIdentifier + '_');

    var aExtraRooms = int.f.GetElementsByClassName('tr', 'extra', 'tblPopupRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
    for (i = 0; i < aExtraRooms.length; i++) {
        int.f.Hide(aExtraRooms[i]);
    }

    int.f.Hide('aPopupHideExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);
    int.f.Show('aPopupShowExtraRooms_' + sIdentifier + iPropertyReferenceID + '_' + iRoom);

};

//#region image gallery popup

HotelPopup.ImageGallerySetup = function () {

    //get the objects we need to attact events to
    var oImageNavLeft = int.f.GetObject('aImageNavLeft');
    var oImageNavRight = int.f.GetObject('aImageNavRight');
    var oThumbNavLeft = int.f.GetObject('aThumbNavLeft');
    var oThumbNavRight = int.f.GetObject('aThumbNavRight');
    var aThumbNails = int.f.GetElementsByClassName('img', 'thumbnail', 'divImagePopup_ThumbnailHolder');

    //attach the events
    int.f.AttachEvent(
		oImageNavLeft,
		'click',
		function () {
		    HotelPopup.ImageGallery.ImageNav('left')
		}
	);

    int.f.AttachEvent(
		oImageNavRight,
		'click',
		function () {
		    HotelPopup.ImageGallery.ImageNav('right')
		}
	);

    int.f.AttachEvent(
		oThumbNavLeft,
		'click',
		function () {
		    HotelPopup.ImageGallery.ThumbnailNav('left')
		}
	);

    int.f.AttachEvent(
		oThumbNavRight,
		'click',
		function () {
		    HotelPopup.ImageGallery.ThumbnailNav('right')
		}
	);

    for (i = 0; i < aThumbNails.length; i++) {
        int.f.AttachEvent(
			aThumbNails[i],
			'click',
			function () {
			    HotelPopup.ImageGallery.ChangeImage(this)
			}
		);
    };

};

HotelPopup.ShowImagePopup = function (iPropertyReferenceID) {

    int.ff.Call('=iVectorWidgets.HotelPopup.GetPopupContent',
        function (sHTML) {
            HotelPopup.ShowImagePopupDone(sHTML);
        },
        iPropertyReferenceID, 'Images'
    );

};

HotelPopup.ShowImagePopupDone = function (sHTML) {

    //show gallery modal popup
    web.ModalPopup.Show(sHTML, true, 'modalpopup hotelImagePopup');

    //attach onload event for first image
    var oCurrentImage = int.f.GetObject('imgImagePopup_CurrentImage');
    oCurrentImage.onload = function () { HotelPopup.SetupGallery(); };

};

HotelPopup.SetupGallery = function () {

    //set up lightbox image gallery
    var o_HotelImages = new web.LightboxImageGallery('o_HotelImages');
    HotelPopup.ImageGallery = o_HotelImages;
    var oParams = {
        container: 'divProperty_Images',
        currentImage: 'imgImagePopup_CurrentImage',
        imageNavLeft: 'aImageNavLeft',
        imageNavRight: 'aImageNavRight',
        thumbnailContainer: 'divImagePopup_Thumbnails',
        thumbnailHolder: 'divImagePopup_ThumbnailHolder',
        thumbnailPrefix: 'divImagePopup_Thumbnail_',
        thumbnailNavLeft: 'aThumbNavLeft',
        thumbnailNavRight: 'aThumbNavRight',
        modalPopup: int.f.GetElementsByClassName('div', 'hotelImagePopup')[0],
        transitionClass: 'popupTrans'
    };

    //setup gallery
    HotelPopup.ImageGallery.Setup(oParams);

    //set up the events
    HotelPopup.ImageGallerySetup();

    var oCurrentImage = int.f.GetObject('imgImagePopup_CurrentImage');
    oCurrentImage.onload = function () { return false; };
};

//#endregion
var web = new function () {

	this.PubSub = {};
	(function (q) {
		var topics = {}, subUid = -1;
		q.subscribe = function (topic, func) {
			if (!topics[topic]) {
				topics[topic] = [];
			}
			var token = (++subUid).toString();
			topics[topic].push({
				token: token,
				func: func
			});
			return token;
		};

		q.publish = function (topic, args) {
			if (!topics[topic]) {
				return false;
			}
			setTimeout(function () {
				var subscribers = topics[topic],
				len = subscribers ? subscribers.length : 0;

				while (len--) {
					subscribers[len].func(topic, args);
				}
			}, 0);
			return true;

		};

		q.unsubscribe = function (token) {
			for (var m in topics) {
				if (topics[m]) {
					for (var i = 0, j = topics[m].length; i < j; i++) {
						if (topics[m][i].token === token) {
							topics[m].splice(i, 1);
							return token;
						}
					}
				}
			}
			return false;
		};
	}(this.PubSub));

	this.Window = new function () {
		this.Redirect = function (sURL) {

			//	var targetURL = sURL;
			var sRootPath = getRootPath();
			var locationHost = window.location.host;
			var bAbsoluteURL = int.s.StartsWith(sURL, 'http');
			var bDoubleSlash = int.s.StartsWith(sURL, '//');
			var bSecure = int.s.StartsWith(sURL, 'https') || (bAbsoluteURL === false && int.s.StartsWith(window.location.href, 'https'));
			var sProtocol = bSecure ? 'https://' : 'http://';
			var bExternalLink = bAbsoluteURL && sURL.indexOf(locationHost) === -1;

			// if it is relative URL make sure that it also has a '/' prepended
			if (!bAbsoluteURL && !int.s.StartsWith(sURL, '/')) {
				sURL = '/' + sURL;
			}

			//if its not linking to our site then just redirect it
			if (bExternalLink) {
				window.location = sURL;
			}

			//if it is an absolute URL strip out everything except the relative part
			if (bAbsoluteURL) {
				sURL = int.s.Replace(sURL, sProtocol + locationHost, '');
			}

			//if double slash strip out everthing except the relative part
			if (bDoubleSlash) {
				sURL = int.s.Replace(sURL, '//' + locationHost, '');
			}
			
			//if the location already starts with the root path then redirect, else append it
			if (int.s.StartsWith(sURL, sRootPath)) {
				window.location = sProtocol + locationHost + sURL;
			} else {
				window.location = sProtocol + locationHost + sRootPath.concat(sURL);
			}
		}

		this.Replace = function (sURL) {

			//	var targetURL = sURL;
			var sRootPath = getRootPath();
			var locationHost = window.location.host;
			var bAbsoluteURL = int.s.StartsWith(sURL, 'http');
			var bSecure = int.s.StartsWith(sURL, 'https');
			var sProtocol = bSecure ? 'https://' : 'http://';
			var bExternalLink = bAbsoluteURL && sURL.indexOf(locationHost) === -1;

			// if it is relative URL make sure that it also has a '/' prepended
			if (!bAbsoluteURL && !int.s.StartsWith(sURL, '/')) {
				sURL = '/' + sURL;
			}

			//if its not linking to our site then just redirect it
			if (bExternalLink) {
				window.location.replace(sURL);
			}

			//if it is an absolute URL strip out everything except the relative part
			if (bAbsoluteURL) {
				sURL = int.s.Replace(sURL, sProtocol + locationHost, '');
			}

			if (int.s.StartsWith(sURL, sRootPath)) {
				window.location.replace(sURL);
			} else {
				window.location.replace(sRootPath.concat(sURL));
			}

		}


		function getRootPath() {
			var sRootPath = '';
			try {
				sRootPath = INTV.Settings.RootPath;
			} catch (ex) {
				//do nothing
			}
			return sRootPath;
		}

	}

	//#region Tooltip
	this.Tooltip = new function () {

		var me = this;
		this.Object = null;

		//hide
		this.Hide = function () {

			if (me.Object) {
				document.body.appendChild(me.Object);
				int.f.Hide(me.Object);
				me.Object = null;
			}
			else {
				$('div.tooltip').remove();
			}
		}


		//show
		this.Show = function (oObject, sContent, sDirection, oOffset, xOffset, yOffset, sClass) {

			//make sure other tooltips are hidden
			web.Tooltip.Hide();


			//build tooltip
			var oDiv

			//create container
			if (int.f.SafeObject(sContent) != null) {
				oDiv = int.f.SafeObject(sContent);
				int.f.Show(oDiv);
				me.Object = oDiv;
			}
			else {
				oDiv = document.createElement('div');
				// add content
				oDiv.innerHTML = oDiv.innerHTML + sContent;
			}

			oDiv.setAttribute('class', 'tooltip ' + sDirection + ' ' + sClass);
			

			//create arrow
			var oDivArrow = document.createElement('div');
			oDivArrow.setAttribute('class', 'arrow');
			oDiv.appendChild(oDivArrow);




			//get offset
			var offset = (oOffset == undefined || oOffset == null) ? web.getPosition(oObject) : oOffset;
			xOffset = (xOffset == undefined) ? 0 : int.n.SafeNumeric(xOffset);
			yOffset = (yOffset == undefined) ? 0 : int.n.SafeNumeric(yOffset);

			//add to page
			document.body.appendChild(oDiv);

			//get object height and width
			var iObjectHeight = (oObject) ? oObject.clientHeight : 0;
			var iObjectWidth = (oObject) ? oObject.clientWidth : 0;

			//set position
			oDiv.style.position = 'absolute';
			oDiv.style['z-index'] = 102;

			if (sDirection == 'bottom') {
				oDiv.style.left = offset.left + xOffset + 'px';
				oDiv.style.top = (offset.top + iObjectHeight) + yOffset + 'px';
			}
			else if (sDirection == 'top') {
				oDiv.style.left = offset.left + xOffset + 'px';
				oDiv.style.top = (offset.top - oDiv.clientHeight) + yOffset + 'px';
			}
			else if (sDirection == 'left') {
				oDiv.style.left = (offset.left - oDiv.clientWidth) + xOffset + 'px';
				oDiv.style.top = ((offset.top + (iObjectHeight / 2)) - (oDiv.clientHeight / 2)) + yOffset + 'px';
			}
			else if (sDirection == 'right') {
				oDiv.style.left = (offset.left + iObjectWidth) + xOffset + 'px';
				oDiv.style.top = ((offset.top + (iObjectHeight / 2)) - (oDiv.clientHeight / 2)) + yOffset + 'px';
			}

		}

	}
	//#endregion

	//#region JQuery Modal Popup
	this.JQueryModalPopup = new function () {
		var me = this;
		this.Object = null;

		//hide
		this.Hide = function () {

			if (me.Object) {
				//re-insert object at placeholder location

				if (int.f.GetObject('divModalPopupPlaceholder')) {
					var oDivPlaceholder = int.f.SafeObject('divModalPopupPlaceholder');

					oDivPlaceholder.parentNode.insertBefore(me.Object, oDivPlaceholder);

					//remove placeholder
					oDivPlaceholder.parentNode.removeChild(oDivPlaceholder);
				} else {

					if (me.Object.parentNode != null) {
						me.Object.parentNode.removeChild(me.Object);
					};

					document.body.appendChild(me.Object);
					int.f.Hide(me.Object);
					me.Object = null
				}

				int.f.Hide(me.Object);
			}

			$('#divModalPopup_wrapper').remove()
			$('#divModalPopup_background').remove();

		}

		this.Show = function (sContent, bCloseLink, sClassOverride, bRemoveOnClose) {
			//create popup

			if (!int.f.GetObject(oDivPopup)) {
				var oDivPopup = document.createElement('div');
			}
			oDivPopup.id = 'divModalPopup';

			sClassOverride = (sClassOverride == null || sClassOverride == undefined) ? '' : sClassOverride;
			oDivPopup.className = (sClassOverride != '') ? sClassOverride : ''


			//add close link
			if (bCloseLink) {
				var oCloseLink = document.createElement('a');
				int.f.SetHTML(oCloseLink, 'x');
				oCloseLink.className = 'divModalPopup_close close';
				oDivPopup.appendChild(oCloseLink);
			}


			//add content
			var oSourceObject = int.f.SafeObject(sContent);
			if (oSourceObject != null) {

				var oDivPlaceholder = document.createElement('div');
				oDivPlaceholder.id = 'divModalPopupPlaceholder'

				oSourceObject.parentNode.insertBefore(oDivPlaceholder, oSourceObject);
				oSourceObject.parentNode.removeChild(oSourceObject);

				me.Object = oSourceObject;
				oDivPopup.appendChild(me.Object);
				int.f.Show(me.Object);
			}
			else {
				int.f.SetHTML(oDivPopup, oDivPopup.innerHTML + sContent, true);
			}

			document.body.appendChild(oDivPopup);

			$(oDivPopup).popup({
				onclose: function () {
					web.JQueryModalPopup.Hide();
                }
            })
            $(oDivPopup).popup('show');

            $('#divModalPopup_wrapper')
                .on('click',
                    function (event) {
                        if (event.currentTarget.id == 'divModalPopup_wrapper') {
                            web.JQueryModalPopup.Hide();
                        }
                    });

            $('.divModalPopup_close').on('click', function () {
                web.JQueryModalPopup.Hide();
            })

            $('#divModalPopup')
                .on('click',
                    function (event) {
                        event.stopPropagation();
                    });
		}
	}

	//#region Modal Popup
	this.ModalPopup = new function () {

		var me = this;
		this.Object = null;


		//hide
		this.Hide = function () {

			if (me.Object) {
				//re-insert object at placeholder location

				if (int.f.GetObject('divModalPopupPlaceholder')) {
					var oDivPlaceholder = int.f.SafeObject('divModalPopupPlaceholder');

					oDivPlaceholder.parentNode.insertBefore(me.Object, oDivPlaceholder);

					//remove placeholder
					oDivPlaceholder.parentNode.removeChild(oDivPlaceholder);
				} else {

					if (me.Object.parentNode != null) {
						me.Object.parentNode.removeChild(me.Object);
					};

					document.body.appendChild(me.Object);
					int.f.Hide(me.Object);
					me.Object = null
				}

				int.f.Hide(me.Object);
			}

			$('div.overlay, div.modalpopup').remove();
		}


		//show
		this.Show = function (sContent, bCloseLink, sClassOverride, iTop, iLeft, sContainer, sOverlayClassOverride) {

			//close any current popups
			web.ModalPopup.Hide();



			//create overlay
			var oDivOverlay = document.createElement('div');

			sOverlayClassOverride = (sOverlayClassOverride == null || sOverlayClassOverride == undefined) ? '' : sOverlayClassOverride;
			oDivOverlay.className = (sOverlayClassOverride != '') ? sOverlayClassOverride : 'overlay'
			oDivOverlay.id = 'divOverlay';

			oDivOverlay.style.height = '100%';
			oDivOverlay.style.width = '100%';

			document.body.appendChild(oDivOverlay);


			//create popup
			var oDivPopup = document.createElement('div');
			oDivPopup.id = 'divModalPopup';

			sClassOverride = (sClassOverride == null || sClassOverride == undefined) ? '' : sClassOverride;
			oDivPopup.className = (sClassOverride != '') ? sClassOverride : 'modalpopup'


			//add close link
			if (bCloseLink) {
				var oCloseLink = document.createElement('a');
				int.f.SetHTML(oCloseLink, 'x');
				oCloseLink.setAttribute('href', 'javascript:web.ModalPopup.Hide()');
				oCloseLink.className = 'close';
				oDivPopup.appendChild(oCloseLink);
			}


			//add content
			var oSourceObject = int.f.SafeObject(sContent);
			if (oSourceObject != null) {

				//remove original element and insert placeholder
				var oDivPlaceholder = document.createElement('div');
				oDivPlaceholder.id = 'divModalPopupPlaceholder'

				oSourceObject.parentNode.insertBefore(oDivPlaceholder, oSourceObject);
				oSourceObject.parentNode.removeChild(oSourceObject);

				me.Object = oSourceObject;
				oDivPopup.appendChild(me.Object);
				int.f.Show(me.Object);
			}
			else {
				int.f.SetHTML(oDivPopup, oDivPopup.innerHTML + sContent, true);
			}


			//add to page
			if (sContainer == undefined || sContainer == '') {
				document.body.appendChild(oDivPopup);
			} else {
				//Allow us to set an container our modal popup is insterted into (I know its absolute positioned but for responsive design,
				//where we need to set widths based on parent elements widths, this is important -JS.
				var oContainer = int.f.GetObject(sContainer);
				oContainer.appendChild(oDivPopup);
			}



			//add escape key listener
			document.onkeyup = function (evt) {
				evt = evt || window.event;
				if (evt.keyCode == 27) web.ModalPopup.Hide();
			};

			//set position
			web.ModalPopup.Centre(oDivPopup, iLeft, iTop);

		}

		this.Centre = function (oDivPopup, iLeft, iTop) {

			oDivPopup = int.f.SafeObject(oDivPopup);

			//set position
			var iScrollTop = ($(window).scrollTop() != 0) ? $(window).scrollTop() : document.body.scrollTop;
			var iScrollLeft = ($(window).scrollLeft() != 0) ? $(window).scrollLeft() : document.body.scrollLeft;

			var iMidHeight = $(window).height() / 2;
			var iPopupMidHeight = oDivPopup.clientHeight / 2;

			//check if popup height is bigger than window height, if so adjust acordingly
			if (oDivPopup.clientHeight > $(window).height()) {
				iMidHeight = iMidHeight + ((oDivPopup.clientHeight - $(window).height()) / 2) + 100;
			}

			var iMidWidth = $(window).width() / 2;
			var iPopupMidWidth = oDivPopup.clientWidth / 2;

			oDivPopup.style.left = iLeft == undefined || (iLeft == '' && !int.v.IsNumeric(iLeft)) ? iScrollLeft + (iMidWidth - iPopupMidWidth) + 'px' : iLeft + 'px';

			oDivPopup.style.top = iTop == undefined || (iTop == '' && !int.v.IsNumeric(iTop)) ? iScrollTop + (iMidHeight - iPopupMidHeight) + 'px' : iTop + 'px';

		}

	}

	//#endregion


	//#region InfoBox
	this.InfoBox = new function () {

		//settings
		var me = this;
		this.Container = 'divAll';
		this.InsertBefore = 'divMain';


		//close infobox
		this.Close = function () {
			$('.infobox').hide(600);
			for (aInfoBoxes = int.f.GetElementsByClassName('div', 'infobox'), i = 0; i < aInfoBoxes.length; i++) {
				aInfoBoxes[i].parentNode.removeChild(aInfoBoxes[i]);
			};
		}


		//show infobox
		this.Show = function (sWarning, sType, bAgreementRequired, sInsertAfter, sClass, iOffset, iViewPortOffset) {
			if (typeof (iOffset) === 'undefined') iOffset = 20;
			if (typeof (iViewPortOffset) === 'undefined') iViewPortOffset = 0;

			//hide warnings
			web.InfoBox.Close();

			var aWarnings = new Array();

			if (Object.prototype.toString.call(sWarning) === '[object Array]') {
				aWarnings = sWarning;
			}
			else {
				aWarnings.push(sWarning.toString());
			};

			for (var iWarning = 0; iWarning < aWarnings.length; iWarning++) {
				//type - warning, information, success

				if (sClass === null || sClass === undefined) {
					sClass = '';
				};

				//create container
				var oDiv = document.createElement('div');
				oDiv.className = 'infobox ' + sType + ' ' + sClass;

				if (!bAgreementRequired) {
					//add close button
					var oClose = document.createElement('a');
					oClose.className = 'close';
					oClose.setAttribute('href', 'javascript:web.InfoBox.Close()');
					oClose.innerHTML = 'x';
					oDiv.appendChild(oClose);
				}

				//create paragraph
				var oText = document.createElement('p');
				oDiv.appendChild(oText);


				//set icon class
				var sIconClass = 'icon';

				if (sType === 'warning') {
					sIconClass = sIconClass + ' warning_sign';
				}
				else if (sType === 'information') {
					sIconClass = sIconClass + ' circle_info';
				}
				else if (sType === 'success') {
					sIconClass = sIconClass + ' ok_2';
				};

				oText.className = sIconClass;
				oText.setAttribute('id', 'pWarning');


				//add icon
				var oIcon = document.createElement('i');
				oText.appendChild(oIcon);


				//add message
				oText.innerHTML = oText.innerHTML + aWarnings[iWarning];

				//if we require them to accept the warning add a cb.
				if (bAgreementRequired) {
					var oAgree = document.createElement('input');
					oAgree.setAttribute('type', 'checkbox');
					oAgree.setAttribute('id', 'chkAcceptWarning');
					oAgree.setAttribute('name', 'chkAcceptWarning');
					oText.appendChild(oAgree);
				}

				//add to page
				var oContainer = int.f.GetObject(me.Container);

				if (sInsertAfter != undefined && sInsertAfter != '') {
					var oReferenceNode = int.f.GetObject(sInsertAfter);
					oReferenceNode.parentNode.insertBefore(oDiv, oReferenceNode.nextSibling);
				}
				else {
					oContainer.insertBefore(oDiv, oContainer.firstChild);
				}

				//scroll up to it
				int.e.ScrollIntoView(oDiv, iOffset, 2, iViewPortOffset);

			}
		}
	}

	//#endregion


	//#region Datepicker
	//requires datepicker.js from jQueryUI
	this.DatePicker = new function () {

		//settings
		var me = this;
		this.numberOfMonths = 1;
		this.dateFormat = 'dd/mm/yy';
		this.firstDay = 1;

		this.dayNamesMin = ["S", "M", "T", "W", "T", "F", "S"];
		this.dayNamesShort = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
		this.monthNamesShort = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

		//setup function for datepicker textboxes
		this.Setup = function (sID, dMinDate, dDefaultDate, iDatepickerMonths, iDatepickerFirstDay, iFirstDay, oFunction, sClass, bShowDropdowns, dMaxDate, oBeforeShowFunction, oIconClickFunction,
			sShowAnim, sDateFormat, bUseShortDates, iSelectedMonthPos, showOn, buttonText, beforeShowDayFunction, sCultureCode) {

			if (sCultureCode !== undefined && sCultureCode !== '' && $.datepicker.regional[sCultureCode]) {
				me.dateFormat = $.datepicker.regional[sCultureCode].dateFormat;
				me.firstDay = $.datepicker.regional[sCultureCode].firstDay;
				me.dayNamesMin = $.datepicker.regional[sCultureCode].dayNamesMin;
				me.dayNamesShort = $.datepicker.regional[sCultureCode].dayNamesShort;
				me.monthNamesShort = $.datepicker.regional[sCultureCode].monthNamesShort;
			}
			me.dateFormat = (sDateFormat !== '' && sDateFormat !== undefined) ? sDateFormat : me.dateFormat;

			if (iDatepickerMonths !== undefined && iDatepickerMonths !== '') me.numberOfMonths = iDatepickerMonths;
			if (iDatepickerMonths !== undefined && iDatepickerFirstDay >= 0 && iDatepickerFirstDay <= 6) me.firstDay = iDatepickerFirstDay;

			var datepickerInitObj = {
				numberOfMonths: me.numberOfMonths,
				dateFormat: me.dateFormat,
				minDate: dMinDate,
				maxDate: dMaxDate,
				defaultDate: dDefaultDate,
				firstDay: me.firstDay,
				onSelect: oFunction,
				dayNamesMin: me.dayNamesMin,
				dayNamesShort: me.dayNamesShort,
				beforeShow: (oBeforeShowFunction != undefined && oBeforeShowFunction !== '') ? oBeforeShowFunction : '',
				showAnim: (sShowAnim !== undefined) ? sShowAnim : 'show', //do not check for empty string because that's how we remove the animation
				showCurrentAtPos: (iSelectedMonthPos !== undefined) ? iSelectedMonthPos : 0,
				constrainInput: true,
				changeMonth: (bShowDropdowns === 'True') ? true : false,
				changeYear: (bShowDropdowns === 'True') ? true : false,
                beforeShowDay: beforeShowDayFunction,
                inline: true
			}

			if (showOn) {
				datepickerInitObj['showOn'] = showOn;
			}
			if (buttonText) {
				datepickerInitObj['buttonText'] = buttonText;
			}

			$(function () {
				$(sID).datepicker(datepickerInitObj);
			});

			if (bShowDropdowns === 'True') {
				$(sID).datepicker("option", 'changeMonth', true);
				$(sID).datepicker("option", 'changeYear', true);
			}

			if (bUseShortDates === 'True') {
				$(sID).datepicker("option", 'monthNames', me.monthNamesShort);
			}

			//set to icon
			var bindTo = 'div.calendar i';
			if (sClass != undefined && sClass !== '') {
				//multiple datepickers on page
				bindTo = bindTo + '.' + sClass;
			};


			//attach event
			$(bindTo).bind("click",
				function () {
					if (oIconClickFunction != undefined && oIconClickFunction !== '') oIconClickFunction();
					$(sID).datepicker("show");
				}
			);
		}

		//Same as above but uses an object rather than 16 params.
		this.SetupFromObject = function (oParams) {

			if (oParams.CultureCode !== undefined && oParams.CultureCode !== '' && $.datepicker.regional[oParams.CultureCode]) {
				me.dateFormat = $.datepicker.regional[oParams.CultureCode].dateFormat;
				me.firstDay = $.datepicker.regional[oParams.CultureCode].firstDay;
				me.dayNamesMin = $.datepicker.regional[oParams.CultureCode].dayNamesMin;
				me.dayNamesShort = $.datepicker.regional[oParams.CultureCode].dayNamesShort;
				me.monthNamesShort = $.datepicker.regional[oParams.CultureCode].monthNamesShort;
			}
			me.dateFormat = (oParams.DateFormat !== '' && oParams.DateFormat != undefined) ? oParams.DateFormat : me.dateFormat;

			if (oParams.DatepickerMonths != undefined) me.numberOfMonths = oParams.DatepickerMonths;
			if (oParams.DatepickerFirstDay != undefined && oParams.DatepickerFirstDay >= 0 && oParams.DatepickerFirstDay <= 6) me.firstDay = oParams.DatepickerFirstDay;

			var datepickerInitObj = {
				numberOfMonths: me.numberOfMonths,
				dateFormat: me.dateFormat,
				minDate: oParams.MinDate,
				maxDate: oParams.MaxDate,
				defaultDate: oParams.DefaultDate,
				firstDay: me.firstDay,
				onSelect: oParams.Function,
				dayNamesMin: me.dayNamesMin,
				dayNamesShort: me.dayNamesShort,
				beforeShow: (oParams.BeforeShowFunction != undefined && oParams.BeforeShowFunction !== '') ? oParams.BeforeShowFunction : '',
				showAnim: (oParams.ShowAnim != undefined) ? oParams.ShowAnim : 'show', //do not check for empty string because that's how we remove the animation
				showCurrentAtPos: (oParams.SelectedMonthPos != undefined) ? oParams.SelectedMonthPos : 0,
				changeMonth: (oParams.changeMonth != undefined) ? oParams.changeMonth : false,
				changeYear: (oParams.changeYear != undefined) ? oParams.changeYear : false,
				yearRange: (oParams.yearRange != undefined) ? oParams.yearRange : "c-10:c+10",
				constrainInput: (oParams.ConstrainInput != undefined) ? oParams.ConstrainInput : true,
				beforeShowDay: oParams.BeforeShowDayFunction
			}

			if (oParams.showOn) {
				datepickerInitObj['showOn'] = oParams.showOn;
			}
			if (oParams.buttonText) {
				datepickerInitObj['buttonText'] = oParams.buttonText;
			}

			$(function () {
				$(oParams.ID).datepicker(datepickerInitObj);

			});

			if (oParams.UseShortDates === 'True' || oParams.UseShortDates) {
				$(oParams.ID).datepicker("option", 'monthNames', me.monthNamesShort);
			}

			//set to icon
			var bindTo = 'div.calendar i';
			if (oParams.Class != undefined && oParams.Class !== '') {
				//multiple datepickers on page
				bindTo = bindTo + '.' + oParams.Class;
			};

			//attach event
			$(bindTo).bind("click",
				function () {
					if (oParams.IconClickFunction !== undefined && oParams.IconClickFunction !== '') oParams.IconClickFunction();
					$(oParams.ID).datepicker("show");
				}
			);
		}
	}

	//#endregion

	//#region Content Scroller
	this.ContentRotator = function (object) {

		var me = this;

		this.Object = object;


		this.ScrollDirection;
		this.AutoScroll;

		this.Container;
		this.Holder;
		this.PageCount;
		this.PageWidth;
		this.PageHeight;
		this.PageMargin;


		this.CurrentPage;
		this.Seconds;

		this.CurrentLeft;
		this.TargetLeft;

		this.CurrentTop;
		this.TargetTop;

		this.StartTime;
		this.TimeOutID;
		this.Paused;
		this.Sliding;
		this.CallBackFunction;

		this.Setup = function (sContainer, sHolder, iPageCount, iPageWidth, iPageMargin, iSeconds, iCurrentPage, sScrollDirection, bAutoScroll, iPageHeight) {
			this.Container = int.f.GetObject(sContainer);
			this.Holder = int.f.GetObject(sHolder);
			this.PageCount = iPageCount;
			this.PageWidth = iPageWidth;
			this.PageHeight = iPageHeight;
			this.PageMargin = iPageMargin;
			this.Seconds = iSeconds;
			this.CurrentPage = iCurrentPage;
			this.CurrentLeft = 0 - (me.PageWidth * (iCurrentPage - 1));
			this.CurrentTop = 0 - (me.PageHeight * (iCurrentPage - 1));
			this.TimeOutID = -1;
			this.Paused = false;
			this.TargetLeft = 0;
			this.TargetTop = 0;
			this.ScrollDirection = sScrollDirection;
			this.AutoScroll = bAutoScroll;

			if (this.AutoScroll !== false) setTimeout(this.Object + '.Forward();', 5000);
		}

		this.Forward = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			if (me.CurrentPage < me.PageCount) {
				me.TargetLeft = me.CurrentLeft - me.PageWidth;
				me.CurrentPage += 1;
			}
			else {
				me.TargetLeft = 0;
				me.CurrentPage = 1;
			};

			if (Math.abs(me.TargetLeft) <= (parseInt(me.Holder.style.width) - me.Container.clientWidth)) {
				this.StartTime = new Date();
				this.Slide();
			}
			else {
				me.CurrentPage = me.PageCount;
			};

			if (Math.abs(me.TargetLeft) == (parseInt(me.Holder.style.width) - me.Container.clientWidth) - me.PageMargin) {
				me.CurrentPage = me.PageCount;
			}

		}

		this.Backward = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			//work out target left
			if (me.CurrentPage > 1) {
				me.TargetLeft = me.CurrentLeft + me.PageWidth;
				me.CurrentPage -= 1;
			};

			if (me.TargetLeft <= 0) {
				this.StartTime = new Date();
				this.Slide();
			}
		}

		this.Down = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			if (me.CurrentPage < me.PageCount) {
				me.TargetTop = me.CurrentTop - me.PageHeight;
				me.CurrentPage += 1;
			}
			else {
				me.TargetTop = 0;
				me.CurrentPage = 1;
			};

			if (Math.abs(me.TargetTop) <= (parseInt(me.Holder.style.height) - me.Container.clientHeight)) {
				this.StartTime = new Date();
				this.Slide();
			}
			else {
				me.CurrentPage = me.PageCount;
			};

			if (Math.abs(me.TargetLeft) == (parseInt(me.Holder.style.height) - me.Container.clientHeight) - me.PageMargin) {
				me.CurrentPage = me.PageCount;
			}

		}

		this.Up = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			//work out target left
			if (me.CurrentPage > 1) {
				me.TargetTop = me.CurrentTop + me.PageHeight;
				me.CurrentPage -= 1;
			};

			if (me.TargetTop <= 0) {
				this.StartTime = new Date();
				this.Slide();
			}
		}

		this.Slide = function () {

			me.Sliding = true;

			var nFractionDone = Math.abs((new Date() - me.StartTime) / me.Seconds / 1000);

			if (me.ScrollDirection == 'Vertical') {
				if (nFractionDone < 1) {
					me.Holder.style.top = me.CurrentTop + (me.TargetTop - me.CurrentTop) * Math.sin(Math.PI / 2 * nFractionDone) + 'px';
					me.TimeOutID = setTimeout(this.Object + '.Slide();', 20);
				}
				else {
					me.Holder.style.top = me.TargetTop + 'px';
					me.CurrentTop = me.TargetTop;
					me.Sliding = false;
					if (this.AutoScroll !== false) me.TimeOutID = setTimeout(this.Object + '.Down();', 5000);
				}
			}
			else {
				if (nFractionDone < 1) {
					me.Holder.style.left = me.CurrentLeft + (me.TargetLeft - me.CurrentLeft) * Math.sin(Math.PI / 2 * nFractionDone) + 'px';
					me.TimeOutID = setTimeout(this.Object + '.Slide();', 20);
				}
				else {
					me.Holder.style.left = me.TargetLeft + 'px';
					me.CurrentLeft = me.TargetLeft;
					me.Sliding = false;
					if (this.AutoScroll !== false) me.TimeOutID = setTimeout(this.Object + '.Forward();', 5000);
				}
			}


		}


		this.Pause = function () {
			this.Paused = true;
			clearTimeout(me.TimeOutID);
		}


		this.SelectImage = function (iImage) {

			me.Pause();

			me.CurrentPage = iImage;
			me.TargetLeft = -((iImage - 1) * me.PageWidth);

			this.StartTime = new Date();
			ImageSlider.Slide();

			me.TimeOutID = setTimeout(this.Object + '.Forward();', 5000);

		}

	}

	//#endregion


	//#region Responsive Content Scroller
	this.ResponsiveContentRotator = function (object) {

		var me = this;

		this.Object = object;


		this.ScrollDirection;
		this.AutoScroll;

		this.Container;
		this.Holder;
		this.PageCount;
		this.PageWidth;
		this.PageHeight;
		this.PageMargin;


		this.CurrentPage;
		this.Seconds;

		this.CurrentLeft;
		this.TargetLeft;

		this.CurrentTop;
		this.TargetTop;

		this.StartTime;
		this.TimeOutID;
		this.Paused;
		this.Sliding;
		this.CallBackFunction;

		this.TouchSwipe;
		this.NoWrap;

		this.StartX
		this.StartY

		this.Setup = function (sContainer, sHolder, iPageCount, iPageWidth, iPageMargin, iSeconds, iCurrentPage, sScrollDirection, bAutoScroll, iPageHeight, iElementsPerView, sNavItemPrefix, bTouchSwipe, bNoWrap, iDelay) {
			this.Container = int.f.GetObject(sContainer);
			this.Holder = int.f.GetObject(sHolder);
			this.PageCount = iPageCount;
			this.PageWidth = iPageWidth;
			this.PageHeight = iPageHeight;
			this.PageMargin = Math.round(iPageMargin * 100) / 100;
			this.Seconds = iSeconds;
			this.CurrentPage = iCurrentPage;
			this.CurrentLeft = 0 - (me.PageWidth * (iCurrentPage - 1));
			this.CurrentTop = 0 - (me.PageHeight * (iCurrentPage - 1));
			this.TimeOutID = -1;
			this.Paused = false;
			this.TargetLeft = 0;
			this.TargetTop = 0;
			this.ScrollDirection = sScrollDirection;
			this.AutoScroll = bAutoScroll;
			this.NavItemPrefix = sNavItemPrefix;
			this.TouchSwipe = bTouchSwipe;
			this.StartX = 0;
			this.StartY = 0;
			this.ElementsPerView = iElementsPerView;
			this.NoWrap = bNoWrap;
			this.Delay = iDelay
			if (iDelay === null || iDelay === undefined) {
				this.Delay = 5000
			}

			this.Touching = false;

			//set current selected image
			int.f.AddClass(int.f.GetObject(me.NavItemPrefix + me.CurrentPage), 'selected');



			if (this.AutoScroll !== false) setTimeout(this.Object + '.Forward();', me.Delay);

			//Attach touch events if necessary
			if (this.TouchSwipe) {
				//Set initial finger position
				int.f.AttachEvent(me.Container, 'touchstart',
					function (event) {
						//Get finger start position
						me.Touching = true;

						var fingerStartPosition = event.touches[0];
						me.StartX = fingerStartPosition.screenX;
						me.StartY = fingerStartPosition.screenY;
						return false;
					});

				//Check whether finger moved left/right
				int.f.AttachEvent(me.Container, 'touchmove',
					function (event) {

						if (me.Touching == true) {

							//Get end finger position
							var fingerEndPosition = event.touches[0];
							var iEndX = fingerEndPosition.screenX;
							var iEndY = fingerEndPosition.screenY;

							//Check whether swipe was more vertical or more horizontal
							var iDifferenceX = me.StartX - iEndX
							var iDifferenceY = me.StartY - iEndY

							//If more horizontal then swipe (with added tolerance)
							var iXYDifference = Math.abs(iDifferenceX) - Math.abs(iDifferenceY);
							if (iXYDifference > 50) {

								me.Touching = false;

								//Swipe Right
								if (iDifferenceX < 0) {
									me.Backward();
									return false;
								}
									//Swipe Left
								else if (iDifferenceX > 0) {
									me.Forward();
									return false;
								}
							}
						}
					}
				);
			}
		}

		this.Forward = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			if (me.CurrentPage < me.PageCount) {
				me.TargetLeft = -((me.CurrentPage) * me.PageWidth)
				me.CurrentPage += me.ElementsPerView;
			}
			else if (me.NoWrap == true) {
				return;
			}
			else {
				me.TargetLeft = 0;
				me.CurrentPage = 1;
			};
			//Get the width of holder and remove the unit.
			var iHolderWidth = me.Holder.style.width;
			iHolderWidth = iHolderWidth.substring(0, iHolderWidth.length - 1)


			if (Math.abs(Math.ceil(me.TargetLeft * 100) / 100) <= (Math.ceil((iHolderWidth - 100) * 100) / 100)) {
				me.StartTime = new Date();
				me.Slide();
			}
			else {
				me.CurrentPage = me.PageCount;
			};

			if (Math.abs(Math.round(me.TargetLeft * 100) / 100) == (iHolderWidth - 100 - me.PageMargin)) {
				me.CurrentPage = me.PageCount;
			}

		}

		this.Backward = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			//work out target left
			if (me.CurrentPage > 1) {
				me.TargetLeft = (Math.round(me.CurrentLeft * 100) / 100) + (Math.round(me.PageWidth * 100) / 100);
				me.CurrentPage -= 1;
			}
			else if (me.NoWrap == true) {
				return;
			}
			else {
				me.TargetLeft = -(Math.round((me.PageCount - 1) * (me.PageWidth * 100) / 100));
				me.CurrentPage = me.PageCount;
			};

			if (me.TargetLeft <= 0) {
				me.StartTime = new Date();
				me.Slide();
			}
		}

		this.Down = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			if (me.CurrentPage < me.PageCount) {
				me.TargetTop = me.CurrentTop - me.PageHeight;
				me.CurrentPage += 1;
			}
			else {
				me.TargetTop = 0;
				me.CurrentPage = 1;
			};

			if (Math.abs(me.TargetTop) <= (parseInt(me.Holder.style.height) - me.Container.clientHeight)) {
				this.StartTime = new Date();
				this.Slide();
			}
			else {
				me.CurrentPage = me.PageCount;
			};

			if (Math.abs(me.TargetLeft) == (parseInt(me.Holder.style.height) - me.Container.clientHeight) - me.PageMargin) {
				me.CurrentPage = me.PageCount;
			}

		}

		this.Up = function () {

			//if still sliding return
			if (me.Sliding) return;

			//clear current timeout
			clearTimeout(me.TimeOutID);

			//work out target left
			if (me.CurrentPage > 1) {
				me.TargetTop = me.CurrentTop + me.PageHeight;
				me.CurrentPage -= 1;
			};

			if (me.TargetTop <= 0) {
				this.StartTime = new Date();
				this.Slide();
			}
		}

		this.Slide = function () {

			//To prevent autoscroll going crazy when clicking out of sequence
			clearTimeout(me.TimeOutID);

			me.Sliding = true;

			var nFractionDone = Math.abs((new Date() - me.StartTime) / me.Seconds / 1000);

			if (me.ScrollDirection == 'Vertical') {
				if (nFractionDone < 1) {
					me.Holder.style.top = me.CurrentTop + (me.TargetTop - me.CurrentTop) * Math.sin(Math.PI / 2 * nFractionDone) + '%';
					me.TimeOutID = setTimeout(this.Object + '.Slide();', 20);
				}
				else {
					me.Holder.style.top = me.TargetTop + '%';
					me.CurrentTop = me.TargetTop;
					me.Sliding = false;
					if (this.AutoScroll !== false) me.TimeOutID = setTimeout(this.Object + '.Down();', me.Delay);
				}
			}
			else {
				if (nFractionDone < 1) {
					me.Holder.style.left = me.CurrentLeft + (me.TargetLeft - me.CurrentLeft) * Math.sin(Math.PI / 2 * nFractionDone) + '%';
					me.TimeOutID = setTimeout(this.Object + '.Slide();', 20);
				}
				else {
					me.Holder.style.left = me.TargetLeft + '%';
					me.CurrentLeft = me.TargetLeft;
					me.Sliding = false;
					if (this.AutoScroll !== false) me.TimeOutID = setTimeout(this.Object + '.Forward();', me.Delay);
				}
			}


			//remove selected class on slider links
			var aSelectedLinks = int.f.GetObjectsByIDPrefix(me.NavItemPrefix, 'a', me.Container);
			for (var i = 0; i < aSelectedLinks.length; i++) {
				int.f.RemoveClass(aSelectedLinks[i], 'selected');
			}

			//set current selected image
			int.f.AddClass(int.f.GetObject(me.NavItemPrefix + me.CurrentPage), 'selected');


		}


		this.Pause = function () {
			this.Paused = true;
			clearTimeout(me.TimeOutID);
		}

		this.TargetPage = function (iTarget) {

			iCurrentPage = iTarget - 1;

			if (iCurrentPage < 1) {
				iCurrentPage = me.PageCount;
			};

			me.CurrentPage = iCurrentPage;
			me.Forward();

		}

		this.SelectImage = function (iImage, obutton) {

			me.Pause();

			me.CurrentPage = iImage;
			me.TargetLeft = -((iImage - 1) * me.PageWidth);


			//remove selected class on slider links
			var aSelectedLinks = int.f.GetObjectsByIDPrefix(me.NavItemPrefix, 'a', me.Container);
			for (var i = 0; i < aSelectedLinks.length; i++) {
				int.f.RemoveClass(aSelectedLinks[i], 'selected');
			}

			//set current selected image
			int.f.AddClass(int.f.GetObject(me.NavItemPrefix + me.CurrentPage), 'selected');


			this.StartTime = new Date();
			me.Slide();

			if (this.AutoScroll !== false) me.TimeOutID = setTimeout(this.Object + '.Forward();', me.Delay);

		}

	}

	//#endregion


	//#region Image Hover
	this.ImageHover = function (element, mainimage, container, dataAttribute) {

		var sNewSrc = '';
		if (dataAttribute) {
			//if we are loading an image not yet cached by the browser there is sometimes a lag
			//in this case set image src to the smaller image so the change is instant, the larger image will 
			//sometimes be very pixelated for a moment but will then update to the correct one.
			int.f.GetObject(mainimage).setAttribute('src', element.src);
			int.f.GetObject(mainimage).setAttribute('src', element.getAttribute(dataAttribute));
		} else {
			int.f.GetObject(mainimage).setAttribute('src', element.src);
		}

		var aImages = int.f.GetObject(container).getElementsByTagName('img');

		for (var i = 0; i < aImages.length; i++) {
			int.f.SetClassIf(aImages[i], 'selected', aImages[i] == element);
		};

	};
	//#endregion


	//#region Slider
	this.Slider = new function () {


		//settings
		var me = this;

		this.Container;
		this.Width;

		this.IsSliding = false;

		this.Slider;
		this.SliderControl;
		this.SliderControlWidth;

		this.MouseStartLeft;
		this.MinBound;
		this.MaxBound;


		//setup
		this.Setup = function (sContainer) {

			// 1. set settings
			me.Container = int.f.GetObject(sContainer);

			// 2. attach events			
			for (var aSliders = int.f.GetElementsByClassName('a', 'sliderbutton', me.Container), i = 0; i < aSliders.length; i++) {
				int.f.AttachEvent(
					aSliders[i],
					'mousedown',
					function (event) {
						me.MouseDown(event);
						return false;
					}
				);

				int.f.AttachEvent(aSliders[i], 'touchstart',
				function (event) {
					me.TouchStart(event);
					return false;
				});

			};

			int.f.AttachEvent(document,'mousemove',
				function(event) {
					if (me.IsSliding) {
						me.MouseMoveSelector(event);
						return false;
					}
				}
			);

			int.f.AttachEvent(
				document,
				'mouseup',
				function (event) {
					if (me.IsSliding) {
						me.MouseUp(event);
						return false;
					}
				}
			);




			int.f.AttachEvent(document, 'touchmove',
				function (event) {
					if (me.IsSliding) {
						me.TouchMove(event);
						return false;
					}
				});

			int.f.AttachEvent(
				document,
				'touchend',
				function (event) {
					if (me.IsSliding) {
						me.MouseUp(event);
						return false;
					}
				}
			);

		}



		//mousedown
		this.MouseDown = function (oEvent) {

			//stops cursor turning into 'text' rathen than a pointer
			(oEvent.preventDefault) ? oEvent.preventDefault() : oEvent.returnValue = false;


			//get slider control details
			me.SliderControl = int.f.GetObjectFromEvent(oEvent);
			me.SliderControlWidth = int.e.GetPosition(me.SliderControl).Width;


			//work out positions
			me.StartLeft = int.e.GetPosition(me.SliderControl).Left;
			me.Width = int.e.GetPosition(me.SliderControl).Width;


			//get slider
			me.Slider = me.GetSlider(me.SliderControl);


			//calculate bounds
			me.MouseStartLeft = me.GetMouseCoords(oEvent).x;

			if (me.SliderControl.id.indexOf('Start') > 0) {
				me.MinBound = (-me.SliderControlWidth + 1) / 2;
				me.MaxBound = me.Slider.EndLeft - me.Slider.Left - me.SliderControlWidth;
			}
			else {

				if (int.f.GetObject(me.Slider.ID + '_Start')) {
					me.MinBound = me.Slider.StartLeft - me.Slider.Left + me.SliderControlWidth;
				} else {
					me.MinBound = (-me.SliderControlWidth + 1) / 2;
				}

				me.MaxBound = me.Slider.Width - me.SliderControlWidth / 2 - 1;
			}

			document.body.focus();
			me.SliderControl.ondragstart = function () {
				return false;
			};
			me.SliderControl.onselectstart = function () {
				return false;
			};
			me.IsSliding = true;
			return false;

		}

		this.MouseMoveSelector = function (oEvent) {
			if (me.Slider.IsStepSlider) {
				me.MouseStepMove(oEvent);
			} else {
				me.MouseMove(oEvent);
			}
		}

		//mousemove
		this.MouseMove = function (oEvent) {

			var bIsTimeSlider = int.f.HasClass(me.SliderControl.parentNode, 'time');
			var bLinearSlider = int.f.HasClass(me.SliderControl.parentNode, 'linear');
			var sTimeValue = '';
			var iPos = me.StartLeft + me.GetMouseCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;

			var iCalculatedPosition;
			if (iPos >= me.MinBound && iPos <= me.MaxBound) {
				iCalculatedPosition = me.StartLeft + me.GetMouseCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;
			}
			else if (iPos < me.MinBound) {
				iCalculatedPosition = me.MinBound;
			}
			else {
				iCalculatedPosition = me.MaxBound;
			}
			me.SliderControl.style.left = iCalculatedPosition + 'px';


			var iPercent = Math.round((iCalculatedPosition + (me.SliderControlWidth - 1) / 2) / me.Slider.Width * 100);
			iPercent = iPercent == 99 ? 100 : iPercent;
			iPercent = iPercent == 1 ? 0 : iPercent;

			if (bIsTimeSlider || bLinearSlider) {
				var iValue = Math.round((this.Slider.MaxValue - this.Slider.MinValue) * iPercent / 100 + this.Slider.MinValue);
			}
			else {
				var iValue = (me.log10(this.Slider.MaxValue) - me.log10(this.Slider.MinValue)) * iPercent / 100 + me.log10(this.Slider.MinValue);
				iValue = Math.round(Math.pow(10, iValue));
			}

			// if its a time slider then do it a little differently			
			if (bIsTimeSlider) {

				//work out the time value
				var iHours = int.n.SafeInt(iValue / 60);
				if (iHours < 10) iHours = '0' + iHours;
				var iMinutes = iValue % 60;
				if (iMinutes < 10) iMinutes = '0' + iMinutes;
				var sTime = iHours + ':' + iMinutes;
				sTimeValue = sTime

				if (me.Slider.ShowControl) {
					int.f.Show(me.Slider.ShowControl);
					int.f.SetHTML(me.Slider.ShowControl, sTime);
					me.Slider.ShowControl.style.left = iCalculatedPosition - (int.e.GetPosition(me.Slider.ShowControl).Width - me.SliderControlWidth) / 2 + 'px';
				}
			}
			else if (me.Slider.ShowControl) {
				int.f.Show(me.Slider.ShowControl);
				int.f.SetHTML(me.Slider.ShowControl, iValue);
				me.ShowControl.style.left = iCalculatedPosition - (int.e.GetPosition(me.Slider.ShowControl).Width - me.SliderControlWidth) / 2 + 'px';
			}

			if (me.SliderControl.id.indexOf('_Start') > 0) {
				int.f.SetValue(me.Slider.ID + '_MinValue', iValue);

				var iBookingAdjustmentValue = 0

				if (int.f.GetObject(me.Slider.ID + '_BookingAdjustmentValue')) {
					iBookingAdjustmentValue += int.f.GetValue(me.Slider.ID + '_BookingAdjustmentValue')
				}

				if (me.Slider.DisplayMin) {
					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round((parseFloat(iValue) + parseFloat(iBookingAdjustmentValue)) / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;
					int.f.SetHTML(me.Slider.DisplayMin, bIsTimeSlider ? sTimeValue : iDisplayValue);
				}

				if (me.Slider.DisplayMinSlider) {

					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round((parseFloat(iValue) + parseFloat(iBookingAdjustmentValue)) / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;

					var sMinDisplayHTML = bIsTimeSlider ? sTimeValue : iDisplayValue
					if (me.Slider.DisplayPrefix) {
						sMinDisplayHTML = me.Slider.DisplayPrefix + sMinDisplayHTML;
					};
					if (me.Slider.DisplaySuffix) {
						sMinDisplayHTML = sMinDisplayHTML + me.Slider.DisplaySuffix;
					};
					int.f.SetHTML(me.Slider.DisplayMinSlider, sMinDisplayHTML);
				}

			}
			else {
				int.f.SetValue(me.Slider.ID + '_MaxValue', iValue);

				var iBookingAdjustmentValue = 0

				if (int.f.GetObject(me.Slider.ID + '_BookingAdjustmentValue')) {
					iBookingAdjustmentValue += int.f.GetValue(me.Slider.ID + '_BookingAdjustmentValue')
				}

				if (me.Slider.DisplayMax) {
					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round((parseFloat(iValue) + parseFloat(iBookingAdjustmentValue)) / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;
					int.f.SetHTML(me.Slider.DisplayMax, bIsTimeSlider ? sTimeValue : iDisplayValue);
				}

				if (me.Slider.DisplayMaxSlider) {
					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round((parseFloat(iValue) + parseFloat(iBookingAdjustmentValue)) / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;
					var sMaxDisplayHTML = bIsTimeSlider ? sTimeValue : iDisplayValue
					if (me.Slider.DisplayPrefix) {
						sMaxDisplayHTML = me.Slider.DisplayPrefix + sMaxDisplayHTML;
					};
					if (me.Slider.DisplaySuffix) {
						sMaxDisplayHTML = sMaxDisplayHTML + me.Slider.DisplaySuffix;
					};
					int.f.SetHTML(me.Slider.DisplayMaxSlider, sMaxDisplayHTML);
				}
			}

			// set highlight
			me.SetHighlight();


			// return
			return false;

		}

		this.MouseStepMove = function (oEvent) {
			var aRange = [];

			var iMinimum = Math.floor(me.Slider.MinValue / me.Slider.StepValue) * me.Slider.StepValue;
			var iMaximum = Math.ceil(me.Slider.MaxValue / me.Slider.StepValue) * me.Slider.StepValue;

			for (let i = iMinimum; i <= iMaximum; i += me.Slider.StepValue) {
				aRange.push(i);
			}

			var iWidthPerSegment = me.Slider.Width / aRange.length;
			var iSliderDelta = me.StartLeft - me.Slider.Left;
			var iMouseDelta = me.GetMouseCoords(oEvent).x - me.MouseStartLeft;
			var iMovement = iSliderDelta + iMouseDelta;

			if (iMovement <= me.MinBound) {
				iMovement = me.MinBound;
			} else if (iMovement >= me.MaxBound) {
				iMovement = me.MaxBound;
			}

			var iMinDiff = 3000;
			var iClosestPosition = 0;
			var iSliderIndex = 0;
			for (var j = 0; j < aRange.length; j++) {
				var iDiff = Math.abs(iMovement - (j * iWidthPerSegment));
				if (iDiff < iMinDiff) {
					iMinDiff = iDiff;
					iClosestPosition = aRange[j];
					iSliderIndex = j;
				}
			}

			me.SliderControl.style.left = (iSliderIndex * iWidthPerSegment) + 'px';

			if (me.SliderControl.id.indexOf('_Start') > 0) {
				int.f.SetValue(me.Slider.ID + '_MinValue', iClosestPosition);

				var hours = (Math.floor(Math.abs(iClosestPosition) / 60));
				var minutes = (Math.floor(Math.abs(iClosestPosition) % 60));

				var sDisplayMinValue = '';

				if (hours > 0) {
					sDisplayMinValue += hours + 'hrs ';
				}
				if (minutes > 0) {
					sDisplayMinValue += minutes + 'min';
				}

				if (me.Slider.DisplayMin) {
					int.f.SetHTML(me.Slider.DisplayMin, sDisplayMinValue);
				}

				if (me.Slider.DisplayMinSlider) {

					var sMinDisplayHTML = sDisplayMinValue;
					if (me.Slider.DisplayPrefix) {
						sMinDisplayHTML = me.Slider.DisplayPrefix + sMinDisplayHTML;
					};
					if (me.Slider.DisplaySuffix) {
						sMinDisplayHTML = sMinDisplayHTML + me.Slider.DisplaySuffix;
					};
					int.f.SetHTML(me.Slider.DisplayMinSlider, sMinDisplayHTML);
				}
			} else {
				int.f.SetValue(me.Slider.ID + '_MaxValue', iClosestPosition);

				var hours = (Math.floor(Math.abs(iClosestPosition) / 60));
				var minutes = (Math.floor(Math.abs(iClosestPosition) % 60));

				var sDisplayMaxValue = '';

				if (hours > 0) {
					sDisplayMaxValue += hours + 'hrs ';
				}
				if (minutes > 0) {
					sDisplayMaxValue += minutes + 'min';
				}

				if (me.Slider.DisplayMax) {
					int.f.SetHTML(me.Slider.DisplayMax, sDisplayMaxValue);
				}

				if (me.Slider.DisplayMaxSlider) {

					var sMaxDisplayHTML = sDisplayMaxValue;
					if (me.Slider.DisplayPrefix) {
						sMaxDisplayHTML = me.Slider.DisplayPrefix + sMaxDisplayHTML;
					};
					if (me.Slider.DisplaySuffix) {
						sMaxDisplayHTML = sMaxDisplayHTML + me.Slider.DisplaySuffix;
					};
					int.f.SetHTML(me.Slider.DisplayMaxSlider, sMaxDisplayHTML);
				}
			}

			me.SetHighlight();
			return false;
		};

		//TouchStart
		this.TouchStart = function (oEvent) {

			//			//get slider and holder details
			//			me.SliderControl = int.f.GetObjectFromEvent(oEvent);
			//			me.SliderControlWidth = int.e.GetPosition(me.SliderControl).Width;
			//			int.f.SetClass(me.SliderControl, 'sliding');

			//			//work out positions
			//			me.StartLeft = int.e.GetPosition(me.SliderControl).Left;
			//			me.Width = int.e.GetPosition(me.SliderControl).Width;

			//			me.Slider = me.GetSlider(me.SliderControl);

			//			//calculate bounds
			//			me.MouseStartLeft = me.GetTouchCoords(oEvent).x;

			//			if (me.SliderControl.id.indexOf('Start') > 0) {
			//				me.MinBound = (-me.SliderControlWidth + 1) / 2;
			//				me.MaxBound = me.Slider.EndLeft - me.Slider.Left - me.SliderControlWidth;
			//			} else {
			//				me.MinBound = me.Slider.StartLeft - me.Slider.Left + me.SliderControlWidth;
			//				me.MaxBound = me.Slider.Width - me.SliderControlWidth / 2 - 1;
			//			}

			//			document.body.focus();
			//			me.SliderControl.ondragstart = function () { return false; };
			//			me.SliderControl.onselectstart = function () { return false; };
			//			me.IsSliding = true;
			//			return false;







			//stops cursor turning into 'text' rathen than a pointer
			(oEvent.preventDefault) ? oEvent.preventDefault() : oEvent.returnValue = false;


			//get slider control details
			me.SliderControl = int.f.GetObjectFromEvent(oEvent);
			me.SliderControlWidth = int.e.GetPosition(me.SliderControl).Width;


			//work out positions
			me.StartLeft = int.e.GetPosition(me.SliderControl).Left;
			me.Width = int.e.GetPosition(me.SliderControl).Width;


			//get slider
			me.Slider = me.GetSlider(me.SliderControl);


			//calculate bounds
			me.MouseStartLeft = me.GetTouchCoords(oEvent).x;

			if (me.SliderControl.id.indexOf('Start') > 0) {
				me.MinBound = (-me.SliderControlWidth + 1) / 2;
				me.MaxBound = me.Slider.EndLeft - me.Slider.Left - me.SliderControlWidth;
			}
			else {

				if (int.f.GetObject(me.Slider.ID + '_Start')) {
					me.MinBound = me.Slider.StartLeft - me.Slider.Left + me.SliderControlWidth;
				} else {
					me.MinBound = (-me.SliderControlWidth + 1) / 2;
				}

				me.MaxBound = me.Slider.Width - me.SliderControlWidth / 2 - 1;
			}

			document.body.focus();
			me.SliderControl.ondragstart = function () {
				return false;
			};
			me.SliderControl.onselectstart = function () {
				return false;
			};
			me.IsSliding = true;
			return false;
		}



		//mouseup
		this.MouseUp = function (oEvent) {



			me.IsSliding = false;

			if (me.Slider.ShowControl) int.f.Hide(me.Slider.ShowControl);

			document.body.focus();
			if (int.f.GetObject(me.Slider.ID + '_OnChange')) {
				var sOnChange = int.f.GetValue(me.Slider.ID + '_OnChange');
				eval(sOnChange);
			}

			return false;



		}

		this.TouchMove = function (oEvent) {

			//			var iPos = me.StartLeft + me.GetTouchCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;
			//			var iCalculatedPosition;
			//			if (iPos >= me.MinBound && iPos <= me.MaxBound) {
			//				iCalculatedPosition = me.StartLeft + me.GetTouchCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;
			//			} else if (iPos < me.MinBound) {
			//				iCalculatedPosition = me.MinBound;
			//			} else {
			//				iCalculatedPosition = me.MaxBound;
			//			}
			//			me.SliderControl.style.left = iCalculatedPosition + 'px';

			//			var iPercent = Math.round((iCalculatedPosition + (me.SliderControlWidth - 1) / 2) / me.Slider.Width * 100);
			//			iPercent = iPercent == 99 ? 100 : iPercent;
			//			iPercent = iPercent == 1 ? 0 : iPercent;
			//			var iValue = Math.round((this.Slider.MaxValue - this.Slider.MinValue) * iPercent / 100 + this.Slider.MinValue);

			//			if (me.Slider.ShowControl) {
			//				int.f.Show(me.Slider.ShowControl);
			//				int.f.SetHTML(me.Slider.ShowControl, Math.round(iValue * me.Slider.ExchangeRate));
			//				me.Slider.ShowControl.style.left = iCalculatedPosition - (int.e.GetPosition(me.Slider.ShowControl).Width - me.SliderControlWidth) / 2 + 'px';
			//			}

			//			if (me.SliderControl.id.indexOf('_Start') > 0) {
			//				int.f.SetValue(me.Slider.ID + '_MinValue', iValue);
			//			} else {
			//				int.f.SetValue(me.Slider.ID + '_MaxValue', iValue);
			//			}

			//			me.SetHighlight();

			//			return false;





			var bIsTimeSlider = int.f.HasClass(me.SliderControl.parentNode, 'time');
			var bLinearSlider = int.f.HasClass(me.SliderControl.parentNode, 'linear');
			var sTimeValue = '';
			var iPos = me.StartLeft + me.GetTouchCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;

			var iCalculatedPosition;
			if (iPos >= me.MinBound && iPos <= me.MaxBound) {
				iCalculatedPosition = me.StartLeft + me.GetTouchCoords(oEvent).x - me.MouseStartLeft - me.Slider.Left;
			}
			else if (iPos < me.MinBound) {
				iCalculatedPosition = me.MinBound;
			}
			else {
				iCalculatedPosition = me.MaxBound;
			}
			me.SliderControl.style.left = iCalculatedPosition + 'px';


			var iPercent = Math.round((iCalculatedPosition + (me.SliderControlWidth - 1) / 2) / me.Slider.Width * 100);
			iPercent = iPercent == 99 ? 100 : iPercent;
			iPercent = iPercent == 1 ? 0 : iPercent;

			if (bIsTimeSlider || bLinearSlider) {
				var iValue = Math.round((this.Slider.MaxValue - this.Slider.MinValue) * iPercent / 100 + this.Slider.MinValue);
			}
			else {
				var iValue = (me.log10(this.Slider.MaxValue) - me.log10(this.Slider.MinValue)) * iPercent / 100 + me.log10(this.Slider.MinValue);
				iValue = Math.round(Math.pow(10, iValue));
			}

			// if its a time slider then do it a little differently			
			if (bIsTimeSlider) {

				//work out the time value
				var iHours = int.n.SafeInt(iValue / 60);
				if (iHours < 10) iHours = '0' + iHours;
				var iMinutes = iValue % 60;
				if (iMinutes < 10) iMinutes = '0' + iMinutes;
				var sTime = iHours + ':' + iMinutes;
				sTimeValue = sTime

				if (me.Slider.ShowControl) {
					int.f.Show(me.Slider.ShowControl);
					int.f.SetHTML(me.Slider.ShowControl, sTime);
					me.Slider.ShowControl.style.left = iCalculatedPosition - (int.e.GetPosition(me.Slider.ShowControl).Width - me.SliderControlWidth) / 2 + 'px';
				}
			}
			else if (me.Slider.ShowControl) {
				int.f.Show(me.Slider.ShowControl);
				int.f.SetHTML(me.Slider.ShowControl, iValue);
				me.ShowControl.style.left = iCalculatedPosition - (int.e.GetPosition(me.Slider.ShowControl).Width - me.SliderControlWidth) / 2 + 'px';
			}

			if (me.SliderControl.id.indexOf('_Start') > 0) {
				int.f.SetValue(me.Slider.ID + '_MinValue', iValue);

				if (me.Slider.DisplayMin) {
					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round(iValue / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;
					int.f.SetHTML(me.Slider.DisplayMin, bIsTimeSlider ? sTimeValue : iDisplayValue);
				}

			}
			else {
				int.f.SetValue(me.Slider.ID + '_MaxValue', iValue);

				if (me.Slider.DisplayMax) {
					var iDisplayValue = (me.Slider.DisplayValueDivide) ? Math.round(iValue / int.f.GetValue(me.Slider.DisplayValueDivide)) : iValue;
					int.f.SetHTML(me.Slider.DisplayMax, bIsTimeSlider ? sTimeValue : iDisplayValue);
				}
			}

			// set highlight
			me.SetHighlight();


			// return
			return false;
		}




		//set highlight
		this.SetHighlight = function () {
			var oStartSlider = int.f.GetObject(me.Slider.ID + '_Start');
			var oEndSlider = int.f.GetObject(me.Slider.ID + '_End');
			var oDiv = int.f.GetObject(me.Slider.ID + '_Highlight');

			//only try if the highlight object exists
			if (oDiv) {
				var iLeft = 0;
				if (oStartSlider) {
					iLeft = oStartSlider.offsetLeft + (0.5 * oStartSlider.offsetWidth);
				}
				var iRight = oEndSlider.offsetLeft + (0.5 * oEndSlider.offsetWidth);
				oDiv.style.left = iLeft + 'px';
				oDiv.style.width = iRight - iLeft + 'px';
			};

		}



		/* slider */
		this.GetSlider = function (oSliderControl) {

			var oSlider = oSliderControl.parentNode;
			var bIsTimeSlider = int.f.HasClass(oSlider, 'time');
			var StartMinValue;
			var StartMaxValue;
			var MinValue;
			var MaxValue;
			var StepValue;
			var IsStepSlider = false;

			if (bIsTimeSlider) {
				StartMinValue = 0;
				StartMaxValue = 1439;
				MinValue = 0;
				MaxValue = 1439;
			} else {
				StartMinValue = int.n.SafeInt(int.f.GetValue(oSlider.id + '_MinValue'));
				StartMaxValue = int.n.SafeInt(int.f.GetValue(oSlider.id + '_MaxValue'));
				MinValue = int.n.SafeInt(int.f.GetValue(oSlider.id + '_AbsMinValue'));
				MaxValue = int.n.SafeInt(int.f.GetValue(oSlider.id + '_AbsMaxValue'));
				if (int.f.GetObject(oSlider.id + '_Step') && int.f.HasClass(oSlider, 'step')) {
					StepValue = int.n.SafeInt(int.f.GetValue(oSlider.id + '_Step'));
					IsStepSlider = true;
				}
			}

			var startleft = 0;
			if (int.f.GetObject(oSlider.id + '_Start')) {
				startleft = int.e.GetPosition(oSlider.id + '_Start').Left;
			};

			var oResult = {
				Control: oSlider,
				ID: oSlider.id,
				Width: int.e.GetPosition(oSlider).Width,
				Left: int.e.GetPosition(oSlider).Left,
				StartLeft: startleft,
				EndLeft: int.e.GetPosition(oSlider.id + '_End').Left,
				StartMinValue: StartMinValue,
				StartMaxValue: StartMaxValue,
				MinValue: MinValue,
				MaxValue: MaxValue,
				ExchangeRate: int.n.SafeNumeric(int.f.GetValue(oSlider.id + '_ExchangeRate')),
				ShowControl: int.f.GetObject(oSlider.id + '_Show'),
				DisplayMin: int.f.GetObject(oSlider.id + '_DisplayMin'),
				DisplayMax: int.f.GetObject(oSlider.id + '_DisplayMax'),
				DisplayMinSlider: int.f.GetObject(oSlider.id + '_DisplayMinSlider'),
				DisplayMaxSlider: int.f.GetObject(oSlider.id + '_DisplayMaxSlider'),
				DisplayValueDivide: int.f.GetObject(oSlider.id + '_DisplayValueDivide'),
				DisplayPrefix: int.f.GetValue(oSlider.id + '_DisplayPrefix'),
				DisplaySuffix: int.f.GetValue(oSlider.id + '_DisplaySuffix'),
				StepValue: StepValue,
				IsStepSlider: IsStepSlider
			};

			if (oResult.ExchangeRate == 0) oResult.ExchangeRate = 1;

			return oResult;

		}


		/* support */
		this.GetMouseCoords = function (oEvent) {

			if (oEvent.pageX || oEvent.pageY) {
				return {
					x: oEvent.pageX,
					y: oEvent.pageY
				};
			}
			else {
				return {
					x: oEvent.clientX + document.body.scrollLeft - document.body.clientLeft,
					y: oEvent.clientY + document.body.scrollTop - document.body.clientTop
				};
			}
		}

		this.GetTouchCoords = function (oEvent) {

			return {
				x: oEvent.touches[0].pageX,
				y: oEvent.touches[0].pageY
			};

		}

		this.GetMinMax = function (sID) {
			return {
				MinValue: int.n.SafeInt(int.f.GetValue(sID + '_MinValue')),
				MaxValue: int.n.SafeInt(int.f.GetValue(sID + '_MaxValue'))
			}

		}

		this.log10 = function (val) {
			return Math.log(val) / Math.log(10);
		}

	}


	//#endregion


	//#region Auto Complete
	this.AutoComplete = new function () {

		// properties
		var me = this;

		this.TextBox;
		this.Timer;

		//#region Events


		// key up
		this.AutoSuggestKeyUp = function (oEvent, oTextbox, sFunction, iTimeOut) {

			var hasEvent = oTextbox.getAttribute("EventBound");

			if (hasEvent !== "true") {
				oTextbox.setAttribute("EventBound", true);
				int.f.AttachEvent(oTextbox, "focus", function () { oTextbox.setAttribute("Selected", false); });
			}

			//set timeout
			iTimeOut = (iTimeOut == undefined || iTimeOut == null) ? 100 : iTimeOut;

			if (oEvent.keyCode) { iKeyCode = oEvent.keyCode } else { iKeyCode = oEvent.which };
			if (iKeyCode < 41 && iKeyCode != 32 && iKeyCode != 8 && iKeyCode != 0) return;

			var oDiv = int.f.GetObject(oTextbox.id + 'Options');

			if (oTextbox.value.length > 2) {
				if (this.Timer) clearTimeout(this.Timer);
				me.Textbox = oTextbox;
				me.Timer = setTimeout(sFunction, iTimeOut);
			}
			else {
				//clear the value
				me.AutoSuggestClear(oTextbox);
			};

		}


		// key down
		this.AutoSuggestKeyDown = function (oEvent, oTextbox) {

			if (oEvent.keyCode) { iKeyCode = oEvent.keyCode } else { iKeyCode = oEvent.which };
			var oDiv = document.getElementById(oTextbox.id + 'Options');
			var oHidden = document.getElementById(oTextbox.id + 'Hidden');
			var oScript = document.getElementById(oTextbox.id + 'Script');

			if (oDiv.style.display != 'none') {

				switch (iKeyCode) {
					case 38: //up arrow
						me.AutoSuggestMove(oDiv, -1);
						return;
					case 40: //down arrow
						me.AutoSuggestMove(oDiv, 1);
						return;
					case 33: //page up
						me.AutoSuggestMove(oDiv, -5);
						return;
					case 34: //page down
						me.AutoSuggestMove(oDiv, 5);
						return;
					case 27: //escape
						me.AutoSuggestHideContainer(oDiv, true);
						return;
					case 9: //tab
						//if there's only one item in the box then select it
						var iSelected = me.AutoSuggestGetCurrentSelectedIndex(oDiv);
						if (iSelected == -1) iSelected = 0;
						var iIDSelected = me.AutoSuggestGetCurrentSelectedID(oDiv);

						var sName = me.AutoSuggestGetCurrentSelectedName(oDiv);
						me.AutoCompleteSelect(oDiv.id, oTextbox.id, iIDSelected, sName);
						return;
					case 13: //enter

						//get selected option (or first if none selected)
						var iSelected = me.AutoSuggestGetCurrentSelectedIndex(oDiv);

						var iIDSelected = me.AutoSuggestGetCurrentSelectedID(oDiv);

						var sName = me.AutoSuggestGetCurrentSelectedName(oDiv);
						me.AutoCompleteSelect(oDiv.id, oTextbox.id, iIDSelected, sName);

						//Get the script from the hidden input and run it.
						var oRun = new Function(oScript.value);
						oRun();

						return false;

						if (bAutoPostback != 'True' && f.GetValue(oTextbox.id + '_Script') != '') {
							var sScript = int.f.GetValue(oTextbox.id + '_Script');
							eval(sScript);
						}

						oEvent.cancelBubble = true;
						oEvent.returnValue = false;
						return false;
				}

			}

		}

		//#endregion



		this.AutoCompleteDisplayResults = function (sJSON) {

			// display the auto complete results
			var oResult = eval('(' + sJSON + ')');

			//work out ids
			var sTextbox = oResult.TextBoxID;
			var oASTextbox = int.f.GetObject(sTextbox);
			var selected = oASTextbox.getAttribute("Selected");

			if (selected === "true") return;

			var oContainer = int.f.GetObject(oResult.TextBoxID + 'Options');
			var sDiv = oContainer.id;
			var sHidden = sTextbox + 'Hidden';

			var sResult = '';
			var sText = oASTextbox.value;

			// if the textbox value is > 2 then show results in case this returns slowly and contents have already been removed
			if (sText.length > 2) {

				if (oResult.Items.length == 0) {
					me.AutoSuggestClear(oASTextbox);
					return;
				}

				var aCaptureGroups = [];
				for (i = sText.length; i > 2; i--) {
					aCaptureGroups.push('(' + sText.substring(0, i) + ')');
				}
				var oRegEx = new RegExp(aCaptureGroups.join('|'), 'gi');

				for (var i = 0; i < oResult.Items.length; i++) {

					var sDisplay = oResult.Items[i].Display;
					var sClass = (i % 2 == 0) ? 'autoCompleteItem' : 'autoCompleteItem alternate';
					var sDisplay = sDisplay.replace(oRegEx, '<em>$&</em>');

					//replace " with HTML safe &#34; to JS doesn't error
					var sScript = 'web.AutoComplete.AutoCompleteSelect(\'' + oContainer.id + '\',\'' + sTextbox + '\',' + oResult.Items[i].Value + ',\'' + oResult.Items[i].Display.replace(/'/g, "\\'").replace(/"/g, '&#34;') + '\');' + oResult.SelectedScript;

					sResult += '<div id="' + oResult.Items[i].Value + '" class="' + sClass + '" onclick="' + sScript + '" >' + sDisplay + '</div>';

				};

				oContainer.innerHTML = sResult;
				int.f.Show(oContainer);

			}
			else {
				int.f.Hide(oContainer);
			};

			int.f.RemoveClass(oASTextbox, 'Working');

		};



		this.AutoCompleteSelect = function (sContainerID, sTextbox, iLocationID, sDisplay) {

			int.f.Hide(sContainerID);
			int.f.GetObject(sTextbox + 'Hidden').value = iLocationID;

			//Remove HTML Elements from string
			var regex = /(<([^>]+)>)/ig
			sDisplay = sDisplay.replace(regex, "");

			int.f.GetObject(sTextbox).value = sDisplay;
            int.f.GetObject(sTextbox).setAttribute("Selected", false);
            int.f.GetObject(sTextbox).focus();
		}


		//clear
		this.AutoSuggestClear = function (oTextbox, sAdditionalDropdownScripts) {
			var oHidden = int.f.GetObject(oTextbox.id + 'Hidden');
			oHidden.value = '0';

			//Hide container
			int.f.Hide(oTextbox.id + 'Options');

			if (sAdditionalDropdownScripts != undefined && sAdditionalDropdownScripts != null) {
				eval(sAdditionalDropdownScripts);
			};

		}


		//hide
		this.AutoSuggestHideContainer = function (oContainer) {
			int.f.Hide(oContainer);
		};



		// move
		this.AutoSuggestMove = function (oDiv, iNumberToMove) {
			var iItemCount = oDiv.childNodes.length - 1;
			var iCurrentSelected = me.AutoSuggestGetCurrentSelectedIndex(oDiv);
			var iTarget = iCurrentSelected + iNumberToMove;

			if (iTarget < 0) {
				iTarget = 0;
			}
			else if (iTarget > iItemCount) {
				iTarget = iItemCount;
			}

			me.AutoSuggestSetSelected(oDiv, iTarget, iCurrentSelected);
		}

		//get current selected index
		this.AutoSuggestGetCurrentSelectedIndex = function (oDiv) {
			for (var i = 0; i < oDiv.childNodes.length; i++) {
				if (int.f.HasClass(oDiv.childNodes[i], 'selected')) {
					return i;
				}
			}
			/*if we get here then we are coming from the input rather than an option in the dropdown list */
			return -1;
		}

		//get current selected ID
		this.AutoSuggestGetCurrentSelectedID = function (oDiv) {
			for (var i = 0; i < oDiv.childNodes.length; i++) {
				if (int.f.HasClass(oDiv.childNodes[i], 'selected')) {
					return oDiv.childNodes[i].id;
				}
			}
			return oDiv.childNodes[0].id;
		}

		//get current selected name
		this.AutoSuggestGetCurrentSelectedName = function (oDiv) {
			for (var i = 0; i < oDiv.childNodes.length; i++) {
				if (int.f.HasClass(oDiv.childNodes[i], 'selected')) {
					return oDiv.childNodes[i].innerText || oDiv.childNodes[i].textContent; /* FF does not support innerText */
				}
			}
			return oDiv.childNodes[0].innerText;
		}

		//set selected
		this.AutoSuggestSetSelected = function (oDiv, iSelectIndex, iDeselectIndex) {

			if (iDeselectIndex > -1) {
				int.f.RemoveClass(oDiv.childNodes[iDeselectIndex], 'selected');
			}
			int.f.AddClass(oDiv.childNodes[iSelectIndex], 'selected');

			if (oDiv.childNodes[iSelectIndex].offsetTop < oDiv.scrollTop) {
				oDiv.scrollTop = oDiv.childNodes[iSelectIndex].offsetTop;
			}
			else if (oDiv.childNodes[iSelectIndex].offsetTop > oDiv.scrollTop + 240) {
				oDiv.scrollTop = oDiv.childNodes[iSelectIndex].offsetTop - 240;
			}

		}

	}

	//#endregion


	//#region Placeholder
	//primarily for when the placeholder attribute is not supported by the browser (use IsSupported to check)
	this.Placeholder = new function () {

		this.AttachEvents = function (sObject, sPlaceholderText) {
			var oObject = int.f.SafeObject(sObject);
			var bIsSupported = web.Placeholder.IsSupported();

			if (!!oObject) {
				if (!bIsSupported) {
					if (int.f.GetValue(oObject) == '') int.f.SetValue(oObject, sPlaceholderText);

					int.f.AttachEvent(
						oObject,
						'focus',
						function () {
							web.Placeholder.Focus(oObject.id, sPlaceholderText);
						}
					);
					int.f.AttachEvent(
						oObject,
						'blur',
						function () {
							web.Placeholder.Blur(oObject.id, sPlaceholderText);
						}
					);
					oObject.placeholder = sPlaceholderText;
				}
				else {
					oObject.placeholder = sPlaceholderText;
				}
			}
		}


		//blur
		this.Blur = function (oObject, sPlaceholderText) {
			oObject = int.f.SafeObject(oObject);
			if (int.f.GetValue(oObject) == '') int.f.SetValue(oObject, sPlaceholderText);
		}


		//focus
		this.Focus = function (oObject, sPlaceholderText) {
			oObject = int.f.SafeObject(oObject);
			if (int.f.GetValue(oObject) == sPlaceholderText) int.f.SetValue(oObject, '');
		}


		//check if placeholder supported
		this.IsSupported = function () {
			var oTestInput = document.createElement('input');
			return ('placeholder' in oTestInput);
		}


		//check value does not equal placeholder value (mainly for where placeholder isn't supported)
		this.NotPlaceholderText = function (oObject) {
			oObject = int.f.SafeObject(oObject);
			return oObject.placeholder != oObject.value;
		}



	}
	//#endregion


	//#region Custom Inputs
	//used to handle the selected class of the parent label
	this.CustomInputs = new function () {

		this.ToggleRadioLabel = function (oObject, sName) {

			if (!int.f.HasClass(oObject, 'selected')) {
				oObject = int.f.SafeObject(oObject);

				//remove class from other labels containing radios with the same name
				var aRadios = [];
				if (sName != undefined && sName != '') aRadios = document.getElementsByName(sName);
				else aRadios = document.getElementsByName(oObject.firstChild.name);

				for (i = 0; i < aRadios.length; i++) {
					int.f.RemoveClass(aRadios[i].parentNode, 'selected');
				}

				//add selected class to new label
				int.f.AddClass(oObject, 'selected');
			}

		}

		this.ToggleCheckboxLabel = function (oObject) {
			//this may need some further work in future which is why it's in a function
			oObject = int.f.SafeObject(oObject);
			int.f.ToggleClass(oObject, 'selected');
		}

	}
	//#endregion


	//#region Drag Scroll
	//used to handle the selected class of the parent label

	//	this.DragScroller = function (object) {
	//		var me = this;

	//		this.StartX = 0;
	//		this.StartY = 0;
	//		this.OffsetX = 0;
	//		this.OffsetY = 0;
	//		this.DragElement;



	//		this.Setup = function (sDragElement) {
	//			this.DragElement = int.f.GetObject(sDragElement);

	//			// 2. attach events			
	//			int.f.AttachEvent(
	//					this.DragElement,
	//					'mousedown',
	//					function (event) {
	//						me.OnMouseDown(event);
	//						return false;
	//					}
	//				);

	//			int.f.AttachEvent(
	//					document,
	//					'mouseup',
	//					function (event) {
	//						me.OnMouseUp(event);
	//						return false;
	//					}
	//				);

	//		}

	//		this.OnMouseDown = function (event) {
	//			document.onmousemove = me.OnMouseMove;

	//			me.StartX = event.clientX;
	//			me.StartY = event.clientY;
	//			me.OffsetX = me.DragElement.offsetLeft;
	//			me.OffsetY = me.DragElement.offsetTop;

	//		}

	//		this.OnMouseMove = function (event) {
	//			me.DragElement.style.left = (me.OffsetX + event.clientX - me.StartX) + 'px';
	//		}

	//		this.OnMouseUp = function () {
	//			document.onmousemove = null;
	//		}



	//}
	//#endregion


	//#region Mobile Options Window
	this.MobileOptionsWindow = new function () {

		this.WindowObject;
		this.NewObject;

		this.Show = function (oObject, oCompleteFunction) {

			//make sure we have object
			oObject = int.f.GetObject(oObject);

			//show new object
			$('body').append(oObject);
			$(oObject).show();

			//animate
			$(oObject).animate({ top: '0px' }, 600, function () { $('#divAll').hide(); if (oCompleteFunction != undefined) { oCompleteFunction(); } });

		}

		this.Hide = function (oObject, oNewObject) {


			//re-show divAll if not showing new object	
			if (oNewObject == undefined) {
				$('#divAll').show();
			}


			//make sure we have object
			oObject = int.f.GetObject(oObject);


			//complete function
			var oCompleteFunction = function () {

				if (oNewObject != undefined) {
					web.MobileOptionsWindow.Show(oNewObject);
				}

				$(oObject).hide();
			}


			//hide mobile options
			$(oObject).animate({ top: '120%' }, 500, function () { oCompleteFunction(); });


		}

	}
	//#endregion


	//#region Mobile Bounce Scroll
	this.BounceScroll = new function () {

		this.Disable = function () {

			var xStart, yStart = 0;

			//capture X/Y values on start touch
			document.addEventListener('touchstart', function (e) {
				xStart = e.touches[0].screenX;
				yStart = e.touches[0].screenY;
			});

			//capture new X/Y and compare to original
			document.addEventListener('touchmove', function (e) {
				var xMovement = Math.abs(e.touches[0].screenX - xStart);
				var yMovement = Math.abs(e.touches[0].screenY - yStart);
				if ((yMovement * 3) > xMovement) {
					e.preventDefault();
				}
			});

		}

		//		this.Enable = function () {

		//			int.f.DetachEvent('')

		//		}

	}
	//#endregion


	//#region Mobile Zoom
	this.MobileZoom = new function () {

		this.DisableZoom = function () {
			var oMetaTag = int.f.GetObject('viewport-meta');
			oMetaTag.setAttribute('content', 'user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1, width=device-width, height=device-height, target-densitydpi=device-dpi')
		}

		this.EnableZoom = function () {
			var oMetaTag = int.f.GetObject('viewport-meta');
			oMetaTag.setAttribute('content', 'user-scalable=yes, initial-scale=1, maximum-scale=2, minimum-scale=1, width=device-width, height=device-height, target-densitydpi=device-dpi')
		}

	}
	//#endregion


	//#region Get Postition
	this.getPosition = function (oElement) {

		var iReturnLeftValue = 0;
		var iReturnTopValue = 0;

		var oElementLeft = oElement;
		var oElementTop = oElement;

		//set top
		while (oElementLeft != null) {
			iReturnTopValue += oElementLeft.offsetTop;
			oElementLeft = oElementLeft.offsetParent;
		}

		//set left
		while (oElementTop != null) {
			iReturnLeftValue += oElementTop.offsetLeft;
			oElementTop = oElementTop.offsetParent;
		}

		var offset = {
			left: 0,
			top: 0
		};
		offset.left = int.n.SafeNumeric(iReturnLeftValue);
		offset.top = int.n.SafeNumeric(iReturnTopValue);

		return offset;

	}
	//#endregion


	//#region Lightbox Image Gallery
	this.LightboxImageGallery = function (object) {

		var me = this;

		this.Object = object;

		this.Container;
		this.CurrentImage;

		this.ImageNavLeft;
		this.ImageNavRight;

		this.ThumbnailContainer;
		this.ThumbnailHolder;
		this.ThumbnailNavLeft;
		this.ThumbnailNavRight;

		this.AllThumbnails;
		this.ThumbnailCount;
		this.ThumbnailWidth;

		this.ModalPopup;

		this.TransitionClass;
		this.TransitionTime;
		this.DoNotCenterPopup;

		this.Setup = function (oParams) {

			//save gallery parameters
			me.Container = int.f.GetObject(oParams.container);
			me.CurrentImage = int.f.GetObject(oParams.currentImage);

			me.DoNotCenterPopup = oParams.doNotPositionPopup;

			if (me.CurrentImage) {

				me.ImageNavLeft = int.f.GetObject(oParams.imageNavLeft);
				me.ImageNavRight = int.f.GetObject(oParams.imageNavRight);

				me.ThumbnailContainer = int.f.GetObject(oParams.thumbnailContainer);
				me.ThumbnailHolder = int.f.GetObject(oParams.thumbnailHolder);
				me.ThumbnailNavLeft = int.f.GetObject(oParams.thumbnailNavLeft);
				me.ThumbnailNavRight = int.f.GetObject(oParams.thumbnailNavRight);

				me.AllThumbnails = int.f.GetElementsByClassName('img', '', me.ThumbnailContainer);
				me.ThumbnailCount = me.AllThumbnails.length;
				me.ThumbnailWidth = int.f.GetObject(oParams.thumbnailPrefix + '1').offsetWidth;

				me.ModalPopup = int.f.SafeObject(oParams.modalPopup);

				me.TransitionClass = oParams.transitionClass;
				me.TransitionTime = oParams.transitionTime;

				//fix missing <ie9 array indexof
				IEArrayIndexOfFix();

				//set container initial dimensions to first image width
				var oMaxDimensions = me.MaxDimensions(me.CurrentImage);
				me.Container.style.width = oMaxDimensions.width + 'px';
				me.Container.style.height = oMaxDimensions.height + 'px';

				//set initial width of thumbnail container
				me.CentreThumbnails(int.f.GetObject(oParams.thumbnailPrefix + '1'), oMaxDimensions.width);

				//centre popup
				if (!me.DoNotCenterPopup) {
				me.CentreImagePopup(oMaxDimensions.width, oMaxDimensions.height);
				}
				//add transition class to popup
				setTimeout(
					function () {
						int.f.AddClass(me.ModalPopup, me.TransitionClass);
					}, me.TransitionTime
				);

			}
		}

		this.ChangeImage = function (oNewImage) {

			//get image dimensions
			var oNewDimensions = me.MaxDimensions(oNewImage);

			//set width of container
			me.Container.style.width = oNewDimensions.width + 'px';
			me.Container.style.height = oNewDimensions.height + 'px';

			//centre image popup
			if (!me.DoNotCenterPopup) {
			me.CentreImagePopup(oNewDimensions.width, oNewDimensions.height);
			}

			//set current image attributes to new image attributes
			me.CurrentImage.src = oNewImage.src;
			me.CurrentImage.alt = oNewImage.alt;

			//show/hide nav buttons if we're at either end
			var iImagePosition = me.AllThumbnails.indexOf(oNewImage) + 1;
			int.f.ShowIf(me.ImageNavLeft, iImagePosition > 1);
			int.f.ShowIf(me.ImageNavRight, iImagePosition < me.ThumbnailCount);

			//work out how many thumbnails to show
			me.CentreThumbnails(oNewImage, oNewDimensions.width);

			//remove selected class from all thumbnails
			for (i = 0; i < me.AllThumbnails.length; i++) {
				int.f.RemoveClass(me.AllThumbnails[i], 'selected');
			}

			//add selected class to new thumbnail
			int.f.AddClass(oNewImage, 'selected');

		}

		this.ImageNav = function (sDirection) {

			//get current thumbnail position
			var oCurrentThumbnail = int.f.GetElementsByClassName('img', 'selected', me.ThumbnailContainer)[0];
			var iCurrentPosition = me.AllThumbnails.indexOf(oCurrentThumbnail) + 1;

			//get new position
			var iNewPosition = iCurrentPosition;
			switch (sDirection) {
				case 'left':
					iNewPosition--;
					break;
				case 'right':
					iNewPosition++;
					break;
				default:
					return false;
			}

			//change image
			me.ChangeImage(me.AllThumbnails[iNewPosition - 1]);

		}

		this.ThumbnailNav = function (sDirection) {

			//get holder left
			var iThumbnailHolderLeft = parseInt(me.ThumbnailHolder.style.left);

			//get width of one thumbnail page
			var oMaxDimensions = me.MaxDimensions(me.CurrentImage);
			var iVisibleThumbnails = Math.floor((oMaxDimensions.width - 100) / me.ThumbnailWidth);
			var iThumbnailPageWidth = iVisibleThumbnails * me.ThumbnailWidth;

			//calculate new left position
			switch (sDirection) {
				case 'left':
					iThumbnailHolderLeft = Math.abs(iThumbnailHolderLeft) - iThumbnailPageWidth;
					break;
				case 'right':
					iThumbnailHolderLeft = Math.abs(iThumbnailHolderLeft) + iThumbnailPageWidth;
					break;
				default:
					return false;
			}

			//modify new left pos if we've gone over the end and hide nav buttons if we've reached the end
			if (sDirection == 'left' && iThumbnailHolderLeft < 0) {
				iThumbnailHolderLeft = 0;
				int.f.Hide(me.ThumbnailNavLeft);
				int.f.Show(me.ThumbnailNavRight);
			}
			else if (sDirection == 'right' && iThumbnailHolderLeft >= ((me.ThumbnailCount - iVisibleThumbnails) * me.ThumbnailWidth)) {
				iThumbnailHolderLeft = (me.ThumbnailCount - iVisibleThumbnails) * me.ThumbnailWidth;
				int.f.Hide(me.ThumbnailNavRight);
				int.f.Show(me.ThumbnailNavLeft);
			}
			else {
				int.f.Show(me.ThumbnailNavLeft);
				int.f.Show(me.ThumbnailNavRight);
			}

			//set new position
			me.ThumbnailHolder.style.left = -iThumbnailHolderLeft + 'px';

		}

		this.CentreThumbnails = function (oNewThumbnail, iImageWidth) {

			//calculate width of visible thumbnails
			var iVisibleThumbnails = Math.floor((iImageWidth - 100) / me.ThumbnailWidth);
			var iThumbnailContainerWidth = (((iVisibleThumbnails * me.ThumbnailWidth) > me.ThumbnailCount * me.ThumbnailWidth) ? me.ThumbnailCount * me.ThumbnailWidth : iVisibleThumbnails * me.ThumbnailWidth);

			//check if new thumbnail is visible
			var iThumbnailLeft = oNewThumbnail.offsetLeft;
			var iThumbnailPosition = (iThumbnailLeft / me.ThumbnailWidth) + 1;

			if (iThumbnailLeft >= (iThumbnailContainerWidth - me.ThumbnailWidth)) {
				//make sure we don't go over the end of the thumbnails
				if (iThumbnailPosition > (me.ThumbnailCount - iVisibleThumbnails)) {
					iThumbnailLeft = (me.ThumbnailCount - iVisibleThumbnails) * me.ThumbnailWidth;
				}
				me.ThumbnailHolder.style.left = -iThumbnailLeft + 'px';
			}
			else {
				me.ThumbnailHolder.style.left = '0px';
			}

			//hide/show thumbnail nav buttons
			int.f.ShowIf(me.ThumbnailNavLeft, (iThumbnailContainerWidth < (me.ThumbnailCount * me.ThumbnailWidth)) && iThumbnailPosition >= iVisibleThumbnails);
			int.f.ShowIf(me.ThumbnailNavRight, (iThumbnailContainerWidth < (me.ThumbnailCount * me.ThumbnailWidth)) && iThumbnailPosition <= (me.ThumbnailCount - iVisibleThumbnails));

			//set width and left margin to keep thumbnails centred
			me.ThumbnailContainer.style.width = iThumbnailContainerWidth + 'px';
			me.ThumbnailContainer.style.marginLeft = -(iThumbnailContainerWidth / 2) + 'px';

		}

		this.CentreImagePopup = function (iNewWidth, iNewHeight) {

			//set position
			var iScrollTop = ($(window).scrollTop() != 0) ? $(window).scrollTop() : document.body.scrollTop;
			var iScrollLeft = ($(window).scrollLeft() != 0) ? $(window).scrollLeft() : document.body.scrollLeft;

			var iMidHeight = $(window).height() / 2;
			var iPopupMidHeight = iNewHeight / 2;

			//check if popup height is bigger than window height, if so adjust acordingly
			if (me.ModalPopup.clientHeight > $(window).height()) {
				iMidHeight = iMidHeight + ((iNewHeight - $(window).height()) / 2);
			}

			var iMidWidth = $(window).width() / 2;
			var iPopupMidWidth = iNewWidth / 2;

			me.ModalPopup.style.left = iScrollLeft + (iMidWidth - iPopupMidWidth) + 'px';
			me.ModalPopup.style.top = iScrollTop + (iMidHeight - iPopupMidHeight) + 'px';

		}

		this.MaxDimensions = function (oImage) {

			//get original dimensions
			var oNaturalDimensions = me.NaturalDimensions(oImage);
			var iWidth = oNaturalDimensions.width;
			var iHeight = oNaturalDimensions.height;

			//check if image is too big for the window and adjust sizes if so
			var dRatio = 1;
			if (iWidth >= iHeight) {
				if (iWidth > (window.innerWidth - 120)) {
					dRatio = ((window.innerWidth - 120) / iWidth).toFixed(2);
				}
				else if (iHeight > (window.innerHeight - 120)) {
					dRatio = ((window.innerHeight - 120) / iHeight).toFixed(2);
				}
			}
			else {
				if (iHeight > (window.innerHeight - 120)) {
					dRatio = ((window.innerHeight - 120) / iHeight).toFixed(2);
				}
				else if (iWidth > (window.innerWidth - 120)) {
					dRatio = ((window.innerWidth - 120) / iWidth).toFixed(2);
				}
			}

			//apply ratio
			iWidth = iWidth * dRatio;
			iHeight = iHeight * dRatio;

			//create object
			var oDimensions = {
				width: iWidth,
				height: iHeight
			};

			return oDimensions;
		}

		this.NaturalDimensions = function (oImage) {

			var oImg = document.createElement('img');

			oImg.onload = function () {
				var iImgWidth = this.width;
				var iImgHeight = this.height;
			}

			oImg.src = oImage.src;

			var oDimensions = {
				width: oImg.width,
				height: oImg.height
			};

			return oDimensions;

		}

	}
	//#endregion


	//#region Fading Image Gallery
	this.ImageFader = function () {

		this.FaderObject;
		this.FaderName;

		this.ImageCount;

		this.ValidControls = ['Arrows', 'Numbers'];

		this.HoldTime;
		this.Controls = [];
		this.AutoFade;

		this.Fading = false;
		this.Timeout;

		this.Setup = function (o) {

			//parse JSON object
			o = JSON.parse(o);

			//make sure container exists
			if (!int.f.GetObject('div' + o.FaderName)) return false;

			//setup params
			this.FaderObject = int.f.GetObject('div' + o.FaderName);
			this.FaderName = o.FaderName;
			this.HoldTime = (o.HoldTime % 1 === 0) ? o.HoldTime : 5000; // ensure int value

			//for each control passed, check it's valid and push to array
			if (o.Controls.constructor === Array) {
				var iLen = o.Controls.length;
				for (i = 0; i < iLen; i++) {
					if (this.ValidControls.indexOf(o.Controls[i]) > -1) this.Controls.push(o.Controls[i]);
				}
			} else if (typeof (o.Controls) === 'string' && this.ValidControls.indexOf(o.Controls) > -1) {
				this.Controls.push(o.Controls);
			} else {
				this.Controls.push('Numbers');
			}

			//check if autofdae param is boolean or string and then set accordingly
			if (typeof (o.AutoFade) === 'boolean') {
				this.AutoFade = o.AutoFade
			} else if (['true', 'false'].indexOf(o.AutoFade.toLowerCase()) > -1) {
				this.AutoFade = o.AutoFade.toLowerCase() === 'true';
			} else {
				this.AutoFade = true;
			}

			//give IDs to images and set current class on first
			var aImages = int.f.GetElementsByClassName('div', 'item', this.FaderObject);
			var iImages = aImages.length;
			for (i = 0; i < iImages; i++) {
				if (i == 0) int.f.AddClass(aImages[i], 'first current');
				aImages[i].id = 'div' + this.FaderName + '_Item_' + (i + 1);
			}

			//save image count
			this.ImageCount = iImages;

			//build controls
			this.BuildControls();

			//begin fading if setting is true and number of images is greater than 1
			if (this.AutoFade && iImages > 1) {
				(function (fader) {
					fader.Timeout = setTimeout(function () {
						fader.FadeImage(1, 2);
					}, fader.HoldTime);
				})(this);
			}

		}

		this.FadeImage = function (iCurrent, iNew) {

			//make sure we're workign with ints
			iCurrent = parseInt(iCurrent);
			iNew = parseInt(iNew);

			//bomb out if new is same as current
			if (iCurrent == iNew) return false;

			//clear timeout
			clearTimeout(this.Timeout);

			//get current and new images
			var oCurrentImage = int.f.GetObject('div' + this.FaderName + '_Item_' + iCurrent);
			var oNewImage = int.f.GetObject('div' + this.FaderName + '_Item_' + iNew);

			//only fade if new image exists and we aren't already fading
			if (oNewImage && !this.Fading) {
				this.Fading = true;

				//switch class to fade images
				int.f.RemoveClass(oCurrentImage, 'current');
				int.f.AddClass(oNewImage, 'current');

				//select control if we have any built
				if (int.f.GetObject('div' + this.FaderName + '_NavContainer')) {
					this.SelectControl(iNew);
				}

				//reset fading property once transition is over
				(function (fader) {
					int.e.TransitionEnd(oNewImage, function () {
						fader.Fading = false;
					});
				})(this);

				//create timeout for next fade
				(function (fader) {
					fader.Timeout = setTimeout(function () {
						var oNextImage = int.f.GetObject('div' + fader.FaderName + '_Item_' + (iNew + 1));
						var iNextImage = (oNextImage) ? (iNew + 1) : 1;
						fader.Fading = false;
						fader.FadeImage(iNew, iNextImage);
					}, fader.HoldTime);
				})(this);

			}

		}

		this.SelectControl = function (iNew) {

			//remove current class from controls
			var aNavElements = int.f.GetObjectsByIDPrefix('a' + this.FaderName + '_NavItem_', 'a', 'ul' + this.FaderName + '_Nav');
			var iNavElements = aNavElements.length;
			for (var i = 0; i < iNavElements; i++) {
				int.f.RemoveClass(aNavElements[i], 'current');
			}

			//add current class to new image control
			int.f.AddClass('a' + this.FaderName + '_NavItem_' + iNew, 'current');

			//mobile controls
			var iFinalID = this.GetFinalID();
			if (iNew == 1) {
				int.f.AddClass('a' + this.FaderName + '_Left', 'hide');
				int.f.RemoveClass('a' + this.FaderName + '_Right', 'hide');
			}
			else if (iNew == iFinalID) {
				int.f.AddClass('a' + this.FaderName + '_Right', 'hide');
				int.f.RemoveClass('a' + this.FaderName + '_Left', 'hide');
			}

		}

		this.BuildControls = function () {

			//get number of control types
			var iControls = this.Controls.length;

			//build controls if we've specified that we want some and if there is more than one image
			if (iControls > 0 && this.ImageCount > 1) {

				//create nav container
				var oNav = document.createElement('div');
				oNav.id = 'div' + this.FaderName + '_NavContainer';
				oNav.className = 'navigation';
				this.FaderObject.appendChild(oNav);

				//call build function for each type of control
				for (i = 0; i < iControls; i++) {
					this['Build' + this.Controls[i]]();
				}

			}

		}

		this.BuildArrows = function () {

			//build left arrow
			var oLeft = document.createElement('a');
			oLeft.id = 'a' + this.FaderName + '_Left';
			oLeft.className = 'arrowNav left hide';
			oLeft.href = 'javascript:void(0)';
			oLeft.setAttribute('title', 'Previous');

			(function (fader) {
				int.f.AttachEvent(oLeft, 'click', function () {
					fader.Left(fader);
				});
			})(this);

			//build right arrow
			var oRight = document.createElement('a');
			oRight.id = 'a' + this.FaderName + '_Right';
			oRight.className = 'arrowNav right';
			oRight.href = 'javascript:void(0)';
			oRight.setAttribute('title', 'Next');

			//add click & touch event
			(function (fader) {
				int.f.AttachEvent(oRight, 'click', function () {
					fader.Right(fader);
				});
				int.f.AttachEvent(oRight, 'touchend', function () {
					fader.Right(fader);
				});
			})(this);

			//add to fader
			int.f.GetObject('div' + this.FaderName + '_NavContainer').appendChild(oLeft);
			int.f.GetObject('div' + this.FaderName + '_NavContainer').appendChild(oRight);

		}

		this.BuildNumbers = function () {

			//build list
			var oList = document.createElement('ul');
			oList.id = 'ul' + this.FaderName + '_Nav';
			oList.className = 'numbers clear';

			//build list items
			var aItems = int.f.GetElementsByClassName('div', 'item', this.FaderObject);
			var iItems = aItems.length;
			for (i = 0; i < iItems; i++) {
				var oListItem = document.createElement('li');
				oListItem.className = 'clear';

				//build links
				var oLink = document.createElement('a');
				oLink.id = 'a' + this.FaderName + '_NavItem_' + (i + 1);
				oLink.href = 'javascript:void(0)';
				oLink.innerHTML = (i + 1);
				if (i == 0) oLink.className = 'current';

				//add click & touch events
				(function (fader) {
					int.f.AttachEvent(oLink, 'click', function () {
						var iCurrentID = int.f.GetElementsByClassName('a', 'current', oList)[0].id.replace('a' + fader.FaderName + '_NavItem_', '');
						fader.FadeImage(iCurrentID, this.id.replace('a' + fader.FaderName + '_NavItem_', ''));
					});
					int.f.AttachEvent(oLink, 'touchend', function () {
						var iCurrentID = int.f.GetElementsByClassName('a', 'current', oList)[0].id.split(this.FaderName + '_NavItem_')[0];
						fader.FadeImage(iCurrentID, this.id.replace('a' + fader.FaderName + '_NavItem_', ''));
					});
				})(this);

				oListItem.appendChild(oLink);
				oList.appendChild(oListItem);
			}

			int.f.GetObject('div' + this.FaderName + '_NavContainer').appendChild(oList);
		}

		this.Left = function (fader) {
			if (!fader.Fading) {
				var iCurrentID = fader.GetCurrentID();

				//hide left arrow if we're at the end
				if (iCurrentID == 2) {
					int.f.AddClass('a' + this.Fader + '_Left', 'hide');
				}

				//make sure we're showing right arrow
				int.f.RemoveClass('a' + fader.Fader + '_Right', 'hide');

				//fade
				fader.FadeImage(iCurrentID, (iCurrentID - 1));
			}
		}

		this.Right = function (fader) {
			if (!fader.Fading) {
				var iCurrentID = fader.GetCurrentID();
				var iFinalID = fader.GetFinalID();

				//hide right button if we're at the end
				if (iCurrentID == iFinalID - 1) {
					int.f.AddClass('a' + fader.Fader + '_Right', 'hide');
				}

				//make sure we're showing left arrow
				int.f.RemoveClass('a' + fader.Fader + '_Left', 'hide');

				//fade
				fader.FadeImage(iCurrentID, (iCurrentID + 1));
			}

		}

		this.GetCurrentID = function () {
			var oCurrentImage = int.f.GetElementsByClassName('div', 'current', this.FaderObject)[0];
			var iCurrentID = parseInt(oCurrentImage.id.split('_')[3]);
			return iCurrentID;
		}

		this.GetFinalID = function () {
			var oImages = int.f.GetElementsByClassName('div', 'item', this.FaderObject);
			var iFinalID = parseInt(oImages[oImages.length - 1].id.split('_')[3]);
			return iFinalID;
		}

	}
	//#endregion

}
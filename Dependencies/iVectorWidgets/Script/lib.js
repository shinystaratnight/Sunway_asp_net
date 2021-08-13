var int = new function () {
    this.a = new ArrayFunctions();
    this.d = new DateFunctions();
    this.f = new FormFunctions();
    this.n = new NumberFunctions();
    this.v = new Validators();
    this.s = new StringFunctions();
    this.dd = new DropdownFunctions();
    this.cb = new CheckBoxFunctions();
    this.e = new Effects();
    this.dl = new DataListFunctions();
    this.c = new CookieFunctions();
    this.b = new BrowserFunctions();
    this.ll = new LazyLoadFunctions;
    this.rb = new RadioButtonFunctions();

    //#region array functions
    function ArrayFunctions() {
        this.IsArray = function (o) {
            return Object.prototype.toString.call(o) === '[object Array]'
        }

        this.ArrayContains = function (oArray, oValue) {
            if (int.a.IsArray(oArray)) {
                for (var i = 0; i < oArray.length; i++) {
                    if (oArray[i] == oValue) return true;
                }
            }
            return false;
        }

        this.MaxArrayValue = function (oArray) {
            return Math.max.apply(Math, oArray);
        }

        this.MinArrayValue = function (oArray) {
            return Math.min.apply(Math, oArray);
        }
    }

    //#endregion

    //#region date functions
    function DateFunctions() {
        this.New = function (iDay, iMonth, iYear) {
            return new Date(iYear, iMonth - 1, iDay);
        }

        this.Today = function () {
            var dToday = new Date();
            return int.d.New(int.d.Day(dToday), int.d.Month(dToday), int.d.Year(dToday));
        }

        this.GetDateOnly = function (dDate) {
            return int.d.New(int.d.Day(dDate), int.d.Month(dDate), int.d.Year(dDate));
        }

        this.AddDays = function (dDate, iDays) {
            dDate.setDate(dDate.getDate() + int.n.SafeInt(iDays));
            return dDate;
        }

        this.Year = function (dDate) {
            return dDate.getFullYear();
        }

        this.Month = function (dDate) {
            return dDate.getMonth() + 1;
        }

        this.Day = function (dDate) {
            return dDate.getDate();
        }

        this.DayName = function (dDate) {
            return int.s.Left(dDate + '', 3);
        }

        this.MonthName = function (dDate) {
            var aMonths = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
            return aMonths[int.d.Month(dDate) - 1];
        }

        this.MonthEnd = function (dDate) {
            //create new date 01/month+1/year
            return int.d.AddDays(int.d.New(1, (int.d.Month(dDate) == 12) ? 1 : int.d.Month(dDate) + 1,
			(int.d.Month(dDate) == 12) ? int.d.Year(dDate) + 1 : int.d.Year(dDate)), -1);
        }

        this.Weekend = function (dDate) {
            return (int.s.Left(dDate + '', 1) == 'S');
        }

        this.IsDate = function (oDate) {
            return !isNaN(new Date(oDate));
        }

        this.SafeDate = function (oDate) {
            if (this.IsDate(oDate)) return new Date(oDate);
        }

        this.FromSQLDate = function (sDate) {
            return int.d.New(sDate.substring(8, 10), sDate.substring(5, 7), sDate.substring(0, 4));
        }
        this.ToSQLDate = function (dDate) {
            var sYear = dDate.getFullYear().toString();
            var sMonth = int.s.PadWithZeros((dDate.getMonth() + 1).toString(), 2);
            var sDay = int.s.PadWithZeros(dDate.getDate().toString(), 2);
            var sHours = int.s.PadWithZeros(dDate.getHours().toString(), 2);
            var sMinutes = int.s.PadWithZeros(dDate.getMinutes().toString(), 2);
            var sSeconds = int.s.PadWithZeros(dDate.getSeconds().toString(), 2);
            return sYear + '-' + sMonth + '-' + sDay + 'T' + sHours + ':' + sMinutes + ':' + sSeconds;
        }

        this.DisplayDate = function (dDate) {
            dDate = new Date(dDate)
            var aMonths = new Array('Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec')
            var sDay = dDate.getDate().toString()
            if (sDay.length == 1) sDay = '0' + sDay;
            return sDay + ' ' + aMonths[dDate.getMonth()] + ' ' + dDate.getFullYear();
        }

        this.GetAge = function (dDateOfBirth) {
            var dNow = new Date();
            var iAge = -1;
            while (dNow >= dDateOfBirth) {
                iAge++;
                dDateOfBirth.setFullYear(dDateOfBirth.getFullYear() + 1);
            }
            return iAge;
        }

        this.DateDiff = function (sStartDate, sEndDate) {
            var dStartDate = new Date(sStartDate);
            var dEndDate = new Date(sEndDate);
            var iStartYear;
            var iEndYear;
            var iStartDayOfYear;
            var iEndDayOfYear;
            var iDiff;

            //get the years and day of years, if end date is before start date then swap them round
            if (dStartDate <= dEndDate) {
                iStartYear = dStartDate.getYear();
                iEndYear = dEndDate.getYear();
                iStartDayOfYear = this.DayOfYear(dStartDate);
                iEndDayOfYear = this.DayOfYear(dEndDate);
            }
            else {
                iStartYear = dEndDate.getYear();
                iEndYear = dStartDate.getYear();
                iStartDayOfYear = this.DayOfYear(dEndDate);
                iEndDayOfYear = this.DayOfYear(dStartDate);
            }

            //2 possibilities, same year, different years
            if (iStartYear == iEndYear) {
                iDiff = iEndDayOfYear - iStartDayOfYear;
            }
            else {
                //one or more years apart starts with same calculation
                iDiff = iEndDayOfYear + (365 - iStartDayOfYear);

                //if it's a leap year and next year is different then add
                if (this.CheckLeapYear(iStartYear) == 1 && iEndYear != iStartYear) {
                    iDiff += 1;
                }

                //now loop through all (if any years inbetween)
                for (var iLoop = iStartYear + 1; iLoop < iEndYear; iLoop++) {
                    //add 365 for a normal year, 366 for a leap year
                    iDiff += this.CheckLeapYear(iLoop) == 1 ? 366 : 365;
                }
            }

            // if start date > end date invert the difference
            if (dStartDate > dEndDate) iDiff = iDiff * (-1);

            return iDiff;
        }

        this.CheckLeapYear = function (iYear) {
            return (((iYear % 4 == 0) && (iYear % 100 != 0)) || (iYear % 400 == 0)) ? 1 : 0;
        }

        this.DayOfYear = function (dDate) {
            //start with current day of month and then add on preivous mointh days
            var iDayOfYear = dDate.getDate();
            var iMonth = dDate.getMonth();
            var iYear = dDate.getYear();

            //if it's a leap year and we are past Februrary then add 1
            if ((this.CheckLeapYear(iYear) == 1) && (iMonth >= 2)) {
                iDayOfYear++;
            }

            //now do a huge ugly if statement adding the rest on for the months
            if (iMonth == 1) {
                iDayOfYear += 31;
            }
            else if (iMonth == 2) {
                iDayOfYear += 59;
            }
            else if (iMonth == 3) {
                iDayOfYear += 90;
            }
            else if (iMonth == 4) {
                iDayOfYear += 120;
            }
            else if (iMonth == 5) {
                iDayOfYear += 151;
            }
            else if (iMonth == 6) {
                iDayOfYear += 181;
            }
            else if (iMonth == 7) {
                iDayOfYear += 212;
            }
            else if (iMonth == 8) {
                iDayOfYear += 243;
            }
            else if (iMonth == 9) {
                iDayOfYear += 273;
            }
            else if (iMonth == 10) {
                iDayOfYear += 304;
            }
            else if (iMonth == 11) {
                iDayOfYear += 334;
            }

            return iDayOfYear;
        }

        this.TimeZoneSafeDate = function (dDate) {
            dDate.setHours(dDate.getHours() - dDate.getTimezoneOffset() / 60);
        }

        //returns a date in the format DD/MM/YYYY with zeros included - used for datepicker
        this.DatepickerDate = function (dDate) {
            var sDay = ((dDate.getDate().toString().length == 1) ? '0' + dDate.getDate().toString() : dDate.getDate().toString());
            var sMonth = (((dDate.getMonth() + 1).toString().length == 1) ? '0' + (dDate.getMonth() + 1).toString() : (dDate.getMonth() + 1).toString());
            var sYear = dDate.getFullYear();

            return sDay + '/' + sMonth + '/' + sYear;
        }
    }

    //#endregion

    //#region number functions
    function NumberFunctions() {
        this.SafeInt = function (sInteger) {
            if ((sInteger == null) || (sInteger == '') || (sInteger == '0') || isNaN(parseFloat(sInteger))) {
                return 0;
            }
            else {
                //remove any commas
                sInteger += '';
                var aInt = sInteger.split(",");
                var sTotal = '';
                for (var loop = 0; loop < aInt.length; loop++) {
                    sTotal += aInt[loop];
                }
                return parseInt(parseFloat(sTotal), 10);
            }
        }

        this.SafeNumeric = function (sNumber) {
            if (sNumber == null || sNumber == '' || sNumber == '0' || isNaN(parseFloat(sNumber))) {
                return 0;
            }
            else {
                //remove any commas
                sNumber += '';
                return parseFloat(sNumber.replace(',', ''));
            }
        }

        this.Cent = function (nNumber) {
            // returns the amount in the .99 format
            return (nNumber == Math.floor(nNumber)) ? nNumber + '.00' : ((nNumber * 10 == Math.floor(nNumber * 10)) ? nNumber + '0' : nNumber);
        }

        this.Round = function (nNumber, X) {
            // rounds number to X decimal places, defaults to 2
            X = (!X ? 2 : X);
            return Math.round(nNumber * Math.pow(10, X)) / Math.pow(10, X);
        }

        this.FormatMoney = function (nNumber, sCurrency, sPosition) {
            sPosition = (sPosition == undefined) ? 'Prepend' : sPosition;
            //get the rounded figure
            var nRounded = int.n.Cent(int.n.Round(nNumber));
            if (sCurrency != undefined) {
                if (nRounded < 0) {
                    nRounded = int.n.Cent(nRounded * (-1));
                    return '-' + sCurrency + nRounded;
                }
                else {
                    return sPosition == 'Append' ? nRounded + sCurrency : sCurrency + nRounded;
                }
            }
            else {
                return nRounded;
            }
        }

        this.FormatNumber = function (o, iDecimalPlaces) {
            o = int.n.SafeNumeric(o);
            return o.toFixed(iDecimalPlaces == undefined ? 2 : iDecimalPlaces);
        }

        this.FormatCommas = function (nStr) {
            nStr += '';
            x = nStr.split('.');
            x1 = x[0];
            x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }
    }

    //#endregion

    //#region string functions
    function StringFunctions() {
        this.Left = function (s, i) {
            return s.substring(0, i);
        }

        this.Right = function (s, i) {
            return s.substring(s.length - i);
        }

        this.Chop = function (sString, i) {
            i = i | 1;
            return int.s.Substring(sString, 0, sString.length - i);
        }

        this.Substring = function (s, iStart, iLength) {
            return iLength == undefined ? s.substring(iStart) : s.substring(iStart, iLength);
        }

        this.Slice = function (s, iStart, iEnd) {
            iEnd = iEnd | iStart;
            return s.substring(iStart, iStart + (iEnd - iStart) + 1);
        }

        this.StartsWith = function (sBase, sCompare) {
            return sBase.indexOf(sCompare) == 0;
        }

        this.EndsWith = function (sBase, sCompare) {
            return sBase.lastIndexOf(sCompare) == sBase.length - sCompare.length;
        }

        this.Replace = function (sString, sStringToReplace, sReplacement) {
            while (sString.indexOf(sStringToReplace) != -1) {
                sString = sString.replace(sStringToReplace, sReplacement);
            }
            return sString;
        }

        this.Trim = function (sBase) {
            return sBase.replace(/^\s*|\s*$/g, '');
        }

        this.ArrayToCSV = function (aArray) {
            var sReturn = '';
            if (aArray.constructor.toString().indexOf('Array') > 0) {
                for (var i = 0; i < aArray.length; i++) {
                    sReturn += aArray[i].toString() + (i < aArray.length - 1 ? ',' : '');
                }
            }
            return sReturn;
        }

        this.PadWithZeros = function (sBase, iLength) {
            return sBase.length < iLength ? '0' * (iLength - sBase.length) + sBase : sBase;
        }

        this.EncodeString = function (sString) {
            return encodeURIComponent(sString);
        }

        this.Format = function (sString) {
            if (arguments.length <= 1) return sString;
            var tokenCount = arguments.length - 2;
            for (var token = 0; token <= tokenCount; token++) {
                sString = sString.replace(new RegExp('\\{' + token + '\\}', 'gi'), arguments[token + 1]);
            }
            return sString;
        }
    }

    //#endregion

    //#region form functions
    function FormFunctions() {

        this.getParameterByName = function (name, url) {
	        if (!url) url = window.location.href;
	        name = name.replace(/[\[\]]/g, "\\$&");
	        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
	        if (!results) return null;
	        if (!results[2]) return '';
	        return decodeURIComponent(results[2].replace(/\+/g, " "));
	    }

        this.SelectText = function (el, win) {
            win = win || window;
            var doc = win.document, sel, range;
            if (win.getSelection && doc.createRange) {
                sel = win.getSelection();
                range = doc.createRange();
                range.selectNodeContents(el);
                sel.removeAllRanges();
                sel.addRange(range);
            } else if (doc.body.createTextRange) {
                range = doc.body.createTextRange();
                range.moveToElementText(el);
                range.select();
            }
        }

        //#region Object Function
        this.SafeObject = function (o) {
            if (typeof (o) == 'object') {
                return o;
            }
            else if (typeof (o) == 'string') {
                return this.GetObject(o);
            }
            else {
                return null;
            }
        }

        this.GetObject = function (sID) {
            return document.getElementById(sID);
        }

        this.GetObjectsByIDPrefix = function (sPrefix, sTagName, oContainer) {
            oContainer = oContainer == undefined ? document : int.f.SafeObject(oContainer);
            if (sTagName == undefined) sTagName = 'input';

            var aObjects = new Array();

            var aElements = oContainer.getElementsByTagName(sTagName);
            for (var i = 0; i < aElements.length; i++) {
                if (int.s.StartsWith(aElements[i].id, sPrefix)) aObjects.push(aElements[i]);
            }

            return aObjects;
        }

        this.GetElementsByClassName = function (sElement, sClassName, oContainer) {
            oContainer = oContainer == undefined ? document : int.f.SafeObject(oContainer);
            var aReturn = new Array();

            if (oContainer == undefined) return aReturn;

            if (sElement == '' || sElement == null || sElement == undefined) sElement = '*';

            var aElements = oContainer.getElementsByTagName(sElement);
            for (var i = 0; i < aElements.length; i++) {
                if (aElements[i].className.indexOf(sClassName) > -1) aReturn[aReturn.length] = aElements[i];
            };

            return aReturn;
        }

        this.GetObjectsBySelector = function (sSelector) {
            var aObjects = new Array();

            aObjects = document.querySelectorAll(sSelector);

            return aObjects;
        }

        //#endregion

        //#region Value functions
        this.GetValue = function (o) {
            var oControl = this.SafeObject(o);
            return oControl != null ? oControl.value : '';
        }

        this.GetIntValue = function (o) {
            var oControl = this.SafeObject(o);
            return oControl != null ? int.n.SafeInt(oControl.value) : 0;
        }

        this.GetNumericValue = function (o) {
            var oControl = this.SafeObject(o);
            return oControl != null ? int.n.SafeNumeric(oControl.value) : 0;
        }

        this.SetValue = function (o, sValue) {
            var oControl = this.SafeObject(o);
            if (oControl != null) oControl.value = sValue;
        }

        this.SetValueIf = function (o, sValue, bCondition) {
            if (bCondition) int.f.SetValue(o, sValue);
        }
        //#endregion

        //#region HTML funcs
        this.GetHTML = function (o) {
            var oControl = this.SafeObject(o);
            return oControl != null ? oControl.innerHTML : '';
        }

        this.SetHTML = function (o, sValue, bRunInlineScripts) {
            var oControl = this.SafeObject(o);
            if (oControl != null) oControl.innerHTML = sValue;
            // run any inline scripts included in the HTML if the bRunScriptTags parameter was specified.
            if (bRunInlineScripts != undefined && bRunInlineScripts) int.f.RunScriptsWithinHTML(sValue);
        }

        this.RunScriptsWithinHTML = function (sHTML) {
            var scriptregex = /\<script(?:\stype=(?:"text\/javascript"|text\/javascript))?\s?\>([\s\S]*?)\<\/script\>/gim;
            var match;
            while (match = scriptregex.exec(sHTML)) {
                eval(match[1]);
            }
        }
        //#endregion

        this.Toggle = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl.tagName.toUpperCase() == 'TR' && !int.b.IE()) {
                oControl.style.display = oControl.style.display == 'none' ? 'table-row' : 'none';
            }
            else {
                oControl.style.display = oControl.style.display == 'none' ? 'block' : 'none';
            }
        }

        this.Show = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl != null) oControl.style.display = oControl.style.display = '';
        }

        this.Hide = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl != null) oControl.style.display = oControl.style.display = 'none';
        }

        this.ShowIf = function (o, bCondition) {
            if (o.constructor != Array) {
                var oControl = this.SafeObject(o);
                bCondition ? this.Show(o) : this.Hide(o);
            }
            else {
                for (var i = 0; i < o.length; i++) {
                    int.f.ShowIf(o[i], bCondition);
                }
            }
        }

        this.Visible = function (o) {
            var oControl = this.SafeObject(o);
            return oControl != null ? oControl.style.display != 'none' : false;
        }

        //#region Class functions
        this.SetClass = function (o, s) {
            var oControl = this.SafeObject(o);
            if (oControl != null) oControl.className = s;
        }

        this.GetClass = function (o) {
            var oControl = this.SafeObject(o);
            return oControl == null ? '' : oControl.className;
        }

        this.SetClassIf = function (o, ClassName, bCondition) {
            bCondition ? int.f.AddClass(o, ClassName) : int.f.RemoveClass(o, ClassName);
        }

        this.ToggleClassIf = function (o, TrueClass, FalseClass, bCondition) {
            if (bCondition) {
                int.f.AddClass(o, TrueClass);
                int.f.RemoveClass(o, FalseClass);
            }
            else {
                int.f.RemoveClass(o, TrueClass);
                int.f.AddClass(o, FalseClass);
            }
        }

        this.AddClass = function (o, s) {
            var oControl = this.SafeObject(o);
            var aClassNames = int.f.GetClass(o).split(' ');
            // add class if it doesn't exist already
            if (!int.f.HasClass(oControl, s)) int.f.SetClass(oControl, int.f.GetClass(oControl) + ' ' + s);
        }

        this.RemoveClass = function (o, s) {
            var sClassName = '';
            var aClassNames = int.f.GetClass(o).split(' ');

            for (var i = 0; i < aClassNames.length; i++) {
                if (aClassNames[i] != s) {
                    sClassName = sClassName + aClassNames[i] + ' ';
                    //chop off last character if it's a space
                    if (sClassName.indexOf(' ', sClassName.length - 1) != -1 && i == aClassNames.length - 1) {
                        sClassName = sClassName.substring(0, sClassName.length - 1);
                    }
                }
            }

            int.f.SetClass(o, sClassName);
        }

        this.ToggleClass = function (o, s) {
            int.f.HasClass(o, s) ? int.f.RemoveClass(o, s) : int.f.AddClass(o, s);
        }

        this.HasClass = function (o, s) {
            var sClass = int.f.GetClass(o);
            var aClassNames = sClass == undefined ? [] : sClass.split(' ');
            for (var i = 0; i < aClassNames.length; i++) {
                if (aClassNames[i] == s) return true;
            }
            return false;
        }
        //#endregion

        this.SetFocus = function (o) { // will also accept a pipe seperated list of objects to try in turn.
            if (typeof (o) == 'string') {
                var aObjects = o.split('|');
                for (var i = 0; i < aObjects.length; i++) {
                    if (int.f.SetFocus(int.f.SafeObject(aObjects[i]))) return true;
                }
            }
            else {
                var oControl = int.f.SafeObject(o);
                if (oControl && oControl.focus != undefined) {
                    try {
                        oControl.focus();
                        return true;
                    }
                    catch (exception) { /* stops the browser throwing up a silly error */
                    }
                }
            }
            return false;
        }

        this.ToggleFocus = function (o) {
            var oControl = this.SafeObject(o);
            oControl.style.display = oControl.style.display == 'none' ? 'block' : 'none';
            if (oControl.style.display == 'block') {
                int.f.SetFocus(oControl);
            }
        }

        this.BuildList = function (aListItems) {
            var sList = '<ul>';
            for (var i = 0; i < aListItems.length; i++) {
                sList += '<li>' + aListItems[i] + '</li>';
            }
            sList += '</ul>';
            return sList;
        }

        this.Disable = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl != null) {
                oControl.readOnly = true;
            }
        }

        this.Enable = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl != null) {
                oControl.readOnly = false;
            }
        }

        this.EnableIf = function (o, bCondition) {
            bCondition ? int.f.Enable(o) : int.f.Disable(o);
        }

        this.ClearFileUpload = function (o) {
            var oControl = this.SafeObject(o);
            if (oControl != null) {
                oControl.outerHTML = oControl.outerHTML;
            }
        }

        //#region event handling
        this.AttachEvent = function (oObject, sEventName, oFunction) {
            oObject = this.SafeObject(oObject);

            if (oObject) {
                var oListenerFunction = oFunction;

                if (oObject.addEventListener) {
                    oObject.addEventListener(sEventName, oListenerFunction, false);
                }
                else if (oObject.attachEvent) {
                    oListenerFunction = function () {
                        oFunction(window.event);
                    }
                    oObject.attachEvent("on" + sEventName, oListenerFunction);
                }
                else {
                    throw new Error("Event registration not supported");
                }

                var oEvent = {
                    Instance: oObject,
                    EventName: sEventName,
                    Listener: oListenerFunction
                };
                return oEvent;
            }

            return false;
        }

        this.DetachEvent = function (oEvent) {
            var oObject = oEvent.Instance;
            if (oObject.removeEventListener) {
                oObject.removeEventListener(oEvent.EventName, oEvent.Listener, false);
            }
            else if (oObject.detachEvent) {
                oObject.detachEvent("on" + oEvent.EventName, oEvent.Listener);
            }
        }

        this.GetObjectFromEvent = function (oEvent) {
            return oEvent.srcElement ? oEvent.srcElement : oEvent.target;
        }

        this.GetKeyCodeFromEvent = function (oEvent) {
            return oEvent.keyCode ? oEvent.keyCode : oEvent.which;
        }

        this.FireEvent = function (oObject, oEvent) {
            var o = int.f.SafeObject(oObject);
            if (o.dispatchEvent) {
                o.dispatchEvent(oEvent);
            }
            else if (o.fireEvent) {
                o.fireEvent('on' + oEvent.type, oEvent);
            }
            else {
                throw new Error("Event firing not supported");
            }
        }
        //#endregion

        //#region Popup
        this.ShowPopup = function (oObject, sClassName, sHTML, sSourceObjectID, bRightAlign, iYOffset, iXOffset, bBottomAlign, bCentreAlign, iBackgroundYOffset, bCentreBackground) {
            if (iYOffset == undefined) iYOffset = 0;
            if (iXOffset == undefined) iXOffset = 0;

            if (sSourceObjectID != undefined && int.f.GetObject(sSourceObjectID)) {
                sHTML = int.f.GetObject(sSourceObjectID).innerHTML;
            }

            //create container
            var oHelp = document.createElement('div');
            oHelp.setAttribute('id', 'divPopup');
            int.f.SetClass(oHelp, sClassName);
            oHelp.style.position = 'absolute';
            oHelp.innerHTML = sHTML;

            //set position
            var oDimensions = new int.e.BrowserDimensions();
            var oLinkPosition;
            if (!oObject.Left) {
                oLinkPosition = int.e.GetPosition(oObject);
            }
            else {
                oLinkPosition = new int.e.Position();
                oLinkPosition.Left = oObject.Left;
                oLinkPosition.Top = oObject.Top;
            }

            oHelp.style.top = int.n.SafeInt(oLinkPosition.Top + 20 + iYOffset) + 'px';
            oHelp.style.left = int.n.SafeInt(oLinkPosition.Left + iXOffset) + 'px';

            //create mask
            if (int.b.IE6()) {
                var oMask = document.createElement('iframe');
                oMask.setAttribute('id', 'iMask');
                oMask.src = '';
                int.e.SetPosition(oMask, int.e.GetPosition(oHelp));
                int.f.GetObject('body').appendChild(oMask);
            }

            int.f.GetObject('frm').appendChild(oHelp);

            //move it if it's too low
            oHelpPosition = int.e.GetPosition(oHelp);
            if (oHelpPosition.Top + oHelpPosition.Height > oDimensions.ViewportHeight + int.f.ScrollPosition()) {
                oHelp.style.top = oDimensions.ViewportHeight + int.f.ScrollPosition() - oHelp.offsetHeight - 10 + 'px';
            }

            //if we're right aligning then shift over now
            if (bRightAlign != undefined && bRightAlign) {
                oHelp.style.left = oLinkPosition.Left - oHelp.clientWidth + iXOffset + 'px';
            }

            //if we're bottom aligning then shift over now
            if (bBottomAlign != undefined && bBottomAlign) {
                oHelp.style.top = oLinkPosition.Top - oHelp.clientHeight + iYOffset + 'px';
            }

            if (bCentreAlign != undefined && bCentreAlign) {
                oHelp.style.top = oLinkPosition.Top - (oHelp.clientHeight / 2) + iYOffset + 'px';
            }

            //if we're centre aligning  (y) the background image then shift over now
            if (bCentreBackground != undefined && bCentreBackground) {
                var iYposition = (oHelp.clientHeight / 2) - iBackgroundYOffset + 'px';
                oHelp.style.backgroundPosition = '0px ' + iYposition;
            }
        }

        this.HidePopup = function () {
            if (int.b.IE6() && int.f.GetObject('iMask')) {
                int.f.GetObject('frm').removeChild(int.f.GetObject('iMask'));
                int.f.HidePopup();
            }

            if (int.f.GetObject('divPopup')) {
                int.f.GetObject('frm').removeChild(int.f.GetObject('divPopup'));
                int.f.HidePopup();
            }
        }
        //#endregion

        this.ScrollPosition = function () {
            return (window.pageYOffset) ?
					window.pageYOffset
					: (document.documentElement && document.documentElement.scrollTop)
						? document.documentElement.scrollTop : document.body.scrollTop;
        }

        this.GetContainerQueryString = function (oContainer, aExclude) {
            var aElements = int.f.SafeObject(oContainer).getElementsByTagName('*');

            var sQueryString = '';
            for (var i = 0; i < aElements.length; i++) {
                if (aElements[i].name && !int.a.ArrayContains(aExclude, aElements[i].name)) {
                    var bRadio = aElements[i].type && aElements[i].type.toUpperCase() == 'RADIO'

                    if (!bRadio) {
                        sQueryString += (sQueryString == '' ? '' : '&') + aElements[i].name + '=';
                    }

                    if (aElements[i].nodeName == 'INPUT' && int.s.StartsWith(aElements[i].id, 'chk')) {
                        sQueryString += aElements[i].checked;
                    }
                    else if (aElements[i].nodeName == 'INPUT' && bRadio) {
                        if (aElements[i].checked) {
                            sQueryString += aElements[i].name + '=' + int.f.GetValue(aElements[i]);
                        }
                    }
                    else if (aElements[i].nodeName == 'INPUT') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]));
                    }
                    else if (aElements[i].nodeName == 'SELECT') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]) != '' ? int.dd.GetValue(aElements[i]) : int.dd.GetText(aElements[i]));
                    }
                    else if (aElements[i].nodeName == 'TEXTAREA') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]));
                    }
                }
            }

            return sQueryString;
        }

        this.GetContainerQueryStringByIDs = function (oContainer, aExclude) {
            var aElements = int.f.SafeObject(oContainer).getElementsByTagName('*');

            var sQueryString = '';
            for (var i = 0; i < aElements.length; i++) {
                if (aElements[i].id && !int.a.ArrayContains(aExclude, aElements[i].id)
                    && aElements[i].name && !int.a.ArrayContains(aExclude, aElements[i].name)) {
                    var bRadio = aElements[i].type && aElements[i].type.toUpperCase() == 'RADIO'

                    if (!bRadio) {
                        sQueryString += (sQueryString == '' ? '' : '&') + aElements[i].id + '=';
                    }

                    if (aElements[i].nodeName == 'INPUT' && int.s.StartsWith(aElements[i].id, 'chk')) {
                        sQueryString += aElements[i].checked;
                    }
                    else if (aElements[i].nodeName == 'INPUT' && bRadio) {
                        if (aElements[i].checked) {
                            sQueryString += aElements[i].id + '=' + int.f.GetValue(aElements[i]);
                        }
                    }
                    else if (aElements[i].nodeName == 'INPUT') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]));
                    }
                    else if (aElements[i].nodeName == 'SELECT') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]) != '' ? int.dd.GetValue(aElements[i]) : int.dd.GetText(aElements[i]));
                    }
                    else if (aElements[i].nodeName == 'TEXTAREA') {
                        sQueryString += int.s.EncodeString(int.f.GetValue(aElements[i]));
                    }
                }
            }

            return sQueryString;
        }

        //#region Radio
        this.GetRadioButtonValue = function (sName) {
            var aElements = document.body.getElementsByTagName('INPUT');
            var sReturn = '';

            for (var i = 0; i < aElements.length; i++) {
                if (aElements[i].name == sName && aElements[i].checked) {
                    sReturn = aElements[i].value;
                    break;
                }
            }

            return sReturn
        }

        this.SetRadioButtonValue = function (sName, sValue) {
            var aElements = document.body.getElementsByTagName('INPUT');

            for (var i = 0; i < aElements[i].length; i++) {
                if (aElements[i].name == sName && aElements[i].value == sValue) {
                    aElements[i].checked = true;
                    break;
                }
            }
        }
        //#endregion

        //#region Postback
        this.Postback = function (sCommand, sArgument, bCheckDelete) {
            //confirm delete if required
            if (bCheckDelete == true) {
                if (confirm('Are you sure that you want to delete this record?') == false) {
                    return;
                }
            }

            if (typeof (oWYSIWYG) != 'undefined') {
                TidyXHTML();
            }

            document.forms[0].Command.value = sCommand;
            document.forms[0].Argument.value = sArgument;
            document.forms[0].action = '';
            document.forms[0].submit();
        }

        this.ButtonPostBack = function (oButton) {
            oButton.disabled = true;
            int.f.Postback(oButton.id, '');
        }
        //#endregion
    }

    //#endregion

    //#region Validators
    function Validators() {
        this.IsEmail = function (sEmail) {
            var sEmailRegEx = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            var o = new RegExp(sEmailRegEx);
            return o.test(sEmail);
        }

        this.IsURL = function (sURL) {
            var sURLRegEx = /(ht|f)tp(s?)\:\/\/[a-zA-Z0-9\-\._]+(\.[a-zA-Z0-9\-\._]+){2,}(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?/;
            var o = new RegExp(sURLRegEx);
            return o.test(sURL);
        }

        this.IsTime = function (sTime) {
            var sTimeRegEx = /(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]/;
            var o = new RegExp(sTimeRegEx);
            return o.test(sTime);
        }

        this.IsNumeric = function (sNumber) {
            var sNumberRegEx = /^[0-9]+$/;
            var o = new RegExp(sNumberRegEx);
            return o.test(sNumber);
        }

        this.IsAlpha = function (sAlpha) {
            var sAlphaRegEx = /^[a-zA-Z ]*$/;
            var o = new RegExp(sAlphaRegEx);
            return o.test(sAlpha);
        }

        this.IsPhoneNumber = function (sAlpha) {
            var sAlphaRegEx = /^[a-zA-Z 0-9\(\)\/\+]*$/;
            var o = new RegExp(sAlphaRegEx);
            return o.test(sAlpha);
        }

        this.IsNumericPhoneNumber = function (sAlpha) {
            var sAlphaRegEx = /^[ 0-9\(\)\/\+]*$/;
            var o = new RegExp(sAlphaRegEx);
            return o.test(sAlpha);
        }

        this.IsDate = function (sDate) {
            var nonDigit = /\D/g;
            var displaydateformat = /^[0-3][0-9]\s(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s(19|20)\d\d$/;
            var now = new Date();
            var dDate;
            var iDay;
            var sMonth;
            var iYear;

            //bomb out if no characters
            if (sDate.length == 0) return false;

            //display date
            if (displaydateformat.test(sDate)) {
                //test for max days
                iDay = parseInt(sDate.substring(0, 2), 10);
                sMonth = sDate.substring(3, 6);
                iYear = parseInt(sDate.substring(7, 11), 10);
                if (iDay <= 31 && (sMonth == 'Jan' || sMonth == 'Mar' || sMonth == 'May' || sMonth == 'Jul'
							|| sMonth == 'Aug' || sMonth == 'Oct' || sMonth == 'Dec')) {
                    return true;
                }
                else if (iDay <= 30 && (sMonth == 'Apr' || sMonth == 'Jun' || sMonth == 'Sep' || sMonth == 'Nov')) {
                    return true;
                }
                else if (sMonth == 'Feb' && ((iDay <= 29 && d.CheckLeapYear(iYear) == true)
								|| (iDay <= 28 && d.CheckLeapYear(iYear) == false))) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        this.IsValidDate = function (sDate) {
            //this checks if the date is format dd/mm/yyyy
            var sDateRegEx = /^(0[1-9]|[1-2][0-9]|3[0-1])[\/.-](0[1-9]|1[0-2])[\/.-][1,2][0-9]{3}$/;
            var o = new RegExp(sDateRegEx);
            return o.test(sDate);
        }

        this.IsSecurityCode = function (sCode, length) {
            if (length == undefined) {
                length = 3;
            };
            var sExp = '^[0-9]{3,4}$';
            //    for (var i = 0;i<length;i++){
            //        sExp+='[0-9]';
            //    }
            //    sExp+='$';

            var o = new RegExp(sExp);
            return o.test(sCode);
        }

        this.IsCardNumber = function (sCardNumber) {
            var iSum = 0;
            var bAlt = false;
            var iDigit;
            for (var i = sCardNumber.length; i--; i >= 0) {
                iDigit = int.n.SafeInt(int.s.Substring(sCardNumber, i, i + 1));
                if (bAlt) {
                    iDigit *= 2;
                    if (iDigit > 9) {
                        iDigit -= 9;
                    };
                };
                iSum += iDigit;
                bAlt = !bAlt;
            };
            return iSum % 10 == 0;
        }
    }

    //#endregion

    //#region checkbox functions
    function CheckBoxFunctions() {
        this.Checked = function (o) {
            o = int.f.SafeObject(o);
            if (o != null) {
                return o.checked;
            } else {
                return false;
            }
        }

        this.SetValue = function (o, sBoolean) {
            o = int.f.SafeObject(o);
            if (o != null) {
                if ((sBoolean + '').toLowerCase() == 'false') {
                    o.checked = false;
                } else {
                    o.checked = true;
                }
            } else {
                return false;
            }
        }

        this.Toggle = function (o) {
            o = int.f.SafeObject(o);
            if (o != null) {
                o.checked = !o.checked;
            }
        }
    }
    //#endregion

    //#region Radio Button Functions

    function RadioButtonFunctions() {
        this.GetValue = function (o) {
            var chosen = "";
            if (typeof (o) == 'string') {
                o = document.getElementsByName(o)
            }

            for (i = 0; i < o.length; i++) {
                if (int.cb.Checked(o[i])) {
                    chosen = o[i].value;
                }
            }
            if (chosen == "") {
                return null
            }
            else {
                return chosen
            }
        }
    }

    //#endregion

    //#region dropdown functions
    function DropdownFunctions() {
        this.GetText = function (o) {
            o = int.f.SafeObject(o);
            if (o != null && o.options.length > 0 && o.selectedIndex > -1) {
                return o.options[o.selectedIndex].text;
            } else {
                return '';
            }
        }

        this.GetIntText = function (o) {
            return int.n.SafeInt(int.dd.GetText(o));
        }

        this.GetValue = function (o) {
            o = int.f.SafeObject(o);
            if (o != null && o.selectedIndex >= 0) {
                return o.options[o.selectedIndex].value;
            } else {
                return '';
            }
        }

        this.GetIntValue = function (o) {
            return int.n.SafeInt(int.dd.GetValue(o));
        }

        this.GetIndex = function (o) {
            o = int.f.SafeObject(o);
            if (o != null) { return o.selectedIndex; }
        }

        this.ListCount = function (o) {
            o = int.f.SafeObject(o);
            if (o != null) { return o.options.length; }
        }

        this.SetIndex = function (o, iIndex) {
            o = int.f.SafeObject(o);
            if (o != null) { o.selectedIndex = iIndex; }
        }

        this.SetValue = function (o, iValue) {
            o = int.f.SafeObject(o);
            if (o != null && o.options) {
                for (var i = 0; i <= o.options.length - 1; i++) {
                    if (o.options[i].value == iValue) {
                        o.selectedIndex = i;
                        break;
                    }
                }
            }
        }

        this.SetText = function (o, sText) {
            var o = int.f.SafeObject(o);
            if (o != null) {
                for (var i = 0; i <= o.options.length - 1; i++) {
                    if (o.options[i].text != null) {
                        if (o.options[i].text == sText) {
                            o.selectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        this.Clear = function (o, sText) {
            var o = int.f.SafeObject(o);
            if (o != null) {
                o.options.length = 0;
            }
        }

        this.AddOption = function (o, sText, iValue, sClass) {
            var o = int.f.SafeObject(o);
            if (o != null) {
                o.options[o.length] = new Option(sText, iValue);
                if (sClass != undefined) {
                    o.options[o.length - 1].className = sClass;
                }
            }
        }

        this.SetOptions = function (o, sOptions) {
            var o = int.f.SafeObject(o);
            if (o != null) {
                this.Clear(o);

                var i = 0;
                while (sOptions.split('#')[i] != undefined) {
                    var sOption = sOptions.split('#')[i];
                    if (sOption.indexOf('|') >= 0) {
                        o.options[i] = new Option(sOption.split('|')[0], sOption.split('|')[1]);
                    } else {
                        o.options[i] = new Option(sOptions.split('#')[i]);
                    }
                    i += 1;
                }
            }
        }
    }

    // checked data list
    this.CheckedDatalist = function (o) {
        this.List = int.f.SafeObject(o);

        this.HasCheckedItems = function () {
            var aCheckboxes = int.f.GetObjectsByIDPrefix(this.List.id + 'chk');

            for (var i = 0; i < aCheckboxes.length; i++) {
                if (int.f.SafeObject(aCheckboxes[i]).checked == true) {
                    return true;
                }
            }

            return false;
        }
    }

    //#endregion

    //#region formfunctions
    this.ff = new function () {
        var me = this;

        /* safe param */
        this.SafeParam = function (sParam) {
            if (sParam.replace) {
                return sParam.replace(/\|/g, '/\\pipe\\/');
            } else if (sParam.getDate && sParam.getUTCFullYear && sParam.toDateString) { // attempt to find out whether sParam is a Date object
                return int.d.ToSQLDate(sParam);
            } else {
                return sParam;
            }
        }

        /* call */
        this.Call = function (FunctionName, CallBack) {
            //work out the params
            var sParams = '';
            for (var i = 2; i <= this.Call.arguments.length - 1; i++) {
                sParams += this.SafeParam(this.Call.arguments[i]) + '|'
            }
            if (sParams != '') { sParams = int.s.Chop(sParams); }

            //build up the url
            var sURL = window.location.href;
            if (int.s.Right(sURL, 1) == '#') sURL = int.s.Chop(sURL);
            if (int.s.Right(sURL, 1) == '/') sURL += 'default.aspx';

            if (sURL.indexOf('#') > 0) sURL = sURL.split("#")[0]

            sURL += (sURL.indexOf('?') == -1) ? '?' : '&';
            sURL += 'executeformfunction';
            sURL += '&function=' + FunctionName;

            //request
            var oRequest;
            if (window.XMLHttpRequest) {
                oRequest = new XMLHttpRequest();
                oRequest.open("POST", sURL, true);
            } else {
                oRequest = new ActiveXObject("Microsoft.XMLHTTP");
                oRequest.open("POST", sURL, true);
            }
            oRequest.onreadystatechange = function () {
                if (oRequest.readyState == 4) {
                    if (oRequest.status != 200 && window.location.toString().indexOf('localhost') > -1) {
                        alert(oRequest.responseText);
                        return;
                    }
                    int.ff.Response(oRequest.responseText, CallBack);
                }
            }

            var sParamString = 'params=' + encodeURIComponent(sParams);

            // this should always be available but in case of old dependencies...
            try {
                sParamString += '&pagename=' + INTV.Settings.PageName;
            } catch (ex) {
                //continue as normal
            }

            // set parameter string
            var sParameterString = sParamString;

            // set the necessary request headers
            oRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

            // send params
            oRequest.send(sParameterString);
        }

        /* call */
        this.SyncCall = function (FunctionName, CallBack) {
            //work out the params
            var sParams = '';
            for (var i = 2; i <= this.SyncCall.arguments.length - 1; i++) {
                sParams += this.SafeParam(this.SyncCall.arguments[i]) + '|'
            }
            if (sParams != '') { sParams = int.s.Chop(sParams); }

            //build up the url
            var sURL = window.location.href;
            if (int.s.Right(sURL, 1) == '#') sURL = int.s.Chop(sURL);
            if (int.s.Right(sURL, 1) == '/') sURL += 'default.aspx';

            if (sURL.indexOf('#') > 0) sURL = sURL.split("#")[0]

            sURL += (sURL.indexOf('?') == -1) ? '?' : '&';
            sURL += 'executeformfunction';
            sURL += '&function=' + FunctionName;

            //request
            var oRequest;
            if (window.XMLHttpRequest) {
                oRequest = new XMLHttpRequest();
                oRequest.open("POST", sURL, false);
            } else {
                oRequest = new ActiveXObject("Microsoft.XMLHTTP");
                oRequest.open("POST", sURL, false);
            }
            oRequest.onreadystatechange = function () {
                if (oRequest.readyState == 4) {
                    if (oRequest.status != 200 && window.location.toString().indexOf('localhost') > -1) {
                        alert(oRequest.responseText);
                        return;
                    }
                    int.ff.Response(oRequest.responseText, CallBack);
                }
            }

            // set parameter string
            var sParameterString = 'params=' + encodeURIComponent(sParams);

            // set the necessary request headers
            oRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

            // send params
            oRequest.send(sParameterString);
        }

        /* response */
        this.Response = function (sResponse, oCallBack) {
            if (typeof (oCallBack) == 'string') {
                eval(oCallBack + '(\'' + int.s.Replace(sResponse, '\'', '\\\'') + '\')');
            } else if (typeof (oCallBack == 'function')) {
                oCallBack(sResponse);
            }
        }
    }
    //#endregion

    //#region webservice
    this.WebService = function () {
        var oRequest, oResponseObject, bResponseTextOnly, oPopulateList, oHideDiv, SelectedID;

        //getlist
        this.PopulateList = function (URL, sNamespace, oList, SourceSQL, oDiv, iSelectedID) {
            oHideDiv = oDiv;
            SelectedID = iSelectedID;

            // get the data
            aParams = new Array(['SourceSQL', SourceSQL]);
            oPopulateList = int.f.SafeObject(oList);
            this.RunWebService(URL, sNamespace, 'GetList', aParams, oPopulateList);
        }

        this.FillList = function (oXML) {
            var bHasAll = (oPopulateList.options.length > 0 && oPopulateList.options[0].text == 'All');
            var bHasBlank = (oPopulateList.options.length > 0 && oPopulateList.options[0].text == '');
            var iOffset = 1;

            // clear the list
            int.dd.Clear(oPopulateList);

            //add all if required
            if (bHasAll) {
                oPopulateList[0] = new Option('All', -1);
            } else if (bHasBlank) {
                oPopulateList[0] = new Option('', 0);
            } else {
                iOffset = 0;
            }

            var oListItems = oXML.getElementsByTagName('ListItem');
            var sLastGroup = 'alanpartridge';
            var sValue;
            var iID;
            for (var i = 0; i < oListItems.length; i++) {
                sValue = this.GetNodeText(oListItems[i].childNodes[0]);
                iID = this.GetNodeText(oListItems[i].childNodes[1]);

                //if we're grouping
                if (sValue.indexOf('~') > -1) {
                    if (sValue.split('~')[0] != sLastGroup) {
                        oPopulateList[i + iOffset] = new Option(sValue.split('~')[0], 0);
                        oPopulateList.options[i + iOffset].className = 'dropdowngroup';
                        iOffset += 1;
                    }

                    oPopulateList[i + iOffset] = new Option(sValue.split('~')[1], iID);
                    sLastGroup = sValue.split('~')[0];
                } else {
                    oPopulateList[i + iOffset] = new Option(sValue, iID);
                }
            }

            if (oHideDiv != undefined) {
                int.f.ShowIf(oHideDiv, oListItems.length > 0);
            }

            if (SelectedID != null && SelectedID != undefined) {
                int.dd.SetValue(oPopulateList, SelectedID);
            }
        }

        //support functions
        this.GetTagValue = function (oLocalXML, sTag) {
            var aItems = oLocalXML.getElementsByTagName(sTag);
            if (aItems.length == 1 && aItems[0].childNodes[0]) {
                if (aItems[0].textContent) {
                    return aItems[0].textContent;
                } else {
                    return aItems[0].childNodes[0].data;
                }
            } else {
                return '';
            }
        }

        this.GetNodeText = function (oNode) {
            return oNode.text ? oNode.text : oNode.textContent;
        }

        this.SafeParam = function (sParam) {
            if (sParam.replace) {
                return sParam.replace(/&/g, '&amp;');
            } else if (sParam.getDate && sParam.getUTCFullYear && sParam.toDateString) { // attempt to find out whether sParam is a Date object
                return int.d.ToSQLDate(sParam);
            } else {
                return sParam;
            }
        }

        this.CallServer = function (sURL, oCallback) {
            if (window.XMLHttpRequest) {
                oRequest = new XMLHttpRequest();
                oRequest.open("POST", sURL, true);
            } else {
                oRequest = new ActiveXObject("Microsoft.XMLHTTP");
                oRequest.open("POST", sURL, true);
            }

            oRequest.onreadystatechange = function () {
                if (oRequest.readyState == 4) {
                    if (oRequest.status != 200 && window.location.toString().indexOf('localhost') > -1) {
                        alert(oRequest.responseText);
                        return;
                    } else {
                        oCallback(oRequest.responseText);
                    }
                }
            }

            oRequest.send();
        }

        this.RunWebService = function (sUrl, sNamespace, sFunction, aParameters, oCallingObject, bTextOnly) {
            var sRequest =
			'<?xml version="1.0" encoding="utf-8"?>' +
			'<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ' +
			'xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">' +
			'<soap:Body>' +
			'	<' + sFunction + ' xmlns="' + sNamespace + '">'

            for (var i = 0; i < aParameters.length; i++) {
                sRequest = sRequest + '<' + aParameters[i][0] + '>' +
				this.SafeParam(aParameters[i][1]) + '</' + aParameters[i][0] + '>';
            }

            sRequest = sRequest + '	</' + sFunction + '>' + '</soap:Body>' + '</soap:Envelope>';

            // branch for native XMLHttpRequest object
            if (window.XMLHttpRequest) {
                oRequest = new XMLHttpRequest();
                oRequest.open("POST", sUrl, true);
            } else {
                oRequest = new ActiveXObject("Microsoft.XMLHTTP");
                oRequest.open("POST", sUrl, true);
            }

            oResponseObject = oCallingObject;
            bResponseTextOnly = bTextOnly == undefined ? false : bTextOnly;

            oRequest.onreadystatechange = function () {
                if (oRequest.readyState == 4) {
                    if (oRequest.status != 200 && window.location.toString().indexOf('localhost') > -1) {
                        alert(oRequest.responseText);
                        return;
                    }

                    if (oResponseObject == oPopulateList) {
                        this.FillList(oRequest.responseXML);
                    } else if (bResponseTextOnly == false && oResponseObject.Done != undefined) {
                        oResponseObject.Done(oRequest.responseXML);
                    } else if (oResponseObject.Done != undefined) {
                        oResponseObject.Done(oRequest.responseText);
                    }
                }
            }

            oRequest.setRequestHeader("Content-Type", "text/xml;charset=UTF-8")
            oRequest.setRequestHeader("MessageType", "CALL")
            oRequest.setRequestHeader('SOAPAction', sNamespace + '/' + sFunction)
            oRequest.send(sRequest);
        }
    }

    //#endregion

    //#region effects
    function Effects() {
        this.SetOpacity = function (o, iOpacity) {
            var oControl = int.f.SafeObject(o);
            oControl.style.opacity = iOpacity / 100;
            oControl.style.filter = 'alpha(opacity=' + iOpacity + ')';
        }

        this.SlideOpen = function (oObject, SlideTime, FinalHeight, EndTime) {
            oObject = int.f.SafeObject(oObject);
            SlideTime = SlideTime == undefined ? 0.75 : SlideTime;

            if (EndTime == undefined) {
                oObject.style.overflow = 'hidden';
                oObject.style.display = 'block';
                oObject.style.height = '1px';
                oObject.style.height = 'auto';
                FinalHeight = oObject.scrollHeight;
                var dStart = new Date();
                EndTime = new Date(dStart.getTime() + (SlideTime * 1000));
            } else {
                EndTime = new Date(EndTime);
            }

            if (new Date() < EndTime) {
                oObject.style.height = Math.round(Math.sin(Math.PI / 2 * (1 - (EndTime - new Date()) / 1000 / SlideTime)) * FinalHeight) + 'px'
                setTimeout('int.e.SlideOpen(\'' + oObject.id + '\',' + SlideTime + ',' + FinalHeight + ',\'' + EndTime + '\')', 10);
            } else {
                oObject.style.height = FinalHeight + 'px';
            }
        }

        this.SlideClose = function (oObject, SlideTime, FinalHeight, EndTime) {
            oObject = int.f.SafeObject(oObject);
            SlideTime = SlideTime == undefined ? 0.75 : SlideTime;

            if (EndTime == undefined) {
                FinalHeight = oObject.scrollHeight;
                var dStart = new Date();
                EndTime = new Date(dStart.getTime() + (SlideTime * 1000));
            } else {
                EndTime = new Date(EndTime);
            }

            if (new Date() < EndTime) {
                oObject.style.height = Math.round(Math.sin(Math.PI / 2 * ((EndTime - new Date()) / 1000 / SlideTime)) * FinalHeight) + 'px'
                setTimeout('int.e.SlideClose(\'' + oObject.id + '\',' + SlideTime + ',' + FinalHeight + ',\'' + EndTime + '\')', 10);
            } else {
                oObject.style.height = 0;
                oObject.style.display = 'none';
            }
        }

        this.ScrollIntoView = function (Object, Padding, ScrollTime, iViewPortOffset) {

            iViewPortOffset = iViewPortOffset ? iViewPortOffset : 0;
            oObject = int.f.SafeObject(Object);
            Padding = Padding == undefined ? 0 : Padding;
            ScrollTime = ScrollTime == undefined ? 2 : ScrollTime;

            var oBrowserDimensions = new int.e.BrowserDimensions();
            var oObjectPosition = int.e.GetPosition(oObject);

            var iObjectTop = oObjectPosition.Top;
            var iObjectBottom = oObjectPosition.Top + oObjectPosition.Height;
            var iObjectHeight = oObjectPosition.Height;
            var iViewportTop = oBrowserDimensions.ScrollYPos;
            var iViewportBottom = oBrowserDimensions.ScrollYPos + oBrowserDimensions.ViewportHeight;
            var iViewportHeight = oBrowserDimensions.ViewportHeight;

            var dStart = new Date();
            var dEndTime = new Date(dStart.getTime() + (ScrollTime * 1000));

            if (iObjectHeight > iViewportHeight && iObjectTop < iViewportTop && iObjectBottom > iViewportBottom) {
                return;
            } else if (iObjectTop < (iViewportTop + iViewPortOffset)) {
                int.e.ScrollToObject(oObject, ScrollTime, iViewportTop, iObjectTop - Padding, dEndTime);
            } else if (iObjectBottom > iViewportBottom) {
                int.e.ScrollToObject(oObject, ScrollTime, iViewportTop, iObjectBottom - iViewportHeight + Padding, dEndTime);
            }
        }

        this.ScrollToObject = function (Object, ScrollTime, StartPosition, FinalPosition, EndTime) {
            oObject = int.f.SafeObject(Object);
            ScrollTime = ScrollTime == undefined ? 2 : ScrollTime;

            if (EndTime == undefined) {
                var oBrowserDimensions = new int.e.BrowserDimensions();
                StartPosition = oBrowserDimensions.ScrollYPos;
                FinalPosition = int.e.GetPosition(oObject).Top;

                var dStart = new Date();
                EndTime = new Date(dStart.getTime() + (ScrollTime * 1000));
            } else {
                EndTime = new Date(EndTime);
            }

            if (new Date() < EndTime) {
                var nFractionNotDone = (EndTime - new Date()) / (ScrollTime * 1000);
                // uses a combination of cosine and power for the multiplier so it starts smooth and ends smooth, but goes faster when it starts than when it ends...
                var nMultiplier = 0.5 + Math.cos(Math.PI * Math.pow(nFractionNotDone, 2)) / 2;
                var iScrollPosition = Math.round(StartPosition + nMultiplier * (FinalPosition - StartPosition));

                if (window.pageYOffset) {
                    window.scrollTo(0, iScrollPosition);
                } else if (document.documentElement) {
                    document.documentElement.scrollTop = iScrollPosition;
                } else {
                    document.body.scrollTop = iScrollPosition;
                }

                setTimeout(
					function () {
					    int.e.ScrollToObject(oObject, ScrollTime, StartPosition, FinalPosition, EndTime);
					},
					10
				);
            } else if (window.pageYOffset) {
                window.scrollTo(0, FinalPosition);
            } else if (document.documentElement) {
                document.documentElement.scrollTop = FinalPosition;
            } else {
                document.body.scrollTop = FinalPosition;
            }
        }

        this.FadeOut = function (oObject, FadeTime, Opacity) {
            this.FadeOutObject = int.f.SafeObject(oObject);
            FadeTime = FadeTime == undefined ? 1 : FadeTime;
            this.FadeInterval = FadeTime / 20 * 800;
            this.Opacity = Opacity == undefined ? 100 : Opacity;

            this.Opacity -= 5;

            if (this.Opacity < 0) {
                int.e.SetOpacity(this.FadeOutObject, 0);
            } else {
                int.e.SetOpacity(this.FadeOutObject, this.Opacity);
                setTimeout('int.e.FadeOut(\'' + this.FadeOutObject.id + '\',' + FadeTime + ',' + this.Opacity + ')',
				this.FadeInterval);
            }
        }

        this.FadeIn = function (oObject, FadeTime, Opacity) {
            this.FadeInObject = int.f.SafeObject(oObject);
            FadeTime = FadeTime == undefined ? 1 : FadeTime;
            this.FadeInterval = FadeTime / 20 * 800;
            this.Opacity = Opacity == undefined ? 0 : Opacity;

            this.Opacity += 5;

            if (this.Opacity > 100) {
                int.e.SetOpacity(this.FadeInObject, 100);
            } else {
                int.e.SetOpacity(this.FadeInObject, this.Opacity);
                setTimeout('int.e.FadeIn(\'' + this.FadeInObject.id + '\',' + FadeTime + ',' + this.Opacity + ')',
				this.FadeInterval);
            }
        }

        this.ImageRotator = function (IDBase, ItemCount, RotateTime, CurrentIndex) {
            if (ItemCount > 1) {
                RotateTime = RotateTime == undefined ? 2 : parseInt(RotateTime);
                CurrentIndex = CurrentIndex == undefined ? 0 : CurrentIndex;

                if (CurrentIndex == 0) {
                    CurrentIndex = 1;

                    var oFirst = int.f.GetObject(IDBase + CurrentIndex);
                    oFirst.style.zIndex = 100;
                } else {
                    var oFadeOut = int.f.GetObject(IDBase + CurrentIndex);
                    if (oFadeOut == null) {
                        return false;
                    }

                    CurrentIndex += 1;
                    CurrentIndex = CurrentIndex > ItemCount ? 1 : CurrentIndex;

                    var oFadeIn = int.f.GetObject(IDBase + CurrentIndex);

                    int.e.FadeOut(oFadeOut);
                    int.e.FadeIn(oFadeIn);

                    oFadeOut.style.zIndex = 0;
                    oFadeIn.style.zIndex = 100;
                }

                setTimeout('int.e.ImageRotator(\'' + IDBase + '\',' + ItemCount + ',' + RotateTime + ',' + CurrentIndex + ')', RotateTime * 1000);
            }
        }

        this.GetPosition = function (o) {
            var oControl = int.f.SafeObject(o);

            var s = oControl.id;

            var iLeft = 0, iTop = 0, iWidth, iHeight, iScrollLeft, iScrollTop;
            iWidth = oControl.offsetWidth;
            iHeight = oControl.offsetHeight;

            if (oControl.offsetParent) {
                iLeft = oControl.offsetLeft;
                iTop = oControl.offsetTop;
                while (oControl = oControl.offsetParent) {
                    iScrollLeft = (oControl.offsetParent && oControl.scrollLeft > 0 ? oControl.scrollLeft : 0)
                    iScrollTop = (oControl.offsetParent && oControl.scrollTop > 0 ? oControl.scrollTop : 0)
                    iLeft += oControl.offsetLeft - iScrollLeft;
                    iTop += oControl.offsetTop - iScrollTop;
                }
            }

            return new this.Position(iLeft, iTop, iWidth, iHeight);
        }

        this.SetPosition = function (o, oPosition) {
            oControl = int.f.SafeObject(o);
            oControl.style.top = oPosition.Top + 'px';
            oControl.style.left = oPosition.Left + 'px';
            oControl.style.width = oPosition.Width + 'px';
            if (oPosition.Height > 0) {
                oControl.style.height = oPosition.Height + 'px';
            }
        }

        this.SetTopLeft = function (o, iTop, iLeft) {
            oControl = int.f.SafeObject(o);
            oControl.style.top = iTop + 'px';
            oControl.style.left = iLeft + 'px';
        }

        this.Position = function (iLeft, iTop, iWidth, iHeight) {
            this.Left = iLeft;
            this.Top = iTop;
            this.Width = iWidth;
            this.Height = iHeight;
        }

        this.BrowserDimensions = function () {
            var iXScroll, iYScroll;

            if (document.documentElement != undefined && document.documentElement.scrollHeight) {
                iXScroll = document.documentElement.scrollWidth;
                iYScroll = document.documentElement.scrollHeight;
            } else if (window.innerHeight && window.scrollMaxY) {
                iXScroll = window.innerWidth + window.scrollMaxX;
                iYScroll = window.innerHeight + window.scrollMaxY;
            } else if (document.body.scrollHeight > document.body.offsetHeight) {
                iXScroll = document.body.scrollWidth;
                iYScroll = document.body.scrollHeight;
            } else {
                iXScroll = document.body.offsetWidth;
                iYScroll = document.body.offsetHeight;
            }

            var iWindowWidth, iWindowHeight;

            if (self.innerHeight) {
                if (document.documentElement.clientWidth) {
                    iWindowWidth = document.documentElement.clientWidth;
                } else {
                    iWindowWidth = self.innerWidth;
                }
                iWindowHeight = self.innerHeight;
            } else if (document.documentElement && document.documentElement.clientHeight) {
                iWindowWidth = document.documentElement.clientWidth;
                iWindowHeight = document.documentElement.clientHeight;
            } else if (document.body) {
                iWindowWidth = document.body.clientWidth;
                iWindowHeight = document.body.clientHeight;
            }

            var iXScrollPos, iYScrollPos;

            if (window.pageYOffset && window.pageXOffset) {
                iXScrollPos = window.pageXOffset
                iYScrollPos = window.pageYOffset
            } else if (document.documentElement) {
                iXScrollPos = document.documentElement.scrollLeft;
                iYScrollPos = document.documentElement.scrollTop;
            } else {
                iXScrollPos = document.body.scrollLeft;
                iYScrollPos = document.body.scrollTop;
            }

            if (window.pageYOffset) {
                iYScrollPos = window.pageYOffset
            } else if (document.documentElement) {
                iYScrollPos = document.documentElement.scrollTop;
            } else {
                iYScrollPos = document.body.scrollTop;
            }

            var iPageHeight, iPageWidth
            var iPageHeight = iYScroll < iWindowHeight ? iWindowHeight : iYScroll;
            var iPageWidth = iXScroll < iWindowWidth ? iXScroll : iWindowWidth;

            this.ViewportWidth = iWindowWidth;
            this.ViewportHeight = iWindowHeight;
            this.PageWidth = iPageWidth;
            this.PageHeight = iPageHeight;
            this.PageScrollTop = iYScroll;
            this.ScrollYPos = iYScrollPos;
            this.ScrollXPos = iXScrollPos;
        }

        this.CreateOverlay = function (oOpacity, oContainer) {
            var oOverlay = document.createElement('div');
            oOverlay.setAttribute('id', 'divOverlay');

            if (oContainer == undefined) {
                document.body.appendChild(oOverlay);
            } else {
                oContainer.appendChild(oOverlay);
            }

            var oDimensions = new int.e.BrowserDimensions();
            oOverlay.style.height = oDimensions.PageHeight + 'px';
            oOverlay.style.left = oDimensions.ScrollXPos + 'px';
            oOverlay.style.width = '100%';

            if (oOpacity == undefined) {
                int.e.SetOpacity(oOverlay, 100);
            } else {
                int.e.SetOpacity(oOverlay, int.n.SafeInt(oOpacity));
            }

            return oOverlay;
        }

        //if ie6, hide/show the dropdowns!
        var ToggleDropdownVisibility = new function () {
            //hide
            this.Hide = function () {
                if (int.b.IE6()) {
                    var aSelect = document.getElementsByTagName('select');
                    for (var i = 0; i < aSelect.length; i++) {
                        aSelect[i].style.visibility = 'hidden';
                    }
                }
            }

            //show
            this.Show = function () {
                if (int.b.IE6()) {
                    var aSelect = document.getElementsByTagName('select');
                    for (var i = 0; i < aSelect.length; i++) {
                        aSelect[i].style.visibility = 'visible';
                    }
                }
            }
        }

        /* modal popup */
        this.ModalPopup = new function () {
            this.PopupDiv;
            this.ContainerDiv;
            this.EscapeEvent;

            this.Show = function (oDiv, oParent, sTitle, iOpacity) {
                int.e.ModalPopup.Close();

                ToggleDropdownVisibility.Hide();

                this.PopupDiv = int.f.SafeObject(oDiv);

                //if we've been drawn from the modal popup control we will have a container div
                this.ContainerDiv = int.f.SafeObject(this.PopupDiv.id + '_Container');

                if (iOpacity == undefined || iOpacity == null) { iOpacity = 100; };

                if (this.ContainerDiv != null) {
                    //ensure we are outside of div all
                    document.forms[0].appendChild(this.ContainerDiv);
                    int.e.CreateOverlay(iOpacity, this.ContainerDiv);

                    int.f.Show(this.ContainerDiv);
                } else {
                    int.e.CreateOverlay(iOpacity);
                    int.f.Show(this.PopupDiv);
                }

                this.UpdateScreenPosition(oParent);

                if (sTitle != undefined) {
                    int.f.SetHTML('h3' + this.PopupDiv.id, sTitle);
                }

                int.e.ModalPopup.EscapeEvent = int.f.AttachEvent(document, 'keypress',
				function (oEvent) {
				    if (int.f.GetKeyCodeFromEvent(oEvent) == 27) {
				        int.e.ModalPopup.Close();
				    }
				});
            }

            this.Close = function () {
                if (int.f.Visible(int.e.ModalPopup.PopupDiv)) {
                    if (this.ContainerDiv != null) {
                        int.f.Hide(int.e.ModalPopup.ContainerDiv);
                        if (int.f.GetObject('divOverlay') != null) {
                            this.ContainerDiv.removeChild(int.f.GetObject('divOverlay'));
                        }
                    } else {
                        int.f.Hide(int.e.ModalPopup.PopupDiv);
                        if (int.f.GetObject('divOverlay') != null) {
                            document.body.removeChild(int.f.GetObject('divOverlay'));
                        }
                    }

                    int.f.DetachEvent(int.e.ModalPopup.EscapeEvent);
                    ToggleDropdownVisibility.Show();
                }
            }

            this.UpdateScreenPosition = function (oParent) {
                var iControlWidth = this.PopupDiv.offsetWidth;
                var iControlHeight = this.PopupDiv.offsetHeight;

                var oDimensions = new int.e.BrowserDimensions();
                var iLeft;
                if (oParent == undefined) {
                    iLeft = (oDimensions.ViewportWidth - iControlWidth) / 2 + oDimensions.ScrollXPos;
                } else {
                    oParentPosition = int.e.GetPosition(oParent);
                    iLeft = oParentPosition.Left + (oParentPosition.Width - iControlWidth) / 2;
                }

                var iTop = (oDimensions.ViewportHeight / 3) - (iControlHeight / 2);

                //set min top
                iTop = iTop < 50 ? 50 : iTop;
                iTop += oDimensions.ScrollYPos;

                this.PopupDiv.style.left = iLeft + 'px';
                this.PopupDiv.style.top = iTop + 'px';

                this.CheckOverlayIsTallEnough();
            }

            this.CheckOverlayIsTallEnough = function () {
                var oOverlay = int.f.GetObject('divOverlay');
                var oDimensions = new int.e.BrowserDimensions();
                var iPopupBottom = this.PopupDiv.offsetTop + this.PopupDiv.offsetHeight;

                if (iPopupBottom + 20 > oDimensions.PageHeight) {
                    oOverlay.style.height = iPopupBottom + 20 + 'px';
                }
            }
        }

        this.TransitionEnd = function (sObject, oFunction) {
            var oObject = int.f.SafeObject(sObject);
            var sTransitionEvent = int.e.SupportedTransitionEvent();

            oObject.addEventListener(sTransitionEvent, function (e) {
                e.target.removeEventListener(e.type, arguments.callee);
                oFunction();
            });
        }

        this.SupportedTransitionEvent = function () {
            var sTransition,
			oElement = document.createElement("fakeelement");

            var transitions = {
                "transition": "transitionend",
                "OTransition": "oTransitionEnd",
                "MozTransition": "transitionend",
                "WebkitTransition": "webkitTransitionEnd"
            }

            for (sTransition in transitions) {
                if (oElement.style[sTransition] !== undefined) {
                    return transitions[sTransition];
                }
            }
        }
    }

    //#endregion

    //#region datalist functions
    function DataListFunctions() {
        this.SetSelected = function (ListID, ID) {
            //work out the id prefix
            var oHolder = int.f.GetObject(ListID + 'Scroll');
            var sBaseID = oHolder.childNodes[0].nodeName == '#text' ? oHolder.childNodes[1].id : oHolder.childNodes[0].id;

            var oRows = int.f.GetObjectsByIDPrefix(sBaseID, 'tr');
            for (var i = 0; i < oRows.length; i++) {
                if (oRows[i].id == sBaseID + '_' + ID) {
                    int.f.AddClass(oRows[i], 'selected');
                } else {
                    int.f.RemoveClass(oRows[i], 'selected');
                }
            }
        }
    }

    //#endregion

    //#region cookie functions
    function CookieFunctions() {
        this.Set = function (sName, sValue, iDays) {
            var sExpires;
            if (iDays) {
                var dDate = new Date();
                dDate.setTime(dDate.getTime() + (iDays * 24 * 60 * 60 * 1000));
                sExpires = '; expires=' + dDate.toGMTString();
            } else {
                sExpires = '';
            }
            document.cookie = sName + '=' + sValue + sExpires + '; path=/';
        }

        this.Get = function (sName) {
            var sNameEQ = sName + '=';
            var aCookies = document.cookie.split(';');
            for (var i = 0; i < aCookies.length; i++) {
                var sCookie = aCookies[i];
                while (sCookie.charAt(0) == ' ') sCookie = sCookie.substring(1, sCookie.length);
                if (sCookie.indexOf(sNameEQ) == 0) {
                    return sCookie.substring(sNameEQ.length, sCookie.length);
                }
            }
            return '';
        }

        this.Delete = function (sName) {
            int.c.Set(sName, '', -1);
        }
    }

    //#endregion

    //#region browser functions
    function BrowserFunctions() {
        this.BrowserName = function () {
            return navigator.appName
        }

        this.BrowserVersion = function () {
            return navigator.appVersion
        }

        this.IE = function () {
            return this.BrowserName() == 'Microsoft Internet Explorer'
        }

        this.IE6 = function () {
            return this.IE() && this.BrowserVersion().indexOf('MSIE 6') > 0;
        }
    }

    //#endregion

    //#region lazy load

    //set up on load runner execute

    if (!window.addEventListener) {
        window.attachEvent('onload',
			function () {
			    int.ll.OnLoad.Execute();
			}
		);
    }
    else {
        window.addEventListener('load', function () { int.ll.OnLoad.Execute(); }, false);
    }

    function LazyLoadFunctions() {
        var me = this;
        var aOnLoad = new Array();

        /*
		0. OnLoad runner
		*/
        this.OnLoad = new function () {
            //0a run (adds to array)
            this.Run = function (oFunction, iPriority) {
                if (iPriority == undefined) {
                    iPriority = 3
                }

                var oPushFunction = new Array(2);
                oPushFunction[0] = oFunction;
                oPushFunction[1] = iPriority;
                aOnLoad.push(oPushFunction);
            }

            //0b Execute
            this.Execute = function () {
                for (var i = 0; i < aOnLoad.length; i++) {
                    if (aOnLoad[i][1] == 1) {
                        aOnLoad[i][0]();
                    }
                }
                for (var i = 0; i < aOnLoad.length; i++) {
                    if (aOnLoad[i][1] == 2) {
                        aOnLoad[i][0]();
                    }
                }
                for (var i = 0; i < aOnLoad.length; i++) {
                    if (aOnLoad[i][1] == 3) {
                        aOnLoad[i][0]();
                    }
                }
                for (var i = 0; i < aOnLoad.length; i++) {
                    if (aOnLoad[i][1] == 4) {
                        aOnLoad[i][0]();
                    }
                }
                for (var i = 0; i < aOnLoad.length; i++) {
                    if (aOnLoad[i][1] > 4) {
                        aOnLoad[i][0]();
                    }
                }
            }
        }

        /*
		1. images
		*/
        this.Image = new function () {
            //1a. load
            this.Load = function (oImage) {
                var oImage = int.f.SafeObject(oImage);
                if (oImage) {
                    oImage.setAttribute('src', oImage.getAttribute('original'));
                    oImage.removeAttribute('original');
                }
            }

            // 1b. loadcontainer
            this.LoadContainer = function (sID) {
                var oContainer = int.f.GetObject(sID);
                if (oContainer) {
                    var aImages = oContainer.getElementsByTagName('img');
                    for (var i = 0; i < aImages.length; i++) {
                        if (aImages[i].getAttribute('original')) {
                            me.Image.Load(aImages[i]);
                        }
                    }
                }
            }
        }

        /*
		2. Social Media
		*/
        this.SocialMedia = new function () {



            //2b. Add All three Buttons
            this.AddAll = function (sFacebook, sTwitter, sGoogle) {

                var sHost = window.location.host;
                if (sHost.indexOf('localhost') != -1) {
                    sHost = 'www.lowcostholidays.com'
                }

                var sPath = window.location.pathname.toLowerCase()
                if (sPath == '/default.aspx') {
                    sPath = '/'
                }

			    var urlBase = window.location.origin.indexOf('https:') > -1 ? 'https://' : 'http://';

			    var sPage = urlBase + sHost + sPath;
                me.SocialMedia.AddTwitter(sPage, sTwitter);

                //delay adding in google and facebook as it causes twitter's script to error
                setTimeout('ll.SocialMedia.AddFacebook(\'' + sPage + '\',\'' + sFacebook + '\');', 1500);
                setTimeout('ll.SocialMedia.AddGooglePlus(\'' + sPage + '\',\'' + sGoogle + '\');', 1500);

                //Styling fix for IE8 as google plus does not render
                if (!window.addEventListener) {
                    var oContainer = int.f.GetObject('divSocialNetworkingPanel');
                    if (oContainer) {
                        oContainer.style.marginRight = '-15px';
                    }
                }

            }

            //2c. Add the twitter icon and run twitter's own script
            this.AddTwitter = function (sPage, sTwitter) {

                var oContainer = int.f.GetObject('divSocialNetworkingPanel');
                if (oContainer) {

                    var sTwitterURL = 'https://twitter.com/share?url=' + encodeURIComponent(sPage);

                    // If culture code is not set default to english
                    if (sTwitter == '')
                        oContainer.innerHTML += '<div class="socialNetworkContainer"><a href="' + sTwitterURL + '" class="twitter-share-button" data-count="horizontal">Tweet</a></div>'
                    else
                        oContainer.innerHTML += '<div class="socialNetworkContainer"><a href="' + sTwitterURL + '" class="twitter-share-button" data-count="horizontal" data-lang="' + sTwitter + '">Tweet</a></div>'

                    //Twitter's Own Code Begin
                    !function (d, s, id) {
                        var js, fjs = d.getElementsByTagName(s)[0];
                        if (!d.getElementById(id)) {
                            js = d.createElement(s);
                            js.id = id;
							js.src = 'https://platform.twitter.com/widgets.js';
                            fjs.parentNode.insertBefore(js, fjs);
                        }
					} (document, 'script', 'twitter-wjs');

                }
            }



            //2d. Run Facebooks own script
            this.AddFacebook = function (sPage, sFacebook) {

                var oContainer = int.f.GetObject('divSocialNetworkingPanel')

                if (oContainer) {
                    //Facebook own script
                    !function (d, s, id) {
                        var js, fjs = d.getElementsByTagName(s)[0];
                        if (d.getElementById(id)) return;
                        js = d.createElement(s); js.id = id;

                        // If culture code is not set default to english
                        if (sFacebook == '')
                            js.src = '//connect.facebook.net/en_GB/all.js#xfbml=1';
                        else
                            js.src = '//connect.facebook.net/' + sFacebook + '/all.js#xfbml=1';

                        fjs.parentNode.insertBefore(js, fjs);
					} (document, 'script', 'facebook-jssdk');

                    oContainer.innerHTML = '<div id="divFacebookContainer" class="socialNetworkContainer"><fb:like href="' + sPage + '" send="false" layout="button_count" width="50" show_faces="false" action="like" font="" /></div>' + oContainer.innerHTML

                }

            }

            //2e. Add the GooglePlus icon and its own script
            this.AddGooglePlus = function (sPage, sGoogle) {

                var oContainer = int.f.GetObject('divSocialNetworkingPanel')
                if (oContainer) {

                    //Google's own script
                    !function () {
                        var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
                        po.src = 'https://apis.google.com/js/plusone.js';
                        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
					} ();

                    var sHTML = '<div class="socialNetworkContainer"><div id="divGooglePlusOneButton"><g:plusone size="medium"> </g:plusone><link rel="canonical" href="' + sPage + '">'


                    sHTML += '<script type="text/javascript">'

                    // If culture code is not set default to english
                    if (sGoogle != '') {
                        sHTML += 'window.___gcfg = {lang: \'' + sGoogle + '\'};'
                    }

                    sHTML += '(!function () {'
                    sHTML += 'var po = document.createElement(\'script\'); po.type = \'text/javascript\'; po.async = true;'
                    sHTML += 'po.src = \'https://apis.google.com/js/plusone.js'
                    sHTML += ';'
                    sHTML += 'var s = document.getElementsByTagName(\'script\')[0]; s.parentNode.insertBefore(po, s);'
                    sHTML += '})();'
                    sHTML += '</script>'

                    sHTML += '</div></div>'

                    oContainer.innerHTML += sHTML

                }
            }

        }

    }

    //#endregion
}

//#region Freeze library
//this will prevent int being overwritten by external js in newer browsers
if (Object.freeze) Object.freeze(int);
//#endregion

//#region Functions

//kept here for backwards compatability for textbox web controls

function iif(bCondition, oTrue, oFalse) {
    return bCondition ? oTrue : oFalse;
}

function IntegerOnly(oEvent) {
    iKeyPress = iif(oEvent.keyCode, oEvent.keyCode, oEvent.which);
    return (iKeyPress > 47 && iKeyPress < 58) || (iKeyPress == 8) || (iKeyPress == 37) || (iKeyPress == 39);
}

function TimeOnly(oEvent) {
    iKeyPress = iif(oEvent.keyCode, oEvent.keyCode, oEvent.which);
    return iKeyPress > 47 && iKeyPress < 59;
}

function ParseTime(oTextBox) {
    var sTime = int.f.GetValue(oTextBox);

    if (sTime.length == 3 && parseInt(sTime.charAt(1), 10) < 6 && parseInt(sTime, 10) > 0) {
        int.f.SetValue(oTextBox, '0' + sTime.charAt(0) + ':' + sTime.charAt(1) + sTime.charAt(2));
    }
    else if (sTime.length == 4 && sTime.charAt(1) != ':' && parseInt(sTime.charAt(2), 10) < 6 && parseInt(sTime.substr(0, 2), 10) < 24 && parseInt(sTime, 10) > 0) {
        int.f.SetValue(oTextBox, sTime.charAt(0) + sTime.charAt(1) + ':' + sTime.charAt(2) + sTime.charAt(3));
    }
    else if (sTime.length == 4 && sTime.charAt(1) == ':' && parseInt(sTime.charAt(2), 10) < 6 && parseInt(sTime.charAt(0), 10) > 0 && parseInt(sTime, 10) > 0) {
        int.f.SetValue(oTextBox, '0' + sTime.charAt(0) + ':' + sTime.charAt(2) + sTime.charAt(3));
    }
}

function TextboxOnEnter(oEvent, oFunction) {
    if (int.f.GetKeyCodeFromEvent(oEvent) == 13) {
        oFunction();
        oEvent.cancelBubble = true;
        oEvent.returnValue = false;
    }
}

function IEArrayIndexOfFix() {
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (elt /*, from*/) {
            var len = this.length >>> 0;

            var from = Number(arguments[1]) || 0;
            from = (from < 0) ? Math.ceil(from) : Math.floor(from);

            if (from < 0) from += len;

            for (; from < len; from++) {
                if (from in this && this[from] === elt)
                    return from;
            }

            return -1;
        };
    }
}

//#endregion
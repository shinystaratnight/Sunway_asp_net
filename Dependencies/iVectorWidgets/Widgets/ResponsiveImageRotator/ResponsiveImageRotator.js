function ResponsiveImageRotator(object) {

	var me = this;

	this.Object = object;
	this.Identifier;
	this.Holder;
	this.PageCount;
	this.CurrentPage;
	this.Seconds;

	this.NavigationType;
	this.TransitionType;
	this.HoldTime;

	this.CurrentLeft;
	this.TargetLeft;
	this.StartTime;
	this.TimeOutID;
	this.Sliding;
	this.CallBackFunction;


	//#region Setup
	this.Setup = function (sIdentifier, iPageCount, iSeconds, sNavigationType, sTransitionType, iHoldTime) {

		this.Identifier = sIdentifier;
		this.Holder = int.f.GetObject('divImageRotatorImages_' + me.Identifier);
		this.PageCount = iPageCount;
		this.Seconds = iSeconds;
		this.CurrentPage = 1;
		this.CurrentLeft = 0;
		this.TimeOutID = -1;
		this.NavigationType = sNavigationType;
		this.TransitionType = sTransitionType;
		this.HoldTime = iHoldTime;

		//set to auto rotate
		if (me.TransitionType == 'slide') {
			setTimeout(this.Object + '.Forward();', 5000);
		}
		else if (me.TransitionType == 'fade') {
			this.Fade();
		}

		//Since we need this to be responsive, we will dynamically set the width of each image based on the number of images we have

		//1.Get the images
		var aImageRotatorImages = int.f.GetObjectsByIDPrefix('imgImageRotatorImage_' + me.Identifier, 'img', 'divImageRotatorImages_' + me.Identifier);

		//2.Loop through those images
		for (var i = 0; i < aImageRotatorImages.length; i++) {
			aImageRotatorImages[i].style.width = (100 / iPageCount) + '%'
		}
		//

	}

	//#endregion


	//#region Forward
	this.Forward = function () {

		//if still sliding return
		if (me.Sliding) return;

		//clear current timeout
		clearTimeout(me.TimeOutID);

		//work out target left
		if (me.CurrentPage < me.PageCount) {
			me.TargetLeft = me.CurrentLeft - 100;
			me.CurrentPage += 1;
		}
		else {
			me.TargetLeft = 0;
			me.CurrentPage = 1;
		}

		//reset start time
		this.StartTime = new Date();

		//slide
		this.Slide();

	}

	//#endregion


	//#region Backward

	this.Backward = function () {

		//if still sliding return
		if (me.Sliding) return;

		//clear current timeout
		clearTimeout(me.TimeOutID);

		//work out target left
		if (me.CurrentPage > 1) {
			me.TargetLeft = me.CurrentLeft + 100;
			me.CurrentPage -= 1;
		}
		else {
			me.TargetLeft = -((me.PageCount - 1) * 100);
			me.CurrentPage = me.PageCount;
		}

		//reset start time
		this.StartTime = new Date();


		//slide
		this.Slide();


	}

	//#endregion


	//#region Fade

	this.Fade = function () {

		var IDBase = 'aImageRotatorImage_' + me.Identifier + '_'

		if (me.PageCount > 1) {
			me.HoldTime = me.HoldTime == undefined ? 2 : parseInt(me.HoldTime);
			me.CurrentPage = me.CurrentPage == undefined ? 0 : me.CurrentPage;

			if (me.CurrentPage == 0) {

				me.CurrentPage = 1;

				var oFirst = int.f.GetObject(IDBase + me.CurrentPage);
				oFirst.style.zIndex = 100;

			}
			else {

				var oFadeOut = int.f.GetObject(IDBase + me.CurrentPage);
				if (oFadeOut == null) {
					return false;
				}

				me.CurrentPage += 1;
				me.CurrentPage = me.CurrentPage > me.PageCount ? 1 : me.CurrentPage;

				var oFadeIn = int.f.GetObject(IDBase + me.CurrentPage);

				int.e.FadeOut(oFadeOut);
				int.e.FadeIn(oFadeIn);

				oFadeOut.style.zIndex = 0;
				oFadeIn.style.zIndex = 100;

			}

			setTimeout(this.Object + '.Fade()', me.HoldTime * 1000);
		}

	}

	//#endregion


	//#region Slide

	this.Slide = function () {

		me.Sliding = true;

		//set class of number link if nav typ is numbers
		if (me.NavigationType.indexOf('numbers') != -1) {
			//remove selected class on slider links
			var aSelectedLinks = int.f.GetElementsByClassName('a', 'selected', 'divImageRotatorNumbers_' + me.Identifier);
			for (var i = 0; i < aSelectedLinks.length; i++) {
				int.f.RemoveClass(aSelectedLinks[i], 'selected');
			}

			//set current selected image
			int.f.AddClass('aImageRotatorNumber_' + me.CurrentPage, 'selected');
		}

		var nFractionDone = Math.abs((new Date() - me.StartTime) / me.Seconds / 1000);
		if (nFractionDone < 1) {
			me.Holder.style.left = me.CurrentLeft + (me.TargetLeft - me.CurrentLeft) * Math.sin(Math.PI / 2 * nFractionDone) + '%';
			me.TimeOutID = setTimeout(this.Object + '.Slide();', 20);
		}
		else {
			me.Holder.style.left = me.TargetLeft + '%';
			me.CurrentLeft = me.TargetLeft;
			me.Sliding = false;
			me.TimeOutID = setTimeout(this.Object + '.Forward();', 5000);
		}
	}

	//#endregion


	//#region Select Image

	this.SelectImage = function (iImage) {

		clearTimeout(me.TimeOutID);

		me.CurrentPage = iImage;
		me.TargetLeft = -((iImage - 1) * 100);

		this.StartTime = new Date();
		this.Slide();

	}

	//#endregion


}
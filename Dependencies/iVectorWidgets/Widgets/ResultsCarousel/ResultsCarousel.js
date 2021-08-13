var ResultsCarousel = ResultsCarousel || {}

var ResultsCarousel = ResultsCarousel;
	ResultsCarousel.Rotator;
	ResultsCarousel.Mode;
	ResultsCarousel.StartPage;
	ResultsCarousel.CurrentPage;
	ResultsCarousel.TotalPages;
	ResultsCarousel.PageWidth;

	ResultsCarousel.StartTime;
	ResultsCarousel.Seconds;
	ResultsCarousel.CurrentLeft = 0;
	ResultsCarousel.TargetLeft;
	ResultsCarousel.UpdateFunction

	ResultsCarousel.Holder;

	ResultsCarousel.SearchAgain = false;

	ResultsCarousel.Setup = function (sMode, iCurrentPage, iTotalPages, iPageWidth, oUpdateFunction) {
		ResultsCarousel.Mode = sMode;
		ResultsCarousel.StartPage = iCurrentPage;
		ResultsCarousel.CurrentPage = iCurrentPage;
		ResultsCarousel.TotalPages = iTotalPages;
		ResultsCarousel.PageWidth = iPageWidth;

		ResultsCarousel.Holder = int.f.GetObject('divResultsCarouselItems');

		ResultsCarousel.CurrentLeft = -(iCurrentPage - 1) * iPageWidth;
		ResultsCarousel.Holder.style.left = ResultsCarousel.CurrentLeft + 'px';

		ResultsCarousel.SearchAgain = int.f.GetValue('hidCarouselSearchAgain');

		ResultsCarousel.UpdateFunction = (oUpdateFunction != undefined ? oUpdateFunction : FlightResults.UpdateResults);

		ResultsCarousel.ScrollCenter();
	}

	ResultsCarousel.ScrollCenter = function () {
		var middle = (document.getElementById('divResultsCarouselItems').clientWidth - document.getElementById('divResultsCarouselContent').clientWidth) / 2;
		$('#divResultsCarouselContent').scrollLeft(middle);		
	}


	ResultsCarousel.Filter = function (sDate) {
		int.ff.Call('=iVectorWidgets.ResultsCarousel.Filter', ResultsCarousel.FilterComplete, sDate);
	}


	ResultsCarousel.FilterComplete = function (sHTML) {
		ResultsCarousel.UpdateFunction();
		int.f.SetHTML('divResultsCarouselItems', sHTML);

		ResultsCarousel.RedrawFilters();
	};

	ResultsCarousel.RedrawFilters = function () {

		if (typeof FlightResultsFilter !== 'undefined') {
		    FlightResultsFilter.RedrawFilters();
		};
		
	};


	// search flights on a different departure date and reload carousel and results
	ResultsCarousel.ReSearch = function (sDate) {
		WaitMessage.Show('Search');
		int.ff.Call('=iVectorWidgets.ResultsCarousel.ReSearch', ResultsCarousel.ReSearchComplete, sDate);
	}



	// reload the carousel and result HTML
	ResultsCarousel.ReSearchComplete = function (sJSON) {


		WaitMessage.Hide();
		WaitMessage.Suppress = true;

		var oReturn = eval('(' + sJSON + ')');

		var sRedirectURL = '';

		if (oReturn.SearchMode == 'FlightPlusHotel' && oReturn.PropertyCount > 0 && oReturn.FlightCount > 0) {
			sRedirectURL = int.f.GetValue('hidFlightPlusHotelRedirectURL');
			web.Window.Redirect(sRedirectURL != '' ? sRedirectURL : '/search-results');
		}
		else if (oReturn.SearchMode == 'HotelOnly' && oReturn.PropertyCount > 0) {
			web.Window.Redirect('/search-results');
		}
		else if (oReturn.SearchMode == 'FlightOnly' && oReturn.FlightCount > 0) {
			sRedirectURL = int.f.GetValue('hidFlightOnlyRedirectURL');
			web.Window.Redirect(sRedirectURL != '' ? sRedirectURL : '/flight-results');
		}
		else if ((oReturn.SearchMode == 'FlightPlusHotel' || sSearchMode == 'FlightOnly') && oReturn.FlightCarouselCount > 0) {
			web.Window.Redirect('/search-results');
		}
		else {
			web.InfoBox.Show(int.f.GetValue('hidWarning_NoSearchResults'), 'warning');
		}

	}


	ResultsCarousel.Forward = function (iDays) {

		if (ResultsCarousel.SearchAgain == 'True') {

			var iDaysPerPage = int.f.GetValue('hidCarouselDaysPerPage');
			ResultsCarousel.Search(iDaysPerPage);

		}
		else {

			//if still sliding return
			if (ResultsCarousel.Sliding) return;

			//set target left and update current page			
			ResultsCarousel.TargetLeft = ResultsCarousel.CurrentLeft - ResultsCarousel.PageWidth;
			ResultsCarousel.CurrentPage += 1;

			//only show link if not on last page
			int.f.ShowIf('aResultsCarouselLeft', ResultsCarousel.CurrentPage > 1);
			int.f.ShowIf('aResultsCarouselRight', ResultsCarousel.CurrentPage < ResultsCarousel.TotalPages);

			//slide
			ResultsCarousel.StartTime = new Date();
			ResultsCarousel.Slide();
		}
	}


	ResultsCarousel.Backward = function (iDays) {

		if (ResultsCarousel.SearchAgain == 'True') {

			var iDaysPerPage = int.f.GetValue('hidCarouselDaysPerPage');
			ResultsCarousel.Search(-iDaysPerPage);

		}
		else {

			//if still sliding return
			if (ResultsCarousel.Sliding) return;

			//set target left and update current page			
			ResultsCarousel.TargetLeft = ResultsCarousel.CurrentLeft + ResultsCarousel.PageWidth;
			ResultsCarousel.CurrentPage -= 1;

			//only show link if not on first page
			int.f.ShowIf('aResultsCarouselLeft', ResultsCarousel.CurrentPage > 1);
			int.f.ShowIf('aResultsCarouselRight', ResultsCarousel.CurrentPage < ResultsCarousel.TotalPages);

			//slide
			ResultsCarousel.StartTime = new Date();
			ResultsCarousel.Slide();
		}

	}


	ResultsCarousel.Slide = function () {

		ResultsCarousel.Sliding = true;

		var nFractionDone = Math.abs((new Date() - ResultsCarousel.StartTime) / ResultsCarousel.Seconds / 1000);
		if (nFractionDone < 1) {
			ResultsCarousel.Holder.style.left = ResultsCarousel.CurrentLeft + (ResultsCarousel.TargetLeft - ResultsCarousel.CurrentLeft) * Math.sin(Math.PI / 2 * nFractionDone) + 'px';
		}
		else {
			ResultsCarousel.Holder.style.left = ResultsCarousel.TargetLeft + 'px';
			ResultsCarousel.CurrentLeft = ResultsCarousel.TargetLeft;
			ResultsCarousel.Sliding = false;
		}
	}

	ResultsCarousel.Search = function (iDays) {

		WaitMessage.Show(
				'Search',
				function () {
					int.ff.Call('=iVectorWidgets.ResultsCarousel.SearchAgain', ResultsCarousel.SearchComplete, iDays);
				}
			);

	}

	ResultsCarousel.SearchComplete = function (sJSON) {


		//parse results and redirect to appropriate page
		var oReturn = JSON.parse(sJSON);

		var sSearchMode = int.f.GetValue('hidSearchMode');

		if (sSearchMode == 'FlightPlusHotel' && oReturn.PropertyCount > 0 && oReturn.FlightCount > 0) {
			var sRedirectURL = int.f.GetValue('hidFlightPlusHotelRedirectURL');
			web.Window.Redirect(sRedirectURL != '' ? sRedirectURL : int.f.GetValue('hidFlightPlusHotelURL'));
		}
		else if (sSearchMode == 'HotelOnly' && oReturn.PropertyCount > 0) {
			web.Window.Redirect('/search-results');
		}
		else if (sSearchMode == 'FlightOnly' && oReturn.FlightCount > 0) {
			web.Window.Redirect('/flight-results');
		}
		else if (sSearchMode == 'TransferOnly' && oReturn.TransferCount > 0) {
			web.Window.Redirect('/booking-summary');
		}
		else if ((sSearchMode == 'FlightPlusHotel' || sSearchMode == 'FlightOnly') && oReturn.FlightCarouselCount > 0) {
			web.Window.Redirect(int.f.GetValue('hidFlightPlusHotelURL'));
		}
		else {
			WaitMessage.Hide();
			WaitMessage.Suppress = true;
			web.InfoBox.Show(int.f.GetValue('hidWarning_NoSearchResults'), 'warning');
		}

	}






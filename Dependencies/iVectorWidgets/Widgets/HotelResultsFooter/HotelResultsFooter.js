var HotelResultsFooter = new function () {

	var me = this;


	this.Setup = function () {

		int.f.AttachEvent('btnBackToTop', 'click', function () {
			window.scrollTo(0, 0);
		});

	}


	this.Hide = function () {
		int.f.Hide('divHotelResultsFooter');
	}

	this.Show = function () {
		int.f.Show('divHotelResultsFooter');
	}



	//#region Paging		
	this.CurrentPage = 1

	this.PreviousPage = function () {
		me.SelectPage(me.CurrentPage - 1);
	};
	this.NextPage = function () {
		me.SelectPage(me.CurrentPage + 1);
	};

	this.SelectPage = function (iPage) {
		me.CurrentPage = iPage;
		int.ff.Call(
			'=iVectorWidgets.HotelResultsFooter.SelectPage',
			function (sHTML) {
				int.f.SetHTML('divPagingBottom', sHTML);
				int.f.SetHTML('divPagingTop', sHTML);
				int.e.ScrollIntoView('divPagingTop', 10, 2);
				HotelResults.UpdateResults();
			},
			iPage
		);
	}

	this.Update = function () {
		me.CurrentPage = 1;
		int.ff.Call('=iVectorWidgets.HotelResultsFooter.SelectPage', HotelResultsFooter.UpdateComplete, me.CurrentPage);
	}


	this.UpdateComplete = function (sHTML) {
		int.f.ShowIf('divHotelResultsFooter', sHTML != '');
		int.f.SetHTML('divPagingBottom', sHTML);
	}

	//#endregion


}
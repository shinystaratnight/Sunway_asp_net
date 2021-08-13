var MapWidget = new function () {

	/* Properties */
	var me = this;
	this.CurrentMarkers;
	this.GoogleMap;
	this.SinglePropertyView = false;

	this.iPopupX;
	this.iPopupY;

	this.iClusterPopupX;
	this.iClusterPopupY;


	//#region Setup

	this.Setup = function (containerid, theme) {
		//save popup co-ords
		me.iPopupX = (int.f.GetValue('hidPopupX') != '') ? int.f.GetValue('hidPopupX') : -150;
		me.iPopupY = (int.f.GetValue('hidPopupY') != '') ? int.f.GetValue('hidPopupY') : -30;
		me.iClusterPopupX = (int.f.GetValue('hidClusterPopupX') != '') ? int.f.GetValue('hidClusterPopupX') : -110;
		me.iClusterPopupX = (int.f.GetValue('hidClusterPopupY') != '') ? int.f.GetValue('hidClusterPopupY') : -30;

		//set container and theme
		containerid = containerid == undefined ? 'divMap' : containerid;
		theme = theme == undefined ? int.f.GetValue('hidMapTheme') : theme;

		//setup map
		this.GoogleMap = new GoogleMap(int.f.GetObject(containerid), null, this.OnZoom, this.OnBoundsChange, null, 'Full', this.OnIdle);

		me.AddMapIcons(int.f.GetValue('hidMapIcons'));
	};


	this.AddMapIcons = function (sJSON) {
		//add new icons
		for (var oMapIcons = JSON.parse(sJSON), i = 0; i < oMapIcons.length; i++) {
			me.GoogleMap.AddIcon(
				oMapIcons[i].Name,
				oMapIcons[i].URL,
				oMapIcons[i].Width,
				oMapIcons[i].Height,
				oMapIcons[i].AnchorX,
				oMapIcons[i].AnchorY,
				oMapIcons[i].OriginX,
				oMapIcons[i].OriginY
			);
		}

		//if we have map points add to map
		if (int.f.GetValue('hidMapPoints') != '') me.GetMapComplete(int.f.GetValue('hidMapPoints'));
	}

	//#endregion


	//#region Events

	this.OnZoom = function (iZoomLevel, bZoomChangedByUser) {
		if (bZoomChangedByUser) {
			int.f.HidePopup();
			oClusterMap();
		};
	};

	this.OnBoundsChange = function (iZoomLevel, bZoomChangedByUser) {
		oClusterMap();
		int.f.HidePopup();
		web.Tooltip.Hide();
	};

	this.OnIdle = function () {
		if (me.SinglePropertyView == false) {
			me.ResetMarkers();
			oClusterMap();
		}
		int.f.HidePopup();
		web.Tooltip.Hide();
	};


	/* Property Cluster Event Handling */
	this.PropertyClusterEvents = new function () {

		this.OnClick = function (oMarker) {
			me.GoogleMap.ZoomToMarker(oMarker);
		};

		this.OnMouseOver = function (oMarker) {
			web.Tooltip.Hide();
			web.Tooltip.Show('', '<em>' + oMarker.MarkerCount + ' hotels</em> - Click on the icon to see this location', 'top',
					me.GoogleMap.GetMarkerScreenPosition(oMarker), me.iClusterPopupX, me.iClusterPopupY);
		};

		this.OnMouseOut = function (oMarker) {
			web.Tooltip.Hide();
		};

	};


	/* Marker Event Handling */
	this.DefaultMarkerEvents = new function () {

		this.OnClick;
		this.OnMouseOver;
		this.OnMouseOut;


		this.Setup = function (oOnClick, oOnMouseOver, oOnMouseOut) {

			if (oOnClick) this.OnClick = oOnClick;
			if (oOnMouseOver) this.OnMouseOver = oOnMouseOver;
			if (oOnMouseOut) this.OnMouseOut = oOnMouseOut;

		}


		//		this.OnClick = function (oMarker) {
		//			var oMapPoint = Map.GetMapPoint(oMarker.MarkerID.split('_')[1]);
		//			HotelResults.ShowDetailsPopup(oMapPoint.ID);
		//		};

		//		this.OnMouseOver = function (oMarker) {
		//			web.Tooltip.Hide();

		//			var iPropertyID = oMarker.MarkerID.split('_')[1];
		//			var oSetPopupContent = function (sHTML) {
		//				web.Tooltip.Show('', sHTML, 'top', me.GoogleMap.GetMarkerScreenPosition(oMarker), me.iPopupX, me.iPopupY, 'mapHover');
		//			};

		//			int.ff.Call('=iVectorWidgets.HotelResults.MapPopup', oSetPopupContent, iPropertyID);
		//		};

		//		this.OnMouseOut = function (oMarker) {
		//			web.Tooltip.Hide();
		//		};

	};

	//#endregion


	//#region Hotel Results Map

	this.CreateMapFromFilteredHotels = function () {
        int.ff.Call('=iVectorWidgets.Map.CreateMapFromFilteredHotels', MapWidget.GetMapComplete);
	}

	this.MapZoomOnSpecificProperty = function (iLatitude, iLongitude) {
		int.ff.Call('=iVectorWidgets.Map.CreateMapFromFilteredHotels', function (sJSON) { MapWidget.GetMapComplete(sJSON, iLatitude, iLongitude); });
	}

	this.Show = function () {
		//need to know if we're generating a map with more than one pin.
		this.SinglePropertyView = false;

		int.f.Show('divHotelResultsMapHolder');
		int.f.Show('divHotelResultsMap');
		HotelResultsFilter.MapView = true;
		MapWidget.Setup('divHotelResultsMap', int.f.GetValue('hidMapTheme'));

		//If there is a key in the theme show that too
		if (int.f.GetObject('divHotelResultsMapKey') != undefined) {
			int.f.Show('divHotelResultsMapKey');
		}
	}

	this.Hide = function () {
		int.f.Hide('divHotelResultsMap');
		HotelResultsFilter.MapView = false;
		//If there is a key in the theme show that too
		if (int.f.GetObject('divHotelResultsMapKey') != undefined) {
			int.f.Hide('divHotelResultsMapKey');
		}
		//hide any tooltips still showing
		for (var aTooltips = int.f.GetElementsByClassName('div', 'tooltip'), i = 0; i < aTooltips.length; i++) int.f.Hide(aTooltips[i]);
	}

	//#endregion


	//#region Support Functions

	this.GetMapComplete = function (sJSON, iLatitude, iLongitude) {
		var oMapReturn = JSON.parse(sJSON);
		me.CurrentMarkers = oMapReturn.Markers;

		//set popup co-ords
		me.iPopupX = oMapReturn.PopupX;
		me.iPopupY = oMapReturn.PopupY;
		me.iClusterPopupX = oMapReturn.ClusterPopupX;
		me.iClusterPopupY = oMapReturn.ClusterPopupY;

		me.ResetMarkers();
		oClusterMap();

		me.GoogleMap.RemoveOutliers(2, 5);

		//if we are passing in lat/long then we want to center manually
		if (iLatitude != undefined && iLongitude != undefined) {
			me.GoogleMap.SetCentre(iLatitude, iLongitude);
			me.GoogleMap.SetZoom(13);
		}
		else {
			me.GoogleMap.CentreAndZoom(13);
		}
		
	}

	this.GetMapPoint = function (iID) {
		var oMapPoint;
		for (var i = 0; i < me.CurrentMarkers.length; i++) {
			if (me.CurrentMarkers[i].ID == iID) oMapPoint = me.CurrentMarkers[i];
		}
		return oMapPoint;
	}


	this.ResetMarkers = function () {
		//clear current markers
		me.GoogleMap.ClearMarkers();

		//add new markers
		if (me.CurrentMarkers != undefined && me.CurrentMarkers != null) {
			for (var i = 0; i < me.CurrentMarkers.length; i++) {
				me.GoogleMap.AddMarker(me.CurrentMarkers[i].Latitude, me.CurrentMarkers[i].Longitude, 'markerid_' + me.CurrentMarkers[i].ID,
										me.CurrentMarkers[i].Type, me.DefaultMarkerEvents.OnClick, me.DefaultMarkerEvents.OnMouseOver, me.DefaultMarkerEvents.OnMouseOut);
			}
		};
	};

	this.AddSingleMarker = function (iLatitude, iLongitude, sMarkerID, sType) {
		this.SinglePropertyView = true;
		//clear current markers
		me.GoogleMap.ClearMarkers();
		//add marker
		me.GoogleMap.AddMarker(iLatitude, iLongitude, sMarkerID, sType);
		//centre and zoom
		me.GoogleMap.CentreAndZoom(13);
	}


	//#endregion


	//#region Cluster Map


	var oClusterMap = function () {

		var oClusteringPredicate = function (oMarker) {
			return oMarker.MarkerID.indexOf('markerid_') == 0;
		};

		var oClusterDefiner = function (sClusterMarkerID, iMarkerCount) {
			var oClusterDefinition = {
				OnClick: me.PropertyClusterEvents.OnClick,
				OnMouseOver: me.PropertyClusterEvents.OnMouseOver,
				OnMouseOut: me.PropertyClusterEvents.OnMouseOut
			};

			if (iMarkerCount >= 100) {
				oClusterDefinition.IconKey = '100plus';
			}
			else if (iMarkerCount >= 50) {
				oClusterDefinition.IconKey = '50plus';
			}
			else if (iMarkerCount >= 20) {
				oClusterDefinition.IconKey = '20plus';
			}
			else if (iMarkerCount >= 10) {
				oClusterDefinition.IconKey = '10plus';
			}
			else if (iMarkerCount >= 5) {
				oClusterDefinition.IconKey = '5plus';
			};

			return oClusterDefinition;
		};

		setTimeout(
            function () {
            	me.GoogleMap.Cluster('50px', 'hotelcluster_', oClusteringPredicate, oClusterDefiner, 5);
            },
            500
		);
	};

	//#endregion

}
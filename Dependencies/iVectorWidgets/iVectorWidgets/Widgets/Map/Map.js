
var Map = new function () {

	/* Properties */
	var me = this;
	this.CurrentMarkers;
	this.GoogleMap;

	/* Functions */
	this.Setup = function (containerid) {

		this.GoogleMap = new GoogleMap(int.f.GetObject(containerid), null, oOnZoom, oOnBoundsChange, null, 'Full');
		//		this.GoogleMap.AddIcon('current', 'IMAGEPATH', 32, 52, 16, 47);
		//		this.GoogleMap.AddIcon('hotel', 'IMAGEPATH', 23, 38, 12, 35);

	};

	//#region Events

	var oOnZoom = function (iZoomLevel, bZoomChangedByUser) {
		if (bZoomChangedByUser) {
			int.f.HidePopup();
		};
	};

	var oOnBoundsChange = function (iZoomLevel, bZoomChangedByUser) {
		int.f.HidePopup();
	};

	/* Property Cluster Event Handling */
	this.PropertyClusterEvents = new function () {

		this.OnClick = function (oMarker) {
			me.GoogleMap.ZoomToMarker(oMarker);
		};

		this.OnMouseOver = function (oMarker) {
			int.f.HidePopup();
			int.f.ShowPopup(me.GoogleMap.GetMarkerScreenPosition(oMarker), 'multipleproperties',
                '<em>' + oMarker.MarkerCount + ' hotels:</em> Click on the icon to see this location', '', false, -90, -37);
		};

		this.OnMouseOut = function (oMarker) {
			int.f.HidePopup();
		};

	};


	/* Marker Event Handling */
	this.DefaultMarkerEvents = new function () {

		this.OnClick = function (oMarker) {

//			f.HidePopup();

//			var iPropertyID = oMarker.MarkerID.split('_')[1];
//			f.ShowPopup(oMarker.ParentMap.GetMarkerScreenPosition(oMarker), 'mapHover', '<div class="top"> </div><img class="wait" src="/Images/waiting.gif" alt="Please wait" />', '', false, -179, -46);
//			var oPopup = f.GetElementsByClassName('div', 'mapHover')[0];

//			var oSetPopupContent = function (sHTML) {
//				f.SetHTML(oPopup, sHTML);
//			};

//			ff.Call('Widgets.Map.PropertyMapPopupDetails', oSetPopupContent, iPropertyID);

		};

		this.OnMouseOver = function (oMarker) {

		};

		this.OnMouseOut = function (oMarker) {

		};

	};

	//#endregion

	this.GetMapComplete = function () {

		var oMarkers = eval('(' + sJSON + ')');
		me.CurrentMarkers = oMarkers;
		me.ResetMarkers();

	}

	this.ResetMarkers = function () {

		//clear current markers
		me.GoogleMap.ClearMarkers();

		//add new markers
		if (me.CurrentMarkers != undefined && me.CurrentMarkers != null) {
			for (var i = 0; i < me.CurrentMarkers.Markers.length; i++) {

				me.GoogleMap.AddMarker(me.CurrentMarkers.Markers[i].Latitude, me.CurrentMarkers.Markers[i].Longitude, 'markerid_' + me.CurrentMarkers.Markers[i].ID,
				                    me.CurrentMarkers.Markers[i].Type, me.DefaultMarkerEvents.OnClick, me.DefaultMarkerEvents.OnMouseOver, me.DefaultMarkerEvents.OnMouseOut);

			}
		};

	};

}
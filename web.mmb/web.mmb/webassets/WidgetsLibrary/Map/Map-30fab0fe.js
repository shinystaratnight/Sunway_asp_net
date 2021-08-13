var Map=new function(){var e=this;this.CurrentMarkers,this.GoogleMap,this.SinglePropertyView=!1,this.iPopupX,this.iPopupY,this.iClusterPopupX,this.iClusterPopupY,this.Setup=function(t,i){e.iPopupX=""!=int.f.GetValue("hidPopupX")?int.f.GetValue("hidPopupX"):-150,e.iPopupY=""!=int.f.GetValue("hidPopupY")?int.f.GetValue("hidPopupY"):-30,e.iClusterPopupX=""!=int.f.GetValue("hidClusterPopupX")?int.f.GetValue("hidClusterPopupX"):-110,e.iClusterPopupX=""!=int.f.GetValue("hidClusterPopupY")?int.f.GetValue("hidClusterPopupY"):-30,t=void 0==t?"divMap":t,i=void 0==i?int.f.GetValue("hidMapTheme"):i,this.GoogleMap=new GoogleMap(int.f.GetObject(t),null,this.OnZoom,this.OnBoundsChange,null,"Full",this.OnIdle),e.AddMapIcons(int.f.GetValue("hidMapIcons"))},this.AddMapIcons=function(t){for(var i=JSON.parse(t),o=0;o<i.length;o++)e.GoogleMap.AddIcon(i[o].Name,i[o].URL,i[o].Width,i[o].Height,i[o].AnchorX,i[o].AnchorY,i[o].OriginX,i[o].OriginY);""!=int.f.GetValue("hidMapPoints")&&e.GetMapComplete(int.f.GetValue("hidMapPoints"))},this.OnZoom=function(e,i){i&&(int.f.HidePopup(),t())},this.OnBoundsChange=function(e,i){t(),int.f.HidePopup(),web.Tooltip.Hide()},this.OnIdle=function(){0==e.SinglePropertyView&&(e.ResetMarkers(),t()),int.f.HidePopup(),web.Tooltip.Hide()},this.PropertyClusterEvents=new function(){this.OnClick=function(t){e.GoogleMap.ZoomToMarker(t)},this.OnMouseOver=function(t){web.Tooltip.Hide(),web.Tooltip.Show("","<em>"+t.MarkerCount+" hotels</em> - Click on the icon to see this location","top",e.GoogleMap.GetMarkerScreenPosition(t),e.iClusterPopupX,e.iClusterPopupY)},this.OnMouseOut=function(e){web.Tooltip.Hide()}},this.DefaultMarkerEvents=new function(){this.OnClick,this.OnMouseOver,this.OnMouseOut,this.Setup=function(e,t,i){e&&(this.OnClick=e),t&&(this.OnMouseOver=t),i&&(this.OnMouseOut=i)}},this.CreateMapFromFilteredHotels=function(){int.ff.Call("=iVectorWidgets.Map.CreateMapFromFilteredHotels",Map.GetMapComplete)},this.MapZoomOnSpecificProperty=function(e,t){int.ff.Call("=iVectorWidgets.Map.CreateMapFromFilteredHotels",function(i){Map.GetMapComplete(i,e,t)})},this.Show=function(){this.SinglePropertyView=!1,int.f.Show("divHotelResultsMapHolder"),int.f.Show("divHotelResultsMap"),HotelResultsFilter.MapView=!0,Map.Setup("divHotelResultsMap",int.f.GetValue("hidMapTheme")),void 0!=int.f.GetObject("divHotelResultsMapKey")&&int.f.Show("divHotelResultsMapKey")},this.Hide=function(){int.f.Hide("divHotelResultsMap"),HotelResultsFilter.MapView=!1,void 0!=int.f.GetObject("divHotelResultsMapKey")&&int.f.Hide("divHotelResultsMapKey");for(var e=int.f.GetElementsByClassName("div","tooltip"),t=0;t<e.length;t++)int.f.Hide(e[t])},this.GetMapComplete=function(i,o,r){var n=JSON.parse(i);e.CurrentMarkers=n.Markers,e.iPopupX=n.PopupX,e.iPopupY=n.PopupY,e.iClusterPopupX=n.ClusterPopupX,e.iClusterPopupY=n.ClusterPopupY,e.ResetMarkers(),t(),e.GoogleMap.RemoveOutliers(2,5),void 0!=o&&void 0!=r?(e.GoogleMap.SetCentre(o,r),e.GoogleMap.SetZoom(13)):e.GoogleMap.CentreAndZoom(13)},this.GetMapPoint=function(t){for(var i,o=0;o<e.CurrentMarkers.length;o++)e.CurrentMarkers[o].ID==t&&(i=e.CurrentMarkers[o]);return i},this.ResetMarkers=function(){if(e.GoogleMap.ClearMarkers(),void 0!=e.CurrentMarkers&&null!=e.CurrentMarkers)for(var t=0;t<e.CurrentMarkers.length;t++)e.GoogleMap.AddMarker(e.CurrentMarkers[t].Latitude,e.CurrentMarkers[t].Longitude,"markerid_"+e.CurrentMarkers[t].ID,e.CurrentMarkers[t].Type,e.DefaultMarkerEvents.OnClick,e.DefaultMarkerEvents.OnMouseOver,e.DefaultMarkerEvents.OnMouseOut)},this.AddSingleMarker=function(t,i,o,r){this.SinglePropertyView=!0,e.GoogleMap.ClearMarkers(),e.GoogleMap.AddMarker(t,i,o,r),e.GoogleMap.CentreAndZoom(13)};var t=function(){var t=function(e){return 0==e.MarkerID.indexOf("markerid_")},i=function(t,i){var o={OnClick:e.PropertyClusterEvents.OnClick,OnMouseOver:e.PropertyClusterEvents.OnMouseOver,OnMouseOut:e.PropertyClusterEvents.OnMouseOut};return i>=100?o.IconKey="100plus":i>=50?o.IconKey="50plus":i>=20?o.IconKey="20plus":i>=10?o.IconKey="10plus":i>=5&&(o.IconKey="5plus"),o};setTimeout(function(){e.GoogleMap.Cluster("50px","hotelcluster_",t,i,5)},500)}};
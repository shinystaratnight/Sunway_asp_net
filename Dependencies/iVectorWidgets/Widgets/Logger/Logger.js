var Logger = Logger || {}

Logger.LogEvent = function (sEventType, nValue) {
	int.ff.Call('=iVectorWidgets.Logger.LogEvent', function () { }, sEventType, nValue);
};
web.PubSub.subscribe('PaymentBreakdown_Setup', function () {
	Logger.LogEvent('BookDisplayed', 0);
});
web.PubSub.subscribe('HotelResults_Setup', function () {
	Logger.LogEvent('ResultsDisplayed', int.f.GetValue('hidResultsDisplayed'));
	Logger.LogEvent('HotelsDisplayed', int.f.GetValue('hidHotelsDisplayed'));
	Logger.LogEvent('SuppliersDisplayed', int.f.GetValue('hidSuppliersDisplayed'));
	Logger.LogEvent('AverageSuppliers', int.f.GetValue('hidAverageSuppliers'));
	Logger.LogEvent('IVectorConnectElapsedTime', int.f.GetValue('hidIVectorConnectElapsedTime'));
	const iElapsedTime = int.f.GetValue('hidRoomMappingElapsedTime');
	if (iElapsedTime > 0) {
		Logger.LogEvent('RoomMappingElapsedTime', iElapsedTime);
	}
});
web.PubSub.subscribe('HotelPopup_ShowGroupPopupDone', function () {
	Logger.LogEvent('InterimClickThrough', int.f.GetValue('hidOptionsCount'));
});
web.PubSub.subscribe('HotelResults_RoomSelected', function () {
	Logger.LogEvent('ClickThrough', 0);
});
web.PubSub.subscribe('BookingConfirmation_Setup', function () {
	Logger.LogEvent('BookingRevenue', int.f.GetValue('hidTotalPrice'));
	Logger.LogEvent('BookingMargin', int.f.GetValue('hidTotalMargin'));
})
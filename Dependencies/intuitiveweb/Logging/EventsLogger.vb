Imports Intuitive

Public Class EventsLogger
	Private sConnectString As String
	Private sSessionID As String
	Private iCount As Integer
	Private sCurrentDomain As String

	Public Sub New(sConnectString As String, sSessionID As String)
		Me.sConnectString = sConnectString
		Me.sSessionID = sSessionID
		iCount = 0

#If DEBUG Then
		sCurrentDomain = System.Configuration.ConfigurationManager.AppSettings("WebsiteURL")
#Else
		sCurrentDomain = Functions.GetBaseURL()
#End If

	End Sub

	Public Sub Log(eType As eEvent, nVal As Decimal)
		If eType = eEvent.Search OrElse eType = eEvent.OtherSearch Then
			iCount += 1
		End If

		Dim bUseRoomMapping As Boolean = CType(HttpContext.Current.Session("__booking_useroommapping"), Boolean)

		iCount = SQL.ExecuteSingleValueWithConnectString(sConnectString, "LogEvent", SQL.GetSqlValues(sSessionID,
																									   iCount,
																									   bUseRoomMapping,
																									   eType.ToSafeString(),
																									   nVal,
																									   sCurrentDomain))
	End Sub

	Public Enum eEvent
		Search
		OtherSearch
		ResultsDisplayed
		HotelsDisplayed
		SuppliersDisplayed
		AverageSuppliers
		RoomMappingElapsedTime
		IVectorConnectElapsedTime
		WebsitePostConnectElapsedTime
		InterimClickThrough
		ClickThrough
		BookDisplayed
		BookingRevenue
		BookingMargin
	End Enum

End Class

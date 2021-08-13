Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.EventsLogger
Imports Intuitive.Web.Widgets

Public Class Logger
	Inherits WidgetBase

	Private ReadOnly sSessionID As String
	Private ReadOnly Property oLogger As EventsLogger
		Get
			Dim oLog As EventsLogger = CType(HttpContext.Current.Session("logger"), EventsLogger)
			If oLog Is Nothing Then
				oLog = New EventsLogger(ConfigurationManager.AppSettings("ConnectString").ToSafeString(), sSessionID)
				HttpContext.Current.Session("logger") = oLog
			End If
			Return oLog
		End Get
	End Property

	Public Sub New()
		sSessionID = HttpContext.Current.Session.SessionID
	End Sub

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)
		writer.Write("<input type=""hidden"" id=""hidLogger"" value=""true"" />")
	End Sub

	Public Sub LogEvent(ByVal sEventType As String, ByVal nEventNumber As Decimal)
		oLogger.Log(GetEventType(sEventType), nEventNumber)
	End Sub

	Private Shared Function GetEventType(ByVal sEventType As String) As eEvent
		Select Case sEventType
			Case "Search"
				Return eEvent.Search
			Case "OtherSearch"
				Return eEvent.OtherSearch
			Case "ResultsDisplayed"
				Return eEvent.ResultsDisplayed
			Case "HotelsDisplayed"
				Return eEvent.HotelsDisplayed
			Case "SuppliersDisplayed"
				Return eEvent.SuppliersDisplayed
			Case "AverageSuppliers"
				Return eEvent.AverageSuppliers
			Case "RoomMappingElapsedTime"
				Return eEvent.RoomMappingElapsedTime
			Case "IVectorConnectElapsedTime"
				Return eEvent.IVectorConnectElapsedTime
			Case "WebsitePostConnectElapsedTime"
				Return eEvent.WebsitePostConnectElapsedTime
			Case "InterimClickThrough"
				Return eEvent.InterimClickThrough
			Case "ClickThrough"
				Return eEvent.ClickThrough
			Case "BookDisplayed"
				Return eEvent.BookDisplayed
			Case "BookingRevenue"
				Return eEvent.BookingRevenue
			Case "BookingMargin"
				Return eEvent.BookingMargin
		End Select
	End Function

End Class

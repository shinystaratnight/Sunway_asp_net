Imports Intuitive.Web
Imports Intuitive.Web.Translation
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions

Public Class SearchSummary
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("searchSummary_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("searchSummary_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("searchSummary_customsettings") = value
		End Set

	End Property

#End Region

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. save settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.ControlOverride = SafeString(Settings.GetValue("ControlOverride"))
			.CSSClassOverride = SafeString(Settings.GetValue("CSSClassOverride"))
			.HotelOnlySearchSummary = SafeString(Settings.GetValue("HotelOnlySearchSummary"))
			.FlightPlusHotelSearchSummary = SafeString(Settings.GetValue("FlightPlusHotelSearchSummary"))
			.ExtrasPage = SafeBoolean(Settings.GetValue("ExtrasPage"))
			.FlightPage = SafeBoolean(Settings.GetValue("FlightPage"))
			.FlightPageSearchSummary = SafeString(Settings.GetValue("FlightPageSearchSummary"))
			.TransferPage = SafeBoolean(Settings.GetValue("TransferPage"))
			.TransferPageSearchSummary = SafeString(Settings.GetValue("TransferPageSearchSummary"))
			.TransferReturnString = SafeString(Settings.GetValue("TransferReturnString"))
			.TransferOnlyJourney = SafeBoolean(Settings.GetValue("TransferOnlyJourney"))
			.ShowTotalPages = SafeBoolean(Settings.GetValue("ShowTotalPages"))
			.AlwaysShow = SafeBoolean(Settings.GetValue("AlwaysShow"))
			.ShortMonth = SafeBoolean(Settings.GetValue("ShortMonth"))
			.NextIcon = SafeString(Settings.GetValue("NextIcon"))
			.PreviousIcon = SafeString(Settings.GetValue("PreviousIcon"))
			.NoResultsHide = IIf(Settings.GetValue("NoResultsHide") = "", False, SafeBoolean(Settings.GetValue("NoResultsHide")))
			.Upsell = SafeBoolean(Settings.GetValue("Upsell"))
			.RenderHolderOnly = SafeBoolean(Settings.GetValue("RenderHolderOnly"))

			If Settings.GetValue("PagingTotalLinks") <> "" Then
				.PagingTotalLinks = SafeInt(Settings.GetValue("PagingTotalLinks"))
			End If

		End With

		SearchSummary.CustomSettings = oCustomSettings

		Dim bResultsValid As Boolean = True
		If oCustomSettings.NoResultsHide Then
			If BookingBase.SearchDetails.PropertyResults.TotalHotels = 0 AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then bResultsValid = False
			If (BookingBase.SearchDetails.PropertyResults.TotalHotels = 0 OrElse BookingBase.SearchDetails.FlightResults.TotalFlights = 0) AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then bResultsValid = False
			If BookingBase.SearchDetails.FlightResults.TotalFlights = 0 AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly Then bResultsValid = False
		End If

		If bResultsValid Then

			'exit if we only want to display on transfer only journey
			If Not SearchSummary.CustomSettings.TransferOnlyJourney AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.TransferOnly Then
				Exit Sub
			End If

			'2. build control
			Dim sControlHTML As String = ""

			If SearchSummary.CustomSettings.RenderHolderOnly Then
				If BookingBase.SearchDetails.FlightResults.TotalFlights = 0 AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then
					sControlHTML = "<div id=" & "divSummaryContainer" & "></div>"
				ElseIf BookingBase.SearchDetails.PropertyResults.TotalHotels = 0 AndAlso BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly Then
					sControlHTML = "<div id=" & "divSummaryContainer" & "></div>"
				End If
			Else
				sControlHTML = Me.RenderControlHTML()
			End If

			writer.Write(sControlHTML)

		End If

	End Sub

	Public Function RenderControlHTML() As String

		If SearchSummary.CustomSettings.ControlOverride <> "" Then Me.URLPath = SearchSummary.CustomSettings.ControlOverride
		Dim sControlPath As String

		If Me.URLPath.EndsWith(".ascx") Then
			sControlPath = Me.URLPath
		Else
			sControlPath = Me.URLPath & "/searchsummary.ascx"
		End If

		Dim oControl As New Control
		Try
			If Me.Page Is Nothing Then
				Me.Page = New UI.Page()
			End If
			oControl = Me.Page.LoadControl(sControlPath)
		Catch ex As Exception
			oControl = Me.Page.LoadControl("/widgetslibrary/SearchTool/searchsummary.ascx")
		End Try

		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
		Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oControl)

		Return sControlHTML

	End Function

	Public Shared Function SortResults(ByVal SortBy As String, ByVal SortOrder As String) As String

		'sort results
		BookingBase.SearchDetails.PropertyResults.SortResults(SortBy, SortOrder)

		'return
		Return "success"

	End Function

	Public Shared Function SortFlightResults(ByVal SortBy As String, ByVal SortOrder As String) As String

		'sort results
		BookingBase.SearchDetails.FlightResults.SortResults(SortBy, SortOrder)

		'return
		Return "success"

	End Function

	Public Overridable Function SelectPage(ByVal PageNumber As Integer) As String

		BookingBase.SearchDetails.PropertyResults.CurrentPage = PageNumber

		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = IIf(BookingBase.SearchDetails.PropertyResults.TotalPages = 0, 1, BookingBase.SearchDetails.PropertyResults.TotalPages)
			.TotalLinksToDisplay = SearchSummary.CustomSettings.PagingTotalLinks
			.CurrentPage = PageNumber
			.ScriptPrevious = "SearchSummary.PreviousPage()"
			.ScriptNext = "SearchSummary.NextPage()"
			.ScriptPage = "SearchSummary.SelectPage({0})"
			.ShowTotalPages = SearchSummary.CustomSettings.ShowTotalPages
			.NextIcon = SearchSummary.CustomSettings.NextIcon
			.PreviousIcon = SearchSummary.CustomSettings.PreviousIcon
		End With

		Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oPaging)
		Return Intuitive.Web.Translation.TranslateHTML(sControlHTML)

	End Function

	Public Overridable Function Update() As String

		Dim oUpdateReturn As New UpdateSummaryReturn

		oUpdateReturn.PagingControl = Me.SelectPage(1)
		oUpdateReturn.SummaryHeader = SearchSummary.GenerateSearchSummary()
		oUpdateReturn.HotelCount = BookingBase.SearchDetails.PropertyResults.TotalHotels

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oUpdateReturn)

	End Function

	Public Class UpdateSummaryReturn
		Public SummaryHeader As String
		Public PagingControl As String
		Public HotelCount As Integer
	End Class

#Region "GenerateSearchSummary"

	Public Shared Function GenerateSearchSummary() As String

		Dim oSearchDetails As BookingSearch = BookingBase.SearchDetails

		Dim sb As New System.Text.StringBuilder()

		'set up variables

		'hotel count
		Dim iHotelCount As Integer = BookingBase.SearchDetails.PropertyResults.TotalHotels

		'flight count
		Dim iFlightCount As Integer = BookingBase.SearchDetails.FlightResults.TotalFlights

		'departing airport
		Dim sAirport As String = BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.AirportGroupAndAirport, oSearchDetails.DepartingFromID)

		'arriving destination
		Dim sDestination As String = ""

		'Altered so Destination can take airport as parameter as well
		If oSearchDetails.ArrivingAtID > 1000000 Then
			sDestination = BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.AllAirportGroupAndAirport, oSearchDetails.ArrivingAtID - 1000000)
		ElseIf oSearchDetails.GeographyGroupingID > 0 Then
			sDestination = BookingBase.Lookups.GeographyGroupingNameBy(oSearchDetails.GeographyGroupingID, BookingBase.DisplayLanguageID)
		ElseIf oSearchDetails.ArrivingAtID > 0 Then
			sDestination = BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.Region, oSearchDetails.ArrivingAtID)
		ElseIf oSearchDetails.ArrivingAtID < 0 Then
			sDestination = BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.Resort, oSearchDetails.ArrivingAtID * -1)
		ElseIf oSearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			sDestination = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.ProductAttribute, oSearchDetails.ProductAttributeID)
		End If

		'dates
		Dim sDepartureDate As String = oSearchDetails.DepartureDate.ToString("dd MMMM yyy")
		Dim sReturnDate As String = oSearchDetails.DepartureDate.AddDays(oSearchDetails.Duration).ToString("dd MMMM yyy")

		If CustomSettings.ShortMonth Then
			sDepartureDate = oSearchDetails.DepartureDate.ToString("dd MMM yyy")
			sReturnDate = oSearchDetails.DepartureDate.AddDays(oSearchDetails.Duration).ToString("dd MMM yyy")
		End If

        'duration
        Dim sDuration As String

        Dim iDuration As Integer = If(oSearchDetails.HotelDuration = 0, oSearchDetails.Duration, oSearchDetails.HotelDuration)
        If iDuration = 1 Then
            sDuration = iDuration & " Night"
        Else
            sDuration = iDuration & " Nights"
        End If

		'pax
		Dim sPaxBuilder As String = ""
		If oSearchDetails.TotalAdults = 1 Then
			sPaxBuilder = String.Format("{0} adult", oSearchDetails.TotalAdults)
		Else
			sPaxBuilder = String.Format("{0} adults", oSearchDetails.TotalAdults)
		End If

		If oSearchDetails.TotalChildren > 0 Then
			If oSearchDetails.TotalInfants > 0 Then
				sPaxBuilder += String.Format(", {0} Child", oSearchDetails.TotalChildren)
			Else
				sPaxBuilder += String.Format(" and {0} Child", oSearchDetails.TotalChildren)
			End If

			If oSearchDetails.TotalChildren > 1 Then
				sPaxBuilder += "ren"
			End If

		End If
		If oSearchDetails.TotalInfants > 0 Then
			sPaxBuilder += String.Format(" and {0} Infant", oSearchDetails.TotalInfants)
			If oSearchDetails.TotalInfants > 1 Then
				sPaxBuilder += "s"
			End If
		End If

		sPaxBuilder += "<br />"

		'transfer details
		Dim sTransferPickup As String = ""
		If oSearchDetails.TransferResults.Transfers.Count > 0 Then
			sTransferPickup = oSearchDetails.TransferResults.Transfers(0).DepartureParent
		End If

		Dim sTransferDropOff As String = ""
		If oSearchDetails.TransferResults.Transfers.Count > 0 Then
			sTransferDropOff = oSearchDetails.TransferResults.Transfers(0).ArrivalParent
		End If

		Dim sTransferDepartureDate As String = Format(oSearchDetails.TransferSearch.DepartureDate, "dd MMM yyyy")
		Dim sTransferReturnDate As String = Format(oSearchDetails.TransferSearch.ReturnDate, "dd MMM yyyy")
		Dim sTransferDepartureTime As String = oSearchDetails.TransferSearch.DepartureTime
		Dim sTransferReturnTime As String = oSearchDetails.TransferSearch.ReturnTime

		'replace the tokens
		Dim sSearchSummary As String
		If SearchSummary.CustomSettings.FlightPage OrElse (oSearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly AndAlso SearchSummary.CustomSettings.Upsell) Then
			sSearchSummary = GetCustomTranslation("Search Summary", SearchSummary.CustomSettings.FlightPageSearchSummary)
		ElseIf SearchSummary.CustomSettings.TransferPage And SearchSummary.CustomSettings.TransferOnlyJourney And sTransferDepartureTime <> "" Then
			sSearchSummary = GetCustomTranslation("Search Summary", SearchSummary.CustomSettings.TransferPageSearchSummary)
			If Not oSearchDetails.TransferSearch.Oneway Then
				sSearchSummary = sSearchSummary & GetCustomTranslation("Search Summary", SearchSummary.CustomSettings.TransferReturnString)
			End If
		ElseIf oSearchDetails.SearchMode = Intuitive.Web.BookingSearch.SearchModes.FlightPlusHotel Then
			sSearchSummary = GetCustomTranslation("Search Summary", SearchSummary.CustomSettings.FlightPlusHotelSearchSummary)
		Else
			sSearchSummary = GetCustomTranslation("Search Summary", SearchSummary.CustomSettings.HotelOnlySearchSummary)
		End If

		If sSearchSummary.Contains("#hotelcount#") Then
			sSearchSummary = sSearchSummary.Replace("#hotelcount#", iHotelCount.ToString)
		End If

		If sSearchSummary.Contains("#flightcount#") Then
			sSearchSummary = sSearchSummary.Replace("#flightcount#", iFlightCount.ToString)
		End If

		If sSearchSummary.Contains("#airport#") Then
			sSearchSummary = sSearchSummary.Replace("#airport#", sAirport)
		End If

		If sSearchSummary.Contains("#returnairport#") Then
			sSearchSummary = sSearchSummary.Replace("#returnairport#", sDestination)
		End If

		If sSearchSummary.Contains("#destination#") Then
			sSearchSummary = sSearchSummary.Replace("#destination#", sDestination)
		End If

		If sSearchSummary.Contains("#departuredate#") Then
			sSearchSummary = sSearchSummary.Replace("#departuredate#", sDepartureDate)
		End If

		If sSearchSummary.Contains("#returndate#") Then
			sSearchSummary = sSearchSummary.Replace("#returndate#", sReturnDate)
		End If

		If sSearchSummary.Contains("#duration#") Then
			sSearchSummary = sSearchSummary.Replace("#duration#", sDuration)
		End If

		If sSearchSummary.Contains("#pax#") Then
			sSearchSummary = sSearchSummary.Replace("#pax#", sPaxBuilder)
		End If

		If sSearchSummary.Contains("#openparagraph#") Then
			sSearchSummary = sSearchSummary.Replace("#openparagraph#", "<p>")
		End If

		If sSearchSummary.Contains("#closeparagraph#") Then
			sSearchSummary = sSearchSummary.Replace("#closeparagraph#", "</p>")
		End If

		If sSearchSummary.Contains("#openstrong#") Then
			sSearchSummary = sSearchSummary.Replace("#openstrong#", "<strong>")
		End If

		If sSearchSummary.Contains("#closestrong#") Then
			sSearchSummary = sSearchSummary.Replace("#closestrong#", "</strong>")
		End If

		If sSearchSummary.Contains("hotels") AndAlso iHotelCount = 1 Then
			sSearchSummary = sSearchSummary.Replace("hotels", "hotel")
		End If

		If sSearchSummary.Contains("#transferpickup#") Then
			sSearchSummary = sSearchSummary.Replace("#transferpickup#", sTransferPickup)
		End If

		If sSearchSummary.Contains("#transferdropoff#") Then
			sSearchSummary = sSearchSummary.Replace("#transferdropoff#", sTransferDropOff)
		End If

		If sSearchSummary.Contains("#transferdeparturedate#") Then
			sSearchSummary = sSearchSummary.Replace("#transferdeparturedate#", sTransferDepartureDate)
		End If

		If sSearchSummary.Contains("#transferreturndate#") Then
			sSearchSummary = sSearchSummary.Replace("#transferreturndate#", sTransferReturnDate)
		End If

		If sSearchSummary.Contains("#transferdeparturetime#") Then
			sSearchSummary = sSearchSummary.Replace("#transferdeparturetime#", sTransferDepartureTime)
		End If

		If sSearchSummary.Contains("#transferreturntime#") Then
			sSearchSummary = sSearchSummary.Replace("#transferreturntime#", sTransferReturnTime)
		End If

		If sSearchSummary.Contains("#openh5#") Then
			sSearchSummary = sSearchSummary.Replace("#openh5#", "<p class='searchSummaryHeading'>")
		End If

		If sSearchSummary.Contains("#closeh5#") Then
			sSearchSummary = sSearchSummary.Replace("#closeh5#", "</p>")
		End If

		Return SafeString(sSearchSummary)

	End Function

#End Region

#Region "Support Classes"

	Public Class CustomSetting
		Public ControlOverride As String
		Public CSSClassOverride As String
		Public HotelOnlySearchSummary As String
		Public ExtrasPage As Boolean
		Public FlightPlusHotelSearchSummary As String
		Public FlightPage As Boolean
		Public FlightPageSearchSummary As String
		Public TransferPage As Boolean
		Public TransferPageSearchSummary As String
		Public TransferReturnString As String
		Public TransferOnlyJourney As Boolean
		Public ShowTotalPages As Boolean
		Public PagingTotalLinks As Integer = 5
		Public AlwaysShow As Boolean 'A setting to decide whether we should always show the the summary(even when we only have 1 hotel)
		Public ShortMonth As Boolean    'Self Explanatory - Dates to shorten months
		Public NextIcon As String
		Public PreviousIcon As String
		Public NoResultsHide As Boolean = False
		Public SortByInterestingnessTitle As String
		Public InterestingnessByBrand As Boolean = False
		Public Upsell As Boolean
		Public RenderHolderOnly As Boolean
	End Class

#End Region

End Class
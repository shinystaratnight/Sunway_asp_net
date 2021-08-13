Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web.Translation
Imports System.Xml
Imports System.Linq
Imports Intuitive.Web.Lookups

Public Class SearchTool
	Inherits WidgetBase

#Region "Properties"

	Private Shared Property PropertyLock As New Object

	Public Shared Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("searchtool_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("searchtool_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(ByVal value As CustomSetting)
			HttpContext.Current.Session("searchtool_customsettings") = value
		End Set

	End Property

#End Region

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'1. save settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.PlaceholderOverrides = Functions.SafeString(Settings.GetValue("PlaceholderOverrides"))
			.PreSearchScript = Settings.GetValue("PreSearchScript")
			.DurationRange = Settings.GetValue("DurationRange")
			.AdultsRange = Settings.GetValue("AdultsRange")
			.ChildrenRange = Settings.GetValue("ChildrenRange")
			.InfantsRange = Settings.GetValue("InfantsRange")
			.IncludeGeographyLevel1Name = Settings.GetValue("IncludeGeographyLevel1Name").ToSafeBoolean
			.IncludeGeographyGroups = Settings.GetValue("IncludeGeographyGroups").ToSafeBoolean
			.CheckAvailability = Settings.GetValue("CheckAvailability").ToSafeBoolean
			.InsertWarningAfter = Settings.GetValue("InsertWarningAfter").ToSafeString
			.ResetCookie = Settings.GetValue("ResetCookie").ToSafeBoolean
			.DepartureMode = Functions.SafeEnum(Of LocationChooseMode)(Settings.GetValue("DepartureMode"))
			.DestinationMode = Functions.SafeEnum(Of LocationChooseMode)(Settings.GetValue("DestinationMode"))
			.FlightOnlyDestinationMode = Functions.SafeEnum(Of LocationChooseMode)(Settings.GetValue("FlightOnlyDestinationMode"))
			.RegionDropdownScript = Settings.GetValue("RegionDropdownScript")
			.QueryStringAutoPopulate = Settings.GetValue("QueryStringAutoPopulate").ToSafeBoolean
			.AlwaysIncludeResorts = SafeBoolean(Settings.GetValue("AlwaysIncludeResorts"))
			.AutoCompleteTranslated = SafeBoolean(Settings.GetValue("AutoCompleteTranslated"))
		End With

		SearchTool.CustomSettings = oCustomSettings

		'1b. If we need to use the query string to auto populate the search tool, then save the query string
		Dim oQueryString As NameValueCollection = If(SearchTool.CustomSettings.QueryStringAutoPopulate AndAlso HttpContext.Current IsNot Nothing,
													 HttpContext.Current.Request.QueryString, Nothing)

		'2. ensure cookie is set
		BookingBase.SearchDetails.Setup(SearchTool.CustomSettings.ResetCookie, oQueryString)


		'3. Add a setting for default values
		Dim oSetting As New Intuitive.Web.PageDefinition.WidgetSetting
		oSetting.Key = "DefaultSearchValuesJSON"
		oSetting.Value = Newtonsoft.Json.JsonConvert.SerializeObject(New DefaultSearchValues())
		Me.Settings.Add(oSetting)

		'Auto Fill Destination
		If SafeBoolean(Settings.GetValue("AutoFillDestination")) Then
			BookingBase.SearchDetails.ArrivingAtID = GetDefaultRegion()
			BookingBase.SearchDetails.SetSearchCookie()
		End If

		'4. build control
		If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")
		Dim sControlPath As String

		If Me.URLPath.EndsWith(".ascx") Then
			sControlPath = Me.URLPath
		Else
			sControlPath = Me.URLPath & "/" & "searchtool.ascx"
		End If

		Dim oControl As New Control
		Try
			oControl = Me.Page.LoadControl(sControlPath)
		Catch ex As Exception
			oControl = Me.Page.LoadControl("/widgetslibrary/SearchTool/searchtool.ascx")
		End Try

		If Not Me.Settings.Exists(Function(o) o.Key = "IsOverbranded") Then
			Dim widgetSetting As New PageDefinition.WidgetSetting("IsOverbranded", Me.PageDefinition.Overbranding.isOverbranded.ToString())
			Me.Settings.Add(widgetSetting)
		End If

		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)

		Me.DrawControl(writer, oControl)

	End Sub

#Region "Search"

	Public Shared Function Search(Optional ByVal PreserveBasket As Boolean = False) As String

		Dim oSearchToolReturn As SearchToolReturn = SendSearch(PreserveBasket)

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSearchToolReturn)

	End Function

	Public Shared Function Search_MaintainResults(ByVal MaintainFlightResults As Boolean, ByVal MaintainHotelResults As Boolean,
												  ByVal MaintainExtraResults As Boolean, ByVal MaintainTransferResults As Boolean,
												  Optional ByVal PreserveBasket As Boolean = False) As String

		Dim oSearchToolReturn As SearchToolReturn = SendSearch(PreserveBasket, MaintainFlightResults, MaintainHotelResults, MaintainExtraResults, MaintainTransferResults)

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSearchToolReturn)

	End Function

	Public Shared Function SendSearch(Optional ByVal PreserveBasket As Boolean = False, Optional ByVal MaintainFlightResults As Boolean = False,
									  Optional ByVal MaintainHotelResults As Boolean = False, Optional ByVal MaintainTransferResults As Boolean = False,
									  Optional ByVal MaintainExtraResults As Boolean = False) As SearchToolReturn

		'Store flights for later if required
		Dim oFlightsStore As New FlightResultHandler
		If MaintainFlightResults Then
			oFlightsStore = BookingBase.SearchDetails.FlightResults
		End If

		Dim oPropertyStore As New PropertyResultHandler
		If MaintainHotelResults Then
			oPropertyStore = BookingBase.SearchDetails.PropertyResults
		End If

		Dim oExtraStore As New ExtraResultHandler
		If MaintainExtraResults Then
			oExtraStore = BookingBase.SearchDetails.ExtraResults
		End If

		Dim oTransferStore As New BookingTransfer.Results
        If MaintainTransferResults Then
            oTransferStore = BookingBase.SearchDetails.TransferResults
        End If

        'clear session
        BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
        If Not PreserveBasket Then
            BookingBase.ClearSearchBasket()
        End If

        'get value pairs and decode to object
        Dim oBookingSearch As New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
        oBookingSearch.ProcessTimer.RecordStart("iVectorWidgets", "Search Tool Search", ProcessTimer.MainProcess)
        Dim sKeyValuePairs As String = oBookingSearch.GetSearchCookie
        oBookingSearch.Decode(sKeyValuePairs)

        Dim oSearchReturn As New BookingSearch.SearchReturn
        Dim oAvailbilityResponse As New iVectorConnectInterface.Property.RoomAvailabilityResponse

        'Restore Flight Results
        If MaintainFlightResults Then
            oBookingSearch.FlightResults = oFlightsStore
        End If

        'Restore Hotel Results
        If MaintainHotelResults Then
            oBookingSearch.PropertyResults = oPropertyStore
        End If

        'Restore Extra Results
        If MaintainExtraResults Then
            oBookingSearch.ExtraResults = oExtraStore
        End If

        'Restore Transfer Results
        If MaintainTransferResults Then
            oBookingSearch.TransferResults = oTransferStore
        End If

        'If we are not doing a check availability request or if our search includes a flight then we just want to do a normal search
        If Not SearchTool.CustomSettings.CheckAvailability OrElse oBookingSearch.SearchMode <> BookingSearch.SearchModes.HotelOnly Then

            'If we are doing a F+H Search, but replacing doing a check availibility request rather than a normal search, we still need to do a normal flight
            'Search.
            If SearchTool.CustomSettings.CheckAvailability AndAlso oBookingSearch.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
                oBookingSearch.SearchMode = BookingSearch.SearchModes.HotelOnly
            End If

            'search
            oBookingSearch.ProcessTimer.RecordStart("iVectorWidgets", "Perform the search", ProcessTimer.MainProcess)
            oSearchReturn = oBookingSearch.Search()
            oBookingSearch.ProcessTimer.RecordEnd("iVectorWidgets", "Perform the search", ProcessTimer.MainProcess)

            'if flight plus hotel set selected flight
            If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
                OrElse BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
                BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight(False)
            End If

            BookingBase.SearchDetails.PropertyResults.ApplyDefaultFilters(oBookingSearch, BookingBase.SearchDetails.PropertyResults.ResultsFilter)

            'perform initial filter and sort
            BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
            BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder,
               BookingBase.SearchDetails.PriorityPropertyID)
            BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter)
            BookingBase.SearchDetails.FlightResults.SortResults()

            'update counts in case results are not saved
            oBookingSearch.ProcessTimer.RecordStart("iVectorWidgets", "Update results counts", ProcessTimer.MainProcess)
            oSearchReturn.PropertyCount = BookingBase.SearchDetails.PropertyResults.TotalHotels
            oSearchReturn.FlightCount = BookingBase.SearchDetails.FlightResults.iVectorConnectResults.Count
            oSearchReturn.FlightCarouselCount = BookingBase.SearchDetails.FlightCarouselResults.TotalValidDates
            oBookingSearch.ProcessTimer.RecordEnd("iVectorWidgets", "Update results counts", ProcessTimer.MainProcess)

            'setup basket guests from search
            BookingBase.SearchBasket.SetupBasketGuestsFromSearch()

            oBookingSearch.ProcessTimer.RecordEnd("iVectorWidgets", "Search Tool Search", ProcessTimer.MainProcess)

            'send image
            If BookingBase.EmailSearchImage <> "" Then
                SearchDiagnostics.EmailSearchImage(oBookingSearch.ProcessTimer.Times, BookingBase.EmailSearchImage)
            End If

        End If

        'If we're going to do a Check Availability rather than a hotel search do it here.
        If SearchTool.CustomSettings.CheckAvailability Then
            BookingBase.SearchDetails = oBookingSearch
            oAvailbilityResponse = BookingAvailability.Search(oBookingSearch.ArrivingAtID, oBookingSearch.DepartureDate, oBookingSearch.Duration, oBookingSearch.TotalAdults)
        End If

        Dim oSearchToolReturn As New SearchToolReturn

        oSearchToolReturn.SearchAvailabilities.Availability = BookingBase.SearchAvailabilities.Availability
        With oSearchToolReturn.SearchReturn
            .OK = oSearchReturn.OK
            .Warning = oSearchReturn.Warning
            .PropertyCount = oSearchReturn.PropertyCount
            .FlightCount = oSearchReturn.FlightCount
            .ExactMatchFlightCount = oSearchReturn.ExactMatchFlightCount
            .FlightCarouselCount = oSearchReturn.FlightCarouselCount
            .TransferCount = oSearchReturn.TransferCount
            .ExtraCount = oSearchReturn.ExtraCount
            .CarHireCount = oSearchReturn.CarHireCount
			.CarHireDepotCount = oSearchReturn.CarHireDepotCount
		End With

        'just before we log the search, set a Guid on the search details so we can link page views to searches in the log tables
        BookingBase.SearchDetails.SearchGuid = System.Guid.NewGuid.ToString
		BookingBase.DataLogger.LogSearch(BookingBase.SearchDetails)

		oSearchToolReturn.SearchReturn.DataCollectionInfo.WebsitePostConnectElapsedTime = (DateTime.Now - oSearchReturn.DataCollectionInfo.IVectorConnectReceivedTimeStamp).TotalSeconds

		'return
		Return oSearchToolReturn

    End Function

#End Region

#Region "Deeplink Search"

	Public Function DeeplinkSearch() As String

		'1. clear session
		BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
		BookingBase.ClearSearchBasket()

		'2. Get key value pairs as decode into a search object
		Dim oBookingSearch As New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
		Dim sKeyValuePairs As String = oBookingSearch.GetSearchCookie
		oBookingSearch.Decode(sKeyValuePairs)

		'3.Turn the booking search into a deeplink and pass in the search page
		Dim sDeeplink As String = BookingDeeplink.CurrentSearchToDeepLink(oBookingSearch)
		sDeeplink = GetBaseURL() + "search" + sDeeplink

		'4. Return deeplink
		Return sDeeplink

	End Function

#End Region

#Region "ArrivingAtDropdowns"

	'Original ArrivingAtDropdowns, passes in 0 to BuildDropdowns to maintain original functionality
	Public Overridable Function ArrivingAtDropdowns(ByVal ArrivingAtID As Integer, ByVal AirportID As Integer) As String

		Return BuildDropdowns(ArrivingAtID, AirportID, 0)

	End Function

	'Arriving at dropdowns filtered by country
	Public Overridable Function ArrivingAtDropdownsCountry(ByVal ArrivingAtID As Integer, ByVal AirportID As Integer, ByVal CountryID As Integer) As String

		Return BuildDropdowns(ArrivingAtID, AirportID, CountryID)

	End Function

	'Formerly ArrivingAtDropDowns, filters regions by country if countryid != 0
	Public Overridable Function BuildDropdowns(ByVal ArrivingAtID As Integer, ByVal AirportID As Integer, ByVal CountryID As Integer) As String

		Dim sHTML As String = ""

		'set geography
		Dim iRegionID As Integer = 0
		Dim iResortID As Integer = 0

		If ArrivingAtID > 0 Then
			iRegionID = ArrivingAtID
		ElseIf ArrivingAtID < 0 Then
			iResortID = ArrivingAtID * -1
			'if deeplinks pass in invalid IDs it all falls over - just return 0
			Try
				iRegionID = Lookups.GetLocationFromResort(iResortID).GeographyLevel2ID
			Catch ex As Exception
				iRegionID = 0
			End Try
		End If

		'region dropdown
		Dim oRegionDropdown As New Intuitive.Web.Controls.Dropdown
		With oRegionDropdown
			.ID = "ddlRegionID"

			'set placeholder
			If SearchTool.CustomSettings.PlaceholderOverrides <> "" Then
				Dim sPlaceholder As String = GetDropdownPlaceholder("ddlRegionID", SearchTool.CustomSettings.PlaceholderOverrides)
				.OverrideBlankText = GetCustomTranslation("Search Tool", IIf(sPlaceholder <> "", sPlaceholder, "Going To..."))
			Else
				.OverrideBlankText = GetCustomTranslation("Search Tool", "Going To...")
			End If

			.IncludeParent = True
			.Value = iRegionID.ToString

			If SearchTool.CustomSettings.RegionDropdownScript <> "" Then
				.Script = SearchTool.CustomSettings.RegionDropdownScript
			Else
				If CountryID = 0 Then
					.Script = "int.f.SetValue('hidArrivingAtID',  int.dd.GetIntValue(this));SearchTool.Support.ArrivingAtDropdowns();"
				Else
					.Script = "int.f.SetValue('hidArrivingAtID',  int.dd.GetIntValue(this));SearchTool.Support.ArrivingAtDropdownsCountry();"
				End If

			End If

			.AutoGroupType = "standard"

			'if we have all airport build dropdown from country regions else get regions by airport ID
			If AirportID = 0 Then
				If CountryID = 0 Then
					.Lookup = Lookups.LookupTypes.CountryRegion
				Else
					.ProcessKeyValuePairs(Lookups.ListRegionsByCountryAndAirport(CountryID, AirportID))
				End If

			Else
				If CountryID = 0 Then
					.ProcessKeyValuePairsWithParent(Lookups.ListRegionsByAirportID(AirportID))
				Else
					.ProcessKeyValuePairs(Lookups.ListRegionsByCountryAndAirport(CountryID, AirportID))
				End If

			End If

		End With

		sHTML = Intuitive.Functions.RenderControlToString(oRegionDropdown) & "<i></i>"

		'resort dropdown
		If iRegionID > 0 OrElse SearchTool.CustomSettings.AlwaysIncludeResorts Then

			Dim oResortDropdown As New Intuitive.Web.Controls.Dropdown
			With oResortDropdown

				.ID = "ddlResortID"

				'set placeholder
				If SearchTool.CustomSettings.PlaceholderOverrides <> "" Then
					Dim sPlaceholder As String = GetDropdownPlaceholder("ddlResortID", SearchTool.CustomSettings.PlaceholderOverrides)
					.OverrideBlankText = GetCustomTranslation("Search Tool", IIf(sPlaceholder <> "", sPlaceholder, "Any Resort"))
				Else
					.OverrideBlankText = GetCustomTranslation("Search Tool", "Any Resort")
				End If

				.ProcessKeyValuePairs(Lookups.ListResortsByRegionAndAirport(iRegionID, AirportID))
				.Value = iResortID.ToString
				.Script = "SearchTool.Support.SetArrivingAtID();"

			End With

			sHTML = sHTML & Intuitive.Functions.RenderControlToString(oResortDropdown) & "<i></i>"
		End If

		Return sHTML

	End Function

	Public Overridable Function ArrivingAtCountryDropdowns(ByVal CountryID As Integer, ByVal AirportID As Integer) As String

		Dim sHTML As String = ""

		'region dropdown
		Dim oRegionDropdown As New Intuitive.Web.Controls.Dropdown
		With oRegionDropdown
			.ID = "ddlRegionID"

			'set placeholder
			If SearchTool.CustomSettings.PlaceholderOverrides <> "" Then
				Dim sPlaceholder As String = GetDropdownPlaceholder("ddlRegionID", SearchTool.CustomSettings.PlaceholderOverrides)
				.OverrideBlankText = GetCustomTranslation("Search Tool", IIf(sPlaceholder <> "", sPlaceholder, "Going To..."))
			Else
				.OverrideBlankText = GetCustomTranslation("Search Tool", "Going To...")
			End If

			.IncludeParent = True
			.Script = "int.f.SetValue('hidArrivingAtID',  int.dd.GetIntValue(this));SearchTool.Support.ArrivingAtDropdownsCountry();"
			.AutoGroupType = "standard"
			.ProcessKeyValuePairs(Lookups.ListRegionsByCountryAndAirport(CountryID, AirportID))

		End With

		sHTML = Intuitive.Functions.RenderControlToString(oRegionDropdown) & "<i></i>"

		Return sHTML

	End Function

	Public Overridable Function DepratingFromCountryDropdowns(ByVal CountryID As Integer) As String

		Dim sHTML As String = ""

		'region dropdown
		Dim oDepartingDropdown As New Intuitive.Web.Controls.Dropdown
		With oDepartingDropdown
			.ID = "ddlDepartingFromID"

			'set placeholder
			If SearchTool.CustomSettings.PlaceholderOverrides <> "" Then
				Dim sPlaceholder As String = GetDropdownPlaceholder("ddlDepartingFromID", SearchTool.CustomSettings.PlaceholderOverrides)
				.OverrideBlankText = GetCustomTranslation("Search Tool", IIf(sPlaceholder <> "", sPlaceholder, "Airport"))
			Else
				.OverrideBlankText = GetCustomTranslation("Search Tool", "Airport")
			End If

			.IncludeParent = True
			.Script = "int.f.SetValue('hidAirportDepartingFromID',  int.dd.GetIntValue(this));SearchTool.Support.SetDepartingFromID();"
			.AutoGroupType = "standard"
			.ProcessKeyValuePairs(Lookups.ListDepartureAirportsByCountry(CountryID))

		End With

		sHTML = Intuitive.Functions.RenderControlToString(oDepartingDropdown) & "<i></i>"

		Return sHTML

	End Function

#End Region

#Region "Arrival Airport Dropdown"

	Public Overridable Function ArrivalAirportDropdown(ByVal DepartureAirportID As Integer, ByVal ArrivalAirportID As Integer) As String

		Dim oArrivalAirportDropdown As New Intuitive.Web.Controls.Dropdown
		With oArrivalAirportDropdown
			.ID = "ddlArrivalAirportID"
			.AutoGroupType = "standard"

			'set placeholder
			If SearchTool.CustomSettings.PlaceholderOverrides <> "" Then
				Dim sPlaceholder As String = GetDropdownPlaceholder("ddlArrivalAirportID", SearchTool.CustomSettings.PlaceholderOverrides)
				.OverrideBlankText = GetCustomTranslation("Search Tool", IIf(sPlaceholder <> "", sPlaceholder, "Going To..."))
			Else
				.OverrideBlankText = GetCustomTranslation("Search Tool", "Going To...")
			End If

			If SearchTool.CustomSettings.DestinationMode = LocationChooseMode.DropdownGrouped OrElse
			 SearchTool.CustomSettings.DestinationMode = LocationChooseMode.AutoCompleteAndDropdownGrouped Then
				.IncludeParent = True
				.ProcessKeyValuePairsWithParent(BookingBase.Lookups.ListArrivalAirportsWithParentByDeparture(DepartureAirportID))
			ElseIf SearchTool.CustomSettings.DestinationMode = LocationChooseMode.DropdownGroupedWithIATA Then
				.IncludeParent = True
				.ProcessKeyValuePairsWithParent(BookingBase.Lookups.ListArrivalAirportsWithParentByDeparture(DepartureAirportID, True))
			Else
				.ProcessKeyValuePairsWithParent(BookingBase.Lookups.ListArrivalAirportsWithParentByDeparture(DepartureAirportID, IncludeAirportGroups:=True))
			End If

			.Value = ArrivalAirportID.ToString
			.Script = "int.f.SetValue('hidArrivingAtID', int.dd.GetIntValue(this) + 1000000);"

		End With

		Return Intuitive.Functions.RenderControlToString(oArrivalAirportDropdown)

	End Function

#End Region

#Region "AutoComplete"

	Public Function ArrivingAtName(ByVal ArrivingAtID As Integer) As String

		Dim sName As String = ""

		If ArrivingAtID < -1000000 Then
			Dim oGeographyGroup As Lookups.GeographyGrouping = Lookups.GeographyGroupSearchLookup().Where(Function(o) o.GeographyGroupingID = ArrivingAtID).FirstOrDefault
			If oGeographyGroup IsNot Nothing Then
				sName = oGeographyGroup.GeographyGroup
			End If
		Else
			Dim oGeographyLevel As Lookups.GeographyLevel = Lookups.GeographySearchLookup(CustomSettings.IncludeGeographyLevel1Name).Where(Function(o) o.ID = ArrivingAtID).FirstOrDefault
			If oGeographyLevel IsNot Nothing Then
				sName = oGeographyLevel.Name
			End If
		End If

		Return sName

	End Function
	Public Function HotelArrivingAtDropdowns(ByVal HotelOnlyArrivingAtID As Integer) As String

		Dim sName As String = ""
		If Not HotelOnlyArrivingAtID = 0 And Not HotelOnlyArrivingAtID = Nothing Then
			Dim oPropertyRefReturn As Lookups.PropertyReference = Lookups.PropertyReferences().
				Where(Function(o) o.PropertyReferenceID = HotelOnlyArrivingAtID AndAlso
					Lookups.BrandGeographyLevel3.Exists(Function(x) x.GeographyLevel3ID = o.GeographyLevel3ID)).
					ToList()(0)
			Dim sGeographyRegion As String = Lookups.GetKeyPairValue(LookupTypes.Resort, oPropertyRefReturn.GeographyLevel3ID)
			sName = oPropertyRefReturn.PropertyName & " (" & sGeographyRegion & ")"
		End If

		Return sName

	End Function

	Public Function AirPortName(ByVal AirportID As Integer) As String

		Dim sAirport As String = ""

		'check if group or airport
		If AirportID > 1000000 Then
			Dim oAirportGroup As Lookups.AirportGroup = Lookups.AirportGroups.Where(Function(o) o.AirportGroupID = (AirportID - 1000000)).FirstOrDefault
			If Not oAirportGroup Is Nothing Then Return oAirportGroup.AirportGroup
		Else
			Dim oAirport As Lookups.Airport = Lookups.Airports.Where(Function(o) o.AirportID = AirportID).FirstOrDefault
			If Not oAirport Is Nothing Then Return oAirport.Airport & " (" & oAirport.IATACode & ")"
		End If

		Return sAirport

	End Function

	Public Function AutoCompleteRegionDropdown(ByVal SearchText As String, ByVal DepartureAirportID As Integer) As String
		Return AutoCompleteRegionDropdownWithScript(SearchText, DepartureAirportID, "SearchTool.Support.SetArrivingAtID();")
	End Function

	Public Function AutoCompleteRegionDropdownWithScript(ByVal SearchText As String, ByVal DepartureAirportID As Integer, ByVal SelectedScript As String) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = "acpArrivingAtID"
		If String.IsNullOrEmpty(SelectedScript) Then
			oAutoCompleteReturn.SelectedScript = "SearchTool.Support.SetArrivingAtID();"
		Else
			oAutoCompleteReturn.SelectedScript = SelectedScript
		End If

		Return ProcessRegionAutoCompleteReturn(oAutoCompleteReturn, SearchText, DepartureAirportID)

	End Function

	Public Function AutoCompleteRegionDropdownWithTextBoxAndScript(ByVal SearchText As String, ByVal DepartureAirportID As Integer, sSequence As String, sFunction As String) As String
		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = sSequence
		oAutoCompleteReturn.SelectedScript = sFunction

		Dim sJSON As String = ""

        If sSequence = "acpArrivingAtID" Then
            If DepartureAirportID >= 2000000 Then
                DepartureAirportID -= 2000000
            End If

            sJSON = ProcessRegionAutoCompleteReturn(oAutoCompleteReturn, SearchText, DepartureAirportID)

        Else
            Dim oResorts As New KeyValuePairs()
            Dim oRegions As New List(Of KeyValuePairWithParent)
            Dim oRegions_Filtered As New List(Of KeyValuePairWithParent)
            Dim oAirports As New List(Of Integer)
            If DepartureAirportID > 2000000 Then
                oRegions.AddRange(Lookups.ListRegionsByAirportID(DepartureAirportID - 2000000).ToList())
            Else
                oAirports = GetAirportsByRegionResort(DepartureAirportID, SearchText)
                For Each AirportID As Integer In oAirports
                    oRegions.AddRange(Lookups.ListRegionsByAirportID(AirportID))
                Next
            End If
			oRegions_Filtered = oRegions.Where(Function(loc) loc.Value.ToLower().Contains(SearchText)).GroupBy(Function(o) o.ID).Select(Function(o) o.First()).ToList()

			If oRegions_Filtered.Count = 0 Or oRegions Is Nothing Then
                Dim oLocs As List(Of Location) = New List(Of Location)
                For Each oRegion As KeyValuePairWithParent In oRegions.Distinct()
                    oLocs.AddRange(Lookups.Locations.Where(Function(loc) loc.GeographyLevel3Name.ToLower().Contains(SearchText) _
                                              And loc.GeographyLevel2ID = oRegion.ID))
                Next
                For Each oloc As Location In oLocs
                    oAutoCompleteReturn.Items.Add(New AutoCompleteReturn.AutoCompleteItem(Intuitive.ToSafeString(oloc.GeographyLevel3ID * -1), oloc.GeographyLevel3Name + ", " + oloc.GeographyLevel2Name))
                Next
            Else
                For Each oRegion As KeyValuePairWithParent In oRegions_Filtered
                    'add region to list
                    oAutoCompleteReturn.Items.Add(New AutoCompleteReturn.AutoCompleteItem(Intuitive.ToSafeString(oRegion.ID), oRegion.Value + ", " +
                                                                                                                          Lookups.Locations.Where(Function(loc) loc.GeographyLevel2ID = oRegion.ID).FirstOrDefault().GeographyLevel1Name))
                    If DepartureAirportID > 2000000 Then
                        DepartureAirportID -= 2000000
                        oResorts = Lookups.ListResortsByRegionAndAirport(oRegion.ID, DepartureAirportID)
                    Else
                        For Each AirportID As Integer In oAirports
                            Dim oResortList As KeyValuePairs = Lookups.ListResortsByRegionAndAirport(oRegion.ID, AirportID)
                            For i As Integer = 0 To oResortList.Count - 1
                                If Not oResorts.Keys.Contains(oResortList.Keys(i)) Then
                                    oResorts.Add(oResortList.Keys(i), oResortList.Values(i))
                                End If
                            Next
                        Next
                    End If
                    For iCurrentIndex As Integer = 0 To oResorts.Count - 1

                        Dim iIndex As Integer = iCurrentIndex
                        Dim sGeographyLevel2Name As String =
                            Lookups.Locations.FirstOrDefault(Function(loc) loc.GeographyLevel3ID = oResorts.Keys(iIndex)).GeographyLevel2Name

                        oAutoCompleteReturn.Items.Add(
                        New AutoCompleteReturn.AutoCompleteItem(Intuitive.ToSafeString(oResorts.Keys(iCurrentIndex) * -1),
                                                                oResorts.Values(iCurrentIndex) + ", " + sGeographyLevel2Name))

                    Next
                Next
            End If
            sJSON = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)
        End If

        Return sJSON

    End Function

    Private Function ProcessRegionAutoCompleteReturn(ByRef AutoCompleteReturn As AutoCompleteReturn, SearchText As String, DepartureAirportID As Integer) As String
        If CustomSettings.AutoCompleteTranslated AndAlso BookingBase.DisplayLanguageID > 0 Then
            If CustomSettings.IncludeGeographyGroups Then
                AutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteGeographyGroupings(SearchText, BookingBase.DisplayLanguageID))
            End If
            AutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteGeographies(SearchText, DepartureAirportID, BookingBase.DisplayLanguageID))
        Else
            If CustomSettings.IncludeGeographyGroups Then
                AutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteGeographyGroupings(SearchText))
            End If
            AutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteGeographies(SearchText, DepartureAirportID))
        End If

        Return Newtonsoft.Json.JsonConvert.SerializeObject(AutoCompleteReturn)

    End Function

    Private Function GetAirportsByRegionResort(ByVal DepartureAirportID As Integer, ByVal SearchText As String) As List(Of Integer)

        Dim airports As IEnumerable(Of AirportGeography)
        If DepartureAirportID < 0 Then
            airports = Lookups.AirportGeographies.Where(Function(a) a.GeographyLevel3ID = (DepartureAirportID * -1)).ToList()
        Else
            Dim resortids As List(Of Integer) = Lookups.Locations.Where(Function(loc) loc.GeographyLevel2ID = DepartureAirportID).Select(Function(o) o.GeographyLevel3ID).ToList()
            airports = Lookups.AirportGeographies.Where(Function(a) resortids.Exists(Function(r) r = a.GeographyLevel3ID))
        End If
        Return airports.Select(Function(o) o.AirportID).Distinct.ToList()
    End Function

	Public Function AutoCompleteHotelAndRegionDropdown(ByVal SearchText As String, ByVal DepartureAirportID As Integer) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = "acpHotelOnlyArrivingAtID"
		oAutoCompleteReturn.SelectedScript = "SearchTool.HotelOnlySupport.SetArrivingAtID();"

		If CustomSettings.AutoCompleteTranslated AndAlso BookingBase.DisplayLanguageID > 0 Then
			If CustomSettings.IncludeGeographyGroups Then
				oAutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteGeographyGroupings(SearchText, BookingBase.DisplayLanguageID))
			End If
			oAutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteGeographiesAndHotels(SearchText, DepartureAirportID, BookingBase.DisplayLanguageID))
		Else
			If CustomSettings.IncludeGeographyGroups Then
				oAutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteGeographyGroupings(SearchText))
			End If
			oAutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteGeographiesAndHotels(SearchText, DepartureAirportID))
		End If

		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)

		Return sJSON

	End Function

	Private Function SearchTranslatedAutoCompleteGeographyGroupings(SearchText As String, LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oGeographyGroups As IEnumerable(Of Lookups.TranslatedGeographyGrouping) =
			Lookups.SearchTranslatedGeographyGroups(SearchText, LanguageID, 100)

		Return oGeographyGroups.Select(Function(gg) New AutoCompleteReturn.AutoCompleteItem(
										   gg.GeographyGroupingID.ToString,
										   gg.Translations.
												Where(Function(translation) translation.LanguageID = LanguageID).
												FirstOrDefault().GeographyGrouping))

	End Function

	Private Function SearchAutoCompleteGeographyGroupings(SearchText As String) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)
		Dim oGeographyGroups As IEnumerable(Of Lookups.GeographyGrouping) = Lookups.SearchGeographyGroups(SearchText, 100)

		Return oGeographyGroups.Select(Function(gg) _
					New AutoCompleteReturn.AutoCompleteItem(gg.GeographyGroupingID.ToString, gg.GeographyGroup))

	End Function

	Private Function SearchTranslatedAutoCompleteGeographies(SearchText As String, ByVal DepartureAirportID As Integer, LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oRegionResorts As IEnumerable(Of Lookups.TranslatedGeographyLevel) =
			Lookups.SearchTranslatedGeography(SearchText, DepartureAirportID, LanguageID, 100, CustomSettings.IncludeGeographyLevel1Name)

		Return oRegionResorts.Select(Function(o) New AutoCompleteReturn.AutoCompleteItem(o.ID.ToString, o.Translations(LanguageID)))

	End Function
	Private Function SearchTranslatedAutoCompleteGeographiesAndHotels(SearchText As String, ByVal DepartureAirportID As Integer, LanguageID As Integer) As List(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oRegionResorts As List(Of Lookups.TranslatedGeographyLevel) =
			Lookups.SearchTranslatedGeography(SearchText, DepartureAirportID, LanguageID, 100, CustomSettings.IncludeGeographyLevel1Name)
		Dim oRegionAndHotel As List(Of AutoCompleteReturn.AutoCompleteItem) = oRegionResorts.Select(Function(o) New AutoCompleteReturn.AutoCompleteItem(o.ID.ToString, o.Translations(LanguageID))).ToList()

		Dim oHotels As List(Of Lookups.PropertyReference) = PropertySearch(SearchText, 0, 100)

		For Each Hotel As Lookups.PropertyReference In oHotels
			Dim searchHotel As New AutoCompleteReturn.AutoCompleteItem(Hotel.GeographyLevel2ID.ToString() & "." & Hotel.PropertyReferenceID, Hotel.PropertyName + " (" + Hotel.GeographyLevel3Name + ")")
			oRegionAndHotel.Add(searchHotel)
		Next

		Return oRegionAndHotel
	End Function

	Private Function SearchAutoCompleteGeographies(SearchText As String, ByVal DepartureAirportID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oRegionResorts As IEnumerable(Of Lookups.GeographyLevel) =
			Lookups.SearchGeography(SearchText, DepartureAirportID, 100, CustomSettings.IncludeGeographyLevel1Name)

		Return oRegionResorts.Select(Function(o) New AutoCompleteReturn.AutoCompleteItem(o.ID.ToString, o.Name))

	End Function
	Private Function SearchAutoCompleteGeographiesAndHotels(SearchText As String, ByVal DepartureAirportID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oRegionResorts As IEnumerable(Of Lookups.GeographyLevel) =
			Lookups.SearchGeography(SearchText, DepartureAirportID, 100, CustomSettings.IncludeGeographyLevel1Name)
		Dim oRegionAndHotel As List(Of AutoCompleteReturn.AutoCompleteItem) = oRegionResorts.Select(Function(o) New AutoCompleteReturn.AutoCompleteItem(o.ID.ToString, o.Name)).ToList()

		For Each Hotel As Lookups.PropertyReference In PropertySearch(SearchText, 0, 100)
			Dim sGeographyRegion As String = Lookups.GetKeyPairValue(LookupTypes.Resort, Hotel.GeographyLevel3ID)
			Dim searchHotel As New AutoCompleteReturn.AutoCompleteItem(Hotel.GeographyLevel2ID.ToString() & "." & Hotel.PropertyReferenceID, Hotel.PropertyName + " (" + sGeographyRegion + ")")

			oRegionAndHotel.Add(searchHotel)
		Next

		Return oRegionAndHotel

	End Function

	Public Function PropertyName(ByVal PropertyReferenceID As Integer) As String

		Dim oProperty As Lookups.PropertyReference = Lookups.PropertyReferences.Where(Function(o) o.PropertyReferenceID = PropertyReferenceID).FirstOrDefault

		If Not oProperty Is Nothing Then
			Return oProperty.PropertyName
		Else
			Return ""
		End If

	End Function

	Public Function AutoCompletePropertyDropdown(ByVal SearchText As String, ByVal ArrivingAtID As Integer, ByVal sTextBoxID As String) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		'oAutoCompleteReturn.TextBoxID = "acpPriorityPropertyID"
		oAutoCompleteReturn.TextBoxID = sTextBoxID

		'I'm not sure if this is needed seems to work without it.
		'oAutoCompleteReturn.SelectedScript = "SearchTool.Support.SetPriorityPropertyID();"
		oAutoCompleteReturn.SelectedScript = ""

		Dim oProperties As Generic.List(Of Lookups.PropertyReference) = SearchTool.PropertySearch(SearchText, ArrivingAtID, 100)

		For Each oProperty As Lookups.PropertyReference In oProperties
			Dim oAutoCompleteItem As New AutoCompleteReturn.AutoCompleteItem(oProperty.PropertyReferenceID.ToString, oProperty.PropertyName)
			oAutoCompleteReturn.Items.Add(oAutoCompleteItem)
		Next

		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)

		Return sJSON

	End Function

	Public Shared Function PropertySearch(ByVal SearchText As String, ByVal ArrivingAtID As Integer, Optional ByVal MaxResults As Integer = 0) As Generic.List(Of Lookups.PropertyReference)

		'set region and resort id
		Dim iRegionID As Integer = Functions.IIf(ArrivingAtID > 0, ArrivingAtID, 0)
		Dim iResortID As Integer = Functions.IIf(ArrivingAtID < 0, ArrivingAtID * -1, 0)

		'loop through each property and add any properties that match geography if set and text input
		Dim oProperties As New Generic.List(Of Lookups.PropertyReference)
		Dim oPropertiesToCheck As List(Of Lookups.PropertyReference) = GetListOfBrandProperties()

		For Each oProperty As Lookups.PropertyReference In oPropertiesToCheck _
   .Where(Function(o) (iRegionID = 0 OrElse o.GeographyLevel2ID = iRegionID) AndAlso (iResortID = 0 OrElse o.GeographyLevel3ID = iResortID) _
	 AndAlso (o.PropertyName.ToLower.StartsWith(SearchText.ToLower) OrElse o.PropertyName.ToLower.Contains(" " & SearchText.ToLower))) _
   .OrderBy(Function(o) o.PropertyName) _
   .OrderBy(Function(o) Functions.IIf(o.PropertyName.StartsWith(SearchText.ToLower), 0, 1))

			oProperties.Add(oProperty)
		Next

		'If there is no max results set, set it to the maximum number, otherwise get the appropriate number
		Dim iMaxResults As Integer = 0
		If MaxResults = 0 Then
			iMaxResults = oProperties.Count
		Else
			iMaxResults = Functions.IIf(oProperties.Count < MaxResults, oProperties.Count, MaxResults)
		End If

		'return results
		oProperties = oProperties.GetRange(0, iMaxResults)
		Return oProperties

	End Function

	''' <summary>
	''' A function that returns a list of all properties that are valid to the current brand
	''' </summary>
	''' <returns></returns>
	Private Shared Function GetListOfBrandProperties() As List(Of Lookups.PropertyReference)
		Dim oPropertiesToCheck As New List(Of Lookups.PropertyReference)
		SyncLock PropertyLock

			oPropertiesToCheck = CType(HttpContext.Current.Cache("searchtool_brandproperties"), List(Of Lookups.PropertyReference))
			If oPropertiesToCheck Is Nothing Then

				oPropertiesToCheck = BookingBase.Lookups.PropertyReferences().Where(Function(o) BookingBase.Lookups.BrandGeographyLevel3.Exists(Function(x) x.GeographyLevel3ID = o.GeographyLevel3ID)).ToList()
				Intuitive.Functions.AddToCache("searchtool_brandproperties", oPropertiesToCheck, 600)

			End If

		End SyncLock
		Return oPropertiesToCheck
	End Function

	Public Function AutoCompleteCarHireDepotDropdown(ByVal SearchText As String, ByVal GeographyLevel1ID As Integer, ByVal sTextBoxID As String) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = sTextBoxID
		If sTextBoxID.ToLower.Contains("pickup") Then
			oAutoCompleteReturn.SelectedScript = "SearchTool.Support.SetCarHirePickUpDepotID();"
		Else
			oAutoCompleteReturn.SelectedScript = "SearchTool.Support.SetCarHireDropOffDepotID();"
		End If

		Dim oDepots As New Generic.List(Of Lookups.CarHireDepot)

		Dim oCarHireDepots As Lookups.KeyValuePairs = BookingBase.Lookups.ListCarHireDepotsByCountryID(GeographyLevel1ID)
		For Each Lookup As Generic.KeyValuePair(Of Integer, String) In oCarHireDepots.Where(Function(o) (o.Value.ToLower.Contains(SearchText.ToLower))) _
			.OrderBy(Function(o) o.Value) _
			.OrderBy(Function(o) Functions.IIf(o.Value.StartsWith(SearchText.ToLower), 0, 1))

			Dim oAutoCompleteItem As New AutoCompleteReturn.AutoCompleteItem(Lookup.Key.ToString, Lookup.Value)
			oAutoCompleteReturn.Items.Add(oAutoCompleteItem)
		Next

		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)

		Return sJSON

	End Function

    Public Function AutoCompleteCarHireCountryDropdown(ByVal SearchText As String, ByVal TextBoxID As String, ByVal SelectedScript As String) As String
        Dim oAutoCompleteReturn As New AutoCompleteReturn
        oAutoCompleteReturn.TextBoxID = TextBoxID
        oAutoCompleteReturn.SelectedScript = SelectedScript

        oAutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteDepartureCarHireCountries(SearchText))

        Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)
        Return sJSON

    End Function

	Public Function AutoCompleteAirportDropdown(ByVal SearchText As String, ByVal TextBoxID As String, Optional ByVal SelectedScript As String = "", Optional ByVal DepartureAirportID As Integer = 0, Optional ByVal IncludeAirportGroups As Boolean = False) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = TextBoxID
		oAutoCompleteReturn.SelectedScript = SelectedScript

		oAutoCompleteReturn.Items.AddRange(Me.AutoCompleteAirportSearch(SearchText, 100, DepartureAirportID,
				CustomSettings.AutoCompleteTranslated, BookingBase.DisplayLanguageID))

		If IncludeAirportGroups Then
			oAutoCompleteReturn.Items.AddRange(Me.AutoCompleteAirportGroupSearch(SearchText, 100 - oAutoCompleteReturn.Items.Count,
				DepartureAirportID, CustomSettings.AutoCompleteTranslated, BookingBase.DisplayLanguageID))
		End If

		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)

		Return sJSON

	End Function

	Private Function AutoCompleteAirportGroupSearch(ByVal SearchText As String, ByVal MaxResults As Integer,
													ByVal DepartureAirportID As Integer, ByVal SearchTranslatedAirports As Boolean,
													ByVal LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAutoCompleteItems As New List(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oLookupAirportGroups As Generic.List(Of Lookups.AirportGroup)
		If DepartureAirportID = 0 Then
			oLookupAirportGroups = BookingBase.Lookups.ArrivalAirportGroups
		Else
			oLookupAirportGroups = BookingBase.Lookups.ListArrivalAirportGroupsByDepartureAsLookup(DepartureAirportID)
		End If

		If SearchTranslatedAirports AndAlso LanguageID > 0 Then
			Dim oTranslatedAirportGroups As IEnumerable(Of Lookups.TranslatedAirportGroup) = BookingBase.Lookups.TranslatedAirportGroups.
				Where(Function(ag) oLookupAirportGroups.Select(Function(o) o.AirportGroupID).Contains(ag.AirportGroupID)).
				Where(Function(ag) ag.Translations.Where(Function(t) t.LanguageID = LanguageID).Any).
				Where(Function(ag) ag.Translations.Where(Function(t) t.AirportGroup.ToLower.StartsWith(SearchText.ToLower) OrElse
															 t.AirportGroup.ToLower.Contains(" " & SearchText.ToLower)).Any).
				OrderBy(Function(ag) ag.Translations.Where(Function(t) t.LanguageID = LanguageID).FirstOrDefault.AirportGroup).
				OrderBy(Function(ag) IIf(ag.Translations.Where(
						Function(t) t.LanguageID = LanguageID).FirstOrDefault.AirportGroup.ToLower.StartsWith(SearchText.ToLower), 0, 1))

			oAutoCompleteItems.AddRange(oTranslatedAirportGroups.Take(IIf(MaxResults = 0, oTranslatedAirportGroups.Count, MaxResults)).
										Select(Function(ag) New AutoCompleteReturn.AutoCompleteItem(SafeString(ag.AirportGroupID + 1000000),
												ag.Translations.Where(Function(t) t.LanguageID = LanguageID).FirstOrDefault.AirportGroup)))

		Else
			Dim oAirportGroups As IEnumerable(Of Lookups.AirportGroup) = oLookupAirportGroups.
				Where(Function(o) (o.AirportGroup.ToLower.StartsWith(SearchText.ToLower) OrElse
								   o.AirportGroup.ToLower.Contains(" " & SearchText.ToLower))).
				OrderBy(Function(o) o.AirportGroup).
				OrderBy(Function(o) Functions.IIf(o.AirportGroup.StartsWith(SearchText.ToLower), 0, 1))

			oAutoCompleteItems.AddRange(oAirportGroups.Take(IIf(MaxResults = 0, oAirportGroups.Count, MaxResults)).
						Select(Function(ag) New AutoCompleteReturn.AutoCompleteItem(SafeString(ag.AirportGroupID + 1000000), ag.AirportGroup)))

		End If

		Return oAutoCompleteItems

	End Function

	Private Function AutoCompleteAirportSearch(ByVal SearchText As String, ByVal MaxResults As Integer, ByVal DepartureAirportID As Integer,
								  ByVal SearchTranslatedAirports As Boolean, ByVal LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAutoCompleteItems As New List(Of AutoCompleteReturn.AutoCompleteItem)

		'set region and resort id
		Dim iRegionID As Integer = 0
		Dim iResortID As Integer = 0

		'loop through each Airport and add any Airports that match the text input

		Dim oLookupAirports As Generic.List(Of Lookups.Airport)
		If DepartureAirportID = 0 Then
			oLookupAirports = BookingBase.Lookups.ArrivalAirports
		Else
			oLookupAirports = BookingBase.Lookups.ListArrivalAirportsByDepartureAsLookup(DepartureAirportID)
		End If

		If SearchTranslatedAirports AndAlso LanguageID > 0 Then

			Dim oTranslatedAirports As IEnumerable(Of Lookups.TranslatedAirport) = BookingBase.Lookups.TranslatedAirports.
				Where(Function(airport) oLookupAirports.Select(Function(o) o.AirportID).Contains(airport.AirportID)).
				Where(Function(airport) airport.Translations.Where(Function(trans) trans.LanguageID = LanguageID).Any).
				Where(Function(airport) airport.Translations.Where(Function(trans) trans.Airport.ToLower.StartsWith(SearchText.ToLower) OrElse
																	   trans.Airport.ToLower.Contains(" " & SearchText.ToLower)).Any OrElse
																   airport.IATACode.ToLower.StartsWith(SearchText.ToLower) OrElse
																   airport.IATACode.ToLower.Contains(" " & SearchText.ToLower)).
				OrderBy(Function(airport) airport.Translations.Where(Function(trans) trans.LanguageID = LanguageID).FirstOrDefault().Airport).
				OrderBy(Function(airport) IIf(airport.Translations.Where(Function(trans) trans.LanguageID = LanguageID).
											  FirstOrDefault().Airport.ToLower.StartsWith(SearchText.ToLower), 0, 1))

			oAutoCompleteItems.AddRange(oTranslatedAirports.Take(IIf(MaxResults = 0, oTranslatedAirports.Count, MaxResults)).
				Select(Function(airport) New AutoCompleteReturn.AutoCompleteItem(airport.AirportID.ToString,
				airport.Translations.Where(Function(trans) trans.LanguageID = LanguageID).FirstOrDefault().Airport & " (" & airport.IATACode & ")")))

		Else
			Dim oAirports As IEnumerable(Of Lookups.Airport) = oLookupAirports.
			Where(Function(o) (o.Airport.ToLower.StartsWith(SearchText.ToLower) OrElse
								o.Airport.ToLower.Contains(" " & SearchText.ToLower) OrElse
								o.IATACode.ToLower.StartsWith(SearchText.ToLower) OrElse
								o.IATACode.ToLower.Contains(" " & SearchText.ToLower))).
			OrderBy(Function(o) o.Airport).
			OrderBy(Function(o) Functions.IIf(o.Airport.StartsWith(SearchText.ToLower), 0, 1))

			oAutoCompleteItems.AddRange(oAirports.Take(IIf(MaxResults = 0, oAirports.Count, MaxResults)).
				Select(Function(airport) New AutoCompleteReturn.AutoCompleteItem(airport.AirportID.ToString,
					IIf(airport.Airport.Contains("(" & airport.IATACode & ")"), airport.Airport.Replace("""", "'"), airport.Airport.Replace("""", "'") & "(" & airport.IATACode & ")"))))

		End If

		Return oAutoCompleteItems

	End Function

	Public Function AutoCompleteDepartureAirport(ByVal SearchText As String, ByVal TextBoxID As String, Optional ByVal SelectedScript As String = "") As String

		'Auto Return
		Dim oAutoCompleteReturn As New AutoCompleteReturn
		oAutoCompleteReturn.TextBoxID = TextBoxID
		oAutoCompleteReturn.SelectedScript = SelectedScript

		'Get airport groups and airports
		If CustomSettings.AutoCompleteTranslated AndAlso BookingBase.DisplayLanguageID > 0 Then
			oAutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteDepartureAirportGroups(SearchText, BookingBase.DisplayLanguageID))
			oAutoCompleteReturn.Items.AddRange(Me.SearchTranslatedAutoCompleteDepartureAirports(SearchText, BookingBase.DisplayLanguageID))
		Else
			oAutoCompleteReturn.Items.AddRange(Me.SearchAutoCompeleteDepartureAirportGroups(SearchText))
			oAutoCompleteReturn.Items.AddRange(Me.SearchAutoCompleteDepartureAirports(SearchText))
		End If

		'return JSON
		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)
		Return sJSON

	End Function

	Private Function SearchTranslatedAutoCompleteDepartureAirportGroups(SearchText As String, LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAirportGroups As IEnumerable(Of TranslatedAirportGroup) = Lookups.TranslatedAirportGroups.
				Where(Function(ag) ag.Type = "Departure").
				Where(Function(ag) ag.Translations.Where(Function(trans) trans.LanguageID = LanguageID).Any).
				Where(Function(ag) ag.Translations.Where(Function(trans) trans.AirportGroup.ToLower.Contains(" " & SearchText.ToLower) OrElse
															 trans.AirportGroup.ToLower.StartsWith(SearchText.ToLower)).Any).
				OrderBy(Function(ag) ag.Translations.Where(Function(trans) trans.LanguageID = LanguageID).FirstOrDefault.AirportGroup).
				OrderBy(Function(ag) IIf(ag.Translations.Where(Function(trans) trans.LanguageID = LanguageID).
										 FirstOrDefault.AirportGroup.ToLower.StartsWith(SearchText.ToLower), 0, 1))

		Return oAirportGroups.Select(Function(ag) New AutoCompleteReturn.AutoCompleteItem((ag.AirportGroupID + 1000000).ToString,
			ag.Translations.Where(Function(translation) translation.LanguageID = LanguageID).FirstOrDefault.AirportGroup))

	End Function

	Private Function SearchTranslatedAutoCompleteDepartureAirports(SearchText As String, LanguageID As Integer) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAirports As IEnumerable(Of TranslatedAirport) = Lookups.TranslatedDepartureAirports.
			Where(Function(airport) airport.Translations.Where(Function(translation) translation.LanguageID = LanguageID).Any).
			Where(Function(airport) airport.Translations.Where(Function(translation) translation.Airport.ToLower.Contains(" " & SearchText.ToLower) OrElse
																   airport.IATACode.ToLower.Contains(" " & SearchText.ToLower) OrElse
																   translation.Airport.ToLower.StartsWith(SearchText.ToLower) OrElse
																   airport.IATACode.ToLower.StartsWith(SearchText.ToLower)).Any).
			OrderBy(Function(airport) airport.Translations.Where(Function(translation) translation.LanguageID = LanguageID).
															FirstOrDefault.Airport).
			OrderBy(Function(airport) IIf(airport.Translations.Where(Function(translation) translation.LanguageID = LanguageID).
															FirstOrDefault.Airport.ToLower.StartsWith(SearchText.ToLower), 0, 1))

		Return oAirports.Select(Function(airport) New AutoCompleteReturn.AutoCompleteItem(
			airport.AirportID.ToString, airport.Translations.Where(Function(translation) translation.LanguageID = LanguageID).
			FirstOrDefault.Airport & " (" & airport.IATACode & ")"))

	End Function

	Private Function SearchAutoCompeleteDepartureAirportGroups(SearchText As String) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAirportGroups As IEnumerable(Of AirportGroup) = Lookups.AirportGroups.
			Where(Function(ag) ag.Type = "Departure").
			Where(Function(ag) ag.AirportGroup.ToLower.Contains(" " & SearchText.ToLower) OrElse
					  ag.AirportGroup.ToLower.StartsWith(SearchText.ToLower)).
			OrderBy(Function(ag) ag.AirportGroup).
			OrderBy(Function(ag) Functions.IIf(ag.AirportGroup.StartsWith(SearchText.ToLower), 0, 1))

		Return oAirportGroups.Select(Function(ag) _
				New AutoCompleteReturn.AutoCompleteItem((ag.AirportGroupID + 1000000).ToString, ag.AirportGroup))

	End Function

	Private Function SearchAutoCompleteDepartureAirports(SearchText As String) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

		Dim oAirports As IEnumerable(Of Airport) = Lookups.DepartureAirports.
			Where(Function(a) a.Airport.ToLower.Contains(" " & SearchText.ToLower) OrElse
					  a.IATACode.ToLower.Contains(" " & SearchText.ToLower) OrElse
					  a.Airport.ToLower.StartsWith(SearchText.ToLower) OrElse
					  a.IATACode.ToLower.StartsWith(SearchText.ToLower)).
			OrderBy(Function(o) o.Airport).
			OrderBy(Function(o) Functions.IIf(o.Airport.StartsWith(SearchText.ToLower), 0, 1))

		Return oAirports.Select(Function(a) _
				New AutoCompleteReturn.AutoCompleteItem(a.AirportID.ToString, a.Airport & " (" & a.IATACode & ")"))

	End Function

    Private Function SearchAutoCompleteDepartureCountries(SearchText As String) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)

        Dim oCountries As IEnumerable(of Location) = Lookups.Locations.
                Where(Function(o) o.GeographyLevel1Name.ToLower.Contains(" " & SearchText.ToLower) OrElse
                                  o.GeographyLevel1Name.ToLower.StartsWith(SearchText.ToLower)).
                    OrderBy(Function(o) o.GeographyLevel1Name).
                    OrderBy(Function(o) Functions.IIf(o.GeographyLevel1Name.StartsWith(SearchText.ToLower), 0, 1))

        Return oCountries.GroupBy(Function(o) o.GeographyLevel1Name).
                Select(Function(a) New AutoCompleteReturn.AutoCompleteItem(a.First().GeographyLevel1ID.ToString, a.First().GeographyLevel1Name))
    End Function

    Private Function SearchAutoCompleteDepartureCarHireCountries(SearchText As String) As IEnumerable(Of AutoCompleteReturn.AutoCompleteItem)
        Dim oCountries As IEnumerable(Of KeyValuePair(Of Integer, String)) =
                Lookups.ListValidCarHireGeographies(BookingBase.Params.BrandID).
                    Where(Function(o) o.Value.ToLower.Contains(" " & SearchText.ToLower) OrElse
                                      o.Value.ToLower.StartsWith(SearchText.ToLower)).
                    OrderBy(Function(o) o.Value).
                    OrderBy(Function(o) Functions.IIf(o.Value.StartsWith(SearchText.ToLower), 0, 1))

        Return oCountries.GroupBy(Function(o) o.Value).
            Select(Function(a) New AutoCompleteReturn.AutoCompleteItem(a.First().Key.ToString, a.First().Value))
    End Function

	Public Class AutoCompleteReturn
		Public TextBoxID As String
		Public SelectedScript As String
		Public Items As New Generic.List(Of AutoCompleteItem)

		Public Class AutoCompleteItem
			Public Value As String 'This is usually an Integer ID but don't want to restrict this
			Public Display As String

			Public Sub New()
			End Sub

			Public Sub New(ByVal sValue As String, ByVal sDisplay As String)
				Me.Value = sValue
				Me.Display = sDisplay
			End Sub

		End Class

	End Class

#End Region

#Region "Dropdown Placeholder"

	Public Shared Function GetDropdownPlaceholder(ByVal sDropdownID As String, ByVal sOverrides As String) As String

		If Not sOverrides = "" Then
			Dim aOverrides As String() = sOverrides.Split("#"c)

			For Each sOverride As String In aOverrides

				Dim sID As String = sOverride.Split("|"c)(0)
				Dim sValue As String = sOverride.Split("|"c)(1)

				If sID = sDropdownID Then Return sValue

			Next
		End If

		Return ""

	End Function

#End Region

#Region "Get Default Region"

	Public Function GetDefaultRegion() As Integer

		Try
			If Me.PageDefinition.Content.ObjectType = "Country" Then
				Return Lookups.ListRegionsByCountryAndAirport(Me.PageDefinition.Content.ID, 0).First.Key
			ElseIf Me.PageDefinition.Content.ObjectType = "Region" Then
				Return Me.PageDefinition.Content.ID
			ElseIf Me.PageDefinition.Content.ObjectType = "Resort" Then
				Return -Me.PageDefinition.Content.ID
			End If
		Catch ex As Exception
			'just return 0
		End Try

		Return 0

	End Function

#End Region

#Region "Support Classes"

	'Public Class Airport
	'	Public AirportID As Integer
	'	Public AirportName As String
	'End Class

	Public Class PropertyReference
		Public PropertyReferenceID As Integer
		Public PropertyName As String
		Public GeographyLevel2ID As Integer
		Public GeographyLevel3ID As Integer
	End Class

	Public Class SearchToolReturn
		Public SearchReturn As New BookingSearch.SearchReturn
		Public SearchAvailabilities As New BookingAvailability.Availability
	End Class

	Public Class CustomSetting
		Public PlaceholderOverrides As String
		Public PreSearchScript As String
		Public DurationRange As String
		Public AdultsRange As String
		Public ChildrenRange As String
		Public InfantsRange As String
		Public IncludeGeographyLevel1Name As Boolean
		Public IncludeGeographyGroups As Boolean
		Public CheckAvailability As Boolean
		Public InsertWarningAfter As String
		Public ResetCookie As Boolean
		Public DepartureMode As LocationChooseMode
		Public DestinationMode As LocationChooseMode
		Public FlightOnlyDestinationMode As LocationChooseMode
		Public RegionDropdownScript As String
		Public QueryStringAutoPopulate As Boolean
		Public AlwaysIncludeResorts As Boolean
		Public AutoCompleteTranslated As Boolean
	End Class

	Public Enum LocationChooseMode
		AutoComplete
		Dropdown
		AutoCompleteAndDropdown
		'Same as auto complete and dropdown but with dropdown split to include country
		AutoCompleteAndDropdownWithCountry
		DropdownGrouped
		DropdownGroupedWithCountry
		DropdownGroupedWithIATA
		AutoCompleteAndDropdownGrouped
	End Enum

#End Region
End Class
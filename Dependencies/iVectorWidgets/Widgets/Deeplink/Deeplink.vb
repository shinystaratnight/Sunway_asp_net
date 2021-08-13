Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class Deeplink
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("deeplink_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("deeplink_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("deeplink_customsettings") = value
		End Set

	End Property

	Public Shared Property DeeplinkString As String
		Get
			Return SafeString(HttpContext.Current.Session("deeplinkstring"))
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("deeplinkstring") = value
		End Set
	End Property



	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .AddFlightToBasket = SafeBoolean(Settings.GetValue("AddFlightToBasket"))
            .AddHotelToBasketSingleResult = SafeBoolean(Settings.GetValue("AddHotelToBasketSingleResult"))
            .HotelPageRedirect = SafeBoolean(Settings.GetValue("HotelPageRedirect"))
            .FlightPlusHotelURL = SafeString(Settings.GetValue("FlightPlusHotelURL"))
            .SearchScript = SafeString(Settings.GetValue("SearchScript"))
            If SafeString(Settings.GetValue("RedirectURL")) <> "" Then
                .RedirectURL = SafeString(Settings.GetValue("RedirectURL"))
            End If

        End With
        Deeplink.CustomSettings = oCustomSettings



		'set deeplinkstring on session
		Deeplink.DeeplinkString = Me.Page.Request.QueryString.ToString

		'add javascript
		Dim sScript As String = IIf(Deeplink.CustomSettings.SearchScript <> "", Deeplink.CustomSettings.SearchScript, "int.ll.OnLoad.Run(function () { Deeplink.Search(); })")
		sScript = String.Format("<script type=""text/javascript"">{0}</script>", sScript)

		writer.Write(sScript)

	End Sub


	Public Shared Function Search() As String

		'set redirect URL
		Dim sRedirectURL As String = Deeplink.CustomSettings.RedirectURL

		Dim oDeeplinkReturn As New DeeplinkReturn

		Try
			'clear sessionsecure
			BookingBase.SearchDetails = New BookingSearch(BookingBase.Params, BookingBase.Markups, BookingBase.Lookups)
            BookingBase.SearchBasket = New BookingBasket(True)


			'get params
			Dim oParams As Hashtable = Functions.Web.ConvertQueryStringToHashTable(DeeplinkString.ToLower)


			'if package reference perform package search otherwise deeplink
			Dim oSearchReturn As New BookingSearch.SearchReturn

			If Deeplink.DeeplinkString.ToLower.Contains("packagereference") Then

				Dim sPackageReference As String = Functions.SafeString(oParams("packagereference"))
				oSearchReturn = BookingBase.SearchDetails.PackageSearch(sPackageReference)
			Else

				'create deeplink object from query string
				Dim oDeeplink As New BookingDeeplink(Deeplink.DeeplinkString)

				If oDeeplink.Valid Then
					oSearchReturn = oDeeplink.Search()
				Else
					oDeeplinkReturn.RedirectURL = "Invalid|" & oDeeplink.ErrorMessage
					oDeeplinkReturn.InvalidMessage = Intuitive.Web.Translation.GetCustomTranslation("Deeplink", "Invalid Deeplink URL")
					Return Newtonsoft.Json.JsonConvert.SerializeObject(oDeeplinkReturn)
				End If

			End If


			If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso oSearchReturn.FlightCount > 0 _
			  AndAlso oSearchReturn.PropertyCount > 0 Then

				'add flight to basket if set
				If Deeplink.CustomSettings.AddFlightToBasket Then
					BookingBase.SearchDetails.FlightResults.AddCheapestFlightToBasket()
				End If

				'perform initial filter and sort
				BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
				BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder, BookingBase.SearchDetails.PriorityPropertyID)

				'if flight plus hotel set selected flight
				If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
					BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight()
				End If

				BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter)
				BookingBase.SearchDetails.FlightResults.SortResults()

				'check for additional search
				Dim sAdditionalSearch As String = Functions.SafeString(oParams("additionalsearch"))

				'if additional search is for transfers, search for transfers
				If sAdditionalSearch.ToLower = "transfer" Then
					Dim oTransferSearchReturn As New BookingSearch.SearchReturn
					oTransferSearchReturn = BookingBase.SearchDetails.TransferSearchFromHotelResults()

					If oTransferSearchReturn.TransferCount > 0 Then
						BookingBase.SearchDetails.PropertyResults.SetupSelectedTransfer()
					End If
				End If

				'if additional search is for carhire, search for carhire
				If sAdditionalSearch.ToLower = "carhire" Then
					Dim oCarHireSearchReturn As New BookingSearch.SearchReturn
					oCarHireSearchReturn = BookingBase.SearchDetails.CarHireSearchFromResults
					
					If oCarHireSearchReturn.CarHireCount > 0 Then
						BookingBase.SearchDetails.PropertyResults.SetupSelectedCarHire()
					End If
				End If


				'setup basket guests from search
				BookingBase.SearchBasket.SetupBasketGuestsFromSearch()

				'set redirect
				If Deeplink.CustomSettings.HotelPageRedirect AndAlso (BookingBase.SearchDetails.PropertyReferenceID > 0 OrElse BookingBase.SearchDetails.PriorityPropertyID > 0) Then

					Dim iPropertyReferenceID As Integer = Functions.IIf(BookingBase.SearchDetails.PriorityPropertyID > 0, _
					  BookingBase.SearchDetails.PriorityPropertyID, BookingBase.SearchDetails.PropertyReferenceID)
					Dim oHotel As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(iPropertyReferenceID)
					sRedirectURL = Functions.IIf(oHotel.URL <> "", oHotel.URL, "/?warn=noresults")


				ElseIf Deeplink.CustomSettings.FlightPlusHotelURL <> "" Then
					sRedirectURL = Deeplink.CustomSettings.FlightPlusHotelURL
				Else
                    sRedirectURL = "/search-results"
				End If


			ElseIf BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly AndAlso oSearchReturn.PropertyCount > 0 Then

				'perform initial filter and sort
				BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
				BookingBase.SearchDetails.PropertyResults.SortResults(BookingBase.Params.HotelResults_DefaultSortBy, BookingBase.Params.HotelResults_DefaultSortOrder, BookingBase.SearchDetails.PriorityPropertyID)

				'setup basket guests from search
				BookingBase.SearchBasket.SetupBasketGuestsFromSearch()


				'add hotel to basket if set (only if we have single result)
				If Deeplink.CustomSettings.AddHotelToBasketSingleResult AndAlso (BookingBase.SearchDetails.PropertyReferenceID > 0 OrElse BookingBase.SearchDetails.PriorityPropertyID > 0) Then

					Dim iPropertyReferenceID As Integer = Functions.IIf(BookingBase.SearchDetails.PriorityPropertyID > 0, _
					  BookingBase.SearchDetails.PriorityPropertyID, BookingBase.SearchDetails.PropertyReferenceID)
					Dim oHotel As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(iPropertyReferenceID)


					If oHotel.Rooms.Count = 1 AndAlso BookingBase.SearchDetails.MealBasisID > 0 AndAlso _
					 oHotel.Rooms(0).RoomOptions.Where(Function(oRoomOption) oRoomOption.MealBasisID = BookingBase.SearchDetails.MealBasisID).Count = 1 Then

						Dim oRoomOption As PropertyResultHandler.Hotel.RoomOption = oHotel.Rooms(0).RoomOptions.Where(Function(o) o.MealBasisID = BookingBase.SearchDetails.MealBasisID).FirstOrDefault

						If Not oRoomOption Is Nothing Then
							Dim sToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(oRoomOption.Index)
							BookingProperty.AddRoomToBasket(sToken)
							sRedirectURL = "/booking-summary"
						End If

					ElseIf oHotel.Rooms.Count = 1 AndAlso oHotel.Rooms(0).RoomOptions.Count = 1 Then

						Dim oRoomOption As PropertyResultHandler.Hotel.RoomOption = oHotel.Rooms(0).RoomOptions(0)
						Dim sToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(oRoomOption.Index)

						BookingProperty.AddRoomToBasket(sToken)

						sRedirectURL = "/booking-summary"
					Else
						sRedirectURL = "/search-results"
					End If


					'redirect to hotel page if searching with prop ref id and setting turned on
				ElseIf Deeplink.CustomSettings.HotelPageRedirect AndAlso (BookingBase.SearchDetails.PropertyReferenceID > 0 OrElse BookingBase.SearchDetails.PriorityPropertyID > 0) Then
					Dim iPropertyReferenceID As Integer = Functions.IIf(BookingBase.SearchDetails.PriorityPropertyID > 0, _
					 BookingBase.SearchDetails.PriorityPropertyID, BookingBase.SearchDetails.PropertyReferenceID)
					Dim oHotel As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(iPropertyReferenceID)
					sRedirectURL = Functions.IIf(oHotel.URL <> "", oHotel.URL, "/?warn=noresults")
				Else
					sRedirectURL = "/search-results"
				End If


			ElseIf BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly AndAlso oSearchReturn.FlightCount > 0 Then

				BookingBase.SearchDetails.FlightResults.FilterResults(BookingBase.SearchDetails.FlightResults.ResultsFilter)
				BookingBase.SearchDetails.FlightResults.SortResults()

				'setup basket guests from search
				BookingBase.SearchBasket.SetupBasketGuestsFromSearch()

				'set redirect
				sRedirectURL = "/flight-results"
			ElseIf BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.TransferOnly AndAlso oSearchReturn.TransferCount > 0 Then

				'setup basket guests from search
				BookingBase.SearchBasket.SetupBasketGuestsFromSearch()

				'set redirect
				sRedirectURL = "/booking-summary"
			End If
		Catch ex As Exception
		End Try

        oDeeplinkReturn.RedirectURL = sRedirectURL

        'just before we log the search, set a Guid on the search details so we can link page views to searches in the log tables
        BookingBase.SearchDetails.SearchGuid = System.Guid.NewGuid.ToString
        BookingBase.DataLogger.LogSearch(BookingBase.SearchDetails)

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oDeeplinkReturn)

	End Function

	Public Class DeeplinkReturn
		Public RedirectURL As String
		Public InvalidMessage As String
	End Class

    Public Class CustomSetting
        Public AddFlightToBasket As Boolean
        Public AddHotelToBasketSingleResult As Boolean
        Public FlightPlusHotelURL As String
        Public HotelPageRedirect As Boolean
        Public SearchScript As String
        Public RedirectURL As String = "/?warn=noresults"
    End Class

End Class
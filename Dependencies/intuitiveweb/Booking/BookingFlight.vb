Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions

Public Class BookingFlight

#Region "Search"

	Public Shared Function Search(ByVal oBookingSearch As BookingSearch, ByVal oLookups As Lookups) As BookingSearch.SearchReturn

		oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightSearch, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.Flight.SearchRequest

		Try

			oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

			'Add details to the class
			With oiVectorConnectSearchRequest

				'set login details
				.LoginDetails = oBookingSearch.LoginDetails

				'Set either the airport group or airport id
				If oBookingSearch.DepartingFromID > 1000000 Then
					.DepartureAirportGroupID = oBookingSearch.DepartingFromID - 1000000
				Else
					.DepartureAirportID = oBookingSearch.DepartingFromID
				End If

				'geography and dates
				If oBookingSearch.ArrivingAtID > 2000000 Then
					.ArrivalAirportGroupID = oBookingSearch.ArrivingAtID - 2000000
				ElseIf oBookingSearch.ArrivingAtID > 1000000 Then
					.ArrivalAirportID = oBookingSearch.ArrivingAtID - 1000000
				ElseIf oBookingSearch.GeographyGroupingID > 0 Then
					Dim aResort As Generic.List(Of Integer) = oLookups.GeographyGroupGeographies(oBookingSearch.GeographyGroupingID)
					.Resorts.AddRange(aResort)
				ElseIf oBookingSearch.ArrivingAtID > 0 Then
					.RegionID = oBookingSearch.ArrivingAtID
				Else
					.Resorts.Add(oBookingSearch.ArrivingAtID * -1)
				End If

				.DepartureDate = oBookingSearch.DepartureDate
				.Duration = oBookingSearch.Duration
				.OneWay = oBookingSearch.OneWay
				.AllowMultisectorFlights = Not oBookingSearch.Direct
				.CarouselMode = oBookingSearch.Params.CarouselMode
				.ExactMatch = BookingBase.Params.ExactMatchesOnly OrElse oBookingSearch.SearchMode = BookingSearch.SearchModes.Anywhere
				.WidenSearch = oBookingSearch.WidenSearch
				.FlightClassID = oBookingSearch.FlightClassID

				If oBookingSearch.MaxResults > 0 then
                     .MaxResults = oBookingSearch.MaxResults
                End If

				'set flight and hotel flag
				.FlightAndHotel = oBookingSearch.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
                    OrElse oBookingSearch.SearchMode = BookingSearch.SearchModes.Anywhere

				.MultiCarrier = oBookingSearch.MultiCarrier


				'pax
				.GuestConfiguration.Adults = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
				.GuestConfiguration.Children = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Children)
				.GuestConfiguration.Infants = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
				For Each oRoomGuest As BookingSearch.Guest In oBookingSearch.RoomGuests
					.GuestConfiguration.ChildAges.AddRange(oRoomGuest.ChildAges)
				Next

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If

			End With

			oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

			'If everything is ok then send request
			If oSearchReturn.OK Then

				Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
				oHeaders.AddNew("processtimerguid", oBookingSearch.ProcessTimer.TimerGUID.ToString)

				oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				If oBookingSearch.EmailSearchTimes Then
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Flight.SearchResponse)(oiVectorConnectSearchRequest, oBookingSearch.EmailTo, Headers:=oHeaders)
				Else
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Flight.SearchResponse)(oiVectorConnectSearchRequest, Headers:=oHeaders)
				End If

				oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

				Dim oSearchResponse As New ivci.Flight.SearchResponse

				Dim oRequestInfo As New BookingSearch.RequestInfo
				With oRequestInfo
					.RequestTime = oIVCReturn.RequestTime
					.RequestXML = oIVCReturn.RequestXML
					.ResponseXML = oIVCReturn.ResponseXML
					.NetworkLatency = oIVCReturn.NetworkLatency
					.Type = BookingSearch.RequestInfoType.FlightSearch
				End With

				oSearchReturn.RequestInfo = oRequestInfo

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.Flight.SearchResponse)

					'if we have results set search return
					If oSearchResponse.Flights.Count > 0 Then
						oSearchReturn.FlightResults = oSearchResponse.Flights
						oSearchReturn.FlightCount = oSearchResponse.Flights.Count
						oSearchReturn.ExactMatchFlightCount = oSearchResponse.Flights.Where(Function(o) o.ExactMatch = True).Count
					End If

				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.FlightCount = 0
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/FlightSearch", "FlightSearchException", ex.ToString)
		End Try

		oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightSearch, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

		Return oSearchReturn

	End Function

#End Region

#Region "Flight Carousel"

	Public Class FlightCarousel

		Public Shared Function Search(ByVal oBookingSearch As BookingSearch, ByVal DaysEitherSide As Integer, ByVal oLookups As Lookups) As BookingSearch.SearchReturn

			Dim oSearchReturn As New BookingSearch.SearchReturn
			Dim oiVectorConnectFlightCarouselRequest As New ivci.FlightCarouselRequest

			Try

				'Add details to the class
				With oiVectorConnectFlightCarouselRequest

					'set login details
					.LoginDetails = oBookingSearch.LoginDetails

					'Set either the airport group or airport id
					If oBookingSearch.DepartingFromID > 1000000 Then
						.DepartureAirportGroupID = oBookingSearch.DepartingFromID - 1000000
					Else
						.DepartureAirportID = oBookingSearch.DepartingFromID
					End If

					'geography and dates
					If oBookingSearch.ArrivingAtID > 0 Then
						.RegionID = oBookingSearch.ArrivingAtID
					Else
						.Resorts.Add(oBookingSearch.ArrivingAtID * -1)
					End If

					If oBookingSearch.GeographyGroupingID > 0 Then

						Dim oGeographyGroup As New Lookups.GeographyGrouping
						oGeographyGroup = oLookups.GeographyGroupings.Where(Function(o) o.GeographyGroupingID = oBookingSearch.GeographyGroupingID).FirstOrDefault

						If oGeographyGroup IsNot Nothing AndAlso oGeographyGroup.Level = "Resort" Then

							Dim oGeographyGroupGeographies As New Generic.List(Of Integer)
							oGeographyGroupGeographies = oLookups.GeographyGroupGeographies(oGeographyGroup.GeographyGroupingID)

							.Resorts.AddRange(oGeographyGroupGeographies)
						End If

					End If

					.DepartureDate = oBookingSearch.DepartureDate
					.Duration = oBookingSearch.Duration
					.OneWay = False
					.DaysEitherSide = DaysEitherSide

					'Do the iVectorConnect validation procedure
					Dim oWarnings As Generic.List(Of String) = .Validate()

					If oWarnings.Count > 0 Then
						oSearchReturn.OK = False
						oSearchReturn.Warning.AddRange(oWarnings)
					End If

				End With

				'If everything is ok then send request
				If oSearchReturn.OK Then

					Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.FlightCarouselResponse)(oiVectorConnectFlightCarouselRequest)

					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.Type = BookingSearch.RequestInfoType.FlightSearch
					End With

					oSearchReturn.RequestInfo = oRequestInfo

					Dim oFlightCarouselResponse As New ivci.FlightCarouselResponse

					If oIVCReturn.Success Then

						oFlightCarouselResponse = CType(oIVCReturn.ReturnObject, ivci.FlightCarouselResponse)

						If oFlightCarouselResponse.Dates.Count > 0 Then
							oSearchReturn.FlightCarouselDates = oFlightCarouselResponse.Dates
							oSearchReturn.FlightCarouselCount = oFlightCarouselResponse.Dates.Count
						End If

					Else
						oSearchReturn.OK = False
						oSearchReturn.Warning = oIVCReturn.Warning
					End If

				End If

			Catch ex As Exception
				oSearchReturn.OK = False
				oSearchReturn.Warning.Add(ex.ToString)
				FileFunctions.AddLogEntry("iVectorConnect/FlightCarousel", "FlightCarouselException", ex.ToString)
			End Try

			Return oSearchReturn

		End Function

		Public Shared Sub GenerateFromFlightResults()

			Dim oDates As Generic.List(Of ivci.FlightCarouselResponse.Date) = FlightCarouselSearch()

			'save dates
			Results.Save(oDates)

		End Sub

		Public Shared Function FlightCarouselSearch() As Generic.List(Of ivci.FlightCarouselResponse.Date)

			Dim oDates As New Generic.List(Of ivci.FlightCarouselResponse.Date)

			Dim dStartDate As Date = BookingBase.SearchDetails.DepartureDate.AddDays(-BookingBase.Params.FlightCarouselDaysEitherSide)
			Dim dEndDate As Date = BookingBase.SearchDetails.DepartureDate.AddDays(BookingBase.Params.FlightCarouselDaysEitherSide)

			Dim dCurrentDate As Date = dStartDate

			While dCurrentDate <= dEndDate

				Dim oDate As New ivci.FlightCarouselResponse.Date
				oDate.Date = dCurrentDate
				oDate.AvailableFlights = BookingBase.SearchDetails.FlightResults.WorkTable.Where(Function(o) o.OutboundDepartureDate = dCurrentDate).Count

				If oDate.AvailableFlights > 0 Then
					Dim nFlightPrice As Decimal = BookingBase.SearchDetails.FlightResults.WorkTable.Where(Function(o) o.OutboundDepartureDate = dCurrentDate).Min(Function(o) o.Price)

					oDate.FlightFromPrice = nFlightPrice / (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
				End If

				oDates.Add(oDate)

				dCurrentDate = dCurrentDate.AddDays(1)

			End While

			Return oDates

		End Function

		'Confusingly the search function does a carousel search that just grabs the results from the cache, mark made a new calander search request that does
		'what a search actually should do.
		Public Shared Sub GenerateFromCalendarSearch(ByVal oBookingSearch As BookingSearch, ByVal DaysEitherSide As Integer)

			Dim oCalanderSearchRequest As New ivci.CalendarSearchRequest

			Try
				'Build up search request
				With oCalanderSearchRequest
					.LoginDetails = oBookingSearch.LoginDetails

					.SearchType = "Flight"

					.Adults = oBookingSearch.TotalAdults
					.Children = oBookingSearch.TotalChildren

					'if airport group (id > 1000000) set airport group id otherwise set departure airport
					If oBookingSearch.DepartingFromID > 1000000 Then
						.DepartureAirportGroupID = oBookingSearch.DepartingFromID - 1000000
					Else
						.DepartureAirportID = oBookingSearch.DepartingFromID
					End If

					If oBookingSearch.ArrivingAtID > 2000000 Then
						.ArrivalAirportGroupID = oBookingSearch.ArrivingAtID - 2000000
					ElseIf oBookingSearch.ArrivingAtID > 1000000 Then
						.ArrivalAirportID = oBookingSearch.ArrivingAtID - 1000000
					ElseIf oBookingSearch.ArrivingAtID > 0 Then
						.ArrivalGeographyLevel2ID = oBookingSearch.ArrivingAtID
					Else
						.ArrivalGeographyLevel3ID = oBookingSearch.ArrivingAtID * -1
					End If

					.DurationStart = oBookingSearch.Duration
					.DurationEnd = oBookingSearch.Duration
					.OneWay = oBookingSearch.OneWay
					.DepartureDateStart = DateAdd(DateInterval.Day, -BookingBase.Params.FlightCarouselDaysEitherSide, oBookingSearch.DepartureDate)
					.DepartureDateEnd = DateAdd(DateInterval.Day, BookingBase.Params.FlightCarouselDaysEitherSide, oBookingSearch.DepartureDate)
					If .DepartureDateStart < Today Then .DepartureDateStart = Today
					.NeoCache = BookingBase.Params.FlightCarouselNeoCache

				End With

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CalendarSearchResponse)(oCalanderSearchRequest)

				Dim oCalanderSearchResponse As New ivci.CalendarSearchResponse

				If oIVCReturn.Success Then

					oCalanderSearchResponse = CType(oIVCReturn.ReturnObject, ivci.CalendarSearchResponse)

					'Do a carousel search as well
					Dim oCarouselDates As New Generic.List(Of ivci.FlightCarouselResponse.Date)
					If BookingBase.Params.FlightCalendarCarouselSearch Then
						oCarouselDates = FlightCarouselSearch()
					End If

					If oCalanderSearchResponse.Days.Count > 0 Then

						'Unlike the other search calendar search does not return nodes for days with no results we need to insert them
						Dim dayCount As Date = oCalanderSearchRequest.DepartureDateStart

						'Loop through all days between start and end date
						Do While (dayCount <= oCalanderSearchRequest.DepartureDateEnd)

							'If a node doesnt exist, make one
							If Not oCalanderSearchResponse.Days.Exists(Function(x) x.Date = dayCount) Then

								Dim oDay As New ivci.CalendarSearchResponse.Day

								With oDay
									.TotalPrice = 0
									.Date = dayCount
								End With

								oCalanderSearchResponse.Days.Add(oDay)

							End If

							dayCount = dayCount.AddDays(1)

						Loop

						'combine results
						If BookingBase.Params.FlightCalendarCarouselSearch Then
							For Each oDay As ivci.CalendarSearchResponse.Day In oCalanderSearchResponse.Days

								Dim oInnerDay As ivci.CalendarSearchResponse.Day = oDay

								Dim oCarouselDate As ivci.FlightCarouselResponse.Date = CType(oCarouselDates.Where(Function(o) o.Date = oInnerDay.Date).FirstOrDefault, ivci.FlightCarouselResponse.Date)

								If oCarouselDate.FlightFromPrice < oDay.TotalPrice AndAlso oCarouselDate.FlightFromPrice > 0 OrElse oDay.TotalPrice = 0 Then
									oDay.TotalPrice = oCarouselDate.FlightFromPrice
								End If

							Next
						End If

						'save dates
						Results.Save(oCalanderSearchResponse.Days)

					End If

				Else

				End If

			Catch ex As Exception

			End Try

		End Sub

		Public Class Results
			Public Dates As New Generic.List(Of ivci.FlightCarouselResponse.Date)

			Public ReadOnly Property TotalValidDates As Integer
				Get
					Dim iCount As Integer = 0
					For Each oDate As ivci.FlightCarouselResponse.Date In Me.Dates
						If oDate.AvailableFlights > 0 Then
							iCount += 1
						End If
					Next
					Return iCount
				End Get
			End Property

			Public Shared Sub Save(ByVal Dates As Generic.List(Of ivci.FlightCarouselResponse.Date))

				'save results
				Dim oFlightCarouselResults As New FlightCarousel.Results
				oFlightCarouselResults.Dates = Dates

				'sort results by date
				oFlightCarouselResults.Dates.Sort(New SortByDate)

				'save on session
				BookingBase.SearchDetails.FlightCarouselResults = oFlightCarouselResults

			End Sub

			Public Shared Sub Save(ByVal Days As Generic.List(Of ivci.CalendarSearchResponse.Day))

				'save results
				Dim oFlightCarouselResults As New FlightCarousel.Results

				For Each oDay As ivci.CalendarSearchResponse.Day In Days
					Dim oDate As New ivci.FlightCarouselResponse.Date

					With oDate
						.AvailableFlights = IIf(oDay.TotalPrice > 0, 1, 0)
						.Date = oDay.Date
						.FlightFromPrice = oDay.TotalPrice
						.PropertyFromPrice = 0D
					End With

					oFlightCarouselResults.Dates.Add(oDate)

				Next

				'sort results by date
				oFlightCarouselResults.Dates.Sort(New SortByDate)

				'save on session
				BookingBase.SearchDetails.FlightCarouselResults = oFlightCarouselResults

			End Sub

			'return XML
			Public Function GetResultXML() As XmlDocument

				'serialize to xml and return
				Dim oResultXML As XmlDocument = Serializer.Serialize(Me, True)
				Return oResultXML

			End Function

			Public Function GetResultXML(ByVal DaysEitherSide As Integer) As XmlDocument

				Dim oResults As New Results
				Dim iBookAheadDays As Integer = BookingBase.Params.Search_BookAheadDays
				Dim dBookAheadStartDate As DateTime = DateTime.Today.AddDays(iBookAheadDays)
				Dim dStartDate As Date = BookingBase.SearchDetails.DepartureDate.AddDays(-DaysEitherSide)
				Dim dEndDate As Date = BookingBase.SearchDetails.DepartureDate.AddDays(DaysEitherSide)


				For Each oDate As ivci.FlightCarouselResponse.Date In Me.Dates

					If oDate.Date >= dBookAheadStartDate AndAlso oDate.Date >= dStartDate AndAlso oDate.Date <= dEndDate Then
						oResults.Dates.Add(oDate)
					End If

				Next

				'serialize to xml and return
				Dim oResultXML As XmlDocument = Serializer.Serialize(oResults, True)
				Return oResultXML

			End Function

		End Class

		Public Class SortByDate
			Implements IComparer(Of ivci.FlightCarouselResponse.Date)

			Overloads Function Compare(ByVal x As ivci.FlightCarouselResponse.Date, ByVal y As ivci.FlightCarouselResponse.Date) As Integer Implements IComparer(Of ivci.FlightCarouselResponse.Date).Compare

				If x.Date > y.Date Then
					Return 1
				ElseIf x.Date < y.Date Then
					Return -1
				Else
					Return 0
				End If

			End Function

		End Class

	End Class

#End Region

#Region "Results"

	'	Public Class Results

	'#Region "Properties"

	'		Public TotalFlights As Integer
	'        Public TotalExactMatchFlights As Integer

	'		<XmlIgnore()>
	'		Public AllFlights As New Generic.List(Of Flight)
	'		<XmlIgnore()>
	'		Public FilteredFlights As New Generic.List(Of Flight)

	'		Public Flights As New Generic.List(Of Flight)
	'		Public AlternativeFlights As New Generic.List(Of Flight)

	'        Public Filters As New Filter
	'        Public DefaultFilters As New Filter

	'        Public FlightsPerPage As Integer = 10
	'		Public CurrentPage As Integer = 1

	'		Public ReadOnly Property MinPrice As Decimal
	'			Get
	'				If Me.AllFlights.Where(Function(o) o.ExactMatch).Count > 0 Then
	'					Return Me.AllFlights.Where(Function(o) o.ExactMatch).Min(Function(o) o.Total)
	'				Else
	'					Return 0
	'				End If
	'			End Get
	'		End Property

	'#End Region

	'#Region "Save Results"

	'        Public Shared Sub Save(ByVal FlightResults As Generic.List(Of ivci.Flight.SearchResponse.Flight))

	'			Dim oFlightResults As New Results

	'			'Get Flight Markups
	'			Dim aFlightMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Flight AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

	'            'loop through flight results and add to collection
	'            For Each oFlightResult As ivci.Flight.SearchResponse.Flight In FlightResults

	'                'if we only want direct flights skip to next flight result if flight is not direct
	'                If BookingBase.Params.DirectFlightsOnly AndAlso _
	'                 (oFlightResult.NumberOfOutboundStops > 0 OrElse oFlightResult.NumberOfReturnStops > 0) Then Continue For

	'                'create result flight
	'                Dim oFlight As New Results.Flight
	'                With oFlight
	'                    .BookingToken = oFlightResult.BookingToken
	'                    .ExactMatch = oFlightResult.ExactMatch

	'                    'flight carrier
	'                    .FlightCarrierID = oFlightResult.FlightCarrierID
	'                    .FlightCarrier = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, oFlightResult.FlightCarrierID)
	'					.FlightCarrierLogo = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierLogo, oFlightResult.FlightCarrierID)
	'					.FlightCarrierType = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierType, oFlightResult.FlightCarrierID)

	'                    'airports
	'                    .DepartureAirportID = oFlightResult.DepartureAirportID
	'                    .DepartureAirport = Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportGroupAndAirport, oFlightResult.DepartureAirportID)
	'                    .DepartureAirportCode = Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResult.DepartureAirportID)
	'                    .ArrivalAirportID = oFlightResult.ArrivalAirportID
	'                    .ArrivalAirport = Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResult.ArrivalAirportID)
	'                    .ArrivalAirportCode = Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResult.ArrivalAirportID)

	'                    'outbound details
	'                    .OutboundFlightCode = oFlightResult.OutboundFlightCode
	'                    .OutboundDepartureDate = oFlightResult.OutboundDepartureDate
	'                    .OutboundDepartureTime = oFlightResult.OutboundDepartureTime
	'                    .OutboundArrivalDate = oFlightResult.OutboundArrivalDate
	'                    .OutboundArrivalTime = oFlightResult.OutboundArrivalTime
	'					.OutboundNumberOfStops = oFlightResult.NumberOfOutboundStops
	'					.OutboundFlightClassID = oFlightResult.OutboundFlightClassID
	'					.OutboundFlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResult.OutboundFlightClassID)

	'                    'return details
	'                    .ReturnFlightCode = oFlightResult.ReturnFlightCode
	'                    .ReturnDepartureDate = oFlightResult.ReturnDepartureDate
	'                    .ReturnDepartureTime = oFlightResult.ReturnDepartureTime
	'                    .ReturnArrivalDate = oFlightResult.ReturnArrivalDate
	'                    .ReturnArrivalTime = oFlightResult.ReturnArrivalTime
	'					.ReturnNumberOfStops = oFlightResult.NumberOfReturnStops
	'					.ReturnFlightClassID = oFlightResult.ReturnFlightClassID
	'					.ReturnFlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResult.ReturnFlightClassID)

	'					'baggage
	'					.IncludesSupplierBaggage = oFlightResult.IncludesSupplierBaggage
	'					.TotalBaggagePrice = oFlightResult.TotalBaggagePrice

	'                    'total price
	'					.Total = oFlightResult.TotalPrice

	'					'supplier and costs
	'					.SupplierID = oFlightResult.SupplierDetails.SupplierID
	'					.LocalCost = oFlightResult.SupplierDetails.Cost
	'					.LocalCostCurrencyID = oFlightResult.SupplierDetails.CurrencyID

	'					Dim oFlightSectors As New List(Of FlightSector)
	'					For Each oFlightResultSector As ivci.Support.FlightSector In oFlightResult.FlightSectors
	'						Dim oFlightSector As New FlightSector

	'						With oFlightSector
	'							.ArrivalAirportID = oFlightResultSector.ArrivalAirportID
	'							.ArrivalAirport = Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResultSector.ArrivalAirportID)
	'							.ArrivalAirportCode = Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResultSector.ArrivalAirportID)
	'							.ArrivalDate = oFlightResultSector.ArrivalDate
	'							.ArrivalTime = oFlightResultSector.ArrivalTime
	'							.DepartureAirportID = oFlightResultSector.DepartureAirportID
	'							.DepartureAirport = Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResultSector.DepartureAirportID)
	'							.DepartureAirportCode = Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResultSector.DepartureAirportID)
	'							.DepartureDate = oFlightResultSector.DepartureDate
	'							.DepartureTime = oFlightResultSector.DepartureTime
	'							.Direction = oFlightResultSector.Direction
	'							.FlightCarrierID = oFlightResultSector.FlightCarrierID
	'							.FlightCarrier = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, oFlightResultSector.FlightCarrierID)
	'							.FlightCarrierLogo = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierLogo, oFlightResultSector.FlightCarrierID)
	'							.FlightCode = oFlightResultSector.FlightCode
	'							.FlightClassID = oFlightResultSector.FlightClassID
	'							.FlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResultSector.FlightClassID)
	'							.Seq = oFlightResultSector.Seq
	'							.NumberOfStops = Intuitive.Functions.SafeInt(oFlightResultSector.NumberOfStops)
	'							.FlightTime = Intuitive.Functions.SafeInt(oFlightResultSector.FlightTime)
	'							.TravelTime = Intuitive.Functions.SafeInt(oFlightResultSector.TravelTime)
	'							.VehicleName = Lookups.GetKeyPairValue(Lookups.LookupTypes.Vehicle, Intuitive.Functions.SafeInt(oFlightResultSector.VehicleID))
	'						End With

	'						oFlightSectors.Add(oFlightSector)

	'					Next

	'					.FlightSectors = oFlightSectors

	'				End With

	'				'create flight option
	'				Dim oFlightOption As New BasketFlight.FlightOption
	'				With oFlightOption
	'					.BookingToken = oFlight.BookingToken
	'					.Price = oFlight.Total
	'					.SupplierID = oFlightResult.SupplierDetails.SupplierID
	'					.LocalCost = oFlightResult.SupplierDetails.Cost
	'					.CurrencyID = oFlightResult.SupplierDetails.CurrencyID
	'					.OutboundFlightCode = oFlight.OutboundFlightCode
	'					.ReturnFlightCode = oFlight.ReturnFlightCode
	'					.OutboundDepartureDate = oFlight.OutboundDepartureDate
	'					.OutboundDepartureTime = oFlight.OutboundDepartureTime
	'					.OutboundArrivalDate = oFlight.OutboundArrivalDate
	'					.OutboundArrivalTime = oFlight.OutboundArrivalTime
	'					.ReturnDepartureDate = oFlight.ReturnDepartureDate
	'					.ReturnDepartureTime = oFlight.ReturnDepartureTime
	'					.ReturnArrivalDate = oFlight.ReturnArrivalDate
	'					.ReturnArrivalTime = oFlight.ReturnArrivalTime
	'                    .DepartureAirportID = oFlight.DepartureAirportID
	'                    .ArrivalAirportID = oFlight.ArrivalAirportID
	'					.FlightCarrierID = oFlight.FlightCarrierID
	'					.FlightCarrierType = oFlight.FlightCarrierType
	'					.Adults = BookingBase.SearchDetails.TotalAdults
	'					.Children = BookingBase.SearchDetails.TotalChildren
	'					.Infants = BookingBase.SearchDetails.TotalInfants
	'					.FlightPlusHotel = IIf(BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel, True, False)
	'				End With
	'				'add markup (after creating flight option)
	'				For Each oMarkup As BookingBase.Markup In aFlightMarkups
	'					Select Case oMarkup.Type
	'						Case BookingBase.Markup.eType.Amount
	'							oFlight.MarkupAmount += oMarkup.Value
	'						Case BookingBase.Markup.eType.AmountPP
	'							oFlight.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
	'						Case BookingBase.Markup.eType.Percentage
	'							oFlight.MarkupPercentage = oMarkup.Value
	'					End Select
	'				Next

	'				'add mark up to total
	'				oFlight.Total += oFlight.MarkupAmount
	'				oFlight.Total *= (oFlight.MarkupPercentage / 100) + 1

	'				'set hash token
	'				oFlight.FlightOptionHashToken = oFlightOption.GenerateHashToken()

	'				'add flight to results
	'				oFlightResults.Flights.Add(oFlight)

	'			Next

	'            'set total
	'            oFlightResults.TotalFlights = oFlightResults.Flights.Count
	'            oFlightResults.TotalExactMatchFlights = oFlightResults.Flights.Where(Function(o) o.ExactMatch = True).Count

	'            'save all flights before we start filtering and sorting
	'            oFlightResults.AllFlights = Results.CloneFlights(oFlightResults.Flights)

	'            'Initial filter, and sort
	'            oFlightResults.Filters.OutboundDepartureDate = BookingBase.SearchDetails.DepartureDate

	'            oFlightResults.FilterResults()
	'			'SortResults is already called in the FilterResults method called above?
	'			'oFlightResults.SortResults()

	'            'must come after the filter results method
	'            oFlightResults.DefaultFilters = Results.CloneFilter(oFlightResults.Filters)

	'            'save on session
	'            BookingBase.SearchDetails.FlightResults = oFlightResults

	'        End Sub

	'#End Region

	'#Region "Get Default Flight"

	'		Public Function GetDefaultFlightPerResort(aResorts As IEnumerable(Of Integer)) As Dictionary(Of Integer, Results.Flight)

	'			'sort flights
	'			Me.AllFlights = Me.AllFlights.OrderBy(Function(oFlight) oFlight.Total).ToList

	'			Dim oDictionary As New Dictionary(Of Integer, Results.Flight)

	'			For Each iResort In aResorts

	'				Dim oFlight As Flight = Me.GetDefaultFlight(iResort)

	'				If Not oFlight Is Nothing Then
	'					oDictionary.Add(iResort, oFlight)
	'				End If

	'			Next

	'			Return oDictionary

	'		End Function

	'		Public Function GetDefaultFlight(ByVal GeographyLevel3ID As Integer) As Flight

	'			Dim oDefaultFlight As New Flight

	'			'Find the cheapest valid flight based on resort
	'			oDefaultFlight = Me.AllFlights _
	'			  .Where(Function(oFlight) oFlight.ExactMatch AndAlso (Lookups.AirportResortCheck(oFlight.ArrivalAirportID, GeographyLevel3ID))) _
	'			   .FirstOrDefault()

	'			Return oDefaultFlight

	'		End Function

	'#End Region

	'#Region "Get Single Flight"

	'        'get single flight (booking token)
	'        Public Function GetSingleFlight(ByVal BookingToken As String) As Flight

	'            Dim oFlight As Flight = Me.AllFlights.Where(Function(o) o.BookingToken = BookingToken).FirstOrDefault
	'            Return oFlight

	'        End Function

	'#End Region

	'		'#Region "Add Cheapest Flight To Basket"

	'		'		Public Sub AddCheapestFlightToBasket()

	'		'			'Find the cheapest valid flight
	'		'			Dim oDefaultFlight As New Flight
	'		'			oDefaultFlight = Me.AllFlights.Where(Function(oFlight) oFlight.ExactMatch).OrderBy(Function(oFlight) oFlight.Total).FirstOrDefault()

	'		'			'add to basket
	'		'			BookingFlight.AddFlightToBasket(oDefaultFlight.FlightOptionHashToken)

	'		'		End Sub

	'		'#End Region

	'#Region "Flight Class"

	'		Public Class Flight

	'			Public BookingToken As String
	'			Public FlightOptionHashToken As String

	'			Public ExactMatch As Boolean

	'			'flight carrier
	'			Public FlightCarrierID As Integer
	'			Public FlightCarrier As String
	'			Public FlightCarrierLogo As String
	'			Public FlightCarrierType As String

	'			'airports
	'			Public DepartureAirportID As Integer
	'			Public DepartureAirport As String
	'			Public DepartureAirportCode As String
	'			Public ArrivalAirportID As Integer
	'			Public ArrivalAirport As String
	'			Public ArrivalAirportCode As String

	'			'outbound flight
	'			Public OutboundFlightCode As String
	'			Public OutboundDepartureDate As Date
	'			Public OutboundDepartureTime As String
	'			Public OutboundArrivalDate As Date
	'			Public OutboundArrivalTime As String
	'			Public OutboundNumberOfStops As Integer
	'			Public OutboundFlightClassID As Integer
	'			Public OutboundFlightClass As String

	'			Public ReadOnly Property OutBoundDepartureTimeOfDay As eTimeOfDay
	'				Get
	'					Return Results.FlightTimeOfDay(Me.OutboundDepartureTime)
	'				End Get
	'			End Property

	'			'return flight
	'			Public ReturnFlightCode As String
	'			Public ReturnDepartureDate As Date
	'			Public ReturnDepartureTime As String
	'			Public ReturnArrivalDate As Date
	'			Public ReturnArrivalTime As String
	'			Public ReturnNumberOfStops As Integer
	'			Public ReturnFlightClassID As Integer
	'			Public ReturnFlightClass As String

	'			Public ReadOnly Property ReturnDepartureTimeOfDay As eTimeOfDay
	'				Get
	'					Return Results.FlightTimeOfDay(Me.ReturnDepartureTime)
	'				End Get
	'			End Property

	'			'baggage
	'			Public IncludesSupplierBaggage As Boolean
	'			Public TotalBaggagePrice As Decimal

	'			'price
	'			Public MarkupAmount As Decimal
	'			Public MarkupPercentage As Decimal
	'			Public Total As Decimal

	'			'Supplier Details
	'			Public SupplierID As Integer
	'			Public LocalCost As Decimal
	'			Public LocalCostCurrencyID As Integer

	'			'flight sectors
	'			Public FlightSectors As New List(Of FlightSector)

	'			'MaxStops
	'			Public ReadOnly Property MaxStops As Integer
	'				Get
	'					Return Math.Max(Me.OutboundNumberOfStops, Me.ReturnNumberOfStops)
	'				End Get
	'			End Property

	'		End Class

	'		Public Class FlightSector
	'			Public ArrivalAirportID As Integer
	'			Public ArrivalAirport As String
	'			Public ArrivalAirportCode As String
	'			Public ArrivalDate As Date
	'			Public ArrivalTime As String
	'			Public DepartureAirportID As Integer
	'			Public DepartureAirport As String
	'			Public DepartureAirportCode As String
	'			Public DepartureDate As Date
	'			Public DepartureTime As String
	'			Public Direction As String
	'			Public FlightCarrierID As Integer
	'			Public FlightCarrier As String
	'			Public FlightCarrierLogo As String
	'			Public FlightCode As String
	'			Public FlightClassID As Integer
	'			Public FlightClass As String
	'			Public Seq As Integer
	'			Public NumberOfStops As Integer
	'			Public FlightTime As Integer
	'			Public TravelTime As Integer
	'			Public VehicleName As String
	'		End Class

	'#End Region

	'#Region "Get Page"

	'		'return XML
	'		Public Function GetPageXML(ByVal PageNumber As Integer) As XmlDocument

	'			Me.GetPageFlights(PageNumber)

	'			'serialize to xml and return
	'			Dim oResultXML As XmlDocument = Serializer.Serialize(Me, True)
	'			Return oResultXML

	'		End Function

	'		'return list of hotels
	'		Public Function GetPageFlights(ByVal PageNumber As Integer) As Generic.List(Of Flight)

	'			'Check bookingbase for number of flights per page
	'			If BookingBase.Params.FlightResultsPerPage > 0 Then
	'				Me.FlightsPerPage = BookingBase.Params.FlightResultsPerPage
	'			End If

	'			'set current page
	'			Me.CurrentPage = PageNumber

	'			'apply paging
	'			Dim iStartIndex As Integer = (Me.CurrentPage - 1) * Me.FlightsPerPage

	'			Dim iCount As Integer = Functions.IIf(Me.FilteredFlights.Count < Me.FlightsPerPage, Me.FilteredFlights.Count, Me.FlightsPerPage)
	'			iCount = Functions.IIf(iStartIndex + iCount > Me.FilteredFlights.Count, Me.FilteredFlights.Count - iStartIndex, iCount)

	'			Me.Flights = Me.FilteredFlights.GetRange(iStartIndex, iCount)

	'			'save on session
	'			BookingBase.SearchDetails.FlightResults = Me

	'			Return Me.Flights

	'		End Function

	'#End Region

	'#Region "Filter Class"

	'		Public Class Filter

	'			'outbound departure date
	'			Public OutboundDepartureDate As Date

	'			'filter settings
	'			Public MinPrice As Decimal = 0
	'			Public MaxPrice As Decimal = 0
	'			Public FilterDepartureAirportIDs As New Generic.List(Of Integer)
	'			Public FilterArrivalAirportIDs As New Generic.List(Of Integer)
	'			Public FilterFlightCarrerIDs As New Generic.List(Of Integer)
	'			Public FilterDepartureTimes As New Generic.List(Of eTimeOfDay)
	'			Public FilterReturnTimes As New Generic.List(Of eTimeOfDay)
	'			Public FilterStops As New Generic.List(Of Integer)
	'			Public FilterFlightClassIDs As New Generic.List(Of Integer)
	'			Public BookingTokenCSV As String = ""
	'			Public MinOutboundDepartureFlightTimeMinutes As Integer
	'			Public MaxOutboundDepartureFlightTimeMinutes As Integer = 1439
	'			Public MinReturnDepartureFlightTimeMinutes As Integer
	'			Public MaxReturnDepartureFlightTimeMinutes As Integer = 1439

	'			Public IncludesSupplierBaggage As Boolean

	'			Public SortBy As eSortBy = eSortBy.Price
	'			Public SortOrder As eSortOrder = eSortOrder.Ascending

	'			'filter counts and from prices
	'			'times
	'			Public DepartureTimes As New Generic.List(Of FlightTime)
	'			Public ReturnTimes As New Generic.List(Of FlightTime)
	'			Public MinOutboundDepartureFlightTime As String
	'			Public MaxOutboundDepartureFlightTime As String
	'			Public MinReturnDepartureFlightTime As String
	'			Public MaxReturnDepartureFlightTime As String

	'			'airports
	'			Public DepartureAirports As New Generic.List(Of Airport)
	'			Public ArrivalAirports As New Generic.List(Of Airport)

	'			'carriers
	'			Public FlightCarriers As New Generic.List(Of FlightCarrier)

	'			'stops - will be max stops from either outbound or return leg
	'			Public Stops As New Generic.List(Of FlightStop)

	'			'Flight classes
	'			Public FlightClasses As New Generic.List(Of FlightClass)

	'			Public Class FlightStop
	'				Public Stops As Integer
	'				Public Count As Integer
	'				Public FromPrice As Decimal
	'				Public Selected As Boolean
	'			End Class

	'			Public Class FlightTime
	'				Public TimeOfDay As eTimeOfDay
	'				Public Count As Integer
	'				Public FromPrice As Decimal
	'			End Class

	'			Public Class FlightCarrier
	'				Public FlightCarrierID As Integer
	'				Public FlightCarrier As String
	'				Public FlightCarrierLogo As String
	'				Public Count As Integer
	'				Public FromPrice As Decimal
	'				Public Selected As Boolean
	'			End Class

	'			Public Class Airport
	'				Public AirportID As Integer
	'				Public AirportName As String
	'				Public AirportCode As String
	'				Public Count As Integer
	'				Public Selected As Boolean
	'			End Class

	'			Public Class FlightClass
	'				Public FlightClassID As Integer
	'				Public FlightClass As String
	'				Public Count As Integer
	'			End Class

	'		End Class

	'#End Region

	'#Region "Filter Results"

	'		Public Sub FilterResults(Optional ByVal DateChanged As Boolean = False)

	'			Dim oFilterItems As New FilterItems

	'			'if date has changed reset min max flight time minutes and min max price so that we do not incorrectly filter out the flights
	'			If DateChanged Then
	'				Me.Filters.MinOutboundDepartureFlightTimeMinutes = 0
	'				Me.Filters.MaxOutboundDepartureFlightTimeMinutes = 1439
	'				Me.Filters.MinReturnDepartureFlightTimeMinutes = 0
	'				Me.Filters.MaxReturnDepartureFlightTimeMinutes = 1439
	'				Me.Filters.MinPrice = 0
	'				Me.Filters.MaxPrice = 0
	'			End If

	'			'clone all flights
	'			Me.FilteredFlights = Results.CloneFlights(Me.AllFlights)

	'			'filter
	'			Me.FilterResults(Me.FilteredFlights, oFilterItems)

	'			'generate filter counts and from prices
	'			Me.GenerateFromPricesAndCounts()

	'			Me.SortResults()

	'		End Sub

	'		Public Sub FilterResults(ByVal oFlights As Generic.List(Of Flight), ByVal oFilterItems As FilterItems)

	'			Dim oFlightsToRemove As New Generic.List(Of Flight)

	'			For Each oFlight As Flight In oFlights

	'				'outbound departure date
	'				If oFlight.OutboundDepartureDate <> Me.Filters.OutboundDepartureDate Then
	'					oFlightsToRemove.Add(oFlight)
	'					Continue For
	'				End If

	'				'outbound departure times of day
	'				If oFilterItems.OutboundDepartureTimesOfDay AndAlso Me.Filters.FilterDepartureTimes.Count > 0 Then
	'					If Not Me.Filters.FilterDepartureTimes.Contains(oFlight.OutBoundDepartureTimeOfDay) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'return departure times of day
	'				If oFilterItems.ReturnDepartureTimesOfDay AndAlso Me.Filters.FilterReturnTimes.Count > 0 Then
	'					If Not Me.Filters.FilterReturnTimes.Contains(oFlight.ReturnDepartureTimeOfDay) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'  outbound departure time
	'				If oFilterItems.OutboundDepartureTime AndAlso (Me.Filters.MinOutboundDepartureFlightTimeMinutes <> 0 OrElse Me.Filters.MaxOutboundDepartureFlightTimeMinutes <> 1439) Then
	'					Dim sMinOutboundDepartureTime As String = SafeString(New TimeSpan(0, Me.Filters.MinOutboundDepartureFlightTimeMinutes, 0)).Substring(0, 5)
	'					Dim sMaxOutboundDepartureTime As String = SafeString(New TimeSpan(0, Me.Filters.MaxOutboundDepartureFlightTimeMinutes, 0)).Substring(0, 5)

	'					'.net can compare strings and knows whether the time is greater
	'					If oFlight.OutboundDepartureTime < sMinOutboundDepartureTime OrElse oFlight.OutboundDepartureTime > sMaxOutboundDepartureTime Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'return departure time
	'				If oFilterItems.ReturnDepartureTime AndAlso (Me.Filters.MinReturnDepartureFlightTimeMinutes <> 0 OrElse Me.Filters.MaxReturnDepartureFlightTimeMinutes <> 1439) Then
	'					Dim sMinReturnDepartureTime As String = SafeString(New TimeSpan(0, Me.Filters.MinReturnDepartureFlightTimeMinutes, 0)).Substring(0, 5)
	'					Dim sMaxReturnDepartureTime As String = SafeString(New TimeSpan(0, Me.Filters.MaxReturnDepartureFlightTimeMinutes, 0)).Substring(0, 5)

	'					'.net can compare strings and knows whether the time is greater
	'					If oFlight.ReturnDepartureTime < sMinReturnDepartureTime OrElse oFlight.ReturnDepartureTime > sMaxReturnDepartureTime Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'flight carriers
	'				If oFilterItems.FlightCarriers AndAlso Me.Filters.FilterFlightCarrerIDs.Count > 0 Then
	'					If Not Me.Filters.FilterFlightCarrerIDs.Contains(oFlight.FlightCarrierID) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'departure airports
	'				If oFilterItems.OutboundDepartureAirport AndAlso Me.Filters.FilterDepartureAirportIDs.Count > 0 Then
	'					If Not Me.Filters.FilterDepartureAirportIDs.Contains(oFlight.DepartureAirportID) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'arrival airports
	'				If oFilterItems.OutboundArrivalAirport AndAlso Me.Filters.FilterArrivalAirportIDs.Count > 0 Then
	'					If Not Me.Filters.FilterArrivalAirportIDs.Contains(oFlight.ArrivalAirportID) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'stops
	'				If oFilterItems.Stops AndAlso Me.Filters.FilterStops.Count > 0 Then
	'					If Not Me.Filters.FilterStops.Contains(oFlight.MaxStops) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'baggage
	'				If oFilterItems.IncludesSupplierBaggage AndAlso Me.Filters.IncludesSupplierBaggage Then
	'					If Not oFlight.IncludesSupplierBaggage Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'price
	'				If oFilterItems.MinMaxPrice AndAlso Me.Filters.MaxPrice <> 0 Then
	'					If oFlight.Total < Me.Filters.MinPrice OrElse oFlight.Total > Me.Filters.MaxPrice Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'				'flight classes
	'				'If oFilterItems.FlightClasses AndAlso Me.Filters.FilterFlightClassIDs.Count > 0 Then
	'				'	If Not Me.Filters.FilterFlightCarrerIDs.Contains(oFlight.OutboundFlightClassID) Then
	'				'		oFlightsToRemove.Add(oFlight)
	'				'		Continue For
	'				'	End If
	'				'End If

	'				'booking tokens - if it matches remove, different to the other filters
	'				If oFilterItems.BookingTokens AndAlso Me.Filters.BookingTokenCSV <> "" Then
	'					Dim aBookingTokens As String() = Me.Filters.BookingTokenCSV.Split(","c)
	'					If aBookingTokens.Contains(oFlight.BookingToken.ToString) Then
	'						oFlightsToRemove.Add(oFlight)
	'						Continue For
	'					End If
	'				End If

	'			Next

	'			'remove flights
	'			For Each oFlight As Flight In oFlightsToRemove
	'				oFlights.Remove(oFlight)
	'			Next

	'		End Sub

	'		Public Class FilterItems
	'			Public OutboundDepartureTimesOfDay As Boolean = True
	'			Public ReturnDepartureTimesOfDay As Boolean = True
	'			Public FlightCarriers As Boolean = True
	'			Public OutboundDepartureAirport As Boolean = True
	'			Public OutboundArrivalAirport As Boolean = True
	'			Public OutboundDepartureTime As Boolean = True
	'			Public ReturnDepartureTime As Boolean = True
	'			Public MinMaxPrice As Boolean = True
	'			Public IncludesSupplierBaggage As Boolean = True
	'			Public Stops As Boolean = True
	'			Public FlightClasses As Boolean = True
	'			'in case you need to exclude certain flights from the list eg. already in the basket
	'			Public BookingTokens As Boolean = True
	'		End Class

	'#End Region

	'#Region "Sort Results"

	'		Public Sub SortResults()

	'			Select Case Me.Filters.SortBy
	'				Case eSortBy.Price
	'					Me.FilteredFlights.Sort(New SortByPrice)
	'				Case eSortBy.DepartureTime
	'					Me.FilteredFlights.Sort(New SortByOutboundDepartureTime)
	'				Case eSortBy.ReturnTime
	'					Me.FilteredFlights.Sort(New SortByReturnTime)
	'				Case eSortBy.Airline
	'					Me.FilteredFlights.Sort(New SortByAirline)
	'			End Select

	'		End Sub

	'#End Region

	'#Region "Generate Filter From Prices and Counts"

	'		Public Sub GenerateFromPricesAndCounts()

	'			'1. reset counts
	'			Me.Filters.DepartureTimes.Clear()
	'			Me.Filters.ReturnTimes.Clear()
	'			Me.Filters.FlightCarriers.Clear()
	'			Me.Filters.Stops.Clear()
	'			Me.Filters.FlightClasses.Clear()
	'			Me.Filters.DepartureAirports.Clear()
	'			Me.Filters.ArrivalAirports.Clear()

	'			'2. Outbound departure times

	'			'2.1 Filter flight results on everything except the outbound flight times
	'			Dim oOutboundTimesFilterItems As New FilterItems
	'			oOutboundTimesFilterItems.OutboundDepartureTimesOfDay = False

	'			Dim oOutboundTimeFlights As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oOutboundTimeFlights, oOutboundTimesFilterItems)

	'			'2.2 Calculate outbound departure time
	'			Dim aDepartureTimes As IEnumerable(Of eTimeOfDay) = (From oFlight In oOutboundTimeFlights Select oFlight.OutBoundDepartureTimeOfDay).Distinct

	'			For Each TimeOfDay As eTimeOfDay In aDepartureTimes
	'				Dim oFlightTime As New Filter.FlightTime
	'				With oFlightTime
	'					.TimeOfDay = TimeOfDay
	'					.Count = oOutboundTimeFlights.Where(Function(oFlight) oFlight.OutBoundDepartureTimeOfDay = oFlightTime.TimeOfDay).Count()
	'					.FromPrice = oOutboundTimeFlights.Where(Function(oFlight) oFlight.OutBoundDepartureTimeOfDay = oFlightTime.TimeOfDay).Min(Function(oFlight) oFlight.Total)
	'				End With
	'				Me.Filters.DepartureTimes.Add(oFlightTime)
	'			Next

	'			'3. Return departure times

	'			'3.1 Filter flight results on everything except the return flight times
	'			Dim oReturnTimesFilterItems As New FilterItems
	'			oReturnTimesFilterItems.ReturnDepartureTimesOfDay = False

	'			Dim oReturnTimeFlights As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oReturnTimeFlights, oReturnTimesFilterItems)

	'			'3.1 Calculate return departure time
	'			Dim aReturnTimes As IEnumerable(Of eTimeOfDay) = (From oFlight In oReturnTimeFlights Select oFlight.ReturnDepartureTimeOfDay).Distinct

	'			For Each TimeOfDay As eTimeOfDay In aReturnTimes
	'				Dim oFlightTime As New Filter.FlightTime
	'				With oFlightTime
	'					.TimeOfDay = TimeOfDay
	'					.Count = oReturnTimeFlights.Where(Function(oFlight) oFlight.ReturnDepartureTimeOfDay = oFlightTime.TimeOfDay).Count()
	'					.FromPrice = oReturnTimeFlights.Where(Function(oFlight) oFlight.ReturnDepartureTimeOfDay = oFlightTime.TimeOfDay).Min(Function(oFlight) oFlight.Total)
	'				End With
	'				Me.Filters.ReturnTimes.Add(oFlightTime)
	'			Next

	'			'4. flight carriers

	'			'4.1 filter results on everything except flight carriers
	'			Dim oFlightCarrierFilterItems As New FilterItems
	'			oFlightCarrierFilterItems.FlightCarriers = False

	'			Dim oFlightCarrierFlights As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oFlightCarrierFlights, oFlightCarrierFilterItems)

	'			Dim aFlightCarriers As IEnumerable(Of Integer) = (From oFlight In oFlightCarrierFlights Select oFlight.FlightCarrierID).Distinct

	'			For Each iFlightCarrier As Integer In aFlightCarriers
	'				Dim oFilterFlightCarrier As New Filter.FlightCarrier
	'				With oFilterFlightCarrier
	'					.FlightCarrierID = iFlightCarrier
	'					.FlightCarrier = oFlightCarrierFlights.Where(Function(oFlight) oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID).First.FlightCarrier
	'					.FlightCarrierLogo = oFlightCarrierFlights.Where(Function(oFlight) oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID).First.FlightCarrierLogo
	'					.Count = oFlightCarrierFlights.Where(Function(oFlight) oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID).Count()
	'					.FromPrice = oFlightCarrierFlights.Where(Function(oFlight) oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID).Min(Function(oFlight) oFlight.Total)
	'					.Selected = Me.Filters.FilterFlightCarrerIDs.Contains(iFlightCarrier)
	'				End With
	'				Me.Filters.FlightCarriers.Add(oFilterFlightCarrier)
	'			Next

	'			'5. stops (maximum from either leg)
	'			Dim oFlightStopFilterItems As New FilterItems
	'			oFlightStopFilterItems.Stops = False

	'			Dim oFlightStopFlights As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oFlightStopFlights, oFlightStopFilterItems)

	'			Dim aFlightStops As IEnumerable(Of Integer) = (From oFlight In oFlightStopFlights Select oFlight.MaxStops).Distinct

	'			For Each iStop As Integer In aFlightStops
	'				Dim oFlightStop As New Filter.FlightStop
	'				With oFlightStop
	'					.Stops = iStop
	'					.Count = oFlightStopFlights.Where(Function(oFlight) oFlight.MaxStops = oFlightStop.Stops).Count()
	'					.FromPrice = oFlightStopFlights.Where(Function(oFlight) oFlight.MaxStops = oFlightStop.Stops).Min(Function(oFlight) oFlight.Total)
	'					.Selected = Me.Filters.FilterStops.Contains(iStop)
	'				End With
	'				Me.Filters.Stops.Add(oFlightStop)
	'			Next

	'			'6. Min/Max Price
	'			Me.Filters.MinPrice = 0
	'			Me.Filters.MaxPrice = 0

	'			If Me.FilteredFlights.Count > 0 Then
	'				Me.Filters.MinPrice = Me.FilteredFlights.Min(Function(oFlight) oFlight.Total)
	'				Me.Filters.MaxPrice = Me.FilteredFlights.Max(Function(oFlight) oFlight.Total)
	'			End If

	'			'7 min max outbound times
	'			'reset these, so we only use if not equal to 0
	'			Me.Filters.MinOutboundDepartureFlightTime = ""
	'			Me.Filters.MaxOutboundDepartureFlightTime = ""
	'			Me.Filters.MinOutboundDepartureFlightTimeMinutes = 0
	'			Me.Filters.MaxOutboundDepartureFlightTimeMinutes = 0

	'			If Me.FilteredFlights.Count > 0 Then
	'				Me.Filters.MinOutboundDepartureFlightTime = Me.FilteredFlights.Min(Function(o) o.OutboundDepartureTime)
	'				Me.Filters.MaxOutboundDepartureFlightTime = Me.FilteredFlights.Max(Function(o) o.OutboundDepartureTime)
	'				Me.Filters.MinOutboundDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.Filters.MinOutboundDepartureFlightTime).TotalMinutes)
	'				Me.Filters.MaxOutboundDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.Filters.MaxOutboundDepartureFlightTime).TotalMinutes)
	'			End If

	'			'8 min max return times reset these, so we only use if not equal to 0
	'			Me.Filters.MinReturnDepartureFlightTime = ""
	'			Me.Filters.MaxReturnDepartureFlightTime = ""
	'			Me.Filters.MinReturnDepartureFlightTimeMinutes = 0
	'			Me.Filters.MaxReturnDepartureFlightTimeMinutes = 0

	'			If Me.FilteredFlights.Count > 0 Then
	'				Me.Filters.MinReturnDepartureFlightTime = Me.FilteredFlights.Min(Function(o) o.ReturnDepartureTime)
	'				Me.Filters.MaxReturnDepartureFlightTime = Me.FilteredFlights.Max(Function(o) o.ReturnDepartureTime)
	'				Me.Filters.MinReturnDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.Filters.MinReturnDepartureFlightTime).TotalMinutes)
	'				Me.Filters.MaxReturnDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.Filters.MaxReturnDepartureFlightTime).TotalMinutes)
	'			End If

	'			'9 departure and arrival airports

	'			'9.1 Departure airports
	'			Dim oOutboundDepartureAirportsFilterItems As New FilterItems
	'			oOutboundDepartureAirportsFilterItems.OutboundDepartureAirport = False

	'			Dim oOutboundDepartureAirports As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oOutboundDepartureAirports, oOutboundDepartureAirportsFilterItems)

	'			Dim aDepartureAirports As IEnumerable(Of Integer) = (From oFlight In oOutboundDepartureAirports Select oFlight.DepartureAirportID).Distinct

	'			For Each iDepartureAirport As Integer In aDepartureAirports
	'				Dim oFilterDepartureAirport As New Filter.Airport
	'				With oFilterDepartureAirport
	'					.AirportID = iDepartureAirport
	'					.AirportCode = oOutboundDepartureAirports.Where(Function(oFlight) oFlight.DepartureAirportID = oFilterDepartureAirport.AirportID).First.DepartureAirportCode
	'					.AirportName = oOutboundDepartureAirports.Where(Function(oFlight) oFlight.DepartureAirportID = oFilterDepartureAirport.AirportID).First.DepartureAirport
	'					.Count = oOutboundDepartureAirports.Where(Function(oFlight) oFlight.DepartureAirportID = oFilterDepartureAirport.AirportID).Count
	'					.Selected = Me.Filters.FilterDepartureAirportIDs.Contains(iDepartureAirport)
	'				End With
	'				Me.Filters.DepartureAirports.Add(oFilterDepartureAirport)
	'			Next

	'			'9.2 Arrival airports
	'			Dim oOutboundArrivalAirportsFilterItems As New FilterItems
	'			oOutboundArrivalAirportsFilterItems.OutboundDepartureAirport = False

	'			Dim oOutboundArrivalAirports As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oOutboundArrivalAirports, oOutboundArrivalAirportsFilterItems)

	'			Dim aArrivalAirports As IEnumerable(Of Integer) = (From oFlight In oOutboundArrivalAirports Select oFlight.ArrivalAirportID).Distinct

	'			For Each iArrivalAirport As Integer In aArrivalAirports
	'				Dim oFilterArrivalAirport As New Filter.Airport
	'				With oFilterArrivalAirport
	'					.AirportID = iArrivalAirport
	'					.AirportCode = oOutboundArrivalAirports.Where(Function(oFlight) oFlight.ArrivalAirportID = oFilterArrivalAirport.AirportID).First.ArrivalAirportCode
	'					.AirportName = oOutboundArrivalAirports.Where(Function(oFlight) oFlight.ArrivalAirportID = oFilterArrivalAirport.AirportID).First.ArrivalAirport
	'					.Count = oOutboundArrivalAirports.Where(Function(oFlight) oFlight.ArrivalAirportID = oFilterArrivalAirport.AirportID).Count
	'					.Selected = Me.Filters.FilterArrivalAirportIDs.Contains(iArrivalAirport)
	'				End With
	'				Me.Filters.ArrivalAirports.Add(oFilterArrivalAirport)
	'			Next

	'			'10. Flight Classes
	'			Dim oFlightClassFilterItems As New FilterItems
	'			oFlightClassFilterItems.FlightClasses = False

	'			Dim oFlightClassFlights As Generic.List(Of Flight) = Results.CloneFlights(Me.AllFlights)
	'			Me.FilterResults(oFlightClassFlights, oFlightClassFilterItems)

	'			Dim aFlightClasses As IEnumerable(Of Integer) = (From oFlight In oFlightClassFlights Select oFlight.OutboundFlightClassID).Distinct

	'			For Each iFlightClass As Integer In aFlightClasses
	'				Dim oFilterFlightClass As New Filter.FlightClass
	'				With oFilterFlightClass
	'					.FlightClassID = iFlightClass
	'					.FlightClass = oFlightClassFlights.Where(Function(oFlight) oFlight.OutboundFlightClassID = oFilterFlightClass.FlightClassID).First.OutboundFlightClass
	'					.Count = oFlightClassFlights.Where(Function(oFlight) oFlight.OutboundFlightClassID = oFilterFlightClass.FlightClassID).Count()
	'				End With
	'				Me.Filters.FlightClasses.Add(oFilterFlightClass)
	'			Next

	'		End Sub

	'#End Region

	'#Region "Support classes and enums"

	'		Public Shared Function FlightTimeOfDay(ByVal FlightTime As String) As eTimeOfDay

	'			If DateFunctions.InTimeRange(FlightTime, "06:00", "11:59") Then
	'				Return eTimeOfDay.Morning
	'			ElseIf DateFunctions.InTimeRange(FlightTime, "12:00", "17:59") Then
	'				Return eTimeOfDay.Afternoon
	'			ElseIf DateFunctions.InTimeRange(FlightTime, "18:00", "21:59") Then
	'				Return eTimeOfDay.Evening
	'			Else
	'				Return eTimeOfDay.Night
	'			End If

	'		End Function

	'		Public Shared Function FromPrice(ByVal DepartureDate As Date) As Decimal

	'			'Get the specified dates results
	'			Dim oFlightResults As New Results
	'			oFlightResults.Flights.AddRange(BookingBase.SearchDetails.FlightResults.Flights.Where(Function(oFlight) oFlight.OutboundDepartureDate = DepartureDate))

	'			Dim nFromPrice As Decimal = 0

	'			'If there are results then sort by price and get the price of the cheapest one
	'			If oFlightResults.Flights.Count > 0 Then

	'				oFlightResults.Flights.Sort(New SortByPrice)

	'				nFromPrice = oFlightResults.Flights.First.Total

	'			End If

	'			Return nFromPrice

	'		End Function

	'		Public Class SortByPrice
	'			Implements IComparer(Of Flight)

	'			Overloads Function Compare(ByVal x As Flight, ByVal y As Flight) As Integer Implements IComparer(Of Flight).Compare

	'				Dim iBase As Integer = Functions.IIf(BookingBase.SearchDetails.FlightResults.Filters.SortOrder = eSortOrder.Ascending, 1, -1)

	'				If x.Total > y.Total Then
	'					Return iBase
	'				ElseIf x.Total < y.Total Then
	'					Return -1 * iBase
	'				Else
	'					Return 0
	'				End If

	'			End Function

	'		End Class

	'		Public Class SortByOutboundDepartureTime
	'			Implements IComparer(Of Flight)

	'			Overloads Function Compare(ByVal x As Flight, ByVal y As Flight) As Integer Implements IComparer(Of Flight).Compare

	'				Dim iBase As Integer = Functions.IIf(BookingBase.SearchDetails.FlightResults.Filters.SortOrder = eSortOrder.Ascending, 1, -1)

	'				If x.OutboundDepartureTime > y.OutboundDepartureTime Then
	'					Return iBase
	'				ElseIf x.OutboundDepartureTime < y.OutboundDepartureTime Then
	'					Return -1 * iBase
	'				Else
	'					Return 0
	'				End If

	'			End Function

	'		End Class
	'		Public Class SortByReturnTime
	'			Implements IComparer(Of Flight)

	'			Overloads Function Compare(ByVal x As Flight, ByVal y As Flight) As Integer Implements IComparer(Of Flight).Compare

	'				Dim iBase As Integer = Functions.IIf(BookingBase.SearchDetails.FlightResults.Filters.SortOrder = eSortOrder.Ascending, 1, -1)

	'				If x.ReturnDepartureTime > y.ReturnDepartureTime Then
	'					Return iBase
	'				ElseIf x.ReturnDepartureTime < y.ReturnDepartureTime Then
	'					Return -1 * iBase
	'				Else
	'					Return 0
	'				End If

	'			End Function

	'		End Class
	'		Public Class SortByAirline
	'			Implements IComparer(Of Flight)

	'			Overloads Function Compare(ByVal x As Flight, ByVal y As Flight) As Integer Implements IComparer(Of Flight).Compare

	'				Dim iBase As Integer = Functions.IIf(BookingBase.SearchDetails.FlightResults.Filters.SortOrder = eSortOrder.Ascending, 1, -1)

	'				If x.FlightCarrier > y.FlightCarrier Then
	'					Return iBase
	'				ElseIf x.FlightCarrier < y.FlightCarrier Then
	'					Return -1 * iBase
	'				Else
	'					Return 0
	'				End If

	'			End Function

	'		End Class

	'		Public Class BaggageOptions

	'			'Filters
	'			Public BaggageOptions As New Generic.List(Of BaggageOption)

	'			Public Class BaggageOption

	'				'Filters
	'				Public Token As String
	'				Public Quantity As Integer

	'			End Class

	'		End Class

	'		Public Class ExtraOptions

	'			'Filters
	'			Public ExtraOptions As New Generic.List(Of ExtraOption)

	'			Public Class ExtraOption

	'				'Filters
	'				Public Token As String
	'				Public Quantity As Integer

	'			End Class

	'		End Class

	'#Region "Support - Enums"

	'		Public Enum eSortBy
	'			Price
	'			DepartureTime
	'			Airline
	'			ReturnTime
	'		End Enum

	'		Public Enum eSortOrder
	'			Ascending
	'			Descending
	'		End Enum

	'		Enum eTimeOfDay
	'			Morning
	'			Afternoon
	'			Evening
	'			Night
	'		End Enum

	'#End Region

	'#End Region

	'#Region "Remove Markup"

	'		Public Sub RemoveMarkup()

	'			For Each oFlight As Flight In Me.AllFlights

	'				oFlight.Total -= oFlight.MarkupAmount
	'				oFlight.Total /= (oFlight.MarkupPercentage / 100) + 1

	'				oFlight.MarkupAmount = 0
	'				oFlight.MarkupPercentage = 0

	'			Next

	'		End Sub

	'#End Region

	'#Region "Clone"

	'		Public Shared Function CloneFlights(oFlights As Generic.List(Of Flight)) As Generic.List(Of Flight)

	'			Dim oReturn As New Generic.List(Of Flight)

	'			For Each oFlight As Flight In oFlights
	'				Dim oFlightNew As Flight = BookingFlight.Results.CloneFlight(oFlight)

	'				oReturn.Add(oFlightNew)
	'			Next

	'			Return oReturn

	'		End Function

	'		Public Shared Function CloneFlight(ByVal Flight As Flight) As Flight

	'			Dim oReturn As New Flight

	'			Dim oFlightXML As XmlDocument = Serializer.Serialize(Flight, True)
	'			oReturn = CType(Serializer.DeSerialize(Flight.GetType, oFlightXML.InnerXml), Flight)

	'			Return oReturn

	'		End Function

	'		Public Shared Function CloneFilter(ByVal Filter As Filter) As Filter

	'			Dim oReturn As New Filter

	'			Dim oFilterXML As XmlDocument = Serializer.Serialize(Filter, True)
	'			oReturn = CType(Serializer.DeSerialize(Filter.GetType, oFilterXML.InnerXml), Filter)

	'			Return oReturn

	'		End Function

	'#End Region

	'	End Class

#End Region

#Region "Basket"

#Region "add flight"

	' add flight option (hash token)
	Public Shared Sub AddFlightToBasket(ByVal HashToken As String, Optional ByVal ClearBasket As Boolean = True)

		'create flight option from hash token
		Dim oFlightOption As BasketFlight.FlightOption = BasketFlight.FlightOption.DeHashToken(Of BasketFlight.FlightOption)(HashToken)

		'add flight option to basket
		BookingFlight.AddFlightToBasket(oFlightOption, ClearBasket)

	End Sub

	Public Shared Sub AddFlightToBasket(ByVal FlightOption As BasketFlight.FlightOption, Optional ByVal ClearBasket As Boolean = True)
		AddFlightToBasket(FlightOption, ClearBasket, True)
	End Sub

	'add flight option (flight option)
	Public Shared Sub AddFlightToBasket(ByVal FlightOption As BasketFlight.FlightOption, ClearBasket As Boolean, SetupExtras As Boolean)

		'clear current basket flight
		If ClearBasket Then BookingBase.SearchBasket.BasketFlights.Clear()

		'create basket flight
		Dim oBasketFlight As New BasketFlight
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketFlight.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1
		oBasketFlight.Flight = FlightOption

		'save result booking token so we can find result content later
		oBasketFlight.ResultBookingToken = FlightOption.BookingToken
        oBasketFlight.ReturnMultiCarrierResultBookingToken = FlightOption.ReturnMultiCarrierDetails.BookingToken

		'setup extras - baggage and seat maps
		If SetupExtras Then
			oBasketFlight.SetupExtras()
		End If

		'set content xml
		Dim oFlight As FlightResultHandler.Flight = BookingBase.SearchDetails.FlightResults.GetSingleFlight(oBasketFlight.ResultBookingToken, _
            oBasketFlight.ReturnMultiCarrierResultBookingToken)
  
		If Not oFlight Is Nothing Then
			oBasketFlight.ContentXML = Serializer.Serialize(oFlight, True)
		    oBasketFlight.FlightSectors.AddRange(oFlight.FlightSectors)
		End If

		'add to basket
		BookingBase.SearchBasket.BasketFlights.Add(oBasketFlight)

		'if flight and hotel update selected flight
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
			BookingBase.SearchDetails.PropertyResults.SetupSelectedFlight()
		End If

	End Sub

	Public Shared Sub AddFlightToBasket(ByVal sBookingReference As String, ByVal iFlightBookingID As Integer, ByVal nOriginalPrice As Decimal,
	  ByVal oBasketFlightExtras As Generic.List(Of BasketFlight.BasketFlightExtra))

		'clear the main basket
		BookingBase.Basket = New BookingBasket(True)

		'set the booking reference
		BookingBase.Basket.BookingReference = sBookingReference

		'create basket flight and set the flight booking id
		Dim oBasketFlight As New BookingFlight.BasketFlight
		oBasketFlight.FlightBookingID = iFlightBookingID

		'set component id
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketFlight.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1

		'set the extras
		oBasketFlight.BasketFlightExtras = oBasketFlightExtras

		'sum up the price of the extras in the basket
		Dim nCurrentPriceOfExtras As Decimal = oBasketFlight.BasketFlightExtras.Sum(Function(oExtra) oExtra.Price)
		'get the price difference between the amount paid for extras and the new price of extras
		Dim nPaymentDue As Decimal = nCurrentPriceOfExtras - nOriginalPrice
		If nPaymentDue < 0 Then nPaymentDue = 0

		'we don't have a token so we need to setup the flight option
		oBasketFlight.Flight = New BookingFlight.BasketFlight.FlightOption
		'set the seat map cost that will contribute to the total price on the basket
		oBasketFlight.Flight.SeatMapCost = nPaymentDue

		'add to main basket
		BookingBase.Basket.BasketFlights.Add(oBasketFlight)

		'set the amount to pay on the basket
		BookingBase.Basket.AmountDueToday = nPaymentDue

	End Sub

#End Region

#Region "Support Classes - BasketFlight"

	Public Class BasketFlight
		Public ComponentID As Integer

		Public Flight As New FlightOption
		Public BasketFlightExtras As New Generic.List(Of BasketFlightExtra)
		Public ReturnMultiCarrierFlightExtras As New Generic.List(Of BasketFlightExtra)
		Public ContentXML As XmlDocument
		Public ResultBookingToken As String
		Public ReturnMultiCarrierResultBookingToken As String
		Public GuestIDs As New Generic.List(Of Integer)
		Public FlightBookingID As Integer ' we will need this for booking extras through MMB
		Public TermsAndConditions As String
		Public TermsAndConditionsURL As String
		Public ReturnMultiCarrierTermsAndConditions As String
		Public ReturnMultiCarrierTermsAndConditionsURL As String
        Public FlightSectors As New List(Of FlightResultHandler.FlightSector)
		Public Property Markup As Decimal
			Get
				Dim nTotalMarkup As Decimal = 0

				For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Flight)
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							nTotalMarkup += oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							nTotalMarkup += oMarkup.Value * (Me.Flight.Adults + Me.Flight.Children)
						Case BookingBase.Markup.eType.Percentage
							nTotalMarkup += (oMarkup.Value * Me.Flight.Price) / 100
					End Select
				Next

				Return nTotalMarkup
			End Get
			Set(value As Decimal)
				'require this to be serialised
			End Set
		End Property

        public Property CurrentCenterLastArrivalSector As FlightResultHandler.FlightSector
            get

                Dim currentCenter As Integer = BookingBase.SearchDetails.ItineraryDetails.CurrentCenter

                Dim remainingSectors As new List(Of FlightResultHandler.FlightSector) 

                If me.FlightSectors IsNot Nothing
                    remainingSectors.AddRange(me.FlightSectors)
                    remainingSectors = remainingSectors.OrderBy(Function(fs) fs.DepartureDate).ThenBy(Function(fs)fs.DepartureTime).ToList()
                End If

                Dim selectedCenter As Integer = 1 
                Dim selectedSector As new FlightResultHandler.FlightSector

                'Loop through the sectors unless we've gone past the sector we care about or would throw an exception
                While(remainingSectors.Any() AndAlso selectedCenter <= currentCenter)

                    'We want to select and pop off the first sector on the list
                    selectedSector = remainingSectors.Take(1).FirstOrDefault()
                    remainingSectors = remainingSectors.Skip(1).ToList()

                    'If there are still sectors remaining in the list, and the seq of the next sector is not greater than the seq of the current center
                    'That means that the next loop round we will be in the next sector
                    If(remainingSectors.Any() AndAlso selectedSector.Seq >= remainingSectors(0).Seq)
                        selectedCenter += 1
                    End If
                End While

                Return selectedSector

            End Get
            Set(value As FlightResultHandler.FlightSector)
                'require this to be serialised
            End Set
        end Property

		Public Property NextCenterFirstDepartureSector As FlightResultHandler.FlightSector 
		    Get
		        Dim currentCenter As Integer = BookingBase.SearchDetails.ItineraryDetails.CurrentCenter

		        Dim firstDepartureSector As FlightResultHandler.FlightSector = me.FlightSectors.Where(Function(o) o.Seq = 1).OrderBy(function(o) o.DepartureDate) _
		                .ThenBy(Function(o) o.DepartureTime).Skip(currentCenter).FirstOrDefault()
                        
		        Return firstDepartureSector
		    End Get
		    Set(value As FlightResultHandler.FlightSector)
		        'require this to be serialised
		    End Set
		End Property


		Public Class FlightOption
			Inherits Utility.Hasher

			Public ReturnMultiCarrierDetails As New MultiCarrierDetails
			Public BookingToken As String
			Public hlpSearchBookingToken As String
			Public Price As Decimal
			Public OutboundPrice As Decimal
			Public TotalCommission As Decimal

			Public SupplierID As Integer
			Public LocalCost As Decimal
			Public CurrencyID As Integer

			Public SeatMapCost As Decimal

			Public OutboundFlightCode As String
			Public ReturnFlightCode As String

			Public OutboundDepartureDate As Date
			Public OutboundDepartureTime As String
			Public OutboundArrivalDate As Date
			Public OutboundArrivalTime As String

			Public ReturnDepartureDate As Date
			Public ReturnDepartureTime As String
			Public ReturnArrivalDate As Date
			Public ReturnArrivalTime As String

			Public DepartureAirportID As Integer
			Public ArrivalAirportID As Integer

			Public FlightCarrierID As Integer
			Public FlightCarrier As String

			Public FlightCarrierType As String

			Public Adults As Integer
			Public Children As Integer
			Public Infants As Integer
			Public FlightPlusHotel As Boolean

			Public IncludedBaggageAllowance As Integer
			Public IncludedBaggageText As String

		End Class

		Public Class MultiCarrierDetails
			Public BookingToken As String
			Public Price As Decimal
			Public TotalCommission As Decimal
			Public SeatMapCost As Decimal
			Public FlightBookingID As Integer
			Public hlpSearchBookingToken As String
			Public SupplierID As Integer
			Public LocalCost As Decimal
		End Class

        Public Function CurrentCentersArrivalAndDepartureMatch() As Boolean

            dim bSectorsMatch as Boolean = false

            dim currentArrivalSector As  FlightResultHandler.FlightSector = Me.CurrentCenterLastArrivalSector
            dim nextDepartureSector As  FlightResultHandler.FlightSector = Me.NextCenterFirstDepartureSector

            If currentArrivalSector IsNot Nothing AndAlso nextDepartureSector IsNot Nothing Then
                bSectorsMatch = currentArrivalSector.ArrivalAirportID = nextDepartureSector.DepartureAirportID
            End If

            return bSectorsMatch

        End Function

		Public Function CreatePreBookRequest(Optional ByVal BaggageRequest As Boolean = False,
		   Optional ByVal SeatMapsRequest As Boolean = False) As Generic.List(Of ivci.Flight.PreBookRequest)

			Dim oPreBookRequests As New Generic.List(Of ivci.Flight.PreBookRequest)
			Dim bIsMultiCarrier As Boolean = False

			If Me.Flight.ReturnMultiCarrierDetails.BookingToken <> "" Then
				bIsMultiCarrier = True
			End If

			Dim oPreBookRequest As New ivci.Flight.PreBookRequest
			With oPreBookRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingToken = Me.Flight.BookingToken
				.FlightAndHotel = Me.Flight.FlightPlusHotel
				If bIsMultiCarrier Then .MultiCarrierOutbound = True
				For Each oBasketFlightExtra As BookingFlight.BasketFlight.BasketFlightExtra In Me.BasketFlightExtras.Where(Function(oFlightExtra) oFlightExtra.QuantitySelected > 0)
					Dim oExtra As New ivci.Flight.PreBookRequest.Extra
					oExtra.ExtraBookingToken = oBasketFlightExtra.ExtraBookingToken
					oExtra.Quantity = oBasketFlightExtra.QuantitySelected
					oExtra.GuestID = oBasketFlightExtra.GuestID
					If oBasketFlightExtra.ExtraType = "Seat" Then
						oExtra.RequestedExtraType = "Seat"
					Else
						oExtra.RequestedExtraType = ""
					End If
					.Extras.Add(oExtra)
				Next
			End With

			oPreBookRequests.Add(oPreBookRequest)

			If bIsMultiCarrier Then
				Dim oReturnPreBookRequest As New ivci.Flight.PreBookRequest
				With oReturnPreBookRequest
					.LoginDetails = BookingBase.IVCLoginDetails
					.BookingToken = Me.Flight.ReturnMultiCarrierDetails.BookingToken
					.FlightAndHotel = Me.Flight.FlightPlusHotel
					.MultiCarrierReturn = True
					For Each oBasketFlightExtra As BookingFlight.BasketFlight.BasketFlightExtra In Me.ReturnMultiCarrierFlightExtras.Where(Function(oFlightExtra) oFlightExtra.QuantitySelected > 0)
						Dim oExtra As New ivci.Flight.PreBookRequest.Extra
						oExtra.ExtraBookingToken = oBasketFlightExtra.ExtraBookingToken
						oExtra.Quantity = oBasketFlightExtra.QuantitySelected
						oExtra.GuestID = oBasketFlightExtra.GuestID
						If oBasketFlightExtra.ExtraType = "Seat" Then
							oExtra.RequestedExtraType = "Seat"
						Else
							oExtra.RequestedExtraType = ""
						End If
						.Extras.Add(oExtra)
					Next
				End With
				oPreBookRequests.Add(oReturnPreBookRequest)
			End If

			Return oPreBookRequests

		End Function


		Public Function CreateBookRequest() As Generic.List(Of ivci.Flight.BookRequest)

			Dim oBookRequests As New Generic.List(Of ivci.Flight.BookRequest)
			Dim bIsMultiCarrier As Boolean = False

			If Me.Flight.ReturnMultiCarrierDetails.BookingToken <> "" Then
				bIsMultiCarrier = True
			End If

			Dim oBookRequest As New ivci.Flight.BookRequest
			With oBookRequest
				.BookingToken = Me.Flight.BookingToken
				.GuestIDs.AddRange(Me.GuestIDs)
				If bIsMultiCarrier Then
					.ExpectedTotal = Me.Flight.OutboundPrice
					.MultiCarrierOutbound = True
				Else
					.ExpectedTotal = Me.Flight.Price
				End If
			End With

			oBookRequests.Add(oBookRequest)

			If bIsMultiCarrier Then
				Dim oReturnBookRequest As New ivci.Flight.BookRequest
				With oReturnBookRequest
					.BookingToken = Me.Flight.ReturnMultiCarrierDetails.BookingToken
					.ExpectedTotal = Me.Flight.ReturnMultiCarrierDetails.Price
					.GuestIDs.AddRange(Me.GuestIDs)
					.MultiCarrierReturn = True
				End With
				oBookRequests.Add(oReturnBookRequest)
			End If

			Return oBookRequests

		End Function

#Region "Extras"

		Public Class BasketFlightExtra
			Public ExtraBookingToken As String
			Public ExtraType As String
			Public Description As String
			Public CostingBasis As String
			Public DefaultBaggage As String
			Public Price As Decimal
			Public QuantityAvailable As Integer
			Public QuantitySelected As Integer
			Public GuestID As Integer

			Public Sub UpdateExtraDetails(ByVal oExtra As iVectorConnectInterface.Flight.PreBookResponse.Extra)

				Me.ExtraBookingToken = oExtra.ExtraBookingToken
				Me.ExtraType = oExtra.ExtraType
				Me.CostingBasis = oExtra.CostingBasis
				Me.Description = oExtra.Description
				Me.DefaultBaggage = oExtra.DefaultBaggage.ToString.ToLower
				Me.Price = SafeDecimal(oExtra.Price)
				Me.QuantityAvailable = oExtra.QuantityAvailable
				Me.QuantitySelected = oExtra.QuantitySelected
				Me.GuestID = oExtra.GuestID

			End Sub

		End Class

		Public Sub SetupExtras()

			Dim oBasketPreBookRequest As New iVectorConnectInterface.Basket.PreBookRequest

			'Add the login details 
			oBasketPreBookRequest.LoginDetails = BookingBase.IVCLoginDetails

			oBasketPreBookRequest.FlightBookings.AddRange(Me.CreatePreBookRequest(True, True))

			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.Basket.PreBookResponse)(oBasketPreBookRequest)

			Dim oPreBookResponse As New iVectorConnectInterface.Basket.PreBookResponse

			If oIVCReturn.Success Then

				oPreBookResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.Basket.PreBookResponse)

				'save booking token
				Me.Flight.BookingToken = oPreBookResponse.FlightBookings(0).BookingToken

				'update the price
				Me.Flight.Price = oPreBookResponse.FlightBookings(0).TotalPrice
				Me.Flight.OutboundPrice = oPreBookResponse.FlightBookings(0).TotalPrice

				'update the total commission
				Me.Flight.TotalCommission = oPreBookResponse.FlightBookings(0).TotalCommission

				'update included baggage
				Me.Flight.IncludedBaggageAllowance = oPreBookResponse.FlightBookings(0).IncludedBaggageAllowance
				Me.Flight.IncludedBaggageText = oPreBookResponse.FlightBookings(0).IncludedBaggageText

				'update flight extras
				For Each oFlightExtra As ivci.Flight.PreBookResponse.Extra In oPreBookResponse.FlightBookings(0).Extras
					Dim sExtraBookingToken As String = oFlightExtra.ExtraBookingToken
					Dim oExistingExtra As BasketFlightExtra = Me.BasketFlightExtras.FirstOrDefault(Function(oExtra) oExtra.ExtraBookingToken = sExtraBookingToken)
					If oExistingExtra Is Nothing Then
						Dim oBasketFlightExtra As New BasketFlightExtra
						oBasketFlightExtra.UpdateExtraDetails(oFlightExtra)
						Me.BasketFlightExtras.Add(oBasketFlightExtra)
					Else
						oExistingExtra.UpdateExtraDetails(oFlightExtra)
					End If
				Next

				If oPreBookResponse.FlightBookings.Count > 1 Then

					'save booking token
					Me.Flight.ReturnMultiCarrierDetails.BookingToken = oPreBookResponse.FlightBookings(1).BookingToken

					Me.Flight.Price += oPreBookResponse.FlightBookings(1).TotalPrice
					'update the price
					Me.Flight.ReturnMultiCarrierDetails.Price = oPreBookResponse.FlightBookings(1).TotalPrice

					'update the total commission
					Me.Flight.ReturnMultiCarrierDetails.TotalCommission = oPreBookResponse.FlightBookings(1).TotalCommission

					'update flight extras
					For Each oFlightExtra As ivci.Flight.PreBookResponse.Extra In oPreBookResponse.FlightBookings(1).Extras
						Dim sExtraBookingToken As String = oFlightExtra.ExtraBookingToken
						Dim oExistingExtra As BasketFlightExtra = Me.ReturnMultiCarrierFlightExtras.FirstOrDefault(Function(oExtra) oExtra.ExtraBookingToken = sExtraBookingToken)
						If oExistingExtra Is Nothing Then
							Dim oBasketFlightExtra As New BasketFlightExtra
							oBasketFlightExtra.UpdateExtraDetails(oFlightExtra)
							Me.ReturnMultiCarrierFlightExtras.Add(oBasketFlightExtra)
						Else
							oExistingExtra.UpdateExtraDetails(oFlightExtra)
						End If
					Next

				End If

			End If

			If BookingBase.LogAllXML Then
				Dim oRequestInfo As New BookingSearch.RequestInfo
				With oRequestInfo
					.RequestTime = oIVCReturn.RequestTime
					.RequestXML = oIVCReturn.RequestXML
					.ResponseXML = oIVCReturn.ResponseXML
					.NetworkLatency = oIVCReturn.NetworkLatency
					.Type = BookingSearch.RequestInfoType.FlightExtraSearch
				End With

				WebSupportToolbar.AddUniqueLog(oRequestInfo)
			End If

		End Sub

		Public Sub UpdateBaggageQuantity(ByVal oBaggageTokens As FlightResultHandler.BaggageOptions)

			For Each oOption As FlightResultHandler.BaggageOptions.BaggageOption In oBaggageTokens.BaggageOptions

				For Each oBaggage As BasketFlightExtra In Me.BasketFlightExtras.Where(Function(oExtra) oExtra.ExtraType = "Baggage")
					If oBaggage.ExtraBookingToken = oOption.Token Then
						Me.Flight.Price += oBaggage.Price * (oOption.Quantity - oBaggage.QuantitySelected)
						Me.Flight.OutboundPrice += oBaggage.Price * (oOption.Quantity - oBaggage.QuantitySelected)
						oBaggage.QuantitySelected = oOption.Quantity
					End If
				Next

			Next

		End Sub

		Public Sub UpdateReturnBaggageQuantity(ByVal oBaggageTokens As FlightResultHandler.BaggageOptions)

			For Each oOption As FlightResultHandler.BaggageOptions.BaggageOption In oBaggageTokens.BaggageOptions

				For Each oBaggage As BasketFlightExtra In Me.ReturnMultiCarrierFlightExtras.Where(Function(oExtra) oExtra.ExtraType = "Baggage")
					If oBaggage.ExtraBookingToken = oOption.Token Then
						Me.Flight.Price += oBaggage.Price * (oOption.Quantity - oBaggage.QuantitySelected)
						Me.Flight.ReturnMultiCarrierDetails.Price += oBaggage.Price * (oOption.Quantity - oBaggage.QuantitySelected)
						oBaggage.QuantitySelected = oOption.Quantity
					End If
				Next

			Next

		End Sub

		'this should really be one function for baggage and every other extra type
		'seperate for now as other sites use the normal baggage widget
		Public Sub UpdateFlightExtrasQuantity(ByVal oExtraTokens As FlightResultHandler.ExtraOptions)

			For Each oOption As FlightResultHandler.ExtraOptions.ExtraOption In oExtraTokens.ExtraOptions

				For Each oExtra As BasketFlightExtra In Me.BasketFlightExtras
					If oExtra.ExtraBookingToken = oOption.Token Then
						Me.Flight.Price += oExtra.Price * (oOption.Quantity - oExtra.QuantitySelected)
						Me.Flight.OutboundPrice += oExtra.Price * (oOption.Quantity - oExtra.QuantitySelected)
						oExtra.QuantitySelected = oOption.Quantity
					End If
				Next

			Next

		End Sub

		'this should really be one function for baggage and every other extra type
		'seperate for now as other sites use the normal baggage widget
		Public Sub UpdateReturnFlightExtrasQuantity(ByVal oExtraTokens As FlightResultHandler.ExtraOptions)

			For Each oOption As FlightResultHandler.ExtraOptions.ExtraOption In oExtraTokens.ExtraOptions

				For Each oExtra As BasketFlightExtra In Me.ReturnMultiCarrierFlightExtras
					If oExtra.ExtraBookingToken = oOption.Token Then
						Me.Flight.Price += oExtra.Price * (oOption.Quantity - oExtra.QuantitySelected)
						Me.Flight.ReturnMultiCarrierDetails.Price += oExtra.Price * (oOption.Quantity - oExtra.QuantitySelected)
						oExtra.QuantitySelected = oOption.Quantity
					End If
				Next

			Next

		End Sub

#End Region

	End Class

#End Region

#Region "Flight Price Request / Baggage"

	'this is not doing anything special to be asynchronous
	'just takes an object and returns another, rather than accessing the session like in the baggage setup function above
	Public Shared Function AsynchronousFlightPreBookRequest(FlightPreBookRequest As ivci.Flight.PreBookRequest) As ivci.Flight.PreBookResponse

		Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
		oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Flight.PreBookResponse)(FlightPreBookRequest)

		Dim oPriceResponse As New ivci.Flight.PreBookResponse

		If oIVCReturn.Success Then
			oPriceResponse = CType(oIVCReturn.ReturnObject, ivci.Flight.PreBookResponse)
		End If

		Return oPriceResponse

	End Function

#End Region

#End Region

#Region "Flight Itinerary"

	Public Shared Function ItinerarySearch(ByVal oBookingSearch As BookingSearch) As BookingSearch.SearchReturn

		oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightItinerary, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.FlightItinerarySearchRequest

		Try

			oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

			'Add details to the class
			With oiVectorConnectSearchRequest

				'set login details
				.LoginDetails = oBookingSearch.LoginDetails

				.FlightSectors.AddRange(oBookingSearch.FlightSectors)

				'pax
				With .GuestConfiguration
					.Adults = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Adults)
					.Children = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Children)
					.Infants = oBookingSearch.RoomGuests.Sum(Function(oRoom) oRoom.Infants)
					For Each oRoomGuest As BookingSearch.Guest In oBookingSearch.RoomGuests
						.ChildAges.AddRange(oRoomGuest.ChildAges)
					Next
				End With

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If

			End With

			oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

			'If everything is ok then send request
			If oSearchReturn.OK Then

				Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
				oHeaders.AddNew("processtimerguid", oBookingSearch.ProcessTimer.TimerGUID.ToString)

				oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				If oBookingSearch.EmailSearchTimes Then
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.FlightItinerarySearchResponse)(oiVectorConnectSearchRequest, oBookingSearch.EmailTo, Headers:=oHeaders)
				Else
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.FlightItinerarySearchResponse)(oiVectorConnectSearchRequest, Headers:=oHeaders)
				End If

				oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

				Dim oSearchResponse As New ivci.FlightItinerarySearchResponse

				Dim oRequestInfo As New BookingSearch.RequestInfo
				With oRequestInfo
					.RequestTime = oIVCReturn.RequestTime
					.RequestXML = oIVCReturn.RequestXML
					.ResponseXML = oIVCReturn.ResponseXML
					.Type = BookingSearch.RequestInfoType.FlightSearch
				End With

				oSearchReturn.RequestInfo = oRequestInfo

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.FlightItinerarySearchResponse)

					'if we have results set search return
					If oSearchResponse.Flights.Count > 0 Then
						oSearchReturn.FlightResults = oSearchResponse.Flights
						oSearchReturn.FlightCount = oSearchResponse.Flights.Count
						oSearchReturn.ExactMatchFlightCount = oSearchResponse.Flights.Where(Function(o) o.ExactMatch = True).Count
					End If

				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.FlightCount = 0
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/FlightSearch", "FlightSearchException", ex.ToString)
		End Try

		oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.FlightSearch, ProcessTimer.MainProcess, ProcessTimerItemType.Flight, 0)

		Return oSearchReturn

	End Function

#End Region

End Class
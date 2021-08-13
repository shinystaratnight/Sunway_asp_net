Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports Intuitive.Functions

Public Class FlightResultHandler

#Region "Properties"

	Private Property iVectorConnectResultStore As New Dictionary(Of Integer, ivci.Flight.SearchResponse.Flight)

	Public WorkTable As New Generic.List(Of WorkTableItem)
	Public ResultsSort As New Sort
	Public ResultsFilter As New Filters
	Public RequestDiagnostic As New RequestDiagnostic

	Public ReadOnly Property TotalFlights As Integer
		Get
			Dim iTotal As Integer = Me.WorkTable.Where(Function(o) o.Display = True).Count
			Return iTotal
		End Get
	End Property


	Public CurrentPage As Integer = 1

	Public ReadOnly Property TotalPages As Integer
		Get
			Return Functions.SafeInt(Math.Ceiling(Me.TotalFlights / Me.Params.HotelResultsPerPage))
		End Get
	End Property

	Public ReadOnly Property MinPrice As Decimal
		Get
			If Me.WorkTable.Count > 0 Then
				Dim nPrice As Decimal = Me.WorkTable.Min(Function(o) o.Price)
				Return nPrice
			Else
				Return 0
			End If
		End Get
	End Property

	Public MarkupAmount As Decimal
	Public MarkupPercentage As Decimal

#End Region

#Region "Accessors"
	''' <summary>
	''' Adds the results to our internal store to access later
	''' </summary>
	''' <param name="Key">Unique integer key for the result</param>
	''' <param name="iVectorConnectResult">ivc result</param>
	''' <remarks>Should only be being called in the save routine, which loops. So the key should always be unique. Overwrite if it isn't though to be safe</remarks>
	Private Sub SaveiVectorConnectResult(Key As Integer, ByVal iVectorConnectResult As ivci.Flight.SearchResponse.Flight)
		Me.iVectorConnectResultStore(Key) = iVectorConnectResult
	End Sub

	Public Function iVectorConnectResults(Key As Integer) As ivci.Flight.SearchResponse.Flight
		Return Me.iVectorConnectResultStore(Key)
	End Function

	Public Function iVectorConnectResults() As Generic.List(Of ivci.Flight.SearchResponse.Flight)
		Return Me.iVectorConnectResultStore.Values.ToList
	End Function

#End Region


	'markups should not be set on bookingbase - they need a rewrite
	Public Sub New(Params As BookingBase.ParamDef, SearchDetails As BookingSearch, Markups As List(Of BookingBase.Markup),
		  Lookups As Lookups)
		Me.Params = Params
		Me.SearchDetails = SearchDetails
		Me.Markups = Markups
		Me.Lookups = Lookups
	End Sub

	Public Sub New()

	End Sub

	Public Params As BookingBase.ParamDef
	Public SearchDetails As BookingSearch
	Public Markups As New List(Of BookingBase.Markup)
	Public Lookups As Lookups

#Region "Save"

	' Public Sub Save(ByVal iVectorConnectResults As Generic.List(Of ivci.Flight.SearchResponse.Flight)

	'Save
	Public Sub Save(ByVal iVectorConnectResults As Generic.List(Of ivci.Flight.SearchResponse.Flight),
					Optional ClearIVCResults As Boolean = True,
					Optional RequestInfo As BookingSearch.RequestInfo = Nothing)

		'1. Clear Work Table
		Me.ClearWorkTable(ClearIVCResults)


		'2. setup diagnostic
		If Not RequestInfo Is Nothing Then
			Me.RequestDiagnostic = RequestInfo.ConvertToDiagnostic()
		End If


		'3. Get Property Markups
		Dim aFlightMarkups As Generic.List(Of BookingBase.Markup) = Me.Markups.Where(Function(o) o.Component =
		   BookingBase.Markup.eComponentType.Flight AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

		'reset the markup
		Me.MarkupAmount = 0
		Me.MarkupPercentage = 0

		'update with new markup
		For Each oMarkup As BookingBase.Markup In aFlightMarkups
			Select Case oMarkup.Type
				Case BookingBase.Markup.eType.Amount
					Me.MarkupAmount += oMarkup.Value
				Case BookingBase.Markup.eType.AmountPP
					Me.MarkupAmount += oMarkup.Value * (Me.SearchDetails.TotalAdults + Me.SearchDetails.TotalChildren)
				Case BookingBase.Markup.eType.Percentage
					Me.MarkupPercentage = oMarkup.Value
			End Select
		Next


		'3. populate the work table with one work item per flight
		Dim iIndex As Integer = 0
		'For Each oFlightResult As ivci.Flight.SearchResponse.Flight In Me.iVectorConnectResults
		For Each oFlightResult As ivci.Flight.SearchResponse.Flight In iVectorConnectResults

			Me.SaveiVectorConnectResult(iIndex, oFlightResult)

			'3a . if we only want direct flights skip to next flight result if flight is not direct
			'if we only want direct flights skip to next flight result if flight is not direct
			If Me.Params.DirectFlightsOnly AndAlso
			(oFlightResult.NumberOfOutboundStops > 0 OrElse oFlightResult.NumberOfReturnStops > 0) Then Continue For


			'3c. Create ResultIndex and set index
			oFlightResult.TotalPrice += Me.MarkupAmount
			oFlightResult.TotalPrice *= (Me.MarkupPercentage / 100) + 1

			Dim bIsMultiCarrierResult As Boolean = False
			Dim sReturnBookingToken As String = ""
			Dim sReturnSource As String = ""
			Dim iReturnFlightSupplierID As Integer = 0
			Dim sReturnTPSessionID As String = ""
			Dim iReturnFlightCarrierID As Integer = 0
			If oFlightResult.MultiCarrierDetails IsNot Nothing Then
				bIsMultiCarrierResult = True
				sReturnBookingToken = oFlightResult.MultiCarrierDetails.BookingToken
				sReturnSource = oFlightResult.MultiCarrierDetails.Source
				iReturnFlightSupplierID = oFlightResult.MultiCarrierDetails.FlightSupplierID
				sReturnTPSessionID = oFlightResult.MultiCarrierDetails.TPSessionID
				iReturnFlightCarrierID = oFlightResult.ReturnFlightCarrierID
			End If

			Dim OutboundFlightDuration As Integer = oFlightResult.FlightSectors.Where(Function(o) _
														String.Compare(o.Direction, "outbound", StringComparison.CurrentCultureIgnoreCase) = 0).Sum(Function(o) o.TravelTime)
			Dim ReturnFlightDuration As Integer = oFlightResult.FlightSectors.Where(Function(o) _
														String.Compare(o.Direction, "return", StringComparison.CurrentCultureIgnoreCase) = 0).Sum(Function(o) o.TravelTime)

			Dim oItem As New WorkTableItem(iIndex, oFlightResult.BookingToken, oFlightResult.TotalPrice, oFlightResult.OutboundDepartureTime,
			  oFlightResult.ReturnDepartureTime, oFlightResult.FlightCarrierID, oFlightResult.NumberOfOutboundStops, oFlightResult.NumberOfReturnStops,
			  oFlightResult.OutboundDepartureDate, oFlightResult.ExactMatch, oFlightResult.ArrivalAirportID, oFlightResult.DepartureAirportID,
			  oFlightResult.OutboundFlightClassID, oFlightResult.OutboundArrivalDate, oFlightResult.OutboundFlightCode,
			  Me.FlightTimeOfDay(oFlightResult.OutboundDepartureTime), Me.FlightTimeOfDay(oFlightResult.ReturnDepartureTime),
			  Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, oFlightResult.FlightCarrierID), oFlightResult.TotalBaggagePrice,
			  bIsMultiCarrierResult, sReturnBookingToken, sReturnSource, iReturnFlightSupplierID, sReturnTPSessionID, iReturnFlightCarrierID,
			  OutboundFlightDuration, ReturnFlightDuration)


			'3d. Add ResultIndex to WorkTable
			Me.WorkTable.Add(oItem)
			iIndex += 1

		Next


		'store count
		'Me.ResultsFilter.FilteredFlightsCount = Me.WorkTable.Count


		'4. Store Result Handler On SearchDetails (which is also in the session)
		Me.SearchDetails.FlightResults = Me


		'5. Set filter date
		Me.ResultsFilter.OutboundDepartureDate = Me.SearchDetails.DepartureDate

	End Sub


#End Region


#Region "Remove Markup"

	Public Sub RemoveMarkup()


		Dim aFlightMarkups As Generic.List(Of BookingBase.Markup) = Me.Markups.Where(Function(o) o.Component =
		BookingBase.Markup.eComponentType.Flight AndAlso Not o.Value = 0).ToList


		For Each oFlightResult As ivci.Flight.SearchResponse.Flight In Me.iVectorConnectResults

			For Each oMarkup As BookingBase.Markup In aFlightMarkups
				Select Case oMarkup.Type
					Case BookingBase.Markup.eType.Amount
						oFlightResult.TotalPrice -= oMarkup.Value
					Case BookingBase.Markup.eType.AmountPP
						oFlightResult.TotalPrice -= oMarkup.Value * (Me.SearchDetails.TotalAdults + Me.SearchDetails.TotalChildren)
					Case BookingBase.Markup.eType.Percentage
						oFlightResult.TotalPrice /= 1 + (oMarkup.Value / 100)
				End Select
			Next

		Next

	End Sub

#End Region


#Region "Filter"

	'Filter Results
	Public Sub FilterResults(Filter As Filters, Optional ByVal DateChanged As Boolean = False)

		Me.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Filter flight results", ProcessTimer.MainProcess)


		'1
		Me.ResultsFilter = Filter


		'2. if date has changed reset min max flight time minutes and min max price so that we do not incorrectly filter out the flights
		If DateChanged Then
			Me.ResultsFilter.MinOutboundDepartureFlightTimeMinutes = 0
			Me.ResultsFilter.MaxOutboundDepartureFlightTimeMinutes = 1439
			Me.ResultsFilter.MinReturnDepartureFlightTimeMinutes = 0
			Me.ResultsFilter.MaxReturnDepartureFlightTimeMinutes = 1439
			Me.ResultsFilter.MinPrice = 0
			Me.ResultsFilter.MaxPrice = 0
			Me.ResultsFilter.MinFlightDurationMinutes = 0
			Me.ResultsFilter.MaxFlightDurationMinutes = 0
		End If

		'3. Generate Filter Counts
		Me.GenerateFilterCounts()


		'4. Set Display value for each work item
		For Each oItem As WorkTableItem In Me.WorkTable

			'4.a Filter Result
			Me.FilterItem(oItem)

		Next

		'4. Filtered Flights
		'Me.ResultsFilter.FilteredFlightsCount = Me.WorkTable.Where(Function(o) o.Display = True).Count

		'4. Generate Price Bands
		Me.GeneratePriceBandFilter()

		'5. Order Filter
		Me.OrderFilters()

		'if anywhere search check selected flights are still valid
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			Me.CheckValidSelectedFlights()
		End If

		Me.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Filter flight results", ProcessTimer.MainProcess)

	End Sub

	'Order Filters
	Public Sub OrderFilters()

		'Departure Times
		Me.ResultsFilter.DepartureTimes = Me.ResultsFilter.DepartureTimes.OrderBy(Function(o) o.Sequence).ToList

		'Return Times
		Me.ResultsFilter.ReturnTimes = Me.ResultsFilter.ReturnTimes.OrderBy(Function(o) o.Sequence).ToList

		'Stops
		Me.ResultsFilter.Stops = Me.ResultsFilter.Stops.OrderBy(Function(o) o.Stops).ToList

		'Flight Carriers
		Me.ResultsFilter.FlightCarriers = Me.ResultsFilter.FlightCarriers.OrderBy(Function(o) o.FlightCarrier).ToList


	End Sub

	Public Sub FilterItem(ByVal oWorkTableItem As WorkTableItem, Optional ByVal IgnoreOutboundFlightTime As Boolean = False,
	 Optional ByVal IgnoreReturnFlightTime As Boolean = False, Optional ByVal IgnoreFlightCarrier As Boolean = False,
	  Optional ByVal IgnoreStops As Boolean = False, Optional IgnoreDepartureAirport As Boolean = False,
	 Optional IgnoreArrivalAirport As Boolean = False, Optional IgnoreFlightClass As Boolean = False,
	 Optional IgnoreFlightDuration As Boolean = False)

		Dim oFlight As ivci.Flight.SearchResponse.Flight = Me.iVectorConnectResults(oWorkTableItem.Index)

		'Properties
		Dim bDisplay As Boolean = True

		'outbound departure date
		If bDisplay AndAlso oFlight.OutboundDepartureDate <> Me.ResultsFilter.OutboundDepartureDate Then
			bDisplay = False
		End If



		'outbound departure times of day
		Dim eOutboundTimeOfDay As eTimeOfDay = Me.FlightTimeOfDay(oFlight.OutboundDepartureTime)
		If bDisplay AndAlso Not IgnoreOutboundFlightTime AndAlso Me.ResultsFilter.FilterDepartureTimes.Count > 0 Then
			If Not Me.ResultsFilter.FilterDepartureTimes.Contains(eOutboundTimeOfDay) Then
				bDisplay = False
			End If
		End If

		' old departure times of day 
		'If bDisplay AndAlso Not IgnoreOutboundFlightTime AndAlso Me.ResultsFilter.DepartureCSV <> "" Then
		'	Dim aDepartureTimes As String() = Me.ResultsFilter.DepartureCSV.Split(","c)
		'	If Not aDepartureTimes.Contains(FlightResultHandler.FlightTimeOfDay(oFlight.OutboundDepartureTime)) Then
		'		bDisplay = False
		'	End If
		'End If


		'return departure times of day
		Dim eReturnTimeOfDay As eTimeOfDay = Me.FlightTimeOfDay(oFlight.ReturnDepartureTime)
		If bDisplay AndAlso Not IgnoreReturnFlightTime AndAlso Me.ResultsFilter.FilterReturnTimes.Count > 0 Then
			If Not Me.ResultsFilter.FilterReturnTimes.Contains(eReturnTimeOfDay) Then
				bDisplay = False
			End If
		End If

		'old return departure times of day 
		'If bDisplay AndAlso IgnoreReturnFlightTime AndAlso Me.ResultsFilter.ReturnCSV <> "" Then
		'	Dim aReturnTimes As String() = Me.ResultsFilter.ReturnCSV.Split(","c)
		'	If Not aReturnTimes.Contains(FlightResultHandler.FlightTimeOfDay(oFlight.ReturnDepartureTime)) Then
		'		bDisplay = False
		'	End If
		'End If


		'  outbound departure time
		If bDisplay AndAlso (Me.ResultsFilter.MinOutboundDepartureFlightTimeMinutes <> 0 OrElse Me.ResultsFilter.MaxOutboundDepartureFlightTimeMinutes <> 0) Then
			Dim sMinOutboundDepartureTime As String = SafeString(New TimeSpan(0, Me.ResultsFilter.MinOutboundDepartureFlightTimeMinutes, 0)).Substring(0, 5)
			Dim sMaxOutboundDepartureTime As String = SafeString(New TimeSpan(0, Me.ResultsFilter.MaxOutboundDepartureFlightTimeMinutes, 0)).Substring(0, 5)

			'.net can compare strings and knows whether the time is greater
			If oFlight.OutboundDepartureTime < sMinOutboundDepartureTime OrElse oFlight.OutboundDepartureTime > sMaxOutboundDepartureTime Then
				bDisplay = False
			End If
		End If


		'Return departure time, will not be set if its a one way or Itinerary search.
		If Not BookingBase.SearchDetails.OneWay AndAlso Me.SearchDetails.FlightSearchMode = BookingSearch.FlightSearchModes.FlightSearch Then

			If bDisplay AndAlso (Me.ResultsFilter.MinReturnDepartureFlightTimeMinutes <> 0 OrElse Me.ResultsFilter.MaxReturnDepartureFlightTimeMinutes <> 0) Then
				Dim sMinReturnDepartureTime As String = SafeString(New TimeSpan(0, Me.ResultsFilter.MinReturnDepartureFlightTimeMinutes, 0)).Substring(0, 5)
				Dim sMaxReturnDepartureTime As String = SafeString(New TimeSpan(0, Me.ResultsFilter.MaxReturnDepartureFlightTimeMinutes, 0)).Substring(0, 5)

				'.net can compare strings and knows whether the time is greater
				If oFlight.ReturnDepartureTime < sMinReturnDepartureTime OrElse oFlight.ReturnDepartureTime > sMaxReturnDepartureTime Then
					bDisplay = False
				End If
			End If
		End If

		'flight carriers
		If bDisplay AndAlso Not IgnoreFlightCarrier AndAlso Me.ResultsFilter.FilterFlightCarrierIDs.Count > 0 Then
			If Not Me.ResultsFilter.FilterFlightCarrierIDs.Contains(oFlight.FlightCarrierID) AndAlso
				Not Me.ResultsFilter.FilterFlightCarrierIDs.Contains(oFlight.ReturnFlightCarrierID) Then
				bDisplay = False
			End If
		End If

		'old flight carrier filtering
		'If bDisplay AndAlso Not IgnoreFlightCarrier AndAlso Me.ResultsFilter.FlightCarrerIDCSV <> "" Then
		'	Dim aFlightCarriers As String() = Me.ResultsFilter.FlightCarrerIDCSV.Split(","c)
		'	If Not aFlightCarriers.Contains(oFlight.FlightCarrierID.ToString) Then
		'		bDisplay = False
		'	End If
		'End If


		'stops
		If bDisplay AndAlso Not IgnoreStops AndAlso Me.ResultsFilter.FilterStops.Count > 0 Then
			Dim iMaxStops As Integer = Math.Max(oFlight.NumberOfOutboundStops, oFlight.NumberOfReturnStops)
			If Not Me.ResultsFilter.FilterStops.Contains(iMaxStops) Then
				bDisplay = False
			End If
		End If

		'old stops filterin
		'If bDisplay AndAlso Not IgnoreStops AndAlso Me.ResultsFilter.StopsCSV <> "" Then
		'	Dim aFlightStops As String() = Me.ResultsFilter.StopsCSV.Split(","c)
		'	Dim iMaxStops As Integer = Math.Max(oFlight.NumberOfOutboundStops, oFlight.NumberOfReturnStops)
		'	If Not aFlightStops.Contains(iMaxStops.ToString) Then
		'		bDisplay = False
		'	End If
		'End If


		'price
		If bDisplay AndAlso Me.ResultsFilter.MaxPrice <> 0 Then
			If oFlight.TotalPrice < Me.ResultsFilter.MinPrice OrElse oFlight.TotalPrice > Me.ResultsFilter.MaxPrice Then
				bDisplay = False
			End If
		End If

		'booking tokens - if it matches remove, different to the other filters
		If bDisplay AndAlso Me.ResultsFilter.BookingTokenCSV <> "" Then
			Dim aBookingTokens As String() = Me.ResultsFilter.BookingTokenCSV.Split(","c)
			If aBookingTokens.Contains(oFlight.BookingToken.ToString) Then
				bDisplay = False
			End If
		End If


		'departure airport
		If bDisplay AndAlso Not IgnoreDepartureAirport AndAlso Me.ResultsFilter.FilterDepartureAirportIDs.Count > 0 Then
			If Not Me.ResultsFilter.FilterDepartureAirportIDs.Contains(oFlight.DepartureAirportID) Then
				bDisplay = False
			End If
		End If


		'arrival airport
		If bDisplay AndAlso Not IgnoreArrivalAirport AndAlso Me.ResultsFilter.FilterArrivalAirportIDs.Count > 0 Then
			If Not Me.ResultsFilter.FilterArrivalAirportIDs.Contains(oFlight.ArrivalAirportID) Then
				bDisplay = False
			End If
		End If


		'flight class
		If bDisplay AndAlso Not IgnoreFlightClass AndAlso Me.ResultsFilter.FilterFlightClassIDs.Count > 0 Then
			If Not Me.ResultsFilter.FilterFlightClassIDs.Contains(oFlight.OutboundFlightClassID) Then
				bDisplay = False
			End If
		End If

		'flight duration
		If bDisplay AndAlso Not IgnoreFlightDuration AndAlso Me.ResultsFilter.MaxFlightDurationMinutes <> 0 Then

			Dim iMin As Integer = Me.ResultsFilter.MinFlightDurationMinutes
			Dim iMax As Integer = Me.ResultsFilter.MaxFlightDurationMinutes

			If iMin > iMax Then
				Dim iSwap As Integer = iMax
				iMax = iMin
				iMin = iSwap
			End If

			Dim iRangeDifference As Integer = iMax - iMin

			Dim FlightDurationRange As IEnumerable(Of Integer) = Enumerable.Range(iMin, iRangeDifference).ToList()
			If Not FlightDurationRange.Contains(oWorkTableItem.OutboundFlightDuration) OrElse
			   Not FlightDurationRange.Contains(oWorkTableItem.ReturnFlightDuration) Then
				bDisplay = False
			End If
		End If

		oWorkTableItem.Display = bDisplay

	End Sub

	'Filter Items
	Public Sub FilterItems(ByVal WorkTable As Generic.List(Of WorkTableItem), Optional bIgnoreOutboundFlightTime As Boolean = False,
	  Optional bIgnoreReturnFlightTime As Boolean = False, Optional bIgnoreFlightCarrier As Boolean = False,
	  Optional bIgnoreStops As Boolean = False, Optional bIgnoreDepartureAirport As Boolean = False,
	  Optional bIgnoreArrivalAirport As Boolean = False, Optional bIgnoreFlightClass As Boolean = False)

		For Each oItem As WorkTableItem In WorkTable

			Me.FilterItem(oItem, bIgnoreOutboundFlightTime, bIgnoreReturnFlightTime, bIgnoreFlightCarrier, bIgnoreStops, bIgnoreDepartureAirport,
			  bIgnoreArrivalAirport, bIgnoreFlightClass)


		Next

	End Sub

	Public Sub GenerateFilterCounts()

		'1. Clear Filter Counts
		Me.ResultsFilter.Clear()


		'2. Outbound departure times
		'2.1 Filter flight results on everything except the outbound flight times
		Me.FilterItems(Me.WorkTable, bIgnoreOutboundFlightTime:=True)


		'2.2 Generate list of departure time
		Dim aDepartureTimes As IEnumerable(Of eTimeOfDay) = (From oFlight In Me.WorkTable Select oFlight.OutboundDepartureTimeOfDay).Distinct

		For Each eTimeOfDay As eTimeOfDay In aDepartureTimes
			Dim oFlightTime As New Filters.FlightTime
			With oFlightTime
				.TimeOfDay = eTimeOfDay
				.Sequence = Me.FlightTimeOfDaySequence(eTimeOfDay)
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.OutboundDepartureTimeOfDay = oFlightTime.TimeOfDay And oFlight.Display = True).Count()

				If .Count > 0 Then
					.FromPrice = Me.WorkTable.Where(Function(oFlight) oFlight.OutboundDepartureTimeOfDay = oFlightTime.TimeOfDay And oFlight.Display = True).Min(Function(oFlight) oFlight.Price)
				Else
					.FromPrice = 0
				End If


			End With
			Me.ResultsFilter.DepartureTimes.Add(oFlightTime)
		Next


		'3. Return departure times
		'3.1 Filter flight results on everything except the return flight times
		Me.FilterItems(Me.WorkTable, bIgnoreReturnFlightTime:=True)

		'3.2 Generate list of return time
		Dim aReturnTimes As IEnumerable(Of eTimeOfDay) = (From oFlight In Me.WorkTable Select oFlight.ReturnDepartureTimeOfDay).Distinct

		For Each eTimeOfDay As eTimeOfDay In aReturnTimes
			Dim oFlightTime As New Filters.FlightTime
			With oFlightTime
				.TimeOfDay = eTimeOfDay
				.Sequence = Me.FlightTimeOfDaySequence(eTimeOfDay)
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.ReturnDepartureTimeOfDay = oFlightTime.TimeOfDay And oFlight.Display = True).Count()

				If .Count > 0 Then
					.FromPrice = Me.WorkTable.Where(Function(oFlight) oFlight.ReturnDepartureTimeOfDay = oFlightTime.TimeOfDay And oFlight.Display = True).Min(Function(oFlight) oFlight.Price)
				Else
					.FromPrice = 0
				End If


			End With
			Me.ResultsFilter.ReturnTimes.Add(oFlightTime)
		Next


		'4. flight carriers

		'4.1 filter results on everything except flight carriers
		Me.FilterItems(Me.WorkTable, bIgnoreFlightCarrier:=True)

		'4.2 Generate list of carriers
		Dim aFlightCarrierIDs As IEnumerable(Of Integer) = (From oFlight In Me.WorkTable Select oFlight.FlightCarrierID).Union(From oFlight In Me.WorkTable Select oFlight.ReturnFlightCarrierID).Distinct
		Dim aSelectedFlightCarriers As List(Of Integer) = Me.ResultsFilter.FilterFlightCarrierIDs

		For Each iFlightCarrierID As Integer In aFlightCarrierIDs

			If iFlightCarrierID <> 0 Then
				Dim oFilterFlightCarrier As New Filters.FlightCarrier
				With oFilterFlightCarrier
					.FlightCarrierID = iFlightCarrierID
					.FlightCarrier = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, iFlightCarrierID)
					.FlightCarrierLogo = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierLogo, iFlightCarrierID)
					.Count = Me.WorkTable.Where(Function(oFlight) (oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID OrElse
																   oFlight.ReturnFlightCarrierID = oFilterFlightCarrier.FlightCarrierID) AndAlso
																	oFlight.Display = True).Count()

					If .Count > 0 Then
						.FromPrice = Me.WorkTable.Where(Function(oFlight) (oFlight.FlightCarrierID = oFilterFlightCarrier.FlightCarrierID OrElse
																		   oFlight.ReturnFlightCarrierID = oFilterFlightCarrier.FlightCarrierID) AndAlso
																			oFlight.Display = True).Min(Function(oFlight) oFlight.Price)
					Else
						.FromPrice = 0
					End If

					.Selected = aSelectedFlightCarriers.Contains(iFlightCarrierID)

				End With
				Me.ResultsFilter.FlightCarriers.Add(oFilterFlightCarrier)

			End If

		Next

		'If Me.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso Me.ResultsFilter.FlightCarriers.Where(Function(o) o.Selected = True).Count = 0 Then
		'	Me.ResultsFilter.FlightCarriers.OrderBy(Function(o) o.FromPrice)(0).Selected = True
		'End If


		'5. stops (maximum from either leg) - filter results on everything except stops
		Me.FilterItems(Me.WorkTable, bIgnoreStops:=True)

		'5.2 Generate list of stops
		Dim aFlightStops As IEnumerable(Of Integer) = (From oFlight In Me.WorkTable Select oFlight.MaxStops).Distinct
		Dim aSelectedFlightStops As List(Of Integer) = Me.ResultsFilter.FilterStops

		For Each iStop As Integer In aFlightStops

			Dim oFlightStop As New Filters.FlightStop
			With oFlightStop
				.Stops = iStop
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.MaxStops = oFlightStop.Stops And oFlight.Display = True).Count()

				If .Count > 0 Then
					.FromPrice = Me.WorkTable.Where(Function(oFlight) oFlight.MaxStops = oFlightStop.Stops And oFlight.Display).Min(Function(oFlight) oFlight.Price)
				Else
					.FromPrice = 0
				End If

				.Selected = aSelectedFlightStops.Contains(iStop)
			End With
			Me.ResultsFilter.Stops.Add(oFlightStop)

		Next


		'9 departure and arrival airports

		'9.1 Departure airports
		Me.FilterItems(Me.WorkTable, bIgnoreDepartureAirport:=True)
		Dim aDepartureAirports As IEnumerable(Of Integer) = (From oFlight In Me.WorkTable Select oFlight.DepartureAirportID).Distinct

		For Each iDepartureAirportID As Integer In aDepartureAirports
			Dim oFilterDepartureAirport As New Filters.Airport

			With oFilterDepartureAirport
				.AirportID = iDepartureAirportID
				.AirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, iDepartureAirportID)
				.AirportName = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, iDepartureAirportID)
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.DepartureAirportID = oFilterDepartureAirport.AirportID And oFlight.Display).Count
				.Selected = aDepartureAirports.Contains(iDepartureAirportID)
			End With
			Me.ResultsFilter.DepartureAirports.Add(oFilterDepartureAirport)
		Next


		'9.2 Arrival airports
		Me.FilterItems(Me.WorkTable, bIgnoreArrivalAirport:=True)
		Dim aArrivalAirports As IEnumerable(Of Integer) = (From oFlight In Me.WorkTable Select oFlight.ArrivalAirportID).Distinct

		For Each iArrivalAirportID As Integer In aArrivalAirports
			Dim oFilterArrivalAirport As New Filters.Airport

			With oFilterArrivalAirport
				.AirportID = iArrivalAirportID
				.AirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, iArrivalAirportID)
				.AirportName = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, iArrivalAirportID)
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.ArrivalAirportID = oFilterArrivalAirport.AirportID And oFlight.Display).Count
				.Selected = aArrivalAirports.Contains(iArrivalAirportID)
			End With
			Me.ResultsFilter.ArrivalAirports.Add(oFilterArrivalAirport)
		Next


		'10. Flight Classes
		Me.FilterItems(Me.WorkTable, bIgnoreFlightClass:=True)
		Dim aFlightClasses As IEnumerable(Of Integer) = (From oFlight In Me.WorkTable Select oFlight.OutboundFlightClassID).Distinct

		For Each iFlightClassID As Integer In aFlightClasses
			Dim oFilterFlightClass As New Filters.FlightClass

			With oFilterFlightClass
				.FlightClassID = iFlightClassID
				.FlightClass = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, iFlightClassID)
				.Count = Me.WorkTable.Where(Function(oFlight) oFlight.OutboundFlightClassID = oFilterFlightClass.FlightClassID And oFlight.Display).Count()
			End With
			Me.ResultsFilter.FlightClasses.Add(oFilterFlightClass)
		Next


	End Sub

	Public Sub GeneratePriceBandFilter()

		'6. Min/Max Price
		Me.ResultsFilter.MinPrice = 0
		Me.ResultsFilter.MaxPrice = 0

		If Me.WorkTable.Where(Function(o) o.Display = True).Count > 0 Then
			Me.ResultsFilter.MinPrice = Me.WorkTable.Min(Function(oFlight) oFlight.Price)
			Me.ResultsFilter.MaxPrice = Me.WorkTable.Max(Function(oFlight) oFlight.Price)
		End If


		'7 min max outbound times and duration
		'reset these, so we only use if not equal to 0
		Me.ResultsFilter.MinOutboundDepartureFlightTime = ""
		Me.ResultsFilter.MaxOutboundDepartureFlightTime = ""
		Me.ResultsFilter.MinOutboundDepartureFlightTimeMinutes = 0
		Me.ResultsFilter.MaxOutboundDepartureFlightTimeMinutes = 0
		Me.ResultsFilter.MinFlightDurationMinutes = 0
		Me.ResultsFilter.MaxFlightDurationMinutes = 0

		If Me.WorkTable.Where(Function(o) o.Display = True).Count > 0 Then
			Me.ResultsFilter.MinOutboundDepartureFlightTime = Me.WorkTable.Where(Function(o) o.Display = True).Min(Function(o) o.OutboundDepartureTime)
			Me.ResultsFilter.MaxOutboundDepartureFlightTime = Me.WorkTable.Where(Function(o) o.Display = True).Max(Function(o) o.OutboundDepartureTime)
			Me.ResultsFilter.MinOutboundDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.ResultsFilter.MinOutboundDepartureFlightTime).TotalMinutes)
			Me.ResultsFilter.MaxOutboundDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.ResultsFilter.MaxOutboundDepartureFlightTime).TotalMinutes)

			If Me.WorkTable.Any(Function(o) o.Display = True AndAlso o.OutboundFlightDuration > 0 AndAlso o.ReturnFlightDuration > 0) Then
				Dim iMinFlightMinutes As Integer = Math.Min(Me.WorkTable.Where(Function(o) o.Display = True AndAlso o.OutboundFlightDuration > 0).Min(Function(o) o.OutboundFlightDuration),
																	 Me.WorkTable.Where(Function(o) o.Display = True AndAlso o.ReturnFlightDuration > 0).Min(Function(o) o.ReturnFlightDuration))
				Dim iMaxFlightMinutes As Integer = Math.Max(Me.WorkTable.Where(Function(o) o.Display = True).Max(Function(o) o.OutboundFlightDuration),
																	 Me.WorkTable.Where(Function(o) o.Display = True).Max(Function(o) o.ReturnFlightDuration))

				Dim iFlightDurationStep As Integer = 15
				Me.ResultsFilter.MinFlightDurationMinutes = SafeInt(Math.Floor(iMinFlightMinutes / iFlightDurationStep) * iFlightDurationStep)
				Me.ResultsFilter.MaxFlightDurationMinutes = SafeInt(Math.Ceiling(iMaxFlightMinutes / iFlightDurationStep) * iFlightDurationStep)
			End If
		End If

		'Don't filter if it's a one way flight or FI search
		If Not BookingBase.SearchDetails.OneWay AndAlso Me.SearchDetails.FlightSearchMode = BookingSearch.FlightSearchModes.FlightSearch Then

			'8 min max return times reset these, so we only use if not equal to 0
			Me.ResultsFilter.MinReturnDepartureFlightTime = ""
			Me.ResultsFilter.MaxReturnDepartureFlightTime = ""
			Me.ResultsFilter.MinReturnDepartureFlightTimeMinutes = 0
			Me.ResultsFilter.MaxReturnDepartureFlightTimeMinutes = 0

			If Me.WorkTable.Where(Function(o) o.Display = True).Count > 0 Then
				Me.ResultsFilter.MinReturnDepartureFlightTime = Me.WorkTable.Where(Function(o) o.Display = True).Min(Function(o) o.ReturnDepartureTime)
				Me.ResultsFilter.MaxReturnDepartureFlightTime = Me.WorkTable.Where(Function(o) o.Display = True).Max(Function(o) o.ReturnDepartureTime)
				Me.ResultsFilter.MinReturnDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.ResultsFilter.MinReturnDepartureFlightTime).TotalMinutes)
				Me.ResultsFilter.MaxReturnDepartureFlightTimeMinutes = SafeInt(TimeSpan.Parse(Me.ResultsFilter.MaxReturnDepartureFlightTime).TotalMinutes)
			End If

		End If
	End Sub

	Public Sub CheckValidSelectedFlights()

		Dim oSelectedFlights As New Dictionary(Of Integer, Flight)

		For Each oPropertyItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
			Dim oCurrentFlight As Flight = Nothing
			Dim bFlightDisplayed As Boolean = False
			If BookingBase.SearchDetails.PropertyResults.HotelFlightDictionary.ContainsKey(oPropertyItem.PropertyReferenceID) Then
				oCurrentFlight = BookingBase.SearchDetails.PropertyResults.HotelFlightDictionary(oPropertyItem.PropertyReferenceID)
				Dim oFlightItem As WorkTableItem = Me.WorkTable.FirstOrDefault(Function(item) item.BookingToken = oCurrentFlight.BookingToken)
				bFlightDisplayed = oFlightItem.Display
			End If

			Dim oFlight As Flight = Me.GetDefaultFlight(oPropertyItem.GeographyLevel3ID)
			If oCurrentFlight Is Nothing OrElse Not bFlightDisplayed Then
				If Not oFlight Is Nothing Then
					oSelectedFlights.Add(oPropertyItem.PropertyReferenceID, oFlight)
				End If
			Else
				If oFlight.Total < oCurrentFlight.Total Then
					oSelectedFlights.Add(oPropertyItem.PropertyReferenceID, oFlight)
				Else
					oSelectedFlights.Add(oPropertyItem.PropertyReferenceID, oCurrentFlight)
				End If
			End If
		Next

		BookingBase.SearchDetails.PropertyResults.HotelFlightDictionary = oSelectedFlights
		BookingBase.SearchDetails.PropertyResults.FilterResults(BookingBase.SearchDetails.PropertyResults.ResultsFilter)
	End Sub

#End Region

#Region "Sort"

	Public Sub SortResults(ByVal SortBy As String, ByVal SortOrder As String)

		'build new sort 
		Dim oSort As New Sort
		With oSort
			.SortBy = Functions.SafeEnum(Of eSortBy)(SortBy)
			.SortOrder = Functions.SafeEnum(Of eSortOrder)(SortOrder)
		End With

		'save sort of results
		Me.ResultsSort = oSort

		'sort
		Me.SortResults()

	End Sub

	'Sort Results
	Public Sub SortResults()

		Me.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Sort flight results", ProcessTimer.MainProcess)

		'1. Order WorkTable
		If Me.ResultsSort.SortBy = eSortBy.DepartureTime Then
			If Me.ResultsSort.SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderBy(Function(o) o.OutboundDepartureTime).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderByDescending(Function(o) o.OutboundDepartureTime).ToList
			End If
		ElseIf Me.ResultsSort.SortBy = eSortBy.Price Then
			If Me.ResultsSort.SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderByDescending(Function(o) o.Price).ToList
			End If
		ElseIf Me.ResultsSort.SortBy = eSortBy.Airline Then
			If Me.ResultsSort.SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderBy(Function(o) o.FlightCarrier).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderByDescending(Function(o) o.FlightCarrier).ToList
			End If
		ElseIf Me.ResultsSort.SortBy = eSortBy.ReturnTime Then
			If Me.ResultsSort.SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderBy(Function(o) o.ReturnDepartureTime).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Price).OrderByDescending(Function(o) o.ReturnDepartureTime).ToList
			End If
		End If

		Me.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Sort flight results", ProcessTimer.MainProcess)

	End Sub

#End Region


#Region "Get"

	'return XML
	Public Function GetFullXML() As XmlDocument

		'Get Every Flight As XML
		Dim oResultXML As XmlDocument = Me.GetPageXML(-1)
		Return oResultXML

	End Function

	'Get Results as XML
	Public Function GetPageXML(ByVal Page As Integer) As XmlDocument

		Dim oResults As New Results
		oResults.Flights = Me.GetFlightsPage(Page)
		oResults.Filters = Me.ResultsFilter

		oResults.MinPrice = Me.MinPrice

		'serialize to xml and return
		Dim oFlightsXML As XmlDocument = Serializer.Serialize(oResults, True)
		Return oFlightsXML

	End Function


	'Get Flight Page
	Public Function GetFlightsPage(ByVal Page As Integer) As Generic.List(Of FlightResultHandler.Flight)

		'1. Get oIVectorConnectResultsIndexs
		Dim iVectorConnectResultsIndexes As Generic.List(Of IVectorConnectResultsIndex) = GetRange(Page)

		Dim oFlights As New Generic.List(Of Flight)

		For Each iVectorConnectResultsIndex As IVectorConnectResultsIndex In iVectorConnectResultsIndexes
			'add flight to results
			Dim oFlight As Flight = Me.GenerateFlight(iVectorConnectResultsIndex.FlightResult, iVectorConnectResultsIndex.Index)
			If Not oFlight Is Nothing Then
				oFlights.Add(oFlight)
			End If
		Next

		Return oFlights

	End Function


	'Get Range
	Public Function GetRange(ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. List Of iVectorConnectResultIndex
		Dim oIVectorConnectResultsIndexes As New Generic.List(Of IVectorConnectResultsIndex)

		'2. Get List of WorkTableItems
		Dim oWorkTable As Generic.List(Of WorkTableItem) = Me.GetWorkTable(Page)

		'3. Add PropertyResult to oIVectorConnectResultsIndex from iVectorConnectResults using index
		For Each oItem As WorkTableItem In oWorkTable
			Dim oIVectorConnectResultsIndex As New IVectorConnectResultsIndex
			oIVectorConnectResultsIndex.Index = oItem.Index
			oIVectorConnectResultsIndex.FlightResult = (Me.iVectorConnectResults(oItem.Index))
			oIVectorConnectResultsIndexes.Add(oIVectorConnectResultsIndex)
		Next

		'4. Return
		Return oIVectorConnectResultsIndexes

	End Function

	Public Class Results
		Public MinPrice As Decimal
		Public MaxPrice As Decimal
		Public Flights As New Generic.List(Of Flight)
		Public Filters As Filters
	End Class


	Public Class IVectorConnectResultsIndex
		Public Index As Integer
		Public FlightResult As ivci.Flight.SearchResponse.Flight
	End Class



	'Get Work Table Item - Get Range of work table items using Page to generate a range
	Public Function GetWorkTable(ByVal Page As Integer) As Generic.List(Of WorkTableItem)

		Dim oWorkTable As New Generic.List(Of WorkTableItem)

		Try

			'1. Select WorkItems where Display = true
			oWorkTable = Me.WorkTable.Where(Function(o) o.Display = True).ToList

			'2. Set range indexes
			Dim iStartIndex As Integer = (Page - 1) * Me.Params.FlightResultsPerPage
			Dim iCount As Integer = Functions.IIf(oWorkTable.Count < Me.Params.FlightResultsPerPage, oWorkTable.Count, Me.Params.FlightResultsPerPage)
			iCount = Functions.IIf(iStartIndex + iCount > oWorkTable.Count, oWorkTable.Count - iStartIndex, iCount)

			'3. Select WorkTableItems within range
			If Page > 0 Then
				oWorkTable = oWorkTable.GetRange(iStartIndex, iCount)
			End If



		Catch ex As Exception
			oWorkTable = New Generic.List(Of WorkTableItem)
		End Try

		'3. Return WorkTable
		Return oWorkTable

	End Function



	Public Function GetDefaultFlightPerResort(aResorts As IEnumerable(Of Integer)) As Dictionary(Of Integer, Flight)


		Dim oDictionary As New Dictionary(Of Integer, Flight)

		For Each iResort As Integer In aResorts

			Dim oFlight As Flight = Me.GetDefaultFlight(iResort)

			If Not oFlight Is Nothing Then
				oDictionary.Add(iResort, oFlight)
			End If

		Next

		Return oDictionary

	End Function

	Public Function GetFlightOptionsForResort(ByVal GeographyLevel3ID As Integer, ByVal SelectedFlightToken As String) As List(Of Flight)

		Dim oFlightOptions As New List(Of Flight)

		Dim oFlights As List(Of WorkTableItem) = Me.WorkTable _
			.Where(Function(oFlight) oFlight.Display _
			AndAlso oFlight.ExactMatch _
			AndAlso Not oFlight.BookingToken = SelectedFlightToken _
			AndAlso (Me.Lookups.AirportResortCheck(oFlight.ArrivalAirportID, GeographyLevel3ID))).ToList()

		For Each oWorkTableItem As WorkTableItem In oFlights
			Dim oIVCFlight As ivci.Flight.SearchResponse.Flight = Me.iVectorConnectResults(oWorkTableItem.Index)
			Dim oFlightOption As FlightResultHandler.Flight = Me.GenerateFlight(oIVCFlight, oWorkTableItem.Index)
			oFlightOptions.Add(oFlightOption)
		Next

		Return oFlightOptions
	End Function


	Public Function GetDefaultFlight(ByVal GeographyLevel3ID As Integer) As Flight

		Dim oItem As WorkTableItem = Me.WorkTable.
			Where(Function(oFlight) oFlight.Display AndAlso
				oFlight.ExactMatch AndAlso
				(Me.Lookups.AirportResortCheck(oFlight.ArrivalAirportID, GeographyLevel3ID))).
			OrderBy(Function(o) o.Price).FirstOrDefault()

		'because we are now looking in the work table instead of all the flights listed - we need to check if we have something to return
		If Not oItem Is Nothing Then

			Dim oIVCFlight As ivci.Flight.SearchResponse.Flight = Me.iVectorConnectResults(oItem.Index)
			Dim oFlightOption As FlightResultHandler.Flight = Me.GenerateFlight(oIVCFlight, oItem.Index)

			Return oFlightOption

		End If

		Return Nothing

	End Function


	Public Sub AddCheapestFlightToBasket()

		Dim oItem As WorkTableItem = Me.WorkTable.Where(Function(flight) flight.ExactMatch).OrderBy(Function(o) o.Price).ThenBy(Function(o) o.MaxStops).ThenBy(
		 Function(o) o.NumberOfOutboundStops).ThenBy(Function(o) o.NumberOfReturnStops).ThenBy(
		 Function(o) o.OutboundDepartureTime).ThenByDescending(Function(o) o.ReturnDepartureTime).ToList.First

		Dim oIVCFlight As ivci.Flight.SearchResponse.Flight = Me.iVectorConnectResults(oItem.Index)

		Dim oFlightOption As FlightResultHandler.Flight = Me.GenerateFlight(oIVCFlight, oItem.Index)

		BookingFlight.AddFlightToBasket(oFlightOption.FlightOptionHashToken)

	End Sub


	'Get Single Hotel
	Public Function GetSingleFlight(ByVal sBookingToken As String, Optional ByVal sMultiCarrierReturnBookingToken As String = "") As FlightResultHandler.Flight

		'1. Get Work Table Item
		Dim oItem As WorkTableItem = Me.WorkTable.FirstOrDefault(Function (o) o.BookingToken = sBookingToken _
            AndAlso (String.IsNullOrEmpty(sMultiCarrierReturnBookingToken) OrElse o.ReturnBookingToken = sMultiCarrierReturnBookingToken))

		'2. Get ivc Flight
		Dim oIVCFlight As ivci.Flight.SearchResponse.Flight = Me.iVectorConnectResults(oItem.Index)

		'3. Generate Flight
		Dim oFlight As FlightResultHandler.Flight = Me.GenerateFlight(oIVCFlight, oItem.Index)

		'4. Return Flight
		Return oFlight

	End Function


	'Get Filter XML
	Public Function GetFilterXML() As XmlDocument

		Dim oXML As New XmlDocument

		Try
			oXML = Intuitive.Serializer.Serialize(Me.ResultsFilter)
		Catch ex As Exception
			oXML = New XmlDocument
		End Try

		Return oXML

	End Function


#End Region


#Region "Functions"

	'Clear Work Table
	Public Sub ClearWorkTable(Optional ClearIVCResults As Boolean = True)
		If ClearIVCResults Then
			Me.iVectorConnectResults.Clear()
		End If

		Me.WorkTable.Clear()
	End Sub


	'Remove From Work Table
	Public Sub RemoveWorkTableItem(ByVal sBookingToken As String)

		Try

			Dim oItemsToRemove As New Generic.List(Of WorkTableItem)

			For Each oItem As WorkTableItem In Me.WorkTable

				If oItem.BookingToken = sBookingToken Then
					oItemsToRemove.Add(oItem)
				End If
			Next

			For Each oItemToRemove As WorkTableItem In oItemsToRemove
				Me.WorkTable.Remove(oItemToRemove)
			Next

		Catch ex As Exception

		End Try

	End Sub

	Public Function GenerateFlight(ByVal oFlightResult As ivci.Flight.SearchResponse.Flight, Optional ByVal Index As Integer = 0) As FlightResultHandler.Flight

		'Get Flight Markups
		Dim aFlightMarkups As Generic.List(Of BookingBase.Markup) = Me.Markups.Where(Function(o) o.Component =
		   BookingBase.Markup.eComponentType.Flight AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop


		'create result flight
		Dim oFlight As New FlightResultHandler.Flight
		With oFlight
			.BookingToken = oFlightResult.BookingToken
			.ExactMatch = oFlightResult.ExactMatch
			.Key = Index 'names do not match because of legacy code, the key is the dictionary lookup on ivector connect result store property
			'index is just the loop counter from when we save

			'flight carrier
			.FlightCarrierID = oFlightResult.FlightCarrierID
			.FlightCarrier = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, oFlightResult.FlightCarrierID)
			.FlightCarrierLogo = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierLogo, oFlightResult.FlightCarrierID)
			.FlightCarrierType = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierType, oFlightResult.FlightCarrierID)



			'airports
			.DepartureAirportID = oFlightResult.DepartureAirportID
			.DepartureAirport = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportGroupAndAirport, oFlightResult.DepartureAirportID)
			.DepartureAirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResult.DepartureAirportID)
			.ArrivalAirportID = oFlightResult.ArrivalAirportID
			.ArrivalAirport = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResult.ArrivalAirportID)
			.ArrivalAirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResult.ArrivalAirportID)


			'outbound details
			.OutboundFlightCode = oFlightResult.OutboundFlightCode
			.OutboundDepartureDate = oFlightResult.OutboundDepartureDate
			.OutboundDepartureTime = oFlightResult.OutboundDepartureTime
			.OutboundArrivalDate = oFlightResult.OutboundArrivalDate
			.OutboundArrivalTime = oFlightResult.OutboundArrivalTime
			.OutboundNumberOfStops = oFlightResult.NumberOfOutboundStops
			.OutBoundDepartureTimeOfDay = Me.FlightTimeOfDay(oFlightResult.OutboundDepartureTime)
			.OutboundFlightClassID = oFlightResult.OutboundFlightClassID
			.OutboundFlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResult.OutboundFlightClassID)

			'return details
			.ReturnFlightCode = oFlightResult.ReturnFlightCode
			.ReturnDepartureDate = oFlightResult.ReturnDepartureDate
			.ReturnDepartureTime = oFlightResult.ReturnDepartureTime
			.ReturnArrivalDate = oFlightResult.ReturnArrivalDate
			.ReturnArrivalTime = oFlightResult.ReturnArrivalTime
			.ReturnNumberOfStops = oFlightResult.NumberOfReturnStops
			.ReturnDepartureTimeOfDay = Me.FlightTimeOfDay(oFlightResult.ReturnDepartureTime)
			.ReturnFlightClassID = oFlightResult.ReturnFlightClassID
			.ReturnFlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResult.ReturnFlightClassID)

			'baggage
			.IncludesSupplierBaggage = oFlightResult.IncludesSupplierBaggage


			'total price
			.Total = oFlightResult.TotalPrice
			.TotalBaggagePrice = oFlightResult.TotalBaggagePrice
			.TotalSeatPrice = oFlightResult.TotalSeatPrice
			.TotalCommission = oFlightResult.TotalCommission


            .Fare = Math.Round(oFlightResult.TotalPrice - oFlightResult.TotalTaxes,2)
            .TotalTaxes = oFlightResult.TotalTaxes

			'payment info
			If oFlightResult.TicketingDeadline.AddDays(-BookingBase.Params.TicketingOffset).Date > Date.Today() Then
				.PaymentFlag = ePaymentFlags.DepositAvailable.ToString()
				.FinalPaymentDue = Format(oFlightResult.TicketingDeadline.AddDays(-BookingBase.Params.TicketingOffset), "dd/MM/yyyy")
			Else
				.PaymentFlag = ePaymentFlags.InstantPurchase.ToString()
                .FinalPaymentDue = Format(Date.Today(),"dd/MM/yyyy") 
            End If

			'supplier and costs
			If Not oFlightResult.SupplierDetails Is Nothing Then
				.SupplierID = oFlightResult.SupplierDetails.SupplierID
				.LocalCost = oFlightResult.SupplierDetails.Cost
				.LocalCostCurrencyID = oFlightResult.SupplierDetails.CurrencyID
			End If


			Dim oFlightSectors As New List(Of FlightResultHandler.FlightSector)
			For Each oFlightResultSector As ivci.Support.FlightSector In oFlightResult.FlightSectors.OrderBy(Function(o) o.Seq).ThenBy(Function(o) o.Direction)
				Dim oFlightSector As New FlightResultHandler.FlightSector

				With oFlightSector
					.ArrivalAirportID = oFlightResultSector.ArrivalAirportID
					.ArrivalAirport = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResultSector.ArrivalAirportID)
					.ArrivalAirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResultSector.ArrivalAirportID)
					.ArrivalDate = oFlightResultSector.ArrivalDate
					.ArrivalTime = oFlightResultSector.ArrivalTime
					.DepartureAirportID = oFlightResultSector.DepartureAirportID
					.DepartureAirport = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oFlightResultSector.DepartureAirportID)
					.DepartureAirportCode = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.AirportIATACode, oFlightResultSector.DepartureAirportID)
					.DepartureDate = oFlightResultSector.DepartureDate
					.DepartureTime = oFlightResultSector.DepartureTime
					.Direction = oFlightResultSector.Direction
					.FlightCarrierID = oFlightResultSector.FlightCarrierID
					.FlightCarrier = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrier, oFlightResultSector.FlightCarrierID)
					.FlightCarrierLogo = Me.Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightCarrierLogo, oFlightResultSector.FlightCarrierID)
					.FlightCode = oFlightResultSector.FlightCode
					.FlightClassID = oFlightResultSector.FlightClassID
					.FlightClass = Lookups.GetKeyPairValue(Lookups.LookupTypes.FlightClass, oFlightResultSector.FlightClassID)
					.NumberOfStops = Intuitive.Functions.SafeInt(oFlightResultSector.NumberOfStops)
					.FlightTime = Intuitive.Functions.SafeInt(oFlightResultSector.FlightTime)
					.FriendlyFlightTime = Me.FriendlyTime(oFlightResultSector.FlightTime)
					.TravelTime = Intuitive.Functions.SafeInt(oFlightResultSector.TravelTime)
					.FriendlyTravelTime = Me.FriendlyTime(oFlightResultSector.TravelTime)
					.VehicleName = Lookups.GetKeyPairValue(Lookups.LookupTypes.Vehicle, Intuitive.Functions.SafeInt(oFlightResultSector.VehicleID))
					.Seq = oFlightResultSector.Seq
				End With

				oFlightSectors.Add(oFlightSector)

			Next

			.FlightSectors = oFlightSectors

		End With



		'create flight option
		Dim oFlightOption As New BookingFlight.BasketFlight.FlightOption
		With oFlightOption
			.BookingToken = oFlight.BookingToken
			.Price = oFlight.Total

			If Not oFlightResult.SupplierDetails Is Nothing Then
				.SupplierID = oFlightResult.SupplierDetails.SupplierID
				.LocalCost = oFlightResult.SupplierDetails.Cost
				.CurrencyID = oFlightResult.SupplierDetails.CurrencyID
			End If

			If Not oFlightResult.MultiCarrierDetails Is Nothing Then
				.ReturnMultiCarrierDetails.BookingToken = oFlightResult.MultiCarrierDetails.BookingToken
				.ReturnMultiCarrierDetails.Price = 0
			End If

			.OutboundFlightCode = oFlight.OutboundFlightCode
			.ReturnFlightCode = oFlight.ReturnFlightCode
			.OutboundDepartureDate = oFlight.OutboundDepartureDate
			.OutboundDepartureTime = oFlight.OutboundDepartureTime
			.OutboundArrivalDate = oFlight.OutboundArrivalDate
			.OutboundArrivalTime = oFlight.OutboundArrivalTime
			.ReturnDepartureDate = oFlight.ReturnDepartureDate
			.ReturnDepartureTime = oFlight.ReturnDepartureTime
			.ReturnArrivalDate = oFlight.ReturnArrivalDate
			.ReturnArrivalTime = oFlight.ReturnArrivalTime
			.DepartureAirportID = oFlight.DepartureAirportID
			.ArrivalAirportID = oFlight.ArrivalAirportID
			.FlightCarrierID = oFlight.FlightCarrierID
			.FlightCarrier = oFlight.FlightCarrier
			.FlightCarrierType = oFlight.FlightCarrierType
			.Adults = Me.SearchDetails.TotalAdults
			.Children = Me.SearchDetails.TotalChildren
			.Infants = Me.SearchDetails.TotalInfants
			.FlightPlusHotel = Me.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
				OrElse Me.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere
		End With


		'set hash token
		oFlight.FlightOptionHashToken = oFlightOption.GenerateHashToken()


		'add flight to results
		Return oFlight

	End Function


	Public Function FriendlyTime(Minutes As Integer) As String

		Try
			Dim ts As TimeSpan = TimeSpan.FromMinutes(Minutes)

			Dim sTime As String = ts.ToString("hh\:mm")

			Dim iHours As Integer = SafeInt(sTime.Split(":"c)(0)) + (ts.Days * 24)
			Dim iMinutes As Integer = SafeInt(sTime.Split(":"c)(1))
			Dim sStringFormat As String = ""



			If iHours > 0 Then
				sStringFormat += "{0} hour"
				If iHours > 1 Then
					sStringFormat += "s"
				End If
				If iMinutes > 0 Then
					sStringFormat += ", "
				End If
			End If

			If iMinutes > 0 Then
				sStringFormat += "{1} minute"
				If iMinutes > 1 Then
					sStringFormat += "s"
				End If
			End If

			Dim sTranslatedStringFormat As String = Translation.GetCustomTranslation("Search Tool", sStringFormat)

			Dim sFriendlyTime As String = String.Format(sTranslatedStringFormat, iHours, iMinutes)

			Return sFriendlyTime


		Catch ex As Exception
			Return ""
		End Try

	End Function

	Public Function FlightTimeOfDay(ByVal FlightTime As String) As eTimeOfDay

		If DateFunctions.InTimeRange(FlightTime, "06:00", "11:59") Then
			Return eTimeOfDay.Morning
		ElseIf DateFunctions.InTimeRange(FlightTime, "12:00", "17:59") Then
			Return eTimeOfDay.Afternoon
		ElseIf DateFunctions.InTimeRange(FlightTime, "18:00", "21:59") Then
			Return eTimeOfDay.Evening
		Else
			Return eTimeOfDay.Night
		End If

	End Function

	Public Function FlightTimeOfDaySequence(ByVal TimeOfDay As eTimeOfDay) As Integer

		If TimeOfDay = eTimeOfDay.Morning Then
			Return 1
		ElseIf TimeOfDay = eTimeOfDay.Afternoon Then
			Return 2
		ElseIf TimeOfDay = eTimeOfDay.Evening Then
			Return 3
		ElseIf TimeOfDay = eTimeOfDay.Night Then
			Return 4
		Else
			Return 5
		End If

	End Function


#End Region


#Region "Classes"

#Region "Work Table Item"

	Public Class WorkTableItem

		Public Index As Integer
		Public BookingToken As String
		Public Display As Boolean = True
		Public SortValue As String = ""

		Public Price As Decimal
		Public BaggagePrice As Decimal
		Public OutboundDepartureDate As Date
		Public OutboundArrivalDate As Date
		Public OutboundDepartureTime As String
		Public OutboundDepartureTimeOfDay As eTimeOfDay
		Public ReturnDepartureTime As String
		Public ReturnDepartureTimeOfDay As eTimeOfDay
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public MaxStops As Integer
		Public NumberOfOutboundStops As Integer
		Public NumberOfReturnStops As Integer
		Public ExactMatch As Boolean
		Public ArrivalAirportID As Integer
		Public DepartureAirportID As Integer
		Public OutboundFlightClassID As Integer
		Public OutboundFlightCode As String
		Public MultiCarrier As Boolean
		Public ReturnSource As String
		Public ReturnFlightSupplierID As Integer
		Public ReturnTPSessionID As String
		Public ReturnBookingToken As String
		Public ReturnFlightCarrierID As Integer
		Public OutboundFlightDuration As Integer
		Public ReturnFlightDuration As Integer

		Public Sub New(ByVal Index As Integer, ByVal BookingToken As String, ByVal Price As Decimal, ByVal OutboundDepartureTime As String,
		  ByVal ReturnDepartureTime As String, ByVal FlightCarrierID As Integer, ByVal NumberOfOutboundStops As Integer, ByVal NumberOfReturnStops As Integer,
		  OutboundDepartureDate As Date, ExactMatch As Boolean, ArrivalAirportID As Integer, DepartureAirportID As Integer, OutboundFlightClassID As Integer,
		  OutboundArrivalDate As Date, OutboundFlightCode As String, OutboundDepartureTimeOfDay As eTimeOfDay,
		  ReturnDepartureTimeOfDay As eTimeOfDay, FlightCarrier As String, BaggagePrice As Decimal, MultiCarrier As Boolean,
		  ReturnBookingToken As String, ReturnSource As String, ReturnFlightSupplierID As Integer, ReturnTPSessionID As String,
		  ReturnFlightCarrierID As Integer, OutboundFlightDuration As Integer, ReturnFlightDuration As Integer)
			Me.Index = Index
			Me.BookingToken = BookingToken
			Me.Price = Price
			Me.OutboundDepartureTime = OutboundDepartureTime
			Me.OutboundDepartureTimeOfDay = OutboundDepartureTimeOfDay
			Me.ReturnDepartureTime = ReturnDepartureTime
			Me.ReturnDepartureTimeOfDay = ReturnDepartureTimeOfDay
			Me.FlightCarrierID = FlightCarrierID
			Me.FlightCarrier = FlightCarrier
			Me.MaxStops = Math.Max(NumberOfOutboundStops, NumberOfReturnStops)
			Me.NumberOfOutboundStops = NumberOfOutboundStops
			Me.NumberOfReturnStops = NumberOfReturnStops
			Me.OutboundDepartureDate = OutboundDepartureDate
			Me.ExactMatch = ExactMatch
			Me.ArrivalAirportID = ArrivalAirportID
			Me.DepartureAirportID = DepartureAirportID
			Me.OutboundFlightClassID = OutboundFlightClassID
			Me.OutboundArrivalDate = OutboundArrivalDate
			Me.OutboundFlightCode = OutboundFlightCode
			Me.BaggagePrice = BaggagePrice
			Me.MultiCarrier = MultiCarrier
			Me.ReturnSource = ReturnSource
			Me.ReturnFlightSupplierID = ReturnFlightSupplierID
			Me.ReturnTPSessionID = ReturnTPSessionID
			Me.ReturnBookingToken = ReturnBookingToken
			Me.ReturnFlightCarrierID = ReturnFlightCarrierID
			Me.OutboundFlightDuration = OutboundFlightDuration
			Me.ReturnFlightDuration = ReturnFlightDuration
		End Sub


	End Class

#End Region

#Region "Filter Class"

	Public Class Filters

		'outbound departure date
		Public OutboundDepartureDate As Date

		'filter settings
		Public MinPrice As Decimal = 0
		Public MaxPrice As Decimal = 0
		Public FilterDepartureAirportIDs As New Generic.List(Of Integer)
		Public FilterArrivalAirportIDs As New Generic.List(Of Integer)
		Public FilterFlightCarrierIDs As New Generic.List(Of Integer)
		Public FilterDepartureTimes As New Generic.List(Of eTimeOfDay)
		Public FilterReturnTimes As New Generic.List(Of eTimeOfDay)
		Public FilterStops As New Generic.List(Of Integer)
		Public FilterFlightClassIDs As New Generic.List(Of Integer)
		Public BookingTokenCSV As String = ""
		Public MinOutboundDepartureFlightTimeMinutes As Integer
		Public MaxOutboundDepartureFlightTimeMinutes As Integer = 1439
		Public MinReturnDepartureFlightTimeMinutes As Integer
		Public MaxReturnDepartureFlightTimeMinutes As Integer = 1439
		Public MinFlightDurationMinutes As Integer
		Public MaxFlightDurationMinutes As Integer
		Public IncludesSupplierBaggage As Boolean

		'filter counts and from prices
		'times
		Public DepartureTimes As New Generic.List(Of FlightTime)
		Public ReturnTimes As New Generic.List(Of FlightTime)
		Public MinOutboundDepartureFlightTime As String
		Public MaxOutboundDepartureFlightTime As String
		Public MinReturnDepartureFlightTime As String
		Public MaxReturnDepartureFlightTime As String

		'airports
		Public DepartureAirports As New Generic.List(Of Airport)
		Public ArrivalAirports As New Generic.List(Of Airport)

		'carriers
		Public FlightCarriers As New Generic.List(Of FlightCarrier)

		'stops - will be max stops from either outbound or return leg
		Public Stops As New Generic.List(Of FlightStop)

		'Flight classes
		Public FlightClasses As New Generic.List(Of FlightClass)

		Public Class FlightStop
			Public Stops As Integer
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class FlightTime
			Public TimeOfDay As eTimeOfDay
			Public Count As Integer
			Public FromPrice As Decimal
			Public Sequence As Integer
		End Class


		Public Class FlightCarrier
			Public FlightCarrierID As Integer
			Public FlightCarrier As String
			Public FlightCarrierLogo As String
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class Airport
			Public AirportID As Integer
			Public AirportName As String
			Public AirportCode As String
			Public Count As Integer
			Public Selected As Boolean
		End Class

		Public Class FlightClass
			Public FlightClassID As Integer
			Public FlightClass As String
			Public Count As Integer
		End Class

		Public Sub Clear()
			Me.DepartureTimes.Clear()
			Me.ReturnTimes.Clear()
			Me.FlightCarriers.Clear()
			Me.Stops.Clear()
			Me.FlightClasses.Clear()
			Me.DepartureAirports.Clear()
			Me.ArrivalAirports.Clear()
		End Sub

	End Class

#End Region

#Region "Sorting"

	Public Class Sort

		Public SortBy As eSortBy = eSortBy.Price
		Public SortOrder As eSortOrder = eSortOrder.Ascending

	End Class

#End Region

#Region "Support - Enums"

	Public Enum eSortBy
		Price
		DepartureTime
		Airline
		ReturnTime
	End Enum

	Public Enum eSortOrder
		Ascending
		Descending
	End Enum

	Enum eTimeOfDay
		Morning
		Afternoon
		Evening
		Night
	End Enum

#End Region

#Region "baggage and extras"
	Public Class BaggageOptions

		'Filters
		Public BaggageOptions As New Generic.List(Of BaggageOption)

		Public Class BaggageOption

			'Filters
			Public Token As String
			Public Quantity As Integer

		End Class

	End Class

	Public Class ExtraOptions

		'Filters
		Public ExtraOptions As New Generic.List(Of ExtraOption)

		Public Class ExtraOption

			'Filters
			Public Token As String
			Public Quantity As Integer

		End Class

	End Class

#End Region

#Region "Flight Class"

	Public Class Flight

		Public BookingToken As String
		Public FlightOptionHashToken As String
		Public ExactMatch As Boolean
		Public Key As Integer  'lookup identifier on ivector connect result store

		'flight carrier
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public FlightCarrierLogo As String
		Public FlightCarrierType As String

		'airports
		Public DepartureAirportID As Integer
		Public DepartureAirport As String
		Public DepartureAirportCode As String
		Public ArrivalAirportID As Integer
		Public ArrivalAirport As String
		Public ArrivalAirportCode As String

		'outbound flight
		Public OutboundFlightCode As String
		Public OutboundDepartureDate As Date
		Public Property OutboundDepartureShortDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundDepartureDate, "shortdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public Property OutboundDepartureMediumDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundDepartureDate, "mediumdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public OutboundDepartureTime As String
		Public OutboundArrivalDate As Date
		Public Property OutboundArrivalShortDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundArrivalDate, "shortdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public Property OutboundArrivalMediumDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundArrivalDate, "mediumdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public OutboundArrivalTime As String
		Public OutboundNumberOfStops As Integer
		Public OutboundFlightClassID As Integer
		Public OutboundFlightClass As String
		Public OutboundFlightDuration As String

		Public OutBoundDepartureTimeOfDay As eTimeOfDay

		'return flight
		Public ReturnFlightCode As String
		Public ReturnDepartureDate As Date
		Public Property ReturnDepartureShortDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnDepartureDate, "shortdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public Property ReturnDepartureMediumDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnDepartureDate, "mediumdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public ReturnDepartureTime As String
		Public ReturnArrivalDate As Date
		Public Property ReturnArrivalShortDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnArrivalDate, "shortdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public Property ReturnArrivalMediumDate As String
			Get
				Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnArrivalDate, "mediumdate")
			End Get
			Set(value As String)
			End Set
		End Property
		Public ReturnArrivalTime As String
		Public ReturnNumberOfStops As Integer
		Public ReturnFlightClassID As Integer
		Public ReturnFlightClass As String
		Public ReturnFlightDuration As String

		Public ReturnDepartureTimeOfDay As eTimeOfDay

		'baggage
		Public IncludesSupplierBaggage As Boolean

		'price
		Public MarkupAmount As Decimal
		Public MarkupPercentage As Decimal
		Public Total As Decimal
		Public TotalSeatPrice As Decimal
		Public TotalBaggagePrice As Decimal
		Public TotalCommission As Decimal
        Public Fare as Decimal
        Public TotalTaxes as Decimal
        Public PaymentFlag As String
        Public FinalPaymentDue as String

		'Supplier Details
		Public SupplierID As Integer
		Public LocalCost As Decimal
		Public LocalCostCurrencyID As Integer

		'Baggage
		Public ChangeDefaultBaggage As Boolean = False
		Public BaggagePriceBasis As eBaggagePriceBasis
		Public DefaultBaggageCost As Decimal
		Public BaggageCost As Decimal
		Public BaggageQuantity As Integer

		'flight sectors
		Public FlightSectors As New List(Of FlightSector)

		'MaxStops
		Public ReadOnly Property MaxStops As Integer
			Get
				Return Math.Max(Me.OutboundNumberOfStops, Me.ReturnNumberOfStops)
			End Get
		End Property


	End Class

	Public Class FlightSector
		Public ArrivalAirportID As Integer
		Public ArrivalAirport As String
		Public ArrivalAirportCode As String
		Public ArrivalDate As Date
		Public ArrivalTime As String
		Public DepartureAirportID As Integer
		Public DepartureAirport As String
		Public DepartureAirportCode As String
		Public DepartureDate As Date
		Public DepartureTime As String
		Public Direction As String
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public FlightCarrierLogo As String
		Public FlightCarrierWebDescription As String
		Public FlightCode As String
		Public LowcostFlightCarrier As Boolean
		Public FlightClassID As Integer
		Public FlightClass As String
		Public NumberOfStops As Integer
		Public FlightTime As Integer
		Public FriendlyFlightTime As String
		Public TravelTime As Integer
		Public FriendlyTravelTime As String
		Public VehicleName As String
		Public Seq As Integer
	End Class

	Public Enum eBaggagePriceBasis
		None
		PerBag
		PerBooking
		Inclusive
	End Enum


    Public Enum ePaymentFlags
        InstantPurchase
        DepositAvailable
    End Enum

#End Region


#End Region

End Class

Imports System.Web.UI.WebControls.Expressions
Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports iVectorConnectInterface.Property.SearchResponse
Imports Intuitive.Web

Public Class PropertyResultHandler

#Region "Properties"

	Private Property iVectorConnectResultStore As New Dictionary(Of Integer, Booking.Property.PropertyResult)
	Public WorkTable As New Generic.List(Of WorkTableItem)
	Public ResultsFilter As New Filters
	Public ResultsSort As New Sort
	Public RequestDiagnostic As New RequestDiagnostic

	'Dictionaries
	Public MealBasisDictionary As New Dictionary(Of String, String)
	Public FlightDictionary As New Dictionary(Of Integer, FlightResultHandler.Flight)
	Public TransferDictionary As New Dictionary(Of Integer, BookingTransfer.Results.Transfer)
	Public CarHireDictionary As New Dictionary(Of String, BookingCarHire.CarHireResults.CarHire)
	Public HotelFlightDictionary As New Dictionary(Of Integer, FlightResultHandler.Flight)

	Public ChangeFlightProperties As New List(Of Integer)

	Private UniqueResorts As New List(Of Integer)

	Public LogValue As New LogValues

	Public ReadOnly Property TotalHotels As Integer
		Get
			Dim iTotal As Integer = Me.WorkTable.Where(Function(o) o.Display = True).Count
			Return iTotal
		End Get
	End Property


	Public ReadOnly Property GeocodedHotels As Boolean
		Get
			For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable

				If oWorkTableItem.Display Then

					Dim oIVCResult As ivci.Property.SearchResponse.PropertyResult = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults(oWorkTableItem.Index)

					Dim nLatitude As Decimal = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oIVCResult.SearchResponseXML, "Property/Latitude"))
					Dim nLongitude As Decimal = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oIVCResult.SearchResponseXML, "Property/Longitude"))

					If nLatitude <> 0 AndAlso nLongitude <> 0 Then
						Return True
					End If

				End If

			Next

			Return False
		End Get
	End Property

	Public CurrentPage As Integer = 1

	Public ReadOnly Property TotalPages As Integer
		Get
			Return Functions.SafeInt(Math.Ceiling(Me.TotalHotels / BookingBase.Params.HotelResultsPerPage))
		End Get
	End Property


	Public ReadOnly Property MinPrice As Decimal
		Get
			If Me.WorkTable.Where(Function(o) o.Display = True).Count > 0 Then
				Dim nPrice As Decimal = Me.WorkTable.Where(Function(o) o.Display = True).Min(Function(o) o.MinPrice)
				Return nPrice
			Else
				Return 0
			End If
		End Get
	End Property

	Public ReadOnly Property MaxPrice As Decimal
		Get
			If Me.WorkTable.Where(Function(o) o.Display = True).Count > 0 Then
				Dim nPrice As Decimal = Me.WorkTable.Where(Function(o) o.Display = True).Max(Function(o) o.MaxPrice)
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
	<Obsolete("Use the method that accepts PropertyResult class")>
	Private Sub SaveiVectorConnectResult(Key As Integer, ByVal iVectorConnectResult As ivci.Property.SearchResponse.PropertyResult)
		Me.iVectorConnectResultStore(Key) = DirectCast(iVectorConnectResult, Booking.Property.PropertyResult)
	End Sub

	''' <summary>
	''' Adds the results to our internal store to access later
	''' </summary>
	''' <param name="Key">Unique integer key for the result</param>
	''' <param name="PropertyResult">property result</param>
	''' <remarks>Should only be being called in the save routine, which loops. So the key should always be unique. Overwrite if it isn't though to be safe</remarks>
	Private Sub SaveiVectorConnectResult(Key As Integer, ByVal PropertyResult As Booking.Property.PropertyResult)
		Me.iVectorConnectResultStore(Key) = PropertyResult
	End Sub

	Public Function iVectorConnectResults(Key As Integer) As Booking.Property.PropertyResult
		Return Me.iVectorConnectResultStore(Key)
	End Function

	Public Function iVectorConnectResults() As Generic.List(Of Booking.Property.PropertyResult)
		Return Me.iVectorConnectResultStore.Values.ToList
	End Function
#End Region

#Region "Save"

	'Save
	Public Sub Save(ByVal iVectorConnectResults As Generic.List(Of Booking.Property.PropertyResult),
	 Optional ClearIVCResults As Boolean = True,
	 Optional RequestInfo As BookingSearch.RequestInfo = Nothing)

		Dim oFactory As New PropertyResultsFactory
		Dim oIVCResults As List(Of ivci.Property.SearchResponse.PropertyResult) = oFactory.CreateIVCList(iVectorConnectResults)

		Save(oIVCResults, ClearIVCResults, RequestInfo)

	End Sub

	'Save
	Public Sub Save(ByVal iVectorConnectResults As Generic.List(Of ivci.Property.SearchResponse.PropertyResult),
	 Optional ClearIVCResults As Boolean = True,
	 Optional RequestInfo As BookingSearch.RequestInfo = Nothing,
	 Optional LogValues As LogValues = Nothing)

		'1. Clear Work Table
		Me.ClearWorkTable(ClearIVCResults)


		'2. set up diagnostic
		If Not RequestInfo Is Nothing Then
			Me.RequestDiagnostic = RequestInfo.ConvertToDiagnostic()
		End If

		If Not LogValues Is Nothing Then
			Me.LogValue = LogValues
		End If


		'3. Get Property Markups
		Dim aPropertyMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Property AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

		'reset the markup
		Me.MarkupAmount = 0
		Me.MarkupPercentage = 0

		'update with new markup
		For Each oMarkup As BookingBase.Markup In aPropertyMarkups
			Select Case oMarkup.Type
				Case BookingBase.Markup.eType.Amount
					Me.MarkupAmount += oMarkup.Value
				Case BookingBase.Markup.eType.AmountPP
					Me.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
				Case BookingBase.Markup.eType.Percentage
					Me.MarkupPercentage = oMarkup.Value
			End Select
		Next

		Dim oPropertyResultsFactory As New PropertyResultsFactory

		'4. populate the work table with one work item per property
		Dim iIndex As Integer = 0
		For Each oPropertyResult As ivci.Property.SearchResponse.PropertyResult In iVectorConnectResults

			Me.SaveiVectorConnectResult(iIndex, oPropertyResultsFactory.Create(oPropertyResult))

			'4a. Create ResultIndex and set index
			Dim oItem As New WorkTableItem(iIndex, oPropertyResult.PropertyReferenceID, oPropertyResult.GeographyLevel3ID, oPropertyResult.GeographyLevel2ID)

			'4b. Add ResultIndex to WorkTable
			If Not Me.WorkTable.Exists(Function(o) o.PropertyReferenceID = oItem.PropertyReferenceID) Then
				Me.WorkTable.Add(oItem)
			End If

			'4c add to unique resort list
			If Not Me.UniqueResorts.Contains(oPropertyResult.GeographyLevel3ID) Then
				Me.UniqueResorts.Add(oPropertyResult.GeographyLevel3ID)
			End If

			iIndex += 1

			'4d Add Markup
			For Each oRoom As ivci.Property.SearchResponse.RoomType In oPropertyResult.RoomTypes
				oRoom.Total += Me.MarkupAmount
				oRoom.Total *= (Me.MarkupPercentage / 100) + 1
			Next

			For Each oGroup As ivci.Property.SearchResponse.RoomGroup In oPropertyResult.RoomGroups
				For Each oMB As RoomGroupMealBasis In oGroup.RoomGroupMealBases
					oMB.LeadInPrice += Me.MarkupAmount
					oMB.LeadInPrice *= (Me.MarkupPercentage / 100) + 1
				Next
			Next

		Next



		'5. Store Result Handler On SearchDetails (which is also in the session)
		BookingBase.SearchDetails.PropertyResults = Me

		'6. Generate landmark filter
		Me.GenerateLandmarkFilter()

	End Sub

#End Region


#Region "Get"

	'Get Results XML
	Public Function GetResultsXML(ByVal Page As Integer) As XmlDocument

		'1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexes As Generic.List(Of IVectorConnectResultsIndex) = Me.GetRange(Page)


		'2. List of PropertyResult to return as XML
		Dim oIVectorConnectResults As New Generic.List(Of ivci.Property.SearchResponse.PropertyResult)
		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexes
			oIVectorConnectResults.Add(oIVectorConnectResultsIndex.PropertyResult)
		Next


		'3. Serialize oIVectorConnectResults into XML
		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oIVectorConnectResults)


		'4. Return XML
		Return oXML

	End Function


	'Get Range
	Public Function GetRange(ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. List Of iVectorConnectResultIndex
		Dim oIVectorConnectResultsIndexs As New Generic.List(Of IVectorConnectResultsIndex)

		'2. Get List of WorkTableItems
		Dim oWorkTable As Generic.List(Of WorkTableItem) = Me.GetWorkTable(Page)

		'3. Add PropertyResult to oIVectorConnectResultsIndex from iVectorConnectResults using index
		For Each oItem As WorkTableItem In oWorkTable
			Dim oIVectorConnectResultsIndex As New IVectorConnectResultsIndex
			oIVectorConnectResultsIndex.Index = oItem.Index
			oIVectorConnectResultsIndex.PropertyResult = (Me.iVectorConnectResults(oItem.Index))
			oIVectorConnectResultsIndexs.Add(oIVectorConnectResultsIndex)
		Next

		'4. Return
		Return oIVectorConnectResultsIndexs

	End Function


	'Get Results as XML
	Public Function GetResultsAsHotelXML(ByVal Page As Integer) As XmlDocument


		'2. List of Hotels to return as XML
		Dim oResults As New Results
		oResults.Hotels = Me.GetHotelsPage(Page)
		oResults.Filters = Me.ResultsFilter

		oResults.MinPrice = Me.MinPrice
		oResults.MaxPrice = Me.MaxPrice

		oResults.LogValues = Me.LogValue

		'serialize to xml and return
		Dim oHotelsXML As XmlDocument = Serializer.Serialize(oResults, True)
		Return oHotelsXML

	End Function

	Public Class Results
		Public MinPrice As Decimal
		Public MaxPrice As Decimal
		Public Hotels As New Generic.List(Of Hotel)
		Public Filters As Filters
		Public LogValues As LogValues
	End Class


	'Get Work Table Item - Get Range of work table items using Page to generate a range
	Public Function GetWorkTable(ByVal Page As Integer) As Generic.List(Of WorkTableItem)

		Dim oWorkTable As New Generic.List(Of WorkTableItem)

		Try

			'1. Select WorkItems where Display = true
			oWorkTable = Me.WorkTable.Where(Function(o) o.Display = True).ToList

			'2. Set range indexes
			Dim iStartIndex As Integer = (Page - 1) * BookingBase.Params.HotelResultsPerPage
			Dim iCount As Integer = Functions.IIf(oWorkTable.Count < BookingBase.Params.HotelResultsPerPage, oWorkTable.Count, BookingBase.Params.HotelResultsPerPage)
			iCount = Functions.IIf(iStartIndex + iCount > oWorkTable.Count, oWorkTable.Count - iStartIndex, iCount)

			'3. Select WorkTableItems within range
			oWorkTable = oWorkTable.GetRange(iStartIndex, iCount)

		Catch ex As Exception
			oWorkTable = New Generic.List(Of WorkTableItem)
		End Try

		'3. Return WorkTable
		Return oWorkTable

	End Function


	'Get Single Hotel
	Public Function GetSinglePropertyXML(ByVal iIndex As Integer) As XmlDocument

		'1. Get Single Hotel
		Dim oProperty As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(iIndex)


		'2. Generate Hotel
		Dim oHotel As Hotel = Me.GenerateHotel(oProperty, iIndex)

		'3. Generate XML
		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oHotel, True)

		'4. Return XML
		Return oXML

	End Function


	'Get Single Hotel
	Public Function GetSingleHotel(ByVal iPropertyReferenceID As Integer, Optional ByVal iIndex As Integer = 0) As Hotel

		Dim oHotel As New Hotel

		Try
			If iIndex = 0 Then
				For Each oItem As WorkTableItem In Me.WorkTable
					If oItem.PropertyReferenceID = iPropertyReferenceID Then
						Dim oProperty As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oItem.Index)
						oHotel = Me.GenerateHotel(oProperty, oItem.Index)
						Exit For
					End If
				Next
			Else
				Dim oWorkTableItem As WorkTableItem = Me.WorkTable(iIndex)
				Dim oProperty As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oWorkTableItem.Index)
				oHotel = Me.GenerateHotel(oProperty, oWorkTableItem.Index)
			End If
		Catch ex As Exception
			'in case indexes do not exist
		End Try

		Return oHotel

	End Function


	'Get Hotels Page
	Public Function GetHotelsPage(ByVal Page As Integer) As Generic.List(Of Hotel)

		'1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexs As Generic.List(Of IVectorConnectResultsIndex) = GetRange(Page)

		Dim oHotels As New Generic.List(Of Hotel)

		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexs
			'add hotel to results
			Dim oHotel As Hotel = Me.GenerateHotel(oIVectorConnectResultsIndex.PropertyResult, oIVectorConnectResultsIndex.Index)
			If Not oHotel Is Nothing Then
				oHotels.Add(oHotel)
			End If
		Next

		Return oHotels

	End Function

#End Region


#Region "Sort"

	'Sort Results (string)
	Public Sub SortResults(ByVal SortBy As String, ByVal SortOrder As String, Optional ByVal PriorityPropertyID As Integer = 0)

		'get enum values
		Dim eSortBy As eSortBy = Intuitive.Functions.SafeEnum(Of PropertyResultHandler.eSortBy)(SortBy)
		Dim eSortOrder As eSortOrder = Intuitive.Functions.SafeEnum(Of PropertyResultHandler.eSortOrder)(SortOrder)

		'sort results
		Me.SortResults(eSortBy, eSortOrder, PriorityPropertyID)

	End Sub


	'Sort Results (enum)
	Public Sub SortResults(ByVal SortBy As eSortBy, ByVal SortOrder As eSortOrder,
	  Optional ByVal PriorityPropertyID As Integer = 0)

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Sort property results", ProcessTimer.MainProcess)

		Me.ResultsSort.SortBy = SortBy
		Me.ResultsSort.SortOrder = SortOrder

		Dim sSortXPath As String = Me.ResultsSort.SortXPath(SortBy)

		'1. Set Sort Value For each ResultIndex in WorkTable
		If Not SortBy = eSortBy.Custom Then
			For Each oItem As WorkTableItem In Me.WorkTable

				If oItem.Display Then

					'1.a. Select Property Using Index
					Dim oPropertyResult As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oItem.Index)

					'1.b. Set SortValue
					Dim sSortValue As String = ""
					If Not sSortXPath = "" Then
						sSortValue = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, sSortXPath)
					End If
					oItem.SortValue = sSortValue.ToLower

					'1.c. Set Min Price
					If oItem.MinPrice = 0 Then
						Dim nPrice As Decimal = oPropertyResult.RoomTypes.OrderBy(Function(o) o.Total)(0).Total
						oItem.MinPrice = nPrice
					End If

					'1.d. Set Sort Price
					oItem.SortPrice = oItem.MinPrice
					If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
						Dim oSelectedFlight As FlightResultHandler.Flight = Me.HotelFlightDictionary(oPropertyResult.PropertyReferenceID)
						oItem.SortPrice += oSelectedFlight.Total
					End If
				Else
					oItem.SortValue = ""
				End If

			Next
		End If


		'2. Order WorkTable
		If SortBy = eSortBy.Custom Then
			If SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.SortPrice).OrderBy(Function(o) o.CustomSortValue).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.SortPrice).OrderByDescending(Function(o) o.CustomSortValue).ToList
			End If
		ElseIf sSortXPath = "" Then
			If SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.SortPrice).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderByDescending(Function(o) o.SortPrice).ToList
			End If
		Else
			If SortOrder = eSortOrder.Ascending Then
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.SortPrice).OrderBy(Function(o) o.SortValue).ToList
			Else
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.SortPrice).OrderByDescending(Function(o) o.SortValue).ToList
			End If

		End If


		'3. Priority property
		If PriorityPropertyID > 0 Then
			If Me.WorkTable.Where(Function(o) o.PropertyReferenceID = PriorityPropertyID).Count > 0 Then
				Dim oWorkTableItem As WorkTableItem = Me.WorkTable.Where(Function(o) o.PropertyReferenceID = PriorityPropertyID)(0)
				Me.WorkTable.Remove(oWorkTableItem)
				Me.WorkTable.Insert(0, oWorkTableItem)
			End If
		End If

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Sort property results", ProcessTimer.MainProcess)

	End Sub

	Public Function GetSortXML() As XmlDocument

		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(Me.ResultsSort)

		Return oXML

	End Function

#End Region


#Region "Filter"

	'Filter Results
	Public Sub FilterResults(ByVal Filter As Filters, Optional ByVal ResetCustomFilter As Boolean = True)

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Filter property results", ProcessTimer.MainProcess)

		'1. Store filter as class property
		Me.ResultsFilter = Filter


		'2. if reset custom filter set custom filter display to true
		If ResetCustomFilter Then
			For Each oItem As WorkTableItem In Me.WorkTable
				oItem.CustomFilterDisplay = True
			Next
		End If


		'3. . Generate Filter Counts
		Me.GenerateFilterCounts()


		'4. Generate Price Band Filter
		Me.GeneratePriceBandFilter()


		'5. Set Display value for each work item
		For Each oItem As WorkTableItem In Me.WorkTable

			'5.a Select Property using Item index
			Dim ivcPropertyResult As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oItem.Index)

			'5.b Filter PropertyResult
			Me.FilterItem(ivcPropertyResult, oItem)

		Next


		'6. Re-sort Results as displayed hotels have changed
		Me.SortResults(Me.ResultsSort.SortBy, Me.ResultsSort.SortOrder, BookingBase.SearchDetails.PriorityPropertyID)

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Filter property results", ProcessTimer.MainProcess)

	End Sub

	Public Sub ApplyDefaultFilters(BookingSearch As BookingSearch, ByRef ResultsFilter As PropertyResultHandler.Filters)

		ResultsFilter.MinAverageScore = IIf(BookingSearch.RemoveLowRatedHotels AndAlso
											BookingBase.Params.HotelResults_PropertyMinimumScore >= 0,
											BookingBase.Params.HotelResults_PropertyMinimumScore, 0)

	End Sub

	'Filter Item
	Public Sub FilterItem(ByVal ivcPropertyResult As ivci.Property.SearchResponse.PropertyResult, ByVal oWorkTableItem As WorkTableItem,
	 Optional ByVal IgnoreRating As Boolean = False, Optional ByVal IgnoreTripAdvisorRating As Boolean = False,
	 Optional ByVal IgnoreResort As Boolean = False,
	 Optional ByVal IgnoreFacility As Boolean = False, Optional ByVal IgnoreMealBasis As Boolean = False,
	 Optional ByVal IgnoreProductAttribute As Boolean = False, Optional ByVal IgnorePrice As Boolean = False,
	 Optional ByVal IgnoreAverageReviewScore As Boolean = False, Optional ByVal IgnorePropertyType As Boolean = False,
	 Optional ByVal IgnoreRegion As Boolean = False)


		'1. Declare Filter Value Variables
		Dim bDisplay As Boolean = True
		Dim iRating As Decimal = 0
		Dim iTripAdvisorRating As Decimal = 0D
		Dim sName As String = ""
		Dim iGeographyLevel3ID As Integer = 0
		Dim iFacilityFlag As Integer = 0
		Dim iMealBasisIDs As New Generic.List(Of Integer)
		Dim iProductAttributeIDs As New Generic.List(Of Integer)
		Dim iHotelMinFromPrice As Decimal = 0
		Dim bBestSeller As Boolean = False
		Dim bFreeWifi As Boolean = False
		Dim iReviewAverageScore As Decimal = 0
		Dim nLatitude As Decimal = 0
		Dim nLongitude As Decimal = 0
		Dim iPropertyTypeID As Integer = 0
		Dim iGeographyLevel2ID As Integer = 0

		'1.1 if custom filter display set to false then set display false
		If Not oWorkTableItem.CustomFilterDisplay Then bDisplay = False


		'2. Check if property should still display using filters.
		'2.a. Rating
		iRating = Functions.IIf(oWorkTableItem.Rating > 0, oWorkTableItem.Rating, Math.Floor(Intuitive.Functions.SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.RatingXPath))))
		If bDisplay AndAlso Not IgnoreRating AndAlso Me.ResultsFilter.RatingCSV <> "" Then
			Dim aRatings As String() = Me.ResultsFilter.RatingCSV.Split(","c)
			If Not aRatings.Contains(iRating.ToString) Then
				bDisplay = False
			End If
		End If


		'2.b. Name
		sName = Functions.IIf(Not oWorkTableItem.Name = "", oWorkTableItem.Name, XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.NameXPath))
		If bDisplay AndAlso Me.ResultsFilter.Name <> "" Then
			If sName Is Nothing OrElse Not sName.ToLower.Contains(Me.ResultsFilter.Name.ToLower) Then
				bDisplay = False
			End If
		End If


		'2.c. Geography
		iGeographyLevel3ID = Functions.IIf(oWorkTableItem.GeographyLevel3ID > 0, oWorkTableItem.GeographyLevel3ID, ivcPropertyResult.GeographyLevel3ID)
		If bDisplay AndAlso Not IgnoreResort AndAlso Me.ResultsFilter.GeographyLevel3IDCSV <> "" Then
			Dim aGeographyLevel3s As String() = Me.ResultsFilter.GeographyLevel3IDCSV.Split(","c)
			If Not aGeographyLevel3s.Contains(iGeographyLevel3ID.ToString) Then
				bDisplay = False
			End If
		End If

		iGeographyLevel2ID = Functions.IIf(oWorkTableItem.GeographyLevel2ID > 0, oWorkTableItem.GeographyLevel2ID, ivcPropertyResult.GeographyLevel2ID)
		If bDisplay AndAlso Not IgnoreRegion AndAlso Me.ResultsFilter.GeographyLevel2IDCSV <> "" Then
			Dim aGeographyLevel2s As String() = Me.ResultsFilter.GeographyLevel2IDCSV.Split(","c)
			If Not aGeographyLevel2s.Contains(iGeographyLevel2ID.ToString) Then
				bDisplay = False
			End If
		End If


		'2.d. Facililty
		iFacilityFlag = Functions.IIf(oWorkTableItem.FacilityFlag > 0, oWorkTableItem.FacilityFlag, SafeInt(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.FacilityFilterIDXPath)))
		If bDisplay AndAlso Not IgnoreFacility AndAlso Me.ResultsFilter.FilterFacilityIDCSV <> "" Then

			Dim aFacilityIDs As String() = Me.ResultsFilter.FilterFacilityIDCSV.Split(","c)
			Dim iFacilitySum As Integer = 0

			For Each iFacility As Integer In aFacilityIDs

				Dim iFacilityID As Integer = iFacility
				'dont get the priority from ResultsFilter.FilterFacilities as this gets cleared down
				'instead look up the value from the lookups
				Dim iPriority As Integer = BookingBase.Lookups.FilterFacilities.Where(Function(o) o.FilterFacilityID = iFacilityID).First.Priority

				iPriority = Functions.SafeInt(2 ^ iPriority)
				iFacilitySum = iFacilitySum + iPriority
			Next

			If (iFacilityFlag And iFacilitySum) <> iFacilitySum Or iFacilitySum = 0 Then
				bDisplay = False
			End If
		End If



		'2.e. MealBasis and Price
		'2.e.i. Set Filter Variables
		Dim bShowProperty As Boolean = False
		Dim iPropertyMinPrice As Decimal = 0
		Dim iPropertyMaxPrice As Decimal = 0

		Dim iFilterMinPrice As Integer = Me.ResultsFilter.MinPrice
		Dim iFilterMaxPrice As Integer = Functions.IIf(Me.ResultsFilter.MaxPrice <= 0, 999999999, Me.ResultsFilter.MaxPrice)
		Dim aFilterMealBasisIDs As String() = Me.ResultsFilter.MealBasisIDCSV.Split(","c)
		Dim oRoomSequences As New Generic.List(Of Integer)

		'2.e.ii. Loop through RoomTypes
		Dim iRoomCount As Integer = 1
		For Each oRoomType As ivci.Property.SearchResponse.RoomType In ivcPropertyResult.RoomTypes.OrderBy(Function(o) o.Total)

			'2.e.iii. If 1 property is valid display hotel
			Dim bValidRoom As Boolean = False
			Dim nPrice As Decimal = oRoomType.Total

			'if showing package prices, add flight price to room total
			If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso BookingBase.Params.HotelFilterPackagePrices Then
				Dim oFlight As New FlightResultHandler.Flight
				If Me.FlightDictionary.ContainsKey(iGeographyLevel3ID) Then
					oFlight = Me.FlightDictionary(iGeographyLevel3ID)
					nPrice += oFlight.Total
				End If
			End If

			'2.e.iv. Check Meal basis and price
			If Not IgnoreMealBasis AndAlso Me.ResultsFilter.MealBasisIDCSV <> "" Then
				If aFilterMealBasisIDs.Contains(oRoomType.MealBasisID.ToString) AndAlso nPrice >= iFilterMinPrice AndAlso nPrice <= iFilterMaxPrice Then
					bValidRoom = True
				End If
			ElseIf IgnorePrice Then
				bValidRoom = True
			Else
				If nPrice >= iFilterMinPrice AndAlso nPrice <= iFilterMaxPrice Then
					bValidRoom = True
				End If
			End If

			If Not iMealBasisIDs.Contains(oRoomType.MealBasisID) Then iMealBasisIDs.Add(oRoomType.MealBasisID)

			'2.e.v If we are supressing onRequestRoomTypes and the property is on request it is not valid
			If BookingBase.Params.SuppressOnRequestRoomTypes AndAlso oRoomType.OnRequest Then
				bValidRoom = False
			End If

			'2.e.vi. If Property Is Valid and Min Price = 0 then set min price
			If bValidRoom Then
				If Not oRoomSequences.Contains(oRoomType.Seq) Then oRoomSequences.Add(oRoomType.Seq)

				If iPropertyMinPrice = 0 Then iPropertyMinPrice = nPrice
				If iPropertyMaxPrice = 0 Then iPropertyMaxPrice = nPrice

				iPropertyMinPrice = Functions.IIf(nPrice < iPropertyMinPrice, nPrice, iPropertyMinPrice)
				iPropertyMaxPrice = Functions.IIf(nPrice > iPropertyMaxPrice, nPrice, iPropertyMaxPrice)
			End If

			iRoomCount += 1

		Next

		bShowProperty = oRoomSequences.Count = BookingBase.SearchDetails.Rooms

		'2.f. Best Seller
		bBestSeller = Functions.IIf(oWorkTableItem.BestSeller = Nothing, SafeBoolean(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.BestSellerXPath)), oWorkTableItem.BestSeller)
		If bDisplay AndAlso Me.ResultsFilter.BestSeller Then
			bDisplay = bBestSeller
		End If


		'2.g. Free Wifi
		bFreeWifi = Functions.IIf(oWorkTableItem.FreeWifi = Nothing, SafeBoolean(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.FreeWifiXPath)), oWorkTableItem.FreeWifi)
		If bDisplay AndAlso Me.ResultsFilter.FreeWifi Then
			bDisplay = bFreeWifi
		End If

		'2.i Landmarks
		'Get the long and lat of the property
		nLongitude = Functions.IIf(oWorkTableItem.Longitude = Nothing, SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.LongitudeXPath)), oWorkTableItem.Longitude)
		nLatitude = Functions.IIf(oWorkTableItem.Latitude = Nothing, SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.LatitudeXPath)), oWorkTableItem.Latitude)

		Dim nDistanceFromLandmark As Double = 0
		If bDisplay AndAlso Me.ResultsFilter.LandmarkID <> 0 Then

			'Find the selected landmark and calculate the difference between it's position and that of the hotels
			Dim oSelectedLandmark As Filters.Landmark = Me.ResultsFilter.Landmarks.Where(Function(o) o.LandmarkID = Me.ResultsFilter.LandmarkID).FirstOrDefault
			Dim R As Decimal = 6371
			Dim dLat As Double = (Math.PI / 180) * (nLatitude - oSelectedLandmark.Latitude)
			Dim dLon As Double = (Math.PI / 180) * (nLongitude - oSelectedLandmark.Longitude)
			Dim dLat1 As Double = (Math.PI / 180) * oSelectedLandmark.Latitude
			Dim dLat2 As Double = (Math.PI / 180) * nLatitude

			Dim a As Double = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(dLat1) * Math.Cos(dLat2)
			Dim c As Double = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a))
			nDistanceFromLandmark = R * c

			'If this distance is greater than the value we have decided that landmarks need to fall into then hide the hotel
			If (nDistanceFromLandmark > Me.ResultsFilter.DistanceFromLandmark) Then
				bDisplay = False
			End If

		End If

		'2.h. Review Average Score
		Dim dMinAverageScore As Decimal = Functions.IIf(Me.ResultsFilter.MinAverageScore <= 1, 0, Me.ResultsFilter.MinAverageScore)
		Dim dMaxAverageScore As Decimal = Me.ResultsFilter.MaxAverageScore

		iReviewAverageScore = SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, String.Format("/Property/ReviewScores/ReviewScore[CMSWebsiteID = '{0}']/ReviewAverageScore", BookingBase.Params.CMSWebsiteID)))

		If iReviewAverageScore = 0 Then
			iReviewAverageScore = Functions.IIf(oWorkTableItem.ReviewAverageScore > 0, oWorkTableItem.ReviewAverageScore, Intuitive.Functions.SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.AverageScoreXPath)))
		End If

		If bDisplay AndAlso Not IgnoreAverageReviewScore Then
			If iReviewAverageScore > dMaxAverageScore OrElse iReviewAverageScore < dMinAverageScore Then
				bDisplay = False
			End If
		End If

		'2.j. Product attributes
		Dim aProductAtrributeIDs As String() = Me.ResultsFilter.ProductAttributeIDCSV.Split(","c)
		If oWorkTableItem.ProductAttributeIDs.Count > 0 Then
			iProductAttributeIDs = oWorkTableItem.ProductAttributeIDs
		Else
			Dim oNodeList As XmlNodeList = ivcPropertyResult.SearchResponseXML.SelectNodes(Me.ResultsFilter.ProductAttributeXPath)
			For Each oNode As XmlNode In oNodeList
				iProductAttributeIDs.Add(SafeInt(oNode("ProductAttributeID").InnerText))
			Next
		End If
		If Not IgnoreProductAttribute AndAlso Me.ResultsFilter.ProductAttributeIDCSV <> "" Then
			For Each ID As Integer In aProductAtrributeIDs
				If Not iProductAttributeIDs.Contains(ID) Then
					bDisplay = False
				End If
			Next
			If iProductAttributeIDs.Count = 0 Then
				bDisplay = False
			End If
		End If


		'2.k Property Type
		iPropertyTypeID = Functions.IIf(oWorkTableItem.PropertyTypeID > 0, oWorkTableItem.PropertyTypeID, Functions.SafeInt(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.PropertyTypeXPath)))
		If bDisplay AndAlso Not IgnorePropertyType AndAlso Me.ResultsFilter.PropertyTypeIDCSV <> "" Then
			Dim aPropertyTypeIDs As String() = Me.ResultsFilter.PropertyTypeIDCSV.Split(","c)
			If Not aPropertyTypeIDs.Contains(iPropertyTypeID.ToString) Then
				bDisplay = False
			End If

		End If


		'2.l. hide hotels if they do not have a selected flight
		If Not oWorkTableItem.GeographyLevel3ID = 0 Then
			If Not Me.FlightDictionary.ContainsKey(oWorkTableItem.GeographyLevel3ID) AndAlso Me.FlightDictionary.Count > 0 Then
				bDisplay = False
			End If
		End If

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere AndAlso Not Me.HotelFlightDictionary.ContainsKey(oWorkTableItem.PropertyReferenceID) Then
			bDisplay = False
		End If


		'2.m. TARating
		iTripAdvisorRating = Functions.IIf(oWorkTableItem.ReviewAverageScore > 0, oWorkTableItem.ReviewAverageScore, Math.Floor(Intuitive.Functions.SafeDecimal(XMLFunctions.SafeNodeValue(ivcPropertyResult.SearchResponseXML, Me.ResultsFilter.AverageScoreXPath))))
		If bDisplay AndAlso Not IgnoreTripAdvisorRating AndAlso Me.ResultsFilter.TripAdvisorRatingCSV <> "" Then
			Dim aTripAdvisorRatings As String() = Me.ResultsFilter.TripAdvisorRatingCSV.Split(","c)
			If Not aTripAdvisorRatings.Contains(SafeInt(Math.Floor(iTripAdvisorRating)).ToString) Then
				bDisplay = False
			End If
		End If

		If bDisplay AndAlso BookingBase.Params.SuppressOnRequestRoomTypes Then
			bDisplay = ivcPropertyResult.RoomTypes.Exists(Function(oRoomType) Not oRoomType.OnRequest)
		End If

		'3. Display Property
		If bDisplay Then
			bDisplay = bShowProperty
		End If


		'4. Set Work Table Item Properties
		oWorkTableItem.SetProperties(iPropertyMinPrice, iPropertyMaxPrice, sName, iMealBasisIDs, iProductAttributeIDs, iRating,
		  iGeographyLevel3ID, iFacilityFlag, bBestSeller, bFreeWifi, iReviewAverageScore, nLongitude, nLatitude,
		  nDistanceFromLandmark, iPropertyTypeID, bDisplay, iGeographyLevel2ID)


	End Sub


	'Filter Items
	Public Sub FilterItems(ByVal WorkTable As Generic.List(Of WorkTableItem), Optional ByVal bIgnoreRating As Boolean = False,
	 Optional ByVal bIgnoreTripAdvisorRating As Boolean = False, Optional ByVal bIgnoreResort As Boolean = False, Optional ByVal bIgnoreFacility As Boolean = False,
	  Optional ByVal bIgnoreMealBasis As Boolean = False, Optional ByVal bIgnorePrice As Boolean = False,
	  Optional ByVal bIgnoreProductAttribute As Boolean = False, Optional ByVal bIgnorePropertyType As Boolean = False,
	  Optional ByVal bIgnoreRegion As Boolean = False)

		For Each oItem As WorkTableItem In WorkTable

			Dim ivcPropertyResult As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oItem.Index)

			Me.FilterItem(ivcPropertyResult, oItem, IgnoreRating:=bIgnoreRating, IgnoreTripAdvisorRating:=bIgnoreTripAdvisorRating,
			  IgnoreResort:=bIgnoreResort, IgnoreFacility:=bIgnoreFacility, IgnoreMealBasis:=bIgnoreMealBasis, IgnorePrice:=bIgnorePrice,
			  IgnoreProductAttribute:=bIgnoreProductAttribute, IgnorePropertyType:=bIgnorePropertyType,
			  IgnoreRegion:=bIgnoreRegion)
		Next

	End Sub


	'Generate Filter Counts
	Public Sub GenerateFilterCounts()

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Generate Filter Counts", ProcessTimer.MainProcess)

		'1. Clear Filter Counts
		Me.ResultsFilter.Clear()


		'2. Star Ratings
		Me.FilterItems(Me.WorkTable, bIgnoreRating:=True)


		'2.2 get distinct ratings then loop through each rating and set the count and from price
		Dim aRatings As IEnumerable(Of Decimal) = (From oHotel In Me.WorkTable Select Math.Floor(oHotel.Rating)).Distinct
		Dim aSelectedRatings As String() = Me.ResultsFilter.RatingCSV.Split(","c)

		For Each nRating As Decimal In aRatings
			Dim oRating As New Filters.Rating
			With oRating
				.Rating = nRating
				.Count = Me.WorkTable.Where(Function(oHotel) Math.Floor(oHotel.Rating) = Math.Floor(oRating.Rating) And oHotel.Display = True).Count
				.Selected = aSelectedRatings.Contains(nRating.ToString)
			End With
			Me.ResultsFilter.Ratings.Add(oRating)
		Next

		'3. TripAdvisor ratings
		Me.FilterItems(Me.WorkTable, bIgnoreTripAdvisorRating:=True)

		Dim aTARatings As IEnumerable(Of Decimal) = (From oHotel In Me.WorkTable Select Math.Floor(oHotel.ReviewAverageScore)).Distinct
		Dim aSelectedTARatings As String() = Me.ResultsFilter.TripAdvisorRatingCSV.Split(","c)

		For Each nTARating As Decimal In aTARatings
			Dim iTARating As Integer = SafeInt(Math.Floor(nTARating))
			Dim oTARating As New Filters.TripAdvisorRating
			With oTARating
				.Rating = iTARating
				.Count = Me.WorkTable.Where(Function(oHotel) _
				  Math.Floor(oHotel.ReviewAverageScore) = Math.Floor(oTARating.Rating) And oHotel.Display = True).Count
				.Selected = aSelectedTARatings.Contains(iTARating.ToString)
			End With
			Me.ResultsFilter.TripAdvisorRatings.Add(oTARating)
		Next

		'4. Resorts
		Me.FilterItems(Me.WorkTable, bIgnoreResort:=True)

		'4.1 get distinct resorts then loop through each resort and set the count and from price
		Dim aResorts As IEnumerable(Of Integer) = (From oHotel In Me.WorkTable Select oHotel.GeographyLevel3ID).Distinct
		Dim aSelectedResorts As String() = Me.ResultsFilter.GeographyLevel3IDCSV.Split(","c)

		For Each iResort As Integer In aResorts
			Dim oFilterResort As New Filters.Resort
			With oFilterResort
				.GeographyLevel3ID = iResort

				If BookingBase.Lookups.GeographyDictionary.ContainsKey(iResort) Then
					Dim oRegionResort As Lookups.RegionResort = BookingBase.Lookups.GeographyDictionary(iResort)
					.Resort = oRegionResort.Resort
				End If

				.Count = Me.WorkTable.Where(Function(oHotel) oHotel.GeographyLevel3ID = oFilterResort.GeographyLevel3ID And oHotel.Display = True).Count
				.Selected = aSelectedResorts.Contains(iResort.ToString)
			End With
			Me.ResultsFilter.Resorts.Add(oFilterResort)
		Next


		'5. Facilities
		'5.1 we want to filter hotels using all filters set except facility to calculate other facility prices and counts
		Me.FilterItems(Me.WorkTable, bIgnoreFacility:=True)
		Dim aSelectedFacilities As String() = Me.ResultsFilter.FilterFacilityIDCSV.Split(","c)

		'5.2 loop through each of our cached filter facilities and set the count and from price
		For Each oFacility As Lookups.FilterFacility In BookingBase.Lookups.FilterFacilities

			Dim iPriority As Integer = oFacility.Priority

			Dim oFilterFacility As New Filters.FilterFacility
			With oFilterFacility
				.FilterFacilityID = oFacility.FilterFacilityID
				.Facility = oFacility.FilterFacility
				.Priority = iPriority
				.Count = Me.WorkTable.Where(Function(oHotel) oHotel.Display = True And (oHotel.FacilityFlag And Functions.SafeInt((2 ^ iPriority))) = Functions.SafeInt((2 ^ iPriority))).Count
				.Selected = aSelectedFacilities.Contains(oFacility.Priority.ToString)
			End With

			If oFilterFacility.Count > 0 Then
				Me.ResultsFilter.FilterFacilities.Add(oFilterFacility)
			End If
		Next


		'6. Product Attributes

		'6.1 we want to filter hotels using all filters set except product attributes to calculate other product attribute prices and counts
		Me.FilterItems(Me.WorkTable, bIgnoreProductAttribute:=True)
		Dim aSelectedProductAttributes As String() = Me.ResultsFilter.ProductAttributeIDCSV.Split(","c)

		'6.2 loop through each of our cached filter facilities and set the count and from price
		For Each oProductAttribute As Lookups.ProductAttribute In BookingBase.Lookups.ProductAttributes

			Dim oFilterAttribute As New Filters.ProductAttribute
			With oFilterAttribute
				.ProductAttributeID = oProductAttribute.ProductAttributeID
				.ProductAttribute = oProductAttribute.ProductAttribute
				.Count = Me.WorkTable.Where(Function(oHotel) oHotel.Display = True AndAlso oHotel.ProductAttributeIDs.Contains(oProductAttribute.ProductAttributeID)).Count
				.Selected = aSelectedProductAttributes.Contains(oProductAttribute.ProductAttributeID.ToString)
			End With

			If oFilterAttribute.Count > 0 Then
				Me.ResultsFilter.ProductAttributes.Add(oFilterAttribute)
			End If
		Next



		'7.1  we want to filter hotels using all filters set except meal basis to calculate other ratings prices and counts
		Me.FilterItems(Me.WorkTable, bIgnoreMealBasis:=True)
		Dim aSelectedMealBases As String() = Me.ResultsFilter.MealBasisIDCSV.Split(","c)

		'7.2 loop through each hotel and room option and add new meal basis or update existing rating and count
		For Each oHotel As WorkTableItem In Me.WorkTable.Where(Function(o) o.Display = True)

			Dim aMealBases As New Generic.List(Of Integer)

			For Each iMealBasisID As Integer In oHotel.MealBasisIDs
				If aMealBases.Contains(iMealBasisID) Then Continue For

				Dim bMealBasisExists As Boolean = False

				For Each oMealBasis As Filters.MealBasis In Me.ResultsFilter.MealBases
					If iMealBasisID = oMealBasis.MealBasisID Then
						With oMealBasis
							.Count = oMealBasis.Count + 1
							.Selected = aSelectedMealBases.Contains(oMealBasis.MealBasisID.ToString)
						End With
						bMealBasisExists = True
						Exit For
					End If
				Next

				If Not bMealBasisExists Then
					Dim oMealBasis As New Filters.MealBasis
					With oMealBasis
						.MealBasisID = iMealBasisID
						.MealBasis = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.MealBasis, iMealBasisID)
						.Count = 1
						.Selected = aSelectedMealBases.Contains(oMealBasis.MealBasisID.ToString)
					End With
					Me.ResultsFilter.MealBases.Add(oMealBasis)
				End If

				aMealBases.Add(iMealBasisID)

			Next

		Next


		'8. Free Wifi Count
		Me.ResultsFilter.FreeWifiCount = Me.WorkTable.Where(Function(o) o.Display AndAlso o.FreeWifi).Count



		'9. Property Types
		Me.FilterItems(Me.WorkTable, bIgnorePropertyType:=True)



		'10 get distinct property types then loop through each property type and set the count and from price
		Dim aPropertyTypes As IEnumerable(Of Integer) = (From oHotel In Me.WorkTable Select oHotel.PropertyTypeID).Distinct
		Dim aSelectedPropertyTypes As String() = Me.ResultsFilter.PropertyTypeIDCSV.Split(","c)

		For Each iPropertyTypeID As Integer In aPropertyTypes
			Dim oFilterPropertyType As New Filters.PropertyType
			With oFilterPropertyType
				.PropertyTypeID = iPropertyTypeID
				.PropertyType = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.PropertyType, iPropertyTypeID)
				.Count = Me.WorkTable.Where(Function(oHotel) oHotel.PropertyTypeID = oFilterPropertyType.PropertyTypeID And oHotel.Display = True).Count
				.Selected = aSelectedPropertyTypes.Contains(iPropertyTypeID.ToString)
			End With
			Me.ResultsFilter.PropertyTypes.Add(oFilterPropertyType)
		Next


		'11. Regions
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			Me.FilterItems(Me.WorkTable, bIgnoreRegion:=True)

			'11.1 get distinct regions then loop through each region and set the count and from price
			Dim aRegions As IEnumerable(Of Integer) = (From oHotel In Me.WorkTable Select oHotel.GeographyLevel2ID).Distinct
			Dim aSelectedRegions As String() = Me.ResultsFilter.GeographyLevel2IDCSV.Split(","c)
			For Each iRegion As Integer In aRegions

				Dim oFilterRegion As New Filters.Region
				With oFilterRegion
					.GeographyLevel2ID = iRegion
					.Region = BookingBase.Lookups.GetLocationFromRegion(iRegion).GeographyLevel2Name
					.Count = Me.WorkTable.Where(Function(oHotel) oHotel.GeographyLevel2ID = oFilterRegion.GeographyLevel2ID AndAlso oHotel.Display).Count
					.Selected = aSelectedRegions.Contains(iRegion.ToString)
				End With

				Dim oCheapestHotel As WorkTableItem = Me.WorkTable.Where(Function(oHotel) oHotel.GeographyLevel2ID = oFilterRegion.GeographyLevel2ID AndAlso oHotel.Display).OrderBy(Function(oHotel) oHotel.MinPrice + Me.HotelFlightDictionary(oHotel.PropertyReferenceID).Total).FirstOrDefault()
				If Not oCheapestHotel Is Nothing Then
					oFilterRegion.FromPrice = oCheapestHotel.MinPrice + Me.HotelFlightDictionary(oCheapestHotel.PropertyReferenceID).Total
				End If

				Me.ResultsFilter.Regions.Add(oFilterRegion)
			Next
		End If

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Generate Filter Counts", ProcessTimer.MainProcess)
	End Sub




	'Generate Price Band Filter
	Public Sub GeneratePriceBandFilter()

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Generate Price Band Filter", ProcessTimer.MainProcess)

		'clear price bands
		Me.ResultsFilter.PriceBands.Clear()

		'generate price bands
		Dim oPriceBandsXML As XmlDocument = Utility.BigCXML("FilterPriceBands", 1, 600)
		Me.ResultsFilter.PriceBands = Utility.XMLToGenericList(Of Filters.PriceBand)(oPriceBandsXML)

		'if no price bands then bomb out - no point running the filter and iterating over the list an extra time
		If Not Me.ResultsFilter.PriceBands.Count > 0 Then Exit Sub

		'filter items, ignoring price
		Me.FilterItems(Me.WorkTable, bIgnorePrice:=True)

		'loop through price bands and set counts
		Dim iPropertyDuration As Integer = BookingBase.SearchDetails.Duration

		For Each oPriceBand As Filters.PriceBand In Me.ResultsFilter.PriceBands

			'set selected
			If oPriceBand.MinPrice = Me.ResultsFilter.MinPrice / iPropertyDuration OrElse oPriceBand.MaxPrice = Me.ResultsFilter.MaxPrice / iPropertyDuration Then
				oPriceBand.Selected = True
			End If


			'reset count
			oPriceBand.Count = 0

			For Each oItem As WorkTableItem In Me.WorkTable.Where(Function(o) o.Display)

				'3.a Select Property using Item index
				Dim ivcPropertyResult As ivci.Property.SearchResponse.PropertyResult = Me.iVectorConnectResults(oItem.Index)


				For Each oRoomType As ivci.Property.SearchResponse.RoomType In ivcPropertyResult.RoomTypes

					If Me.ResultsFilter.MealBasisIDCSV <> "" Then
						Dim aFilterMealBasisIDs As String() = Me.ResultsFilter.MealBasisIDCSV.Split(","c)
						If Not aFilterMealBasisIDs.Contains(oRoomType.MealBasisID.ToString) Then
							Continue For
						End If
					End If

					If oRoomType.Total / iPropertyDuration >= oPriceBand.MinPrice AndAlso oRoomType.Total / iPropertyDuration <= oPriceBand.MaxPrice Then
						oPriceBand.Count += 1
						Exit For
					End If

				Next

			Next

		Next

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Generate Price Band Filter", ProcessTimer.MainProcess)

	End Sub


	'Generate landmarks
	Public Sub GenerateLandmarkFilter()

		'Grab the landmarks from BigC
		Dim oXml As XmlDocument = Utility.BigCXML("Landmark", 1, 600)
		Dim oLandmarks As Generic.List(Of Filters.Landmark) = Utility.XMLToGenericList(Of Filters.Landmark)(oXml, "Landmarks/Landmark")

		'Find all geo2s from our results and then get all the locations which are part of the geo2
		Dim iGeographyLevel2ID As Integer = Me.iVectorConnectResults.FirstOrDefault.GeographyLevel2ID

		Dim aRegionResorts As IEnumerable(Of Integer) =
		 (From oLocation In BookingBase.Lookups.Locations.Where(Function(o) o.GeographyLevel2ID = iGeographyLevel2ID) Select oLocation.GeographyLevel3ID).Distinct

		'Loop through all our landmarks
		For Each oLandmark As Filters.Landmark In oLandmarks
			'See if our landmarks geo3 is included in our collection of locations, if it is generate a filter landmark and add it to the filters collection.
			If aRegionResorts.Contains(oLandmark.GeographyLevel3ID) Then
				Dim oFilterLandmark As New Filters.Landmark
				With oFilterLandmark
					.LandmarkID = oLandmark.LandmarkID
					.Name = oLandmark.Name
					.Latitude = oLandmark.Latitude
					.Longitude = oLandmark.Longitude
					.Priority = Functions.IIf(oLandmark.Priority = 0, 1000, oLandmark.Priority)
				End With
				Me.ResultsFilter.Landmarks.Add(oFilterLandmark)
			End If

		Next
		Me.ResultsFilter.Landmarks = Me.ResultsFilter.Landmarks.OrderBy(Function(o) o.Name).OrderBy(Function(o) o.Priority).ToList

	End Sub


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


#Region "Support"

#Region "Work Table Item"

	Public Class WorkTableItem

		Public Index As Integer
		Public PropertyReferenceID As Integer
		Public SortValue As String = ""
		Public CustomSortValue As Integer = 0
		Public Display As Boolean = True
		Public CustomFilterDisplay As Boolean = True

		Public Name As String = ""
		Public MealBasisIDs As New Generic.List(Of Integer)
		Public ProductAttributeIDs As New Generic.List(Of Integer)
		Public Rating As Decimal = 0
		Public GeographyLevel2ID As Integer = 0
		Public GeographyLevel3ID As Integer = 0
		Public FacilityFlag As Integer = 0
		Public BestSeller As Boolean = Nothing
		Public FreeWifi As Boolean = Nothing
		Public ReviewAverageScore As Decimal = 0
		Public Longitude As Decimal = Nothing
		Public Latitude As Decimal = Nothing
		Public DistanceFromLandMark As Double = 0
		Public PropertyTypeID As Integer = 0

		Public MinPrice As Decimal = 0
		Public MaxPrice As Decimal = 0

		Public SortPrice As Decimal = 0

		Public Sub New(ByVal Index As Integer, ByVal PropertyReferenceID As Integer, GeographyLevel3ID As Integer, GeographyLevel2ID As Integer)
			Me.Index = Index
			Me.PropertyReferenceID = PropertyReferenceID
			Me.GeographyLevel3ID = GeographyLevel3ID
			Me.GeographyLevel2ID = GeographyLevel2ID
		End Sub


		'Set Work Table Item Values
		Public Sub SetProperties(ByVal iMinPrice As Decimal, ByVal iMaxPrice As Decimal, ByVal sName As String, ByVal iMealBasisIDs As Generic.List(Of Integer),
		  ByVal iProductAttributeIDs As Generic.List(Of Integer), ByVal iRating As Decimal, ByVal iGeographyLevel3ID As Integer, ByVal iFacilityFlag As Integer,
		  ByVal bBestSeller As Boolean, ByVal bFreeWifi As Boolean, ByVal iReviewAverageScore As Decimal, ByVal nLongitude As Decimal, ByVal nLatitude As Decimal,
		  ByVal nDistanceFromLandmark As Double, ByVal iPropertyTypeID As Integer, ByVal bDisplay As Boolean, ByVal iGeographyLevel2ID As Integer)

			Me.MinPrice = iMinPrice
			Me.MaxPrice = iMaxPrice
			Me.Name = sName
			Me.MealBasisIDs = iMealBasisIDs
			Me.ProductAttributeIDs = iProductAttributeIDs
			Me.Rating = iRating
			Me.GeographyLevel3ID = iGeographyLevel3ID
			Me.FacilityFlag = iFacilityFlag
			Me.BestSeller = bBestSeller
			Me.FreeWifi = bFreeWifi
			Me.ReviewAverageScore = iReviewAverageScore
			Me.Longitude = nLongitude
			Me.Latitude = nLatitude
			Me.DistanceFromLandMark = nDistanceFromLandmark
			Me.PropertyTypeID = iPropertyTypeID
			Me.Display = bDisplay
			Me.GeographyLevel2ID = iGeographyLevel2ID
		End Sub


	End Class

#End Region


#Region "Filter"

	Public Class Filters
		Implements ICloneable


#Region "Properties"

		'Filters
		Public Name As String
		Public RatingCSV As String = ""
		Public TripAdvisorRatingCSV As String = ""
		Public GeographyLevel3IDCSV As String = ""
		Public FilterFacilityIDCSV As String = ""
		Public ProductAttributeIDCSV As String = ""
		Public MealBasisIDCSV As String = ""
		Public PropertyTypeIDCSV As String = ""
		Public MinPrice As Integer = -1 'min room price
		Public MaxPrice As Integer = -1 'max room price
		Public MinAverageScore As Decimal = 0   'min average rating
		Public MaxAverageScore As Decimal = 10  'max average
		Public BestSeller As Boolean = False
		Public FreeWifi As Boolean = False
		Public LandmarkID As Integer
		Public DistanceFromLandmark As Decimal
		Public GeographyLevel2IDCSV As String = ""

		'XPaths
		Public NameXPath As String = "Property/PropertyName"
		Public RatingXPath As String = "Property/Rating"
		Public FacilityFilterIDXPath As String = "Property/FacilityFlag"
		Public BestSellerXPath As String = "Property/BestSeller"
		Public FreeWifiXPath As String = "Property/FreeWifi"
		Public AverageScoreXPath As String = "Property/ReviewAverageScore"
		Public LongitudeXPath As String = "Property/Longitude"
		Public LatitudeXPath As String = "Property/Latitude"
		Public ProductAttributeXPath As String = "Property/ProductAttributes/ProductAttribute"
		Public PropertyTypeXPath As String = "Property/PropertyTypeID"


		'Filter Counts and Prices
		Public Ratings As New Generic.List(Of Rating)
		Public Resorts As New Generic.List(Of Resort)
		Public FilterFacilities As New Generic.List(Of FilterFacility)
		Public MealBases As New Generic.List(Of MealBasis)
		Public PriceBands As New Generic.List(Of PriceBand)
		Public ProductAttributes As New Generic.List(Of ProductAttribute)
		Public Landmarks As New Generic.List(Of Landmark)
		Public FreeWifiCount As Integer
		Public PropertyTypes As New Generic.List(Of PropertyType)
		Public Regions As New List(Of Region)
		Private _TripAdvisorRatings As TripAdvisorRatingFilters = Nothing

#End Region

#Region "accessors and mutators"

		Public Property TripAdvisorRatings As TripAdvisorRatingFilters
			Get
				Return Me._TripAdvisorRatings
			End Get
			Set(value As TripAdvisorRatingFilters)
				Me._TripAdvisorRatings = value
			End Set
		End Property

#End Region

#Region "Constructor"

		Public Sub New()
			Me._TripAdvisorRatings = New TripAdvisorRatingFilters
		End Sub

		Public Sub New(ByVal UseFilter As Boolean, Optional ByVal Name As String = "", Optional ByVal RatingCSV As String = "", Optional ByVal GeographyLevel3IDCSV As String = "",
		Optional ByVal FacilityFilterIDCSV As String = "", Optional ByVal ProductAttributeIDCSV As String = "", Optional ByVal MealBasisIDCSV As String = "",
		Optional ByVal MinPrice As Integer = -1, Optional ByVal MaxPrice As Integer = -1, Optional ByVal FilterBy As eNumFilterBy = eNumFilterBy.Null, Optional GeographyLevel2IDCSV As String = "")

			Me.New()

			Me.RatingCSV = RatingCSV
			Me.MealBasisIDCSV = MealBasisIDCSV
			Me.GeographyLevel3IDCSV = GeographyLevel3IDCSV
			Me.FilterFacilityIDCSV = FacilityFilterIDCSV
			Me.ProductAttributeIDCSV = ProductAttributeIDCSV
			Me.GeographyLevel2IDCSV = GeographyLevel2IDCSV

			Me.Name = Name
			Me.MinPrice = MinPrice
			Me.MaxPrice = MaxPrice

		End Sub

#End Region


#Region "Filter Counts"

		Public Class Rating
			Public Rating As Decimal
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class TripAdvisorRatingFilters

			Public Sub New()
				Me._TripAdvisorRatings = New Generic.List(Of TripAdvisorRating)
			End Sub

			Private _TripAdvisorRatings As Generic.List(Of TripAdvisorRating) = Nothing

			Public Property TripAdvisorRatings As Generic.List(Of TripAdvisorRating)
				Get
					Return Me._TripAdvisorRatings
				End Get
				Set(value As Generic.List(Of TripAdvisorRating))
					Me._TripAdvisorRatings = value
				End Set
			End Property

			Public Sub Add(ByVal TripAdvisorRating As TripAdvisorRating)

				If Not Me.Contains(TripAdvisorRating) Then Me._TripAdvisorRatings.Add(TripAdvisorRating)

			End Sub

			Public Function Contains(ByVal TripAdvisorRating As TripAdvisorRating) As Boolean

				Dim bContains As Boolean = False

				For Each ContainedTripAdvisorRating As TripAdvisorRating In Me._TripAdvisorRatings
					If TripAdvisorRating.Rating = ContainedTripAdvisorRating.Rating Then
						bContains = True
						If TripAdvisorRating.Selected Then ContainedTripAdvisorRating.Selected = True
					End If

				Next

				Return bContains

			End Function

		End Class

		Public Class TripAdvisorRating

			Private _Rating As Decimal
			Private _Count As Integer
			Private _FromPrice As Decimal
			Private _Selected As Boolean

			Public Property Rating As Decimal
				Get
					Return Me._Rating
				End Get
				Set(value As Decimal)
					Me._Rating = value
				End Set
			End Property

			Public Property Count As Integer
				Get
					Return Me._Count
				End Get
				Set(value As Integer)
					Me._Count = value
				End Set
			End Property

			Public Property FromPrice As Decimal
				Get
					Return Me._FromPrice
				End Get
				Set(value As Decimal)
					Me._FromPrice = value
				End Set
			End Property

			Public Property Selected As Boolean
				Get
					Return Me._Selected
				End Get
				Set(value As Boolean)
					Me._Selected = value
				End Set
			End Property

		End Class

		Public Class Resort
			Public GeographyLevel3ID As Integer
			Public Resort As String
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class FilterFacility
			Public FilterFacilityID As Integer
			Public Facility As String
			Public Priority As Integer
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class MealBasis
			Public MealBasisID As Integer
			Public MealBasis As String
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Class PriceBand
			Public MinPrice As Decimal
			Public MaxPrice As Decimal
			Public Count As Integer
			Public Selected As Boolean
		End Class

		Public Class Landmark
			Public LandmarkID As Integer
			Public Name As String
			Public GeographyLevel3ID As Integer
			Public Longitude As Decimal
			Public Latitude As Decimal
			Public Priority As Integer
		End Class

		Public Class ProductAttribute
			Public ProductAttributeID As Integer
			Public ProductAttribute As String
			Public Count As Integer
			Public Selected As Boolean
		End Class

		Public Class PropertyType
			Public PropertyTypeID As Integer
			Public PropertyType As String
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class
		Public Class Region
			Public GeographyLevel2ID As Integer
			Public Region As String
			Public Count As Integer
			Public FromPrice As Decimal
			Public Selected As Boolean
		End Class

		Public Enum eNumFilterBy
			Rating
			Resort
			Facility
			MealBasis
			ProductAttribute
			PropertyType
			Null
		End Enum

		'Clear Filters
		Public Sub Clear()
			Me.Ratings.Clear()
			Me.Resorts.Clear()
			Me.FilterFacilities.Clear()
			Me.MealBases.Clear()
			Me.ProductAttributes.Clear()
			Me.PropertyTypes.Clear()
			Me.TripAdvisorRatings.TripAdvisorRatings.Clear()
			Me.Regions.Clear()
		End Sub

#End Region

		Public Function CloneFilter() As Filters
			Return CType(Me.Clone(), Filters)
		End Function
		Private Function Clone() As Object Implements System.ICloneable.Clone
			'add custom logic here (eg properties that dont get serialised/deserialied etc)

			Return Serializer.DeSerialize(Of Filters)(Serializer.Serialize(Me).OuterXml, False)
		End Function
	End Class

#End Region

#Region "Log Values"
	Public Class LogValues
		Public HotelCount As Integer
		Public SupplierCount As Integer
		Public AverageSuppliers As Double
		Public ResultCount As Integer
		Public IVectorConnectElapsedTime As Long
		Public RoomMappingElapsedTime As Long
		Public IVectorConnectReceivedTimeStamp As DateTime
		Public WebsitePostConnectElapsedTime As Double
	End Class
#End Region

#Region "Sorting"

	Public Class Sort

		Public SortBy As eSortBy = eSortBy.Price
		Public SortOrder As eSortOrder = eSortOrder.Ascending

		Public CustomSortBy As String = ""

		<XmlIgnore()> Public NameXPath As String = "Property/PropertyName"
		<XmlIgnore()> Public RatingXPath As String = "Property/Rating"
		<XmlIgnore()> Public ReviewScoreXPath As String = "Property/ReviewAverageScore"
		<XmlIgnore()> Public BestSellerXPath As String = "Property/BestSeller"
		<XmlIgnore()> Public InterestingnessXPath As String = "Property/Interestingness"
		'Property XML should use XML attributes e.g. <Brand ID="1" Int="42229" />
		<XmlIgnore()> Public InterestingnessByBrandXPath As String = "Property/Interestingness/Brand[@ID = #BrandID#]/@Int"
		<XmlIgnore()> Public Recommended As String = "Property/Recommended"

		'Sort X Path
		Public Function SortXPath(ByVal SortBy As eSortBy) As String

			Select Case SortBy
				Case eSortBy.Name
					Return Me.NameXPath
				Case eSortBy.Rating
					Return Me.RatingXPath
				Case eSortBy.ReviewScore
					Return Me.ReviewScoreXPath
				Case eSortBy.BestSeller
					Return Me.BestSellerXPath
				Case eSortBy.Interestingness
					Return Me.InterestingnessXPath
				Case eSortBy.InterestingnessByBrand
					Return ReplaceXPathTags(Me.InterestingnessByBrandXPath)
				Case eSortBy.Recommended
					Return Me.Recommended
			End Select

			Return ""

		End Function

		Public Function ReplaceXPathTags(ByVal XPath As String) As String

			Try
				Return XPath.Replace("#BrandID#", BookingBase.Params.BrandID.ToString)
			Catch ex As Exception
				Return ""
			End Try

		End Function

	End Class

#End Region


#Region "Classes"

	Public Class IVectorConnectResultsIndex
		Public Index As Integer
		Public PropertyResult As ivci.Property.SearchResponse.PropertyResult
	End Class

#End Region


#Region "Functions"

	'Clear Work Table
	Public Sub ClearWorkTable(Optional ClearIVCResults As Boolean = True)
		If ClearIVCResults Then
			Me.iVectorConnectResults.Clear()
		End If

		Me.WorkTable.Clear()
	End Sub
	Public Function GetHotelResultIndex(ByVal PropertyReferenceID As Integer) As Integer
		Dim iIndex As Integer = 0
		For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
			If oWorkTableItem.PropertyReferenceID = PropertyReferenceID Then
				iIndex = oWorkTableItem.Index
				Exit For
			End If
		Next

		Return iIndex
	End Function

	''' <summary>
	''' Sets up selected flight
	''' </summary>
	''' <param name="HideHotelsWithoutSelectedFlight">True by default, set to false if the routine calling this function calls Filter afterwards
	''' anyway as there is no need to iterate over the result set then</param>
	''' <remarks></remarks>
	Public Sub SetupSelectedFlight(Optional HideHotelsWithoutSelectedFlight As Boolean = True)

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Setup selected flight", ProcessTimer.MainProcess)

		'clear current flight dictionary
		Me.FlightDictionary.Clear()

		'if we have a basket flight use this flight otherwise get default flight per resort from results
		If BookingBase.SearchBasket.BasketFlights.Count > 0 Then

			Dim oBasketFlight As BookingFlight.BasketFlight = BookingBase.SearchBasket.BasketFlights(0)

			'get unique resorts
			Dim oResultFlight As FlightResultHandler.Flight = BookingBase.SearchDetails.FlightResults.GetSingleFlight(
			  oBasketFlight.ResultBookingToken, oBasketFlight.ReturnMultiCarrierResultBookingToken)


			'get unique resorts
			Dim aResorts As new List(Of Integer)

            if BookingBase.SearchDetails.FlightSearchMode = BookingSearch.FlightSearchModes.FlightSearch then
                aResorts =  BookingBase.Lookups.GetAirportGeographyLevel3IDs(oResultFlight.ArrivalAirportID)
            Else 
                For Each flightSector As FlightResultHandler.FlightSector In oResultFlight.FlightSectors
                    aResorts.AddRange(BookingBase.Lookups.GetAirportGeographyLevel3IDs(flightSector.ArrivalAirportID))
                    aResorts = aResorts.Distinct().ToList()
                Next
            End If

			'override from basket price in case of price change
			oResultFlight.Total = BookingBase.SearchBasket.BasketFlights(0).Flight.Price + BookingBase.SearchBasket.BasketFlights(0).Markup

			For Each iResort As Integer In aResorts
				Me.FlightDictionary.Add(iResort, oResultFlight)
			Next

		Else
			'get unique resorts
			Me.FlightDictionary = BookingBase.SearchDetails.FlightResults.GetDefaultFlightPerResort(Me.UniqueResorts)

			'get selected flight per hotel for anywhere search as can be changed separately
			If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
				Me.HotelFlightDictionary.Clear()
				For Each oWorkItem As WorkTableItem In Me.WorkTable
					If Me.FlightDictionary.ContainsKey(oWorkItem.GeographyLevel3ID) Then
						Dim oSelectedFlight As FlightResultHandler.Flight = Me.FlightDictionary(oWorkItem.GeographyLevel3ID)
						Me.HotelFlightDictionary.Add(oWorkItem.PropertyReferenceID, oSelectedFlight)
					Else
						oWorkItem.Display = False
					End If
				Next
			End If
		End If

		'hide hotels if they do not have a selected flight
		If HideHotelsWithoutSelectedFlight Then
			For Each oWorkItem As WorkTableItem In Me.WorkTable
				If Not Me.FlightDictionary.ContainsKey(oWorkItem.GeographyLevel3ID) Then
					oWorkItem.Display = False
				End If
			Next
		End If

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("IntuitiveWeb", "Setup selected flight", ProcessTimer.MainProcess)

	End Sub

	Public Function GetAlternativeFlights(ByVal GeographyLevel3ID As Integer, ByVal SelectedFlightToken As String) As List(Of FlightResultHandler.Flight)
		Dim oFlightOptions As List(Of FlightResultHandler.Flight) = BookingBase.SearchDetails.FlightResults.GetFlightOptionsForResort(GeographyLevel3ID, SelectedFlightToken)
		Return oFlightOptions
	End Function

	Public Sub UpdateSelectedFlight(ByVal sBookingToken As String, Optional ByVal sReturnMultiCarrierBookingToken As String = "")

		'1. Create a temp dictionary and get the correct flight, based on the Booking Token we have passed in.
		Dim oUpdateFlightDictionary As New Dictionary(Of Integer, FlightResultHandler.Flight)
		Dim oNewFlight As FlightResultHandler.Flight = BookingBase.SearchDetails.FlightResults.GetSingleFlight(sBookingToken, sReturnMultiCarrierBookingToken)


		'2.Loop through the existing dictionary, and add an item to our temp dictionary containing the new flight for each entry.
		For Each oItem As KeyValuePair(Of Integer, FlightResultHandler.Flight) In Me.FlightDictionary
			oUpdateFlightDictionary.Add(oItem.Key, oNewFlight)
		Next


		'3 update the flight dictionary to our new one.
		Me.FlightDictionary = oUpdateFlightDictionary

	End Sub

	Public Sub UpdateHotelSelectedFlight(ByVal iPropertyRefernceID As Integer, ByVal sFlightToken As String, Optional ByVal sReturnMultiCarrierToken As String = "")
		Dim oNewFlight As FlightResultHandler.Flight = BookingBase.SearchDetails.FlightResults.GetSingleFlight(sFlightToken, sReturnMultiCarrierToken)
		Me.HotelFlightDictionary(iPropertyRefernceID) = oNewFlight

		Me.ChangeFlightProperties.Remove(iPropertyRefernceID)
	End Sub

	'setup selected transfer
	Public Sub SetupSelectedTransfer()

		'get unique resorts
		Dim aResorts As IEnumerable(Of Integer) = From oIVCPropertyResult As ivci.Property.SearchResponse.PropertyResult In Me.iVectorConnectResults Select oIVCPropertyResult.GeographyLevel3ID Distinct


		'set transfer dictionary
		Me.TransferDictionary = BookingBase.SearchDetails.TransferResults.GetDefaultTransferPerResort(aResorts)

	End Sub



	'setup selected car hire
	Public Sub SetupSelectedCarHire()

		'get distinct selected flights
		Dim oFlights As New Generic.List(Of FlightResultHandler.Flight)

		For Each oFlight As FlightResultHandler.Flight In Me.FlightDictionary.Values.ToList()
			If Not oFlights.Contains(oFlight) Then
				oFlights.Add(oFlight)
			End If
		Next

		'set car hire dictionary
		Me.CarHireDictionary = BookingBase.SearchDetails.CarHireResults.GetDefaultCarHirePerFlight(oFlights)

	End Sub



	'Generate Hotel
	Public Function GenerateHotel(ByVal oPropertyResult As ivci.Property.SearchResponse.PropertyResult, ByVal iIndex As Integer) As Hotel


		'1. Create new hotel result object
		Dim oHotel As New Hotel
		With oHotel

			.Index = iIndex
			.PropertyReferenceID = oPropertyResult.PropertyReferenceID
			.BookingToken = oPropertyResult.BookingToken
			.GeographyLevel2ID = oPropertyResult.GeographyLevel2ID
			.GeographyLevel3ID = oPropertyResult.GeographyLevel3ID


			'2. Geo Location
			'if a resort hast just been mapped in the system it may not exist in the lookups yet
			' in this case don't render the hotel out but don't fall over !
			Dim oLocation As Lookups.Location = BookingBase.Lookups.GetLocationFromResort(oHotel.GeographyLevel3ID)
			If oLocation Is Nothing Then Return Nothing

			.Country = oLocation.GeographyLevel1Name
			.Region = oLocation.GeographyLevel2Name
			.Resort = oLocation.GeographyLevel3Name

			If Me.ResultsFilter.LandmarkID <> 0 Then

				'Grab the landmarks from BigC
				Dim oXml As XmlDocument = Utility.BigCXML("Landmark", 1, 600)
				Dim oLandmarks As Generic.List(Of Filters.Landmark) = Utility.XMLToGenericList(Of Filters.Landmark)(oXml, "Landmarks/Landmark")

				Dim oSelectedLandmark As New PropertyResultHandler.Hotel.SelectedLandmark
				With oSelectedLandmark
					.DistanceFromLandmark = Me.WorkTable(iIndex).DistanceFromLandMark.ToSafeDecimal
					.Name = oLandmarks.Find(Function(x) x.LandmarkID = Me.ResultsFilter.LandmarkID).Name
				End With
				.Landmark = oSelectedLandmark

			End If


			'3. XML Fields
			If Not oPropertyResult.SearchResponseXML Is Nothing Then
				Dim iReviewAverageScore As Decimal
				Dim iReviewNumberOfReviews As Integer

				iReviewAverageScore = SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, String.Format("/Property/ReviewScores/ReviewScore[CMSWebsiteID = '{0}']/ReviewAverageScore", BookingBase.Params.CMSWebsiteID)))
				iReviewNumberOfReviews = SafeInt(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, String.Format("/Property/ReviewScores/ReviewScore[CMSWebsiteID = '{0}']/ReviewNumberOfReviews", BookingBase.Params.CMSWebsiteID)))

				.Name = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/PropertyName")
				.Rating = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Rating"))
				.Address1 = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Address1")
				.Address2 = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Address2")
				.TownCity = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/TownCity")
				.PostcodeZip = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/PostcodeZip")

				.FacilityFlag = Functions.SafeInt(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/FacilityFlag"))
				.Longitude = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Longitude"))
				.Latitude = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Latitude"))
				.ReviewNumberOfReviews = IIf(iReviewNumberOfReviews > 0, iReviewNumberOfReviews, Functions.SafeInt(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/ReviewNumberOfReviews")))
				.ReviewAverageScore = IIf(iReviewAverageScore > 0, iReviewAverageScore, Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/ReviewAverageScore")))
				.BestSeller = Functions.SafeBoolean(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/BestSeller"))
				.FreeWifi = Functions.SafeBoolean(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/FreeWifi"))
				.PropertyType = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/PropertyType")

				Dim oMainImage As New Hotel.Image
				oMainImage.Image = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/MainImage")
				oMainImage.ImageTitle = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/MainImageTitle")
				oMainImage.AdditionalInfo = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/MainImageAdditionalInfo")
				.MainImage = oMainImage

				Dim oSmallImage As New Hotel.Image
				oSmallImage.Image = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/SmallImage")
				oSmallImage.ImageTitle = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/SmallImageTitle")
				oSmallImage.AdditionalInfo = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/SmallImageAdditionalInfo")
				.SmallImage = oSmallImage

				.ImageCount = Functions.SafeInt(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/ImageCount"))

				.Summary = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Summary")
				.URL = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/URL")


				Dim oNodeList As XmlNodeList = oPropertyResult.SearchResponseXML.SelectNodes("Property/Facilities/Facility")
				For Each oNode As XmlNode In oNodeList
					Dim oFacility As New Hotel.Facility
					oFacility.Facility = XMLFunctions.SafeNodeValue(oNode, "Facility")
					.Facilities.Add(oFacility)
				Next

				Dim sCustomXML As String = XMLFunctions.SafeOuterXML(oPropertyResult.SearchResponseXML, "Property/CustomXML")
				If sCustomXML <> "" AndAlso sCustomXML <> "<CustomXML />" Then
					Dim oCustomXML As New XmlDocument
					oCustomXML.LoadXml(sCustomXML)
					.CustomXML = oCustomXML
				End If

			End If

			'if showing package prices, add flight price to room total
			Dim nFlightPrice As Decimal = 0D
			If (BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel AndAlso BookingBase.Params.HotelFilterPackagePrices) Then
				Dim oFlight As New FlightResultHandler.Flight
				If Me.FlightDictionary.ContainsKey(oPropertyResult.GeographyLevel3ID) Then
					oFlight = Me.FlightDictionary(oPropertyResult.GeographyLevel3ID)
					nFlightPrice = oFlight.Total
				End If
			End If

			'4. Add rooms
			Dim iRoomNumber As Integer = 1
			For Each oRoomGuest As BookingSearch.Guest In BookingBase.SearchDetails.RoomGuests

				'4.a. Create new room object
				Dim oRoom As New Hotel.Room
				With oRoom
					.Adults = oRoomGuest.Adults
					.Children = oRoomGuest.Children
					.Infants = oRoomGuest.Infants
					.ChildAges = oRoomGuest.ChildAges
				End With



				'4.b. loop through room options and add to room
				Dim iRoomOption As Integer = 1
				For Each oRoomType As ivci.Property.SearchResponse.RoomType In oPropertyResult.RoomTypes.Where(Function(o) o.Seq = iRoomNumber)

					If BookingBase.Params.SuppressOnRequestRoomTypes AndAlso oRoomType.OnRequest Then
						iRoomOption += 1
						Continue For
					End If

					Dim nPrice As Decimal = oRoomType.Total

					'if showing package prices, add flight price to room total
					If nFlightPrice > 0D Then
						nPrice += nFlightPrice
					End If

					'4.b.i check if valid room option if we have a min/max price or mealbasis
					If Me.ResultsFilter.MinPrice > -1 Then
						If nPrice < Me.ResultsFilter.MinPrice OrElse nPrice > Me.ResultsFilter.MaxPrice Then
							iRoomOption += 1
							Continue For
						End If
					End If

					If Me.ResultsFilter.MealBasisIDCSV <> "" Then
						Dim aFilterMealBasisIDs As String() = Me.ResultsFilter.MealBasisIDCSV.Split(","c)
						If Not aFilterMealBasisIDs.Contains(oRoomType.MealBasisID.ToString) Then
							iRoomOption += 1
							Continue For
						End If
					End If


					'4.b.ii Set room option details
					Dim oRoomOption As New Hotel.RoomOption
					With oRoomOption
						.Index = oHotel.Index & "#" & iRoomNumber & "#" & iRoomOption
						.Seq = oRoomType.Seq
						.RoomBookingToken = oRoomType.RoomBookingToken
						.MealBasisID = oRoomType.MealBasisID

						'4.b.ii Meal Basis Lookup
						Dim sKey As String = oRoomOption.MealBasisID.ToString()
						If Not HttpContext.Current Is Nothing Then
							sKey += "_" + BookingBase.DisplayLanguageID.ToString()
						End If

						If Not Me.MealBasisDictionary.ContainsKey(sKey) Then
							Me.MealBasisDictionary.Add(sKey, BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.MealBasis, oRoomOption.MealBasisID))
						End If
						.MealBasis = Me.MealBasisDictionary(sKey)

						.RoomView = oRoomType.RoomView
						.RoomType = oRoomType.RoomType
						.GroupID = oRoomType.RoomGroupID
						.Discount = oRoomType.Discount
						.SpecialOffer = oRoomType.SpecialOffer
						.RegionalTax = oRoomType.RegionalTax
						.Price = oRoomType.Total
						.SupplierID = oRoomType.SupplierDetails.SupplierID
						.Source = oRoomType.SupplierDetails.Source
						.NonRefundable = oRoomType.NonRefundable
						.LocalCost = oRoomType.SupplierDetails.Cost
						.LocalCostCurrencyID = oRoomType.SupplierDetails.CurrencyID
						.OnRequest = oRoomType.OnRequest

						.TotalCommission = oRoomType.TotalCommission

						If oRoomType.TotalCommission > 0 Then
							.CommissionPercentage = Math.Round((oRoomType.TotalCommission / oRoomType.Total) * 100, 1, MidpointRounding.AwayFromZero)
						End If

						If Me.MarkupAmount > 0 Then
							.Markup = Me.MarkupAmount
						ElseIf Me.MarkupPercentage > 0 Then
							.Markup = oRoomType.Total - (oRoomType.Total / (1 + (Me.MarkupPercentage / 100)))
						End If
					End With


					'add to room options
					oRoom.RoomOptions.Add(oRoomOption)

					iRoomOption += 1
				Next

				iRoomNumber += 1

				'6. Add room to hotel
				oHotel.Rooms.Add(oRoom)
			Next

			'Room Mapping only
			If oPropertyResult.RoomGroups.Any Then

				Dim oLowestGroup As RoomGroup = oPropertyResult.RoomGroups.OrderBy(Function(group) group.RoomGroupMealBases.Min(Function(mb) mb.LeadInPrice)).FirstOrDefault()

				oHotel.MealBases = oHotel.Rooms.SelectMany(Function(room) room.RoomOptions.Select(Function(opt) New Hotel.HotelMealBasis(opt.MealBasis, opt.MealBasisID))).
											GroupBy(Function(mb) mb.MealBasisID).
											Select(Function(mb) mb.First()).
											OrderBy(Function(mb) OrderByLowestGroup(mb, oLowestGroup)).
											ThenBy(Function(mb) oHotel.Rooms.SelectMany(Function(room) room.RoomOptions.Where(Function(opt) opt.MealBasisID = mb.MealBasisID)).
																				OrderBy(Function(room) room.Price).
																				First().Price).
											ThenBy(Function(mb) mb.MealBasis).
											ToList()
				'Add Groups
				Dim iPosition As Integer = 0
				For Each oGroup As RoomGroup In oPropertyResult.RoomGroups.OrderBy(Function(group) group, New RoomGroupComparer(oHotel.MealBases.Select(Function(mb) mb.MealBasisID).ToList()))
					Dim oNewGroup As New Hotel.Group(oGroup.RoomGroupID, oGroup.RoomGroupName, oGroup.MealBasisCount, iPosition)
					oNewGroup.MealBases.AddRange(oGroup.RoomGroupMealBases.Select(Function(m) New Hotel.GroupMealBasis(m.MealBasisID, m.LeadInPrice)))
					oHotel.Groups.Add(oNewGroup)
					iPosition += 1
				Next

			End If

			'if flight plus hotel set selected flight
			If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel Then
				If Me.FlightDictionary.ContainsKey(oHotel.GeographyLevel3ID) Then
					oHotel.SelectedFlight = Me.FlightDictionary(oHotel.GeographyLevel3ID)
				End If
			ElseIf BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
				If Me.HotelFlightDictionary.ContainsKey(oHotel.PropertyReferenceID) Then
					oHotel.SelectedFlight = Me.HotelFlightDictionary(oHotel.PropertyReferenceID)
				End If

				Dim oAlternativeFlights As List(Of FlightResultHandler.Flight) = Me.GetAlternativeFlights(oHotel.GeographyLevel3ID, oHotel.SelectedFlight.BookingToken)
				oHotel.AlternativeFlightCount = oAlternativeFlights.Count

				If (Me.ChangeFlightProperties.Contains(oHotel.PropertyReferenceID)) Then
					.AlternativeFlights = oAlternativeFlights
				End If

			End If


			'if we have transfers in dictionary set selected transfer
			If Me.TransferDictionary.ContainsKey(oHotel.GeographyLevel3ID) Then
				oHotel.SelectedTransfer = Me.TransferDictionary(oHotel.GeographyLevel3ID)
			End If


			'if we have a selected flight and car hire dictionary is set, set selected car hire
			If Not oHotel.SelectedFlight Is Nothing AndAlso Me.CarHireDictionary.ContainsKey(oHotel.SelectedFlight.BookingToken) Then
				oHotel.SelectedCarHire = Me.CarHireDictionary(oHotel.SelectedFlight.BookingToken)
			End If


		End With

		'7. Calculate Mix/Max Price
		oHotel.CalculateMinMaxPrice()

		'8. Return Hotel
		Return oHotel

	End Function


	'Set Average Long/Lat
	Public Sub SetAverageLongLat()

		Dim dStart As DateTime = DateTime.Now

		'1. Get First Page of Results
		Dim oHotels As Generic.List(Of Hotel) = Me.GetHotelsPage(1)

		'2. Check Hotels Have Long/Lat set
		If oHotels.Where(Function(o) Not o.Latitude = 0 AndAlso Not o.Longitude = 0).Count > 0 Then

			Dim iTotal As Integer = 0
			Dim TotalX As Double = 0
			Dim TotalY As Double = 0
			Dim TotalZ As Double = 0

			For Each oHotel As Hotel In oHotels.Where(Function(o) Not o.Latitude = 0 AndAlso Not o.Longitude = 0)

				'3. Just calculate the average lon/lat
				If iTotal <= 20 Then

					Dim Lat As Double = oHotel.Latitude * Math.PI / 180
					Dim Lon As Double = oHotel.Longitude * Math.PI / 180

					Dim x As Double = Math.Cos(Lat) * Math.Cos(Lon)
					Dim y As Double = Math.Cos(Lat) * Math.Sin(Lon)
					Dim z As Double = Math.Sin(Lat)

					TotalX += x
					TotalY += y
					TotalZ += z

					iTotal += 1

				End If

			Next

			TotalX = TotalX / iTotal
			TotalY = TotalY / iTotal
			TotalZ = TotalZ / iTotal

			'4. Calculate
			Dim Longitude As Double = Math.Atan2(TotalY, TotalX) * 180 / Math.PI
			Dim Hyp As Double = Math.Sqrt(TotalX * TotalX + TotalY * TotalY + TotalZ * TotalZ)
			Dim Latitude As Double = Math.Asin(TotalZ / Hyp) * 180 / Math.PI

			'5. Set Long/Lat
			'BookingBase.SearchDetails.PropertyResults.AverageLatitude = Intuitive.Functions.SafeDecimal(Latitude)
			'BookingBase.SearchDetails.PropertyResults.AverageLongitude = Intuitive.Functions.SafeDecimal(Longitude)


		End If

	End Sub


	'Get room hash token
	Public Function RoomHashToken(ByVal PropertyRoomIndex As String) As String

		Dim iIndex As Integer = Functions.SafeInt(PropertyRoomIndex.Split("#"c)(0))
		Dim iRoomNumber As Integer = Functions.SafeInt(PropertyRoomIndex.Split("#"c)(1))
		Dim iRoomOption As Integer = Functions.SafeInt(PropertyRoomIndex.Split("#"c)(2))

		Return RoomHashToken(iIndex, iRoomNumber, iRoomOption)

	End Function


	Public Sub RemoveMarkup()

		Dim aPropertyMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Property AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

		For Each oPropertyResult As ivci.Property.SearchResponse.PropertyResult In Me.iVectorConnectResults

			For Each oRoomType As ivci.Property.SearchResponse.RoomType In oPropertyResult.RoomTypes

				For Each oMarkup As BookingBase.Markup In aPropertyMarkups
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							oRoomType.Total -= oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							oRoomType.Total -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
						Case BookingBase.Markup.eType.Percentage
							oRoomType.Total /= 1 + (oMarkup.Value / 100)
					End Select
				Next

			Next

			For Each oGroup As ivci.Property.SearchResponse.RoomGroup In oPropertyResult.RoomGroups

				For Each oMB As RoomGroupMealBasis In oGroup.RoomGroupMealBases
					For Each oMarkup As BookingBase.Markup In aPropertyMarkups
						Select Case oMarkup.Type
							Case BookingBase.Markup.eType.Amount
								oMB.LeadInPrice -= oMarkup.Value
							Case BookingBase.Markup.eType.AmountPP
								oMB.LeadInPrice -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
							Case BookingBase.Markup.eType.Percentage
								oMB.LeadInPrice /= 1 + (oMarkup.Value / 100)
						End Select
					Next
				Next
			Next

		Next

	End Sub


	Public Function RoomHashToken(ByVal Index As Integer, ByVal RoomNumber As Integer, ByVal RoomOption As Integer) As String

		Dim oWorkTable As WorkTableItem = Me.WorkTable(Index)

		Dim oPropertyResult As Booking.Property.PropertyResult = Me.iVectorConnectResults(Index)

		Dim aPropertyMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Property AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop


		Dim iRoomOption As Integer = 1
		For Each oRoomType As ivci.Property.SearchResponse.RoomType In oPropertyResult.RoomTypes.Where(Function(o) o.Seq = RoomNumber)

			If iRoomOption = RoomOption Then

				Dim oPropertyRoomOption As New BookingProperty.BasketProperty.BasketPropertyRoomOption
				With oPropertyRoomOption
					.BookingToken = oPropertyResult.BookingToken
					.Source = oRoomType.SupplierDetails.Source
					.RoomType = oRoomType.RoomType
					.ArrivalDate = oPropertyResult.ArrivalDate
					.Duration = oPropertyResult.Duration
					.DepartureDate = .ArrivalDate.AddDays(.Duration)
					.RoomBookingToken = oRoomType.RoomBookingToken
					.PropertyRoomTypeID = oRoomType.RoomTypeID
					.SupplierID = oRoomType.SupplierDetails.SupplierID
					.LocalCost = oRoomType.SupplierDetails.Cost
					.LocalCostCurrencyID = oRoomType.SupplierDetails.CurrencyID
					.CommissionPercentage = oRoomType.SupplierDetails.CommissionPercentage
					.GrossCost = oRoomType.SupplierDetails.GrossCost
					.TotalMargin = oRoomType.SupplierDetails.TotalMargin
					.RegionalTax = oRoomType.RegionalTax
					.TotalPrice = oRoomType.Total
					.TotalCommission = oRoomType.TotalCommission
					For Each oMarkup As BookingBase.Markup In aPropertyMarkups
						Select Case oMarkup.Type
							Case BookingBase.Markup.eType.Amount
								.TotalPrice -= oMarkup.Value
							Case BookingBase.Markup.eType.AmountPP
								.TotalPrice -= oMarkup.Value * (oPropertyRoomOption.GuestConfiguration.Adults + oPropertyRoomOption.GuestConfiguration.Children)
							Case BookingBase.Markup.eType.Percentage
								.TotalPrice /= 1 + (oMarkup.Value / 100)
						End Select
					Next

					Dim oGuests As BookingSearch.Guest = BookingBase.SearchDetails.RoomGuests(RoomNumber - 1)

					.GuestConfiguration = BookingSearch.GuestToGuestConfiguration(oGuests)
					.MealBasisID = oRoomType.MealBasisID
					.NonRefundable = oRoomType.NonRefundable
					.PropertyReferenceID = oPropertyResult.PropertyReferenceID
					.PropertyName = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/PropertyName")
					.Rating = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, "Property/Rating"))
					.GeographyLevel3ID = oPropertyResult.GeographyLevel3ID
					.hlpAffiliatePreferredRates = False
				End With

				Return oPropertyRoomOption.GenerateHashToken()
			End If


			iRoomOption += 1

		Next


		Return ""

	End Function

	Function OrderByLowestGroup(oMB As Hotel.HotelMealBasis, oLowestGroup As RoomGroup) As Decimal
		If oLowestGroup IsNot Nothing AndAlso oLowestGroup.RoomGroupMealBases.Any(Function(basis) basis.MealBasisID = oMB.MealBasisID) Then
			Return oLowestGroup.RoomGroupMealBases.First(Function(basis) basis.MealBasisID = oMB.MealBasisID).LeadInPrice
		Else
			Return Decimal.MaxValue
		End If
	End Function

	Public Function PropertyExists(ByVal PropertyReferenceID As Integer) As Boolean

		For Each oWorkItem As WorkTableItem In Me.WorkTable
			If oWorkItem.PropertyReferenceID = PropertyReferenceID Then
				Return True
			End If
		Next

		Return False

	End Function

#End Region


#Region "Hotel Class"

	Public Class Hotel

		Public Index As Integer
		Public BookingToken As String

		Public PropertyReferenceID As Integer
		Public URL As String

		Public Name As String
		Public Rating As Decimal

		Public Address1 As String
		Public Address2 As String
		Public TownCity As String
		Public PostcodeZip As String

		Public GeographyLevel2ID As Integer
		Public GeographyLevel3ID As Integer
		Public Country As String
		Public Region As String
		Public Resort As String

		Public Longitude As Decimal
		Public Latitude As Decimal
		Public Landmark As SelectedLandmark

		Public Summary As String
		Public Description As String
		Public MainImage As Image
		Public MainImageThumbnail As Image
		Public SmallImage As Image
		Public Images As New Generic.List(Of Image)
		Public ImageCount As Integer
		Public BestSeller As Boolean
		Public FreeWifi As Boolean
		Public PropertyType As String

		Public ReviewAverageScore As Decimal
		Public ReviewNumberOfReviews As Integer

		Public MinHotelPrice As Decimal
		Public MaxHotelPrice As Decimal

		Public MinPackagePrice As Decimal
		Public MaxPackagePrice As Decimal

		Public FacilityFlag As Integer
		Public Facilities As New Generic.List(Of Facility)

		Public CustomXML As XmlDocument

		Public Rooms As New Generic.List(Of Room)
		Public Groups As New List(Of Group)
		<XmlArrayItem("MealBasis")>
		Public MealBases As New List(Of HotelMealBasis)

		Public SelectedFlight As FlightResultHandler.Flight
		Public SelectedTransfer As BookingTransfer.Results.Transfer
		Public SelectedCarHire As BookingCarHire.CarHireResults.CarHire

		Public AlternativeFlights As List(Of FlightResultHandler.Flight)
		Public AlternativeFlightCount As Integer


#Region "Support Classes"

		Public Class HotelMealBasis
			Public MealBasis As String
			Public MealBasisID As Integer
			Private Shared ReadOnly Property aSequence As New List(Of String)(New String() {"Room Only", "Self Catering", "Continental Breakfast", "Bed and Breakfast", "Brunch", "Half Board", "HB with Brunch", "Full Board", "All Inclusive"})

			Public Sub New()

			End Sub

			Public Sub New(ByVal sMealBasis As String, ByVal iMealBasisID As Integer)
				MealBasis = sMealBasis
				MealBasisID = iMealBasisID
			End Sub

			Public Overrides Function Equals(obj As Object) As Boolean
				If (TryCast(obj, HotelMealBasis) IsNot Nothing) Then
					Return TryCast(obj, HotelMealBasis).MealBasis = MealBasis
				Else
					Return False
				End If
			End Function

			Public Function Seq() As Integer
				If aSequence.Contains(MealBasis) Then
					Return aSequence.IndexOf(MealBasis)
				Else
					Return 9
				End If
			End Function
		End Class

		Public Class Image
			Public Image As String
			Public ImageTitle As String
			Public AdditionalInfo As String
		End Class


		Public Class SelectedLandmark
			Public Name As String
			Public DistanceFromLandmark As Decimal
		End Class


		Public Class Room
			Public PropertyRoomBookingID As Integer
			Public RoomMinPrice As Decimal
			Public RoomMinPriceDiscount As Decimal
			Public Adults As Integer
			Public Children As Integer
			Public Infants As Integer
			Public ChildAges As New Generic.List(Of Integer)
			Public RoomOptions As New Generic.List(Of RoomOption)
		End Class

		Public Class Group
			Public GroupID As Integer
			Public Position As Integer
			Public GroupName As String
			<XmlArrayItem("MealBasis")>
			Public MealBases As List(Of GroupMealBasis)
			Public MealBasisIDCount As Integer

			Public Sub New()

			End Sub

			Public Sub New(Id As Integer, Name As String, Count As Integer, Seq As Integer)
				GroupID = Id
				Position = Seq
				GroupName = Name
				MealBasisIDCount = Count
				MealBases = New List(Of GroupMealBasis)
			End Sub
		End Class

		Public Class GroupMealBasis
			Public MealBasisID As Integer
			Public LeadInPrice As Decimal

			Public Sub New()

			End Sub

			Public Sub New(MealBasisID As Integer, LeadInPrice As Decimal)
				Me.MealBasisID = MealBasisID
				Me.LeadInPrice = LeadInPrice
			End Sub
		End Class

		Public Class RoomOption
			Public Index As String
			Public RoomBookingToken As String
			Public Seq As Integer
			Public MealBasisID As Integer
			Public RoomType As String
			Public GroupID As Integer
			Public RoomView As String
			Public MealBasis As String
			Public AvailableRooms As Integer
			Public Discount As Decimal
			Public SpecialOffer As String
			Public Price As Decimal
			Public RegionalTax As Decimal
			Public Selected As Boolean
			Public SupplierID As Integer
			Public Source As String
			Public LocalCost As Decimal
			Public LocalCostCurrencyID As Integer
			Public TotalCommission As Decimal
			Public CommissionPercentage As Decimal = 0
			Public NonRefundable As Boolean
			Public DailyRates As New Generic.List(Of DailyRate)
			Public Display As Boolean = True
			Public Markup As Decimal
			Public OnRequest As Boolean
		End Class

		Public Class DailyRate
			Public Rate As Decimal
			Public Discount As Decimal
			Public DayOfTheWeek As String
			Public RateDate As Date
		End Class

		Public Class Facility
			Public Facility As String
		End Class

#End Region


#Region "Calculate Min Max Price"

		Public Sub CalculateMinMaxPrice()

			'loop through each room and room option and set the min/max price
			Me.MinHotelPrice = 0
			For Each oRoom As Room In Me.Rooms

				oRoom.RoomMinPrice = 9999999
				oRoom.RoomMinPriceDiscount = 0

				For Each oRoomOption As RoomOption In oRoom.RoomOptions
					oRoom.RoomMinPriceDiscount = Functions.IIf(oRoomOption.Price < oRoom.RoomMinPrice, oRoomOption.Discount, oRoom.RoomMinPriceDiscount)

					Dim nRoomPrice As Decimal = oRoomOption.Price
					If BookingBase.Params.TaxExclusiveRates Then
						nRoomPrice -= oRoomOption.RegionalTax
					End If

					oRoom.RoomMinPrice = Functions.IIf(nRoomPrice < oRoom.RoomMinPrice, nRoomPrice, oRoom.RoomMinPrice)
				Next

				Me.MinHotelPrice += oRoom.RoomMinPrice
			Next


			'if we have a selected flight set min/max package price
			If Not Me.SelectedFlight Is Nothing Then
				Me.MinPackagePrice = Me.MinHotelPrice + Me.SelectedFlight.Total
				Me.MaxPackagePrice = Me.MaxHotelPrice + Me.SelectedFlight.Total
			End If




		End Sub

#End Region


#Region "Set Hotel Content"

		'Public Sub SetHotelContent()

		'	Dim oPropertyXML As XmlDocument = Utility.BigCXML("Property", Me.PropertyReferenceID, 0)
		'	Me.Summary = XMLFunctions.SafeNodeValue(oPropertyXML, "/Property/Summary")
		'	Me.Description = XMLFunctions.SafeNodeValue(oPropertyXML, "/Property/Description")
		'	Me.URL = XMLFunctions.SafeNodeValue(oPropertyXML, "/Property/URL")

		'	'main image
		'	Dim oMainImage As New Hotel.Image
		'	With oMainImage
		'		.Image = XMLFunctions.SafeNodeValue(oPropertyXML, "/Property/MainImage")
		'	End With
		'	Me.MainImage = oMainImage


		'	'other images
		'	For Each oNode As XmlNode In oPropertyXML.SelectNodes("/Property/Images/Image")
		'		Dim oImage As New Hotel.Image
		'		With oImage
		'			.Image = XMLFunctions.SafeNodeValue(oNode, "Image")
		'		End With
		'		Me.Images.Add(oImage)
		'	Next

		'End Sub

#End Region


	End Class


#End Region


#Region "Enums"

	Public Enum eSortBy
		Price
		Name
		Rating
		TripAdvisorRating
		ReviewScore
		BestSeller
		Interestingness
		InterestingnessByBrand
		Recommended
		Custom
	End Enum

	Public Enum eSortOrder
		Ascending
		Descending
	End Enum

#End Region

#End Region

End Class
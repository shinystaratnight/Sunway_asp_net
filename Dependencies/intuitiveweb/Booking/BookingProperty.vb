Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Linq

Public Class BookingProperty

#Region "Search"

	Public Shared Function Search(ByVal oBookingSearch As BookingSearch, ByVal oLookups As Lookups, Optional ByVal bUseRoomMapping As Boolean = False) As BookingSearch.SearchReturn

		oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.PropertySearch, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.[Property].SearchRequest

		Try

			oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)

			'Add details to the class
			With oiVectorConnectSearchRequest

				'set login details
				.LoginDetails = oBookingSearch.LoginDetails

				'search mode
				If oBookingSearch.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
					OrElse oBookingSearch.SearchMode = BookingSearch.SearchModes.Anywhere Then
					.SearchMode = "FlightPlusHotel"
				Else
					.SearchMode = "Hotel"
				End If

				.UseRoomMapping = bUseRoomMapping AndAlso .SearchMode = "Hotel" AndAlso oBookingSearch.Rooms < 2

				'geography
				If oBookingSearch.ArrivingAtID > 1000000 Then
					.AirportID = oBookingSearch.ArrivingAtID - 1000000
				ElseIf oBookingSearch.ArrivingAtID > 0 Then
					.RegionID = oBookingSearch.ArrivingAtID
				ElseIf oBookingSearch.GeographyGroupingID > 0 Then
					Dim aResort As Generic.List(Of Integer) = oLookups.GeographyGroupGeographies(oBookingSearch.GeographyGroupingID)
					.Resorts.AddRange(aResort)
				ElseIf oBookingSearch.ArrivingAtID < 0 Then
					.Resorts.Add(oBookingSearch.ArrivingAtID * -1)
				End If

				'geocode
				If oBookingSearch.Longitude <> 0 OrElse oBookingSearch.Latitude <> 0 Then
					.Longitude = oBookingSearch.Longitude
					.Latitude = oBookingSearch.Latitude
					.Radius = oBookingSearch.Radius
				End If

				'dates and duration
				.ArrivalDate = oBookingSearch.DepartureDate
				.Duration = oBookingSearch.Duration

				'rooms
				For Each oRoomGuest As BookingSearch.Guest In oBookingSearch.RoomGuests
					Dim oRoom As New ivci.[Property].SearchRequest.RoomRequest
					oRoom.GuestConfiguration = BookingSearch.GuestToGuestConfiguration(oRoomGuest)
					oiVectorConnectSearchRequest.RoomRequests.Add(oRoom)
				Next

				'advanced search
				.MealBasisID = oBookingSearch.MealBasisID
				.MinStarRating = oBookingSearch.Rating
				.PropertyReferenceID = oBookingSearch.PropertyReferenceID

				If Not IsNothing(oBookingSearch.PropertyReferenceIDs) AndAlso oBookingSearch.PropertyReferenceIDs.Count > 0 Then
					.PropertyReferenceIDs.AddRange(oBookingSearch.PropertyReferenceIDs)
				End If

				'set flight details if flight and hotel
				If oBookingSearch.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
					OrElse oBookingSearch.SearchMode = BookingSearch.SearchModes.Anywhere Then
					Dim oFlightDetails As New ivci.Property.SearchRequest.FlightDetailsDef
					.FlightDetails = oFlightDetails
					With oFlightDetails
						.FlightAndHotel = True
						If oBookingSearch.DepartingFromID > 1000000 Then
							.DepartureAirportGroupID = oBookingSearch.DepartingFromID - 1000000
						Else
							.DepartureAirportID = oBookingSearch.DepartingFromID
						End If
					End With
				End If

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If

			End With

			oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SetIVCRequestDetails, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)

			'If everything is ok then send request
			If oSearchReturn.OK Then

				Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
				oHeaders.AddNew("processtimerguid", oBookingSearch.ProcessTimer.TimerGUID.ToString)

				oBookingSearch.ProcessTimer.RecordStart(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				If oBookingSearch.EmailSearchTimes Then
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.[Property].SearchResponse)(oiVectorConnectSearchRequest, oBookingSearch.EmailTo, Headers:=oHeaders)
				Else
					oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.[Property].SearchResponse)(oiVectorConnectSearchRequest, Headers:=oHeaders)
				End If

				oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)
				oSearchReturn.DataCollectionInfo.IVectorConnectReceivedTimeStamp = DateTime.Now

				Dim oRequestInfo As New BookingSearch.RequestInfo
				With oRequestInfo
					.RequestTime = oIVCReturn.RequestTime
					.RequestXML = oIVCReturn.RequestXML
					.ResponseXML = oIVCReturn.ResponseXML
					.NetworkLatency = oIVCReturn.NetworkLatency
					.Type = BookingSearch.RequestInfoType.PropertySearch
				End With

				oSearchReturn.RequestInfo = oRequestInfo

				Dim oSearchResponse As New ivci.[Property].SearchResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.[Property].SearchResponse)

					'Check if there were any results
					If oSearchResponse.PropertyResults.Count > 0 Then

						'set search return
						oSearchReturn.PropertyResults = oSearchResponse.PropertyResults
						oSearchReturn.PropertyCount = oSearchResponse.PropertyResults.Count
						oSearchReturn.HotelArrivalDate = oSearchResponse.ArrivalDate
						oSearchReturn.HotelDuration = oSearchResponse.Duration

						oSearchReturn.DataCollectionInfo.SupplierCount = oSearchResponse.PropertyResults.SelectMany(Function(o) o.RoomTypes.Select(Function(r) r.SupplierDetails.SupplierID)).Distinct().Count()
						oSearchReturn.DataCollectionInfo.AverageSuppliers = oSearchResponse.PropertyResults.Average(Function(o) o.RoomTypes.Select(Function(r) r.SupplierDetails.SupplierID).Distinct().Count())
						oSearchReturn.DataCollectionInfo.HotelCount = oSearchReturn.PropertyCount
						Dim oValue As ProcessTimer.TimerItem = oBookingSearch.ProcessTimer.Times.First(Function(o) o.Key = String.Join("|", Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.SendingRequestToiVectorConnect, ProcessTimer.MainProcess)).Value
						oSearchReturn.DataCollectionInfo.IVectorConnectElapsedTime = CLng((oValue.EndTicks - oValue.StartTicks) / 10000)

						If bUseRoomMapping Then
							oSearchReturn.DataCollectionInfo.ResultCount = oSearchResponse.PropertyResults.Sum(Function(o) o.RoomGroups.Sum(Function(g) g.MealBasisCount))
							oSearchReturn.DataCollectionInfo.RoomMappingElapsedTime = oSearchResponse.RoomMappingElapsedTime
						Else
							oSearchReturn.DataCollectionInfo.ResultCount = oSearchResponse.PropertyResults.SelectMany(Function(o) o.RoomTypes).Count()
						End If

					End If
				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.PropertyCount = 0
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/PropertySearch", "PropertySearchException", ex.ToString)
		End Try

		oBookingSearch.ProcessTimer.RecordEnd(Utility.ProcessTimerSupport.ApplicationName, Utility.ProcessTimerSupport.ProcessStep.PropertySearch, ProcessTimer.MainProcess, ProcessTimerItemType.Property, 0)

		Return oSearchReturn

	End Function

#End Region

#Region "Basket"

#Region "add room"

	'add single room (hash token)
	Public Shared Sub AddRoomToBasket(ByVal HashToken As String, Optional ByVal ClearBasket As Boolean = True)

		'create room option from hash token
		Dim oPropertyRoomOption As BasketProperty.BasketPropertyRoomOption = BasketProperty.BasketPropertyRoomOption.DeHashToken(Of BasketProperty.BasketPropertyRoomOption)(HashToken)

		'add room option to basket
		BookingProperty.AddRoomToBasket(oPropertyRoomOption, ClearBasket)

	End Sub

	'add single room (room option)
	Public Shared Sub AddRoomToBasket(ByVal RoomOption As BasketProperty.BasketPropertyRoomOption, Optional ByVal ClearBasket As Boolean = True)

		'clear current basket properties
		If ClearBasket Then BookingBase.SearchBasket.BasketProperties.Clear()

		'create basket property
		Dim oBasketProperty As New BasketProperty
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketProperty.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1
		oBasketProperty.RoomOptions.Add(RoomOption)
		oBasketProperty.ResultBookingToken = RoomOption.BookingToken
		oBasketProperty.FlightAndHotel = (BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
			OrElse BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere)

		'set content xml
		Dim iPropertyReferenceID As Integer = oBasketProperty.RoomOptions(0).PropertyReferenceID
		Dim oHotel As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(iPropertyReferenceID, 0)
		If Not oHotel Is Nothing Then
			oBasketProperty.ContentXML = Serializer.Serialize(oHotel, True)
		Else
			Throw New Exception("Hotel not found in results")
		End If

		'add to basket
		BookingBase.SearchBasket.BasketProperties.Add(oBasketProperty)

	End Sub

	'add multiple rooms (hash tokens)
	Public Shared Sub AddRoomsToBasket(ByVal HashTokens As Generic.List(Of String), Optional ByVal ClearBasket As Boolean = True, Optional Byval MultiCenterID As Integer = 0)

		Dim oRoomOptions As New Generic.List(Of BasketProperty.BasketPropertyRoomOption)

		For Each sHashToken As String In HashTokens

			'create room option from hash token
			Dim oPropertyRoomOption As BasketProperty.BasketPropertyRoomOption = BasketProperty.BasketPropertyRoomOption.DeHashToken(Of BasketProperty.BasketPropertyRoomOption)(sHashToken)

			'add room option
			oRoomOptions.Add(oPropertyRoomOption)

		Next

		BookingProperty.AddRoomsToBasket(oRoomOptions, ClearBasket, MultiCenterID)

	End Sub

	'add multiple rooms (room options)
	Public Shared Sub AddRoomsToBasket(ByVal RoomOptions As Generic.List(Of BasketProperty.BasketPropertyRoomOption), Optional ByVal ClearBasket As Boolean = True, Optional Byval MultiCenterID As Integer = 0)

		'If we've passed in a multi center id, it means we're doing a multi-center search, remove any properties for this sector or later on in the journey, as this means we've
        'gone back in the breadcrumbs.
        If(MultiCenterID > 0) Then
            Dim propertiesToRemove As New List(Of BasketProperty)
            For Each basketProperty As BasketProperty In BookingBase.SearchBasket.BasketProperties
                If basketProperty.MultiCenterId >= MultiCenterID
                    propertiesToRemove.Add(basketProperty)
                End If
            Next
            For Each basketProperty As BasketProperty In propertiesToRemove
                BookingBase.SearchBasket.BasketProperties.Remove(basketProperty)
            Next
        ElseIf ClearBasket Then 'clear current basket properties
            BookingBase.SearchBasket.BasketProperties.Clear() 
        End If


		'create basket property
		Dim oBasketProperty As New BasketProperty
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketProperty.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1
        oBasketProperty.MultiCenterId = MultiCenterID

		'add each room option
		For Each oRoomOption As BasketProperty.BasketPropertyRoomOption In RoomOptions
			oBasketProperty.RoomOptions.Add(oRoomOption)
			oBasketProperty.ResultBookingToken = oRoomOption.BookingToken
		Next

		'set content xml
		Dim iPropertyReferenceID As Integer = oBasketProperty.RoomOptions(0).PropertyReferenceID
		Dim oHotel As PropertyResultHandler.Hotel = BookingBase.SearchDetails.PropertyResults.GetSingleHotel(iPropertyReferenceID, 0)
		If Not oHotel Is Nothing Then
			oBasketProperty.ContentXML = Serializer.Serialize(oHotel, True)
		Else
			Throw New Exception("Hotel not found in results")
		End If

		'add to basket
		BookingBase.SearchBasket.BasketProperties.Add(oBasketProperty)

	End Sub

#End Region

#Region "Remove From Basket"

	Public Shared Function RemoveRoomFromBasket(ByVal HashToken As String) As Boolean

		'Loop through each property
		For Each oProperty As BasketProperty In BookingBase.SearchBasket.BasketProperties

			'Loop through each room within that property checking to see if a room option matches the hash token
			For Each oRoomOption As BookingProperty.BasketProperty.BasketPropertyRoomOption In oProperty.RoomOptions

				If oRoomOption.HashToken = HashToken Then

					'If we have a match, remove the room option, if it is the last room option on the property remove that, then bomb out.
					oProperty.RoomOptions.Remove(oRoomOption)

					If oProperty.RoomOptions.Count = 0 Then BookingBase.SearchBasket.BasketProperties.Remove(oProperty)

					Return True

				End If
			Next

		Next

		'If no matches return false.
		Return False

	End Function
#End Region

#Region "Support Classes - BasketProperty"

	Public Class BasketProperty
		Public ComponentID As Integer
		Public RoomOptions As New Generic.List(Of BasketPropertyRoomOption)
		Public ContentXML As XmlDocument
		Public ResultBookingToken As String
		Public FlightAndHotel As Boolean
		Public TermsAndConditions As String
		Public TermsAndConditionsURL As String
        Public MultiCenterId As Integer = 0

		Public Class BasketPropertyRoomOption
			Inherits Utility.Hasher

			Public BookingToken As String
			Public Source As String
			Public RoomType As String
			Public ArrivalDate As Date
			Public Property ArrivalShortDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(ArrivalDate, "shortdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property ArrivalMediumDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(ArrivalDate, "mediumdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Duration As Integer
			Public DepartureDate As Date
			Public Property DepartureShortDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(DepartureDate, "shortdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property DepartureMediumDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(DepartureDate, "mediumdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public RoomBookingToken As String
			Public PropertyRoomTypeID As Integer
			Public SupplierID As Integer
			Public LocalCost As Decimal
			Public LocalCostCurrencyID As Integer
			Public TotalPrice As Decimal
			Public TotalCommission As Decimal
			Public GuestConfiguration As ivci.Support.GuestConfiguration
			Public Request As String
			Public MealBasisID As Integer
			Public PropertyReferenceID As Integer
			Public PropertyName As String
			Public Rating As Decimal
			Public GeographyLevel3ID As Integer
			Public hlpAffiliatePreferredRates As Boolean
			Public NonRefundable As Boolean

			'supplier details
			Public CommissionPercentage As Decimal 'this is buying commission, not selling commission
			Public GrossCost As Decimal
			Public TotalMargin As Decimal
			Public RegionalTax As Decimal
			Public Discount As Decimal

			Public GuestIDs As New Generic.List(Of Integer)

			Public Property Markup As Decimal
				Get
					Dim nTotalMarkup As Decimal = 0
					'Properties
					For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Property)
						Select Case oMarkup.Type
							Case BookingBase.Markup.eType.Amount
								nTotalMarkup += oMarkup.Value
							Case BookingBase.Markup.eType.AmountPP
								nTotalMarkup += oMarkup.Value * (Me.GuestConfiguration.Adults + Me.GuestConfiguration.Children)
							Case BookingBase.Markup.eType.Percentage
								nTotalMarkup += (oMarkup.Value * Me.TotalPrice) / 100
						End Select
					Next

					Return nTotalMarkup
				End Get
				Set(value As Decimal)
					'require this to be serialised
				End Set
			End Property

		End Class

		Public Function CreatePreBookRequest() As ivci.Property.PreBookRequest

			Dim oPreBookRequest As New ivci.Property.PreBookRequest
			With oPreBookRequest
				.BookingToken = Me.RoomOptions(0).BookingToken
				.ArrivalDate = Me.RoomOptions(0).ArrivalDate
				.Duration = Me.RoomOptions(0).Duration
				.FlightAndHotel = Me.FlightAndHotel

				For Each oRoomOption As BasketProperty.BasketPropertyRoomOption In Me.RoomOptions
					Dim oRoomBooking As New ivci.Property.PreBookRequest.RoomBooking
					With oRoomBooking
						.RoomBookingToken = oRoomOption.RoomBookingToken
						.GuestConfiguration = oRoomOption.GuestConfiguration
					End With
					oPreBookRequest.RoomBookings.Add(oRoomBooking)
				Next

			End With

			Return oPreBookRequest

		End Function

		Public Function CreateBookRequest() As ivci.Property.BookRequest

			Dim oBookRequest As New ivci.Property.BookRequest

			With oBookRequest
				.BookingToken = Me.RoomOptions(0).BookingToken
				.ArrivalDate = Me.RoomOptions(0).ArrivalDate
				.Duration = Me.RoomOptions(0).Duration
				.ExpectedTotal = Me.RoomOptions.Sum(Function(o) o.TotalPrice)
				.Request = Me.RoomOptions(0).Request

				For Each oRoomOption As BasketProperty.BasketPropertyRoomOption In Me.RoomOptions
					Dim oRoomBooking As New ivci.Support.RoomBooking

					With oRoomBooking
						.RoomBookingToken = oRoomOption.RoomBookingToken
						.GuestIDs.AddRange(oRoomOption.GuestIDs)
					End With

					oBookRequest.RoomBookings.Add(oRoomBooking)
				Next

			End With

			Return oBookRequest

		End Function

	End Class

#End Region

#End Region

End Class
Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization

Public Class BookingTransfer

#Region "Search"

	Public Shared Function Search(ByVal oTransferSearch As TransferSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.Transfer.SearchRequest

		If Not oTransferSearch.Oneway Then
			'update transfer return time to take into account StandardAirportArrivalMinutes
			Dim dReturnDateTime As Date = BookingTransfer.CalculateReturnDateTime(oTransferSearch.ReturnDate, oTransferSearch.ReturnTime)
			oTransferSearch.ReturnDate = dReturnDateTime.Date
			oTransferSearch.ReturnTime = dReturnDateTime.ToString("HH:mm")
		End If

		Try
			With oiVectorConnectSearchRequest

				'login details
				.LoginDetails = oTransferSearch.LoginDetails

				'departure and arrival points
				.DepartureParentType = oTransferSearch.DepartureParentType.ToString
				.DepartureParentID = oTransferSearch.DepartureParentID
				.ArrivalParentType = oTransferSearch.ArrivalParentType.ToString
				.ArrivalParentID = oTransferSearch.ArrivalParentID

				'one way
				.OneWay = oTransferSearch.Oneway

				'departure and return dates and time
				.DepartureDate = oTransferSearch.DepartureDate
				.DepartureTime = oTransferSearch.DepartureTime

				If Not .OneWay Then
					.ReturnDate = oTransferSearch.ReturnDate
					.ReturnTime = oTransferSearch.ReturnTime
				End If

				'pax
				.GuestConfiguration.Adults = oTransferSearch.Adults
				.GuestConfiguration.Children = oTransferSearch.Children
				.GuestConfiguration.Infants = oTransferSearch.Infants
				.GuestConfiguration.ChildAges = oTransferSearch.ChildAges

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
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Transfer.SearchResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.Transfer.SearchResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.Transfer.SearchResponse)

					'Check if there were any results
					If oSearchResponse.Transfers.Count > 0 Then

						'set search return
						oSearchReturn.TransferResults = oSearchResponse.Transfers
						oSearchReturn.TransferCount = oSearchResponse.Transfers.Count

					End If
				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.TransferCount = 0
				End If

				'this would be better if we did it in bookingsearch like for flights + hotels but there is not one central method
				If BookingBase.LogAllXML Then

					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.NetworkLatency = oIVCReturn.NetworkLatency
						.Type = BookingSearch.RequestInfoType.TransferSearch
					End With

					WebSupportToolbar.AddUniqueLog(oRequestInfo)

				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

	Public Class TransferSearch

		Public LoginDetails As iVectorConnectInterface.LoginDetails

		Public DepartureParentType As ParentType
		Public DepartureParentID As Integer
		Public ArrivalParentType As ParentType
		Public ArrivalParentID As Integer

		Public DepartureDate As Date
		Public DepartureTime As String
		Public ReturnDate As Date
		Public ReturnTime As String

		Public Oneway As Boolean = False

		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public ChildAges As New Generic.List(Of Integer)

		Public Enum ParentType
			Airport
			Resort
			[Property]
			Port
			Station
		End Enum

		Public Warning As New Generic.List(Of String)

	End Class

#End Region

#Region "Transfer information"

	Public Shared Function TransferInformation(ByVal oTransferInformationRequest As ivci.Transfer.InformationRequest) As BookingTransfer.TransferInformationReturn

		Dim oTransferInformationReturn As New TransferInformationReturn

		'Do the iVectorConnect validation procedure
		Dim oWarnings As Generic.List(Of String) = oTransferInformationRequest.Validate()

		If oWarnings.Count > 0 Then
			oTransferInformationReturn.OK = False
			oTransferInformationReturn.Warning.AddRange(oWarnings)
		End If

		If oTransferInformationReturn.OK Then

			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Transfer.InformationResponse)(oTransferInformationRequest)

			Dim oTransferInformationResponse As New ivci.Transfer.InformationResponse

			If oIVCReturn.Success Then
				oTransferInformationResponse = CType(oIVCReturn.ReturnObject, ivci.Transfer.InformationResponse)
				oTransferInformationReturn.Response = oTransferInformationResponse
			Else
				oTransferInformationReturn.OK = False
				oTransferInformationReturn.Warning = oIVCReturn.Warning
			End If

		End If

		Return oTransferInformationReturn

	End Function

	Public Class TransferInformationReturn
		Public OK As Boolean = True
		Public Warning As New Generic.List(Of String)
		Public Response As New ivci.Transfer.InformationResponse
	End Class

#End Region

#Region "Results"

	Public Class Results

		Public TotalTransfers As Integer
		Public Transfers As New Generic.List(Of Transfer)

		Public OutboundFlightCode As String = ""
		Public ReturnFlightCode As String = ""

#Region "Save"

		Public Shared Sub Save(ByVal TransferResults As Generic.List(Of ivci.Transfer.SearchResponse.Transfer), ByVal TransferSearch As BookingTransfer.TransferSearch,
		  ByVal OutboundFlightCode As String, ByVal ReturnFlightCode As String)

			Dim oTransferResults As New BookingTransfer.Results

			'Get Transfer Markups
			Dim aTransferMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Transfer AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

			'Filter Transfers to only return transfers from the cheapest supplier.
			Dim nLowestPrice As Decimal = TransferResults.Min(Function(oTransferPrice) oTransferPrice.Price)
			Dim iSupplierID As Integer = TransferResults.Where(Function(oTransfer) oTransfer.Price = nLowestPrice).FirstOrDefault.SupplierDetails.SupplierID

			For Each oTransferResult As ivci.Transfer.SearchResponse.Transfer In TransferResults
				'if we are deduping by supplier and not cheapest supplier dont try and save - just keep going
				If BookingBase.Params.DedupeTransfers AndAlso Not oTransferResult.SupplierDetails.SupplierID = iSupplierID Then Continue For

				Dim oTransfer As New Results.Transfer
				With oTransfer

					.BookingToken = oTransferResult.BookingToken
					.Price = oTransferResult.Price
					.OneWay = TransferSearch.Oneway

					'vehicle details
					.Vehicle = oTransferResult.Vehicle
					.MinimumCapacity = oTransferResult.MinimumCapacity
					.MaximumCapacity = oTransferResult.MaximumCapacity
					.VehicleQuantity = oTransferResult.VehicleQuantity

					'depature date and time
					.OutboundDate = TransferSearch.DepartureDate
					.OutboundTime = TransferSearch.DepartureTime
					.OutboundJourneyTime = oTransferResult.OutboundJourneyTime

					'calculate return time
					'return time should already have StandardAirportArrivalMinutes taken off at search
					'only need to take of journey time from the result as we do not know this at search
					Dim iHour As Integer = 0
					Dim iMinutes As Integer = 0

					If TransferSearch.ReturnTime <> "" AndAlso TransferSearch.ReturnTime.Contains(":") Then
						iHour = Functions.SafeInt(TransferSearch.ReturnTime.Split(":"c)(0))
						iMinutes = Functions.SafeInt(TransferSearch.ReturnTime.Split(":"c)(1))
					End If

					If Not TransferSearch.Oneway Then
						Dim dReturnDate As Date = TransferSearch.ReturnDate.AddHours(iHour).AddMinutes(iMinutes)

				    If BookingBase.Params.UseReturnTransferJourneyTime Then
						Dim iJourneyMinutes As Integer = Functions.SafeInt(oTransferResult.ReturnJourneyTime)
						dReturnDate = dReturnDate.AddMinutes(-iJourneyMinutes)
				    End If

						'set return date and time
						.ReturnDate = TransferSearch.ReturnDate
						.ReturnTime = dReturnDate.ToString("HH:mm")
						.ReturnJourneyTime = oTransferResult.ReturnJourneyTime
					End If

					'departure and arrival points
					.DepartureParentType = oTransferResult.DepartureParentType
					.DepartureParentID = oTransferResult.DepartureParentID

					.ArrivalParentType = oTransferResult.ArrivalParentType
					.ArrivalParentID = oTransferResult.ArrivalParentID

					'flight codes
					.OutboundFlightCode = OutboundFlightCode
					.ReturnFlightCode = ReturnFlightCode

                    If oTransferResult.DepartureParentType = "Property" Then
						Dim oPropertyXML As XmlDocument = Utility.BigCXML("Property", oTransferResult.DepartureParentID, 0)
						.DepartureParent = XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name")
					ElseIf oTransferResult.DepartureParentType = "Resort" Then
						.DepartureParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Resort, oTransferResult.DepartureParentID)
                    ElseIf oTransferResult.DepartureParentType = "Airport" Then
                        .DepartureParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oTransferResult.DepartureParentID)
                    ElseIf oTransferResult.DepartureParentType = "Port" Then
                        .DepartureParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Port, oTransferResult.DepartureParentID)
                    ElseIf oTransferResult.DepartureParentType = "Station" Then
                        .DepartureParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Station, oTransferResult.DepartureParentID)
					End If

					If oTransferResult.ArrivalParentType = "Property" Then
						Dim oPropertyXML As XmlDocument = Utility.BigCXML("Property", oTransferResult.ArrivalParentID, 0)
						.ArrivalParent = XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name")
					ElseIf oTransferResult.ArrivalParentType = "Resort" Then
						.ArrivalParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Resort, oTransferResult.ArrivalParentID)
                    ElseIf oTransferResult.ArrivalParentType = "Airport" Then
                        .ArrivalParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, oTransferResult.ArrivalParentID)
                    ElseIf oTransferResult.ArrivalParentType = "Port" Then
                        .ArrivalParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Port, oTransferResult.ArrivalParentID)
                    ElseIf oTransferResult.ArrivalParentType = "Station" Then
                        .ArrivalParent = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Station, oTransferResult.ArrivalParentID)
					End If

				End With

				'set up transfer option
				Dim oTransferOption As New BasketTransfer.TransferOption
				With oTransferOption
					.BookingToken = oTransfer.BookingToken
					.Price = oTransfer.Price
					.DepartureParentType = oTransfer.DepartureParentType
					.DepartureParentID = oTransfer.DepartureParentID
					.ArrivalParentType = oTransfer.ArrivalParentType
					.ArrivalParentID = oTransfer.ArrivalParentID
					.DepartureDate = oTransfer.OutboundDate
					.DepartureTime = oTransfer.OutboundTime
					.ReturnDate = oTransfer.ReturnDate
					.ReturnTime = oTransfer.ReturnTime
					.Adults = BookingBase.SearchDetails.TotalAdults
					.Children = BookingBase.SearchDetails.TotalChildren
					.Infants = BookingBase.SearchDetails.TotalInfants
					.ChildAges = BookingBase.SearchDetails.AllChildAges
					.SupplierID = oTransfer.SupplierID
					.LocalCost = oTransfer.LocalCost
					.CurrencyID = oTransfer.LocalCostCurrencyID
					.OneWay = TransferSearch.Oneway

					.OutboundFlightCode = OutboundFlightCode
					.ReturnFlightCode = ReturnFlightCode
				End With

				'generate hash token
				oTransfer.TransferOptionHashToken = oTransferOption.GenerateHashToken()

				'add markup (after creating flight option)
				For Each oMarkup As BookingBase.Markup In aTransferMarkups
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							oTransfer.MarkupAmount += oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							oTransfer.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
						Case BookingBase.Markup.eType.Percentage
							oTransfer.MarkupPercentage = oMarkup.Value
					End Select
				Next

				'add mark up to total
				oTransfer.Price += oTransfer.MarkupAmount
				oTransfer.Price *= (oTransfer.MarkupPercentage / 100) + 1

				'add transfer to results
				oTransferResults.Transfers.Add(oTransfer)

			Next

			'set total
			oTransferResults.TotalTransfers = oTransferResults.Transfers.Count

			'save on session
			BookingBase.SearchDetails.TransferResults = oTransferResults

		End Sub

#End Region

#Region "Get Single Transfer"

		'get single transfer (booking token)
		Public Function GetSingleTransfer(ByVal BookingToken As String) As Transfer

			Dim oTransfer As Transfer = Me.Transfers.Where(Function(o) o.BookingToken = BookingToken).FirstOrDefault
			Return oTransfer

		End Function

#End Region

#Region "Get Default Transfer"

		Public Function GetDefaultTransferPerResort(ByVal aResorts As IEnumerable(Of Integer)) As Dictionary(Of Integer, Results.Transfer)

			'sort flights
			Me.Transfers = Me.Transfers.OrderBy(Function(o) o.Price).ToList

			Dim oDictionary As New Dictionary(Of Integer, Results.Transfer)

			For Each iResort As Integer In aResorts

				Dim oTransfer As Transfer = Me.GetDefaultTransfer(iResort)

				If Not oTransfer Is Nothing Then
					oDictionary.Add(iResort, oTransfer)
				End If

			Next

			Return oDictionary

		End Function

		Public Function GetDefaultTransfer(ByVal GeographyLevel3ID As Integer) As Transfer

			'Find the cheapest valid transfer based on resort
			Dim oDefaultTransfer As New Transfer
			oDefaultTransfer = Me.Transfers _
			  .Where(Function(oTransfer) oTransfer.ArrivalParentType = "Resort" AndAlso oTransfer.ArrivalParentID = GeographyLevel3ID) _
			.FirstOrDefault()

			Return oDefaultTransfer

		End Function

#End Region

#Region "Get Results"

		Public Function GetResultsXML() As XmlDocument

			'serialize to xml and return
			Dim oResultXML As XmlDocument = Serializer.Serialize(Me, True)
			Return oResultXML

		End Function

#End Region

#Region "Transfer"

		Public Class Transfer
			Public Property BookingToken As String
			Public Property TransferOptionHashToken As String

			Public Property Vehicle As String
			Public Property MinimumCapacity As Integer
			Public Property MaximumCapacity As Integer
			Public Property VehicleQuantity As Integer

			Public Property DepartureParentType As String
			Public Property DepartureParentID As Integer
			Public Property DepartureParent As String

			Public Property ArrivalParentType As String
			Public Property ArrivalParentID As Integer
			Public Property ArrivalParent As String

			Public Property OutboundDate As Date
			Public Property OutboundShortDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundDate, "shortdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property OutboundMediumDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(OutboundDate, "mediumdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property OutboundTime As String
			Public Property OutboundJourneyTime As String

			Public Property ReturnDate As Date
			Public Property ReturnShortDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnDate, "shortdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property ReturnMediumDate As String
				Get
					Return Intuitive.Web.Translation.TranslateAndFormatDate(ReturnDate, "mediumdate")
				End Get
				Set(value As String)
				End Set
			End Property
			Public Property ReturnTime As String
			Public Property ReturnJourneyTime As String

			Public Property Price As Decimal
			Public Property SupplierID As Integer
			Public Property LocalCost As Decimal
			Public Property LocalCostCurrencyID As Integer

			Public Property OutboundFlightCode As String
			Public Property ReturnFlightCode As String

			Public Property OneWay As Boolean

			Public MarkupAmount As Decimal
			Public MarkupPercentage As Decimal

		End Class

#End Region

#Region "Remove Markup"

		Public Sub RemoveMarkup()

			For Each oTransfer As Transfer In Me.Transfers

				oTransfer.Price -= oTransfer.MarkupAmount
				oTransfer.Price /= (oTransfer.MarkupPercentage / 100) + 1

				oTransfer.MarkupAmount = 0
				oTransfer.MarkupPercentage = 0

			Next

		End Sub

#End Region

#Region "Clone"

		Public Shared Function CloneTransfer(ByVal Transfer As Transfer) As Transfer

			Dim oReturn As New Transfer

			Dim oTransferXML As XmlDocument = Serializer.Serialize(Transfer, True)
			oReturn = CType(Serializer.DeSerialize(Transfer.GetType, oTransferXML.InnerXml), Transfer)

			Return oReturn

		End Function

#End Region

	End Class

#End Region

#Region "Basket"

#Region "Add Transfer"

	'add transfer option (hash token)
	Public Shared Sub AddTransferToBasket(ByVal HashToken As String, Optional ByVal ClearBasket As Boolean = True, _
		 Optional ByVal OutboundFlightCode As String = "", Optional ByVal ReturnFlightCode As String = "", _
		 Optional ByVal DepartureTime As String = "", Optional ByVal ReturnTime As String = "")

		'create transfer option from hash token
		Dim oTransferOption As BasketTransfer.TransferOption = BasketTransfer.TransferOption.DeHashToken(Of BasketTransfer.TransferOption)(HashToken)

		If OutboundFlightCode <> "" AndAlso ReturnFlightCode <> "" Then
			oTransferOption.OutboundFlightCode = OutboundFlightCode
			oTransferOption.ReturnFlightCode = ReturnFlightCode
		ElseIf OutboundFlightCode <> "" AndAlso ReturnFlightCode = "" Then
			oTransferOption.OutboundFlightCode = OutboundFlightCode
			oTransferOption.OneWay = True
		End If

		'set times if being passed in
		If DepartureTime <> "" Then oTransferOption.DepartureTime = DepartureTime
		If ReturnTime <> "" Then oTransferOption.ReturnTime = ReturnTime

		'add transfer option
		BookingTransfer.AddTransferToBasket(oTransferOption, ClearBasket)

	End Sub

	'add transfer option (transfer option) 'should be private but theres a funtion using it somewhere...
	Public Shared Sub AddTransferToBasket(ByVal TransferOption As BasketTransfer.TransferOption, ByVal ClearBasket As Boolean)

		ClearTransfers(ClearBasket)

		'create basket transfer
		Dim oBasketTransfer As New BasketTransfer
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketTransfer.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1
		oBasketTransfer.Transfer = TransferOption
        oBasketTransfer.MultiCenterId = BookingBase.SearchDetails.ItineraryDetails.CurrentCenter

		'set content xml
		Dim oTransfer As Results.Transfer = BookingBase.SearchDetails.TransferResults.GetSingleTransfer(oBasketTransfer.Transfer.BookingToken)

		'Need to pass through the flight codes if we manually added them.
		If oTransfer.OutboundFlightCode = "" AndAlso oTransfer.ReturnFlightCode = "" Then
			oTransfer.OutboundFlightCode = oBasketTransfer.Transfer.OutboundFlightCode
			oTransfer.ReturnFlightCode = oBasketTransfer.Transfer.ReturnFlightCode
		End If
		If Not TransferOption.OutboundFlightCode = "" Then oTransfer.OutboundFlightCode = TransferOption.OutboundFlightCode
		If Not TransferOption.ReturnFlightCode = "" Then oTransfer.ReturnFlightCode = TransferOption.ReturnFlightCode

		If Not oTransfer.OutboundFlightCode = "" Then oBasketTransfer.Transfer.OutboundFlightCode = oTransfer.OutboundFlightCode
		If Not oTransfer.ReturnFlightCode = "" Then oBasketTransfer.Transfer.ReturnFlightCode = oTransfer.ReturnFlightCode

		oTransfer.OneWay = TransferOption.OneWay

		oBasketTransfer.ContentXML = Serializer.Serialize(oTransfer, True)

		'add to basket
		BookingBase.SearchBasket.BasketTransfers.Add(oBasketTransfer)

	End Sub

	Public Shared Sub ClearTransfers(Optional ByVal clearBasket As Boolean = true)

	    'clear current basket transfers, If we're doing a multi-center booking, we want to keep the transfers from previous centers
	    If ClearBasket Then
	        Dim transfersToKeep As New List(Of BasketTransfer)
	        If BookingBase.SearchDetails.ItineraryDetails.CurrentCenter > 0 Then
	            For Each basketTransfer As BasketTransfer In BookingBase.SearchBasket.BasketTransfers
	                If basketTransfer.MultiCenterId < BookingBase.SearchDetails.ItineraryDetails.CurrentCenter Then
	                    transfersToKeep.Add(basketTransfer)
	                End If
	            Next
	        End If
	        BookingBase.SearchBasket.BasketTransfers.Clear()
	        BookingBase.SearchBasket.BasketTransfers.AddRange(transfersToKeep)
	    End If

	End Sub

#End Region

#Region "Remove Transfer"

	'remove transfer option (hash token)
	Public Shared Sub RemoveTransferFromBasket(ByVal HashToken As String)

		'create transfer option from hash token
		Dim oTransferOption As BasketTransfer.TransferOption = BasketTransfer.TransferOption.DeHashToken(Of BasketTransfer.TransferOption)(HashToken)

		For Each oTransfer As BasketTransfer In BookingBase.SearchBasket.BasketTransfers
			If oTransfer.Transfer.HashToken = oTransferOption.HashToken Then
				BookingBase.SearchBasket.BasketTransfers.Remove(oTransfer)
				Exit For
			End If
		Next

	End Sub

#End Region

#Region "Update Transfer"

	'remove transfer option (hash token) - where is this being used?
	Public Shared Sub UpdateTransferInBasket(ByVal HashToken As String)

		'create transfer option from hash token
		Dim oTransferOption As BasketTransfer.TransferOption = BasketTransfer.TransferOption.DeHashToken(Of BasketTransfer.TransferOption)(HashToken)

		'Remove the transfer in the basket with the same transfer type
		For Each oTransfer As BasketTransfer In BookingBase.SearchBasket.BasketTransfers
			If oTransfer.Transfer.HashToken = oTransferOption.HashToken Then
				BookingBase.SearchBasket.BasketTransfers.Remove(oTransfer)
				Exit For
			End If
		Next

		'add transfer option
		BookingTransfer.AddTransferToBasket(oTransferOption, False)

	End Sub

#End Region

#Region "Support Classes - BasketTransfer"

	Public Class BasketTransfer
		Public ComponentID As Integer

		Public Transfer As TransferOption
		Public ContentXML As XmlDocument
		Public GuestIDs As New Generic.List(Of Integer)
        Public MultiCenterId As Integer = 0

		Public Property Markup As Decimal
			Get
				Dim nTotalMarkup As Decimal = 0
				For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Transfer)
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							nTotalMarkup += oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							nTotalMarkup += oMarkup.Value * (Me.Transfer.Adults + Me.Transfer.Children)
						Case BookingBase.Markup.eType.Percentage
							nTotalMarkup += (oMarkup.Value * Me.Transfer.Price) / 100
					End Select
				Next

				Return nTotalMarkup
			End Get
			Set(value As Decimal)
				'require this to be serialised
			End Set
		End Property

		Public Class TransferOption
			Inherits Utility.Hasher

			Public BookingToken As String
			Public hlpSearchBookingToken As String
			Public Price As Decimal
			Public TotalCommission As Decimal

			Public DepartureParentType As String
			Public DepartureParentID As Integer
			Public DepartureParentName As String
			Public ArrivalParentType As String
			Public ArrivalParentID As Integer
			Public ArrivalParentName As String
			Public DepartureDate As Date
			Public DepartureTime As String
			Public OneWay As Boolean
			Public ReturnDate As Date
			Public ReturnTime As String

			Public OutboundFlightCode As String = ""
			Public ReturnFlightCode As String = ""

			Public Adults As Integer
			Public Children As Integer
			Public Infants As Integer
			Public ChildAges As Generic.List(Of Integer)

			Public SupplierID As Integer ' dowe need these 3?
			Public LocalCost As Decimal
			Public CurrencyID As Integer
		End Class

#Region "Pre Book"

		Public Function CreatePreBookRequest() As ivci.Transfer.PreBookRequest

			Dim oPreBookRequest As New ivci.Transfer.PreBookRequest

			With oPreBookRequest
				.BookingToken = Me.Transfer.BookingToken
				.DepartureParentType = Me.Transfer.DepartureParentType
				.DepartureParentID = Me.Transfer.DepartureParentID
				.ArrivalParentType = Me.Transfer.ArrivalParentType
				.ArrivalParentID = Me.Transfer.ArrivalParentID
				.DepartureDate = Me.Transfer.DepartureDate
				.DepartureTime = Me.Transfer.DepartureTime
				.OneWay = Me.Transfer.OneWay
				.ReturnDate = Me.Transfer.ReturnDate
				.ReturnTime = Me.Transfer.ReturnTime

				'Added these nodes as they are needed for some third parties, there are additional details that could be added also.
				Dim oOutboundDetails As New ivci.Transfer.BookRequest.OutboundJourneyDetails
				With oOutboundDetails
					.FlightCode = Me.Transfer.OutboundFlightCode
				End With
				.OutboundDetails = oOutboundDetails

				Dim oReturnDetails As New ivci.Transfer.BookRequest.ReturnJourneyDetails
				With oReturnDetails
					.FlightCode = Me.Transfer.ReturnFlightCode
				End With
				.ReturnDetails = oReturnDetails

				'set guest configuration
				Dim oGuestConfiguration As New ivci.Support.GuestConfiguration
				oGuestConfiguration.Adults = Me.Transfer.Adults
				oGuestConfiguration.Children = Me.Transfer.Children
				oGuestConfiguration.ChildAges = Me.Transfer.ChildAges
				oGuestConfiguration.Infants = Me.Transfer.Infants

				.GuestConfiguration = oGuestConfiguration

			End With

			Return oPreBookRequest

		End Function

#End Region

#Region "Book"

		Public Function CreateBookRequest() As ivci.Transfer.BookRequest

			Dim oBookRequest As New ivci.Transfer.BookRequest

			With oBookRequest
				.BookingToken = Me.Transfer.BookingToken
				.ExpectedTotal = Me.Transfer.Price

				'setup outbound details
				Dim oOutboundDetails As New ivci.Transfer.BookRequest.OutboundJourneyDetails
				With oOutboundDetails
					.JourneyOrigin = Me.Transfer.DepartureParentName
					.AccommodationName = Me.Transfer.ArrivalParentName
					If Not Me.Transfer.OutboundFlightCode = "" Then .FlightCode = Me.Transfer.OutboundFlightCode
				End With
				.OutboundDetails = oOutboundDetails

				'setup return details
				Dim oReturnDetails As New ivci.Transfer.BookRequest.ReturnJourneyDetails
				With oReturnDetails
					If Not Me.Transfer.ReturnFlightCode = "" Then .FlightCode = Me.Transfer.ReturnFlightCode
					.PickupTime = Me.Transfer.ReturnTime
				End With
				.ReturnDetails = oReturnDetails

				'add the guests
				.GuestIDs.AddRange(Me.GuestIDs)

			End With

			Return oBookRequest

		End Function

#End Region

	End Class

#End Region

#End Region

#Region "Calculate Return Date Time"

	Public Shared Function CalculateReturnDateTime(ByVal ReturnDate As Date, ByVal ReturnTime As String) As Date

		'calculate return time
		Dim iHour As Integer = 0
		Dim iMinutes As Integer = 0

		If ReturnTime <> "" AndAlso ReturnTime.Contains(":") Then
			iHour = Functions.SafeInt(ReturnTime.Split(":"c)(0))
			iMinutes = Functions.SafeInt(ReturnTime.Split(":"c)(1))
		End If

		ReturnDate = ReturnDate.AddHours(iHour).AddMinutes(iMinutes)
		ReturnDate = ReturnDate.AddMinutes(-BookingBase.Params.StandardAirportArrivalMinutes)

		Return ReturnDate

	End Function

#End Region

End Class
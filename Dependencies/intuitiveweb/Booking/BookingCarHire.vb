Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Text.RegularExpressions
Imports System.Linq

Public Class BookingCarHire

	Public ResultsFilter As New CarHireResults.Filters

#Region "Search"

	Public Shared Function Search(ByVal oCarHireSearch As CarHireSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.CarHire.SearchRequest

		Try
			With oiVectorConnectSearchRequest

				'login details
				.LoginDetails = oCarHireSearch.LoginDetails

				'Pick up and Drop off data
				.PickUpDepotID = oCarHireSearch.PickUpDepotID
				.PickUpDate = oCarHireSearch.PickUpDate
				.PickUpTime = oCarHireSearch.PickUpTime

				.DropOffDepotID = oCarHireSearch.DropOffDepotID
				.DropOffDate = oCarHireSearch.DropOffDate
				.DropOffTime = oCarHireSearch.DropOffTime

				.LeadDriverBookingCountryID = oCarHireSearch.LeadDriverBookingCountryID
				.DriverAges = oCarHireSearch.DriverAges
				.TotalPassengers = oCarHireSearch.TotalPassengers
				.CustomerIP = oCarHireSearch.CustomerIP

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
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CarHire.SearchResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.CarHire.SearchResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.CarHire.SearchResponse)

					'Check if there were any results
					If oSearchResponse.CarHireResults.Count > 0 Then

						'set search return
						oSearchReturn.CarHireResults = oSearchResponse.CarHireResults
						oSearchReturn.CarHireCount = oSearchResponse.CarHireResults.Count

						'save the results since there's no point calling it seperately on the widget
						BookingCarHire.CarHireResults.Save(oSearchReturn.CarHireResults)
					Else
						BookingBase.SearchDetails.CarHireResults.TotalCarHires = 0
					End If
				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.CarHireCount = 0
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/CarHireSearch", "CarHireSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

	Public Shared Function GetAvailableDepots(ByVal iGeographyLevel3ID As Integer) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn

		If iGeographyLevel3ID > 0 Then

			Dim oCarHireDepots As New Generic.List(Of CarHireDepot)

			Dim sCarHireDepotIDs As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CarHireDepotGeographyLevel3, iGeographyLevel3ID)

			For Each sCarHireDepotID As String In sCarHireDepotIDs.Split(","c)
				If Not String.IsNullOrEmpty(sCarHireDepotID) Then
					Dim oCarHireDepot As New CarHireDepot
					oCarHireDepot.CarHireDepotID = sCarHireDepotID.ToSafeInt
					oCarHireDepot.DepotName = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CarHireDepot, oCarHireDepot.CarHireDepotID)
					oCarHireDepots.Add(oCarHireDepot)
				End If
			Next

			oSearchReturn.CarHireDepots = oCarHireDepots
			oSearchReturn.CarHireDepotCount = oCarHireDepots.Count

		End If

		Return oSearchReturn

	End Function

	Public Class CarHireSearch

		Public LoginDetails As iVectorConnectInterface.LoginDetails

		Public PickUpDepotID As Integer
		Public PickUpDate As Date
		Public PickUpTime As String

		Public DropOffDepotID As Integer
		Public DropOffDate As Date
		Public DropOffTime As String

		Public LeadDriverBookingCountryID As Integer
		Public DriverAges As New Generic.List(Of Integer)
		Public TotalPassengers As Integer
		Public CustomerIP As String

		Public Warning As New Generic.List(Of String)

	End Class

	Public Class CarHireDepot
		Public CarHireDepotID As Integer
		Public DepotName As String
	End Class

	Public Class CarHireInsuranceQuoteSearch

		Public LoginDetails As iVectorConnectInterface.LoginDetails
		Public BookingToken As String
		Public LeadGuestFirstName As String
		Public LeadGuestLastName As String

	End Class

	Public Class CarHireInsuranceQuoteSearchReturn

		Public Success As Boolean = True
		Public Warning As New Generic.List(Of String)
		Public CarHireInsuranceQuote As New CarHireResults.CarHireInsuranceQuote

	End Class

	Public Shared Function GetCarHireInsuranceQuote(ByVal oCarHireInsuranceQuoteSearch As CarHireInsuranceQuoteSearch) As CarHireInsuranceQuoteSearchReturn

		Dim oSearchReturn As New CarHireInsuranceQuoteSearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.CarHire.InsuranceQuoteRequest

		Try
			With oiVectorConnectSearchRequest
				.LoginDetails = oCarHireInsuranceQuoteSearch.LoginDetails
				.BookingToken = oCarHireInsuranceQuoteSearch.BookingToken
				.LeadGuestFirstName = oCarHireInsuranceQuoteSearch.LeadGuestFirstName
				.LeadGuestLastName = oCarHireInsuranceQuoteSearch.LeadGuestLastName

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.Success = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If
			End With

			'If everything is ok then send request
			If oSearchReturn.Success Then
				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CarHire.InsuranceQuoteResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.CarHire.InsuranceQuoteResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.CarHire.InsuranceQuoteResponse)

					With oSearchReturn.CarHireInsuranceQuote
						.BookingToken = oSearchResponse.BookingToken
						.ConfirmationText = oSearchResponse.ConfirmationText
						.CoveredItems = oSearchResponse.CoveredItems
						.FullPremiumName = oSearchResponse.FullPremiumName
						.InsuranceDescription = oSearchResponse.InsuranceDescription
						.InsuranceRegulationText = oSearchResponse.InsuranceRegulationsText
						.PremiumAmount = oSearchResponse.PremiumAmount
						.PremiumName = oSearchResponse.PremiumName
						.ProviderLogoURL = oSearchResponse.ProviderLogoURL
						.TermsAndConditions = oSearchResponse.TermsAndConditions
						.TermsAndConditionsURL = oSearchResponse.TermsAndConditionsURL
					End With
				Else
					oSearchReturn.Success = False
					oSearchReturn.Warning = oIVCReturn.Warning
				End If
			End If

		Catch ex As Exception
			FileFunctions.AddLogEntry("iVectorConnect/GetCarHireInsuranceQuote", "GetCarHireInsuranceQuoteException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

#End Region

#Region "information prebook"

	Public Shared Function InformationPreBook(ByVal oPreBookRequest As ivci.CarHire.PreBookRequest) As BookingCarHire.InformationPreBookReturn

		Dim oInformationPreBookReturn As New InformationPreBookReturn

		'Do the iVectorConnect validation procedure
		Dim oWarnings As Generic.List(Of String) = oPreBookRequest.Validate()

		If oWarnings.Count > 0 Then
			oInformationPreBookReturn.OK = False
			oInformationPreBookReturn.Warning.AddRange(oWarnings)
		End If

		'If everything is ok then send request
		If oInformationPreBookReturn.OK Then

			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CarHire.PreBookResponse)(oPreBookRequest)

			Dim oPreBookResponse As New ivci.CarHire.PreBookResponse

			If oIVCReturn.Success Then
				oPreBookResponse = CType(oIVCReturn.ReturnObject, ivci.CarHire.PreBookResponse)
				oInformationPreBookReturn.Response = oPreBookResponse
			Else
				oInformationPreBookReturn.OK = False
				oInformationPreBookReturn.Warning = oIVCReturn.Warning
			End If

		End If

		Return oInformationPreBookReturn

	End Function

#End Region

#Region "Results"

	Public Class CarHireResults

		Public TotalCarHires As Integer
		Public CarHires As New Generic.List(Of CarHire)
		Public TotalCarHireDepots As Integer
		Public CarHireDepots As New Generic.List(Of CarHireDepot)

#Region "Save"

		Public Shared Sub Save(ByVal CarHireResults As Generic.List(Of ivci.CarHire.SearchResponse.CarHireResult), Optional ByVal iDuration As Integer = 0)

			Dim oCarHireResults As New BookingCarHire.CarHireResults

			'Get Car Hire Markups
			Dim aCarHireMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.CarHire _
																									   AndAlso Not o.Value = 0).ToList

			For Each oCarHireResult As ivci.CarHire.SearchResponse.CarHireResult In CarHireResults

				Dim oCarHire As New CarHireResults.CarHire
				With oCarHire
                    .Source = oCarHireResult.SupplierDetails.Source
					.BookingToken = oCarHireResult.BookingToken
					.Identifier = oCarHireResult.BookingToken
					.VehicleDescription = oCarHireResult.VehicleDescription
					.PaxCapacity = oCarHireResult.PaxCapacity
					.AdultCapacity = oCarHireResult.AdultCapacity
					.ChildCapacity = oCarHireResult.ChildCapacity
					.BaggageCapacity = oCarHireResult.BaggageCapacity
					.LargeBaggageCapacity = oCarHireResult.LargeBaggageCapacity
					.SmallBaggageCapacity = oCarHireResult.SmallBaggageCapacity
					.Price = oCarHireResult.Price
					.Duration = iDuration

					'wanky - but your-carhire tp doesnt return the protocol so we need to add it or browser thinks its a relative URL
					'this is totally assuming we are on http page - will need some refactoring for secure pages
					If Not oCarHireResult.ImageURL.StartsWith("http") Then
						.ImageURL = "http://" + oCarHireResult.ImageURL
					Else
						.ImageURL = oCarHireResult.ImageURL
					End If
					.CarInformation = oCarHireResult.CarInformation
					.VehicleCode = ExtractCarInformation(.CarInformation & "|", "Vehicle Code")
					.Type = ExtractCarInformation(.CarInformation & "|", "Type")
					.Size = ExtractCarInformation(.CarInformation & "|", "Size")
					.NumberOfDoors = ExtractCarInformation(.CarInformation & "|", "Number of Doors")

					'HACK for now until we get the interface changed
					If .NumberOfDoors.ToSafeInt = 0 Then
						.NumberOfDoors = ExtractCarInformation(.CarInformation & "|", "Door")
					End If

					.FuelType = ExtractCarInformation(.CarInformation & "|", "Fuel Type")
					.AirConditioning = Intuitive.Functions.SafeBoolean(ExtractCarInformation(.CarInformation & "|", "Air Conditioning"))
					.TransmissionType = ExtractCarInformation(.CarInformation & "|", "Transmission Type")
					.PickUpInformation = oCarHireResult.PickUpInformation
					.RateServiceType = ExtractCarInformation(.CarInformation & "|", "Rate Service Type")
					.DropOffInformation = oCarHireResult.DropOffInformation
					.AdditionalInformation = oCarHireResult.AdditionalInformation

					.PickUpDepotID = oCarHireResult.PickUpDepotID
					.PickUpDepotName = oCarHireResult.PickUpDepotName
					.PickUpDate = oCarHireResult.PickUpDate
					.PickUpTime = oCarHireResult.PickUpTime
					.DropOffDepotID = oCarHireResult.DropOffDepotID
					.DropOffDepotName = oCarHireResult.DropOffDepotName
				    .DropOffDate = oCarHireResult.DropOffDate
				    .DropOffTime = oCarHireResult.DropOffTime

					'extras
					For Each oCarHireExtraResult As ivci.CarHire.SearchResponse.CarHireExtra In oCarHireResult.CarHireExtras

						Dim oCarHireExtra As New CarHireResults.CarHireExtra

						oCarHireExtra.ExtraBookingToken = oCarHireExtraResult.ExtraBookingToken
						oCarHireExtra.Description = oCarHireExtraResult.Description
						oCarHireExtra.Price = oCarHireExtraResult.Price
						oCarHireExtra.Mandatory = oCarHireExtraResult.Mandatory
						oCarHireExtra.IncludedInPrice = oCarHireExtraResult.IncludedInPrice
						oCarHireExtra.PrePaid = oCarHireExtraResult.PrePaid

						.CarHireExtras.Add(oCarHireExtra)
					Next

				End With

				'set up car hire option
				Dim oCarHireOption As New BasketCarHire.CarHireOption
				With oCarHireOption
					.BookingToken = oCarHire.BookingToken
					.Identifier = oCarHire.Identifier
					.VehicleDescription = oCarHire.VehicleDescription
					.PaxCapacity = oCarHire.PaxCapacity
					.BaggageCapacity = oCarHire.BaggageCapacity
					.LargeBaggageCapacity = oCarHireResult.LargeBaggageCapacity
					.SmallBaggageCapacity = oCarHireResult.SmallBaggageCapacity
					.Price = oCarHire.Price
					.Duration = oCarHire.Duration
					.ImageURL = oCarHire.ImageURL
					.CarInformation = oCarHire.CarInformation
					.Type = oCarHire.Type
					.Size = oCarHire.Size
					.NumberOfDoors = oCarHire.NumberOfDoors
					.FuelType = oCarHire.FuelType
					.AirConditioning = oCarHire.AirConditioning
					.TransmissionType = oCarHire.TransmissionType
					.PickUpInformation = oCarHire.PickUpInformation
					.DropOffInformation = oCarHire.DropOffInformation
					.AdditionalInformation = oCarHire.AdditionalInformation
					.PickUpDepotID = oCarHire.PickUpDepotID
					.PickUpDepotName = oCarHire.PickUpDepotName
					.PickUpDate = oCarHireResult.PickUpDate
					.PickUpTime = oCarHireResult.PickUpTime
					.DropOffDepotID = oCarHire.DropOffDepotID
					.DropOffDepotName = oCarHire.DropOffDepotName
					.DropOffDate = oCarHireResult.DropOffDate
					.DropOffTime = oCarHireResult.DropOffTime
					.CarHireContractCarTypeID = oCarHireResult.CarHireContractCarTypeID
					.Adults = BookingBase.SearchDetails.TotalAdults
					.Children = BookingBase.SearchDetails.TotalChildren
					.Infants = BookingBase.SearchDetails.TotalInfants
					.ChildAges = BookingBase.SearchDetails.AllChildAges

					For Each oCarHireExtra As CarHireExtra In oCarHire.CarHireExtras

						Dim oCarHireExtraOption As New BasketCarHire.CarHireExtraOption
						oCarHireExtraOption.ExtraBookingToken = oCarHireExtra.ExtraBookingToken
						oCarHireExtraOption.Description = oCarHireExtra.Description
						oCarHireExtraOption.Price = oCarHireExtra.Price
						oCarHireExtraOption.Mandatory = oCarHireExtra.Mandatory
						oCarHireExtraOption.IncludedInPrice = oCarHireExtra.IncludedInPrice
						oCarHireExtraOption.PrePaid = oCarHireExtra.PrePaid

						.CarHireExtras.Add(oCarHireExtraOption)

					Next

				End With

				'generate hash token
				oCarHire.CarHireOptionHashToken = oCarHireOption.GenerateHashToken()

				'add markup (after creating flight option)
				For Each oMarkup As BookingBase.Markup In aCarHireMarkups
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							oCarHire.MarkupAmount += oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							oCarHire.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
						Case BookingBase.Markup.eType.Percentage
							oCarHire.MarkupPercentage = oMarkup.Value
					End Select
				Next

				'add mark up to total
				oCarHire.Price += oCarHire.MarkupAmount
				oCarHire.Price *= (oCarHire.MarkupPercentage / 100) + 1

				oCarHireResults.CarHires.Add(oCarHire)

			Next

			'set total
			oCarHireResults.TotalCarHires = oCarHireResults.CarHires.Count

			'maintain stored car hire depots
			oCarHireResults.CarHireDepots = BookingBase.SearchDetails.CarHireResults.CarHireDepots
			oCarHireResults.TotalCarHireDepots = BookingBase.SearchDetails.CarHireResults.TotalCarHireDepots

			'save on session and sort
			BookingBase.SearchDetails.CarHireResults = oCarHireResults


		End Sub

		Public Shared Sub SaveAvailableDepots(ByVal oCarHireDepots As Generic.List(Of CarHireDepot))

			Dim oCarHireResults As New BookingCarHire.CarHireResults

			oCarHireResults.CarHireDepots = oCarHireDepots
			oCarHireResults.TotalCarHireDepots = oCarHireDepots.Count

			'save on session
			BookingBase.SearchDetails.CarHireResults = oCarHireResults

		End Sub

#End Region

#Region "Filter"

		Public Shared Function Filter(ByVal oFilters As Filters) As BookingCarHire.CarHireResults

			Dim oFilteredResults As New BookingCarHire.CarHireResults

			For Each oResult As BookingCarHire.CarHireResults.CarHire In BookingBase.SearchDetails.CarHireResults.CarHires
				FilterResult(oFilters, oResult, oFilteredResults)
			Next

			Return oFilteredResults

		End Function

		Public Shared Sub FilterResult(ByVal oFilters As Filters, ByRef oResult As BookingCarHire.CarHireResults.CarHire, ByRef oFilteredResults As BookingCarHire.CarHireResults)

			Dim bDisplay As Boolean = True

			If oFilters.Transmission <> oResult.TransmissionType AndAlso oFilters.Transmission <> "All" Then
				bDisplay = False
			End If

			If bDisplay AndAlso oFilters.AirConditioning <> oResult.AirConditioning.ToSafeString() AndAlso oFilters.AirConditioning <> "All" Then
				bDisplay = False
			End If

			If bDisplay Then
				oFilteredResults.CarHires.Add(oResult)
			End If

			oFilteredResults.TotalCarHires = oFilteredResults.CarHires.Count

		End Sub

		Public Class Filters
			Public Transmission As String = ""
			Public AirConditioning As String = ""
		End Class

#End Region

#Region "Sort Results"

		'Sort Results (string)
		Public Shared Sub SortResults(ByVal SortBy As String, ByVal SortOrder As String)

			'get enum values
			Dim eSortBy As eSortBy = Intuitive.Functions.SafeEnum(Of eSortBy)(SortBy)
			Dim eSortOrder As eSortOrder = Intuitive.Functions.SafeEnum(Of eSortOrder)(SortOrder)

			'sort results
			BookingCarHire.CarHireResults.SortResults(eSortBy, eSortOrder)

		End Sub

		'Sort Results (enum)
		Private Shared Sub SortResults(ByVal SortBy As eSortBy, ByVal SortOrder As eSortOrder)

			Select Case SortBy
				Case eSortBy.Price
					If SortOrder = eSortOrder.Ascending Then
						BookingBase.SearchDetails.CarHireResults.CarHires = BookingBase.SearchDetails.CarHireResults.CarHires.OrderBy(Function(o) o.Price).ToList()
					Else
						BookingBase.SearchDetails.CarHireResults.CarHires = BookingBase.SearchDetails.CarHireResults.CarHires.OrderByDescending(Function(o) o.Price).ToList()
					End If
			End Select

		End Sub

#End Region

#Region "Select Car Hire"

		Public Shared Function SelectCarHire(ByVal sIdentifier As String, Optional ByVal ClearSelectedCarHires As Boolean = True) As Boolean

			Dim bSuccess As Boolean = False

			If ClearSelectedCarHires Then
				For Each oCarHire As CarHire In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Selected = True)
					oCarHire.Selected = False
					For Each oExtra As CarHireExtra In oCarHire.CarHireExtras.Where(Function(x) x.Quantity > 0)
						oExtra.Quantity = 0
					Next
				Next
			End If

			For Each oCarHire As CarHire In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Identifier = sIdentifier)
				oCarHire.Selected = True
				bSuccess = True
				Exit For
			Next

			Return bSuccess

		End Function

		Public Shared Function DeSelectCarHires() As Boolean

			Dim bSuccess As Boolean = False

			For Each oCarHire As CarHire In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Selected = True)
				oCarHire.Selected = False
				For Each oExtra As CarHireExtra In oCarHire.CarHireExtras.Where(Function(x) x.Quantity > 0)
					oExtra.Quantity = 0
				Next
				bSuccess = True
			Next

			Return bSuccess

		End Function

#End Region

#Region "Update Car Hire Extras"

		'Each string in oExtraBookingTokenQuantityHashes should be of the form 'BookingToken###Quantity'.
		Public Shared Function UpdateCarHireExtras(ByVal oExtraBookingTokenQuantityHashes As Generic.List(Of String)) As Boolean

			For Each sExtraBookingTokenQuantityHash As String In oExtraBookingTokenQuantityHashes

				Dim iSplitIndex As Integer = InStr(sExtraBookingTokenQuantityHash, "###")
				Dim sExtraBookingToken As String = sExtraBookingTokenQuantityHash.Substring(0, iSplitIndex - 1)
				Dim iQuantity As Integer = sExtraBookingTokenQuantityHash.Substring(iSplitIndex + 2).ToSafeInt

				'For Each oExtra In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Selected = True).First.CarHireExtras.Where(Function(x) x.ExtraBookingToken = sExtraBookingToken)
				'	oExtra.Quantity = iQuantity
				'Next

				If BookingBase.SearchBasket.BasketCarHires.Count > 0 Then
					For Each oExtra As BasketCarHire.CarHireExtraOption In BookingBase.SearchBasket.BasketCarHires(0).CarHire.CarHireExtras.Where(Function(x) x.ExtraBookingToken = sExtraBookingToken)
						If oExtra.PrePaid = True AndAlso oExtra.IncludedInPrice = False Then
							BookingBase.SearchBasket.BasketCarHires(0).CarHire.Price += oExtra.Price * (iQuantity - oExtra.Quantity)
						End If
						oExtra.Quantity = iQuantity
					Next

				End If

			Next

			Return True

		End Function

#End Region

#Region "Toggle Car Hire Insurance"

		Public Shared Function ToggleCarHireInsurance(ByVal sIdentifier As String) As Boolean

			For Each oCarHire As CarHire In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Identifier = sIdentifier)
				If Not oCarHire.CarHireInsuranceQuote.BookingToken = "" Then
					oCarHire.CarHireInsuranceQuote.Selected = Not oCarHire.CarHireInsuranceQuote.Selected
					Return True
				End If
			Next
			Return False

		End Function

#End Region

#Region "Get Results"

		Public Function GetResultsXML() As XmlDocument

			'serialize to xml and return
			Dim oResultXML As XmlDocument = Serializer.Serialize(Me, True)
			Return oResultXML

		End Function

		Public Function GetSelectedCarHireXML() As XmlDocument

			Dim oSelectedCarHire As CarHire = BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Selected = True).First

			'serialize to xml
			Dim oSelectedCarHireXML As XmlDocument = Serializer.Serialize(oSelectedCarHire, True)

			'create an AddedToBasket node if needed
			For Each oBasketCarHire As BookingCarHire.BasketCarHire In BookingBase.Basket.BasketCarHires
				Dim oNodeCarHires As XmlNodeList = oSelectedCarHireXML.SelectNodes("CarHire")
				For Each oNodeCarHire As XmlNode In oNodeCarHires
					If oBasketCarHire.CarHire.Identifier.ToLower = XMLFunctions.SafeNodeValue(oNodeCarHire, "Identifier").ToSafeString.ToLower Then
						oNodeCarHire.InnerXml = oNodeCarHire.InnerXml & "<AddedToBasket>true</AddedToBasket>"
						Exit For
					End If
				Next
			Next

			Return oSelectedCarHireXML

		End Function

#Region "Get Single Car Hire"

		Public Function GetSingleCarHire(ByVal sIdentifier As String) As CarHire

			Dim oCarHire As CarHire = Me.CarHires.Where(Function(o) o.Identifier = sIdentifier).FirstOrDefault
			Return oCarHire

		End Function

#End Region

#End Region

#Region "Get Default Car Hire Per Flight"

		Public Function GetDefaultCarHirePerFlight(ByVal oFlights As Generic.List(Of FlightResultHandler.Flight)) _
	   As Dictionary(Of String, CarHireResults.CarHire)

			'sort car hire results
			Me.CarHires = Me.CarHires.OrderBy(Function(o) o.Price).ToList

			'build up dictionary
			Dim oDictionary As New Dictionary(Of String, CarHireResults.CarHire)
			For Each oFlight As FlightResultHandler.Flight In oFlights

				'get our depots
				Dim oDepots As Generic.List(Of Lookups.CarHireDepot) = BookingBase.Lookups.GetDepotsByAirportID(oFlight.ArrivalAirportID)

				'if we have depots get default car hire
				If oDepots.Count > 0 Then
					Dim oCarHire As CarHire = Me.GetDefaultCarHire(oDepots.FirstOrDefault.CarHireDepotID)
					If Not oCarHire Is Nothing AndAlso Not oDictionary.ContainsKey(oFlight.BookingToken) Then
						oDictionary.Add(oFlight.BookingToken, oCarHire)
					End If
				End If

			Next

			'return
			Return oDictionary

		End Function

		Public Function GetDefaultCarHire(ByVal CarHireDepotID As Integer) As CarHire

			'find the cheapest valid car hire based on the depot
			Dim oDefaultCarHire As New CarHire
			oDefaultCarHire = Me.CarHires.Where(Function(o) o.PickUpDepotID = CarHireDepotID).FirstOrDefault

			'return
			Return oDefaultCarHire

		End Function

#End Region

#Region "Car Hire"

		Public Class CarHire

			Public Property BookingToken As String
			Public Property Identifier As String
			Public Property VehicleDescription As String
			Public Property VehicleCode As String
			Public Property PaxCapacity As Integer
			Public Property AdultCapacity As Integer
			Public Property ChildCapacity As Integer
			Public Property BaggageCapacity As Integer
			Public Property LargeBaggageCapacity As Integer
			Public Property SmallBaggageCapacity As Integer
			Public Property Price As Decimal
			Public Property Duration As Integer
			Public Property ImageURL As String
			Public Property CarInformation As String
			Public Property Type As String
			Public Property Size As String
			Public Property NumberOfDoors As String
			Public Property FuelType As String
			Public Property AirConditioning As Boolean
			Public Property TransmissionType As String
			Public Property PickUpInformation As iVectorConnectInterface.CarHire.SearchResponse.DepotInformation
			Public Property DropOffInformation As iVectorConnectInterface.CarHire.SearchResponse.DepotInformation
			Public Property AdditionalInformation As String
			Public Property RateServiceType As String
			Public Property CarHireExtras As New Generic.List(Of CarHireExtra)
			Public Property CarHireInsuranceQuote As New CarHireInsuranceQuote
			Public Property CarHireOptionHashToken As String
			Public Property Selected As Boolean = False
			Public Property PickUpDepotID As Integer
			Public Property PickUpDepotName As String
			Public Property PickUpDate As Date
			Public Property PickUpTime As String
			Public Property DropOffDepotID As Integer
			Public Property DropOffDepotName As String
			Public Property DropOffDate As Date
			Public Property DropOffTime As String
			Public Property MarkupAmount As Decimal
			Public Property MarkupPercentage As Decimal
            Public Property Source As String
		End Class

		Public Class CarHireExtra

			Public Property ExtraBookingToken As String
			Public Property Description As String
			Public Property Price As Decimal
			Public Property Mandatory As Boolean
			Public Property IncludedInPrice As Boolean
			Public Property PrePaid As Boolean
			Public Property Quantity As Integer = 0

		End Class

		Public Class CarHireInsuranceQuote

			Public Property BookingToken As String = ""
			Public Property PremiumName As String = ""
			Public Property FullPremiumName As String = ""
			Public Property PremiumAmount As Decimal = 0
			Public Property ProviderLogoURL As String = ""
			Public Property InsuranceDescription As String = ""
			Public Property InsuranceRegulationText As String = ""
			Public Property TermsAndConditions As String = ""
			Public Property TermsAndConditionsURL As String = ""
			Public Property ConfirmationText As String = ""
			Public Property CoveredItems As New Generic.List(Of String)
			Public Property Selected As Boolean = False

		End Class

#End Region

#Region "Remove Markup"

		Public Sub RemoveMarkup()

			For Each oCarHire As CarHire In Me.CarHires

				oCarHire.Price -= oCarHire.MarkupAmount
				oCarHire.Price /= (oCarHire.MarkupPercentage / 100) + 1

				oCarHire.MarkupAmount = 0
				oCarHire.MarkupPercentage = 0

			Next

		End Sub

#End Region

#Region "Enums"

		Private Enum eSortBy
			Price
		End Enum

		Private Enum eSortOrder
			Ascending
			Descending
		End Enum

#End Region

	End Class

#End Region

#Region "Basket"

#Region "Add Car Hire"

	Public Shared Sub AddSelectedCarHireToBasket()

		For Each oSelectedCarHire As CarHireResults.CarHire In BookingBase.SearchDetails.CarHireResults.CarHires.Where(Function(o) o.Selected = True)

			Dim oCarHireOption As New BasketCarHire.CarHireOption
			With oCarHireOption
				.BaggageCapacity = oSelectedCarHire.BaggageCapacity
				.SmallBaggageCapacity = oSelectedCarHire.SmallBaggageCapacity
				.LargeBaggageCapacity = oSelectedCarHire.LargeBaggageCapacity
				.BookingToken = oSelectedCarHire.BookingToken
				.Identifier = oSelectedCarHire.Identifier
				.CarInformation = oSelectedCarHire.CarInformation
				.Type = oSelectedCarHire.Type
				.Size = oSelectedCarHire.Size
				.NumberOfDoors = oSelectedCarHire.NumberOfDoors
				.FuelType = oSelectedCarHire.FuelType
				.AirConditioning = oSelectedCarHire.AirConditioning
				.TransmissionType = oSelectedCarHire.TransmissionType
				.DropOffInformation = oSelectedCarHire.DropOffInformation
				.ImageURL = oSelectedCarHire.ImageURL
				.PaxCapacity = oSelectedCarHire.PaxCapacity
				.PickUpInformation = oSelectedCarHire.PickUpInformation
				.Price = oSelectedCarHire.Price
				.Duration = oSelectedCarHire.Duration
				.VehicleDescription = oSelectedCarHire.VehicleDescription
				For Each oCarHireExtra As CarHireResults.CarHireExtra In oSelectedCarHire.CarHireExtras
					Dim oCarHireExtraOption As New BasketCarHire.CarHireExtraOption
					With oCarHireExtraOption
						.Description = oCarHireExtra.Description
						.ExtraBookingToken = oCarHireExtra.ExtraBookingToken
						.IncludedInPrice = oCarHireExtra.IncludedInPrice
						.Mandatory = oCarHireExtra.Mandatory
						.PrePaid = oCarHireExtra.PrePaid
						.Price = oCarHireExtra.Price
						.Quantity = oCarHireExtra.Quantity
					End With
					.CarHireExtras.Add(oCarHireExtraOption)
				Next
				.CarHireInsuranceQuote.BookingToken = oSelectedCarHire.CarHireInsuranceQuote.BookingToken
				.CarHireInsuranceQuote.PremiumName = oSelectedCarHire.CarHireInsuranceQuote.PremiumName
				.CarHireInsuranceQuote.FullPremiumName = oSelectedCarHire.CarHireInsuranceQuote.FullPremiumName
				.CarHireInsuranceQuote.PremiumAmount = oSelectedCarHire.CarHireInsuranceQuote.PremiumAmount
				.CarHireInsuranceQuote.ProviderLogoURL = oSelectedCarHire.CarHireInsuranceQuote.ProviderLogoURL
				.CarHireInsuranceQuote.InsuranceDescription = oSelectedCarHire.CarHireInsuranceQuote.InsuranceDescription
				.CarHireInsuranceQuote.InsuranceRegulationText = oSelectedCarHire.CarHireInsuranceQuote.InsuranceRegulationText
				.CarHireInsuranceQuote.TermsAndConditions = oSelectedCarHire.CarHireInsuranceQuote.TermsAndConditions
				.CarHireInsuranceQuote.TermsAndConditionsURL = oSelectedCarHire.CarHireInsuranceQuote.TermsAndConditionsURL
				.CarHireInsuranceQuote.ConfirmationText = oSelectedCarHire.CarHireInsuranceQuote.ConfirmationText
				.CarHireInsuranceQuote.CoveredItems = oSelectedCarHire.CarHireInsuranceQuote.CoveredItems
				.CarHireInsuranceQuote.Selected = oSelectedCarHire.CarHireInsuranceQuote.Selected
			End With

			AddCarHireToBasket(oCarHireOption, True)

		Next

	End Sub

	'add car hire option (hash token)
	Public Shared Sub AddCarHireToBasket(ByVal HashToken As String, Optional ByVal ClearBasket As Boolean = True)

		'create car hire option from hash token
		Dim oCarHireOption As BasketCarHire.CarHireOption = BasketCarHire.CarHireOption.DeHashToken(Of BasketCarHire.CarHireOption)(HashToken)

		'add car hire option
		BookingCarHire.AddCarHireToBasket(oCarHireOption, ClearBasket)

	End Sub

	'add car hire option (car hire option)
	Public Shared Sub AddCarHireToBasket(ByVal CarHireOption As BasketCarHire.CarHireOption, Optional ByVal ClearBasket As Boolean = True)

		'clear current basket car hires
		If ClearBasket Then BookingBase.SearchBasket.BasketCarHires.Clear()

		'create basket car hire
		Dim oBasketCarHire As New BasketCarHire
		Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
		oBasketCarHire.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1

		oBasketCarHire.CarHire = CarHireOption

		'add to basket
		BookingBase.SearchBasket.BasketCarHires.Add(oBasketCarHire)

		'make sure we pre book again
		BookingBase.SearchBasket.PreBooked = False

	End Sub

#End Region

#Region "Remove Car Hire"

	Public Shared Function RemoveCarHireFromBasket() As Boolean

		Try
			BookingBase.SearchBasket.BasketCarHires.Clear()
			BookingBase.SearchBasket.PreBooked = False
			Return True
		Catch ex As Exception
			Return False
		End Try

	End Function

	'remove car hire option (hash token)
	Public Shared Sub RemoveCarHireFromBasket(ByVal HashToken As String)

		'create car hire option from hash token
		Dim oCarHireOption As BasketCarHire.CarHireOption = BasketCarHire.CarHireOption.DeHashToken(Of BasketCarHire.CarHireOption)(HashToken)

		For Each oCarHire As BasketCarHire In BookingBase.SearchBasket.BasketCarHires
			If oCarHire.CarHire.HashToken = oCarHireOption.HashToken Then
				BookingBase.SearchBasket.BasketCarHires.Remove(oCarHire)
				Exit For
			End If
		Next

		'make sure we pre book again
		BookingBase.SearchBasket.PreBooked = False

	End Sub

#End Region

#Region "Support Classes - BasketCarHire"

	Public Class BasketCarHire
		Public ComponentID As Integer
		Public CarHire As CarHireOption
		Public ContentXML As XmlDocument
		Public GuestIDs As New Generic.List(Of Integer)
		Public DriverIDs As New Generic.List(Of Integer)

		Public Property Markup As Decimal
			Get
				Dim nTotalMarkup As Decimal = 0
				For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.CarHire)
					Select Case oMarkup.Type
						Case BookingBase.Markup.eType.Amount
							nTotalMarkup += oMarkup.Value
						Case BookingBase.Markup.eType.AmountPP
							nTotalMarkup += oMarkup.Value * (Me.CarHire.Adults + Me.CarHire.Children)
						Case BookingBase.Markup.eType.Percentage
							nTotalMarkup += (oMarkup.Value * Me.CarHire.Price) / 100
					End Select
				Next

				Return nTotalMarkup
			End Get
			Set(value As Decimal)
				'require this to be serialised
			End Set
		End Property

		Public Class CarHireOption
			Inherits Utility.Hasher

			Public Property BookingToken As String
			Public Property Identifier As String
			Public Property VehicleDescription As String
			Public Property PaxCapacity As Integer
			Public Property BaggageCapacity As Integer
			Public Property LargeBaggageCapacity As Integer
			Public Property SmallBaggageCapacity As Integer
			Public Property Price As Decimal
			Public Property Duration As Integer
			Public Property ImageURL As String
			Public Property CarInformation As String
			Public Property Type As String
			Public Property Size As String
			Public Property NumberOfDoors As String
			Public Property FuelType As String
			Public Property AirConditioning As Boolean
			Public Property TransmissionType As String
			Public Property PickUpInformation As iVectorConnectInterface.CarHire.SearchResponse.DepotInformation
			Public Property DropOffInformation As iVectorConnectInterface.CarHire.SearchResponse.DepotInformation
			Public Property AdditionalInformation As String
			Public Property CarHireExtras As New Generic.List(Of CarHireExtraOption)
			Public Property CarHireInsuranceQuote As New CarHireInsuranceQuoteOption
			Public Property PickUpDepotID As Integer
			Public Property PickUpDepotName As String
			Public Property PickUpDate As Date
			Public Property PickUpTime As String
			Public Property DropOffDepotID As Integer
			Public Property DropOffDepotName As String
			Public Property DropOffDate As Date
			Public Property DropOffTime As String
			Public Property CarHireContractCarTypeID As Integer

			Public Adults As Integer
			Public Children As Integer
			Public Infants As Integer
			Public ChildAges As Generic.List(Of Integer)

		End Class

		Public Class CarHireExtraOption

			Public Property ExtraBookingToken As String
			Public Property Description As String
			Public Property Price As Decimal
			Public Property Mandatory As Boolean
			Public Property IncludedInPrice As Boolean
			Public Property PrePaid As Boolean
			Public Property Quantity As Integer

		End Class

		Public Class CarHireInsuranceQuoteOption

			Public Property BookingToken As String = ""
			Public Property PremiumName As String = ""
			Public Property FullPremiumName As String = ""
			Public Property PremiumAmount As Decimal = 0
			Public Property ProviderLogoURL As String = ""
			Public Property InsuranceDescription As String = ""
			Public Property InsuranceRegulationText As String = ""
			Public Property TermsAndConditions As String = ""
			Public Property TermsAndConditionsURL As String = ""
			Public Property ConfirmationText As String = ""
			Public Property CoveredItems As New Generic.List(Of String)
			Public Property Selected As Boolean = False

		End Class

#Region "Pre Book"

		Public Function CreatePreBookRequest() As ivci.CarHire.PreBookRequest

			Dim oPreBookRequest As New ivci.CarHire.PreBookRequest

			With oPreBookRequest
				If Me.CarHire.CarHireInsuranceQuote.Selected Then
					.BookingToken = Me.CarHire.CarHireInsuranceQuote.BookingToken
					.Insurance = True
				Else
					'uses Identifier in case we have returned to the basket page after prebook. Identifier isn't changed by prebook and represents all car hire information
					.BookingToken = Me.CarHire.Identifier
					.Insurance = False
				End If
				For Each oCarHireExtra As CarHireExtraOption In Me.CarHire.CarHireExtras.Where(Function(x) x.Quantity > 0)
					Dim oExtraPreBookRequest As New ivci.CarHire.PreBookRequest.CarHireExtra
					With oExtraPreBookRequest
						.ExtraToken = oCarHireExtra.ExtraBookingToken
						.Quantity = oCarHireExtra.Quantity
					End With
					.CarHireExtras.Add(oExtraPreBookRequest)
				Next

			End With

			Return oPreBookRequest

		End Function

#End Region

#Region "Book"

		Public Function CreateBookRequest() As ivci.CarHire.BookRequest

			Dim oBookRequest As New ivci.CarHire.BookRequest
			With oBookRequest
				.BookingToken = Me.CarHire.BookingToken
				.ExpectedTotal = Me.CarHire.Price

				'add the guests
				For Each oGuestDetail As ivci.Support.GuestDetail In BookingBase.Basket.GuestDetails
					.GuestIDs.Add(oGuestDetail.GuestID)
				Next
				.Drivers = Me.DriverIDs

			End With

			Return oBookRequest

		End Function

#End Region

	End Class

#End Region

#Region "Helper Functions"

	Public Shared Function ExtractCarInformation(ByVal sCarInformation As String, ByVal sRequiredData As String) As String

		Dim oMatch As Match = Regex.Match(sCarInformation, sRequiredData & ":([^|]*).*")

		Dim sReturnData As String = Intuitive.Functions.SafeString(oMatch.Groups(1).Value)

		Return sReturnData

	End Function

#End Region

#Region "Helper Classes"

	Public Class InformationPreBookReturn
		Public OK As Boolean = True
		Public Warning As New Generic.List(Of String)
		Public Response As New ivci.CarHire.PreBookResponse
	End Class

#End Region

#End Region



End Class
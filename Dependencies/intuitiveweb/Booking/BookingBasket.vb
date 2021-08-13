Imports System.Web.UI.WebControls
Imports ivci = iVectorConnectInterface
Imports Intuitive.Functions
Imports System.Xml

Public Class BookingBasket
	Implements Basket.Interfaces.IBasketVisitable

#Region "constructors"

    'force everything else to tell us it has explicitly instantiated
    Public Sub New(ExplicitInstantiation As Boolean)
        BasketProperties = New Basket.Collections.BasketList(Of BookingProperty.BasketProperty)(True)
        BasketFlights = New Basket.Collections.BasketList(Of BookingFlight.BasketFlight)(True)
    End Sub

    'ideally this would be private, we only want the empty constructor to identify serialization
    'it is public for backwards compatability
    Public Sub New()
        BasketProperties = New Basket.Collections.BasketList(Of BookingProperty.BasketProperty)
        BasketFlights = New Basket.Collections.BasketList(Of BookingFlight.BasketFlight)
    End Sub

#End Region

#Region "Properties"

	Public BookingReference As String
	Public ExternalReference As String

	Public TradeReference As String
	Public TradeContactID As Integer
	Public IncludePaymentDetails As Boolean = True
	Public AllowNoPayment As Boolean

	Public TrackingAffiliateID As Integer
	Public TrackingAffiliate As String

    Public WithEvents BasketProperties As Basket.Collections.BasketList(Of BookingProperty.BasketProperty)
    Public WithEvents BasketFlights As Basket.Collections.BasketList(Of BookingFlight.BasketFlight)


	Public BasketTransfers As New Generic.List(Of BookingTransfer.BasketTransfer)
	Public BasketExtras As New Generic.List(Of BookingExtra.BasketExtra)
	Public BasketCarHires As New Generic.List(Of BookingCarHire.BasketCarHire)

	Public BasketErrata As New Generic.List(Of ivci.Support.Erratum)
	Public BookingTags As New Generic.List(Of ivci.Support.BookingTag)

	Public LeadCustomer As New ivci.Support.LeadCustomerDetails
	Public CustomerPassword As String
	Public GuestDetails As New Generic.List(Of ivci.Support.GuestDetail)

	Public BookingAdjustments As New Generic.List(Of ivci.Basket.PreBookResponse.BookingAdjustment)

	Public PaymentsDue As New Generic.List(Of ivci.Support.PaymentDue)

	Public PaymentDetails As New ivci.Support.PaymentDetails
	Public PromoCode As String = ""
	Public PromoCodeDiscount As Decimal = 0
	Public PromoCodeUpdated As Boolean
	Public PromoCodePrevious As String = ""

	Public AmountDueToday As Decimal
	Public OutstandingAmount As Decimal
	Public PayDeposit As Boolean = False
    Public ResetBasketOnSearch as Boolean = False


	Public SurchargePercentage As Decimal '% used to calculate surcharges coming to us, not used on pwcc amount
	'Public SurchargeAmount As Decimal 'the surchage amount coming to us, to show on confirmation

	Public CreditCardTypes As New List(Of CreditCardType) 'valid card types as returned from the flight suppliers - only display these on the form if returned at pre book

	Public PreBooked As Boolean = False
	Public Booked As Boolean = False
	Public BookInProgress As Boolean = False
	Public PreBookResponse As ivci.Basket.PreBookResponse
	Public BookResponse As ivci.Basket.BookResponse

	Public PreBookTotalPrice As Decimal = 0	'set upon pre book response completion, used instead of component sums once me.prebooked = true
	Public FlightSupplierPaymentAmount As Decimal = 0 'not used for calculations, only to show correct figures on confirmation for PWCC bookings

	Public ComponentFailed As Boolean = False
	Public VehicleInfo As New VehicleInformation
	Public CustomXML As XmlDocument = Nothing

    Public ThreeDSecureHTML As String = ""

	Public Property AgentCommission As Decimal
		Get
			Dim nAgentCommission As Decimal = 0
			nAgentCommission += SafeDecimal(Me.BasketProperties.Sum(Function(oProperty) oProperty.RoomOptions.Sum(Function(oRoom) oRoom.TotalCommission)))
			nAgentCommission += Me.BasketFlights.Sum(Function(o) o.Flight.TotalCommission)
			nAgentCommission += Me.BasketTransfers.Sum(Function(o) o.Transfer.TotalCommission)
			Return nAgentCommission

		End Get
		Set(value As Decimal)

		End Set
	End Property

    Public Property TotalAmountDue As Decimal
        Get
            Return me.PaymentsDue.Sum(Function(paymentDue) paymentDue.Amount)
        End Get
        Set(value As Decimal)
        End Set
    End Property

	Public Property TotalPrice As Decimal
		Get
			Dim nTotalPrice As Decimal = 0

			'Sum properties
			nTotalPrice += SafeDecimal(Me.BasketProperties.Sum(Function(oProperty) oProperty.RoomOptions.Sum(Function(oRoom) oRoom.TotalPrice)))

			'Sum flights 
			nTotalPrice += SafeDecimal(Me.BasketFlights.Sum(Function(oFlight) oFlight.Flight.Price))

			'Sum Transfers
			nTotalPrice += SafeDecimal(Me.BasketTransfers.Sum(Function(oTransfer) oTransfer.Transfer.Price))

			'Sum Seat Maps
			nTotalPrice += SafeDecimal(Me.BasketFlights.Sum(Function(oFlight) oFlight.Flight.SeatMapCost))

			'Sum Extras
			nTotalPrice += SafeDecimal(Me.BasketExtras.Sum(Function(oExtra) oExtra.BasketExtraOptions.Sum(Function(oExtraItem) oExtraItem.TotalPrice)))

			'Sum car hires
			nTotalPrice += SafeDecimal(Me.BasketCarHires.Sum(Function(oCarHire) oCarHire.CarHire.Price))

			'Sum Booking Adjustments
			'not including PWCC amount
			'best way at present to identify these is empty type, parent type and calc basis
			Dim oAdjustments As IEnumerable(Of ivci.Basket.PreBookResponse.BookingAdjustment) = _
			 Me.BookingAdjustments.Where(Function(o) Not (o.AdjustmentType = "" AndAlso o.BookingAdjustmentTypeID = 0 _
			  AndAlso o.ParentType = "Flight" AndAlso o.CalculationBasis = "Amount Per Booking"))

			Dim nAdjustmentTotal As Decimal = SafeDecimal(oAdjustments.Sum(Function(o) o.AdjustmentAmount))

			nTotalPrice += nAdjustmentTotal

			Return nTotalPrice - Me.PromoCodeDiscount
		End Get
		Set(value As Decimal)

		End Set
	End Property



	Public Property TotalCommission As Decimal
		Get
			Dim nTotalCommission As Decimal = 0

			'Sum properties, flights, transfers and extras
			nTotalCommission += SafeDecimal(Me.BasketProperties.Sum(Function(oProperty) oProperty.RoomOptions.Sum(Function(oRoom) oRoom.TotalCommission)))
			nTotalCommission += SafeDecimal(Me.BasketFlights.Sum(Function(oFlight) oFlight.Flight.TotalCommission))
			nTotalCommission += SafeDecimal(Me.BasketTransfers.Sum(Function(oTransfer) oTransfer.Transfer.TotalCommission))
			nTotalCommission += SafeDecimal(Me.BasketExtras.Sum(Function(oExtra) oExtra.BasketExtraOptions.Sum(Function(oExtraOption) oExtraOption.TotalCommission)))

			Return nTotalCommission
		End Get
		Set(value As Decimal)

		End Set
	End Property


	Public Property TotalMarkup As Decimal
		Get
			Dim nTotalMarkup As Decimal = 0

			'Sum properties
			nTotalMarkup += SafeDecimal(Me.BasketProperties.Sum(Function(oProperty) oProperty.RoomOptions.Sum(Function(oRoom) oRoom.Markup)))

			'Sum flights 
			nTotalMarkup += SafeDecimal(Me.BasketFlights.Sum(Function(oFlight) oFlight.Markup))

			'Sum Transfers
			nTotalMarkup += SafeDecimal(Me.BasketTransfers.Sum(Function(oTransfer) oTransfer.Markup))

			'Sum Extras
			nTotalMarkup += SafeDecimal(Me.BasketExtras.Sum(Function(oExtra) oExtra.Markup))

            'Sum Car Hires
            nTotalMarkup += SafeDecimal(Me.BasketCarHires.Sum(Function(oCarHire) oCarHire.Markup))

			For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Basket)
				Select Case oMarkup.Type
					Case BookingBase.Markup.eType.Amount
						nTotalMarkup += oMarkup.Value
					Case BookingBase.Markup.eType.AmountPP
						nTotalMarkup += oMarkup.Value * (Me.TotalAdults + Me.TotalChildren)
					Case BookingBase.Markup.eType.Percentage
						nTotalMarkup += (oMarkup.Value * Me.TotalPrice) / 100
				End Select
			Next

			Return nTotalMarkup

		End Get
		Set(value As Decimal)
		End Set
	End Property

	Public Property TotalAdults As Integer
		Get
			Dim iAdults As Integer = 0
			For Each oGuest As ivci.Support.GuestDetail In Me.GuestDetails
				If oGuest.Type = "Adult" Then iAdults += 1
			Next
			Return iAdults
		End Get
		Set(value As Integer)
		End Set
	End Property


	Public Property TotalChildren As Integer
		Get
			Dim iChildren As Integer = 0
			For Each oGuest As ivci.Support.GuestDetail In Me.GuestDetails
				If oGuest.Type = "Child" Then iChildren += 1
			Next
			Return iChildren
		End Get
		Set(value As Integer)
		End Set
	End Property

	Public Property TotalInfants As Integer
		Get
			Dim iInfants As Integer = 0
			For Each oGuest As ivci.Support.GuestDetail In Me.GuestDetails
				If oGuest.Type = "Infants" Then iInfants += 1
			Next
			Return iInfants
		End Get
		Set(value As Integer)
		End Set
	End Property


	Public ReadOnly Property FirstDepartureDate As DateTime
		Get
			Dim dFirstDepartureDate As DateTime = Now.AddYears(1000)

			For Each oBasketProperty As BookingProperty.BasketProperty In Me.BasketProperties
				If oBasketProperty.RoomOptions.Min(Of DateTime)(Function(o) o.ArrivalDate) < dFirstDepartureDate Then
					dFirstDepartureDate = oBasketProperty.RoomOptions.Min(Of DateTime)(Function(o) o.ArrivalDate)
				End If
			Next

			For Each oBasketFlight As BookingFlight.BasketFlight In Me.BasketFlights
				If oBasketFlight.Flight.OutboundDepartureDate < dFirstDepartureDate Then
					dFirstDepartureDate = oBasketFlight.Flight.OutboundDepartureDate
				End If
			Next

			For Each oBasketTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers
				If oBasketTransfer.Transfer.DepartureDate < dFirstDepartureDate Then
					dFirstDepartureDate = oBasketTransfer.Transfer.DepartureDate
				End If
			Next

			For Each oBasketExtra As BookingExtra.BasketExtra In Me.BasketExtras
				If oBasketExtra.BasketExtraOptions(0).StartDate < dFirstDepartureDate Then
					dFirstDepartureDate = oBasketExtra.BasketExtraOptions(0).StartDate
				End If
			Next

			Return dFirstDepartureDate
		End Get
	End Property


	Public ReadOnly Property XML(Optional ByVal AddContent As Boolean = True) As System.Xml.XmlDocument
		Get

			'serialize and return
			Dim oBasketXML As XmlDocument = Serializer.Serialize(Me, True)
			Return oBasketXML

		End Get
	End Property


	Public ReadOnly Property TotalComponents As Integer
		Get

			Dim iTotalComponents As Integer = 0
			iTotalComponents += Me.BasketProperties.Count
			iTotalComponents += Me.BasketFlights.Count
			iTotalComponents += Me.BasketTransfers.Count
			iTotalComponents += Me.BasketExtras.Count
			iTotalComponents += Me.BasketCarHires.Count

			Return iTotalComponents

		End Get
	End Property



#End Region

#Region "Event handlers"

	Private _SearchExtras As Boolean = True
	Public Property SearchExtras As Boolean
		Get
			Return _SearchExtras
		End Get
		Set(value As Boolean)
			_SearchExtras = value
		End Set
	End Property

	Public Sub BasketPropertiesChanged() Handles BasketProperties.ListChangedEvent
		Me._SearchExtras = True
		If BookingBase.Params.ClearExtrasOnFlightOrHotelChange Then
			Me.BasketExtras.Clear()
		End If
	End Sub

	Public Sub BasketFlightsChanged() Handles BasketFlights.ListChangedEvent
		Me._SearchExtras = True
		If BookingBase.Params.ClearExtrasOnFlightOrHotelChange Then
			Me.BasketExtras.Clear()
		End If
	End Sub

#End Region

#Region "Create Booking Reference"

	Public Sub CreateBookingReference()

		Dim oRequest As New ivci.CreateBookingReferenceRequest
		Dim oResponse As New ivci.CreateBookingReferenceResponse

		Try

			'Add the login details 
			oRequest.LoginDetails = BookingBase.IVCLoginDetails


			'Add the lead guest 
			oRequest.CustomerDetails = Me.LeadCustomer


			'Send the request
			Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
			 Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.CreateBookingReferenceResponse)(oRequest)


			If oIVCReturn.Success Then

				'Get the response object
				oResponse = CType(oIVCReturn.ReturnObject, ivci.CreateBookingReferenceResponse)

				'Set the bookingreference
				Me.BookingReference = Trim(oResponse.BookingReference)

			End If

		Catch ex As Exception
			FileFunctions.AddLogEntry("iVectorConnect/Book", "BookingReferenceException", ex.ToString)
		End Try

	End Sub

#End Region


#Region "Prebook"

	Public Function PreBook(Optional ByVal oOverrideLoginDetails As LoginDetails = Nothing, Optional ByVal bLog As Boolean? = Nothing) As PreBookReturn

		Dim oPreBookReturn As New PreBookReturn

		If Not Me.PreBooked Then

			'clear existing errata
			Me.BasketErrata.Clear()

			'create pre book request
			Dim oBasketPreBookRequest As New iVectorConnectInterface.Basket.PreBookRequest

			Try

                If oOverrideLoginDetails Is Nothing Then
				'Add the login details 
				oBasketPreBookRequest.LoginDetails = BookingBase.IVCLoginDetails
                Else
                    oBasketPreBookRequest.LoginDetails = oOverrideLoginDetails
                End If

				'set up each set of pre book requests and add to the basket request

				'property
				For Each oBasketProperty As BookingProperty.BasketProperty In Me.BasketProperties
					oBasketPreBookRequest.PropertyBookings.Add(oBasketProperty.CreatePreBookRequest())
				Next


				'flight
				For Each oFlight As BookingFlight.BasketFlight In Me.BasketFlights
					oBasketPreBookRequest.FlightBookings.AddRange(oFlight.CreatePreBookRequest(False, True))
				Next


				'transfers
				For Each oBasketTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers
					oBasketPreBookRequest.TransferBookings.Add(oBasketTransfer.CreatePreBookRequest())
				Next


				'extras
				For Each oBasketExtra As BookingExtra.BasketExtra In Me.BasketExtras
					oBasketPreBookRequest.ExtraBookings.Add(oBasketExtra.CreatePreBookRequest())
				Next

				'car hire
				For Each oBasketCarHire As BookingCarHire.BasketCarHire In Me.BasketCarHires
					oBasketPreBookRequest.CarHireBookings.Add(oBasketCarHire.CreatePreBookRequest())
				Next


				'Send request to iVectorConnect
				Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
				  Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.Basket.PreBookResponse)(oBasketPreBookRequest, True)


				If oIVCReturn.Success Then

					'Get the prebook response 
					Dim oPreBookResponse As iVectorConnectInterface.Basket.PreBookResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.Basket.PreBookResponse)

					'Check each component was successful 
					For Each oPropertyBooking As iVectorConnectInterface.Property.PreBookResponse In oPreBookResponse.PropertyBookings
						If Not oPropertyBooking.ReturnStatus.Success Then
							oPreBookReturn.OK = False
							oPreBookReturn.Warnings.Add("Unable to reserve hotel")
						End If
						If oPropertyBooking.Errata.Count > 0 Then Me.BasketErrata.AddRange(oPropertyBooking.Errata)
					Next

					For Each oFlightBooking As iVectorConnectInterface.Flight.PreBookResponse In oPreBookResponse.FlightBookings
						If Not oFlightBooking.ReturnStatus.Success Then
							oPreBookReturn.OK = False
							oPreBookReturn.Warnings.Add("Unable to reserve flight")
						End If
					Next

					For Each oTransferBooking As iVectorConnectInterface.Transfer.PreBookResponse In oPreBookResponse.TransferBookings
						If Not oTransferBooking.ReturnStatus.Success Then
							oPreBookReturn.OK = False
							oPreBookReturn.Warnings.Add("Unable to reserve transfer")
						End If
					Next

					For Each oExtraBooking As iVectorConnectInterface.Extra.PreBookResponse In oPreBookResponse.ExtraBookings
						If Not oExtraBooking.ReturnStatus.Success Then
							oPreBookReturn.OK = False
							oPreBookReturn.Warnings.Add("Unable to reserve extra")
						End If
					Next

					For Each oCarHireBooking As iVectorConnectInterface.CarHire.PreBookResponse In oPreBookResponse.CarHireBookings
						If Not oCarHireBooking.ReturnStatus.Success Then
							oPreBookReturn.OK = False
							oPreBookReturn.Warnings.Add("Unable to reserve car hire")
						End If
					Next


					If oPreBookReturn.OK Then

						'Save it  
						Me.PreBookResponse = oPreBookResponse


						'Set it as prebooked
						Me.PreBooked = True


						'Get the total price and set on the basket
						oPreBookReturn.TotalPrice = oPreBookResponse.TotalPrice
						Me.PreBookTotalPrice = oPreBookResponse.TotalPrice
						Me.FlightSupplierPaymentAmount = oPreBookResponse.PaymentAmountDetail.FlightSupplierPaymentAmount


						'Update the booking tokens and save payments due
						Dim oPaymentsDue As New Generic.List(Of iVectorConnectInterface.Support.PaymentDue)


						'set valid credit card types as returned from the pre book response
						For Each oCreditCard As ivci.Support.CreditCardFeeDetail In oPreBookResponse.PaymentAmountDetail.CreditCardFeeDetails
							Dim oCreditCardType As New CreditCardType
							With oCreditCardType
								.Name = BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.CardType, oCreditCard.CreditCardTypeID)
								.CreditCardTypeID = oCreditCard.CreditCardTypeID
								.FlightSupplierFee = oCreditCard.FlightSupplierFee



								.SurchargePercentage = Functions.SafeDecimal(BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.CardSurcharge,
						   oCreditCard.CreditCardTypeID))
							End With
							Me.CreditCardTypes.Add(oCreditCardType)
						Next


						'property
						Dim iComponentCount As Integer = 0
						For Each oProperty As iVectorConnectInterface.Property.PreBookResponse In oPreBookResponse.PropertyBookings

							Dim nPriceChange As Decimal = oProperty.TotalPrice - Me.BasketProperties(iComponentCount).RoomOptions.Sum(Function(o) o.TotalPrice)
							Dim iRooms As Integer = Me.BasketProperties(iComponentCount).RoomOptions.Count()

							'update token
							For Each oRoomOption As BookingProperty.BasketProperty.BasketPropertyRoomOption In Me.BasketProperties(iComponentCount).RoomOptions
								oRoomOption.BookingToken = oProperty.BookingToken
								oRoomOption.TotalPrice += nPriceChange / iRooms
							Next

							'add payments
							oPaymentsDue.AddRange(oProperty.PaymentsDue)

							'Terms And Conditions
							Me.BasketProperties(iComponentCount).TermsAndConditions = oProperty.TermsAndConditions
							Me.BasketProperties(iComponentCount).TermsAndConditionsURL = oProperty.TermsAndConditionsURL

							iComponentCount += 1

						Next


						'flight
						iComponentCount = 0
						Dim bReturnMultiCarrier As Boolean = False
						For Each oFlight As iVectorConnectInterface.Flight.PreBookResponse In oPreBookResponse.FlightBookings

							If Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.BookingToken <> "" AndAlso bReturnMultiCarrier Then

								'save old bookingtoken in case we need it for baggage
								If String.IsNullOrEmpty(Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.hlpSearchBookingToken) Then
									Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.hlpSearchBookingToken = Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.BookingToken
								End If

								'update token
								Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.BookingToken = oFlight.BookingToken

								'update price
								Me.BasketFlights(iComponentCount).Flight.Price += oFlight.TotalPrice

								Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.Price = oFlight.TotalPrice

								'update total commission
								Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.TotalCommission = oFlight.TotalCommission

								'update seat maps
								Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.SeatMapCost = oFlight.SeatMapCost

								'update flight extras
								For Each oFlightExtra As iVectorConnectInterface.Flight.PreBookResponse.Extra In oFlight.Extras
									Dim sExtraBookingToken As String = oFlightExtra.ExtraBookingToken
									Dim oExistingExtra As BookingFlight.BasketFlight.BasketFlightExtra = Me.BasketFlights(iComponentCount).ReturnMultiCarrierFlightExtras.FirstOrDefault(Function(oExtra) oExtra.ExtraBookingToken = sExtraBookingToken)
									If oExistingExtra Is Nothing Then
										Dim oBasketFlightExtra As New BookingFlight.BasketFlight.BasketFlightExtra
										oBasketFlightExtra.UpdateExtraDetails(oFlightExtra)
										Me.BasketFlights(iComponentCount).ReturnMultiCarrierFlightExtras.Add(oBasketFlightExtra)
									Else
										oExistingExtra.UpdateExtraDetails(oFlightExtra)
									End If
								Next

								'add payments
								oPaymentsDue.AddRange(oFlight.PaymentsDue)

								'Terms And Conditions
								Me.BasketFlights(iComponentCount).ReturnMultiCarrierTermsAndConditions = oFlight.TermsAndConditions
								Me.BasketFlights(iComponentCount).ReturnMultiCarrierTermsAndConditionsURL = oFlight.TermsAndConditionsURL


							Else

							'save old bookingtoken in case we need it for baggage
							If String.IsNullOrEmpty(Me.BasketFlights(iComponentCount).Flight.hlpSearchBookingToken) Then
								Me.BasketFlights(iComponentCount).Flight.hlpSearchBookingToken = Me.BasketFlights(iComponentCount).Flight.BookingToken
							End If

							'update token
							Me.BasketFlights(iComponentCount).Flight.BookingToken = oFlight.BookingToken

							'update price
							Me.BasketFlights(iComponentCount).Flight.Price = oFlight.TotalPrice

								'update price
								Me.BasketFlights(iComponentCount).Flight.OutboundPrice = oFlight.TotalPrice

							'update total commission
							Me.BasketFlights(iComponentCount).Flight.TotalCommission = oFlight.TotalCommission

							'update seat maps
							Me.BasketFlights(iComponentCount).Flight.SeatMapCost = oFlight.SeatMapCost

                            'update included baggage
                            Me.BasketFlights(iComponentCount).Flight.IncludedBaggageAllowance = oFlight.IncludedBaggageAllowance
                            Me.BasketFlights(iComponentCount).Flight.IncludedBaggageText = oFlight.IncludedBaggageText

							'update flight extras
							For Each oFlightExtra As iVectorConnectInterface.Flight.PreBookResponse.Extra In oFlight.Extras
								Dim sExtraBookingToken As String = oFlightExtra.ExtraBookingToken
								Dim oExistingExtra As BookingFlight.BasketFlight.BasketFlightExtra = Me.BasketFlights(iComponentCount).BasketFlightExtras.FirstOrDefault(Function(oExtra) oExtra.ExtraBookingToken = sExtraBookingToken)
								If oExistingExtra Is Nothing Then
									Dim oBasketFlightExtra As New BookingFlight.BasketFlight.BasketFlightExtra
									oBasketFlightExtra.UpdateExtraDetails(oFlightExtra)
									Me.BasketFlights(iComponentCount).BasketFlightExtras.Add(oBasketFlightExtra)
								Else
									oExistingExtra.UpdateExtraDetails(oFlightExtra)
								End If
							Next

							'add payments
							oPaymentsDue.AddRange(oFlight.PaymentsDue)

							'Terms And Conditions
							Me.BasketFlights(iComponentCount).TermsAndConditions = oFlight.TermsAndConditions
							Me.BasketFlights(iComponentCount).TermsAndConditionsURL = oFlight.TermsAndConditionsURL

							End If

							If Me.BasketFlights(iComponentCount).Flight.ReturnMultiCarrierDetails.BookingToken <> "" AndAlso Not bReturnMultiCarrier Then
								bReturnMultiCarrier = True
							Else
								bReturnMultiCarrier = False
								iComponentCount += 1
							End If
						Next


						'transfer
						iComponentCount = 0
						For Each oTransfer As iVectorConnectInterface.Transfer.PreBookResponse In oPreBookResponse.TransferBookings

							If String.IsNullOrEmpty(Me.BasketTransfers(iComponentCount).Transfer.hlpSearchBookingToken) Then
								Me.BasketTransfers(iComponentCount).Transfer.hlpSearchBookingToken = Me.BasketTransfers(iComponentCount).Transfer.BookingToken
							End If

							'update token
							Me.BasketTransfers(iComponentCount).Transfer.BookingToken = oTransfer.BookingToken
							Me.BasketTransfers(iComponentCount).Transfer.Price = oTransfer.TotalPrice
							Me.BasketTransfers(iComponentCount).Transfer.TotalCommission = oTransfer.TotalCommission
							iComponentCount += 1


							'add payments
							oPaymentsDue.AddRange(oTransfer.PaymentsDue)


						Next


						'Update extras booking token
						iComponentCount = 0
						For Each oExtra As iVectorConnectInterface.Extra.PreBookResponse In oPreBookResponse.ExtraBookings

							'1. classic 
							If oExtra.BookingToken <> Nothing Then


								For Each oExtraOption As BookingExtra.BasketExtra.BasketExtraOption In Me.BasketExtras(iComponentCount).BasketExtraOptions

									'Store the original pre-prebook booking token
									If String.IsNullOrEmpty(oExtraOption.hlpSearchBookingToken) Then
										oExtraOption.hlpSearchBookingToken = oExtraOption.BookingToken
									End If

									oExtraOption.BookingToken = oExtra.BookingToken
									oExtraOption.TotalPrice = oExtra.TotalPrice
									oExtraOption.TotalCommission = oExtra.TotalCommission
								Next


							End If

							'2. with options 
							For Each oExtraOption As ivci.Extra.PreBookResponse.ExtraOption In oExtra.ExtraOptions
								For Each oBasketExtraOption As BookingExtra.BasketExtra.BasketExtraOption In Me.BasketExtras(iComponentCount).BasketExtraOptions
									oBasketExtraOption.BookingToken = oExtraOption.BookingToken
                                    oBasketExtraOption.ExtraCategoryDescription = oExtraOption.ExtraCategoryDescription
								Next                                
							Next

							Me.BasketExtras(iComponentCount).BookingQuestions.Clear()
							Me.BasketExtras(iComponentCount).BookingQuestions.AddRange(oExtra.BookingQuestions)

							iComponentCount += 1

							'add payments
							oPaymentsDue.AddRange(oExtra.PaymentsDue)
						Next


						'car hire
						iComponentCount = 0
						For Each oCarHire As iVectorConnectInterface.CarHire.PreBookResponse In oPreBookResponse.CarHireBookings

							'update token
							Me.BasketCarHires(iComponentCount).CarHire.BookingToken = oCarHire.BookingToken
							Me.BasketCarHires(iComponentCount).CarHire.Price = oCarHire.TotalPrice
							iComponentCount += 1


							'add payments
							oPaymentsDue.AddRange(oCarHire.PaymentsDue)
						Next



						'booking adjustments

						'clear adjustments
						Me.BookingAdjustments = New Generic.List(Of ivci.Basket.PreBookResponse.BookingAdjustment)

						'loop through prebook adjustments
						iComponentCount = 0
						For Each oPreBookBookingAdjustment As iVectorConnectInterface.Basket.PreBookResponse.BookingAdjustment In oPreBookResponse.BookingAdjustments

							'add adjustment
							Dim oNewBookingAdjustment As New ivci.Basket.PreBookResponse.BookingAdjustment
							With oNewBookingAdjustment
								.AdjustmentType = oPreBookBookingAdjustment.AdjustmentType
								.AdjustmentAmount = oPreBookBookingAdjustment.AdjustmentAmount
								.CalculationBasis = oPreBookBookingAdjustment.CalculationBasis
								.ParentType = oPreBookBookingAdjustment.ParentType
							End With

							Me.BookingAdjustments.Add(oNewBookingAdjustment)


							iComponentCount += 1


						Next


						'calculate payment plan

						'Reset the value of the amount due today so that it doesn't keep adding if called more than once
						Me.AmountDueToday = 0
						Me.PaymentsDue.Clear()

						For Each oPaymentDue As ivci.Support.PaymentDue In oPaymentsDue.OrderBy(Function(o) o.DateDue)

							'set date due / outstanding amount
							If oPaymentDue.DateDue <= Now Then
								Me.AmountDueToday += oPaymentDue.Amount
							Else
								Me.OutstandingAmount += oPaymentDue.Amount
							End If


							'add payment due to basket, if date already exists add the amount
							If Me.PaymentsDue.Where(Function(o) o.DateDue = oPaymentDue.DateDue).Count > 0 Then
								Dim oBasketPaymentDue As ivci.Support.PaymentDue = Me.PaymentsDue.Where(Function(o) o.DateDue = oPaymentDue.DateDue).FirstOrDefault
								oBasketPaymentDue.Amount += oPaymentDue.Amount
							Else
								Me.PaymentsDue.Add(oPaymentDue)
							End If

						Next

						'Add seat reservation payments to amount due
						Me.AmountDueToday += SafeDecimal(Me.BasketFlights.Sum(Function(oFlight) oFlight.Flight.SeatMapCost))

						'add the flight supplier payment amount to amount due today as this must always be paid
						Me.AmountDueToday += Me.FlightSupplierPaymentAmount

						'add the booking adjustment total as this must always be paid
						Me.AmountDueToday += Me.BookingAdjustments.Sum(Function(oAdjustment) oAdjustment.AdjustmentAmount)

					End If

				Else
					oPreBookReturn.OK = False
					oPreBookReturn.Warnings.AddRange(oIVCReturn.Warning)
				End If



                If not bLog.HasValue then
                    bLog = BookingBase.LogAllXML
                End If

				If bLog Then

					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.NetworkLatency = oIVCReturn.NetworkLatency
						.Type = BookingSearch.RequestInfoType.BasketPreBook
					End With

					WebSupportToolbar.AddUniqueLog(oRequestInfo)
				End If


			Catch ex As Exception
				oPreBookReturn.OK = False
				oPreBookReturn.Warnings.Add(ex.ToString)
			End Try


		End If


		Return oPreBookReturn


	End Function



#End Region


#Region "Book"

	Public Function Book() As BookReturn

		Dim oBookReturn As New BookReturn
		Dim oBasketBookRequest As New iVectorConnectInterface.Basket.BookRequest

		Try
			If Me.BookInProgress Then Throw New Exception("Basket is being booked already")
			Me.BookInProgress = True

			If Not Me.Booked Then

				'remove unused guests
				Me.RemoveUnusedGuests()


				'Add the login details 
				oBasketBookRequest.LoginDetails = BookingBase.IVCLoginDetails


				'Booking reference if we have one 
				If Me.BookingReference <> "" Then
					oBasketBookRequest.BookingReference = Me.BookingReference
				End If

				'external reference
				oBasketBookRequest.ExternalReference = Me.ExternalReference


				'trade details
				oBasketBookRequest.TradeContactID = Me.TradeContactID
				oBasketBookRequest.TradeReference = Me.TradeReference


				'affiliate
				oBasketBookRequest.AffiliateID = Me.TrackingAffiliateID


				'Add any promocodes
				If Me.PromoCode <> "" Then oBasketBookRequest.PromotionalCode = Me.PromoCode


				'Add the lead guest 
				oBasketBookRequest.LeadCustomer = Me.LeadCustomer


				'set customer password
				oBasketBookRequest.Password = Me.CustomerPassword



				'Set up each set of book requests to add to the basket book request

				'properties
				For Each oProperty As BookingProperty.BasketProperty In Me.BasketProperties
					oBasketBookRequest.PropertyBookings.Add(oProperty.CreateBookRequest())
				Next


				'flight
				For Each oFlight As BookingFlight.BasketFlight In Me.BasketFlights
					oBasketBookRequest.FlightBookings.AddRange(oFlight.CreateBookRequest())
				Next


				'transfers
				For Each oTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers
					oBasketBookRequest.TransferBookings.Add(oTransfer.CreateBookRequest())
				Next


				'Extras
				For Each oExtra As BookingExtra.BasketExtra In Me.BasketExtras
					oBasketBookRequest.ExtraBookings.Add(oExtra.CreateBookRequest())
				Next

				'Car Hire
				For Each oCarHire As BookingCarHire.BasketCarHire In Me.BasketCarHires
					oBasketBookRequest.CarHireBookings.Add(oCarHire.CreateBookRequest())
				Next


				'List of guests - Note - this must come after greating extra book requests it may add more guests to the basket
				oBasketBookRequest.GuestDetails.AddRange(Me.GuestDetails)


				'Add the payment details
				If Me.IncludePaymentDetails Then oBasketBookRequest.Payment = Me.PaymentDetails Else oBasketBookRequest.Payment = Nothing


				'If we don't want to take payment now, still calculate booking totals for outstanding amount
				If Me.AllowNoPayment Then
					oBasketBookRequest.ForceCalculateBookingTotals = True
				End If


				'Add any booking tags
				If Me.BookingTags.Count > 0 Then
					oBasketBookRequest.BookingTags = Me.BookingTags
				End If


				'Send request to iVectorConnect
				Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
				   Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.Basket.BookResponse)(oBasketBookRequest, True, 180, OverrideServiceURL:=BookingBase.Params.BookingServiceURL)


				If oIVCReturn.Success Then

					'Get the book response 
					Dim oBookResponse As iVectorConnectInterface.Basket.BookResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.Basket.BookResponse)

					'set return status
					oBookReturn.OK = oBookResponse.ReturnStatus.Success

					'Return the booking reference
					oBookReturn.BookingReference = oBookResponse.BookingReference

					'return property booking supplier reference
					For Each oPropertyBookResponse As ivci.Property.BookResponse In oBookResponse.PropertyBookings
						Dim oPropertyBooking As New BookReturnComponent

						If Not oPropertyBookResponse.SupplierDetails Is Nothing Then
							oPropertyBooking.SupplierReference = oPropertyBookResponse.SupplierDetails.SupplierReference
						End If

						oBookReturn.PropertyBookings.Add(oPropertyBooking)
					Next

					'Save it  
					Me.BookResponse = oBookResponse
					Me.BookingReference = oBookReturn.BookingReference

					'Set the booked flag
					Me.Booked = oBookReturn.OK

					'check if any property components failed
					For Each oPropertyBooking As iVectorConnectInterface.Property.BookResponse In oBookResponse.PropertyBookings
						If Not oPropertyBooking.ReturnStatus.Success Then
							Me.ComponentFailed = True
							oBookReturn.ComponentFailed = True
						End If
					Next

					'check if any transfer components failed
					For Each oTransferBookings As iVectorConnectInterface.Transfer.BookResponse In oBookResponse.TransferBookings
						If Not oTransferBookings.ReturnStatus.Success Then
							Me.ComponentFailed = True
							oBookReturn.ComponentFailed = True
						End If
					Next

					'check if any flight components failed
					For Each oFlightBooking As iVectorConnectInterface.Flight.BookResponse In oBookResponse.FlightBookings
						If Not oFlightBooking.ReturnStatus.Success Then
							Me.ComponentFailed = True
							oBookReturn.ComponentFailed = True
						End If
					Next

					'check if any extra components failed
					For Each oExtraBooking As iVectorConnectInterface.Extra.BookResponse In oBookResponse.ExtraBookings
						If Not oExtraBooking.ReturnStatus.Success Then
							Me.ComponentFailed = True
							oBookReturn.ComponentFailed = True
						End If
					Next

					'check if any car hire components failed
					For Each oCarHireBooking As iVectorConnectInterface.CarHire.BookResponse In oBookResponse.CarHireBookings
						If Not oCarHireBooking.ReturnStatus.Success Then
							Me.ComponentFailed = True
							oBookReturn.ComponentFailed = True
						End If
					Next

				Else
					oBookReturn.OK = False
					oBookReturn.Warnings.AddRange(oIVCReturn.Warning)
					If oIVCReturn.Timeout Then
						oBookReturn.Warnings.Add(BookWarning.Timeout)
					End If
				End If

				If BookingBase.LogAllXML Then
					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.Type = BookingSearch.RequestInfoType.BasketBook
						.NetworkLatency = oIVCReturn.NetworkLatency
					End With

					WebSupportToolbar.AddUniqueLog(oRequestInfo)
				End If

			End If

		Catch ex As Exception
			oBookReturn.OK = False
			oBookReturn.Warnings.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/Book", "BookingException", ex.ToString)
		End Try

		'only clear the being booked just before we return
		Me.BookInProgress = False
		Return oBookReturn


	End Function


#End Region


#Region "Store and Retrieve Basket"

	Public Sub StoreBasket()

		Dim oStoreBasketRequest As New ivci.StoreBasketRequest
		Dim oStoreBasketResponse As New ivci.StoreBasketResponse

		Try

			'Add the login details 
			oStoreBasketRequest.LoginDetails = BookingBase.IVCLoginDetails


			'Add the details
			oStoreBasketRequest.BookingReference = Me.BookingReference
			oStoreBasketRequest.BasketXML = Me.XML.InnerXml


			'Send the request
			Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
			 Utility.iVectorConnect.SendRequest(Of ivci.StoreBasketResponse)(oStoreBasketRequest)


			If oIVCReturn.Success Then

				'Get the response object
				oStoreBasketResponse = CType(oIVCReturn.ReturnObject, ivci.StoreBasketResponse)

				'Can store the basket store ID somewhere but at present it is not needed

			End If

		Catch ex As Exception
			FileFunctions.AddLogEntry("iVectorConnect/StoreBasket", "StoreBasketException", ex.ToString)
		End Try

	End Sub

	Public Sub RebuildBasketFromStoredBasket(Optional ByVal BookingReference As String = "", Optional ByVal BasketStoreID As Integer = 0)

		Dim oRetrieveStoredBasketRequest As New ivci.RetrieveStoredBasketRequest
		Dim oRetrieveStoredBasketResponse As New ivci.RetrieveStoredBasketResponse

		Try

			'Add the request details
			oRetrieveStoredBasketRequest.LoginDetails = BookingBase.IVCLoginDetails
			oRetrieveStoredBasketRequest.BookingReference = BookingReference
			oRetrieveStoredBasketRequest.BasketStoreID = BasketStoreID


			'Send the request
			Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
			 Utility.iVectorConnect.SendRequest(Of ivci.RetrieveStoredBasketResponse)(oRetrieveStoredBasketRequest)


			If oIVCReturn.Success Then

				'Get the response object
				oRetrieveStoredBasketResponse = CType(oIVCReturn.ReturnObject, ivci.RetrieveStoredBasketResponse)

				'Create a basket 
				Dim oBasket As BookingBasket = Serializer.DeSerialize(Of BookingBasket)(oRetrieveStoredBasketResponse.BasketXML)

				'Set the basket XML to the current basket
				BookingBase.Basket = oBasket

			End If

		Catch ex As Exception
			FileFunctions.AddLogEntry("iVectorConnect/RetrieveBasket", "RebuildBasketFromStoredBasketException", ex.ToString)
		End Try

	End Sub

#End Region


#Region "Remove Component"

	Public Function RemoveComponent(ByVal iComponentID As Integer) As Boolean

		'check properties
		For Each oProperty As BookingProperty.BasketProperty In Me.BasketProperties
			If oProperty.ComponentID = iComponentID Then
				Me.BasketProperties.Remove(oProperty)
				Return True
			End If
		Next

		'check flgihts
		For Each oFlight As BookingFlight.BasketFlight In Me.BasketFlights
			If oFlight.ComponentID = iComponentID Then
				Me.BasketFlights.Remove(oFlight)
				Return True
			End If
		Next

		'check transfers
		For Each oTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers
			If oTransfer.ComponentID = iComponentID Then
				Me.BasketTransfers.Remove(oTransfer)
				Return True
			End If
		Next

		'check extras
		For Each oExtra As BookingExtra.BasketExtra In Me.BasketExtras
			If oExtra.ComponentID = iComponentID Then
				Me.BasketExtras.Remove(oExtra)
				Return True
			End If
		Next

		'check car hire
		For Each oCarHire As BookingCarHire.BasketCarHire In Me.BasketCarHires
			If oCarHire.ComponentID = iComponentID Then
				Me.BasketCarHires.Remove(oCarHire)
				Return True
			End If
		Next

		Return False

	End Function

#End Region


#Region "Setup Basket Guests"


	Public Sub SetupBasketGuestsFromSearch()

		BookingBase.SearchDetails.ProcessTimer.RecordStart("IntuitiveWeb", "Setup basket guests from search", ProcessTimer.MainProcess)

		'clear existing basket guests
		Me.GuestDetails.Clear()

		'set starting guest id
		Dim iGuestID As Integer = 1
		If BookingBase.Basket.GuestDetails.Count > 0 Then iGuestID = BookingBase.Basket.GuestDetails.Max(Function(o) o.GuestID) + 1


		'adults
		For iAdults As Integer = 1 To BookingBase.SearchDetails.TotalAdults
			Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
			oGuest.GuestID = iGuestID
			oGuest.Type = "Adult"
			Me.GuestDetails.Add(oGuest)
			iGuestID += 1
		Next


		'children
		For iRoom As Integer = 1 To BookingBase.SearchDetails.RoomGuests.Count
			For iChild As Integer = 0 To BookingBase.SearchDetails.RoomGuests(iRoom - 1).Children - 1
				Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
				oGuest.GuestID = iGuestID
				oGuest.Type = "Child"
				oGuest.Age = BookingBase.SearchDetails.RoomGuests(iRoom - 1).ChildAges(iChild)
				Me.GuestDetails.Add(oGuest)
				iGuestID += 1
			Next
		Next

		'infants
		For iInfant As Integer = 1 To BookingBase.SearchDetails.TotalInfants
			Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
			oGuest.GuestID = iGuestID
			oGuest.Type = "Infant"
			oGuest.Age = 0
			Me.GuestDetails.Add(oGuest)
			iGuestID += 1
		Next

		BookingBase.SearchDetails.ProcessTimer.RecordEnd("iVectorWidgets", "Setup basket guests from search", ProcessTimer.MainProcess)


	End Sub


#End Region

#Region "Setup Guests from basket"

	Public Sub SetupBasketGuestsFromRooms()

		'clear existing basket guests
		Me.GuestDetails.Clear()

		'set starting guest id
		Dim iGuestID As Integer = 1
		If BookingBase.Basket.GuestDetails.Count > 0 Then iGuestID = BookingBase.Basket.GuestDetails.Max(Function(o) o.GuestID) + 1

		For Each oBasketProperty As BookingProperty.BasketProperty In BookingBase.Basket.BasketProperties

			For Each oRoom As BookingProperty.BasketProperty.BasketPropertyRoomOption In oBasketProperty.RoomOptions

				'adults
				For iAdults As Integer = 1 To oRoom.GuestConfiguration.Adults
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.GuestID = iGuestID
					oGuest.Type = "Adult"
					Me.GuestDetails.Add(oGuest)
					iGuestID += 1
				Next


				'children
				For iChild As Integer = 0 To oRoom.GuestConfiguration.Children - 1
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.GuestID = iGuestID
					oGuest.Type = "Child"
					oGuest.Age = oRoom.GuestConfiguration.ChildAges(iChild)
					Me.GuestDetails.Add(oGuest)
					iGuestID += 1
				Next

				'infants
				For iInfant As Integer = 1 To oRoom.GuestConfiguration.Infants
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.GuestID = iGuestID
					oGuest.Type = "Infant"
					oGuest.Age = 0
					Me.GuestDetails.Add(oGuest)
					iGuestID += 1
				Next

			Next

		Next

		'set up our placeholder guests
		Me.TBAUnknownGuests()

	End Sub
#End Region

#Region "Setup compment guest IDs from guest config"

	Public Sub TBAUnknownGuests()

		'add guest details
		Dim iGuestID As Integer = 1
		Dim aGuestIDS As New Generic.List(Of Integer)
		If BookingBase.Basket.GuestDetails.Count > 0 Then iGuestID = BookingBase.Basket.GuestDetails.Max(Function(o) o.GuestID) + 1
		For iGuest As Integer = 1 To BookingBase.Basket.GuestDetails.Count
			Dim oGuestDetail As iVectorConnectInterface.Support.GuestDetail = BookingBase.Basket.GuestDetails(iGuest - 1)
			With oGuestDetail
				.Title = "Mr"
				.FirstName = "TBA" & iGuest.ToString
				.LastName = "TBA" & iGuest.ToString
			End With

			If oGuestDetail.Type = "Child" Then
				oGuestDetail.Age = 7
				oGuestDetail.DateOfBirth = Now.AddYears(-7).Date
			ElseIf oGuestDetail.Type = "Infant" Then
				oGuestDetail.Age = 0
				oGuestDetail.DateOfBirth = Now.AddMonths(-6).Date
			End If

			'add guests to non-property components
			For Each oBasketFlight As BookingFlight.BasketFlight In BookingBase.Basket.BasketFlights
				oBasketFlight.GuestIDs.Add(iGuest)
			Next

			For Each oTransfer As BookingTransfer.BasketTransfer In BookingBase.Basket.BasketTransfers
				oTransfer.GuestIDs.Add(iGuest)
			Next

			For Each oExtra As BookingExtra.BasketExtra In BookingBase.Basket.BasketExtras

				If oExtra.GuestIDs Is Nothing Then
					oExtra.GuestIDs = New Generic.List(Of Integer)
				End If

				If iGuest <= oExtra.BasketExtraOptions(0).Adults + oExtra.BasketExtraOptions(0).Children Then
					oExtra.GuestIDs.Add(iGuest)
				End If

			Next

			For Each oCarHire As BookingCarHire.BasketCarHire In BookingBase.Basket.BasketCarHires
				oCarHire.GuestIDs.Add(iGuest)
			Next

			aGuestIDS.Add(iGuest)
		Next

		'add to each com
		For Each oBasketProperty As BookingProperty.BasketProperty In BookingBase.Basket.BasketProperties
			Dim iAdult As Integer = 0
			Dim iChild As Integer = 0
			Dim iInfant As Integer = 0
			For iroom As Integer = 0 To BookingBase.Basket.BasketProperties.Last().RoomOptions.Count - 1
				For i As Integer = 1 To BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestConfiguration.Adults
					Dim iAdultID As Integer = BookingBase.Basket.GuestDetails.Where(Function(o) o.Type = "Adult")(iAdult).GuestID
					BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestIDs.Add(iAdultID)

					iAdult += 1
				Next

				For i As Integer = 1 To BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestConfiguration.Children
					Dim iChildID As Integer = BookingBase.Basket.GuestDetails.Where(Function(o) o.Type = "Child")(iChild).GuestID
					BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestIDs.Add(iChildID)

					iChild += 1
				Next

				For i As Integer = 1 To BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestConfiguration.Infants
					Dim iInfantID As Integer = BookingBase.Basket.GuestDetails.Where(Function(o) o.Type = "Infant")(iInfant).GuestID
					BookingBase.Basket.BasketProperties.Last().RoomOptions(iroom).GuestIDs.Add(iInfantID)

					iInfant += 1
				Next
			Next
		Next

	End Sub


#End Region


#Region "Remove Unused Guests"

	Public Sub RemoveUnusedGuests()

		Dim oGuestsToRemove As New Generic.List(Of ivci.Support.GuestDetail)

		For Each oGuest As ivci.Support.GuestDetail In Me.GuestDetails

			Dim bGuestFound As Boolean = False

			'1. check properties
			For Each oProperty As BookingProperty.BasketProperty In Me.BasketProperties
				For Each oRoom As BookingProperty.BasketProperty.BasketPropertyRoomOption In oProperty.RoomOptions
					bGuestFound = oRoom.GuestIDs.Contains(oGuest.GuestID)
					If bGuestFound Then Exit For
				Next
				If bGuestFound Then Exit For
			Next

			'guest found? continue to next guest
			If bGuestFound Then Continue For



			'2. check flights
			For Each oFlight As BookingFlight.BasketFlight In Me.BasketFlights
				bGuestFound = oFlight.GuestIDs.Contains(oGuest.GuestID)
				If bGuestFound Then Exit For
			Next

			'guest found? continue to next guest
			If bGuestFound Then Continue For



			'3. check transfers
			For Each oTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers
				bGuestFound = oTransfer.GuestIDs.Contains(oGuest.GuestID)
				If bGuestFound Then Exit For
			Next

			'guest found? continue to next guest
			If bGuestFound Then Continue For



			'4. check extras
			For Each oExtra As BookingExtra.BasketExtra In Me.BasketExtras
				bGuestFound = oExtra.GuestIDs.Contains(oGuest.GuestID)
				If bGuestFound Then Exit For
			Next

			'guest found? continue to next guest
			If bGuestFound Then Continue For



			'5. check car hire
			For Each oCarHire As BookingCarHire.BasketCarHire In Me.BasketCarHires
				bGuestFound = oCarHire.GuestIDs.Contains(oGuest.GuestID)
				If bGuestFound Then Exit For
			Next

			'guest found? continue to next guest
			If bGuestFound Then Continue For



			'6. guest is not used in any component so remove
			oGuestsToRemove.Add(oGuest)

		Next


		'remove guests
		For Each oGuestToRemove As ivci.Support.GuestDetail In oGuestsToRemove
			Me.GuestDetails.Remove(oGuestToRemove)
		Next


	End Sub



#End Region


#Region "setup basket guests from booking"

	Public Sub SetupBasketGuestsFromBooking(ByVal oBookingDetails As ivci.GetBookingDetailsResponse)

		'clear existing basket guests
		Me.GuestDetails.Clear()

		'add in the guests from the rooms
		For Each oProperty As ivci.GetBookingDetailsResponse.Property In oBookingDetails.Properties

			For Each oRoom As ivci.GetBookingDetailsResponse.Room In oProperty.Rooms

				For Each oGuestDetail As ivci.Support.GuestDetail In oRoom.GuestDetails

					Me.GuestDetails.Add(oGuestDetail)

					'add to our components
					For Each oExtra As BookingExtra.BasketExtra In BookingBase.Basket.BasketExtras
						BookingBase.Basket.BasketExtras.Last().GuestIDs.Add(oGuestDetail.GuestID)
					Next

				Next

			Next

		Next


	End Sub

#End Region


#Region "Promo Codes"

	Public Sub RefreshPromoCode()

		If Me.PromoCode <> "" Then
			Me.ApplyPromoCode(Me.PromoCode)
		End If

	End Sub

	Public Function ApplyPromoCode(ByVal PromoCode As String) As ApplyPromoCodeReturn

		Me.PromoCodeUpdated = False
		Me.PromoCodePrevious = Me.PromoCode

		Dim oApplyPromoCodeReturn As New ApplyPromoCodeReturn

		Try

			Dim oApplyPromoCodeRequest As New iVectorConnectInterface.ApplyPromoCodeRequest

			With oApplyPromoCodeRequest

				.LoginDetails = BookingBase.IVCLoginDetails
				.PromoCode = PromoCode
				.UseDate = BookingBase.SearchDetails.DepartureDate
				.Duration = BookingBase.SearchDetails.Duration

				If Me.BasketProperties.Count > 0 Then
					.GeographyLevel3ID = Me.BasketProperties(0).RoomOptions(0).GeographyLevel3ID
				End If

				.Adults = Me.TotalAdults
				.Children = Me.TotalChildren

				For Each oFlight As BookingFlight.BasketFlight In Me.BasketFlights

					Dim oComponent As New iVectorConnectInterface.ApplyPromoCodeRequest.Component

					With oComponent
						.BookingComponentID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.BookingComponent, "Flight")
						.BookingComponent = "Flight"
						.Price = oFlight.Flight.Price
						.DepartureAirportID = oFlight.Flight.DepartureAirportID
						.ArrivalAirportID = oFlight.Flight.ArrivalAirportID
						.FlightCarrierID = oFlight.Flight.FlightCarrierID
						.SupplierID = oFlight.Flight.SupplierID
						.TotalPassengers = oFlight.Flight.Adults + oFlight.Flight.Children
						.Adults = oFlight.Flight.Adults
					End With

					.Components.Add(oComponent)

				Next

				For Each oProperty As BookingProperty.BasketProperty In Me.BasketProperties

					Dim oComponent As New iVectorConnectInterface.ApplyPromoCodeRequest.Component

					With oComponent

						.BookingComponentID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.BookingComponent, "Property")
						.BookingComponent = "Property"

						.PropertyReferenceID = oProperty.RoomOptions(0).PropertyReferenceID

						'Pick the first one at the moment as there is currently no way to deal with multiple rooms with different meal bases
						.MealBasisID = oProperty.RoomOptions(0).MealBasisID

						.StarRating = Math.Floor(oProperty.RoomOptions(0).Rating).ToSafeInt
						.Price = oProperty.RoomOptions.Sum(Function(oRoom) oRoom.TotalPrice)
						.TotalPassengers = oProperty.RoomOptions.Sum(Function(oRoom) oRoom.GuestConfiguration.Adults + oRoom.GuestConfiguration.Children)
						.Adults = oProperty.RoomOptions.Sum(Function(oRoom) oRoom.GuestConfiguration.Adults)
						.SupplierID = oProperty.RoomOptions(0).SupplierID

					End With

					.Components.Add(oComponent)

				Next

				For Each oTransfer As BookingTransfer.BasketTransfer In Me.BasketTransfers

					Dim oComponent As New iVectorConnectInterface.ApplyPromoCodeRequest.Component

					With oComponent
						.BookingComponentID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.BookingComponent, "Transfer")
						.BookingComponent = "Transfer"
						.Price = oTransfer.Transfer.Price
						.Adults = Me.TotalAdults
						.TotalPassengers = Me.TotalAdults + Me.TotalChildren
					End With

					.Components.Add(oComponent)

				Next

				For Each oExtra As BookingExtra.BasketExtra In Me.BasketExtras

					For Each oExtraOption As BookingExtra.BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions

						Dim oComponent As New iVectorConnectInterface.ApplyPromoCodeRequest.Component

						With oComponent
							.BookingComponentID = oExtra.ComponentID
							.BookingComponent = "Extra"
							.Price = oExtraOption.TotalPrice
							.ExtraTypeID = oExtraOption.ExtraTypeID
							.Adults = Me.TotalAdults
							.TotalPassengers = Me.TotalAdults + Me.TotalChildren
							.SupplierID = oExtraOption.SupplierID
						End With

						.Components.Add(oComponent)

					Next

				Next

				For Each oCarHire As BookingCarHire.BasketCarHire In Me.BasketCarHires

					Dim oComponent As New iVectorConnectInterface.ApplyPromoCodeRequest.Component

					With oComponent
						.BookingComponentID = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.BookingComponent, "CarHire")
						.BookingComponent = "CarHire"
						.Price = oCarHire.CarHire.Price
						.Adults = Me.TotalAdults
						.TotalPassengers = Me.TotalAdults + Me.TotalChildren
					End With

					.Components.Add(oComponent)

				Next

				oApplyPromoCodeRequest.TotalPrice = Me.TotalPrice + Me.PromoCodeDiscount

			End With

			If oApplyPromoCodeRequest.Components.Count = 0 Then
				Me.PromoCode = ""
				Me.PromoCodeDiscount = 0
				Me.PromoCodeUpdated = True
			End If

			'Send the request 
			Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
			 Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.ApplyPromoCodeResponse)(oApplyPromoCodeRequest)

			Dim oPromoCodeResponse As New iVectorConnectInterface.ApplyPromoCodeResponse

			If oIVCReturn.Success Then

				oPromoCodeResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.ApplyPromoCodeResponse)

				If oPromoCodeResponse.Discount <> 0 Then
					'Set the discount and promo code on the basket 
					oApplyPromoCodeReturn.OK = True
					oApplyPromoCodeReturn.Discount = oPromoCodeResponse.Discount

					'If the promo code is the same and the price has changed it has been updated
					If Me.PromoCode = PromoCode AndAlso Me.PromoCodeDiscount <> oPromoCodeResponse.Discount Then
						Me.PromoCodeUpdated = True
					End If

					Me.PromoCode = PromoCode
					Me.PromoCodeDiscount = oApplyPromoCodeReturn.Discount
				Else
					oApplyPromoCodeReturn.OK = False
					oApplyPromoCodeReturn.Warnings.Add(oPromoCodeResponse.Warning)

					'if the promo code applied is the same as already on basket it has become invalid since last use and should be removed
					If Me.PromoCode = PromoCode Then
						Me.PromoCode = ""
						Me.PromoCodeDiscount = 0
						Me.PromoCodeUpdated = True
					End If
				End If

			End If

		Catch ex As Exception
			oApplyPromoCodeReturn.OK = False
			oApplyPromoCodeReturn.Warnings.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/Book", "BookingException", ex.ToString)
		End Try

		Return oApplyPromoCodeReturn

	End Function

#End Region


#Region "Support Classes"

    Public Class CreditCardType
        Public Name As String
        Public CreditCardTypeID As Integer
        Public FlightSupplierFee As Decimal 'pwcc surcharge amount, NOT %
        Public SurchargePercentage As Decimal '% surcharge on the amount coming to us, eg not including pwcc amount
    End Class

    Public Class PreBookReturn
        Public OK As Boolean = True
        Public Warnings As New Generic.List(Of String)
        Public TotalPrice As Decimal = 0
        Public RedirectString As String
    End Class


    Public Class BookReturn
        Public OK As Boolean = True
        Public Warnings As New Generic.List(Of String)
        Public BookingReference As String
        Public RedirectString As String
        Public ComponentFailed As Boolean = False
        Public SecureEnrollment As Boolean

        Public PropertyBookings As New Generic.List(Of BookReturnComponent)
    End Class

    Public Class ApplyPromoCodeReturn
        Public OK As Boolean = True
        Public Warnings As New Generic.List(Of String)
        Public Discount As Decimal
    End Class

    Public Class BookReturnComponent
        Public SupplierReference As String
    End Class


    Public Class BookWarning
        Public Const Timeout As String = "Timeout exceeded"
    End Class

    Public Class VehicleInformation
        Public CarRegistration As String
        Public CarMake As String
        Public CarModel As String
        Public CarColour As String
    End Class

#End Region



    Public Function Accept(Visitor As Basket.Interfaces.IBasketVisitor) As Integer Implements Basket.Interfaces.IBasketVisitable.Accept
        Return Visitor.Visit(Me)
    End Function
End Class

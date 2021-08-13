Imports System.Xml.Serialization
Imports iVectorConnectInterface.Cruise

Public Class GetBookingDetailsResponse
    Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

#Region "Properties"

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

	Public BookingID As Integer
	Public SystemUserID As Integer
	Public BookingReference As String
	Public ExternalReference As String
	Public TradeReference As String
	Public BrandName As String
	Public BookingDescriptionID As Integer
	Public BrandCode As String
	Public Status As String
	Public BookingDate As Date
	Public AccountStatus As String
	Public ImportantDocument As Boolean
	Public LeadCustomer As Support.LeadCustomerDetails
	Public Adults As Integer
	Public Children As Integer
	Public Infants As Integer
	Public BookingPassengers As Generic.List(Of [Passenger])
	Public FirstDepartureDate As Date
	Public LastReturnDate As Date
	Public TotalPrice As Decimal
	Public TotalPaid As Decimal
	Public TotalOutstanding As Decimal
	Public TotalCommission As Decimal
	Public VATOnCommission As Decimal
	Public CurrencyID As Integer
	Public CurrencyCode As String
	Public CurrencySymbol As String
	Public CurrencySymbolPosition As String
	Public CurrencyCustomerSymbolOverride As String
    Public CustomerPayments As List(Of CustomerPayment)
    Public PaymentsReceived As List(Of PaymentReceived)
    Public BookingAdjustments As List(Of BookingAdjustment)
    Public Inclusions As List(Of Inclusion)

    Public Properties As List(Of [Property])
    Public Flights As List(Of Flight)
    Public Transfers As List(Of Transfer)
    Public Extras As List(Of Extra)
    Public AdHocBookings As List(Of AdHocBooking)
    Public CarHireBookings As List(Of CarHireBooking)
    Public Cruises As List(Of Cruise)
    Public PayPalTransactions As List(Of Support.PayPalTransaction)
    Public BookingTags As List(Of Support.BookingTag)

#End Region

#Region "Helper Classes"

#Region "General"

	Public Class CustomerPayment
		Public CustomerPaymentID As Integer
		Public Status As String
		Public DateDue As Date
		Public CurrencyID As Integer
		Public Symbol As String
		Public TotalPayment As Decimal
		Public Outstanding As Decimal
	End Class

	Public Class PaymentReceived
		Public PaymentReceivedID As Integer
		Public CustomerPaymentID As Integer
		Public PaymentAmount As Decimal
	End Class

	Public Class Passenger
		Public Type As String
		Public Title As String
		Public FirstName As String
		Public LastName As String
		Public Age As Integer
		Public DateOfBirth As Date
		Public Nationality As String
		Public BookingPassengerID As Integer

	End Class

	Public Class BookingAdjustment
		Public AdjustmentType As String
		Public AdjustmentID As Integer
		Public Adjustment As String
		Public AdjustmentAmount As Decimal
		Public CustomerAdjustmentAmount As Decimal
	End Class

	Public Class Inclusion
		Public Name As String
		Public Description As String
		Public MoreInfoLink As String
		Public Mandatory As Integer
		Public Current As Integer
		Public Included As Integer
		Public InclusionType As String
		Public BookingComponentiD As Integer
		Public BondingID As Integer
		Public GeographyLevel1ID As Integer
		Public GeographyLevel2ID As Integer
		Public GeographyLevel3ID As Integer
	End Class

#End Region

#Region "Property"

	Public Class [Property]
		Public PropertyID As Integer
		Public PropertyBookingID As Integer
		Public PropertyBookingReference As String
		Public Source As String
		Public SourceReference As String
		Public SourceSecondaryReference As String
		Public PropertyReferenceID As Integer
		'''<summary>
		'''Taken from PropertyBooking.PropertyID, to be used if PropertyReferenceID is unavailable.
		'''</summary>
		Public GeographyLevel1ID As Integer
		Public GeographyLevel2ID As Integer
		Public GeographyLevel3ID As Integer
		Public Status As String
		Public ConfirmationStatus As String
		Public ArrivalDate As Date
		Public ReturnDate As Date
		Public Duration As Integer
		Public TotalPrice As Decimal
		Public Rooms As New Generic.List(Of Room)
		Public Errata As New Generic.List(Of Erratum)
		<XmlArrayItem("Comment")>
		Public Comments As New Generic.List(Of String)
		Public PropertyName As String
		Public GeographyLevel2Name As String

	End Class

	Public Class Room
		Public PropertyRoomBookingID As Integer
		Public MealBasisID As Integer
		Public MealBasis As String
		Public RoomTypeID As Integer
		Public RoomType As String
		Public RoomViewID As Integer
		Public PropertyRoomTypeID As Integer
		Public UpgradeMealBasis As String
		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public TotalPrice As Decimal
		Public PayLocalTotal As Decimal
		Public GuestDetails As New Generic.List(Of Support.GuestDetail)
		Public Adjustments As New Generic.List(Of Adjustment)
        Public OptionalSupplements As New Generic.List(Of OptionalSupplement)

        <XmlArrayItem("GuestID")>
        Public GuestIDs As Generic.List(Of Integer) = Nothing
        Public RoomBookingToken As String
    End Class

	Public Class Adjustment
		Public AdjustmentType As String
		Public AdjustmentID As Integer
		Public AdjustmentName As String
		Public Total As Decimal
		Public PayLocal As Boolean
	End Class

	Public Class OptionalSupplement
		Public Property ContractSupplementID As Integer
		Public Property Units As Integer
		Public Property Duration As Integer
		Public Property Adults As Integer
		Public Property Children As Integer
		Public Property Infants As Integer
		Public Property PayLocal As Boolean
	End Class

	Public Class Erratum
		Public ErratumID As Integer
		Public ErratumDescription As String
	End Class

#End Region

#Region "Flights"

	Public Class Flight

		Public FlightBookingID As Integer
		Public FlightBookingReference As String
		Public Status As String
		Public ConfirmationStatus As String
		Public Price As Decimal

		Public BaggagePrice As Decimal
		Public BaggageQuantity As Integer
		Public ChildBaggagePrice As Decimal
		Public ChildBaggageQuantity As Integer

		Public DepartureAirportID As Integer
		Public DepartureAirport As String
		Public DepartureAirportCode As String

		Public ArrivalAirportID As Integer
		Public ArrivalAirport As String
		Public ArrivalAirportCode As String

		Public OutboundDepartureDate As Date
		Public OutboundDepartureTime As String
		Public OutboundArrivalDate As Date
		Public OutboundArrivalTime As String
		Public OutboundAirlineCode As String
		Public OutboundFlightCode As String
        Public OutboundFlightClassID As Integer
        Public OutboundFlightClass As String

		Public ReturnArrivalDate As Date
		Public ReturnArrivalTime As String
		Public ReturnDepartureDate As Date
		Public ReturnDepartureTime As String
		Public ReturnAirlineCode As String
		Public ReturnFlightCode As String
        Public ReturnFlightClassID As Integer
        Public ReturnFlightClass As String

		Public Source As String
		Public SourceReference As String
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public FlightCarrierLogo As String
		Public OneWay As Boolean

		Public APISUpdatesSupported As Boolean
		Public SupplierCode As String
		Public SupplierName As String

		Public ImportantDocument As Boolean

		Public FlightSectors As New Generic.List(Of FlightSector)

		Public FlightPassengers As New Generic.List(Of FlightPassenger)

		Public FlightErrata As New Generic.List(Of FlightErratum)

		Public FlightExtras As New Generic.List(Of FlightExtra)

		Public FlightTickets As New List(Of FlightTicket)
	End Class

	Public Class FlightSector

		Public FlightBookingMultiSectorID As Integer
		Public Direction As String
		Public FlightBookingDesiredMultiSectorID As Integer

		Public DepartureAirportID As Integer
		Public DepartureAirport As String
		Public DepartureAirportCode As String
		Public DepartureDate As Date
		Public DepartureTime As String
		Public DepartureTerminalID As Integer
		Public DepartureTerminal As String

		Public ArrivalAirportID As Integer
		Public ArrivalAirport As String
		Public ArrivalAirportCode As String
		Public ArrivalDate As Date
		Public ArrivalTime As String
		Public ArrivalTerminalID As Integer
		Public ArrivalTerminal As String

		Public StopAirportID As Integer
		Public StopAirport As String

		Public TravelTime As Integer

		Public FlightClassID As Integer
		Public FlightClass As String
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public FlightCarrierLogo As String
		Public FlightCode As String
		Public AirlineCode As String

		Public AwaitingSchedule As Boolean

		Public FlightSeats As New Generic.List(Of FlightSeat)

		Public FlightDesiredSector As FlightSector

	End Class

	Public Class FlightSeat
		Public FlightBookingSeatID As Integer
		Public FlightBookingID As Integer
		Public FlightBookingMultiSectorID As Integer
		Public FlightBookingPassengerID As Integer
		Public TotalPrice As Decimal
	End Class

	Public Class FlightPassenger

		Public FlightBookingPassengerID As Integer

		Public Title As String
		Public FirstName As String
		Public LastName As String
		Public DateOfBirth As Date
		Public GuestType As String

		Public MiddleName As String
		Public PassportNumber As String
		Public PassportIssueDate As Date
		Public PassportExpiryDate As Date
		Public NationalityID As Integer
		Public PassportIssuingGeographyLevel1ID As Integer
		Public hlpOutboundCharterFlightInventoryID As Integer
		Public hlpReturnCharterFlightInventoryID As Integer
		Public hlpOutboundSeatCode As String
		Public hlpReturnSeatCode As String
		Public Gender As String

	End Class

	Public Class FlightErratum
		Public FlightErratumID As Integer
		Public ErratumDescription As String
	End Class

	Public Class FlightExtra
		Public ExtraType As String
		Public Description As String
        Public FriendlyDescription As String
		Public CostingBasis As String
		Public Price As String
		Public Quantity As Integer
		Public OptionExpiry As Date
	End Class

	Public Class FlightTicket
		Public Property FlightBookingTicketID As Integer
		Public Property FlightBookingID As Integer
		Public Property FlightBookingPassengerID As Integer
		Public Property BookingPassengerID As Integer
		Public Property Type As String
		Public Property TicketNumber As String
		Public Property GrossFare As Decimal
		Public Property Tax As Decimal
		Public Property AmountPayable As Decimal
		Public Property RetrievedFromPNR As Boolean
		Public Property Added As Date
		Public Property Archived As Boolean
		Public Property LastUpdated As Date
		Public Property IssueDate As Date
		Public Property BSPGrossFare As Decimal
		Public Property BSPTax As Decimal
		Public Property BSPAmountPayable As Decimal
		Public Property BSPIssueDate As Date
		Public Property TicketGroupNumber As Integer

		Public Property FlightTicketMultiSectors As New FlightTicketMultiSectors
	End Class

	Public Class FlightTicketMultiSectors
		Inherits List(Of FlightTicketMultiSector)
	End Class

	Public Class FlightTicketMultiSector
		Public FlightBookingTicketMultiSectorID As Integer
		Public FlightBookingTicketID As Integer
		Public FlightBookingMultiSectorID As Integer
	End Class

#End Region

#Region "Transfer"
	Public Class Transfer

		Public TransferBookingID As Integer
		Public TransferBookingReference As String
		Public Source As String
		Public SourceReference As String
		Public Status As String
		Public ConfirmationStatus As String
		Public DepartureParentType As String
		Public DepartureParentID As Integer
		Public ArrivalParentType As String
		Public ArrivalParentID As Integer
		Public OneWay As Boolean

		Public DepartureDate As Date
		Public DepartureTime As String
		Public DepartureFlightCode As String
		Public DepartureNotes As String
		Public ReturnDate As Date
		Public ReturnTime As String
		Public ReturnFlightCode As String
		Public ReturnNotes As String
		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public VehicleClass As String
		Public Vehicle As String
		Public VehicleQuantity As Integer
		Public Price As Decimal
		Public DepartureParentName As String
		Public ArrivalParentName As String

		Public OutboundDetails As New iVectorConnectInterface.Transfer.BookRequest.OutboundJourneyDetails
		Public ReturnDetails As New iVectorConnectInterface.Transfer.BookRequest.ReturnJourneyDetails
		Public GuestDetails As New Generic.List(Of Support.GuestDetail)

	End Class
#End Region

#Region "Extra"
	Public Class Extra
		Public ExtraBookingID As Integer
		Public ExtraBookingReference As String
		Public ExternalReference As String
		Public ExtraName As String
		Public ExtraType As String
		Public ExtraCategory As String
		Public ExtraID As Integer
		Public ExtraTypeID As Integer
		Public ExtraCategoryID As Integer
		Public ExtraDurationID As Integer
		Public ExtraDuration As Double
		Public Status As String
		Public ConfirmationStatus As String
		Public AddedDateTime As Date
		Public StartDate As Date
		Public StartTime As String
		Public Expiry As Date
		Public EndTime As String
		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public Seniors As Integer
		Public TotalPrice As Decimal
		Public SalesTax As Decimal
		Public CarParkInformation As New CarParkDetails
		Public GuestDetails As New Generic.List(Of Support.GuestDetail)
		Public Source As String
		Public SourceReference As String
		Public SupplierID As Integer
		Public SupplierName As String
		Public SupplierConfirmation As String
		Public ExtraPickupPoint As String
		Public CancellationPolicy As String
		Public Comment As String
		Public ProductAttributes As List(Of ProductAttribute)
		Public ExtraPickupPointID As Integer
		Public Schedule As String
		Public Location As String

		Public MaxChildAge As Integer
		Public MinChildAge As Integer
		Public SeniorAge As Integer

		Public ExtraOptions As New Generic.List(Of ExtraOption)

		Public Passengers As New Generic.List(Of Passenger)

		Public Class ExtraOption
			Public ExtraBookingOptionID As Integer
			Public ExtraCategoryID As Integer
			Public ExtraCategory As String
			Public UseDate As Date
			Public EndDate As Date
			Public Adults As Integer
			Public Children As Integer
			Public Infants As Integer
			Public TotalPrice As Decimal
		End Class

		Public Class CarParkDetails
			Public AirportID As Integer
			Public AirportTerminalID As Integer
			Public CarParkName As String
			Public CarParkCode As String
			Public TransferFrequency As String
			Public TransferTime As String
			Public CarMake As String
			Public CarColour As String
			Public CarRegistration As String
			Public OutboundFlightCode As String
			Public ReturnFlightCode As String
			Public Instructions As String
			Public IATACode As String
		End Class

		Public Class ProductAttribute
			Public Property ProductAttributeID As Integer
			Public Property ProductAttributeName As String
		End Class

	End Class
#End Region

#Region "AdHocBooking"

	Public Class AdHocBooking

		Public AdHocBookingID As Integer
		Public AdHocBookingReference As String
		Public BookingComponentID As Integer
		Public BookingComponent As String
		Public Status As String
		Public ConfirmationStatus As String
		Public ExtraTypeID As Integer
		Public ExtraType As String
		Public Description As String
		Public SubDescription As String
		Public UseStartDate As Date
		Public UseEndDate As Date
		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer
		Public Source As String
		Public SourceReference As String
		Public SupplierName As String
		Public TotalCost As Decimal
		Public TotalPrice As Decimal

		Public AdHocCustoms As New Generic.List(Of AdHocCustom)
		Public GuestDetails As New Generic.List(Of Support.GuestDetail)

	End Class

	Public Class AdHocCustom

		Public AdHocCustomID As Integer
		Public AdHocComponentCustomID As Integer
		Public CustomValue As String
		Public CustomFieldName As String

	End Class

#End Region

#Region "Car Hire"
	Public Class CarHireBooking
		Public CarHireBookingID As Integer
		Public CarHireBookingReference As String
		Public Source As String
		Public SourceReference As String
		Public SupplierID As Integer
		Public SupplierName As String
		Public Status As String
		Public ConfirmationStatus As String
		Public PickUpDepotID As Integer
		Public DropOffDepotID As Integer
		Public PickUpDepotName As String
		Public DropOffDepotName As String
		Public PickUpDate As Date
		Public PickUpTime As String
		Public DropOffDate As Date
		Public DropOffTime As String

		Public Adults As Integer
		Public Children As Integer
		Public Infants As Integer

		Public VehicleDescription As String
		Public VehicleImage As String
		Public CarInformation As String
		Public CarTypeID As Integer
		Public AdditionalInformation As String

		Public TotalPrice As Decimal

		Public CarHireBookingAdjustments As New Generic.List(Of CarHireBookingAdjustment)

		Public GuestDetails As New Generic.List(Of Support.GuestDetail)
		Public CarHireBookingExtras As New List(Of CarHireBookingExtra)

        Public PickUpInformation As CarHire.SearchResponse.DepotInformation
        Public DropOffInformation As CarHire.SearchResponse.DepotInformation
	End Class

	Public Class CarHireBookingExtra
		Public CarHireBookingExtraID As Integer
		Public CarHireBookingID As Integer
		Public Description As String
		Public Cost As Decimal
		Public Total As Decimal
		Public Quantity As Integer
		Public IncludedInPrice As Boolean
		Public PrePaid As Boolean
		Public PayAtDepot As Boolean
		Public SupplierReference As String
        Public MaxAmount As Integer
	End Class

	Public Class CarHireBookingAdjustment
		Public CarHireBookingAdjustmentID As Integer
		Public AdjustmentType As String
		Public AdjustmentID As Integer
		Public Adjustment As String
		Public Amount As Decimal
		Public TotalAmount As Decimal
		Public ReportingCode As String
	End Class
#End Region

#Region "Cruise"

    Public Class Cruise
        Public CruiseBookingID As Integer
        Public CruiseBookingReference As String
        Public TotalPrice As Decimal
        Public TotalCommission As Decimal
        Public VATOnCommission As Decimal
        Public SupplierReference As String
        Public Source As String
        Public Status As String
        Public ConfirmationStatus As String
        Public CruiseShipID As Integer
        Public ShipName As String
        Public DeparturePort As String
        Public DeparturePortID As Integer
        Public ReturnPort As String
        Public ReturnPortID As Integer
        Public ItineraryName As String
        Public ItineraryDetails As String
        Public DepartureDate As Date
        Public ReturnDate As Date
        Public Duration As Integer
        Public PayLocal As Boolean
        Public PayLocalTotal As Decimal
        Public Cabins As List(Of Cabin)
    End Class

#End Region

#End Region

End Class

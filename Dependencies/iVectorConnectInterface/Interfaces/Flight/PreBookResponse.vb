Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace Flight
    <XmlType("FlightPreBookResponse")>
    <XmlRoot("FlightPreBookResponse")>
    Public Class PreBookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

        Public Property BookingToken As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property CommissionType As String
		Public Property CommissionValue As String

        'additional supplier costs
        Public Property TotalInfantCost As Decimal
		Public Property AdditionalSupplierCosts As Decimal

		'seat maps cost
		Public Property SeatMapCost As Decimal

		'date of birth requirement
		Public Property ShowDateOfBirth As Boolean

		'mix and match
		Public Property MultiCarrierOutbound As Boolean = False
		Public Property MultiCarrierReturn As Boolean = False

		''' <summary>
		'''     Gets a value indicating whether multi carrier.
		''' </summary>
		''' <value>
		'''     <c>true</c> if multi carrier; otherwise, <c>false</c>.
		''' </value>
		<XmlIgnore()>
		Public ReadOnly Property MultiCarrier As Boolean
			Get
				If MultiCarrierOutbound Or MultiCarrierReturn Then Return True

				Return False
			End Get
		End Property

        Public Property Extras As New Generic.List(Of Extra)

		Public Property CreditCardSurcharges As New List(Of CreditCardSurcharge)
		Public Property Cancellations As New CancellationsList(Of Cancellation)

		''' <summary>
		'''     The multi carrier cancellations.
		''' </summary>
		Public MultiCarrierCancellations As New CancellationsList(Of Cancellation)

        Public Property PaymentsDue As New Generic.List(Of Support.PaymentDue)

		Public Property TerminalInformations As New Generic.List(Of TerminalInformation)

		<XmlIgnore()>
		Public Property DepartureDate As Date

		<XmlIgnore()>
		Public Property TotalCost As Decimal

		<XmlIgnore()>
		Public Property FlightSupplierPaymentAmount As Decimal

		<XmlIgnore()>
		Public Property SeatMapsRequested As Boolean

		<XmlIgnore()>
		Public Property hlpFlightBookingID As Integer

		<XmlIgnore()>
		Public Property hlpBuyingCurrencyID As Integer

		<XmlIgnore()>
		Public Property hlpOutboundArrivalDate As Date

		<XmlIgnore()>
		Public Property PackageDetails As New PackageDetailsDef

		Public Property FlightErrata As New List(Of Support.FlightErratum)

		Public Property ProductAttributes As New List(Of Support.ProductAttribute)

		Public Property SeatMaps As New List(Of SeatMap)

		'Terms and conditions
		Public Property TermsAndConditions As String
        Public Property TermsAndConditionsURL As String

        Public Property FlightSupplierFee As Decimal

        Public Property IncludedBaggageAllowance As Integer
        Public Property IncludedBaggageText As String
        Public Property IncludesSupplierBaggage As Boolean
        Public Property TotalBaggagePrice As Decimal

        Public Property UseCustomerCard As Boolean

        Public Property FareFamilies As List(Of Flight.FareFamily.FareFamily)

		Public Property PassengerFareBreakDown As Support.PassengerFareBreakdown

        Public Property PassengerFares As Generic.List(Of Support.PassengerFare)

		Public Property PayWithCustomerCardCharges As Generic.List(Of Support.PayWithCustomerCardCharge)

		Public Property APISCheckRequired As Boolean

        Public Property BondingID As Integer

#End Region

#Region "Helper Classes"

        Public Class Extra
            Public ExtraBookingToken As String
            Public ExtraType As String
            Public Description As String
            Public FriendlyDescription As String
			Public DefaultBaggage As Boolean
			Public CostingBasis As String
			Public Price As Decimal
            Public QuantityAvailable As Integer
			Public QuantitySelected As Integer
			Public GuestID As Integer
			Public Mandatory As Boolean = False
			Public PaxType As String
            Public ExtraGroup As String
		End Class

        Public Class PackageDetailsDef
			<XmlIgnore()>
			Public Property FlightSeatCost As Decimal

			<XmlIgnore()>
			Public Property FlightSeatPrice As Decimal

            <XmlIgnore()>
            Public Property BaggageQuantity As Integer

            <XmlIgnore()>
            Public Property BaggageCost As Decimal
        End Class

        Public Class CreditCardSurcharge
            Public CreditCardTypeID As Integer
            Public Amount As Decimal
		End Class

		Public Class TerminalInformation
			Public Direction As String
			Public DepartureAirportID As Integer
			Public ArrivalAirportID As Integer
			Public DepartureTerminal As String
			Public ArrivalTerminal As String

            Public FareRules As New List(Of FareRule)
        End Class

        Public Class FareRule
            Public FareSupplierReference As String
            Public FareRuleInformation As String
            Public Category As String
        End Class

#Region "Seat maps"

        Public Class SeatMap
            Public Property Direction As String
            Public Property Seq As Integer
            Public Property Rows As New Generic.List(Of Row)
        End Class

        Public Class Row
            Public RowNumber As String
            Public Columns As New Generic.List(Of Column)
        End Class

        Public Class Column
            Public FlightBookingSeatToken As String
            Public ExtraType As String
            Public SeatReference As String
            Public Price As Decimal
            Public QuantityAvailable As Integer
            Public QuantitySelected As Integer
            Public InfantAllowed As Boolean
			<XmlArray("Attributes"), XmlArrayItem("Attribute")>
            Public Attributes As New Generic.List(Of String)

        End Class

#End Region

#End Region

    End Class

End Namespace

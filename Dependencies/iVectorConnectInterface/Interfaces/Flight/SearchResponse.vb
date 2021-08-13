Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces

Namespace Flight

	<XmlType("FlightSearchResponse")>
	<XmlRoot("FlightSearchResponse")>
	Public Class SearchResponse
		Implements IVectorConnectResponse

#Region "Properties"

		Public Property ReturnStatus As New ReturnStatus Implements IVectorConnectResponse.ReturnStatus
		Public Property Flights As New Generic.List(Of Flight)

#End Region

#Region "Helper Classes"

		Public Class Flight
			Public Property FlightID As Integer
			Public Property BookingToken As String

			Public Property FlightCarrierID As Integer
			Public Property ReturnFlightCarrierID As Integer

			Public Property Source As String
			Public Property FlightSupplierID As Integer
			Public Property FlightSupplierCarrierID As Integer

			Public Property TPSessionID As String
			Public Property DepartureAirportID As Integer
			Public Property ArrivalAirportID As Integer
			Public Property AltReturnAirportID As Integer
			Public Property OutboundDepartureDate As Date
			Public Property OutboundDepartureTime As String
			Public Property OutboundArrivalDate As Date
			Public Property OutboundArrivalTime As String
			Public Property OutboundFlightClassID As Integer
			Public Property OutboundFlightCode As String
			Public Property OutboundOperatingFlightCarrierID As Integer
			Public Property OutboundFareCode As String
            Public Property OutboundThirdPartyFlightID As String
			Public Property ReturnDepartureDate As Date
			Public Property ReturnDepartureTime As String
			Public Property ReturnArrivalDate As Date
			Public Property ReturnArrivalTime As String
			Public Property ReturnFlightClassID As Integer
			Public Property ReturnFlightCode As String
			Public Property ReturnOperatingFlightCarrierID As Integer
			Public Property ReturnFareCode As String
            Public Property ReturnThirdPartyFlightID As String
			Public Property NumberOfOutboundStops As Integer
			Public Property NumberOfReturnStops As Integer
			Public Property PerAdultFare As Decimal
			Public Property PerAdultTax As Decimal
			Public Property PerChildFare As Decimal
			Public Property PerChildTax As Decimal
			Public Property PerInfantFare As Decimal
			Public Property PerInfantTax As Decimal
			Public Property PerYouthFare As Decimal
			Public Property PerYouthTax As Decimal
			Public Property TotalSeatCost As Decimal
			Public Property TotalSeatPrice As Decimal
			Public Property TotalBaggagePrice As Decimal
			Public Property BaggagePricePerPerson As Decimal
			Public Property BaggageDescription As String
			Public Property Saving As Decimal
			Public Property TotalTaxes As Decimal
			Public Property TotalPrice As Decimal
			Public Property TotalCommission As Decimal
			Public Property TicketingDeadline As Date
			Public Property ExactMatch As Boolean
			Public Property HotelArrivalDate As Date
			Public Property HotelDuration As Integer
			Public Property IncludesSupplierBaggage As Boolean
			Public Property IncludedBaggageAllowance As Integer
			Public Property IncludedBaggageAllowancePerPerson As Integer
            Public Property IncludedBaggageWeight As Decimal
			Public Property IncludedBaggageDescription As String
			Public Property IncludedBaggageText As String
			Public Property SupplierDetails As Support.SupplierDetails
			Public Property FareFamilyName As String = String.Empty
			Public Property FlightSectors As New Generic.List(Of Support.FlightSector)
			Public Property FlightErrata As New Generic.List(Of Support.FlightErratum)
			Public Property BookingClass As String = String.Empty
			Public Property MultiCarrierDetails As MultiCarrierDetails
			Public Property ProductAttributes As New Generic.List(Of Support.ProductAttribute)
			Public Property FareTypeCode As String = String.Empty
            Public Property PassengerFareBreakDown As Support.PassengerFareBreakdown
		End Class


		Public Class MultiCarrierDetails
			Public Property BookingToken As String
			Public Property Source As String
			Public Property FlightSupplierID As Integer
			Public Property TPSessionID As String
		End Class
#End Region

	End Class

End Namespace

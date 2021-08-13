Imports System.Xml.Serialization

Namespace Basket
	<XmlRoot("BasketPreBookResponse")>
	Public Class PreBookResponse
		Implements Interfaces.IVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property TotalPrice As Decimal = 0
		Public Property TotalCommission As Decimal = 0
		Public Property VATOnCommission As Decimal = 0
		Public Property CommissionPercentage As Decimal = 0
		Public Property VATOnCommissionPercentage As Decimal = 0
		Public Property IsTrade As Boolean = False

		Public ReadOnly Property PriceChange As Boolean
			Get
				Return Me.PropertyBookings.Any(Function(o) o.PriceChange)
			End Get
		End Property

		<XmlArrayItem("PropertyPreBookResponse")>
		Public Property PropertyBookings As New List(Of [Property].PreBookResponse)

		<XmlArrayItem("FlightPreBookResponse")>
		Public Property FlightBookings As New List(Of Flight.PreBookResponse)

		<XmlArrayItem("TransferPreBookResponse")>
		Public Property TransferBookings As New List(Of Transfer.PreBookResponse)

		<XmlArrayItem("ExtraPreBookResponse")>
		Public Property ExtraBookings As New List(Of Extra.PreBookResponse)

		<XmlArrayItem("CarHirePreBookResponse")>
		Public Property CarHireBookings As New List(Of CarHire.PreBookResponse)

		Public Property BookingAdjustments As New List(Of BookingAdjustment)

		Public Property PaymentAmountDetail As New Support.PaymentAmountDetail

#Region "helper classes"

		Public Class BookingAdjustment
			Public BookingAdjustmentTypeID As Integer
			Public AdjustmentType As String
			Public AdjustmentAmount As Decimal
			Public CalculationBasis As String
			Public ParentType As String
		End Class

#End Region
	End Class
End Namespace
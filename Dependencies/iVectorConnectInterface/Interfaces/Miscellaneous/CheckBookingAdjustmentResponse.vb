Imports System.Xml.Serialization
Public Class CheckBookingAdjustmentResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse
    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
    Public Property BookingAdjustments As New Generic.List(Of BookingAdjustment)

#Region "helper classes"
    Public Class BookingAdjustment
        Public Property BookingAdjustmentTypeID As Integer
        Public Property AdjustmentType As String
        Public Property CalculationBasis As String
        Public Property AdjustmentValue As Decimal
        Public Property OverridableInCallCentre As Boolean
        Public Property BookingAdjustmentComponents As New Generic.List(Of BookingAdjustmentComponents)
    End Class
    Public Class BookingAdjustmentComponents
        Public Property ComponentID As Integer
        Public Property BookingComponentID As Integer
        Public Property ComponentType As String
    End Class
#End Region
End Class

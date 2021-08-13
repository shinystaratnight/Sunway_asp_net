Imports System.Xml.Serialization

Public Class PreCancelComponentResponse

    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

    Public Property BookingReference As String
    Public Property TotalCost As Decimal

    Public Property BookingComponents As New Generic.List(Of PreCancelComponentResponse.BookingComponent)


#Region "helper classes"

    Public Class BookingComponent
        Public Property ComponentBookingID As Integer
        Public Property ComponentType As String
        Public Property CancellationCost As Decimal
		Public Property CancellationToken As String
		Public Property AdHocBooking As Boolean = False
        Public Property SupplierDetails As New SupplierDetail
    End Class

    Public Class SupplierDetail
		Public Property SupplierID As Integer
		Public Property CurrencyID As Integer
        Public Property Cost As Decimal
    End Class

#End Region

End Class

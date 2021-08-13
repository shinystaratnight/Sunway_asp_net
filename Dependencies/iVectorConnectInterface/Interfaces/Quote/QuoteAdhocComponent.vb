Imports System.Xml.Serialization
Imports iVectorConnectInterface.GetBookingDetailsResponse

Public Class QuoteAdhocComponent
    Public Property AdHocQuoteID As Integer
    Public Property SupplierID As Integer
    Public Property SupplierReference As String
    Public Property Location As String
    Public Property ComponentRepriced As Boolean
    Public Property BookingToken As String
    Public Property TotalPrice As Decimal
    Public Property TotalCommission As Decimal
    Public Property VATOnCommission As Decimal
    Public Property ComponentType As String
    Public Property Labels As New List(Of Label)
    Public Property CustomFields As New List(Of CustomField)

End Class

Public Class Label

    Public Property Name As String
    Public Property Value As String

    Public Sub New()
    End Sub

    Public Sub New(name As String, value As String)
        Me.Name = name
        Me.Value = value
    End Sub
End Class

Public Class CustomField
    Public Property Name As String
    Public Property Value As String

    Public Sub New()
    End Sub

    Public Sub New(name As String, value As String)
        Me.Name = name
        Me.Value = value
    End Sub
End Class

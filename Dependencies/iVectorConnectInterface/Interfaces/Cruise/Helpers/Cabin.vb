Imports System.Xml.Serialization
Imports iVectorConnectInterface.GetBookingDetailsResponse
Imports iVectorConnectInterface.Support

Namespace Cruise

    Public Class Cabin
        Public Property MealBasisID As Integer
        Public Property MealBasis As String
        Public Property CabinType As String
        Public Property Adults As Integer
        Public Property Children As Integer
        Public Property Infants As Integer
        Public Property TotalPrice As Decimal

        <XmlArrayItem("GuestID")>
        Public Property GuestIDs As New List(Of Integer)
        Public Property GuestDetails As New List(Of GuestDetail)
        Public Property Adjustments As New List(Of GetBookingDetailsResponse.Adjustment)

        Public Function ShouldSerializeGuestIDs() As Boolean
            Return Me.GuestIDs.Any()
        End Function
        Public Function ShouldSerializeGuestDetails() As Boolean
            Return Me.GuestDetails.Any()
        End Function
    End Class

End Namespace

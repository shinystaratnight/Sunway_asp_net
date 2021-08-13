Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces

Namespace AdHoc
	<XmlType("AdHocPropertyBookRequest")>
	<XmlRoot("AdHocPropertyBookRequest")>
	Public Class PropertyBookRequest

#Region "Properties"
		Public Property BookingToken As String
		Public Property ExpectedTotal As Decimal
		Public Property RoomBookings As List(Of Support.RoomBooking)
#End Region


		Public Function Validate(ByVal oGuestDetails As Generic.List(Of Support.GuestDetail), Optional ValidationType As eValidationType = eValidationType.None) As List(Of String)
			Dim aWarnings As New List(Of String)

			If RoomBookings.Any(Function(o) o.GuestIDs.Any(Function(p) Not oGuestDetails.Select(Function(q) q.GuestID).Contains(p))) Then aWarnings.Add("GuestIDs on Room Booking not found in Guest Details")

			If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

			Return aWarnings
		End Function

	End Class

End Namespace
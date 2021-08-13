Imports System.Xml.Serialization

Namespace Support

	Public Class RoomBooking
		Public Property RoomBookingToken As String
		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New Generic.List(Of Integer)
	End Class

End Namespace

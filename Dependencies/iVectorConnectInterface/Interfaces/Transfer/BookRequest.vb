Imports Intuitive.Validators
Imports Intuitive.Functions
Imports System.Xml.Serialization

Namespace Transfer

    <XmlType("TransferBookRequest")>
    <XmlRoot("TransferBookRequest")>
    Public Class BookRequest

#Region "Properties"

        Public Property BookingToken As String
        Public Property ExpectedTotal As Decimal
        Public Property OutboundDetails As OutboundJourneyDetails
        Public Property ReturnDetails As ReturnJourneyDetails
		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New Generic.List(Of Integer)
        Public Property BookingTags As New Generic.List(Of Support.BookingTag)
        Public Property OverrideTransferTimes As Boolean
        Public Property DepartureNotesOverride As String
        Public Property ReturnNotesOverride As String

#End Region

#Region "Validation"

        Public Function Validate(ByVal oGuestDetails As Generic.List(Of Support.GuestDetail), Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String)

			Dim aWarnings As New Generic.List(Of String)
			Dim aGuestIDs As New ArrayList


			'get the guest ids from guestdetails
			Dim aGuestDetailIDs As New ArrayList
			For Each oGuestDetail As Support.GuestDetail In oGuestDetails
				aGuestDetailIDs.Add(oGuestDetail.GuestID)
			Next


			'booking token
			If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")


			'expected total
			'If Me.ExpectedTotal = 0 Then aWarnings.Add("An expected total must be specified.")


			'guests
			If Me.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for each transfer booking")

			For Each iGuestID As Integer In Me.GuestIDs

				'no duped guests
				If aGuestIDs.Contains(iGuestID) Then aWarnings.Add("The same Guest cannot be specified twice on an transfer booking")
				aGuestIDs.Add(iGuestID)

				'guest details for each guest
				If Not aGuestDetailIDs.Contains(iGuestID) Then aWarnings.Add("Guest Details must be specified for each Guest")
			Next


			Return aWarnings

		End Function

#End Region

#Region "Helper Classes"

        Public Class OutboundJourneyDetails
            Public Property FlightCode As String = ""
            Public Property JourneyOrigin As String = ""
            Public Property Company As String = ""
            Public Property VesselName As String = ""
            Public Property PickupTime As String = ""
            Public Property SailingTime As String = ""
            Public Property TrainDetails As String = ""
            Public Property DepartureTime As String = ""
            Public Property AccommodationName As String = ""
            Public Property AccommodationDetails As String = ""
            Public Property OutboundSupplierNote As String
        End Class

        Public Class ReturnJourneyDetails
            Public Property FlightCode As String = ""
            Public Property Company As String = ""
            Public Property VesselName As String = ""
            Public Property PickupTime As String = ""
            Public Property SailingTime As String = ""
            Public Property TrainDetails As String
            Public Property DepartureTime As String
            Public Property ReturnSupplierNote As String
        End Class

#End Region

    End Class

End Namespace
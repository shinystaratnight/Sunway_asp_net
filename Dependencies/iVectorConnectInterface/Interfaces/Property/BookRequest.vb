Imports Intuitive.Functions
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Xml.Serialization

Namespace [Property]

	<XmlType("PropertyBookRequest")>
	<XmlRoot("PropertyBookRequest")>
	Public Class BookRequest


#Region "Properties"

		Public Property BookingToken As String
		Public Property ArrivalDate As Date
		Public Property Duration As Integer
		Public Property Request As String
		Public Property ExpectedTotal As Decimal
		Public Property RoomBookings As New Generic.List(Of Support.RoomBooking)
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)
		Public Property ThirdPartyForceFail As Boolean
		Public Property PayLocal As Boolean

		'needed for package bookings
		Public Property PackageDetails As New [Property].PreBookRequest.PackageDetailsDef

        Public Property InstalmentPaymentOptionID As Integer
        <XmlIgnore()>
		Public Property hlpInvalidToken As Boolean

#End Region

#Region "Validation"

		Public Function Validate(ByVal oGuestDetails As Generic.List(Of Support.GuestDetail)) As Generic.List(Of String)

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
			If Not Me.ExpectedTotal > 0 Then
				aWarnings.Add("An Expected Total must be specified.")
			End If

			'room bookings
			If Me.RoomBookings.Count > 0 Then

				For Each oRoomBooking As Support.RoomBooking In Me.RoomBookings

					'guests
					If oRoomBooking.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for each room booking")

					For Each iGuestID As Integer In oRoomBooking.GuestIDs

						'no duped guests
						If aGuestIDs.Contains(iGuestID) Then aWarnings.Add("The same Guest cannot be specified twice on a property booking")
						aGuestIDs.Add(iGuestID)

						'guest details for each guest
						If Not aGuestDetailIDs.Contains(iGuestID) Then aWarnings.Add("Guest Details must be specified for each Guest")
					Next


				Next

			Else
				aWarnings.Add("At least one room must be specified.")
			End If

			'package
			If Not Me.PackageDetails Is Nothing AndAlso Me.PackageDetails.PackageBooking AndAlso Me.PackageDetails.TargetPrice > 0 Then

				'max tolerance
				If Not Me.PackageDetails.MaxOverAbsorbPrice > 0 Then
					aWarnings.Add("The maximum tolerance price must be specified for package bookings")
				End If

			End If

			Return aWarnings

		End Function

#End Region

	End Class

End Namespace

Imports System.Xml.Serialization
Imports Intuitive.Validators
Imports Intuitive.Functions

Namespace Flight
	<XmlType("FlightBookRequest")>
	<XmlRoot("FlightBookRequest")>
	Public Class BookRequest

#Region "Properties"

		Public Property BookingToken As String
		Public Property ExpectedTotal As Decimal

		''' <summary>
		'''     The flight and hotel.
		''' </summary>
		Public FlightAndHotel As Boolean

		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New List(Of Integer)

		Public Property APISInformation As APIS.APISInformation

		Public Property BookingTags As New List(Of Support.BookingTag)
		Public Property MultiCarrierOutbound As Boolean = False
        Public Property MultiCarrierReturn As Boolean = False

        Public Property BookingBasketID As String = ""

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


			'guests
			If Me.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for a flight booking")

			For Each iGuestID As Integer In Me.GuestIDs

				'no duped guests
				If aGuestIDs.Contains(iGuestID) Then aWarnings.Add("The same Guest cannot be specified twice on a flight booking")
				aGuestIDs.Add(iGuestID)

				'guest details for each guest
				If Not aGuestDetailIDs.Contains(iGuestID) Then aWarnings.Add("Guest Details must be specified for each Guest")

            Next

			Return aWarnings
		End Function

#End Region


#Region "Helper Classes"

		Public Class Flight
		End Class

		Public Class FlightExtra
			Public Property BookingToken As String
			Public Property Quantity As Integer
		End Class

#End Region
	End Class
End Namespace

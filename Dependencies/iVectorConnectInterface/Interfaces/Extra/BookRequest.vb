Imports System.Xml.Serialization

Namespace Extra

    <XmlType("ExtraBookRequest")>
    <XmlRoot("ExtraBookRequest")>
    Public Class BookRequest

#Region "Properties"


		Public Property BookingToken As String
		Public Property UseDate As Date
        Public Property UseTime As String
		Public Property ExpectedTotal As Decimal = -1
		Public Property LanguageID As Integer
		<XmlElement("SupplierInformation")>
		Public Property AdditionalInformation As String
		Public Property ExtraPickupPointID As Integer
		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New Generic.List(Of Integer)
        Public Property OptionalSupplements As Generic.List(Of OptionalSupplement)
        Public Property CarPark As CarParkDetails
		Public Property Details As HolidayDetails
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)
		Public Property BookingAnswers As New List(Of BookingAnswer)
		Public Property JoiningOverride As String
		Public Property SupplierNote As String

		'attraction tickets
		Public Property GroupInformation As New Generic.List(Of Extra.BookRequest.InformationItem)
		Public Property ExtraOptions As New Generic.List(Of ExtraOption)

        'Confirmation Reference
        Public Property ConfirmationReference As String

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

			'normal or option
			If Me.ExtraOptions.Count = 0 Then

				'booking token
				If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

				'expected total
				If Me.ExpectedTotal < 0 Then aWarnings.Add("An expected total must be specified.")

				'guests
				If Me.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for each extra booking")

				For Each iGuestID As Integer In Me.GuestIDs

					'no duped guests
					If aGuestIDs.Contains(iGuestID) Then aWarnings.Add("The same Guest cannot be specified twice on an extra booking")
					aGuestIDs.Add(iGuestID)

					'guest details for each guest
					If Not aGuestDetailIDs.Contains(iGuestID) Then aWarnings.Add("Guest Details must be specified for each Guest")
				Next

				'car park
				If Not Me.CarPark Is Nothing Then

					If Me.CarPark.CarMake = "" Then aWarnings.Add("A car make must be specified for the car park.")
					If Me.CarPark.CarModel = "" Then aWarnings.Add("A car model must be specified for the car park.")
					If Me.CarPark.CarColour = "" Then aWarnings.Add("A car colour must be specified for the car park.")
					If Me.CarPark.CarRegistration = "" Then aWarnings.Add("A car registration  must be specified for the car park.")

				End If


			Else

				'extra options
				For Each oExtraOption As ExtraOption In Me.ExtraOptions

					'option booking tokens
					If oExtraOption.BookingToken = "" Then aWarnings.Add("A booking token must be specified for each Extra Option.")

					'booking token
					If Me.BookingToken <> "" Then aWarnings.Add("A booking token must not be specified on the Extra when Extra Options have been.")

					'guests
					If oExtraOption.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for each extra booking option")

					For Each iGuestID As Integer In oExtraOption.GuestIDs

						'no duped guests
						If aGuestIDs.Contains(iGuestID) Then aWarnings.Add("The same Guest cannot be specified twice on an extra booking option")
						aGuestIDs.Add(iGuestID)

						'guest details for each guest
						If Not aGuestDetailIDs.Contains(iGuestID) Then aWarnings.Add("Guest Details must be specified for each Guest")
					Next

				Next

			End If

            Return aWarnings

		End Function

#End Region

#Region "Helper Classes"

        <XmlType("ExtraOptionalSupplement")>
        Public Class OptionalSupplement
            Public Property SupplementBookingToken As String
            Public Property Quantity As Integer
            Public Property Adults As Integer
            Public Property Children As Integer
            Public Property Infants As Integer

            <XmlIgnore()> Public Property hlpExtraSupplementID As Integer
        End Class

        Public Class CarParkDetails
            Public Property CarMake As String
            Public Property CarModel As String
            Public Property CarColour As String
            Public Property CarRegistration As String
        End Class

        Public Class HolidayDetails
            Public Property Hotel As String
            Public Property Resort As String
            Public Property OutboundFlightNumber As String
            Public Property ReturnFlightNumber As String
        End Class

		Public Class ExtraOption
			Public Property BookingToken As String

			<XmlArrayItem("GuestID")>
			Public Property GuestIDs As New Generic.List(Of Integer)

			Public Property IndividualInformation As Generic.List(Of Extra.BookRequest.InformationItem)

		End Class

		Public Class InformationItem
			Public Property Type As String
			Public Property Value As String
			Public Property ID As Integer
			Public Property MultiValues As New Generic.List(Of MultiValue)
		End Class

		Public Class MultiValue
			Public Property Name As String
			Public Property Selected As Boolean
		End Class
#End Region

    End Class

End Namespace

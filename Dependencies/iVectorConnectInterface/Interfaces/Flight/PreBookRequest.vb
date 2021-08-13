Imports Intuitive.DateFunctions
Imports Intuitive.Functions
Imports System.Xml.Serialization

Namespace Flight

	<XmlType("FlightPreBookRequest")>
	<XmlRoot("FlightPreBookRequest")>
	Public Class PreBookRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

		Public Property BookingToken As String

		Public Property FlightAndHotel As Boolean
		Public Property MultiCarrierOutbound As Boolean = False
		Public Property MultiCarrierReturn As Boolean = False
		Public Property SeatMaps As Boolean = False
		Public Property Extras As New Generic.List(Of Flight.PreBookRequest.Extra)
        Public Property SelectedFareFamilyName As String
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)
		Public Property BookingBasketID As String = ""

#End Region

#Region "Helper Classes"

		Public Class Extra
			Public Property ExtraBookingToken As String
			Public Property Quantity As Integer
			Public Property RequestedExtraType As String
			Public Property GuestID As Integer
            Public Property ExtraGroup As String

			'store extra seat details at book only
			Public Property SeatDetails As SeatDetailsDef

			Public Class SeatDetailsDef

				Public Property FlightBookingSeatID As Integer
				Public Property FlightBookingID As Integer
				Public Property FlightBookingMultiSectorID As Integer
				Public Property FlightBookingPassengerID As Integer
				Public Property RowReference As String
				Public Property SeatReference As String
				Public Property TPReference As String
				Public Property LocalCost As Decimal
				Public Property TotalCost As Decimal
				Public Property TotalPrice As Decimal
				Public Property Selected As Boolean = False
				Public Property hlpColour As String

				Public Property RequiredQuantity As Integer = 1
				Public Property BookedQuantity As Integer

				Public Property FlightBookingSeatAttributes As New Generic.List(Of FlightBookingSeatAttribute)

			End Class

			Public Class FlightBookingSeatAttribute
				Public Property FlightBookingSeatAttributeID As Integer
				Public Property FlightBookingSeatID As Integer
				Public Property Attribute As String
				Public Property hlpColour As String
			End Class

		End Class

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

			Dim aWarnings As New Generic.List(Of String)

			If Me.BookingToken = "" Then aWarnings.Add("A Booking Token must be specified.")

			'flight extras
			Dim oArraylist As New ArrayList

			For Each oExtra As Extra In Me.Extras

				If oExtra.ExtraBookingToken = "" Then
					aWarnings.Add("A booking token must be specified for each flight extra.")
				ElseIf Not IsEncrypted(oExtra.ExtraBookingToken) Then
					aWarnings.Add("Invalid extra booking token entered.")
				End If

				If oExtra.RequestedExtraType = "Seat" Then

					If oExtra.Quantity = 0 Then aWarnings.Add("Each Seat must have a quantity selected.")
					If oExtra.Quantity > 1 Then aWarnings.Add("Only one of each seat can be booked.")

					If Not oExtra.GuestID > 0 Then aWarnings.Add("Each Seat must have a GuestID specified.")

				ElseIf oExtra.RequestedExtraType = "" Then

					If oExtra.GuestID > 0 Then aWarnings.Add("A Guest ID cannot be entered for this extra type.")
				Else
					aWarnings.Add("Unrecognised Requested Extra Type.")
				End If

				'check extras are only entered once
				If oArraylist.Contains(oExtra.ExtraBookingToken) Then
					aWarnings.Add("Each Extra can only be selected once.")
				Else
					oArraylist.Add(oExtra.ExtraBookingToken)
				End If

			Next

			Return aWarnings

		End Function

#End Region

	End Class

End Namespace
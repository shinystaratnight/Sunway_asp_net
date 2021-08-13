Imports Intuitive.Functions

Public Class ReserveSeatsRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property BookingReference As String
	Public Property FlightBookingID As Integer

	Public Property Extras As New Generic.List(Of Flight.PreBookRequest.Extra)
	Public Property Payment As Support.PaymentDetails

#End Region

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be provided.")
		If Me.FlightBookingID = 0 Then aWarnings.Add("A flight booking id must be provided.")

		'flight extras
		Dim oArraylist As New ArrayList

		For Each oExtra As Flight.PreBookRequest.Extra In Me.Extras

			If oExtra.ExtraBookingToken = "" Then
				aWarnings.Add("A booking token must be specified for each flight extra.")
			ElseIf Not IsEncrypted(oExtra.ExtraBookingToken) Then
				aWarnings.Add("Invalid extra booking token entered.")
			End If

			If oExtra.RequestedExtraType <> "Seat" AndAlso oExtra.RequestedExtraType <> "" Then
				aWarnings.Add("Unrecognised Requested Extra Type.")
			End If

			If oExtra.Quantity = 0 Then aWarnings.Add("Each Seat must have a quantity selected.")
			If oExtra.Quantity > 1 Then aWarnings.Add("Only one of each seat can be booked.")

			If Not oExtra.GuestID > 0 Then aWarnings.Add("Each Seat must have a GuestID specified.")

			'check extras are only entered once
			If oArraylist.Contains(oExtra.ExtraBookingToken) Then
				aWarnings.Add("Each Extra can only be selected once.")
			Else
				oArraylist.Add(oExtra.ExtraBookingToken)
			End If

		Next

		'payment 
		If Not Me.Payment Is Nothing Then
			aWarnings.AddRange(Me.Payment.Validate)
		End If

		Return aWarnings

	End Function

#End Region

#Region "Helper Classes"
	Public Class FlightPassengerSeat
		Public Property FlightBookingPassengerID As Integer
		Public Property OutboundCharterFlightInventorySeatID As Integer
		Public Property ReturnCharterFlightInventorySeatID As Integer
	End Class
#End Region

End Class
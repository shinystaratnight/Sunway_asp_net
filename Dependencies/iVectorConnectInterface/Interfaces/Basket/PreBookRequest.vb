Imports System.Xml.Serialization

Namespace Basket

    <XmlRoot("BasketPreBookRequest")>
    Public Class PreBookRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
        Public Property ThirdPartyForceFail As Boolean

        <XmlArrayItem("PropertyPreBookRequest")>
        Public Property PropertyBookings As New Generic.List(Of [Property].PreBookRequest)
        <XmlArrayItem("FlightPreBookRequest")>
        Public Property FlightBookings As New Generic.List(Of Flight.PreBookRequest)
        <XmlArrayItem("TransferPreBookRequest")>
        Public Property TransferBookings As New Generic.List(Of Transfer.PreBookRequest)
        <XmlArrayItem("ExtraPreBookRequest")>
        Public Property ExtraBookings As New Generic.List(Of Extra.PreBookRequest)
        <XmlArrayItem("CarHirePreBookRequest")>
        Public Property CarHireBookings As New Generic.List(Of CarHire.PreBookRequest)


#Region "validate"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

			Dim aWarnings As New Generic.List(Of String)
            If Me.PropertyBookings.Count + Me.FlightBookings.Count + Me.TransferBookings.Count + Me.ExtraBookings.Count + Me.CarHireBookings.Count = 0 Then
                aWarnings.Add("At least one Booking Component must be specified")
            End If

			'sub validation
			'property
            For Each oPropertyPrebookRequest As [Property].PreBookRequest In Me.PropertyBookings
                aWarnings.AddRange(oPropertyPrebookRequest.Validate())
            Next

			'flight
			Dim bMultiCarrierOutbound As Boolean = False
			Dim bMultiCarrierReturn As Boolean = False

			For Each oFlightPreBookRequest As Flight.PreBookRequest In Me.FlightBookings
				aWarnings.AddRange(oFlightPreBookRequest.Validate)

				'mix and match - check we have corresponding flight
				If oFlightPreBookRequest.MultiCarrierOutbound Then
					bMultiCarrierOutbound = True
				End If

				If oFlightPreBookRequest.MultiCarrierReturn Then
					bMultiCarrierReturn = True
				End If

			Next

			'mix and match check
			If bMultiCarrierOutbound <> bMultiCarrierReturn Then

				If bMultiCarrierOutbound Then
					aWarnings.Add("A Mix And Match Return Flight must be specified along with the Mix And Match Outbound Flight")
				ElseIf bMultiCarrierReturn Then
					aWarnings.Add("A Mix And Match Outbound Flight must be specified along with the Mix And Match Return Flight")
				End If

			End If


			'transfer
			For Each oTransferPrebookRequest As Transfer.PreBookRequest In Me.TransferBookings
				aWarnings.AddRange(oTransferPrebookRequest.Validate)
			Next

			'extra
			For Each oExtraPrebookRequest As Extra.PreBookRequest In Me.ExtraBookings
				aWarnings.AddRange(oExtraPrebookRequest.Validate)
			Next

			'carhire
			For Each oCarHirePrebookRequest As CarHire.PreBookRequest In Me.CarHireBookings
				aWarnings.AddRange(oCarHirePrebookRequest.Validate)
			Next

			'for packages we should have one property and one flight component
			If Me.PropertyBookings.Count > 0 AndAlso Me.PropertyBookings(0).PackageDetails.PackageBooking Then

				'property
				If Me.PropertyBookings.Count > 1 Then
					aWarnings.Add("Only one property component can be specified for a package booking")
				End If

				'flights
				If Me.FlightBookings.Count = 0 Then
					aWarnings.Add("One flight component must be specified for package bookings")
				End If

				If Me.FlightBookings.Count > 1 Then
					aWarnings.Add("Only one flight component can be specified for a package booking")
				End If

			End If

			Return aWarnings
		End Function

#End Region

    End Class

End Namespace

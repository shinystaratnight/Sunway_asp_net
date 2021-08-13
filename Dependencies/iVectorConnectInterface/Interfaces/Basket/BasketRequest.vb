Imports Intuitive.Validators
Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces
Imports iVectorConnectInterface.Support

Namespace Basket

	Public Class BasketRequest
        Implements iVectorConnectRequest


		Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
		Public Property TradeReference As String
		Public Property TradeContactID As Integer
		Public Property PromotionalCode As String
		Public Property LeadCustomer As Support.LeadCustomerDetails
		Public Property GuestDetails As New Generic.List(Of Support.GuestDetail)
		<XmlIgnore()> Public Property DataStorageKey As String
		<XmlArrayItem("PropertyBookRequest")>
		Public Property PropertyBookings As New Generic.List(Of [Property].BookRequest)
		<XmlArrayItem("FlightBookRequest")>
		Public Property FlightBookings As New Generic.List(Of Flight.BookRequest)
		<XmlArrayItem("TransferBookRequest")>
		Public Property TransferBookings As New Generic.List(Of Transfer.BookRequest)
		<XmlArrayItem("ExtraBookRequest")>
		Public Property ExtraBookings As New Generic.List(Of Extra.BookRequest)
		<XmlArrayItem("CarHireBookRequest")>
		Public Property CarHireBookings As New Generic.List(Of CarHire.BookRequest)
		<XmlArrayItem("AdHocPropertyBookRequest")>
		Public Property AdHocPropertyBookings As New Generic.List(Of AdHoc.PropertyBookRequest)
		<XmlArrayItem("Comment")>
		Public Property Comments As New Generic.List(Of Comment)
		<XmlArrayItem("OwnArrangement")>
		Public Property OwnArrangements As New Generic.List(Of OwnArrangement)
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)
		Public Property BookingAdjustments As New Generic.List(Of Support.BookingAdjustmentDef)
        Public Property BondingID As Integer
        
        Public Overridable Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

			Dim aWarnings As New Generic.List(Of String)

			If ValidationType = Interfaces.eValidationType.None Then
				Throw New Exception("A Validation Type must be specified for this request")
			End If

			'validate the components
			'property
			For Each oPropertyBookingRequest As [Property].BookRequest In Me.PropertyBookings
				aWarnings.AddRange(oPropertyBookingRequest.Validate(Me.GuestDetails))
			Next

			'transfer
			For Each oTransferBookingRequest As Transfer.BookRequest In Me.TransferBookings
				aWarnings.AddRange(oTransferBookingRequest.Validate(Me.GuestDetails))
			Next

			'extras
			For Each oExtraBookingRequest As Extra.BookRequest In Me.ExtraBookings
				aWarnings.AddRange(oExtraBookingRequest.Validate(Me.GuestDetails))
			Next

			'carhire
			For Each oCarHireBooking As CarHire.BookRequest In Me.CarHireBookings
				aWarnings.AddRange(oCarHireBooking.Validate(Me.GuestDetails))
			Next

			'ad hoc
			For Each oAdHocPropertyBookingRequest As AdHoc.PropertyBookRequest In Me.AdHocPropertyBookings
				aWarnings.AddRange(oAdHocPropertyBookingRequest.Validate(Me.GuestDetails))
			Next

			If Me.PropertyBookings.Count +
				Me.FlightBookings.Count +
				Me.TransferBookings.Count +
				Me.ExtraBookings.Count +
				Me.CarHireBookings.Count +
				Me.AdHocPropertyBookings.Count = 0 Then
				aWarnings.Add("At least one component must be provided")
			End If

			'flight
			Dim bMultiCarrierOutbound As Boolean = False
			Dim bMultiCarrierReturn As Boolean = False

			For Each oFlightBookRequest As Flight.BookRequest In Me.FlightBookings
				aWarnings.AddRange(oFlightBookRequest.Validate(Me.GuestDetails))

				'mix and match - check we have corresponding flight
				If oFlightBookRequest.MultiCarrierOutbound Then
					bMultiCarrierOutbound = True
				End If

				If oFlightBookRequest.MultiCarrierReturn Then
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

			If Me.GuestDetails.FirstOrDefault(Function(o) o.Type = "Infant" AndAlso Not String.IsNullOrEmpty(o.FrequentFlyerNumber)) IsNot Nothing Then
				aWarnings.Add("A frequent flyer number cannot be specified for an infant")
			End If

			'for packages we should have one property and one flight component
			If Me.PropertyBookings.Count > 0 AndAlso Not Me.PropertyBookings(0).PackageDetails Is Nothing AndAlso
			 Me.PropertyBookings(0).PackageDetails.PackageBooking Then

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

			'booking adjustment
			For Each oBookingAdjustment As Support.BookingAdjustmentDef In Me.BookingAdjustments

				If oBookingAdjustment.AdjustmentType = "" Then aWarnings.Add("A Booking Adjustment Type must be specified")
				If oBookingAdjustment.AdjustmentAmount = 0 AndAlso
					oBookingAdjustment.CustomerAdjustmentAmount.ToSafeDecimal() = 0 Then
					aWarnings.Add("A Booking Adjustment Amount must be specified or a CustomerAdjustmentAmount must be specified")
				End If

			Next

		    'guest details
		    If Me.GuestDetails.Count > 0 Then
		        For Each oGuestDetail As Support.GuestDetail In Me.GuestDetails
		            aWarnings.AddRange(oGuestDetail.Validate())
		        Next

		        'guestids must be unique
		        Dim aGuestIDs As New ArrayList
		        For Each oGuestDetail As Support.GuestDetail In Me.GuestDetails
		            If aGuestIDs.Contains(oGuestDetail.GuestID) Then aWarnings.Add("The GuestIDs must be unique")
		            aGuestIDs.Add(oGuestDetail.GuestID)
		        Next
		    End If

			Return aWarnings
		End Function

	End Class
    Public Class Comment
        Public Property CommentID As Integer = 0
        Public Property CommentDescription As String = ""
        Public Sub New()
        End Sub
    End Class
	Public Class OwnArrangement
		Public Property Title As String
		Public Property Description As String
		Public Property StartDate As Date
		Public Property EndDate As Date
		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New Generic.List(Of Integer)
	End Class
End Namespace
Imports Intuitive.Validators
Imports System.Xml.Serialization

Public Class AddComponentRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest


#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

    Public Property BookingReference As String
	Public Property GuestDetails As New Generic.List(Of Support.GuestDetail)
	Public Property LeadCustomer As Support.LeadCustomerDetails


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

    Public Property Payment As Support.PaymentDetails
	Public Property PayPalTransactions As New Generic.List(Of Support.PayPalTransaction)

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'booking reference
        If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be specified.")

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
        Else
            aWarnings.Add("At least one Guest must be specified")
        End If

        If Me.PropertyBookings.Count + Me.TransferBookings.Count + _
   Me.FlightBookings.Count + Me.ExtraBookings.Count + Me.CarHireBookings.Count = 0 Then
            aWarnings.Add("At least one Component must be specified")
        End If

        'payment 
        If Not Me.Payment Is Nothing Then
            aWarnings.AddRange(Me.Payment.Validate)
        End If

		'lead customer
		If Not Me.LeadCustomer Is Nothing Then
			aWarnings.AddRange(Me.LeadCustomer.Validate(ValidationType))
		End If

		If Not Me.PayPalTransactions Is Nothing Then

			For Each oPayPalTransaction As Support.PayPalTransaction In Me.PayPalTransactions

				If oPayPalTransaction.AmountPaid = Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				ElseIf oPayPalTransaction.TransactionID = Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				ElseIf oPayPalTransaction.CorrelationID = Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				ElseIf oPayPalTransaction.PayerID = Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				ElseIf oPayPalTransaction.SellerProtectionEligibility = Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				ElseIf oPayPalTransaction.EmailAddress Is Nothing Then
					aWarnings.Add("Must Specify all details of each PayPalTransaction.")
				End If

			Next

		End If

        Return aWarnings

    End Function

#End Region

End Class

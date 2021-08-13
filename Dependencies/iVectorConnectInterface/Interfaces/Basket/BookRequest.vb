Imports Intuitive.Validators
Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces

Namespace Basket
	<XmlRoot("BasketBookRequest")>
	Public Class BookRequest
		Inherits BookableBasketRequest
		Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest, Interfaces.IEncryptedRequest

#Region "Properties"
		Public Property Payment As Support.PaymentDetails
		<XmlIgnore()> Public Property BookingID As Integer
		Public Property QuoteDetails As New Quote
		Public Property PayPalTransactions As New Generic.List(Of Support.PayPalTransaction)
		Public Property ForceCalculateBookingTotals As Boolean
		Public Property ThirdPartyForceFail As Boolean
		Public Property PreBookingStoreToken As String
		Public Property OverrideTotalPrice As Decimal

        <XmlIgnore()> Public ReadOnly Property PaymentProcessingAllowed() As Boolean
			Get
				Return (Me.Payment.TotalAmount > 0 AndAlso Me.Payment.Amount > 0) OrElse Me.HasPayLocal
			End Get
		End Property


		Public Sub Decrypt(EncryptionKey As String, EncryptionType As Security.SecretKeeper.EncryptionType) Implements IEncryptedRequest.Decrypt

			If Me.Payment IsNot Nothing Then
				Me.Payment.Decrypt(EncryptionKey, EncryptionType)
			End If

		End Sub

        Public ReadOnly Property HasPayLocal As Boolean
            Get
                Return Me.CarHireBookings.Any(Function(o) o.PayLocal) OrElse Me.PropertyBookings.Any(Function(o) o.PayLocal)
            End Get
        End Property
        Public ReadOnly Property TakePayment As Boolean
            Get
                Return Me.Payment IsNot Nothing AndAlso Me.Payment.TotalAmount > 0
            End Get
        End Property

#End Region

#Region "Validation"

        Public Overrides Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String)

			Dim aWarnings As New Generic.List(Of String)

            aWarnings.AddRange(MyBase.Validate(ValidationType))
			aWarnings.AddRange(QuoteValidate(ValidationType))
            aWarnings.AddRange(PaypalValidate())

            Return aWarnings

		End Function

        Private Function QuoteValidate(ValidationType As Interfaces.eValidationType) As List(Of String)

            Dim aWarnings As New Generic.List(Of String)

            If Not Me.QuoteDetails.CreateQuote Then

                'lead customer details
                If Not Me.LeadCustomer Is Nothing Then
                    aWarnings.AddRange(Me.LeadCustomer.Validate(ValidationType))
                End If

                'payment 
                If Me.HasPayLocal AndAlso (Me.Payment Is Nothing OrElse Me.Payment.PaymentType = "" OrElse Me.Payment.PaymentTypeEnum <> Support.PaymentDetails.ePaymentType.CreditCard) Then
                    aWarnings.Add("Bookings with Pay Local Components require a credit card")
                End If

                If Not Me.Payment Is Nothing Then
                    aWarnings.AddRange(Me.Payment.Validate(Me.HasPayLocal OrElse Me.FlightBookings.Any()))
                End If

            Else

                If Me.QuoteDetails.Title = "" Then aWarnings.Add("A Title must be specified.")

                If Me.QuoteDetails.FirstName = "" Then aWarnings.Add("A First Name must be specified.")

                If Me.QuoteDetails.LastName = "" Then aWarnings.Add("A Last Name must be specified.")

                If Not QuoteDetails.SuppressEmail Then

                    If Me.QuoteDetails.Email = "" Then aWarnings.Add("An Email must be specified.")
                    If Not IsEmail(Intuitive.Functions.SafeString(Me.QuoteDetails.Email)) Then aWarnings.Add("Invalid Email Address")

                    If Not Me.QuoteDetails.FriendEmails Is Nothing Then
                        For Each sEmail As String In Me.QuoteDetails.FriendEmails.Split(","c)
                            If Not IsEmail(Intuitive.Functions.SafeString(Me.QuoteDetails.Email)) Then aWarnings.Add("Invalid Email Address")
                        Next
                    End If

                End If

            End If

            Return aWarnings

        End Function

        Private Function PaypalValidate() As List(Of String)
            Dim aWarnings As New Generic.List(Of String)

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

#Region "helper classes"

        Public Class Quote
			Public Property CreateQuote As Boolean
			Public Property Title As String
			Public Property FirstName As String
			Public Property LastName As String
			Public Property Email As String
			Public Property Phone As String
			Public Property CustomerID As Integer
			Public Property ExternalReference As String
			Public Property FriendEmails As String
			Public Property SuppressEmail As Boolean
		End Class
#End Region

	End Class

End Namespace

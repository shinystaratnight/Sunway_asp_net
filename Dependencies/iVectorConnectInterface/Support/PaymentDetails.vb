Imports System.Xml.Serialization

Namespace Support

	Public Class PaymentDetails
		Public Property PaymentType As String
		Public Property CCCardHoldersName As String
		Public Property CCCardTypeID As Integer
		Public Property CCCardNumber As String
		Public Property CCStartMonth As String
		Public Property CCStartYear As String
		Public Property CCExpireMonth As String
		Public Property CCExpireYear As String
		Public Property CCSecurityCode As String
		Public Property CCIssueNumber As Integer
		Public Property TransactionID As String
		Public Property Amount As Decimal
		Public Property Surcharge As Decimal
		Public Property TotalAmount As Decimal
		Public Property ThreeDSecureCode As String
		Public Property PaymentToken As String
		Public Property PreAuthorisationOnly As Boolean = False
		Public Property OffsitePaymentTypeID As Integer

		'store BasketstoreID if needed to be retrieved later (for 3Dsecure redirects)
		Public Property BasketReference As String

		'billing address
		Public Property BillingAddress As Address

		'override properties
		Public Property OverridePaymentCurrency As Boolean = False
		Public Property OverridePaymentCurrencyID As Integer
		Public Property OverrideExchangeRate As Decimal
		Public Property OverridePaymentAmount As Decimal
		Public Property EncryptedCardDetails As String
		Public Property UseEncryptedCardDetails As Boolean

		<XmlIgnore()>
		Private EncryptionKey As String
		<XmlIgnore()>
		Private EncryptionType As Security.SecretKeeper.EncryptionType

		'helper n.b the horrible name is because we had a clash with another field being used in the deserialize query string from another project.
		'seemed the least worst option
		Public ReadOnly Property PaymentTypeEnum As ePaymentType
			Get
				If PaymentType.Replace(" ", "") = "CreditCard" Then
					Return ePaymentType.CreditCard
				ElseIf PaymentType = "OFP_Deferred_Payment" Then
					Return ePaymentType.OFPDeferredPayment
				ElseIf PaymentType.Replace(" ", "") = "CustomWithMakePayment" Then
					Return ePaymentType.CustomWithMakePayment
				ElseIf PaymentType.Replace(" ", "") = "OffsiteMakePayment" Then
					Return ePaymentType.OffsiteMakePayment
				ElseIf PaymentType.Replace(" ", "") = "OffsitePaymentTaken" Then
					Return ePaymentType.OffsitePaymentTaken
				ElseIf PaymentType.Replace(" ", "") = "Token" Then
					Return ePaymentType.Token
				Else
					Return ePaymentType.Custom
				End If
			End Get
		End Property

		Public Function Validate(Optional PayLocal As Boolean = False) As List(Of String)

			Dim aWarnings As New List(Of String)

			If String.IsNullOrEmpty(Me.PaymentType) Then
				aWarnings.Add(WarningMessage.PaymentTypeNotSpecified)
			Else

				If Me.PaymentTypeEnum = ePaymentType.CreditCard Then

					If Not Me.CCCardTypeID > 0 Then aWarnings.Add(WarningMessage.CardTypeNotSpecified)
					If Not Me.CCCardHoldersName <> "" Then aWarnings.Add(WarningMessage.CardHolderNameNotSpecified)

					If Me.EncryptedCardDetails.ToSafeString = "" AndAlso Me.CCCardNumber = "" AndAlso Me.CCExpireMonth = "" AndAlso Me.CCExpireYear = "" Then
						aWarnings.Add(WarningMessage.CardDetailsNotSpecified)
					ElseIf Me.EncryptedCardDetails.ToSafeString = "" Then
						If Not Me.CCCardNumber <> "" Then aWarnings.Add(WarningMessage.CardNumberNotSpecified)
						If Not Me.CCExpireMonth <> "" Then aWarnings.Add(WarningMessage.CardExpireMonthNotSpecified)
						If Not Me.CCExpireYear <> "" Then aWarnings.Add(WarningMessage.CardExpireYearNotSpecified)
					End If

					'ensure the credit card start/expiry Years are only 2 digits
					If Not Me.EncryptedCard Then
						Me.CCExpireYear = Right(Me.CCExpireYear, 2)
						Me.CCStartYear = Right(Me.CCStartYear, 2)
					End If

				End If

				If Me.PaymentTypeEnum = ePaymentType.OffsiteMakePayment Then
					If Not Me.TransactionID <> "" Then aWarnings.Add(WarningMessage.TransactionIDNotSpecified)
				End If

				If Me.PaymentTypeEnum = ePaymentType.Token Then
					If String.IsNullOrWhiteSpace(Me.PaymentToken) Then aWarnings.Add(WarningMessage.PaymentTokenNotSpecified)
				End If

				'validate amount (allow zero amounts if some components are pay local)
				If Not PayLocal Then
					If Me.Amount = 0 Then aWarnings.Add(WarningMessage.NonZeroAmountNotSpecified)
					If Me.TotalAmount = 0 Then aWarnings.Add(WarningMessage.NonZeroTotalAmountNotSpecified)
				End If

				'billing address
				If Not Me.BillingAddress Is Nothing Then
					If Me.BillingAddress.Address1 = "" Then aWarnings.Add(WarningMessage.BillingAddress1NotSpecified)
					If Me.BillingAddress.TownCity = "" Then aWarnings.Add(WarningMessage.BillingTownCityNotSpecified)
					If Me.BillingAddress.Postcode = "" Then aWarnings.Add(WarningMessage.BillingPostCodeNotSpecified)
					If Not Me.BillingAddress.BookingCountryID > 0 Then aWarnings.Add(WarningMessage.BillingBookingCountryIDNotSpecified)
				End If

				'override fields
				If Me.OverridePaymentCurrency Then
					If Me.OverridePaymentCurrencyID.ToSafeInt = 0 Then aWarnings.Add(WarningMessage.OverrideCurrencyIDNotSpecified)
					If Me.OverrideExchangeRate.ToSafeDecimal = 0 Then aWarnings.Add(WarningMessage.OverrideExchangeRateNotSpecified)
					If Me.OverridePaymentAmount.ToSafeDecimal = 0 Then aWarnings.Add(WarningMessage.OverridePaymentAmountNotSpecified)
				End If

			End If

			Return aWarnings

		End Function

		Public Enum ePaymentType
			CreditCard
			Custom
			OFPDeferredPayment
			CustomWithMakePayment 'Need to take payments in the normal way using the interfaces but don't have all the card details as this is handled offsite
			OffsiteMakePayment 'the payment can be pre authed offsite and then payment confirmed in basketbook
			OffsitePaymentTaken 'the offsite payment was taken at Pre-Auth
			Token
		End Enum

		Public Sub Decrypt(EncryptionKey As String, EncryptionType As Security.SecretKeeper.EncryptionType)
			Me.EncryptionKey = EncryptionKey
			Me.EncryptionType = EncryptionType
			If Me.EncryptedCard Then
				With Me
					.CCCardNumber = DecryptField(Me.CCCardNumber)
					.CCExpireMonth = DecryptField(Me.CCExpireMonth)
					.CCExpireYear = DecryptField(Me.CCExpireYear)
					.CCSecurityCode = DecryptField(Me.CCSecurityCode)
					.CCExpireYear = Right(Me.CCExpireYear, 2)
					.CCStartYear = Right(Me.CCStartYear, 2)
				End With
			End If
		End Sub

		Public ReadOnly Property EncryptedCard As Boolean
			Get
				If Me.PaymentTypeEnum = Support.PaymentDetails.ePaymentType.CreditCard Then
					Return Not String.IsNullOrEmpty(DecryptField(Me.CCCardNumber))
				End If
				Return False
			End Get
		End Property

		Public Function DecryptField(ByVal FieldToDecrypt As String) As String

			If Me.EncryptionKey = String.Empty Then
				Return String.Empty
			End If

			Dim oIntuitiveSecretKeeper As New Security.SecretKeeper(Me.EncryptionKey, Me.EncryptionType, Security.Cryptography.CipherMode.ECB)

			Dim sResult As String = oIntuitiveSecretKeeper.SafeDecryptHex(FieldToDecrypt)

			If sResult = String.Empty Then
				sResult = oIntuitiveSecretKeeper.SafeDecryptHex(FieldToDecrypt.Substring(2, FieldToDecrypt.Length - 3))
			End If

			If sResult = String.Empty Then
				sResult = oIntuitiveSecretKeeper.SafeDecrypt(FieldToDecrypt)
			End If

			Return sResult

		End Function

	End Class

End Namespace
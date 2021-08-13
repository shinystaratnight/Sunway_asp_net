Imports Intuitive.Functions

Public Class Get3DSecureRedirectRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest


	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property UserIPAddress As String
	Public Property BrowserAcceptHeader As String
	Public Property UserAgentHeader As String
	Public Property Payment As Support.PaymentDetails
	Public Property BookingReference As String
	Public Property RedirectURL As String

	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)
		Dim bError As Boolean = False

		If Me.Payment Is Nothing Then aWarnings.Add("Payment details must be specified.")

		'Check card is of a valid format
		With Me.Payment

			If Me.Payment.PaymentType.ToLower = "creditcard" OrElse Me.Payment.PaymentType.ToLower = "credit card" Then

				Dim iStartYearLength As Integer = 0
				If .CCStartYear IsNot Nothing Then
					iStartYearLength = .CCStartYear.Length
				End If

				Dim iExpireYearLength As Integer = 0
				If .CCExpireYear IsNot Nothing Then
					iExpireYearLength = .CCExpireYear.Length
				End If

				If .CCCardHoldersName = "" Then
					aWarnings.Add("Card holder name must be specified.")
					bError = True
				End If

				If .CCCardNumber = "" Then
					aWarnings.Add("Card number must be specified.")
					bError = True
				End If

				If .CCStartMonth <> "" AndAlso (SafeInt(.CCStartMonth) < 1 OrElse SafeInt(.CCStartMonth) > 12 OrElse .CCStartYear = "") Then
					aWarnings.Add("Invalid card start date supplied.")
					bError = True
				End If

				If .CCStartYear <> "" AndAlso .CCStartMonth = "" Then
					aWarnings.Add("Invalid card start date supplied.")
					bError = True
				End If

				Dim iCurrentDate As Integer = SafeInt(Now.ToString("yyyyMM"))

				If .CCStartMonth <> "" AndAlso .CCStartYear <> "" Then

					Dim sStartMonth As String = IIf(.CCStartMonth.Length = 1, "0" & .CCStartMonth, .CCStartMonth)
					Dim iStartDate As Integer = SafeInt(Now.Year.ToString.Substring(0, 4 - iStartYearLength) & .CCStartYear & sStartMonth)

					If iStartDate > iCurrentDate Then
						aWarnings.Add("Card start date must not be in the future.")
						bError = True
					End If

				End If

				Dim sExpireMonth As String = IIf(.CCExpireMonth.Length = 1, "0" & .CCExpireMonth, .CCExpireMonth)
				Dim iExpiryDate As Integer = SafeInt(Now.Year.ToString.Substring(0, 4 - iExpireYearLength) & .CCExpireYear & sExpireMonth)

				If .CCExpireMonth = "" OrElse .CCExpireYear = "" Then
					aWarnings.Add("Card expiry date must be specified.")
					bError = True
				ElseIf SafeInt(.CCExpireMonth) < 1 OrElse SafeInt(.CCExpireMonth) > 12 Then
					aWarnings.Add("Invalid card expiry month supplied.")
					bError = True
				ElseIf iExpiryDate < iCurrentDate Then
					aWarnings.Add("Expiry date must not be in the past.")
					bError = True
				End If

				If .CCIssueNumber < 0 Then
					aWarnings.Add("Invalid credit card issue number supplied.")
					bError = True
				End If

			End If



		End With

		'payment 
		If Not Me.Payment Is Nothing AndAlso bError = False Then

			aWarnings.AddRange(Me.Payment.Validate)

		End If

		Return aWarnings

	End Function
End Class

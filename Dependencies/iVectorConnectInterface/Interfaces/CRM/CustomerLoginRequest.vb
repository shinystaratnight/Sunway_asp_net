Imports Intuitive.Validators

Public Class CustomerLoginRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

   Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property Email As String
	Public Property Password As String

	Public Property BookingReference As String
	Public Property LastName As String
	Public Property DepartureDate As Date

	Public Property BrandID As Integer = 0

	Public Enum LoginMethod
		EmailAndReference
		EmailAndPassword
        BookingDetails
        NoBooking
	End Enum

	Public Property LoginBy As LoginMethod

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
		Dim aWarnings As New Generic.List(Of String)


		If LoginBy = LoginMethod.EmailAndPassword Then

			' Check email and password
			If Not IsEmail(Intuitive.Functions.SafeString(Me.Email)) Then
				aWarnings.Add("A valid email address must be specified.")
			End If
			If Me.Password = "" Then
				aWarnings.Add("A password must be specified.")
			End If

		ElseIf LoginBy = LoginMethod.BookingDetails Then

			' Check last name, booking reference and departure date
			If Me.BookingReference = "" Then
				aWarnings.Add("A booking reference must be specified.")
			End If
			If Me.LastName = "" OrElse Me.DepartureDate = Nothing Then
				aWarnings.Add("Last name and departure date must be specified.")
			End If

        ElseIf LoginBy = LoginMethod.NoBooking Then

            If Not IsEmail(Intuitive.Functions.SafeString(Me.Email)) Then
                aWarnings.Add("A valid email address must be specified.")
            End If
            If Me.Password = "" Then
                aWarnings.Add("A password must be specified.")
            End If

        Else

            ' Default to : Check booking reference and email
            If Me.BookingReference = "" Then
                aWarnings.Add("A booking reference must be specified.")
            End If
            If Not IsEmail(Intuitive.Functions.SafeString(Me.Email)) Then
                aWarnings.Add("A valid email address must be specified.")
            End If

		End If

		Return aWarnings

	End Function

#End Region

End Class

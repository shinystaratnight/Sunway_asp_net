Imports Intuitive.Validators

Public Class NewsletterSignupRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property Email As String
	Public Property Customer As NewsletterSignupCustomer

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

		'if we don't have a request email, check if we have a customer and an email on the customer.
		If Not IsEmail(Intuitive.Functions.SafeString(Me.Email)) Then

			If Me.Customer Is Nothing OrElse Not IsEmail(Intuitive.Functions.SafeString(Me.Customer.Email)) Then
				aWarnings.Add("A valid email must be specified")
			End If
		End If

		Return aWarnings

	End Function

#End Region

	Public Class NewsletterSignupCustomer

#Region "Properties"

	    Private _TitleID As Integer
		Private _Forename As String
		Private _Surname As String
		Private _Postcode As String
		Private _Email As String
		Private _ReceivesNewsLetter As Boolean

#End Region

#Region "accessors and mutators"

	    Public Property TitleID As Integer
	        Get
	            Return Me._TitleID
	        End Get
	        Set(value As Integer)
	            Me._TitleID = value
	        End Set
	    End Property

		Public Property Forename As String
			Get
				Return Me._Forename
			End Get
			Set(value As String)
				Me._Forename = value
			End Set
		End Property

		Public Property Surname As String
			Get
				Return Me._Surname
			End Get
			Set(value As String)
				Me._Surname = value
			End Set
		End Property

		Public Property Postcode As String
			Get
				Return Me._Postcode
			End Get
			Set(value As String)
				Me._Postcode = value
			End Set
		End Property

		Public Property Email As String
			Get
				Return Me._Email
			End Get
			Set(value As String)
				Me._Email = value
			End Set
		End Property

		Public Property ReceivesNewsLetter As Boolean
			Get
				Return Me._ReceivesNewsLetter
			End Get
			Set(value As Boolean)
				Me._ReceivesNewsLetter = value
			End Set
		End Property

#End Region

	End Class

End Class

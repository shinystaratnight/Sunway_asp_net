Imports System.Xml.Serialization
Imports Intuitive.Validators

Namespace Support

	Public Class LeadCustomerDetails
		Public Property CustomerTitle As String = ""
		Public Property CustomerId As Integer = 0
		Public Property CustomerFirstName As String = ""
		Public Property CustomerLastName As String = ""
		Public Property DateOfBirth As Date = Intuitive.DateFunctions.EmptyDate
		Public Property CustomerAddress1 As String = ""
		Public Property CustomerAddress2 As String = ""
		Public Property CustomerTownCity As String = ""
		Public Property CustomerCounty As String = ""
		Public Property CustomerPostcode As String = ""
		Public Property CustomerBookingCountryID As Integer = 0
		Public Property CustomerPhone As String = ""
		Public Property CustomerMobile As String = ""
		Public Property CustomerFax As String = ""
		Public Property CustomerEmail As String = ""
		Public Property CustomerPassportNumber As String = ""
		Public Property CustomerExternalReference As String = ""

		Public Property ContactCustomer As Boolean = False
		Public Property ContactByPost As Boolean = False
		Public Property ContactByEmail As Boolean = False
		Public Property ContactByPhone As Boolean = False
		Public Property ContactBySMS As Boolean = False
        Public Property EMarketingSignUp As Boolean = False

		<XmlIgnore>
        Public Property ValidateAddress As Boolean = True

		<XmlIgnore>
		Public Property ValidatePostcode As Boolean = True

        Public Function Validate(ValidationType As Interfaces.eValidationType) As List(Of String)
			Dim oWarnings As New List(Of String)

			If Me.CustomerTitle = "" Then oWarnings.Add(WarningMessage.CustomerTitleNotSpecified)
			If Me.CustomerFirstName = "" Then oWarnings.Add(WarningMessage.CustomerFirstNameNotSpecified)
			If Me.CustomerLastName = "" Then oWarnings.Add(WarningMessage.CustomerLastNameNotSpecified)

			'for public bookings only
			If ValidationType = Interfaces.eValidationType.Public Then
				If ValidateAddress AndAlso Me.CustomerAddress1 = String.Empty Then oWarnings.Add(WarningMessage.CustomerAddress1NotSpecified)
				If ValidateAddress AndAlso Me.CustomerTownCity = String.Empty Then oWarnings.Add(WarningMessage.CustomerTownCityNotSpecified)
			    If ValidatePostcode AndAlso Me.CustomerPostcode = String.Empty Then oWarnings.Add(WarningMessage.CustomerPostcodeNotSpecified)
				If Me.CustomerBookingCountryID = 0 Then oWarnings.Add(WarningMessage.CustomerBookingCountryIDNotSpecified)
				If Not IsEmail(Intuitive.Functions.SafeString(Me.CustomerEmail)) Then oWarnings.Add(WarningMessage.InvalidCustomerEmail)
			End If

			'field size limits
			If CustomerTitle.Length > 15 Then oWarnings.Add(WarningMessage.InvalidCustomerTitleLength)
			If CustomerFirstName.Length > 30 Then oWarnings.Add(WarningMessage.InvalidCustomerFirstNameLength)
			If CustomerLastName.Length > 30 Then oWarnings.Add(WarningMessage.InvalidCustomerLastNameLength)
            If CustomerAddress1.Length > 100 Then oWarnings.Add(WarningMessage.InvalidCustomerAddress1Length)
            If CustomerAddress2.Length > 100 Then oWarnings.Add(WarningMessage.InvalidCustomerAddress2Length)
            If CustomerTownCity.Length > 50 Then oWarnings.Add(WarningMessage.InvalidCustomerTownCityLength)
			If CustomerCounty.Length > 30 Then oWarnings.Add(WarningMessage.InvalidCustomerCountyLength)
			If CustomerPostcode.Length > 15 Then oWarnings.Add(WarningMessage.InvalidCustomerPostCodeLength)
			If CustomerPhone.Length > 30 Then oWarnings.Add(WarningMessage.InvalidCustomerPhoneLength)
			If CustomerMobile.Length > 30 Then oWarnings.Add(WarningMessage.InvalidCustomerMobileLength)
			If CustomerFax.Length > 20 Then oWarnings.Add(WarningMessage.InvalidCustomerFaxLength)
			If CustomerEmail.Length > 50 Then oWarnings.Add(WarningMessage.InvalidCustomerEmailLength)
			If CustomerPassportNumber.Length > 20 Then oWarnings.Add(WarningMessage.InvalidCustomerPassportNumberLength)
			If CustomerExternalReference.Length > 50 Then oWarnings.Add(WarningMessage.InvalidCustomerExternalReferenceLength)

			Return oWarnings

		End Function

	End Class

End Namespace
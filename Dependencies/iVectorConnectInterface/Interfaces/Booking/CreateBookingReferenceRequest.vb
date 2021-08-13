
Public Class CreateBookingReferenceRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest


    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
    Public Property CustomerDetails As New Support.LeadCustomerDetails
    Public Property ExternalReference As String

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'mandatory customer details
		If Me.CustomerDetails.CustomerTitle = "" Then aWarnings.Add("A Customer Title must be specified.")

		If Me.CustomerDetails.CustomerFirstName = "" Then aWarnings.Add("A Customer First Name must be specified.")

		If Me.CustomerDetails.CustomerLastName = "" Then aWarnings.Add("A Customer Last Name must be specified.")

		'mandatory fields for public bookings
		If ValidationType = Interfaces.eValidationType.Public Then

			If Me.CustomerDetails.CustomerAddress1 = "" Then aWarnings.Add("The Customer Address 1 must be specified.")

			If Me.CustomerDetails.CustomerTownCity = "" Then aWarnings.Add("A Customer Town/City must be specified.")

			If Me.CustomerDetails.CustomerPostcode = "" Then aWarnings.Add("A Customer Postcode must be specified.")

			If Me.CustomerDetails.CustomerBookingCountryID = 0 Then aWarnings.Add("A Customer Booking Country must be specified.")

			If Me.CustomerDetails.CustomerEmail = "" Then aWarnings.Add("A Customer Email must be specified.")

		End If

		Return aWarnings

    End Function
End Class

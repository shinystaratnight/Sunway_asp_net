Imports System.Xml.Serialization

Public Class GetOffsitePaymentRedirectRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property ReturnURL As String
	Public Property OffsitePaymentTypeID As Integer
	Public Property PreAuthorisationOnly As Boolean = False

	Public Property BookingDetails As BookingDetailsDef


#Region "validation"


	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'return URL
		If Me.ReturnURL = "" Then aWarnings.Add("A Return URL must be specified")

		'booking details?
		If Me.BookingDetails Is Nothing Then

			aWarnings.Add("Booking Details must be specified")

		Else

			'booking details validation
			aWarnings.AddRange(Me.BookingDetails.Validate(ValidationType))

		End If


		Return aWarnings

	End Function

#End Region


#Region "Helper Classes"

	Public Class BookingDetailsDef
		Public Property BookingReference As String
		Public Property TotalPayment As Decimal
		Public Property TotalPassengers As Integer
        Public Property LeadCustomer As Support.LeadCustomerDetails
        Public Property BasketStoreID As Integer = 0


		Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String)

			Dim aWarnings As New Generic.List(Of String)

			'booking reference
            If Me.BookingReference = "" AndAlso BasketStoreID = 0 Then aWarnings.Add("A Booking Reference or BasketStoreID must be specified")

			'total payment
			If Not Me.TotalPayment > 0 Then aWarnings.Add("A Total Payment must be specified")


			'total passengers
			If Not Me.TotalPassengers > 0 Then aWarnings.Add("Total Passengers must be specified")

			'must have a lead passenger
			If Me.LeadCustomer Is Nothing Then
				aWarnings.Add("A Lead Customer must be specified")
			Else
				aWarnings.AddRange(Me.LeadCustomer.Validate(ValidationType))
			End If


			Return aWarnings
		End Function
	End Class

#End Region

End Class

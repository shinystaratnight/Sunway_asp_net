Imports Intuitive.DateFunctions

Public Class ApplyPromoCodeRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

    Public Property PromoCode As String
    Public Property UseDate As Date
    Public Property Duration As Integer
    Public Property GeographyLevel3ID As Integer
    Public Property Components As New Generic.List(Of Component)
	Public Property TotalPrice As Decimal
    Public Property Adults As Integer
    Public Property Children As Integer

#End Region

#Region "Validation"

    Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

        'promo code
        If Me.PromoCode = "" Then aWarnings.Add("A Promo Code must be specified.")

        'use date
        If IsEmptyDate(Me.UseDate) Then aWarnings.Add("A valid Use Date must be specified.")

        'duration
        If Me.Duration < 1 Then aWarnings.Add("A Duration of at least one night must be specified.")

        'pax
        If Me.Adults + Me.Children = 0 Then aWarnings.Add("At least one Passenger must be specified.")

        'total price
        If Not Me.TotalPrice > 0 Then aWarnings.Add("A Total Price must be specified.")

        'Component prices
        If Not Components.Any(Function(o) o.Price <> 0) Then aWarnings.Add("A Price must be specified for each component.")

        'components
		For Each Component As Component In Me.Components

			If Component.BookingComponentID = 0 Then aWarnings.Add("A Booking Component ID must be specified for each component.")
			If Component.BookingComponent = "" Then aWarnings.Add("A Booking Component must be specified for each component.")
			If Component.TotalPassengers = 0 Then aWarnings.Add("The Total Passengers must be specified for each component.")
			If Component.Adults = 0 Then aWarnings.Add("The number of Adults must be specified for each component.")
			'If Component.SupplierID = 0 Then aWarnings.Add("A Supplier ID must be specified for each component.")

			'property bookings
			If Component.BookingComponent = "Property" Then

				If Component.PropertyReferenceID < 1 Then aWarnings.Add("A Property Reference ID must be specified for each property component.")
				If Component.MealBasisID < 1 Then aWarnings.Add("A Meal Basis ID must be specified for each property component.")
                'If Component.StarRating < 1 Then aWarnings.Add("A star rating must be specified for each property component.")

			End If

			'flight bookings
			If Component.BookingComponent = "Flight" Then

				If Component.DepartureAirportID < 1 Then aWarnings.Add("A Departure Airport ID must be specified for each flight component.")
				If Component.ArrivalAirportID < 1 Then aWarnings.Add("A Arrival Airport ID must be specified for each flight component.")
				If Component.FlightCarrierID < 1 Then aWarnings.Add("A Flight Carrier ID must be specified for each flight component.")
                If Component.FlightSupplierID < 1 Then aWarnings.Add("A Flight Supplier ID must be specified for each flight component")
                If String.IsNullOrWhiteSpace(Component.OutboundFlightCode) OrElse String.IsNullOrWhiteSpace(Component.ReturnFlightCode) Then aWarnings.Add("A Flight Code must be specified for each flight component")

			End If


		Next

        Return aWarnings

    End Function

#End Region

#Region "Helper Classes"

    Public Class Component

        Public Property BookingComponentID As Integer
        Public Property BookingComponent As String
        Public Property Price As Decimal
        Public Property TotalPassengers As Integer
        Public Property Adults As Integer
        Public Property PropertyReferenceID As Integer
        Public Property MealBasisID As Integer
        Public Property StarRating As Integer
        Public Property DepartureAirportID As Integer
        Public Property ArrivalAirportID As Integer
        Public Property FlightCarrierID As Integer
        Public Property FlightSupplierID As Integer
        Public Property OutboundFlightCode As String
        Public Property ReturnFlightCode As String
        Public Property ExtraTypeID As Integer
        Public Property SupplierID As Integer

    End Class

#End Region

End Class

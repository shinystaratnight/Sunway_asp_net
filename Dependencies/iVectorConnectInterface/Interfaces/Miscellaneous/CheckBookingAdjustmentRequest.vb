Imports Intuitive.DateFunctions
Imports System.Xml.Serialization
Imports Intuitive.Validators


Public Class CheckBookingAdjustmentRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property BrandID As Integer
	Public Property SalesChannelID As Integer
	Public Property SellingCountryID As Integer
	Public Property FirstDepartureDate As Date
	Public Property BookingDate As Date
	Public Property FlightCarrierID As Integer
	Public Property FlightSupplierID As Integer
	Public Property FlightCarrierType As String
	Public Property BookingTotal As Decimal
	Public Property MarginTotal As Decimal
	Public Property TotalPassengers As Integer
	Public Property CustomerCurrencyID As Integer
	Public Property SellingExchangeRate As Decimal
	Public Property GeographyLevel3ID As Integer

	<XmlArrayItem("ComponentsInformation")>
	Public Property ComponentsInformation As New Generic.List(Of ComponentInformation)

	Public Property HasProperty As Boolean
	Public Property HasFlight As Boolean

#End Region

#Region "Validation"

	Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'login details
		If Me.LoginDetails Is Nothing Then aWarnings.Add("Login details must be provided.")

		'brand
		If Me.BrandID = 0 Then aWarnings.Add("A Brand ID must be specified.")

		'sales channel
		If Me.SalesChannelID = 0 Then aWarnings.Add("A Sales Channel ID must be specified.")

		'selling country
		If Me.SellingCountryID = 0 Then aWarnings.Add("A Selling Country ID must be specified.")

		'first departure date
		If Not IsEmptyDate(Me.FirstDepartureDate) AndAlso Me.FirstDepartureDate < Now.Date Then
			aWarnings.Add("The first departure date must not be in the past.")
		ElseIf IsEmptyDate(Me.FirstDepartureDate) Then
			aWarnings.Add("A first departure date must be specified.")
		End If

		'booking date
		If Not IsEmptyDate(Me.BookingDate) AndAlso Me.BookingDate < Now.Date Then
			aWarnings.Add("The booking date must not be in the past.")
		ElseIf IsEmptyDate(Me.BookingDate) Then
			aWarnings.Add("A booking date must be specified.")
		End If

		'pax
		If Me.TotalPassengers = 0 Then aWarnings.Add("At least one passenger must be specified.")

		'customer currency id
		If Me.CustomerCurrencyID = 0 Then aWarnings.Add("A Customer Currency ID must be specified.")

		'selling exchange rate
		If Me.SellingExchangeRate = 0 Then aWarnings.Add("A selling exchange rate must be specified.")

		'components
		For Each ComponentInformation As ComponentInformation In Me.ComponentsInformation

			If ComponentInformation.BookingComponentID = 0 Then aWarnings.Add("A Booking Component ID must be specified for each component.")
			If ComponentInformation.ComponentType = "" Then aWarnings.Add("A Booking Component Type must be specified for each component.")
			If ComponentInformation.TotalPrice = 0 Then aWarnings.Add("A total price must be specified for each component.")

		Next

		Return aWarnings

	End Function

#End Region

#Region "Helper Classes"

	Public Class ComponentInformation

		Public Property BookingComponentID As Integer
		Public Property ComponentType As String
		Public Property PropertyReferenceID As Integer
		Public Property FlightID As Integer
		Public Property TotalPrice As Decimal

	End Class

#End Region

End Class


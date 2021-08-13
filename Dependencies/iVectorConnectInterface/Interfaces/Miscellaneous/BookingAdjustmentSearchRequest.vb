Imports System.Xml.Serialization
Imports Intuitive.DateFunctions

Public Class BookingAdjustmentSearchRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Private _BookingAdjustmentTypesCSV As String
	Private _DepartureGeographyLevel1ID As Integer
	Private _ArrivalGeographyLevel1ID As Integer
	Private _DepartureDate As Date
	Private _TotalPassengers As Integer

#End Region


#Region "Accessor Methods"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property BookingAdjustmentTypesCSV As String
		Get
			Return Me._BookingAdjustmentTypesCSV
		End Get
		Set(value As String)
			Me._BookingAdjustmentTypesCSV = value
		End Set
	End Property

	Public Property DepartureGeographyLevel1ID As Integer
		Get
			Return Me._DepartureGeographyLevel1ID
		End Get
		Set(value As Integer)
			Me._DepartureGeographyLevel1ID = value
		End Set
	End Property

	Public Property ArrivalGeographyLevel1ID As Integer
		Get
			Return Me._ArrivalGeographyLevel1ID
		End Get
		Set(value As Integer)
			Me._ArrivalGeographyLevel1ID = value
		End Set
	End Property

	Public Property DepartureDate As Date
		Get
			Return Me._DepartureDate
		End Get
		Set(value As Date)
			Me._DepartureDate = value
		End Set
	End Property

	Public Property TotalPassengers As Integer
		Get
			Return Me._TotalPassengers
		End Get
		Set(value As Integer)
			Me._TotalPassengers = value
		End Set
	End Property

#End Region


#Region "Validate"

	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'booking adjustment types
		If Me._BookingAdjustmentTypesCSV = "" Then aWarnings.Add("At least 1 Booking Adjustment Type must be specified.")

		'departure geography
		If Me._DepartureGeographyLevel1ID = 0 Then aWarnings.Add("A Departure GeographyLevel1ID  must be specified.")

		'arrival geography
		If Me._ArrivalGeographyLevel1ID = 0 Then aWarnings.Add("An Arrival GeographyLevel1ID  must be specified.")

		'departure date

		If Not IsEmptyDate(Me._DepartureDate) AndAlso Me._DepartureDate < Now.Date Then
			aWarnings.Add("The departure date must not be in the past.")
		ElseIf IsEmptyDate(Me._DepartureDate) Then
			aWarnings.Add("A valid departure date must be specified.")
		End If

		If Me._TotalPassengers = 0 Then aWarnings.Add("At least one passenger must be specified.")

		Return aWarnings

	End Function

#End Region

End Class

Imports Intuitive.DateFunctions
Imports System.Xml.Serialization

Public Class SearchBookingsRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property CustomerID As Integer
	Public Property TradeID As Integer
    Public Property TradeContactID As Integer
    Public Property BookingReference As String
    Public Property TradeReference As String
    Public Property EarliestBookingDate As DateTime
    Public Property LatestBookingDate As DateTime
    Public Property EarliestBookingTime As String
    Public Property LatestBookingTime As String
	Public Property EarliestDepartureDate As Date
	Public Property LatestDepartureDate As Date
	Public Property LatestReturnDate As Date
	Public Property Duration As Integer
	Public Property GeographyLevel3ID As Integer
	Public Property DepartureAirportID As Integer
	Public Property DepartureAirportGroupID As Integer
	Public Property ArrivalAirportID As Integer
	Public Property UseCustomerCurrency As Boolean
    <XmlArrayItem("BrandID")>
    Public Property BrandIDs As New Generic.List(Of Integer)

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

        'for public logins we don't want to search for every booking
        If ValidationType = Interfaces.eValidationType.Public Then

            'search criteria
            If Me.BookingReference = "" AndAlso IsEmptyDate(Me.EarliestBookingDate) AndAlso IsEmptyDate(Me.LatestBookingDate) _
             AndAlso IsEmptyDate(Me.EarliestDepartureDate) AndAlso IsEmptyDate(Me.LatestReturnDate) AndAlso Me.Duration = 0 _
             AndAlso Me.DepartureAirportID + Me.DepartureAirportGroupID = 0 AndAlso Me.ArrivalAirportID = 0 AndAlso GeographyLevel3ID = 0 Then
                aWarnings.Add("At least one of Booking Reference, Earliest Departure Date, Latest Return Date, Duration, Departure Airport ID, " & _
                  "Departure Airport Group ID, Arrival Airport Id or Geography Level 3 ID must be specified.")
            End If

        End If

        'if we have a time check the format and check we have a date
        If Not Me.EarliestBookingTime = "" Then

            If Not IsDate(Me.EarliestBookingTime) Then
                aWarnings.Add("The Earliest Booking Time is not a valid time")
            ElseIf IsEmptyDate(Me.EarliestBookingDate) Then
                aWarnings.Add("An Earliest Booking Date must be specified to search by Earliest Booking Time")
            End If

        End If

        'latest time
        If Not Me.LatestBookingTime = "" Then

            If Not IsDate(Me.LatestBookingTime) Then
                aWarnings.Add("The Latest Booking Time is not a valid time")
            ElseIf IsEmptyDate(Me.LatestBookingDate) Then
                aWarnings.Add("An Latest Booking Date must be specified to search by Latest Booking Time")
            End If

		End If

		'brand ids
		For Each i As Integer In Me.BrandIDs
			If i < 1 Then aWarnings.Add("Each BrandID must be greater than zero")
		Next

        Return aWarnings

    End Function

#End Region

End Class

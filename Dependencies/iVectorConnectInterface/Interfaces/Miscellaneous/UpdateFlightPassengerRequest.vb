Imports Intuitive.DateFunctions
Imports System.Xml.Serialization

Public Class UpdateFlightPassengerRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

    Public Property FlightPassengers As New Generic.List(Of FlightPassenger)

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        For Each oFlightPassenger As FlightPassenger In Me.FlightPassengers

            'Flight Booking Passenger
            If oFlightPassenger.FlightBookingPassengerID = 0 Then
                aWarnings.Add("A Flight Booking Passenger ID must be specified")

            Else

                'nationality id
                If oFlightPassenger.NationalityID = 0 Then aWarnings.Add(String.Format("A Nationality ID for Flight Booking Passenger {0} must be specified", oFlightPassenger.FlightBookingPassengerID))

                'passport number
                If oFlightPassenger.PassportNumber = "" Then aWarnings.Add(String.Format("A Passport Number for Flight Booking Passenger {0} must be specified", oFlightPassenger.FlightBookingPassengerID))

                'passport issue GL1
                If oFlightPassenger.PassportIssuingGeographyLevel1ID = 0 Then aWarnings.Add(String.Format("A Passport Issuing Geography Level 1 ID for Flight Booking Passenger {0} must be specified", oFlightPassenger.FlightBookingPassengerID))

                'expiry and issue dates
                If IsEmptyDate(oFlightPassenger.PassportExpiryDate) Then aWarnings.Add(String.Format("A Passport Expiry Date for Flight Booking Passenger {0} must be specified", oFlightPassenger.FlightBookingPassengerID))
                If IsEmptyDate(oFlightPassenger.PassportIssueDate) Then aWarnings.Add(String.Format("A Passport Issue Date for Flight Booking Passenger {0} must be specified", oFlightPassenger.FlightBookingPassengerID))
                If oFlightPassenger.PassportExpiryDate < oFlightPassenger.PassportIssueDate Then aWarnings.Add(String.Format("The Passport Expiry Date for Flight Booking Passenger {0} must be after the Passport Issue Date", oFlightPassenger.FlightBookingPassengerID))

                'gender
				If oFlightPassenger.Gender <> "" AndAlso oFlightPassenger.Gender.ToLower() <> "male" AndAlso oFlightPassenger.Gender.ToLower() <> "female" Then
                    aWarnings.Add(String.Format("Please specify a Gender of either Male or Female for Flight Booking Passenger {0}", oFlightPassenger.FlightBookingPassengerID))
                End If

            End If

        Next

        Return aWarnings

    End Function

#End Region

#Region "Helper Classes"
    Public Class FlightPassenger
        Public Property FlightBookingPassengerID As Integer
        Public Property MiddleName As String
        Public Property PassportNumber As String
        Public Property PassportIssueDate As Date
        Public Property PassportExpiryDate As Date
        Public Property NationalityID As Integer
        Public Property PassportIssuingGeographyLevel1ID As Integer
        Public Property DateOfBirth As Date = DateFunctions.EmptyDate()
        Public Property Gender As String
    End Class
#End Region

End Class
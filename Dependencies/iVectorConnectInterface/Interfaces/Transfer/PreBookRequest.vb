Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports Intuitive.Validators
Imports Intuitive.DateFunctions

Namespace Transfer

    <XmlType("TransferPreBookRequest")>
    <XmlRoot("TransferPreBookRequest")>
    Public Class PreBookRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property BookingToken As String
        Public Property DepartureParentType As String
        Public Property DepartureParentID As Integer
        Public Property ArrivalParentType As String
        Public Property ArrivalParentID As Integer
        Public Property DepartureDate As Date
        Public Property DepartureTime As String
        Public Property OneWay As Boolean
        Public Property ReturnDate As Date
        Public Property ReturnTime As String
        Public Property OverrideTransferTimes As Boolean


        Public Property GuestConfiguration As New Support.GuestConfiguration

		Public Property OutboundDetails As Transfer.BookRequest.OutboundJourneyDetails
		Public Property ReturnDetails As Transfer.BookRequest.ReturnJourneyDetails

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'booking token
            If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

            'departure point
            If Not InList(Me.DepartureParentType, "Airport", "Resort", "Property", "Port", "Station") Then aWarnings.Add("A Departure Parent Type of Airport, Resort, Property, Port or Station must be specified.")
            If Me.DepartureParentID < 1 Then aWarnings.Add("A Departure Parent ID must be specified.")

            'arrival point
            If Not InList(Me.ArrivalParentType, "Airport", "Resort", "Property", "Port", "Station") Then aWarnings.Add("An Arrival Parent Type of Airport, Resort, Property, Port or Station must be specified.")
            If Me.ArrivalParentID < 1 Then aWarnings.Add("An Arrival Parent ID must be specified.")

            'departure date
            If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
                aWarnings.Add("The departure date must not be in the past.")
            ElseIf IsEmptyDate(Me.DepartureDate) Then
                aWarnings.Add("A valid departure date must be specified.")
            End If

            'departure time
            If Me.DepartureTime IsNot Nothing AndAlso Not IsTime(Me.DepartureTime) Then
                aWarnings.Add("The departure time is not valid.")
            End If

            'return transfer?
            If Not Me.OneWay Then

                'return date
                If Not IsEmptyDate(Me.ReturnDate) AndAlso Me.ReturnDate < Now.Date Then
                    aWarnings.Add("The return date must not be in the past.")
                ElseIf IsEmptyDate(Me.ReturnDate) Then
                    aWarnings.Add("A valid return date must be specified.")
                End If

                If Me.ReturnDate <= Me.DepartureDate Then aWarnings.Add("The return date must be after the departure date.")

                'return time
                If Me.ReturnTime IsNot Nothing AndAlso Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time is not valid.")


            End If

            'pax
            If Me.GuestConfiguration Is Nothing Then
                aWarnings.Add("A Guest Configuration must be specified.")
            Else
                If Me.GuestConfiguration.Adults + Me.GuestConfiguration.Children + Me.GuestConfiguration.Infants = 0 Then aWarnings.Add("At least one passenger must be specified.")
                If Me.GuestConfiguration.Children > Me.GuestConfiguration.ChildAges.Count Then aWarnings.Add("A child age must be specified for each child.")

                'adult ages
                For Each AdultAge As Integer In GuestConfiguration.AdultAges
                    If AdultAge < 18 Then
                        aWarnings.Add("The adult age specified must be at least 18.")
                    End If
                Next

                'child ages
                For Each ChildAge As Integer In GuestConfiguration.ChildAges
                    If ChildAge < 2 Or ChildAge >= 18 Then
                        aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
                    End If
                Next
            End If

            Return aWarnings

        End Function

#End Region

    End Class

End Namespace

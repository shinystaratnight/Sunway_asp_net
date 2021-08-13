Imports System.Xml.Serialization
Imports Intuitive.DateFunctions

Namespace [Property]

    <XmlType("PropertyPreBookLiteRequest")>
    <XmlRoot("PropertyPreBookLiteRequest")>
    Public Class PreBookLiteRequest
        Implements Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
        Public Property BookingToken As String
        Public Property ArrivalDate As Date
        Public Property Duration As Integer
        Public Property RoomBookings As New Generic.List(Of RoomBooking)

#End Region

#Region "Validation"

        Public Function Validate(Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)


            'arrival date
            If Not IsEmptyDate(Me.ArrivalDate) AndAlso Me.ArrivalDate < Now.Date Then
                aWarnings.Add("The arrival date must not be in the past.")
            ElseIf IsEmptyDate(Me.ArrivalDate) Then
                aWarnings.Add("An arrival date must be specified.")
            End If

            'duration
            If Me.Duration < 1 Then aWarnings.Add("A duration of at least one night must be specified.")

            'room bookings
            If Me.RoomBookings.Count = 0 Then
                aWarnings.Add("At least one room request must be specified.")
            Else
                For Each oRoomBooking As RoomBooking In Me.RoomBookings

                    'booking token - no longer have to be added
                    'If oRoomBooking.RoomBookingToken = "" Then aWarnings.Add("A booking token must be specified for each room.")

                    If oRoomBooking.GuestConfiguration Is Nothing Then
                        aWarnings.Add("A Guest Configuration must be specified for each room.")
                    Else

                        If oRoomBooking.GuestConfiguration.Adults + oRoomBooking.GuestConfiguration.Children + oRoomBooking.GuestConfiguration.Infants = 0 Then
                            aWarnings.Add("At least one guest must be specified for each room")
                        End If

                        If oRoomBooking.GuestConfiguration.Children > oRoomBooking.GuestConfiguration.ChildAges.Count Then
                            aWarnings.Add("A child age must be specified for each child.")
                        End If

                        'child ages
                        For Each ChildAge As Integer In oRoomBooking.GuestConfiguration.ChildAges
                            If ChildAge < 2 Or ChildAge >= 18 Then
                                aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
                            End If
                        Next

                    End If

                Next
            End If

            Return aWarnings

        End Function

#End Region

#Region "Helper Classes"

        Public Class RoomBooking
            Public Property RoomBookingToken As String
            Public Property GuestConfiguration As Support.GuestConfiguration
        End Class

#End Region

    End Class

End Namespace

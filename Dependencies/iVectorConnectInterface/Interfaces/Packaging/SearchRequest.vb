Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports iVectorConnectInterface.Property.SearchRequest
Imports iVectorConnectInterface.Support

Namespace [Package]

    <XmlRoot("PackageSearchRequest")>
    Public Class SearchRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Packages"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property PackageReference As String

        Public Property DepartureAirportID As Integer
        Public Property DepartureAirportGroupID As Integer
        Public Property ArrivalAirportID As Integer
        Public Property ArrivalAirportGroupID As Integer
        Public Property RegionID As Integer
        Public Property ResortID As Integer
        Public Property PropertyReferenceID As Integer
        Public Property MealBasisID As Integer
        Public Property MinStarRating As Integer
        Public Property DepartureDate As Date
        Public Property Duration As Integer
        Public Property FlightClassID As Integer
        Public Property InboundFlightClassID As Integer
        Public Property ExactMatch As Boolean
		Public Property AllowMultisectorFlights As Boolean = True
		Public Property WidenSearch As Boolean = True
        Public Property MixedFlightClasses As Boolean = False


		<XmlArrayItem("RoomRequest")>
        Public Property RoomRequests As New Generic.List(Of RoomRequest)

        Public Property IPAddress As String
        Public Property URL As String

        Public Property MultiCarrier As Boolean = True

        Public Property OffsetDays As Integer?

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            If Me.LoginDetails Is Nothing Then aWarnings.Add(WarningMessage.LoginDetailsNotSpecified)

            If Me.PackageReference = "" Then

                If Me.RegionID < 1 AndAlso Me.ResortID < 1 AndAlso Me.ArrivalAirportID < 1 AndAlso ArrivalAirportGroupID = 0 AndAlso Me.PropertyReferenceID < 1 Then
                    aWarnings.Add(WarningMessage.PackageSearchDestinationNotSpecified)
                End If

                If Me.DepartureAirportID < 1 AndAlso Me.DepartureAirportGroupID < 1 Then
                    aWarnings.Add(WarningMessage.DepartureDataNotSpecified)
                End If

                If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
                    aWarnings.Add(WarningMessage.DepartureDateInThePast)
                ElseIf IsEmptyDate(Me.DepartureDate) Then
                    aWarnings.Add(WarningMessage.DepartureDateNotSpecified)
                End If

                If Me.DepartureAirportID > 0 And Me.DepartureAirportGroupID > 0 Then
                    aWarnings.Add(WarningMessage.DepartureAirportAndDepartureAirportGroupSpecified)
                End If

                If Me.ArrivalAirportID > 0 AndAlso ArrivalAirportGroupID > 0 Then
                    aWarnings.Add(WarningMessage.ArrivalAirportAndArrivalAirportGroupSpecified)
                End If

                If Me.RoomRequests Is Nothing OrElse Me.RoomRequests.Count = 0 Then
                    aWarnings.Add(WarningMessage.PropertyRoomNotSpecified)
                Else
                    Dim bValidGuest As Boolean = True
                    Dim bValidChildren As Boolean = True

                    For Each oRoomRequest As RoomRequest In Me.RoomRequests

                        If bValidGuest AndAlso oRoomRequest.GuestConfiguration.Adults + oRoomRequest.GuestConfiguration.Children + oRoomRequest.GuestConfiguration.Infants < 1 Then
                            aWarnings.Add(WarningMessage.NoGuestsSpecified)
                            bValidGuest = False
                        End If

                        If bValidChildren Then
                            If oRoomRequest.GuestConfiguration.Children > oRoomRequest.GuestConfiguration.ChildAges.Count Then
                                aWarnings.Add(WarningMessage.ChildAgeForEachChildNotSpecified)
                                bValidChildren = False
                            End If

                            For Each ChildAge As Integer In oRoomRequest.GuestConfiguration.ChildAges
                                If ChildAge < 2 Or ChildAge >= 18 Then
                                    aWarnings.Add(WarningMessage.InvalidChildAge)
                                    bValidChildren = False
                                End If
                            Next
                        End If


                    Next
                End If

                If Me.Duration < 1 Then aWarnings.Add(WarningMessage.InvalidDuration)
            End If

            Return aWarnings

        End Function

#End Region

    End Class

End Namespace

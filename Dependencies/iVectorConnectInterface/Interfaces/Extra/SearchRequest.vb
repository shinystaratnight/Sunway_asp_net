Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports System.Text.RegularExpressions
Imports System.Reflection

Namespace Extra

	<XmlRoot("ExtraSearchRequest")>
 Public Class SearchRequest
		Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

		Public Property ExtraTypes As New List(Of ExtraType)
		Public Property ExtraID As Integer

        <XmlArrayItem("ExtraID")>
        Public Property ExtraIDs As New List(Of Integer)
		Public Property ExtraGroupID As Integer
		Public Property DepartureDate As Date
		Public Property DepartureTime As String
		Public Property ReturnDate As Date
		Public Property ReturnTime As String
		Public Property Duration As Integer
		Public Property DepartureAirportID As Integer
		Public Property ArrivalAirportID As Integer
		Public Property GeographyLevel1ID As Integer
		Public Property GeographyLevel2ID As Integer
		Public Property GeographyLevel3ID As Integer
		Public Property PropertyID As Integer
        Public Property PropertyReferenceID As Integer
        Public Property BookingPrice As Decimal
        Public Property GuestConfiguration As New Support.GuestConfiguration
 	Public Property DepartureOnly As Boolean = False        
	Public Property UseAdditionalInformation As Boolean = False
        Public Property AdditionalInformation As New Information
        Public Property BookingType As String
        Public Property BookingDate As DateTime


        Public Class Information
            Public Property OneWay As Boolean = False
            Public Property FareType As String = "AllFareTypes"
            Public Property VehicleRegistration As String = ""
            Public Property VehicleType As String = "Car (Under 6ft/1.85m)"
            Public Property VehicleMakeModel As String = ""
            Public Property VehicleLength As Integer
            Public Property VehicleHeight As Integer
            Public Property RoofBoxBikes As Boolean = False
            Public Property Trailer As Boolean = False
            Public Property TrailerType As String = ""
            Public Property TrailerLength As Integer
            Public Property TrailerHeight As Integer
            Public Property Pets As Integer
            Public Property GuideDogs As Integer
            <XmlElement(ElementName:="WheelChairs")>
            Public Property WheelChair As Integer
            Public Property Direction As String = "Folkestone to Calais"
			Public Property DepartureStationID As Integer
			Public Property ArrivalStationID As Integer
        End Class

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'departure date
            If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
                aWarnings.Add("The departure date must not be in the past.")
            ElseIf IsEmptyDate(Me.DepartureDate) Then
                aWarnings.Add("A valid departure date must be specified.")
            End If

            'departure time
            If Not Me.DepartureTime Is Nothing Then
                If Not IsTime(Me.DepartureTime) Then aWarnings.Add("The departure time specified must be a valid time.")
            End If

            'return date
			If Not (Me.DepartureOnly OrElse Me.AdditionalInformation.OneWay) Then
                If Not IsEmptyDate(Me.ReturnDate) Then
                    If Me.ReturnDate < Now.Date Then aWarnings.Add("The return date must not be in the past.")
                    If Me.ReturnDate < Me.DepartureDate Then aWarnings.Add("The return date must not be before the departure date.")
                ElseIf IsEmptyDate(Me.ReturnDate) Then
                    aWarnings.Add("A valid return date must be specified.")
                End If
            End If


            'return time
            If Not Me.ReturnTime Is Nothing Then
                If Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time specified must be a valid time.")
            End If

            'pax
            If Not Me.GuestConfiguration Is Nothing Then
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

            'eurotunnel
            If UseAdditionalInformation Then
                With AdditionalInformation

                    If Not .OneWay Then
                        If Not IsEmptyDate(Me.ReturnDate) Then
                            If Me.ReturnDate < Now.Date Then aWarnings.Add("The return date must not be in the past.")
                            If Me.ReturnDate < Me.DepartureDate Then aWarnings.Add("The return date must not be before the departure date.")
                        ElseIf IsEmptyDate(Me.ReturnDate) Then
                            aWarnings.Add("A valid return date must be specified.")
                        End If

                        'return time
                        If Not Me.ReturnTime Is Nothing Then
                            If Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time specified must be a valid time.")
                        End If
                    End If

                    If Not [Enum].IsDefined(GetType(FareTypes), .FareType.Replace("-", "")) Then
                        aWarnings.Add("A valid FareType must be specified")
                    End If

                    If Not String.IsNullOrEmpty(.VehicleType) Then
                        Dim cLists As New List(Of String)(New String() {" ", "(", ")", "/", "."})
                        Dim sVehicleType As String = .VehicleType
                        For Each cList As String In cLists
                            sVehicleType = sVehicleType.Replace(cList, "")
                        Next
                        If Not [Enum].IsDefined(GetType(VehicleTypes), sVehicleType) Then
                            aWarnings.Add("Unknown vehicle type")
                        End If
                    Else
                        aWarnings.Add("Vehicle type must be defined")
                    End If

                    If .Trailer Then
                        If Not [Enum].IsDefined(GetType(TrailerTypes), .TrailerType) Then
                                aWarnings.Add("Unknown trailer type")
                            End If
                    End If

                    If .Pets > 0 AndAlso GuestConfiguration IsNot Nothing Then
                        If .Pets / GuestConfiguration.Adults > 5 Then
                            aWarnings.Add("Maximum 5 pets per adult.")
                        End If
                    End If

                    If Not String.IsNullOrEmpty(.Direction) Then
                        If Not [Enum].IsDefined(GetType(Directions), .Direction.Replace(" ", "")) Then
                            aWarnings.Add("Unknown journey direction")
                        End If
                    Else
                        aWarnings.Add("A journey direction must be specified")
                    End If

                End With
            Else

                'if not using additional info validate return dates normally
                If Not IsEmptyDate(Me.ReturnDate) Then
                    If Me.ReturnDate < Now.Date Then aWarnings.Add("The return date must not be in the past.")
                    If Me.ReturnDate < Me.DepartureDate Then aWarnings.Add("The return date must not be before the departure date.")
                ElseIf IsEmptyDate(Me.ReturnDate) Then
                    aWarnings.Add("A valid return date must be specified.")
                End If

                'return time
                If Not Me.ReturnTime Is Nothing Then
                    If Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time specified must be a valid time.")
                End If


            End If

            Return aWarnings

        End Function

        Public Enum FareTypes
            AllFareTypes
            DayTrip
            FlexiLongStay
            FlexiShortStay
            FrequentTraveller
            ShortStay
            Standard
        End Enum

        Public Enum VehicleTypes
            CarUnder6ft185m
            CarOver6ft185m
            VanUnder6ft185m
            VanOver6ft185m
            CamperVan
            Minibus
            Motorcycle
        End Enum

        Public Enum TrailerTypes
            Trailer
            Caravan
        End Enum

        Public Enum Directions
            FolkestonetoCalais
            CalaistoFolkestone
        End Enum



#End Region

#Region "Helper Classes"

		Public Class ExtraType
			Public Property ExtraTypeID As Integer
		End Class

		Public Shared Function IsTime(ByVal sTime As String) As Boolean
			Return Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]$") Or Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$")
		End Function

#End Region

	End Class

End Namespace

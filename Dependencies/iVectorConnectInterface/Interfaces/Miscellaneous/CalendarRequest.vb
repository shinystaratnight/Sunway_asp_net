Imports Intuitive.DateFunctions
Imports System.Xml.Serialization

Public Class CalendarSearchRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	'search type
	Public SearchType As String

	'departure points - for flights and packages only
	Public Property DepartureAirportID As Integer
	Public Property DepartureAirportGroupID As Integer

	'arrival points - for all types of search
	Public Property ArrivalAirportID As Integer
	Public Property ArrivalAirportGroupID As Integer
	Public Property ArrivalGeographyLevel2ID As Integer
	Public Property ArrivalGeographyLevel3ID As Integer
	Public Property ArrivalPropertyReferenceID As Integer

	'dates
	Public Property DepartureDateStart As Date
	Public Property DepartureDateEnd As Date

	'One Way
	Public Property OneWay As Boolean

	'duration
	Public Property DurationStart As Integer
	Public Property DurationEnd As Integer

	'other attributes - hotels and packages only
	Public Property MealBasisID As Integer

	'neo cache? assume yes
    Public Property NeoCache As Boolean = True

    'passengers
    Public Property Adults As Integer

    'children
    Public Property Children As Integer

    Public Property PackageGroupID As Integer

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'search type
        If Not Me.SearchType = "Property" AndAlso Not Me.SearchType = "Flight" AndAlso Not Me.SearchType = "Package" Then aWarnings.Add("A search type of Property, Flight or Package must be specified")

        'departure airport
        If (Me.SearchType = "Flight" OrElse Me.SearchType = "Package") AndAlso Me.DepartureAirportID + Me.DepartureAirportGroupID = 0 Then aWarnings.Add("A Departure Airport ID or Departure Airprot Group ID must be specified.")

        'destination
        If Me.ArrivalAirportID = 0 AndAlso Me.ArrivalAirportGroupID = 0 AndAlso Me.ArrivalGeographyLevel2ID = 0 AndAlso Me.ArrivalGeographyLevel3ID = 0 AndAlso Me.ArrivalPropertyReferenceID = 0 Then aWarnings.Add("An arrival point must be specified")

        'departure date start
        If Not IsEmptyDate(Me.DepartureDateStart) AndAlso Me.DepartureDateStart < Now.Date Then
            aWarnings.Add("The departure date start point must not be in the past.")
        ElseIf IsEmptyDate(Me.DepartureDateStart) Then
            aWarnings.Add("A valid departure date start must be specified.")
        End If

        'departure date end
        If Not IsEmptyDate(Me.DepartureDateEnd) AndAlso Me.DepartureDateEnd < Now.Date Then
            aWarnings.Add("The departure date start point must not be in the past.")
        ElseIf IsEmptyDate(Me.DepartureDateEnd) Then
            aWarnings.Add("A valid departure date start must be specified.")
        End If

        'check pax
        If Me.Adults + Me.Children = 0 Then aWarnings.Add("Adults or children must be specified")

        'check PackageGroupID exists if neocache search for properties or packages
        If Me.NeoCache = True AndAlso (Me.SearchType = "Property" Or Me.SearchType = "Package") AndAlso Me.PackageGroupID = 0 Then
            aWarnings.Add("A PackageGroupID must be specified for NeoCache searches of type Property or Package")
        End If

        Return aWarnings

    End Function

#End Region

End Class

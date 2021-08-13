Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Text.RegularExpressions

Namespace CarHire

    <XmlRoot("CarHireSearchRequest")>
    Public Class SearchRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest, Interfaces.ISearchRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property PickUpDate As Date Implements Interfaces.ISearchRequest.ArrivalDate
        Public Property PickUpTime As String
        Public Property PickUpDepotID As Integer
        Public Property DropOffDate As Date
        Public Property DropOffTime As String
        Public Property DropOffDepotID As Integer

        Public Property LeadDriverBookingCountryID As Integer

        <XmlArrayItem("DriverAge")>
        Public Property DriverAges As New Generic.List(Of Integer)
        Public Property TotalPassengers As Integer
        Public Property CustomerIP As String
        Public Property PackageOnly As Boolean
        Public Property BookingDate As DateTime

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'pick up date
            If Not IsEmptyDate(Me.PickUpDate) AndAlso Me.PickUpDate < Now.Date Then
                aWarnings.Add("The pick up date must not be in the past")
            ElseIf IsEmptyDate(Me.PickUpDate) Then
                aWarnings.Add("A pick up date must be specified")
            End If

            'pick up time
            If Not Me.PickUpTime Is Nothing Then
                If Not IsTime(Me.PickUpTime) Then aWarnings.Add("The pick up time specified must be a valid time")
            End If

            'pick up depot
            If Not Me.PickUpDepotID > 0 Then aWarnings.Add("A pick up depot ID must be specified")

            'drop off date
            If Not IsEmptyDate(Me.DropOffDate) AndAlso Me.DropOffDate < Now.Date Then
                aWarnings.Add("The drop off date must not be in the past")
            ElseIf IsEmptyDate(Me.DropOffDate) Then
                aWarnings.Add("A drop off date must be specified")
            End If

            'drop off time
            If Not Me.DropOffTime Is Nothing Then
                If Not IsTime(Me.DropOffTime) Then aWarnings.Add("The drop off time specified must be a valid time")
            End If

            'drop off depot
            If Not Me.DropOffDepotID > 0 Then aWarnings.Add("A drop off depot ID must be specified")

            'lead driver country
            If Not Me.LeadDriverBookingCountryID > 0 Then aWarnings.Add("The booking country of the lead driver must be specified")

            'driver ages
            If Me.DriverAges.Count = 0 Then
                aWarnings.Add("At least one driver age must be specified")
            Else
                For Each iAge As Integer In Me.DriverAges
                    If Not iAge > 0 Then aWarnings.Add("Invalid driver ages")
                Next
            End If
            Return aWarnings

        End Function

#End Region


    End Class

End Namespace
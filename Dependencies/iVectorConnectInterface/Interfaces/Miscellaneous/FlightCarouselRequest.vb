Imports Intuitive.DateFunctions
Imports System.Xml.Serialization

Public Class FlightCarouselRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property DepartureAirportID As Integer
    Public Property DepartureAirportGroupID As Integer
    Public Property ArrivalAirportID As Integer
    Public Property RegionID As Integer
    <XmlArrayItem("ResortID")>
    Public Property Resorts As New Generic.List(Of Integer)
	Public Property DepartureDate As Date
	Public Property OneWay As Boolean
	Public Property Duration As Integer
	Public Property DaysEitherSide As Integer = -1

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'departure airport
        If Me.DepartureAirportID + Me.DepartureAirportGroupID = 0 Then aWarnings.Add("A Departure Airport ID or Departure Airprot Group ID must be specified.")

        'destination
        If Me.ArrivalAirportID = 0 AndAlso Me.RegionID = 0 AndAlso Me.Resorts.Count = 0 Then aWarnings.Add("An Arrival Airport ID, Region ID, or Resort ID must be specified.")

        'departure date
        If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
            aWarnings.Add("The departure date must not be in the past.")
        ElseIf IsEmptyDate(Me.DepartureDate) Then
            aWarnings.Add("A valid departure date must be specified.")
        End If

        'return flights?
        If Not Me.OneWay Then
            If Me.Duration < 1 Then aWarnings.Add("A duration of at least one night must be specified for return flights.")
        End If

        'days either side
        If Me.DaysEitherSide < 0 Then aWarnings.Add("Days Either Side must be speicifed.")

        Return aWarnings

    End Function

#End Region

#Region "Support"

    Public Function ResortXML() As String

        Dim sbXML As New System.Text.StringBuilder
        If Me.Resorts.Count > 0 Then
            sbXML.Append("<ResortIDs>")
            For Each iResortID As Integer In Me.Resorts
                sbXML.AppendFormat("<ResortID>{0}</ResortID>", iResortID)
            Next
            sbXML.Append("</ResortIDs>")

        End If

        Return sbXML.ToString

    End Function

#End Region

End Class

Imports System.Globalization
Imports System.Xml.Serialization

Public Class QuoteFlightSector

    Public Property Direction As String

    Public Property SequenceNumber As Integer

    Public Property DepartureAirportID As Integer

    Public Property DepartureAirport As String

    Public Property ArrivalAirportID As Integer

    Public Property ArrivalAirport As String

    Public Property FlightCode As String

    Public Property FlightCarrierID As Integer

    Public Property FlightCarrier As String

    <XmlIgnore>
    Public Property DepartureDate As Date
        Get
            Return Date.ParseExact(XmlDepartureDate, "yyyy-MM-dd", CultureInfo.CurrentCulture)
        End Get
        Set
            XmlDepartureDate = Value.ToString("yyyy-MM-dd")
        End Set
    End Property

    <XmlElement("DepartureDate")>
    Public Property XmlDepartureDate As String

    Public Property DepartureTime As String


    <XmlIgnore>
    Public Property ArrivalDate As Date
        Get
            Return Date.ParseExact(XmlArrivalDate, "yyyy-MM-dd", CultureInfo.CurrentCulture)
        End Get
        Set
            XmlArrivalDate = Value.ToString("yyyy-MM-dd")
        End Set
    End Property

    <XmlElement("ArrivalDate")>
    Public Property XmlArrivalDate As String

    Public Property ArrivalTime As String

    Public Property FlightClassID As Integer

    Public Property FlightClass As String

End Class
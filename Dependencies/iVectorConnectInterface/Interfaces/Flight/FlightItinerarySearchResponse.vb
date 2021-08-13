Imports System.Xml.Serialization

Public Class FlightItinerarySearchResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
    Public Property Flights As New Generic.List(Of iVectorConnectInterface.Flight.SearchResponse.Flight)

#End Region


End Class


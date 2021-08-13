Imports System.Xml.Serialization

Namespace Transfer

    <XmlType("TransferInformationResponse")>
    <XmlRoot("TransferInformationResponse")>
    Public Class InformationResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

        Public Property VehicleDescription As String
        Public Property VehicleDetails As String
        Public Property BaggageDescription As String

    End Class

End Namespace

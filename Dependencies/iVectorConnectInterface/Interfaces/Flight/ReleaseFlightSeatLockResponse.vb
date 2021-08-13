Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces
Namespace Flight

    <XmlType("ReleaseFlightSeatLockResponse")>
    <XmlRoot("ReleaseFlightSeatLockResponse")>
    Public Class ReleaseFlightSeatLockResponse
        Implements Interfaces.iVectorConnectResponse

        Public Property ReturnStatus As ReturnStatus Implements iVectorConnectResponse.ReturnStatus

        Public Sub SetupReturnStatus(success As Boolean)
            ReturnStatus = New ReturnStatus()
            If Not success Then ReturnStatus.AddWarning("Seat lock could not be removed.")
        End Sub
    End Class
End Namespace
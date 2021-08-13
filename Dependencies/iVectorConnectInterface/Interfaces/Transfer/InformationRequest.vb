Imports Intuitive.Functions
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Xml.Serialization

Namespace Transfer

    <XmlType("TransferInformationRequest")>
    <XmlRoot("TransferInformationRequest")>
    Public Class InformationRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property BookingToken As String

#End Region

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'departure time
            If Me.BookingToken = "" Then
                aWarnings.Add("A booking token must be provided.")
            End If

            Return aWarnings

        End Function

    End Class

End Namespace

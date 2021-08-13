Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports Intuitive.Validators
Imports Intuitive.DateFunctions

Namespace CarHire

    <XmlType("CarHirePreBookRequest")>
    <XmlRoot("CarHirePreBookRequest")>
    Public Class PreBookRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property BookingToken As String
        Public Property CarHireExtras As New Generic.List(Of CarHire.PreBookRequest.CarHireExtra)
        Public Property Insurance As Boolean
        Public Property PickUpTime As String
        Public Property DropOffTime As String
        Public Property PickUpDate As Date
        Public Property DropOffDate As Date

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'booking token
            If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

            'extras
            For Each oExtra As CarHire.PreBookRequest.CarHireExtra In Me.CarHireExtras
                If oExtra.ExtraToken = "" Then aWarnings.Add("Extra tokens must not be blank")
                If oExtra.Quantity = 0 Then
                    aWarnings.Add("The extra quantity cannot be zero")
                End If
            Next

            Return aWarnings

        End Function

#End Region

#Region "helper classes"

        Public Class CarHireExtra
            Public Property ExtraToken As String
            Public Property Quantity As Integer
        End Class

#End Region

    End Class

End Namespace

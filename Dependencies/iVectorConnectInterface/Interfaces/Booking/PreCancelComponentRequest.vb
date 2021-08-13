Imports Intuitive.Functions
Imports Intuitive.Validators
Imports System.Xml.Serialization

Public Class PreCancelComponentRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
    Public Property BookingReference As String

    Public Property BookingComponents As New Generic.List(Of PreCancelComponentRequest.BookingComponent)

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'booking reference
        If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be specified.")

        For Each oComponent As PreCancelComponentRequest.BookingComponent In Me.BookingComponents

            If SafeEnum(Of eComponentType)(oComponent.ComponentType).ToString = "NoComponent" Then
                aWarnings.Add("Invalid component type entered.")
            Else
                If Not oComponent.ComponentBookingID > 0 Then aWarnings.Add("A component booking id must be specified for each component.")
            End If

        Next

        Return aWarnings

    End Function

#End Region

#Region "helper classes"

    Public Class BookingComponent
        Public Property ComponentBookingID As Integer
        Public Property ComponentType As String
    End Class

    Public Enum eComponentType
        NoComponent
        [Property]
        Flight
        Transfer
        Extra
    End Enum

#End Region

End Class

Imports Intuitive.Validators
Imports System.Xml.Serialization
Imports Intuitive.Functions

Public Class CancelComponentRequest
    Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest


#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

    Public Property BookingReference As String
    Public Property BookingComponents As New Generic.List(Of CancelComponentRequest.BookingComponent)
    Public Property Payment As Support.PaymentDetails
    Public Property CancellationReason As String

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

        'booking reference
        If Me.BookingReference = "" Then aWarnings.Add("A booking reference must be specified.")

        If BookingComponents.Count = 0 Then
            aWarnings.Add("At least one component must me specified")
        End If


        'components
        For Each oComponent As CancelComponentRequest.BookingComponent In Me.BookingComponents

            If SafeEnum(Of eComponentType)(oComponent.ComponentType).ToString = "NoComponent" Then

                aWarnings.Add("Invalid component type entered.")

            Else
                If oComponent.ComponentBookingID = 0 Then aWarnings.Add("A component booking ID must be entered for each component.")

                'cancellation cost and token
                If oComponent.CancellationCost < 0 Then aWarnings.Add("A cancellation cost must be specified for each component.")
                If oComponent.CancellationToken = "" Then aWarnings.Add("A cancellation token must be specified for each component.")

            End If

        Next

        'payment 
        If Not Me.Payment Is Nothing Then
            aWarnings.AddRange(Me.Payment.Validate)
        End If

        Return aWarnings

    End Function

#End Region


#Region "helper classes"

    Public Class BookingComponent
        Public Property ComponentBookingID As Integer
        Public Property ComponentType As String
        Public Property CancellationCost As Decimal
        Public Property CancellationToken As String
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

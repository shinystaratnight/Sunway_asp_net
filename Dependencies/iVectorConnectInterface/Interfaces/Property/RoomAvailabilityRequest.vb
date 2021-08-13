Imports Intuitive.DateFunctions

Namespace [Property]

    Public Class RoomAvailabilityRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property PropertyReferenceID As Integer
        Public Property GeographyLevel2ID As Integer
        Public Property GeographyLevel3ID As Integer
        Public Property ArrivalDate As Date
        Public Property Duration As Integer


#Region "Validate"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'login details
            If Me.LoginDetails Is Nothing Then aWarnings.Add("Login details must be provided.")

            'destination
            If (Me.PropertyReferenceID < 1 AndAlso Me.GeographyLevel2ID < 1 AndAlso Me.GeographyLevel3ID < 1) Then
                aWarnings.Add("A PropertyReferenceID, GeographyLevel2ID or GeographyLevel3ID  must be specified.")
            End If

            'arrival date
           If Not IsEmptyDate(Me.ArrivalDate) AndAlso Me.ArrivalDate < Now.Date Then
                aWarnings.Add("The arrival date must not be in the past.")
            ElseIf IsEmptyDate(Me.ArrivalDate) Then
                aWarnings.Add("An arrival date must be specified.")
            End If

            'duration
            If Me.Duration < 1 Then aWarnings.Add("A duration of at least one night must be specified.")

            Return aWarnings

        End Function

#End Region

    End Class


End Namespace
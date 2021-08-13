Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports Intuitive.DateFunctions
Imports Intuitive.Validators

Namespace Transfer

	<XmlRoot("TransferSearchRequest")>
	Public Class SearchRequest
		Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

       Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

		Public Property DepartureParentType As String
		Public Property DepartureParentID As Integer
		Public Property ArrivalParentType As String
		Public Property ArrivalParentID As Integer
		Public Property DepartureDate As Date
		Public Property DepartureTime As String
		Public Property OneWay As Boolean
		Public Property ReturnDate As Date
		Public Property ReturnTime As String
        Public Property GuestConfiguration As New Support.GuestConfiguration
        Public Property PackageOnly As Boolean
        Public Property BookingDate As DateTime

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            'departure point
			If Not Me.DepartureParentType Is Nothing AndAlso Not InList(Me.DepartureParentType, "Airport", "Resort", "Property", "Port", "Station") Then
				aWarnings.Add("A Departure Parent Type of Airport, Resort, Property, Port or Station must be specified.")
			End If

            If Me.DepartureParentID < 1 Then aWarnings.Add("A Departure Parent ID must be specified.")

            'arrival point
			If Not Me.ArrivalParentType Is Nothing AndAlso Not InList(Me.ArrivalParentType, "Airport", "Resort", "Property", "Port", "Station") Then
				aWarnings.Add("An Arrival Parent Type of Airport, Resort, Property, Port or Station must be specified.")
			End If

            If Me.ArrivalParentID < 1 Then aWarnings.Add("An Arrival Parent ID must be specified.")

            'departure date
            If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
                aWarnings.Add("The departure date must not be in the past.")
            ElseIf IsEmptyDate(Me.DepartureDate) Then
                aWarnings.Add("A valid departure date must be specified.")
            End If

            'departure time
            If Me.DepartureTime Is Nothing Then
                aWarnings.Add("A departure time must be specified.")
            ElseIf Not IsTime(Me.DepartureTime) Then
                aWarnings.Add("A valid departure time must be specified.")
            End If

            'return transfer?
            If Not Me.OneWay Then

                'return date
                If Not IsEmptyDate(Me.ReturnDate) AndAlso Me.ReturnDate < Now.Date Then
                    aWarnings.Add("The return date must not be in the past.")
                ElseIf IsEmptyDate(Me.ReturnDate) Then
                    aWarnings.Add("A valid return date must be specified.")
                End If

				If Me.ReturnDate <= Me.DepartureDate Then aWarnings.Add("The return date must be after the departure date.")

                'return time
				If Not Me.ReturnTime Is Nothing AndAlso Not IsTime(Me.ReturnTime) Then
					aWarnings.Add("A valid return time must be specified.")
				End If

            End If

            'pax
            If Not Me.GuestConfiguration Is Nothing Then
                If Me.GuestConfiguration.Adults + Me.GuestConfiguration.Children + Me.GuestConfiguration.Infants = 0 Then aWarnings.Add("At least one passenger must be specified.")
                If Me.GuestConfiguration.Children > Me.GuestConfiguration.ChildAges.Count Then aWarnings.Add("A child age must be specified for each child.")

                'adult ages
				For Each AdultAge As Integer In GuestConfiguration.AdultAges
					If AdultAge < 18 Then
						aWarnings.Add("The adult age specified must be at least 18.")
					End If
				Next

                'child ages
                If Me.GuestConfiguration.Children > 0 Then
					For Each ChildAge As Integer In GuestConfiguration.ChildAges
						If ChildAge < 2 Or ChildAge >= 18 Then
							aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
						End If
					Next
                End If
            Else
                aWarnings.Add("At least one passenger must be specified.")
            End If

            Return aWarnings
        End Function

#End Region

	End Class

End Namespace

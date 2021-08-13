Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Text.RegularExpressions
Imports iVectorConnectInterface.Interfaces
Imports iVectorConnectInterface.Extra

Namespace Extra

	<XmlRoot("ExtraAvailabilityRequest")>
	Public Class AvailabilityRequest
		Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

		Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
		Public Property ExtraTypes As New Generic.List(Of SearchRequest.ExtraType)
		Public Property ExtraID As Integer
		Public Property ExtraGroupID As Integer
		Public Property DepartureDate As Date
		Public Property DepartureTime As String
		Public Property ReturnDate As Date
		Public Property ReturnTime As String
		Public Property Duration As Integer
		Public Property GuestConfiguration As New Support.GuestConfiguration
		Public Property DepartureOnly As Boolean = False

		Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

			Dim aWarnings As New Generic.List(Of String)

			'extraid
			If ExtraID < 0 Then
				aWarnings.Add("The ExtraID must be greater than zero.")
			End If

			'departure date
			If Not IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
				aWarnings.Add("The departure date must not be in the past.")
			ElseIf IsEmptyDate(Me.DepartureDate) Then
				aWarnings.Add("A valid departure date must be specified.")
			End If

			'departure time
			If Not Me.DepartureTime Is Nothing Then
				If Not IsTime(Me.DepartureTime) Then aWarnings.Add("The departure time specified must be a valid time.")
			End If

			'return date
			If Not Me.DepartureOnly Then
				If Not IsEmptyDate(Me.ReturnDate) Then
					If Me.ReturnDate < Now.Date Then aWarnings.Add("The return date must not be in the past.")
					If Me.ReturnDate < Me.DepartureDate Then aWarnings.Add("The return date must not be before the departure date.")
				ElseIf IsEmptyDate(Me.ReturnDate) Then
					aWarnings.Add("A valid return date must be specified.")
				End If
			End If


			'return time
			If Not Me.ReturnTime Is Nothing Then
				If Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time specified must be a valid time.")
			End If

			'pax
			If Not Me.GuestConfiguration Is Nothing Then
				If GuestConfiguration.Children > GuestConfiguration.ChildAges.Count Then
					aWarnings.Add("A child age must be specified for each child.")
				ElseIf GuestConfiguration.ChildAges.Count > GuestConfiguration.Children Then
					aWarnings.Add("The number of children must match the number of child ages.")
				End If

				'adult ages
				For Each AdultAge As Integer In GuestConfiguration.AdultAges
					If AdultAge < 18 Then
						aWarnings.Add("The adult age specified must be at least 18.")
					End If
				Next
				If GuestConfiguration.AdultAges.Count > GuestConfiguration.Adults Then
					aWarnings.Add("The number of adult ages must not exceed the number of adults.")
				End If

				'child ages
				For Each ChildAge As Integer In GuestConfiguration.ChildAges
					If ChildAge < 2 Or ChildAge >= 18 Then
						aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
					End If
				Next
			End If

			Return aWarnings
		End Function

		Public Class ExtraType
			Public Property ExtraTypeID As Integer
		End Class

		Public Shared Function IsTime(ByVal sTime As String) As Boolean
			Return Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]$") Or Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$")
		End Function

	End Class

End Namespace

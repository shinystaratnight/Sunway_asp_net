Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports System.Text.RegularExpressions

Namespace Extra

    <XmlType("ExtraPreBookRequest")>
    <XmlRoot("ExtraPreBookRequest")>
    Public Class PreBookRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property BookingToken As String
        Public Property ExtraID As Integer
        Public Property DepartureDate As Date
        Public Property DepartureTime As String
        Public Property ReturnDate As Date
        Public Property ReturnTime As String
        Public Property BookingPrice As Decimal

        Public Property GuestConfiguration As New Support.GuestConfiguration
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)

		'attraction tickets
		Public Property ExtraOptions As New Generic.List(Of Extra.PreBookRequest.ExtraOption)

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

			'normal or option
			If Me.ExtraOptions.Count = 0 Then

				'booking token
				If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

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
				'DH - I have removed this, as it would fail for any date not after the departure date and this date is not relevant to all extras even though it is required in the request
				'If Not IsEmptyDate(Me.ReturnDate) Then
				'    If Me.ReturnDate < Now.Date Then aWarnings.Add("The return date must not be in the past.")
				'    If Me.ReturnDate < Me.DepartureDate Then aWarnings.Add("The return date must not be before the departure date.")
				'ElseIf IsEmptyDate(Me.ReturnDate) Then
				'    aWarnings.Add("A valid return date must be specified.")
				'End If

				'return time
				If Not Me.ReturnTime Is Nothing Then
					If Not IsTime(Me.ReturnTime) Then aWarnings.Add("The return time specified must be a valid time.")
				End If

				'pax
				If Me.GuestConfiguration Is Nothing Then
					aWarnings.Add("A Guest Configuration must be specified.")
				Else
					If Me.GuestConfiguration.Adults + Me.GuestConfiguration.Children + Me.GuestConfiguration.Infants = 0 Then aWarnings.Add("At least one guest must be specified.")
					If Me.GuestConfiguration.Children > Me.GuestConfiguration.ChildAges.Count Then aWarnings.Add("A child age must be specified for each child.")

					'adult ages
					For Each AdultAge As Integer In GuestConfiguration.AdultAges
						If AdultAge < 18 Then
							aWarnings.Add("The adult age specified must be at least 18.")
						End If
					Next

					'child ages
					For Each ChildAge As Integer In GuestConfiguration.ChildAges
						If ChildAge < 2 Or ChildAge >= 18 Then
							aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
						End If
					Next
				End If

			Else

				'extra options
				For Each oExtraOption As Extra.PreBookRequest.ExtraOption In Me.ExtraOptions

					'option booking tokens
					If oExtraOption.BookingToken = "" Then aWarnings.Add("A booking token must be specified for each Extra Option.")

					'booking token
					If Me.BookingToken <> "" Then aWarnings.Add("A booking token must not be specified on the Extra when Extra Options have been specified.")

					'pax
					If oExtraOption.GuestConfiguration Is Nothing Then
						aWarnings.Add("A Guest Configuration must be specified for each Option.")
					Else
						If oExtraOption.GuestConfiguration.Adults + oExtraOption.GuestConfiguration.Children + oExtraOption.GuestConfiguration.Infants = 0 Then aWarnings.Add("At least one guest must be specified.")
						If oExtraOption.GuestConfiguration.Children > oExtraOption.GuestConfiguration.ChildAges.Count Then aWarnings.Add("A child age must be specified for each child.")

						'adult ages
						For Each AdultAge As Integer In GuestConfiguration.AdultAges
							If AdultAge < 18 Then
								aWarnings.Add("The adult age specified must be at least 18.")
							End If
						Next

						'child ages
						For Each ChildAge As Integer In GuestConfiguration.ChildAges
							If ChildAge < 2 Or ChildAge >= 18 Then
								aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
							End If
						Next
					End If

				Next

			End If


			Return aWarnings

		End Function

#End Region


#Region "helper classes"


		Public Class ExtraOption
			Public Property BookingToken As String

			Public Property GuestConfiguration As New Support.GuestConfiguration

			Public Property IndividualInformation As Generic.List(Of Extra.BookRequest.InformationItem)

		End Class


#End Region

#Region "Helper functions"

		Public Shared Function IsTime(ByVal sTime As String) As Boolean
			Return Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]$") Or Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$")
		End Function

#End Region

	End Class

End Namespace

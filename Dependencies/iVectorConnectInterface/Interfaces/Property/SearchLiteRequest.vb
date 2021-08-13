Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports iVectorConnectInterface.Support

Namespace [Property]

	<XmlRoot("PropertySearchRequest")>
	Public Class SearchLiteRequest
		Implements Interfaces.iVectorConnectRequest

		Public Property LoginDetails As LoginDetails Implements Interfaces.iVectorConnectRequest.LoginDetails

		<XmlArrayItem("PropertyReferenceID")>
		Public PropertyReferenceIDs As New List(Of Integer)

		Public Property ArrivalDate As Date
		Public Property Duration As Integer

		<XmlArrayItem("RoomRequest")>
		Public Property RoomRequests As New List(Of RoomRequest)
		Public Property MealBasisID As Integer
		Public Property MinStarRating As Integer

		Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.iVectorConnectRequest.Validate

			Dim oWarnings As New List(Of String)

			'destination
			If Not Me.PropertyReferenceIDs.Any OrElse Me.PropertyReferenceIDs.Sum = 0 Then
				oWarnings.Add(WarningMessage.PropertySearchDestinationNotSpecified)
			End If

			'arrival date
			If Not IsEmptyDate(Me.ArrivalDate) AndAlso Me.ArrivalDate < Now.Date Then
				oWarnings.Add(WarningMessage.ArrivalDateInThePast)
			ElseIf IsEmptyDate(Me.ArrivalDate) Then
				oWarnings.Add(WarningMessage.ArrivalDateNotSpecified)
			End If

			'duration
			If Me.Duration < 1 Then oWarnings.Add(WarningMessage.InvalidDuration)

			'room requests
			If Me.RoomRequests.Count = 0 Then
				oWarnings.Add(WarningMessage.PropertyRoomNotSpecified)
			Else
				'guests
				For Each RoomRequest As RoomRequest In Me.RoomRequests
					If RoomRequest.GuestConfiguration.Adults + RoomRequest.GuestConfiguration.Children + RoomRequest.GuestConfiguration.Infants = 0 Then oWarnings.Add("At least one occupant must be specified for each room.")

					'number of child ages
					If RoomRequest.GuestConfiguration.Children > RoomRequest.GuestConfiguration.ChildAges.Count Then
						oWarnings.Add(WarningMessage.ChildAgeForEachChildNotSpecified)
					End If

					'child ages
					For Each ChildAge As Integer In RoomRequest.GuestConfiguration.ChildAges
						If ChildAge < 2 Or ChildAge >= 18 Then
							oWarnings.Add(WarningMessage.InvalidChildAge)
						End If
					Next

				Next
			End If

			Return oWarnings

		End Function

		<Serializable()>
		Public Class RoomRequest
			Public Property GuestConfiguration As New GuestConfiguration
		End Class

	End Class

End Namespace

Imports System.Xml.Serialization
Imports Intuitive.Functions

Namespace Support

	Public Class GuestDetail
		Public Property GuestID As Integer
		Public Property Type As String
		Public Property Title As String
		Public Property FirstName As String
		Public Property MiddleName As String
		Public Property LastName As String
		Public Property Age As Integer
		Public Property DateOfBirth As Date
		Public Property NationalityID As Integer
		Public Property PlaceOfBirth As String
		Public Property PassportNumber As String
		Public Property Gender As String
		Public Property BookingPassengerID As Integer
		Public Property Weight As Decimal
        Public Property hlpOutboundCharterFlightInventorySeatID As Integer
        Public Property hlpReturnCharterFlightInventorySeatID As Integer
        Public Property hlpOutboundSeatSupplementCost As Decimal
        Public Property hlpReturnSeatSupplementCost As Decimal
        Public Property FrequentFlyerCarrierID As Integer
        Public Property FrequentFlyerNumber As String
        Public Property GuestAddress1 As String
		Public Property GuestAddress2 As String
		Public Property GuestTownCity As String
		Public Property GuestCounty As String
        Public Property GuestPostcode As String
        Public Property GuestBookingCountryID As Integer
        Public Property GuestPhone As String
        Public Property GuestEmail As String
        Public Property RefusedContact As Boolean

        <XmlIgnore>
        Public Property EmailRequired As Boolean

        Public Function ShouldSerializeBookingPassengerID() As Boolean
            Return BookingPassengerID <> 0
        End Function

        Public Function ShouldSerializeWeight() As Boolean
            Return Weight <> 0
        End Function

        Public Function ShouldSerializehlpOutboundCharterFlightInventorySeatID() As Boolean
            Return hlpOutboundCharterFlightInventorySeatID <> 0
        End Function

        Public Function ShouldSerializehlpReturnCharterFlightInventorySeatID() As Boolean
            Return hlpReturnCharterFlightInventorySeatID <> 0
        End Function

        Public Function ShouldSerializehlpOutboundSeatSupplementCost() As Boolean
            Return hlpOutboundSeatSupplementCost <> 0
        End Function

        Public Function ShouldSerializehlpReturnSeatSupplementCost() As Boolean
            Return hlpReturnSeatSupplementCost <> 0
        End Function

        Public Function ShouldSerializeFrequentFlyerCarrierID() As Boolean
            Return FrequentFlyerCarrierID <> 0
        End Function

        Public Sub New()

		End Sub

		Public Sub New(
			ByVal Type As String,
			ByVal Title As String,
			ByVal FirstName As String,
			ByVal LastName As String,
			ByVal Age As Integer,
			ByVal DateOfBirth As Date,
			ByVal NationalityID As Integer)

			Me.Type = Type
			Me.Title = Title
			Me.FirstName = FirstName
			Me.LastName = LastName
			Me.Age = Age
			Me.DateOfBirth = DateOfBirth
			Me.NationalityID = NationalityID

		End Sub

		Public Function Validate() As List(Of String)

			Dim oWarnings As New List(Of String)

			If Me.GuestID = 0 Then oWarnings.Add(WarningMessage.GuestIDNotSpecified)

			If Me.BookingPassengerID <> 0 Then

				If Me.BookingPassengerID <> 0 AndAlso
					(Me.Type <> String.Empty OrElse
					Me.Title <> String.Empty OrElse
					Me.FirstName <> String.Empty OrElse
					Me.LastName <> String.Empty) Then
					oWarnings.Add(WarningMessage.GuestDetailsSpecifiedWhenBookingPassengerIDSpecified)
				End If

			Else

				If Me.Type = String.Empty Then
					oWarnings.Add(WarningMessage.GuestTypeNotSpecified)
				ElseIf Not InList(Me.Type, "Adult", "Child", "Infant") Then
					oWarnings.Add(WarningMessage.GuestTypeInvalid)
				End If

				If Me.Title = String.Empty Then oWarnings.Add(WarningMessage.GuestTitleNotSpecified)
				If Me.FirstName = String.Empty Then oWarnings.Add(WarningMessage.GuestFirstNameNotSpecified)
				If Me.LastName = String.Empty Then oWarnings.Add(WarningMessage.GuestLastNameNotSpecified)

				If Me.Type = "Child" And Me.Age = 0 Then oWarnings.Add(WarningMessage.GuestAgeNotSpecifiedForChild)

			End If

            If Not ((Not String.IsNullOrWhiteSpace(Me.GuestAddress1) _
                        AndAlso Not String.IsNullOrWhiteSpace(Me.GuestTownCity) _
                        AndAlso Not String.IsNullOrWhiteSpace(Me.GuestPostcode) _
                        AndAlso Me.GuestBookingCountryID > 0 _
                        AndAlso Not ((EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestEmail)) _
                            OrElse (Not EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestPhone)))) _
                    Xor (String.IsNullOrWhiteSpace(Me.GuestAddress1) _
                        AndAlso String.IsNullOrWhiteSpace(Me.GuestTownCity) _
                        AndAlso String.IsNullOrWhiteSpace(Me.GuestPostcode) _
                        AndAlso Me.GuestBookingCountryID = 0 _
                        AndAlso ((EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestEmail)) _
                                 OrElse (Not EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestPhone))))) Then
                If EmailRequired Then
                    oWarnings.Add($"GuestID {Me.GuestID}: {WarningMessage.GuestContactDetailsNotSpecifiedWithEmail}")
                Else
                    oWarnings.Add($"GuestID {Me.GuestID}: {WarningMessage.GuestContactDetailsNotSpecifiedWithPhone}")
                End If

                If String.IsNullOrWhiteSpace(Me.GuestAddress1) Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestAddress")
                If String.IsNullOrWhiteSpace(Me.GuestTownCity) Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestTownCity")
                If String.IsNullOrWhiteSpace(Me.GuestPostcode) Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestPostcode")
                If Me.GuestBookingCountryID = 0 Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestBookingCountryID")
                If EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestEmail) Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestEmail")
                If Not EmailRequired AndAlso String.IsNullOrWhiteSpace(Me.GuestPhone) Then oWarnings.Add($"GuestID {Me.GuestID}: Invalid GuestPhone")
            End If

            Return oWarnings

		End Function

	End Class

End Namespace
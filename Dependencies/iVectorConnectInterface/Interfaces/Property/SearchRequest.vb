Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports iVectorConnectInterface.Support

Namespace [Property]

	<XmlRoot("PropertySearchRequest")>
	Public Class SearchRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest, Interfaces.ISearchRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

		Public Property SearchMode As String = "Hotel"

		Public Property AirportID As Integer
		Public Property AirportGroupID As Integer
		Public Property RegionID As Integer
		Public Property PropertyReference As String
		Public Property PropertyReferenceID As Integer
		Public Property PropertyTypeID As Integer

		<XmlArrayItem("ResortID")>
		Public Resorts As New Generic.List(Of Integer)

		<XmlArrayItem("PropertyReferenceID")>
		Public PropertyReferenceIDs As New Generic.List(Of Integer)

        Public Property ArrivalDate As Date Implements Interfaces.ISearchRequest.ArrivalDate
		Public Property Duration As Integer

		<XmlArrayItem("RoomRequest")>
		Public Property RoomRequests As New Generic.List(Of RoomRequest)
		Public Property MealBasisID As Integer
		Public Property MinStarRating As Integer

		Public Property Latitude As Decimal
		Public Property Longitude As Decimal
		Public Property Radius As Decimal

		<XmlArrayItem("ProductAttributeGroupID")>
		Public ProductAttributeGroups As New Generic.List(Of Integer)

		<XmlArrayItem("ProductAttributeID")>
		Public ProductAttributes As New Generic.List(Of Integer)

		Public Property MinimumPrice As Decimal
		Public Property MaximumPrice As Decimal

		'needed to store search results by user ip and website
		Public Property IPAddress As String
		Public Property URL As String

		'needed for converting search response format
		Public KeepSearchResults As Boolean = False

		'flight and hotel search
		Public Property FlightDetails As New FlightDetailsDef

		<XmlArrayItem("PropertySource")>
		Public Property PropertySources As New Generic.List(Of String)
		Public Property PackageOnly As Boolean
        Public Property UseRoomMapping As Boolean
        Public Property SuppressPropertyErrata As Boolean = False
        Public Property GetPropertyErrataAtPropertyLevel As Boolean = False
        Public Property ExcludeNonRefundable As Boolean
        Public Property BookingDate As DateTime

#End Region

#Region "Validation"

		Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

			Dim oWarnings As New Generic.List(Of String)

			'login details
			If Me.LoginDetails Is Nothing Then oWarnings.Add(WarningMessage.LoginDetailsNotSpecified)

			'search type
			If SearchMode Is Nothing OrElse Not Intuitive.Functions.InList(SearchMode, "Hotel", "FlightPlusHotel") Then
				oWarnings.Add(WarningMessage.InvalidSearchMode)
			End If

			'destination
			If Me.RegionID < 1 AndAlso
				Me.AirportID < 1 AndAlso
				Me.AirportGroupID < 1 AndAlso
				Me.PropertyReferenceID < 1 AndAlso
				Me.Resorts.Sum = 0 AndAlso
				Me.PropertyReferenceIDs.Sum = 0 AndAlso
				Me.Latitude = 0 AndAlso
				Me.Longitude = 0 AndAlso
				Me.Radius = 0 AndAlso
				String.IsNullOrWhiteSpace(Me.PropertyReference) Then
				oWarnings.Add(WarningMessage.PropertySearchDestinationNotSpecified)
			End If

			If Me.Latitude < -90 OrElse Me.Latitude > 90 Then
				oWarnings.Add(WarningMessage.InvalidLatitude)
			End If

			If Me.Longitude < -180 OrElse Me.Longitude > 180 Then
				oWarnings.Add(WarningMessage.InvalidLongitude)
			End If

			If (Me.Latitude <> 0 OrElse Me.Longitude <> 0) AndAlso Me.Radius <= 0 Then
				oWarnings.Add(WarningMessage.InvalidRadius)
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

			'flight details
			If Not Me.FlightDetails Is Nothing Then

				If Me.FlightDetails.FlightAndHotel Then
					If Me.FlightDetails.DepartureAirportID = 0 And Me.FlightDetails.DepartureAirportGroupID = 0 Then
						oWarnings.Add(WarningMessage.DepartureAirportNotSpecified)
					End If

					If Me.FlightDetails.DepartureAirportID > 0 And Me.FlightDetails.DepartureAirportGroupID > 0 Then
						oWarnings.Add(WarningMessage.DepartureAirportAndDepartureAirportGroupSpecified)
					End If
				ElseIf Me.FlightDetails.OpaqueRates Then
					oWarnings.Add(WarningMessage.OpaqueRatesSpecifiedForNonFlightSearch)
				End If

			End If

			Return oWarnings

		End Function

#End Region

#Region "Helper Classes"
		<Serializable()>
		Public Class RoomRequest
			Public Property GuestConfiguration As New Support.GuestConfiguration
		End Class

        Public Class FlightDetailsDef
            Public Property FlightAndHotel As Boolean
            Public Property DepartureAirportID As Integer
			Public Property DepartureAirportGroupID As Integer
			Public Property OpaqueRates As Boolean
        End Class

#End Region

    End Class

End Namespace

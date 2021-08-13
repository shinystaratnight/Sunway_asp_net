Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports Intuitive.Functions

Public Class FlightItinerarySearchRequest
	Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.iVectorConnectRequest.LoginDetails

	Public Property GuestConfiguration As New Support.GuestConfiguration

	Public Property FlightClassID As Integer

	Public Property FlightSectors As New Generic.List(Of FlightItinerarySearchRequest.FlightSector)

	Public Property RegeneratedFlightSectors As Generic.List(Of FlightItinerarySearchRequest.FlightSector)

	'needed to store search results by user ip
	Public Property IPAddress As String

	Public Property PreferredAirport As Boolean

	'needed to distinguish websites
	Public Property URL As String

	'Set this to true as default as it is an itinerary search
	Public Property ItinerarySearch As Boolean = True

#End Region

#Region "Validation"

	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.iVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'flight sectors
		If Me.FlightSectors.Count = 0 Then
			aWarnings.Add("At least one flight sector must be added.")
		ElseIf Me.FlightSectors.Count > 10 Then
			aWarnings.Add("There can only be a maximum of 10 flight legs specified")
		Else

			Dim aPositions As New ArrayList

			For Each oFlightSector As FlightItinerarySearchRequest.FlightSector In Me.FlightSectors

				'airports
				If oFlightSector.DepartureAirportGroupID <= 0 AndAlso oFlightSector.DepartureAirportID <= 0 AndAlso oFlightSector.DepartureGeoLevel2ID <= 0 AndAlso oFlightSector.DepartureGeoLevel3ID <= 0 Then
					aWarnings.Add("At least one of departure airport group id or departure airport id must be specified for each flight sectors.")
				End If
				If oFlightSector.ArrivalAirportGroupID <= 0 AndAlso oFlightSector.ArrivalAirportID <= 0 AndAlso oFlightSector.ArrivalGeoLevel2ID <= 0 AndAlso oFlightSector.ArrivalGeoLevel3ID <= 0 Then
					aWarnings.Add("At least one of arrival airport group id or arrival airport id must be specified for each flight sectors.")
				End If

				'departure dates
				If Not IsEmptyDate(oFlightSector.DepartureDate) AndAlso oFlightSector.DepartureDate < Now.Date Then
					aWarnings.Add("The departure dates must not be in the past.")
				End If

				If IsEmptyDate(oFlightSector.DepartureDate) Then
					aWarnings.Add("A valid departure date must be specified for each flight sector.")
				End If

				'arrival dates
				If IsEmptyDate(oFlightSector.ArrivalDate) Then
					aWarnings.Add("A valid arrival date must be specified for each flight sector.")
				End If

				If Not IsEmptyDate(oFlightSector.ArrivalDate) AndAlso oFlightSector.ArrivalDate < oFlightSector.DepartureDate Then
					aWarnings.Add("The arrival date must be later than the departure date")
				End If

				'positions
				If oFlightSector.Position <= 0 Then aWarnings.Add("A position must be specified for each flight sectors.")

				If aPositions.Contains(oFlightSector.Position) Then aWarnings.Add("The same flight sector connect position cannot be specified more than once.")

				aPositions.Add(oFlightSector.Position)

			Next

			'check positions are sequential
			For i As Integer = 1 To Me.FlightSectors.Count
				If Not aPositions.Contains(i) Then aWarnings.Add("The positions of the flight sectors must be sequential.")
			Next

		End If

		'pax
		If Not Me.GuestConfiguration Is Nothing Then
			If Me.GuestConfiguration.Adults + Me.GuestConfiguration.Children + Me.GuestConfiguration.Infants = 0 Then aWarnings.Add("At least one passenger must be specified.")
			If Me.GuestConfiguration.Children > Me.GuestConfiguration.ChildAges.Count Then aWarnings.Add("A child age must be specified for each child.")

			'child ages
			For Each ChildAge As Integer In GuestConfiguration.ChildAges
				If ChildAge < 2 Or ChildAge >= 18 Then
					aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
				End If
			Next
		Else
			aWarnings.Add("At least one passenger must be specified.")
		End If

		Return aWarnings

	End Function

#End Region
	Public Function ShallowCopy() As FlightItinerarySearchRequest
		Return DirectCast(Me.MemberwiseClone(), FlightItinerarySearchRequest)
	End Function

#Region "helper classes"

	Public Class FlightSector

		Public Position As Integer
		Public DepartureAirportGroupID As Integer
		Public DepartureAirportID As Integer
		Public DepartureAirportIDs As List(Of Integer)
		Public ArrivalAirportGroupID As Integer
		Public ArrivalAirportID As Integer
		Public ArrivalAirportIDs As List(Of Integer)
		Public FlightClassID As Integer
		Public DepartureGeoLevel3ID As Integer
		Public DepartureGeoLevel2ID As Integer
		Public ArrivalGeoLevel3ID As Integer
		Public ArrivalGeoLevel2ID As Integer
		Public DepartureDate As Date
		Public ArrivalDate As Date

		Public Sub SetFlightSector(ByVal preferredAirports As Boolean)
			If Me.DepartureAirportID <= 0 Then
				Me.SetAirportGroupID(FlightSectorDirection.Departure, Me.DepartureAirportGroupID)
			End If

			If Me.ArrivalAirportID <= 0 Then
				Me.SetAirportGroupID(FlightSectorDirection.Arrival, Me.ArrivalAirportGroupID)
			End If

			If Me.ArrivalGeoLevel3ID > 0 OrElse Me.ArrivalGeoLevel2ID > 0 Then
				Me.SetArrivalAirportIDs(preferredAirports)
			End If

			If Me.DepartureGeoLevel3ID > 0 OrElse Me.DepartureGeoLevel2ID > 0 Then
                Me.SetDepartureAirportIDs(preferredAirports)
            End If

			If Not me.DepartureAirportIDs.Any() AndAlso  Me.DepartureAirportID > 0
				Me.DepartureAirportIDs.Add(Me.DepartureAirportID)
			End If

	        If me.ArrivalAirportID > 0 AndAlso not me.ArrivalAirportIDs.Any()
				me.ArrivalAirportIDs.Add(me.ArrivalAirportID)
			End If

		End Sub

		Public Sub SetAirportGroupID(ByVal eFlightSectorDirection As FlightSectorDirection, ByVal iAirportGroupID As Integer)

			Dim iAirportGroup As Integer = SQL.ExecuteSingleValue($"select count(*) from AirportGroup where AirportGroupID = {SQL.GetSqlValue(iAirportGroupID, SQL.SqlValueType.Integer)}")
			If iAirportGroup > 0 Then
				Dim dt As DataTable = SQL.GetDataTable("select * from vAirportGroupAirport where AirportGroupID = {0}", iAirportGroupID)
				Dim oAirportIDs As new List(Of Integer)
				Dim bHasCityCode As boolean
				for each dr As DataRow in dt.Rows

					bHasCityCode = not String.IsNullOrEmpty(dr("CityCode").ToString)
					oAirportIDs.Add(dr("AirportID").ToString.ToSafeInt)
				Next

	            If eFlightSectorDirection = FlightSectorDirection.Departure Then
                    Me.DepartureAirportGroupID += 1000000
					If Not bHasCityCode
						me.DepartureAirportIDs = oAirportIDs
					else
						me.DepartureAirportIDs.Add(Me.DepartureAirportGroupID)
					End If
                ElseIf eFlightSectorDirection = FlightSectorDirection.Arrival Then
                    Me.ArrivalAirportGroupID += 1000000
	                If Not bHasCityCode
		                me.ArrivalAirportIDs = oAirportIDs
						else
							me.ArrivalAirportIDs.Add(Me.ArrivalAirportGroupID)
	                End If
                End If
            End If
        End Sub


        ''' <summary>
        ''' Use arrival geo 2 or geo 3 id to find the associated airports and populate the collection on the sector
        ''' However we only want to do that if the user has not specified an airport already
        ''' </summary>
        ''' <param name="preferredAirports">if set to <c>true</c> [preferred airports].</param>
        Public Sub SetArrivalAirportIDs(ByVal preferredAirports As Boolean)
            Dim oArrivalIDs As New List(Of Integer)
            If (Me.ArrivalAirportID > 0) Then
                oArrivalIDs.Add(Me.ArrivalAirportID)
            else If  Me.ArrivalAirportGroupID > 0 Then
                oArrivalIDs.Add(Me.ArrivalAirportGroupID)
            Else
                oArrivalIDs = GetGeographyAirports(Me.ArrivalGeoLevel3ID, Me.ArrivalGeoLevel2ID, preferredAirports)
            End If
            Me.ArrivalAirportIDs = oArrivalIDs
        End Sub

		''' <summary>
		''' Use arrival geo 2 or geo 3 id to find the associated airports and populate the collection on the sector
		''' However we only want to do that if the user has not specified an airport already
		''' </summary>
		''' <param name="preferredAirports">if set to <c>true</c> [preferred airports].</param>
		Public Sub SetDepartureAirportIDs(ByVal preferredAirports As Boolean)
			Dim oDepartureIDs As New List(Of Integer)
			If Me.DepartureAirportID > 0 Then
				oDepartureIDs.Add(Me.DepartureAirportID)
			ElseIf Me.DepartureAirportGroupID > 0 Then
				oDepartureIDs.Add(Me.DepartureAirportGroupID)
			Else
				oDepartureIDs = GetGeographyAirports(Me.DepartureGeoLevel3ID, Me.DepartureGeoLevel2ID, preferredAirports)
			End If
			Me.DepartureAirportIDs = oDepartureIDs
		End Sub

		Public Function ShallowCopy() As FlightItinerarySearchRequest.FlightSector
			Return DirectCast(Me.MemberwiseClone(), FlightItinerarySearchRequest.FlightSector)
		End Function

		''' <summary>
		''' A function to return the airports associated with the passed in geography with the following rules
		''' 1. If a level three ID is provided we will ignore the two ID.
		''' 2. If The only preferred airports flag is true we only want preferred airports, unless there are no matches
		''' </summary>
		''' <param name="geographylevel3ID">The geographylevel3 identifier.</param>
		''' <param name="geographylevel2ID">The geographylevel2 identifier.</param>
		''' <param name="onlyPreferredAirports">if set to <c>true</c> [only preferred airports].</param>
		''' <returns>A list of Airport IDs that match</returns>
		Private Function GetGeographyAirports(ByVal geographylevel3ID As Integer, ByVal geographylevel2ID As Integer, ByVal onlyPreferredAirports As Boolean) As List(Of Integer)

            Dim oPreferredAirports As New List(Of Integer)
            Dim oAirports As New List(Of Integer)

            Dim oAirportDataTable As DataTable = SQL.GetDataTableCache("exec iVectorConnect_getGeographyAirports")

            Dim filterString As String = "[geographylevel3ID] =" & geographylevel3ID
            If geographylevel3ID <= 0 Then
                filterString = "[geographylevel2ID] =" & geographylevel2ID
            End If

            For Each oAirport As DataRow In oAirportDataTable.Select(filterString)
                If SafeBoolean(oAirport("PreferredAirport")) Then
                    If Not oPreferredAirports.Contains(SafeInt(oAirport("AirportID"))) Then
                        oPreferredAirports.Add(SafeInt(oAirport("AirportID")))
                    End If
                Else
                    If Not oAirports.Contains(SafeInt(oAirport("AirportID"))) Then
                        oAirports.Add(SafeInt(oAirport("AirportID")))
                    End If
                End If
            Next

            If onlyPreferredAirports AndAlso oPreferredAirports.Count > 0 Then
                oAirports = oPreferredAirports
            Else
                oAirports.AddRange(oPreferredAirports)
            End If

            Return oAirports

        End Function

	End Class

#End Region

#Region "helper functions"
	Public Sub RegenerateFlightSectors()
		If Me.FlightSectors.Any() Then

			Me.RegeneratedFlightSectors = New Generic.List(Of FlightItinerarySearchRequest.FlightSector)

			For Each oFlightSector As FlightSector In Me.FlightSectors
				oFlightSector.SetFlightSector(Me.PreferredAirport)

				Dim oCopiedFlightSector As FlightItinerarySearchRequest.FlightSector = oFlightSector.ShallowCopy
				With oCopiedFlightSector
					.Position = If(Me.RegeneratedFlightSectors.Count > 0, Me.RegeneratedFlightSectors.Last().Position + 1, 1)
					.DepartureAirportID = If(oFlightSector.DepartureAirportGroupID > 1000000, oFlightSector.DepartureAirportGroupID, oFlightSector.DepartureAirportID)
					.ArrivalAirportID = If(oFlightSector.ArrivalAirportGroupID > 1000000, oFlightSector.ArrivalAirportGroupID, oFlightSector.ArrivalAirportID)
				End With

				Me.RegeneratedFlightSectors.Add(oCopiedFlightSector)
			Next
		End If
	End Sub

	Enum FlightSectorDirection
		Departure
		Arrival
	End Enum
#End Region

End Class




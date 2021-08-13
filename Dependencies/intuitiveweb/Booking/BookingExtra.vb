Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization

Public Class BookingExtra

#Region "Search"

    Public Shared Function Search(ByVal oExtraSearch As ExtraSearch) As BookingSearch.SearchReturn

        Dim oSearchReturn As New BookingSearch.SearchReturn
        Dim oiVectorConnectSearchRequest As New ivci.Extra.SearchRequest

        Try
            With oiVectorConnectSearchRequest

                'login details
                .LoginDetails = oExtraSearch.LoginDetails

                'SearchMode
                .BookingType = BookingBase.SearchDetails.SearchMode.ToString

                'departure and arrival points
                .DepartureAirportID = oExtraSearch.DepartureAirportID
                .ArrivalAirportID = oExtraSearch.ArrivalAirportID

                'Geogrpahy levels
                .GeographyLevel1ID = oExtraSearch.GeographyLevel1ID
                .GeographyLevel2ID = oExtraSearch.GeographyLevel2ID
                .GeographyLevel3ID = oExtraSearch.GeographyLevel3ID

                'Property
                .PropertyID = oExtraSearch.PropertyID

                'departure and return dates and time
                .DepartureDate = oExtraSearch.DepartureDate
                .ReturnDate = oExtraSearch.ReturnDate

                'Use times
                If oExtraSearch.TimeSpecifc AndAlso Not oExtraSearch.DepartureTime Is Nothing Then
                    .DepartureTime = oExtraSearch.DepartureTime
                End If
                If oExtraSearch.TimeSpecifc AndAlso Not oExtraSearch.ReturnTime Is Nothing Then
                    .ReturnTime = oExtraSearch.ReturnTime
                End If

                .ExtraID = oExtraSearch.ExtraID
                .ExtraGroupID = oExtraSearch.ExtraGroupID
                For Each oExtraID As Integer In oExtraSearch.ExtraTypeIDs
                    Dim oExtraType As New ivci.Extra.SearchRequest.ExtraType
                    oExtraType.ExtraTypeID = oExtraID
                    .ExtraTypes.Add(oExtraType)
                Next

                'booking price for percentage extra
                .BookingPrice = oExtraSearch.BookingPrice

                'pax
                .GuestConfiguration.Adults = oExtraSearch.Adults
                .GuestConfiguration.Children = oExtraSearch.Children
                .GuestConfiguration.Infants = oExtraSearch.Infants
                .GuestConfiguration.ChildAges = oExtraSearch.ChildAges
                .GuestConfiguration.AdultAges = oExtraSearch.AdultAges

                'Do the iVectorConnect validation procedure
                Dim oWarnings As Generic.List(Of String) = .Validate()

                If oWarnings.Count > 0 Then
                    oSearchReturn.OK = False
                    oSearchReturn.Warning.AddRange(oWarnings)
                End If

            End With

            'If everything is ok then send request
            If oSearchReturn.OK Then

                Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
                oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Extra.SearchResponse)(oiVectorConnectSearchRequest)

                Dim oSearchResponse As New ivci.Extra.SearchResponse

                If oIVCReturn.Success Then
                    oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.Extra.SearchResponse)

                    'Check if there were any results
                    If oSearchResponse.ExtraTypes.Count > 0 Then

                        'set search return
                        oSearchReturn.ExtraResults = oSearchResponse.ExtraTypes
                        oSearchReturn.ExtraCount = oSearchResponse.ExtraTypes.Count

                    End If
                Else
                    oSearchReturn.OK = False
                    oSearchReturn.Warning = oIVCReturn.Warning
                    oSearchReturn.ExtraCount = 0
                End If

                'if not ok add warnings
                If Not oSearchReturn.OK Then
                    If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
                End If

                'this would be better if we did it in bookingsearch like for flights + hotels but there is not one central method
                If BookingBase.LogAllXML Then
                    Dim oRequestInfo As New BookingSearch.RequestInfo
                    With oRequestInfo
                        .RequestTime = oIVCReturn.RequestTime
                        .RequestXML = oIVCReturn.RequestXML
                        .ResponseXML = oIVCReturn.ResponseXML
                        .NetworkLatency = oIVCReturn.NetworkLatency
                        .Type = BookingSearch.RequestInfoType.ExtraSearch
                    End With

                    WebSupportToolbar.AddUniqueLog(oRequestInfo)
                End If

            End If

        Catch ex As Exception
            oSearchReturn.OK = False
            oSearchReturn.Warning.Add(ex.ToString)
            FileFunctions.AddLogEntry("iVectorConnect/ExtraSearch", "ExtraSearchException", ex.ToString)
        End Try

        Return oSearchReturn

    End Function

	Public Shared Function AvailabilitySearch(ByVal oExtraSearch As ExtraSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.Extra.AvailabilityRequest

		Try
			With oiVectorConnectSearchRequest

				'login details
				.LoginDetails = oExtraSearch.LoginDetails
			
				'departure and return dates and time
				.DepartureDate = oExtraSearch.DepartureDate
				.ReturnDate = oExtraSearch.ReturnDate

				'Use times
				If oExtraSearch.TimeSpecifc AndAlso Not oExtraSearch.DepartureTime Is Nothing Then
					.DepartureTime = oExtraSearch.DepartureTime
				End If
				If oExtraSearch.TimeSpecifc AndAlso Not oExtraSearch.ReturnTime Is Nothing Then
					.ReturnTime = oExtraSearch.ReturnTime
				End If

				.ExtraID = oExtraSearch.ExtraID
				.ExtraGroupID = oExtraSearch.ExtraGroupID
				For Each oExtraID As Integer In oExtraSearch.ExtraTypeIDs
					Dim oExtraType As New ivci.Extra.SearchRequest.ExtraType
					oExtraType.ExtraTypeID = oExtraID
					.ExtraTypes.Add(oExtraType)
				Next

				'pax
				.GuestConfiguration.Adults = oExtraSearch.Adults
				.GuestConfiguration.Children = oExtraSearch.Children
				.GuestConfiguration.Infants = oExtraSearch.Infants
				.GuestConfiguration.ChildAges = oExtraSearch.ChildAges
				.GuestConfiguration.AdultAges = oExtraSearch.AdultAges

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then send request
			If oSearchReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Extra.AvailabilityResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.Extra.AvailabilityResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.Extra.AvailabilityResponse)

					'Check if there were any results
					If oSearchResponse.ExtraTypes.Count > 0 Then

						'set search return
						oSearchReturn.ExtraAvailabilityResults = oSearchResponse.ExtraTypes

						oSearchReturn.ExtraCount = oSearchResponse.ExtraTypes.Count

					End If
				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.ExtraCount = 0
				End If

				'if not ok add warnings
				If Not oSearchReturn.OK Then
					If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
				End If

				If BookingBase.LogAllXML Then
					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.NetworkLatency = oIVCReturn.NetworkLatency
						.Type = BookingSearch.RequestInfoType.ExtraSearch
					End With

					WebSupportToolbar.AddUniqueLog(oRequestInfo)
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/ExtraSearch", "ExtraSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

	Public Shared Function OptionSearch(ByVal oExtraSearch As ExtraSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.Extra.OptionsRequest

		Try
			With oiVectorConnectSearchRequest

				'login details
				.LoginDetails = oExtraSearch.LoginDetails

				'departure and return dates and time
				.UseDate = oExtraSearch.DepartureDate
				.EndDate = oExtraSearch.ReturnDate
				.BookingToken = oExtraSearch.BookingToken
				.ExtraID = oExtraSearch.ExtraID

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then send request
			If oSearchReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Extra.OptionsResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.Extra.OptionsResponse

				If oIVCReturn.Success Then
					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.Extra.OptionsResponse)

					'Check if there were any results
					If oSearchResponse.Options.Count > 0 Then

						'set search return

						'PROCESS THE RESPONSE
						oSearchReturn.ExtraOptionResult = oSearchResponse.Options
						oSearchReturn.ExtraCount = oSearchResponse.Options.Count


					End If
				Else
					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning
					oSearchReturn.ExtraCount = 0
				End If

				'if not ok add warnings
				If Not oSearchReturn.OK Then
					If oSearchReturn.Warning.Count > 0 Then oSearchReturn.Warning.AddRange(oSearchReturn.Warning)
				End If

				'this would be better if we did it in bookingsearch like for flights + hotels but there is not one central method
				If BookingBase.LogAllXML Then
					Dim oRequestInfo As New BookingSearch.RequestInfo
					With oRequestInfo
						.RequestTime = oIVCReturn.RequestTime
						.RequestXML = oIVCReturn.RequestXML
						.ResponseXML = oIVCReturn.ResponseXML
						.NetworkLatency = oIVCReturn.NetworkLatency
						.Type = BookingSearch.RequestInfoType.ExtraSearch
					End With

					WebSupportToolbar.AddUniqueLog(oRequestInfo)
				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/ExtraSearch", "ExtraSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function
	

    Public Class ExtraSearch

        Public LoginDetails As iVectorConnectInterface.LoginDetails

        Public ExtraID As Integer
        Public ExtraGroupID As Integer
        Public ExtraTypeIDs As New Generic.List(Of Integer)

        Public BookingPrice As Decimal
        Public DepartureAirportID As Integer
        Public ArrivalAirportID As Integer
        Public GeographyLevel1ID As Integer
        Public GeographyLevel2ID As Integer
        Public GeographyLevel3ID As Integer
        Public PropertyID As Integer
        Public DepartureDate As Date
        Public DepartureTime As String
        Public ReturnDate As Date
        Public ReturnTime As String
        Public TimeSpecifc As Boolean = True
        Public Adults As Integer
        Public Children As Integer
        Public Infants As Integer
        Public ChildAges As New Generic.List(Of Integer)
		Public AdultAges As New Generic.List(Of Integer)
		Public BookingToken As String

        Public Warning As New Generic.List(Of String)

    End Class

#End Region

#Region "Basket"

#Region "Add Extra"

    'add extra option (hash token)
    Public Shared Sub AddExtraToBasket(ByVal Identifier As String, ByVal HashToken As String, ByVal InformationItems As Generic.List(Of ivci.Extra.SearchResponse.IndividualInfoItem), Optional ByVal Quantity As Integer = 1, Optional ComponentID As Integer = 0, Optional ByVal IncludeOptionsAtBooking As Boolean = False)

        'create extra option from hash token
        Dim oExtraOption As BasketExtra.BasketExtraOption = BasketExtra.BasketExtraOption.DeHashToken(Of BasketExtra.BasketExtraOption)(HashToken)

        oExtraOption.InformationItems = InformationItems

        'add extra option
        BookingExtra.AddExtraToBasket(Identifier, oExtraOption, ComponentID, Quantity, IncludeOptionsAtBooking:=IncludeOptionsAtBooking)

    End Sub

    'add extra option (hash token)
    Public Shared Sub AddExtraToBasket(ByVal Identifier As String, ByVal HashToken As String, Optional ByVal Quantity As Integer = 1,
      Optional ComponentID As Integer = 0, Optional BasketType As BookingExtra.eBasketType = eBasketType.SearchBasket,
      Optional ByVal ClearBasket As Boolean = False, Optional ByVal GuestIDs As Generic.List(Of Integer) = Nothing,
      Optional ByVal IncludeOptionsAtBooking As Boolean = False, Optional ByVal ConfirmationReference As String = "",
      Optional ByVal CustomXML As XmlDocument = Nothing)

        'create extra option from hash token
        Dim oExtraOption As BasketExtra.BasketExtraOption = BasketExtra.BasketExtraOption.DeHashToken(Of BasketExtra.BasketExtraOption)(HashToken)

        If CustomXML IsNot Nothing Then oExtraOption.CustomXML = CustomXML

        'add extra option
        BookingExtra.AddExtraToBasket(Identifier, oExtraOption, ComponentID, Quantity, BasketType, ClearBasket, GuestIDs, IncludeOptionsAtBooking, ConfirmationReference)

    End Sub
    
	Public Shared Sub AddExtraToBasket(ByVal Identifier As String, ByVal ExtraOption As BasketExtra.BasketExtraOption,
	   Optional ComponentID As Integer = 0, Optional ByVal Quantity As Integer = 1,
	   Optional BasketType As BookingExtra.eBasketType = eBasketType.SearchBasket, Optional ByVal ClearBasket As Boolean = False,
	   Optional ByVal GuestIDs As Generic.List(Of Integer) = Nothing, Optional ByVal IncludeOptionsAtBooking As Boolean = False,
	   Optional ByVal ConfirmationReference As String = "")

        ClearExtras(ClearBasket, BasketType)

		'Reset prebooked flag
		If BasketType = eBasketType.Basket Then
			BookingBase.Basket.PreBooked = False
		Else
			BookingBase.SearchBasket.PreBooked = False
		End If

		Dim oBasketExtra As BasketExtra

		If ComponentID = 0 Then

			'If we are using the options at booking, then we want multiple options on a single extra for more quantity.
			Dim iOptionsQuantity As Integer = Intuitive.ToSafeInt(IIf(IncludeOptionsAtBooking, Quantity, 1))

			'create basket extra
			oBasketExtra = New BasketExtra
			Dim oVisitor As New Basket.Visitors.MaxBasketComponentIDVisitor
			With oBasketExtra
				.ExtraTypeID = ExtraOption.ExtraTypeID
				.IncludeOptionsAtBooking = IncludeOptionsAtBooking
				.BasketExtraOptions = New Generic.List(Of BasketExtra.BasketExtraOption)
				.ComponentID = Math.Max(BookingBase.SearchBasket.Accept(oVisitor), BookingBase.Basket.Accept(oVisitor)) + 1
				.ConfirmationReference = ConfirmationReference
			End With

			'set content xml

			If ExtraOption.ExtraID > 1000000 Then
				Dim oExtra As TwoStepExtraResultsHandler.Extra = BookingBase.SearchDetails.TwoStepExtraResults.GetSingleExtra(Identifier, ExtraOption.BookingToken)
				ExtraOption.ContentXML = Serializer.Serialize(oExtra, True)
			Else
				Dim oExtra As ExtraResultHandler.Extra = BookingBase.SearchDetails.ExtraResults.GetSingleExtra(Identifier, ExtraOption.BookingToken)
				ExtraOption.ContentXML = Serializer.Serialize(oExtra, True)

				'add locations
				oBasketExtra.ExtraLocations = oExtra.ExtraLocations
			End If

			For i As Integer = 1 To iOptionsQuantity
				oBasketExtra.BasketExtraOptions.Add(ExtraOption)
			Next

			If GuestIDs IsNot Nothing Then oBasketExtra.GuestIDs = GuestIDs

			'If we are not using the options at booking, then we want multiple extras, not multiple options
			Quantity = Intuitive.ToSafeInt(IIf(Not IncludeOptionsAtBooking, Quantity, 1))

			Dim oBasket As BookingBasket
			If BasketType = eBasketType.Basket Then
				oBasket = BookingBase.Basket
			Else
				oBasket = BookingBase.SearchBasket
			End If

			For i As Integer = 1 To Quantity
				Dim oAddtoBasketExtra As New BasketExtra
				oAddtoBasketExtra = BookingExtra.CloneExtra(oBasketExtra)
				oBasket.BasketExtras.Add(oAddtoBasketExtra)
			Next

		Else
			'add to existing extra
			oBasketExtra = BookingBase.SearchBasket.BasketExtras.Where(Function(o) o.ComponentID = ComponentID).FirstOrDefault
			If oBasketExtra Is Nothing Then Throw New Exception("No Extra Component matching ComponentID " & ComponentID.ToString & " in search basket")
			For i As Integer = 1 To Quantity

				'set content xml
				Dim oExtra As ExtraResultHandler.Extra = BookingBase.SearchDetails.ExtraResults.GetSingleExtra(Identifier, ExtraOption.BookingToken)

				ExtraOption.ContentXML = Serializer.Serialize(oExtra, True)

				oBasketExtra.BasketExtraOptions.Add(ExtraOption)
			Next

		End If

		'Check promo codes still valid
		BookingBase.Basket.RefreshPromoCode()

	End Sub

    Private Shared Sub ClearExtras(ClearBasket As Boolean, BasketType As eBasketType)
        If ClearBasket Then
            If BasketType = eBasketType.Basket Then
                BookingBase.Basket.BasketExtras.Clear()
            Else
                'If we're doing a multi-center booking, we want to keep the extras from previous centers
                Dim extrasToKeep As New List(Of BasketExtra)
                If BookingBase.SearchDetails.ItineraryDetails.CurrentCenter > 0 Then
                    For Each basketExtra As BasketExtra In BookingBase.SearchBasket.BasketExtras
                        If basketExtra.MultiCenterId < BookingBase.SearchDetails.ItineraryDetails.CurrentCenter Then
                            extrasToKeep.Add(basketExtra)
                        End If
                    Next
                End If
                BookingBase.SearchBasket.BasketExtras.Clear()
                BookingBase.SearchBasket.BasketExtras.AddRange(extrasToKeep)
            End If
        End If
    End Sub

    Public Shared Function CloneExtra(ByVal Extra As BasketExtra) As BasketExtra

        Dim oReturn As New BasketExtra

        Dim oExtraXML As XmlDocument = Serializer.Serialize(Extra, True)
        oReturn = CType(Serializer.DeSerialize(Extra.GetType, oExtraXML.InnerXml), BasketExtra)

        Return oReturn

    End Function

#End Region

#Region "Remove Extra"

    'remove extra option (hash token)
    Public Shared Sub RemoveExtraFromBasket(ByVal HashToken As String, Optional ByVal BasketType As BookingExtra.eBasketType = eBasketType.SearchBasket)

        'create extra option from hash token
        Dim oExtraOption As BasketExtra.BasketExtraOption = BasketExtra.BasketExtraOption.DeHashToken(Of BasketExtra.BasketExtraOption)(HashToken)

        Dim oBasketExtras As New List(Of BasketExtra)
        oBasketExtras = Functions.IIf(BasketType = eBasketType.Basket, BookingBase.Basket.BasketExtras, BookingBase.SearchBasket.BasketExtras)

        Dim oExtrasToRemove As New List(Of BasketExtra)

        For Each oExtra As BasketExtra In oBasketExtras

            Dim oExtraOptionsToRemove As New List(Of BasketExtra.BasketExtraOption)

            'Find the extra options we want to remove
            For Each oBasketOption As BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions
                If oBasketOption.HashToken = HashToken Then
                    oExtraOptionsToRemove.Add(oBasketOption)
                End If
            Next

            'Remove them
            For Each oBasketOption As BasketExtra.BasketExtraOption In oExtraOptionsToRemove
                oExtra.BasketExtraOptions.Remove(oBasketOption)
            Next

            'Check  if the  extra has any options left, if not we want to add it to our collection to remvoe
            If oExtra.BasketExtraOptions.Count = 0 Then
                oExtrasToRemove.Add(oExtra)
            End If
        Next

        'Loop through the extras we want to remove and remove them
        For Each oExtra As BasketExtra In oExtrasToRemove
            If BasketType = eBasketType.Basket Then
                BookingBase.Basket.BasketExtras.Remove(oExtra)
            Else
                BookingBase.SearchBasket.BasketExtras.Remove(oExtra)
            End If
        Next

        'Check promo codes still valid
        BookingBase.Basket.RefreshPromoCode()


    End Sub

    'remove extras from basket (remove all extras with a ExtraType)
    Public Shared Sub RemoveExtrasFromBasket(ByVal ExtraTypeID As Integer)

        For Each oExtra As BasketExtra In BookingBase.SearchBasket.BasketExtras

            For Each oBasketOption As BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions

                If oBasketOption.ExtraTypeID = ExtraTypeID Then
                    oExtra.BasketExtraOptions.Remove(oBasketOption)

                    If oExtra.BasketExtraOptions.Count = 0 Then
                        BookingBase.SearchBasket.BasketExtras.Remove(oExtra)
                    End If

                    'Check promo codes still valid
                    BookingBase.Basket.RefreshPromoCode()

                    Return

                End If

            Next
        Next

    End Sub

    'remove all extras from basket by ExtraID
    Public Shared Sub RemoveExtrasFromBasketByExtraID(ByVal ExtraID As Integer, Optional ByVal BasketType As BookingExtra.eBasketType = eBasketType.SearchBasket)

        Dim oBasketExtras As New List(Of BasketExtra)
        oBasketExtras = Functions.IIf(BasketType = eBasketType.Basket, BookingBase.Basket.BasketExtras, BookingBase.SearchBasket.BasketExtras)

        Dim oExtrasToRemove As New List(Of BasketExtra)

        For Each oExtra As BasketExtra In oBasketExtras

            For Each oBasketOption As BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions

                If oBasketOption.ExtraID = ExtraID Then
                    oExtrasToRemove.Add(oExtra)
                End If

            Next
        Next

		For Each oExtraToRemove As BasketExtra In oExtrasToRemove
			oBasketExtras.Remove(oExtraToRemove)
		Next

        'Check promo codes still valid
        BookingBase.Basket.RefreshPromoCode()

    End Sub

    Public Shared Sub RemoveExtraFromBookingBasket(ByVal HashToken As String)

        'create extra option from hash token
        Dim oExtraOption As BasketExtra.BasketExtraOption = BasketExtra.BasketExtraOption.DeHashToken(Of BasketExtra.BasketExtraOption)(HashToken)

        For Each oExtra As BasketExtra In BookingBase.Basket.BasketExtras

            For Each oBasketOption As BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions

                If oBasketOption.HashToken = HashToken Then
                    oExtra.BasketExtraOptions.Remove(oBasketOption)

                    If oExtra.BasketExtraOptions.Count = 0 Then
                        BookingBase.Basket.BasketExtras.Remove(oExtra)
                    End If

                    'Check promo codes still valid
                    BookingBase.Basket.RefreshPromoCode()

                    Return

                End If

            Next
        Next

    End Sub

#End Region

#Region "Update Extra"

    'remove extra option (hash token)
    Public Shared Sub UpdateExtraInBasket(ByVal Identifier As String, ByVal HashToken As String)

        'create extra option from hash token
        Dim oExtraOption As BasketExtra.BasketExtraOption = BasketExtra.BasketExtraOption.DeHashToken(Of BasketExtra.BasketExtraOption)(HashToken)

        'Remove the extra in the basket with the same extra type
        For Each oExtra As BasketExtra In BookingBase.SearchBasket.BasketExtras
            If oExtra.BasketExtraOptions(0).ExtraType = oExtraOption.ExtraType Then
                BookingBase.SearchBasket.BasketExtras.Remove(oExtra)
                Exit For
            End If
        Next

        'add extra option
        BookingExtra.AddExtraToBasket(Identifier, oExtraOption)

    End Sub

#End Region

#Region "Support Classes - BasketExtra"

    Public Class BasketExtra
        Public ComponentID As Integer
        Public ExtraTypeID As Integer
        Public AirportSpecific As Boolean ' used for display only eg airport lounges will want the aiport name, ski passes would not
        Public BasketExtraOptions As Generic.List(Of BasketExtraOption)
        Public GuestIDs As New Generic.List(Of Integer)
        Public IncludeOptionsAtBooking As Boolean
        Public ConfirmationReference As String
        Public ExtraLocations As ivci.Extra.ExtraLocations
		Public BookingQuestions As List(Of ivci.BookingQuestion)
		Public BookingAnswers As List(Of ivci.BookingAnswer)
        Public MultiCenterId As Integer = 0

        Public Property Markup As Decimal
            Get
                Dim nTotalMarkup As Decimal = 0
                For Each oMarkup As BookingBase.Markup In BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Extra)
                    Select Case oMarkup.Type
                        Case BookingBase.Markup.eType.Amount
                            nTotalMarkup += oMarkup.Value
                        Case BookingBase.Markup.eType.AmountPP
                            nTotalMarkup += oMarkup.Value * (Me.BasketExtraOptions.Sum(Function(o) o.Adults) + Me.BasketExtraOptions.Sum(Function(o) o.Children))
                        Case BookingBase.Markup.eType.Percentage
                            nTotalMarkup += (oMarkup.Value * Me.BasketExtraOptions.Sum(Function(o) o.TotalPrice)) / 100
                    End Select
                Next

                Return nTotalMarkup
            End Get
            Set(value As Decimal)
                'require this to be serialised
            End Set
        End Property

        Public Class BasketExtraOption
            Inherits Utility.Hasher

            Public BookingToken As String
            Public hlpSearchBookingToken As String
            Public ExtraType As String
            Public ExtraID As Integer
            Public ExtraName As String
            Public ExtraCategory As String
            Public ExtraCategoryDescription As string
            Public Notes As String
            Public ExtraTypeID As Integer
            Public Duration As Integer
            Public ExtraLocations As New Generic.List(Of ivci.Extra.ExtraLocations)

            Public DateRequired As Boolean
            Public TimeRequired As Boolean
            Public StartDate As DateTime
            Public StartTime As String
            Public EndDate As DateTime
            Public EndTime As String

            Public MinChildAge As Integer
            Public MaxChildAge As Integer

            Public AdultPrice As Decimal
            Public ChildPrice As Decimal
            Public InfantPrice As Decimal
            Public SeniorPrice As Decimal
            Public TotalPrice As Decimal
            Public TotalCommission As Decimal

            Public Adults As Integer
            Public Children As Integer
            Public Infants As Integer
            Public SeniorAge As Integer
            Public ChildAges As New Generic.List(Of Integer)
            Public AdultAges As New Generic.List(Of Integer)
            Public InformationItems As New Generic.List(Of ivci.Extra.SearchResponse.IndividualInfoItem)
            Public ContentXML As New XmlDocument
            Public CustomXML As New XmlDocument

            Private sHashToken As String = ""
            Public SupplierID As Integer

            Public Overrides Property HashToken As String
                Get
                    If sHashToken = "" Then
                        sHashToken = "xx"
                        sHashToken = Me.GenerateHashToken()
                    End If
                    Return sHashToken
                End Get
                Set(value As String)

                End Set
            End Property

            Public Overrides Function GenerateHashToken() As String

                Dim oExtraOption As New BasketExtraOption
                With oExtraOption

                    .BookingToken = Me.BookingToken
                    .Adults = Me.Adults
                    .Children = Me.Children
                    .Infants = Me.Infants

                    .AdultAges.AddRange(Me.AdultAges)
                    .ChildAges.AddRange(Me.ChildAges)
                    .SeniorAge = Me.SeniorAge

                    .MinChildAge = Me.MinChildAge
                    .MaxChildAge = Me.MaxChildAge

                    .TotalPrice = Me.TotalPrice
                    .AdultPrice = Me.AdultPrice
                    .ChildPrice = Me.ChildPrice
                    .InfantPrice = Me.InfantPrice
                    .SeniorPrice = Me.SeniorPrice

                    .ExtraTypeID = Me.ExtraTypeID
                    .ExtraType = Me.ExtraType
                    .ExtraID = Me.ExtraID
                    .ExtraName = Me.ExtraName
                    .ExtraCategory = Me.ExtraCategory
                    .Notes = Me.Notes
                    .Duration = Me.Duration

                    .SupplierID = Me.SupplierID

                    .DateRequired = Me.DateRequired
                    .StartDate = Me.StartDate
                    .EndDate = Me.EndDate

                    .TimeRequired = Me.TimeRequired
                    If Me.TimeRequired Then
                        .StartTime = Me.StartTime
                        .EndTime = Me.EndTime
                    End If

                    .sHashToken = "xx"
                    .ContentXML = New XmlDocument
                End With

                Return Functions.Encrypt(Serializer.Serialize(oExtraOption, True).InnerXml)
            End Function

        End Class

        Public Class InformationItem
            Public Type As String
            Public Value As String
            Public ID As Integer
            Public Name As String
        End Class

#Region "PreBook"

        Public Function CreatePreBookRequest() As ivci.Extra.PreBookRequest

            Dim oPreBookRequest As New ivci.Extra.PreBookRequest

            With oPreBookRequest
                .ExtraID = Me.BasketExtraOptions(0).ExtraID

                'This is rubbish but connect validates the field and even if a date isnt needed we are made to pass one in
                .DepartureDate = Me.BasketExtraOptions(0).StartDate
                .ReturnDate = Me.BasketExtraOptions(0).EndDate

                If Me.BasketExtraOptions(0).TimeRequired Then
                    .DepartureTime = Me.BasketExtraOptions(0).StartTime
                    .ReturnTime = Me.BasketExtraOptions(0).EndTime
                End If

                If Not IncludeOptionsAtBooking Then
                    .BookingToken = Me.BasketExtraOptions(0).BookingToken
                End If

                .GuestConfiguration.Adults = Me.BasketExtraOptions.Sum(Function(o) o.Adults)
                .GuestConfiguration.Children = Me.BasketExtraOptions.Sum(Function(o) o.Children)
                'Currently not used for attractions, if another extra needs this, need to work out a way to use it
				.GuestConfiguration.AdultAges = Me.BasketExtraOptions(0).AdultAges
                .GuestConfiguration.ChildAges = Me.BasketExtraOptions(0).ChildAges
                .GuestConfiguration.Infants = Me.BasketExtraOptions.Sum(Function(o) o.Infants)

                'DY has said only include options in prebook/book request if its attractionworld/do something different, otherwise don't inconsistant I know.
                'Loop through Each Extra option we have on the basket and add it to the booking

                If Me.IncludeOptionsAtBooking Then
                    For Each oExtraOption As BasketExtra.BasketExtraOption In Me.BasketExtraOptions
                        Dim oBasketExtraOption As New Extra.PreBookRequest.ExtraOption

                        oBasketExtraOption.IndividualInformation = New Generic.List(Of Extra.BookRequest.InformationItem)

                        oBasketExtraOption.BookingToken = oExtraOption.BookingToken
                        oBasketExtraOption.GuestConfiguration = New ivci.Support.GuestConfiguration

                        oBasketExtraOption.GuestConfiguration.Adults = oExtraOption.Adults
                        oBasketExtraOption.GuestConfiguration.Children = oExtraOption.Children
                        oBasketExtraOption.GuestConfiguration.ChildAges = oExtraOption.ChildAges
                        oBasketExtraOption.GuestConfiguration.Infants = oExtraOption.Infants

                        'Convert the Search response Information Items we have on the basket into the ones we want to hand into the prebook.
                        For Each oitem As Extra.SearchResponse.IndividualInfoItem In oExtraOption.InformationItems

                            Dim oPrebookItem As New Extra.BookRequest.InformationItem
                            oPrebookItem.ID = oitem.ID
                            oPrebookItem.Type = oitem.Type
                            oPrebookItem.Value = oitem.Values(0)
                            oBasketExtraOption.IndividualInformation.Add(oPrebookItem)
                        Next

                        .ExtraOptions.Add(oBasketExtraOption)
                    Next
                End If

            End With

            Return oPreBookRequest

        End Function

#End Region

#Region "Book"

        Public Function CreateBookRequest() As ivci.Extra.BookRequest

            Dim oBookRequests As New Generic.List(Of ivci.Extra.BookRequest)

            'setup a book request for each extra
            Dim oBookRequest As New ivci.Extra.BookRequest

            Dim aUsedGuestExtraIDs As New Generic.List(Of Integer)
            Dim aUsedGuestIDs As New Generic.List(Of Integer)

            With oBookRequest

                If Not BookingBase.Basket.VehicleInfo.CarRegistration = "" Then
                    Dim oDetails As New Extra.BookRequest.CarParkDetails
                    oDetails.CarRegistration = BookingBase.Basket.VehicleInfo.CarRegistration
                    oDetails.CarMake = BookingBase.Basket.VehicleInfo.CarMake
                    oDetails.CarModel = BookingBase.Basket.VehicleInfo.CarModel
                    oDetails.CarColour = BookingBase.Basket.VehicleInfo.CarColour
                    oBookRequest.CarPark = oDetails
                End If

                .ConfirmationReference = Me.ConfirmationReference

                If Me.BasketExtraOptions(0).DateRequired Then
                    .UseDate = Me.BasketExtraOptions(0).StartDate
                End If

                If Me.BasketExtraOptions(0).TimeRequired Then
                    .UseTime = Me.BasketExtraOptions(0).StartTime
                End If

                .ExpectedTotal = Me.BasketExtraOptions.Sum(Function(o) o.TotalPrice)

				.GuestIDs = Me.GuestIDs

				.BookingAnswers = Me.BookingAnswers

                If Me.BasketExtraOptions.Count = 1 AndAlso Not Me.IncludeOptionsAtBooking Then
                    .BookingToken = Me.BasketExtraOptions(0).BookingToken
                End If

                'Add the guests for each option
                '1. add the adults
                If Me.GuestIDs.Count = 0 Then

                    For iAdult As Integer = 1 To Me.BasketExtraOptions.Sum(Function(o) o.Adults)
                        Dim bAdultFound As Boolean = False
                        'get first adult that we've not added yet
                        Dim aUnusedGuestIDs As Generic.List(Of Integer) = Me.GuestIDs.Where(Function(o) Not aUsedGuestExtraIDs.Contains(o)).ToList
                        Dim oGuest As ivci.Support.GuestDetail = BookingBase.Basket.GuestDetails.Where(Function(o) aUnusedGuestIDs.Contains(o.GuestID)).FirstOrDefault
                        If oGuest IsNot Nothing Then
                            .GuestIDs.Add(oGuest.GuestID)
                            bAdultFound = True
                            aUsedGuestExtraIDs.Add(oGuest.GuestID)
                            Continue For
                        End If

                        'if we didnt find an existing adult < need to add a new one
                        If Not bAdultFound Then
                            Dim oGuestDetail As New ivci.Support.GuestDetail
                            With oGuestDetail
                                .GuestID = BookingBase.Basket.GuestDetails.Max(Of Integer)(Function(o) o.GuestID) + 1
                                .FirstName = "TBA"
                                .LastName = "TBA"
                                .Type = "Adult"
                                .Title = "Mr"
                            End With
                            .GuestIDs.Add(oGuestDetail.GuestID)
                            aUnusedGuestIDs.Add(oGuestDetail.GuestID)
                            'add to the basket
                            BookingBase.Basket.GuestDetails.Add(oGuestDetail)
                        End If

                    Next
                End If

                'add the options
                'DY has said only include options in prebook/book request if its attractionworld/do something different, otherwise don't inconsistant I know.
                If Me.IncludeOptionsAtBooking Then
                    For Each oExtraOption As BasketExtra.BasketExtraOption In Me.BasketExtraOptions
                        Dim oBasketExtraOption As New Extra.BookRequest.ExtraOption

                        oBasketExtraOption.IndividualInformation = New Generic.List(Of Extra.BookRequest.InformationItem)

                        oBasketExtraOption.BookingToken = oExtraOption.BookingToken

                        'Convert the Search response Information Items we have on the basket into the ones we want to hand into the prebook.
                        For Each oitem As Extra.SearchResponse.IndividualInfoItem In oExtraOption.InformationItems

                            Dim oBookItem As New Extra.BookRequest.InformationItem
                            oBookItem.ID = oitem.ID
                            oBookItem.Type = oitem.Type
                            oBookItem.Value = oitem.Values(0)
                            oBasketExtraOption.IndividualInformation.Add(oBookItem)
                        Next

                        'Add the guests for each option
                        '1. add the adults
                        For iAdult As Integer = 1 To oExtraOption.Adults
                            Dim bAdultFound As Boolean = False
                            'get first adult that we've not added yet
                            Dim aUnusedGuestIDs As Generic.List(Of Integer) = Me.GuestIDs.Where(Function(o) Not aUsedGuestIDs.Contains(o)).ToList
                            Dim oGuest As ivci.Support.GuestDetail = BookingBase.Basket.GuestDetails.Where(Function(o) aUnusedGuestIDs.Contains(o.GuestID)).FirstOrDefault
                            If oGuest IsNot Nothing Then
                                oBasketExtraOption.GuestIDs.Add(oGuest.GuestID)
                                bAdultFound = True
                                aUsedGuestIDs.Add(oGuest.GuestID)
                                Continue For
                            End If

                            'if we didnt find an existing adult < need to add a new one
                            If Not bAdultFound Then
                                Dim oGuestDetail As New ivci.Support.GuestDetail
                                With oGuestDetail
                                    .GuestID = BookingBase.Basket.GuestDetails.Max(Of Integer)(Function(o) o.GuestID) + 1
                                    .FirstName = "TBA"
                                    .LastName = "TBA"
                                    .Type = "Adult"
                                    .Title = "Mr"
                                End With
                                oBasketExtraOption.GuestIDs.Add(oGuestDetail.GuestID)
                                aUnusedGuestIDs.Add(oGuestDetail.GuestID)
                            End If

                        Next

                        .ExtraOptions.Add(oBasketExtraOption)
                    Next
                End If

            End With
            Return oBookRequest

        End Function

#End Region

    End Class
#End Region

#End Region

    Public Enum eBasketType
        Basket
        SearchBasket
    End Enum

End Class
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.AsyncCache
Imports Intuitive.Functions
Imports Intuitive.Net

Public Class Lookups

	'no external code should be able to edit the lock!
	Private Property PropertyLock As New Object

	Public Property LookupFileTypes As String = "Airport,AirportGeography,AirportGroup,AirportGroupAirport,AirportGroupRouteAvailability,AirportTerminal,BookingComponent,BookingCountry,BookingCountryRegion,BookingDescription,BookingDocumentation,BookingSource,BrandGeography,CreditCardType,CardType,ContractSupplement,CarHireDepot,CarHireDepotGeographyLevel3,Currency,ExchangeRates,Extra,ExtraCategory,ExtraDuration,ExtraGroup,ExtraSubType,ExtraSupplement,ExtraType,FilterFacility,FlightCarrier,FlightClass,GeographyGrouping,GeographyDateBand,Landmark,Language,Location,MarketingCode,MealBasis,Nationality,Port,ProductAttributes,PropertyType,PropertyRoomType,RoomType,RoomView,RouteAvailability,SalesChannel,SellingExchangeRates,StarRating,Station,Supplier,ThirdParty,TrackingAffiliate,TradeGroup,Vehicle"
	Private Property IsInitialised As Boolean = False
	Public TestXML As New XmlDocument 'only used for unit testing

	Public Params As BookingBase.ParamDef

#Region "Constructors"

	'no default empty constructor - this class is not serializable

	Public Sub New(Params As BookingBase.ParamDef)
		Me.Params = Params
	End Sub

#End Region

#Region "xml"

	Public Function LookupXML(NodeName As String, Optional IgnoreEndings As Boolean = False) As XmlDocument

		'get the correct lookup value
		Dim sLookup As String = ""
		If Not IgnoreEndings Then
			If NodeName.EndsWith("ies") Then
				sLookup = NodeName.Substring(0, NodeName.Length - 3) & "y"
			Else
				sLookup = NodeName.Chop()
			End If
		Else
			sLookup = NodeName
		End If

		'we have to do this check because these lookups are plural and we need to be backwards compatible
		'bad design
		If sLookup.ToLower = "productattribute" Then sLookup = "productattributes"
		If sLookup.ToLower = "exchangerate" Then sLookup = "exchangerates"
		If sLookup.ToLower = "sellingexchangerate" Then sLookup = "sellingexchangerates"
		'this check is just unfortunate
		If sLookup.ToLower = "mealbase" Then sLookup = "mealbasis"

		'get lookup xml from async cache
		Dim oXML As New XmlDocument
		If Params.TestProject AndAlso Me.TestXML.InnerXml <> "" Then

			'this is rubbish, we should be loosely coupled and be able to pass this in somewhere but all of ours methods are static
			oXML = TestXML
		Else
			oXML = Me.GetAsyncLookup(sLookup)
		End If

		'return
		Return oXML

	End Function

	Private Function GetAsyncLookup(Lookup As String) As XmlDocument

		Dim iDisplayLanguage As Integer = 0
		If HttpContext.Current IsNot Nothing Then
			iDisplayLanguage = BookingBase.DisplayLanguageID
		End If

		Dim sKey As String = Controller(Of XmlDocument).GenerateKey("lookupxml_" & Lookup.ToLower & iDisplayLanguage)

		'get lookup xml from async cache
		Dim oXML As XmlDocument = Controller(Of XmlDocument).GetCache(sKey, 600,
		  Function()

			  Dim oLookupXML As New XmlDocument

              Try
                  Dim sURL As String = "{0}lookups/lookups.ashx?files={1}&login={2}&password={3}&languageid={4}"
                  sURL = String.Format(sURL, Params.ServiceURL, Lookup, Params.iVectorConnectLogin,
                   Params.iVectorConnectPassword, iDisplayLanguage)

                  oLookupXML = Utility.URLToXML(sURL, 0, True)

              Catch ex As Exception
                  Intuitive.FileFunctions.AddLogEntry("Lookups", "Exception", ex.ToString)
              End Try

              Return oLookupXML

          End Function, "^(?!<Lookups).*")

        Return oXML

    End Function

    Private Sub Initialise()

        Dim aLookupFiles() As String = Me.LookupFileTypes.Split(","c)

        If aLookupFiles.Count > 0 Then
            For Each sFile As String In aLookupFiles

                'dont use iteration variables in lamdas (inside of getasynclookup method) - so set this locally
                Dim sFileName As String = sFile
                GetAsyncLookup(sFileName)

            Next
        End If

    End Sub

#End Region

#Region "Property"

#Region "Properties"

    Public ReadOnly Property PropertyAutoCompleteXML As XmlDocument
        Get

            Dim oPropertyAutoCompleteXML As New XmlDocument
            SyncLock Me.PropertyLock

                oPropertyAutoCompleteXML = CType(HttpContext.Current.Cache("lookups_propertyautocompletexml"), XmlDocument)
                If oPropertyAutoCompleteXML Is Nothing Then

                    oPropertyAutoCompleteXML = Utility.BigCXML("PropertyAutoComplete", 1, 0)
                    AddToCache("lookups_propertyautocompletexml", oPropertyAutoCompleteXML, 600)

                End If

            End SyncLock

            Return oPropertyAutoCompleteXML

        End Get
    End Property

    Public ReadOnly Property PropertyReferences As List(Of PropertyReference)
        Get
            Dim oPropertyReferences As Generic.List(Of PropertyReference) = AsyncCache.Controller(Of List(Of PropertyReference)).GetCache("Cachekey", 600,
            Function()

                Dim oCachedReferences As New Generic.List(Of PropertyReference)

                oCachedReferences = Utility.XMLToGenericList(Of PropertyReference)(Me.PropertyAutoCompleteXML)
                Intuitive.Functions.AddToCache("lookups_propertyreferences", oCachedReferences, 600)
                Return oCachedReferences

            End Function, "^(?!<Lookups).*")

            Return oPropertyReferences

        End Get
    End Property

#End Region

#Region "Classes"

    Public Class PropertyReference
        Public PropertyReferenceID As Integer
        Public PropertyName As String
        Public GeographyLevel2ID As Integer
        Public GeographyLevel3ID As Integer
        Public GeographyLevel3Name As String
    End Class

#End Region

#End Region

#Region "Geography"

#Region "Properties"

    Public ReadOnly Property Locations As List(Of Location)
        Get

            Dim iDisplayLanguage As Integer = 0
            If HttpContext.Current IsNot Nothing Then
                iDisplayLanguage = BookingBase.DisplayLanguageID
            End If

            'filter on brand geography
            Dim oLocations As New List(Of Location)
            If Not GetCache(Of List(Of Location))(CacheName("Locations_" & iDisplayLanguage)) Is Nothing Then
                oLocations = GetCache(Of List(Of Location))(CacheName("Locations_" & iDisplayLanguage))
            End If

            If oLocations.Count = 0 Then

                'get all locations
                Dim oAllLocations As List(Of Location) = Me.LookupCollection(Of Location)(LookupCollectionIdentifier.Locations)

                For Each oBrandGeographyLevel3 As BrandGeography In Me.BrandGeographyLevel3
                    Dim iGeographyLevel3ID As Integer = oBrandGeographyLevel3.GeographyLevel3ID
                    Dim oLocation As Location = Nothing
                    Try
                        oLocation = oAllLocations.Where(Function(o) o.GeographyLevel3ID = iGeographyLevel3ID).First
                    Catch ex As Exception
                        'continue - for some reason there are BrandGeographyLevel3 entries for non existent Geography Level 3 IDs
                    End Try
                    If Not oLocation Is Nothing Then oLocations.Add(oLocation)
                Next

                AddToCache(CacheName("Locations_" & iDisplayLanguage), oLocations)

            End If

            'return
            Return oLocations

        End Get
    End Property

    Public ReadOnly Property TranslatedLocations As List(Of TranslatedLocation)
        Get

            'filter on brand geography
            Dim oLocations As New List(Of TranslatedLocation)

            If Not HttpRuntime.Cache("TranslatedLocations") Is Nothing Then
                oLocations = CType(HttpRuntime.Cache("TranslatedLocations"), List(Of TranslatedLocation))
            Else
                'get all locations
                Dim oAllLocations As New List(Of TranslatedLocation)
                oAllLocations = Me.LookupCollection(Of TranslatedLocation)(LookupCollectionIdentifier.TranslatedLocations)

                For Each oBrandGeographyLevel3 As BrandGeography In Me.BrandGeographyLevel3
                    Dim iGeographyLevel3ID As Integer = oBrandGeographyLevel3.GeographyLevel3ID
                    Dim oLocation As TranslatedLocation = Nothing
                    Try
                        oLocation = oAllLocations.Where(Function(o) o.GeographyLevel3ID = iGeographyLevel3ID).First
                    Catch ex As Exception
                        'continue - for some reason there are BrandGeographyLevel3 entries for non existent Geography Level 3 IDs
                    End Try
                    If Not oLocation Is Nothing Then oLocations.Add(oLocation)
                Next

                HttpRuntime.Cache("TranslatedLocations") = oLocations

            End If

            'return
            Return oLocations

        End Get
    End Property

    Public Property GeographyDictionary As Dictionary(Of Integer, RegionResort)

        Get
            Dim iDisplayLanguage As Integer = 0
            If HttpContext.Current IsNot Nothing Then
                iDisplayLanguage = BookingBase.DisplayLanguageID
            End If
            If Not HttpRuntime.Cache("GeographyDictionary_" & iDisplayLanguage) Is Nothing Then
                Return CType(HttpRuntime.Cache("GeographyDictionary_" & iDisplayLanguage), Dictionary(Of Integer, RegionResort))
            Else

                Dim oDictionary As New Dictionary(Of Integer, RegionResort)

                'populate locations
                For Each oLocation As Location In Me.Locations

                    Dim oRegionResort As New RegionResort
                    With oRegionResort
                        .GeographyLevel2ID = oLocation.GeographyLevel2ID
                        .Region = oLocation.GeographyLevel2Name
                        .GeographyLevel3ID = oLocation.GeographyLevel3ID
                        .Resort = oLocation.GeographyLevel3Name
                    End With
                    oDictionary.Add(oLocation.GeographyLevel3ID, oRegionResort)

                Next

                HttpRuntime.Cache("GeographyDictionary_" & iDisplayLanguage) = oDictionary
                Return oDictionary

            End If
        End Get
        Set(value As Dictionary(Of Integer, RegionResort))
            Dim iDisplayLanguage As Integer = 0
            If HttpContext.Current IsNot Nothing Then
                iDisplayLanguage = BookingBase.DisplayLanguageID
            End If
            HttpRuntime.Cache("GeographyDictionary_" & iDisplayLanguage) = value
        End Set
    End Property

    Public ReadOnly Property BrandGeographyLevel3() As List(Of BrandGeography)
        Get

            '1. if in cache return
            If Not HttpRuntime.Cache(CacheName("brandgeography")) Is Nothing Then
                Return CType(HttpRuntime.Cache(CacheName("brandgeography")), List(Of BrandGeography))
            End If

            '2. build list
            Dim oXML As XmlDocument = Me.GetLookupXML("/Lookups/BrandGeographies", "BrandGeographies")
            Dim oList As List(Of BrandGeography) = Utility.XMLToGenericList(Of BrandGeography)(oXML, "BrandGeographies/BrandGeography[BrandID=" & Params.BrandID & "]")

            '3. add to cache and return
            AddToCache(CacheName("brandgeography"), oList, 60)

            Return oList

        End Get
    End Property

    Public Function ListRegionsByAirportID(ByVal DepartureAirportID As Integer) As KeyValuePairsWithParent

        '1. if in cache return
        Dim sCacheName As String = "listregionsbyairport_" & DepartureAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        '2 build up lookups

        'get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)
        'then get all regions/resorts served by the arrival airports
        Dim oGeographyLevel3IDs As List(Of Integer) = Me.GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

        Dim oLocations As New List(Of Location)
        Dim oRegionIDs As New List(Of Integer)

        For Each oLocation As Location In Me.Locations
            If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then

                If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then
                    oRegionIDs.Add(oLocation.GeographyLevel2ID)
                    oLocations.Add(oLocation)
                End If

            End If
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(oLocations, "GeographyLevel2ID", "GeographyLevel2Name", "GeographyLevel1Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)
        Return oKeyValuePairs

    End Function

    Public Function ListRegionsByCountryAndAirport(ByVal DepartureCountryID As Integer, ByVal DepartureAirportID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "listregionsbycountry_" & DepartureCountryID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2 build up lookups
        Dim oLocations As New List(Of Location)
        Dim oRegionIDs As New List(Of Integer)

        For Each oLocation As Location In Me.Locations.OrderBy(Function(o) o.GeographyLevel2Name)

            If DepartureAirportID > 0 Then

                'get list of arrival airports, based on departure airport
                Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)

                'then get all regions/resorts served by the arrival airports
                Dim oGeographyLevel3IDs As List(Of Integer) = Me.GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

                If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) And oLocation.GeographyLevel1ID = DepartureCountryID Then
                    If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then
                        oRegionIDs.Add(oLocation.GeographyLevel2ID)
                        oLocations.Add(oLocation)
                    End If
                End If

            Else

                If oLocation.GeographyLevel1ID = DepartureCountryID Then
                    If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then
                        oRegionIDs.Add(oLocation.GeographyLevel2ID)
                        oLocations.Add(oLocation)
                    End If

                End If

            End If
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oLocations, "GeographyLevel2ID", "GeographyLevel2Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)

        Return oKeyValuePairs

    End Function

    Public Function ListResortsByRegionAndAirport(ByVal RegionID As Integer, ByVal DepartureAirportID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "listresortsbyregionandairport_" & RegionID & "_" & DepartureAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2 build up lookups
        Dim oLocations As New List(Of Location)

        If DepartureAirportID > 0 Then

            'get list of arrival airports, based on departure airport
            Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)
            'then get all regions/resorts served by the arrival airports
            Dim oGeographyLevel3IDs As List(Of Integer) = Me.GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

            For Each oLocation As Location In Me.Locations.OrderBy(Function(o) o.GeographyLevel3Name)
                If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then
                    If oLocation.GeographyLevel2ID = RegionID Then oLocations.Add(oLocation)
                End If
            Next

        Else
            For Each oLocation As Location In Me.Locations.OrderBy(Function(o) o.GeographyLevel3Name)
                If oLocation.GeographyLevel2ID = RegionID Then oLocations.Add(oLocation)
            Next
        End If

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oLocations, "GeographyLevel3ID", "GeographyLevel3Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)
        Return oKeyValuePairs

    End Function

    Public Function GetLocationFromResort(ByVal ResortID As Integer) As Location
        'newly mapped locations will cause this to break if not in a try catch
        Try
            Dim oLocation As New Location
            oLocation = Me.Locations.Where(Function(o) o.GeographyLevel3ID = ResortID).FirstOrDefault
            Return oLocation
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetLocationFromRegion(ByVal RegionID As Integer) As Location
        'newly mapped locations will cause this to break if not in a try catch
        Try
            Dim oLocation As New Location
            oLocation = Me.Locations.Where(Function(o) o.GeographyLevel2ID = RegionID).FirstOrDefault
            Return oLocation
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function ListAirportMappedRegions(ByVal AirportID As Integer) As KeyValuePairsWithParent

        '1. if in cache return
        Dim sCacheName As String = "listairportmappedregions_" & AirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        '2. get airport regions
        Dim oGeographyLevel3IDs As List(Of Integer) = Me.GetAirportGeographyLevel3IDs(AirportID)
        Dim oGeographyLevel2Locations As New List(Of Location)

        For Each oLocation As Location In Me.Locations
            If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then
                oGeographyLevel2Locations.Add(oLocation)
            End If
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(oGeographyLevel2Locations, "GeographyLevel2ID", "GeographyLevel2Name", "GeographyLevel1Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)
        Return oKeyValuePairs

    End Function

    Public Function ListAirportMappedResorts(ByVal AirportID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "listairportmappedresorts_" & AirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2. get airport resorts
        Dim oGeographyLevel3IDs As List(Of Integer) = Me.GetAirportGeographyLevel3IDs(AirportID)
        Dim oGeographyLevel3Locations As New List(Of Location)

        For Each oLocation As Location In Me.Locations
            If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then
                oGeographyLevel3Locations.Add(oLocation)
            End If
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oGeographyLevel3Locations, "GeographyLevel3ID", "GeographyLevel3Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)
        Return oKeyValuePairs

    End Function

    Public Function IsValidRouteForSupplier(iDepartureID As Integer, iArrivalID As Integer, iSupplierID As Integer) As Boolean

        Return FlightSupplierRoutes.Where(Function(o) o.ArrivalAirportID = iArrivalID _
                                 And o.DepartureAirportID = iDepartureID _
                                    And o.Suppliers _
                                        .Where(Function(s) s.SupplierID = iSupplierID).Count > 0).Count > 0
    End Function

#End Region

#Region "GeographySearchLookup"

    Public ReadOnly Property GeographySearchLookup(Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of GeographyLevel)
        Get

            '1. if in cache return
            If Not HttpRuntime.Cache(CacheName("geographysearchlookup")) Is Nothing Then
                Return CType(HttpRuntime.Cache(CacheName("geographysearchlookup")), List(Of GeographyLevel))
            End If

            '2 build up lookups
            Dim oLookups As New List(Of GeographyLevel)
            Dim oRegionIDs As New List(Of Integer)

            For Each oLocation As Location In Me.Locations

                'add region if it does not exist
                If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then

                    Dim oRegion As New GeographyLevel
                    With oRegion
                        .Name = oLocation.GeographyLevel2Name
                        If IncludeGeographyLevel1Name Then .Name += ", " & oLocation.GeographyLevel1Name
                        .ID = oLocation.GeographyLevel2ID
                    End With

                    oRegionIDs.Add(oRegion.ID)
                    oLookups.Add(oRegion)
                End If

                'set up resort and add to lookups (if not same as region name)
                If oLocation.GeographyLevel2Name <> oLocation.GeographyLevel3Name Then

                    Dim oResort As New GeographyLevel
                    With oResort
                        .Name = oLocation.GeographyLevel3Name & ", " & oLocation.GeographyLevel2Name
                        .ID = oLocation.GeographyLevel3ID * -1
                    End With

                    oLookups.Add(oResort)

                End If

            Next

            '3. add to cache and return
            AddToCache(CacheName("geographysearchlookup"), oLookups, 60)
            Return oLookups

        End Get
    End Property

    Public ReadOnly Property TranslatedGeographySearchLookup(Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of TranslatedGeographyLevel)
        Get
            '1. if in cache return
            If Not HttpRuntime.Cache("translatedgeographysearchlookup") Is Nothing Then
                Return CType(HttpRuntime.Cache("translatedgeographysearchlookup"), List(Of TranslatedGeographyLevel))
            End If

            '2 build up lookups
            Dim oLookups As New List(Of TranslatedGeographyLevel)
            Dim oRegionIDs As New List(Of Integer)

            For Each oLocation As TranslatedLocation In Me.TranslatedLocations

                'add region if it does not exist
                If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then

                    Dim oRegion As New TranslatedGeographyLevel
                    With oRegion
                        .Translations = oLocation.Translations.ToDictionary(Of Integer, String)(
                            Function(translation) translation.LanguageID,
                            Function(translation) IIf(IncludeGeographyLevel1Name,
                                                      translation.GeographyLevel2Name + ", " + translation.GeographyLevel1Name,
                                                      translation.GeographyLevel2Name))
                        .ID = oLocation.GeographyLevel2ID
                    End With

                    oRegionIDs.Add(oRegion.ID)
                    oLookups.Add(oRegion)
                End If

                'set up resort and add to lookups (if not same as region name)
                Dim oResort As New TranslatedGeographyLevel
                oResort.ID = oLocation.GeographyLevel3ID * -1
                For Each translation As GeographyTranslation In oLocation.Translations
                    If translation.GeographyLevel2Name <> translation.GeographyLevel3Name Then
                        oResort.Translations.Add(translation.LanguageID,
                                                 translation.GeographyLevel3Name + ", " + translation.GeographyLevel2Name)
                    End If
                Next

                If oResort.Translations.Count > 0 Then oLookups.Add(oResort)

            Next

            '3. add to cache and return
            AddToCache("translatedgeographysearchlookup", oLookups, 60)
            Return oLookups

        End Get
    End Property

    Public Function GeographySearchLookupByDepartureAirportID(ByVal DepartureAirportID As Integer, Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of GeographyLevel)

        '1. if in cache return
        Dim sCacheName As String = "geographysearchlookupbyairport_" & DepartureAirportID

        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), List(Of GeographyLevel))
        End If

        '2 build up lookups
        Dim oLookups As New List(Of GeographyLevel)

        'get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = GetArrivalAirportIDsByDeparture(DepartureAirportID)
        'then get all regions/resorts served by the arrival airports
        Dim oGeographyLevel3IDs As List(Of Integer) = GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

        Dim oRegionIDs As New List(Of Integer)

        For Each oLocation As Location In Me.Locations

            If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then
                'add region if it does not exist
                If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then

                    Dim oRegion As New GeographyLevel
                    With oRegion
                        .Name = oLocation.GeographyLevel2Name
                        If IncludeGeographyLevel1Name Then .Name += ", " & oLocation.GeographyLevel1Name
                        .ID = oLocation.GeographyLevel2ID
                    End With

                    oRegionIDs.Add(oRegion.ID)
                    oLookups.Add(oRegion)
                End If

                'set up resort and add to lookups (if not same as region name)
                If oLocation.GeographyLevel2Name <> oLocation.GeographyLevel3Name Then
                    'but only if its in the geo3 list

                    Dim oResort As New GeographyLevel
                    With oResort
                        .Name = oLocation.GeographyLevel3Name & ", " & oLocation.GeographyLevel2Name
                        .ID = oLocation.GeographyLevel3ID * -1
                    End With

                    oLookups.Add(oResort)

                End If

            End If

        Next

        '3. add to cache and return
        AddToCache(CacheName(sCacheName), oLookups, 60)
        Return oLookups

    End Function

    Public Function TranslatedGeographySearchLookupByDepartureAirportID(ByVal DepartureAirportID As Integer, Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of TranslatedGeographyLevel)
        '1. if in cache return
        Dim sCacheName As String = "translatedgeographysearchlookupbyairport_" & DepartureAirportID

        If Not HttpRuntime.Cache(sCacheName) Is Nothing Then
            Return CType(HttpRuntime.Cache(sCacheName), List(Of TranslatedGeographyLevel))
        End If

        '2 build up lookups
        Dim oLookups As New List(Of TranslatedGeographyLevel)

        'get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = GetArrivalAirportIDsByDeparture(DepartureAirportID)
        'then get all regions/resorts served by the arrival airports
        Dim oGeographyLevel3IDs As List(Of Integer) = GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

        Dim oRegionIDs As New List(Of Integer)

        For Each oLocation As TranslatedLocation In Me.TranslatedLocations

            If oGeographyLevel3IDs.Contains(oLocation.GeographyLevel3ID) Then
                'add region if it does not exist
                If Not oRegionIDs.Contains(oLocation.GeographyLevel2ID) Then

                    Dim oRegion As New TranslatedGeographyLevel
                    With oRegion
                        .Translations = oLocation.Translations.ToDictionary(Of Integer, String)(
                            Function(translation) translation.LanguageID,
                            Function(translation) IIf(IncludeGeographyLevel1Name,
                                                      translation.GeographyLevel2Name + ", " + translation.GeographyLevel1Name,
                                                      translation.GeographyLevel2Name))
                        .ID = oLocation.GeographyLevel2ID
                    End With

                    oRegionIDs.Add(oRegion.ID)
                    oLookups.Add(oRegion)
                End If

                'set up resort and add to lookups (if not same as region name)
                Dim oResort As New TranslatedGeographyLevel
                oResort.ID = oLocation.GeographyLevel3ID * -1
                For Each oTranslation As GeographyTranslation In oLocation.Translations
                    If oTranslation.GeographyLevel2Name <> oTranslation.GeographyLevel3Name Then
                        oResort.Translations.Add(oTranslation.LanguageID, oTranslation.GeographyLevel3Name + ", " + oTranslation.GeographyLevel2Name)
                    End If
                Next

                If oResort.Translations.Count > 0 Then oLookups.Add(oResort)

            End If

        Next

        '3. add to cache and return
        AddToCache(sCacheName, oLookups, 60)
        Return oLookups

    End Function

#End Region

#Region "Search Geography"

    Public Function SearchGeography(ByVal SearchText As String, ByVal DepartureAirportID As Integer, Optional ByVal MaxResults As Integer = 0,
                                    Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of GeographyLevel)

        'get search geography either by airport or all
        Dim oSearchGeography As New List(Of GeographyLevel)
        If DepartureAirportID > 0 Then
            oSearchGeography = Me.GeographySearchLookupByDepartureAirportID(DepartureAirportID, IncludeGeographyLevel1Name)
        Else
            oSearchGeography = Me.GeographySearchLookup(IncludeGeographyLevel1Name)
        End If

        'loop through each location and add region/resorts that match text input
        Dim oRegionResorts As New List(Of GeographyLevel)
        For Each oGeographyLevel As GeographyLevel In oSearchGeography _
           .Where(Function(o) o.Name.ToLower.StartsWith(SearchText.ToLower) OrElse o.Name.ToLower.Contains(" " & SearchText.ToLower)) _
           .OrderBy(Function(o) o.Name) _
           .OrderBy(Function(o) IIf(o.ID > 0, 0, 1)) _
         .OrderBy(Function(o) IIf(o.Name.ToLower.StartsWith(SearchText.ToLower), 0, 1))

            oRegionResorts.Add(oGeographyLevel)
        Next

        'If there is no max results set, set it to the maximum number, otherwise get the appropriate number
        Dim iMaxResults As Integer
        If MaxResults = 0 Then
            iMaxResults = oRegionResorts.Count
        Else
            iMaxResults = IIf(oRegionResorts.Count < MaxResults, oRegionResorts.Count, MaxResults)
        End If

        'return results
        oRegionResorts = oRegionResorts.GetRange(0, iMaxResults)
        Return oRegionResorts

    End Function

    Public Function SearchTranslatedGeography(ByVal SearchText As String, ByVal DepartureAirportID As Integer,
                                              ByVal LanguageID As Integer, Optional ByVal MaxResults As Integer = 0,
                                              Optional ByVal IncludeGeographyLevel1Name As Boolean = False) As List(Of TranslatedGeographyLevel)

        'get search geography either by airport or all
        Dim oSearchGeography As New List(Of TranslatedGeographyLevel)
        If DepartureAirportID > 0 Then
            oSearchGeography = Me.TranslatedGeographySearchLookupByDepartureAirportID(DepartureAirportID, IncludeGeographyLevel1Name)
        Else
            oSearchGeography = Me.TranslatedGeographySearchLookup(IncludeGeographyLevel1Name)
        End If

        'loop through each location and add region/resorts that match text input
        Dim oRegionResorts As New List(Of TranslatedGeographyLevel)
        For Each oGeographyLevel As TranslatedGeographyLevel In oSearchGeography.
            Where(Function(o) o.Translations.ContainsKey(LanguageID) AndAlso
                      (o.Translations.Where(Function(translation) translation.Value.ToLower.StartsWith(SearchText.ToLower) OrElse
                                translation.Value.ToLower.Contains(" " & SearchText.ToLower))).
                      Any).
            OrderBy(Function(o) o.Translations(LanguageID)).
            OrderBy(Function(o) IIf(o.ID > 0, 0, 1)).
            OrderBy(Function(o) IIf(o.Translations(LanguageID).ToLower.StartsWith(SearchText.ToLower), 0, 1))

            oRegionResorts.Add(oGeographyLevel)
        Next

        'If there is no max results set, set it to the maximum number, otherwise get the appropriate number
        Dim iMaxResults As Integer
        If MaxResults = 0 Then
            iMaxResults = oRegionResorts.Count
        Else
            iMaxResults = IIf(oRegionResorts.Count < MaxResults, oRegionResorts.Count, MaxResults)
        End If

        'return results
        oRegionResorts = oRegionResorts.GetRange(0, iMaxResults)
        Return oRegionResorts

    End Function

#End Region

#Region "Geography Grouping"

    Public ReadOnly Property GeographyGroupings As List(Of GeographyGrouping)
        Get
            Dim oAllGeographyGroups As New List(Of GeographyGrouping)
            oAllGeographyGroups = Me.LookupCollection(Of GeographyGrouping)(LookupCollectionIdentifier.GeographyGroupings)

            Return oAllGeographyGroups.Where(Function(o) o.BrandID = BookingBase.Params.BrandID OrElse o.BrandID = 0).ToList()
        End Get
    End Property

    Public ReadOnly Property TranslatedGeographyGroupings As List(Of TranslatedGeographyGrouping)
        Get
            Dim oAllGeographyGroups As New List(Of TranslatedGeographyGrouping)
            oAllGeographyGroups = Me.LookupCollection(Of TranslatedGeographyGrouping)(LookupCollectionIdentifier.TranslatedGeographyGroupings)

            Return oAllGeographyGroups.Where(Function(o) o.BrandID = BookingBase.Params.BrandID OrElse o.BrandID = 0).ToList()
        End Get
    End Property

    Public ReadOnly Property GeographyGroupingGeographies As List(Of GeographyGroupingGeography)
        Get

            Dim oAllGeographyGroupingGeographies As New List(Of GeographyGroupingGeography)
            oAllGeographyGroupingGeographies = Me.LookupCollection(Of GeographyGroupingGeography)(LookupCollectionIdentifier.GeographyGroupingGeographies)

            Dim oGeographyGroupingGeographies As New List(Of GeographyGroupingGeography)

            For Each oGeographyGrouping As GeographyGrouping In Me.GeographyGroupings
                oGeographyGroupingGeographies.AddRange(oAllGeographyGroupingGeographies.Where(Function(o) o.GeographyGroupingID = oGeographyGrouping.GeographyGroupingID).ToList())
            Next

            Return oGeographyGroupingGeographies

        End Get
    End Property

    Public Function GeographyGroupGeographies(ByVal iGeographyGroupingID As Integer) As List(Of Integer)

        Dim aGeographies As New List(Of Integer)

        For Each oGeographyGroupingGeography As GeographyGroupingGeography In
                Me.GeographyGroupingGeographies.Where(Function(o) o.GeographyGroupingID = iGeographyGroupingID)

            aGeographies.Add(oGeographyGroupingGeography.GeographyID)
        Next

        Return aGeographies

    End Function

    Public ReadOnly Property GeographyGroupSearchLookup() As List(Of GeographyGrouping)
        Get
            '1. if in cache return
            If Not HttpRuntime.Cache(CacheName("geographygroupsearchlookup")) Is Nothing Then
                Return CType(HttpRuntime.Cache(CacheName("geographygroupsearchlookup")), List(Of GeographyGrouping))
            End If

            Dim oLookups As New List(Of GeographyGrouping)

            For Each oGeographyGroup As GeographyGrouping In Me.GeographyGroupings

                Dim oGroup As New GeographyGrouping
                oGroup.GeographyGroup = oGeographyGroup.GeographyGroup
                oGroup.GeographyGroupingID = (oGeographyGroup.GeographyGroupingID * -1) - 1000000
                oGroup.ShowInSearch = oGeographyGroup.ShowInSearch
                oLookups.Add(oGroup)
            Next

            '3. add to cache and return
            AddToCache(CacheName("geographygroupsearchlookup"), oLookups, 60)
            Return oLookups
        End Get
    End Property

    Public ReadOnly Property TranslatedGeographyGroupSearchLookup() As List(Of TranslatedGeographyGrouping)
        Get
            '1. if in cache return
            If Not HttpRuntime.Cache("translatedgeographygroupsearchlookup") Is Nothing Then
                Return CType(HttpRuntime.Cache("translatedgeographygroupsearchlookup"), List(Of TranslatedGeographyGrouping))
            End If

            Dim oLookups As New List(Of TranslatedGeographyGrouping)

            For Each oGeographyGroup As TranslatedGeographyGrouping In Me.TranslatedGeographyGroupings

                Dim oGroup As New TranslatedGeographyGrouping
                oGroup.Translations = oGeographyGroup.Translations
                oGroup.GeographyGroupingID = (oGeographyGroup.GeographyGroupingID * -1) - 1000000
                oGroup.ShowInSearch = oGeographyGroup.ShowInSearch
                oLookups.Add(oGroup)
            Next

            '3. add to cache and return
            AddToCache("translatedgeographygroupsearchlookup", oLookups, 60)
            Return oLookups
        End Get
    End Property

    Public Function SearchGeographyGroups(ByVal SearchText As String, Optional ByVal MaxResults As Integer = 0) As List(Of GeographyGrouping)

        Dim oSearchGeographyGroups As New List(Of GeographyGrouping)
        oSearchGeographyGroups = Me.GeographyGroupSearchLookup()

        Dim oGeographyGroups As New List(Of GeographyGrouping)

        For Each oGeographyGroup As GeographyGrouping In oSearchGeographyGroups _
                .Where(Function(o) o.GeographyGroup.ToLower.StartsWith(SearchText.ToLower) _
                        OrElse o.GeographyGroup.ToLower.Contains(" " & SearchText.ToLower) _
                        AndAlso o.ShowInSearch) _
                .OrderBy(Function(o) o.GeographyGroup) _
                .OrderBy(Function(o) IIf(o.GeographyGroupingID > 0, 0, 1)) _
                .OrderBy(Function(o) IIf(o.GeographyGroup.ToLower.StartsWith(SearchText.ToLower), 0, 1))

            oGeographyGroups.Add(oGeographyGroup)
        Next

        Dim iMaxResults As Integer
        If MaxResults = 0 Then
            iMaxResults = oGeographyGroups.Count
        Else
            iMaxResults = IIf(oGeographyGroups.Count < MaxResults, oGeographyGroups.Count, MaxResults)
        End If

        oGeographyGroups = oGeographyGroups.GetRange(0, iMaxResults)

        Return oGeographyGroups

    End Function

    Public Function SearchTranslatedGeographyGroups(ByVal SearchText As String, ByVal LanguageID As Integer,
                                                    Optional ByVal MaxResults As Integer = 0) As List(Of TranslatedGeographyGrouping)

        Dim oSearchGeographyGroups As New List(Of TranslatedGeographyGrouping)
        oSearchGeographyGroups = Me.TranslatedGeographyGroupSearchLookup()

        Dim oGeographyGroups As New List(Of TranslatedGeographyGrouping)

        For Each oGeographyGroup As TranslatedGeographyGrouping In oSearchGeographyGroups.
            Where(Function(o) o.Translations.
                      Where(Function(translation) translation.GeographyGrouping.ToLower.StartsWith(SearchText.ToLower) _
                                OrElse translation.GeographyGrouping.ToLower.Contains(" " & SearchText.ToLower)).Any _
                                AndAlso o.ShowInSearch _
                                AndAlso o.Translations.Where(Function(translation) translation.LanguageID = LanguageID).Any).
            OrderBy(Function(o) o.Translations.
                        Where(Function(translation) translation.LanguageID = LanguageID).
                        FirstOrDefault().GeographyGrouping).
            OrderBy(Function(o) IIf(o.GeographyGroupingID > 0, 0, 1)).
            OrderBy(Function(o) IIf(o.Translations.
                        Where(Function(translation) translation.LanguageID = LanguageID).
                        FirstOrDefault().GeographyGrouping.ToLower.StartsWith(SearchText.ToLower), 0, 1))

            oGeographyGroups.Add(oGeographyGroup)
        Next

        Dim iMaxResults As Integer
        If MaxResults = 0 Then
            iMaxResults = oGeographyGroups.Count
        Else
            iMaxResults = IIf(oGeographyGroups.Count < MaxResults, oGeographyGroups.Count, MaxResults)
        End If

        oGeographyGroups = oGeographyGroups.GetRange(0, iMaxResults)

        Return oGeographyGroups

    End Function

    Public Function GeographyGroupingNameBy(ByVal GeographyGroupingID As Integer, ByVal LanguageID As Integer) As String

        Dim oGeographyGrouping As TranslatedGeographyGrouping = Me.TranslatedGeographyGroupings.Where(Function(geographyGrouping) geographyGrouping.GeographyGroupingID = GeographyGroupingID).FirstOrDefault()

        If oGeographyGrouping Is Nothing Then Return String.Empty

        Return oGeographyGrouping.Translations.
            Where(Function(geographyGrouping) geographyGrouping.LanguageID = LanguageID OrElse geographyGrouping.LanguageID = BookingBase.Params.BaseLanguageID).
            OrderBy(Function(geographyGrouping) Not geographyGrouping.LanguageID = LanguageID).
            FirstOrDefault.GeographyGrouping

    End Function

#End Region


#Region "Nationality"
	Public Function GetNationalityIdByName(sNationality As String) As Integer
		For Each dr As DataRow In SQL.GetDataTableCache("exec Nationality_List").Rows
			If dr("Nationality").ToString() = sNationality Then
				Return Intuitive.ToSafeInt(dr("NationalityID"))
			End If
		Next
		Return 0
	End Function
#End Region

#Region "Classes"

    Public Class GeographyRegion
        Public Resorts As New List(Of GeographyLevel)
        Public RegionName As String
        Public RegionID As Integer
    End Class

    Public Class GeographyLevel
        Public Name As String
        Public ID As Integer
    End Class

    Public Class TranslatedGeographyLevel
        Public ID As Integer
        Public Translations As New Dictionary(Of Integer, String)
    End Class

    Public Class BrandGeography
        Public BrandID As Integer
        Public GeographyLevel3ID As Integer
    End Class

    Public Class Location
        Public GeographyLevel1ID As Integer
        Public GeographyLevel1Name As String
        Public GeographyLevel2ID As Integer
        Public GeographyLevel2Name As String
        Public GeographyLevel3ID As Integer
        Public GeographyLevel3Name As String
    End Class

    Public Class TranslatedLocation
        Public GeographyLevel1ID As Integer
        Public GeographyLevel1Code As String
        Public GeographyLevel2ID As Integer
        Public GeographyLevel2Code As String
        Public GeographyLevel3ID As Integer
        Public GeographyLevel3Code As String
        Public Translations As List(Of GeographyTranslation)
    End Class

    Public Class GeographyTranslation
        Public LanguageID As Integer
        Public Language As String
        Public SystemsLanguage As Boolean
        Public GeographyLevel1Name As String
        Public GeographyLevel2Name As String
        Public GeographyLevel3Name As String
    End Class

    Public Class RegionResort
        Public GeographyLevel2ID As Integer
        Public Region As String
        Public GeographyLevel3ID As Integer
        Public Resort As String
    End Class

    Public Class GeographyGrouping
        Public GeographyGroupingID As Integer
        Public GeographyGroup As String
        Public Level As String
        Public BrandID As Integer
        Public ShowInSearch As Boolean
    End Class

    Public Class TranslatedGeographyGrouping
        Public GeographyGroupingID As Integer
        Public Level As String
        Public BrandID As Integer
        Public ShowInSearch As Boolean
        Public Translations As List(Of GeographyGroupingTranslation)
    End Class

    Public Class GeographyGroupingTranslation
        Public LanguageID As Integer
        Public Language As String
        Public SystemsLanguage As Boolean
        Public GeographyGrouping As String
    End Class

    Public Class GeographyGroupingGeography
        Public GeographyGroupingID As Integer
        Public GeographyID As Integer
    End Class

#End Region

#End Region

#Region "Booking"

    Public Function ListBookingRegionsByCountryID(ByVal BookingCountryID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "ListBookingRegionsByCountryID_" & BookingCountryID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2. get booking country regions
        Dim oBookingCountryRegions As New List(Of BookingCountryRegion)
        For Each oBookingCountryRegion As BookingCountryRegion In Me.BookingCountryRegions.Where(Function(o) o.BookingCountryID = BookingCountryID)
            oBookingCountryRegions.Add(oBookingCountryRegion)
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oBookingCountryRegions, "BookingCountryRegionID", "Region")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)
        Return oKeyValuePairs

    End Function

    Public Class BookingCountryRegion
        Public BookingCountryRegionID As Integer
        Public BookingCountryID As Integer
        Public Region As String
    End Class

    Public ReadOnly Property BookingCountryRegions As List(Of BookingCountryRegion)
        Get
            Return LookupCollection(Of BookingCountryRegion)(LookupCollectionIdentifier.BookingCountryRegions)
        End Get
    End Property

#End Region

#Region "Car hire"

#Region "Properties"

    Public ReadOnly Property CarHireDepots As List(Of CarHireDepot)
        Get
            Return Me.LookupCollection(Of CarHireDepot)(LookupCollectionIdentifier.CarHireDepots)
        End Get
    End Property

    Public ReadOnly Property CarTypes As List(Of CarType)
        Get
            Return Me.LookupCollection(Of CarType)(LookupCollectionIdentifier.CarTypes)
        End Get
    End Property

#End Region

#Region "Classes"

    Public Class CarHireDepot
        Public CarHireDepotID As Integer
        Public DepotName As String
        Public AirportID As Integer
        Public GeographyLevel1ID As Integer
    End Class

    Public Class CarType
        Public CarHireContractID As Integer
        Public CarHireContractCarTypeID As Integer
        Public CarTypeID As Integer
    End Class

#End Region

#Region "Functions"

    Public Function GetDepotByDepotID(ByVal CarHireDepotID As Integer) As CarHireDepot

        Dim oCarHireDepots As List(Of CarHireDepot) = Me.CarHireDepots.Where(Function(oDepot) oDepot.CarHireDepotID = CarHireDepotID).ToList()

        If oCarHireDepots.Count > 0 Then
            Return oCarHireDepots.First()
        Else
            Return New CarHireDepot
        End If

    End Function

    Public Function GetDepotsByAirportID(ByVal ArrivalAirportID As Integer) As List(Of CarHireDepot)

        Dim oCarHireDepots As New List(Of CarHireDepot)
        For Each oDepot As CarHireDepot In Me.CarHireDepots
            If oDepot.AirportID = ArrivalAirportID Then
                oCarHireDepots.Add(oDepot)
            End If
        Next

        Return oCarHireDepots
    End Function

    Public Function ListValidCarHireGeographies(ByVal BrandID As Integer) As KeyValuePairs

        Dim sCacheName As String = "ListValidCarHireGeographies_" & BrandID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        Dim oAllCountries As KeyValuePairs = Me.GenerateKeyValuePairs(LookupTypes.Country)

        Dim oFilteredGeographies As New KeyValuePairs

        For Each oPair As KeyValuePair(Of Integer, String) In oAllCountries
            Dim oDepots As New KeyValuePairs
            oDepots = Me.ListCarHireDepotsByCountryID(oPair.Key)

            If oDepots.Count > 0 Then
                oFilteredGeographies.Add(oPair.Key, oPair.Value)
            End If
        Next

        HttpRuntime.Cache(CacheName(sCacheName)) = oFilteredGeographies

        Return oFilteredGeographies

    End Function

    Public Function ListCarHireDeposByResortID(ByVal ResortID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "listCarDepotsByResort_" & ResortID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2.Get a list of all the depotIDS for our resort
        Dim sDepotIDS As String() = Me.GetKeyPairValue(LookupTypes.CarHireDepotGeographyLevel3, ResortID).Split(New Char() {","c})

        '2b.Turn that into a list of ints
        Dim oDepotIDS As New List(Of Integer)
        For i As Integer = 0 To sDepotIDS.Length - 1
            oDepotIDS.Add(Intuitive.ToSafeInt(sDepotIDS(i)))
        Next

        'Get a list of all car hire depots
        Dim oAllCarHireDepots As KeyValuePairs = Me.GenerateKeyValuePairs(LookupTypes.CarHireDepot)
        Dim oFilteredCarHireDepots As New KeyValuePairs

        'Loop through the list of all car hire depots and see if any match our resort, if they do add them to our filtered collection
        For Each oPair As KeyValuePair(Of Integer, String) In oAllCarHireDepots

            If oDepotIDS.Contains(oPair.Key) Then
                oFilteredCarHireDepots.Add(oPair.Key, oPair.Value)
            End If

        Next

        HttpRuntime.Cache(CacheName(sCacheName)) = oFilteredCarHireDepots

        Return oFilteredCarHireDepots
    End Function

    Public Function ListCarHireDepotsByCountryID(ByVal GeographyLevel1ID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "listCarDepotsByCountry_" & GeographyLevel1ID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2 get all the car hire depots and filter by geographylevel 1
        Dim oDepots As List(Of CarHireDepot) = Me.CarHireDepots.Where(
            Function(oCarHire) oCarHire.GeographyLevel1ID = GeographyLevel1ID).ToList()

        '3 add them to a keyvaluepairs collection
        Dim oDepotKeyValues As New KeyValuePairs()
        For Each Depot As CarHireDepot In oDepots
            oDepotKeyValues.Add(Depot.CarHireDepotID, Depot.DepotName)
        Next

        '4 cache it
        HttpRuntime.Cache(CacheName(sCacheName)) = oDepotKeyValues

        '5 return it
        Return oDepotKeyValues

    End Function

#End Region

#End Region

#Region "Airports and Flights"

#Region "Properties"

    'be aware - you will have duplicate airports in this list, made unique by different terminals
    Public ReadOnly Property Airports As List(Of Airport)
        Get
            Return Me.LookupCollection(Of Airport)(LookupCollectionIdentifier.Airports)
        End Get
    End Property

    Public ReadOnly Property TranslatedAirports As List(Of TranslatedAirport)
        Get
            Return Me.LookupCollection(Of TranslatedAirport)(LookupCollectionIdentifier.TranslatedAirports)
        End Get
    End Property

    Public ReadOnly Property AirportGroups As List(Of AirportGroup)
        Get
            Return Me.LookupCollection(Of AirportGroup)(LookupCollectionIdentifier.AirportGroups)
        End Get
    End Property

    Public ReadOnly Property TranslatedAirportGroups As List(Of TranslatedAirportGroup)
        Get
            Return Me.LookupCollection(Of TranslatedAirportGroup)(LookupCollectionIdentifier.TranslatedAirportGroups)
        End Get
    End Property

    Public ReadOnly Property DepartureAirports As List(Of Airport)
        Get
            Dim oDepartureAirports As New List(Of Airport)

            For Each oAirport As Airport In Me.Airports
                If oAirport.Type <> "Arrival Only" Then
                    oDepartureAirports.Add(oAirport)
                End If
            Next

            Return oDepartureAirports
        End Get
    End Property

    Public ReadOnly Property TranslatedDepartureAirports As List(Of TranslatedAirport)
        Get
            Return Me.TranslatedAirports.Where(Function(airport) airport.Type <> "Arrival Only").ToList
        End Get
    End Property

    Public ReadOnly Property ArrivalAirports As List(Of Airport)
        Get
            Dim oArrivalAirports As New List(Of Airport)

            For Each oAirport As Airport In Me.Airports

                'check if airport has a valid resort enabled for the brand
                Dim bBrandResort As Boolean = False
                Dim iAirportID As Integer = oAirport.AirportID
                For Each oAirportGeography As AirportGeography In Me.AirportGeographies.Where(Function(o) o.AirportID = iAirportID)

                    Dim iGeographyLevel3ID As Integer = oAirportGeography.GeographyLevel3ID

                    If Me.BrandGeographyLevel3.Where(Function(o) o.GeographyLevel3ID = iGeographyLevel3ID).Count > 0 Then
                        bBrandResort = True
                        Exit For
                    End If

                Next

                'if not departure only and has valid brand resort add to collection
                If oAirport.Type <> "Departure Only" AndAlso bBrandResort Then
                    oArrivalAirports.Add(oAirport)
                End If
            Next

            Return oArrivalAirports
        End Get
    End Property

    Public ReadOnly Property DepartureAirportGroups As List(Of AirportGroup)
        Get
            Dim oDepartureAirportGroups As New List(Of AirportGroup)

            For Each oAirportGroup As AirportGroup In Me.AirportGroups
                'if not departure only add to collection
                If oAirportGroup.Type = "Departure" Then
                    oDepartureAirportGroups.Add(oAirportGroup)
                End If
            Next

            Return oDepartureAirportGroups
        End Get
    End Property

    Public ReadOnly Property ArrivalAirportGroups As List(Of AirportGroup)
        Get
            Dim oArrivalAirportGroups As New List(Of AirportGroup)

            For Each oAirportGroup As AirportGroup In Me.AirportGroups
                'if not departure only add to collection
                If oAirportGroup.Type = "Arrival" Then
                    oArrivalAirportGroups.Add(oAirportGroup)
                End If
            Next

            Return oArrivalAirportGroups
        End Get
    End Property

    Public Function GetAirportNameFromAirportID(ByVal AirportID As Integer) As String
        Dim oAirport As Airport = Me.Airports.Where(Function(airport) airport.AirportID = AirportID).FirstOrDefault
        Return oAirport.Airport
    End Function

    Public Function GetAirportGroupNameFromAirportGroupID(ByVal AirportGroupID As Integer) As String
        Dim oAirportGroup As AirportGroup = Me.AirportGroups.Where(Function(airportgroup) airportgroup.AirportGroupID = AirportGroupID).FirstOrDefault
        Return oAirportGroup.AirportGroup
    End Function

    Public Function ListDepartureAirportsAndAirportGroupsBySellingGeographyLevel1ID(SellingGeographyLevel1ID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "ListDepartureAirportsAndAirportGroupsBySellingGeographyLevel1ID_" & SellingGeographyLevel1ID
        If Not HttpRuntime.Cache(Me.CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        Dim oAirports As New KeyValuePairs
        Dim oValidAirportGroupIDs As New List(Of Integer)
        Dim oFinalAirportList As New KeyValuePairs

        '1 get departure airport2
        For Each o As Airport In Me.GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID)
            If Not o.Type = "Arrival Only" AndAlso Not o.Airport.StartsWith("xx") Then
                oAirports.Add(o.AirportID, o.Airport)
            End If
        Next

        '2 get valid airport groups from airports already included
        For Each o As AirportGroupAirport In Me.AirportGroupAirports
            If oAirports.ContainsKey(o.AirportID) AndAlso Not oValidAirportGroupIDs.Contains(o.AirportID) Then
                oValidAirportGroupIDs.Add(o.AirportGroupID)
            End If
        Next

        '3 add airport groups
        For Each o As AirportGroup In Me.AirportGroups
            If oValidAirportGroupIDs.Contains(o.AirportGroupID) AndAlso o.Type = "Departure" AndAlso o.AirportGroup.StartsWith("All") AndAlso Not o.AirportGroup.StartsWith("xx") Then
                oFinalAirportList.Add(o.AirportGroupID + 1000000, o.AirportGroup)
            End If
        Next

        oFinalAirportList.Append(oAirports)

        AddToCache(CacheName(sCacheName), oFinalAirportList, 60)
        Return oFinalAirportList

    End Function

    Public Function ListArrivalAirportsByDeparture(ByVal DepartureAirportID As Integer, Optional ByVal IncludeAirportGroups As Boolean = False) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "ListArrivalAirportsByDeparture_" & DepartureAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2. get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)
        Dim oArrivalAirports As New List(Of Airport)

        For Each oAirport As Airport In Me.ArrivalAirports
            If oArrivalAirportIDs.Contains(oAirport.AirportID) Then
                oArrivalAirports.Add(oAirport)
            End If
        Next

        '2.5 Loop through the collection and append the IATA codes
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oArrivalAirports, "AirportID", "Airport")
        Dim oKeyValuePairsWithCode As New KeyValuePairs

        For Each oPair As KeyValuePair(Of Integer, String) In oKeyValuePairs

            Dim sIATACode As String = Me.GetKeyPairValue(LookupTypes.AirportIATACode, oPair.Key)
            oPair = New KeyValuePair(Of Integer, String)(oPair.Key, oPair.Value + String.Format(" ({0})", sIATACode))
            oKeyValuePairsWithCode.Add(oPair.Key, oPair.Value)

        Next

        '3. get list of arrival airport groups, based on departure airport
        If IncludeAirportGroups Then

            Dim oArrivalAirportGroupIDs As List(Of Integer) = Me.GetArrivalAirportGroupIDsByDeparture(DepartureAirportID)
            Dim oArrivalAirportGroups As New List(Of AirportGroup)

            For Each oAirportGroup As AirportGroup In Me.ArrivalAirportGroups
                If oArrivalAirportGroupIDs.Contains(oAirportGroup.AirportGroupID) Then
                    oArrivalAirportGroups.Add(oAirportGroup)
                End If
            Next

            oKeyValuePairsWithCode.Append(Me.GetIDValuePairs(oArrivalAirportGroups, "AirportGroupID", "AirportGroup"))

        End If

        '4. add to cache and return
        AddToCache(CacheName(sCacheName), oKeyValuePairsWithCode, 60)
        Return oKeyValuePairsWithCode

    End Function

    'Returns a Look up of Arrival Airports
    Public Function ListArrivalAirportsByDepartureAsLookup(ByVal DepartureAirportID As Integer) As List(Of Airport)

        '1. if in cache return
        Dim sCacheName As String = "ListArrivalAirportsByDepartureLookup_" & DepartureAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), List(Of Airport))
        End If

        '2. get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)
        Dim oArrivalAirports As New List(Of Airport)

        For Each oAirport As Airport In Me.ArrivalAirports
            If oArrivalAirportIDs.Contains(oAirport.AirportID) Then
                oArrivalAirports.Add(oAirport)
            End If
        Next

        Return oArrivalAirports

    End Function

    'Returns a Look up of Arrival Airport Groups
    Public Function ListArrivalAirportGroupsByDepartureAsLookup(ByVal DepartureAirportID As Integer) As List(Of AirportGroup)

        '1. if in cache return
        Dim sCacheName As String = "ListArrivalAirportGroupsByDepartureLookup_" & DepartureAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), List(Of AirportGroup))
        End If

        '2. get list of arrival airports, based on departure airport
        Dim oArrivalAirportGroupIDs As List(Of Integer) = Me.GetArrivalAirportGroupIDsByDeparture(DepartureAirportID)
        Dim oArrivalAirportGroups As New List(Of AirportGroup)

        For Each oAirportGroup As AirportGroup In Me.ArrivalAirportGroups
            If oArrivalAirportGroupIDs.Contains(oAirportGroup.AirportGroupID) Then
                oArrivalAirportGroups.Add(oAirportGroup)
            End If
        Next

        Return oArrivalAirportGroups

    End Function

    Public Function ListArrivalAirportsWithParentByDeparture(ByVal DepartureAirportID As Integer,
      Optional ByVal IncludeIATACode As Boolean = False,
      Optional ByVal IncludeAirportGroups As Boolean = False) As KeyValuePairsWithParent

        '1. if in cache return
        Dim sCacheName As String = "ListArrivalAirportsWithParentByDeparture_" & DepartureAirportID
        If Not HttpRuntime.Cache(Me.CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(Me.CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        '2. get list of arrival airports, based on departure airport
        Dim oArrivalAirportIDs As List(Of Integer) = Me.GetArrivalAirportIDsByDeparture(DepartureAirportID)
        Dim oArrivalAirports As New List(Of Airport)

        For Each oAirport As Airport In Me.ArrivalAirports
            If oArrivalAirportIDs.Contains(oAirport.AirportID) Then
                If IncludeIATACode Then
                    'if iata code not added already then add it to the name
                    Dim IATAString As String = String.Format(" ({0})", oAirport.IATACode)
                    If Not oAirport.Airport.Contains(IATAString) Then
                        oAirport.Airport = oAirport.Airport + IATAString
                    End If
                End If
                oArrivalAirports.Add(oAirport)
            End If
        Next

        '3. add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(oArrivalAirports, "AirportID", "Airport", "GeographyLevel1Name")

        '3. get list of arrival airport groups, based on departure airport
        If IncludeAirportGroups Then

            Dim oArrivalAirportGroupIDs As List(Of Integer) = Me.GetArrivalAirportGroupIDsByDeparture(DepartureAirportID)
            Dim oArrivalAirportGroups As New List(Of AirportGroup)

            For Each oAirportGroup As AirportGroup In Me.ArrivalAirportGroups
                If oArrivalAirportGroupIDs.Contains(oAirportGroup.AirportGroupID) Then
                    oArrivalAirportGroups.Add(oAirportGroup)
                End If
            Next

            oKeyValuePairs.Append(Me.GetIDValuePairsWithParent(oArrivalAirportGroups, "AirportGroupID", "AirportGroup", "GeographyLevel1Name", 1000000))

        End If

        '4. add to cache and return
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)

        Return oKeyValuePairs

    End Function

    Public Function ListDepartureAirportsWithParentByArrival(ByVal ArrivalAirportID As Integer, Optional ByVal IncludeIATACode As Boolean = False) As KeyValuePairsWithParent

        'if in cache return
        Dim sCacheName As String = "ListDepartureAirportsWithParentByArrival_" & ArrivalAirportID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        'get list of departure airports, based on arrival airport
        Dim oDepartureAirportIDs As List(Of Integer) = Me.GetDepartureAirportIDsByArrival(ArrivalAirportID)
        Dim oDepartureAirports As New List(Of Airport)

        For Each oAirport As Airport In Me.DepartureAirports
            If oDepartureAirportIDs.Contains(oAirport.AirportID) Or oAirport.AirportID = ArrivalAirportID Then
                If IncludeIATACode Then
                    'if iata code not added already then add it to the name
                    Dim IATAString As String = String.Format(" ({0})", oAirport.IATACode)
                    If Not oAirport.Airport.Contains(IATAString) Then
                        oAirport.Airport = oAirport.Airport + IATAString
                    End If
                End If
                oDepartureAirports.Add(oAirport)
            End If
        Next

        'add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(oDepartureAirports, "AirportID", "Airport", "GeographyLevel1Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)

        Return oKeyValuePairs

    End Function

    Public Function ListDepartureAirportsWithParent() As KeyValuePairsWithParent

        'if in cache return
        Dim sCacheName As String = "ListDepartureAirportsWithParent"
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        'add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(Me.DepartureAirports, "AirportID", "Airport", "GeographyLevel1Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)

        Return oKeyValuePairs

    End Function

    Public Function ListArrivalAirportsWithParent() As KeyValuePairsWithParent

        'if in cache return
        Dim sCacheName As String = "ListArrivalAirportsWithParent"
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairsWithParent)
        End If

        'add to cache and return
        Dim oKeyValuePairs As KeyValuePairsWithParent = Me.GetIDValuePairsWithParent(Me.ArrivalAirports, "AirportID", "Airport", "GeographyLevel1Name")
        AddToCache(CacheName(sCacheName), oKeyValuePairs, 60)

        Return oKeyValuePairs

    End Function

    Public Function ListDepartureAirportsByCountry(ByVal CountryID As Integer) As KeyValuePairs

        '1. if in cache return
        Dim sCacheName As String = "ListArrivalAirportsByCountry_" & CountryID
        If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
            Return CType(HttpRuntime.Cache(CacheName(sCacheName)), KeyValuePairs)
        End If

        '2. get list of arrival airports, based on Country
        Dim oArrivalAirports As New List(Of Airport)
        For Each oAirport As Airport In Me.Airports
            If oAirport.Type <> "Departure Only" AndAlso oAirport.GeographyLevel1ID = CountryID Then
                oArrivalAirports.Add(oAirport)
            End If
        Next

        '2.5 Loop through the collection and append the IATA codes
        Dim oKeyValuePairs As KeyValuePairs = Me.GetIDValuePairs(oArrivalAirports, "AirportID", "Airport")
        Dim oKeyValuePairsWithCode As New KeyValuePairs

        For Each oPair As KeyValuePair(Of Integer, String) In oKeyValuePairs

            Dim sIATACode As String = Me.GetKeyPairValue(LookupTypes.AirportIATACode, oPair.Key)
            oPair = New KeyValuePair(Of Integer, String)(oPair.Key, oPair.Value + String.Format(" ({0})", sIATACode))
            oKeyValuePairsWithCode.Add(oPair.Key, oPair.Value)

        Next

        '3. add to cache and return
        AddToCache(CacheName(sCacheName), oKeyValuePairsWithCode, 60)
        Return oKeyValuePairsWithCode

    End Function

    Public ReadOnly Property AirportGroupAirports As List(Of AirportGroupAirport)
        Get
            Return Me.LookupCollection(Of AirportGroupAirport)(LookupCollectionIdentifier.AirportGroupAirports)
        End Get
    End Property

    Public ReadOnly Property AirportGeographies As List(Of AirportGeography)
        Get
            Return Me.LookupCollection(Of AirportGeography)(LookupCollectionIdentifier.AirportGeographies)
        End Get
    End Property

    Public ReadOnly Property RouteAvailabilities As List(Of RouteAvailability)
        Get
            Return Me.LookupCollection(Of RouteAvailability)(LookupCollectionIdentifier.RouteAvailabilities)
        End Get
    End Property

    Public ReadOnly Property Vehicles As List(Of Vehicle)
        Get
            Return Me.LookupCollection(Of Vehicle)(LookupCollectionIdentifier.Vehicles)
        End Get
    End Property

    Public ReadOnly Property CreditCardTypes As List(Of CreditCardType)
        Get
            Dim oList As List(Of CreditCardType) = Me.LookupCollection(Of CreditCardType)(LookupCollectionIdentifier.CreditCardTypes)
            Dim oReturnList As New List(Of CreditCardType)

            For Each oCard As CreditCardType In oList
                If Params.SellingGeographyLevel1ID = oCard.SellingGeographyLevel1ID AndAlso
                 oCard.UseCreditCard Then
                    oReturnList.Add(oCard)
                End If
            Next

            Return oReturnList
        End Get
    End Property

    <XmlAttribute("Routes")>
    Public ReadOnly Property FlightSupplierRoutes As List(Of FlightSupplierRoute)
        Get
            Return Me.LookupCollection(Of FlightSupplierRoute)(LookupCollectionIdentifier.FlightSupplierRoutes)
        End Get
    End Property

#End Region

#Region "Functions"

    Public Function AirportResortCheck(ByVal AirportID As Integer, ByVal GeographyLevel3ID As Integer) As Boolean

        Dim bResult As Boolean = Me.AirportGeographies.Exists(Function(oAirportGeography) oAirportGeography.AirportID = AirportID _
           AndAlso oAirportGeography.GeographyLevel3ID = GeographyLevel3ID)
        Return bResult

    End Function

    Public Function GetAllArrivalAirportIDs() As List(Of Integer)

        Dim oArrivalAirports As List(Of Integer) = New List(Of Integer)
        For Each oRoute As Lookups.RouteAvailability In Me.RouteAvailabilities
            If Not oArrivalAirports.Contains(oRoute.ArrivalAirportID) Then
                oArrivalAirports.Add(oRoute.ArrivalAirportID)
            End If
        Next

        Return oArrivalAirports

    End Function

    Public Function GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID As Integer) As List(Of Airport)

        Return GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID, Me.Airports)

    End Function

    Public Function GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID As Integer, AirportLookups As List(Of Airport)) As _
	List(Of Airport)

		Dim oDepartureAirports As New List(Of Airport)

		For Each oAirport As Airport In AirportLookups
			If oAirport.GeographyLevel1ID = SellingGeographyLevel1ID Then
				'get distinct only- lookups contain multiple where there are multiple terminals
				Dim sAirportName As String = oAirport.Airport
				If oDepartureAirports.Where(Function(o) o.Airport = sAirportName).Count = 0 Then oDepartureAirports.Add(oAirport)
			End If
		Next

		Return oDepartureAirports

	End Function

	Public Function GetArrivalAirportIDsByDeparture(DepartureAirportID As Integer) As List(Of Integer)

		'deal with airport groups
		Dim oDepartureAirportIDs As New List(Of Integer)
		If DepartureAirportID < 1000000 Then
			oDepartureAirportIDs.Add(DepartureAirportID)
		Else
			For Each oAirportGroupAirport As AirportGroupAirport In Me.AirportGroupAirports.Where(Function(o) o.AirportGroupID = DepartureAirportID - 1000000)
				oDepartureAirportIDs.Add(oAirportGroupAirport.AirportID)
			Next
		End If

		Dim oValidArrivalAirportIDs As New List(Of Integer)
		For Each oRoute As RouteAvailability In Me.RouteAvailabilities
			If oDepartureAirportIDs.Contains(oRoute.DepartureAirportID) Then
				oValidArrivalAirportIDs.Add(oRoute.ArrivalAirportID)
			End If
		Next

		Return oValidArrivalAirportIDs
	End Function

	Public Function GetArrivalAirportGroupIDsByDeparture(DepartureAirportID As Integer) As List(Of Integer)

		Dim oValidArrivalAirportIDs As New List(Of Integer)
		oValidArrivalAirportIDs = GetArrivalAirportIDsByDeparture(DepartureAirportID)

		Dim oValidArrivalAirportGroupIDs As New List(Of Integer)
		For Each oArrivalAirportID As Integer In oValidArrivalAirportIDs
			Dim iArrivalAirportID As Integer = oArrivalAirportID
			For Each oAirportGroupAirport As AirportGroupAirport In Me.AirportGroupAirports.Where(Function(o) o.AirportID = iArrivalAirportID)
				If Not oValidArrivalAirportGroupIDs.Contains(oAirportGroupAirport.AirportGroupID) Then
					oValidArrivalAirportGroupIDs.Add(oAirportGroupAirport.AirportGroupID)
				End If
			Next
		Next

		Return oValidArrivalAirportGroupIDs

	End Function

	Public Function GetDepartureAirportIDsByArrival(ArrivalAirportID As Integer) As List(Of Integer)

		Dim oValidDepartureAirportIDs As New List(Of Integer)
		For Each oRoute As RouteAvailability In Me.RouteAvailabilities
			If oRoute.ArrivalAirportID = ArrivalAirportID Then
				oValidDepartureAirportIDs.Add(oRoute.DepartureAirportID)
			End If
		Next
		Return oValidDepartureAirportIDs
	End Function

	Public Function GetAirportGeographyLevel3IDs(AirportIDs As List(Of Integer)) As List(Of Integer)

		Dim oGeo3IDs As New List(Of Integer)

		'loop through our list of airports
		For Each iAirportID As Integer In AirportIDs
			'loop through lookups, get all the geo3s that match our airport list
			For Each oAirportGeography As AirportGeography In Me.AirportGeographies
				If oAirportGeography.AirportID = iAirportID Then
					If Not oGeo3IDs.Contains(oAirportGeography.GeographyLevel3ID) Then oGeo3IDs.Add(oAirportGeography.GeographyLevel3ID)
				End If
			Next
		Next

		Return oGeo3IDs

	End Function

	Public Function GetAirportGeographyLevel3IDs(AirportID As Integer) As List(Of Integer)

		Dim oGeo3IDs As New List(Of Integer)

		'loop through our list of airports
		'loop through lookups, get all the geo3s that match our airport list
		For Each oAirportGeography As AirportGeography In Me.AirportGeographies
			If oAirportGeography.AirportID = AirportID Then
				If Not oGeo3IDs.Contains(oAirportGeography.GeographyLevel3ID) Then oGeo3IDs.Add(oAirportGeography.GeographyLevel3ID)
			End If
		Next

		Return oGeo3IDs

	End Function

	Public Function GetGeographyLevel3AirportIDs(GeographyLevel3ID As Integer) As List(Of Integer)

		Dim oGeo3IDs As New List(Of Integer)
		oGeo3IDs.Add(GeographyLevel3ID)
		Return GetGeographyLevel3AirportIDs(oGeo3IDs)

	End Function

	Public Function GetGeographyLevel3AirportIDs(GeographyLevel3IDs As List(Of Integer)) As List(Of Integer)

		Dim oAirportIDs As New List(Of Integer)

		'loop through geo 3ds
		For Each iGeo3ID As Integer In GeographyLevel3IDs
			For Each oAirportGeography As AirportGeography In Me.AirportGeographies
				If oAirportGeography.GeographyLevel3ID = iGeo3ID Then
					If Not oAirportIDs.Contains(oAirportGeography.AirportID) Then oAirportIDs.Add(oAirportGeography.AirportID)
				End If
			Next
		Next

		Return oAirportIDs

	End Function

	Public Function AirportSearchLookupByDestinationID(DestinationID As Integer, SellingGeographyLevel1ID As Integer) As List(Of Airport)

		'1. if in cache return
		Dim sCacheName As String = "airportsearchlookupbydestination_" & DestinationID

		If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
			Return CType(HttpRuntime.Cache(CacheName(sCacheName)), List(Of Airport))
		End If

		'2 build up lookups
		Dim oLookups As New List(Of Airport)

		'is it -ve?
		'if so get  all airports that serve that geo3 and find which departure airports link
		If DestinationID < 0 Then
			Dim oArrivalAirportIDs As List(Of Integer) = GetGeographyLevel3AirportIDs(DestinationID * -1)
			Dim oDepartureAirportIDs As New List(Of Integer)
			For Each iArrivalAirportID As Integer In oArrivalAirportIDs
				oDepartureAirportIDs.AddRange(GetDepartureAirportIDsByArrival(iArrivalAirportID))
			Next
			For Each oAirport As Airport In Me.Airports
				If oDepartureAirportIDs.Contains(oAirport.AirportID) Then oLookups.Add(oAirport)
			Next
		Else
			'else it is +ve
			'so get all geo3s in this region, then get  all airports that serve that geo3 and find which departure airports link
			Dim oGeo3IDs As New List(Of Integer)
			For Each oLocation As Location In Me.Locations
				If oLocation.GeographyLevel2ID = DestinationID Then oGeo3IDs.Add(oLocation.GeographyLevel3ID)
			Next
			Dim oArrivalAirportIDs As List(Of Integer) = GetGeographyLevel3AirportIDs(oGeo3IDs)
			Dim oDepartureAirportIDs As New List(Of Integer)
			For Each iArrivalAirportID As Integer In oArrivalAirportIDs
				oDepartureAirportIDs.AddRange(GetDepartureAirportIDsByArrival(iArrivalAirportID))
			Next
			For Each oAirport As Airport In Me.Airports
				If oDepartureAirportIDs.Contains(oAirport.AirportID) Then oLookups.Add(oAirport)
			Next

		End If

		'3. add to cache and return
		oLookups = GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID, oLookups)
		AddToCache(CacheName(sCacheName), oLookups, 60)
		Return oLookups

	End Function

	'Search for airports that fly to a specified country
	Public Function AirportSearchLookupByCountryID(DestinationID As Integer, SellingGeographyLevel1ID As Integer) As List(Of Airport)

		'Return if cached
		Dim sCacheName As String = "airportsearchlookupbycountry_" & DestinationID
		If Not HttpRuntime.Cache(CacheName(sCacheName)) Is Nothing Then
			Return CType(HttpRuntime.Cache(CacheName(sCacheName)), List(Of Airport))
		End If

		'Create lookup
		Dim oLookups As New List(Of Airport)

		'Get list of GeographyLevel3IDs in the country
		Dim oGeo3IDs As New List(Of Integer)
		For Each oLocation As Location In Me.Locations
			If oLocation.GeographyLevel1ID = DestinationID Then oGeo3IDs.Add(oLocation.GeographyLevel3ID)
		Next

		'Generate list of arrival airports
		Dim oArrivalAirportIDs As List(Of Integer) = GetGeographyLevel3AirportIDs(oGeo3IDs)

		'Generate and populate list of departure airports ids based on arrival airports
		Dim oDepartureAirportIDs As New List(Of Integer)
		For Each iArrivalAirportID As Integer In oArrivalAirportIDs
			oDepartureAirportIDs.AddRange(GetDepartureAirportIDsByArrival(iArrivalAirportID))
		Next

		'Populate lookups with airports
		For Each oAirport As Airport In Me.Airports
			If oDepartureAirportIDs.Contains(oAirport.AirportID) Then oLookups.Add(oAirport)
		Next

		'Refine airports to just be ones within the SellingGeographyLevel1ID
		oLookups = GetDepartureAirportsBySellingGeographyID(SellingGeographyLevel1ID, oLookups)

		'Cache lookups
		AddToCache(CacheName(sCacheName), oLookups, 60)

		Return oLookups

	End Function

#End Region

#Region "Classes"

	Public Class Airport
		Public Airport As String
		Public AirportID As Integer
		Public IATACode As String
		Public Type As String
		Public OffsetDays As Integer
		Public GeographyLevel1ID As Integer
		Public GeographyLevel1Name As String
		Public Latitude As String
		Public Longitude As String
		Public PreferredAirport As Boolean
	End Class

	Public Class TranslatedAirport
		Public AirportID As Integer
		Public GeographyLevel1ID As Integer
		Public IATACode As String
		Public Latitude As String
		Public Longitude As String
		Public OffsetDays As Integer
		Public PreferredAirport As Boolean
		Public Translations As List(Of AirportTranslation)
		Public Type As String
	End Class

	Public Class AirportTranslation
		Public Airport As String
		Public GeographyLevel1Name As String
		Public Language As String
		Public LanguageID As Integer
	End Class

	Public Class AirportGeography
		Public AirportID As Integer
		Public GeographyLevel3ID As Integer
	End Class

	Public Class AirportGroup
		Public AirportGroupID As Integer
		Public AirportGroup As String
		Public Type As String
		Public GeographyLevel1ID As Integer
		Public GeographyLevel1Name As String
	End Class

	Public Class TranslatedAirportGroup
		Public AirportGroupID As Integer
		Public GeographyLevel1ID As Integer
		Public Translations As List(Of AirportGroupTranslation)
		Public Type As String
	End Class

	Public Class AirportGroupTranslation
		Public AirportGroup As String
		Public GeographyLevel1Name As String
		Public Language As String
		Public LanguageID As Integer
	End Class

	Public Class AirportGroupAirport
		Public AirportGroupAirportID As Integer
		Public AirportGroupID As Integer
		Public AirportID As Integer
	End Class

	Public Class RouteAvailability
		Public DepartureAirportID As Integer
		Public ArrivalAirportID As Integer
	End Class

	Public Class FlightCarrier
		Public FlightCarrierID As Integer
		Public FlightCarrier As String
		Public CarrierType As String
		Public Logo As String
	End Class

	Public Class Vehicle
		Public VehicleID As Integer
		Public VehicleName As String
	End Class

	Public Class CreditCardType
		Public CreditCardTypeID As Integer
		Public CreditCardType As String
		Public UseCreditCard As Boolean
		Public SellingGeographyLevel1ID As Integer
		Public SurchargePercentage As Decimal
	End Class

#End Region

#End Region

#Region "Facilities, Product Attributes"

	Public ReadOnly Property FilterFacilities As List(Of FilterFacility)
		Get
			Return Me.LookupCollection(Of FilterFacility)(LookupCollectionIdentifier.FilterFacilities)
		End Get
	End Property

	Public Class FilterFacility
		Public FilterFacilityID As Integer
		Public FilterFacility As String
		Public Priority As Integer
	End Class

	Public ReadOnly Property ProductAttributes As List(Of ProductAttribute)
		Get
			Return Me.LookupCollection(Of ProductAttribute)(LookupCollectionIdentifier.ProductAttributes)
		End Get
	End Property

	Public Class ProductAttribute
		Public ProductAttriubteTypeID As Integer
		Public ProductAttributeType As String
		Public ProductAttributeGroupID As Integer
		Public ProductAttributeGroup As String
		Public ProductAttributeID As Integer
		Public ProductAttribute As String
	End Class

#End Region

#Region "Port"

#Region "Properties"
	Public ReadOnly Property Ports As List(Of Port)
		Get
			Return Me.LookupCollection(Of Port)(LookupCollectionIdentifier.Ports)
		End Get
	End Property
#End Region

#Region "Functions"

	Public Function ListPortsInGeographyLevel3(ByVal iGeographyLevel3ID As Integer) As List(Of Port)
		Dim oPortsInGeo3 As New List(Of Port)

		For Each oPort As Port In Me.Ports
			If oPort.GeographyLevel3ID = iGeographyLevel3ID Then
				oPortsInGeo3.Add(oPort)
			End If
		Next

		Return oPortsInGeo3
	End Function

#End Region

#Region "classes"
	Public Class Port
		Public PortID As Integer
		Public PortName As String
		Public GeographyLevel3ID As Integer
	End Class
#End Region

#End Region

#Region "Station"

#Region "Properties"
	Public ReadOnly Property Stations As List(Of Station)
		Get
			Return Me.LookupCollection(Of Station)(LookupCollectionIdentifier.Stations)
		End Get
	End Property
#End Region

#Region "Functions"

	Public Function ListStationsInGeographyLevel3(ByVal iGeographyLevel3ID As Integer) As List(Of Station)
		Dim oStationsInGeo3 As New List(Of Station)

		For Each oStation As Station In Me.Stations
			If oStation.GeographyLevel3ID = iGeographyLevel3ID Then
				oStationsInGeo3.Add(oStation)
			End If
		Next

		Return oStationsInGeo3
	End Function

#End Region

#Region "Classes"
	Public Class Station
		Public StationID As Integer
		Public StationName As String
		Public GeographyLevel3ID As Integer
	End Class
#End Region

#End Region

#Region "IP Address"

	Public Function ClientIPAddress() As String 'decided these would be better as functions rather than properties so not evaluated with lookups object as whole
#If DEBUG Then
		Return "82.108.7.146"
#End If
		Return HttpContext.Current.Request.UserHostAddress
	End Function

	Public Function ClientCountryCode() As String
		Dim sClientCountryCode As String = ""

		Try
			'get ip details from our ip lookup service
			Dim sIPLookupResponse As String = WebRequests.GetResponse("http://iplookup.ivector.co.uk/?ip=" & Me.ClientIPAddress, "", , "", , , , , , 60)
			Dim oIPLookupResponseXML As New XmlDocument
			oIPLookupResponseXML.LoadXml(sIPLookupResponse)

			If XMLFunctions.SafeNodeValue(oIPLookupResponseXML, "//ReturnStatus/Success").ToSafeBoolean Then
				sClientCountryCode = XMLFunctions.SafeNodeValue(oIPLookupResponseXML, "//IPAddressResponse/GeoIPLocation/CountryCode")
			End If
		Catch ex As Exception
		End Try

		Return sClientCountryCode
	End Function

#End Region

#Region "Supplier"
    Public ReadOnly Property Suppliers As List(Of Supplier)
        Get
            Return Me.LookupCollection(Of Supplier)(LookupCollectionIdentifier.Suppliers)
        End Get
    End Property
#End Region

#Region "Generate Key Value Pairs"

    Public Function GenerateKeyValuePairs(ByVal Lookup As LookupTypes) As KeyValuePairs

		Dim oPairs As KeyValuePairs = New KeyValuePairs

		'airports
		If Lookup = LookupTypes.Airport Then
			oPairs = Me.GetIDValuePairs("Airport", "AirportID", "Airport")
		End If

		'airport group
		If Lookup = LookupTypes.AirportGroup Then
			oPairs = Me.GetIDValuePairs(Me.AirportGroups, "AirportGroupID", "AirportGroup", 1000000)
		End If

		'airportgroup and airports
		If Lookup = LookupTypes.AirportGroupAndAirport Then
			oPairs = Me.GetIDValuePairs(Me.DepartureAirportGroups, "AirportGroupID", "AirportGroup", 1000000)
			If oPairs.Count > 0 Then oPairs.Add(0, "-----------------------")
			oPairs.Append(Me.GetIDValuePairs(Me.DepartureAirports, "AirportID", "Airport"))
		End If

	    If Lookup = LookupTypes.AllAirportGroupAndAirport Then
	        oPairs = Me.GetIDValuePairs(Me.AirportGroups, "AirportGroupID", "AirportGroup", 1000000)
	        If oPairs.Count > 0 Then oPairs.Add(0, "-----------------------")
	        oPairs.Append(Me.GetIDValuePairs(Me.Airports, "AirportID", "Airport"))
	    End If

		'airports iata code
		If Lookup = LookupTypes.AirportIATACode Then
			oPairs = Me.GetIDValuePairs("Airport", "AirportID", "IATACode")
		End If

		'arrival airports
		If Lookup = LookupTypes.ArrivalAirport Then
			oPairs = Me.GetIDValuePairs(Me.ArrivalAirports, "AirportID", "Airport")
		End If

		'arrival airport group
		If Lookup = LookupTypes.ArrivalAirportGroup Then
			oPairs = Me.GetIDValuePairs(Me.AirportGroups, "AirportGroupID", "AirportGroup", 1000000)
		End If

		'arrival airport group and airports
		If Lookup = LookupTypes.ArrivalAirportGroupAndAirport Then
			oPairs = Me.GetIDValuePairs(Me.ArrivalAirportGroups, "AirportGroupID", "AirportGroup", 1000000)
			If oPairs.Count > 0 Then oPairs.Add(0, "-----------------------")
			oPairs.Append(Me.GetIDValuePairs(Me.DepartureAirports, "AirportID", "Airport"))
		End If

		'mealbasis
		If Lookup = LookupTypes.MealBasis Then
			oPairs = Me.GetIDValuePairs("MealBasis", "MealBasisID", "MealBasis")
		End If

		'currency symbol
		If Lookup = LookupTypes.CurrencySymbol Then
			oPairs = Me.GetIDValuePairs("Currency", "CurrencyID", "Symbol", 0, "CustomerSymbolOverride")
		End If

		'currency code
		If Lookup = LookupTypes.CurrencyCode Then
			oPairs = Me.GetIDValuePairs("Currency", "CurrencyID", "CurrencyCode")
		End If

		'currency symbol position
		If Lookup = LookupTypes.CurrencySymbolPosition Then
			oPairs = Me.GetIDValuePairs("Currency", "CurrencyID", "SymbolPosition")
		End If

		'selling currency id
		If Lookup = LookupTypes.SellingCurrencyID Then
			oPairs = Me.GetIDValuePairs("Currency", "CurrencyID", "SellingCurrencyID")
		End If

		'selling exchange rates
		If Lookup = LookupTypes.SellingExchangeRate Then
			oPairs = Me.GetIDValuePairs("SellingExchangeRate", "CurrencyID", "ExchangeRate")
		End If

		'geography grouping
		If Lookup = LookupTypes.GeographyGrouping Then
			oPairs = Me.GetIDValuePairs("GeographyGrouping", "GeographyGroupingID", "GeographyGroup")
		End If

		'country
		If Lookup = LookupTypes.Country Then
			oPairs = Me.GetIDValuePairs("Location", "GeographyLevel1ID", "GeographyLevel1Name")
		End If

		'region
		If Lookup = LookupTypes.Region Then
			oPairs = Me.GetIDValuePairs("Location", "GeographyLevel2ID", "GeographyLevel2Name")
		End If

		'resort
		If Lookup = LookupTypes.Resort Then
			oPairs = Me.GetIDValuePairs("Location", "GeographyLevel3ID", "GeographyLevel3Name")
		End If

		'BookingCountry
		If Lookup = LookupTypes.BookingCountry Then
			oPairs = Me.GetIDValuePairs("BookingCountry", "BookingCountryID", "BookingCountry")
		End If

		'Booking Description
		If Lookup = LookupTypes.BookingDescription Then
			oPairs = Me.GetIDValuePairs("BookingDescription", "BookingDescriptionID", "Description")
		End If

		'Booking Documentation
		If Lookup = LookupTypes.BookingDocumentation Then
			oPairs = Me.GetIDValuePairs("BookingDocumentation", "BookingDocumentationID", "DocumentName")
		End If

		'culture code
		If Lookup = LookupTypes.CultureCode Then
			oPairs = Me.GetIDValuePairs("Language", "LanguageID", "CultureCode")
		End If

		'language
		If Lookup = LookupTypes.Language Then
			oPairs = Me.GetIDValuePairs("Language", "LanguageID", "Language")
		End If

		'language code
		If Lookup = LookupTypes.LanguageCode Then
			oPairs = Me.GetIDValuePairs("Language", "LanguageID", "LanguageCode")
		End If

		'flight carrier
		If Lookup = LookupTypes.FlightCarrier Then
			oPairs = Me.GetIDValuePairs("FlightCarrier", "FlightCarrierID", "FlightCarrier")
		End If

		'flight carrier
		If Lookup = LookupTypes.FlightCarrierType Then
			oPairs = Me.GetIDValuePairs("FlightCarrier", "FlightCarrierID", "CarrierType")
		End If

		'flight carrier logo
		If Lookup = LookupTypes.FlightCarrierLogo Then
			oPairs = Me.GetIDValuePairs("FlightCarrier", "FlightCarrierID", "Logo")
		End If

		'vehicle
		If Lookup = LookupTypes.Vehicle Then
			oPairs = Me.GetIDValuePairs("Vehicle", "VehicleID", "VehicleName")
		End If

		'flight class
		If Lookup = LookupTypes.FlightClass Then
			oPairs = Me.GetIDValuePairs("FlightClass", "FlightClassID", "FlightClass")
		End If

		'booking component
		If Lookup = LookupTypes.BookingComponent Then
			oPairs = Me.GetIDValuePairs("BookingComponent", "BookingComponentID", "BookingComponent")
		End If

		'extra type
		If Lookup = LookupTypes.ExtraType Then
			oPairs = Me.GetIDValuePairs("ExtraType", "ExtraTypeID", "ExtraType")
		End If

		'extra category
		If Lookup = LookupTypes.ExtraCategory Then
			oPairs = Me.GetIDValuePairs("ExtraCategory", "ExtraCategoryID", "ExtraCategory")
		End If

		'extra duration
		If Lookup = LookupTypes.ExtraDuration Then
			oPairs = Me.GetIDValuePairs("ExtraDuration", "ExtraDurationID", "ExtraDuration")
		End If

		'credit card types
		If Lookup = LookupTypes.CardType Then
			oPairs = Me.GetIDValuePairs("CardType", "CreditCardTypeID", "CreditCardType")
		End If

		'credit card types
		If Lookup = LookupTypes.CardSurcharge Then
			oPairs = Me.GetIDValuePairs("CreditCardType", "CreditCardTypeID", "SurchargePercentage")
		End If

		'car hire depot
		If Lookup = LookupTypes.CarHireDepot Then
			oPairs = Me.GetIDValuePairs("CarHireDepot", "CarHireDepotID", "DepotName")
		End If

		If Lookup = LookupTypes.CarHireDepotGeographyLevel1 Then
			oPairs = Me.GetIDValuePairs("CarHireDepotGeographyLevel1", "GeographyLevel1ID", "CarHireDepotIDs")
		End If

		'car hire depot geographyLevel3
		If Lookup = LookupTypes.CarHireDepotGeographyLevel3 Then
			oPairs = Me.GetIDValuePairs("CarHireDepotGeographyLevel3", "GeographyLevel3ID", "CarHireDepotIDs")
		End If

		'room views
		If Lookup = LookupTypes.RoomView Then
			oPairs = Me.GetIDValuePairs("RoomView", "RoomViewID", "RoomView")
		End If

		'product attribute
		If Lookup = LookupTypes.ProductAttribute Then
			oPairs = Me.GetIDValuePairs("ProductAttribute", "ProductAttributeID", "ProductAttribute")
		End If

		'property type
		If Lookup = LookupTypes.PropertyType Then
			oPairs = Me.GetIDValuePairs("PropertyType", "PropertyTypeID", "PropertyType")
		End If

		'port
		If Lookup = LookupTypes.Port Then
			oPairs = Me.GetIDValuePairs("Port", "PortID", "PortName")
		End If

		'station
		If Lookup = LookupTypes.Station Then
			oPairs = Me.GetIDValuePairs("Station", "StationID", "StationName")
		End If

		'sort by value
		Dim oList As New List(Of KeyValuePair(Of Integer, String))
		For Each oPair As KeyValuePair(Of Integer, String) In oPairs
			oList.Add(oPair)
		Next
		oList = oList.OrderBy(Function(o) o.Value).ToList()

		Dim oSortedPairs As New KeyValuePairs
		For Each oListItem As KeyValuePair(Of Integer, String) In oList
			oSortedPairs.Add(oListItem.Key, oListItem.Value)
		Next

		'return
		Return oSortedPairs

	End Function

	Public Function GenerateKeyValuePairsWithParent(ByVal Lookup As LookupTypes) As KeyValuePairsWithParent

		Dim oPairs As KeyValuePairsWithParent = Nothing

		'country airport
		If Lookup = LookupTypes.CountryAirport Then

			oPairs = Me.GetIDValuePairsWithParent(Me.DepartureAirports, "AirportID", "Airport", "GeographyLevel1Name")

			For Each oPair As KeyValuePairWithParent In oPairs
				Dim sIATACode As String = Me.GetKeyPairValue(LookupTypes.AirportIATACode, oPair.ID)
				oPair.Value = oPair.Value + String.Format(" ({0})", sIATACode)
			Next

		End If

		'country arrival airport
		If Lookup = LookupTypes.CountryArrivalAirport Then
			oPairs = Me.GetIDValuePairsWithParent(Me.ArrivalAirports, "AirportID", "Airport", "GeographyLevel1Name")

			For Each oPair As KeyValuePairWithParent In oPairs
				Dim sIATACode As String = Me.GetKeyPairValue(LookupTypes.AirportIATACode, oPair.ID)
				oPair.Value = oPair.Value + String.Format(" ({0})", sIATACode)
			Next

		End If

		'country arrival airport group and airport
		If Lookup = LookupTypes.CountryArrivalAirportGroupAndAirport Then

			oPairs = Me.GetIDValuePairsWithParent(Me.ArrivalAirportGroups, "AirportGroupID", "AirportGroup", "GeographyLevel1Name", 1000000)

			Dim oTempPairs As KeyValuePairsWithParent = Nothing
			oTempPairs = Me.GetIDValuePairsWithParent(Me.ArrivalAirports, "AirportID", "Airport", "GeographyLevel1Name")

			For Each oTempPair As KeyValuePairWithParent In oTempPairs
				Dim sIATACode As String = Me.GetKeyPairValue(LookupTypes.AirportIATACode, oTempPair.ID)
				oTempPair.Value = oTempPair.Value + String.Format(" ({0})", sIATACode)
			Next

			oPairs.Append(oTempPairs)

		End If

		'country and region
		If Lookup = LookupTypes.CountryRegion Then
			oPairs = Me.GetIDValuePairsWithParent(Me.Locations, "GeographyLevel2ID", "GeographyLevel2Name", "GeographyLevel1Name")
		End If

		'sort by parent
		oPairs.Sort(New KeyValuePairsSortByParent)

		Return oPairs

	End Function

#End Region

#Region "Key Pair Value/ID"

	Public Function GetKeyPairValue(ByVal Lookup As LookupTypes, ByVal ID As Integer) As String

		'cache ones we actually use
		'we cache all of the lookup xml but then we still have to query the in memory xml each time to just get values we use
		'for large clients we only use a small amount of key value pairs compared to the amount of lookup options we have
		Dim sKey As String = "__lookups_keypairsvalue_" & Lookup.ToString & "_" & ID
		If Not HttpContext.Current Is Nothing Then
			sKey += "_" + BookingBase.DisplayLanguageID.ToString
		End If

		Dim oPairs As New KeyValuePairs
		Dim sValue As String = ""

		If Not HttpRuntime.Cache(sKey) Is Nothing Then
			Return CType(HttpRuntime.Cache(sKey), String)
		Else
			oPairs = Me.GenerateKeyValuePairs(Lookup)
			If oPairs.ContainsKey(ID) Then
				sValue = oPairs.Item(ID)
			End If
		End If

		AddToCache(sKey, sValue, 60)

		Return sValue

	End Function

	Public Function GetKeyPairID(ByVal Lookup As LookupTypes, ByVal Value As String) As Integer

		'get pairs
		Dim oPairs As KeyValuePairs = Me.GenerateKeyValuePairs(Lookup)

		'loop through and return key if value matches
		For Each oPair As KeyValuePair(Of Integer, String) In oPairs
			If oPair.Value = Value Then Return oPair.Key
		Next

		'no match found return 0
		Return 0

	End Function

	Public Function GetKeyPairParent(ByVal Lookup As LookupTypes, ByVal ID As Integer) As String
		Dim oPairs As KeyValuePairsWithParent = Me.GenerateKeyValuePairsWithParent(Lookup)
		For Each oPair As KeyValuePairWithParent In oPairs
			If oPair.ID = ID Then Return oPair.Parent
		Next
		Return ""
	End Function

#End Region

#Region "Support Classes and Enums"

	Public Enum LookupCollectionIdentifier
		CarHireDepots
		CarTypes
		Locations
		BookingCountryRegions
		Airports
		AirportGroups
		AirportGroupAirports
		AirportGeographies
		RouteAvailabilities
		FilterFacilities
		ProductAttributes
		Ports
		Vehicles
        CreditCardTypes
        FlightSupplierRoutes
        Suppliers
        GeographyGroupings
		GeographyGroupingGeographies
		Stations
		TranslatedAirports
		TranslatedAirportGroups
		TranslatedGeographyGroupings
		TranslatedLocations
	End Enum

	Public Function LookupCollectionXPath(Identifier As LookupCollectionIdentifier) As String
		Return String.Format("/Lookups/{0}", Identifier.ToString)
	End Function

	Public Enum LookupTypes
		None
		Airport
		AirportGroup
		AirportGroupAndAirport
        AllAirportGroupAndAirport
		AirportIATACode
		ArrivalAirport
		ArrivalAirportGroup
		ArrivalAirportGroupAndAirport
		BookingComponent
		BookingCountry
		BookingCountryRegion
		BookingDescription
		BookingDocumentation
		CarHireDepot
		CarHireDepotGeographyLevel1
		CarHireDepotGeographyLevel3
		CardType
		CardSurcharge
		Country
		CountryAirport
		CountryArrivalAirport
		CountryAirportGroupAndAirport
		CountryArrivalAirportGroupAndAirport
		CountryRegion
		CultureCode
		CurrencyCode
		CurrencySymbol
		CurrencySymbolPosition
		Erratum
		Extra
		ExtraGroup
		ExtraType
		ExtraCategory
		ExtraDuration
		FlightCarrier
		FlightCarrierLogo
		FlightCarrierType
		FlightClass
		GeographyGrouping
        GeographyGroupingGeographies
        FlightSupplierRoute
        Language
		LanguageCode
		MealBasis
		Port
		ProductAttribute
		PropertyType
		Region
		Resort
		RoomView
		SellingCurrencyID
		SellingExchangeRate
		Station
		TradeName
		Vehicle
	End Enum

	Public Class KeyValuePairs
		Inherits Generic.Dictionary(Of Integer, String)

		Public Sub Append(ByVal KeyValuePairs As KeyValuePairs)
			For Each oPair As Generic.KeyValuePair(Of Integer, String) In KeyValuePairs
				Me.Add(oPair.Key, oPair.Value)
			Next
		End Sub

	End Class

	Public Class KeyValuePairsWithParent
		Inherits Generic.List(Of KeyValuePairWithParent)

		Public Sub Append(ByVal KeyValuePairsWithParent As KeyValuePairsWithParent)
			For Each oPair As KeyValuePairWithParent In KeyValuePairsWithParent
				Me.Add(oPair)
			Next
		End Sub

	End Class

	Public Class KeyValuePairWithParent
		Public Property ID As Integer
		Public Property Value As String
		Public Property Parent As String

		Public Sub New()
		End Sub

		Public Sub New(ByVal ID As Integer, ByVal Value As String, ByVal Parent As String)
			Me.ID = ID
			Me.Value = Value
			Me.Parent = Parent
		End Sub

	End Class

    Public Class KeyValuePairsSortByParent
        Implements IComparer(Of KeyValuePairWithParent)

        Overloads Function Compare(ByVal x As KeyValuePairWithParent, ByVal y As KeyValuePairWithParent) As Integer Implements IComparer(Of KeyValuePairWithParent).Compare

            If x.Parent > y.Parent Then
                Return 1
            ElseIf x.Parent < y.Parent Then
                Return -1
            ElseIf x.Value > y.Value Then
                Return 1
            ElseIf x.Value < y.Value Then
                Return -1
            Else
                Return 0
            End If

        End Function

    End Class

    <XmlRoot("Route")>
    Public Class FlightSupplierRoute
        Public Property ArrivalAirportID As Integer
        Public Property DepartureAirportID As Integer
        <XmlArray("Suppliers")>
        <XmlArrayItem("FlightSupplier")>
        Public Property Suppliers As List(Of FlightSupplier)
    End Class
    Public Class FlightSupplier
        Public Property FlightSupplierID As Integer
        Public Property FlightSupplierName As String
        Public Property SupplierID As Integer
    End Class
    Public Class Supplier
        Public Property SupplierID As Integer
        Public Property SupplierName As String
    End Class
#End Region

#Region "Support Functions"

    Public Function GetLookupXML(ByVal XPath As String, Type As String) As XmlDocument

		Dim oReturnXML As New XmlDocument

		Dim sXML As String = XMLFunctions.SafeOuterXML(Me.LookupXML(Type), XPath)
		If sXML <> "" Then oReturnXML.LoadXml(sXML)

		Return oReturnXML

	End Function

	Public Function LookupCollection(Of T)(ByVal Identifier As LookupCollectionIdentifier) As List(Of T)

		Dim sIdentifier As String = Identifier.ToString
		Dim XPath As String = LookupCollectionXPath(Identifier)

		'1. if in cache return
		If Not HttpRuntime.Cache(CacheName(sIdentifier)) Is Nothing Then
			Return CType(HttpRuntime.Cache(CacheName(sIdentifier)), List(Of T))
		End If

		'2. build list
		Dim oXML As XmlDocument = Me.GetLookupXML(XPath, sIdentifier)
		Dim oList As List(Of T) = Utility.XMLToGenericList(Of T)(oXML)

		'3. add to cache and return
		AddToCache(CacheName(sIdentifier), oList, 60)
		Return oList
	End Function

	<Obsolete("You should be passing in a LookupCollectionIdentifier enum instead of a string")>
	Public Function LookupCollection(Of T)(ByVal Identifier As String, ByVal XPath As String) As List(Of T)

		'1. if in cache return
		If Not HttpRuntime.Cache(CacheName(Identifier)) Is Nothing Then
			Return CType(HttpRuntime.Cache(CacheName(Identifier)), List(Of T))
		End If

		'2. build list
		Dim oXML As XmlDocument = Me.GetLookupXML(XPath, Identifier)
		Dim oList As List(Of T) = Utility.XMLToGenericList(Of T)(oXML)

		'3. add to cache and return
		AddToCache(CacheName(Identifier), oList, 60)
		Return oList

	End Function

	Public Function NameLookup(Type As LookupTypes, ByVal TextElement As String, ByVal ValueElement As String, ByVal ID As Integer) As String

		Dim sXPath As String = Me.GetLookupPath(Type.ToString)

		For Each oNode As XmlNode In Me.LookupXML(Type.ToString, True).SelectNodes(sXPath)
			If XMLFunctions.SafeNodeValue(oNode, ValueElement) = ID.ToString Then
				Return XMLFunctions.SafeNodeValue(oNode, TextElement)
			End If
		Next

		Return ""

	End Function

	<Obsolete("You should be calling NameLookup with the Lookup Type enum instead of a string")>
	Public Function NameLookup(ByVal NodeXPath As String, ByVal TextElement As String, ByVal ValueElement As String, ByVal ID As Integer) As String

		For Each oNode As XmlNode In Me.LookupXML(TextElement, True).SelectNodes(NodeXPath)
			If XMLFunctions.SafeNodeValue(oNode, ValueElement) = ID.ToString Then
				Return XMLFunctions.SafeNodeValue(oNode, TextElement)
			End If
		Next

		Return ""

	End Function

	Public Function IDLookup(Type As LookupTypes, ByVal TextElement As String, ByVal ValueElement As String, ByVal Name As String) As Integer

		Dim sXPath As String = Me.GetLookupPath(Type.ToString)

		For Each oNode As XmlNode In Me.LookupXML(Type.ToString, True).SelectNodes(sXPath)
			If XMLFunctions.SafeNodeValue(oNode, TextElement) = Name Then
				Return XMLFunctions.SafeNodeValue(oNode, ValueElement).ToSafeInt
			End If
		Next

		Return 0

	End Function

	<Obsolete("You should be calling IDLookup with the Lookup Type enum instead of a string")>
	Public Function IDLookup(ByVal NodeXPath As String, ByVal TextElement As String, ByVal ValueElement As String, ByVal Name As String) As Integer

		For Each oNode As XmlNode In Me.LookupXML(TextElement, True).SelectNodes(NodeXPath)
			If XMLFunctions.SafeNodeValue(oNode, TextElement) = Name Then
				Return XMLFunctions.SafeNodeValue(oNode, ValueElement).ToSafeInt
			End If
		Next

		Return 0

	End Function

	Private Function GetLookupPath(Type As String) As String

		'TODO - check if lookups currency can be changed to currencies instead of currencys
		Dim sXPath As String = ""
		If Type.EndsWith("y") AndAlso Not Type = "Currency" Then
			sXPath = "/Lookups/" & Type.Replace("y", "") & "ies/" & Type
		ElseIf Type = "MealBasis" Then
			sXPath = "/Lookups/MealBases/MealBasis"
		ElseIf Type.EndsWith("ss") Then
			sXPath = "/Lookups/" & Type & "es/" & Type
		Else
			sXPath = "/Lookups/" & Type & "s/" & Type
		End If

		Return sXPath

	End Function

	Public Function GetIDValuePairs(ByVal Type As String, ByVal IDElement As String,
	ByVal ValueElement As String, Optional OffsetKeyBy As Integer = 0, Optional OverrideElement As String = "") As KeyValuePairs

		'get the relevant nodes
		Dim sXPath As String = Me.GetLookupPath(Type)

		Dim oNodeList As XmlNodeList = Me.LookupXML(Type, True).SelectNodes(sXPath)

		'scan and build list
		Dim aResult As New KeyValuePairs
		Dim iID As Integer
		Dim sValue As String
		For Each oNode As XmlNode In oNodeList
			iID = SafeInt(oNode.SelectSingleNode(IDElement).InnerText) + OffsetKeyBy
			sValue = ""

			If OverrideElement <> "" AndAlso Not oNode.SelectSingleNode(OverrideElement) Is Nothing AndAlso oNode.SelectSingleNode(OverrideElement).InnerText <> "" Then
				sValue = oNode.SelectSingleNode(OverrideElement).InnerText
			ElseIf Not oNode.SelectSingleNode(ValueElement) Is Nothing Then
				sValue = oNode.SelectSingleNode(ValueElement).InnerText
			End If

			If Not aResult.ContainsKey(iID) Then aResult.Add(iID, sValue)
		Next

		Return aResult

	End Function

	Public Function GetIDValuePairs(Of T)(ByVal oList As List(Of T), ByVal IDElement As String,
	   ByVal ValueElement As String, Optional OffsetKeyBy As Integer = 0) As KeyValuePairs

		'loop through list and build key value pairs
		Dim aResult As New KeyValuePairs
		Dim oFields As FieldInfo() = GetType(T).GetFields()

		For Each oItem As T In oList
			Dim iID As Integer = SafeInt(oFields.Where(Function(o) o.Name = IDElement).First.GetValue(oItem)) + OffsetKeyBy
			Dim sValue As String = SafeString(oFields.Where(Function(o) o.Name = ValueElement).First.GetValue(oItem))
			If Not aResult.ContainsKey(iID) Then aResult.Add(iID, sValue)
		Next

		Return aResult

	End Function

	Public Function GetIDValuePairsWithParent(Of T)(ByVal oList As List(Of T), ByVal IDElement As String,
	  ByVal ValueElement As String, ByVal ParentElement As String, Optional OffsetKeyBy As Integer = 0) As KeyValuePairsWithParent

		'loop through list and build key value pairs
		Dim oResult As New KeyValuePairsWithParent
		Dim oFields As FieldInfo() = GetType(T).GetFields()

		For Each oItem As T In oList
			Dim iID As Integer = SafeInt(oFields.Where(Function(o) o.Name = IDElement).First.GetValue(oItem)) + OffsetKeyBy
			Dim sValue As String = SafeString(oFields.Where(Function(o) o.Name = ValueElement).First.GetValue(oItem))
			Dim sParent As String = SafeString(oFields.Where(Function(o) o.Name = ParentElement).First.GetValue(oItem))

			Dim oKeyValuePairWithParent As New KeyValuePairWithParent(iID, sValue, sParent)
			If oResult.Where(Function(o) o.ID = oKeyValuePairWithParent.ID).Count = 0 Then oResult.Add(oKeyValuePairWithParent)
		Next

		'sort by parent
		oResult.Sort(New KeyValuePairsSortByParent)

		'return
		Return oResult

	End Function

	Private Function CacheName(ByVal sCacheName As String) As String
		Dim iLanguageID As Integer = Params.LanguageID
		If HttpContext.Current IsNot Nothing Then
			iLanguageID = BookingBase.DisplayLanguageID
		End If

		sCacheName = sCacheName & iLanguageID
		Return sCacheName
	End Function

#End Region

End Class
Imports System.Xml
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.ComponentModel
Imports Newtonsoft.Json

Public Class Extras
    Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As Dictionary(Of String, CustomSetting)
		Get
			If HttpContext.Current.Session("extras_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("extras_customsettings"), Dictionary(Of String, CustomSetting))
			End If
			HttpContext.Current.Session("extras_customsettings") = New Dictionary(Of String, CustomSetting)
			Return CType(HttpContext.Current.Session("extras_customsettings"), Dictionary(Of String, CustomSetting))
		End Get
		Set(value As Dictionary(Of String, CustomSetting))
			HttpContext.Current.Session("extras_customsettings") = value
		End Set
	End Property

#End Region

#Region "Draw, Params & Update"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

        '1. Set up custom settings
        Dim sIdentifier As String = Settings.GetValue("Identifier")
        Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .Identifier = sIdentifier
            .BasketType = Functions.SafeEnum(Of eBasketType)(GetSetting(eSetting.BasketType))
            .Title = Functions.SafeString(GetSetting(eSetting.Title))
            .Extra = Functions.SafeString(GetSetting(eSetting.Extra))
            .ExtraGroup = Functions.SafeString(GetSetting(eSetting.ExtraGroup))
            .ExtraTypes = GetSetting(eSetting.ExtraTypes).Split("|"c).ToList
            .DisplayType = Functions.SafeEnum(Of eDisplayType)(GetSetting(eSetting.DisplayType))
            .LazyLoad = Functions.SafeBoolean(GetSetting(eSetting.LazyLoad))
            .TemplateOverride = Functions.SafeString(GetSetting(eSetting.TemplateOverride))
            .CSSClassOverride = Functions.SafeString(GetSetting(eSetting.CSSClassOverride))
            .SubText = Functions.SafeString(GetSetting(eSetting.SubText))
        End With

        If Not Extras.CustomSettings.ContainsKey(oCustomSettings.Identifier) Then
            Extras.CustomSettings.Add(oCustomSettings.Identifier, oCustomSettings)
        End If

        '2. Clear down the work table.
        BookingBase.SearchDetails.ExtraResults.ClearWorkTable(oCustomSettings.Identifier)

        '3. If not lazyloading perform search
        If Not Extras.CustomSettings(oCustomSettings.Identifier).LazyLoad Then Me.ExtraSearch(sIdentifier)

        '4. Set up xsl
        Dim oExtrasXML As XmlDocument = BookingBase.SearchDetails.ExtraResults.GetExtrasXML(sIdentifier, 1)
        Dim oBasketXML As New XmlDocument
        Select Case oCustomSettings.BasketType
            Case eBasketType.Search
                oBasketXML = BookingBase.SearchBasket.XML
            Case eBasketType.Main
                oBasketXML = BookingBase.Basket.XML
            Case Else
                oBasketXML = BookingBase.Basket.XML
        End Select
        Dim oExtraInformationXML As XmlDocument = Utility.BigCXML(sIdentifier + "Information", 1, 60)
        Dim oXML As XmlDocument = XMLFunctions.MergeXMLDocuments("Extras", oExtrasXML, oBasketXML, oExtraInformationXML)

        '5. Params
        Dim oXSLParameters As WebControls.XSL.XSLParams = Extras.XSLParameters(sIdentifier)

        '6. Transform
        If Functions.SafeString(Extras.CustomSettings(oCustomSettings.Identifier).TemplateOverride) <> "" Then
            Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & oCustomSettings.TemplateOverride), writer, oXSLParameters)
        Else
            Me.XSLTransform(oXML, XSL.SetupTemplate(res.Extras, True, False), writer, oXSLParameters)
        End If

    End Sub

    Public Shared Function XSLParameters(ByVal sIdentifier As String) As WebControls.XSL.XSLParams

        Dim oXSLParameters As New WebControls.XSL.XSLParams
        With oXSLParameters
            .AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("Identifier", sIdentifier)
            .AddParam("DisplayType", Extras.CustomSettings(sIdentifier).DisplayType.ToString)
            .AddParam("LazyLoad", Extras.CustomSettings(sIdentifier).LazyLoad.ToString)
            .AddParam("Title", Extras.CustomSettings(sIdentifier).Title.ToString)
            .AddParam("ExtraCount", Extras.GetExtraCount(sIdentifier))
            .AddParam("CSSClassOverride", Extras.CustomSettings(sIdentifier).CSSClassOverride)
            .AddParam("SubText", Extras.CustomSettings(sIdentifier).SubText)
            .AddParam("TotalPassengers", BookingBase.SearchBasket.TotalAdults + BookingBase.SearchBasket.TotalChildren)
        End With

        Return oXSLParameters

    End Function

    Public Function UpdateExtras(ByVal Identifier As String, Optional ByVal SearchAgain As Boolean = False) As String

        '1. Get settings for current identifier
        Dim oCustomSettings As New CustomSetting
        oCustomSettings = Extras.CustomSettings(Identifier)

        '2. Check if we're searching again
        If SearchAgain Then
            '2.1. Clear down the work table.
            BookingBase.SearchDetails.ExtraResults.ClearWorkTable(oCustomSettings.Identifier)

            '2.2. If not lazyloading perform search
            If Not Extras.CustomSettings(oCustomSettings.Identifier).LazyLoad Then Me.ExtraSearch(Identifier)
        End If

        '3. Set up xml
        Dim oExtrasXML As XmlDocument = BookingBase.SearchDetails.ExtraResults.GetExtrasXML(Identifier, 1)
        Dim oBasketXML As New XmlDocument
        Select Case oCustomSettings.BasketType
            Case eBasketType.Search
                oBasketXML = BookingBase.SearchBasket.XML
            Case eBasketType.Main
                oBasketXML = BookingBase.Basket.XML
            Case Else
                oBasketXML = BookingBase.Basket.XML
        End Select
        Dim ExtraInformationXML As XmlDocument = Utility.BigCXML(Identifier + "Information", 1, 60)
        Dim oXML As XmlDocument = XMLFunctions.MergeXMLDocuments("Extras", oExtrasXML, oBasketXML, ExtraInformationXML)

        '4. Params
        Dim oXSLParameters As WebControls.XSL.XSLParams = Extras.XSLParameters(Identifier)
        oXSLParameters.AddParam("JustResults", True)
        oXSLParameters.AddParam("StartDate", BookingBase.SearchBasket.FirstDepartureDate.ToString("s"))
        oXSLParameters.AddParam("EndDate", Me.LastReturnDate().ToString("s"))

        '5. Build up the HTML
        Dim sHTML As String = ""
        If oCustomSettings.TemplateOverride <> "" Then
            sHTML = XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & oCustomSettings.TemplateOverride), oXSLParameters)
        Else
            sHTML = XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.Extras, True, True), oXSLParameters)
        End If

        '6. Put it all into a return object and send it on its way.
        Dim oUpdateReturn As New UpdateReturn
        With oUpdateReturn
            .HTML = Web.Translation.TranslateHTML(sHTML)
            .Identifier = Identifier
            .Title = oCustomSettings.Title
        End With

        '7. Return
        Return JsonConvert.SerializeObject(oUpdateReturn)

    End Function

#End Region

#Region "Search & Add To Basket"

    Public Sub ExtraSearch(ByVal Identifier As String)

        '1. Find ids for specific search
        Dim iExtraID As Integer = 0

        '2. Search for specific extra ID is setting is set
        If Not Extras.CustomSettings(Identifier).Extra = "" Then
            iExtraID = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.Extra, "ExtraName", "ExtraID", Extras.CustomSettings(Identifier).Extra)
        End If

        '3. Search for extra group ID if setting is set
        Dim iExtraGroupID As Integer = 0
        If Not Extras.CustomSettings(Identifier).ExtraGroup = "" Then
            iExtraGroupID = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.ExtraGroup, "ExtraGroup", "ExtraGroupID", Extras.CustomSettings(Identifier).ExtraGroup)
        End If

        '4. Search for any extra types that have been set in the ExtraTypes setting and get their IDs
        Dim aExtraTypeIDs As New Generic.List(Of Integer)
        For Each sExtraType As String In Extras.CustomSettings(Identifier).ExtraTypes
            Dim iExtraTypeID As Integer = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", sExtraType)
            If iExtraTypeID > 0 Then aExtraTypeIDs.Add(iExtraTypeID)
        Next

        '5. Search based on which setting have been set
        Dim oExtraSearchReturn As BookingSearch.SearchReturn = BookingBase.SearchDetails.ExtraSearchFromBasket(Identifier, iExtraID, iExtraGroupID, aExtraTypeIDs)

    End Sub

    Public Function AddExtraToBasket(ByVal BookingToken As String, ByVal Identifier As String, Optional ByVal ClearBasket As Boolean = False) As String

        '1. Get the handler result
        Dim oExtra As ExtraResultHandler.Extra = BookingBase.SearchDetails.ExtraResults.GetSingleExtra(Identifier, BookingToken)

        '2. Use the index and booking token to ensure we have the right extra with the right options and hash token that baby up.
        Dim sHashtoken As String = BookingBase.SearchDetails.ExtraResults.ExtraHashToken(Identifier, oExtra.Index, BookingToken)

        '3. Add extra to basket
        BookingExtra.AddExtraToBasket(Identifier, sHashtoken, ClearBasket:=ClearBasket)

        '4. Get updated extras html
        Dim sJSON As String = Me.UpdateExtras(Identifier)
        Return sJSON

    End Function

#End Region

#Region "Remove Extra"

    Public Function RemoveExtraByType(ByVal Identifier As String) As String

        '1. Loop through ExtraTypes associated with the identifier and get the ExtraTypeIDs
        Dim aExtraTypeIDs As New Generic.List(Of Integer)
        For Each sExtraType As String In Extras.CustomSettings(Identifier).ExtraTypes
            Dim iExtraTypeID As Integer = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", sExtraType)
            If iExtraTypeID > 0 Then aExtraTypeIDs.Add(iExtraTypeID)
        Next

        '2. Loop through our collection of ExtraTypes and remove all of them.
        For Each iExtraTypeID As Integer In aExtraTypeIDs
            BookingExtra.RemoveExtrasFromBasket(iExtraTypeID)
        Next

        '3. Get updated extras and return JSON
        Dim sJSON As String = Me.UpdateExtras(Identifier)
        Return sJSON

    End Function

    Public Function RemoveExtraByIndex(ByVal iIndex As Integer, ByVal Identifier As String) As String

        Try
            '1. Get the hashtoken
            Dim sHashToken As String = BookingBase.SearchDetails.ExtraResults.ExtraHashToken(Identifier, iIndex)

            '2. Use the token to remove the extra from the basket.
            BookingExtra.RemoveExtraFromBasket(sHashToken)

            '3.Get Updated html and return
            Dim sJSON As String = Me.UpdateExtras(Identifier)
            Return sJSON

        Catch ex As Exception
            Return "Error|" & ex.ToString
        End Try

    End Function

#End Region

#Region "Support Functions"

    Public Shared Function GetExtraCount(ByVal Identifier As String) As Integer

        '1.Loop through and get the ids associated with the extra
        Dim aExtraTypeIDs As New Generic.List(Of Integer)
        For Each sExtraType As String In Extras.CustomSettings(Identifier).ExtraTypes
            Dim iExtraTypeID As Integer = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", sExtraType)
            If iExtraTypeID > 0 Then aExtraTypeIDs.Add(iExtraTypeID)
        Next
        Dim iExtraID As Integer = 0
        If Not Extras.CustomSettings(Identifier).Extra = "" Then
            iExtraID = BookingBase.Lookups.IDLookup(Web.Lookups.LookupTypes.Extra, "ExtraName", "ExtraID", Extras.CustomSettings(Identifier).Extra)
        End If

        Dim iExtraCount As Integer = 0

        '2.Loop through our basket and if any of the extras within it match our extra type or extra, increment our count
        For Each oExtra As BookingExtra.BasketExtra In BookingBase.SearchBasket.BasketExtras

            For Each oBasketOption As BookingExtra.BasketExtra.BasketExtraOption In oExtra.BasketExtraOptions
                If oBasketOption.ExtraID = iExtraID OrElse aExtraTypeIDs.Contains(oBasketOption.ExtraTypeID) Then
                    iExtraCount += 1
                End If
            Next

        Next

        Return iExtraCount

    End Function

    Public Function LastReturnDate() As DateTime
        Dim dLastReturnDate As DateTime = Now.AddYears(-1000)

        For Each oBasketProperty As BookingProperty.BasketProperty In BookingBase.SearchBasket.BasketProperties
            If oBasketProperty.RoomOptions.Max(Of DateTime)(Function(o) o.DepartureDate) > dLastReturnDate Then
                dLastReturnDate = oBasketProperty.RoomOptions.Max(Of DateTime)(Function(o) o.DepartureDate)
            End If
        Next

        For Each oBasketFlight As BookingFlight.BasketFlight In BookingBase.SearchBasket.BasketFlights
            If oBasketFlight.Flight.ReturnArrivalDate > dLastReturnDate Then
                dLastReturnDate = oBasketFlight.Flight.ReturnArrivalDate
            End If
        Next

        For Each oBasketTransfer As BookingTransfer.BasketTransfer In BookingBase.SearchBasket.BasketTransfers
            If oBasketTransfer.Transfer.ReturnDate > dLastReturnDate Then
                dLastReturnDate = oBasketTransfer.Transfer.ReturnDate
            End If
        Next

        For Each oBasketExtra As BookingExtra.BasketExtra In BookingBase.SearchBasket.BasketExtras
            If oBasketExtra.BasketExtraOptions(0).EndDate > dLastReturnDate Then
                dLastReturnDate = oBasketExtra.BasketExtraOptions(0).EndDate
            End If
        Next

        Return dLastReturnDate
    End Function

#End Region

#Region "Support Classes"

    Public Enum eSetting
        <Title("Unique identifier")>
        <Description("Unique identifier for this instance of the extras widget")>
        <DeveloperOnly(True)>
        Identifier

        <Title("Basket Type")>
        <Description("Basket type to check. Search or Main")>
        <DeveloperOnly(True)>
        BasketType

        <Title("Title")>
        <Description("Title to show in widget header")>
        Title

        <Title("Extra")>
        <Description("Name of specific extra to search for")>
        <DeveloperOnly(True)>
        Extra

        <Title("Extra Types")>
        <Description("List of extra types to search for. These should be separated with a pipe")>
        <DeveloperOnly(True)>
        ExtraTypes

        <Title("Extra Group")>
        <Description("Extra group to search for")>
        <DeveloperOnly(True)>
        ExtraGroup

        <Title("Display Type")>
        <Description("Method of displaying results. SimpleRadio or SimpleCheckbox")>
        <DeveloperOnly(True)>
        DisplayType

        <Title("Lazy Load")>
        <Description("Whether to lazy load or search immediately")>
        <DeveloperOnly(True)>
        LazyLoad

        <Title("Template Override")>
        <Description("XSL Template Override")>
        <DeveloperOnly(True)>
        TemplateOverride

        <Title("CSS Class Override")>
        <Description("Overrides the CSS class of the widget's container to the specified value")>
        <DeveloperOnly(True)>
        CSSClassOverride

        <Title("Subtext")>
        <Description("Text to display under the header, before the extra results")>
        SubText
    End Enum

    Public Class CustomSetting
        Public Identifier As String = ""
        Public BasketType As eBasketType = eBasketType.Search
        Public Title As String = ""
        Public Extra As String = ""
        Public ExtraTypes As New Generic.List(Of String)
        Public ExtraGroup As String = ""
        Public DisplayType As eDisplayType = eDisplayType.SimpleCheckBox 'could be possible to extend to have list of types/groups and a display type for each if want to search for all in one go
        Public LazyLoad As Boolean = False
        Public TemplateOverride As String
        Public CSSClassOverride As String
        Public SubText As String
    End Class

    Public Class UpdateReturn
        Public Identifier As String = ""
        Public Title As String = ""
        Public HTML As String = ""
    End Class

    Public Enum eBasketType
        Search
        Main
    End Enum

    Public Enum eDisplayType
        SimpleCheckBox
        SimpleRadio
    End Enum

    Public Class RemoveExtraReturn
        Public OK As String
        Public UpdateBasket As String
    End Class

#End Region

End Class
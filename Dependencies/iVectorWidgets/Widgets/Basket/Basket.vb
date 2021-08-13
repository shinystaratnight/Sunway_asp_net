Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.Configuration.ConfigurationManager
Imports System.ComponentModel

Public Class Basket
    Inherits WidgetBase

#Region "Fields + properties"

	Protected oSettings As New CustomSetting

#End Region


#Region "Render methods"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)
		Dim sHTML As String = WidgetRender(True, False)
		writer.Write(sHTML)
	End Sub

	Public Overridable Function UpdateBasketHTML() As String
		Return WidgetRender(False, True)
	End Function

	'holder for any setup from parent widgets eg adding custom xsl params 
	'they should always call MyBase.Setup() to get the settings set up correctly
	'ALL settings should exist on the base widget
	Public Overridable Sub Setup()

		'set up the custom settings for this widget - done in the common render code because if we use a constructor it falls over
		'not sure why, by the Activator.CreateInstance errors if we add default constructor to a widget
		oSettings = New CustomSetting
		oSettings = Me.CustomSettings(Of CustomSetting)(oSettings, "Basket", New PageServer_Beta())
        Me.WidgetName = "Basket"

	End Sub

	''' <summary>
	''' Common render function, whether from page load or ajax
	''' </summary>
	''' <param name="IncludeContainer">True on page load, false on ajax so we only replace inner content</param>
	''' <param name="Translate">False on page load as this happens in master page code, true on ajax so "ml"s are correctly replaced</param>
	''' <returns>HTML string</returns>
	Private Function WidgetRender(IncludeContainer As Boolean, Translate As Boolean) As String

		'1. run any setup code 
		Me.Setup()

		'2. get basket based on setting, exit if nothing in there
		Dim oBasket As BookingBasket = Functions.IIf(oSettings.BasketType = eBasketType.Main, BookingBase.Basket, BookingBase.SearchBasket)
		If oBasket.TotalComponents = 0 Then Return ""


        Dim oBasketComponents As New Generic.List(Of BasketComponentSortDef)
	    For Each BasketProperty As BookingProperty.BasketProperty In oBasket.BasketProperties
            Dim PropertyComponent As New BasketComponentSortDef
            Dim PropertyArrivalDate As DateTime  = BasketProperty.RoomOptions(0).ArrivalDate
            With PropertyComponent
                .Token = BasketProperty.RoomOptions(0).BookingToken
                .ComponentType = "Property"
                .ReturnLeg = False
                .DepartureDate = New DateTime(PropertyArrivalDate.Year, PropertyArrivalDate.Month, PropertyArrivalDate.Day, 23, 59, 00)
            End With
            oBasketComponents.Add(PropertyComponent)
	    Next
	    For Each BasketTransfer As BookingTransfer.BasketTransfer In oBasket.BasketTransfers
	        Dim TransferComponent As New BasketComponentSortDef
            Dim TransferTime As TimeSpan = TimeSpan.Parse(BasketTransfer.Transfer.DepartureTime)
	        With TransferComponent
	            .Token = BasketTransfer.Transfer.BookingToken
	            .ComponentType = "Transfer"
	            .ReturnLeg = False
	            .DepartureDate = BasketTransfer.Transfer.DepartureDate.Add(TransferTime)
	        End With
	        oBasketComponents.Add(TransferComponent)

	        If Not Date.Compare(BasketTransfer.Transfer.ReturnDate, New Date()) = 0  Then
	            Dim ReturnTransferComponent As New BasketComponentSortDef
	            Dim ReturnTransferTime As TimeSpan = TimeSpan.Parse(BasketTransfer.Transfer.ReturnTime)
	            With ReturnTransferComponent
	                .Token = BasketTransfer.Transfer.BookingToken
	                .ComponentType = "Transfer"
	                .ReturnLeg = True
	                .DepartureDate = BasketTransfer.Transfer.ReturnDate.Add(ReturnTransferTime)
	            End With
	            oBasketComponents.Add(ReturnTransferComponent)
	        End If
	    Next
	    For Each BasketExtra As BookingExtra.BasketExtra In oBasket.BasketExtras
            Dim ExtraComponent As New BasketComponentSortDef
	        Dim ExtraTime As TimeSpan = TimeSpan.Parse(BasketExtra.BasketExtraOptions(0).StartTime)
            With ExtraComponent
                .Token = BasketExtra.BasketExtraOptions(0).BookingToken
                .ComponentType = "Extra"
                .DepartureDate = BasketExtra.BasketExtraOptions(0).StartDate.Add(ExtraTime)
                .ReturnLeg = False
            End With
	        oBasketComponents.Add(ExtraComponent)
	    Next
        For Each BasketFlight As BookingFlight.BasketFlight In oBasket.BasketFlights
            Dim FlightComponent As New BasketComponentSortDef
            Dim FlightTime As TimeSpan = TimeSpan.Parse(BasketFlight.Flight.OutboundDepartureTime)
            With FlightComponent
                .Token = BasketFlight.Flight.BookingToken
                .ComponentType = "Flight"
                .ReturnLeg = False
                .DepartureDate = BasketFlight.Flight.OutboundDepartureDate.Add(FlightTime)
            End With
            oBasketComponents.Add(FlightComponent)
        Next
	    For Each BasketCarHire As BookingCarHire.BasketCarHire In oBasket.BasketCarHires
	        Dim CarHireComponent As New iVectorWidgets.BasketComponentSortDef
	        Dim CarHireTime As TimeSpan = TimeSpan.Parse(BasketCarHire.CarHire.PickUpTime)
	        With CarHireComponent
	            .Token = BasketCarHire.CarHire.BookingToken
	            .ComponentType = "CarHire"
	            .ReturnLeg = False
	            .DepartureDate = BasketCarHire.CarHire.PickUpDate.Add(CarHireTime)
	        End With
	        oBasketComponents.Add(CarHireComponent)
	    Next

        oBasketComponents.Sort(Function(x, y) x.DepartureDate.CompareTo(y.DepartureDate))

	    Dim oxml As XmlDocument = Me.GenerateXML(oBasket)
        For Each oBasketComponent As BasketComponentSortDef In oBasketComponents 
            oxml = XMLFunctions.MergeXMLDocuments(oxml, Serializer.Serialize(oBasketComponent))
        Next

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParameters(oBasket, IncludeContainer)


        '8. transform and return
        Dim sHTML As String
        If oSettings.TemplateOverride <> "" Then
            sHTML = Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & oSettings.TemplateOverride), oXSLParams, Translate)
        Else
            sHTML = Me.XSLTransform(oXML, XSL.SetupTemplate(res.Basket, True, False), oXSLParams, Translate)
        End If
        Return sHTML

	End Function


	''' <summary>
	''' Generates the basket xml
	''' </summary>
	''' <param name="Basket">Pass in the BookingBasket</param>
	''' <returns>HTML string</returns>
	Protected Function GenerateXML(Basket As BookingBasket) As XmlDocument


		If Basket.TotalComponents = 0 Then Return New XmlDocument

		'3 Work out number of weeks until departure (This is needed for Arrowtour's basket - JS)
		'Adam says - this should probably be added to the settings code - can someone confirm?
		Dim iWeeksTilDeparture As Integer = 0
		If Basket.BasketFlights.Count > 0 Then
			iWeeksTilDeparture = Intuitive.Functions.SafeInt(DateDiff(DateInterval.Weekday, Today, Basket.BasketFlights(0).Flight.OutboundDepartureDate))
		ElseIf Basket.BasketProperties.Count > 0 Then
			iWeeksTilDeparture = Intuitive.Functions.SafeInt(DateDiff(DateInterval.Weekday, Today, Basket.BasketProperties(0).RoomOptions(0).ArrivalDate))
		End If
		oSettings.WeeksUntilDeparture = iWeeksTilDeparture


		'4 set up various xml
		Dim oBasketXML As XmlDocument = Basket.XML
		Dim oTextOverridesXML As XmlDocument = Utility.GetTextOverridesXML(oSettings.TextOverrides)
		Dim oFlightResultsExtraXML As XmlDocument = Utility.BigCXML("FlightResultsExtraXML", 1, 60)
        Dim oCarHireDepotXML As XmlDocument = Serializer.Serialize(Lookups.CarHireDepots, True)
		Dim oExtraTypesXML As XmlDocument = Utility.BigCXML("ExtraTypes", 1, 60)
		Dim oExtraTimeOffsetXML As New XmlDocument

		If (Not oExtraTypesXML Is Nothing) AndAlso (BookingBase.SearchBasket.BasketFlights.Count > 0) Then
			Dim iEntryTimeOffsetHours As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(oExtraTypesXML, "ExtraTypes/ExtraType[ExtraType = 'Airport Parking']/EntryTimeOffsetHours"))
			Dim iExitTimeOffsetHours As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(oExtraTypesXML, "ExtraTypes/ExtraType[ExtraType = 'Airport Parking']/ExitTimeOffsetHours"))

			Dim dOutboundDepartureDateTime As DateTime = BookingBase.SearchBasket.BasketFlights.FirstOrDefault().Flight.OutboundDepartureDate
			Dim sOutboundDepartureTime As String = BookingBase.SearchBasket.BasketFlights.FirstOrDefault().Flight.OutboundDepartureTime

			'Generate correct outbound departure datetime
			dOutboundDepartureDateTime = dOutboundDepartureDateTime.AddHours(Functions.SafeInt(sOutboundDepartureTime.Split(":"c)(0)))
			dOutboundDepartureDateTime = dOutboundDepartureDateTime.AddMinutes(Functions.SafeInt(sOutboundDepartureTime.Split(":"c)(1)))

			'Add offset
			dOutboundDepartureDateTime = dOutboundDepartureDateTime.AddHours(-iEntryTimeOffsetHours)

			Dim oOffsets As New ExtraOffsetTimes
			oOffsets.OutboundOffset = dOutboundDepartureDateTime

			'Calculate offset for return flights
			If Not DateFunctions.IsEmptyDate(BookingBase.SearchBasket.BasketFlights.FirstOrDefault().Flight.ReturnArrivalDate) Then
				Dim dReturnArrivalDateTime As DateTime = BookingBase.SearchBasket.BasketFlights.FirstOrDefault().Flight.ReturnArrivalDate
				Dim sReturnArrivalTime As String = BookingBase.SearchBasket.BasketFlights.FirstOrDefault().Flight.ReturnArrivalTime

				'Generate correct return arrival datetime
				dReturnArrivalDateTime = dReturnArrivalDateTime.AddHours(Functions.SafeInt(sReturnArrivalTime.Split(":"c)(0)))
				dReturnArrivalDateTime = dReturnArrivalDateTime.AddMinutes(Functions.SafeInt(sReturnArrivalTime.Split(":"c)(1)))

				'Add offset
				dReturnArrivalDateTime = dReturnArrivalDateTime.AddHours(iExitTimeOffsetHours)

				oOffsets.ReturnOffset = dReturnArrivalDateTime
			End If

			oExtraTimeOffsetXML = Intuitive.Serializer.Serialize(oOffsets, True)
		End If
		'5 How we get our points values (currently only used for monster)
		'TODO - Better solution needed than having customer specific content here
		' Adam says - if this is TODO then please discuss a better solution with the team , this will just be left otherwise
		Dim oPointsXML As New XmlDocument
		If oSettings.UsePoints Then
			'We probably want to include these in our Global on session start to get them in the cache early.
			oPointsXML = Utility.BigCXML("BrandTiers", 1, 600)
			oPointsXML = XMLFunctions.MergeXMLDocuments("Points", oPointsXML, Utility.BigCXML("Settings", 1, 600))
		End If


		'6 Carrierlogo overbranding
		Dim oCarrierLogosXML As New XmlDocument
		If oSettings.CarrierLogoOverbranding Then
			oCarrierLogosXML = Utility.BigCXML("CarrierLogos", 1, 60)
		End If

		Dim oBookingAdjustmentsXML As New XmlDocument
		If Me.oSettings.PageName = "BookingSummary" Then
			oBookingAdjustmentsXML = Utility.BigCXML("BookingAdjustmentTypes", 1, 60)
		End If

		Dim oBookingAdjustmentTypesXML As New XmlDocument
		If Me.oSettings.LoadBookingAdjustmentTypes AndAlso (Not BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes Is Nothing) Then
			oBookingAdjustmentTypesXML = Intuitive.Serializer.Serialize(BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes, True)
		End If



		'7 final XML + params
		Dim oXML As XmlDocument = MergeXMLDocuments(oBasketXML, oTextOverridesXML, oFlightResultsExtraXML, oCarHireDepotXML, oPointsXML, oCarrierLogosXML, oBookingAdjustmentsXML, oExtraTypesXML, oExtraTimeOffsetXML, oBookingAdjustmentTypesXML)

		Return oXML

	End Function


	Public Function XSLParameters(ByVal oBasket As BookingBasket, ByVal IncludeContainer As Boolean) As WebControls.XSL.XSLParams
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Title", Me.oSettings.Title)
			.AddParam("ExcludeSubtotals", Me.oSettings.ExcludeSubTotals)
			.AddParam("PassengerBreakdown", Me.oSettings.PassengerBreakdown)
			.AddParam("HideBaggage", Me.oSettings.HideBaggage)
			.AddParam("TotalComponents", oBasket.TotalComponents)
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("CMSBaseURL", Functions.IIf(Me.UseSecureCMSBaseURL, BookingBase.Params.CMSBaseURL.Replace("http", "https"), BookingBase.Params.CMSBaseURL))
			.AddParam("IncludeContainer", IncludeContainer)
			.AddParam("Printable", Me.oSettings.Printable)
			.AddParam("DescriptiveSubtotals", Me.oSettings.DescriptiveSubtotals)
			.AddParam("WeeksUntilDeparture", Me.oSettings.WeeksUntilDeparture)
			.AddParam("CSSClassOverride", Me.oSettings.CSSClassOverride)
			.AddParam("TodaysDate", Intuitive.DateFunctions.SQLDateFormat(Date.Today))
			.AddParam("BrandID", BookingBase.Params.BrandID)
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("PageName", Me.oSettings.PageName)
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("UpdateFunction", Me.oSettings.UpdateFunction)
			.AddParam("HasOverBrandedLogos", Me.oSettings.CarrierLogoOverbranding)
			.AddParam("TotalPriceText", Me.oSettings.TotalPriceText)
			.AddParam("PriceFormat", Me.oSettings.PriceFormat)
			.AddParam("BookingAdjustmentAmount", BookingAdjustment.CalculateBookingAdjustments(Me.oSettings.BookingAdjustmentTypeCSV))
            .AddParam("AddAdjustmentTypesToPrice", Me.oSettings.AddAdjustmentTypesToPrice)
            .AddParam("RemoveComponentFailMessage", Me.oSettings.RemoveComponentFailMessage)
			For Each oParam As KeyValuePair(Of String, String) In Me.ExtraParams
				.AddParam(oParam.Key, oParam.Value)
			Next

		End With
		Return oXSLParams
	End Function

#End Region

#Region "Utility"

	'functions like this should just be restful endpoints - we dont need to go through instantiating this class via reflection for this
	Public Function RemoveRoom(ByVal HashToken As String) As Boolean

		Return BookingProperty.RemoveRoomFromBasket(HashToken)

	End Function

	Public Function UseSecureCMSBaseURL() As Boolean

		If Me.oSettings.PaymentPage Then
			Return BookingBase.Params.SecurePaymentPage OrElse Functions.SafeBoolean(AppSettings("SecurePaymentPage"))
		Else
			Return False
		End If

	End Function

	Protected Enum eBasketType
		Search
		Main
	End Enum

	Public Class ExtraOffsetTimes
		Public OutboundOffset As DateTime
		Public ReturnOffset As DateTime
	End Class

#End Region

#Region "Settings"

	Protected Class CustomSetting
		Implements Widgets.iWidgetSettings(Of CustomSetting)

		Public BasketType As eBasketType
		Public Title As String
		Public ExcludeSubTotals As Boolean
		Public TextOverrides As String
		Public PassengerBreakdown As Boolean
		Public HideBaggage As Boolean
		Public TemplateOverride As String
		Public Printable As Boolean
		Public DescriptiveSubtotals As Boolean
		Public WeeksUntilDeparture As Integer
		Public CSSClassOverride As String
		Public UsePoints As Boolean
		Public PageName As String
		Public UpdateFunction As String
		Public CarrierLogoOverbranding As Boolean
		Public TotalPriceText As String
		Public PaymentPage As Boolean
		Public PriceFormat As String
		Public BookingAdjustmentTypeCSV As String = ""
		Public LoadBookingAdjustmentTypes As Boolean = False
		Public AddAdjustmentTypesToPrice As Boolean = False
		Public RemoveComponentFailMessage As String


		Private Function Setup(Basket As WidgetBase) As CustomSetting Implements iWidgetSettings(Of CustomSetting).Setup

			Me.BasketType = Functions.SafeEnum(Of eBasketType)(Basket.GetSetting(eSetting.BasketType))
			Me.Title = Functions.SafeString(Basket.GetSetting(eSetting.Title))
			Me.ExcludeSubTotals = Functions.SafeBoolean(Basket.GetSetting(eSetting.ExcludeSubtotals))
			Me.TextOverrides = Functions.SafeString(Basket.GetSetting(eSetting.TextOverrides))
			Me.PassengerBreakdown = Functions.SafeBoolean(Basket.GetSetting(eSetting.PassengerBreakdown))
			Me.HideBaggage = Functions.SafeBoolean(Basket.GetSetting(eSetting.HideBaggage))
			Me.TemplateOverride = Basket.GetSetting(eSetting.TemplateOverride)
			Me.Printable = Functions.SafeBoolean(Basket.GetSetting(eSetting.Printable))
			Me.DescriptiveSubtotals = Functions.SafeBoolean(Basket.GetSetting(eSetting.DescriptiveSubtotals))
			Me.CSSClassOverride = Functions.SafeString(Basket.GetSetting(eSetting.CSSClassOverride))
			Me.UsePoints = Functions.SafeBoolean(Basket.GetSetting(eSetting.UsePoints))
			Me.PageName = Functions.SafeString(Basket.PageDefinition.PageName)
			Me.UpdateFunction = Functions.SafeString(Basket.GetSetting(eSetting.UpdateFunction))
			Me.CarrierLogoOverbranding = Functions.SafeBoolean(Basket.GetSetting(eSetting.CarrierLogoOverbranding))
			Me.TotalPriceText = Basket.GetSetting(eSetting.TotalPriceText)
			Me.PaymentPage = Functions.SafeBoolean(Basket.GetSetting(eSetting.PaymentPage))
			Me.PriceFormat = Functions.IIf(Functions.SafeString(Basket.GetSetting(eSetting.PriceFormat)) <> "", Basket.GetSetting(eSetting.PriceFormat), "######")
			Me.BookingAdjustmentTypeCSV = Functions.SafeString(Basket.GetSetting(eSetting.BookingAdjustmentTypeCSV))
			Me.LoadBookingAdjustmentTypes = Functions.SafeBoolean(Basket.GetSetting(eSetting.LoadBookingAdjustmentTypes))
			Me.AddAdjustmentTypesToPrice = Functions.SafeBoolean(Basket.GetSetting(eSetting.AddAdjustmentTypesToPrice))
			Me.RemoveComponentFailMessage = Functions.SafeString(Basket.GetSetting(eSetting.RemoveComponentFailMessage))
			Return Me

		End Function

	End Class

	Public Enum eSetting
		<Title("Title")>
		<Description("Main title text for basket section")>
		Title

		<Title("Text Override")>
		<Description("String")>
		<DeveloperOnly(True)>
		TextOverrides

		<Title("Basket Type")>
		<Description("Search or Main")>
		BasketType

		<Title("Exclude Subtotals Boolean")>
		<Description("Boolean that specifies whether or not to hide subtotals")>
		ExcludeSubtotals

		<Title("Passenger Breakdown Boolean")>
		<Description("Boolean that specifies whether or not to show passenger breakdown")>
		PassengerBreakdown

		<Title("Hide Baggage Boolean")>
		<Description("Boolean that specifies whether or not to hide baggage information")>
		HideBaggage

		<Title("XSL Template Override")>
		<Description("File path to an overide XSL (name it breadcrumbs.xsl)")>
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("Printable Boolean")>
		<Description("Boolean that specifies whether or not to show a print link")>
		Printable

		<Title("Descriptive Subtotals Boolean")>
		<Description("Boolean that specifies whether or not to show descriptive subtotals")>
		DescriptiveSubtotals

		<Title("CSS Class Override")>
		<Description("CSS class overide on main wrapper div for basket section")>
		CSSClassOverride

		<Title("Use Points")>
		<Description("Boolean")>
		UsePoints

		<Title("Update Function")>
		<Description("JS update function")>
		<DeveloperOnly(True)>
		UpdateFunction

		<Title("Carrier Logo Overbranding Boolean")>
		<Description("Boolean specifying if there are oveerbranding carrier logos")>
		CarrierLogoOverbranding

		<Title("Total price header text")>
		<Description("Text for the price heading")>
		TotalPriceText

		<Title("Payment Page")>
		<Description("Boolean specifying if the basket widget will be used on payment page.")>
		PaymentPage

		<Title("Price Format")>
		<Description("The format for the various subtotals/total")>
		PriceFormat

		<Title("BookingAdjustmentTypeCSV")>
		<Description("CSV of bookingadjustmentstypes we want from the SearchDetails.BookingAdjustmentTypes to be shown in the basket price")>
		BookingAdjustmentTypeCSV

		<Title("LoadBookingAdjustmentTypes")>
		<Description("Boolean to say whether to load the BookingAdjustmentType xml")>
		LoadBookingAdjustmentTypes

        <Title("AddAdjustmentTypesToPrice")>
        <Description("Boolean to say whether to add the BookingAdjustmentType values to the display price")>
        AddAdjustmentTypesToPrice

        <Title("RemoveComponentFailMessage")>
        <Description("String stores the message to be displayed if the removal of a component has failed.")>
        RemoveComponentFailMessage

	End Enum

#End Region

End Class

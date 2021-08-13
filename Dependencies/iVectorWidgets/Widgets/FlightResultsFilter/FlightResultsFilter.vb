Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class FlightResultsFilter
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("flightResultsFilters_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("flightResultsFilters_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("flightResultsFilters_customsettings") = value
		End Set
	End Property

#End Region


#Region "Draw"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Set up session settings
        Dim oCustomSettings As CustomSetting = Me.GetCustomSetting()
        FlightResultsFilter.CustomSettings = oCustomSettings

		'2.1 Get XML
		Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetPageXML(1)

		'2.2 Carrierlogo overbranding
		If oCustomSettings.CarrierLogoOverbranding Then
			Dim oCarrierLogosXML As XmlDocument = Utility.BigCXML("CarrierLogos", 1, 60)
			oXML = XMLFunctions.MergeXMLDocuments(oXML, oCarrierLogosXML)
		End If

		If oCustomSettings.UseCustomXML Then
			Dim oCustomXML As XmlDocument = Utility.BigCXML(oCustomSettings.CustomXMLObject, 1, 60, False)
			oXML = Intuitive.XMLFunctions.MergeXMLDocuments(oXML, oCustomXML)
		End If
		
		'3. Setup Params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParameters(True)

		'4. Transform
		If FlightResultsFilter.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & FlightResultsFilter.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.FlightResultsFilter, True, False), writer, oXSLParams)
		End If
	End Sub

#End Region


#Region "Redraw Filters"

	Public Shared Function RedrawFilters() As String

		'1. Get xml
		Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetPageXML(1)

		'2. Setup params
		Dim oXSLParams As WebControls.XSL.XSLParams = XSLParameters(False)

		'3. Transform to html string
        Dim sHTML As String = ""
        If FlightResultsFilter.CustomSettings.TemplateOverride <> "" Then
            sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & FlightResultsFilter.CustomSettings.TemplateOverride), oXSLParams)
        Else
            sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.FlightResultsFilter, True, True), oXSLParams)
        End If

		'4. Return html string
		Return Web.Translation.TranslateHTML(sHTML)

	End Function

#End Region

#Region "Custom Settings"

    ''' <summary>
    ''' Gets all the custom settings from the client
    ''' </summary>
    ''' <returns>Custom Setting with client's settings</returns>
    ''' <remarks></remarks>
    Public Function GetCustomSetting() As CustomSetting

        Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .PerPersonPrices = Functions.SafeBoolean(Settings.GetValue("PerPersonPrices"))
            .TimeOfDayControls = Functions.SafeBoolean(Settings.GetValue("TimeOfDayControls"))
            .TimeSliders = Functions.SafeBoolean(Settings.GetValue("TimeSliders"))
            .UpdateResultsFunction = Functions.SafeString(Settings.GetValue("UpdateResultsFunction"))
            .IncludeMobileFilter = Functions.SafeString(Settings.GetValue("IncludeMobileFilter"))
            .TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
            .IncludeBaggageFilter = Functions.SafeBoolean(Settings.GetValue("IncludeBaggageFilter"))
            .CarrierLogoOverbranding = Functions.SafeBoolean(Settings.GetValue("CarrierLogoOverbranding"))
            .BookingAdjustmentTypeCSV = Functions.SafeString(Settings.GetValue("BookingAdjustmentTypeCSV"))
            .UseCustomXML = Functions.SafeBoolean(Settings.GetValue("UseCustomXML"))
            .CustomXMLObject = Functions.SafeString(Settings.GetValue("CustomXMLObject"))
        End With

        Return oCustomSettings

    End Function

#End Region

#Region "XSL Parameters"

	Public Shared Function XSLParameters(ByVal bIncludeContainer As Boolean) As WebControls.XSL.XSLParams

		Dim oParams As New WebControls.XSL.XSLParams

		With oParams
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
			.AddParam("PerPersonPrices", FlightResultsFilter.CustomSettings.PerPersonPrices)
			.AddParam("PaxCount", (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren).ToSafeInt)
			.AddParam("TimeOfDayControls", FlightResultsFilter.CustomSettings.TimeOfDayControls)
			.AddParam("TimeSliders", FlightResultsFilter.CustomSettings.TimeSliders)
			.AddParam("UpdateResultsFunction", FlightResultsFilter.CustomSettings.UpdateResultsFunction)
			.AddParam("IncludeBaggageFilter", FlightResultsFilter.CustomSettings.IncludeBaggageFilter)
			.AddParam("IncludeContainer", bIncludeContainer)
			.AddParam("HasOverBrandedLogos", FlightResultsFilter.CustomSettings.CarrierLogoOverbranding)
            .AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
		End With

		Return oParams

	End Function

#End Region


#Region "Filter Results"

	Public Shared Function FilterResults(ByVal sRequest As String) As String
		Dim oFilterReturn As New FlightResultsFilter.FilterReturn
		
		Dim oFilter As FlightResultHandler.Filters = BookingBase.SearchDetails.FlightResults.ResultsFilter
		oFilter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of FlightResultHandler.Filters)(sRequest, _
		   New Newtonsoft.Json.Converters.StringEnumConverter)

		'Make sure the Outbound departure date is not overwritten by the new oFilter instance
		'This function is called after the results carousel which will modify the outbound departure date if changed
		oFilter.OutboundDepartureDate = BookingBase.SearchDetails.FlightResults.ResultsFilter.OutboundDepartureDate


		BookingBase.SearchDetails.FlightResults.FilterResults(oFilter)
		With oFilterReturn
			.Filters = oFilter
			.FlightsWithBaggage = BookingBase.SearchDetails.FlightResults.iVectorConnectResults.Where(Function(o) o.IncludesSupplierBaggage).Count()
			.BookingAdjustmentAmount = BookingAdjustment.CalculateBookingAdjustments(FlightResultsFilter.CustomSettings.BookingAdjustmentTypeCSV)
		End With

		'serialize to json and return
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oFilterReturn, New Newtonsoft.Json.Converters.StringEnumConverter)

	End Function

#End Region


#Region "Support Classes"

	Public Class CustomSetting
		Public PerPersonPrices As Boolean
		Public TimeOfDayControls As Boolean
		Public TimeSliders As Boolean
		Public UpdateResultsFunction As String
		Public IncludeMobileFilter As String
		Public TemplateOverride As String
		Public IncludeBaggageFilter As Boolean
		Public CarrierLogoOverbranding As Boolean
		Public BookingAdjustmentTypeCSV As String = ""
		Public UseCustomXML As Boolean
		Public CustomXMLObject As String
	End Class

	Public Class FilterReturn
		Public Filters As FlightResultHandler.Filters
		Public FlightsWithBaggage As Integer
		Public BookingAdjustmentAmount As Decimal
	End Class

#End Region


End Class

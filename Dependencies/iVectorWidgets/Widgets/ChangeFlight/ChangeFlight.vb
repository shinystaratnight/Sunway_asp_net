Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml


Public Class ChangeFlight
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("changeflight_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("changeflight_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(ByVal value As CustomSetting)
			HttpContext.Current.Session("changeflight_customsettings") = value
		End Set

	End Property

#End Region


#Region "Draw"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.ChangeFlightText = Functions.SafeString(Settings.GetValue("ChangeFlightText"))
			.ChangeFlightTextAlt = Functions.SafeString(Settings.GetValue("ChangeFlightTextAlt"))
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
		End With

		ChangeFlight.CustomSettings = oCustomSettings

		If Not BookingBase.SearchDetails.PropertyResults.FlightDictionary.Count = 0 Then

			'get page xml
			Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetFullXML()

			Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParams("Details")

			If ChangeFlight.CustomSettings.TemplateOverride <> "" Then
				Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & ChangeFlight.CustomSettings.TemplateOverride), writer, oXSLParams)
			Else
				Me.XSLTransform(oXML, XSL.SetupTemplate(res.ChangeFlight, True, False), writer, oXSLParams)
			End If

		End If

	End Sub

#End Region


#Region "Select Flight"

	Public Overridable Function SelectFlight(ByVal BookingToken As String) As String

		Dim sHTML As String = ""

		Try
			Dim oPropertyResultsHandler As PropertyResultHandler = BookingBase.SearchDetails.PropertyResults
			oPropertyResultsHandler.UpdateSelectedFlight(BookingToken)
			sHTML = UpdateFlights()
		Catch ex As Exception
			Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.Success = False, _
				.RedirectURL = BookingBase.Params.RootPath & "/?warn=sessionexpired"})
		End Try

		Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.Success = True, .HTML = UpdateFlights()})

	End Function

#End Region


#Region "Update Flights"

	Public Overridable Function UpdateFlights() As String

		'Order our flights by price
		Me.SetupFlights()


		'get flights xml
		Dim oXML As XmlDocument = BookingBase.SearchDetails.FlightResults.GetPageXML(1)


		'setup params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = Me.XSLParams("JustResults")


		'generate html
		Dim sHTML As String = ""
		If ChangeFlight.CustomSettings.TemplateOverride <> "" Then
			sHTML = XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & ChangeFlight.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.ChangeFlight, True, True), oXSLParams)
		End If


		'translate and return
		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

#End Region


#Region "Helpers - SetupFlights, XSLParams"

	Public Function XSLParams(ByVal sTemplate As String) As Intuitive.WebControls.XSL.XSLParams

		'transform xsl
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("SelectedFlightToken", BookingBase.SearchDetails.PropertyResults.FlightDictionary.FirstOrDefault().Value.BookingToken)
            .AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("Template", sTemplate)
			.AddParam("Passengers", BookingBase.SearchBasket.TotalAdults + BookingBase.SearchBasket.TotalChildren)
			.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
			.AddParam("ChangeFlightText", ChangeFlight.CustomSettings.ChangeFlightText)
			.AddParam("ChangeFlightTextAlt", ChangeFlight.CustomSettings.ChangeFlightTextAlt)
		End With

		Return oXSLParams

	End Function


	Public Sub SetupFlights()

		BookingBase.SearchDetails.FlightResults.ResultsSort.SortBy = FlightResultHandler.eSortBy.Price
		BookingBase.SearchDetails.FlightResults.ResultsSort.SortOrder = FlightResultHandler.eSortOrder.Ascending
		BookingBase.SearchDetails.FlightResults.SortResults()

	End Sub


#End Region


End Class


#Region "Support Classes"

Public Class CustomSetting
	Public ChangeFlightText As String
	Public ChangeFlightTextAlt As String
	Public TemplateOverride As String
End Class

#End Region

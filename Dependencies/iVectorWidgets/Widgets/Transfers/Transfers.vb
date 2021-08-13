Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml

Public Class Transfers
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("transfers_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("transfers_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("transfers_customsettings") = value
		End Set

	End Property

#End Region

#Region "Draw"

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'We don't want this on Flight only mode
		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightOnly Then Exit Sub

		' set up custom settings
		Me.SetupCustomSettings()

		' if hotel only get matching airports for property
		Dim oAirportsXML As New XmlDocument
		oAirportsXML = Me.GetAirportsXML
		If oAirportsXML Is Nothing Then Exit Sub

		' if flight+hotel search for transfers - TODO lazyload instead?
        If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
            OrElse BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere Then
			Dim oSearchReturn As BookingSearch.SearchReturn = BookingBase.SearchDetails.TransferSearchFromBasket()
            If oSearchReturn.TransferCount = 0 Then Exit Sub
		End If

		' set up transfers xml
		Dim oTransfersXML As XmlDocument = BookingBase.SearchDetails.TransferResults.GetResultsXML
		' set up text overrides xml
		Dim oTextOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Transfers.CustomSettings.TextOverrides)
		' merge xml
		Dim oXML As XmlDocument = MergeXMLDocuments(oTransfersXML, oTextOverridesXML, oAirportsXML)

		' If we do not have Outbound flight code, we want to set a boolean to tell us to render the fields to add one (will not have one if here from searchtool)
		Dim bFlightCodefields As Boolean = BookingBase.SearchDetails.TransferResults.TotalTransfers > 0 AndAlso BookingBase.SearchDetails.TransferResults.OutboundFlightCode = "" AndAlso BookingBase.SearchDetails.FlightResults.TotalFlights = 0 AndAlso BookingBase.SearchDetails.PropertyResults.TotalHotels = 0

		' set up params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParams("", bFlightCodefields)

		' transform
		Me.DrawTransform(oXML, writer, oXSLParams)

	End Sub

#End Region

#Region "Search Transfers"

	Public Shared Function SearchTransfersFromBasket() As String

		Dim oSearchreturn As New BookingSearch.SearchReturn
		oSearchreturn = BookingBase.SearchDetails.TransferSearchFromBasket()

		Return Transfers.ReturnResults()

	End Function

	Public Shared Function SearchTransfers(ByVal sSearchRequest As String) As String

		Dim oSearchRequest As SearchRequest = Newtonsoft.Json.JsonConvert.DeserializeObject(Of SearchRequest)(sSearchRequest)

		'search for transfers
		Dim oSearchreturn As New BookingSearch.SearchReturn
		If oSearchRequest.PropertyReferenceID <> 0 AndAlso oSearchRequest.AirportID = 0 Then
			oSearchreturn = BookingBase.SearchDetails.TransferSearchFromFlight(oSearchRequest.PropertyReferenceID, oSearchRequest.ReturnDepartureTime)
		ElseIf oSearchRequest.ExtraLocationID <> 0 AndAlso oSearchRequest.AirportID <> 0 Then
			oSearchreturn = BookingBase.SearchDetails.TransferSearchFromExtraLocation(
			  oSearchRequest.AirportID, oSearchRequest.OutboundArrivalTime, oSearchRequest.ReturnDepartureTime,
			  oSearchRequest.OutboundFlightCode, oSearchRequest.ReturnFlightCode, oSearchRequest.ExtraLocationType, oSearchRequest.ExtraLocationID)
		Else
			oSearchreturn = BookingBase.SearchDetails.TransferSearchFromHotel(
			  oSearchRequest.AirportID, oSearchRequest.OutboundArrivalTime, oSearchRequest.ReturnDepartureTime,
			  oSearchRequest.OutboundFlightCode, oSearchRequest.ReturnFlightCode)
		End If

		Return Transfers.ReturnResults()

	End Function

	Public Shared Function ReturnResults() As String

		'create return object
		Dim oTransferSearchReturn As New TransferSearchReturn

		If BookingBase.SearchDetails.TransferResults.TotalTransfers > 0 Then
			'set up transfers xml
			Dim oTransfersXML As XmlDocument = BookingBase.SearchDetails.TransferResults.GetResultsXML

			'set up text overrides xml
			Dim oTextOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Transfers.CustomSettings.TextOverrides)

			'merge xml
			Dim oXML As XmlDocument = MergeXMLDocuments(oTransfersXML, oTextOverridesXML)

			'set up params
			Dim sTemplate As String = Functions.IIf(BookingBase.SearchDetails.TransferResults.TotalTransfers > 0, "Rates", "")
			Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParams(sTemplate, False)

			With oXSLParams
				.AddParam("Redraw", "true")
			End With

			oXML = XMLFunctions.MergeXMLDocuments(oXML, BookingBase.SearchBasket.XML)

			'transform and return
			If Transfers.CustomSettings.TemplateOverride <> "" Then
				oTransferSearchReturn.HTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & Transfers.CustomSettings.TemplateOverride), oXSLParams)
			Else
				oTransferSearchReturn.HTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.Transfers, True, True), oXSLParams)
			End If

		Else
			oTransferSearchReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Transfers", "Sorry there are no transfers available for your holiday combination.")
		End If

		'success if transfers returned
		oTransferSearchReturn.Success = BookingBase.SearchDetails.TransferResults.TotalTransfers > 0

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oTransferSearchReturn)

	End Function

#End Region

#Region "AutoComplete Property Dropdown"

	Public Function AutoCompletePropertyDropdown(ByVal SearchText As String, ByVal sTextBoxID As String) As String

		Dim oAutoCompleteReturn As New AutoCompleteReturn
		'oAutoCompleteReturn.TextBoxID = "acpPriorityPropertyID"
		oAutoCompleteReturn.TextBoxID = sTextBoxID

		'I'm not sure if this is needed seems to work without it.
		'oAutoCompleteReturn.SelectedScript = "SearchTool.Support.SetPriorityPropertyID();"
		oAutoCompleteReturn.SelectedScript = ""

		Dim oProperties As Generic.List(Of Lookups.PropertyReference) = Transfers.PropertySearch(SearchText, 100)

		For Each oProperty As Lookups.PropertyReference In oProperties
			Dim oAutoCompleteItem As New AutoCompleteReturn.AutoCompleteItem(oProperty.PropertyReferenceID.ToString, oProperty.PropertyName)

			If Transfers.CustomSettings.ShowResortsWithHotels Then
				Dim sLocationName As String = Lookups.Locations.FirstOrDefault(Function(o) o.GeographyLevel3ID = oProperty.GeographyLevel3ID).GeographyLevel3Name

				oAutoCompleteItem.Display = String.Format("{0} ({1})", oAutoCompleteItem.Display, sLocationName)
			End If

			oAutoCompleteReturn.Items.Add(oAutoCompleteItem)
		Next

		Dim sJSON As String = Newtonsoft.Json.JsonConvert.SerializeObject(oAutoCompleteReturn)

		Return sJSON

	End Function

#End Region

#Region "Property Search"

	Public Shared Function PropertySearch(ByVal SearchText As String, Optional ByVal MaxResults As Integer = 0) As Generic.List(Of Lookups.PropertyReference)

		'arriving at id
		Dim AirportID As Integer = Functions.SafeInt(BookingBase.SearchBasket.BasketFlights.Last.Flight.ArrivalAirportID)

		'Get Airport GeographyLevel3IDs
		Dim oArrivalAirportIDs As New List(Of Integer)
		oArrivalAirportIDs.Add(AirportID)
		Dim Params As New BookingBase.ParamDef
		Dim oLookups As New Lookups(Params)
		Dim oGeographyLevel3IDs As List(Of Integer) = oLookups.GetAirportGeographyLevel3IDs(oArrivalAirportIDs)

		'loop through each property and add any properties that match geography if set and text input
		Dim oProperties As New Generic.List(Of Lookups.PropertyReference)
		For Each oProperty As Lookups.PropertyReference In BookingBase.Lookups.PropertyReferences _
			.Where(Function(o) (oGeographyLevel3IDs.Contains(o.GeographyLevel3ID)) _
					AndAlso (o.PropertyName.ToLower.StartsWith(SearchText.ToLower) OrElse o.PropertyName.ToLower.Contains(" " & SearchText.ToLower))) _
			.OrderBy(Function(o) o.PropertyName) _
			.OrderBy(Function(o) Functions.IIf(o.PropertyName.StartsWith(SearchText.ToLower), 0, 1))
			oProperties.Add(oProperty)
		Next

		'If there is no max results set, set it to the maximum number, otherwise get the appropriate number
		Dim iMaxResults As Integer = 0
		If MaxResults = 0 Then
			iMaxResults = oProperties.Count
		Else
			iMaxResults = Functions.IIf(oProperties.Count < MaxResults, oProperties.Count, MaxResults)
		End If

		'return results
		oProperties = oProperties.GetRange(0, iMaxResults)
		Return oProperties

	End Function

#End Region

#Region "Add Transfer To Basket"

	Public Shared Function AddTransferToBasket(ByVal HashToken As String, Optional ByVal sOutBoundFlightCode As String = "", Optional ByVal sReturnFlightCode As String = "", Optional ByVal sOutboundTime As String = "", Optional ByVal sReturnTime As String = "") As String

		Dim oAddTransferReturn As New AddTransferReturn With {.Success = False}

		Try
			If Transfers.CustomSettings.TransferOrCarHireOnly AndAlso BookingBase.SearchBasket.BasketCarHires.Count > 0 Then
				'if the setting is on and we already have a transfer, dont allow car hire to be added
				oAddTransferReturn.Success = False
				oAddTransferReturn.Warnings.Add(Functions.IIf(Transfers.CustomSettings.TransferOrCarHireOnlyWarning <> "", Transfers.CustomSettings.TransferOrCarHireOnlyWarning, "You cannot add a transfer as you already have car hire"))

			Else

				'If we are passing in the flight times, we are wanting to ensure the transfer we're trying to add is for the correct times
				If sOutboundTime <> "" AndAlso sReturnTime <> "" Then

					'First check to see if the times match what we already have in the serach results
					Dim oTransferResult As BookingTransfer.Results.Transfer = BookingBase.SearchDetails.TransferResults.Transfers.Where(Function(o) o.TransferOptionHashToken = HashToken).First()
					If oTransferResult.OutboundTime = sOutboundTime AndAlso oTransferResult.ReturnTime = sReturnTime Then

						'all good - just add the transfer as normal
						BookingTransfer.AddTransferToBasket(HashToken, True, sOutBoundFlightCode, sReturnFlightCode)
						oAddTransferReturn.Success = True

					Else

						'we need to research based on new times
						'first grab the current price and vehicle
						Dim sCurrentPrice As Decimal = oTransferResult.Price
						Dim sCurrentVehicle As String = oTransferResult.Vehicle

						'search!
						Dim oSearchReturn As BookingSearch.SearchReturn = BookingBase.SearchDetails.TransferSearchFromHotel(
						 oTransferResult.DepartureParentID, sOutboundTime, sReturnTime, sOutBoundFlightCode, sReturnFlightCode
						)

						If oSearchReturn.OK AndAlso oSearchReturn.TransferCount > 0 Then
							'find the same vehicle from the new results
							Dim oMatchingTransfer As BookingTransfer.Results.Transfer = BookingBase.SearchDetails.TransferResults.Transfers.Where(Function(o) o.Vehicle = sCurrentVehicle).FirstOrDefault

							If oMatchingTransfer IsNot Nothing Then
								'check for a price change in our matching transfer
								If Not oMatchingTransfer.Price = sCurrentPrice Then
									oAddTransferReturn.Warnings.Add("The transfer you have chosen has changed in price for the times entered")
								End If
								'add the transfer to the basket
								BookingTransfer.AddTransferToBasket(oMatchingTransfer.TransferOptionHashToken, True, sOutBoundFlightCode, sReturnFlightCode)
							Else
								'if we cant find a matching transfer - add a warning
								oAddTransferReturn.Warnings.Add("We could not find a transfer for the times entered that matches your selection. Please select another transfer.")
							End If

							'now we need to redraw the transfers results for the new search results
							Dim oTransfersXML As XmlDocument = BookingBase.SearchDetails.TransferResults.GetResultsXML
							Dim oXSLParams As WebControls.XSL.XSLParams = XSLParams("Rates", False, oMatchingTransfer.TransferOptionHashToken)
							Dim sTransferResultsHTML As String = XMLFunctions.XMLTransformToString(oTransfersXML, HttpContext.Current.Server.MapPath("~/Widgets/Transfers/Transfers.xsl"), oXSLParams)
							oAddTransferReturn.ResultsHTML = Web.Translation.TranslateHTML(sTransferResultsHTML)

							oAddTransferReturn.Success = True '- finally yay!

						Else
							'no search results> - add warning
							oAddTransferReturn.Warnings.Add("Sorry, we could not find transfers for the times entered")
						End If

					End If

				Else
					'add transfer to basket
					BookingTransfer.AddTransferToBasket(HashToken, True, sOutBoundFlightCode, sReturnFlightCode)

					'return
					oAddTransferReturn.Success = True
				End If

			End If

		Catch ex As Exception
			oAddTransferReturn.Warnings.Add("Sorry, there was an unexpected error adding this transfer")
		End Try

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oAddTransferReturn)

	End Function

#End Region

#Region "Remove Transfer"

	Public Shared Function RemoveTransfer() As String

		Dim oRemoveTransferReturn As New RemoveTransferReturn

		'we don't need to update the basket if there are no transfers in there
		If BookingBase.SearchBasket.BasketTransfers.Count > 0 Then
			oRemoveTransferReturn.UpdateBasket = "True"
		Else
			oRemoveTransferReturn.UpdateBasket = "False"
		End If

		'clear transfers
		BookingBase.SearchBasket.BasketTransfers.Clear()

		'return
		oRemoveTransferReturn.OK = "True"
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oRemoveTransferReturn)

	End Function

#End Region

#Region "Transfer Information"

	Public Shared Function TransferInformation(ByVal iIndex As Integer) As String

		Dim oInformationReturn As New InformationReturn With {.Success = False}

		Try

			Dim oTransfer As BookingTransfer.Results.Transfer
			oTransfer = BookingBase.SearchDetails.TransferResults.Transfers(iIndex)

			Dim oTransferInformationRequest As New iVectorConnectInterface.Transfer.InformationRequest
			oTransferInformationRequest.LoginDetails = BookingBase.IVCLoginDetails
			oTransferInformationRequest.BookingToken = oTransfer.BookingToken

			Dim oTransferInformationReturn As BookingTransfer.TransferInformationReturn
			oTransferInformationReturn = BookingTransfer.TransferInformation(oTransferInformationRequest)

			Dim oInformationReturnXML As XmlDocument = Intuitive.Serializer.Serialize(oTransferInformationReturn, True)

			If Transfers.CustomSettings.PopupTemplateOverride <> "" Then
				oInformationReturn.HTML = Intuitive.XMLFunctions.XMLTransformToString(oInformationReturnXML, HttpContext.Current.Server.MapPath("~" & Transfers.CustomSettings.PopupTemplateOverride), Transfers.XSLParams("TransferInformationPopup", False))
			Else
				oInformationReturn.HTML = Intuitive.XMLFunctions.XMLStringTransformToString(oInformationReturnXML, XSL.SetupTemplate(res.TransferInformationPopup, True, True), Transfers.XSLParams("TransferInformationPopup", False))
			End If

			If oTransferInformationReturn.OK Then
				oInformationReturn.Success = True
			End If

		Catch ex As Exception
			oInformationReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Transfer", "Unable to display vehicle information")
		End Try

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oInformationReturn)

	End Function

#End Region

#Region "Update Lead Guest"

	Public Shared Function UpdateLeadGuest(ByVal sNumber As String) As String

		'set up our lead customer details on the basket from the trade
		Dim oBasket As BookingBasket = BookingBase.SearchBasket

		'set on basket
		oBasket.LeadCustomer.CustomerMobile = sNumber

		'return
		Return "Success"

	End Function

#End Region

#Region "Change Airport"

	Public Shared Function ChangeAirport(ByVal iAirportID As Integer) As String
		BookingBase.SearchBasket.BasketTransfers.Clear()
		BookingBase.SearchDetails.TransferSearchFromHotel(iAirportID, "12:00", "12:00", "", "")
		Dim oTransfersXML As XmlDocument = BookingBase.SearchDetails.TransferResults.GetResultsXML
		Dim oXSLParams As WebControls.XSL.XSLParams = XSLParams("Rates", True)
		Dim sTransferResultsHTML As String = XMLFunctions.XMLTransformToString(oTransfersXML, HttpContext.Current.Server.MapPath("~/Widgets/Transfers/Transfers.xsl"), oXSLParams)
		Return Web.Translation.TranslateHTML(sTransferResultsHTML)
	End Function

#End Region

#Region "XSL Params"

	Public Shared Function XSLParams(ByVal sTemplate As String, ByVal bShowFlightCodes As Boolean, Optional ByVal sSelectedHashToken As String = "") As Intuitive.WebControls.XSL.XSLParams
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("Title", CustomSettings.Title)
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
			.AddParam("Template", sTemplate)
			.AddParam("ShowFlightCodeFields", bShowFlightCodes)
			.AddParam("RequiresPhoneNumber", CustomSettings.RequiresPhoneNumber)
			.AddParam("HotelOnlyInitialSearch", CustomSettings.HotelOnlyInitialSearch)
			.AddParam("EnablePropertySearch", CustomSettings.EnablePropertySearch)
			.AddParam("OneWay", Functions.SafeBoolean(BookingBase.SearchDetails.TransferSearch.Oneway).ToString.ToLower)
			.AddParam("CSSClassOverride", Transfers.CustomSettings.CSSClassOverride)
			If Not BookingBase.Trade Is Nothing Then
				.AddParam("IsCommissionable", BookingBase.Trade.Commissionable)
			End If
		End With

		'if we have already searched for transfers - add times + flightcodes
		If BookingBase.SearchDetails.TransferResults.TotalTransfers > 0 Then
			Dim oTransfer As BookingTransfer.Results.Transfer = BookingBase.SearchDetails.TransferResults.Transfers.First()
			With oXSLParams
				.AddParam("CurrentOutboundTime", oTransfer.OutboundTime)
				.AddParam("CurrentReturnTime", oTransfer.ReturnTime)
				.AddParam("CurrentOutboundFlightCode", oTransfer.OutboundFlightCode)
				.AddParam("CurrentReturnFlightCode", oTransfer.ReturnFlightCode)
				.AddParam("CurrentDepartureParentID", oTransfer.DepartureParentID)
				.AddParam("CurrentDepartureParentType", oTransfer.DepartureParentType)
			End With
		End If

		'if we already have a transfer in the basked - get the selected hashcode
		If BookingBase.SearchBasket.BasketTransfers.Count > 0 Then
			Try
				'Dim sBookingToken As String = BookingBase.SearchBasket.BasketTransfers.Last().Transfer.BookingToken
				'Dim sHashToken As String = BookingBase.SearchDetails.TransferResults.Transfers.Where(Function(o) o.BookingToken = sBookingToken).First.TransferOptionHashToken
				oXSLParams.AddParam("CurrentBasketTransferHashCode", sSelectedHashToken)
			Catch
			End Try
		End If

		Return oXSLParams

	End Function

#End Region

#Region "Helpers"

	Public Sub SetupCustomSettings()
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.TextOverrides = Settings.GetValue("TextOverrides")
			.Title = Settings.GetValue("Title")
			.RequiresPhoneNumber = Settings.GetValue("RequiresPhoneNumber").ToSafeBoolean
			.TemplateOverride = Settings.GetValue("TemplateOverride")
			.HotelOnlyInitialSearch = Settings.GetValue("HotelOnlyInitialSearch").ToSafeBoolean
			.EnablePropertySearch = Settings.GetValue("EnablePropertySearch").ToSafeBoolean
			.CSSClassOverride = Settings.GetValue("CSSClassOverride").ToSafeString
			.TransferOrCarHireOnly = Settings.GetValue("TransferOrCarHireOnly").ToSafeBoolean
			.TransferOrCarHireOnlyWarning = Settings.GetValue("TransferOrCarHireOnlyWarning").ToSafeString
			.PopupTemplateOverride = Settings.GetValue("PopupTemplateOverride").ToSafeString
			.ShowResortsWithHotels = Settings.GetValue("ShowResortsWithHotels").ToSafeBoolean
		End With
		Transfers.CustomSettings = oCustomSettings
	End Sub

	Public Function GetAirportsXML() As XmlDocument
		Dim oAirportsXML As New XmlDocument

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.HotelOnly Then

			Dim iResortID As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(BookingBase.SearchBasket.XML, "BookingBasket/BasketProperties/BasketProperty/ContentXML/Hotel/GeographyLevel3ID"))
			Dim oAirports As New HotelAirports
			For Each oAirport As Lookups.Airport In Lookups.Airports
				If Lookups.AirportResortCheck(oAirport.AirportID, iResortID) Then oAirports.Airports.Add(oAirport)
			Next

			'2.2 if no airports serve the property - dont bother rendering the widget - just bomb out
			If oAirports.Airports.Count = 0 Then Return Nothing

			'2.3 If required do an initial search from the first airport for midday --TODO lazyload instead?
			If Transfers.CustomSettings.HotelOnlyInitialSearch Then
				BookingBase.SearchDetails.TransferSearchFromHotel(oAirports.Airports.First.AirportID, "12:00", "12:00", "", "")
			End If

			oAirportsXML = Serializer.Serialize(oAirports, True)

		End If

		Return oAirportsXML
	End Function

	Public Sub DrawTransform(ByVal oXML As XmlDocument, ByVal writer As System.Web.UI.HtmlTextWriter, oXSLParams As Intuitive.WebControls.XSL.XSLParams)
		If Settings.GetValue("TemplateOverride") <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & Transfers.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.Transfers, True, False), writer, oXSLParams)
		End If
	End Sub

#End Region

#Region "Helper Classes"

	Public Class AutoCompleteReturn
		Public TextBoxID As String
		Public SelectedScript As String
		Public Items As New Generic.List(Of AutoCompleteItem)

		Public Class AutoCompleteItem
			Public Value As String 'This is usually an Integer ID but don't want to restrict this
			Public Display As String

			Public Sub New()
			End Sub

			Public Sub New(ByVal sValue As String, ByVal sDisplay As String)
				Me.Value = sValue
				Me.Display = sDisplay
			End Sub
		End Class
	End Class

	Public Class RemoveTransferReturn
		Public OK As String
		Public UpdateBasket As String
	End Class

	Public Class SearchRequest
		Public PropertyReferenceID As Integer
		Public ExtraLocationType As BookingTransfer.TransferSearch.ParentType
		Public ExtraLocationID As Integer
		Public AirportID As Integer
		Public OutboundArrivalTime As String
		Public ReturnDepartureTime As String
		Public OutboundFlightCode As String
		Public ReturnFlightCode As String
	End Class

	Public Class TransferSearchReturn
		Public Success As Boolean
		Public HTML As String
		Public Warning As String
	End Class

	Public Class CustomSetting
		Public TextOverrides As String
		Public Title As String
		Public RequiresPhoneNumber As Boolean
		Public TemplateOverride As String
		Public HotelOnlyInitialSearch As Boolean = False
		Public EnablePropertySearch As Boolean = False
		Public CSSClassOverride As String
		Public TransferOrCarHireOnly As Boolean
		Public TransferOrCarHireOnlyWarning As String
		Public PopupTemplateOverride As String
		Public ShowResortsWithHotels As Boolean
	End Class

	Public Class HotelAirports
		Public Airports As New Generic.List(Of Lookups.Airport)
	End Class

	Public Class AddTransferReturn
		Public Success As Boolean
		Public Warnings As New Generic.List(Of String)
		Public ResultsHTML As String = ""
	End Class

	Public Class InformationReturn
		Public Success As Boolean
		Public Warning As String
		Public HTML As String = ""
	End Class

#End Region

End Class
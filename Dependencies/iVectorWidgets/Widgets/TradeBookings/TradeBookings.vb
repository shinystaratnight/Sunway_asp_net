Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.XMLFunctions
Imports Intuitive.Web.Widgets
Imports Intuitive.Web
Imports System.Xml

Public Class TradeBookings
	Inherits WidgetBase

	Public Shared Shadows Property BookingDocuments As Generic.List(Of BookingDocument)
		Get
			If HttpContext.Current.Session("tradebookings_bookingdocuments") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("tradebookings_bookingdocuments"), Generic.List(Of BookingDocument))
			End If
			Return New Generic.List(Of BookingDocument)
		End Get
		Set(ByVal value As Generic.List(Of BookingDocument))
			HttpContext.Current.Session("tradebookings_bookingdocuments") = value
		End Set
	End Property

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("tradebookings_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("tradebookings_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("tradebookings_customsettings") = value
		End Set

	End Property


#Region "Constants"

	Public Const CONST_Cancelled As String = "cancelled"

#End Region


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim bRemoveEarliestDepartureDate As Boolean = SafeBoolean(Settings.GetValue("RemoveEarliestDepartureDate"))

		'1 Set up the custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
			.RemoveEarliestDepartureDate = bRemoveEarliestDepartureDate
		End With
		TradeBookings.CustomSettings = oCustomSettings

		'1.1 get bookings xml
		Dim oXML As System.Xml.XmlDocument = GetBookingsXML()

		'1.2 get header overrides xml
		Dim oHeaderOverridesXML As XmlDocument = Utility.GetTextOverridesXML(Settings.GetValue("HeaderOverrides"), "HeaderOverrides")

		'2 merge xml
		oXML = MergeXMLDocuments(oXML, oHeaderOverridesXML)


		'3. set up params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("SystemCurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.Params.CurrencyID))
			.AddParam("SystemCurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.Params.CurrencyID))
			.AddParam("CSSClassOverride", Functions.SafeString(Settings.GetValue("CSSClassOverride")))
			.AddParam("CreditCardAgent", BookingBase.Trade.CreditCardAgent)
			.AddParam("Update", False)
		End With

		'4. transform

		If TradeBookings.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & TradeBookings.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXML, XSL.SetupTemplate(res.TradeBookings, True, True), writer, oXSLParams)
		End If

		'5. Add paging
		writer.AddAttribute(UI.HtmlTextWriterAttribute.Id, "divTradeBookingsFooter") 'possibly could add a content box class to this
		writer.RenderBeginTag(UI.HtmlTextWriterTag.Div)

		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = Math.Ceiling(BookingBase.SearchBookings.Bookings.iVectorConnectBookings.Count / BookingBase.Params.BookingsPerPage).ToSafeInt
			If .TotalPages = 0 Then .TotalPages = 1
			.TotalLinksToDisplay = 5
			.CurrentPage = BookingBase.SearchBookings.Bookings.CurrentPage
			.ScriptPrevious = "TradeBookings.PreviousPage();"
			.ScriptNext = "TradeBookings.NextPage();"
			.ScriptPage = "TradeBookings.SelectPage({0})"
			.ShowTotalPages = True
		End With
		oPaging.RenderControl(writer)

		writer.AddAttribute(UI.HtmlTextWriterAttribute.Class, "clear")
		writer.RenderBeginTag(UI.HtmlTextWriterTag.Div)
		writer.RenderEndTag()

		writer.RenderEndTag()

	End Sub

#Region "Booking Details"

	Public Function GetBookingsXML() As System.Xml.XmlDocument

		Dim oTradeBookingsXML As New System.Xml.XmlDocument

		'setup request
		Dim oSearchBookingsRequest As New iVectorConnectInterface.SearchBookingsRequest
		With oSearchBookingsRequest
			.LoginDetails = BookingBase.IVCLoginDetails
			.UseCustomerCurrency = BookingBase.Params.UseCustomerCurrency
		    If Not CustomSettings.RemoveEarliestDepartureDate Then
                If Me.Settings.GetValue("IncludeTravelled", true).ToSafeBoolean() then
                    .EarliestDepartureDate = Date.Parse("1901-01-01T00:00:00")
                else
                        .EarliestDepartureDate = Date.Now.AddDays(1)
                End If
		        
		    End If
		End With

		'2. Get bookings
		Dim oSearchBookingsReturn As BookingManagement.SearchBookingsReturn
		oSearchBookingsReturn = BookingManagement.SearchTradeBookings(oSearchBookingsRequest)

		oTradeBookingsXML = oSearchBookingsReturn.XML

		Return oTradeBookingsXML

	End Function

#End Region

	Public Shared Function FilterBookings(ByVal sJSONParams As String) As String

		Dim oParams As BookingsHandler.Filters = Newtonsoft.Json.JsonConvert.DeserializeObject(Of BookingsHandler.Filters)(sJSONParams)

		'setup filter object
		Dim oFilter As BookingsHandler.Filters = BookingBase.SearchBookings.Bookings.BookingsFilter
		With oFilter
			.Reference = oParams.Reference
			.GuestName = oParams.GuestName
			.BookedStartDate = oParams.BookedStartDate
			.BookedEndDate = oParams.BookedEndDate
			.ArrivalStartDate = oParams.ArrivalStartDate
			.ArrivalEndDate = oParams.ArrivalEndDate
		    .Status = oParams.Status
		End With

		'filter results
		BookingBase.SearchBookings.Bookings.FilterResults(oFilter)

		'serialize to json and return
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oFilter)
	End Function

	Public Shared Function SortBookings(ByVal sField As String, ByVal sType As String) As String

		BookingBase.SearchBookings.Bookings.SortResults(sField, sType)

		Dim oXML As System.Xml.XmlDocument = BookingBase.SearchBookings.Bookings.GetSortXML()
		
		Return ""

	End Function


#Region "Update Results"

	Public Shared Function UpdateResults(ByVal iPage As Integer) As String

		BookingBase.SearchBookings.Bookings.CurrentPage = iPage

		Dim oXML As System.Xml.XmlDocument = BookingBase.SearchBookings.Bookings.GetBookingsXML(iPage)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
		oXSLParams.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
		oXSLParams.AddParam("SystemCurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.Params.CurrencyID))
		oXSLParams.AddParam("SystemCurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.Params.CurrencyID))
		oXSLParams.AddParam("CreditCardAgent", BookingBase.Trade.CreditCardAgent)
		oXSLParams.AddParam("Update", True)

		'paging
		Dim oPaging As New Intuitive.Web.Controls.Paging
		With oPaging
			.TotalPages = Math.Ceiling(BookingBase.SearchBookings.Bookings.WorkTable.Where(Function(o) o.Display = True).Count / BookingBase.Params.BookingsPerPage).ToSafeInt
			If .TotalPages = 0 Then .TotalPages = 1
			.TotalLinksToDisplay = 5
			.CurrentPage = BookingBase.SearchBookings.Bookings.CurrentPage
			.ScriptPrevious = "TradeBookings.PreviousPage();"
			.ScriptNext = "TradeBookings.NextPage();"
			.ScriptPage = "TradeBookings.SelectPage({0})"
			.ShowTotalPages = True
		End With

		Dim sPagingHTML As String = Intuitive.Web.Translation.TranslateHTML(Functions.RenderControlToString(oPaging))

		Dim sHTML As String = ""
		If TradeBookings.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & TradeBookings.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.TradeBookings, True, True), oXSLParams)
		End If

		Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.TradeBookingsHTML = Intuitive.Web.Translation.TranslateHTML(sHTML), .PagingHTML = sPagingHTML})


	End Function

#End Region

#Region "Documentation"

    Public Shared Function GetDocumentation(ByVal sBookingReference As String, ByVal sDocument As String) As String

		Dim oViewDocumentationReturn As New ViewDocumentationReturn
		Dim oViewDocumentationResponse As New iVectorConnectInterface.ViewDocumentationResponse
		Dim iDocumentationID As Integer = 0
        iDocumentationID = GetDocumentationID(sDocument)

		Try

			Dim oViewDocumentaionRequest As New iVectorConnectInterface.ViewDocumentationRequest
			With oViewDocumentaionRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
				.BookingDocumentationID = iDocumentationID

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()
				If oWarnings.Count > 0 Then
					oViewDocumentationReturn.OK = False
					oViewDocumentationReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			If oViewDocumentationReturn.OK Then
				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.ViewDocumentationResponse)(oViewDocumentaionRequest)

				oViewDocumentationResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.ViewDocumentationResponse)

			Else
				oViewDocumentationReturn.OK = False
			End If

		Catch ex As Exception
			oViewDocumentationReturn.OK = False
			oViewDocumentationReturn.Warnings.Add(ex.ToString)
		End Try

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oViewDocumentationResponse)

	End Function

	Public Shared Function GetSendDocumentationPopupContent(ByVal sBookingReference As String) As String

		Dim oXML As New XmlDocument

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("BookingReference", sBookingReference)
		End With

		Dim sHTML As String = ""
		sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.EmailPopup, True, True), oXSLParams)

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

    Public Shared Function SendDocumentation(ByVal sBookingReference As String, ByVal sDocument As String, ByVal sOverrideEmail As String) As String

		Dim oSendDocumentationReturn As New SendDocumentationReturn
		Dim oSendDocumentationResponse As New iVectorConnectInterface.SendDocumentationResponse
		Dim iDocumentationID As Integer = 0
        iDocumentationID = GetDocumentationID(sDocument)

		Try

			Dim oSendDocumentationRequest As New iVectorConnectInterface.SendDocumentationRequest
			With oSendDocumentationRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
				.BookingDocumentationID = iDocumentationID
				If sOverrideEmail <> "" Then
					.OverrideEmailAddress = sOverrideEmail
				End If

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()
				If oWarnings.Count > 0 Then
					oSendDocumentationReturn.OK = False
					oSendDocumentationReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			If oSendDocumentationReturn.OK Then
				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.SendDocumentationResponse)(oSendDocumentationRequest)

				oSendDocumentationResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.SendDocumentationResponse)

			Else
				oSendDocumentationReturn.OK = False
			End If

		Catch ex As Exception
			oSendDocumentationReturn.OK = False
			oSendDocumentationReturn.Warnings.Add(ex.ToString)
		End Try


		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSendDocumentationResponse)
	End Function

    Public Shared Function GetDocumentationID(ByVal sDocument As String) As Integer

        Dim iDocumentationID As Integer = 0
        Dim oXML As XmlDocument = Utility.BigCXML("BookingDocumentation", 1, 60)

        If sDocument = "Trade Documentation Only" Then
            If BookingBase.Trade.Commissionable Then
                iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/CommissionableAgentDocumentationID").ToSafeInt
            Else
                iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/NetAgentDocumentationID").ToSafeInt
            End If
        Else
            iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/ClientDocumentationID").ToSafeInt
        End If

        If iDocumentationID = 0 Then
            TradeBookings.BookingDocuments = Utility.XMLToGenericList(Of BookingDocument)(oXML)

            For Each oDocument As BookingDocument In TradeBookings.BookingDocuments
                If oDocument.DocumentName = sDocument Then
                    iDocumentationID = oDocument.BookingDocumentationID
                End If
            Next
        End If

        Return iDocumentationID

    End Function

	Public Class ViewDocumentationReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class SendDocumentationReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class BookingDocument
		Public BookingDocumentationID As Integer
		Public DocumentName As String
	End Class

#End Region

#Region "Cancellations"

	Public Shared Function GetCancellationPopupDetails(ByVal sBookingReference As String) As String

		Dim oCancellationDetails As New CancellationDetails With {.BookingReference = sBookingReference}

		'1. Get Booking Cancellation Details
		Dim oPreCancelReturn As BookingManagement.PreCancelReturn = BookingManagement.RequestPreCancel(sBookingReference)
		If oPreCancelReturn.OK Then
			With oCancellationDetails
				.BookingCancellationToken = oPreCancelReturn.CancellationToken
				.CancellationCost = oPreCancelReturn.CancellationCost
			End With
		End If

		'2. Get Component Canx Details

		'2.1 - get booking details (we'll need these later - so get here to avoid making this request more than we need to)
		Dim oGetBookingDetailsResponse As iVectorConnectInterface.GetBookingDetailsResponse = TradeBookings.GetBookingDetails(sBookingReference)

		'2.2 - get component details
		Dim oPreCancelComponentResponse As iVectorConnectInterface.PreCancelComponentResponse = TradeBookings.ComponentPreCancelRequest(sBookingReference, oGetBookingDetailsResponse)
		If oPreCancelComponentResponse.ReturnStatus.Success Then
			For Each oComponent As iVectorConnectInterface.PreCancelComponentResponse.BookingComponent In oPreCancelComponentResponse.BookingComponents
				Dim oCancellationComponent As New CancellationDetails.CancellationComponent
				With oCancellationComponent
					.Component = oComponent.ComponentType
					.CancellationToken = oComponent.CancellationToken
					.CancellationCost = oComponent.CancellationCost
					.ComponentBookingID = SafeInt(oComponent.ComponentBookingID)
				End With

				Select Case oComponent.ComponentType
					Case "Property"
						oCancellationComponent.Description = TradeBookings.PropertyName(oGetBookingDetailsResponse.Properties.Where(Function(o) o.PropertyBookingID = SafeInt(oComponent.ComponentBookingID)).First.PropertyReferenceID)
					Case "Flight"
						Dim oFlight As iVectorConnectInterface.GetBookingDetailsResponse.Flight = oGetBookingDetailsResponse.Flights.Where(Function(o) o.FlightBookingID = SafeInt(oComponent.ComponentBookingID)).First
						oCancellationComponent.Description = oFlight.DepartureAirport & " to " & oFlight.ArrivalAirport
					Case "Transfer"
						oCancellationComponent.Description = oGetBookingDetailsResponse.Transfers.Where(Function(o) o.TransferBookingID = SafeInt(oComponent.ComponentBookingID)).First.Vehicle
					Case "Extra"
						oCancellationComponent.Description = oGetBookingDetailsResponse.Extras.Where(Function(o) o.ExtraBookingID = SafeInt(oComponent.ComponentBookingID)).First.ExtraName
				End Select

				oCancellationDetails.CancellationComponentDetails.Add(oCancellationComponent)
			Next
		End If

		Dim oXML As New XmlDocument
		oXML = Serializer.Serialize(oCancellationDetails)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("SellingCurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
		End With

		Dim sHTML As String = ""
		sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.CancelComponentPopup, True, True), oXSLParams)

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function

	Public Shared Function GetBookingDetails(ByVal sBookingReference As String) As iVectorConnectInterface.GetBookingDetailsResponse

		Dim oGetBookingDetailsResponse As New iVectorConnectInterface.GetBookingDetailsResponse

		Try

			'1. Create request object
			Dim oGetBookingDetailsRequest As New iVectorConnectInterface.GetBookingDetailsRequest
			With oGetBookingDetailsRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
			End With

			'2. Serialize request object
			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.GetBookingDetailsResponse)(oGetBookingDetailsRequest)

			oGetBookingDetailsResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.GetBookingDetailsResponse)

		Catch ex As Exception
			oGetBookingDetailsResponse.ReturnStatus.Success = False
			oGetBookingDetailsResponse.ReturnStatus.Exceptions.Add(ex.ToString)
		End Try


		Return oGetBookingDetailsResponse

	End Function

	Public Shared Function ComponentPreCancelRequest(ByVal sBookingReference As String, Optional ByVal oGetBookingDetailsResponse As iVectorConnectInterface.GetBookingDetailsResponse = Nothing) As iVectorConnectInterface.PreCancelComponentResponse

		Dim oPreCancelComponentResponse As New iVectorConnectInterface.PreCancelComponentResponse

		Try

			'1. Get BookingDetals
			If oGetBookingDetailsResponse Is Nothing Then oGetBookingDetailsResponse = TradeBookings.GetBookingDetails(sBookingReference)

			'2. Build component pre cancel request
			Dim oPreCancelComponentRequest As New iVectorConnectInterface.PreCancelComponentRequest
			With oPreCancelComponentRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference

				'2.1. Get Component Cancellation Details
				'2.1.1 Properties
				For Each oProperty As iVectorConnectInterface.GetBookingDetailsResponse.Property In oGetBookingDetailsResponse.Properties
					If oProperty.Status.ToLower <> CONST_Cancelled Then
						Dim oPropertyPreCancelRequest As New iVectorConnectInterface.PreCancelComponentRequest.BookingComponent With {.ComponentBookingID = oProperty.PropertyBookingID, .ComponentType = iVectorConnectInterface.PreCancelComponentRequest.eComponentType.Property.ToString}
						.BookingComponents.Add(oPropertyPreCancelRequest)
					End If
				Next
				'2.1.2 Flights
				For Each oFlight As iVectorConnectInterface.GetBookingDetailsResponse.Flight In oGetBookingDetailsResponse.Flights
					If oFlight.Status.ToLower <> CONST_Cancelled Then
						Dim oFlightPreCancelRequest As New iVectorConnectInterface.PreCancelComponentRequest.BookingComponent With {.ComponentBookingID = oFlight.FlightBookingID, .ComponentType = iVectorConnectInterface.PreCancelComponentRequest.eComponentType.Flight.ToString}
						.BookingComponents.Add(oFlightPreCancelRequest)
					End If
				Next
				'2.1.3 Transfers
				For Each oTransfer As iVectorConnectInterface.GetBookingDetailsResponse.Transfer In oGetBookingDetailsResponse.Transfers
					If oTransfer.Status.ToLower <> CONST_Cancelled Then
						Dim oTransferPreCancelRequest As New iVectorConnectInterface.PreCancelComponentRequest.BookingComponent With {.ComponentBookingID = oTransfer.TransferBookingID, .ComponentType = iVectorConnectInterface.PreCancelComponentRequest.eComponentType.Transfer.ToString}
						.BookingComponents.Add(oTransferPreCancelRequest)
					End If
				Next
				'2.1.4 Extras
				For Each oExtra As iVectorConnectInterface.GetBookingDetailsResponse.Extra In oGetBookingDetailsResponse.Extras
					If oExtra.Status.ToLower <> CONST_Cancelled Then
						Dim oExtraPreCancelRequest As New iVectorConnectInterface.PreCancelComponentRequest.BookingComponent With {.ComponentBookingID = oExtra.ExtraBookingID, .ComponentType = iVectorConnectInterface.PreCancelComponentRequest.eComponentType.Extra.ToString}
						.BookingComponents.Add(oExtraPreCancelRequest)
					End If
				Next

			End With

			'3. Serialize request object
			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.PreCancelComponentResponse)(oPreCancelComponentRequest)

			oPreCancelComponentResponse = CType(oIVCReturn.ReturnObject, iVectorConnectInterface.PreCancelComponentResponse)

		Catch ex As Exception
			oPreCancelComponentResponse.ReturnStatus.Success = False
			oPreCancelComponentResponse.ReturnStatus.Exceptions.Add(ex.ToString)
		End Try


		Return oPreCancelComponentResponse

	End Function

	Public Shared Function RequestCancellation(ByVal sBookingReference As String, ByVal nCancellationCost As Decimal, ByVal sCancellationToken As String) As String
		Dim oCancelResponse As BookingManagement.CancelReturn = BookingManagement.RequestCancel(nCancellationCost, sCancellationToken, sBookingReference)
		Return oCancelResponse.OK.ToString.ToLower
	End Function

	Public Shared Function CancelComponents(ByVal sRequest As String) As String

		Dim oCancelComponentRequest As iVectorConnectInterface.CancelComponentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject(Of iVectorConnectInterface.CancelComponentRequest)(sRequest)

		oCancelComponentRequest.LoginDetails = BookingBase.IVCLoginDetails

		Dim oCancelComponentReturn As BookingManagement.CancelComponentReturn
		oCancelComponentReturn = BookingManagement.CancelComponets(oCancelComponentRequest)

		Return oCancelComponentReturn.ToString.ToLower
	End Function



	Public Class CancellationDetails
		Public BookingReference As String = ""
		Public BookingCancellationToken As String = ""
		Public CancellationCost As Decimal = 0

		Public CancellationComponentDetails As New Generic.List(Of CancellationComponent)

		Public Class CancellationComponent
			Public Component As String 'Property/Flight/Transfer...
			Public Description As String 'PropertyName/FlightDetails/Vehicle...
			Public ComponentBookingID As Integer
			Public CancellationToken As String = ""
			Public CancellationCost As Decimal = 0
		End Class
	End Class

#End Region

	Public Shared Function PropertyName(ByVal PropertyReferenceID As Integer) As String
		Return SQL.GetValue("select Name from IVC_PropertyReference where PropertyReferenceID = {0} and LanguageID = {1}", PropertyReferenceID, BookingBase.DisplayLanguageID)
	End Function
	
	Public Class CustomSetting

		Public TemplateOverride As String
        Public RemoveEarliestDepartureDate As Boolean
        Public DaysSinceArrival As Integer

	End Class

End Class

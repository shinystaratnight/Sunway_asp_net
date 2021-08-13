Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Configuration.ConfigurationManager
Imports Intuitive.Web.BookingProperty.BasketProperty


Public Class HotelDetails
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("hotelDetails_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelDetails_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("hotelDetails_customsettings") = value
		End Set

	End Property

	Public Shared Property PageXML As XmlDocument

		Get
			If HttpContext.Current.Session("hotelDetails_PageXML") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelDetails_PageXML"), XmlDocument)
			End If
			Return New XmlDocument
		End Get
		Set(value As XmlDocument)
			HttpContext.Current.Session("hotelDetails_PageXML") = value
		End Set

	End Property

	Public Shared Property PropertyReferenceID As Integer

		Get
			If HttpContext.Current.Session("hotelDetails_PropertyReferenceID") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("hotelDetails_PropertyReferenceID"), Integer)
			End If
			Return 0
		End Get
		Set(value As Integer)
			HttpContext.Current.Session("hotelDetails_PropertyReferenceID") = value
		End Set

	End Property


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'We do not have access to the page def outside of the draw, so save these on the session.
		HotelDetails.PageXML = Me.PageDefinition.Content.XML
		HotelDetails.PropertyReferenceID = Me.PageDefinition.Content.ID
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.TemplateOverride = Functions.SafeString(Settings.GetValue("TemplateOverride"))
			.RequestRoomPopupOverride = Functions.SafeString(Settings.GetValue("RequestRoomPopupOverride"))
			.NextIconClass = Functions.SafeString(Settings.GetValue("NextIconClass"))
			.PreviousIconClass = Functions.SafeString(Settings.GetValue("PreviousIconClass"))
			.RedirectURL = (IIf(Functions.SafeString(Settings.GetValue("RedirectURL")) <> "", Functions.SafeString(Settings.GetValue("RedirectURL")), "/booking-summary"))
		End With

		HotelDetails.CustomSettings = oCustomSettings

		'Set up XSL params
		Dim oXSLParams As WebControls.XSL.XSLParams = Me.GetXSLParams()

		'Get our xml
		Dim oXml As XmlDocument = HotelDetails.GetPropertyDetailsXML

		'If template override is set, use that if not use default.
		If HotelDetails.CustomSettings.TemplateOverride <> "" Then
			Me.XSLPathTransform(oXml, HttpContext.Current.Server.MapPath("~" & HotelDetails.CustomSettings.TemplateOverride), writer, oXSLParams)
		Else
			Me.XSLTransform(oXml, XSL.SetupTemplate(res.HotelDetails, True, True), writer, oXSLParams)
		End If


	End Sub

	Public Function GetXSLParams() As WebControls.XSL.XSLParams
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams

		With oXSLParams
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("MapIconsJSON", HotelDetails.GetMapIcons())
			.AddParam("NextIconClass", CustomSettings.NextIconClass)
			.AddParam("PreviousIconClass", CustomSettings.PreviousIconClass)
			.AddParam("HotelArrivalDate", BookingBase.SearchDetails.HotelArrivalDate.ToString("dd MMMM yyy"))
			.AddParam("HotelDuration", BookingBase.SearchDetails.HotelDuration)
            .AddParam("SearchMode", BookingBase.SearchDetails.SearchMode)
            .AddParam("TotalPassengers", BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
		End With

		Return oXSLParams
	End Function

	Public Function AddRoomOptionToBasket(ByVal sJSON As String) As String

		Try

			'deserialize
			Dim oAddToBasket As New AddToBasket
			oAddToBasket = Newtonsoft.Json.JsonConvert.DeserializeObject(Of AddToBasket)(sJSON)

			'get hash tokens
			Dim HashTokens As New Generic.List(Of String)
			For Each sIndex As String In oAddToBasket.PropertyRoomIndexes
				Dim sToken As String = BookingBase.SearchDetails.PropertyResults.RoomHashToken(sIndex)
				HashTokens.Add(sToken)
			Next

			'add room(s) to basket
			BookingProperty.AddRoomsToBasket(HashTokens)


			'if flight option hash token is set then also add the flight
			If oAddToBasket.FlightOptionHashToken <> "" Then
				BookingFlight.AddFlightToBasket(oAddToBasket.FlightOptionHashToken)
			End If


			'if transfer option hash token is set then add the transfer
			If oAddToBasket.TransferOptionHashToken <> "" Then

				'get basket flight to pass through flight code and times
				Dim oFlight As New BookingFlight.BasketFlight
				If BookingBase.Basket.BasketFlights.Count > 0 Then
					oFlight = BookingBase.Basket.BasketFlights(0)
				End If

				BookingTransfer.AddTransferToBasket(oAddToBasket.TransferOptionHashToken, True, oFlight.Flight.OutboundFlightCode, oFlight.Flight.ReturnFlightCode, _
				   oFlight.Flight.OutboundArrivalTime, oFlight.Flight.ReturnDepartureTime)

			End If


			'if car hire option hash token is set then add the car hire
			If oAddToBasket.CarHireOptionHashToken <> "" Then
				BookingCarHire.AddCarHireToBasket(oAddToBasket.CarHireOptionHashToken)
			End If



		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

		Return HotelDetails.CustomSettings.RedirectURL

	End Function

#Region "Request Room"

	Public Function GetRequestRoomHTML() As String

		Try
			Dim sHTML As String = ""
			If HotelDetails.CustomSettings.RequestRoomPopupOverride <> "" Then
				sHTML = Intuitive.XMLFunctions.XMLTransformToString(New XmlDocument, HttpContext.Current.Server.MapPath("~" & HotelDetails.CustomSettings.RequestRoomPopupOverride), New Intuitive.WebControls.XSL.XSLParams)
			Else
				sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(New XmlDocument, XSL.SetupTemplate(res.HotelDetails_RequestRoomPopup, True, True), New Intuitive.WebControls.XSL.XSLParams)
			End If
			Return Intuitive.Web.Translation.TranslateHTML(sHTML)
		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

	End Function

	Public Function RequestRoom(ByVal sJSON As String) As String

		Try
			Dim oRequestRoomData As New RequestRoomData
			oRequestRoomData = Newtonsoft.Json.JsonConvert.DeserializeObject(Of RequestRoomData)(sJSON)

			SendRequestRoomEmail(oRequestRoomData)

			Return "success"
		Catch ex As Exception
			Return "Error|" & ex.ToString
		End Try

	End Function

	Public Sub SendRequestRoomEmail(ByVal oRequestRoomData As RequestRoomData)

		Dim oEmail As New Email
		Dim sb As New StringBuilder

		sb.AppendLine("Passenger Name: " & oRequestRoomData.PassengerName)
		sb.AppendLine("Passenger Email Address: " & oRequestRoomData.PassengerEmailAddress)
		sb.AppendLine("Passenger Telephone Number: " & oRequestRoomData.PassengerTelephoneNumber)
		For Each sRoomIndex As String In oRequestRoomData.RoomIndex.Split("|"c)
			If Not String.IsNullOrEmpty(sRoomIndex) Then
				Dim oProperyRoomOption As BasketPropertyRoomOption = BasketPropertyRoomOption.DeHashToken(Of BasketPropertyRoomOption)(BookingBase.SearchDetails.PropertyResults.RoomHashToken(sRoomIndex))
				sb.AppendLine("")
				sb.AppendLine("Property Name: " & oProperyRoomOption.PropertyName)
				sb.AppendLine("Property Reference ID: " & oProperyRoomOption.PropertyReferenceID)
				sb.AppendLine("Room Type: " & oProperyRoomOption.RoomType)
				sb.AppendLine("Room Type ID: " & oProperyRoomOption.PropertyRoomTypeID)
				sb.AppendLine("Meal Basis: " & Lookups.GetKeyPairValue(Lookups.LookupTypes.MealBasis, oProperyRoomOption.MealBasisID))
				sb.AppendLine("Meal Basis ID: " & oProperyRoomOption.MealBasisID)
				sb.AppendLine("Guests: " & oProperyRoomOption.GuestConfiguration.Adults & " Adult(s), " & oProperyRoomOption.GuestConfiguration.Children & " Child(ren), " & oProperyRoomOption.GuestConfiguration.Infants & " Infant(s)")
				sb.AppendLine("Start Date: " & oProperyRoomOption.ArrivalDate.ToString("D"))
				sb.AppendLine("Duration: " & oProperyRoomOption.Duration & " night(s)")
			End If
		Next

		Dim oHotelResultsXML As XmlDocument = Utility.BigCXML("HotelResults", 1, 60)
		Dim sRequestRoomEmail As String = XMLFunctions.SafeNodeValue(oHotelResultsXML, "//HotelResults/RequestRoomEmailAddress")

		With oEmail
			.SMTPHost = AppSettings("SMTPHost")
			.EmailTo = sRequestRoomEmail
			.From = "Website - " & BookingBase.Params.Domain
			.FromEmail = sRequestRoomEmail
			.Subject = "Room Request"
			.Body = sb.ToString
			.SendEmail()
		End With

	End Sub

#End Region

#Region "Tabbed Content"

	Public Function TabbedContent(ByVal Tab As ContentTab) As String

		'Grab our XML
		Dim oXml As XmlDocument = HotelDetails.GetPropertyDetailsXML

		'Set up our XSL params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = Me.GetXSLParams
		oXSLParams.AddParam("Template", Tab)

		'If an override template is set use that, if not use default.
		Dim sHTML As String = ""
		If HotelDetails.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXml, HttpContext.Current.Server.MapPath("~" & HotelDetails.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXml, XSL.SetupTemplate(res.HotelDetails, True, True), oXSLParams)
		End If

		'Translate and return html
		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function


#End Region

#Region "Change Flight"

    Public Function ChangeFlight(ByVal PropertyReferenceID As integer) As String
        BookingBase.SearchDetails.PropertyResults.ChangeFlightProperties.Add(PropertyReferenceID)
        Return Me.SelectedFlightHTML()
    End Function

    public Function KeepFlight(ByVal PropertyReferenceID As integer) As String
        BookingBase.SearchDetails.PropertyResults.ChangeFlightProperties.Remove(PropertyReferenceID)
        Return Me.SelectedFlightHTML()
    End Function

    Public Function UpdateSelectedFlight(ByVal propertyReferenceID As integer, ByVal flightToken As string) As string
        BookingBase.SearchDetails.PropertyResults.UpdateHotelSelectedFlight(propertyReferenceID, flightToken)
        Return Me.SelectedFlightHTML()
    End Function

    Public Function SelectedFlightHTML() As String

		'Grab our XML
		Dim oXml As XmlDocument = HotelDetails.GetPropertyDetailsXML

		'Set up our XSL params
		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = Me.GetXSLParams
		oXSLParams.AddParam("Template", "SelectedFlight")

		'If an override template is set use that, if not use default.
		Dim sHTML As String = ""
		If HotelDetails.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXml, HttpContext.Current.Server.MapPath("~" & HotelDetails.CustomSettings.TemplateOverride), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXml, XSL.SetupTemplate(res.HotelDetails, True, True), oXSLParams)
		End If

		'Translate and return html
		Return Intuitive.Web.Translation.TranslateHTML(sHTML)
    End Function

#End Region


#Region "Helper Functions"

	Public Shared Function GetPropertyDetailsXML() As XmlDocument

		'get hotel xml from pagedef
		Dim oHotelXML As XmlDocument = HotelDetails.PageXML

		'get result details
		Dim oResultDetailsXML As New XmlDocument
		If BookingBase.SearchDetails.PropertyResults.iVectorConnectResults.Count > 0 Then

			Dim iIndex As Integer = 0
			For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable
				If oWorkTableItem.PropertyReferenceID = HotelDetails.PropertyReferenceID Then
					iIndex = oWorkTableItem.Index
					Exit For
				End If
			Next

			oResultDetailsXML = BookingBase.SearchDetails.PropertyResults.GetSinglePropertyXML(iIndex)

		End If

		'merge xml and transform
		Dim oXML As XmlDocument = MergeXMLDocuments(oHotelXML, oResultDetailsXML)

		Return oXML

	End Function


	Public Shared Function GetMapIcons() As String

		Dim oMapIcons As Generic.List(Of MapIcon) = Utility.XMLToGenericList(Of MapIcon)(Utility.BigCXML("MapIcons", 1, 60))
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapIcons)

	End Function


#End Region


#Region "Support Classes"

	Public Class AddToBasket
		Public FlightOptionHashToken As String
		Public TransferOptionHashToken As String
		Public CarHireOptionHashToken As String
		Public PropertyRoomIndexes As New Generic.List(Of String)
	End Class

	Public Class RequestRoomData
		Public RoomIndex As String
		Public PassengerName As String
		Public PassengerEmailAddress As String
		Public PassengerTelephoneNumber As String
	End Class

	Public Enum ContentTab
		Overview
		Description
		Images
		Map
		Facilities
		Location
	End Enum

	Public Class CustomSetting
		Public TemplateOverride As String
		Public NextIconClass As String
		Public PreviousIconClass As String
		Public RequestRoomPopupOverride As String
		Public RedirectURL As String
	End Class

	Public Class MapIcon
		Public Property Name As String
		Public Property URL As String
		Public Property Width As Integer
		Public Property Height As Integer
		Public Property AnchorX As Integer
		Public Property AnchorY As Integer
		Public Property OriginX As Integer
		Public Property OriginY As Integer
	End Class

#End Region

End Class


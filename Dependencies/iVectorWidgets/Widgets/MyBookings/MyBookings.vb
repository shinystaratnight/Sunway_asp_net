Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web.Widgets
Imports Intuitive.Web
Imports ivci = iVectorConnectInterface
Imports System.Xml

Public Class MyBookings
	Inherits WidgetBase


	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("mybookings_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("mybookings_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("mybookings_customsettings") = value
		End Set
	End Property

	Public Shared Property AmendmentEmailAddress As String
		Get
			If Not HttpContext.Current.Session("usersession_amendmentemailaddress") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_amendmentemailaddress"), String)
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("usersession_amendmentemailaddress") = value
		End Set
	End Property

	Public Shared Property CustomerEmailAddress As String
		Get
			If Not HttpContext.Current.Session("usersession_customeremailaddress") Is Nothing Then
				Return CType(HttpContext.Current.Session("usersession_customeremailaddress"), String)
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			HttpContext.Current.Session("usersession_customeremailaddress") = value
		End Set
	End Property

	Public Shared Property CustomerDocumentation As DocumentationList
		Get
			Return CType(BookingBase.Session.Get("customer_docs"), DocumentationList)
		End Get
		Set(value As DocumentationList)
			BookingBase.Session.Set("customer_docs", value)
		End Set
	End Property


	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.DocumentName = Intuitive.Functions.SafeString(Settings.GetValue("DocumentName"))
			.SeatReservations = Intuitive.Functions.SafeBoolean(Settings.GetValue("SeatReservations"))
			.MakeReservationsScript = Intuitive.Functions.SafeString(Settings.GetValue("MakeReservationsScript"))
			.CSSClassOverride = Intuitive.Functions.SafeString(Settings.GetValue("CSSClassOverride"))
			.LogoutRedirectURL = Intuitive.Functions.SafeString(Settings.GetValue("LogoutRedirectURL"))
			.OverrideXSL = Intuitive.Functions.SafeString(Settings.GetValue("OverrideXSL"))
			.UseShortDates = Intuitive.Functions.SafeBoolean(Settings.GetValue("UseShortDates"))
			.PriceFormat = Intuitive.Functions.SafeString(Settings.GetValue("PriceFormat"))
			.SupressPropertyLandingPages = Intuitive.Functions.SafeBoolean(Settings.GetValue("SupressPropertyLandingPages"))
			.ThreeDSecureRedirectURL = Intuitive.Functions.SafeString(Settings.GetValue("ThreeDSecureRedirectURL"))
			.PropertyContentSource = Intuitive.Functions.SafeEnum(Of PropertyContentSource)(Settings.GetValue("PropertyContentSource"))
			.PropertyContentObjectType = Intuitive.Functions.SafeString(Settings.GetValue("PropertyContentObjectType"))
		End With

		MyBookings.CustomSettings = oCustomSettings

		If MyBookingsLogin.CustomerID = 0 AndAlso MyBookingsLogin.MyBookingsReference = "" Then
			Response.Redirect("/booking-login")
		Else
			Dim oXML As System.Xml.XmlDocument = GetBookingsXML()
			MyBookings.AmendmentEmailAddress = Me.Setting("AmendmentEmailAddress")

			Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = MyBookings.GetXSLParams("false")

			If MyBookings.CustomSettings.OverrideXSL <> "" Then
				Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath(MyBookings.CustomSettings.OverrideXSL), writer, oXSLParams)
			Else
				Me.XSLTransform(oXML, res.MyBookings, writer, oXSLParams)
			End If

		End If

	End Sub


	Public Shared Function GetXSLParams(sSeatsChanged As String) As WebControls.XSL.XSLParams

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
			.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
			.AddParam("CSSClassOverride", MyBookings.CustomSettings.CSSClassOverride)
			.AddParam("SeatReservations", MyBookings.CustomSettings.SeatReservations.ToString.ToLower)
			.AddParam("SeatReservationsChanged", "false")
			.AddParam("LogoutRedirectURL", MyBookings.CustomSettings.LogoutRedirectURL)
			.AddParam("Theme", BookingBase.Params.Theme)
			.AddParam("UseShortDates", MyBookings.CustomSettings.UseShortDates)
			.AddParam("PriceFormat", MyBookings.CustomSettings.PriceFormat)
			.AddParam("SupressPropertyLandingPages", MyBookings.CustomSettings.SupressPropertyLandingPages)
		End With
		Return oXSLParams

	End Function


	Public Shared Function UpdateMyBookingsHTML(ByVal sSeatsChanged As String) As String

		Dim oXML As System.Xml.XmlDocument = GetBookingsXML()

		Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = MyBookings.GetXSLParams(sSeatsChanged.ToLower)

		Dim sHTML As String

		If MyBookings.CustomSettings.OverrideXSL <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, HttpContext.Current.Server.MapPath(MyBookings.CustomSettings.OverrideXSL), oXSLParams)
		Else
			sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, res.MyBookings, oXSLParams)
		End If

		Return Intuitive.Web.Translation.TranslateHTML(sHTML)

	End Function


#Region "Booking Details"

	Public Shared Function GetBookingsXML(Optional ByVal CustomerID As Integer = 0, Optional ByVal BookingReference As String = "", Optional ByVal BrandIDs As Generic.List(Of Integer) = Nothing) As System.Xml.XmlDocument

		CustomerID = IIf(CustomerID = 0, MyBookingsLogin.CustomerID, CustomerID)
		BookingReference = IIf(BookingReference = "", MyBookingsLogin.MyBookingsReference, BookingReference)

		Dim oBookingReferences As New Generic.List(Of String)

		Dim oMyBookingsXML As New System.Xml.XmlDocument
		oMyBookingsXML.AppendChild(oMyBookingsXML.CreateNode(System.Xml.XmlNodeType.Element, "MyBookings", ""))

		If CustomerID <> 0 Then
			Dim oSearchBookingsReturn As BookingManagement.SearchBookingsReturn = BookingManagement.SearchBookings(CustomerID, BrandIDs)
			oBookingReferences = oSearchBookingsReturn.BookingReferences
		ElseIf BookingReference <> "" Then
			oBookingReferences.Add(BookingReference)
		End If

		For Each sBookingReference As String In oBookingReferences

			Dim oBookingDetailsReturn As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(sBookingReference)

			If oBookingDetailsReturn.PropertyReferenceID > 0 Then

				' get property details
				Dim oPropertyXML As System.Xml.XmlDocument = Utility.BigCXML("Property", oBookingDetailsReturn.PropertyReferenceID, 0)

				' insert xml nodes
				XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Name", XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name"))
				XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Rating", XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Rating"))
				XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "URL", XMLFunctions.SafeNodeValue(oPropertyXML, "Property/URL"))
				XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "MainImage", XMLFunctions.SafeNodeValue(oPropertyXML, "Property/MainImage"))
				XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Resort", XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Resort"))

				'Get the meal basis names from ids
				For Each RoomNode As XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/Properties/Property/Rooms/Room")
					Dim MealBasisNode As XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(XmlNodeType.Element, "MealBasis", "")
					MealBasisNode.InnerText = BookingBase.Lookups.NameLookup("/Lookups/MealBasiss/MealBasis", "MealBasis", "MealBasisID", Functions.SafeInt(XMLFunctions.SafeNodeValue(RoomNode, "MealBasisID")))
					RoomNode.AppendChild(MealBasisNode)
				Next

				'Get extra type name from lookups and inject
				For Each ExtraNode As XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/Extras/Extra")
					Dim iExtraTypeID As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(ExtraNode, "ExtraTypeID"))
					Dim ExtraTypeNode As XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(XmlNodeType.Element, "ExtraType", "")
					ExtraTypeNode.InnerText = BookingBase.Lookups.NameLookup(Intuitive.Web.Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", iExtraTypeID)
					ExtraNode.AppendChild(ExtraTypeNode)

					Dim iExtraDurationID As Integer = Functions.SafeInt(XMLFunctions.SafeNodeValue(ExtraNode, "ExtraDurationID"))
					Dim ExtraDurationNode As XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(XmlNodeType.Element, "ExtraDuration", "")
					ExtraDurationNode.InnerText = BookingBase.Lookups.NameLookup(Intuitive.Web.Lookups.LookupTypes.ExtraDuration, "ExtraDuration", "ExtraDurationID", iExtraDurationID)
					ExtraNode.AppendChild(ExtraDurationNode)
				Next

			End If

			' Add the CMS Car Hire image to the booking xml
			For Each CarHireBookingNode As XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/CarHireBookings/CarHireBooking")
				Dim CarTypeXML As System.Xml.XmlDocument = Utility.BigCXML("CarTypes", SafeInt(XMLFunctions.SafeNodeValue(CarHireBookingNode, "CarTypeID")), 60)
				Dim CMSVehicleImageNode As XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(XmlNodeType.Element, "CMSVehicleImage", "")
				CMSVehicleImageNode.InnerText = SafeString(XMLFunctions.SafeNodeValue(CarTypeXML, "CarTypes/CarType/Image"))
				CarHireBookingNode.AppendChild(CMSVehicleImageNode)
			Next

			oMyBookingsXML = Intuitive.XMLFunctions.MergeXMLDocuments(oMyBookingsXML, oBookingDetailsReturn.BookingXML)

		Next

		Return oMyBookingsXML

	End Function

#End Region


#Region "Seat Maps"

	Public Shared Function AddSeatsToBasket(ByVal sBookingReference As String, ByVal iFlightBookingID As Integer, ByVal nOriginalPrice As Decimal, ByVal sJSON As String) As String

		'get extras from json string
		Dim oExtras As Extras = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Extras)(sJSON)

		'add a new basket flight with the extras
		BookingFlight.AddFlightToBasket(sBookingReference, iFlightBookingID, nOriginalPrice, oExtras.BasketFlightExtras)

		Dim sCompleteFunction As String = ""

		'check if there's anything to pay
		If BookingBase.Basket.AmountDueToday > 0 Then
			sCompleteFunction = "function() { " + MyBookings.CustomSettings.MakeReservationsScript + "}"
		Else
			'just reserve the seats
			Dim oReserveSeatsReturn As Intuitive.Web.BookingManagement.ReserveSeatsReturn = BookingManagement.ReserveSeats()

			If oReserveSeatsReturn.OK Then
				sCompleteFunction = "function() { MyBookings.ReserveSeatsDone('');}"
			Else
				sCompleteFunction = "function() { MyBookings.ReserveSeatsDone('Sorry, there was a problem reserving your seats. Please try again or choose an alternative option.');}"
			End If

		End If

		Return sCompleteFunction

	End Function

#End Region

#Region "Booking Actions"

#Region "Amend Booking"

	Public Function RequestAmendment(ByVal BookingReference As String, ByVal CheckInDate As Date,
	  ByVal CheckOutDate As Date, ByVal Destination As String, ByVal HotelName As String,
	  ByVal MaxTotalRate As Integer, ByVal AdditionalInformation As String) As String

		'build email body
		Dim sb As New StringBuilder

		sb.Append("Amendment Request").AppendLine()
		sb.Append("----------------------------").AppendLine().AppendLine()

		sb.AppendFormat("Booking Reference: {0}", BookingReference).AppendLine()
		sb.AppendFormat("Check In Date: {0}", CheckInDate.ToString("dd MMM yyyy")).AppendLine()
		sb.AppendFormat("Check Out Date: {0}", CheckOutDate.ToString("dd MMM yyyy")).AppendLine()
		sb.AppendFormat("Destination: {0}", Destination).AppendLine()
		sb.AppendFormat("Hotel Name: {0}", HotelName).AppendLine()
		sb.AppendFormat("Max Total Rate: {0}", MaxTotalRate).AppendLine().AppendLine()

		'add additional information if set
		If AdditionalInformation <> "" Then
			sb.Append("Additional Information").AppendLine()
			sb.Append("----------------------------").AppendLine().AppendLine()
			sb.Append(AdditionalInformation)
		End If

		'setup and send email
		Dim oEmail As New Intuitive.Email
		With oEmail
			.SMTPHost = BookingBase.Params.SMTPHost
			.Subject = "Amendment Request - Booking " & BookingReference
			.From = "Intuitive"
			.FromEmail = "intuitive@ivector.co.uk"
			.EmailTo = MyBookings.AmendmentEmailAddress
			.Body = sb.ToString
		End With

		If oEmail.SendEmail() Then
			Return "Success"
		Else
			Return "Failed"
		End If

	End Function

#End Region

#Region "Booking Cancellation"

	Public Shared Function PreCancel(ByVal sBookingReference As String) As String

		Dim oPreCancelReturn As BookingManagement.PreCancelReturn = BookingManagement.RequestPreCancel(sBookingReference)
		Dim sJSONReturn As String = Newtonsoft.Json.JsonConvert.SerializeObject(oPreCancelReturn)
		Return sJSONReturn

	End Function

	Public Shared Function CancelBooking(ByVal CancellationCost As Decimal, ByVal CancellationToken As String, ByVal sBookingReference As String) As String

		Dim oCancelReturn As BookingManagement.CancelReturn = BookingManagement.RequestCancel(CancellationCost, CancellationToken, sBookingReference)
		Dim sJSONReturn As String = Newtonsoft.Json.JsonConvert.SerializeObject(oCancelReturn)
		Return sJSONReturn

	End Function

#End Region

#Region "Documentation"

	''' <summary>
	''' This function should never be called directly from the browser - it gets the public URL of customer documentation that
	''' should never be exposed on the front end eg lch.ivector.co.uk/bookingdocumentation/2234345.pdf
	''' This can easily be changed to view another customers documentation
	''' </summary>
	''' <param name="key">
	''' the key that maps to the public URL of a booking document internally - will be stored on the session by the ShowDocumentation function below 
	''' The ShowDocumentation function is the one that should be called from the browser
	''' </param>
	''' <returns>
	''' a one off URL stored on the session that will return the PDF booking document without exposing the ivector URL
	''' </returns>
	''' <remarks></remarks>
	Public Shared Function GetDocumentationURL(key As String) As String

		Dim oDocList As DocumentationList = MyBookings.CustomerDocumentation
		Dim url As String = ""

		Try
			url = oDocList.Find(Function(o) o.Key = key).URL
		Catch ex As Exception
			'either the request has been made without logging in and requesting the docs already or the session has died
		End Try

		Return url

	End Function


	Public Function ShowDocumentation(ByVal sBookingReference As String, Optional ByVal iBookingDocumentationID As Integer = 0) As String

		Dim iDocumentationID As Integer = IIf(iBookingDocumentationID <> 0, iBookingDocumentationID, GetDocumentationID())
		Dim oViewDocumentationReturn As BookingManagement.ViewDocumentationReturn = BookingManagement.ViewBookingDocumentationRequest(sBookingReference, iDocumentationID)

		Dim oCustomerDocs As New DocumentationList
		Dim oDocumentKeyList As New List(Of String)


		If Not oViewDocumentationReturn.OK Then
			Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.OK = False})
		End If

		If oViewDocumentationReturn.DocumentPaths IsNot Nothing Then
			For Each oDocumentationURL As String In oViewDocumentationReturn.DocumentPaths
				Dim oDocumentation As New Documentation
				oDocumentation.URL = oDocumentationURL
				oDocumentation.Key = Intuitive.Security.Functions.GenerateFixedLengthAlphanumeric(10)

				oDocumentKeyList.Add(oDocumentation.Key)
				oCustomerDocs.Add(oDocumentation)
			Next
		End If


		'set on session so we can easily access securely
		MyBookings.CustomerDocumentation = oCustomerDocs


		'return list of keys to the browser - so it can request the list of docs available securely without exposing actual URL
		Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.OK = True, .Keys = oDocumentKeyList})

	End Function

	Public Function SendDocumentation(ByVal sBookingReference As String, Optional ByVal sOverrideEmail As String = "") As String

		Dim iDocumentationID As Integer = GetDocumentationID()

		Dim oSendDocumentationReturn As BookingManagement.SendDocumentationReturn = BookingManagement.SendBookingDocumentationRequest(sBookingReference, iDocumentationID, sOverrideEmail)
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oSendDocumentationReturn)
	End Function

	Public Shared Function GetDocumentationID() As Integer

		Dim sDocumentationName As String = MyBookings.CustomSettings.DocumentName

		Return BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.BookingDocumentation, sDocumentationName)

	End Function


#End Region

#End Region

	Public Class Extras
		Public BasketFlightExtras As New Generic.List(Of BookingFlight.BasketFlight.BasketFlightExtra)
	End Class

	Public Sub LogOut()
		MyBookingsLogin.CustomerID = 0
		MyBookingsLogin.MyBookingsReference = ""
		BookingBase.LoggedIn = False
	End Sub

	Public Class CustomSetting
		Public DocumentName As String
		Public SeatReservations As Boolean
		Public MakeReservationsScript As String
		Public CSSClassOverride As String
		Public LogoutRedirectURL As String
		Public OverrideXSL As String
		Public UseShortDates As Boolean
		Public PriceFormat As String
		Public SupressPropertyLandingPages As Boolean
		Public ThreeDSecureRedirectURL As String
		Public PropertyContentSource As PropertyContentSource
		Public PropertyContentObjectType As String
		Public UseOffsitePayment As Boolean
	End Class

	Public Enum PropertyContentSource
		PropertyID
		PropertyReferenceID
	End Enum

	'this should never be shared
	Public Class DocumentationList
		Inherits Generic.List(Of Documentation)
	End Class

	Public Class Documentation
		Public URL As String
		Public Key As String
	End Class

End Class
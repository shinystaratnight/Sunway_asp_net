Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports Intuitive
Imports System.Linq
Imports System.ComponentModel
Imports iVectorConnectInterface

Public Class BookingConfirmation
    Inherits WidgetBase

    Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

        'redirect to homepage if no booking reference is set
        If BookingBase.Basket.BookingReference = "" Then
            Response.Redirect("/")
        End If

        Dim oBookingDetailsReturn As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(BookingBase.Basket.BookingReference)

        Dim oRoomViews As New RoomViewLookupList

        If oBookingDetailsReturn.OK Then

            If oBookingDetailsReturn.PropertyReferenceID > 0 Then

                ' get property details
                Dim oPropertyXML As System.Xml.XmlDocument = Utility.BigCXML("Property", oBookingDetailsReturn.PropertyReferenceID, 0)

                ' insert xml nodes
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Name", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Rating", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Rating"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "MainImage", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/MainImage"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Resort", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Resort"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Region", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Region"))

                ' Get the meal basis names from ids
                For Each RoomNode As System.Xml.XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/Properties/Property/Rooms/Room")
                    Dim MealBasisNode As System.Xml.XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(System.Xml.XmlNodeType.Element, "MealBasis", "")
                    MealBasisNode.InnerText = BookingBase.Lookups.NameLookup(Web.Lookups.LookupTypes.MealBasis, "MealBasis", "MealBasisID", Intuitive.ToSafeInt(Intuitive.XMLFunctions.SafeNodeValue(RoomNode, "MealBasisID")))
                    RoomNode.AppendChild(MealBasisNode)
                Next

                For Each Room As iVectorConnectInterface.GetBookingDetailsResponse.Room In oBookingDetailsReturn.BookingDetails.Properties(0).Rooms
                    oRoomViews.AddNewLookup(Room.RoomViewID)
                Next

                'get booking country name
                Dim iBookingCountryID As Integer = Intuitive.Functions.SafeInt(Intuitive.XMLFunctions.SafeNodeValue(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/LeadCustomer/CustomerBookingCountryID"))
                Dim sBookingCountry As String = BookingBase.Lookups.NameLookup(Web.Lookups.LookupTypes.BookingCountry, "BookingCountry", "BookingCountryID", iBookingCountryID)
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/LeadCustomer", "CustomerBookingCountry", sBookingCountry)

            End If

            ' Add the CMS Car Hire image to the booking xml
            For Each CarHireBookingNode As XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/CarHireBookings/CarHireBooking")
                Dim CarTypeXML As System.Xml.XmlDocument = Utility.BigCXML("CarTypes", Intuitive.Functions.SafeInt(XMLFunctions.SafeNodeValue(CarHireBookingNode, "CarTypeID")), 60)
                Dim CMSVehicleImageNode As XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(XmlNodeType.Element, "CMSVehicleImage", "")
                CMSVehicleImageNode.InnerText = Intuitive.Functions.SafeString(XMLFunctions.SafeNodeValue(CarTypeXML, "CarTypes/CarType/Image"))
                CarHireBookingNode.AppendChild(CMSVehicleImageNode)
            Next

            Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("TradeSite", GetSetting(eSetting.TradeSite))
				.AddParam("SummaryTitle", GetSetting(eSetting.SummaryTitle))
				.AddParam("DetailedPriceBreakdown", GetSetting(eSetting.DetailedPriceBreakdown))
				.AddParam("SummaryIncludeDates", GetSetting(eSetting.SummaryIncludeDates))
				.AddParam("PriceFormat", Intuitive.Functions.IIf(GetSetting(eSetting.PriceFormat) = "", "###,##0.00", GetSetting(eSetting.PriceFormat)))
				.AddParam("SubText", GetSetting(eSetting.SubText))
				.AddParam("CMSBaseURL", BookingBase.Params.CMSBaseURL)
				.AddParam("CommissionableAgent", Intuitive.Functions.SafeBoolean(BookingBase.Trade.Commissionable))
			End With

			Dim oXML As XmlDocument = oBookingDetailsReturn.BookingXML
			Dim oConfirmationXml As XmlDocument = Utility.BigCXML("Confirmation", 1, 60)
			Dim oExtraTypesXml As XmlDocument = Utility.BigCXML("ExtraTypes", 1, 60)
			Dim depotXml As XmlDocument = Serializer.Serialize(Lookups.CarHireDepots, True)
			
			Dim oComponentFailedXML As System.Xml.XmlDocument = New XmlDocument()

			if GetSetting(eSetting.CmsSource) <> "" then
				oComponentFailedXML = Utility.BigCXML(GetSetting(eSetting.CmsSource), 1, 100)
			End If

			oXML = XMLFunctions.MergeXMLDocuments(oXML, Serializer.Serialize(BookingBase.Basket), Serializer.Serialize(oRoomViews), oConfirmationXml, oExtraTypesXml, depotXml, oComponentFailedXML)

			'transform
			Dim overrideTemplate As String = GetSetting(eSetting.TemplateOverride)

			If overrideTemplate <> "" Then
				Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & overrideTemplate), writer, oXSLParams)
			Else
				Me.XSLTransform(oXML, res.BookingConfirmation, writer, oXSLParams)
			End If

		End If

	End Sub

	'THIS IS A TEMPORARY SOLUTION AS WE DO NO GET THE ROOM VIEW TEXT IN THE IVC RESPONSE YET.
	Public Class RoomViewLookup
		Property RoomViewID As Integer
		Property RoomView As String
	End Class

	Public Class RoomViewLookupList
		Property RoomViews As New Generic.List(Of RoomViewLookup)

		Public Sub AddNewLookup(ByVal RoomTypeID As Integer)

			If RoomTypeID <> 0 AndAlso RoomViews.Where(Function(o) o.RoomViewID = RoomTypeID).Count = 0 Then
				Dim oRoomViewLookup As New RoomViewLookup
				oRoomViewLookup.RoomViewID = RoomTypeID
				oRoomViewLookup.RoomView = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.RoomView, RoomTypeID)
				Me.RoomViews.Add(oRoomViewLookup)
			End If

		End Sub

	End Class

#Region "Settings"

	Public Enum eSetting
		<Title("Summary Title")>
		<Description("Main title text for booking confirmation section")>
		SummaryTitle

		<Title("Sub Text")>
		<Description("Optional text to show below the title")>
		SubText

		<Title("Detailed Price Breakdown Boolean")>
		<Description("Boolean to specify whether or not to show a detailed price break down")>
		DetailedPriceBreakdown

		<Title("Summary Include Dates Boolean")>
		<Description("Boolean to specify whether or not to show the departure date")>
		SummaryIncludeDates

		<Title("Trade Site Boolean")>
		<Description("Boolean to specify if this is a b2b site")>
		<DeveloperOnly(True)>
		TradeSite

		<Title("Price Format")>
		<Description("XSL number format eg ###,##0 to be used for prices")>
		<DeveloperOnly(True)>
		PriceFormat

		<Title("TemplateOverride")>
		<Description("Main title text for booking confirmation section")>
		<DeveloperOnly(True)>
		TemplateOverride

		<Title("CmsSource")>
		<Description("BigC object type used to import cms xml")>
		<DeveloperOnly(True)>
		CmsSource

	End Enum

#End Region

End Class
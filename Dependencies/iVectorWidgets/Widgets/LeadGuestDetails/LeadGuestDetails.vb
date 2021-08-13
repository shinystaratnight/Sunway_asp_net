Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports System.Xml

Public Class LeadGuestDetails
	Inherits WidgetBase


	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("leadguestdetails_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("leadguestdetails_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("leadguestdetails_customsettings") = value
		End Set

	End Property


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.BasketType = Intuitive.Functions.SafeEnum(Of eBasketType)(Settings.GetValue("BasketType"))
			.Title = Functions.SafeString(Settings.GetValue("Title"))
		End With
		LeadGuestDetails.CustomSettings = oCustomSettings

		'2.build control if it's not trade
		If Settings.GetValue("IsTrade") = "True" Then

			'set up our lead customer details on the basket from the trade
			Dim oBasket As BookingBasket = Functions.IIf(LeadGuestDetails.CustomSettings.BasketType = eBasketType.Search, BookingBase.SearchBasket, BookingBase.Basket)

			Dim oLeadGuestDetails As New iVectorConnectInterface.Support.LeadCustomerDetails
			oLeadGuestDetails.CustomerEmail = BookingBase.Trade.Email
			oLeadGuestDetails.CustomerAddress1 = BookingBase.Trade.Address1
			oLeadGuestDetails.CustomerAddress2 = BookingBase.Trade.Address2
			oLeadGuestDetails.CustomerPostcode = BookingBase.Trade.Postcode
			oLeadGuestDetails.CustomerTownCity = BookingBase.Trade.TownCity
			oLeadGuestDetails.CustomerBookingCountryID = BookingBase.Trade.BookingCountryID
			oLeadGuestDetails.CustomerPhone = Functions.IIf(oBasket.LeadCustomer.CustomerPhone <> "", oBasket.LeadCustomer.CustomerPhone, BookingBase.Trade.Telephone)
            oLeadGuestDetails.CustomerMobile = Functions.IIf(oBasket.LeadCustomer.CustomerMobile <> "", oBasket.LeadCustomer.CustomerMobile, "")

			oLeadGuestDetails.CustomerTitle = oBasket.LeadCustomer.CustomerTitle
			oLeadGuestDetails.CustomerFirstName = oBasket.LeadCustomer.CustomerFirstName
			oLeadGuestDetails.CustomerLastName = oBasket.LeadCustomer.CustomerLastName

			'set on basket
			oBasket.LeadCustomer = oLeadGuestDetails

		Else

			'build control
			If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")
			Dim sControlPath As String

			If Me.URLPath.EndsWith(".ascx") Then
				sControlPath = Me.URLPath
			Else
				sControlPath = Me.URLPath & "/leadguestdetails.ascx"
			End If

			Dim oControl As New Control
			Try
				oControl = Me.Page.LoadControl(sControlPath)
			Catch ex As Exception
				oControl = Me.Page.LoadControl("/widgetslibrary/LeadGuestDetails/LeadGuestDetails.ascx")
			End Try
			CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
			 Me.DrawControl(writer, oControl)

		End If

	End Sub


	Public Shared Function AddDetailsToBasket(ByVal sJSON As String) As String

		Dim oBasket As BookingBasket = Functions.IIf(LeadGuestDetails.CustomSettings.BasketType = eBasketType.Search, BookingBase.SearchBasket, BookingBase.Basket)

		'setup lead guest details from json
		Dim oLeadGuestDetails As New iVectorConnectInterface.Support.LeadCustomerDetails
		oLeadGuestDetails = Newtonsoft.Json.JsonConvert.DeserializeObject(Of iVectorConnectInterface.Support.LeadCustomerDetails)(sJSON)

        'this is poor but unfortunately gets set on a different widget so rip it out here
        oLeadGuestDetails.ContactCustomer = BookingBase.Basket.LeadCustomer.ContactCustomer

		'We do not always validate this field, but it is required, so if its not present, add a "."
		If oLeadGuestDetails.CustomerPostcode = "" Then oLeadGuestDetails.CustomerPostcode = "."

		'set on basket
		oBasket.LeadCustomer = oLeadGuestDetails


		'return so we know it is complete
		Return "Success"

	End Function


	Public Shared Function FindAddresses(ByVal Postcode As String) As String

		'get addresses
		Dim PostcodeLookup As New Intuitive.Web.PostcodeLookup(Postcode, Intuitive.Web.BookingBase.Params.SellingGeographyLevel1ID)
		Dim Addresses As Generic.List(Of Intuitive.Web.PostcodeLookup.Address) = PostcodeLookup.GetAddresses()

		Dim sb As New StringBuilder

		If Addresses.Count > 0 Then
			sb.Append("<option ml=""Lead Guest Details"" value=""0"">Please select... </option>")
			For Each Address As PostcodeLookup.Address In Addresses
				sb.Append("<option value=""")
				sb.Append(Address.BuildingID)
				sb.Append(""">")
				sb.Append(Address.Description)
				sb.Append("</option>")
			Next
		End If

		Return sb.ToString
	End Function


	Public Shared Function SelectAddress(ByVal BuildingID As Integer) As String

		Dim oAddress As PostcodeLookup.Address = PostcodeLookup.GetSingleAddress(BuildingID)

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oAddress)
	End Function


	Public Class CustomSetting
		Public BasketType As eBasketType
		Public Title As String
	End Class


	Public Enum eBasketType
		Search
		Main
	End Enum


End Class




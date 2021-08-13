Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports Intuitive.Functions
Imports System.Linq

Public Class CompleteBooking
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting
		Get
			If HttpContext.Current.Session("completebooking_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("completebooking_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("completebooking_customsettings") = value
		End Set
	End Property

	Public Shared Property Get3DSecureRedirectDetails As Booking3DSecure.Get3DSecureRedirectReturn
		Get
			If HttpContext.Current.Session("redirect_get3dsecureredirectreturn") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("redirect_get3dsecureredirectreturn"), Booking3DSecure.Get3DSecureRedirectReturn)
			End If
			Return New Booking3DSecure.Get3DSecureRedirectReturn
		End Get
		Set(value As Booking3DSecure.Get3DSecureRedirectReturn)
			HttpContext.Current.Session("redirect_get3dsecureredirectreturn") = value
		End Set
	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.BasketType = Settings.GetValue("BasketType")
			.SearchToMainBasket = Intuitive.Functions.SafeBoolean(Settings.GetValue("SearchToMainBasket"))
			.SetupBasketGuestsFromRooms = Intuitive.Functions.SafeBoolean(Settings.GetValue("SetupBasketGuestsFromRooms"))
			.UsePaymentAuthorisation = Intuitive.Functions.SafeBoolean(Settings.GetValue("UsePaymentAuthorisation"))
		End With

		CompleteBooking.CustomSettings = oCustomSettings

		'2. build control
		If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")

		Dim sControlPath As String = Me.URLPath

		If Not sControlPath.ToLower.Contains(".ascx") Then
			sControlPath = Me.URLPath & "/completebooking.ascx"
		End If

		Dim oControl As New Control
		Try
			oControl = Me.Page.LoadControl(sControlPath)
		Catch ex As Exception
			oControl = Me.Page.LoadControl("/widgetslibrary/CompleteBooking/CompleteBooking.ascx")
		End Try
		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)

		Me.DrawControl(writer, oControl)

	End Sub

	Public Shared Function SearchToMainBasket() As String
		Try
			'copy search basket to main basket
			BookingBase.SearchToMainBasket()
			Return "Success"
		Catch ex As Exception
			Return "Fail"
		End Try
	End Function

	Public Shared Function UpdatePrice() As String

		Dim oBasket As BookingBasket = Intuitive.Functions.IIf(CustomSettings.BasketType = "Main", BookingBase.Basket, BookingBase.SearchBasket)
		Dim oReturn As New UpdateTotalPriceReturn

		Dim sCurrencySymbol As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
		Dim sCurrencyPosition As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)

		With oReturn
			.TotalPrice = oBasket.TotalPrice
			.CurrencySymbol = sCurrencySymbol
			.CurrencyPosition = sCurrencyPosition
		End With

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)

	End Function

	Public Class UpdateTotalPriceReturn
		Public TotalPrice As Decimal
		Public CurrencySymbol As String
		Public CurrencyPosition As String
	End Class

	Public Shared Function PreBook() As String

		'copy search to main basket if set
		If CompleteBooking.CustomSettings.SearchToMainBasket Then
			BookingBase.SearchToMainBasket()
		End If

		If CompleteBooking.CustomSettings.SetupBasketGuestsFromRooms Then
			BookingBase.Basket.SetupBasketGuestsFromRooms()
		End If

		Return MainPreBook()

	End Function

	Public Shared Function PreBookNoCopy() As String

		Return MainPreBook()

	End Function

	'moved common functionality from prebook and prebooknocopy here
	Private Shared Function MainPreBook() As String

		Dim nPrePreBookPrice As Decimal = BookingBase.Basket.TotalPrice

		Dim oPreBookReturn As BookingBasket.PreBookReturn = BookingBase.Basket.PreBook()

		Dim nShownBookingAdjustmentValue As Decimal = 0

		If Not BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes Is Nothing Then
			For Each oAdjustment As iVectorConnectInterface.BookingAdjustmentSearchResponse.BookingAdjustmentType In BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes
				For Each oBookingAdjustment As iVectorConnectInterface.Basket.PreBookResponse.BookingAdjustment In BookingBase.Basket.BookingAdjustments
					If oAdjustment.AdjustmentType = oBookingAdjustment.AdjustmentType AndAlso oAdjustment.AdjustmentAmount = oBookingAdjustment.AdjustmentAmount Then
						nShownBookingAdjustmentValue += oAdjustment.AdjustmentAmount
					End If
				Next
			Next
		End If

		'price change check
		'the sumcomponent price is used as the check because of PWCC calculations
		'any issue with this please discuss with Adam.

		If oPreBookReturn.OK AndAlso Not BookingBase.Params.SuppressPriceChangeWarning AndAlso
			Math.Round(nPrePreBookPrice + nShownBookingAdjustmentValue, 2, MidpointRounding.AwayFromZero) <> BookingBase.Basket.TotalPrice Then
			Dim dPriceChange As Decimal = Math.Round(BookingBase.Basket.TotalPrice - nPrePreBookPrice, 2, MidpointRounding.AwayFromZero)
			oPreBookReturn.RedirectString = Intuitive.Functions.SafeString("?warn=pricechange&amount=" & dPriceChange)
		End If

		'return
		If oPreBookReturn.OK Then
			Return Newtonsoft.Json.JsonConvert.SerializeObject(oPreBookReturn)
		Else
			oPreBookReturn.RedirectString = "?warn=prebookfailed"
			Return Newtonsoft.Json.JsonConvert.SerializeObject(oPreBookReturn)
		End If

	End Function

	Public Shared Function Book() As String

		'create return object
		Dim oBookReturn As New Intuitive.Web.BookingBasket.BookReturn

		Dim bBooked As Boolean = False

		'check if we're using any third party payment authorisation
		If CompleteBooking.CustomSettings.UsePaymentAuthorisation Then

			Dim oStoreBasket As New StoreBasket.StoreBasketReturn
			Dim oStoreBasketRequest As New StoreBasket.StoreBasketDetails
			oStoreBasketRequest.BasketXML = BookingBase.Basket.XML.InnerXml.ToString

			oStoreBasket = StoreBasket.StoreBasket(oStoreBasketRequest)

			Dim o3DSecureReturnDetails As New Booking3DSecure.Get3DSecureRedirectDetails
			With o3DSecureReturnDetails
				.BookingReference = SafeString(oStoreBasket.BasketStoreID)
				.PaymentDetails = BookingBase.Basket.PaymentDetails
			End With

			Dim o3DSecureRedirectReturn As New Booking3DSecure.Get3DSecureRedirectReturn
			o3DSecureRedirectReturn = Booking3DSecure.Get3DSecureRedirect(o3DSecureReturnDetails)

			'check if there are no errors from connect, if so, throw an error because we dont want to take any chances
			If o3DSecureRedirectReturn.Success Then

				'check if the card requires authentication, if not carry on and make the booking as normal
				If o3DSecureRedirectReturn.RequiresSecureAuthenticationSuccess Then

					'check if the card is secure enrolled, if so, go off to third party
					If o3DSecureRedirectReturn.Enrollment Then

						CompleteBooking.Get3DSecureRedirectDetails = o3DSecureRedirectReturn

						'Update the payment token that has been returned
						BookingBase.Basket.PaymentDetails.PaymentToken = o3DSecureRedirectReturn.PaymentToken

						'regenerate the basket xml via booking base
						oStoreBasketRequest.BasketXML = BookingBase.Basket.XML.InnerXml.ToString

						'Update the stored basket with the updates
						oStoreBasketRequest.BasketStoreID = oStoreBasket.BasketStoreID
						StoreBasket.StoreBasket(oStoreBasketRequest)

						oBookReturn.SecureEnrollment = True

						Return Newtonsoft.Json.JsonConvert.SerializeObject(oBookReturn)

					Else
						'if not secure enrolled - make booking
						oBookReturn = BookingBase.Basket.Book()
						bBooked = True
					End If

				Else
					'if doesn't require auth - make booking
					oBookReturn = BookingBase.Basket.Book()
					bBooked = True
				End If

			Else
				oBookReturn.OK = False
				oBookReturn.RedirectString = "?warn=authfailed"
				Return Newtonsoft.Json.JsonConvert.SerializeObject(oBookReturn)
			End If

		End If

		'Make the booking
		If Not bBooked Then
			oBookReturn = BookingBase.Basket.Book()
		End If

		'return
		If oBookReturn.OK Then
			Return Newtonsoft.Json.JsonConvert.SerializeObject(oBookReturn)
		Else
			oBookReturn.RedirectString = Intuitive.Functions.IIf(oBookReturn.Warnings.Contains(BookingBasket.BookWarning.Timeout), "?warn=booktimeout", "?warn=bookfailed")
			If BookingBase.Params.SendFailedBookingEmail Then SendFailedBookingDetails()
			Return Newtonsoft.Json.JsonConvert.SerializeObject(oBookReturn)
		End If

	End Function

	Public Shared Sub SendFailedBookingDetails()

		If BookingBase.Params.WarningEmail <> "" Then
			Dim Email As New Intuitive.Email
			With Email
				.SMTPHost = BookingBase.Params.SMTPHost
				.Subject = "Failed Booking"
				.Body = GetFailedBookingDetailsEmailBody()
				.EmailTo = ""
				.From = "Sun Holidays Public Site"
				.FromEmail = "info@" & BookingBase.Params.Theme & ".ie"
				.SendEmail(True)
			End With
		End If
	End Sub

	Public Shared Function GetFailedBookingDetailsEmailBody() As String

		Dim sb As New StringBuilder

		Try
			With sb
				.Append("Customer booking failed with following details:")
				.AppendLine(" ")
				.AppendLine("Lead Guest Title: " & BookingBase.Basket.LeadCustomer.CustomerTitle)
				.AppendLine("Lead Guest Forename: " & BookingBase.Basket.LeadCustomer.CustomerFirstName)
				.AppendLine("Lead Guest Surname: " & BookingBase.Basket.LeadCustomer.CustomerLastName)
				.AppendLine("Lead Guest Contact Number: " & BookingBase.Basket.LeadCustomer.CustomerPhone)
				.AppendLine("Lead Guest Contact Email: " & BookingBase.Basket.LeadCustomer.CustomerEmail)
				.AppendLine("Lead Guest Address: " & BookingBase.Basket.LeadCustomer.CustomerAddress1 & ", " & BookingBase.Basket.LeadCustomer.CustomerAddress2)
				.AppendLine("Lead Guest Postcode: " & BookingBase.Basket.LeadCustomer.CustomerPostcode)
				.AppendLine("Total Price: " & BookingBase.Basket.TotalPrice.ToString)

				.AppendLine(" ")
				.AppendLine("Adults: " & BookingBase.Basket.TotalAdults.ToString)
				.AppendLine("Children: " & BookingBase.Basket.TotalChildren.ToString)
				.AppendLine("Infants: " & BookingBase.Basket.TotalInfants.ToString)
				.AppendLine("Departure Date: " & BookingBase.Basket.FirstDepartureDate.ToString)

				If BookingBase.Basket.BasketProperties.Count > 0 Then
					.AppendLine("Duration: " & BookingBase.Basket.BasketProperties(0).RoomOptions(0).Duration)
					.AppendLine("Destination: " & BookingBase.Lookups.GetLocationFromResort(BookingBase.Basket.BasketProperties(0).RoomOptions(0).GeographyLevel3ID).GeographyLevel3Name)
				ElseIf BookingBase.Basket.BasketFlights.Count > 0 Then
					.AppendLine("Duration: " & DateDiff(DateInterval.Day, BookingBase.Basket.BasketFlights(0).Flight.ReturnDepartureDate, BookingBase.Basket.BasketFlights(0).Flight.OutboundDepartureDate))
				End If

				If BookingBase.Basket.BasketProperties.Count > 0 Then
					.AppendLine(" ")
					.AppendLine("Hotel Name: " & BookingBase.Basket.BasketProperties(0).RoomOptions(0).PropertyName)
					.AppendLine("Number of Rooms: " & BookingBase.Basket.BasketProperties(0).RoomOptions.Count)
					.AppendLine("Hotel Price: " & BookingBase.Basket.BasketProperties(0).RoomOptions.Sum(Function(roomOption) roomOption.TotalPrice))
				End If

				If BookingBase.Basket.BasketFlights.Count > 0 Then
					.AppendLine(" ")
					.AppendLine("Departure Airport: " & BookingBase.Lookups.GetAirportNameFromAirportID(BookingBase.Basket.BasketFlights(0).Flight.DepartureAirportID))
					.AppendLine("Arrival Airport: " & BookingBase.Lookups.GetAirportNameFromAirportID(BookingBase.Basket.BasketFlights(0).Flight.ArrivalAirportID))
					.AppendLine("Departure Flightcode: " & BookingBase.Basket.BasketFlights(0).Flight.OutboundFlightCode)
					.AppendLine("Return Flightcode: " & BookingBase.Basket.BasketFlights(0).Flight.ReturnFlightCode)
					.AppendLine("Flight Total: " & BookingBase.Basket.BasketFlights(0).Flight.Price)
				End If

				If BookingBase.Basket.BasketExtras.Count > 0 Then
					.AppendLine(" ")
					Dim i As Integer = 1
					For Each extra As BookingExtra.BasketExtra In BookingBase.Basket.BasketExtras
						.AppendLine("Extra " & i.ToString & ": " & extra.BasketExtraOptions(0).ExtraName)
						.AppendLine("Extra " & i.ToString & "Price : " & extra.BasketExtraOptions(0).TotalPrice)
						i = i + 1
					Next
				End If

				If BookingBase.Basket.BasketTransfers.Count > 0 Then
					.AppendLine(" ")
					For Each transfer As BookingTransfer.BasketTransfer In BookingBase.Basket.BasketTransfers
						.AppendLine("Transfer Type: " & transfer.Transfer.ArrivalParentType & " - " & transfer.Transfer.DepartureParentType)
						.AppendLine("Transfer Price: " & transfer.Transfer.Price)
					Next
				End If

				If BookingBase.Basket.BasketCarHires.Count > 0 Then

					.AppendLine(" ")
					For Each carHire As BookingCarHire.BasketCarHire In BookingBase.Basket.BasketCarHires
						.AppendLine("Car Hire Type: " & carHire.CarHire.VehicleDescription)
						.AppendLine("Car Hire Duration: " & carHire.CarHire.Duration)
						.AppendLine("Car Hire Pickup Info: " & SplitDepotInformation(carHire.CarHire.PickUpInformation))
						.AppendLine("Car Hire Dropoff Info: " & SplitDepotInformation(carHire.CarHire.DropOffInformation))
						Dim j As Integer = 1
						For Each carHireExtra As BookingCarHire.BasketCarHire.CarHireExtraOption In carHire.CarHire.CarHireExtras
							.AppendLine("Car Hire Extras " & j.ToString & ": " & carHireExtra.Description)
							j = j + 1
						Next
						.AppendLine("Car Hire Price: " & carHire.CarHire.Price)
					Next
				End If
			End With
		Catch ex As Exception
			sb.AppendLine("Couldn't retreive further basket details")
			Return sb.ToString
		End Try

		Return sb.ToString

	End Function

	Private Shared Function SplitDepotInformation(ByVal sDepotInformation As iVectorConnectInterface.CarHire.SearchResponse.DepotInformation) As String

		Dim sb As New StringBuilder

		sb.AppendLine(sDepotInformation.DepotName)
		sb.AppendLine(sDepotInformation.Address1)
		sb.AppendLine(sDepotInformation.City)
		sb.AppendLine(sDepotInformation.Phone)
		sb.AppendLine(sDepotInformation.Postcode)
		sb.AppendLine(sDepotInformation.OpeningTimes)

		Return sb.ToString()
	End Function

	Public Class CustomSetting
		Public BasketType As String
		Public SearchToMainBasket As Boolean
		Public SetupBasketGuestsFromRooms As Boolean
		Public UsePaymentAuthorisation As Boolean
	End Class

End Class
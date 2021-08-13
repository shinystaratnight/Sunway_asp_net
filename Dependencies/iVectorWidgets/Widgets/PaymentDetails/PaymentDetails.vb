Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports ivc = iVectorConnectInterface
Imports System.Xml

Public Class PaymentDetails
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Me.PageDefinition.Overbranding = Functions.IIf(Me.PageDefinition.Overbranding Is Nothing, New Overbranding, Me.PageDefinition.Overbranding)

		If BookingBase.Trade.TradeID > 0 AndAlso Not BookingBase.Trade.CreditCardAgent AndAlso Not Me.PageDefinition.Overbranding.isOverbranded Then
			BookingBase.Basket.IncludePaymentDetails = False
		Else
			If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")

			Dim sControlPath As String = IIf(Me.URLPath.EndsWith(".ascx"), Me.URLPath, Me.URLPath & "/paymentdetails.ascx").ToString

			Dim oControl As New Control
			Try
				oControl = Me.Page.LoadControl(sControlPath)
			Catch ex As Exception
				oControl = Me.Page.LoadControl("/widgetslibrary/PaymentDetails/PaymentDetails.ascx")
			End Try

			Dim oSetting As New PageDefinition.WidgetSetting
			oSetting.Key = "IsOverbranded"
			oSetting.Value = Functions.SafeString(Me.PageDefinition.Overbranding.isOverbranded)
			Me.Settings.Add(oSetting)

			CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)

            Me.DrawControl(writer, oControl)

		End If

	End Sub


	Public Shared Function AddDetailsToBasket(ByVal sKeyValuePairs As String) As String

		'setup payment details from keyvalue paris
		Dim oPaymentDetails As New iVectorConnectInterface.Support.PaymentDetails
		Intuitive.Serializer.DeserializeQueryString(oPaymentDetails, sKeyValuePairs)

		Dim oFields As Dictionary(Of String, Object)
		oFields = Utility.DictionaryKeyValuePair.Decode(sKeyValuePairs)

		'Billing Address
		If oFields.ContainsKey("hidUseBillingAddress") Then
			Dim bUseBillingAddress As Boolean = SafeBoolean(oFields("hidUseBillingAddress"))
			If bUseBillingAddress Then
				Dim oBillingAddress As New iVectorConnectInterface.Support.Address
				With oBillingAddress
					.CustomerTitle = SafeString(oFields("ddlBillingAddress_Title"))
					.CustomerFirstName = SafeString(oFields("txtBillingAddress_FirstName"))
					.CustomerLastName = SafeString(oFields("txtBillingAddress_LastName"))
					.Address1 = SafeString(oFields("txtBillingAddress_Address"))
					.TownCity = SafeString(oFields("txtBillingAddress_City"))
					.Postcode = SafeString(oFields("txtBillingAddress_Postcode"))
					.BookingCountryID = SafeInt(oFields("ddlBillingAddress_BookingCountryID"))
				End With

				oPaymentDetails.BillingAddress = oBillingAddress

			End If
		End If



		'set payment type and amount
		oPaymentDetails.PaymentType = "CreditCard"
        'oPaymentDetails.Amount = Functions.IIf(BookingBase.Basket.PayDeposit, BookingBase.Basket.AmountDueToday, BookingBase.Basket.TotalPrice)
        '
        If Not BookingBase.Basket.PayDeposit Then
			oPaymentDetails.Amount = 0
			'remove agent commisson from amount paid by creditcard.
			For Each oPaymentDue As iVectorConnectInterface.Support.PaymentDue In BookingBase.Basket.PaymentsDue
				oPaymentDetails.Amount += oPaymentDue.Amount
			Next
		Else
			oPaymentDetails.Amount = BookingBase.Basket.AmountDueToday
        End If

        'remove pwcc amount - we should never pass this to connect
		oPaymentDetails.Amount = oPaymentDetails.Amount - BookingBase.Basket.FlightSupplierPaymentAmount


   

		'calculate surcharge
		Dim nSurcharge As Decimal
		nSurcharge = ((oPaymentDetails.Amount * (100 + BookingBase.Basket.SurchargePercentage)) / 100) - oPaymentDetails.Amount


		'set total amount and surcharge
		oPaymentDetails.TotalAmount = oPaymentDetails.Amount + nSurcharge
		oPaymentDetails.Surcharge = nSurcharge


		'set on basket
		BookingBase.Basket.IncludePaymentDetails = True
		BookingBase.Basket.PaymentDetails = oPaymentDetails


		'return so we know it is complete
		Return "Success"

	End Function



	Public Shared Function ExcludePaymentDetails() As String

		BookingBase.Basket.IncludePaymentDetails = False

		Return "Success"

	End Function

	Public Shared Function MakePayment(ByVal sKeyValuePairs As String) As Boolean

		Dim oPaymentDetails As New iVectorConnectInterface.Support.PaymentDetails
		Intuitive.Serializer.DeserializeQueryString(oPaymentDetails, sKeyValuePairs)
		oPaymentDetails.PaymentType = "CreditCard"
		oPaymentDetails.TotalAmount = oPaymentDetails.Amount

		Dim oiVCResponse As New Utility.iVectorConnect.iVectorConnectReturn

		If BookingBase.Basket.TotalComponents > 0 Then
			Dim oAddComponentRequest As New ivc.AddComponentRequest
			oAddComponentRequest.LoginDetails = BookingBase.IVCLoginDetails
			oAddComponentRequest.BookingReference = MyBookingsLogin.MyBookingsReference
			oAddComponentRequest.Payment = oPaymentDetails

			'setup our basket guest details from the booking
			BookingBase.Basket.SetupBasketGuestsFromBooking(MyBookingsLogin.ManageMyBookingDetails.BookingDetails)

			oAddComponentRequest.GuestDetails = BookingBase.Basket.GuestDetails

			'add component book requests
			Dim oExtraBookings As New Generic.List(Of ivc.Extra.BookRequest)
			For Each oExtra As BookingExtra.BasketExtra In BookingBase.Basket.BasketExtras
				oExtraBookings.Add(oExtra.CreateBookRequest())
			Next

			oAddComponentRequest.ExtraBookings = oExtraBookings

			oiVCResponse = Utility.iVectorConnect.SendRequest(Of ivc.AddComponentResponse)(oAddComponentRequest)
		Else
			Dim oMakePaymentRequest As New ivc.MakePaymentRequest
			oMakePaymentRequest.LoginDetails = BookingBase.IVCLoginDetails
			oMakePaymentRequest.BookingReference = MyBookingsLogin.MyBookingsReference
			oMakePaymentRequest.Payment = oPaymentDetails

			If oMakePaymentRequest.Validate.Count > 0 Then
				Return False
			End If

			'3. send request
			oiVCResponse = Utility.iVectorConnect.SendRequest(Of ivc.MakePaymentResponse)(oMakePaymentRequest)
		End If

		'4. return if ok, else nothing
		If oiVCResponse.Success Then
			MyBookingsLogin.UpdateManageMyBookingDetails(MyBookingsLogin.MyBookingsReference)
            BookingBase.Basket = New BookingBasket(True)
			Return True
		End If

		Return False


	End Function


#Region "Credit Card Select"

	Public Function CreditCardSelect(ByVal iCreditCardTypeID As Integer) As String

		Dim oPriceCalculations As New PriceCalculations(iCreditCardTypeID)

		'set on basket
		BookingBase.Basket.SurchargePercentage = oPriceCalculations.CreditCardSurchargePercentage

		'Return JSON object
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oPriceCalculations.GetData())

	End Function



#End Region



#Region "Support"

	Public Class PriceCalculations

		'Public CreditCardTypeID As Integer
		Public CreditCardSurchargePercentage As Decimal
		Public FlightSupplierFee As Decimal
		Public SurchargeOnTotalPrice As Decimal
		Public SurchargeOnDepositPrice As Decimal
		Public RequiresCSV As Boolean = True
		Public CurrencySymbol As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
		Public CurrencyPosition As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)


		Public Sub New(CreditCardTypeID As Integer)

			Dim Basket As BookingBasket = BookingBase.Basket

			'TODO THIS IS WRONG - IT DOES NOT CHECK THE SELLING GEO 1 ID AT THE CARD SURCHARGE LEVEL
			'PLEASE REWRITE THIS
			Me.CreditCardSurchargePercentage = Functions.SafeDecimal(BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.CardSurcharge, CreditCardTypeID))

			If BookingBase.Basket.CreditCardTypes.Where(Function(o) o.CreditCardTypeID = CreditCardTypeID).Count > 0 Then
				Me.FlightSupplierFee =
				 BookingBase.Basket.CreditCardTypes.Where(Function(o) o.CreditCardTypeID = CreditCardTypeID).First.FlightSupplierFee
			End If

			'Check if CSV required
			Dim oXML As XmlDocument = Utility.BigCXML("CreditCardTypes", 1, 600)
			Me.CheckIfCSVRequired(oXML, CreditCardTypeID)

			'get the
			Me.SurchargeOnTotalPrice = (Basket.TotalPrice - Basket.FlightSupplierPaymentAmount) * (Me.CreditCardSurchargePercentage / 100)
			Me.SurchargeOnDepositPrice = (Basket.AmountDueToday - Basket.FlightSupplierPaymentAmount) * (Me.CreditCardSurchargePercentage / 100)

		End Sub

		Public Sub CheckIfCSVRequired(ByVal oXML As XmlDocument, ByVal CreditCardTypeID As Integer)

			Dim oCardTypes As Generic.List(Of CreditCardType) = Utility.XMLToGenericList(Of CreditCardType)(oXML)

			Dim oSelectedCardType As CreditCardType = oCardTypes.Where(Function(o) o.CreditCardTypeID = CreditCardTypeID).FirstOrDefault

			If oSelectedCardType IsNot Nothing Then
				Me.RequiresCSV = Not oSelectedCardType.CSVNotRequired
			End If

		End Sub


		Public Function GetData() As PriceWithSurcharge

			Dim oData As New PriceWithSurcharge
			Dim Basket As BookingBasket = BookingBase.Basket

			With oData
				.TotalPriceUnformated = Basket.TotalPrice + Me.SurchargeOnTotalPrice + Me.FlightSupplierFee
				.DepositAmountUnformated = Basket.AmountDueToday + Me.SurchargeOnDepositPrice + Me.FlightSupplierFee

				If Me.CurrencyPosition = "Append" Then
					.TotalPrice = Functions.SafeString(FormatNumber(.TotalPriceUnformated, 2)) + Me.CurrencySymbol
					.DepositAmount = Functions.SafeString(FormatNumber(.DepositAmountUnformated, 2)) + Me.CurrencySymbol
				Else
					.TotalPrice = CurrencySymbol + Functions.SafeString(FormatNumber(.TotalPriceUnformated, 2))
					.DepositAmount = CurrencySymbol + Functions.SafeString(FormatNumber(.DepositAmountUnformated, 2))
				End If
				.FlightSupplierFee = Me.FlightSupplierFee
				.Surcharge = Me.CreditCardSurchargePercentage
				.TotalPriceSurchargeAmount = Me.SurchargeOnTotalPrice
				.DepositPriceSurchargeAmount = Me.SurchargeOnDepositPrice
				.RequiresCSV = Me.RequiresCSV
			End With

			Return oData

		End Function


	End Class



	Public Class PriceWithSurcharge
		Public Surcharge As Double = 0 'surcharge % 
		Public TotalPriceSurchargeAmount As Decimal = 0
		Public DepositPriceSurchargeAmount As Decimal = 0
		Public TotalPriceUnformated As Double = 0
		Public DepositAmountUnformated As Double = 0
		Public TotalPrice As String = ""
		Public DepositAmount As String = ""
		Public FlightSupplierFee As Decimal = 0	'pwcc charges
		Public RequiresCSV As Boolean = True
	End Class

	Public Class CreditCardType
		Public Name As String
		Public CSVNotRequired As Boolean
		Public CreditCardTypeID As Integer
	End Class

#End Region


End Class

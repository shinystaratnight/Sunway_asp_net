Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports System.Xml
Imports Intuitive
Imports Intuitive.WebControls
Imports iVectorWidgets

Namespace Widgets

	Public Class OffsitePaymentRedirect
		Inherits WidgetBase

		Public Shared Property CustomSettings As CustomSetting
			Get
				If HttpContext.Current.Session("offsitepaymentredirect_customsettings") IsNot Nothing Then
					Return CType(HttpContext.Current.Session("offsitepaymentredirect_customsettings"), CustomSetting)
				End If
				Return New CustomSetting
			End Get
			Set(value As CustomSetting)
				HttpContext.Current.Session("offsitepaymentredirect_customsettings") = value
			End Set
		End Property

#Region "Draw"

		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

			'set up custom settings
			Dim oCustomSettings As New CustomSetting
			With oCustomSettings
				.SuccessURL = Intuitive.Functions.SafeString(Settings.GetValue("SuccessURL"))
				.FailURL = Intuitive.Functions.SafeString(Settings.GetValue("FailURL"))
			End With

			OffsitePaymentRedirect.CustomSettings = oCustomSettings

		End Sub

#End Region

#Region "OffitePayment"

        Public Function GetOffsitePaymentURL(Optional ByVal PaymentAmount As Decimal = 0, Optional ByVal BookingReference As String = "", Optional ByVal ExternalReference As String = "") As String
            'Setup lead guest, save the basket, work out where we want to redirect to, fire off to connect and return

            'if we have a custom amount then we are paying off the booking in MMB
            'we don't need to create a booking reference but we do need some custom basket xml
            Dim iCustomPrice As New Decimal

            Dim oRedirect As New BookingOffsitePayment.OffsitePaymentRedirectReturn

            If PaymentAmount > 0 Then
                'set the outstanding amount to pay
                iCustomPrice = PaymentAmount

                'set the custom basket xml so when we retrieve the basket we know to make a payment
                SetUpCustomXML(True, iCustomPrice)

                'set the booking reference on our new basket
                BookingBase.Basket.BookingReference = BookingReference
                BookingBase.Basket.ExternalReference = ExternalReference

                'set up our lead guest details
                iVectorWidgets.MyBookingsLogin.UpdateManageMyBookingDetails(BookingReference)
                SetUpLeadGuest()

                'set up the guest numbers for connect validation
                SetUpBasketGuests()
            Else

                oRedirect.Warnings.AddRange(CheckPrebook())

                iCustomPrice = Functions.IIf(BookingBase.Basket.PayDeposit, BookingBase.Basket.AmountDueToday, BookingBase.Basket.TotalPrice)
                SetUpCustomXML(False)
            End If

            If oRedirect.Warnings.Count = 0 Then
                Dim oStoreBasketReturn As StoreBasket.StoreBasketReturn = SaveBasket()
                Dim sRedirectURL As String = GetRedirectURL(oStoreBasketReturn.BasketStoreID)
                oRedirect = BookingOffsitePayment.GetRedirect(sRedirectURL, False, iCustomPrice, oStoreBasketReturn.BasketStoreID)
            Else
                oRedirect.Success = False
            End If

            Return Newtonsoft.Json.JsonConvert.SerializeObject(oRedirect)

        End Function

#End Region

        Public Shared Function CheckPrebook() As Generic.List(Of String)

            Dim oWarnings As New Generic.List(Of String)

            If Not BookingBase.Basket.PreBooked Then
                Dim nPrePreBookPrice As Decimal = BookingBase.Basket.TotalPrice
                Dim oPrebookReturn As BookingBasket.PreBookReturn = BookingBase.Basket.PreBook()

                Dim nShownBookingAdjustmentValue As Decimal = 0

                If Not BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes Is Nothing Then
                    For Each oAdjustment In BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes
                        For Each oBookingAdjustment In BookingBase.Basket.BookingAdjustments
                            If oAdjustment.AdjustmentType = oBookingAdjustment.AdjustmentType AndAlso oAdjustment.AdjustmentAmount = oBookingAdjustment.AdjustmentAmount Then
                                nShownBookingAdjustmentValue += oAdjustment.AdjustmentAmount
                            End If
                        Next
                    Next
                End If

                If Not oPrebookReturn.OK Then
                    oWarnings.Add("prebookfailed")
                ElseIf oPrebookReturn.OK AndAlso Not BookingBase.Params.SuppressPriceChangeWarning AndAlso
                    Math.Round(nPrePreBookPrice + nShownBookingAdjustmentValue, 2, MidpointRounding.AwayFromZero) <> BookingBase.Basket.TotalPrice Then

                    Dim dPriceChange As Decimal = Math.Round(BookingBase.Basket.TotalPrice - nPrePreBookPrice, 2, MidpointRounding.AwayFromZero)
                    oWarnings.Add("pricechange&amount=" & dPriceChange)
                End If
            End If

            Return oWarnings

        End Function


#Region "Support Functions"

		Public Shared Function SaveBasket() As StoreBasket.StoreBasketReturn

            Dim oStoreBasketDetails As New StoreBasket.StoreBasketDetails
            Dim iExistingStoreID As Integer = CType(BookingBase.Basket.ExternalReference, Integer)

			With oStoreBasketDetails
                .BasketXML = BookingBase.Basket.XML.InnerXml.ToString
            End With

            If iExistingStoreID > 0 Then
                oStoreBasketDetails.BasketStoreID = iExistingStoreID
            End If

			Dim oStoreBasketReturn As New StoreBasket.StoreBasketReturn
			oStoreBasketReturn = StoreBasket.StoreBasket(oStoreBasketDetails)

			Return oStoreBasketReturn

		End Function

        Public Shared Function GetRedirectURL(ByVal BasketStoreID As Integer) As String
            'Get Querystring, find out what resort we're on, and build up the url using this.

            Dim sQueryString = "BasketStoreID=" & BasketStoreID

            Dim sCurrentDomain As String = GetBaseURL()

            Dim sRedirectURL As String = sCurrentDomain & "paymentauthorisation?" & sQueryString
            Return sRedirectURL

        End Function

		Public Sub SetUpCustomXML(Optional ByVal bPaymentOnly As Boolean = False, Optional ByVal nPaymentAmount As Decimal = 0)

			Dim oCustomXML As New CustomXML
			With oCustomXML
				.PaymentOnly = bPaymentOnly
				.PaymentAmount = nPaymentAmount
				.SuccessURL = OffsitePaymentRedirect.CustomSettings.SuccessURL
				.FailURL = OffsitePaymentRedirect.CustomSettings.FailURL
			End With

			BookingBase.Basket.CustomXML = Serializer.Serialize(oCustomXML, True)
		End Sub

		Public Sub SetUpLeadGuest()

			Try

				'set up our lead customer
				Dim oLeadGuestDetails As New iVectorConnectInterface.Support.LeadCustomerDetails

				With oLeadGuestDetails
					.CustomerEmail = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerEmail
					.CustomerAddress1 = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerAddress1
					.CustomerAddress2 = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerAddress2
					.CustomerPostcode = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerPostcode
					.CustomerTownCity = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerTownCity
					.CustomerBookingCountryID = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerBookingCountryID
					.CustomerPhone = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerPhone

					.CustomerTitle = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerTitle
					.CustomerFirstName = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerFirstName
					.CustomerLastName = MyBookingsLogin.ManageMyBookingDetails.BookingDetails.LeadCustomer.CustomerLastName
				End With

				'set on basket
				BookingBase.Basket.LeadCustomer = oLeadGuestDetails

			Catch ex As Exception

			End Try

		End Sub

		Public Sub SetUpBasketGuests()

			Try
				'adults
				For iAdults As Integer = 1 To MyBookingsLogin.ManageMyBookingDetails.BookingDetails.Adults
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.Type = "Adult"
					BookingBase.Basket.GuestDetails.Add(oGuest)
				Next

				'children
				For iChild As Integer = 1 To MyBookingsLogin.ManageMyBookingDetails.BookingDetails.Children
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.Type = "Child"
					BookingBase.Basket.GuestDetails.Add(oGuest)
				Next

				'infants
				For iInfant As Integer = 1 To MyBookingsLogin.ManageMyBookingDetails.BookingDetails.Infants
					Dim oGuest As New iVectorConnectInterface.Support.GuestDetail
					oGuest.Type = "Infant"
					BookingBase.Basket.GuestDetails.Add(oGuest)
				Next

			Catch ex As Exception

			End Try

		End Sub


#End Region

#Region "Support classes"

		Public Class CustomXML
			Public Property PaymentOnly As Boolean
			Public Property SuccessURL As String
            Public Property FailURL As String
			Public Property PaymentAmount As Decimal
		End Class

		Public Class CustomSetting
			Public SuccessURL As String
			Public FailURL As String
		End Class

#End Region

	End Class

End Namespace



Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports Intuitive.Functions
Imports iVectorConnectInterface

Public Class PaymentAuthHandler
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("paymentauthhandler_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("paymentauthhandler_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("paymentauthhandler_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.AuthFailURL = SafeString(Settings.GetValue("AuthFailURL"))
            .BookFailURL = SafeString(Settings.GetValue("BookFailURL"))
            .MMBAuthFailURL = SafeString(Settings.GetValue("MMBAuthFailURL"))
            If SafeString(Settings.GetValue("BasketIDQuerystringName")) <> "" Then
                .BasketIDQuerystringName = SafeString(Settings.GetValue("BasketIDQuerystringName"))
            Else
                .BasketIDQuerystringName = "orderid"
            End If
        End With

		PaymentAuthHandler.CustomSettings = oCustomSettings

		'get our various redirect urls
		Dim sAuthFailURL As String = IIf(PaymentAuthHandler.CustomSettings.AuthFailURL <> "", PaymentAuthHandler.CustomSettings.AuthFailURL, "/payment?warn=authfailed")
		Dim sBookFailURL As String = IIf(PaymentAuthHandler.CustomSettings.BookFailURL <> "", PaymentAuthHandler.CustomSettings.BookFailURL, "/payment?warn=bookfailed")

		If sBookFailURL <> "" AndAlso BookingBase.Params.SendFailedBookingEmail Then CompleteBooking.SendFailedBookingDetails()

		'get the basket/order id
        Dim ID As Integer = SafeInt(Me.Page.Request.QueryString(PaymentAuthHandler.CustomSettings.BasketIDQuerystringName))

		If ID = 0 Then
			Me.Page.Response.Redirect(sAuthFailURL)
		End If

		'retrieve the basket
		Dim oRetrieveBasketReturn As New StoreBasket.RetrieveBasketReturn
		Dim oRetrieveBasketDetails As New StoreBasket.RetrieveBasketDetails
        oRetrieveBasketDetails.BasketStoreID = ID
        oRetrieveBasketDetails.ClearBasket = True

        oRetrieveBasketReturn = StoreBasket.RetrieveBasket(oRetrieveBasketDetails)
        If oRetrieveBasketReturn.BasketXML = "AutoCleared" Then
            Me.Page.Response.Redirect(sAuthFailURL)
            Return
        End If

		BookingBase.Basket = Intuitive.Serializer.DeSerialize(Of BookingBasket)(oRetrieveBasketReturn.BasketXML)

        If BookingBase.Basket.Booked Then
            sAuthFailURL = IIf(PaymentAuthHandler.CustomSettings.MMBAuthFailURL <> "", PaymentAuthHandler.CustomSettings.MMBAuthFailURL, sAuthFailURL)
        End If

		'setup the process request
		Dim oProcess3DSecureDetails As New Booking3DSecure.Process3DSecureDetails
		With oProcess3DSecureDetails
			.QueryString = SafeString(Me.Page.Request.QueryString)
			.PaymentDetails = BookingBase.Basket.PaymentDetails
		End With

		'do the processing
		Dim oProcess3DSecureReturn As New Booking3DSecure.Process3DSecureReturn
		oProcess3DSecureReturn = Booking3DSecure.Process3DSecure(oProcess3DSecureDetails)

		'if successful, make the booking
		If oProcess3DSecureReturn.Success Then

            If BookingBase.Basket.Booked Then
                BookingBase.Basket.PaymentDetails.PaymentToken = oProcess3DSecureReturn.PaymentToken
                BookingBase.Basket.PaymentDetails.PaymentType = "OffsitePaymentTaken"
                MakePaymentRequest("/account-management?paymentsuccessful=true", sAuthFailURL)
            Else
                MakeBooking("/confirmation", sBookFailURL)
            End If
        Else
            Response.Redirect(sAuthFailURL)
        End If

    End Sub

    Private Sub MakePaymentRequest(SuccessURL As String, FailURL As String)
        '2. Build up the make payment details request
        Dim oMakePaymentRequest As New MakePaymentRequest
        oMakePaymentRequest.LoginDetails = BookingBase.IVCLoginDetails
        oMakePaymentRequest.BookingReference = BookingBase.Basket.BookingReference
        oMakePaymentRequest.Payment = BookingBase.Basket.PaymentDetails

        'If it's not valid, return false.
        If oMakePaymentRequest.Validate.Count > 0 Then
            Response.Redirect(FailURL)
        End If

        '3. send request
        Dim oiVCResponse As New Utility.iVectorConnect.iVectorConnectReturn
        oiVCResponse = Utility.iVectorConnect.SendRequest(Of MakePaymentResponse)(oMakePaymentRequest)

        If oiVCResponse.Success Then
            Response.Redirect(SuccessURL)
        Else
            Response.Redirect(FailURL)
        End If
    End Sub

    Private Sub MakeBooking(SuccessURL As String, FailURL As String)

			Dim oBookReturn As New Intuitive.Web.BookingBasket.BookReturn
			oBookReturn = BookingBase.Basket.Book()

			'return
			If oBookReturn.OK Then
            Response.Redirect(SuccessURL)
			Else
            Response.Redirect(FailURL)
		End If

	End Sub

	Public Class CustomSetting
        Public AuthFailURL As String
        Public BookFailURL As String
        Public MMBAuthFailURL As String
        Public BasketIDQuerystringName As String
	End Class

End Class

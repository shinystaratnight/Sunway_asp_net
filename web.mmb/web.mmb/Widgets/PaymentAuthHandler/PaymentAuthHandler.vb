Imports Intuitive.Functions
Imports Intuitive.Web.Widgets
Imports iVectorWidgets
Imports ivci = iVectorConnectInterface

Namespace Widgets

	Public Class PaymentAuthHandler
		Inherits WidgetBase

#Region "Properties"

		Private Const SuccessUrl As String = "/my-bookings?paymentsuccessful=true"
		Private Const AuthFailUrl As String = "/my-bookings?authfailed=true"
		Private Const BookFailUrl As String = "/my-bookings?paymentfailed=true"

		Public Shared Shadows Property CustomSettings As CustomSetting

			Get
				If HttpContext.Current.Session("offsitepayment_customsettings") IsNot Nothing Then
					Return CType(HttpContext.Current.Session("offsitepayment_customsettings"), CustomSetting)
				End If
				Return New CustomSetting
			End Get
			Set(value As CustomSetting)
				HttpContext.Current.Session("offsitepayment_customsettings") = value
			End Set

		End Property

		Public Shared Property PaymentMade As Boolean

			Get
				If HttpContext.Current.Session("offsitepayment_paymentmade") IsNot Nothing Then
					Return CType(HttpContext.Current.Session("offsitepayment_paymentmade"), Boolean)
				End If
				Return False
			End Get
			Set
				HttpContext.Current.Session("offsitepayment_paymentmade") = Value
			End Set
		End Property

#End Region


		Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

			Dim redirectUrl = "/booking-login?paymentfailed=true"

			Try
				Dim sb As New StringBuilder
				sb.AppendLine("PaymentMade:" + PaymentMade.ToString())
				sb.AppendLine("URL:" + Me.Page.Request.RawUrl.ToString())
				Intuitive.FileFunctions.AddLogEntry("paymentAuthHandler", "payment Auth Handler", sb.ToString())
			Catch ex As Exception
				Intuitive.FileFunctions.AddLogEntry("paymentAuthHandler", "payment Auth Handler logging error", ex.ToString())
			End Try

			If Not PaymentMade Then
				Try

					'get the basket/order id
					Dim basketStoreId As Integer = SafeInt(Me.Page.Request.QueryString("basketref"))

					'retrieve the basket
					Dim bRetrievedSuccessful As Boolean = RetrieveBasket(basketStoreId)
					If bRetrievedSuccessful Then

						'Log them in
						MyBookingsLogin.MyBookingsReference = BookingBase.Basket.BookingReference

						Dim success As Boolean

						If SafeBoolean(Settings.GetValue("UseOffsitePayment")) Then

							Dim sBookingReference As String = SafeString(Me.Page.Request.QueryString("BookingReference"))

							Dim oReturn As BookingOffsitePayment.ProcessOffsitePaymentRedirectReturn =
										BookingOffsitePayment.ProcessOffsitePaymentRedirect(SuccessUrl,
																							Me.Page.Request,
																							sBookingReference,
																							0,
																							basketStoreId)

							If Not String.IsNullOrWhiteSpace(oReturn.AuthorisationCode) Then
								BookingBase.Basket.PaymentDetails.TransactionID = oReturn.AuthorisationCode
								success = oReturn.Success
							End If

						Else
							success = (SafeString(Me.Page.Request.QueryString("result")) = "A")
						End If

						If (success) Then
							Dim oMakePaymentResponse As Utility.iVectorConnect.iVectorConnectReturn = MakePayment(AuthFailUrl, redirectUrl)
							If oMakePaymentResponse.Success Then
								PaymentMade = True
								redirectUrl = SuccessUrl
							End If
						Else
							redirectUrl = BookFailUrl
						End If

					End If

				Catch ex As Exception
					Intuitive.FileFunctions.AddLogEntry("Payment Response", "Payment response error", ex.ToString())
				End Try
			Else
				redirectUrl = SuccessUrl
			End If

			Response.Redirect(redirectUrl)

		End Sub

		Private Function MakePayment(sAuthFailURL As String, ByRef redirectUrl As String) As Utility.iVectorConnect.iVectorConnectReturn

			'create payment request
			Dim oMakePaymentRequest As New ivci.MakePaymentRequest
			With oMakePaymentRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = BookingBase.Basket.BookingReference
				.Payment = BookingBase.Basket.PaymentDetails
			End With

			If Not SafeBoolean(Settings.GetValue("UseOffsitePayment")) Then
				oMakePaymentRequest.Payment.ThreeDSecureCode = BookingBase.Basket.BookingReference
			End If

			If oMakePaymentRequest.Validate.Count > 0 Then
				redirectUrl = sAuthFailURL
			End If

			'send request
			Dim oMakePaymentResponse As New Utility.iVectorConnect.iVectorConnectReturn

			oMakePaymentResponse = Utility.iVectorConnect.SendRequest(Of ivci.MakePaymentResponse)(oMakePaymentRequest)
			Return oMakePaymentResponse

		End Function

#Region "Retrieve Basket"

		Public Shared Function RetrieveBasket(ByVal BasketStoreID As Integer) As Boolean

			Dim bSuccess As Boolean = True
			Dim oRetrieveBasketDetails As New StoreBasket.RetrieveBasketDetails

			With oRetrieveBasketDetails
				.BasketStoreID = BasketStoreID
			End With

			Dim oRetrieveBasketReturn As New StoreBasket.RetrieveBasketReturn
			oRetrieveBasketReturn = StoreBasket.RetrieveBasket(oRetrieveBasketDetails)

			If oRetrieveBasketReturn.BasketXML = "AutoCleared" Then
				bSuccess = False
			Else
				BookingBase.Basket = Intuitive.Serializer.DeSerialize(Of BookingBasket)(oRetrieveBasketReturn.BasketXML)
				BookingBase.Basket.ExternalReference = CType(BasketStoreID, String)
			End If

			Return bSuccess

		End Function

#End Region

	End Class




End Namespace
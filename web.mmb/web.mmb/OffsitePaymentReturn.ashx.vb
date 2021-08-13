Imports System.Web
Imports System.Web.Services
Imports Intuitive.Functions
Imports iVectorWidgets
Imports ivci = iVectorConnectInterface

Public Class OffsitePaymentReturn
	Implements System.Web.IHttpHandler, IRequiresSessionState

	Private Const SuccessUrl As String = "/my-bookings?paymentsuccessful=true"
	Private Const AuthFailUrl As String = "/my-bookings?authfailed=true"
	Private Const BookFailUrl As String = "/my-bookings?paymentfailed=true"

	Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

		Dim sb As New StringBuilder()
		sb.AppendLine("Entered OffsitePaymentReturn.ashx")

		Dim redirectUrl = BookFailUrl
		sb.AppendLine($"RealexForm:{context.Request.Form}")
		FileFunctions.AddLogEntry("OffsitePaymentReturn", "RealexForm", context.Request.Form.ToString())
		Try
			'get the basket/order id
			Dim basketStoreId As Integer = SafeInt(context.Request.QueryString("basketref"))
			sb.AppendLine($"BasketStoreID:{basketStoreId}")
			'retrieve the basket
			Dim bRetrievedSuccessful As Boolean = RetrieveBasket(basketStoreId)
			If bRetrievedSuccessful Then

				'Log them in
				MyBookingsLogin.MyBookingsReference = BookingBase.Basket.BookingReference

				Dim sBookingReference As String = SafeString(context.Request.QueryString("BookingReference"))

				Dim oReturn As BookingOffsitePayment.ProcessOffsitePaymentRedirectReturn =
									BookingOffsitePayment.ProcessOffsitePaymentRedirect(SuccessUrl,
																						context.Request,
																						sBookingReference,
																						0,
																						basketStoreId)

				Dim AuthorisationCode As String = context.Request.Form("PASREF")
				Dim Result As Boolean = context.Request.Form("RESULT") = "00"
				Dim success As Boolean

				If Not String.IsNullOrWhiteSpace(AuthorisationCode) AndAlso Result Then
					BookingBase.Basket.PaymentDetails.TransactionID = AuthorisationCode
					success = True
				End If
				sb.AppendLine($"FormResult:{Result}, {success}")
				sb.AppendLine($"ProcessOffsitePaymentRedirectReturn:{AuthorisationCode}")
				sb.AppendLine($"ProcessOffsitePaymentRedirectWarnings:{oReturn.Warnings}")
				If (success) Then
					Dim oMakePaymentResponse As Utility.iVectorConnect.iVectorConnectReturn = MakePayment(AuthFailUrl, redirectUrl)
					sb.AppendLine($"MakePaymentResult:{oMakePaymentResponse.Success}")
					sb.AppendLine($"MakePaymentRequest:{oMakePaymentResponse.RequestXML}")
					sb.AppendLine($"MakePaymentResponse:{oMakePaymentResponse.ResponseXML}")
					If oMakePaymentResponse.Success Then
						redirectUrl = SuccessUrl
					End If
				Else
					redirectUrl = BookFailUrl
				End If

			End If

		Catch ex As Exception
			sb.AppendLine($"Error:{ex}")
			Intuitive.FileFunctions.AddLogEntry("Payment Response", "Payment response error", ex.ToString())
		End Try

		FileFunctions.AddLogEntry("OffsitePaymentReturn", "ProcessRequest", sb.ToString())

		Dim url As String = context.Request.Url.Scheme + "://" + context.Request.Url.Authority

		context.Response.ContentType = "text/html"
		context.Response.Write($"<script language='javascript' type='text/javascript'>window.location.replace(""{url}{redirectUrl}"");</script>")
	End Sub

	Private Function MakePayment(sAuthFailURL As String, ByRef redirectUrl As String) As Utility.iVectorConnect.iVectorConnectReturn
		Dim sb As New StringBuilder()
		'create payment request
		Dim oMakePaymentRequest As New ivci.MakePaymentRequest
		With oMakePaymentRequest
			.LoginDetails = BookingBase.IVCLoginDetails
			.BookingReference = BookingBase.Basket.BookingReference
			.Payment = BookingBase.Basket.PaymentDetails
		End With

		If oMakePaymentRequest.Validate.Count > 0 Then
			sb.AppendLine($"MakePaymentWarnings:{String.Join(", ", oMakePaymentRequest.Validate)}")
			redirectUrl = sAuthFailURL
		End If

		'send request
		Dim oMakePaymentResponse As New Utility.iVectorConnect.iVectorConnectReturn

		oMakePaymentResponse = Utility.iVectorConnect.SendRequest(Of ivci.MakePaymentResponse)(oMakePaymentRequest)

		FileFunctions.AddLogEntry("OffsitePaymentReturn", "MakePayment", sb.ToString())
		Return oMakePaymentResponse

	End Function

	Public Shared Function RetrieveBasket(ByVal BasketStoreID As Integer) As Boolean
		Dim sb As New StringBuilder()

		Dim bSuccess As Boolean = True
		Dim oRetrieveBasketDetails As New StoreBasket.RetrieveBasketDetails

		With oRetrieveBasketDetails
			.BasketStoreID = BasketStoreID
		End With

		Dim oRetrieveBasketReturn As New StoreBasket.RetrieveBasketReturn
		oRetrieveBasketReturn = StoreBasket.RetrieveBasket(oRetrieveBasketDetails)
		sb.AppendLine($"BasketXML:{oRetrieveBasketReturn.BasketXML}")
		If oRetrieveBasketReturn.BasketXML = "AutoCleared" OrElse oRetrieveBasketReturn.BasketXML Is Nothing Then
			bSuccess = False
		Else
			BookingBase.Basket = Intuitive.Serializer.DeSerialize(Of BookingBasket)(oRetrieveBasketReturn.BasketXML)
			BookingBase.Basket.ExternalReference = CType(BasketStoreID, String)
		End If
		sb.AppendLine($"Success:{bSuccess}")
		FileFunctions.AddLogEntry("OffsitePaymentReturn", "RetrieveBasket", sb.ToString())
		Return bSuccess

	End Function

	ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
		Get
			Return False
		End Get
	End Property

End Class
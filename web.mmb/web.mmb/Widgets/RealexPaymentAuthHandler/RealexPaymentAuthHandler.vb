Imports Intuitive
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions
Imports iVectorWidgets
Imports ivci = iVectorConnectInterface

Namespace Widgets

    Public Class RealexPaymentAuthHandler
        Inherits WidgetBase

#Region "Properties"

        Private Const SuccessUrl As String = "/my-bookings?paymentsuccessful=true"
        Private Const AuthFailUrl As String = "/my-bookings?authfailed=true"

        Public Shared Property CustomSettings As CustomSetting

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

            'retrieve the basket
            Dim iBasketStoreId As Integer = SafeInt(Me.Page.Request.QueryString("basketref"))
            Dim oRetrieveBasketDetails As New StoreBasket.RetrieveBasketDetails
       		oRetrieveBasketDetails.BasketStoreID = iBasketStoreId

            Dim oRetrieveBasketReturn As New StoreBasket.RetrieveBasketReturn
		    oRetrieveBasketReturn = StoreBasket.RetrieveBasket(oRetrieveBasketDetails)
		    BookingBase.Basket = Intuitive.Serializer.DeSerialize(Of BookingBasket)(oRetrieveBasketReturn.BasketXML)


            'setup the process request
		    Dim oProcess3DSecureDetails As New Booking3DSecure.Process3DSecureDetails
		    With oProcess3DSecureDetails
			    .QueryString = SafeString(Me.Page.Request.QueryString)
			    .PaymentDetails = BookingBase.Basket.PaymentDetails
		    End With
           
            For Each key As string In HttpContext.Current.Request.Form.AllKeys
                oProcess3DSecureDetails.FormValues += String.Format("{0}={1}&", key, HttpContext.Current.Request.Form.Item(key))
            Next
            oProcess3DSecureDetails.FormValues = oProcess3DSecureDetails.FormValues.TrimEnd("&"c)


  		    'do the processing
		    Dim oProcess3DSecureReturn As New Booking3DSecure.Process3DSecureReturn
		    oProcess3DSecureReturn = Booking3DSecure.Process3DSecure(oProcess3DSecureDetails)
            
		    'if successful, make the payment
		    If oProcess3DSecureReturn.Success Then
                Dim oMakePaymentResponse As Utility.iVectorConnect.iVectorConnectReturn = MakePayment("")
                    If oMakePaymentResponse.Success Then
                        Response.Redirect(SuccessUrl)
                    End If
		    Else
			    Response.Redirect(AuthFailUrl)
		    End If
        End Sub

        Private Function MakePayment(ByRef redirectUrl As String) As Utility.iVectorConnect.iVectorConnectReturn

            'create payment request
            Dim oMakePaymentRequest As New ivci.MakePaymentRequest
            With oMakePaymentRequest
                .LoginDetails = BookingBase.IVCLoginDetails
                .BookingReference = BookingBase.Basket.BookingReference
                .Payment = BookingBase.Basket.PaymentDetails
            End With

            If oMakePaymentRequest.Validate.Count > 0 Then
                redirectUrl = AuthFailUrl
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
Imports Intuitive
Imports System.Xml
Imports iVectorConnectInterface
Imports iVectorWidgets
Imports ivc = iVectorConnectInterface

Namespace Widgets

    Public Class MyBookings
        Inherits iVectorWidgets.MyBookings

        Property SecureProcessor() As IBooking3DSecure

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

        Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

            '1. set up custom settings
            Dim oCustomSettings As New CustomSetting
            With oCustomSettings
                .CSSClassOverride = Intuitive.Functions.SafeString(Settings.GetValue("CSSClassOverride"))
                .LogoutRedirectURL = Intuitive.Functions.SafeString(Settings.GetValue("LogoutRedirectURL"))
                .OverrideXSL = Intuitive.Functions.SafeString(Settings.GetValue("OverrideXSL"))
                .DocumentName = Intuitive.Functions.SafeString(Settings.GetValue("DocumentName"))
                .ThreeDSecureRedirectURL = Intuitive.Functions.SafeString(Settings.GetValue("ThreeDSecureRedirectURL"))
                .PropertyContentSource = Intuitive.Functions.SafeEnum(Of PropertyContentSource)(Settings.GetValue("PropertyContentSource"))
                .PropertyContentObjectType = Intuitive.Functions.SafeString(Settings.GetValue("PropertyContentObjectType"))
                .UseOffsitePayment = Functions.SafeBoolean(Settings.GetValue("UseOffsitePayment"))
            End With

            MyBookings.CustomSettings = oCustomSettings

            If MyBookingsLogin.CustomerID = 0 AndAlso MyBookingsLogin.MyBookingsReference = "" Then
                Response.Redirect("/booking-login")
            Else

                Dim oBookingDetailsReturn As BookingManagement.BookingDetailsReturn =
                    BookingManagement.GetBookingDetails(MyBookingsLogin.MyBookingsReference)

                Dim oXML As XmlDocument = GetBookingXML(oBookingDetailsReturn)

                oXML = XMLFunctions.MergeXMLDocuments(oXML, Utility.BigCXML("ManageMyBooking", 1, 60))

                MyBookings.AmendmentEmailAddress = Me.Setting("AmendmentEmailAddress")
                MyBookings.CustomerEmailAddress = Intuitive.Functions.SafeString(XMLFunctions.SafeNodeValue(oXML, "//MyBookings/GetBookingDetailsResponse/LeadCustomer/CustomerEmail"))

                Dim oXslParams As WebControls.XSL.XSLParams = MyBookings.GetXSLParams("false")
                oXslParams.AddParam("PaymentSuccessful", Intuitive.Functions.SafeBoolean(HttpContext.Current.Request.QueryString("paymentsuccessful=true")))

                Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath(MyBookings.CustomSettings.OverrideXSL), writer, oXslParams)
            End If

        End Sub

        Private Function GetSinglePropertyBookingXML(ByVal BookingReference As String) As System.Xml.XmlDocument

            Dim oMyBookingsXML As New System.Xml.XmlDocument
            oMyBookingsXML.AppendChild(oMyBookingsXML.CreateNode(System.Xml.XmlNodeType.Element, "MyBookings", ""))

            Dim oBookingDetailsReturn As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(BookingReference)

            ' get property details
            Dim iPropertyID As Integer = Intuitive.Functions.SafeInt(IIf(oBookingDetailsReturn.PropertyReferenceID <> 0,
                                                                          oBookingDetailsReturn.PropertyReferenceID,
                                                                          oBookingDetailsReturn.PropertyID))

            Dim oPropertyXML As XmlDocument = Utility.BigCXML("Property", iPropertyID, 60, 0, 0)

            If Not oPropertyXML Is Nothing Then
                ' insert xml nodes
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Name", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Rating", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Rating"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "URL", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/URL"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "MainImage", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/MainImage"))
                XMLFunctions.AddXMLNode(oBookingDetailsReturn.BookingXML, "GetBookingDetailsResponse/Properties/Property", "Resort", Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Resort"))
                ' Get the meal basis names from ids
                For Each RoomNode As System.Xml.XmlNode In oBookingDetailsReturn.BookingXML.SelectNodes("GetBookingDetailsResponse/Properties/Property/Rooms/Room")
                    Dim MealBasisNode As System.Xml.XmlNode = oBookingDetailsReturn.BookingXML.CreateNode(System.Xml.XmlNodeType.Element, "MealBasis", "")
                    MealBasisNode.InnerText = Lookups.NameLookup(Intuitive.Web.Lookups.LookupTypes.MealBasis, "MealBasis", "MealBasisID", Intuitive.ToSafeInt(Intuitive.XMLFunctions.SafeNodeValue(RoomNode, "MealBasisID")))
                    RoomNode.AppendChild(MealBasisNode)
                Next
            End If

            GetExtraDetails(oBookingDetailsReturn.BookingXML)

            oMyBookingsXML = Intuitive.XMLFunctions.MergeXMLDocuments(oMyBookingsXML, oBookingDetailsReturn.BookingXML)

            Return oMyBookingsXML

        End Function

        Private Function GetBookingXML(ByVal oBookingDetailsReturn As BookingManagement.BookingDetailsReturn) As XmlDocument
            Dim oMyBookingsXML As New System.Xml.XmlDocument
            oMyBookingsXML.AppendChild(oMyBookingsXML.CreateNode(System.Xml.XmlNodeType.Element, "MyBookings", ""))

            GetPropertyDetails(oBookingDetailsReturn)

            GetExtraDetails(oBookingDetailsReturn.BookingXML)

            GetCarHireDetails(oBookingDetailsReturn)

            oMyBookingsXML = Intuitive.XMLFunctions.MergeXMLDocuments(oMyBookingsXML, oBookingDetailsReturn.BookingXML)

            Return oMyBookingsXML
        End Function

        Public Function RequestHotelAmendment(ByVal BookingReference As String, ByVal PropertyBookingReference As String, ByVal CheckInDate As Date,
                ByVal CheckOutDate As Date, ByVal Destination As String, ByVal HotelName As String, ByVal AdditionalInformation As String) As String

            Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
            With oXSLParams
                .AddParam("PropertyBookingReference", PropertyBookingReference)
                .AddParam("CheckInDate", CheckInDate)
                .AddParam("CheckOutDate", CheckOutDate)
                .AddParam("Destination", Destination)
                .AddParam("HotelName", HotelName)
                .AddParam("AdditionalInformation", AdditionalInformation)
            End With

            Dim oEmail As New XSLEmail
            With oEmail
                .SMTPHost = BookingBase.Params.SMTPHost
                .Subject = "Amendment Request - Booking " & BookingReference
                .From = "Intuitive"
                .FromEmail = "admin@intuitivesystems.co.uk"
                .EmailTo = MyBookings.AmendmentEmailAddress
                .XSLTemplate = HttpContext.Current.Server.MapPath("~" & "/Widgets/MyBookings/AmmendmentEmail.xsl")
                .XMLDocument = GetSinglePropertyBookingXML(BookingReference)
                .XSLParameters = oXSLParams
            End With

            Dim sReturn As String

            Try
                If oEmail.SendEmail(True) Then
                    sReturn = "Success"
                Else
                    Intuitive.FileFunctions.AddLogEntry("AmendBookingRequest", "Send Email Failure", oEmail.LastError)
                    sReturn = "Failed"
                End If
            Catch ex As Exception
                Intuitive.FileFunctions.AddLogEntry("AmendBookingRequest", "Send Email Failure", ex.ToString())
                sReturn = "Failed"
            End Try

            Return sReturn

        End Function

        Public Function SendCustomerDocumentation(ByVal sBookingReference As String) As String

            Dim sDocumentationReturn As String = Me.SendDocumentation(sBookingReference, MyBookings.CustomerEmailAddress)

            Return sDocumentationReturn
        End Function

        Public Function MakePayment(ByVal sJSON As String) As String

            PaymentMade = False

            Dim oPaymentDetails As ivc.Support.PaymentDetails = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ivc.Support.PaymentDetails)(sJSON)

            If CustomSettings.UseOffsitePayment Then

                oPaymentDetails.PaymentType = "Custom Payment"

                Try

                    Dim oBookingXml As XmlDocument = MyBookings.GetBookingsXML(BookingReference:=MyBookingsLogin.MyBookingsReference.Trim())

                    Dim oSearchBookingsResponse As GetBookingDetailsResponse = Serializer.DeSerialize(Of GetBookingDetailsResponse)(oBookingXml.SelectSingleNode($"MyBookings/GetBookingDetailsResponse[BookingReference={MyBookingsLogin.MyBookingsReference.Trim()}]").OuterXml)

                    Dim oBasketFromBooking As New BookingBasket
                    With oBasketFromBooking
                        .BookingReference = oSearchBookingsResponse.BookingReference
                        .LeadCustomer = oSearchBookingsResponse.LeadCustomer
                        .PaymentDetails = oPaymentDetails

                        For Each oGuest As GetBookingDetailsResponse.Passenger In oSearchBookingsResponse.BookingPassengers

                            Dim oGuestToAdd As New iVectorConnectInterface.Support.GuestDetail
                            With oGuestToAdd
                                .Type = oGuest.Type
                                .Age = oGuest.Age
                                .FirstName = oGuest.FirstName
                                .LastName = oGuest.LastName
                                .BookingPassengerID = oGuest.BookingPassengerID
                                .DateOfBirth = oGuest.DateOfBirth
                            End With

                            .GuestDetails.Add(oGuestToAdd)

                        Next

                        For Each oCarHire As GetBookingDetailsResponse.CarHireBooking In oSearchBookingsResponse.CarHireBookings

                            Dim oCarHireToAdd As New BookingCarHire.BasketCarHire
                            With oCarHireToAdd
                                .CarHire = New BookingCarHire.BasketCarHire.CarHireOption()
                                With .CarHire
                                    .Price = oCarHire.TotalPrice
                                End With
                            End With

                            .BasketCarHires.Add(oCarHireToAdd)

                        Next

                        For Each oTransfer As GetBookingDetailsResponse.Transfer In oSearchBookingsResponse.Transfers
                            Dim oTransferToAdd As New BookingTransfer.BasketTransfer
                            With oTransferToAdd
                                .Transfer.Price = oTransfer.Price
                            End With

                            .BasketTransfers.Add(oTransferToAdd)
                        Next

                        For Each oProperty As GetBookingDetailsResponse.Property In oSearchBookingsResponse.Properties

                            Dim oPropertyToAdd As New BookingProperty.BasketProperty
                            With oPropertyToAdd
                                .RoomOptions = New List(Of BookingProperty.BasketProperty.BasketPropertyRoomOption)

                                For Each oRoom As GetBookingDetailsResponse.Room In oProperty.Rooms
                                    Dim oRoomToAdd As New BookingProperty.BasketProperty.BasketPropertyRoomOption
                                    With oRoomToAdd
                                        .TotalPrice = oRoom.TotalPrice
                                    End With

                                    oPropertyToAdd.RoomOptions.Add(oRoomToAdd)
                                Next

                            End With

                            .BasketProperties.Add(oPropertyToAdd)

                        Next

                        For Each oFlight As GetBookingDetailsResponse.Flight In oSearchBookingsResponse.Flights

                            Dim oFlightToAdd As New BookingFlight.BasketFlight
                            With oFlightToAdd
                                .Flight.Price = oFlight.Price
                            End With

                            .BasketFlights.Add(oFlightToAdd)

                        Next

                    End With

                    BookingBase.Basket = oBasketFromBooking

                    Dim oStoreBasketRequest As New StoreBasket.StoreBasketDetails
                    With oStoreBasketRequest
                        .BasketXML = BookingBase.Basket.XML.InnerXml
                        .BookingReference = MyBookingsLogin.MyBookingsReference
                    End With

                    Dim oStoreBasketReturn As StoreBasket.StoreBasketReturn = StoreBasket.StoreBasket(oStoreBasketRequest)

                    Dim sRedirectURL As String = String.Format("{0}/OffsitePaymentReturn.ashx?basketref={1}&BookingReference={2}",
                                                               HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority),
                                                               oStoreBasketReturn.BasketStoreID,
                                                               oStoreBasketReturn.BookingReference)

                    Dim oRedirectReturn As BookingOffsitePayment.OffsitePaymentRedirectReturn =
                            BookingOffsitePayment.GetRedirect(sRedirectURL, False, oPaymentDetails.TotalAmount, oStoreBasketReturn.BasketStoreID)

                    FileFunctions.AddLogEntry("MyBookings", "OffsitePayment", sRedirectURL)

                    Dim oReturn As Object = New With {.ReturnType = "Offsite Payment",
                                                    oRedirectReturn.Success,
                                                    oRedirectReturn.HTML,
                                                    oRedirectReturn.Warnings}

                    PaymentAuthHandler.PaymentMade = False

                    Return Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)

                Catch ex As Exception
                    FileFunctions.AddLogEntry("OffsitePayment", "Make Payment Failure", ex.ToString())
                End Try

            Else

                oPaymentDetails.PaymentType = "CreditCard"

                If oPaymentDetails.TotalAmount = 0 AndAlso oPaymentDetails.Amount > 0 Then
                    oPaymentDetails.TotalAmount = oPaymentDetails.Amount
                End If

                Dim oStoreBasket As New StoreBasket.StoreBasketReturn
                Dim oStoreBasketRequest As New StoreBasket.StoreBasketDetails
                oStoreBasketRequest.BasketXML = BookingBase.Basket.XML.InnerXml.ToString

                oStoreBasket = StoreBasket.StoreBasket(oStoreBasketRequest)
                oPaymentDetails.BasketReference = CType(oStoreBasket.BasketStoreID, String)

                If SecureProcessor Is Nothing Then
                    SecureProcessor = New Booking3DSecure()
                End If

                Dim o3DSecureReturnDetails As IGet3DSecureRedirectDetails = SecureProcessor.GetRedirectDetails()

                With o3DSecureReturnDetails
                    .BookingReference = MyBookingsLogin.MyBookingsReference
                    .PaymentDetails = oPaymentDetails
                End With

                If CustomSettings.ThreeDSecureRedirectURL <> "" Then
                    Dim oRequest = HttpContext.Current.Request
                    Dim sBaseUrl = String.Format("{0}://{1}{2}/", oRequest.Url.Scheme, oRequest.Url.Authority, oRequest.ApplicationPath.TrimEnd("/"c))

                    o3DSecureReturnDetails.RedirectURL = String.Format("{0}{1}?basketref={2}", sBaseUrl,
                    MyBookings.CustomSettings.ThreeDSecureRedirectURL, oStoreBasket.BasketStoreID)
                End If

                Dim o3DSecureRedirectReturn As IGet3DSecureRedirectReturn
                o3DSecureRedirectReturn = SecureProcessor.Get3DSecureRedirect(o3DSecureReturnDetails)

                'check if there are no errors from connect, if so, throw an error because we dont want to take any chances
                If o3DSecureRedirectReturn.Success Then

                    oPaymentDetails.PaymentToken = o3DSecureRedirectReturn.PaymentToken

                    'check if the card requires authentication, if not carry on and make the booking as normal
                    If o3DSecureRedirectReturn.RequiresSecureAuthenticationSuccess Then

                        'check if the card is secure enrolled, if so, go off to third party
                        If o3DSecureRedirectReturn.Enrollment Then

                            BookingBase.Basket.BookingReference = MyBookingsLogin.MyBookingsReference
                            BookingBase.Basket.PaymentDetails = oPaymentDetails

                            'Update the payment token that has been returned
                            BookingBase.Basket.PaymentDetails.PaymentToken = o3DSecureRedirectReturn.PaymentToken
                            BookingBase.Basket.ThreeDSecureHTML = o3DSecureRedirectReturn.HTMLData

                            'regenerate the basket xml via booking base
                            oStoreBasketRequest.BasketXML = BookingBase.Basket.XML.InnerXml.ToString

                            'Update the stored basket with the updates
                            oStoreBasketRequest.BasketStoreID = oStoreBasket.BasketStoreID
                            StoreBasket.StoreBasket(oStoreBasketRequest)

                            Dim oReturn As Object = New With {.ReturnType = "SecureRedirect",
                                                          .Success = True,
                                                          .SecureEnrollment = True,
                                                          .HTML = o3DSecureRedirectReturn.HTMLData}

                            PaymentAuthHandler.PaymentMade = False

                            Return Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)
                        End If
                    End If
                End If

            End If

            Return SendPayment(oPaymentDetails)
        End Function

        Public Function SendPayment(oPaymentDetails As ivc.Support.PaymentDetails) As String

            Dim bSuccess As Boolean

            Dim oiVCResponse As New Utility.iVectorConnect.iVectorConnectReturn

            Dim oMakePaymentRequest As New ivc.MakePaymentRequest
            oMakePaymentRequest.LoginDetails = BookingBase.IVCLoginDetails
            oMakePaymentRequest.BookingReference = MyBookingsLogin.MyBookingsReference
            oMakePaymentRequest.Payment = oPaymentDetails

            If oMakePaymentRequest.Validate.Count = 0 Then
                'send request
                oiVCResponse = Utility.iVectorConnect.SendRequest(Of ivc.MakePaymentResponse)(oMakePaymentRequest)

                'return if ok, else nothing
                If oiVCResponse.Success Then
                    MyBookingsLogin.UpdateManageMyBookingDetails(MyBookingsLogin.MyBookingsReference)
                    bSuccess = True
                End If
            Else
                bSuccess = False
            End If

            Dim oBookingDetails As BookingManagement.BookingDetailsReturn =
                    BookingManagement.GetBookingDetails(MyBookingsLogin.MyBookingsReference)

            Dim nTotalOutstanding As Decimal = oBookingDetails.TotalOutstanding
            Dim nTotalPaid As Decimal = oBookingDetails.TotalPaid

            Dim oReturn As Object = New With {.ReturnType = "Payment",
                                              .Success = bSuccess,
                                              .TotalOutstanding = nTotalOutstanding,
                                              .TotalPaid = nTotalPaid,
                                              .BookingReference = MyBookingsLogin.MyBookingsReference}

            Dim sReturn As String = Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)

            Return sReturn
        End Function

        Public Function GetPaymentForm() As String

            Dim sPaymentDetailsHtml As String
            Dim bSuccess As Boolean
            Dim oPaymentControl As New Control
            Try
                Dim page = TryCast(HttpContext.Current.Handler, Page)
                oPaymentControl = page.LoadControl("/widgets/MyBookings/MyBookingsPayment.ascx")

                Dim oSetting As New PageDefinition.WidgetSetting
                With oSetting
                    .Key = "UseOffsitePayment"
                    .Value = Intuitive.Functions.SafeString(CustomSettings.UseOffsitePayment)
                End With

                Me.Settings.Add(oSetting)

                CType(oPaymentControl, UserControlBase).ApplySettings(Me.Settings)

                sPaymentDetailsHtml = Intuitive.Functions.RenderControlToString(oPaymentControl)
                bSuccess = True
            Catch ex As Exception
                sPaymentDetailsHtml = String.Empty
                bSuccess = False
            End Try

            Dim sCurrencySymbol As String = Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
            Dim sCurrencySymbolPosition As String = Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)
            Dim oReturn As Object = New With {.Success = bSuccess,
                                                .HTML = sPaymentDetailsHtml,
                                                .CurrencySymbol = sCurrencySymbol,
                                                .CurrencySymbolPosition = sCurrencySymbolPosition,
                                                .UseOffsitePayment = Intuitive.Functions.SafeBoolean(Me.Settings.GetValue("UseOffsitePayment"))}

            Dim sReturn As String = Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)
            Return sReturn
        End Function

        Public Function CreditCardSelect(creditCardTypeID As Integer) As String
            Dim oPriceCalculations As New PaymentDetails.PriceCalculations(creditCardTypeID)

            'set on basket
            BookingBase.Basket.SurchargePercentage = oPriceCalculations.CreditCardSurchargePercentage

            'Return JSON object
            Return Newtonsoft.Json.JsonConvert.SerializeObject(oPriceCalculations.GetData())
        End Function

        Private Sub GetCarHireDetails(bookingDetailsReturn As BookingManagement.BookingDetailsReturn)
            For Each oCarHireBooking As ivc.GetBookingDetailsResponse.CarHireBooking In bookingDetailsReturn.BookingDetails.CarHireBookings
                Dim oCarHireNode As XmlNode = bookingDetailsReturn.BookingXML.SelectSingleNode(
                                                    String.Format("GetBookingDetailsResponse/CarHireBookings/CarHireBooking[CarHireBookingID = {0}]",
                                                                  oCarHireBooking.CarHireBookingID))

                Dim oPickupDepotNode As XmlNode = oCarHireNode.SelectSingleNode("PickUpDepotName")
                Dim oDropOffDepotNode As XmlNode = oCarHireNode.SelectSingleNode("DropOffDepotName")

                If oPickupDepotNode Is Nothing Then
                    oPickupDepotNode = bookingDetailsReturn.BookingXML.CreateElement("PickUpDepotName")
                    oCarHireNode.AppendChild(oPickupDepotNode)
                End If
                If oDropOffDepotNode Is Nothing Then
                    oDropOffDepotNode = bookingDetailsReturn.BookingXML.CreateElement("DropOffDepotName")
                    oCarHireNode.AppendChild(oDropOffDepotNode)
                End If

                Dim iPickupDepotID As Integer = oCarHireBooking.PickUpDepotID
                Dim iDropOffDepotID As Integer = oCarHireBooking.DropOffDepotID

                oPickupDepotNode.InnerText = Lookups.NameLookup(Lookups.LookupTypes.CarHireDepot, "DepotName", "CarHireDepotID", iPickupDepotID)
                oDropOffDepotNode.InnerText = Lookups.NameLookup(Lookups.LookupTypes.CarHireDepot, "DepotName", "CarHireDepotID", iDropOffDepotID)
            Next
        End Sub

        Private Sub GetExtraDetails(bookingXML As XmlDocument)
            Dim list As XmlNodeList = bookingXML.GetElementsByTagName("Extra")

            For Each xmlNode As XmlNode In list
                Dim iExtraTypeID As Integer = Intuitive.Functions.SafeInt(XMLFunctions.SafeNodeValue(xmlNode, "ExtraTypeID"))
                Dim sExtraType As String = Intuitive.Functions.SafeString(Lookups.GetKeyPairValue(Lookups.LookupTypes.ExtraType, iExtraTypeID))

                Dim oNode = bookingXML.CreateElement("ExtraType")
                oNode.InnerText = sExtraType

                xmlNode.AppendChild(oNode)
            Next
        End Sub

        Private Sub GetPropertyDetails(bookingDetailsReturn As BookingManagement.BookingDetailsReturn)

            For Each oProperty As ivc.GetBookingDetailsResponse.Property In bookingDetailsReturn.BookingDetails.Properties

                Dim iPropertyID As Integer = 0

                If MyBookings.CustomSettings.PropertyContentSource = PropertyContentSource.PropertyID Then
                    iPropertyID = oProperty.PropertyID
                Else
                    iPropertyID = oProperty.PropertyReferenceID
                End If

                Dim oPropertyNode As XmlNode = bookingDetailsReturn.BookingXML.SelectSingleNode(String.Format("GetBookingDetailsResponse/Properties/Property[PropertyBookingID = {0}]", oProperty.PropertyBookingID))

                ' get property details
                Dim oPropertyXML As System.Xml.XmlDocument

                Dim propertyObjectType As String = "Property"
                If MyBookings.CustomSettings.PropertyContentObjectType <> String.Empty Then
                    propertyObjectType = MyBookings.CustomSettings.PropertyContentObjectType
                End If


                oPropertyXML = Utility.BigCXML(propertyObjectType, iPropertyID, 60, 0, 0)

                If Not oPropertyXML Is Nothing Then

                    'create nodes
                    Dim oNameNode = bookingDetailsReturn.BookingXML.CreateElement("Name")
                    Dim oRatingNode = bookingDetailsReturn.BookingXML.CreateElement("Rating")
                    Dim oUrlNode = bookingDetailsReturn.BookingXML.CreateElement("URL")
                    Dim oMainImageNode = bookingDetailsReturn.BookingXML.CreateElement("MainImage")
                    Dim oResortNode = bookingDetailsReturn.BookingXML.CreateElement("Resort")

                    'set values from xml
                    oNameNode.InnerText = Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Name")
                    oRatingNode.InnerText = Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Rating")
                    oUrlNode.InnerText = Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/URL")
                    oMainImageNode.InnerText = Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/MainImage")
                    oResortNode.InnerText = Intuitive.XMLFunctions.SafeNodeValue(oPropertyXML, "Property/Resort")

                    'append nodes to properties
                    With oPropertyNode
                        .AppendChild(oNameNode)
                        .AppendChild(oRatingNode)
                        .AppendChild(oUrlNode)
                        .AppendChild(oMainImageNode)
                        .AppendChild(oResortNode)
                    End With

                    ' Get the meal basis names from ids
                    For Each RoomNode As System.Xml.XmlNode In oPropertyNode.SelectNodes("Rooms/Room")
                        Dim MealBasisNode As System.Xml.XmlNode = bookingDetailsReturn.BookingXML.CreateElement("MealBasis")

                        Dim iMealBasisID As Integer = Intuitive.XMLFunctions.SafeNodeValue(RoomNode, "MealBasisID").ToSafeInt()
                        MealBasisNode.InnerText = Lookups.NameLookup(Intuitive.Web.Lookups.LookupTypes.MealBasis, "MealBasis", "MealBasisID", iMealBasisID)
                        RoomNode.AppendChild(MealBasisNode)
                    Next
                End If

            Next
        End Sub

    End Class
End Namespace
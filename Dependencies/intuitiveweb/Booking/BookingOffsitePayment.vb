
Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.Functions
Imports ivci = iVectorConnectInterface
Public Class BookingOffsitePayment


#Region "Class Definitions"

    Public Class OffsitePaymentRedirectReturn
        Public Success As Boolean = True
        Public URL As String
        Public HTML As String
        Public Warnings As New Generic.List(Of String)
    End Class

    Public Class ProcessOffsitePaymentRedirectReturn
        Public Success As Boolean = True
        Public AuthorisationCode As String
        Public Warnings As New Generic.List(Of String)
    End Class

#End Region

#Region "Store Basket"

    Public Shared Function GetRedirect(ByVal RedirectURL As String, ByVal PreAuthorisationOnly As Boolean, Optional ByVal CustomPrice As Decimal = 0, Optional ByVal BasketStoreID As Integer = 0) As OffsitePaymentRedirectReturn

        Dim oOffsitePaymentRedirectReturn As New OffsitePaymentRedirectReturn

        Try

            Dim oGetOffisitePaymentRedirectRequest As New ivci.GetOffsitePaymentRedirectRequest

            Dim oBookingDetails As New ivci.GetOffsitePaymentRedirectRequest.BookingDetailsDef

            With oBookingDetails
                .TotalPayment = Functions.IIf(CustomPrice <> 0, CustomPrice, BookingBase.Basket.TotalPrice)
                .TotalPassengers = BookingBase.Basket.TotalAdults + BookingBase.Basket.TotalChildren + BookingBase.Basket.TotalInfants
                .LeadCustomer = BookingBase.Basket.LeadCustomer
                .BasketStoreID = BasketStoreID
            End With

            If oBookingDetails.BasketStoreID = 0 Then
                oBookingDetails.BookingReference = BookingBase.Basket.BookingReference
            End If

            With oGetOffisitePaymentRedirectRequest
                .LoginDetails = BookingBase.IVCLoginDetails
                .BookingDetails = oBookingDetails
                .ReturnURL = RedirectURL
                .PreAuthorisationOnly = PreAuthorisationOnly
                .OffsitePaymentTypeID = 0
            End With

            'Do the iVectorConnect validation procedure
            Dim oWarnings As Generic.List(Of String) = oGetOffisitePaymentRedirectRequest.Validate()

            'If everything is ok then serialise the request to xml
            If oWarnings.Count = 0 Then

                Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
                oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.GetOffsitePaymentRedirectResponse)(oGetOffisitePaymentRedirectRequest)

                Dim oGetOffsitePaymentRedirectResponse As New ivci.GetOffsitePaymentRedirectResponse

                If oIVCReturn.Success Then

                    oGetOffsitePaymentRedirectResponse = CType(oIVCReturn.ReturnObject, ivci.GetOffsitePaymentRedirectResponse)
                    oOffsitePaymentRedirectReturn.URL = oGetOffsitePaymentRedirectResponse.RedirectURL
                    oOffsitePaymentRedirectReturn.HTML = oGetOffsitePaymentRedirectResponse.HTML
                    oOffsitePaymentRedirectReturn.Success = True

                Else

                    oOffsitePaymentRedirectReturn.Success = False

                End If

            Else

                oOffsitePaymentRedirectReturn.Warnings = oWarnings
                oOffsitePaymentRedirectReturn.Success = False

            End If

        Catch ex As Exception

            oOffsitePaymentRedirectReturn.Success = False
            oOffsitePaymentRedirectReturn.Warnings.Add(ex.ToString)

        End Try

        Return oOffsitePaymentRedirectReturn

    End Function

#End Region

#Region "Retrieve Basket"
    Public Shared Function ProcessOffsitePaymentRedirect(ByVal URL As String, ByVal Request As HttpRequest, ByVal BookingReference As String, Optional ByVal CustomPrice As Decimal = 0, Optional ByVal BasketStoreID As Integer = 0) As ProcessOffsitePaymentRedirectReturn

        Dim oOffsitePaymentRedirectReturn As New ProcessOffsitePaymentRedirectReturn

        Try

            Dim oProcessOffsitePaymentRedirectRequest As New ivci.ProcessOffsitePaymentRedirectRequest

            Dim oBookingDetails As New ivci.GetOffsitePaymentRedirectRequest.BookingDetailsDef

            With oBookingDetails
                .TotalPayment = Functions.IIf(CustomPrice <> 0, CustomPrice, BookingBase.Basket.TotalPrice)
                .TotalPassengers = BookingBase.Basket.TotalAdults + BookingBase.Basket.TotalChildren + BookingBase.Basket.TotalInfants
                .LeadCustomer = BookingBase.Basket.LeadCustomer
                .BasketStoreID = BasketStoreID
            End With

            If oBookingDetails.BasketStoreID = 0 Then
                oBookingDetails.BookingReference = BookingBase.Basket.BookingReference
            End If

            With oProcessOffsitePaymentRedirectRequest
                .LoginDetails = BookingBase.IVCLoginDetails
                .BookingDetails = oBookingDetails
                .URL = URL
                .QueryString = Request.QueryString.ToString()
                .Body = Request.Form.ToString()
            End With

            'Do the iVectorConnect validation procedure
            Dim oWarnings As Generic.List(Of String) = oProcessOffsitePaymentRedirectRequest.Validate()

            'If everything is ok then serialise the request to xml
            If oWarnings.Count = 0 Then

                Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
                oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.ProcessOffsitePaymentRedirectResponse)(oProcessOffsitePaymentRedirectRequest)

                Dim oProcessOffsitePaymentRedirectResponse As New ivci.ProcessOffsitePaymentRedirectResponse

                If oIVCReturn.Success Then

                    oProcessOffsitePaymentRedirectResponse = CType(oIVCReturn.ReturnObject, ivci.ProcessOffsitePaymentRedirectResponse)
                    oOffsitePaymentRedirectReturn.AuthorisationCode = oProcessOffsitePaymentRedirectResponse.AuthorisationCode
                    oOffsitePaymentRedirectReturn.Success = oProcessOffsitePaymentRedirectResponse.ReturnStatus.Success
                    oOffsitePaymentRedirectReturn.Warnings = oProcessOffsitePaymentRedirectResponse.ReturnStatus.Exceptions

                Else
                    oOffsitePaymentRedirectReturn.Success = False
                End If
            Else
                oOffsitePaymentRedirectReturn.Success = False
            End If

        Catch ex As Exception

            oOffsitePaymentRedirectReturn.Success = False
            oOffsitePaymentRedirectReturn.Warnings.Add(ex.ToString)

            FileFunctions.AddLogEntry("iVectorConnect/ProcessOffsitePaymentRedirectRequest", "ProcessOffsitePaymentRedirectRequestException", ex.ToString)

        End Try

        Return oOffsitePaymentRedirectReturn

    End Function


#End Region


End Class
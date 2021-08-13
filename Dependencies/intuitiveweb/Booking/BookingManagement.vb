Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization

Public Class BookingManagement

#Region "Properties"

	'results
	<XmlIgnore()>
	Public Property Bookings As New BookingsHandler

#End Region

#Region "Class Definitions"

	Public Class CustomerLoginReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public CustomerID As Integer
		Public BookingID As Integer
		Public BrandID As Integer
	End Class

	Public Class SearchBookingsReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public XML As XmlDocument
		Public BookingReferences As New Generic.List(Of String)
	End Class

	Public Class BookingDetailsReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public TotalPrice As Decimal
		Public TotalOutstanding As Decimal
		Public TotalPaid As Decimal
		Public BookingXML As XmlDocument
		Public BookingDetails As ivci.GetBookingDetailsResponse
		Public PropertyReferenceID As Integer
        Public PropertyID As Integer
		Public Transfers As New Generic.List(Of TransferDetailsReturn)
	End Class

	Public Class TransferDetailsReturn
		Public DepartureParentID As Integer
		Public DepartureParentType As String
	End Class

	Public Class PreCancelReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public CancellationCost As Decimal
		Public CancellationToken As String
	End Class

	Public Class CancelReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

    Public Class PreCancelComponentReturn
        Public OK As Boolean = True
        Public Warnings As New Generic.List(Of String)
        Public Components As New Generic.List(Of PreCancelComponent)
    End Class

    Public Class PreCancelComponent
        Public ComponentBookingID As Integer
        Public ComponentType As String
        Public CancellationCost As Decimal
    End Class

	Public Class CancelComponentReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class LoginRequestDetails
		Public LoginType As String
		Public BookingReference As String
		Public EmailAddress As String
		Public Password As String
		Public DepartureDate As String
		Public LastName As String
	End Class

	Public Class ViewDocumentationReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public DocumentPaths As Generic.List(Of String)
	End Class

	Public Class SendDocumentationReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class SeatMapsReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
		Public SeatMapsXML As New System.Xml.XmlDocument
	End Class

	Public Class ReserveSeatsReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class

#End Region

#Region "Customer Login"

	Public Shared Function CustomerLogin(ByVal oLoginRequestDetails As LoginRequestDetails) As CustomerLoginReturn

		Dim oCustomerLoginReturn As New CustomerLoginReturn

		Try
			Dim oCustomerLoginRequest As New ivci.CustomerLoginRequest
			With oCustomerLoginRequest
				.LoginDetails = BookingBase.IVCLoginDetails

				Select Case oLoginRequestDetails.LoginType
					Case "EmailAndReference"
						.LoginBy = ivci.CustomerLoginRequest.LoginMethod.EmailAndReference
						.BookingReference = oLoginRequestDetails.BookingReference
						.Email = oLoginRequestDetails.EmailAddress
					Case "EmailAndPassword"
						.LoginBy = ivci.CustomerLoginRequest.LoginMethod.EmailAndPassword
						.Email = oLoginRequestDetails.EmailAddress
						.Password = oLoginRequestDetails.Password
					Case "BookingDetails"
						.LoginBy = ivci.CustomerLoginRequest.LoginMethod.BookingDetails
						.BookingReference = oLoginRequestDetails.BookingReference
						.DepartureDate = Intuitive.DateFunctions.SafeDate(oLoginRequestDetails.DepartureDate)
						.LastName = oLoginRequestDetails.LastName
					Case "NoBooking"
						.LoginBy = CustomerLoginRequest.LoginMethod.NoBooking
						.Email = oLoginRequestDetails.EmailAddress
						.Password = oLoginRequestDetails.Password
				End Select

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oCustomerLoginReturn.OK = False
					oCustomerLoginReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oCustomerLoginReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CustomerLoginResponse)(oCustomerLoginRequest)

				Dim oCustomerLoginResponse As New ivci.CustomerLoginResponse

				If oIVCReturn.Success Then
					oCustomerLoginResponse = CType(oIVCReturn.ReturnObject, ivci.CustomerLoginResponse)

					oCustomerLoginReturn.CustomerID = oCustomerLoginResponse.CustomerID
					oCustomerLoginReturn.BookingID = oCustomerLoginResponse.BookingID
					oCustomerLoginReturn.BrandID = oCustomerLoginResponse.BrandID

				Else
					oCustomerLoginReturn.OK = False
				End If
			End If

		Catch ex As Exception
			oCustomerLoginReturn.OK = False
			oCustomerLoginReturn.Warnings.Add(ex.ToString)
		End Try

		Return oCustomerLoginReturn

	End Function

#End Region

#Region "Booking Details"

	'original
	Public Shared Function SearchBookings(ByVal CustomerID As Integer, Optional ByVal BrandIDs As Generic.List(Of Integer) = Nothing) As SearchBookingsReturn

		Dim oSearchBookingsReturn As New SearchBookingsReturn

		Try
			Dim oSearchBookingsRequest As New ivci.SearchBookingsRequest
			With oSearchBookingsRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.CustomerID = CustomerID
				.EarliestDepartureDate = Date.Now.AddDays(BookingBase.Params.EarliestDepartureDateAddDays)

				If BrandIDs IsNot Nothing Then
					.BrandIDs = BrandIDs
				End If

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchBookingsReturn.OK = False
					oSearchBookingsReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oSearchBookingsReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.SearchBookingsResponse)(oSearchBookingsRequest)

				Dim oSearchBookingsResponse As New ivci.SearchBookingsResponse

				If oIVCReturn.Success Then
					oSearchBookingsResponse = CType(oIVCReturn.ReturnObject, ivci.SearchBookingsResponse)

					Dim iBookingCount As Integer = Intuitive.ToSafeInt(IIf(oSearchBookingsResponse.Bookings.Count > 0, oSearchBookingsResponse.Bookings.Count - 1, 0))

					For iCount As Integer = 0 To iBookingCount
						oSearchBookingsReturn.BookingReferences.Add(oSearchBookingsResponse.Bookings(iCount).BookingReference)
					Next

				Else
					oSearchBookingsReturn.OK = False
				End If
			End If

		Catch ex As Exception
			oSearchBookingsReturn.OK = False
			oSearchBookingsReturn.Warnings.Add(ex.ToString)
		End Try

		Return oSearchBookingsReturn

	End Function

	'new functionality
	Public Shared Function SearchTradeBookings(ByVal oSearchBookingsRequest As ivci.SearchBookingsRequest) As SearchBookingsReturn

		Dim oSearchBookingsReturn As New SearchBookingsReturn

		Try
			With oSearchBookingsRequest

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchBookingsReturn.OK = False
					oSearchBookingsReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oSearchBookingsReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.SearchBookingsResponse)(oSearchBookingsRequest)

				Dim oSearchBookingsResponse As New ivci.SearchBookingsResponse

				If oIVCReturn.Success Then
					oSearchBookingsResponse = CType(oIVCReturn.ReturnObject, ivci.SearchBookingsResponse)

					BookingBase.SearchBookings.Bookings.Save(oSearchBookingsResponse.Bookings)

					oSearchBookingsReturn.XML = BookingBase.SearchBookings.Bookings.GetBookingsXML(1)

					Dim iBookingCount As Integer = Intuitive.ToSafeInt(IIf(oSearchBookingsResponse.Bookings.Count > 0, oSearchBookingsResponse.Bookings.Count - 1, 0))

					For iCount As Integer = 0 To iBookingCount
						oSearchBookingsReturn.BookingReferences.Add(oSearchBookingsResponse.Bookings(iCount).BookingReference)
					Next

				Else
					oSearchBookingsReturn.OK = False
				End If
			End If

		Catch ex As Exception
			oSearchBookingsReturn.OK = False
			oSearchBookingsReturn.Warnings.Add(ex.ToString)
		End Try

		Return oSearchBookingsReturn

	End Function

	Public Shared Function GetBookingDetails(ByVal sBookingReference As String) As BookingDetailsReturn

		Dim oGetBookingDetailsReturn As New BookingDetailsReturn

		Try
			Dim oGetBookingDetailsRequest As New ivci.GetBookingDetailsRequest
			With oGetBookingDetailsRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
                .ShowAllLiveCustomerPayments = BookingBase.Params.ShowAllLiveCustomerPayments

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oGetBookingDetailsReturn.OK = False
					oGetBookingDetailsReturn.Warnings.AddRange(oWarnings)
				End If
			End With

			If oGetBookingDetailsReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.GetBookingDetailsResponse)(oGetBookingDetailsRequest)

				Dim oGetBookingDetailsResponse As New ivci.GetBookingDetailsResponse

				If oIVCReturn.Success Then
					oGetBookingDetailsResponse = CType(oIVCReturn.ReturnObject, ivci.GetBookingDetailsResponse)

					oGetBookingDetailsReturn.BookingXML = Intuitive.Serializer.Serialize(oGetBookingDetailsResponse)
					oGetBookingDetailsReturn.BookingDetails = oGetBookingDetailsResponse
                    oGetBookingDetailsReturn.TotalOutstanding = oGetBookingDetailsResponse.TotalOutstanding
                    oGetBookingDetailsReturn.TotalPaid = oGetBookingDetailsResponse.TotalPaid
                    oGetBookingDetailsReturn.TotalPrice = oGetBookingDetailsResponse.TotalPrice
					If oGetBookingDetailsResponse.Properties.Count > 0 Then
						oGetBookingDetailsReturn.PropertyReferenceID = oGetBookingDetailsResponse.Properties(0).PropertyReferenceID
                        oGetBookingDetailsReturn.PropertyID = oGetBookingDetailsResponse.Properties(0).PropertyID
					End If
					If oGetBookingDetailsResponse.Transfers.Count > 0 Then
						For Each oTransfer As GetBookingDetailsResponse.Transfer In oGetBookingDetailsResponse.Transfers
							Dim oTransferDetailsReturn As New TransferDetailsReturn
							With oTransferDetailsReturn
								.DepartureParentID = oTransfer.DepartureParentID
								.DepartureParentType = oTransfer.DepartureParentType
							End With
							oGetBookingDetailsReturn.Transfers.Add(oTransferDetailsReturn)
						Next
					End If

				Else
					oGetBookingDetailsReturn.OK = False
					oGetBookingDetailsReturn.Warnings = oIVCReturn.Warning
				End If

			End If

		Catch ex As Exception
			oGetBookingDetailsReturn.OK = False
			oGetBookingDetailsReturn.Warnings.Add(ex.ToString)
		End Try

		Return oGetBookingDetailsReturn

	End Function


	Public Shared Function ReserveSeats() As ReserveSeatsReturn

		Dim oReserveSeatsReturn As New ReserveSeatsReturn

		Try
			Dim oReserveSeatsRequest As New ivci.ReserveSeatsRequest
			With oReserveSeatsRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = BookingBase.Basket.BookingReference
				.FlightBookingID = BookingBase.Basket.BasketFlights.Last().FlightBookingID

				For Each oBasketFlightExtra As BookingFlight.BasketFlight.BasketFlightExtra In BookingBase.Basket.BasketFlights.Last().BasketFlightExtras
					Dim oExtra As New ivci.Flight.PreBookRequest.Extra
					oExtra.ExtraBookingToken = oBasketFlightExtra.ExtraBookingToken
					oExtra.Quantity = oBasketFlightExtra.QuantitySelected
					oExtra.RequestedExtraType = oBasketFlightExtra.ExtraType
					oExtra.GuestID = oBasketFlightExtra.GuestID
					.Extras.Add(oExtra)
				Next

				If BookingBase.Basket.PaymentDetails.TotalAmount > 0 Then
					.Payment = BookingBase.Basket.PaymentDetails
				End If

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oReserveSeatsReturn.OK = False
					oReserveSeatsReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oReserveSeatsReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.ReserveSeatsResponse)(oReserveSeatsRequest)

				Dim oReserveSeatsResponse As New ivci.ReserveSeatsResponse

				If oIVCReturn.Success Then
					oReserveSeatsResponse = CType(oIVCReturn.ReturnObject, ivci.ReserveSeatsResponse)

				Else
					oReserveSeatsReturn.OK = False
				End If
			End If

		Catch ex As Exception
			oReserveSeatsReturn.OK = False
			oReserveSeatsReturn.Warnings.Add(ex.ToString)
		End Try

		Return oReserveSeatsReturn

	End Function

#End Region

#Region "Cancellation"

#Region "Pre-Cancel"

	Public Shared Function RequestPreCancel(ByVal sBookingReference As String) As PreCancelReturn

		Dim oPreCancelReturn As New PreCancelReturn

		Try
			Dim oPreCancelRequest As New ivci.PreCancelRequest
			With oPreCancelRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oPreCancelReturn.OK = False
					oPreCancelReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oPreCancelReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.PreCancelResponse)(oPreCancelRequest)

				Dim oPreCancelResponse As New ivci.PreCancelResponse

				If oIVCReturn.Success Then
					oPreCancelResponse = CType(oIVCReturn.ReturnObject, ivci.PreCancelResponse)

					oPreCancelReturn.CancellationCost = oPreCancelResponse.CancellationCost
					oPreCancelReturn.CancellationToken = oPreCancelResponse.CancellationToken
				Else
					oPreCancelReturn.OK = False
					oPreCancelReturn.Warnings = oIVCReturn.Warning
				End If
			End If

		Catch ex As Exception
			oPreCancelReturn.OK = False
			oPreCancelReturn.Warnings.Add(ex.ToString)
		End Try

		Return oPreCancelReturn

	End Function

#End Region

#Region "Confirm Cancel"

	Public Shared Function RequestCancel(ByVal CancellationCost As Decimal, ByVal CancellationToken As String, ByVal sBookingReference As String, Optional _
	                                        ByVal sCancellationReason As String = "") As CancelReturn

		Dim oCancelReturn As New CancelReturn

		Try
			Dim oCancelRequest As New ivci.CancelRequest
			With oCancelRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
				.CancellationCost = CancellationCost
                .CancellationToken = CancellationToken
                .CancellationReason = sCancellationReason

                'Do the iVectorConnect validation procedure
                Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oCancelReturn.OK = False
					oCancelReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oCancelReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CancelResponse)(oCancelRequest)

				Dim oCancelResponse As New ivci.CancelResponse

				If oIVCReturn.Success Then
					oCancelResponse = CType(oIVCReturn.ReturnObject, ivci.CancelResponse)
				Else
					oCancelReturn.OK = False
					oCancelReturn.Warnings = oIVCReturn.Warning
				End If
			End If

		Catch ex As Exception
			oCancelReturn.OK = False
			oCancelReturn.Warnings.Add(ex.ToString)
		End Try

		Return oCancelReturn

	End Function

#End Region

#Region "Component Pre-Cancel"

    Public Shared Function RequestComponentPreCancel(ByVal sBookingReference As String, ByVal Components As Generic.List(Of ivci.PreCancelComponentRequest.BookingComponent)) As PreCancelComponentReturn

        Dim oPreCancelComponentReturn As New PreCancelComponentReturn

        Try
            Dim oPreCancelComponentRequest As New ivci.PreCancelComponentRequest
            With oPreCancelComponentRequest
                .LoginDetails = BookingBase.IVCLoginDetails
                .BookingReference = sBookingReference
                .BookingComponents = Components

                'Do the iVectorConnect validation procedure
                Dim oWarnings As Generic.List(Of String) = .Validate()

                If oWarnings.Count > 0 Then
                    oPreCancelComponentReturn.OK = False
                    oPreCancelComponentReturn.Warnings.AddRange(oWarnings)
                End If

            End With

            'If everything is ok then serialise the request to xml
            If oPreCancelComponentReturn.OK Then

                Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
                oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.PreCancelComponentResponse)(oPreCancelComponentRequest)

                Dim oPreCancelComponentResponse As New ivci.PreCancelComponentResponse

                If oIVCReturn.Success Then
                    oPreCancelComponentResponse = CType(oIVCReturn.ReturnObject, ivci.PreCancelComponentResponse)

                    For Each Component As ivci.PreCancelComponentResponse.BookingComponent In oPreCancelComponentResponse.BookingComponents
                        Dim oPreCancelComponent As New PreCancelComponent
                        With oPreCancelComponent
                            .ComponentBookingID = Component.ComponentBookingID
                            .ComponentType = Component.ComponentType
                            .CancellationCost = Component.CancellationCost
                        End With
                        oPreCancelComponentReturn.Components.Add(oPreCancelComponent)
                    Next
                Else
                    oPreCancelComponentReturn.OK = False
                    oPreCancelComponentReturn.Warnings = oIVCReturn.Warning
                End If
            End If

        Catch ex As Exception
            oPreCancelComponentReturn.OK = False
            oPreCancelComponentReturn.Warnings.Add(ex.ToString)
        End Try

        Return oPreCancelComponentReturn

    End Function

#End Region

#Region "Confirm Component Cancel"

	Public Shared Function CancelComponets(ByVal oCancelComponentRequest As iVectorConnectInterface.CancelComponentRequest) As CancelComponentReturn

		Dim oCancelComponentReturn As New CancelComponentReturn

		Try

			Dim oWarnings As Generic.List(Of String) = oCancelComponentRequest.Validate()

			If oWarnings.Count > 0 Then
				oCancelComponentReturn.OK = False
				oCancelComponentReturn.Warnings.AddRange(oWarnings)
			End If

			'If everything is ok then serialise the request to xml
			If oCancelComponentReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.CancelComponentResponse)(oCancelComponentRequest)

				Dim oCancelResponse As New ivci.CancelComponentResponse

				If oIVCReturn.Success Then
					oCancelResponse = CType(oIVCReturn.ReturnObject, ivci.CancelComponentResponse)
				Else
					oCancelComponentReturn.OK = False
					oCancelComponentReturn.Warnings = oIVCReturn.Warning
				End If
			End If

		Catch ex As Exception
			oCancelComponentReturn.OK = False
			oCancelComponentReturn.Warnings.Add(ex.ToString)
		End Try

		Return oCancelComponentReturn

	End Function

#End Region

#End Region

#Region "Documentation"

	Public Shared Function ViewBookingDocumentationRequest(ByVal sBookingReference As String, _
	 ByVal DocumentationID As Integer) As ViewDocumentationReturn

		Dim oViewDocumentationReturn As New ViewDocumentationReturn

		Try

			Dim oViewDocumentationRequest As New ivci.ViewDocumentationRequest
			With oViewDocumentationRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
				.BookingDocumentationID = DocumentationID

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oViewDocumentationReturn.OK = False
					oViewDocumentationReturn.Warnings.AddRange(oWarnings)
				End If
			End With

			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.ViewDocumentationResponse)(oViewDocumentationRequest)

			Dim oViewDocumentationResponse As New ivci.ViewDocumentationResponse

			If oIVCReturn.Success Then
				oViewDocumentationResponse = CType(oIVCReturn.ReturnObject, ivci.ViewDocumentationResponse)
				oViewDocumentationReturn.DocumentPaths = oViewDocumentationResponse.DocumentPaths
			Else
				oViewDocumentationReturn.OK = False
				oViewDocumentationReturn.Warnings = oIVCReturn.Warning
			End If

		Catch ex As Exception
			oViewDocumentationReturn.OK = False
			oViewDocumentationReturn.Warnings.Add(ex.ToString)
		End Try

		Return oViewDocumentationReturn

	End Function

	Public Shared Function SendBookingDocumentationRequest(ByVal sBookingReference As String, ByVal DocumentationID As Integer, Optional ByVal sOverrideEmail As String = "") As SendDocumentationReturn

		Dim oSendDocumentationReturn As New SendDocumentationReturn

		Try

			Dim oSendDocumentationRequest As New ivci.SendDocumentationRequest
			With oSendDocumentationRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = sBookingReference
				.BookingDocumentationID = DocumentationID
				If sOverrideEmail <> "" Then
					oSendDocumentationRequest.OverrideEmailAddress = sOverrideEmail
				End If

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSendDocumentationReturn.OK = False
					oSendDocumentationReturn.Warnings.AddRange(oWarnings)
				End If
			End With

			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
			oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.SendDocumentationResponse)(oSendDocumentationRequest)

			Dim oSendDocumentationResponse As New ivci.SendDocumentationResponse

			If oIVCReturn.Success Then
				oSendDocumentationResponse = CType(oIVCReturn.ReturnObject, ivci.SendDocumentationResponse)
			Else
				oSendDocumentationReturn.OK = False
				oSendDocumentationReturn.Warnings = oIVCReturn.Warning
			End If

		Catch ex As Exception
			oSendDocumentationReturn.OK = False
			oSendDocumentationReturn.Warnings.Add(ex.ToString)
		End Try

		Return oSendDocumentationReturn

	End Function

#End Region

End Class

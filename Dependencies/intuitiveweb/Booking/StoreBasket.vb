Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.Functions
Imports ivci = iVectorConnectInterface

Public Class StoreBasket

#Region "Class Definitions"

	Public Class StoreBasketReturn
		Public Success As Boolean = True
		Public BasketStoreID As Integer
		Public BookingReference As String
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class StoreBasketDetails
		Public BookingReference As String
		Public BasketXML As String
		Public BasketStoreID As Integer = 0
	End Class

	Public Class RetrieveBasketReturn
		Public Success As Boolean = True
		Public BasketXML As String
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class RetrieveBasketDetails
		Public BookingReference As String
        Public BasketStoreID As Integer = 0
        Public ClearBasket As Boolean = False
	End Class

#End Region

#Region "Store Basket"

	Public Shared Function StoreBasket(ByVal oStoreBasketDetails As StoreBasketDetails) As StoreBasketReturn

		Dim oStoreBasketReturn As New StoreBasketReturn

		Try
			Dim oStoreBasketRequest As New ivci.StoreBasketRequest
			With oStoreBasketRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = oStoreBasketDetails.BookingReference
				.BasketXML = oStoreBasketDetails.BasketXML
				.BasketStoreID = oStoreBasketDetails.BasketStoreID

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oStoreBasketReturn.Success = False
					oStoreBasketReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oStoreBasketReturn.Success Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.StoreBasketResponse)(oStoreBasketRequest)

				Dim oStoreBasketResponse As New ivci.StoreBasketResponse

				If oIVCReturn.Success Then
					oStoreBasketResponse = CType(oIVCReturn.ReturnObject, ivci.StoreBasketResponse)

					oStoreBasketReturn.BasketStoreID = oStoreBasketResponse.BasketStoreID
					oStoreBasketReturn.BookingReference = oStoreBasketResponse.BookingReference
				Else
					oStoreBasketReturn.Success = False
				End If
			End If

		Catch ex As Exception
			oStoreBasketReturn.Success = False
			oStoreBasketReturn.Warnings.Add(ex.ToString)
		End Try

		Return oStoreBasketReturn

	End Function

#End Region

#Region "Retrieve Basket"

	Public Shared Function RetrieveBasket(ByVal oRetrieveBasketDetails As RetrieveBasketDetails) As RetrieveBasketReturn

		Dim oRetrieveBasketReturn As New RetrieveBasketReturn

		Try
			Dim oRetrieveBasketRequest As New ivci.RetrieveStoredBasketRequest
			With oRetrieveBasketRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = oRetrieveBasketDetails.BookingReference
                .BasketStoreID = oRetrieveBasketDetails.BasketStoreID
                .ClearBasket = oRetrieveBasketDetails.ClearBasket

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oRetrieveBasketReturn.Success = False
					oRetrieveBasketReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oRetrieveBasketReturn.Success Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.RetrieveStoredBasketResponse)(oRetrieveBasketRequest)

				Dim oRetrieveBasketResponse As New ivci.RetrieveStoredBasketResponse

				If oIVCReturn.Success Then
					oRetrieveBasketResponse = CType(oIVCReturn.ReturnObject, ivci.RetrieveStoredBasketResponse)

					oRetrieveBasketReturn.BasketXML = oRetrieveBasketResponse.BasketXML
				Else
					oRetrieveBasketReturn.Success = False
				End If
			End If

		Catch ex As Exception
			oRetrieveBasketReturn.Success = False
			oRetrieveBasketReturn.Warnings.Add(ex.ToString)
		End Try

		Return oRetrieveBasketReturn

	End Function

#End Region

End Class

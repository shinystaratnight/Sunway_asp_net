Imports ivci = iVectorConnectInterface
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports Intuitive

Public Class PostcodeLookup

	Public Postcode As String
	Public GeographyLevel1ID As Integer
	Public Customer As String = ""

	Public Sub New()

	End Sub

	Public Sub New(ByVal sPostcode As String, ByVal UserCountryID As Integer)
		Me.Postcode = sPostcode
		Me.GeographyLevel1ID = UserCountryID
	End Sub

	Public Class Address
		Public Address1 As String = ""
		Public Address2 As String = ""
		Public TownCity As String
		Public County As String = ""
		Public Postcode As String
		Public Sequence As Integer
		Public BookingCountryID As Integer = 1
		Public CountryISOCode As String
		Public Type As String = ""
		Public LanguageID As Integer = 0

		Public BuildingID As String
		Public Description As String
	End Class

	Public Function GetAddresses() As Generic.List(Of Address)

		Dim oSearchReturn As New SearchReturn
		Dim oiVectorConnectSearchRequest As New PostcodeLookupRequest

		Try
			'Build up the request
			With oiVectorConnectSearchRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.Postcode = Me.Postcode
				.Customer = Me.Customer
				.GeographyLevel1ID = Me.GeographyLevel1ID
			End With

			Dim oSearchRequestXML As XmlDocument = Serializer.Serialize(oiVectorConnectSearchRequest)

			'Send the request
			Dim oSearchResponseXML As New XmlDocument
			Dim sSearchResponse As String = Intuitive.Net.WebRequests.GetResponse(BookingBase.Params.ServiceURL & "ivectorconnect.ashx", oSearchRequestXML.InnerXml, , "")
			oSearchResponseXML.LoadXml(sSearchResponse)

			Dim oAddresses As New Generic.List(Of Address)

			If XMLFunctions.SafeNodeValue(oSearchResponseXML, "//ReturnStatus/Success").ToSafeBoolean = True Then

				'Deserialise to the response class
				Dim oSearchResponse As PostcodeLookupResponse = Serializer.DeSerialize(Of PostcodeLookupResponse)(oSearchResponseXML.OuterXml)

				'Check if there were any results
				If oSearchResponse.Addresses.Count > 0 Then

					For Each oAddressReturn As ivci.Support.Address In oSearchResponse.Addresses

						Dim oNewAddress As New Address

						With oNewAddress
							.Address1 = oAddressReturn.Address1
							.Address2 = oAddressReturn.Address2
							.TownCity = oAddressReturn.TownCity
							.County = oAddressReturn.County
							.Postcode = oAddressReturn.Postcode
							.Sequence = oAddressReturn.Sequence
							.BookingCountryID = oAddressReturn.BookingCountryID
							.CountryISOCode = oAddressReturn.CountryISOCode
							.Type = oAddressReturn.Type
							.LanguageID = oAddressReturn.LanguageID


							.BuildingID = oAddressReturn.BuildingID
							.Description = oAddressReturn.Description
						End With

						oAddresses.Add(oNewAddress)
					Next


				End If
			End If

			Return oAddresses

		Catch ex As Exception
			Return New Generic.List(Of Address)
		End Try
	End Function

	Public Shared Function GetSingleAddress(ByVal BuildingID As Integer) As Address

		Dim oSearchReturn As New SearchReturn
		Dim oiVectorConnectSearchRequest As New GetAddressRequest

		Try
			'Build up the request
			With oiVectorConnectSearchRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BuildingID = CStr(BuildingID)
			End With

			Dim oSearchRequestXML As XmlDocument = Serializer.Serialize(oiVectorConnectSearchRequest)

			'Send the request
			Dim oSearchResponseXML As New XmlDocument
			Dim sSearchResponse As String = Intuitive.Net.WebRequests.GetResponse(BookingBase.Params.ServiceURL & "ivectorconnect.ashx", oSearchRequestXML.InnerXml, , "")
			oSearchResponseXML.LoadXml(sSearchResponse)

			Dim oNewAddress As New PostcodeLookup.Address

			If XMLFunctions.SafeNodeValue(oSearchResponseXML, "//ReturnStatus/Success").ToSafeBoolean = True Then

				'Deserialise to the response class
				Dim oSearchResponse As GetAddressResponse = Serializer.DeSerialize(Of GetAddressResponse)(oSearchResponseXML.OuterXml)

				'Check if there were any results
				If oSearchResponse.ReturnStatus.Success Then



					With oNewAddress
						.Address1 = oSearchResponse.Address.Address1
						.Address2 = oSearchResponse.Address.Address2
						.TownCity = oSearchResponse.Address.TownCity
						.County = oSearchResponse.Address.County
						.Postcode = oSearchResponse.Address.Postcode
						.Sequence = oSearchResponse.Address.Sequence
						.BookingCountryID = oSearchResponse.Address.BookingCountryID
						.CountryISOCode = oSearchResponse.Address.CountryISOCode
						.Type = oSearchResponse.Address.Type
						.LanguageID = oSearchResponse.Address.LanguageID


						.BuildingID = oSearchResponse.Address.BuildingID
						.Description = oSearchResponse.Address.Description
					End With

				End If
			End If

			Return oNewAddress

		Catch ex As Exception
			Return New Address
		End Try
	End Function

	Public Class SearchReturn

		Public OK As Boolean = True
		Public Warning As New Generic.List(Of String)

	End Class

End Class

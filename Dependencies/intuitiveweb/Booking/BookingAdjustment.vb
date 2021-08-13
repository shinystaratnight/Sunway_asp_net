Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization

Public Class BookingAdjustment

	Public Shared Function Search(ByVal oBookingSearch As BookingSearch) As BookingSearch.SearchReturn

		Dim oSearchReturn As New BookingSearch.SearchReturn
		Dim oiVectorConnectSearchRequest As New ivci.BookingAdjustmentSearchRequest

		Try

			Dim sBookingAdjustmentTypesCSV As String = oBookingSearch.Params.BookingAdjustmentsCSV
			Dim iDepartureGeoLevel1ID As Integer = BookingAdjustment.GetGeographyLevel1ID(oBookingSearch, oBookingSearch.DepartingFromID + 1000000)
			Dim iArrivalGeoLevel1ID As Integer = BookingAdjustment.GetGeographyLevel1ID(oBookingSearch, oBookingSearch.ArrivingAtID)

			With oiVectorConnectSearchRequest

				.LoginDetails = oBookingSearch.LoginDetails

				.BookingAdjustmentTypesCSV = sBookingAdjustmentTypesCSV
				.DepartureGeographyLevel1ID = iDepartureGeoLevel1ID
				.ArrivalGeographyLevel1ID = iArrivalGeoLevel1ID
				.DepartureDate = oBookingSearch.DepartureDate
				.TotalPassengers = oBookingSearch.TotalAdults + oBookingSearch.TotalChildren

				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oSearchReturn.OK = False
					oSearchReturn.Warning.AddRange(oWarnings)
				End If
			End With

			If oSearchReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.BookingAdjustmentSearchResponse)(oiVectorConnectSearchRequest)

				Dim oSearchResponse As New ivci.BookingAdjustmentSearchResponse


				If oIVCReturn.Success Then

					oSearchResponse = CType(oIVCReturn.ReturnObject, ivci.BookingAdjustmentSearchResponse)

					If oSearchResponse.BookingAdjustmentTypes.Count > 0 Then

						oSearchReturn.BookingAdjustments = oSearchResponse.BookingAdjustmentTypes

					End If

				Else

					oSearchReturn.OK = False
					oSearchReturn.Warning = oIVCReturn.Warning

				End If

			End If

		Catch ex As Exception
			oSearchReturn.OK = False
			oSearchReturn.Warning.Add(ex.ToString)
			FileFunctions.AddLogEntry("iVectorConnect/TransferSearch", "TransferSearchException", ex.ToString)
		End Try

		Return oSearchReturn

	End Function

	Public Shared Function GetGeographyLevel1ID(ByVal BookingSearch As BookingSearch, ByVal LocationID As Integer) As Integer

		Dim iGeographyLevel1ID As Integer = 0

		If Not LocationID = 0 Then
			If LocationID < 0 Then
				iGeographyLevel1ID = BookingSearch.Lookups.GetLocationFromResort(LocationID * -1).GeographyLevel1ID
			ElseIf LocationID > 1000000 Then
				iGeographyLevel1ID = BookingSearch.Lookups.Airports.Where(Function(airport) airport.AirportID = (LocationID - 1000000)).FirstOrDefault().GeographyLevel1ID
			Else
				iGeographyLevel1ID = BookingSearch.Lookups.GetLocationFromRegion(LocationID).GeographyLevel1ID
			End If
		End If

		Return iGeographyLevel1ID

	End Function

	Public Shared Sub Save(ByVal BookingAdjustments As Generic.List(Of ivci.BookingAdjustmentSearchResponse.BookingAdjustmentType))

		'Save results to SearchDetails
		BookingBase.SearchDetails.BookingAdjustments.TotalAdjustments = BookingAdjustments.Count
		BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes = BookingAdjustments
	End Sub

#Region "CalculateBookingAdjustments"
	Public Shared Function CalculateBookingAdjustments(ByVal CSV As String) As Decimal

		If Not BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes Is Nothing Then
			Dim AdjustmentValue As Decimal = 0

			For Each oAdjustmentType As ivci.BookingAdjustmentSearchResponse.BookingAdjustmentType In BookingBase.SearchDetails.BookingAdjustments.AdjustmentTypes
				If CSV.Contains(oAdjustmentType.AdjustmentType.ToString) Then
					AdjustmentValue += oAdjustmentType.AdjustmentAmount
				End If
			Next

			Return AdjustmentValue
		Else
			Return 0
		End If
		



	End Function
#End Region

	Public Class Results
		Public TotalAdjustments As Integer
		Public AdjustmentTypes As Generic.List(Of ivci.BookingAdjustmentSearchResponse.BookingAdjustmentType)
	End Class

End Class

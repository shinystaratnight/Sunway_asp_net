Imports System.Xml.Serialization
Imports System.Xml
Imports Intuitive.Domain.Financial

Namespace [Property]

	<XmlType("PropertySearchResponse")>
	<XmlRoot("PropertySearchResponse")>
	Public Class SearchResponse
		Implements Interfaces.IVectorConnectResponse

		Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property ArrivalDate As Date
		Public Property Duration As Integer

		Property PropertyResults As New Generic.List(Of PropertyResult)
		Property RoomMappingElapsedTime As Long

		Public Class PropertyResult
			Public Property BookingToken As String
			Public Property PropertyReferenceID As Integer
			Public Property GeographyLevel1ID As Integer
			Public Property GeographyLevel2ID As Integer
			Public Property GeographyLevel3ID As Integer
			Public Property SearchResponseXML As New XmlDocument
			Public Property RoomTypes As New Generic.List(Of RoomType)
			Public Property RoomGroups As New List(Of RoomGroup)
			Public Property Rental As Boolean
			Public Property PropertyName As String
			Public Property Country As String
			Public Property Region As String
			Public Property Resort As String
            <XmlArrayItem("Image")>
            <XmlArray("OtherImages")>
            Public Property OtherImages As List(Of String)

			''' <summary>
			''' Source
			''' </summary>
			''' <returns>Source</returns>
			Public Property Source As String

			''' <summary>
			''' Contract ID
			''' </summary>
			''' <returns>Contract ID</returns>
			Public Property ContractID As Integer

			'not returned
			Public Property TokenHelpers As TokenHelpers

            Public Property Errata As New Generic.List(Of Support.Erratum)
		End Class

		'the should serialize functions are .Net functions that allow custom serialisation. 
		Public Function ShouldSerializeArrivalDate() As Boolean
			Return PropertyResults.Count > 0
		End Function

		Public Function ShouldSerializeDuration() As Boolean
			Return PropertyResults.Count > 0
		End Function

#Region "Helper Classes"

		Public Class RoomGroup
			Public Property RoomGroupID As Integer
			Public Property RoomGroupName As String
			Public Property RoomGroupMealBases As List(Of RoomGroupMealBasis)
			Public Property MealBasisCount As Integer
		End Class

		Public Class RoomGroupMealBasis
			Public Property MealBasisID As Integer
			Public Property LeadInPrice As Decimal
		End Class

		Public Class RoomGroupComparer
			Implements IComparer(Of RoomGroup)

			Private ReadOnly MealBasisOrder As List(Of Integer)

			Public Sub New(aOrder As List(Of Integer))
				MealBasisOrder = aOrder
			End Sub

			Public Function Compare(x As RoomGroup, y As RoomGroup) As Integer Implements IComparer(Of RoomGroup).Compare
				Dim iCompare As Integer = 0

				For Each iMealBasis As Integer In MealBasisOrder
					Dim oXMealBasis As RoomGroupMealBasis = x.RoomGroupMealBases.FirstOrDefault(Function(mb) mb.MealBasisID = iMealBasis)
					Dim oYMealBasis As RoomGroupMealBasis = y.RoomGroupMealBases.FirstOrDefault(Function(mb) mb.MealBasisID = iMealBasis)

					If oXMealBasis IsNot Nothing AndAlso oYMealBasis IsNot Nothing Then
						If (oXMealBasis.LeadInPrice > oYMealBasis.LeadInPrice) Then
							Return 1
						ElseIf (oYMealBasis.LeadInPrice > oXMealBasis.LeadInPrice) Then
							Return -1
						End If
					ElseIf oXMealBasis IsNot Nothing Then
						Return -1
					ElseIf oYMealBasis IsNot Nothing Then
						Return 1
					End If
				Next

				If x.RoomGroupName > y.RoomGroupName Then
					Return 1
				ElseIf y.RoomGroupName > x.RoomGroupName Then
					Return -1
				Else
					Return x.RoomGroupID - y.RoomGroupID
				End If

			End Function
		End Class

		Public Class RoomType
			Public Property Seq As Integer
			Public Property RoomBookingToken As String
			Public Property TPReference As String
			Public Property MealBasisID As Integer
			Public Property MealBasisCustomerNotes As String
			Public Property MealBasisGroupID As Integer
			Public Property RoomTypeID As Integer
			Public Property RoomType As String
			Public Property RoomGroupID As Integer
			Public Property RoomView As String
			Public Property RoomViewID As Integer
			Public Property RoomDescription As String
			<XmlArrayItem("DescriptionLine")>
			Public Property Description As List(Of String)
			Public Property AvailableRooms As Integer
			Public Property DiscountID As Integer
			Public Property Discount As Decimal
			Public Property Saving As Decimal
			Public Property SubTotal As Decimal
			Public Property Total As Decimal
			Public Property PayLocalTotal As Decimal
			Public Property TotalCommission As Decimal
			Public Property Cancellations As Generic.List(Of Cancellation)
			Public Property SupplierDetails As New Support.SupplierDetails
			Public Property Errata As New Generic.List(Of Support.Erratum)
			Public Property Adjustments As New Generic.List(Of Adjustment)
			Public Property OptionalSupplements As New Generic.List(Of OptionalSupplement)
			Public Property OptionalSpecialOffers As New Generic.List(Of OptionalSpecialOffer)
			Public Property DailyRates As New Generic.List(Of DailyRate)
			Public Property SpecialOffer As String
			Public Property NonRefundable As Boolean
			Public Property OnRequest As Boolean
			Public Property RegionalTax As Decimal
			Public Property Taxes As New Generic.List(Of Support.Tax)
			Public Property Source As String

			<XmlArrayItem("FlightID")>
			Public Property InvalidFlightResults As New List(Of Integer)

			Public Property BenefitRateCodeID As String

			Public Property SupplierID As Integer
            Public Property PayLocalAvailable As Boolean
            Public Property PayLocalRequired As Boolean

            'not returned 
            Public Property RoomTokenHelpers As RoomTokenHelpers

            Public Property ItineraryInformation As New ItineraryInformation

			Public Property LocalCurrencyID As Integer

        End Class

		Public Class Adjustment
			Public Property AdjustmentType As String
			Public Property AdjustmentID As Integer
			Public Property AdjustmentName As String
			Public Property Cost As Decimal
			Public Property Total As Decimal
			Public Property PayLocal As Boolean
			Public Property CustomerNotes As String
		End Class



		Public Class OptionalSupplement
			Public Property ContractSupplementID As Integer
			Public Property Supplement As String
			Public Property PaxType As String
			Public Property PayableLocal As Boolean
			Public Property RateCalculation As String
			Public Property Value As Decimal
			Public Property Adult As Decimal
			Public Property Child As Decimal
			Public Property Infant As Decimal
			Public Property CustomerNotes As String
		End Class

		Public Class OptionalSpecialOffer
			Public Property ContractSpecialOfferID As Integer
			Public Property OfferName As String
			Public Property OfferCategory As String
			Public Property OfferType As String
			Public Property OfferTypeID As Integer
			Public Property TotalAmount As Decimal
			Public Property OfferCustomerNotes As String
		End Class

		Public Class DailyRate
			Public Property Rate As Decimal
			Public Property Discount As Decimal
		End Class

		Public Class TokenHelpers
			Public Property ArrivalDate As Date
			Public Property Duration As Integer
		End Class

		Public Class RoomTokenHelpers
			Public Property OnlineSearchID As Integer
			Public Property PropertyRoomTypeID As Integer
			Public Property PropertyID As Integer
			Public Property CurrencyID As Integer
			Public Property TotalLocalCost As Decimal
			Public Property GrossCost As Decimal
            Public Property TotalPrice As Decimal
			Public Property CommissionPercentage As Decimal
			Public Property BindingOverridePrice As Decimal
			Public Property RoomTypeCode As String
			Public Property ContractID As Integer
			Public Property MarginID As Integer
			Public Property TPRateCode As String
			Public Property SupplierID As Integer
            Public Property ChannelManagerContractID As Integer
            Public Property OverridePaymentTermProfileID As Integer
            Public Property OverrideCancellationTermProfileID As Integer
		End Class

#End Region

	End Class

    Public Class ItineraryInformation
        Public Property PropertyRoomTypeID As Integer
    End Class
End Namespace

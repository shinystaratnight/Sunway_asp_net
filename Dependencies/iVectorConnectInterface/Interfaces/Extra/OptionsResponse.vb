Imports System.Xml.Serialization

Namespace Extra

	<XmlRoot("ExtraOptionsResponse")>
	Public Class OptionsResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus
		Public Property Options As New Generic.List(Of [Option])

		Public Class [Option]
			Public Property BookingToken As String
			Public Property SupplierID As Integer
			Public Property ExtraCategoryID As Integer
			Public Property ExtraCategory As String
			Public Property Notes As String
			Public Property ExtraDurationID As Integer
			Public Property Duration As Integer
			Public Property DateRequired As Boolean
			Public Property TimeRequired As Boolean
			Public Property AgeRequired As Boolean
			Public Property UseDate As Date
			Public Property UseTime As String
			Public Property EndDate As Date
			Public Property EndTime As String
			Public Property OccupancyRules As Boolean
			Public Property MinPassengers As Integer
			Public Property MaxPassengers As Integer
			Public Property MinAdults As Integer
			Public Property MaxAdults As Integer
			Public Property MinChildren As Integer
			Public Property MaxChildren As Integer
			Public Property MinimumAge As Integer
			Public Property MaximumAge As Integer
			Public Property MinChildAge As Integer
			Public Property MaxChildAge As Integer
			Public Property SeniorAge As Integer
			Public Property PricingType As String
			Public Property ExtraPrice As Decimal
			Public Property AdultPrice As Decimal
			Public Property ChildPrice As Decimal
			Public Property InfantPrice As Decimal
			Public Property SeniorPrice As Decimal
			Public Property TotalComission As Decimal
			Public Property TotalPrice As Decimal
			Public Property MoreInfoKey As String
			Public Property Description As String
			Public Property MultiBook As Boolean
			Public Property MaximumQuantity As Integer
			Public Property OptionalSupplements As Generic.List(Of OptionalSupplement)
			Public Property AttractionDetails As AttractionDetails
		End Class

		Public Class OptionalSupplement
			Public Property SupplementBookingToken As String
			Public Property ExtraSupplementID As Integer
			Public Property PricingType As String
			Public Property ExtraPrice As Decimal
			Public Property AdultPrice As Decimal
			Public Property ChildPrice As Decimal
			Public Property InfantPrice As Decimal
		End Class

		Public Class AttractionDetails
			Public Property Image As String
			Public Property ImageThumbnail As String
			<XmlArrayItem("OtherImage")>
			Public Property OtherImages As Generic.List(Of String)
			Public Property URL As String
			Public Property Description As String
			Public Property TicketType As String
			Public Property IndividualInfoItems As Generic.List(Of IndividualInfoItem)
			<XmlArrayItem("GroupInfoItem")>
			Public Property GroupInfoItems As Generic.List(Of IndividualInfoItem)
			<XmlArrayItem("Time")>
			Public Property Times As Generic.List(Of String)
			Public Property CancellationFees As Generic.List(Of CancellationFee)
		End Class

		Public Class IndividualInfoItem
			Public Property Name As String
			Public Property ID As Integer
			Public Property Type As String
			<XmlArrayItem("Value")>
			Public Property Value As Generic.List(Of String)
		End Class

		Public Class CancellationFee
			Public Property StartDate As Date
			Public Property EndDate As Date
			Public Property Percentage As String
			Public Property Amount As Decimal
		End Class

	End Class

End Namespace
Imports System.Xml.Serialization

Namespace Extra

	<XmlRoot("ExtraAvailabilityResponse")>
	Public Class AvailabilityResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

		Public Property ExtraTypes As Generic.List(Of ExtraType)

		Public Class ExtraType
			Public Property ExtraTypeID As Integer
			Public Property ExtraSubTypes As Generic.List(Of ExtraSubType)
			Public Property Notes As String
		End Class

		Public Class ExtraSubType
			Public Property ExtraSubTypeID As Integer
			Public Property Extras As Generic.List(Of Extra)
		End Class

		Public Class Extra
			Public Property ExtraID As Integer
			Public Property ExtraName As String
			Public Property ExtraSubName As String
			Public Property SupplierID As Integer
			Public Property Notes As String
			Public Property RequiresFullPaxInfo As Boolean
			Public Property Languages As Generic.List(Of Language)
			Public Property ProductAttributes As Generic.List(Of ProductAttribute)
			Public Property PickupPoints As Generic.List(Of PickupPoint)
			Public Property UseDates As Generic.List(Of UseDate)
		End Class

		Public Class Language
			Public Property LanguageID As Integer
			Public Property LanguageName As String
		End Class

		Public Class ProductAttribute
			Public Property ProductAttributeID As Integer
			Public Property ProductAttributeName As String
		End Class

		Public Class PickupPoint
			Public Property PickupPointID As Integer
			Public Property PickupTime As String
			Public Property PickupLocation As String
		End Class

		Public Class UseDate
			Public Property UseDate As Date
			Public Property BookingToken As String
		End Class

	End Class

	Public Class AvailabilityGuestTokenDef
		Public Property GuestConfiguration As Support.GuestConfiguration
		Public Property UseDate As Date
	End Class

End Namespace
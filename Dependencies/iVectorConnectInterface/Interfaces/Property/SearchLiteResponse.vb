Imports System.Xml.Serialization

Namespace [Property]

	<XmlType("PropertySearchResponse")>
	<XmlRoot("PropertySearchResponse")>
	Public Class SearchLiteResponse
		Implements Interfaces.iVectorConnectResponse

		Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus

		Property PropertyResults As New List(Of PropertyResult)

		Public Class PropertyResult
			Public Property BookingToken As String
			Public Property PropertyReferenceID As Integer
			Public Property GeographyLevel1ID As Integer
			Public Property GeographyLevel2ID As Integer
			Public Property GeographyLevel3ID As Integer
			Public Property RoomTypes As New List(Of RoomType)
			Public Property PropertyName As String
		End Class

		Public Class RoomType
			Public Property Seq As Integer
			Public Property RoomBookingToken As String
			Public Property MealBasisID As Integer
			Public Property PropertyRoomTypeID As Integer
			Public Property RoomTypeID As Integer
			Public Property RoomType As String
			Public Property RoomView As String
			Public Property RoomViewID As Integer
			Public Property AvailableRooms As Integer
			Public Property Total As Decimal
			Public Property TotalCommission As Decimal
			Public Property Adjustments As New List(Of Adjustment)
			Public Property NonRefundable As Boolean
			Public Property OnRequest As Boolean
		End Class

		Public Class Adjustment
			Public Property AdjustmentType As String
			Public Property AdjustmentID As Integer
			Public Property AdjustmentName As String
			Public Property Total As Decimal
			Public Property PayLocal As Boolean
		End Class

	End Class

End Namespace
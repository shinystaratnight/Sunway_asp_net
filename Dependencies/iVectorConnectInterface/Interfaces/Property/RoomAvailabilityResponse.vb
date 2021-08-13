Imports System.Xml.Serialization

Namespace [Property]

    Public Class RoomAvailabilityResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

        Public Property PropertyResults As New Generic.List(Of PropertyResult)

        Public Class PropertyResult

            Public Property PropertyReferenceID As Integer
            Public Property RoomTypes As New Generic.List(Of RoomType)

        End Class





        Public Class RoomType
            Public Property Seq As Integer
            Public Property PropertyRoomTypeID As Integer
            Public Property RoomTypeID As Integer
            Public Property RoomViewID As Integer
            Public Property RoomType As String
            Public Property AvailableRooms As Integer
			Public Property MinOccupancy As Integer
			Public Property MaxOccupancy As Integer
            Public Property Unavailable As Boolean
            Public Property MinStay As Integer

            <XmlArrayItem("MealBasisID")>
            Public Property MealBases As New Generic.List(Of Integer)

        End Class


    End Class

End Namespace

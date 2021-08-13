Imports iVectorConnectInterface.Interfaces
Imports System.Xml.Serialization

Namespace Itinerary
	<XmlType("OwnArrangement")>
	<XmlRoot("OwnArrangement")>
	Public Class OwnArrangementsResponse
		Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

		Public Property ReturnStatus As New ReturnStatus Implements iVectorConnectResponse.ReturnStatus
		Public Property Title As String

	End Class
End Namespace

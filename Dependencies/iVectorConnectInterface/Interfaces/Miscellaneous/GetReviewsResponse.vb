
Imports System.Xml
Imports System.Xml.Serialization

Public Class GetReviewsResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property ResultsXML As XmlElement

#End Region

End Class

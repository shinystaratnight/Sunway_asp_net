Imports System.Xml
Imports System.Xml.Serialization

Public Class CustomQueryResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As new ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	'Public Property Query As String
	'<XmlIgnore()>
	Public Property CustomXML As XmlDocument

End Class

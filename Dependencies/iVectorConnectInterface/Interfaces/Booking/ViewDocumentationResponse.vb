Imports System.Xml.Serialization

Public Class ViewDocumentationResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	<XmlArrayItem("DocumentPath")>
	Public DocumentPaths As New Generic.List(Of String)

	'Public Property DocumentPath As String

End Class

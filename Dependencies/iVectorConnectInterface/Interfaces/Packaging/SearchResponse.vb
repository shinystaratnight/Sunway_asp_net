Imports System.Xml.Serialization
Imports System.Xml

Namespace [Package]

	<XmlRoot("PackageSearchResponse")>
 Public Class SearchResponse
		Implements Interfaces.IVectorConnectResponse

		Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property LeadInPrice As Decimal
        
		Public Property ArrivalDate As Date
		Public Property Duration As Integer

		Public Property PropertyResults As Generic.List(Of [Property].SearchResponse.PropertyResult)

        Public Property Flights As Generic.List(Of Flight.SearchResponse.Flight)

    End Class
End Namespace

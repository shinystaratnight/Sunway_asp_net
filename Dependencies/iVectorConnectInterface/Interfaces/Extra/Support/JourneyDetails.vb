Imports System.Xml.Serialization

Namespace Extra

	Public Class JourneyDetails

		Public PNR As String

		<XmlArrayItem("TicketURL")>
		Public TicketURLs As New List(Of String)

		Public Sub New()

		End Sub
	End Class

End Namespace
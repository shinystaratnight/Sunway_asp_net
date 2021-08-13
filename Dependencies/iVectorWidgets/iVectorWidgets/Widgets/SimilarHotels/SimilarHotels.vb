Imports Intuitive
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class SimilarHotels
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		Dim oTestHotel As New SimilarHotels.SimilarHotel
		With oTestHotel
			.Name = "Test"
			.PropertyReferenceID = 1
			.Rating = 3
			.Region = "Test Region"
			.Resort = "Test Resort"
		End With

		Dim oTestHotelTwo As New SimilarHotels.SimilarHotel
		With oTestHotelTwo
			.Name = "Test"
			.PropertyReferenceID = 2
			.Rating = 3
			.Region = "Test Region"
			.Resort = "Test Resort"
		End With

		Dim oTestHotelThree As New SimilarHotels.SimilarHotel
		With oTestHotelThree
			.Name = "Test"
			.PropertyReferenceID = 3
			.Rating = 3
			.Region = "Test Region"
			.Resort = "Test Resort"
		End With

		Dim oSimilarHotelResults As New SimilarHotelResults
		oSimilarHotelResults.SimilarHotels.Add(oTestHotel)
		oSimilarHotelResults.SimilarHotels.Add(oTestHotelTwo)
		oSimilarHotelResults.SimilarHotels.Add(oTestHotelThree)

		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oSimilarHotelResults)
		Me.XSLTransform(oXML, res.SimilarHotels, writer)

	End Sub

	Public Class SimilarHotelResults
		Public Property SimilarHotels As New Generic.List(Of SimilarHotel)
	End Class

	Public Class SimilarHotel
		Public Property PropertyReferenceID As Integer
		Public Property Name As String
		Public Property Rating As Decimal
		Public Property Region As String
		Public Property Resort As String
	End Class

End Class


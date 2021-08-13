Imports Intuitive.Web.Widgets
Imports Intuitive
Imports System.Linq
Imports System.Xml

Public Class Map
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oXML As New XmlDocument
		Me.XSLTransform(oXML, writer)

	End Sub

	'Public Shared Function PropertyMapPopupDetailsBase(ByVal oHotel As HotelResults.Hotel) As String

	'	Dim sbPopupHTML As New StringBuilder

	'	sbPopupHTML.AppendLine("<div class=""top""> </div>")

	'	'close button
	'	sbPopupHTML.AppendLine("<a class=""close"" href=""#"" onclick=""f.HidePopup();return false;""> </a>")

	'	'from price, name, resort, main image
	'	sbPopupHTML.AppendLine("<div><div class=""priceTag"">")
	'	sbPopupHTML.AppendLine("<span class=""from"">from</span>")
	'	sbPopupHTML.AppendFormatLine("<p class=""price""><span class=""currencySymbol"">{0}</span>{1}</p>", "$", Math.Ceiling(oHotel.MinPrice))
	'	sbPopupHTML.AppendLine("<span class=""icon""></span></div>")
	'	sbPopupHTML.AppendFormatLine("<h2>{0}</h2>", oHotel.Name)
	'	sbPopupHTML.AppendFormatLine("<h4 class=""resort"">{0}</h4></div>", oHotel.Resort)
	'	sbPopupHTML.AppendFormatLine("<img alt=""{2}"" class=""mainImg"" src=""{0}{1}""/>", Config.CMSBaseURL, oHotel.MainImage.Image, oHotel.Name)

	'	sbPopupHTML.AppendFormatLine("<div class=""location"">{0}</div>", oHotel.Resort)

	'	'elipsis for long descriptions
	'	Dim oElipsis As String
	'	If oHotel.Summary.Length > 90 Then
	'		oElipsis = "..."
	'	Else
	'		oElipsis = ""
	'	End If

	'	'markdown summary
	'	Dim oXSL As Intuitive.WebControls.XSL = New Intuitive.WebControls.XSL
	'	oXSL.XSLTemplate = "/XSL/Markdown.xsl"
	'	oXSL.XSLParameters.AddParam("text", Intuitive.Functions.SafeSubString(oHotel.Summary, 90) & oElipsis)
	'	oXSL.XMLDocument = New XmlDocument
	'	Dim sSummary As String = Intuitive.Functions.RenderControlToString(oXSL)
	'	sbPopupHTML.AppendFormatLine("<div class=""description"">{0}</div>", sSummary)

	'	'star rating
	'	sbPopupHTML.AppendFormatLine("<p class=""rating""><span class=""starRating star{0}""> </span></p>", Math.Floor(oHotel.Rating).ToString + Intuitive.Functions.IIf(Math.Floor(oHotel.Rating + 0.5) = Math.Floor(oHotel.Rating), "", "half"))

	'	'view details button
	'	sbPopupHTML.AppendFormatLine("<a class=""button secondary"" href=""/property-details?propertyid={0}"">View Details</a><div style=""clear:both""> </div>", oHotel.PropertyReferenceID)

	'	Return sbPopupHTML.ToString

	'End Function

	Public Shared Function GetMap(MapPoints As Generic.List(Of MapPoint)) As String

		Dim oMapReturn As New MapReturn

		For Each oMapPoint As MapPoint In MapPoints
			oMapReturn.Markers.Add(oMapPoint)
		Next

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapReturn)

	End Function

	Public Class MapReturn
		Public Property Markers As Generic.List(Of MapPoint)
	End Class

	Public Class MapPoint
		Public Property Longitude As Decimal
		Public Property Latitude As Decimal
		Public Property ID As Integer
		Public Property Type As String
	End Class

End Class

Imports System.IO
Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web.Translation
Imports iw = Intuitive.Web
Imports System.Web.UI.HtmlControls
Imports Intuitive.Web

Public Class CarHire1
	Inherits UserControlBase

	
	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

		'Set our titles
		Dim sTitle As String = SafeString(Settings.GetValue("Title"))
		If sTitle <> "" Then
			Me.h2CarHire.InnerHtml = sTitle
		End If

		Dim sSubTitle As String = SafeString(Settings.GetValue("SubTitle"))
		If sSubTitle <> "" Then
			Me.pSubTitle.Visible = True
			Me.pSubTitle.InnerHtml = sSubTitle
		End If

		'Show or hide the section by default
		Dim bHidden As Boolean = SafeBoolean(Settings.GetValue("Hidden"))
		If bHidden Then
			Me.divCarHireContainer.Attributes("style") = "display:none;"
		End If

		'check if there is a custom script that needs to run on load
		Dim sScript As String = SafeString(Settings.GetValue("CustomScript"))
		If sScript <> "" Then
			Dim sb As New StringBuilder
			sb.Append("<script type=""text/javascript"">")
			sb.Append("int.ll.OnLoad.Run(function () {  ")
			sb.Append(sScript)
			sb.Append(" })")
			sb.Append("</script>")

			Me.divCustomScript.InnerHtml = sb.ToString
		End If

		'Generate CarDepot Dropdowns
		Dim oBasket As Intuitive.Web.BookingBasket = BookingBase.SearchBasket

		Me.divSelectedCar.InnerHtml = CarHire.GetSelectedCarHireAndExtras()

		Dim ResortID As Integer = 0
		'Get ResortID
		If oBasket.BasketProperties.Count > 0 Then
			ResortID = oBasket.BasketProperties(0).RoomOptions(0).GeographyLevel3ID
		ElseIf oBasket.BasketFlights.Count > 0 Then
			ResortID = BookingBase.Lookups.GetAirportGeographyLevel3IDs(oBasket.BasketFlights(0).Flight.ArrivalAirportID)(0)
		End If

		Me.ddlCarHirePickUpDepotID.ProcessKeyValuePairs(BookingBase.Lookups.ListCarHireDeposByResortID(ResortID))
		Me.ddlCarHireDropOffDepotID.ProcessKeyValuePairs(BookingBase.Lookups.ListCarHireDeposByResortID(ResortID))


		'Generate Main Driver Age's
		Dim sDriverAge As New StringBuilder

		For i As Integer = 25 To 65
			sDriverAge.Append(i.ToSafeString).Append("#")
		Next

		Me.ddlCarHireDriverAge.Options = sDriverAge.ToSafeString.Chop()


		'Generate Default dates from dates used in the search
		Me.hidCarHirePickUpDate.Value = DateFunctions.ShortDate(oBasket.FirstDepartureDate)
		If oBasket.BasketFlights.Count > 0 Then
			Me.hidCarHireDropOffDate.Value = DateFunctions.ShortDate(oBasket.BasketFlights(0).Flight.ReturnDepartureDate)
		ElseIf oBasket.BasketProperties.Count > 0 Then
			Me.hidCarHireDropOffDate.Value = DateFunctions.ShortDate(oBasket.BasketProperties(0).RoomOptions(0).DepartureDate)
		End If

		'the default driver country id will match the selling geography of the site
		Me.hidDefaultDriverCountryID.Value = SafeString(BookingBase.Params.SellingGeographyLevel1ID)

		'check if we need to auto search for a specific search mode
		Dim bAutoSearch As Boolean = False
		Dim sSearchModeAutoSearch As String = Settings.GetValue("SearchModeAutoSearch")
		If sSearchModeAutoSearch <> "" Then
			Dim aSearchModeAutoSearches As String() = sSearchModeAutoSearch.Split("#"c)

			For Each sSearchMode As String In aSearchModeAutoSearches
				If sSearchMode = BookingBase.SearchDetails.SearchMode.ToString Then
					bAutoSearch = True
				End If
			Next

		End If

		Me.hidAutoSearch.Value = bAutoSearch.ToString

		Me.hidCarHireResultsPage.Value = SafeString(SafeBoolean(Settings.GetValue("CarHireResultsPage")))

	End Sub

	

End Class
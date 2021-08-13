Imports System.IO
Imports Intuitive
Imports Intuitive.Functions
Imports iw = Intuitive.Web



Public Class SearchToolUserControl
	Inherits UserControlBase


	Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)


		'css class override
		If Settings.GetValue("CSSClassOverride") <> "" Then
			Me.divSearch.Attributes("class") = Settings.GetValue("CSSClassOverride")
		End If


		'set search title
		Me.h2_SearchTitle.InnerText = Settings.GetValue("SearchTitle")


		'set search modes
		Dim aSearchModes As String() = Settings.GetValue("SearchModes").ToLower.Split(","c)

		Me.li_SearchMode_FlightPlusHotel.Visible = aSearchModes.Contains("flightplushotel")
		Me.li_SearchMode_Hotel.Visible = aSearchModes.Contains("hotel")
		Me.li_SearchMode_Flight.Visible = aSearchModes.Contains("flight")

	End Sub



End Class
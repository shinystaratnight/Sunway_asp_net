Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports Intuitive.Functions
Imports System.ComponentModel

Public Class HotelRequests
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'we don't want this on flight only or transfer only booknig journey
        If BookingBase.Basket.BasketProperties.Count = 0 Then Exit Sub

		'1.build control
		Dim sControlOverride As String = GetSetting(eSetting.ControlOverride)
		If SafeString(sControlOverride) <> "" Then Me.URLPath = sControlOverride
		Dim sControlPath As String = Me.URLPath & "/hotelrequests.ascx"
		Dim oControl As New Control
		Try
			oControl = Me.Page.LoadControl(sControlPath)
		Catch ex As Exception
			oControl = Me.Page.LoadControl("/widgetslibrary/HotelRequests/HotelRequests.ascx")
		End Try
		CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
		 Me.DrawControl(writer, oControl)


	End Sub


	Public Function AddRequestToBasket(ByVal sRequest As String, ByVal sArrivalTime As String) As String

		Dim sHotelRequest As String
		If sArrivalTime <> "" Then
			sHotelRequest = "Arrival Time: " & sArrivalTime & Environment.NewLine & sRequest
		Else
			sHotelRequest = sRequest
		End If

		If BookingBase.Basket.BasketProperties.Count > 0 Then
			BookingBase.Basket.BasketProperties(0).RoomOptions(0).Request = sHotelRequest
		End If

		'return so we know it is complete
		Return "Success"

	End Function

	Public Enum eSetting
		<Title("Control override")>
		<Description("Specificies the path of the ascx control override to use instead of the default")>
		<DeveloperOnly(True)>
		ControlOverride
	End Enum

End Class

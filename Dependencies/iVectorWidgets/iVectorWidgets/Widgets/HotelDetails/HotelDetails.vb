Imports System.Xml
Imports Intuitive.Web.Widgets
Imports Intuitive.Web

Public Class HotelDetails
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("CMSBaseURL", "gointernational")

		Dim oXML As New XmlDocument
		Me.XSLTransform(oXML, res.HotelDetails, writer, oXSLParams)

	End Sub

	'#Region "Add To Basket"

	'	Public Function AddToBasket(ByVal PropertyBookingToken As String, ByVal RoomBookingTokens As String) As Boolean

	'		'Split tokens out
	'		Dim oRoomBookingTokens As New Generic.List(Of String)
	'		For Each sRoomBookingToken As String In RoomBookingTokens.Split(","c)
	'			oRoomBookingTokens.Add(sRoomBookingToken)
	'		Next

	'		'Clear out the basket first
	'		Basket.RemovePropertiesFromBasket()
	'		'Reset the prebook flag
	'		UserSession.Basket.PreBooked = False

	'		'Create the basket property 
	'		Dim oBasketProperty As iVectorConnectWebBooking.Basket.BasketProperty = _
	'		 HotelResults.PropertyToBasketProperty(PropertyBookingToken, oRoomBookingTokens)

	'		'Add the property to the basket 
	'		If oBasketProperty IsNot Nothing Then
	'			UserSession.Basket.AddPropertyToBasket(oBasketProperty)
	'		End If

	'		If UserSession.Basket.BasketProperties.Count = 0 Then
	'			Return False
	'		Else
	'			Return True
	'		End If

	'	End Function

	'#End Region


End Class
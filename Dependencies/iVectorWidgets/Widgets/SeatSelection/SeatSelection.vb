Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports ivci = iVectorConnectInterface

Public Class SeatSelection
	Inherits WidgetBase

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. Get xml from object type
		Dim oXML As System.Xml.XmlDocument = BookingBase.SearchBasket.XML

		'2. Create params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		With oXSLParams
			.AddParam("CurrencySymbol", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
			.AddParam("CurrencySymbolPosition", Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
		End With

		'3. Transform
		Me.XSLTransform(oXML, XSL.SetupTemplate(res.SeatSelection, True, True), writer, oXSLParams)

	End Sub


	Public Shared Sub AddSeatsToBasket(ByVal sKeyValuePair As String)

		Dim oParams As Hashtable = Intuitive.Functions.Web.ConvertQueryStringToHashTable(sKeyValuePair)

		Dim oFlight As BookingFlight.BasketFlight = BookingBase.SearchBasket.BasketFlights.Last()

		Dim iGuestID As Integer = 1
		For Each oGuestDetail As ivci.Support.GuestDetail In BookingBase.SearchBasket.GuestDetails

			Dim sBookingToken As String = Intuitive.Functions.SafeString(oParams("ddlSeats_" & iGuestID & "_Outbound")).Split("_"c)(0)

			If sBookingToken <> "0" AndAlso sBookingToken <> "" Then

				Dim oExtra As BookingFlight.BasketFlight.BasketFlightExtra = CreateBasketFlightExtra(sBookingToken, oGuestDetail.GuestID)
				oFlight.BasketFlightExtras.Add(oExtra)

			End If

			Dim sReturnBookingToken As String = Intuitive.Functions.SafeString(oParams("ddlSeats_" & iGuestID & "_Return")).Split("_"c)(0)

			If sReturnBookingToken <> "0" AndAlso sReturnBookingToken <> "" Then

				Dim oExtra As BookingFlight.BasketFlight.BasketFlightExtra = CreateBasketFlightExtra(sReturnBookingToken, oGuestDetail.GuestID)
				oFlight.BasketFlightExtras.Add(oExtra)

			End If

			iGuestID += 1
		Next


	End Sub

	Public Shared Function CreateBasketFlightExtra(ByVal sBookingToken As String, ByVal GuestID As Integer) As BookingFlight.BasketFlight.BasketFlightExtra

		Dim oExtra As New BookingFlight.BasketFlight.BasketFlightExtra
		oExtra.ExtraBookingToken = sBookingToken
		oExtra.QuantitySelected = 1
		oExtra.GuestID = GuestID
		oExtra.ExtraType = "Seat"

		Return oExtra

	End Function

End Class

Namespace Basket.Visitors

	Public Class MaxBasketComponentIDVisitor
		Implements Basket.Interfaces.IBasketVisitor

		Public Function Visit(Visitable As Interfaces.IBasketVisitable) As Integer Implements Interfaces.IBasketVisitor.Visit

			Dim oBookingBasket As BookingBasket = CType(Visitable, BookingBasket)

			Dim iMaxComponentID As Integer = 0

			iMaxComponentID = If(oBookingBasket.BasketCarHires.Count > 0, oBookingBasket.BasketCarHires.Max(Function(oCarHire) oCarHire.ComponentID), iMaxComponentID)
			iMaxComponentID = Math.Max(If(oBookingBasket.BasketExtras.Count > 0, oBookingBasket.BasketExtras.Max(Function(oExtra) oExtra.ComponentID), iMaxComponentID), iMaxComponentID)
			iMaxComponentID = Math.Max(If(oBookingBasket.BasketFlights.Count > 0, oBookingBasket.BasketFlights.Max(Function(oFlight) oFlight.ComponentID), iMaxComponentID), iMaxComponentID)
			iMaxComponentID = Math.Max(If(oBookingBasket.BasketProperties.Count > 0, oBookingBasket.BasketProperties.Max(Function(oProperty) oProperty.ComponentID), iMaxComponentID), iMaxComponentID)
			iMaxComponentID = Math.Max(If(oBookingBasket.BasketTransfers.Count > 0, oBookingBasket.BasketTransfers.Max(Function(oTransfer) oTransfer.ComponentID), iMaxComponentID), iMaxComponentID)

			Return iMaxComponentID

		End Function

	End Class

End Namespace
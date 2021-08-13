namespace Web.Template.Application.Quote.Adaptors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Adaptors;

    using BookRequest = iVectorConnectInterface.Flight.BookRequest;

    /// <summary>
    /// Class FlightQuoteAdaptor.
    /// </summary>
    public class FlightQuoteAdaptor : IQuoteCreateRequestAdaptor
    {
        public ComponentType ComponentType => ComponentType.Flight;

        public void Create(IBasketComponent basketComponent, QuoteRequest connectRequestBody)
        {
            var flight = (Flight)basketComponent;

            var flightRequest = new iVectorConnectInterface.Flight.BookRequest()
            {
                BookingToken = flight.BookingToken,
                ExpectedTotal = flight.TotalPrice,
                GuestIDs = flight.GuestIDs
            };

            if (flight.ReturnMultiCarrierDetails != null && flight.ReturnMultiCarrierDetails.BookingToken != string.Empty)
            {
                flightRequest.MultiCarrierOutbound = true;
                var returnFlightRequest = new BookRequest()
                {
                    BookingToken = flight.ReturnMultiCarrierDetails.BookingToken,
                    ExpectedTotal = flight.ReturnMultiCarrierDetails.Price,
                    GuestIDs = flight.GuestIDs,
                    MultiCarrierReturn = true
                };
                connectRequestBody.FlightBookings.Add(returnFlightRequest);
            }

            connectRequestBody.FlightBookings.Add(flightRequest);
        }
    }
}

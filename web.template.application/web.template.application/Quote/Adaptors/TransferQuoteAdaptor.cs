namespace Web.Template.Application.Quote.Adaptors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Adaptors;

    using BookRequest = iVectorConnectInterface.Transfer.BookRequest;

    /// <summary>
    /// Class TransferQuoteAdaptor.
    /// </summary>
    public class TransferQuoteAdaptor : IQuoteCreateRequestAdaptor
    {
        public ComponentType ComponentType => ComponentType.Transfer;

        public void Create(IBasketComponent basketComponent, QuoteRequest connectRequestBody)
        {
            var transfer = (Transfer)basketComponent;

            var outboundDetails = new iVectorConnectInterface.Transfer.BookRequest.OutboundJourneyDetails()
            {
                JourneyOrigin = transfer.DepartureParentName,
                AccommodationName = transfer.ArrivalParentName,
                FlightCode = transfer.OutboundJourneyDetails.FlightCode,
                PickupTime = transfer.OutboundJourneyDetails.Time,
            };
            var returnDetails = new iVectorConnectInterface.Transfer.BookRequest.ReturnJourneyDetails() { PickupTime = transfer.ReturnJourneyDetails.Time, FlightCode = transfer.ReturnJourneyDetails.FlightCode };

            var transferRequest = new BookRequest() { BookingToken = transfer.BookingToken, ExpectedTotal = transfer.TotalPrice, GuestIDs = transfer.GuestIDs, ReturnDetails = returnDetails, OutboundDetails = outboundDetails };

            connectRequestBody.TransferBookings.Add(transferRequest);
        }
    }
}

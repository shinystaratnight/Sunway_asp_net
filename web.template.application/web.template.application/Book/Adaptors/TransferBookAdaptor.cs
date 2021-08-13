namespace Web.Template.Application.Book.Adaptors
{
    using iVectorConnectInterface.Transfer;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;

    /// <summary>
    /// Class that builds Property search requests
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookRequestAdaptor" />
    /// <seealso cref="ISearchRequestAdapter" />
    public class TransferBookAdaptor : IBookRequestAdaptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferBookAdaptor" /> class.
        /// </summary>
        public TransferBookAdaptor()
        {
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public ComponentType ComponentType => ComponentType.Transfer;

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        public void Create(IBasketComponent component, iVectorConnectInterface.Basket.BookRequest connectRequestBody)
        {
            var transfer = (Transfer)component;

            var outboundDetails = new BookRequest.OutboundJourneyDetails()
                                      {
                                          JourneyOrigin = transfer.DepartureParentName, 
                                          AccommodationName = transfer.ArrivalParentName, 
                                          FlightCode = transfer.OutboundJourneyDetails.FlightCode, 
                                          PickupTime = transfer.OutboundJourneyDetails.Time, 
                                      };
            var returnDetails = new BookRequest.ReturnJourneyDetails() { PickupTime = transfer.ReturnJourneyDetails.Time, FlightCode = transfer.ReturnJourneyDetails.FlightCode };

            var transferRequest = new BookRequest() { BookingToken = transfer.BookingToken, ExpectedTotal = transfer.TotalPrice, GuestIDs = transfer.GuestIDs, ReturnDetails = returnDetails, OutboundDetails = outboundDetails };

            connectRequestBody.TransferBookings.Add(transferRequest);
        }
    }
}
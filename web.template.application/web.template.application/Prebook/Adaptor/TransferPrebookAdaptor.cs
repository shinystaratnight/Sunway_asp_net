namespace Web.Template.Application.Prebook.Adaptor
{
    using iVectorConnectInterface.Transfer;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Search;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class that builds Property search requests
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookRequestAdaptor" />
    /// <seealso cref="ISearchRequestAdapter" />
    public class TransferPrebookAdaptor : IPrebookRequestAdaptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPrebookAdaptor" /> class.
        /// </summary>
        public TransferPrebookAdaptor()
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
        public void Create(IBasketComponent component, iVectorConnectInterface.Basket.PreBookRequest connectRequestBody)
        {
            var transfer = (Transfer)component;

            var outboundDetails = new BookRequest.OutboundJourneyDetails() { FlightCode = transfer.OutboundJourneyDetails.FlightCode };
            var returnDetails = new BookRequest.ReturnJourneyDetails() { FlightCode = transfer.OutboundJourneyDetails.FlightCode };

            var transferRequest = new PreBookRequest()
                                      {
                                          BookingToken = transfer.BookingToken, 
                                          DepartureParentType = transfer.DepartureParentType, 
                                          DepartureParentID = transfer.DepartureParentId, 
                                          ArrivalParentType = transfer.ArrivalParentType, 
                                          ArrivalParentID = transfer.ArrivalParentId, 
                                          DepartureDate = transfer.OutboundJourneyDetails.Date, 
                                          DepartureTime = transfer.OutboundJourneyDetails.Time, 
                                          OneWay = transfer.OneWay, 
                                          OutboundDetails = outboundDetails, 
                                          ReturnDate = transfer.ReturnJourneyDetails.Date, 
                                          ReturnTime = transfer.ReturnJourneyDetails.Time, 
                                          ReturnDetails = returnDetails, 
                                          GuestConfiguration = new ivci.Support.GuestConfiguration() { Adults = transfer.Adults, Children = transfer.Children, Infants = transfer.Infants, ChildAges = transfer.ChildAges }
                                      };

            connectRequestBody.TransferBookings.Add(transferRequest);
        }
    }
}
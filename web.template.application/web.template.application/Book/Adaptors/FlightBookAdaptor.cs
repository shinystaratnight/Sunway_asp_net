namespace Web.Template.Application.Book.Adaptors
{
    using iVectorConnectInterface.Flight;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Support;

    /// <summary>
    /// Class that builds Property search requests
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Book.IBookRequestAdaptor" />
    public class FlightBookAdaptor : IBookRequestAdaptor
    {
        /// <summary>
        /// The configuration settings
        /// </summary>
        private readonly IConfiguration configurationSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightBookAdaptor"/> class.
        /// </summary>
        /// <param name="configurationSettings">The configuration settings.</param>
        public FlightBookAdaptor(IConfiguration configurationSettings)
        {
            this.configurationSettings = configurationSettings;
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public ComponentType ComponentType => ComponentType.Flight;

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        public void Create(IBasketComponent component, iVectorConnectInterface.Basket.BookRequest connectRequestBody)
        {
            var flight = (Flight)component;

            var flightRequest = new BookRequest()
                                    {
                                        BookingToken = flight.BookingToken,
                                        ExpectedTotal = flight.TotalPrice,
                                        GuestIDs = flight.GuestIDs,
                                        BookingBasketID = flight.BasketToken
                                    };

            if (flight.ReturnMultiCarrierDetails != null && flight.ReturnMultiCarrierDetails.BookingToken != string.Empty)
            {
                flightRequest.MultiCarrierOutbound = true;
                var returnFlightRequest = new BookRequest()
                                              {
                                                  BookingToken = flight.ReturnMultiCarrierDetails.BookingToken,
                                                  ExpectedTotal = flight.ReturnMultiCarrierDetails.Price,
                                                  GuestIDs = flight.GuestIDs,
                                                  MultiCarrierReturn = true,
                                                  BookingBasketID = flight.BasketToken
                                              };
                                   


                connectRequestBody.FlightBookings.Add(returnFlightRequest);
            }


            connectRequestBody.FlightBookings.Add(flightRequest);
        }
    }
}
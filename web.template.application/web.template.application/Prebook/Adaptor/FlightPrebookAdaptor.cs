namespace Web.Template.Application.Prebook.Adaptor
{
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Flight;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Search;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Class that builds Property search requests
    /// </summary>
    /// <seealso cref="ISearchRequestAdapter" />
    public class FlightPrebookAdaptor : IPrebookRequestAdaptor
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightPrebookAdaptor" /> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public FlightPrebookAdaptor(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
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
        public void Create(IBasketComponent component, iVectorConnectInterface.Basket.PreBookRequest connectRequestBody)
        {
            var flight = (Flight)component;

            var connectExtras = new List<PreBookRequest.Extra>();
            foreach (var subComponent in flight.SubComponents)
            {
                var flightExtra = (FlightExtra)subComponent;
                if ((flightExtra.QuantitySelected > 0 || IsAutomaticSeatSelection(flightExtra)) && !flightExtra.ReturnMultiCarrierExtra)
                {
                    var connectExtra = new PreBookRequest.Extra() { 
                        ExtraBookingToken = flightExtra.BookingToken, 
                        Quantity = IsAutomaticSeatSelection(flightExtra) ? 1 : flightExtra.QuantitySelected, 
                        GuestID = flightExtra.GuestID, 
                        RequestedExtraType = string.Empty 
                    };
                    connectExtras.Add(connectExtra);
                }
            }

            var flightRequest = new PreBookRequest()
            {
                BookingToken = flight.BookingToken,
                LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                FlightAndHotel = flight.SearchMode == SearchMode.FlightPlusHotel,
                Extras = connectExtras,
                BookingBasketID = flight.BasketToken
            };

            if (!string.IsNullOrEmpty(flight.ReturnMultiCarrierDetails?.BookingToken))
            {
                var returnConnectExtras = new List<PreBookRequest.Extra>();
                foreach (var subComponent in flight.SubComponents)
                {
                    var flightExtra = (FlightExtra)subComponent;
                    if ((flightExtra.QuantitySelected > 0 || IsAutomaticSeatSelection(flightExtra)) && flightExtra.ReturnMultiCarrierExtra)
                    {
                        var connectExtra = new PreBookRequest.Extra() 
                        { 
                            ExtraBookingToken = flightExtra.BookingToken,
                            Quantity = IsAutomaticSeatSelection(flightExtra) ? 1 : flightExtra.QuantitySelected,
                            GuestID = flightExtra.GuestID, 
                            RequestedExtraType = string.Empty 
                        };
                        returnConnectExtras.Add(connectExtra);
                    }
                }

                flightRequest.MultiCarrierOutbound = true;
                var returnFlightRequest = new PreBookRequest()
                {
                    LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                    BookingToken = flight.ReturnMultiCarrierDetails.BookingToken,
                    FlightAndHotel = flight.SearchMode == SearchMode.FlightPlusHotel,
                    MultiCarrierReturn = true,
                    Extras = returnConnectExtras,
                    BookingBasketID = flight.BasketToken
                };

                connectRequestBody.FlightBookings.Add(returnFlightRequest);
            }

            connectRequestBody.FlightBookings.Add(flightRequest);
        }

        private bool IsAutomaticSeatSelection(FlightExtra flightExtra)
        {
            return flightExtra.Description == "Automatic seat selection for entire party";
        }
    }
}
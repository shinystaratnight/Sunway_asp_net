namespace Web.Template.Application.Prebook.Adaptor
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Search;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Class that builds Property search requests
    /// </summary>
    /// <seealso cref="ISearchRequestAdapter" />
    public class PropertyPrebookAdaptor : IPrebookRequestAdaptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPrebookAdaptor" /> class.
        /// </summary>
        public PropertyPrebookAdaptor()
        {
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public ComponentType ComponentType => ComponentType.Hotel;

        /// <summary>
        /// Creates a search connect search request using a WebTemplate search model.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        public void Create(IBasketComponent component, PreBookRequest connectRequestBody)
        {
            var hotel = (Hotel)component;

            var propertyRequest = new ivci.Property.PreBookRequest() { BookingToken = hotel.BookingToken, ArrivalDate = hotel.ArrivalDate, Duration = hotel.Duration, FlightAndHotel = hotel.SearchMode == SearchMode.FlightPlusHotel };

            if (hotel.SubComponents != null)
            {
                foreach (ISubComponent subComponent in hotel.SubComponents)
                {
                    Room room = (Room)subComponent;

                    var roomRequest = new ivci.Property.PreBookRequest.RoomBooking()
                                          {
                                              RoomBookingToken = subComponent.BookingToken, 
                                              GuestConfiguration = new ivci.Support.GuestConfiguration() { Adults = room.Adults, Children = room.Children, Infants = room.Infants, ChildAges = room.ChildAges }
                                          };
                    propertyRequest.RoomBookings.Add(roomRequest);
                }
            }

            connectRequestBody.PropertyBookings.Add(propertyRequest);
        }
    }
}
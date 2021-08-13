namespace Web.Template.Application.Book.Adaptors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Search;

    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Class that builds Property search requests
    /// </summary>
    /// <seealso cref="ISearchRequestAdapter" />
    public class PropertyBookAdaptor : IBookRequestAdaptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBookAdaptor" /> class.
        /// </summary>
        public PropertyBookAdaptor()
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
        public void Create(IBasketComponent component, BookRequest connectRequestBody)
        {
            var hotel = (Hotel)component;

            var propertyRequest = new ivci.Property.BookRequest() { BookingToken = hotel.BookingToken, ArrivalDate = hotel.ArrivalDate, Duration = hotel.Duration, ExpectedTotal = hotel.TotalPrice, Request = hotel.Request };

            if (hotel.SubComponents != null)
            {
                foreach (ISubComponent subComponent in hotel.SubComponents)
                {
                    Room room = (Room)subComponent;

                    var roomRequest = new ivci.Support.RoomBooking() { RoomBookingToken = subComponent.BookingToken, GuestIDs = room.GuestIDs };
                    propertyRequest.RoomBookings.Add(roomRequest);
                }
            }

            connectRequestBody.PropertyBookings.Add(propertyRequest);
        }
    }
}
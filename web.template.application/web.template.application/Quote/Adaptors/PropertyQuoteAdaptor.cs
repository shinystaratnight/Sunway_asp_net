namespace Web.Template.Application.Quote.Adaptors
{
    using iVectorConnectInterface.Basket;

    using Web.Template.Application.Basket.Models.Components;
    using Web.Template.Application.Basket.Models.Components.SubComponent;
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Adaptors;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class PropertyQuoteAdaptor.
    /// </summary>
    public class PropertyQuoteAdaptor : IQuoteCreateRequestAdaptor
    {
        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public ComponentType ComponentType => ComponentType.Hotel;

        /// <summary>
        /// Creates the specified basket component.
        /// </summary>
        /// <param name="basketComponent">The basket component.</param>
        /// <param name="connectRequestBody">The connect request body.</param>
        public void Create(IBasketComponent basketComponent, QuoteRequest connectRequestBody)
        {
            var hotel = (Hotel)basketComponent;

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

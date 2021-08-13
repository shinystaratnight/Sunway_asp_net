
namespace Web.Template.Application.Quote.Adaptors
{
    using System.Collections.Generic;
    using System.Linq;

    using iVectorConnectInterface;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Adaptors;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Class QuoteRetrieveSearchAdaptor.
    /// </summary>
    public class QuoteRetrieveSearchAdaptor : IQuoteRetrieveSearchAdaptor
    {
        /// <summary>
        /// Creates the specified search model.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="quoteRetrieveResponse">The quote retrieve response.</param>
        public void Create(ISearchModel searchModel, QuoteRetrieveResponse quoteRetrieveResponse)
        {
            searchModel.DepartureDate = quoteRetrieveResponse.FirstDepartureDate;
            searchModel.Duration =
                (quoteRetrieveResponse.LastReturnDate - quoteRetrieveResponse.FirstDepartureDate).Days;

            if (searchModel.SearchMode == SearchMode.FlightPlusHotel || searchModel.SearchMode == SearchMode.Hotel)
            {
                var property = quoteRetrieveResponse.Properties.FirstOrDefault();
                if (property != null)
                {
                    searchModel.ArrivalType = LocationType.Resort;
                    searchModel.ArrivalID = property.GeographyLevel3ID;
                    searchModel.Rooms = new List<Room>();

                    foreach (var room in property.Rooms)
                    {
                        var searchRoom = new Room()
                        {
                            Adults = room.Adults,
                            Children = room.Children,
                            Infants = room.Infants,
                            ChildAges = new List<int>()
                        };

                        foreach (var guestId in room.GuestIDs)
                        {
                            var guest = quoteRetrieveResponse.GuestDetails.FirstOrDefault(guestDetail => guestDetail.GuestID == guestId);
                            if (guest != null && guest.Type == "Child")
                            {
                                searchRoom.ChildAges.Add(guest.Age);
                            }
                        }

                        searchModel.Rooms.Add(searchRoom);
                    }
                }
            }

            if (searchModel.SearchMode == SearchMode.FlightPlusHotel || searchModel.SearchMode == SearchMode.Flight)
            {
                var flight = quoteRetrieveResponse.Flights.FirstOrDefault();
                if (flight != null)
                {
                    searchModel.DepartureType = LocationType.Airport;
                    searchModel.DepartureID = flight.DepartureAirportID;
                }
            }

            if (searchModel.SearchMode == SearchMode.Flight)
            {
                var flight = quoteRetrieveResponse.Flights.FirstOrDefault();
                if (flight != null)
                {
                    searchModel.ArrivalType = LocationType.Airport;
                    searchModel.ArrivalID = flight.ArrivalAirportID;
                    searchModel.Rooms = new List<Room>();

                    var searchRoom = new Room()
                    {
                        Adults = quoteRetrieveResponse.Adults,
                        Children = quoteRetrieveResponse.Children,
                        Infants = quoteRetrieveResponse.Infants,
                        ChildAges = new List<int>()
                    };

                    foreach (var guestId in flight.GuestIDs)
                    {
                        var guest = quoteRetrieveResponse.GuestDetails.FirstOrDefault(guestDetail => guestDetail.GuestID == guestId);
                        if (guest != null && guest.Type == "Child")
                        {
                            searchRoom.ChildAges.Add(guest.Age);
                        }
                    }

                    searchModel.Rooms.Add(searchRoom);
                }
            }
        }
    }
}

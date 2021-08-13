namespace Web.Template.Application.Booking.Adapters
{
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Adapters;
    using Web.Template.Application.Interfaces.Booking.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class responsible for mapping from a connect search booking response booking to our domain booking result
    /// </summary>
    public class BookingSearchResultAdapter : IBookingSearchResultAdapter
    {
        /// <summary>
        /// Creates the booking search result.
        /// </summary>
        /// <param name="ivcBooking">The connect booking.</param>
        /// <returns>
        /// a booking search result
        /// </returns>
        public IBookingSearchResult CreateBookingSearchResult(ivci.SearchBookingsResponse.Booking ivcBooking)
        {
            IBookingSearchResult searchResult = new BookingSearchResult()
                                                    {
                                                        ArrivalDate = ivcBooking.ArrivalDate, 
                                                        AccountStatus = ivcBooking.AccountStatus, 
                                                        BookingDate = ivcBooking.BookingDate, 
                                                        BookingReference = ivcBooking.BookingReference, 
                                                        CurrencySymbol = ivcBooking.CurrencySymbol, 
                                                        CurrencySymbolPosition = ivcBooking.CurrencySymbolPosition, 
                                                        CustomerCurrencyId = ivcBooking.CustomerCurrencyID, 
                                                        Duration = ivcBooking.Duration, 
                                                        GeographyLevel1Id = ivcBooking.GeographyLevel1ID, 
                                                        GeographyLevel2Id = ivcBooking.GeographyLevel2ID, 
                                                        GeographyLevel3Id = ivcBooking.GeographyLevel3ID, 
                                                        LeadCustomerFirstName = ivcBooking.LeadCustomerFirstName, 
                                                        LeadCustomerLastName = ivcBooking.LeadCustomerLastName,
                                                        Resort = ivcBooking.hlpGeographyLevel3Name, 
                                                        Status = ivcBooking.Status, 
                                                        TotalPrice = ivcBooking.TotalPrice, 
                                                        TotalCommission = ivcBooking.TotalCommission, 
                                                        TotalOutstanding = ivcBooking.TotalOutstanding, 
                                                        TotalPaid = ivcBooking.TotalPaid, 
                                                        TotalPassengers = ivcBooking.TotalPax, 
                                                        TotalVatOnCommission = ivcBooking.TotalVATOnCommission, 
                                                        TradeReference = ivcBooking.TradeReference,
                                                        ComponentList = ivcBooking.ComponentSummary
                                                    };

            return searchResult;
        }
    }
}
namespace Web.Template.Application.BookingAdjustment.Factories
{
    using System.Linq;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class BookingAdjustmentSearchRequestFactory.
    /// </summary>
    public class BookingAdjustmentSearchRequestFactory : IBookingAdjustmentSearchRequestFactory
    {
        /// <summary>
        /// The booking component repository
        /// </summary>
        private readonly IBookingComponentRepository bookingComponentRepository;

        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingAdjustmentSearchRequestFactory" /> class.
        /// </summary>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="bookingComponentRepository">The booking component repository.</param>
        public BookingAdjustmentSearchRequestFactory(IConnectLoginDetailsFactory connectLoginDetailsFactory, IBookingComponentRepository bookingComponentRepository)
        {
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.bookingComponentRepository = bookingComponentRepository;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="searchModel">The booking adjustment search model.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        public iVectorConnectRequest Create(IBookingAdjustmentSearchModel searchModel)
        {
            var request = new ivci.CheckBookingAdjustmentRequest()
                              {
                                  LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current), 
                                  BrandID = searchModel.BrandId, 
                                  SalesChannelID = searchModel.SalesChannelId, 
                                  SellingCountryID = searchModel.SellingCountryId, 
                                  CustomerCurrencyID = searchModel.CustomerCurrencyId, 
                                  SellingExchangeRate = searchModel.SellingExchangeRate, 
                                  FirstDepartureDate = searchModel.FirstDepartureDate, 
                                  BookingDate = searchModel.BookingDate, 
                                  FlightCarrierID = searchModel.FlightCarrierId, 
                                  FlightSupplierID = searchModel.FlightSupplierId, 
                                  FlightCarrierType = searchModel.FlightCarrierType, 
                                  TotalPassengers = searchModel.TotalPassengers, 
                                  GeographyLevel3ID = searchModel.GeographyLevel3Id, 
                                  HasFlight = new[] { SearchMode.Flight, SearchMode.FlightPlusHotel }.Contains(searchModel.SearchMode), 
                                  HasProperty = new[] { SearchMode.Hotel, SearchMode.FlightPlusHotel }.Contains(searchModel.SearchMode), 
                              };

            this.SetupFlightComponent(searchModel, request);

            this.SetupPropertyComponent(searchModel, request);

            return request;
        }

        /// <summary>
        /// Setups the flight component.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="request">The request.</param>
        private void SetupFlightComponent(IBookingAdjustmentSearchModel searchModel, ivci.CheckBookingAdjustmentRequest request)
        {
            if (request.HasFlight)
            {
                var flightId = 0;
                BookingComponent firstOrDefault = this.bookingComponentRepository.FindBy(bc => bc.Name == "Flight").FirstOrDefault();
                if (firstOrDefault != null)
                {
                    flightId = firstOrDefault.Id;
                }

                var componentInfo = new ivci.CheckBookingAdjustmentRequest.ComponentInformation() { ComponentType = "Flight", BookingComponentID = flightId, TotalPrice = searchModel.FlightPrice };

                request.ComponentsInformation.Add(componentInfo);
            }
        }

        /// <summary>
        /// Setups the property component.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="request">The request.</param>
        private void SetupPropertyComponent(IBookingAdjustmentSearchModel searchModel, ivci.CheckBookingAdjustmentRequest request)
        {
            if (request.HasProperty)
            {
                var propertyId = 0;
                BookingComponent firstOrDefault = this.bookingComponentRepository.FindBy(bc => bc.Name == "Property").FirstOrDefault();
                if (firstOrDefault != null)
                {
                    propertyId = firstOrDefault.Id;
                }

                var componentInfo = new ivci.CheckBookingAdjustmentRequest.ComponentInformation() { ComponentType = "Property", BookingComponentID = propertyId, TotalPrice = searchModel.PropertyPrice };

                request.ComponentsInformation.Add(componentInfo);
            }
        }
    }
}
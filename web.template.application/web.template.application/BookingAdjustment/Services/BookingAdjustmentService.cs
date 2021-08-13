namespace Web.Template.Application.BookingAdjustment.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Web.Template.Application.BookingAdjustment.Models;
    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.Net.IVectorConnect;
    using Web.Template.Application.Prebook.Models;
    using Web.Template.Domain.Entities.Booking;

    using IBookingService = Web.Template.Application.Interfaces.Lookup.Services.IBookingService;
    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class BookingAdjustmentService.
    /// </summary>
    public class BookingAdjustmentService : IBookingAdjustmentService
    {
        /// <summary>
        /// The booking adjustment search request factory
        /// </summary>
        private readonly IBookingAdjustmentSearchRequestFactory bookingAdjustmentSearchRequestFactory;

        /// <summary>
        /// The booking service
        /// </summary>
        private readonly IBookingService bookingService;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The flight service
        /// </summary>
        private readonly IFlightService flightService;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingAdjustmentService" /> class.
        /// </summary>
        /// <param name="bookingAdjustmentSearchRequestFactory">The booking adjustment search request factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="bookingService">The booking service.</param>
        /// <param name="flightService">The flight service.</param>
        public BookingAdjustmentService(
            IBookingAdjustmentSearchRequestFactory bookingAdjustmentSearchRequestFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            IUserService userService,
            ISiteService siteService,
            IBookingService bookingService,
            IFlightService flightService)
        {
            this.bookingAdjustmentSearchRequestFactory = bookingAdjustmentSearchRequestFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.userService = userService;
            this.siteService = siteService;
            this.bookingService = bookingService;
            this.flightService = flightService;
        }

        /// <summary>
        /// Retrieves the results.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The adjustments</returns>
        public List<IAdjustment> RetrieveResult(string searchToken)
        {
            var adjustments = new List<IAdjustment>();

            try
            {
                byte[] data = Convert.FromBase64String(searchToken);
                DateTime tokenTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                if (tokenTime > DateTime.UtcNow.AddMinutes(-30))
                {
                    adjustments = (List<IAdjustment>)HttpContext.Current.Cache[searchToken];
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("BookingAdjustmentService", "Retrieve Result Exception", ex.ToString());
            }

            return adjustments;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <param name="bookingAdjustmentSearchModel">The booking adjustment search model.</param>
        /// <returns>The search return</returns>
        public IBookingAdjustmentSearchReturn Search(IBookingAdjustmentSearchModel bookingAdjustmentSearchModel)
        {
            var searchReturn = new BookingAdjustmentSearchReturn();

            var searchModel = this.SetupSearchModel(bookingAdjustmentSearchModel);
            var requestBody = this.bookingAdjustmentSearchRequestFactory.Create(searchModel);

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
                ivci.CheckBookingAdjustmentResponse response = ivcRequest.Go<ivci.CheckBookingAdjustmentResponse>();

                if (response.ReturnStatus.Success)
                {
                    searchReturn.Success = true;
                    foreach (var adjustment in response.BookingAdjustments)
                    {
                        IAdjustment bookingAdjustment = new BookingAdjustment()
                                                            {
                                                                AdjustmentType = adjustment.AdjustmentType,
                                                                AdjustmentAmount = adjustment.AdjustmentValue,
                                                                CalculationBasis = adjustment.CalculationBasis
                                                            };
                        searchReturn.BookingAdjustments.Add(bookingAdjustment);
                    }
                }
            }
            this.SaveResults(searchReturn.ResultToken, searchReturn.BookingAdjustments);
            return searchReturn;
        }

        /// <summary>
        /// Saves the results.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="adjustments">The adjustments.</param>
        private void SaveResults(string token, List<IAdjustment> adjustments)
        {
            HttpContext.Current.Cache.Insert(token, adjustments, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
        }

        /// <summary>
        /// Setups the search model.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns>The search model</returns>
        private IBookingAdjustmentSearchModel SetupSearchModel(IBookingAdjustmentSearchModel searchModel)
        {
            IUserSession userSession = this.userService.GetUser(HttpContext.Current);
            ISite site = this.siteService.GetSite(HttpContext.Current);

            searchModel.BrandId = site.BrandId;

            Brand brand = this.bookingService.GetBrand(site.BrandId);
            if (brand.SellingGeographyLevel1Id != null)
            {
                searchModel.SellingCountryId = brand.SellingGeographyLevel1Id.Value;
            }

            SalesChannel salesChannel = this.bookingService.GetSalesChannel("Web");
            searchModel.SalesChannelId = salesChannel.Id;

            searchModel.BookingDate = DateTime.Now.Date;

            searchModel.CustomerCurrencyId = userSession.SelectCurrency.Id;

            SellingExchangeRate sellingExchangeRate =
                this.bookingService.GetSellingExchangeRate(userSession.SelectCurrency.SellingCurrencyId);
            searchModel.SellingExchangeRate = sellingExchangeRate.Rate;

            if (searchModel.FlightCarrierId > 0)
            {
                var flightCarrier = this.flightService.GetFlightCarrierById(searchModel.FlightCarrierId);
                searchModel.FlightCarrierType = flightCarrier.CarrierType;
            }

            return searchModel;
        }
    }
}

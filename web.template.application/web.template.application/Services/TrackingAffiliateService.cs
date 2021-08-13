namespace Web.Template.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;

    using iVectorConnectInterface;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.Tracking;
    using Web.Template.Application.IVectorConnect.Requests;
    using Web.Template.Application.Support;
    using Web.Template.Application.Tracking;

    using IBookingService = Web.Template.Application.Interfaces.Lookup.Services.IBookingService;

    /// <summary>
    /// This class gets the Tracking Affiliates from iVector Connect
    /// </summary>
    /// <seealso cref="ITrackingAffiliateService" />
    public class TrackingAffiliateService : ITrackingAffiliateService
    {
        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The connect login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory connectLoginDetailsFactory;

        /// <summary>
        /// The booking service
        /// </summary>
        private readonly IBookingService bookingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingAffiliateService" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="connectLoginDetailsFactory">The connect login details factory.</param>
        /// <param name="bookingService">The booking service.</param>
        public TrackingAffiliateService(ISiteService siteService, 
            IIVectorConnectRequestFactory connectRequestFactory, 
            IConnectLoginDetailsFactory connectLoginDetailsFactory,
            IBookingService bookingService)
        {
            this.siteService = siteService;
            this.connectRequestFactory = connectRequestFactory;
            this.connectLoginDetailsFactory = connectLoginDetailsFactory;
            this.bookingService = bookingService;
        }

        /// <summary>
        /// Creates the specified iVectorConnect tracking affiliate.
        /// </summary>
        /// <param name="ivcTrackingAffiliate">The iVectorConnect tracking affiliate.</param>
        /// <returns>
        /// Returns a single Tracking Affiliate
        /// </returns>
        public ITrackingAffiliate Create(GetTrackingAffiliatesResponse.TrackingAffiliate ivcTrackingAffiliate)
        {
            var trackingAffiliate = new TrackingAffiliate()
                                        {
                                            AccomTokenOverride = ivcTrackingAffiliate.AccomTokenOverride, 
                                            BrandId = ivcTrackingAffiliate.BrandID, 
                                            CmsWebsiteId = ivcTrackingAffiliate.CMSWebsiteID, 
                                            ConfirmationScript = ivcTrackingAffiliate.ConfirmationScript, 
                                            FlightAndAccomTokenOverride = ivcTrackingAffiliate.FlightAndAccomTokenOverride, 
                                            FlightTokenOverride = ivcTrackingAffiliate.FlightTokenOverride, 
                                            LandingPageScript = ivcTrackingAffiliate.LandingPageScript, 
                                            Name = ivcTrackingAffiliate.Name, 
                                            Pages = ivcTrackingAffiliate.Pages, 
                                            Position = ivcTrackingAffiliate.Position, 
                                            QueryStringIdentifier = ivcTrackingAffiliate.QueryStringIdentifier, 
                                            Script = ivcTrackingAffiliate.Script, 
                                            SecureScript = ivcTrackingAffiliate.SecureScript, 
                                            TrackingAffiliateId = ivcTrackingAffiliate.TrackingAffiliateID, 
                                            Type = ivcTrackingAffiliate.Type, 
                                            ValidForDays = ivcTrackingAffiliate.TrackingAffiliateID
                                        };
            return trackingAffiliate;
        }

        /// <summary>
        /// Retrieves the affiliates from connect.
        /// </summary>
        /// <returns>
        /// Returns a collection of Tracking Affiliates
        /// </returns>
        public async Task<GetTrackingAffiliatesReturn> RetrieveAffiliatesFromConnect()
        {
            var getTrackingAffiliatesResponse = new GetTrackingAffiliatesResponse();
            var getTrackingAffiliatesRequest = new GetTrackingAffiliatesRequest();
            var getTrackingAffiliatesReturn = new GetTrackingAffiliatesReturn();

            ISite site = this.siteService.GetSite(HttpContext.Current);

            try
            {
                getTrackingAffiliatesRequest.LoginDetails = this.connectLoginDetailsFactory.Create(HttpContext.Current, true);
                getTrackingAffiliatesRequest.BrandID = site.BrandId;
                getTrackingAffiliatesRequest.SalesChannelID = this.bookingService.GetSalesChannel("Web").Id;
                var request = this.connectRequestFactory.Create(getTrackingAffiliatesRequest, HttpContext.Current);
                getTrackingAffiliatesResponse = await request.GoAsync<GetTrackingAffiliatesResponse>();

                if (getTrackingAffiliatesResponse.ReturnStatus.Success)
                {
                    getTrackingAffiliatesReturn.TrackingAffiliates = new List<ITrackingAffiliate>();
                    foreach (GetTrackingAffiliatesResponse.TrackingAffiliate trackingAffiliate in getTrackingAffiliatesResponse.TrackingAffiliates)
                    {
                        getTrackingAffiliatesReturn.TrackingAffiliates.Add(this.Create(trackingAffiliate));
                    }

                    getTrackingAffiliatesReturn.TrackingAffiliateTypeIds = getTrackingAffiliatesResponse.TrackingAffiliateTypeIDs;
                }
                else
                {
                    getTrackingAffiliatesReturn.Ok = false;
                }
            }
            catch (Exception)
            {
                getTrackingAffiliatesReturn.Ok = false;
                getTrackingAffiliatesReturn.Warnings = new List<string>
                                                           {
                                                               "iVectorConnect failed to return tracking affiliates"
                                                           };
            }

            return getTrackingAffiliatesReturn;
        }

        /// <summary>
        /// Setups the tracking affiliates. First checks to see if they are in the cache, if not 
        /// then gets them from iVectorConnect and puts them in the cache
        /// </summary>
        /// <returns>
        /// Returns the tracking affiliates
        /// </returns>
        public async Task<List<ITrackingAffiliate>> SetupTrackingAffiliates()
        {
            List<ITrackingAffiliate> trackingAffiliates = Intuitive.Functions.GetCache<List<ITrackingAffiliate>>("affiliates");
            if (trackingAffiliates == null)
            {
                var affiliateReturn = await this.RetrieveAffiliatesFromConnect();
                trackingAffiliates = affiliateReturn.TrackingAffiliates ?? new List<ITrackingAffiliate>();
                Intuitive.Functions.AddToCache("affiliates", trackingAffiliates, 720);
            }

            return trackingAffiliates;
        }

        /// <summary>
        /// This defines the return type from iVectorConnect
        /// </summary>
        public class GetTrackingAffiliatesReturn
        {
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="GetTrackingAffiliatesReturn"/> is ok.
            /// </summary>
            /// <value>
            ///   <c>true</c> if ok; otherwise, <c>false</c>.
            /// </value>
            public bool Ok { get; set; }

            /// <summary>
            /// Gets or sets the tracking affiliates XML.
            /// </summary>
            /// <value>
            /// The tracking affiliates XML.
            /// </value>
            public List<ITrackingAffiliate> TrackingAffiliates { get; set; }

            /// <summary>
            /// Gets or sets the tracking affiliate type ids.
            /// </summary>
            /// <value>
            /// The tracking affiliate type ids.
            /// </value>
            public string TrackingAffiliateTypeIds { get; set; }

            /// <summary>
            /// Gets or sets the warnings.
            /// </summary>
            /// <value>
            /// The warnings.
            /// </value>
            public List<string> Warnings { get; set; }
        }
    }
}
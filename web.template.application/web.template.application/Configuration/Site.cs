namespace Web.Template.Application.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;
    using System.Xml;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Maps;
    using Web.Template.Application.SiteBuilderService;
    using Web.Template.Application.SocialMedia;
    using Web.Template.Application.Support;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;

    /// <summary>
    /// Main class for configuring site information.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.ISite" />
    public class Site : ISite
    {
        /// <summary>
        /// The brand repository
        /// </summary>
        private readonly IBrandRepository brandRepository;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The site builder request
        /// </summary>
        private readonly ISiteBuilderRequest siteBuilderRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="Site"/> class.
        /// </summary>
        public Site()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Site" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="siteBuilderRequest">The site builder request.</param>
        /// <param name="context">The context.</param>
        public Site(IConfiguration configuration, ISiteBuilderRequest siteBuilderRequest, HttpContext context)
        {
            this.configuration = configuration;
            this.siteBuilderRequest = siteBuilderRequest;

            this.SetSiteFromUrl(context);

            if (!string.IsNullOrEmpty(this.Environment) && !string.IsNullOrEmpty(this.Name))
            {
                this.ConfigureSite();
            }
        }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        /// <value>The brand identifier.</value>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [customer specific page definition].
        /// </summary>
        /// <value><c>true</c> if [customer specific page definition]; otherwise, <c>false</c>.</value>
        public bool CustomerSpecificPageDef { get; set; }

        /// <summary>
        /// Gets or sets the default currency code.
        /// </summary>
        /// <value>The default currency code.</value>
        public string DefaultCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the default language code.
        /// </summary>
        /// <value>The default language code.</value>
        public string DefaultLanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the default page.
        /// </summary>
        /// <value>The default page.</value>
        public string DefaultPage { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect base URL.
        /// </summary>
        /// <value>The ivector connect base URL.</value>
        [JsonIgnore]
        public string IvectorConnectBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect password.
        /// </summary>
        /// <value>The ivector connect password.</value>
        [JsonIgnore]
        public string IvectorConnectPassword { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect username.
        /// </summary>
        /// <value>The ivector connect username.</value>
        [JsonIgnore]
        public string IvectorConnectUsername { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect password.
        /// </summary>
        /// <value>
        /// The ivector connect password.
        /// </value>
        [JsonIgnore]
        public string IvectorConnectContentPassword { get; set; }

        /// <summary>
        /// Gets or sets the ivector connect username.
        /// </summary>
        /// <value>
        /// The ivector connect username.
        /// </value>
        [JsonIgnore]
        public string IvectorConnectContentUsername { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the site configuration.
        /// </summary>
        /// <value>
        /// The site configuration.
        /// </value>
        public ISiteConfiguration SiteConfiguration { get; set; }

        /// <summary>
        /// Maps the concrete configuration to the actual configuration
        /// </summary>
        /// <param name="data">The concrete configuration.</param>
        /// <returns>Site configuration</returns>
        public SiteConfiguration MapConfiguration(dynamic data)
        {
            SiteConfiguration config = new SiteConfiguration()
            {
                OriginUrl = data.OriginUrl,
                BookingJourneyConfiguration = new BookingJourneyConfiguration()
                {
                    SearchModes = new List<ISearchModeConfiguration>(),
                    ChangeFlightPages = new List<string>(),
                    OnRequestDisplay = data.BookingJourneyConfiguration.OnRequestDisplay ?? OnRequestDisplay.None,
                    ThreeDSecureProvider = data.BookingJourneyConfiguration.ThreeDSecureProvider ?? ThreeDSecureProvider.None,
                    DefaultDepositPayment = data.BookingJourneyConfiguration.DefaultDepositPayment ?? false,
                    HideCancellationCharges = data.BookingJourneyConfiguration.HideCancellationCharges ?? false,
                    PaymentMode = data.BookingJourneyConfiguration.PaymentMode ?? PaymentMode.Standard,
                    ValidateChildInfantAges = data.BookingJourneyConfiguration.ValidateChildInfantAges ?? false,
                    SearchToolLocation = data.BookingJourneyConfiguration.SearchToolLocation ?? SearchToolLocation.Hotel,
                    RenderMobileSummary = data.BookingJourneyConfiguration.RenderMobileSummary ?? false,
                    PaymentUrl = data.BookingJourneyConfiguration.PaymentUrl ?? "payment",
                },
                DateConfiguration =
                                                     new DateConfiguration()
                                                     {
                                                         DatePickerFirstDay = data.DateConfiguration.DatePickerFirstDay ?? DatePickerFirstDay.Monday,
                                                         DatePickerDropdowns = data.DateConfiguration.DatePickerDropdowns ?? false,
                                                         DatePickerMonths = data.DateConfiguration.DatePickerMonths ?? 1
                                                     },
                FlightConfiguration = new FlightConfiguration() { ShowFlightExtras = data.FlightConfiguration.ShowFlightExtras ?? false },
                PricingConfiguration =
                                                     new PricingConfiguration()
                                                     {
                                                         PackagePrice = data.PricingConfiguration.PackagePrice ?? false,
                                                         PerPersonPricing = data.PricingConfiguration.PerPersonPricing ?? false,
                                                         PriceFormat = data.PricingConfiguration.PriceFormat ?? PriceFormat.TwoDP,
                                                         PerPersonPriceFormat = data.PricingConfiguration.PerPersonPriceFormat ?? PriceFormat.TwoDP,
                                                         ShowGroupSeparator = data.PricingConfiguration.ShowGroupSeparator ?? true
                                                     },
                SearchConfiguration =
                                                     new SearchConfiguration()
                                                     {
                                                         PackageSearch = data.SearchConfiguration.PackageSearch ?? false,
                                                         PriorityProperty = data.SearchConfiguration.PriorityProperty ?? false,
                                                         SearchBookAheadDays = data.SearchConfiguration.SearchBookAheadDays ?? 2,
                                                         SearchExpiry = data.SearchConfiguration.SearchExpiry ?? 2880,
                                                         SearchModes = new List<SearchMode>(),
                                                         FailedSearchUrl = data.SearchConfiguration.FailedSearchUrl,
                                                         SearchBookingAdjustments = data.SearchConfiguration.SearchBookingAdjustments ?? false,
                                                         UseDealFinder = data.SearchConfiguration.UseDealFinder ?? false
                                                     },
                StarRatingConfiguration = new StarRatingConfiguration()
                {
                    DisplayHalfRatings = data.StarRatingConfiguration.DisplayHalfRatings ?? true,
                    AppendText = new List<StarRatingConfiguration.AppendTextItem>()
                },
                TradeConfiguration =
                                                 new TradeConfiguration()
                                                 {
                                                     DisplayDirectDebits = data.TradeConfiguration.DisplayDirectDebits
                                                 },
                TwitterConfiguration =
                                                     new TwitterConfiguration()
                                                     {
                                                         AccessToken = data.TwitterConfiguration.AccessToken,
                                                         AccessTokenSecret = data.TwitterConfiguration.AccessTokenSecret,
                                                         ConsumerKey = data.TwitterConfiguration.ConsumerKey,
                                                         ConsumerSecret = data.TwitterConfiguration.ConsumerSecret,
                                                         TwitterHandle = data.TwitterConfiguration.TwitterHandle
                                                     },
                MapConfiguration = new GoogleMapsConfiguration() { Key = data.MapConfiguration.Key },
                CmsConfiguration = new CmsConfiguration() { BaseUrl = data.CmsConfiguration.BaseUrl }
            };

            foreach (var searchModeData in data.BookingJourneyConfiguration.SearchModes)
            {
                var searchMode = new SearchModeConfiguration() { SearchMode = searchModeData.Mode, Pages = new List<string>(), UpsellItems = new List<UpsellType>() };

                foreach (var page in searchModeData.Pages)
                {
                    searchMode.Pages.Add(page.ToString());
                }

                foreach (var upsellItem in searchModeData.UpsellItems)
                {
                    searchMode.UpsellItems.Add(Enum.Parse(typeof(UpsellType), upsellItem.ToString()));
                }

                config.BookingJourneyConfiguration.SearchModes.Add(searchMode);
            }

            foreach (var page in data.BookingJourneyConfiguration.ChangeFlightPages)
            {
                config.BookingJourneyConfiguration.ChangeFlightPages.Add(page.ToString());
            }

            foreach (var searchMode in data.SearchConfiguration.SearchModes)
            {
                config.SearchConfiguration.SearchModes.Add(Enum.Parse(typeof(SearchMode), searchMode.ToString()));
            }

            foreach (var item in data.StarRatingConfiguration.AppendText)
            {
                var appendTextItem = new StarRatingConfiguration.AppendTextItem()
                {
                    Rating = (decimal)item.Rating,
                    Text = item.Text
                };
                config.StarRatingConfiguration.AppendText.Add(appendTextItem);
            }

            return config;
        }

        /// <summary>
        /// Sets up the site configuration process
        /// </summary>
        private void ConfigureSite()
        {
            dynamic data = this.GetSiteData();

            this.BrandId = data.BrandId;
            var page = HttpContext.Current.Request.Url.AbsolutePath;
            var isBookingJourney = page.StartsWith("/booking") || page.StartsWith("/tradebookings");

            this.CustomerSpecificPageDef = !isBookingJourney;

            this.DefaultCurrencyCode = data.DefaultCurrency.CurrencyCode;
            this.DefaultLanguageCode = data.DefaultLanguage.CultureCode;
            this.DefaultPage = data.DefaultPage;
            this.IvectorConnectBaseUrl = data.IvectorConnectConfiguration.BaseUrl;
            this.IvectorConnectUsername = data.IvectorConnectConfiguration.Username;
            this.IvectorConnectPassword = data.IvectorConnectConfiguration.Password;
            this.IvectorConnectContentUsername = data.IvectorConnectConfiguration.ContentUsername ?? "";
            this.IvectorConnectContentPassword = data.IvectorConnectConfiguration.ContentPassword ?? "";
            this.SiteConfiguration = this.MapConfiguration(data);
        }

        /// <summary>
        /// Gets the site data.
        /// </summary>
        /// <returns>the site data</returns>
        private dynamic GetSiteData()
        {
            string url = $"{this.configuration.SiteBuilderUrl}/sites/shared/instances/{this.Environment.ToLower()}/entities/sitesettings/en/default-{this.Name.ToLower()}";

            var key = $"sitedata_{this.Environment.ToLower()}_{this.Environment.ToLower()}_{this.Name.ToLower()}";
            var cachekey = Intuitive.AsyncCache.Controller<XmlDocument>.GenerateKey(key);

            dynamic data = Intuitive.AsyncCache.Controller<dynamic>.GetCache(
                cachekey,
                600,
                () =>
                {
                    var modelResponse = this.siteBuilderRequest.Send("GET", url);
                    var modelReturn = JsonConvert.DeserializeObject<ContentDetailsReturn>(modelResponse);

                    dynamic content = JObject.Parse(modelReturn.Content);
                    return content;
                });
            return data;
        }

        /// <summary>
        /// Sets the site from URL.
        /// </summary>
        private void SetSiteFromUrl(HttpContext context)
        {
            var request = context.Request;
            string baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath?.TrimEnd('/')}/";

#if DEBUG
            baseUrl = WebConfigurationManager.AppSettings["SiteURL"];
#endif
            dynamic data = this.GetWebsiteList();

            foreach (var website in data.Websites)
            {
                foreach (string url in website.Urls)
                {
                    if (baseUrl == url)
                    {
                        this.Name = website.SiteName;
                        this.Environment = website.Environment;
                        this.Url = website.Url;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the website list.
        /// </summary>
        /// <returns>the data for the list of websites</returns>
        private dynamic GetWebsiteList()
        {
            dynamic data;
            var key = $"site_websites";
            var cachekey = Intuitive.AsyncCache.Controller<XmlDocument>.GenerateKey(key);

            data = Intuitive.AsyncCache.Controller<dynamic>.GetCache(
                cachekey,
                600,
                () =>
                {
                    var locationBase = HttpRuntime.AppDomainAppPath;
                    var location = $"{locationBase}\\WebsiteUrlSettings.json";
                    StreamReader r = new StreamReader(location);
                    string json = r.ReadToEnd();

                    dynamic content = JObject.Parse(json);
                    return content;
                });
            return data;
        }
    }
}
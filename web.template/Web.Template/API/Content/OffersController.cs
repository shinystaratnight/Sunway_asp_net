namespace Web.Template.API.Content
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Xml;

    using AutoMapper;

    using Intuitive.WebControls;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Services;

    using IConfiguration = Web.Template.Application.Support.IConfiguration;

    /// <summary>
    ///     The API to CMS Content to be called from a front end widget when additional content is needed
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class OffersController : ApiController
    {
        /// <summary>
        /// The page service
        /// </summary>
        private readonly IOffersService offersService;

        /// <summary>
        /// The custom query
        /// </summary>
        private readonly ICustomQuery customQuery;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The document service.
        /// </summary>
        private readonly IDocumentService documentService;

        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userservice;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffersController" /> class.
        /// </summary>
        /// <param name="offersService">The offers service.</param>
        /// <param name="documentService">The document Service.</param>
        /// <param name="userservice">The user service</param>
        /// <param name="customQuery">The custom query.</param>
        /// <param name="configuration">The configuration.</param>
        public OffersController(
            IOffersService offersService,
            IDocumentService documentService,
            IUserService userservice,
            ICustomQuery customQuery,
            IConfiguration configuration)
        {
            this.offersService = offersService;
            this.documentService = documentService;
            this.userservice = userservice;
            this.customQuery = customQuery;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="geographyLevel1">The geography level1 the offers are coming from.</param>
        /// <param name="geographyLevel2">The geography level2 the offers are coming from.</param>
        /// <param name="numberOfOffers">The upper limit of the number of offers returned.</param>
        /// <param name="productAttribute">The name of the product attribute, for example "skis".</param>
        /// <param name="orderBy">The sort by, for example newest.</param>
        /// <param name="propertyReference">The property reference.</param>
        /// <param name="includeLeadInHotels">The include lead in hotels.</param>
        /// <returns>
        /// Returns the offers model of given URL
        /// </returns>
        [Route("api/offers/query")]
        [HttpGet]
        public XmlDocument GetOffersFromQueryString(
            [FromUri] string geographyLevel1 = "0", 
            [FromUri] string geographyLevel2 = "0", 
            [FromUri] string numberOfOffers = "3", 
            [FromUri] string productAttribute = "", 
            [FromUri] string orderBy = "",
            [FromUri] string propertyReference = "0",
            [FromUri] string includeLeadInHotels = "false",
            [FromUri] string highlightedPropertiesOnly = "false")
        {
            var offersModel = new OfferModel();
            offersModel.ParamList.Add(geographyLevel1);
            offersModel.ParamList.Add(geographyLevel2);
            offersModel.ParamList.Add(numberOfOffers);
            offersModel.ParamList.Add(productAttribute);
            offersModel.ParamList.Add(orderBy);
            offersModel.ParamList.Add(propertyReference);
            offersModel.ParamList.Add(includeLeadInHotels);
            offersModel.ParamList.Add(highlightedPropertiesOnly);

            var user = this.userservice.GetUser(HttpContext.Current);
            offersModel.ParamList.Add(user.SelectCurrency.Id.ToString());

            var xml = this.offersService.GetOffersByParams(offersModel.ParamList);

            return xml;
        }

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns the offers model of given URL
        /// </returns>
        [Route("api/offer/{id}")]
        [HttpGet]
        public XmlDocument GetOfferByID(int id)
        {
            var user = this.userservice.GetUser(HttpContext.Current);
            var currencyId = user.SelectCurrency.Id;
            return this.offersService.GetOfferById(id, currencyId);
        }

        /// <summary>
        /// Gets the offer poster by offer id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="airportID">The airport id.</param>
        /// <param name="location">The location of the offer.</param>
        /// <param name="type">the filetype of the poster to be generated.</param>
        /// <returns>
        /// Returns the offer poster for the specified offer id
        /// </returns>
        [Route("api/offer/poster/{id}")]
        public DocumentServiceReturn GetOfferPoster(int id, [FromUri] int airportID, [FromUri] string location, [FromUri] string type)
        {
            XSL xsl = this.GetXsl(id, airportID, location);
            var posterFileName = HttpContext.Current.Server.MapPath("\\Documents\\Posters\\" + $"Poster_{id}_{airportID}." + type);
			var documentUrl = $"{this.configuration.DocumentGeneratorUrl}/sunway/poster";

			DocumentServiceReturn documentReturn = this.documentService.GetDocument(xsl, posterFileName, documentUrl, type);
            documentReturn.DocumentUrl = "\\Documents\\Posters\\" + $"Poster_{id}_{airportID}." + type;

            return documentReturn;
        }

        /// <summary>
        /// Gets the XSL and sets the XSL parameters
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="airportID">The airport id.</param>
        /// <param name="location">The location of the offer.</param>
        /// <returns>
        /// Returns the XSL for the specified offer
        /// </returns>
        public XSL GetXsl(int id, int airportID, string location)
        {
            var user = this.userservice.GetUser(HttpContext.Current);
            var currencyId = user.SelectCurrency.Id;
            var tradeId = user.TradeSession.Trade.Id;
            var queryParams = new List<string>();
            queryParams.Add(id.ToString());
            queryParams.Add(currencyId.ToString());
            queryParams.Add(tradeId.ToString());
            XmlDocument offerxml = this.customQuery.GetCustomQueryXml(queryParams, "Poster");
            var xslTemplate = HttpContext.Current.Server.MapPath("\\Documents\\Poster.xsl");
            var parameters = new Intuitive.WebControls.XSL.XSLParams();

            parameters.AddParam("Locale", user.SelectedCmsWebsite.Name);
            parameters.AddParam("AirportID", airportID);
            parameters.AddParam("CurrencySymbol", user.SelectCurrency.CustomerSymbolOverride);
            parameters.AddParam("CurrencySymbolPosition", user.SelectCurrency.SymbolPosition);

            var xsl = new XSL { XSLTemplate = xslTemplate, XMLDocument = offerxml, XSLParameters = parameters };

            return xsl;
        }
    }
}
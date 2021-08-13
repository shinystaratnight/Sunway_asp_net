namespace Web.Template.Application.Quote.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Configuration;
    using System.Xml;
    using Intuitive;
    using Intuitive.WebControls;
    using Newtonsoft.Json;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Models;
    using Web.Template.Application.Interfaces.Quote.Services;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Models;
    using Web.Template.Application.Quote.Models;
    using Web.Template.Application.Results.ResultModels;
    using Web.Template.Application.Services;

    /// <summary>
    /// Class QuoteService.
    /// </summary>
    public class QuoteService : IQuoteService
    {
        /// <summary>
        /// The basket repository
        /// </summary>
        private readonly IBasketRepository basketRepository;

        /// <summary>
        /// The basket quote create service
        /// </summary>
        private readonly IBasketQuoteCreateService basketQuoteCreateService;

        /// <summary>
        /// The quote result service
        /// </summary>
        private readonly IResultService resultService;

        /// <summary>
        /// The quote content service
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The quote retrieve service
        /// </summary>
        private readonly IQuoteRetrieveService quoteRetrieveService;

        /// <summary>
        /// The quote search service
        /// </summary>
        private readonly IQuoteSearchService quoteSearchService;

        /// <summary>
        /// The document service.
        /// </summary>
        private readonly IDocumentService documentService;

        /// <summary>
        /// The trade service.
        /// </summary>
        private readonly ITradeService tradeService;

        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The document service.
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// The quote flight service
        /// </summary>
        private readonly IFlightService flightService;

        /// <summary>
        /// The quote property service
        /// </summary>
        private readonly IPropertyService propertyService;

        /// <summary>
        /// The quote airport service
        /// </summary>
        private readonly IAirportService airportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteService" /> class.
        /// </summary>
        /// <param name="basketRepository">The basket repository.</param>
        /// <param name="basketQuoteCreateService">The basket quote create service.</param>
        /// <param name="quoteSearchService">The quote search service.</param>
        /// <param name="quoteRetrieveService">The quote retrieve service.</param>
        /// <param name="resultService">The result retrieve service.</param>
        /// <param name="contentService">The content retrieve service.</param>
        /// <param name="userService">The user retrieve service.</param>
        /// <param name="siteService">The site retrieve service.</param>
        /// <param name="tradeService">The trade retrieve service.</param>
        /// <param name="propertyService">The property retrieve service.</param>
        /// <param name="flightService">The flight retrieve service.</param>
        /// <param name="airportService">The airport retrieve service.</param>
        /// <param name="documentService">The document retrieve service.</param>
        public QuoteService(
            IBasketRepository basketRepository,
            IBasketQuoteCreateService basketQuoteCreateService,
            IQuoteSearchService quoteSearchService,
            IQuoteRetrieveService quoteRetrieveService,
            IResultService resultService,
            IContentService contentService,
            IUserService userService,
            ISiteService siteService,
            ITradeService tradeService,
            IPropertyService propertyService,
            IFlightService flightService,
            IAirportService airportService,
            IDocumentService documentService)
        {
            this.basketRepository = basketRepository;
            this.basketQuoteCreateService = basketQuoteCreateService;
            this.quoteSearchService = quoteSearchService;
            this.quoteRetrieveService = quoteRetrieveService;
            this.resultService = resultService;
            this.contentService = contentService;
            this.userService = userService;
            this.siteService = siteService;
            this.tradeService = tradeService;
            this.propertyService = propertyService;
            this.flightService = flightService;
            this.airportService = airportService;
            this.documentService = documentService;
        }

        /// <summary>
        /// Creates the specified basket token.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The quote create return.</returns>
        public IQuoteCreateReturn Create(string basketToken)
        {
            IBasket basket = this.basketRepository.RetrieveBasketByToken(basketToken);
            return this.basketQuoteCreateService.Create(basket);
        }

        /// <summary>
        /// Creates a pdf from the passed quote
        /// </summary>
        /// <param name="quote">the quote model.</param>
        /// <returns>The file name of the created quote</returns>
        public DocumentServiceReturn CreatePDF(QuoteModel quote)
        {
            string siteName = this.siteService.GetSite(HttpContext.Current).Name;
            var xsl = new XSL
            {
                XSLTemplate = HttpContext.Current.Server.MapPath("../../EmailTemplates/" + siteName + "/QuoteEmailPDF.xsl"),
                XMLDocument = this.BuildQuoteXMLDocument(
                    this.BuildQuoteDocumentationModel(quote, (quote.FlightSearchToken != null ? this.BuildQuoteDocumentationFlightResult(quote) : null))),
                XSLParameters = null
            };
            var compactDate = GetCompactDate();
            var quoteFileName = HttpContext.Current.Server.MapPath(string.Concat("\\Documents\\Quotes\\", $"QuotePdf_", compactDate, ".pdf"));
            var documentURL = (new Web.Template.Application.Support.Configuration()).DocumentGeneratorUrl +
                                                                                                   "/" + siteName.Replace("B2C","") + "/quote";
            DocumentServiceReturn documentReturn = this.documentService.GetDocument(xsl, quoteFileName, documentURL);
            documentReturn.DocumentUrl = "/Documents/Quotes/" + $"QuotePdf_" + compactDate + ".pdf";
            return documentReturn;
        }

        /// <summary>
        /// Sends an email from the passed quote
        /// </summary>
        /// <param name="quote">the quote model.</param>
        /// <returns>Whether the email successfully sent</returns>
        public bool Email(QuoteModel quote)
        {
            Email quoteEmail = new Email
            {
                SMTPHost = WebConfigurationManager.AppSettings["SMTPHost"],
                Subject = "Quote Email",
                Body = XMLFunctions.XMLTransformToString(
                                    this.BuildQuoteXMLDocument(this.BuildQuoteDocumentationModel(quote,(quote.FlightSearchToken != null ? this.BuildQuoteDocumentationFlightResult(quote) : null))),
                                    HttpContext.Current.Server.MapPath("../../EmailTemplates/" + this.siteService.GetSite(HttpContext.Current).Name + "/QuoteEmail.xsl"),
                                    null),
                From = quote.FromName,
                FromEmail = quote.FromEmail,
                EmailTo = quote.ToEmail,
                CC = quote.CCEmail
            };
            quoteEmail.SendEmail(true);
            return true;
        }

        /// <summary>
        /// Retrieves the specified quote reference.
        /// </summary>
        /// <param name="quoteReference">The quote reference.</param>
        /// <returns>The Quote Retrieve Return.</returns>
        public Task<QuoteRetrieveReturn> Retrieve(string quoteReference)
        {
            return this.quoteRetrieveService.Retrieve(quoteReference);
        }

        /// <summary>
        /// Searches the specified quote search.
        /// </summary>
        /// <param name="quoteSearch">The quote search.</param>
        /// <returns>The Quote Search Return.</returns>
        public IQuoteSearchReturn Search(IQuoteSearch quoteSearch)
        {
            return this.quoteSearchService.Search(quoteSearch);
        }

        /// <summary>
        /// Builds the xml document for the passed quote
        /// </summary>
        /// <param name="quote">the quote model.</param>
        /// <returns>an xml representation of the quote for use in email/pdf</returns>
        private XmlDocument BuildQuoteXMLDocument(QuoteDocumentationModel quote)
        {
            return XMLFunctions.MergeXMLDocuments(
                Serializer.Serialize(quote),
                this.contentService.GetCmsxmlModel("PropertyFull", quote.PropertyResult.PropertyReferenceId),
                JsonConvert.DeserializeXmlNode(this.contentService.GetContentForContext(this.siteService.GetSite(HttpContext.Current).Name, "footer", "default").ContentJSON.ToString(), "FooterContent"));
        }

        /// <summary>
        /// Builds the QuoteDocumentationModel for a passed quote + flight result
        /// </summary>
        /// <param name="quote">the quote model.</param>
        /// <param name="flightResult">the flight result</param>
        /// <returns>A QuoteDocumentationModel formed of the quote model and the flight result</returns>
        private QuoteDocumentationModel BuildQuoteDocumentationModel(QuoteModel quote, QuoteDocumentationFlightResult flightResult)
        {
            QuoteDocumentationModel quoteDocModel = new QuoteDocumentationModel();
            quoteDocModel.PropertyResult = (PropertyResult)this.resultService.RetrieveResult(quote.PropertySearchToken, quote.PropertyToken);
            quoteDocModel.FlightResult = flightResult;
            quoteDocModel.Trade =
                this.tradeService.GetTradeById(this.userService.GetUser(HttpContext.Current).TradeSession.TradeId);
            quoteDocModel.CmsBaseURL =
                this.siteService.GetSite(HttpContext.Current).SiteConfiguration.CmsConfiguration.BaseUrl;
            quoteDocModel.SellingCurrencySymbol = this.userService.GetUser(HttpContext.Current).SelectCurrency.Symbol;
            foreach (RoomOption option in quote.RoomOptions)
            {
                quoteDocModel.QuoteDocumentationRoomOptions.Add(new QuoteDocumentationRoomOption() { RoomOption = option, MealBasis = this.propertyService.GetMealBasis(option.MealBasisId) });
            }

            return quoteDocModel;
        }

        /// <summary>
        /// Builds the flight results for the passed quote
        /// </summary>
        /// <param name="quote">the quote model.</param>
        /// <returns>An xml representation of the flights on the quote</returns>
        private QuoteDocumentationFlightResult BuildQuoteDocumentationFlightResult(QuoteModel quote)
        {
            FlightResult result = (FlightResult)this.resultService.RetrieveResult(quote.FlightSearchToken, quote.FlightToken);
            QuoteDocumentationFlightResult quoteDocResult = new QuoteDocumentationFlightResult
            {
                FlightResult = result,
                CarrierName = this.flightService.GetFlightCarrierById(result.FlightCarrierId).Name,
                CarrierLogo = this.flightService.GetFlightCarrierById(result.FlightCarrierId).Logo
            };
            foreach (FlightSector fs in result.FlightSectors.Where(x => x.FlightCode == result.OutboundFlightDetails.FlightCode || x.FlightCode == result.ReturnFlightDetails.FlightCode))
            {
                quoteDocResult.QuoteDocumentationSectorAdditionalInformation.Add(
                    new QuoteDocumentationSectorAdditionalInformation()
                    {
                        FlightCode = fs.FlightCode,
                        DepartureAirportName = this.airportService.GetAirportById(fs.DepartureAirportID).Name,
                        DepartureAirportCode = this.airportService.GetAirportById(fs.DepartureAirportID).IATACode,
                        ArrivalAirportName = this.airportService.GetAirportById(fs.ArrivalAirportID).Name,
                        ArrivalAirportCode = this.airportService.GetAirportById(fs.ArrivalAirportID).IATACode,
                        ClassName = this.flightService.GetFlightClassById(fs.Direction == "Outbound"
                            ? result.OutboundFlightDetails.FlightClassId
                            : result.ReturnFlightDetails.FlightClassId).Name,
                    });
            }

            return quoteDocResult;
        }

        /// <summary>
        /// Gets a compacted version of the date to avoid pathing issue
        /// </summary>
        /// <returns>The compacted date</returns>
        private string GetCompactDate()
        {
            DateTime date = DateTime.Now;
            return string.Concat(date.Hour, date.Minute, date.Second, date.Millisecond, date.Day, date.Month, date.Year);
        }
    }
}

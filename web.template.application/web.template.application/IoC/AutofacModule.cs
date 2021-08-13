namespace Web.Template.Application.IoC
{
    using Autofac;
    using Basket.Factories;
    using Basket.Models;
    using Basket.Models.Components;
    using Book;
    using Book.Adaptors;
    using Book.Builders;
    using Book.Factories;
    using Book.Models;
    using Booking;
    using Booking.Adapters;
    using Booking.Factories;
    using Booking.Models;
    using Booking.Services;
    using BookingAdjustment.Factories;
    using Domain.Interfaces.Lookup.Repositories.Geography;
    using Email.Models;
    using Email.Services;
    using iVectorConnectInterface;
    using Interfaces.Basket;
    using Interfaces.Book;
    using Interfaces.Booking.Adapters;
    using Interfaces.Booking.Factories;
    using Interfaces.Booking.Models;
    using Interfaces.Booking.Services;
    using Interfaces.BookingAdjustment;
    using Interfaces.Email.Models;
    using Interfaces.Email.Services;
    using Interfaces.Logging;
    using Interfaces.Models;
    using Interfaces.PageBuilder.Factories;
    using Interfaces.Payment;
    using Interfaces.Prebook;
    using Interfaces.Quote.Adaptors;
    using Interfaces.Quote.Builders;
    using Interfaces.Quote.Models;
    using Interfaces.Quote.Processors;
    using Interfaces.Quote.Services;
    using Interfaces.Repositories;
    using Interfaces.Results;
    using Interfaces.Search;
    using Interfaces.Services;
    using Interfaces.Site;
    using Interfaces.Site.ivcRequests;
    using Interfaces.Trade;
    using Interfaces.User;
    using Intuitive;
    using IVectorConnect.Lookups.Factories;
    using IVectorConnect.Requests;
    using Net.IVectorConnect;
    using Net.Logging;
    using Payment.Factories;
    using Payment.Services;
    using Prebook;
    using Prebook.Adaptor;
    using Prebook.Builders;
    using Prebook.Factories;
    using Prebook.Models;
    using Quote;
    using Quote.Adaptors;
    using Quote.Builders;
    using Quote.Factories;
    using Quote.Models;
    using Quote.Processors;
    using Quote.Services;
    using Repositories;
    using Repositories.Domain.Geography;
    using Results.Adaptors;
    using Results.Factories;
    using Search.Adaptor;
    using Search.Factories;
    using Services;
    using Site;
    using Site.ivcRequests;
    using Support;
    using Trade;
    using Trade.Adaptor;
    using User.Models;
    using Web.Template.Application.Lookup.Services;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using BookingService = Web.Template.Application.Services.BookingService;
    using IBookingService = Web.Template.Application.Interfaces.Services.IBookingService;

    /// <summary>
    ///     An auto face module
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class AutofacModule : Module
    {
        /// <summary>
        ///     Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">
        ///     The builder through which components can be
        ///     registered.
        /// </param>
        /// <remarks>
        ///     Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Basket>().As<IBasket>();
            builder.RegisterType<BasketRepository>().SingleInstance().As<IBasketRepository>();

            RegisterLogging(builder);
            RegisterDomainObjects(builder);
            RegisterAdaptors(builder);
            RegisterFactories(builder);
            RegisterModels(builder);

            RegisterBookingClasses(builder);
            RegisterSiteObjects(builder);

            base.Load(builder);
        }

        /// <summary>
        ///     Registers the adaptors.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterAdaptors(ContainerBuilder builder)
        {
            ////search
            builder.RegisterType<ConnectFlightAdaptor>().As<ConnectFlightAdaptor>();
            builder.RegisterType<ConnectPropertyAdaptor>().As<ConnectPropertyAdaptor>();
            builder.RegisterType<FlightSearchRequestAdaptor>().As<FlightSearchRequestAdaptor>();
            builder.RegisterType<PropertySearchRequestAdapter>().As<PropertySearchRequestAdapter>();
            builder.RegisterType<TransferSearchRequestAdaptor>().As<TransferSearchRequestAdaptor>();
            builder.RegisterType<PackageSearchRequestAdaptor>().As<PackageSearchRequestAdaptor>();
            builder.RegisterType<ConnectExtraResultAdaptor>().As<ConnectExtraResultAdaptor>();

            builder.RegisterType<ConnectPropertyResultAdaptor>().As<ConnectPropertyResultAdaptor>();
            builder.RegisterType<ConnectFlightResultAdaptor>().As<ConnectFlightResultAdaptor>();
            builder.RegisterType<ConnectTransferResultAdaptor>().As<ConnectTransferResultAdaptor>();
            builder.RegisterType<ConnectPackageResultAdaptor>().As<ConnectPackageResultAdaptor>();
            builder.RegisterType<ConnectExtraResultAdaptor>().As<ConnectExtraResultAdaptor>();

            builder.RegisterType<ConnectSearchAdaptor>().As<ISearchAdaptor>();
            builder.RegisterType<ExtraSearchModelAdaptor>().As<IExtraSearchModelAdaptor>();
            builder.RegisterType<ExtraSearchRequestAdaptor>().As<IExtraSearchRequestAdaptor>();

            ////prebook
            builder.RegisterType<TransferPrebookAdaptor>().As<TransferPrebookAdaptor>();
            builder.RegisterType<PropertyPrebookAdaptor>().As<PropertyPrebookAdaptor>();
            builder.RegisterType<FlightPrebookAdaptor>().As<FlightPrebookAdaptor>();
            builder.RegisterType<ExtraPrebookAdaptor>().As<ExtraPrebookAdaptor>();
            builder.RegisterType<PrebookAdaptorFactory>().As<IPrebookRequestAdaptorFactory>();
            builder.RegisterType<ConnectBasketPrebookService>().As<IBasketPrebookService>();
            builder.RegisterType<PrebookResponseProcessor>().As<IPrebookResponseProcessor>();

            ////book
            builder.RegisterType<TransferBookAdaptor>().As<TransferBookAdaptor>();
            builder.RegisterType<PropertyBookAdaptor>().As<PropertyBookAdaptor>();
            builder.RegisterType<FlightBookAdaptor>().As<FlightBookAdaptor>();
            builder.RegisterType<ExtraBookAdaptor>().As<ExtraBookAdaptor>();
            builder.RegisterType<BookAdaptorFactory>().As<IBookRequestAdaptorFactory>();
            builder.RegisterType<ConnectBasketBookService>().As<IBasketBookService>();
            builder.RegisterType<BookResponseProcessor>().As<IBookResponseProcessor>();

            ////quote
            builder.RegisterType<PropertyQuoteAdaptor>().As<PropertyQuoteAdaptor>();
            builder.RegisterType<FlightQuoteAdaptor>().As<FlightQuoteAdaptor>();
            builder.RegisterType<TransferQuoteAdaptor>().As<TransferQuoteAdaptor>();
            builder.RegisterType<QuoteCreateRequestAdaptorFactory>().As<IQuoteCreateRequestAdaptorFactory>();
            builder.RegisterType<ConnectBasketQuoteCreateService>().As<IBasketQuoteCreateService>();
            builder.RegisterType<QuoteCreateResponseProcessor>().As<IQuoteCreateResponseProcessor>();
            builder.RegisterType<ContentService>().As<IContentService>();
            builder.RegisterType<ConnectQuoteSearchService>().As<IQuoteSearchService>();
            builder.RegisterType<ConnectQuoteRetrieveService>().As<IQuoteRetrieveService>();
            builder.RegisterType<QuoteRetrieveSearchAdaptor>().As<IQuoteRetrieveSearchAdaptor>();
            builder.RegisterType<QuoteRetrieveBasketAdaptor>().As<IQuoteRetrieveBasketAdaptor>();
            builder.RegisterType<QuoteRetrieveResponseProcessor>().As<IQuoteRetrieveResponseProcessor>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<SiteService>().As<ISiteService>();
            builder.RegisterType<TradeService>().As<ITradeService>();
            builder.RegisterType<PropertyService>().As<IPropertyService>();
            builder.RegisterType<FlightService>().As<IFlightService>();
            builder.RegisterType<AirportService>().As<IAirportService>();
        }

        /// <summary>
        ///     Registers the booking classes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterBookingClasses(ContainerBuilder builder)
        {
            ////models
            builder.RegisterType<Booking>().As<IBooking>();
            builder.RegisterType<BookingRetrieveReturn>().As<IBookingRetrieveReturn>();
            builder.RegisterType<BookingSearchResult>().As<IBookingSearchResult>();
            builder.RegisterType<BookingSearchReturn>().As<IBookingSearchReturn>();
            builder.RegisterType<SearchBookingsModel>().As<ISearchBookingsModel>();
            builder.RegisterType<BookingDocumentationModel>().As<IBookingDocumentationModel>();
            builder.RegisterType<BookingDocumentationReturn>().As<IBookingDocumentationReturn>();
            builder.RegisterType<DirectDebitRetrieveReturn>().As<IDirectDebitRetrieveReturn>();
            builder.RegisterType<CancellationModel>().As<ICancellationModel>();
            builder.RegisterType<CancellationReturn>().As<ICancellationReturn>();
            builder.RegisterType<ComponentPaymentCancellationReturn>().As<IComponentPaymentCancellationReturn>();

            builder.RegisterType<ComponentCancellationModel>().As<IComponentCancellationModel>();
            builder.RegisterType<ComponentCancellationReturn>().As<IComponentCancellationReturn>();

            ////services
            builder.RegisterType<BookingRetrieveService>().As<IBookingRetrieveService>();
            builder.RegisterType<Lookup.Services.BookingService>().As<Interfaces.Lookup.Services.IBookingService>();
            builder.RegisterType<BookingSearchService>().As<IBookingSearchService>();
            builder.RegisterType<BookingDocumentationService>().As<IBookingDocumentationService>();
            builder.RegisterType<CancellationService>().As<ICancellationService>();
            builder.RegisterType<DirectDebitRetrieveService>().As<IDirectDebitRetrieveService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<ThreeDSecureService>().As<IThreeDSecureService>();
            builder.RegisterType<ExtraService>().As<IExtraService>();

            ////factories
            builder.RegisterType<GetBookingDetailsRequestFactory>().As<IGetBookingDetailsRequestFactory>();
            builder.RegisterType<SearchBookingsRequestFactory>().As<ISearchBookingsRequestFactory>();
            builder.RegisterType<SendDocumentationRequestFactory>().As<ISendDocumentationRequestFactory>();
            builder.RegisterType<ViewDocumentationRequestFactory>().As<IViewDocumentationRequestFactory>();

            builder.RegisterType<CancellationReturnFactory>().As<ICancellationReturnFactory>();
            builder.RegisterType<PreCancelRequestFactory>().As<IPreCancelRequestFactory>();
            builder.RegisterType<CancelRequestFactory>().As<ICancelRequestFactory>();

            builder.RegisterType<ComponentCancellationReturnFactory>().As<IComponentCancellationReturnFactory>();
            builder.RegisterType<PreCancelComponentRequestFactory>().As<IPreCancelComponentRequestFactory>();
            builder.RegisterType<CancelComponentRequestFactory>().As<ICancelComponentRequestFactory>();
            builder.RegisterType<ModuleRequestFactory>().As<IModuleRequestFactory>();

            builder.RegisterType<StoreBasketFactory>().As<IStoreBasketFactory>();
            builder.RegisterType<RetrieveStoredBasketFactory>().As<IRetrieveStoredBasketFactory>();

            builder.RegisterType<ReleaseFlightSeatLockFactory>().As<IReleaseFlightSeatLockFactory>();

            builder.RegisterType<Get3DSecureRedirectRequestFactory>().As<IGet3DSecureRedirectRequestFactory>();
            builder.RegisterType<ThreeDSecureRedirectReturnFactory>().As<IThreeDSecureRedirectReturnFactory>();

            builder.RegisterType<Process3DSecureReturnRequestFactory>().As<IProcess3DSecureReturnRequestFactory>();
            builder.RegisterType<Process3DSecureReturnFactory>().As<IProcess3DSecureReturnFactory>();

            builder.RegisterType<BookingAdjustmentSearchRequestFactory>().As<IBookingAdjustmentSearchRequestFactory>();

            ////adapters
            builder.RegisterType<BookingAdapter>().As<IBookingAdapter>();
            builder.RegisterType<DirectDebitAdapter>().As<IDirectDebitAdapter>();
            builder.RegisterType<BookingSearchResultAdapter>().As<IBookingSearchResultAdapter>();
        }

        /// <summary>
        ///     Registers the domain objects.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterDomainObjects(ContainerBuilder builder)
        {
            builder.RegisterType<GeographyGroupFactory>().As<GeographyGroupFactory>();

            builder.RegisterType<GeographyGroupingRepository>().SingleInstance().As<IGeographyGroupingRepository>();
        }

        /// <summary>
        ///     Registers the factories.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterFactories(ContainerBuilder builder)
        {
            builder.RegisterType<SearchRequestAdaptorFactory>().As<ISearchRequestAdaptorFactory>();

            builder.RegisterType<IVectorConnectRequestFactory>().As<IIVectorConnectRequestFactory>();

            builder.RegisterType<IVConnectResultsAdaptorFactory>().As<IIVConnectResultsAdaptorFactory>();

            builder.RegisterType<TradeSessionFactory>().As<ITradeSessionFactory>();

            builder.RegisterType<TradeLoginRequestFactory>().As<ITradeLoginRequestFactory>();

            builder.RegisterType<PrebookReturnBuilder>().As<IPrebookReturnBuilder>();
            builder.RegisterType<BookReturnBuilder>().As<IBookReturnBuilder>();
            builder.RegisterType<QuoteCreateReturnBuilder>().As<IQuoteCreateReturnBuilder>();
            builder.RegisterType<QuoteRetrieveReturnBuilder>().As<IQuoteRetrieveReturnBuilder>();

            builder.RegisterType<IvConnectResultComponentAdaptorFactory>().As<IIvConnectResultComponentAdaptorFactory>();

            builder.RegisterType<ConnectLoginDetailsFactory>().As<IConnectLoginDetailsFactory>();
        }

        /// <summary>
        ///     Registers the logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterLogging(ContainerBuilder builder)
        {
            builder.RegisterType<WebRequestLogger>().As<IWebRequestLogger>().InstancePerDependency();

            builder.RegisterType<WebRequestLogFormatter>().InstancePerDependency();
            builder.RegisterType<WebExceptionLogFormatter>().InstancePerDependency();
            builder.RegisterType<LogFileWriter>().As<ILogWriter>().InstancePerDependency();
        }

        /// <summary>
        ///     Registers the models.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterModels(ContainerBuilder builder)
        {
            builder.RegisterType<EmailModel>().As<IEmailModel>();
            builder.RegisterType<UserSession>().As<IUserSession>();
            builder.RegisterType<TradeSession>().As<ITradeSession>();
            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.RegisterType<PrebookReturn>().As<IPrebookReturn>();
            builder.RegisterType<BookReturn>().As<IBookReturn>();
            builder.RegisterType<QuoteCreateReturn>().As<IQuoteCreateReturn>();
            builder.RegisterType<QuoteRetrieveReturn>().As<IQuoteRetrieveReturn>();
            builder.RegisterType<Transfer>().As<Transfer>();
        }

        /// <summary>
        ///     Registers the models.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterSiteObjects(ContainerBuilder builder)
        {
            builder.RegisterType<RedirectService>().As<IRedirectService>();
            builder.RegisterType<Redirect>().As<IRedirect>();
            
            //connect requests
            builder.RegisterType<AddUrlRedirectRequestFactory>().As<IAddUrlRedirectRequestFactory>();
            builder.RegisterType<ModifyUrlRedirectRequestFactory>().As<IModifyUrlRedirectRequestFactory>();
            builder.RegisterType<DeleteUrlRedirectRequestFactory>().As<IDeleteUrlRedirectRequestFactory>();
        }
    }
}
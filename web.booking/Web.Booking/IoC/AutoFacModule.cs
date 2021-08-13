namespace Web.Booking.IoC
{
    using System.Collections.Generic;
    using System.Data.Entity;

    using Autofac;

    using AutoMapper;

    using Web.Booking.Factories;
    using Web.Template.Application.Basket.Models;
    using Web.Template.Application.Basket.Services;
    using Web.Template.Application.BookingAdjustment.Services;
    using Web.Template.Application.Configuration;
    using Web.Template.Application.Interfaces.BookingAdjustment;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Interfaces.PageDefinition;
    using Web.Template.Application.Interfaces.Promocode;
    using Web.Template.Application.Interfaces.Repositories;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.Interfaces.SocialMedia;
    using Web.Template.Application.Interfaces.Trade;
    using Web.Template.Application.Lookup.Services;
    using Web.Template.Application.PageBuilder.Factories;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.Quote.Services;
    using Web.Template.Application.Repositories;
    using Web.Template.Application.Search.SearchModels;
    using Web.Template.Application.Services;
    using Web.Template.Application.SiteBuilderService;
    using Web.Template.Application.SocialMedia;
    using Web.Template.Application.Support;
    using Web.Template.Application.Trade.Adaptor;
    using Web.Template.Data.Context;
    using Web.Template.Data.Lookup.Repositories.Booking;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Payment;
    using Web.Template.Data.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;
    using Web.Template.Interfaces;

    using BookingService = Web.Template.Application.Lookup.Services.BookingService;
    using Template.Application.Search.Services;

    using Web.Template.Application.Interfaces.Search;

    /// <summary>
    ///     Registers dependency injection types
    /// </summary>
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
            this.RegisterFactories(builder);
            this.RegisterPages(builder);
            this.RegisterRepositories(builder);
            this.RegisterDbContexts(builder);
            this.RegisterServices(builder);
            this.RegisterModels(builder);
            this.RegisterConfigurationClasses(builder);
            base.Load(builder);
        }

        /// <summary>
        /// Registers the configuration classes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterConfigurationClasses(ContainerBuilder builder)
        {
            builder.RegisterType<SiteConfiguration>().As<ISiteConfiguration>();
            builder.RegisterType<FlightConfiguration>().As<IFlightConfiguration>();
            builder.RegisterType<SearchConfiguration>().As<ISearchConfiguration>();
            builder.RegisterType<DateConfiguration>().As<IDateConfiguration>();
            builder.RegisterType<PricingConfiguration>().As<IPricingConfiguration>();
            builder.RegisterType<EntityPageConfiguration>().As<IEntityPageConfiguration>();

            builder.RegisterAssemblyTypes(typeof(AutofacModule).Assembly).As<Profile>();
            builder.Register(
                c => new MapperConfiguration(
                               configuration =>
                               {
                                   var profiles = c.Resolve<IEnumerable<Profile>>();
                                   var context = c.Resolve<IComponentContext>();
                                   foreach (var profile in profiles)
                                   {
                                       configuration.AddProfile(profile);
                                       configuration.ConstructServicesUsing(t => context.Resolve(t));
                                   }
                               })).AsSelf().SingleInstance();

            builder.Register(
                c =>
                    {
                        var context = c.Resolve<IComponentContext>();
                        var config = context.Resolve<MapperConfiguration>();
                        return config.CreateMapper(t => context.Resolve(t));
                    }).As<IMapper>().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the database contexts.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterDbContexts(ContainerBuilder builder)
        {
            builder.RegisterType<LookupContext>().As<DbContext>();
        }

        /// <summary>
        ///     Registers the factories.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterFactories(ContainerBuilder builder)
        {
            builder.RegisterType<SearchModelFactory>().As<ISearchModelFactory>();
            builder.RegisterType<ContentModelFactory>().As<IContentModelFactory>();
        }

        /// <summary>
        ///     Registers the models.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterModels(ContainerBuilder builder)
        {
            builder.RegisterType<SearchModel>().As<ISearchModel>();
            builder.RegisterType<GuestDetail>().As<IGuest>();
            builder.RegisterType<LeadGuestDetails>().As<ILeadGuest>();
            builder.RegisterType<PaymentDetails>().As<IPaymentDetails>();
        }

        /// <summary>
        ///     Registers the pages.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterPages(ContainerBuilder builder)
        {
            ////builder.RegisterType<PageTemplates.StandardSidebarLeft>().SingleInstance();

            ////builder.RegisterType<PageSection.PageSection>().SingleInstance().As<IPageSection>();
        }

        /// <summary>
        ///     Registers the repositories.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<PageRepository>().As<IPageRepository>();
            builder.RegisterType<EntityPageConfiguration>().As<IEntityPageConfiguration>();

            builder.RegisterType<ConnectFlightCarrierRepository>().As<IFlightCarrierRepository>();
            builder.RegisterType<ConnectAirportRepository>().As<IAirportRepository>();
            builder.RegisterType<ConnectVehicleRepository>().As<IVehicleRepository>();
            builder.RegisterType<BrandRepository>().As<IBrandRepository>();
            builder.RegisterType<ConnectCurrencyRepository>().As<ICurrencyRepository>();
            builder.RegisterType<ConnectLanguageRepository>().As<ILanguageRepository>();
        }

        /// <summary>
        ///     Registers the services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<BasketService>().As<IBasketService>();
            builder.RegisterType<PageService>().As<IPageService>();
            builder.RegisterType<OffersService>().As<IOffersService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
            builder.RegisterType<ContentService>().As<IContentService>();
            builder.RegisterType<CachedResultService>().As<IResultService>();
            builder.RegisterType<AirportService>().As<IAirportService>();
            builder.RegisterType<TradeLookupService>().As<ITradeLookupService>();
            builder.RegisterType<GeographyService>().As<IGeographyService>();
            builder.RegisterType<BrandService>().As<IBrandService>();
            builder.RegisterType<SiteBuilderService>().As<ISiteBuilderService>();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<PaymentService>().As<IPaymentService>();
            builder.RegisterType<BookingService>().As<Template.Application.Interfaces.Lookup.Services.IBookingService>();
            builder.RegisterType<Template.Application.Services.BookingService>().As<Template.Application.Interfaces.Services.IBookingService>();
            builder.RegisterType<PropertyService>().As<IPropertyService>();
            builder.RegisterType<TradeService>().As<ITradeService>();
            builder.RegisterType<SiteService>().As<ISiteService>();
            builder.RegisterType<TrackingAffiliateService>().As<ITrackingAffiliateService>();
            builder.RegisterType<PromoCodeService>().As<IPromoCodeService>();
            builder.RegisterType<FlightService>().As<IFlightService>();
            builder.RegisterType<TwitterService>().As<ITwitterService>();
            builder.RegisterType<FlightCacheRouteService>().As<IFlightCacheRouteService>();
            builder.RegisterType<DealFinderFlightCacheRouteService>().As<IAsyncFlightCacheRouteService>();
            builder.RegisterType<BookingAdjustmentService>().As<IBookingAdjustmentService>();
            builder.RegisterType<QuoteService>().As<IQuoteService>();
            builder.RegisterType<ExtraSearchService>().As<IExtraSearchService>();

            builder.RegisterType<ConnectTradeLoginAdaptor>().As<ITradeLoginAdaptor>();

            builder.RegisterType<Configuration>().As<Configuration>();
            builder.RegisterType<CustomQuery>().As<ICustomQuery>();
            builder.RegisterType<SiteBuilderRequest>().As<ISiteBuilderRequest>();
        }
    }
}
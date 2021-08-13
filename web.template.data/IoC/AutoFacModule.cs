namespace Web.Template.Data.IoC
{
    using Autofac;
    using DealFinder.Services;
    using System.Net.Http;
    using System.Web.Configuration;
    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.CMS.Flight;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Extras;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Flight;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Geography;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Payment;
    using Web.Template.Data.Lookup.Repositories.ConnectLookups.Property;
    using Web.Template.Data.Lookup.Repositories.Flight;
    using Web.Template.Data.Site;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Extras;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Payment;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Property;

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
            this.RegisterRepositories(builder);

            base.Load(builder);
        }

        /// <summary>
        ///     Registers the Contexts.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<AsyncLookup>().As<IAsyncLookup>();

            builder.RegisterType<ConnectFlightCarrierRepository>().As<IFlightCarrierRepository>();
            builder.RegisterType<ConnectAirportRepository>().As<IAirportRepository>();
            builder.RegisterType<ConnectAirportGroupRepository>().As<IAirportGroupRepository>();
            builder.RegisterType<ConnectAirportGroupAirportRepository>().As<IAirportGroupAirportRepository>();
            builder.RegisterType<ConnectAirportGroupResortRepository>().As<IAirportGroupResortRepository>();
            builder.RegisterType<ConnectVehicleRepository>().As<IVehicleRepository>();
            builder.RegisterType<ConnectResortRepository>().As<IResortRepository>();
            builder.RegisterType<ConnectRegionRepository>().As<IRegionRepository>();
            builder.RegisterType<ConnectCountryRepository>().As<ICountryRepository>();
            builder.RegisterType<ConnectRouteAvailabilityRepository>().As<IRouteAvailabilityRepository>();
            builder.RegisterType<ConnectFlightClassRepository>().As<IFlightClassRepository>();
            builder.RegisterType<CMSFlightCacheRouteRepository>().As<IFlightCacheRouteRepository>();

            ////Booking Repos
            builder.RegisterType<ConnectBookingCountryRepository>().As<IBookingCountryRepository>();
            builder.RegisterType<ConnectBookingDocumentationRepository>().As<IBookingDocumentationRepository>();
            builder.RegisterType<ConnectTradeContactRepository>().As<ITradeContactRepository>();
            builder.RegisterType<ConnectTradeRepository>().As<ITradeRepository>();
            builder.RegisterType<ConnectTradeGroupRepository>().As<ITradeGroupRepository>();
            builder.RegisterType<ConnectTradeParentGroupRepository>().As<ITradeParentGroupRepository>();
            builder.RegisterType<ConnectTradeContactGroupRepository>().As<ITradeContactGroupRepository>();
            builder.RegisterType<ConnectBrandRepository>().As<IBrandRepository>();
            builder.RegisterType<ConnectLanguageRepository>().As<ILanguageRepository>();
            builder.RegisterType<ConnectNationalityRepository>().As<INationalityRepository>();
            builder.RegisterType<ConnectSalesChannelRepository>().As<ISalesChannelRepository>();
            builder.RegisterType<ConnectBookingComponentRepository>().As<IBookingComponentRepository>();
            builder.RegisterType<ConnectBrandGeographyRepository>().As<IBrandGeographyRepository>();
            builder.RegisterType<ConnectSellingExchangeRateRepository>().As<ISellingExchangeRateRepository>();
            builder.RegisterType<ConnectURL301RedirectRepository>().As<IURL301RedirectRepository>();

            ////Payment Repos
            builder.RegisterType<ConnectCreditCardSurchargeRepository>().As<ICreditCardSurchargeRepository>();
            builder.RegisterType<ConnectCreditCardTypeRepository>().As<ICreditCardTypeRepository>();
            builder.RegisterType<ConnectCurrencyRepository>().As<ICurrencyRepository>();
            builder.RegisterType<ConnectExchangeRateRepository>().As<IExchangeRateRepository>();

            ////Property Repos
            builder.RegisterType<ConnectFilterFacilityRepository>().As<IFilterFacilityRepository>();
            builder.RegisterType<ConnectMealBasisRepository>().As<IMealBasisRepository>();
            builder.RegisterType<ConnectProductAttributeRepository>().As<IProductAttributeRepository>();
            builder.RegisterType<ConnectPropertyReferenceRepository>().As<IPropertyReferenceRepository>();

            ////Site Repos
            builder.RegisterType<WebsiteRepository>().As<IWebsiteRepository>();

            ////Extra repos
            builder.RegisterType<ConnectExtraTypeRepository>().As<IExtraTypeRepository>();

            builder.Register<HttpClient>(c => new HttpClient()).SingleInstance();

            ////Deal finder services
            builder.Register(context => new FlightCacheSearchService(
                context.Resolve<HttpClient>(), 
                WebConfigurationManager.AppSettings["DealFinderURL"]))
                .As<IFlightCacheSearchService>();
        }
    }
}
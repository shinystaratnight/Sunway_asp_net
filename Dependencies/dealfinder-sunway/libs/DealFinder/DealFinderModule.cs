namespace DealFinder
{
    using Microsoft.Extensions.DependencyInjection;

    using Intuitive.DependencyInjection;
    using Intuitive.Modules;
    using Intuitive.Data;
    using Intuitive;
    using DealFinder.Services;
    using DealFinder.Request;
    using DealFinder.Response;
    using DealFinder.Handlers;
    using DealFinder.Validation;

    /// <summary>
    /// DealFinder module.
    /// </summary>
    public class DealFinderModule : ModuleBase, IServicesBuilder
    {
        /// <summary>
        /// Initialises a new instance of <see cref="DealFinderModule"/>.
        /// </summary>
        public DealFinderModule()
            : base(DealFinderInfo.DealFinderModuleId, DealFinderInfo.DealFinderModuleName, DealFinderInfo.DealFinderModuleDescription,
                  new[] { DataInfo.DataModuleId })
        { }

        /// <inheritdoc />
        public void BuildServices(ServicesBuilderContext context, IServiceCollection services)
        {
            Ensure.IsNotNull(services, nameof(services));

            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ILookupsService, LookupsService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IFlightCacheSearchService, FlightCacheSearchService>();
            services.AddScoped<IPricingService, PricingService>();

            services.AddHandlerAndValidator<
                CalendarRequest,
                CalendarResponse,
                CalendarRequestHandler,
                CalendarRequestValidator>();

            services.AddHandlerAndValidator<
                LookupsRequest,
                LookupsResponse,
                LookupsRequestHandler,
                LookupsRequestValidator>();

            services.AddHandlerAndValidator<
                SearchRequest,
                SearchResponse,
                SearchRequestHandler, 
                SearchRequestValidator>();

            services.AddHandlerAndValidator<
                FlightCacheSearchRequest,
                FlightCacheSearchResponse,
                FlightCacheSearchHandler,
                FlightCacheSearchRequestValidator>();
        }
    }
}

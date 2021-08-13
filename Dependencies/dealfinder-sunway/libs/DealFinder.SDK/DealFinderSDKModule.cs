namespace DealFinder.SDK
{
    using DealFinder.Services;
    using Intuitive;
    using Intuitive.DependencyInjection;
    using Intuitive.Modules;
    using Microsoft.Extensions.DependencyInjection;

    public class DealFinderSDKModule : ModuleBase, IServicesBuilder
    {
        public DealFinderSDKModule() 
            : base(DealFinderInfo.DealFinderModuleId, DealFinderInfo.DealFinderModuleName, DealFinderInfo.DealFinderModuleDescription)
        {
        }

        public void BuildServices(ServicesBuilderContext context, IServiceCollection services)
        {
            Ensure.IsNotNull(services, nameof(services));

            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ILookupsService, LookupsService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IFlightCacheSearchService, FlightCacheSearchService>();
        }
    }
}

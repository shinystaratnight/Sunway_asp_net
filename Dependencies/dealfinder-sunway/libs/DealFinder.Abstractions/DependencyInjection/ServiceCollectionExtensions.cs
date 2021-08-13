// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Intuitive.Modules;

    /// <summary>
    /// Provides extensions for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the set of module services to the collection.
        /// </summary>
        /// <param name="services">The set of services.</param>
        /// <param name="context">The services builder context.</param>
        /// <returns>The set of services.</returns>
        public static IServiceCollection AddModuleServices(this IServiceCollection services, ServicesBuilderContext context)
        {
            Ensure.IsNotNull(services, nameof(services));
            Ensure.IsNotNull(context, nameof(context));

            services.AddSingleton(context.ModuleProvider);

            foreach (var module in context.ModuleProvider.Modules)
            {
                if (module is IServicesBuilder builder)
                {
                    builder.BuildServices(context, services);
                }
            }

            return services;
        }
    }
}
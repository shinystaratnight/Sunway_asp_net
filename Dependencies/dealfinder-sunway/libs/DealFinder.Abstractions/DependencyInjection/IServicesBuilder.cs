// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Defines the required contract for implementing a services builder.
    /// </summary>
    public interface IServicesBuilder
    {
        /// <summary>
        /// Builds the services for the application.
        /// </summary>
        /// <param name="context">The services builder context.</param>
        /// <param name="services">The set of services.</param>
        void BuildServices(ServicesBuilderContext context, IServiceCollection services);
    }
}
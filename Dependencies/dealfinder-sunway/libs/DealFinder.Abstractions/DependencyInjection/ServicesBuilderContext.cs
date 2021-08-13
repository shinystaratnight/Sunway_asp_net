// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using Intuitive.Modules;

    /// <summary>
    /// Represents a context for building services
    /// </summary>
    public class ServicesBuilderContext
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ServicesBuilderContext"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="moduleProvider">The module provider.</param>
        public ServicesBuilderContext(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            IModuleProvider moduleProvider)
        {
            Configuration = Ensure.IsNotNull(configuration, nameof(configuration));
            HostingEnvironment = Ensure.IsNotNull(hostingEnvironment, nameof(hostingEnvironment));
            ModuleProvider = Ensure.IsNotNull(moduleProvider, nameof(moduleProvider));
        }

        /// <summary>
        /// Gets the configration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the module provider
        /// </summary>
        public IModuleProvider ModuleProvider { get; }
    }
}

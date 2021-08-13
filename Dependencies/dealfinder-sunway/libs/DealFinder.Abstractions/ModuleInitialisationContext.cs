// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Modules
{
    using System;

    /// <summary>
    /// Provides a context for initialising a module.
    /// </summary>
    public class ModuleInitialisationContext
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ModuleInitialisationContext"/>.
        /// </summary>
        /// <param name="applicationServices">The application service provider.</param>
        public ModuleInitialisationContext(IServiceProvider applicationServices)
        {
            ApplicationServices = Ensure.IsNotNull(applicationServices, nameof(applicationServices));
        }

        /// <summary>
        /// Gets the application service provider.
        /// </summary>
        public IServiceProvider ApplicationServices { get; }
    }
}
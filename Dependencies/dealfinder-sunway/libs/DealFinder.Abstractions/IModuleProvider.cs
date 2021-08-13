// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the required contract for implementing a module provider.
    /// </summary>
    public interface IModuleProvider
    {
        /// <summary>
        /// Gets the set of modules.
        /// </summary>
        IReadOnlyCollection<IModule> Modules { get; }
    }
}
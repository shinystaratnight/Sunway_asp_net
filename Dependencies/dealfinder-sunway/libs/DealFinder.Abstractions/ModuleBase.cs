// Copyight © intuitive Ltd. All rights reserved
#nullable enable
namespace Intuitive.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Provides a base implementation of a module.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ModuleBase"/>.
        /// </summary>
        /// <param name="id">The module id.</param>
        /// <param name="name">[Optional] The module name.</param>
        /// <param name="description">[Optional] The module description.</param>
        /// <param name="dependencies">[Optional] The set of dependencies for this module.</param>
        protected ModuleBase(
            ModuleId id, 
            string? name = null, 
            string? description = null, 
            IEnumerable<ModuleId>? dependencies = null)
        {
            if (id.Equals(ModuleId.Empty))
            {
                throw new ArgumentException(CoreAbstractionsResources.ModuleIdRequiredExceptionMessage, nameof(id));
            }

            Id = id;
            Name = name;
            Description = description;
            Dependencies = new ReadOnlyCollection<ModuleId>((dependencies ?? Enumerable.Empty<ModuleId>()).ToList());
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ModuleId> Dependencies { get; }

        /// <inheritdoc />
        public ModuleId Id { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public string? Name { get; }

        /// <inheritdoc />
        public virtual void Initialise(ModuleInitialisationContext context) { }
    }
}
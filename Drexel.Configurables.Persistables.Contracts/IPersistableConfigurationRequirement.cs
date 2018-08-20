using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a uniquely-identifiable <see cref="IConfigurationRequirement"/> that is safe to persist.
    /// </summary>
    public interface IPersistableConfigurationRequirement : IConfigurationRequirement
    {
        /// <summary>
        /// Gets the unique ID of this <see cref="IPersistableConfigurationRequirement"/>. This ID is invariant, and
        /// permanently identifies this resource.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the version of this <see cref="IPersistableConfigurationRequirement"/>.
        /// </summary>
        Version Version { get; }
    }
}

using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a source from which <see cref="IPersistableConfigurationRequirement"/>s are derived.
    /// </summary>
    public interface IPersistableConfigurationRequirementSource : IRequirementSource
    {
        /// <summary>
        /// Gets the set of <see cref="IPersistableConfigurationRequirement"/>s required by this
        /// <see cref="IPersistableConfigurationRequirementSource"/>.
        /// </summary>
        new IReadOnlyList<IPersistableConfigurationRequirement> Requirements { get; }
    }
}

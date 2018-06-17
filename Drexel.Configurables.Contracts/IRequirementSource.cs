using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a source from which requirements are derived.
    /// </summary>
    public interface IRequirementSource
    {
        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s required by this <see cref="IRequirementSource"/>.
        /// </summary>
        IReadOnlyList<IConfigurationRequirement> Requirements { get; }
    }
}

using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a uniquely-identifiable <see cref="IConfiguration"/> that is safe to persist.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "Unnecessary.")]
    public interface IPersistableConfiguration :
        IConfiguration,
        IEnumerable<IMapping<IPersistableConfigurationRequirement>>
    {
        /// <summary>
        /// Gets the set of <see cref="IPersistableConfigurationRequirement"/>s contained by this
        /// <see cref="IPersistableConfiguration"/>.
        /// </summary>
        new IReadOnlyList<IPersistableConfigurationRequirement> Keys { get; }
    }
}

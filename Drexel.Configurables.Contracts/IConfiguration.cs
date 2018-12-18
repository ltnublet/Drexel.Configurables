using System.Collections.Generic;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents a validated set of mappings between <see cref="IRequirement"/>s and <see cref="object"/>s.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1710:Identifiers should have correct suffix",
        Justification = "Unnecessary.")]
    public interface IConfiguration : IReadOnlyDictionary<IRequirement, object>
    {
    }
}

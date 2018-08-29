using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a uniquely-identifiable <see cref="IConfiguration"/> that is safe to persist.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Has additional functionality.")]
    public interface IPersistableConfiguration : IConfiguration, IPersistable
    {
    }
}

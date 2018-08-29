using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    /// <summary>
    /// Represents a uniquely-identifiable <see cref="IConfigurationRequirement"/> that is safe to persist.
    /// </summary>
    public interface IPersistableConfigurationRequirement : IConfigurationRequirement, IPersistable
    {
    }
}

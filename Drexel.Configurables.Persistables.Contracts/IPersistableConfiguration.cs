using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    public interface IPersistableConfiguration : IConfiguration, IEnumerable<IMapping<IPersistableConfigurationRequirement>>
    {
        new IReadOnlyList<IPersistableConfigurationRequirement> Keys { get; }
    }
}

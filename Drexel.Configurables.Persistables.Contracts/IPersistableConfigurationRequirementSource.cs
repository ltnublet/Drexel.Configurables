using System.Collections.Generic;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    public interface IPersistableConfigurationRequirementSource : IRequirementSource
    {
        new IReadOnlyList<IPersistableConfigurationRequirement> Requirements { get; }
    }
}

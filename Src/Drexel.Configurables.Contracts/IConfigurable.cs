using System;

namespace Drexel.Configurables.Contracts
{
    public interface IConfigurable
    {
        Guid Id { get; }

        RequirementSet Requirements { get; }
    }
}

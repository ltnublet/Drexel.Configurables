using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    public interface IConfigurationPersister
    {
        Task PersistAsync(
            IConfiguration configuration,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IConfiguration> RestoreAsync(
            IReadOnlyCollection<IRequirement> requirementSet,
            Func<IReadOnlyCollection<IRequirement>, IReadOnlyDictionary<IRequirement, object>> unpersistableRequirementsCallback,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}

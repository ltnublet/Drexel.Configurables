using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Contracts
{
    public interface IConfigurationPersister
    {
        Task PersistAsync(
            IConfiguration configuration,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}

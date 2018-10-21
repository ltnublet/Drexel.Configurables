using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Persistables.Contracts
{
    public interface IPersister
    {
        Task PersistAsync(
            IPersistableConfiguration configuration,
            CancellationToken token);
    }
}

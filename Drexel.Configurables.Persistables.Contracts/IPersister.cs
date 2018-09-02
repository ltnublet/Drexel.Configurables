using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Persistables.Contracts
{
    public interface IPersister
    {
        Task Persist(
            IPersistableConfiguration configuration,
            CancellationToken token);
    }
}

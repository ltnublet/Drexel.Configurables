using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Persistables.Contracts
{
    public interface IRestorer
    {
        Task<IPersistableConfiguration> Restore(CancellationToken token);
    }
}

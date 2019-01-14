using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Serialization
{
    public interface IStructTypeSerializer<T, TIntermediary> : ITypeSerializer<TIntermediary>
        where T : struct
    {
        Task SerializeAsync(
            TIntermediary intermediary,
            T value,
            CancellationToken cancellationToken = default);

        Task SerializeAsync(
            TIntermediary intermediary,
            IEnumerable<T>? values,
            CancellationToken cancellationToken = default);
    }
}

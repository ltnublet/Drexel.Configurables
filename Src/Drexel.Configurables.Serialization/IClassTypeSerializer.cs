using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Serialization
{
    public interface IClassTypeSerializer<T, TIntermediary> : ITypeSerializer<TIntermediary>
        where T : class
    {
        Task SerializeAsync(
            TIntermediary intermediary,
            T? value,
            CancellationToken cancellationToken = default);

        Task SerializeAsync(
            TIntermediary intermediary,
            IEnumerable<T?>? values,
            CancellationToken cancellationToken = default);
    }
}

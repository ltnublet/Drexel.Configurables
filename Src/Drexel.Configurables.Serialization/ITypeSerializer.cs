using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Configurables.Serialization
{
    public interface ITypeSerializer<TIntermediary>
    {
        bool SupportsWrites { get; }

        Task SerializeAsync(
            TIntermediary intermediary,
            object? value,
            CancellationToken cancellationToken = default);

        Task SerializeAsync(
            TIntermediary intermediary,
            IEnumerable? values,
            CancellationToken cancellationToken = default);
    }
}

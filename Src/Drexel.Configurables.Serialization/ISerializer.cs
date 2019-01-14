using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Serialization
{
    public interface ISerializer
    {
        Task SerializeAsync(
            Configuration configuration,
            CancellationToken cancellationToken = default);
    }

    public interface ISerializer<TIntermediary> : ISerializer
    {
        TypeSerializerDictionary<TIntermediary> Supported { get; }
    }
}

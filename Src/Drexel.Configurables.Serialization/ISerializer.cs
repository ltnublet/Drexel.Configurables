using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Serialization
{
    public interface ISerializer
    {
        TypeSerializerDictionary Supported { get; }

        Task Serialize(
            Configuration configuration,
            CancellationToken cancellationToken = default);
    }
}

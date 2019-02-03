using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Configurations;

namespace Drexel.Configurables.Serialization
{
    public interface IDeserializer : IDisposable
    {
        Task<Configuration> DeserializeAsync(CancellationToken cancellationToken = default);
    }
}

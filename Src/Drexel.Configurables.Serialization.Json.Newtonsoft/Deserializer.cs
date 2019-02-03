using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Configurations;
using Drexel.Configurables.Contracts.Ids;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft
{
    public class Deserializer : IDeserializer
    {
        public Deserializer(Stream stream, RequirementIdMap idMap)
        {
            throw new NotImplementedException();
        }

        public Task<Configuration> DeserializeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Configurations;
using Drexel.Configurables.Contracts.Ids;
using Drexel.Configurables.Serialization;

namespace Drexel.Configurables.Json.Newtonsoft
{
    public class Serializer : ISerializer
    {
        public Serializer(
            Stream stream,
            RequirementIdMap idMap,
            int bufferSize = 1024,
            bool leaveOpen = true)
        {
            throw new NotImplementedException();
        }

        public Task SerializeAsync(
            Configuration configuration,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables.Json
{
    public class JsonPersister : IPersister
    {
        public JsonPersister(Stream stream)
        {

        }

        public Task<PersistResult> Persist(IPersistableConfiguration configuration, CancellationToken token)
        {
            // TODO: implement
            throw new NotImplementedException();
        }
    }
}

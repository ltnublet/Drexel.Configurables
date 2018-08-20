using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables.Json
{
    public class JsonPersister : IPersister
    {
        public Task<PersistResult> Persist(IPersistableConfiguration configuration, CancellationToken token)
        {
            // TODO: implement
            throw new NotImplementedException();
        }
    }
}

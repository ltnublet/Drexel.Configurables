using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Persistables.Contracts;

namespace Drexel.Configurables.Persistables.Json
{
    public class JsonRestorer : IRestorer
    {
        public Task<IPersistableConfiguration> RestoreAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}

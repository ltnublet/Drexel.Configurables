using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.JSON
{
    public class JsonConfigurationPersister : IConfigurationPersister
    {
        public Task PersistAsync(
            IConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

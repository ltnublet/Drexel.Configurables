using System;
using System.Collections.Generic;
using System.Text;

namespace Drexel.Configurables.Contracts
{
    public interface IConfiguration : IReadOnlyDictionary<IRequirement, object>
    {
    }
}

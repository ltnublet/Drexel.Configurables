using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    public sealed class PersistableMapping : IMapping<IPersistableConfigurationRequirement>
    {
        public PersistableMapping(IPersistableConfigurationRequirement requirement, object value)
        {
            this.Key = requirement ?? throw new ArgumentNullException(nameof(requirement));
            this.Value = value;
        }

        public IPersistableConfigurationRequirement Key { get; }

        public object Value { get; }
    }
}

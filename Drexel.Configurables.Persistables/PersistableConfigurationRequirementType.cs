using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables
{
    public class PersistableConfigurationRequirementType : IConfigurationRequirementType
    {
        private readonly IConfigurationRequirementType type;
        private readonly Func<string, object> restoreFunc;

        public PersistableConfigurationRequirementType(
            IConfigurationRequirementType type,
            Func<string, object> restoreFunc)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.restoreFunc = restoreFunc ?? throw new ArgumentNullException(nameof(restoreFunc));
        }

        public Type Type => this.type.Type;

        public object Restore(string value) => this.restoreFunc.Invoke(value);
    }
}

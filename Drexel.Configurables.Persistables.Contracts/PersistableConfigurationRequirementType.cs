using System;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables.Persistables.Contracts
{
    public class PersistableConfigurationRequirementType : IConfigurationRequirementType
    {
        private readonly IConfigurationRequirementType type;
        private readonly Func<object, string> encodeFunc;
        private readonly Func<string, object> restoreFunc;

        public PersistableConfigurationRequirementType(
            IConfigurationRequirementType type,
            Version version,
            Func<object, string> encodeFunc,
            Func<string, object> restoreFunc)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.encodeFunc = encodeFunc ?? throw new ArgumentNullException(nameof(encodeFunc));
            this.restoreFunc = restoreFunc ?? throw new ArgumentNullException(nameof(restoreFunc));
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Type Type => this.type.Type;

        public Version Version { get; }

        public string Encode(object value) => this.encodeFunc.Invoke(value);

        public object Restore(string value) => this.restoreFunc.Invoke(value);
    }
}

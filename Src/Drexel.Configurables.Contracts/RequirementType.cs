using System;

namespace Drexel.Configurables.Contracts
{
    public abstract class RequirementType
    {
        private protected RequirementType(Type type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type Type { get; }
    }
}

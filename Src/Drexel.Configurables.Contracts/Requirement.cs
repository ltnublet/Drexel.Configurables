using System;

namespace Drexel.Configurables.Contracts
{
    public abstract class Requirement
    {
        private protected Requirement(RequirementType type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public RequirementType Type { get; }
    }
}

using System;

namespace Drexel.Configurables.Contracts.Structs
{
    public abstract class StructRequirement<T> : Requirement
        where T : struct
    {
        private protected StructRequirement(StructRequirementType<T> type)
            : base(type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public new StructRequirementType<T> Type { get; }
    }
}

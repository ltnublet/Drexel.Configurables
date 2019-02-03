using System;

namespace Drexel.Configurables.Contracts.Classes
{
    public abstract class ClassRequirement<T> : Requirement
        where T : class
    {
        private protected ClassRequirement(ClassRequirementType<T> type)
            : base(type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public new ClassRequirementType<T> Type { get; }
    }
}

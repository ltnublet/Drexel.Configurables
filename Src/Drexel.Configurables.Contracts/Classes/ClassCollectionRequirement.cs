using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Classes
{
    public sealed class ClassCollectionRequirement<T> : ClassRequirement<IEnumerable<T>>
        where T : class
    {
        public ClassCollectionRequirement(ClassRequirementType<IEnumerable<T>> type)
            : base(type)
        {
        }
    }
}

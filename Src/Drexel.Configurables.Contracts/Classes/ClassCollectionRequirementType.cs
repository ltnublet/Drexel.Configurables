using System.Collections.Generic;

namespace Drexel.Configurables.Contracts.Classes
{
    public sealed class ClassCollectionRequirementType<T> : ClassRequirementType<IEnumerable<T>>
        where T : class
    {
        public ClassCollectionRequirementType()
            : base()
        {
        }
    }
}

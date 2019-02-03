using System.Collections.Generic;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Contracts.Structs
{
    public sealed class StructCollectionRequirement<T> : ClassRequirement<IEnumerable<T>>
        where T : struct
    {
        public StructCollectionRequirement(StructCollectionRequirementType<T> type)
            : base(type)
        {
        }
    }
}

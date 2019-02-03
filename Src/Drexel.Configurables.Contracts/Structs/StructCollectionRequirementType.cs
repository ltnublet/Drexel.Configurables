using System.Collections.Generic;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Contracts.Structs
{
    public sealed class StructCollectionRequirementType<T> : ClassRequirementType<IEnumerable<T>>
        where T : struct
    {
        public StructCollectionRequirementType()
            : base()
        {
        }
    }
}

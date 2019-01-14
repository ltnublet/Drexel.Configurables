using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class TimeSpanRequirementType
    {
        private static readonly string Id = @"D3B2226E-BE13-4240-948B-A09EC732B598";

        public static StructRequirementType<TimeSpan> Instance { get; } =
            new StructRequirementType<TimeSpan>(
                Guid.Parse(TimeSpanRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

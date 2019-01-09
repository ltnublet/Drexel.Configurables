using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class SingleRequirementType
    {
        private static readonly string Id = @"2DBCA6DB-D536-4466-B3ED-F45F772FAC3C";

        public static StructRequirementType<float> Instance { get; } =
            new StructRequirementType<float>(
                Guid.Parse(SingleRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

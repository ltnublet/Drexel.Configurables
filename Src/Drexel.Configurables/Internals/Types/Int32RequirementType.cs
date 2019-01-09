using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class Int32RequirementType
    {
        private static readonly string Id = @"07F04E56-584C-4062-91B5-28482167DFDF";

        public static StructRequirementType<int> Instance { get; } =
            new StructRequirementType<int>(
                Guid.Parse(Int32RequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

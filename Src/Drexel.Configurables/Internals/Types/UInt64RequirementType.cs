using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class UInt64RequirementType
    {
        private static readonly string Id = @"F07DB95A-00D5-40E2-BC3F-3FF367631E3D";

        public static StructRequirementType<ulong> Instance { get; } =
            new StructRequirementType<ulong>(
                Guid.Parse(UInt64RequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

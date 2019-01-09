using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class UInt16RequirementType
    {
        private static readonly string Id = @"381F8093-E64A-4042-ACB3-AC18063F138D";

        public static StructRequirementType<ushort> Instance { get; } =
            new StructRequirementType<ushort>(
                Guid.Parse(UInt16RequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

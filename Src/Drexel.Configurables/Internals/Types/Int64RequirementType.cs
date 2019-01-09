using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class Int64RequirementType
    {
        private static readonly string Id = @"5F64E460-CEF7-44B7-8CAD-6ECF33E4FBB3";

        public static StructRequirementType<long> Instance { get; } =
            new StructRequirementType<long>(
                Guid.Parse(Int64RequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

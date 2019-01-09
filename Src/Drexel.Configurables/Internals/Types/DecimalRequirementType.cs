using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class DecimalRequirementType
    {
        private static readonly string Id = @"06249E41-DF5C-4E64-8EEB-F876C61D7B94";

        public static StructRequirementType<decimal> Instance { get; } =
            new StructRequirementType<decimal>(
                Guid.Parse(DecimalRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

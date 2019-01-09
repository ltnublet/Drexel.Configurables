using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class DoubleRequirementType
    {
        private static readonly string Id = @"9A376B9A-BFE0-4462-9CC0-830CC4743DAC";

        public static StructRequirementType<double> Instance { get; } =
            new StructRequirementType<double>(
                Guid.Parse(DoubleRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

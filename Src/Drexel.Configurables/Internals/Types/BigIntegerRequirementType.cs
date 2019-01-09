using System;
using System.Numerics;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class BigIntegerRequirementType
    {
        private static readonly string Id = @"A97BDE73-8393-4B35-BEDC-ECACF21D7E5A";

        public static StructRequirementType<BigInteger> Instance { get; } =
            new StructRequirementType<BigInteger>(
                Guid.Parse(BigIntegerRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

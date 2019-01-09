using System;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Internals.Types
{
    internal static class StringRequirementType
    {
        private static readonly string Id = @"499F429D-4B57-4D04-9C11-377BF9F49D99";

        public static ClassRequirementType<string> Instance { get; } =
            new ClassRequirementType<string>(
                Guid.Parse(StringRequirementType.Id),
                DefaultMethods.TryCastClassValue,
                DefaultMethods.TryCastClassCollection);
    }
}

using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class BooleanRequirementType
    {
        private static readonly string Id = @"6565954A-8999-49C1-81B8-DC866BBF43C9";

        public static StructRequirementType<bool> Instance { get; } =
            new StructRequirementType<bool>(
                Guid.Parse(BooleanRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

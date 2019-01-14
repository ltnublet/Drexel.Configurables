using System;
using Drexel.Configurables.Contracts.Structs;

namespace Drexel.Configurables.Internals.Types
{
    internal static class DateTimeRequirementType
    {
        private static readonly string Id = @"F66F3FBF-FCD2-41FD-86B7-B3FAC37F6F5F";

        public static StructRequirementType<DateTime> Instance { get; } =
            new StructRequirementType<DateTime>(
                Guid.Parse(DateTimeRequirementType.Id),
                DefaultMethods.TryCastStructValue,
                DefaultMethods.TryCastStructCollection);
    }
}

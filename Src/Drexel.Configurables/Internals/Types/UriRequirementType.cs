using System;
using Drexel.Configurables.Contracts.Classes;

namespace Drexel.Configurables.Internals.Types
{
    internal static class UriRequirementType
    {
        private static readonly string Id = @"0889D9B5-68F5-4721-BE0C-15A448045A22";

        public static ClassRequirementType<Uri> Instance { get; } =
            new ClassRequirementType<Uri>(
                Guid.Parse(UriRequirementType.Id),
                DefaultMethods.TryCastClassValue,
                DefaultMethods.TryCastClassCollection);
    }
}

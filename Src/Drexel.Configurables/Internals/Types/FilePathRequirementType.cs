using System;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Internals.Types
{
    internal static class FilePathRequirementType
    {
        private static readonly string Id = @"78EE51D3-43FA-4FF3-9728-DFC16B3AEB34";

        public static ClassRequirementType<FilePath> Instance { get; } =
            new ClassRequirementType<FilePath>(
                Guid.Parse(FilePathRequirementType.Id),
                DefaultMethods.TryCastClassValue,
                DefaultMethods.TryCastClassCollection);
    }
}
